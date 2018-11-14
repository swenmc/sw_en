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

            Combobox_CrossSection.Items.Add("C270095");
            Combobox_CrossSection.Items.Add("C270115");
            Combobox_CrossSection.Items.Add("C270155");
            Combobox_CrossSection.Items.Add("C270195");
            Combobox_CrossSection.Items.Add("C270095n");
            Combobox_CrossSection.Items.Add("C270115n");
            Combobox_CrossSection.Items.Add("C270155n");
            Combobox_CrossSection.Items.Add("C270195n");

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
                "TributaryWidth_B",
                "CrossSectionIndex",
                "CladdingSelfWeight_gc",
                "AdditionalDeadLoad_g",
                "LiveLoad_q",
                "SnowLoad_s",

                "WindLoadInternalPressure_pimin",
                "WindLoadInternalPressure_pimax",
                "WindLoadExternalPressure_pemin",
                "WindLoadExternalPressure_pemax"
            };

            return list.Contains(propName);
        }

        void SetInputAndCalculate()
        {
            PurlinDesignerViewModel calcModel = this.DataContext as PurlinDesignerViewModel;

            calcModel.TributaryArea_A = calcModel.Length_L * calcModel.TributaryWidth_B;

            float fE = 2e+11f;
            float fI_x = MomentOfInertia[calcModel.CrossSectionIndex] / 1e+12f;
            float fI_x_eff = 0.8f * fI_x; // TODO - doplnit aj Ieff, zistit ci sa da pre simlpy supported beam vyjadrit faktorom

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
            fload_up_ULS *= 1000;
            fload_down_ULS *= 1000;

            fload_up_SLS *= 1000;
            fload_down_SLS *= 1000;

            // Simply supported beam
            calcModel.BendingMomentUpwind_M_asterix = 1f/8f * fload_up_ULS * MathF.Pow2(calcModel.Length_L);
            calcModel.ShearForceUpwind_V_asterix =  1f / 2f * fload_up_ULS * calcModel.Length_L;
            calcModel.BendingMomentDownwind_M_asterix = 1f / 8f * fload_down_ULS * MathF.Pow2(calcModel.Length_L);
            calcModel.ShearForceDownwind_V_asterix = 1f / 2f * fload_down_ULS * calcModel.Length_L;

            int iNumberOfSectionsPerBeam = 11;

            float fx_step = 0.1f * calcModel.Length_L;

            calcModel.BendingCapacity_Mb = 0.002f; // Nm napojit
            calcModel.ShearCapacity_Vw = 0.012f; // Nm napojit
            calcModel.BracingLength_Lb = 2f; // m napojit

            float fMaximumDesignRatio_Strength = 0;

            // Cycle per half of beam
            for(int i = 0; i < iNumberOfSectionsPerBeam / 2; i++)
            {
                float fx = i * fx_step;

                float fBendingMomentUpwind_M_asterix_inLocation_x = calcModel.ShearForceUpwind_V_asterix * fx - fload_up_ULS * 0.5f * MathF.Pow2(fx);
                float ShearForceUpwind_V_asterix_inLocation_x = calcModel.ShearForceUpwind_V_asterix - fload_up_ULS * fx;
                float fBendingMomentDownwind_M_asterix_inLocation_x = calcModel.ShearForceDownwind_V_asterix * fx - fload_down_ULS * 0.5f * MathF.Pow2(fx);
                float ShearForceDownwind_V_asterix_inLocation_x = calcModel.ShearForceDownwind_V_asterix - fload_down_ULS * fx;

                float fRatio_M_upwind_inLocation_x = Math.Abs(fBendingMomentUpwind_M_asterix_inLocation_x) / calcModel.BendingCapacity_Mb;
                float fRatio_V_upwind_inLocation_x = Math.Abs(ShearForceUpwind_V_asterix_inLocation_x) / calcModel.ShearCapacity_Vw;

                float fRatio_M_downwind_inLocation_x = Math.Abs(fBendingMomentDownwind_M_asterix_inLocation_x) / calcModel.BendingCapacity_Mb;
                float fRatio_V_downwind_inLocation_x = Math.Abs(ShearForceDownwind_V_asterix_inLocation_x) / calcModel.ShearCapacity_Vw;

                // Interaction
                float fRatio_MandV_upwind_inLocation_x = fRatio_M_upwind_inLocation_x + fRatio_V_upwind_inLocation_x;
                float fRatio_MandV_downwind_inLocation_x = fRatio_M_downwind_inLocation_x + fRatio_V_downwind_inLocation_x;

                float fMaximumRatio_Strength = Math.Max(fRatio_MandV_upwind_inLocation_x , fRatio_MandV_downwind_inLocation_x);

                if (fMaximumRatio_Strength > fMaximumDesignRatio_Strength)
                    fMaximumDesignRatio_Strength = fMaximumRatio_Strength;
            }

            calcModel.DeflectionUpwind_Delta = 5f / 384f * fload_up_SLS * MathF.Pow4(calcModel.Length_L) / (fE * fI_x_eff);
            calcModel.DeflectionDownwind_Delta = 5f / 384f * fload_down_SLS * MathF.Pow4(calcModel.Length_L) / (fE * fI_x_eff);

            float fLimitRatioDelta = 1f / 150f;
            calcModel.DeflectionLimit_Delta_lim = calcModel.Length_L * fLimitRatioDelta;

            float fRatioDeflectionUpwind = Math.Abs(calcModel.DeflectionUpwind_Delta / calcModel.DeflectionLimit_Delta_lim);
            float fRatioDeflectionDownwind = Math.Abs(calcModel.DeflectionDownwind_Delta / calcModel.DeflectionLimit_Delta_lim);

            calcModel.DesignRatioDeflection_eta = Math.Max(fRatioDeflectionUpwind, fRatioDeflectionDownwind);

            float fMaximumDesignRatio = Math.Max(fMaximumDesignRatio_Strength, calcModel.DesignRatioDeflection_eta);
        }

        private void SetOutputValues()
        {
            PurlinDesignerViewModel vm = this.DataContext as PurlinDesignerViewModel;

            vm.BendingMomentUpwind_M_asterix *= 0.001f;
            vm.ShearForceUpwind_V_asterix *= 0.001f;
            vm.BendingMomentDownwind_M_asterix *= 0.001f;
            vm.ShearForceDownwind_V_asterix *= 0.001f;


            vm.DeflectionUpwind_Delta *= 1000f;
            vm.DeflectionDownwind_Delta *= 1000f;

            vm.DeflectionLimit_Delta_lim *= 1000f;
            vm.DesignRatioDeflection_eta *= 100;
        }

        public void TextBoxLostFocus()
        {
            this.Focus();
        }

        float[] MomentOfInertia = new float[8]
        {
            4513000f,
            5432000f,
            7446751f,
            9351163f,

           09237100f,
           11000000f,
           13097830f,
           18873060f
        };
    }
}
