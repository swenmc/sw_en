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
using System.Windows.Interactivity;
using MATH;
using BaseClasses;
using CRSC;
using DATABASE;
using M_AS4600;

namespace PFD
{
    /// <summary>
    /// Interaction logic for PurlinDesigner.xaml
    /// </summary>
    public partial class PurlinDesigner : Window
    {
        //PurlinDesignerViewModel calcModel;

        public PurlinDesigner()
        {
            InitializeComponent();

            Combobox_CrossSection.Items.Add("27095");
            Combobox_CrossSection.Items.Add("270115");
            //Combobox_CrossSection.Items.Add("270155");
            //Combobox_CrossSection.Items.Add("270195");
            Combobox_CrossSection.Items.Add("50020");
            Combobox_CrossSection.Items.Add("27095n");
            Combobox_CrossSection.Items.Add("270115n");
            //Combobox_CrossSection.Items.Add("270155n");
            //Combobox_CrossSection.Items.Add("270195n");
            Combobox_CrossSection.Items.Add("50020n");

            Combobox_Material.Items.Add("G550‡");

            PurlinDesignerViewModel vm = new PurlinDesignerViewModel();
            vm.PropertyChanged += HandleComponentViewerPropertyChangedEvent;
            this.DataContext = vm;

            // Calculate
            SetInputAndCalculate();

            // Set Results
            SetOutputValues();
        }

        private void HandleComponentViewerPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is PurlinDesignerViewModel)
            {
                if (IsInputProperty(e.PropertyName))
                {
                    // Calculate
                    SetInputAndCalculate();

                    // Set Results
                    SetOutputValues();
                }
            }
        }

        private bool IsInputProperty(string propName)
        {
            List<string> list = new List<string>()
            {   "Length_L",
                "BracingLength_Lb",
                "TributaryWidth_B",
                "CrossSectionIndex",

                "CladdingSelfWeight_gc",
                "AdditionalDeadLoad_g",
                "LiveLoad_q",
                "SnowLoad_s",

                "WindLoadInternalPressure_pimin",
                "WindLoadInternalPressure_pimax",
                "WindLoadExternalPressure_pemin",
                "WindLoadExternalPressure_pemax",

                "DeflectionGQLimitFraction",
                "DeflectionSWLimitFraction",
                "DeflectionTotalLimitFraction"
            };

            return list.Contains(propName);
        }

        void SetInputAndCalculate()
        {
            PurlinDesignerViewModel calcModel = this.DataContext as PurlinDesignerViewModel;

            calcModel.TributaryArea_A = calcModel.Length_L * calcModel.TributaryWidth_B;

            MATERIAL.CMat_03_00 mat = new MATERIAL.CMat_03_00();
            CMaterialManager.LoadMaterialProperties(mat, (string)Combobox_Material.SelectedValue);
            mat.m_fE = 2e+11f; // Change default value of E (see AS 4600)

            CCrSc_TW cs = new CCrSc_3_270XX_C(0, 0.27f, 0.07f, 0.00095f, Colors.Aquamarine);
            CSectionManager.LoadCrossSectionProperties_meters(cs, (string)Combobox_CrossSection.SelectedValue);
            cs.m_Mat = mat; // Set material from GUI to the cross-section

            CMember member = new CMember(0, new CNode(0, 0, 0, 0), new CNode(1, calcModel.Length_L, 0, 0), cs, 0);

            calcModel.Area_Ag = (float)cs.A_g;
            calcModel.MomentOfInertia_Ix = (float)cs.I_y; // (Ix = Iy a Iy = Iz podla AS 4600)
            float fI_x_eff = 0.9f * calcModel.MomentOfInertia_Ix; // TODO - doplnit aj Ieff, zistit ci sa da pre simply supported beam vyjadrit faktorom

            calcModel.PurlinSelfWeight_gp = calcModel.Area_Ag * mat.m_fRho;

            calcModel.AdditionalDeadLoad_gl =  calcModel.AdditionalDeadLoad_g * calcModel.TributaryWidth_B;

            calcModel.CladdingSelfWeight_gcl = calcModel.CladdingSelfWeight_gc * calcModel.TributaryWidth_B;
            calcModel.AdditionalDeadLoad_gl = calcModel.AdditionalDeadLoad_g * calcModel.TributaryWidth_B;
            calcModel.LiveLoad_ql = calcModel.LiveLoad_q * calcModel.TributaryWidth_B;
            calcModel.SnowLoad_sl = calcModel.SnowLoad_s * calcModel.TributaryWidth_B;
            calcModel.InternalPressure_piminl = calcModel.WindLoadInternalPressure_pimin * calcModel.TributaryWidth_B;
            calcModel.InternalPressure_pimaxl = calcModel.WindLoadInternalPressure_pimax * calcModel.TributaryWidth_B;
            calcModel.ExternalPressure_peminl = calcModel.WindLoadExternalPressure_pemin * calcModel.TributaryWidth_B;
            calcModel.ExternalPressure_pemaxl = calcModel.WindLoadExternalPressure_pemax * calcModel.TributaryWidth_B;

            float fTotalDeadLoad_l = calcModel.CladdingSelfWeight_gcl + calcModel.AdditionalDeadLoad_gl + calcModel.CladdingSelfWeight_gcl + calcModel.AdditionalDeadLoad_gl;
            calcModel.WindLoadUpwind_puwl = Math.Abs(-calcModel.InternalPressure_pimaxl + calcModel.ExternalPressure_peminl);
            calcModel.WindLoadDownwind_pdwl = Math.Abs(-calcModel.InternalPressure_piminl + calcModel.ExternalPressure_pemaxl);

            // Load Combinations

            /*
            1.2G + 1.5Q
            0.9G + W1
            1.2G + W2
            1.2G + Su
            G + 0.65*W1
            G + 0.65*W2
            */

            float fGamma_G_stab = 0.9f;
            float fGamma_G_dest = 1.2f;
            float fGamma_Q = 1.5f;
            float fPsi_liveload = 0.7f;
            float fPsi_wind = 0.65f;

            float fload_CO1_ULS = fGamma_G_dest * fTotalDeadLoad_l + fGamma_Q * calcModel.LiveLoad_ql;
            float fload_CO2_ULS = fGamma_G_stab * (fTotalDeadLoad_l - calcModel.AdditionalDeadLoad_gl) + calcModel.WindLoadUpwind_puwl;
            float fload_CO3_ULS = fGamma_G_dest * fTotalDeadLoad_l + calcModel.WindLoadDownwind_pdwl;
            float fload_CO4_ULS = fGamma_G_dest * fTotalDeadLoad_l + calcModel.SnowLoad_sl;
            float fload_CO5_ULS = (fTotalDeadLoad_l - calcModel.AdditionalDeadLoad_gl) + fPsi_wind * calcModel.WindLoadUpwind_puwl;
            float fload_CO6_ULS = fTotalDeadLoad_l + fPsi_wind * calcModel.WindLoadDownwind_pdwl;

            float fload_up_ULS = MathF.Min(fload_CO1_ULS, fload_CO2_ULS, fload_CO3_ULS, fload_CO4_ULS, fload_CO5_ULS, fload_CO6_ULS);
            float fload_down_ULS = MathF.Max(fload_CO1_ULS, fload_CO2_ULS, fload_CO3_ULS, fload_CO4_ULS, fload_CO5_ULS, fload_CO6_ULS);

            float fload_CO1_SLS = fTotalDeadLoad_l + fPsi_liveload * calcModel.LiveLoad_ql;
            float fload_CO2_SLS = (fTotalDeadLoad_l - calcModel.AdditionalDeadLoad_gl) + calcModel.WindLoadUpwind_puwl;
            float fload_CO3_SLS = fTotalDeadLoad_l + calcModel.WindLoadDownwind_pdwl;
            float fload_CO4_SLS = fTotalDeadLoad_l + calcModel.SnowLoad_sl;

            float fload_up_SLS = MathF.Min(fload_CO1_SLS, fload_CO2_SLS, fload_CO3_SLS, fload_CO4_SLS);
            float fload_down_SLS = MathF.Max(fload_CO1_SLS, fload_CO2_SLS, fload_CO3_SLS, fload_CO4_SLS);

            // Convert kN/m to N/m
            float fLoadPerLength_UnitFactor = 1000f;

            fTotalDeadLoad_l *= fLoadPerLength_UnitFactor;

            fload_up_ULS *= fLoadPerLength_UnitFactor;
            fload_down_ULS *= fLoadPerLength_UnitFactor;

            fload_up_SLS *= fLoadPerLength_UnitFactor;
            fload_down_SLS *= fLoadPerLength_UnitFactor;

            // Simply supported beam
            calcModel.BendingMomentUpwind_M_asterix = 1f / 8f * fload_up_ULS * MathF.Pow2(calcModel.Length_L);
            calcModel.ShearForceUpwind_V_asterix =  1f / 2f * fload_up_ULS * calcModel.Length_L;
            calcModel.BendingMomentDownwind_M_asterix = 1f / 8f * fload_down_ULS * MathF.Pow2(calcModel.Length_L);
            calcModel.ShearForceDownwind_V_asterix = 1f / 2f * fload_down_ULS * calcModel.Length_L;

            int iNumberOfSectionsPerBeam = 11;

            float fx_step = 0.1f * calcModel.Length_L;

            calcModel.BendingCapacity_Mb = 0;
            calcModel.ShearCapacity_Vw = 0;
            calcModel.DesignRatioStrength_eta = 0;

            // Cycle per half of beam
            for(int i = 0; i < iNumberOfSectionsPerBeam / 2; i++)
            {
                float fx = i * fx_step;

                // Upwind
                float fBendingMomentUpwind_M_asterix_inLocation_x = calcModel.ShearForceUpwind_V_asterix * fx - fload_up_ULS * 0.5f * MathF.Pow2(fx);
                float ShearForceUpwind_V_asterix_inLocation_x = calcModel.ShearForceUpwind_V_asterix - fload_up_ULS * fx;

                designInternalForces sDIF_x_temp_upwind;

                SetZeroValues(out sDIF_x_temp_upwind);
                sDIF_x_temp_upwind.fV_zv = ShearForceUpwind_V_asterix_inLocation_x;
                sDIF_x_temp_upwind.fV_zz = ShearForceUpwind_V_asterix_inLocation_x;
                sDIF_x_temp_upwind.fM_yu = fBendingMomentUpwind_M_asterix_inLocation_x;
                sDIF_x_temp_upwind.fM_yy = fBendingMomentUpwind_M_asterix_inLocation_x;

                designMomentValuesForCb sMomentValuesForCb_upwind;
                sMomentValuesForCb_upwind.fM_14 = calcModel.ShearForceUpwind_V_asterix * (0.25f * calcModel.Length_L) - fload_up_ULS * 0.5f * MathF.Pow2(0.25f * calcModel.Length_L);
                sMomentValuesForCb_upwind.fM_24 = calcModel.ShearForceUpwind_V_asterix * (0.50f * calcModel.Length_L) - fload_up_ULS * 0.5f * MathF.Pow2(0.50f * calcModel.Length_L);
                sMomentValuesForCb_upwind.fM_34 = calcModel.ShearForceUpwind_V_asterix * (0.75f * calcModel.Length_L) - fload_up_ULS * 0.5f * MathF.Pow2(0.75f * calcModel.Length_L);
                sMomentValuesForCb_upwind.fM_max = calcModel.BendingMomentUpwind_M_asterix;

                CCalculMember cCalcULS_upwind = new CCalculMember(false, sDIF_x_temp_upwind, member, sMomentValuesForCb_upwind);

                float fRatio_M_upwind_inLocation_x = cCalcULS_upwind.fEta_7232_xu;
                float fRatio_V_upwind_inLocation_x = cCalcULS_upwind.fEta_7232_yv; // Combined bending and shear

                // Downwind
                float fBendingMomentDownwind_M_asterix_inLocation_x = calcModel.ShearForceDownwind_V_asterix * fx - fload_down_ULS * 0.5f * MathF.Pow2(fx);
                float ShearForceDownwind_V_asterix_inLocation_x = calcModel.ShearForceDownwind_V_asterix - fload_down_ULS * fx;

                designInternalForces sDIF_x_temp_downwind;

                SetZeroValues(out sDIF_x_temp_downwind);
                sDIF_x_temp_downwind.fV_zv = ShearForceDownwind_V_asterix_inLocation_x;
                sDIF_x_temp_downwind.fV_zz = ShearForceDownwind_V_asterix_inLocation_x;
                sDIF_x_temp_downwind.fM_yu = fBendingMomentDownwind_M_asterix_inLocation_x;
                sDIF_x_temp_downwind.fM_yy = fBendingMomentDownwind_M_asterix_inLocation_x;

                designMomentValuesForCb sMomentValuesForCb_downwind;
                sMomentValuesForCb_downwind.fM_14 = calcModel.ShearForceDownwind_V_asterix * (0.25f * calcModel.Length_L) - fload_down_ULS * 0.5f * MathF.Pow2(0.25f * calcModel.Length_L);
                sMomentValuesForCb_downwind.fM_24 = calcModel.ShearForceDownwind_V_asterix * (0.50f * calcModel.Length_L) - fload_down_ULS * 0.5f * MathF.Pow2(0.50f * calcModel.Length_L);
                sMomentValuesForCb_downwind.fM_34 = calcModel.ShearForceDownwind_V_asterix * (0.75f * calcModel.Length_L) - fload_down_ULS * 0.5f * MathF.Pow2(0.75f * calcModel.Length_L);
                sMomentValuesForCb_downwind.fM_max = calcModel.BendingMomentDownwind_M_asterix;

                CCalculMember cCalcULS_downwind = new CCalculMember(false, sDIF_x_temp_downwind, member, sMomentValuesForCb_downwind);

                float fRatio_M_downwind_inLocation_x = cCalcULS_downwind.fEta_7232_xu;
                float fRatio_V_downwind_inLocation_x = cCalcULS_downwind.fEta_7232_yv;

                // Interaction
                float fRatio_MandV_upwind_inLocation_x = Math.Max(fRatio_M_upwind_inLocation_x + fRatio_V_upwind_inLocation_x, cCalcULS_upwind.fEta_max);
                float fRatio_MandV_downwind_inLocation_x = Math.Max(fRatio_M_downwind_inLocation_x + fRatio_V_downwind_inLocation_x, cCalcULS_downwind.fEta_max);

                float fMaximumRatio_Strength = Math.Max(fRatio_MandV_upwind_inLocation_x , fRatio_MandV_downwind_inLocation_x);

                // Upwind - results
                if (fRatio_MandV_downwind_inLocation_x > calcModel.DesignRatioStrength_eta)
                {
                    calcModel.BendingCapacity_Ms = cCalcULS_upwind.fM_s_xu;
                    calcModel.BendingCapacity_Mb = cCalcULS_upwind.fM_b_xu;
                    calcModel.ShearCapacity_Vy = cCalcULS_upwind.fV_y_yv;
                    calcModel.ShearCapacity_Vw = cCalcULS_upwind.fV_v_yv;
                    calcModel.DesignRatioStrength_eta = fRatio_MandV_upwind_inLocation_x;
                }

                // Downwind results
                if (fRatio_MandV_downwind_inLocation_x > calcModel.DesignRatioStrength_eta)
                {
                    calcModel.BendingCapacity_Ms = cCalcULS_downwind.fM_s_xu;
                    calcModel.BendingCapacity_Mb = cCalcULS_downwind.fM_b_xu;
                    calcModel.ShearCapacity_Vy = cCalcULS_downwind.fV_y_yv;
                    calcModel.ShearCapacity_Vw = cCalcULS_downwind.fV_v_yv;
                    calcModel.DesignRatioStrength_eta = fRatio_MandV_downwind_inLocation_x;
                }
            }

            // Dead Load + imposed live load (long-term)
            float DeflectionGQUpwind_Delta = 0;
            float DeflectionGQDownwind_Delta = 5f / 384f * (fTotalDeadLoad_l + fPsi_liveload * calcModel.LiveLoad_ql * fLoadPerLength_UnitFactor) * MathF.Pow4(calcModel.Length_L) / (mat.m_fE * fI_x_eff);

            calcModel.DeflectionGQ_Delta = DeflectionGQDownwind_Delta;
            calcModel.DeflectionGQLimit_Delta_lim = calcModel.Length_L / calcModel.DeflectionGQLimitFraction;
            calcModel.DesignRatioDeflectionGQ_eta = Math.Abs(calcModel.DeflectionGQ_Delta) / calcModel.DeflectionGQLimit_Delta_lim;

            // Imposed load (snow + wind)
            calcModel.Deflection_W_Upwind_Delta = 5f / 384f * calcModel.WindLoadUpwind_puwl * fLoadPerLength_UnitFactor * MathF.Pow4(calcModel.Length_L) / (mat.m_fE * fI_x_eff);
            calcModel.Deflection_SW_Downwind_Delta = 5f / 384f * (fPsi_liveload * calcModel.LiveLoad_ql + calcModel.WindLoadDownwind_pdwl + calcModel.SnowLoad_sl) *fLoadPerLength_UnitFactor * MathF.Pow4(calcModel.Length_L) / (mat.m_fE * fI_x_eff);
            float fDeflection_SW_Maximum_Delta = Math.Max(Math.Abs(calcModel.Deflection_W_Upwind_Delta), Math.Abs(calcModel.Deflection_SW_Downwind_Delta));
            calcModel.DeflectionLimit_SW_Delta_lim = calcModel.Length_L / calcModel.DeflectionSWLimitFraction;
            calcModel.DesignRatio_SW_Deflection_eta = Math.Abs(fDeflection_SW_Maximum_Delta) / calcModel.DeflectionLimit_SW_Delta_lim;

            // Total (maximum) load
            calcModel.DeflectionTotalUpwind_Delta = 5f / 384f * fload_up_SLS * MathF.Pow4(calcModel.Length_L) / (mat.m_fE * fI_x_eff);
            calcModel.DeflectionTotalDownwind_Delta = 5f / 384f * fload_down_SLS * MathF.Pow4(calcModel.Length_L) / (mat.m_fE * fI_x_eff);
            calcModel.DeflectionTotalLimit_Delta_lim = calcModel.Length_L / calcModel.DeflectionTotalLimitFraction;
            float fDeflectionTotalMaximum_Delta = Math.Max(Math.Abs(calcModel.DeflectionTotalUpwind_Delta), Math.Abs(calcModel.DeflectionTotalDownwind_Delta));
            calcModel.DesignRatioDeflectionTotal_eta = Math.Abs(fDeflectionTotalMaximum_Delta / calcModel.DeflectionTotalLimit_Delta_lim);

            // ULS and SLS - Maximum
            float fMaximumDesignRatio = Math.Max(calcModel.DesignRatioStrength_eta, calcModel.DesignRatioDeflectionTotal_eta);
        }

        private void SetOutputValues()
        {
            PurlinDesignerViewModel vm = this.DataContext as PurlinDesignerViewModel;

            // Cross-section
            vm.Area_Ag *= 1e+6f;
            vm.MomentOfInertia_Ix *= 1e+12f;

            // Loads
            vm.PurlinSelfWeight_gp *= 0.001f;

            // Strength
            vm.BendingMomentUpwind_M_asterix *= 0.001f;
            vm.ShearForceUpwind_V_asterix *= 0.001f;
            vm.BendingMomentDownwind_M_asterix *= 0.001f;
            vm.ShearForceDownwind_V_asterix *= 0.001f;

            vm.BendingCapacity_Ms *= 0.001f;
            vm.BendingCapacity_Mb *= 0.001f;
            vm.ShearCapacity_Vy *= 0.001f;
            vm.ShearCapacity_Vw *= 0.001f;

            vm.DesignRatioStrength_eta *= 100;

            // Deflection
            vm.DeflectionGQ_Delta *= 1000f;
            vm.DeflectionGQLimit_Delta_lim *= 1000f;
            vm.DesignRatioDeflectionGQ_eta *= 100;

            vm.Deflection_W_Upwind_Delta *= 1000f;
            vm.Deflection_SW_Downwind_Delta *= 1000f;
            vm.DeflectionLimit_SW_Delta_lim *= 1000f;
            vm.DesignRatio_SW_Deflection_eta *= 100;

            vm.DeflectionTotalUpwind_Delta *= 1000f;
            vm.DeflectionTotalDownwind_Delta *= 1000f;
            vm.DeflectionTotalLimit_Delta_lim *= 1000f;
            vm.DesignRatioDeflectionTotal_eta *= 100;
        }

        public void TextBoxLostFocus()
        {
            this.Focus();
        }

        private void SetZeroValues(out designInternalForces sDIF)
        {
            sDIF.fN = 0;
            sDIF.fN_c = 0;
            sDIF.fN_t = 0;
            sDIF.fV_yu = 0;
            sDIF.fV_yy = 0;
            sDIF.fV_zv = 0;
            sDIF.fV_zz = 0;
            sDIF.fT = 0;
            sDIF.fM_yu = 0;
            sDIF.fM_yy = 0;
            sDIF.fM_zv = 0;
            sDIF.fM_zz = 0;
        }

        private void SetZeroValues(out designDeflections sDDeflections)
        {
            sDDeflections.fDelta_yu = 0;
            sDDeflections.fDelta_yy = 0;
            sDDeflections.fDelta_zv = 0;
            sDDeflections.fDelta_tot = 0;
            sDDeflections.fDelta_zz = 0;
        }
    }
}
