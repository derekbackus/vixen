using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using OpenTK;
using OpenTK.Graphics.ES11;
using Vixen.Sys;

namespace VixenModules.Preview.Vixen3DPreview.Props
{
    [DataContract]
    sealed class Line: PropBase 
    {
        private List<PreviewPixel> _points;
        private int _lightCount;
        private const int DefaultLightSpacing = 10;

        public Line(ElementNode selectedNode, Vector3 startPoint)
        {
            EndPoints.Add(new PreviewPixel(startPoint));
            EndPoints.Add(new PreviewPixel(startPoint));
            
            SelectedNode = selectedNode;
            
            if (selectedNode != null)
            {
                var children = PreviewUtils.GetLeafNodes(selectedNode);
                // is this a single node?
                if (children.Count == 0)
                {
                    StringType = StringTypes.Standard;
                }
                else
                {
                    StringType = StringTypes.Pixel;
                    //LightCount = children.Count;
                    // Just add the pixels, they will get layed out next
                    foreach (var child in children)
                    {
                        //Console.WriteLine(child.Id);
                        var pixel = new PreviewPixel(10, 10, 0)
                        {
                            NodeId = child.Id
                        };
                        Pixels.Add(pixel);
                        //pixel.= Color.White;
                    }
                }
            }
            Adding = true;
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext c) { }

        private ElementNode SelectedNode { get; set; }

        #region "Public Properties"
        [DataMember,
         Category("Settings"),
         Description("The number of unique light points in the Line"),
         DisplayName("Light Count")]
        public int LightCount
        {
            get 
            { 
                //return _lightCount; 
                return Pixels.Count;
            }
            set
            {
                _lightCount = value;
                // Are we short pixels? Add some.
                if (Pixels.Count < _lightCount)
                {
                    while (Pixels.Count < _lightCount)
                    {
                        var pixel = new PreviewPixel(new Vector3());
                        Pixels.Add(pixel);
                    }
                }
                // Do we have too many pixels? Remove some.
                else if (Pixels.Count > _lightCount)
                {
                    while (Pixels.Count > _lightCount)
                    {
                        Pixels.RemoveAt(Pixels.Count - 1);
                    }
                }
            }
        }
        #endregion

        [DataMember]
        public List<PreviewPixel> EndPoints
        {
            get { return _points ?? (_points = new List<PreviewPixel>()); }
            set { _points = value ?? (_points = new List<PreviewPixel>()); }
        }

        /// <summary>
        /// Layout all of the pixels for the Line
        /// </summary>
        public override void Layout()
        {
            //Console.WriteLine("Layout - Line: (" + EndPoints[0].Location.X + ":" + EndPoints[1].Location.X + "):" + LightCount + ":" + Pixels.Count);
            var length = (EndPoints[0].Location - EndPoints[1].Location).Length;
            if (Adding && (SelectedNode == null))
            {
                //Console.WriteLine("    Layout - Adding");
                var pointCount = length/DefaultLightSpacing;
                if (!pointCount.Equals(Convert.ToSingle(Math.Round(pointCount))))
                {
                    pointCount += 1f;
                }
                if (pointCount < 2f) pointCount = 2f;
                pointCount = Convert.ToSingle(Math.Truncate(pointCount));
                LightCount = Convert.ToInt32(pointCount);
            }

            var xSpacing = (EndPoints[0].Location.X - EndPoints[1].Location.X) / (LightCount - 1);
            var ySpacing = (EndPoints[0].Location.Y - EndPoints[1].Location.Y) / (LightCount - 1);
            var zSpacing = (EndPoints[0].Location.Z - EndPoints[1].Location.Z) / (LightCount - 1);
            var x = EndPoints[0].Location.X;
            var y = EndPoints[0].Location.Y;
            var z = EndPoints[0].Location.Z;

            for (var lightNum = 0; lightNum < LightCount; lightNum++)
            {
                var pixel = Pixels[lightNum];
                pixel.Size = PixelSize;
                pixel.Location = new Vector3(x, y, z);
                x -= xSpacing;
                y -= ySpacing;
                z -= zSpacing;
            }
        }

        public override void SetSelectedPointPosition(Vector3 vector)
        {
            //SelectionPoints[1].Location = vector;
            //EndPoints[1].Location = vector;

            if (SelectedPoint != null)
            {
                SelectedPoint.Location = vector;
            }

            Layout();
        }

        public override void CompleteAdd(Vector3 clientVectorPosition)
        {
            EndPoints[1].Location = clientVectorPosition;
            Layout();
            Adding = false;
        }

        public override float Left
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override float Top
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override float Width
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override float Height
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (!_selected)
                {
                    SelectionPoints = null;
                }
                else
                {
                    if (SelectionPoints == null)
                    {
                        SelectionPoints = new List<PreviewPixel>();
                    }
                    SelectionPoints.Add(EndPoints[0]);
                    SelectionPoints.Add(EndPoints[1]);
                }
            }    
    
        }

        public override void SelectDefaultSelectPoint()
        {
            if (EndPoints != null && EndPoints.Count >= 2)
            {
                SelectedPoint = EndPoints[1];
            }
        }
    }
}
