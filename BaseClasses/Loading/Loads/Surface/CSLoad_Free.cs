using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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

        public float m_fOpacity;
        public Color m_Color = new Color(); // Default

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public CSLoad_Free(ELoadCoordSystem eLoadCS_temp, ELoadDir eLoadDirection_temp, bool bIsDisplayed, float fTime)
        {
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            ELoadDirection = eLoadDirection_temp;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }
    }
}
