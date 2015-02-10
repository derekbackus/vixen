using System;
using System.Drawing;
using System.Windows.Forms;
using VixenModules.Preview.Vixen3DPreview.Props;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Preview.Vixen3DPreview
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

        public SetupForm_Document(Vixen3DPreviewPrivateData data, SetupForm setupForm)
        {
            InitializeComponent();
            Data = data;
            SetupForm = setupForm;
        }

        private Vixen3DPreviewPrivateData Data { get; set; }

        private SetupForm SetupForm { get; set; }

        public float WorldWidth {
            get { return glEditControl1.WorldWidth; }
            set
            {
                glEditControl1.WorldWidth = value;
                glEditControl2.WorldWidth = value;
                glEditControl3.WorldWidth = value;
                glEditControl4.WorldWidth = value;
            }
        }

        public float WorldHeight
        {
            get { return glEditControl1.WorldHeight; }
            set
            {
                glEditControl1.WorldHeight = value;
                glEditControl2.WorldHeight = value;
                glEditControl3.WorldHeight = value;
                glEditControl4.WorldHeight = value;                
            }
        }

        public float WorldDepth
        {
            get { return glEditControl1.WorldDepth; }
            set
            {
                glEditControl1.WorldDepth = value;
                glEditControl2.WorldDepth = value;
                glEditControl3.WorldDepth = value;
                glEditControl4.WorldDepth = value;                
            }
        }

        public DocumentLayoutType DocumentLayout
        {
            get { return _documentLayout; }
            set
            {
                _documentLayout = value;
                switch (_documentLayout)
                {
                    case DocumentLayoutType.Single:
                        splitContainerVert.Panel2Collapsed = true;
                        splitContainer1.Panel2Collapsed = true;
                        break;
                    case DocumentLayoutType.Quad:
                        splitContainerVert.Panel1Collapsed = false;
                        splitContainerVert.Panel2Collapsed = false;
                        splitContainer1.Panel1Collapsed = false;
                        splitContainer1.Panel2Collapsed = false;
                        splitContainer2.Panel1Collapsed = false;
                        splitContainer2.Panel2Collapsed = false;
                        break;
                    case DocumentLayoutType.DualHorizontal:
                        splitContainerVert.Panel1Collapsed = false;
                        splitContainerVert.Panel2Collapsed = false;
                        splitContainer1.Panel1Collapsed = false;
                        splitContainer1.Panel2Collapsed = true;
                        splitContainer2.Panel1Collapsed = false;
                        splitContainer2.Panel2Collapsed = true;
                        break;
                    case DocumentLayoutType.DualVertical:
                        splitContainerVert.Panel1Collapsed = false;
                        splitContainerVert.Panel2Collapsed = true;
                        splitContainer1.Panel1Collapsed = false;
                        splitContainer1.Panel2Collapsed = false;
                        splitContainer2.Panel1Collapsed = true;
                        splitContainer2.Panel2Collapsed = true;
                        break;
                }
            }
        }

        public bool ShowBoundingBox {
            get { return glEditControl1.ShowBoundingBox; }
            set
            {
                glEditControl1.ShowBoundingBox = value;
                glEditControl2.ShowBoundingBox = value;
                glEditControl3.ShowBoundingBox = value;
                glEditControl4.ShowBoundingBox = value;
            }
        }

        public bool ShowOrigin
        {
            get { return glEditControl1.ShowOrigin; }
            set
            {
                glEditControl1.ShowOrigin = value;
                glEditControl2.ShowOrigin = value;
                glEditControl3.ShowOrigin = value;
                glEditControl4.ShowOrigin = value;
            }
        }

        private void SetupForm_Document_Load(object sender, EventArgs e)
        {
            glEditControl1.Data = Data;
            glEditControl2.Data = Data;
            glEditControl3.Data = Data;
            glEditControl4.Data = Data;
            glEditControl1.Editing = true;
            glEditControl2.Editing = true;
            glEditControl3.Editing = true;
            glEditControl4.Editing = true;
            glEditControl1.SetupForm = SetupForm;
            glEditControl2.SetupForm = SetupForm;
            glEditControl3.SetupForm = SetupForm;
            glEditControl4.SetupForm = SetupForm;
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
            //Console.WriteLine(Name + ".Resize");
            RenderOpenGLControls();
        }

        private void SetupForm_Document_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void glEditControl_ViewDoubleClicked(object sender, EventArgs e)
        {
            var glEditControl = (sender as GLEditControl);
            bool isCollapsed;
            if (glEditControl == glEditControl1)
            {
                switch (DocumentLayout)
                {
                    case DocumentLayoutType.Single:
                        // Do nothing -- we're already in single mode
                        break;
                    case DocumentLayoutType.Quad:
                        splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
                        splitContainerVert.Panel2Collapsed = splitContainer1.Panel2Collapsed;
                        break;
                    case DocumentLayoutType.DualHorizontal:
                        splitContainerVert.Panel1Collapsed = false;
                        splitContainerVert.Panel2Collapsed = !splitContainerVert.Panel2Collapsed;
                        splitContainer1.Panel1Collapsed = false;
                        splitContainer1.Panel2Collapsed = true;
                        splitContainer2.Panel1Collapsed = false;
                        splitContainer2.Panel2Collapsed = true;
                        break;
                    case DocumentLayoutType.DualVertical:
                        splitContainerVert.Panel1Collapsed = false;
                        splitContainerVert.Panel2Collapsed = true;
                        splitContainer1.Panel1Collapsed = false;
                        splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
                        splitContainer2.Panel1Collapsed = true;
                        splitContainer2.Panel2Collapsed = true;
                        break;
                }
            }
            else if (glEditControl == glEditControl2)
            {
                splitContainerVert.Panel1Collapsed = false;
                splitContainer1.Panel2Collapsed = false;

                switch (DocumentLayout)
                {
                    case DocumentLayoutType.Single:
                        // Never Happens
                        break;
                    case DocumentLayoutType.Quad:
                        isCollapsed = splitContainer1.Panel1Collapsed;
                        splitContainerVert.Panel2Collapsed = !isCollapsed;
                        splitContainer1.Panel1Collapsed = !isCollapsed;
                        splitContainer2.Panel1Collapsed = !isCollapsed;
                        splitContainer2.Panel2Collapsed = !isCollapsed;
                        break;
                    case DocumentLayoutType.DualHorizontal:
                        isCollapsed = splitContainerVert.Panel2Collapsed;
                        splitContainerVert.Panel2Collapsed = !isCollapsed;
                        break;
                    case DocumentLayoutType.DualVertical:
                        isCollapsed = splitContainer1.Panel1Collapsed;
                        splitContainer1.Panel1Collapsed = !isCollapsed;
                        break;
                }
            }
            else if (glEditControl == glEditControl3)
            {
                splitContainerVert.Panel2Collapsed = false;
                splitContainer2.Panel1Collapsed = false;

                switch (DocumentLayout)
                {
                    case DocumentLayoutType.Quad:
                        isCollapsed = splitContainerVert.Panel1Collapsed;
                        splitContainerVert.Panel1Collapsed = !isCollapsed;
                        splitContainer2.Panel2Collapsed = !isCollapsed;
                        break;
                    case DocumentLayoutType.DualHorizontal:
                        isCollapsed = splitContainerVert.Panel1Collapsed;
                        splitContainerVert.Panel1Collapsed = !isCollapsed;
                        break;
                    case DocumentLayoutType.Single:
                    case DocumentLayoutType.DualVertical:
                        // Never happens
                        break;
                }
            }
            else if (glEditControl == glEditControl4)
            {
                splitContainerVert.Panel1Collapsed = false;
                splitContainer2.Panel2Collapsed = false;
                isCollapsed = splitContainer2.Panel1Collapsed;
                splitContainerVert.Panel1Collapsed = !isCollapsed;
                splitContainer2.Panel1Collapsed = !isCollapsed;
                splitContainer1.Panel1Collapsed = !isCollapsed;
                splitContainer1.Panel2Collapsed = !isCollapsed;
            }
        }

        //private PropBase _currentProp;
        //private GLEditControl _currentGLEditControl;
        //private Point _mouseStartPos;
        private void glEditControl_MouseDown(object sender, MouseEventArgs e)
        {
            //_currentGLEditControl = (sender as GLEditControl);
            //if (e.Button == MouseButtons.Left)
            //{
            //    if (_currentGLEditControl.CurrentView != GLEditControl.ViewTypes.Perspective)
            //    {
            //        AddCurrentlySelectedPropType();
            //    }
            //}
        }

        private void glEditControl1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left && _currentGLEditControl != null && 
            //    _currentGLEditControl.Capture && _currentProp != null)
            //{
            //    var mousePosition = _currentGLEditControl.GetMousePosition();
            //    _currentProp.SetSelectedPointPosition(mousePosition);
            //    _currentGLEditControl.Render();
            //}
        }

        private void glEditControl_MouseUp(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left && _currentGLEditControl != null &&
            //    _currentGLEditControl.Capture && _currentProp != null)
            //{
            //    _currentGLEditControl.Capture = false;
            //    _currentProp.CompleteAdd(_currentGLEditControl.GetMousePosition());
            //    _currentProp = null;
            //    SetupForm.SelectPropButton("Select");
            //}
        }

        private void SetupForm_Document_Shown(object sender, EventArgs e)
        {
            splitContainerVert.SplitterDistance = splitContainerVert.Height / 2;
            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
            splitContainer2.SplitterDistance = splitContainer1.SplitterDistance;

            glEditControl1.CurrentView = GLEditControl.ViewTypes.Front;
            glEditControl2.CurrentView = GLEditControl.ViewTypes.Perspective;
            glEditControl3.CurrentView = GLEditControl.ViewTypes.Left;
            glEditControl4.CurrentView = GLEditControl.ViewTypes.Top;

            glEditControl1.SetInitialZoomLevel();
            glEditControl2.SetInitialZoomLevel();
            glEditControl3.SetInitialZoomLevel();
            glEditControl4.SetInitialZoomLevel();
        }

        //private void AddCurrentlySelectedPropType()
        //{
        //    if (!string.IsNullOrEmpty(SetupForm.CurrentPropTypeName))
        //    {
        //        _currentProp = null;
        //        switch (SetupForm.CurrentPropTypeName)
        //        {
        //            case "Line":
        //                _currentProp = new Line(SetupForm.ElementsForm.SelectedNode, _currentGLEditControl.GetMousePosition());
        //                break;
        //        }
        //        if (_currentProp != null)
        //        {
        //            Data.Props.Add(_currentProp);
        //            _currentGLEditControl.Capture = true;
        //            _currentGLEditControl.SelectProp(_currentProp);
        //            _currentProp.SelectDefaultSelectPoint();
        //        }
        //    }
        //}

    }
}
