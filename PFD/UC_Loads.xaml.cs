using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Configuration;
using BaseClasses;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_Loads.xaml
    /// </summary>
    public partial class UC_Loads : UserControl
    {
        SQLiteConnection conn;

        public UC_Loads()
        {
            InitializeComponent();

            // Fill combobox items
            DatabaseLocations dlocations = new DatabaseLocations();
            foreach (string locationname in dlocations.arr_LocationNames)
                Combobox_Location.Items.Add(locationname);

            // Connect to database and fill items of all comboboxes
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                FillComboboxValues("importancelevel", "imporantacelevelint", ref reader, ref Combobox_ImportanceClass);
                FillComboboxValues("snow_regions", "snow_zone", ref reader, ref Combobox_SnowRegion);
                FillComboboxValues("wind_regions", "id", ref reader, ref Combobox_WindRegion);
                FillComboboxValues("terrain_multiplier", "terrain_category", ref reader, ref Combobox_TerrainRoughness);
                FillComboboxValues("sitesubsoilclass", "class", ref reader, ref Combobox_SubSoilClass);
                reader.Close();
            }

            loadInputComboboxIndexes loadInputIndexes;

            loadInputIndexes.LocationComboboxIndex = 11;
            loadInputIndexes.ImportanceLevelComboboxIndex = 0;
            loadInputIndexes.SnowRegionComboboxIndex = 0;
            loadInputIndexes.WindRegionComboboxIndex = 0;
            loadInputIndexes.TerrainMultiplierComboboxIndex = 0;
            loadInputIndexes.SiteSubSoilClassComboboxIndex = 0;

            // Loading
            CPFDLoadInput loadinput = new CPFDLoadInput(loadInputIndexes);
            loadinput.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = loadinput;
        }
        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDLoadInput loadInput = sender as CPFDLoadInput;
            if (loadInput != null && loadInput.IsSetFromCode) return;
        }
        protected void FillComboboxValues(string sTableName, string sColumnName, ref SQLiteDataReader reader, ref ComboBox combobox)
        {
            SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);

            using (reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    combobox.Items.Add(reader[sColumnName].ToString());
                }
            }
        }
    }
}
