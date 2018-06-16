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
    [Serializable]
    public partial class EN1999_1_1Form : Form
    {
        DataTableForm dtform;
        OleDbDataReader dat_reader;
        DatabaseConnection dat_conn;
        #region Global variables definition and property method
        // Global Variables for Messageboxes initializing


        bool item0_check;

        public bool Item0_check
        {
            get { return item0_check; }
            set { item0_check = value; }
        }





        bool item1_check;

        public bool Item1_check
        {
            get { return item1_check; }
            set { item1_check = value; }
        }
        bool item2_check;

        public bool Item2_check
        {
            get { return item2_check; }
            set { item2_check = value; }
        }


        #endregion





        private void loadCombo1()
        {
            string item;
            comboBox1.Items.Clear();
            dat_reader = dat_conn.getDBReader("Select mat_name from ALUMINIUM_32a where id_Aluminium_T32a not in (1,2,3) order by id_Aluminium_T32a"); 
            while (dat_reader.Read())
            {
                item = dat_reader.GetValue(0).ToString();
                comboBox1.Items.Add(item);
            }

        }








        //Constructor
        public EN1999_1_1Form()
        {
            InitializeComponent();
           
            dat_conn = DatabaseConnection.getInstance();
            this.loadCombo1();  //loading data to combobox1
            
            dtform = new DataTableForm(1);    //for inicializing of object default value is 1











        }


        public void MessageBoxList ()
{

    EN1999_1_1 en = new EN1999_1_1(comboBox1.Text);
            
            #region // Database loaded data checking

    bool item0_check = true; // for function
            if (item0_check == true)
            #region Aluminium attributes
            {

                MessageBox.Show
                    (" Checking of loaded variables " + "\n"

                + "Steel variables: " + "\n"
                + "\n"
                + " fo = " + en.Fo + "\n"
                + " fu = " + en.Fu + "\n"

                + "\n"

                + " A = " + en.Mat_A + "\n"
                + " fo.haz = " + en.Fo_haz + "\n"
                + " fu.haz = " + en.Fu_haz + "\n"
                + " ρo.haz = " + en.Ro0_haz + "\n"
                + " ρu.haz = " + en.Rou_haz + "\n"
                + " BC = " + en.Mat_BC + "\n"
                + " np = " + en.Mat_np + "\n"

                + "\n"

                + " γM1 = " + en.GamaM1 + "\n"
                + " γM2 = " + en.GamaM2 + "\n"
                + " γM3 = " + en.GamaM3 + "\n"
                );

            }
            #endregion
            #endregion
}

        private void messageBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EN1999_1_1MessageBoxes i = new EN1999_1_1MessageBoxes();
            i.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /////////////////// LOADING OF MAIN CLASS ///////////////////

            //Main class object

            EN1999_1_1 en = new EN1999_1_1(comboBox1.Text);


            // docasna pomocna premenna
            item0_check = true;

            #region // Database loaded data checking
            if (item0_check == true)
            #region Aluminium attributes
            {

                MessageBox.Show
                    (" Checking of loaded variables " + "\n"
                // Missing units of variables
                + "Steel variables: " + "\n"
                + "\n"
                + " fo = " + en.Fo + "\n"
                + " fu = " + en.Fu + "\n"

                + "\n"

                + " A = " + en.Mat_A + "\n"
                + " fo.haz = " + en.Fo_haz + "\n"
                + " fu.haz = " + en.Fu_haz + "\n"
                + " ρo.haz = " + en.Ro0_haz + "\n"
                + " ρu.haz = " + en.Rou_haz + "\n"
                + " BC = " + en.Mat_BC + "\n"
                + " np = " + en.Mat_np + "\n"

                + "\n"

                + " γM1 = " + en.GamaM1 + "\n"
                + " γM2 = " + en.GamaM2 + "\n"
                + " γM3 = " + en.GamaM3 + "\n"
                );

            }
            #endregion

            #endregion

        }
    }
}
