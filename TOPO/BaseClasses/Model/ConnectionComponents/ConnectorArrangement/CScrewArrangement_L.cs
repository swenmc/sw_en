using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_L : CScrewArrangement
    {
        private float m_fecx;

        public float Fecx
        {
            get
            {
                return m_fecx;
            }

            set
            {
                m_fecx = value;
            }
        }

        private float m_fecy;

        public float Fecy
        {
            get
            {
                return m_fecy;
            }

            set
            {
                m_fecy = value;
            }
        }

        private float m_fey;

        public float Fey
        {
            get
            {
                return m_fey;
            }

            set
            {
                m_fey = value;
            }
        }

        private float m_fsy;

        public float Fsy
        {
            get
            {
                return m_fsy;
            }

            set
            {
                m_fsy = value;
            }
        }

        private float m_fszy_left;

        public float Fszy_left
        {
            get
            {
                return m_fszy_left;
            }

            set
            {
                m_fszy_left = value;
            }
        }

        private float m_fszy_right;

        public float Fszy_right
        {
            get
            {
                return m_fszy_right;
            }

            set
            {
                m_fszy_right = value;
            }
        }

        private float m_feoy_left;

        public float Feoy_left
        {
            get
            {
                return m_feoy_left;
            }

            set
            {
                m_feoy_left = value;
            }
        }

        private float m_feoy_right;

        public float Feoy_right
        {
            get
            {
                return m_feoy_right;
            }

            set
            {
                m_feoy_right = value;
            }
        }

        public CScrewArrangement_L() { }

        // Rohove skrutky a jeden stlpec v strede ramena - Plate LH, LI, LK
        public CScrewArrangement_L(
            int iScrewsNumber_temp,
            CScrew referenceScrew_temp,
            float fEdgePositionOfCornerScrews_x,
            float fEdgePositionOfCornerScrews_y,
            float fEdgePositionOfScrews_y,
            float fSpacingOfScrews_y,
            float fLeftLegScrewZone_y,
            float fRightLegScrewZone_y,
            float fLeftLegEdgeOffset_y,
            float fRightLegEdgeOffset_y
            )
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
            m_fecx = fEdgePositionOfCornerScrews_x;
            m_fecy = fEdgePositionOfCornerScrews_y;
            m_fey = fEdgePositionOfScrews_y;
            m_fsy = fSpacingOfScrews_y;
            m_fszy_left = fLeftLegScrewZone_y;
            m_fszy_right = fRightLegScrewZone_y;
            m_feoy_left = fLeftLegEdgeOffset_y;
            m_feoy_right = fRightLegEdgeOffset_y;
        }

        // Len rohove skrutky - Plate LJ
        public CScrewArrangement_L(
            int iScrewsNumber_temp,
            CScrew referenceScrew_temp,
            float fEdgePositionOfCornerScrews_x,
            float fEdgePositionOfCornerScrews_y,
            float fLeftLegScrewZone_y,
            float fRightLegScrewZone_y,
            float fLeftLegEdgeOffset_y,
            float fRightLegEdgeOffset_y
            )
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
            m_fecx = fEdgePositionOfCornerScrews_x;
            m_fecy = fEdgePositionOfCornerScrews_y;
            m_fszy_left = fLeftLegScrewZone_y;
            m_fszy_right = fRightLegScrewZone_y;
            m_feoy_left = fLeftLegEdgeOffset_y;
            m_feoy_right = fRightLegEdgeOffset_y;
            m_fey = 0; // Unused parameter
            m_fsy = 0; // Unused parameter
        }

        public void Calc_HolesCentersCoord2D(float fbX1, float fhY, float flZ)
        {
            float m_e_min_x_LeftLeg = m_fecx; //0.010f; // Left leg
            float m_e_min_z_RightLeg = m_fecx; //0.010f; // Right leg

            float m_e_min_y_LeftLeg = m_fecy; //0.010f;
            float m_e_min_y_RightLeg = m_fecy;//0.010f;

            float fy_edge2 = m_fey; //0.030f;
            float fy_edge3 = m_fey + m_fsy; // 0.120f;

            // TODO nahradit enumom a switchom

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                if (IHolesNumber == 16) // LH, LI, LK
                {
                    // Left Leg

                    HolesCentersPoints2D[0] = new Point(m_e_min_x_LeftLeg, m_e_min_y_LeftLeg);

                    HolesCentersPoints2D[1] = new Point(flZ - m_e_min_x_LeftLeg, HolesCentersPoints2D[0].Y);

                    HolesCentersPoints2D[2] = new Point(0.5f * flZ, fy_edge2);

                    HolesCentersPoints2D[3] = new Point(0.5f * flZ, fy_edge3);

                    HolesCentersPoints2D[4] = new Point(HolesCentersPoints2D[3].X, m_fszy_left - fy_edge3);

                    HolesCentersPoints2D[5] = new Point(HolesCentersPoints2D[3].X, m_fszy_left - fy_edge2);

                    HolesCentersPoints2D[6] = new Point(HolesCentersPoints2D[0].X, m_fszy_left - m_e_min_y_LeftLeg);

                    HolesCentersPoints2D[7] = new Point(HolesCentersPoints2D[1].X, m_fszy_left - m_e_min_y_LeftLeg);

                    // Right Leg
                    if (MATH.MathF.d_equal(fbX1, flZ)) // Sirka bX1 a lZ su rovnake - skrutky posunieme v smere X
                    {
                        HolesCentersPoints2D[8] = new Point(flZ + HolesCentersPoints2D[6].X, m_fszy_right - m_e_min_y_RightLeg);

                        HolesCentersPoints2D[9] = new Point(flZ + HolesCentersPoints2D[7].X, m_fszy_right - m_e_min_y_RightLeg);

                        HolesCentersPoints2D[10] = new Point(flZ + HolesCentersPoints2D[5].X, m_fszy_right - fy_edge2);

                        HolesCentersPoints2D[11] = new Point(flZ + HolesCentersPoints2D[4].X, m_fszy_right - fy_edge3);

                        HolesCentersPoints2D[12] = new Point(flZ + HolesCentersPoints2D[3].X, HolesCentersPoints2D[3].Y);

                        HolesCentersPoints2D[13] = new Point(flZ + HolesCentersPoints2D[2].X, HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[14] = new Point(flZ + HolesCentersPoints2D[1].X, HolesCentersPoints2D[1].Y);

                        HolesCentersPoints2D[15] = new Point(flZ + HolesCentersPoints2D[0].X, HolesCentersPoints2D[0].Y);
                    }
                    else // Sirka bX1 a lZ nie su rovnake
                    {
                        HolesCentersPoints2D[8] = new Point(flZ + m_e_min_z_RightLeg, m_fszy_right - m_e_min_y_RightLeg);

                        HolesCentersPoints2D[9] = new Point(flZ + fbX1 - m_e_min_z_RightLeg, m_fszy_right - m_e_min_y_RightLeg);

                        HolesCentersPoints2D[10] = new Point(flZ + 0.5f * fbX1, m_fszy_right - fy_edge2);

                        HolesCentersPoints2D[11] = new Point(flZ + 0.5f * fbX1, m_fszy_right - fy_edge3);

                        HolesCentersPoints2D[12] = new Point(flZ + 0.5f * fbX1, HolesCentersPoints2D[3].Y);

                        HolesCentersPoints2D[13] = new Point(flZ + 0.5f * fbX1, HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[14] = new Point(flZ + fbX1 - m_e_min_z_RightLeg, HolesCentersPoints2D[1].Y);

                        HolesCentersPoints2D[15] = new Point(flZ + m_e_min_z_RightLeg,HolesCentersPoints2D[0].Y);
                    }
                }
                else if (IHolesNumber == 8) // LJ
                {
                    // Left Leg

                    HolesCentersPoints2D[0] = new Point(m_e_min_x_LeftLeg, m_e_min_y_LeftLeg);

                    HolesCentersPoints2D[1] = new Point(flZ - m_e_min_x_LeftLeg, HolesCentersPoints2D[0].Y);

                    HolesCentersPoints2D[2] = new Point(HolesCentersPoints2D[0].X, m_fszy_left - m_e_min_y_LeftLeg);

                    HolesCentersPoints2D[3] = new Point(HolesCentersPoints2D[1].X, HolesCentersPoints2D[2].Y);

                    if (MATH.MathF.d_equal(fbX1, flZ)) // Sirka bX1 a lZ su rovnake - skrutky posunieme v smere X
                    {
                        // Right Leg
                        HolesCentersPoints2D[4] = new Point(flZ + HolesCentersPoints2D[2].X, m_fszy_right - m_e_min_y_RightLeg);

                        HolesCentersPoints2D[5] = new Point(flZ + HolesCentersPoints2D[3].X, HolesCentersPoints2D[4].Y);

                        HolesCentersPoints2D[6] = new Point(flZ + HolesCentersPoints2D[0].X, HolesCentersPoints2D[0].Y);

                        HolesCentersPoints2D[7] = new Point(flZ + HolesCentersPoints2D[1].X, HolesCentersPoints2D[0].Y);
                    }
                    else
                    {
                        // Right Leg
                        HolesCentersPoints2D[4] = new Point(flZ + m_e_min_z_RightLeg, m_fszy_right - m_e_min_y_RightLeg);

                        HolesCentersPoints2D[5] = new Point(flZ + fbX1 - m_e_min_z_RightLeg, HolesCentersPoints2D[4].Y);

                        HolesCentersPoints2D[6] = new Point(flZ + m_e_min_z_RightLeg, HolesCentersPoints2D[0].Y);

                        HolesCentersPoints2D[7] = new Point(flZ + fbX1 - m_e_min_z_RightLeg, HolesCentersPoints2D[0].Y);
                    }
                }
                else
                {
                    // Not defined expected number of holes for L plate
                }

                // Zohladnime pridavny offset v smere y
                // Pre polovicu pridame offset na lavom ramene, pre druhu polovicu na pravom ramene
                for (int i = 0; i < IHolesNumber / 2; i++)
                {
                    HolesCentersPoints2D[i].Y += Feoy_left;
                    HolesCentersPoints2D[IHolesNumber / 2 + i].Y += Feoy_right;
                }
            }
        }
    }
}
