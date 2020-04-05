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
        public CScrewArrangement_L() { }

        public CScrewArrangement_L(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fbX1, float fhY, float flZ)
        {
            float m_e_min_x_LeftLeg = 0.010f; // Left leg
            float m_e_min_z_RightLeg = 0.010f; // Right leg

            float m_e_min_y_LeftLeg = 0.010f;
            float m_e_min_y_RightLeg = 0.010f;

            float fy_edge2 = 0.030f;
            float fy_edge3 = 0.120f;

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

                    HolesCentersPoints2D[4] = new Point(HolesCentersPoints2D[3].X, fhY - fy_edge3);

                    HolesCentersPoints2D[5] = new Point(HolesCentersPoints2D[3].X, fhY - fy_edge2);

                    HolesCentersPoints2D[6] = new Point(HolesCentersPoints2D[0].X, fhY - m_e_min_y_LeftLeg);

                    HolesCentersPoints2D[7] = new Point(HolesCentersPoints2D[1].X, fhY - m_e_min_y_LeftLeg);

                    // Right Leg
                    if (MATH.MathF.d_equal(fbX1, flZ)) // Sirka bX1 a lZ su rovnake - skrutky posunieme v smere X
                    {
                        HolesCentersPoints2D[8] = new Point(flZ + HolesCentersPoints2D[6].X, HolesCentersPoints2D[6].Y);

                        HolesCentersPoints2D[9] = new Point(flZ + HolesCentersPoints2D[7].X, HolesCentersPoints2D[7].Y);

                        HolesCentersPoints2D[10] = new Point(flZ + HolesCentersPoints2D[5].X, HolesCentersPoints2D[5].Y);

                        HolesCentersPoints2D[11] = new Point(flZ + HolesCentersPoints2D[4].X, HolesCentersPoints2D[4].Y);

                        HolesCentersPoints2D[12] = new Point(flZ + HolesCentersPoints2D[3].X, HolesCentersPoints2D[3].Y);

                        HolesCentersPoints2D[13] = new Point(flZ + HolesCentersPoints2D[2].X, HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[14] = new Point(flZ + HolesCentersPoints2D[1].X, HolesCentersPoints2D[1].Y);

                        HolesCentersPoints2D[15] = new Point(flZ + HolesCentersPoints2D[0].X, HolesCentersPoints2D[0].Y);
                    }
                    else // Sirka bX1 a lZ nie su rovnake
                    {
                        HolesCentersPoints2D[8] = new Point(flZ + m_e_min_z_RightLeg, HolesCentersPoints2D[6].Y);

                        HolesCentersPoints2D[9] = new Point(flZ + fbX1 - m_e_min_z_RightLeg, HolesCentersPoints2D[7].Y);

                        HolesCentersPoints2D[10] = new Point(flZ + 0.5f * fbX1, HolesCentersPoints2D[5].Y);

                        HolesCentersPoints2D[11] = new Point(flZ + 0.5f * fbX1, HolesCentersPoints2D[4].Y);

                        HolesCentersPoints2D[12] = new Point(flZ + 0.5f * fbX1, HolesCentersPoints2D[3].Y);

                        HolesCentersPoints2D[13] = new Point(flZ + 0.5f * fbX1, HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[14] = new Point(flZ + fbX1 - m_e_min_z_RightLeg, HolesCentersPoints2D[1].Y);

                        HolesCentersPoints2D[15] = new Point(flZ + m_e_min_z_RightLeg, HolesCentersPoints2D[0].Y);
                    }
                }
                else if (IHolesNumber == 8) // LJ
                {
                    // Left Leg

                    HolesCentersPoints2D[0] = new Point(m_e_min_x_LeftLeg, m_e_min_y_LeftLeg);

                    HolesCentersPoints2D[1] = new Point(flZ - m_e_min_x_LeftLeg, HolesCentersPoints2D[0].Y);

                    HolesCentersPoints2D[2] = new Point(HolesCentersPoints2D[0].X, fhY - m_e_min_y_LeftLeg);

                    HolesCentersPoints2D[3] = new Point(HolesCentersPoints2D[1].X, HolesCentersPoints2D[2].Y);

                    if (MATH.MathF.d_equal(fbX1, flZ)) // Sirka bX1 a lZ su rovnake - skrutky posunieme v smere X
                    {
                        // Right Leg
                        HolesCentersPoints2D[4] = new Point(flZ + HolesCentersPoints2D[2].X, HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[5] = new Point(flZ + HolesCentersPoints2D[3].X, HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[6] = new Point(flZ + HolesCentersPoints2D[0].X, HolesCentersPoints2D[0].Y);

                        HolesCentersPoints2D[7] = new Point(flZ + HolesCentersPoints2D[1].X, HolesCentersPoints2D[0].Y);
                    }
                    else
                    {
                        // Right Leg
                        HolesCentersPoints2D[4] = new Point(flZ + m_e_min_z_RightLeg, HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[5] = new Point(flZ + fbX1 - m_e_min_z_RightLeg, HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[6] = new Point(flZ + m_e_min_z_RightLeg, HolesCentersPoints2D[0].Y);

                        HolesCentersPoints2D[7] = new Point(flZ + fbX1 - m_e_min_z_RightLeg, HolesCentersPoints2D[0].Y);
                    }
                }
                else
                {
                    // Not defined expected number of holes for L plate
                }
            }
        }
    }
}
