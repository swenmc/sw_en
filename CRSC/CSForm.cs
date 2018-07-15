using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace CRSC
{
    public partial class CSForm : Form
    {
        private List<double> ySuradnice;
        private List<double> zSuradnice;
        private List<double> ySuradniceDatagrid;
        private List<double> zSuradniceDatagrid;
        private List<double> tHodnoty;
        private List<int> idHodnoty;

        public bool bIsShapeSolid;
        public float[,] arrPointCoord;

        //Constructor
        public CSForm()
        {
            InitializeComponent();

            ySuradnice = new List<double>(5);  //for counting
            zSuradnice = new List<double>(5);
            ySuradniceDatagrid = new List<double>(5); //for writing values to the picture
            zSuradniceDatagrid = new List<double>(5);
            tHodnoty = new List<double>(5);
            idHodnoty = new List<int>(5);

            // Load example data - default
            /*
            CSO cs = new CSO(); // Choose class CSO (open) or CSC (closed)
            cs.CrScDefPoints_EX_2710115();
            */

            CSC cs = new CSC(); // Choose class CSO (open) or CSC (closed)
            cs.CrScDefPoints_EX_10075();

            bIsShapeSolid = cs.IsShapeSolid;
            arrPointCoord = new float[cs.arrPointCoord.Length / 3, 3];

            for (int i = 0; i < arrPointCoord.Length / 3; i++)
            {
                arrPointCoord[i, 0] = cs.arrPointCoord[i, 0];
                arrPointCoord[i, 1] = cs.arrPointCoord[i, 1];
                arrPointCoord[i, 2] = cs.arrPointCoord[i, 2];
            }

            //Fill example data into datagrid
            FillDataGridView();

            // Draw default cross-section
            this.drawPictureFromDatagrid();

            /*
            DataSet ds = new DataSet();
            DataTable table = new DataTable();
            DataRow row = table.NewRow();
            row[0] = "stlpec0";
            row[1] = "stlpec1";
            row[2] = "stlpec2";
            table.Rows.Add(row);
            ds.Tables.Add(table);
            */
        }

        public void button1_Click(object sender, EventArgs e)
        {
            CCrSc_TW cs;

            if (bIsShapeSolid)
            {
                cs = new CSO();
            }
            else
            {
                cs = new CSC();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Calculation of cross section attributes
            this.getListsFromDatagrid();

            // Check cross-section shape and create open or closed cross-section class

            // Temporary acc. to default
            CCrSc_TW cs;

            if (bIsShapeSolid)
            {
                cs = new CSO(this.ySuradniceDatagrid, this.zSuradniceDatagrid, this.tHodnoty);
            }
            else
            {
                cs = new CSC(this.ySuradniceDatagrid, this.zSuradniceDatagrid, this.tHodnoty);
            }

            // Round numerical values
            int dec_place_num1 = 1;
            int dec_place_num2 = 2;

            double d_A = Math.Round(cs.A_g, dec_place_num2);
            double d_A_vy = Math.Round(cs.A_vy, dec_place_num2);
            double d_A_vz = Math.Round(cs.A_vz, dec_place_num2);
            double d_y_gc = Math.Round(cs.D_y_gc, dec_place_num2);
            double d_z_gc = Math.Round(cs.D_z_gc, dec_place_num2);
            double d_S_y0 = Math.Round(cs.S_y0, dec_place_num2);
            double d_S_z0 = Math.Round(cs.S_z0, dec_place_num2);
            double d_I_y = Math.Round(cs.I_y, dec_place_num2);
            double d_I_z = Math.Round(cs.I_z, dec_place_num2);
            double d_Wy_el_1 = Math.Round(cs.W_y_el_1, dec_place_num2);
            double d_Wz_el_1 = Math.Round(cs.W_z_el_1, dec_place_num2);
            double d_Wy_el_2 = Math.Round(cs.W_y_el_2, dec_place_num2);
            double d_Wz_el_2 = Math.Round(cs.W_z_el_2, dec_place_num2);

            double d_Alpha = Math.Round(cs.Alpha_rad, dec_place_num2);
            double d_I_yz = Math.Round(cs.I_yz, dec_place_num2);
            double d_I_eps = Math.Round(cs.I_epsilon, dec_place_num2);
            double d_I_eta = Math.Round(cs.I_mikro, dec_place_num2);
            double d_I_ome = Math.Round(cs.Iomega, dec_place_num2);
            double d_ome_mean = Math.Round(cs.Omega_mean, dec_place_num2);
            double d_ome_max = Math.Round(cs.Omega_max, dec_place_num2);
            double d_I_y_ome = Math.Round(cs.Iy_omega, dec_place_num2);
            double d_I_z_ome = Math.Round(cs.Iz_omega, dec_place_num2);
            double d_I_ome_ome = Math.Round(cs.Iomega_omega, dec_place_num2);

            double d_y_s = Math.Round(cs.D_y_s, dec_place_num2);
            double d_z_s = Math.Round(cs.D_z_s, dec_place_num2);
            double d_I_p = Math.Round(cs.Ip, dec_place_num2);
            double d_y_j = Math.Round(cs.D_y_j, dec_place_num2);
            double d_z_j = Math.Round(cs.D_z_j, dec_place_num2);
            double d_I_w = Math.Round(cs.I_w, dec_place_num2);
            double d_W_w = Math.Round(cs.W_w, dec_place_num2);
            double d_I_t = Math.Round(cs.I_t, dec_place_num2);
            double d_W_t = Math.Round(cs.W_t_el, dec_place_num2);

            double d_Beta_y = Math.Round(cs.Beta_y, dec_place_num2);
            double d_Beta_z = Math.Round(cs.Beta_z, dec_place_num2);

            //vymazanie datagridview2
            dataGridView2.Rows.Clear();
            //Pridavanie Riadkov do Datagridview2 
            //Potrebne popridavat vsetky premenne,ktore chceme zobrazit

            string s_sup_2 = "\xB2";
            string s_sup_3 = "\xB3";
            string s_sup_4 = "\u2074";
            string s_sup_6 = "\u2076";

            string s_unit_length = "mm";

            string s_unit_area = s_unit_length + s_sup_2;
            string s_unit_first_moment_of_area = s_unit_length + s_sup_3;
            string s_unit_second_moment_of_area = s_unit_length + s_sup_4;
            string s_unit_moment_omega = s_unit_length + s_sup_6;

            dataGridView2.Rows.Add("Ag ="     , d_A,       s_unit_area,                  "Avy ="       , d_A_vy, s_unit_area,                    "Avz ="         , d_A_vz     , s_unit_area);
            dataGridView2.Rows.Add("ygc ="    , d_y_gc,    s_unit_length,                "SyO ="       , d_S_y0, s_unit_first_moment_of_area,    "Iy ="          , d_I_y      , s_unit_second_moment_of_area);
            dataGridView2.Rows.Add("zgc ="    , d_z_gc,    s_unit_length,                "SzO ="       , d_S_z0, s_unit_first_moment_of_area,    "Iz ="          , d_I_z      , s_unit_second_moment_of_area);
            dataGridView2.Rows.Add("Wyel1 ="  , d_Wy_el_1, s_unit_length,                "Wzel1 ="     , d_Wz_el_1, s_unit_first_moment_of_area, " "             , " "        , " "  );
            dataGridView2.Rows.Add("Wyel2 ="  , d_Wy_el_2, s_unit_length,                "Wzel2 ="     , d_Wz_el_2, s_unit_first_moment_of_area, " "             , " "        , " "  );
            dataGridView2.Rows.Add("α ="      , d_Alpha,   "rad",                        " "           , " ",                        " "  ,      "Iyz ="         , d_I_yz     , s_unit_second_moment_of_area);
            dataGridView2.Rows.Add("Iξ ="     , d_I_eps,   s_unit_second_moment_of_area, "Iη ="        , d_I_eta, s_unit_second_moment_of_area,  " "             , " "        , " "  );
            dataGridView2.Rows.Add("Iω ="     , d_I_ome,   s_unit_moment_omega,          "ω mean ="    , d_ome_mean, s_unit_area,                "ω max ="       , d_ome_max  , s_unit_area);
            dataGridView2.Rows.Add("Iyω ="    , d_I_y_ome, s_unit_moment_omega,          "Izω ="       , d_I_z_ome, s_unit_moment_omega,         "Iωω ="         , d_I_ome_ome, s_unit_moment_omega);
            dataGridView2.Rows.Add("ys ="     , d_y_s,     s_unit_length,                "zs ="        , d_z_s, s_unit_length,                   "Ip ="          , d_I_p      , s_unit_second_moment_of_area);
            dataGridView2.Rows.Add("yj ="     , d_y_j,     s_unit_length,                "zj ="        , d_z_j, s_unit_length,                   " "             , " "        , " "  );
            dataGridView2.Rows.Add("Iw ="     , d_I_w,     s_unit_moment_omega,          "Ww ="        , d_W_w, s_unit_first_moment_of_area,     " "             , " "        , " "  );
            dataGridView2.Rows.Add("It ="     , d_I_t,     s_unit_second_moment_of_area, "Wt ="        , d_W_t, s_unit_first_moment_of_area,     " "             , " "        , " "  );
            dataGridView2.Rows.Add("βy ="     , d_Beta_y,  s_unit_length,                "βz ="        , d_Beta_z, s_unit_length,                " "             , " "        , " "  );
        }

        private void FillDataGridView()
        {
            for (int i = 0; i < arrPointCoord.Length / 3; i++)
            {
                dataGridView1.Rows.Add(new DataGridViewRow());
                dataGridView1.Rows[i].Cells[0].Value = i + 1;
                dataGridView1.Rows[i].Cells[1].Value = arrPointCoord[i, 0];
                dataGridView1.Rows[i].Cells[2].Value = arrPointCoord[i, 1];
                dataGridView1.Rows[i].Cells[3].Value = arrPointCoord[i, 2];
            }
        }

        private void getListsFromDatagrid()
        {
            int id;
            double y, z, t;
            ySuradnice.Clear();
            zSuradnice.Clear();
            ySuradniceDatagrid.Clear();
            zSuradniceDatagrid.Clear();
            tHodnoty.Clear();
            idHodnoty.Clear();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                y = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value.ToString().Replace(",", "."), new CultureInfo("en-us"));
                z = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value.ToString().Replace(",", "."), new CultureInfo("en-us"));
                t = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value.ToString().Replace(",", "."), new CultureInfo("en-us"));
                id = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value.ToString());
                ySuradnice.Add(y);
                zSuradnice.Add(z);
                ySuradniceDatagrid.Add(y);
                zSuradniceDatagrid.Add(z);
                tHodnoty.Add(t);
                idHodnoty.Add(id);
            }
        }
        //funkcia na najdenie maxima v zozname int hodnot
        private int findMax(List<int> zoznam) 
        {
            int max = zoznam[0];
            for (int i = 1; i < zoznam.Count; i++) 
            {
                if (zoznam[i] > max) max = zoznam[i];
            }
            return max;
        }
        //funkcia na najdenie maxima v zozname double hodnot
        private double findMax(List<double> zoznam)
        {
            double max = zoznam[0];
            for (int i = 1; i < zoznam.Count; i++)
            {
                if (zoznam[i] > max) max = zoznam[i];
            }
            return max;
        }

        //funkcia na najdenie minima v zozname int hodnot
        private int findMin(List<int> zoznam)
        {
            int min = zoznam[0];
            for (int i = 1; i < zoznam.Count; i++)
            {
                if (zoznam[i] < min) min = zoznam[i];
            }
            return min;
        }

        //funkcia na najdenie minima v zozname double hodnot
        private double findMin(List<double> zoznam)
        {
            double min = zoznam[0];
            for (int i = 1; i < zoznam.Count; i++)
            {
                if (zoznam[i] < min) min = zoznam[i];
            }
            return min;
        }

        private double countZoomForPicture(double xmax,double xmin,double ymax,double ymin) 
        {
            double pomer;
            int width = pictureBox1.Width - 60; //vynechane nejake miesto od okrajov
            int height = pictureBox1.Height - 60;
            if (width != height) MessageBox.Show("Nerovnake velkosti width a height obrazka");
            double maxDifference;
            try
            {
                maxDifference = Math.Max(xmax - xmin, ymax - ymin);
                pomer = width / maxDifference;
                //MessageBox.Show("pomer:" + pomer);
            }
            catch (DivideByZeroException) { pomer = 1; }
                
            return pomer;
        }

        private void countPositionsForLists() 
        {
            double minx = this.findMin(ySuradnice);
            double miny = this.findMin(zSuradnice);
            double posun_x, posun_y;

            if (minx < 0) 
            {
                posun_x = Math.Abs(minx);
                for (int i = 0; i < ySuradnice.Count; i++)
                {
                    ySuradnice[i] += posun_x;
                }
            }
            if (minx > 0) 
            {
                posun_x = minx;
                for (int i = 0; i < ySuradnice.Count; i++)
                {
                    ySuradnice[i] -= posun_x;
                }
            }
            if (miny < 0)
            {
                posun_y = Math.Abs(miny);
                for (int i = 0; i < zSuradnice.Count; i++)
                {
                    zSuradnice[i] += posun_y;
                }
            }
            if (miny > 0)
            {
                posun_y = miny;
                for (int i = 0; i < zSuradnice.Count; i++)
                {
                    zSuradnice[i] -= posun_y;
                }
            }
           
        }

        private void drawPictureFromDatagrid()
        {
            Image image = Image.FromFile(@"biely.bmp");
            Bitmap myBitmap = new Bitmap(image, pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(myBitmap);

            bool bDrawPoints = true;
            bool bDrawElements = true;
            bool bDraPointNumbers = true;
            bool bDrawPOintCoordinates = false;

            float fPointSquareSize = 1f;

            int iPointTextSize = 7;
            string sPointTextFont = "Arial";
            int iPointCoordinatesTextSize = 7;
            string sPointCoordinatesTextFont = "Arial";

            Color cPointColor = Color.Black;
            Color cPointNumberColor = Color.Blue;
            Color cPointCoordinatesColor = Color.DarkGreen;

            Color cElementColor = Color.Red;
            float fElementDefaultThickness = 2f;

            Font font1 = new Font(sPointTextFont, iPointTextSize, FontStyle.Bold);
            Font font2 = new Font(sPointCoordinatesTextFont, iPointCoordinatesTextSize);

            Brush brush1 = new SolidBrush(cPointNumberColor);
            Brush brush2 = new SolidBrush(cPointCoordinatesColor);

            Pen p = new Pen(cElementColor, fElementDefaultThickness);

            p.DashStyle = DashStyle.Solid;

            double y1, z1, t1, y2, z2, t2;
            int okraj_y = 50;
            int okraj_z = 30;

            int posun_text_y = 30;
            int posun_text_z = 20;

            //cast inicializacie a prepoctov suradnic pre maximalne vykreslenie
            try
            {
                this.getListsFromDatagrid();   //ziskanie zoznamov int suradnic 
            }
            catch (FormatException) { MessageBox.Show("Attributes in table in wrong format!"); return; }
            catch (NullReferenceException) { MessageBox.Show("Set all atributes in a row!"); return; }
            
            double ymax = this.findMax(ySuradnice);  //najdenie maximalnej hodnoty suradnice y
            double ymin = this.findMin(ySuradnice);
            double zmax = this.findMax(zSuradnice);
            double zmin = this.findMin(zSuradnice);
            double pomer;

            pomer = this.countZoomForPicture(ymax, ymin, zmax, zmin);  //vypocet pomeru
            this.countPositionsForLists(); //nastavenie hodnot na pociatocny bod

            if (ySuradnice.Count > 1)
            {
                for (int i = 0; i < ySuradnice.Count - 1; i++)
                {
                    y1 = (ySuradnice[i] * pomer + okraj_y);
                    z1 = 400 - (zSuradnice[i] * pomer + okraj_z);
                    t1 = tHodnoty[i];
                    y2 = (ySuradnice[i + 1] * pomer + okraj_y);
                    z2 = 400 - (zSuradnice[i + 1] * pomer + okraj_z);
                    t2 = tHodnoty[i + 1];

                    if (bDrawElements)
                    {
                        p = new Pen(cElementColor, /*fElementDefaultThickness * */(float)t2);
                        if (t2 > 0) g.DrawLine(p, (int)Math.Round(y1), (int)Math.Round(z1), (int)Math.Round(y2), (int)Math.Round(z2));
                    }

                    if (bDrawPoints)
                    {
                        p = new Pen(cPointColor, 4);
                        g.DrawRectangle(p, (float)y1, (float)z1, fPointSquareSize, fPointSquareSize);
                    }

                    if(bDraPointNumbers)
                       g.DrawString(idHodnoty[i].ToString(), font1, brush1, (float)y1 - 5, (float)z1 + 4);

                    if(bDrawPOintCoordinates)
                       g.DrawString("["+ ySuradniceDatagrid[i]+";"+ zSuradniceDatagrid[i]+"]",
                                font2, brush2, (float)y1 - posun_text_y, (float)z1 - posun_text_z);

                    if (bDrawPoints)
                    {
                        p = new Pen(cPointColor, 4);
                        g.DrawRectangle(p, (float)y2, (float)z2, fPointSquareSize, fPointSquareSize);
                    }

                    if(bDraPointNumbers)
                       g.DrawString(idHodnoty[i + 1].ToString(), font1, brush1, (float)y2 - 5, (float)z2 + 4);

                    if(bDrawPOintCoordinates)
                       g.DrawString("[" + ySuradniceDatagrid[i+1] + ";" + zSuradniceDatagrid[i+1] + "]",
                                font2, brush2, (float)y2 - posun_text_y, (float)z2 - posun_text_z);

                }
            }
            else
            {
                y1 = ySuradnice[0] + okraj_y;
                z1 = 400- (zSuradnice[0] + okraj_z);
                p = new Pen(Color.Black, 4);

                if (bDrawPoints)
                {
                    p = new Pen(cPointColor, 4);
                    g.DrawRectangle(p, (float)y1, (float)z1, fPointSquareSize, fPointSquareSize);
                }

                if (bDraPointNumbers)
                    g.DrawString(idHodnoty[0].ToString(), font1, brush1, (float)y1 - 5, (float)z1 + 4);
                if (bDrawPOintCoordinates)
                    g.DrawString("[" + ySuradniceDatagrid[0] + ";" + zSuradniceDatagrid[0] + "]",
                                font2, brush2, (float)y1 - posun_text_y, (float)z1 - posun_text_z);

            }

            p.Dispose();
            g.Dispose();
            pictureBox1.Image = myBitmap;
        }

        private void buttonDraw_Click(object sender, EventArgs e)
        {
            try
            {
                this.drawPictureFromDatagrid();

                // Check cross-section shape and create open or closed cross-section class
                CCrSc_TW cs;

                // Temporary acc. to default
                if (bIsShapeSolid)
                {
                    cs = new CSO(ySuradnice, zSuradnice, tHodnoty);
                }
                else
                {
                    cs = new CSC(ySuradnice, zSuradnice, tHodnoty);
                }
            }
            catch (ArgumentOutOfRangeException) { MessageBox.Show("Set values to y,z,t in the table."); }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.drawPictureFromDatagrid();
            }
            catch (ArgumentOutOfRangeException) { MessageBox.Show("Set values to y,z,t in the table."); }

        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            for (int i = 1; i < dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i-1].Cells[0].Value = i;
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            for (int i = 1; i < dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i-1].Cells[0].Value = i;
        }
    }
}
