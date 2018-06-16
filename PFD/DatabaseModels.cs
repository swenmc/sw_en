using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class DatabaseModels
    {
        public float fb_mm;
        public float fL_mm;
        public float fL1_mm;
        public float fh_mm;
        public int iFrNo;
        public float fRoof_Pitch_deg;
        public float fdist_girt_mm;
        public float fdist_purlin_mm;
        public float fdist_frontcolumn_mm;

        public DatabaseModels()
        { }

        public DatabaseModels(int iSelectedIndex)
        {
            fb_mm = arr_Models_Dimensions[iSelectedIndex, 0];
            fL_mm = arr_Models_Dimensions[iSelectedIndex, 1];
            fh_mm = arr_Models_Dimensions[iSelectedIndex, 2];
            iFrNo = (int)arr_Models_Dimensions[iSelectedIndex, 3];
            fL1_mm = fL_mm / (iFrNo - 1);
            fRoof_Pitch_deg = 10;
            fdist_girt_mm = 0.25f * fL1_mm;
            fdist_purlin_mm = 0.25f * fL1_mm;
            fdist_frontcolumn_mm = 0.5f * fL1_mm;
        }

        //MODEL GABLE WIDTH (MM) LENGTH (MM) WALL HEIGHT(MM)
        /*
        GB0810L 8000 10000 3600
        GB0812L 8000 12000 3600
        GB0812M 8000 12000 4200
        GB1012L 10000 12000 3600
        GB1012M 10000 12000 4200
        GB1212L 12000 12000 3600
        GB1212M 12000 12000 4200
        GB1215M 12000 15000 4200
        GB1218M 12000 18000 4200
        GB1218H 12000 18000 4800
        GB1518M 15000 18000 4200
        GB1518H 15000 18000 4800
        GB1820M 18000 20000 4200
        GB1820H 18000 20000 4800
        GB1824H 18000 24000 4800
        GB1824XH 18000 24000 6000
        GB2024H 20000 24000 4800
        GB2024XH 20000 24000 6000
        GB2430H 24000 30000 4800
        GB2430XH 24000 30000 6000
        GB3036H 30000 36000 4800
        GB3036XH 30000 36000 6000
        GB3042H 30000 42000 4800
        GB3042XH 30000 42000 6000
        */

        public string []arr_ModelNames = new string[24]
            {"GB0810L ",
             "GB0812L ",
             "GB0812M ",
             "GB1012L ",
             "GB1012M ",
             "GB1212L ",
             "GB1212M ",
             "GB1215M ",
             "GB1218M ",
             "GB1218H ",
             "GB1518M ",
             "GB1518H ",
             "GB1820M ",
             "GB1820H ",
             "GB1824H ",
             "GB1824XH",
             "GB2024H ",
             "GB2024XH",
             "GB2430H ",
             "GB2430XH",
             "GB3036H ",
             "GB3036XH",
             "GB3042H ",
             "GB3042XH"};

        public float[,] arr_Models_Dimensions = new float[24, 4]
            {
                {08000, 10000, 3600, 3},
                {08000, 12000, 3600, 4},
                {08000, 12000, 4200, 4},
                {10000, 12000, 3600, 4},
                {10000, 12000, 4200, 4},
                {12000, 12000, 3600, 4},
                {12000, 12000, 4200, 4},
                {12000, 15000, 4200, 5},
                {12000, 18000, 4200, 4},
                {12000, 18000, 4800, 4},
                {15000, 18000, 4200, 4},
                {15000, 18000, 4800, 4},
                {18000, 20000, 4200, 5},
                {18000, 20000, 4800, 5},
                {18000, 24000, 4800, 6},
                {18000, 24000, 6000, 6},
                {20000, 24000, 4800, 6},
                {20000, 24000, 6000, 6},
                {24000, 30000, 4800, 7},
                {24000, 30000, 6000, 7},
                {30000, 36000, 4800, 7},
                {30000, 36000, 6000, 7},
                {30000, 42000, 4800, 8},
                {30000, 42000, 6000, 8}
            };
     }
}
