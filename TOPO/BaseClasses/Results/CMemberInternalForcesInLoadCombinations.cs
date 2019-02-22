using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMemberInternalForcesInLoadCombinations
    {
        private CMember MMember;
        private CLoadCombination MLoadCombination;
        private basicInternalForces[] MInternalForces;
        private designMomentValuesForCb MBendingMomentValues;

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

        public basicInternalForces[] InternalForces
        {
            get
            {
                return MInternalForces;
            }

            set
            {
                MInternalForces = value;
            }
        }

        public designMomentValuesForCb BendingMomentValues
        {
            get
            {
                return MBendingMomentValues;
            }

            set
            {
                MBendingMomentValues = value;
            }
        }

        public CMemberInternalForcesInLoadCombinations()
        {

        }
        public CMemberInternalForcesInLoadCombinations(CMember member, CLoadCombination loadcombination, basicInternalForces[] internalForces, designMomentValuesForCb bendingMomentValuesForCb)
        {
            MMember = member;
            MLoadCombination = loadcombination;
            MInternalForces = internalForces;
            MBendingMomentValues = bendingMomentValuesForCb;
        }
    }
}
