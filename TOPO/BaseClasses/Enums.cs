﻿using System.Windows.Media;

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
    // Define load direction in GCS (global coordinate system) or LCS (local coordinate system) of members or PCS (principal coordinate system) of members
    //----------------------------------------------------------------------------
    public enum ELoadDirection
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
    // Define load type (node, member or surface loads)
    //----------------------------------------------------------------------------
    public enum ELoadType
    {
        eLT_F = 0, // Force
        eLT_M = 1, // Moment
        eLT_Temperature = 2, // Temperature
        eLT_Settlement = 3
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
        //eShape3D_Cylinder,
        eShape3D_Frustum,
        eShape3D_Cone,
        //eShape3D_Sphere,
        eShape3D_Ellipsoid,
        eShape3D_Torus,
        eShape3D_Tetrahedron,
        eShape3D_Pentahedron,
        eShape3D_Octahedron,
        eShape3D_Docecahedron,
        eShape3D_Icosahedron
    }

    //----------------------------------------------------------------------------
    // Define foundation type
    //----------------------------------------------------------------------------
    public enum EFoundationType
    {
        eStrip = 0, // Strip footings / Wall footings
        eRaft = 1,  // Raft foundations
        ePad = 2,   // Spread footings / Footing pads
        ePile = 2,  // Deep foundation - pile foundations
    }

    //----------------------------------------------------------------------------
    // Define limit state type
    //----------------------------------------------------------------------------
    public enum ELSType
    {
        eLS_ULS = 0, // ULS (include strength, stability, Fire, EQ, ...)
        eLS_SLS = 1,  // SLS
        eLS_ALL = 2
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
        eGeneral = 7,
        eMinusX = 0, // (load acting in -X) Theta = 000 deg (see AS/NZS 1170.2, Figure 2.2)
        ePlusX = 2, // (load acting in +X) Theta = 180 deg (see AS/NZS 1170.2, Figure 2.2)
        eMinusY = 3, // (load acting in -Y) Theta = 270 deg (see AS/NZS 1170.2, Figure 2.2)
        ePlusY = 1, // (load acting in +Y) Theta = 090 deg (see AS/NZS 1170.2, Figure 2.2)
        eMinusZ = 4,
        ePlusZ = 5
    }

    //----------------------------------------------------------------------------
    // Define load case name
    //----------------------------------------------------------------------------
    public enum ELCName
    {
        eDL_G = 0,                                  // "Dead load G"
        eIL_Q = 1,                                  // "Imposed load Q"

        eSL_Su_Full = 2,                            // "Snow load Su - full"
        eSL_Su_Left = 3,                            // "Snow load Su - left"
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
    // Define wind load case type
    //----------------------------------------------------------------------------
    public enum ELCWindType
    {
        eUndefined = 0,
        eWL_Cpi_min,                            // "Wind load Cpi,min
        eWL_Cpi_max,                            // "Wind load Cpi,max
        eWL_Cpe_min,                            // "Wind load Cpe,min
        eWL_Cpe_max                             // "Wind load Cpe,max
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

    public enum EModelType_FS
    {
        eKitsetMonoRoofEnclosed = 0,
        eKitsetGableRoofEnclosed = 1,
        eKitsetSheltersSingleSpan = 2,
        eKitsetSheltersDoubleSpan = 3
    }

    public enum ESerieTypePlate
    {
        eSerie_B,
        eSerie_L,
        eSerie_LL,
        eSerie_F,
        eSerie_G,
        eSerie_H,
        eSerie_Q,
        eSerie_S,
        eSerie_T,
        eSerie_X,
        eSerie_Y,
        eSerie_J,
        eSerie_K,
        eSerie_M,
        eSerie_N,
        eSerie_O,
        eSerie_W
    };

    public enum ESerieTypeCrSc_FS
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

    public enum EMemberType_FS
    {
        //eInvalid = -1,
        eG = 0,  // Girt
        eC = 1,  // Column
        eER = 2, // End Rafter
        eWP = 3, // Wind Post
        eEC = 4, // End Column
        eEP = 5, // Edge Purlin
        eP = 6,  // Purlin
        eDT = 7, // Door Trimmer (roller door)
        eDL = 8, // Door Lintel (roller door)
        ePB = 9, // Purlin Block
        eGB = 10, // Girt Block
        eDF = 11, // Door Frame (personnel door)
        eWF = 12, // Window Frame
        eBG = 13, // Base Girt
        eMR = 14, // Main Rafter
        eMC = 15,  // Main Column
        eCB = 16 // Cross-bracing
    }

    public enum EMemberType_FS_Position
    {
        Unknown = 0,
        MainColumn = 1,
        MainRafter = 2,
        EdgeColumn = 3,
        EdgeRafter = 4,
        EdgePurlin = 5,
        Girt = 6,
        Purlin = 7,
        WindPostFrontSide = 8,
        WindPostBackSide = 9,
        GirtFrontSide = 10,
        GirtBackSide = 11,
        BracingBlockGirts = 12,
        BracingBlockPurlins = 13,
        BracingBlockGirtsFrontSide = 14,
        BracingBlockGirtsBackSide = 15,
        DoorFrame = 16,
        WindowFrame = 17,
        DoorTrimmer = 18,
        DoorLintel = 19,
        CrossBracingWall = 20,
        CrossBracingRoof = 21,

        MainRafterCanopy = 22,
        EdgeRafterCanopy = 23,
        PurlinCanopy = 24,
        BracingBlockPurlinsCanopy = 25,
        CrossBracingRoofCanopy = 26,
        EdgePurlinCanopy = 27
    }

    // TODO - Ondrej TOTO je asi zbytocna duplicita
    // EMemberType_FS_Position a EMemberGroupNames by sme mali zlucit

    public enum EMemberType_FEM
    {
        General = 1,
        Column = 2,
        Beam = 3,
        BeamColumn = 4,
        Cable = 5,
        Tendon = 6,
        Truss = 7,
        Tension = 8,
        Compression = 9,
        OnSubsoil = 10,
        Rigid = 11,
        Spring = 12,
        Coupling_RigidRigid = 13,
        Coupling_RigidHinge = 14,
        Coupling_HingeRigid = 15,
        Coupling_HingeHinge = 16,
        Dummy_Null = 17
    }

    //toto sa uz pouzivat nebude a bude to vymazane
    ////to Mato- keby bolo na mne,tak cele toto vyhodim
    ////Mne sa vobec nepaci,ze sa enum pouziva ako index v poli vid:
    ////ComponentList[(int)EMemberGroupNames.eRafter].ComponentName
    ////malo by sa to vyberat ako ComponentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.MainRafter).ComponentName potom je jedno ako su usporiadane v tom zozname
    //public enum EMemberGroupNames
    //{
    //    eMainColumn = 0,    // Internal frame
    //    eRafter = 1,        // Internal frame
    //    eMainColumn_EF = 2, // Edge frame
    //    eRafter_EF = 3,     // Edge frame
    //    eEavesPurlin = 4,
    //    eGirtWall = 5,
    //    ePurlin = 6,
    //    eFrontWindPost = 7,
    //    eBackWindPost = 8,
    //    eFrontGirt = 9,
    //    eBackGirt = 10,
    //    eGirtBracing = 11,
    //    ePurlinBracing = 12,
    //    eFrontGirtBracing = 13,
    //    eBackGirtBracing = 14,
    //    eCrossBracing_Walls = 15,
    //    eCrossBracing_Roof = 16
    //}

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

    public enum EModelType
    {
        eJoint = 0,
        eFooting = 1
        
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

    public enum ETypesOfBearingConnection
    {
        // See AS / NZS 4600 - Table 5.3.4.2(A)
        eType1 = 0, // Single shear and outside sheets of double shear connection with washers under both bolt head and nut (alpha = 1.00)
        eType2 = 1, // Single shear and outside sheets of double shear connection without washers under both head and nut, or with only one washer (alpha = 0.75)
        eType3 = 2, // Single shear and outside sheets of double shear connection using oversized or short-slotted holes parallel to the applied load without washers under both bolt head and nut, or with only one washer(alpha = 0.70)
        eType4 = 3, // Single shear and outside sheets of double shear connection using short-slotted holes perpendicular to the applied load without washers under both bolt head and nut, or with only one washer (alpha = 0.55)
        eType5 = 4, // Inside sheet of double shear connection with or without washers(alpha = 1.33)
        eType6 = 5, // Inside sheet of double shear connection using oversized or short slotted holes parallel to the applied load with or without washers (alpha = 1.10)
        eType7 = 6  // Inside sheet of double shear connection using short slotted holes perpendicular to the applied load with or without washers(alpha = 0.90)
    }

    public enum EJointType
    {
        eBase_MainColumn = 1,
        eKnee_MainRafter_Column = 2,
        eApex_MainRafters = 3,
        eBase_EdgeColumn = 4,
        eKnee_EgdeRafter_Column = 5,
        eApex_Edge_Rafters = 6,
        ePurlin_MainRafter = 7,
        ePurlin_EdgeRafter = 8,
        ePurlin_MainRafter_FlyBracing = 9,
        ePurlin_EdgeRafter_FlyBracing = 10,

        eEdgePurlin_MainRafter = 11,
        eEdgePurlin_EdgeRafter = 12,
        eGirt_MainColumn = 13,
        eGirt_EdgeColumn = 14,
        eGirt_MainColumn_FlyBracing = 15,
        eGirt_EdgeColumn_FlyBracing = 16,
        eBase_WindPost_Front = 17,
        eBase_WindPost_Back = 18,
        eWindPost_EdgeRafter_Front = 19,
        eWindPost_EdgeRafter_Back = 20,

        eGirt_EdgeColumn_Front = 21,
        eGirt_EdgeColumn_Back = 22,
        eGirt_WindPost_Front = 23,
        eGirt_WindPost_Back = 24,
        eBase_DoorTrimmer = 25,
        eDoorTrimmer_Girt = 26,
        eDoorTrimmer_GirtFront = 27,
        eDoorTrimmer_Girt_Back = 28,
        eGirt_DoorTrimmer = 29,
        eGirt_DoorTrimmer_Front = 30,

        eGirt_DoorTrimmer_Back = 31,
        eDoorTrimmer_EdgePurlin = 32,
        eDoorTrimmer_EdgeRafter = 33,
        eDoorLintel_Trimmer = 34,
        eBase_DoorFrame = 35,
        eDoorFrame_Girt = 36,
        eDoorFrame_Girt_Front = 37,
        eDoorFrame_Girt_Back = 38,
        eGirt_DoorFrame = 39,
        eGirt_DoorFrame_Front = 40,

        eGirt_DoorFrame_Back = 41,
        eDoorFrame_EdgePurlin = 42,
        eDoorFrame_EdgeRafter = 43,
        eDoorFrame = 44,
        eWindowFrame_Girt = 45,
        eWindowFrame_Girt_Front = 46,
        eWindowFrame_Girt_Back = 47,
        eWindowFrame_EdgePurlin = 48,
        eWindowFrame_EdgeRafter = 49,
        eWindowFrame_Header_Sill_WindowFrameColumn = 50,

        eGirtbracing_Girt = 51,
        eGirtBracing_EdgePurlin = 52,
        eGirtBracing_Girt_Front = 53,
        eGirtBracing_EdgeRafter_Front = 54,
        eGirtBracing_Girt_Back = 55,
        eGirtBracing_EdgeRafter_Back = 56,
        ePurlinBracing_Purlin = 57,
        ePurlinBracing_EdgePurlin = 58,
        eCrossBracing_MainColumn = 59,
        eCrossBracing_EdgeColumn = 60,

        eCrossBracing_MainRafter = 61,
        eCrossBracing_EdgeRafter = 62,

        // Canopy
        eKnee_MainRafterCanopy_Column = 63,
        eKnee_EdgeRafterCanopy_Column = 64,
        eKnee_EdgeRafterCanopy_EdgeColumn = 65,
        ePurlin_MainRafterCanopy = 66,
        ePurlin_EdgeRafterCanopy = 67,
        ePurlin_MainRafterCanopy_FlyBracing = 68,
        ePurlin_EdgeRafterCanopy_FlyBracing = 69,
        ePurlin_MainRafterCanopyEdge_FlyBracing = 70,
        ePurlin_EdgeRafterCanopyEdge_FlyBracing = 71,
        ePurlinBracingCanopy_PurlinCanopy = 72,
        ePurlinBracingCanopy_EdgePurlin = 73,
        ePurlinBracingCanopy_EdgePurlinCanopy = 74,
        eCrossBracing_MainRafterCanopy = 75,
        eCrossBracing_EdgeRafterCanopy = 76
    }

    public enum EModelViews
    {
        ISO_FRONT_RIGHT = 0, // Default
        ISO_FRONT_LEFT = 1,
        ISO_BACK_RIGHT = 2,
        ISO_BACK_LEFT = 3,
        FRONT = 4,
        BACK = 5,
        LEFT = 6,
        RIGHT = 7,
        TOP = 8
    }
    public enum EViewModelMemberFilters
    {
        All = 0,
        FRONT = 1,
        BACK = 2,
        LEFT = 3,
        RIGHT = 4,
        ROOF = 5,
        MIDDLE_FRAME = 6,
        COLUMNS = 7,
        FOUNDATIONS = 8,
        FLOOR = 9
    }

    public enum EViewCladdingFilters
    {        
        CLADDING_FRONT = 1,
        CLADDING_BACK = 2,
        CLADDING_LEFT = 3,
        CLADDING_RIGHT = 4,
        CLADDING_ROOF = 5
    }

    public enum EPDFPageContentType
    {
        Isometric_View = 0,
        Front_Elevation = 1,
        Back_Elevation = 2,
        Left_Elevation = 3,
        Right_Elevation = 4,
        Roof_Layout = 5,
        Middle_Frame = 6,
        Columns = 7,
        Foundation_Pads = 8,
        Floor_Plan = 9,
        Details_Standard_1 = 10,
        Details_Standard_2 = 11,
        Details_Joints = 12,
        Details_Footing_Pads = 13,
        Details_Floor = 14
    }

    public enum ELinePatternType
    {
        CONTINUOUS = 0,
        DASHED = 1,
        DOTTED = 2,
        DASHDOTTED = 3,
        DIVIDE = 4
    }

    public enum EPageSizes
    {
        A0 = 0,
        A1 = 1,
        A2 = 2,
        A3 = 3,
        A4 = 4
    }

    public enum EPageOrientation
    {
        Portrait = 0,
        Landscape = 1
    }

    public enum EImagesQuality
    {
        Low = 1,
        Normal = 2,
        Hight = 3,
        Best = 4
    }

    public enum EBuildingSide
    {
        Left = 1,
        Right =2,
        Front = 3,
        Back = 4,
        Roof_Left_Side = 5,
        Roof_Right_Side = 6,
        Roof = 7
    }

    public enum EOpeningType
    {
        Doors = 1,
        Windows = 2,
        Fibreglass = 3
    }

    public enum EDisplayOptionsTypes
    {
        GUI_3D_Scene = 0,
        GUI_Joint_Preview = 1,
        GUI_Foundation_Preview = 2,
        GUI_Accessories_Preview = 3,

        Report_3DScene = 4,
        Report_Joints = 5,
        Report_Foundations = 6,

        Layouts_3D_Scene = 7,
        Layouts_FW_Elevations = 8,
        Layouts_FW_Roof = 9,
        Layouts_FW_Frames = 10,
        Layouts_FW_Columns = 11,
        Layouts_FW_Foundations = 12,
        Layouts_FW_Floor = 13,
        Layouts_CW_Elevations = 14,
        Layouts_CW_Roof = 15,
        Layouts_Joints = 16,
        Layouts_Foundations = 17
    }


    public enum EFlashingType
    {
        RoofRidge = 1, 
        RoofRidgeSoftEdge = 2,
        WallCorner = 3,
        Barge = 4,
        BargeBirdproof = 5,
        EavePurlinBirdproofStrip = 6,
        RollerDoorTrimmer = 7,
        RollerDoorHeader = 8,
        RollerDoorHeaderCap = 9,
        /*"PA Door Trimmer",*/
        PADoorHeaderCap = 10,
        Window = 11,
        FibreglassRoofRidgeCap = 12
    }

    

    // Globalna rovina GCS do ktorej sa kota kresli 0 - XY, 1 - YZ, 2 - XZ, -1 nedefinovana (vseobecna kota)
    public enum EGlobalPlane
    {
        XY = 0,
        YZ = 1,
        XZ = 2,

        UNDEFINED = -1
    }

    //smer GCS
    public enum EGCSDirection
    {
        X = 0,
        Y = 1,
        Z = 2,

        UNDEFINED = -1
    }

    public enum ESoftware
    {
        Unknown = 0,
        PFD = 1,
        SCV = 2 //System Component Viewer
        
         
    }
    
    
}
