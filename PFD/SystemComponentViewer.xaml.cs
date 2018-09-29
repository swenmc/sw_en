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
using System.Linq;

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
            if (sender is SystemComponentViewerViewModel)
            {
                SystemComponentViewerViewModel vm = sender as SystemComponentViewerViewModel;
                if (vm != null && vm.IsSetFromCode) return;

                if (e.PropertyName == "ComponentIndex") { UpdateAll(); SetUIElementsVisibility(vm); }

                if (e.PropertyName == "DrawPoints2D" ||
                    e.PropertyName == "DrawOutLine2D" ||
                    e.PropertyName == "DrawPointNumbers2D" ||
                    e.PropertyName == "DrawHoles2D" ||
                    e.PropertyName == "DrawHoleCentreSymbol2D" ||
                    e.PropertyName == "DrawDrillingRoute2D" ||
                    e.PropertyName == "DrawScrews3D") UpdateAll();

                if (e.PropertyName == "MirrorX" || e.PropertyName == "MirrorY")
                {
                    //plate.DrillingRoutePoints = null;
                    vm.DrillingRoutePoints = null;
                    tabItemDoc.Visibility = Visibility.Hidden;
                    BtnShowCNCDrillingFile.IsEnabled = false;
                    UpdateAll();
                }

                if (e.PropertyName == "Rotate90CW")
                {
                    //plate.DrillingRoutePoints = null;
                    vm.DrillingRoutePoints = null;
                    tabItemDoc.Visibility = Visibility.Hidden;
                    BtnShowCNCDrillingFile.IsEnabled = false;
                    if (vm.Rotate90CW == true && vm.Rotate90CCW == true) vm.Rotate90CCW = false;

                    UpdateAll();
                }
                if (e.PropertyName == "Rotate90CCW")
                {
                    //plate.DrillingRoutePoints = null;
                    vm.DrillingRoutePoints = null;
                    tabItemDoc.Visibility = Visibility.Hidden;
                    BtnShowCNCDrillingFile.IsEnabled = false;
                    if (vm.Rotate90CW == true && vm.Rotate90CCW == true) vm.Rotate90CW = false;

                    UpdateAll();
                }

                if (e.PropertyName == "ScrewArrangementIndex")
                {
                    SetUIElementsVisibilityForScrewArrangement(vm);
                    UpdateAll();
                }

                //if (e.PropertyName == "ScrewArrangementParameters")
                //{
                //    UpdateAll();
                //}

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

                panelOptionsTransform2D.Visibility = Visibility.Visible;
                
                tabItemDoc.IsEnabled = false;
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

                panelOptionsTransform2D.Visibility = Visibility.Visible;

                tabItemDoc.IsEnabled = true;
                if (plate != null && plate.ScrewArrangement != null)
                {
                    BtnFindCNCPath.IsEnabled = plate.ScrewArrangement.IHolesNumber > 0;
                    // BtnExportCNC can be enabled for export of plate setup file or cutting file evenif drilling route is not defined or holes are not defined.
                    //BtnExportCNC.IsEnabled = (plate.DrillingRoutePoints != null && plate.DrillingRoutePoints.Count > 0);
                    BtnShowCNCDrillingFile.IsEnabled = (plate.DrillingRoutePoints != null && plate.DrillingRoutePoints.Count > 0);
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

                panelOptionsTransform2D.Visibility = Visibility.Hidden;

                tabItemDoc.IsEnabled = false;
            }

            //uncheck all Transformation Options
            if(vm.MirrorX) vm.MirrorX = false;
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

                            crsc = new CCrSc_3_10075_BOX(0,fh, fb, ft, cComponentColor); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Z:
                        {
                            crsc = new CCrSc_3_Z(0,fh, fb_fl, fc_lip1, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            if(vm.ComponentIndex < 3) // C270
                                crsc = new CCrSc_3_270XX_C(0,fh, fb, ft, cComponentColor);
                            else
                                crsc = new CCrSc_3_50020_C(0,fh, fb, ft, cComponentColor);

                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
                        {
                            crsc = new CCrSc_3_270XX_C_BACK_TO_BACK(0, fh, fb, fc_lip1, ft, cComponentColor);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
                        {
                            if (vm.ComponentIndex == 0)
                                crsc = new CCrSc_3_270XX_C_NESTED(0, fh, fb, ft, cComponentColor); // C270115n
                            else
                                crsc = new CCrSc_3_50020_C_NESTED(0, fh, fb, ft, cComponentColor); // C50020n
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            crsc = new CCrSc_3_63020_BOX(0, fh, fb, ft, ft_f, cComponentColor); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_SmartDek:
                        {
                            crsc = new CCrSc_3_TR_SMARTDEK(0, fh, fb, ft, cComponentColor); // Trapezoidal sheeting
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_PurlinDek:
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
                CScrew referenceScrew = new CScrew("TEK", "14");
                CAnchor referenceAnchor = new CAnchor(0.02f, 0.18f, 0.5f, true);
                CScrewArrangement_BB_BG screwArrangement_BB_BG = new CScrewArrangement_BB_BG(2, referenceScrew, referenceAnchor);
                CScrewArrangement_F_or_L screwArrangement_ForL = new CScrewArrangement_F_or_L(iNumberofHoles, referenceScrew);
                CScrewArrangement_LL screwArrangement_LL = new CScrewArrangement_LL(iNumberofHoles, referenceScrew);

                bool bUseAdditionalConnectors = true;
                int iNumberOfAdditionalConnectorsInCorner = 4;
                int iConnectorNumberInCircleSeuence = 20;

                List<CScrewSequenceGroup> screwSeqGroups = new List<CScrewSequenceGroup>();
                CScrewSequenceGroup gr1 = new CScrewSequenceGroup();
                gr1.NumberOfHalfCircleSequences = 2;
                gr1.NumberOfRectangularSequences = 4;
                gr1.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f, iConnectorNumberInCircleSeuence));
                gr1.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f, iConnectorNumberInCircleSeuence));
                screwSeqGroups.Add(gr1);
                CScrewSequenceGroup gr2 = new CScrewSequenceGroup();
                gr2.NumberOfHalfCircleSequences = 2;
                gr2.NumberOfRectangularSequences = 4;
                gr2.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f, iConnectorNumberInCircleSeuence));
                gr2.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f, iConnectorNumberInCircleSeuence));
                screwSeqGroups.Add(gr2);

                List<CScrewSequenceGroup> screwSeqGroups2Circles = new List<CScrewSequenceGroup>();
                CScrewSequenceGroup gr2_1 = new CScrewSequenceGroup();
                gr2_1.NumberOfHalfCircleSequences = 4;
                gr2_1.NumberOfRectangularSequences = 4;
                gr2_1.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f, iConnectorNumberInCircleSeuence));
                gr2_1.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f, iConnectorNumberInCircleSeuence));
                gr2_1.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f * 0.7f, iConnectorNumberInCircleSeuence - 5));
                gr2_1.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f * 0.7f, iConnectorNumberInCircleSeuence - 5));
                screwSeqGroups2Circles.Add(gr2_1);
                CScrewSequenceGroup gr2_2 = new CScrewSequenceGroup();
                gr2_2.NumberOfHalfCircleSequences = 4;
                gr2_2.NumberOfRectangularSequences = 4;
                gr2_2.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f, iConnectorNumberInCircleSeuence));
                gr2_2.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f, iConnectorNumberInCircleSeuence));
                gr2_2.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f * 0.7f, iConnectorNumberInCircleSeuence - 5));
                gr2_2.ListScrewSequence.Add(new CScrewHalfCircleSequence(0.25f * 0.7f, iConnectorNumberInCircleSeuence - 5));
                screwSeqGroups2Circles.Add(gr2_2);                

                CScrewArrangementCircleApexOrKnee screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 1, screwSeqGroups, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
                CScrewArrangementCircleApexOrKnee screwArrangementCircle2 = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 2, screwSeqGroups2Circles, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);

                //CScrewArrangementCircleApexOrKnee screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 1, iConnectorNumberInCircleSeuence, 0.25f, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
                //CScrewArrangementCircleApexOrKnee screwArrangementCircle2 = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 2, iConnectorNumberInCircleSeuence, 0.25f, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
                CScrewArrangementRectApexOrKnee screwArrangementRectangleApex = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 0.0f, 0.0f, 0.07f, 0.05f, 15, 3, 0.1f, 0.5f, 0.04f, 0.04f);
                CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 10, 2);
                CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee2 = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 10, 2);

                switch ((ESerieTypePlate)vm.ComponentSerieIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            // Vynimka, je potrebne prepracovat na screwArrangement a anchorArrangement
                            plate = new CConCom_Plate_BB_BG(dcomponents.arr_Serie_B_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles, referenceScrew, referenceAnchor, 0 ,0 ,0 , screwArrangement_BB_BG, true); // B
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            plate = new CConCom_Plate_F_or_L(dcomponents.arr_Serie_L_Names[0], controlpoint, fb, fh, fl, ft,0,0,0, screwArrangement_ForL, true); // L
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            plate = new CConCom_Plate_LL(dcomponents.arr_Serie_LL_Names[0], controlpoint, fb, fb2, fh, fl, ft,0, 0, 0, screwArrangement_LL, true); // LL
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
                            // TODO - Ondrej
                            // TODO - Mato nerozumiem,co tu mam robit/opravit

                            // To Ondrej - pre Plates J a K mame zatial 3 mozne typy screwArrangements (undefined, circle, rectangular)
                            // Momentalne to mam pre JA/JB a KA spravene tak ze maju 3 konstruktory, jeden undefined, dalsi pre circle, a dalsi pre rectangular
                            // Pointa je, ze mam pre J - 2 bloky if/else a pre K - 5 blokov kodu if/else, do ktorych by som (tak to som zacal u JA/JB a KA) rozkopiroval dalsi if/else s troma blokmi
                            // Urcite mi poradis ako by to malo byt krajsie, najradsej by som mal u plate len jeden konstruktor s obecnym screwArrangement a az v ramci triedy JA, JB, KA, KB, KC, KD, KE sa rozhodoval akeho je typu a ako ho pouzit a ktore funkcie zavolat

                            // TEMPORARY
                            if (vm.ComponentIndex == 0) // JA
                            {
                                // TODO - Ondrej tymto stylom to nechcem mat rozkopirovane 7 krat, asi sa to da urobit elegantnejsie
                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, true);
                                else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, screwArrangementRectangleApex, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, screwArrangementCircle, true);
                            }
                            else //if (vm.ComponentIndex == 1) // JB
                            {
                                // TODO - Ondrej tymto stylom to nechcem mat rozkopirovane 7 krat, asi sa to da urobit elegantnejsie
                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true);
                                else if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0,screwArrangementRectangleApex, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0,screwArrangementCircle2, true);
                            }

                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            // TODO - Ondrej
                            // TEMPORARY - vyriesit ako vytvorit plate s roznym typom objektu screwArrangement
                            // Plate KA ma rozne konstruktory podla typu arrangement ale bolo by krajsie ak by bol konstruktor len jeden s obecnym CScrewArrangement
                            // a v objekte plate by sa identifikovalo o ktory potomok CScrewArrangement sa jedna a ako s nim nalozit

                            // TODO - Ondrej
                            // TODO - Mato nerozumiem,co tu mam robit/opravit

                            // To Ondrej - pre Plates J a K mame zatial 3 mozne typy screwArrangements (undefined, circle, rectangular)
                            // Momentalne to mam pre JA/JB a KA spravene tak ze maju 3 konstruktory, jeden undefined, dalsi pre circle, a dalsi pre rectangular
                            // Pointa je, ze mam pre J - 2 bloky if/else a pre K - 5 blokov kodu if/else, do ktorych by som (tak to som zacal u JA/JB a KA) rozkopiroval dalsi if/else s troma blokmi
                            // Urcite mi poradis ako by to malo byt krajsie, najradsej by som mal u plate len jeden konstruktor s obecnym screwArrangement a az v ramci triedy JA, JB, KA, KB, KC, KD, KE sa rozhodoval akeho je typu a ako ho pouzit a ktore funkcie zavolat

                            // TEMPORARY for KA, potrebujeme vyriesit obecne pre vsetky plates
                            if (vm.ComponentIndex == 0) // KA
                            {
                                // TODO - Ondrej tymto stylom to nechcem mat rozkopirovane 7 krat, asi sa to da urobit elegantnejsie
                                if (vm.ScrewArrangementIndex == 0) // Undefined
                                    plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, true);
                                if (vm.ScrewArrangementIndex == 1) // Rectangular
                                    plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, screwArrangementRectangleKnee2, true);
                                else//(vm.ScrewArrangementIndex == 2) // Circle
                                    plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, screwArrangementCircle, true);
                            }

                            /*
                            if (vm.ComponentIndex == 0) // KA
                                plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0,0,0, screwArrangementRectangleKnee2, true);
                            */else if (vm.ComponentIndex == 1)
                                plate = new CConCom_Plate_KB(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle2, true);
                            else if (vm.ComponentIndex == 2)
                                plate = new CConCom_Plate_KC(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true);
                            else if (vm.ComponentIndex == 3)
                                plate = new CConCom_Plate_KD(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true);
                            else
                                plate = new CConCom_Plate_KE(dcomponents.arr_Serie_K_Names[4], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true);
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
                vm.SetComponentProperties(plate);
                if(plate != null) vm.SetScrewArrangementProperties(plate.ScrewArrangement); 
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
            // Nenastavovat z maximalnych rozmerov screen, ale z aktualnych rozmerov okna System Component Viewer
            if (Frame2DWidth == 0) Frame2DWidth = this.Width - 669; // SystemParameters.PrimaryScreenWidth / 2 - 15;
            if (Frame2DHeight == 0) Frame2DHeight = this.Height - 116; // SystemParameters.PrimaryScreenHeight - 145;

            if (vm.ComponentTypeIndex == 0)
            {
                if (vm.MirrorX) crsc.MirrorCRSCAboutX();
                if (vm.MirrorY) crsc.MirrorCRSCAboutY();
                if (vm.Rotate90CW) crsc.RotateCrsc_CW(90);
                if (vm.Rotate90CCW) crsc.RotateCrsc_CW(-90);
                
                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D, 
                   vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                if (vm.MirrorX) plate.MirrorPlateAboutX();
                if (vm.MirrorY) plate.MirrorPlateAboutY();
                if (vm.Rotate90CW) plate.RotatePlateAboutZ_CW(90);
                if (vm.Rotate90CCW) plate.RotatePlateAboutZ_CW(-90);
                if (vm.DrillingRoutePoints != null) plate.DrillingRoutePoints = vm.DrillingRoutePoints;

                Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                   vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawHoleCentreSymbol2D, vm.DrawDrillingRoute2D);
            }
            else
            {
                // Screw
                bool bDrawCentreSymbol = true;
                Drawing2D.DrawScrewToCanvas(screw, Frame2DWidth, Frame2DHeight, ref page2D, bDrawCentreSymbol);
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;

            // Create 3D window
            page3D = null;

            if (vm.ComponentTypeIndex == 0)
            {
                page3D = new Page3Dmodel(crsc, sDisplayOptions);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                sDisplayOptions.bDisplayConnectors = vm.DrawScrews3D;
                page3D = new Page3Dmodel(plate, sDisplayOptions);
            }
            else
            {
                // Screw
                PerspectiveCamera camera = new PerspectiveCamera(new Point3D(36.6796089675504, -63.5328099899833, 57.4552066599888), new Vector3D(-43.3, 75, -50), new Vector3D(0, 0, 1), 51.5103932666685);
                page3D = new Page3Dmodel("../../Resources/self_drilling_screwModel3D.xaml", camera);
            }

            // Display model in 3D preview frame
            Frame3D.Content = page3D;

            this.UpdateLayout();
        }

        //public void CreateModelObject()
        //{
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
        //    if (vm.ComponentTypeIndex == 0) // Cross-sections
        //    {
        //        switch ((ESerieTypeCrSc_FormSteel) vm.ComponentSerieIndex)
        //        {
        //            case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
        //                {
        //                    crsc = new CCrSc_3_10075_BOX(0, fh, fb, ft, cComponentColor); // BOX
        //                    break;
        //                }
        //            case ESerieTypeCrSc_FormSteel.eSerie_Z:
        //                {
        //                    crsc = new CCrSc_3_Z(0, fh, fb_fl, fc_lip1, ft, cComponentColor); // BOX
        //                    break;
        //                }
        //            case ESerieTypeCrSc_FormSteel.eSerie_C_single:
        //                {
        //                    if (vm.ComponentIndex < 3) // C270
        //                        crsc = new CCrSc_3_270XX_C(0, fh, fb, ft, cComponentColor);
        //                    else
        //                        crsc = new CCrSc_3_50020_C(0, fh, fb, ft, cComponentColor);
        //                    break;
        //                }
        //            case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
        //                {
        //                    crsc = new CCrSc_3_270XX_C_BACK_TO_BACK(0, fh, fb, fc_lip1, ft, cComponentColor);
        //                    break;
        //                }
        //            case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
        //                {
        //                    crsc = new CCrSc_3_270XX_C_NESTED(0, fh, fb, ft, cComponentColor);
        //                    break;
        //                }
        //            case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
        //                {
        //                    crsc = new CCrSc_3_63020_BOX(0, fh, fb, ft, ft_f, cComponentColor); // Box
        //                    break;
        //                }
        //            default:
        //                {
        //                    // Not implemented
        //                    break;
        //                }
        //        }
        //    }
        //    else if (vm.ComponentTypeIndex == 1) // Plates
        //    {
        //        CScrew referenceScrew = new CScrew("TEK", "14");
        //        CAnchor referenceAnchor = new CAnchor(0.02f, 0.18f, 0.5f, true);
        //        CScrewArrangement_BB_BG screwArrangement_BB_BG = new CScrewArrangement_BB_BG(2, referenceScrew, referenceAnchor);
        //        CScrewArrangement_F_or_L screwArrangement_ForL = new CScrewArrangement_F_or_L(iNumberofHoles, referenceScrew);
        //        CScrewArrangement_LL screwArrangement_LL = new CScrewArrangement_LL(iNumberofHoles, referenceScrew);

        //        bool bUseAdditionalConnectors = true;
        //        int iNumberOfAdditionalConnectorsInCorner = 4;
        //        int iConnectorNumberInCircleSeuence = 20;

        //        CScrewArrangementCircleApexOrKnee screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 1, iConnectorNumberInCircleSeuence, 0.25f, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
        //        CScrewArrangementCircleApexOrKnee screwArrangementCircle2 = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 2, iConnectorNumberInCircleSeuence, 0.25f, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
        //        CScrewArrangementRectApexOrKnee screwArrangementRectangleApex = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 0.0f, 0.0f, 0.07f, 0.05f, 15, 3, 0.1f, 0.5f, 0.04f, 0.04f);
        //        CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 15, 2, 0.0f, 0.0f, 0.05f, 0.04f, 15, 2, 0.1f, 0.5f, 0.04f, 0.04f);
        //        CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee2 = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 10, 2);

        //        switch ((ESerieTypePlate) vm.ComponentSerieIndex)
        //        {
        //            case ESerieTypePlate.eSerie_B:
        //                {
        //                    plate = new CConCom_Plate_BB_BG(dcomponents.arr_Serie_B_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles, referenceScrew, referenceAnchor, 0 ,0 ,0 , screwArrangement_BB_BG, true); // B - TODO pridat vsetky typy, zatial len BB a BG, pridat do databazy rozmery dier
        //                    break;
        //                }
        //            case ESerieTypePlate.eSerie_L:
        //                {
        //                    plate = new CConCom_Plate_F_or_L(dcomponents.arr_Serie_L_Names[0], controlpoint, fb, fh, fl, ft,0,0,0, screwArrangement_ForL, true); // L
        //                    break;
        //                }
        //            case ESerieTypePlate.eSerie_LL:
        //                {
        //                    plate = new CConCom_Plate_LL(dcomponents.arr_Serie_LL_Names[0], controlpoint, fb, fb2, fh, fl, ft, 0, 0, 0, screwArrangement_LL, true); // LL
        //                    break;
        //                }
        //            case ESerieTypePlate.eSerie_F:
        //                {
        //                    plate = new CConCom_Plate_F_or_L(dcomponents.arr_Serie_F_Names[0], controlpoint, vm.ComponentIndex, fb, fb2, fh, fl, ft,0f,0f,0f, true); // F
        //                    break;
        //                }
        //            case ESerieTypePlate.eSerie_Q:
        //                {
        //                    plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_Q_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // Q
        //                    break;
        //                }
        //            case ESerieTypePlate.eSerie_T:
        //                {
        //                    plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_T_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // T
        //                    break;
        //                }
        //            case ESerieTypePlate.eSerie_Y:
        //                {
        //                    plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_Y_Names[0], controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles, true); // Y
        //                    break;
        //                }
        //            case ESerieTypePlate.eSerie_J:
        //                {
        //                    // TODO - Ondrej
        //                    // TODO - Mato nerozumiem,co tu mam robit/opravit

        //                    // To Ondrej - pre Plates J a K mame zatial 3 mozne typy screwArrangements (undefined, circle, rectangular)
        //                    // Momentalne to mam pre JA/JB a KA spravene tak ze maju 3 konstruktory, jeden undefined, dalsi pre circle, a dalsi pre rectangular
        //                    // Pointa je, ze mam pre J - 2 bloky if/else a pre K - 5 blokov kodu if/else, do ktorych by som (tak to som zacal u JA/JB a KA) rozkopiroval dalsi if/else s troma blokmi
        //                    // Urcite mi poradis ako by to malo byt krajsie, najradsej by som mal u plate len jeden konstruktor s obecnym screwArrangement a az v ramci triedy JA, JB, KA, KB, KC, KD, KE sa rozhodoval akeho je typu a ako ho pouzit a ktore funkcie zavolat

        //                    if (vm.ComponentIndex == 0) // JA
        //                    {
        //                        // TODO - Ondrej tymto stylom to nechcem mat rozkopirovane 7 krat, asi sa to da urobit elegantnejsie
        //                        if (vm.ScrewArrangementIndex == 0) // Undefined
        //                            plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, true);
        //                        else if (vm.ScrewArrangementIndex == 1) // Rectangular
        //                            plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, screwArrangementRectangleApex, true);
        //                        else//(vm.ScrewArrangementIndex == 2) // Circle
        //                            plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, screwArrangementCircle, true);
        //                    }
        //                    else //if (vm.ComponentIndex == 1) // JB
        //                    {
        //                        // TODO - Ondrej tymto stylom to nechcem mat rozkopirovane 7 krat, asi sa to da urobit elegantnejsie
        //                        if (vm.ScrewArrangementIndex == 0) // Undefined
        //                            plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true);
        //                        else if (vm.ScrewArrangementIndex == 1) // Rectangular
        //                            plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, screwArrangementRectangleApex, true);
        //                        else//(vm.ScrewArrangementIndex == 2) // Circle
        //                            plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, screwArrangementCircle2, true);
        //                    }
        //                    break;
        //                }
        //            case ESerieTypePlate.eSerie_K:
        //                {
        //                    // TODO - Ondrej
        //                    // TEMPORARY - vyriesit ako vytvorit plate s roznym typom objektu screwArrangement
        //                    // Plate KA ma rozne konstruktory podla typu arrangement ale bolo by krajsie ak by bol konstruktor len jeden s obecnym CScrewArrangement
        //                    // a v objekte plate by sa identifikovalo o ktory potomok CScrewArrangement sa jedna a ako s nim nalozit

        //                    // TODO - Ondrej
        //                    // TODO - Mato nerozumiem,co tu mam robit/opravit

        //                    // To Ondrej - pre Plates J a K mame zatial 3 mozne typy screwArrangements (undefined, circle, rectangular)
        //                    // Momentalne to mam pre JA/JB a KA spravene tak ze maju 3 konstruktory, jeden undefined, dalsi pre circle, a dalsi pre rectangular
        //                    // Pointa je, ze mam pre J - 2 bloky if/else a pre K - 5 blokov kodu if/else, do ktorych by som (tak to som zacal u JA/JB a KA) rozkopiroval dalsi if/else s troma blokmi
        //                    // Urcite mi poradis ako by to malo byt krajsie, najradsej by som mal u plate len jeden konstruktor s obecnym screwArrangement a az v ramci triedy JA, JB, KA, KB, KC, KD, KE sa rozhodoval akeho je typu a ako ho pouzit a ktore funkcie zavolat


        //                    // Temporary for KA, potrebujeme vyriesit obecne pre vsetky plates
        //                    if (vm.ComponentIndex == 0) // KA
        //                    {
        //                        // TODO - Ondrej tymto stylom to nechcem mat rozkopirovane 7 krat, asi sa to da urobit elegantnejsie
        //                        if (vm.ScrewArrangementIndex == 0) // Undefined
        //                            plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, true);
        //                        if (vm.ScrewArrangementIndex == 1) // Rectangular
        //                            plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, screwArrangementRectangleKnee2, true);
        //                        else//(vm.ScrewArrangementIndex == 2) // Circle
        //                            plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, screwArrangementCircle, true);
        //                    }

        //                    /*
        //                    if (vm.ComponentIndex == 0) // KA
        //                        plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0,0,0, screwArrangementRectangleKnee2, true);
        //                    else */if (vm.ComponentIndex == 1)
        //                        plate = new CConCom_Plate_KB(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0,0,0, screwArrangementCircle2, true);
        //                    else if (vm.ComponentIndex == 2)
        //                        plate = new CConCom_Plate_KC(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true);
        //                    else if (vm.ComponentIndex == 3)
        //                        plate = new CConCom_Plate_KD(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true);
        //                    else
        //                        plate = new CConCom_Plate_KE(dcomponents.arr_Serie_K_Names[4], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true);
        //                    break;
        //                }
        //            default:
        //                {
        //                    // Not implemented
        //                    break;
        //                }
        //        }
        //    }
        //    else
        //    {
        //        screw = new CScrew("TEK", sGauge_Screw);
        //    }
        //}

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
                if (plate.DrillingRoutePoints == null) { MessageBox.Show("Could not create NC file. Drilling route points not found. Only setup file is exported."); }

                float fUnitFactor = 1000; // defined in m, exported in mm

                // Export drilling file
                if (plate.DrillingRoutePoints != null)
                    CExportToNC.ExportHolesToNC(plate.DrillingRoutePoints, plate.Ft, fUnitFactor);

                // Export setup file
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
            List<Point> points = null;

            if(plate.ScrewArrangement != null) // Screw arrangmenet must exists
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

            if (vm.ComponentTypeIndex == 1) //ma to vyznam iba pre Plates
            {
                // Set drilling route points
                vm.DrillingRoutePoints = PathPoints;
                // Enable button to display of CNC drilling file
                BtnShowCNCDrillingFile.IsEnabled = true;

                UpdateAll();
            }

            //if (vm.ComponentTypeIndex == 0)
            //{
            //    Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
            //        vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
            //}
            //else if (vm.ComponentTypeIndex == 1)
            //{
            //    // Set drilling route points
            //    vm.DrillingRoutePoints = PathPoints;
            //    // Draw plate
            //    Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
            //        vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawHoleCentreSymbol2D, vm.DrawDrillingRoute2D);

            //    // Update value of drilling route in grid view
            //    vm.SetComponentProperties(plate);
            //}
            //else
            //{
            //    // Screw
            //    bool bDrawCentreSymbol = true;
            //    Drawing2D.DrawScrewToCanvas(screw, Frame2DWidth, Frame2DHeight, ref page2D, bDrawCentreSymbol);
            //}

            //// Display plate in 2D preview frame
            //Frame2D.Content = page2D;
        }

        //private void CheckBox_MirrorX_Checked(object sender, RoutedEventArgs e)
        //{
        //    plate.DrillingRoutePoints=null;
        //    tabItemDoc.Visibility = Visibility.Hidden;

        //    UpdateAll();
        //    //RedrawMirroredComponentAboutXIn2D();
        //    //RedrawComponentIn2D();
        //}

        //private void CheckBox_MirrorY_Checked(object sender, RoutedEventArgs e)
        //{
        //    plate.DrillingRoutePoints = null;
        //    tabItemDoc.Visibility = Visibility.Hidden;

        //    //RedrawMirroredComponentAboutYIn2D();
        //    RedrawComponentIn2D();
        //}

        //private void CheckBox_Rotate_CW_Checked(object sender, RoutedEventArgs e)
        //{
        //    plate.DrillingRoutePoints = null;
        //    tabItemDoc.Visibility = Visibility.Hidden;
            
        //    // Can't be rotated CW and in the same time CCW
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
        //    if (vm.Rotate_90_CCW) vm.Rotate_90_CCW = false;
        //    //RedrawRotatedComponentIn2D(90);
        //    RedrawComponentIn2D();
        //}

        //private void CheckBox_Rotate_CCW_Checked(object sender, RoutedEventArgs e)
        //{
        //    plate.DrillingRoutePoints = null;
        //    tabItemDoc.Visibility = Visibility.Hidden;
            
        //    // Can't be rotated CW and in the same time CCW
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
        //    if (vm.Rotate_90_CW) vm.Rotate_90_CW = false;

        //    //RedrawRotatedComponentIn2D(-90);
        //    RedrawComponentIn2D();
        //}

        //private void CheckBox_MirrorX_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    plate.DrillingRoutePoints = null;
        //    tabItemDoc.Visibility = Visibility.Hidden;

        //    //RedrawMirroredComponentAboutXIn2D();
        //    RedrawComponentIn2D();
        //}

        //private void CheckBox_MirrorY_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    plate.DrillingRoutePoints = null;
        //    tabItemDoc.Visibility = Visibility.Hidden;

        //    //RedrawMirroredComponentAboutYIn2D();
        //    RedrawComponentIn2D();
        //}

        //private void CheckBox_Rotate_CW_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    plate.DrillingRoutePoints = null;
        //    tabItemDoc.Visibility = Visibility.Hidden;

        //    //RedrawRotatedComponentIn2D(-90);
        //    RedrawComponentIn2D();
        //}

        //private void CheckBox_Rotate_CCW_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    plate.DrillingRoutePoints = null;
        //    tabItemDoc.Visibility = Visibility.Hidden;

        //    //RedrawRotatedComponentIn2D(90);
        //    RedrawComponentIn2D();
        //}

        //private void RedrawMirroredComponentAboutXIn2D()
        //{
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

        //    if (vm.ComponentTypeIndex == 0)
        //    {
        //        // Mirror coordinates
        //        crsc.MirrorCRSCAboutX();
        //        Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
        //            vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
        //    }
        //    else if (vm.ComponentTypeIndex == 1)
        //    {
        //        // Mirror coordinates
        //        plate.MirrorPlateAboutX();
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
        //private void RedrawMirroredComponentAboutYIn2D()
        //{
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

        //    if (vm.ComponentTypeIndex == 0)
        //    {
        //        // Mirror coordinates
        //        crsc.MirrorCRSCAboutY();
        //        Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
        //            vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
        //    }
        //    else if (vm.ComponentTypeIndex == 1)
        //    {
        //        // Mirror coordinates
        //        plate.MirrorPlateAboutY();
        //        // Redraw plate in 2D
        //        Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
        //            vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawHoleCentreSymbol2D, vm.DrawDrillingRoute2D);
        //    }
        //    else // Screw
        //    {
        //        bool bDrawCentreSymbol = true;
        //        Drawing2D.DrawScrewToCanvas(screw, Frame2DWidth, Frame2DHeight, ref page2D, bDrawCentreSymbol);
        //    }

        //    // Display component in 2D preview frame
        //    Frame2D.Content = page2D;
        //}
        //private void RedrawRotatedComponentIn2D(float fTheta_deg)
        //{
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

        //    if (vm.ComponentTypeIndex == 0)
        //    {
        //        // Rotate coordinates
        //        crsc.RotateCrsc_CW(fTheta_deg);
        //        Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
        //            vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
        //    }
        //    else if (vm.ComponentTypeIndex == 1)
        //    {
        //        // Rotate coordinates
        //        plate.RotatePlateAboutZ_CW(fTheta_deg);
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

        //private void RedrawComponentIn2D()
        //{
        //    SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

        //    if (vm.ComponentTypeIndex == 0)
        //    {
        //        if (vm.MirrorX) crsc.MirrorCRSCAboutX();
        //        if (vm.MirrorY) crsc.MirrorCRSCAboutY();
        //        if (vm.Rotate_90_CW) crsc.RotateCrsc_CW(90);
        //        if (vm.Rotate_90_CCW) crsc.RotateCrsc_CW(-90);

        //        Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
        //            vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
        //    }
        //    else if (vm.ComponentTypeIndex == 1)
        //    {
        //        if (vm.MirrorX) plate.MirrorPlateAboutX();
        //        if (vm.MirrorY) plate.MirrorPlateAboutY();
        //        if (vm.Rotate_90_CW) plate.RotatePlateAboutZ_CW(90);
        //        if (vm.Rotate_90_CCW) plate.RotatePlateAboutZ_CW(-90);
                
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

                StringBuilder sb1 = CExportToNC.GetCNCFileContentForHoles(plate.DrillingRoutePoints, plate.Ft, fUnitFactor);
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
                        if (item.Name.Equals(CParamsResources.NumberOfCirclesInGroupS.Name))
                        {
                            int numberOfCirclesInGroup = int.Parse(itemStr.Value);
                            //arrangementTemp.INumberOfCirclesInGroup = int.Parse(itemStr.Value);
                            arrangementTemp.NumberOfCirclesInGroup_Updated(numberOfCirclesInGroup);
                        }

                        

                        //if (item.Name.Equals(CParamsResources.NumberOfScrewsInCircleSequenceS.Name)) arrangementTemp.INumberOfScrewsInOneHalfCircleSequence_SQ1 = int.Parse(itemStr.Value); // TODO - pre SQ1 (ale moze byt rozne podla poctu kruhov)
                        //if (item.Name.Equals(CParamsResources.RadiusOfScrewsInCircleSequenceS.Name)) arrangementTemp.FRadius_SQ1 = (float.Parse(itemStr.Value) / fLengthUnitFactor);  // TODO - pre SQ1 (ale moze byt rozne podla poctu kruhov)

                        // Corner screws                        
                        if (item.Name.Equals(CParamsResources.NumberOfAdditionalScrewsInCornerS.Name)) arrangementTemp.IAdditionalConnectorInCornerNumber = int.Parse(itemStr.Value);
                        if (item.Name.Equals(CParamsResources.DistanceOfAdditionalScrewsInxS.Name)) arrangementTemp.FAdditionalScrewsDistance_x = float.Parse(itemStr.Value) / fLengthUnitFactor;
                        if (item.Name.Equals(CParamsResources.DistanceOfAdditionalScrewsInyS.Name)) arrangementTemp.FAdditionalScrewsDistance_y = float.Parse(itemStr.Value) / fLengthUnitFactor;
                    }
                    else if (item is CComponentParamsViewBool)
                    {
                        CComponentParamsViewBool itemBool = item as CComponentParamsViewBool;
                        if (item.Name.Equals(CParamsResources.UseAdditionalCornerScrewsS.Name))
                        {
                            arrangementTemp.BUseAdditionalCornerScrews = itemBool.Value;
                            arrangementTemp.UpdateAdditionalCornerScrews();
                        }
                    }
                    else if (item is CComponentParamsViewList)
                    {
                        CComponentParamsViewList itemList = item as CComponentParamsViewList;
                        if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                    }
                                        
                    plate.ScrewArrangement = arrangementTemp;
                    RedrawComponentIn2D();
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

                    arrangementTemp.UpdateArrangmentData();
                    plate.ScrewArrangement = arrangementTemp;
                    RedrawComponentIn2D();
                }
                else
                {
                    // Screw arrangement is not implemented
                }
                vm.DrillingRoutePoints = null;
                plate.DrillingRoutePoints = null;
            }
        }

        private void DataGridGeometry_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string changedText = ((TextBox)e.EditingElement).Text;
            CComponentParamsViewString item = ((CComponentParamsViewString)e.Row.Item);
            if (changedText == item.Value) return;

            float fLengthUnitFactor = 1000; // GUI input in mm, change to m used in source code

            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;
            if (vm.ComponentTypeIndex == 0)
            {
                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                // Set current basic geometry of plate
                if (plate is CConCom_Plate_JA)
                {
                    CConCom_Plate_JA plateTemp = (CConCom_Plate_JA)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fb_X = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    if (plateTemp.ScrewArrangement is CScrewArrangementCircleApexOrKnee)
                        plateTemp.UpdatePlateData((CScrewArrangementCircleApexOrKnee)plateTemp.ScrewArrangement);
                    else if (plateTemp.ScrewArrangement is CScrewArrangementRectApexOrKnee)
                        plateTemp.UpdatePlateData((CScrewArrangementRectApexOrKnee)plateTemp.ScrewArrangement);
                    else
                    {
                        // Not Defined
                    }

                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_JB)
                {
                    CConCom_Plate_JB plateTemp = (CConCom_Plate_JB)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fb_X = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    if (plateTemp.ScrewArrangement is CScrewArrangementCircleApexOrKnee)
                        plateTemp.UpdatePlateData((CScrewArrangementCircleApexOrKnee)plateTemp.ScrewArrangement);
                    else if (plateTemp.ScrewArrangement is CScrewArrangementRectApexOrKnee)
                        plateTemp.UpdatePlateData((CScrewArrangementRectApexOrKnee)plateTemp.ScrewArrangement);
                    else
                    {
                        // Not Defined
                    }

                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KA)
                {
                    CConCom_Plate_KA plateTemp = (CConCom_Plate_KA)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    if (plateTemp.ScrewArrangement is CScrewArrangementCircleApexOrKnee)
                        plateTemp.UpdatePlateData((CScrewArrangementCircleApexOrKnee)plateTemp.ScrewArrangement);
                    else if (plateTemp.ScrewArrangement is CScrewArrangementRectApexOrKnee)
                        plateTemp.UpdatePlateData((CScrewArrangementRectApexOrKnee)plateTemp.ScrewArrangement);
                    else
                    {
                        // Not Defined
                    }
                }
                else if (plate is CConCom_Plate_KB)
                {
                    CConCom_Plate_KB plateTemp = (CConCom_Plate_KB)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData((CScrewArrangementCircleApexOrKnee)plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KC)
                {
                    CConCom_Plate_KC plateTemp = (CConCom_Plate_KC)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData((CScrewArrangementCircleApexOrKnee)plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KD)
                {
                    CConCom_Plate_KD plateTemp = (CConCom_Plate_KD)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData((CScrewArrangementCircleApexOrKnee)plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else if (plate is CConCom_Plate_KE) // Nepouzivat, kym nebude zobecnene screw arrangement
                {
                    CConCom_Plate_KE plateTemp = (CConCom_Plate_KE)plate;

                    if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = float.Parse(changedText) / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = float.Parse(changedText) / fLengthUnitFactor;

                    if (item.Name.Equals(CParamsResources.RafterWidthS.Name)) plateTemp.Fb_XR = float.Parse(changedText) / fLengthUnitFactor;

                    // Update plate data
                    plateTemp.UpdatePlateData((CScrewArrangementCircleApexOrKnee)plateTemp.ScrewArrangement);
                    plate = plateTemp;
                }
                else
                {
                    // Plate is not implemented
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Redraw plate in 2D
                Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawHoleCentreSymbol2D, vm.DrawDrillingRoute2D);
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
                }
                else
                {
                    panelOptions2D.Visibility = Visibility.Hidden;
                    panelOptionsTransform2D.Visibility = Visibility.Hidden;
                    panelOptions3D.Visibility = Visibility.Hidden;
                }                
            }
            else if(MainTabControl.SelectedIndex == 1)
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


        private void RedrawComponentIn2D()
        {
            SystemComponentViewerViewModel vm = this.DataContext as SystemComponentViewerViewModel;

            if (vm.ComponentTypeIndex == 0)
            {
                if (vm.MirrorX) crsc.MirrorCRSCAboutX();
                if (vm.MirrorY) crsc.MirrorCRSCAboutY();
                if (vm.Rotate90CW) crsc.RotateCrsc_CW(90);
                if (vm.Rotate90CCW) crsc.RotateCrsc_CW(-90);

                Drawing2D.DrawCrscToCanvas(crsc, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D);
            }
            else if (vm.ComponentTypeIndex == 1)
            {
                if (vm.MirrorX) plate.MirrorPlateAboutX();
                if (vm.MirrorY) plate.MirrorPlateAboutY();
                if (vm.Rotate90CW) plate.RotatePlateAboutZ_CW(90);
                if (vm.Rotate90CCW) plate.RotatePlateAboutZ_CW(-90);
                
                // Redraw plate in 2D
                Drawing2D.DrawPlateToCanvas(plate, Frame2DWidth, Frame2DHeight, ref page2D,
                    vm.DrawPoints2D, vm.DrawOutLine2D, vm.DrawPointNumbers2D, vm.DrawHoles2D, vm.DrawHoleCentreSymbol2D, vm.DrawDrillingRoute2D);
            }
            else // Screw
            {
                bool bDrawCentreSymbol = true;
                Drawing2D.DrawScrewToCanvas(screw, Frame2DWidth, Frame2DHeight, ref page2D, bDrawCentreSymbol);
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;
        }
    }
}