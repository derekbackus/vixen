using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace Vixen3DPreview
{
    public partial class ViewerForm : Form, IDisplayForm
    {
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        private bool _glLoaded = false;
        Stopwatch _fpsStopwatch = new Stopwatch();

        public ViewerForm(Vixen3DPreviewData data)
        {
            InitializeComponent();
            Data = data;
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

            Data.Width = Width;
            Data.Height = Height;
        }

        private void ViewerForm_Move(object sender, EventArgs e)
        {
            if (Data == null)
            {
                Logging.Warn("Vixen3DPreviewDisplay_Move: Data is null. abandoning move. (Thread ID: " +
                                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
                return;
            }

            Data.Top = Top;
            Data.Left = Left;
        }

        private void SetupWindowLocation()
        {
            var minX = Screen.AllScreens.Min(m => m.Bounds.X);
            var maxX = Screen.AllScreens.Sum(m => m.Bounds.Width) + minX;

            var minY = Screen.AllScreens.Min(m => m.Bounds.Y);
            var maxY = Screen.AllScreens.Sum(m => m.Bounds.Height) + minY;

            // avoid 0 with/height in case Data comes in 'bad' -- even small is bad,
            // as it doesn't give a sizeable enough canvas to render on.
            if (Data.Width < 100)
            {
                Data.Width = 400;
            }

            if (Data.SetupWidth < 200)
            {
                Data.SetupWidth = 400;
            }

            if (Data.Height < 100)
            {
                Data.Height = 300;
            }

            if (Data.SetupHeight < 200)
            {
                Data.SetupHeight = 300;
            }

            if (Data.Left < minX || Data.Left > maxX)
                Data.Left = 0;
            if (Data.Top < minY || Data.Top > maxY)
                Data.Top = 0;

            SetDesktopLocation(Data.Left, Data.Top);
            Size = new Size(Data.Width, Data.Height);
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

        public Vixen3DPreviewData Data { get; set; }

        private void glControl_Load(object sender, EventArgs e)
        {
            _glLoaded = true;
            SetupOpenGL();
            _fpsStopwatch.Start();
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
        }

        public void Setup()
        {
            SetupWindowLocation();
        }

        private void SetupOpenGL()
        {
            if (!_glLoaded) return;
            glControl.MakeCurrent();
            GL.ClearColor(Color.Black);
        }

        public void UpdatePreview()
        {
            if (!_glLoaded) return;

            DisplayFPS();

            glControl.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            glControl.SwapBuffers();            
        }





    }
}
