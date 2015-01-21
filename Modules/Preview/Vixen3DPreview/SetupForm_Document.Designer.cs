namespace Vixen3DPreview
{
    partial class SetupForm_Document
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
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.glEditControl1 = new Vixen3DPreview.GLEditControl();
            this.glEditControl2 = new Vixen3DPreview.GLEditControl();
            this.glEditControl3 = new Vixen3DPreview.GLEditControl();
            this.glEditControl4 = new Vixen3DPreview.GLEditControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerVert
            // 
            this.splitContainerVert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerVert.Location = new System.Drawing.Point(0, 0);
            this.splitContainerVert.Name = "splitContainerVert";
            this.splitContainerVert.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainerVert.Size = new System.Drawing.Size(1026, 501);
            this.splitContainerVert.SplitterDistance = 267;
            this.splitContainerVert.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.glEditControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.glEditControl2);
            this.splitContainer1.Size = new System.Drawing.Size(1026, 267);
            this.splitContainer1.SplitterDistance = 416;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.glEditControl3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.glEditControl4);
            this.splitContainer2.Size = new System.Drawing.Size(1026, 230);
            this.splitContainer2.SplitterDistance = 342;
            this.splitContainer2.TabIndex = 0;
            // 
            // glEditControl1
            // 
            this.glEditControl1.BackColor = System.Drawing.Color.Black;
            this.glEditControl1.CurrentView = Vixen3DPreview.GLEditControl.ViewTypes.Front;
            this.glEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glEditControl1.Location = new System.Drawing.Point(0, 0);
            this.glEditControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glEditControl1.Name = "glEditControl1";
            this.glEditControl1.Size = new System.Drawing.Size(416, 267);
            this.glEditControl1.TabIndex = 1;
            this.glEditControl1.VSync = false;
            this.glEditControl1.ViewDoubleClicked += new Vixen3DPreview.ViewDoubleClickedEventHandler(this.glEditControl_ViewDoubleClicked);
            this.glEditControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glEditControl_MouseDown);
            this.glEditControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glEditControl_MouseUp);
            // 
            // glEditControl2
            // 
            this.glEditControl2.BackColor = System.Drawing.Color.Black;
            this.glEditControl2.CurrentView = Vixen3DPreview.GLEditControl.ViewTypes.Front;
            this.glEditControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glEditControl2.Location = new System.Drawing.Point(0, 0);
            this.glEditControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glEditControl2.Name = "glEditControl2";
            this.glEditControl2.Size = new System.Drawing.Size(606, 267);
            this.glEditControl2.TabIndex = 0;
            this.glEditControl2.VSync = false;
            this.glEditControl2.ViewDoubleClicked += new Vixen3DPreview.ViewDoubleClickedEventHandler(this.glEditControl_ViewDoubleClicked);
            // 
            // glEditControl3
            // 
            this.glEditControl3.BackColor = System.Drawing.Color.Black;
            this.glEditControl3.CurrentView = Vixen3DPreview.GLEditControl.ViewTypes.Front;
            this.glEditControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glEditControl3.Location = new System.Drawing.Point(0, 0);
            this.glEditControl3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glEditControl3.Name = "glEditControl3";
            this.glEditControl3.Size = new System.Drawing.Size(342, 230);
            this.glEditControl3.TabIndex = 0;
            this.glEditControl3.VSync = false;
            this.glEditControl3.ViewDoubleClicked += new Vixen3DPreview.ViewDoubleClickedEventHandler(this.glEditControl_ViewDoubleClicked);
            // 
            // glEditControl4
            // 
            this.glEditControl4.BackColor = System.Drawing.Color.Black;
            this.glEditControl4.CurrentView = Vixen3DPreview.GLEditControl.ViewTypes.Front;
            this.glEditControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glEditControl4.Location = new System.Drawing.Point(0, 0);
            this.glEditControl4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glEditControl4.Name = "glEditControl4";
            this.glEditControl4.Size = new System.Drawing.Size(680, 230);
            this.glEditControl4.TabIndex = 0;
            this.glEditControl4.VSync = false;
            this.glEditControl4.ViewDoubleClicked += new Vixen3DPreview.ViewDoubleClickedEventHandler(this.glEditControl_ViewDoubleClicked);
            // 
            // SetupForm_Document
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 501);
            this.Controls.Add(this.splitContainerVert);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SetupForm_Document";
            this.Text = "Preview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_Document_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SetupForm_Document_FormClosed);
            this.Load += new System.EventHandler(this.SetupForm_Document_Load);
            this.Resize += new System.EventHandler(this.SetupForm_Document_Resize);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerVert;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private GLEditControl glEditControl3;
        private GLEditControl glEditControl2;
        private GLEditControl glEditControl4;
        private GLEditControl glEditControl1;
    }
}