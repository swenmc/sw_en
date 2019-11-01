using _3DTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    // Class CSlab
    [Serializable]
    public class CSlab : CVolume
    {
        private float m_Eccentricity_x;
        private float m_Eccentricity_y;
        private float m_RotationAboutZ_deg;

        private CReinforcementBar m_Reference_Top_Bar_x;
        private CReinforcementBar m_Reference_Top_Bar_y;
        private CReinforcementBar m_Reference_Bottom_Bar_x;
        private CReinforcementBar m_Reference_Bottom_Bar_y;

        private List<CReinforcementBar> m_Top_Bars_x;
        private List<CReinforcementBar> m_Top_Bars_y;
        private List<CReinforcementBar> m_Bottom_Bars_x;
        private List<CReinforcementBar> m_Bottom_Bars_y;

        private int m_Count_Top_Bars_x;
        private int m_Count_Top_Bars_y;
        private int m_Count_Bottom_Bars_x;
        private int m_Count_Bottom_Bars_y;

        private float m_fDistanceOfBars_Top_x_SpacingInyDirection;
        private float m_fDistanceOfBars_Top_y_SpacingInxDirection;
        private float m_fDistanceOfBars_Bottom_x_SpacingInyDirection;
        private float m_fDistanceOfBars_Bottom_y_SpacingInxDirection;

        private float m_fConcreteCover;

        private string m_sMeshGradeName;

        private int m_NumberOfSawCutsInDirectionX;
        private int m_NumberOfSawCutsInDirectionY;
        private float m_FirstSawCutPositionInDirectionX;
        private float m_FirstSawCutPositionInDirectionY;
        private float m_SawCutsSpacingInDirectionX;
        private float m_SawCutsSpacingInDirectionY;
        private CSawCut m_ReferenceSawCut;
        private List<CSawCut> m_SawCuts;

        private int m_NumberOfControlJointsInDirectionX;
        private int m_NumberOfControlJointsInDirectionY;
        private float m_FirstControlJointPositionInDirectionX;
        private float m_FirstControlJointPositionInDirectionY;
        private float m_ControlJointsSpacingInDirectionX;
        private float m_ControlJointsSpacingInDirectionY;
        private CControlJoint m_ReferenceControlJoint;
        private List<CControlJoint> m_ControlJoints;

        private float m_PerimeterDepth_LRSide;
        private float m_PerimeterWidth_LRSide;
        private float m_StartersLapLength_LRSide;
        private float m_StartersSpacing_LRSide;
        private float m_Starters_Phi_LRSide;
        private float m_Longitud_Reinf_TopAndBotom_Phi_LRSide;
        private float m_Longitud_Reinf_Intermediate_Phi_LRSide;
        private int m_Longitud_Reinf_Intermediate_Count_LRSide;

        private float m_RebateWidth_LRSide;

        private float m_PerimeterDepth_FBSide;
        private float m_PerimeterWidth_FBSide;
        private float m_StartersLapLength_FBSide;
        private float m_StartersSpacing_FBSide;
        private float m_Starters_Phi_FBSide;
        private float m_Longitud_Reinf_TopAndBotom_Phi_FBSide;
        private float m_Longitud_Reinf_Intermediate_Phi_FBSide;
        private int m_Longitud_Reinf_Intermediate_Count_FBSide;

        private float m_RebateWidth_FBSide;

        private List<CSlabPerimeter> m_PerimeterBeams;

        private ObservableCollection<DoorProperties> m_DoorBlocksProperties;

        private List<Point3D> MWireFramePoints;

        public float Eccentricity_x
        {
            get
            {
                return m_Eccentricity_x;
            }

            set
            {
                m_Eccentricity_x = value;
            }
        }

        public float Eccentricity_y
        {
            get
            {
                return m_Eccentricity_y;
            }

            set
            {
                m_Eccentricity_y = value;
            }
        }

        public float RotationAboutZ_deg
        {
            get
            {
                return m_RotationAboutZ_deg;
            }

            set
            {
                m_RotationAboutZ_deg = value;
            }
        }

        public CReinforcementBar Reference_Top_Bar_x
        {
            get
            {
                return m_Reference_Top_Bar_x;
            }

            set
            {
                m_Reference_Top_Bar_x = value;
            }
        }

        public CReinforcementBar Reference_Top_Bar_y
        {
            get
            {
                return m_Reference_Top_Bar_y;
            }

            set
            {
                m_Reference_Top_Bar_y = value;
            }
        }

        public CReinforcementBar Reference_Bottom_Bar_x
        {
            get
            {
                return m_Reference_Bottom_Bar_x;
            }

            set
            {
                m_Reference_Bottom_Bar_x = value;
            }
        }

        public CReinforcementBar Reference_Bottom_Bar_y
        {
            get
            {
                return m_Reference_Bottom_Bar_y;
            }

            set
            {
                m_Reference_Bottom_Bar_y = value;
            }
        }

        public List<CReinforcementBar> Top_Bars_x
        {
            get
            {
                return m_Top_Bars_x;
            }

            set
            {
                m_Top_Bars_x = value;
            }
        }

        public List<CReinforcementBar> Top_Bars_y
        {
            get
            {
                return m_Top_Bars_y;
            }

            set
            {
                m_Top_Bars_y = value;
            }
        }

        public List<CReinforcementBar> Bottom_Bars_x
        {
            get
            {
                return m_Bottom_Bars_x;
            }

            set
            {
                m_Bottom_Bars_x = value;
            }
        }

        public List<CReinforcementBar> Bottom_Bars_y
        {
            get
            {
                return m_Bottom_Bars_y;
            }

            set
            {
                m_Bottom_Bars_y = value;
            }
        }

        public int Count_Top_Bars_x
        {
            get
            {
                return m_Count_Top_Bars_x;
            }

            set
            {
                m_Count_Top_Bars_x = value;
            }
        }

        public int Count_Top_Bars_y
        {
            get
            {
                return m_Count_Top_Bars_y;
            }

            set
            {
                m_Count_Top_Bars_y = value;
            }
        }

        public int Count_Bottom_Bars_x
        {
            get
            {
                return m_Count_Bottom_Bars_x;
            }

            set
            {
                m_Count_Bottom_Bars_x = value;
            }
        }

        public int Count_Bottom_Bars_y
        {
            get
            {
                return m_Count_Bottom_Bars_y;
            }

            set
            {
                m_Count_Bottom_Bars_y = value;
            }
        }

        public float DistanceOfBars_Top_x_SpacingInyDirection
        {
            get
            {
                return m_fDistanceOfBars_Top_x_SpacingInyDirection;
            }

            set
            {
                m_fDistanceOfBars_Top_x_SpacingInyDirection = value;
            }
        }

        public float DistanceOfBars_Top_y_SpacingInxDirection
        {
            get
            {
                return m_fDistanceOfBars_Top_y_SpacingInxDirection;
            }

            set
            {
                m_fDistanceOfBars_Top_y_SpacingInxDirection = value;
            }
        }

        public float DistanceOfBars_Bottom_x_SpacingInyDirection
        {
            get
            {
                return m_fDistanceOfBars_Bottom_x_SpacingInyDirection;
            }

            set
            {
                m_fDistanceOfBars_Bottom_x_SpacingInyDirection = value;
            }
        }

        public float DistanceOfBars_Bottom_y_SpacingInxDirection
        {
            get
            {
                return m_fDistanceOfBars_Bottom_y_SpacingInxDirection;
            }

            set
            {
                m_fDistanceOfBars_Bottom_y_SpacingInxDirection = value;
            }
        }

        public float ConcreteCover
        {
            get
            {
                return m_fConcreteCover;
            }

            set
            {
                m_fConcreteCover = value;
            }
        }

        public string MeshGradeName
        {
            get { return m_sMeshGradeName; }
            set { m_sMeshGradeName = value; }
        }

        public List<Point3D> WireFramePoints
        {
            get
            {
                if (MWireFramePoints == null) MWireFramePoints = new List<Point3D>();
                return MWireFramePoints;
            }

            set
            {
                MWireFramePoints = value;
            }
        }

        private Point3D m_PointText;

        public Point3D PointText
        {
            get { return m_PointText; }
            set { m_PointText = value; }
        }

        private string m_Text;

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        public int NumberOfSawCutsInDirectionX
        {
            get { return m_NumberOfSawCutsInDirectionX; }
            set { m_NumberOfSawCutsInDirectionX = value; }
        }

        public int NumberOfSawCutsInDirectionY
        {
            get { return m_NumberOfSawCutsInDirectionY; }
            set { m_NumberOfSawCutsInDirectionY = value; }
        }

        public float FirstSawCutPositionInDirectionX
        {
            get { return m_FirstSawCutPositionInDirectionX; }
            set { m_FirstSawCutPositionInDirectionX = value; }
        }

        public float FirstSawCutPositionInDirectionY
        {
            get { return m_FirstSawCutPositionInDirectionY; }
            set { m_FirstSawCutPositionInDirectionY = value; }
        }

        public float SawCutsSpacingInDirectionX
        {
            get { return m_SawCutsSpacingInDirectionX; }
            set { m_SawCutsSpacingInDirectionX = value; }
        }

        public float SawCutsSpacingInDirectionY
        {
            get { return m_SawCutsSpacingInDirectionY; }
            set { m_SawCutsSpacingInDirectionY = value; }
        }

        public CSawCut ReferenceSawCut
        {
            get { return m_ReferenceSawCut; }
            set { m_ReferenceSawCut = value; }
        }

        public List<CSawCut> SawCuts
        {
            get { return m_SawCuts; }
            set { m_SawCuts = value; }
        }

        public int NumberOfControlJointsInDirectionX
        {
            get { return m_NumberOfControlJointsInDirectionX; }
            set { m_NumberOfControlJointsInDirectionX = value; }
        }

        public int NumberOfControlJointsInDirectionY
        {
            get { return m_NumberOfControlJointsInDirectionY; }
            set { m_NumberOfControlJointsInDirectionY = value; }
        }

        public float FirstControlJointPositionInDirectionX
        {
            get { return m_FirstControlJointPositionInDirectionX; }
            set { m_FirstControlJointPositionInDirectionX = value; }
        }

        public float FirstControlJointPositionInDirectionY
        {
            get { return m_FirstControlJointPositionInDirectionY; }
            set { m_FirstControlJointPositionInDirectionY = value; }
        }

        public float ControlJointsSpacingInDirectionX
        {
            get { return m_ControlJointsSpacingInDirectionX; }
            set { m_ControlJointsSpacingInDirectionX = value; }
        }

        public float ControlJointsSpacingInDirectionY
        {
            get { return m_ControlJointsSpacingInDirectionY; }
            set { m_ControlJointsSpacingInDirectionY = value; }
        }

        public CControlJoint ReferenceControlJoint
        {
            get { return m_ReferenceControlJoint; }
            set { m_ReferenceControlJoint = value; }
        }

        public List<CControlJoint> ControlJoints
        {
            get { return m_ControlJoints; }
            set { m_ControlJoints = value; }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterDepth_LRSide
        {
            get
            {
                return m_PerimeterDepth_LRSide;
            }

            set
            {
                m_PerimeterDepth_LRSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterWidth_LRSide
        {
            get
            {
                return m_PerimeterWidth_LRSide;
            }

            set
            {
                m_PerimeterWidth_LRSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersLapLength_LRSide
        {
            get
            {
                return m_StartersLapLength_LRSide;
            }

            set
            {
                m_StartersLapLength_LRSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersSpacing_LRSide
        {
            get
            {
                return m_StartersSpacing_LRSide;
            }

            set
            {
                m_StartersSpacing_LRSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Starters_Phi_LRSide
        {
            get
            {
                return m_Starters_Phi_LRSide;
            }

            set
            {
                m_Starters_Phi_LRSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Longitud_Reinf_TopAndBotom_Phi_LRSide
        {
            get
            {
                return m_Longitud_Reinf_TopAndBotom_Phi_LRSide;
            }

            set
            {
                m_Longitud_Reinf_TopAndBotom_Phi_LRSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Longitud_Reinf_Intermediate_Phi_LRSide
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Phi_LRSide;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Phi_LRSide = value;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        public int Longitud_Reinf_Intermediate_Count_LRSide
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Count_LRSide;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Count_LRSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateWidth_LRSide
        {
            get
            {
                return m_RebateWidth_LRSide;
            }

            set
            {
                m_RebateWidth_LRSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterDepth_FBSide
        {
            get
            {
                return m_PerimeterDepth_FBSide;
            }

            set
            {
                m_PerimeterDepth_FBSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterWidth_FBSide
        {
            get
            {
                return m_PerimeterWidth_FBSide;
            }

            set
            {
                m_PerimeterWidth_FBSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersLapLength_FBSide
        {
            get
            {
                return m_StartersLapLength_FBSide;
            }

            set
            {
                m_StartersLapLength_FBSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersSpacing_FBSide
        {
            get
            {
                return m_StartersSpacing_FBSide;
            }

            set
            {
                m_StartersSpacing_FBSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Starters_Phi_FBSide
        {
            get
            {
                return m_Starters_Phi_FBSide;
            }

            set
            {
                m_Starters_Phi_FBSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Longitud_Reinf_TopAndBotom_Phi_FBSide
        {
            get
            {
                return m_Longitud_Reinf_TopAndBotom_Phi_FBSide;
            }

            set
            {
                m_Longitud_Reinf_TopAndBotom_Phi_FBSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Longitud_Reinf_Intermediate_Phi_FBSide
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Phi_FBSide;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Phi_FBSide = value;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        public int Longitud_Reinf_Intermediate_Count_FBSide
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Count_FBSide;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Count_FBSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateWidth_FBSide
        {
            get
            {
                return m_RebateWidth_FBSide;
            }

            set
            {
                m_RebateWidth_FBSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<CSlabPerimeter> PerimeterBeams
        {
            get
            {
                return m_PerimeterBeams;
            }

            set
            {
                m_PerimeterBeams = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public ObservableCollection<DoorProperties> DoorBlocksProperties
        {
            get
            {
                return m_DoorBlocksProperties;
            }

            set
            {
                m_DoorBlocksProperties = value;
            }
        }

        // DOCASNE
        // TODO Ondrej - tieto parametre sa mi nepacia, sluzia na vypocet polohy zaciatku rebate a dlzky rebate, malo by to podla mna prist do CSlab uz nejako v ramci door properties
        //float m_fL1_frame; // Vzdialenost ramov
        //float m_fDist_FrontColumns; // Vzdialenost wind posts (stlpov v prednej stene)
        //float m_fDist_BackColumns; // Vzdialenost wind posts (stlpov v zadnej stene)
        //float m_fTrimmerWidth; // Sirka cross-section typu roller door trimmer

        private float fTolerance = 0.0001f; // Tolerance - 3D graphics collision (doska o 0.1 mm nizsie nez stlpy aby bolo vidno ich obrys

        public CSlab()
        {
        }

        // Rectangular
        public CSlab(int iSlab_ID,
            MATERIAL.CMat_02_00 materialConcrete,
            float fX,
            float fY,
            float fZ,
            float ex,
            float ey,
            float rotationAboiutZInDeg,
            float fConcreteCover,
            string sMeshGradeName,
            int   iNumberOfSawCutsInDirectionX,
            int   iNumberOfSawCutsInDirectionY,
            float fFirstSawCutPositionInDirectionX,
            float fFirstSawCutPositionInDirectionY,
            float fSawCutsSpacingInDirectionX,
            float fSawCutsSpacingInDirectionY,
            CSawCut refSawCut,
            int   iNumberOfControlJointsInDirectionX,
            int   iNumberOfControlJointsInDirectionY,
            float fFirstControlJointPositionInDirectionX,
            float fFirstControlJointPositionInDirectionY,
            float fControlJointsSpacingInDirectionX,
            float fControlJointsSpacingInDirectionY,
            CControlJoint refControlJoint,
            float fPerimeterDepth_LRSide,
            float fPerimeterWidth_LRSide,
            float fStartersLapLength_LRSide,
            float fStartersSpacing_LRSide,
            float fStarters_Phi_LRSide,
            float fLongitud_Reinf_TopAndBotom_Phi_LRSide,
            float fLongitud_Reinf_Intermediate_Phi_LRSide,
            int   iLongitud_Reinf_Intermediate_Count_LRSide,
            float fRebateWidth_LRSide,
            float fPerimeterDepth_FBSide,
            float fPerimeterWidth_FBSide,
            float fStartersLapLength_FBSide,
            float fStartersSpacing_FBSide,
            float fStarters_Phi_FBSide,
            float fLongitud_Reinf_TopAndBotom_Phi_FBSide,
            float fLongitud_Reinf_Intermediate_Phi_FBSide,
            int   iLongitud_Reinf_Intermediate_Count_FBSide,
            float fRebateWidth_FBSide,
            ObservableCollection<DoorProperties> doorBlocksProperties,

            // TODO Ondrej - tieto parametre sa mi nepacia, sluzia na vypocet polohy zaciatku rebate a dlzky rebate, malo by to podla mna prist do CSlab uz nejako v ramci door properties
            //float fL1_frame, // Vzdialenost ramov
            //float fDist_FrontColumns, // Vzdialenost wind posts (stlpov v prednej stene)
            //float fDist_BackColumns, // Vzdialenost wind posts (stlpov v zadnej stene)
            //float fTrimmerWidth, // Sirka cross-section typu roller door trimmer

        //CReinforcementBar refTopBar_x,
        //CReinforcementBar refTopBar_y,
        //CReinforcementBar refBottomBar_x,
        //CReinforcementBar refBottomBar_y,
        //int iNumberOfBarsTop_x,
        //int iNumberOfBarsTop_y,
        //int iNumberOfBarsBottom_x,
        //int iNumberOfBarsBottom_y,
        //Color volColor,
        float fvolOpacity,
            bool bIsDisplayed,
            float fTime)
        {
            ID = iSlab_ID;
            m_Mat = materialConcrete;
            m_fDim1 = fX; // Width
            m_fDim2 = fY; // Length
            m_fDim3 = fZ;
            m_Eccentricity_x = ex;
            m_Eccentricity_y = ey;
            m_RotationAboutZ_deg = rotationAboiutZInDeg;
            m_fConcreteCover = fConcreteCover;
            m_sMeshGradeName = sMeshGradeName;
            m_NumberOfSawCutsInDirectionX = iNumberOfSawCutsInDirectionX;
            m_NumberOfSawCutsInDirectionY = iNumberOfSawCutsInDirectionY;
            m_FirstSawCutPositionInDirectionX = fFirstSawCutPositionInDirectionX;
            m_FirstSawCutPositionInDirectionY = fFirstSawCutPositionInDirectionY;
            m_SawCutsSpacingInDirectionX = fSawCutsSpacingInDirectionX;
            m_SawCutsSpacingInDirectionY = fSawCutsSpacingInDirectionY;
            m_ReferenceSawCut = refSawCut;
            m_NumberOfControlJointsInDirectionX = iNumberOfControlJointsInDirectionX;
            m_NumberOfControlJointsInDirectionY = iNumberOfControlJointsInDirectionY;
            m_FirstControlJointPositionInDirectionX = fFirstControlJointPositionInDirectionX;
            m_FirstControlJointPositionInDirectionY = fFirstControlJointPositionInDirectionY;
            m_ControlJointsSpacingInDirectionX = fControlJointsSpacingInDirectionX;
            m_ControlJointsSpacingInDirectionY = fControlJointsSpacingInDirectionY;
            m_ReferenceControlJoint = refControlJoint;

            m_PerimeterDepth_LRSide = fPerimeterDepth_LRSide;
            m_PerimeterWidth_LRSide = fPerimeterWidth_LRSide;
            m_StartersLapLength_LRSide = fStartersLapLength_LRSide;
            m_StartersSpacing_LRSide = fStartersSpacing_LRSide;
            m_Starters_Phi_LRSide = fStarters_Phi_LRSide;
            m_Longitud_Reinf_TopAndBotom_Phi_LRSide = fLongitud_Reinf_TopAndBotom_Phi_LRSide;
            m_Longitud_Reinf_Intermediate_Phi_LRSide = fLongitud_Reinf_Intermediate_Phi_LRSide;
            m_Longitud_Reinf_Intermediate_Count_LRSide = iLongitud_Reinf_Intermediate_Count_LRSide;

            m_RebateWidth_LRSide = fRebateWidth_LRSide;

            m_PerimeterDepth_FBSide = fPerimeterDepth_FBSide;
            m_PerimeterWidth_FBSide = fPerimeterWidth_FBSide;
            m_StartersLapLength_FBSide = fStartersLapLength_FBSide;
            m_StartersSpacing_FBSide = fStartersSpacing_FBSide;
            m_Starters_Phi_FBSide = fStarters_Phi_FBSide;
            m_Longitud_Reinf_TopAndBotom_Phi_FBSide = fLongitud_Reinf_TopAndBotom_Phi_FBSide;
            m_Longitud_Reinf_Intermediate_Phi_FBSide = fLongitud_Reinf_Intermediate_Phi_FBSide;
            m_Longitud_Reinf_Intermediate_Count_FBSide = iLongitud_Reinf_Intermediate_Count_FBSide;

            m_RebateWidth_FBSide = fRebateWidth_FBSide;

            m_DoorBlocksProperties = doorBlocksProperties;

            // TODO Ondrej - tieto parametre sa mi nepacia, sluzia na vypocet polohy zaciatku rebate a dlzky rebate, malo by to podla mna prist do CSlab uz nejako v ramci door properties
            //m_fL1_frame = fL1_frame; // Vzdialenost ramov
            //m_fDist_FrontColumns = fDist_FrontColumns; // Vzdialenost wind posts (stlpov v prednej stene)
            //m_fDist_BackColumns = fDist_BackColumns; // Vzdialenost wind posts (stlpov v zadnej stene)
            //m_fTrimmerWidth = fTrimmerWidth; // Sirka cross-section typu roller door trimmer

            //m_Reference_Top_Bar_x = refTopBar_x;
            //m_Reference_Top_Bar_y = refTopBar_y;
            //m_Reference_Bottom_Bar_x = refBottomBar_x;
            //m_Reference_Bottom_Bar_y = refBottomBar_y;
            //m_Count_Top_Bars_x = iNumberOfBarsTop_x;
            //m_Count_Top_Bars_y = iNumberOfBarsTop_y;
            //m_Count_Bottom_Bars_x = iNumberOfBarsBottom_x;
            //m_Count_Bottom_Bars_y = iNumberOfBarsBottom_y;
            //m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            SetControlPoint();
            SetTextPoint();
            CreateSawCuts();
            CreateControlJoints();
            CreatePerimeters();
            SetDescriptionText();
        }

        public void SetTextPoint()
        {
            // V systeme GCS
            float fAdditionalOffsetX = 0.6f;
            float fAdditionalOffsetY = 0.7f; // TODO Ondrej - Toto by bolo super vediet nastavit podla polohy saw cut a control joint aby sa neprekryvali texty
            float fOffsetX = 0.5f * m_fDim1 + fAdditionalOffsetX;
            float fOffsetY = 0.5f * m_fDim2 + fAdditionalOffsetY;
            float fOffsetFromPlane = m_fDim3 + 0.005f; // Offset nad urovnou podlahy aby sa text nevnoril do jej 3D reprezentacie

            m_PointText = new Point3D()
            {
                X = fOffsetX,
                Y = fOffsetY,
                Z = fOffsetFromPlane
            };
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(Color color, float fOpacity)
        {
            m_volColor_2 = color;
            Visual_Object = CreateGeomModel3D(new SolidColorBrush(m_volColor_2), fOpacity);
            return Visual_Object;
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(SolidColorBrush brush, float fOpacity)
        {
            brush.Opacity = fOpacity; // Set brush opacity // TODO - mozeme nejako vylepsit a prepojit s GUI, aby to bol piamo parameter zadavany spolu s farbou zakladu

            GeometryModel3D model = new GeometryModel3D();

            // TODO - pohrat sa s nastavenim farieb
            DiffuseMaterial qDiffTrans = new DiffuseMaterial(brush);
            SpecularMaterial qSpecTrans = new SpecularMaterial(brush, 90.0);

            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTrans);
            qOuterMaterial.Children.Add(qSpecTrans);

            // Create slab - origin [0,0,0]
            CVolume volume = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, new Point3D(0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial, true, 0);
            model = volume.CreateM_3D_G_Volume_8Edges(new Point3D(0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial);

            // Set the Transform property of the GeometryModel to the Transform3DGroup
            // Nastavim vyslednu transformaciu
            model.Transform = GetSlabTransformGroup();

            // Naplnime pole bodov wireFrame
            // TODO - Ondrej - chcelo by to nejako elegantne zjednotit u vsetkych objektov ktore maju 3D geometriu kde a ako ziskavat wireframe
            // TODO Ondrej - tu chyba v tom ze beriem pozicie z povodneho zakladu nie z posunuteho do finalnej pozicie
            WireFramePoints = GetWireFramePoints_Volume(model);

            Visual_Object = model;

            return model;
        }

        public Transform3DGroup GetSlabTransformGroup()
        {
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            // Rotate about Z axis - otocime patku okolo [0,0,0]
            // Create and apply a transformation that rotates the object.
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = new Vector3D(0, 0, 1);
            myAxisAngleRotation3d.Angle = RotationAboutZ_deg;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;

            // Add the rotation transform to a Transform3DGroup
            myTransform3DGroup.Children.Add(myRotateTransform3D);

            // Presun celej dosky do GCS z [0,0,0] do control point
            // Create and apply translation
            TranslateTransform3D myTranslateTransform3D_GCS = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            // Add the translation transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myTranslateTransform3D_GCS);

            return myTransform3DGroup;
        }

        public void CreateSawCuts()
        {
            bool bGenerateSawCuts = true;

            if (bGenerateSawCuts)
            {
                SawCuts = new List<CSawCut>();

                // Sawcuts per X axis - rezanie v smere Y
                for (int i = 0; i < NumberOfSawCutsInDirectionX; i++)
                {
                    double coordX = m_pControlPoint.X;
                    double coordStartY = m_pControlPoint.Y;
                    double coordEndY = m_pControlPoint.Y + m_fDim2;
                    double coordZ = fTolerance; // Nad hornou stranou plochy

                    if (i == 0) // First
                    {
                        coordX = m_pControlPoint.X + FirstSawCutPositionInDirectionX;
                        SawCuts.Add(new CSawCut(i + 1, new Point3D(coordX, coordStartY, coordZ), new Point3D(coordX, coordEndY, coordZ), m_ReferenceSawCut.CutWidth, m_ReferenceSawCut.CutDepth, true, 0));
                    }
                    else
                    {
                        coordX = m_pControlPoint.X + FirstSawCutPositionInDirectionX + i * SawCutsSpacingInDirectionX;
                        SawCuts.Add(new CSawCut(i + 1, new Point3D(coordX, coordStartY, coordZ), new Point3D(coordX, coordEndY, coordZ), m_ReferenceSawCut.CutWidth, m_ReferenceSawCut.CutDepth, true, 0));
                    }
                }
                // Sawcuts per Y axis - rezanie v smere X
                for (int i = 0; i < NumberOfSawCutsInDirectionY; i++)
                {
                    double coordStartX = m_pControlPoint.X;
                    double coordEndX = m_pControlPoint.X + m_fDim1;
                    double coordY = m_pControlPoint.Y;
                    double coordZ = fTolerance; // Nad hornou stranou plochy

                    if (i == 0) // First
                    {
                        coordY = m_pControlPoint.Y + FirstSawCutPositionInDirectionY;
                        SawCuts.Add(new CSawCut(NumberOfSawCutsInDirectionX + i + 1, new Point3D(coordStartX, coordY, coordZ), new Point3D(coordEndX, coordY, coordZ), m_ReferenceSawCut.CutWidth, m_ReferenceSawCut.CutDepth, true, 0));
                    }
                    else
                    {
                        coordY = m_pControlPoint.Y + FirstSawCutPositionInDirectionY + i * SawCutsSpacingInDirectionY;
                        SawCuts.Add(new CSawCut(NumberOfSawCutsInDirectionX + i + 1, new Point3D(coordStartX, coordY, coordZ), new Point3D(coordEndX, coordY, coordZ), m_ReferenceSawCut.CutWidth, m_ReferenceSawCut.CutDepth, true, 0));
                    }
                }
            }
        }

        public void CreateControlJoints()
        {
            bool bGenerateControlJoints = true;

            if (bGenerateControlJoints)
            {
                //Diameters available = 10, 12, 16, 20, 25, 32, 40
                /*
                12mm x 460mm Galvanised Dowel
                16mm x 400mm Galvanised Dowel
                16mm x 600mm Galvanised Dowel
                20mm x 400mm Galvanised Dowel
                20mm x 600mm Galvanised Dowel
                33mm x 450mm Galvanised Dowel
                */

                // Create raster of lines in XY-plane
                ControlJoints = new List<CControlJoint>();

                // ControlJoints per X axis
                for (int i = 0; i < NumberOfControlJointsInDirectionX; i++)
                {
                    double coordX = m_pControlPoint.X;
                    double coordStartY = m_pControlPoint.Y;
                    double coordEndY = m_pControlPoint.Y + m_fDim2;
                    double coordZ = fTolerance; // Nad hornou stranou plochy

                    if (i == 0) // First
                    {
                        coordX = m_pControlPoint.X + FirstControlJointPositionInDirectionX;
                        ControlJoints.Add(new CControlJoint(i + 1, new Point3D(coordX, coordStartY, coordZ), new Point3D(coordX, coordEndY, coordZ), m_ReferenceControlJoint.ReferenceDowel, m_ReferenceControlJoint.DowelSpacing, true, 0));
                    }
                    else
                    {
                        coordX = m_pControlPoint.X + FirstControlJointPositionInDirectionX + i * ControlJointsSpacingInDirectionX;
                        ControlJoints.Add(new CControlJoint(i + 1, new Point3D(coordX, coordStartY, coordZ), new Point3D(coordX, coordEndY, coordZ), m_ReferenceControlJoint.ReferenceDowel, m_ReferenceControlJoint.DowelSpacing, true, 0));
                    }
                }
                // ControlJoints per Y axis
                for (int i = 0; i < NumberOfControlJointsInDirectionY; i++)
                {
                    double coordStartX = m_pControlPoint.X;
                    double coordEndX = m_pControlPoint.X + m_fDim1;
                    double coordY = m_pControlPoint.Y;
                    double coordZ = fTolerance; // Nad hornou stranou plochy

                    if (i == 0) // First
                    {
                        coordY = m_pControlPoint.Y + FirstControlJointPositionInDirectionY;
                        ControlJoints.Add(new CControlJoint(NumberOfControlJointsInDirectionX + i + 1, new Point3D(coordStartX, coordY, coordZ), new Point3D(coordEndX, coordY, coordZ), m_ReferenceControlJoint.ReferenceDowel, m_ReferenceControlJoint.DowelSpacing, true, 0));
                    }
                    else
                    {
                        coordY = m_pControlPoint.Y + FirstControlJointPositionInDirectionY + i * ControlJointsSpacingInDirectionY;
                        ControlJoints.Add(new CControlJoint(NumberOfControlJointsInDirectionX + i + 1, new Point3D(coordStartX, coordY, coordZ), new Point3D(coordEndX, coordY, coordZ), m_ReferenceControlJoint.ReferenceDowel, m_ReferenceControlJoint.DowelSpacing, true, 0));
                    }
                }
            }
        }

        public void CreatePerimeters()
        {
            m_PerimeterBeams = new List<CSlabPerimeter>(); // Jeden zakladovy prvok na kazdej strane podlahy

            // TODO - potrebujeme sem dostat vsetky vytvorene roller doors, stranu na ktorej sa nachadzaju, ich pozicie na strane, rozmer roller door trimmer
            // z toho urcime cistu sirku dveri a zaciatok floor slab rebate (vyrez do dosky v mieste dveri)
            // TODO - vytvorime list rebates, ktory priradime CSlabPerimeter ako posledny parameters
            // TODO - potrebujeme se dostat vysku zakladovych patiek na kazdej strane budovy aby sme nastavili default, resp. perimeter depth z GUI
            // Vytvorime objekty a zoznam rebates podla toho kde sa nachadzaju roller doors a tieto rebates posleme ako zoznam do konstruktora CSlabPerimeter

            List<CSlabRebate> rebatesLeftSide = null;
            List<CSlabRebate> rebatesRightSide = null;
            List<CSlabRebate> rebatesFrontSide = null;
            List<CSlabRebate> rebatesBackSide = null;

            if (m_DoorBlocksProperties != null && m_DoorBlocksProperties.Count > 0) // Some door exists
            {
                for (int i = 0; i < m_DoorBlocksProperties.Count; i++)
                {
                    DoorProperties doorProp = m_DoorBlocksProperties[i];

                    if (doorProp.sDoorType == "Roller Door")
                    {
                        // TODO Ondrej
                        // napojit nejako elegantne premenne fL1_frame, ... - su v CModel_PFD_01_GR
                        // Trosku sa mi nepaci ze doska ma mat take parametre
                        // Najlepsie by bolo keby Slab dostala uz rovno zoznam doors typu roller door (upraveny m_DoorBlocksProperties) a v ramci toho uz aj ich aj absolutnu poziciu (na strane budovy) , poziciu rebates a dlzku rebates
                        // podla jednotlivych rebates, ktore su pre tieto doors potrebne
                        // podla mna by do dosky nemalo ist ako parameter nieco ako sirka stlpa dveri

                        //float fL1_frame = 3f; // Vzdialenost ramov
                        //float fColumnDistance = 2f; // Vzdialenost wind posts (stlpov v prednej alebo zadnej stene)
                        //float fTrimmerWidth = 0.14f; // Sirka cross-section typu roller door trimmer

                        // TODO Ondrej
                        // Este potrebujem urcit vkladaci bod rebate v 3D
                        // Rozmery budovy W a L by sa sem dali dostat ako parametre ale nechcem ich pridavat do slab, takze ich spocitam z rozmerov dosky a excentricit
                        float fW_frame = m_fDim1 + 2 * m_Eccentricity_x;
                        float fL_tot = m_fDim2 + 2 * m_Eccentricity_y;

                        float fSlabCoordinateMax_X = -Eccentricity_x + fW_frame;
                        float fSlabCoordinateMax_Y = -Eccentricity_y + fL_tot;

                        // TODO Ondrej - Refaktorovat - 4 strany
                        if (doorProp.sBuildingSide == "Left")
                        {
                            if (rebatesLeftSide == null) rebatesLeftSide = new List<CSlabRebate>(); // Ak zoznam nie je inicializovany, tak ho inicializujeme a pridame rebates pre vsetky roller doors na danej strane budovy
                                rebatesLeftSide.Add(new CSlabRebate(i + 1, doorProp.RebateProp, -90, new Point3D (Eccentricity_x , doorProp.RebateProp.RebatePosition + doorProp.RebateProp.RebateLength, /*- doorProp.RebateProp.RebateDepth_Edge +*/ 0.001f), true, 0));
                        }

                        if (doorProp.sBuildingSide == "Right")
                        {
                            if (rebatesRightSide == null) rebatesRightSide = new List<CSlabRebate>(); // Ak zoznam nie je inicializovany, tak ho inicializujeme a pridame rebates pre vsetky roller doors na danej strane budovy
                            rebatesRightSide.Add(new CSlabRebate(i + 1, doorProp.RebateProp, 90, new Point3D(fSlabCoordinateMax_X, doorProp.RebateProp.RebatePosition, /*- doorProp.RebateProp.RebateDepth_Edge +*/ 0.001f),true, 0));
                        }

                        if (doorProp.sBuildingSide == "Front")
                        {
                            if (rebatesFrontSide == null) rebatesFrontSide = new List<CSlabRebate>(); // Ak zoznam nie je inicializovany, tak ho inicializujeme a pridame rebates pre vsetky roller doors na danej strane budovy
                            rebatesFrontSide.Add(new CSlabRebate(i + 1, doorProp.RebateProp, 0, new Point3D(doorProp.RebateProp.RebatePosition, Eccentricity_y, /*-doorProp.RebateProp.RebateDepth_Edge +*/ 0.001f),true, 0));
                        }

                        if (doorProp.sBuildingSide == "Back")
                        {
                            if (rebatesBackSide == null) rebatesBackSide = new List<CSlabRebate>(); // Ak zoznam nie je inicializovany, tak ho inicializujeme a pridame rebates pre vsetky roller doors na danej strane budovy
                            rebatesBackSide.Add(new CSlabRebate(i + 1, doorProp.RebateProp, 180, new Point3D(doorProp.RebateProp.RebatePosition + doorProp.RebateProp.RebateLength, fSlabCoordinateMax_Y, /*-doorProp.RebateProp.RebateDepth_Edge +*/ 0.001f), true, 0));
                        }
                    }
                }
            }

            m_PerimeterBeams.Add(new CSlabPerimeter(1,
                "Left",
                m_PerimeterDepth_LRSide,
                m_PerimeterWidth_LRSide,
                m_StartersLapLength_LRSide,
                m_StartersSpacing_LRSide,
                m_Starters_Phi_LRSide,
                m_Longitud_Reinf_TopAndBotom_Phi_LRSide,
                m_Longitud_Reinf_Intermediate_Phi_LRSide,
                m_Longitud_Reinf_Intermediate_Count_LRSide,
                true,
                0,
                rebatesLeftSide));

            m_PerimeterBeams.Add(new CSlabPerimeter(2,
                "Right",
                m_PerimeterDepth_LRSide,
                m_PerimeterWidth_LRSide,
                m_StartersLapLength_LRSide,
                m_StartersSpacing_LRSide,
                m_Starters_Phi_LRSide,
                m_Longitud_Reinf_TopAndBotom_Phi_LRSide,
                m_Longitud_Reinf_Intermediate_Phi_LRSide,
                m_Longitud_Reinf_Intermediate_Count_LRSide,
                true,
                0,
                rebatesRightSide));

            m_PerimeterBeams.Add(new CSlabPerimeter(3,
                "Front",
                m_PerimeterDepth_FBSide,
                m_PerimeterWidth_FBSide,
                m_StartersLapLength_FBSide,
                m_StartersSpacing_FBSide,
                m_Starters_Phi_FBSide,
                m_Longitud_Reinf_TopAndBotom_Phi_FBSide,
                m_Longitud_Reinf_Intermediate_Phi_FBSide,
                m_Longitud_Reinf_Intermediate_Count_FBSide,
                true,
                0,
                rebatesFrontSide));

            m_PerimeterBeams.Add(new CSlabPerimeter(4,
                "Back",
                m_PerimeterDepth_FBSide,
                m_PerimeterWidth_FBSide,
                m_StartersLapLength_FBSide,
                m_StartersSpacing_FBSide,
                m_Starters_Phi_FBSide,
                m_Longitud_Reinf_TopAndBotom_Phi_FBSide,
                m_Longitud_Reinf_Intermediate_Phi_FBSide,
                m_Longitud_Reinf_Intermediate_Count_FBSide,
                true,
                0,
                rebatesBackSide));
        }

        public void SetDescriptionText()
        {
            /*
            m_Text = m_sMeshGradeName + " MESH" + "\n" +
                        (m_fConcreteCover * 1000).ToString("F0") + " mm TOP COVER" + "\n" +
                        (m_fDim3 * 1000).ToString("F0") + " mm THICK" + "\n" +
                        "CONCRETE SLAB" + "\n" +
                        "DPC OVER SANDBLINDING" + "\n" +
                        "& COMPACTED HARDFILL";
            */

            m_Text = (m_fDim3 * 1000).ToString("F0") + " mm THICK CONCRETE SLAB" + "\n" +
                     "WITH " + m_sMeshGradeName + " MESH on DPM" + "\n" +
                     "ON 25 mm THICK SAND" + "\n" +
                     "ON 150 mm THICK COMPACTED GAP 40 mm";

            // Vysvetlivka - GAP (general all passing)
            // DPM - damp-proof membrane
            // DPC - damp proof course
        }

        public void SetControlPoint()
        {
            m_pControlPoint = new Point3D(0 + m_Eccentricity_x, 0 + m_Eccentricity_y, 0 - m_fDim3 - fTolerance);
        }
    }
}
