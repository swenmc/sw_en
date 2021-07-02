using System.Windows.Media;

namespace BaseClasses
{
    public struct DisplayOptions
    {
        // General
        public bool bUseLightDirectional;
        public bool bUseLightPoint;
        public bool bUseLightSpot;
        public bool bUseLightAmbient;

        public bool bUseDiffuseMaterial;
        public bool bUseEmissiveMaterial;

        public bool bDisplayGlobalAxis;
        public bool bDisplayLocalMembersAxis;
        public bool bDisplaySurfaceLoadAxis;

        // Objects
        public bool bDisplayMembers;
        public bool bDisplayJoints;
        public bool bDisplayPlates;
        public bool bDisplayConnectors;
        public bool bDisplayNodes;
        public bool bDisplayFoundations;
        public bool bDisplayReinforcementBars;
        public bool bDisplayFloorSlab;
        public bool bDisplaySawCuts;
        public bool bDisplayControlJoints;
        public bool bDisplayNodalSupports;

        public bool bDisplayCladding;
        public bool bDisplayCladdingLeftWall;
        public bool bDisplayCladdingRightWall;
        public bool bDisplayCladdingFrontWall;
        public bool bDisplayCladdingBackWall;
        public bool bDisplayCladdingRoof;
        public bool bDisplayFibreglass;
        public bool bDisplayDoors;
        public bool bDisplayWindows;

        public bool bDisplayMembersCenterLines;
        public bool bDisplaySolidModel;

        public bool bDisplayWireFrameModel;
        public bool bDisplayMembersWireFrame;
        public bool bDisplayJointsWireFrame;
        public bool bDisplayPlatesWireFrame;
        public bool bDisplayConnectorsWireFrame;
        //public bool bDisplayNodesWireFrame; // Nie je v GUI
        public bool bDisplayFoundationsWireFrame;
        public bool bDisplayReinforcementBarsWireFrame;
        public bool bDisplayFloorSlabWireFrame;
        //public bool bDisplaySlabRebateWireFrame; // Nie je v GUI
        public bool bDisplayCladdingWireFrame;
        public bool bDisplayFibreglassWireFrame;
        public bool bDisplayDoorsWireFrame;
        public bool bDisplayWindowsWireFrame;

        public bool bDoorsSimpleSolidModel;
        public bool bDoorsSimpleWireframe;
        public bool bWindowOutlineOnly;

        public bool bDisplayMemberDescription;
        public bool bDisplayMemberID;
        public bool bDisplayMemberPrefix;
        public bool bDisplayMemberCrossSectionStartName;
        public bool bDisplayMemberRealLength;
        public bool bDisplayMemberRealLengthInMM;
        public bool bDisplayMemberRealLengthUnit;

        public bool bDisplayNodesDescription;

        public bool bDisplayFoundationsDescription;
        public bool bDisplayFloorSlabDescription;
        public bool bDisplaySawCutsDescription;
        public bool bDisplayControlJointsDescription;
        public bool bDisplayDimensions;
        public bool bDisplayGridlines;
        public bool bDisplaySectionSymbols;
        public bool bDisplayDetailSymbols;
        public bool bDisplaySlabRebates;

        public bool bDisplayCladdingDescription;
        public bool bDisplayCladdingID;
        public bool bDisplayCladdingPrefix;
        public bool bDisplayCladdingLengthWidth;
        public bool bDisplayCladdingArea;
        public bool bDisplayCladdingUnits;

        public bool bDisplayFibreglassDescription;
        public bool bDisplayFibreglassID;
        public bool bDisplayFibreglassPrefix;
        public bool bDisplayFibreglassLengthWidth;
        public bool bDisplayFibreglassArea;
        public bool bDisplayFibreglassUnits;

        public bool bDisplayDoorDescription;
        public bool bDisplayDoorID;
        public bool bDisplayDoorType;
        public bool bDisplayDoorHeightWidth;
        public bool bDisplayDoorArea;
        public bool bDisplayDoorUnits;

        public bool bDisplayWindowDescription;
        public bool bDisplayWindowID;
        public bool bDisplayWindowHeightWidth;
        public bool bDisplayWindowArea;
        public bool bDisplayWindowUnits;

        // Colours
        public Color wireFrameColor;
        public float fWireFrameLineThickness;

        public Color NodeColor;
        public Color NodeDescriptionTextColor;

        public Color MemberDescriptionTextColor;

        public Color DimensionTextColor;
        public Color DimensionLineColor;

        public Color GridLineLabelTextColor;
        public Color GridLineColor;
        public ELinePatternType GridLinePatternType;

        public Color SectionSymbolLabelTextColor;
        public Color SectionSymbolColor;
        //public ELinePatternType SectionSymbolPatternType;

        public Color DetailSymbolLabelTextColor;
        public Color? DetailSymbolLabelBackColor;
        public Color DetailSymbolColor;
        //public ELinePatternType DetailSymbolPatternType;

        public Color SawCutTextColor;
        public Color SawCutLineColor;
        public ELinePatternType SawCutLinePatternType;

        public Color ControlJointTextColor;
        public Color ControlJointLineColor;
        public ELinePatternType ControlJointLinePatternType;

        public Color FoundationTextColor;

        public Color FloorSlabTextColor;

        public Color FoundationColor;

        public Color FloorSlabColor;

        public Color SlabRebateColor;

        public Color memberCenterlineColor;
        public float fmemberCenterlineThickness;

        public Color CladdingTextColor;
        public Color FibreglassTextColor;
        public Color DoorTextColor;
        public Color WindowTextColor;

        public Color backgroundColor;

        public Color PlateColor;
        public Color ScrewColor;
        public Color AnchorColor;
        public Color WasherColor;
        public Color NutColor;
        public Color ReinforcementBarColor_Top_x;
        public Color ReinforcementBarColor_Top_y;
        public Color ReinforcementBarColor_Bottom_x;
        public Color ReinforcementBarColor_Bottom_y;

        public bool bColorsAccordingToMembersPrefix;
        public bool bColorsAccordingToMembersPosition;
        public bool bColorsAccordingToMembers;
        public bool bColorsAccordingToSections;

        public bool bDistinguishedColor; // Distinquished color of front and back area of member (slower), if false -> one color of whole member (faster)
        public bool bColoredCenterlines;

        public bool bCladdingSheetColoursByID;
        public bool bUseDistColorOfSheetWithoutOverlap;

        public Color CladdingSheetNoOverlapColor;
        public Color FibreglassSheetNoOverlapColor;

        public bool bUseTextures;
        public bool bUseTexturesMembers;
        public bool bUseTexturesPlates;
        public bool bUseTexturesCladding;

        // Opacity
        public float fMemberSolidModelOpacity;
        public float fPlateSolidModelOpacity;
        public float fScrewSolidModelOpacity;
        public float fAnchorSolidModelOpacity;
        public float fFoundationSolidModelOpacity;
        public float fReinforcementBarSolidModelOpacity;
        public float fFloorSlabSolidModelOpacity;
        public float fSlabRebateSolidModelOpacity;

        public float fFrontCladdingOpacity;
        public float fLeftCladdingOpacity;
        public float fRoofCladdingOpacity;
        public float fFlashingOpacity;
        public float fDoorPanelOpacity;
        public float fWindowPanelOpacity;
        public float fFibreglassOpacity;

        // Loads
        public bool bDisplayLoads;
        public bool bDisplayNodalLoads;
        public bool bDisplayMemberLoads;
        public bool bDisplayMemberLoads_Girts;
        public bool bDisplayMemberLoads_Purlins;
        public bool bDisplayMemberLoads_EavePurlins;
        public bool bDisplayMemberLoads_WindPosts;
        public bool bDisplayMemberLoads_Frames;
        public bool bDisplaySurfaceLoads;

        public bool bDisplayLoadsLabels;
        public bool bDisplayLoadsLabelsUnits;

        public float LoadSizeScaleIn3D;

        // Texts and Symbols
        public float FloorSlabTextSize;
        public int fFloorSlabTextFontSize;

        public float GridlinesSize;
        public float GridLineLabelSize;
        public int fGridLineLabelTextFontSize;

        public float SectionSymbolsSize;
        public float SectionSymbolLabelSize;
        public int fSectionSymbolLabelTextFontSize;

        public float DetailSymbolSize;
        public float DetailSymbolLabelSize;
        public int fDetailSymbolLabelTextFontSize;

        public float MembersDescriptionSize;
        public int fMemberDescriptionTextFontSize;

        public float NodesDescriptionSize;
        public int fNodeDescriptionTextFontSize;

        public float SawCutTextSize;
        public int fSawCutTextFontSize;

        public float ControlJointTextSize;
        public int fControlJointTextFontSize;

        public float FoundationTextSize;
        public int fFoundationTextFontSize;

        public float DimensionsTextSize;
        public float DimensionsLineRadius;
        public float DimensionsScale;
        public int fDimensionTextFontSize;

        public float DescriptionTextWidthScaleFactor;

        public float CladdingDescriptionSize;
        public float FibreglassDescriptionSize;
        public float DoorDescriptionSize;
        public float WindowDescriptionSize;

        // Properties defined only for export - layouts (LY - layout)
        public EPageSizes LY_ViewsPageSize;
        public EImagesQuality LY_ExportImagesQuality;

        // Properties defined only in source code (CO - code only)
        public int CO_View;
        public int CO_ModelFilter;
        public bool CO_IsExport;
        public bool CO_SameScaleForViews;
        public bool CO_bTransformScreenLines3DToCylinders3D;
        public int CO_RotateModelX;
        public int CO_RotateModelY;
        public int CO_RotateModelZ;
        public int CO_ViewCladding;
        public bool CO_bUseOrtographicCamera;
        public double CO_OrtographicCameraWidth;
        public bool CO_bCreateHorizontalGridlines;
        public bool CO_bCreateVerticalGridlinesFront;
        public bool CO_bCreateVerticalGridlinesBack;
        public bool CO_bCreateVerticalGridlinesLeft;
        public bool CO_bCreateVerticalGridlinesRight;

        public bool bMirrorPlate3D; // Nie je v GUI PFD - presunut do CO?, pouziva sa v SCV
    }

    public struct DisplayOptionsFootingPad2D
    {
        public bool bDrawFootingPad;
        public SolidColorBrush FootingPadColor;
        public double FootingPadThickness;

        public bool bDrawColumnOutline;
        public SolidColorBrush ColumnOutlineColor;
        public SolidColorBrush ColumnOutlineBehindColor;
        public SolidColorBrush ColumnOutlineAboveColor;
        public DashStyle ColumnOutlineBehindLineStyle;
        public double ColumnOutlineThickness;
        
        public bool bDrawBasePlate;
        public SolidColorBrush BasePlateColor;
        public double BasePlateThickness;

        public bool bDrawScrews;

        public bool bDrawHoles;
        public SolidColorBrush bHoleColor;
        public double HoleLineThickness;

        public bool bDrawHoleCentreSymbols;
        public SolidColorBrush bHoleCenterSymbolColor;
        public double HoleCenterSymbolLineThickness;

        public bool bDrawAnchors;
        public SolidColorBrush AnchorStrokeColor;
        public double AnchorLineThickness;

        public bool bDrawWashers;
        public SolidColorBrush WasherStrokeColor;
        public double WasherLineThickness;

        public bool bDrawNuts;
        public SolidColorBrush NutStrokeColor;
        public double NutLineThickness;

        public bool bDrawPerimeter;
        public SolidColorBrush PerimeterColor;
        public DashStyle PerimeterLineStyle;
        public double PerimeterThickness;

        public bool bDrawReinforcement;
        public SolidColorBrush ReinforcementInSectionFillColor;
        public SolidColorBrush ReinforcementInSectionStrokeColor;
        public double ReinforcementInSectionThickness;

        public SolidColorBrush ReinforcementInWiewColorTop;
        public double ReinforcementInViewThicknessTop;

        public SolidColorBrush ReinforcementInWiewColorBottom;
        public double ReinforcementInViewThicknessBottom;

        public SolidColorBrush ReinforcementInWiewColorStarter;
        public double ReinforcementInViewThicknessStarter;

        public bool bDrawReinforcementInSlab;
        public SolidColorBrush ReinforcementInSlabColor;
        public DashStyle ReinforcementInSlabLineStyle;
        public double ReinforcementInSlabThickness;

        public bool bDrawDPC_DPM;
        public SolidColorBrush DPC_DPMColor;
        public DashStyle DPC_DPMLineStyle;
        public double DPC_DPMThickness;

        public bool bDrawDimensions;
        public SolidColorBrush DimensionsLinesColor;
        public SolidColorBrush DimensionsTextColor;
        public double DimensionsThickness;

        public bool bDrawNotes;
        public SolidColorBrush NotesArrowFillColor;
        public SolidColorBrush NotesArrowStrokeColor;
        public double NotesThickness;
    }

    public struct BuildingDataInput
    {
        public ELocation location;               // City / Town
        public float fDesignLife_Value;          // Years
        public int iImportanceClass;             // Importance Level

        public float fAnnualProbabilityULS_Snow; // Annual Probability of Exceedence ULS - Snow
        public float fAnnualProbabilityULS_Wind; // Annual Probability of Exceedence ULS - Wind
        public float fAnnualProbabilityULS_EQ;   // Annual Probability of Exceedence ULS - EQ
        public float fAnnualProbabilitySLS;      // Annual Probability of Exceedence SLS

        public float fR_ULS_Snow;                // Number of years - ULS - Snow
        public float fR_ULS_Wind;                // Number of years - ULS - Wind
        public float fR_ULS_EQ;                  // Number of years - ULS - EQ
        public float fR_SLS;                     // Number of years - SLS

        public float fE;
    }

    public struct BuildingGeometryDataInput
    {
        public float fW_centerline;     // Width X -axis - centerlines
        public float fL_centerline;     // Length Y -axis - centerlines
        public float fH_1_centerline;   // Height of wall - centerlines
        public float fH_2_centerline;   // Height of building - centerlines

        public float fWidth_overall;
        public float fLength_overall;
        public float fHeight_1_overall; // Eave
        public float fHeight_2_overall; // Ridge

        public float fRoofPitch_deg;

        public int iMainColumnFlyBracingEveryXXGirt;
        public int iRafterFlyBracingEveryXXPurlin;
        public int iEdgePurlin_ILS_Number;
        public int iGirt_ILS_Number;
        public int iPurlin_ILS_Number;
        public int iFrontWindPostFlyBracingEveryXXGirt;
        public int iBackWindPostFlyBracingEveryXXGirt;
        public int iGirtFrontSide_ILS_Number;
        public int iGirtBackSide_ILS_Number;

        public int iRafterCanopyFlyBracingEveryXXPurlin;
        public int iPurlinCanopy_ILS_Number;
    }

    public struct SnowLoadDataInput
    {
        public ECountry eCountry;
        public ESnowRegion eSnowRegion; // Snow region // Wind region Cl 2.3 - Fig 2.2
        public ERoofExposureCategory eExposureCategory;
        public float fh_0_SiteElevation_meters;
    }

    public struct WindLoadDataInput
    {
        public EWindRegion eWindRegion; // Wind region // Wind region Cl 3.2 - Fig 3.1(A)
        public int iAngleWindDirection; // Clockwise angle between Notrth  cardinal direction (Beta = 0) and Theta = 0 (building side L)
        //public int iTerrainCategoryIndex;
        public float fTerrainCategory;  // float value 1-4 see Tab 4.2
        public float fInternalPressureCoefficientCpiMaximumPressure;
        public float fInternalPressureCoefficientCpiMaximumSuction;

        public float fLocalPressureFactorKl_Girt;
        public float fLocalPressureFactorKl_Purlin;
        public float fLocalPressureFactorKl_EavePurlin_Wall;
        public float fLocalPressureFactorKl_EavePurlin_Roof;
    }

    public struct WindLoadDataSpecificInput
    {
        public float fTributaryArea;
        //public float fz;
        //public float fh;

        public ELocalWindPressureReference eLocalPressureReferenceUpwind;
        public ELocalWindPressureReference eLocalPressureReferenceDownwind;

        public float fM_lee;
        public float fM_h;
        public float fM_s;

        public float fK_p;
        public float fK_ce_min;
        public float fK_ce_max;
        public float fK_ci_min;
        public float fK_ci_max;

        /*
        public float fRoofArea;
        public float fWallArea_0or180;
        public float fWallArea_90or270;
        */
    }

    public struct SeisLoadDataInput
    {
        public ESiteSubSoilClass eSiteSubsoilClass;
        public string sSiteSubsoilClass;
        public float fProximityToFault_D_km; // km
        public float fZoneFactor_Z;
        //public float fPeriodAlongXDirection_Tx; // sec
        //public float fPeriodAlongYDirection_Ty; // sec
        //public float fSpectralShapeFactor_Ch_Tx;
        //public float fSpectralShapeFactor_Ch_Ty;
    }

    public struct FreeSurfaceLoadsMemberTypeData
    {
        public EMemberType_FS memberType;
        public float fLoadingWidth;

        public FreeSurfaceLoadsMemberTypeData(EMemberType_FS memberType_temp, float fLoadingWidth_temp)
        {
            memberType = memberType_temp;
            fLoadingWidth = fLoadingWidth_temp;
        }
    }

    public struct FreeSurfaceLoadsMemberData
    {
        public CMember loadedMember;
        public float fLoadingArea; // Toto by sa asi mohlo zmazat za predpokladu ze vsetky pruty rovnakeho typu budu mat rovnaku zatazovaciu sirku, resp plochu (sirka = plocha A / dlzka pruta L) (nemusi to tak byt, ale teraz je to zjednodusene)

        public FreeSurfaceLoadsMemberData(CMember member_temp, float fLoadingArea_temp)
        {
            loadedMember = member_temp;
            fLoadingArea = fLoadingArea_temp;
        }
    }

    //public struct PropertiesToInsertOpening
    //{
    //    public string sBuildingSide;
    //    public int iBayNumber;
    //}





    // AS 1170.0 - AS 1170.5
    public struct loadInputComboboxIndexes
    {
        public int LocationIndex;
        public int ImportanceLevelIndex;
        public int DesignLifeIndex;
        public int ExposureCategoryIndex;
        public int SiteSubSoilClassIndex;
        public int TerrainCategoryIndex;
        public int AngleWindDirectionIndex;
    }

    public struct loadInputTextBoxValues
    {
        public float InternalPressureCoefficientCpiMaximumPressure;
        public float InternalPressureCoefficientCpiMaximumSuction;
        public float LocalPressureFactorKl_Girt; // Dalo by sa urcit automaticky
        public float LocalPressureFactorKl_Purlin; // Dalo by sa urcit automaticky
        public float LocalPressureFactorKl_EavePurlin_Wall; // Dalo by sa urcit automaticky
        public float LocalPressureFactorKl_EavePurlin_Roof; // Dalo by sa urcit automaticky
        public float SiteElevation;
        public float FaultDistanceDmin_km;
        public float FaultDistanceDmax_km;
        public float PeriodAlongXDirectionTx;
        public float PeriodAlongYDirectionTy;
        public float AdditionalDeadActionRoof;
        public float AdditionalDeadActionWall;
        public float ImposedActionRoof;
    }

    public struct basicInternalForces
    {
        public float fN;
        public float fV_yu;
        public float fV_zv;
        public float fV_yy;
        public float fV_zz;
        public float fT;
        public float fM_yu;
        public float fM_zv;
        public float fM_yy;
        public float fM_zz;
    }

    public struct basicDeflections
    {
        public float fDelta_yu;
        public float fDelta_zv;
        public float fDelta_yy;
        public float fDelta_zz;
        public float fDelta_tot;
    }

    public struct designDeflections
    {
        public float fDelta_yu;
        public float fDelta_zv;
        public float fDelta_yy;
        public float fDelta_zz;
        public float fDelta_tot;
    }

    public struct designInternalForces
    {
        // TODO - mozno je zbytocne mat tu 12 hodnot kedze zvycajne posudzujeme v jednom systeme bud geometrical axes alebo principal axes
        public float fN;
        public float fN_c;
        public float fN_t;
        public float fV_yu;
        public float fV_zv;
        public float fV_yy;
        public float fV_zz;
        public float fT;
        public float fM_yu;
        public float fM_zv;
        public float fM_yy;
        public float fM_zz;
    }

    // AS 4600
    public struct designMomentValuesForCb
    {
        public float fM_max;
        public float fM_14;
        public float fM_24;
        public float fM_34;
    }

    public struct designBucklingLengthFactors
    {
        public float fBeta_x_FB_fl_ex;
        public float fBeta_y_FB_fl_ey;
        public float fBeta_z_TB_TFB_l_ez;
        public float fBeta_LTB_fl_LTB;
    }
}
