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
        public float fPhi_SecondaryMember;
        public float fA_n_SecondaryMember;
        public float fN_t_SecondaryMember;
        public float fEta_N_t_5423_SecondaryMember;

        public CJointDesignDetails_CrossBracing()
        {

        }
    }
}
