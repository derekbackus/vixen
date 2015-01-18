using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vixen3DPreview
{
    public partial class SetupForm : Form
    {
        public SetupForm(Vixen3DPreviewData data)
        {
            Data = data;
            InitializeComponent();
        }

        public Vixen3DPreviewData Data { get; set; }
    }
}
