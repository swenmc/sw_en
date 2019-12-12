using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using M_AS4600;
using M_EC1.AS_NZS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EXPIMP
{
    public class QuotationData
    {
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private float MGableWidth;
        private float MLength;
        private float MWallHeight;
        private float MApexHeight_H2;
        private float MRoofPitch_deg;
        private int MFrames;
        private float MGirtDistance;
        private float MPurlinDistance;
        private float MColumnDistance;
        private float MBottomGirtPosition;
        private float MBayWidth;

        private string MRoofCladding;
        private string MWallCladding;
        private string MRoofCladdingThickness_mm;
        private string MWallCladdingThickness_mm;
        private string MRoofCladdingCoating;
        private string MWallCladdingCoating;

        private string MLocation;
        private string MWindRegion;

        private int MNumberOfRollerDoors;
        private int MNumberOfPersonnelDoors;

        private CProjectInfo projectInfo;

        double m_BuildingNetPrice_WithoutMargin_WithoutGST;

        double m_MarginAbsolute;
        double m_Margin_Percentage;
        double m_BuildingPrice_WithMargin_WithoutGST;
        double m_GST_Absolute;
        double m_GST_Percentage;
        double m_TotalBuildingPrice_IncludingGST;

        float m_BuildingArea_Gross;
        float m_BuildingVolume_Gross;
        double m_BuildingMass;

        double m_BuildingPrice_PSM;
        double m_BuildingPrice_PCM;
        double m_BuildingPrice_PPKG;

        //-------------------------------------------------------------------------------------------------------------
        public float GableWidth
        {
            get
            {
                return MGableWidth;
            }
            set
            {
                MGableWidth = value;
            }
        }

        public float Length
        {
            get
            {
                return MLength;
            }

            set
            {
                MLength = value;
            }
        }

        public float WallHeight
        {
            get
            {
                return MWallHeight;
            }

            set
            {
                MWallHeight = value;
            }
        }

        public float ApexHeight_H2
        {
            get
            {
                return MApexHeight_H2;
            }

            set
            {
                MApexHeight_H2 = value;
            }
        }

        public float RoofPitch_deg
        {
            get
            {
                return MRoofPitch_deg;
            }

            set
            {
                MRoofPitch_deg = value;
            }
        }

        public int Frames
        {
            get
            {
                return MFrames;
            }

            set
            {
                MFrames = value;
            }
        }

        public float GirtDistance
        {
            get
            {
                return MGirtDistance;
            }

            set
            {
                MGirtDistance = value;
            }
        }

        public float PurlinDistance
        {
            get
            {
                return MPurlinDistance;
            }

            set
            {
                MPurlinDistance = value;
            }
        }

        public float ColumnDistance
        {
            get
            {
                return MColumnDistance;
            }

            set
            {
                MColumnDistance = value;
            }
        }

        public float BottomGirtPosition
        {
            get
            {
                return MBottomGirtPosition;
            }

            set
            {
                MBottomGirtPosition = value;
            }
        }

        public float BayWidth
        {
            get
            {
                return MBayWidth;
            }

            set
            {
                MBayWidth = value;
            }
        }

        public string RoofCladding
        {
            get
            {
                return MRoofCladding;
            }

            set
            {
                MRoofCladding = value;
            }
        }

        public string WallCladding
        {
            get
            {
                return MWallCladding;
            }

            set
            {
                MWallCladding = value;
            }
        }

        public string RoofCladdingThickness_mm
        {
            get
            {
                return MRoofCladdingThickness_mm;
            }

            set
            {
                MRoofCladdingThickness_mm = value;
            }
        }

        public string WallCladdingThickness_mm
        {
            get
            {
                return MWallCladdingThickness_mm;
            }

            set
            {
                MWallCladdingThickness_mm = value;
            }
        }

        public string RoofCladdingCoating
        {
            get
            {
                return MRoofCladdingCoating;
            }

            set
            {
                MRoofCladdingCoating = value;
            }
        }

        public string WallCladdingCoating
        {
            get
            {
                return MWallCladdingCoating;
            }

            set
            {
                MWallCladdingCoating = value;
            }
        }

        public string Location
        {
            get
            {
                return MLocation;
            }

            set
            {
                MLocation = value;
            }
        }

        public string WindRegion
        {
            get
            {
                return MWindRegion;
            }

            set
            {
                MWindRegion = value;
            }
        }

        public int NumberOfRollerDoors
        {
            get
            {
                return MNumberOfRollerDoors;
            }

            set
            {
                MNumberOfRollerDoors = value;
            }
        }

        public int NumberOfPersonnelDoors
        {
            get
            {
                return MNumberOfPersonnelDoors;
            }

            set
            {
                MNumberOfPersonnelDoors = value;
            }
        }

        public CProjectInfo ProjectInfo
        {
            get
            {
                return projectInfo;
            }

            set
            {
                projectInfo = value;
            }
        }

        public double BuildingNetPrice_WithoutMargin_WithoutGST
        {
            get
            {
                return m_BuildingNetPrice_WithoutMargin_WithoutGST;
            }

            set
            {
                m_BuildingNetPrice_WithoutMargin_WithoutGST = value;
            }
        }

        public double MarginAbsolute
        {
            get
            {
                return m_MarginAbsolute;
            }

            set
            {
                m_MarginAbsolute = value;
            }
        }

        public double Margin_Percentage
        {
            get
            {
                return m_Margin_Percentage;
            }

            set
            {
                m_Margin_Percentage = value;
            }
        }

        public double BuildingPrice_WithMargin_WithoutGST
        {
            get
            {
                return m_BuildingPrice_WithMargin_WithoutGST;
            }

            set
            {
                m_BuildingPrice_WithMargin_WithoutGST = value;
            }
        }

        public double GST_Absolute
        {
            get
            {
                return m_GST_Absolute;
            }

            set
            {
                m_GST_Absolute = value;
            }
        }

        public double GST_Percentage
        {
            get
            {
                return m_GST_Percentage;
            }

            set
            {
                m_GST_Percentage = value;
            }
        }

        public double TotalBuildingPrice_IncludingGST
        {
            get
            {
                return m_TotalBuildingPrice_IncludingGST;
            }

            set
            {
                m_TotalBuildingPrice_IncludingGST = value;
            }
        }

        public float BuildingArea_Gross
        {
            get
            {
                return m_BuildingArea_Gross;
            }

            set
            {
                m_BuildingArea_Gross = value;
            }
        }

        public float BuildingVolume_Gross
        {
            get
            {
                return m_BuildingVolume_Gross;
            }

            set
            {
                m_BuildingVolume_Gross = value;
            }
        }

        public double BuildingMass
        {
            get
            {
                return m_BuildingMass;
            }

            set
            {
                m_BuildingMass = value;
            }
        }

        public double BuildingPrice_PSM
        {
            get
            {
                return m_BuildingPrice_PSM;
            }

            set
            {
                m_BuildingPrice_PSM = value;
            }
        }

        public double BuildingPrice_PCM
        {
            get
            {
                return m_BuildingPrice_PCM;
            }

            set
            {
                m_BuildingPrice_PCM = value;
            }
        }

        public double BuildingPrice_PPKG
        {
            get
            {
                return m_BuildingPrice_PPKG;
            }

            set
            {
                m_BuildingPrice_PPKG = value;
            }
        }



        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public QuotationData()
        {
        }
    }
}
