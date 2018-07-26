using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    // Nodes and Members
    //----------------------------------------------------------------------------
    // Define load direction or orientation in GCC (global coordinate system)
    //----------------------------------------------------------------------------
    public enum ELoadDirGCC1
    {
        eLD_GCC_X = 0,
        eLD_GCC_Y = 1,
        eLD_GCC_Z = 2
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
    // Define load type
    //----------------------------------------------------------------------------
    public enum EMLoadType
    {
        eMLT_F = 0, // Force
        eMLT_M = 1, // Moment
        eMLD_Temperature = 2 // Temperature
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
        eSerie_K
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

    public enum EPlateNumberAndPositionInJoint
    {
        eOneLeftPlate,   // 1 plate on the left side of cross-section (- y local axis)
        eOneRightPlate,  // 1 plate on the right side of cross-section (+ y local axis)
        eTwoPlates       // 2 plates in joint (one on the left and one on the right side of cross-section)
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

        public bool bDisplayLoads;
    }

    public struct BuildingDataInput
    {
        public ELocation location;               // City / Town
        public int iDesignLife;                  // Years
        public int iImportanceClass;             // Importance Level

        public float fAnnualProbabilityULS_Snow; // Annual Probability of Exceedence ULS - Snow
        public float fAnnualProbabilityULS_Wind; // Annual Probability of Exceedence ULS - Wind
        public float fAnnualProbabilityULS_EQ;   // Annual Probability of Exceedence ULS - EQ
        public float fAnnualProbabilitySLS;      // Annual Probability of Exceedence SLS

        public float fR_ULS_Snow;                // Number of years - ULS - Snow
        public float fR_ULS_Wind;                // Number of years - ULS - Wind
        public float fR_ULS_EQ;                  // Number of years - ULS - EQ
        public float fR_SLS;                     // Number of years - SLS
    }

    public struct WindLoadDataInput
    {
        public string sWindRegion; // Wind region // Wind region CI 3.2. Fig 3.1(A)
        public EWindRegion eWindRegion;
        public int iWindDirectionIndex;
        public float fTerrainCategory;
        public float fh; // Height
    }

    public struct SeisLoadDataInput
    {
        public string sSiteSubsoilClass;
        public float fProximityToFault; // m
        public float fZoneFactor_Z;
        public float fPeriodAlongXDirection_Tx; //sec
        public float fPeriodAlongYDirection_Ty;//sec
        public float fSpectralShapeFactor_Ch_T;
    }

    public struct DoorProperties
    {
        public string sBuildingSide;
        public int iBayNumber;
        public float fDoorsHeight;
        public float fDoorsWidth;
        public float fDoorCoordinateXinBlock;
    }

    public struct WindowProperties
    {
        public string sBuildingSide;
        public int iBayNumber;
        public float fWindowsHeight;
        public float fWindowsWidth;
        public float fWindowCoordinateXinBay;
        public float fWindowCoordinateZinBay;
        public int iNumberOfWindowColumns;
    }

    public struct loadInputComboboxIndexes
    {
        public int LocationIndex;
        public int ImportanceLevelIndex;
        public int DesignLifeIndex;
        public int SiteSubSoilClassIndex;
        public int TerrainRoughnessIndex;
        public int WindDirectionIndex;
        public float SiteElevation;
        public float FaultDistanceDmin;
        public float FaultDistanceDmax;
        public float PeriodAlongXDirectionTx;
        public float PeriodAlongYDirectionTy;
    }
}
