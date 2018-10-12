using _3DTools;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;

namespace BaseClasses
{
    public class CConCom_Plate_KE : CPlate
    {
        private float m_fbXR; // Rafter Width

        public float Fb_XR
        {
            get
            {
                return m_fbXR;
            }

            set
            {
                m_fbXR = value;
            }
        }

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

        private float m_fhY1;

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

        private float m_flZ;

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

        public CConCom_Plate_KE()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
            BIsDisplayed = true;
        }

        public CConCom_Plate_KE(string sName_temp,
            GraphObj.CPoint controlpoint,
            float fb_R_temp,
            float fb_1_temp,
            float fh_1_temp,
            float fb_2_temp,
            float fh_2_temp,
            float fl_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement screwArrangement,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 12;
            INoPoints2Dfor3D = 16;
            ITotNoPointsin3D = 30;

            m_pControlPoint = controlpoint;
            m_fbXR = fb_R_temp;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
            m_fhY2 = fh_2_temp;
            m_flZ = fl_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            UpdatePlateData(screwArrangement);
        }

        public override void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            // TODO - screw arrangement pre tento plech nie je implementovany!!!! (zdvojene usporiadanie pre obe strany)

            if(MathF.d_equal(m_fSlope_rad, 0))
               m_fSlope_rad = (float)Math.Atan((Fh_Y2 - Fh_Y1) / Fb_X2);
            else
                Fh_Y2 = Fh_Y1 + ((float)Math.Tan(m_fSlope_rad) * Fb_X2);

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            Set_DimensionPoints2D();
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            fWidth_bx = Math.Max(m_fbX1, m_fbX2);
            fHeight_hy = Math.Max(m_fhY1, m_fhY2);
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            // Priblizne predpoklad ze 2 * mflZ = m_fbXR
            fA_g = Get_A_channel(Math.Min(2f * m_flZ, m_fbXR), 2 * Ft, Ft, m_fbX1);
            int iNumberOfScrewsInSection = 8; // TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;

            if (screwArrangement != null)
            {
                fA_n -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fA_v_zv = Get_A_rect(2 * Ft, m_fbX1);

            fA_vn_zv = fA_v_zv;

            if (screwArrangement != null)
            {
                fA_vn_zv -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fI_yu = Get_I_yu_channel(m_flZ, Ft, Ft, m_fbX1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fbX1); // Elastic section modulus
            // TODO - Priblizne predpoklad ze 2 * mflZ = m_fbXR
            fI_yu = Get_I_yu_channel(Math.Min(2f * m_flZ, m_fbXR), Ft, Ft, m_fbX1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fbX1); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp = m_flZ * (float)Math.Cos(fBeta);
            float fy_temp = m_flZ * (float)Math.Sin(fBeta);

            PointsOut2D[0].X = 0.5f * m_fbXR;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = 0.5f * m_fbXR + m_fbX1;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = 0.5f * m_fbXR + m_fbX1 + fx_temp;
            PointsOut2D[2].Y = - fy_temp;

            PointsOut2D[3].X = 0.5f * m_fbXR + m_fbX2 + fx_temp;
            PointsOut2D[3].Y = m_fhY2 - fy_temp;

            PointsOut2D[4].X = 0.5f * m_fbXR + m_fbX2;
            PointsOut2D[4].Y = m_fhY2;

            PointsOut2D[5].X = 0.5f * m_fbXR;
            PointsOut2D[5].Y = m_fhY1;

            // Copy - symmetry about y-axis
            for(int i = 0; i < ITotNoPointsin2D / 2; i++)
            {
                PointsOut2D[ITotNoPointsin2D - i - 1].X = -PointsOut2D[i].X;  // Change sign - Negative coordinates "x"
                PointsOut2D[ITotNoPointsin2D - i - 1].Y = PointsOut2D[i].Y;
            }
        }

        void Calc_Coord3D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp2 = Ft * (float)Math.Cos(fBeta);
            float fy_temp2 = Ft * (float)Math.Sin(fBeta);

            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0.5f * m_fbXR;

            arrPoints3D[1].X = m_fbX1;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = arrPoints3D[0].Z;

            arrPoints3D[2].X = m_fbX1 + fx_temp2;
            arrPoints3D[2].Y = -fy_temp2;
            arrPoints3D[2].Z = arrPoints3D[0].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[2].Y;
            arrPoints3D[3].Z = arrPoints3D[0].Z + m_flZ;

            arrPoints3D[4].X = m_fbX2 + fx_temp2;
            arrPoints3D[4].Y = m_fhY2 - fy_temp2;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[4].X;
            arrPoints3D[5].Y = arrPoints3D[4].Y;
            arrPoints3D[5].Z = arrPoints3D[0].Z;

            arrPoints3D[6].X = m_fbX2;
            arrPoints3D[6].Y = m_fhY2;
            arrPoints3D[6].Z = arrPoints3D[0].Z;

            arrPoints3D[7].X = arrPoints3D[0].X;
            arrPoints3D[7].Y = m_fhY1;
            arrPoints3D[7].Z = arrPoints3D[0].Z;

            // Copy - symmetry about y-axis
            for (int i = 0; i < INoPoints2Dfor3D / 2; i++)
            {
                arrPoints3D[INoPoints2Dfor3D - i - 1].X = arrPoints3D[i].X;
                arrPoints3D[INoPoints2Dfor3D - i - 1].Y = arrPoints3D[i].Y;
                arrPoints3D[INoPoints2Dfor3D - i - 1].Z = -arrPoints3D[i].Z; // Change sign - Negative coordinates "z"
            }

            // Second layer
            arrPoints3D[INoPoints2Dfor3D + 0].X = -Ft;
            arrPoints3D[INoPoints2Dfor3D + 0].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 0].Z = 0.5f * m_fbXR + Ft;

            arrPoints3D[INoPoints2Dfor3D + 1].X = m_fbX1;
            arrPoints3D[INoPoints2Dfor3D + 1].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 1].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 2].X = arrPoints3D[INoPoints2Dfor3D + 1].X;
            arrPoints3D[INoPoints2Dfor3D + 2].Y = arrPoints3D[INoPoints2Dfor3D + 1].Y;
            arrPoints3D[INoPoints2Dfor3D + 2].Z = 0.5f * m_fbXR + m_flZ;

            arrPoints3D[INoPoints2Dfor3D + 3].X = m_fbX2;
            arrPoints3D[INoPoints2Dfor3D + 3].Y = m_fhY2;
            arrPoints3D[INoPoints2Dfor3D + 3].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 4].X = arrPoints3D[INoPoints2Dfor3D + 3].X;
            arrPoints3D[INoPoints2Dfor3D + 4].Y = arrPoints3D[INoPoints2Dfor3D + 3].Y;
            arrPoints3D[INoPoints2Dfor3D + 4].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 5].X = 0;
            arrPoints3D[INoPoints2Dfor3D + 5].Y = m_fhY1;
            arrPoints3D[INoPoints2Dfor3D + 5].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 6].X = -Ft;
            arrPoints3D[INoPoints2Dfor3D + 6].Y = m_fhY1;
            arrPoints3D[INoPoints2Dfor3D + 6].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            // Copy - symmetry about y-axis
            for (int i = 0; i < 14; i++)
            {
                arrPoints3D[ITotNoPointsin3D - i - 1].X = arrPoints3D[INoPoints2Dfor3D + i].X;
                arrPoints3D[ITotNoPointsin3D - i - 1].Y = arrPoints3D[INoPoints2Dfor3D + i].Y;
                arrPoints3D[ITotNoPointsin3D - i - 1].Z = -arrPoints3D[INoPoints2Dfor3D + i].Z; // Change sign - Negative coordinates "z"
            }
        }

        void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 6;
            Dimensions = new GraphObj.CDimension[iNumberOfDimensions + 1];

            Dimensions[0] = new GraphObj.CDimensionLinear(PointsOut2D[0], PointsOut2D[1]);
            Dimensions[1] = new GraphObj.CDimensionLinear(PointsOut2D[2], PointsOut2D[3]);
            Dimensions[2] = new GraphObj.CDimensionLinear(PointsOut2D[4], PointsOut2D[3], true, false);
            Dimensions[3] = new GraphObj.CDimensionLinear(PointsOut2D[5], PointsOut2D[4], true, false);
            Dimensions[4] = new GraphObj.CDimensionLinear(PointsOut2D[0], PointsOut2D[5], true, true, 100);
            Dimensions[5] = new GraphObj.CDimensionLinear(PointsOut2D[11], PointsOut2D[0]);

            Dimensions[6] = new GraphObj.CDimensionArc(new Point(PointsOut2D[1].X, PointsOut2D[5].Y), PointsOut2D[4], PointsOut2D[5]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Front Side / BackSide
            AddPenthagonIndices_CCW_12345(TriangleIndices, 17, 20, 21, 22,16);
            AddPenthagonIndices_CCW_12345(TriangleIndices, 28,29,23,24,25);

            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 6, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 5, 6);

            AddRectangleIndices_CW_1234(TriangleIndices, 8, 9, 14, 15);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 10, 13, 14);

            AddRectangleIndices_CW_1234(TriangleIndices, 3, 4, 19, 18);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 12, 27, 26);

            // Top Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 19, 6, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 6, 20, 21, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 21, 22, 23, 24);
            AddRectangleIndices_CW_1234(TriangleIndices, 8, 24, 25, 9);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 26, 11, 10);

            // Bottom Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 18, 3, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 16, 17, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 15, 29, 16);
            AddRectangleIndices_CW_1234(TriangleIndices, 14, 28, 29, 15);
            AddRectangleIndices_CW_1234(TriangleIndices, 12, 27, 14, 13);

            // Side Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 10, 11, 12, 13);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 7, 8, 15);
            AddRectangleIndices_CW_1234(TriangleIndices, 16, 29, 23, 22);

            AddRectangleIndices_CW_1234(TriangleIndices, 17, 20, 19, 18);
            AddRectangleIndices_CW_1234(TriangleIndices, 25, 28, 27, 26);

            AddRectangleIndices_CW_1234(TriangleIndices, 3, 18, 19, 4);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 26, 27, 12);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // BackSide
            for (int i = 0; i < INoPoints2Dfor3D; i++)
            {
                if (i < (INoPoints2Dfor3D - 1))
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

            // FrontSide
            for (int i = 0; i < 14; i++)
            {
                if (i < (14) - 1)
                {
                    pi = arrPoints3D[INoPoints2Dfor3D + i];
                    pj = arrPoints3D[INoPoints2Dfor3D + i + 1];
                }
                else // Last line
                {
                    pi = arrPoints3D[INoPoints2Dfor3D + i];
                    pj = arrPoints3D[INoPoints2Dfor3D + 0];
                }

                // Add points
                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[15]);
            
            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[21]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[24]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[18]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[19]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[16]);
            wireFrame.Points.Add(arrPoints3D[22]);

            wireFrame.Points.Add(arrPoints3D[23]);
            wireFrame.Points.Add(arrPoints3D[29]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[26]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[27]);

            wireFrame.Points.Add(arrPoints3D[17]);
            wireFrame.Points.Add(arrPoints3D[20]);

            wireFrame.Points.Add(arrPoints3D[25]);
            wireFrame.Points.Add(arrPoints3D[28]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[17]);

            wireFrame.Points.Add(arrPoints3D[14]);
            wireFrame.Points.Add(arrPoints3D[28]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[25]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[20]);

            return wireFrame;
        }
    }
}
