using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;

namespace EXPIMP
{
    public class ExportToExcel
    {
        string fileName;
        DataSet ds;
        public ExportToExcel(string filename,DataSet ds)
        {
            this.fileName = filename;
            this.ds = ds;
        }

        public void writeToExcel()
        {

            System.Data.OleDb.OleDbConnection objConn = new System.Data.OleDb.OleDbConnection(
        "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName +
        ";Extended Properties=Excel 8.0;");
            try
            {
                objConn.Open();
            }
            catch (OleDbException) { return; }


            System.Data.OleDb.OleDbCommand objCmd = new System.Data.OleDb.OleDbCommand();
            objCmd.Connection = objConn;

            try
            {
                objCmd.CommandText = "CREATE TABLE kurz ( Premenna char(40),Hodnota char(20), " +
                        "Jednotka char(20), Premenna1 char(40), Hodnota1 char(20),Jednotka1 char(20), Premenna2 char(40), " +
                        "Hodnota2 char(20),Jednotka2 char(20) )"; ;
                objCmd.ExecuteNonQuery();
            }
            catch (OleDbException e) { MessageBox.Show(e.Message); }

            try
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    objCmd.CommandText = @"INSERT INTO [kurz$] VALUES ('" + r.ItemArray[0].ToString() + "','"
                        + r.ItemArray[1].ToString() + "','" + r.ItemArray[2].ToString() + "','" + r.ItemArray[3].ToString() + "','"
                        + r.ItemArray[4].ToString() + "','" + r.ItemArray[5].ToString() + "','" + r.ItemArray[6].ToString() + "','"
                        + r.ItemArray[7].ToString() + "','" + r.ItemArray[8].ToString() + "')";
                    objCmd.ExecuteNonQuery();
                }
            }
            catch (OleDbException e) { MessageBox.Show(e.Message); }
            

            // Close the connection.
            objConn.Close();

        }

        //public static void Main(string[] args) {
        //ExtractToExcel e= new ExtractToExcel();
        //e.Run();
        //}
        
        //const string ConnStringSource = "Provider=sqloledb;DataSource=dinoch-1\\vsdotnet;Initial Catalog=Northwind;" +
        //    "IntegratedSecurity=SSPI;";
        //string OutputFilename = "";

        //string ConnStringDest = "";
        
        //private OleDbConnection ConnSource = null;
        //private OleDbConnection ConnDest = null;

        //const string sqlSelect = "SELECT * from EN1993-1-1";
        //const string sqlInsert = "INSERT INTO EN1993-1-1 (Premenna, Hodnota,Jednotka, " +
        //    "Premenna1, Hodnota1, Jednotka1,  Premnna2, Hodnota2, Jednotka2) VALUES (@Premenna, @Hodnota,@Jednotka, " +
        //    "@Premenna1, @Hodnota1, @Jednotka1,  @Premnna2, @Hodnota2, @Jednotka2)";
        //const string sqlCreate = "CREATE TABLE EN1993-1-1 ( Premenna char(40),Hodnota NUMBER, " +
        //    "Jednotka char(20), Premenna char(40),Hodnota NUMBER,Jednotka char(20), Premenna char(40), "+
        //    "Hodnota NUMBER,Jednotka char(20) )";

        //OleDbDataAdapter da;
        //DataSet ds;


        ////konstruktor
        //public ExportToExcel(string file_name,DataSet ds) 
        //{
        //    this.OutputFilename = file_name;
        //    this.ds = ds;
        //    this.ConnStringDest =
        //"Provider=Microsoft.Jet.OLEDB.4.0;" +
        //"Data Source=" + file_name + ";" +
        //"Extended Properties=\"Excel 8.0;HDR=yes;\"";
        //}

        //public void CreateTable()
        //{
        //    System.Console.WriteLine("Creating table in Excel file...");

        //    ConnDest = new System.Data.OleDb.OleDbConnection(ConnStringDest);
        //    OleDbCommand cmd = new
        //    OleDbCommand(sqlCreate, ConnDest);
        //    try
        //    {
        //        ConnDest.Open();
        //        cmd.ExecuteNonQuery();
        //    }
        //    catch (System.Exception e2)
        //    {
        //        if (!e2.Message.Trim().EndsWith("already exists."))
        //            System.Console.WriteLine("Error while creating. " + e2);
        //        else
        //            System.Console.WriteLine("File and Table (worksheet) already exist...");
        //    }
        //    finally
        //    {
        //        ConnDest.Close();
        //    }
        //}


        //private void Read()
        //{
        //    da.Fill(ds);            
        //}

        //private void InsertIntoExcel()
        //{
        //    System.Console.WriteLine("Inserting data into Excel...");
            
        //    da.UpdateCommand = new OleDbCommand(sqlInsert);
        //    da.UpdateCommand.Connection = ConnDest;

        //    da.UpdateCommand.Parameters.Add("@Premenna",
        //    OleDbType.VarWChar, 40, "Premenna");
        //    da.UpdateCommand.Parameters.Add("@Hodnota",
        //    OleDbType.Double, 20, "Hodnota");
        //    da.UpdateCommand.Parameters.Add("@Jednotka",
        //    OleDbType.VarWChar, 20, "Jednotka");
        //    da.UpdateCommand.Parameters.Add("@Premenna1",
        //    OleDbType.VarWChar, 40, "Premenna1");
        //    da.UpdateCommand.Parameters.Add("@Hodnota1",
        //    OleDbType.Double, 20, "Hodnota1");
        //    da.UpdateCommand.Parameters.Add("@Jednotka1",
        //    OleDbType.VarWChar, 20, "Jednotka1");
        //    da.UpdateCommand.Parameters.Add("@Premenna2",
        //    OleDbType.VarWChar, 40, "Premenna2");
        //    da.UpdateCommand.Parameters.Add("@Hodnota2",
        //    OleDbType.Double, 20, "Hodnota2");
        //    da.UpdateCommand.Parameters.Add("@Jednotka2",
        //    OleDbType.VarWChar, 20, "Jednotka2");
            

        //    da.Update(ds, "EN1993-1-1");

        //    // in the event you want to update a datasource via a different
        //    //DataAdapter --
        //    // for example you want to fill from a
        //    //System.Data.SqlClient.DataAdapter and
        //    // then Update via a System.Data.Oledb.OledbDataAdapter -- then you
        //    //could define
        //    // two distinct DataAdapters. Fill the DataSet with the first DA,
        //    //then Update
        //    // with the second DA.
        //}

        //private void OpenResultInExcel()
        //{
        //    System.Console.WriteLine("Starting MS-Excel...");
        //    System.Diagnostics.Process.Start(OutputFilename);
        //}



        //public void Run()
        //{
        //    try
        //    {
        //        Read();
        //        CreateTable();
        //        InsertIntoExcel();

        //        OpenResultInExcel();

        //    }
        //    catch (System.Exception e1)
        //    {
        //        System.Console.WriteLine("Exception: " + e1);
        //    }
        //}


    }
}

