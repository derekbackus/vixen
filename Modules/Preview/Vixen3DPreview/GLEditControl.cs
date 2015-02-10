using System;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Vixen.Sys;
using VixenModules.Preview.Vixen3DPreview.Props;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Drawing.Point;
using Vixen.Intent;
using System.Collections.Generic;

namespace VixenModules.Preview.Vixen3DPreview
{

    public delegate void ViewDoubleClickedEventHandler(object sender, EventArgs e);

    public partial class GLEditControl : GLControl
    {
        private PreviewUtils utils = new PreviewUtils();

        private bool _glLoaded = false;
        private bool _setupComplete = false;
        private Matrix4 _projectionMatrix;
        private Matrix4 _viewMatrix;
        private float _panX = 0;
        private float _panY = 0;
        Matrix4 _identityMatrix = Matrix4.Identity;

        //
        // priate vars for our properties
        //
        private ViewTypes _currentView = ViewTypes.Front;
        private float _worldWidth;
        private float _worldHeight;
        private float _worldDepth;
        private bool _showBoundingBox = true;
        private bool _showOrigin = true;
        private float _originLength = 30;
        private float _originThickness = 2;
        private float _zoomLevel = 1;
        private Color _selectionPointsColor = Color.Yellow;
        private Color _selectedColor = Color.LimeGreen;
        private Color _linkedColor = Color.Turquoise;
        private Color _unlinkedColor = Color.White;
        private List<PropBase> _selectedProps = new List<PropBase>();

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

        public event ViewDoubleClickedEventHandler ViewDoubleClicked;

        protected virtual void OnViewDoubleClicked(EventArgs e)
        {
            if (ViewDoubleClicked != null)
                ViewDoubleClicked(this, e);
        }

        private bool _editing = false;
        public bool Editing
        {
            get { return _editing;  }
            set
            {
                _editing = value;
                //labelView.Visible = false;
                //labelViewButton.Visible = false;
            }
        }

        #region "Properties"

        public SetupForm SetupForm { get; set; }

        public List<PropBase> SelectedProps
        {
            get { return _selectedProps; }
            set { _selectedProps = value; }
        } 

        public Color SelectionPointsColor
        {
            get { return _selectionPointsColor; }
            set { _selectionPointsColor = value; }
        }

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set { _selectedColor = value; }
        }

        public Color LinkedColor
        {
            get { return _linkedColor; }
            set { _linkedColor = value; }
        }

        public Color UnlinkedColor
        {
            get { return _unlinkedColor; }
            set { _linkedColor = value; }
        }

        public Vixen3DPreviewPrivateData Data { get; set; }

        public float PanX
        {
            get { return _panX; }
            set { _panX = value; }
        }

        public float PanY
        {
            get { return _panY; }
            set { _panY = value; }
        }
        
        public float ZoomLevel
        {
            get { return _zoomLevel; }
            set { _zoomLevel = value; }
        }

        /// <summary>
        ///  Our current view. Defaults to Front
        /// </summary>
        public ViewTypes CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                labelView.Text = _currentView.ToString();
                _setupComplete = false;
                //SetupViewport();
            }
        }

        /// <summary>
        /// Witdth of our yard
        /// </summary>
        public float WorldWidth
        {
            get
            {
                if (_worldWidth.Equals(0f))
                {
                    _worldWidth = 300;
                }
                return _worldWidth;
            }
            set { _worldWidth = value; }
        }

        /// <summary>
        /// Height of our yard
        /// </summary>
        public float WorldHeight
        {
            get
            {
                if (_worldHeight.Equals(0f))
                {
                    _worldHeight = 300;
                }
                return _worldHeight;
            }
            set { _worldHeight = value; }
        }

        /// <summary>
        /// Depth of our yard
        /// </summary>
        public float WorldDepth
        {
            get
            {
                if (_worldDepth.Equals(0))
                {
                    _worldDepth = 300;
                }
                return _worldDepth;
            }
            set { _worldDepth = value; }
        }

        /// <summary>
        ///  Our world (yard) is set to a particular size. Turn this on to draw a wireframe box around our yard.
        /// </summary>
        public bool ShowBoundingBox
        {
            get { return _showBoundingBox; }
            set { _showBoundingBox = value; }
        }

        /// <summary>
        /// Do we want to see the origin on our screen?
        /// </summary>
        public bool ShowOrigin
        {
            get { return _showOrigin; }
            set { _showOrigin = value; }
        }

        /// <summary>
        /// The origin is drawn with three lines (xyz). This is the length of those lines in units
        /// </summary>
        public float OriginLength
        {
            get { return _originLength; }
            set { _originLength = value; }
        }

        /// <summary>
        /// The origin is drawn with three lines (xyz). This is the thickness of these lines.
        /// </summary>
        public float OriginThickness
        {
            get { return _originThickness; }
            set { _originThickness = value; }
        }

        #endregion

        private void GLEditControl_Load(object sender, EventArgs e)
        {
            // Can't do anything on the OpenGL surface until it reports that it has been loaded
            _glLoaded = true;

            MouseWheel += GLEditControl_MouseWheel;
        }

        private void GLEditControl_MouseWheel(object sender, MouseEventArgs e)
        {
            //Console.WriteLine(Name + "_MouseWheel: " + e.Delta);
            if (e.Delta > 0)
            {
                ZoomLevel -= .1f;
            }
            else
            {
                ZoomLevel += .1f;
            }
        }

        private Point _mouseStartPos;
        public void StartPan()
        {
            _mouseStartPos = MousePosition;
            Capture = true;
        }

        public void Pan()
        {
            if (Capture)
            {
                PanX += (_mouseStartPos.X - MousePosition.X);
                PanY += (MousePosition.Y - _mouseStartPos.Y);
                _mouseStartPos = new Point(MousePosition.X, MousePosition.Y);
            }
        }

        public void EndPan()
        {
            Capture = false;
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
                CurrentView = ViewTypes.Top;
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

        /// <summary>
        /// Get the mouse position in our world
        /// </summary>
        /// <returns></returns>
        public Vector3 GetMousePosition()
        {
            var clientPoint = PointToClient(MousePosition);
            return ClientToWorld(new Vector3(clientPoint.X, clientPoint.Y, 0));
        }

        /// <summary>
        /// Convert a client point in pixels to a point in our 3D world
        /// </summary>
        /// <param name="clientVector"></param>
        /// <returns></returns>
        public Vector3 ClientToWorld(float x, float y, float z)
        {
            return ClientToWorld(new Vector3(x, y, z));
        }

        /// <summary>
        /// Convert a client point in pixels to a point in our 3D world
        /// </summary>
        /// <param name="clientVector"></param>
        /// <returns></returns>
        public Vector3 ClientToWorld(Vector3 clientVector)
        {
            var x = clientVector.X;
            var y = Height - clientVector.Y;
            var z = clientVector.Z;
            var outVector = new Vector3();
            switch (CurrentView)
            {
                case ViewTypes.Front:
                    outVector = new Vector3(x + PanX, y + PanY, 0);
                    outVector = ZoomVector3(outVector);
                    //outVector.Z = 0;
                    break;
                case ViewTypes.Back:
                    outVector = new Vector3(x + PanX, y + PanY, 0);
                    outVector = ZoomVector3(outVector);
                    outVector.X = WorldWidth - outVector.X;
                    //outVector.Z = -WorldDepth;
                    break;
                case ViewTypes.Perspective:
                    break;
                case ViewTypes.Left:
                    outVector = new Vector3(0, y + PanY, x + PanX);
                    outVector = ZoomVector3(outVector);
                    //outVector.Z -= WorldDepth;
                    break;
                case ViewTypes.Right:
                    outVector = new Vector3(0, y + PanY, x + PanX);
                    outVector = ZoomVector3(outVector);
                    outVector.X = WorldWidth;
                    //outVector.Z *= -1;
                    break;
                case ViewTypes.Top:
                    outVector = new Vector3(x + PanX, 0, y + PanY);
                    outVector = ZoomVector3(outVector);
                    outVector.Z = -outVector.Z;
                    //outVector.Y = WorldHeight;
                    break;
                case ViewTypes.Bottom:
                    outVector = new Vector3(x + PanX, 0, y + PanY);
                    outVector = ZoomVector3(outVector);
                    outVector.Z = outVector.Z - WorldDepth;
                    //outVector.Y = 0;
                    break;
            }
            return outVector;
        }

        /// <summary>
        /// Given a client vector, zoom it in to our world
        /// </summary>
        /// <param name="vectorToZoom"></param>
        /// <returns></returns>
        public Vector3 ZoomVector3(Vector3 vectorToZoom)
        {
            return new Vector3(vectorToZoom.X * ZoomLevel, vectorToZoom.Y * ZoomLevel, vectorToZoom.Z * ZoomLevel);
        }

        public void StartRender()
        {
            if (!_glLoaded || Context == null) return;

            if (!_setupComplete) SetupViewport();

            //Console.WriteLine("StartRender");

            // First, make this GLControl current
            MakeCurrent();
            // Clear the background
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            var aspect = (float) Height/(float) Width;
            var worldAspect = WorldWidth/Width;
            var translatedPanX = _panX*ZoomLevel;
            var translatedPanY = _panY*ZoomLevel;
            //var worldAspect = 1;
            if (CurrentView == ViewTypes.Perspective)
            {
                float perspectiveZoomLevel = 1.5f + ZoomLevel;
                //_projectionMatrix = Matrix4.CreateOrthographic(Width * perspectiveZoomLevel, Width * aspect * perspectiveZoomLevel, -100, WorldDepth + 1000);
                _projectionMatrix = Matrix4.CreateOrthographicOffCenter(translatedPanX*worldAspect,
                    (Width*perspectiveZoomLevel) + (translatedPanX*worldAspect), translatedPanY*worldAspect,
                    (Width*aspect*perspectiveZoomLevel) + (translatedPanY*worldAspect), -100, WorldDepth + 1000);
            }
            else
            {
                _projectionMatrix = Matrix4.CreateOrthographicOffCenter(translatedPanX,
                    (Width*ZoomLevel) + translatedPanX, translatedPanY,
                    (Width*aspect*ZoomLevel) + translatedPanY, -1000, WorldDepth + 1000);
            }
            GL.LoadMatrix(ref _projectionMatrix);
        }

        public void EndRender()
        {
            if (!_glLoaded || Context == null) return;
            //Console.WriteLine("EndRender");

            //// Test cube to see projections
            //GL.Begin(PrimitiveType.Quads);
            //// Front
            //GL.Color3(Color.Blue);
            //GL.Vertex3(200, 200, 0);
            //GL.Vertex3(300, 200, 0);
            //GL.Vertex3(300, 300, 0);
            //GL.Vertex3(200, 300, 0);
            //// Left
            //GL.Color3(Color.Red);
            //GL.Vertex3(200, 200, 0);
            //GL.Vertex3(200, 300, 0);
            //GL.Vertex3(200, 300, -100);
            //GL.Vertex3(200, 200, -100);
            //// Right
            //GL.Color3(Color.Green);
            //GL.Vertex3(300, 200, 0);
            //GL.Vertex3(300, 300, 0);
            //GL.Vertex3(300, 300, -100);
            //GL.Vertex3(300, 200, -100);
            //// Back
            //GL.Color3(Color.BlueViolet);
            //GL.Vertex3(200, 200, -100);
            //GL.Vertex3(300, 200, -100);
            //GL.Vertex3(300, 300, -100);
            //GL.Vertex3(200, 300, -100);
            //// Top
            //GL.Color3(Color.DeepPink);
            //GL.Vertex3(200, 300, 0);
            //GL.Vertex3(300, 300, 0);
            //GL.Vertex3(300, 300, -100);
            //GL.Vertex3(200, 300, -100);
            //// Bottom
            //GL.Color3(Color.Yellow);
            //GL.Vertex3(200, 200, 0);
            //GL.Vertex3(300, 200, 0);
            //GL.Vertex3(300, 200, -100);
            //GL.Vertex3(200, 200, -100);
            //GL.End();

            DrawOrigin();
            DrawBoundingBox();
            DrawFocusRectangle();

            SwapBuffers();
        }

        /// <summary>
        /// Paint our pretty world
        /// </summary>
        public void Render()
        {
            StartRender();
            DrawProps();
            EndRender();
        }

        public void DrawSelectPoints(PropBase prop)
        {
            if (prop.SelectionPoints != null && prop.SelectionPoints.Count > 0)
            {
                foreach (var point in prop.SelectionPoints)
                {
                    GL.PointSize(8);
                    GL.Begin(PrimitiveType.Points);
                    GL.Color4(SelectionPointsColor);
                    GL.Vertex3(point.Location);
                    GL.End();
                }
            }
        }

        public void DrawPixel(PreviewPixel pixel, IIntentStates states)
        {
            if (!_glLoaded || Context == null) return;
            GL.PointSize(pixel.Size);
            //GL.PointSize(4);
            GL.Begin(PrimitiveType.Points);
            if (pixel.DiscreteColored)
            {
                var colors = IntentHelpers.GetAlphaAffectedDiscreteColorsForIntents(states);
                foreach (var color in colors)
                {
                    GL.Color4(color);
                    GL.Vertex3(pixel.Location);
                }
            }
            else
            {
                var color = IntentHelpers.GetAlphaRGBMaxColorForIntents(states);
                GL.Color4(color);
                GL.Vertex3(pixel.Location);
            }

            GL.End();
        }

        /// <summary>
        /// Draw all the props in our world
        /// </summary>
        private void DrawProps()
        {
            if (Data == null) return;
            foreach (var prop in Data.Props)
            {
                if (Editing)
                {
                    // If we're currently adding the prop, tell it to lay itself out.
                    if (prop.Adding)
                    {
                        prop.Layout();
                    }
                    // Iterate through and draw all the pixels
                    foreach (var pixel in prop.Pixels)
                    {
                        if (prop.Selected)
                        {
                            GL.Color3(SelectedColor);
                        } else if (pixel.Node != null) 
                        {
                            GL.Color3(LinkedColor);
                        }
                        else
                        {
                            GL.Color3(UnlinkedColor);
                        }
                        GL.PointSize(4);
                        GL.Begin(PrimitiveType.Points);
                        GL.Vertex3(pixel.Location);
                        GL.End();
                    }
                    DrawSelectPoints(prop);
                }
                else
                {
                    //Console.WriteLine("Render: " + DateTime.Now);
                    // Iterate through and draw all the pixels
                    foreach (var pixel in prop.Pixels)
                    {
                        GL.PointSize(4);
                        GL.Begin(PrimitiveType.Points);
                        if (pixel.Colors != null && pixel.Colors.Length > 0)
                        {
                            //Console.WriteLine("Got Colors");
                            foreach (var color in pixel.Colors)
                            {
                                GL.Color4(color);
                                GL.Vertex3(pixel.Location);
                            }
                        }
                        GL.End();
                    }
                }
            }
        }

        /// <summary>
        /// Draw a rectangle around the control to show we've got focus
        /// </summary>
        private void DrawFocusRectangle()
        {
            if (!Focused) return;

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Ortho(0, Width, 0, Height, 1, -1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            var lineWidth = 4;
            GL.Color3(Color.DodgerBlue);
            GL.LineWidth(lineWidth);
            GL.Begin(PrimitiveType.Lines);
            // Bottom
            GL.Vertex3(0, (lineWidth/2), -1);
            GL.Vertex3(Width, (lineWidth/2), -1);
            // Right
            GL.Vertex3(Width-(lineWidth/2), 0, -1);
            GL.Vertex3(Width - (lineWidth / 2), Height - (lineWidth / 2), -1);
            // Top
            GL.Vertex3(Width, Height - (lineWidth / 2), -1);
            GL.Vertex3(0, Height - (lineWidth / 2), -1);
            // Left
            GL.Vertex3((lineWidth / 2), Height - (lineWidth / 2), -1);
            GL.Vertex3((lineWidth / 2), (lineWidth / 2), 0);
            GL.End();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }

        /// <summary>
        /// Draw the XYZ origin
        /// </summary>
        private void DrawOrigin()
        {
            if (ShowOrigin)
            {
                GL.LineWidth(OriginThickness);
                GL.Begin(PrimitiveType.Lines);
                // X Axis (red)
                GL.Color3(Color.Red);
                GL.Vertex3(0.0f, 0.0f, 0.0f);
                GL.Vertex3(OriginLength, 0.0f, 0.0f);
                // Y Axis (green)
                GL.Color3(Color.Green);
                GL.Vertex3(0.0f, 0.0f, 0.0f);
                GL.Vertex3(0.0f, OriginLength, 0.0f);
                // Z Axis (blue)
                GL.Color3(Color.Blue);
                GL.Vertex3(0.0f, 0.0f, 0.0);
                GL.Vertex3(0.0f, 0.0f, -OriginLength);
                GL.End();
            }
        }

        /// <summary>
        /// Draw a wireframe box around our world
        /// </summary>
        private void DrawBoundingBox()
        {
            if (ShowBoundingBox)
            {
                // Draw the bounding box
                GL.Color3(Color.Gray);
                GL.LineWidth(1);
                var cubeLineVertices = utils.GetCubeVertexes(0, 0, 0, WorldWidth, WorldHeight, WorldDepth);
                GL.Begin(PrimitiveType.Lines);
                foreach (var vertex in cubeLineVertices)
                {
                    GL.Vertex3(vertex);
                }
                GL.End();
            }
        }

        private void GLEditControl_Resize(object sender, EventArgs e)
        {
            //Console.WriteLine(Name + ".Resize");
            _setupComplete = false;
        }

        private void SetupViewport()
        {
            //Console.WriteLine(Name + ".SetupViewport");
            if (!_glLoaded) return;
            _setupComplete = true;

            MakeCurrent();

            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(Color.Black);

            GL.Viewport(0, 0, Width, Height);

            // Setup the identity view matrix
            var eye = new Vector3(0, 0, 0);
            var target = new Vector3(0, 0, -1);
            var up = new Vector3(0, 1, 0);

            switch (CurrentView)
            {
                case ViewTypes.Front:
                    break;
                case ViewTypes.Left:
                    eye = new Vector3(-1, 0, -WorldDepth);
                    target = new Vector3(0, 0, -WorldDepth);
                    up = new Vector3(0, 1, 0);
                    break;
                case ViewTypes.Right:
                    eye = new Vector3(WorldWidth, 0f, 0f);
                    target = new Vector3(0f, 0f, 0f);
                    up = new Vector3(0f, 1f, 0f);
                    break;
                case ViewTypes.Top:
                    eye = new Vector3(0f, WorldHeight+1, 0f);
                    target = new Vector3(0f, 0.0f, 0f);
                    up = new Vector3(0, 0, -1);
                    break;
                case ViewTypes.Bottom:
                    eye = new Vector3(0, 0, -WorldDepth);
                    target = new Vector3(0, 1, -WorldDepth);
                    up = new Vector3(0, 0, 1);
                    break;
                case ViewTypes.Back:
                    eye = new Vector3(WorldWidth, 0, -WorldDepth+1);
                    target = new Vector3(WorldWidth, 0, 0);
                    up = new Vector3(0, 1, 0);
                    break;
                case ViewTypes.Perspective:
                    eye = new Vector3(0, 0, 0);
                    target = new Vector3(WorldWidth/2, WorldHeight/2, -WorldDepth);
                    up = new Vector3(0, 1, 0);
                    break;
            }
            _viewMatrix = Matrix4.Identity;
            _viewMatrix = Matrix4.LookAt(eye, target, up);

            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _identityMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref _viewMatrix);
        }

        public void SetInitialZoomLevel()
        {
            ZoomLevel = WorldWidth / Width;
        }

        public Point GetClientMousePosition()
        {
            return PointToClient(MousePosition);
        }

        private PropBase _currentProp;
        private void GLEditControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                StartPan();
                return;
            }

            if (!Editing) return;

            if (e.Button == MouseButtons.Left)
            {
                if (SetupForm.CurrentPropTypeName == "Select")
                {
                    // See if we have a selected prop and the mouse is over a selection point
                    foreach (var prop in SelectedProps)
                    {
                        var selectedPoint = prop.SelectionPointsHitTest(GetMousePosition(), ZoomLevel);
                        if (selectedPoint != null)
                        {
                            Capture = true;
                            _currentProp = prop;
                            prop.SelectedPoint = selectedPoint;
                            return;
                        }
                    }
                    // Select the prop that was clicked on
                    DeselectEverything();
                    foreach (var prop in Data.Props)
                    {
                        if (prop.HitTest(GetMousePosition(), ZoomLevel))
                        {
                            prop.Selected = true;
                            SelectedProps.Add(prop);
                            return;
                        }
                    }
                }
                else if (CurrentView != ViewTypes.Perspective)
                {
                    AddCurrentlySelectedPropType();
                    return;
                }
            }
        }

        private void GLEditControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Pan();
                return;
            }

            if (!Editing) return;
            if (e.Button == MouseButtons.Left)
            {
                if (Capture)
                {
                    if (_currentProp != null)
                    {
                        Console.WriteLine("_currentProp != null");
                        _currentProp.SetSelectedPointPosition(GetMousePosition());
                    }
                }
            }
            else
            {
                foreach (var prop in SelectedProps)
                {
                    if (prop.SelectionPointsHitTest(GetMousePosition(), ZoomLevel) != null)
                    {
                        Cursor.Current = Cursors.Cross;
                        return;
                    }
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void GLEditControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                EndPan();
            }
            else if (e.Button == MouseButtons.Left)
            {
                SetupForm.SelectPropButton("Select");
                Capture = false;
            }
        }

        private void DeselectEverything()
        {
            if (!Editing) return;
            foreach (var prop in SelectedProps)
            {
                prop.Selected = false;
            }
            SelectedProps.Clear();
            _currentProp = null;
        }

        public void SelectProp(PropBase prop)
        {
            prop.Selected = true;
            SelectedProps.Add(prop);
        }

        private void AddCurrentlySelectedPropType()
        {
            if (!string.IsNullOrEmpty(SetupForm.CurrentPropTypeName))
            {
                _currentProp = null;
                switch (SetupForm.CurrentPropTypeName)
                {
                    case "Line":
                        _currentProp = new Line(SetupForm.ElementsForm.SelectedNode, GetMousePosition());
                        break;
                }
                if (_currentProp != null)
                {
                    Data.Props.Add(_currentProp);
                    Capture = true;
                    SelectProp(_currentProp);
                    _currentProp.SelectDefaultSelectPoint();
                }
            }
        }

        private void GLEditControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (SelectedProps != null)
                    {
                        foreach (var prop in SelectedProps)
                        {
                            Data.Props.Remove(prop);
                        }
                    }
                    break;
            }
        }

    }
}