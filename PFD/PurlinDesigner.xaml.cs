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
using MATH;

namespace PFD
{
    /// <summary>
    /// Interaction logic for PurlinDesigner.xaml
    /// </summary>
    public partial class PurlinDesigner : Window
    {
        PurlinDesignerViewModel vm;

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

            vm = new PurlinDesignerViewModel();
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
            vm.TributaryArea_A = vm.Length_L * vm.TributaryWidth_B;

            float fE = 2e+11f;
            float fI_x = 0.001111f;
            float fI_x_eff = 0.000511f;


            vm.AdditionalDeadLoad_gl *=  vm.TributaryWidth_B;

            vm.CladdingSelfWeight_gcl *= vm.TributaryWidth_B;
            vm.AdditionalDeadLoad_gl *= vm.TributaryWidth_B;
            vm.LiveLoad_ql *= vm.TributaryWidth_B;
            vm.SnowLoad_sl *= vm.TributaryWidth_B;
            vm.InternalPressure_piminl *= vm.TributaryWidth_B;
            vm.InternalPressure_pimaxl *= vm.TributaryWidth_B;
            vm.ExternalPressure_peminl *= vm.TributaryWidth_B;
            vm.ExternalPressure_pemaxl *= vm.TributaryWidth_B;

            float fTotalDeadLoad_l = vm.CladdingSelfWeight_gcl + vm.AdditionalDeadLoad_gl + vm.CladdingSelfWeight_gcl + vm.AdditionalDeadLoad_gl;
            float fTotalWindLoad_upwind_l = vm.InternalPressure_pimaxl + vm.ExternalPressure_peminl;
            float fTotalWindLoad_downwind_l = Math.Abs(vm.InternalPressure_piminl) + vm.ExternalPressure_pemaxl;


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
            float fPsi_wind = 0.65f;

            float fload_CO1 = fGamma_G_dest * fTotalDeadLoad_l + fGamma_Q * vm.LiveLoad_ql;
            float fload_CO2 = fGamma_G_stab * (fTotalDeadLoad_l - vm.AdditionalDeadLoad_gl) + fTotalWindLoad_upwind_l;
            float fload_CO3 = fGamma_G_dest * fTotalDeadLoad_l + fTotalWindLoad_downwind_l;
            float fload_CO4 = fGamma_G_dest * fTotalDeadLoad_l + vm.SnowLoad_sl;
            float fload_CO5 = (fTotalDeadLoad_l - vm.AdditionalDeadLoad_gl) + fPsi_wind * fTotalWindLoad_upwind_l;
            float fload_CO6 = fTotalDeadLoad_l + fPsi_wind * fTotalWindLoad_downwind_l;

            float fload_up = MathF.Min(fload_CO1, fload_CO2, fload_CO3, fload_CO4, fload_CO5, fload_CO6);
            float fload_down = MathF.Min(fload_CO1, fload_CO2, fload_CO3, fload_CO4, fload_CO5, fload_CO6);

            // Simply supported beam
            vm.BendingMomentUpwind_M_asterix = 1f/8f * fload_up * MathF.Pow2(vm.Length_L);
            vm.ShearForceUpwind_V_asterix =  1f / 2f * fload_up * vm.Length_L;
            vm.BendingMomentDownwind_M_asterix = 1f / 8f * fload_down * MathF.Pow2(vm.Length_L);
            vm.ShearForceDownwind_V_asterix = 1f / 2f * fload_down * vm.Length_L;

            int iNumberOfSectionsPerBeam = 11;

            float fx_step = 0.1f * vm.Length_L;

            vm.BendingCapacity_Mb = 0.002f; // Nm napojit
            vm.ShearCapacity_Vw = 0.012f; // Nm napojit
            vm.BracingLength_Lb = 2f; // m napojit

            float fMaximumDesignRatio_Strength = 0;

            // Cycle per half of beam
            for(int i = 0; i < iNumberOfSectionsPerBeam / 2; i++)
            {
                float fx = i * fx_step;

                float fBendingMomentUpwind_M_asterix_inLocation_x = vm.ShearForceUpwind_V_asterix * fx - fload_up * 0.5f * MathF.Pow2(fx);
                float ShearForceUpwind_V_asterix_inLocation_x = vm.ShearForceUpwind_V_asterix - fload_up * fx;
                float fBendingMomentDownwind_M_asterix_inLocation_x = vm.ShearForceDownwind_V_asterix * fx - fload_down * 0.5f * MathF.Pow2(fx);
                float ShearForceDownwind_V_asterix_inLocation_x = vm.ShearForceDownwind_V_asterix - fload_down * fx;

                float fRatio_M_upwind_inLocation_x = Math.Abs(fBendingMomentUpwind_M_asterix_inLocation_x) / vm.BendingCapacity_Mb;
                float fRatio_V_upwind_inLocation_x = Math.Abs(ShearForceUpwind_V_asterix_inLocation_x) / vm.ShearCapacity_Vw;

                float fRatio_M_downwind_inLocation_x = Math.Abs(fBendingMomentDownwind_M_asterix_inLocation_x) / vm.BendingCapacity_Mb;
                float fRatio_V_downwind_inLocation_x = Math.Abs(ShearForceDownwind_V_asterix_inLocation_x) / vm.ShearCapacity_Vw;

                // Interaction
                float fRatio_MandV_upwind_inLocation_x = fRatio_M_upwind_inLocation_x + fRatio_V_upwind_inLocation_x;
                float fRatio_MandV_downwind_inLocation_x = fRatio_M_downwind_inLocation_x + fRatio_V_downwind_inLocation_x;

                float fMaximumRatio_Strength = Math.Max(fRatio_MandV_upwind_inLocation_x , fRatio_MandV_downwind_inLocation_x);

                if (fMaximumRatio_Strength > fMaximumDesignRatio_Strength)
                    fMaximumDesignRatio_Strength = fMaximumRatio_Strength;
            }

            vm.DeflectionUpwind_Delta = 5f / 384f * fload_up * MathF.Pow4(vm.Length_L) / (fE * fI_x_eff);
            vm.DeflectionDownwind_Delta = 5f / 384f * fload_down * MathF.Pow4(vm.Length_L) / (fE * fI_x_eff);

            float fLimitRatioDelta = 1f / 150f;
            float fLimitValueDelta = vm.Length_L * fLimitRatioDelta;

            float fRatioDeflectionUpwind = Math.Abs(vm.DeflectionUpwind_Delta / fLimitValueDelta);
            float fRatioDeflectionDownwind = Math.Abs(vm.DeflectionDownwind_Delta / fLimitValueDelta);

            float fMaximumDesignRatio_Deflection = Math.Max(fRatioDeflectionUpwind, fRatioDeflectionDownwind);

            float fMaximumDesignRatio = Math.Max(fMaximumDesignRatio_Strength, fMaximumDesignRatio_Deflection);
        }

        private void SetOutputValues()
        {
            PurlinDesignerViewModel vm = this.DataContext as PurlinDesignerViewModel;

            // Set results



        }
    }
}
