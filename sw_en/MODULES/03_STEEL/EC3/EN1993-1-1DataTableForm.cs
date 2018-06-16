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
using System.Runtime.Serialization;
using System.Collections;
using System.Data.OleDb;
using DATABASE;

namespace CENEX
{
    public partial class DataTableForm : Form
    {
        OleDbDataReader dat_reader;
        DatabaseConnection dat_conn;
        private bool selectedAllItems1;
        private bool selectedAllItems2;
        private bool selectedAllItems3;
        private bool selectedAllItems4;

        private CheckedListInfo[,] zozChLInfos = new CheckedListInfo[11, 3];

        public CheckedListInfo[,] ZozChLInfos
        {
            get { return zozChLInfos; }
            set { zozChLInfos = value; }
        }

        private byte typ_prierezu;

        public byte Typ_prierezu
        {
            get { return typ_prierezu; }
            set { typ_prierezu = value; }
        }


        // DATA1-SECTIONS   number of columns in Database is 141
        CheckedListInfo info1;
        CheckedListInfo info2;
        CheckedListInfo info3;
        object[] collection1 = new object[141];   //tolko stlpcov je v databaze SECTIONS(141)
        object[] collection2 = new object[141];
        object[] collection3 = new object[141];

        // DATA1-STEEL number of columns in database sheet is 20
        CheckedListInfo infoSteel = new CheckedListInfo();
        object[] collection4 = new object[20];   //tolko stlpcov je v databaze STEEL(20)
        object[] collection5 = new object[20];
        object[] collection6 = new object[20];

        public DataTableForm(byte typ_prierezu)
        {
            InitializeComponent();
            
            dat_conn = DatabaseConnection.getInstance();
            this.typ_prierezu = typ_prierezu;
            
            //najprv sa naloaduju data z databazy 
            //obnovi sa tak zoznam zozChLInfos zo suboru checklist.dat
            this.LoadData();
            this.nacitajCheckedListBox();
            selectedAllItems1 = false;
            selectedAllItems2 = false;
            selectedAllItems3 = false;
            selectedAllItems4 = false;
        }

        //private void inicializeZozChlInfo()
        //{
        //    for (int i = 0; i <= 10; i++)
        //        for (int j = 0; j <= 2; j++)
        //        {
        //            zozChLInfos[i, j] = new CheckedListInfo();
        //        }
        //    zozChLInfos[0, 0] = new CheckedListInfo(); //vyhradene pre Steel
        //}

        #region metody inicializujuce nastavenia,su volane iba raz ak nie je dostupny subor "checkList.dat"
        public void loadDataUserInputandResult(byte typ_pr) 
        {
            info2 = new CheckedListInfo();
            //MessageBox.Show("loadDataUserInputandResult"+typ_prierezu);
            // Checked List Box2 - USER DATA
            for (int i = 0; i < checkedListBox3.Items.Count; i++)
            {
                info2.ZoznamPremennych.Add(checkedListBox3.Items[i].ToString());
                info2.ZoznamZasktPrem.Add(false);
            }

            info3 = new CheckedListInfo();
            // Checked List Box3 - CENEX DATA - OUTPUT
            for (int i = 0; i < checkedListBox4.Items.Count; i++)
            {
                info3.ZoznamPremennych.Add(checkedListBox4.Items[i].ToString());
                info3.ZoznamZasktPrem.Add(false);
            }
            zozChLInfos[typ_pr, 1] = this.info2;
            zozChLInfos[typ_pr, 2] = info3;
            this.SaveData();
            
        }
        private void loadDataForSection(byte typ_prierezu)
        {
            //MessageBox.Show("loadDataForSection()"+typ_prierezu);
            // Data from DATA1-SECTIONS_VAR2 - Variable
            string sql = "Select * from sections_var2 where csprofshape_var like '" + typ_prierezu + "'";
            dat_reader = dat_conn.getDBReader(sql);
            dat_reader.Read();
            dat_reader.GetValues(collection1);

            // Data from DATA1-SECTIONS_VAR2 - Variable Table Name
            sql = "Select * from sections_var2 where id_sections_var2 like '1'";
            dat_reader = dat_conn.getDBReader(sql);
            dat_reader.Read();
            dat_reader.GetValues(collection2);
            // Data from DATA1-SECTIONS_VAR2 - Units
            sql = "Select * from sections_var2 where id_sections_var2 like '3'";
            dat_reader = dat_conn.getDBReader(sql);
            dat_reader.Read();
            dat_reader.GetValues(collection3);

            // SECTION VARIABLES CHECKED LIST BOX 1
            //prvych 5 stlpcov v databaze vynechavame
            info1 = new CheckedListInfo();
            for (int i = 5; i < collection1.Count(); i++)
            {
                if (!collection1[i].ToString().Equals(""))
                {
                    info1.ZoznamPremennych.Add(collection1[i].ToString());
                    info1.ZoznamZobrazPremennych.Add(collection2[i].ToString());
                    info1.ZoznamJednotiek.Add(collection3[i].ToString());
                    info1.ZoznamZasktPrem.Add(false);
                }
            }
            zozChLInfos[typ_prierezu, 0] = info1;
            this.SaveData();
        }

        private void loadDataForSteel()
        {
            //MessageBox.Show("loadDataForSteel()");
            // Data from DATA1-STEEL - Variable
            string sql2 = "Select * from Steel where id_steel like '1'";
            dat_reader = dat_conn.getDBReader(sql2);
            dat_reader.Read();
            dat_reader.GetValues(collection4);

            // Data from DATA1-STEEL - Variable Table name
            sql2 = "Select * from Steel where id_steel like '2'";
            dat_reader = dat_conn.getDBReader(sql2);
            dat_reader.Read();
            dat_reader.GetValues(collection5);

            // Data from DATA1-STEEL - Units
            sql2 = "Select * from Steel where id_steel like '3'";
            dat_reader = dat_conn.getDBReader(sql2);
            dat_reader.Read();
            dat_reader.GetValues(collection6);

            // STEEL VARIABLES CHECKED LIST BOX 4
            //ide sa od indexu 3 lebo prve tri stlpce z databazy vynechavame
            for (int i = 3; i < collection4.Count(); i++)
            {
                //loading data from database to lists ZoznamPremennych,ZoznamZobrazPremennych,ZoznamJednotiek
                infoSteel.ZoznamPremennych.Add(collection4[i].ToString());
                infoSteel.ZoznamZobrazPremennych.Add(collection5[i].ToString());
                infoSteel.ZoznamJednotiek.Add(collection6[i].ToString());
                infoSteel.ZoznamZasktPrem.Add(false);
            }
            zozChLInfos[0, 0] = infoSteel;
            this.SaveData();
        }
        #endregion

        private void nacitajCheckedListBox()
        {
            checkedListBox2.Items.Clear();
            checkedListBox1.Items.Clear();

            //pre checkedListbox1 su data ulozene v objekte info1
            
            for (int i = 0; i < info1.ZoznamZobrazPremennych.Count; i++)
            {
                checkedListBox2.Items.Add(info1.ZoznamZobrazPremennych[i], info1.ZoznamZasktPrem[i]);
            }
            for (int i = 0; i < info2.ZoznamPremennych.Count; i++)
            {
                checkedListBox3.SetItemChecked(i,info2.ZoznamZasktPrem[i]);
            }
            for (int i = 0; i < info3.ZoznamPremennych.Count; i++)
            {
                checkedListBox4.SetItemChecked(i,info3.ZoznamZasktPrem[i]);
            }

            //pre checkedListbox4 su data ulozene v objekte infoSteel
            for (int i = 0; i < infoSteel.ZoznamZobrazPremennych.Count; i++)
            {
                checkedListBox1.Items.Add(infoSteel.ZoznamZobrazPremennych[i], infoSteel.ZoznamZasktPrem[i]);
            }
        }

        private void LoadData()
        {
            //MessageBox.Show("LoadData()");
            try
            {
                FileStream fsIn = new FileStream("checkList.dat", FileMode.Open);
                BinaryFormatter binF2 = new BinaryFormatter();
                this.zozChLInfos = (CheckedListInfo[,])binF2.Deserialize(fsIn);
                fsIn.Close();

                //MessageBox.Show("load" + typ_prierezu.ToString());

                this.info1 = (CheckedListInfo)zozChLInfos[typ_prierezu, 0];
                this.info2 = (CheckedListInfo)zozChLInfos[typ_prierezu, 1];
                this.info3 = (CheckedListInfo)zozChLInfos[typ_prierezu, 2];
                // STEEL
                this.infoSteel = (CheckedListInfo)zozChLInfos[0, 0];  //reserved place for info about Steel
            }
            catch (FileNotFoundException) //ak nie je najdeny subor "checkList.dat"
            {
                //pre kazdy prierez zavolame merodu loadDataForSection a loadDataUserInputandResult(i);
                for (byte i = 1; i <= 10; i++)
                {
                    this.loadDataForSection(i);
                    this.loadDataUserInputandResult(i);
                }
                this.loadDataForSteel();
            }
            catch (NullReferenceException) { }
        }

        private void SaveData()
        {
            //MessageBox.Show("SaveData()");
            FileStream fsOut = new FileStream("checkList.dat", FileMode.Create);
            BinaryFormatter binF = new BinaryFormatter();
            binF.Serialize(fsOut, zozChLInfos);
            fsOut.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //tu sa ukladaju vlastne iba data o tom ktore premenne boli v checkedListe zaskrtane
            //takze sa prechadzaju cele checkedListboxy a data sa ukladaju do zoznamuZaskrtnutychPremenych

            // Checked List Box1 - SECTIONS
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                info1.ZoznamZasktPrem[i] = checkedListBox2.GetItemChecked(i);
            }
            // Checked List Box2 - USER DATA
            for (int i = 0; i < checkedListBox3.Items.Count; i++)
            {
                info2.ZoznamZasktPrem[i] = checkedListBox3.GetItemChecked(i);
            }
            // Checked List Box3 - CENEX DATA - OUTPUT
            for (int i = 0; i < checkedListBox4.Items.Count; i++)
            {
                info3.ZoznamZasktPrem[i] = checkedListBox4.GetItemChecked(i);
            }
            // Checked List Box4 - STEEL
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                infoSteel.ZoznamZasktPrem[i] = checkedListBox1.GetItemChecked(i);
            }

            zozChLInfos[typ_prierezu, 0] = info1;
            zozChLInfos[typ_prierezu, 1] = info2;
            zozChLInfos[typ_prierezu, 2] = info3;
            zozChLInfos[0, 0] = infoSteel; //[0,0] is reserved place for data from STEEL dopisal som nieco

            this.SaveData();
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonLOAD_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }


        #region methods for checking and unchecking of all items in checkedListBox

        private void button1_Click(object sender, EventArgs e)
        {
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (!selectedAllItems2)
            {
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    checkedListBox2.SetItemChecked(i, true);
                }
                selectedAllItems2 = true;
            }
            else
            {
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    checkedListBox2.SetItemChecked(i, false);
                }
                selectedAllItems2 = false;
            }
        }
  
        private void button3_Click(object sender, EventArgs e)
        {
            if (!selectedAllItems3)
            {
                for (int i = 0; i < checkedListBox3.Items.Count; i++)
                {
                    checkedListBox3.SetItemChecked(i, true);
                }
                selectedAllItems3 = true;
            }
            else
            {
                for (int i = 0; i < checkedListBox3.Items.Count; i++)
                {
                    checkedListBox3.SetItemChecked(i, false);
                }
                selectedAllItems3 = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!selectedAllItems4)
            {
                for (int i = 0; i < checkedListBox4.Items.Count; i++)
                {
                    checkedListBox4.SetItemChecked(i, true);
                }
                selectedAllItems4 = true;
            }
            else
            {
                for (int i = 0; i < checkedListBox4.Items.Count; i++)
                {
                    checkedListBox4.SetItemChecked(i, false);
                }
                selectedAllItems4 = false;
            }

        }

        #endregion


    }
}
