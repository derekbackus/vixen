﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes;
using System.Linq;
using System.Collections.Concurrent;

namespace VixenModules.Preview.VixenPreview
{
    public partial class OpenGLViewer : Form, IDisplayForm
    {
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        private bool _glLoaded = false;

        private Color4[] _colors;
        private int _quadLen;
        private int _vaoProps;
        private int _vboPropQuads;
        private int _vboPropColors;
        private int _vaoBG;
        private int _vboBGPoints;
        private int _propMvLocation;
        private int _propProjLocation;
        private int _bgProjLocation;
        private int _propShaderProgram;
        private int _bgShaderProgram;
        private const int[] NullData = null;
        private int _colorAttribute;
        private Stopwatch _stopWatch = new Stopwatch();

        private const string VertexShaderSource =
            "#version 330\n" +
            "in vec4 vertex_position;" +
            "in vec4 vertex_color;" +
            "out vec2 texture_coordinates;" +
            "" +
            "uniform mat4 mv_matrix;" +
            "uniform mat4 proj_matrix;" +
            "" +
            "out vec4 color;" +
            "void main() {" +
            "   gl_Position = proj_matrix * mv_matrix * vertex_position;" +
            "   color = vertex_color;" +
            "   int vertexID = gl_VertexID;" +
            "   float vertexNum = mod(vertexID, 4.0);" + 
            "   if (vertexNum == 0)" + 
            "   {" +
            "       texture_coordinates = vec2(0,0);" +
            "   }" +
            "   else if (vertexNum == 1)" +
            "   {" +
            "       texture_coordinates = vec2(1,0);" +
            "   }" +
            "   else if (vertexNum == 2)" +
            "   {" +
            "       texture_coordinates = vec2(1,1);" +
            "   }" +
            "   else if (vertexNum == 3)" +
            "   {" +
            "       texture_coordinates = vec2(0,1);" +
            "   }" +
            "}";

        private const string FragmentShaderSource =
            "#version 330\n" +
            "in vec4 color;" +
            "in vec2 texture_coordinates;" +
            "out vec4 out_color;" +
            "void main() {" +
            // Discard the color information if it is black.
            "   if (color.rgb == vec3(0f,0f,0f))" +
            "       discard;" +
            // We need to make a pretty circle. Otherwise, our whole quad will be colored.
            "   float d = distance(texture_coordinates, vec2(0.5, 0.5));" +
            "   float radius = .5; " +
            //discard if the distance is bigger than radius, otherwise draw the pixel
            "   if (d > radius)" +
            "       discard;" +
            "   out_color = color;" +
            "}";

        private const string BGVertexShaderSource =
            "#version 330\n" +
            "in vec4 vertex_position;" +
            "uniform mat4 proj_matrix;" +
            "in vec2 texCoords;"+
            "out vec2 texture_coordinates;"+
            "void main ()" + 
            "{" +
            "   texture_coordinates = texCoords;" +
            "   gl_Position = proj_matrix * vertex_position;" +
            "}";

        private const string BGFragmentShaderSource =
            "#version 330\n" +
            "" +
            "" +
            "in vec2 texture_coordinates;" +
            "uniform sampler2D tex;" +
            "" +
            "void main()" +
            "{" +
            "   gl_FragColor = texture2D(tex, texture_coordinates);" +
            "}";

        public OpenGLViewer(VixenPreviewData data)
		{
            Data = data;
            InitializeComponent();
		}

        public VixenPreviewData Data { get; set; }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        double _accumulator = 0;
        int _idleCounter = 0;
        private void Accumulate(double milliseconds)
        {
            _idleCounter++;
            _accumulator += milliseconds;
            if (_accumulator > 1000)
            {
                toolStripStatusFPS.Text = _idleCounter.ToString() + " fps";
                _accumulator -= 1000;
                _idleCounter = 0; 
            }
        }

        private bool _showPropsWhenInactive = new Properties.Settings().ShowPropsWhenInactive;
        private bool ShowPropsWhenInactive
        {
            get { return _showPropsWhenInactive; }
            set
            {
                var settings = new Properties.Settings();
                settings.ShowPropsWhenInactive = value;
                settings.Save();
                _showPropsWhenInactive = value;
            }
        }

        private double ComputeTimeSlice()
        {
            _stopWatch.Stop();
            double timeslice = _stopWatch.Elapsed.TotalMilliseconds;
            _stopWatch.Reset();
            _stopWatch.Start();
            return timeslice;
        }

        public void UpdatePreview()
        {
            double milliseconds = ComputeTimeSlice();
            Accumulate(milliseconds);

            IEnumerable<Element> elementArray = VixenSystem.Elements.Where(e => e.State.Any());

            if (!elementArray.Any() && ShowPropsWhenInactive)
            {
                for (var i = 0; i < _colors.Length; i++)
                {
                    _colors[i].R = .3f;
                    _colors[i].G = .3f;
                    _colors[i].B = .3f;
                    _colors[i].A = 1f;
                }
                Render();
                return;
            }

            // Set all of our colors to black/transparent
            for (var i = 0; i < _colors.Length; i++)
            {
                _colors[i].R = 0f;
                _colors[i].G = 0f;
                _colors[i].B = 0f;
                _colors[i].A = 0f;
            }

            try
            {
                //var po = new ParallelOptions
                //{
                //    MaxDegreeOfParallelism = Environment.ProcessorCount
                //};

                // Parallel just doesn't seem to help here...
                //Parallel.ForEach(elementArray, po, element =>
                foreach (var element in elementArray)
                {
                    ElementNode node = VixenSystem.Elements.GetElementNodeForElement(element);
                    if (node != null)
                    {
                        List<PreviewPixel> pixels;
                        if (NodeToPixel.TryGetValue(node, out pixels))
                        {
                            foreach (PreviewPixel pixel in pixels)
                            {
                                List<System.Drawing.Color> elementColors = pixel.IntentColors(element.State);

                                int arrayPosOffset = 0;
                                foreach (var color in elementColors)
                                {
                                    // Need to set the color of each of the 4 quad vertices
                                    for (var vertexNum = 0; vertexNum < 4; vertexNum++)
                                    {
                                        int colorPosition = ((pixel.GLArrayPosition + arrayPosOffset) * 4) + vertexNum;
                                        _colors[colorPosition].R = (float) color.R/byte.MaxValue;
                                        _colors[colorPosition].G = (float) color.G/byte.MaxValue;
                                        _colors[colorPosition].B = (float) color.B/byte.MaxValue;
                                        _colors[colorPosition].A = (float) color.A/byte.MaxValue;
                                    }
                                    arrayPosOffset++;
                                }
                            }
                        }
                    }
                //        });
                }
            }
            catch (Exception e)
            {
                Logging.Error(e.Message, e);
            }

            Render();
        }

        public void Setup()
        {
            var minX = Screen.AllScreens.Min(m => m.Bounds.X);
            var maxX = Screen.AllScreens.Sum(m => m.Bounds.Width) + minX;

            var minY = Screen.AllScreens.Min(m => m.Bounds.Y);
            var maxY = Screen.AllScreens.Sum(m => m.Bounds.Height) + minY;

            // avoid 0 with/height in case Data comes in 'bad' -- even small is bad,
            // as it doesn't give a sizeable enough canvas to render on.
            if (Data.Width < 300)
            {
                //if (Background != null && Background.Width > 300)
                //    Data.Width = Background.Width;
                //else
                    Data.Width = 400;
            }

            if (Data.SetupWidth < 300)
            {
                //if (Background != null && Background.Width > 300)
                //    Data.SetupWidth = Background.Width;
                //else
                    Data.SetupWidth = 400;
            }

            if (Data.Height < 100)
            {
                //if (Background != null && Background.Height > 200)
                //    Data.Height = Background.Height;
                //else
                    Data.Height = 100;
            }

            if (Data.SetupHeight < 100)
            {
                //if (Background != null && Background.Height > 200)
                //    Data.SetupHeight = Background.Height;
                //else
                    Data.SetupHeight = 100;
            }

            if (Data.Left < minX || Data.Left > maxX)
                Data.Left = 0;
            if (Data.Top < minY || Data.Top > maxY)
                Data.Top = 0;

            SetDesktopLocation(Data.Left, Data.Top);
            Size = new Size(Data.Width, Data.Height);
        }


        public ConcurrentDictionary<ElementNode, List<PreviewPixel>> NodeToPixel = new ConcurrentDictionary<ElementNode, List<PreviewPixel>>();
        public List<DisplayItem> DisplayItems
        {
            get
            {
                if (Data != null)
                {
                    return Data.DisplayItems;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Reload()
        {
            if (NodeToPixel == null)
                throw new System.ArgumentException("PreviewBase.NodeToPixel == null");

            NodeToPixel.Clear();

            if (DisplayItems == null)
                throw new System.ArgumentException("DisplayItems == null");

            if (DisplayItems != null)
            {
                var pixelCount = 0;
                foreach (DisplayItem item in DisplayItems)
                {
                    item.Shape.Layout();
                    if (item.Shape.Pixels == null)
                        throw new System.ArgumentException("item.Shape.Pixels == null");

                    foreach (var pixel in item.Shape.Pixels.Where(pixel => pixel.Node != null))
                    {
                        pixelCount++;
                        List<PreviewPixel> pixels;
                        if (NodeToPixel.TryGetValue(pixel.Node, out pixels))
                        {
                            if (!pixels.Contains(pixel))
                            {
                                pixels.Add(pixel);
                            }
                        }
                        else
                        {
                            pixels = new List<PreviewPixel> {pixel};
                            NodeToPixel.TryAdd(pixel.Node, pixels);
                        }
                    }
                }
                toolStripStatusPixels.Text = pixelCount + " lights";
            }

            CreateAlphaBackground();
            AddPropsToViewport();
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            _glLoaded = true;

            int major, minor;
            GL.GetInteger(GetPName.MajorVersion, out major);
            GL.GetInteger(GetPName.MinorVersion, out minor);
            toolStripStatusLabelOpenGLVersion.Text = "OpenGL Version: " + major + "." + minor;

            Reload();

            _stopWatch.Start();
        }

        private void OpenGLViewer_Resize(object sender, EventArgs e)
        {
            if (!_glLoaded) return;

            if (Data == null)
            {
                Logging.Warn("VixenPreviewDisplay_Resize: Data is null. abandoning resize. (Thread ID: " +
                                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
                return;
            }

            Data.Width = Width;
            Data.Height = Height;

            ResizeViewport();
        }

        private void OpenGLViewer_Move(object sender, EventArgs e)
        {
            if (Data == null)
            {
                Logging.Warn("VixenPreviewDisplay_Move: Data is null. abandoning move. (Thread ID: " +
                                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
                return;
            }

            Data.Top = Top;
            Data.Left = Left;
        }

        private void OpenGLViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            MessageBox.Show("The preview can only be closed from the Preview Configuration dialog.", "Close",
                MessageBoxButtons.OKCancel);
            e.Cancel = true;
        }

        private void AddPropsToViewport()
        {
            SetupPropsVAO();
            SetupBackgroundVAO();
            SetPropsProjectionMatrix();
            SetBackgroundProjectionMatrix();

            glControl.MakeCurrent();

            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            // Enable alpha blending
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            // Enable textures
            GL.Enable(EnableCap.Texture2D);
            // Enable depth testing
            GL.Enable(EnableCap.DepthTest);
            // Enable variable point size
            GL.Enable(EnableCap.VertexProgramPointSize);
        }

        private void ResizeViewport()
        {
            glControl.MakeCurrent();

            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            SetPropsProjectionMatrix();
            SetBackgroundProjectionMatrix();
        }

        private void AddQuad(ref Vector4[] quads, int pointNum, int size, float x, float y, float z, float w)
        {
            var sizeToGrow = size;
            var pointPos = pointNum*4;
            // Offset this quad so our vertexes are in the center
            var offset = -(sizeToGrow / 2);
            x += offset;
            y += offset;
            quads[pointPos].Wxyz = new Vector4(w, x, y, z);
            quads[pointPos + 1].Wxyz = new Vector4(w, x + sizeToGrow, y, z);
            quads[pointPos + 2].Wxyz = new Vector4(w, x + sizeToGrow, y + sizeToGrow, z);
            quads[pointPos + 3].Wxyz = new Vector4(w, x, y + sizeToGrow, z);
        }

        private void SetupPropsVAO()
        {
            if (!_glLoaded) return;

            glControl.MakeCurrent();

            // Figure out how much memory we need!
            var numPoints = 0;
            foreach (var pixel in NodeToPixel.SelectMany(pixels => pixels.Value))
            {
                if (pixel.IsDiscreteColored)
                {
                    var elementColors =
                        Property.Color.ColorModule.getValidColorsForElementNode(pixel.Node, false);
                    numPoints += elementColors.Count();
                }
                else
                {
                    numPoints++;
                }
            }

            var quads = new Vector4[numPoints * 4];
            _colors = new Color4[numPoints * 4];
            var pointSizes = new float[numPoints];

            float x, y;
            float z = 0f;
            float w = 1f;
            var pointNum = 0;
            foreach (var pixel in NodeToPixel.SelectMany(pixels => pixels.Value))
            {
                pixel.GLArrayPosition = pointNum;

                if (pixel.IsDiscreteColored)
                {
                    var elementColors =
                        VixenModules.Property.Color.ColorModule.getValidColorsForElementNode(pixel.Node, false);
                    var origY = 0f;
                    var lastX = 0f;
                    var lastY = 0f;
                    for (var i = 0; i < elementColors.Count(); i++)
                    {
                        pointSizes[pointNum] = pixel.PixelSize;
                        if (i == 0)
                        {
                            origY = pixel.Y;
                            x = (float) pixel.X;
                            y = (float) pixel.Y;
                        }
                        else if (i%2 == 0)
                        {
                            x = lastX;
                            y = origY + pixel.PixelSize;
                        }
                        else
                        {
                            x = lastX + pixel.PixelSize;
                            y = lastY;
                        }
                        AddQuad(ref quads, pointNum, pixel.PixelSize, x, y, z, w);
                        pointNum++;
                        lastX = x;
                        lastY = y;
                    }
                }
                else
                {
                    AddQuad(ref quads, pointNum, pixel.PixelSize, pixel.X, pixel.Y, z, w);
                    pointNum++;
                }
            }

            // Create a vertex array object for our props
            _vaoProps = GL.GenVertexArray();
            // Bind it (set it active)
            GL.BindVertexArray(_vaoProps);

            var vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, VertexShaderSource);
            GL.CompileShader(vs);

            var fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, FragmentShaderSource);
            GL.CompileShader(fs);

            _propShaderProgram = GL.CreateProgram();
            GL.AttachShader(_propShaderProgram, fs);
            GL.AttachShader(_propShaderProgram, vs);
            GL.LinkProgram(_propShaderProgram);
            GL.UseProgram(_propShaderProgram);

            var positionAttribute = GL.GetAttribLocation(_propShaderProgram, "vertex_position");
            _quadLen = quads.Length * Vector4.SizeInBytes;
            _vboPropQuads = GL.GenBuffer();
            _colorAttribute = GL.GetAttribLocation(_propShaderProgram, "vertex_color");
            var colorsLen = _colors.Length * Vector4.SizeInBytes;
            _vboPropColors = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPropQuads);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_quadLen), quads, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPropColors);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(colorsLen), _colors, BufferUsageHint.StaticDraw);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPropQuads);
            GL.VertexAttribPointer(positionAttribute, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPropColors);
            GL.VertexAttribPointer(_colorAttribute, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(positionAttribute);
            GL.EnableVertexAttribArray(_colorAttribute);

            //
            // Setup the Matrix for projection, rotation, etc.
            //
            _propMvLocation = GL.GetUniformLocation(_propShaderProgram, "mv_matrix");
            _propProjLocation = GL.GetUniformLocation(_propShaderProgram, "proj_matrix");
        }

        private void SetPropsProjectionMatrix()
        {
            glControl.MakeCurrent();

            GL.UseProgram(_propShaderProgram);

            var modelViewMatrix = Matrix4.Identity;
            GL.UniformMatrix4(_propMvLocation, false, ref modelViewMatrix);

            var projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, glControl.Width, glControl.Height, 0, 0.0f, 100f);
            GL.UniformMatrix4(_propProjLocation, false, ref projectionMatrix);            
        }
        
        private void SetupBackgroundVAO()
        {
            if (!_glLoaded) return;

            glControl.MakeCurrent();

            var bmpWidth = 0;
            var bmpHeight = 0;
            LoadBackgroundTexture(out bmpWidth, out bmpHeight);

            if (bmpWidth > 0 && bmpHeight > 0)
            {
                var vs = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(vs, BGVertexShaderSource);
                GL.CompileShader(vs);
                var fs = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(fs, BGFragmentShaderSource);
                GL.CompileShader(fs);

                _bgShaderProgram = GL.CreateProgram();
                GL.AttachShader(_bgShaderProgram, vs);
                GL.AttachShader(_bgShaderProgram, fs);
                GL.LinkProgram(_bgShaderProgram);
                GL.UseProgram(_bgShaderProgram);

                // Create the vao for the background geometry
                _vaoBG = GL.GenVertexArray();
                GL.BindVertexArray(_vaoBG);

                var bgPoints = new Vector4[]
                {
                    new Vector4(0f, 0f, -10f, 1f),
                    new Vector4(bmpWidth, 0, -10f, 1f),
                    new Vector4(bmpWidth, bmpHeight, -10f, 1f),
                    new Vector4(0f, bmpHeight, -10f, 1f),
                };

                var bgPointsLen = bgPoints.Length*Vector4.SizeInBytes;
                var positionAttribute = GL.GetAttribLocation(_bgShaderProgram, "vertex_position");

                _vboBGPoints = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboBGPoints);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(bgPointsLen), bgPoints, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboBGPoints);
                GL.VertexAttribPointer(positionAttribute, 4, VertexAttribPointerType.Float, false, 0, 0);

                GL.EnableVertexAttribArray(positionAttribute);

                // OpenGL needs texture coordinates to place the texture (picture) on the geometry
                float[] texCoords =
                {
                    0.0f, 0.0f,
                    1.0f, 0.0f,
                    1.0f, 1.0f,
                    0.0f, 1.0f,
                };
                var texCoordsLen = texCoords.Length*sizeof (float);
                var texCoordsLocation = GL.GetAttribLocation(_bgShaderProgram, "texCoords");
                var texVBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, texVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(texCoordsLen), texCoords, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, texVBO);
                GL.VertexAttribPointer(texCoordsLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(texCoordsLocation);


                //
                // Setup the Matrix for projection, rotation, etc.
                //
                _bgProjLocation = GL.GetUniformLocation(_bgShaderProgram, "proj_matrix");
            }
        }

        private void SetBackgroundProjectionMatrix()
        {
            glControl.MakeCurrent();
            GL.UseProgram(_bgShaderProgram);
            var projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, glControl.Width, glControl.Height, 0, 0.0f,
                100f);
            GL.UniformMatrix4(_bgProjLocation, false, ref projectionMatrix);
        }

        private void Render()
        {
            if (!_glLoaded) return;

            glControl.MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Display the background
            GL.BindVertexArray(_vaoBG);
            GL.UseProgram(_bgShaderProgram);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            // Display the props
            var colorsLen = _colors.Length * Vector4.SizeInBytes;
            // Set our program for this output
            GL.UseProgram(_propShaderProgram);
            GL.BindVertexArray(_vaoProps);
            // Send the new colors to the GPU
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPropColors);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(0), new IntPtr(colorsLen), _colors);
            // Finally, draw the points
            GL.DrawArrays(PrimitiveType.Quads, 0, _quadLen);
            glControl.SwapBuffers();
        }

        public Bitmap CreateAlphaBackground()
        {
            Bitmap backgroundAlphaImage = null;
            try
            {
                if (System.IO.File.Exists(Data.BackgroundFileName))
                {
                    var backgroundImage = new Bitmap(Data.BackgroundFileName);
                    System.Drawing.Color c;
                    c = System.Drawing.Color.FromArgb(255 - Data.BackgroundAlpha, 0, 0, 0);

                    backgroundAlphaImage = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    Graphics gfx = Graphics.FromImage(backgroundAlphaImage);
                    using (var brush = new SolidBrush(c))
                    {
                        gfx.FillRectangle(Brushes.Black, 0, 0, backgroundAlphaImage.Width, backgroundAlphaImage.Height);
                        gfx.DrawImage(backgroundImage, 0, 0, backgroundImage.Width, backgroundImage.Height);
                        gfx.FillRectangle(brush, 0, 0, backgroundImage.Width, backgroundImage.Height);
                    }
                    gfx.Dispose();
                }
            }
            catch (Exception ex)
            {
                // The iamge was not found, oh no!
            }
            return backgroundAlphaImage;
        }

        private int _textureID = -1;
        public void LoadBackgroundTexture(out int bmpWidth, out int bmpHeight)
        {
            glControl.MakeCurrent();

            _textureID = -1;
            var bmp = CreateAlphaBackground();
            bmpWidth = 0;
            bmpHeight = 0;
            if (bmp != null)
            {
                bmpWidth = bmp.Width;
                bmpHeight = bmp.Height;

                //Generate empty texture
                _textureID = GL.GenTexture();
                //Link empty texture to texture2d
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _textureID);

                //Must be set else the texture will show glColor
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int) TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int) TextureMagFilter.Linear);

                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //Describe to gl what we want the bound texture to look like
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                bmp.UnlockBits(bmp_data);
            }
        }

        private void OpenGLViewer_Load(object sender, EventArgs e)
        {
            var chkBox = new CheckBox();
            chkBox.CheckedChanged += ShowPropsWhenInactive_CheckedChanged;
            chkBox.Text = "Show Props When Inactive";
            var cbItem = new ToolStripControlHost(chkBox);
            cbItem.Alignment = ToolStripItemAlignment.Right;
            statusStrip.Items.Insert(statusStrip.Items.Count-1, cbItem);
            chkBox.Checked = ShowPropsWhenInactive;
        }

        private void ShowPropsWhenInactive_CheckedChanged(Object sender, EventArgs e)
        {
            ShowPropsWhenInactive = (sender as CheckBox).Checked;
        }

    }
}
