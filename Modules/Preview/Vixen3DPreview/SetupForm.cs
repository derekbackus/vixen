using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VixenModules.Preview.Vixen3DPreview
{
    public partial class SetupForm : Form
    {
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        private bool _loaded = false;
        private PreviewUtils utils = new PreviewUtils();

        public SetupForm(Vixen3DPreviewPrivateData data)
        {
            Data = data;
            Console.WriteLine("SetupForm InstanceId: " + Data.InstanceId);
            InitializeComponent();
        }

        public Vixen3DPreviewPrivateData Data { get; set; }
        public SetupForm_Document DocumentForm { get; set; }
        public SetupForm_Elements ElementsForm { get; set; }
        public SetupForm_Properties PropertiesForm { get; set; }

        private void SetupForm_Load(object sender, EventArgs e)
        {
            // Set the window location and size before other things get created
            SetDesktopLocation(utils.GetPrivateSetting("SetupLeft", 0), utils.GetPrivateSetting("SetupTop", 0));
            Size = new Size(utils.GetPrivateSetting("SetupWidth", 1024), utils.GetPrivateSetting("SetupHeight", 600));

            DocumentForm = new SetupForm_Document(Data, this);
            ElementsForm = new SetupForm_Elements();
            PropertiesForm = new SetupForm_Properties();

            DocumentForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            ElementsForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            PropertiesForm.Show(ElementsForm.Pane, WeifenLuo.WinFormsUI.Docking.DockAlignment.Bottom, 0.5);

            boundingBoxToolStripMenuItem.Checked = utils.GetPrivateSetting("ShowBoundingBox", true);
            DocumentForm.ShowBoundingBox = boundingBoxToolStripMenuItem.Checked;
            originToolStripMenuItem.Checked = utils.GetPrivateSetting("ShowOrigin", true);
            DocumentForm.ShowOrigin = originToolStripMenuItem.Checked;

            DocumentForm.WorldWidth = 12*100;
            DocumentForm.WorldHeight = 12*20;
            DocumentForm.WorldDepth = 12*75;

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

            utils.SavePrivateSetting("SetupWidth", Width);
            utils.SavePrivateSetting("SetupHeight", Height);
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

            utils.SavePrivateSetting("SetupTop", Top);
            utils.SavePrivateSetting("SetupLeft", Left);
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

        private void boundingBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (sender as ToolStripMenuItem);
            menuItem.Checked = !menuItem.Checked;
            DocumentForm.ShowBoundingBox = menuItem.Checked;
            utils.SavePrivateSetting("ShowBoundingBox", menuItem.Checked);
        }

        private void originToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (sender as ToolStripMenuItem);
            menuItem.Checked = !menuItem.Checked;
            DocumentForm.ShowOrigin = menuItem.Checked;
            utils.SavePrivateSetting("ShowOrigin", menuItem.Checked);
        }

        private void singleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DocumentForm.DocumentLayout = SetupForm_Document.DocumentLayoutType.Single;
        }

        private void quadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DocumentForm.DocumentLayout = SetupForm_Document.DocumentLayoutType.Quad;;
        }

        private void dualHorizontalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DocumentForm.DocumentLayout = SetupForm_Document.DocumentLayoutType.DualHorizontal;
        }

        private void dualVerticalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DocumentForm.DocumentLayout = SetupForm_Document.DocumentLayoutType.DualVertical;
        }

        private void toolStripPropButton_Click(object sender, EventArgs e)
        {
            var button = (sender as ToolStripButton);
            if (button == null) return;

            foreach (var btn in ToolStripButtons)
            {
                btn.Checked = false;
            }
            button.Checked = true;
        }

        public string CurrentPropTypeName
        {
            get
            {
                foreach (var button in ToolStripButtons)
                {
                    if (button.Tag != null)
                    {
                        if (button.Checked) return button.Tag.ToString();
                    }
                }
                return "";
            }
        }

        public List<ToolStripButton> ToolStripButtons
        {
            get
            {
                var buttons = new List<ToolStripButton>();
                foreach (var container in Controls.OfType<ToolStripContainer>())
                {
                    foreach (var panel in container.Controls.OfType<ToolStripPanel>())
                    {
                        foreach (var strip in panel.Controls.OfType<ToolStrip>())
                        {
                            foreach (var item in strip.Items.OfType<ToolStripButton>())
                            {
                                buttons.Add(item);
                            }
                        }
                    }
                }
                return buttons;
            }
        }

        public void SelectPropButton(string buttonName)
        {
            foreach (var button in ToolStripButtons)
            {
                button.Checked = false;
                //Console.WriteLine("SelectPropButton:" + buttonName);
                if (button.Tag != null && button.Tag.ToString() == buttonName)
                {
                    //Console.WriteLine("SelectPropButton2:" + buttonName);
                    button.Checked = true;
                }
            }
        }

        private void toolStripButtonFileSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        public void SaveData()
        {
            PreviewUtils.SaveData(Data);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveData();
        }
    }
}
