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
    abstract public class CMLoad : CLoad
    {
        //----------------------------------------------------------------------------
        private int[] m_iMemberCollection; // List / Collection of members IDs where load is defined
        private EMLoadTypeDistr m_mLoadTypeDistr; // Type of external force distribution
        private EMLoadType m_mLoadType; // Type of external force
        private EMLoadDirPCC1 m_eDirPPC; // External Force Direction in Principal Coordinate System of Member
        //----------------------------------------------------------------------------
        public int[] IMemberCollection
        {
            get { return m_iMemberCollection; }
            set { m_iMemberCollection = value; }
        }
        public EMLoadTypeDistr MLoadTypeDistr
        {
            get { return m_mLoadTypeDistr; }
            set { m_mLoadTypeDistr = value; }
        }
        public EMLoadType MLoadType
        {
            get { return m_mLoadType; }
            set { m_mLoadType = value; }
        }
        public EMLoadDirPCC1 EDirPPC
        {
            get { return m_eDirPPC; }
            set { m_eDirPPC = value; }
        }

        public CMember Member = new CMember(); // Temporary
        public float m_fOpacity;
        public Color m_Color = new Color(); // Default
        public DiffuseMaterial m_Material = new DiffuseMaterial();

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------


        public CMLoad()
        {
            // Set Load Model "material" Color and Opacity - default
            m_Color = Colors.Cyan;
            m_Material.Brush = new SolidColorBrush(m_Color);
            m_Material.Brush.Opacity = 0.9f;
        }

        public ENLoadType TransformLoadTypefroMemberToPoint(EMLoadDirPCC1 eDirPPC, EMLoadType eMLoadType)
        {
            ENLoadType nLoadType = ENLoadType.eNLT_OTHER; // Auxliary

            if (eMLoadType == EMLoadType.eMLT_F) // Force
            {
                if (eDirPPC == EMLoadDirPCC1.eMLD_PCC_FXX_MXX)
                {
                    nLoadType = ENLoadType.eNLT_Fx;
                }
                else if (eDirPPC == EMLoadDirPCC1.eMLD_PCC_FYU_MZV)
                {
                    nLoadType = ENLoadType.eNLT_Fy;
                }
                else if (eDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
                {
                    nLoadType = ENLoadType.eNLT_Fz;
                }
            }
            else if (eMLoadType == EMLoadType.eMLT_M) // Moment
            {
                if (eDirPPC == EMLoadDirPCC1.eMLD_PCC_FXX_MXX)
                {
                    nLoadType = ENLoadType.eNLT_Mx;
                }
                else if (eDirPPC == EMLoadDirPCC1.eMLD_PCC_FYU_MZV)
                {
                    nLoadType = ENLoadType.eNLT_Mz;
                }
                else if (eDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
                {
                    nLoadType = ENLoadType.eNLT_My;
                }
            }
            else
            {
                nLoadType = ENLoadType.eNLT_OTHER;
            } //Temperature

            return nLoadType;
        }
    }
}
