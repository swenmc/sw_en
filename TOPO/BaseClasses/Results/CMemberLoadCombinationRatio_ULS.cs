using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMemberLoadCombinationRatio_ULS
    {
        private CMember MMember;
        private CLoadCombination MLoadCombination;
        private float MMaximumDesignRatio;

        private designInternalForces MDesignInternalForces;
        private designBucklingLengthFactors MDesignBucklingLengthFactors;
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

        public designBucklingLengthFactors DesignBucklingLengthFactors
        {
            get
            {
                return MDesignBucklingLengthFactors;
            }

            set
            {
                MDesignBucklingLengthFactors = value;
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

        public CMemberLoadCombinationRatio_ULS()
        {

        }
        public CMemberLoadCombinationRatio_ULS(CMember member, CLoadCombination loadCombination, float maxDesignRatio,
            designInternalForces designInternalForces, designBucklingLengthFactors bucklingLengthFactors, designMomentValuesForCb designMomentValuesForCb)
        {
            Member = member;
            LoadCombination = loadCombination;
            MaximumDesignRatio = maxDesignRatio;
            DesignInternalForces = designInternalForces;
            DesignBucklingLengthFactors = bucklingLengthFactors;
            DesignMomentValuesForCb = designMomentValuesForCb;
        }
    }
}
