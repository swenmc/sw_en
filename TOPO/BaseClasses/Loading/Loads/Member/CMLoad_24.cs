using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CMLoad_24 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        private float m_faA;     // Distance of Load from Member Start / User Input (distance between load start point and member start)
        private float m_fa;      // Distance of Midpoint(Centre) of Load from Member Start 
        private float m_fs;      // Distance of Load / Length along which load acts
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }
        public float FaA
        {
            get { return m_faA; }
            set { m_faA = value; }
        }
        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }
        public float Fs
        {
            get { return m_fs; }
            set { m_fs = value; }
        }
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_24()
        {


        }

        public CMLoad_24(float fq, float faA, float fs)
        {
            Fq = fq;
            FaA = faA; // Distance between load start point and member start
            Fs = fs;
            // Calculate Load Centre Position from Start of Member
            m_fa = m_faA + m_fs / 2.0f;
        }

        public CMLoad_24(int id_temp,
            float fq,
            float faA,
            float fs,
            ELoadCoordSystem mLoadCoordSystem,
            EMLoadTypeDistr mLoadTypeDistr,
            EMLoadType mLoadType,
            EMLoadDirPCC1 eDirPPC,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            FaA = faA; // Distance between load start point and member start
            Fs = fs;
            // Calculate Load Centre Position from Start of Member
            m_fa = m_faA + m_fs / 2.0f;
            ELoadCS = mLoadCoordSystem;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            EDirPPC = eDirPPC;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public CMLoad_24(int id_temp,
            float fq,
            float faA,
            float fs,
            CMember member_aux,
            ELoadCoordSystem mLoadCoordSystem,
            EMLoadTypeDistr mLoadTypeDistr,
            EMLoadType mLoadType,
            EMLoadDirPCC1 eDirPPC,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            FaA = faA; // Distance between load start point and member start
            Fs = fs;
            // Calculate Load Centre Position from Start of Member
            m_fa = m_faA + m_fs / 2.0f;
            Member = member_aux;
            ELoadCS = mLoadCoordSystem;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            EDirPPC = eDirPPC;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load(bool bConsiderCrossSectionDimensions)
        {
            return CreateUniformLoadSequence(Fq, FaA, Fs, bConsiderCrossSectionDimensions);
        }
    }
}
