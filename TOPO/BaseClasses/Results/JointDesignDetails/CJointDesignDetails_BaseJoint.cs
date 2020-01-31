using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class CJointDesignDetails_BaseJoint : CJointDesignDetails
    {
        public float fN_asterix_joint_uplif;
        public float fN_asterix_joint_bearing;

        // Plate design

        // Plate tension design 3.2.1
        //public float fA_g_plate;
        //public float fN_t_plate_321;
        //public float fEta_N_t_plate_321;

        // Plate tension design
        public float fPhi_plate;
        public float fA_n_plate;
        public float fN_t_plate;
        public float fEta_N_t_5423_plate;

        // Plate compression - bearing
        public float fPhi_c_plate;
        public float fA_c_plate;
        public float fN_s_plate;
        public float fEta_341a_plate;

        // Plate shear resistance
        public float fPhi_v_plate;
        public float fA_vn_yv_plate;
        public float fV_y_yv_plate;
        public float fEta_V_yv_3341_plate;

        public float fEta_V_yv_723_11_plate;

        // Plate bending resistance
        public float fPhi_b_plate;
        public float fM_xu_resistance_plate;
        public float fEta_Mb_plate;

        // Shear in connection
        public float fPhi_shear_screw;
        public float fVb_MainMember;
        public int iNumberOfScrewsInShear;
        public float fEta_MainMember;

        // Plastic Design
        public float fMb_MainMember_oneside_plastic;
        public float fEta_Mb_MainMember_oneside_plastic;

        // Elastic Design
        public float fV_asterix_b_max_screw_Mxu;
        public float fV_asterix_b_max_screw_Vyv;
        public float fV_asterix_b_max_screw_N;
        public float fV_asterix_b_max_screw;

        public float fEta_Vb_5424_MainMember;

        // 5.4.2.5 Connection shear as limited by end distance
        public float fe;
        public float fV_fv_MainMember;
        public float fV_fv_Plate;
        public float fV_asterix_fv;

        public float fEta_V_fv_5425_MainMember;
        public float fEta_V_fv_5425_Plate;
        public float fEta_V_fv_5425;

        // 5.4.2.6 Screws in shear
        public float fV_w_nom_screw_5426;
        public float fEta_V_w_5426;

        // 5.4.2.3 Tension in the connected part
        public float fPhi_CrSc;
        public float fA_n_MainMember;
        public float fN_t_section_MainMember;
        public float fEta_N_t_5423_MainMember;

        // Plate local bending - uplift tension force
        public float fPhi_b_Plate;
        public float fa_force;
        public float fM_y_asterix_plate;
        public float fZ_pl_y_plate;
        public float fM_s_y_plate;
        public float fEta_M_s_y_331_1_plate;

        public CJointDesignDetails_BaseJoint()
        {

        }
    }
}
