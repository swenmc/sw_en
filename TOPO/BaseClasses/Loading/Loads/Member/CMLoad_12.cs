using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CMLoad_12 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fF; // Force Value

        //----------------------------------------------------------------------------
        public float FF
        {
            get { return m_fF; }
            set { m_fF = value; }
        }
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_12(float fF)
        {
            FF = fF;
        }

        public CMLoad_12(float fF, int iMemberID)
        {
            FF = fF;

            // Dokoncit
            //IMemberCollection. add member ID
        }

        public CMLoad_12(float fF,
            CMember member_aux,
            EMLoadTypeDistr mLoadTypeDistr,
            EMLoadType mLoadType,
            EMLoadDirPCC1 eDirPPC,
            bool bIsDislayed,
            int fTime)
        {
            FF = fF;
            Member = member_aux;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            EDirPPC = eDirPPC;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            Model3DGroup model_gr = new Model3DGroup();

            ENLoadType nLoadType = TransformLoadTypefroMemberToPoint(EDirPPC, MLoadType);

            return model_gr = CreateM_3D_G_SimpleLoad(new Point3D(0.5f * Member.FLength, 0, 0), nLoadType, m_Color, FF, m_fOpacity, m_Material3DGraphics);
        }
    }
}
