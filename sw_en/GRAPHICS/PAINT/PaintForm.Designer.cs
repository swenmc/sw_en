namespace CENEX
{

    partial class PaintForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.LineDraw = new System.Windows.Forms.ToolStripMenuItem();
            this.RectDraw = new System.Windows.Forms.ToolStripMenuItem();
            this.EllipseDraw = new System.Windows.Forms.ToolStripMenuItem();
            this.FilledRectangle = new System.Windows.Forms.ToolStripMenuItem();
            this.FilledEllipse = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.specimen1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tutorial2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LineDraw,
            this.RectDraw,
            this.EllipseDraw,
            this.FilledRectangle,
            this.FilledEllipse});
            this.menuStrip1.Location = new System.Drawing.Point(0, 24);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1051, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // LineDraw
            // 
            this.LineDraw.Name = "LineDraw";
            this.LineDraw.Size = new System.Drawing.Size(63, 20);
            this.LineDraw.Text = "LineDraw";
            this.LineDraw.Click += new System.EventHandler(this.LineDraw_Click);
            // 
            // RectDraw
            // 
            this.RectDraw.Name = "RectDraw";
            this.RectDraw.Size = new System.Drawing.Size(66, 20);
            this.RectDraw.Text = "RectDraw";
            this.RectDraw.Click += new System.EventHandler(this.RectDraw_Click);
            // 
            // EllipseDraw
            // 
            this.EllipseDraw.Name = "EllipseDraw";
            this.EllipseDraw.Size = new System.Drawing.Size(73, 20);
            this.EllipseDraw.Text = "EllipseDraw";
            this.EllipseDraw.Click += new System.EventHandler(this.EllipseDraw_Click);
            // 
            // FilledRectangle
            // 
            this.FilledRectangle.Name = "FilledRectangle";
            this.FilledRectangle.Size = new System.Drawing.Size(91, 20);
            this.FilledRectangle.Text = "FilledRectangle";
            this.FilledRectangle.Click += new System.EventHandler(this.FilledRectangle_Click);
            // 
            // FilledEllipse
            // 
            this.FilledEllipse.Name = "FilledEllipse";
            this.FilledEllipse.Size = new System.Drawing.Size(72, 20);
            this.FilledEllipse.Text = "FilledEllipse";
            this.FilledEllipse.Click += new System.EventHandler(this.FilledEllipse_Click);
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.specimen1ToolStripMenuItem,
            this.tutorial2ToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(1051, 24);
            this.menuStrip2.Stretch = false;
            this.menuStrip2.TabIndex = 1;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // specimen1ToolStripMenuItem
            // 
            this.specimen1ToolStripMenuItem.Name = "specimen1ToolStripMenuItem";
            this.specimen1ToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.specimen1ToolStripMenuItem.Text = "Tutorial 1";
            this.specimen1ToolStripMenuItem.Click += new System.EventHandler(this.specimen1ToolStripMenuItem_Click);
            // 
            // tutorial2ToolStripMenuItem
            // 
            this.tutorial2ToolStripMenuItem.Name = "tutorial2ToolStripMenuItem";
            this.tutorial2ToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.tutorial2ToolStripMenuItem.Text = "Tutorial 2";
            this.tutorial2ToolStripMenuItem.Click += new System.EventHandler(this.tutorial2ToolStripMenuItem_Click);
            // 
            // PaintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1051, 662);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.menuStrip2);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PaintForm";
            this.Text = "Paint";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem LineDraw;
        private System.Windows.Forms.ToolStripMenuItem RectDraw;
        private System.Windows.Forms.ToolStripMenuItem EllipseDraw;
        private System.Windows.Forms.ToolStripMenuItem FilledRectangle;
        private System.Windows.Forms.ToolStripMenuItem FilledEllipse;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem specimen1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tutorial2ToolStripMenuItem;
        

    }
}

