using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CQuantityLibrary
    {
        private static Dictionary<string, QuantityLibraryItem> QuantityLibrary = null;

        public static Dictionary<string, QuantityLibraryItem> GetQuantityLibrary()
        {
            if (QuantityLibrary != null) return QuantityLibrary;

            QuantityLibraryItem item = null;
            Dictionary<string, QuantityLibraryItem> items = new Dictionary<string, QuantityLibraryItem>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["StringsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from QuantityLibrary", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetQuantityLibraryItem(reader);
                        items.Add(item.UnitIdentificator, item);
                    }
                }
            }
            QuantityLibrary = items;
            return items;
        }

        public static string GetReportUnit(string UnitIdentificator)
        {
            if (QuantityLibrary == null) QuantityLibrary = GetQuantityLibrary();
            return QuantityLibrary[UnitIdentificator].ReportUnit;
        }
       
        private static QuantityLibraryItem GetQuantityLibraryItem(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            QuantityLibraryItem item = new QuantityLibraryItem();
            item.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            item.UnitIdentificator = reader["UnitIdentificator"].ToString();
            item.BasicUnit = reader["BasicUnit"].ToString();
            item.GUIUnit = reader["GUIUnit"].ToString();

            // TO Ondrej - toto sa Ti nebude pacit, ale v databaze mam vsetko okrem IDs ako text, tak som to tu zmenil
            // Pouzivat exportovaci nastroj z xls do sql http://converttosqlite.com/convert/
            // a musel by som tam pri kazdom exporte nastavovat rucne datove typy.

            // Ak najdes lepsi nastroj na konverziu alebo napises vlastny (:-),
            // ktory by to urobil nejako automaticky za mna tak sa nebranim tomu, ze budu mat stlpce v databaze specificky datovy typ a tieto pretypovania by sme mohli zmazat

            if (!reader.IsDBNull(reader.GetOrdinal("GUIUnitFactor"))) item.GUIUnitFactor = float.Parse(reader["GUIUnitFactor"].ToString(), nfi); //  reader.GetFloat(reader.GetOrdinal("GUIUnitFactor"));
            if (!reader.IsDBNull(reader.GetOrdinal("GUIDecimalPlaces"))) item.GUIDecimalPlaces = Int32.Parse(reader["GUIDecimalPlaces"].ToString()); // reader.GetInt32(reader.GetOrdinal("GUIDecimalPlaces"));            
            item.ReportUnit = reader["ReportUnit"].ToString();
            if (!reader.IsDBNull(reader.GetOrdinal("ReportUnitFactor"))) item.ReportUnitFactor = float.Parse(reader["ReportUnitFactor"].ToString(), nfi);//reader.GetFloat(reader.GetOrdinal("ReportUnitFactor"));
            if (!reader.IsDBNull(reader.GetOrdinal("ReportDecimalPlaces"))) item.ReportDecimalPlaces = Int32.Parse(reader["ReportDecimalPlaces"].ToString()); //reader.GetInt32(reader.GetOrdinal("ReportDecimalPlaces"));

            return item;
        }
       
    }
}
