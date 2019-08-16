using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj.Objects_3D
{
    public class StraightLineArrow3D
    {
        // Todo - Ondrej, tato trieda by asi mala vracat priamo objekt model3D alebo by triedy, ktore maju 3D geometricku reprezentaciu, napr. CVolume aj tato trieda mali mat model3D ako predka

        public int iPrimaryArrowModelDirection; // Kod pre smer modelu sipky v LCS (0 - X, 1 - Y, 2 - Z, default pre zatazenie je 2 (smer z)
        public bool bArrowAtBothEnds;
        public float fConeHeight;
        public float fCylinderHeight;
        public float fTotalHeight;

        public float[,] fAnnulusOutPoints;
        public float[,] fAnnulusInPoints;

        public Point3DCollection ArrowPoints;

        const int number_of_segments = 16; //72; // Pocet segmentov v kruhu - PERFORMANCE!!!

        public StraightLineArrow3D()
        { }

        public StraightLineArrow3D(float totalHeight, float fCylinderRadius, int iPrimaryArrowModelDirection_temp = 2, bool bArrowAtBothEnds_temp = false)
        {
            fTotalHeight = totalHeight;
            iPrimaryArrowModelDirection = iPrimaryArrowModelDirection_temp;
            bArrowAtBothEnds = bArrowAtBothEnds_temp;

            fConeHeight = 50 * fCylinderRadius; /// ???
            fCylinderHeight = fTotalHeight - fConeHeight;

            AnnulusPoints(fCylinderRadius, fCylinderRadius * 10); // ???
        }

        public StraightLineArrow3D(float totalHeight, int iPrimaryArrowModelDirection_temp = 2, bool bArrowAtBothEnds_temp = false)
        {
            fTotalHeight = totalHeight;
            iPrimaryArrowModelDirection = iPrimaryArrowModelDirection_temp;
            bArrowAtBothEnds = bArrowAtBothEnds_temp;

            fConeHeight = 0.2f * fTotalHeight;
            fCylinderHeight = fTotalHeight - fConeHeight;

            AnnulusPoints(totalHeight * 0.005f, totalHeight * 0.05f);
        }

        float[,] GetCircleCoordinates(float fr)
        {
            float[,] fCirclePoints = new float[number_of_segments, 2];

            for (int i = 0; i < number_of_segments; i++)
            {
                float theta = 2.0f * (float)Math.PI * i / number_of_segments;
                fCirclePoints[i, 0] = fr * (float)Math.Cos(theta);
                fCirclePoints[i, 1] = fr * (float)Math.Sin(theta);
            }

            return fCirclePoints;
        }

        void AnnulusPoints(float fr_in, float fr_out)
        {
            fAnnulusOutPoints = new float[number_of_segments, 2];
            fAnnulusInPoints = new float[number_of_segments, 2];

            fAnnulusOutPoints = GetCircleCoordinates(fr_out);
            fAnnulusInPoints = GetCircleCoordinates(fr_in);
        }

        public Point3DCollection GetArrowPoints()
        {
            Point3DCollection cPointsCollection = new Point3DCollection(1 + 3 * number_of_segments + 1);

            cPointsCollection.Add(new Point3D(0, 0, 0)); // Tip

            for (int i = 0; i < number_of_segments; i++)
            {
                cPointsCollection.Add(GetPointinLCS(fAnnulusOutPoints[i, 0], fAnnulusOutPoints[i, 1], fConeHeight));
            }

            for (int i = 0; i < number_of_segments; i++)
            {
                cPointsCollection.Add(GetPointinLCS(fAnnulusInPoints[i, 0], fAnnulusInPoints[i, 1], fConeHeight));
            }

            for (int i = 0; i < number_of_segments; i++)
            {
                cPointsCollection.Add(GetPointinLCS(fAnnulusInPoints[i, 0], fAnnulusInPoints[i, 1], bArrowAtBothEnds ? fTotalHeight - fConeHeight : fTotalHeight));
            }

            if(bArrowAtBothEnds) // Second Arrow Outside Points
            {
                for (int i = 0; i < number_of_segments; i++)
                {
                    cPointsCollection.Add(GetPointinLCS(fAnnulusOutPoints[i, 0], fAnnulusOutPoints[i, 1], fTotalHeight - fConeHeight));
                }
            }

            cPointsCollection.Add(GetPointinLCS(0, 0, fTotalHeight)); // Top middle point or tip

            return cPointsCollection;
        }

        public Int32Collection GetArrowIndices()
        {
            Int32Collection cArrowIndices = new Int32Collection();

            for (int i = 0; i < number_of_segments; i++)
            {
                if (i < number_of_segments - 1)
                {
                    cArrowIndices.Add(0); // Tip
                    cArrowIndices.Add(i + 2);
                    cArrowIndices.Add(i + 1);
                }
                else // last
                {
                    cArrowIndices.Add(0); // Tip
                    cArrowIndices.Add(1);
                    cArrowIndices.Add(i + 1);
                }
            }

            // annulus
            for (int i = 0; i < number_of_segments; i++)
            {
                if (i < number_of_segments - 1)
                {
                    CreateRectangle_CCW(cArrowIndices, i + 1, i + 2, i + 2 + number_of_segments, i + 1 + number_of_segments);
                }
                else // last
                {
                    CreateRectangle_CCW(cArrowIndices, i + 1, 1, 1 + number_of_segments, i + 1 + number_of_segments);
                }
            }

            // Rozna orientacia normal !!!
            // shell
            for (int i = 0; i < number_of_segments; i++)
            {
                if (i < number_of_segments - 1)
                {
                    CreateRectangle_CW(cArrowIndices, i + 1 + number_of_segments, i + 1 + 2 * number_of_segments, i + 2 + 2 * number_of_segments, i + 2 + number_of_segments);
                }
                else // last
                {
                    CreateRectangle_CW(cArrowIndices, 2 * number_of_segments, 3 * number_of_segments, i + 2 + number_of_segments, i + 2);
                }
            }

            if (!bArrowAtBothEnds)
            {
                // Rozna orientacia normal !!!
                // Top surface
                for (int i = 0; i < number_of_segments; i++)
                {
                    if (i < number_of_segments - 1)
                    {
                        cArrowIndices.Add(3 * number_of_segments + 1);
                        cArrowIndices.Add(i + 1 + 2 * number_of_segments);
                        cArrowIndices.Add(i + 2 + 2 * number_of_segments);
                    }
                    else // last
                    {
                        cArrowIndices.Add(3 * number_of_segments + 1);
                        cArrowIndices.Add(3 * number_of_segments);
                        cArrowIndices.Add(2 * number_of_segments + 1);
                    }
                }
            }

            if (bArrowAtBothEnds)
            {
                // Annulus - second arrow
                for (int i = 0; i < number_of_segments; i++)
                {
                    if (i < number_of_segments - 1)
                    {
                        CreateRectangle_CCW(cArrowIndices, 2 * number_of_segments + i + 1, 2 * number_of_segments + i + 2, 2 * number_of_segments + i + 2 + number_of_segments, 2 * number_of_segments + i + 1 + number_of_segments);
                    }
                    else // last
                    {
                        CreateRectangle_CCW(cArrowIndices, 2 * number_of_segments + i + 1, 2 * number_of_segments + 1, 2 * number_of_segments + 1 + number_of_segments, 2 * number_of_segments + i + 1 + number_of_segments);
                    }
                }

                for (int i = 0; i < number_of_segments; i++)
                {
                    if (i < number_of_segments - 1)
                    {
                        cArrowIndices.Add(4 * number_of_segments + 1); // Tip
                        cArrowIndices.Add(3 * number_of_segments + i + 1);
                        cArrowIndices.Add(3 * number_of_segments + i + 2);
                    }
                    else // last
                    {
                        cArrowIndices.Add(4 * number_of_segments + 1); // Tip
                        cArrowIndices.Add(3 * number_of_segments + i + 1);
                        cArrowIndices.Add(3 * number_of_segments + 1);
                    }
                }
            }

            return cArrowIndices;
        }

        public void CreateRectangle_CCW(Int32Collection ArrowIndices, int i0, int i1, int i2, int i3)
        {
            ArrowIndices.Add(i0);
            ArrowIndices.Add(i1);
            ArrowIndices.Add(i2);

            ArrowIndices.Add(i0);
            ArrowIndices.Add(i2);
            ArrowIndices.Add(i3);
        }

        public void CreateRectangle_CW(Int32Collection ArrowIndices, int i0, int i1, int i2, int i3)
        {
            ArrowIndices.Add(i0);
            ArrowIndices.Add(i2);
            ArrowIndices.Add(i1);

            ArrowIndices.Add(i0);
            ArrowIndices.Add(i3);
            ArrowIndices.Add(i2);
        }

        // Refaktorovat s CVolume
        private Point3D GetPointinLCS(double dCoordX, double dCoordY, double dCoordZ)
        {
            Point3D p = new Point3D();
            // Nastavi suradnice uzla podla toho v akom smere sa ma sipka primarne vykreslit

            // iPrimaryArrowModelDirection Kod pre smer modelu sipky v LCS(0 - X, 1 - Y, 2 - Z - default
            if (iPrimaryArrowModelDirection == 0)
            {
                p.X = dCoordZ;
                p.Y = dCoordX;
                p.Z = dCoordY;
            }
            else if (iPrimaryArrowModelDirection == 1)
            {
                p.X = dCoordX;
                p.Y = dCoordZ;
                p.Z = dCoordY;
            }
            else //if (iPrimaryArrowModelDirection == 2)// Default (sipka v smere Z)
            {
                p.X = dCoordX;
                p.Y = dCoordY;
                p.Z = dCoordZ;
            }

            return p;
        }
    }
}
