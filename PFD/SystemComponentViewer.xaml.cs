using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//using System.Windows.Shapes;
using BaseClasses;
using BaseClasses.GraphObj;
using MATH;
using DATABASE;
using DATABASE.DTO;
using CRSC;
using EXPIMP;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Text;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Threading;
using System.Configuration;

namespace PFD
{
    /// <summary>
    /// Interaction logic for System_Component_Viewer.xaml
    /// </summary>
    public partial class SystemComponentViewer : Window
    {
        public CDatabaseComponents dcomponents; // Todo nahradit databazov component
        public Canvas page2D;
        double Frame2DWidth;
        double Frame2DHeight;
        public Page3Dmodel page3D;
        public DisplayOptions sDisplayOptions = new DisplayOptions();

        CCrSc_TW crsc;
        public CPlate plate;
        CScrew screw;
        Point3D controlpoint = new Point3D(0, 0, 0);
        System.Windows.Media.Color cComponentColor = Colors.Aquamarine; // Default
        float fb_R; // Rafter Width
        float fb_B; // Wind Post Width
        float fb; // in plane XY -X coord
        float fb2; // in plane XY - X coord
        float fh; // in plane XY - Y coord
        float fh2; // in plane XY - Y coord
        float fl; // out of plane - Z coord
        float fl2; // out of plane - Z coord
        float ft;
        int iNumberOfStiffeners = 0;
        float fb_fl; // Flange - Z-section
        float fc_lip1; // LIP - Z-section
        float fRoofPitch_rad;
        float fGamma_rad; // Plate M alebo N uhol medzi hranou prierezu a vonkajsou hranou plechu
        int iNumberofHoles = 0;

        string sGauge_Screw;

        double WindowHeight;
        double WindowWidth;

        public SystemComponentViewer()
        {
            sDisplayOptions.bDisplayMembers = true;
            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayWireFrameModel = true;

            InitializeComponent();

            this.SizeChanged += OnWindowSizeChanged;
            //this.RaiseEvent(new RoutedEventArgs(Window.SizeChangedEvent));

            dcomponents = new CDatabaseComponents();

            SystemComponentViewerViewModel vm = new SystemComponentViewerViewModel(dcomponents);
            vm.PropertyChanged += HandleComponentViewerPropertyChangedEvent;
            this.DataContext = vm;
            vm.ComponentIndex = 1;
        }

        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            WindowHeight = e.NewSize.Height;
            WindowWidth = e.NewSize.Width;
            double prevWindowHeight = e.PreviousSize.Height;
            double prevWindowWidth = e.PreviousSize.Width;

            if (MainTabControl.SelectedIndex == 0) // 2D View TabItem
            {
                //// Bug No 96 - prekreslit plech - TODO - Ondrej - ma to tu byt ??
                //// Ak je okno defaultne a som 2D, prepnem na 3D, maximalizujem okno a prepnem na 2D tak sa sem nacitaju hodnoty z defaultnej velkosti, nie z maximalizovanej
                //Frame2DWidth = Frame2D.ActualWidth;
                //Frame2DHeight = Frame2D.ActualHeight;
                //RedrawComponentIn2D();

                SystemComponentViewerViewModel vm = sender as SystemComponentViewerViewModel;
                DisplayComponent(vm);
            }
        }

        private void HandleComponentViewerPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is SystemComponentViewerViewModel)
            {
                SystemComponentViewerViewModel vm = sender as SystemComponentViewerViewModel;
                if (vm != null && vm.IsSetFromCode) return;

                if (e.PropertyName == "ComponentIndex") { vm.DrillingRoutePoints = null; UpdateAll(); SetUIElementsVisibility(vm); this.Title = "System Component Viewer"; }

                if (e.PropertyName == "DrawPoints2D" ||
                    e.PropertyName == "DrawOutLine2D" ||
                    e.PropertyName == "DrawPointNumbers2D" ||
                    e.PropertyName == "DrawHoles2D" ||
                    e.PropertyName == "DrawHoleCentreSymbol2D" ||
                    e.PropertyName == "DrawDrillingRoute2D" ||
                    e.PropertyName == "DrawDimensions2D" ||
                    e.PropertyName == "DrawMemberOutline2D" ||
                    e.PropertyName == "DrawBendLines2D" ||
                    e.PropertyName == "DrawScrews3D")
                {
                    DisplayComponent(vm);
                }

                if (e.PropertyName == "MirrorX")
                {
                    vm.DrillingRoutePoints = null;
                    tabItemDoc.Visibility = Visibility.Hidden;
                    BtnShowCNCDrillingFile.IsEnabled = false;

                    MirrorComponentX(vm);
                    DisplayComponent(vm);
                }
                if (e.PropertyName == "MirrorY")
                {
                    vm.DrillingRoutePoints = null;
                    tabItemDoc.Visibility = Visibility.Hidden;
                    BtnShowCNCDrillingFile.IsEnabled = false;

                    MirrorComponentY(vm);
                    DisplayComponent(vm);
                }

                if (e.PropertyName == "Rotate90CW")
                {
                    vm.DrillingRoutePoints = null;
                    tabItemDoc.Visibility = Visibility.Hidden;
                    BtnShowCNCDrillingFile.IsEnabled = false;
                    if (vm.Rotate90CW == true && vm.Rotate90CCW == true) vm.Rotate90CCW = false;

                    if(vm.Rotate90CW) RotateComponent90CW(vm);
                    else RotateComponent90CCW(vm);
                    DisplayComponent(vm);
                }
                if (e.PropertyName == "Rotate90CCW")
                {
                    vm.DrillingRoutePoints = null;
                    tabItemDoc.Visibility = Visibility.Hidden;
                    BtnShowCNCDrillingFile.IsEnabled = false;
                    if (vm.Rotate90CW == true && vm.Rotate90CCW == true) vm.Rotate90CW = false;

                    if (vm.Rotate90CCW) RotateComponent90CCW(vm);
                    else RotateComponent90CW(vm);
                    
                    DisplayComponent(vm);
                }

                if (e.PropertyName == "ScrewArrangementIndex")
                {
                    SetUIElementsVisibilityForScrewArrangement(vm);
                    vm.DrillingRoutePoints = null;

                    ScrewArrangementChanged();
                    UpdateAndDisplayPlate();
                }
            }
            else if (sender is CComponentParamsViewBool)
            {
                CComponentParamsViewBool cpw = sender as CComponentParamsViewBool;
                if (e.PropertyName == "Value")
                {
                    DataGridScrewArrangement_ValueChanged(cpw);
                }
            }
            else if (sender is CComponentParamsViewString)
            {
                CComponentParamsViewString cpw = sender as CComponentParamsViewString;
                if (e.PropertyName == "Value")
                {
                    DataGridScrewArrangement_ValueChanged(cpw);
                }
            }
            else if (sender is CComponentParamsViewList)
            {
                CComponentParamsViewList cpw = sender as CComponentParamsViewList;
                DataGridScrewArrangement_ValueChanged(cpw);
            }
        }

        private void ScrewArrangementChanged()
        {
            CAnchor referenceAnchor = new CAnchor("M16", "8.8", 0.33f, 0.3f, true);
            CScrew referenceScrew = new CScrew("TEK", "14");

            bool bUseAdditionalConnectors = true;
            int iNumberOfAdditionalConnectorsInCorner = 4;
            int iConnectorNumberInCircleSequence = 20;
            float fConnectorRadiusInCircleSequence = 0.25f;

            List<CScrewSequenceGroup> screwSeqGroups = new List<CScrewSequenceGroup>();
            CScrewSequenceGroup gr1 = new CScrewSequenceGroup();
            gr1.NumberOfHalfCircleSequences = 2;
            gr1.NumberOfRectangularSequences = 4;
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr1);
            CScrewSequenceGroup gr2 = new CScrewSequenceGroup();
            gr2.NumberOfHalfCircleSequences = 2;
            gr2.NumberOfRectangularSequences = 4;
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr2);

            CScrewArrangementCircleApexOrKnee screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 1, screwSeqGroups, bUseAdditionalConnectors, fConnectorRadiusInCircleSequence, fConnectorRadiusInCircleSequence, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
            CScrewArrangementRectApexOrKnee screwArrangementRectangleApex = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 0.05f, 0.05f, 0.07f, 0.05f, 8, 2, 0.15f, 0.55f, 0.075f, 0.05f);
            //CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 10, 2);
            CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 12, 2, 0.040f, 0.047f, 0.050f, 0.158f, 12, 2, 0.040f, 0.425f, 0.050f, 0.158f, 12, 2, 0.05f, 0.047f, 0.05f, 0.158f, 14, 2, 0.05f, 0.425f, 0.05f, 0.158f);

            // 270xx default
            bool bUseAdditionalConnectors_270xx = false;
            int iNumberOfAdditionalConnectorsInCorner_270xx = 4;
            int iConnectorNumberInCircleSequence_270xx = 8;
            float fConnectorRadiusInCircleSequence_270xx = 0.12f;

            List<CScrewSequenceGroup> screwSeqGroups_270xx = new List<CScrewSequenceGroup>();
            CScrewSequenceGroup gr1_270xx = new CScrewSequenceGroup();
            gr1_270xx.NumberOfHalfCircleSequences = 2;
            gr1_270xx.NumberOfRectangularSequences = 4;
            gr1_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
            gr1_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
            screwSeqGroups_270xx.Add(gr1_270xx);
            CScrewSequenceGroup gr2_270xx = new CScrewSequenceGroup();
            gr2_270xx.NumberOfHalfCircleSequences = 2;
            gr2_270xx.NumberOfRectangularSequences = 4;
            gr2_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
            gr2_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
            screwSeqGroups_270xx.Add(gr2_270xx);

            CScrewArrangementCircleApexOrKnee screwArrangementCircle_270 = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.27f, 0.27f - 2 * 0.007f - 2 * 0.00115f, 0.18f, 1, screwSeqGroups_270xx, bUseAdditionalConnectors_270xx, fConnectorRadiusInCircleSequence_270xx, fConnectorRadiusInCircleSequence_270xx, iNumberOfAdditionalConnectorsInCorner_270xx, 0.025f, 0.025f);
            CScrewArrangementRectApexOrKnee screwArrangementRectangleApex_270 = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.27f, 0.27f - 2 * 0.007f - 2 * 0.00115f, 0.18f, 5, 2, 0.025f, 0.015f, 0.035f, 0.03f, 5, 2, 0.025f, 0.225f, 0.035f, 0.03f);

            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            switch ((ESerieTypePlate)vm.ComponentSerieIndex)
            {
                case ESerieTypePlate.eSerie_B:
                    {
                        // "BA" - 0
                        // "BB" - 1
                        // "BC" - 2
                        // "BD" - 3
                        // "BE-2 holes" - 4
                        // "BE-3 holes" - 5
                        // "BF-4 holes" - 6
                        // "BF-6 holes" - 7
                        // "BG" - 8
                        // "BH" - 9
                        // "BI" - 10
                        // "BJ- 2 holes" - 11
                        // "BJ- 3 holes" - 12

                        switch (vm.ComponentIndex)
                        {
                            case 0: // BA
                            case 1: // BB
                            case 2: // BC
                            case 8: // BG
                            case 3: // BD
                            case 4: // BE
                            case 5: // BE
                            case 6: // BF
                            case 7: // BF
                            case 11: // BJ
                            case 12: // BJ
                            case 9:  // BH
                            case 10: // BI
                            default:
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate.ScrewArrangement = null;
                                    else
                                    {
                                        CConnectionJointTypes tempJoint = new CConnectionJointTypes(); // TODO Ondrej - to by trebalo refaktorovat a odstranit vytvaranie tempJoint, potrebujeme zavolat GetBasePlateArrangement aby sa podla prefixu plate nastavilo defaultne usporiadanie screwarrangement ale ziaden joint neexistuje
                                        // GetBasePlateArrangement by asi nemalo byt v CConnectionJointTypes ale priamo v CPlate_B_basic
                                        plate.ScrewArrangement = tempJoint.GetBasePlateArrangement(plate.Prefix, referenceScrew, plate.Height_hy);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_L:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // L
                        {
                            CScrewArrangement_L screwArrangement_L = new CScrewArrangement_L(iNumberofHoles, referenceScrew);
                            plate.ScrewArrangement = screwArrangement_L;
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_LL:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // LLH / LLK
                        {
                            CScrewArrangement_LL screwArrangement_LL = new CScrewArrangement_LL(iNumberofHoles, referenceScrew);
                            plate.ScrewArrangement = screwArrangement_LL;
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_F:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // F
                        {
                            CScrewArrangement_F screwArrangement_F = new CScrewArrangement_F(iNumberofHoles, referenceScrew);
                            plate.ScrewArrangement = screwArrangement_F;
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_G:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // G
                        {
                            CScrewArrangement_G screwArrangement_G = new CScrewArrangement_G(/*iNumberofHoles, */ referenceScrew);
                            plate.ScrewArrangement = screwArrangement_G;
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_H:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // H
                        {
                            CScrewArrangement_H screwArrangement_H = new CScrewArrangement_H(/*iNumberofHoles, */ referenceScrew);
                            plate.ScrewArrangement = screwArrangement_H;
                        }

                        break;
                    }
                    /*
                case ESerieTypePlate.eSerie_Q:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // Rectangular
                            plate.ScrewArrangement = screwArrangement_Q;

                        break;
                    }*/
                case ESerieTypePlate.eSerie_J:
                    {
                        if (vm.ComponentIndex == 0) // JA
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleApex;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else if (vm.ComponentIndex == 1) // JB
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleApex;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else //if (vm.ComponentIndex == 2) // JC
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleApex_270;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle_270;
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_K:
                    {
                        if (vm.ComponentIndex == 0) // KA
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else if (vm.ComponentIndex == 1) // KB
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;

                        }
                        else if (vm.ComponentIndex == 2) // KC
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else if (vm.ComponentIndex == 3) // KD
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else if (vm.ComponentIndex == 4) // KE
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else if (vm.ComponentIndex == 5) // KF
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else // KK - TODO - screws are not implemented !!!
                        {
                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(vm.ScrewArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        break;
                    }
                case ESerieTypePlate.eSerie_M:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // Plate M
                        {
                            CScrewArrangement_M screwArrangement_M = new CScrewArrangement_M(iNumberofHoles, referenceScrew);
                            plate.ScrewArrangement = screwArrangement_M;
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_N:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // Plate N
                        {
                            CScrewArrangement_N screwArrangement_N = new CScrewArrangement_N(iNumberofHoles, referenceScrew);
                            plate.ScrewArrangement = screwArrangement_N;
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_O:
                    {
                        if (vm.ScrewArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (vm.ScrewArrangementIndex == 1) // Rectangular - Plate O
                        {
                            CScrewArrangement_O screwArrangement_O = new CScrewArrangement_O(referenceScrew, 1, 10, 0.02f, 0.02f, 0.05f, 0.05f, 1, 10, 0.18f, 0.02f, 0.05f, 0.05f);
                            plate.ScrewArrangement = screwArrangement_O;
                        }

                        break;
                    }
               default:
                    {
                        // Not implemented
                        break;
                    }
            }
            vm.SetComponentProperties(plate);
            if (plate != null) vm.SetScrewArrangementProperties(plate.ScrewArrangement);
        }

        private void SetUIElementsVisibility(SystemComponentViewerViewModel vm)
        {
            if (vm.ComponentTypeIndex == 0)  //CRSC
            {
                TxtCombScrewArrangment.Visibility = Visibility.Hidden;
                Combobox_ScrewArrangement.Visibility = Visibility.Hidden;
                TxtScrewArrangment.Visibility = Visibility.Hidden;
                DataGridScrewArrangement.Visibility = Visibility.Hidden;

                TxtGeometry.Visibility = Visibility.Hidden;
                DataGridGeometry.Visibility = Visibility.Hidden;

                BtnFindCNCPath.Visibility = Visibility.Hidden;
                BtnExportCNC.Visibility = Visibility.Hidden;
                BtnShowCNCSetupFile.Visibility = Visibility.Hidden;
                BtnShowCNCDrillingFile.Visibility = Visibility.Hidden;

                if (MainTabControl.SelectedIndex == 0) // 2D
                {
                    panelOptions2D.Visibility = Visibility.Visible;
                    panelOptions3D.Visibility = Visibility.Hidden;
                }
                else if (MainTabControl.SelectedIndex == 1) //3D
                {
                    panelOptions2D.Visibility = Visibility.Hidden;
                    panelOptions3D.Visibility = Visibility.Hidden;
                }
                else
                {
                    panelOptions2D.Visibility = Visibility.Hidden;
                    panelOptions3D.Visibility = Visibility.Hidden;
                }

                chbDrawPoints2D.IsEnabled = true;
                chbDrawOutLine2D.IsEnabled = true;
                chbDrawPointNumbers2D.IsEnabled = true;
                chbDrawHoles2D.IsEnabled = false;
                chbDrawHoleCentreSymbol2D.IsEnabled = false;
                chbDrawDrillingRoute2D.IsEnabled = false;
                chbDrawDimensions2D.IsEnabled = false;
                chbDrawMemberOutline2D.IsEnabled = false;
                chbDrawBendLines2D.IsEnabled = false;

                panelOptionsTransform2D.Visibility = Visibility.Visible;

                tabItemDoc.IsEnabled = false;
                panelProductionInfo.Visibility = Visibility.Hidden;
                BtnExportToPDF.Visibility = Visibility.Hidden;
                BtnSavePlate.Visibility = Visibility.Hidden;
                BtnLoadPlate.Visibility = Visibility.Hidden;
            }
            else if (vm.ComponentTypeIndex == 1) //plate
            {
                TxtCombScrewArrangment.Visibility = Visibility.Visible;
                Combobox_ScrewArrangement.Visibility = Visibility.Visible;
                TxtScrewArrangment.Visibility = Visibility.Visible;
                DataGridScrewArrangement.Visibility = Visibility.Visible;

                TxtGeometry.Visibility = Visibility.Visible;
                DataGridGeometry.IsReadOnly = false;
                DataGridGeometry.Visibility = Visibility.Visible;
                BtnFindCNCPath.Visibility = Visibility.Visible;
                BtnExportCNC.Visibility = Visibility.Visible;
                BtnShowCNCSetupFile.Visibility = Visibility.Visible;
                BtnShowCNCDrillingFile.Visibility = Visibility.Visible;

                if (MainTabControl.SelectedIndex == 0) // 2D
                {
                    panelOptions2D.Visibility = Visibility.Visible;
                    panelOptions3D.Visibility = Visibility.Hidden;
                }
                else if (MainTabControl.SelectedIndex == 1) //3D
                {
                    panelOptions2D.Visibility = Visibility.Hidden;
                    panelOptions3D.Visibility = Visibility.Visible;
                }
                else
                {
                    panelOptions2D.Visibility = Visibility.Hidden;
                    panelOptions3D.Visibility = Visibility.Hidden;
                }

                chbDrawPoints2D.IsEnabled = true;
                chbDrawOutLine2D.IsEnabled = true;
                chbDrawPointNumbers2D.IsEnabled = true;
                chbDrawHoles2D.IsEnabled = true;
                chbDrawHoleCentreSymbol2D.IsEnabled = true;
                chbDrawDrillingRoute2D.IsEnabled = true;
                chbDrawDimensions2D.IsEnabled = true;
                chbDrawMemberOutline2D.IsEnabled = true;
                chbDrawBendLines2D.IsEnabled = true;

                panelOptionsTransform2D.Visibility = Visibility.Visible;

                tabItemDoc.IsEnabled = true;
                if (plate != null && plate.ScrewArrangement != null)
                {
                    BtnFindCNCPath.IsEnabled = plate.ScrewArrangement.IHolesNumber > 0;
                    // BtnExportCNC can be enabled for export of plate setup file or cutting file evenif drilling route is not defined or holes are not defined.
                    //BtnExportCNC.IsEnabled = (plate.DrillingRoutePoints != null && plate.DrillingRoutePoints.Count > 0);
                    BtnShowCNCDrillingFile.IsEnabled = (plate.DrillingRoutePoints != null && plate.DrillingRoutePoints.Count > 0);
                }
                panelProductionInfo.Visibility = Visibility.Visible;
                BtnExportToPDF.Visibility = Visibility.Visible;
                BtnSavePlate.Visibility = Visibility.Visible;
                BtnLoadPlate.Visibility = Visibility.Visible;

                //mozno by sa to dalo naviazat na plate.IsSymmetric() metodu
                if (vm.ComponentSerieIndex == (int)ESerieTypePlate.eSerie_K &&
                    (vm.ComponentIndex != 0 && // Plate KA
                     vm.ComponentIndex != 6))  // Plate KK
                {
                    LabelAmountRH.Visibility = Visibility.Visible;
                    TextBoxAmountRH.Visibility = Visibility.Visible;
                    LabelAmountLH.Visibility = Visibility.Visible;
                    TextBoxAmountLH.Visibility = Visibility.Visible;
                }
                else
                {
                    LabelAmountRH.Visibility = Visibility.Hidden;
                    TextBoxAmountRH.Visibility = Visibility.Hidden;
                    LabelAmountLH.Visibility = Visibility.Hidden;
                    TextBoxAmountLH.Visibility = Visibility.Hidden;
                }
            }
            else if (vm.ComponentTypeIndex == 2) //screw
            {
                TxtCombScrewArrangment.Visibility = Visibility.Hidden;
                Combobox_ScrewArrangement.Visibility = Visibility.Hidden;
                TxtScrewArrangment.Visibility = Visibility.Hidden;
                DataGridScrewArrangement.Visibility = Visibility.Hidden;

                TxtGeometry.Visibility = Visibility.Hidden;
                DataGridGeometry.Visibility = Visibility.Hidden;

                BtnFindCNCPath.Visibility = Visibility.Hidden;
                BtnExportCNC.Visibility = Visibility.Hidden;
                BtnShowCNCSetupFile.Visibility = Visibility.Hidden;
                BtnShowCNCDrillingFile.Visibility = Visibility.Hidden;

                panelOptions2D.Visibility = Visibility.Hidden;
                panelOptions3D.Visibility = Visibility.Hidden;
                chbDrawPoints2D.IsEnabled = false;
                chbDrawOutLine2D.IsEnabled = false;
                chbDrawPointNumbers2D.IsEnabled = false;
                chbDrawHoles2D.IsEnabled = false;
                chbDrawHoleCentreSymbol2D.IsEnabled = false;
                chbDrawDrillingRoute2D.IsEnabled = false;
                chbDrawDimensions2D.IsEnabled = false;
                chbDrawMemberOutline2D.IsEnabled = false;
                chbDrawBendLines2D.IsEnabled = false;

                panelOptionsTransform2D.Visibility = Visibility.Hidden;

                tabItemDoc.IsEnabled = false;

                panelProductionInfo.Visibility = Visibility.Hidden;
                BtnExportToPDF.Visibility = Visibility.Hidden;
                BtnSavePlate.Visibility = Visibility.Hidden;
                BtnLoadPlate.Visibility = Visibility.Hidden;
            }

            //uncheck all Transformation Options
            if (vm.MirrorX) vm.MirrorX = false;
            if (vm.MirrorY) vm.MirrorY = false;
            if (vm.Rotate90CCW) vm.Rotate90CCW = false;
            if (vm.Rotate90CW) vm.Rotate90CW = false;
        }

        private void SetUIElementsVisibilityForScrewArrangement(SystemComponentViewerViewModel vm)
        {
            if (vm.ScrewArrangementIndex == 0)
            {
                TxtScrewArrangment.Visibility = Visibility.Hidden;
                DataGridScrewArrangement.Visibility = Visibility.Hidden;
                chbDrawHoles2D.IsEnabled = false;
                chbDrawHoleCentreSymbol2D.IsEnabled = false;
                chbDrawDrillingRoute2D.IsEnabled = false;
                BtnFindCNCPath.IsEnabled = false;
                BtnShowCNCDrillingFile.IsEnabled = false;
            }
            else
            {
                TxtScrewArrangment.Visibility = Visibility.Visible;
                DataGridScrewArrangement.Visibility = Visibility.Visible;
                chbDrawHoles2D.IsEnabled = true;
                chbDrawHoleCentreSymbol2D.IsEnabled = true;
                chbDrawDrillingRoute2D.IsEnabled = true;
                BtnFindCNCPath.IsEnabled = true;
                BtnShowCNCDrillingFile.IsEnabled = true;
            }
        }

        private void LoadDataFromDatabase()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentIndex < 0) // Index is invalid - set default 0
                vm.ComponentIndex = 0;

            if (vm.ComponentTypeIndex == 0) // Cross-section
            {
                switch ((ESerieTypeCrSc_FS)vm.ComponentSerieIndex)
                {
                    case ESerieTypeCrSc_FS.eSerie_Box_10075:
                        {
                            fb = dcomponents.arr_Serie_Box_FS_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_Box_FS_Dimension[vm.ComponentIndex, 1] / 1000f;
                            ft = dcomponents.arr_Serie_Box_FS_Dimension[vm.ComponentIndex, 2] / 1000f;
                            cComponentColor = dcomponents.arr_Serie_Box_FS_Colors[vm.ComponentIndex];
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_Z:
                        {
                            fh = dcomponents.arr_Serie_Z_FS_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fb_fl = dcomponents.arr_Serie_Z_FS_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fc_lip1 = dcomponents.arr_Serie_Z_FS_Dimension[vm.ComponentIndex, 2] / 1000f;
                            ft = dcomponents.arr_Serie_Z_FS_Dimension[vm.ComponentIndex, 3] / 1000f;
                            cComponentColor = dcomponents.arr_Serie_Z_FS_Colors[vm.ComponentIndex];
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_single:
                        {
                            fb = dcomponents.arr_Serie_C_FS_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_C_FS_Dimension[vm.ComponentIndex, 1] / 1000f;
                            ft = dcomponents.arr_Serie_C_FS_Dimension[vm.ComponentIndex, 2] / 1000f;
                            cComponentColor = dcomponents.arr_Serie_C_FS_Colors[vm.ComponentIndex];
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_back_to_back:
                        {
                            fb = dcomponents.arr_Serie_C_BtoB_FS_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_C_BtoB_FS_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fc_lip1 = dcomponents.arr_Serie_C_BtoB_FS_Dimension[vm.ComponentIndex, 2] / 1000f;
                            ft = dcomponents.arr_Serie_C_BtoB_FS_Dimension[vm.ComponentIndex, 3] / 1000f;
                            cComponentColor = dcomponents.arr_Serie_C_BtoB_FS_Colors[vm.ComponentIndex];
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_nested:
                        {
                            fb = dcomponents.arr_Serie_C_Nested_FS_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_C_Nested_FS_Dimension[vm.ComponentIndex, 1] / 1000f;
                            ft = dcomponents.arr_Serie_C_Nested_FS_Dimension[vm.ComponentIndex, 2] / 1000f;
                            cComponentColor = dcomponents.arr_Serie_C_Nested_FS_Colors[vm.ComponentIndex];
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_Box_63020:
                        {
                            fb = dcomponents.arr_Serie_Box63020_FS_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_Box63020_FS_Dimension[vm.ComponentIndex, 1] / 1000f;
                            ft = dcomponents.arr_Serie_Box63020_FS_Dimension[vm.ComponentIndex, 2] / 1000f;
                            iNumberOfStiffeners = (int)dcomponents.arr_Serie_Box63020_FS_Dimension[vm.ComponentIndex, 4];
                            cComponentColor = dcomponents.arr_Serie_Box63020_FS_Colors[vm.ComponentIndex];
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_PurlinDek:
                    case ESerieTypeCrSc_FS.eSerie_SmartDek:
                        {
                            // TODO TEMP
                            fb = 0.1f;
                            fh = 0.2f;
                            ft = 0.001f;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            else if (vm.ComponentTypeIndex == 1) // Plate
            {
                switch ((ESerieTypePlate)vm.ComponentSerieIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            CPlate_B_Properties prop = CJointsManager.GetPlate_B_Properties(vm.ComponentIndex + 1);

                            fb = (float)prop.dim1;
                            fb2 = fb;
                            fh = (float)prop.dim2y;
                            fl = (float)prop.dim3;
                            ft = (float)prop.t;
                            iNumberofHoles = prop.iNumberHolesAnchors; // !!!! - rozlisovat medzi otvormi pre skrutky a pre anchors

                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            CPlate_L_Properties prop = CJointsManager.GetPlate_L_Properties(vm.ComponentIndex + 1);

                            fb = (float)prop.dim1;
                            fb2 = fb;
                            fh = (float)prop.dim2y;
                            fl = (float)prop.dim3;
                            ft = (float)prop.thickness;
                            iNumberofHoles = prop.NumberOfHolesScrews;
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            CPlate_LL_Properties prop = CJointsManager.GetPlate_LL_Properties(vm.ComponentIndex + 1);

                            fb = (float)prop.dim11;
                            fb2 = (float)prop.dim12;
                            fh = (float)prop.dim2y;
                            fl = (float)prop.dim3;
                            ft = (float)prop.thickness;
                            iNumberofHoles = prop.NumberOfHolesScrews;
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            CPlate_F_Properties prop = CJointsManager.GetPlate_F_Properties(vm.ComponentIndex + 1);

                            fb = (float)prop.dim11;
                            fb2 = (float)prop.dim12;
                            fh = (float)prop.dim2y;
                            fl = (float)prop.dim3;
                            ft = (float)prop.thickness;
                            iNumberofHoles = prop.NumberOfHolesScrews;
                            break;
                        }
                    case ESerieTypePlate.eSerie_G:
                        {
                            fb = dcomponents.arr_Serie_G_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fb2 = dcomponents.arr_Serie_G_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fh = dcomponents.arr_Serie_G_Dimension[vm.ComponentIndex, 2] / 1000f;
                            fh2 = dcomponents.arr_Serie_G_Dimension[vm.ComponentIndex, 3] / 1000f;
                            fl = dcomponents.arr_Serie_G_Dimension[vm.ComponentIndex, 4] / 1000f;
                            ft = dcomponents.arr_Serie_G_Dimension[vm.ComponentIndex, 5] / 1000f;
                            break;
                        }
                    case ESerieTypePlate.eSerie_H:
                        {
                            fb = dcomponents.arr_Serie_H_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_H_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fh2 = dcomponents.arr_Serie_H_Dimension[vm.ComponentIndex, 2] / 1000f;
                            ft = dcomponents.arr_Serie_H_Dimension[vm.ComponentIndex, 3] / 1000f;
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                        {
                            fb = dcomponents.arr_Serie_Q_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_Q_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fl = dcomponents.arr_Serie_Q_Dimension[vm.ComponentIndex, 2] / 1000f;
                            ft = dcomponents.arr_Serie_Q_Dimension[vm.ComponentIndex, 3] / 1000f;
                            iNumberofHoles = (int)dcomponents.arr_Serie_Q_Dimension[vm.ComponentIndex, 4];
                            break;
                        }
                    case ESerieTypePlate.eSerie_T:
                        {
                            fb = dcomponents.arr_Serie_T_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_T_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fl = dcomponents.arr_Serie_T_Dimension[vm.ComponentIndex, 2] / 1000f;
                            ft = dcomponents.arr_Serie_T_Dimension[vm.ComponentIndex, 3] / 1000f;
                            iNumberofHoles = (int)dcomponents.arr_Serie_T_Dimension[vm.ComponentIndex, 4];
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            fb = dcomponents.arr_Serie_Y_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_Y_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fl = dcomponents.arr_Serie_Y_Dimension[vm.ComponentIndex, 2] / 1000f;
                            fl2 = dcomponents.arr_Serie_Y_Dimension[vm.ComponentIndex, 3] / 1000f;
                            ft = dcomponents.arr_Serie_Y_Dimension[vm.ComponentIndex, 4] / 1000f;
                            iNumberofHoles = (int)dcomponents.arr_Serie_Y_Dimension[vm.ComponentIndex, 5];
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            fb = dcomponents.arr_Serie_J_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_J_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fh2 = dcomponents.arr_Serie_J_Dimension[vm.ComponentIndex, 2] / 1000f;
                            fl = dcomponents.arr_Serie_J_Dimension[vm.ComponentIndex, 3] / 1000f;
                            ft = dcomponents.arr_Serie_J_Dimension[vm.ComponentIndex, 4] / 1000f;
                            iNumberofHoles = (int)dcomponents.arr_Serie_J_Dimension[vm.ComponentIndex, 5];
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            fb_R = dcomponents.arr_Serie_K_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fb = dcomponents.arr_Serie_K_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fh = dcomponents.arr_Serie_K_Dimension[vm.ComponentIndex, 2] / 1000f;
                            fb2 = dcomponents.arr_Serie_K_Dimension[vm.ComponentIndex, 3] / 1000f;
                            fh2 = dcomponents.arr_Serie_K_Dimension[vm.ComponentIndex, 4] / 1000f;
                            fl = dcomponents.arr_Serie_K_Dimension[vm.ComponentIndex, 5] / 1000f;
                            ft = dcomponents.arr_Serie_K_Dimension[vm.ComponentIndex, 6] / 1000f;
                            iNumberofHoles = (int)dcomponents.arr_Serie_K_Dimension[vm.ComponentIndex, 7];
                            break;
                        }
                    case ESerieTypePlate.eSerie_N:
                        {
                            fb = dcomponents.arr_Serie_N_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fb2 = dcomponents.arr_Serie_N_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fh = dcomponents.arr_Serie_N_Dimension[vm.ComponentIndex, 2] / 1000f;
                            fl = dcomponents.arr_Serie_N_Dimension[vm.ComponentIndex, 3] / 1000f;
                            ft = dcomponents.arr_Serie_N_Dimension[vm.ComponentIndex, 4] / 1000f;
                            iNumberofHoles = (int)dcomponents.arr_Serie_N_Dimension[vm.ComponentIndex, 5];
                            break;
                        }
                    case ESerieTypePlate.eSerie_M:
                        {
                            // b, h, t, iHoles, bBeam, slope_deg
                            fb = dcomponents.arr_Serie_M_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fh = dcomponents.arr_Serie_M_Dimension[vm.ComponentIndex, 1] / 1000f;
                            ft = dcomponents.arr_Serie_M_Dimension[vm.ComponentIndex, 2] / 1000f;
                            iNumberofHoles = (int)dcomponents.arr_Serie_M_Dimension[vm.ComponentIndex, 3];
                            fb_B = dcomponents.arr_Serie_M_Dimension[vm.ComponentIndex, 4] / 1000f;
                            fRoofPitch_rad = dcomponents.arr_Serie_M_Dimension[vm.ComponentIndex, 5] / 180 * MathF.fPI;
                            fGamma_rad = dcomponents.arr_Serie_M_Dimension[vm.ComponentIndex, 6] / 180 * MathF.fPI;
                            break;
                        }
                    case ESerieTypePlate.eSerie_O:
                        {
                            fb = dcomponents.arr_Serie_O_Dimension[vm.ComponentIndex, 0] / 1000f;
                            fb2 = dcomponents.arr_Serie_O_Dimension[vm.ComponentIndex, 1] / 1000f;
                            fh = dcomponents.arr_Serie_O_Dimension[vm.ComponentIndex, 2] / 1000f;
                            fh2 = dcomponents.arr_Serie_O_Dimension[vm.ComponentIndex, 3] / 1000f;
                            ft = dcomponents.arr_Serie_O_Dimension[vm.ComponentIndex, 4] / 1000f;
                            iNumberofHoles = (int)dcomponents.arr_Serie_O_Dimension[vm.ComponentIndex, 5];
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else //  Screw
            {
                CTEKScrewProperties prop = CTEKScrewsManager.GetScrewProperties(vm.ComponentIndex + 1);
                sGauge_Screw = prop.gauge;
            }
        }

        private void UpdateAndDisplayPlate()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentTypeIndex == 1) // Plate
            {
                if (plate.ScrewArrangement != null)
                    plate.ScrewArrangement.UpdateArrangmentData();

                if (plate is CConCom_Plate_B_basic) // Base plates
                {
                    CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
                    if (basePlate.AnchorArrangement != null)
                        basePlate.AnchorArrangement.UpdateArrangmentData();

                    basePlate.UpdatePlateData(plate.ScrewArrangement);
                }
                else // other plates (without anchors)
                    plate.UpdatePlateData(plate.ScrewArrangement);

                DisplayPlate(true);
            }
        }

        private void SetFrame2DSize()
        {
            Frame2DWidth = Frame2D.ActualWidth;
            Frame2DHeight = Frame2D.ActualHeight;
            // Nenastavovat z maximalnych rozmerov screen, ale z aktualnych rozmerov okna System Component Viewer
            if (Frame2DWidth == 0) Frame2DWidth = this.Width - 669; // SystemParameters.PrimaryScreenWidth / 2 - 15;
            if (Frame2DHeight == 0) Frame2DHeight = this.Height - 116; // SystemParameters.PrimaryScreenHeight - 145;
        }

        private void MirrorComponentX(SystemComponentViewerViewModel vm)
        {
            if (vm.ComponentTypeIndex == 0)
            {
                crsc.MirrorCRSCAboutX();
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                plate.MirrorPlateAboutX();
            }
        }

        private void MirrorComponentY(SystemComponentViewerViewModel vm)
        {
            if (vm.ComponentTypeIndex == 0)
            {
                crsc.MirrorCRSCAboutY();
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                plate.MirrorPlateAboutY();
            }
        }

        private void RotateComponent90CW(SystemComponentViewerViewModel vm)
        {
            if (vm.ComponentTypeIndex == 0)
            {
                crsc.RotateCrsc_CW(90);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                plate.RotatePlateAboutZ_CW(90);
            }
        }

        private void RotateComponent90CCW(SystemComponentViewerViewModel vm)
        {
            if (vm.ComponentTypeIndex == 0)
            {
                crsc.RotateCrsc_CW(-90);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                plate.RotatePlateAboutZ_CW(-90);
            }
        }

        private void DisplayComponent(SystemComponentViewerViewModel vm)
        {
            if (vm == null) return;
            // Display Component
            if (vm.ComponentTypeIndex == 0)
            {
                DisplayCRSC(false);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                //DisplayPlate(false);
                UpdateAndDisplayPlate();
            }
            else
            {
                DisplayScrew();
            }
        }

        private void DisplayPlate(bool useTransformOptions)
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            // Create 2D page
            page2D = new Canvas();

            SetFrame2DSize();

            //page2D.RenderSize = new System.Windows.Size(Frame2DWidth, Frame2DHeight);

            if (useTransformOptions)
            {
                if (vm.MirrorX) plate.MirrorPlateAboutX();
                if (vm.MirrorY) plate.MirrorPlateAboutY();
                if (vm.Rotate90CW) plate.RotatePlateAboutZ_CW(90);
                if (vm.Rotate90CCW) plate.RotatePlateAboutZ_CW(-90);
            }
            if (vm.DrillingRoutePoints != null) plate.DrillingRoutePoints = vm.DrillingRoutePoints;

            Drawing2D.DrawPlateToCanvas(plate,
               Frame2DWidth,
               Frame2DHeight,
               ref page2D,
               vm.DrawPoints2D,
               vm.DrawOutLine2D,
               vm.DrawPointNumbers2D,
               vm.DrawHoles2D,
               vm.DrawHoleCentreSymbol2D,
               vm.DrawDrillingRoute2D,
               vm.DrawDimensions2D,
               vm.DrawMemberOutline2D,
               vm.DrawBendLines2D);

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;

            // Create 3D window
            sDisplayOptions.bDisplayGlobalAxis = false;
            sDisplayOptions.bUseEmissiveMaterial = true;
            sDisplayOptions.bUseLightAmbient = true;
            sDisplayOptions.bDisplayConnectors = vm.DrawScrews3D;
            page3D = new Page3Dmodel(plate, sDisplayOptions);

            // Display model in 3D preview frame
            Frame3D.Content = page3D;

            this.UpdateLayout();
        }

        private void DisplayCRSC(bool useTransformOptions)
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            // Create 2D page
            page2D = new Canvas();

            SetFrame2DSize();

            if (useTransformOptions)
            {
                if (vm.MirrorX) crsc.MirrorCRSCAboutX();
                if (vm.MirrorY) crsc.MirrorCRSCAboutY();
                if (vm.Rotate90CW) crsc.RotateCrsc_CW(90);
                if (vm.Rotate90CCW) crsc.RotateCrsc_CW(-90);
            }

            Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
               vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawDimensions2D);

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;

            // Create 3D window
            page3D = new Page3Dmodel(crsc, sDisplayOptions);

            // Display model in 3D preview frame
            Frame3D.Content = page3D;

            this.UpdateLayout();
        }

        private void DisplayScrew()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            // Create 2D page
            page2D = new Canvas();

            SetFrame2DSize();

            // Screw
            bool bDrawCentreSymbol = true;
            Drawing2D.DrawScrewToCanvas(screw, Frame2DWidth, Frame2DHeight, ref page2D, bDrawCentreSymbol);

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;

            // Create 3D window
            // Screw
            PerspectiveCamera camera = new PerspectiveCamera(new Point3D(36.6796089675504, -63.5328099899833, 57.4552066599888), new Vector3D(-43.3, 75, -50), new Vector3D(0, 0, 1), 51.5103932666685);
            page3D = new Page3Dmodel(ConfigurationManager.AppSettings["screwModel3D"], camera);

            // Display model in 3D preview frame
            Frame3D.Content = page3D;

            this.UpdateLayout();
        }

        private void UpdateAll()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            LoadDataFromDatabase();

            // Create Model
            // Change Combobox
            if (vm.ComponentTypeIndex == 0) // Cross-section
            {
                switch ((ESerieTypeCrSc_FS)vm.ComponentSerieIndex)
                {
                    case ESerieTypeCrSc_FS.eSerie_Box_10075:
                        {
                            //temp test 
                            //crsc = new CCrSc_3_10075_BOX(fh * 3 , fb * 3, ft * 3, Colors.Red); 

                            crsc = new CCrSc_3_10075_BOX(0, fh, fb, ft, cComponentColor); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_Z:
                        {
                            crsc = new CCrSc_3_Z(0, fh, fb_fl, fc_lip1, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_single:
                        {
                            if (vm.ComponentIndex < 3) // C270
                                crsc = new CCrSc_3_270XX_C(0, fh, fb, ft, cComponentColor);
                            else
                                crsc = new CCrSc_3_50020_C(0, fh, fb, ft, cComponentColor);

                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_back_to_back:
                        {
                            crsc = new CCrSc_3_270XX_C_BACK_TO_BACK(0, fh, fb, fc_lip1, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_nested:
                        {
                            if (vm.ComponentIndex == 0)
                                crsc = new CCrSc_3_270XX_C_NESTED(0, fh, fb, ft, cComponentColor); // C270115n
                            else
                                crsc = new CCrSc_3_50020_C_NESTED(0, fh, fb, ft, cComponentColor); // C50020n
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_Box_63020:
                        {
                            crsc = new CCrSc_3_63020_BOX(0, fh, fb, ft, iNumberOfStiffeners, cComponentColor); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_SmartDek:
                        {
                            crsc = new CCrSc_3_TR_SMARTDEK(0, fh, fb, ft, cComponentColor); // Trapezoidal sheeting
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_PurlinDek:
                        {
                            crsc = new CCrSc_3_TR_PURLINDEK(0, fh, fb, ft, cComponentColor); // Trapezoidal sheeting
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
                vm.SetComponentProperties(crsc);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                bool bUseSimpleShapeOfPlates = true; // Zjednoduseny alebo presny tvar plechu

                CAnchor referenceAnchor = new CAnchor("M16", "8.8", 0.33f, 0.3f, true);
                CScrew referenceScrew = new CScrew("TEK", "14");

                float fCrsc_h = 0.27f; // Default depth of connected member cross-section

                bool bUseAdditionalConnectors = true;
                int iNumberOfAdditionalConnectorsInCorner = 4;
                int iConnectorNumberInCircleSequence = 20;
                float fConnectorRadiusInCircleSequence = 0.25f;

                List<CScrewSequenceGroup> screwSeqGroups = new List<CScrewSequenceGroup>();
                CScrewSequenceGroup gr1 = new CScrewSequenceGroup();
                gr1.NumberOfHalfCircleSequences = 2;
                gr1.NumberOfRectangularSequences = 4;
                gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
                gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
                screwSeqGroups.Add(gr1);
                CScrewSequenceGroup gr2 = new CScrewSequenceGroup();
                gr2.NumberOfHalfCircleSequences = 2;
                gr2.NumberOfRectangularSequences = 4;
                gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
                gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
                screwSeqGroups.Add(gr2);

                // 63020 default
                CScrewArrangementCircleApexOrKnee screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 1, screwSeqGroups, bUseAdditionalConnectors, fConnectorRadiusInCircleSequence, fConnectorRadiusInCircleSequence, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
                CScrewArrangementRectApexOrKnee screwArrangementRectangleApex = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 0.05f, 0.05f, 0.07f, 0.05f, 8, 2, 0.15f, 0.55f, 0.075f, 0.05f);
                //CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 10, 2);
                CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 12, 2, 0.040f, 0.047f, 0.050f, 0.158f, 12, 2, 0.040f, 0.425f, 0.050f, 0.158f, 12, 2, 0.05f, 0.047f, 0.05f, 0.158f, 14, 2, 0.05f, 0.425f, 0.05f, 0.158f);

                // 270xx default
                bool bUseAdditionalConnectors_270xx = false;
                int iNumberOfAdditionalConnectorsInCorner_270xx = 4;
                int iConnectorNumberInCircleSequence_270xx = 8;
                float fConnectorRadiusInCircleSequence_270xx = 0.12f;

                List<CScrewSequenceGroup> screwSeqGroups_270xx = new List<CScrewSequenceGroup>();
                CScrewSequenceGroup gr1_270xx = new CScrewSequenceGroup();
                gr1_270xx.NumberOfHalfCircleSequences = 2;
                gr1_270xx.NumberOfRectangularSequences = 4;
                gr1_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
                gr1_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
                screwSeqGroups_270xx.Add(gr1_270xx);
                CScrewSequenceGroup gr2_270xx = new CScrewSequenceGroup();
                gr2_270xx.NumberOfHalfCircleSequences = 2;
                gr2_270xx.NumberOfRectangularSequences = 4;
                gr2_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
                gr2_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
                screwSeqGroups_270xx.Add(gr2_270xx);

                CScrewArrangementCircleApexOrKnee screwArrangementCircle_270 = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.27f, 0.27f - 2 * 0.007f - 2 * 0.00115f, 0.18f, 1, screwSeqGroups_270xx, bUseAdditionalConnectors_270xx, fConnectorRadiusInCircleSequence_270xx, fConnectorRadiusInCircleSequence_270xx, iNumberOfAdditionalConnectorsInCorner_270xx, 0.025f, 0.025f);
                CScrewArrangementRectApexOrKnee screwArrangementRectangleApex_270 = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.27f, 0.27f - 2 * 0.007f - 2 * 0.00115f, 0.18f, 5, 2, 0.025f, 0.015f, 0.035f, 0.03f, 5, 2, 0.025f, 0.225f, 0.035f, 0.03f);

                switch ((ESerieTypePlate)vm.ComponentSerieIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            CPlate_B_Properties prop = CJointsManager.GetPlate_B_Properties(vm.ComponentIndex + 1);
                            CConnectionJointTypes tempJoint = new CConnectionJointTypes(); // TODO Ondrej - to by trebalo refaktorovat a odstranit vytvaranie tempJoint, potrebujeme zavolat GetBasePlateArrangement aby sa podla prefixu plate nastavilo defaultne usporiadanie screwarrangement ale ziaden joint neexistuje
                                                                                           // GetBasePlateArrangement by asi nemalo byt v CConnectionJointTypes ale priamo v CPlate_B_basic
                            plate = new CConCom_Plate_B_basic(prop.Name, controlpoint, fb, fh, fl, ft, 0, 0, 0, referenceAnchor, tempJoint.GetBasePlateArrangement(prop.Name, referenceScrew, fh), true); // B
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            CPlate_L_Properties prop = CJointsManager.GetPlate_L_Properties(vm.ComponentIndex + 1);
                            CScrewArrangement_L screwArrangement_L = new CScrewArrangement_L(prop.NumberOfHolesScrews, referenceScrew);
                            plate = new CConCom_Plate_F_or_L(prop.Name, controlpoint, fb, fh, fl, ft, fCrsc_h, 0, 0, 0, screwArrangement_L, true); // L
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            CPlate_LL_Properties prop = CJointsManager.GetPlate_LL_Properties(vm.ComponentIndex + 1);
                            CScrewArrangement_LL screwArrangement_LL = new CScrewArrangement_LL(prop.NumberOfHolesScrews, referenceScrew);
                            plate = new CConCom_Plate_LL(prop.Name, controlpoint, fb, fb2, fh, fl, ft, 0, 0, 0, screwArrangement_LL, true); // LL
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            CPlate_F_Properties prop = CJointsManager.GetPlate_F_Properties(vm.ComponentIndex + 1);
                            CScrewArrangement_F screwArrangement_F = new CScrewArrangement_F(prop.NumberOfHolesScrews, referenceScrew);
                            plate = new CConCom_Plate_F_or_L(prop.Name, controlpoint, fb, fb2, fh, fl, ft, fCrsc_h, 0f, 0f, 0f, screwArrangement_F, true); // F
                            break;
                        }
                    case ESerieTypePlate.eSerie_G:
                        {
                            CScrewArrangement_G screwArrangement_G = new CScrewArrangement_G(/*iNumberofHoles, */ referenceScrew);
                            plate = new CConCom_Plate_G(dcomponents.arr_Serie_G_Names[vm.ComponentIndex], controlpoint, fb, fb2, fh, fh2, fl, 0.5f * fh2, ft, 0f, 0f, 0f, screwArrangement_G, true); // G
                            break;
                        }
                    case ESerieTypePlate.eSerie_H:
                        {
                            CScrewArrangement_H screwArrangement_H = new CScrewArrangement_H(/*iNumberofHoles, */ referenceScrew);
                            plate = new CConCom_Plate_H(dcomponents.arr_Serie_H_Names[vm.ComponentIndex], controlpoint, fb, fh, fh2, 0.2f * fb, ft, 11f * MathF.fPI / 180f, 0f, 0f, 0f, screwArrangement_H, true); // H
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                        {
                            plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_Q_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // Q
                            break;
                        }
                    case ESerieTypePlate.eSerie_T:
                        {
                            plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_T_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // T
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_Y_Names[0], controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles, true); // Y
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            if (vm.ComponentIndex == 0) // JA
                            {
                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, true, null, true);
                                else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, true, screwArrangementRectangleApex, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, true, screwArrangementCircle, true);
                            }
                            else if (vm.ComponentIndex == 1) // JB
                            {
                                if (bUseSimpleShapeOfPlates)
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate = new CConCom_Plate_JBS(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true, null, true);
                                    else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                        plate = new CConCom_Plate_JBS(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true, screwArrangementRectangleApex, true);
                                    else//(vm.ScrewArrangementIndex == 2) // Circle
                                        plate = new CConCom_Plate_JBS(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true, screwArrangementCircle, true);
                                }
                                else
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true, null, true);
                                    else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                        plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true, screwArrangementRectangleApex, true);
                                    else//(vm.ScrewArrangementIndex == 2) // Circle
                                        plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true, screwArrangementCircle, true);
                                }
                            }
                            else //(vm.ComponentIndex == 2) // JC
                            {
                                float fw = 0.205f; // TODO - dopracovat vypocet tak aby sa konvertovali hodnoty z databazy
                                float fd = 0.27f;
                                float fSlope_rad = 3f * MathF.fPI / 180f; // Default 3 stupne

                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_JCS(dcomponents.arr_Serie_J_Names[2], controlpoint, fd, fw, fl, fSlope_rad, ft, 0, 0, 0, true, null, true);
                                else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_JCS(dcomponents.arr_Serie_J_Names[2], controlpoint, fd, fw, fl, fSlope_rad, ft, 0, 0, 0, true, screwArrangementRectangleApex_270, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_JCS(dcomponents.arr_Serie_J_Names[2], controlpoint, fd, fw, fl, fSlope_rad, ft, 0, 0, 0, true, screwArrangementCircle_270, true);
                            }

                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            if (vm.ComponentIndex == 0) // KA
                            {
                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, false, null, true);
                                else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, false, screwArrangementCircle, true);
                            }
                            else if (vm.ComponentIndex == 1) // KB
                            {
                                if (bUseSimpleShapeOfPlates)
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate = new CConCom_Plate_KBS(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, null, true);
                                    else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                        plate = new CConCom_Plate_KBS(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                    else//(vm.ScrewArrangementIndex == 2) // Circle
                                        plate = new CConCom_Plate_KBS(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementCircle, true);
                                }
                                else
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate = new CConCom_Plate_KB(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, null, true);
                                    else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                        plate = new CConCom_Plate_KB(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                    else//(vm.ScrewArrangementIndex == 2) // Circle
                                        plate = new CConCom_Plate_KB(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementCircle, true);
                                }
                            }
                            else if (vm.ComponentIndex == 2) // KC
                            {
                                if (bUseSimpleShapeOfPlates)
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate = new CConCom_Plate_KCS(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, null, true);
                                    else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                        plate = new CConCom_Plate_KCS(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                    else//(vm.ScrewArrangementIndex == 2) // Circle
                                        plate = new CConCom_Plate_KCS(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementCircle, true);
                                }
                                else
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate = new CConCom_Plate_KC(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, null, true);
                                    else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                        plate = new CConCom_Plate_KC(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                    else//(vm.ScrewArrangementIndex == 2) // Circle
                                        plate = new CConCom_Plate_KC(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementCircle, true);
                                }
                            }
                            else if (vm.ComponentIndex == 3) // KD
                            {
                                if (bUseSimpleShapeOfPlates)
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate = new CConCom_Plate_KDS(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, null, true);
                                    else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                        plate = new CConCom_Plate_KDS(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                    else//(vm.ScrewArrangementIndex == 2) // Circle
                                        plate = new CConCom_Plate_KDS(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementCircle, true);
                                }
                                else
                                {
                                    if (vm.ScrewArrangementIndex == 0) // Undefined
                                        plate = new CConCom_Plate_KD(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, null, true);
                                    else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                        plate = new CConCom_Plate_KD(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                    else//(vm.ScrewArrangementIndex == 2) // Circle
                                        plate = new CConCom_Plate_KD(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementCircle, true);
                                }
                            }
                            else if (vm.ComponentIndex == 4) // KES
                            {
                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_KES(dcomponents.arr_Serie_K_Names[4], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, null, true);
                                else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_KES(dcomponents.arr_Serie_K_Names[4], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_KES(dcomponents.arr_Serie_K_Names[4], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementCircle, true);
                            }
                            else if (vm.ComponentIndex == 5) // KFS
                            {
                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_KFS(dcomponents.arr_Serie_K_Names[5], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, null, true);
                                else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_KFS(dcomponents.arr_Serie_K_Names[5], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementRectangleKnee, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_KFS(dcomponents.arr_Serie_K_Names[5], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, screwArrangementCircle, true);
                            }
                            else // KK - TODO - screws are not implemented !!!
                            {
                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_KK(dcomponents.arr_Serie_K_Names[6], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, null, true);
                                else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_KK(dcomponents.arr_Serie_K_Names[6], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementRectangleKnee, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_KK(dcomponents.arr_Serie_K_Names[6], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true);
                            }
                            break;
                        }
                    case ESerieTypePlate.eSerie_M:
                        {
                            CScrewArrangement_M screwArrangement_M = new CScrewArrangement_M(iNumberofHoles, referenceScrew);
                            // b, h, t, iHoles, bBeam, slope_rad
                            plate = new CConCom_Plate_M(dcomponents.arr_Serie_M_Names[0], controlpoint, 0.5f*(fb- fb_B), 0.5f * (fb - fb_B), fh, ft, fb_B, fRoofPitch_rad, fGamma_rad, 0, 0, 0, screwArrangement_M, true); // M
                            break;
                        }
                    case ESerieTypePlate.eSerie_N:
                        {
                            CScrewArrangement_N screwArrangement_N = new CScrewArrangement_N(iNumberofHoles, referenceScrew);
                            plate = new CConCom_Plate_N(dcomponents.arr_Serie_N_Names[0], controlpoint, fb, fb2, fh, fl, ft, 0,0,0, screwArrangement_N, true); // N
                            break;
                        }
                    case ESerieTypePlate.eSerie_O:
                        {
                            CScrewArrangement_O screwArrangement_O = new CScrewArrangement_O(referenceScrew, 1, 10, 0.02f, 0.02f, 0.05f, 0.05f, 1, 10, 0.18f, 0.02f, 0.05f, 0.05f);

                            if (vm.ScrewArrangementIndex == 0) // Undefined
                                plate = new CConCom_Plate_O(dcomponents.arr_Serie_O_Names[0], controlpoint, fb, fb2, fh, fh2, ft, 11f * MathF.fPI / 180f, 0, 0, 0, null, true);
                            else //if (vm.ScrewArrangementIndex == 1) // Rectangular
                                plate = new CConCom_Plate_O(dcomponents.arr_Serie_O_Names[0], controlpoint, fb, fb2, fh, fh2, ft, 11f * MathF.fPI / 180f, 0, 0, 0, screwArrangement_O, true); // O
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
                vm.SetComponentProperties(plate);
                if (plate != null) vm.SetScrewArrangementProperties(plate.ScrewArrangement);
            }
            else
            {
                screw = new CScrew("TEK", sGauge_Screw);
                vm.SetComponentProperties(screw);
            }

            // Display Component
            DisplayComponent(vm);
        }

        private void BtnExportDXF_Click(object sender, RoutedEventArgs e)
        {
            SCVDisplayOptionsWindow displayOptionsWindow = new SCVDisplayOptionsWindow();
            displayOptionsWindow.Closed += DisplayOptionsWindow_Closed;
            displayOptionsWindow.ShowDialog();
        }

        private void DisplayOptionsWindow_Closed(object sender, EventArgs e)
        {
            SCVDisplayOptionsWindow w = sender as SCVDisplayOptionsWindow;
            SCVDisplayOptionsViewModel vm = w.DataContext as SCVDisplayOptionsViewModel;

            // Bug 396 - docasna oprava - Ondrej prosim o upravu funkcii pre zrkadlenie.
            // Nasledne odstranit klonovanie vstupneho objektu plate plate.GetClonedPlate() a skontrlovat ci to funguje spravne, mozno bude potrebne nejake zrkadlenia aktivovat
            // Spravne by sme CPlate podla mna nemali pre ucely vykreslenia klonovat, ale musime ustrazit, aby sme pri vykresleni alebo exporte
            // nezmenili jej povodne parametre

            Canvas dxfCanvas = Drawing2D.DrawRealPlateToCanvas(plate, vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawHoleCentreSymbol2D,
                        vm.DrawDrillingRoute2D, vm.DrawDimensions2D, vm.DrawMemberOutline2D, vm.DrawBendLines2D);
            //Canvas dxfCanvas = Drawing2D.DrawRealPlateToCanvas(plate, true, true, true, true, true, true, true, true, true);
            //Canvas dxfCanvas = new Canvas();
            //Drawing2D.DrawPlateToCanvas(plate, 400, 500, ref dxfCanvas, vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawHoleCentreSymbol2D,
            //            vm.DrawDrillingRoute2D, vm.DrawDimensions2D, vm.DrawMemberOutline2D, vm.DrawBendLines2D);
            CExportToDXF.ExportCanvas_DXF(dxfCanvas, 0, 0);
        }

        private void BtnExportDXF_3D_Click(object sender, RoutedEventArgs e)
        {
            CExportToDXF.ExportViewPort_DXF(page3D._trackport.ViewPort);
        }

        private void BtnExportCNC_Click(object sender, RoutedEventArgs e)
        {
            // Export of drilling route to the .nc files
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            if (vm.ComponentTypeIndex == 0)
            {
                MessageBox.Show("NC file export of Cross Section not implemented.");
                return;
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string folder = dialog.SelectedPath;
                        float fUnitFactor = 1000; // defined in m, exported in mm
                        if (plate.DrillingRoutePoints == null) { MessageBox.Show("Could not create NC file. Drilling route points not found. Only setup file is exported."); }

                        //Export Plate to NC = create Setup and Holes NC files
                        MessageBox.Show(CExportToNC.ExportPlateToNC(plate, fUnitFactor, folder));
                    }
                }
            }
            else
            {
                // Screw - not implemented
                MessageBox.Show("NC file export of screw not implemented.");
                return;
            }
        }

        private void BtnFindCNCPath_Click(object sender, RoutedEventArgs e)
        {
            List<System.Windows.Point> points = null;

            if (plate.ScrewArrangement != null) // Screw arrangmenet must exists
                points = plate.ScrewArrangement.HolesCentersPoints2D.ToList();
            else
            {
                MessageBox.Show("Screws are not defined.");
                return;
            }

            if (points == null || points.Count == 0)
            {
                MessageBox.Show("Drilling points are not defined for selected plate.");
                return;
            }

            // Calculate size of plate and width to height ratio to set size of "salesman" algorthim window
            double fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;
            Drawing2D.CalculateModelLimits(plate.ScrewArrangement.HolesCentersPoints2D, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            double fWidth = fTempMax_X - fTempMin_X;
            double fHeigth = fTempMax_Y - fTempMin_Y;
            double fHeightToWidthRatio = fHeigth / fWidth;

            // Add coordinates of drilling machine start point
            points.Insert(0, new System.Windows.Point(0, 0));

            TwoOpt.WindowRunSalesman w = new TwoOpt.WindowRunSalesman(points, fHeightToWidthRatio);
            TwoOpt.MainWindowViewModel viewModel = w.DataContext as TwoOpt.MainWindowViewModel;

            w.Show();
            w.Closing += SalesmanWindow_Closing;

            //Dialog mi nefunguje nejako
            //w.ShowDialog();
            //SalesmanWindow_Closing(null, null);
        }

        private void SalesmanWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TwoOpt.WindowRunSalesman w = sender as TwoOpt.WindowRunSalesman;
            TwoOpt.MainWindowViewModel viewModel = w.DataContext as TwoOpt.MainWindowViewModel;

            List<System.Windows.Point> PathPoints = new List<System.Windows.Point>(viewModel.RoutePoints.Count);
            for (int i = 0; i < viewModel.RoutePoints.Count; i++)
            {
                PathPoints.Add(viewModel.RoutePoints[viewModel._model._tour.GetCities()[i]]);
            }

            Frame2DWidth = Frame2D.ActualWidth;
            Frame2DHeight = Frame2D.ActualHeight;
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentTypeIndex == 1) //ma to vyznam iba pre Plates
            {
                // Set drilling route points
                vm.DrillingRoutePoints = PathPoints;
                // Enable button to display of CNC drilling file
                BtnShowCNCDrillingFile.IsEnabled = true;
                // Update plate data and display
                UpdateAndDisplayPlate();
                vm.SetComponentProperties(plate);
            }
        }

        private void BtnShowCNCSetupFile_Click(object sender, RoutedEventArgs e)
        {
            float fUnitFactor = 1000; // defined in m, exported in mm
            try
            {
                tabItemDoc.Visibility = Visibility.Visible;

                StringBuilder sb2 = CExportToNC.GetCNCFileContentForSetup(plate.PointsOut2D, fUnitFactor);
                Paragraph paragraph = new Paragraph();
                paragraph.FontSize = 14;
                paragraph.FontFamily = new System.Windows.Media.FontFamily("Consolas");
                paragraph.Inlines.Add(sb2.ToString());
                FlowDocument document = new FlowDocument(paragraph);
                FlowDocViewer.Document = document;
                tabItemDoc.Focus();
            }
            catch (Exception)
            {
                MessageBox.Show("Error occurs. Check geometry input.");
            }
        }

        private void BtnShowCNCDrillingFile_Click(object sender, RoutedEventArgs e)
        {
            float fUnitFactor = 1000; // defined in m, exported in mm
            try
            {
                if (plate.DrillingRoutePoints == null) { MessageBox.Show("Drilling points are not defined."); return; }
                tabItemDoc.Visibility = Visibility.Visible;

                StringBuilder sb1 = CExportToNC.GetCNCFileContentForHoles(plate.DrillingRoutePoints, plate.Ft, fUnitFactor);
                Paragraph paragraph = new Paragraph();
                paragraph.FontSize = 14;
                paragraph.FontFamily = new System.Windows.Media.FontFamily("Consolas");
                paragraph.Inlines.Add(sb1.ToString());
                FlowDocument document = new FlowDocument(paragraph);
                FlowDocViewer.Document = document;
                tabItemDoc.Focus();
            }
            catch (Exception)
            {
                MessageBox.Show("Error occurs. Check geometry input.");
            }
        }

        private void DataGridScrewArrangement_ValueChanged(CComponentParamsView item)
        {
            float fLengthUnitFactor = 1000; // GUI input in mm, change to m used in source code

            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentTypeIndex == 1) // Only plates
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // TODO No. 63

                // Set current screw arrangement parameters
                if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangementCircleApexOrKnee)
                {
                    CScrewArrangementCircleApexOrKnee arrangementTemp = (CScrewArrangementCircleApexOrKnee)plate.ScrewArrangement;
                    if (item is CComponentParamsViewString)
                    {
                        CComponentParamsViewString itemStr = item as CComponentParamsViewString;
                        if (string.IsNullOrEmpty(itemStr.Value)) return;

                        if (item.Name.Equals(CParamsResources.CrscDepthS.Name)) arrangementTemp.FCrscRafterDepth = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.CrscWebStraightDepthS.Name)) arrangementTemp.FCrscWebStraightDepth = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.CrscWebMiddleStiffenerSizeS.Name)) arrangementTemp.FStiffenerSize = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        // Circle screws
                        // Changed number of circles
                        if (item.Name.Equals(CParamsResources.NumberOfCirclesInGroupS.Name))
                        {
                            int numberOfCirclesInGroup = int.Parse(itemStr.Value);
                            arrangementTemp.NumberOfCirclesInGroup_Updated(numberOfCirclesInGroup);
                            vm.SetScrewArrangementProperties(arrangementTemp);
                        }

                        // Changed number of screws in circle
                        if (item.Name.Contains(CParamsResources.NumberOfScrewsInCircleSequenceS.Name + " "))
                        {
                            int circleNum = int.Parse(item.Name.Substring((CParamsResources.NumberOfScrewsInCircleSequenceS.Name + " ").Length));                            
                            CPlateHelper.UpdateCircleSequencesNumberOfScrews(circleNum, itemStr, ref arrangementTemp);
                        }
                        // Changed Radius
                        if (item.Name.Contains(CParamsResources.RadiusOfScrewsInCircleSequenceS.Name + " "))
                        {
                            int circleNum = int.Parse(item.Name.Substring((CParamsResources.RadiusOfScrewsInCircleSequenceS.Name + " ").Length));
                            CPlateHelper.UpdateCircleSequencesRadius(circleNum, fLengthUnitFactor, itemStr, ref arrangementTemp);
                        }

                        // Corner screws
                        if (item.Name.Equals(CParamsResources.PositionOfCornerSequence_xS.Name)) arrangementTemp.FPositionOfCornerSequence_x = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.PositionOfCornerSequence_yS.Name)) arrangementTemp.FPositionOfCornerSequence_y = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.DistanceOfAdditionalScrewsInxS.Name)) arrangementTemp.FAdditionalScrewsDistance_x = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.DistanceOfAdditionalScrewsInyS.Name)) arrangementTemp.FAdditionalScrewsDistance_y = float.Parse(itemStr.Value) / fLengthUnitFactor;
                    }
                    else if (item is CComponentParamsViewBool)
                    {
                        CComponentParamsViewBool itemBool = item as CComponentParamsViewBool;
                        if (item.Name.Equals(CParamsResources.UseAdditionalCornerScrewsS.Name))
                        {
                            arrangementTemp.BUseAdditionalCornerScrews = itemBool.Value;
                        }
                    }
                    else if (item is CComponentParamsViewList)
                    {
                        CComponentParamsViewList itemList = item as CComponentParamsViewList;
                        if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                        if (item.Name.Equals(CParamsResources.NumberOfAdditionalScrewsInCornerS.Name)) arrangementTemp.IAdditionalConnectorInCornerNumber = int.Parse(itemList.Value);
                    }

                    arrangementTemp.UpdateArrangmentData();         // Update data of screw arrangement
                    plate.ScrewArrangement = arrangementTemp;       // Set current screw arrangement to the plate
                }
                else if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangementRectApexOrKnee)
                {
                    CScrewArrangementRectApexOrKnee arrangementTemp = (CScrewArrangementRectApexOrKnee)plate.ScrewArrangement;

                    if (item is CComponentParamsViewString)
                    {
                        CComponentParamsViewString itemStr = item as CComponentParamsViewString;
                        if (string.IsNullOrEmpty(itemStr.Value)) return;

                        if (item.Name.Equals(CParamsResources.CrscDepthS.Name)) arrangementTemp.FCrscRafterDepth = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.CrscWebStraightDepthS.Name)) arrangementTemp.FCrscWebStraightDepth = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.CrscWebMiddleStiffenerSizeS.Name)) arrangementTemp.FStiffenerSize = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        // TODO - Ondrej, TODO No. 105
                        // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
                        // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
                        // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                        if (item.Name == "Number of screws in row SQ1") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ1 = int.Parse(itemStr.Value);
                        if (item.Name == "Number of screws in column SQ1") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ1 = int.Parse(itemStr.Value);
                        if (item.Name == "Inserting point coordinate x SQ1") arrangementTemp.fx_c_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ1") arrangementTemp.fy_c_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ1") arrangementTemp.fDistanceOfPointsX_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ1") arrangementTemp.fDistanceOfPointsY_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        if (item.Name == "Number of screws in row SQ2") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ2 = int.Parse(itemStr.Value);
                        if (item.Name == "Number of screws in column SQ2") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ2 = int.Parse(itemStr.Value);
                        if (item.Name == "Inserting point coordinate x SQ2") arrangementTemp.fx_c_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ2") arrangementTemp.fy_c_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ2") arrangementTemp.fDistanceOfPointsX_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ2") arrangementTemp.fDistanceOfPointsY_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        if (item.Name == "Number of screws in row SQ3") { int num = 0; int.TryParse(itemStr.Value, out num); if (num > 0) arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ3 = num; else itemStr.Value = arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ3.ToString(); }
                        if (item.Name == "Number of screws in column SQ3") { int num = 0; int.TryParse(itemStr.Value, out num); if (num > 0) arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ3 = num; else itemStr.Value = arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ3.ToString(); }
                        if (item.Name == "Inserting point coordinate x SQ3") arrangementTemp.fx_c_SQ3 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ3") arrangementTemp.fy_c_SQ3 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ3") arrangementTemp.fDistanceOfPointsX_SQ3 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ3") arrangementTemp.fDistanceOfPointsY_SQ3 = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        if (item.Name == "Number of screws in row SQ4") { int num = 0; int.TryParse(itemStr.Value, out num); if (num > 0) arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ4 = num; else itemStr.Value = arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ4.ToString(); }
                        if (item.Name == "Number of screws in column SQ4") { int num = 0; int.TryParse(itemStr.Value, out num); if (num > 0) arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ4 = num; else itemStr.Value = arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ4.ToString(); }
                        if (item.Name == "Inserting point coordinate x SQ4") arrangementTemp.fx_c_SQ4 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ4") arrangementTemp.fy_c_SQ4 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ4") arrangementTemp.fDistanceOfPointsX_SQ4 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ4") arrangementTemp.fDistanceOfPointsY_SQ4 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                    }
                    else if (item is CComponentParamsViewList)
                    {
                        CComponentParamsViewList itemList = item as CComponentParamsViewList;
                        if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                    }

                    arrangementTemp.UpdateArrangmentData();        // Update data of screw arrangement
                    plate.ScrewArrangement = arrangementTemp;      // Set current screw arrangement to the plate
                }
                else if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangement_BX_1)
                {
                    CScrewArrangement_BX_1 arrangementTemp = (CScrewArrangement_BX_1)plate.ScrewArrangement;

                    if (item is CComponentParamsViewString)
                    {
                        CComponentParamsViewString itemStr = item as CComponentParamsViewString;
                        if (string.IsNullOrEmpty(itemStr.Value)) return;

                        if (item.Name.Equals(CParamsResources.CrscDepthS.Name)) arrangementTemp.FCrscColumnDepth = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.CrscWebStraightDepthS.Name)) arrangementTemp.FCrscWebStraightDepth = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.CrscWebMiddleStiffenerSizeS.Name)) arrangementTemp.FStiffenerSize = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        // TODO - Ondrej, TODO No. 105
                        // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
                        // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
                        // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                        if (item.Name == "Number of screws in row SQ1") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ1 = int.Parse(itemStr.Value);
                        if (item.Name == "Number of screws in column SQ1") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ1 = int.Parse(itemStr.Value);
                        if (item.Name == "Inserting point coordinate x SQ1") arrangementTemp.fx_c_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ1") arrangementTemp.fy_c_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ1") arrangementTemp.fDistanceOfPointsX_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ1") arrangementTemp.fDistanceOfPointsY_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        if (item.Name == "Number of screws in row SQ2") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ2 = int.Parse(itemStr.Value);
                        if (item.Name == "Number of screws in column SQ2") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ2 = int.Parse(itemStr.Value);
                        if (item.Name == "Inserting point coordinate x SQ2") arrangementTemp.fx_c_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ2") arrangementTemp.fy_c_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ2") arrangementTemp.fDistanceOfPointsX_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ2") arrangementTemp.fDistanceOfPointsY_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        if (item.Name == "Number of screws in row SQ3") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ3 = int.Parse(itemStr.Value);
                        if (item.Name == "Number of screws in column SQ3") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ3 = int.Parse(itemStr.Value);
                        if (item.Name == "Inserting point coordinate x SQ3") arrangementTemp.fx_c_SQ3 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ3") arrangementTemp.fy_c_SQ3 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ3") arrangementTemp.fDistanceOfPointsX_SQ3 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ3") arrangementTemp.fDistanceOfPointsY_SQ3 = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        if (item.Name == "Number of screws in row SQ4") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ4 = int.Parse(itemStr.Value);
                        if (item.Name == "Number of screws in column SQ4") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ4 = int.Parse(itemStr.Value);
                        if (item.Name == "Inserting point coordinate x SQ4") arrangementTemp.fx_c_SQ4 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ4") arrangementTemp.fy_c_SQ4 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ4") arrangementTemp.fDistanceOfPointsX_SQ4 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ4") arrangementTemp.fDistanceOfPointsY_SQ4 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                    }
                    else if (item is CComponentParamsViewList)
                    {
                        CComponentParamsViewList itemList = item as CComponentParamsViewList;
                        if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                    }

                    arrangementTemp.UpdateArrangmentData();        // Update data of screw arrangement
                    plate.ScrewArrangement = arrangementTemp;      // Set current screw arrangement to the plate
                }
                else if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangement_O)
                {
                    CScrewArrangement_O arrangementTemp = (CScrewArrangement_O)plate.ScrewArrangement;

                    if (item is CComponentParamsViewString)
                    {
                        CComponentParamsViewString itemStr = item as CComponentParamsViewString;
                        if (string.IsNullOrEmpty(itemStr.Value)) return;

                        if (item.Name == "Number of screws in row SQ1") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ1 = int.Parse(itemStr.Value);
                        if (item.Name == "Number of screws in column SQ1") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ1 = int.Parse(itemStr.Value);
                        if (item.Name == "Inserting point coordinate x SQ1") arrangementTemp.fx_c_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ1") arrangementTemp.fy_c_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ1") arrangementTemp.fDistanceOfPointsX_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ1") arrangementTemp.fDistanceOfPointsY_SQ1 = float.Parse(itemStr.Value) / fLengthUnitFactor;

                        if (item.Name == "Number of screws in row SQ2") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ2 = int.Parse(itemStr.Value);
                        if (item.Name == "Number of screws in column SQ2") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ2 = int.Parse(itemStr.Value);
                        if (item.Name == "Inserting point coordinate x SQ2") arrangementTemp.fx_c_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Inserting point coordinate y SQ2") arrangementTemp.fy_c_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws x SQ2") arrangementTemp.fDistanceOfPointsX_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name == "Distance between screws y SQ2") arrangementTemp.fDistanceOfPointsY_SQ2 = float.Parse(itemStr.Value) / fLengthUnitFactor;
                    }
                    else if (item is CComponentParamsViewList)
                    {
                        CComponentParamsViewList itemList = item as CComponentParamsViewList;
                        if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                    }

                    arrangementTemp.UpdateArrangmentData();        // Update data of screw arrangement
                    plate.ScrewArrangement = arrangementTemp;      // Set current screw arrangement to the plate
                }
                else
                {
                    // Screw arrangement is not implemented
                }

                // Delete drilling route
                vm.DrillingRoutePoints = null;
                // Redraw plate in 2D and 3D
                UpdateAndDisplayPlate();
            }
        }

        //private void UpdateCircleSequencesNumberOfScrews(int iCircleNumberInGroup, CComponentParamsViewString itemNewValueString, ref CScrewArrangementCircleApexOrKnee arrangementTemp)
        //{
        //    int numberOfScrews = int.Parse(itemNewValueString.Value);
        //    if (numberOfScrews < 2) return; // Validacia - pocet skrutiek v kruhu musi byt min 2, inak ignorovat

        //    // Change each group
        //    foreach (CScrewSequenceGroup gr in arrangementTemp.ListOfSequenceGroups)
        //    {
        //        IEnumerable<CConnectorSequence> halfCircleSequences = (IEnumerable<CConnectorSequence>)gr.ListSequence.Where(s => s is CScrewHalfCircleSequence);
        //        CConnectorSequence seq = null;
        //        seq = halfCircleSequences.ElementAtOrDefault((iCircleNumberInGroup - 1) * 2); //1.half of circle
        //        if (seq != null) seq.INumberOfConnectors = numberOfScrews;
        //        seq = halfCircleSequences.ElementAtOrDefault((iCircleNumberInGroup - 1) * 2 + 1); //2.half of circle
        //        if (seq != null) seq.INumberOfConnectors = numberOfScrews;
        //    }
        //    // Recalculate total number of screws in the arrangement
        //    arrangementTemp.RecalculateTotalNumberOfScrews();
        //}

        //private void UpdateCircleSequencesRadius(int iCircleNumberInGroup, float fLengthUnitFactor, CComponentParamsViewString itemNewValueString, ref CScrewArrangementCircleApexOrKnee arrangementTemp)
        //{
        //    float radius = (float.Parse(itemNewValueString.Value) / fLengthUnitFactor);
        //    if (!IsValidCircleRadius(radius, arrangementTemp)) throw new Exception("Radius is not valid.");  //if radius is not valid => return
        //    // Change each group
        //    foreach (CScrewSequenceGroup gr in arrangementTemp.ListOfSequenceGroups)
        //    {
        //        IEnumerable<CConnectorSequence> halfCircleSequences = (IEnumerable<CConnectorSequence>)gr.ListSequence.Where(s => s is CScrewHalfCircleSequence); // Bug - To Ondrej tu je nejaka chyba v pretypovani, ja som to kedysi menil, aby mi to slo prelozit ale ... :)
        //        CConnectorSequence seq = null;
        //        seq = halfCircleSequences.ElementAtOrDefault((iCircleNumberInGroup - 1) * 2); //1.half of circle
        //        if (seq != null) ((CScrewHalfCircleSequence)seq).Radius = radius;
        //        seq = halfCircleSequences.ElementAtOrDefault((iCircleNumberInGroup - 1) * 2 + 1); //2.half of circle
        //        if (seq != null) ((CScrewHalfCircleSequence)seq).Radius = radius;
        //    }
        //}

        //private bool IsValidCircleRadius(float radius, CScrewArrangementCircleApexOrKnee arrangementTemp)
        //{
        //    float fAdditionalMargin = 0.02f; // TODO - napojit na GUI, napojit na generovanie screw arrangement - vid Circle Arrangement Get_ScrewGroup_IncludingAdditionalScrews
        //    if (radius > 0.5 * arrangementTemp.FStiffenerSize + fAdditionalMargin) return true;
        //    else return false;
        //}

        private void DataGridGeometry_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string changedText = ((TextBox)e.EditingElement).Text;
            CComponentParamsViewString item = ((CComponentParamsViewString)e.Row.Item);
            if (changedText == item.Value) return;

            float fLengthUnitFactor = 1000; // GUI input in mm, change to m used in source code
            float fDegToRadianFactor = 180f / MathF.fPI;

            bool bUseRoofSlope = true;

            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            if (vm.ComponentTypeIndex == 0)
            {
                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawDimensions2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                // Set current basic geometry of plate
                if (plate is CConCom_Plate_B_basic)
                {
                    CConCom_Plate_B_basic plateTemp = (CConCom_Plate_B_basic)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fb_X = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeightS.Name)) plateTemp.Fh_Y = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_JA)
                {
                    CConCom_Plate_JA plateTemp = (CConCom_Plate_JA)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fb_X = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_JB || plate is CConCom_Plate_JBS)
                {
                    CConCom_Plate_JB plateTemp = (CConCom_Plate_JB)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fb_X = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_JCS)
                {
                    CConCom_Plate_JCS plateTemp = (CConCom_Plate_JCS)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fw_apexHalfLength = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.CrscDepthS.Name)) plateTemp.Fd_crsc = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        // Not implemented !!!
                        //if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KA)
                {
                    CConCom_Plate_KA plateTemp = (CConCom_Plate_KA)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KB || plate is CConCom_Plate_KBS)
                {
                    CConCom_Plate_KB plateTemp = (CConCom_Plate_KB)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KC || plate is CConCom_Plate_KCS)
                {
                    CConCom_Plate_KC plateTemp = (CConCom_Plate_KC)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KD || plate is CConCom_Plate_KDS)
                {
                    CConCom_Plate_KD plateTemp = (CConCom_Plate_KD)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KES)
                {
                    CConCom_Plate_KES plateTemp = (CConCom_Plate_KES)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KFS)
                {
                    CConCom_Plate_KFS plateTemp = (CConCom_Plate_KFS)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KK) // Nepouzivat, kym nebude zobecnene screw arrangement
                {
                    CConCom_Plate_KK plateTemp = (CConCom_Plate_KK)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;

                    if (bUseRoofSlope)
                    {
                        if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;
                    }
                    else
                    {
                        if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    }

                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    if (item.Name.Equals(CParamsResources.RafterWidthS.Name)) plateTemp.Fb_XR = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_O)
                {
                    CConCom_Plate_O plateTemp = (CConCom_Plate_O)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.RafterWidthS.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor; // Oznacene ako BR ale premenna je bX2
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;

                    if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = float.Parse(changedText) / fDegToRadianFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else
                {
                    // Plate is not implemented
                }

                //Delete drilling route
                vm.DrillingRoutePoints = null;
                plate.DrillingRoutePoints = null;
                // Redraw plate in 2D and 3D
                DisplayPlate(true);

                //Update ComponentDetails Datagrid
                vm.SetComponentProperties(plate);
            }
            else // Screw
            {
                bool bDrawCentreSymbol = true;
                Drawing2D.DrawScrewToCanvas(screw, Frame2DWidth, Frame2DHeight, ref page2D, bDrawCentreSymbol);
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedIndex == 0 && Combobox_Type.SelectedIndex != 2)
            {
                if (Combobox_Type.SelectedIndex != 2)
                {
                    // Display only for cross-sections and plates (for 2D view of screw display options and transformations are not implemented yet)
                    panelOptions2D.Visibility = Visibility.Visible;
                    panelOptionsTransform2D.Visibility = Visibility.Visible;
                    panelOptions3D.Visibility = Visibility.Hidden;

                    // Bug No 96 - prekreslit plech - TODO - Ondrej - ma to tu byt alebo to ma reagovat v OnWindowSizeChanged ???
                    // Ak je okno defaultne a som 2D, prepnem na 3D, maximalizujem okno a prepnem na 2D tak sa sem nacitaju hodnoty z defaultnej velkosti, nie z maximalizovanej
                    //Frame2DWidth = Frame2D.ActualWidth;
                    //Frame2DHeight = Frame2D.ActualHeight;
                    //RedrawComponentIn2D();
                    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
                    DisplayComponent(vm);
                }
                else
                {
                    panelOptions2D.Visibility = Visibility.Hidden;
                    panelOptionsTransform2D.Visibility = Visibility.Hidden;
                    panelOptions3D.Visibility = Visibility.Hidden;
                }
            }
            else if (MainTabControl.SelectedIndex == 1)
            {
                panelOptions2D.Visibility = Visibility.Hidden;
                panelOptionsTransform2D.Visibility = Visibility.Hidden;
                panelOptions3D.Visibility = Visibility.Visible;
            }
            else if (MainTabControl.SelectedIndex == 2)
            {
                panelOptions2D.Visibility = Visibility.Hidden;
                panelOptionsTransform2D.Visibility = Visibility.Hidden;
                panelOptions3D.Visibility = Visibility.Hidden;
            }
        }

        private void BtnExportToPDF_Click(object sender, RoutedEventArgs e)
        {
            WaitWindow ww = new WaitWindow("PDF");
            ww.Show();

            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            List<string[]> list = new List<string[]>();
            foreach (CComponentParamsView o in vm.ComponentGeometry)
            {
                if (o is CComponentParamsViewString)
                {
                    CComponentParamsViewString oStr = o as CComponentParamsViewString;
                    list.Add(new string[] { o.Name, o.ShortCut, oStr.Value, o.Unit });
                }
            }
            foreach (CComponentParamsView o in vm.ComponentDetails)
            {
                if (o is CComponentParamsViewString)
                {
                    CComponentParamsViewString oStr = o as CComponentParamsViewString;
                    list.Add(new string[] { o.Name, o.ShortCut, oStr.Value, o.Unit });
                }
            }
            CProductionInfo pInfo = new CProductionInfo(vm.JobNumber, vm.Customer, vm.Amount, vm.AmountRH, vm.AmountLH);

            if (Frame2D.Content is Canvas) CExportToPDF.CreatePDFFileForPlate(Frame2D.Content as Canvas, list, plate, pInfo);
            else MessageBox.Show("Exporting to PDF is not possible because 2D view does not contain required image.");
            
            ww.Close();
        }

        public static void ForceUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
        }

        public void ShowMessageBoxInWindow(string text)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(text);
            });
        }

        //SplashScreen splashScreen = null;
        //public void ShowWaitScreen()
        //{
        //    splashScreen = new SplashScreen("loading2.gif");
        //    splashScreen.Show(false);
        //}
        //public void HideWaitScreen()
        //{
        //    splashScreen.Close(TimeSpan.FromSeconds(0.1));
        //}

        private void BtnSavePlate_Click(object sender, RoutedEventArgs e)
        {
            if (plate == null) { MessageBox.Show("No plate to serialize."); return; }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Data Files (*.scw)|*.scw";
            sfd.DefaultExt = "scw";
            sfd.AddExtension = true;
            sfd.FileName = "Plate_" + plate.Name;

            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            CProductionInfo pInfo = new CProductionInfo(vm.JobNumber, vm.Customer, vm.Amount, vm.AmountRH, vm.AmountLH);

            object[] arr = new object[2];
            arr[0] = plate;
            arr[1] = pInfo;

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = File.Open(sfd.FileName, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, arr);
                    stream.Close();
                }
            }
        }

        private void BtnLoadPlate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Data Files (*.scw)|*.scw";
            ofd.DefaultExt = "scw";
            ofd.AddExtension = true;
            
            if (ofd.ShowDialog() == true)
            {
                OpenDataFile(ofd.FileName);
            }
        }

        private int GetPlateIndex(CPlate plate)
        {
            // TODO Ondrej - da sa na tieto indexy vymysliet nieco krajsie :)
            //Type plateObjectClassType = plate.GetType(); // pre porovnanie plate ak plate A dedi od plate B , akurat neviem co s tym

            if (plate is CConCom_Plate_JB || plate is CConCom_Plate_JBS ||
                plate is CConCom_Plate_KB || plate is CConCom_Plate_KBS) return 1;
            else if (plate is CConCom_Plate_JCS ||
                plate is CConCom_Plate_KC || plate is CConCom_Plate_KCS) return 2;
            else if (plate is CConCom_Plate_KD || plate is CConCom_Plate_KDS) return 3;
            else if (plate is CConCom_Plate_KES) return 4;
            else if (plate is CConCom_Plate_KFS) return 5;
            else if (plate is CConCom_Plate_KK) return 6;
            else return 0; // JA, KA, O
        }

        private void BtnExportToPDFFromDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WaitWindow ww = new WaitWindow("PDF");
                    ww.Show();

                    string folder = dialog.SelectedPath;
                    DirectoryInfo dirInfo = new DirectoryInfo(folder);
                    FileInfo[] files = dirInfo.GetFiles("*.scw", SearchOption.TopDirectoryOnly);
                    if (files.Length == 0) { MessageBox.Show("No .scw files in the directory."); return; }

                    List<string[]> tableParams = new List<string[]>();

                    //header row
                    tableParams.Add(new string[] {
                        "ID",
                        "Name",
                        "Width [m]",
                        "Height [m]",
                        "Thickness [mm]",
                        "Area [m²]",
                        "Volume [m³]",
                        "Mass [kg]",
                        "Amount",
                        "Amount Left",
                        "Amount Right",
                        "Mass Total [kg]",
                        "Screws Plate",
                        "Screws Total"});

                    CExportToPDF.CreatePDFDocument();
                    int count = 0;
                    int totalAmount = 0;
                    float totalMass = 0;
                    int totalScrews = 0;

                    foreach (FileInfo fi in files)
                    {
                        OpenDataFile(fi.FullName);

                        SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
                        CProductionInfo pInfo = new CProductionInfo(vm.JobNumber, vm.Customer, vm.Amount, vm.AmountRH, vm.AmountLH);

                        // TO Ondrej, neviem ci je to chyba alebo zamer ale mam pocit ze pre plate neukladame vsetky property z production info
                        // pInfo.JobNumber
                        // Info.Customer
                        // v Open File to nacitavame ak to nie je null, ale vyzera to tak ze tu nikde tieto parametre neukladame

                        //page2D.RenderSize = new System.Windows.Size(((Canvas)Frame2D.Content).RenderSize.Width, ((Canvas)Frame2D.Content).RenderSize.Height);
                        //if (Frame2D.Content is Canvas) CExportToPDF.AddPlateToPDF(Frame2D.Content as Canvas, plate, pInfo);
                        if (page2D == null) { MessageBox.Show("Exporting to PDF is not possible because 2D view does not contain required image. " + fi.Name); return; }

                        ForceUIToUpdate();
                        //CExportToPDF.AddPlateToPDF(page2D, plate, pInfo);
                        //CExportToPDF.AddPlateToPDF(page2D, ((Canvas)Frame2D.Content).RenderSize.Width, plate, pInfo);
                        CExportToPDF.AddPlateToPDF(((Canvas)Frame2D.Content), ((Canvas)Frame2D.Content).RenderSize.Width, plate, pInfo);

                        float fUnitFactor_Length_m_to_mm = 1000f;

                        count++;
                        string[] plateParams = new string[14];
                        plateParams[0] = count.ToString();
                        plateParams[1] = plate.Name;
                        plateParams[2] = Math.Round(plate.Width_bx,3).ToString();
                        plateParams[3] = Math.Round(plate.Height_hy,3).ToString();
                        plateParams[4] = Math.Round(plate.Ft * fUnitFactor_Length_m_to_mm, 2).ToString();
                        plateParams[5] = Math.Round(plate.fArea, 3).ToString();
                        plateParams[6] = Math.Round(plate.fVolume, 4).ToString();

                        if (plate.fMass < 0)
                            plate.fMass = plate.fVolume * 7850f; // Pre pripad, ze nebol spravne inicializovany objekt materialu plechu

                        plateParams[7] = Math.Round(plate.fMass, 1).ToString();
                        plateParams[8] = pInfo.Amount.ToString();
                        totalAmount += pInfo.Amount;
                        if (plate.IsSymmetric())
                        {
                            plateParams[9] = string.Empty;
                            plateParams[10] = string.Empty;
                        }
                        else
                        {
                            plateParams[9] = pInfo.AmountLH.ToString();
                            plateParams[10] = pInfo.AmountRH.ToString();
                        }

                        plateParams[11] = Math.Round(plate.fMass * pInfo.Amount, 1).ToString();
                        totalMass += (plate.fMass * pInfo.Amount);
                        // Screws
                        if (plate.ScrewArrangement != null)
                        {
                            plateParams[12] = plate.ScrewArrangement.IHolesNumber.ToString();
                            plateParams[13] = (plate.ScrewArrangement.IHolesNumber * pInfo.Amount).ToString();
                            totalScrews += (plate.ScrewArrangement.IHolesNumber * pInfo.Amount);
                        }
                        else
                        {
                            plateParams[12] = "0";
                            plateParams[13] = "0";
                        }
                        tableParams.Add(plateParams);
                    }

                    //last total count row
                    tableParams.Add(new string[] {"","","","","","","Total", "Σ", totalAmount.ToString(),"", "Σ", totalMass.ToString("F1"), "Σ", totalScrews.ToString()});

                    CExportToPDF.AddPlatesParamsTableToDocumentOnNewPage(tableParams);

                    string fileName = string.Format("{0}\\{1}", folder, "ExportAllPlatesInFolder.pdf");
                    string fileNameXLSX = string.Format("{0}\\{1}", folder, "ExportAllPlatesInFolder.xlsx");
                    ExportToExcelDocument.ExportToExcel(fileNameXLSX, tableParams, "plates");
                    CExportToPDF.SavePDFDocument(fileName);
                    ww.Close();
                }
            }
        }

        private void OpenDataFile(string fileName)
        {   
            CPlate deserializedPlate = null;
            CProductionInfo pInfo = null;
            using (Stream stream = File.Open(fileName, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                object[] arr = (object[])binaryFormatter.Deserialize(stream);
                deserializedPlate = (CPlate)arr[0];
                pInfo = (CProductionInfo)arr[1];
            }

            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            if (pInfo != null)
            {
                vm.JobNumber = pInfo.JobNumber;
                vm.Customer = pInfo.Customer;
                vm.Amount = pInfo.Amount;
                vm.AmountLH = pInfo.AmountLH;
                vm.AmountRH = pInfo.AmountRH;
            }
            if (deserializedPlate != null)
            {
                vm.ComponentTypeIndex = 1;
                vm.ComponentSerieIndex = (int)deserializedPlate.m_ePlateSerieType_FS;
                vm.ComponentIndex = GetPlateIndex(deserializedPlate);

                //TODO Mato: tu by trebalo ponastavovat vsetky dolezite premenne,ktore chceme preniest, lebo ked to preniesiem tak ako je na riadku dole, tak to potom neskor pada
                // samozrejme preto,ze nie je mozne serializovat vsetko ako je napr. Material, 3DTriedy ktore nie su urcene na serializaciu atd

                if (deserializedPlate.ScrewArrangement is CScrewArrangementCircleApexOrKnee) vm.ScrewArrangementIndex = 2;
                else if (deserializedPlate.ScrewArrangement is CScrewArrangementRectApexOrKnee) vm.ScrewArrangementIndex = 1;
                else if (deserializedPlate.ScrewArrangement is CScrewArrangement_O) vm.ScrewArrangementIndex = 1;
                else vm.ScrewArrangementIndex = 0;

                plate = deserializedPlate;
                vm.SetComponentProperties(plate);
                if (plate != null) vm.SetScrewArrangementProperties(plate.ScrewArrangement);

                DisplayComponent(vm);
                
                this.Title = $"System Component Viewer - {Path.GetFileName(fileName)}";
            }
        }

        private void BtnExportToXLSFromDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WaitWindow ww = new WaitWindow("XLS");
                    ww.Show();

                    string folder = dialog.SelectedPath;
                    DirectoryInfo dirInfo = new DirectoryInfo(folder);
                    FileInfo[] files = dirInfo.GetFiles("*.scw", SearchOption.TopDirectoryOnly);
                    if (files.Length == 0) { MessageBox.Show("No .scw files in the directory."); return; }

                    List<string[]> tableParams = new List<string[]>();

                    //header row
                    tableParams.Add(new string[] {
                        "ID",
                        "Name",
                        "Width [m]",
                        "Height [m]",
                        "Thickness [mm]",
                        "Area [m²]",
                        "Volume [m³]",
                        "Mass [kg]",
                        "Amount",
                        "Amount Left",
                        "Amount Right",
                        "Mass Total [kg]",
                        "Screws Plate",
                        "Screws Total"});

                    CExportToPDF.CreatePDFDocument();
                    int count = 0;
                    int totalAmount = 0;
                    float totalMass = 0;
                    int totalScrews = 0;

                    foreach (FileInfo fi in files)
                    {
                        OpenDataFile(fi.FullName);

                        SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
                        CProductionInfo pInfo = new CProductionInfo(vm.JobNumber, vm.Customer, vm.Amount, vm.AmountRH, vm.AmountLH);
                        
                        float fUnitFactor_Length_m_to_mm = 1000f;

                        count++;
                        string[] plateParams = new string[14];
                        plateParams[0] = count.ToString();
                        plateParams[1] = plate.Name;
                        plateParams[2] = Math.Round(plate.Width_bx, 3).ToString();
                        plateParams[3] = Math.Round(plate.Height_hy, 3).ToString();
                        plateParams[4] = Math.Round(plate.Ft * fUnitFactor_Length_m_to_mm, 2).ToString();
                        plateParams[5] = Math.Round(plate.fArea, 3).ToString();
                        plateParams[6] = Math.Round(plate.fVolume, 4).ToString();
                        plateParams[7] = Math.Round(plate.fMass, 1).ToString();
                        plateParams[8] = pInfo.Amount.ToString();
                        totalAmount += pInfo.Amount;
                        if (plate.IsSymmetric())
                        {
                            plateParams[9] = string.Empty;
                            plateParams[10] = string.Empty;
                        }
                        else
                        {
                            plateParams[9] = pInfo.AmountLH.ToString();
                            plateParams[10] = pInfo.AmountRH.ToString();
                        }

                        plateParams[11] = Math.Round(plate.fMass * pInfo.Amount, 1).ToString();
                        totalMass += (plate.fMass * pInfo.Amount);
                        // Screws
                        if (plate.ScrewArrangement != null)
                        {
                            plateParams[12] = plate.ScrewArrangement.IHolesNumber.ToString();
                            plateParams[13] = (plate.ScrewArrangement.IHolesNumber * pInfo.Amount).ToString();
                            totalScrews += (plate.ScrewArrangement.IHolesNumber * pInfo.Amount);
                        }
                        else
                        {
                            plateParams[12] = "0";
                            plateParams[13] = "0";
                        }
                        tableParams.Add(plateParams);
                    }

                    //last total count row
                    tableParams.Add(new string[] { "", "", "", "", "", "", "Total", "Σ", totalAmount.ToString(), "", "Σ", totalMass.ToString("F1"), "Σ", totalScrews.ToString() });
                    
                    string fileNameXLSX = string.Format("{0}\\{1}", folder, "ExportAllPlatesInFolder.xlsx");
                    ExportToExcelDocument.ExportToExcel(fileNameXLSX, tableParams, "plates");
                    
                    ww.Close();
                }
            }
        }

        //private void RedrawComponentIn2D()
        //{
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

        //    if (vm.ComponentTypeIndex == 0)
        //    {
        //        if (vm.MirrorX) crsc.MirrorCRSCAboutX();
        //        if (vm.MirrorY) crsc.MirrorCRSCAboutY();
        //        if (vm.Rotate90CW) crsc.RotateCrsc_CW(90);
        //        if (vm.Rotate90CCW) crsc.RotateCrsc_CW(-90);

        //        Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
        //            vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
        //    }
        //    else if (vm.ComponentTypeIndex == 1)
        //    {
        //        if (vm.MirrorX) plate.MirrorPlateAboutX();
        //        if (vm.MirrorY) plate.MirrorPlateAboutY();
        //        if (vm.Rotate90CW) plate.RotatePlateAboutZ_CW(90);
        //        if (vm.Rotate90CCW) plate.RotatePlateAboutZ_CW(-90);
        //        if (vm.DrillingRoutePoints != null) plate.DrillingRoutePoints = vm.DrillingRoutePoints;

        //        // Redraw plate in 2D
        //        Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
        //            vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawHoleCentreSymbol2D, vm.DrawDrillingRoute2D);
        //    }
        //    else // Screw
        //    {
        //        bool bDrawCentreSymbol = true;
        //        Drawing2D.DrawScrewToCanvas(screw, Frame2DWidth, Frame2DHeight, ref page2D, bDrawCentreSymbol);
        //    }

        //    // Display plate in 2D preview frame
        //    Frame2D.Content = page2D;
        //}
    }
}