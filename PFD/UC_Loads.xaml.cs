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
using DATABASE;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_Loads.xaml
    /// </summary>
    public partial class UC_Loads : UserControl
    {
        BuildingGeometryDataInput sGeometryInputData;
        public UC_Loads(BuildingGeometryDataInput geometryInputData)
        {
            InitializeComponent();

            sGeometryInputData = geometryInputData;

            // Connect to database and fill items of all comboboxes
            //CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "nzLocations", "city", Combobox_Location);
            //CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "ASNZS1170_Tab3_3_DWL", "design_working_life", Combobox_DesignLife);
            //CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "ASNZS1170_Tab3_2_IL", "importanceLevelInt", Combobox_ImportanceClass);
            //CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "SnowRegions", "snowZone", Combobox_SnowRegion);
            //CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "ExposureReductionCoefficient", "categoryName", Combobox_ExposureCategory);
            //CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "WindRegions", "windRegion", Combobox_WindRegion);
            //CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "ASNZS1170_2_421_THM_category", "terrainCategory_abb", Combobox_TerrainCategory);
            //CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "SiteSubSoilClass", "class", Combobox_SiteSubSoilClass);

            List<string> listLocations = CDatabaseManager.GetStringList("MainSQLiteDB", "nzLocations", "city");
            List<string> listDesignLife = CDatabaseManager.GetStringList("MainSQLiteDB", "ASNZS1170_Tab3_3_DWL", "design_working_life");
            List<string> listImportanceClass = CDatabaseManager.GetStringList("MainSQLiteDB", "ASNZS1170_Tab3_2_IL", "importanceLevelInt");
            List<string> listSnowRegion = CDatabaseManager.GetStringList("MainSQLiteDB", "SnowRegions", "snowZone");
            List<string> listExposureCategory = CDatabaseManager.GetStringList("MainSQLiteDB", "ExposureReductionCoefficient", "categoryName");
            List<string> listWindRegion = CDatabaseManager.GetStringList("MainSQLiteDB", "WindRegions", "windRegion");
            List<string> listTerrainCategory = CDatabaseManager.GetStringList("MainSQLiteDB", "ASNZS1170_2_421_THM_category", "terrainCategory_abb");
            List<string> listSiteSubSoilClass = CDatabaseManager.GetStringList("MainSQLiteDB", "SiteSubSoilClass", "class");
            
            for (int i = 0; i < 360; i++)
                Combobox_AngleWindDirection.Items.Add(i);

            loadInputComboboxIndexes sloadInputComboBoxes;

            sloadInputComboBoxes.LocationIndex = 0;
            sloadInputComboBoxes.DesignLifeIndex = 4;
            sloadInputComboBoxes.ExposureCategoryIndex = 0;
            sloadInputComboBoxes.SiteSubSoilClassIndex = 1;
            sloadInputComboBoxes.ImportanceLevelIndex = 1;
            sloadInputComboBoxes.TerrainCategoryIndex = 4;     // Default terrain category 3
            sloadInputComboBoxes.AngleWindDirectionIndex = 90; // Default ??? see Figure 2.2

            loadInputTextBoxValues sloadInputTextBoxes;

            sloadInputTextBoxes.SiteElevation = 30;               // m  // nastavovat tu - zavisi od Location Index
            sloadInputTextBoxes.FaultDistanceDmin_km = 0f;        // km // nastavovat tu - zavisi od Location Index (osetrit nacitanie z databazy, ak je null)
            sloadInputTextBoxes.FaultDistanceDmax_km = 0f;        // km // nastavovat tu - zavisi od Location Index (osetrit nacitanie z databazy, ak je null)
            sloadInputTextBoxes.PeriodAlongXDirectionTx = 0.4f;
            sloadInputTextBoxes.PeriodAlongYDirectionTy = 0.4f;
            sloadInputTextBoxes.AdditionalDeadActionRoof = 0f;    // kN / m^2
            sloadInputTextBoxes.AdditionalDeadActionWall = 0f;    // kN / m^2
            sloadInputTextBoxes.ImposedActionRoof = 0.25f; // kN / m^2

            // Loading
            CPFDLoadInput loadinput = new CPFDLoadInput(sloadInputComboBoxes, sloadInputTextBoxes);
            loadinput.ListLocations = listLocations;
            loadinput.ListDesignLife = listDesignLife;
            loadinput.ListImportanceClass = listImportanceClass;
            loadinput.ListSnowRegion = listSnowRegion;
            loadinput.ListExposureCategory = listExposureCategory;
            loadinput.ListWindRegion = listWindRegion;
            loadinput.ListTerrainCategory = listTerrainCategory;
            loadinput.ListSiteSubSoilClass = listSiteSubSoilClass;

            loadinput.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = loadinput;
        }
        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDLoadInput loadInput = sender as CPFDLoadInput;
            if (loadInput != null && loadInput.IsSetFromCode) return;
        }

        private void WindSpeedChart_Click(object sender, RoutedEventArgs e)
        {
            CPFDLoadInput loadinput = this.DataContext as CPFDLoadInput;
            // Basic data
            BuildingDataInput sBuildingInputData;
            sBuildingInputData.location = (ELocation)loadinput.LocationIndex;                    // locations (cities) enum
            sBuildingInputData.fDesignLife_Value = loadinput.DesignLife_Value;                   // Database value in years
            sBuildingInputData.iImportanceClass = loadinput.ImportanceClassIndex + 1;            // Importance Level (index + 1)

            sBuildingInputData.fAnnualProbabilityULS_Snow = loadinput.AnnualProbabilityULS_Snow; // Annual Probability of Exceedence ULS - Snow
            sBuildingInputData.fAnnualProbabilityULS_Wind = loadinput.AnnualProbabilityULS_Wind; // Annual Probability of Exceedence ULS - Wind
            sBuildingInputData.fAnnualProbabilityULS_EQ = loadinput.AnnualProbabilityULS_EQ;     // Annual Probability of Exceedence ULS - EQ
            sBuildingInputData.fAnnualProbabilitySLS = loadinput.AnnualProbabilitySLS;           // Annual Probability of Exceedence SLS

            sBuildingInputData.fR_ULS_Snow = loadinput.R_ULS_Snow;
            sBuildingInputData.fR_ULS_Wind = loadinput.R_ULS_Wind;
            sBuildingInputData.fR_ULS_EQ = loadinput.R_ULS_EQ;
            sBuildingInputData.fR_SLS = loadinput.R_SLS;

            sBuildingInputData.fE = loadinput.SiteElevation;

            WindLoadDataInput sWindInputData;
            sWindInputData.eWindRegion = loadinput.WindRegion;
            sWindInputData.iAngleWindDirection = loadinput.AngleWindDirectionIndex;
            sWindInputData.fTerrainCategory = GetTerrainCategory(loadinput.TerrainCategoryIndex);

            M_EC1.AS_NZS.CCalcul_1170_2 wind = new M_EC1.AS_NZS.CCalcul_1170_2(sBuildingInputData, sGeometryInputData, sWindInputData);

            WindSpeedChart wind_chart = new WindSpeedChart(wind);
            wind_chart.Show();
        }

        private float GetTerrainCategory(int iCategoryIndex) // TODO - prerobit na nacitanie z databazy
        {
            if (iCategoryIndex == 0)
                return 1.0f;
            else if (iCategoryIndex == 1)
                return 1.5f;
            else if (iCategoryIndex == 2)
                return 2.0f;
            else if (iCategoryIndex == 3)
                return 2.5f;
            else if (iCategoryIndex == 4)
                return 3.0f;
            else if (iCategoryIndex == 5)
                return 4.0f;
            else
            {
                // Invalid index
                return -1;
            }
        }
    }
}
