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

        private List<CComponentParamsView> MComponentGeometry;
        private List<CComponentParamsView> MComponentDetails;

        bool bDrawPoints;
        bool bDrawOutLine;
        bool bDrawPointNumbers;
        bool bDrawHoles;
        bool bDrawHoleCentreSymbol;
        bool bDrawDrillingRoute;

        bool bMirrorY;
        bool bMirrorX;
        bool bRotate_90_CW;
        bool bRotate_90_CCW;

        public bool IsSetFromCode = false;
        
        float fUnitFactor_Length = 1000;
        float fUnitFactor_Area = 1000000;//fUnitFactor_Length * fUnitFactor_Length;
        float fUnitFactor_Volume = 1000000000;//fUnitFactor_Length * fUnitFactor_Length * fUnitFactor_Length;
        int iNumberOfDecimalPlaces_Length = 1;
        int iNumberOfDecimalPlaces_Area = 1;
        int iNumberOfDecimalPlaces_Volume = 1;
        int iNumberOfDecimalPlaces_Weight = 1;
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
                if (MComponentTypeIndex != -1)
                {
                    NotifyPropertyChanged("ComponentTypeIndex");
                    ComponentTypeChanged();
                    ComponentSerieIndex = 0;
                }
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
                if (MComponentSerieIndex != -1)
                {
                    NotifyPropertyChanged("ComponentSerieIndex");
                    ComponentSeriesChanged();
                    ComponentIndex = 0;
                }
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
                if(MComponentIndex != -1) NotifyPropertyChanged("ComponentIndex");
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

        public List<CComponentParamsView> ComponentGeometry
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

        public List<CComponentParamsView> ComponentDetails
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

        public bool DrawPoints2D
        {
            get
            {
                return bDrawPoints;
            }

            set
            {
                bDrawPoints = value;
                NotifyPropertyChanged("DrawPoints2D");
            }
        }

        public bool DrawOutLine2D
        {
            get
            {
                return bDrawOutLine;
            }

            set
            {
                bDrawOutLine = value;
                NotifyPropertyChanged("DrawOutLine2D");
            }
        }

        public bool DrawPointNumbers2D
        {
            get
            {
                return bDrawPointNumbers;
            }

            set
            {
                bDrawPointNumbers = value;
                NotifyPropertyChanged("DrawPointNumbers2D");
            }
        }

        public bool DrawHoles2D
        {
            get
            {
                return bDrawHoles;
            }

            set
            {
                bDrawHoles = value;
                NotifyPropertyChanged("DrawHoles2D");
            }
        }

        public bool DrawHoleCentreSymbol2D
        {
            get
            {
                return bDrawHoleCentreSymbol;
            }

            set
            {
                bDrawHoleCentreSymbol = value;
                NotifyPropertyChanged("DrawHoleCentreSymbol2D");
            }
        }

        public bool DrawDrillingRoute2D
        {
            get
            {
                return bDrawDrillingRoute;
            }

            set
            {
                bDrawDrillingRoute = value;
                NotifyPropertyChanged("DrawDrillingRoute2D");
            }
        }

        public bool MirrorY
        {
            get
            {
                return bMirrorY;
            }

            set
            {
                bMirrorY = value;
                NotifyPropertyChanged("MirrorY");
            }
        }

        public bool MirrorX
        {
            get
            {
                return bMirrorX;
            }

            set
            {
                bMirrorX = value;
                NotifyPropertyChanged("MirrorX");
            }
        }

        public bool Rotate_90_CW
        {
            get
            {
                return bRotate_90_CW;
            }

            set
            {
                bRotate_90_CW = value;
                NotifyPropertyChanged("Rotate90CW");
            }
        }

        public bool Rotate_90_CCW
        {
            get
            {
                return bRotate_90_CCW;
            }

            set
            {
                bRotate_90_CCW = value;
                NotifyPropertyChanged("Rotate90CCW");
            }
        }

        CDatabaseComponents databaseComponents;
        private Dictionary<string, List<CCrSc_TW>> crossSections;
        private Dictionary<string, List<CPlate>> plates;
        private Dictionary<string, List<CScrew>> screws;

        //public Dictionary<string, List<CCrSc_TW>> CrossSections
        //{
        //    get
        //    {
        //        return crossSections;
        //    }

        //    set
        //    {
        //        crossSections = value;
        //        NotifyPropertyChanged("CrossSections");
        //    }
        //}
        //public Dictionary<string, List<CPlate>> Plates
        //{
        //    get
        //    {
        //        return plates;
        //    }

        //    set
        //    {
        //        plates = value;
        //        NotifyPropertyChanged("Plates");
        //    }
        //}
        //List<CPlate> selectedSerie;
        //public List<CPlate> SelectedSerie
        //{
        //    get
        //    {
        //        return selectedSerie;
        //    }

        //    set
        //    {
        //        selectedSerie = value;
        //        SelectedComponent = selectedSerie[0];
        //        NotifyPropertyChanged("SelectedSerie");
        //    }
        //}
        

        //public Dictionary<string, List<CScrew>> Screws
        //{
        //    get
        //    {
        //        return screws;
        //    }

        //    set
        //    {
        //        screws = value;
        //        NotifyPropertyChanged("Screws");
        //    }
        //}

        //private object selectedComponent;
        //public object SelectedComponent
        //{
        //    get
        //    {
        //        return selectedComponent;
        //    }

        //    set
        //    {
        //        selectedComponent = value;
        //        if (selectedComponent == null)
        //        {
                    
        //        }
        //        NotifyPropertyChanged("SelectedComponent");
        //    }
        //}


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

            DrawPoints2D = true;
            DrawOutLine2D= true;
            DrawPointNumbers2D = true;
            DrawHoles2D = true;
            DrawHoleCentreSymbol2D = true;
            DrawDrillingRoute2D = true;

            MirrorY = false;
            MirrorX = false;
            Rotate_90_CW = false;
            Rotate_90_CCW = false;

            //TODO naplnit zoznamy cross sections, plates, screws
            //CreateCrossSections();
            //CreatePlates();
            //CreateScrews();

            //SelectedComponent = Plates["Serie B"][0];

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
                ComponentSeries = databaseComponents.arr_Serie_CrSc_FormSteel_Names; // Cross-sections
                Components = databaseComponents.arr_Serie_Box_FormSteel_Names;
            }
            else if (ComponentTypeIndex == 1)
            {
                ComponentSeries = databaseComponents.arr_SeriesNames; // Plates
                Components = databaseComponents.arr_Serie_B_Names;
            }
            else // Screws
            {
                ComponentSeries = databaseComponents.arr_Serie_Screws_Names; // Screws
                Components = databaseComponents.arr_Serie_TEK_Names;
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
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Z:
                        {
                            Components = databaseComponents.arr_Serie_Z_FormSteel_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_single:
                        {
                            Components = databaseComponents.arr_Serie_C_FormSteel_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_back_to_back:
                        {
                            Components = databaseComponents.arr_Serie_C_BtoB_FormSteel_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_C_nested:
                        {
                            Components = databaseComponents.arr_Serie_C_Nested_FormSteel_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FormSteel.eSerie_Box_63020:
                        {
                            Components = databaseComponents.arr_Serie_Box63020_FormSteel_Names;
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
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            Components = databaseComponents.arr_Serie_L_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            Components = databaseComponents.arr_Serie_LL_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            Components = databaseComponents.arr_Serie_F_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                        {
                            Components = databaseComponents.arr_Serie_Q_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_S:
                        {
                            Components = databaseComponents.arr_Serie_S_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_T:
                        {
                            Components = databaseComponents.arr_Serie_T_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_X:
                        {
                            Components = databaseComponents.arr_Serie_X_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            Components = databaseComponents.arr_Serie_Y_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            Components = databaseComponents.arr_Serie_J_Names;
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            Components = databaseComponents.arr_Serie_K_Names;
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
                Components = databaseComponents.arr_Serie_TEK_Names;
            }
        }

        public void SetComponentProperties(CPlate plate)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<CComponentParamsView> geometry = new List<CComponentParamsView>();

            geometry.Add(new CComponentParamsView(CParamsResources.PlateNameS.Name, "", plate.Name, ""));
            geometry.Add(new CComponentParamsView(CParamsResources.PlateThicknessS.Name, CParamsResources.PlateThicknessS.Symbol, (Math.Round(plate.Ft * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateThicknessS.Unit));

            if (plate is CConCom_Plate_JA)
            {
                CConCom_Plate_JA plateTemp = (CConCom_Plate_JA)plate;

                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidthS.Name,   CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fb_X * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
            }
            else if (plate is CConCom_Plate_JB)
            {
                CConCom_Plate_JB plateTemp = (CConCom_Plate_JB)plate;

                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidthS.Name, CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fb_X * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KA)
            {
                CConCom_Plate_KA plateTemp = (CConCom_Plate_KA)plate;

                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
            }
            else if (plate is CConCom_Plate_KB)
            {
                CConCom_Plate_KB plateTemp = (CConCom_Plate_KB)plate;

                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KC)
            {
                CConCom_Plate_KC plateTemp = (CConCom_Plate_KC)plate;

                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KD)
            {
                CConCom_Plate_KD plateTemp = (CConCom_Plate_KD)plate;

                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KE)
            {
                CConCom_Plate_KE plateTemp = (CConCom_Plate_KE)plate;

                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
                geometry.Add(new CComponentParamsView(CParamsResources.RafterWidthS.Name, CParamsResources.RafterWidthS.Symbol, (Math.Round(plateTemp.Fb_XR * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.RafterWidthS.Unit));
            }
            else
            {
                // Plate is not implemented
            }

            if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangementCircleApexOrKnee)
            {
                CScrewArrangementCircleApexOrKnee arrangementTemp = (CScrewArrangementCircleApexOrKnee)plate.ScrewArrangement;

                geometry.Add(new CComponentParamsView("Number of screws in circle", "n", arrangementTemp.IHolesInCirclesNumber.ToString(nfi), "[-]"));
                geometry.Add(new CComponentParamsView("Screw gauge", "No", arrangementTemp.referenceScrew.Gauge.ToString(), "[-]"));  // TODO prerobit na vyber objektu skrutky z databazy
                geometry.Add(new CComponentParamsView("Radius", "r", (Math.Round(arrangementTemp.FRadius * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                geometry.Add(new CComponentParamsView("Cross-section depth", "h", (Math.Round(arrangementTemp.FCrscRafterDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                geometry.Add(new CComponentParamsView("Cross-section web straight depth", "dw", (Math.Round(arrangementTemp.FCrscWebStraightDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                geometry.Add(new CComponentParamsView("Middle stiffener size", "dw.m", (Math.Round(arrangementTemp.FStiffenerSize * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                geometry.Add(new CComponentParamsView("Use additional corner screws", " ", arrangementTemp.BUseAdditionalCornerScrews.ToString(), "[-]"));
                geometry.Add(new CComponentParamsView("Number of additional screws", "No", arrangementTemp.IAdditionalConnectorNumber.ToString(nfi), "[-]"));
            }
            else
            {
                // Screw arrangement is not implemented
            }

            ComponentGeometry = geometry;

            List<CComponentParamsView> details = new List<CComponentParamsView>();
            details.Add(new CComponentParamsView("Perimeter - Cutting route distance", "Lcr", (Math.Round(plate.fCuttingRouteDistance * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsView("Surface", "S", (Math.Round(plate.fSurface * fUnitFactor_Area, iNumberOfDecimalPlaces_Area)).ToString(nfi), "[mm^2]"));
            details.Add(new CComponentParamsView("Area", "A", (Math.Round(plate.fArea * fUnitFactor_Area, iNumberOfDecimalPlaces_Area)).ToString(nfi), "[mm^2]"));
            details.Add(new CComponentParamsView("Volume", "V", (Math.Round(plate.fVolume * fUnitFactor_Volume, iNumberOfDecimalPlaces_Volume)).ToString(nfi), "[mm^3]"));
            details.Add(new CComponentParamsView("Weight", "w", Math.Round(plate.fWeight, iNumberOfDecimalPlaces_Weight).ToString(nfi), "[kg]"));
            CCNCPathFinder c = new CCNCPathFinder();
            c.RoutePoints = plate.DrillingRoutePoints;
            double dist = c.GetRouteDistance();
            details.Add(new CComponentParamsView("Drilling route distance", "Ldr", (Math.Round(dist * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

            ComponentDetails = details;
        }
        public void SetComponentProperties(CCrSc crsc)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<CComponentParamsView> geometry = new List<CComponentParamsView>();
            geometry.Add(new CComponentParamsView("Name", "", crsc.Name, ""));
            geometry.Add(new CComponentParamsView("Width", "b", (Math.Round(crsc.b * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(new CComponentParamsView("Height", "h", (Math.Round(crsc.h * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(new CComponentParamsView("Thickness", "t", (Math.Round(crsc.t_min * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

            ComponentGeometry = geometry;

            List<CComponentParamsView> details = new List<CComponentParamsView>();
            List<CSectionPropertiesText> sectionTexts = CSectionManager.LoadSectionPropertiesNamesSymbolsUnits();
            List<string> listSectionPropertyValue = new List<string>();

            try
            {
                listSectionPropertyValue = CSectionManager.LoadSectionPropertiesStringList(crsc.NameDatabase);

                foreach (CSectionPropertiesText textRow in sectionTexts)
                {
                    if (listSectionPropertyValue[textRow.ID - 1] != "") // Add only row for property value which is not empty string
                        details.Add(new CComponentParamsView(textRow.text, textRow.symbol, listSectionPropertyValue[textRow.ID - 1], textRow.unit_NmmMpa));
                }

                ComponentDetails = details;
            }
            catch
            {
                throw new ArgumentException("Cross section name wasn't found in the database or invalid database data.");
            }
        }
        public void SetComponentProperties(CScrew screw)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<CComponentParamsView> details = new List<CComponentParamsView>();
            details.Add(new CComponentParamsView("Gauge", "No", screw.Gauge.ToString(nfi), "[-]"));
            details.Add(new CComponentParamsView("Thread diameter", "dt", (Math.Round(screw.Diameter_thread * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsView("Shank diameter", "ds", (Math.Round(screw.Diameter_shank * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsView("Head diameter", "dh", (Math.Round(screw.D_h_headdiameter * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsView("Washer diameter", "dw", (Math.Round(screw.D_w_washerdiameter * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsView("Washer thickness", "tw", (Math.Round(screw.T_w_washerthickness * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsView("Weight", "w", Math.Round(screw.Weight, iNumberOfDecimalPlaces_Weight).ToString(nfi), "[kg]"));

            ComponentDetails = details;
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
            //float fPitch_rad = 11f / 180f * (float)Math.PI; // Roof Pitch - default value (11 deg)
            int iNumberofHoles;
            bool bUseAdditinalConnectors = true;
            int iNumberOfAdditionalConnectorsInPlate = 32; // 2*4*4
            float fRadius = 0.25f; // Radius of screws in circle
            float fRafterDepth = 0.63f;
            float fRafterStraightDepth = 0.63f - 2 * 0.025f - 2 * 0.002f;
            float fMiddleStiffenerSize = 0.18f;
                        CPoint controlpoint = new CPoint(0, 0, 0, 0, 0);
            CScrew referenceScrew = new CScrew("TEK", "14");
            CAnchor referenceAnchor = new CAnchor(0.02f, 0.18f, 0.5f, true);

            CScrewArrangement screwArrangement;

            CScrewArrangementCircleApexOrKnee screwArrangementCircle;

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
                platesInSerie1.Add(new CConCom_Plate_BB_BG(databaseComponents.arr_Serie_B_Names[i], controlpoint, fb, fh, fl, ft, iNumberofHoles, referenceScrew, referenceAnchor, 0, 0, 0, true)); // B
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

                screwArrangement = new CScrewArrangement(iNumberofHoles, referenceScrew);
                platesInSerie2.Add(new CConCom_Plate_F_or_L(databaseComponents.arr_Serie_L_Names[i], controlpoint, fb, fh, fl, ft, 0, 0, 0, screwArrangement, true)); // L
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

                screwArrangement = new CScrewArrangement(iNumberofHoles, referenceScrew);
                platesInSerie3.Add(new CConCom_Plate_LL(databaseComponents.arr_Serie_LL_Names[i], controlpoint, fb, fb2, fh, fl, ft, 0, 0, 0, screwArrangement, true)); // LL
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

            screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(iNumberofHoles, referenceScrew, fRadius, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate);
            platesInSerie8.Add(new CConCom_Plate_JA(databaseComponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, screwArrangementCircle, true));

            fb = databaseComponents.arr_Serie_J_Dimension[1, 0] / 1000f;
            fh = databaseComponents.arr_Serie_J_Dimension[1, 1] / 1000f;
            fh2 = databaseComponents.arr_Serie_J_Dimension[1, 2] / 1000f;
            fl = databaseComponents.arr_Serie_J_Dimension[1, 3] / 1000f;
            ft = databaseComponents.arr_Serie_J_Dimension[1, 4] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_J_Dimension[1, 5];

            screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(iNumberofHoles, referenceScrew, fRadius, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate);
            platesInSerie8.Add(new CConCom_Plate_JB(databaseComponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

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

            screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(iNumberofHoles, referenceScrew, fRadius, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate);
            platesInSerie9.Add(new CConCom_Plate_KA(databaseComponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, screwArrangementCircle, true));

            fb_R = databaseComponents.arr_Serie_K_Dimension[1, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[1, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[1, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[1, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[1, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[1, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[1, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[1, 7];

            screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(iNumberofHoles, referenceScrew, fRadius, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate);
            platesInSerie9.Add(new CConCom_Plate_KB(databaseComponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

            fb_R = databaseComponents.arr_Serie_K_Dimension[2, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[2, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[2, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[2, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[2, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[2, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[2, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[2, 7];

            screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(iNumberofHoles, referenceScrew, fRadius, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate);
            platesInSerie9.Add(new CConCom_Plate_KC(databaseComponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

            fb_R = databaseComponents.arr_Serie_K_Dimension[3, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[3, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[3, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[3, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[3, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[3, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[3, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[3, 7];

            screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(iNumberofHoles, referenceScrew, fRadius, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate);
            platesInSerie9.Add(new CConCom_Plate_KD(databaseComponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

            fb_R = databaseComponents.arr_Serie_K_Dimension[4, 0] / 1000f;
            fb = databaseComponents.arr_Serie_K_Dimension[4, 1] / 1000f;
            fh = databaseComponents.arr_Serie_K_Dimension[4, 2] / 1000f;
            fb2 = databaseComponents.arr_Serie_K_Dimension[4, 3] / 1000f;
            fh2 = databaseComponents.arr_Serie_K_Dimension[4, 4] / 1000f;
            fl = databaseComponents.arr_Serie_K_Dimension[4, 5] / 1000f;
            ft = databaseComponents.arr_Serie_K_Dimension[4, 6] / 1000f;
            iNumberofHoles = (int)databaseComponents.arr_Serie_K_Dimension[4, 7];

            screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(iNumberofHoles, referenceScrew, fRadius, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, bUseAdditinalConnectors, iNumberOfAdditionalConnectorsInPlate);
            platesInSerie9.Add(new CConCom_Plate_KE(databaseComponents.arr_Serie_K_Names[4], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

            plates.Add("Serie K", platesInSerie9);
        }

        private void CreateScrews()
        {
            // TODO - prepracovat a nacitavat vlastnosti z databazy, plati pre vsetky komponenty - prierezy, plechy, skrutky
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

                screwsInSerie.Add(new CScrew(databaseComponents.arr_Serie_TEK_Names[i], controlpoint, iGauge, fHoleDiameter,0,0,0, fConnectorLength, fConnectorWeight,0,0,0, true));
            }
            screws.Add("Hex Head Tek", screwsInSerie);
        }
    }
}
