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
using WindowsShapes = System.Windows.Shapes;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using BaseClasses;
using BaseClasses.GraphObj;
using sw_en_GUI;
using CRSC;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using System.Windows.Media.Media3D;
using _3DTools;

namespace PFD
{
    /// <summary>
    /// Interaction logic for System_Component_Viewer.xaml
    /// </summary>
    public partial class SystemComponentViewer : Window
    {
        public DatabaseComponents dcomponents; // Todo nahradit databazov component
        public WindowCrossSection2D page2D;
        public Page3Dmodel page3D;

        CCrSc_TW crsc;
        CPlate component;
        CPoint controlpoint = new CPoint(0, 0, 0, 0, 0);
        float fb; // in plane XY -X coord
        float fb2; // in plane XY - X coord
        float fh; // in plane XY - Y coord
        float fh2; // in plane XY - Y coord
        float fl; // out of plane - Z coord
        float fl2; // out of plane - Z coord
        float ft;
        float ft_f;
        float fPitch_rad =  11f / 180f * (float)Math.PI; // Roof Pitch - default value (11 deg)
        int iNumberofHoles;

        public SystemComponentViewer()
        {
            InitializeComponent();
            dcomponents = new DatabaseComponents();

            Combobox_Type.Items.Add("Cross-section");
            Combobox_Type.Items.Add("Plate");
            Combobox_Type.Items.Add("Screw");

            // Set data from database in to the GUI
            SetDataFromDatabasetoWindow();

            // Load data from database
            LoadDataFromDatabase();

            // Create Component Model
            CreateModelObject();

            // Create 2D page
            page2D = null;

            double dWidth = Frame2D.Width;
            double dHeight = Frame2D.Height;

            if (Combobox_Type.SelectedIndex == 0)
            {
               page2D = new WindowCrossSection2D(crsc, dWidth, dHeight);
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                page2D = new WindowCrossSection2D(component, dWidth, dHeight);
            }
            else
            {
                // Screw - not implemented
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D.Content;

            // Create 3D window
            page3D = null;

            if (Combobox_Type.SelectedIndex == 0)
            {
                page3D = new Page3Dmodel(crsc);
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                page3D = new Page3Dmodel(component);
            }
            else
            {
                // Screw - not implemented
            }

            // Display model in 3D preview frame
            Frame3D.Content = page3D;
        }

        private void SetDataFromDatabasetoWindow()
        {
            if (Combobox_Type.SelectedIndex == 0) // Cross-sections
            {
                foreach (string seriename in dcomponents.arr_Serie_CrSc_FormSteel_Names) // Plates
                    Combobox_Series.Items.Add(seriename);

                Combobox_Series.SelectedIndex = (int)ESerieTypeCrSc_FormSteel.eSerie_Box_10075; // TODO Temp

                switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                        {
                            foreach (string name in dcomponents.arr_Serie_Box_FormSteel_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            foreach (string name in dcomponents.arr_Serie_C_FormSteel_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            foreach (string name in dcomponents.arr_Serie_Box63020_FormSteel_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    default:
                        {
                            // Not Implemented
                            break;
                        }
                }
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                foreach (string seriename in dcomponents.arr_SeriesNames) // Plates
                    Combobox_Series.Items.Add(seriename);

                Combobox_Series.SelectedIndex = (int)ESerieTypePlate.eSerie_B; // TODO Temp

                switch ((ESerieTypePlate)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            foreach (string name in dcomponents.arr_Serie_B_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            foreach (string name in dcomponents.arr_Serie_L_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            foreach (string name in dcomponents.arr_Serie_LL_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            foreach (string name in dcomponents.arr_Serie_F_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                        {
                            foreach (string name in dcomponents.arr_Serie_Q_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_S:
                        {
                            foreach (string name in dcomponents.arr_Serie_S_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_T:
                        {
                            foreach (string name in dcomponents.arr_Serie_T_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_X:
                        {
                            foreach (string name in dcomponents.arr_Serie_X_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            foreach (string name in dcomponents.arr_Serie_Y_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            foreach (string name in dcomponents.arr_Serie_J_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            foreach (string name in dcomponents.arr_Serie_K_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else // Screws
            {
                // TODO not implemented
            }

            if(Combobox_Component.Items.Count > 0) Combobox_Component.SelectedIndex = 0;
        }

        private void LoadDataFromDatabase()
        {
            if (Combobox_Component.SelectedIndex >= 0) // Index is valid
            {
                if (Combobox_Type.SelectedIndex == 0) // Cross-section
                {
                    switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                    {
                        case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                            {
                                fb = dcomponents.arr_Serie_Box_FormSteel_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Box_FormSteel_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                ft = dcomponents.arr_Serie_Box_FormSteel_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                            {
                                fb = dcomponents.arr_Serie_C_FormSteel_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_C_FormSteel_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                ft = dcomponents.arr_Serie_C_FormSteel_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                            {
                                fb = dcomponents.arr_Serie_Box63020_FormSteel_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Box63020_FormSteel_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                ft = dcomponents.arr_Serie_Box63020_FormSteel_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                ft_f = dcomponents.arr_Serie_Box63020_FormSteel_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
                        case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
                        case ESerieTypeCrSc_FormSteel.eSerie_PurlinDek:
                        case ESerieTypeCrSc_FormSteel.eSerie_SmartDek:
                        case ESerieTypeCrSc_FormSteel.eSerie_Z:
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
                else if (Combobox_Type.SelectedIndex == 1) // Plate
                {
                    switch ((ESerieTypePlate)Combobox_Series.SelectedIndex)
                    {
                        case ESerieTypePlate.eSerie_B:
                            {
                                fb = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fb2 = fb;
                                fh = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fl = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_L:
                            {
                                fb = dcomponents.arr_Serie_L_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fb2 = fb;
                                fh = dcomponents.arr_Serie_L_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fl = dcomponents.arr_Serie_L_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_L_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_L_Dimension[Combobox_Component.SelectedIndex, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_LL:
                            {
                                fb = dcomponents.arr_Serie_LL_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fb2 = dcomponents.arr_Serie_LL_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fh = dcomponents.arr_Serie_LL_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                fl = dcomponents.arr_Serie_LL_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                ft = dcomponents.arr_Serie_LL_Dimension[Combobox_Component.SelectedIndex, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_LL_Dimension[Combobox_Component.SelectedIndex, 5];
                                break;
                            }
                        case ESerieTypePlate.eSerie_F:
                            {
                                fb = dcomponents.arr_Serie_F_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fb2 = dcomponents.arr_Serie_F_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fh = dcomponents.arr_Serie_F_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                fl = dcomponents.arr_Serie_F_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                ft = dcomponents.arr_Serie_F_Dimension[Combobox_Component.SelectedIndex, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_F_Dimension[Combobox_Component.SelectedIndex, 5];
                                break;
                            }
                        case ESerieTypePlate.eSerie_Q:
                            {
                                fb = dcomponents.arr_Serie_Q_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Q_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fl = dcomponents.arr_Serie_Q_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_Q_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_Q_Dimension[Combobox_Component.SelectedIndex, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_T:
                            {
                                fb = dcomponents.arr_Serie_T_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_T_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fl = dcomponents.arr_Serie_T_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_T_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_T_Dimension[Combobox_Component.SelectedIndex, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_Y:
                            {
                                fb = dcomponents.arr_Serie_Y_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Y_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fl = dcomponents.arr_Serie_Y_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                fl2 = dcomponents.arr_Serie_Y_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                ft = dcomponents.arr_Serie_Y_Dimension[Combobox_Component.SelectedIndex, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_Y_Dimension[Combobox_Component.SelectedIndex, 5];
                                break;
                            }
                        case ESerieTypePlate.eSerie_J:
                            {
                                fb = dcomponents.arr_Serie_J_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_J_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fh2 = dcomponents.arr_Serie_J_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                fl = dcomponents.arr_Serie_J_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                ft = dcomponents.arr_Serie_J_Dimension[Combobox_Component.SelectedIndex, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_J_Dimension[Combobox_Component.SelectedIndex, 5];
                                break;
                            }
                        case ESerieTypePlate.eSerie_K:
                            {
                                fb = dcomponents.arr_Serie_K_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_K_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fb2 = dcomponents.arr_Serie_K_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                fh2 = dcomponents.arr_Serie_K_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                fl = dcomponents.arr_Serie_K_Dimension[Combobox_Component.SelectedIndex, 4] / 1000f;
                                ft = dcomponents.arr_Serie_K_Dimension[Combobox_Component.SelectedIndex, 5] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_K_Dimension[Combobox_Component.SelectedIndex, 6];
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
                    // Not implemented
                }
            }
        }

        private void UpdateAll()
        {
            LoadDataFromDatabase();

            // Create Model
            // Change Combobox
            if (Combobox_Type.SelectedIndex == 0) // Cross-section
            {
                switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                        {
                            //temp test 
                            //crsc = new CCrSc_3_10075_BOX(fh * 3 , fb * 3, ft * 3, Colors.Red); 

                            crsc = new CCrSc_3_10075_BOX(fh, fb, ft, Colors.Aquamarine); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            if(Combobox_Component.SelectedIndex < 3) // C270
                                crsc = new CCrSc_3_270XX_C(fh, fb, ft, Colors.Aquamarine);
                            else
                                crsc = new CCrSc_3_50020_C(fh, fb, ft, Colors.Aquamarine);

                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
                        {
                            crsc = new CCrSc_3_270XX_C_BACK_TO_BACK(fh, fb, ft, Colors.Aquamarine);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
                        {
                            crsc = new CCrSc_3_270XX_C_NESTED(fh, fb, ft, Colors.Aquamarine);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Z:
                        {
                            crsc = new CCrSc_3_Z(fh, fb, ft, Colors.Aquamarine);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            crsc = new CCrSc_3_63020_BOX(fh, fb, ft, ft_f, Colors.Aquamarine); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_SmartDek:
                        {
                            crsc = new CCrSc_3_TR_SMARTDEK(fh, fb, ft, Colors.Aquamarine); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_PurlinDek:
                        {
                            crsc = new CCrSc_3_TR_PURLINDEK(fh, fb, ft, Colors.Aquamarine); // BOX
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                switch ((ESerieTypePlate)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            component = new BaseClasses.CConCom_Plate_BB_BG(controlpoint, fb, fh, fl, ft, iNumberofHoles, 0.02f, true); // L
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            component = new BaseClasses.CConCom_Plate_F_or_L(controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // L
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            component = new BaseClasses.CConCom_Plate_LL(controlpoint, fb, fb2, fh, fl, ft, iNumberofHoles, true); // LL
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            component = new BaseClasses.CConCom_Plate_F_or_L(controlpoint, Combobox_Component.SelectedIndex, fb, fb2, fh, fl, ft, iNumberofHoles, true); // F
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                    case ESerieTypePlate.eSerie_T:
                        {
                            component = new BaseClasses.CConCom_Plate_Q_T_Y(controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // Q T
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            component = new BaseClasses.CConCom_Plate_Q_T_Y(controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles, true); // Y
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            if (Combobox_Component.SelectedIndex == 0) // JA
                                component = new BaseClasses.CConCom_Plate_JA(controlpoint, fb, fh, fh2, ft, iNumberofHoles, true);
                            else
                                component = new BaseClasses.CConCom_Plate_JB(controlpoint, fb, fh, fh2, fl, ft, fPitch_rad, iNumberofHoles, true);
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            if (Combobox_Component.SelectedIndex == 0) // KA
                                component = new BaseClasses.CConCom_Plate_KA(controlpoint, fb, fh, fb2, fh2, ft, iNumberofHoles, true);
                            else
                                component = new BaseClasses.CConCom_Plate_KB(controlpoint, fb, fh, fb2, fh2, fl, ft, iNumberofHoles, true);
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
                 // Not implemented
            }

            // Create 2D page
            page2D = null;

            if (Combobox_Type.SelectedIndex == 0)
            {
                page2D = new WindowCrossSection2D(crsc, Frame2D.Width, Frame2D.Height);
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                page2D = new WindowCrossSection2D(component, Frame2D.Width, Frame2D.Height);
            }
            else
            {
                // Screw - not implemented
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D.Content;

            // Create 3D window
            page3D = null;

            if (Combobox_Type.SelectedIndex == 0)
            {
                page3D = new Page3Dmodel(crsc);
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                page3D = new Page3Dmodel(component);
            }
            else
            {
                // Screw - not implemented
            }

            // Display model in 3D preview frame
            Frame3D.Content = page3D;

            this.UpdateLayout();
        }

        private void Combobox_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Combobox_Series.Items.Clear();
            Combobox_Component.Items.Clear();

            SetDataFromDatabasetoWindow();

            UpdateAll();
        }

        public void CreateModelObject()
        {
            if (Combobox_Type.SelectedIndex == 0) // Cross-sections
            {
                switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                        {
                            crsc = new CCrSc_3_10075_BOX(fh, fb, ft, Colors.Aquamarine); // BOX
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            if (Combobox_Component.SelectedIndex < 3) // C270
                                crsc = new CCrSc_3_270XX_C(fh, fb, ft, Colors.Aquamarine);
                            else
                                crsc = new CCrSc_3_50020_C(fh, fb, ft, Colors.Aquamarine);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            crsc = new CCrSc_3_63020_BOX(fh, fb, ft, ft_f, Colors.Aquamarine); // Box
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else if (Combobox_Type.SelectedIndex == 1) // Plates
            {
                switch ((ESerieTypePlate)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            component = new BaseClasses.CConCom_Plate_BB_BG(controlpoint, fb, fh, fl, ft, iNumberofHoles, 0.02f, true); // B - TODO pridat vsetky typy, zatial len BB a BG, pridat do databazy rozmery dier
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            component = new BaseClasses.CConCom_Plate_F_or_L(controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // L
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            component = new BaseClasses.CConCom_Plate_LL(controlpoint, fb, fb2, fh, fl, ft, iNumberofHoles, true); // LL
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            component = new BaseClasses.CConCom_Plate_F_or_L(controlpoint, Combobox_Component.SelectedIndex, fb, fb2, fh, fl, ft, iNumberofHoles, true); // F
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                    case ESerieTypePlate.eSerie_T:
                        {
                            component = new BaseClasses.CConCom_Plate_Q_T_Y(controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // Q, T
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            component = new BaseClasses.CConCom_Plate_Q_T_Y(controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles, true); // Y
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            if (Combobox_Component.SelectedIndex == 0) // JA
                                component = new BaseClasses.CConCom_Plate_JA(controlpoint, fb, fh, fh2, ft, iNumberofHoles, true);
                            else
                                component = new BaseClasses.CConCom_Plate_JB(controlpoint, fb, fh, fh2, fl, ft, fPitch_rad, iNumberofHoles, true);
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            if (Combobox_Component.SelectedIndex == 0) // KA
                                component = new BaseClasses.CConCom_Plate_KA(controlpoint, fb, fh, fb2, fh2, ft, iNumberofHoles, true);
                            else
                                component = new BaseClasses.CConCom_Plate_KB(controlpoint, fb, fh, fb2, fh2, fl, ft, iNumberofHoles, true);
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
                // Not Implemented
            }
        }

        private void Combobox_Series_DropDownClosed(object sender, EventArgs e)
        {
            // Change Component Items Combobox
            Combobox_Component.Items.Clear();

            if (Combobox_Type.SelectedIndex == 0) // Cross-section
            {
                switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                        {
                            foreach (string name in dcomponents.arr_Serie_Box_FormSteel_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            foreach (string name in dcomponents.arr_Serie_C_FormSteel_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            foreach (string name in dcomponents.arr_Serie_Box63020_FormSteel_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                switch ((ESerieTypePlate)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            foreach (string name in dcomponents.arr_Serie_B_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            foreach (string name in dcomponents.arr_Serie_L_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            foreach (string name in dcomponents.arr_Serie_LL_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            foreach (string name in dcomponents.arr_Serie_F_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                        {
                            foreach (string name in dcomponents.arr_Serie_Q_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_S:
                        {
                            foreach (string name in dcomponents.arr_Serie_S_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_T:
                        {
                            foreach (string name in dcomponents.arr_Serie_T_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_X:
                        {
                            foreach (string name in dcomponents.arr_Serie_X_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            foreach (string name in dcomponents.arr_Serie_Y_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            foreach (string name in dcomponents.arr_Serie_J_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            foreach (string name in dcomponents.arr_Serie_K_Names)
                                Combobox_Component.Items.Add(name); // Add values into the combobox
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
                // Not implemented
            }

            // Set default
            Combobox_Component.SelectedIndex = 0;
            if(Combobox_Component.Items.Count > 0) Combobox_Component.SelectedItem = Combobox_Component.Items[0];

            UpdateAll();
        }

        private void Combobox_Series_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // To Ondrej; OK, ale teraz to nefunguje pokial nad comboboxom pouzijem wheel, preview sa nepregeneruje
            // Takze musime zakazat wheel alebo to update / SelectionChanged oddelit od pridania poloziek do comboboxu, aby sa to nevolalo opakovane

            //// Change Component Items Combobox
            //Combobox_Component.Items.Clear();

            //if (Combobox_Type.SelectedIndex == 0) // Cross-section
            //{
            //    switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
            //    {
            //        case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_Box_FormSteel_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypeCrSc_FormSteel.eSerie_C_single:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_C_FormSteel_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_Box63020_FormSteel_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        default:
            //            {
            //                // Not implemented
            //                break;
            //            }
            //    }
            //}
            //else if (Combobox_Type.SelectedIndex == 1)
            //{
            //    switch ((ESerieTypePlate)Combobox_Series.SelectedIndex)
            //    {
            //        case ESerieTypePlate.eSerie_B:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_B_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_L:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_L_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_LL:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_LL_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_F:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_F_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_Q:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_Q_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_S:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_S_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_T:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_T_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_X:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_X_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_Y:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_Y_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_J:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_J_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        case ESerieTypePlate.eSerie_K:
            //            {
            //                foreach (string name in dcomponents.arr_Serie_K_Names)
            //                    Combobox_Component.Items.Add(name); // Add values into the combobox
            //                break;
            //            }
            //        default:
            //            {
            //                // Not implemented
            //                break;
            //            }
            //    }
            //}
            //else
            //{
            //    // Not implemented
            //}


            //// Set default
            //Combobox_Component.SelectedIndex = 0;

            //UpdateAll();
        }

        private void Combobox_Component_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //ani nahodou, sak sa to vola po kazdom pridani prvku do comboboxu
            //UpdateAll();

            // To Ondrej; OK, ale teraz to nefunguje pokial nad comboboxom pouzijem wheel, preview sa nepregeneruje
            // Takze musime zakazat wheel alebo to updateall / SelectionChanged oddelit od pridania poloziek do comboboxu, aby sa to nevolalo opakovane
        }

        private void Combobox_Component_DropDownClosed(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void BtnExportDXF_Click(object sender, RoutedEventArgs e)
        {
            Canvas canvas = page2D.CanvasSection2D;
            DxfDocument doc = new DxfDocument();
            double Z = 0; //is is 2D so Z axis is always 0
            double fontSize = 10;

            foreach (object o in canvas.Children)
            {
                System.Diagnostics.Trace.WriteLine(o.GetType());

                if (o is WindowsShapes.Rectangle)
                {
                    WindowsShapes.Rectangle winRect = o as WindowsShapes.Rectangle;
                    double x = Canvas.GetLeft(winRect);
                    double y = Canvas.GetTop(winRect);
                    //double y = Canvas.GetTop(winRect) * -1; pretocenim podla osi y dostanem body tak ako v canvase

                    //System.Windows.Point p = winRect.RenderedGeometry.Bounds.Location;
                    System.Windows.Point p1 = winRect.RenderedGeometry.Bounds.TopLeft;
                    System.Windows.Point p2 = winRect.RenderedGeometry.Bounds.BottomRight;
                    
                    Wipeout wip = new Wipeout(new Vector2(p1.X + x, p1.Y + y), new Vector2(p2.X + x, p2.Y + y));
                    //wip.Color = AciColor.Red;
                    //wip.Lineweight = Lineweight.W200;
                    
                    doc.AddEntity(wip);
                }
                else if (o is WindowsShapes.Polyline)
                {
                    WindowsShapes.Polyline winPol = o as WindowsShapes.Polyline;
                    Polyline poly = new Polyline();

                    foreach (System.Windows.Point p in winPol.Points)
                    {
                        poly.Vertexes.Add(new PolylineVertex(p.X, p.Y, Z));
                    }
                    
                    doc.AddEntity(poly);
                }
                else if (o is WindowsShapes.Ellipse)
                {
                    WindowsShapes.Ellipse winElipse = o as WindowsShapes.Ellipse;
                    double majorAxis = winElipse.Width;
                    double minorAxis = winElipse.Height;

                    System.Windows.Point p1 = winElipse.RenderedGeometry.Bounds.TopLeft;
                    System.Windows.Point p2 = winElipse.RenderedGeometry.Bounds.BottomRight;
                    System.Windows.Point pCenter = new System.Windows.Point((p2.X - p1.X) / 2, (p2.Y - p1.Y) / 2);

                    double x = Canvas.GetLeft(winElipse);
                    double y = Canvas.GetTop(winElipse);
                    Ellipse elipse = new Ellipse(new Vector2(pCenter.X + x, pCenter.Y + y), majorAxis, minorAxis);
                    
                    doc.AddEntity(elipse);
                }
                else if (o is WindowsShapes.Line)
                {
                    WindowsShapes.Line winLine = o as WindowsShapes.Line;
                    
                    Vector2 startPoint = new Vector2(winLine.X1, winLine.Y1);
                    Vector2 endPoint = new Vector2(winLine.X2, winLine.Y2);
                    Line line = new Line(startPoint, endPoint);
                    
                    doc.AddEntity(line);
                }
                else if (o is System.Windows.Controls.TextBlock)
                {
                    System.Windows.Controls.TextBlock winText = o as System.Windows.Controls.TextBlock;
                    
                    double x = Canvas.GetLeft(winText);
                    x += winText.ActualWidth / 2;
                    double y = Canvas.GetTop(winText);
                    y -= winText.BaselineOffset;
                    y += fontSize / 2;
                    
                    Text txt = new Text(winText.Text, new Vector2(x, y), fontSize);
                    //Text txt = new Text(winText.Text, new Vector2(x, -y), fontSize);  //pretocenim podla osi y dostanem body tak ako v canvase
                    txt.Color = AciColor.Yellow;
                    doc.AddEntity(txt);

                    //Takto sa da spravit zlozitejsi text, napr. Bold atd..
                    /*TextStyle style = new TextStyle("Times.ttf");
                    //TextStyle style = TextStyle.Default;
                    MText mText = new MText(new Vector2(x, y), fontSize, 100.0f, style);
                    mText.Layer = new Layer("Multiline Text");
                    //mText.Layer.Color.Index = 8;
                    mText.Rotation = 0;
                    //mText.LineSpacingFactor = 1.0;
                    //mText.ParagraphHeightFactor = 1.0;
                    //mText.AttachmentPoint = MTextAttachmentPoint.TopCenter;

                    MTextFormattingOptions options = new MTextFormattingOptions(mText.Style);
                    options.Bold = true;
                    options.Color = AciColor.Yellow;
                    mText.Write(winText.Text, options);
                    mText.EndParagraph();
                    doc.AddEntity(mText);    */
                }


            }

            DateTime d = DateTime.Now;
            string fileName = string.Format("ExportDXF_{0}{1}{2}T{3}{4}{5}.dxf", 
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

            doc.Save(fileName);
        }

        private void BtnExportDXF_3D_Click(object sender, RoutedEventArgs e)
        {               
            DxfDocument doc = new DxfDocument();
            foreach (Visual3D objVisual3D in page3D._trackport.ViewPort.Children)
            {
                if (objVisual3D is ScreenSpaceLines3D)
                {
                    ScreenSpaceLines3D lines3D = objVisual3D as ScreenSpaceLines3D;
                    if (lines3D == null) continue;

                    AddLinesToDXF(lines3D, doc);
                }
            }            

            DateTime d = DateTime.Now;
            string fileName = string.Format("3DExportDXF_{0}{1}{2}T{3}{4}{5}.dxf",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

            doc.Save(fileName);
        }

        private void AddLinesToDXF(ScreenSpaceLines3D lines3D, DxfDocument doc)
        {
            Point3D startPoint;
            Point3D endPoint;
            for (int i = 0; i < lines3D.Points.Count; i = i + 2)
            {
                startPoint = lines3D.Points[i];
                endPoint = lines3D.Points[i + 1];
                Line line = new Line(new Vector3(startPoint.X, startPoint.Y, startPoint.Z), new Vector3(endPoint.X, endPoint.Y, endPoint.Z));

                doc.AddEntity(line);
            }
        }





    }
}
