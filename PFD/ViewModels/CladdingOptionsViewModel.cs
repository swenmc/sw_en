﻿using BaseClasses;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using DATABASE;
using DATABASE.DTO;
using CRSC;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows;

namespace PFD
{
    [Serializable]
    public class CladdingOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        #region private fields
        private int m_RoofCladdingIndex;
        private int m_RoofCladdingID;
        private int m_RoofCladdingCoatingIndex;
        private int m_RoofCladdingCoatingID;
        private List<CoatingColour> m_RoofCladdingColors;
        private int m_RoofCladdingColorIndex;
        private int m_RoofCladdingThicknessIndex;
        private float m_MaxSheetLengthRoof;
        private float m_RoofCladdingOverlap;

        private int m_WallCladdingIndex;
        private int m_WallCladdingID;
        private int m_WallCladdingCoatingIndex;
        private int m_WallCladdingCoatingID;
        private List<CoatingColour> m_WallCladdingColors;
        private int m_WallCladdingColorIndex;
        private int m_WallCladdingThicknessIndex;
        private float m_MaxSheetLengthWall;
        private float m_WallCladdingOverlap;

        private int m_RoofFibreglassThicknessIndex;
        private int m_ColorRoof_FG_Index;
        private int m_WallFibreglassThicknessIndex;
        private int m_ColorWall_FG_Index;
        private float m_FibreglassRoofSurfaceMass;
        private List<string> m_RoofFibreglassTypes;
        private string m_RoofFibreglassType;
        private int m_RoofFibreglassTypeIndex;
        private float m_FibreglassWallSurfaceMass;
        private List<string> m_WallFibreglassTypes;
        private string m_WallFibreglassType;
        private int m_WallFibreglassTypeIndex;

        private float m_MaxSheetLengthRoofFibreglass;
        private float m_MaxSheetLengthWallFibreglass;
        private float m_RoofFibreglassOverlap;
        private float m_WallFibreglassOverlap;
        private float m_FibreglassAreaRoof;
        private float m_FibreglassAreaWall;
        private List<CoatingColour> m_FibreglassColors;

        private List<string> m_Claddings;
        private List<string> m_Coatings;
        private string m_RoofCladding;
        private string m_WallCladding;
        private string m_RoofCladdingCoating;
        private string m_WallCladdingCoating;

        private CTS_CrscProperties m_RoofCladdingProps;
        private CTS_CrscProperties m_WallCladdingProps;
        private CTS_CoilProperties m_RoofCladdingCoilProps;
        private CTS_CoilProperties m_WallCladdingCoilProps;

        private List<string> m_RoofCladdingsThicknessTypes;
        private List<string> m_WallCladdingsThicknessTypes;
        private string m_RoofCladdingThickness;
        private string m_WallCladdingThickness;

        private List<string> m_RoofFibreglassThicknessTypes;
        private List<string> m_WallFibreglassThicknessTypes;

        private List<string> m_RoofFibreglassSheetMassTypes;
        private List<string> m_WallFibreglassSheetMassTypes;

        private float m_RoofEdgeOverHang_FB_Y;
        private float m_RoofEdgeOverHang_FB_Y_old;
        private float m_RoofEdgeOverHang_LR_X;
        private float m_RoofEdgeOverHang_LR_X_old;
        private float m_CanopyRoofEdgeOverHang_LR_X;
        private float m_WallBottomOffset_Z;
        private float m_WallBottomOffset_Z_old;

        private bool m_ConsiderRoofCladdingFor_FB_WallHeight;

        private ObservableCollection<FibreglassProperties> m_FibreglassProperties;

        #endregion private fields

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        #region Properties
        //-------------------------------------------------------------------------------------------------------------
        public int RoofCladdingIndex
        {
            get
            {
                return m_RoofCladdingIndex;
            }

            set
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                m_RoofCladdingIndex = value;

                bool isChangedFromCode = IsSetFromCode;

                if (!isChangedFromCode) IsSetFromCode = true;
                m_RoofCladdingID = m_RoofCladdingIndex;
                RoofCladding = Claddings.ElementAtOrDefault(m_RoofCladdingIndex);
                RoofCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(RoofCladdingCoatingID) && p.claddingIDs.Contains(RoofCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                RoofCladdingThicknessIndex = 0;
                RoofCladdingThickness = RoofCladdingsThicknessTypes.ElementAtOrDefault(RoofCladdingThicknessIndex);

                RoofFibreglassThicknessTypes = CDatabaseManager.GetStringList("FibreglassSQLiteDB", RoofCladding, "name");
                RoofFibreglassSheetMassTypes = CDatabaseManager.GetStringList("FibreglassSQLiteDB", RoofCladding, "flatsheet_mass_g_m2");
                RoofFibreglassThicknessIndex = 0;

                //SetResultsAreNotValid();
                if (!isChangedFromCode) IsSetFromCode = false;

                //RecreateQuotation = false;//musi byt zmena nasilu
                //RecreateModel = true;
                NotifyPropertyChanged("RoofCladdingIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int RoofCladdingCoatingIndex
        {
            get
            {
                return m_RoofCladdingCoatingIndex;
            }

            set
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                m_RoofCladdingCoatingIndex = value;

                bool isChangedFromCode = IsSetFromCode;

                if (!isChangedFromCode) IsSetFromCode = true;
                RoofCladdingCoatingID = m_RoofCladdingCoatingIndex + 1;
                RoofCladdingCoating = Coatings.ElementAtOrDefault(m_RoofCladdingCoatingIndex);
                RoofCladdingColors = CCoatingColorManager.LoadCoatingColours(RoofCladdingCoatingID);
                RoofCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(RoofCladdingCoatingID) && p.claddingIDs.Contains(RoofCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                RoofCladdingColorIndex = 0;
                RoofCladdingThicknessIndex = 0;
                RoofCladdingThickness = RoofCladdingsThicknessTypes.ElementAtOrDefault(RoofCladdingThicknessIndex);

                if (!isChangedFromCode) IsSetFromCode = false;
                //RecreateQuotation = false;//musi byt zmena nasilu
                //RecreateQuotation = true;
                NotifyPropertyChanged("RoofCladdingCoatingIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int RoofCladdingColorIndex
        {
            get
            {
                return m_RoofCladdingColorIndex;
            }

            set
            {
                m_RoofCladdingColorIndex = value;
                //RecreateQuotation = true;
                NotifyPropertyChanged("RoofCladdingColorIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int RoofCladdingThicknessIndex
        {
            get
            {
                return m_RoofCladdingThicknessIndex;
            }

            set
            {
                m_RoofCladdingThicknessIndex = value;

                RoofCladdingThickness = RoofCladdingsThicknessTypes.ElementAtOrDefault(m_RoofCladdingThicknessIndex);
                //SetResultsAreNotValid();
                //RecreateQuotation = true;
                NotifyPropertyChanged("RoofCladdingThicknessIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallCladdingIndex
        {
            get
            {
                return m_WallCladdingIndex;
            }

            set
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                m_WallCladdingIndex = value;

                bool isChangedFromCode = IsSetFromCode;

                if (!isChangedFromCode) IsSetFromCode = true;
                WallCladdingID = m_WallCladdingIndex;
                WallCladding = Claddings.ElementAtOrDefault(m_WallCladdingIndex);
                WallCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(WallCladdingCoatingID) && p.claddingIDs.Contains(WallCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                WallCladdingThicknessIndex = 0;
                WallCladdingThickness = WallCladdingsThicknessTypes.ElementAtOrDefault(WallCladdingThicknessIndex);

                WallFibreglassThicknessTypes = CDatabaseManager.GetStringList("FibreglassSQLiteDB", WallCladding, "name");
                WallFibreglassSheetMassTypes = CDatabaseManager.GetStringList("FibreglassSQLiteDB", WallCladding, "flatsheet_mass_g_m2");                
                WallFibreglassThicknessIndex = 0;

                if (!isChangedFromCode) IsSetFromCode = false;
                //RecreateQuotation = false;//musi byt zmena nasilu
                //RecreateModel = true;
                NotifyPropertyChanged("WallCladdingIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallCladdingCoatingIndex
        {
            get
            {
                return m_WallCladdingCoatingIndex;
            }

            set
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                m_WallCladdingCoatingIndex = value;

                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode) IsSetFromCode = true;
                WallCladdingCoatingID = m_WallCladdingCoatingIndex + 1;
                WallCladdingCoating = Coatings.ElementAtOrDefault(m_WallCladdingCoatingIndex);
                WallCladdingColors = CCoatingColorManager.LoadCoatingColours(WallCladdingCoatingID);
                WallCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(WallCladdingCoatingID) && p.claddingIDs.Contains(WallCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                WallCladdingThicknessIndex = 0;
                WallCladdingThickness = WallCladdingsThicknessTypes.ElementAtOrDefault(WallCladdingThicknessIndex);
                WallCladdingColorIndex = 0;

                if (!isChangedFromCode) IsSetFromCode = false;
                //RecreateQuotation = false;//musi byt zmena nasilu
                //RecreateQuotation = true;
                NotifyPropertyChanged("WallCladdingCoatingIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallCladdingColorIndex
        {
            get
            {
                return m_WallCladdingColorIndex;
            }

            set
            {
                m_WallCladdingColorIndex = value;
                //RecreateQuotation = true;
                NotifyPropertyChanged("WallCladdingColorIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallCladdingThicknessIndex
        {
            get
            {
                return m_WallCladdingThicknessIndex;
            }

            set
            {
                m_WallCladdingThicknessIndex = value;


                WallCladdingThickness = WallCladdingsThicknessTypes.ElementAtOrDefault(m_WallCladdingThicknessIndex);
                
                //RecreateQuotation = true;
                NotifyPropertyChanged("WallCladdingThicknessIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int RoofFibreglassThicknessIndex
        {
            get
            {
                return m_RoofFibreglassThicknessIndex;
            }

            set
            {
                m_RoofFibreglassThicknessIndex = value;

                if (m_RoofFibreglassThicknessIndex == -1) return;
                InitRoofFibreglassTypes();
                FibreglassRoofSurfaceMass = int.Parse(RoofFibreglassSheetMassTypes.ElementAtOrDefault(RoofFibreglassThicknessIndex));
                //RecreateQuotation = true;
                NotifyPropertyChanged("RoofFibreglassThicknessIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallFibreglassThicknessIndex
        {
            get
            {
                return m_WallFibreglassThicknessIndex;
            }

            set
            {
                m_WallFibreglassThicknessIndex = value;

                if (m_WallFibreglassThicknessIndex == -1) return;
                InitWallFibreglassTypes();
                FibreglassWallSurfaceMass = int.Parse(WallFibreglassSheetMassTypes.ElementAtOrDefault(WallFibreglassThicknessIndex));
                //RecreateQuotation = true;
                NotifyPropertyChanged("WallFibreglassThicknessIndex");
            }
        }

        //percentage of fibreglass of total area roof
        public float FibreglassAreaRoof
        {
            get
            {
                return m_FibreglassAreaRoof;
            }

            set
            {
                if (value < 0.0 || value > 99.0) // Limit is 99% of area
                    throw new ArgumentException("Fibreglass area must be between 0.0 and 99 [%]");
                m_FibreglassAreaRoof = value;
                //RecreateModel = true;
                NotifyPropertyChanged("FibreglassAreaRoof");
            }
        }

        //percentage of fibreglass of total area wall
        //-------------------------------------------------------------------------------------------------------------
        public float FibreglassAreaWall
        {
            get
            {
                return m_FibreglassAreaWall;
            }

            set
            {
                if (value < 0.0 || value > 99.0) // Limit is 99% of area
                    throw new ArgumentException("Fibreglass area must be between 0.0 and 99 [%]");
                m_FibreglassAreaWall = value;
                //RecreateModel = true;
                NotifyPropertyChanged("FibreglassAreaWall");
            }
        }


        public int RoofCladdingID
        {
            get
            {
                return m_RoofCladdingID;
            }

            set
            {
                m_RoofCladdingID = value;
            }
        }

        public int RoofCladdingCoatingID
        {
            get
            {
                return m_RoofCladdingCoatingID;
            }

            set
            {
                m_RoofCladdingCoatingID = value;
            }
        }

        public int WallCladdingID
        {
            get
            {
                return m_WallCladdingID;
            }

            set
            {
                m_WallCladdingID = value;
            }
        }

        public int WallCladdingCoatingID
        {
            get
            {
                return m_WallCladdingCoatingID;
            }

            set
            {
                m_WallCladdingCoatingID = value;
            }
        }

        public List<string> Claddings
        {
            get
            {
                if (m_Claddings == null) m_Claddings = CDatabaseManager.GetStringList("TrapezoidalSheetingSQLiteDB", "trapezoidalSheeting_m", "name");
                return m_Claddings;
            }

            set
            {
                m_Claddings = value;
            }
        }

        public List<string> Coatings
        {
            get
            {
                if (m_Coatings == null) m_Coatings = CDatabaseManager.GetStringList("TrapezoidalSheetingSQLiteDB", "coating", "name_short");
                return m_Coatings;
            }

            set
            {
                m_Coatings = value;
            }
        }

        public string RoofCladding
        {
            get
            {
                return m_RoofCladding;
            }

            set
            {
                m_RoofCladding = value;
                UpdateFibreglassPropertiesRoof();
                SetRoofCladdingProps();
            }
        }

        public string WallCladding
        {
            get
            {
                return m_WallCladding;
            }

            set
            {
                m_WallCladding = value;
                UpdateFibreglassPropertiesWall();
                SetWallCladdingProps();
            }
        }

        public string RoofCladdingCoating
        {
            get
            {
                return m_RoofCladdingCoating;
            }

            set
            {
                m_RoofCladdingCoating = value;
            }
        }

        public string WallCladdingCoating
        {
            get
            {
                return m_WallCladdingCoating;
            }

            set
            {
                m_WallCladdingCoating = value;
            }
        }

        public List<string> RoofCladdingsThicknessTypes
        {
            get
            {
                if (m_RoofCladdingsThicknessTypes == null)
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";
                    m_RoofCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(RoofCladdingCoatingID) && p.claddingIDs.Contains(RoofCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                }
                return m_RoofCladdingsThicknessTypes;
            }

            set
            {
                m_RoofCladdingsThicknessTypes = value;
                NotifyPropertyChanged("RoofCladdingsThicknessTypes");
            }
        }

        public List<string> WallCladdingsThicknessTypes
        {
            get
            {
                if (m_WallCladdingsThicknessTypes == null)
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";
                    m_WallCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(WallCladdingCoatingID) && p.claddingIDs.Contains(WallCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                }
                return m_WallCladdingsThicknessTypes;
            }

            set
            {
                m_WallCladdingsThicknessTypes = value;
                NotifyPropertyChanged("WallCladdingsThicknessTypes");
            }
        }

        private List<CTS_ThicknessProperties> m_ThicknessPropertiesList;
        public List<CTS_ThicknessProperties> ThicknessPropertiesList
        {
            get
            {
                if (m_ThicknessPropertiesList == null) m_ThicknessPropertiesList = CTrapezoidalSheetingManager.LoadThicknessPropertiesList();
                return m_ThicknessPropertiesList;
            }

            set
            {
                m_ThicknessPropertiesList = value;
            }
        }

        public string RoofCladdingThickness
        {
            get
            {
                return m_RoofCladdingThickness;
            }

            set
            {
                m_RoofCladdingThickness = value;
                SetRoofCladdingProps();
            }
        }

        public string WallCladdingThickness
        {
            get
            {
                return m_WallCladdingThickness;
            }

            set
            {
                m_WallCladdingThickness = value;
                SetWallCladdingProps();
            }
        }

        public List<string> RoofFibreglassThicknessTypes
        {
            get
            {
                return m_RoofFibreglassThicknessTypes;
            }

            set
            {
                m_RoofFibreglassThicknessTypes = value;
                NotifyPropertyChanged("RoofFibreglassThicknessTypes");
            }
        }

        public List<string> WallFibreglassThicknessTypes
        {
            get
            {
                return m_WallFibreglassThicknessTypes;
            }

            set
            {
                m_WallFibreglassThicknessTypes = value;
                NotifyPropertyChanged("WallFibreglassThicknessTypes");
            }
        }

        public List<CoatingColour> RoofCladdingColors
        {
            get
            {
                if (m_RoofCladdingColors == null) m_RoofCladdingColors = CCoatingColorManager.LoadCoatingColours(RoofCladdingCoatingIndex + 1);

                return m_RoofCladdingColors;
            }

            set
            {
                m_RoofCladdingColors = value;
                NotifyPropertyChanged("RoofCladdingColors");
            }
        }

        public List<CoatingColour> WallCladdingColors
        {
            get
            {
                if (m_WallCladdingColors == null) m_WallCladdingColors = CCoatingColorManager.LoadCoatingColours(WallCladdingCoatingIndex + 1);
                return m_WallCladdingColors;
            }

            set
            {
                m_WallCladdingColors = value;
                NotifyPropertyChanged("WallCladdingColors");
            }
        }

        public CTS_CrscProperties RoofCladdingProps
        {
            get
            {
                if (m_RoofCladdingProps == null)
                {
                    SetRoofCladdingProps();
                }
                //tato podmienka nizsie je pekna ale vobec nefunguje
                //else if (m_RoofCladdingProps.name != RoofCladding || m_RoofCladdingProps.thickness_for_name != RoofCladdingThickness)
                //{
                //    RoofCladdingProps = CTrapezoidalSheetingManager.GetSectionProperties($"{RoofCladding}-{RoofCladdingThickness}");                    
                //}
                return m_RoofCladdingProps;
            }

            set
            {
                m_RoofCladdingProps = value;
                //SetCTS_CoilProperties_Roof();
            }
        }

        public CTS_CrscProperties WallCladdingProps
        {
            get
            {
                if (m_WallCladdingProps == null)
                {
                    SetWallCladdingProps();
                }
                //tato podmienka nizsie je pekna ale vobec nefunguje
                //else if (m_WallCladdingProps.name != WallCladding || m_WallCladdingProps.thickness_for_name != WallCladdingThickness)
                //{
                //    WallCladdingProps = CTrapezoidalSheetingManager.GetSectionProperties($"{WallCladding}-{WallCladdingThickness}");                    
                //}
                return m_WallCladdingProps;
            }

            set
            {
                m_WallCladdingProps = value;
                //SetCTS_CoilProperties_Wall();
            }
        }

        public CTS_CoilProperties RoofCladdingCoilProps
        {
            get
            {
                return m_RoofCladdingCoilProps;
            }

            set
            {
                m_RoofCladdingCoilProps = value;
            }
        }

        public CTS_CoilProperties WallCladdingCoilProps
        {
            get
            {
                return m_WallCladdingCoilProps;
            }

            set
            {
                m_WallCladdingCoilProps = value;
            }
        }

        public float MaxSheetLengthRoof
        {
            get
            {
                return m_MaxSheetLengthRoof;
            }

            set
            {
                if (value < 1.0 || value > 20.0)
                    throw new ArgumentException("Maximum sheet length must be between 1 and 20 [m]");
                m_MaxSheetLengthRoof = value;
                NotifyPropertyChanged("MaxSheetLengthRoof");
            }
        }

        public float RoofCladdingOverlap
        {
            get
            {
                return m_RoofCladdingOverlap;
            }

            set
            {
                if (value < 0.05 || value > 0.50)
                    throw new ArgumentException("Overlap length must be between 50 and 500 [mm]");
                m_RoofCladdingOverlap = value;
                NotifyPropertyChanged("RoofCladdingOverlap");
            }
        }

        public float MaxSheetLengthWall
        {
            get
            {
                return m_MaxSheetLengthWall;
            }

            set
            {
                if (value < 1.0 || value > 20.0)
                    throw new ArgumentException("Maximum sheet length must be between 1 and 20 [m]");
                m_MaxSheetLengthWall = value;
                NotifyPropertyChanged("MaxSheetLengthWall");
            }
        }

        public float WallCladdingOverlap
        {
            get
            {
                return m_WallCladdingOverlap;
            }

            set
            {
                if (value < 0.05 || value > 0.50)
                    throw new ArgumentException("Overlap length must be between 50 and 500 [mm]");
                m_WallCladdingOverlap = value;
                NotifyPropertyChanged("WallCladdingOverlap");
            }
        }

        public float MaxSheetLengthRoofFibreglass
        {
            get
            {
                return m_MaxSheetLengthRoofFibreglass;
            }

            set
            {
                if (value < 1.0 || value > 10.0)
                    throw new ArgumentException("Maximum sheet length must be between 1 and 10 [m]");
                m_MaxSheetLengthRoofFibreglass = value;
                NotifyPropertyChanged("MaxSheetLengthRoofFibreglass");
            }
        }

        public float MaxSheetLengthWallFibreglass
        {
            get
            {
                return m_MaxSheetLengthWallFibreglass;
            }

            set
            {
                if (value < 1.0 || value > 10.0)
                    throw new ArgumentException("Maximum sheet length must be between 1 and 10 [m]");
                m_MaxSheetLengthWallFibreglass = value;
                NotifyPropertyChanged("MaxSheetLengthWallFibreglass");
            }
        }

        public float RoofFibreglassOverlap
        {
            get
            {
                return m_RoofFibreglassOverlap;
            }

            set
            {
                if (value < 0.05 || value > 0.50)
                    throw new ArgumentException("Overlap length must be between 50 and 500 [mm]");
                m_RoofFibreglassOverlap = value;
                NotifyPropertyChanged("RoofFibreglassOverlap");
            }
        }

        public float WallFibreglassOverlap
        {
            get
            {
                return m_WallFibreglassOverlap;
            }

            set
            {
                if (value < 0.05 || value > 0.50)
                    throw new ArgumentException("Overlap length must be between 50 and 500 [mm]");
                m_WallFibreglassOverlap = value;
                NotifyPropertyChanged("WallFibreglassOverlap");
            }
        }

        public float RoofEdgeOverHang_FB_Y
        {
            get
            {
                return m_RoofEdgeOverHang_FB_Y;
            }

            set
            {
                if (value < 0.00 || value > 0.30)
                    throw new ArgumentException("Overhang length must be between 0 and 300 [mm]");
                m_RoofEdgeOverHang_FB_Y_old = m_RoofEdgeOverHang_FB_Y;
                m_RoofEdgeOverHang_FB_Y = value;

                //task 732
                if (m_ConsiderRoofCladdingFor_FB_WallHeight && m_RoofEdgeOverHang_FB_Y > 0)
                {
                    MessageBox.Show("Invalid input. Roof cladding is in the collision with front/back wall cladding.");
                    m_RoofEdgeOverHang_FB_Y = m_RoofEdgeOverHang_FB_Y_old;
                    //throw new Exception("Invalid input. Roof cladding is in the collision with front/back wall cladding.");
                }                    

                UpdateFibreglassProperties();
                NotifyPropertyChanged("RoofEdgeOverHang_FB_Y");
            }
        }
        public float RoofEdgeOverHang_FB_Y_old
        {
            get
            {
                return m_RoofEdgeOverHang_FB_Y_old;
            }

            set
            {
                m_RoofEdgeOverHang_FB_Y_old = value;
            }
        }
        public float RoofEdgeOverHang_LR_X
        {
            get
            {
                return m_RoofEdgeOverHang_LR_X;
            }

            set
            {
                if (value < 0.00 || value > 0.50)
                    throw new ArgumentException("Overhang length must be between 0 and 500 [mm]");
                m_RoofEdgeOverHang_LR_X_old = m_RoofEdgeOverHang_LR_X;
                m_RoofEdgeOverHang_LR_X = value;
                UpdateFibreglassProperties();
                NotifyPropertyChanged("RoofEdgeOverHang_LR_X");
            }
        }
        public float RoofEdgeOverHang_LR_X_old
        {
            get
            {
                return m_RoofEdgeOverHang_LR_X_old;
            }

            set
            {
                m_RoofEdgeOverHang_LR_X_old = value;
            }
        }

        public float CanopyRoofEdgeOverHang_LR_X
        {
            get
            {
                return m_CanopyRoofEdgeOverHang_LR_X;
            }

            set
            {
                if (value < 0.00 || value > 0.50)
                    throw new ArgumentException("Overhang length must be between 0 and 500 [mm]");
                m_CanopyRoofEdgeOverHang_LR_X = value;
                NotifyPropertyChanged("CanopyRoofEdgeOverHang_LR_X");
            }
        }

        public float WallBottomOffset_Z
        {
            get
            {
                return m_WallBottomOffset_Z;
            }

            set
            {
                if (value < -0.50 || value > 0.00)
                    throw new ArgumentException("Bottom offset under floor level must be between -500 and 0 [mm]");
                m_WallBottomOffset_Z_old = m_WallBottomOffset_Z;
                m_WallBottomOffset_Z = value;
                UpdateFibreglassProperties();
                NotifyPropertyChanged("WallBottomOffset_Z");
            }
        }
        public float WallBottomOffset_Z_old
        {
            get
            {
                return m_WallBottomOffset_Z_old;
            }

            set
            {
                m_WallBottomOffset_Z_old = value;
            }
        }

        public bool ConsiderRoofCladdingFor_FB_WallHeight
        {
            get
            {
                return m_ConsiderRoofCladdingFor_FB_WallHeight;
            }

            set
            {
                m_ConsiderRoofCladdingFor_FB_WallHeight = value;

                //task 732
                if (m_ConsiderRoofCladdingFor_FB_WallHeight && m_RoofEdgeOverHang_FB_Y > 0)
                {
                    MessageBox.Show("Invalid input. Roof cladding is in the collision with front/back wall cladding.");
                    m_ConsiderRoofCladdingFor_FB_WallHeight = !m_ConsiderRoofCladdingFor_FB_WallHeight;
                    //throw new Exception("Invalid input. Roof cladding is in the collision with front/back wall cladding.");
                }   

                NotifyPropertyChanged("ConsiderRoofCladdingFor_FB_WallHeight");
            }
        }


        public ObservableCollection<FibreglassProperties> FibreglassProperties
        {
            get
            {
                if (m_FibreglassProperties == null) m_FibreglassProperties = new ObservableCollection<FibreglassProperties>();
                return m_FibreglassProperties;
            }

            set
            {
                m_FibreglassProperties = value;
                if (m_FibreglassProperties == null) return;
                m_FibreglassProperties.CollectionChanged += FibreglassProperties_CollectionChanged;
                foreach (FibreglassProperties f in m_FibreglassProperties)
                {
                    f.PropertyChanged -= HandleFibreglassPropertiesPropertyChangedEvent;
                    f.PropertyChanged += HandleFibreglassPropertiesPropertyChangedEvent;
                }
                SetFibreglassAreasPercentages();
                NotifyPropertyChanged("FibreglassProperties");
            }
        }

        public int ColorRoof_FG_Index
        {
            get
            {
                return m_ColorRoof_FG_Index;
            }

            set
            {
                m_ColorRoof_FG_Index = value;
                NotifyPropertyChanged("ColorRoof_FG_Index");
            }
        }

        public int ColorWall_FG_Index
        {
            get
            {
                return m_ColorWall_FG_Index;
            }

            set
            {
                m_ColorWall_FG_Index = value;
                NotifyPropertyChanged("ColorWall_FG_Index");
            }
        }

        public List<CoatingColour> FibreglassColors
        {
            get
            {
                return m_FibreglassColors;
            }

            set
            {
                m_FibreglassColors = value;
                NotifyPropertyChanged("FibreglassColors");
            }
        }

        public float FibreglassRoofSurfaceMass
        {
            get
            {
                return m_FibreglassRoofSurfaceMass;
            }

            set
            {
                m_FibreglassRoofSurfaceMass = value;
                NotifyPropertyChanged("FibreglassRoofSurfaceMass");
            }
        }

        public List<string> RoofFibreglassTypes
        {
            get
            {
                return m_RoofFibreglassTypes;
            }

            set
            {
                m_RoofFibreglassTypes = value;
                NotifyPropertyChanged("RoofFibreglassTypes");
            }
        }

        public string RoofFibreglassType
        {
            get
            {
                return m_RoofFibreglassType;
            }

            set
            {
                m_RoofFibreglassType = value;
                NotifyPropertyChanged("RoofFibreglassType");
            }
        }

        public float FibreglassWallSurfaceMass
        {
            get
            {
                return m_FibreglassWallSurfaceMass;
            }

            set
            {
                m_FibreglassWallSurfaceMass = value;
                NotifyPropertyChanged("FibreglassWallSurfaceMass");
            }
        }

        public List<string> WallFibreglassTypes
        {
            get
            {
                return m_WallFibreglassTypes;
            }

            set
            {
                m_WallFibreglassTypes = value; 
                NotifyPropertyChanged("WallFibreglassTypes");
            }
        }

        public string WallFibreglassType
        {
            get
            {
                return m_WallFibreglassType;
            }

            set
            {
                m_WallFibreglassType = value;                
            }
        }

        public int RoofFibreglassTypeIndex
        {
            get
            {
                return m_RoofFibreglassTypeIndex;
            }

            set
            {
                m_RoofFibreglassTypeIndex = value;
                RoofFibreglassType = RoofFibreglassTypes.ElementAtOrDefault(m_RoofFibreglassTypeIndex);
                NotifyPropertyChanged("RoofFibreglassTypeIndex");
            }
        }

        public int WallFibreglassTypeIndex
        {
            get
            {
                return m_WallFibreglassTypeIndex;
            }

            set
            {
                m_WallFibreglassTypeIndex = value;
                WallFibreglassType = WallFibreglassTypes.ElementAtOrDefault(m_WallFibreglassTypeIndex);
                NotifyPropertyChanged("WallFibreglassTypeIndex");
            }
        }

        public List<string> RoofFibreglassSheetMassTypes
        {
            get
            {
                return m_RoofFibreglassSheetMassTypes;
            }

            set
            {
                m_RoofFibreglassSheetMassTypes = value;
            }
        }

        public List<string> WallFibreglassSheetMassTypes
        {
            get
            {
                return m_WallFibreglassSheetMassTypes;
            }

            set
            {
                m_WallFibreglassSheetMassTypes = value;
            }
        }

        





        //TO Mato - co budeme nastavovat, co sa ma udiat ked pridame, zmazeme riadok a podobne?Alebo budeme reagovat az ked sa opusti tab?
        private void FibreglassProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                FibreglassProperties f = m_FibreglassProperties.LastOrDefault();
                if (f != null)
                {
                    //// Neviem ci je vhodne aby boli toto vsetko properties a ak ano, tak ci to rovno nedat priamo do pfdVM
                    //double height_1_final_edge_LR_Wall;
                    //double height_2_final_edge_LR_Wall;
                    //double height_1_final_edge_FB_Wall;
                    //double height_2_final_edge_FB_Wall;
                    //double additionalOffsetWall;

                    //CalculateWallHeightsForCladding(
                    //out height_1_final_edge_LR_Wall,
                    //out height_2_final_edge_LR_Wall,
                    //out height_1_final_edge_FB_Wall,
                    //out height_2_final_edge_FB_Wall,
                    ////out bottomEdge_z,
                    //out additionalOffsetWall);

                    //To Mato - skontrolovat parametre
                    //Podla mna pre strechu staci tento parameter _pfdVM.RoofSideLength a ten kontrolovat
                    f.SetDefaults((EModelType_FS)_pfdVM.KitsetTypeIndex, _pfdVM.WidthOverall, _pfdVM.LengthOverall, _pfdVM.RoofPitch_deg,
                        _pfdVM.Height_1_final_edge_LR_Wall, _pfdVM.Height_2_final_edge_LR_Wall,
                        _pfdVM.Height_1_final_edge_FB_Wall, _pfdVM.Height_2_final_edge_FB_Wall,
                        WallBottomOffset_Z, _pfdVM.AdditionalOffsetWall,
                        RoofEdgeOverHang_LR_X, RoofEdgeOverHang_FB_Y,
                        _pfdVM._claddingOptionsVM.WallCladdingProps.widthModular_m, _pfdVM._claddingOptionsVM.RoofCladdingProps.widthModular_m);
                    ChangeToBeUnique(f);
                    f.PropertyChanged += HandleFibreglassPropertiesPropertyChangedEvent;
                    NotifyPropertyChanged("FibreglassProperties_Add");
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                //To Mato - treba nam tu nieco nastavovat? alebo dame len nejaku property changed, aby sme vedeli,ze bola nejaka zmena?
                //RecreateModel = true;
                //RecreateJoints = true;
                //RecreateFloorSlab = true;
                //SetResultsAreNotValid();
                NotifyPropertyChanged("FibreglassProperties_CollectionChanged");
            }
            SetFibreglassAreasPercentages();
        }

        private void ChangeToBeUnique(FibreglassProperties f)
        {
            if (m_FibreglassProperties.Count < 1) return;
            List<FibreglassProperties> list = new List<FibreglassProperties>(m_FibreglassProperties.Take(m_FibreglassProperties.Count - 1));

            if (!list.Exists(fp => fp.IsInCollisionWith(f))) return;

            for (int i = 1; i < f.XValues.Count; i++)
            {
                f.X = f.XValues[i];
                if (!list.Exists(fp => fp.IsInCollisionWith(f))) return;
            }
                        
            for (int s = 1; s < f.Sides.Count; s++)
            {
                f.Side = f.Sides[s];
                if (!list.Exists(fp => fp.IsInCollisionWith(f))) return;
                for (int i = 0; i < f.XValues.Count; i++)
                {
                    f.X = f.XValues[i];
                    if (!list.Exists(fp => fp.IsInCollisionWith(f))) return;
                }
            }            
        }

        private void HandleFibreglassPropertiesPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (IsSetFromCode) return;

                if (e.PropertyName == "Side")
                {

                }

                SetFibreglassAreasPercentages();

                this.PropertyChanged(sender, e);
            }
            catch (Exception ex)
            {
                //tu by sa mohli detekovat nejake kolizie
            }
        }

        #endregion Properties

        CPFDViewModel _pfdVM;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CladdingOptionsViewModel()
        {
            IsSetFromCode = true;

            RoofCladdingIndex = 1;
            RoofCladdingCoatingIndex = 1;
            RoofCladdingColorIndex = 8;
            WallCladdingIndex = 0;
            WallCladdingCoatingIndex = 1;
            WallCladdingColorIndex = 8;
            FibreglassAreaRoof = 0; // % 0-ziadne fibreglass, 99 - takmer cela strecha fibreglass
            FibreglassAreaWall = 0; // % 0-ziadne fibreglass, 99 - takmer cela strecha fibreglass

            LoadFibreglassColors();
            ColorRoof_FG_Index = 0;
            ColorWall_FG_Index = 0;

            MaxSheetLengthRoof = 6;
            MaxSheetLengthWall = 6;
            MaxSheetLengthRoofFibreglass = 3;
            MaxSheetLengthWallFibreglass = 3;

            RoofCladdingOverlap = 0.15f;
            WallCladdingOverlap = 0.15f;
            RoofFibreglassOverlap = 0.15f;
            WallFibreglassOverlap = 0.15f;

            RoofEdgeOverHang_FB_Y = 0.0f;
            RoofEdgeOverHang_LR_X = 0.15f;
            CanopyRoofEdgeOverHang_LR_X = 0.15f;
            WallBottomOffset_Z = -0.05f;
            ConsiderRoofCladdingFor_FB_WallHeight = true;

            FibreglassProperties = new ObservableCollection<FibreglassProperties>();
            
            IsSetFromCode = false;
        }

        //Toto je premiestnene do PFDViewModel
        //TODO - zmazat
        //public void CalculateWallHeightsForCladding(
        //    out double height_1_final_edge_LR_Wall,
        //    out double height_2_final_edge_LR_Wall,
        //    out double height_1_final_edge_FB_Wall,
        //    out double height_2_final_edge_FB_Wall,
        //    //out double bottomEdge_z,
        //    out double additionalOffsetWall)
        //{
        //    double claddingHeight_Roof = 0;

        //    if (m_RoofCladdingProps != null)
        //       claddingHeight_Roof = m_RoofCladdingProps.height_m;
        //    else
        //    {
        //        CTS_CrscProperties prop = CTrapezoidalSheetingManager.LoadCrossSectionProperties_meters(RoofCladding);
        //        claddingHeight_Roof = prop.height_m;
        //    }

        //    //bottomEdge_z = WallBottomOffset_Z;

        //    CCrSc_TW edgeColumn = CrScFactory.GetCrSc(_pfdVM.ComponentList[(int)EMemberType_FS_Position.EdgeColumn].Section);

        //    double column_crsc_y_minus = edgeColumn.y_min;
        //    double column_crsc_y_plus = edgeColumn.y_max;
        //    double column_crsc_z_plus = edgeColumn.z_max;

        //    additionalOffsetWall = 0.005;  // 5 mm Aby nekolidovali plochy cladding s members
        //    double additionalOffsetRoof = 0.010; // Aby nekolidovali plochy cladding s members (cross-bracing) na streche

        //    // Pridame odsadenie aby prvky ramov konstrukcie vizualne nekolidovali s povrchom cladding
        //    double column_crsc_y_minus_temp = column_crsc_y_minus - additionalOffsetWall;
        //    double column_crsc_y_plus_temp = column_crsc_y_plus + additionalOffsetWall;
        //    double column_crsc_z_plus_temp = column_crsc_z_plus + additionalOffsetWall;

        //    double height_1_final = _pfdVM.WallHeight + column_crsc_z_plus / Math.Cos(_pfdVM.RoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon - prevziat z vypoctu polohy edge purlin
        //    double height_2_final = _pfdVM.Height_H2 + column_crsc_z_plus / Math.Cos(_pfdVM.RoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon

        //    height_1_final_edge_LR_Wall = height_1_final - column_crsc_z_plus_temp * Math.Tan(_pfdVM.RoofPitch_deg * Math.PI / 180);
        //    height_2_final_edge_LR_Wall = height_2_final;

        //    double height_1_final_edge_Roof = height_1_final + additionalOffsetRoof - (column_crsc_z_plus_temp + RoofEdgeOverHang_LR_X) * Math.Tan(_pfdVM.RoofPitch_deg * Math.PI / 180);
        //    double height_2_final_edge_Roof = height_2_final + additionalOffsetRoof;

        //    if (_pfdVM.KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
        //    {
        //        height_2_final_edge_LR_Wall = height_2_final + column_crsc_z_plus_temp * Math.Tan(_pfdVM.RoofPitch_deg * Math.PI / 180);
        //        height_2_final_edge_Roof = height_2_final + additionalOffsetRoof + (column_crsc_z_plus_temp + RoofEdgeOverHang_LR_X) * Math.Tan(_pfdVM.RoofPitch_deg * Math.PI / 180);
        //    }

        //    // Nastavime rovnaku vysku hornej hrany
        //    height_1_final_edge_FB_Wall = height_1_final_edge_LR_Wall;
        //    height_2_final_edge_FB_Wall = height_2_final_edge_LR_Wall;

        //    if (ConsiderRoofCladdingFor_FB_WallHeight)
        //    {
        //        height_1_final_edge_FB_Wall = height_1_final_edge_FB_Wall + claddingHeight_Roof * Math.Tan(_pfdVM.RoofPitch_deg * Math.PI / 180);
        //        height_2_final_edge_FB_Wall = height_2_final_edge_FB_Wall + claddingHeight_Roof * Math.Tan(_pfdVM.RoofPitch_deg * Math.PI / 180);

        //        if (_pfdVM.KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
        //            height_2_final_edge_FB_Wall = height_2_final + (column_crsc_z_plus_temp + claddingHeight_Roof) * Math.Tan(_pfdVM.RoofPitch_deg * Math.PI / 180);
        //    }
        //}

        public void SetDefaultValuesOnModelIndexChange(CPFDViewModel pfdVM)
        {
            _pfdVM = pfdVM;

            RoofCladdingIndex = 1;
            RoofCladdingCoatingIndex = 1;
            RoofCladdingColorIndex = 8;
            WallCladdingIndex = 0;
            WallCladdingCoatingIndex = 1;
            WallCladdingColorIndex = 8;
            FibreglassAreaRoof = 0; // % 0-ziadne fibreglass, 99 - takmer cela strecha fibreglass
            FibreglassAreaWall = 0; // % 0-ziadne fibreglass, 99 - takmer cela strecha fibreglass

            MaxSheetLengthRoof = 6;
            MaxSheetLengthWall = 6;
            MaxSheetLengthRoofFibreglass = 3;
            MaxSheetLengthWallFibreglass = 3;

            RoofCladdingOverlap = 0.15f;
            WallCladdingOverlap = 0.15f;
            RoofFibreglassOverlap = 0.15f;
            WallFibreglassOverlap = 0.15f;

            RoofEdgeOverHang_FB_Y = 0.0f;
            RoofEdgeOverHang_LR_X = 0.15f;
            CanopyRoofEdgeOverHang_LR_X = 0.15f;
            WallBottomOffset_Z = -0.05f;
            ConsiderRoofCladdingFor_FB_WallHeight = true;

            FibreglassProperties = new ObservableCollection<FibreglassProperties>();

            ////to Mato 748 - skontrolovat parametre
            ////temp for testing first object
            //FibreglassProperties f = new FibreglassProperties();
            //f.SetDefaults((EModelType_FS)_pfdVM.KitsetTypeIndex, _pfdVM.WidthOverall, _pfdVM.LengthOverall, _pfdVM.RoofPitch_deg,
            //  _pfdVM.Height_1_final_edge_LR_Wall, _pfdVM.Height_2_final_edge_LR_Wall,
            //  _pfdVM.Height_1_final_edge_FB_Wall, _pfdVM.Height_2_final_edge_FB_Wall,
            //  WallBottomOffset_Z, _pfdVM.AdditionalOffsetWall,
            //  RoofEdgeOverHang_LR_X, RoofEdgeOverHang_FB_Y,
            //  _pfdVM._claddingOptionsVM.WallCladdingProps.widthModular_m, _pfdVM._claddingOptionsVM.RoofCladdingProps.widthModular_m);
            //FibreglassProperties.Add(f);
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        //To Mato 755 - zamysli sa, ci treba kontrolovat,updatovat atd aj RoofEdgeOverHang_LR_X, RoofEdgeOverHang_FB_Y
        private void UpdateFibreglassProperties()
        {            
            foreach (FibreglassProperties f in this.FibreglassProperties)
            {
                //tuto property ze treba vracat mam otestovane, tie dalsie 2 dole si uz isty nie som,ci je nutne takto osetrit
                if (!MathF.d_equal(f.BottomEdge_z, WallBottomOffset_Z))
                {
                    f.BottomEdge_z = WallBottomOffset_Z;
                    if (!f.ValidateMaxHeight())
                    {
                        MessageBox.Show("Fibreglass is outside of building dimensions.");
                        WallBottomOffset_Z = WallBottomOffset_Z_old;
                        UpdateFibreglassProperties();
                    } 
                }

                if (!MathF.d_equal(f.RoofEdgeOverhang_X, RoofEdgeOverHang_LR_X))
                {
                    f.RoofEdgeOverhang_X = RoofEdgeOverHang_LR_X;
                    if (!f.ValidateMaxHeight())
                    {
                        MessageBox.Show("Fibreglass is outside of building dimensions.");
                        RoofEdgeOverHang_LR_X = RoofEdgeOverHang_LR_X_old;
                        UpdateFibreglassProperties();
                    }
                }

                if (!MathF.d_equal(f.RoofEdgeOverhang_Y, RoofEdgeOverHang_FB_Y))
                {
                    f.RoofEdgeOverhang_Y = RoofEdgeOverHang_FB_Y;
                    if (!f.ValidateMaxHeight())
                    {
                        MessageBox.Show("Fibreglass is outside of building dimensions.");
                        RoofEdgeOverHang_FB_Y = RoofEdgeOverHang_FB_Y_old;
                        UpdateFibreglassProperties();
                    }
                }
            }            
        }

        private void UpdateFibreglassPropertiesWall()
        {
            bool changed = false;
            foreach (FibreglassProperties f in this.FibreglassProperties)
            {
                if (MathF.d_equal(f.CladdingWidthModular_Wall, WallCladdingProps.widthModular_m)) continue; //do not change if nothing changes
                changed = true;
                f.CladdingWidthModular_Wall = (float)WallCladdingProps.widthModular_m;
            }
            if (changed) ValidateFibreglassPropertiesRemoveDuplicities();
        }
        private void UpdateFibreglassPropertiesRoof()
        {
            bool changed = false;
            foreach (FibreglassProperties f in this.FibreglassProperties)
            {
                if (MathF.d_equal(f.CladdingWidthModular_Roof, RoofCladdingProps.widthModular_m)) continue; //do not change if nothing changes
                changed = true;
                f.CladdingWidthModular_Roof = (float)RoofCladdingProps.widthModular_m;
            }
            if (changed) ValidateFibreglassPropertiesRemoveDuplicities();
        }

        public void UpdateFibreglassPropertiesMaxX()
        {
            foreach (FibreglassProperties f in this.FibreglassProperties)
            {
                if (!MathF.d_equal(f.ModelTotalLengthFront, _pfdVM.WidthOverall)) f.ModelTotalLengthFront = _pfdVM.WidthOverall;
                if (!MathF.d_equal(f.ModelTotalLengthLeft, _pfdVM.LengthOverall)) f.ModelTotalLengthLeft = _pfdVM.LengthOverall;
            }
        }

        public void ValidateFibreglassPropertiesRemoveDuplicities()
        {
            List<FibreglassProperties> newListWithoutDuplicities = new List<FibreglassProperties>();
            foreach (FibreglassProperties f in FibreglassProperties)
            {
                if (!newListWithoutDuplicities.Contains(f)) newListWithoutDuplicities.Add(f);
            }

            if (newListWithoutDuplicities.Count < FibreglassProperties.Count) // ak je prvkov naozaj menej, tak sa nastavi nova kolekcia, inak netreba robit nic
            {
                FibreglassProperties = new ObservableCollection<FibreglassProperties>(newListWithoutDuplicities);
            }
        }

        //public void FindAndRemoveDuplicities(int startFromIndex, FibreglassProperties f)
        //{
        //    if (f == null) return;
        //    List<int> indexesToDelete = new List<int>();
        //    for (int i = FibreglassProperties.Count - 1; i >= startFromIndex; i--)
        //    {
        //        if (f.Equals(FibreglassProperties[i])) indexesToDelete.Add(i);
        //    }

        //    foreach (int index in indexesToDelete) FibreglassProperties.RemoveAt(index);
        //}

        public void SetViewModel(CladdingOptionsViewModel newVM)
        {
            IsSetFromCode = true;

            //otazka je, ci tu nebudu chybat aj nejake zoznamy hodnot

            RoofCladdingIndex = newVM.RoofCladdingIndex;
            RoofCladdingCoatingIndex = newVM.RoofCladdingCoatingIndex;
            RoofCladdingColorIndex = newVM.RoofCladdingColorIndex;
            WallCladdingIndex = newVM.WallCladdingIndex;
            WallCladdingCoatingIndex = newVM.WallCladdingCoatingIndex;
            WallCladdingColorIndex = newVM.WallCladdingColorIndex;
            FibreglassAreaRoof = newVM.FibreglassAreaRoof;
            FibreglassAreaWall = newVM.FibreglassAreaWall;

            MaxSheetLengthRoof = newVM.MaxSheetLengthRoof;
            MaxSheetLengthWall = newVM.MaxSheetLengthWall;
            MaxSheetLengthRoofFibreglass = newVM.MaxSheetLengthRoofFibreglass;
            MaxSheetLengthWallFibreglass = newVM.MaxSheetLengthWallFibreglass;

            RoofCladdingOverlap = newVM.RoofCladdingOverlap;
            WallCladdingOverlap = newVM.WallCladdingOverlap;
            RoofFibreglassOverlap = newVM.RoofFibreglassOverlap;
            WallFibreglassOverlap = newVM.WallFibreglassOverlap;

            RoofEdgeOverHang_FB_Y = newVM.RoofEdgeOverHang_FB_Y;
            RoofEdgeOverHang_LR_X = newVM.RoofEdgeOverHang_LR_X;
            CanopyRoofEdgeOverHang_LR_X = newVM.CanopyRoofEdgeOverHang_LR_X;
            WallBottomOffset_Z = newVM.WallBottomOffset_Z;

            ConsiderRoofCladdingFor_FB_WallHeight = newVM.ConsiderRoofCladdingFor_FB_WallHeight;

            IsSetFromCode = false;
        }

        //public void GetCTS_CoilProperties(out CTS_CoilProperties prop_RoofCladdingCoil, out CTS_CoilProperties prop_WallCladdingCoil,
        //        out CoatingColour prop_RoofCladdingColor, out CoatingColour prop_WallCladdingColor)
        //{
        //    List<CTS_CoatingProperties> coatingsProperties = CTrapezoidalSheetingManager.LoadCoatingPropertiesList();

        //    CTS_CoatingProperties prop_RoofCladdingCoating = new CTS_CoatingProperties();
        //    prop_RoofCladdingCoating = CTrapezoidalSheetingManager.LoadCoatingProperties(RoofCladdingCoating);

        //    CTS_CoatingProperties prop_WallCladdingCoating = new CTS_CoatingProperties();
        //    prop_WallCladdingCoating = CTrapezoidalSheetingManager.LoadCoatingProperties(WallCladdingCoating);

        //    prop_RoofCladdingColor = RoofCladdingColors.ElementAtOrDefault(RoofCladdingColorIndex); // TODO Ondrej - pre Formclad a vyber color Zinc potrebujem vratit spravnu farbu odpovedajuce ID = 18 v databaze
        //    prop_WallCladdingColor = WallCladdingColors.ElementAtOrDefault(WallCladdingColorIndex);

        //    prop_RoofCladdingCoil = CTrapezoidalSheetingManager.GetCladdingCoilProperties(coatingsProperties.ElementAtOrDefault(RoofCladdingCoatingIndex), prop_RoofCladdingColor, RoofCladdingProps); // Ceny urcujeme podla coating a color
        //    prop_WallCladdingCoil = CTrapezoidalSheetingManager.GetCladdingCoilProperties(coatingsProperties.ElementAtOrDefault(WallCladdingCoatingIndex), prop_WallCladdingColor, WallCladdingProps); // Ceny urcujeme podla coating a color
        //}

        public void SetWallCladdingProps()
        {
            if (m_WallCladdingProps != null && m_WallCladdingProps.name == $"{WallCladding}-{WallCladdingThickness}") return;  //tieto returny treba prejst, lebo nechapem preco sa to tak vela krat vola
            m_WallCladdingProps = CTrapezoidalSheetingManager.GetSectionProperties($"{WallCladding}-{WallCladdingThickness}");
            SetCTS_CoilProperties_Wall();
        }
        public void SetRoofCladdingProps()
        {
            if (m_RoofCladdingProps != null && m_RoofCladdingProps.name == $"{RoofCladding}-{RoofCladdingThickness}") return;
            m_RoofCladdingProps = CTrapezoidalSheetingManager.GetSectionProperties($"{RoofCladding}-{RoofCladdingThickness}");
            SetCTS_CoilProperties_Roof();
        }

        public void SetCTS_CoilProperties_Wall()
        {
            if (m_WallCladdingProps == null) return;
            List<CTS_CoatingProperties> coatingsProperties = CTrapezoidalSheetingManager.LoadCoatingPropertiesList();
            CoatingColour WallCladdingColor = WallCladdingColors.ElementAtOrDefault(WallCladdingColorIndex);
            WallCladdingCoilProps = CTrapezoidalSheetingManager.GetCladdingCoilProperties(coatingsProperties.ElementAtOrDefault(WallCladdingCoatingIndex), WallCladdingColor, m_WallCladdingProps); // Ceny urcujeme podla coating a color
        }
        public void SetCTS_CoilProperties_Roof()
        {
            if (m_RoofCladdingProps == null) return;
            List<CTS_CoatingProperties> coatingsProperties = CTrapezoidalSheetingManager.LoadCoatingPropertiesList();
            CoatingColour RoofCladdingColor = RoofCladdingColors.ElementAtOrDefault(RoofCladdingColorIndex); // TODO Ondrej - pre Formclad a vyber color Zinc potrebujem vratit spravnu farbu odpovedajuce ID = 18 v databaze
            RoofCladdingCoilProps = CTrapezoidalSheetingManager.GetCladdingCoilProperties(coatingsProperties.ElementAtOrDefault(RoofCladdingCoatingIndex), RoofCladdingColor, m_RoofCladdingProps); // Ceny urcujeme podla coating a color            
        }

        public bool CollisionsExists()
        {
            bool collisionDetected = false;
            List<FibreglassProperties> noColisionItems = new List<FibreglassProperties>();
            foreach (FibreglassProperties f in FibreglassProperties)
            {
                if (noColisionItems.Exists(fp => fp.Equals(f))) { collisionDetected = true; break; }

                if (!noColisionItems.Exists(fp => fp.IsInCollisionWith(f))) noColisionItems.Add(f);
                else { collisionDetected = true; break; }
            }
            noColisionItems = null;
            return collisionDetected;
        }

        private void LoadFibreglassColors()
        {
            FibreglassColors = CCoatingColorManager.LoadColours("FibreglassSQLiteDB");
        }

        private void InitRoofFibreglassTypes()
        {
            if (RoofFibreglassThicknessIndex == 2) //2.5mm
            {
                RoofFibreglassTypes = new List<string>() { "Trafficable", "Fire rated" };
            }
            else
            {
                RoofFibreglassTypes = new List<string>() { "General" };
            }
            RoofFibreglassTypeIndex = 0;
        }
        private void InitWallFibreglassTypes()
        {
            if (WallFibreglassThicknessIndex == 2) //2.5mm
            {
                WallFibreglassTypes = new List<string>() { "Trafficable", "Fire rated" };
            }
            else
            {
                WallFibreglassTypes = new List<string>() { "General" };
            }
            WallFibreglassTypeIndex = 0;
        }

        
        private float GetTotalFibreglassAreaRoof()
        {
            float area = 0;
            foreach (FibreglassProperties fp in FibreglassProperties)
            {
                //rozmyslam,ci vsetky tieto veci nie su pomale a vsade by sme ich mali nahradit enumom, alebo indexom
                if (fp.Side == "Roof" || fp.Side == "Roof-Left Side" || fp.Side == "Roof-Right Side")
                {
                    area += fp.CladdingWidthModular_Roof * fp.Length;
                }
            }
            return area;
        }
        private float GetTotalFibreglassAreaWall()
        {
            float area = 0;
            foreach (FibreglassProperties fp in FibreglassProperties)
            {
                //rozmyslam,ci vsetky tieto veci nie su pomale a vsade by sme ich mali nahradit enumom, alebo indexom
                if (fp.Side == "Roof" || fp.Side == "Roof-Left Side" || fp.Side == "Roof-Right Side")
                {
                    continue;
                }
                area += fp.CladdingWidthModular_Wall * fp.Length;
            }
            return area;
        }

        private void SetFibreglassAreasPercentages()
        {
            if (IsSetFromCode) return;

            //mozno by bolo lepsie takto...ked to treba, tak vtedy sa to prepocita
            if (MathF.d_equal(_pfdVM.TotalRoofArea, 0)) _pfdVM.CountWallAndRoofAreas();
            if (MathF.d_equal(_pfdVM.TotalWallArea, 0)) _pfdVM.CountWallAndRoofAreas();

            FibreglassAreaRoof = GetTotalFibreglassAreaRoof() / _pfdVM.TotalRoofArea * 100;
            FibreglassAreaWall = GetTotalFibreglassAreaWall() / _pfdVM.TotalWallArea * 100;
        }

        public bool HasFibreglass()
        {
            if (FibreglassProperties == null) return false;
            else if (FibreglassProperties.Count == 0) return false;
            else return true;
        }

    }
}