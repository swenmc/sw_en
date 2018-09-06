using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Globalization;
using System.Windows.Media;
using System.Collections.ObjectModel;
using BaseClasses;
using BaseClasses.GraphObj;
using CRSC;
using DATABASE;
using DATABASE.DTO;

namespace PFD
{
    public class SystemComponentViewerViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MComponentTypeIndex;
        private int MComponentSerieIndex;
        private int MComponentIndex;

        private List<string> MComponentTypes;
        private string[] MComponentSeries;
        private string[] MComponents;

        private List<Tuple<string, string, string, string>> MComponentGeometry;
        private List<Tuple<string, string, string, string>> MComponentDetails;

        public bool IsSetFromCode = false;

        private List<CSystemComponent<object>> MSystemComponents = new List<CSystemComponent<object>>();

        float fUnitFactor_Length = 1000;
        float fUnitFactor_Area = 1000000;//fUnitFactor_Length * fUnitFactor_Length;
        int iNumberOfDecimalPlaces_Length = 1;
        int iNumberOfDecimalPlaces_Area = 1;

        //-------------------------------------------------------------------------------------------------------------
        public int ComponentTypeIndex
        {
            get
            {
                return MComponentTypeIndex;
            }

            set
            {
                MComponentTypeIndex = value;
                ComponentTypeChanged();
                NotifyPropertyChanged("ComponentTypeIndex");
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        public int ComponentSerieIndex
        {
            get
            {
                return MComponentSerieIndex;
            }

            set
            {
                MComponentSerieIndex = value;
                ComponentSeriesChanged();
                NotifyPropertyChanged("ComponentSerieIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int ComponentIndex
        {
            get
            {
                return MComponentIndex;
            }

            set
            {
                MComponentIndex = value;
                NotifyPropertyChanged("ComponentIndex");
            }
        }



        public List<string> ComponentTypes
        {
            get
            {
                return MComponentTypes;
            }

            set
            {
                MComponentTypes = value;
                NotifyPropertyChanged("ComponentTypes");
            }
        }

        public string[] ComponentSeries
        {
            get
            {
                return MComponentSeries;
            }

            set
            {
                MComponentSeries = value;
                NotifyPropertyChanged("ComponentSeries");
            }
        }

        public string[] Components
        {
            get
            {
                return MComponents;
            }

            set
            {
                MComponents = value;
                NotifyPropertyChanged("Components");
            }
        }

        public List<Tuple<string, string, string, string>> ComponentGeometry
        {
            get
            {
                return MComponentGeometry;
            }

            set
            {
                MComponentGeometry = value;
                NotifyPropertyChanged("ComponentGeometry");
            }
        }

        public List<Tuple<string, string, string, string>> ComponentDetails
        {
            get
            {
                return MComponentDetails;
            }

            set
            {
                MComponentDetails = value;
                NotifyPropertyChanged("ComponentDetails");
            }
        }

        public List<CSystemComponent<object>> SystemComponents
        {
            get
            {
                return MSystemComponents;
            }

            set
            {
                MSystemComponents = value;
                NotifyPropertyChanged("SystemComponents");
            }
        }

        CDatabaseComponents databaseComponents;
        private Dictionary<string, List<CCrSc_TW>> crossSections;
        private Dictionary<string, List<CPlate>> plates;
        private Dictionary<string, List<CScrew>> screws;

        public Dictionary<string, List<CCrSc_TW>> CrossSections
        {
            get
            {
                return crossSections;
            }

            set
            {
                crossSections = value;
                NotifyPropertyChanged("CrossSections");
            }
        }
        public Dictionary<string, List<CPlate>> Plates
        {
            get
            {
                return plates;
            }

            set
            {
                plates = value;
                NotifyPropertyChanged("Plates");
            }
        }
        public Dictionary<string, List<CScrew>> Screws
        {
            get
            {
                return screws;
            }

            set
            {
                screws = value;
                NotifyPropertyChanged("Screws");
            }
        }

        private object selectedComponent;
        public object SelectedComponent
        {
            get
            {
                return selectedComponent;
            }

            set
            {
                selectedComponent = value;
                NotifyPropertyChanged("SelectedComponent");
            }
        }
        

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public SystemComponentViewerViewModel(CDatabaseComponents dcomponents)
        {
            databaseComponents = dcomponents;
            MComponentTypes = new List<string>() { "Cross-section", "Plate", "Screw" };
            MComponentSeries = dcomponents.arr_SeriesNames;
            MComponents = dcomponents.arr_Serie_L_Names;

            // Set default
            ComponentTypeIndex = 1;
            ComponentSerieIndex = 1;
            ComponentIndex = 1;

            //TODO naplnit zoznamy cross sections, plates, screws
            CreateCrossSections();
            CreatePlates();
            CreateScrews();

            IsSetFromCode = false;
        }

        //private void LoadSystemComponents()
        //{
        //    List<CSystemComponent<Type>> components = new List<CSystemComponent<Type>>();
        //    components.Add(new CSystemComponent<CRSC.CCrSc>("Cross section", list_crossSections));
        //    components.Add(new CSystemComponent<CPlate>("Plates", list_plates));
        //}

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ComponentTypeChanged()
        {
            if (ComponentTypeIndex == 0) // Cross-sections
            {
                ComponentSeries = databaseComponents.arr_Serie_CrSc_FormSteel_Names; // Plates
                ComponentSerieIndex = 0;
                Components = databaseComponents.arr_Serie_Box_FormSteel_Names;
                ComponentIndex = 0;
            }
            else if (ComponentTypeIndex == 1)
            {
                ComponentSeries = databaseComponents.arr_SeriesNames; // Plates
                ComponentSerieIndex = 0;
                Components = databaseComponents.arr_Serie_B_Names;
                ComponentIndex = 0;
            }
            else // Screws
            {
                ComponentSeries = null; // Plates
                ComponentSerieIndex = 0;
                Components = null;
                ComponentIndex = 0;

                // TODO not implemented
            }
        }

        private void ComponentSeriesChanged()
        {
            if (ComponentTypeIndex == 0) // Cross-sections
            {
                switch ((ESerieTypeCrSc_FormSteel)ComponentSerieIndex)
                {
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_10075:
                        {
                            Components = databaseComponents.arr_Serie_Box_FormSteel_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Z:
                        {
                            Components = databaseComponents.arr_Serie_Z_FormSteel_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            Components = databaseComponents.arr_Serie_C_FormSteel_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
                        {
                            Components = databaseComponents.arr_Serie_C_BtoB_FormSteel_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
                        {
                            Components = databaseComponents.arr_Serie_C_Nested_FormSteel_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            Components = databaseComponents.arr_Serie_Box63020_FormSteel_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    default:
                        {
                            // Not Implemented
                            break;
                        }
                }
            }
            else if (ComponentTypeIndex == 1) //Plate
            {
                switch ((ESerieTypePlate)ComponentSerieIndex)
                {
                    case ESerieTypePlate.eSerie_B:
                        {
                            Components = databaseComponents.arr_Serie_B_Names;
                            ComponentIndex = 0;

                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            Components = databaseComponents.arr_Serie_L_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            Components = databaseComponents.arr_Serie_LL_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            Components = databaseComponents.arr_Serie_F_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                        {
                            Components = databaseComponents.arr_Serie_Q_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_S:
                        {
                            Components = databaseComponents.arr_Serie_S_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_T:
                        {
                            Components = databaseComponents.arr_Serie_T_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_X:
                        {
                            Components = databaseComponents.arr_Serie_X_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            Components = databaseComponents.arr_Serie_Y_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            Components = databaseComponents.arr_Serie_J_Names;
                            ComponentIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            Components = databaseComponents.arr_Serie_K_Names;
                            ComponentIndex = 0;
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
        }

        public void SetComponentProperties(CPlate plate)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<Tuple<string, string, string, string>> geometry = new List<Tuple<string, string, string, string>>();
            geometry.Add(Tuple.Create("Name", "", plate.Name, ""));
            geometry.Add(Tuple.Create("Width", "b", (Math.Round(plate.fWidth_bx * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Height", "h", (Math.Round(plate.fHeight_hy * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Number of holes", "nh", plate.IHolesNumber.ToString(nfi), "[-]"));
            geometry.Add(Tuple.Create("Hole diameter", "dh", (Math.Round(plate.FHoleDiameter * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Hole radius", "rh", (Math.Round(plate.FHoleDiameter * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Thickness", "t", (Math.Round(plate.fThickness_tz * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

            ComponentGeometry = geometry;

            List<Tuple<string, string, string, string>> details = new List<Tuple<string, string, string, string>>();
            CCNCPathFinder c = new CCNCPathFinder();
            c.RoutePoints = plate.DrillingRoutePoints;
            double dist = c.GetRouteDistance();
            details.Add(Tuple.Create("Drilling Route Distance", "Ldr", (Math.Round(dist * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(Tuple.Create("Area", "A", (Math.Round(plate.fArea * fUnitFactor_Area, iNumberOfDecimalPlaces_Area)).ToString(nfi), "[mm^2]"));
            details.Add(Tuple.Create("Weight", "w", Math.Round(plate.fWeight, 2).ToString(nfi), "[kg]"));
            //TODO
            //doplnit potrebne parametre

            ComponentDetails = details;
        }
        public void SetComponentProperties(CCrSc crsc)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<Tuple<string, string, string, string>> geometry = new List<Tuple<string, string, string, string>>();
            geometry.Add(Tuple.Create("Name", "", crsc.Name, ""));
            geometry.Add(Tuple.Create("Width", "b", (Math.Round(crsc.b * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Height", "h", (Math.Round(crsc.h * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Thickness", "t", (Math.Round(crsc.t_min * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

            ComponentGeometry = geometry;

            List<Tuple<string, string, string, string>> details = new List<Tuple<string, string, string, string>>();
            List<CSectionPropertiesText> sectionTexts = CSectionManager.LoadSectionPropertiesNamesSymbolsUnits();
            List<string> listSectionPropertyValue = new List<string>();

            try
            {
                listSectionPropertyValue = CSectionManager.LoadSectionPropertiesStringList(crsc.NameDatabase);

                foreach (CSectionPropertiesText textRow in sectionTexts)
                {
                    if (listSectionPropertyValue[textRow.ID - 1] != "") // Add only row for property value is which is not empty string
                        details.Add(Tuple.Create(textRow.text, textRow.symbol, listSectionPropertyValue[textRow.ID - 1], textRow.unit_NmmMpa));
                }

                ComponentDetails = details;
            }
            catch
            {
                throw new ArgumentException("Cross section name wasn't found in the database or invalid database data.");
            }
        }

        private void CreateCrossSections()
        {
            Color cComponentColor;
            float fb;
            float fh;
            float ft;
            float ft_f;    // Flange thickness (temporary for box 63020 with stiffeners)
            float fb_fl;   // Flange - Z-section
            float fc_lip1; // LIP - Z-section

            databaseComponents = new CDatabaseComponents();

            crossSections = new Dictionary<string, List<CCrSc_TW>>();

            // BOX 10075
            List<CCrSc_TW> sectionsInSerie1 = new List<CCrSc_TW>();
            fb = databaseComponents.arr_Serie_Box_FormSteel_Dimension[0, 0] / 1000f;
            fh = databaseComponents.arr_Serie_Box_FormSteel_Dimension[0, 1] / 1000f;
            ft = databaseComponents.arr_Serie_Box_FormSteel_Dimension[0, 2] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_Box_FormSteel_Colors[0];
            sectionsInSerie1.Add(new CCrSc_3_10075_BOX(fh, fb, ft, cComponentColor));

            crossSections.Add("Box-10075", sectionsInSerie1);

            // Z
            List<CCrSc_TW> sectionsInSerie2 = new List<CCrSc_TW>();
            fh = databaseComponents.arr_Serie_Z_FormSteel_Dimension[0, 0] / 1000f;
            fb_fl = databaseComponents.arr_Serie_Z_FormSteel_Dimension[0, 1] / 1000f;
            fc_lip1 = databaseComponents.arr_Serie_Z_FormSteel_Dimension[0, 2] / 1000f;
            ft = databaseComponents.arr_Serie_Z_FormSteel_Dimension[0, 3] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_Z_FormSteel_Colors[0];
            sectionsInSerie2.Add(new CCrSc_3_Z(fh, fb_fl, fc_lip1, ft, cComponentColor));

            crossSections.Add("Z", sectionsInSerie2);


            // C single
            List<CCrSc_TW> sectionsInSerie3 = new List<CCrSc_TW>();
            fb = databaseComponents.arr_Serie_C_FormSteel_Dimension[0, 0] / 1000f;
            fh = databaseComponents.arr_Serie_C_FormSteel_Dimension[0, 1] / 1000f;
            ft = databaseComponents.arr_Serie_C_FormSteel_Dimension[0, 2] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_C_FormSteel_Colors[0];
            sectionsInSerie3.Add(new CCrSc_3_270XX_C(fh, fb, ft, cComponentColor));

            fb = databaseComponents.arr_Serie_C_FormSteel_Dimension[1, 0] / 1000f;
            fh = databaseComponents.arr_Serie_C_FormSteel_Dimension[1, 1] / 1000f;
            ft = databaseComponents.arr_Serie_C_FormSteel_Dimension[1, 2] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_C_FormSteel_Colors[1];
            sectionsInSerie3.Add(new CCrSc_3_270XX_C(fh, fb, ft, cComponentColor));

            fb = databaseComponents.arr_Serie_C_FormSteel_Dimension[2, 0] / 1000f;
            fh = databaseComponents.arr_Serie_C_FormSteel_Dimension[2, 1] / 1000f;
            ft = databaseComponents.arr_Serie_C_FormSteel_Dimension[2, 2] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_C_FormSteel_Colors[2];
            sectionsInSerie3.Add(new CCrSc_3_270XX_C(fh, fb, ft, cComponentColor));

            fb = databaseComponents.arr_Serie_C_FormSteel_Dimension[3, 0] / 1000f;
            fh = databaseComponents.arr_Serie_C_FormSteel_Dimension[3, 1] / 1000f;
            ft = databaseComponents.arr_Serie_C_FormSteel_Dimension[3, 2] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_C_FormSteel_Colors[3];
            sectionsInSerie3.Add(new CCrSc_3_50020_C(fh, fb, ft, cComponentColor));

            crossSections.Add("C-single", sectionsInSerie3);

            // C back to back
            List<CCrSc_TW> sectionsInSerie4 = new List<CCrSc_TW>();
            fb = databaseComponents.arr_Serie_C_BtoB_FormSteel_Dimension[0, 0] / 1000f;
            fh = databaseComponents.arr_Serie_C_BtoB_FormSteel_Dimension[0, 1] / 1000f;
            fc_lip1 = databaseComponents.arr_Serie_C_BtoB_FormSteel_Dimension[0, 2] / 1000f;
            ft = databaseComponents.arr_Serie_C_BtoB_FormSteel_Dimension[0, 3] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_C_BtoB_FormSteel_Colors[0];
            sectionsInSerie4.Add(new CCrSc_3_270XX_C_BACK_TO_BACK(fh, fb, fc_lip1, ft, cComponentColor));

            crossSections.Add("C-back to back", sectionsInSerie4);

            // C nested
            List<CCrSc_TW> sectionsInSerie5 = new List<CCrSc_TW>();
            fb = databaseComponents.arr_Serie_C_Nested_FormSteel_Dimension[0, 0] / 1000f;
            fh = databaseComponents.arr_Serie_C_Nested_FormSteel_Dimension[0, 1] / 1000f;
            ft = databaseComponents.arr_Serie_C_Nested_FormSteel_Dimension[0, 2] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_C_Nested_FormSteel_Colors[0];
            sectionsInSerie5.Add(new CCrSc_3_270XX_C_NESTED(fh, fb, ft, cComponentColor)); // C270115n

            fb = databaseComponents.arr_Serie_C_Nested_FormSteel_Dimension[1, 0] / 1000f;
            fh = databaseComponents.arr_Serie_C_Nested_FormSteel_Dimension[1, 1] / 1000f;
            ft = databaseComponents.arr_Serie_C_Nested_FormSteel_Dimension[1, 2] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_C_Nested_FormSteel_Colors[1];
            sectionsInSerie5.Add(new CCrSc_3_50020_C_NESTED(fh, fb, ft, cComponentColor)); // C50020n

            crossSections.Add("C-nested", sectionsInSerie5);

            // BOX 63020
            List<CCrSc_TW> sectionsInSerie6 = new List<CCrSc_TW>();
            fb = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[0, 0] / 1000f;
            fh = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[0, 1] / 1000f;
            ft = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[0, 2] / 1000f;
            ft_f = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[0, 3] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_Box63020_FormSteel_Colors[0];
            sectionsInSerie6.Add(new CCrSc_3_63020_BOX(fh, fb, ft, ft_f, cComponentColor));

            fb = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[1, 0] / 1000f;
            fh = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[1, 1] / 1000f;
            ft = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[1, 2] / 1000f;
            ft_f = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[1, 3] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_Box63020_FormSteel_Colors[1];
            sectionsInSerie6.Add(new CCrSc_3_63020_BOX(fh, fb, ft, ft_f, cComponentColor));

            fb = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[2, 0] / 1000f;
            fh = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[2, 1] / 1000f;
            ft = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[2, 2] / 1000f;
            ft_f = databaseComponents.arr_Serie_Box63020_FormSteel_Dimension[2, 3] / 1000f;
            cComponentColor = databaseComponents.arr_Serie_Box63020_FormSteel_Colors[2];
            sectionsInSerie6.Add(new CCrSc_3_63020_BOX(fh, fb, ft, ft_f, cComponentColor));

            crossSections.Add("Box-63020", sectionsInSerie6);
        }

        private void CreatePlates()
        {
            float fb_R; // Rafter Width
            float fb; // in plane XY -X coord
            float fb2; // in plane XY - X coord
            float fh; // in plane XY - Y coord
            float fh2; // in plane XY - Y coord
            float fl; // out of plane - Z coord
            float fl2; // out of plane - Z coord
            float ft;
            float fPitch_rad = 11f / 180f * (float)Math.PI; // Roof Pitch - default value (11 deg)
            int iNumberofHoles;
            float fAnchorHoleDiameter = 0.02f; // Diameter of hole, temporary for preview
            float fScrewHoleDiameter = 0.007f; // Diameter of hole, temporary for preview
            bool bUseAdditinalConnectors = true;
            int iNumberOfAdditionalConnectorsInPlate = 32; // 2*4*4
            CPoint controlpoint = new CPoint(0, 0, 0, 0, 0);

            databaseComponents = new CDatabaseComponents();

            plates = new Dictionary<string, List<CPlate>>();

            // Serie B
            List<CPlate> platesInSerie1 = new List<CPlate>();
            for (int i = 0; i < databaseComponents.arr_Serie_B_Names.Length; i++)
            {
                fb = databaseComponents.arr_Serie_B_Dimension[i, 0] / 1000f;
                fb2 = fb;
                fh = databaseComponents.arr_Serie_B_Dimension[i, 1] / 1000f;
                fl = databaseComponents.arr_Serie_B_Dimension[i, 2] / 1000f;
                ft = databaseComponents.arr_Serie_B_Dimension[i, 3] / 1000f;
                iNumberofHoles = (int)databaseComponents.arr_Serie_B_Dimension[i, 4];
                platesInSerie1.Add(new CConCom_Plate_BB_BG(databaseComponents.arr_Serie_B_Names[i], controlpoint, fb, fh, fl, ft, iNumberofHoles, fAnchorHoleDiameter, 0, 0, 0, true)); // B
            }
            plates.Add("Serie B", platesInSerie1);

            // Serie L
            List<CPlate> platesInSerie2 = new List<CPlate>();
            for (int i = 0; i < databaseComponents.arr_Serie_L_Names.Length; i++)
            {
                fb = databaseComponents.arr_Serie_L_Dimension[i, 0] / 1000f;
                fb2 = fb;
                fh = databaseComponents.arr_Serie_L_Dimension[i, 1] / 1000f;
                fl = databaseComponents.arr_Serie_L_Dimension[i, 2] / 1000f;
                ft = databaseComponents.arr_Serie_L_Dimension[i, 3] / 1000f;
                iNumberofHoles = (int)databaseComponents.arr_Serie_L_Dimension[i, 4];
                platesInSerie2.Add(new CConCom_Plate_F_or_L(databaseComponents.arr_Serie_L_Names[i], controlpoint, fb, fh, fl, ft, 0, 0, 0, iNumberofHoles, fScrewHoleDiameter, null, true)); // L
            }
            plates.Add("Serie L", platesInSerie2);

            // Serie LL
            List<CPlate> platesInSerie3 = new List<CPlate>();
            for (int i = 0; i < databaseComponents.arr_Serie_LL_Names.Length; i++)
            {
                fb = databaseComponents.arr_Serie_LL_Dimension[i, 0] / 1000f;
                fb2 = databaseComponents.arr_Serie_LL_Dimension[i, 1] / 1000f;
                fh = databaseComponents.arr_Serie_LL_Dimension[i, 2] / 1000f;
                fl = databaseComponents.arr_Serie_LL_Dimension[i, 3] / 1000f;
                ft = databaseComponents.arr_Serie_LL_Dimension[i, 4] / 1000f;
                iNumberofHoles = (int)databaseComponents.arr_Serie_LL_Dimension[i, 5];
                platesInSerie3.Add(new CConCom_Plate_LL(databaseComponents.arr_Serie_LL_Names[i], controlpoint, fb, fb2, fh, fl, ft, 0, 0, 0, iNumberofHoles, fScrewHoleDiameter, 0, true)); // LL
            }
            plates.Add("Serie LL", platesInSerie3);

            // Serie F
            List<CPlate> platesInSerie4 = new List<CPlate>();
            for (int i = 0; i < databaseComponents.arr_Serie_F_Names.Length; i++)
            {
                fb = databaseComponents.arr_Serie_F_Dimension[i, 0] / 1000f;
                fb2 = databaseComponents.arr_Serie_F_Dimension[i, 1] / 1000f;
                fh = databaseComponents.arr_Serie_F_Dimension[i, 2] / 1000f;
                fl = databaseComponents.arr_Serie_F_Dimension[i, 3] / 1000f;
                ft = databaseComponents.arr_Serie_F_Dimension[i, 4] / 1000f;
                iNumberofHoles = (int)databaseComponents.arr_Serie_F_Dimension[i, 5];
                platesInSerie4.Add(new CConCom_Plate_F_or_L(databaseComponents.arr_Serie_F_Names[i], controlpoint, i, fb, fb2, fh, fl, ft, 0f, 0f, 0f, true)); // F
            }
            plates.Add("Serie F", platesInSerie4);

            // Serie Q
            List<CPlate> platesInSerie5 = new List<CPlate>();
            for (int i = 0; i < databaseComponents.arr_Serie_Q_Names.Length; i++)
            {
                fb = databaseComponents.arr_Serie_Q_Dimension[i, 0] / 1000f;
                fh = databaseComponents.arr_Serie_Q_Dimension[i, 1] / 1000f;
                fl = databaseComponents.arr_Serie_Q_Dimension[i, 2] / 1000f;
                ft = databaseComponents.arr_Serie_Q_Dimension[i, 3] / 1000f;
                iNumberofHoles = (int)databaseComponents.arr_Serie_Q_Dimension[i, 4];
                platesInSerie5.Add(new CConCom_Plate_Q_T_Y(databaseComponents.arr_Serie_Q_Names[i], controlpoint, fb, fh, fl, ft, iNumberofHoles, true)); // Q
            }
            plates.Add("Serie Q", platesInSerie5);

            // Serie T
            List<CPlate> platesInSerie6 = new List<CPlate>();
            for (int i = 0; i < databaseComponents.arr_Serie_T_Names.Length; i++)
            {
                fb = databaseComponents.arr_Serie_T_Dimension[i, 0] / 1000f;
                fh = databaseComponents.arr_Serie_T_Dimension[i, 1] / 1000f;
                fl = databaseComponents.arr_Serie_T_Dimension[i, 2] / 1000f;
                ft = databaseComponents.arr_Serie_T_Dimension[i, 3] / 1000f;
                iNumberofHoles = (int)databaseComponents.arr_Serie_T_Dimension[i, 4];
                platesInSerie6.Add(new CConCom_Plate_Q_T_Y(databaseComponents.arr_Serie_T_Names[i], controlpoint, fb, fh, fl, ft, iNumberofHoles, true)); // T
            }
            plates.Add("Serie T", platesInSerie6);

            // Serie Y
            List<CPlate> platesInSerie7 = new List<CPlate>();
            for (int i = 0; i < databaseComponents.arr_Serie_Y_Names.Length; i++)
            {
                fb = databaseComponents.arr_Serie_Y_Dimension[i, 0] / 1000f;
                fh = databaseComponents.arr_Serie_Y_Dimension[i, 1] / 1000f;
                fl = databaseComponents.arr_Serie_Y_Dimension[i, 2] / 1000f;
                fl2 = databaseComponents.arr_Serie_Y_Dimension[i, 3] / 1000f;
                ft = databaseComponents.arr_Serie_Y_Dimension[i, 4] / 1000f;
                iNumberofHoles = (int)databaseComponents.arr_Serie_Y_Dimension[i, 5];
                platesInSerie7.Add(new CConCom_Plate_Q_T_Y(databaseComponents.arr_Serie_Y_Names[i], controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles, true)); // Y
            }
            plates.Add("Serie Y", platesInSerie7);

            // Serie J
            List<CPlate> platesInSerie8 = new List<CPlate>();
            fb = databaseComponents.arr_Serie_J_Dimension[0, 0] / 1000f;
            fh = databaseComponents.arr_Serie_J_Dimension[0, 1] / 1000f;
            fh2 = databaseComponents.arr_Serie_J_Dimension[0, 2] / 1000f;
            fl = databaseComponents.arr_Serie_J_Dimension[0, 3] / 1000f;
            ft = databaseComponents.arr_Serie_J_Dimension[0, 4] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_J_Dimension[0, 5];
            platesInSerie8.Add(new CConCom_Plate_JA(databaseComponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, iNumberofHoles, fScrewHoleDiameter, 0, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true));

            fb = databaseComponents.arr_Serie_J_Dimension[1, 0] / 1000f;
            fh = databaseComponents.arr_Serie_J_Dimension[1, 1] / 1000f;
            fh2 = databaseComponents.arr_Serie_J_Dimension[1, 2] / 1000f;
            fl = databaseComponents.arr_Serie_J_Dimension[1, 3] / 1000f;
            ft = databaseComponents.arr_Serie_J_Dimension[1, 4] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_J_Dimension[1, 5];
            platesInSerie8.Add(new CConCom_Plate_JB(databaseComponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, fPitch_rad, 0, 0, 0, iNumberofHoles, fScrewHoleDiameter, 0, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true));

            plates.Add("Serie J", platesInSerie8);

            // Serie K
            List<CPlate> platesInSerie9 = new List<CPlate>();
            fb_R = databaseComponents.arr_Serie_K_Dimension[0, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[0, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[0, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[0, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[0, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[0, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[0, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[0, 7];
            platesInSerie9.Add(new CConCom_Plate_KA(databaseComponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, iNumberofHoles, fScrewHoleDiameter, 0, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true));

            fb_R = databaseComponents.arr_Serie_K_Dimension[1, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[1, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[1, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[1, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[1, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[1, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[1, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[1, 7];
            platesInSerie9.Add(new CConCom_Plate_KB(databaseComponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, fScrewHoleDiameter, 0, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true));

            fb_R = databaseComponents.arr_Serie_K_Dimension[2, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[2, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[2, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[2, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[2, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[2, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[2, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[2, 7];
            platesInSerie9.Add(new CConCom_Plate_KC(databaseComponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, iNumberofHoles, fScrewHoleDiameter, 0, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate, true));

            fb_R = databaseComponents.arr_Serie_K_Dimension[3, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[3, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[3, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[3, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[3, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[3, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[3, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[3, 7];
            platesInSerie9.Add(new CConCom_Plate_KD(databaseComponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, true));

            fb_R = databaseComponents.arr_Serie_K_Dimension[4, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[4, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[4, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[4, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[4, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[4, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[4, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[4, 7];
            platesInSerie9.Add(new CConCom_Plate_KE(databaseComponents.arr_Serie_K_Names[4], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, true));

            plates.Add("Serie K", platesInSerie9);
        }

        private void CreateScrews()
        {
            int iGauge;
            float fHoleDiameter;
            float fConnectorLength = 0.02f;
            float fConnectorWeight = 0.012f;
            CPoint controlpoint = new CPoint(0, 0, 0, 0, 0);

            databaseComponents = new CDatabaseComponents();

            screws = new Dictionary<string, List<CScrew>>();

            // Serie TEK
            List<CScrew> screwsInSerie = new List<CScrew>();
            for (int i = 0; i < databaseComponents.arr_Serie_TEK_Names.Length; i++)
            {
                iGauge = (int)databaseComponents.arr_Screws_TEK_Dimensions[i, 0];
                fHoleDiameter = databaseComponents.arr_Screws_TEK_Dimensions[i, 1] / 1000f;

                screwsInSerie.Add(new CScrew(databaseComponents.arr_Serie_TEK_Names[i], controlpoint, iGauge, fHoleDiameter, fConnectorLength, fConnectorWeight, true));
            }
            screws.Add("Hex Head Tek", screwsInSerie);
        }
    }
}
