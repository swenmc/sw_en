using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    // Class CSlab
    [Serializable]
    public class CSlab : CVolume
    {
        private float m_Eccentricity_x;
        private float m_Eccentricity_y;
        private float m_RotationAboutZ_deg;

        private CReinforcementBar m_Reference_Top_Bar_x;
        private CReinforcementBar m_Reference_Top_Bar_y;
        private CReinforcementBar m_Reference_Bottom_Bar_x;
        private CReinforcementBar m_Reference_Bottom_Bar_y;

        private List<CReinforcementBar> m_Top_Bars_x;
        private List<CReinforcementBar> m_Top_Bars_y;
        private List<CReinforcementBar> m_Bottom_Bars_x;
        private List<CReinforcementBar> m_Bottom_Bars_y;

        private int m_Count_Top_Bars_x;
        private int m_Count_Top_Bars_y;
        private int m_Count_Bottom_Bars_x;
        private int m_Count_Bottom_Bars_y;

        private float m_fDistanceOfBars_Top_x_SpacingInyDirection;
        private float m_fDistanceOfBars_Top_y_SpacingInxDirection;
        private float m_fDistanceOfBars_Bottom_x_SpacingInyDirection;
        private float m_fDistanceOfBars_Bottom_y_SpacingInxDirection;

        private float m_fConcreteCover;

        private List<Point3D> MWireFramePoints;

        private CJointDesignDetails m_DesignDetails;
        public CJointDesignDetails DesignDetails
        {
            get { return m_DesignDetails; }
            set { m_DesignDetails = value; }
        }

        public float Eccentricity_x
        {
            get
            {
                return m_Eccentricity_x;
            }

            set
            {
                m_Eccentricity_x = value;
            }
        }

        public float Eccentricity_y
        {
            get
            {
                return m_Eccentricity_y;
            }

            set
            {
                m_Eccentricity_y = value;
            }
        }

        public float RotationAboutZ_deg
        {
            get
            {
                return m_RotationAboutZ_deg;
            }

            set
            {
                m_RotationAboutZ_deg = value;
            }
        }

        public CReinforcementBar Reference_Top_Bar_x
        {
            get
            {
                return m_Reference_Top_Bar_x;
            }

            set
            {
                m_Reference_Top_Bar_x = value;
            }
        }

        public CReinforcementBar Reference_Top_Bar_y
        {
            get
            {
                return m_Reference_Top_Bar_y;
            }

            set
            {
                m_Reference_Top_Bar_y = value;
            }
        }

        public CReinforcementBar Reference_Bottom_Bar_x
        {
            get
            {
                return m_Reference_Bottom_Bar_x;
            }

            set
            {
                m_Reference_Bottom_Bar_x = value;
            }
        }

        public CReinforcementBar Reference_Bottom_Bar_y
        {
            get
            {
                return m_Reference_Bottom_Bar_y;
            }

            set
            {
                m_Reference_Bottom_Bar_y = value;
            }
        }

        public List<CReinforcementBar> Top_Bars_x
        {
            get
            {
                return m_Top_Bars_x;
            }

            set
            {
                m_Top_Bars_x = value;
            }
        }

        public List<CReinforcementBar> Top_Bars_y
        {
            get
            {
                return m_Top_Bars_y;
            }

            set
            {
                m_Top_Bars_y = value;
            }
        }

        public List<CReinforcementBar> Bottom_Bars_x
        {
            get
            {
                return m_Bottom_Bars_x;
            }

            set
            {
                m_Bottom_Bars_x = value;
            }
        }

        public List<CReinforcementBar> Bottom_Bars_y
        {
            get
            {
                return m_Bottom_Bars_y;
            }

            set
            {
                m_Bottom_Bars_y = value;
            }
        }

        public int Count_Top_Bars_x
        {
            get
            {
                return m_Count_Top_Bars_x;
            }

            set
            {
                m_Count_Top_Bars_x = value;
            }
        }

        public int Count_Top_Bars_y
        {
            get
            {
                return m_Count_Top_Bars_y;
            }

            set
            {
                m_Count_Top_Bars_y = value;
            }
        }

        public int Count_Bottom_Bars_x
        {
            get
            {
                return m_Count_Bottom_Bars_x;
            }

            set
            {
                m_Count_Bottom_Bars_x = value;
            }
        }

        public int Count_Bottom_Bars_y
        {
            get
            {
                return m_Count_Bottom_Bars_y;
            }

            set
            {
                m_Count_Bottom_Bars_y = value;
            }
        }

        public float DistanceOfBars_Top_x_SpacingInyDirection
        {
            get
            {
                return m_fDistanceOfBars_Top_x_SpacingInyDirection;
            }

            set
            {
                m_fDistanceOfBars_Top_x_SpacingInyDirection = value;
            }
        }

        public float DistanceOfBars_Top_y_SpacingInxDirection
        {
            get
            {
                return m_fDistanceOfBars_Top_y_SpacingInxDirection;
            }

            set
            {
                m_fDistanceOfBars_Top_y_SpacingInxDirection = value;
            }
        }

        public float DistanceOfBars_Bottom_x_SpacingInyDirection
        {
            get
            {
                return m_fDistanceOfBars_Bottom_x_SpacingInyDirection;
            }

            set
            {
                m_fDistanceOfBars_Bottom_x_SpacingInyDirection = value;
            }
        }

        public float DistanceOfBars_Bottom_y_SpacingInxDirection
        {
            get
            {
                return m_fDistanceOfBars_Bottom_y_SpacingInxDirection;
            }

            set
            {
                m_fDistanceOfBars_Bottom_y_SpacingInxDirection = value;
            }
        }

        public float ConcreteCover
        {
            get
            {
                return m_fConcreteCover;
            }

            set
            {
                m_fConcreteCover = value;
            }
        }

        public List<Point3D> WireFramePoints
        {
            get
            {
                if (MWireFramePoints == null) MWireFramePoints = new List<Point3D>();
                return MWireFramePoints;
            }

            set
            {
                MWireFramePoints = value;
            }
        }

        public CSlab()
        {
        }

        // Rectangular
        public CSlab(int iSlab_ID,
            CPoint pControlEdgePoint,
            MATERIAL.CMat_02_00 materialConcrete,
            float fX,
            float fY,
            float fZ,
            float ex,
            float ey,
            float rotationAboiutZInDeg,
            //float fConcreteCover,
            //CReinforcementBar refTopBar_x,
            //CReinforcementBar refTopBar_y,
            //CReinforcementBar refBottomBar_x,
            //CReinforcementBar refBottomBar_y,
            //int iNumberOfBarsTop_x,
            //int iNumberOfBarsTop_y,
            //int iNumberOfBarsBottom_x,
            //int iNumberOfBarsBottom_y,
            Color volColor,
            float fvolOpacity,
            bool bIsDisplayed,
            float fTime)
        {
            ID = iSlab_ID;
            m_pControlPoint = pControlEdgePoint;
            m_Mat = materialConcrete;
            m_fDim1 = fX;
            m_fDim2 = fY;
            m_fDim3 = fZ;
            m_Eccentricity_x = ex;
            m_Eccentricity_y = ey;
            //m_RotationAboutZ_deg = rotationAboiutZInDeg;
            //m_fConcreteCover = fConcreteCover;
            //m_Reference_Top_Bar_x = refTopBar_x;
            //m_Reference_Top_Bar_y = refTopBar_y;
            //m_Reference_Bottom_Bar_x = refBottomBar_x;
            //m_Reference_Bottom_Bar_y = refBottomBar_y;
            //m_Count_Top_Bars_x = iNumberOfBarsTop_x;
            //m_Count_Top_Bars_y = iNumberOfBarsTop_y;
            //m_Count_Bottom_Bars_x = iNumberOfBarsBottom_x;
            //m_Count_Bottom_Bars_y = iNumberOfBarsBottom_y;
            m_volColor_2 = volColor;
            m_fvolOpacity = fvolOpacity;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(float fOpacity)
        {
            Visual_Object = CreateGeomModel3D(new SolidColorBrush(m_volColor_2), fOpacity);
            return Visual_Object;
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(SolidColorBrush brush, float fOpacity)
        {
            brush.Opacity = fOpacity; // Set brush opacity // TODO - mozeme nejako vypesit a prepojit s GUI, aby to bol piamo parameter zadavany spolu s farbou zakladu

            GeometryModel3D model = new GeometryModel3D();

            // TODO - pohrat sa s nastavenim farieb
            DiffuseMaterial qDiffTrans = new DiffuseMaterial(brush);
            SpecularMaterial qSpecTrans = new SpecularMaterial(brush/*new SolidColorBrush(Color.FromArgb(210, 210, 210, 210))*/, 90.0);

            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTrans);
            qOuterMaterial.Children.Add(qSpecTrans);

            // Create slab - origin [0,0,0]
            CVolume volume = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, new CPoint(1, 0, 0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial, true, 0);
            model = volume.CreateM_3D_G_Volume_8Edges(new Point3D(0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial);

            // Set the Transform property of the GeometryModel to the Transform3DGroup
            // Nastavim vyslednu transformaciu
            model.Transform = GetSlabTransformGroup();

            // Naplnime pole bodov wireFrame
            // TODO - Ondrej - chcelo by to nejako elegantne zjednotit u vsetkych objektov ktore maju 3D geometriu kde a ako ziskavat wireframe
            // TODO Ondrej - tu chyba v tom ze beriem pozicie z povodneho zakladu nie z posunuteho do finalnej pozicie
            WireFramePoints = GetWireFramePoints_Volume_8Edges(model);

            Visual_Object = model;

            return model;
        }

        public Transform3DGroup GetSlabTransformGroup()
        {
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            // Rotate about Z axis - otocime patku okolo [0,0,0]
            // Create and apply a transformation that rotates the object.
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = new Vector3D(0, 0, 1);
            myAxisAngleRotation3d.Angle = RotationAboutZ_deg;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;

            // Add the rotation transform to a Transform3DGroup
            myTransform3DGroup.Children.Add(myRotateTransform3D);

            // Presun celej dosky do GCS z [0,0,0] do control point
            // Create and apply translation
            TranslateTransform3D myTranslateTransform3D_GCS = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            // Add the translation transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myTranslateTransform3D_GCS);

            return myTransform3DGroup;
        }
    }
}
