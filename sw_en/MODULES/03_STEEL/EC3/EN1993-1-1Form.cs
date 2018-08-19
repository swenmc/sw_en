using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Office.Interop;
using PresentationControls;
using CENEX.MODULES.EC3;
using CENEX.MODULES._03_STEEL.EC3;
using EXPIMP;
using DATABASE;
//using Microsoft.Office.Interop.Excel;

namespace CENEX
{
    [Serializable]
    public partial class EN1993_1_1Form : Form
    {
        List<bool> data_menu = new List<bool>(10);
        List<string> zoznamNazvyPremennych = new List<string>(20);   //premenne do databazy
        List<string> zoznamMenuNazvy = new List<string>(20);          //premenne zobrazene v tabulke
        List<string> zoznamMenuHodnoty = new List<string>(20);        //hodnoty danych premennych
        List<string> zoznamMenuJednotky = new List<string>(20);        //jednotky danych premennych

        List<string> zoznamNazvyPremeSTEEL = new List<string>(20);        //premenne do databazy STEEL
        List<string> zoznamMenuNazvySTEEL = new List<string>(20);          //premenne zobrazene v tabulke
        List<string> zoznamMenuHodnotySTEEL = new List<string>(20);        //hodnoty danych premennych
        List<string> zoznamMenuJednotkySTEEL = new List<string>(20);        //jednotky danych premennych

        DataTableForm dtform;
        OleDbDataReader dat_reader;
        DatabaseConnection dat_conn;
        DataSetSteelInfo ds;

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
        bool item3_check;
        bool item4_check;
        bool item5_check;
        bool item6_check;
        
        
        #endregion



        #region Zero value of user input data
        // Zero value of user input data
        double N = 0, Vy = 0, Vz = 0, Mx = 0, My = 0, Mz = 0;

        double My_a = 0;
        double My_b = 0;
        double Mz_a = 0;
        double Mz_b = 0;
        double My_s = 0;
        double Mz_s = 0;
        double My_Ed_x = 0;
        double Mz_Ed_x = 0;
        double Deflection_y_x = 0;
        double Deflection_z_x = 0;

        double C1y = 0;
        double C2y = 0;
        double C3y = 0;
        double Kz_LT = 0;
        double Kw_LT = 0;

        double L_teor = 0;
        double L_y_buck = 0;
        double L_z_buck = 0;
        double L_y_LT = 0;
        double L_z_LT = 0;
        double L_T = 0;

        double Anet = 0;

        double MEd_T = 0;
        double VEd_T = 0;
        double E_T = 0;


        #endregion

        //byte typ_steel = 1;  // auxiliary
        byte typ_prierezu = 1;

        //constructor of class INfoSteelForm
        //first step is getting object of connection to the database from class DatabaseConnection
        public EN1993_1_1Form()
        {
            InitializeComponent();
            dat_conn = DatabaseConnection.getInstance();
            this.loadCombo1();  //loading data to combobox1
            this.loadCombo2();  //loading data to combobox2
            dtform = new DataTableForm(1);    //for inicializing of object default value is 1
          
        }
        public void Annex(int annex_num1)
        {
            // auxiliary metod for Annex Pictures - WIN FORM ANNEX
            // object of class Annex
            annex_num1 = comboBoxAnex.SelectedIndex;
            Annex en = new Annex(annex_num1);
            MessageBox.Show(" Annex number = " + annex_num1.ToString() + "");
        }
        public void Kc(int kc_num1)
        {
            kc_num1 = comboBoxMomentDiag_kc.SelectedIndex;
            Kc en = new Kc(kc_num1);
            MessageBox.Show(" Kc moment diagram number = " + kc_num1.ToString() + "");
        }  
  
        //loading of combobox1
        //data from mat_name from table STEEL in database
        //loading all elements from column mat_num except 0,1,List_name,Unit

        private void loadCombo1()
        {
            string item;
            comboBox1.Items.Clear();
            dat_reader = dat_conn.getDBReader("Select mat_name from Steel where id_steel not in (1,2,3) order by id_steel"); // if cell type is text - string '2' 
            while (dat_reader.Read())
            {
                item = dat_reader.GetValue(0).ToString();
                comboBox1.Items.Add(item);
            }

        }
        //loading of combobox2
        //data from column csprof_name from table Sections in database
        private void loadCombo2()
        {
            string item;
            comboBox2.Items.Clear();
            dat_reader = dat_conn.getDBReader("Select csprof_name from Sections where id_sections not in (1,2,3) order by id_sections");//,3,24,43,62,80,105,130,155,180,222,278)");
            while (dat_reader.Read())
            {
                item = dat_reader.GetValue(0).ToString();
                comboBox2.Items.Add(item);
            }

        }
        //deleting lists for updating aktual values
        private void deleteLists()
        {
            zoznamNazvyPremennych.Clear();
            zoznamMenuNazvy.Clear();
            zoznamMenuJednotky.Clear();
            zoznamMenuHodnoty.Clear();
            zoznamMenuHodnotySTEEL.Clear();
            zoznamMenuNazvySTEEL.Clear();
            zoznamMenuJednotkySTEEL.Clear();
            zoznamNazvyPremeSTEEL.Clear();
        }
        private void naplnZoznamy()
        {
            string combo1 = comboBox1.Text;     //actual text in combobox1
            string combo2 = comboBox2.Text;     //actual text in combobox2
            string unit = "nedef";
            CheckedListInfo chLInfo;      

            this.deleteLists();

            #region choosing typ_prierezu from database
            // checking type of cross-section from database for further processing
            string sql = "Select csprofshape from sections where csprof_name like '" + combo2 + "'";
            dat_reader = dat_conn.getDBReader(sql);
            dat_reader.Read();
            try
            {
                typ_prierezu = Convert.ToByte(dat_reader.GetValue(0).ToString());
            }
            catch (InvalidOperationException) { /*MessageBox.Show("Set up cross-section in ComboBox Cross-section, please!");*/
                                                //This type of exception appears mainly because text in combo2 is empty
                                                }
            catch (FormatException) { }
            #endregion

            dtform = new DataTableForm(typ_prierezu);
            
            try
            {
                #region filling lists with actual variables,their names in table and units
                chLInfo = dtform.ZozChLInfos[0, 0]; //for checkListbox 4. (STEEL)
                for (int i = 0; i < chLInfo.ZoznamPremennych.Count; i++)
                {
                    //if the variable is checked in checkedlistbox4 then add this variable to list of variables
                    if (chLInfo.ZoznamZasktPrem[i])
                    {
                        zoznamNazvyPremeSTEEL.Add(chLInfo.ZoznamPremennych[i]); //variables for database
                        zoznamMenuNazvySTEEL.Add(chLInfo.ZoznamZobrazPremennych[i]); //variables names for table 
                        zoznamMenuJednotkySTEEL.Add(chLInfo.ZoznamJednotiek[i]); //units of these variables
                    }
                }

                chLInfo = dtform.ZozChLInfos[typ_prierezu, 0]; //pre checkListbox 1.
                for (int i = 0; i < chLInfo.ZoznamPremennych.Count; i++)
                {
                    if (chLInfo.ZoznamZasktPrem[i])
                    {
                        zoznamNazvyPremennych.Add(chLInfo.ZoznamPremennych[i]); //premenne do databazy
                        zoznamMenuNazvy.Add(chLInfo.ZoznamZobrazPremennych[i]); //premenne zobrazene v tabulke
                        zoznamMenuJednotky.Add(chLInfo.ZoznamJednotiek[i]);
                        //MessageBox.Show(chLInfo.ZoznamPremennych[i] + "\n" + chLInfo.ZoznamZobrazPremennych[i] + "\n"
                        //    + chLInfo.ZoznamJednotiek[i]);
                    }
                }
                chLInfo = dtform.ZozChLInfos[typ_prierezu, 1]; //pre checkListbox 2.
                for (int i = 0; i < chLInfo.ZoznamPremennych.Count; i++)
                {
                    if (chLInfo.ZoznamZasktPrem[i])
                    {
                        zoznamMenuNazvy.Add(chLInfo.ZoznamPremennych[i]);  //premenne zadavane pouzivatelom
                        zoznamNazvyPremennych.Add(chLInfo.ZoznamPremennych[i]);
                        //MessageBox.Show(chLInfo.ZoznamPremennych[i]);
                    }
                }
                #endregion

                #region loading values for variables from database
                //getting values of actual choosen variables of STEEL
                for (int i = 0; i < zoznamNazvyPremeSTEEL.Count; i++)
                {
                    if (combo1 != "" && comboBox1.SelectedIndex != 0 && combo2 != "")
                    {
                        sql = "Select " + zoznamNazvyPremeSTEEL[i] + " from Steel where mat_name like '"+combo1+"'";
                        dat_reader = dat_conn.getDBReader(sql);
                        dat_reader.Read();
                        try
                        {
                            zoznamMenuHodnotySTEEL.Add(dat_reader.GetValue(0).ToString());
                        }
                        catch (InvalidOperationException) { }
                    }
                }
                //getting values of actual choosen variables of cross sections
                //if variable is from user input - getting character from database that determines
                //that it is user input,adding unit for this variable ,other visual style for this 
                //variable in datagrid is defined in method updateDataGridView()
                for (int i = 0; i < zoznamNazvyPremennych.Count; i++)
                {
                    if (combo2 != "" && combo1 != "")
                    {
                        sql = "Select ";
                        switch (zoznamNazvyPremennych[i])
                        {
                            #region Table - user-data cells basic loading - database "user" column

                            // Internal forces in section
                            case "N": sql += "user"; unit = "kN"; zoznamMenuJednotky.Add(unit); break;
                            case "Vy": sql += "user"; unit = "kN"; zoznamMenuJednotky.Add(unit); break;
                            case "Vz": sql += "user"; unit = "kN"; zoznamMenuJednotky.Add(unit); break;
                            case "Mx": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;
                            case "My": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;
                            case "Mz": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;

                            // End bending moments
                            case "My.a": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;
                            case "My.b": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;
                            case "Mz.a": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;
                            case "Mz.b": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;

                            // Midspan bending moments
                            case "My.s": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;
                            case "Mz.s": sql += "user"; unit = "kNm"; zoznamMenuJednotky.Add(unit); break;

                            // Annex A - Table A.2 input data
                            case "My.Ed.x": sql += "user"; unit = "[kNm]"; zoznamMenuJednotky.Add(unit); break;
                            case "Mz.Ed.x": sql += "user"; unit = "[kNm]"; zoznamMenuJednotky.Add(unit); break;
                            case "Deflection.y.x": sql += "user"; unit = "[mm]"; zoznamMenuJednotky.Add(unit); break;
                            case "Deflection.z.x": sql += "user"; unit = "[mm]"; zoznamMenuJednotky.Add(unit); break;

                            // Lateral buckling Mcr variables
                            case "C1y": sql += "user"; unit = "[-]"; zoznamMenuJednotky.Add(unit); break;
                            case "C2y": sql += "user"; unit = "[-]"; zoznamMenuJednotky.Add(unit); break;
                            case "C3y": sql += "user"; unit = "[-]"; zoznamMenuJednotky.Add(unit); break;
                            case "kz_LT": sql += "user"; unit = "[-]"; zoznamMenuJednotky.Add(unit); break;
                            case "kw_LT": sql += "user"; unit = "[-]"; zoznamMenuJednotky.Add(unit); break;

                            // Beam properties - lenght
                            case "L_teor": sql += "user"; unit = "mm"; zoznamMenuJednotky.Add(unit); break;
                            case "L_y_buck": sql += "user"; unit = "mm"; zoznamMenuJednotky.Add(unit); break;
                            case "L_z_buck": sql += "user"; unit = "mm"; zoznamMenuJednotky.Add(unit); break;
                            case "L_y_LT": sql += "user"; unit = "mm"; zoznamMenuJednotky.Add(unit); break;
                            case "L_z_LT": sql += "user"; unit = "mm"; zoznamMenuJednotky.Add(unit); break;
                            case "L_T": sql += "user"; unit = "mm"; zoznamMenuJednotky.Add(unit); break;

                            // Cross-section data

                            case "Anet": sql += "user"; unit = "mm2"; zoznamMenuJednotky.Add(unit); break;

                            // Variables - Torsion NAD CZE
                            case "MEd.T": sql += "user"; unit = "[kNm]"; zoznamMenuJednotky.Add(unit); break;
                            case "VEd.T": sql += "user"; unit = "[kN]"; zoznamMenuJednotky.Add(unit); break;
                            case "e.T": sql += "user"; unit = "[mm]"; zoznamMenuJednotky.Add(unit); break;

                            #endregion

                            default: sql += zoznamNazvyPremennych[i]; break;

                        }
                        sql += " from SECTIONS where csprof_name like '" + combo2 + "'";
                        dat_reader = dat_conn.getDBReader(sql);
                        dat_reader.Read();
                        try
                        {
                            zoznamMenuHodnoty.Add(dat_reader.GetValue(0).ToString());//zapise sa hodnota do zoznamu
                        }
                        catch (InvalidOperationException) { }

                    }
                }
#endregion
            }
            catch (NullReferenceException) { MessageBox.Show("There is nothing to display! Please choose some variables in Options -> Data table list."); }

        }
        //updating data in table 
        private void updateDataGridView()
        {
            ds = new DataSetSteelInfo();    //using dataset - like table
            DataTable table = ds.Tables["DataTableSteelInfo"];

            //Naplnenie tabulky
            //Vrati uz riadok podla navrhnutej schemy
            if (comboBox1.Text != "" && comboBox2.Text != "")
            {
                for (int i = 0; i < zoznamMenuNazvySTEEL.Count; i++)
                {
                    DataRow row = table.NewRow();    //new row in table

                    try
                    {
                        row["Premenna"] = zoznamMenuNazvySTEEL[i];
                        row["Hodnota"] = zoznamMenuHodnotySTEEL[i];
                        row["Jednotka"] = zoznamMenuJednotkySTEEL[i];
                        i++;
                        row["Premenna1"] = zoznamMenuNazvySTEEL[i];
                        row["Hodnota1"] = zoznamMenuHodnotySTEEL[i];
                        row["Jednotka1"] = zoznamMenuJednotkySTEEL[i];
                        i++;
                        row["Premenna2"] = zoznamMenuNazvySTEEL[i];
                        row["Hodnota2"] = zoznamMenuHodnotySTEEL[i];
                        row["Jednotka2"] = zoznamMenuJednotkySTEEL[i];
                    }
                    catch (ArgumentOutOfRangeException) { /*MessageBox.Show(e.Message);*/ }
                    table.Rows.Add(row);   //adding of new row with values to the table
                }

                for (int i = 0; i < zoznamMenuNazvy.Count; i++)
                {
                    DataRow row = table.NewRow();

                    try
                    {
                        row["Premenna"] = zoznamMenuNazvy[i];
                        row["Hodnota"] = zoznamMenuHodnoty[i];
                        row["Jednotka"] = zoznamMenuJednotky[i];
                        i++;
                        row["Premenna1"] = zoznamMenuNazvy[i];
                        row["Hodnota1"] = zoznamMenuHodnoty[i];
                        row["Jednotka1"] = zoznamMenuJednotky[i];
                        i++;
                        row["Premenna2"] = zoznamMenuNazvy[i];
                        row["Hodnota2"] = zoznamMenuHodnoty[i];
                        row["Jednotka2"] = zoznamMenuJednotky[i];
                    }
                    catch (ArgumentOutOfRangeException) { }
                    table.Rows.Add(row);
                }
                dataGridView1.DataSource = ds.Tables[0];  //draw the table to datagridview
            }
            
            //working with style of datagridview
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                dataGridView1.Rows[i].Cells[3].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                dataGridView1.Rows[i].Cells[6].Style.Font = new Font("Arial", 10, FontStyle.Bold);

                //bunky ktore sa maju vyplnat maju byt editovatelne
                try
                {
                    for (int k = 0; k <= 8; k++)
                        if (dataGridView1.Rows[i].Cells[k].Value.ToString().Equals("_"))
                        {
                            dataGridView1.Rows[i].Cells[k].Style.BackColor = Color.Yellow;

                        }
                        else dataGridView1.Rows[i].Cells[k].ReadOnly = true;

                }
                catch (NullReferenceException) { }
            }
            this.naplnZadanePolicka();
        }
        private void naplnZadanePolicka()
        {
            //MessageBox.Show("napln N= "+N);
            //if (N != 0 || Vy != 0 || Vz != 0 || Mx != 0 || My != 0 || Mz != 0 ||
            //    My_a != 0 || My_b != 0 || Mz_a != 0 || Mz_b != 0 || My_s != 0 || Mz_s != 0 ||
            //    My_Ed_x != 0 || Mz_Ed_x != 0 || Deflection_y_x != 0 || Deflection_z_x != 0 ||
            //    C1y != 0 || C2y != 0 || C3y != 0 || Kz_LT != 0 || Kw_LT != 0 ||
            //    L_teor != 0 || L_y_buck != 0 || L_z_buck != 0 || L_y_LT != 0 || L_z_LT != 0 || L_T != 0 ||
            //    Anet != 0 ||
            //    MEd_T != 0 || VEd_T != 0 || E_T != 0)
            //{
                try
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        for (int k = 0; k <= 6; k = k + 3)
                            switch (dataGridView1.Rows[i].Cells[k].Value.ToString())
                            {
                                #region Table cells for user input data
                                // Table cells for user input data

                                case "N": dataGridView1.Rows[i].Cells[k + 1].Value = N / 1000; break;
                                case "Vy": dataGridView1.Rows[i].Cells[k + 1].Value = Vy / 1000; break;
                                case "Vz": dataGridView1.Rows[i].Cells[k + 1].Value = Vz / 1000; break;
                                case "Mx": dataGridView1.Rows[i].Cells[k + 1].Value = Mx / 1000000; break;
                                case "My": dataGridView1.Rows[i].Cells[k + 1].Value = My / 1000000; break;
                                case "Mz": dataGridView1.Rows[i].Cells[k + 1].Value = Mz / 1000000; break;

                                // End bending moments
                                case "My.a": dataGridView1.Rows[i].Cells[k + 1].Value = My_a / 1000000; break;
                                case "My.b": dataGridView1.Rows[i].Cells[k + 1].Value = My_b / 1000000; break;
                                case "Mz.a": dataGridView1.Rows[i].Cells[k + 1].Value = Mz_a / 1000000; break;
                                case "Mz.b": dataGridView1.Rows[i].Cells[k + 1].Value = Mz_b / 1000000; break;

                                // Midspan bending moments
                                case "My.s": dataGridView1.Rows[i].Cells[k + 1].Value = My_s / 1000000; break;
                                case "Mz.s": dataGridView1.Rows[i].Cells[k + 1].Value = Mz_s / 1000000; break;

                                // Annex A - Table A.2 input data
                                case "My.Ed.x": dataGridView1.Rows[i].Cells[k + 1].Value = My_Ed_x / 1000000; break;
                                case "Mz.Ed.x": dataGridView1.Rows[i].Cells[k + 1].Value = Mz_Ed_x / 1000000; break;
                                case "Deflection.y.x": dataGridView1.Rows[i].Cells[k + 1].Value = Deflection_y_x / 1; break;
                                case "Deflection.z.x": dataGridView1.Rows[i].Cells[k + 1].Value = Deflection_z_x / 1; break;


                                // Lateral buckling and torsional parameters
                                case "C1y": dataGridView1.Rows[i].Cells[k + 1].Value = C1y / 1; break;
                                case "C2y": dataGridView1.Rows[i].Cells[k + 1].Value = C2y / 1; break;
                                case "C3y": dataGridView1.Rows[i].Cells[k + 1].Value = C3y / 1; break;
                                case "kz_LT": dataGridView1.Rows[i].Cells[k + 1].Value = Kz_LT / 1; break;
                                case "kw_LT": dataGridView1.Rows[i].Cells[k + 1].Value = Kw_LT / 1; break;

                                // Beam properties - lenght
                                case "L_teor": dataGridView1.Rows[i].Cells[k + 1].Value = L_teor / 1; break;
                                case "L_y_buck": dataGridView1.Rows[i].Cells[k + 1].Value = L_y_buck / 1; break;
                                case "L_z_buck": dataGridView1.Rows[i].Cells[k + 1].Value = L_z_buck / 1; break;
                                case "L_y_LT": dataGridView1.Rows[i].Cells[k + 1].Value = L_y_LT / 1; break;
                                case "L_z_LT": dataGridView1.Rows[i].Cells[k + 1].Value = L_z_LT / 1; break;
                                case "L_T": dataGridView1.Rows[i].Cells[k + 1].Value = L_T / 1; break;

                                // Cross-section data


                                case "Anet": dataGridView1.Rows[i].Cells[k + 1].Value = Anet / 1; break;

                                // Variables - Torsion NAD CZE

                                case "MEd.T": dataGridView1.Rows[i].Cells[k + 1].Value = MEd_T / 1000000; break;
                                case "VEd.T": dataGridView1.Rows[i].Cells[k + 1].Value = VEd_T / 1000; break;
                                case "e.T": dataGridView1.Rows[i].Cells[k + 1].Value = E_T / 1; break;



                                #endregion


                            }
                }
                catch (NullReferenceException) { }
            

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.naplnZoznamy();
            this.updateDataGridView();

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.naplnZoznamy();
            this.updateDataGridView();
        }
        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureForm p_form = new PictureForm(comboBox2.Text, typ_prierezu);
            p_form.ShowDialog();
        }
        // draw moment diagrams for Annexes and kc




        //definition






        //private void saveMenuStrip() 
        //{
        //    data_menu.Clear();

        //    foreach (ToolStripMenuItem item in dataToolStripMenuItem.DropDownItems) 
        //    {
        //        data_menu.Add(item.Checked);
        //    }
        //    FileStream fsOut = new FileStream("menu.dat", FileMode.Create);
        //    BinaryFormatter binF = new BinaryFormatter();
        //    binF.Serialize(fsOut, this.data_menu);
        //    fsOut.Close();
        //}
        //private void loadMenuStrip() 
        //{
        //    int i = 0;
        //    try
        //    {
        //        FileStream fsIn = new FileStream("menu.dat", FileMode.Open);

        //        BinaryFormatter binF2 = new BinaryFormatter();
        //        this.data_menu = (List<bool>)binF2.Deserialize(fsIn);
        //        fsIn.Close();

        //        foreach (ToolStripMenuItem item in dataToolStripMenuItem.DropDownItems)
        //        {
        //            item.Checked = data_menu[i++];
        //        }
        //    }
        //    catch (FileNotFoundException) { this.saveMenuStrip(); }

        //}
        private void checkOutputVariables(EN1993_1_1 en)
        {
            CheckedListInfo chLInfo = dtform.ZozChLInfos[typ_prierezu, 2]; //pre checkListbox 2.
            for (int i = 0; i < chLInfo.ZoznamPremennych.Count; i++)
            {
                if (chLInfo.ZoznamZasktPrem[i])
                {
                    switch (chLInfo.ZoznamPremennych[i])
                    {

                        #region Table cells for CENEX output data - EN1993_1_1

                        // Cross-section class
                        case "Class.w":
                            this.zoznamMenuNazvy.Add("Class.w");
                            this.zoznamMenuHodnoty.Add(en.Class_w.ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Class.f":
                            this.zoznamMenuNazvy.Add("Class.f");
                            this.zoznamMenuHodnoty.Add(en.Class_f.ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Class":
                            this.zoznamMenuNazvy.Add("Class");
                            this.zoznamMenuHodnoty.Add(en.Classall.ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;

                        // Class 4 cross-section data

                        case "Aeff":
                            this.zoznamMenuNazvy.Add("Aeff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Aeff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm2]");
                            break;
                        case "Ay.v.eff":
                            this.zoznamMenuNazvy.Add("Ay.v.eff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ay_v_eff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm2]");
                            break;
                        case "Az.v.eff":
                            this.zoznamMenuNazvy.Add("Az.v.eff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Az_v_eff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm2]");
                            break;
                        case "Wy.eff":
                            this.zoznamMenuNazvy.Add("Wy.eff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Wy_eff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm3]");
                            break;
                        case "Wz.eff":
                            this.zoznamMenuNazvy.Add("Wz.eff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Wz_eff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm3]");
                            break;
                        case "Wy.eff.min":
                            this.zoznamMenuNazvy.Add("Wy.eff.min");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Wy_eff_min, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm3]");
                            break;
                        case "Wz.eff.min":
                            this.zoznamMenuNazvy.Add("Wz.eff.min");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Wz_eff_min, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm3]");
                            break;
                        case "Iy.eff":
                            this.zoznamMenuNazvy.Add("Iy.eff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Iy_eff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm4]");
                            break;
                        case "Iz.eff":
                            this.zoznamMenuNazvy.Add("Iz.eff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Iz_eff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm4]");
                            break;
                        case "iy.eff":
                            this.zoznamMenuNazvy.Add("iy.eff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Iy_r_eff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm]");
                            break;
                        case "iz.eff":
                            this.zoznamMenuNazvy.Add("iz.eff");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Iz_r_eff, 2).ToString());
                            this.zoznamMenuJednotky.Add("[mm]");
                            break;



                        // Resistance


                        case "Npl.Rd":
                            this.zoznamMenuNazvy.Add("Npl.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Npl_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Nu.Rd":
                            this.zoznamMenuNazvy.Add("Nu.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Nu_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Nnet.Rd":
                            this.zoznamMenuNazvy.Add("Nnet.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Nnet_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Nt.Rd":
                            this.zoznamMenuNazvy.Add("Nt.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Nt_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Ny.cr":
                            this.zoznamMenuNazvy.Add("Ny.cr");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ny_cr / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Nz.cr":
                            this.zoznamMenuNazvy.Add("Nz.cr");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Nz_cr / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Nc.Rd":
                            this.zoznamMenuNazvy.Add("Nc.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Nc_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Ncr.TF":
                            this.zoznamMenuNazvy.Add("Ncr.TF");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.N_cr_TF / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Ncr.T":
                            this.zoznamMenuNazvy.Add("Ncr.T");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.N_cr_T / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Ny.b.Rd":
                            this.zoznamMenuNazvy.Add("Ny.b.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ny_b_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Nz.b.Rd":
                            this.zoznamMenuNazvy.Add("Nz.b.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Nz_b_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "NT.b.Rd":
                            this.zoznamMenuNazvy.Add("NT.b.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.NT_b_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Nb.Rd":
                            this.zoznamMenuNazvy.Add("Nb.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.N_b_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Vy.c.Rd":
                            this.zoznamMenuNazvy.Add("Vy.c.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Vy_c_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "Vz.c.Rd":
                            this.zoznamMenuNazvy.Add("Vz.c.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Vz_c_Rd / 1000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kN]");
                            break;
                        case "My.c.Rd":
                            this.zoznamMenuNazvy.Add("My.c.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.My_c_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "Mz.c.Rd":
                            this.zoznamMenuNazvy.Add("Mz.c.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Mz_c_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "My.el.Rd":
                            this.zoznamMenuNazvy.Add("My.el.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.My_el_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "Mz.el.Rd":
                            this.zoznamMenuNazvy.Add("Mz.el.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.My_c_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "My.pl.Rd":
                            this.zoznamMenuNazvy.Add("My.pl.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.My_pl_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "Mz.pl.Rd":
                            this.zoznamMenuNazvy.Add("Mz.pl.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Mz_pl_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "My.V.el.Rd":
                            this.zoznamMenuNazvy.Add("My.V.el.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.My_V_el_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "Mz.V.el.Rd":
                            this.zoznamMenuNazvy.Add("Mz.V.el.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Mz_V_el_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "My.V.pl.Rd":
                            this.zoznamMenuNazvy.Add("My.V.pl.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.My_V_pl_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "Mz.V.pl.Rd":
                            this.zoznamMenuNazvy.Add("Mz.V.pl.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Mz_V_pl_Rd / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "My.Rk":
                            this.zoznamMenuNazvy.Add("My.Rk");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.My_Rk / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;
                        case "Mz.Rk":
                            this.zoznamMenuNazvy.Add("Mz.Rk");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Mz_Rk / 1000000, 2).ToString());
                            this.zoznamMenuJednotky.Add("[kNm]");
                            break;

                        // Check ratios

                        case "NEd/Nt.Rd":
                            this.zoznamMenuNazvy.Add("NEd/Nt.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_65, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "NEd/Nc.Rd":
                            this.zoznamMenuNazvy.Add("NEd/Nc.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_69, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Vy.Ed/Vy.c.Rd":
                            this.zoznamMenuNazvy.Add("Vy.Ed/Vy.c.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_617y, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Vz.Ed/Vz.c.Rd":
                            this.zoznamMenuNazvy.Add("Vz.Ed/Vz.c.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_617z, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "My.Ed/My.c.Rd":
                            this.zoznamMenuNazvy.Add("My.Ed/My.c.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_612y, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Mz.Ed/Mz.c.Rd":
                            this.zoznamMenuNazvy.Add("Mz.Ed/Mz.c.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_612z, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "My.Ed/My.V.Rd":
                            this.zoznamMenuNazvy.Add("My.Ed/My.V.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_612y_MV, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Mz.Ed/Mz.V.Rd":
                            this.zoznamMenuNazvy.Add("Mz.Ed/Mz.V.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_612z_MV, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Tx.Ed/TRd":
                            this.zoznamMenuNazvy.Add("Tx.Ed/TRd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_623, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "σx.Ed/fyd":
                            this.zoznamMenuNazvy.Add("σx.Ed/fyd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_629_max_d, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "NEd/Nb.Rd":
                            this.zoznamMenuNazvy.Add("NEd/Nb.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_646, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "My.Ed/My.b.Rd":
                            this.zoznamMenuNazvy.Add("My.Ed/My.b.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_654y, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Mz.Ed/Mz.b.Rd":
                            this.zoznamMenuNazvy.Add("Mz.Ed/Mz.b.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_654z, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "NEd/Ny.b.Rd":
                            this.zoznamMenuNazvy.Add("NEd/Ny.b.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_661N, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "NEd/Nz.b.Rd":
                            this.zoznamMenuNazvy.Add("NEd/Nz.b.Rd");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_662N, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;




                        /* 
                        
                         case "My.Ed/My.b.Rd":
                             this.zoznamMenuNazvy.Add("My.Ed/My.b.Rd");
                             this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_661My, 2).ToString());
                             this.zoznamMenuJednotky.Add("[-]");
                             break;
                         case "Mz.Ed/Mz.Rd":
                             this.zoznamMenuNazvy.Add("Mz.Ed/Mz.Rd");
                             this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_661Mz, 2).ToString());
                             this.zoznamMenuJednotky.Add("[-]");
                             break;
                         */


                        case "Nc.kyyMy.kyzMz":
                            this.zoznamMenuNazvy.Add("Nc.kyyMy.kyzMz");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_661, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;
                        case "Nc.kzyMy.kzzMz":
                            this.zoznamMenuNazvy.Add("Nc.kzyMy.kzzMz");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_662, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;

                        case "Maximum ratio":
                            this.zoznamMenuNazvy.Add("Maximum ratio");
                            this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_maxtot, 2).ToString());
                            this.zoznamMenuJednotky.Add("[-]");
                            break;







                    }




                    //this.zoznamMenuNazvy.Add("N+My T");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_631y, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");

                    //this.zoznamMenuNazvy.Add("N+Mz T");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_631z, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");

                    //this.zoznamMenuNazvy.Add("σx Ed/fyd");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_642, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");

                    //this.zoznamMenuNazvy.Add("σx Ed/fyd");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_643, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");

                    //this.zoznamMenuNazvy.Add("NMyMz");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_644, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");


                    //this.zoznamMenuNazvy.Add("NEd/Nb.Rd");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_646, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");

                    //this.zoznamMenuNazvy.Add("My.Ed/My.b.Rd");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_654y, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");

                    //this.zoznamMenuNazvy.Add("Mz.Ed/Mz.b.Rd");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_654z, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");


                    //this.zoznamMenuNazvy.Add("N+My+Mz 1 C");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_661, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");

                    //this.zoznamMenuNazvy.Add("N+My+Mz 2 C");
                    //this.zoznamMenuHodnoty.Add(Math.Round(en.Ratio_662, 2).ToString());
                    //this.zoznamMenuJednotky.Add("[-]");

                        #endregion

                }
            }


        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Original
            this.zoznamMenuNazvy.Clear();
            this.zoznamMenuHodnoty.Clear();
            this.zoznamMenuJednotky.Clear();
            this.naplnZoznamy();



            /////////////////// LOADING OF MAIN CLASS ///////////////////

            //Main class object

            EN1993_1_1 en = new EN1993_1_1(comboBox1.Text, comboBox2.Text,item3_check,item4_check,item5_check,item6_check);



            //pridanie producera1
            // zoznamMenuNazvy.Add("producer");
            // zoznamMenuHodnoty.Add(en.Csproducer1);
            // zoznamMenuJednotky.Add("-");

            //test ci boli zadane vsetky hodnoty v polickach oznacenych zltou farbou
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    try
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value.Equals("_"))
                        {
                            MessageBox.Show("Enter all required input data. \n Fill all yellow cells with numerical values, please.");
                            return;
                        }
                    }
                    catch (NullReferenceException) { }
                }



            #region // Database loaded data checking
            if (item0_check == true)
            #region Steel attributes
            {

                MessageBox.Show
                    ((" Control of loaded variables " + "\n"

                + "Steel variables: " + "\n"
                + "\n"
                + " fy1 = " + en.Fy1 + "\n"
                + " fu1 = " + en.Fu1 + "\n"
                + " fy2 = " + en.Fy2 + "\n"
                + " fu2 = " + en.Fu2 + "\n"
                + "\n"
                + " used fy = " + en.Fy + "\n"
                + " used fu = " + en.Fu + "\n"
                + "\n"
                + " γM0 = " + en.GamaM0 + "\n"
                + " γM1 = " + en.GamaM1 + "\n"
                + " γM2 = " + en.GamaM2 + "\n"
                + "\n"
                + " βw = " + en.Betaw + "\n"
                + " η = " + en.Eta_shear + "\n"
                + "\n"
                + " E = " + en.E + "\n"
                + " G = " + en.G + "\n"
                + "\n"
                + " ν = " + en.Nu_pois + "\n"
                + " α = " + en.Alpha_temp + ""), "Material data");

            }
            #endregion
            if (item1_check == true)
            #region Cross-section attributes

            {

                MessageBox.Show
                    ((" Checking of loaded variables " + "\n"

                + " Cross-section variables: " + "\n"
                + "\n"
                + " Csprof_namenum " + en.Csprof_namenum + "\n"
                + " Csprof_name " + en.Csprof_name + "\n"
                + " Csprofshape " + en.Csprofshape + "\n"
                + " Csprof_production " + en.Csprof_production + "\n"
                + " Csprodcode1_name1 " + en.Csprodcode1_name1 + "\n"
                + " Csprodcode1_name2 " + en.Csprodcode1_name2 + "\n"
                + " Csprodcode2_name1 " + en.Csprodcode2_name1 + "\n"
                + " Csprodcode2_name2 " + en.Csprodcode2_name2 + "\n"
                + " Csproducer1 " + en.Csproducer1 + "\n"
                + " Csproducer2 " + en.Csproducer2 + "\n"
                + " Csprof_weight " + en.Csprof_weight + "\n"
                + "\n"
                + " Geometrical properties:" + "\n"
                + "\n"
                + " A = " + en.A + "\n"
                + " Asu = " + en.A_su + "\n"
                + " Af = " + en.Af + "\n"
                + " Aw = " + en.Aw + "\n"
                + " Av = " + en.Av + "\n"
                + " Ay.v = " + en.Ay_v + "\n"
                + " Az.v = " + en.Az_v + "\n"
                + "\n"
                + " Sy = " + en.Sy + "\n"
                + " Sz = " + en.Sz + "\n"
                + " Wy.el = " + en.Wy_el + "\n"
                + " Wz.el = " + en.Wz_el + "\n"
                + " Wy.pl = " + en.Wy_pl + "\n"
                + " Wz.pl = " + en.Wz_pl + "\n"
                + "\n"
                + " Iy = " + en.Iy + "\n"
                + " Iz = " + en.Iz + "\n"
                + " It = " + en.IT + "\n"
                + " Iw = " + en.Iw + "\n"
                + " Wt = " + en.Wt + "\n"
                + " iy = " + en.Iy_r + "\n"
                + " iz = " + en.Iz_r + "\n"
                + "\n"
                + " Cross-section data for class determination: " + "\n"
                + "\n"
                + " cy1 = " + en.Cy1 + "\n"
                + " ty1 = " + en.Ty1 + "\n"
                + " cz1 = " + en.Cz1 + "\n"
                + " tz1 = " + en.Tz1 + "\n"
                + "\n"
                + " cy2 = " + en.Cy2 + "\n"
                + " ty2 = " + en.Ty2 + "\n"
                + " cz2 = " + en.Cz2 + "\n"
                + " tz2 = " + en.Tz2 + " "
                ), "Cross-section data");
            }
            #endregion
            #endregion



            #region Variable property for User input data

            // Input - User

            en.N_Ed = this.N;
            en.Vy_Ed = this.Vy;
            en.Vz_Ed = this.Vz;
            en.Mx_Ed = this.Mx;
            en.My_Ed = this.My;
            en.Mz_Ed = this.Mz;


            en.My_a = this.My_a;
            en.My_b = this.My_b;
            en.Mz_a = this.Mz_a;
            en.Mz_b = this.Mz_b;

            en.My_s = this.My_s;
            en.Mz_s = this.Mz_s;


            en.My_Ed_x = this.My_Ed_x;
            en.Mz_Ed_x = this.Mz_Ed_x;

            en.Deflection_y_x = this.Deflection_y_x;
            en.Deflection_z_x = this.Deflection_z_x;

            en.C1y = this.C1y;
            en.C2y = this.C2y;
            en.C3y = this.C3y;

            en.Kz_LT = this.Kz_LT;
            en.Kw_LT = this.Kw_LT;

            en.L_teor = this.L_teor;
            en.L_y_buck = this.L_y_buck;
            en.L_z_buck = this.L_z_buck;
            en.L_y_LT = this.L_y_LT;
            en.L_z_LT = this.L_z_LT;
            en.L_T = this.L_T;



            en.Anet = this.Anet;

            en.MEd_T = this.MEd_T;
            en.VEd_T = this.VEd_T;
            en.E_T = this.E_T;




            #endregion




            /////////////////////////////////// USER INPUT DEFINITION /////////////////////////////////
            ///////////////////////////////////      COMBOBOXes       /////////////////////////////////

            // diagram for Kc

            // (comboBoxMomentDiag_kc.SelectedIndex)

            en.Kc_diagram = comboBoxMomentDiag_kc.SelectedIndex;
            //MessageBox.Show(en.Kc_diagram.ToString());

            // type of load index

            en.Loading_type_y = comboBoxLoad_y.SelectedIndex;
            en.Loading_type_z = comboBoxLoad_z.SelectedIndex;

            //MessageBox.Show(en.Loading_type_y + "  " + en.Loading_type_z);


            // Moment diagram in Method 1 - Annex A

            en.Moment_diagram_My1 = comboBoxMomentDiag_y.SelectedIndex;
            //en.Moment_diagram_Mz1 = ;

            // Moment diagram in Method 2 - Annex B

            en.Moment_diagram_My2 = comboBoxMomentDiag_y.SelectedIndex;
            //en.Moment_diagram_Mz2 = ;

            en.Torsion_koeficientNADCZ = comboBoxTorsionNADCZ.SelectedIndex;

            

            ////////////////////////////////     CALCULATION   ////////////////////////////
            //////////////////////////////// CLASS EN 1993-1-1 ////////////////////////////

            // Material properties definition
            en.EN_1993_1_1_Material();


            // Basic method: EN 1993-1-1 and EN 1993-1-5 - class of cross-section
            // Table 5.2
            en.EN_1993_1_1_55();

            #region // CS class determination and Class 4 data solution

            /*MessageBox.Show
                (" Checking of CS class determination and Class 4 data solution " + "\n"
            + "\n"
            + "Classes of cross section parts" + "\n"
            + "\n"
            + " Class.y1 = " + en.Class_y1 + "\n"
            + " Class.z1 = " + en.Class_z1 + "\n"
            + " Class.y2 = " + en.Class_y2 + "\n"
            + " Class.z2 = " + en.Class_z2 + "\n"
            + "\n"
            + "\n"
            + " Class.w = " + en.Class_w + "\n"
            + " Class.f = " + en.Class_f + "\n"
            + " Class = " + en.Classall + "\n"
            + "\n"
            + "\n"
            + "\n"

            + " Aeff = " + en.Aeff + "\n"
            );
             */

            #endregion

            // Basic method: Section 6.2 Resistance of cross-sections
            en.EN_1993_1_1_62();

            if (item2_check == true)
            #region Torsion input attributes
            {
             MessageBox.Show
                (("Auxiliary values EN 1993-1-1: " + "\n"
            + "\n"
            + "Section 6.2.7 Torsion (p. 53)"
            + "\n"
            + " αt = " + en.Alpha_T + "\n"
            + " βt = " + en.Beta_T + "\n"
            ), " NAD of Czech Republic - Table NB.2.1");
            }
            #endregion
            

            // Basic method: Section 6.3.1 Uniform members in compression (p. 56)
            en.EN_1993_1_1_631();

            // Basic method: Section 6.3.2 Uniform members in bending

            #region EN_1993_1_1_632
            switch (comboBoxChiLT.SelectedIndex)
            {
                case 0:
                    en.EN_1993_1_1_6322();
                    break;
                case 1:
                    en.Chi_y_LT = 1;
                    en.Chi_z_LT = 1;
                    en.My_b_Rd = en.My_c_Rd; /// !!!! Other moment resistance decreasing -rho,N, V combination !!!
                    en.Mz_b_Rd = en.Mz_c_Rd;
                    break;
                case 2:
                    en.EN_1993_1_1_6322();
                    break;
                case 3:
                    en.EN_1993_1_1_6323();
                    break;
                case 4:
                    en.EN_1993_1_1_6324();
                    break;

            }
            #endregion

            // Section 6.3.3 Uniform members in bending and axial compression - basic data Definition
            en.EN_1993_1_1_633();

            // Section 6.3.3 Uniform members in bending and axial compression - kij calculation -> Annex A or B
            #region EN_1993_1_633 - Annexes
            switch (comboBoxAnex.SelectedIndex)
            {
                case 0:
                    switch (comboBoxMomentDiag_y.SelectedIndex)
                    {
                        case 0:
                            en.AnnexB_mDiag1();
                            break;
                        case 1:
                            en.AnnexB_mDiag2();
                            break;
                        case 2:
                            en.AnnexB_mDiag3();
                            break;
                    }
                    en.AnnexB();

                    break;
                case 1:
                    en.Kyy = 1; // it should to be kij = 1 and solve ratio 661
                    en.Kyz = 1;
                    en.Kzy = 1; // it should to be kij = 1 and solve ratio 662
                    en.Kzz = 1;

                    break;
                case 2:
                    switch (comboBoxMomentDiag_y.SelectedIndex)
                    {
                        case 0:
                            en.AnnexA_mDiag1();
                            break;
                        case 1:
                            en.AnnexA_mDiag2();
                            break;
                        case 2:
                            en.AnnexA_mDiag3();
                            break;
                        case 3:
                            en.AnnexA_mDiag4();
                            break;
                    }
                    en.AnnexA();

                    break;
                case 3:
                    switch (comboBoxMomentDiag_y.SelectedIndex)
                    {
                        case 0:
                            en.AnnexB_mDiag1();
                            break;
                        case 1:
                            en.AnnexB_mDiag2();
                            break;
                        case 2:
                            en.AnnexB_mDiag3();
                            break;
                    }
                    en.AnnexB();

                    break;
                default: break;

            }

            #endregion

            // Final ratios calculation
            en.final_Check_Ratio();


            // Output textbox - text box Ratio max
            this.Ratio_max.Text = Math.Round(en.Ratio_maxtot, 2).ToString();

            //////////////////////////////////////////////////////////////////////////////
            this.checkOutputVariables(en);

           this.updateDataGridView();


        }
        //metoda,ktora sa stara, aby hodnota ktora je zadana do policka v datagridview sa ulozi do danej premmennej
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //hodnoty zadane do policok sa zapisu do premmenych + je kazda vynasobena podla jednotky
            try
            {
                switch (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString())
                {

                    case "N":
                        N = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        N = N * 1000; // Metoda pre N ma byt pred alebo po vypočte??
                        break;
                    case "Vy":
                        Vy = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Vy = Vy * 1000;
                        break;
                    case "Vz":
                        Vz = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Vz = Vz * 1000;
                        break;
                    case "Mx":
                        Mx = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Mx = Mx * 1000000;
                        break;
                    case "My":
                        My = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        My = My * 1000000;
                        break;
                    case "Mz":
                        Mz = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Mz = Mz * 1000000;
                        break;
                    case "My.a":
                        My_a = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        My_a = My_a * 1000000;
                        break;
                    case "My.b":
                        My_b = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        My_b = My_b * 1000000;
                        break;
                    case "Mz.a":
                        Mz_a = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Mz_a = Mz_a * 1000000;
                        break;
                    case "Mz.b":
                        Mz_b = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Mz_b = Mz_b * 1000000;
                        break;
                    case "My.s":
                        My_s = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        My_s = My_s * 1000000;
                        break;
                    case "Mz.s":
                        Mz_s = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Mz_s = Mz_s * 1000000;
                        break;
                    case "My.Ed.x":
                        My_Ed_x = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        My_Ed_x = My_Ed_x * 1000000;
                        break;
                    case "Mz.Ed.x":
                        Mz_Ed_x = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Mz_Ed_x = Mz_Ed_x * 1000000;
                        break;

                    case "Deflection.y.x":
                        Deflection_y_x = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Deflection_y_x = Deflection_y_x * 1;
                        break;
                    case "Deflection.z.x":
                        Deflection_z_x = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Deflection_z_x = Deflection_z_x * 1;
                        break;

                    case "C1y":
                        C1y = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        C1y = C1y * 1;
                        break;
                    case "C2y":
                        C2y = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        C2y = C2y * 1;
                        break;
                    case "C3y":
                        C3y = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        C3y = C3y * 1;
                        break;
                    case "kz_LT":
                        Kz_LT = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Kz_LT = Kz_LT * 1;
                        break;
                    case "kw_LT":
                        Kw_LT = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Kw_LT = Kw_LT * 1;
                        break;

                    case "L_teor":
                        L_teor = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        L_teor = L_teor * 1;
                        break;
                    case "L_y_buck":
                        L_y_buck = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        L_y_buck = L_y_buck * 1;
                        break;
                    case "L_z_buck":
                        L_z_buck = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        L_z_buck = L_z_buck * 1;
                        break;
                    case "L_y_LT":
                        L_y_LT = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        L_y_LT = L_y_LT * 1;
                        break;
                    case "L_z_LT":
                        L_z_LT = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        L_z_LT = L_z_LT * 1;
                        break;
                    case "L_T":
                        L_T = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        L_T = L_T * 1;
                        break;


                    case "Anet":
                        Anet = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        Anet = Anet * 1;
                        break;

                    case "MEd.T":
                        MEd_T = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        MEd_T = MEd_T * 1000000;
                        break;

                    case "VEd.T":
                        VEd_T = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        VEd_T = VEd_T * 1000;
                        break;
                    case "e.T":
                        E_T = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        E_T = E_T * 1;
                        break;



                   // After cell input and pressing ENTER
                    // default:
                    // MessageBox.Show("Chyba v metode CellEndEdit.\nZadali ste nespravnu hodnotu!");
                    // break;
                }
            }
            catch (FormatException) { MessageBox.Show("Wrong input format for numerical value! \n Enter real number. \n For example: 99,9"); }
            catch (ArgumentOutOfRangeException) { }

        }

        


        
        
        ///SPECIMEN
        ///// EXPORT TO MS EXCEL FILE .xls
        #region EXPORT


        /// METHOD 0

        #region 0
        /*
            datagridview1.SelectAll();
            Clipboard.SetDataObject(datagridview1.GetClipboardContent(), true);
            StreamWriter sw = new StreamWriter("d:\\EN1993Form_test.xls");
            sw.Write(Clipboard.GetText());
            sw.Flush();
            sw.Close();
             */
        #endregion


        // METHOD 1

        #region 1
        ///
        /// http://www.codeproject.com/KB/dotnet/ExportToExcel.aspx?df=100&forumid=146533&exp=0&select=1357703
        /// 


        public void exportToExcel(DataSet source, string fileName)
        {

            System.IO.StreamWriter excelDoc;

            excelDoc = new System.IO.StreamWriter(fileName);
            const string startExcelXML = "<xml version>\r\n<Workbook " +
                  "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                  "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                  "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                  "office:spreadsheet\">\r\n <Styles>\r\n " +
                  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                  "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                  "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                  "\r\n <Protection/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                  "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                  "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                  " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                  "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                  "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                  "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                  "</Styles>\r\n ";
            const string endExcelXML = "</Workbook>";

            int rowCount = 0;
            int sheetCount = 1;
            /*
           <xml version>
           <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
           xmlns:o="urn:schemas-microsoft-com:office:office"
           xmlns:x="urn:schemas-microsoft-com:office:excel"
           xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
           <Styles>
           <Style ss:ID="Default" ss:Name="Normal">
             <Alignment ss:Vertical="Bottom"/>
             <Borders/>
             <Font/>
             <Interior/>
             <NumberFormat/>
             <Protection/>
           </Style>
           <Style ss:ID="BoldColumn">
             <Font x:Family="Swiss" ss:Bold="1"/>
           </Style>
           <Style ss:ID="StringLiteral">
             <NumberFormat ss:Format="@"/>
           </Style>
           <Style ss:ID="Decimal">
             <NumberFormat ss:Format="0.0000"/>
           </Style>
           <Style ss:ID="Integer">
             <NumberFormat ss:Format="0"/>
           </Style>
           <Style ss:ID="DateLiteral">
             <NumberFormat ss:Format="mm/dd/yyyy;@"/>
           </Style>
           </Styles>
           <Worksheet ss:Name="Sheet1">
           </Worksheet>
           </Workbook>
           */
            excelDoc.Write(startExcelXML);
            excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
            excelDoc.Write("<Table>");
            excelDoc.Write("<Row>");
            for (int x = 0; x < source.Tables[0].Columns.Count; x++)
            {
                excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                excelDoc.Write(source.Tables[0].Columns[x].ColumnName);
                excelDoc.Write("</Data></Cell>");
            }
            excelDoc.Write("</Row>");
            foreach (DataRow x in source.Tables[0].Rows)
            {
                rowCount++;
                //if the number of rows is > 64000 create a new page to continue output

                if (rowCount == 64000)
                {
                    rowCount = 0;
                    sheetCount++;
                    excelDoc.Write("</Table>");
                    excelDoc.Write(" </Worksheet>");
                    excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                    excelDoc.Write("<Table>");
                }
                excelDoc.Write("<Row>"); //ID=" + rowCount + "

                for (int y = 0; y < source.Tables[0].Columns.Count; y++)
                {
                    System.Type rowType;
                    rowType = x[y].GetType();
                    switch (rowType.ToString())
                    {
                        case "System.String":
                            string XMLstring = x[y].ToString();
                            XMLstring = XMLstring.Trim();
                            XMLstring = XMLstring.Replace("&", "&");
                            XMLstring = XMLstring.Replace(">", ">");
                            XMLstring = XMLstring.Replace("<", "<");
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                           "<Data ss:Type=\"String\">");
                            excelDoc.Write(XMLstring);
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.DateTime":
                            //Excel has a specific Date Format of YYYY-MM-DD followed by  

                            //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000

                            //The Following Code puts the date stored in XMLDate 

                            //to the format above

                            DateTime XMLDate = (DateTime)x[y];
                            string XMLDatetoString = ""; //Excel Converted Date

                            XMLDatetoString = XMLDate.Year.ToString() +
                                 "-" +
                                 (XMLDate.Month < 10 ? "0" +
                                 XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                                 "-" +
                                 (XMLDate.Day < 10 ? "0" +
                                 XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                                 "T" +
                                 (XMLDate.Hour < 10 ? "0" +
                                 XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                                 ":" +
                                 (XMLDate.Minute < 10 ? "0" +
                                 XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                                 ":" +
                                 (XMLDate.Second < 10 ? "0" +
                                 XMLDate.Second.ToString() : XMLDate.Second.ToString()) +
                                 ".000";
                            excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                                         "<Data ss:Type=\"DateTime\">");
                            excelDoc.Write(XMLDatetoString);
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Boolean":
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                        "<Data ss:Type=\"String\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                                    "<Data ss:Type=\"Number\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                                  "<Data ss:Type=\"Number\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.DBNull":
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                  "<Data ss:Type=\"String\">");
                            excelDoc.Write("");
                            excelDoc.Write("</Data></Cell>");
                            break;
                        default:
                            throw (new Exception(rowType.ToString() + " not handled."));
                    }
                }
                excelDoc.Write("</Row>");
            }
            excelDoc.Write("</Table>");
            excelDoc.Write(" </Worksheet>");
            excelDoc.Write(endExcelXML);
            excelDoc.Close();
        }

        #endregion

        // METHOD 2

        #region 2
        /*
        /// <summary>
        /// Exports a passed datagridview to an Excel worksheet.
        /// If captions is true, grid headers will appear in row 1.
        /// Data will start in row 2.
        /// </summary>
        /// <param name="datagridview"></param>
        /// <param name="captions"></param>
        private void Export2Excel(DataGridView datagridview, bool captions)
        {
            object objApp_Late;
            object objBook_Late;
            object objBooks_Late;
            object objSheets_Late;
            object objSheet_Late;
            object objRange_Late;
            object[] Parameters;
            string[] headers = new string[datagridview.ColumnCount - 1];
            string[] columns = new string[datagridview.ColumnCount - 1];

            int i = 0;
            int c = 0;
            for (c = 0; c < datagridview.ColumnCount - 1; c++)
            {
                headers[c] = datagridview.Rows[0].Cells[c].OwningColumn.Name.ToString();
                i = c + 65;
                columns[c] = Convert.ToString((char)i);
            }

            try
            {
                // Get the class type and instantiate Excel.
                Type objClassType;
                objClassType = Type.GetTypeFromProgID("Excel.Application");
                objApp_Late = Activator.CreateInstance(objClassType);
                //Get the workbooks collection.
                objBooks_Late = objApp_Late.GetType().InvokeMember("Workbooks",
                BindingFlags.GetProperty, null, objApp_Late, null);
                //Add a new workbook.
                objBook_Late = objBooks_Late.GetType().InvokeMember("Add",
                BindingFlags.InvokeMethod, null, objBooks_Late, null);
                //Get the worksheets collection.
                objSheets_Late = objBook_Late.GetType().InvokeMember("Worksheets",
                BindingFlags.GetProperty, null, objBook_Late, null);
                //Get the first worksheet.
                Parameters = new Object[1];
                Parameters[0] = 1;
                objSheet_Late = objSheets_Late.GetType().InvokeMember("Item",
                BindingFlags.GetProperty, null, objSheets_Late, Parameters);

                if (captions)
                {
                    // Create the headers in the first row of the sheet
                    for (c = 0; c < datagridview.ColumnCount - 1; c++)
                    {
                        //Get a range object that contains cell.
                        Parameters = new Object[2];
                        Parameters[0] = columns[c] + "1";
                        Parameters[1] = Missing.Value;
                        objRange_Late = objSheet_Late.GetType().InvokeMember("Range",
                        BindingFlags.GetProperty, null, objSheet_Late, Parameters);
                        //Write Headers in cell.
                        Parameters = new Object[1];
                        Parameters[0] = headers[c];
                        objRange_Late.GetType().InvokeMember("Value", BindingFlags.SetProperty,
                        null, objRange_Late, Parameters);
                    }
                }

                // Now add the data from the grid to the sheet starting in row 2
                for (i = 0; i < datagridview.RowCount; i++)
                {
                    for (c = 0; c < datagridview.ColumnCount - 1; c++)
                    {
                        //Get a range object that contains cell.
                        Parameters = new Object[2];
                        Parameters[0] = columns[c] + Convert.ToString(i + 2);
                        Parameters[1] = Missing.Value;
                        objRange_Late = objSheet_Late.GetType().InvokeMember("Range",
                        BindingFlags.GetProperty, null, objSheet_Late, Parameters);
                        //Write Headers in cell.
                        Parameters = new Object[1];
                        Parameters[0] = datagridview.Rows[i].Cells[headers[c]].Value.ToString();
                        objRange_Late.GetType().InvokeMember("Value", BindingFlags.SetProperty,
                        null, objRange_Late, Parameters);
                    }
                }

                //Return control of Excel to the user.
                Parameters = new Object[1];
                Parameters[0] = true;
                objApp_Late.GetType().InvokeMember("Visible", BindingFlags.SetProperty,
                null, objApp_Late, Parameters);
                objApp_Late.GetType().InvokeMember("UserControl", BindingFlags.SetProperty,
                null, objApp_Late, Parameters);
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);

                MessageBox.Show(errorMessage, "Error");
            }
        }
         
         
         */

        #endregion



        public void WriteToExcelSpreadsheet(string fileName, System.Data.DataTable dt)
        {

            //string filepath = getPath(fileName).Trim();
            string filepath = fileName;
            //dt = SQLProductProvider.GetExcelImport();

            // dt.WriteXml(filepath, XmlWriteMode.IgnoreSchema);

            Microsoft.Office.Interop.Excel.Application ExlApp = new Microsoft.Office.Interop.Excel.Application();

            int iCol, iRow, iColVal;

            Object missing = System.Reflection.Missing.Value;

            // Open the document that was chosen by the dialog

            Microsoft.Office.Interop.Excel.Workbook aBook;

            try
            {

                //'re-initialize excel app

                ExlApp = new Microsoft.Office.Interop.Excel.Application();

                if (ExlApp == null)
                {

                    //'throw an exception

                    throw (new Exception("Unable to Start Microsoft Excel"));

                }

                else
                {

                    //'supresses overwrite warnings

                    ExlApp.DisplayAlerts = false;

                    //aBook = New Excel.Workbook

                    //'check if file exists

                    if (File.Exists(filepath))
                    {

                        aBook = ExlApp.Workbooks._Open(filepath, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);

                    }

                    else
                    {

                        aBook = ExlApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

                    }//End If

                    //With ExlApp

                    ExlApp.SheetsInNewWorkbook = 1;

                    //ExlApp.Worksheets[1].Select();

                    //For displaying the column name in the the excel file.

                    for (iCol = 0; iCol < dt.Columns.Count; iCol++)
                    {

                        //'clear column name before setting a new value

                        ExlApp.Cells[1, iCol + 1] = "";

                        ExlApp.Cells[1, iCol + 1] = dt.Columns[iCol].ColumnName.ToString();

                    }//next

                    //For displaying the column value row-by-row in the the excel file.

                    for (iRow = 0; iRow < dt.Rows.Count; iRow++)
                    {

                        try
                        {

                            for (iColVal = 0; iColVal < dt.Columns.Count; iColVal++)
                            {

                                if (dt.Rows[iRow].ItemArray[iColVal] is string)
                                {

                                    ExlApp.Cells[iRow + 2, iColVal + 1] = "'" + dt.Rows[iRow].ItemArray[iColVal].ToString();

                                }

                                else
                                {

                                    ExlApp.Cells[iRow + 2, iColVal + 1] = dt.Rows[iRow].ItemArray[iColVal].ToString();

                                }//End If

                            }//next

                        }

                        catch (Exception ex)
                        {

                            Console.Write("ERROR: " + ex.Message);

                        }//End Try

                    }//next

                    if (File.Exists(filepath))
                    {

                        ExlApp.ActiveWorkbook.Save(); //fileName)

                    }

                    else
                    {

                        ExlApp.ActiveWorkbook.SaveAs(filepath.Trim(), missing, missing, missing, missing, missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);

                    }//End If

                    ExlApp.ActiveWorkbook.Close(true, missing, missing);

                    //End With

                    //Console.Write("File exported sucessfully");

                }//End if

            }

            catch (System.Runtime.InteropServices.COMException ex)
            {



                Console.Write("ERROR: " + ex.Message);

            }

            catch (Exception ex)
            {



                Console.Write("ERROR: " + ex.Message);

            }

            finally
            {

                ExlApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(ExlApp);

                aBook = null;

                ExlApp = null;

            }//End Try

        }//End Sub

        #endregion
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        private void comboBoxAnex_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxMomentDiag_y.Items.Clear();
            switch (comboBoxAnex.SelectedIndex)
            {
                case 0:
                    comboBoxMomentDiag_y.Items.Add("picture 1");
                    comboBoxMomentDiag_y.Items.Add("picture 2");
                    comboBoxMomentDiag_y.Items.Add("picture 3");
                    break;
                case 1:
                    break;
                case 2:
                    comboBoxMomentDiag_y.Items.Add("picture 1");
                    comboBoxMomentDiag_y.Items.Add("picture 2");
                    comboBoxMomentDiag_y.Items.Add("picture 3");
                    comboBoxMomentDiag_y.Items.Add("picture 4");
                    break;
                case 3:
                    comboBoxMomentDiag_y.Items.Add("picture 1");
                    comboBoxMomentDiag_y.Items.Add("picture 2");
                    comboBoxMomentDiag_y.Items.Add("picture 3");
                    break;
                default: break;
            }
        }
        private void TableDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtform = new DataTableForm(typ_prierezu);
            if (dtform.ShowDialog() == DialogResult.Cancel)
            {
                naplnZoznamy();
                updateDataGridView();
            }
        }
        private void PictMD_Annex_My_TSMI_Click(object sender, EventArgs e)
        {
            Annex i = new Annex(comboBoxAnex.SelectedIndex);
            i.ShowDialog();
        }
        private void InfoSteelForm_Load(object sender, EventArgs e)
        {

        }
        private void PictMD_Kc_TSMI_Click_1(object sender, EventArgs e)
        {
            Kc i = new Kc(comboBoxMomentDiag_kc.SelectedIndex);
            i.ShowDialog();
        }
        private void messageBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EN1993_1_1MessageBoxes i = new EN1993_1_1MessageBoxes ();
            if (i.ShowDialog() == DialogResult.OK) 
            {
                item0_check = i.SelectedItems[0];
                item1_check = i.SelectedItems[1];
                item2_check = i.SelectedItems[2];
                item3_check = i.SelectedItems[3];
                item4_check = i.SelectedItems[4];
                item5_check = i.SelectedItems[5];
                item6_check = i.SelectedItems[6];
                
            }

        }
        private void export_Click(object sender, EventArgs e)
        {
            SaveFileDialog s_dialog = new SaveFileDialog();
            s_dialog.DefaultExt = ".xls";
            s_dialog.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
            s_dialog.ShowDialog();
            
            //slow method
            //this.WriteToExcelSpreadsheet(s_dialog.FileName,ds.Tables[0]);

            //my own class and methods for faster export
            ExportToExcel export = new ExportToExcel(s_dialog.FileName,ds);            
            export.writeToExcel();


            // EXPORT TO MS EXCEL METHOD

        }
        private void print_pdf_Click(object sender, EventArgs e)
        {

            // PDF PRINT METHOD

        }
        



        //export do excelu
        private void toExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog s_dialog = new SaveFileDialog();
            s_dialog.DefaultExt = ".xls";
            s_dialog.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
            s_dialog.ShowDialog();
            try
            {
                this.exportToExcel(this.ds, s_dialog.FileName);
            }
            catch (IOException) { MessageBox.Show("File is probably used by another process."); }
            catch (ArgumentException) {}
            catch (Exception) { MessageBox.Show("Unexpected error."); }
        }
        //export do wordu
        private void toWordToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //export do txt
        private void toTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            EN1999_1_1Annex_I_Form i = new EN1999_1_1Annex_I_Form();
            i.ShowDialog();
        }


    }
}
