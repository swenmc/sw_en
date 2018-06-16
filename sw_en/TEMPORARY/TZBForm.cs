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
using CENEX.TEMPORARY;

namespace CENEX
{
    public partial class TZBForm : Form
    {
        private DataSet1 ds;

        
        int num;
        int num_rowindex;
        int num_rowindex1;
        // Konstruktor
        public TZBForm()
        {
            InitializeComponent();
            ds = new DataSet1();
            dataGridView1.DataSource = ds.Tables["DataTable1"];

            num = ComboBox1.SelectedIndex;
            

        }
       
        


        // DRAW PICTURE


        public void picture_draw()
        {
            num = ComboBox1.SelectedIndex;
            

            switch (num)
            {
                case 0:
                    Image image = Image.FromFile(@"tzb_tvary\1.jpg");
                    Bitmap myBitmap = new Bitmap(image, pictureBox1.Width, pictureBox1.Height);
                    Graphics g = Graphics.FromImage(myBitmap);

                    pictureBox1.Image = myBitmap;
                    break;
                case 1:
                    image = Image.FromFile(@"tzb_tvary\\2.jpg");
                    myBitmap = new Bitmap(image, pictureBox1.Width, pictureBox1.Height);
                    g = Graphics.FromImage(myBitmap);

                    pictureBox1.Image = myBitmap;
                    break;
                case 2:
                    image = Image.FromFile(@"tzb_tvary\\3.jpg");
                    myBitmap = new Bitmap(image, pictureBox1.Width, pictureBox1.Height);
                    g = Graphics.FromImage(myBitmap);

                    pictureBox1.Image = myBitmap;

                    break;
                case 3:
                    image = Image.FromFile(@"tzb_tvary\\4.jpg");
                    myBitmap = new Bitmap(image, pictureBox1.Width, pictureBox1.Height);
                    g = Graphics.FromImage(myBitmap);

                    pictureBox1.Image = myBitmap;

                    break;

                default: MessageBox.Show("Neimplementovane!"); break;


            }
        }
        
        public void TZBFormN(int shape_pol)
        {
            // Nova polozka - premenne
           
            
            try
            {
            double _A = Convert.ToDouble(ComboBox_A.Text);
            }
            catch (FormatException) { }
            try
            {
                double _B = Convert.ToDouble(ComboBox_B.Text);
            }
            catch (FormatException) { }
            try
            {
                double _C = Convert.ToDouble(ComboBox_C.Text);
            }
            catch (FormatException) { }
            try
            {
                double _D = Convert.ToDouble(ComboBox_D.Text);
            }
            catch (FormatException) { }
            try
            {
                double _E = Convert.ToDouble(ComboBox_E.Text);
            }
            catch (FormatException) { }
            try
            {
                double _F = Convert.ToDouble(ComboBox_F.Text.ToString());
            }
            catch (FormatException) { }
            try
            {
                double _G = Convert.ToDouble(ComboBox_G.Text.ToString());
            }
            catch (FormatException) { }
            try
            {
                double _R = Convert.ToDouble(ComboBox_R.Text.ToString());
            }
            catch (FormatException) { }
            try
            {
                double _L = Convert.ToDouble(ComboBox_L.Text.ToString());
            }
            catch (FormatException) { }
            try
            {
                double _Uhol = Convert.ToDouble(ComboBox_U.Text.ToString());
            }
            catch (FormatException) { }
            // Values control
            /*
            MessageBox.Show
                    (
            " Index = " + shape_pol.ToString() + " - " + "\n"
            + " A = " + ComboBox_A.Text + " [mm]\n"
            + " B = " + ComboBox_B.Text + " [mm]\n"
            + " C = " + ComboBox_C.Text + " [mm]\n"
            + " D = " + ComboBox_D.Text + " [mm]\n"
            + " E = " + ComboBox_E.Text + " [mm]\n"
            + " F = " + ComboBox_F.Text + " [mm]\n"
            + " G = " + ComboBox_G.Text + " [mm]\n"

            + " R = " + ComboBox_R.Text + " [mm]\n"
            + " L = " + ComboBox_L.Text + " [mm]\n"
            + " α = " + ComboBox_U.Text + " [°] \n"
            );
             */


            #region Change Format  test
            // TEXT convert text to number
            /*
            string a = ComboBox_A.Text;

            double aDouble = Convert.ToDouble(a);
            decimal aDecimal = Convert.ToDecimal(a);
            byte aByte = Convert.ToByte(a);
            //char aChar = Convert.ToChar(a);
            int aInt16 = Convert.ToInt16(a);
            int aInt32 = Convert.ToInt32(a);
            long aInt64 = Convert.ToInt64(a);

            MessageBox.Show
                    (
            " Index = " + shape_pol.ToString() + " - " + "\n"
            + " aDouble = " + Convert.ToString (aDouble) + " [mm]\n"
            + " aDecimal = " + Convert.ToString(aDecimal) + " [mm]\n"
            + " aByte = " + Convert.ToString(aByte) + " [mm]\n"
            //+ " aChar = " + Convert.ToString(aChar) + " [mm]\n"
            + " aInt16 = " + Convert.ToString(aInt16) + " [mm]\n"
            + " aInt32 = " + Convert.ToString(aInt32) + " [mm]\n"
            + " aInt64 (long) = " + Convert.ToString(aInt64) + " [mm]"
                      
            );
             */
            #endregion


        }

        public void TZBFormA(int row_pol)
        {
            // Aktualna polozka v zozname - premenne

            //this.row_pol = row_pol;




        }





        // Nova polozka
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // vy/prekreslenie obrazku
            this.picture_draw();
            // vymazanie vlastnosti
            richTextBoxNew.Clear();
            

            //Kontrola indexov
            MessageBox.Show
                    (
            " Index num = " + num.ToString());
            
            // vy/prepisanie vlastnosti v Textbox
            switch (num)
                    
            {
                case 0:
                    richTextBoxNew.Text =

             " A = " + ComboBox_A.Text + " [mm]\n"
            + " B = " + ComboBox_B.Text + " [mm]\n"    
            + " L = " + ComboBox_L.Text + " [mm]";
            
                    
                    break;
                case 1:
                    richTextBoxNew.Text =
            " A = " + ComboBox_A.Text + " [mm]\n"
            + " B = " + ComboBox_B.Text + " [mm]\n"
          
            + " R = " + ComboBox_R.Text + " [mm]\n"
            + " L = " + ComboBox_L.Text + " [mm]\n"
            + " α = " + ComboBox_U.Text + " [°]";
                    
                    break;
                case 2:

                    richTextBoxNew.Text =
            " A = " + ComboBox_A.Text + " [mm]\n"
            + " B = " + ComboBox_B.Text + " [mm]\n"
            + " C = " + ComboBox_C.Text + " [mm]\n"
            + " D = " + ComboBox_D.Text + " [mm]\n"
            + " E = " + ComboBox_E.Text + " [mm]\n"
            + " F = " + ComboBox_F.Text + " [mm]\n"
            + " G = " + ComboBox_G.Text + " [mm]\n"

            + " R = " + ComboBox_R.Text + " [mm]\n"
            + " L = " + ComboBox_L.Text + " [mm]\n"
            + " α = " + ComboBox_U.Text + " [°]";
                               
                    break;
                case 3:
                    richTextBoxNew.Text =
            " A = " + ComboBox_A.Text + " [mm]\n"
            + " B = " + ComboBox_B.Text + " [mm]";
                    
                    break;
                default: break;
                 


                
            }
            //Zapisanie vlastností

            this.TZBFormN(num);
            this.TZBFormA(0); // index riadku v tabulke !!! doplnit
            
        }







        #region RICHTEXTBOX_HELP
        // HELP

        /*

                            to select a line you can do something like this:

        Public Sub SelectLine(line as Integer)
            Dim start as Integer = richtextbox1.GetFirstCharIndexFromLine(line)
            Dim end as Integer = richtextbox1..GetFirstCharIndexFromLine(line+1)

            If start<=0 Then
                start = 0
                end = 0
            End If
            If end<= 0 Then
                end = richtextbox1.Text.Length
            End If
            richtextbox1.Select(start, end-start)        
        End Sub
         * 
         * 
         to set the text unformatted:
             richtextbox1.Text = "Line one" & chr(13) & "Second line" & chr(13) & "The last line"

        then call my previous procedure:
            SelectLine(0)
        and format the line
            richtextbox1.SelectionFont=New Font("Arial", 12, FontStyle.Regular)

        similar with other lines:
            SelectLine(1)
            richtextbox1.SelectionFont=New Font("Arial", 12, FontStyle.Bold)

            SelectLine(2)
            richtextbox1.SelectionFont=New Font("Arial", 12, FontStyle.Italic)
         * #
        /*
        #
        * A function to delete a set amount of text from a RichTextBox
        #
        */
        /*
       #
 
       #
       bool DeleteText (System.Windows.Forms.RichTextBox rtb, int start, int length)
       #
       { // pass the rich text box, the position to highlight from, and the length to highlight
       #
           int currentCaret = rtb.SelectionStart; // use to reset position
       #
           bool success = false; // the boolean to return
       #
           rtb.Select (start, length); // select the text
       #
           rtb.SelectedText = String.Empty; // set it to nothing
       #
           if (rtb.SelectedText != String.Empty) // if for any reason it could not be changed
       #
               success = false;
       #
           else success = true;
       #
           rtb.SelectionStart = currentCaret; // move the caret back to the current position
       #
           return success;
           */
        /*
        C#

        private void WriteIndentedTextToRichTextBox()
        {
            // Clear all text from the RichTextBox;
            richTextBox1.Clear();
            // Specify a 20 pixel right indent in all paragraphs.
              richTextBox1.SelectionRightIndent = 20;
            // Set the font for the text.
            richTextBox1.Font = new Font("Lucinda Console", 12);
            // Set the text within the control.
            richTextBox1.SelectedText = "All text is indented 20 pixels from the right edge of the RichTextBox.";
            richTextBox1.SelectedText = "You can use this property with the SelectionIndent property to provide right and left margins.";
            richTextBox1.SelectedText = "After this paragraph the indentation will end.\n\n";
            // Remove all right indentation.
            richTextBox1.SelectionRightIndent = 0;
            richTextBox1.SelectedText = "This paragraph has no right indentation. All text should flow as normal.";
        }

        */


        #endregion







        public void button1_Click(object sender, EventArgs e)
        {
            ////vytvorime novy riadok datagridu 
            //DataGridViewRow row = new DataGridViewRow();
            ////metoda CreateCells ma prvy argument objekt datagridu a dalsimy argumentami je pole objektov ktore sa nachadzaju
            ////v bunkach v danom riadku
            //row.CreateCells(dataGridView1,ComboBox_A.Text,ComboBox_B.Text,ComboBox_C.Text,ComboBox_D.Text,
            //    ComboBox_E.Text,ComboBox_F.Text);
            ////metodou add pridame riadok do datagridview
            //dataGridView1.Rows.Add(row);
            DataTable table = ds.Tables["DataTable1"];

            DataRow row = table.NewRow();    //new row in table

            row[0] = ComboBox_A.Text;
            row[1] = ComboBox_B.Text;
            row[2] = ComboBox_C.Text;
            row[3] = ComboBox_D.Text;
            row[4] = ComboBox_E.Text;
            row[5] = ComboBox_F.Text;
            //row[6] = ComboBox_G.Text;
            //row[7] = ComboBox_R.Text;
            //row[8] = ComboBox_L.Text;
            //row[9] = ComboBox_U.Text;

            table.Rows.Add(row);   //adding of new row with values to the table

            // Selected row index in data grid view
            num_rowindex = dataGridView1.CurrentRow.Index;
            num_rowindex1 = this.dataGridView1.CurrentCellAddress.Y;
            
            

        }

        // Aktualna polozka v zozname - vykreslenie a vypis vlastností

             

    
        public void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
  
{
           
 // vymazanie vlastnosti
            richTextBoxAct.Clear();

            MessageBox.Show
                    (
            " Index num_rowindex = " + num_rowindex.ToString() + " - " + "\n"
            + " Index num_rowindex1 = " + num_rowindex1.ToString() + " - " + "\n");

        
// podmienka ze musi byt zvoleny len jeden riadok
           if (dataGridView1.SelectedRows.Count > 0)
               {
               MessageBox.Show
                    ("Vyberte len jeden riadok alebo jednu bunku zoznamu");
           }
// proiradenie hodnot typu string premennym _XText z prislusnych stlpcov tabulky datagrid pre aktualne vybrany riadok

           // int ID = Convert.ToInt32(dataGridView1.Rows[num_rowindex].Cells[0].Value);

           // asi ma byt od [0]
           string _AText = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[1].Value);
           string _BText = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[2].Value);
           string _CText = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[3].Value);
           string _DText = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[4].Value);
           string _EText = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[5].Value);

           string _FText = "a";// = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[6].Value);
           string _GText = "a";// = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[7].Value);
           string _RText = "a";// = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[8].Value);
           string _LText = "a";// = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[9].Value);
           string _UText = "a";// = Convert.ToString(dataGridView1.Rows[num_rowindex].Cells[10].Value);

            
            // vy/prepisanie vlastnosti v Textbox

            switch (num) // !!!!!   original type of element - index - este nie je zahranutý v tabulke
                    
            {
                case 0:
                    richTextBoxAct.Text =

             " A = " + _AText + " [mm]\n"
            + " B = " + _BText + " [mm]\n"    
            + " L = " + _LText + " [mm]";
            
                    
                    break;
                case 1:
                    richTextBoxAct.Text =
            " A = " + _AText + " [mm]\n"
            + " B = " + _BText + " [mm]\n"
          
            + " R = " + _RText + " [mm]\n"
            + " L = " + _LText + " [mm]\n"
            + " α = " + _UText + " [°]";
                    
                    break;
                case 2:

                    richTextBoxAct.Text =
            " A = " + _AText + " [mm]\n"
            + " B = " + _BText + " [mm]\n"
            + " C = " + _CText + " [mm]\n"
            + " D = " + _DText + " [mm]\n"
            + " E = " + _EText + " [mm]\n"
            + " F = " + _FText + " [mm]\n"
            + " G = " + _GText + " [mm]\n"

            + " R = " + _RText + " [mm]\n"
            + " L = " + _LText + " [mm]\n"
            + " α = " + _UText + " [°]";
                               
                    break;
                case 3:
                    richTextBoxAct.Text =
            " A = " + _AText + " [mm]\n"
            + " B = " + _BText + " [mm]";
                    
                    break;
                default: break;
              
            }


        }

        


        private void SaveData(string fileName)
        {
            FileStream fsOut = new FileStream(fileName, FileMode.Create);
            BinaryFormatter binF = new BinaryFormatter();
            binF.Serialize(fsOut, ds);
            fsOut.Close();
        }
        private void LoadData(string fileName) 
        {
            FileStream fsIn = new FileStream(fileName, FileMode.Open);
            BinaryFormatter binF = new BinaryFormatter();
            this.ds = (DataSet1)binF.Deserialize(fsIn);
            fsIn.Close();

            this.dataGridView1.DataSource = ds.Tables["DataTable1"];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //dataGridView1.Rows.Clear();
            this.ds = new DataSet1();
            dataGridView1.DataSource = ds.Tables["DataTable1"];
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog s_dialog = new SaveFileDialog();
            s_dialog.DefaultExt = ".dat";
            s_dialog.ShowDialog();
            this.SaveData(s_dialog.FileName);
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog o_dialog = new OpenFileDialog();
            o_dialog.DefaultExt = ".dat";
            o_dialog.ShowDialog();
            this.LoadData(o_dialog.FileName);
        }
        

        
    }
    
    }


