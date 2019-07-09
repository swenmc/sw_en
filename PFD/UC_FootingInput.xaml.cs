using BaseClasses;
using BaseClasses.Helpers;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_FootingInput.xaml
    /// </summary>
    public partial class UC_FootingInput : UserControl
    {
        DisplayOptions sDisplayOptions;
        //CPFDViewModel _pfdVM;
        CFootingInputVM vm;

        public UC_FootingInput(/*CPFDViewModel pfdVM*/)
        {
            InitializeComponent();

            //_pfdVM = pfdVM;
            //_pfdVM.PropertyChanged += _pfdVM_PropertyChanged;

            vm = new CFootingInputVM();
            vm.PropertyChanged += HandleFootingPadPropertyChangedEvent;
            this.DataContext = vm;

            // Set default GUI
            vm.FootingPadMemberTypeIndex = 1;
            vm.ConcreteGrade = "30";
            vm.ConcreteDensity = 2300f; // kg / m^3
            vm.ReinforcementGrade = "500E";

            vm.LongReinTop_x_No = "5";
            vm.LongReinTop_x_Phi = "12";
            //vm.LongReinTop_x_Phi = 0.012f; // bar diameter mm to m
            vm.LongReinTop_x_distance_s_y = 0.2035f; // m
            vm.LongReinTop_x_distance_s_y = GetDistanceBetweenReinforcementBars(vm.FootingPadSize_y_Or_b, Convert.ToInt32(vm.LongReinTop_x_No), (float)Convert.ToDouble(vm.LongReinTop_x_Phi) * 0.001f, vm.ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            vm.LongReinTop_y_No = "5";
            vm.LongReinTop_y_Phi = "12";
            //vm.LongReinTop_y_Phi = 0.012f; // bar diameter mm to m
            vm.LongReinTop_y_distance_s_x = 0.2035f; // m
            vm.LongReinTop_y_distance_s_x = GetDistanceBetweenReinforcementBars(vm.FootingPadSize_x_Or_a, Convert.ToInt32(vm.LongReinTop_y_No), (float)Convert.ToDouble(vm.LongReinTop_y_Phi) * 0.001f, vm.ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            vm.LongReinBottom_x_No = "5";
            vm.LongReinBottom_x_Phi = "12";
            //vm.LongReinBottom_x_Phi = 0.012f; // bar diameter mm to m
            vm.LongReinBottom_x_distance_s_y = 0.2035f; // m
            vm.LongReinBottom_x_distance_s_y = GetDistanceBetweenReinforcementBars(vm.FootingPadSize_y_Or_b, Convert.ToInt32(vm.LongReinBottom_x_No), (float)Convert.ToDouble(vm.LongReinBottom_x_Phi) * 0.001f, vm.ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            vm.LongReinBottom_y_No = "5";
            vm.LongReinBottom_y_Phi = "12";
            //vm.LongReinBottom_y_Phi = 0.012f; // bar diameter mm to m
            vm.LongReinBottom_y_distance_s_x = 0.2035f; // m
            vm.LongReinBottom_y_distance_s_x = GetDistanceBetweenReinforcementBars(vm.FootingPadSize_x_Or_a, Convert.ToInt32(vm.LongReinBottom_y_No), (float)Convert.ToDouble(vm.LongReinBottom_y_Phi) * 0.001f, vm.ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            // Dimensions of footing pad in meters
            vm.FootingPadSize_x_Or_a = 1.0f;
            vm.FootingPadSize_y_Or_b = 1.0f;
            vm.FootingPadSize_z_Or_h = 0.4f;

            vm.SoilReductionFactor_Phi = 0.5f;
            vm.SoilReductionFactorEQ_Phi = 0.8f;

            vm.SoilBearingCapacity = 200f; // kPa (konverovat kPa na Pa)

            // To Ondrej - doriesit zadavacie a vypoctove jednotky mm a m
            vm.ConcreteCover = 75; // mm  0.075f; m
            vm.FloorSlabThickness = 125; // mm 0.125f; m
        }

        /*
        private void _pfdVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is CPFDViewModel)) return;
            CConnectionJointTypes joint = GetSelectedJoint();
            displayJoint(joint);
        }
        */

        protected void HandleFootingPadPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CJointsVM vm = sender as CJointsVM;
            if (vm != null && vm.IsSetFromCode) return;
        }

        private float GetDistanceBetweenReinforcementBars(float footingPadWidth, int iNumberOfBarsPerSection, float fBarDiameter, float fConcreteCover)
        {
            return (footingPadWidth - 2 * fConcreteCover - 3 * fBarDiameter) / (iNumberOfBarsPerSection - 1);
        }
    }
}
