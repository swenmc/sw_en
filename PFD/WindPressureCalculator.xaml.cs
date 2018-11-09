using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using BaseClasses;

namespace PFD
{
    /// <summary>
    /// Interaction logic for WindPressureCalculator.xaml
    /// </summary>
    public partial class WindPressureCalculator : Window
    {
        BuildingGeometryDataInput sGeometryInputData;
        public WindPressureCalculator()
        {
            InitializeComponent();

            // Connect to database and fill items of all comboboxes
            CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "ASNZS1170_Tab3_3_DWL", "design_working_life", Combobox_DesignLife);
            CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "ASNZS1170_Tab3_2_IL", "importanceLevelInt", Combobox_ImportanceClass);
            CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "WindRegions", "windRegion", Combobox_WindRegion);
            CComboBoxHelper.FillComboboxValues("MainSQLiteDB", "ASNZS1170_2_421_THM_category", "terrainCategory_abb", Combobox_TerrainCategory);

            for (int i = 0; i < 360; i++)
                Combobox_AngleWindDirection.Items.Add(i);

            loadInputComboboxIndexes sloadInputComboBoxes;

            sloadInputComboBoxes.LocationIndex = 0;
            sloadInputComboBoxes.DesignLifeIndex = 4;
            sloadInputComboBoxes.ExposureCategoryIndex = 0;
            sloadInputComboBoxes.SiteSubSoilClassIndex = 1;
            sloadInputComboBoxes.ImportanceLevelIndex = 1;
            sloadInputComboBoxes.TerrainCategoryIndex = 0;
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

            TextBox_Gable_Width.Text = "20";
            TextBox_Wall_Height.Text = "5";
            TextBox_Length.Text = "30";
            TextBox_Roof_Pitch.Text = "5";

            sGeometryInputData.fW = 20;
            sGeometryInputData.fH_1 = 5;
            sGeometryInputData.fL = 30;
            sGeometryInputData.fRoofPitch_deg = 5;

            sGeometryInputData.fH_2 = 7; // Dopocitat

            // Loading
            CPFDLoadInput loadinput = new CPFDLoadInput(sloadInputComboBoxes, sloadInputTextBoxes);
            this.DataContext = loadinput;
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

            WindLoadDataInput sWindInputData;
            sWindInputData.eWindRegion = loadinput.WindRegion;
            sWindInputData.iAngleWindDirection = loadinput.AngleWindDirectionIndex;
            sWindInputData.fTerrainCategory = loadinput.TerrainCategoryIndex;

            M_EC1.AS_NZS.CCalcul_1170_2 wind = new M_EC1.AS_NZS.CCalcul_1170_2(sBuildingInputData, sGeometryInputData, sWindInputData);

            WindSpeedChart wind_chart = new WindSpeedChart(wind);
            wind_chart.Show();
        }
    }
}
