using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class CJointDesignDetails_CrossBracing : CJointDesignDetails
    {
        // Cross-bracing design
        public float fPhi_CrSc;
        public float fA_n_SecondaryMember;
        public float fN_t_Section_SecondaryMember;
        public float fEta_N_t_5423_SecondaryMember;

        public int iNumberOfScrewsInShear;

        public float fPhi_shear_Vb_5424;
        public float fe_x;
        public float fC_for5424;
        public float fV_b_for5424;
        public float fV_asterix_b_for5424;
        public float fEta_5424_1;
        public float fV_asterix_fv;
        public float fV_fv;
        public float fEta_V_fv_5425;
        public float fV_w_nom_screw_5426;
        public float fEta_V_w_5426;

        public CJointDesignDetails_CrossBracing()
        {

        }
    }
}
