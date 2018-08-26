using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMemberLoadCombinationRatio
    {
        private CMember MMember;
        private CLoadCombination MLoadCombination;
        private float MMaximumDesignRatio;

        private designInternalForces MDesignInternalForces;
        private designMomentValuesForCb MDesignMomentValuesForCb;


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

        public designMomentValuesForCb DesignMomentValuesForCb
        {
            get
            {
                return MDesignMomentValuesForCb;
            }

            set
            {
                MDesignMomentValuesForCb = value;
            }
        }

        public CMemberLoadCombinationRatio()
        {

        }
        public CMemberLoadCombinationRatio(CMember member, CLoadCombination loadCombination, float maxDesignRatio,
            designInternalForces designInternalForces, designMomentValuesForCb designMomentValuesForCb)
        {
            MMember = member;
            MLoadCombination = loadCombination;
            MMaximumDesignRatio = maxDesignRatio;
            MDesignInternalForces = designInternalForces;
            MDesignMomentValuesForCb = designMomentValuesForCb;

        }
    }
}
