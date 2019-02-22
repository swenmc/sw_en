using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMemberLoadCombinationRatio_SLS
    {
        private CMember MMember;
        private CLoadCombination MLoadCombination;
        private float MMaximumDesignRatio;

        private designDeflections MDesignDeflections;

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

        public designDeflections DesignDeflections
        {
            get
            {
                return MDesignDeflections;
            }

            set
            {
                MDesignDeflections = value;
            }
        }

        public CMemberLoadCombinationRatio_SLS()
        {

        }
        public CMemberLoadCombinationRatio_SLS(CMember member, CLoadCombination loadCombination, float maxDesignRatio,
           designDeflections designDeflections_temp)
        {
            Member = member;
            LoadCombination = loadCombination;
            MaximumDesignRatio = maxDesignRatio;
            DesignDeflections = designDeflections_temp;
        }
    }
}
