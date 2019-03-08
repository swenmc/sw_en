using BaseClasses.GraphObj.Objects_3D;
using System;
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
        private ELoadType m_mLoadType; // Type of external force
        private CMember m_member;
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
        public ELoadType MLoadType
        {
            get { return m_mLoadType; }
            set { m_mLoadType = value; }
        }

        public CMember Member
        {
            get { return m_member; }
            set { m_member = value; }
        }

        //public CMember Member = new CMember(); // Temporary   ked je to temporary,tak som to zakomentoval
        public float m_fOpacity;
        public Color m_Color = new Color(); // Default

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------


        public CMLoad()
        {
            // Set Load Model "material" Color and Opacity - default
            m_Color = Colors.Cyan;
            m_Material3DGraphics = new DiffuseMaterial();
            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_Material3DGraphics.Brush.Opacity = 0.9f;
        }

        public ENLoadType TransformLoadTypefroMemberToPoint(ELoadDirection eLoadDirection, ELoadType eMLoadType)
        {
            ENLoadType nLoadType = ENLoadType.eNLT_OTHER; // Auxliary

            if (eMLoadType == ELoadType.eLT_F) // Force
            {
                if (eLoadDirection == ELoadDirection.eLD_X)
                {
                    nLoadType = ENLoadType.eNLT_Fx;
                }
                else if (eLoadDirection == ELoadDirection.eLD_Y)
                {
                    nLoadType = ENLoadType.eNLT_Fy;
                }
                else if (eLoadDirection == ELoadDirection.eLD_Z)
                {
                    nLoadType = ENLoadType.eNLT_Fz;
                }
            }
            else if (eMLoadType == ELoadType.eLT_M) // Moment
            {
                if (eLoadDirection == ELoadDirection.eLD_X)
                {
                    nLoadType = ENLoadType.eNLT_Mx;
                }
                else if (eLoadDirection == ELoadDirection.eLD_Y)
                {
                    nLoadType = ENLoadType.eNLT_Mz;
                }
                else if (eLoadDirection == ELoadDirection.eLD_Z)
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

        public Model3DGroup CreateUniformLoadSequence(float fq_value, float fstartPosition, float floadsequencelength, bool bConsiderCrossSectionDimensions)
        {
            float fDisplayin3D_ratio = 0.001f; // (1 kN = 1 m, fValue is in [N] so for 1000 N = 1 m, display ratio = 1/1000)

            Model3DGroup model_gr = new Model3DGroup();

            ENLoadType nLoadType = TransformLoadTypefroMemberToPoint(ELoadDir, MLoadType);

            // Set Load Model "material" Color and Opacity - default
            if (fq_value >= 0)
                m_Color = Colors.IndianRed;
            else
                m_Color = Colors.DeepSkyBlue;

            m_Material3DGraphics.Brush = new SolidColorBrush(m_Color);
            m_Material3DGraphics.Brush.Opacity = m_fOpacity = 0.9f;

            int floadarrowsgapcount = 10; // Number of gap arrows (10 gaps, 11 arrows) per member length (cast/convert int to float because we use it to calculate double coordinates)

            for (int i = 0; i <= floadarrowsgapcount; i++)
            {
                Model3DGroup model_temp = new Model3DGroup();
                model_temp = CreateM_3D_G_SimpleLoad(new Point3D(i / (float)floadarrowsgapcount * floadsequencelength, 0, 0), nLoadType, m_Color, fq_value, m_fOpacity, m_Material3DGraphics, fDisplayin3D_ratio); // Model of one Arrow
                model_gr.Children.Add(model_temp);
            }

            if (ELoadDir == ELoadDirection.eLD_Y || ELoadDir == ELoadDirection.eLD_Z)
            {
                // Add connecting top (z = q) "line" in LCS x-direction
                Point3D pPoint;
                if (ELoadDir == ELoadDirection.eLD_Z)
                {
                    pPoint = new Point3D(0, 0, -fq_value * fDisplayin3D_ratio);
                }
                else
                    pPoint = new Point3D(0, -fq_value * fDisplayin3D_ratio, 0);

                Cylinder cConnectLine = new Cylinder(0.005f * Math.Abs(fq_value * fDisplayin3D_ratio), floadsequencelength, m_Material3DGraphics);
                model_gr.Children.Add(cConnectLine.CreateM_G_M_3D_Volume_Cylinder(pPoint, 0.005f * Math.Abs(fq_value * fDisplayin3D_ratio), floadsequencelength, m_Material3DGraphics));
            }

            // Trasnform position of load on member (consider eccentricity of load / member / cross-section dimensions)

            double dOffset_y = 0f;
            double dOffset_z = 0f;

            if(bConsiderCrossSectionDimensions)
            {
                dOffset_y = ELoadDir == ELoadDirection.eLD_Y ? (fq_value > 0 ? -0.5 * Member.CrScStart.b : 0.5 * Member.CrScStart.b) : 0;
                dOffset_z = ELoadDir == ELoadDirection.eLD_Z ? (fq_value > 0 ? -0.5 * Member.CrScStart.h : 0.5 * Member.CrScStart.h) : 0;
            }

            TranslateTransform3D translate = new TranslateTransform3D(fstartPosition, dOffset_y, dOffset_z);

            // Add the transform to a Transform3DGroup
            Transform3DGroup loadTransform3DGroup = new Transform3DGroup();
            loadTransform3DGroup.Children.Add(translate);

            // Set the Transform property of the GeometryModel to the Transform3DGroup 
            model_gr.Transform = loadTransform3DGroup;

            return model_gr; // Model group of whole member load in LCS
        }
    }
}
