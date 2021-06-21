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
        #region private fields
        #region general
        private bool m_LightDirectional;
        private bool m_LightPoint;
        private bool m_LightSpot;
        private bool m_LightAmbient;

        private bool m_MaterialDiffuse;
        private bool m_MaterialEmissive;

        //Color display options
        private bool m_ColorsAccordingToMembersPrefix;
        private bool m_ColorsAccordingToMembersPosition;
        private bool m_ColorsAccordingToSections;

        private bool m_DisplayDistinguishedColorMember;
        //private bool m_DisplayTransparentModelMember;

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
        private bool m_DisplayCladdingWireFrame;
        private bool m_DisplayFibreglassWireFrame;
        private bool m_DisplayDoorsWireFrame;
        private bool m_DisplayWindowsWireFrame;

        private bool m_DoorsSimpleSolidModel;
        private bool m_DoorsSimpleWireframe;
        private bool m_WindowOutlineOnly;

        private bool m_DisplayCladding;
        private bool m_DisplayCladdingLeftWall;
        private bool m_DisplayCladdingRightWall;
        private bool m_DisplayCladdingFrontWall;
        private bool m_DisplayCladdingBackWall;
        private bool m_DisplayCladdingRoof;
        private bool m_DisplayFibreglass;
        private bool m_DisplayDoors;
        private bool m_DisplayWindows;

        private bool m_ColoredCenterlines;

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

        private bool m_DisplayCladdingDescription;
        private bool m_DisplayCladdingID;
        private bool m_DisplayCladdingPrefix;
        private bool m_DisplayCladdingLengthWidth;
        private bool m_DisplayCladdingArea;
        private bool m_DisplayCladdingUnits;

        private bool m_DisplayFibreglassDescription;
        private bool m_DisplayFibreglassID;
        private bool m_DisplayFibreglassPrefix;
        private bool m_DisplayFibreglassLengthWidth;
        private bool m_DisplayFibreglassArea;
        private bool m_DisplayFibreglassUnits;

        private bool m_DisplayDoorDescription;
        private bool m_DisplayDoorID;
        private bool m_DisplayDoorType;
        private bool m_DisplayDoorHeightWidth;
        private bool m_DisplayDoorArea;
        private bool m_DisplayDoorUnits;

        private bool m_DisplayWindowDescription;
        private bool m_DisplayWindowID;
        private bool m_DisplayWindowHeightWidth;
        private bool m_DisplayWindowArea;
        private bool m_DisplayWindowUnits;

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

        [NonSerialized]
        private Color m_CladdingTextColor = Colors.OrangeRed;
        private int m_CladdingTextColorIndex;
        [NonSerialized]
        private Color m_FibreglassTextColor = Colors.Indigo;
        private int m_FibreglassTextColorIndex;
        [NonSerialized]
        private Color m_DoorTextColor = Colors.Coral;
        private int m_DoorTextColorIndex;
        [NonSerialized]
        private Color m_WindowTextColor = Colors.Cyan;
        private int m_WindowTextColorIndex;

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

        //To Mato: naco sa to tu inicializovalo, nato je konstruktor
        private float m_MemberSolidModelOpacity;
        private float m_PlateSolidModelOpacity;
        private float m_ScrewSolidModelOpacity;
        private float m_AnchorSolidModelOpacity;
        private float m_FoundationSolidModelOpacity;
        private float m_ReinforcementBarSolidModelOpacity;
        private float m_FloorSlabSolidModelOpacity;
        private float m_SlabRebateSolidModelOpacity;

        private float m_FrontCladdingOpacity;
        private float m_LeftCladdingOpacity;
        private float m_RoofCladdingOpacity;
        private float m_FlashingOpacity;
        private float m_DoorPanelOpacity;
        private float m_WindowPanelOpacity;
        private float m_FibreglassOpacity;

        private float m_FloorSlabTextSize;
        private float m_GridlinesSize;
        private float m_GridLineLabelSize;
        private float m_SectionSymbolsSize;
        private float m_SectionSymbolLabelSize;
        private float m_DetailSymbolSize;
        private float m_DetailSymbolLabelSize;
        private float m_MembersDescriptionSize;
        private float m_NodesDescriptionSize;
        private float m_SawCutTextSize;
        private float m_ControlJointTextSize;
        private float m_FoundationTextSize;
        private float m_DimensionsTextSize;
        private float m_DimensionsLineRadius;
        private float m_DimensionsScale;
        private float m_DescriptionTextWidthScaleFactor;
        private float m_CladdingDescriptionSize;
        private float m_FibreglassDescriptionSize;
        private float m_DoorDescriptionSize;
        private float m_WindowDescriptionSize;

        private bool m_CladdingSheetColoursByID;

        private bool m_UseDifColorForSheetWithOverlap;
        private int m_CladdingSheetColorIndex;
        [NonSerialized]
        private Color m_CladdingSheetColor = Colors.Yellow;
        private int m_FibreglassSheetColorIndex;
        [NonSerialized]
        private Color m_FibreglassSheetColor = Colors.OrangeRed;

        private bool m_UseTextures;
        private bool m_UseTexturesMembers;
        private bool m_UseTexturesPlates;
        private bool m_UseTexturesCladding;

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

        #endregion private fields

        // TODO 701

        // Properties defined only for export - layouts (LY - layout)
        private EPageSizes m_LY_ViewsPageSize;
        private EImagesQuality m_LY_ExportImagesQuality;

        // Properties defined only in source code (CO - code only)
        private bool m_CO_IsExport;
        private bool m_CO_SameScaleForViews;

        private bool m_CO_bTransformScreenLines3DToCylinders3D;
        private float m_CO_DisplayIn3DRatio;
        private int m_CO_RotateModelX;
        private int m_CO_RotateModelY;
        private int m_CO_RotateModelZ;
        private int m_CO_ModelView;
        private int m_CO_ViewModelMembers;
        private int m_CO_ViewCladding;
        private bool m_CO_bUseOrtographicCamera;
        private double m_CO_OrtographicCameraWidth;

        private bool m_CO_bCreateHorizontalGridlines;
        private bool m_CO_bCreateVerticalGridlinesFront;
        private bool m_CO_bCreateVerticalGridlinesBack;
        private bool m_CO_bCreateVerticalGridlinesLeft;
        private bool m_CO_bCreateVerticalGridlinesRight;

        private int m_FO_ReinforcementBarColor_Top_x_Index;
        private int m_FO_ReinforcementBarColor_Top_y_Index;
        private int m_FO_ReinforcementBarColor_Bottom_x_Index;
        private int m_FO_ReinforcementBarColor_Bottom_y_Index;

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        #region Properties
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

        //public bool DisplayTransparentModelMember
        //{
        //    get
        //    {
        //        return m_DisplayTransparentModelMember;
        //    }

        //    set
        //    {
        //        m_DisplayTransparentModelMember = value;

        //        NotifyPropertyChanged("DisplayTransparentModelMember");
        //    }
        //}

        public bool ColorsAccordingToMembersPrefix
        {
            get
            {
                return m_ColorsAccordingToMembersPrefix;
            }

            set
            {
                if (value == false && m_ColorsAccordingToSections == false && m_ColorsAccordingToMembersPosition == false) return;

                m_ColorsAccordingToMembersPrefix = value;
                NotifyPropertyChanged("ColorsAccordingToMembersPrefix");
            }
        }

        public bool ColorsAccordingToMembersPosition
        {
            get
            {
                return m_ColorsAccordingToMembersPosition;
            }

            set
            {
                if (value == false && m_ColorsAccordingToSections == false && m_ColorsAccordingToMembersPrefix == false) return;

                m_ColorsAccordingToMembersPosition = value;
                NotifyPropertyChanged("ColorsAccordingToMembersPosition");
            }
        }
        //public bool ColorsAccordingToMembers
        //{
        //    get
        //    {
        //        return m_ColorsAccordingToMembers;
        //    }

        //    set
        //    {
        //        if (value == false && m_ColorsAccordingToSections == false) return;

        //        m_ColorsAccordingToMembers = value;

        //        NotifyPropertyChanged("ColorsAccordingToMembers");

        //    }
        //}

        public bool ColorsAccordingToSections
        {
            get
            {
                return m_ColorsAccordingToSections;
            }

            set
            {
                if (value == false && m_ColorsAccordingToMembersPrefix == false && m_ColorsAccordingToMembersPosition == false) return;

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

        public Color CladdingTextColor
        {
            get
            {
                return m_CladdingTextColor;
            }

            set
            {
                m_CladdingTextColor = value;
                NotifyPropertyChanged("CladdingTextColor");
            }
        }

        public Color FibreglassTextColor
        {
            get
            {
                return m_FibreglassTextColor;
            }

            set
            {
                m_FibreglassTextColor = value;
                NotifyPropertyChanged("FibreglassTextColor");
            }
        }

        public Color DoorTextColor
        {
            get
            {
                return m_DoorTextColor;
            }

            set
            {
                m_DoorTextColor = value;
                NotifyPropertyChanged("DoorTextColor");
            }
        }

        public Color WindowTextColor
        {
            get
            {
                return m_WindowTextColor;
            }

            set
            {
                m_WindowTextColor = value;
                NotifyPropertyChanged("WindowTextColor");
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

        public int CladdingTextColorIndex
        {
            get
            {
                return m_CladdingTextColorIndex;
            }

            set
            {
                m_CladdingTextColorIndex = value;

                CladdingTextColor = CComboBoxHelper.ColorList[m_CladdingTextColorIndex].Color.Value;

                NotifyPropertyChanged("CladdingTextColorIndex");
            }
        }

        public int FibreglassTextColorIndex
        {
            get
            {
                return m_FibreglassTextColorIndex;
            }

            set
            {
                m_FibreglassTextColorIndex = value;

                FibreglassTextColor = CComboBoxHelper.ColorList[m_FibreglassTextColorIndex].Color.Value;

                NotifyPropertyChanged("FibreglassTextColorIndex");
            }
        }

        public int DoorTextColorIndex
        {
            get
            {
                return m_DoorTextColorIndex;
            }

            set
            {
                m_DoorTextColorIndex = value;

                DoorTextColor = CComboBoxHelper.ColorList[m_DoorTextColorIndex].Color.Value;

                NotifyPropertyChanged("DoorTextColorIndex");
            }
        }

        public int WindowTextColorIndex
        {
            get
            {
                return m_WindowTextColorIndex;
            }

            set
            {
                m_WindowTextColorIndex = value;

                WindowTextColor = CComboBoxHelper.ColorList[m_WindowTextColorIndex].Color.Value;

                NotifyPropertyChanged("WindowTextColorIndex");
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
        //public List<CComboColor> ColorListWithTransparent
        //{
        //    get
        //    {
        //        return CComboBoxHelper.ColorListWithTransparent;
        //    }
        //}
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

        public bool DisplayCladdingWireFrame
        {
            get
            {
                return m_DisplayCladdingWireFrame;
            }

            set
            {
                m_DisplayCladdingWireFrame = value;
                NotifyPropertyChanged("DisplayCladddingWireFrame");
            }
        }

        public bool DisplayFibreglassWireFrame
        {
            get
            {
                return m_DisplayFibreglassWireFrame;
            }

            set
            {
                m_DisplayFibreglassWireFrame = value;
                NotifyPropertyChanged("DisplayFibreglassWireFrame");
            }
        }

        public bool DisplayDoorsWireFrame
        {
            get
            {
                return m_DisplayDoorsWireFrame;
            }

            set
            {
                m_DisplayDoorsWireFrame = value;
                NotifyPropertyChanged("DisplayDoorsWireFrame");
            }
        }

        public bool DisplayWindowsWireFrame
        {
            get
            {
                return m_DisplayWindowsWireFrame;
            }

            set
            {
                m_DisplayWindowsWireFrame = value;
                NotifyPropertyChanged("DisplayWindowsWireFrame");
            }
        }


        public bool DisplayCladding
        {
            get
            {
                return m_DisplayCladding;
            }

            set
            {
                m_DisplayCladding = value;
                NotifyPropertyChanged("DisplayCladding");
            }
        }

        public bool DisplayCladdingLeftWall
        {
            get
            {
                return m_DisplayCladdingLeftWall;
            }

            set
            {
                m_DisplayCladdingLeftWall = value;
                NotifyPropertyChanged("DisplayCladdingLeftWall");
            }
        }

        public bool DisplayCladdingRightWall
        {
            get
            {
                return m_DisplayCladdingRightWall;
            }

            set
            {
                m_DisplayCladdingRightWall = value;
                NotifyPropertyChanged("DisplayCladdingRightWall");
            }
        }

        public bool DisplayCladdingFrontWall
        {
            get
            {
                return m_DisplayCladdingFrontWall;
            }

            set
            {
                m_DisplayCladdingFrontWall = value;
                NotifyPropertyChanged("DisplayCladdingFrontWall");
            }
        }

        public bool DisplayCladdingBackWall
        {
            get
            {
                return m_DisplayCladdingBackWall;
            }

            set
            {
                m_DisplayCladdingBackWall = value;
                NotifyPropertyChanged("DisplayCladdingBackWall");
            }
        }

        public bool DisplayCladdingRoof
        {
            get
            {
                return m_DisplayCladdingRoof;
            }

            set
            {
                m_DisplayCladdingRoof = value;
                NotifyPropertyChanged("DisplayCladdingRoof");
            }
        }
        public bool DisplayFibreglass
        {
            get
            {
                return m_DisplayFibreglass;
            }

            set
            {
                m_DisplayFibreglass = value;
                NotifyPropertyChanged("DisplayFibreglass");
            }
        }

        public bool DisplayDoors
        {
            get
            {
                return m_DisplayDoors;
            }

            set
            {
                m_DisplayDoors = value;
                NotifyPropertyChanged("DisplayDoors");
            }
        }

        public bool DisplayWindows
        {
            get
            {
                return m_DisplayWindows;
            }

            set
            {
                m_DisplayWindows = value;
                NotifyPropertyChanged("DisplayWindows");
            }
        }

        #region Export Options commnented

        //public float ExportFloorSlabTextSize
        //{
        //    get
        //    {
        //        return m_ExportFloorSlabTextSize;
        //    }

        //    set
        //    {
        //        m_ExportFloorSlabTextSize = value;
        //        NotifyPropertyChanged("ExportFloorSlabTextSize");
        //    }
        //}

        //public float ExportGridlinesSize
        //{
        //    get
        //    {
        //        return m_ExportGridlinesSize;
        //    }

        //    set
        //    {
        //        m_ExportGridlinesSize = value;
        //        NotifyPropertyChanged("ExportGridlinesSize");
        //    }
        //}

        //public float ExportGridLineLabelSize
        //{
        //    get
        //    {
        //        return m_ExportGridLineLabelSize;
        //    }

        //    set
        //    {
        //        m_ExportGridLineLabelSize = value;
        //        NotifyPropertyChanged("ExportGridLineLabelSize");
        //    }
        //}

        //public float ExportSectionSymbolsSize
        //{
        //    get
        //    {
        //        return m_ExportSectionSymbolsSize;
        //    }

        //    set
        //    {
        //        m_ExportSectionSymbolsSize = value;
        //        NotifyPropertyChanged("ExportSectionSymbolsSize");
        //    }
        //}

        //public float ExportSectionSymbolLabelSize
        //{
        //    get
        //    {
        //        return m_ExportSectionSymbolLabelSize;
        //    }

        //    set
        //    {
        //        m_ExportSectionSymbolLabelSize = value;
        //        NotifyPropertyChanged("ExportSectionSymbolLabelSize");
        //    }
        //}

        //public float ExportDetailSymbolSize
        //{
        //    get
        //    {
        //        return m_ExportDetailSymbolSize;
        //    }

        //    set
        //    {
        //        m_ExportDetailSymbolSize = value;
        //        NotifyPropertyChanged("ExportDetailSymbolSize");
        //    }
        //}

        //public float ExportDetailSymbolLabelSize
        //{
        //    get
        //    {
        //        return m_ExportDetailSymbolLabelSize;
        //    }

        //    set
        //    {
        //        m_ExportDetailSymbolLabelSize = value;
        //        NotifyPropertyChanged("ExportDetailSymbolLabelSize");
        //    }
        //}

        //public float ExportMembersDescriptionSize
        //{
        //    get
        //    {
        //        return m_ExportMembersDescriptionSize;
        //    }

        //    set
        //    {
        //        m_ExportMembersDescriptionSize = value;
        //        NotifyPropertyChanged("ExportMembersDescriptionSize");
        //    }
        //}

        //public float ExportNodesDescriptionSize
        //{
        //    get
        //    {
        //        return m_ExportNodesDescriptionSize;
        //    }

        //    set
        //    {
        //        m_ExportNodesDescriptionSize = value;
        //        NotifyPropertyChanged("ExportNodesDescriptionSize");
        //    }
        //}

        //public float ExportSawCutTextSize
        //{
        //    get
        //    {
        //        return m_ExportSawCutTextSize;
        //    }

        //    set
        //    {
        //        m_ExportSawCutTextSize = value;
        //        NotifyPropertyChanged("ExportSawCutTextSize");
        //    }
        //}

        //public float ExportControlJointTextSize
        //{
        //    get
        //    {
        //        return m_ExportControlJointTextSize;
        //    }

        //    set
        //    {
        //        m_ExportControlJointTextSize = value;
        //        NotifyPropertyChanged("ExportControlJointTextSize");
        //    }
        //}

        //public float ExportFoundationTextSize
        //{
        //    get
        //    {
        //        return m_ExportFoundationTextSize;
        //    }

        //    set
        //    {
        //        m_ExportFoundationTextSize = value;
        //        NotifyPropertyChanged("ExportFoundationTextSize");
        //    }
        //}

        //public float ExportCladdingDescriptionSize
        //{
        //    get
        //    {
        //        return m_ExportCladdingDescriptionSize;
        //    }

        //    set
        //    {
        //        m_ExportCladdingDescriptionSize = value;
        //        NotifyPropertyChanged("ExportCladdingDescriptionSize");
        //    }
        //}

        //public float ExportFibreglassDescriptionSize
        //{
        //    get
        //    {
        //        return m_ExportFibreglassDescriptionSize;
        //    }

        //    set
        //    {
        //        m_ExportFibreglassDescriptionSize = value;
        //        NotifyPropertyChanged("ExportFibreglassDescriptionSize");
        //    }
        //}

        //public float ExportDoorDescriptionSize
        //{
        //    get
        //    {
        //        return m_ExportDoorDescriptionSize;
        //    }

        //    set
        //    {
        //        m_ExportDoorDescriptionSize = value;
        //        NotifyPropertyChanged("ExportDoorDescriptionSize");
        //    }
        //}

        //public float ExportWindowDescriptionSize
        //{
        //    get
        //    {
        //        return m_ExportWindowDescriptionSize;
        //    }

        //    set
        //    {
        //        m_ExportWindowDescriptionSize = value;
        //        NotifyPropertyChanged("ExportWindowDescriptionSize");
        //    }
        //}

        //public float ExportDimensionsTextSize
        //{
        //    get
        //    {
        //        return m_ExportDimensionsTextSize;
        //    }

        //    set
        //    {
        //        m_ExportDimensionsTextSize = value;
        //        NotifyPropertyChanged("ExportDimensionsTextSize");
        //    }
        //}

        //public float ExportDimensionsLineRadius
        //{
        //    get
        //    {
        //        return m_ExportDimensionsLineRadius;
        //    }

        //    set
        //    {
        //        m_ExportDimensionsLineRadius = value;
        //        NotifyPropertyChanged("ExportDimensionsLineRadius");
        //    }
        //}

        //public float ExportDimensionsScale
        //{
        //    get
        //    {
        //        return m_ExportDimensionsScale;
        //    }

        //    set
        //    {
        //        m_ExportDimensionsScale = value;
        //        NotifyPropertyChanged("ExportDimensionsScale");
        //    }
        //}
        //public float ExportDescriptionTextWidthScaleFactor
        //{
        //    get
        //    {
        //        return m_ExportDescriptionTextWidthScaleFactor;
        //    }

        //    set
        //    {
        //        m_ExportDescriptionTextWidthScaleFactor = value;
        //    }
        //}

        #endregion

        public float FloorSlabTextSize
        {
            get
            {
                return m_FloorSlabTextSize;
            }

            set
            {
                m_FloorSlabTextSize = value;
                NotifyPropertyChanged("FloorSlabTextSize");
            }
        }

        public float GridlinesSize
        {
            get
            {
                return m_GridlinesSize;
            }

            set
            {
                m_GridlinesSize = value;
                NotifyPropertyChanged("GridlinesSize");
            }
        }

        public float GridLineLabelSize
        {
            get
            {
                return m_GridLineLabelSize;
            }

            set
            {
                m_GridLineLabelSize = value;
                NotifyPropertyChanged("GridLineLabelSize");
            }
        }

        public float SectionSymbolsSize
        {
            get
            {
                return m_SectionSymbolsSize;
            }

            set
            {
                m_SectionSymbolsSize = value;
                NotifyPropertyChanged("SectionSymbolsSize");
            }
        }

        public float SectionSymbolLabelSize
        {
            get
            {
                return m_SectionSymbolLabelSize;
            }

            set
            {
                m_SectionSymbolLabelSize = value;
                NotifyPropertyChanged("SectionSymbolLabelSize");
            }
        }

        public float DetailSymbolSize
        {
            get
            {
                return m_DetailSymbolSize;
            }

            set
            {
                m_DetailSymbolSize = value;
                NotifyPropertyChanged("DetailSymbolSize");
            }
        }

        public float DetailSymbolLabelSize
        {
            get
            {
                return m_DetailSymbolLabelSize;
            }

            set
            {
                m_DetailSymbolLabelSize = value;
                NotifyPropertyChanged("DetailSymbolLabelSize");
            }
        }

        public float MembersDescriptionSize
        {
            get
            {
                return m_MembersDescriptionSize;
            }

            set
            {
                m_MembersDescriptionSize = value;
                NotifyPropertyChanged("MembersDescriptionSize");
            }
        }

        public float NodesDescriptionSize
        {
            get
            {
                return m_NodesDescriptionSize;
            }

            set
            {
                m_NodesDescriptionSize = value;
                NotifyPropertyChanged("NodesDescriptionSize");
            }
        }

        public float SawCutTextSize
        {
            get
            {
                return m_SawCutTextSize;
            }

            set
            {
                m_SawCutTextSize = value;
                NotifyPropertyChanged("SawCutTextSize");
            }
        }

        public float ControlJointTextSize
        {
            get
            {
                return m_ControlJointTextSize;
            }

            set
            {
                m_ControlJointTextSize = value;
                NotifyPropertyChanged("ControlJointTextSize");
            }
        }

        public float FoundationTextSize
        {
            get
            {
                return m_FoundationTextSize;
            }

            set
            {
                m_FoundationTextSize = value;
                NotifyPropertyChanged("FoundationTextSize");
            }
        }

        public float DimensionsTextSize
        {
            get
            {
                return m_DimensionsTextSize;
            }

            set
            {
                m_DimensionsTextSize = value;
                NotifyPropertyChanged("DimensionsTextSize");
            }
        }

        public float CladdingDescriptionSize
        {
            get
            {
                return m_CladdingDescriptionSize;
            }

            set
            {
                m_CladdingDescriptionSize = value;
                NotifyPropertyChanged("CladdingDescriptionSize");
            }
        }

        public float FibreglassDescriptionSize
        {
            get
            {
                return m_FibreglassDescriptionSize;
            }

            set
            {
                m_FibreglassDescriptionSize = value;
                NotifyPropertyChanged("FibreglassDescriptionSize");
            }
        }

        public float DoorDescriptionSize
        {
            get
            {
                return m_DoorDescriptionSize;
            }

            set
            {
                m_DoorDescriptionSize = value;
                NotifyPropertyChanged("DoorDescriptionSize");
            }
        }

        public float WindowDescriptionSize
        {
            get
            {
                return m_WindowDescriptionSize;
            }

            set
            {
                m_WindowDescriptionSize = value;
                NotifyPropertyChanged("WindowDescriptionSize");
            }
        }

       

        public float DimensionsLineRadius
        {
            get
            {
                return m_DimensionsLineRadius;
            }

            set
            {
                m_DimensionsLineRadius = value;
                NotifyPropertyChanged("DimensionsLineRadius");
            }
        }

        public float DimensionsScale
        {
            get
            {
                return m_DimensionsScale;
            }

            set
            {
                m_DimensionsScale = value;
                NotifyPropertyChanged("DimensionsScale");
            }
        }

        public float FrontCladdingOpacity
        {
            get
            {
                return m_FrontCladdingOpacity;
            }

            set
            {
                m_FrontCladdingOpacity = value;
                NotifyPropertyChanged("FrontCladdingOpacity");
            }
        }

        public float LeftCladdingOpacity
        {
            get
            {
                return m_LeftCladdingOpacity;
            }

            set
            {
                m_LeftCladdingOpacity = value;
                NotifyPropertyChanged("LeftCladdingOpacity");
            }
        }

        public float RoofCladdingOpacity
        {
            get
            {
                return m_RoofCladdingOpacity;
            }

            set
            {
                m_RoofCladdingOpacity = value;
                NotifyPropertyChanged("RoofCladdingOpacity");
            }
        }

        public float FlashingOpacity
        {
            get
            {
                return m_FlashingOpacity;
            }

            set
            {
                m_FlashingOpacity = value;
                NotifyPropertyChanged("FlashingOpacity");
            }
        }

        public float DoorPanelOpacity
        {
            get
            {
                return m_DoorPanelOpacity;
            }

            set
            {
                m_DoorPanelOpacity = value;
                NotifyPropertyChanged("DoorPanelOpacity");
            }
        }

        public float WindowPanelOpacity
        {
            get
            {
                return m_WindowPanelOpacity;
            }

            set
            {
                m_WindowPanelOpacity = value;
                NotifyPropertyChanged("WindowPanelOpacity");
            }
        }
        public float FibreglassOpacity
        {
            get
            {
                return m_FibreglassOpacity;
            }

            set
            {
                m_FibreglassOpacity = value;
                NotifyPropertyChanged("FibreglassOpacity");
            }
        }

        public float DescriptionTextWidthScaleFactor
        {
            get
            {
                return m_DescriptionTextWidthScaleFactor;
            }

            set
            {
                m_DescriptionTextWidthScaleFactor = value;
            }
        }

        public bool ColoredCenterlines
        {
            get
            {
                return m_ColoredCenterlines;
            }

            set
            {
                m_ColoredCenterlines = value;
                NotifyPropertyChanged("ColoredCenterlines");
            }
        }

        public bool CladdingSheetColoursByID
        {
            get
            {
                return m_CladdingSheetColoursByID;
            }

            set
            {
                m_CladdingSheetColoursByID = value;
                if (m_CladdingSheetColoursByID == true && m_UseDifColorForSheetWithOverlap == true) UseDifColorForSheetWithOverlap = false;
                NotifyPropertyChanged("CladdingSheetColoursByID");
            }
        }

        public bool DisplayCladdingDescription
        {
            get
            {
                return m_DisplayCladdingDescription;
            }

            set
            {
                m_DisplayCladdingDescription = value;
                NotifyPropertyChanged("DisplayCladdingDescription");
            }
        }

        public bool DisplayCladdingID
        {
            get
            {
                return m_DisplayCladdingID;
            }

            set
            {
                m_DisplayCladdingID = value;
                NotifyPropertyChanged("DisplayCladdingID");
            }
        }

        public bool DisplayCladdingPrefix
        {
            get
            {
                return m_DisplayCladdingPrefix;
            }

            set
            {
                m_DisplayCladdingPrefix = value;
                NotifyPropertyChanged("DisplayCladdingPrefix");
            }
        }

        public bool DisplayCladdingLengthWidth
        {
            get
            {
                return m_DisplayCladdingLengthWidth;
            }

            set
            {
                m_DisplayCladdingLengthWidth = value;
                NotifyPropertyChanged("DisplayCladdingLengthWidth");
            }
        }

        public bool DisplayCladdingArea
        {
            get
            {
                return m_DisplayCladdingArea;
            }

            set
            {
                m_DisplayCladdingArea = value;
                NotifyPropertyChanged("DisplayCladdingArea");
            }
        }

        public bool DisplayCladdingUnits
        {
            get
            {
                return m_DisplayCladdingUnits;
            }

            set
            {
                m_DisplayCladdingUnits = value;
                NotifyPropertyChanged("DisplayCladdingUnits");
            }
        }

        public bool DisplayFibreglassDescription
        {
            get
            {
                return m_DisplayFibreglassDescription;
            }

            set
            {
                m_DisplayFibreglassDescription = value;
                NotifyPropertyChanged("DisplayFibreglassDescription");
            }
        }

        public bool DisplayFibreglassID
        {
            get
            {
                return m_DisplayFibreglassID;
            }

            set
            {
                m_DisplayFibreglassID = value;
                NotifyPropertyChanged("DisplayFibreglassID");
            }
        }

        public bool DisplayFibreglassPrefix
        {
            get
            {
                return m_DisplayFibreglassPrefix;
            }

            set
            {
                m_DisplayFibreglassPrefix = value;
                NotifyPropertyChanged("DisplayFibreglassPrefix");
            }
        }

        public bool DisplayFibreglassLengthWidth
        {
            get
            {
                return m_DisplayFibreglassLengthWidth;
            }

            set
            {
                m_DisplayFibreglassLengthWidth = value;
                NotifyPropertyChanged("DisplayFibreglassLengthWidth");
            }
        }

        public bool DisplayFibreglassArea
        {
            get
            {
                return m_DisplayFibreglassArea;
            }

            set
            {
                m_DisplayFibreglassArea = value;
                NotifyPropertyChanged("DisplayFibreglassArea");
            }
        }

        public bool DisplayFibreglassUnits
        {
            get
            {
                return m_DisplayFibreglassUnits;
            }

            set
            {
                m_DisplayFibreglassUnits = value;
                NotifyPropertyChanged("DisplayFibreglassUnits");
            }
        }

        public bool DisplayDoorDescription
        {
            get
            {
                return m_DisplayDoorDescription;
            }

            set
            {
                m_DisplayDoorDescription = value;
                NotifyPropertyChanged("DisplayDoorDescription");
            }
        }

        public bool DisplayDoorID
        {
            get
            {
                return m_DisplayDoorID;
            }

            set
            {
                m_DisplayDoorID = value;
                NotifyPropertyChanged("DisplayDoorID");
            }
        }

        public bool DisplayDoorType
        {
            get
            {
                return m_DisplayDoorType;
            }

            set
            {
                m_DisplayDoorType = value;
                NotifyPropertyChanged("DisplayDoorType");
            }
        }

        public bool DisplayDoorHeightWidth
        {
            get
            {
                return m_DisplayDoorHeightWidth;
            }

            set
            {
                m_DisplayDoorHeightWidth = value;
                NotifyPropertyChanged("DisplayDoorHeightWidth");
            }
        }

        public bool DisplayDoorArea
        {
            get
            {
                return m_DisplayDoorArea;
            }

            set
            {
                m_DisplayDoorArea = value;
                NotifyPropertyChanged("DisplayDoorArea");
            }
        }

        public bool DisplayDoorUnits
        {
            get
            {
                return m_DisplayDoorUnits;
            }

            set
            {
                m_DisplayDoorUnits = value;
                NotifyPropertyChanged("DisplayDoorUnits");
            }
        }

        public bool DisplayWindowDescription
        {
            get
            {
                return m_DisplayWindowDescription;
            }

            set
            {
                m_DisplayWindowDescription = value;
                NotifyPropertyChanged("DisplayWindowDescription");
            }
        }

        public bool DisplayWindowID
        {
            get
            {
                return m_DisplayWindowID;
            }

            set
            {
                m_DisplayWindowID = value;
                NotifyPropertyChanged("DisplayWindowID");
            }
        }

        public bool DisplayWindowHeightWidth
        {
            get
            {
                return m_DisplayWindowHeightWidth;
            }

            set
            {
                m_DisplayWindowHeightWidth = value;
                NotifyPropertyChanged("DisplayWindowHeightWidth");
            }
        }

        public bool DisplayWindowArea
        {
            get
            {
                return m_DisplayWindowArea;
            }

            set
            {
                m_DisplayWindowArea = value;
                NotifyPropertyChanged("DisplayWindowArea");
            }
        }

        public bool DisplayWindowUnits
        {
            get
            {
                return m_DisplayWindowUnits;
            }

            set
            {
                m_DisplayWindowUnits = value;
                NotifyPropertyChanged("DisplayWindowUnits");
            }
        }

        public bool DoorsSimpleSolidModel
        {
            get
            {
                return m_DoorsSimpleSolidModel;
            }

            set
            {
                m_DoorsSimpleSolidModel = value;
                if (m_DoorsSimpleSolidModel == false)
                {
                    DoorsSimpleWireframe = false;
                    WindowOutlineOnly = false;
                }
                
                NotifyPropertyChanged("DoorsSimpleSolidModel");
            }
        }

        public bool DoorsSimpleWireframe
        {
            get
            {
                return m_DoorsSimpleWireframe;
            }

            set
            {
                m_DoorsSimpleWireframe = value;
                if (m_DoorsSimpleSolidModel == false && m_DoorsSimpleWireframe == true) m_DoorsSimpleWireframe = false;

                if (m_DoorsSimpleWireframe == false && m_WindowOutlineOnly == true) WindowOutlineOnly = false;
                NotifyPropertyChanged("DoorsSimpleWireframe");
            }
        }

        public bool WindowOutlineOnly
        {
            get
            {
                return m_WindowOutlineOnly;
            }

            set
            {
                m_WindowOutlineOnly = value;
                if (m_DoorsSimpleWireframe == false && m_WindowOutlineOnly == true) m_WindowOutlineOnly = false;
                NotifyPropertyChanged("WindowOutlineOnly");
            }
        }

        public bool UseDifColorForSheetWithOverlap
        {
            get
            {
                return m_UseDifColorForSheetWithOverlap;
            }

            set
            {
                m_UseDifColorForSheetWithOverlap = value;
                if (m_CladdingSheetColoursByID == true && m_UseDifColorForSheetWithOverlap == true) CladdingSheetColoursByID = false;
                NotifyPropertyChanged("UseDifColorForSheetWithOverlap");
            }
        }

        public int CladdingSheetColorIndex
        {
            get
            {
                return m_CladdingSheetColorIndex;
            }

            set
            {
                m_CladdingSheetColorIndex = value;

                CladdingSheetColor = CComboBoxHelper.ColorList[m_CladdingSheetColorIndex].Color.Value;
                NotifyPropertyChanged("CladdingSheetColorIndex");
            }
        }

        public int FibreglassSheetColorIndex
        {
            get
            {
                return m_FibreglassSheetColorIndex;
            }

            set
            {
                m_FibreglassSheetColorIndex = value;

                FibreglassSheetColor = CComboBoxHelper.ColorList[m_FibreglassSheetColorIndex].Color.Value;
                NotifyPropertyChanged("FibreglassSheetColorIndex");
            }
        }

        public Color CladdingSheetColor
        {
            get
            {
                return m_CladdingSheetColor;
            }

            set
            {
                m_CladdingSheetColor = value;
            }
        }

        public Color FibreglassSheetColor
        {
            get
            {
                return m_FibreglassSheetColor;
            }

            set
            {
                m_FibreglassSheetColor = value;
            }
        }

        public bool UseTextures
        {
            get
            {
                return m_UseTextures;
            }

            set
            {
                m_UseTextures = value;
                NotifyPropertyChanged("UseTextures");
            }
        }
        public bool UseTexturesMembers
        {
            get
            {
                return m_UseTexturesMembers;
            }

            set
            {
                m_UseTexturesMembers = value;
                NotifyPropertyChanged("UseTexturesMembers");
            }
        }
        public bool UseTexturesPlates
        {
            get
            {
                return m_UseTexturesPlates;
            }

            set
            {
                m_UseTexturesPlates = value;
                NotifyPropertyChanged("UseTexturesPlates");
            }
        }

        public bool UseTexturesCladding
        {
            get
            {
                return m_UseTexturesCladding;
            }

            set
            {
                m_UseTexturesCladding = value;
                NotifyPropertyChanged("UseTexturesCladding");
            }
        }

        public EPageSizes LY_ViewsPageSize { get => m_LY_ViewsPageSize; set => m_LY_ViewsPageSize = value; }
        public EImagesQuality LY_ExportImagesQuality { get => m_LY_ExportImagesQuality; set => m_LY_ExportImagesQuality = value; }
        public bool CO_IsExport { get => m_CO_IsExport; set => m_CO_IsExport = value; }
        public bool CO_SameScaleForViews { get => m_CO_SameScaleForViews; set => m_CO_SameScaleForViews = value; }
        public bool CO_TransformScreenLines3DToCylinders3D { get => m_CO_bTransformScreenLines3DToCylinders3D; set => m_CO_bTransformScreenLines3DToCylinders3D = value; }
        public float CO_DisplayIn3DRatio { get => m_CO_DisplayIn3DRatio; set => m_CO_DisplayIn3DRatio = value; }
        public int CO_RotateModelX { get => m_CO_RotateModelX; set => m_CO_RotateModelX = value; }
        public int CO_RotateModelY { get => m_CO_RotateModelY; set => m_CO_RotateModelY = value; }
        public int CO_RotateModelZ { get => m_CO_RotateModelZ; set => m_CO_RotateModelZ = value; }
        public int CO_ModelView { get => m_CO_ModelView; set => m_CO_ModelView = value; }
        public int CO_ViewModelMembers { get => m_CO_ViewModelMembers; set => m_CO_ViewModelMembers = value; }
        public int CO_ViewCladding { get => m_CO_ViewCladding; set => m_CO_ViewCladding = value; }
        public bool CO_UseOrtographicCamera { get => m_CO_bUseOrtographicCamera; set => m_CO_bUseOrtographicCamera = value; }
        public double CO_OrtographicCameraWidth { get => m_CO_OrtographicCameraWidth; set => m_CO_OrtographicCameraWidth = value; }
        public bool CO_CreateHorizontalGridlines { get => m_CO_bCreateHorizontalGridlines; set => m_CO_bCreateHorizontalGridlines = value; }
        public bool CO_CreateVerticalGridlinesFront { get => m_CO_bCreateVerticalGridlinesFront; set => m_CO_bCreateVerticalGridlinesFront = value; }
        public bool CO_CreateVerticalGridlinesBack { get => m_CO_bCreateVerticalGridlinesBack; set => m_CO_bCreateVerticalGridlinesBack = value; }
        public bool CO_CreateVerticalGridlinesLeft { get => m_CO_bCreateVerticalGridlinesLeft; set => m_CO_bCreateVerticalGridlinesLeft = value; }
        public bool CO_CreateVerticalGridlinesRight { get => m_CO_bCreateVerticalGridlinesRight; set => m_CO_bCreateVerticalGridlinesRight = value; }
        public int FO_ReinforcementBarColor_Top_x_Index { get => m_FO_ReinforcementBarColor_Top_x_Index; set => m_FO_ReinforcementBarColor_Top_x_Index = value; }
        public int FO_ReinforcementBarColor_Top_y_Index { get => m_FO_ReinforcementBarColor_Top_y_Index; set => m_FO_ReinforcementBarColor_Top_y_Index = value; }
        public int FO_ReinforcementBarColor_Bottom_x_Index { get => m_FO_ReinforcementBarColor_Bottom_x_Index; set => m_FO_ReinforcementBarColor_Bottom_x_Index = value; }
        public int FO_ReinforcementBarColor_Bottom_y_Index { get => m_FO_ReinforcementBarColor_Bottom_y_Index; set => m_FO_ReinforcementBarColor_Bottom_y_Index = value; }

        #endregion Properties

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DisplayOptionsViewModel() { }

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
            DisplayFloorSlabWireFrame = newVM.DisplayFloorSlabWireFrame;
            DisplayCladdingWireFrame = newVM.DisplayCladdingWireFrame;
            DisplayFibreglassWireFrame = newVM.DisplayFibreglassWireFrame;
            DisplayDoorsWireFrame = newVM.DisplayDoorsWireFrame;
            DisplayWindowsWireFrame = newVM.DisplayWindowsWireFrame;

            DoorsSimpleSolidModel = newVM.DoorsSimpleSolidModel;
            DoorsSimpleWireframe = newVM.DoorsSimpleWireframe;
            WindowOutlineOnly = newVM.WindowOutlineOnly;

            DisplayCladding = newVM.DisplayCladding;
            DisplayCladdingLeftWall = newVM.DisplayCladdingLeftWall;
            DisplayCladdingRightWall = newVM.DisplayCladdingRightWall;
            DisplayCladdingFrontWall = newVM.DisplayCladdingFrontWall;
            DisplayCladdingBackWall = newVM.DisplayCladdingBackWall;
            DisplayCladdingRoof = newVM.DisplayCladdingRoof;
            //DisplayIndividualCladdingSheets = newVM.DisplayIndividualCladdingSheets;

            DisplayFibreglass = newVM.DisplayFibreglass;
            DisplayDoors = newVM.DisplayDoors;
            DisplayWindows = newVM.DisplayWindows;
            DisplayNodalSupports = newVM.DisplayNodalSupports;
            DisplayMembersCenterLines = newVM.DisplayMembersCenterLines;
            DisplaySolidModel = newVM.DisplaySolidModel;
            DisplayWireFrameModel = newVM.DisplayWireFrameModel;
            DisplayDistinguishedColorMember = newVM.DisplayDistinguishedColorMember;
            //DisplayTransparentModelMember = newVM.DisplayTransparentModelMember;
            ColorsAccordingToMembersPrefix = newVM.ColorsAccordingToMembersPrefix;
            ColorsAccordingToMembersPosition = newVM.ColorsAccordingToMembersPosition;
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

            DisplayCladdingDescription = newVM.DisplayCladdingDescription;
            DisplayCladdingID = newVM.DisplayCladdingID;
            DisplayCladdingPrefix = newVM.DisplayCladdingPrefix;
            DisplayCladdingLengthWidth = newVM.DisplayCladdingLengthWidth;
            DisplayCladdingArea = newVM.DisplayCladdingArea;
            DisplayCladdingUnits = newVM.DisplayCladdingUnits;

            DisplayFibreglassDescription = newVM.DisplayFibreglassDescription;
            DisplayFibreglassID = newVM.DisplayFibreglassID;
            DisplayFibreglassPrefix = newVM.DisplayFibreglassPrefix;
            DisplayFibreglassLengthWidth = newVM.DisplayFibreglassLengthWidth;
            DisplayFibreglassArea = newVM.DisplayFibreglassArea;
            DisplayFibreglassUnits = newVM.DisplayFibreglassUnits;

            DisplayDoorDescription = newVM.DisplayDoorDescription;
            DisplayDoorID = newVM.DisplayDoorID;
            DisplayDoorType = newVM.DisplayDoorType;
            DisplayDoorHeightWidth = newVM.DisplayDoorHeightWidth;
            DisplayDoorArea = newVM.DisplayDoorArea;
            DisplayDoorUnits = newVM.DisplayDoorUnits;

            DisplayWindowDescription = newVM.DisplayWindowDescription;
            DisplayWindowID = newVM.DisplayWindowID;
            DisplayWindowHeightWidth = newVM.DisplayWindowHeightWidth;
            DisplayWindowArea = newVM.DisplayWindowArea;
            DisplayWindowUnits = newVM.DisplayWindowUnits;

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

            FrontCladdingOpacity = newVM.FrontCladdingOpacity;
            LeftCladdingOpacity = newVM.LeftCladdingOpacity;
            RoofCladdingOpacity = newVM.RoofCladdingOpacity;
            FlashingOpacity = newVM.FlashingOpacity;
            DoorPanelOpacity = newVM.DoorPanelOpacity;
            WindowPanelOpacity = newVM.WindowPanelOpacity;
            FibreglassOpacity = newVM.FibreglassOpacity;

            BackgroundColorIndex = newVM.BackgroundColorIndex;

            FloorSlabTextSize = newVM.FloorSlabTextSize;
            GridlinesSize = newVM.GridlinesSize;
            GridLineLabelSize = newVM.GridLineLabelSize;
            SectionSymbolsSize = newVM.SectionSymbolsSize;
            SectionSymbolLabelSize = newVM.SectionSymbolLabelSize;
            DetailSymbolSize = newVM.DetailSymbolSize;
            DetailSymbolLabelSize = newVM.DetailSymbolLabelSize;
            MembersDescriptionSize = newVM.MembersDescriptionSize;
            NodesDescriptionSize = newVM.NodesDescriptionSize;
            SawCutTextSize = newVM.SawCutTextSize;
            ControlJointTextSize = newVM.ControlJointTextSize;
            FoundationTextSize = newVM.FoundationTextSize;
            DimensionsTextSize = newVM.DimensionsTextSize;
            DimensionsLineRadius = newVM.DimensionsLineRadius;
            DimensionsScale = newVM.DimensionsScale;
            DescriptionTextWidthScaleFactor = newVM.DescriptionTextWidthScaleFactor;

            UseTextures = newVM.UseTextures;
            UseTexturesMembers = newVM.UseTexturesMembers;
            UseTexturesPlates = newVM.UseTexturesPlates;
            UseTexturesCladding = newVM.UseTexturesCladding;
            ColoredCenterlines = newVM.ColoredCenterlines;
            CladdingSheetColoursByID = newVM.CladdingSheetColoursByID;

            UseDifColorForSheetWithOverlap = newVM.UseDifColorForSheetWithOverlap;
            CladdingSheetColorIndex = newVM.CladdingSheetColorIndex;
            FibreglassSheetColorIndex = newVM.FibreglassSheetColorIndex;

            IsSetFromCode = false;
        }

        public void SetViewModel_CODE(DisplayOptionsViewModel newVM)
        {
            SetViewModel(newVM);

            // tieto sa nastavia iba takto, ale bezne sa uz bude pouzivat iba SetViewModel metoda
            LY_ViewsPageSize = newVM.LY_ViewsPageSize;
            LY_ExportImagesQuality = newVM.LY_ExportImagesQuality;

            CO_IsExport = newVM.CO_IsExport;
            CO_SameScaleForViews = newVM.CO_SameScaleForViews;

            CO_TransformScreenLines3DToCylinders3D = newVM.CO_TransformScreenLines3DToCylinders3D;
            CO_DisplayIn3DRatio = newVM.CO_DisplayIn3DRatio;
            CO_RotateModelX = newVM.CO_RotateModelX;
            CO_RotateModelY = newVM.CO_RotateModelY;
            CO_RotateModelZ = newVM.CO_RotateModelZ;
            CO_ModelView = newVM.CO_ModelView;
            CO_ViewModelMembers = newVM.CO_ViewModelMembers;
            CO_ViewCladding = newVM.CO_ViewCladding;
            CO_UseOrtographicCamera = newVM.CO_UseOrtographicCamera;
            CO_OrtographicCameraWidth = newVM.CO_OrtographicCameraWidth;

            CO_CreateHorizontalGridlines = newVM.CO_CreateHorizontalGridlines;
            CO_CreateVerticalGridlinesFront = newVM.CO_CreateVerticalGridlinesFront;
            CO_CreateVerticalGridlinesBack = newVM.CO_CreateVerticalGridlinesBack;
            CO_CreateVerticalGridlinesLeft = newVM.CO_CreateVerticalGridlinesLeft;
            CO_CreateVerticalGridlinesRight = newVM.CO_CreateVerticalGridlinesRight;

            // TODO 701 - Presunut nastavenie farieb z Footing Options do Display options
            FO_ReinforcementBarColor_Top_x_Index = newVM.FO_ReinforcementBarColor_Top_x_Index;
            FO_ReinforcementBarColor_Top_y_Index = newVM.FO_ReinforcementBarColor_Top_y_Index;
            FO_ReinforcementBarColor_Bottom_x_Index = newVM.FO_ReinforcementBarColor_Bottom_x_Index;
            FO_ReinforcementBarColor_Bottom_y_Index = newVM.FO_ReinforcementBarColor_Bottom_y_Index;

            
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
    }
}