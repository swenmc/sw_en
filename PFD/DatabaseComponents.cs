using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;

namespace PFD
{
    public class DatabaseComponents
    {
        public float fbX_mm;
        public float fbX2_mm;
        public float fhY_mm;
        public float fhY2_mm;
        public float flZ_mm; // Not used in 2D model
        public float flZ2_mm;
        public float ft_mm; // Not used in 2D model
        public int iHolesNoumber;

        public DatabaseComponents()
        { }

        public DatabaseComponents(int iSerieIndex, int iComponentIndex)
        {
            ESerieTypePlate eSerieIndex = (ESerieTypePlate)iSerieIndex;

            switch (eSerieIndex)
            {
                case ESerieTypePlate.eSerie_B:
                    {
                        fbX_mm = arr_Serie_B_Dimension[iComponentIndex, 0];
                        fhY_mm = arr_Serie_B_Dimension[iComponentIndex, 1];
                        flZ_mm = arr_Serie_B_Dimension[iComponentIndex, 2];
                        ft_mm = arr_Serie_B_Dimension[iComponentIndex, 3];
                        iHolesNoumber = (int)arr_Serie_B_Dimension[iComponentIndex, 4];

                        break;
                    }
                case ESerieTypePlate.eSerie_F:
                    {
                        fbX_mm = arr_Serie_F_Dimension[iComponentIndex, 0];
                        fbX2_mm = arr_Serie_F_Dimension[iComponentIndex, 1];
                        fhY_mm = arr_Serie_F_Dimension[iComponentIndex, 2];
                        flZ_mm = arr_Serie_F_Dimension[iComponentIndex, 3];
                        ft_mm = arr_Serie_F_Dimension[iComponentIndex, 4];
                        iHolesNoumber = (int)arr_Serie_L_Dimension[iComponentIndex, 5];

                        break;
                    }
                case ESerieTypePlate.eSerie_L:
                    {
                        fbX_mm = arr_Serie_L_Dimension[iComponentIndex, 0];
                        fhY_mm = arr_Serie_L_Dimension[iComponentIndex, 1];
                        flZ_mm = arr_Serie_L_Dimension[iComponentIndex, 2];
                        ft_mm = arr_Serie_L_Dimension[iComponentIndex, 3];
                        iHolesNoumber = (int)arr_Serie_L_Dimension[iComponentIndex, 4];

                        break;
                    }
                case ESerieTypePlate.eSerie_Q:
                    {
                        fbX_mm = arr_Serie_Q_Dimension[iComponentIndex, 0];
                        fhY_mm = arr_Serie_Q_Dimension[iComponentIndex, 1];
                        flZ_mm = arr_Serie_Q_Dimension[iComponentIndex, 2];
                        ft_mm = arr_Serie_Q_Dimension[iComponentIndex, 3];
                        iHolesNoumber = (int)arr_Serie_Q_Dimension[iComponentIndex, 4];

                        break;
                    }
                case ESerieTypePlate.eSerie_T:
                    {
                        fbX_mm = arr_Serie_T_Dimension[iComponentIndex, 0];
                        fhY_mm = arr_Serie_T_Dimension[iComponentIndex, 1];
                        flZ_mm = arr_Serie_T_Dimension[iComponentIndex, 2];
                        ft_mm = arr_Serie_T_Dimension[iComponentIndex, 3];
                        iHolesNoumber = (int)arr_Serie_T_Dimension[iComponentIndex, 4];

                        break;
                    }
                case ESerieTypePlate.eSerie_Y:
                    {
                        fbX_mm = arr_Serie_Y_Dimension[iComponentIndex, 0];
                        fhY_mm = arr_Serie_Y_Dimension[iComponentIndex, 1];
                        flZ_mm = arr_Serie_Y_Dimension[iComponentIndex, 2];
                        flZ2_mm = arr_Serie_Y_Dimension[iComponentIndex, 3];
                        ft_mm = arr_Serie_Y_Dimension[iComponentIndex, 4];
                        iHolesNoumber = (int)arr_Serie_Y_Dimension[iComponentIndex, 5];

                        break;
                    }
                case ESerieTypePlate.eSerie_J:
                    {
                        fbX_mm = arr_Serie_J_Dimension[iComponentIndex, 0];
                        fhY_mm = arr_Serie_J_Dimension[iComponentIndex, 1];
                        fhY2_mm = arr_Serie_J_Dimension[iComponentIndex, 2];
                        flZ_mm = arr_Serie_J_Dimension[iComponentIndex, 3];
                        ft_mm = arr_Serie_J_Dimension[iComponentIndex, 4];
                        iHolesNoumber = (int)arr_Serie_J_Dimension[iComponentIndex, 5];

                        break;
                    }
                case ESerieTypePlate.eSerie_K:
                    {
                        fbX_mm = arr_Serie_K_Dimension[iComponentIndex, 0];
                        fhY_mm = arr_Serie_K_Dimension[iComponentIndex, 1];
                        fbX2_mm = arr_Serie_K_Dimension[iComponentIndex, 2];
                        fhY2_mm = arr_Serie_K_Dimension[iComponentIndex, 3];
                        ft_mm = arr_Serie_K_Dimension[iComponentIndex, 4];
                        iHolesNoumber = (int)arr_Serie_K_Dimension[iComponentIndex, 5];

                        break;
                    }

                default:
                    {
                        // Not implemented
                        break;
                    }
            }
        }

        public string[] arr_SeriesNames = new string[11]
            {"Serie B",
             "Serie L",
             "Serie LL",
             "Serie F",
             "Serie Q",
             "Serie S",
             "Serie T",
             "Serie X",
             "Serie Y",
             "Serie J",
             "Serie K"};

        public string[] arr_Serie_B_Names = new string[10]
            {"BA",
             "BB",
             "BC",
             "BD",
             "BE",
             "BF",
             "BG",
             "BH",
             "BI",
             "BJ"};

        public string[] arr_Serie_L_Names = new string[11]
            {"LA",
             "LB",
             "LC",
             "LD",
             "LE",
             "LF",
             "LG",
             "LH",
             "LI",
             "LJ",
             "LK"};

        public string[] arr_Serie_LL_Names = new string[2]
            {"LLH",
             "LLK"};

        public string[] arr_Serie_F_Names = new string[10]
            {"FA - LH",
             "FA - RH",
             "FB - LH",
             "FB - RH",
             "FC - LH",
             "FC - RH",
             "FD - LH",
             "FD - RH",
             "FE - LH",
             "FE - RH"};

        public string[] arr_Serie_Q_Names = new string[1]
            {"Q"};

        public string[] arr_Serie_S_Names = new string[1]
            {"S"};

        public string[] arr_Serie_T_Names = new string[1]
            {"T"};

        public string[] arr_Serie_X_Names = new string[1]
            {"X"};

        public string[] arr_Serie_Y_Names = new string[1]
            {"Y"};

        public string[] arr_Serie_J_Names = new string[2]
        {"JA", "JB"};

        public string[] arr_Serie_K_Names = new string[1]
        {"KA"};

        public float[,] arr_Serie_B_Dimension = new float[10, 5]
            {
                {140, 270, 180, 3, 4},
                {072, 290, 180, 3, 2},
                {145, 290, 180, 3, 4},
                {100, 490, 154, 3, 3},
                {102, 540, 154, 3, 4},
                {185, 580, 400, 3, 6},
                {072, 270, 180, 3, 2},
                {092, 092, 075, 3, 2},
                {092, 092, 075, 3, 1},
                {185, 580, 180, 3, 3}
            };

        public float[,] arr_Serie_L_Dimension = new float[11, 5]
            {
                {050, 268, 120, 3, 0},
                {075, 288, 150, 3, 0},
                {085, 268, 085, 3, 0},
                {085, 288, 085, 3, 0},
                {100, 495, 100, 3, 0},
                {100, 545, 100, 3, 0},
                {150, 545, 150, 3, 0},
                {050, 268, 050, 3, 16},
                {050, 340, 050, 3, 16},
                {050, 100, 050, 3, 8},
                {050, 288, 050, 3, 16}
            };

        public float[,] arr_Serie_LL_Dimension = new float[2, 6]
    {
                {050, 072, 268, 050, 2, 0},
                {050, 072, 288, 050, 2, 0}
    };

        public float[,] arr_Serie_F_Dimension = new float[10, 6]
            {
                {120, 035, 600, 065, 2, 0},
                {120, 035, 600, 065, 2, 0},
                {110, 035, 545, 065, 2, 0},
                {110, 035, 545, 065, 2, 0},
                {110, 035, 490, 065, 2, 0},
                {110, 035, 490, 065, 2, 0},
                {200, 035, 545, 050, 2, 0},
                {200, 035, 545, 050, 2, 0},
                {200, 035, 490, 050, 2, 0},
                {200, 035, 490, 050, 2, 0}
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

        public float[,] arr_Serie_J_Dimension = new float[2, 6]
        {
                // b, h1, h2, t, L, iHoles
                {500, 300, 350, 0, 3, 0},
                {500, 300, 350, 50, 3, 0}
        };

        public float[,] arr_Serie_K_Dimension = new float[1, 6]
        {
                // b, h, b2, h2, t, iHoles
                {300, 700, 500, 1000, 3, 0}
        };


        // Cross-section - len docasne, mali by byt v samostatnej databaze
        public string[] arr_Serie_CrSc_FormSteel_Names = new string[8]
        {
                "Box-10075",
                "Z",
                "C-single",
                "C-back to back",
                "C-nested",
                "Box-63020",
                "SmartDek",
                "PurlinDek"
        };

        public string[] arr_Serie_Box_FormSteel_Names = new string[1]
        {
                "10075"
        };

        public float[,] arr_Serie_Box_FormSteel_Dimension = new float[1, 3]
        {
        {100, 100, 0.75f}
        };

        public string[] arr_Serie_C_FormSteel_Names = new string[4]
        {
                "27095",
                "270115",
                "27055",
                "50020"
        };

        public float[,] arr_Serie_C_FormSteel_Dimension = new float[4, 3]
        {
                {70,270,0.95f},
                {70,270,1.15f},
                {70,270,0.55f},
                {100,500,1.95f}
        };

        public string[] arr_Serie_Box63020_FormSteel_Names = new string[3]
        {
                "63020-Without Stiffener",
                "63020-Single Stiffened",
                "63020-Double Stiffened"
        };

        public float[,] arr_Serie_Box63020_FormSteel_Dimension = new float[3, 4]
        {
                {180, 630, 1.95f, 1.95f},
                {180, 630, 1.95f, 4.95f},
                {180, 630, 1.95f, 7.95f}
        };
    }
}
