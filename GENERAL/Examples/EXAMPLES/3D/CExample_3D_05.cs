using BaseClasses;
using BaseClasses.CRSC;
using MATERIAL;
using System;

namespace Examples
{
    public class CExample_3D_05 : CExample
    {
        /*
                public CNode[] m_arrNodes = new CNode[68];
                public CMember[] m_arrMembers = new CMember[101];
                public CNSupport[] arrSupports = new CNSupport[2];
                //public CNForce[] arrForces = new CNForce[35];
                int eNDOF = (int)ENDOF.e3DEnv;
         */

        public CExample_3D_05()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new BaseClasses.CNode[135];
            m_arrMembers = new CMember[237];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[21];
            m_arrNSupports = new BaseClasses.CNSupport[2];
            //m_arrNLoads = new BaseClasses.CNLoad[35];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            //m_arrCrSc[0] = new CCrSc_0_05(2.8f, 1.2f);
            m_arrCrSc[0] = new CCrSc_3_07(0,2.8f, 1.2f, 0.08f);
            m_arrCrSc[1] = new CCrSc_3_00(0, 8, 1.500f, 0.500f, 0.080f, 0.050f, 0.050f, 0.030f, 0.640f);
            m_arrCrSc[2] = new CCrSc_0_02(0.4f, 13); // Tie, cable, tendon - solid round section
            //m_arrCrSc[2] = new CCrSc_0_22(0.3f, 0.03f, 36); // Tube
            //m_arrCrSc[3] = new CCrSc_3_07(0, 0.900f, 0.500f, 0.08f);

            m_arrCrSc[3] = new CCrSc_0_05(5.00f, 1.50f);
            m_arrCrSc[4] = new CCrSc_0_05(4.80f, 1.48f);
            m_arrCrSc[5] = new CCrSc_0_05(4.60f, 1.46f);
            m_arrCrSc[6] = new CCrSc_0_05(4.40f, 1.44f);
            m_arrCrSc[7] = new CCrSc_0_05(4.20f, 1.42f);
            m_arrCrSc[8] = new CCrSc_0_05(4.00f, 1.40f);
            m_arrCrSc[9] = new CCrSc_0_05(3.80f, 1.38f);
            m_arrCrSc[10] = new CCrSc_0_05(3.60f, 1.36f);
            m_arrCrSc[11] = new CCrSc_0_05(3.40f, 1.34f);
            m_arrCrSc[12] = new CCrSc_0_05(3.20f, 1.32f);
            m_arrCrSc[13] = new CCrSc_0_05(3.00f, 1.30f);
            m_arrCrSc[14] = new CCrSc_0_05(2.80f, 1.28f);
            m_arrCrSc[15] = new CCrSc_0_05(2.60f, 1.26f);
            m_arrCrSc[16] = new CCrSc_0_05(2.40f, 1.24f);
            m_arrCrSc[17] = new CCrSc_0_05(2.20f, 1.22f);
            m_arrCrSc[18] = new CCrSc_0_05(2.00f, 1.20f);
            m_arrCrSc[19] = new CCrSc_0_05(1.80f, 1.18f);
            m_arrCrSc[20] = new CCrSc_0_05(1.60f, 1.16f);

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Nodes
            m_arrNodes[00] = new CNode(01, 000000, -15000, 00000, 0);
            m_arrNodes[01] = new CNode(02, 008126, -12769, 05354, 0);
            m_arrNodes[02] = new CNode(03, 011471, -15000, 00000, 0);
            m_arrNodes[03] = new CNode(04, 014160, -11258, 08980, 0);
            m_arrNodes[04] = new CNode(05, 019344, -15000, 00000, 0);
            m_arrNodes[05] = new CNode(06, 020317, -9836, 12393, 0);
            m_arrNodes[06] = new CNode(07, 026589, -8505, 15589, 0);
            m_arrNodes[07] = new CNode(08, 026906, -15000, 00000, 0);
            m_arrNodes[08] = new CNode(09, 032969, -7265, 18564, 0);
            m_arrNodes[09] = new CNode(10, 034192, -15000, 00000, 0);
            m_arrNodes[10] = new CNode(11, 039449, -6119, 21314, 0);
            m_arrNodes[11] = new CNode(12, 041234, -15000, 00000, 0);
            m_arrNodes[12] = new CNode(13, 046021, -5068, 23837, 0);
            m_arrNodes[13] = new CNode(14, 048061, -15000, 00000, 0);
            m_arrNodes[14] = new CNode(15, 052677, -4113, 26129, 0);
            m_arrNodes[15] = new CNode(16, 054697, -15000, 00000, 0);
            m_arrNodes[16] = new CNode(17, 059409, -3255, 28187, 0);
            m_arrNodes[17] = new CNode(18, 061167, -15000, 00000, 0);
            m_arrNodes[18] = new CNode(19, 066209, -2496, 30009, 0);
            m_arrNodes[19] = new CNode(20, 067492, -15000, 00000, 0);
            m_arrNodes[20] = new CNode(21, 073068, -1836, 31593, 0);
            m_arrNodes[21] = new CNode(22, 073691, -15000, 00000, 0);
            m_arrNodes[22] = new CNode(23, 079784, -15000, 00000, 0);
            m_arrNodes[23] = new CNode(24, 079979, -1277, 32936, 0);
            m_arrNodes[24] = new CNode(25, 085786, -15000, 00000, 0);
            m_arrNodes[25] = new CNode(26, 086931, -818, 34037, 0);
            m_arrNodes[26] = new CNode(27, 091715, -15000, 00000, 0);
            m_arrNodes[27] = new CNode(28, 093919, -460, 34895, 0);
            m_arrNodes[28] = new CNode(29, 097586, -15000, 00000, 0);
            m_arrNodes[29] = new CNode(30, 100931, -205, 35509, 0);
            m_arrNodes[30] = new CNode(31, 103414, -15000, 00000, 0);
            m_arrNodes[31] = new CNode(32, 107961, -51, 35877, 0);
            m_arrNodes[32] = new CNode(33, 109214, -15000, 00000, 0);
            m_arrNodes[33] = new CNode(34, 115000,  0, 36000, 0);
            m_arrNodes[34] = new CNode(35, 115000, -15000, 00000, 0);
            m_arrNodes[35] = new CNode(36, 120786, -15000, 00000, 0);
            m_arrNodes[36] = new CNode(37, 122039, -51, 35877, 0);
            m_arrNodes[37] = new CNode(38, 126586, -15000, 00000, 0);
            m_arrNodes[38] = new CNode(39, 129069, -205, 35509, 0);
            m_arrNodes[39] = new CNode(40, 132414, -15000, 00000, 0);
            m_arrNodes[40] = new CNode(41, 136081, -460, 34895, 0);
            m_arrNodes[41] = new CNode(42, 138285, -15000, 00000, 0);
            m_arrNodes[42] = new CNode(43, 143069, -818, 34037, 0);
            m_arrNodes[43] = new CNode(44, 144214, -15000, 00000, 0);
            m_arrNodes[44] = new CNode(45, 150021, -1277, 32936, 0);
            m_arrNodes[45] = new CNode(46, 150216, -15000, 00000, 0);
            m_arrNodes[46] = new CNode(47, 156309, -15000, 00000, 0);
            m_arrNodes[47] = new CNode(48, 156932, -1836, 31593, 0);
            m_arrNodes[48] = new CNode(49, 162508, -15000, 00000, 0);
            m_arrNodes[49] = new CNode(50, 163791, -2496, 30009, 0);
            m_arrNodes[50] = new CNode(51, 168833, -15000, 00000, 0);
            m_arrNodes[51] = new CNode(52, 170591, -3255, 28187, 0);
            m_arrNodes[52] = new CNode(53, 175303, -15000, 00000, 0);
            m_arrNodes[53] = new CNode(54, 177323, -4113, 26129, 0);
            m_arrNodes[54] = new CNode(55, 181939, -15000, 00000, 0);
            m_arrNodes[55] = new CNode(56, 183979, -5068, 23837, 0);
            m_arrNodes[56] = new CNode(57, 188766, -15000, 00000, 0);
            m_arrNodes[57] = new CNode(58, 190551, -6119, 21314, 0);
            m_arrNodes[58] = new CNode(59, 195808, -15000, 00000, 0);
            m_arrNodes[59] = new CNode(60, 197031, -7265, 18564, 0);
            m_arrNodes[60] = new CNode(61, 203094, -15000, 00000, 0);
            m_arrNodes[61] = new CNode(62, 203411, -8505, 15589, 0);
            m_arrNodes[62] = new CNode(63, 209683, -9836, 12393, 0);
            m_arrNodes[63] = new CNode(64, 210656, -15000, 00000, 0);
            m_arrNodes[64] = new CNode(65, 215840, -11258, 08980, 0);
            m_arrNodes[65] = new CNode(66, 218529, -15000, 00000, 0);
            m_arrNodes[66] = new CNode(67, 221874, -12769, 05354, 0);
            m_arrNodes[67] = new CNode(68, 230000, -15000, 00000, 0);
            m_arrNodes[068] = new CNode(069, 000000, 15000, 00000, 0);
            m_arrNodes[069] = new CNode(070, 008126, 12769, 05354, 0);
            m_arrNodes[070] = new CNode(071, 011471, 15000, 00000, 0);
            m_arrNodes[071] = new CNode(072, 014160, 11258, 08980, 0);
            m_arrNodes[072] = new CNode(073, 019344, 15000, 00000, 0);
            m_arrNodes[073] = new CNode(074, 020317, 09836, 12393, 0);
            m_arrNodes[074] = new CNode(075, 026589, 08505, 15589, 0);
            m_arrNodes[075] = new CNode(076, 026906, 15000, 00000, 0);
            m_arrNodes[076] = new CNode(077, 032969, 07265, 18564, 0);
            m_arrNodes[077] = new CNode(078, 034192, 15000, 00000, 0);
            m_arrNodes[078] = new CNode(079, 039449, 06119, 21314, 0);
            m_arrNodes[079] = new CNode(080, 041234, 15000, 00000, 0);
            m_arrNodes[080] = new CNode(081, 046021, 05068, 23837, 0);
            m_arrNodes[081] = new CNode(082, 048061, 15000, 00000, 0);
            m_arrNodes[082] = new CNode(083, 052677, 04113, 26129, 0);
            m_arrNodes[083] = new CNode(084, 054697, 15000, 00000, 0);
            m_arrNodes[084] = new CNode(085, 059409, 03255, 28187, 0);
            m_arrNodes[085] = new CNode(086, 061167, 15000, 00000, 0);
            m_arrNodes[086] = new CNode(087, 066209, 02496, 30009, 0);
            m_arrNodes[087] = new CNode(088, 067492, 15000, 00000, 0);
            m_arrNodes[088] = new CNode(089, 073068, 01836, 31593, 0);
            m_arrNodes[089] = new CNode(090, 073691, 15000, 00000, 0);
            m_arrNodes[090] = new CNode(091, 079784, 15000, 00000, 0);
            m_arrNodes[091] = new CNode(092, 079979, 01277, 32936, 0);
            m_arrNodes[092] = new CNode(093, 085786, 15000, 00000, 0);
            m_arrNodes[093] = new CNode(094, 086931, 00818, 34037, 0);
            m_arrNodes[094] = new CNode(095, 091715, 15000, 00000, 0);
            m_arrNodes[095] = new CNode(096, 093919, 00460, 34895, 0);
            m_arrNodes[096] = new CNode(097, 097586, 15000, 00000, 0);
            m_arrNodes[097] = new CNode(098, 100931, 00205, 35509, 0);
            m_arrNodes[098] = new CNode(099, 103414, 15000, 00000, 0);
            m_arrNodes[099] = new CNode(100, 107961, 00051, 35877, 0);
            m_arrNodes[100] = new CNode(101, 109214, 15000, 00000, 0);
            m_arrNodes[101] = new CNode(102, 115000, 15000, 00000, 0);
            m_arrNodes[102] = new CNode(103, 120786, 15000, 00000, 0);
            m_arrNodes[103] = new CNode(104, 122039, 00051, 35877, 0);
            m_arrNodes[104] = new CNode(105, 126586, 15000, 00000, 0);
            m_arrNodes[105] = new CNode(106, 129069, 00205, 35509, 0);
            m_arrNodes[106] = new CNode(107, 132414, 15000, 00000, 0);
            m_arrNodes[107] = new CNode(108, 136081, 00460, 34895, 0);
            m_arrNodes[108] = new CNode(109, 138285, 15000, 00000, 0);
            m_arrNodes[109] = new CNode(110, 143069, 00818, 34037, 0);
            m_arrNodes[110] = new CNode(111, 144214, 15000, 00000, 0);
            m_arrNodes[111] = new CNode(112, 150021, 01277, 32936, 0);
            m_arrNodes[112] = new CNode(113, 150216, 15000, 00000, 0);
            m_arrNodes[113] = new CNode(114, 156309, 15000, 00000, 0);
            m_arrNodes[114] = new CNode(115, 156932, 01836, 31593, 0);
            m_arrNodes[115] = new CNode(116, 162508, 15000, 00000, 0);
            m_arrNodes[116] = new CNode(117, 163791, 02496, 30009, 0);
            m_arrNodes[117] = new CNode(118, 168833, 15000, 00000, 0);
            m_arrNodes[118] = new CNode(119, 170591, 03255, 28187, 0);
            m_arrNodes[119] = new CNode(120, 175303, 15000, 00000, 0);
            m_arrNodes[120] = new CNode(121, 177323, 04113, 26129, 0);
            m_arrNodes[121] = new CNode(122, 181939, 15000, 00000, 0);
            m_arrNodes[122] = new CNode(123, 183979, 05068, 23837, 0);
            m_arrNodes[123] = new CNode(124, 188766, 15000, 00000, 0);
            m_arrNodes[124] = new CNode(125, 190551, 06119, 21314, 0);
            m_arrNodes[125] = new CNode(126, 195808, 15000, 00000, 0);
            m_arrNodes[126] = new CNode(127, 197031, 07265, 18564, 0);
            m_arrNodes[127] = new CNode(128, 203094, 15000, 00000, 0);
            m_arrNodes[128] = new CNode(129, 203411, 08505, 15589, 0);
            m_arrNodes[129] = new CNode(130, 209683, 09836, 12393, 0);
            m_arrNodes[130] = new CNode(131, 210656, 15000, 00000, 0);
            m_arrNodes[131] = new CNode(132, 215840, 11258, 08980, 0);
            m_arrNodes[132] = new CNode(133, 218529, 15000, 00000, 0);
            m_arrNodes[133] = new CNode(134, 221874, 12769, 05354, 0);
            m_arrNodes[134] = new CNode(135, 230000, 15000, 00000, 0);

            // Convert coordinates to meters
            foreach (CNode node in m_arrNodes)
            {
                node.X /= 1000;
                node.Y /= 1000;
                node.Z /= 1000;
            }

            // Setridit pole podle ID
            //Array.Sort(m_arrNodes, new CCompare_NodeID());

            // Members Automatic Generation
            // Members List - Members Array

            // Members
            m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[3], m_arrCrSc[4],0 ,0 , 0);
            m_arrMembers[001] = new CMember(002, m_arrNodes[00], m_arrNodes[02], m_arrCrSc[0], 0);
            m_arrMembers[002] = new CMember(003, m_arrNodes[02], m_arrNodes[01], m_arrCrSc[2], 0);
            m_arrMembers[003] = new CMember(004, m_arrNodes[01], m_arrNodes[03], m_arrCrSc[4], m_arrCrSc[5], 0, 0, 0);
            m_arrMembers[004] = new CMember(005, m_arrNodes[02], m_arrNodes[04], m_arrCrSc[0], 0);
            m_arrMembers[005] = new CMember(006, m_arrNodes[04], m_arrNodes[03], m_arrCrSc[2], 0);
            m_arrMembers[006] = new CMember(007, m_arrNodes[03], m_arrNodes[05], m_arrCrSc[5], m_arrCrSc[6], 0, 0, 0);
            m_arrMembers[007] = new CMember(008, m_arrNodes[04], m_arrNodes[07], m_arrCrSc[0], 0);
            m_arrMembers[008] = new CMember(009, m_arrNodes[05], m_arrNodes[06], m_arrCrSc[6], m_arrCrSc[7], 0, 0, 0);
            m_arrMembers[009] = new CMember(010, m_arrNodes[07], m_arrNodes[05], m_arrCrSc[2], 0);
            m_arrMembers[010] = new CMember(011, m_arrNodes[06], m_arrNodes[08], m_arrCrSc[7], m_arrCrSc[8], 0, 0, 0);
            m_arrMembers[011] = new CMember(012, m_arrNodes[06], m_arrNodes[09], m_arrCrSc[2], 0);
            m_arrMembers[012] = new CMember(013, m_arrNodes[07], m_arrNodes[09], m_arrCrSc[0], 0);
            m_arrMembers[013] = new CMember(014, m_arrNodes[08], m_arrNodes[10], m_arrCrSc[8], m_arrCrSc[9], 0, 0, 0);
            m_arrMembers[014] = new CMember(015, m_arrNodes[08], m_arrNodes[11], m_arrCrSc[2], 0);
            m_arrMembers[015] = new CMember(016, m_arrNodes[09], m_arrNodes[11], m_arrCrSc[0], 0);
            m_arrMembers[016] = new CMember(017, m_arrNodes[10], m_arrNodes[12], m_arrCrSc[9], m_arrCrSc[10], 0, 0, 0);
            m_arrMembers[017] = new CMember(018, m_arrNodes[10], m_arrNodes[13], m_arrCrSc[2], 0);
            m_arrMembers[018] = new CMember(019, m_arrNodes[11], m_arrNodes[13], m_arrCrSc[0], 0);
            m_arrMembers[019] = new CMember(020, m_arrNodes[12], m_arrNodes[14], m_arrCrSc[10], m_arrCrSc[11], 0, 0, 0);
            m_arrMembers[020] = new CMember(021, m_arrNodes[12], m_arrNodes[15], m_arrCrSc[2], 0);
            m_arrMembers[021] = new CMember(022, m_arrNodes[13], m_arrNodes[15], m_arrCrSc[0], 0);
            m_arrMembers[022] = new CMember(023, m_arrNodes[14], m_arrNodes[16], m_arrCrSc[11], m_arrCrSc[12], 0, 0, 0);
            m_arrMembers[023] = new CMember(024, m_arrNodes[14], m_arrNodes[17], m_arrCrSc[2], 0);
            m_arrMembers[024] = new CMember(025, m_arrNodes[15], m_arrNodes[17], m_arrCrSc[0], 0);
            m_arrMembers[025] = new CMember(026, m_arrNodes[16], m_arrNodes[18], m_arrCrSc[12], m_arrCrSc[13], 0, 0, 0);
            m_arrMembers[026] = new CMember(027, m_arrNodes[16], m_arrNodes[19], m_arrCrSc[2], 0);
            m_arrMembers[027] = new CMember(028, m_arrNodes[17], m_arrNodes[19], m_arrCrSc[0], 0);
            m_arrMembers[028] = new CMember(029, m_arrNodes[18], m_arrNodes[20], m_arrCrSc[13], m_arrCrSc[14], 0, 0, 0);
            m_arrMembers[029] = new CMember(030, m_arrNodes[18], m_arrNodes[21], m_arrCrSc[2], 0);
            m_arrMembers[030] = new CMember(031, m_arrNodes[19], m_arrNodes[21], m_arrCrSc[0], 0);
            m_arrMembers[031] = new CMember(032, m_arrNodes[20], m_arrNodes[22], m_arrCrSc[2], 0);
            m_arrMembers[032] = new CMember(033, m_arrNodes[20], m_arrNodes[23], m_arrCrSc[14], m_arrCrSc[15], 0, 0, 0);
            m_arrMembers[033] = new CMember(034, m_arrNodes[21], m_arrNodes[22], m_arrCrSc[0], 0);
            m_arrMembers[034] = new CMember(035, m_arrNodes[22], m_arrNodes[24], m_arrCrSc[0], 0);
            m_arrMembers[035] = new CMember(036, m_arrNodes[23], m_arrNodes[24], m_arrCrSc[2], 0);
            m_arrMembers[036] = new CMember(037, m_arrNodes[23], m_arrNodes[25], m_arrCrSc[15], m_arrCrSc[16], 0, 0, 0);
            m_arrMembers[037] = new CMember(038, m_arrNodes[24], m_arrNodes[26], m_arrCrSc[0], 0);
            m_arrMembers[038] = new CMember(039, m_arrNodes[25], m_arrNodes[26], m_arrCrSc[2], 0);
            m_arrMembers[039] = new CMember(040, m_arrNodes[25], m_arrNodes[27], m_arrCrSc[16], m_arrCrSc[17], 0, 0, 0);
            m_arrMembers[040] = new CMember(041, m_arrNodes[26], m_arrNodes[28], m_arrCrSc[0], 0);
            m_arrMembers[041] = new CMember(042, m_arrNodes[27], m_arrNodes[28], m_arrCrSc[2], 0);
            m_arrMembers[042] = new CMember(043, m_arrNodes[27], m_arrNodes[29], m_arrCrSc[17], m_arrCrSc[18], 0, 0, 0);
            m_arrMembers[043] = new CMember(044, m_arrNodes[28], m_arrNodes[30], m_arrCrSc[0], 0);
            m_arrMembers[044] = new CMember(045, m_arrNodes[29], m_arrNodes[30], m_arrCrSc[2], 0);
            m_arrMembers[045] = new CMember(046, m_arrNodes[29], m_arrNodes[31], m_arrCrSc[18], m_arrCrSc[19], 0, 0, 0);
            m_arrMembers[046] = new CMember(047, m_arrNodes[30], m_arrNodes[32], m_arrCrSc[0], 0);
            m_arrMembers[047] = new CMember(048, m_arrNodes[31], m_arrNodes[32], m_arrCrSc[2], 0);
            m_arrMembers[048] = new CMember(049, m_arrNodes[31], m_arrNodes[33], m_arrCrSc[19], m_arrCrSc[20], 0, 0, 0);
            m_arrMembers[049] = new CMember(050, m_arrNodes[32], m_arrNodes[34], m_arrCrSc[0], 0);
            m_arrMembers[050] = new CMember(051, m_arrNodes[33], m_arrNodes[34], m_arrCrSc[2], 0);
            m_arrMembers[051] = new CMember(052, m_arrNodes[34], m_arrNodes[35], m_arrCrSc[0], 0);
            m_arrMembers[052] = new CMember(053, m_arrNodes[33], m_arrNodes[36], m_arrCrSc[20], m_arrCrSc[19], 0, 0, 0);
            m_arrMembers[053] = new CMember(054, m_arrNodes[35], m_arrNodes[36], m_arrCrSc[2], 0);
            m_arrMembers[054] = new CMember(055, m_arrNodes[35], m_arrNodes[37], m_arrCrSc[0], 0);
            m_arrMembers[055] = new CMember(056, m_arrNodes[36], m_arrNodes[38], m_arrCrSc[19], m_arrCrSc[18], 0, 0, 0);
            m_arrMembers[056] = new CMember(057, m_arrNodes[37], m_arrNodes[38], m_arrCrSc[2], 0);
            m_arrMembers[057] = new CMember(058, m_arrNodes[37], m_arrNodes[39], m_arrCrSc[0], 0);
            m_arrMembers[058] = new CMember(059, m_arrNodes[38], m_arrNodes[40], m_arrCrSc[18], m_arrCrSc[17], 0, 0, 0);
            m_arrMembers[059] = new CMember(060, m_arrNodes[39], m_arrNodes[40], m_arrCrSc[2], 0);
            m_arrMembers[060] = new CMember(061, m_arrNodes[39], m_arrNodes[41], m_arrCrSc[0], 0);
            m_arrMembers[061] = new CMember(062, m_arrNodes[40], m_arrNodes[42], m_arrCrSc[17], m_arrCrSc[16], 0, 0, 0);
            m_arrMembers[062] = new CMember(063, m_arrNodes[41], m_arrNodes[42], m_arrCrSc[2], 0);
            m_arrMembers[063] = new CMember(064, m_arrNodes[41], m_arrNodes[43], m_arrCrSc[0], 0);
            m_arrMembers[064] = new CMember(065, m_arrNodes[42], m_arrNodes[44], m_arrCrSc[16], m_arrCrSc[15], 0, 0, 0);
            m_arrMembers[065] = new CMember(066, m_arrNodes[43], m_arrNodes[44], m_arrCrSc[2], 0);
            m_arrMembers[066] = new CMember(067, m_arrNodes[43], m_arrNodes[45], m_arrCrSc[0], 0);
            m_arrMembers[067] = new CMember(068, m_arrNodes[45], m_arrNodes[46], m_arrCrSc[0], 0);
            m_arrMembers[068] = new CMember(069, m_arrNodes[44], m_arrNodes[47], m_arrCrSc[15], m_arrCrSc[14], 0, 0, 0);
            m_arrMembers[069] = new CMember(070, m_arrNodes[45], m_arrNodes[47], m_arrCrSc[2], 0);
            m_arrMembers[070] = new CMember(071, m_arrNodes[46], m_arrNodes[48], m_arrCrSc[0], 0);
            m_arrMembers[071] = new CMember(072, m_arrNodes[46], m_arrNodes[49], m_arrCrSc[2], 0);
            m_arrMembers[072] = new CMember(073, m_arrNodes[47], m_arrNodes[49], m_arrCrSc[14], m_arrCrSc[13], 0, 0, 0);
            m_arrMembers[073] = new CMember(074, m_arrNodes[48], m_arrNodes[50], m_arrCrSc[0], 0);
            m_arrMembers[074] = new CMember(075, m_arrNodes[48], m_arrNodes[51], m_arrCrSc[2], 0);
            m_arrMembers[075] = new CMember(076, m_arrNodes[49], m_arrNodes[51], m_arrCrSc[13], m_arrCrSc[12], 0, 0, 0);
            m_arrMembers[076] = new CMember(077, m_arrNodes[50], m_arrNodes[52], m_arrCrSc[0], 0);
            m_arrMembers[077] = new CMember(078, m_arrNodes[50], m_arrNodes[53], m_arrCrSc[2], 0);
            m_arrMembers[078] = new CMember(079, m_arrNodes[51], m_arrNodes[53], m_arrCrSc[12], m_arrCrSc[11], 0, 0, 0);
            m_arrMembers[079] = new CMember(080, m_arrNodes[52], m_arrNodes[54], m_arrCrSc[0], 0);
            m_arrMembers[080] = new CMember(081, m_arrNodes[52], m_arrNodes[55], m_arrCrSc[2], 0);
            m_arrMembers[081] = new CMember(082, m_arrNodes[53], m_arrNodes[55], m_arrCrSc[11], m_arrCrSc[10], 0, 0, 0);
            m_arrMembers[082] = new CMember(083, m_arrNodes[54], m_arrNodes[56], m_arrCrSc[0], 0);
            m_arrMembers[083] = new CMember(084, m_arrNodes[54], m_arrNodes[57], m_arrCrSc[2], 0);
            m_arrMembers[084] = new CMember(085, m_arrNodes[55], m_arrNodes[57], m_arrCrSc[10], m_arrCrSc[09], 0, 0, 0);
            m_arrMembers[085] = new CMember(086, m_arrNodes[56], m_arrNodes[58], m_arrCrSc[0], 0);
            m_arrMembers[086] = new CMember(087, m_arrNodes[56], m_arrNodes[59], m_arrCrSc[2], 0);
            m_arrMembers[087] = new CMember(088, m_arrNodes[57], m_arrNodes[59], m_arrCrSc[09], m_arrCrSc[08], 0, 0, 0);
            m_arrMembers[088] = new CMember(089, m_arrNodes[58], m_arrNodes[60], m_arrCrSc[0], 0);
            m_arrMembers[089] = new CMember(090, m_arrNodes[58], m_arrNodes[61], m_arrCrSc[2], 0);
            m_arrMembers[090] = new CMember(091, m_arrNodes[59], m_arrNodes[61], m_arrCrSc[08], m_arrCrSc[07], 0, 0, 0);
            m_arrMembers[091] = new CMember(092, m_arrNodes[62], m_arrNodes[60], m_arrCrSc[2], 0);
            m_arrMembers[092] = new CMember(093, m_arrNodes[61], m_arrNodes[62], m_arrCrSc[07], m_arrCrSc[06], 0, 0, 0);
            m_arrMembers[093] = new CMember(094, m_arrNodes[60], m_arrNodes[63], m_arrCrSc[0], 0);
            m_arrMembers[094] = new CMember(095, m_arrNodes[62], m_arrNodes[64], m_arrCrSc[06], m_arrCrSc[05], 0, 0, 0);
            m_arrMembers[095] = new CMember(096, m_arrNodes[64], m_arrNodes[63], m_arrCrSc[2], 0);
            m_arrMembers[096] = new CMember(097, m_arrNodes[63], m_arrNodes[65], m_arrCrSc[0], 0);
            m_arrMembers[097] = new CMember(098, m_arrNodes[64], m_arrNodes[66], m_arrCrSc[05], m_arrCrSc[04], 0, 0, 0);
            m_arrMembers[098] = new CMember(099, m_arrNodes[66], m_arrNodes[65], m_arrCrSc[2], 0);
            m_arrMembers[099] = new CMember(100, m_arrNodes[65], m_arrNodes[67], m_arrCrSc[0], 0);
            m_arrMembers[100] = new CMember(101, m_arrNodes[66], m_arrNodes[67], m_arrCrSc[04], m_arrCrSc[3], 0, 0, 0);
            m_arrMembers[101] = new CMember(102, m_arrNodes[068], m_arrNodes[069], m_arrCrSc[3], m_arrCrSc[4], 0, 0, 0);
            m_arrMembers[102] = new CMember(103, m_arrNodes[068], m_arrNodes[070], m_arrCrSc[0], 0);
            m_arrMembers[103] = new CMember(104, m_arrNodes[070], m_arrNodes[069], m_arrCrSc[2], 0);
            m_arrMembers[104] = new CMember(105, m_arrNodes[069], m_arrNodes[071], m_arrCrSc[4], m_arrCrSc[5], 0, 0, 0);
            m_arrMembers[105] = new CMember(106, m_arrNodes[070], m_arrNodes[072], m_arrCrSc[0], 0);
            m_arrMembers[106] = new CMember(107, m_arrNodes[072], m_arrNodes[071], m_arrCrSc[2], 0);
            m_arrMembers[107] = new CMember(108, m_arrNodes[071], m_arrNodes[073], m_arrCrSc[5], m_arrCrSc[6], 0, 0, 0);
            m_arrMembers[108] = new CMember(109, m_arrNodes[072], m_arrNodes[075], m_arrCrSc[0], 0);
            m_arrMembers[109] = new CMember(110, m_arrNodes[073], m_arrNodes[074], m_arrCrSc[6], m_arrCrSc[7], 0, 0, 0);
            m_arrMembers[110] = new CMember(111, m_arrNodes[075], m_arrNodes[073], m_arrCrSc[2], 0);
            m_arrMembers[111] = new CMember(112, m_arrNodes[074], m_arrNodes[076], m_arrCrSc[7], m_arrCrSc[8], 0, 0, 0);
            m_arrMembers[112] = new CMember(113, m_arrNodes[074], m_arrNodes[077], m_arrCrSc[2], 0);
            m_arrMembers[113] = new CMember(114, m_arrNodes[075], m_arrNodes[077], m_arrCrSc[0], 0);
            m_arrMembers[114] = new CMember(115, m_arrNodes[076], m_arrNodes[078], m_arrCrSc[8], m_arrCrSc[9], 0, 0, 0);
            m_arrMembers[115] = new CMember(116, m_arrNodes[076], m_arrNodes[079], m_arrCrSc[2], 0);
            m_arrMembers[116] = new CMember(117, m_arrNodes[077], m_arrNodes[079], m_arrCrSc[0], 0);
            m_arrMembers[117] = new CMember(118, m_arrNodes[078], m_arrNodes[080], m_arrCrSc[9], m_arrCrSc[10], 0, 0, 0);
            m_arrMembers[118] = new CMember(119, m_arrNodes[078], m_arrNodes[081], m_arrCrSc[2], 0);
            m_arrMembers[119] = new CMember(120, m_arrNodes[079], m_arrNodes[081], m_arrCrSc[0], 0);
            m_arrMembers[120] = new CMember(121, m_arrNodes[080], m_arrNodes[082], m_arrCrSc[10], m_arrCrSc[11], 0, 0, 0);
            m_arrMembers[121] = new CMember(122, m_arrNodes[080], m_arrNodes[083], m_arrCrSc[2], 0);
            m_arrMembers[122] = new CMember(123, m_arrNodes[081], m_arrNodes[083], m_arrCrSc[0], 0);
            m_arrMembers[123] = new CMember(124, m_arrNodes[082], m_arrNodes[084], m_arrCrSc[11], m_arrCrSc[12], 0, 0, 0);
            m_arrMembers[124] = new CMember(125, m_arrNodes[082], m_arrNodes[085], m_arrCrSc[2], 0);
            m_arrMembers[125] = new CMember(126, m_arrNodes[083], m_arrNodes[085], m_arrCrSc[0], 0);
            m_arrMembers[126] = new CMember(127, m_arrNodes[084], m_arrNodes[086], m_arrCrSc[12], m_arrCrSc[13], 0, 0, 0);
            m_arrMembers[127] = new CMember(128, m_arrNodes[084], m_arrNodes[087], m_arrCrSc[2], 0);
            m_arrMembers[128] = new CMember(129, m_arrNodes[085], m_arrNodes[087], m_arrCrSc[0], 0);
            m_arrMembers[129] = new CMember(130, m_arrNodes[086], m_arrNodes[088], m_arrCrSc[13], m_arrCrSc[14], 0, 0, 0);
            m_arrMembers[130] = new CMember(131, m_arrNodes[086], m_arrNodes[089], m_arrCrSc[2], 0);
            m_arrMembers[131] = new CMember(132, m_arrNodes[087], m_arrNodes[089], m_arrCrSc[0], 0);
            m_arrMembers[132] = new CMember(133, m_arrNodes[088], m_arrNodes[090], m_arrCrSc[2], 0);
            m_arrMembers[133] = new CMember(134, m_arrNodes[088], m_arrNodes[091], m_arrCrSc[14], m_arrCrSc[15], 0, 0, 0);
            m_arrMembers[134] = new CMember(135, m_arrNodes[089], m_arrNodes[090], m_arrCrSc[0], 0);
            m_arrMembers[135] = new CMember(136, m_arrNodes[090], m_arrNodes[092], m_arrCrSc[0], 0);
            m_arrMembers[136] = new CMember(137, m_arrNodes[091], m_arrNodes[092], m_arrCrSc[2], 0);
            m_arrMembers[137] = new CMember(138, m_arrNodes[091], m_arrNodes[093], m_arrCrSc[15], m_arrCrSc[16], 0, 0, 0);
            m_arrMembers[138] = new CMember(139, m_arrNodes[092], m_arrNodes[094], m_arrCrSc[0], 0);
            m_arrMembers[139] = new CMember(140, m_arrNodes[093], m_arrNodes[094], m_arrCrSc[2], 0);
            m_arrMembers[140] = new CMember(141, m_arrNodes[093], m_arrNodes[095], m_arrCrSc[16], m_arrCrSc[17], 0, 0, 0);
            m_arrMembers[141] = new CMember(142, m_arrNodes[094], m_arrNodes[096], m_arrCrSc[0], 0);
            m_arrMembers[142] = new CMember(143, m_arrNodes[095], m_arrNodes[096], m_arrCrSc[2], 0);
            m_arrMembers[143] = new CMember(144, m_arrNodes[095], m_arrNodes[097], m_arrCrSc[17], m_arrCrSc[18], 0, 0, 0);
            m_arrMembers[144] = new CMember(145, m_arrNodes[096], m_arrNodes[098], m_arrCrSc[0], 0);
            m_arrMembers[145] = new CMember(146, m_arrNodes[097], m_arrNodes[098], m_arrCrSc[2], 0);
            m_arrMembers[146] = new CMember(147, m_arrNodes[097], m_arrNodes[099], m_arrCrSc[18], m_arrCrSc[19], 0, 0, 0);
            m_arrMembers[147] = new CMember(148, m_arrNodes[098], m_arrNodes[100], m_arrCrSc[0], 0);
            m_arrMembers[148] = new CMember(149, m_arrNodes[099], m_arrNodes[100], m_arrCrSc[2], 0);
            m_arrMembers[149] = new CMember(150, m_arrNodes[099], m_arrNodes[033], m_arrCrSc[19], m_arrCrSc[20], 0, 0, 0);
            m_arrMembers[150] = new CMember(151, m_arrNodes[100], m_arrNodes[101], m_arrCrSc[0], 0);
            m_arrMembers[151] = new CMember(152, m_arrNodes[033], m_arrNodes[101], m_arrCrSc[2], 0);
            m_arrMembers[152] = new CMember(153, m_arrNodes[033], m_arrNodes[103], m_arrCrSc[20], m_arrCrSc[19], 0, 0, 0);
            m_arrMembers[153] = new CMember(154, m_arrNodes[101], m_arrNodes[102], m_arrCrSc[0], 0);
            m_arrMembers[154] = new CMember(155, m_arrNodes[102], m_arrNodes[103], m_arrCrSc[2], 0);
            m_arrMembers[155] = new CMember(156, m_arrNodes[102], m_arrNodes[104], m_arrCrSc[0], 0);
            m_arrMembers[156] = new CMember(157, m_arrNodes[103], m_arrNodes[105], m_arrCrSc[19], m_arrCrSc[18], 0, 0, 0);
            m_arrMembers[157] = new CMember(158, m_arrNodes[104], m_arrNodes[105], m_arrCrSc[2], 0);
            m_arrMembers[158] = new CMember(159, m_arrNodes[104], m_arrNodes[106], m_arrCrSc[0], 0);
            m_arrMembers[159] = new CMember(160, m_arrNodes[105], m_arrNodes[107], m_arrCrSc[18], m_arrCrSc[17], 0, 0, 0);
            m_arrMembers[160] = new CMember(161, m_arrNodes[106], m_arrNodes[107], m_arrCrSc[2], 0);
            m_arrMembers[161] = new CMember(162, m_arrNodes[106], m_arrNodes[108], m_arrCrSc[0], 0);
            m_arrMembers[162] = new CMember(163, m_arrNodes[107], m_arrNodes[109], m_arrCrSc[17], m_arrCrSc[16], 0, 0, 0);
            m_arrMembers[163] = new CMember(164, m_arrNodes[108], m_arrNodes[109], m_arrCrSc[2], 0);
            m_arrMembers[164] = new CMember(165, m_arrNodes[108], m_arrNodes[110], m_arrCrSc[0], 0);
            m_arrMembers[165] = new CMember(166, m_arrNodes[109], m_arrNodes[111], m_arrCrSc[16], m_arrCrSc[15], 0, 0, 0);
            m_arrMembers[166] = new CMember(167, m_arrNodes[110], m_arrNodes[111], m_arrCrSc[2], 0);
            m_arrMembers[167] = new CMember(168, m_arrNodes[110], m_arrNodes[112], m_arrCrSc[0], 0);
            m_arrMembers[168] = new CMember(169, m_arrNodes[111], m_arrNodes[114], m_arrCrSc[15], m_arrCrSc[14], 0, 0, 0);
            m_arrMembers[169] = new CMember(170, m_arrNodes[112], m_arrNodes[114], m_arrCrSc[2], 0);
            m_arrMembers[170] = new CMember(171, m_arrNodes[112], m_arrNodes[113], m_arrCrSc[0], 0);
            m_arrMembers[171] = new CMember(172, m_arrNodes[114], m_arrNodes[116], m_arrCrSc[14], m_arrCrSc[13], 0, 0, 0);
            m_arrMembers[172] = new CMember(173, m_arrNodes[113], m_arrNodes[116], m_arrCrSc[2], 0);
            m_arrMembers[173] = new CMember(174, m_arrNodes[113], m_arrNodes[115], m_arrCrSc[0], 0);
            m_arrMembers[174] = new CMember(175, m_arrNodes[116], m_arrNodes[118], m_arrCrSc[13], m_arrCrSc[12], 0, 0, 0);
            m_arrMembers[175] = new CMember(176, m_arrNodes[115], m_arrNodes[118], m_arrCrSc[2], 0);
            m_arrMembers[176] = new CMember(177, m_arrNodes[115], m_arrNodes[117], m_arrCrSc[0], 0);
            m_arrMembers[177] = new CMember(178, m_arrNodes[118], m_arrNodes[120], m_arrCrSc[12], m_arrCrSc[11], 0, 0, 0);
            m_arrMembers[178] = new CMember(179, m_arrNodes[117], m_arrNodes[120], m_arrCrSc[2], 0);
            m_arrMembers[179] = new CMember(180, m_arrNodes[117], m_arrNodes[119], m_arrCrSc[0], 0);
            m_arrMembers[180] = new CMember(181, m_arrNodes[120], m_arrNodes[122], m_arrCrSc[11], m_arrCrSc[10], 0, 0, 0);
            m_arrMembers[181] = new CMember(182, m_arrNodes[119], m_arrNodes[122], m_arrCrSc[2], 0);
            m_arrMembers[182] = new CMember(183, m_arrNodes[119], m_arrNodes[121], m_arrCrSc[0], 0);
            m_arrMembers[183] = new CMember(184, m_arrNodes[122], m_arrNodes[124], m_arrCrSc[10], m_arrCrSc[9], 0, 0, 0);
            m_arrMembers[184] = new CMember(185, m_arrNodes[121], m_arrNodes[124], m_arrCrSc[2], 0);
            m_arrMembers[185] = new CMember(186, m_arrNodes[121], m_arrNodes[123], m_arrCrSc[0], 0);
            m_arrMembers[186] = new CMember(187, m_arrNodes[124], m_arrNodes[126], m_arrCrSc[9], m_arrCrSc[8], 0, 0, 0);
            m_arrMembers[187] = new CMember(188, m_arrNodes[123], m_arrNodes[126], m_arrCrSc[2], 0);
            m_arrMembers[188] = new CMember(189, m_arrNodes[123], m_arrNodes[125], m_arrCrSc[0], 0);
            m_arrMembers[189] = new CMember(190, m_arrNodes[126], m_arrNodes[128], m_arrCrSc[8], m_arrCrSc[7], 0, 0, 0);
            m_arrMembers[190] = new CMember(191, m_arrNodes[125], m_arrNodes[128], m_arrCrSc[2], 0);
            m_arrMembers[191] = new CMember(192, m_arrNodes[125], m_arrNodes[127], m_arrCrSc[0], 0);
            m_arrMembers[192] = new CMember(193, m_arrNodes[128], m_arrNodes[129], m_arrCrSc[7], m_arrCrSc[6], 0, 0, 0);
            m_arrMembers[193] = new CMember(194, m_arrNodes[127], m_arrNodes[129], m_arrCrSc[2], 0);
            m_arrMembers[194] = new CMember(195, m_arrNodes[127], m_arrNodes[130], m_arrCrSc[0], 0);
            m_arrMembers[195] = new CMember(196, m_arrNodes[129], m_arrNodes[131], m_arrCrSc[6], m_arrCrSc[5], 0, 0, 0);
            m_arrMembers[196] = new CMember(197, m_arrNodes[130], m_arrNodes[131], m_arrCrSc[2], 0);
            m_arrMembers[197] = new CMember(198, m_arrNodes[130], m_arrNodes[132], m_arrCrSc[0], 0);
            m_arrMembers[198] = new CMember(199, m_arrNodes[131], m_arrNodes[133], m_arrCrSc[5], m_arrCrSc[4], 0, 0, 0);
            m_arrMembers[199] = new CMember(200, m_arrNodes[132], m_arrNodes[133], m_arrCrSc[2], 0);
            m_arrMembers[200] = new CMember(201, m_arrNodes[133], m_arrNodes[134], m_arrCrSc[4], m_arrCrSc[3], 0, 0, 0);
            m_arrMembers[201] = new CMember(202, m_arrNodes[132], m_arrNodes[134], m_arrCrSc[0], 0);
            m_arrMembers[202] = new CMember(203, m_arrNodes[00], m_arrNodes[068], m_arrCrSc[1], 0);
            m_arrMembers[203] = new CMember(204, m_arrNodes[02], m_arrNodes[070], m_arrCrSc[1], 0);
            m_arrMembers[204] = new CMember(205, m_arrNodes[04], m_arrNodes[072], m_arrCrSc[1], 0);
            m_arrMembers[205] = new CMember(206, m_arrNodes[07], m_arrNodes[075], m_arrCrSc[1], 0);
            m_arrMembers[206] = new CMember(207, m_arrNodes[09], m_arrNodes[077], m_arrCrSc[1], 0);
            m_arrMembers[207] = new CMember(208, m_arrNodes[11], m_arrNodes[079], m_arrCrSc[1], 0);
            m_arrMembers[208] = new CMember(209, m_arrNodes[13], m_arrNodes[081], m_arrCrSc[1], 0);
            m_arrMembers[209] = new CMember(210, m_arrNodes[15], m_arrNodes[083], m_arrCrSc[1], 0);
            m_arrMembers[210] = new CMember(211, m_arrNodes[17], m_arrNodes[085], m_arrCrSc[1], 0);
            m_arrMembers[211] = new CMember(212, m_arrNodes[19], m_arrNodes[087], m_arrCrSc[1], 0);
            m_arrMembers[212] = new CMember(213, m_arrNodes[21], m_arrNodes[089], m_arrCrSc[1], 0);
            m_arrMembers[213] = new CMember(214, m_arrNodes[22], m_arrNodes[090], m_arrCrSc[1], 0);
            m_arrMembers[214] = new CMember(215, m_arrNodes[24], m_arrNodes[092], m_arrCrSc[1], 0);
            m_arrMembers[215] = new CMember(216, m_arrNodes[26], m_arrNodes[094], m_arrCrSc[1], 0);
            m_arrMembers[216] = new CMember(217, m_arrNodes[28], m_arrNodes[096], m_arrCrSc[1], 0);
            m_arrMembers[217] = new CMember(218, m_arrNodes[30], m_arrNodes[098], m_arrCrSc[1], 0);
            m_arrMembers[218] = new CMember(219, m_arrNodes[32], m_arrNodes[100], m_arrCrSc[1], 0);
            m_arrMembers[219] = new CMember(220, m_arrNodes[34], m_arrNodes[101], m_arrCrSc[1], 0);
            m_arrMembers[220] = new CMember(221, m_arrNodes[35], m_arrNodes[102], m_arrCrSc[1], 0);
            m_arrMembers[221] = new CMember(222, m_arrNodes[37], m_arrNodes[104], m_arrCrSc[1], 0);
            m_arrMembers[222] = new CMember(223, m_arrNodes[39], m_arrNodes[106], m_arrCrSc[1], 0);
            m_arrMembers[223] = new CMember(224, m_arrNodes[41], m_arrNodes[108], m_arrCrSc[1], 0);
            m_arrMembers[224] = new CMember(225, m_arrNodes[43], m_arrNodes[110], m_arrCrSc[1], 0);
            m_arrMembers[225] = new CMember(226, m_arrNodes[45], m_arrNodes[112], m_arrCrSc[1], 0);
            m_arrMembers[226] = new CMember(227, m_arrNodes[46], m_arrNodes[113], m_arrCrSc[1], 0);
            m_arrMembers[227] = new CMember(228, m_arrNodes[48], m_arrNodes[115], m_arrCrSc[1], 0);
            m_arrMembers[228] = new CMember(229, m_arrNodes[50], m_arrNodes[117], m_arrCrSc[1], 0);
            m_arrMembers[229] = new CMember(230, m_arrNodes[52], m_arrNodes[119], m_arrCrSc[1], 0);
            m_arrMembers[230] = new CMember(231, m_arrNodes[54], m_arrNodes[121], m_arrCrSc[1], 0);
            m_arrMembers[231] = new CMember(232, m_arrNodes[56], m_arrNodes[123], m_arrCrSc[1], 0);
            m_arrMembers[232] = new CMember(233, m_arrNodes[58], m_arrNodes[125], m_arrCrSc[1], 0);
            m_arrMembers[233] = new CMember(234, m_arrNodes[60], m_arrNodes[127], m_arrCrSc[1], 0);
            m_arrMembers[234] = new CMember(235, m_arrNodes[63], m_arrNodes[130], m_arrCrSc[1], 0);
            m_arrMembers[235] = new CMember(236, m_arrNodes[65], m_arrNodes[132], m_arrCrSc[1], 0);
            m_arrMembers[236] = new CMember(237, m_arrNodes[67], m_arrNodes[134], m_arrCrSc[1], 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrMembers, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, false, false, false };
            bool[] bSupport2 = { false, false, true, false, false, false };

            // Create Support Objects
            m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[00], bSupport1, 0);
            m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[67], bSupport2, 0);

            // Setridit pole podle ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            m_arrMembers[02].CnRelease1 = new CNRelease(6, m_arrMembers[02].NodeStart, bMembRelase1, 0);
            m_arrMembers[05].CnRelease1 = new CNRelease(6, m_arrMembers[05].NodeStart, bMembRelase1, 0);
            m_arrMembers[09].CnRelease1 = new CNRelease(6, m_arrMembers[09].NodeStart, bMembRelase1, 0);
            m_arrMembers[11].CnRelease1 = new CNRelease(6, m_arrMembers[11].NodeStart, bMembRelase1, 0);
            m_arrMembers[14].CnRelease1 = new CNRelease(6, m_arrMembers[14].NodeStart, bMembRelase1, 0);
            m_arrMembers[17].CnRelease1 = new CNRelease(6, m_arrMembers[17].NodeStart, bMembRelase1, 0);
            m_arrMembers[20].CnRelease1 = new CNRelease(6, m_arrMembers[20].NodeStart, bMembRelase1, 0);
            m_arrMembers[23].CnRelease1 = new CNRelease(6, m_arrMembers[23].NodeStart, bMembRelase1, 0);
            m_arrMembers[26].CnRelease1 = new CNRelease(6, m_arrMembers[26].NodeStart, bMembRelase1, 0);
            m_arrMembers[29].CnRelease1 = new CNRelease(6, m_arrMembers[29].NodeStart, bMembRelase1, 0);
            m_arrMembers[31].CnRelease1 = new CNRelease(6, m_arrMembers[31].NodeStart, bMembRelase1, 0);
            m_arrMembers[35].CnRelease1 = new CNRelease(6, m_arrMembers[35].NodeStart, bMembRelase1, 0);
            m_arrMembers[38].CnRelease1 = new CNRelease(6, m_arrMembers[38].NodeStart, bMembRelase1, 0);
            m_arrMembers[41].CnRelease1 = new CNRelease(6, m_arrMembers[41].NodeStart, bMembRelase1, 0);
            m_arrMembers[44].CnRelease1 = new CNRelease(6, m_arrMembers[44].NodeStart, bMembRelase1, 0);
            m_arrMembers[47].CnRelease1 = new CNRelease(6, m_arrMembers[47].NodeStart, bMembRelase1, 0);
            m_arrMembers[50].CnRelease1 = new CNRelease(6, m_arrMembers[50].NodeStart, bMembRelase1, 0);
            m_arrMembers[53].CnRelease1 = new CNRelease(6, m_arrMembers[53].NodeStart, bMembRelase1, 0);
            m_arrMembers[56].CnRelease1 = new CNRelease(6, m_arrMembers[56].NodeStart, bMembRelase1, 0);
            m_arrMembers[59].CnRelease1 = new CNRelease(6, m_arrMembers[59].NodeStart, bMembRelase1, 0);
            m_arrMembers[62].CnRelease1 = new CNRelease(6, m_arrMembers[62].NodeStart, bMembRelase1, 0);
            m_arrMembers[65].CnRelease1 = new CNRelease(6, m_arrMembers[65].NodeStart, bMembRelase1, 0);
            m_arrMembers[69].CnRelease1 = new CNRelease(6, m_arrMembers[69].NodeStart, bMembRelase1, 0);
            m_arrMembers[71].CnRelease1 = new CNRelease(6, m_arrMembers[71].NodeStart, bMembRelase1, 0);
            m_arrMembers[74].CnRelease1 = new CNRelease(6, m_arrMembers[74].NodeStart, bMembRelase1, 0);
            m_arrMembers[77].CnRelease1 = new CNRelease(6, m_arrMembers[77].NodeStart, bMembRelase1, 0);
            m_arrMembers[80].CnRelease1 = new CNRelease(6, m_arrMembers[80].NodeStart, bMembRelase1, 0);
            m_arrMembers[83].CnRelease1 = new CNRelease(6, m_arrMembers[83].NodeStart, bMembRelase1, 0);
            m_arrMembers[86].CnRelease1 = new CNRelease(6, m_arrMembers[86].NodeStart, bMembRelase1, 0);
            m_arrMembers[89].CnRelease1 = new CNRelease(6, m_arrMembers[89].NodeStart, bMembRelase1, 0);
            m_arrMembers[91].CnRelease1 = new CNRelease(6, m_arrMembers[91].NodeStart, bMembRelase1, 0);
            m_arrMembers[95].CnRelease1 = new CNRelease(6, m_arrMembers[95].NodeStart, bMembRelase1, 0);
            m_arrMembers[98].CnRelease1 = new CNRelease(6, m_arrMembers[98].NodeStart, bMembRelase1, 0);

            // Nodal Forces - fill values
            //arrForces[00] = new CNForce(m_arrNodes[00], -00.0f, 0.0f, -020.0f, m_arrCrSc[0], 0);
            //arrForces[01] = new CNForce(m_arrNodes[02], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[02] = new CNForce(m_arrNodes[04], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[03] = new CNForce(m_arrNodes[07], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[04] = new CNForce(m_arrNodes[09], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[05] = new CNForce(m_arrNodes[11], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[06] = new CNForce(m_arrNodes[13], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[07] = new CNForce(m_arrNodes[15], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[08] = new CNForce(m_arrNodes[17], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[09] = new CNForce(m_arrNodes[19], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[10] = new CNForce(m_arrNodes[21], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[11] = new CNForce(m_arrNodes[22], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[12] = new CNForce(m_arrNodes[24], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[13] = new CNForce(m_arrNodes[26], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[14] = new CNForce(m_arrNodes[28], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[15] = new CNForce(m_arrNodes[30], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[16] = new CNForce(m_arrNodes[32], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[17] = new CNForce(m_arrNodes[34], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[18] = new CNForce(m_arrNodes[35], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[19] = new CNForce(m_arrNodes[37], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[20] = new CNForce(m_arrNodes[39], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[21] = new CNForce(m_arrNodes[41], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[22] = new CNForce(m_arrNodes[43], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[23] = new CNForce(m_arrNodes[45], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[24] = new CNForce(m_arrNodes[46], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[25] = new CNForce(m_arrNodes[48], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[26] = new CNForce(m_arrNodes[50], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[27] = new CNForce(m_arrNodes[52], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[28] = new CNForce(m_arrNodes[54], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[29] = new CNForce(m_arrNodes[56], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[30] = new CNForce(m_arrNodes[58], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[31] = new CNForce(m_arrNodes[60], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[32] = new CNForce(m_arrNodes[63], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[33] = new CNForce(m_arrNodes[65], -00.0f, 0.0f, -050.0f, m_arrCrSc[0], 0);
            //arrForces[34] = new CNForce(m_arrNodes[67], -00.0f, 0.0f, -020.0f, m_arrCrSc[0], 0);


        }


    }
}
