﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Globalization;
using BaseClasses;
using System.Collections.ObjectModel;
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

        CDatabaseComponents databaseComponents;
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

            IsSetFromCode = false;
        }

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
            geometry.Add(Tuple.Create("Name","", plate.Name, ""));
            geometry.Add(Tuple.Create("Width", "b", (Math.Round(plate.fWidth_bx * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Height", "h", (Math.Round(plate.fHeight_hy * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Number of holes", "nh", plate.IHolesNumber.ToString(nfi), "[-]"));
            geometry.Add(Tuple.Create("Hole diameter", "dh", (Math.Round(plate.FHoleDiameter*fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Hole radius", "rh", (Math.Round(plate.FHoleDiameter*fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            geometry.Add(Tuple.Create("Thickness", "t", (Math.Round(plate.fThickness_tz*fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

            ComponentGeometry = geometry;

            List < Tuple<string, string, string, string> > details = new List<Tuple<string, string, string, string>>();
            CCNCPathFinder c = new CCNCPathFinder();
            c.RoutePoints = plate.DrillingRoutePoints;
            double dist = c.GetRouteDistance();
            details.Add(Tuple.Create("Drilling Route Distance","Ldr", (Math.Round(dist * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
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
    }
}
