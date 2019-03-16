using System;
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
            ELoadType mLoadType,
            ELoadCoordSystem eLoadCS,
            ELoadDirection eLoadDir,
            bool bIsDislayed,
            int fTime)
        {
            FF = fF;
            Member = member_aux;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            ELoadCS = eLoadCS;
            ELoadDir = eLoadDir;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load(float displayIn3DRatio)
        {
            Model3DGroup model_gr = new Model3DGroup();

            ENLoadType nLoadType = TransformLoadTypefroMemberToPoint(ELoadDir, MLoadType);

            return model_gr = CreateM_3D_G_SimpleLoad(new Point3D(0.5f * Member.FLength, 0, 0), nLoadType, m_Color, FF, m_fOpacity, m_Material3DGraphics, displayIn3DRatio);
        }

        // Simply supported beam
        // Docasne, hodnoty reakcii zavisia od typu podopretia pruta - rozpracovane v projkte FEM_CALC
        public override float Get_SSB_SupportReactionValue_RA_Start(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_SupportReactionValue_RB_End(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_V_max(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_M_max(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_N_x(float fx)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_V_x(float fx, float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_M_x(float fx, float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_Delta_max(float fL, float fE, float fI)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_Delta_x(float fx, float fL, float fE, float fI)
        {
            throw new NotImplementedException();
        }
    }
}
