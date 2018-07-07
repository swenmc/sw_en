using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CNLoadAll : CNLoad
    {
        //----------------------------------------------------------------------------
        private float m_Value_FX;
        private float m_Value_FY;
        private float m_Value_FZ;
        private float m_Value_MX;
        private float m_Value_MY;
        private float m_Value_MZ;

        //----------------------------------------------------------------------------
        public float Value_FX
        {
            get { return m_Value_FX; }
            set { m_Value_FX = value; }
        }
        public float Value_FY
        {
            get { return m_Value_FY; }
            set { m_Value_FY = value; }
        }
        public float Value_FZ
        {
            get { return m_Value_FZ; }
            set { m_Value_FZ = value; }
        }
        public float Value_MX
        {
            get { return m_Value_MX; }
            set { m_Value_MX = value; }
        }
        public float Value_MY
        {
            get { return m_Value_MY; }
            set { m_Value_MY = value; }
        }
        public float Value_MZ
        {
            get { return m_Value_MZ; }
            set { m_Value_MZ = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CNLoadAll()
        {

        }
        public CNLoadAll(CNode nNode,
              float fValue_FX,
              float fValue_FY,
              float fValue_FZ,
              float fValue_MX,
              float fValue_MY,
              float fValue_MZ,
              bool bIsDisplayed,
              int fTime
            )
        {
            Node = nNode;
            Value_FX = fValue_FX;
            Value_FY = fValue_FY;
            Value_FZ = fValue_FZ;
            Value_MX = fValue_MX;
            Value_MY = fValue_MY;
            Value_MZ = fValue_MZ;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            Model3DGroup model_gr = new Model3DGroup();
            // Not implemented
            return model_gr;
        }
    }
}
