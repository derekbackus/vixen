using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vixen3DPreview.Props;
using WeifenLuo.WinFormsUI.Docking;

namespace Vixen3DPreview
{
    public partial class SetupForm_Document : DockContent
    {
        private DocumentLayoutType _documentLayout = DocumentLayoutType.Quad;

        public enum DocumentLayoutType
        {
            Quad, // Default
            DualVertical,
            DualHorizontal,
            Single
        }

        public SetupForm_Document(Vixen3DPreviewData data)
        {
            InitializeComponent();
            Data = data;
            SetupForm = (Parent as SetupForm);
        }

        private Vixen3DPreviewData Data { get; set; }

        private SetupForm SetupForm { get; set; }

        private DocumentLayoutType DocumentLayout
        {
            get { return _documentLayout; }
            set
            {
                _documentLayout = value;
            }
        }

        private void SetupForm_Document_Load(object sender, EventArgs e)
        {
            splitContainerVert.SplitterDistance = splitContainerVert.Height / 2;
            splitContainer1.SplitterDistance = splitContainer1.Width/2;
            splitContainer2.SplitterDistance = splitContainer1.SplitterDistance;

            glEditControl1.CurrentView = GLEditControl.ViewTypes.Front;
            glEditControl2.CurrentView = GLEditControl.ViewTypes.Perspective;
            glEditControl3.CurrentView = GLEditControl.ViewTypes.Left;
            glEditControl4.CurrentView = GLEditControl.ViewTypes.Top;

            Application.Idle += Application_Idle;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            RenderOpenGLControls();
        }

        void RenderOpenGLControls()
        {
            if (!Disposing)
            {
                glEditControl1.Render();
                glEditControl2.Render();
                glEditControl3.Render();
                glEditControl4.Render();
            }
        }

        private void SetupForm_Document_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Idle -= Application_Idle;
        }

        private void SetupForm_Document_Resize(object sender, EventArgs e)
        {
            RenderOpenGLControls();
        }

        private void SetupForm_Document_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void glEditControl_ViewDoubleClicked(object sender, EventArgs e)
        {
            var glEditControl = (sender as GLEditControl);
            if (glEditControl == glEditControl1)
            {
                splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
                splitContainerVert.Panel2Collapsed = splitContainer1.Panel2Collapsed;
            }
            else if (glEditControl == glEditControl2)
            {
                splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
                splitContainerVert.Panel2Collapsed = splitContainer1.Panel1Collapsed;
            }
            else if (glEditControl == glEditControl3)
            {
                splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
                splitContainerVert.Panel1Collapsed = splitContainer2.Panel2Collapsed;
            }
            else if (glEditControl == glEditControl4)
            {
                splitContainer2.Panel1Collapsed = !splitContainer2.Panel1Collapsed;
                splitContainerVert.Panel1Collapsed = splitContainer2.Panel1Collapsed;
            }
        }

        private PropBase _currentProp;
        private GLEditControl _currentGLEditControl;
        private void glEditControl_MouseDown(object sender, MouseEventArgs e)
        {
            _currentGLEditControl = glEditControl1;
            _currentProp = new Line(null, _currentGLEditControl.GetMousePosition());
            Console.WriteLine("MouseDown: " + _currentProp + ":" + _currentGLEditControl.GetMousePosition().Xzy.ToString());
            _currentGLEditControl.Capture = true;
        }

        private void glEditControl_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine("MouseUp: " + _currentProp + ":" + _currentGLEditControl.GetMousePosition().Xzy.ToString());
            glEditControl1.Capture = false;
            _currentProp.CompleteAdd(_currentGLEditControl.GetMousePosition());
            Data.Props.Add(_currentProp);
        }

    }
}
