namespace BaseClasses
{
    //----------------------------------------------------------------------------
    // Solution Type - Type of Model (geometry, solver, etc.)
    //----------------------------------------------------------------------------
    public enum ESLN
    {
        e1D_1D,  // 1D members - simple member - colum or beam / continuous beam
        e2DD_1D, // 1D members - 2D truss, 2D frame
        e3DD_1D, // 1D members - 2D truss, 2D frame
        e4DD_1D,  // 1D members - 3D + time

        e4DD_3D  // 3D members - 3D + time (only graphical model)
    }

    //----------------------------------------------------------------------------
    // Node DOF - number of degrees of freedom
    //----------------------------------------------------------------------------
    public enum ENDOF
    {
        e2DEnv = 3, // 3 for 2D environment
        e3DEnv = 6  // 6 for 3D environment - no warping effect (bimoment) // DOF in 3D
    }

    // Cartesian coordinate system, point in n-dimensional Euclidean space

    // In two dimensions

    /*Fixing or choosing the x-axis determines the y-axis up to direction. Namely, the y-axis is necessarily the perpendicular 
     * to the x-axis through the point marked 0 on the x-axis. But there is a choice of which of the two half lines on the perpendicular
     * to designate as positive and which as negative. Each of these two choices determines a different orientation (also called handedness) of the Cartesian plane.
    The usual way of orienting the axes, with the positive x-axis pointing right and the positive y-axis pointing up (and the x-axis being
     * the "first" and the y-axis the "second" axis) is considered the positive or standard orientation, also called the right-handed orientation.
    A commonly used mnemonic for defining the positive orientation is the right hand rule. Placing a somewhat closed right hand on the plane with the thumb pointing up,
     * the fingers point from the x-axis to the y-axis, in a positively oriented coordinate system.
    The other way of orienting the axes is following the left hand rule, placing the left hand on the plane with the thumb pointing up.
    When pointing the thumb away from the origin along an axis, the curvature of the fingers indicates a positive rotation along that axis.
    Regardless of the rule used to orient the axes, rotating the coordinate system will preserve the orientation. Switching any two axes will reverse the orientation.*/

    // In three dimensions

    /*
     Once the x- and y-axes are specified, they determine the line along which the z-axis should lie,
     but there are two possible directions on this line. The two possible coordinate systems which result are called 'right-handed' and 'left-handed'.
     The standard orientation, where the xy-plane is horizontal and the z-axis points up (and the x- and the y-axis form a positively oriented two-dimensional coordinate system
     in the xy-plane if observed from above the xy-plane) is called right-handed or positive.
     The name derives from the right-hand rule. If the index finger of the right hand is pointed forward, the middle finger bent inward at a right angle to it,
     and the thumb placed at a right angle to both, the three fingers indicate the relative directions of the x-, y-, and z-axes in a right-handed system.
     The thumb indicates the x-axis, the index finger the y-axis and the middle finger the z-axis. Conversely, if the same is done with the left hand, a left-handed system results.
     Because a three-dimensional object is represented on the two-dimensional screen, distortion and ambiguity result. The axis pointing downward (and to the right)
     is also meant to point towards the observer, whereas the "middle" axis is meant to point away from the observer.
     The red circle is parallel to the horizontal xy-plane and indicates rotation from the x-axis to the y-axis (in both cases).
     Hence the red arrow passes in front of the z-axis. */

    //----------------------------------------------------------------------------
    // Global Coordinate System
    //----------------------------------------------------------------------------
    public enum EGCS
    {
        eGCSRightHanded,
        eGCSLeftHanded
    }

    //----------------------------------------------------------------------------
    // Load Definition Coordinate System
    //----------------------------------------------------------------------------
    public enum ELoadCoordSystem
    {
        eGCS, // Nodes (members, surfaces)
        eLCS, // Members, surfaces
        ePCS  // Members
    }

    //----------------------------------------------------------------------------
    // Nodes
    //----------------------------------------------------------------------------
    public enum ENLoadType
    {
        eNLT_Fx,
        eNLT_Fy,
        eNLT_Fz,
        eNLT_Mx,
        eNLT_My,
        eNLT_Mz,
        eNLT_OTHER
    }
    //----------------------------------------------------------------------------
    public enum ENSupportType
    {
        eNST_Ux = 0,
        eNST_Uy = 1,
        eNST_Uz = 2,
        eNST_Rx = 3,
        eNST_Ry = 4,
        eNST_Rz = 5
    }
    //----------------------------------------------------------------------------
    public enum ENSupportType_2D
    {
        eNST_Ux = 0,
        eNST_Uy = 1,
        eNST_Rz = 2
    }

    // Nodes and Members
    //----------------------------------------------------------------------------
    // Define load direction or orientation in GCC (global coordinate system)
    //----------------------------------------------------------------------------
    public enum ELoadDir
    {
        eLD_X = 0,
        eLD_Y = 1,
        eLD_Z = 2
    }

    //----------------------------------------------------------------------------
    // Members
    //----------------------------------------------------------------------------
    public enum EM_IF
    {
        eFx,
        eFy,
        eFz,
        eMx,
        eMy,
        eMz
    }

    // Definition support conditions for members loading and local stiffeness matrix determination
    public enum EM_PCS_DIR1
    {
        eUXRX,
        eUYRZ,
        eUZRY
    }
    //----------------------------------------------------------------------------
    // Define load direction or orientation in LCC (local coordinate system) of members
    //----------------------------------------------------------------------------
    public enum EMLoadDirLCC1
    {
        eMLD_LCC_FX_MX = 0,
        eMLD_LCC_Y = 1,
        eMLD_LCC_Z = 2
    }
    //----------------------------------------------------------------------------
    // Define load direction or orientation in PCC (principal coordinate system) of members
    //----------------------------------------------------------------------------
    public enum EMLoadDirPCC1
    {
        eMLD_PCC_FXX_MXX = 0,
        eMLD_PCC_FYU_MZV = 1,
        eMLD_PCC_FZV_MYU = 2
    }

    //----------------------------------------------------------------------------
    // Intermediate Transverse Support Type (upper flange, bottom flange, both flanges, etc.)
    //----------------------------------------------------------------------------
    public enum EITSType
    {
        eBothFlanges,
        eUpperFlangeOnly,
        eBottomFlangeOnly,
        eNone,
        eRigid,
        eDefinedByValue
    }

    //----------------------------------------------------------------------------
    // Types of single member loading
    // Typy zatazenia jednoducheho pruta - podla druhu zatazenia, polohy a koncoveho podopretia prvku
    //----------------------------------------------------------------------------
    public enum EMLoadTypeDistr
    {
        // Pozri: Sobota Jan, Statika Stavebnych konstrukcii 2, Tab. 2.1

        // Singular loading
        eMLT_FS_G_11 = 11, // Singular Force - general position
        eMLT_FS_H_12 = 12, // Singular Force - in half of member length
        //eMLT_MS_G_13 = 13, // Singular Moment - general position
        //eMLT_M_SH_14 = 14, // Singular Moment - in half of member length

        // Uniform loading
        // Whole Member
        eMLT_QUF_W_21 = 21,  // Uniformly Distributed Force - whole length of member

        // Part of Member
        eMLT_QUF_PA_22 = 22, // Uniformly Distributed Force - partly from begining of member
        eMLT_QUF_PB_23 = 23, // Uniformly Distributed Force - partly from end of member
        eMLT_QUF_PG_24 = 24, // Uniformly Distributed Force - partly in general position on member

        // Triangular loading
        // Whole Member
        eMLT_QTNF_SW_31 = 31,   // Triangular load, symmetrical acc. to half of length of member
        eMLT_QTNF_MA_W_32 = 32, // Triangular load on whole member, maximum at start
        eMLT_QTNF_MB_W_33 = 33, // Triangular load on whole member, maximum at end

        // Part of Member
        eMLT_QTNF_MA_P_34 = 34, // Triangular load on part of member, maximum near to the start
        eMLT_QTNF_MB_P_35 = 35, // Triangular load on part of member, maximum near to the end
        eMLT_QTNF_SP_36 = 36,   // Triangular load, symmetrical acc. to transversal axis of member, in general position

        // Trapezoidal loading
        // Whole Member
        eMLT_QTPF_SW_41 = 41,   // Trapezoidal load, symmetrical acc. to half of member, apllied on whole member

        // Temperature
        // Whole Member
        eMLT_TMP_UNQ_Wz_51 = 51,   // Temperature loading on whole member, two temperatures (different for upper and bottom surface of member)
        eMLT_TMP_UNQ_Wy_52 = 52    // Temperature loading on whole member, two temperatures (different for upper and bottom surface of member)
    }
    //----------------------------------------------------------------------------
    // Types of surface loading
    // Typy zatazenia plochy
    //----------------------------------------------------------------------------
    public enum ESLoadTypeDistr
    {
        eSLT_QUF_W
    }
    //----------------------------------------------------------------------------
    // Define load type
    //----------------------------------------------------------------------------
    public enum EMLoadType
    {
        eMLT_F = 0, // Force
        eMLT_M = 1, // Moment
        eMLD_Temperature = 2 // Temperature
    }

    public enum ESLoadType
    {
        eSLT_F = 0, // Force
        /*
        eSLT_M = 1, // Moment
        eSLD_Temperature = 2 // Temperature
        */
    }

    //----------------------------------------------------------------------------
    // Define volume shape type (3D)
    //----------------------------------------------------------------------------
    public enum EVolumeShapeType
    {
        eShape3DPrism_8Edges = 0, // Rectangular Prism
        eShape3DCube = 1,         // Cube
        eShape3D_OctagonalPrism,
        eShape3D_TriangularPrism,
        eShape3D_PentagonalPrism,
        eShape3D_Hexagonalprism,
        eShape3D_TriangularPyramid,
        eShape3D_RectangularPyramid,
        eShape3D_SquarePyramid,
        eShape3D_HexagonalPyramid,
        eShape3D_NPyramid,
        eShape3D_Cylinder,
        eShape3D_Frustum,
        eShape3D_Cone,
        eShape3D_Sphere,
        eShape3D_Ellipsoid,
        eShape3D_Torus,
        eShape3D_Tetrahedron,
        eShape3D_Pentahedron,
        eShape3D_Octahedron,
        eShape3D_Docecahedron,
        eShape3D_Icosahedron
    }

    //----------------------------------------------------------------------------
    // Define limit state type
    //----------------------------------------------------------------------------
    public enum ELSType
    {
        eLS_ULS = 0, // ULS (include strength, stability, Fire, EQ, ...)
        eLS_SLS = 1  // SLS
    }

    //----------------------------------------------------------------------------
    // Define load case group type
    //----------------------------------------------------------------------------
    public enum ELCGTypeForLimitState
    {
        eUniversal,
        eULSOnly,
        eSLSOnly
    }

    //----------------------------------------------------------------------------
    // Define load case group type by limit state
    //----------------------------------------------------------------------------
    public enum ELCGType
    {
        // Load groups define "how the individual load cases may be combined together" if inserted into a load case combination
        eStandard,    // do Load Combination moze ist jedna alebo viacero rozne nakombinovanych load cases z jednej load group, kombinuje sa kazda s kazdou a prenasobene su potom v danej kombinacii jednym faktorom (faktor * LCi) alebo faktor * (LCi + LCii) alebo faktor * (LCi + LCiii) alebo faktor * (LCi + LCii + LCiii)
        eTogether,    // do Load Combination sa pridaju vzdy vsetky Load Case z Load Case Group - faktor * (LCi + LCii + LCiii)
        eExclusive    // do Load Combination sa prida vzdy len jeden Load Case z Load Case Group - faktor * LCi) alebo (faktor * LCii) alebo (faktor * LCiii)
    }

    //----------------------------------------------------------------------------
    // Define load case type
    //----------------------------------------------------------------------------
    public enum ELCType
    {
        ePermanentLoad,  // Permanent or dead load
        eImposedLoad_ST, // Short-term
        eImposedLoad_LT, // Long-term
        eSnow,
        eWind,
        eEarthquake
    }

    //----------------------------------------------------------------------------
    // Define load case load source position or direction in global coordinate system (wind, earthquake)
    //----------------------------------------------------------------------------
    public enum ELCMainDirection
    {
        eUndefined = 6,
        eGeneral   = 7,
        eMinusX    = 0, // (load acting in -X) Theta = 000 deg (see AS/NZS 1170.2, Figure 2.2)
        ePlusX     = 2, // (load acting in +X) Theta = 180 deg (see AS/NZS 1170.2, Figure 2.2)
        eMinusY    = 3, // (load acting in -Y) Theta = 270 deg (see AS/NZS 1170.2, Figure 2.2)
        ePlusY     = 1, // (load acting in +Y) Theta = 090 deg (see AS/NZS 1170.2, Figure 2.2)
        eMinusZ    = 4,
        ePlusZ     = 5
    }

    //----------------------------------------------------------------------------
    // Define load case name
    //----------------------------------------------------------------------------
    public enum ELCName
    {
    eDL_G,                                  // "Dead load G"
    eIL_Q,                                  // "Imposed load Q"
    
    eSL_Su_Full,                            // "Snow load Su - full"
    eSL_Su_Left,                            // "Snow load Su - left"
    eSL_Su_Right,                           // "Snow load Su - right"
    eWL_Wu_Cpi_min_Left_X_Plus,             // "Wind load Wu - Cpi,min - Left - X+"
    eWL_Wu_Cpi_min_Right_X_Minus,           // "Wind load Wu - Cpi,min - Right - X-"
    eWL_Wu_Cpi_min_Front_Y_Plus,            // "Wind load Wu - Cpi,min - Front - Y+"
    eWL_Wu_Cpi_min_Rear_Y_Minus,            // "Wind load Wu - Cpi,min - Rear - Y-"
    eWL_Wu_Cpi_max_Left_X_Plus,             // "Wind load Wu - Cpi,max - Left - X+"
    eWL_Wu_Cpi_max_Right_X_Minus,           // "Wind load Wu - Cpi,max - Right - X-"
    eWL_Wu_Cpi_max_Front_Y_Plus,            // "Wind load Wu - Cpi,max - Front - Y+"
    eWL_Wu_Cpi_max_Rear_Y_Minus,            // "Wind load Wu - Cpi,max - Rear - Y-"
    eWL_Wu_Cpe_min_Left_X_Plus,             // "Wind load Wu - Cpe,min - Left - X+"
    eWL_Wu_Cpe_min_Right_X_Minus,           // "Wind load Wu - Cpe,min - Right - X-"
    eWL_Wu_Cpe_min_Front_Y_Plus,            // "Wind load Wu - Cpe,min - Front - Y+"
    eWL_Wu_Cpe_min_Rear_Y_Minus,            // "Wind load Wu - Cpe,min - Rear - Y-"
    eWL_Wu_Cpe_max_Left_X_Plus,             // "Wind load Wu - Cpe,max - Left - X+"
    eWL_Wu_Cpe_max_Right_X_Minus,           // "Wind load Wu - Cpe,max - Right - X-"
    eWL_Wu_Cpe_max_Front_Y_Plus,            // "Wind load Wu - Cpe,max - Front - Y+"
    eWL_Wu_Cpe_max_Rear_Y_Minus,            // "Wind load Wu - Cpe,max - Rear - Y-"
    eEQ_Eu_Left_X_Plus,                     // "Earthquake load Eu - X"
    eEQ_Eu_Front_Y_Plus,                    // "Earthquake load Eu - Y"

    eSL_Ss_Full,                            // "Snow load Ss - full"
    eSL_Ss_Left,                            // "Snow load Ss - left"
    eSL_Ss_Right,                           // "Snow load Ss - right"
    eWL_Ws_Cpi_min_Left_X_Plus,             // "Wind load Ws - Cpi,min - Left - X+"
    eWL_Ws_Cpi_min_Right_X_Minus,           // "Wind load Ws - Cpi,min - Right - X-"
    eWL_Ws_Cpi_min_Front_Y_Plus,            // "Wind load Ws - Cpi,min - Front - Y+"
    eWL_Ws_Cpi_min_Rear_Y_Minus,            // "Wind load Ws - Cpi,min - Rear - Y-"
    eWL_Ws_Cpi_max_Left_X_Plus,             // "Wind load Ws - Cpi,max - Left - X+"
    eWL_Ws_Cpi_max_Right_X_Minus,           // "Wind load Ws - Cpi,max - Right - X-"
    eWL_Ws_Cpi_max_Front_Y_Plus,            // "Wind load Ws - Cpi,max - Front - Y+"
    eWL_Ws_Cpi_max_Rear_Y_Minus,            // "Wind load Ws - Cpi,max - Rear - Y-"
    eWL_Ws_Cpe_min_Left_X_Plus,             // "Wind load Ws - Cpe,min - Left - X+"
    eWL_Ws_Cpe_min_Right_X_Minus,           // "Wind load Ws - Cpe,min - Right - X-"
    eWL_Ws_Cpe_min_Front_Y_Plus,            // "Wind load Ws - Cpe,min - Front - Y+"
    eWL_Ws_Cpe_min_Rear_Y_Minus,            // "Wind load Ws - Cpe,min - Rear - Y-"
    eWL_Ws_Cpe_max_Left_X_Plus,             // "Wind load Ws - Cpe,max - Left - X+"
    eWL_Ws_Cpe_max_Right_X_Minus,           // "Wind load Ws - Cpe,max - Right - X-"
    eWL_Ws_Cpe_max_Front_Y_Plus,            // "Wind load Ws - Cpe,max - Front - Y+"
    eWL_Ws_Cpe_max_Rear_Y_Minus,            // "Wind load Ws - Cpe,max - Rear - Y-"
    eEQ_Es_Left_X_Plus,                     // "Earthquake load Es - X"
    eEQ_Es_Front_Y_Plus                     // "Earthquake load Es - Y"
    }

    //----------------------------------------------------------------------------
    // Define window shape type (3D)
    //----------------------------------------------------------------------------
    public enum EWindowShapeType
    {
        eClassic = 0, // Rectangular Window - many segments
        eRound = 1    // Round
    }

    //----------------------------------------------------------------------------
    // Define connection component type
    //----------------------------------------------------------------------------
    public enum EConnectionComponentType
    {
        ePlate = 0, // Plate
        eBolt = 1,  // Bolt
        eScrew = 2
    }

    public enum ESerieTypePlate
    {
        eSerie_B,
        eSerie_L,
        eSerie_LL,
        eSerie_F,
        eSerie_Q,
        eSerie_S,
        eSerie_T,
        eSerie_X,
        eSerie_Y,
        eSerie_J,
        eSerie_K,
        eSerie_N,
        eSerie_O
    };

    public enum ESerieTypeCrSc_FormSteel
    {
        eSerie_Box_10075,
        eSerie_Z,
        eSerie_C_single,
        eSerie_C_back_to_back,
        eSerie_C_nested,
        eSerie_Box_63020,
        eSerie_SmartDek,
        eSerie_PurlinDek
    };

    public enum EMemberType_FormSteel
    {
        eG,  // Girt
        eC,  // Column
        eER, // End Rafter
        eWP, // Wind Post
        eEC, // End Column
        eEP, // Edge Purlin
        eP,  // Purlin
        eDT, // Door Trimmer
        eDL, // Door Lintel
        ePB, // Purlin Block
        eGB, // Girt Block
        eDF, // Door Frame
        eBG, // Base Girt
        eMR, // Main Rafter
        eMC, // Main Column
    }

    public enum ECountry
    {
        eAustralia,
        eNewZealand
    }

    public enum ELocation
    {
        eAuckland,
        eWellington,
        eChristchurch,
        eHamilton,
        eTauranga,
        eNapier_Hastings,
        eDunedin,
        ePalmerston_North,
        eNelson,
        eRotorua,
        eWhangarei,
        eNew_Plymouth,
        eInvercargill,
        eWhanganui,
        eGibsborne
    }

    public enum EWindRegion
    {
        eA1,
        eA2,
        eA3,
        eA4,
        eA5,
        eA6,
        eA7,
        eW,
        eB,
        eC,
        eD
    }

    public enum ELocalWindPressureReference
    {
        eUndefined = 0,
        eWA1,
        eRC1,
        eRA1 = 1, // Temp - docasne
        eRA2 = 2, // Temp - docasne
        eRA3 = 3,
        eRA4 = 4,
        eSA1,
        eSA2,
        eSA3,
        eSA4,
        eSA5
    }

    public enum ESnowRegion
    {
        eN0,
        eN1,
        eN2,
        eN3,
        eN4,
        eN5
    }

    public enum ESiteSubSoilClass
    {
        eA, // Strong rock
        eB, // Rock
        eC, // Shallow soil
        eD, // Deep and soft soil
        eE  // Very soft soil
    }

    public enum EPlateNumberAndPositionInJoint
    {
        eOneLeftPlate,       // 1 plate on the left side of cross-section (- y local axis)
        eOneRightPlate,      // 1 plate on the right side of cross-section (+ y local axis)
        eTwoPlates           // 2 plates in joint (one on the left and one on the right side of cross-section)
    }

    public enum ESnowElevationRegions
    {
        eNoSignificantSnow,  // No significant snowfall.
        eSubAlpine,          // Sub-alpine - Regions where the maximum snow load is usually due to a single snowfall.
        eAlpine              // Alpine - Regions where the maximum snow load is usually due to accumulation from a number of successive snowfalls.
    }

    public enum ERoofExposureCategory
    {
        // See AS / NZS 1170.3 - cl. 4.2.2 - exposure reduction coefficient (Ce)
        eSheltered,          // (a)Sheltered Sites where the roof is protected from the wind by obstructions such as other structures, terrain features or numbers of closely spaced trees higher than the roof.
        eSemiSheltered,      // (b) Semi-sheltered Sites where the roof is only partially protected by numbers of scattered obstructions higher than the roof(e.g., scattered trees).
        eWindswept           // (c) Windswept Sites where the roof is exposed on all sides, with no protection provided by obstructions, trees, or terrain features higher than the roof.
    }

    public enum EScrewTypes
    {
        // See AS / NZS 4600 - Figure 5.4.3.2 SCREW PULL-OVER WITH WASHER
        eA_HEXheadScrew_FlatWasher,    // (a) Flat steel washer beneath hex head screw head
        eB_PancakeScrewWahserHead,     // (b) Pancake screw washer head
        eC_HWH_FlatWahser,             // (c) Flat steel washer beneath hex washer head screw head (HWH has integral solid washer)
        eD_DomedWasherScrew            // (d) Domed washer (non-solid) beneath screw head
    }

    // Todo Ondrej - urcit kde maju byt tieto struktury, vstupy z dialogu pre grafiku, zatazenie a lokalizaciu budovy

    public struct DisplayOptions
    {
        public bool bUseLightDirectional;
        public bool bUseLightPoint;
        public bool bUseLightSpot;
        public bool bUseLightAmbient;

        public bool bUseDiffuseMaterial;
        public bool bUseEmissiveMaterial;

        public bool bDisplayMembers;
        public bool bDisplayJoints;
        public bool bDisplayPlates;
        public bool bDisplayConnectors;

        public bool bDisplayMembersCenterLines;
        public bool bDisplaySolidModel;
        public bool bDisplayWireFrameModel;

        public bool bDistinguishedColor;       // Distinquished color of front and back area of member (slower), if false -> one color of whole member (faster)
        public bool bTransparentMemberModel;   // Set material opacity less than 1.0

        public bool bDisplayGlobalAxis;
        public bool bDisplayMemberDescription;
        public bool bDisplayLoads;

        public bool bDisplayMemberLoads;
        public bool bDisplaySurfaceLoads;
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
        public float fW;     // Width X -axis
        public float fL;     // Length Y -axis
        public float fH_2;   // Height of building
        public float fH_1;   // Height of wall
        public float fRoofPitch_deg;
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
        public float fTerrainCategory;  // float value 1-4 see Tab 4.2
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
        public float fPeriodAlongXDirection_Tx; // sec
        public float fPeriodAlongYDirection_Ty; // sec
        public float fSpectralShapeFactor_Ch_Tx;
        public float fSpectralShapeFactor_Ch_Ty;
    }

    public struct FreeSurfaceLoadsMemberTypeData
    {
        public EMemberType_FormSteel memberType;
        public float fLoadingWidth;

        public FreeSurfaceLoadsMemberTypeData(EMemberType_FormSteel memberType_temp, float fLoadingWidth_temp)
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

    public struct PropertiesToInsertOpening
    {
        public string sBuildingSide;
        public int iBayNumber;
    }

    public struct DoorProperties
    {
        public float fDoorsHeight;
        public float fDoorsWidth;
        public float fDoorCoordinateXinBlock;
    }

    public struct WindowProperties
    {
        public float fWindowsHeight;
        public float fWindowsWidth;
        public float fWindowCoordinateXinBay;
        public float fWindowCoordinateZinBay;
        public int iNumberOfWindowColumns;
    }

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
