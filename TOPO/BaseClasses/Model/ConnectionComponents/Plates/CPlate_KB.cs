﻿using _3DTools;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConCom_Plate_KB : CPlate
    {
        float m_fbX1;
        float m_fhY1;
        float m_fbX2;
        float m_fhY2;
        float m_flZ;
        float m_ft;
        float m_fSlope_rad;

        float m_fCrscRafterDepth;
        float m_fCrscWebStraightDepth;
        float m_fStiffenerSize; // Middle cross-section stiffener dimension (without screws)
        bool m_bUseAdditionalCornerScrews;
        int m_iAdditionalConnectorNumber;

        public float[] HolesCenterRadii;
        public int INumberOfCircleJoints = 2;

        public CConCom_Plate_KB()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;
            BIsDisplayed = true;
        }

        public CConCom_Plate_KB(string sName_temp,
            GraphObj.CPoint controlpoint,
            float fb_1_temp,
            float fh_1_temp,
            float fb_2_temp,
            float fh_2_temp,
            float fl_temp,
            float ft_platethickness,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            int iHolesNumber,
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            bool bUseAdditionalCornerScrews_temp,
            int iAdditionalConnectorNumber_temp,
            bool bIsDisplayed)
        {
            Name = sName_temp;
            eConnComponentType = EConnectionComponentType.ePlate;
            m_ePlateSerieType_FS = ESerieTypePlate.eSerie_K;

            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 6;
            INoPoints2Dfor3D = 8;
            ITotNoPointsin3D = 14;

            m_pControlPoint = controlpoint;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
            m_fhY2 = fh_2_temp;
            m_flZ = fl_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber;
            referenceScrew = referenceScrew_temp;
            m_fCrscRafterDepth = fCrscRafterDepth_temp;
            m_fCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            m_fStiffenerSize = fStiffenerSize_temp;
            m_bUseAdditionalCornerScrews = bUseAdditionalCornerScrews_temp;
            m_iAdditionalConnectorNumber = iAdditionalConnectorNumber_temp;

            m_fSlope_rad = (float)Math.Atan((fh_2_temp - fh_1_temp) / fb_2_temp);
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber + (m_bUseAdditionalCornerScrews ? m_iAdditionalConnectorNumber : 0), 2];
            HolesCenterRadii = new float[HolesCentersPoints2D.Length / 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber + (m_bUseAdditionalCornerScrews ? m_iAdditionalConnectorNumber : 0)];

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D_KneePlate(
                m_fbX1,
                m_fbX2,
                0, // Distance from the left edge is the same as KA plate (lz is used for KC and KD plates)
                m_fhY1,
                m_fSlope_rad,
                m_bUseAdditionalCornerScrews,
                INumberOfCircleJoints,
                m_iAdditionalConnectorNumber,
                m_fCrscRafterDepth,
                m_fCrscWebStraightDepth,
                m_fStiffenerSize,
                ref HolesCenterRadii);

            Calc_HolesControlPointsCoord3D_ApexOrKneePlate(0, m_ft);

            // Fill list of indices for drawing of surface
            loadIndices();

            GenerateConnectors_ApexOrKneePlate(referenceScrew);

            fWidth_bx = Math.Max(m_fbX1, m_fbX2);
            fHeight_hy = Math.Max(m_fhY1, m_fhY2);
            fThickness_tz = m_ft;
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fWeight = GetWeightIgnoringHoles();

            // Konzervativne, vynechana pasnica
            fA_g = Get_A_rect(m_ft, m_fbX1);
            int iNumberOfScrewsInSection = 4; // TODO, temporary - zavisi na rozmiestneni skrutiek
            fA_n = fA_g - iNumberOfScrewsInSection * referenceScrew.Diameter_thread;
            fA_v_zv = Get_A_rect(m_ft, m_fbX1);
            fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * referenceScrew.Diameter_thread;
            fI_yu = Get_I_yu_rect(m_ft, m_fbX1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fbX1); // Elastic section modulus
        }

        public CConCom_Plate_KB(GraphObj.CPoint controlpoint,
            float fb_1_temp,
            float fh_1_temp,
            float fb_2_temp,
            float fh_2_temp,
            float fl_temp,
            float ft_platethickness,
            float fSLope_rad_temp,
            float fRotation_x_deg,
            float fRotation_y_deg,
            float fRotation_z_deg,
            int iHolesNumber,
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            bool bUseAdditionalCornerScrews_temp,
            int iAdditionalConnectorNumber_temp,
            bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            ITotNoPointsin2D = 6;
            INoPoints2Dfor3D = 8;
            ITotNoPointsin3D = 14;

            m_pControlPoint = controlpoint;
            m_fbX1 = fb_1_temp;
            m_fhY1 = fh_1_temp;
            m_fbX2 = fb_2_temp;
            m_fhY2 = fh_2_temp;
            m_flZ = fl_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber;
            referenceScrew = referenceScrew_temp;
            m_fCrscRafterDepth = fCrscRafterDepth_temp;
            m_fCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            m_fStiffenerSize = fStiffenerSize_temp;
            m_bUseAdditionalCornerScrews = bUseAdditionalCornerScrews_temp;
            m_iAdditionalConnectorNumber = iAdditionalConnectorNumber_temp;

            m_fSlope_rad = fSLope_rad_temp;
            m_fRotationX_deg = fRotation_x_deg;
            m_fRotationY_deg = fRotation_y_deg;
            m_fRotationZ_deg = fRotation_z_deg;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber + (m_bUseAdditionalCornerScrews ? m_iAdditionalConnectorNumber : 0), 2];
            HolesCenterRadii = new float[HolesCentersPoints2D.Length / 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber + (m_bUseAdditionalCornerScrews ? m_iAdditionalConnectorNumber : 0)];

            // Fill Array Data
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D_KneePlate(
                m_fbX1,
                m_fbX2,
                0, // Distance from the left edge is the same as KA plate (lz is used for KC and KD plates)
                m_fhY1,
                m_fSlope_rad,
                m_bUseAdditionalCornerScrews,
                INumberOfCircleJoints,
                m_iAdditionalConnectorNumber,
                m_fCrscRafterDepth,
                m_fCrscWebStraightDepth,
                m_fStiffenerSize,
                ref HolesCenterRadii);

            Calc_HolesControlPointsCoord3D_ApexOrKneePlate(0, m_ft);

            // Fill list of indices for drawing of surface
            loadIndices();

            GenerateConnectors_ApexOrKneePlate(referenceScrew);

            fWidth_bx = Math.Max(m_fbX1, m_fbX2);
            fHeight_hy = Math.Max(m_fhY1, m_fhY2);
            fThickness_tz = m_ft;
            fArea = PolygonArea();
            fCuttingRouteDistance = GetCuttingRouteDistance();
            fSurface = GetSurfaceIgnoringHoles();
            fVolume = GetVolumeIgnoringHoles();
            fWeight = GetWeightIgnoringHoles();

            // Konzervativne, vynechana pasnica
            fA_g = Get_A_rect(m_ft, m_fbX1);
            int iNumberOfScrewsInSection = 4; // TODO, temporary - zavisi na rozmiestneni skrutiek
            fA_n = fA_g - iNumberOfScrewsInSection * referenceScrew.Diameter_thread;
            fA_v_zv = Get_A_rect(m_ft, m_fbX1);
            fA_vn_zv = fA_v_zv - iNumberOfScrewsInSection * referenceScrew.Diameter_thread;
            fI_yu = Get_I_yu_rect(m_ft, m_fbX1);  // Moment of inertia of plate
            fW_el_yu = Get_W_el_yu(fI_yu, m_fbX1); // Elastic section modulus
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp = m_flZ * (float)Math.Cos(fBeta);
            float fy_temp = m_flZ * (float)Math.Sin(fBeta);

            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_fbX1;
            PointsOut2D[1, 1] = 0;

            PointsOut2D[2, 0] = m_fbX1 + fx_temp;
            PointsOut2D[2, 1] = - fy_temp;

            PointsOut2D[3, 0] = m_fbX2 + fx_temp;
            PointsOut2D[3, 1] = m_fhY2 - fy_temp;

            PointsOut2D[4, 0] = m_fbX2;
            PointsOut2D[4, 1] = m_fhY2;

            PointsOut2D[5, 0] = 0;
            PointsOut2D[5, 1] = m_fhY1;
        }

        void Calc_Coord3D()
        {
            float fBeta = (float)Math.Atan((m_fbX2 - m_fbX1) / m_fhY2);
            float fx_temp2 = m_ft * (float)Math.Cos(fBeta);
            float fy_temp2 = m_ft * (float)Math.Sin(fBeta);

            // First layer
            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = 0;

            arrPoints3D[1].X = m_fbX1;
            arrPoints3D[1].Y = 0;
            arrPoints3D[1].Z = arrPoints3D[0].Z;

            arrPoints3D[2].X = m_fbX1 + fx_temp2;
            arrPoints3D[2].Y = -fy_temp2;
            arrPoints3D[2].Z = arrPoints3D[0].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[2].Y;
            arrPoints3D[3].Z = m_flZ;

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

            // Second layer
            arrPoints3D[INoPoints2Dfor3D + 0].X = 0;
            arrPoints3D[INoPoints2Dfor3D + 0].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 0].Z = m_ft;

            arrPoints3D[INoPoints2Dfor3D + 1].X = m_fbX1;
            arrPoints3D[INoPoints2Dfor3D + 1].Y = 0;
            arrPoints3D[INoPoints2Dfor3D + 1].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 2].X = arrPoints3D[INoPoints2Dfor3D + 1].X;
            arrPoints3D[INoPoints2Dfor3D + 2].Y = arrPoints3D[INoPoints2Dfor3D + 1].Y;
            arrPoints3D[INoPoints2Dfor3D + 2].Z = m_flZ;

            arrPoints3D[INoPoints2Dfor3D + 3].X = m_fbX2;
            arrPoints3D[INoPoints2Dfor3D + 3].Y = m_fhY2;
            arrPoints3D[INoPoints2Dfor3D + 3].Z = arrPoints3D[INoPoints2Dfor3D + 2].Z;

            arrPoints3D[INoPoints2Dfor3D + 4].X = arrPoints3D[INoPoints2Dfor3D + 3].X;
            arrPoints3D[INoPoints2Dfor3D + 4].Y = arrPoints3D[INoPoints2Dfor3D + 3].Y;
            arrPoints3D[INoPoints2Dfor3D + 4].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;

            arrPoints3D[INoPoints2Dfor3D + 5].X = arrPoints3D[INoPoints2Dfor3D + 0].X;
            arrPoints3D[INoPoints2Dfor3D + 5].Y = m_fhY1;
            arrPoints3D[INoPoints2Dfor3D + 5].Z = arrPoints3D[INoPoints2Dfor3D + 0].Z;
        }

        protected override void loadIndices()
        {
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndices, 9, 12, 13, 8);
            AddRectangleIndices_CCW_1234(TriangleIndices, 3, 4, 11, 10);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 6, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 5, 6);

            // Top Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 6, 12, 13);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 11, 6, 5);

            // Bottom Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 8, 9, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 10, 3, 2);

            // Side Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 7, 13, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 12, 11, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Color.FromRgb(250, 250, 60);
            wireFrame.Thickness = 1.0;

            Point3D pi = new Point3D();
            Point3D pj = new Point3D();

            // BackSide
            for (int i = 0; i < INoPoints2Dfor3D; i++)
            {
                if (i < (8) - 1)
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

            // Lateral
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[7]);

            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[8]);
            
            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[9]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[10]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[11]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[12]);

            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[8]);
            wireFrame.Points.Add(arrPoints3D[13]);

            wireFrame.Points.Add(arrPoints3D[9]);
            wireFrame.Points.Add(arrPoints3D[12]);

            return wireFrame;
        }
    }
}
