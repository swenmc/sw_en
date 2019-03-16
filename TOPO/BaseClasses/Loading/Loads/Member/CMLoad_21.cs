using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;

namespace BaseClasses
{
    public class CMLoad_21 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_21()
        {
        
        
        }

        public CMLoad_21(float fqValue)
        {
            m_fq = fqValue;
        }

        public CMLoad_21(int id_temp,
            float fq,
            EMLoadTypeDistr mLoadTypeDistr,
            ELoadType mLoadType,
            ELoadCoordSystem mLoadCoordSystem,
            ELoadDirection eLoadDir,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            ELoadCS = mLoadCoordSystem;
            ELoadDir = eLoadDir;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public CMLoad_21(int id_temp, 
            float fq,
            CMember member_aux,
            EMLoadTypeDistr mLoadTypeDistr,
            ELoadType mLoadType,
            ELoadCoordSystem mLoadCS,
            ELoadDirection eLoadDir,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            Member = member_aux;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            ELoadCS = mLoadCS;
            ELoadDir = eLoadDir;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load(bool bConsiderCrossSectionDimensions, float fDisplayIn3DRatio)
        {
            return CreateUniformLoadSequence(Fq, 0.0f, Member.FLength, bConsiderCrossSectionDimensions, fDisplayIn3DRatio);
        }

        // Uniform load per whole beam - simply supported beam
        // Docasne, hodnoty reakcii zavisia od typu podopretia pruta - rozpracovane v projkte FEM_CALC
        public override float Get_SSB_SupportReactionValue_RA_Start(float fL)
        {
            return 0.5f * Fq * fL;
        }
        public override float Get_SSB_SupportReactionValue_RB_End(float fL)
        {
            return 0.5f * Fq * fL;
        }
        public override float Get_SSB_V_max(float fL)
        {
            return Get_SSB_SupportReactionValue_RA_Start(fL);
        }
        public override float Get_SSB_M_max(float fL)
        {
            return 0.125f * Fq * fL * fL;
        }
        public override float Get_SSB_V_x(float fx, float fL)
        {
            return Fq * (0.5f * fL - fx);
        }
        public override float Get_SSB_M_x(float fx, float fL)
        {
            return 0.5f * Fq * fx * (fL - fx);
        }
        public override float Get_SSB_Delta_max(float fL, float fE, float fI)
        {
            return (5f * Fq * MathF.Pow4(fL)) / (384f * fE * fI);
        }
        public override float Get_SSB_Delta_x(float fx, float fL, float fE, float fI)
        {
            return ((Fq * fx) / (24f * fE * fI)) * (MathF.Pow3(fL) - 2f * fL * MathF.Pow2(fx) + MathF.Pow3(fx));
        }
    }
}
