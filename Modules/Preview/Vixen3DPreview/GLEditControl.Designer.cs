namespace Vixen3DPreview
{
    partial class GLEditControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelViewButton = new System.Windows.Forms.Label();
            this.labelView = new System.Windows.Forms.Label();
            this.contextMenuStripView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.frontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.perspectiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripView.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelViewButton
            // 
            this.labelViewButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelViewButton.ForeColor = System.Drawing.Color.White;
            this.labelViewButton.Location = new System.Drawing.Point(105, 0);
            this.labelViewButton.Name = "labelViewButton";
            this.labelViewButton.Size = new System.Drawing.Size(22, 23);
            this.labelViewButton.TabIndex = 1;
            this.labelViewButton.Text = ">";
            this.labelViewButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelViewButton.Click += new System.EventHandler(this.labelViewButton_Click);
            // 
            // labelView
            // 
            this.labelView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelView.ForeColor = System.Drawing.Color.White;
            this.labelView.Location = new System.Drawing.Point(0, 0);
            this.labelView.Name = "labelView";
            this.labelView.Size = new System.Drawing.Size(105, 23);
            this.labelView.TabIndex = 0;
            this.labelView.Text = "Front";
            this.labelView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelView.DoubleClick += new System.EventHandler(this.labelView_DoubleClick);
            this.labelView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelView_MouseClick);
            // 
            // contextMenuStripView
            // 
            this.contextMenuStripView.BackColor = System.Drawing.Color.Black;
            this.contextMenuStripView.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.frontToolStripMenuItem,
            this.backToolStripMenuItem,
            this.rightToolStripMenuItem,
            this.leftToolStripMenuItem,
            this.topToolStripMenuItem,
            this.bottomToolStripMenuItem,
            this.perspectiveToolStripMenuItem});
            this.contextMenuStripView.Name = "contextMenuStripView";
            this.contextMenuStripView.ShowImageMargin = false;
            this.contextMenuStripView.Size = new System.Drawing.Size(129, 172);
            this.contextMenuStripView.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStripView_ItemClicked);
            // 
            // frontToolStripMenuItem
            // 
            this.frontToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.frontToolStripMenuItem.Name = "frontToolStripMenuItem";
            this.frontToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.frontToolStripMenuItem.Text = "Front";
            // 
            // backToolStripMenuItem
            // 
            this.backToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.backToolStripMenuItem.Name = "backToolStripMenuItem";
            this.backToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.backToolStripMenuItem.Text = "Back";
            // 
            // rightToolStripMenuItem
            // 
            this.rightToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.rightToolStripMenuItem.Name = "rightToolStripMenuItem";
            this.rightToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.rightToolStripMenuItem.Text = "Right";
            // 
            // leftToolStripMenuItem
            // 
            this.leftToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.leftToolStripMenuItem.Name = "leftToolStripMenuItem";
            this.leftToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.leftToolStripMenuItem.Text = "Left";
            // 
            // topToolStripMenuItem
            // 
            this.topToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.topToolStripMenuItem.Name = "topToolStripMenuItem";
            this.topToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.topToolStripMenuItem.Text = "Top";
            // 
            // bottomToolStripMenuItem
            // 
            this.bottomToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.bottomToolStripMenuItem.Name = "bottomToolStripMenuItem";
            this.bottomToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.bottomToolStripMenuItem.Text = "Bottom";
            // 
            // perspectiveToolStripMenuItem
            // 
            this.perspectiveToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.perspectiveToolStripMenuItem.Name = "perspectiveToolStripMenuItem";
            this.perspectiveToolStripMenuItem.Size = new System.Drawing.Size(128, 24);
            this.perspectiveToolStripMenuItem.Text = "Perspective";
            // 
            // GLEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelViewButton);
            this.Controls.Add(this.labelView);
            this.Name = "GLEditControl";
            this.Size = new System.Drawing.Size(557, 461);
            this.Load += new System.EventHandler(this.GLEditControl_Load);
            this.Resize += new System.EventHandler(this.GLEditControl_Resize);
            this.contextMenuStripView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelViewButton;
        private System.Windows.Forms.Label labelView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripView;
        private System.Windows.Forms.ToolStripMenuItem frontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem perspectiveToolStripMenuItem;

    }
}
