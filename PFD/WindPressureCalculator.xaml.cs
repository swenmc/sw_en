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

            Combobox_LocalPressureReferenceUpwind.Items.Add("Undefined");
            Combobox_LocalPressureReferenceUpwind.Items.Add("WA1");
            Combobox_LocalPressureReferenceUpwind.Items.Add("RC1");
            Combobox_LocalPressureReferenceUpwind.Items.Add("RA1");
            Combobox_LocalPressureReferenceUpwind.Items.Add("RA2");
            Combobox_LocalPressureReferenceUpwind.Items.Add("RA3");
            Combobox_LocalPressureReferenceUpwind.Items.Add("RA4");

            Combobox_LocalPressureReferenceDownwind.Items.Add("Undefined");
            Combobox_LocalPressureReferenceDownwind.Items.Add("WA1");
            Combobox_LocalPressureReferenceDownwind.Items.Add("RC1");
            Combobox_LocalPressureReferenceDownwind.Items.Add("RA1");
            Combobox_LocalPressureReferenceDownwind.Items.Add("RA2");
            Combobox_LocalPressureReferenceDownwind.Items.Add("RA3");
            Combobox_LocalPressureReferenceDownwind.Items.Add("RA4");

            WindPressureCalculatorViewModel vm = new WindPressureCalculatorViewModel();
            vm.PropertyChanged += HandleComponentViewerPropertyChangedEvent;
            this.DataContext = vm;

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

            sBuildingInputData.fE = input.SiteElevation;

            sGeometryInputData.fW = input.GableWidth;
            sGeometryInputData.fL = input.Length;
            sGeometryInputData.fH_1 = input.WallHeight;
            sGeometryInputData.fRoofPitch_deg = input.RoofPitch_deg;
            sGeometryInputData.fH_2 = sGeometryInputData.fH_1 + (float)Math.Tan(sGeometryInputData.fRoofPitch_deg / 180 * Math.PI) * 0.5f * sGeometryInputData.fW;

            sWindInputData.eWindRegion = input.WindRegion;
            sWindInputData.iAngleWindDirection = input.AngleWindDirectionIndex;
            sWindInputData.fTerrainCategory = GetTerrainCategory(input.TerrainCategoryIndex);

            WindLoadDataSpecificInput sWindInputSpecificData;
            //sWindInputSpecificData.fz = input.AverageStructureHeight_h; //input.ApexHeigth_H_2; // Generally, the wind speed is determined at the average roof height (h).
            //sWindInputSpecificData.fh = input.AverageStructureHeight_h;

            sWindInputSpecificData.eLocalPressureReferenceUpwind = (ELocalWindPressureReference)input.LocalPressureReferenceUpwindIndex;
            sWindInputSpecificData.eLocalPressureReferenceDownwind = (ELocalWindPressureReference)input.LocalPressureReferenceDownwindIndex;

            sWindInputSpecificData.fTributaryArea = input.TributaryArea_A;

            sWindInputSpecificData.fM_lee = input.LeeMultiplier_Mlee;
            sWindInputSpecificData.fM_h = input.HillShapeMultiplier_Mh;
            sWindInputSpecificData.fM_s = input.ShieldingMultiplier_Ms;

            sWindInputSpecificData.fK_p = input.PorousCladdingReductionFactor_Kp;
            sWindInputSpecificData.fK_ce = input.CombinationFactorExternalPressures_Kce;
            sWindInputSpecificData.fK_ci = input.CombinationFactorExternalPressures_Kci;

            // Calculate
            windCalcResults = new M_EC1.AS_NZS.CCalcul_1170_2(sBuildingInputData, sGeometryInputData, sWindInputData, sWindInputSpecificData);
        }

        private void SetOutputValues()
        {
            WindPressureCalculatorViewModel vm = this.DataContext as WindPressureCalculatorViewModel;

            // Set results
            vm.AverageStructureHeight_h = windCalcResults.fh;

            vm.TopographicMultiplier_Mt = windCalcResults.fM_t;
            //vm.WindDirectionMultiplier_Md;
            vm.TerrainHeightMultiplier_Mzcat = windCalcResults.fM_z_cat;

            vm.AreaReductionFactor_Ka = windCalcResults.fK_a_roof;
            vm.LocalPressureFactorUpwind_Kl = windCalcResults.fK_l_upwind;
            vm.LocalPressureFactorDownwind_Kl = windCalcResults.fK_l_downwind;

            vm.WindSpeed_VR = windCalcResults.fV_R_ULS; // ULS
            vm.WindSpeed_VsitBeta = MathF.Max(windCalcResults.fV_sit_ULS_Theta_4); // ULS
            vm.WindSpeed_VdesTheta = MathF.Max(windCalcResults.fV_des_ULS_Theta_4); // ULS
            vm.WindPressure_p_basic = MathF.Max(windCalcResults.fp_basic_ULS_Theta_4); // ULS

            // Cp
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

            // Cfig
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

            // Pressures - ULS

            vm.WindPressure_pimin = MathF.Min(windCalcResults.fp_i_min_ULS_Theta_4); // ULS
            vm.WindPressure_pimax = MathF.Min(windCalcResults.fp_i_max_ULS_Theta_4); // ULS

            float fWindPressure_pemin_U = MathF.Min(windCalcResults.fp_e_min_U_roof_ULS_Theta_4); // ULS
            float fWindPressure_pemax_U = MathF.Max(windCalcResults.fp_e_max_U_roof_ULS_Theta_4); // ULS

            float fWindPressure_pemin_D = MathF.Min(windCalcResults.fp_e_min_D_roof_ULS_Theta_4); // ULS
            float fWindPressure_pemax_D = MathF.Max(windCalcResults.fp_e_max_D_roof_ULS_Theta_4); // ULS

            float fWindPressure_pemin_R = MathF.Min(windCalcResults.fp_e_min_R_roof_ULS_Theta_4); // ULS
            float fWindPressure_pemax_R = MathF.Max(windCalcResults.fp_e_max_R_roof_ULS_Theta_4); // ULS

            vm.WindPressure_pemin = MathF.Min(fWindPressure_pemin_U, fWindPressure_pemin_D, fWindPressure_pemin_R);
            vm.WindPressure_pemax = MathF.Min(fWindPressure_pemax_U, fWindPressure_pemax_D, fWindPressure_pemax_R);
        }

        private void WindRegionMap_Click(object sender, RoutedEventArgs e)
        {
            WindRegionMap win_windregionmap = new WindRegionMap();
            win_windregionmap.Show();
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
                if (IsInputProperty(e.PropertyName))
                {
                    // Calculate
                    SetInputAndCalculateWindPressure();

                    // Set Resutls
                    SetOutputValues();
                }
            }
        }

        private bool IsInputProperty(string propName)
        {
            List<string> list = new List<string>()
            {   "DesignLifeIndex",
                "ImportanceClassIndex",
                "AnnualProbabilityULS_Wind",
                "AnnualProbabilitySLS",
                "R_ULS_Wind",
                "R_SLS",
                "SiteElevation",

                "WindRegionIndex",
                "TerrainCategoryIndex",
                "AngleWindDirectionIndex",

                "LeeMultiplier_Mlee",
                "HillShapeMultiplier_Mh",
                "ShieldingMultiplier_Ms",
                //"WindDirectionMultiplier_Md", // neaktivovat

                "LocalPressureReferenceUpwindIndex",
                "LocalPressureReferenceDownwindIndex",
                "TributaryArea_A",
                "PorousCladdingReductionFactor_Kp",
                "CombinationFactorExternalPressures_Kce",
                "CombinationFactorExternalPressures_Kci",

                "GableWidth",
                "Length",
                "WallHeight",
                "RoofPitch_deg",
                //"ApexHeigth_H_2" // neaktivovat
                //"AverageStructureHeight_h" // neaktivovat
            };

            return list.Contains(propName);
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

        public void WindPressureCalculator_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindPressureCalculatorViewModel w = sender as WindPressureCalculatorViewModel;

            // TODO - Ondrej
        }
    }
}
