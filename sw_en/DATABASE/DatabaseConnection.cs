using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CENEX
{
    class DatabaseConnection
    {
        private static DatabaseConnection instanceDC;
        private OleDbConnection dat_connection;
        private OleDbDataReader dat_reader;
        private DatabaseConnection()
        {
            //create the database connection
            //OleDbConnection aConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\ACCESS+TAB+POKUS.mdb;");
             this.dat_connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Password=;User ID=Admin;Data Source=DATA1.mdb;Jet OLEDB:Database Password='cudo';");
            dat_connection.Open();
                 
        }

        public static DatabaseConnection getInstance() 
        {
            if (instanceDC == null) instanceDC = new DatabaseConnection();
            return instanceDC;
        }
        public void closeConnection() 
        {
            //close the reader 
            dat_reader.Close();
            //close the connection Its important.
            dat_connection.Close();
        }

        public OleDbDataReader getDBReader(string command)
        {
            OleDbCommand sqlCommand = new OleDbCommand(command, dat_connection);
            try
            {
                //create the datareader object to connect to table
                this.dat_reader = sqlCommand.ExecuteReader();
            }

            //Some usual exception handling
            catch (OleDbException e)
            {
                MessageBox.Show("Error: {0}", e.Errors[0].Message);
                return dat_reader;

            }
            return dat_reader;

        }
            
            
        
    }
    
}
