using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vixen3DPreview
{
    public class MyMenuRenderer : ToolStripProfessionalRenderer
    {
        public MyMenuRenderer() : base(new MyMenuColors()) { }
    }

    public class MyMenuColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected
        {
            get { return Color.DimGray; }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.White; }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.White; }
        }
        public override Color MenuItemBorder
        {
            get { return Color.DimGray; }
        }
    }
}
