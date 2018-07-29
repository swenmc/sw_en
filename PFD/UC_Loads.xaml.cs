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

            for (int i = 0; i < 360; i++)
                Combobox_AngleWindDirection.Items.Add(i);

            loadInputComboboxIndexes sloadInputComboBoxes;

            sloadInputComboBoxes.LocationIndex = 0;
            sloadInputComboBoxes.DesignLifeIndex = 4;
            sloadInputComboBoxes.SiteSubSoilClassIndex = 1;
            sloadInputComboBoxes.ImportanceLevelIndex = 1;
            sloadInputComboBoxes.TerrainRoughnessIndex = 0;
            sloadInputComboBoxes.AngleWindDirectionIndex = 90; // Default ??? see Figure 2.2

            loadInputTextBoxValues sloadInputTextBoxes;

            sloadInputTextBoxes.SiteElevation = 30;      // m  // nastavovat tu - zavisi od Location Index
            sloadInputTextBoxes.FaultDistanceDmin = 0f; // km // nastavovat tu - zavisi od Location Index (osetrit nacitanie z databazy, ak je null)
            sloadInputTextBoxes.FaultDistanceDmax = 0f; // km // nastavovat tu - zavisi od Location Index (osetrit nacitanie z databazy, ak je null)
            sloadInputTextBoxes.PeriodAlongXDirectionTx = 0.4f;
            sloadInputTextBoxes.PeriodAlongYDirectionTy = 0.4f;

            // Loading
            CPFDLoadInput loadinput = new CPFDLoadInput(sloadInputComboBoxes, sloadInputTextBoxes);
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
