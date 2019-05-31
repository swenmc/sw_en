using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CJointsManager
    {
        public static List<CConnectionDescription> LoadJointsConnectionDescriptions()
        {
            CConnectionDescription item;
            List<CConnectionDescription> items = new List<CConnectionDescription>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from connectionDescription", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = new CConnectionDescription();
                        item.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        item.Name = reader["name"].ToString();
                        item.JoinType = reader["jointType"].ToString();
                        items.Add(item);
                    }
                }
            }
            return items;
        }
    }
}
