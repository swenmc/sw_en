using System;
using System.Windows.Media;

namespace BaseClasses
{
    [Serializable]
    abstract public class CSLoad_Free : CLoad
    {
        //----------------------------------------------------------------------------
        //private int [] m_iSurfaceCollection;
        private ESLoadTypeDistr m_sLoadTypeDistr; // Type of external force distribution
        private ESLoadType m_sLoadType; // Type of external force
        private ELoadDir m_eLoadDirection;
        private float m_fRotationX_deg;
        private float m_fRotationY_deg;
        private float m_fRotationZ_deg;

        //----------------------------------------------------------------------------
        /*public int[] ISurfaceCollection
        {
            get { return m_iSurfaceCollection; }
            set { m_iSurfaceCollection = value; }
        }*/
        public ESLoadTypeDistr SLoadTypeDistr
        {
            get { return m_sLoadTypeDistr; }
            set { m_sLoadTypeDistr = value; }
        }
        public ESLoadType SLoadType
        {
            get { return m_sLoadType; }
            set { m_sLoadType = value; }
        }
        public ELoadDir ELoadDirection
        {
            get { return m_eLoadDirection; }
            set { m_eLoadDirection = value; }
        }

        public float RotationX_deg
        {
            get { return m_fRotationX_deg; }
            set { m_fRotationX_deg = value; }
        }

        public float RotationY_deg
        {
            get { return m_fRotationY_deg; }
            set { m_fRotationY_deg = value; }
        }

        public float RotationZ_deg
        {
            get { return m_fRotationZ_deg; }
            set { m_fRotationZ_deg = value; }
        }

        public float m_fOpacity;
        public Color m_Color = new Color(); // Default

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public CSLoad_Free(ELoadCoordSystem eLoadCS_temp, ELoadDir eLoadDirection_temp, bool bIsDisplayed, float fTime)
        {
            Displayin3DRatio = 0.001f;
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDirection = eLoadDirection_temp;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }
    }
}
