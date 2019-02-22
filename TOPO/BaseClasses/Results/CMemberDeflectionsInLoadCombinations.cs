using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CMemberDeflectionsInLoadCombinations
    {
        private CMember MMember;
        private CLoadCombination MLoadCombination;
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

        public CMemberDeflectionsInLoadCombinations()
        {

        }
        public CMemberDeflectionsInLoadCombinations(CMember member, CLoadCombination loadcombination, basicDeflections[] deflections)
        {
            Member = member;
            LoadCombination = loadcombination;
            Deflections = deflections;
        }
    }
}
