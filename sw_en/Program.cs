using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data.OleDb;

namespace CENEX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            
        //    //OleDbConnection aConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Password=;User ID=Admin;Data Source=c:\\ACCESS+TAB+POKUS.mdb;Jet OLEDB:Database Password='cudo';");
        //    OleDbConnection aConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Password=;User ID=Admin;Data Source=DATA1.mdb;Jet OLEDB:Database Password='cudo';");

        ////create the command object and store the sql query
        //OleDbCommand aCommand = new OleDbCommand("select * from STEEL", aConnection);

       
        //    aConnection.Open();

        //    //create the datareader object to connect to table
        //    OleDbDataReader aReader = aCommand.ExecuteReader();
        //    Console.WriteLine("This is the returned data from emp_test table");

        //    //Iterate throuth the database
        //    object[] values = new object[20];
        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
