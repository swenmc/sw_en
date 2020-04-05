using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_LL:CScrewArrangement
    {
        public CScrewArrangement_LL() { }

        public CScrewArrangement_LL(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float ft, float fbX1, float fbX2, float fhY, float flZ)
        {
            float fx_edge = 0.010f;  // y-direction
            float fy_edge1 = 0.010f; // x-direction
            float fy_edge2 = 0.030f; // x-direction
            float fy_edge3 = 0.120f; // x-direction

            // TODO nahradit enumom a switchom

            // TODO OPRAVIT SURADNICE

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                if (IHolesNumber == 32) // LLH, LLK
                {
                    HolesCentersPoints2D[0] = new Point(-fhY + ft + fy_edge1, fx_edge);

                    HolesCentersPoints2D[1] = new Point(HolesCentersPoints2D[0].X, fbX1 - fx_edge);

                    HolesCentersPoints2D[2] = new Point(-fhY + ft + fy_edge2, 0.5f * fbX1);

                    HolesCentersPoints2D[3] = new Point(-fhY + ft + fy_edge3, HolesCentersPoints2D[2].Y);

                    HolesCentersPoints2D[4] = new Point(-fy_edge3, HolesCentersPoints2D[2].Y);

                    HolesCentersPoints2D[5] = new Point(-fy_edge2, HolesCentersPoints2D[2].Y);

                    HolesCentersPoints2D[6] = new Point(-fy_edge1, HolesCentersPoints2D[0].Y);

                    HolesCentersPoints2D[7] = new Point(HolesCentersPoints2D[6].X, HolesCentersPoints2D[1].Y);

                    if (MATH.MathF.d_equal(fbX1, flZ)) // Sirka bX1 a lZ su rovnake, skrutky len posunieme v smere Y of fTemp
                    {
                        float fTemp = (0.5f * fbX1 + 0.5f * flZ);

                        HolesCentersPoints2D[8] = new Point(HolesCentersPoints2D[7].X, fTemp + HolesCentersPoints2D[7].Y);

                        HolesCentersPoints2D[9] = new Point(HolesCentersPoints2D[6].X, fTemp + HolesCentersPoints2D[6].Y);

                        HolesCentersPoints2D[10] = new Point(HolesCentersPoints2D[5].X, fTemp + HolesCentersPoints2D[5].Y);

                        HolesCentersPoints2D[11] = new Point(HolesCentersPoints2D[4].X, fTemp + HolesCentersPoints2D[4].Y);

                        HolesCentersPoints2D[12] = new Point(HolesCentersPoints2D[3].X, fTemp + HolesCentersPoints2D[3].Y);

                        HolesCentersPoints2D[13] = new Point(HolesCentersPoints2D[2].X, fTemp + HolesCentersPoints2D[2].Y);

                        HolesCentersPoints2D[14] = new Point(HolesCentersPoints2D[0].X, fTemp + HolesCentersPoints2D[1].Y);

                        HolesCentersPoints2D[15] = new Point(HolesCentersPoints2D[1].X, fTemp + HolesCentersPoints2D[0].Y);
                    }
                    else // Sirka bX1 a lZ nie su rovnake, vypocitame nove pozicie tak aby boli vzdialenosti od okrajov rovnake
                    {
                        HolesCentersPoints2D[8] = new Point(HolesCentersPoints2D[7].X, fbX1 + flZ - fx_edge);

                        HolesCentersPoints2D[9] = new Point(HolesCentersPoints2D[6].X, fbX1 + fx_edge);

                        HolesCentersPoints2D[10] = new Point(HolesCentersPoints2D[5].X, fbX1 + 0.5f * flZ);

                        HolesCentersPoints2D[11] = new Point(HolesCentersPoints2D[4].X, fbX1 + 0.5f * flZ);

                        HolesCentersPoints2D[12] = new Point(HolesCentersPoints2D[3].X, fbX1 + 0.5f * flZ);

                        HolesCentersPoints2D[13] = new Point(HolesCentersPoints2D[2].X, fbX1 + 0.5f * flZ);

                        HolesCentersPoints2D[14] = new Point(HolesCentersPoints2D[0].X, fbX1 + flZ - fx_edge);

                        HolesCentersPoints2D[15] = new Point(HolesCentersPoints2D[1].X, fbX1 + fx_edge);
                    }

                    for (int i = 0; i < IHolesNumber / 2; i++)
                    {
                        HolesCentersPoints2D[IHolesNumber / 2 + i] =
                            new Point(fbX2 - HolesCentersPoints2D[(IHolesNumber / 2 - i - 1)].X, HolesCentersPoints2D[(IHolesNumber / 2 - i - 1)].Y);
                    }
                }
                else
                {
                    // Not defined expected number of holes for LL plate
                }
            }
        }
    }
}
