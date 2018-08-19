using BaseClasses;
using BaseClasses.CRSC;
using MATERIAL;
using System;

namespace Examples
{
    public class CExample_3D_01 : CExample
    {
        /*
        public BaseClasses.CNode[] m_arrNodes = new BaseClasses.CNode[6];
        public CMember[] arrMembers = new CMember[9];
        public CMat[] arrMat = new CMat[5];
        public CRSC.CCrSc[] m_arrCrSc = new CRSC.CCrSc[3];
        public BaseClasses.CNSupport[] arrNSupports = new BaseClasses.CNSupport[3];
        public BaseClasses.CNLoad[] arrNLoads = new BaseClasses.CNLoad[3];
        */

        public CExample_3D_01()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[6];
            m_arrMembers = new CMember[9];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[3];
            //m_arrNLoads = new BaseClasses.CNLoad[3];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            //m_arrCrSc[0] = new CRSC.CCrSc_0_00(0.1f, 20); // Solid Half Circle / Semicircle shape
            //m_arrCrSc[0] = new CRSC.CCrSc_0_01(0.1f, 6); // Solid Quater Circle - chyba nezobrazuje sa jedna strana
            //m_arrCrSc[0] = new CRSC.CCrSc_0_02(0.1f, 20); // Rolled round bar
            //m_arrCrSc[0] = new CRSC.CCrSc_0_03(0.2f, 0.1f, 21); // Solid Ellipse
            //m_arrCrSc[0] = new CRSC.CCrSc_0_04(0.3f, 0.5f); // Triangular Prism / Equilateral
            //m_arrCrSc[0] = new CRSC.CCrSc_0_05(0.1f, 0.05f); // Solid square section
            //m_arrCrSc[0] = new CRSC.CCrSc_0_06(0.1f); // Solid Penthagon
            //m_arrCrSc[0] = new CRSC.CCrSc_0_07(0.1f); // Solid Hexagon
            //m_arrCrSc[0] = new CRSC.CCrSc_0_08(0.1f); // Solid Octagon
            //m_arrCrSc[0] = new CRSC.CCrSc_0_09(0.1f); // Solid Dodecagon
            //m_arrCrSc[0] = new CRSC.CCrSc_0_20(0.2f, 0.010f, 25); // Semicircle Curve
            //m_arrCrSc[0] = new CCrSc_0_22(0.2f, 0.05f, 12); // Circular Hollow Section (Tube, Pipe)
            //m_arrCrSc[0] = new CCrSc_0_23(0.2f, 0.1f, 0.020f, 24); // Elliptical Hollow Section
            //m_arrCrSc[0] = new CCrSc_0_24(0.2f, 0.05f); // Triangular Prism / Equilateral with Opening
            //m_arrCrSc[0] = new CCrSc_0_25(0.2f, 0.15f, 0.01f, 0.008f); // Welded hollow section - doubly symmetrical
            //m_arrCrSc[0] = new CCrSc_0_26(0.2f, 0.05f); // Empty (Hollow) Penthagon
            //m_arrCrSc[0] = new CCrSc_0_27(0.2f, 0.05f); // Empty (Hollow) Hexagon
            //m_arrCrSc[0] = new CCrSc_0_28(0.2f, 0.05f); // Empty (Hollow) Octagon
            //m_arrCrSc[0] = new CCrSc_0_50(0.2f, 0.1f, 0.015f, 0.006f); // Doubly symmetric I section
            //m_arrCrSc[0] = new CCrSc_0_52(0.2f, 0.1f, 0.015f, 0.006f, -0.05f); // Monosymmetric U/C section
            //m_arrCrSc[0] = new CCrSc_0_54(0.2f, 0.1f, 0.015f, 0.010f, 0.050f, 0.010f); // Welded Angle section
            //m_arrCrSc[0] = new CCrSc_0_56(0.2f, 0.1f, 0.015f, 0.010f, 0.15f); // Welded monosymmetric T section
            //m_arrCrSc[0] = new CCrSc_0_58(0.2f, 0.1f, 0.015f, 0.010f); // Welded centrally symmetric Z section
            //m_arrCrSc[0] = new CCrSc_0_60(0.2f, 0.1f, 0.015f); // Doubly symmetric Cruciform
            //m_arrCrSc[0] = new CCrSc_0_61(0.2f, 0.010f); // Y-section
            //m_arrCrSc[0] = new CCrSc_3_00(1, 8, 0.200f, 0.090f, 0.0113f, 0.0075f, 0.0075f, 0.0045f, 0.1699f); - nefunguje
            //m_arrCrSc[0] = new CCrSc_3_01
            //m_arrCrSc[0] = new CCrSc_3_02(0, 0.2f, 0.1f, 0.010f, 0.005f, 0.010f, 0.010f, 0.160f, 0.05f); // Rolled monosymmetric U section (channel) - tapered or paralel flanges
            //m_arrCrSc[0] = new CCrSc_3_02(2, 0.2f, 0.1f, 0.010f, 0.005f, 0.010f, 0.160f, 0.05f); // Rolled monosymmetric U section (channel) - tapered or paralel flanges
            //m_arrCrSc[0] = new CCrSc_3_03(0.2f, 0.010f, 0.010f, 0.010f, 0.050f); // Rolled monosymmetric L section (angle with equal legs)
            //m_arrCrSc[0] = new CCrSc_3_04(0.2f, 0.1f, 0.010f, 0.010f, 0.010f, 0.080f,0.030f); // Rolled L section (angle with unequal legs)
            //m_arrCrSc[0] = new CCrSc_3_05(0.2f, 0.010f); // Circular Hollow Section
            //m_arrCrSc[0] = new CCrSc_3_06(0.2f, 0.1f, 0.010f); // Elliptical Hollow Section
            //m_arrCrSc[0] = new CCrSc_3_07(0, 0.2f, 0.05f, 0.005f, 0.08f); // Rectangular Hollow Section - nefunguje
            //m_arrCrSc[0] = new CCrSc_3_08(0, 0.2f, 0.1f, 0.005f, 0.005f, 0.015f, 0.005f, 0.005f, 0.0025f, 0.1875f);
            //m_arrCrSc[0] = new CCrSc_3_08(1, 0.2f, 0.1f, 0.005f, 0.005f, 0.015f, 0.005f, 0.005f, 0.0025f, 0.1875f);
            //m_arrCrSc[0] = new CCrSc_3_08(2, 0.2f, 0.1f, 0.010f, 0.010f, 0.015f, 0.010f, 0.005f, 0.175f);
            m_arrCrSc[0] = new CCrSc_3_08(3, 0.2f, 0.1f, 0.010f, 0.010f, 0.015f, 0.010f, 0.005f, 0.175f);

            // Nodes
            // Nodes List - Nodes Array

            m_arrNodes[0] = new CNode(1, 0.500f, 0, 2.500f, 0);
            m_arrNodes[1] = new CNode(2, 2.500f, 0, 2.500f, 0);
            m_arrNodes[2] = new CNode(3, 5.500f, 0, 2.500f, 0);
            m_arrNodes[3] = new CNode(4, 0.500f, 0, 0.500f, 0);
            m_arrNodes[4] = new CNode(5, 2.500f, 0, 0.500f, 0);
            m_arrNodes[5] = new CNode(6, 5.500f, 0, 0.500f, 0);

            // Sort by ID
            //Array.Sort(m_arrNodes, new BaseClasses.CCompare_NodeID());

            // Members
            // Members List - Members Array

            m_arrMembers[0] = new CMember(1, m_arrNodes[0], m_arrNodes[1], m_arrCrSc[0], 0);
            m_arrMembers[1] = new CMember(2, m_arrNodes[1], m_arrNodes[2], m_arrCrSc[0], 0);
            m_arrMembers[2] = new CMember(3, m_arrNodes[0], m_arrNodes[3], m_arrCrSc[0], 0);
            m_arrMembers[3] = new CMember(4, m_arrNodes[1], m_arrNodes[4], m_arrCrSc[0], 0);
            m_arrMembers[4] = new CMember(5, m_arrNodes[2], m_arrNodes[5], m_arrCrSc[0], 0);
            m_arrMembers[5] = new CMember(6, m_arrNodes[3], m_arrNodes[4], m_arrCrSc[0], 0);
            m_arrMembers[6] = new CMember(7, m_arrNodes[4], m_arrNodes[5], m_arrCrSc[0], 0);
            m_arrMembers[7] = new CMember(8, m_arrNodes[1], m_arrNodes[3], m_arrCrSc[0], 0);
            m_arrMembers[8] = new CMember(9, m_arrNodes[1], m_arrNodes[5], m_arrCrSc[0], 0);

            //Sort by ID
            //Array.Sort(m_arrMembers, new BaseClasses.CCompare_MemberID());

            // Nodal Supports - fill values
            // Set values
            bool[] bSupport1 = { true, false, true, false, true, false };
            bool[] bSupport2 = { false, false, true, false, true, false };
            bool[] bSupport3 = { true, false, false, false, false, false };

            // Create Support Objects
            // Pozn. Jednym z parametrov by malo byt pole ID uzlov v ktorych je zadefinovana tato podpora
            // objekt podpory bude len jeden a dotknute uzly budu vediet ze na ich podpora existuje a ake je konkretne ID jej nastaveni
            m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[0], bSupport1, 0);
            m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[2], bSupport2, 0);
            m_arrNSupports[2] = new CNSupport(6, 3, m_arrNodes[5], bSupport3, 0);

            // Sort by ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());
        }
    }
}
