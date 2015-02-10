namespace VixenModules.Preview.Vixen3DPreview
{
    partial class ViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewerForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.glControl = new VixenModules.Preview.Vixen3DPreview.GLEditControl();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelFPS});
            this.statusStrip.Location = new System.Drawing.Point(0, 441);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(921, 29);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabelFPS
            // 
            this.toolStripStatusLabelFPS.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabelFPS.Name = "toolStripStatusLabelFPS";
            this.toolStripStatusLabelFPS.Size = new System.Drawing.Size(45, 24);
            this.toolStripStatusLabelFPS.Text = "0 fps";
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.CurrentView = VixenModules.Preview.Vixen3DPreview.GLEditControl.ViewTypes.Front;
            this.glControl.Data = null;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Editing = false;
            this.glControl.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glControl.Name = "glControl";
            this.glControl.OriginLength = 30F;
            this.glControl.OriginThickness = 2F;
            this.glControl.PanX = 0F;
            this.glControl.PanY = 0F;
            this.glControl.ShowBoundingBox = true;
            this.glControl.ShowOrigin = true;
            this.glControl.Size = new System.Drawing.Size(921, 470);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = false;
            this.glControl.WorldDepth = 300F;
            this.glControl.WorldHeight = 300F;
            this.glControl.WorldWidth = 300F;
            this.glControl.ZoomLevel = 1F;
            this.glControl.Load += new System.EventHandler(this.glControl_Load);
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseUp);
            // 
            // ViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 470);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.glControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "ViewerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "3D Preview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewerForm_FormClosing);
            this.Load += new System.EventHandler(this.Viewer_Load);
            this.Move += new System.EventHandler(this.ViewerForm_Move);
            this.Resize += new System.EventHandler(this.ViewerForm_Resize);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GLEditControl glControl;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFPS;
    }
}