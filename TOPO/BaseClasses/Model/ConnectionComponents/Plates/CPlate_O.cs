using _3DTools;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_O : CPlate
    {
        private float m_fbX1; // Bottom - Width of part connected to colum

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

        private float m_fbX2;  // Top Width of part connected to rafter

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

        private float m_fhY1; // Part of bX1

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

        public CConCom_Plate_O()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_O;
        }

        public CConCom_Plate_O(string sName_temp,
            Point3D controlpoint,
            float fbX1_temp,
            float fbX2_temp,
            float fhY1_temp,
            float fhY2_temp,
            float ft_platethickness,
            float fslope_rad_temp,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_O screwArrangement)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_O;

            ITotNoPointsin2D = 6;
            INoPoints2Dfor3D = 8;
            ITotNoPointsin3D = 14;

            ControlPoint = controlpoint;
            m_fbX1 = fbX1_temp;
            m_fbX2 = fbX2_temp;
            m_fhY1 = fhY1_temp;
            m_fhY2 = fhY2_temp;
            Ft = ft_platethickness;
            FSlope_rad = fslope_rad_temp;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            UpdatePlateData(screwArrangement);
        }

        public override void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            m_alpha1_rad = (float)Math.Atan((m_fhY2 - m_fhY1) / (0.5f * (m_fbX1 - m_fbX2)));

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            if (screwArrangement != null)
            {
                arrConnectorControlPoints3D = new Point3D[screwArrangement.IHolesNumber];
            }

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();

            if (screwArrangement != null)
            {
                screwArrangement.Calc_FacePlateData(m_fbX1, m_fbX2, m_fhY1, Ft);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            Set_DimensionPoints2D();

            Set_BendLinesPoints2D();
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            Width_bx = m_fbX1;
            Height_hy = m_fhY1;
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = m_fbX1;
            Height_hy_Stretched = m_fhY2;
            fArea = Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY1);
            int iNumberOfScrewsInSection = 6; // TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;

            if (screwArrangement != null)
            {
                fA_n -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fA_v_zv = Get_A_rect(Ft, m_fhY1);

            fA_vn_zv = fA_v_zv;

            if (screwArrangement != null)
            {
                fA_vn_zv -= iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fI_yu = Get_I_yu_rect(Ft, m_fhY1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY1); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_fbX1;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = PointsOut2D[1].X;
            PointsOut2D[2].Y = m_fhY1;

            PointsOut2D[3].X = PointsOut2D[2].X - 0.5f * (m_fbX1 - m_fbX2);
            PointsOut2D[3].Y = m_fhY2;

            PointsOut2D[4].X = 0.5f * (m_fbX1 - m_fbX2);
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

            arrPoints3D[1].X = m_fbX1;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = arrPoints3D[1].X;
            arrPoints3D[2].Y = m_fhY1;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[2].Y + y_a;
            arrPoints3D[3].Z = Ft;

            arrPoints3D[4].X = arrPoints3D[3].X - 0.5f * (m_fbX1 - m_fbX2);
            arrPoints3D[4].Y = m_fhY1 + y_c;
            arrPoints3D[4].Z = z_c;

            arrPoints3D[5].X = 0.5f * (m_fbX1 - m_fbX2);
            arrPoints3D[5].Y = arrPoints3D[4].Y;
            arrPoints3D[5].Z = arrPoints3D[4].Z;

            arrPoints3D[6].X = 0;
            arrPoints3D[6].Y = arrPoints3D[3].Y;
            arrPoints3D[6].Z = arrPoints3D[3].Z;

            arrPoints3D[7].X = 0;
            arrPoints3D[7].Y = arrPoints3D[2].Y;
            arrPoints3D[7].Z = arrPoints3D[2].Z;

            // Second layer

            float y_b = Ft / (float)Math.Cos(m_fSlope_rad);
            float y_d = Ft * (float)Math.Cos(m_fSlope_rad);
            float z_d = Ft * (float)Math.Sin(m_fSlope_rad);

            arrPoints3D[8].X = arrPoints3D[0].X;
            arrPoints3D[8].Y = arrPoints3D[0].Y;
            arrPoints3D[8].Z = arrPoints3D[0].Z + Ft;

            arrPoints3D[9].X = arrPoints3D[1].X;
            arrPoints3D[9].Y = arrPoints3D[1].Y;
            arrPoints3D[9].Z = arrPoints3D[1].Z + Ft;

            arrPoints3D[10].X = arrPoints3D[9].X;
            arrPoints3D[10].Y = arrPoints3D[3].Y - y_b;
            arrPoints3D[10].Z = arrPoints3D[9].Z;

            arrPoints3D[11].X = arrPoints3D[4].X;
            arrPoints3D[11].Y = arrPoints3D[4].Y - y_d;
            arrPoints3D[11].Z = arrPoints3D[4].Z + z_d;

            arrPoints3D[12].X = arrPoints3D[5].X;
            arrPoints3D[12].Y = arrPoints3D[11].Y;
            arrPoints3D[12].Z = arrPoints3D[11].Z;

            arrPoints3D[13].X = arrPoints3D[8].X;
            arrPoints3D[13].Y = arrPoints3D[10].Y;
            arrPoints3D[13].Z = arrPoints3D[10].Z;
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 4;
            Dimensions = new CDimension[iNumberOfDimensions + 1];
            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1]);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2]);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[4], true, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[0], new Point(PointsOut2D[0].X, PointsOut2D[4].Y), true, true);

            Dimensions[4] = new CDimensionArc(plateCenter, PointsOut2D[2], PointsOut2D[4], PointsOut2D[5]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 10, 13);
            AddRectangleIndices_CCW_1234(TriangleIndices, 13, 10, 11, 12);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 7);
            AddHexagonIndices_CW_123456(TriangleIndices, 2, 3, 4, 5, 6, 7);

            // Shell Surface
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 8, 6, 7);
            AddRectangleIndices_CCW_1234(TriangleIndices, 13, 12, 5, 6);
            AddRectangleIndices_CCW_1234(TriangleIndices, 12, 11, 4, 5);
            AddRectangleIndices_CCW_1234(TriangleIndices, 11, 10, 3, 4);
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 1, 2, 3);
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 1, 9, 8);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // Front Side
            for (int i = 0; i < ITotNoPointsin2D; i++)
            {
                if (i < (ITotNoPointsin2D) - 1)
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

            // Back Side
            for (int i = 0; i < INoPoints2Dfor3D; i++)
            {
                if (i < INoPoints2Dfor3D - 1)
                {
                    pi = arrPoints3D[i];
                    pj = arrPoints3D[i+1];
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

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[8]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[12]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[13]);

            return wireFrame;
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_O)
            {
                CConCom_Plate_O refPlate = (CConCom_Plate_O)plate;
                this.m_fbX1 = refPlate.m_fbX1;
                this.m_fbX2 = refPlate.m_fbX2;
                this.m_fhY1 = refPlate.m_fhY1;
                this.m_fhY2 = refPlate.m_fhY2;
                this.m_fSlope_rad = refPlate.m_fSlope_rad;
                this.m_alpha1_rad = refPlate.m_alpha1_rad;
                this.m_iHolesNumber = refPlate.m_iHolesNumber;
            }
        }
    }
}
