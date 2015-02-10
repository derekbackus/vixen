using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using Vixen.Data.Flow;
using Vixen.Sys;
using System.Collections.Concurrent;
using VixenModules.Preview.Vixen3DPreview.Props;
using Color = System.Drawing.Color;

namespace VixenModules.Preview.Vixen3DPreview
{
    public partial class ViewerForm : Form, IDisplayForm
    {
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        private bool _glLoaded = false;
        Stopwatch _fpsStopwatch = new Stopwatch();
        PreviewUtils _utils = new PreviewUtils();
        public ConcurrentDictionary<ElementNode, List<PreviewPixel>> NodeToPixel = new ConcurrentDictionary<ElementNode, List<PreviewPixel>>();

        public ViewerForm(Vixen3DPreviewPrivateData data)
        {
            Data = data;
            InitializeComponent();
        }

        #region "Window closing and resize"
        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | 0x200;
                return myCp;
            }
        }

        private void ViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                MessageBox.Show("The preview can only be closed from the Preview Configuration dialog.", "Close",
                                MessageBoxButtons.OKCancel);
                e.Cancel = true;
            }
        }

        private void ViewerForm_Resize(object sender, EventArgs e)
        {
            if (Data == null)
            {
                Logging.Warn("Vixen3DPreviewDisplay_Resize: Data is null. abandoning resize. (Thread ID: " +
                                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
                return;
            }

            _utils.SavePrivateSetting("ViewerWidth", Width);
            _utils.SavePrivateSetting("ViewerHeight", Height);
        }

        private void ViewerForm_Move(object sender, EventArgs e)
        {
            if (Data == null)
            {
                Logging.Warn("Vixen3DPreviewDisplay_Move: Data is null. abandoning move. (Thread ID: " +
                                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
                return;
            }

            _utils.SavePrivateSetting("ViewerTop", Top);
            _utils.SavePrivateSetting("ViewerLeft", Left);
        }

        private void SetupWindowLocation()
        {
            var minX = Screen.AllScreens.Min(m => m.Bounds.X);
            var maxX = Screen.AllScreens.Sum(m => m.Bounds.Width) + minX;

            var minY = Screen.AllScreens.Min(m => m.Bounds.Y);
            var maxY = Screen.AllScreens.Sum(m => m.Bounds.Height) + minY;

            // avoid 0 with/height in case Data comes in 'bad' -- even small is bad,
            // as it doesn't give a sizeable enough canvas to render on.
            if (_utils.GetPrivateSetting("ViewerWidth", 100) < 100)
                _utils.SavePrivateSetting("ViewerWidth", 100);

            if (_utils.GetPrivateSetting("ViewerHeight", 100) < 100)
                _utils.SavePrivateSetting("ViewerHeight", 100);

            var left = _utils.GetPrivateSetting("ViewerLeft", 0);
            if (left < minX || left > maxX)
                _utils.SavePrivateSetting("ViewerLeft", 0);

            var top = _utils.GetPrivateSetting("ViewerTop", 0);
            if (top < minY || top > maxY)
            _utils.SavePrivateSetting("ViewerTop", 0);

            // Set the window location and size before other things get created
            SetDesktopLocation(_utils.GetPrivateSetting("ViewerLeft", 0), _utils.GetPrivateSetting("ViewerTop", 0));
            Size = new Size(_utils.GetPrivateSetting("ViewerWidth", 640), _utils.GetPrivateSetting("ViewerHeight", 400));
        }

        #endregion

        #region "FPS"
        double _fpsAccumulator = 0;
        int _fpsCounter = 0;
        private void DisplayFPS()
        {
            _fpsCounter++;
            _fpsAccumulator += ComputeTimeSliceFPS();
            if (_fpsAccumulator > 1000)
            {
                toolStripStatusLabelFPS.Text = _fpsCounter + " fps";
                _fpsAccumulator -= 1000;
                _fpsCounter = 0; 
            }
        }

        private double ComputeTimeSliceFPS()
        {
            _fpsStopwatch.Stop();
            var timeslice = _fpsStopwatch.Elapsed.TotalMilliseconds;
            _fpsStopwatch.Reset();
            _fpsStopwatch.Start();
            return timeslice;
        }

        #endregion

        public Vixen3DPreviewPrivateData Data { get; set; }

        private void glControl_Load(object sender, EventArgs e)
        {
            _glLoaded = true;
            glControl.Data = Data;
            glControl.Editing = false;
            SetupOpenGL();
            SetupWorld();
            Reload();
            _fpsStopwatch.Start();
        }

        private void SetupWorld()
        {
            glControl.WorldWidth = 12*100;
            glControl.WorldHeight = 12*20;
            glControl.WorldDepth = 12*75;
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
        }

        public void Reload()
        {
            if (NodeToPixel == null)
                throw new System.ArgumentException("PreviewBase.NodeToPixel == null");

            NodeToPixel.Clear();

            if (Data.Props == null)
                throw new System.ArgumentException("Props == null");

            //Console.WriteLine("Reload");
            if (Data.Props != null)
            {
                //Console.WriteLine("Props != null");
                int pixelCount = 0;
                //Console.Write("Prop Count: " + Data.Props.Count);
                foreach (PropBase prop in Data.Props)
                {
                    //Console.WriteLine("Do Prop");
                    prop.Layout();
                    if (prop.Pixels == null)
                        throw new System.ArgumentException("prop.Pixels == null");

                    foreach (PreviewPixel pixel in prop.Pixels)
                    {
                        if (pixel.Node != null)
                        {
                            //Console.WriteLine("pixe.Node != null, adding: " + pixel.Node.Id);
                            pixelCount++;
                            //Console.WriteLine("pixels.Node.ID: " + pixel.Node.Id);
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
                                //Console.WriteLine("pixels.Add: " + pixel.Node.Id);
                                pixels.Add(pixel);
                                NodeToPixel.TryAdd(pixel.Node, pixels);
                            }
                        }
                        else
                        {
                            //Console.WriteLine("pixe.Node == null");
                        }
                    }
                }
            }
        }

        public void Setup()
        {
            SetupWindowLocation();
        }

        private void SetupOpenGL()
        {
            //if (!_glLoaded) return;
            //glControl.MakeCurrent();
            //GL.ClearColor(Color.Black);
        }

        public void UpdatePreview()
        {
            if (!_glLoaded) return;

            DisplayFPS();

            //glControl.MakeCurrent();
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            IEnumerable<Element> elementArray = VixenSystem.Elements.Where(e => e.State.Any());
            if (!elementArray.Any())
            {
            }

            try
            {
                glControl.StartRender();
                var po = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                //Console.WriteLine("1");
                //Parallel.ForEach(elementArray, po, element =>
                foreach (var element in elementArray)
                {
                    ElementNode node = VixenSystem.Elements.GetElementNodeForElement(element);
                    //Console.WriteLine("2");
                    if (node != null)
                    {
                        List<PreviewPixel> pixels;
                        //Console.WriteLine("Looking for NodeToPixel: " + NodeToPixel.Count());
                        if (NodeToPixel.TryGetValue(node, out pixels))
                        {
                            //Console.WriteLine("Found NodeToPixel");
                            foreach (PreviewPixel pixel in pixels)
                            {
                                //pixel.SetColorsFromIntents(element.State);
                                glControl.DrawPixel(pixel, element.State);
                                //pixel.Draw(gdiControl.FastPixel, element.State);
                                //pixel.Color = element.State
                            }
                        }
                    }
                    //});
                }
            }
            catch (Exception e)
            {
                Logging.Error(e.Message, e);
            }
            finally
            {
                glControl.EndRender();
            }

            //glControl.Render();
            //glControl.SwapBuffers();

        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                glControl.StartPan();
            }
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                glControl.Pan();
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                glControl.EndPan();
            }
        }

    }
}
