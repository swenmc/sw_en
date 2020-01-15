using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class CJointDesignDetails_FrontOrBackColumnToMainRafterJoint : CJointDesignDetails
    {
        // 5.4.3 Screwed connections in tension
        // 5.4.3.2 Pull-out and pull-over (pull-through)
        public int iNumberOfScrewsInTension;
        public float fPhi_N_screw;
        public float fN_t_5432_MainMember;
        public float fEta_N_t_5432_MainMember;

        // Pripoj plechu k hlavnemu prutu
        // Tension and shear
        public float fPhi_shear_Vb_Nov;
        public float fC_for5434_MainMember;
        public float fV_b_for5434_MainMember;
        public float fd_w_for5434_plate;
        public float fN_ov_for5434_plate;
        public float fV_asterix_b_for5434_MainMember;
        public float fEta_5434_MainMember;

        public float fV_b_for5435_MainMember;
        public float fN_ou_for5435_MainMember;
        public float fV_asterix_b_for5435_MainMember;
        public float fEta_5435_MainMember;

        // 5.4.2.5 Connection shear as limited by end distance
        public float fe_Plate;
        public float fV_asterix_fv_plate;
        public float fV_fv_Plate;
        public float fEta_V_fv_5425_Plate;

        // 5.4.2.6 Screws in shear
        public float fV_w_nom_screw_5426;
        public float fEta_V_w_5426;

        // 5.4.3.3 Screws in tension
        public float fPhi_N_t_screw;
        public float fN_t_nom_screw_5433;
        public float fEta_N_t_screw_5433;

        // 5.4.3.6 Screws subject to combined shear and tension
        public float fEta_V_N_t_screw_5436;

        // Plate design
        public float fPhi_plate;
        public float fA_n_plate;
        public float fN_t_plate;
        public float fEta_N_t_5423_plate;

        public float fA_vn_yv_plate;
        public float fV_y_yv_plate;
        public float fEta_V_yv_3341_plate;

        // Pripoj plechu sekundarneho pruta
        public int iNumberOfScrewsInConnectionOfSecondaryMember;

        public float fPhi_V_screw;

        public float fV_asterix_b_SecondaryMember;
        public float fVb_SecondaryMember;
        public float fEta_Vb_5424_SecondaryMember;

        public float fe_SecondaryMember;
        public float fV_asterix_fv_SecondaryMember;
        public float fV_fv_SecondaryMember;
        public float fEta_V_fv_5425_SecondaryMember;


        // Plates G and H

        // 5.4.2 Screwed connections in shear
        public float fPhi_shear_Vb_5424;

        // Plate G
        public float ft_1_Plate1;
        public float ff_uk_1_Plate1;
        public float ff_yk_1_Plate1;

        // Left Leg
        public int iNumberOfScrewInTension_Plate1_Left;

        // Right Leg
        public int iNumberOfScrewsInShear_Plate1_Right;

        // 5.4.2.4 Tilting and hole bearing
        public float fC_for5424_Plate1;
        public float fV_b_for5424_Plate1;
        public float fV_asterix_b_for5424_Plate1;
        public float fEta_5424_1_Plate1;

        // 5.4.2.5 Connection shear as limited by end distance 
        public float fe_Plate1;
        public float fV_asterix_fv_Plate1;
        public float fV_fv_Plate1;
        public float fEta_V_fv_5425_Plate1;

        // 5.4.2.6 Screws in shear
        public float fV_w_nom_screw_5426_Plate1;
        public float fEta_V_w_5426_Plate1;

        // Plate H
        public float ft_1_Plate2;
        public float ff_uk_1_Plate2;
        public float ff_yk_1_Plate2;

        // Bottom Leg
        public int iNumberOfScrewsInShear_Plate2_BL;

        // 5.4.2.4 Tilting and hole bearing
        public float fC_for5424_Plate2_BL;
        public float fV_b_for5424_Plate2_BL;
        public float fV_asterix_b_for5424_Plate2_BL;
        public float fEta_5424_1_Plate2_BL;

        // 5.4.2.5 Connection shear as limited by end distance 
        public float fe_Plate2_BL;
        public float fV_asterix_fv_Plate2_BL;
        public float fV_fv_Plate2_BL;
        public float fEta_V_fv_5425_Plate2_BL;

        // 5.4.2.6 Screws in shear
        public float fV_w_nom_screw_5426_Plate2_BL;
        public float fEta_V_w_5426_Plate2_BL;

        // Top Leg
        public int iNumberOfScrewsInShear_Plate2_TL;

        // 5.4.2.4 Tilting and hole bearing
        public float fC_for5424_Plate2_TL;
        public float fV_b_for5424_Plate2_TL;
        public float fV_asterix_b_for5424_Plate2_TL;
        public float fEta_5424_1_Plate2_TL;

        // 5.4.2.5 Connection shear as limited by end distance 
        public float fe_Plate2_TL;
        public float fV_asterix_fv_Plate2_TL;
        public float fV_fv_Plate2_TL;
        public float fEta_V_fv_5425_Plate2_TL;

        // 5.4.2.6 Screws in shear
        public float fV_w_nom_screw_5426_Plate2_TL;
        public float fEta_V_w_5426_Plate2_TL;


        public CJointDesignDetails_FrontOrBackColumnToMainRafterJoint()
        {

        }
    }
}
