using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using DATABASE;

namespace CENEX
{
    public partial class PictureForm : Form
    {
        OleDbDataReader dat_reader;
        DatabaseConnection dat_conn;
        string combo_text;
        byte typ_prierezu;

        public PictureForm(string str,byte typ_prierezu)
        {
            InitializeComponent();
            dat_conn = DatabaseConnection.getInstance();
            this.combo_text = str;
            this.typ_prierezu = typ_prierezu;
            this.loadPictureOfCrossSection(typ_prierezu);
        }
        //vykresli obrazok
        private void loadPictureOfCrossSection(byte num)
        {
            string command = "Select h,b,tw,tf,r,d from SECTIONS where csprof_name like '" + combo_text + "'";
            string h = "h", b = "b", tw = "tw", tf = "tf", r = "r", d = "d";
            dat_reader = dat_conn.getDBReader(command);
            dat_reader.Read();
            try
            {
                h = dat_reader.GetValue(0).ToString();
                b = dat_reader.GetValue(1).ToString();
                tw = dat_reader.GetValue(2).ToString();
                tf = dat_reader.GetValue(3).ToString();
                r = dat_reader.GetValue(4).ToString();
                d = dat_reader.GetValue(5).ToString();

            }
            catch (FormatException) { MessageBox.Show("Probably wrong selected cross-section."); }
            catch (InvalidOperationException) { MessageBox.Show("Probably wrong selected cross-section."); }

            switch (num)
            {
                case 1:
                    Image image = Image.FromFile(@"prierezy\prier1.jpg");
                    Bitmap myBitmap = new Bitmap(image, pictureBox1.Width, pictureBox1.Height);
                    Graphics g = Graphics.FromImage(myBitmap);
                    Font f_text = new Font("Arial", 14, FontStyle.Bold);

                    g.DrawString(h, f_text, Brushes.Black, new PointF(2, pictureBox1.Height / 2 + 30));
                    g.DrawString(d, f_text, Brushes.Black, new PointF(pictureBox1.Width / 8, pictureBox1.Height / 2 + 30));
                    f_text = new Font("Arial", 16, FontStyle.Bold);
                    g.DrawString(tw, f_text, Brushes.Black, new PointF(pictureBox1.Width / 2 +50, pictureBox1.Height / 2-30));
                    g.DrawString(tf, f_text, Brushes.Black, new PointF(pictureBox1.Width/1.15f, pictureBox1.Height/1.25f));
                    g.DrawString(r, f_text, Brushes.Black, new PointF(pictureBox1.Width / 2 + 80, pictureBox1.Height / 2 + 47));
                    f_text = new Font("Arial", 19, FontStyle.Bold);
                    g.DrawString(b, f_text, Brushes.Black, new PointF(pictureBox1.Width / 2 + 12, 10));
                    //g.DrawImage(myBitmap, AutoScrollPosition.X, AutoScrollPosition.Y);
                    pictureBox1.Image = myBitmap;
                    break;
                case 2: 
                    image = Image.FromFile(@"prierezy\prier2.jpg");
                    myBitmap = new Bitmap(image, pictureBox1.Width, pictureBox1.Height);
                    g = Graphics.FromImage(myBitmap);
                    f_text = new Font("Arial", 14, FontStyle.Bold);

                    g.DrawString(h, f_text, Brushes.Black, new PointF(2, pictureBox1.Height / 2 + 30));
                    f_text = new Font("Arial", 16, FontStyle.Bold);
                    g.DrawString(b, f_text, Brushes.Black, new PointF(pictureBox1.Width / 2 + 10, 12));
                    //g.DrawString(tw, f_text, Brushes.Black, new PointF(10, 0));
                    //g.DrawString(tf, f_text, Brushes.Black, new PointF(10, 0));
                    //g.DrawString(r, f_text, Brushes.Black, new PointF(10, 0));
                    //g.DrawString(d, f_text, Brushes.Black, new PointF(10, 0));

                    //g.DrawImage(myBitmap, AutoScrollPosition.X, AutoScrollPosition.Y);
                    pictureBox1.Image = myBitmap;
                    break;
                case 3: break;
                default: MessageBox.Show("Neimplementovane..."); break;
            }

        }

        private void PictureForm_Resize(object sender, EventArgs e)
        {
            pictureBox1.Dock = DockStyle.Fill;
            loadPictureOfCrossSection(typ_prierezu);
        }
    }
}
