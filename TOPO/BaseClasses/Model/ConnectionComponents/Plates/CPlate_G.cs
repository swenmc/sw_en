using _3DTools;
using BaseClasses.GraphObj;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConCom_Plate_G : CPlate
    {
        private float m_fbX1;

        public float Fb_X1
        {
            get
            {
                return m_fbX1;
            }

            set
            {
                m_fbX1 = value;
            }
        }

        private float m_fbX2;

        public float Fb_X2
        {
            get
            {
                return m_fbX2;
            }

            set
            {
                m_fbX2 = value;
            }
        }

        private float m_fhY;

        public float Fh_Y
        {
            get
            {
                return m_fhY;
            }

            set
            {
                m_fhY = value;
            }
        }

        private float m_fhY2;

        public float Fh_Y2
        {
            get
            {
                return m_fhY2;
            }

            set
            {
                m_fhY2 = value;
            }
        }

        private float m_flZ; // Not used in 2D model

        public float Fl_Z
        {
            get
            {
                return m_flZ;
            }

            set
            {
                m_flZ = value;
            }
        }

        private bool bDefineLeftPlate = true;
        private int iLeftRightIndex; // plate 0 - left, 1 - right 

        public CConCom_Plate_G()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_G(string sName_temp,
            Point3D controlpoint,
            float fbX1_temp,
            float fbX2_temp,
            float fhY_temp,
            float fhY2_temp,
            float fl_Z_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_G screwArrangement_temp,  // TODO
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_G;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 7;
            ITotNoPointsin3D = 14;

            m_pControlPoint = controlpoint;
            iLeftRightIndex = sName_temp.Substring(4, 2) == "LH" ? 0 : 1; // Side index - 0 - left (original), 1 - right
            m_fbX1 = fbX1_temp;
            m_fbX2 = fbX2_temp;
            m_fhY = fhY_temp;
            m_fhY2 = fhY2_temp;
            m_flZ = fl_Z_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            arrConnectorControlPoints3D = new Point3D[screwArrangement_temp.IHolesNumber];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();
            screwArrangement_temp.Calc_HolesCentersCoord2D(Fb_X1, Fb_X2, Fh_Y, Fl_Z);
            Calc_HolesControlPointsCoord3D(screwArrangement_temp);

            // Fill list of indices for drawing of surface
            loadIndices();

            bool bChangeRotationAngle_MirroredPlate = false;

            if (iLeftRightIndex % 2 != 0) // Change x-coordinates for odd index (RH)
            {
                bChangeRotationAngle_MirroredPlate = true; // Change rotation angle (about vertical axis Y) of screws in the left leg

                for (int i = 0; i < ITotNoPointsin2D; i++)
                {
                    PointsOut2D[i].X *= -1;
                }

                if (screwArrangement_temp != null)
                {
                    for (int i = 0; i < screwArrangement_temp.IHolesNumber; i++)
                    {
                        screwArrangement_temp.HolesCentersPoints2D[i].X *= -1;
                        arrConnectorControlPoints3D[i].X *= -1;
                    }
                }

                for (int i = 0; i < ITotNoPointsin3D; i++)
                {
                    arrPoints3D[i].X *= -1;
                }
            }

            GenerateConnectors(screwArrangement_temp, bChangeRotationAngle_MirroredPlate);

            Width_bx = m_fbX1;
            Height_hy = m_fhY;
            fArea = MATH.Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY);
            int iNumberOfScrewsInSection = 8; // TODO, temporary - zavisi na rozmiestneni skrutiek
            fA_n = fA_g - iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
            fA_v_zv = Get_A_rect(Ft, m_fhY);
            fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
            fI_yu = Get_I_yu_rect(Ft, m_fhY);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY); // Elastic section modulus

            ScrewArrangement = screwArrangement_temp;
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            if (bDefineLeftPlate)
            {
                PointsOut2D[0].X = 0;
                PointsOut2D[0].Y = 0;

                PointsOut2D[1].X = m_flZ;
                PointsOut2D[1].Y = 0;

                PointsOut2D[2].X = PointsOut2D[1].X + m_fbX1;
                PointsOut2D[2].Y = 0;

                PointsOut2D[3].X = PointsOut2D[2].X;
                PointsOut2D[3].Y = m_fhY2;

                PointsOut2D[4].X = PointsOut2D[1].X + m_fbX2;
                PointsOut2D[4].Y = m_fhY;

                PointsOut2D[5].X = PointsOut2D[1].X;
                PointsOut2D[5].Y = m_fhY;

                PointsOut2D[6].X = PointsOut2D[0].X;
                PointsOut2D[6].Y = m_fhY;
            }
            else
            {
                PointsOut2D[0].X = 0;
                PointsOut2D[0].Y = 0;

                PointsOut2D[1].X = m_fbX1;
                PointsOut2D[1].Y = 0;

                PointsOut2D[2].X = PointsOut2D[1].X + m_flZ;
                PointsOut2D[2].Y = 0;

                PointsOut2D[3].X = PointsOut2D[2].X;
                PointsOut2D[3].Y = m_fhY;

                PointsOut2D[4].X = PointsOut2D[1].X;
                PointsOut2D[4].Y = m_fhY;

                PointsOut2D[5].X = PointsOut2D[1].X - m_fbX2;
                PointsOut2D[5].Y = m_fhY;

                PointsOut2D[6].X = PointsOut2D[0].X;
                PointsOut2D[6].Y = m_fhY2;
            }
        }

        public override void Calc_Coord3D()
        {
            Ft = 0.02f;

            if (bDefineLeftPlate)
            {
                arrPoints3D[0].X = 0;
                arrPoints3D[0].Y = 0;
                arrPoints3D[0].Z = m_flZ;

                arrPoints3D[1].X = 0;
                arrPoints3D[1].Y = 0;
                arrPoints3D[1].Z = 0;

                arrPoints3D[2].X = m_fbX1;
                arrPoints3D[2].Y = arrPoints3D[1].Y;
                arrPoints3D[2].Z = 0;

                arrPoints3D[3].X = arrPoints3D[2].X;
                arrPoints3D[3].Y = m_fhY2;
                arrPoints3D[3].Z = 0;

                arrPoints3D[4].X = m_fbX2;
                arrPoints3D[4].Y = m_fhY;
                arrPoints3D[4].Z = 0;

                arrPoints3D[5].X = arrPoints3D[1].X;
                arrPoints3D[5].Y = m_fhY;
                arrPoints3D[5].Z = 0;

                arrPoints3D[6].X = arrPoints3D[0].X;
                arrPoints3D[6].Y = m_fhY;
                arrPoints3D[6].Z = arrPoints3D[0].Z;

                arrPoints3D[7].X = arrPoints3D[0].X + Ft;
                arrPoints3D[7].Y = arrPoints3D[0].Y;
                arrPoints3D[7].Z = arrPoints3D[0].Z;

                arrPoints3D[8].X = arrPoints3D[1].X + Ft;
                arrPoints3D[8].Y = arrPoints3D[1].Y;
                arrPoints3D[8].Z = arrPoints3D[1].Z + Ft;

                arrPoints3D[9].X = arrPoints3D[2].X;
                arrPoints3D[9].Y = arrPoints3D[2].Y;
                arrPoints3D[9].Z = arrPoints3D[2].Z + Ft;

                arrPoints3D[10].X = arrPoints3D[3].X;
                arrPoints3D[10].Y = arrPoints3D[3].Y;
                arrPoints3D[10].Z = arrPoints3D[3].Z + Ft;

                arrPoints3D[11].X = arrPoints3D[4].X;
                arrPoints3D[11].Y = arrPoints3D[4].Y;
                arrPoints3D[11].Z = arrPoints3D[4].Z + Ft;

                arrPoints3D[12].X = arrPoints3D[5].X + Ft;
                arrPoints3D[12].Y = arrPoints3D[5].Y;
                arrPoints3D[12].Z = arrPoints3D[5].Z + Ft;

                arrPoints3D[13].X = arrPoints3D[6].X + Ft;
                arrPoints3D[13].Y = arrPoints3D[6].Y;
                arrPoints3D[13].Z = arrPoints3D[6].Z;
            }
            else
            {
                arrPoints3D[0].X = 0;
                arrPoints3D[0].Y = 0;
                arrPoints3D[0].Z = 0;

                arrPoints3D[1].X = m_fbX1;
                arrPoints3D[1].Y = 0;
                arrPoints3D[1].Z = 0;

                arrPoints3D[2].X = arrPoints3D[1].X;
                arrPoints3D[2].Y = arrPoints3D[1].Y;
                arrPoints3D[2].Z = m_flZ;

                arrPoints3D[3].X = arrPoints3D[2].X;
                arrPoints3D[3].Y = m_fhY;
                arrPoints3D[3].Z = arrPoints3D[2].Z;

                arrPoints3D[4].X = arrPoints3D[1].X;
                arrPoints3D[4].Y = m_fhY;
                arrPoints3D[4].Z = arrPoints3D[1].Z;

                arrPoints3D[5].X = arrPoints3D[4].X - m_fbX2;
                arrPoints3D[5].Y = m_fhY;
                arrPoints3D[5].Z = arrPoints3D[0].Z;

                arrPoints3D[6].X = arrPoints3D[0].X;
                arrPoints3D[6].Y = m_fhY2;
                arrPoints3D[6].Z = arrPoints3D[0].Z;

                arrPoints3D[7].X = arrPoints3D[0].X;
                arrPoints3D[7].Y = arrPoints3D[0].Y;
                arrPoints3D[7].Z = arrPoints3D[0].Z + Ft;

                arrPoints3D[8].X = arrPoints3D[1].X - Ft;
                arrPoints3D[8].Y = arrPoints3D[1].Y;
                arrPoints3D[8].Z = arrPoints3D[1].Z + Ft;

                arrPoints3D[9].X = arrPoints3D[8].X;
                arrPoints3D[9].Y = arrPoints3D[2].Y;
                arrPoints3D[9].Z = arrPoints3D[2].Z;

                arrPoints3D[10].X = arrPoints3D[9].X;
                arrPoints3D[10].Y = arrPoints3D[9].Y + m_fhY;
                arrPoints3D[10].Z = arrPoints3D[9].Z;

                arrPoints3D[11].X = arrPoints3D[8].X;
                arrPoints3D[11].Y = arrPoints3D[8].Y + m_fhY;
                arrPoints3D[11].Z = arrPoints3D[8].Z;

                arrPoints3D[12].X = arrPoints3D[5].X;
                arrPoints3D[12].Y = arrPoints3D[5].Y;
                arrPoints3D[12].Z = arrPoints3D[5].Z + Ft;

                arrPoints3D[13].X = arrPoints3D[6].X;
                arrPoints3D[13].Y = arrPoints3D[6].Y;
                arrPoints3D[13].Z = arrPoints3D[6].Z + Ft;
            }
        }

        void Calc_HolesControlPointsCoord3D(CScrewArrangement screwArrangement)
        {
            // TODO
            float fx_edge = m_flZ * 0.5f; // Middle of left leg
            float fy_edge1 = 0.200f;
            float fy_edge2 = 0.050f;

            float fScrewOffset = screwArrangement.referenceScrew.T_ht_headTotalThickness;

            arrConnectorControlPoints3D[0].X = Ft + fScrewOffset;
            arrConnectorControlPoints3D[0].Y = fy_edge1;
            arrConnectorControlPoints3D[0].Z = m_flZ - fx_edge;

            arrConnectorControlPoints3D[1].X = Ft + fScrewOffset;
            arrConnectorControlPoints3D[1].Y = arrConnectorControlPoints3D[0].Y + fy_edge2;
            arrConnectorControlPoints3D[1].Z = m_flZ - fx_edge;
        }

        void GenerateConnectors(CScrewArrangement screwArrangement, bool bChangeRotationAngle_MirroredPlate)
        {
            if (screwArrangement.IHolesNumber > 0)
            {
                screwArrangement.Screws = new CScrew[screwArrangement.IHolesNumber];

                int iNumberOfScrewsInLeftLeg = screwArrangement.IHolesNumber / 2;

                float fRotationAngleAboutYAxis = 180; // Vertical Axis

                if (bChangeRotationAngle_MirroredPlate)
                    fRotationAngleAboutYAxis = 0;

                if (screwArrangement is CScrewArrangement_G)
                    iNumberOfScrewsInLeftLeg = 2; // TODO - umoznit nastavovat dynamicky

                for (int i = 0; i < screwArrangement.IHolesNumber; i++)
                {
                    if (i < iNumberOfScrewsInLeftLeg) // Left Leg
                    {
                        Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);
                        screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, fRotationAngleAboutYAxis, 0, true);
                    }
                    else
                    {
                        Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);
                        screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, 90, 0, true);
                    }
                }
            }
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            if (bDefineLeftPlate)
            {
                if (iLeftRightIndex == 0) // Left
                {
                    AddPenthagonIndices_CW_12345(TriangleIndices, 8, 12, 11, 10, 9);
                    AddRectangleIndices_CW_1234(TriangleIndices, 9, 10, 3, 2);
                    AddPenthagonIndices_CW_12345(TriangleIndices, 2, 3, 4, 5, 1);
                    AddRectangleIndices_CW_1234(TriangleIndices, 1, 5, 6, 0);
                    AddRectangleIndices_CW_1234(TriangleIndices, 0, 6, 13, 7);
                    AddRectangleIndices_CW_1234(TriangleIndices, 7, 13, 12, 8);

                    AddRectangleIndices_CW_1234(TriangleIndices, 0, 7, 8, 1);
                    AddRectangleIndices_CW_1234(TriangleIndices, 8, 9, 2, 1);

                    AddRectangleIndices_CW_1234(TriangleIndices, 6, 5, 12, 13);
                    AddRectangleIndices_CW_1234(TriangleIndices, 5, 4, 11, 12);
                    AddRectangleIndices_CW_1234(TriangleIndices, 4, 3, 10, 11);
                }
                else if (iLeftRightIndex == 1) // Right
                {
                    AddPenthagonIndices_CCW_12345(TriangleIndices, 8, 12, 11, 10, 9);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 9, 10, 3, 2);
                    AddPenthagonIndices_CCW_12345(TriangleIndices, 2, 3, 4, 5, 1);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 1, 5, 6, 0);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 0, 6, 13, 7);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 7, 13, 12, 8);

                    AddRectangleIndices_CCW_1234(TriangleIndices, 0, 7, 8, 1);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 2, 1);

                    AddRectangleIndices_CCW_1234(TriangleIndices, 6, 5, 12, 13);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 5, 4, 11, 12);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 4, 3, 10, 11);
                }
            }
            else
            {
                if (iLeftRightIndex == 0) // Left
                {
                    AddPenthagonIndices_CW_12345(TriangleIndices, 7, 13, 12, 11, 8);
                    AddRectangleIndices_CW_1234(TriangleIndices, 8, 11, 10, 9);
                    AddRectangleIndices_CW_1234(TriangleIndices, 9, 10, 3, 2);
                    AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 1);
                    AddPenthagonIndices_CW_12345(TriangleIndices, 4, 5, 6, 0, 1);
                    AddRectangleIndices_CW_1234(TriangleIndices, 0, 6, 13, 7);

                    AddRectangleIndices_CW_1234(TriangleIndices, 0, 7, 8, 1);
                    AddRectangleIndices_CW_1234(TriangleIndices, 8, 9, 2, 1);

                    AddRectangleIndices_CW_1234(TriangleIndices, 6, 5, 12, 13);
                    AddRectangleIndices_CW_1234(TriangleIndices, 5, 4, 11, 12);
                    AddRectangleIndices_CW_1234(TriangleIndices, 4, 3, 10, 11);
                }
                else if (iLeftRightIndex == 1) // Right
                {
                    AddPenthagonIndices_CCW_12345(TriangleIndices, 7, 13, 12, 11, 8);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 8, 11, 10, 9);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 9, 10, 3, 2);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 4, 1);
                    AddPenthagonIndices_CCW_12345(TriangleIndices, 4, 5, 6, 0, 1);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 0, 6, 13, 7);

                    AddRectangleIndices_CCW_1234(TriangleIndices, 0, 7, 8, 1);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 2, 1);

                    AddRectangleIndices_CCW_1234(TriangleIndices, 6, 5, 12, 13);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 5, 4, 11, 12);
                    AddRectangleIndices_CCW_1234(TriangleIndices, 4, 3, 10, 11);
                }
            }
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            for (int i = 0; i < ITotNoPointsin2D; i++)
            {
                if (i < ITotNoPointsin2D - 1)
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

            for (int i = 0; i < ITotNoPointsin2D; i++)
            {
                if (i < ITotNoPointsin2D - 1)
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

            if (bDefineLeftPlate)
            {
                // Lateral
                wireFrame.Points.Add(arrPoints3D[0]);
                wireFrame.Points.Add(arrPoints3D[7]);

                wireFrame.Points.Add(arrPoints3D[2]);
                wireFrame.Points.Add(arrPoints3D[9]);

                wireFrame.Points.Add(arrPoints3D[3]);
                wireFrame.Points.Add(arrPoints3D[10]);

                wireFrame.Points.Add(arrPoints3D[4]);
                wireFrame.Points.Add(arrPoints3D[11]);

                wireFrame.Points.Add(arrPoints3D[6]);
                wireFrame.Points.Add(arrPoints3D[13]);

                wireFrame.Points.Add(arrPoints3D[1]);
                wireFrame.Points.Add(arrPoints3D[5]);

                wireFrame.Points.Add(arrPoints3D[8]);
                wireFrame.Points.Add(arrPoints3D[12]);
            }
            else
            {
                // Lateral
                wireFrame.Points.Add(arrPoints3D[0]);
                wireFrame.Points.Add(arrPoints3D[7]);

                wireFrame.Points.Add(arrPoints3D[2]);
                wireFrame.Points.Add(arrPoints3D[9]);

                wireFrame.Points.Add(arrPoints3D[3]);
                wireFrame.Points.Add(arrPoints3D[10]);

                wireFrame.Points.Add(arrPoints3D[5]);
                wireFrame.Points.Add(arrPoints3D[12]);

                wireFrame.Points.Add(arrPoints3D[6]);
                wireFrame.Points.Add(arrPoints3D[13]);

                wireFrame.Points.Add(arrPoints3D[1]);
                wireFrame.Points.Add(arrPoints3D[4]);

                wireFrame.Points.Add(arrPoints3D[8]);
                wireFrame.Points.Add(arrPoints3D[11]);
            }

            return wireFrame;
        }
    }
}
