using _3DTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CSlabRebate : CVolume
    {
        private float m_RebateWidth;      // Distance from the slab edge
        private float m_RebatePosition;   // Roller door start position + half of trimmer width
        private float m_RebateLength;     // Roller door clear width
        private float m_RebateDepth_Step; // Step size (10 mm)
        private float m_RebateDepth_Edge; // Total depth at the edge (20 mm)

        private float m_RotationAboutZ_deg;
        private List<Point3D> MWireFramePoints;

        //-------------------------------------------------------------------------------------------------------------
        public float RebateWidth
        {
            get
            {
                return m_RebateWidth;
            }

            set
            {
                m_RebateWidth = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebatePosition
        {
            get
            {
                return m_RebatePosition;
            }

            set
            {
                m_RebatePosition = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateLength
        {
            get
            {
                return m_RebateLength;
            }

            set
            {
                m_RebateLength = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateDepth_Step
        {
            get
            {
                return m_RebateDepth_Step;
            }

            set
            {
                m_RebateDepth_Step = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateDepth_Edge
        {
            get
            {
                return m_RebateDepth_Edge;
            }

            set
            {
                m_RebateDepth_Edge = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
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

        //-------------------------------------------------------------------------------------------------------------
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

        public CSlabRebate(int id,
           float rebateWidth,
           float rebatePosition,
           float rebateLength,
           float rebateDepth_Step,
           float rebateDepth_Edge,
           float rotationAboiutZInDeg,
           Point3D pControlPoint,
           bool bIsDiplayed_temp,
           int fTime)
        {
            ID = id;
            m_RebateWidth = rebateWidth;
            m_RebatePosition = rebatePosition;
            m_RebateLength = rebateLength;
            m_RebateDepth_Step = rebateDepth_Step;
            m_RebateDepth_Edge = rebateDepth_Edge;
            m_RotationAboutZ_deg = rotationAboiutZInDeg;
            m_pControlPoint = pControlPoint;
            BIsDisplayed = bIsDiplayed_temp;
            FTime = fTime;
        }

        public CSlabRebate(int id,
           RebateProperties prop,
           float rotationAboiutZInDeg,
           Point3D pControlPoint,
           bool bIsDiplayed_temp,
           int fTime)
        {
            ID = id;
            m_RebateWidth = prop.RebateWidth;
            m_RebatePosition = prop.RebatePosition;
            m_RebateLength = prop.RebateLength;
            m_RebateDepth_Step = prop.RebateDepth_Step;
            m_RebateDepth_Edge = prop.RebateDepth_Edge;
            m_RotationAboutZ_deg = rotationAboiutZInDeg;
            m_pControlPoint = pControlPoint;
            BIsDisplayed = bIsDiplayed_temp;
            FTime = fTime;

            float fFictiveDepth = 0.001f; // Fiktivna hrubka rebate pre ucely vykreslenia
            // TODO - Upravit v ramci refaktoringu CVolume
            m_fDim1 = m_RebateLength;
            m_fDim2 = m_RebateWidth;
            m_fDim3 = fFictiveDepth; //m_RebateDepth_Edge;
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(Color color, float fOpacity)
        {
            m_volColor_2 = color;
            Visual_Object = CreateGeomModel3D(new SolidColorBrush(m_volColor_2), fOpacity);
            return Visual_Object;
        }

        public /*override*/ GeometryModel3D CreateGeomModel3D(SolidColorBrush brush, float fOpacity)
        {
            brush.Opacity = fOpacity; // Set brush opacity // TODO - mozeme nejako vylepsit a prepojit s GUI, aby to bol piamo parameter zadavany spolu s farbou rebate

            GeometryModel3D model = new GeometryModel3D();

            // TODO - pohrat sa s nastavenim farieb
            DiffuseMaterial qDiffTrans = new DiffuseMaterial(brush);
            SpecularMaterial qSpecTrans = new SpecularMaterial(brush, 40.0);

            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTrans);
            qOuterMaterial.Children.Add(qSpecTrans);

            // Create rebate - origin [0,0,0]
            CVolume volume = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, new Point3D(0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial, true, 0);
            model = volume.CreateM_3D_G_Volume_8Edges(new Point3D(0, 0, 0), m_fDim1, m_fDim2, m_fDim3, qOuterMaterial);

            // Set the Transform property of the GeometryModel to the Transform3DGroup
            // Nastavim vyslednu transformaciu
            model.Transform = GetRebateTransformGroup();

            // Naplnime pole bodov wireFrame
            // TODO - Ondrej - chcelo by to nejako elegantne zjednotit u vsetkych objektov ktore maju 3D geometriu kde a ako ziskavat wireframe
            // TODO Ondrej - tu chyba v tom ze beriem pozicie z povodneho zakladu nie z posunuteho do finalnej pozicie
            WireFramePoints = GetWireFramePoints_Volume_8Edges(model);

            Visual_Object = model;

            return model;
        }

        public Transform3DGroup GetRebateTransformGroup()
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

            // Presun Rebate do GCS z [0,0,0] do control point
            // Create and apply translation
            TranslateTransform3D myTranslateTransform3D_GCS = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            // Add the translation transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myTranslateTransform3D_GCS);

            return myTransform3DGroup;
        }
    }
}
