using _3DTools;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CConCom_Plate_Q_T_Y : CPlate
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

        public float m_flZ1; // Not used in 2D model

        public float Fl_Z1
        {
            get
            {
                return m_flZ1;
            }

            set
            {
                m_flZ1 = value;
            }
        }

        public float m_flZ2; // Not used in 2D model

        public float Fl_Z2
        {
            get
            {
                return m_flZ2;
            }

            set
            {
                m_flZ2 = value;
            }
        }

        public int m_iHolesNumber = 0;

        public CConCom_Plate_Q_T_Y()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_Q_T_Y(string sName_temp, GraphObj.CPoint controlpoint, float fbX_temp, float fhY_temp, float fl_Z1_temp, float fl_Z2_temp, float ft_platethickness, int iHolesNumber, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;

            m_pControlPoint = controlpoint;
            m_fbX = fbX_temp;
            m_fhY = fhY_temp;
            m_flZ1 = fl_Z1_temp;
            m_flZ2 = fl_Z2_temp;
            Ft = ft_platethickness;
            m_iHolesNumber = iHolesNumber = 0; // Zatial nepodporujeme otvory

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            // Fill list of indices for drawing of surface
            loadIndices();

            Width_bx = m_fbX;
            Height_hy = m_fhY;
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();
        }

        public CConCom_Plate_Q_T_Y(string sName_temp, GraphObj.CPoint controlpoint, float fbX_temp, float fhY_temp, float fl_Z1_temp, float ft_platethickness, int iHolesNumber, bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;

            m_pControlPoint = controlpoint;
            m_fbX = fbX_temp;
            m_fhY = fhY_temp;
            m_flZ1 = fl_Z1_temp;
            m_flZ2 = m_flZ1; // Same
            Ft = ft_platethickness;
            m_iHolesNumber = iHolesNumber = 0; // Zatial nepodporujeme otvory

            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            // Fill list of indices for drawing of surface
            loadIndices();

            Width_bx = m_fbX;
            Height_hy = m_fhY;
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();
        }

        //----------------------------------------------------------------------------
        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_flZ1;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = PointsOut2D[1].X + m_fbX;
            PointsOut2D[2].Y = 0;

            PointsOut2D[3].X = PointsOut2D[2].X + m_flZ2;
            PointsOut2D[3].Y = 0;

            PointsOut2D[4].X = PointsOut2D[3].X;
            PointsOut2D[4].Y = m_fhY;

            PointsOut2D[5].X = PointsOut2D[2].X;
            PointsOut2D[5].Y = m_fhY;

            PointsOut2D[6].X = PointsOut2D[1].X;
            PointsOut2D[6].Y = m_fhY;

            PointsOut2D[7].X = PointsOut2D[0].X;
            PointsOut2D[7].Y = m_fhY;
        }

        public override void Calc_Coord3D()
        {
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ1;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = m_fbX;
            arrPoints3D[3].Y = 0;
            arrPoints3D[3].Z = m_flZ2;

            arrPoints3D[4].X = m_fbX;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[2].X;
            arrPoints3D[5].Y = m_fhY;
            arrPoints3D[5].Z = arrPoints3D[2].Z;

            arrPoints3D[6].X = arrPoints3D[1].X;
            arrPoints3D[6].Y = m_fhY;
            arrPoints3D[6].Z = arrPoints3D[1].Z;

            arrPoints3D[7].X = arrPoints3D[0].X;
            arrPoints3D[7].Y = m_fhY;
            arrPoints3D[7].Z = arrPoints3D[0].Z;

            arrPoints3D[8].X = Ft;
            arrPoints3D[8].Y = 0;
            arrPoints3D[8].Z = m_flZ1;

            arrPoints3D[9].X = arrPoints3D[8].X;
            arrPoints3D[9].Y = 0;
            arrPoints3D[9].Z = Ft;

            arrPoints3D[10].X = m_fbX - Ft;
            arrPoints3D[10].Y = 0;
            arrPoints3D[10].Z = Ft;

            arrPoints3D[11].X = arrPoints3D[10].X;
            arrPoints3D[11].Y = 0;
            arrPoints3D[11].Z = arrPoints3D[3].Z;

            arrPoints3D[12].X = arrPoints3D[11].X;
            arrPoints3D[12].Y = m_fhY;
            arrPoints3D[12].Z = arrPoints3D[11].Z;

            arrPoints3D[13].X = arrPoints3D[10].X;
            arrPoints3D[13].Y = m_fhY;
            arrPoints3D[13].Z = arrPoints3D[10].Z;

            arrPoints3D[14].X = arrPoints3D[9].X;
            arrPoints3D[14].Y = m_fhY;
            arrPoints3D[14].Z = arrPoints3D[9].Z;

            arrPoints3D[15].X = arrPoints3D[8].X;
            arrPoints3D[15].Y = m_fhY;
            arrPoints3D[15].Z = arrPoints3D[8].Z;
        }

        protected override void loadIndices()
        {
            int secNum = 8;
            TriangleIndices = new Int32Collection();

            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 7, 6, 1);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 6, 5, 2);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 5, 4, 3);
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 9, 14, 15);
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 10, 13, 14);
            AddRectangleIndices_CCW_1234(TriangleIndices, 10, 11, 12, 13);

            // Shell Surface
            DrawCaraLaterals_CW(secNum, TriangleIndices);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            // y = 0
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[1]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[2]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[8]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[0]);

            // y = m_fhY
            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[12]);

            wireFrame.Points.Add(arrPoints3D[12]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[13]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[14]);
            wireFrame.Points.Add(arrPoints3D[15]);

            wireFrame.Points.Add(arrPoints3D[15]);
            wireFrame.Points.Add(arrPoints3D[7]);

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[11]);
            wireFrame.Points.Add(arrPoints3D[12]);

            wireFrame.Points.Add(arrPoints3D[10]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[14]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[15]);

            return wireFrame;
        }
    }
}
