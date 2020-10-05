using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CConnectorGroup
    {
        private Point3D m_ControlPoint;
        private Vector3D m_RotationVector;
        private List<CConnector> m_Connectors;

        public List<CConnector> Connectors
        {
            get
            {
                return m_Connectors;
            }

            set
            {
                m_Connectors = value;
            }
        }

        public Point3D ControlPoint
        {
            get
            {
                return m_ControlPoint;
            }

            set
            {
                m_ControlPoint = value;
            }
        }

        public Vector3D RotationVector
        {
            get
            {
                return m_RotationVector;
            }

            set
            {
                m_RotationVector = value;
            }
        }

        public CConnectorGroup()
        {
            m_Connectors = new List<CConnector>();
        }
        public CConnectorGroup(CConnector[] connectors)
        {
            m_Connectors = new List<CConnector>(connectors);
        }

        public Transform3DGroup CreateTransformCoordGroup()
        {
            // Rotate Plate from its cs to joint cs system in LCS of member or GCS
            RotateTransform3D RotateTrans3D_AUX_X = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Y = new RotateTransform3D();
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_X.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), RotationVector.X); // Rotation in degrees
            RotateTrans3D_AUX_Y.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotationVector.Y); // Rotation in degrees
            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), RotationVector.Z); // Rotation in degrees

            // Move 0,0,0 to control point in LCS of member or GCS
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_X);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Y);
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);
            return Trans3DGroup;
        }


        public void Deactivate()
        {
            foreach (CConnector connector in Connectors)
            {
                connector.BIsGenerated = false;
                connector.BIsDisplayed = false;
                connector.BIsSelectedForDesign = false;
                connector.BIsSelectedForMaterialList = false;
            }
        }

    }
}