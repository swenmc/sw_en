using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMemberDeflectionsInLoadCases
    {
        private CMember MMember;
        private CLoadCase MLoadCase;
        private basicDeflections[] MDeflections;

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

        public basicDeflections[] Deflections
        {
            get
            {
                return MDeflections;
            }

            set
            {
                MDeflections = value;
            }
        }

        public CMemberDeflectionsInLoadCases()
        {

        }
        public CMemberDeflectionsInLoadCases(CMember member, CLoadCase loadcase, basicDeflections[] deflections)
        {
            Member = member;
            LoadCase = loadcase;
            Deflections = deflections;
        }
    }
}
