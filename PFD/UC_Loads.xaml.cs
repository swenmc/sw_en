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

            // Connect to database and fill items of all comboboxes
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                FillComboboxValues("nzLocations", "city", ref reader, ref Combobox_Location);
                FillComboboxValues("ASNZS1170_Tab3_3_DWL", "design_working_life", ref reader, ref Combobox_DesignLife);
                FillComboboxValues("ASNZS1170_Tab3_2_IL", "importanceLevelInt", ref reader, ref Combobox_ImportanceClass);
                FillComboboxValues("SnowRegions", "snowZone", ref reader, ref Combobox_SnowRegion);
                FillComboboxValues("WindRegions", "windRegion", ref reader, ref Combobox_WindRegion);
                FillComboboxValues("ASNZS1170_2_421_THM_category", "terrainCategory_abb", ref reader, ref Combobox_TerrainRoughness);
                FillComboboxValues("SiteSubSoilClass", "class", ref reader, ref Combobox_SiteSubSoilClass);
                reader.Close();
            }

            loadInputComboboxIndexes sloadInput;

            sloadInput.LocationIndex = 0;
            sloadInput.DesignLifeIndex = 4;
            sloadInput.SiteSubSoilClassIndex = 1;
            sloadInput.ImportanceLevelIndex = 1;
            sloadInput.TerrainRoughnessIndex = 0;
            sloadInput.FaultDistanceDmin = 20f; // km
            sloadInput.FaultDistanceDmax = 20f; // km
            sloadInput.PeriodAlongXDirectionTx = 0.4f;
            sloadInput.PeriodAlongYDirectionTy = 0.4f;

            // Loading
            CPFDLoadInput loadinput = new CPFDLoadInput(sloadInput);
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
