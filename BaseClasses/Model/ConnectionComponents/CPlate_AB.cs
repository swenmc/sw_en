using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;

namespace BaseClasses
{
    public class CConCom_Plate_AB : CPlate
    {
        float m_fb_1;
        float m_fh_1;
        float m_fb_2;
        float m_fh_2;
        float m_ft;
        float m_fSlope_rad;
        public float m_fRotationZ;

        public CConCom_Plate_AB()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_AB(GraphObj.CPoint controlpoint, float fb_1_temp, float fh_1_temp, float fb_2_temp, float fh_2_temp, float ft_platethickness, float fPlateRotation, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 4;

            m_pControlPoint = controlpoint;
            m_fb_1 = fb_1_temp;
            m_fh_1 = fh_1_temp;
            m_fb_2 = fb_2_temp;
            m_fh_2 = fh_2_temp;
            m_ft = ft_platethickness;
            m_fSlope_rad = (float)Math.Atan((fh_2_temp - fh_1_temp) / fb_2_temp);
            m_fRotationZ = fPlateRotation;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];

            // Calculate point positions
            Calc_Coord();

            // Fill list of indices for drawing of surface
            loadIndices();
        }

        public CConCom_Plate_AB(GraphObj.CPoint controlpoint, float fb_1_temp, float fh_1_temp, float fb_2_temp, float fh_2_temp, float ft_platethickness, float fSLope_rad_temp, float fPlateRotation, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 4;

            m_pControlPoint = controlpoint;
            m_fb_1 = fb_1_temp;
            m_fh_1 = fh_1_temp;
            m_fb_2 = fb_2_temp;
            m_fh_2 = fh_2_temp;
            m_ft = ft_platethickness;
            m_fSlope_rad = fSLope_rad_temp;
            m_fRotationZ = fPlateRotation;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Fill Array Data
            Calc_Coord();

            // Fill list of indices for drawing of surface
            loadIndices();
        }

        //----------------------------------------------------------------------------
        void Calc_Coord()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = m_fh_1;

            PointsOut2D[1, 0] = m_fb_2;
            PointsOut2D[1, 1] = m_fh_2;

            PointsOut2D[2, 0] = m_fb_1;
            PointsOut2D[2, 1] = 0;

            PointsOut2D[3, 0] = 0;
            PointsOut2D[3, 1] = 0;
        }

        protected override void loadIndices()
        {
            int secNum = 5;
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 2, 3);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 6, 7);

            // Shell Surface
            DrawCaraLaterals_CCW(secNum, TriangleIndices);
        }

        protected override Point3DCollection GetDefinitionPoints()
        {
            Point3DCollection pMeshPositions = new Point3DCollection();

            for (int i = 0; i < 2; i++) // 2 cycles - front and back surface
            {
                // One Side
                for (int j = 0; j < ITotNoPointsin2D; j++)
                {
                    pMeshPositions.Add(new Point3D((i-1) * 0.5f * m_ft, PointsOut2D[j, 0], PointsOut2D[j, 1])); // x1 = - 0.5 t and x2 = 0.5 * t
                }
            }

            return pMeshPositions;
        }

        public override GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of plate edge nodes
            loadIndices();
            mesh.TriangleIndices = TriangleIndices;

            model.Geometry = mesh;

            model.Material = new DiffuseMaterial(brush);  // Set Model Material

            TransformPlateCoord(model);

            return model;
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            // Front Side
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                Point3D pi = new Point3D();
                Point3D pj = new Point3D();

                if (i < (PointsOut2D.Length / 2) - 1)
                {
                    pi = new Point3D(0, PointsOut2D[i, 0], PointsOut2D[i, 1]);
                    pj = new Point3D(0, PointsOut2D[i + 1, 0], PointsOut2D[i + 1, 1]);
                }
                else // Last line
                {
                    pi = new Point3D(0, PointsOut2D[i, 0], PointsOut2D[i, 1]);
                    pj = new Point3D(0, PointsOut2D[0, 0], PointsOut2D[0, 1]);
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // BackSide
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                Point3D pi = new Point3D();
                Point3D pj = new Point3D();

                if (i < (PointsOut2D.Length / 2) - 1)
                {
                    pi = new Point3D(m_ft, PointsOut2D[i, 0], PointsOut2D[i, 1]);
                    pj = new Point3D(m_ft, PointsOut2D[i + 1, 0], PointsOut2D[i + 1, 1]);
                }
                else // Last line
                {
                    pi = new Point3D(m_ft, PointsOut2D[i, 0], PointsOut2D[i, 1]);
                    pj = new Point3D(m_ft, PointsOut2D[0, 0], PointsOut2D[0, 1]);
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Lateral
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                Point3D pi = new Point3D();
                Point3D pj = new Point3D();

                pi = new Point3D(0, PointsOut2D[i, 0], PointsOut2D[i, 1]);
                pj = new Point3D(m_ft, PointsOut2D[i, 0], PointsOut2D[i, 1]);

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            return wireFrame;
        }

        public void TransformPlateCoord(GeometryModel3D model)
        {
            // Rotate Plate from its cs to joint cs system in GCS about z
            RotateTransform3D RotateTrans3D_AUX_Z = new RotateTransform3D();

            RotateTrans3D_AUX_Z.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), m_fRotationZ); // Rotation in degrees

            // Move 0,0,0 to control point in GCS
            TranslateTransform3D Translate3D_AUX = new TranslateTransform3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);

            Transform3DGroup Trans3DGroup = new Transform3DGroup();
            Trans3DGroup.Children.Add(RotateTrans3D_AUX_Z);
            Trans3DGroup.Children.Add(Translate3D_AUX);

            model.Transform = Trans3DGroup;
        }
    }
}
