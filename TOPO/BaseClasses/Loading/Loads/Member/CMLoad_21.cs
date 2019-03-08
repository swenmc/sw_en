using System.Windows.Media;
using System.Windows.Media.Media3D;

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
    }
}
