﻿namespace VixenModules.Preview.VixenPreview
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusPixels = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
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
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusFPS,
            this.toolStripStatusPixels});
            this.statusStrip1.Location = new System.Drawing.Point(0, 473);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(886, 25);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusFPS
            // 
            this.toolStripStatusFPS.Name = "toolStripStatusFPS";
            this.toolStripStatusFPS.Size = new System.Drawing.Size(151, 20);
            this.toolStripStatusFPS.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusPixels
            // 
            this.toolStripStatusPixels.Name = "toolStripStatusPixels";
            this.toolStripStatusPixels.Size = new System.Drawing.Size(151, 20);
            this.toolStripStatusPixels.Text = "toolStripStatusLabel1";
            // 
            // OpenGLViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 498);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.glControl);
            this.Name = "OpenGLViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Vixen Preview OpenGL";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpenGLViewer_FormClosing);
            this.Move += new System.EventHandler(this.OpenGLViewer_Move);
            this.Resize += new System.EventHandler(this.OpenGLViewer_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFPS;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusPixels;
    }
}