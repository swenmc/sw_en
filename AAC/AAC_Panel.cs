using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses;
using BaseClasses.GraphObj;
using _3DTools;
using MATH;

namespace AAC
{
    public class AAC_Panel : Object3DModel
    {
        public float fL = 0.0f;   // Panel Length

        public float fc_1 = 0.0f; // Concrete Cover
        public float fc_2 = 0.0f; // Concrete Cover
        public float fc_trans_upper = 0.0f; // Concrete cover of transversal reinforcement (distance between end of bar and concrete surface)
        public float fc_trans_lower = 0.0f;

        public int number_long_upper_bars = 0;
        public int number_long_lower_bars = 0;
        public int number_trans_upper_bars = 0;
        public int number_trans_lower_bars = 0;

        public float fd_long_upper = 0.0f;
        public float fd_long_lower = 0.0f;
        public float fd_trans_upper = 0.0f;
        public float fd_trans_lower = 0.0f;

        public float fsl_upper = 0.0f;
        public float fsl_lower = 0.0f;

        public int number_tr_rein_arr_1 = 0;
        public int number_tr_rein_arr_2 = 0;
        public int number_tr_rein_arr_3 = 0;

        public float fx_tr_rein_arr_1 = 0.0f;
        public float fx_tr_rein_arr_2 = 0.0f;
        public float fx_tr_rein_arr_3 = 0.0f;

        public E_AACElementType panelType;

        public enum E_AACElementType
        {
            Floor_Panel,
            Roof_Panel,
            Vertical_Wall_Panel_1,
            Vertical_Wall_Panel_2,
            Horizontal_Wall_Panel,
            Beam
        };

        public MATERIAL.CMat_02_00_AAC Concrete;
        public MATERIAL.CMat_03_00 Reinforcement;
        public CCrSc Cross_Section;

        public ReinforcementBar[] Long_Bottom_Bars_Array;
        public ReinforcementBar[] Long_Upper_Bars_Array;
        public ReinforcementBar[] Trans_Bottom_Bars_Array;
        public ReinforcementBar[] Trans_Upper_Bars_Array;

        public AAC_Panel()
        {
        }

        public AAC_Panel(E_AACElementType AAC_panel_type,
            float fL_temp,
            CCrSc Cross_Section_temp,
            MATERIAL.CMat_02_00_AAC Concrete_temp,
            MATERIAL.CMat_03_00 Reinforcement_temp,
            int iLB_No,
            int iLU_No,
            int iTB_No,
            int iTU_No,
            float d_long_upper,
            float d_long_lower,
            float d_trans_upper,
            float d_trans_lower,
            float sl_upper,
            float sl_lower,
            float LB_FrontSideCover,
            float LU_FrontSideCover,
            float TB_FrontSideCover,
            float TU_FrontSideCover,
            int iTRA_No_1,
            int iTRA_No_2,
            int iTRA_No_3,
            float TRA_dist_x_1,
            float TRA_dist_x_2,
            float TRA_dist_x_3)
        {
            panelType = AAC_panel_type;

            fL = fL_temp;

            Cross_Section = Cross_Section_temp;

            Concrete = new MATERIAL.CMat_02_00_AAC();
            Concrete = Concrete_temp;

            Reinforcement = new MATERIAL.CMat_03_00();
            Reinforcement = Reinforcement_temp;

            number_long_upper_bars = iLU_No;
            number_long_lower_bars = iLB_No;
            number_trans_upper_bars = iTU_No;
            number_trans_lower_bars = iTB_No;

            fc_1 = LB_FrontSideCover;
            fc_2 = LU_FrontSideCover;
            fc_trans_lower = TB_FrontSideCover;
            fc_trans_upper = TU_FrontSideCover;

            fd_long_upper = d_long_upper;
            fd_long_lower = d_long_lower;
            fd_trans_upper = d_trans_upper;
            fd_trans_lower = d_trans_lower;

            fsl_upper = sl_upper;
            fsl_lower = sl_lower;

            number_tr_rein_arr_1 = iTRA_No_1;
            number_tr_rein_arr_2 = iTRA_No_2;
            number_tr_rein_arr_3 = iTRA_No_3;

            fx_tr_rein_arr_1 = TRA_dist_x_1;
            fx_tr_rein_arr_2 = TRA_dist_x_2;
            fx_tr_rein_arr_3 = TRA_dist_x_3;

        Long_Bottom_Bars_Array = new ReinforcementBar[iLB_No];
            Long_Upper_Bars_Array = new ReinforcementBar[iLU_No];
            Trans_Bottom_Bars_Array = new ReinforcementBar[iTB_No];
            Trans_Upper_Bars_Array = new ReinforcementBar[iTU_No];
        }

        public void FillReinforcementData(int iDirection, ReinforcementBar[] ArrayOfBars, float fd, float fFrontSideCover)
        {
            for (int i = 0; i < ArrayOfBars.Length; i++)
            {
                float fL_bar;
                if (iDirection == 0) // Longitudinal Bars
                    fL_bar = fL - 2 * fFrontSideCover;
                else // Transversal bars
                    fL_bar = (float)Cross_Section.b - 2 * fFrontSideCover;

                ArrayOfBars[i] = new ReinforcementBar(fd, fL_bar, Reinforcement);
                
            }
        }
    }
}
