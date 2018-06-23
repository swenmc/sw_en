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
using BaseClasses;
using BaseClasses.GraphObj;
using sw_en_GUI;
using CRSC;

namespace PFD
{
    /// <summary>
    /// Interaction logic for System_Component_Viewer.xaml
    /// </summary>
    public partial class SystemComponentViewer : Window
    {
        public DatabaseComponents dcomponents; // Todo nahradit databazov component
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
            WindowCrossSection2D page2D = null;

            if (Combobox_Type.SelectedIndex == 0)
            {
               page2D = new WindowCrossSection2D(crsc);
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                page2D = new WindowCrossSection2D(component);
            }
            else
            {
                // Screw - not implemented
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D.Content;

            // Create 3D window
            Page3Dmodel page3D = null;

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
                                fb = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fh2 = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                ft = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_J_Dimension[Combobox_Component.SelectedIndex, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_K:
                            {
                                fb = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 0] / 1000f;
                                fh = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 1] / 1000f;
                                fb2 = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 2] / 1000f;
                                fh2 = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 3] / 1000f;
                                ft = dcomponents.arr_Serie_B_Dimension[Combobox_Component.SelectedIndex, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_K_Dimension[Combobox_Component.SelectedIndex, 5];
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
                            component = new BaseClasses.CConCom_Plate_F_or_L(controlpoint, fb, fb2, fh, fl, ft, iNumberofHoles, true); // F
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                    case ESerieTypePlate.eSerie_T:
                        {
                            component = new BaseClasses.CConCom_Plate_Q_T_Y(controlpoint, fb, fh, fl, ft, iNumberofHoles, true); // F
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            component = new BaseClasses.CConCom_Plate_Q_T_Y(controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles, true); // F
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            component = new BaseClasses.CConCom_Plate_JA(controlpoint, fb, fh, fh2, ft, iNumberofHoles, true);
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            component = new BaseClasses.CConCom_Plate_KA(controlpoint, fb, fh, fb2, fh2, ft, iNumberofHoles, true);
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
            WindowCrossSection2D page2D = null;

            if (Combobox_Type.SelectedIndex == 0)
            {
                page2D = new WindowCrossSection2D(crsc);
            }
            else if (Combobox_Type.SelectedIndex == 1)
            {
                page2D = new WindowCrossSection2D(component);
            }
            else
            {
                // Screw - not implemented
            }

            // Display plate in 2D preview frame
            Frame2D.Content = page2D.Content;

            // Create 3D window
            Page3Dmodel page3D = null;

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
                            component = new BaseClasses.CConCom_Plate_F_or_L(controlpoint, fb, fb2, fh, fl, ft, iNumberofHoles, true); // F
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
                            component = new BaseClasses.CConCom_Plate_Q_T_Y(controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles, true); //  Y
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            component = new BaseClasses.CConCom_Plate_JA(controlpoint, fb, fh, fh2, ft, iNumberofHoles, true);
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            component = new BaseClasses.CConCom_Plate_KA(controlpoint, fb, fh, fb2, fh2, ft, iNumberofHoles, true);
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
        }
        
        private void Combobox_Component_DropDownClosed(object sender, EventArgs e)
        {
            UpdateAll();
        }
    }
}
