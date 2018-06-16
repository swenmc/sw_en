using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using CENEX;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    [Serializable]
    public class CNLoadSingle : CNLoad
    {
        //----------------------------------------------------------------------------
        private float m_Value;
        private ENLoadType m_nLoadType;

        //----------------------------------------------------------------------------
        public float Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        public ENLoadType NLoadType
        {
            get { return m_nLoadType; }
            set { m_nLoadType = value; }
        }

        public float m_fOpacity;
        public Color m_Color = new Color(); // Default
        public DiffuseMaterial m_Material = new DiffuseMaterial();

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CNLoadSingle()
        {

        }

        public CNLoadSingle(CNode nNode,
              ENLoadType nLoadType,
              float fValue,
              bool bIsDislayed,
              int fTime)
        {
            Node = nNode;
            NLoadType = nLoadType;
            Value = fValue;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            Model3DGroup model_gr = new Model3DGroup();
            return model_gr = CreateM_3D_G_SimpleLoad(new Point3D(Node.X, Node.Y, Node.Z), NLoadType, m_Color, Value, m_fOpacity, m_Material);
        }
    }
}
