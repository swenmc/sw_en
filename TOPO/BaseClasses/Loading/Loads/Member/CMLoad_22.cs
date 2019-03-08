using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CMLoad_22 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        private float m_fa; // Distance from Member Start along which load acts
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }
        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_22()
        {
        
        
        }
        public CMLoad_22(float fq, float fa)
        {
            Fq = fq;
            Fa = fa;
        }

        public CMLoad_22(int id_temp,
            float fq,
            float fa,
            CMember member_aux,
            EMLoadTypeDistr mLoadTypeDistr,
            ELoadType mLoadType,
            ELoadCoordSystem eLoadCS,
            ELoadDirection eLoadDir,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            Fa = fa;
            Member = member_aux;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            ELoadCS = eLoadCS;
            ELoadDir = eLoadDir;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load(bool bConsiderCrossSectionDimensions, float fDisplayin3D_ratio)
        {
            return CreateUniformLoadSequence(Fq, 0.0f, Fa, bConsiderCrossSectionDimensions, fDisplayin3D_ratio);
        }
    }
}
