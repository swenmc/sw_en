using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CENEX
{
    public partial class EN1993_1_1MessageBoxes : Form
    {
        #region Variables and property methods
        // Variables and property methods

        // Button - bool = true if all items are selected
        private bool selectedAllItems1=false;


        // selected items in checkedListBox1 true =  item is selected
        private List<bool> selectedItems = new List<bool>(8);

        public List<bool> SelectedItems
        {
            get { return selectedItems; }
            set { selectedItems = value; }
        }

        
        #endregion
            
        // Constructor
        public EN1993_1_1MessageBoxes()
        {
            InitializeComponent();
            loadCheckedListBox();
            
        }

        private void loadCheckedListBox() 
        {
            try
            {
                FileStream fsIn = new FileStream("MessageBoxes.dat", FileMode.Open);
                BinaryFormatter binF2 = new BinaryFormatter();
                this.selectedItems = (List<bool>)binF2.Deserialize(fsIn);
                fsIn.Close();
            }
            catch (FileNotFoundException)
            {
                this.saveData();
                return;
            }
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, selectedItems[i]);
            }
        }

        
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.saveData();
            this.DialogResult = DialogResult.OK;
        }
        private void saveData() 
        {
            selectedItems.Clear();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                selectedItems.Add(checkedListBox1.GetItemChecked(i));
            }
            FileStream fsOut = new FileStream("MessageBoxes.dat", FileMode.Create);
            BinaryFormatter binF = new BinaryFormatter();
            binF.Serialize(fsOut, selectedItems);
            fsOut.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool temp;
            if (selectedAllItems1)
            {
                temp = false;
                selectedAllItems1 = false;
            }
            else
            {
                temp = true;
                selectedAllItems1 = true;
            }

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, temp);
            }
        }

        

        
       




    }
}
