using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    public class CConnector : CConnectionComponentEntity3D
    {
        public int m_iGauge;
        public float m_fLength;
        public float m_fDiameter;
        public float m_fWeight;
        public float m_iNumberOfThreads;
        public DiffuseMaterial m_DiffuseMat;
        public Cylinder m_cylinder;

        public float m_fRotationX_deg, m_fRotationY_deg, m_fRotationZ_deg;

        public CConnector()
        {
            BIsDisplayed = true;
            m_pControlPoint = new CPoint(0, 0, 0, 0, 0);
            m_DiffuseMat = new DiffuseMaterial();
            m_cylinder = new Cylinder();
        }

        public CConnector(bool bIsDisplayed)
        {
            BIsDisplayed = bIsDisplayed;
            m_pControlPoint = new CPoint(0, 0, 0, 0, 0);
            m_DiffuseMat = new DiffuseMaterial();
            m_cylinder = new Cylinder();
        }

        public CConnector(string sName_temp, CPoint controlpoint, int iGauge_temp, float fDiameter_temp, float fLength_temp, float fWeight_temp, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            Name = sName_temp;
            m_Mat = new CMat(); // Todo - material ako parameter
            m_Mat.Name = "Class 3 / 4 / B8";
            m_pControlPoint = controlpoint;
            BIsDisplayed = bIsDisplayed;
            m_fLength = fLength_temp;
            m_iGauge = iGauge_temp;
            m_fDiameter = fDiameter_temp;
            m_fWeight = fWeight_temp;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            m_cylinder = new Cylinder(0.5f * m_fDiameter, m_fLength, m_DiffuseMat);
        }

        protected override void loadIndices()
        { }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D ssl3D = new ScreenSpaceLines3D();
            return ssl3D;
        }

        protected override Point3DCollection GetDefinitionPoints()
        {
            Point3DCollection pMeshPositions = new Point3DCollection();

            foreach (Point3D point in arrPoints3D)
                pMeshPositions.Add(point);

            return pMeshPositions;
        }

        public void TransformCoord(GeometryModel3D model)
        {
            model.Transform = CreateTransformCoordGroup();
        }

        public void TransformCoord(ScreenSpaceLines3D wireframeModel)
        {
            wireframeModel.Transform = CreateTransformCoordGroup();
        }

        public Transform3DGroup CreateTransformCoordGroup()
        {
            // Rotate Plate from its cs to joint cs system in LCS of member or GCS
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), m_fRotationX_deg); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), m_fRotationY_deg); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), m_fRotationZ_deg); // Rotation in degrees

            // Move 0,0,0 to control point in LCS of member or GCS
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_X);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);
            return Trans3DGroup;
        }

        public override GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            m_DiffuseMat = new DiffuseMaterial(brush);
            GeometryModel3D geometryModel = new GeometryModel3D();
            geometryModel = m_cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z), 0.5f * m_fDiameter, m_fLength, m_DiffuseMat);

            TransformCoord(geometryModel);

            return geometryModel;
        }
    }
}