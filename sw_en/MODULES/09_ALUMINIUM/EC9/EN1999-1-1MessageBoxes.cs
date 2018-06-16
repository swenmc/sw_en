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
    public partial class EN1999_1_1MessageBoxes : Form
    {
        #region Variables and property methods
        // Variables and property methods

        // Button - bool = true if all items are selected
        private bool selectedAllItems1;


        // selected items in checkedListBox1 true =  item is selected
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



        #endregion

        // Constructor
        public EN1999_1_1MessageBoxes()
        {
            InitializeComponent();
        
            if (checkedListBox1.GetItemChecked(0)) item0_check = true;
            if (checkedListBox1.GetItemChecked(1)) item1_check = true;
            

           // this.SystemMessageBoxShow ();
           



            // pomocna docasna premenna
            item0_check = true;
           
        }
        private void SystemMessageBoxShow ()
        {
            MessageBox.Show(
                           " Show Message 0: " + item0_check +
                           " Show Message 1: " + item1_check );
                           
                           
            


        }






        private void button1_Click(object sender, EventArgs e)
        {
            // Class object

            EN1999_1_1MessageBoxes en = new EN1999_1_1MessageBoxes();
            this.SystemMessageBoxShow ();
            


            if (!selectedAllItems1)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
                selectedAllItems1 = true;
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
                selectedAllItems1 = false;
            }
        }
    }
}
