using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CENEX
{
    public class DXFImportForm : System.Windows.Forms.Form
    {
        private int cX, cY;
        private bool det1 = false;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button2;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public DXFImportForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();


            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(27, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(62, 20);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open file";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.Control;
            this.button3.Location = new System.Drawing.Point(104, 14);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(63, 20);
            this.button3.TabIndex = 3;
            this.button3.Text = "Zoom In";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.Control;
            this.button4.Location = new System.Drawing.Point(184, 14);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(62, 20);
            this.button4.TabIndex = 4;
            this.button4.Text = "Zoom Out";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(416, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "temp";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DXFImportForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(678, 496);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Name = "DXFImportForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DXFImportDemo";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        protected CADImage FCADImage;// = new CADImage();        

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) != DialogResult.OK) return;
            if (openFileDialog1.FileName != null)
            {
                FCADImage = new CADImage();
                FCADImage.Base.Y = Top + 200;
                FCADImage.Base.X = 100;
                FCADImage.LoadFromFile(openFileDialog1.FileName);
            }
            this.Invalidate();
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (FCADImage == null)
                return;
            FCADImage.Draw(e.Graphics);
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            FCADImage.FScale = FCADImage.FScale * 2;
            this.Invalidate();
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            FCADImage.FScale = FCADImage.FScale / 2;
            FCADImage.Base.Y = Bottom - 100;
            FCADImage.Base.X = 100;
            Invalidate();
        }

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DXFImportForm.ActiveForm.Cursor = Cursors.Hand;
                cX = e.X;
                cY = e.Y;
                det1 = true;
            }
        }

        private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            det1 = false;
            DXFImportForm.ActiveForm.Cursor = Cursors.Default;
            Invalidate();
        }

        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((det1) && (FCADImage != null))
            {
                FCADImage.Base.X -= (cX - e.X) / 10;
                FCADImage.Base.Y -= (cY - e.Y) / 10;
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {

            Graphics g1 = DXFImportForm.ActiveForm.CreateGraphics();
            /*
            string s1 = "gfgtytytytytff";
            g1.TranslateTransform(0, -s1.Length*9);
            g1.RotateTransform(30.0F, MatrixOrder.Append);
            g1.DrawString(s1, new Font("Times New Roman", 14), Brushes.Black, 200, 300);
            g1.Dispose();
            g1 = Form1.ActiveForm.CreateGraphics();
            g1.DrawRectangle(Pens.Blue, 100, 100, 300, 300);
            g1.DrawString(s1, new Font("Times New Roman", 14), Brushes.Black, 200, 300);*/

            //GraphicsContainer gc1 = g1.BeginContainer();
            //g1.RotateTransform(40, MatrixOrder.Append);
            //g1.DrawString("1sdsdsds",new Font("Times New Roman", 14), Brushes.Violet, 200, 200); 
            //g1.DrawRectangle(Pens.Blue, 100, 100, 300, 300);
            //g1.EndContainer(gc1);
            //g1.DrawString("1sdsdsds",new Font("Times New Roman", 14), Brushes.Violet, 200, 200);
            //g1.DrawRectangle(Pens.Blue, 100, 100, 300, 300);

            /*
            string str = string.Empty;
            Graphics grfx = Form1.ActiveForm.CreateGraphics();
            SizeF textSize = SizeF.Empty;
            PointF textLocation = PointF.Empty;
            StringFormat strfmt = new StringFormat();
            str = "Yeewewewwwwwww";
            //strfmt.FormatFlags = StringFormatFlags.DirectionVertical;
            textSize = grfx.MeasureString(str, Font, ClientSize, strfmt);
            textLocation = new Point(ClientSize.Width / 2, ClientSize.Height / 4);
            RectangleF rectFSrc = new RectangleF(PointF.Empty, textSize);
            RectangleF rectFDest = new RectangleF(new PointF(textLocation.X + textSize.Width, textLocation.Y + textSize.Height / 2), textSize);
            GraphicsContainer container = grfx.BeginContainer(rectFDest, rectFSrc, GraphicsUnit.Pixel);
            grfx.RotateTransform(-10);
            grfx.DrawString(str, Font, SystemBrushes.ControlText,
                PointF.Empty, strfmt);
            strfmt.Dispose();
            grfx.EndContainer(container);
            */
            float startAngle = 0.0F;
            float sweepAngle = 360.0F;
            g1.DrawArc(Pens.Black, 100, 100, 82, 162, startAngle, sweepAngle);
            startAngle = 0.0F;
            sweepAngle = 360.0F;
            g1.DrawArc(Pens.Black, 200, 100, 44, 98, startAngle, sweepAngle);
            startAngle = 0.0F;
            sweepAngle = 360.0F;
            g1.DrawArc(Pens.Black, 300, 100, 89, 46, startAngle, sweepAngle);
        }

    }
}
