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
        private ESLoadDirLCC1 m_eDirLCC;
        private ELoadCoordSystem m_eLoadCS;

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
        public ESLoadDirLCC1 EDirLCC
        {
            get { return m_eDirLCC; }
            set { m_eDirLCC = value; }
        }
        public ELoadCoordSystem ELoadCS
        {
            get { return m_eLoadCS; }
            set { m_eLoadCS = value; }
        }

        public float m_fOpacity;
        public Color m_Color = new Color(); // Default

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        public CSLoad_Free(ELoadCoordSystem eLoadCS_temp, bool bIsDisplayed, float fTime)
        {
            ELoadCS = eLoadCS_temp; // GCS or LCS surface load
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

           // Set Load Model "material" Color and Opacity - default
           m_Color = Colors.Cyan;
           m_Material3DGraphics = new DiffuseMaterial();
           m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
           m_Material3DGraphics.Brush.Opacity = 0.9f;
        }
    }
}
