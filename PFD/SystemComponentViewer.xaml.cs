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
        float fl; // out of plane - Z coord
        float fl2; // out of plane - Z coord
        float ft;
        int iNumberofHoles;

        int selected_Type_Index;
        int selected_Serie_Index;
        int selected_Component_Index;

        public SystemComponentViewer()
        {
            InitializeComponent();
            dcomponents = new DatabaseComponents();

            // Database Connection

            Combobox_Type.Items.Add("Cross-section");
            Combobox_Type.Items.Add("Plate");
            Combobox_Type.Items.Add("Screw");

            if (Combobox_Type.SelectedIndex == 0) // Cross-sections
            {
                foreach (string seriename in dcomponents.arr_Serie_CrSc_FormSteel_Names) // Plates
                    Combobox_Series.Items.Add(seriename);

                Combobox_Series.SelectedIndex = (int)ESerieTypeCrSc_FormSteel.eSerie_Box; // TODO Temp

                switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box:
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

                Combobox_Series.SelectedIndex = (int)ESerieTypePlate.eSerie_L; // TODO Temp

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

            // Set data from database in to the Window textboxes
            SetDataFromDatabasetoWindow();

            // Load data from window
            LoadDataFromWindow();
            LoadDataFromDatabase();

            // Create Component Model
            if (Combobox_Type.SelectedIndex == 0) // Cross-sections
            {
                switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box:
                        {
                            crsc = new CCrSc_3_51_BOX_TEMP(fh, fb, ft, Colors.Aquamarine); // BOX - TODO POKUS
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            crsc = new CCrSc_3_51_C_LIP(fh, fb, 0.020f, ft, Colors.Aquamarine); // C with lip - TODO POKUS
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
                           //LL
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
            DatabaseComponents dcomponent = new DatabaseComponents(selected_Serie_Index, selected_Component_Index);
        }

        private void LoadDataFromDatabase()
        {
            if (selected_Component_Index >= 0) // Index is valid
            {
                if (selected_Type_Index == 0) // Cross-section
                {
                    switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                    {
                        case ESerieTypeCrSc_FormSteel.eSerie_Box:
                            {
                                fb = dcomponents.arr_Serie_Box_FormSteel_Dimension[selected_Component_Index, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Box_FormSteel_Dimension[selected_Component_Index, 1] / 1000f;
                                ft  = dcomponents.arr_Serie_Box_FormSteel_Dimension[selected_Component_Index, 2] / 1000f;
                                break;
                            }
                        case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                            {
                                fb = dcomponents.arr_Serie_C_FormSteel_Dimension[selected_Component_Index, 0] / 1000f;
                                fh = dcomponents.arr_Serie_C_FormSteel_Dimension[selected_Component_Index, 1] / 1000f;
                                ft = dcomponents.arr_Serie_C_FormSteel_Dimension[selected_Component_Index, 2] / 1000f;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                else if (selected_Type_Index == 1) // Plate
                {
                    switch ((ESerieTypePlate)Combobox_Series.SelectedIndex)
                    {
                        case ESerieTypePlate.eSerie_B:
                            {
                                fb = dcomponents.arr_Serie_B_Dimension[selected_Component_Index, 0] / 1000f;
                                fb2 = fb;
                                fh = dcomponents.arr_Serie_B_Dimension[selected_Component_Index, 1] / 1000f;
                                fl = dcomponents.arr_Serie_B_Dimension[selected_Component_Index, 2] / 1000f;
                                ft = dcomponents.arr_Serie_B_Dimension[selected_Component_Index, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_B_Dimension[selected_Component_Index, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_L:
                            {
                                fb = dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 0] / 1000f;
                                fb2 = fb;
                                fh = dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 1] / 1000f;
                                fl = dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 2] / 1000f;
                                ft = dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_LL:
                            {
                                //LL
                                break;
                            }
                        case ESerieTypePlate.eSerie_F:
                            {
                                fb = dcomponents.arr_Serie_F_Dimension[selected_Component_Index, 0] / 1000f;
                                fb2 = dcomponents.arr_Serie_F_Dimension[selected_Component_Index, 1] / 1000f;
                                fh = dcomponents.arr_Serie_F_Dimension[selected_Component_Index, 2] / 1000f;
                                fl = dcomponents.arr_Serie_F_Dimension[selected_Component_Index, 3] / 1000f;
                                ft = dcomponents.arr_Serie_F_Dimension[selected_Component_Index, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_F_Dimension[selected_Component_Index, 5];
                                break;
                            }
                        case ESerieTypePlate.eSerie_Q:
                            {
                                fb = dcomponents.arr_Serie_Q_Dimension[selected_Component_Index, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Q_Dimension[selected_Component_Index, 1] / 1000f;
                                fl = dcomponents.arr_Serie_Q_Dimension[selected_Component_Index, 2] / 1000f;
                                ft = dcomponents.arr_Serie_Q_Dimension[selected_Component_Index, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_Q_Dimension[selected_Component_Index, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_T:
                            {
                                fb = dcomponents.arr_Serie_T_Dimension[selected_Component_Index, 0] / 1000f;
                                fh = dcomponents.arr_Serie_T_Dimension[selected_Component_Index, 1] / 1000f;
                                fl = dcomponents.arr_Serie_T_Dimension[selected_Component_Index, 2] / 1000f;
                                ft = dcomponents.arr_Serie_T_Dimension[selected_Component_Index, 3] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_T_Dimension[selected_Component_Index, 4];
                                break;
                            }
                        case ESerieTypePlate.eSerie_Y:
                            {
                                fb = dcomponents.arr_Serie_Y_Dimension[selected_Component_Index, 0] / 1000f;
                                fh = dcomponents.arr_Serie_Y_Dimension[selected_Component_Index, 1] / 1000f;
                                fl = dcomponents.arr_Serie_Y_Dimension[selected_Component_Index, 2] / 1000f;
                                fl2 = dcomponents.arr_Serie_Y_Dimension[selected_Component_Index, 3] / 1000f;
                                ft = dcomponents.arr_Serie_Y_Dimension[selected_Component_Index, 4] / 1000f;
                                iNumberofHoles = (int)dcomponents.arr_Serie_Y_Dimension[selected_Component_Index, 5];
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

        private void LoadDataFromWindow()
        {
            selected_Type_Index = Combobox_Type.SelectedIndex;
            selected_Serie_Index = Combobox_Series.SelectedIndex;
            selected_Component_Index = Combobox_Component.SelectedIndex;
        }

        private void UpdateAll()
        {
            LoadDataFromWindow();
            LoadDataFromDatabase();

            // Create Model
            // Change Combobox
            if (selected_Type_Index == 0) // Cross-section
            {
                switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box:
                        {
                            crsc = new CCrSc_3_51_BOX_TEMP(fh, fb, ft, Colors.Aquamarine);
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            crsc = new CCrSc_3_51_C_LIP(fh, fb, 0.02f, ft, Colors.Aquamarine);
                            break;
                        }
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else if (selected_Type_Index == 1)
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
                            //LL
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
            UpdateAll();
        }

        private void Combobox_Series_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Change Component Items Combobox
            Combobox_Component.Items.Clear();

            if (selected_Type_Index == 0) // Cross-section
            {
                switch ((ESerieTypeCrSc_FormSteel)Combobox_Series.SelectedIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box:
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
                    default:
                        {
                            // Not implemented
                            break;
                        }
                }
            }
            else if (selected_Type_Index == 1)
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

            // Nastavit default
            Combobox_Component.SelectedIndex = 0;

            UpdateAll();
        }

        private void Combobox_Component_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAll();
        }
    }
}
