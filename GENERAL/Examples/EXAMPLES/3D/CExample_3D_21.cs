using BaseClasses;
using CRSC;
using MATERIAL;
using System;

namespace Examples
{
    public class CExample_3D_21 : CExample
    {
        public CExample_3D_21()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[62];
            m_arrMembers = new CMember[31];
            m_arrMat = new System.Collections.Generic.Dictionary<EMemberGroupNames, CMat>();
            m_arrCrSc = new CCrSc[31];
            m_arrNSupports = new CNSupport[3];
            //m_arrNLoads = new BaseClasses.CNLoad[3];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // TEMP CROSS-SECTIONS TEST

            // Half Circle Bar
            // Quater Circle Bar
            // load_0_00_01_TriangelsIndices();
            // Round or Ellipse Bar
            // load_0_02_03_TriangelIndices();
            // Triangular Prism
            // load_0_04_TriangelsIndices();
            // Flat Bar
            // load_0_05_TriangelIndices();
            // Half Circle
            // load_0_20_TriangelIndices();
            // TUBE / PIPE Circle or Ellipse Shape
            // load_0_22_23_TriangelIndices();
            // Triangular Prism with Opening
            // load_0_24_TriangelsIndices();
            // HL-section / Rectanglular Hollow Cross-section
            // load_0_25_TriangelIndices();
            // Polygonal Hollow Section
            // load_0_26_28_TriangelIndices(0,test1.objCrScHollow.INoPoints);
            // I - section
            // load_0_50_TriangelIndices();
            // U-section
            // load_0_52_TriangelIndices();
            // L-section / Angle section
            // load_0_54_TriangelIndices();
            // T-section / T section
            // load_0_56_TriangelIndices();
            // Z-section / Z section
            // load_0_58_TriangelIndices();
            // Cruciform Bar
            // load_0_60_TriangelIndices(test1.objCrScSolid.ITotNoPoints);
            // Y-section / Y section
            // load_0_61_TriangelIndices();

            // OPRAVIT I a BOX !!!!!!!! NIECO SA POKAZILO :)!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Temp

            // Rolled I doubly symmetric profile, Tapered or parallel flanges
            // CCrSc obj_CrSc = new CCrSc_3_00(0, 8, 200, 90, 11.3f, 7.5f, 7.5f, 4.5f, 159.1f);
            // load_3_00_TriangelIndices(0, 12,8); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(1,8, 8); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(2, 4, 8); // Shape ID, number of auxiliary points , number of segments of arc

            // Rolled I monosymmetric profile, Tapered or parallel flanges
            // load_3_00_TriangelIndices(0, 12, 8); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(1,8,4); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(2, 4, 4); // Shape ID, number of auxiliary points , number of segments of arc

            // Rolled U profile, Tapered or parallel flanges, channel section
            // load_3_02_TriangelIndices(0,6, 8); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_02_TriangelIndices(2,2, 8); // Shape ID,number of auxiliary points , number of segments of arc
            // Rolled L profile, angle section
            // load_3_03_04_TriangelIndices(3, 8); // Number of auxiliary points, number of segments of arc
            // Rectanglular Hollow Cross-section
            // load_3_07_TriangelIndices(0, 4, 4); // Shape ID, number of auxiliary points per section, number of segments of one arc
            // load_3_07_TriangelIndices(2, 4, 4); // Shape ID, number of auxiliary points per section, number of segments of one arc
            // load_3_07_TriangelIndices(3, 4, 4); // Shape ID, number of auxiliary points per section, number of segments of one arc (iAux = 4)
            // load_3_07_TriangelIndices(5, 0, 0); // Shape ID, number of auxiliary points per section, number of segments of one arc
            // Rolled T profile, Tapered flanges
            // load_3_08_TriangelIndices(1,6,4); // Shape ID, number of auxiliary points, number of segments of arc
            // load_3_08_TriangelIndices(2,4,4); // Shape ID, number of auxiliary points, number of segments of arc
            // load_3_08_TriangelIndices(3, 2, 4); // Shape ID, number of auxiliary points, number of segments of arc

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[00] = new CCrSc_0_00(0.1f, 20); // Solid Half Circle / Semicircle shape
            m_arrCrSc[01] = new CCrSc_0_01(0.1f, 20); // Solid Quater Cirlce - chyba nezobrazuje sa jedna strana
            m_arrCrSc[02] = new CCrSc_0_02(0.1f, 20); // Rolled round bar
            m_arrCrSc[03] = new CCrSc_0_03(0.2f, 0.1f, 21); // Solid Ellipse
            m_arrCrSc[04] = new CCrSc_0_04(0.3f, 0.5f); // Triangular Prism / Equilateral
            m_arrCrSc[05] = new CCrSc_0_05(0.1f, 0.05f); // Solid square section
            m_arrCrSc[06] = new CCrSc_0_06(0.1f); // Solid Penthagon
            m_arrCrSc[07] = new CCrSc_0_07(0.1f); // Solid Hexagon
            m_arrCrSc[08] = new CCrSc_0_08(0.1f); // Solid Octagon
            m_arrCrSc[09] = new CCrSc_0_09(0.1f); // Solid Dodecagon
            m_arrCrSc[10] = new CCrSc_0_20(0.2f, 0.010f, 25); // Semicircle Curve
            m_arrCrSc[11] = new CCrSc_0_22(0.2f, 0.05f, 12); // Circular Hollow Section (Tube, Pipe)
            m_arrCrSc[12] = new CCrSc_0_23(0.2f, 0.1f, 0.020f, 24); // Elliptical Hollow Section
            m_arrCrSc[13] = new CCrSc_0_24(0.2f, 0.05f); // Triangular Prism / Equilateral with Opening
            m_arrCrSc[14] = new CCrSc_0_25(0.2f, 0.15f, 0.01f, 0.008f); // Welded hollow section - doubly symmetrical
            m_arrCrSc[15] = new CCrSc_0_26(0.2f, 0.05f); // Empty (Hollow) Penthagon
            m_arrCrSc[16] = new CCrSc_0_27(0.2f, 0.05f); // Empty (Hollow) Hexagon
            m_arrCrSc[17] = new CCrSc_0_28(0.2f, 0.05f); // Empty (Hollow) Octagon
            m_arrCrSc[18] = new CCrSc_0_50(0.2f, 0.1f, 0.015f, 0.006f); // Doubly symmetric I section
            m_arrCrSc[19] = new CCrSc_0_52(0.2f, 0.1f, 0.015f, 0.006f, -0.05f); // Monosymmetric U/C section
            m_arrCrSc[20] = new CCrSc_0_54(0.2f, 0.1f, 0.015f, 0.010f, 0.050f, 0.010f); // Welded Angle section
            m_arrCrSc[21] = new CCrSc_0_56(0.2f, 0.1f, 0.015f, 0.010f, 0.15f); // Welded monosymmetric T section
            m_arrCrSc[22] = new CCrSc_0_58(0.2f, 0.1f, 0.015f, 0.010f); // Welded centrally symmetric Z section
            m_arrCrSc[23] = new CCrSc_0_60(0.2f, 0.1f, 0.015f); // Doubly symmetric Cruciform
            m_arrCrSc[24] = new CCrSc_0_61(0.2f, 0.010f); // Y-section

            // Rolled I doubly symmetric profile, Tapered or parallel flanges
            m_arrCrSc[25] = new CCrSc_3_00(0, 8, 0.200f, 0.090f, 0.0113f, 0.0075f, 0.0075f, 0.0045f, 0.1654f);
            m_arrCrSc[26] = new CCrSc_3_00(1, 8, 0.200f, 0.090f, 0.0113f, 0.0075f, 0.0075f, 0.1699f);
            m_arrCrSc[27] = new CCrSc_3_00(2, 8, 0.200f, 0.090f, 0.0113f, 0.0075f, 0.0075f, 0.1699f);

            // load_3_00_TriangelIndices(0, 12,8); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(1,8, 8); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(2, 4, 8); // Shape ID, number of auxiliary points , number of segments of arc

            // Rolled I monosymmetric profile, Tapered or parallel flanges
            m_arrCrSc[28] = new CCrSc_3_01(0, 8, 0.200f, 0.190f, 0.150f, 0.0113f, 0.0075f, 0.0075f, 0.0045f, 0.1654f);
            m_arrCrSc[29] = new CCrSc_3_01(1, 8, 0.200f, 0.150f, 0.100f, 0.0113f, 0.0075f, 0.0075f, 0.1699f);
            m_arrCrSc[30] = new CCrSc_3_01(2, 8, 0.200f, 0.090f, 0.150f, 0.0113f, 0.0075f, 0.0075f, 0.1699f);

            // load_3_00_TriangelIndices(0, 12, 8); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(1,8,4); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(2, 4, 4); // Shape ID, number of auxiliary points , number of segments of arc


            //m_arrCrSc[0] = new CCrSc_3_07(1,0.2f, 0.05f, 0.005f, 0.005f, 0.003f); // rectangular hollow section

            // Nodes
            // Nodes List - Nodes Array

            m_arrNodes[0]  = new BaseClasses.CNode(1, 0.0f, 0.0f, 0.0f, 0);
            m_arrNodes[1]  = new BaseClasses.CNode(2, 5.0f, 0.0f, 0.0f, 0);
            m_arrNodes[2]  = new BaseClasses.CNode(3, 0.0f, 1.0f, 0.0f, 0);
            m_arrNodes[3]  = new BaseClasses.CNode(4, 5.0f, 1.0f, 0.0f, 0);
            m_arrNodes[4]  = new BaseClasses.CNode(5, 0.0f, 2.0f, 0.0f, 0);
            m_arrNodes[5]  = new BaseClasses.CNode(6, 5.0f, 2.0f, 0.0f, 0);
            m_arrNodes[6]  = new BaseClasses.CNode(7, 0.0f, 3.0f, 0.0f, 0);
            m_arrNodes[7]  = new BaseClasses.CNode(8, 5.0f, 3.0f, 0.0f, 0);
            m_arrNodes[8]  = new BaseClasses.CNode(9, 0.0f, 4.0f, 0.0f, 0);
            m_arrNodes[9]  = new BaseClasses.CNode(10, 5.0f, 4.0f, 0.0f, 0);
            m_arrNodes[10] = new BaseClasses.CNode(11, 0.0f, 5.0f, 0.0f, 0);
            m_arrNodes[11] = new BaseClasses.CNode(12, 5.0f, 5.0f, 0.0f, 0);
            m_arrNodes[12] = new BaseClasses.CNode(13, 0.0f, 6.0f, 0.0f, 0);
            m_arrNodes[13] = new BaseClasses.CNode(14, 5.0f, 6.0f, 0.0f, 0);
            m_arrNodes[14] = new BaseClasses.CNode(15, 0.0f, 7.0f, 0.0f, 0);
            m_arrNodes[15] = new BaseClasses.CNode(16, 5.0f, 7.0f, 0.0f, 0);
            m_arrNodes[16] = new BaseClasses.CNode(17, 0.0f, 8.0f, 0.0f, 0);
            m_arrNodes[17] = new BaseClasses.CNode(18, 5.0f, 8.0f, 0.0f, 0);
            m_arrNodes[18] = new BaseClasses.CNode(19, 0.0f, 9.0f, 0.0f, 0);
            m_arrNodes[19] = new BaseClasses.CNode(20, 5.0f, 9.0f, 0.0f, 0);
            m_arrNodes[20] = new BaseClasses.CNode(21, 0.0f, 10.0f, 0.0f, 0);
            m_arrNodes[21] = new BaseClasses.CNode(22, 5.0f, 10.0f, 0.0f, 0);
            m_arrNodes[22] = new BaseClasses.CNode(23, 0.0f, 11.0f, 0.0f, 0);
            m_arrNodes[23] = new BaseClasses.CNode(24, 5.0f, 11.0f, 0.0f, 0);
            m_arrNodes[24] = new BaseClasses.CNode(25, 0.0f, 12.0f, 0.0f, 0);
            m_arrNodes[25] = new BaseClasses.CNode(26, 5.0f, 12.0f, 0.0f, 0);
            m_arrNodes[26] = new BaseClasses.CNode(27, 0.0f, 13.0f, 0.0f, 0);
            m_arrNodes[27] = new BaseClasses.CNode(28, 5.0f, 13.0f, 0.0f, 0);
            m_arrNodes[28] = new BaseClasses.CNode(29, 0.0f, 14.0f, 0.0f, 0);
            m_arrNodes[29] = new BaseClasses.CNode(30, 5.0f, 14.0f, 0.0f, 0);
            m_arrNodes[30] = new BaseClasses.CNode(31, 0.0f, 15.0f, 0.0f, 0);
            m_arrNodes[31] = new BaseClasses.CNode(32, 5.0f, 15.0f, 0.0f, 0);
            m_arrNodes[32] = new BaseClasses.CNode(33, 0.0f, 16.0f, 0.0f, 0);
            m_arrNodes[33] = new BaseClasses.CNode(34, 5.0f, 16.0f, 0.0f, 0);
            m_arrNodes[34] = new BaseClasses.CNode(35, 0.0f, 17.0f, 0.0f, 0);
            m_arrNodes[35] = new BaseClasses.CNode(36, 5.0f, 17.0f, 0.0f, 0);
            m_arrNodes[36] = new BaseClasses.CNode(37, 0.0f, 18.0f, 0.0f, 0);
            m_arrNodes[37] = new BaseClasses.CNode(38, 5.0f, 18.0f, 0.0f, 0);
            m_arrNodes[38] = new BaseClasses.CNode(39, 0.0f, 19.0f, 0.0f, 0);
            m_arrNodes[39] = new BaseClasses.CNode(40, 5.0f, 19.0f, 0.0f, 0);
            m_arrNodes[40] = new BaseClasses.CNode(41, 0.0f, 20.0f, 0.0f, 0);
            m_arrNodes[41] = new BaseClasses.CNode(42, 5.0f, 20.0f, 0.0f, 0);
            m_arrNodes[42] = new BaseClasses.CNode(43, 0.0f, 21.0f, 0.0f, 0);
            m_arrNodes[43] = new BaseClasses.CNode(44, 5.0f, 21.0f, 0.0f, 0);
            m_arrNodes[44] = new BaseClasses.CNode(45, 0.0f, 22.0f, 0.0f, 0);
            m_arrNodes[45] = new BaseClasses.CNode(46, 5.0f, 22.0f, 0.0f, 0);
            m_arrNodes[46] = new BaseClasses.CNode(47, 0.0f, 23.0f, 0.0f, 0);
            m_arrNodes[47] = new BaseClasses.CNode(48, 5.0f, 23.0f, 0.0f, 0);
            m_arrNodes[48] = new BaseClasses.CNode(49, 0.0f, 24.0f, 0.0f, 0);
            m_arrNodes[49] = new BaseClasses.CNode(50, 5.0f, 24.0f, 0.0f, 0);
            m_arrNodes[50] = new BaseClasses.CNode(51, 0.0f, 25.0f, 0.0f, 0);
            m_arrNodes[51] = new BaseClasses.CNode(52, 5.0f, 25.0f, 0.0f, 0);
            m_arrNodes[52] = new BaseClasses.CNode(53, 0.0f, 26.0f, 0.0f, 0);
            m_arrNodes[53] = new BaseClasses.CNode(54, 5.0f, 26.0f, 0.0f, 0);
            m_arrNodes[54] = new BaseClasses.CNode(55, 0.0f, 27.0f, 0.0f, 0);
            m_arrNodes[55] = new BaseClasses.CNode(56, 5.0f, 27.0f, 0.0f, 0);
            m_arrNodes[56] = new BaseClasses.CNode(57, 0.0f, 28.0f, 0.0f, 0);
            m_arrNodes[57] = new BaseClasses.CNode(58, 5.0f, 28.0f, 0.0f, 0);
            m_arrNodes[58] = new BaseClasses.CNode(59, 0.0f, 29.0f, 0.0f, 0);
            m_arrNodes[59] = new BaseClasses.CNode(60, 5.0f, 29.0f, 0.0f, 0);
            m_arrNodes[60] = new BaseClasses.CNode(61, 0.0f, 30.0f, 0.0f, 0);
            m_arrNodes[61] = new BaseClasses.CNode(62, 5.0f, 30.0f, 0.0f, 0);

            // Sort by ID
            //Array.Sort(m_arrNodes, new BaseClasses.CCompare_NodeID());

            // Members
            // Members List - Members Array

            m_arrMembers[0] = new BaseClasses.CMember(1, m_arrNodes[0], m_arrNodes[1], m_arrCrSc[0], 0);
            m_arrMembers[1] = new BaseClasses.CMember(2, m_arrNodes[2], m_arrNodes[3], m_arrCrSc[1], 0);
            m_arrMembers[2] = new BaseClasses.CMember(3, m_arrNodes[4], m_arrNodes[5], m_arrCrSc[2], 0);
            m_arrMembers[3] = new BaseClasses.CMember(4, m_arrNodes[6], m_arrNodes[7], m_arrCrSc[3], 0);
            m_arrMembers[4] = new BaseClasses.CMember(5, m_arrNodes[8], m_arrNodes[9], m_arrCrSc[4], 0);
            m_arrMembers[5] = new BaseClasses.CMember(6, m_arrNodes[10], m_arrNodes[11], m_arrCrSc[5], 0);
            m_arrMembers[6] = new BaseClasses.CMember(7, m_arrNodes[12], m_arrNodes[13], m_arrCrSc[6], 0);
            m_arrMembers[7] = new BaseClasses.CMember(8, m_arrNodes[14], m_arrNodes[15], m_arrCrSc[7], 0);
            m_arrMembers[8] = new BaseClasses.CMember(9, m_arrNodes[16], m_arrNodes[17], m_arrCrSc[8], 0);
            m_arrMembers[9] = new BaseClasses.CMember(10, m_arrNodes[18], m_arrNodes[19], m_arrCrSc[9], 0);
            m_arrMembers[10] = new BaseClasses.CMember(11, m_arrNodes[20], m_arrNodes[21], m_arrCrSc[10], 0);
            m_arrMembers[11] = new BaseClasses.CMember(12, m_arrNodes[22], m_arrNodes[23], m_arrCrSc[11], 0);
            m_arrMembers[12] = new BaseClasses.CMember(13, m_arrNodes[24], m_arrNodes[25], m_arrCrSc[12], 0);
            m_arrMembers[13] = new BaseClasses.CMember(14, m_arrNodes[26], m_arrNodes[27], m_arrCrSc[13], 0);
            m_arrMembers[14] = new BaseClasses.CMember(15, m_arrNodes[28], m_arrNodes[29], m_arrCrSc[14], 0);
            m_arrMembers[15] = new BaseClasses.CMember(16, m_arrNodes[30], m_arrNodes[31], m_arrCrSc[15], 0);
            m_arrMembers[16] = new BaseClasses.CMember(17, m_arrNodes[32], m_arrNodes[33], m_arrCrSc[16], 0);
            m_arrMembers[17] = new BaseClasses.CMember(18, m_arrNodes[34], m_arrNodes[35], m_arrCrSc[17], 0);
            m_arrMembers[18] = new BaseClasses.CMember(19, m_arrNodes[36], m_arrNodes[37], m_arrCrSc[18], 0);
            m_arrMembers[19] = new BaseClasses.CMember(20, m_arrNodes[38], m_arrNodes[39], m_arrCrSc[19], 0);
            m_arrMembers[20] = new BaseClasses.CMember(21, m_arrNodes[40], m_arrNodes[41], m_arrCrSc[20], 0);
            m_arrMembers[21] = new BaseClasses.CMember(22, m_arrNodes[42], m_arrNodes[43], m_arrCrSc[21], 0);
            m_arrMembers[22] = new BaseClasses.CMember(23, m_arrNodes[44], m_arrNodes[45], m_arrCrSc[22], 0);
            m_arrMembers[23] = new BaseClasses.CMember(24, m_arrNodes[46], m_arrNodes[47], m_arrCrSc[23], 0);
            m_arrMembers[24] = new BaseClasses.CMember(25, m_arrNodes[48], m_arrNodes[49], m_arrCrSc[24], 0);
            m_arrMembers[25] = new BaseClasses.CMember(26, m_arrNodes[50], m_arrNodes[51], m_arrCrSc[25], 0);
            m_arrMembers[26] = new BaseClasses.CMember(27, m_arrNodes[52], m_arrNodes[53], m_arrCrSc[26], 0);
            m_arrMembers[27] = new BaseClasses.CMember(28, m_arrNodes[54], m_arrNodes[55], m_arrCrSc[27], 0);
            m_arrMembers[28] = new BaseClasses.CMember(29, m_arrNodes[56], m_arrNodes[57], m_arrCrSc[28], 0);
            m_arrMembers[29] = new BaseClasses.CMember(30, m_arrNodes[58], m_arrNodes[59], m_arrCrSc[29], 0);
            m_arrMembers[30] = new BaseClasses.CMember(31, m_arrNodes[60], m_arrNodes[61], m_arrCrSc[30], 0);


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
            m_arrNSupports[0] = new BaseClasses.CNSupport(6, 1, m_arrNodes[0], bSupport1, 0);
            m_arrNSupports[1] = new BaseClasses.CNSupport(6, 2, m_arrNodes[2], bSupport2, 0);
            m_arrNSupports[2] = new BaseClasses.CNSupport(6, 3, m_arrNodes[5], bSupport3, 0);

            // Sort by ID
            Array.Sort(m_arrNSupports, new BaseClasses.CCompare_NSupportID());
        }
    }
}
