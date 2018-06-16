using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    public class CMLoad_21 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_21()
        {
        
        
        }

        public CMLoad_21(float fqValue)
        {
            m_fq = fqValue;
        }

        public CMLoad_21(float fq,
            CMember member_aux,
            EMLoadTypeDistr mLoadTypeDistr,
            EMLoadType mLoadType,
            EMLoadDirPCC1 eDirPPC,
            bool bIsDislayed,
            int fTime)
        {
            Fq = fq;
            Member = member_aux;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            EDirPPC = eDirPPC;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;

            // Set Load Model "material" Color and Opacity - default
            m_Color = Color.FromRgb(100, 20, 20);
            m_Material.Brush = new SolidColorBrush(m_Color);
            m_Material.Brush.Opacity = m_fOpacity = 0.9f;
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            Model3DGroup model_gr = new Model3DGroup();

            ENLoadType nLoadType = TransformLoadTypefroMemberToPoint(EDirPPC, MLoadType);

            // Set Load Model "material" Color and Opacity - default
            m_Color = Color.FromRgb(200, 20, 20);
            m_Material.Brush = new SolidColorBrush(m_Color);
            m_Material.Brush.Opacity = m_fOpacity = 0.9f;

            float floadarrowsgapcount = 10.0f; // Number of gap arrows (10 gaps, 11 arrows) per member length (not int because we use it to calculate double coordinates)

            for (int i = 0; i <= floadarrowsgapcount; i++)
            {
                Model3DGroup model_temp = new Model3DGroup();
                model_temp = CreateM_3D_G_SimpleLoad(new Point3D(i / floadarrowsgapcount * Member.FLength, 0, 0), nLoadType, m_Color, Fq, m_fOpacity, m_Material); // Model of one Arrow
                model_gr.Children.Add(model_temp);
            }

            if (EDirPPC == EMLoadDirPCC1.eMLD_PCC_FYU_MZV || EDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
            {
                // Add connecting top (z = q) "line" in LCS x-direction
                Point3D pPoint;
                if (EDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
                {
                    pPoint = new Point3D(0, 0, -Fq);
                }
                else
                    pPoint = new Point3D(0, -Fq, 0);

                Cylinder cConnectLine = new Cylinder(0.005f * Math.Abs(Fq), Member.FLength, m_Material);
                model_gr.Children.Add(cConnectLine.CreateM_G_M_3D_Volume_Cylinder(pPoint, 0.005f * Math.Abs(Fq), Member.FLength, m_Material));
            }

            return model_gr; // Model group of whole member load in LCS
        }
    }
}
