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
    public partial class Kc : Form
    {
        int kc_num;


        // Constructor
        public Kc(int kc_num)
        {
            InitializeComponent();
            this.kc_num = kc_num;
            loadPictureOfMomentDiagram(this.kc_num);

        }

        // draw pictures of moment diagrams

        // comboBoxMomentDiag_kc
        // picturesOfMomentDiagrams_Kc_ToolStripMenuItem



        private void loadPictureOfMomentDiagram(int kc_num)
        {

            // Kc factors Table 6.6
            Image image = Image.FromFile(@"EC3-1-1_pictures\table_6.6.jpg");
            Bitmap myBitmap = new Bitmap(image, kcBox1.Width, kcBox1.Height);
            Graphics g = Graphics.FromImage(myBitmap);
            kcBox1.Image = myBitmap;

        }

        private void Kc_Resize(object sender, EventArgs e)
        {
            kcBox1.Dock = DockStyle.Fill;
            loadPictureOfMomentDiagram(kc_num);
        }
        private void comboBoxAnnex_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.loadPictureOfMomentDiagram(kc_num);
        }






    }


}
