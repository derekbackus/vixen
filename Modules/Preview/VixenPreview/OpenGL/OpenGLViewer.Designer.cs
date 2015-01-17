namespace VixenModules.Preview.VixenPreview
{
    partial class OpenGLViewer
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
            this.glControl = new OpenTK.GLControl();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusPixels = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelOpenGLVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.glControl.Size = new System.Drawing.Size(886, 498);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = false;
            this.glControl.Load += new System.EventHandler(this.glControl_Load);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusFPS,
            this.toolStripStatusPixels,
            this.toolStripStatusLabelOpenGLVersion,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip.Location = new System.Drawing.Point(0, 469);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(886, 29);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusFPS
            // 
            this.toolStripStatusFPS.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusFPS.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripStatusFPS.Name = "toolStripStatusFPS";
            this.toolStripStatusFPS.Size = new System.Drawing.Size(36, 24);
            this.toolStripStatusFPS.Text = "FPS";
            // 
            // toolStripStatusPixels
            // 
            this.toolStripStatusPixels.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusPixels.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripStatusPixels.Name = "toolStripStatusPixels";
            this.toolStripStatusPixels.Size = new System.Drawing.Size(52, 24);
            this.toolStripStatusPixels.Text = "Lights";
            // 
            // toolStripStatusLabelOpenGLVersion
            // 
            this.toolStripStatusLabelOpenGLVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabelOpenGLVersion.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripStatusLabelOpenGLVersion.Name = "toolStripStatusLabelOpenGLVersion";
            this.toolStripStatusLabelOpenGLVersion.Size = new System.Drawing.Size(119, 24);
            this.toolStripStatusLabelOpenGLVersion.Text = "OpenGL Version";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(474, 24);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(25, 24);
            this.toolStripStatusLabel2.Text = "    ";
            // 
            // OpenGLViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 498);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.glControl);
            this.Name = "OpenGLViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Vixen Preview OpenGL";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpenGLViewer_FormClosing);
            this.Load += new System.EventHandler(this.OpenGLViewer_Load);
            this.Move += new System.EventHandler(this.OpenGLViewer_Move);
            this.Resize += new System.EventHandler(this.OpenGLViewer_Resize);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFPS;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusPixels;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelOpenGLVersion;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}