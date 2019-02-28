using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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

        public CNLoadSingle(int iLoadID,
              CNode nNode,
              ENLoadType nLoadType,
              float fValue,
              bool bIsDislayed,
              int fTime)
        {
            ID = iLoadID;
            Node = nNode;
            NLoadType = nLoadType;
            Value = fValue;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;

            SetPrefix();
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            Model3DGroup model_gr = new Model3DGroup();
            return model_gr = CreateM_3D_G_SimpleLoad(new Point3D(Node.X, Node.Y, Node.Z), NLoadType, m_Color, Value, m_fOpacity, m_Material);
        }

        private void SetPrefix()
        {
            switch (NLoadType)
            {
                case ENLoadType.eNLT_Fx:
                    {
                       Prefix = "Fx";
                       break;
                    }
                case ENLoadType.eNLT_Fy:
                    {
                        Prefix = "Fy";
                        break;
                    }
                case ENLoadType.eNLT_Fz:
                    {
                        Prefix = "Fz";
                        break;
                    }
                case ENLoadType.eNLT_Mx:
                    {
                        Prefix = "Mx";
                        break;
                    }
                case ENLoadType.eNLT_My:
                    {
                        Prefix = "My";
                        break;
                    }
                case ENLoadType.eNLT_Mz:
                    {
                        Prefix = "Mz";
                        break;
                    }
                default:
                    Console.WriteLine("Not implemented nodal load.");
                    break;
            }
        }
    }
}
