using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMemberInternalForcesInLoadCases
    {
        private CMember MMember;
        private CLoadCase MLoadCase;
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

        public CLoadCase LoadCase
        {
            get
            {
                return MLoadCase;
            }

            set
            {
                MLoadCase = value;
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

        public CMemberInternalForcesInLoadCases()
        {

        }
        public CMemberInternalForcesInLoadCases(CMember member, CLoadCase loadcase, basicInternalForces[] internalForces, designMomentValuesForCb bendingMomentValuesForCb)
        {
            MMember = member;
            MLoadCase = loadcase;
            MInternalForces = internalForces;
            MBendingMomentValues = bendingMomentValuesForCb;
        }
    }
}
