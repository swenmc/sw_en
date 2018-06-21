using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DTools;
using MATH;

namespace BaseClasses
{
    public class CConCom_Plate_BB_BG : CPlate
    {
        public float m_fbX;
        public float m_fhY;
        public float m_flZ; // Not used in 2D model
        public float m_ft; // Not used in 2D model

        float m_fRotationX_deg = 0;
        float m_fRotationY_deg = 0;
        float m_fRotationZ_deg = 0;

        float m_fDistanceBetweenHoles;

        int iNoPoints2Dfor3D;

        public CConCom_Plate_BB_BG()
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = true;
        }

        public CConCom_Plate_BB_BG(GraphObj.CPoint controlpoint, float fbX_temp, float fhY_temp, float fl_Z_temp, float ft_platethickness, int iHolesNumber_temp, float fHoleDiameter_temp, bool bIsDisplayed)
        {
            eConnComponentType = EConnectionComponentType.ePlate;
            BIsDisplayed = bIsDisplayed;

            m_pControlPoint = controlpoint;
            m_fbX = fbX_temp;
            m_fhY = fhY_temp;
            m_flZ = fl_Z_temp;
            m_ft = ft_platethickness;
            IHolesNumber = iHolesNumber_temp = 2;
            FHoleDiameter = fHoleDiameter_temp;

            m_fDistanceBetweenHoles = 0.5f * m_fhY;
            ITotNoPointsin2D = 8;

            iNoPoints2Dfor3D = ITotNoPointsin2D + IHolesNumber * 4 + IHolesNumber * INumberOfPointsOfHole;
            ITotNoPointsin3D = 2 * iNoPoints2Dfor3D;

            // Create Array - allocate memory
            PointsOut2D = new float[ITotNoPointsin2D, 2];
            arrPoints3D = new Point3D[ITotNoPointsin3D];
            HolesCentersPoints2D = new float[IHolesNumber, 2];

            // Calculate point positions
            Calc_Coord2D();
            Calc_Coord3D();
            Calc_HolesCentersCoord2D();

            // Fill list of indices for drawing of surface
            loadIndices();
        }

        //----------------------------------------------------------------------------
        void Calc_Coord2D()
        {
            PointsOut2D[0, 0] = 0;
            PointsOut2D[0, 1] = 0;

            PointsOut2D[1, 0] = m_flZ;
            PointsOut2D[1, 1] = 0;

            PointsOut2D[2, 0] = PointsOut2D[1, 0] + m_fbX;
            PointsOut2D[2, 1] = 0;

            PointsOut2D[3, 0] = PointsOut2D[2, 0] + m_flZ;
            PointsOut2D[3, 1] = 0;

            PointsOut2D[4, 0] = PointsOut2D[3, 0];
            PointsOut2D[4, 1] = m_fhY;

            PointsOut2D[5, 0] = PointsOut2D[2, 0];
            PointsOut2D[5, 1] = m_fhY;

            PointsOut2D[6, 0] = PointsOut2D[1, 0];
            PointsOut2D[6, 1] = m_fhY;

            PointsOut2D[7, 0] = PointsOut2D[0, 0];
            PointsOut2D[7, 1] = m_fhY;
        }

        void Calc_Coord3D()
        {
            float[,] holesCentersPointsfor3D = new float[IHolesNumber, 2];

            holesCentersPointsfor3D[0, 0] = 0.5f * m_fbX;
            holesCentersPointsfor3D[0, 1] = 0.5f * m_fhY - 0.5f * m_fDistanceBetweenHoles;

            holesCentersPointsfor3D[1, 0] = 0.5f * m_fbX;
            holesCentersPointsfor3D[1, 1] = 0.5f * m_fhY + 0.5f * m_fDistanceBetweenHoles;

            float fradius = 0.5f * FHoleDiameter;
            int iRadiusAngle = 360; // Angle

            // First layer

            arrPoints3D[0].X = 0;
            arrPoints3D[0].Y = 0;
            arrPoints3D[0].Z = m_flZ;

            arrPoints3D[1].X = arrPoints3D[0].X;
            arrPoints3D[1].Y = arrPoints3D[0].Y;
            arrPoints3D[1].Z = 0;

            arrPoints3D[2].X = m_fbX;
            arrPoints3D[2].Y = arrPoints3D[0].Y;
            arrPoints3D[2].Z = arrPoints3D[1].Z;

            arrPoints3D[3].X = arrPoints3D[2].X;
            arrPoints3D[3].Y = arrPoints3D[0].Y;
            arrPoints3D[3].Z = arrPoints3D[0].Z;

            arrPoints3D[4].X = arrPoints3D[3].X;
            arrPoints3D[4].Y = m_fhY;
            arrPoints3D[4].Z = arrPoints3D[3].Z;

            arrPoints3D[5].X = arrPoints3D[2].X;
            arrPoints3D[5].Y = arrPoints3D[4].Y;
            arrPoints3D[5].Z = arrPoints3D[2].Z;

            arrPoints3D[6].X = arrPoints3D[1].X;
            arrPoints3D[6].Y = arrPoints3D[4].Y;
            arrPoints3D[6].Z = arrPoints3D[1].Z;

            arrPoints3D[7].X = arrPoints3D[0].X;
            arrPoints3D[7].Y = arrPoints3D[4].Y;
            arrPoints3D[7].Z = arrPoints3D[0].Z;

            // Points in holes square edges

            arrPoints3D[8].X = holesCentersPointsfor3D[0, 0] - fradius;
            arrPoints3D[8].Y = holesCentersPointsfor3D[0, 1] + fradius;
            arrPoints3D[8].Z = 0;

            arrPoints3D[9].X = holesCentersPointsfor3D[0, 0] - fradius;
            arrPoints3D[9].Y = holesCentersPointsfor3D[0, 1] - fradius;
            arrPoints3D[9].Z = 0;

            arrPoints3D[10].X = holesCentersPointsfor3D[0, 0] + fradius;
            arrPoints3D[10].Y = holesCentersPointsfor3D[0, 1] - fradius;
            arrPoints3D[10].Z = 0;

            arrPoints3D[11].X = holesCentersPointsfor3D[0, 0] + fradius;
            arrPoints3D[11].Y = holesCentersPointsfor3D[0, 1] + fradius;
            arrPoints3D[11].Z = 0;

            arrPoints3D[12].X = holesCentersPointsfor3D[1, 0] - fradius;
            arrPoints3D[12].Y = holesCentersPointsfor3D[1, 1] + fradius;
            arrPoints3D[12].Z = 0;

            arrPoints3D[13].X = holesCentersPointsfor3D[1, 0] - fradius;
            arrPoints3D[13].Y = holesCentersPointsfor3D[1, 1] - fradius;
            arrPoints3D[13].Z = 0;

            arrPoints3D[14].X = holesCentersPointsfor3D[1, 0] + fradius;
            arrPoints3D[14].Y = holesCentersPointsfor3D[1, 1] - fradius;
            arrPoints3D[14].Z = 0;

            arrPoints3D[15].X = holesCentersPointsfor3D[1, 0] + fradius;
            arrPoints3D[15].Y = holesCentersPointsfor3D[1, 1] + fradius;
            arrPoints3D[15].Z = 0;

            // Hole 1 - bottom
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].X = holesCentersPointsfor3D[0, 0] + Geom2D.GetPositionX(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].Y = holesCentersPointsfor3D[0, 1] + Geom2D.GetPositionY_CCW(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i].Z = 0;
            }

            // Hole 2 - upper
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].X = holesCentersPointsfor3D[1, 0] + Geom2D.GetPositionX(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].Y = holesCentersPointsfor3D[1, 1] + Geom2D.GetPositionY_CCW(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].Z = 0;
            }

            // Second layer

            arrPoints3D[iNoPoints2Dfor3D + 0].X = m_ft;
            arrPoints3D[iNoPoints2Dfor3D + 0].Y = 0;
            arrPoints3D[iNoPoints2Dfor3D + 0].Z = m_flZ;

            arrPoints3D[iNoPoints2Dfor3D + 1].X = arrPoints3D[iNoPoints2Dfor3D + 0].X;
            arrPoints3D[iNoPoints2Dfor3D + 1].Y = arrPoints3D[0].Y;
            arrPoints3D[iNoPoints2Dfor3D + 1].Z = m_ft;

            arrPoints3D[iNoPoints2Dfor3D + 2].X = m_fbX - m_ft;
            arrPoints3D[iNoPoints2Dfor3D + 2].Y = arrPoints3D[iNoPoints2Dfor3D + 0].Y;
            arrPoints3D[iNoPoints2Dfor3D + 2].Z = arrPoints3D[iNoPoints2Dfor3D + 1].Z;

            arrPoints3D[iNoPoints2Dfor3D + 3].X = arrPoints3D[iNoPoints2Dfor3D + 2].X;
            arrPoints3D[iNoPoints2Dfor3D + 3].Y = arrPoints3D[iNoPoints2Dfor3D + 0].Y;
            arrPoints3D[iNoPoints2Dfor3D + 3].Z = arrPoints3D[iNoPoints2Dfor3D + 0].Z;

            arrPoints3D[iNoPoints2Dfor3D + 4].X = arrPoints3D[iNoPoints2Dfor3D + 3].X;
            arrPoints3D[iNoPoints2Dfor3D + 4].Y = m_fhY;
            arrPoints3D[iNoPoints2Dfor3D + 4].Z = arrPoints3D[iNoPoints2Dfor3D + 3].Z;

            arrPoints3D[iNoPoints2Dfor3D + 5].X = arrPoints3D[iNoPoints2Dfor3D + 2].X;
            arrPoints3D[iNoPoints2Dfor3D + 5].Y = arrPoints3D[iNoPoints2Dfor3D + 4].Y;
            arrPoints3D[iNoPoints2Dfor3D + 5].Z = arrPoints3D[iNoPoints2Dfor3D + 2].Z;

            arrPoints3D[iNoPoints2Dfor3D + 6].X = arrPoints3D[iNoPoints2Dfor3D + 1].X;
            arrPoints3D[iNoPoints2Dfor3D + 6].Y = arrPoints3D[iNoPoints2Dfor3D + 4].Y;
            arrPoints3D[iNoPoints2Dfor3D + 6].Z = arrPoints3D[iNoPoints2Dfor3D + 1].Z;

            arrPoints3D[iNoPoints2Dfor3D + 7].X = arrPoints3D[iNoPoints2Dfor3D + 0].X;
            arrPoints3D[iNoPoints2Dfor3D + 7].Y = arrPoints3D[iNoPoints2Dfor3D + 4].Y;
            arrPoints3D[iNoPoints2Dfor3D + 7].Z = arrPoints3D[iNoPoints2Dfor3D + 0].Z;

            // Points in holes square edges

            arrPoints3D[iNoPoints2Dfor3D + 8].X = holesCentersPointsfor3D[0, 0] - fradius;
            arrPoints3D[iNoPoints2Dfor3D + 8].Y = holesCentersPointsfor3D[0, 1] + fradius;
            arrPoints3D[iNoPoints2Dfor3D + 8].Z = m_ft;

            arrPoints3D[iNoPoints2Dfor3D + 9].X = holesCentersPointsfor3D[0, 0] - fradius;
            arrPoints3D[iNoPoints2Dfor3D + 9].Y = holesCentersPointsfor3D[0, 1] - fradius;
            arrPoints3D[iNoPoints2Dfor3D + 9].Z = m_ft;

            arrPoints3D[iNoPoints2Dfor3D + 10].X = holesCentersPointsfor3D[0, 0] + fradius;
            arrPoints3D[iNoPoints2Dfor3D + 10].Y = holesCentersPointsfor3D[0, 1] - fradius;
            arrPoints3D[iNoPoints2Dfor3D + 10].Z = m_ft;

            arrPoints3D[iNoPoints2Dfor3D + 11].X = holesCentersPointsfor3D[0, 0] + fradius;
            arrPoints3D[iNoPoints2Dfor3D + 11].Y = holesCentersPointsfor3D[0, 1] + fradius;
            arrPoints3D[iNoPoints2Dfor3D + 11].Z = m_ft;

            arrPoints3D[iNoPoints2Dfor3D + 12].X = holesCentersPointsfor3D[1, 0] - fradius;
            arrPoints3D[iNoPoints2Dfor3D + 12].Y = holesCentersPointsfor3D[1, 1] + fradius;
            arrPoints3D[iNoPoints2Dfor3D + 12].Z = m_ft;

            arrPoints3D[iNoPoints2Dfor3D + 13].X = holesCentersPointsfor3D[1, 0] - fradius;
            arrPoints3D[iNoPoints2Dfor3D + 13].Y = holesCentersPointsfor3D[1, 1] - fradius;
            arrPoints3D[iNoPoints2Dfor3D + 13].Z = m_ft;

            arrPoints3D[iNoPoints2Dfor3D + 14].X = holesCentersPointsfor3D[1, 0] + fradius;
            arrPoints3D[iNoPoints2Dfor3D + 14].Y = holesCentersPointsfor3D[1, 1] - fradius;
            arrPoints3D[iNoPoints2Dfor3D + 14].Z = m_ft;

            arrPoints3D[iNoPoints2Dfor3D + 15].X = holesCentersPointsfor3D[1, 0] + fradius;
            arrPoints3D[iNoPoints2Dfor3D + 15].Y = holesCentersPointsfor3D[1, 1] + fradius;
            arrPoints3D[iNoPoints2Dfor3D + 15].Z = m_ft;

            // Holes 1 - bottom
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i].X = holesCentersPointsfor3D[0, 0] + Geom2D.GetPositionX(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i].Y = holesCentersPointsfor3D[0, 1] + Geom2D.GetPositionY_CCW(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i].Z = m_ft;
            }

            // Hole 2 - upper
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].X = holesCentersPointsfor3D[1, 0] + Geom2D.GetPositionX(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole);     // y
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].Y = holesCentersPointsfor3D[1, 1] + Geom2D.GetPositionY_CCW(fradius, 90 + i * iRadiusAngle / INumberOfPointsOfHole); // z
                arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i].Z = m_ft;
            }
        }

        void Calc_HolesCentersCoord2D()
        {
            HolesCentersPoints2D[0, 0] = m_flZ + 0.5f * m_fbX;
            HolesCentersPoints2D[0, 1] = 0.5f * m_fhY - 0.5f * m_fDistanceBetweenHoles;

            HolesCentersPoints2D[1, 0] = HolesCentersPoints2D[0, 0];
            HolesCentersPoints2D[1, 1] = 0.5f * m_fhY + 0.5f * m_fDistanceBetweenHoles;
        }

        protected override void loadIndices()
        {
            int iNumber_of_quaters = 4;
            int iNumber_of_hole_points = 12;
            int iNoTrianglesInquater = iNumber_of_hole_points / iNumber_of_quaters;

            TriangleIndices = new Int32Collection();

            // First layer
            AddRectangleIndices_CCW_1234(TriangleIndices, 0, 7, 6, 1);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 5, 4, 3);

            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 6, 12, 9);
            AddRectangleIndices_CCW_1234(TriangleIndices, 2, 10, 15, 5);
            AddRectangleIndices_CCW_1234(TriangleIndices, 1, 9, 10, 2);
            AddRectangleIndices_CCW_1234(TriangleIndices, 5, 15, 12, 6);

            // Middle
            AddRectangleIndices_CCW_1234(TriangleIndices, 8, 13, 14, 11);

            // Hole 1 - bottom
            for (short i = 0; i < iNumber_of_quaters; i++)
            {
                for (short j = 0; j < INumberOfPointsOfHole / iNumber_of_quaters; j++)
                {
                    if (((i + 1) * (j + 1)) < iNumber_of_hole_points)
                    {
                        AddTriangleIndices_CCW_123(TriangleIndices, i + ITotNoPointsin2D, ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j, ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j + 1);
                    }
                    else // Last triangle
                    {
                        AddTriangleIndices_CCW_123(TriangleIndices, i + ITotNoPointsin2D, ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j, ITotNoPointsin2D + IHolesNumber * 4);
                    }
                }
            }

            // Hole 2 - upper
            for (short i = 0; i < iNumber_of_quaters; i++)
            {
                for (short j = 0; j < INumberOfPointsOfHole / iNumber_of_quaters; j++)
                {
                    if (((i+1) * (j+1)) < iNumber_of_hole_points)
                    {
                        AddTriangleIndices_CCW_123(TriangleIndices, 4 + i + ITotNoPointsin2D, INumberOfPointsOfHole + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j, INumberOfPointsOfHole + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j + 1);
                    }
                    else // Last triangle
                    {
                        AddTriangleIndices_CCW_123(TriangleIndices, 4 + i + ITotNoPointsin2D, INumberOfPointsOfHole + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j, INumberOfPointsOfHole + ITotNoPointsin2D + IHolesNumber * 4);
                    }
                }
            }

            // Second layer
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D + 0, iNoPoints2Dfor3D + 7, iNoPoints2Dfor3D + 6, iNoPoints2Dfor3D + 1);
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D + 2, iNoPoints2Dfor3D + 5, iNoPoints2Dfor3D + 4, iNoPoints2Dfor3D + 3);

            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D +1, iNoPoints2Dfor3D + 6, iNoPoints2Dfor3D + 12, iNoPoints2Dfor3D + 9);
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D +2, iNoPoints2Dfor3D + 10, iNoPoints2Dfor3D + 15, iNoPoints2Dfor3D + 5);
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D +1, iNoPoints2Dfor3D + 9, iNoPoints2Dfor3D + 10, iNoPoints2Dfor3D + 2);
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D + 5, iNoPoints2Dfor3D + 15, iNoPoints2Dfor3D + 12, iNoPoints2Dfor3D + 6);

            // Middle
            AddRectangleIndices_CW_1234(TriangleIndices, iNoPoints2Dfor3D + 8, iNoPoints2Dfor3D + 13, iNoPoints2Dfor3D + 14, iNoPoints2Dfor3D + 11);

            // Hole 1 - bottom
            for (short i = 0; i < iNumber_of_quaters; i++)
            {
                for (short j = 0; j < INumberOfPointsOfHole / iNumber_of_quaters; j++)
                {
                    if (((i + 1) * (j + 1)) < iNumber_of_hole_points)
                    {
                        AddTriangleIndices_CW_123(TriangleIndices, iNoPoints2Dfor3D + i + ITotNoPointsin2D, iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j, iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j + 1);
                    }
                    else // Last triangle
                    {
                        AddTriangleIndices_CW_123(TriangleIndices, iNoPoints2Dfor3D + i + ITotNoPointsin2D, iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j, iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4);
                    }
                }
            }

            // Hole 2 - upper
            for (short i = 0; i < iNumber_of_quaters; i++)
            {
                for (short j = 0; j < INumberOfPointsOfHole / iNumber_of_quaters; j++)
                {
                    if (((i + 1) * (j + 1)) < iNumber_of_hole_points)
                    {
                        AddTriangleIndices_CW_123(TriangleIndices, iNoPoints2Dfor3D + 4 + i + ITotNoPointsin2D, iNoPoints2Dfor3D + INumberOfPointsOfHole + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j, iNoPoints2Dfor3D + INumberOfPointsOfHole + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j + 1);
                    }
                    else // Last triangle
                    {
                        AddTriangleIndices_CW_123(TriangleIndices, iNoPoints2Dfor3D + 4 + i + ITotNoPointsin2D, iNoPoints2Dfor3D + INumberOfPointsOfHole + ITotNoPointsin2D + IHolesNumber * 4 + i * iNoTrianglesInquater + j, iNoPoints2Dfor3D + INumberOfPointsOfHole + ITotNoPointsin2D + IHolesNumber * 4);
                    }
                }
            }

            // Between first and second layer

            // Holes

            // Bottom

            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < (INumberOfPointsOfHole-1))
                {
                    AddRectangleIndices_CCW_1234(TriangleIndices, ITotNoPointsin2D + 4 * IHolesNumber + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * IHolesNumber + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * IHolesNumber + i +1, ITotNoPointsin2D + 4 * IHolesNumber + i + 1);
                }
                else // Last rectangle
                {
                    AddRectangleIndices_CCW_1234(TriangleIndices, ITotNoPointsin2D + 4 * IHolesNumber + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * IHolesNumber + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * IHolesNumber, ITotNoPointsin2D + 4 * IHolesNumber);
                }
            }

            // Upper


            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < (INumberOfPointsOfHole - 1))
                {
                    AddRectangleIndices_CCW_1234(TriangleIndices, ITotNoPointsin2D + 4 * IHolesNumber + INumberOfPointsOfHole + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * IHolesNumber + INumberOfPointsOfHole + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * IHolesNumber + INumberOfPointsOfHole + i +1, ITotNoPointsin2D + 4 * IHolesNumber + INumberOfPointsOfHole + i + 1);
                }
                else // Last rectangle
                {
                    AddRectangleIndices_CCW_1234(TriangleIndices, ITotNoPointsin2D + 4 * IHolesNumber +  INumberOfPointsOfHole + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * IHolesNumber + INumberOfPointsOfHole + i, iNoPoints2Dfor3D + ITotNoPointsin2D + 4 * IHolesNumber + INumberOfPointsOfHole, ITotNoPointsin2D + 4 * IHolesNumber + INumberOfPointsOfHole);
                }
            }


            for (int i = 0; i < ITotNoPointsin2D; i++)
            {
                if (i < (ITotNoPointsin2D - 1))
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, i + 1, iNoPoints2Dfor3D + i + 1, iNoPoints2Dfor3D + i);
                else
                    AddRectangleIndices_CCW_1234(TriangleIndices, i, 0, iNoPoints2Dfor3D, iNoPoints2Dfor3D + i);
            }
        }

        protected override Point3DCollection GetDefinitionPoints()
        {
            Point3DCollection pMeshPositions = new Point3DCollection();

            foreach (Point3D point in arrPoints3D)
                pMeshPositions.Add(point);

            return pMeshPositions;
        }

        public override GeometryModel3D CreateGeomModel3D(SolidColorBrush brush)
        {
            GeometryModel3D model = new GeometryModel3D();

            // All in one mesh
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();
            mesh.Positions = GetDefinitionPoints();

            // Add Positions of plate edge nodes
            loadIndices();
            mesh.TriangleIndices = TriangleIndices;

            model.Geometry = mesh;

            brush.Opacity = 0.4;

            DiffuseMaterial qDiffTrans = new DiffuseMaterial(brush);
            SpecularMaterial qSpecTrans = new SpecularMaterial(brush, 90.0);

            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTrans);
            qOuterMaterial.Children.Add(qSpecTrans);

            model.Material = qOuterMaterial;  // Set Model Material

            TransformPlateCoord(model, m_fRotationX_deg, m_fRotationY_deg, m_fRotationZ_deg); // Not used now

            return model;
        }

        public override ScreenSpaceLines3D CreateWireFrameModel()
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();

            wireFrame.Color = Colors.Yellow;
            wireFrame.Thickness = 1;

            // y = 0
            wireFrame.Points.Add(arrPoints3D[0]);
            wireFrame.Points.Add(arrPoints3D[1]);

            wireFrame.Points.Add(arrPoints3D[1]);
            wireFrame.Points.Add(arrPoints3D[2]);

            wireFrame.Points.Add(arrPoints3D[2]);
            wireFrame.Points.Add(arrPoints3D[3]);

            wireFrame.Points.Add(arrPoints3D[3]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 3]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 3]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 2]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 2]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 1]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 1]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 0]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 0]);
            wireFrame.Points.Add(arrPoints3D[0]);

            // y = m_fhY
            wireFrame.Points.Add(arrPoints3D[7]);
            wireFrame.Points.Add(arrPoints3D[6]);

            wireFrame.Points.Add(arrPoints3D[6]);
            wireFrame.Points.Add(arrPoints3D[5]);

            wireFrame.Points.Add(arrPoints3D[5]);
            wireFrame.Points.Add(arrPoints3D[4]);

            wireFrame.Points.Add(arrPoints3D[4]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 4]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 4]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 5]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 5]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 6]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 6]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 7]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 7]);
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

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 0]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 7]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 1]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 6]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 2]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 5]);

            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 3]);
            wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + 4]);

            // Holes

            // Bottom
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < INumberOfPointsOfHole - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i]);
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i + 1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i]);
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4]);
                }
            }

            // Upper
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < INumberOfPointsOfHole - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i + 1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                    wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole]);
                }
            }

            // Bottom
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < INumberOfPointsOfHole - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i]);
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i+1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i]);
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4]);
                }
            }

            // Upper
            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                if (i < INumberOfPointsOfHole - 1)
                {
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i+1]);
                }
                else
                {
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                    wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole]);
                }
            }

            // Lateral holes

            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {
                wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + i]);
                wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + i]);
            }

            for (short i = 0; i < INumberOfPointsOfHole; i++)
            {

                wireFrame.Points.Add(arrPoints3D[ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i]);
                wireFrame.Points.Add(arrPoints3D[iNoPoints2Dfor3D + ITotNoPointsin2D + IHolesNumber * 4 + INumberOfPointsOfHole + i]);

            }

            return wireFrame;
        }
    }
}
