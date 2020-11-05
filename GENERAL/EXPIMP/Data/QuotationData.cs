﻿using BaseClasses;
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
        private int MKitSetTypeIndex;
        private string MRoofShape;

        private float MWidth_Overall;
        private float MLength_Overall;
        private float MWallHeight_Overall;
        private float MApexHeight_H2_Overall;
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

        private string MRoofFibreglassThickness_mm;
        private string MWallFibreglassThickness_mm;

        private string MLocation;
        private string MWindRegion;

        private int MNumberOfRollerDoors;
        private int MNumberOfPersonnelDoors;

        private CProjectInfo projectInfo;

        double m_BuildingNetPrice_WithoutMargin_WithoutGST;

        double m_Margin_Absolute;
        double m_Margin_Percentage;
        double m_Markup_Absolute;
        double m_Markup_Percentage;
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
        public int KitSetTypeIndex
        {
            get
            {
                return MKitSetTypeIndex;
            }

            set
            {
                MKitSetTypeIndex = value;
            }
        }
        public string RoofShape
        {
            get
            {
                return MRoofShape;
            }
            set
            {
                MRoofShape = value;
            }
        }

        public float Width_Overall
        {
            get
            {
                return MWidth_Overall;
            }
            set
            {
                MWidth_Overall = value;
            }
        }

        public float Length_Overall
        {
            get
            {
                return MLength_Overall;
            }

            set
            {
                MLength_Overall = value;
            }
        }

        public float WallHeight_Overall
        {
            get
            {
                return MWallHeight_Overall;
            }

            set
            {
                MWallHeight_Overall = value;
            }
        }

        public float ApexHeight_H2_Overall
        {
            get
            {
                return MApexHeight_H2_Overall;
            }

            set
            {
                MApexHeight_H2_Overall = value;
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

        public string RoofFibreglassThickness_mm
        {
            get
            {
                return MRoofFibreglassThickness_mm;
            }

            set
            {
                MRoofFibreglassThickness_mm = value;
            }
        }

        public string WallFibreglassThickness_mm
        {
            get
            {
                return MWallFibreglassThickness_mm;
            }

            set
            {
                MWallFibreglassThickness_mm = value;
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

        public double Margin_Absolute
        {
            get
            {
                return m_Margin_Absolute;
            }

            set
            {
                m_Margin_Absolute = value;
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

        public double Markup_Absolute
        {
            get
            {
                return m_Markup_Absolute;
            }

            set
            {
                m_Markup_Absolute = value;
            }
        }

        public double Markup_Percentage
        {
            get
            {
                return m_Markup_Percentage;
            }

            set
            {
                m_Markup_Percentage = value;
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
