using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CFootingLoadCombinationRatio_ULS
    {
        private CMember MMember;
        private CConnectionJointTypes MJoint;
        private CFoundation MFooting;
        private CLoadCombination MLoadCombination;
        private float MMaximumDesignRatio;

        private designInternalForces MDesignInternalForces;


        public CMember Member
        {
            get
            {
                return MMember;
            }

            set
            {
                MMember = value;
            }
        }

        public CConnectionJointTypes Joint
        {
            get
            {
                return MJoint;
            }

            set
            {
                MJoint = value;
            }
        }

        public CLoadCombination LoadCombination
        {
            get
            {
                return MLoadCombination;
            }

            set
            {
                MLoadCombination = value;
            }
        }

        public float MaximumDesignRatio
        {
            get
            {
                return MMaximumDesignRatio;
            }

            set
            {
                MMaximumDesignRatio = value;
            }
        }

        public designInternalForces DesignInternalForces
        {
            get
            {
                return MDesignInternalForces;
            }

            set
            {
                MDesignInternalForces = value;
            }
        }

        public CFoundation Footing
        {
            get
            {
                return MFooting;
            }

            set
            {
                MFooting = value;
            }
        }

        public CFootingLoadCombinationRatio_ULS()
        {

        }
        public CFootingLoadCombinationRatio_ULS(CMember member, CConnectionJointTypes joint, CFoundation footing, CLoadCombination loadCombination, float maxDesignRatio, designInternalForces designIF)
        {
            Member = member;
            Joint = joint;
            Footing = footing;
            LoadCombination = loadCombination;
            MaximumDesignRatio = maxDesignRatio;
            DesignInternalForces = designIF;
        }
    }
}
