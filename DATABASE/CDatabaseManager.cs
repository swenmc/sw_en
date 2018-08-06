using DATABASE.DTO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;

namespace DATABASE
{
    public static class CDatabaseManager
    {
        public static List<string> GetStringList(string connStringDBName, string tableName, string columnName)
        {
            List<string> items = new List<string>();            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[connStringDBName].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from " + tableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(reader[columnName].ToString());
                    }
                }
            }
            return items;
        }



    }
}
