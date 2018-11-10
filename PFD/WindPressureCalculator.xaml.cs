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
using MATH;

namespace PFD
{
    /// <summary>
    /// Interaction logic for WindPressureCalculator.xaml
    /// </summary>
    public partial class WindPressureCalculator : Window
    {
        BuildingGeometryDataInput sGeometryInputData;
        WindPressureCalculatorViewModel input;
        BuildingDataInput sBuildingInputData;
        WindLoadDataInput sWindInputData;
        M_EC1.AS_NZS.CCalcul_1170_2 windCalcResults;

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

            WindPressureCalculatorViewModel vm = new WindPressureCalculatorViewModel();
            vm.PropertyChanged += HandleComponentViewerPropertyChangedEvent;
            this.DataContext = vm;

            // TODO Ondrej - zobrazovat vsetky faktory v GUI na 3 desatinne miesta (staci zopar ako priklad, ostatne si dopracujem)

            // Calculate
            SetInputAndCalculateWindPressure();

            // Set Resutls
            SetOutputValues();
        }

        private void SetInputAndCalculateWindPressure()
        {
            // Set current input data
            input = this.DataContext as WindPressureCalculatorViewModel;
            // Basic data
            sBuildingInputData.location = ELocation.eAuckland; // Temp - nepouzije sa ???
            sBuildingInputData.fDesignLife_Value = input.DesignLife_Value;                   // Database value in years
            sBuildingInputData.iImportanceClass = input.ImportanceClassIndex + 1;            // Importance Level (index + 1)

            sBuildingInputData.fAnnualProbabilityULS_Snow = 0.0f; // Temp - nepouzije sa
            sBuildingInputData.fAnnualProbabilityULS_Wind = input.AnnualProbabilityULS_Wind; // Annual Probability of Exceedence ULS - Wind
            sBuildingInputData.fAnnualProbabilityULS_EQ = 0.0f; // Temp - nepouzije sa
            sBuildingInputData.fAnnualProbabilitySLS = input.AnnualProbabilitySLS;           // Annual Probability of Exceedence SLS

            sBuildingInputData.fR_ULS_Snow = 0.0f; // Temp - nepouzije sa
            sBuildingInputData.fR_ULS_Wind = input.R_ULS_Wind;
            sBuildingInputData.fR_ULS_EQ = 0.0f; // Temp - nepouzije sa
            sBuildingInputData.fR_SLS = input.R_SLS;

            sGeometryInputData.fW = input.GableWidth;
            sGeometryInputData.fL = input.Length;
            sGeometryInputData.fH_1 = input.WallHeight;
            sGeometryInputData.fRoofPitch_deg = input.RoofPitch_deg;
            sGeometryInputData.fH_2 = input.ApexHeigth_H_2;

            sWindInputData.eWindRegion = input.WindRegion;
            sWindInputData.iAngleWindDirection = input.AngleWindDirectionIndex;
            sWindInputData.fTerrainCategory = input.TerrainCategoryIndex;

            // Calculate
            windCalcResults = new M_EC1.AS_NZS.CCalcul_1170_2(sBuildingInputData, sGeometryInputData, sWindInputData);
        }

        private void SetOutputValues()
        {
            WindPressureCalculatorViewModel vm = this.DataContext as WindPressureCalculatorViewModel;

            // Set results
            vm.TopographicMultiplier_Mt = windCalcResults.fM_t;
            vm.HillShapeMultiplier_Mh = windCalcResults.fM_h;
            vm.ShieldingMultiplier_Ms = windCalcResults.fM_s;
            //vm.WindDirectionMultiplier_Md;
            vm.TerrainHeightMultiplier_Mzcat = windCalcResults.fM_z_cat;

            vm.AreaReductionFactor_Ka = windCalcResults.fK_a_roof;
            vm.LocalPressureFactor_Kl = windCalcResults.fK_l;
            vm.PorousCladdingReductionFactor_Kp = windCalcResults.fK_p;
            vm.CombinationFactorExternalPressures_Kce = windCalcResults.fK_ce;
            vm.CombinationFactorExternalPressures_Kci = windCalcResults.fK_ci;

            vm.InternalPressureCoefficient_Cpimin = windCalcResults.fC_pi_min;
            vm.InternalPressureCoefficient_Cpimax = windCalcResults.fC_pi_max;

            // ROOF
            float fExternalPressureCoefficient_Cpemin_U = MathF.Min(windCalcResults.fC_pe_U_roof_values_min);
            float fExternalPressureCoefficient_Cpemax_U = MathF.Max(windCalcResults.fC_pe_U_roof_values_max);

            float fExternalPressureCoefficient_Cpemin_D = MathF.Min(windCalcResults.fC_pe_D_roof_values_min);
            float fExternalPressureCoefficient_Cpemax_D = MathF.Max(windCalcResults.fC_pe_D_roof_values_max);

            float fExternalPressureCoefficient_Cpemin_R = MathF.Min(windCalcResults.fC_pe_R_roof_values_min);
            float fExternalPressureCoefficient_Cpemax_R = MathF.Max(windCalcResults.fC_pe_R_roof_values_max);

            vm.ExternalPressureCoefficient_Cpemin = MathF.Min(fExternalPressureCoefficient_Cpemin_U, fExternalPressureCoefficient_Cpemin_D, fExternalPressureCoefficient_Cpemin_R);
            vm.ExternalPressureCoefficient_Cpemax = MathF.Max(fExternalPressureCoefficient_Cpemax_U, fExternalPressureCoefficient_Cpemax_D, fExternalPressureCoefficient_Cpemax_R);

            vm.AerodynamicShapeFactor_Cfigimin = windCalcResults.fC_fig_i_min;
            vm.AerodynamicShapeFactor_Cfigimax = windCalcResults.fC_fig_i_max;

            // ROOF
            float fAerodynamicShapeFactor_Cfigemin_U = MathF.Min(windCalcResults.fC_fig_e_U_roof_values_min);
            float fAerodynamicShapeFactor_Cfigemax_U = MathF.Max(windCalcResults.fC_fig_e_U_roof_values_max);

            float fAerodynamicShapeFactor_Cfigemin_D = MathF.Min(windCalcResults.fC_fig_e_D_roof_values_min);
            float fAerodynamicShapeFactor_Cfigemax_D = MathF.Max(windCalcResults.fC_fig_e_D_roof_values_max);

            float fAerodynamicShapeFactor_Cfigemin_R = MathF.Min(windCalcResults.fC_fig_e_R_roof_values_min);
            float fAerodynamicShapeFactor_Cfigemax_R = MathF.Max(windCalcResults.fC_fig_e_R_roof_values_max);

            vm.AerodynamicShapeFactor_Cfigemin = MathF.Min(fAerodynamicShapeFactor_Cfigemin_U, fAerodynamicShapeFactor_Cfigemin_D, fAerodynamicShapeFactor_Cfigemin_R);
            vm.AerodynamicShapeFactor_Cfigemax = MathF.Min(fAerodynamicShapeFactor_Cfigemax_U, fAerodynamicShapeFactor_Cfigemax_D, fAerodynamicShapeFactor_Cfigemax_R);
        }

        private void WindSpeedChart_Click(object sender, RoutedEventArgs e)
        {
            WindSpeedChart wind_chart = new WindSpeedChart(windCalcResults);
            wind_chart.Show();
        }

        private void HandleComponentViewerPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is WindPressureCalculatorViewModel)
            {
                // Calculate
                //SetInputAndCalculateWindPressure();

                // Set Resutls
                //SetOutputValues();
            }
        }
    }
}
