using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Vixen3DPreview.Props
{
    public class PropBase
    {
        private int _pixelSize = 2;
        private List<PreviewPixel> _pixels;
        private string _name;

        public enum StringType
        {
            Pixel,
            Normal
        }

        #region "Data Properties"

        [DataMember,
         Browsable(false)]
        public virtual List<PreviewPixel> Pixels 
        {
            get { return _pixels; }
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
        
        #endregion

    }
}
