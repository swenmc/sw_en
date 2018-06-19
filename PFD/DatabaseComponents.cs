using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class DatabaseComponents
    {
        public float fbX_mm;
        public float fhY_mm;
        public float flZ_mm; // Not used in 2D model
        public float ft_mm; // Not used in 2D model
        public int iHolesNoumber;

        public DatabaseComponents()
        { }

        public DatabaseComponents(int iSerieIndex, int iComponentIndex)
        {
            if (iSerieIndex == 0)
            {
                fbX_mm = arr_Serie_B_Dimension[iComponentIndex, 0];
                fhY_mm = arr_Serie_B_Dimension[iComponentIndex, 1];
                flZ_mm = arr_Serie_B_Dimension[iComponentIndex, 2];
                ft_mm = arr_Serie_B_Dimension[iComponentIndex, 3];
                iHolesNoumber = (int)arr_Serie_B_Dimension[iComponentIndex, 4];
            }
            else if (iSerieIndex == 1)
            {
                fbX_mm = arr_Serie_L_Dimension[iComponentIndex, 0];
                fhY_mm = arr_Serie_L_Dimension[iComponentIndex, 1];
                flZ_mm = arr_Serie_L_Dimension[iComponentIndex, 2];
                ft_mm = arr_Serie_L_Dimension[iComponentIndex, 3];
                iHolesNoumber = (int)arr_Serie_L_Dimension[iComponentIndex, 4];
            }
            else
            {
                // Exception - not implemented
            }
        }

        public string[] arr_SeriesNames = new string[9]
            {"Serie B",
             "Serie L",
             "Serie LL",
             "Serie F",
             "Serie Q",
             "Serie S",
             "Serie T",
             "Serie X",
             "Serie Y"};

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
    }
}
