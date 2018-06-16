using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;
using BaseClasses.GraphObj;
using MATERIAL;
using CRSC;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows;

namespace sw_en_GUI.EXAMPLES._3D
{
    class CExample_3D_80 : CExample
    {
        public CExample_3D_80()
        {
            m_eSLN = ESLN.e4DD_3D; // 3D objects in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrGOPoints = new BaseClasses.GraphObj.CPoint[450];
            //m_arrGOLines = new BaseClasses.GraphObj.CLine[21];
            m_arrGOAreas = new BaseClasses.GraphObj.CArea[0];
            m_arrGOVolumes = new BaseClasses.GraphObj.CVolume[434];
            m_arrGOStrWindows = new BaseClasses.GraphObj.CStructure_Window[16];

            m_arrMat = new CMat_00[1];
            //m_arrCrSc = new CRSC.CCrSc[1];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            //m_arrCrSc[0] = new CRSC.CCrSc_0_05(0.1f, 0.05f);

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Ground
            // Level 0 [-1.000]

            m_arrGOPoints[00] = new CPoint(01, -50, -50, -1, 0);

            // Level 1 [+0.000]

            m_arrGOPoints[01] = new CPoint(02, 0, 0, 0, 0);
            m_arrGOPoints[02] = new CPoint(03, 1, 0, 0, 0);
            m_arrGOPoints[03] = new CPoint(04, 2, 0, 0, 0);
            m_arrGOPoints[04] = new CPoint(05, 3, 0, 0, 0);
            m_arrGOPoints[05] = new CPoint(06, 4, 0, 0, 0);
            m_arrGOPoints[06] = new CPoint(07, 5, 0, 0, 0);
            m_arrGOPoints[07] = new CPoint(08, 6, 0, 0, 0);
            m_arrGOPoints[08] = new CPoint(09, 7, 0, 0, 0);
            m_arrGOPoints[09] = new CPoint(10, 8, 0, 0, 0);
            m_arrGOPoints[10] = new CPoint(11, 9, 0, 0, 0);
            m_arrGOPoints[11] = new CPoint(12, 10, 0, 0, 0);
            m_arrGOPoints[12] = new CPoint(13, 11, 0, 0, 0);
            m_arrGOPoints[13] = new CPoint(14, 11.5f, 0.5f, 0, 0);
            m_arrGOPoints[14] = new CPoint(15, 11.5f, 1, 0, 0);
            m_arrGOPoints[15] = new CPoint(16, 11.5f, 2, 0, 0);
            m_arrGOPoints[16] = new CPoint(17, 11.5f, 3, 0, 0);
            m_arrGOPoints[17] = new CPoint(18, 11.5f, 4, 0, 0);
            m_arrGOPoints[18] = new CPoint(19, 11.5f, 5, 0, 0);
            m_arrGOPoints[19] = new CPoint(20, 11.5f, 6, 0, 0);
            m_arrGOPoints[20] = new CPoint(21, 11.5f, 7, 0, 0);
            m_arrGOPoints[21] = new CPoint(22, 11.5f, 8, 0, 0);
            m_arrGOPoints[22] = new CPoint(23, 11.5f, 9, 0, 0);
            m_arrGOPoints[23] = new CPoint(24, 11.5f, 10, 0, 0);
            m_arrGOPoints[24] = new CPoint(25, 11.5f, 11, 0, 0);
            m_arrGOPoints[25] = new CPoint(26, 11, 11.5f, 0, 0);
            m_arrGOPoints[26] = new CPoint(27, 10, 11.5f, 0, 0);
            m_arrGOPoints[27] = new CPoint(28, 09, 11.5f, 0, 0);
            m_arrGOPoints[28] = new CPoint(29, 08, 11.5f, 0, 0);
            m_arrGOPoints[29] = new CPoint(30, 07, 11.5f, 0, 0);
            m_arrGOPoints[30] = new CPoint(31, 06, 11.5f, 0, 0);
            m_arrGOPoints[31] = new CPoint(32, 05, 11.5f, 0, 0);
            m_arrGOPoints[32] = new CPoint(33, 04, 11.5f, 0, 0);
            m_arrGOPoints[33] = new CPoint(34, 03, 11.5f, 0, 0);
            m_arrGOPoints[34] = new CPoint(35, 02, 11.5f, 0, 0);
            m_arrGOPoints[35] = new CPoint(36, 01, 11.5f, 0, 0);
            m_arrGOPoints[36] = new CPoint(37, 00, 11.5f, 0, 0);
            m_arrGOPoints[37] = new CPoint(38, 0, 11, 0, 0);
            m_arrGOPoints[38] = new CPoint(39, 0, 10, 0, 0);
            m_arrGOPoints[39] = new CPoint(40, 0, 09, 0, 0);
            m_arrGOPoints[40] = new CPoint(41, 0, 08, 0, 0);
            m_arrGOPoints[41] = new CPoint(42, 0, 07, 0, 0);
            m_arrGOPoints[42] = new CPoint(43, 0, 06, 0, 0);
            m_arrGOPoints[43] = new CPoint(44, 0, 05, 0, 0);
            m_arrGOPoints[44] = new CPoint(45, 0, 04, 0, 0);
            m_arrGOPoints[45] = new CPoint(46, 0, 03, 0, 0);
            m_arrGOPoints[46] = new CPoint(47, 0, 02, 0, 0);
            m_arrGOPoints[47] = new CPoint(48, 0, 01, 0, 0);
            m_arrGOPoints[48] = new CPoint(49, 0, 0.5f, 0, 0);

            // Level 2 [+1.000]

            m_arrGOPoints[49] = new CPoint(50, 0, 0, 1, 0);
            m_arrGOPoints[50] = new CPoint(51, 1, 0, 1, 0);
            m_arrGOPoints[51] = new CPoint(52, 2, 0, 1, 0);
            m_arrGOPoints[52] = new CPoint(53, 3, 0, 1, 0);
            m_arrGOPoints[53] = new CPoint(54, 4, 0, 1, 0);
            m_arrGOPoints[54] = new CPoint(55, 5, 0, 1, 0);
            m_arrGOPoints[55] = new CPoint(56, 6, 0, 1, 0);
            m_arrGOPoints[56] = new CPoint(57, 7, 0, 1, 0);
            m_arrGOPoints[57] = new CPoint(58, 8, 0, 1, 0);
            m_arrGOPoints[58] = new CPoint(59, 9, 0, 1, 0);
            m_arrGOPoints[59] = new CPoint(60, 10, 0, 1, 0);
            m_arrGOPoints[60] = new CPoint(61, 11, 0, 1, 0);
            m_arrGOPoints[61] = new CPoint(62, 11.5f, 0.5f, 1, 0);
            m_arrGOPoints[62] = new CPoint(63, 11.5f, 1, 1, 0);
            m_arrGOPoints[63] = new CPoint(64, 11.5f, 2, 1, 0);
            m_arrGOPoints[64] = new CPoint(65, 11.5f, 3, 1, 0);
            m_arrGOPoints[65] = new CPoint(66, 11.5f, 4, 1, 0);
            m_arrGOPoints[66] = new CPoint(67, 11.5f, 5, 1, 0);
            m_arrGOPoints[67] = new CPoint(68, 11.5f, 6, 1, 0);
            m_arrGOPoints[68] = new CPoint(69, 11.5f, 7, 1, 0);
            m_arrGOPoints[69] = new CPoint(70, 11.5f, 8, 1, 0);
            m_arrGOPoints[70] = new CPoint(71, 11.5f, 9, 1, 0);
            m_arrGOPoints[71] = new CPoint(72, 11.5f, 10, 1, 0);
            m_arrGOPoints[72] = new CPoint(73, 11.5f, 11, 1, 0);
            m_arrGOPoints[73] = new CPoint(74, 11, 11.5f, 1, 0);
            m_arrGOPoints[74] = new CPoint(75, 10, 11.5f, 1, 0);
            m_arrGOPoints[75] = new CPoint(76, 09, 11.5f, 1, 0);
            m_arrGOPoints[76] = new CPoint(77, 08, 11.5f, 1, 0);
            m_arrGOPoints[77] = new CPoint(78, 07, 11.5f, 1, 0);
            m_arrGOPoints[78] = new CPoint(79, 06, 11.5f, 1, 0);
            m_arrGOPoints[79] = new CPoint(80, 05, 11.5f, 1, 0);
            m_arrGOPoints[80] = new CPoint(81, 04, 11.5f, 1, 0);
            m_arrGOPoints[81] = new CPoint(82, 03, 11.5f, 1, 0);
            m_arrGOPoints[82] = new CPoint(83, 02, 11.5f, 1, 0);
            m_arrGOPoints[83] = new CPoint(84, 01, 11.5f, 1, 0);
            m_arrGOPoints[84] = new CPoint(85, 00, 11.5f, 1, 0);
            m_arrGOPoints[85] = new CPoint(86, 0, 11, 1, 0);
            m_arrGOPoints[86] = new CPoint(87, 0, 10, 1, 0);
            m_arrGOPoints[87] = new CPoint(88, 0, 09, 1, 0);
            m_arrGOPoints[88] = new CPoint(89, 0, 08, 1, 0);
            m_arrGOPoints[89] = new CPoint(90, 0, 07, 1, 0);
            m_arrGOPoints[90] = new CPoint(91, 0, 06, 1, 0);
            m_arrGOPoints[91] = new CPoint(92, 0, 05, 1, 0);
            m_arrGOPoints[92] = new CPoint(93, 0, 04, 1, 0);
            m_arrGOPoints[93] = new CPoint(94, 0, 03, 1, 0);
            m_arrGOPoints[94] = new CPoint(95, 0, 02, 1, 0);
            m_arrGOPoints[95] = new CPoint(96, 0, 01, 1, 0);
            m_arrGOPoints[96] = new CPoint(97, 0, 0.5f, 1, 0);

            // Level 3 [+2.500]

            m_arrGOPoints[97] = new CPoint(98, 0, 0, 2.5f, 0);
            m_arrGOPoints[98] = new CPoint(99, 1, 0, 2.5f, 0);
            m_arrGOPoints[99] = new CPoint(100, 2, 0, 2.5f, 0);
            m_arrGOPoints[100] = new CPoint(101, 3, 0, 2.5f, 0);
            m_arrGOPoints[101] = new CPoint(102, 4, 0, 2.5f, 0);
            m_arrGOPoints[102] = new CPoint(103, 5, 0, 2.5f, 0);
            m_arrGOPoints[103] = new CPoint(104, 6, 0, 2.5f, 0);
            m_arrGOPoints[104] = new CPoint(105, 7, 0, 2.5f, 0);
            m_arrGOPoints[105] = new CPoint(106, 8, 0, 2.5f, 0);
            m_arrGOPoints[106] = new CPoint(107, 9, 0, 2.5f, 0);
            m_arrGOPoints[107] = new CPoint(108, 10, 0, 2.5f, 0);
            m_arrGOPoints[108] = new CPoint(109, 11, 0, 2.5f, 0);
            m_arrGOPoints[109] = new CPoint(110, 11.5f, 0.5f, 2.5f, 0);
            m_arrGOPoints[110] = new CPoint(111, 11.5f, 1, 2.5f, 0);
            m_arrGOPoints[111] = new CPoint(112, 11.5f, 2, 2.5f, 0);
            m_arrGOPoints[112] = new CPoint(113, 11.5f, 3, 2.5f, 0);
            m_arrGOPoints[113] = new CPoint(114, 11.5f, 4, 2.5f, 0);
            m_arrGOPoints[114] = new CPoint(115, 11.5f, 5, 2.5f, 0);
            m_arrGOPoints[115] = new CPoint(116, 11.5f, 6, 2.5f, 0);
            m_arrGOPoints[116] = new CPoint(117, 11.5f, 7, 2.5f, 0);
            m_arrGOPoints[117] = new CPoint(118, 11.5f, 8, 2.5f, 0);
            m_arrGOPoints[118] = new CPoint(119, 11.5f, 9, 2.5f, 0);
            m_arrGOPoints[119] = new CPoint(120, 11.5f, 10, 2.5f, 0);
            m_arrGOPoints[120] = new CPoint(121, 11.5f, 11, 2.5f, 0);
            m_arrGOPoints[121] = new CPoint(122, 11, 11.5f, 2.5f, 0);
            m_arrGOPoints[122] = new CPoint(123, 10, 11.5f, 2.5f, 0);
            m_arrGOPoints[123] = new CPoint(124, 09, 11.5f, 2.5f, 0);
            m_arrGOPoints[124] = new CPoint(125, 08, 11.5f, 2.5f, 0);
            m_arrGOPoints[125] = new CPoint(126, 07, 11.5f, 2.5f, 0);
            m_arrGOPoints[126] = new CPoint(127, 06, 11.5f, 2.5f, 0);
            m_arrGOPoints[127] = new CPoint(128, 05, 11.5f, 2.5f, 0);
            m_arrGOPoints[128] = new CPoint(129, 04, 11.5f, 2.5f, 0);
            m_arrGOPoints[129] = new CPoint(130, 03, 11.5f, 2.5f, 0);
            m_arrGOPoints[130] = new CPoint(131, 02, 11.5f, 2.5f, 0);
            m_arrGOPoints[131] = new CPoint(132, 01, 11.5f, 2.5f, 0);
            m_arrGOPoints[132] = new CPoint(133, 00, 11.5f, 2.5f, 0);
            m_arrGOPoints[133] = new CPoint(134, 0, 11, 2.5f, 0);
            m_arrGOPoints[134] = new CPoint(135, 0, 10, 2.5f, 0);
            m_arrGOPoints[135] = new CPoint(136, 0, 09, 2.5f, 0);
            m_arrGOPoints[136] = new CPoint(137, 0, 08, 2.5f, 0);
            m_arrGOPoints[137] = new CPoint(138, 0, 07, 2.5f, 0);
            m_arrGOPoints[138] = new CPoint(139, 0, 06, 2.5f, 0);
            m_arrGOPoints[139] = new CPoint(140, 0, 05, 2.5f, 0);
            m_arrGOPoints[140] = new CPoint(141, 0, 04, 2.5f, 0);
            m_arrGOPoints[141] = new CPoint(142, 0, 03, 2.5f, 0);
            m_arrGOPoints[142] = new CPoint(143, 0, 02, 2.5f, 0);
            m_arrGOPoints[143] = new CPoint(144, 0, 01, 2.5f, 0);
            m_arrGOPoints[144] = new CPoint(145, 0, 0.5f, 2.5f, 0);

            // Level 4 [+3.000]

            m_arrGOPoints[145] = new CPoint(146, 0, 0, 3, 0);
            m_arrGOPoints[146] = new CPoint(147, 1, 0, 3, 0);
            m_arrGOPoints[147] = new CPoint(148, 2, 0, 3, 0);
            m_arrGOPoints[148] = new CPoint(149, 3, 0, 3, 0);
            m_arrGOPoints[149] = new CPoint(150, 4, 0, 3, 0);
            m_arrGOPoints[150] = new CPoint(151, 5, 0, 3, 0);
            m_arrGOPoints[151] = new CPoint(152, 6, 0, 3, 0);
            m_arrGOPoints[152] = new CPoint(153, 7, 0, 3, 0);
            m_arrGOPoints[153] = new CPoint(154, 8, 0, 3, 0);
            m_arrGOPoints[154] = new CPoint(155, 9, 0, 3, 0);
            m_arrGOPoints[155] = new CPoint(156, 10, 0, 3, 0);
            m_arrGOPoints[156] = new CPoint(157, 11, 0, 3, 0);
            m_arrGOPoints[157] = new CPoint(158, 11.5f, 0.5f, 3, 0);
            m_arrGOPoints[158] = new CPoint(159, 11.5f, 1, 3, 0);
            m_arrGOPoints[159] = new CPoint(160, 11.5f, 2, 3, 0);
            m_arrGOPoints[160] = new CPoint(161, 11.5f, 3, 3, 0);
            m_arrGOPoints[161] = new CPoint(162, 11.5f, 4, 3, 0);

            m_arrGOPoints[162] = new CPoint(163, 04, 11.5f, 3, 0);
            m_arrGOPoints[163] = new CPoint(164, 03, 11.5f, 3, 0);
            m_arrGOPoints[164] = new CPoint(165, 02, 11.5f, 3, 0);
            m_arrGOPoints[165] = new CPoint(166, 01, 11.5f, 3, 0);
            m_arrGOPoints[166] = new CPoint(167, 00, 11.5f, 3, 0);
            m_arrGOPoints[167] = new CPoint(168, 0, 11, 3, 0);
            m_arrGOPoints[168] = new CPoint(169, 0, 10, 3, 0);
            m_arrGOPoints[169] = new CPoint(170, 0, 09, 3, 0);
            m_arrGOPoints[170] = new CPoint(171, 0, 08, 3, 0);
            m_arrGOPoints[171] = new CPoint(172, 0, 07, 3, 0);
            m_arrGOPoints[172] = new CPoint(173, 0, 06, 3, 0);
            m_arrGOPoints[173] = new CPoint(174, 0, 05, 3, 0);
            m_arrGOPoints[174] = new CPoint(175, 0, 04, 3, 0);
            m_arrGOPoints[175] = new CPoint(176, 0, 03, 3, 0);
            m_arrGOPoints[176] = new CPoint(177, 0, 02, 3, 0);
            m_arrGOPoints[177] = new CPoint(178, 0, 01, 3, 0);
            m_arrGOPoints[178] = new CPoint(179, 0, 0.5f, 3, 0);

            // Level 4 [+3.000]

            // Move
            float fx = 5f;
            float fy = 5f;

            m_arrGOPoints[179] = new CPoint(180, 0 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[180] = new CPoint(181, 1 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[181] = new CPoint(182, 2 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[182] = new CPoint(183, 3 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[183] = new CPoint(184, 4 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[184] = new CPoint(185, 5 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[185] = new CPoint(186, 6 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[186] = new CPoint(187, 7 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[187] = new CPoint(188, 8 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[188] = new CPoint(189, 9 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[189] = new CPoint(190, 10 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[190] = new CPoint(191, 11 + fx, 0 + fy, 3, 0);
            m_arrGOPoints[191] = new CPoint(192, 11.5f + fx, 0.5f + fy, 3, 0);
            m_arrGOPoints[192] = new CPoint(193, 11.5f + fx, 1 + fy, 3, 0);
            m_arrGOPoints[193] = new CPoint(194, 11.5f + fx, 2 + fy, 3, 0);
            m_arrGOPoints[194] = new CPoint(195, 11.5f + fx, 3 + fy, 3, 0);
            m_arrGOPoints[195] = new CPoint(196, 11.5f + fx, 4 + fy, 3, 0);
            m_arrGOPoints[196] = new CPoint(197, 11.5f + fx, 5 + fy, 3, 0);
            m_arrGOPoints[197] = new CPoint(198, 11.5f + fx, 6 + fy, 3, 0);
            m_arrGOPoints[198] = new CPoint(199, 11.5f + fx, 7 + fy, 3, 0);
            m_arrGOPoints[199] = new CPoint(200, 11.5f + fx, 8 + fy, 3, 0);
            m_arrGOPoints[200] = new CPoint(201, 11.5f + fx, 9 + fy, 3, 0);
            m_arrGOPoints[201] = new CPoint(202, 11.5f + fx, 10 + fy, 3, 0);
            m_arrGOPoints[202] = new CPoint(203, 11.5f + fx, 11 + fy, 3, 0);
            m_arrGOPoints[203] = new CPoint(204, 11 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[204] = new CPoint(205, 10 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[205] = new CPoint(206, 09 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[206] = new CPoint(207, 08 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[207] = new CPoint(208, 07 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[208] = new CPoint(209, 06 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[209] = new CPoint(210, 05 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[210] = new CPoint(211, 04 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[211] = new CPoint(212, 03 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[212] = new CPoint(213, 02 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[213] = new CPoint(214, 01 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[214] = new CPoint(215, 00 + fx, 11.5f + fy, 3, 0);
            m_arrGOPoints[215] = new CPoint(216, 0 + fx, 11 + fy, 3, 0);
            m_arrGOPoints[216] = new CPoint(217, 0 + fx, 10 + fy, 3, 0);
            m_arrGOPoints[217] = new CPoint(218, 0 + fx, 09 + fy, 3, 0);
            m_arrGOPoints[218] = new CPoint(219, 0 + fx, 08 + fy, 3, 0);
            m_arrGOPoints[219] = new CPoint(220, 0 + fx, 07 + fy, 3, 0);
            m_arrGOPoints[220] = new CPoint(221, 0 + fx, 06 + fy, 3, 0);
            m_arrGOPoints[221] = new CPoint(222, 0 + fx, 05 + fy, 3, 0);
            m_arrGOPoints[222] = new CPoint(223, 0 + fx, 04 + fy, 3, 0);
            m_arrGOPoints[223] = new CPoint(224, 0 + fx, 03 + fy, 3, 0);
            m_arrGOPoints[224] = new CPoint(225, 0 + fx, 02 + fy, 3, 0);
            m_arrGOPoints[225] = new CPoint(226, 0 + fx, 01 + fy, 3, 0);
            m_arrGOPoints[226] = new CPoint(227, 0 + fx, 0.5f + fy, 3, 0);

            // Level 5 [+3.500]

            m_arrGOPoints[227] = new CPoint(228, 0 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[228] = new CPoint(229, 1 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[229] = new CPoint(230, 2 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[230] = new CPoint(231, 3 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[231] = new CPoint(232, 4 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[232] = new CPoint(233, 5 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[233] = new CPoint(234, 6 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[234] = new CPoint(235, 7 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[235] = new CPoint(236, 8 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[236] = new CPoint(237, 9 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[237] = new CPoint(238, 10 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[238] = new CPoint(239, 11 + fx, 0 + fy, 3.5f, 0);
            m_arrGOPoints[239] = new CPoint(240, 11.5f + fx, 0.5f + fy, 3.5f, 0);
            m_arrGOPoints[240] = new CPoint(241, 11.5f + fx, 1 + fy, 3.5f, 0);
            m_arrGOPoints[241] = new CPoint(242, 11.5f + fx, 2 + fy, 3.5f, 0);
            m_arrGOPoints[242] = new CPoint(243, 11.5f + fx, 3 + fy, 3.5f, 0);
            m_arrGOPoints[243] = new CPoint(244, 11.5f + fx, 4 + fy, 3.5f, 0);
            m_arrGOPoints[244] = new CPoint(245, 11.5f + fx, 5 + fy, 3.5f, 0);
            m_arrGOPoints[245] = new CPoint(246, 11.5f + fx, 6 + fy, 3.5f, 0);
            m_arrGOPoints[246] = new CPoint(247, 11.5f + fx, 7 + fy, 3.5f, 0);
            m_arrGOPoints[247] = new CPoint(248, 11.5f + fx, 8 + fy, 3.5f, 0);
            m_arrGOPoints[248] = new CPoint(249, 11.5f + fx, 9 + fy, 3.5f, 0);
            m_arrGOPoints[249] = new CPoint(250, 11.5f + fx, 10 + fy, 3.5f, 0);
            m_arrGOPoints[250] = new CPoint(251, 11.5f + fx, 11 + fy, 3.5f, 0);
            m_arrGOPoints[251] = new CPoint(252, 11 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[252] = new CPoint(253, 10 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[253] = new CPoint(254, 09 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[254] = new CPoint(255, 08 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[255] = new CPoint(256, 07 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[256] = new CPoint(257, 06 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[257] = new CPoint(258, 05 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[258] = new CPoint(259, 04 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[259] = new CPoint(260, 03 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[260] = new CPoint(261, 02 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[261] = new CPoint(262, 01 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[262] = new CPoint(263, 00 + fx, 11.5f + fy, 3.5f, 0);
            m_arrGOPoints[263] = new CPoint(264, 0 + fx, 11 + fy, 3.5f, 0);
            m_arrGOPoints[264] = new CPoint(265, 0 + fx, 10 + fy, 3.5f, 0);
            m_arrGOPoints[265] = new CPoint(266, 0 + fx, 09 + fy, 3.5f, 0);
            m_arrGOPoints[266] = new CPoint(267, 0 + fx, 08 + fy, 3.5f, 0);
            m_arrGOPoints[267] = new CPoint(268, 0 + fx, 07 + fy, 3.5f, 0);
            m_arrGOPoints[268] = new CPoint(269, 0 + fx, 06 + fy, 3.5f, 0);
            m_arrGOPoints[269] = new CPoint(270, 0 + fx, 05 + fy, 3.5f, 0);
            m_arrGOPoints[270] = new CPoint(271, 0 + fx, 04 + fy, 3.5f, 0);
            m_arrGOPoints[271] = new CPoint(272, 0 + fx, 03 + fy, 3.5f, 0);
            m_arrGOPoints[272] = new CPoint(273, 0 + fx, 02 + fy, 3.5f, 0);
            m_arrGOPoints[273] = new CPoint(274, 0 + fx, 01 + fy, 3.5f, 0);
            m_arrGOPoints[274] = new CPoint(275, 0 + fx, 0.5f + fy, 3.5f, 0);

            // Level 6 [+4.000]

            m_arrGOPoints[275] = new CPoint(276, 0 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[276] = new CPoint(277, 1 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[277] = new CPoint(278, 2 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[278] = new CPoint(279, 3 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[279] = new CPoint(280, 4 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[280] = new CPoint(281, 5 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[281] = new CPoint(282, 6 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[282] = new CPoint(283, 7 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[283] = new CPoint(284, 8 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[284] = new CPoint(285, 9 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[285] = new CPoint(286, 10 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[286] = new CPoint(287, 11 + fx, 0 + fy, 4, 0);
            m_arrGOPoints[287] = new CPoint(288, 11.5f + fx, 0.5f + fy, 4, 0);
            m_arrGOPoints[288] = new CPoint(289, 11.5f + fx, 1 + fy, 4, 0);
            m_arrGOPoints[289] = new CPoint(290, 11.5f + fx, 2 + fy, 4, 0);
            m_arrGOPoints[290] = new CPoint(291, 11.5f + fx, 3 + fy, 4, 0);
            m_arrGOPoints[291] = new CPoint(292, 11.5f + fx, 4 + fy, 4, 0);
            m_arrGOPoints[292] = new CPoint(293, 11.5f + fx, 5 + fy, 4, 0);
            m_arrGOPoints[293] = new CPoint(294, 11.5f + fx, 6 + fy, 4, 0);
            m_arrGOPoints[294] = new CPoint(295, 11.5f + fx, 7 + fy, 4, 0);
            m_arrGOPoints[295] = new CPoint(296, 11.5f + fx, 8 + fy, 4, 0);
            m_arrGOPoints[296] = new CPoint(297, 11.5f + fx, 9 + fy, 4, 0);
            m_arrGOPoints[297] = new CPoint(298, 11.5f + fx, 10 + fy, 4, 0);
            m_arrGOPoints[298] = new CPoint(299, 11.5f + fx, 11 + fy, 4, 0);
            m_arrGOPoints[299] = new CPoint(300, 11 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[300] = new CPoint(301, 10 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[301] = new CPoint(302, 09 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[302] = new CPoint(303, 08 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[303] = new CPoint(304, 07 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[304] = new CPoint(305, 06 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[305] = new CPoint(306, 05 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[306] = new CPoint(307, 04 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[307] = new CPoint(308, 03 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[308] = new CPoint(309, 02 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[309] = new CPoint(310, 01 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[310] = new CPoint(311, 00 + fx, 11.5f + fy, 4, 0);
            m_arrGOPoints[311] = new CPoint(312, 0 + fx, 11 + fy, 4, 0);
            m_arrGOPoints[312] = new CPoint(313, 0 + fx, 10 + fy, 4, 0);
            m_arrGOPoints[313] = new CPoint(314, 0 + fx, 09 + fy, 4, 0);
            m_arrGOPoints[314] = new CPoint(315, 0 + fx, 08 + fy, 4, 0);
            m_arrGOPoints[315] = new CPoint(316, 0 + fx, 07 + fy, 4, 0);
            m_arrGOPoints[316] = new CPoint(317, 0 + fx, 06 + fy, 4, 0);
            m_arrGOPoints[317] = new CPoint(318, 0 + fx, 05 + fy, 4, 0);
            m_arrGOPoints[318] = new CPoint(319, 0 + fx, 04 + fy, 4, 0);
            m_arrGOPoints[319] = new CPoint(320, 0 + fx, 03 + fy, 4, 0);
            m_arrGOPoints[320] = new CPoint(321, 0 + fx, 02 + fy, 4, 0);
            m_arrGOPoints[321] = new CPoint(322, 0 + fx, 01 + fy, 4, 0);
            m_arrGOPoints[322] = new CPoint(323, 0 + fx, 0.5f + fy, 4, 0);

            // Level 7 [+5.500]

            m_arrGOPoints[323] = new CPoint(324, 0 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[324] = new CPoint(325, 1 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[325] = new CPoint(326, 2 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[326] = new CPoint(327, 3 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[327] = new CPoint(328, 4 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[328] = new CPoint(329, 5 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[329] = new CPoint(330, 6 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[330] = new CPoint(331, 7 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[331] = new CPoint(332, 8 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[332] = new CPoint(333, 9 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[333] = new CPoint(334, 10 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[334] = new CPoint(335, 11 + fx, 0 + fy, 5.5f, 0);
            m_arrGOPoints[335] = new CPoint(336, 11.5f + fx, 0.5f + fy, 5.5f, 0);
            m_arrGOPoints[336] = new CPoint(337, 11.5f + fx, 1 + fy, 5.5f, 0);
            m_arrGOPoints[337] = new CPoint(338, 11.5f + fx, 2 + fy, 5.5f, 0);
            m_arrGOPoints[338] = new CPoint(339, 11.5f + fx, 3 + fy, 5.5f, 0);
            m_arrGOPoints[339] = new CPoint(340, 11.5f + fx, 4 + fy, 5.5f, 0);
            m_arrGOPoints[340] = new CPoint(341, 11.5f + fx, 5 + fy, 5.5f, 0);
            m_arrGOPoints[341] = new CPoint(342, 11.5f + fx, 6 + fy, 5.5f, 0);
            m_arrGOPoints[342] = new CPoint(343, 11.5f + fx, 7 + fy, 5.5f, 0);
            m_arrGOPoints[343] = new CPoint(344, 11.5f + fx, 8 + fy, 5.5f, 0);
            m_arrGOPoints[344] = new CPoint(345, 11.5f + fx, 9 + fy, 5.5f, 0);
            m_arrGOPoints[345] = new CPoint(346, 11.5f + fx, 10 + fy, 5.5f, 0);
            m_arrGOPoints[346] = new CPoint(347, 11.5f + fx, 11 + fy, 5.5f, 0);
            m_arrGOPoints[347] = new CPoint(348, 11 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[348] = new CPoint(349, 10 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[349] = new CPoint(350, 09 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[350] = new CPoint(351, 08 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[351] = new CPoint(352, 07 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[352] = new CPoint(353, 06 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[353] = new CPoint(354, 05 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[354] = new CPoint(355, 04 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[355] = new CPoint(356, 03 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[356] = new CPoint(357, 02 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[357] = new CPoint(358, 01 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[358] = new CPoint(359, 00 + fx, 11.5f + fy, 5.5f, 0);
            m_arrGOPoints[359] = new CPoint(360, 0 + fx, 11 + fy, 5.5f, 0);
            m_arrGOPoints[360] = new CPoint(361, 0 + fx, 10 + fy, 5.5f, 0);
            m_arrGOPoints[361] = new CPoint(362, 0 + fx, 09 + fy, 5.5f, 0);
            m_arrGOPoints[362] = new CPoint(363, 0 + fx, 08 + fy, 5.5f, 0);
            m_arrGOPoints[363] = new CPoint(364, 0 + fx, 07 + fy, 5.5f, 0);
            m_arrGOPoints[364] = new CPoint(365, 0 + fx, 06 + fy, 5.5f, 0);
            m_arrGOPoints[365] = new CPoint(366, 0 + fx, 05 + fy, 5.5f, 0);
            m_arrGOPoints[366] = new CPoint(367, 0 + fx, 04 + fy, 5.5f, 0);
            m_arrGOPoints[367] = new CPoint(368, 0 + fx, 03 + fy, 5.5f, 0);
            m_arrGOPoints[368] = new CPoint(369, 0 + fx, 02 + fy, 5.5f, 0);
            m_arrGOPoints[369] = new CPoint(370, 0 + fx, 01 + fy, 5.5f, 0);
            m_arrGOPoints[370] = new CPoint(371, 0 + fx, 0.5f + fy, 5.5f, 0);

            // Level 8 [+6.000]

            m_arrGOPoints[371] = new CPoint(372, 0 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[372] = new CPoint(373, 1 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[373] = new CPoint(374, 2 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[374] = new CPoint(375, 3 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[375] = new CPoint(376, 4 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[376] = new CPoint(377, 5 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[377] = new CPoint(378, 6 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[378] = new CPoint(379, 7 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[379] = new CPoint(380, 8 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[380] = new CPoint(381, 9 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[381] = new CPoint(382, 10 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[382] = new CPoint(383, 11 + fx, 0 + fy, 6, 0);
            m_arrGOPoints[383] = new CPoint(384, 11.5f + fx, 0.5f + fy, 6, 0);
            m_arrGOPoints[384] = new CPoint(385, 11.5f + fx, 1 + fy, 6, 0);
            m_arrGOPoints[385] = new CPoint(386, 11.5f + fx, 2 + fy, 6, 0);
            m_arrGOPoints[386] = new CPoint(387, 11.5f + fx, 3 + fy, 6, 0);
            m_arrGOPoints[387] = new CPoint(388, 11.5f + fx, 4 + fy, 6, 0);
            m_arrGOPoints[388] = new CPoint(389, 11.5f + fx, 5 + fy, 6, 0);
            m_arrGOPoints[389] = new CPoint(390, 11.5f + fx, 6 + fy, 6, 0);
            m_arrGOPoints[390] = new CPoint(391, 11.5f + fx, 7 + fy, 6, 0);
            m_arrGOPoints[391] = new CPoint(392, 11.5f + fx, 8 + fy, 6, 0);
            m_arrGOPoints[392] = new CPoint(393, 11.5f + fx, 9 + fy, 6, 0);
            m_arrGOPoints[393] = new CPoint(394, 11.5f + fx, 10 + fy, 6, 0);
            m_arrGOPoints[394] = new CPoint(395, 11.5f + fx, 11 + fy, 6, 0);
            m_arrGOPoints[395] = new CPoint(396, 11 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[396] = new CPoint(397, 10 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[397] = new CPoint(398, 09 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[398] = new CPoint(399, 08 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[399] = new CPoint(400, 07 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[400] = new CPoint(401, 06 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[401] = new CPoint(402, 05 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[402] = new CPoint(403, 04 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[403] = new CPoint(404, 03 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[404] = new CPoint(405, 02 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[405] = new CPoint(406, 01 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[406] = new CPoint(407, 00 + fx, 11.5f + fy, 6, 0);
            m_arrGOPoints[407] = new CPoint(408, 0 + fx, 11 + fy, 6, 0);
            m_arrGOPoints[408] = new CPoint(409, 0 + fx, 10 + fy, 6, 0);
            m_arrGOPoints[409] = new CPoint(410, 0 + fx, 09 + fy, 6, 0);
            m_arrGOPoints[410] = new CPoint(411, 0 + fx, 08 + fy, 6, 0);
            m_arrGOPoints[411] = new CPoint(412, 0 + fx, 07 + fy, 6, 0);
            m_arrGOPoints[412] = new CPoint(413, 0 + fx, 06 + fy, 6, 0);
            m_arrGOPoints[413] = new CPoint(414, 0 + fx, 05 + fy, 6, 0);
            m_arrGOPoints[414] = new CPoint(415, 0 + fx, 04 + fy, 6, 0);
            m_arrGOPoints[415] = new CPoint(416, 0 + fx, 03 + fy, 6, 0);
            m_arrGOPoints[416] = new CPoint(417, 0 + fx, 02 + fy, 6, 0);
            m_arrGOPoints[417] = new CPoint(418, 0 + fx, 01 + fy, 6, 0);
            m_arrGOPoints[418] = new CPoint(419, 0 + fx, 0.5f + fy, 6, 0);

            // Horizontal Structures - floors, roofs

            // Floor 1st
            m_arrGOPoints[419] = new CPoint(420, 0.5f, 0.5f, 0, 0);
            m_arrGOPoints[420] = new CPoint(421, 0.5f, 0.5f, 0.15f, 0);

            // Roof 1st
            m_arrGOPoints[421] = new CPoint(422, 0.5f, 0.5f, 3, 0);
            m_arrGOPoints[422] = new CPoint(423, 5, 0.5f, 3, 0);
            m_arrGOPoints[423] = new CPoint(424, 0.5f, 0.5f, 3.15f, 0);
            m_arrGOPoints[424] = new CPoint(425, 5, 0.5f, 3.15f, 0);

            // Floor 2nd
            m_arrGOPoints[425] = new CPoint(426, 5.5f, 12, 3, 0);
            m_arrGOPoints[426] = new CPoint(427, 12, 5.5f, 3, 0);
            m_arrGOPoints[427] = new CPoint(428, 5.5f, 12, 3.15f, 0);
            m_arrGOPoints[428] = new CPoint(429, 12, 5.5f, 3.15f, 0);

            // Roof
            m_arrGOPoints[429] = new CPoint(430, 5.5f, 5.5f, 5.8f, 0);
            m_arrGOPoints[430] = new CPoint(431, 5.5f, 5.5f, 5.95f, 0);

            // Column
            m_arrGOPoints[431] = new CPoint(432, 16.5f, 16.5f, 0.5f, 0);

            // Chimnney
            m_arrGOPoints[432] = new CPoint(433, 6, 9, 0, 0);



            // Lines Automatic Generation
            // Lines List - Lines Array

            /*
            int[] iLinesArray_000 = new int[2];
            int[] iLinesArray_001 = new int[2];
            int[] iLinesArray_002 = new int[2];
            int[] iLinesArray_003 = new int[2];
            int[] iLinesArray_004 = new int[2];
            int[] iLinesArray_005 = new int[2];
            int[] iLinesArray_006 = new int[2];
            int[] iLinesArray_007 = new int[2];
            int[] iLinesArray_008 = new int[2];
            int[] iLinesArray_009 = new int[2];
            int[] iLinesArray_010 = new int[2];
            int[] iLinesArray_011 = new int[2];
            int[] iLinesArray_012 = new int[2];
            int[] iLinesArray_013 = new int[2];
            int[] iLinesArray_014 = new int[2];
            int[] iLinesArray_015 = new int[2];
            int[] iLinesArray_016 = new int[2];
            int[] iLinesArray_017 = new int[2];
            int[] iLinesArray_018 = new int[2];
            int[] iLinesArray_019 = new int[2];
            int[] iLinesArray_020 = new int[2];


            iLinesArray_000[0] = 0;
            iLinesArray_001[0] = 1;
            iLinesArray_002[0] = 2;
            iLinesArray_003[0] = 3;
            iLinesArray_004[0] = 4;
            iLinesArray_005[0] = 5;
            iLinesArray_006[0] = 00;
            iLinesArray_007[0] = 07;
            iLinesArray_008[0] = 08;
            iLinesArray_009[0] = 09;
            iLinesArray_010[0] = 10;
            iLinesArray_011[0] = 11;
            iLinesArray_012[0] = 01;
            iLinesArray_013[0] = 01;
            iLinesArray_014[0] = 02;
            iLinesArray_015[0] = 02;
            iLinesArray_016[0] = 03;
            iLinesArray_017[0] = 04;
            iLinesArray_018[0] = 04;
            iLinesArray_019[0] = 05;
            iLinesArray_020[0] = 05;

            iLinesArray_000[1] = 1;
            iLinesArray_001[1] = 2;
            iLinesArray_002[1] = 3;
            iLinesArray_003[1] = 4;
            iLinesArray_004[1] = 5;
            iLinesArray_005[1] = 6;
            iLinesArray_006[1] = 07;
            iLinesArray_007[1] = 08;
            iLinesArray_008[1] = 09;
            iLinesArray_009[1] = 10;
            iLinesArray_010[1] = 11;
            iLinesArray_011[1] = 06;
            iLinesArray_012[1] = 07;
            iLinesArray_013[1] = 08;
            iLinesArray_014[1] = 08;
            iLinesArray_015[1] = 09;
            iLinesArray_016[1] = 09;
            iLinesArray_017[1] = 09;
            iLinesArray_018[1] = 10;
            iLinesArray_019[1] = 10;
            iLinesArray_020[1] = 11;

            m_arrGOLines[00] = new CLine(01, iLinesArray_000, 0);
            m_arrGOLines[01] = new CLine(02, iLinesArray_001, 0);
            m_arrGOLines[02] = new CLine(03, iLinesArray_002, 0);
            m_arrGOLines[03] = new CLine(04, iLinesArray_003, 0);
            m_arrGOLines[04] = new CLine(05, iLinesArray_004, 0);
            m_arrGOLines[05] = new CLine(06, iLinesArray_005, 0);
            m_arrGOLines[06] = new CLine(07, iLinesArray_006, 0);
            m_arrGOLines[07] = new CLine(08, iLinesArray_007, 0);
            m_arrGOLines[08] = new CLine(09, iLinesArray_008, 0);
            m_arrGOLines[09] = new CLine(10, iLinesArray_009, 0);
            m_arrGOLines[10] = new CLine(11, iLinesArray_010, 0);
            m_arrGOLines[11] = new CLine(12, iLinesArray_011, 0);
            m_arrGOLines[12] = new CLine(13, iLinesArray_012, 0);
            m_arrGOLines[13] = new CLine(14, iLinesArray_013, 0);
            m_arrGOLines[14] = new CLine(15, iLinesArray_014, 0);
            m_arrGOLines[15] = new CLine(16, iLinesArray_015, 0);
            m_arrGOLines[16] = new CLine(17, iLinesArray_016, 0);
            m_arrGOLines[17] = new CLine(18, iLinesArray_017, 0);
            m_arrGOLines[18] = new CLine(19, iLinesArray_018, 0);
            m_arrGOLines[19] = new CLine(20, iLinesArray_019, 0);
            m_arrGOLines[20] = new CLine(21, iLinesArray_020, 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrLines, new CCompare_LineID());
            */

            /*
            int[] iVolumePointsArray_000 = new int[8];
            int[] iVolumePointsArray_001 = new int[8];
            */

            Color vColor = Color.FromRgb(0, 51, 0);
            float fvOpacity = 0.98f;

            //SolidColorBrush b = new SolidColorBrush(vColor);
            //b.Opacity = fvOpacity;

            //M.C. code
            //BitmapImage grassjpg = new BitmapImage();
            //grassjpg.BeginInit();
            //grassjpg.UriSource = new Uri(@"grass.jpg", UriKind.RelativeOrAbsolute);
            //grassjpg.EndInit();
            //ImageBrush grassIB = new ImageBrush(grassjpg);
            //grassIB.Viewport = new Rect(0, 0, 1, 1);
            //grassIB.ViewportUnits = BrushMappingMode.Absolute;

            //O.P.
            ImageBrush grassIB = new ImageBrush(new BitmapImage(new Uri(@"grass.jpg", UriKind.RelativeOrAbsolute)));
            grassIB.TileMode = TileMode.Tile;
            grassIB.Viewport = new Rect(0, 0, 0.03, 0.03);

            DiffuseMaterial DiffMat1 = new DiffuseMaterial(grassIB);

            // Ground
            // Level 0 [-1.000]
            m_arrGOVolumes[000] = new CVolume(001, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[000], 100, 100, 1, DiffMat1, true, 0);

            // Level 1 [+0.000]
            vColor = Color.FromRgb(255, 255, 204);

            BitmapImage brickjpg = new BitmapImage();
            brickjpg.BeginInit();
            brickjpg.UriSource = new Uri(@"brick.jpg", UriKind.RelativeOrAbsolute);
            brickjpg.EndInit();
            ImageBrush brickIB = new ImageBrush(brickjpg);
            //brickIB.ViewportUnits = BrushMappingMode.Absolute;
            brickIB.TileMode = TileMode.Tile;
            brickIB.Viewport = new Rect(0, 0, 1.2, 1);
            DiffuseMaterial DiffMatText = new DiffuseMaterial(brickIB);

            Color vColor2 = Color.FromRgb(90, 39, 41);
            float fvOpacity2 = 1.0f;
            SolidColorBrush b2 = new SolidColorBrush(vColor2);
            b2.Opacity = fvOpacity2;
            DiffuseMaterial DiffMat2_WindowLedge = new DiffuseMaterial(b2);

            BitmapImage plasterjpg = new BitmapImage();
            plasterjpg.BeginInit();
            plasterjpg.UriSource = new Uri(@"plaster.jpg", UriKind.RelativeOrAbsolute);
            plasterjpg.EndInit();
            ImageBrush plasterIB = new ImageBrush(plasterjpg);
            plasterIB.ViewportUnits = BrushMappingMode.Absolute;
            DiffuseMaterial DiffMat2_Plaster = new DiffuseMaterial(plasterIB);

            m_arrGOVolumes[001] = new CVolume(002, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[001], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[002] = new CVolume(003, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[002], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[003] = new CVolume(004, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[003], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[004] = new CVolume(005, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[004], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[005] = new CVolume(006, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[005], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[006] = new CVolume(007, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[006], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[007] = new CVolume(008, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[007], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[008] = new CVolume(009, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[008], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[009] = new CVolume(010, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[009], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[010] = new CVolume(011, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[010], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[011] = new CVolume(012, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[011], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[012] = new CVolume(013, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[012], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText, true, 0);
            m_arrGOVolumes[013] = new CVolume(014, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[013], 0.5f, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[014] = new CVolume(015, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[014], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[015] = new CVolume(016, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[015], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[016] = new CVolume(017, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[016], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[017] = new CVolume(018, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[017], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[018] = new CVolume(019, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[018], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[019] = new CVolume(020, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[019], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[020] = new CVolume(021, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[020], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[021] = new CVolume(022, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[021], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  false, 0); // Doors
            m_arrGOVolumes[022] = new CVolume(023, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[022], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[023] = new CVolume(024, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[023], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[024] = new CVolume(025, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[024], 0.5f, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[025] = new CVolume(026, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[025], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[026] = new CVolume(027, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[026], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[027] = new CVolume(028, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[027], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[028] = new CVolume(029, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[028], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[029] = new CVolume(030, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[029], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[030] = new CVolume(031, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[030], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[031] = new CVolume(032, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[031], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[032] = new CVolume(033, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[032], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[033] = new CVolume(034, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[033], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[034] = new CVolume(035, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[034], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[035] = new CVolume(036, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[035], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[036] = new CVolume(037, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[036], 1, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[037] = new CVolume(038, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[037], 0.5f, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[038] = new CVolume(039, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[038], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[039] = new CVolume(040, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[039], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[040] = new CVolume(041, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[040], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[041] = new CVolume(042, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[041], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[042] = new CVolume(043, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[042], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[043] = new CVolume(044, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[043], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[044] = new CVolume(045, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[044], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[045] = new CVolume(046, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[045], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[046] = new CVolume(047, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[046], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[047] = new CVolume(048, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[047], 0.5f, 1, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);
            m_arrGOVolumes[048] = new CVolume(049, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[048], 0.5f, 0.5f, 1,DiffMat2_WindowLedge, DiffMatText,  true, 0);

            // Level 2 [+1.000]

            m_arrGOVolumes[049] = new CVolume(050, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[049], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[050] = new CVolume(051, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[050], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[051] = new CVolume(052, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[051], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[052] = new CVolume(053, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[052], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[053] = new CVolume(054, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[053], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[054] = new CVolume(055, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[054], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[055] = new CVolume(056, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[055], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[056] = new CVolume(057, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[056], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[057] = new CVolume(058, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[057], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[058] = new CVolume(059, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[058], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[059] = new CVolume(060, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[059], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[060] = new CVolume(061, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[060], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[061] = new CVolume(062, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[061], 0.5f, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[062] = new CVolume(063, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[062], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[063] = new CVolume(064, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[063], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[064] = new CVolume(065, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[064], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[065] = new CVolume(066, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[065], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[066] = new CVolume(067, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[066], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[067] = new CVolume(068, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[067], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[068] = new CVolume(069, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[068], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[069] = new CVolume(070, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[069], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0); // Doors
            m_arrGOVolumes[070] = new CVolume(071, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[070], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[071] = new CVolume(072, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[071], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[072] = new CVolume(073, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[072], 0.5f, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[073] = new CVolume(074, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[073], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[074] = new CVolume(075, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[074], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[075] = new CVolume(076, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[075], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[076] = new CVolume(077, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[076], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[077] = new CVolume(078, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[077], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[078] = new CVolume(079, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[078], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[079] = new CVolume(080, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[079], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[080] = new CVolume(081, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[080], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[081] = new CVolume(082, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[081], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[082] = new CVolume(083, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[082], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[083] = new CVolume(084, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[083], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[084] = new CVolume(085, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[084], 1, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[085] = new CVolume(086, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[085], 0.5f, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[086] = new CVolume(087, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[086], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[087] = new CVolume(088, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[087], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[088] = new CVolume(089, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[088], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[089] = new CVolume(090, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[089], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[090] = new CVolume(091, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[090], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[091] = new CVolume(092, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[091], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[092] = new CVolume(093, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[092], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[093] = new CVolume(094, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[093], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[094] = new CVolume(095, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[094], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[095] = new CVolume(096, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[095], 0.5f, 1, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[096] = new CVolume(097, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[096], 0.5f, 0.5f, 1.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);

            // Level 3 [+2.500]

            m_arrGOVolumes[097] = new CVolume(098, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[097], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[098] = new CVolume(099, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[098], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[099] = new CVolume(100, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[099], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[100] = new CVolume(101, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[100], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[101] = new CVolume(102, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[101], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[102] = new CVolume(103, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[102], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[103] = new CVolume(104, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[103], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[104] = new CVolume(105, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[104], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[105] = new CVolume(106, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[105], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[106] = new CVolume(107, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[106], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[107] = new CVolume(108, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[107], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[108] = new CVolume(109, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[108], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[109] = new CVolume(110, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[109], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[110] = new CVolume(111, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[110], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[111] = new CVolume(112, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[111], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[112] = new CVolume(113, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[112], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[113] = new CVolume(114, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[113], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[114] = new CVolume(115, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[114], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[115] = new CVolume(116, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[115], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[116] = new CVolume(117, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[116], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[117] = new CVolume(118, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[117], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[118] = new CVolume(119, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[118], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[119] = new CVolume(120, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[119], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[120] = new CVolume(121, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[120], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[121] = new CVolume(122, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[121], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[122] = new CVolume(123, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[122], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[123] = new CVolume(124, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[123], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[124] = new CVolume(125, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[124], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[125] = new CVolume(126, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[125], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[126] = new CVolume(127, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[126], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[127] = new CVolume(128, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[127], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[128] = new CVolume(129, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[128], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[129] = new CVolume(130, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[129], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[130] = new CVolume(131, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[130], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[131] = new CVolume(132, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[131], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[132] = new CVolume(133, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[132], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[133] = new CVolume(134, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[133], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[134] = new CVolume(135, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[134], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[135] = new CVolume(136, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[135], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[136] = new CVolume(137, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[136], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[137] = new CVolume(138, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[137], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[138] = new CVolume(139, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[138], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[139] = new CVolume(140, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[139], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[140] = new CVolume(141, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[140], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[141] = new CVolume(142, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[141], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[142] = new CVolume(143, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[142], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[143] = new CVolume(144, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[143], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[144] = new CVolume(145, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[144], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);

            // Level 4 [+3.000]

            m_arrGOVolumes[145] = new CVolume(146, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[145], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[146] = new CVolume(147, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[146], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[147] = new CVolume(148, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[147], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[148] = new CVolume(149, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[148], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[149] = new CVolume(150, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[149], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[150] = new CVolume(151, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[150], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[151] = new CVolume(152, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[151], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[152] = new CVolume(153, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[152], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[153] = new CVolume(154, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[153], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[154] = new CVolume(155, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[154], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[155] = new CVolume(156, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[155], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[156] = new CVolume(157, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[156], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[157] = new CVolume(158, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[157], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[158] = new CVolume(159, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[158], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[159] = new CVolume(160, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[159], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[160] = new CVolume(161, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[160], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[161] = new CVolume(162, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[161], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);

            m_arrGOVolumes[162] = new CVolume(163, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[162], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[163] = new CVolume(164, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[163], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[164] = new CVolume(165, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[164], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[165] = new CVolume(166, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[165], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[166] = new CVolume(167, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[166], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[167] = new CVolume(168, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[167], 0.5f, 0.5f, 0.5f,  DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[168] = new CVolume(169, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[168], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[169] = new CVolume(170, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[169], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[170] = new CVolume(171, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[170], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[171] = new CVolume(172, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[171], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[172] = new CVolume(173, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[172], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[173] = new CVolume(174, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[173], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[174] = new CVolume(175, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[174], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[175] = new CVolume(176, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[175], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[176] = new CVolume(177, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[176], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[177] = new CVolume(178, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[177], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[178] = new CVolume(179, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[178], 0.5f, 0.5f, 0.5f,  DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);

            // Level 4 [+3.000]
            vColor = Color.FromRgb(229, 255, 204);

            m_arrGOVolumes[179] = new CVolume(180, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[179], 1, 0.5f, 0.5f, DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[180] = new CVolume(181, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[180], 1, 0.5f, 0.5f, DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[181] = new CVolume(182, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[181], 1, 0.5f, 0.5f, DiffMat2_Plaster, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[182] = new CVolume(183, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[182], 1, 0.5f, 0.5f, DiffMat2_Plaster, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[183] = new CVolume(184, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[183], 1, 0.5f, 0.5f, DiffMat2_Plaster, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[184] = new CVolume(185, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[184], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[185] = new CVolume(186, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[185], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[186] = new CVolume(187, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[186], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[187] = new CVolume(188, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[187], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[188] = new CVolume(189, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[188], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[189] = new CVolume(190, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[189], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[190] = new CVolume(191, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[190], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[191] = new CVolume(192, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[191], 0.5f, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[192] = new CVolume(193, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[192], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[193] = new CVolume(194, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[193], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[194] = new CVolume(195, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[194], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[195] = new CVolume(196, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[195], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[196] = new CVolume(197, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[196], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[197] = new CVolume(198, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[197], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[198] = new CVolume(199, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[198], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[199] = new CVolume(200, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[199], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[200] = new CVolume(201, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[200], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[201] = new CVolume(202, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[201], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[202] = new CVolume(203, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[202], 0.5f, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[203] = new CVolume(204, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[203], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[204] = new CVolume(205, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[204], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[205] = new CVolume(206, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[205], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[206] = new CVolume(207, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[206], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[207] = new CVolume(208, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[207], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[208] = new CVolume(209, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[208], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[209] = new CVolume(210, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[209], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[210] = new CVolume(211, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[210], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[211] = new CVolume(212, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[211], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[212] = new CVolume(213, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[212], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[213] = new CVolume(214, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[213], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[214] = new CVolume(215, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[214], 1, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[215] = new CVolume(216, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[215], 0.5f, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[216] = new CVolume(217, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[216], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[217] = new CVolume(218, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[217], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[218] = new CVolume(219, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[218], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[219] = new CVolume(220, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[219], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[220] = new CVolume(221, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[220], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[221] = new CVolume(222, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[221], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[222] = new CVolume(223, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[222], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[223] = new CVolume(224, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[223], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[224] = new CVolume(225, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[224], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[225] = new CVolume(226, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[225], 0.5f, 1, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[226] = new CVolume(227, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[226], 0.5f, 0.5f, 0.5f,  DiffMat2_Plaster, DiffMat2_Plaster, true, 0);

            // Level 5 [+3.500]

            m_arrGOVolumes[227] = new CVolume(228, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[227], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[228] = new CVolume(229, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[228], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[229] = new CVolume(230, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[229], 1, 0.5f, 0.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[230] = new CVolume(231, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[230], 1, 0.5f, 0.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[231] = new CVolume(232, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[231], 1, 0.5f, 0.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[232] = new CVolume(233, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[232], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[233] = new CVolume(234, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[233], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[234] = new CVolume(235, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[234], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[235] = new CVolume(236, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[235], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[236] = new CVolume(237, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[236], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[237] = new CVolume(238, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[237], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[238] = new CVolume(239, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[238], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[239] = new CVolume(240, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[239], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[240] = new CVolume(241, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[240], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[241] = new CVolume(242, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[241], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[242] = new CVolume(243, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[242], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[243] = new CVolume(244, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[243], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[244] = new CVolume(245, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[244], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[245] = new CVolume(246, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[245], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[246] = new CVolume(247, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[246], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[247] = new CVolume(248, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[247], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[248] = new CVolume(249, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[248], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[249] = new CVolume(250, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[249], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[250] = new CVolume(251, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[250], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[251] = new CVolume(252, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[251], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[252] = new CVolume(253, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[252], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[253] = new CVolume(254, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[253], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[254] = new CVolume(255, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[254], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[255] = new CVolume(256, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[255], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[256] = new CVolume(257, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[256], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[257] = new CVolume(258, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[257], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[258] = new CVolume(259, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[258], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[259] = new CVolume(260, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[259], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[260] = new CVolume(261, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[260], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[261] = new CVolume(262, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[261], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[262] = new CVolume(263, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[262], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[263] = new CVolume(264, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[263], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[264] = new CVolume(265, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[264], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[265] = new CVolume(266, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[265], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[266] = new CVolume(267, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[266], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[267] = new CVolume(268, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[267], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[268] = new CVolume(269, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[268], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[269] = new CVolume(270, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[269], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[270] = new CVolume(271, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[270], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[271] = new CVolume(272, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[271], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[272] = new CVolume(273, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[272], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[273] = new CVolume(274, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[273], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[274] = new CVolume(275, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[274], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);

            // Level 6 [+4.000]

            m_arrGOVolumes[275] = new CVolume(276, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[275], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[276] = new CVolume(277, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[276], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[277] = new CVolume(278, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[277], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[278] = new CVolume(279, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[278], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[279] = new CVolume(280, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[279], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[280] = new CVolume(281, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[280], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[281] = new CVolume(282, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[281], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[282] = new CVolume(283, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[282], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[283] = new CVolume(284, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[283], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[284] = new CVolume(285, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[284], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[285] = new CVolume(286, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[285], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[286] = new CVolume(287, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[286], 1, 0.5f, 1.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[287] = new CVolume(288, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[287], 0.5f, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[288] = new CVolume(289, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[288], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[289] = new CVolume(290, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[289], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[290] = new CVolume(291, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[290], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[291] = new CVolume(292, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[291], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[292] = new CVolume(293, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[292], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[293] = new CVolume(294, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[293], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[294] = new CVolume(295, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[294], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[295] = new CVolume(296, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[295], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[296] = new CVolume(297, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[296], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[297] = new CVolume(298, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[297], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[298] = new CVolume(299, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[298], 0.5f, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[299] = new CVolume(300, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[299], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[300] = new CVolume(301, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[300], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[301] = new CVolume(302, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[301], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[302] = new CVolume(303, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[302], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[303] = new CVolume(304, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[303], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[304] = new CVolume(305, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[304], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[305] = new CVolume(306, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[305], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[306] = new CVolume(307, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[306], 1, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[307] = new CVolume(308, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[307], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[308] = new CVolume(309, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[308], 1, 0.5f, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[309] = new CVolume(310, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[309], 1, 0.5f, 1.5f,DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[310] = new CVolume(311, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[310], 1, 0.5f, 1.5f,DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[311] = new CVolume(312, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[311], 0.5f, 0.5f, 1.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[312] = new CVolume(313, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[312], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[313] = new CVolume(314, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[313], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[314] = new CVolume(315, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[314], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[315] = new CVolume(316, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[315], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[316] = new CVolume(317, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[316], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[317] = new CVolume(318, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[317], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[318] = new CVolume(319, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[318], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[319] = new CVolume(320, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[319], 0.5f, 1, 1.5f, DiffMat2_Plaster, false, 0);
            m_arrGOVolumes[320] = new CVolume(321, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[320], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[321] = new CVolume(322, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[321], 0.5f, 1, 1.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[322] = new CVolume(323, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[322], 0.5f, 0.5f, 1.5f, DiffMat2_Plaster, true, 0);

            // Level 7 [+5.500]

            m_arrGOVolumes[323] = new CVolume(324, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[323], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[324] = new CVolume(325, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[324], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[325] = new CVolume(326, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[325], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[326] = new CVolume(327, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[326], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[327] = new CVolume(328, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[327], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[328] = new CVolume(329, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[328], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[329] = new CVolume(330, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[329], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[330] = new CVolume(331, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[330], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[331] = new CVolume(332, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[331], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[332] = new CVolume(333, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[332], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[333] = new CVolume(334, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[333], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[334] = new CVolume(335, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[334], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[335] = new CVolume(336, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[335], 0.5f, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[336] = new CVolume(337, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[336], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[337] = new CVolume(338, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[337], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[338] = new CVolume(339, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[338], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[339] = new CVolume(340, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[339], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[340] = new CVolume(341, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[340], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[341] = new CVolume(342, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[341], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[342] = new CVolume(343, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[342], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[343] = new CVolume(344, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[343], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[344] = new CVolume(345, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[344], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[345] = new CVolume(346, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[345], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[346] = new CVolume(347, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[346], 0.5f, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[347] = new CVolume(348, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[347], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[348] = new CVolume(349, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[348], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[349] = new CVolume(350, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[349], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[350] = new CVolume(351, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[350], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[351] = new CVolume(352, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[351], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[352] = new CVolume(353, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[352], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[353] = new CVolume(354, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[353], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[354] = new CVolume(355, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[354], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[355] = new CVolume(356, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[355], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[356] = new CVolume(357, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[356], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[357] = new CVolume(358, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[357], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[358] = new CVolume(359, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[358], 1, 0.5f, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[359] = new CVolume(360, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[359], 0.5f, 0.5f, 0.5f, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[360] = new CVolume(361, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[360], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[361] = new CVolume(362, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[361], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[362] = new CVolume(363, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[362], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[363] = new CVolume(364, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[363], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[364] = new CVolume(365, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[364], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[365] = new CVolume(366, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[365], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[366] = new CVolume(367, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[366], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[367] = new CVolume(368, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[367], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[368] = new CVolume(369, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[368], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[369] = new CVolume(370, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[369], 0.5f, 1, 0.5f,  DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[370] = new CVolume(371, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[370], 0.5f, 0.5f, 0.5f, DiffMat2_Plaster, true, 0);

            // Level 8 [+6.000]

            m_arrGOVolumes[371] = new CVolume(372, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[371], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[372] = new CVolume(373, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[372], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[373] = new CVolume(374, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[373], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[374] = new CVolume(375, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[374], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[375] = new CVolume(376, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[375], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[376] = new CVolume(377, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[376], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[377] = new CVolume(378, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[377], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[378] = new CVolume(379, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[378], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[379] = new CVolume(380, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[379], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[380] = new CVolume(381, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[380], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[381] = new CVolume(382, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[381], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[382] = new CVolume(383, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[382], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[383] = new CVolume(384, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[383], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[384] = new CVolume(385, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[384], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[385] = new CVolume(386, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[385], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[386] = new CVolume(387, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[386], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[387] = new CVolume(388, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[387], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[388] = new CVolume(389, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[388], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[389] = new CVolume(390, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[389], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[390] = new CVolume(391, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[390], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[391] = new CVolume(392, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[391], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[392] = new CVolume(393, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[392], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[393] = new CVolume(394, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[393], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[394] = new CVolume(395, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[394], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[395] = new CVolume(396, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[395], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[396] = new CVolume(397, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[396], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[397] = new CVolume(398, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[397], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[398] = new CVolume(399, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[398], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[399] = new CVolume(400, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[399], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[400] = new CVolume(401, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[400], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[401] = new CVolume(402, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[401], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[402] = new CVolume(403, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[402], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[403] = new CVolume(404, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[403], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[404] = new CVolume(405, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[404], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[405] = new CVolume(406, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[405], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[406] = new CVolume(407, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[406], 1, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[407] = new CVolume(408, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[407], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[408] = new CVolume(409, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[408], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[409] = new CVolume(410, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[409], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[410] = new CVolume(411, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[410], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[411] = new CVolume(412, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[411], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[412] = new CVolume(413, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[412], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[413] = new CVolume(414, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[413], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[414] = new CVolume(415, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[414], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[415] = new CVolume(416, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[415], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[416] = new CVolume(417, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[416], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[417] = new CVolume(418, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[417], 0.5f, 1, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);
            m_arrGOVolumes[418] = new CVolume(419, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[418], 0.5f, 0.5f, 0.5f, DiffMat2_WindowLedge, DiffMat2_Plaster, true, 0);

            // Horizontal Structures - floors, roofs

            vColor = Color.FromRgb(255, 255, 255);
            SolidColorBrush b3 = new SolidColorBrush(vColor);
            DiffuseMaterial DiffMat3 = new DiffuseMaterial(b3);

            vColor = Color.FromRgb(51, 25, 0);
            SolidColorBrush b4 = new SolidColorBrush(vColor);
            DiffuseMaterial DiffMat4 = new DiffuseMaterial(b4);

            vColor = Color.FromRgb(25, 51, 0);
            SolidColorBrush b5 = new SolidColorBrush(vColor);
            DiffuseMaterial DiffMat5 = new DiffuseMaterial(b5);

            // Floor 1st
            
            m_arrGOVolumes[419] = new CVolume(420, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[419], 11, 11, 0.15f, DiffMat3, true, 0);

            BitmapImage floor_tilejpg = new BitmapImage();
            floor_tilejpg.BeginInit();
            floor_tilejpg.UriSource = new Uri(@"floor_tile.jpg", UriKind.RelativeOrAbsolute);
            floor_tilejpg.EndInit();
            ImageBrush floor_tileIB = new ImageBrush(floor_tilejpg);
            //floor_tileIB.ViewportUnits = BrushMappingMode.Absolute;

            floor_tileIB.TileMode = TileMode.Tile;
            floor_tileIB.Viewport = new Rect(0, 0, 0.5, 0.5);
            DiffuseMaterial DiffMatTextFloorTile = new DiffuseMaterial(floor_tileIB);

            m_arrGOVolumes[420] = new CVolume(421, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[420], 11, 11, 0.05f, DiffMatTextFloorTile, true, 0);


            // Roof 1st
            m_arrGOVolumes[421] = new CVolume(422, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[421], 4.5f, 11, 0.15f, DiffMat3, true, 0);
            m_arrGOVolumes[422] = new CVolume(423, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[422], 6.5f, 4.5f, 0.15f, DiffMat3, true, 0);


            ImageBrush grassIB2 = new ImageBrush(new BitmapImage(new Uri(@"grass.jpg", UriKind.RelativeOrAbsolute)));
            grassIB2.TileMode = TileMode.Tile;
            grassIB2.Viewport = new Rect(0, 0, 0.3, 0.3);

            DiffuseMaterial matgrassIB = new DiffuseMaterial(grassIB2);

            m_arrGOVolumes[423] = new CVolume(424, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[423], 4.5f, 11, 0.05f, matgrassIB, true, 0);
            m_arrGOVolumes[424] = new CVolume(425, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[424], 6.5f, 4.5f, 0.05f, matgrassIB, true, 0);

            // Floor 2nd
            m_arrGOVolumes[425] = new CVolume(426, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[425], 6.5f, 4.5f, 0.15f, DiffMat3, true, 0);
            m_arrGOVolumes[426] = new CVolume(427, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[426], 4.5f, 11, 0.15f, DiffMat3, true, 0);

            m_arrGOVolumes[427] = new CVolume(428, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[427], 6.5f, 4.5f, 0.05f, DiffMat4, true, 0);
            m_arrGOVolumes[428] = new CVolume(429, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[428], 4.5f, 11, 0.05f, DiffMat4, true, 0);

            // Roof
            m_arrGOVolumes[429] = new CVolume(430, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[429], 11, 11, 0.15f, DiffMat3, true, 0);

            m_arrGOVolumes[430] = new CVolume(431, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[430], 11, 11, 0.05f, DiffMat5, true, 0);

            // Column
            m_arrGOVolumes[431] = new CVolume(432, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[431], 0.5f, 0.5f, 2.5f, DiffMat2_Plaster, true, 0);

            // Chimnney
            ImageBrush brickIB2 = new ImageBrush(brickjpg);
            brickIB2.TileMode = TileMode.Tile;
            brickIB2.Viewport = new Rect(0, 0, 1.5, 0.2);
            DiffuseMaterial matBrick2 = new DiffuseMaterial(brickIB2);

            m_arrGOVolumes[432] = new CVolume(433, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[432], 0.5f, 0.5f, 8, DiffMat2_Plaster, matBrick2, true, 0);

            // Ball - Sphere

            SolidColorBrush brushSphere = new SolidColorBrush(Color.FromRgb(204, 000, 102));
            brushSphere.Opacity = 0.8f;

            DiffuseMaterial mat_Ball = new DiffuseMaterial(brushSphere);

            m_arrGOPoints[449] = new CPoint(450, 4, 4, 3.7, 0);
            m_arrGOVolumes[433] = new CVolume(434, EVolumeShapeType.eShape3D_Sphere, m_arrGOPoints[449], 0.5f, mat_Ball, true, 0);





            // Windows

            Color cFrameColor = new Color();
            cFrameColor = Color.FromRgb(51, 0, 0);
            float fFrameOpacity = 0.99f;

            Color cDoorColor = new Color();
            cDoorColor = Color.FromRgb(139,69,19);
            float fDoorOpacity = 0.99f;

            Color cGlassColor = new Color();
            cGlassColor = Color.FromRgb(173,216,230);
            float fGlassOpacity = 0.5f;

            SolidColorBrush bFrame = new SolidColorBrush(cFrameColor);
            SolidColorBrush bDoor = new SolidColorBrush(cDoorColor);
            SolidColorBrush bGlass = new SolidColorBrush(cGlassColor);

            DiffuseMaterial mat_Frame = new DiffuseMaterial(bFrame);
            DiffuseMaterial mat_Door = new DiffuseMaterial(bDoor);
            DiffuseMaterial mat_Glass = new DiffuseMaterial(bGlass);

            bFrame.Opacity = fFrameOpacity;
            bDoor.Opacity = fDoorOpacity;
            bGlass.Opacity = fGlassOpacity;

            BitmapImage heartsjpg = new BitmapImage();
            heartsjpg.BeginInit();
            heartsjpg.UriSource = new Uri(@"hearts.jpg", UriKind.RelativeOrAbsolute);
            heartsjpg.EndInit();
            ImageBrush heartsIB = new ImageBrush(heartsjpg);
            heartsIB.ViewportUnits = BrushMappingMode.Absolute;
            DiffuseMaterial DiffMatHearts = new DiffuseMaterial(heartsIB);

            float fGlassThickness = 0.020f; // 2x4 mm (glass) + 16 mm (gas)
            m_arrGOPoints[433] = new CPoint(434, 2, 0.2f, 1, 0);
            m_arrGOStrWindows[00] = new CStructure_Window(1, EWindowShapeType.eClassic, 2, m_arrGOPoints[433], 1, 1.5f, 0.1f, mat_Frame, DiffMatHearts, fGlassThickness, 0, true, 0);
            m_arrGOPoints[434] = new CPoint(435, 8, 0.2f, 1, 0);
            m_arrGOStrWindows[01] = new CStructure_Window(2, EWindowShapeType.eClassic, 2, m_arrGOPoints[434], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 0, true, 0);

            m_arrGOPoints[435] = new CPoint(436, 11.8f, 3, 1, 0);
            m_arrGOStrWindows[02] = new CStructure_Window(3, EWindowShapeType.eClassic, 2, m_arrGOPoints[435], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 90, true, 0);
            // Doors
            m_arrGOPoints[436] = new CPoint(437, 11.8f, 8, 0, 0);
            m_arrGOStrWindows[03] = new CStructure_Window(4, EWindowShapeType.eClassic, 1, m_arrGOPoints[436], 1, 2.5f, 0.1f, mat_Frame, mat_Door, 0.03f, 90, true, 0);

            m_arrGOPoints[437] = new CPoint(438, 2, 11.7, 1, 0);
            m_arrGOStrWindows[04] = new CStructure_Window(5, EWindowShapeType.eClassic, 2, m_arrGOPoints[437], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 0, true, 0);
            m_arrGOPoints[438] = new CPoint(439, 8, 11.7, 1, 0);
            m_arrGOStrWindows[05] = new CStructure_Window(6, EWindowShapeType.eClassic, 2, m_arrGOPoints[438], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 0, true, 0);

            m_arrGOPoints[439] = new CPoint(440, 0.3f, 3.2, 1, 0);
            m_arrGOStrWindows[06] = new CStructure_Window(7, EWindowShapeType.eClassic, 2, m_arrGOPoints[439], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 90, true, 0);
            m_arrGOPoints[440] = new CPoint(441, 0.3f, 7, 1, 0);
            m_arrGOStrWindows[07] = new CStructure_Window(8, EWindowShapeType.eClassic, 2, m_arrGOPoints[440], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 90, true, 0);

            //French window
            m_arrGOPoints[441] = new CPoint(442, 7, 5.2f, 3, 0);
            m_arrGOStrWindows[08] = new CStructure_Window(9, EWindowShapeType.eClassic, 3, m_arrGOPoints[441], 1, 2.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 0, true, 0);
            m_arrGOPoints[442] = new CPoint(443, 13, 5.2f, 4, 0);
            m_arrGOStrWindows[9] = new CStructure_Window(10, EWindowShapeType.eClassic, 2, m_arrGOPoints[442], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 0, true, 0);

            m_arrGOPoints[443] = new CPoint(444, 5.5f, 8, 4, 0);
            m_arrGOStrWindows[10] = new CStructure_Window(11, EWindowShapeType.eClassic, 2, m_arrGOPoints[443], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 90, true, 0);
            m_arrGOPoints[444] = new CPoint(445, 5.5f, 12, 4, 0);
            m_arrGOStrWindows[11] = new CStructure_Window(12, EWindowShapeType.eClassic, 2, m_arrGOPoints[444], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 90, true, 0);

            m_arrGOPoints[445] = new CPoint(446, 7, 16.7, 4, 0);
            m_arrGOStrWindows[12] = new CStructure_Window(13, EWindowShapeType.eClassic, 2, m_arrGOPoints[445], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 0, true, 0);
            m_arrGOPoints[446] = new CPoint(447, 13, 16.7, 4, 0);
            m_arrGOStrWindows[13] = new CStructure_Window(14, EWindowShapeType.eClassic, 2, m_arrGOPoints[446], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 0, true, 0);

            m_arrGOPoints[447] = new CPoint(448, 16.8, 8, 4, 0);
            m_arrGOStrWindows[14] = new CStructure_Window(15, EWindowShapeType.eClassic, 2, m_arrGOPoints[447], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 90, true, 0);
            m_arrGOPoints[448] = new CPoint(449, 16.8, 12, 4, 0);
            m_arrGOStrWindows[15] = new CStructure_Window(16, EWindowShapeType.eClassic, 2, m_arrGOPoints[448], 1, 1.5f, 0.1f, mat_Frame, mat_Glass, fGlassThickness, 90, true, 0);

            // Setridit pole podle ID
            Array.Sort(m_arrGOPoints, new CCompare_PointID());
        }
    }
}
