using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//using System.Windows.Shapes;
using BaseClasses;
using BaseClasses.GraphObj;
using MATH;
using CRSC;
using EXPIMP;
using System.Windows.Media.Media3D;
using System.Windows.Documents;
using System.Text;

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
        CPlate plate;
        CScrew screw;
        CPoint controlpoint = new CPoint(0, 0, 0, 0, 0);
        Color cComponentColor = Colors.Aquamarine; // Default
        float fb_R; // Rafter Width
        float fb; // in plane XY -X coord
        float fb2; // in plane XY - X coord
        float fh; // in plane XY - Y coord
        float fh2; // in plane XY - Y coord
        float fl; // out of plane - Z coord
        float fl2; // out of plane - Z coord
        float ft;
        float ft_f;
        float fb_fl; // Flange - Z-section
        float fc_lip1; // LIP - Z-section
        float fPitch_rad =  11f / 180f * (float)Math.PI; // Roof Pitch - default value (11 deg)
        int iNumberofHoles;

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
        }

        private void HandleComponentViewerPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            SystemComponentViewerViewModel vm = sender as SystemComponentViewerViewModel;
            if (vm != null && vm.IsSetFromCode) return;

            if (e.PropertyName == "ComponentIndex") UpdateAll();

            if(e.PropertyName == "DrawPoints2D" || e.PropertyName == "DrawOutLine2D" || e.PropertyName == "DrawPointNumbers2D" || 
                e.PropertyName == "DrawHoles2D" || e.PropertyName == "DrawDrillingRoute2D") UpdateAll();

        }

        private void LoadDataFromDatabase()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentIndex < 0) // Index is invalid - set default 0
                vm.ComponentIndex = 0;

                if (vm.ComponentTypeIndex == 0) // Cross-section
                {
                    switch ((ESerieTypeCrSc_FormSteel)vm.ComponentSerieIndex)
                    {
                        case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                            {
                                fb = dcomponents.arr_Serie_Box_FormSteel_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Box_FormSteel_Dimension[vm.ComponentIndex, 1] / 1000f;
                                ft = dcomponents.arr_Serie_Box_FormSteel_Dimension[vm.ComponentIndex, 2] / 1000f;
                                cComponentColor = dcomponents.arr_Serie_Box_FormSteel_Colors[vm.ComponentIndex];
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_Z:
                            {
                                fh = dcomponents.arr_Serie_Z_FormSteel_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fb_fl = dcomponents.arr_Serie_Z_FormSteel_Dimension[vm.ComponentIndex, 1] / 1000f;
                                fc_lip1 = dcomponents.arr_Serie_Z_FormSteel_Dimension[vm.ComponentIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_Z_FormSteel_Dimension[vm.ComponentIndex, 3] / 1000f;
                                cComponentColor = dcomponents.arr_Serie_Z_FormSteel_Colors[vm.ComponentIndex];
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                            {
                                fb = dcomponents.arr_Serie_C_FormSteel_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_C_FormSteel_Dimension[vm.ComponentIndex, 1] / 1000f;
                                ft = dcomponents.arr_Serie_C_FormSteel_Dimension[vm.ComponentIndex, 2] / 1000f;
                                cComponentColor = dcomponents.arr_Serie_C_FormSteel_Colors[vm.ComponentIndex];
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
                            {
                                fb = dcomponents.arr_Serie_C_BtoB_FormSteel_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_C_BtoB_FormSteel_Dimension[vm.ComponentIndex, 1] / 1000f;
                                fc_lip1 = dcomponents.arr_Serie_C_BtoB_FormSteel_Dimension[vm.ComponentIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_C_BtoB_FormSteel_Dimension[vm.ComponentIndex, 3] / 1000f;
                                cComponentColor = dcomponents.arr_Serie_C_BtoB_FormSteel_Colors[vm.ComponentIndex];
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
                            {
                                fb = dcomponents.arr_Serie_C_Nested_FormSteel_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_C_Nested_FormSteel_Dimension[vm.ComponentIndex, 1] / 1000f;
                                ft = dcomponents.arr_Serie_C_Nested_FormSteel_Dimension[vm.ComponentIndex, 2] / 1000f;
                                cComponentColor = dcomponents.arr_Serie_C_Nested_FormSteel_Colors[vm.ComponentIndex];
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                            {
                                fb = dcomponents.arr_Serie_Box63020_FormSteel_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Box63020_FormSteel_Dimension[vm.ComponentIndex, 1] / 1000f;
                                ft = dcomponents.arr_Serie_Box63020_FormSteel_Dimension[vm.ComponentIndex, 2] / 1000f;
                                ft_f = dcomponents.arr_Serie_Box63020_FormSteel_Dimension[vm.ComponentIndex, 3] / 1000f;
                                cComponentColor = dcomponents.arr_Serie_Box63020_FormSteel_Colors[vm.ComponentIndex];
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_PurlinDek:
                        case ESerieTypeCrSc_FormSteel.eSerie_SmartDek:
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
                                fb = dcomponents.arr_Serie_B_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fb2 = fb;
                                fh = dcomponents.arr_Serie_B_Dimension[vm.ComponentIndex, 1] / 1000f;
                                fl = dcomponents.arr_Serie_B_Dimension[vm.ComponentIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_B_Dimension[vm.ComponentIndex, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_B_Dimension[vm.ComponentIndex, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_L:
                            {
                                fb = dcomponents.arr_Serie_L_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fb2 = fb;
                                fh = dcomponents.arr_Serie_L_Dimension[vm.ComponentIndex, 1] / 1000f;
                                fl = dcomponents.arr_Serie_L_Dimension[vm.ComponentIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_L_Dimension[vm.ComponentIndex, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_L_Dimension[vm.ComponentIndex, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_LL:
                            {
                                fb = dcomponents.arr_Serie_LL_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fb2 = dcomponents.arr_Serie_LL_Dimension[vm.ComponentIndex, 1] / 1000f;
                                fh = dcomponents.arr_Serie_LL_Dimension[vm.ComponentIndex, 2] / 1000f;
                                fl = dcomponents.arr_Serie_LL_Dimension[vm.ComponentIndex, 3] / 1000f;
                                ft = dcomponents.arr_Serie_LL_Dimension[vm.ComponentIndex, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_LL_Dimension[vm.ComponentIndex, 5];
                                break;
                            }
                        case ESerieTypePlate.eSerie_F:
                            {
                                fb = dcomponents.arr_Serie_F_Dimension[vm.ComponentIndex, 0] / 1000f;
                                fb2 = dcomponents.arr_Serie_F_Dimension[vm.ComponentIndex, 1] / 1000f;
                                fh = dcomponents.arr_Serie_F_Dimension[vm.ComponentIndex, 2] / 1000f;
                                fl = dcomponents.arr_Serie_F_Dimension[vm.ComponentIndex, 3] / 1000f;
                                ft = dcomponents.arr_Serie_F_Dimension[vm.ComponentIndex, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_F_Dimension[vm.ComponentIndex, 5];
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
                        default:
                            {
                                // Not implemented
                                break;
                            }
                    }
                }
                else //  Screw
                {
                     sGauge_Screw = (dcomponents.arr_Screws_TEK_Dimensions[vm.ComponentIndex, 0]).ToString();
                }
        }

        private void UpdateAll()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel; 
            LoadDataFromDatabase();

            // Create Model
            // Change Combobox
            if (vm.ComponentTypeIndex == 0) // Cross-section
            {
                switch ((ESerieTypeCrSc_FormSteel)vm.ComponentSerieIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                        {
                            //temp test 
                            //crsc = new CCrSc_3_10075_BOX(fh * 3 , fb * 3, ft * 3, Colors.Red); 

                            crsc = new CCrSc_3_10075_BOX(fh, fb, ft, cComponentColor); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Z:
                        {
                            crsc = new CCrSc_3_Z(fh, fb_fl, fc_lip1, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            if(vm.ComponentIndex < 3) // C270
                                crsc = new CCrSc_3_270XX_C(fh, fb, ft, cComponentColor);
                            else
                                crsc = new CCrSc_3_50020_C(fh, fb, ft, cComponentColor);

                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
                        {
                            crsc = new CCrSc_3_270XX_C_BACK_TO_BACK(fh, fb, fc_lip1, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
                        {
                            if (vm.ComponentIndex == 0)
                                crsc = new CCrSc_3_270XX_C_NESTED(fh, fb, ft, cComponentColor); // C270115n
                            else
                                crsc = new CCrSc_3_50020_C_NESTED(fh, fb, ft, cComponentColor); // C50020n
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            crsc = new CCrSc_3_63020_BOX(fh, fb, ft, ft_f, cComponentColor); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_SmartDek:
                        {
                            crsc = new CCrSc_3_TR_SMARTDEK(fh, fb, ft, cComponentColor); // Trapezoidal sheeting
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_PurlinDek:
                        {
                            crsc = new CCrSc_3_TR_PURLINDEK(fh, fb, ft, cComponentColor); // Trapezoidal sheeting
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
                CScrew referenceScrew = new CScrew("TEK", "14");
                CAnchor referenceAnchor = new CAnchor(0.02f, 0.18f, 0.5f, true);

                bool bUseAdditinalConnectors = true;
                int iNumberOfAdditionalConnectorsInPlate = 32; // 2*4*4

                switch ((ESerieTypePlate)vm.ComponentSerieIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            plate = new CConCom_Plate_BB_BG(dcomponents.arr_Serie_B_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles, referenceScrew, referenceAnchor, 0 ,0 ,0 ,true); // L
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            plate = new CConCom_Plate_F_or_L(dcomponents.arr_Serie_L_Names[0], controlpoint, fb, fh, fl, ft,0,0,0, iNumberofHoles, referenceScrew, true); // L
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            plate = new CConCom_Plate_LL(dcomponents.arr_Serie_LL_Names[0], controlpoint, fb, fb2, fh, fl, ft,0, 0, 0, iNumberofHoles, referenceScrew, true); // LL
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            plate = new CConCom_Plate_F_or_L(dcomponents.arr_Serie_F_Names[0], controlpoint, vm.ComponentIndex, fb, fb2, fh, fl, ft,0f,0f,0f, true); // F
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
                                plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft,0,0,0, iNumberofHoles, referenceScrew, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else
                                plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, fPitch_rad,0,0,0, iNumberofHoles, referenceScrew, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            if (vm.ComponentIndex == 0) // KA
                                plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, iNumberofHoles, referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else if(vm.ComponentIndex == 1)
                                plate = new CConCom_Plate_KB(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else if (vm.ComponentIndex == 2)
                                plate = new CConCom_Plate_KC(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else if (vm.ComponentIndex == 3)
                                plate = new CConCom_Plate_KD(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else
                                plate = new CConCom_Plate_KE(dcomponents.arr_Serie_K_Names[4], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, referenceScrew, true);
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
                vm.SetComponentProperties(plate);
            }
            else
            {
                screw = new CScrew("TEK", sGauge_Screw);
                vm.SetComponentProperties(screw);
            }

            // Create 2D page
            page2D = new Canvas();

            Frame2DWidth = Frame2D.ActualWidth;
            Frame2DHeight = Frame2D.ActualHeight;
            if (Frame2DWidth == 0) Frame2DWidth = SystemParameters.PrimaryScreenWidth / 2 - 15;
            if (Frame2DHeight == 0) Frame2DHeight = SystemParameters.PrimaryScreenHeight - 145;

            if (vm.ComponentTypeIndex == 0)
            {
               Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D, 
                   vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
               Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                   vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else
            {
                // Screw - not implemented
                //page2D = new WindowCrossSection2D(); // TODO - Display empty window in current state
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;

            // Create 3D window
            page3D = null;

            if (vm.ComponentTypeIndex == 0)
            {
                // TODO / BUG - Ondrej neprekresluje za prierez podla vyberu v comboboxoch - component serie, podobne to nefunguje ani pre plates

                page3D = new Page3Dmodel(crsc, sDisplayOptions);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                page3D = new Page3Dmodel(plate, sDisplayOptions);
            }
            else
            {
                // Screw
                PerspectiveCamera camera = new PerspectiveCamera(new Point3D(36.6796089675504, -63.5328099899833, 57.4552066599888), new Vector3D(-43.3, 75, -50), new Vector3D(0, 0, 1), 51.5103932666685);
                page3D = new Page3Dmodel("../../Resources/self_drilling_screwModel3D.xaml", camera);
                tabItem3D.Focus();
            }

            // Display model in 3D preview frame
            Frame3D.Content = page3D;

            this.UpdateLayout();
        }

        public void CreateModelObject()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            if (vm.ComponentTypeIndex == 0) // Cross-sections
            {
                switch ((ESerieTypeCrSc_FormSteel) vm.ComponentSerieIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                        {
                            crsc = new CCrSc_3_10075_BOX(fh, fb, ft, cComponentColor); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Z:
                        {
                            crsc = new CCrSc_3_Z(fh, fb_fl, fc_lip1, ft, cComponentColor); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            if (vm.ComponentIndex < 3) // C270
                                crsc = new CCrSc_3_270XX_C(fh, fb, ft, cComponentColor);
                            else
                                crsc = new CCrSc_3_50020_C(fh, fb, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
                        {
                            crsc = new CCrSc_3_270XX_C_BACK_TO_BACK(fh, fb, fc_lip1, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
                        {
                            crsc = new CCrSc_3_270XX_C_NESTED(fh, fb, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            crsc = new CCrSc_3_63020_BOX(fh, fb, ft, ft_f, cComponentColor); // Box
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else if (vm.ComponentTypeIndex == 1) // Plates
            {
                CScrew referenceScrew = new CScrew("TEK", "14");
                CAnchor referenceAnchor = new CAnchor(0.02f, 0.18f, 0.5f, true);

                bool bUseAdditinalConnectors = true;
                int iNumberOfAdditionalConnectorsInPlate = 32; // 2*4*4

                switch ((ESerieTypePlate) vm.ComponentSerieIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            plate = new CConCom_Plate_BB_BG(dcomponents.arr_Serie_B_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles, referenceScrew, referenceAnchor, 0 ,0 ,0 ,true); // B - TODO pridat vsetky typy, zatial len BB a BG, pridat do databazy rozmery dier
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            plate = new CConCom_Plate_F_or_L(dcomponents.arr_Serie_L_Names[0], controlpoint, fb, fh, fl, ft,0,0,0, iNumberofHoles, referenceScrew, true); // L
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            plate = new CConCom_Plate_LL(dcomponents.arr_Serie_LL_Names[0], controlpoint, fb, fb2, fh, fl, ft, 0, 0, 0, iNumberofHoles, referenceScrew, true); // LL
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            plate = new CConCom_Plate_F_or_L(dcomponents.arr_Serie_F_Names[0], controlpoint, vm.ComponentIndex, fb, fb2, fh, fl, ft,0f,0f,0f, true); // F
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
                                plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, iNumberofHoles, referenceScrew, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else
                                plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, fPitch_rad, 0, 0, 0, iNumberofHoles, referenceScrew, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            if (vm.ComponentIndex == 0) // KA
                                plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0,0,0, iNumberofHoles, referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else if (vm.ComponentIndex == 1)
                                plate = new CConCom_Plate_KB(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0,0,0, iNumberofHoles, referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else if (vm.ComponentIndex == 2)
                                plate = new CConCom_Plate_KC(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else if (vm.ComponentIndex == 3)
                                plate = new CConCom_Plate_KD(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true);
                            else
                                plate = new CConCom_Plate_KE(dcomponents.arr_Serie_K_Names[4], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, referenceScrew, true);
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else
            {
                screw = new CScrew("TEK", sGauge_Screw);
            }
        }

        private void Combobox_Series_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Combobox_Component_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void BtnExportDXF_Click(object sender, RoutedEventArgs e)
        {
            CExportToDXF.ExportCanvas_DXF(page2D,0,0);
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
                if (plate.DrillingRoutePoints == null) { MessageBox.Show("Could not create NC file. Drilling route points not found."); return; }

                float fUnitFactor = 1000; // defined in m, exported in mm

                CExportToNC.ExportHolesToNC(plate.DrillingRoutePoints, plate.fThickness_tz, fUnitFactor);
                CExportToNC.ExportSetupToNC(Geom2D.TransformArrayToPointCoord(plate.PointsOut2D), fUnitFactor);
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
            List<Point> points = plate.GetHolesCentersPoints2D();
            if (points == null || points.Count == 0)
            {
                MessageBox.Show("Drilling points are not defined for selected plate.");
                return;
            }

            // Calculate size of plate and width to height ratio to set size of "salesman" algorthim window
            float fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;
            Drawing2D.CalculateModelLimits(plate.HolesCentersPoints2D, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            float fWidth = fTempMax_X - fTempMin_X;
            float fHeigth = fTempMax_Y - fTempMin_Y;
            float fHeightToWidthRatio = fHeigth / fWidth;

            // Add coordinates of drilling machine start point
            points.Insert(0, new Point(0, 0));

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

            List<Point> PathPoints = new List<Point>(viewModel.RoutePoints.Count);
            for (int i = 0; i < viewModel.RoutePoints.Count; i++)
            {
                PathPoints.Add(viewModel.RoutePoints[viewModel._model._tour.GetCities()[i]]);
            }

            Frame2DWidth = Frame2D.ActualWidth;
            Frame2DHeight = Frame2D.ActualHeight;
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            if (vm.ComponentTypeIndex == 0)
            {
                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                // Set drilling route points
                plate.DrillingRoutePoints = PathPoints;
                plate.DrillingRoutePoints2D = Geom2D.TransformPointToArrayCoord(plate.DrillingRoutePoints);
                // Draw plate
                Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);

                // Update value of drilling route in grid view
                vm.SetComponentProperties(plate);
            }
            else
            {
                // Screw - not implemented
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;
        }

        // TODO Ondrej - je potrebne pridat checkboxy do SystemComponentViewerViewModel ???
        // TODO Ondrej, po zmene typu, serie alebo objektu by sa mali checkboxy odchecknut
        // Uvazujem ze by tam mohol byt nielen uhol 90 deg ale aj 180, 270 alebo nejaky combobox, spinbuttons kde by sa dal nastavit lubovolny uhol rotacie prierezu alebo plechu

        private void CheckBox_MirrorX_Checked(object sender, RoutedEventArgs e)
        {
            plate.DrillingRoutePoints2D = null;
            plate.DrillingRoutePoints=null;
            tabItemDoc.Visibility = Visibility.Hidden;

            RedrawMirroredComponentAboutXIn2D();
        }

        private void CheckBox_MirrorY_Checked(object sender, RoutedEventArgs e)
        {
            plate.DrillingRoutePoints2D = null;
            plate.DrillingRoutePoints = null;
            tabItemDoc.Visibility = Visibility.Hidden;

            RedrawMirroredComponentAboutYIn2D();
        }

        private void CheckBox_Rotate_CW_Checked(object sender, RoutedEventArgs e)
        {
            plate.DrillingRoutePoints2D = null;
            plate.DrillingRoutePoints = null;
            tabItemDoc.Visibility = Visibility.Hidden;

            RedrawRotatedComponentIn2D(90);
        }

        private void CheckBox_MirrorX_Unchecked(object sender, RoutedEventArgs e)
        {
            plate.DrillingRoutePoints2D = null;
            plate.DrillingRoutePoints = null;
            tabItemDoc.Visibility = Visibility.Hidden;

            RedrawMirroredComponentAboutXIn2D();
        }

        private void CheckBox_MirrorY_Unchecked(object sender, RoutedEventArgs e)
        {
            plate.DrillingRoutePoints2D = null;
            plate.DrillingRoutePoints = null;
            tabItemDoc.Visibility = Visibility.Hidden;

            RedrawMirroredComponentAboutYIn2D();
        }

        private void CheckBox_Rotate_CW_Unchecked(object sender, RoutedEventArgs e)
        {
            plate.DrillingRoutePoints2D = null;
            plate.DrillingRoutePoints = null;
            tabItemDoc.Visibility = Visibility.Hidden;

            RedrawRotatedComponentIn2D(-90);
        }

        private void RedrawMirroredComponentAboutXIn2D()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentTypeIndex == 0)
            {
                // Mirror coordinates
                crsc.MirrorPlateAboutX();
                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                // Mirror coordinates
                plate.MirrorPlateAboutX();
                // Redraw plate in 2D
                Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else // Screw
            {
                throw new NotImplementedException("Component 'screw' is not implemented!");
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;
        }
        private void RedrawMirroredComponentAboutYIn2D()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentTypeIndex == 0)
            {
                // Mirror coordinates
                crsc.MirrorPlateAboutY();
                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                // Mirror coordinates
                plate.MirrorPlateAboutY();
                // Redraw plate in 2D
                Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else // Screw
            {
                throw new NotImplementedException("Component 'screw' is not implemented!");
            }

            // Display component in 2D preview frame
            Frame2D.Content = page2D;
        }
        private void RedrawRotatedComponentIn2D(float fTheta_deg)
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentTypeIndex == 0)
            {
                // Rotate coordinates
                crsc.RotateCrsc_CW(fTheta_deg);
                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                // Rotate coordinates
                plate.RotatePlateAboutZ_CW(fTheta_deg);
                // Redraw plate in 2D
                Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else // Screw
            {
                throw new NotImplementedException("Component 'screw' is not implemented!");
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;
        }

        private void BtnShowCNCSetupFile_Click(object sender, RoutedEventArgs e)
        {
            float fUnitFactor = 1000; // defined in m, exported in mm
            try
            {
                tabItemDoc.Visibility = Visibility.Visible;

                StringBuilder sb2 = CExportToNC.GetCNCFileContentForSetup(Geom2D.TransformArrayToPointCoord(plate.PointsOut2D), fUnitFactor);
                Paragraph paragraph = new Paragraph();
                paragraph.FontSize = 14;
                paragraph.FontFamily = new FontFamily("Consolas");
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

                StringBuilder sb1 = CExportToNC.GetCNCFileContentForHoles(plate.DrillingRoutePoints, plate.fThickness_tz, fUnitFactor);
                Paragraph paragraph = new Paragraph();
                paragraph.FontSize = 14;
                paragraph.FontFamily = new FontFamily("Consolas");
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

        private void DataGridGeometry_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string changedText = ((TextBox)e.EditingElement).Text;
            CComponentParamsView item = ((CComponentParamsView)e.Row.Item);
            if (changedText == item.Value) return;

            
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            if (vm.ComponentTypeIndex == 0)
            {
                //keby som vedel tak detekujem ktora property sa zmenila takto:
                //if (item.Name == "Width") crsc.Width = int.Parse(changedText);
                //a predpokladam,ze by to vykreslilo samo => iba zeby nie :-)

                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                //keby som vedel tak detekujem ktora property sa zmenila takto:
                //if (item.Name == "Width") plate.Width = int.Parse(changedText);
                //a predpokladam,ze by to vykreslilo samo => iba zeby nie :-)

                
                // Redraw plate in 2D
                Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawDrillingRoute2D);
            }
            else // Screw
            {
                throw new NotImplementedException("Component 'screw' is not implemented!");
            }

            //UpdateAll();
        }
    }
}