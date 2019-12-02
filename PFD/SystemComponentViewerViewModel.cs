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
using MATH;
using BaseClasses;
using BaseClasses.GraphObj;
using CRSC;
using DATABASE;
using DATABASE.DTO;
using System.Windows;
using System.Windows.Controls;
using EXPIMP;

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
        private int MScrewArrangementIndex;

        private List<string> MComponentTypes;
        private List<string> MComponentSeries;
        private string[] MComponents;
        private List<string> MScrewArrangements;

        private List<CComponentParamsView> MScrewArrangementParameters;
        private List<CComponentParamsView> MComponentGeometry;
        private List<CComponentParamsView> MComponentDetails;

        bool bDrawPoints;
        bool bDrawOutLine;
        bool bDrawPointNumbers;
        bool bDrawHoles;
        bool bDrawHoleCentreSymbol;
        bool bDrawDrillingRoute;
        bool bDrawDimensions;
        bool bDrawMemberOutline;
        bool bDrawBendLines;

        bool bMirrorY;
        bool bMirrorX;
        bool bRotate_90_CW;
        bool bRotate_90_CCW;
        bool bDrawScrews3D;

        public bool IsSetFromCode = false;
        
        float fUnitFactor_Length = 1000;
        float fUnitFactor_Area = 1000000;//fUnitFactor_Length * fUnitFactor_Length;
        float fUnitFactor_Volume = 1000000000;//fUnitFactor_Length * fUnitFactor_Length * fUnitFactor_Length;
        float fUnitFactor_Rotation = 180f / MathF.fPI; // Factor from radians to degrees

        int iNumberOfDecimalPlaces_Length = 1;
        int iNumberOfDecimalPlaces_Area = 1;
        int iNumberOfDecimalPlaces_Volume = 1;
        int iNumberOfDecimalPlaces_Mass = 1;
        int iNumberOfDecimalPlaces_Rotation = 1;

        List<Point> MDrillingRoutePoints;

        private string MJobNumber;
        private string MCustomer;
        private int MAmount;
        private int MAmountRH;
        private int MAmountLH;

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

        //-------------------------------------------------------------------------------------------------------------
        public int ScrewArrangementIndex
        {
            get
            {
                return MScrewArrangementIndex;
            }

            set
            {
                MScrewArrangementIndex = value;
                if (MScrewArrangementIndex != -1) NotifyPropertyChanged("ScrewArrangementIndex");
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

        public List<string> ComponentSeries
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

        public List<string> ScrewArrangements
        {
            get
            {
                return MScrewArrangements;
            }

            set
            {
                MScrewArrangements = value;
                NotifyPropertyChanged("ScrewArrangements");
            }
        }

        public List<CComponentParamsView> ScrewArrangementParameters
        {
            get
            {
                return MScrewArrangementParameters;
            }

            set
            {
                MScrewArrangementParameters = value;

                foreach (CComponentParamsView cpw in MScrewArrangementParameters)
                {
                    cpw.PropertyChanged += HandleComponentParamsViewPropertyChangedEvent;
                }

                NotifyPropertyChanged("ScrewArrangementParameters");
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

        public bool DrawDimensions2D
        {
            get
            {
                return bDrawDimensions;
            }

            set
            {
                bDrawDimensions = value;
                NotifyPropertyChanged("DrawDimensions2D");
            }
        }

        public bool DrawMemberOutline2D
        {
            get
            {
                return bDrawMemberOutline;
            }

            set
            {
                bDrawMemberOutline = value;
                NotifyPropertyChanged("DrawMemberOutline2D");
            }
        }

        public bool DrawBendLines2D
        {
            get
            {
                return bDrawBendLines;
            }

            set
            {
                bDrawBendLines = value;
                NotifyPropertyChanged("DrawBendLines2D");
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

        public bool Rotate90CW
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

        public bool Rotate90CCW
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

        public bool DrawScrews3D
        {
            get
            {
                return bDrawScrews3D;
            }

            set
            {
                bDrawScrews3D = value;
                NotifyPropertyChanged("DrawScrews3D");
            }
        }

        public List<Point> DrillingRoutePoints
        {
            get
            {
                return MDrillingRoutePoints;
            }

            set
            {
                MDrillingRoutePoints = value;
            }
        }

        public string JobNumber
        {
            get
            {
                return MJobNumber;
            }

            set
            {
                MJobNumber = value;
            }
        }

        public string Customer
        {
            get
            {
                return MCustomer;
            }

            set
            {
                MCustomer = value;
            }
        }

        public int Amount
        {
            get
            {
                return MAmount;
            }

            set
            {
                MAmount = value;
                MAmountRH = (int)(Math.Ceiling(MAmount / 2.0));
                MAmountLH = MAmount - MAmountRH;

                NotifyPropertyChanged("Amount");
                NotifyPropertyChanged("AmountRH");
                NotifyPropertyChanged("AmountLH");
            }
        }

        public int AmountRH
        {
            get
            {
                return MAmountRH;
            }

            set
            {
                if (value <= Amount)
                {
                    MAmountRH = value;
                    MAmountLH = Amount - AmountRH;
                }
                NotifyPropertyChanged("AmountRH");
                NotifyPropertyChanged("AmountLH");
            }
        }

        public int AmountLH
        {
            get
            {
                return MAmountLH;
            }

            set
            {
                if (value <= Amount)
                {
                    MAmountLH = value;
                    MAmountRH = Amount - AmountLH;
                }                    
                NotifyPropertyChanged("AmountRH");
                NotifyPropertyChanged("AmountLH");
            }
        }

        CDatabaseComponents databaseComponents;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public SystemComponentViewerViewModel(CDatabaseComponents dcomponents)
        {
            databaseComponents = dcomponents;
            MComponentTypes = new List<string>() { "Cross-section", "Plate", "Screw" };
            MComponentSeries = dcomponents.arr_SeriesNames;
            MComponents = dcomponents.arr_Serie_J_Names;
            MScrewArrangements = dcomponents.arr_Serie_J_ScrewArrangement_Names;

            // Set default
            ComponentTypeIndex = 1;
            ComponentSerieIndex = 9;
            ComponentIndex = 1;
            ScrewArrangementIndex = 2;

            DrawPoints2D = true;
            DrawOutLine2D= true;
            DrawPointNumbers2D = true;
            DrawHoles2D = true;
            DrawHoleCentreSymbol2D = true;
            DrawDrillingRoute2D = true;
            DrawDimensions2D = true;
            bDrawMemberOutline = true;
            bDrawBendLines = true;

            MirrorY = false;
            MirrorX = false;
            Rotate90CW = false;
            Rotate90CCW = false;

            JobNumber = "B0000";
            Customer = "write name";
            Amount = 1;

            DrawScrews3D = true;

            IsSetFromCode = false;
        }

        //treba skusit ulozit Canvas do MemoryStream a nasledne v background workeri vybrat z Memory canvas

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleComponentParamsViewPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.PropertyChanged(sender, e);
        }

        private void ComponentTypeChanged()
        {
            if (ComponentTypeIndex == 0) // Cross-sections
            {
                ComponentSeries = databaseComponents.arr_Serie_CrSc_FS_Names; // Cross-sections
                Components = databaseComponents.arr_Serie_Box_FS_Names;
            }
            else if (ComponentTypeIndex == 1)
            {
                ComponentSeries = databaseComponents.arr_SeriesNames; // Plates
                Components = databaseComponents.arr_Serie_B_Names;
                ScrewArrangements = databaseComponents.arr_Serie_B_ScrewArrangement_Names;
                // Set default index // TODO - Ondrej, pri zmene typu komponenty alebo serie tu nastavujem index pre screw arrangement, ale neviem ci je to spravne miesto, ak nie tak to presun inam 
                ScrewArrangementIndex = 0;
            }
            else // Screws
            {
                ComponentSeries = databaseComponents.arr_Serie_Screws_Names; // Screws

                Dictionary<string, CTEKScrewProperties> dict = CTEKScrewsManager.DictTEKScrewProperties;
                string [] arr_Serie_TEK_Names = new string[dict.Count];

                arr_Serie_TEK_Names =  dict.Keys.ToArray();

                Components = arr_Serie_TEK_Names;
            }
        }

        private void ComponentSeriesChanged()
        {
            if (ComponentTypeIndex == 0) // Cross-sections
            {
                switch ((ESerieTypeCrSc_FS)ComponentSerieIndex)
                {
                    case ESerieTypeCrSc_FS.eSerie_Box_10075:
                        {
                            Components = databaseComponents.arr_Serie_Box_FS_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_Z:
                        {
                            Components = databaseComponents.arr_Serie_Z_FS_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_single:
                        {
                            Components = databaseComponents.arr_Serie_C_FS_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_back_to_back:
                        {
                            Components = databaseComponents.arr_Serie_C_BtoB_FS_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_C_nested:
                        {
                            Components = databaseComponents.arr_Serie_C_Nested_FS_Names;
                            break;
                        }
                    case ESerieTypeCrSc_FS.eSerie_Box_63020:
                        {
                            Components = databaseComponents.arr_Serie_Box63020_FS_Names;
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
                            ScrewArrangements = databaseComponents.arr_Serie_B_ScrewArrangement_Names;
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_L:
                        {
                            Components = databaseComponents.arr_Serie_L_Names;
                            ScrewArrangements = databaseComponents.arr_Serie_L_ScrewArrangement_Names;
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_LL:
                        {
                            Components = databaseComponents.arr_Serie_LL_Names;
                            ScrewArrangements = databaseComponents.arr_Serie_LL_ScrewArrangement_Names;
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_F:
                        {
                            Components = databaseComponents.arr_Serie_F_Names;
                            ScrewArrangements = databaseComponents.arr_Serie_F_ScrewArrangement_Names;
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_Q:
                        {
                            Components = databaseComponents.arr_Serie_Q_Names;
                            ScrewArrangements = new List<string>(1) { "Undefined" };
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_S:
                        {
                            Components = databaseComponents.arr_Serie_S_Names;
                            ScrewArrangements = new List<string>(1) { "Undefined" };
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_T:
                        {
                            Components = databaseComponents.arr_Serie_T_Names;
                            ScrewArrangements = new List<string>(1) { "Undefined" };
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_X:
                        {
                            Components = databaseComponents.arr_Serie_X_Names;
                            ScrewArrangements = new List<string>(1) { "Undefined" };
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_Y:
                        {
                            Components = databaseComponents.arr_Serie_Y_Names;
                            ScrewArrangements = new List<string>(1) { "Undefined" };
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_J:
                        {
                            Components = databaseComponents.arr_Serie_J_Names;
                            ScrewArrangements = databaseComponents.arr_Serie_J_ScrewArrangement_Names;
                            ScrewArrangementIndex = 2;
                            break;
                        }
                    case ESerieTypePlate.eSerie_K:
                        {
                            Components = databaseComponents.arr_Serie_K_Names;
                            ScrewArrangements = databaseComponents.arr_Serie_K_ScrewArrangement_Names;
                            ScrewArrangementIndex = 2;
                            break;
                        }
                    case ESerieTypePlate.eSerie_M:
                        {
                            Components = databaseComponents.arr_Serie_M_Names;
                            ScrewArrangements = new List<string>(1) { "Undefined" };
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_N:
                        {
                            Components = databaseComponents.arr_Serie_N_Names;
                            ScrewArrangements = new List<string>(1) { "Undefined" };
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    case ESerieTypePlate.eSerie_O:
                        {
                            Components = databaseComponents.arr_Serie_O_Names;
                            ScrewArrangements = databaseComponents.arr_Serie_O_ScrewArrangement_Names;
                            ScrewArrangementIndex = 0;
                            break;
                        }
                    default:
                        {
                            ScrewArrangementIndex = 0;
                            // Not implemented
                            break;
                        }
                }
            }
            else // Screws
            {
                Dictionary<string, CTEKScrewProperties> dict = CTEKScrewsManager.DictTEKScrewProperties;
                string[] arr_Serie_TEK_Names = new string[dict.Count];

                arr_Serie_TEK_Names = dict.Keys.ToArray();

                Components = arr_Serie_TEK_Names;
            }
        }

        public void SetScrewArrangementProperties(CScrewArrangement screwArrangement) 
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<CComponentParamsView> screwArrangmenetProperties = new List<CComponentParamsView>();

            if (screwArrangement != null && screwArrangement is CScrewArrangementCircleApexOrKnee)
            {
                CScrewArrangementCircleApexOrKnee circArrangement = (CScrewArrangementCircleApexOrKnee)screwArrangement;

                List<string> listScrewGauges = CTEKScrewsManager.LoadTEKScrewsProperties().Select(i => i.gauge).ToList();

                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.ScrewGaugeS.Name, CParamsResources.ScrewGaugeS.Symbol, circArrangement.referenceScrew.Gauge.ToString(), listScrewGauges, CParamsResources.ScrewGaugeS.Unit));  // TODO prerobit na vyber objektu skrutky z databazy
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscDepthS.Name, CParamsResources.CrscDepthS.Symbol, (Math.Round(circArrangement.FCrscRafterDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscDepthS.Unit));
                //screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebStraightDepthS.Name, CParamsResources.CrscWebStraightDepthS.Symbol, (Math.Round(circArrangement.FCrscWebStraightDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebStraightDepthS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebMiddleStiffenerSizeS.Name, CParamsResources.CrscWebMiddleStiffenerSizeS.Symbol, (Math.Round(circArrangement.FStiffenerSize * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebMiddleStiffenerSizeS.Unit));

                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.NumberOfCirclesInGroupS.Name, CParamsResources.NumberOfCirclesInGroupS.Symbol, circArrangement.INumberOfCirclesInGroup.ToString(), CParamsResources.NumberOfCirclesInGroupS.Unit));
                CScrewSequenceGroup gr = circArrangement.ListOfSequenceGroups.FirstOrDefault();
                if (gr != null)
                {
                    int c = 0;
                    for (int i = 0; i < gr.ListSequence.Count; i++)
                    {
                        if (gr.ListSequence[i] is CScrewHalfCircleSequence && i % 2 == 1)
                        {
                            c++;
                            CScrewHalfCircleSequence screwHalfCircleSequence = gr.ListSequence[i] as CScrewHalfCircleSequence;
                            screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.NumberOfScrewsInCircleSequenceS.Name + " " + (c), CParamsResources.NumberOfScrewsInCircleSequenceS.Symbol, screwHalfCircleSequence.INumberOfConnectors.ToString(), CParamsResources.NumberOfScrewsInCircleSequenceS.Unit));
                            screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.RadiusOfScrewsInCircleSequenceS.Name + " " + (c), CParamsResources.RadiusOfScrewsInCircleSequenceS.Symbol, (Math.Round(screwHalfCircleSequence.Radius * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.RadiusOfScrewsInCircleSequenceS.Unit));
                        }
                    }
                }
                
                screwArrangmenetProperties.Add(new CComponentParamsViewBool(CParamsResources.UseAdditionalCornerScrewsS.Name, CParamsResources.UseAdditionalCornerScrewsS.Symbol, circArrangement.BUseAdditionalCornerScrews, CParamsResources.UseAdditionalCornerScrewsS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.PositionOfCornerSequence_xS.Name, CParamsResources.PositionOfCornerSequence_xS.Symbol, (Math.Round(circArrangement.FPositionOfCornerSequence_x * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PositionOfCornerSequence_xS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.PositionOfCornerSequence_yS.Name, CParamsResources.PositionOfCornerSequence_yS.Symbol, (Math.Round(circArrangement.FPositionOfCornerSequence_y * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PositionOfCornerSequence_yS.Unit));
                //screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.NumberOfAdditionalScrewsInCornerS.Name, CParamsResources.NumberOfAdditionalScrewsInCornerS.Symbol, circArrangement.IAdditionalConnectorInCornerNumber.ToString(nfi), CParamsResources.NumberOfAdditionalScrewsInCornerS.Unit));
                List<string> listAdditionalScrewsInCorner = new List<string>() {"1", "4", "9","16","25","36","49","64","81","100" };
                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.NumberOfAdditionalScrewsInCornerS.Name, CParamsResources.NumberOfAdditionalScrewsInCornerS.Symbol, circArrangement.IAdditionalConnectorInCornerNumber.ToString(nfi), listAdditionalScrewsInCorner, CParamsResources.NumberOfAdditionalScrewsInCornerS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.DistanceOfAdditionalScrewsInxS.Name, CParamsResources.DistanceOfAdditionalScrewsInxS.Symbol, (Math.Round(circArrangement.FAdditionalScrewsDistance_x * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.DistanceOfAdditionalScrewsInxS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.DistanceOfAdditionalScrewsInyS.Name, CParamsResources.DistanceOfAdditionalScrewsInyS.Symbol, (Math.Round(circArrangement.FAdditionalScrewsDistance_y * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.DistanceOfAdditionalScrewsInyS.Unit));
            }
            else if (screwArrangement != null && screwArrangement is CScrewArrangementRectApexOrKnee)
            {
                CScrewArrangementRectApexOrKnee rectArrangement = (CScrewArrangementRectApexOrKnee)screwArrangement;

                List<string> listScrewGauges = CTEKScrewsManager.LoadTEKScrewsProperties().Select(i => i.gauge).ToList();

                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.ScrewGaugeS.Name, CParamsResources.ScrewGaugeS.Symbol, rectArrangement.referenceScrew.Gauge.ToString(), listScrewGauges, CParamsResources.ScrewGaugeS.Unit));  // TODO prerobit na vyber objektu skrutky z databazy
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscDepthS.Name, CParamsResources.CrscDepthS.Symbol, (Math.Round(rectArrangement.FCrscRafterDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscDepthS.Unit));
                //screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebStraightDepthS.Name, CParamsResources.CrscWebStraightDepthS.Symbol, (Math.Round(rectArrangement.FCrscWebStraightDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebStraightDepthS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebMiddleStiffenerSizeS.Name, CParamsResources.CrscWebMiddleStiffenerSizeS.Symbol, (Math.Round(rectArrangement.FStiffenerSize * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebMiddleStiffenerSizeS.Unit));

                // TODO Ondrej, toto by malo byt obecne pre rozny pocet rectangular sequences, pripadne groups

                // TODO - Ondrej, TODO No. 105
                // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
                // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
                // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ1", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ1", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ1", "xc1", (Math.Round(rectArrangement.fx_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ1", "yc1", (Math.Round(rectArrangement.fy_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ1", "x1", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ1", "y1", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ2", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ2", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ2", "xc2", (Math.Round(rectArrangement.fx_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ2", "yc2", (Math.Round(rectArrangement.fy_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ2", "bx2", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ2", "by2", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                
                if (rectArrangement.iNumberOfScrewsInRow_xDirection_SQ3 != 0 && rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ3 != 0)
                {
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ3", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ3.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ3", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ3.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ3", "xc3", (Math.Round(rectArrangement.fx_c_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ3", "yc3", (Math.Round(rectArrangement.fy_c_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ3", "x3", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ3", "y3", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                }
                if (rectArrangement.iNumberOfScrewsInRow_xDirection_SQ4 != 0 && rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ4 != 0)
                {
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ4", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ4.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ4", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ4.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ4", "xc4", (Math.Round(rectArrangement.fx_c_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ4", "yc4", (Math.Round(rectArrangement.fy_c_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ4", "x4", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ4", "y4", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                }
            }
            else if (screwArrangement != null && screwArrangement is CScrewArrangement_BX_1)
            {
                CScrewArrangement_BX_1 rectArrangement = (CScrewArrangement_BX_1)screwArrangement;

                List<string> listScrewGauges = CTEKScrewsManager.LoadTEKScrewsProperties().Select(i => i.gauge).ToList();

                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.ScrewGaugeS.Name, CParamsResources.ScrewGaugeS.Symbol, rectArrangement.referenceScrew.Gauge.ToString(), listScrewGauges, CParamsResources.ScrewGaugeS.Unit));  // TODO prerobit na vyber objektu skrutky z databazy
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscDepthS.Name, CParamsResources.CrscDepthS.Symbol, (Math.Round(rectArrangement.FCrscColumnDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscDepthS.Unit));
                //screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebStraightDepthS.Name, CParamsResources.CrscWebStraightDepthS.Symbol, (Math.Round(rectArrangement.FCrscWebStraightDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebStraightDepthS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebMiddleStiffenerSizeS.Name, CParamsResources.CrscWebMiddleStiffenerSizeS.Symbol, (Math.Round(rectArrangement.FStiffenerSize * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebMiddleStiffenerSizeS.Unit));

                // TODO Ondrej, toto by malo byt obecne pre rozny pocet rectangular sequences, pripadne groups

                // TODO - Ondrej, TODO No. 105
                // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
                // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
                // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ1", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ1", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ1", "xc1", (Math.Round(rectArrangement.fx_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ1", "yc1", (Math.Round(rectArrangement.fy_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ1", "x1", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ1", "y1", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ2", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ2", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ2", "xc2", (Math.Round(rectArrangement.fx_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ2", "yc2", (Math.Round(rectArrangement.fy_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ2", "bx2", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ2", "by2", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                if (rectArrangement.iNumberOfScrewsInRow_xDirection_SQ3 != 0 && rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ3 != 0)
                {
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ3", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ3.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ3", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ3.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ3", "xc3", (Math.Round(rectArrangement.fx_c_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ3", "yc3", (Math.Round(rectArrangement.fy_c_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ3", "x3", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ3", "y3", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                }
                if (rectArrangement.iNumberOfScrewsInRow_xDirection_SQ4 != 0 && rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ4 != 0)
                {
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ4", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ4.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ4", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ4.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ4", "xc4", (Math.Round(rectArrangement.fx_c_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ4", "yc4", (Math.Round(rectArrangement.fy_c_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ4", "x4", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ4", "y4", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                }
            }
            else if (screwArrangement != null && screwArrangement is CScrewArrangement_O)
            {
                CScrewArrangement_O rectArrangement = (CScrewArrangement_O)screwArrangement;

                List<string> listScrewGauges = CTEKScrewsManager.LoadTEKScrewsProperties().Select(i => i.gauge).ToList();

                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.ScrewGaugeS.Name, CParamsResources.ScrewGaugeS.Symbol, rectArrangement.referenceScrew.Gauge.ToString(), listScrewGauges, CParamsResources.ScrewGaugeS.Unit));  // TODO prerobit na vyber objektu skrutky z databazy
                // TODO Ondrej, toto by malo byt obecne pre rozny pocet rectangular sequences, pripadne groups
                // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                // Jedna skupina a 2 sekvencie
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ1", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ1", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ1", "xc1", (Math.Round(rectArrangement.fx_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ1", "yc1", (Math.Round(rectArrangement.fy_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ1", "x1", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ1", "y1", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ2", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ2", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ2", "xc2", (Math.Round(rectArrangement.fx_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ2", "yc2", (Math.Round(rectArrangement.fy_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ2", "bx2", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ2", "by2", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            }
            else
            {
                // Screw arrangement is not implemented
            }

            ScrewArrangementParameters = screwArrangmenetProperties;
        }

        public void SetComponentProperties(CPlate plate)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            bool bUseRoofSlope = true;

            List<CComponentParamsView> geometry = new List<CComponentParamsView>();

            geometry.Add(new CComponentParamsViewString(CParamsResources.PlateNameS.Name, "", plate.Name, ""));
            geometry.Add(new CComponentParamsViewString(CParamsResources.PlateThicknessS.Name, CParamsResources.PlateThicknessS.Symbol, (Math.Round(plate.Ft * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateThicknessS.Unit));

            if (plate is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic plateTemp = (CConCom_Plate_B_basic)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidthS.Name, CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fb_X * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeightS.Name, CParamsResources.PlateHeightS.Symbol, (Math.Round(plateTemp.Fh_Y * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeightS.Unit));
            }
            else if (plate is CConCom_Plate_JA)
            {
                CConCom_Plate_JA plateTemp = (CConCom_Plate_JA)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidthS.Name,   CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fb_X * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }
            }
            else if (plate is CConCom_Plate_JB || plate is CConCom_Plate_JBS)
            {
                CConCom_Plate_JB plateTemp = (CConCom_Plate_JB)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidthS.Name, CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fb_X * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_JCS)
            {
                CConCom_Plate_JCS plateTemp = (CConCom_Plate_JCS)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidthS.Name, CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fw_apexHalfLength * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.CrscDepthS.Name, CParamsResources.CrscDepthS.Symbol, (Math.Round(plateTemp.Fd_crsc * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscDepthS.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    // Not implemented
                    //geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }

            else if (plate is CConCom_Plate_KA)
            {
                CConCom_Plate_KA plateTemp = (CConCom_Plate_KA)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }
            }
            else if (plate is CConCom_Plate_KB || plate is CConCom_Plate_KBS)
            {
                CConCom_Plate_KB plateTemp = (CConCom_Plate_KB)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KC || plate is CConCom_Plate_KCS)
            {
                CConCom_Plate_KC plateTemp = (CConCom_Plate_KC)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KD)
            {
                CConCom_Plate_KD plateTemp = (CConCom_Plate_KD)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KES)
            {
                CConCom_Plate_KES plateTemp = (CConCom_Plate_KES)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KFS)
            {
                CConCom_Plate_KFS plateTemp = (CConCom_Plate_KFS)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KK)
            {
                CConCom_Plate_KK plateTemp = (CConCom_Plate_KK)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.RafterWidthS.Name, CParamsResources.RafterWidthS.Symbol, (Math.Round(plateTemp.Fb_XR * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.RafterWidthS.Unit));
            }
            else if (plate is CConCom_Plate_O)
            {
                CConCom_Plate_O plateTemp = (CConCom_Plate_O)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.RafterWidthS.Name, CParamsResources.RafterWidthS.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.RafterWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit));
            }
            else
            {
                // Plate is not implemented
            }

            ComponentGeometry = geometry;

            List<CComponentParamsView> details = new List<CComponentParamsView>();
            details.Add(new CComponentParamsViewString(CParamsResources.PlatePerimeterS.Name, CParamsResources.PlatePerimeterS.Symbol, (Math.Round(plate.fCuttingRouteDistance * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlatePerimeterS.Unit));
            details.Add(new CComponentParamsViewString(CParamsResources.PlateSurfaceS.Name, CParamsResources.PlateSurfaceS.Symbol, (Math.Round(plate.fSurface * fUnitFactor_Area, iNumberOfDecimalPlaces_Area)).ToString(nfi), CParamsResources.PlateSurfaceS.Unit));
            details.Add(new CComponentParamsViewString(CParamsResources.PlateAreaS.Name, CParamsResources.PlateAreaS.Symbol, (Math.Round(plate.fArea * fUnitFactor_Area, iNumberOfDecimalPlaces_Area)).ToString(nfi), CParamsResources.PlateAreaS.Unit));
            details.Add(new CComponentParamsViewString(CParamsResources.PlateVolumeS.Name, CParamsResources.PlateVolumeS.Symbol, (Math.Round(plate.fVolume * fUnitFactor_Volume, iNumberOfDecimalPlaces_Volume)).ToString(nfi), CParamsResources.PlateVolumeS.Unit));
            details.Add(new CComponentParamsViewString(CParamsResources.PlateMassS.Name, CParamsResources.PlateMassS.Symbol, Math.Round(plate.fMass, iNumberOfDecimalPlaces_Mass).ToString(nfi),  CParamsResources.PlateMassS.Unit));
            CCNCPathFinder c = new CCNCPathFinder();
            c.RoutePoints = plate.DrillingRoutePoints;
            double dist = c.GetRouteDistance();
            details.Add(new CComponentParamsViewString(CParamsResources.PlateDrillingRouteDistanceS.Name, CParamsResources.PlateDrillingRouteDistanceS.Symbol, (Math.Round(dist * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateDrillingRouteDistanceS.Unit));

            ComponentDetails = details;
        }

        public void SetComponentProperties(CCrSc crsc)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<CComponentParamsView> geometry = new List<CComponentParamsView>();
            geometry.Add(new CComponentParamsViewString("Name", "", crsc.Name_short, ""));
            geometry.Add(new CComponentParamsViewString("Width", "b", (Math.Round(crsc.b * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(new CComponentParamsViewString("Height", "h", (Math.Round(crsc.h * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(new CComponentParamsViewString("Thickness", "t", (Math.Round(crsc.t_min * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

            ComponentGeometry = geometry;

            List<CComponentParamsView> details = new List<CComponentParamsView>();
            List<CSectionPropertiesText> sectionTexts = CSectionManager.LoadSectionPropertiesNamesSymbolsUnits();
            List<string> listSectionPropertyValue = new List<string>();

            try
            {
                listSectionPropertyValue = CSectionManager.LoadSectionPropertiesStringList(crsc.Name_short);

                foreach (CSectionPropertiesText textRow in sectionTexts)
                {
                    if (listSectionPropertyValue[textRow.ID - 1] != "") // Add only row for property value which is not empty string
                        details.Add(new CComponentParamsViewString(textRow.Text, textRow.Symbol, listSectionPropertyValue[textRow.ID - 1], textRow.Unit_NmmMpa));
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
            details.Add(new CComponentParamsViewString("Gauge", "No", screw.Gauge.ToString(nfi), "[-]"));
            details.Add(new CComponentParamsViewString("Thread diameter", "dt", (Math.Round(screw.Diameter_thread * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsViewString("Shank diameter", "ds", (Math.Round(screw.Diameter_shank * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsViewString("Head diameter", "dh", (Math.Round(screw.D_h_headdiameter * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsViewString("Washer diameter", "dw", (Math.Round(screw.D_w_washerdiameter * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsViewString("Washer thickness", "tw", (Math.Round(screw.T_w_washerthickness * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            details.Add(new CComponentParamsViewString("Mass", "m", Math.Round(screw.Mass, iNumberOfDecimalPlaces_Mass).ToString(nfi), "[kg]"));

            ComponentDetails = details;
        }

        //CODE NOT USED
        //private void CreateCrossSections()
        //{
        //    Color cComponentColor;
        //    float fb;
        //    float fh;
        //    float ft;
        //    float ft_f;    // Flange thickness (temporary for box 63020 with stiffeners)
        //    float fb_fl;   // Flange - Z-section
        //    float fc_lip1; // LIP - Z-section

        //    databaseComponents = new CDatabaseComponents();

        //    crossSections = new Dictionary<string, List<CCrSc_TW>>();

        //    // BOX 10075
        //    List<CCrSc_TW> sectionsInSerie1 = new List<CCrSc_TW>();
        //    fb = databaseComponents.arr_Serie_Box_FS_Dimension[0, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_Box_FS_Dimension[0, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_Box_FS_Dimension[0, 2] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_Box_FS_Colors[0];
        //    sectionsInSerie1.Add(new CCrSc_3_10075_BOX(0, fh, fb, ft, cComponentColor));

        //    crossSections.Add("Box-10075", sectionsInSerie1);

        //    // Z
        //    List<CCrSc_TW> sectionsInSerie2 = new List<CCrSc_TW>();
        //    fh = databaseComponents.arr_Serie_Z_FS_Dimension[0, 0] / 1000f;
        //    fb_fl = databaseComponents.arr_Serie_Z_FS_Dimension[0, 1] / 1000f;
        //    fc_lip1 = databaseComponents.arr_Serie_Z_FS_Dimension[0, 2] / 1000f;
        //    ft = databaseComponents.arr_Serie_Z_FS_Dimension[0, 3] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_Z_FS_Colors[0];
        //    sectionsInSerie2.Add(new CCrSc_3_Z(0, fh, fb_fl, fc_lip1, ft, cComponentColor));

        //    crossSections.Add("Z", sectionsInSerie2);


        //    // C single
        //    List<CCrSc_TW> sectionsInSerie3 = new List<CCrSc_TW>();
        //    fb = databaseComponents.arr_Serie_C_FS_Dimension[0, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_C_FS_Dimension[0, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_C_FS_Dimension[0, 2] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_C_FS_Colors[0];
        //    sectionsInSerie3.Add(new CCrSc_3_270XX_C(0, fh, fb, ft, cComponentColor));

        //    fb = databaseComponents.arr_Serie_C_FS_Dimension[1, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_C_FS_Dimension[1, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_C_FS_Dimension[1, 2] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_C_FS_Colors[1];
        //    sectionsInSerie3.Add(new CCrSc_3_270XX_C(0, fh, fb, ft, cComponentColor));

        //    fb = databaseComponents.arr_Serie_C_FS_Dimension[2, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_C_FS_Dimension[2, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_C_FS_Dimension[2, 2] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_C_FS_Colors[2];
        //    sectionsInSerie3.Add(new CCrSc_3_270XX_C(0, fh, fb, ft, cComponentColor));

        //    fb = databaseComponents.arr_Serie_C_FS_Dimension[3, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_C_FS_Dimension[3, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_C_FS_Dimension[3, 2] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_C_FS_Colors[3];
        //    sectionsInSerie3.Add(new CCrSc_3_50020_C(0, fh, fb, ft, cComponentColor));

        //    crossSections.Add("C-single", sectionsInSerie3);

        //    // C back to back
        //    List<CCrSc_TW> sectionsInSerie4 = new List<CCrSc_TW>();
        //    fb = databaseComponents.arr_Serie_C_BtoB_FS_Dimension[0, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_C_BtoB_FS_Dimension[0, 1] / 1000f;
        //    fc_lip1 = databaseComponents.arr_Serie_C_BtoB_FS_Dimension[0, 2] / 1000f;
        //    ft = databaseComponents.arr_Serie_C_BtoB_FS_Dimension[0, 3] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_C_BtoB_FS_Colors[0];
        //    sectionsInSerie4.Add(new CCrSc_3_270XX_C_BACK_TO_BACK(0, fh, fb, fc_lip1, ft, cComponentColor));

        //    crossSections.Add("C-back to back", sectionsInSerie4);

        //    // C nested
        //    List<CCrSc_TW> sectionsInSerie5 = new List<CCrSc_TW>();
        //    fb = databaseComponents.arr_Serie_C_Nested_FS_Dimension[0, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_C_Nested_FS_Dimension[0, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_C_Nested_FS_Dimension[0, 2] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_C_Nested_FS_Colors[0];
        //    sectionsInSerie5.Add(new CCrSc_3_270XX_C_NESTED(0, fh, fb, ft, cComponentColor)); // C270115n

        //    fb = databaseComponents.arr_Serie_C_Nested_FS_Dimension[1, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_C_Nested_FS_Dimension[1, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_C_Nested_FS_Dimension[1, 2] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_C_Nested_FS_Colors[1];
        //    sectionsInSerie5.Add(new CCrSc_3_50020_C_NESTED(0, fh, fb, ft, cComponentColor)); // C50020n

        //    crossSections.Add("C-nested", sectionsInSerie5);

        //    // BOX 63020
        //    List<CCrSc_TW> sectionsInSerie6 = new List<CCrSc_TW>();
        //    fb = databaseComponents.arr_Serie_Box63020_FS_Dimension[0, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_Box63020_FS_Dimension[0, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_Box63020_FS_Dimension[0, 2] / 1000f;
        //    ft_f = databaseComponents.arr_Serie_Box63020_FS_Dimension[0, 3] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_Box63020_FS_Colors[0];
        //    sectionsInSerie6.Add(new CCrSc_3_63020_BOX(0, fh, fb, ft, ft_f, cComponentColor));

        //    fb = databaseComponents.arr_Serie_Box63020_FS_Dimension[1, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_Box63020_FS_Dimension[1, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_Box63020_FS_Dimension[1, 2] / 1000f;
        //    ft_f = databaseComponents.arr_Serie_Box63020_FS_Dimension[1, 3] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_Box63020_FS_Colors[1];
        //    sectionsInSerie6.Add(new CCrSc_3_63020_BOX(0, fh, fb, ft, ft_f, cComponentColor));

        //    fb = databaseComponents.arr_Serie_Box63020_FS_Dimension[2, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_Box63020_FS_Dimension[2, 1] / 1000f;
        //    ft = databaseComponents.arr_Serie_Box63020_FS_Dimension[2, 2] / 1000f;
        //    ft_f = databaseComponents.arr_Serie_Box63020_FS_Dimension[2, 3] / 1000f;
        //    cComponentColor = databaseComponents.arr_Serie_Box63020_FS_Colors[2];
        //    sectionsInSerie6.Add(new CCrSc_3_63020_BOX(0, fh, fb, ft, ft_f, cComponentColor));

        //    crossSections.Add("Box-63020", sectionsInSerie6);
        //}
        //private void CreatePlates()
        //{
        //    float fb_R; // Rafter Width
        //    float fb; // in plane XY -X coord
        //    float fb2; // in plane XY - X coord
        //    float fh; // in plane XY - Y coord
        //    float fh2; // in plane XY - Y coord
        //    float fl; // out of plane - Z coord
        //    float fl2; // out of plane - Z coord
        //    float ft;
        //    //float fPitch_rad = 11f / 180f * (float)Math.PI; // Roof Pitch - default value (11 deg)
        //    int iNumberofHoles;
        //    bool bUseAdditionalConnectors = true;
        //    int iNumberOfConnectorsInCircleSegment;
        //    int iNumberOfAdditionalConnectorsInCorner;
        //    float fRadius = 0.25f; // Radius of screws in circle
        //    float fRafterDepth = 0.63f;
        //    float fRafterStraightDepth = 0.63f - 2 * 0.025f - 2 * 0.002f;
        //    float fMiddleStiffenerSize = 0.18f;
        //    float fAdditionalConnectorDistance = 0.03f;

        //    Point3D controlpoint = new Point3D(0, 0, 0, 0, 0);
        //    CScrew referenceScrew = new CScrew("TEK", "14");
        //    CAnchor referenceAnchor = new CAnchor(0.02f, 0.18f, 0.5f, true);

        //    CScrewArrangementCircleApexOrKnee screwArrangementCircle;

        //    databaseComponents = new CDatabaseComponents();

        //    plates = new Dictionary<string, List<CPlate>>();

        //    // Serie B
        //    List<CPlate> platesInSerie1 = new List<CPlate>();
        //    for (int i = 0; i < databaseComponents.arr_Serie_B_Names.Length; i++)
        //    {
        //        fb = databaseComponents.arr_Serie_B_Dimension[i, 0] / 1000f;
        //        fb2 = fb;
        //        fh = databaseComponents.arr_Serie_B_Dimension[i, 1] / 1000f;
        //        fl = databaseComponents.arr_Serie_B_Dimension[i, 2] / 1000f;
        //        ft = databaseComponents.arr_Serie_B_Dimension[i, 3] / 1000f;
        //        iNumberofHoles = (int)databaseComponents.arr_Serie_B_Dimension[i, 4];

        //        CScrewArrangement_BX_1 screwArrangement_BX_01 = new CScrewArrangement_BX_1(iNumberofHoles, referenceScrew, referenceAnchor);
        //        platesInSerie1.Add(new CConCom_Plate_B_basic(databaseComponents.arr_Serie_B_Names[i], controlpoint, fb, fh, fl, ft, iNumberofHoles, referenceScrew, referenceAnchor, 0, 0, 0, screwArrangement_BX_01, true)); // B
        //    }
        //    plates.Add("Serie B", platesInSerie1);

        //    // Serie L
        //    List<CPlate> platesInSerie2 = new List<CPlate>();
        //    for (int i = 0; i < databaseComponents.arr_Serie_L_Names.Length; i++)
        //    {
        //        fb = databaseComponents.arr_Serie_L_Dimension[i, 0] / 1000f;
        //        fb2 = fb;
        //        fh = databaseComponents.arr_Serie_L_Dimension[i, 1] / 1000f;
        //        fl = databaseComponents.arr_Serie_L_Dimension[i, 2] / 1000f;
        //        ft = databaseComponents.arr_Serie_L_Dimension[i, 3] / 1000f;
        //        iNumberofHoles = (int)databaseComponents.arr_Serie_L_Dimension[i, 4];

        //        CScrewArrangement_F_or_L screwArrangement_ForL = new CScrewArrangement_F_or_L(iNumberofHoles, referenceScrew);
        //        platesInSerie2.Add(new CConCom_Plate_F_or_L(databaseComponents.arr_Serie_L_Names[i], controlpoint, fb, fh, fl, ft, 0, 0, 0, screwArrangement_ForL, true)); // L
        //    }
        //    plates.Add("Serie L", platesInSerie2);

        //    // Serie LL
        //    List<CPlate> platesInSerie3 = new List<CPlate>();
        //    for (int i = 0; i < databaseComponents.arr_Serie_LL_Names.Length; i++)
        //    {
        //        fb = databaseComponents.arr_Serie_LL_Dimension[i, 0] / 1000f;
        //        fb2 = databaseComponents.arr_Serie_LL_Dimension[i, 1] / 1000f;
        //        fh = databaseComponents.arr_Serie_LL_Dimension[i, 2] / 1000f;
        //        fl = databaseComponents.arr_Serie_LL_Dimension[i, 3] / 1000f;
        //        ft = databaseComponents.arr_Serie_LL_Dimension[i, 4] / 1000f;
        //        iNumberofHoles = (int)databaseComponents.arr_Serie_LL_Dimension[i, 5];

        //        CScrewArrangement_LL screwArrangement_LL = new CScrewArrangement_LL(iNumberofHoles, referenceScrew);
        //        platesInSerie3.Add(new CConCom_Plate_LL(databaseComponents.arr_Serie_LL_Names[i], controlpoint, fb, fb2, fh, fl, ft, 0, 0, 0, screwArrangement_LL, true)); // LL
        //    }
        //    plates.Add("Serie LL", platesInSerie3);

        //    // Serie F
        //    List<CPlate> platesInSerie4 = new List<CPlate>();
        //    for (int i = 0; i < databaseComponents.arr_Serie_F_Names.Length; i++)
        //    {
        //        fb = databaseComponents.arr_Serie_F_Dimension[i, 0] / 1000f;
        //        fb2 = databaseComponents.arr_Serie_F_Dimension[i, 1] / 1000f;
        //        fh = databaseComponents.arr_Serie_F_Dimension[i, 2] / 1000f;
        //        fl = databaseComponents.arr_Serie_F_Dimension[i, 3] / 1000f;
        //        ft = databaseComponents.arr_Serie_F_Dimension[i, 4] / 1000f;
        //        iNumberofHoles = (int)databaseComponents.arr_Serie_F_Dimension[i, 5];
        //        platesInSerie4.Add(new CConCom_Plate_F_or_L(databaseComponents.arr_Serie_F_Names[i], controlpoint, i, fb, fb2, fh, fl, ft, 0f, 0f, 0f, true)); // F
        //    }
        //    plates.Add("Serie F", platesInSerie4);

        //    // Serie Q
        //    List<CPlate> platesInSerie5 = new List<CPlate>();
        //    for (int i = 0; i < databaseComponents.arr_Serie_Q_Names.Length; i++)
        //    {
        //        fb = databaseComponents.arr_Serie_Q_Dimension[i, 0] / 1000f;
        //        fh = databaseComponents.arr_Serie_Q_Dimension[i, 1] / 1000f;
        //        fl = databaseComponents.arr_Serie_Q_Dimension[i, 2] / 1000f;
        //        ft = databaseComponents.arr_Serie_Q_Dimension[i, 3] / 1000f;
        //        iNumberofHoles = (int)databaseComponents.arr_Serie_Q_Dimension[i, 4];
        //        platesInSerie5.Add(new CConCom_Plate_Q_T_Y(databaseComponents.arr_Serie_Q_Names[i], controlpoint, fb, fh, fl, ft, iNumberofHoles, true)); // Q
        //    }
        //    plates.Add("Serie Q", platesInSerie5);

        //    // Serie T
        //    List<CPlate> platesInSerie6 = new List<CPlate>();
        //    for (int i = 0; i < databaseComponents.arr_Serie_T_Names.Length; i++)
        //    {
        //        fb = databaseComponents.arr_Serie_T_Dimension[i, 0] / 1000f;
        //        fh = databaseComponents.arr_Serie_T_Dimension[i, 1] / 1000f;
        //        fl = databaseComponents.arr_Serie_T_Dimension[i, 2] / 1000f;
        //        ft = databaseComponents.arr_Serie_T_Dimension[i, 3] / 1000f;
        //        iNumberofHoles = (int)databaseComponents.arr_Serie_T_Dimension[i, 4];
        //        platesInSerie6.Add(new CConCom_Plate_Q_T_Y(databaseComponents.arr_Serie_T_Names[i], controlpoint, fb, fh, fl, ft, iNumberofHoles, true)); // T
        //    }
        //    plates.Add("Serie T", platesInSerie6);

        //    // Serie Y
        //    List<CPlate> platesInSerie7 = new List<CPlate>();
        //    for (int i = 0; i < databaseComponents.arr_Serie_Y_Names.Length; i++)
        //    {
        //        fb = databaseComponents.arr_Serie_Y_Dimension[i, 0] / 1000f;
        //        fh = databaseComponents.arr_Serie_Y_Dimension[i, 1] / 1000f;
        //        fl = databaseComponents.arr_Serie_Y_Dimension[i, 2] / 1000f;
        //        fl2 = databaseComponents.arr_Serie_Y_Dimension[i, 3] / 1000f;
        //        ft = databaseComponents.arr_Serie_Y_Dimension[i, 4] / 1000f;
        //        iNumberofHoles = (int)databaseComponents.arr_Serie_Y_Dimension[i, 5];
        //        platesInSerie7.Add(new CConCom_Plate_Q_T_Y(databaseComponents.arr_Serie_Y_Names[i], controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles, true)); // Y
        //    }
        //    plates.Add("Serie Y", platesInSerie7);

        //    // Serie J
        //    List<CPlate> platesInSerie8 = new List<CPlate>();
        //    fb = databaseComponents.arr_Serie_J_Dimension[0, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_J_Dimension[0, 1] / 1000f;
        //    fh2 = databaseComponents.arr_Serie_J_Dimension[0, 2] / 1000f;
        //    fl = databaseComponents.arr_Serie_J_Dimension[0, 3] / 1000f;
        //    ft = databaseComponents.arr_Serie_J_Dimension[0, 4] / 1000f;
        //    iNumberOfConnectorsInCircleSegment = (int)databaseComponents.arr_Serie_J_Dimension[0, 5];
        //    iNumberOfAdditionalConnectorsInCorner = (int)databaseComponents.arr_Serie_J_Dimension[0, 6];

        //    screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, 1, iNumberOfConnectorsInCircleSegment, fRadius, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
        //    platesInSerie8.Add(new CConCom_Plate_JA(databaseComponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, screwArrangementCircle, true));

        //    fb = databaseComponents.arr_Serie_J_Dimension[1, 0] / 1000f;
        //    fh = databaseComponents.arr_Serie_J_Dimension[1, 1] / 1000f;
        //    fh2 = databaseComponents.arr_Serie_J_Dimension[1, 2] / 1000f;
        //    fl = databaseComponents.arr_Serie_J_Dimension[1, 3] / 1000f;
        //    ft = databaseComponents.arr_Serie_J_Dimension[1, 4] / 1000f;
        //    iNumberOfConnectorsInCircleSegment = (int)databaseComponents.arr_Serie_J_Dimension[1, 5];
        //    iNumberOfAdditionalConnectorsInCorner = (int)databaseComponents.arr_Serie_J_Dimension[1, 6];

        //    screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, 1, iNumberOfConnectorsInCircleSegment, fRadius, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
        //    platesInSerie8.Add(new CConCom_Plate_JB(databaseComponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

        //    plates.Add("Serie J", platesInSerie8);

        //    // Serie K
        //    List<CPlate> platesInSerie9 = new List<CPlate>();
        //    fb_R = databaseComponents.arr_Serie_K_Dimension[0, 0] / 1000f;
        //    fb = databaseComponents.arr_Serie_K_Dimension[0, 1] / 1000f;
        //    fh = databaseComponents.arr_Serie_K_Dimension[0, 2] / 1000f;
        //    fb2 = databaseComponents.arr_Serie_K_Dimension[0, 3] / 1000f;
        //    fh2 = databaseComponents.arr_Serie_K_Dimension[0, 4] / 1000f;
        //    fl = databaseComponents.arr_Serie_K_Dimension[0, 5] / 1000f;
        //    ft = databaseComponents.arr_Serie_K_Dimension[0, 6] / 1000f;
        //    iNumberOfConnectorsInCircleSegment = (int)databaseComponents.arr_Serie_K_Dimension[0, 7];
        //    iNumberOfAdditionalConnectorsInCorner = (int)databaseComponents.arr_Serie_K_Dimension[0, 8];

        //    screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, 1, iNumberOfConnectorsInCircleSegment, fRadius, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
        //    platesInSerie9.Add(new CConCom_Plate_KA(databaseComponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, screwArrangementCircle, true));

        //    fb_R = databaseComponents.arr_Serie_K_Dimension[1, 0] / 1000f;
        //    fb = databaseComponents.arr_Serie_K_Dimension[1, 1] / 1000f;
        //    fh = databaseComponents.arr_Serie_K_Dimension[1, 2] / 1000f;
        //    fb2 = databaseComponents.arr_Serie_K_Dimension[1, 3] / 1000f;
        //    fh2 = databaseComponents.arr_Serie_K_Dimension[1, 4] / 1000f;
        //    fl = databaseComponents.arr_Serie_K_Dimension[1, 5] / 1000f;
        //    ft = databaseComponents.arr_Serie_K_Dimension[1, 6] / 1000f;
        //    iNumberOfConnectorsInCircleSegment = (int)databaseComponents.arr_Serie_K_Dimension[1, 7];
        //    iNumberOfAdditionalConnectorsInCorner = (int)databaseComponents.arr_Serie_K_Dimension[1, 8];

        //    screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, 1, iNumberOfConnectorsInCircleSegment, fRadius, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
        //    platesInSerie9.Add(new CConCom_Plate_KB(databaseComponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

        //    fb_R = databaseComponents.arr_Serie_K_Dimension[2, 0] / 1000f;
        //    fb = databaseComponents.arr_Serie_K_Dimension[2, 1] / 1000f;
        //    fh = databaseComponents.arr_Serie_K_Dimension[2, 2] / 1000f;
        //    fb2 = databaseComponents.arr_Serie_K_Dimension[2, 3] / 1000f;
        //    fh2 = databaseComponents.arr_Serie_K_Dimension[2, 4] / 1000f;
        //    fl = databaseComponents.arr_Serie_K_Dimension[2, 5] / 1000f;
        //    ft = databaseComponents.arr_Serie_K_Dimension[2, 6] / 1000f;
        //    iNumberOfConnectorsInCircleSegment = (int)databaseComponents.arr_Serie_K_Dimension[2, 7];
        //    iNumberOfAdditionalConnectorsInCorner = (int)databaseComponents.arr_Serie_K_Dimension[2, 8];

        //    screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, 1, iNumberOfConnectorsInCircleSegment, fRadius, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
        //    platesInSerie9.Add(new CConCom_Plate_KC(databaseComponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

        //    fb_R = databaseComponents.arr_Serie_K_Dimension[3, 0] / 1000f;
        //    fb = databaseComponents.arr_Serie_K_Dimension[3, 1] / 1000f;
        //    fh = databaseComponents.arr_Serie_K_Dimension[3, 2] / 1000f;
        //    fb2 = databaseComponents.arr_Serie_K_Dimension[3, 3] / 1000f;
        //    fh2 = databaseComponents.arr_Serie_K_Dimension[3, 4] / 1000f;
        //    fl = databaseComponents.arr_Serie_K_Dimension[3, 5] / 1000f;
        //    ft = databaseComponents.arr_Serie_K_Dimension[3, 6] / 1000f;
        //    iNumberOfConnectorsInCircleSegment = (int)databaseComponents.arr_Serie_K_Dimension[3, 7];
        //    iNumberOfAdditionalConnectorsInCorner = (int)databaseComponents.arr_Serie_K_Dimension[3, 8];

        //    screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, 1, iNumberOfConnectorsInCircleSegment, fRadius, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
        //    platesInSerie9.Add(new CConCom_Plate_KD(databaseComponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

        //    fb_R = databaseComponents.arr_Serie_K_Dimension[4, 0] / 1000f;
        //    fb = databaseComponents.arr_Serie_K_Dimension[4, 1] / 1000f;
        //    fh = databaseComponents.arr_Serie_K_Dimension[4, 2] / 1000f;
        //    fb2 = databaseComponents.arr_Serie_K_Dimension[4, 3] / 1000f;
        //    fh2 = databaseComponents.arr_Serie_K_Dimension[4, 4] / 1000f;
        //    fl = databaseComponents.arr_Serie_K_Dimension[4, 5] / 1000f;
        //    ft = databaseComponents.arr_Serie_K_Dimension[4, 6] / 1000f;
        //    iNumberOfConnectorsInCircleSegment = (int)databaseComponents.arr_Serie_K_Dimension[4, 7];
        //    iNumberOfAdditionalConnectorsInCorner = (int)databaseComponents.arr_Serie_K_Dimension[4, 8];

        //    screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, fRafterDepth, fRafterStraightDepth, fMiddleStiffenerSize, 1, iNumberOfConnectorsInCircleSegment, fRadius, bUseAdditionalConnectors, iNumberOfAdditionalConnectorsInCorner, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
        //    platesInSerie9.Add(new CConCom_Plate_KE(databaseComponents.arr_Serie_K_Names[4], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, screwArrangementCircle, true));

        //    plates.Add("Serie K", platesInSerie9);
        //}
        //private void CreateScrews()
        //{
        //    // TODO - prepracovat a nacitavat vlastnosti z databazy, plati pre vsetky komponenty - prierezy, plechy, skrutky
        //    int iGauge;
        //    float fHoleDiameter;
        //    float fConnectorLength = 0.02f;
        //    float fConnectorMass = 0.012f;
        //    Point3D controlpoint = new Point3D(0, 0, 0, 0, 0);

        //    databaseComponents = new CDatabaseComponents();

        //    screws = new Dictionary<string, List<CScrew>>();

        //    // Serie TEK
        //    List<CScrew> screwsInSerie = new List<CScrew>();
        //    for (int i = 0; i < databaseComponents.arr_Serie_TEK_Names.Length; i++)
        //    {
        //        iGauge = (int)databaseComponents.arr_Screws_TEK_Dimensions[i, 0];
        //        fHoleDiameter = databaseComponents.arr_Screws_TEK_Dimensions[i, 1] / 1000f;

        //        screwsInSerie.Add(new CScrew(databaseComponents.arr_Serie_TEK_Names[i], controlpoint, iGauge, fHoleDiameter,0,0,0, fConnectorLength, fConnectorMass,0,0,0, true));
        //    }
        //    screws.Add("Hex Head Tek", screwsInSerie);
        //}
    }
}
