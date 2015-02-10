using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using OpenTK;
using Vixen.Sys;

namespace VixenModules.Preview.Vixen3DPreview.Props
{
    [DataContract]
    class Matrix : PropBase
    {
        private float _top, _left, _width, _height;
        private const int LeftSelect = 0;
        private const int TopSelect = 1;
        private const int RightSelect = 2;
        private const int BottomSelect = 3;

        public Matrix(ElementNode selectedNode, Vector3 startPoint)
        {
            // Set the initial coordinates of our Matrix
            _left = startPoint.X;
            _top = startPoint.Y;
            _width = 1;
            _height = 1;

            // Add the selection points
            SelectionPoints.Clear();
            // Left
            SelectionPoints.Add(new PreviewPixel(startPoint));
            // Top
            SelectionPoints.Add(new PreviewPixel(startPoint));
            // Right
            SelectionPoints.Add(new PreviewPixel(startPoint));
            // Bottom
            SelectionPoints.Add(new PreviewPixel(startPoint));

            Adding = true;
        }

        public override void Layout()
        {
        }

        public override void SetSelectedPointPosition(OpenTK.Vector3 vector)
        {
            var topY = SelectionPoints[RightSelect].Location.Y;
            var z = SelectionPoints[RightSelect].Location.Z;
            SelectionPoints[RightSelect].Location = new Vector3(vector.X, topY, z);
            SelectionPoints[BottomSelect].Location = new Vector3(vector.X, vector.Y, z);
            Layout();
        }

        public override void CompleteAdd(OpenTK.Vector3 clientVectorPosition)
        {
            throw new NotImplementedException();
        }

        [DataMember,
         Category("Settings"),
         Description("The left position of the matrix.")]
        public override float Left
        {
            get { return _left; }
            set { _left = value; }
        }

        [DataMember,
         Category("Settings"),
         Description("The top position of the matrix.")]
        public override float Top
        {
            get { return _top; }
            set { _top = value; }
        }

        [DataMember,
         Category("Settings"),
         Description("The width of the matrix.")]
        public override float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        [DataMember,
         Category("Settings"),
         Description("The left position of the matrix.")]
        public override float Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public override bool Selected
        {
            get { return _selected; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void SelectDefaultSelectPoint()
        {
            throw new NotImplementedException();
        }
    }
}
