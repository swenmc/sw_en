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
    public partial class Annex : Form
    {
        int annex_num;


        // Constructor
        public Annex(int annex_num) //annex = 1 - Method 1 (A), annex = 2, Method 2 (B)
        {
            InitializeComponent();
            
            
            this.annex_num = annex_num;
            loadPictureOfMomentDiagram(this.annex_num);

        }

        // draw pictures of moment diagrams    
        // comboBoxMomentDiag_y
        // picturesOfMomentDiagrams_Annex_My_ToolStripMenuItem

        private void loadPictureOfMomentDiagram(int annex_num)
        {         
            switch (annex_num)
            {
                
                case 0:
                    {
                        // Default: Annex B - Table B.3
                        Image image = Image.FromFile(@"EC3-1-1_pictures\table_B3.jpg");
                        Bitmap myBitmap = new Bitmap(image, annexBox1.Width, annexBox1.Height);
                        Graphics g = Graphics.FromImage(myBitmap);
                        annexBox1.Image = myBitmap;

                        break;
                    }                      
                case 1:
                    {
                        // kij = 1

                        MessageBox.Show (" No Annex used, interaction factors kij = 1.");

                        break;
                    }
                case 2:
                    {
                        // Annex A - Table A.2
                        Image image = Image.FromFile(@"EC3-1-1_pictures\table_A2.jpg");
                        Bitmap myBitmap = new Bitmap(image, annexBox1.Width, annexBox1.Height);
                        Graphics g = Graphics.FromImage(myBitmap);
                        annexBox1.Image = myBitmap;

                        break;
                    }
                case 3: 
                    {
                        // Annex B - Table B.3
                        Image image = Image.FromFile(@"EC3-1-1_pictures\table_B3.jpg");
                        Bitmap myBitmap = new Bitmap(image, annexBox1.Width, annexBox1.Height);
                        Graphics g = Graphics.FromImage(myBitmap);
                        annexBox1.Image = myBitmap;

                        break;
                    }

                default: MessageBox.Show("Unknow code Annex!"); break;
            }

 
        }

        private void Annex_Resize(object sender, EventArgs e)
        {
            annexBox1.Dock = DockStyle.Fill;
            loadPictureOfMomentDiagram(annex_num);
        }
        private void comboBoxAnnex_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.loadPictureOfMomentDiagram(annex_num);
        }




    }


}
