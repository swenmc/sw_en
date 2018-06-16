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

namespace MATERIAL
{
    public partial class Data_MatForm : Form
    {
        //DataTableForm dtform;
        OleDbDataReader dat_reader;
        DatabaseConnection dat_conn;

        //loading of data
        //data from mat_name from table CONCRETE in database
        //loading all elements from column mat_num except 0,1,List_name,Unit

        private void loadList()
        {
            string item;
            listView1.Items.Clear();
            dat_reader = dat_conn.getDBReader("Select mat_name from concrete where id_concrete not in (1,2,3) order by id_concrete"); // if cell type is text - string '2' 
            while (dat_reader.Read())
            {
                item = dat_reader.GetValue(0).ToString();
                listView1.Items.Add(item);
            }

            /*
            ListViewItem();
            
            listView1.Items.Add();
            ListViewItem.ListViewSubItem
            ListViewGroupCollection
            ListViewItem.ListViewSubItemCollection ();
            
            */


        }

        public Data_MatForm()
        {
            InitializeComponent();
            dat_conn = DatabaseConnection.getInstance();
            this.loadList();                  //loading data
            //dtform = new DataTableForm(1);    //for inicializing of object default value is 1
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
