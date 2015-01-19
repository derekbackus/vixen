using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace Vixen3DPreview
{
    public partial class SetupForm : Form
    {
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        private SetupForm_Document _documentForm;
        private SetupForm_Elements _elementsForm;
        private SetupForm_Properties _propertiesForm;
        private bool _loaded = false;

        public SetupForm(Vixen3DPreviewData data)
        {
            Data = data;
            InitializeComponent();
        }

        public Vixen3DPreviewData Data { get; set; }

        private void SetupForm_Load(object sender, EventArgs e)
        {
            _documentForm = new SetupForm_Document(Data);
            _elementsForm = new SetupForm_Elements();
            _propertiesForm = new SetupForm_Properties();

            _documentForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            _elementsForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            _propertiesForm.Show(_elementsForm.Pane, WeifenLuo.WinFormsUI.Docking.DockAlignment.Bottom, 0.5);

            SetDesktopLocation(Data.SetupLeft, Data.SetupTop);
            Size = new Size(Data.SetupWidth, Data.SetupHeight);

            _loaded = true;
        }

        private void SetupForm_Resize(object sender, EventArgs e)
        {
            if (!_loaded) return;

            if (Data == null)
            {
                Logging.Warn("3D Preview SetupForm_Resize: Data is null. abandoning resize. (Thread ID: " +
                                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
                return;
            }

            Data.SetupWidth = Width;
            Data.SetupHeight = Height;
        }

        private void SetupForm_Move(object sender, EventArgs e)
        {
            if (!_loaded) return;

            if (Data == null)
            {
                Logging.Warn("3D Preview SetupForm_Move: Data is null. abandoning move. (Thread ID: " +
                                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
                return;
            }

            Data.SetupTop = Top;
            Data.SetupLeft = Left;
        }

        private void textImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowToolbarText(toolStripContainer.TopToolStripPanel, true);
            ShowToolbarText(toolStripContainer.BottomToolStripPanel, true);
            ShowToolbarText(toolStripContainer.RightToolStripPanel, true);
            ShowToolbarText(toolStripContainer.LeftToolStripPanel, true);
        }

        private void imagesOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowToolbarText(toolStripContainer.TopToolStripPanel, false);
            ShowToolbarText(toolStripContainer.BottomToolStripPanel, false);
            ShowToolbarText(toolStripContainer.RightToolStripPanel, false);
            ShowToolbarText(toolStripContainer.LeftToolStripPanel, false);
        }

        private void ShowToolbarText(ToolStripPanel panel, bool show)
        {
            foreach (var strip in panel.Controls)
            {
                if (strip is ToolStrip)
                {
                    if (strip != menuStrip)
                    {
                        foreach (ToolStripItem item in (strip as ToolStrip).Items)
                        {
                            if (show)
                            {
                                item.TextImageRelation = TextImageRelation.ImageAboveText;
                                item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                            }
                            else
                            {
                                item.TextImageRelation = TextImageRelation.ImageAboveText;
                                item.DisplayStyle = ToolStripItemDisplayStyle.Image;
                            }
                        }
                    }
                }
            }
        }


    }
}
