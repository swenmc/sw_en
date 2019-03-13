using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class CJointDesignDetails_BaseJoint : CJointDesignDetails
    {
        // Plate design

        public float fPhi_plate;
        public float fA_n_plate;
        public float fN_t_plate;
        public float fEta_N_t_5423_plate;

        public float fA_vn_yv_plate;
        public float fV_y_yv_plate;
        public float fEta_V_yv_3341_plate;

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




        // Anchors
        public float fN_asterix_joint_uplif;
        public float fN_asterix_joint_bearing;

        public float fV_asterix_x_joint;
        public float fV_asterix_y_joint;
        public float fV_asterix_res_joint;

        public int iNumberAnchors;
        public int iNumberAnchors_t;
        public int iNumberAnchors_v;

        public float fN_asterix_anchor_uplif;
        public float fV_asterix_anchor;
        public float fplateWidth_x;
        public float fplateWidth_y;

        public float fFootingDimension_x;
        public float fFootingDimension_y;
        public float fFootingHeight;

        public float fe_x_AnchorToPlateEdge;
        public float fe_y_AnchorToPlateEdge;

        public float fe_x_BasePlateToFootingEdge;
        public float fe_y_BasePlateToFootingEdge;

        public float fu_x_Washer;
        public float fu_y_Washer;

        public float ff_apostrophe_c;
        public float fRho_c;

        public float fd_s;
        public float fd_f;

        public float fA_c;
        public float fA_o;

        public float ff_y_anchor;
        public float ff_u_anchor;

        // AS / NZS 4600:2018 - 5.3 Bolted connections
        public float fPhi_v_532;
        public float fV_f_532;
        public float fDesignRatio_532_1;

        public float fPhi_v_534;
        public float fAlpha_5342;
        public float fC_5342;
        public float fV_b_5342;
        public float fDesignRatio_5342;

        public float fV_b_5343;
        public float fDesignRatio_5343;

        public float fPhi_535;
        public float fV_fv_5351_2_anchor;
        public float fDesignRatio_5351_2;

        public float fN_ft_5352_1;
        public float fDesignRatio_5352_1;

        public float fDesignRatio_5353;



        public CJointDesignDetails_BaseJoint()
        {

        }
    }
}
