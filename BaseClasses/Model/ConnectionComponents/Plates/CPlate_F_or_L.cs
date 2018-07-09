using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    public class CConCom_Plate_F_or_L : CPlate
    {
        public float m_fbX1;
        public float m_fbX2;
        public float m_fhY;
        public float m_flZ; // Not used in 2D model
        public float m_ft; // Not used in 2D model
        private float fConnectorLength;
        
        public float FConnectorLength
        {
            get { return fConnectorLength; }
            set { fConnectorLength = value; }            
        }       

        public CConCom_Plate_F_or_L()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        // L with or without holes
        public CConCom_Plate_F_or_L(string sName_temp, CPoint controlpoint, float fbX_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, int iHolesNumber, float fHoleDiameter_temp, float fConnectorLength_temp, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 6;
            ITotNoPointsin3D = 12;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX_temp;
            m_fbX2 = m_fbX1; // L Series - without slope
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber;
            FHoleDiameter = fHoleDiameter_temp;
            FConnectorLength = fConnectorLength_temp;
            // m_arrPlateConnectors = arrPlateConnectors_temp; // Generate
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

            fWidth_bx = m_fbX1 + m_fbX2;
            fHeight_hy = m_fhY;
            fThickness_tz = m_ft;
            fArea = PolygonArea();
        }

        public CConCom_Plate_F_or_L(string sName_temp, CPoint controlpoint, float fbX_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, int iHolesNumber, float fHoleDiameter_temp, CConnector [] arrPlateConnectors_temp,  bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 6;
            ITotNoPointsin3D = 12;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX_temp;
            m_fbX2 = m_fbX1; // L Series - without slope
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber;
            FHoleDiameter = fHoleDiameter_temp;
            m_arrPlateConnectors = arrPlateConnectors_temp;
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

            fWidth_bx = m_fbX1 + m_fbX2;
            fHeight_hy = m_fhY;
            fThickness_tz = m_ft;
            fArea = PolygonArea();
        }

        // F - no holes
        public CConCom_Plate_F_or_L(string sName_temp, CPoint controlpoint, int iLeftRightIndex, float fbX1_temp, float fbX2_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, float fRotation_x_deg, float fRotation_y_deg, float fRotation_z_deg, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 6;
            ITotNoPointsin3D = 12;

            m_pControlPoint = controlpoint;
            m_fbX1 = fbX1_temp;
            m_fbX2 = fbX2_temp;
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            IHolesNumber = 0;
            FHoleDiameter = 0;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            // Fill list of indices for drawing of surface
            loadIndices();

            if (iLeftRightIndex % 2 != 0) // Change y-coordinates for odd index (RH)
            {
                for (int i = 0; i < ITotNoPointsin2D; i++)
                {
                    PointsOut2D[i, 0] *= -1;
                }

                for (int i = 0; i < IHolesNumber; i++)
                {
                    HolesCentersPoints2D[i, 0] *= -1;
                }

                for (int i = 0; i < ITotNoPointsin3D; i++)
                {
                    arrPoints3D[i].X *= -1;
                }

                // Change indices
                ChangeIndices(TriangleIndices); // Todo - takto to nefunguje :-)
            }

            fWidth_bx = m_fbX1 + m_fbX2;
            fHeight_hy = m_fhY;
            fThickness_tz = m_ft;
            fArea = PolygonArea();
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_flZ;
            PointsOut2D[1, 1] = 0;

            PointsOut2D[2, 0] = PointsOut2D[1, 0] + m_fbX1;
            PointsOut2D[2, 1] = 0;

            PointsOut2D[3, 0] = PointsOut2D[1, 0] + m_fbX2;
            PointsOut2D[3, 1] = m_fhY;

            PointsOut2D[4, 0] = PointsOut2D[1, 0];
            PointsOut2D[4, 1] = m_fhY;

            PointsOut2D[5, 0] = PointsOut2D[0, 0];
            PointsOut2D[5, 1] = m_fhY;
        }

        void Calc_Coord3D()
        {
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX1;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = m_fbX2;
            arrPoints3D[3].Y = m_fhY;
            arrPoints3D[3].Z = arrPoints3D[2].Z;

            arrPoints3D[4].X = arrPoints3D[1].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[1].Z;

            arrPoints3D[5].X = arrPoints3D[0].X;
            arrPoints3D[5].Y = m_fhY;
            arrPoints3D[5].Z = arrPoints3D[0].Z;

            arrPoints3D[6].X = arrPoints3D[0].X + m_ft;
            arrPoints3D[6].Y = arrPoints3D[0].Y;
            arrPoints3D[6].Z = arrPoints3D[0].Z;

            arrPoints3D[7].X = arrPoints3D[1].X + +m_ft;
            arrPoints3D[7].Y = arrPoints3D[1].Y;
            arrPoints3D[7].Z = arrPoints3D[1].Z + m_ft;

            arrPoints3D[8].X = arrPoints3D[2].X;
            arrPoints3D[8].Y = arrPoints3D[2].Y;
            arrPoints3D[8].Z = arrPoints3D[2].Z + m_ft;

            arrPoints3D[9].X = arrPoints3D[3].X;
            arrPoints3D[9].Y = arrPoints3D[3].Y;
            arrPoints3D[9].Z = arrPoints3D[3].Z + m_ft;

            arrPoints3D[10].X = arrPoints3D[7].X;
            arrPoints3D[10].Y = arrPoints3D[7].Y + m_fhY;
            arrPoints3D[10].Z = arrPoints3D[7].Z;

            arrPoints3D[11].X = arrPoints3D[6].X;
            arrPoints3D[11].Y = arrPoints3D[6].Y + m_fhY;
            arrPoints3D[11].Z = arrPoints3D[6].Z;
        }

        void Calc_HolesCentersCoord2D()
        {
            float fx_edge = 0.010f;
            float fy_edge1 = 0.010f;
            float fy_edge2 = 0.030f;
            float fy_edge3 = 0.120f;

            // TODO nahradit enumom a switchom

            if (IHolesNumber == 16) // LH, LI, LK
            {
                // Left Leg

                HolesCentersPoints2D[0, 0] = fx_edge;
                HolesCentersPoints2D[0, 1] = fy_edge1;

                HolesCentersPoints2D[1, 0] = m_flZ - fx_edge;
                HolesCentersPoints2D[1, 1] = HolesCentersPoints2D[0, 1];

                HolesCentersPoints2D[2, 0] = 0.5f * m_flZ;
                HolesCentersPoints2D[2, 1] = fy_edge2;

                HolesCentersPoints2D[3, 0] = 0.5f * m_flZ;
                HolesCentersPoints2D[3, 1] = fy_edge3;

                HolesCentersPoints2D[4, 0] = HolesCentersPoints2D[3, 0];
                HolesCentersPoints2D[4, 1] = m_fhY - fy_edge3;

                HolesCentersPoints2D[5, 0] = HolesCentersPoints2D[3, 0];
                HolesCentersPoints2D[5, 1] = m_fhY - fy_edge2;

                HolesCentersPoints2D[6, 0] = HolesCentersPoints2D[0, 0];
                HolesCentersPoints2D[6, 1] = m_fhY - fy_edge1;

                HolesCentersPoints2D[7, 0] = HolesCentersPoints2D[1, 0];
                HolesCentersPoints2D[7, 1] = m_fhY - fy_edge1;

                // Right Leg

                HolesCentersPoints2D[8, 0] = m_fbX1 + HolesCentersPoints2D[6, 0];
                HolesCentersPoints2D[8, 1] = HolesCentersPoints2D[6, 1];

                HolesCentersPoints2D[9, 0] = m_fbX1 + HolesCentersPoints2D[7, 0];
                HolesCentersPoints2D[9, 1] = HolesCentersPoints2D[7, 1];

                HolesCentersPoints2D[10, 0] = m_fbX1 + HolesCentersPoints2D[5, 0];
                HolesCentersPoints2D[10, 1] = HolesCentersPoints2D[5, 1];

                HolesCentersPoints2D[11, 0] = m_fbX1 + HolesCentersPoints2D[4, 0];
                HolesCentersPoints2D[11, 1] = HolesCentersPoints2D[4, 1];

                HolesCentersPoints2D[12, 0] = m_fbX1 + HolesCentersPoints2D[3, 0];
                HolesCentersPoints2D[12, 1] = HolesCentersPoints2D[3, 1];

                HolesCentersPoints2D[13, 0] = m_fbX1 + HolesCentersPoints2D[2, 0];
                HolesCentersPoints2D[13, 1] = HolesCentersPoints2D[2, 1];

                HolesCentersPoints2D[14, 0] = m_fbX1 + HolesCentersPoints2D[1, 0];
                HolesCentersPoints2D[14, 1] = HolesCentersPoints2D[1, 1];

                HolesCentersPoints2D[15, 0] = m_fbX1 + HolesCentersPoints2D[0, 0];
                HolesCentersPoints2D[15, 1] = HolesCentersPoints2D[0, 1];
            }
            else if (IHolesNumber == 8) // LJ
            {
                // Left Leg

                HolesCentersPoints2D[0, 0] = fx_edge;
                HolesCentersPoints2D[0, 1] = fy_edge1;

                HolesCentersPoints2D[1, 0] = m_flZ - fx_edge;
                HolesCentersPoints2D[1, 1] = HolesCentersPoints2D[0, 1];

                HolesCentersPoints2D[2, 0] = HolesCentersPoints2D[0, 0];
                HolesCentersPoints2D[2, 1] = m_fhY - fy_edge1;

                HolesCentersPoints2D[3, 0] = HolesCentersPoints2D[1, 0];
                HolesCentersPoints2D[3, 1] = HolesCentersPoints2D[2, 1];

                // Right Leg

                HolesCentersPoints2D[4, 0] = m_fbX1 + HolesCentersPoints2D[2, 0];
                HolesCentersPoints2D[4, 1] = HolesCentersPoints2D[2, 1];

                HolesCentersPoints2D[5, 0] = m_fbX1 + HolesCentersPoints2D[3, 0];
                HolesCentersPoints2D[5, 1] = HolesCentersPoints2D[2, 1];

                HolesCentersPoints2D[6, 0] = m_fbX1 + HolesCentersPoints2D[0, 0];
                HolesCentersPoints2D[6, 1] = HolesCentersPoints2D[0, 1];

                HolesCentersPoints2D[7, 0] = m_fbX1 + HolesCentersPoints2D[1, 0];
                HolesCentersPoints2D[7, 1] = HolesCentersPoints2D[0, 1];
            }
            else
            {
                // Not defined expected number of holes for L or F plate
            }
        }

        void Calc_HolesControlPointsCoord3D()
        {
            float fx_edge = 0.010f;
            float fy_edge1 = 0.010f;
            float fy_edge2 = 0.030f;
            float fy_edge3 = 0.120f;

            // TODO nahradit enumom a switchom

            if (IHolesNumber == 16) // LH, LI, LK
            {
                // Left Leg

                arrConnectorControlPoints3D[0].X = -m_ft;
                arrConnectorControlPoints3D[0].Y = fy_edge1;
                arrConnectorControlPoints3D[0].Z = m_flZ - fx_edge;

                arrConnectorControlPoints3D[1].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[1].Y = arrConnectorControlPoints3D[0].Y;
                arrConnectorControlPoints3D[1].Z = fx_edge;

                arrConnectorControlPoints3D[2].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[2].Y = fy_edge2;
                arrConnectorControlPoints3D[2].Z = 0.5f * m_flZ;

                arrConnectorControlPoints3D[3].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[3].Y = fy_edge3;
                arrConnectorControlPoints3D[3].Z = arrConnectorControlPoints3D[2].Z;

                arrConnectorControlPoints3D[4].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[4].Y = m_fhY - fy_edge3;
                arrConnectorControlPoints3D[4].Z = arrConnectorControlPoints3D[2].Z;

                arrConnectorControlPoints3D[5].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[5].Y = m_fhY - fy_edge2;
                arrConnectorControlPoints3D[5].Z = arrConnectorControlPoints3D[2].Z;

                arrConnectorControlPoints3D[6].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[6].Y = m_fhY - fy_edge1;
                arrConnectorControlPoints3D[6].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[7].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[7].Y = m_fhY - fy_edge1;
                arrConnectorControlPoints3D[7].Z = arrConnectorControlPoints3D[1].Z;

                // Right Leg

                arrConnectorControlPoints3D[8].X = fx_edge;
                arrConnectorControlPoints3D[8].Y = m_fhY - fy_edge1;
                arrConnectorControlPoints3D[8].Z = -m_ft;

                arrConnectorControlPoints3D[9].X = m_fbX1 - fx_edge;
                arrConnectorControlPoints3D[9].Y = arrConnectorControlPoints3D[8].Y;
                arrConnectorControlPoints3D[9].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[10].X = 0.5f * m_fbX1;
                arrConnectorControlPoints3D[10].Y = m_fhY - fy_edge2;
                arrConnectorControlPoints3D[10].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[11].X = arrConnectorControlPoints3D[10].X;
                arrConnectorControlPoints3D[11].Y = m_fhY - fy_edge3;
                arrConnectorControlPoints3D[11].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[12].X = arrConnectorControlPoints3D[10].X;
                arrConnectorControlPoints3D[12].Y = fy_edge3;
                arrConnectorControlPoints3D[12].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[13].X = arrConnectorControlPoints3D[10].X;
                arrConnectorControlPoints3D[13].Y = fy_edge2;
                arrConnectorControlPoints3D[13].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[14].X = arrConnectorControlPoints3D[8].X;
                arrConnectorControlPoints3D[14].Y = fy_edge1;
                arrConnectorControlPoints3D[14].Z = arrConnectorControlPoints3D[8].Z;

                arrConnectorControlPoints3D[15].X = arrConnectorControlPoints3D[9].X;
                arrConnectorControlPoints3D[15].Y = fy_edge1;
                arrConnectorControlPoints3D[15].Z = arrConnectorControlPoints3D[8].Z;
            }
            else if (IHolesNumber == 8) // LJ
            {
                // Left Leg

                arrConnectorControlPoints3D[0].X = -m_ft;
                arrConnectorControlPoints3D[0].Y = fy_edge1;
                arrConnectorControlPoints3D[0].Z = m_flZ - fx_edge;

                arrConnectorControlPoints3D[1].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[1].Y = arrConnectorControlPoints3D[0].Y;
                arrConnectorControlPoints3D[1].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[2].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[2].Y = m_fhY - fy_edge1;
                arrConnectorControlPoints3D[2].Z = arrConnectorControlPoints3D[0].Z;

                arrConnectorControlPoints3D[3].X = arrConnectorControlPoints3D[0].X;
                arrConnectorControlPoints3D[3].Y = arrConnectorControlPoints3D[2].Y;
                arrConnectorControlPoints3D[3].Z = arrConnectorControlPoints3D[1].Z;

                // Right Leg

                arrConnectorControlPoints3D[4].X = fx_edge;
                arrConnectorControlPoints3D[4].Y = m_fhY - fy_edge1;
                arrConnectorControlPoints3D[4].Z = -m_ft;

                arrConnectorControlPoints3D[5].X = m_fbX1 - fx_edge;
                arrConnectorControlPoints3D[5].Y = arrConnectorControlPoints3D[4].Y;
                arrConnectorControlPoints3D[5].Z = arrConnectorControlPoints3D[4].Z;

                arrConnectorControlPoints3D[6].X = arrConnectorControlPoints3D[4].X;
                arrConnectorControlPoints3D[6].Y = fy_edge1;
                arrConnectorControlPoints3D[6].Z = arrConnectorControlPoints3D[4].Z;

                arrConnectorControlPoints3D[7].X = arrConnectorControlPoints3D[5].X;
                arrConnectorControlPoints3D[7].Y = arrConnectorControlPoints3D[6].Y;
                arrConnectorControlPoints3D[7].Z = arrConnectorControlPoints3D[4].Z;
            }
            else
            {
                // Not defined expected number of holes for L or F plate
            }
        }

        void GenerateConnectors()
        {
            if (IHolesNumber > 0)
            {
                m_arrPlateConnectors = new CConnector[IHolesNumber];

                for (int i = 0; i < IHolesNumber; i++)
                {
                    if (i < IHolesNumber / 2) // Left Leg
                    {
                        CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                        m_arrPlateConnectors[i] = new CConnector("TEK", controlpoint, 12, FHoleDiameter, 0.02f, 0.012f, 0, 0, 0, true);
                    }
                    else
                    {
                        CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                        m_arrPlateConnectors[i] = new CConnector("TEK", controlpoint, 12, FHoleDiameter, 0.02f, 0.012f, 0, 90, 0, true);
                    }
                }
            }
        }

        protected override void loadIndices()
        {
            int secNum = 6;
            TriangleIndices = new Int32Collection();

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 5, 4, 1);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 4, 3, 2);
            AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 10, 11);
            AddRectangleIndices_CCW_1234(TriangleIndices, 7, 8, 9, 10);

            // Shell Surface
            DrawCaraLaterals_CW(secNum, TriangleIndices);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            // y = 0
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[1]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[2]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[8]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[0]);

            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[8]);

            // y = m_fhY
            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[11]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[4]);

            return wireFrame;
        }


    }
}
