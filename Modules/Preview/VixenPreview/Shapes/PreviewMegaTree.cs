﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Vixen.Sys;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing.Design;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewMegaTree : PreviewBaseShape, ICloneable
	{
		[DataMember] private PreviewPoint _topLeft;
		[DataMember] private PreviewPoint _bottomRight;

		[DataMember] private int _stringCount;
		[DataMember] private int _topHeight;
		[DataMember] private int _topWidth;
		[DataMember] private int _baseHeight;
		[DataMember] private int _lightsPerString;
		[DataMember] private int _degrees;

		[DataMember] private PreviewPoint _topRight, _bottomLeft;

		private PreviewPoint p1Start, p2Start;

		public PreviewMegaTree(PreviewPoint point1, ElementNode selectedNode, double zoomLevel)
		{
			ZoomLevel = zoomLevel;
			_topLeft = PointToZoomPoint(point1);
			_bottomRight = new PreviewPoint(_topLeft.X, _topLeft.Y);

			_stringCount = 16;
			_topWidth = 20;
			_topHeight = _topWidth/2;
			_baseHeight = 40;
			_lightsPerString = 50;
			_degrees = 360;

			_strings = new List<PreviewBaseShape>();

			int childLightCount;
			if (IsPixelTreeSelected(selectedNode, out childLightCount)) {
				StringType = StringTypes.Pixel;
				_lightsPerString = childLightCount;
				foreach (ElementNode child in selectedNode.Children) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString, child, ZoomLevel);
					_strings.Add(line);
				}
				_stringCount = _strings.Count;
			}
			else if (IsStandardTreeSelected(selectedNode)) {
				StringType = StringTypes.Standard;
				foreach (ElementNode child in selectedNode.Children) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString, child, ZoomLevel);
					_strings.Add(line);
				}
				_stringCount = _strings.Count;
			}
			else {
				// Just add the pixels, we don't care where they go... they get positioned in Layout()
				for (int stringNum = 0; stringNum < _stringCount; stringNum++) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString, null, ZoomLevel);
					_strings.Add(line);
				}
			}

			// Lay out the pixels
			Layout();
		}

		private bool IsPixelTreeSelected(ElementNode selectedNode, out int childLightCount)
		{
			int lastChildLightCount = -1;
			childLightCount = -1;
			if (selectedNode != null && selectedNode.Children != null) {
				int parentStringCount = selectedNode.Children.ToList().Count;
				// Selected node has to be a group!
				if (!selectedNode.IsLeaf && parentStringCount >= 4) {
					// Iterate through the strings in the tree
					parentStringCount = selectedNode.Children.ToList().Count;
					foreach (ElementNode parent in selectedNode.Children) {
						int childCount = parent.Children.ToList().Count;
						if (lastChildLightCount == -1) {
							lastChildLightCount = childCount;
						}
							// All the strings have to have the same light count for this to work!
						else if (lastChildLightCount != childCount) {
							return false;
						}
						lastChildLightCount = childCount;

						foreach (ElementNode child in parent.Children) {
							// If there are sub-groups this is not a mega tree element!
							if (!child.IsLeaf) {
								return false;
							}
						}
					}
				}

				if (lastChildLightCount > 4 && parentStringCount >= 4) {
					childLightCount = lastChildLightCount;
					return true;
				}
				else {
					return false;
				}
			}
			else {
				return false;
			}
		}

		private bool IsStandardTreeSelected(ElementNode selectedNode)
		{
			int parentStringCount = 0;
			// Selected node has to be a group!
			if (selectedNode != null && !selectedNode.IsLeaf) {
				// Iterate through the strings in the tree
				foreach (ElementNode parent in selectedNode.Children) {
					parentStringCount += 1;
					// If there are more groups, this is not a Mega Tree
					if (!parent.IsLeaf)
						return false;
				}
			}
			// Gotta have at least 4 strings to make a Mega Tree!
			return (parentStringCount >= 4);
		}

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

		public void SetTopLeft(int X, int Y)
		{
			_topLeft.X = X;
			_topLeft.Y = Y;
		}

		public void SetBottomRight(int X, int Y)
		{
			_bottomRight.X = X;
			_bottomRight.Y = Y;
		}

		#region "Properties'

		public int TopHeight
		{
			set
			{
				_topHeight = value;
				Layout();
			}
			get { return _topHeight; }
		}

		public int TopWidth
		{
			set
			{
				_topWidth = value;
				Layout();
			}
			get { return _topWidth; }
		}

		public int BaseHeight
		{
			set
			{
				_baseHeight = value;
				Layout();
			}
			get { return _baseHeight; }
		}

		public int Degrees
		{
			set
			{
				_degrees = value;
				Layout();
			}
			get { return _degrees; }
		}

		public int LightsPerString
		{
			set
			{
				_lightsPerString = value;
				foreach (PreviewLine line in _strings) {
					line.PixelCount = _lightsPerString;
				}
			}
			get { return _lightsPerString; }
		}

		public int StringCount
		{
			set
			{
				_stringCount = value;
				while (_strings.Count > _stringCount) {
					_strings.RemoveAt(_strings.Count - 1);
				}
				while (_strings.Count < _stringCount) {
					PreviewLine line = new PreviewLine(new PreviewPoint(10, 10), new PreviewPoint(10, 10), _lightsPerString, null, ZoomLevel);
					_strings.Add(line);
				}
				Layout();
			}
			get { return _stringCount; }
		}

        public override int Bottom
        {
            get
            {
                return _bottomRight.Y;
            }
        }

        public override int Right
        {
            get
            {
                return _bottomRight.X;
            }
        }

		public override int Top
		{
			get 
            { 
                return _topLeft.Y; 
            }
			set 
            {
                _bottomRight.Y = value + (_bottomRight.Y - _topLeft.Y);
                _topLeft.Y = value; 
                Layout();
            }
		}

		public override int Left
		{
			get 
            { 
                return _topLeft.X; 
            }
			set 
            {
                _bottomRight.X = value + (_bottomRight.X - _topLeft.X);
                _topLeft.X = value;
                Layout();
            }
		}

		public PreviewPoint BottomRight
		{
			get { return _bottomRight; }
			set { _bottomRight = value; }
		}

        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewMegaTree shape = (matchShape as PreviewMegaTree);
            LightsPerString = shape.LightsPerString;
            StringCount = shape.StringCount;
            PixelSize = shape.PixelSize;
            TopHeight = shape.TopHeight;
            TopWidth = shape.TopWidth;
            BaseHeight = shape.BaseHeight;
            Degrees = shape.Degrees;
            BottomRight.X = _topLeft.X + (shape.BottomRight.X - shape._topLeft.X);
            BottomRight.Y = _topLeft.Y + (shape.BottomRight.Y - shape._topLeft.Y);
            Layout();
        }

		#endregion

		public void SetStrings(List<PreviewBaseShape> strings)
		{
			_strings = new List<PreviewBaseShape>();
			foreach (PreviewBaseShape line in strings) {
				PreviewBaseShape newLine = (PreviewLine) line.Clone();
				_strings.Add(newLine);
			}
			_stringCount = _strings.Count();
		}


		public int PixelCount
		{
			set
			{
				foreach (PreviewLine line in _strings) {
					line.PixelCount = value;
				}
			}
			get { return Pixels.Count; }
		}


		[Browsable(false)]
		public override List<PreviewPixel> Pixels
		{
			get
			{
				if (_strings != null && _strings.Count > 0)
				{
					var outPixels = new List<PreviewPixel>();
					for (int i = 0; i < StringCount; i++)
					{
						foreach (PreviewPixel pixel in _strings[i].Pixels)
						{
							outPixels.Add(pixel);
						}
					}

					return outPixels;
				}
				else {
					return _pixels;
				}
			}
			set
			{
				_pixels = value;
			}
		}

		public override void Layout()
		{
			if (_bottomRight != null && _topLeft != null)
			{
				int width = _bottomRight.X - _topLeft.X;
				int height = _bottomRight.Y - _topLeft.Y;

				List<Point> _topEllipsePoints;
				List<Point> _baseEllipsePoints;

				double topLeftOffset = _topLeft.X + (width / 2) - (_topWidth / 2);
				double bottomTopOffset = _bottomRight.Y - _baseHeight;

				double totalStringsInEllipse = Math.Ceiling((360d / Convert.ToDouble(_degrees)) * Convert.ToDouble(StringCount));

				_topEllipsePoints = PreviewTools.GetEllipsePoints(topLeftOffset,
																  _topLeft.Y,
																  _topWidth,
																  _topHeight,
																  totalStringsInEllipse,
																  _degrees,
																  0);
				_baseEllipsePoints = PreviewTools.GetEllipsePoints(_topLeft.X,
																   bottomTopOffset,
																   width,
																   _baseHeight,
																   totalStringsInEllipse,
																   _degrees,
																   0);

				for (int stringNum = 0; stringNum < (int)_stringCount; stringNum++)
				{
					if (stringNum < StringCount && stringNum < _topEllipsePoints.Count())
					{
						var topPixel = _topEllipsePoints[_stringCount - 1 - stringNum];
						var basePixel = _baseEllipsePoints[_stringCount - 1 - stringNum];

						var line = _strings[stringNum] as PreviewLine;
					    if (line != null)
					    {
					        line.SetPoint0(basePixel.X, basePixel.Y);
					        line.SetPoint1(topPixel.X, topPixel.Y);
					        line.Layout();
					    }
					}
				}

				SetPixelZoom();
			}
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
			PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			// See if we're resizing
			if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size) {
				if (_selectedPoint == _topRight) {
					_topLeft.Y = point.Y;
					_bottomRight.X = point.X;
				}
				else if (_selectedPoint == _bottomLeft) {
					_topLeft.X = point.X;
					_bottomRight.Y = point.Y;
				}
				_selectedPoint.X = point.X;
				_selectedPoint.Y = point.Y;
			}
				// If we get here, we're moving
			else {
				//_topLeft.X = p1Start.X + changeX;
				//_topLeft.Y = p1Start.Y + changeY;
				//_bottomRight.X = p2Start.X + changeX;
				//_bottomRight.Y = p2Start.Y + changeY;

				_topLeft.X = Convert.ToInt32(p1Start.X * ZoomLevel) + changeX;
				_topLeft.Y = Convert.ToInt32(p1Start.Y * ZoomLevel) + changeY;
				_bottomRight.X = Convert.ToInt32(p2Start.X * ZoomLevel) + changeX;
				_bottomRight.Y = Convert.ToInt32(p2Start.Y * ZoomLevel) + changeY;

				PointToZoomPointRef(_topLeft);
				PointToZoomPointRef(_bottomRight);
			}

			_topRight.X = _bottomRight.X;
			_topRight.Y = _topLeft.Y;
			_bottomLeft.X = _topLeft.X;
			_bottomLeft.Y = _bottomRight.Y;

			// Layout the standard shape
			Layout();
		}

		public override void SelectDragPoints()
		{
			// Create the size points
			List<PreviewPoint> selectPoints = new List<PreviewPoint>();

			selectPoints.Add(_topLeft);
			selectPoints.Add(_bottomRight);
			_topRight = new PreviewPoint(_bottomRight.X, _topLeft.Y);
			selectPoints.Add(_topRight);
			_bottomLeft = new PreviewPoint(_topLeft.X, _bottomRight.Y);
			selectPoints.Add(_bottomLeft);

			// Tell the base shape about the newely created points

			SetSelectPoints(selectPoints, null);
		}

		public override bool PointInShape(PreviewPoint point)
		{
			if (_strings != null) {
				foreach (PreviewLine line in _strings) {
					if (line.PointInShape(point))
						return true;
				}
			}
			return false;
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			if (point == null) {
				p1Start = new PreviewPoint(_topLeft.X, _topLeft.Y);
				p2Start = new PreviewPoint(_bottomRight.X, _bottomRight.Y);
			}

			_selectedPoint = point;
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _bottomRight;
		}

		public override void Draw(FastPixel.FastPixel fp, bool editMode, List<ElementNode> highlightedElements, bool selected,
		                          bool forceDraw)
		{
			if (_strings != null)
			{
				for (int i = 0; i < StringCount; i++)
				{
					foreach (PreviewPixel pixel in _strings[i]._pixels)
					{
						DrawPixel(pixel, fp, editMode, highlightedElements, selected, forceDraw);
					}
				}
			}

			base.Draw(fp, editMode, highlightedElements, selected, forceDraw);
		}

		public override object Clone()
		{
			PreviewMegaTree newTree = (PreviewMegaTree) this.MemberwiseClone();

			newTree._strings = new List<PreviewBaseShape>();
			foreach (PreviewBaseShape line in _strings) {
				PreviewBaseShape newLine = (PreviewLine) line.Clone();
				newTree._strings.Add(newLine);
			}
			newTree._topLeft = new PreviewPoint(_topLeft);
			newTree._bottomRight = new PreviewPoint(_bottomRight);

			return newTree;
		}

		[Editor(typeof (PreviewSetElementsUIEditor), typeof (UITypeEditor)),
		 CategoryAttribute("Settings"),
		 DisplayName("Linked Elements")]
		public override List<PreviewBaseShape> Strings
		{
			get
			{
				Layout();
				List<PreviewBaseShape> stringsResult;
				if (_strings.Count != StringCount)
				{
					stringsResult = new List<PreviewBaseShape>();
					for (int i = 0; i < StringCount; i++)
					{
						stringsResult.Add(_strings[i]);
					}
				}
				else
				{
					stringsResult = _strings;
					if (stringsResult == null)
					{
						stringsResult = new List<PreviewBaseShape>();
						stringsResult.Add(this);
					}
				}
				return stringsResult;
			}
			set { }
		}

		public override void MoveTo(int x, int y)
		{
			Point newTopLeft = new Point();
			newTopLeft.X = Math.Min(_topLeft.X, _bottomRight.X);
			newTopLeft.Y = Math.Min(_topLeft.Y, _bottomRight.Y);

			int deltaX = x - newTopLeft.X;
			int deltaY = y - newTopLeft.Y;

			_topLeft.X += deltaX;
			_topLeft.Y += deltaY;
			_bottomRight.X += deltaX;
			_bottomRight.Y += + deltaY;

			_topRight.X = _bottomRight.X;
			_topRight.Y = _topLeft.Y;
			_bottomLeft.X = _topLeft.X;
			_bottomLeft.Y = _bottomRight.Y;

			Layout();
		}

		public override void Resize(double aspect)
		{
			_topLeft.X = (int) (_topLeft.X*aspect);
			_topLeft.Y = (int) (_topLeft.Y*aspect);
			_bottomRight.X = (int) (_bottomRight.X*aspect);
			_bottomRight.Y = (int) (_bottomRight.Y*aspect);

			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			_topLeft.X = p1Start.X;
			_topLeft.Y = p1Start.Y;
			_bottomRight.X = p2Start.X;
			_bottomRight.Y = p2Start.Y;
			Resize(aspect);
		}
	}
}