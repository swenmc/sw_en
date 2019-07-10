using BaseClasses;
using BaseClasses.Helpers;
using DATABASE.DTO;
using MATH;
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
        CPFDViewModel _pfdVM;
        CFootingInputVM vm;

        public UC_FootingInput(CPFDViewModel pfdVM/*, CJointsVM jointsVM*/)
        {
            InitializeComponent();

            _pfdVM = pfdVM;
            //_pfdVM.PropertyChanged += _pfdVM_PropertyChanged;

            vm = new CFootingInputVM();
            vm.PropertyChanged += HandleFootingPadPropertyChangedEvent;
            this.DataContext = vm;

            // Set default GUI
            vm.FootingPadMemberTypeIndex = 1;
            vm.ConcreteGrade = "30";
            vm.ConcreteDensity = 2300f; // kg / m^3
            vm.ReinforcementGrade = "500E";

            vm.SoilReductionFactor_Phi = 0.5f;
            vm.SoilReductionFactorEQ_Phi = 0.8f;

            vm.SoilBearingCapacity = 200f; // kPa (konverovat kPa na Pa)

            // To Ondrej - doriesit zadavacie a vypoctove jednotky mm a m
            vm.ConcreteCover = 75; // mm  0.075f; m
            vm.FloorSlabThickness = 125; // mm 0.125f; m

            UpdateValuesInGUI();
        }

        private void _pfdVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is CPFDViewModel)) return;
            CFoundation pad = GetSelectedFootingPad(); // TO DO Ondrej - dopracovat a napojit objekty pad a joint ako parametre funkcie
            //CConnectionJointTypes joint = GetSelectedJoint();
            CConnectionJointTypes joint = null;
            displayFootingPad(pad, joint, true);
        }

        protected void HandleFootingPadPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CFootingInputVM vm = sender as CFootingInputVM;
            if (vm != null && vm.IsSetFromCode) return;

            // TODO - dopracovat dalsie parametre, ktore ovplyvnia preview
            if (e.PropertyName == "FootingPadSize_x_Or_a" ||
                e.PropertyName == "FootingPadSize_y_Or_b" ||
                e.PropertyName == "FootingPadSize_z_Or_h"
                )
            {
                CFoundation pad = GetSelectedFootingPad();
                CConnectionJointTypes joint = null;
                displayFootingPad(pad, joint);
            }
        }

        private CFoundation GetSelectedFootingPad()
        {
            //CFoundation con = vm.JointTypes[vm.JointTypeIndex];
            //list_pads = jointsDict[con.ID];
            //all pads in list should be the same!
            //CFoundation pad = list_pads.FirstOrDefault();

            CFoundation pad = new CFoundation(); // zmazat
            return pad;
        }

        // TO Ondrej - neviem ci ma byt toho vo viewmodeli alebo UC_FootingInputxaml.cs
        // Jedna sa o prepocitanie hodnot a zobrazenie/update hodnot v GUI pri zmene niektorej hodnoty v GUI
        private void UpdateValuesInGUI()
        {
            // Dimensions of footing pad in meters
            //vm.FootingPadSize_x_Or_a = 1.0f;
            //vm.FootingPadSize_y_Or_b = 1.0f;
            //vm.FootingPadSize_z_Or_h = 0.4f;

            // Default size of footing pad
            float faX, fbY, fhZ;
            GetDefaultFootingPadSize(out faX, out fbY, out fhZ);

            vm.FootingPadSize_x_Or_a = faX;
            vm.FootingPadSize_y_Or_b = fbY;
            vm.FootingPadSize_z_Or_h = fhZ;

            // Default reinforcement

            //vm.LongReinTop_x_No = "5";
            vm.LongReinTop_x_No = GetDefaultNumberOfReiforcingBars(vm.FootingPadSize_y_Or_b, (float)Convert.ToDouble(vm.LongReinTop_x_Phi) * 0.001f, vm.ConcreteCover * 0.001f).ToString();
            vm.LongReinTop_x_Phi = "12";
            //vm.LongReinTop_x_Phi = 0.012f; // bar diameter mm to m
            //vm.LongReinTop_x_distance_s_y = 0.2035f; // m
            vm.LongReinTop_x_distance_s_y = GetDistanceBetweenReinforcementBars(vm.FootingPadSize_y_Or_b, Convert.ToInt32(vm.LongReinTop_x_No), (float)Convert.ToDouble(vm.LongReinTop_x_Phi) * 0.001f, vm.ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            //vm.LongReinTop_y_No = "5";
            vm.LongReinTop_y_No = GetDefaultNumberOfReiforcingBars(vm.FootingPadSize_x_Or_a, (float)Convert.ToDouble(vm.LongReinTop_y_Phi) * 0.001f, vm.ConcreteCover * 0.001f).ToString();
            vm.LongReinTop_y_Phi = "12";
            //vm.LongReinTop_y_Phi = 0.012f; // bar diameter mm to m
            //vm.LongReinTop_y_distance_s_x = 0.2035f; // m
            vm.LongReinTop_y_distance_s_x = GetDistanceBetweenReinforcementBars(vm.FootingPadSize_x_Or_a, Convert.ToInt32(vm.LongReinTop_y_No), (float)Convert.ToDouble(vm.LongReinTop_y_Phi) * 0.001f, vm.ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            //vm.LongReinBottom_x_No = "5";
            vm.LongReinBottom_x_No = GetDefaultNumberOfReiforcingBars(vm.FootingPadSize_y_Or_b, (float)Convert.ToDouble(vm.LongReinBottom_x_Phi) * 0.001f, vm.ConcreteCover * 0.001f).ToString();
            vm.LongReinBottom_x_Phi = "12";
            //vm.LongReinBottom_x_Phi = 0.012f; // bar diameter mm to m
            //vm.LongReinBottom_x_distance_s_y = 0.2035f; // m
            vm.LongReinBottom_x_distance_s_y = GetDistanceBetweenReinforcementBars(vm.FootingPadSize_y_Or_b, Convert.ToInt32(vm.LongReinBottom_x_No), (float)Convert.ToDouble(vm.LongReinBottom_x_Phi) * 0.001f, vm.ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)

            //vm.LongReinBottom_y_No = "5";
            vm.LongReinBottom_y_No = GetDefaultNumberOfReiforcingBars(vm.FootingPadSize_x_Or_a, (float)Convert.ToDouble(vm.LongReinBottom_y_Phi) * 0.001f, vm.ConcreteCover * 0.001f).ToString();
            vm.LongReinBottom_y_Phi = "12";
            //vm.LongReinBottom_y_Phi = 0.012f; // bar diameter mm to m
            //vm.LongReinBottom_y_distance_s_x = 0.2035f; // m
            vm.LongReinBottom_y_distance_s_x = GetDistanceBetweenReinforcementBars(vm.FootingPadSize_x_Or_a, Convert.ToInt32(vm.LongReinBottom_y_No), (float)Convert.ToDouble(vm.LongReinBottom_y_Phi) * 0.001f, vm.ConcreteCover * 0.001f); // Concrete Cover factor - mm to m (docasne faktor pre konverziu, TODO odstranit a nastavit concrete cover na metre)
        }

        private float GetDistanceBetweenReinforcementBars(float footingPadWidth, int iNumberOfBarsPerSection, float fBarDiameter, float fConcreteCover)
        {
            return (footingPadWidth - 2 * fConcreteCover - 3 * fBarDiameter) / (iNumberOfBarsPerSection - 1);
        }

        private void GetDefaultFootingPadSize(out float faX, out float fbY, out float fhZ)
        {
            if (vm.FootingPadMemberTypeIndex <= 1)
            {
                // Main or edge frame column (0 and 1)
                faX = (float)Math.Round(MathF.Max(0.6f, Math.Min(_pfdVM.GableWidth * 0.08f, _pfdVM.fL1 * 0.40f)), 1);
                fbY = (float)Math.Round(MathF.Max(0.6f, Math.Min(_pfdVM.GableWidth * 0.07f, _pfdVM.fL1 * 0.35f)), 1);
                fhZ = 0.4f;
            }
            else // Front a back side wind posts (2 and 3)
            {
                float fDist_Column;

                // Pripravene pre rozne rozostupy wind post na prednej a zadnej strane budovy
                if (vm.FootingPadMemberTypeIndex == 2) // Front Side
                    fDist_Column = _pfdVM.ColumnDistance;
                else // Back Side
                    fDist_Column = _pfdVM.ColumnDistance;

                // Front or back side - wind posts
                faX = (float)Math.Round(MathF.Max(0.5f, fDist_Column * 0.40f), 1);
                fbY = (float)Math.Round(MathF.Max(0.5f, fDist_Column * 0.40f), 1);
                fhZ = 0.4f;
            }
        }

        private int GetDefaultNumberOfReiforcingBars(float footingPadWidth, float fBarDiameter, float fConcreteCover)
        {
            float fDefaultDistanceBetweenReinforcementBars = 0.15f; // 150 mm

            // Number of spacings + 1
            return (int)((footingPadWidth - 2 * fConcreteCover - 3 * fBarDiameter) / fDefaultDistanceBetweenReinforcementBars) + 1;
        }

        private void FrameFootingPadPreview3D_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void displayFootingPad(CFoundation pad, CConnectionJointTypes joint, bool bDisplayJointComponents = true)
        {
            // TO Ondrej - tu potrebujeme refaktorovat cast funkcie display Joint
            // Mala by sa vykreslit patka (betonovy kvader prisluchajuci k uzlu) a v pripade ze je bool bDisplayJointComponents == true tak aj plech a cast pruta ktore su v danom uzle k zakladu pripojene
            // Joint je samozrejme potrebne naklonovat
            // Asi aj patku je potrebne naklonovat, nech to funguje rovnako
            // Vykreslenie vyztuze podla projektu AAC

            if (pad == null) return; // Error - nothing to display

            sDisplayOptions = _pfdVM.GetDisplayOptions();
            //Here is the place to overwrite displayOptions from Main Model
            sDisplayOptions.bDisplayGlobalAxis = true;
            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayPlates = true;
            sDisplayOptions.bDisplayConnectors = true;
            sDisplayOptions.bDisplayJoints = true;

            sDisplayOptions.bDisplayFoundations = true; // Display always footing pads
            sDisplayOptions.bDisplayReinforcement = true;

            CModel padModel = new CModel(); // TODO - Vyrobit model patky + vyztuz + plech spoja a cast pruta spoja

            Page3Dmodel page1 = new Page3Dmodel(padModel, sDisplayOptions);

            // Display model in 3D preview frame
            FrameFootingPadPreview3D.Content = page1;
            FrameFootingPadPreview3D.UpdateLayout();
        }
    }
}