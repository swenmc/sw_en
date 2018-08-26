using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMemberLoadForces
    {
        private CMember MMember;
        private CLoadCase MLoadCase;
        private basicInternalForces[] MForces;
        private designMomentValuesForCb MMomentValues;

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

        public basicInternalForces[] Forces
        {
            get
            {
                return MForces;
            }

            set
            {
                MForces = value;
            }
        }

        public designMomentValuesForCb MomentValues
        {
            get
            {
                return MMomentValues;
            }

            set
            {
                MMomentValues = value;
            }
        }

        public CMemberLoadForces()
        {

        }
        public CMemberLoadForces(CMember member, CLoadCase loadcase, basicInternalForces[] forces, designMomentValuesForCb momentValuesForCb)
        {
            MMember = member;
            MLoadCase = loadcase;
            MForces = forces;
            MMomentValues = momentValuesForCb;
        }
    }
}
