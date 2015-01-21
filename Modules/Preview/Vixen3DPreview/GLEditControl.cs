using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Vixen3DPreview
{

    public delegate void ViewDoubleClickedEventHandler(object sender, EventArgs e);
    
    public partial class GLEditControl : GLControl
    {
        private bool _glLoaded = false;
        private bool _setupComplete = false;
        private ViewTypes _currentView = ViewTypes.Front;
        private Matrix4 _projectionMatrix;
        private Matrix4 _viewMatrix;

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

        #region "Events"

        public GLEditControl()
        {
            InitializeComponent();
            contextMenuStripView.Renderer = new MyMenuRenderer();
        }

        public event ViewDoubleClickedEventHandler ViewDoubleClicked;

        protected virtual void OnViewDoubleClicked(EventArgs e)
        {
            if (ViewDoubleClicked != null)
                ViewDoubleClicked(this, e);
        }
        #endregion

        private void GLEditControl_Load(object sender, EventArgs e)
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

        public Vector3 GetMousePosition()
        {
            var clientPoint = PointToClient(MousePosition);
            return new Vector3(clientPoint.X, clientPoint.Y, 0f);
        }

        public void Render()
        {
            if (!_glLoaded) return;
            if (Context != null)
            {
                if (!_setupComplete) SetupViewport();
                MakeCurrent();
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.MatrixMode(MatrixMode.Modelview);     // To operate on model-view matrix

                // Render a color-cube consisting of 6 quads with different colors
                GL.LoadIdentity(); 

                GL.LineWidth(2.5f);
                GL.Color3(1.0, 0.0, 0.0);
                GL.Begin(PrimitiveType.Lines);
                //GL.Vertex3(-1, -1, 0.0);
                //GL.Vertex3(1, 1, 0);
                GL.Vertex3(0, 0, 0.0);
                GL.Vertex3(Width, Height, 0);
                GL.End();

                SwapBuffers();
            }
        }

        private void GLEditControl_Resize(object sender, EventArgs e)
        {
            SetupViewport();
        }

        private void SetupViewport()
        {
            if (!_glLoaded) return;
            _setupComplete = true;

            MakeCurrent();

            GL.ClearColor(Color.Black);

            GL.Viewport(0, 0, Width, Height);

            float aspect = (float)Width / (float)Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            if (Width >= Height)
            {
                _projectionMatrix = Matrix4.CreateOrthographic(-1.0f * aspect, 1.0f * aspect, -1.0f, 1.0f);
            }
            else
            {
                _projectionMatrix = Matrix4.CreateOrthographic(-1.0f, 1.0f, -1.0f / aspect, 1.0f / aspect);
            }
            GL.LoadMatrix(ref _projectionMatrix);


            //GL.MatrixMode(MatrixMode.Projection);

            //_projectionMatrix = Matrix4.CreateOrthographic(Width, Height, 10, 3000);
            //GL.LoadMatrix(ref _projectionMatrix);

            //_viewMatrix = Matrix4.LookAt(0, 0, 100, 0, 0, 0, 0, 1, 0);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref  _viewMatrix);
        }


    }
}
