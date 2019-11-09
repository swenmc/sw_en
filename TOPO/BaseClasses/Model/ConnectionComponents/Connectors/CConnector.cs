using _3DTools;
using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using MATERIAL;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CConnector : CConnectionComponentEntity3D
    {
        private float m_fDiameter_shank;
        public float Diameter_shank
        {
            get
            {
                return m_fDiameter_shank;
            }

            set
            {
                m_fDiameter_shank = value;
            }
        }

        private float m_fDiameter_thread;
        public float Diameter_thread // d_f - nominal (screw) diameter
        {
            get
            {
                return m_fDiameter_thread;
            }

            set
            {
                m_fDiameter_thread = value;
            }
        }

        private float m_fArea_c_thread;
        public float Area_c_thread  // Core / thread area
        {
            get
            {
                return m_fArea_c_thread;
            }

            set
            {
                m_fArea_c_thread = value;
            }
        }

        private float m_fArea_o_shank;
        public float Area_o_shank // Shank area
        {
            get
            {
                return m_fArea_o_shank;
            }

            set
            {
                m_fArea_o_shank = value;
            }
        }

        private float m_fLength;
        public float Length
        {
            get
            {
                return m_fLength;
            }

            set
            {
                m_fLength = value;
            }
        }

        private float m_fMass;
        public float Mass
        {
            get
            {
                return m_fMass;
            }

            set
            {
                m_fMass = value;
            }
        }

        public float m_iNumberOfThreads;
        [NonSerialized]
        public DiffuseMaterial m_DiffuseMat;
        //[NonSerialized]
        //public Cylinder m_cylinder;
        [NonSerialized]
        public GeometryModel3D Visual_Connector;

        public float m_fRotationX_deg, m_fRotationY_deg, m_fRotationZ_deg;

        public CConnector()
        {
            BIsDisplayed = true;
            m_pControlPoint = new Point3D(0, 0, 0);
            m_DiffuseMat = new DiffuseMaterial();
            //m_cylinder = new Cylinder();
        }

        public CConnector(bool bIsDisplayed)
        {
            BIsDisplayed = bIsDisplayed;
            m_pControlPoint = new Point3D(0, 0, 0);
            m_DiffuseMat = new DiffuseMaterial();
            //m_cylinder = new Cylinder();
        }

        public CConnector(string sName_temp, Point3D controlpoint, float fDiameter_thread_temp, float fLength_temp, float fMass_temp, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            Name = sName_temp;
            m_Mat.Name = "Class 3 / 4 / B8";
            m_pControlPoint = controlpoint;
            BIsDisplayed = bIsDisplayed;
            Length = fLength_temp;
            Diameter_thread = fDiameter_thread_temp;
            Mass = fMass_temp;

            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            m_DiffuseMat = new DiffuseMaterial(Brushes.Azure);
            //m_cylinder = new Cylinder(0.5f * Diameter_thread, m_fLength, m_DiffuseMat);
        }

        protected override void loadIndices()
        { }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D ssl3D = new ScreenSpaceLines3D();

            GeometryModel3D geometryModel = new GeometryModel3D();
            geometryModel = Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z), 13, 0.5f * Diameter_thread, m_fLength, m_DiffuseMat);

            Int32Collection wireFrameIndices = Cylinder.GetWireFrameIndices_Cylinder(13);

            // TODO Ondrej 15/07/2018
            // TODO Dopracovat pristup k bodom v geometry model a pridat ich do ssl3D
            Point3DCollection positions = ((MeshGeometry3D)geometryModel.Geometry).Positions;
            for (int i = 0; i < wireFrameIndices.Count; i++)
            {
                ssl3D.Points.Add(positions[wireFrameIndices[i]]);
            }
            return ssl3D;
        }

        public Point3DCollection WireFrameModelPoints()
        {
            Point3DCollection points3D = new Point3DCollection();

            GeometryModel3D geometryModel = new GeometryModel3D();
            geometryModel = Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z), 13, 0.5f * Diameter_thread, m_fLength, m_DiffuseMat);

            Int32Collection wireFrameIndices = Cylinder.GetWireFrameIndices_Cylinder(13);

            // TODO Ondrej 15/07/2018
            // TODO Dopracovat pristup k bodom v geometry model a pridat ich do points3D
            Point3DCollection positions = ((MeshGeometry3D)geometryModel.Geometry).Positions;
            for (int i = 0; i < wireFrameIndices.Count; i++)
            {
                points3D.Add(positions[wireFrameIndices[i]]);
            }

            return points3D;
        }
        public Point3DCollection WireFrameModelPointsFromVisual()
        {
            Point3DCollection points3D = new Point3DCollection();
            if (Visual_Connector == null) return points3D;
            if (Visual_Connector.Geometry == null) return points3D;

            Int32Collection wireFrameIndices = Cylinder.GetWireFrameIndices_Cylinder(13);

            // TODO Ondrej 15/07/2018
            // TODO Dopracovat pristup k bodom v geometry model a pridat ich do points3D
            Point3DCollection positions = ((MeshGeometry3D)Visual_Connector.Geometry).Positions;
            for (int i = 0; i < wireFrameIndices.Count; i++)
            {
                points3D.Add(positions[wireFrameIndices[i]]);
            }

            return points3D;
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
            // Rotate connector from its cs to joint cs system in LCS of member or GCS
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), m_fRotationX_deg); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), m_fRotationY_deg); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), m_fRotationZ_deg); // Rotation in degrees

            // Move 0,0,0 to the control point in LCS of member or GCS
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
            geometryModel = Cylinder.CreateM_G_M_3D_Volume_Cylinder(new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z), 13, 0.5f * Diameter_thread, m_fLength, m_DiffuseMat);

            TransformCoord(geometryModel);

            return geometryModel;
        }

        public override void loadWireFrameIndices()
        { }
    }
}