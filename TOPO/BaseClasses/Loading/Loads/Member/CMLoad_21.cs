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
            ELoadCoordSystem mLoadCoordSystem,
            EMLoadTypeDistr mLoadTypeDistr,
            EMLoadType mLoadType,
            EMLoadDirPCC1 eDirPPC,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            ELoadCS = mLoadCoordSystem;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            EDirPPC = eDirPPC;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public CMLoad_21(int id_temp, 
            float fq,
            CMember member_aux,
            ELoadCoordSystem mLoadCoordinateSystem,
            EMLoadTypeDistr mLoadTypeDistr,
            EMLoadType mLoadType,
            EMLoadDirPCC1 eDirPPC,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            Member = member_aux;
            ELoadCS = mLoadCoordinateSystem;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            EDirPPC = eDirPPC;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load(bool bConsiderCrossSectionDimensions)
        {
            return CreateUniformLoadSequence(Fq, 0.0f, Member.FLength, bConsiderCrossSectionDimensions);
        }
    }
}
