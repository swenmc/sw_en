using _3DTools;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    public class CConCom_Plate_KA : CPlate
    {
        float m_fbX1;
        float m_fhY1;
        float m_fbX2;
        float m_fhY2;
        float m_ft;
        float m_fSlope_rad;

        float m_fCrscWebStraightDepth;
        float m_fStiffenerSize; // Middle cross-section stiffener dimension (without screws)

        private float fConnectorLength;

        public float FConnectorLength
        {
            get { return fConnectorLength; }
            set { fConnectorLength = value; }
        }

        public CConCom_Plate_KA()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
            BIsDisplayed = true;
        }

        public CConCom_Plate_KA(string sName_temp, GraphObj.CPoint controlpoint, float fb_1_temp, float fh_1_temp, float fb_2_temp, float fh_2_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, int iHolesNumber, float fHoleDiameter_temp, float fConnectorLength_temp, float fCrscWebStraightDepth_temp, float fStiffenerSize_temp, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 4;
            ITotNoPointsin3D = 8;

            m_pControlPoint = controlpoint;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
            m_fhY2 = fh_2_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber;
            FHoleDiameter = fHoleDiameter_temp;
            FConnectorLength = fConnectorLength_temp;
            m_fCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            m_fStiffenerSize = fStiffenerSize_temp;
            m_fSlope_rad = (float)Math.Atan((fh_2_temp - fh_1_temp) / fb_2_temp);
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber, 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D();
            Calc_HolesControlPointsCoord3D();

            // Fill list of indices for drawing of surface
            loadIndices();

            GenerateConnectors();

            fWidth_bx = Math.Max(m_fbX1, m_fbX2);
            fHeight_hy = Math.Max(m_fhY1, m_fhY2);
            fThickness_tz = m_ft;
            fArea = PolygonArea();
        }

        public CConCom_Plate_KA(GraphObj.CPoint controlpoint, float fb_1_temp, float fh_1_temp, float fb_2_temp, float fh_2_temp, float ft_platethickness, float fSLope_rad_temp, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, int iHolesNumber, float fHoleDiameter_temp, float fConnectorLength_temp, float fCrscWebStraightDepth_temp, float fStiffenerSize_temp, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 4;
            ITotNoPointsin3D = 8;

            m_pControlPoint = controlpoint;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
            m_fhY2 = fh_2_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber;
            FHoleDiameter = fHoleDiameter_temp;
            FConnectorLength = fConnectorLength_temp;
            m_fCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            m_fStiffenerSize = fStiffenerSize_temp;
            m_fSlope_rad = fSLope_rad_temp;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber, 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D();
            Calc_HolesControlPointsCoord3D();

            // Fill list of indices for drawing of surface
            loadIndices();

            GenerateConnectors();

            fWidth_bx = Math.Max(m_fbX1, m_fbX2);
            fHeight_hy = Math.Max(m_fhY1, m_fhY2);
            fThickness_tz = m_ft;
            fArea = PolygonArea();
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_fbX1;
            PointsOut2D[1, 1] = 0;

            PointsOut2D[2, 0] = m_fbX2;
            PointsOut2D[2, 1] = m_fhY2;

            PointsOut2D[3, 0] = 0;
            PointsOut2D[3, 1] = m_fhY1;
        }

        void Calc_Coord3D()
        {
            for (int i = 0; i < 2; i++) // 2 cycles - front and back surface
            {
                // One Side
                for (int j = 0; j < ITotNoPointsin2D; j++)
                {
                    arrPoints3D[(i * ITotNoPointsin2D) + j].X = PointsOut2D[j, 0];
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Y = PointsOut2D[j, 1];
                    arrPoints3D[(i * ITotNoPointsin2D) + j].Z = i * m_ft;
                }
            }
        }

        void Calc_HolesCentersCoord2D()
        {
            int iNumberOfCircleJoints = 2;

            float fDistanceOfCenterFromLeftEdge = m_fbX1 / 2f;
            float fx_c1 = fDistanceOfCenterFromLeftEdge;
            float fy_c1 = m_fhY1 / 4f;

            float fx_c2 = fDistanceOfCenterFromLeftEdge; // Symmetrical
            float fy_c2 = m_fhY1 * 3f / 4f; // TODO Dopracovat podla sklonu rafteru

            int iNumberOfSequencesInJoint = 2;

            int iNumberOfScrewsInOneSequence = IHolesNumber / (iNumberOfCircleJoints * iNumberOfSequencesInJoint);

            float fRadius = 0.5f * m_fCrscWebStraightDepth; // m // Input - depending on depth of cross-section
            float fAngle_seq_rotation_init_point_deg = (float)(Math.Atan(0.5f * m_fStiffenerSize / fDistanceOfCenterFromLeftEdge) / MathF.fPI * 180f); // Input - constant for cross-section according to the size of middle sfiffener

            // Left side
            float[,] fSequenceLeftTop;
            float[,] fSequenceLeftBottom;
            Get_ScrewGroup_Circle(IHolesNumber / iNumberOfCircleJoints, fx_c1, fy_c1, m_fCrscWebStraightDepth, fAngle_seq_rotation_init_point_deg, MathF.fPI / 2f, out fSequenceLeftTop, out fSequenceLeftBottom);

            // Right side
            float[,] fSequenceRightTop;
            float[,] fSequenceRightBottom;
            Get_ScrewGroup_Circle(IHolesNumber / iNumberOfCircleJoints, fx_c2, fy_c2, m_fCrscWebStraightDepth, fAngle_seq_rotation_init_point_deg, m_fSlope_rad, out fSequenceRightTop, out fSequenceRightBottom);

            // Fill array of holes centers
            for (int i = 0; i < iNumberOfScrewsInOneSequence; i++) // Add all 4 sequences in one cycle
            {
                HolesCentersPoints2D[i, 0] = fSequenceLeftTop[i, 0];
                HolesCentersPoints2D[i, 1] = fSequenceLeftTop[i, 1];

                HolesCentersPoints2D[iNumberOfScrewsInOneSequence + i, 0] = fSequenceLeftBottom[i, 0];
                HolesCentersPoints2D[iNumberOfScrewsInOneSequence + i, 1] = fSequenceLeftBottom[i, 1];

                HolesCentersPoints2D[2 * iNumberOfScrewsInOneSequence + i, 0] = fSequenceRightTop[i, 0];
                HolesCentersPoints2D[2 * iNumberOfScrewsInOneSequence + i, 1] = fSequenceRightTop[i, 1];

                HolesCentersPoints2D[3 * iNumberOfScrewsInOneSequence + i, 0] = fSequenceRightBottom[i, 0];
                HolesCentersPoints2D[3 * iNumberOfScrewsInOneSequence + i, 1] = fSequenceRightBottom[i, 1];
            }
        }

        void Calc_HolesControlPointsCoord3D()
        {
            for (int i = 0; i < IHolesNumber; i++)
            {
                arrConnectorControlPoints3D[i].X = HolesCentersPoints2D[i, 0];
                arrConnectorControlPoints3D[i].Y = HolesCentersPoints2D[i, 1];
                arrConnectorControlPoints3D[i].Z = -m_ft; // TODO Position depends on screw length;
            }
        }

        void GenerateConnectors()
        {
            m_arrPlateConnectors = new CConnector[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                m_arrPlateConnectors[i] = new CConnector("TEK", controlpoint, 14, FHoleDiameter, FConnectorLength, 0.015f, 0, -90, 0, true);
            }
        }

        protected override void loadIndices()
        {
            int secNum = 4;
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 2, 3);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 6, 7);

            // Shell Surface
            DrawCaraLaterals_CCW(secNum, TriangleIndices);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // Front Side
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                if (i < (PointsOut2D.Length / 2) - 1)
                {
                    pi = arrPoints3D[i];
                    pj = arrPoints3D[i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[i];
                    pj = arrPoints3D[0];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // BackSide
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                if (i < (PointsOut2D.Length / 2) - 1)
                {
                    pi = arrPoints3D[ITotNoPointsin2D + i];
                    pj = arrPoints3D[ITotNoPointsin2D + i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[ITotNoPointsin2D + i];
                    pj = arrPoints3D[ITotNoPointsin2D + 0];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Lateral
            for (int i = 0; i < PointsOut2D.Length / 2; i++)
            {
                pi = arrPoints3D[i];
                pj = arrPoints3D[ITotNoPointsin2D + i];

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            return wireFrame;
        }
    }
}
