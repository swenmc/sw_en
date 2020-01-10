using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BaseClasses;
using DATABASE;
using DATABASE.DTO;

namespace PFD
{
    public class CDatabaseComponents
    {
        // Task 413
        // Toto som zatial prerobil ale nie som si isty ci fakt takto mam prerabat kazdu seriu

        public List<string> arr_SeriesNames = CJointsManager.GetPlateSeries();

        public List<string> arr_Serie_B_Names = CJointsManager.GetPlateB_Names();

        public List<string> arr_Serie_B_ScrewArrangement_Names = CJointsManager.GetArrayPlate_ArrangementNames("B");

        public List<string> arr_Serie_L_Names = CJointsManager.GetPlateL_Names();

        public List<string> arr_Serie_L_ScrewArrangement_Names = CJointsManager.GetArrayPlate_ArrangementNames("L");

        public List<string> arr_Serie_F_Names = CJointsManager.GetPlateF_Names();

        public List<string> arr_Serie_F_ScrewArrangement_Names = CJointsManager.GetArrayPlate_ArrangementNames("F");

        public List<string> arr_Serie_LL_Names = CJointsManager.GetPlateLL_Names();

        public List<string> arr_Serie_LL_ScrewArrangement_Names = CJointsManager.GetArrayPlate_ArrangementNames("LL");

        //------------------------------------------------------------------------------------------------
        // Task 413
        // TO Ondrej - Tohoto sa chcem zbavit a ziskavat to z databazy
        public List<string> arr_Serie_G_Names = new List<string>(2)
            {"G - LH",
             "G - RH"};

    public List<string> arr_Serie_H_Names = new List<string>(2)
            {"H - LH",
             "H - RH"};

        public List<string> arr_Serie_Q_Names = new List<string>(1)
            {"Q"};

        public List<string> arr_Serie_S_Names = new List<string>(1)
            {"S"};

        public List<string> arr_Serie_T_Names = new List<string>(1)
            {"T"};

        public List<string> arr_Serie_X_Names = new List<string>(1)
            {"X"};

        public List<string> arr_Serie_Y_Names = new List<string>(1)
            {"Y"};
        //------------------------------------------------------------------------------------------------

        public List<string> arr_Serie_J_Names = new List<string>(3)
        {"JA", "JB", "JC"};

        public List<string> arr_Serie_J_ScrewArrangement_Names = new List<string>(3)
        {"Undefined",
         "Rectangular",
         "Circle"};

        public List<string> arr_Serie_K_Names = new List<string>(7)
        {"KA",
         "KB",
         "KC",
         "KD",
         "KE",
         "KF",
         "KK"};

        public List<string> arr_Serie_K_ScrewArrangement_Names = new List<string>(3)
        {"Undefined",
         "Rectangular",
         "Circle"};

        //------------------------------------------------------------------------------------------------
        // Task 413
        // TO Ondrej - Tohoto sa chcem zbavit a ziskavat to z databazy
        public List<string> arr_Serie_M_Names = new List<string>(1)
        {"M"};

        public List<string> arr_Serie_M_ScrewArrangement_Names = CJointsManager.GetArrayPlate_ArrangementNames("M");

        //------------------------------------------------------------------------------------------------
        // Task 413
        // TO Ondrej - Tohoto sa chcem zbavit a ziskavat to z databazy
        public List<string> arr_Serie_N_Names = new List<string>(1)
        {"N"};

        //------------------------------------------------------------------------------------------------
        // Task 413
        // TO Ondrej - Tohoto sa chcem zbavit a ziskavat to z databazy
        public List<string> arr_Serie_O_Names = new List<string>(1)
        {"O"};

        public List<string> arr_Serie_O_ScrewArrangement_Names = CJointsManager.GetArrayPlate_ArrangementNames("O");

        //------------------------------------------------------------------------------------------------
        // Task 413
        // TO Ondrej - Tohoto sa chcem zbavit a ziskavat to z databazy

        public float[,] arr_Serie_G_Dimension = new float[2, 7]
        {
                // bX1, bX2, hY, hY2, lZ, t, iNumberOfHoles
                {500-180, 50, 800, 300, 100, 3, 2},
                {500-180, 50, 800, 300, 100, 3, 2},
        };

        public float[,] arr_Serie_H_Dimension = new float[2, 5]
        {
                {500, 400, 600, 3, 49+21},
                {500, 400, 600, 3, 49+21}
        };

        public float[,] arr_Serie_Q_Dimension = new float[1, 5]
        {
                {272, 200, 70, 2, 0}
        };

        public float[,] arr_Serie_S_Dimension = new float[1, 4]
        {
                {270, 270, 1, 0}
        };

        public float[,] arr_Serie_T_Dimension = new float[1, 5]
        {
                {72, 495, 49, 2, 0}
        };

        public float[,] arr_Serie_X_Dimension = new float[1, 6]
        {
                {195, 490, 25, 2, 0, 400}
        };

        public float[,] arr_Serie_Y_Dimension = new float[1, 6]
        {
                {140, 600, 270, 40, 3, 0}
        };
        //------------------------------------------------------------------------------------------------

        public float[,] arr_Serie_J_Dimension = new float[3, 7]
        {
                // b, h1, h2, l, t, radius holes, corner holes
                {1400, 650, 800, 0, 3, 20, 4},  // JA
                {1400, 650, 800, 50, 3, 20, 4}, // JB
                {361, 260, 289, 50, 3, 20, 4}   // JC
        };

        public float[,] arr_Serie_K_Dimension = new float[7, 9]
        {
                // bR, b, h, b2, h2, l, t, radius holes, corner holes
                {0, 630, 1250, 800, 1400, 0, 3, 20, 4},   // KA
                {0, 630, 1250, 800, 1400, 50, 3, 20, 4},  // KB
                {0, 630, 1250, 800, 1400, 50, 3, 20, 4},  // KC
                {0, 630, 1250, 800, 1400, 50, 3, 20, 4},  // KD
                {0, 630, 1250, 800, 1400, 50, 3, 20, 4},  // KE
                {0, 630, 1280, 800, 1430, 50, 3, 20, 4},  // KF
                {200, 630, 1250, 800, 1400, 50, 3, 20, 4} // KK
        };

        //------------------------------------------------------------------------------------------------
        // Task 413
        // TO Ondrej - Tohoto sa chcem zbavit a ziskavat to z databazy
        public float[,] arr_Serie_M_Dimension = new float[1, 7]
        {
                // b, h, t, iHoles, bBeam, slope_deg, gamma_deg
                {1000, 100, 1, 6, 70, 5, 30}
        };

        public float[,] arr_Serie_N_Dimension = new float[1, 6]
        {
                // b1, b3, h, z, t, iHoles
                {100, 100, 100, 300, 2, 12}
        };

        public float[,] arr_Serie_O_Dimension = new float[1, 6]
        {
                // b1, b2, h1, h2, t, iHoles
                {200, 100, 500, 800, 3, 20}
        };
        //------------------------------------------------------------------------------------------------

        // Cross-section - len docasne, mali by byt v samostatnej databaze
        public List<string> arr_Serie_CrSc_FS_Names = new List<string>
        {
                "Box-10075",
                "Z",
                "C-single",
                "C-back to back",
                "C-nested",
                "Box-63020",
                "Smartdek",
                "Purlindek"
        };

        public List<string> arr_Serie_Box_FS_Names = new List<string>(1)
        {
                "10075"
        };

        public Color[] arr_Serie_Box_FS_Colors = new Color[1]
        {
            Colors.Red,
        };

        public float[,] arr_Serie_Box_FS_Dimension = new float[1, 3]
        {
                {100, 100, 0.75f}
        };

        public List<string> arr_Serie_Z_FS_Names = new List<string>(1)
        {
                "Template Z"
        };

        public Color[] arr_Serie_Z_FS_Colors = new Color[1]
        {
            Colors.Gold,
        };

        public float[,] arr_Serie_Z_FS_Dimension = new float[1, 4]
        {
                // h, b_flange, c_lip, t
                {200, 100, 20, 0.95f}
        };

        public List<string> arr_Serie_C_FS_Names = new List<string>(4)
        {
                "27095",
                "270115",
                "27055",
                "50020"
        };

        public Color[] arr_Serie_C_FS_Colors = new Color[4]
        {
                Colors.Yellow,
                Colors.Violet,
                Color.FromRgb(254,153,0), // Orange Peel
                Colors.Green
        };

        public float[,] arr_Serie_C_FS_Dimension = new float[4, 3]
        {
                {70,270,0.95f},
                {70,270,1.15f},
                {70,270,0.55f},
                {100,500,1.95f}
        };

        public List<string> arr_Serie_C_BtoB_FS_Names = new List<string>(1)
        {
                "270115 back to back"
        };

        public Color[] arr_Serie_C_BtoB_FS_Colors = new Color[1]
        {
               Color.FromRgb(204,204,204)
        };

        public float[,] arr_Serie_C_BtoB_FS_Dimension = new float[1, 4]
        {
                {2*70,270,40,1.15f}
        };

        public List<string> arr_Serie_C_Nested_FS_Names = new List<string>(2)
        {
                "270115 nested",
                "50020 nested"
        };

        public Color[] arr_Serie_C_Nested_FS_Colors = new Color[2]
        {
               Color.FromRgb(122,40,204),
               Color.FromRgb(10,40,200)
        };

        public float[,] arr_Serie_C_Nested_FS_Dimension = new float[2, 3]
        {
                {70,290,1.15f},
                {102,550,1.95f},
        };

        public List<string> arr_Serie_Box63020_FS_Names = new List<string>(3)
        {
                "63020-Without Stiffener",
                "63020-Single Stiffened",
                "63020-Double Stiffened"
        };

        public Color[] arr_Serie_Box63020_FS_Colors = new Color[3]
        {
               Color.FromRgb(0,38,255),  // Blue
               Color.FromRgb(0,148,255), // Blue
               Color.FromRgb(0,255,255)  // Blue
        };

        public float[,] arr_Serie_Box63020_FS_Dimension = new float[3, 5]
        {
                {180, 630, 1.95f, 1.95f, 0},
                {180, 630, 1.95f, 4.95f, 1}, // 3.00 mm
                {180, 630, 1.95f, 7.90f, 2}  // 3.00 mm
        };

        public List<string> arr_Serie_Screws_Names = new List<string>(1)
        {
               "Hex Head Tek"
        };
    }
}
