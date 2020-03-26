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
        //private designMomentValuesForCb[] MBendingMomentValues;
        private designBucklingLengthFactors[] MBucklingLengthFactors;

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

        /*
        public designMomentValuesForCb[] BendingMomentValues
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
        */

        public designBucklingLengthFactors[] BucklingLengthFactors
        {
            get
            {
                return MBucklingLengthFactors;
            }

            set
            {
                MBucklingLengthFactors = value;
            }
        }

        public CMemberInternalForcesInLoadCases()
        {

        }
        public CMemberInternalForcesInLoadCases(CMember member, CLoadCase loadcase, basicInternalForces[] internalForces, /*designMomentValuesForCb[] bendingMomentValuesForCb,*/ designBucklingLengthFactors[] bucklingLengthFactors)
        {
            MMember = member;
            MLoadCase = loadcase;
            MInternalForces = internalForces;
            //MBendingMomentValues = bendingMomentValuesForCb;
            MBucklingLengthFactors = bucklingLengthFactors;
        }

        public override bool Equals(object obj)
        {
            if (obj is CMemberInternalForcesInLoadCases)
            {
                CMemberInternalForcesInLoadCases mif = obj as CMemberInternalForcesInLoadCases;
                if (mif.Member.ID != this.Member.ID) return false;
                if (mif.LoadCase.ID != this.LoadCase.ID) return false;
                if (mif.InternalForces.Length != this.InternalForces.Length) return false;
                for (int i = 0; i < mif.InternalForces.Length; i++)
                {
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fM_yu, this.InternalForces[i].fM_yu)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fM_yy, this.InternalForces[i].fM_yy)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fM_zv, this.InternalForces[i].fM_zv)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fM_zz, this.InternalForces[i].fM_zz)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fN, this.InternalForces[i].fN)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fT, this.InternalForces[i].fT)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fV_yu, this.InternalForces[i].fV_yu)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fV_yy, this.InternalForces[i].fV_yy)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fV_zv, this.InternalForces[i].fV_zv)) return false;
                    if (!MATH.MathF.d_equal(mif.InternalForces[i].fV_zz, this.InternalForces[i].fV_zz)) return false;
                }
                return true;
            }
            else return false;
            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
