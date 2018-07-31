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

namespace PFD
{
    public static class DatabaseManager
    {
        public static void FillComboboxValues(string sDBName, string sTableName, string sColumnName, ComboBox combobox)
        {
            List<string> items = new List<string>();
            // Connect to database and fill items of all comboboxes
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[sDBName].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(reader[sColumnName].ToString());
                    }
                }
            }            
            combobox.ItemsSource = items;
        }

        public static void FillComboboxWithColors(string sDBName, string sTableName, string sColumnText, string sColumnColor, ComboBox combobox)
        {
            List<ComboBoxItem> items = new List<ComboBoxItem>();
            // Connect to database and fill items of all comboboxes
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[sDBName].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ComboBoxItem cbi = new ComboBoxItem();
                        try
                        {
                            string[] splitArray = reader[sColumnColor].ToString().Split(',');
                            cbi.Background = new SolidColorBrush(Color.FromRgb(byte.Parse(splitArray[0]), byte.Parse(splitArray[1]), byte.Parse(splitArray[2])));
                        }
                        catch (Exception) {/*tha mne sa nechce riesit ze su debilne data v DB*/ }
                        
                        cbi.Content = reader[sColumnText].ToString();
                        items.Add(cbi);
                    }
                }
            }
            combobox.ItemsSource = items;
        }


        

    }
}
