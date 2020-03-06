using BaseClasses;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using DATABASE;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace PFD
{
    [Serializable]
    public class DisplayOptionsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        #region general
        private bool m_LightDirectional;
        private bool m_LightPoint;
        private bool m_LightSpot;
        private bool m_LightAmbient;

        private bool m_MaterialDiffuse;
        private bool m_MaterialEmissive;

        //Color display options
        private bool m_ColorsAccordingToMembers;
        private bool m_ColorsAccordingToSections;

        private bool m_DisplayDistinguishedColorMember;
        private bool m_DisplayTransparentModelMember;

        private bool MShowGlobalAxis;
        private bool MShowLocalMembersAxis;
        private bool MIsEnabledLocalMembersAxis;
        private bool MShowSurfaceLoadsAxis;
        private bool MIsEnabledSurfaceLoadsAxis;
        #endregion general


        private bool m_DisplayMembers;
        private bool m_DisplayJoints;
        private bool m_DisplayPlates;
        private bool m_DisplayConnectors;
        private bool m_DisplayNodes;
        private bool m_DisplayFoundations;
        private bool m_DisplayReinforcementBars;
        private bool m_DisplayFloorSlab;
        private bool m_DisplaySawCuts;
        private bool m_DisplayControlJoints;
        private bool m_DisplayNodalSupports;
        private bool m_DisplayMembersCenterLines;
        private bool m_DisplaySolidModel;
        private bool m_DisplayWireFrameModel;



        private bool m_DisplayMembersWireFrame;
        private bool m_DisplayJointsWireFrame;
        private bool m_DisplayPlatesWireFrame;
        private bool m_DisplayConnectorsWireFrame;
        private bool m_DisplayNodesWireFrame;
        private bool m_DisplayFoundationsWireFrame;
        private bool m_DisplayReinforcementBarsWireFrame;
        private bool m_DisplayFloorSlabWireFrame;

        // Labels and axes
        private bool MShowLoadsLabels;
        private bool MShowLoadsLabelsUnits;



        // Member description options
        private bool MShowMemberDescription;
        private bool MShowMemberID;
        private bool MShowMemberPrefix;
        private bool MShowMemberCrossSectionStartName;
        private bool MShowMemberRealLength;
        private bool MShowMemberRealLengthInMM;
        private bool MShowMemberRealLengthUnit;
        private bool MShowNodesDescription;

        private bool MShowFoundationsDescription;
        private bool MShowFloorSlabDescription;
        private bool MShowSawCutsDescription;
        private bool MShowControlJointsDescription;
        private bool MShowDimensions;
        private bool MShowGridLines;
        private bool MShowSectionSymbols;
        private bool MShowDetailSymbols;
        private bool MShowSlabRebates;




        [NonSerialized]
        private Color m_WireframeColor;
        private int m_WireframeColorIndex;
        private float m_WireFrameLineThickness;

        [NonSerialized]
        private Color m_MemberCenterlineColor;
        private int m_MemberCenterlineColorIndex;
        private float m_MemberCenterlineThickness;

        private float m_NodeDescriptionTextFontSize;
        private float m_MemberDescriptionTextFontSize;
        private float m_DimensionTextFontSize;
        private float m_GridLineLabelTextFontSize;
        private float m_SectionSymbolLabelTextFontSize;
        private float m_DetailSymbolLabelTextFontSize;

        private float m_SawCutTextFontSize;
        private float m_ControlJointTextFontSize;

        private float m_FoundationTextFontSize;
        private float m_FloorSlabTextFontSize;

        [NonSerialized]
        private Color m_NodeColor;
        private int m_NodeColorIndex;
        [NonSerialized]
        private Color m_NodeDescriptionTextColor = Colors.Cyan;
        private int m_NodeDescriptionTextColorIndex;
        [NonSerialized]
        private Color m_MemberDescriptionTextColor = Colors.Beige;
        private int m_MemberDescriptionTextColorIndex;
        [NonSerialized]
        private Color m_DimensionTextColor = Colors.LightGreen;
        private int m_DimensionTextColorIndex;
        [NonSerialized]
        private Color m_DimensionLineColor = Colors.LightGreen;
        private int m_DimensionLineColorIndex;

        [NonSerialized]
        private Color m_GridLineLabelTextColor = Colors.Coral;
        private int m_GridLineLabelTextColorIndex;
        [NonSerialized]
        private Color m_GridLineColor = Colors.Coral;
        private int m_GridLineColorIndex;
        private int m_GridLinePatternType = (int)ELinePatternType.DASHDOTTED;

        [NonSerialized]
        private Color m_SectionSymbolLabelTextColor = Colors.Cyan;
        private int m_SectionSymbolLabelTextColorIndex;
        [NonSerialized]
        private Color m_SectionSymbolColor = Colors.Cyan;
        private int m_SectionSymbolColorIndex;

        [NonSerialized]
        private Color m_DetailSymbolLabelTextColor = Colors.LightPink;

        [NonSerialized]
        private Color? m_DetailSymbolLabelBackColor = Colors.White;
        private int m_DetailSymbolLabelTextColorIndex;
        private int m_DetailSymbolLabelBackColorIndex;
        [NonSerialized]
        private Color m_DetailSymbolColor = Colors.LightPink;
        private int m_DetailSymbolColorIndex;


        [NonSerialized]
        private Color m_SawCutTextColor = Colors.Goldenrod;
        private int m_SawCutTextColorIndex;
        [NonSerialized]
        private Color m_SawCutLineColor = Colors.Goldenrod;
        private int m_SawCutLineColorIndex;
        private int m_SawCutLinePatternType = (int)ELinePatternType.DOTTED;

        [NonSerialized]
        private Color m_ControlJointTextColor = Colors.BlueViolet;
        private int m_ControlJointTextColorIndex;
        [NonSerialized]
        private Color m_ControlJointLineColor = Colors.BlueViolet;
        private int m_ControlJointLineColorIndex;
        private int m_ControlJointLinePatternType = (int)ELinePatternType.DIVIDE;

        [NonSerialized]
        private Color m_FoundationTextColor = Colors.HotPink;
        private int m_FoundationTextColorIndex;
        [NonSerialized]
        private Color m_FloorSlabTextColor = Colors.HotPink;
        private int m_FloorSlabTextColorIndex;

        [NonSerialized]
        private Color m_FoundationColor = Colors.DarkGray;
        private int m_FoundationColorIndex;
        [NonSerialized]
        private Color m_FloorSlabColor = Colors.LightGray;
        private int m_FloorSlabColorIndex;
        [NonSerialized]
        private Color m_SlabRebateColor = Colors.OrangeRed;
        private int m_SlabRebateColorIndex;

        private int m_PlateColorIndex;
        [NonSerialized]
        private Color m_PlateColor = Colors.Gray;

        private int m_ScrewColorIndex;
        [NonSerialized]
        private Color m_ScrewColor = Colors.Blue;

        private int m_AnchorColorIndex;
        [NonSerialized]
        private Color m_AnchorColor = Colors.LightGoldenrodYellow;

        private int m_WasherColorIndex;
        [NonSerialized]
        private Color m_WasherColor = Colors.LightGreen;

        private int m_NutColorIndex;
        [NonSerialized]
        private Color m_NutColor = Colors.LightPink;

        private int m_BackgroundColorIndex;
        [NonSerialized]
        private Color m_BackgroundColor = Colors.Black;

        private float m_MemberSolidModelOpacity = 0.8f;
        private float m_PlateSolidModelOpacity = 0.5f;
        private float m_ScrewSolidModelOpacity = 0.9f;
        private float m_AnchorSolidModelOpacity = 0.9f;
        private float m_FoundationSolidModelOpacity = 0.4f;
        private float m_ReinforcementBarSolidModelOpacity = 0.9f;
        private float m_FloorSlabSolidModelOpacity = 0.3f;
        private float m_SlabRebateSolidModelOpacity = 0.5f;


        #region Loads
        // Load Case - display options
        private bool MShowLoads;
        private bool MShowNodalLoads;
        private bool MShowLoadsOnMembers;
        private bool MShowLoadsOnGirts;
        private bool MShowLoadsOnPurlins;
        private bool MShowLoadsOnEavePurlins;
        private bool MShowLoadsOnWindPosts;
        private bool MShowLoadsOnFrameMembers;
        private bool MShowSurfaceLoads;



        private float MDisplayIn3DRatio;
        #endregion Loads


        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public bool LightDirectional
        {
            get
            {
                return m_LightDirectional;
            }

            set
            {
                m_LightDirectional = value;

                NotifyPropertyChanged("LightDirectional");
            }
        }

        public bool LightPoint
        {
            get
            {
                return m_LightPoint;
            }

            set
            {
                m_LightPoint = value;

                NotifyPropertyChanged("LightPoint");
            }
        }

        public bool LightSpot
        {
            get
            {
                return m_LightSpot;
            }

            set
            {
                m_LightSpot = value;

                NotifyPropertyChanged("LightSpot");
            }
        }

        public bool LightAmbient
        {
            get
            {
                return m_LightAmbient;
            }

            set
            {
                m_LightAmbient = value;

                NotifyPropertyChanged("LightAmbient");
            }
        }

        public bool MaterialDiffuse
        {
            get
            {
                return m_MaterialDiffuse;
            }

            set
            {
                m_MaterialDiffuse = value;
                if (!m_MaterialDiffuse && !m_MaterialEmissive) MaterialEmissive = true;

                NotifyPropertyChanged("MaterialDiffuse");
            }
        }

        public bool MaterialEmissive
        {
            get
            {
                return m_MaterialEmissive;
            }

            set
            {
                m_MaterialEmissive = value;
                if (!m_MaterialEmissive && !m_MaterialDiffuse) MaterialDiffuse = true;

                NotifyPropertyChanged("MaterialEmissive");
            }
        }

        public bool DisplayMembers
        {
            get
            {
                return m_DisplayMembers;
            }

            set
            {
                m_DisplayMembers = value;
                if (!m_DisplayMembers && MShowLocalMembersAxis) ShowLocalMembersAxis = false;
                SetIsEnabledLocalMembersAxis();

                NotifyPropertyChanged("DisplayMembers");
            }
        }

        public bool DisplayJoints
        {
            get
            {
                return m_DisplayJoints;
            }

            set
            {
                m_DisplayJoints = value;

                NotifyPropertyChanged("DisplayJoints");
            }
        }

        public bool DisplayPlates
        {
            get
            {
                return m_DisplayPlates;
            }

            set
            {
                m_DisplayPlates = value;
                if (m_DisplayPlates) DisplayJoints = true;

                NotifyPropertyChanged("DisplayPlates");
            }
        }

        public bool DisplayConnectors
        {
            get
            {
                return m_DisplayConnectors;
            }

            set
            {
                m_DisplayConnectors = value;
                if (m_DisplayConnectors) DisplayJoints = true;

                NotifyPropertyChanged("DisplayConnectors");
            }
        }

        public bool DisplayNodes
        {
            get
            {
                return m_DisplayNodes;
            }

            set
            {
                m_DisplayNodes = value;

                NotifyPropertyChanged("DisplayNodes");
            }
        }

        public bool DisplayFoundations
        {
            get
            {
                return m_DisplayFoundations;
            }

            set
            {
                m_DisplayFoundations = value;

                NotifyPropertyChanged("DisplayFoundations");
            }
        }

        public bool DisplayReinforcementBars
        {
            get
            {
                return m_DisplayReinforcementBars;
            }

            set
            {
                m_DisplayReinforcementBars = value;

                NotifyPropertyChanged("DisplayReinforcementBars");
            }
        }

        public bool DisplayFloorSlab
        {
            get
            {
                return m_DisplayFloorSlab;
            }

            set
            {
                m_DisplayFloorSlab = value;

                NotifyPropertyChanged("DisplayFloorSlab");
            }
        }

        public bool DisplaySawCuts
        {
            get
            {
                return m_DisplaySawCuts;
            }

            set
            {
                m_DisplaySawCuts = value;

                NotifyPropertyChanged("DisplaySawCuts");
            }
        }

        public bool DisplayControlJoints
        {
            get
            {
                return m_DisplayControlJoints;
            }

            set
            {
                m_DisplayControlJoints = value;

                NotifyPropertyChanged("DisplayControlJoints");
            }
        }

        public bool DisplayNodalSupports
        {
            get
            {
                return m_DisplayNodalSupports;
            }

            set
            {
                m_DisplayNodalSupports = value;

                NotifyPropertyChanged("DisplayNodalSupports");
            }
        }

        public bool DisplayMembersCenterLines
        {
            get
            {
                return m_DisplayMembersCenterLines;
            }

            set
            {
                m_DisplayMembersCenterLines = value;
                SetIsEnabledLocalMembersAxis();

                NotifyPropertyChanged("DisplayMembersCenterLines");
            }
        }

        public bool DisplaySolidModel
        {
            get
            {
                return m_DisplaySolidModel;
            }

            set
            {
                m_DisplaySolidModel = value;
                SetIsEnabledLocalMembersAxis();

                NotifyPropertyChanged("DisplaySolidModel");
            }
        }

        public bool DisplayWireFrameModel
        {
            get
            {
                return m_DisplayWireFrameModel;
            }

            set
            {
                m_DisplayWireFrameModel = value;
                SetIsEnabledLocalMembersAxis();

                NotifyPropertyChanged("DisplayWireFrameModel");
            }
        }

        public bool DisplayDistinguishedColorMember
        {
            get
            {
                return m_DisplayDistinguishedColorMember;
            }

            set
            {
                m_DisplayDistinguishedColorMember = value;

                NotifyPropertyChanged("DisplayDistinguishedColorMember");
            }
        }

        public bool DisplayTransparentModelMember
        {
            get
            {
                return m_DisplayTransparentModelMember;
            }

            set
            {
                m_DisplayTransparentModelMember = value;

                NotifyPropertyChanged("DisplayTransparentModelMember");
            }
        }

        public bool ColorsAccordingToMembers
        {
            get
            {
                return m_ColorsAccordingToMembers;
            }

            set
            {
                if (value == false && m_ColorsAccordingToSections == false) return;

                m_ColorsAccordingToMembers = value;

                NotifyPropertyChanged("ColorsAccordingToMembers");

            }
        }

        public bool ColorsAccordingToSections
        {
            get
            {
                return m_ColorsAccordingToSections;
            }

            set
            {
                if (value == false && m_ColorsAccordingToMembers == false) return;

                m_ColorsAccordingToSections = value;

                NotifyPropertyChanged("ColorsAccordingToSections");

            }
        }

        public bool ShowMemberDescription
        {
            get
            {
                return MShowMemberDescription;
            }

            set
            {
                MShowMemberDescription = value;

                NotifyPropertyChanged("ShowMemberDescription");
            }
        }

        public bool ShowMemberID
        {
            get
            {
                return MShowMemberID;
            }

            set
            {
                MShowMemberID = value;

                NotifyPropertyChanged("ShowMemberID");
            }
        }

        public bool ShowMemberPrefix
        {
            get
            {
                return MShowMemberPrefix;
            }

            set
            {
                MShowMemberPrefix = value;

                NotifyPropertyChanged("ShowMemberPrefix");
            }
        }

        public bool ShowMemberCrossSectionStartName
        {
            get
            {
                return MShowMemberCrossSectionStartName;
            }

            set
            {
                MShowMemberCrossSectionStartName = value;

                NotifyPropertyChanged("ShowMemberCrossSectionStartName");
            }
        }

        public bool ShowMemberRealLength
        {
            get
            {
                return MShowMemberRealLength;
            }

            set
            {
                MShowMemberRealLength = value;

                NotifyPropertyChanged("ShowMemberRealLength");
            }
        }
        public bool ShowMemberRealLengthInMM
        {
            get
            {
                return MShowMemberRealLengthInMM;
            }

            set
            {
                MShowMemberRealLengthInMM = value;
            }
        }
        public bool ShowMemberRealLengthUnit
        {
            get
            {
                return MShowMemberRealLengthUnit;
            }

            set
            {
                MShowMemberRealLengthUnit = value;
                NotifyPropertyChanged("ShowMemberRealLengthUnit");
            }
        }

        public bool ShowLoadsLabels
        {
            get
            {
                return MShowLoadsLabels;
            }

            set
            {
                MShowLoadsLabels = value;

                NotifyPropertyChanged("ShowLoadsLabels");
            }
        }

        public bool ShowLoadsLabelsUnits
        {
            get
            {
                return MShowLoadsLabelsUnits;
            }

            set
            {
                MShowLoadsLabelsUnits = value;

                NotifyPropertyChanged("ShowLoadsLabelsUnits");
            }
        }

        public float DisplayIn3DRatio
        {
            get
            {
                return MDisplayIn3DRatio;
            }

            set
            {
                MDisplayIn3DRatio = value;

                NotifyPropertyChanged("MDisplayIn3DRatio");
            }
        }

        public bool ShowGlobalAxis
        {
            get { return MShowGlobalAxis; }
            set { MShowGlobalAxis = value; NotifyPropertyChanged("ShowGlobalAxis"); }
        }

        public bool ShowLocalMembersAxis
        {
            get
            {
                return MShowLocalMembersAxis;
            }

            set
            {
                MShowLocalMembersAxis = value;

                NotifyPropertyChanged("ShowLocalMembersAxis");
            }
        }

        public bool IsEnabledLocalMembersAxis
        {
            get
            {
                return MIsEnabledLocalMembersAxis;
            }

            set
            {
                MIsEnabledLocalMembersAxis = value;

                NotifyPropertyChanged("IsEnabledLocalMembersAxis");
            }
        }

        public bool ShowSurfaceLoadsAxis
        {
            get
            {
                return MShowSurfaceLoadsAxis;
            }

            set
            {
                MShowSurfaceLoadsAxis = value;

                NotifyPropertyChanged("ShowSurfaceLoadsAxis");
            }
        }

        public bool IsEnabledSurfaceLoadsAxis
        {
            get
            {
                return MIsEnabledSurfaceLoadsAxis;
            }

            set
            {
                MIsEnabledSurfaceLoadsAxis = value;

                NotifyPropertyChanged("IsEnabledSurfaceLoadsAxis");
            }
        }




        public Color WireframeColor
        {
            get
            {
                return m_WireframeColor;
            }
            set
            {
                m_WireframeColor = value;
            }
        }

        public float WireFrameLineThickness
        {
            get
            {
                return m_WireFrameLineThickness;
            }

            set
            {
                m_WireFrameLineThickness = value;
                NotifyPropertyChanged("WireFrameLineThickness");
            }
        }

        public Color MemberCenterlineColor
        {
            get
            {
                return m_MemberCenterlineColor;
            }
            set
            {
                m_MemberCenterlineColor = value;
            }
        }

        public float MemberCenterlineThickness
        {
            get
            {
                return m_MemberCenterlineThickness;
            }

            set
            {
                m_MemberCenterlineThickness = value;
                NotifyPropertyChanged("MemberCenterlineThickness");
            }
        }

        public float NodeDescriptionTextFontSize
        {
            get
            {
                return m_NodeDescriptionTextFontSize;
            }

            set
            {
                m_NodeDescriptionTextFontSize = value;
                NotifyPropertyChanged("NodeDescriptionTextFontSize");
            }
        }

        public float MemberDescriptionTextFontSize
        {
            get
            {
                return m_MemberDescriptionTextFontSize;
            }

            set
            {
                m_MemberDescriptionTextFontSize = value;
                NotifyPropertyChanged("MemberDescriptionTextFontSize");
            }
        }

        public float DimensionTextFontSize
        {
            get
            {
                return m_DimensionTextFontSize;
            }

            set
            {
                m_DimensionTextFontSize = value;
                NotifyPropertyChanged("DimensionTextFontSize");
            }
        }

        public float GridLineLabelTextFontSize
        {
            get
            {
                return m_GridLineLabelTextFontSize;
            }

            set
            {
                m_GridLineLabelTextFontSize = value;
                NotifyPropertyChanged("GridLineLabelTextFontSize");
            }
        }

        public float SectionSymbolLabelTextFontSize
        {
            get
            {
                return m_SectionSymbolLabelTextFontSize;
            }

            set
            {
                m_SectionSymbolLabelTextFontSize = value;
                NotifyPropertyChanged("SectionSymbolLabelTextFontSize");
            }
        }

        public float DetailSymbolLabelTextFontSize
        {
            get
            {
                return m_DetailSymbolLabelTextFontSize;
            }

            set
            {
                m_DetailSymbolLabelTextFontSize = value;
                NotifyPropertyChanged("DetailSymbolLabelTextFontSize");
            }
        }

        public float SawCutTextFontSize
        {
            get
            {
                return m_SawCutTextFontSize;
            }

            set
            {
                m_SawCutTextFontSize = value;
                NotifyPropertyChanged("SawCutTextFontSize");
            }
        }

        public float ControlJointTextFontSize
        {
            get
            {
                return m_ControlJointTextFontSize;
            }

            set
            {
                m_ControlJointTextFontSize = value;
                NotifyPropertyChanged("ControlJointTextFontSize");
            }
        }

        public float FoundationTextFontSize
        {
            get
            {
                return m_FoundationTextFontSize;
            }

            set
            {
                m_FoundationTextFontSize = value;
                NotifyPropertyChanged("FoundationTextFontSize");
            }
        }

        public float FloorSlabTextFontSize
        {
            get
            {
                return m_FloorSlabTextFontSize;
            }

            set
            {
                m_FloorSlabTextFontSize = value;
                NotifyPropertyChanged("FloorSlabTextFontSize");
            }
        }

        public Color NodeColor
        {
            get
            {
                return m_NodeColor;
            }

            set
            {
                m_NodeColor = value;
                NotifyPropertyChanged("NodeColor");
            }
        }

        public Color NodeDescriptionTextColor
        {
            get
            {
                return m_NodeDescriptionTextColor;
            }

            set
            {
                m_NodeDescriptionTextColor = value;
                NotifyPropertyChanged("NodeDescriptionTextColor");
            }
        }

        public Color MemberDescriptionTextColor
        {
            get
            {
                return m_MemberDescriptionTextColor;
            }

            set
            {
                m_MemberDescriptionTextColor = value;
                NotifyPropertyChanged("MemberDescriptionTextColor");
            }
        }

        public Color DimensionTextColor
        {
            get
            {
                return m_DimensionTextColor;
            }

            set
            {
                m_DimensionTextColor = value;
                NotifyPropertyChanged("DimensionTextColor");
            }
        }

        public Color DimensionLineColor
        {
            get
            {
                return m_DimensionLineColor;
            }

            set
            {
                m_DimensionLineColor = value;
                NotifyPropertyChanged("DimensionLineColor");
            }
        }

        public Color GridLineLabelTextColor
        {
            get
            {
                return m_GridLineLabelTextColor;
            }

            set
            {
                m_GridLineLabelTextColor = value;
                NotifyPropertyChanged("GridLineLabelTextColor");
            }
        }

        public Color GridLineColor
        {
            get
            {
                return m_GridLineColor;
            }

            set
            {
                m_GridLineColor = value;
                NotifyPropertyChanged("GridLineColor");
            }
        }

        public int GridLinePatternType
        {
            get
            {
                return m_GridLinePatternType;
            }

            set
            {
                m_GridLinePatternType = value;
                NotifyPropertyChanged("GridLinePatternType");
            }
        }

        public Color SectionSymbolLabelTextColor
        {
            get
            {
                return m_SectionSymbolLabelTextColor;
            }

            set
            {
                m_SectionSymbolLabelTextColor = value;
                NotifyPropertyChanged("SectionSymbolLabelTextColor");
            }
        }

        public Color SectionSymbolColor
        {
            get
            {
                return m_SectionSymbolColor;
            }

            set
            {
                m_SectionSymbolColor = value;
                NotifyPropertyChanged("SectionSymbolColor");
            }
        }

        public Color DetailSymbolLabelTextColor
        {
            get
            {
                return m_DetailSymbolLabelTextColor;
            }

            set
            {
                m_DetailSymbolLabelTextColor = value;
                NotifyPropertyChanged("DetailSymbolLabelTextColor");
            }
        }
        public Color? DetailSymbolLabelBackColor
        {
            get
            {
                return m_DetailSymbolLabelBackColor;
            }

            set
            {
                m_DetailSymbolLabelBackColor = value;
                NotifyPropertyChanged("DetailSymbolLabelBackColor");
            }
        }

        public Color DetailSymbolColor
        {
            get
            {
                return m_DetailSymbolColor;
            }

            set
            {
                m_DetailSymbolColor = value;
                NotifyPropertyChanged("DetailSymbolColor");
            }
        }

        public Color SawCutTextColor
        {
            get
            {
                return m_SawCutTextColor;
            }

            set
            {
                m_SawCutTextColor = value;
                NotifyPropertyChanged("SawCutTextColor");
            }
        }

        public Color SawCutLineColor
        {
            get
            {
                return m_SawCutLineColor;
            }

            set
            {
                m_SawCutLineColor = value;
                NotifyPropertyChanged("SawCutLineColor");
            }
        }

        public int SawCutLinePatternType
        {
            get
            {
                return m_SawCutLinePatternType;
            }

            set
            {
                m_SawCutLinePatternType = value;
                NotifyPropertyChanged("SawCutLinePatternType");
            }
        }

        public Color ControlJointTextColor
        {
            get
            {
                return m_ControlJointTextColor;
            }

            set
            {
                m_ControlJointTextColor = value;
                NotifyPropertyChanged("ControlJointTextColor");
            }
        }

        public Color ControlJointLineColor
        {
            get
            {
                return m_ControlJointLineColor;
            }

            set
            {
                m_ControlJointLineColor = value;
                NotifyPropertyChanged("ControlJointLineColor");
            }
        }

        public int ControlJointLinePatternType
        {
            get
            {
                return m_ControlJointLinePatternType;
            }

            set
            {
                m_ControlJointLinePatternType = value;
                NotifyPropertyChanged("ControlJointLinePatternType");
            }
        }

        public Color FoundationTextColor
        {
            get
            {
                return m_FoundationTextColor;
            }

            set
            {
                m_FoundationTextColor = value;
                NotifyPropertyChanged("FoundationTextColor");
            }
        }

        public Color FloorSlabTextColor
        {
            get
            {
                return m_FloorSlabTextColor;
            }

            set
            {
                m_FloorSlabTextColor = value;
                NotifyPropertyChanged("FloorSlabTextColor");
            }
        }

        public Color FoundationColor
        {
            get
            {
                return m_FoundationColor;
            }

            set
            {
                m_FoundationColor = value;
                NotifyPropertyChanged("FoundationColor");
            }
        }

        public Color FloorSlabColor
        {
            get
            {
                return m_FloorSlabColor;
            }

            set
            {
                m_FloorSlabColor = value;
                NotifyPropertyChanged("FloorSlabColor");
            }
        }

        public Color SlabRebateColor
        {
            get
            {
                return m_SlabRebateColor;
            }

            set
            {
                m_SlabRebateColor = value;
                NotifyPropertyChanged("SlabRebateColor");
            }
        }

        public float MemberSolidModelOpacity
        {
            get
            {
                return m_MemberSolidModelOpacity;
            }

            set
            {
                m_MemberSolidModelOpacity = value;
                NotifyPropertyChanged("MemberSolidModelOpacity");
            }
        }

        public float PlateSolidModelOpacity
        {
            get
            {
                return m_PlateSolidModelOpacity;
            }

            set
            {
                m_PlateSolidModelOpacity = value;
                NotifyPropertyChanged("PlateSolidModelOpacity");
            }
        }

        public float ScrewSolidModelOpacity
        {
            get
            {
                return m_ScrewSolidModelOpacity;
            }

            set
            {
                m_ScrewSolidModelOpacity = value;
                NotifyPropertyChanged("ScrewSolidModelOpacity");
            }
        }

        public float AnchorSolidModelOpacity
        {
            get
            {
                return m_AnchorSolidModelOpacity;
            }

            set
            {
                m_AnchorSolidModelOpacity = value;
                NotifyPropertyChanged("AnchorSolidModelOpacity");
            }
        }

        public float FoundationSolidModelOpacity
        {
            get
            {
                return m_FoundationSolidModelOpacity;
            }

            set
            {
                m_FoundationSolidModelOpacity = value;
                NotifyPropertyChanged("FoundationSolidModelOpacity");
            }
        }

        public float ReinforcementBarSolidModelOpacity
        {
            get
            {
                return m_ReinforcementBarSolidModelOpacity;
            }

            set
            {
                m_ReinforcementBarSolidModelOpacity = value;
                NotifyPropertyChanged("ReinforcementBarSolidModelOpacity");
            }
        }

        public float FloorSlabSolidModelOpacity
        {
            get
            {
                return m_FloorSlabSolidModelOpacity;
            }

            set
            {
                m_FloorSlabSolidModelOpacity = value;
                NotifyPropertyChanged("FloorSlabSolidModelOpacity");
            }
        }

        public float SlabRebateSolidModelOpacity
        {
            get
            {
                return m_SlabRebateSolidModelOpacity;
            }

            set
            {
                m_SlabRebateSolidModelOpacity = value;
                NotifyPropertyChanged("SlabRebateSolidModelOpacity");
            }
        }

        public int WireframeColorIndex
        {
            get
            {
                return m_WireframeColorIndex;
            }

            set
            {
                m_WireframeColorIndex = value;

                WireframeColor = CComboBoxHelper.ColorList[m_WireframeColorIndex].Color.Value;

                NotifyPropertyChanged("WireframeColorIndex");
            }
        }

        public int MemberCenterlineColorIndex
        {
            get
            {
                return m_MemberCenterlineColorIndex;
            }

            set
            {
                m_MemberCenterlineColorIndex = value;

                MemberCenterlineColor = CComboBoxHelper.ColorList[m_MemberCenterlineColorIndex].Color.Value;

                NotifyPropertyChanged("MemberCenterlineColorIndex");
            }
        }

        public int NodeColorIndex
        {
            get
            {
                return m_NodeColorIndex;
            }

            set
            {
                m_NodeColorIndex = value;

                NodeColor = CComboBoxHelper.ColorList[m_NodeColorIndex].Color.Value;

                NotifyPropertyChanged("NodeColorIndex");
            }
        }

        public int NodeDescriptionTextColorIndex
        {
            get
            {
                return m_NodeDescriptionTextColorIndex;
            }

            set
            {
                m_NodeDescriptionTextColorIndex = value;

                NodeDescriptionTextColor = CComboBoxHelper.ColorList[m_NodeDescriptionTextColorIndex].Color.Value;

                NotifyPropertyChanged("NodeDescriptionTextColorIndex");
            }
        }

        public int MemberDescriptionTextColorIndex
        {
            get
            {
                return m_MemberDescriptionTextColorIndex;
            }

            set
            {
                m_MemberDescriptionTextColorIndex = value;

                MemberDescriptionTextColor = CComboBoxHelper.ColorList[m_MemberDescriptionTextColorIndex].Color.Value;

                NotifyPropertyChanged("MemberDescriptionTextColorIndex");
            }
        }

        public int DimensionTextColorIndex
        {
            get
            {
                return m_DimensionTextColorIndex;
            }

            set
            {
                m_DimensionTextColorIndex = value;

                DimensionTextColor = CComboBoxHelper.ColorList[m_DimensionTextColorIndex].Color.Value;

                NotifyPropertyChanged("DimensionTextColorIndex");
            }
        }

        public int DimensionLineColorIndex
        {
            get
            {
                return m_DimensionLineColorIndex;
            }

            set
            {
                m_DimensionLineColorIndex = value;

                DimensionLineColor = CComboBoxHelper.ColorList[m_DimensionLineColorIndex].Color.Value;

                NotifyPropertyChanged("DimensionLineColorIndex");
            }
        }

        public int GridLineLabelTextColorIndex
        {
            get
            {
                return m_GridLineLabelTextColorIndex;
            }

            set
            {
                m_GridLineLabelTextColorIndex = value;

                GridLineLabelTextColor = CComboBoxHelper.ColorList[m_GridLineLabelTextColorIndex].Color.Value;

                NotifyPropertyChanged("GridLineLabelTextColorIndex");
            }
        }

        public int GridLineColorIndex
        {
            get
            {
                return m_GridLineColorIndex;
            }

            set
            {
                m_GridLineColorIndex = value;

                GridLineColor = CComboBoxHelper.ColorList[m_GridLineColorIndex].Color.Value;

                NotifyPropertyChanged("GridLineColorIndex");
            }
        }
        public int SectionSymbolLabelTextColorIndex
        {
            get
            {
                return m_SectionSymbolLabelTextColorIndex;
            }

            set
            {
                m_SectionSymbolLabelTextColorIndex = value;
                SectionSymbolLabelTextColor = CComboBoxHelper.ColorList[m_SectionSymbolLabelTextColorIndex].Color.Value;

                NotifyPropertyChanged("SectionSymbolLabelTextColorIndex");
            }
        }
        public int SectionSymbolColorIndex
        {
            get
            {
                return m_SectionSymbolColorIndex;
            }

            set
            {
                m_SectionSymbolColorIndex = value;

                SectionSymbolColor = CComboBoxHelper.ColorList[m_SectionSymbolColorIndex].Color.Value;

                NotifyPropertyChanged("SectionSymbolColorIndex");
            }
        }

        public int DetailSymbolLabelTextColorIndex
        {
            get
            {
                return m_DetailSymbolLabelTextColorIndex;
            }

            set
            {
                m_DetailSymbolLabelTextColorIndex = value;

                DetailSymbolLabelTextColor = CComboBoxHelper.ColorList[m_DetailSymbolLabelTextColorIndex].Color.Value;

                NotifyPropertyChanged("DetailSymbolLabelTextColorIndex");
            }
        }
        public int DetailSymbolLabelBackColorIndex
        {
            get
            {
                return m_DetailSymbolLabelBackColorIndex;
            }

            set
            {
                m_DetailSymbolLabelBackColorIndex = value;

                DetailSymbolLabelBackColor = CComboBoxHelper.ColorListWithTransparent[m_DetailSymbolLabelBackColorIndex].Color;

                NotifyPropertyChanged("DetailSymbolLabelBackColorIndex");
            }
        }

        public int DetailSymbolColorIndex
        {
            get
            {
                return m_DetailSymbolColorIndex;
            }

            set
            {
                m_DetailSymbolColorIndex = value;

                DetailSymbolColor = CComboBoxHelper.ColorList[m_DetailSymbolColorIndex].Color.Value;

                NotifyPropertyChanged("DetailSymbolColorIndex");
            }
        }

        public int SawCutTextColorIndex
        {
            get
            {
                return m_SawCutTextColorIndex;
            }

            set
            {
                m_SawCutTextColorIndex = value;

                SawCutTextColor = CComboBoxHelper.ColorList[m_SawCutTextColorIndex].Color.Value;

                NotifyPropertyChanged("SawCutTextColorIndex");
            }
        }

        public int SawCutLineColorIndex
        {
            get
            {
                return m_SawCutLineColorIndex;
            }

            set
            {
                m_SawCutLineColorIndex = value;

                SawCutLineColor = CComboBoxHelper.ColorList[m_SawCutLineColorIndex].Color.Value;

                NotifyPropertyChanged("SawCutLineColorIndex");
            }
        }

        public int ControlJointTextColorIndex
        {
            get
            {
                return m_ControlJointTextColorIndex;
            }

            set
            {
                m_ControlJointTextColorIndex = value;

                ControlJointTextColor = CComboBoxHelper.ColorList[m_ControlJointTextColorIndex].Color.Value;

                NotifyPropertyChanged("ControlJointTextColorIndex");
            }
        }

        public int ControlJointLineColorIndex
        {
            get
            {
                return m_ControlJointLineColorIndex;
            }

            set
            {
                m_ControlJointLineColorIndex = value;

                ControlJointLineColor = CComboBoxHelper.ColorList[m_ControlJointLineColorIndex].Color.Value;

                NotifyPropertyChanged("ControlJointLineColorIndex");
            }
        }

        public int FoundationTextColorIndex
        {
            get
            {
                return m_FoundationTextColorIndex;
            }

            set
            {
                m_FoundationTextColorIndex = value;

                FoundationTextColor = CComboBoxHelper.ColorList[m_FoundationTextColorIndex].Color.Value;

                NotifyPropertyChanged("FoundationTextColorIndex");
            }
        }

        public int FloorSlabTextColorIndex
        {
            get
            {
                return m_FloorSlabTextColorIndex;
            }

            set
            {
                m_FloorSlabTextColorIndex = value;

                FloorSlabTextColor = CComboBoxHelper.ColorList[m_FloorSlabTextColorIndex].Color.Value;

                NotifyPropertyChanged("FloorSlabTextColorIndex");
            }
        }

        public int FoundationColorIndex
        {
            get
            {
                return m_FoundationColorIndex;
            }

            set
            {
                m_FoundationColorIndex = value;

                FoundationColor = CComboBoxHelper.ColorList[m_FoundationColorIndex].Color.Value;

                NotifyPropertyChanged("FoundationColorIndex");
            }
        }

        public int FloorSlabColorIndex
        {
            get
            {
                return m_FloorSlabColorIndex;
            }

            set
            {
                m_FloorSlabColorIndex = value;

                FloorSlabColor = CComboBoxHelper.ColorList[m_FloorSlabColorIndex].Color.Value;

                NotifyPropertyChanged("FloorSlabColorIndex");
            }
        }

        public int SlabRebateColorIndex
        {
            get
            {
                return m_SlabRebateColorIndex;
            }

            set
            {
                m_SlabRebateColorIndex = value;

                SlabRebateColor = CComboBoxHelper.ColorList[m_SlabRebateColorIndex].Color.Value;

                NotifyPropertyChanged("SlabRebateColorIndex");
            }
        }

        public int PlateColorIndex
        {
            get
            {
                return m_PlateColorIndex;
            }

            set
            {
                m_PlateColorIndex = value;

                PlateColor = CComboBoxHelper.ColorList[m_PlateColorIndex].Color.Value;

                NotifyPropertyChanged("PlateColorIndex");
            }
        }

        public Color PlateColor
        {
            get
            {
                return m_PlateColor;
            }

            set
            {
                m_PlateColor = value;
                NotifyPropertyChanged("PlateColor");
            }
        }

        public int ScrewColorIndex
        {
            get
            {
                return m_ScrewColorIndex;
            }

            set
            {
                m_ScrewColorIndex = value;

                ScrewColor = CComboBoxHelper.ColorList[m_ScrewColorIndex].Color.Value;

                NotifyPropertyChanged("ScrewColorIndex");
            }
        }

        public Color ScrewColor
        {
            get
            {
                return m_ScrewColor;
            }

            set
            {
                m_ScrewColor = value;
                NotifyPropertyChanged("ScrewColor");
            }
        }

        public int AnchorColorIndex
        {
            get
            {
                return m_AnchorColorIndex;
            }

            set
            {
                m_AnchorColorIndex = value;

                AnchorColor = CComboBoxHelper.ColorList[m_AnchorColorIndex].Color.Value;

                NotifyPropertyChanged("AnchorColorIndex");
            }
        }

        public Color AnchorColor
        {
            get
            {
                return m_AnchorColor;
            }

            set
            {
                m_AnchorColor = value;
                NotifyPropertyChanged("AnchorColor");
            }
        }

        public int WasherColorIndex
        {
            get
            {
                return m_WasherColorIndex;
            }

            set
            {
                m_WasherColorIndex = value;

                WasherColor = CComboBoxHelper.ColorList[m_WasherColorIndex].Color.Value;

                NotifyPropertyChanged("WasherColorIndex");
            }
        }

        public Color WasherColor
        {
            get
            {
                return m_WasherColor;
            }

            set
            {
                m_WasherColor = value;
                NotifyPropertyChanged("WasherColor");
            }
        }

        public int NutColorIndex
        {
            get
            {
                return m_NutColorIndex;
            }

            set
            {
                m_NutColorIndex = value;

                NutColor = CComboBoxHelper.ColorList[m_NutColorIndex].Color.Value;

                NotifyPropertyChanged("NutColorIndex");
            }
        }

        public Color NutColor
        {
            get
            {
                return m_NutColor;
            }

            set
            {
                m_NutColor = value;
                NotifyPropertyChanged("NutColor");
            }
        }

        public List<CComboColor> ColorList
        {
            get
            {
                return CComboBoxHelper.ColorList;
            }
        }
        public List<CComboColor> ColorListWithTransparent
        {
            get
            {
                return CComboBoxHelper.ColorListWithTransparent;
            }
        }
        public List<ComboItem> LinePatternTypes
        {
            get
            {
                return new List<ComboItem>() { new ComboItem((int)ELinePatternType.CONTINUOUS, "Continuous"),
                    new ComboItem((int)ELinePatternType.DASHED, "Dashed"),
                    new ComboItem((int)ELinePatternType.DOTTED, "Dotted"),
                    new ComboItem((int)ELinePatternType.DASHDOTTED, "Dashdotted"),
                    new ComboItem((int)ELinePatternType.DIVIDE, "Divide")
                };
            }
        }

        public int BackgroundColorIndex
        {
            get
            {
                return m_BackgroundColorIndex;
            }

            set
            {
                m_BackgroundColorIndex = value;

                BackgroundColor = CComboBoxHelper.ColorList[m_BackgroundColorIndex].Color.Value;

                NotifyPropertyChanged("BackgroundColorIndex");
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return m_BackgroundColor;
            }

            set
            {
                m_BackgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
            }
        }

        public bool ShowLoads
        {
            get
            {
                return MShowLoads;
            }

            set
            {
                MShowLoads = value;
                SetIsEnabledSurfaceLoadsAxis();

                NotifyPropertyChanged("ShowLoads");
            }
        }

        public bool ShowNodalLoads
        {
            get
            {
                return MShowNodalLoads;
            }

            set
            {
                MShowNodalLoads = value;

                NotifyPropertyChanged("ShowNodalLoads");
            }
        }

        public bool ShowLoadsOnMembers
        {
            get
            {
                return MShowLoadsOnMembers;
            }

            set
            {
                MShowLoadsOnMembers = value;
                NotifyPropertyChanged("ShowLoadsOnMembers");
            }
        }

        public bool ShowLoadsOnGirts
        {
            get
            {
                return MShowLoadsOnGirts;
            }

            set
            {
                MShowLoadsOnGirts = value;

                //if (MShowLoadsOnPurlinsAndGirts && MShowLoadsOnFrameMembers) ShowLoadsOnFrameMembers = false; // Umoznit zobrazit aj single members a frames spolocne
                NotifyPropertyChanged("ShowLoadsOnGirts");
            }
        }

        public bool ShowLoadsOnPurlins
        {
            get
            {
                return MShowLoadsOnPurlins;
            }

            set
            {
                MShowLoadsOnPurlins = value;

                //if (MShowLoadsOnPurlinsAndGirts && MShowLoadsOnFrameMembers) ShowLoadsOnFrameMembers = false; // Umoznit zobrazit aj single members a frames spolocne
                NotifyPropertyChanged("ShowLoadsOnPurlins");
            }
        }

        public bool ShowLoadsOnEavePurlins
        {
            get
            {
                return MShowLoadsOnEavePurlins;
            }

            set
            {
                MShowLoadsOnEavePurlins = value;

                //if (MShowLoadsOnPurlinsAndGirts && MShowLoadsOnFrameMembers) ShowLoadsOnFrameMembers = false; // Umoznit zobrazit aj single members a frames spolocne
                NotifyPropertyChanged("ShowLoadsOnEavePurlins");
            }
        }

        public bool ShowLoadsOnWindPosts
        {
            get
            {
                return MShowLoadsOnWindPosts;
            }

            set
            {
                MShowLoadsOnWindPosts = value;

                //if (MShowLoadsOnPurlinsAndGirts && MShowLoadsOnFrameMembers) ShowLoadsOnFrameMembers = false; // Umoznit zobrazit aj single members a frames spolocne
                NotifyPropertyChanged("ShowLoadsOnWindPosts");
            }
        }

        public bool ShowLoadsOnFrameMembers
        {
            get
            {
                return MShowLoadsOnFrameMembers;
            }

            set
            {
                MShowLoadsOnFrameMembers = value;

                //if (MShowLoadsOnPurlinsAndGirts && MShowLoadsOnFrameMembers) ShowLoadsOnPurlinsAndGirts = false; // Umoznit zobrazit aj single members a frames spolocne
                NotifyPropertyChanged("ShowLoadsOnFrameMembers");
            }
        }

        public bool ShowSurfaceLoads
        {
            get
            {
                return MShowSurfaceLoads;
            }

            set
            {
                MShowSurfaceLoads = value;
                if (!MShowSurfaceLoads && MShowSurfaceLoadsAxis) ShowSurfaceLoadsAxis = false;
                SetIsEnabledSurfaceLoadsAxis();

                NotifyPropertyChanged("ShowSurfaceLoads");
            }
        }

        public bool ShowNodesDescription
        {
            get
            {
                return MShowNodesDescription;
            }

            set
            {
                MShowNodesDescription = value;

                NotifyPropertyChanged("ShowNodesDescription");
            }
        }

        public bool ShowFoundationsDescription
        {
            get
            {
                return MShowFoundationsDescription;
            }

            set
            {
                MShowFoundationsDescription = value;
                NotifyPropertyChanged("ShowFoundationsDescription");
            }
        }

        public bool ShowFloorSlabDescription
        {
            get
            {
                return MShowFloorSlabDescription;
            }

            set
            {
                MShowFloorSlabDescription = value;
                NotifyPropertyChanged("ShowFloorSlabDescription");
            }
        }

        public bool ShowSawCutsDescription
        {
            get
            {
                return MShowSawCutsDescription;
            }

            set
            {
                MShowSawCutsDescription = value;
                NotifyPropertyChanged("ShowSawCutsDescription");
            }
        }

        public bool ShowControlJointsDescription
        {
            get
            {
                return MShowControlJointsDescription;
            }

            set
            {
                MShowControlJointsDescription = value;
                NotifyPropertyChanged("ShowControlJointsDescription");
            }
        }

        public bool ShowDimensions
        {
            get
            {
                return MShowDimensions;
            }

            set
            {
                MShowDimensions = value;
                NotifyPropertyChanged("ShowDimensions");
            }
        }

        public bool ShowGridLines
        {
            get
            {
                return MShowGridLines;
            }

            set
            {
                MShowGridLines = value;
                NotifyPropertyChanged("ShowGridLines");
            }
        }

        public bool ShowSectionSymbols
        {
            get
            {
                return MShowSectionSymbols;
            }

            set
            {
                MShowSectionSymbols = value;
                NotifyPropertyChanged("ShowSectionSymbols");
            }
        }

        public bool ShowDetailSymbols
        {
            get
            {
                return MShowDetailSymbols;
            }

            set
            {
                MShowDetailSymbols = value;
                NotifyPropertyChanged("ShowDetailSymbols");
            }
        }

        public bool ShowSlabRebates
        {
            get
            {
                return MShowSlabRebates;
            }

            set
            {
                MShowSlabRebates = value;
                NotifyPropertyChanged("ShowSlabRebates");
            }
        }

        public bool DisplayMembersWireFrame
        {
            get
            {
                return m_DisplayMembersWireFrame;
            }

            set
            {
                m_DisplayMembersWireFrame = value;
                NotifyPropertyChanged("DisplayMembersWireFrame");
            }
        }

        public bool DisplayJointsWireFrame
        {
            get
            {
                return m_DisplayJointsWireFrame;
            }

            set
            {
                m_DisplayJointsWireFrame = value;
                NotifyPropertyChanged("DisplayJointsWireFrame");
            }
        }

        public bool DisplayPlatesWireFrame
        {
            get
            {
                return m_DisplayPlatesWireFrame;
            }

            set
            {
                m_DisplayPlatesWireFrame = value;
                NotifyPropertyChanged("DisplayPlatesWireFrame");
            }
        }

        public bool DisplayConnectorsWireFrame
        {
            get
            {
                return m_DisplayConnectorsWireFrame;
            }

            set
            {
                m_DisplayConnectorsWireFrame = value;
                NotifyPropertyChanged("DisplayConnectorsWireFrame");
            }
        }

        public bool DisplayNodesWireFrame
        {
            get
            {
                return m_DisplayNodesWireFrame;
            }

            set
            {
                m_DisplayNodesWireFrame = value;
                NotifyPropertyChanged("DisplayNodesWireFrame");
            }
        }

        public bool DisplayFoundationsWireFrame
        {
            get
            {
                return m_DisplayFoundationsWireFrame;
            }

            set
            {
                m_DisplayFoundationsWireFrame = value;
                NotifyPropertyChanged("DisplayFoundationsWireFrame");
            }
        }

        public bool DisplayReinforcementBarsWireFrame
        {
            get
            {
                return m_DisplayReinforcementBarsWireFrame;
            }

            set
            {
                m_DisplayReinforcementBarsWireFrame = value;
                NotifyPropertyChanged("DisplayReinforcementBarsWireFrame");
            }
        }

        public bool DisplayFloorSlabWireFrame
        {
            get
            {
                return m_DisplayFloorSlabWireFrame;
            }

            set
            {
                m_DisplayFloorSlabWireFrame = value;
                NotifyPropertyChanged("DisplayFloorSlabWireFrame");
            }
        }

        private void SetIsEnabledLocalMembersAxis()
        {
            //ak su zapnute Members, ale nie je ziaden z checkboxov Display Members Centerline, Solid Model, Wireframe Model zapnuty, 
            //tak by malo byt zobrazenie os Local Member Axis disabled.            
            if (m_DisplayMembers)
            {
                if (m_DisplayMembersCenterLines || m_DisplaySolidModel || m_DisplayWireFrameModel) IsEnabledLocalMembersAxis = true;
                else { IsEnabledLocalMembersAxis = false; }
            }
            else { IsEnabledLocalMembersAxis = false; }

            if (!IsEnabledLocalMembersAxis && ShowLocalMembersAxis) ShowLocalMembersAxis = false;
        }

        private void SetIsEnabledSurfaceLoadsAxis()
        {
            //Podobne ak su sice zapnute Surface loads, ale nie su zapnute Loads ako celok, tak by Surface Loads Axis malo byt disabled.
            if (MShowSurfaceLoads && MShowLoads) IsEnabledSurfaceLoadsAxis = true;
            else IsEnabledSurfaceLoadsAxis = false;

            if (!IsEnabledSurfaceLoadsAxis && ShowSurfaceLoadsAxis) ShowSurfaceLoadsAxis = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DisplayOptionsViewModel(bool bRelease = false)
        {
            IsSetFromCode = true;

            LightDirectional = false;
            LightPoint = false;
            LightSpot = false;
            LightAmbient = true;
            MaterialDiffuse = true;
            MaterialEmissive = false;
            DisplayMembers = true;
            DisplayJoints = true; // Vypnute v defaulte
            DisplayPlates = true; // Vypnute v defaulte
            DisplayConnectors = false; // Vypnute v defaulte
            DisplayNodes = false;
            DisplayFoundations = true;
            DisplayReinforcementBars = false;
            DisplayFloorSlab = true;
            DisplaySawCuts = true;
            DisplayControlJoints = true;

            DisplayMembersWireFrame = false;
            DisplayJointsWireFrame = false;
            DisplayPlatesWireFrame = false;
            DisplayConnectorsWireFrame = false;
            DisplayNodesWireFrame = false;
            DisplayFoundationsWireFrame = false;
            DisplayReinforcementBarsWireFrame = false;
            DisplayNodalSupports = false;
            DisplayMembersCenterLines = false;
            DisplaySolidModel = true;
            DisplayWireFrameModel = false;
            DisplayDistinguishedColorMember = false;
            DisplayTransparentModelMember = false;
            ColorsAccordingToMembers = true;
            ColorsAccordingToSections = false;

            ShowNodesDescription = false;
            ShowMemberDescription = false;
            ShowMemberID = true;
            ShowMemberPrefix = true;
            ShowMemberRealLength = true;
            ShowMemberRealLengthInMM = true;
            ShowMemberRealLengthUnit = false;
            ShowMemberCrossSectionStartName = false;
            ShowFoundationsDescription = false;
            ShowSawCutsDescription = false;
            ShowControlJointsDescription = false;
            ShowDimensions = true;
            ShowGridLines = false;
            ShowSectionSymbols = false;
            ShowDetailSymbols = false;
            ShowSlabRebates = true;

            if(bRelease) // Vsetko okrem centerlines a zakladnych kot vypneme
            {
                LightAmbient = true;
                DisplayJoints = false;
                DisplayPlates = false;
                DisplayFoundations = false;
                DisplayFloorSlab = false;
                DisplaySawCuts = false;
                DisplayControlJoints = false;
                DisplayMembersCenterLines = true; // Zobrazujeme
                DisplaySolidModel = false;
                ShowSlabRebates = false;
            }

            ShowLoads = false;
            ShowLoadsOnMembers = false;
            ShowLoadsOnGirts = true;
            ShowLoadsOnPurlins = true;
            ShowLoadsOnEavePurlins = true;
            ShowLoadsOnWindPosts = true;
            ShowLoadsOnFrameMembers = true;
            ShowNodalLoads = false;
            ShowSurfaceLoads = false;
            ShowLoadsLabels = false;
            ShowLoadsLabelsUnits = false;
            ShowGlobalAxis = true;
            ShowLocalMembersAxis = false;
            ShowSurfaceLoadsAxis = false;

            DisplayIn3DRatio = 0.003f;


            WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.CadetBlue);
            WireFrameLineThickness = 2;

            MemberCenterlineColorIndex = CComboBoxHelper.GetColorIndex(Colors.WhiteSmoke);
            MemberCenterlineThickness = 2;

            NodeDescriptionTextFontSize = 12;
            MemberDescriptionTextFontSize = 12;
            DimensionTextFontSize = 12;
            GridLineLabelTextFontSize = 30;
            SectionSymbolLabelTextFontSize = 30;
            DetailSymbolLabelTextFontSize = 30;

            SawCutTextFontSize = 12;
            ControlJointTextFontSize = 12;

            FoundationTextFontSize = 12;
            FloorSlabTextFontSize = 12;

            NodeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            NodeDescriptionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            MemberDescriptionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Beige);
            DimensionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGreen);
            DimensionLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGreen);

            GridLineLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Coral);
            GridLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Coral);
            GridLinePatternType = (int)ELinePatternType.DASHDOTTED;

            SectionSymbolLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            SectionSymbolColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);

            DetailSymbolLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightPink);
            DetailSymbolLabelBackColorIndex = CComboBoxHelper.GetColorIndexWithTransparent(Colors.White);
            DetailSymbolColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightPink);

            SawCutTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Goldenrod);
            SawCutLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Goldenrod);
            SawCutLinePatternType = (int)ELinePatternType.DOTTED;

            ControlJointTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.BlueViolet);
            ControlJointLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.BlueViolet);
            ControlJointLinePatternType = (int)ELinePatternType.DIVIDE;

            FoundationTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.HotPink);
            FloorSlabTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.HotPink);

            FoundationColorIndex = CComboBoxHelper.GetColorIndex(Colors.DarkGray);
            FloorSlabColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGray);
            SlabRebateColorIndex = CComboBoxHelper.GetColorIndex(Colors.DarkOrange);

            PlateColorIndex = CComboBoxHelper.GetColorIndex(Colors.Gray);
            ScrewColorIndex = CComboBoxHelper.GetColorIndex(Colors.Blue);
            AnchorColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGoldenrodYellow);
            WasherColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGreen);
            NutColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightPink);

            MemberSolidModelOpacity = 0.8f;
            PlateSolidModelOpacity = 0.5f;
            ScrewSolidModelOpacity = 0.9f;
            AnchorSolidModelOpacity = 0.9f;
            FoundationSolidModelOpacity = 0.4f;
            ReinforcementBarSolidModelOpacity = 0.9f;
            FloorSlabSolidModelOpacity = 0.3f;
            SlabRebateSolidModelOpacity = 0.3f;

            BackgroundColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetViewModel(DisplayOptionsViewModel newVM)
        {
            IsSetFromCode = true;
            LightDirectional = newVM.LightDirectional;
            LightPoint = newVM.LightPoint;
            LightSpot = newVM.LightSpot;
            LightAmbient = newVM.LightAmbient;
            MaterialDiffuse = newVM.MaterialDiffuse;
            MaterialEmissive = newVM.MaterialEmissive;
            DisplayMembers = newVM.DisplayMembers;
            DisplayJoints = newVM.DisplayJoints;
            DisplayPlates = newVM.DisplayPlates;
            DisplayConnectors = newVM.DisplayConnectors;
            DisplayNodes = newVM.DisplayNodes;
            DisplayFoundations = newVM.DisplayFoundations;
            DisplayReinforcementBars = newVM.DisplayReinforcementBars;
            DisplayFloorSlab = newVM.DisplayFloorSlab;
            DisplaySawCuts = newVM.DisplaySawCuts;
            DisplayControlJoints = newVM.DisplayControlJoints;

            DisplayMembersWireFrame = newVM.DisplayMembersWireFrame;
            DisplayJointsWireFrame = newVM.DisplayJointsWireFrame;
            DisplayPlatesWireFrame = newVM.DisplayPlatesWireFrame;
            DisplayConnectorsWireFrame = newVM.DisplayConnectorsWireFrame;
            DisplayNodesWireFrame = newVM.DisplayNodesWireFrame;
            DisplayFoundationsWireFrame = newVM.DisplayFoundationsWireFrame;
            DisplayReinforcementBarsWireFrame = newVM.DisplayReinforcementBarsWireFrame;
            DisplayNodalSupports = newVM.DisplayNodalSupports;
            DisplayMembersCenterLines = newVM.DisplayMembersCenterLines;
            DisplaySolidModel = newVM.DisplaySolidModel;
            DisplayWireFrameModel = newVM.DisplayWireFrameModel;
            DisplayDistinguishedColorMember = newVM.DisplayDistinguishedColorMember;
            DisplayTransparentModelMember = newVM.DisplayTransparentModelMember;
            ColorsAccordingToMembers = newVM.ColorsAccordingToMembers;
            ColorsAccordingToSections = newVM.ColorsAccordingToSections;

            ShowNodesDescription = newVM.ShowNodesDescription;
            ShowMemberDescription = newVM.ShowMemberDescription;
            ShowMemberID = newVM.ShowMemberID;
            ShowMemberPrefix = newVM.ShowMemberPrefix;
            ShowMemberRealLength = newVM.ShowMemberRealLength;
            ShowMemberRealLengthInMM = newVM.ShowMemberRealLengthInMM;
            ShowMemberRealLengthUnit = newVM.ShowMemberRealLengthUnit;
            ShowMemberCrossSectionStartName = newVM.ShowMemberCrossSectionStartName;
            ShowFoundationsDescription = newVM.ShowFoundationsDescription;
            ShowSawCutsDescription = newVM.ShowSawCutsDescription;
            ShowControlJointsDescription = newVM.ShowControlJointsDescription;
            ShowDimensions = newVM.ShowDimensions;
            ShowGridLines = newVM.ShowGridLines;
            ShowSectionSymbols = newVM.ShowSectionSymbols;
            ShowDetailSymbols = newVM.ShowDetailSymbols;
            ShowSlabRebates = newVM.ShowSlabRebates;

            ShowLoads = newVM.ShowLoads;
            ShowLoadsOnMembers = newVM.ShowLoadsOnMembers;
            ShowLoadsOnGirts = newVM.ShowLoadsOnGirts;
            ShowLoadsOnPurlins = newVM.ShowLoadsOnPurlins;
            ShowLoadsOnEavePurlins = newVM.ShowLoadsOnEavePurlins;
            ShowLoadsOnWindPosts = newVM.ShowLoadsOnWindPosts;
            ShowLoadsOnFrameMembers = newVM.ShowLoadsOnFrameMembers;
            ShowNodalLoads = newVM.ShowNodalLoads;
            ShowSurfaceLoads = newVM.ShowSurfaceLoads;
            ShowLoadsLabels = newVM.ShowLoadsLabels;
            ShowLoadsLabelsUnits = newVM.ShowLoadsLabelsUnits;
            ShowGlobalAxis = newVM.ShowGlobalAxis;
            ShowLocalMembersAxis = newVM.ShowLocalMembersAxis;
            ShowSurfaceLoadsAxis = newVM.ShowSurfaceLoadsAxis;

            DisplayIn3DRatio = newVM.DisplayIn3DRatio;


            WireframeColorIndex = newVM.WireframeColorIndex;
            WireFrameLineThickness = newVM.WireFrameLineThickness;

            MemberCenterlineColorIndex = newVM.MemberCenterlineColorIndex;
            MemberCenterlineThickness = newVM.MemberCenterlineThickness;

            NodeDescriptionTextFontSize = newVM.NodeDescriptionTextFontSize;
            MemberDescriptionTextFontSize = newVM.MemberDescriptionTextFontSize;
            DimensionTextFontSize = newVM.DimensionTextFontSize;
            GridLineLabelTextFontSize = newVM.GridLineLabelTextFontSize;
            SectionSymbolLabelTextFontSize = newVM.SectionSymbolLabelTextFontSize;
            DetailSymbolLabelTextFontSize = newVM.DetailSymbolLabelTextFontSize;

            SawCutTextFontSize = newVM.SawCutTextFontSize;
            ControlJointTextFontSize = newVM.ControlJointTextFontSize;

            FoundationTextFontSize = newVM.FoundationTextFontSize;
            FloorSlabTextFontSize = newVM.FloorSlabTextFontSize;

            NodeColorIndex = newVM.NodeColorIndex;
            NodeDescriptionTextColorIndex = newVM.NodeDescriptionTextColorIndex;
            MemberDescriptionTextColorIndex = newVM.MemberDescriptionTextColorIndex;
            DimensionTextColorIndex = newVM.DimensionTextColorIndex;
            DimensionLineColorIndex = newVM.DimensionLineColorIndex;
            GridLineLabelTextColorIndex = newVM.GridLineLabelTextColorIndex;
            GridLineColorIndex = newVM.GridLineColorIndex;
            GridLinePatternType = newVM.GridLinePatternType;
            SectionSymbolLabelTextColorIndex = newVM.SectionSymbolLabelTextColorIndex;
            SectionSymbolColorIndex = newVM.SectionSymbolColorIndex;
            DetailSymbolLabelTextColorIndex = newVM.DetailSymbolLabelTextColorIndex;
            DetailSymbolLabelBackColorIndex = newVM.DetailSymbolLabelBackColorIndex;
            DetailSymbolColorIndex = newVM.DetailSymbolColorIndex;
            SawCutTextColorIndex = newVM.SawCutTextColorIndex;
            SawCutLineColorIndex = newVM.SawCutLineColorIndex;
            SawCutLinePatternType = newVM.SawCutLinePatternType;
            ControlJointTextColorIndex = newVM.ControlJointTextColorIndex;
            ControlJointLineColorIndex = newVM.ControlJointLineColorIndex;
            ControlJointLinePatternType = newVM.ControlJointLinePatternType;

            FoundationTextColorIndex = newVM.FoundationTextColorIndex;
            FloorSlabTextColorIndex = newVM.FloorSlabTextColorIndex;

            FoundationColorIndex = newVM.FoundationColorIndex;
            FloorSlabColorIndex = newVM.FloorSlabColorIndex;
            SlabRebateColorIndex = newVM.SlabRebateColorIndex;

            PlateColorIndex = newVM.PlateColorIndex;
            ScrewColorIndex = newVM.ScrewColorIndex;
            AnchorColorIndex = newVM.AnchorColorIndex;
            WasherColorIndex = newVM.WasherColorIndex;
            NutColorIndex = newVM.NutColorIndex;

            MemberSolidModelOpacity = newVM.MemberSolidModelOpacity;
            PlateSolidModelOpacity = newVM.PlateSolidModelOpacity;
            ScrewSolidModelOpacity = newVM.ScrewSolidModelOpacity;
            AnchorSolidModelOpacity = newVM.AnchorSolidModelOpacity;
            FoundationSolidModelOpacity = newVM.FoundationSolidModelOpacity;
            ReinforcementBarSolidModelOpacity = newVM.ReinforcementBarSolidModelOpacity;
            FloorSlabSolidModelOpacity = newVM.FloorSlabSolidModelOpacity;
            SlabRebateSolidModelOpacity = newVM.SlabRebateSolidModelOpacity;

            BackgroundColorIndex = newVM.BackgroundColorIndex;

            IsSetFromCode = false;
        }
    }
}