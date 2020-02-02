using _3DTools;
using BaseClasses.GraphObj;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConCom_Plate_H : CPlate
    {
        private float m_fbX;

        public float Fb_X
        {
            get
            {
                return m_fbX;
            }

            set
            {
                m_fbX = value;
            }
        }

        private float m_fhY1; // Bottom part

        public float Fh_Y1
        {
            get
            {
                return m_fhY1;
            }

            set
            {
                m_fhY1 = value;
            }
        }

        private float m_fhY2; // Total

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

        private float m_fMainMemberWidth; // Sirka rafteru - potrebne pre urcenie priestoru pre skrutky

        public float MainMemberWidth
        {
            get
            {
                return m_fMainMemberWidth;
            }

            set
            {
                m_fMainMemberWidth = value;
            }
        }

        float m_fSlope_rad;

        public float FSlope_rad
        {
            get
            {
                return m_fSlope_rad;
            }

            set
            {
                m_fSlope_rad = value;
            }
        }

        float m_alpha1_rad;

        public float Alpha1_rad
        {
            get
            {
                return m_alpha1_rad;
            }

            set
            {
                m_alpha1_rad = value;
            }
        }

        public int m_iHolesNumber = 0;

        private int iLeftRightIndex; // plate 0 - left, 1 - right

        private float m_e_min_x_BottomLeg;

        public float e_min_x_BottomLeg
        {
            get
            {
                return m_e_min_x_BottomLeg;
            }

            set
            {
                m_e_min_x_BottomLeg = value;
            }
        }

        private float m_e_min_y_BottomLeg;

        public float e_min_y_BottomLeg
        {
            get
            {
                return m_e_min_y_BottomLeg;
            }

            set
            {
                m_e_min_y_BottomLeg = value;
            }
        }

        private float m_e_min_x_TopLeg;

        public float e_min_x_TopLeg
        {
            get
            {
                return m_e_min_x_TopLeg;
            }

            set
            {
                m_e_min_x_TopLeg = value;
            }
        }

        private float m_e_min_y_TopLeg;

        public float e_min_y_TopLeg
        {
            get
            {
                return m_e_min_y_TopLeg;
            }

            set
            {
                m_e_min_y_TopLeg = value;
            }
        }

        public CConCom_Plate_H()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_H;
        }

        public CConCom_Plate_H(string sName_temp,
            Point3D controlpoint,
            float fbX_temp,
            float fhY1_temp,
            float fhY2_temp,
            float fMainMemberWidth_temp,
            float ft_platethickness,
            float fslope_rad_temp,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_H screwArrangement_temp)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_H;

            ITotNoPointsin2D = 6;
            ITotNoPointsin3D = 12;

            m_pControlPoint = controlpoint;
            iLeftRightIndex = sName_temp.Substring(4, 2) == "LH" ? 0 : 1; // Side index - 0 - left (original), 1 - right
            m_fbX = fbX_temp;
            m_fhY1 = fhY1_temp;
            m_fhY2 = fhY2_temp;
            m_fMainMemberWidth = fMainMemberWidth_temp;
            Ft = ft_platethickness;
            FSlope_rad = fslope_rad_temp;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();
            screwArrangement_temp.Calc_HolesCentersCoord2D(m_fbX, m_fhY1, m_fhY2, m_fMainMemberWidth);
            arrConnectorControlPoints3D = new Point3D[screwArrangement_temp.IHolesNumber];
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

            Width_bx = m_fbX;
            Height_hy = m_fhY1;
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = m_fbX; // Total width
            Height_hy_Stretched = m_fhY2;
            fArea = MATH.Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY2);
            int iNumberOfScrewsInSection = 8; // TODO, temporary - zavisi na rozmiestneni skrutiek
            fA_n = fA_g - iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
            fA_v_zv = Get_A_rect(Ft, m_fhY2);
            fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * screwArrangement_temp.referenceScrew.Diameter_thread * Ft;
            fI_yu = Get_I_yu_rect(Ft, m_fhY1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY2); // Elastic section modulus

            ScrewArrangement = screwArrangement_temp;
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_fbX;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = PointsOut2D[1].X;
            PointsOut2D[2].Y = m_fhY1;

            PointsOut2D[3].X = PointsOut2D[2].X;
            PointsOut2D[3].Y = m_fhY2;

            PointsOut2D[4].X = 0;
            PointsOut2D[4].Y = m_fhY2;

            PointsOut2D[5].X = PointsOut2D[0].X;
            PointsOut2D[5].Y = m_fhY1;
        }

        public override void Calc_Coord3D()
        {
            float y_a = (float)Math.Tan(m_fSlope_rad) * Ft;
            float y_c = (m_fhY2 - m_fhY1) * (float)Math.Sin(m_fSlope_rad);
            float z_c = (m_fhY2 - m_fhY1) * (float)Math.Cos(m_fSlope_rad);

            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0;

            arrPoints3D[1].X = m_fbX;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = arrPoints3D[1].X;
            arrPoints3D[2].Y = m_fhY1;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = m_fhY1 + y_c;
            arrPoints3D[3].Z = z_c;

            arrPoints3D[4].X = 0;
            arrPoints3D[4].Y = arrPoints3D[3].Y;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = 0;
            arrPoints3D[5].Y = arrPoints3D[2].Y;
            arrPoints3D[5].Z = arrPoints3D[2].Z;

            // Second layer
            float y_b = Ft / (float)Math.Cos(m_fSlope_rad);
            float y_d = Ft * (float)Math.Cos(m_fSlope_rad);
            float z_d = Ft * (float)Math.Sin(m_fSlope_rad);

            arrPoints3D[6].X = arrPoints3D[0].X;
            arrPoints3D[6].Y = arrPoints3D[0].Y;
            arrPoints3D[6].Z = arrPoints3D[0].Z + Ft;

            arrPoints3D[7].X = arrPoints3D[1].X;
            arrPoints3D[7].Y = arrPoints3D[1].Y;
            arrPoints3D[7].Z = arrPoints3D[1].Z + Ft;

            arrPoints3D[8].X = arrPoints3D[7].X;
            arrPoints3D[8].Y = arrPoints3D[2].Y + y_a - y_b;
            arrPoints3D[8].Z = arrPoints3D[7].Z;

            arrPoints3D[9].X = arrPoints3D[3].X;
            arrPoints3D[9].Y = arrPoints3D[3].Y - y_d;
            arrPoints3D[9].Z = arrPoints3D[3].Z + z_d;

            arrPoints3D[10].X = arrPoints3D[4].X;
            arrPoints3D[10].Y = arrPoints3D[9].Y;
            arrPoints3D[10].Z = arrPoints3D[9].Z;

            arrPoints3D[11].X = arrPoints3D[6].X;
            arrPoints3D[11].Y = arrPoints3D[8].Y;
            arrPoints3D[11].Z = arrPoints3D[8].Z;
        }

        void Calc_HolesControlPointsCoord3D(CScrewArrangement_H screwArrangement)
        {
            e_min_x_BottomLeg = screwArrangement.fx_edge_BottomLeg;
            e_min_y_BottomLeg = screwArrangement.fy_edge_BottomLeg;

            e_min_x_TopLeg = screwArrangement.fx_edge_TopLeg;
            e_min_y_TopLeg = screwArrangement.fy_edge_TopLeg;

            float fScrewOffset = screwArrangement.referenceScrew.T_ht_headTotalThickness;

            // Bottom leg
            Point3D[] bottomLegPoints = GetHolesControlPointsCoord3D_RectSequence(
                screwArrangement.iColumns_BottomLeg,
                screwArrangement.iRows_BottomLeg,
                screwArrangement.fx_edge_BottomLeg,
                screwArrangement.fy_edge_BottomLeg,
                screwArrangement.fx_spacing_BottomLeg,
                screwArrangement.fy_spacing_BottomLeg,
                fScrewOffset,
                false);

            // Top leg
            Point3D[] topLegPoints = GetHolesControlPointsCoord3D_RectSequence(
                screwArrangement.iColumns_TopLeg,
                screwArrangement.iRows_TopLeg,
                screwArrangement.fx_edge_TopLeg,
                screwArrangement.fy_edge_TopLeg,
                screwArrangement.fx_spacing_TopLeg,
                screwArrangement.fy_spacing_TopLeg,
                fScrewOffset,
                true);

            // Merge arrays
            bottomLegPoints.CopyTo(arrConnectorControlPoints3D, 0);
            topLegPoints.CopyTo(arrConnectorControlPoints3D, bottomLegPoints.Length);
        }

        void GenerateConnectors(CScrewArrangement_H screwArrangement, bool bChangeRotationAngle_MirroredPlate)
        {
            if (screwArrangement.IHolesNumber > 0)
            {
                screwArrangement.Screws = new CScrew[screwArrangement.IHolesNumber];

                int iNumberOfScrewsInBottomPart = screwArrangement.iColumns_BottomLeg * screwArrangement.iRows_BottomLeg; // TODO - umoznit nastavovat dynamicky

                for (int i = 0; i < screwArrangement.IHolesNumber; i++)
                {
                    if (i < iNumberOfScrewsInBottomPart) // Bottom Part
                    {
                        Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);
                        screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, 90, 0);
                    }
                    else // Top Part
                    {
                        Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);
                        screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, m_fSlope_rad * 180 / MATH.MathF.fPI, 90);
                    }
                }
            }
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            if (iLeftRightIndex == 0) // Left
            {
                // Front Side / Forehead
                AddRectangleIndices_CCW_1234(TriangleIndices, 6, 7, 8, 11);
                AddRectangleIndices_CCW_1234(TriangleIndices, 11, 8, 9, 10);

                // Back Side
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 5);
                AddRectangleIndices_CW_1234(TriangleIndices, 5, 2, 3, 4);

                // Shell Surface
                AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 7, 6);
                AddRectangleIndices_CCW_1234(TriangleIndices, 1, 2, 8, 7);
                AddRectangleIndices_CCW_1234(TriangleIndices, 2, 3, 9, 8);
                AddRectangleIndices_CCW_1234(TriangleIndices, 3, 4, 10, 9);
                AddRectangleIndices_CCW_1234(TriangleIndices, 0, 6, 11, 5);
                AddRectangleIndices_CCW_1234(TriangleIndices, 4, 5, 11, 10);
            }
            else if (iLeftRightIndex == 1) // Right
            {
                // Front Side / Forehead
                AddRectangleIndices_CW_1234(TriangleIndices, 6, 7, 8, 11);
                AddRectangleIndices_CW_1234(TriangleIndices, 11, 8, 9, 10);

                // Back Side
                AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 2, 5);
                AddRectangleIndices_CCW_1234(TriangleIndices, 5, 2, 3, 4);

                // Shell Surface
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 7, 6);
                AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 8, 7);
                AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 9, 8);
                AddRectangleIndices_CW_1234(TriangleIndices, 3, 4, 10, 9);
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 6, 11, 5);
                AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 11, 10);
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

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[11]);

            return wireFrame;
        }

        Point3D[] GetHolesControlPointsCoord3D_RectSequence(int iColumns, int iRows, float fx_edge, float fy_edge, float fx_spacing, float fy_spacing, float fScrewOffset, bool bIsTopLeg)
        {
            Point3D[] arrayControlPoints3D = new Point3D[iColumns * iRows];

            // Horna cast - pomocne hodnoty
            float y_a = (float)Math.Tan(m_fSlope_rad) * Ft;
            float y_b = Ft / (float)Math.Cos(m_fSlope_rad);
            float coordinateY = m_fhY1 + y_a - y_b;
            float fOffset_y = fScrewOffset * (float)Math.Cos(m_fSlope_rad);
            float fOffset_z = fScrewOffset * (float)Math.Sin(m_fSlope_rad);

            for (int i = 0; i < iColumns; i++)
            {
                for (int j = 0; j < iRows; j++)
                {
                    arrayControlPoints3D[i * iRows + j].X = fx_edge + i * fx_spacing;

                    if (!bIsTopLeg) // Kolma spodna cast
                    {
                        arrayControlPoints3D[i * iRows + j].Y = fy_edge + j * fy_spacing;
                        arrayControlPoints3D[i * iRows + j].Z = Ft + fScrewOffset;
                    }
                    else // Horna cast
                    {
                        float fDelta_y = (fy_edge + j * fy_spacing) * (float)Math.Sin(m_fSlope_rad);
                        float fDelta_z = (fy_edge + j * fy_spacing) * (float)Math.Cos(m_fSlope_rad);

                        arrayControlPoints3D[i * iRows + j].Y = coordinateY + fDelta_y - fOffset_y;
                        arrayControlPoints3D[i * iRows + j].Z = Ft + fDelta_z + fOffset_z;
                    }
                }
            }

            return arrayControlPoints3D;
        }
    }
}
