using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace Vixen3DPreview
{

    public delegate void ViewDoubleClickedEventHandler(object sender, EventArgs e);
    
    public partial class GLEditControl : UserControl
    {
        private bool _glLoaded = false;
        private bool _setupComplete = false;
        private ViewTypes _currentView = ViewTypes.Front;

        public enum ViewTypes
        {
            Front,
            Back,
            Top,
            Bottom,
            Right,
            Left,
            Perspective
        }

        public GLEditControl()
        {
            InitializeComponent();
            contextMenuStripView.Renderer = new MyMenuRenderer();
        }

        #region "Events"

        public event ViewDoubleClickedEventHandler ViewDoubleClicked;

        protected virtual void OnViewDoubleClicked(EventArgs e)
        {
            if (ViewDoubleClicked != null)
                ViewDoubleClicked(this, e);
        }
        #endregion

        private void GLEditControl_Load(object sender, EventArgs e)
        {

        }

        private void glControl_Load(object sender, EventArgs e)
        {
            _glLoaded = true;
        }

        public ViewTypes CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                labelView.Text = _currentView.ToString();
            }
        }

        private void SetupOpenGL()
        {
            if (!_glLoaded) return;
            _setupComplete = true;

            glControl.MakeCurrent();
            GL.ClearColor(Color.Black);
        }

        public void Render()
        {
            if (!_glLoaded) return;
            if (!_setupComplete) SetupOpenGL();

            if (glControl.Context != null)
            {
                glControl.MakeCurrent();
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                glControl.SwapBuffers();
            }
        }

        private void labelView_DoubleClick(object sender, EventArgs e)
        {
            OnViewDoubleClicked(e);
        }

        private void labelViewButton_Click(object sender, EventArgs e)
        {
            var p = PointToScreen(new Point(labelView.Left, labelView.Top + labelView.Height));
            contextMenuStripView.Show(p);
        }

        private void labelView_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void contextMenuStripView_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == topToolStripMenuItem)
            {
                CurrentView = ViewTypes.Top;;
            }
            else if (e.ClickedItem == bottomToolStripMenuItem)
            {
                CurrentView = ViewTypes.Bottom;
            }
            else if (e.ClickedItem == leftToolStripMenuItem)
            {
                CurrentView = ViewTypes.Left;
            }
            else if (e.ClickedItem == rightToolStripMenuItem)
            {
                CurrentView = ViewTypes.Right;
            }
            else if (e.ClickedItem == frontToolStripMenuItem)
            {
                CurrentView = ViewTypes.Front;
            }
            else if (e.ClickedItem == backToolStripMenuItem)
            {
                CurrentView = ViewTypes.Back;
            }
            else if (e.ClickedItem == perspectiveToolStripMenuItem)
            {
                CurrentView = ViewTypes.Perspective;
            }
        }
    }
}
