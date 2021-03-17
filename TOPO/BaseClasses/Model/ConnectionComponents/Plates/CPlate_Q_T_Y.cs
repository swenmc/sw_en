﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using _3DTools;

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
        }

        public CConCom_Plate_Q_T_Y(string sName_temp, Point3D controlpoint, float fbX_temp, float fhY_temp, float fl_Z1_temp, float fl_Z2_temp, float ft_platethickness, int iHolesNumber)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_Y;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;

            ControlPoint = controlpoint;
            m_fbX = fbX_temp;
            m_fhY = fhY_temp;
            m_flZ1 = fl_Z1_temp;
            m_flZ2 = fl_Z2_temp;
            Ft = ft_platethickness;
            m_iHolesNumber = iHolesNumber = 0; // Zatial nepodporujeme otvory

            // ScrewArrangement is not implemented
            UpdatePlateData(null);
        }

        public CConCom_Plate_Q_T_Y(string sName_temp, Point3D controlpoint, float fbX_temp, float fhY_temp, float fl_Z1_temp, float ft_platethickness, int iHolesNumber)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;

            if (sName_temp == "Q")
                m_ePlateSerieType_FS = ESerieTypePlate.eSerie_Q;
            else if (sName_temp == "T")
                m_ePlateSerieType_FS = ESerieTypePlate.eSerie_T;

            ITotNoPointsin2D = 8;
            ITotNoPointsin3D = 16;

            ControlPoint = controlpoint;
            m_fbX = fbX_temp;
            m_fhY = fhY_temp;
            m_flZ1 = fl_Z1_temp;
            m_flZ2 = m_flZ1; // Same
            Ft = ft_platethickness;
            m_iHolesNumber = iHolesNumber = 0; // Zatial nepodporujeme otvory

            // ScrewArrangement is not implemented
            UpdatePlateData(null);
        }

        //----------------------------------------------------------------------------
        public override void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            if (screwArrangement != null)
            {
                arrConnectorControlPoints3D = new Point3D[screwArrangement.IHolesNumber];
            }

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            if (screwArrangement != null)
            {
               // Not implemented
               //screwArrangement.Calc_HolesCentersCoord2D
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic();

            Set_DimensionPoints2D();

            Set_MemberOutlinePoints2D();

            Set_BendLinesPoints2D();

            if (screwArrangement != null)
            {
                // Not implemented
                //GenerateConnectors(screwArrangement, false);
            }
        }

        public void UpdatePlateData_Basic()
        {
            Width_bx = m_fbX;
            Height_hy = m_fhY;
            //SetFlatedPlateDimensions();

            if (m_ePlateSerieType_FS == ESerieTypePlate.eSerie_Y)
                Width_bx_Stretched = m_flZ1 + m_fbX + m_flZ2;
            else
                Width_bx_Stretched = 2 * m_flZ1 + m_fbX;

            Height_hy_Stretched = m_fhY;

            fArea = MATH.Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            ScrewArrangement = null;

            DrillingRoutePoints = null;
        }
 
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

            arrPoints3D[2].X = m_fbX + 2 * Ft;
            arrPoints3D[2].Y = 0;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = 0;
            arrPoints3D[3].Z = m_flZ2;

            arrPoints3D[4].X = arrPoints3D[3].X;
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

            arrPoints3D[10].X = m_fbX + Ft;
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

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 5;
            Dimensions = new CDimension[iNumberOfDimensions];

            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1], false, true);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2], false, true);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[2], PointsOut2D[3], false, true);
            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[3], false, true, 53);
            Dimensions[4] = new CDimensionLinear(plateCenter, PointsOut2D[3], PointsOut2D[4], false, true);
        }

        public override void Set_BendLinesPoints2D()
        {
            int iNumberOfLines = 2;
            BendLines = new CLine2D[iNumberOfLines];

            BendLines[0] = new CLine2D(PointsOut2D[1], PointsOut2D[6]);
            BendLines[1] = new CLine2D(PointsOut2D[2], PointsOut2D[5]);
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

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_Q_T_Y)
            {
                CConCom_Plate_Q_T_Y refPlate = (CConCom_Plate_Q_T_Y)plate;
                this.m_fbX = refPlate.m_fbX;
                this.m_fhY = refPlate.m_fhY;
                this.m_flZ1 = refPlate.m_flZ1;
                this.m_flZ2 = refPlate.m_flZ2;
                this.m_iHolesNumber = refPlate.m_iHolesNumber;
            }
        }
    }
}
