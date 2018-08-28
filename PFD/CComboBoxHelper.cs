using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BaseClasses;
using System.Data.SQLite;
using System.Configuration;
using System.Windows.Controls;
using System.Globalization;
using DATABASE;
using DATABASE.DTO;

namespace PFD
{
    public static class CComboBoxHelper
    {
        public static void FillComboboxValues(string sDBName, string sTableName, string sColumnName, ComboBox combobox)
        {
            combobox.ItemsSource = CDatabaseManager.GetStringList(sDBName, sTableName, sColumnName);
        }

        public static void FillComboboxWithColors(ComboBox combobox)
        {
            List<CTrapezoidalSheetingColours> colours = CTrapezoidalSheetingManager.LoadTrapezoidalSheetingColours();
            combobox.ItemsSource = colours;

            //List<Tuple<string, string>> color_items = new List<Tuple<string, string>>();

            //// Connect to database and fill items of all comboboxes
            //using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[sDBName].ConnectionString))
            //{
            //    conn.Open();
            //    SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);
            //    using (SQLiteDataReader reader = command.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {                        
            //            color_items.Add(Tuple.Create<string, string>(reader[sColumnText].ToString(), "#"+reader[sColumnColor].ToString()));                        
            //        }
            //    }
            //}
            //combobox.ItemsSource = color_items;
        }

        // TODO Ondrej - ak je mozne zobecnit tuto funkciu tak, aby to vracalo rozne typy podla typu, aky zistilo v stlpci "sColumnName"
        // Hruza a des. Neverim,ze taku vseobecnu metodu nam treba
        // To bol len taky "napad", ze by to bolo super :)

        public static float GetValueFromDatabasebyRowID(string sDBName, string sTableName, string sColumnName, int IDValue, string sKeyColumnName = "ID")
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            float fValue = float.NaN;

            // Connect to database
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[sDBName].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where " + sKeyColumnName + " = '" + IDValue + "'", conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fValue = float.Parse(reader[sColumnName].ToString(), nfi);
                    }
                }

                reader.Close();
            }

            return fValue;
        }
    }
}
