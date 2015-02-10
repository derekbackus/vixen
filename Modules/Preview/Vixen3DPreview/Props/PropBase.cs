using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Converters;
using NLog.Layouts;
using OpenTK;

namespace VixenModules.Preview.Vixen3DPreview.Props
{
    [DataContract]
    public abstract class PropBase
    {
        private int _pixelSize = 2;
        private List<PreviewPixel> _pixels;
        private string _name;
        private List<PreviewPixel> _selectionPoints = new List<PreviewPixel>();
        private StringTypes _stringType = StringTypes.Standard;
        protected bool _selected = false;

        public enum StringTypes
        {
            Standard,
            Pixel
        }

        #region "Data Properties"

        [DataMember,
         Browsable(false)]
        public virtual List<PreviewPixel> Pixels
        {
            get { return _pixels ?? (_pixels = new List<PreviewPixel>()); }
            set { _pixels = value; }
        }

        #endregion

        #region "Configurable Options"

        [DataMember,
         Category("Settings"),
         Description("The size of the light point on the preview."),
         DisplayName("Light Size")]
        public virtual int PixelSize
        {
            get { return _pixelSize; }
            set { _pixelSize = value; }
        }

        [DataMember,
         CategoryAttribute("Settings"),
         DescriptionAttribute("The name of this string. Used in to distinguish various strings."),
         DisplayName("String Name")]
        public string Name
        {
            get { return _name ?? (_name = ""); }
            set { _name = value ?? ""; }
        }

        [DataMember,
         CategoryAttribute("Settings"),
         DescriptionAttribute("The type of string."),
         DisplayName("String Type")]
        public StringTypes StringType
        {
            get { return _stringType; }
            set { _stringType = value; }
        }


        #endregion

        /// <summary>
        /// Layout all of the pixels for this prop
        /// </summary>
        public abstract void Layout();

        /// <summary>
        /// Used during add -- set the end point and then layout the prop
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public abstract void SetSelectedPointPosition(Vector3 vector);

        /// <summary>
        /// Call on the mouseup event to complete the addition of a prop
        /// </summary>
        public abstract void CompleteAdd(Vector3 clientVectorPosition);

        /// <summary>
        /// Set this to true when adding a prop
        /// </summary>
        public bool Adding { get; set; }

        /// <summary>
        /// Used when resizing, etc. This is the point that is selected
        /// </summary>
        public PreviewPixel SelectedPoint { get; set; }

        public List<PreviewPixel> SelectionPoints
        {
            get { return _selectionPoints; }
            set { _selectionPoints = value; }
        }

        public abstract bool Selected { get; set; }
        public abstract float Left { get; set; }
        public abstract float Top { get; set; }
        public abstract float Width { get; set; }
        public abstract float Height { get; set; }

        const int DefaultSelectionOffset = 6;
        public bool HitTest(Vector3 coordinate, float zoomLevel)
        {
            var selectionOffset = DefaultSelectionOffset*zoomLevel;
            foreach (var pixel in Pixels)
            {
                if ((pixel.Location.X >= coordinate.X - selectionOffset && pixel.Location.X <= coordinate.X + selectionOffset) &&
                    (pixel.Location.Y >= coordinate.Y - selectionOffset && pixel.Location.Y <= coordinate.Y + selectionOffset))
                {
                    return true;
                }
            }
            return false;
        }

        public PreviewPixel SelectionPointsHitTest(Vector3 coordinate, float zoomLevel)
        {
            if (SelectionPoints != null && SelectionPoints.Count > 0)
            {
                var selectionOffset = DefaultSelectionOffset * zoomLevel;
                foreach (var point in SelectionPoints)
                {
                    if ((point.Location.X >= coordinate.X - selectionOffset && point.Location.X <= coordinate.X + selectionOffset) &&
                        (point.Location.Y >= coordinate.Y - selectionOffset && point.Location.Y <= coordinate.Y + selectionOffset))
                    {
                        return point;
                    }
                    
                }
            }
            return null;
        }

        public abstract void SelectDefaultSelectPoint();
    }
}
