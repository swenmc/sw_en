using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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

        public CMLoad_21(int id_temp, 
            float fq,
            CMember member_aux,
            EMLoadTypeDistr mLoadTypeDistr,
            EMLoadType mLoadType,
            EMLoadDirPCC1 eDirPPC,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            Member = member_aux;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            EDirPPC = eDirPPC;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;

            // Set Load Model "material" Color and Opacity - default
            m_Color = Colors.Cyan;
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_Material3DGraphics.Brush.Opacity = m_fOpacity = 0.9f;
        }

        public override Model3DGroup CreateM_3D_G_Load()
        {
            float fDisplayin3D_ratio = 0.001f; // (1 kN = 1 m, fValue is in [N] so for 1000 N = 1 m, display ratio = 1/1000)

            Model3DGroup model_gr = new Model3DGroup();

            ENLoadType nLoadType = TransformLoadTypefroMemberToPoint(EDirPPC, MLoadType);

            // Set Load Model "material" Color and Opacity - default
            m_Color = Colors.Cyan;
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_Material3DGraphics.Brush.Opacity = m_fOpacity = 0.9f;

            float floadarrowsgapcount = 10.0f; // Number of gap arrows (10 gaps, 11 arrows) per member length (not int because we use it to calculate double coordinates)

            for (int i = 0; i <= floadarrowsgapcount; i++)
            {
                Model3DGroup model_temp = new Model3DGroup();
                model_temp = CreateM_3D_G_SimpleLoad(new Point3D(i / floadarrowsgapcount * Member.FLength, 0, 0), nLoadType, m_Color, Fq, m_fOpacity, m_Material3DGraphics, fDisplayin3D_ratio); // Model of one Arrow
                model_gr.Children.Add(model_temp);
            }

            if (EDirPPC == EMLoadDirPCC1.eMLD_PCC_FYU_MZV || EDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
            {
                // Add connecting top (z = q) "line" in LCS x-direction
                Point3D pPoint;
                if (EDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
                {
                    pPoint = new Point3D(0, 0, -Fq * fDisplayin3D_ratio);
                }
                else
                    pPoint = new Point3D(0, -Fq * fDisplayin3D_ratio, 0);

                Cylinder cConnectLine = new Cylinder(0.005f * Math.Abs(Fq * fDisplayin3D_ratio), Member.FLength, m_Material3DGraphics);
                model_gr.Children.Add(cConnectLine.CreateM_G_M_3D_Volume_Cylinder(pPoint, 0.005f * Math.Abs(Fq * fDisplayin3D_ratio), Member.FLength, m_Material3DGraphics));
            }

            // Trasnform position of load on member (consider eccentricity of load / member / cross-section dimensions)
            TranslateTransform3D translate = new TranslateTransform3D(0, 0, 0.5 * Member.CrScStart.h);

            // Add the transform to a Transform3DGroup
            Transform3DGroup loadTransform3DGroup = new Transform3DGroup();
            loadTransform3DGroup.Children.Add(translate);

            // Set the Transform property of the GeometryModel to the Transform3DGroup 
            model_gr.Transform = loadTransform3DGroup;

            return model_gr; // Model group of whole member load in LCS
        }
    }
}
