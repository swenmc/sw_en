﻿using _3DTools;
using BaseClasses.GraphObj;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
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

        private float m_fMainMemberDepth; // Vyska rafteru - potrebne pre urcenie priestoru pre skrutky v lavom ramene plechu

        public float MainMemberDepth
        {
            get
            {
                return m_fMainMemberDepth;
            }

            set
            {
                m_fMainMemberDepth = value;
            }
        }

        private int m_iLeftRightIndex; // plate 0 - left, 1 - right

        public int LeftRightIndex
        {
            get
            {
                return m_iLeftRightIndex;
            }

            set
            {
                m_iLeftRightIndex = value;
            }
        }

        private float m_e_min_x_LeftLeg;

        public float e_min_x_LeftLeg
        {
            get
            {
                return m_e_min_x_LeftLeg;
            }

            set
            {
                m_e_min_x_LeftLeg = value;
            }
        }

        private float m_e_min_y_LeftLeg;

        public float e_min_y_LeftLeg
        {
            get
            {
                return m_e_min_y_LeftLeg;
            }

            set
            {
                m_e_min_y_LeftLeg = value;
            }
        }

        private float m_e_min_z_RightLeg;

        public float e_min_z_RightLeg
        {
            get
            {
                return m_e_min_z_RightLeg;
            }

            set
            {
                m_e_min_z_RightLeg = value;
            }
        }

        private float m_e_min_y_RightLeg;

        public float e_min_y_RightLeg
        {
            get
            {
                return m_e_min_y_RightLeg;
            }

            set
            {
                m_e_min_y_RightLeg = value;
            }
        }

        public CConCom_Plate_G()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_G;
        }

        public CConCom_Plate_G(string sName_temp,
            Point3D controlpoint,
            float fbX1_temp,
            float fbX2_temp,
            float fhY1_temp,
            float fhY2_temp,
            float fl_Z_temp,
            float fMainMemberDepth_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            CScrewArrangement_G screwArrangement)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_G;

            ITotNoPointsin2D = 7;
            ITotNoPointsin3D = 14;

            ControlPoint = controlpoint;
            m_iLeftRightIndex = sName_temp.Substring(4, 2) == "LH" ? 0 : 1; // Side index - 0 - left (original), 1 - right
            m_fbX1 = fbX1_temp;
            m_fbX2 = fbX2_temp;
            m_fhY1 = fhY1_temp;
            m_fhY2 = fhY2_temp;
            m_flZ = fl_Z_temp;
            m_fMainMemberDepth = fMainMemberDepth_temp;
            Ft = ft_platethickness;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            UpdatePlateData(screwArrangement);
        }

        //----------------------------------------------------------------------------
        public override void UpdatePlateData(CScrewArrangement screwArrangement)
        {
            // Create Array - allocate memory
            PointsOut2D = new Point[ITotNoPointsin2D];
            arrPoints3D = new Point3D[ITotNoPointsin3D];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();

            if (screwArrangement != null)
            {
                ((CScrewArrangement_G)screwArrangement).Calc_HolesCentersCoord2D(Fb_X1, Fb_X2, Fh_Y1, Fh_Y2, Fl_Z, MainMemberDepth);
                arrConnectorControlPoints3D = new Point3D[screwArrangement.IHolesNumber];
                Calc_HolesControlPointsCoord3D((CScrewArrangement_G)screwArrangement);
            }

            // Fill list of indices for drawing of surface
            loadIndices();

            UpdatePlateData_Basic(screwArrangement);

            bool bChangeRotationAngle_MirroredPlate = false;

            if (m_iLeftRightIndex % 2 != 0) // Change x-coordinates for odd index (RH)
            {
                bChangeRotationAngle_MirroredPlate = true; // Change rotation angle (about vertical axis Y) of screws in the left leg

                for (int i = 0; i < ITotNoPointsin2D; i++)
                {
                    PointsOut2D[i].X *= -1;
                }

                if (screwArrangement != null)
                {
                    for (int i = 0; i < screwArrangement.IHolesNumber; i++)
                    {
                        screwArrangement.HolesCentersPoints2D[i].X *= -1;
                        arrConnectorControlPoints3D[i].X *= -1;
                    }
                }

                for (int i = 0; i < ITotNoPointsin3D; i++)
                {
                    arrPoints3D[i].X *= -1;
                }
            }

            Set_DimensionPoints2D();

            Set_MemberOutlinePoints2D();

            Set_BendLinesPoints2D();

            if (screwArrangement != null)
            {
                GenerateConnectors((CScrewArrangement_G)screwArrangement, bChangeRotationAngle_MirroredPlate);
            }
        }

        public void UpdatePlateData_Basic(CScrewArrangement screwArrangement)
        {
            Width_bx = m_fbX2;
            Height_hy = m_fhY2;
            //SetFlatedPlateDimensions();
            Width_bx_Stretched = m_fbX2 + m_flZ;
            Height_hy_Stretched = m_fhY2;
            fArea = MATH.Geom2D.PolygonArea(PointsOut2D);
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fMass = GetMassIgnoringHoles();

            fA_g = Get_A_rect(Ft, m_fhY2);
            int iNumberOfScrewsInSection = 8; // TODO, temporary - zavisi na rozmiestneni skrutiek

            fA_n = fA_g;
            if (screwArrangement != null)
            {
                fA_n = fA_g - iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fA_v_zv = Get_A_rect(Ft, m_fhY2);

            fA_vn_zv = fA_v_zv;
            if (screwArrangement != null)
            {
                fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * screwArrangement.referenceScrew.Diameter_thread * Ft;
            }

            fI_yu = Get_I_yu_rect(Ft, m_fhY2);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fhY2); // Elastic section modulus

            ScrewArrangement = screwArrangement;

            DrillingRoutePoints = null;
        }

        public override void Calc_Coord2D()
        {
            PointsOut2D[0].X = 0;
            PointsOut2D[0].Y = 0;

            PointsOut2D[1].X = m_flZ;
            PointsOut2D[1].Y = 0;

            PointsOut2D[2].X = PointsOut2D[1].X + m_fbX2;
            PointsOut2D[2].Y = 0;

            PointsOut2D[3].X = PointsOut2D[2].X;
            PointsOut2D[3].Y = m_fhY1;

            PointsOut2D[4].X = PointsOut2D[1].X + m_fbX1;
            PointsOut2D[4].Y = m_fhY2;

            PointsOut2D[5].X = PointsOut2D[1].X;
            PointsOut2D[5].Y = m_fhY2;

            PointsOut2D[6].X = PointsOut2D[0].X;
            PointsOut2D[6].Y = m_fhY2;
        }

        public override void Calc_Coord3D()
        {
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = 0;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX2;
            arrPoints3D[2].Y = arrPoints3D[1].Y;
            arrPoints3D[2].Z = 0;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = m_fhY1;
            arrPoints3D[3].Z = 0;

            arrPoints3D[4].X = m_fbX1;
            arrPoints3D[4].Y = m_fhY2;
            arrPoints3D[4].Z = 0;

            arrPoints3D[5].X = arrPoints3D[1].X;
            arrPoints3D[5].Y = m_fhY2;
            arrPoints3D[5].Z = 0;

            arrPoints3D[6].X = arrPoints3D[0].X;
            arrPoints3D[6].Y = m_fhY2;
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

        void Calc_HolesControlPointsCoord3D(CScrewArrangement_G screwArrangement)
        {
            e_min_x_LeftLeg = screwArrangement.fx_edge_LeftLeg;
            e_min_y_LeftLeg = screwArrangement.fy_edge_LeftLeg;

            e_min_z_RightLeg = screwArrangement.fx_edge_RightLeg;
            e_min_y_RightLeg = screwArrangement.fy_edge_RightLeg;

            float fScrewOffset = screwArrangement.referenceScrew.T_ht_headTotalThickness;

            // Left leg
            Point3D[] leftLegPoints = GetHolesControlPointsCoord3D_RectSequence(
                screwArrangement.iColumns_LeftLeg,
                screwArrangement.iRows_LeftLeg,
                screwArrangement.fx_edge_LeftLeg,
                screwArrangement.fy_edge_LeftLeg,
                screwArrangement.fx_spacing_LeftLeg,
                screwArrangement.fy_spacing_LeftLeg,
                fScrewOffset,
                true);

            // Right leg
            Point3D[] rightLegPoints = GetHolesControlPointsCoord3D_RectSequence(
                screwArrangement.iColumns_RightLeg,
                screwArrangement.iRows_RightLeg,
                screwArrangement.fx_edge_RightLeg,
                screwArrangement.fy_edge_RightLeg,
                screwArrangement.fx_spacing_RightLeg,
                screwArrangement.fy_spacing_RightLeg,
                fScrewOffset,
                false);

            // Merge arrays
            leftLegPoints.CopyTo(arrConnectorControlPoints3D, 0);
            rightLegPoints.CopyTo(arrConnectorControlPoints3D, leftLegPoints.Length);
        }

        void GenerateConnectors(CScrewArrangement_G screwArrangement, bool bChangeRotationAngle_MirroredPlate)
        {
            if (screwArrangement.IHolesNumber > 0)
            {
                screwArrangement.Screws = new CScrew[screwArrangement.IHolesNumber];

                float fRotationAngleAboutYAxis = 180; // Vertical Axis

                if (bChangeRotationAngle_MirroredPlate)
                    fRotationAngleAboutYAxis = 0;

                int iNumberOfScrewsInLeftLeg = screwArrangement.iColumns_LeftLeg * screwArrangement.iRows_LeftLeg; // TODO - umoznit nastavovat dynamicky

                for (int i = 0; i < screwArrangement.IHolesNumber; i++)
                {
                    if (i < iNumberOfScrewsInLeftLeg) // Left Leg
                    {
                        Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);
                        screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, fRotationAngleAboutYAxis, 0);
                    }
                    else
                    {
                        Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);
                        screwArrangement.Screws[i] = new CScrew(screwArrangement.referenceScrew, controlpoint, 0, 90, 0);
                    }
                }
            }
        }

        public override void Set_DimensionPoints2D()
        {
            int iNumberOfDimensions = 7;
            Dimensions = new CDimension[iNumberOfDimensions];

            Point plateCenter = Drawing2D.CalculateModelCenter(PointsOut2D);

            Dimensions[0] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[1], false, true);
            Dimensions[1] = new CDimensionLinear(plateCenter, PointsOut2D[1], PointsOut2D[2], false, true);
            Dimensions[2] = new CDimensionLinear(plateCenter, PointsOut2D[0], PointsOut2D[2], false, true, 53);

            Dimensions[3] = new CDimensionLinear(plateCenter, PointsOut2D[2], PointsOut2D[3], false, true);
            Dimensions[4] = new CDimensionLinear(plateCenter, PointsOut2D[3], new Point(PointsOut2D[3].X, PointsOut2D[4].Y), false, true);
            Dimensions[5] = new CDimensionLinear(plateCenter, PointsOut2D[2], new Point(PointsOut2D[2].X, PointsOut2D[4].Y), false, true, 53);

            Dimensions[6] = new CDimensionLinear(plateCenter, PointsOut2D[5], PointsOut2D[4], true, true);
        }

        public override void Set_BendLinesPoints2D()
        {
            int iNumberOfLines = 1;
            BendLines = new CLine2D[iNumberOfLines];

            BendLines[0] = new CLine2D(PointsOut2D[1], PointsOut2D[5]);
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            if (m_iLeftRightIndex == 0) // Left
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
            else if (m_iLeftRightIndex == 1) // Right
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

            return wireFrame;
        }

        Point3D[] GetHolesControlPointsCoord3D_RectSequence(int iColumns, int iRows, float fx_edge, float fy_edge, float fx_spacing, float fy_spacing, float fScrewOffset, bool bIsLeftLeg)
        {
            Point3D[] arrayControlPoints3D = new Point3D[iColumns * iRows];

            for (int i = 0; i < iColumns; i++)
            {
                for (int j = 0; j < iRows; j++)
                {
                    if (bIsLeftLeg) // Lava cast
                    {
                        arrayControlPoints3D[i * iRows + j].X = Ft + fScrewOffset;
                        arrayControlPoints3D[i * iRows + j].Y = fy_edge + j * fy_spacing;
                        arrayControlPoints3D[i * iRows + j].Z = fx_edge + i * fx_spacing;
                    }
                    else // Prava cast
                    {
                        arrayControlPoints3D[i * iRows + j].X = fx_edge + i * fx_spacing;
                        arrayControlPoints3D[i * iRows + j].Y = fy_edge + j * fy_spacing;
                        arrayControlPoints3D[i * iRows + j].Z = Ft + fScrewOffset;
                    }
                }
            }

            return arrayControlPoints3D;
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CConCom_Plate_G)
            {
                CConCom_Plate_G refPlate = (CConCom_Plate_G)plate;
                this.m_fbX1 = refPlate.m_fbX1;
                this.m_fbX2 = refPlate.m_fbX2;
                this.m_fhY1 = refPlate.m_fhY1;
                this.m_fhY2 = refPlate.m_fhY2;
                this.m_flZ = refPlate.m_flZ;
                this.m_fMainMemberDepth = refPlate.m_fMainMemberDepth;
                this.m_iLeftRightIndex = refPlate.m_iLeftRightIndex;
                this.m_e_min_x_LeftLeg = refPlate.m_e_min_x_LeftLeg;
                this.m_e_min_y_LeftLeg = refPlate.m_e_min_y_LeftLeg;
                this.m_e_min_z_RightLeg = refPlate.m_e_min_z_RightLeg;
                this.m_e_min_y_RightLeg = refPlate.m_e_min_y_RightLeg;
            }
        }
    }
}