using System;
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
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace VixenModules.Preview.VixenPreview
{
    public partial class OpenGLViewer : Form, IDisplayForm
    {
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        private bool _glLoaded = false;

        Vector4[] _points;
        private Color4[] _colors;
        private int[] _vbo = new int[2];
        private int[] _vao = new int[2];
        private int _mvLocation = 0;
        private int _projLocation = 0;

        private const string VertexShaderSource =
            "#version 430\n" +
            "in vec4 vertex_position;" +
            "in vec4 vertex_color;" +
            "" +
            "uniform mat4 mv_matrix;" +
            "uniform mat4 proj_matrix;" +
            "" +
            "out vec4 color;" +
            "void main() {" +
            "   gl_Position = proj_matrix * mv_matrix * vertex_position;" +
            "   color = vertex_color;" +
            "}";

        private const string FragmentShaderSource =
            "#version 430\n" +
            "in vec4 color;" +
            "out vec4 out_color;" +
            "void main() {" +
            "   out_color = color;" +
            "}";

        private int _shaderProgram;

        private Stopwatch _stopWatch = new Stopwatch();
        private int _frames = 0;

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

        double accumulator = 0;
        int idleCounter = 0;
        private void Accumulate(double milliseconds)
        {
            idleCounter++;
            accumulator += milliseconds;
            if (accumulator > 1000)
            {
                toolStripStatusFPS.Text = idleCounter.ToString() + " fps";
                accumulator -= 1000;
                idleCounter = 0; // don't forget to reset the counter!
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

            if (!elementArray.Any())
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
                var po = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                Parallel.ForEach(elementArray, po, element =>
                {
                    ElementNode node = VixenSystem.Elements.GetElementNodeForElement(element);
                    if (node != null)
                    {
                        List<PreviewPixel> pixels;
                        if (NodeToPixel.TryGetValue(node, out pixels))
                        {
                            foreach (PreviewPixel pixel in pixels)
                            {
                                List<System.Drawing.Color> colors = pixel.IntentColors(element.State);

                                if (colors.Any())
                                {
                                    // Need to do something here to deal with discrete colors!
                                    _colors[pixel.GLArrayPosition].R = (float) colors[0].R/byte.MaxValue;
                                    _colors[pixel.GLArrayPosition].G = (float) colors[0].G/byte.MaxValue;
                                    _colors[pixel.GLArrayPosition].B = (float) colors[0].B/byte.MaxValue;
                                    _colors[pixel.GLArrayPosition].A = (float) colors[0].A/byte.MaxValue;
                                }
                            }
                        }
                    }
                });
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
                if (Background != null && Background.Width > 300)
                    Data.Width = Background.Width;
                else
                    Data.Width = 400;
            }

            if (Data.SetupWidth < 300)
            {
                if (Background != null && Background.Width > 300)
                    Data.SetupWidth = Background.Width;
                else
                    Data.SetupWidth = 400;
            }

            if (Data.Height < 200)
            {
                if (Background != null && Background.Height > 200)
                    Data.Height = Background.Height;
                else
                    Data.Height = 300;
            }

            if (Data.SetupHeight < 200)
            {
                if (Background != null && Background.Height > 200)
                    Data.SetupHeight = Background.Height;
                else
                    Data.SetupHeight = 300;
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
            Console.WriteLine("Reload");

            if (NodeToPixel == null)
                throw new System.ArgumentException("PreviewBase.NodeToPixel == null");

            NodeToPixel.Clear();

            if (DisplayItems == null)
                throw new System.ArgumentException("DisplayItems == null");

            if (DisplayItems != null)
            {
                LayoutProps();
                int pixelCount = 0;
                foreach (DisplayItem item in DisplayItems)
                {
                    item.Shape.Layout();
                    if (item.Shape.Pixels == null)
                        throw new System.ArgumentException("item.Shape.Pixels == null");

                    foreach (PreviewPixel pixel in item.Shape.Pixels)
                    {
                        if (pixel.Node != null)
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
                                pixels = new List<PreviewPixel>();
                                pixels.Add(pixel);
                                NodeToPixel.TryAdd(pixel.Node, pixels);
                            }
                        }
                    }
                }
                toolStripStatusPixels.Text = pixelCount.ToString() + " lights";
            }

            //gdiControl.BackgroundAlpha = Data.BackgroundAlpha;
            //if (System.IO.File.Exists(Data.BackgroundFileName))
            //    gdiControl.Background = Image.FromFile(Data.BackgroundFileName);
            //else
            //    gdiControl.Background = null;

            CreateAlphaBackground();

            AddPropsToViewport();
        }

        /// <summary>
        /// Tells every prop to find the location for each pixel
        /// </summary>
        public void LayoutProps()
        {
            Console.WriteLine("LayoutProps");
            if (DisplayItems != null)
            {
                foreach (DisplayItem item in DisplayItems)
                {
                    item.Shape.Layout();
                }
            }
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            _glLoaded = true;

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
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MessageBox.Show("The preview can only be closed from the Preview Configuration dialog.", "Close",
                                MessageBoxButtons.OKCancel);
                e.Cancel = true;
            }
        }

        private void AddPropsToViewport()
        {
            //Console.WriteLine("AddPropsToViewport 1");
            if (!_glLoaded) return;

            int numPoints = NodeToPixel.Count;

            //Console.WriteLine("AddPropsToViewport: " + numPoints);

            Array.Resize(ref _points, numPoints);
            int pointNum = 0;
            foreach (var pixels in NodeToPixel)
            {
                PreviewPixel pixel = pixels.Value[0];
                pixel.GLArrayPosition = pointNum;
                _points[pointNum].X = (float)pixel.X;
                _points[pointNum].Y = (float)pixel.Y;
                _points[pointNum].Z = 0f;
                _points[pointNum].W = 1f;
                pointNum++;
            }

            GL.PointSize(2);

            Array.Resize(ref _colors, numPoints);

            // Create a number of vertex array objects
            GL.GenVertexArrays(_vao.Length, _vao);
            // Bind the first one [0]
            GL.BindVertexArray(_vao[0]);

            // Generate buffers for the _vao
            GL.GenBuffers(_vao.Length, _vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo[0]);
            int pointsLen = _points.Length * Vector4.SizeInBytes;
            int colorsLen = _colors.Length * Vector4.SizeInBytes;
            int[] nullData = null;
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(pointsLen + colorsLen), nullData, BufferUsageHint.StaticDraw);

            // Transfer the vertex positions:
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(0), new IntPtr(pointsLen), _points);
            // Transfer the vertex colors:
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(pointsLen), new IntPtr(colorsLen), _colors);

            var vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, VertexShaderSource);
            GL.CompileShader(vs);

            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, FragmentShaderSource);
            GL.CompileShader(fs);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, fs);
            GL.AttachShader(_shaderProgram, vs);
            GL.LinkProgram(_shaderProgram);
            GL.UseProgram(_shaderProgram);

            // Get the location of the attributes that enters in the vertex shader
            int positionAttribute = GL.GetAttribLocation(_shaderProgram, "vertex_position");
            //Console.WriteLine("position_attribute: " + positionAttribute);
            // Specify how the data for position can be accessed
            GL.VertexAttribPointer(positionAttribute, 4, VertexAttribPointerType.Float, false, 0, 0);
            // Enable the attribute
            GL.EnableVertexAttribArray(positionAttribute);

            // Color attribute
            int colorAttribute = GL.GetAttribLocation(_shaderProgram, "vertex_color");
            //Console.WriteLine("color_attribute: " + colorAttribute);
            GL.VertexAttribPointer(colorAttribute, 4, VertexAttribPointerType.Float, false, 0, pointsLen);
            GL.EnableVertexAttribArray(colorAttribute);

            _mvLocation = GL.GetUniformLocation(_shaderProgram, "mv_matrix");
            _projLocation = GL.GetUniformLocation(_shaderProgram, "proj_matrix");
            //Console.WriteLine("_mv_Location: " + _mvLocation);
            //Console.WriteLine("_projLocation: " + _projLocation);

            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            // Enable alpha blending
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            // Enable textures
            GL.Enable(EnableCap.Texture2D);

            var modelViewMatrix = Matrix4.Identity;
            GL.UniformMatrix4(_mvLocation, false, ref modelViewMatrix);

            var projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, glControl.Width, glControl.Height, 0, 0.0f, 100f);
            GL.UniformMatrix4(_projLocation, false, ref projectionMatrix);
        }

        private void Render()
        {
            if (!_glLoaded) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var pointsLen = _points.Length * Vector4.SizeInBytes;
            var colorsLen = _colors.Length * Vector4.SizeInBytes;

            // We're transffering just the point colors. The points are already in the GPU memory
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(pointsLen), new IntPtr(colorsLen), _colors);

            //GL.Rotate(180, 0f, 1f, 0f);
            GL.DrawArrays(PrimitiveType.Points, 0, _points.Length);

            //GL.BindTexture(TextureTarget.Texture2D, texture);
            //GL.Enable(EnableCap.DepthTest);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Red);
            //GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);

            //GL.TexCoord2(1, 0);
            GL.Vertex2(glControl.Width/2, 0);

            //GL.TexCoord2(1, 1);
            GL.Vertex2(glControl.Width/2, glControl.Height/2);

            //GL.TexCoord2(0, 1);
            GL.Vertex2(0, glControl.Height/2);
            GL.End();

            glControl.SwapBuffers();
        }

        private Image _background;
        public Image Background
        {
            get
            {
                return _background;
            }
            set
            {
                if (value == null)
                {
                    //DefaultBackground = true;
                    _background = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    Graphics gfx = Graphics.FromImage(_background);
                    gfx.Clear(System.Drawing.Color.Black);
                    gfx.Dispose();
                }
                else
                {
                    //DefaultBackground = false;
                    _background = value;
                }
                CreateAlphaBackground();
            }
        }

        private int _backgroundAlpha;
        public int BackgroundAlpha
        {
            get
            {
                return _backgroundAlpha;
            }
            set
            {
                _backgroundAlpha = value;
                if (Background != null) CreateAlphaBackground();
            }
        }

        private Bitmap _backgroundAlphaImage;
        public void CreateAlphaBackground()
        {
            if (Background != null)
            {
            //    System.Drawing.Color c;
            //    c = System.Drawing.Color.FromArgb(255 - BackgroundAlpha, 0, 0, 0);

            //    //_backgroundAlphaImage = new Bitmap(Background.Width, Background.Height, PixelFormat.Format32bppPArgb);
            //    _backgroundAlphaImage = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            //    Graphics gfx = Graphics.FromImage(_backgroundAlphaImage);
            //    using (SolidBrush brush = new SolidBrush(c))
            //    {
            //        gfx.FillRectangle(Brushes.Black, 0, 0, _backgroundAlphaImage.Width, _backgroundAlphaImage.Height);
            //        gfx.DrawImage(Background, 0, 0, Background.Width, Background.Height);
            //        gfx.FillRectangle(brush, 0, 0, Background.Width, Background.Height);
            //    }
            //    gfx.Dispose();
            //}
            //else
            //{
            //    _backgroundAlphaImage = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            //    Graphics gfx = Graphics.FromImage(_backgroundAlphaImage);
            //    gfx.Clear(Color.Black);
            //    gfx.Dispose();
            }
            //_fastPixel = new FastPixel.FastPixel(_backgroundAlphaImage.Width, _backgroundAlphaImage.Height);

            texture = LoadTexture(Data.BackgroundFileName);
            Console.WriteLine("TextureID: " + texture);

        }

        private int texture = 0;
        public int LoadTexture(string file)
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentException(file);

            //Generate empty texture
            int id = GL.GenTexture();
            //Link empty texture to texture2d
            GL.BindTexture(TextureTarget.Texture2D, id);

            //Must be set else the texture will show glColor
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            Bitmap bmp = new Bitmap(file);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //Describe to gl what we want the bound texture to look like
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            return id;
        }

    }
}
