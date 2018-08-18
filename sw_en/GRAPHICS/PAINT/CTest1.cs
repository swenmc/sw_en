using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BaseClasses;
using MATERIAL;
using CRSC;
using BaseClasses.CRSC;

namespace CENEX
{
    public class CTest1
    {
        public CNode[] arrNodes = new CNode[6];
        public CMember[] arrMembers = new CMember[9];
        public CMat[] arrMat = new CMat[5];
        public CCrSc[] arrCrSc = new CCrSc[3];
        public CNSupport[] arrNSupports = new CNSupport[3];
        public CNLoad[] arrNLoads = new CNLoad[3];

        // Temporary objecs for testing

        // public CCrSc_0_00 objCrScSolid = new CCrSc_0_00(100);
        // public CCrSc_0_01 objCrScSolid = new CCrSc_0_01(100);
        // public CCrSc_0_02 objCrScSolid = new CCrSc_0_02(50);
        // public CCrSc_0_03 objCrSc = new CCrSc_0_03(100, 50);
        // public CCrSc_0_04 objCrSc = new CCrSc_0_04(100);
        // public CCrSc_0_04 objCrSc = new CCrSc_0_04(100, 40);
        // public CCrSc_0_04 objCrSc = new CCrSc_0_04(-100, -40, 100,0, 0,50);
        public CCrSc_0_05 objCrScWF = new CCrSc_0_05(100, 50);
        // public CCrSc_0_20 objCrScHollow = new CCrSc_0_20(200, 10);
        // public CCrSc_0_22 objCrScHollow = new CCrSc_0_22(200, 10);
        // public CCrSc_0_23 objCrScHollow = new CCrSc_0_23(200, 100, 5);
        // public CCrSc_0_24 objCrScHollow = new CCrSc_0_24(100, 10);
        // public CCrSc_0_25 objCrScHollow = new CCrSc_HL(200, 100, 10, 5);
        // public CCrSc_0_26 objCrScHollow = new CCrSc_0_26(200,10);
        // public CCrSc_0_27 objCrScHollow = new CCrSc_0_27(200, 10);
        // public CCrSc_0_28 objCrScHollow = new CCrSc_0_28(200, 10);
        // public CCrSc_0_50 objCrScSolid = new CCrSc_0_50(200,100,10,5);
        // public CCrSc_0_50 objCrScSolid = new CCrSc_0_50(200, 100, 150, 10, 15, 5, 70);
        // public CCrSc_0_52 objCrScSolid = new CCrSc_0_52(200, 100, 10, 5, 30);
        // public CCrSc_0_52 objCrScSolid = new CCrSc_0_52(200, 100, 150, 10, 15, 5, 30, 70);
        // public CCrSc_0_54 objCrScSolid = new CCrSc_0_54(200, 100, 10, 5,30,50);
        // public CCrSc_0_56 objCrScSolid = new CCrSc_0_56(200, 100, 10, 5,130);
        // public CCrSc_0_56 objCrScSolid = new CCrSc_0_56(200, 100, 30, 10, 5, 20, 130);
        // public CCrSc_0_58 objCrScSolid = new CCrSc_0_58(200, 100, 10, 5);
        // public CCrSc_0_58 objCrScSolid = new CCrSc_0_58(200, 100, 150, 10, 15, 5, 30, 70);
        // public CCrSc_0_60 objCrScSolid = new CCrSc_0_60(200,100,10);
        // public CCrSc_0_61 objCrScSolid = new CCrSc_0_61(100,10);
        // public CCrSc_3_00 objCrScSolid = new CCrSc_3_00(0,200,90,11.3f,7.5f,7.5f,4.5f,159.1f);
        // public CCrSc_3_00 objCrScSolid = new CCrSc_3_00(1, 200, 90, 11.3f, 7.5f, 4.5f, 159.1f);
        // public CCrSc_3_00 objCrScSolid = new CCrSc_3_00(2, 200, 90, 11.3f, 7.5f, 7.5f, 159.1f);
        // public CCrSc_3_01 objCrScSolid = new CCrSc_3_01(0, 200, 90, 120, 11.3f, 11.3f, 7.5f, 7.5f, 4.5f, 159.1f);
        // public CCrSc_3_01 objCrScSolid = new CCrSc_3_01(1, 200, 90, 120, 11.3f, 11.3f, 7.5f, 4.5f, 80.0f);
        // public CCrSc_3_01 objCrScSolid = new CCrSc_3_01(2, 200, 80, 100, 10, 8, 12, 10, 5, 5, 80);
        // public CCrSc_3_02 objCrScSolid = new CCrSc_3_02(0,300, 100, 16, 10, 10, 8, 232, 27);
        // public CCrSc_3_02 objCrScSolid = new CCrSc_3_02(2,300, 100, 12.5f, 10, 10, 250, 30);
        // public CCrSc_3_03 objCrScSolid = new CCrSc_3_03(150, 10, 15, 10, 50);
        // public CCrSc_3_04 objCrScSolid = new CCrSc_3_04(250, 150, 20, 15, 20, 50, 75);
        // public CCrSc_3_05 objCrScHollow = new CCrSc_3_05(200,10);
        // public CCrSc_3_06 objCrScHollow = new CCrSc_3_06(200, 100, 20);
        // public CCrSc_3_07 objCrScHollow = new CCrSc_3_07(0, 200, 100, 10, 30); // Both radii, coincident centres
        // public CCrSc_3_07 objCrScHollow = new CCrSc_3_07(1, 200, 100, 10, 5, 30); // Both radii, incoincident centres
        // public CCrSc_3_07 objCrScHollow = new CCrSc_3_07(2, 500, 300, 20, 50); // Outside radius = 0
        // public CCrSc_3_07 objCrScHollow = new CCrSc_3_07(3, 500, 300, 30); // Outside radius = 0, coincident centres
        // public CCrSc_3_07 objCrScHollow = new CCrSc_3_07(0, 400, 150, 20); // Both radii, coincident centres
        // public CCrSc_3_07 objCrScHollow = new CCrSc_3_07(2, 500, 300, 20); // Outside radius = 0
        // public CCrSc_3_07 objCrScHollow = new CCrSc_3_07(3, 500, 300, 30); // Inside radius = 0, coincident centres
        // public CCrSc_3_07 objCrScHollow = new CCrSc_3_07(5, 400, 150, 20); // No radii, Outside radius = 0, Inside radius = 0
        // public CCrSc_3_08 objCrScSolid = new CCrSc_3_08(0, 120, 100, 12.5f, 15, 100, 10, 10, 5, 90);
        // public CCrSc_3_08 objCrScSolid = new CCrSc_3_08(1, 120, 100, 12.5f, 15, 100, 10, 10, 5, 95);
        // public CCrSc_3_08 objCrScSolid = new CCrSc_3_08(2, 120, 100, 12.5f, 15, 100, 10, 5, 105);
        // public CCrSc_3_08 objCrScSolid = new CCrSc_3_08(3, 120, 100, 12.5f, 15, 100, 10, 5, 95);

        public CTest1()
        {
            // !!!!!! Vytvarat len ODKAZY na objekty "ref", aby sa zbytocne nevytvarali lokalne kopie

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array

            arrMat[0] = new CMat(); // Vytvarat priamo pre konkretne typy materialov (uzivatelsky, ocel, beton, drevo, hlinik)
            arrMat[1] = new CMat_02_00();
            arrMat[2] = new CMat_03_00();
            arrMat[3] = new CMat_05_00();
            arrMat[4] = new CMat_09_00();

            // CrSc
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array

            arrCrSc[0] = new CCrSc_3_07(3, 500, 300, 30);
            arrCrSc[1] = new CCrSc_0_05(100, 50);
            arrCrSc[2] = new CCrSc_3_04(250, 150, 20, 15, 20, 50, 75);

            arrCrSc[0].m_Mat = arrMat[0];
            arrCrSc[1].m_Mat = arrMat[1];
            arrCrSc[2].m_Mat = arrMat[4];

            // Nodes
            // Nodes List - Nodes Array

            arrNodes[0] = new CNode(1, 500, 0, 2500, 0);
            arrNodes[1] = new CNode(2, 2500, 0, 2500, 0);
            arrNodes[2] = new CNode(3, 5500, 0, 2500, 0);
            arrNodes[3] = new CNode(4, 500, 0, 500, 0);
            arrNodes[4] = new CNode(5, 2500, 0, 500, 0);
            arrNodes[5] = new CNode(6, 5500, 0, 500, 0);

            // Sort by ID
            //Array.Sort(arrNodes, new BaseClasses.CCompare_NodeID());

            // Members
            // Members List - Members Array

            arrMembers[0] = new CMember(1, arrNodes[0], arrNodes[1], arrCrSc[0], 0);
            arrMembers[1] = new CMember(2, arrNodes[1], arrNodes[2], arrCrSc[0], 0);
            arrMembers[2] = new CMember(3, arrNodes[0], arrNodes[3], arrCrSc[1], 0);
            arrMembers[3] = new CMember(4, arrNodes[1], arrNodes[4], arrCrSc[0], 0);
            arrMembers[4] = new CMember(5, arrNodes[2], arrNodes[5], arrCrSc[0], 0);
            arrMembers[5] = new CMember(6, arrNodes[3], arrNodes[4], arrCrSc[2], 0);
            arrMembers[6] = new CMember(7, arrNodes[4], arrNodes[5], arrCrSc[0], 0);
            arrMembers[7] = new CMember(8, arrNodes[1], arrNodes[3], arrCrSc[0], 0);
            arrMembers[8] = new CMember(9, arrNodes[1], arrNodes[5], arrCrSc[1], 0);

            //Sort by ID
            //Array.Sort(arrMembers, new BaseClasses.CCompare_MemberID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, false, true, false };
            bool[] bSupport2 = { false, false, true, false, true, false };
            bool[] bSupport3 = { true, false, false, false, false, false };

            // Create Support Objects
            // Pozn. Jednym z parametrov by malo byt pole ID uzlov v ktorych je zadefinovana tato podpora
            // objekt podpory bude len jeden a dotknute uzly budu vediet ze na ich podpora existuje a ake je konkretne ID jej nastaveni
            arrNSupports[0] = new CNSupport(6,1, arrNodes[0], bSupport1, 0);
            arrNSupports[1] = new CNSupport(6,2, arrNodes[2], bSupport2, 0);
            arrNSupports[2] = new CNSupport(6,3, arrNodes[5], bSupport3, 0);

            // Sort by ID
            Array.Sort(arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Pozn. Jednym z parametrov by malo byt pole ID prutov v ktorych je zadefinovany klb
            // objekt klbu bude len jeden a dotknute pruty budu vediet ze na ich klb existuje, v ktorom uzle (start 1 / End 2) a ake je konkretne ID jeho nastaveni

            // Set values
            //bool[] bMembRelase1 = { true, false, false, false, false, false };
            //bool[] bMembRelase2 = { false, false, true, false, false, false };
            //bool[] bMembRelase3 = { false, false, false, false, true, false };
            bool? [] bMembRelase4 = { true, false, true, false, true, false };


            // Create Release / Hinge Objects
            arrMembers[7].CnRelease1 = new CNRelease(6,bMembRelase4, 0);
            arrMembers[8].CnRelease2 = new CNRelease(6,bMembRelase4, 0);

            // Nodal Forces - fill values

            // Pozn. Jednym z parametrov by malo byt pole ID uzlov v ktorych je zadefinovane uzlove zatazenie
            // objekt zatazenia bude len jeden a dotknute uzly budu vediet ze na ich dane zatazenie existuje a ake je konkretne ID jeho vlastnosti

            arrNLoads[0] = new CNLoadSingle(arrNodes[1], ENLoadType.eNLT_Fx, 40.0f, true, 0);
            arrNLoads[1] = new CNLoadSingle(arrNodes[4], ENLoadType.eNLT_Fx, -60.0f, true, 0);
            arrNLoads[2] = new CNLoadSingle(arrNodes[5], ENLoadType.eNLT_Fz, 80.0f, true, 0);
        }
    }
}
