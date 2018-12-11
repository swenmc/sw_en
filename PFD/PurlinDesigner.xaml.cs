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
        CCrSc_TW cs;
        MATERIAL.CMat_03_00 mat;
        CCalculMember cCalcULS_data;

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

            // Fill material combobox items
            CComboBoxHelper.FillComboboxValues("MaterialsSQLiteDB", "materialSteelAS4600", "Grade", Combobox_Material);

            PurlinDesignerViewModel vm = new PurlinDesignerViewModel();
            vm.PropertyChanged += HandleComponentViewerPropertyChangedEvent;
            this.DataContext = vm;

            // Default object of material and cross-section
            //cs = new CCrSc_3_270XX_C(0, 0.27f, 0.07f, 0.00095f, Colors.Aquamarine);
            mat = new MATERIAL.CMat_03_00();

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
                //recalculate only if input property is changed (prevent cycling)
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
                "MaterialIndex",
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

            CMaterialManager.LoadMaterialProperties(mat, (string)Combobox_Material.Items[calcModel.MaterialIndex]);
            mat.m_fE = 2e+11f; // Change default value of E (see AS 4600)
            mat.m_fG = 08e+10f;
            mat.m_fNu = 0.25f;

            FillCrossSectionData((string)Combobox_CrossSection.Items[calcModel.CrossSectionIndex]);

            cs.m_Mat = mat; // Set material from GUI to the cross-section

            calcModel.YieldStrength_fy = mat.Get_f_yk_by_thickness((float)cs.m_t_min);
            calcModel.TensileStrength_fu = mat.Get_f_uk_by_thickness((float)cs.m_t_min);

            CMember member = new CMember(0, new CNode(0, 0, 0, 0), new CNode(1, calcModel.Length_L, 0, 0), cs, 0);

            calcModel.Area_Ag = (float)cs.A_g;
            calcModel.MomentOfInertia_Ix = (float)cs.I_y; // (Ix = Iy a Iy = Iz podla AS 4600)
            float fI_x_eff = 0.9f * calcModel.MomentOfInertia_Ix; // TODO - doplnit aj Ieff, zistit ci sa da pre simply supported beam vyjadrit faktorom

            float fMemberMassPerLength = calcModel.Area_Ag * mat.m_fRho;
            calcModel.PurlinSelfWeight_gp = fMemberMassPerLength * GlobalConstants.fg_acceleration;

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

            designBucklingLengthFactors sBucklingLengthFactors;
            sBucklingLengthFactors.fBeta_x_FB_fl_ex = 1f;
            sBucklingLengthFactors.fBeta_y_FB_fl_ey =  calcModel.BracingLength_Lb / member.FLength;
            sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez = calcModel.BracingLength_Lb / member.FLength;
            sBucklingLengthFactors.fBeta_LTB_fl_LTB = calcModel.BracingLength_Lb / member.FLength;

            int iNumberOfSectionsPerBeam = 11;

            float fx_step = 0.1f * calcModel.Length_L;

            calcModel.BendingCapacity_Mb = 0;
            calcModel.ShearCapacity_Vw = 0;
            calcModel.DesignRatioStrength_eta = 0;
            float fMaximumRatio_Strength = 0;

            // Cycle per half of beam
            for (int i = 0; i < (iNumberOfSectionsPerBeam / 2) + 1; i++)
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

                CCalculMember cCalcULS_upwind = new CCalculMember(false, sDIF_x_temp_upwind, member, sBucklingLengthFactors, sMomentValuesForCb_upwind);

                float fRatio_M_upwind_inLocation_x = cCalcULS_upwind.fEta_722_M_xu; // Lateral-torsional bending
                float fRatio_V_upwind_inLocation_x = cCalcULS_upwind.fEta_723_11_V_yv; // Combined bending and shear

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

                CCalculMember cCalcULS_downwind = new CCalculMember(false, sDIF_x_temp_downwind, member, sBucklingLengthFactors, sMomentValuesForCb_downwind);

                float fRatio_M_downwind_inLocation_x = cCalcULS_downwind.fEta_722_M_xu; // Lateral-torsional bending
                float fRatio_V_downwind_inLocation_x = cCalcULS_downwind.fEta_723_11_V_yv; // Combined bending and shear

                // Maximum
                float fRatio_Max_upwind_inLocation_x = cCalcULS_upwind.fEta_max;
                float fRatio_Max_downwind_inLocation_x = cCalcULS_downwind.fEta_max;

                // Upwind - results
                if (fRatio_Max_upwind_inLocation_x > fMaximumRatio_Strength)
                {
                    cCalcULS_data = cCalcULS_upwind;
                    calcModel.BendingCapacity_Ms = cCalcULS_upwind.fM_s_xu;
                    calcModel.ElasticBucklingMoment_Mo = cCalcULS_upwind.fM_o_xu;
                    calcModel.NominalMemberCapacity_Mbe = cCalcULS_upwind.fM_be_xu;
                    calcModel.ElasticBucklingMoment_Mol = cCalcULS_upwind.fM_ol_xu;
                    calcModel.NominalMemberCapacity_Mbl = cCalcULS_upwind.fM_bl_xu;
                    calcModel.ElasticBucklingMoment_Mod = cCalcULS_upwind.fM_od_xu;
                    calcModel.NominalMemberCapacity_Mbd = cCalcULS_upwind.fM_bd_xu;
                    calcModel.BendingCapacity_Mb = cCalcULS_upwind.fM_b_xu;
                    calcModel.ShearCapacity_Vy = cCalcULS_upwind.fV_y_yv;
                    calcModel.ShearCapacity_Vw = cCalcULS_upwind.fV_v_yv;
                    calcModel.DesignRatioStrength_eta = fRatio_Max_upwind_inLocation_x;

                    fMaximumRatio_Strength = fRatio_Max_upwind_inLocation_x;
                }

                // Downwind results
                if (fRatio_Max_downwind_inLocation_x > fMaximumRatio_Strength)
                {
                    cCalcULS_data = cCalcULS_downwind;
                    calcModel.BendingCapacity_Ms = cCalcULS_downwind.fM_s_xu;
                    calcModel.ElasticBucklingMoment_Mo = cCalcULS_downwind.fM_o_xu;
                    calcModel.NominalMemberCapacity_Mbe = cCalcULS_upwind.fM_be_xu;
                    calcModel.ElasticBucklingMoment_Mol = cCalcULS_downwind.fM_ol_xu;
                    calcModel.NominalMemberCapacity_Mbl = cCalcULS_downwind.fM_bl_xu;
                    calcModel.ElasticBucklingMoment_Mod = cCalcULS_downwind.fM_od_xu;
                    calcModel.NominalMemberCapacity_Mbd = cCalcULS_downwind.fM_bd_xu;
                    calcModel.BendingCapacity_Mb = cCalcULS_downwind.fM_b_xu;
                    calcModel.ShearCapacity_Vy = cCalcULS_downwind.fV_y_yv;
                    calcModel.ShearCapacity_Vw = cCalcULS_downwind.fV_v_yv;
                    calcModel.DesignRatioStrength_eta = fRatio_Max_downwind_inLocation_x;

                    fMaximumRatio_Strength = fRatio_Max_downwind_inLocation_x;
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

            // Material
            vm.YieldStrength_fy *= 0.000001f;
            vm.TensileStrength_fu *= 0.000001f;

            // Loads
            vm.PurlinSelfWeight_gp *= 0.001f;

            // Strength
            vm.BendingMomentUpwind_M_asterix *= 0.001f;
            vm.ShearForceUpwind_V_asterix *= 0.001f;
            vm.BendingMomentDownwind_M_asterix *= 0.001f;
            vm.ShearForceDownwind_V_asterix *= 0.001f;

            vm.BendingCapacity_Ms *= 0.001f;

            vm.ElasticBucklingMoment_Mo *= 0.001f;
            vm.NominalMemberCapacity_Mbe *= 0.001f;
            vm.ElasticBucklingMoment_Mol *= 0.001f;
            vm.NominalMemberCapacity_Mbl *= 0.001f;
            vm.ElasticBucklingMoment_Mod *= 0.001f;
            vm.NominalMemberCapacity_Mbd *= 0.001f;

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

            // Set background colors

            // TODO - doplnit i pre dalsie textboxy
            if (vm.DesignRatioStrength_eta > 100)
                TextBox_DesignRatioStrength_eta.Background = Brushes.Red;
            else
                TextBox_DesignRatioStrength_eta.Background = Brushes.LightGreen;

            if (vm.DesignRatioDeflectionTotal_eta > 100)
                TextBox_DesignRatioDeflectionTotal_eta.Background = Brushes.Red;
            else
                TextBox_DesignRatioDeflectionTotal_eta.Background = Brushes.LightGreen;
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

        private void FillCrossSectionData(string sSectionNameDatabase)
        {
            Color cComponentColor = Colors.Aquamarine;

            /*
            10075
            27055
            27095
            27095n
            270115
            270115btb
            270115n
            50020
            50020n
            63020
            63020s1
            63020s2
            */

            if (sSectionNameDatabase == "10075")
            {
                cs = new CCrSc_3_10075_BOX(0, 01f, 0.1f, 0.00075f, cComponentColor); // BOX
            }
            else if (sSectionNameDatabase == "27055")
            {
                cs = new CCrSc_3_270XX_C(0, 0.27f, 0.07f, 0.00055f, cComponentColor);
            }
            else if (sSectionNameDatabase == "27095")
            {
                cs = new CCrSc_3_270XX_C(0, 0.27f, 0.07f, 0.00095f, cComponentColor);
            }
            else if (sSectionNameDatabase == "27095n")
            {
                cs = new CCrSc_3_270XX_C_NESTED(0, 0.29f, 0.071f, 0.00095f, cComponentColor);
            }
            else if (sSectionNameDatabase == "270115")
            {
                cs = new CCrSc_3_270XX_C(0, 0.27f, 0.07f, 0.00115f, cComponentColor);
            }
            else if (sSectionNameDatabase == "270115btb")
            {
                cs = new CCrSc_3_270XX_C_BACK_TO_BACK(0, 0.29f, 0.14f, 0.020f, 0.00115f, cComponentColor);
            }
            else if (sSectionNameDatabase == "270115n")
            {
                cs = new CCrSc_3_270XX_C_NESTED(0, 0.29f, 0.071f, 0.00115f, cComponentColor);
            }
            else if (sSectionNameDatabase == "50020")
            {
                cs = new CCrSc_3_50020_C(0, 0.5f, 0.01f, 0.00195f, cComponentColor);
            }
            else if (sSectionNameDatabase == "50020n")
            {
                cs = new CCrSc_3_50020_C_NESTED(0, 0.5f, 0.01f, 0.00195f, cComponentColor);
            }
            else if (sSectionNameDatabase == "63020")
            {
                cs = new CCrSc_3_63020_BOX(0, 0.63f, 0.18f, 0.00195f, 0.00195f, cComponentColor);
            }
            else if (sSectionNameDatabase == "63020s1")
            {
                cs = new CCrSc_3_63020_BOX(0, 0.63f, 0.18f, 0.00195f, 0.00390f, cComponentColor);
            }
            else if (sSectionNameDatabase == "63020s2")
            {
                cs = new CCrSc_3_63020_BOX(0, 0.63f, 0.18f, 0.00195f, 0.00858f, cComponentColor);
            }
            else
            {
                cs = null;
                throw new NotImplementedException("Invalid cross-section name: " + sSectionNameDatabase + ". \n" +
                                                  "Cross-section with this name is not implemented");
            }

            if(cs != null)
                CSectionManager.LoadCrossSectionProperties_meters(cs, sSectionNameDatabase);
        }

        private void WindPressureCalculator_Click(object sender, RoutedEventArgs e)
        {
            WindPressureCalculator window_windpressure = new WindPressureCalculator();
            window_windpressure.Show();
            // Reakcia na zavretie okna - nastavit tieto hodnoty do VM purlin designer
            window_windpressure.Closing += WindPressureCalculator_Closing;
        }

        private void WindPressureCalculator_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindPressureCalculator window_windpressure = sender as WindPressureCalculator;
            WindPressureCalculatorViewModel vm_WindPressure = window_windpressure.DataContext as WindPressureCalculatorViewModel;

            PurlinDesignerViewModel vm_CalcModel = this.DataContext as PurlinDesignerViewModel;
            vm_CalcModel.WindLoadInternalPressure_pimin = vm_WindPressure.WindPressure_pimin * 0.001f; // Convert from Pa to kPa
            vm_CalcModel.WindLoadInternalPressure_pimax = vm_WindPressure.WindPressure_pimax * 0.001f;
            vm_CalcModel.WindLoadExternalPressure_pemin = vm_WindPressure.WindPressure_pemin * 0.001f;
            vm_CalcModel.WindLoadExternalPressure_pemax = vm_WindPressure.WindPressure_pemin * 0.001f;
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            MemberDesignDetails win = new MemberDesignDetails();
            cCalcULS_data.DisplayDesignResultsInGridView(ELSType.eLS_ULS, win.Results_GridView);
            win.Show();
        }
    }
}
