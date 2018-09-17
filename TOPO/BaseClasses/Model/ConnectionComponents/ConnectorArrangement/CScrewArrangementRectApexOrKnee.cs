using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    public struct ScrewRectSequence
    {
        public int iNumberOfScrewsInRow_xDirection;
        public int iNumberOfScrewsInColumn_yDirection;
        public float fx_c;
        public float fy_c;
        public float fDistanceOfPointsX;
        public float fDistanceOfPointsY;
        public float[,] fHolesCentersPoints2D;
    }

    public class CScrewArrangementRectApexOrKnee : CScrewArrangement
    {
        private List<ScrewRectSequence> m_listOfRectSequences;

        public List<ScrewRectSequence> RectangularSequences
        {
            get
            {
                return m_listOfRectSequences;
            }

            set
            {
                m_listOfRectSequences = value;
            }
        }

        private float m_fCrscRafterDepth;

        public float FCrscRafterDepth
        {
            get
            {
                return m_fCrscRafterDepth;
            }

            set
            {
                m_fCrscRafterDepth = value;
            }
        }

        private float m_fCrscWebStraightDepth;

        public float FCrscWebStraightDepth
        {
            get
            {
                return m_fCrscWebStraightDepth;
            }

            set
            {
                m_fCrscWebStraightDepth = value;
            }
        }

        float m_fStiffenerSize; // Middle cross-section stiffener dimension (without screws)

        public float FStiffenerSize
        {
            get
            {
                return m_fStiffenerSize;
            }

            set
            {
                m_fStiffenerSize = value;
            }
        }

        private int m_iNumberOfCircleGroupsInJoint = 2; // Pocet kruhov na jednom plechu (skupina - group)

        public int INumberOfCircleGroupsInJoint
        {
            get
            {
                return m_iNumberOfCircleGroupsInJoint;
            }

            set
            {
                m_iNumberOfCircleGroupsInJoint = value;
            }
        }

        // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii
        public int iNumberOfScrewsInRow_xDirection_SQ1;
        public int iNumberOfScrewsInColumn_yDirection_SQ1;
        public float fx_c_SQ1;
        public float fy_c_SQ1;
        public float fDistanceOfPointsX_SQ1;
        public float fDistanceOfPointsY_SQ1;
        public int iNumberOfScrewsInRow_xDirection_SQ2;
        public int iNumberOfScrewsInColumn_yDirection_SQ2;
        public float fx_c_SQ2;
        public float fy_c_SQ2;
        public float fDistanceOfPointsX_SQ2;
        public float fDistanceOfPointsY_SQ2;

        float m_fSlope_rad;
        public float[] HolesCenterRadii;

        public CScrewArrangementRectApexOrKnee()
        { }

        public CScrewArrangementRectApexOrKnee(
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            int iNumberOfScrewsInRow_xDirection_SQ1_temp,
            int iNumberOfScrewsInColumn_yDirection_SQ1_temp,
            float fx_c_SQ1_temp,
            float fy_c_SQ1_temp,
            float fDistanceOfPointsX_SQ1_temp,
            float fDistanceOfPointsY_SQ1_temp,
            int iNumberOfScrewsInRow_xDirection_SQ2_temp,
            int iNumberOfScrewsInColumn_yDirection_SQ2_temp,
            float fx_c_SQ2_temp,
            float fy_c_SQ2_temp,
            float fDistanceOfPointsX_SQ2_temp,
            float fDistanceOfPointsY_SQ2_temp) : base(iNumberOfScrewsInRow_xDirection_SQ1_temp * iNumberOfScrewsInColumn_yDirection_SQ1_temp + iNumberOfScrewsInRow_xDirection_SQ2_temp * iNumberOfScrewsInColumn_yDirection_SQ2_temp, referenceScrew_temp)
        {
            RectangularSequences = new List<ScrewRectSequence>(2); // TODO nastavit pocet sekvencii v spoji
            referenceScrew = referenceScrew_temp;
            FCrscRafterDepth = fCrscRafterDepth_temp;
            FCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            FStiffenerSize = fStiffenerSize_temp;

            // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii
            iNumberOfScrewsInRow_xDirection_SQ1 = iNumberOfScrewsInRow_xDirection_SQ1_temp;
            iNumberOfScrewsInColumn_yDirection_SQ1 = iNumberOfScrewsInColumn_yDirection_SQ1_temp;
            fx_c_SQ1 = fx_c_SQ1_temp;
            fy_c_SQ1 = fy_c_SQ1_temp;
            fDistanceOfPointsX_SQ1 = fDistanceOfPointsX_SQ1_temp;
            fDistanceOfPointsY_SQ1 = fDistanceOfPointsY_SQ1_temp;

            iNumberOfScrewsInRow_xDirection_SQ2 = iNumberOfScrewsInRow_xDirection_SQ2_temp;
            iNumberOfScrewsInColumn_yDirection_SQ2 = iNumberOfScrewsInColumn_yDirection_SQ2_temp;
            fx_c_SQ2 = fx_c_SQ2_temp;
            fy_c_SQ2 = fy_c_SQ2_temp;
            fDistanceOfPointsX_SQ2 = fDistanceOfPointsX_SQ2_temp;
            fDistanceOfPointsY_SQ2 = fDistanceOfPointsY_SQ2_temp;

            ScrewRectSequence seq1 = new ScrewRectSequence();
            seq1.iNumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ1_temp;
            seq1.iNumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ1_temp;
            seq1.fx_c = fx_c_SQ1_temp;
            seq1.fy_c = fy_c_SQ1_temp;
            seq1.fDistanceOfPointsX = fDistanceOfPointsX_SQ1_temp;
            seq1.fDistanceOfPointsY = fDistanceOfPointsY_SQ1_temp;
            seq1.fHolesCentersPoints2D = new float[seq1.iNumberOfScrewsInRow_xDirection * seq1.iNumberOfScrewsInColumn_yDirection, 2];
            RectangularSequences.Add(seq1);

            ScrewRectSequence seq2 = new ScrewRectSequence();
            seq2.iNumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ2_temp;
            seq2.iNumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ2_temp;
            seq2.fx_c = fx_c_SQ2_temp;
            seq2.fy_c = fy_c_SQ2_temp;
            seq2.fDistanceOfPointsX = fDistanceOfPointsX_SQ2_temp;
            seq2.fDistanceOfPointsY = fDistanceOfPointsY_SQ2_temp;
            seq2.fHolesCentersPoints2D = new float[seq2.iNumberOfScrewsInRow_xDirection * seq2.iNumberOfScrewsInColumn_yDirection, 2];
            RectangularSequences.Add(seq2);

            UpdateArrangmentData();
        }

        public void UpdateArrangmentData()
        {
            IHolesNumber = 0;

            foreach (ScrewRectSequence seq in RectangularSequences)
               IHolesNumber += seq.iNumberOfScrewsInRow_xDirection * seq.iNumberOfScrewsInColumn_yDirection;

            int iNumberOfGroupsInPlate = 2;

            IHolesNumber *= iNumberOfGroupsInPlate;

            HolesCentersPoints2D = new float[IHolesNumber, 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public float[,] Get_ScrewSequencePointCoordinates(ScrewRectSequence srectSeq)
        {
            // Connectors in Sequence
            return GetRegularArrayOfPointsInCartesianCoordinates(srectSeq.fx_c, srectSeq.fy_c, srectSeq.iNumberOfScrewsInRow_xDirection, srectSeq.iNumberOfScrewsInColumn_yDirection, srectSeq.fDistanceOfPointsX, srectSeq.fDistanceOfPointsY);
        }

        public void Calc_HolesCentersCoord2DApexPlate(
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad,
            ref float[,] fHolesCentersPoints2D)
        {
            // Coordinates of [0,0] of sequence point on plate
            float fx_c = 0.05f;
            float fy_c = 0.05f;

            // Left side
            ScrewRectSequence seq1 = RectangularSequences[0]; // TODO - Doriesit ako pristupovat k premennym v struct a menit ich, neda sa odkazovat referenciou
            seq1.fHolesCentersPoints2D = Get_ScrewSequencePointCoordinates(RectangularSequences[0]);
            RectangularSequences[0] = seq1;

            ScrewRectSequence seq2 = RectangularSequences[1];
            seq2.fHolesCentersPoints2D = Get_ScrewSequencePointCoordinates(RectangularSequences[1]);
            RectangularSequences[1] = seq2;

            // Rotate screws by roof slope
            // Rotate about [0,0]
            RotateSequence_CCW_rad(0, 0, fSlope_rad, RectangularSequences[0]);
            RotateSequence_CCW_rad(0, 0, fSlope_rad, RectangularSequences[1]);

            // Translate from [0,0] on plate to the final position
            TranslateSequence(fx_c, fy_c, RectangularSequences[0]);
            TranslateSequence(fx_c, fy_c, RectangularSequences[1]);

            // Right side
            ScrewRectSequence seq3 = RectangularSequences[0];
            seq3.fHolesCentersPoints2D = GetMirroredSequenceAboutY(0.5f * fbX, RectangularSequences[0].fHolesCentersPoints2D);
            ScrewRectSequence seq4 = RectangularSequences[1];
            seq4.fHolesCentersPoints2D = GetMirroredSequenceAboutY(0.5f * fbX, RectangularSequences[1].fHolesCentersPoints2D);

            // Add mirrored sequences into the list
            RectangularSequences.Add(seq3);
            RectangularSequences.Add(seq4);

            // Fill array of holes centers
            int iPointIndex = 0;
            for (int i = 0; i < RectangularSequences.Count; i++) // Add each sequence
            {
                for (int j = 0; j < RectangularSequences[i].fHolesCentersPoints2D.Length / 2; j++) // Add each point in the sequence
                {
                    HolesCentersPoints2D[iPointIndex + j, 0] = RectangularSequences[i].fHolesCentersPoints2D[j, 0];
                    HolesCentersPoints2D[iPointIndex + j, 1] = RectangularSequences[i].fHolesCentersPoints2D[j, 1];
                }

                iPointIndex += RectangularSequences[i].fHolesCentersPoints2D.Length / 2;
            }

            // TODO - temporary nastavit pre pole suradnic ktore je sucastou plate
            // teoereticky moze mat usporadanie iny pocet ako je pocet na plate, napriklad ak sa usporiadanie odzrkadli alebo skopiruje vramci plochy (napr. Typ KE)
            fHolesCentersPoints2D = HolesCentersPoints2D;
        }

        public float[,] GetMirroredSequenceAboutY(float fXDistanceOfMirrorAxis, float[,] fInputSequence)
        {
            float[,] fOutputSequence = new float[fInputSequence.Length / 2, 2];
            for (int i = 0; i< fInputSequence.Length /2; i++)
            {
                fOutputSequence[i, 0] = 2 * fXDistanceOfMirrorAxis + fInputSequence[i, 0] * (-1f);
                fOutputSequence[i, 1] = fInputSequence[i, 1];
            }

            return fOutputSequence;
        }

        public void RotateSequence_CCW_rad(float fRotationCenterPoint_x, float fRotationCenterPoint_y, float fRotationAngle_rad, ScrewRectSequence sequence)
        {
            float[,] seqPoints = sequence.fHolesCentersPoints2D;
            Geom2D.TransformPositions_CCW_rad(fRotationCenterPoint_x, fRotationCenterPoint_y, fRotationAngle_rad, ref seqPoints);
            sequence.fHolesCentersPoints2D = seqPoints; // Skontrolovat ci je to potrebne nastavit
        }

        public void TranslateSequence(float fPoint_x, float fPoint_y, ScrewRectSequence sequence)
        {
            float[,] seqPoints = sequence.fHolesCentersPoints2D;
            Geom2D.TransformPositions_CCW_rad(fPoint_x, fPoint_y, 0, ref seqPoints);
            sequence.fHolesCentersPoints2D = seqPoints; // Skontrolovat ci je to potrebne nastavit
        }

        public void Calc_HolesControlPointsCoord3D(float flZ, float ft)
        {
            for (int i = 0; i < IHolesNumber; i++)
            {
                arrConnectorControlPoints3D[i].X = HolesCentersPoints2D[i, 0];
                arrConnectorControlPoints3D[i].Y = HolesCentersPoints2D[i, 1] - flZ; // Musime odpocitat zalomenie hrany plechu, v 2D zobrazeni sa totiz pripocitalo
                arrConnectorControlPoints3D[i].Z = -ft; // TODO Position depends on screw length;
            }
        }

        public void GenerateConnectors()
        {
            Screws = new CScrew[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);
                Screws[i] = new CScrew(referenceScrew.Name, controlpoint, referenceScrew.Gauge, referenceScrew.Diameter_thread, referenceScrew.D_h_headdiameter, referenceScrew.D_w_washerdiameter, referenceScrew.T_w_washerthickness, referenceScrew.Length, referenceScrew.Weight, 0, -90, 0, true);
            }
        }
    }
}