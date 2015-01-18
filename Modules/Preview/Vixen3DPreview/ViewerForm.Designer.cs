namespace Vixen3DPreview
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
            this.glControl = new OpenTK.GLControl();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(921, 470);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = false;
            this.glControl.Load += new System.EventHandler(this.glControl_Load);
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

        private OpenTK.GLControl glControl;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFPS;
    }
}