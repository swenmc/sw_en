﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;
using System.Windows;

namespace BaseClasses
{
    

    public class CScrewArrangementRectApexOrKnee : CScrewArrangement
    {
        private List<CScrewRectSequence> m_listOfRectSequences;

        public List<CScrewRectSequence> RectangularSequences
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

        // Bottom (knee plate) or left (apex plate) group
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

        // Top (knee plate) or right (apex plate) group
        public int iNumberOfScrewsInRow_xDirection_SQ3;
        public int iNumberOfScrewsInColumn_yDirection_SQ3;
        public float fx_c_SQ3;
        public float fy_c_SQ3;
        public float fDistanceOfPointsX_SQ3;
        public float fDistanceOfPointsY_SQ3;
        public int iNumberOfScrewsInRow_xDirection_SQ4;
        public int iNumberOfScrewsInColumn_yDirection_SQ4;
        public float fx_c_SQ4;
        public float fy_c_SQ4;
        public float fDistanceOfPointsX_SQ4;
        public float fDistanceOfPointsY_SQ4;

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
            RectangularSequences = new List<CScrewRectSequence>(2); // TODO nastavit pocet sekvencii v spoji
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

            UpdateArrangmentData();
        }

        public CScrewArrangementRectApexOrKnee(
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            int iNumberOfScrewsInRow_xDirection_G1_SQ_temp,  // Bottom group of knee plate
            int iNumberOfScrewsInColumn_yDirection_G1_SQ_temp,
            int iNumberOfScrewsInRow_xDirection_G2_SQ_temp,  // Upper group of knee plate
            int iNumberOfScrewsInColumn_yDirection_G2_SQ_temp) : base(iNumberOfScrewsInRow_xDirection_G1_SQ_temp * iNumberOfScrewsInColumn_yDirection_G1_SQ_temp + iNumberOfScrewsInRow_xDirection_G2_SQ_temp * iNumberOfScrewsInColumn_yDirection_G2_SQ_temp, referenceScrew_temp)
        {
            RectangularSequences = new List<CScrewRectSequence>(2); // TODO nastavit pocet sekvencii v spoji
            referenceScrew = referenceScrew_temp;
            FCrscRafterDepth = fCrscRafterDepth_temp;
            FCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            FStiffenerSize = fStiffenerSize_temp;

            float fFreeEdgeDistance = 0.05f;
            float fDistanceinX = 0.05f;
            float fDistanceFromEdgeLine = 0.02f;
            float fDepthOfOneStraightPartOfWeb = 0.5f * (fCrscWebStraightDepth_temp - fStiffenerSize_temp);

            // Bottom group
            iNumberOfScrewsInRow_xDirection_SQ1 = iNumberOfScrewsInRow_xDirection_G1_SQ_temp;
            iNumberOfScrewsInColumn_yDirection_SQ1 = iNumberOfScrewsInColumn_yDirection_G1_SQ_temp;
            fx_c_SQ1 = fFreeEdgeDistance;
            fy_c_SQ1 = 0.5f * (fCrscRafterDepth_temp - fCrscWebStraightDepth_temp) + fDistanceFromEdgeLine;
            fDistanceOfPointsX_SQ1 = fDistanceinX;
            fDistanceOfPointsY_SQ1 = fDepthOfOneStraightPartOfWeb - 2 * fDistanceFromEdgeLine;

            iNumberOfScrewsInRow_xDirection_SQ2 = iNumberOfScrewsInRow_xDirection_G1_SQ_temp;
            iNumberOfScrewsInColumn_yDirection_SQ2 = iNumberOfScrewsInColumn_yDirection_G1_SQ_temp;
            fx_c_SQ2 = fFreeEdgeDistance;
            fy_c_SQ2 = 0.5f * (fCrscRafterDepth_temp - fCrscWebStraightDepth_temp) + fDepthOfOneStraightPartOfWeb + fStiffenerSize_temp + fDistanceFromEdgeLine;
            fDistanceOfPointsX_SQ2 = fDistanceinX;
            fDistanceOfPointsY_SQ2 = fDepthOfOneStraightPartOfWeb - 2 * fDistanceFromEdgeLine;

            // Upper group
            iNumberOfScrewsInRow_xDirection_SQ3 = iNumberOfScrewsInRow_xDirection_G2_SQ_temp;
            iNumberOfScrewsInColumn_yDirection_SQ3 = iNumberOfScrewsInColumn_yDirection_G2_SQ_temp;
            fx_c_SQ3 = fFreeEdgeDistance;
            fy_c_SQ3 = 0.5f * (fCrscRafterDepth_temp - fCrscWebStraightDepth_temp) + fDistanceFromEdgeLine;
            fDistanceOfPointsX_SQ3 = fDistanceinX;
            fDistanceOfPointsY_SQ3 = fDepthOfOneStraightPartOfWeb - 2 * fDistanceFromEdgeLine;

            iNumberOfScrewsInRow_xDirection_SQ4 = iNumberOfScrewsInRow_xDirection_G2_SQ_temp;
            iNumberOfScrewsInColumn_yDirection_SQ4 = iNumberOfScrewsInColumn_yDirection_G2_SQ_temp;
            fx_c_SQ4 = fFreeEdgeDistance;
            fy_c_SQ4 = 0.5f * (fCrscRafterDepth_temp - fCrscWebStraightDepth_temp) + fDepthOfOneStraightPartOfWeb + fStiffenerSize_temp + fDistanceFromEdgeLine;
            fDistanceOfPointsX_SQ4 = fDistanceinX;
            fDistanceOfPointsY_SQ4 = fDepthOfOneStraightPartOfWeb - 2 * fDistanceFromEdgeLine;

            UpdateArrangmentData();
        }

        public void UpdateArrangmentData()
        {
            RectangularSequences.Clear(); // Delete previous data otherwise are added more and more new screws to the list

            CScrewRectSequence seq1 = new CScrewRectSequence();
            seq1.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ1;
            seq1.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ1;
            seq1.ReferencePoint = new Point(fx_c_SQ1, fy_c_SQ1);
            seq1.DistanceOfPointsX = fDistanceOfPointsX_SQ1;
            seq1.DistanceOfPointsY = fDistanceOfPointsY_SQ1;
            seq1.HolesCentersPoints = new Point[seq1.NumberOfScrewsInRow_xDirection * seq1.NumberOfScrewsInColumn_yDirection];
            RectangularSequences.Add(seq1);

            CScrewRectSequence seq2 = new CScrewRectSequence();
            seq2.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ2;
            seq2.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ2;
            seq2.ReferencePoint = new Point(fx_c_SQ2, fy_c_SQ2);
            seq2.DistanceOfPointsX = fDistanceOfPointsX_SQ2;
            seq2.DistanceOfPointsY = fDistanceOfPointsY_SQ2;
            seq2.HolesCentersPoints = new Point[seq2.NumberOfScrewsInRow_xDirection * seq2.NumberOfScrewsInColumn_yDirection];
            RectangularSequences.Add(seq2);

            // Celkovy pocet skrutiek
            // Definovane su len sekvencie v jednej group, ocakava sa ze pocet v groups je rovnaky a hodnoty sa skopiruju (napr. pre apex plate)
            IHolesNumber = 0;

            foreach (CScrewRectSequence seq in RectangularSequences)
                IHolesNumber += seq.NumberOfScrewsInRow_xDirection * seq.NumberOfScrewsInColumn_yDirection;

            int iNumberOfGroupsInPlate = 2;

            IHolesNumber *= iNumberOfGroupsInPlate;

            if (iNumberOfScrewsInRow_xDirection_SQ3 != 0 && iNumberOfScrewsInColumn_yDirection_SQ3 != 0 &&
               iNumberOfScrewsInRow_xDirection_SQ4 != 0 && iNumberOfScrewsInColumn_yDirection_SQ4 != 0)
            {
                CScrewRectSequence seq3 = new CScrewRectSequence();
                seq3.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ3;
                seq3.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ3;
                seq3.ReferencePoint = new Point(fx_c_SQ3, fy_c_SQ3);
                seq3.DistanceOfPointsX = fDistanceOfPointsX_SQ3;
                seq3.DistanceOfPointsY = fDistanceOfPointsY_SQ3;
                seq3.HolesCentersPoints = new Point[seq3.NumberOfScrewsInRow_xDirection * seq3.NumberOfScrewsInColumn_yDirection];
                RectangularSequences.Add(seq3);

                CScrewRectSequence seq4 = new CScrewRectSequence();
                seq4.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ4;
                seq4.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ4;
                seq4.ReferencePoint = new Point(fx_c_SQ4, fy_c_SQ4);
                seq4.DistanceOfPointsX = fDistanceOfPointsX_SQ4;
                seq4.DistanceOfPointsY = fDistanceOfPointsY_SQ4;
                seq4.HolesCentersPoints = new Point[seq4.NumberOfScrewsInRow_xDirection * seq4.NumberOfScrewsInColumn_yDirection];
                RectangularSequences.Add(seq4);

                // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
                IHolesNumber = 0;

                foreach (CScrewRectSequence seq in RectangularSequences)
                    IHolesNumber += seq.NumberOfScrewsInRow_xDirection * seq.NumberOfScrewsInColumn_yDirection;
            }

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public Point[] Get_ScrewSequencePointCoordinates(CScrewRectSequence srectSeq)
        {
            // Connectors in Sequence
            return GetRegularArrayOfPointsInCartesianCoordinates(srectSeq.ReferencePoint, srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
        }

        public void Calc_HolesCentersCoord2DApexPlate(
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad,
            ref Point[] HolesCentersPoints)
        {
            // Coordinates of [0,0] of sequence point on plate
            float fx_c = 0.05f;
            float fy_c = 0.05f;

            // Left side
            CScrewRectSequence seq1 = RectangularSequences[0]; // TODO - Doriesit ako pristupovat k premennym v struct a menit ich, neda sa odkazovat referenciou
            seq1.HolesCentersPoints = Get_ScrewSequencePointCoordinates(RectangularSequences[0]);
            RectangularSequences[0] = seq1;

            CScrewRectSequence seq2 = RectangularSequences[1];
            seq2.HolesCentersPoints = Get_ScrewSequencePointCoordinates(RectangularSequences[1]);
            RectangularSequences[1] = seq2;

            // Rotate screws by roof slope
            // Rotate about [0,0]
            RotateSequence_CCW_rad(0, 0, fSlope_rad, RectangularSequences[0]);
            RotateSequence_CCW_rad(0, 0, fSlope_rad, RectangularSequences[1]);

            // Translate from [0,0] on plate to the final position
            TranslateSequence(fx_c, fy_c, RectangularSequences[0]);
            TranslateSequence(fx_c, fy_c, RectangularSequences[1]);

            // Right side
            CScrewRectSequence seq3 = RectangularSequences[0];
            seq3.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * fbX, RectangularSequences[0]);
            CScrewRectSequence seq4 = RectangularSequences[1];
            seq4.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * fbX, RectangularSequences[1]);

            // Add mirrored sequences into the list
            RectangularSequences.Add(seq3);
            RectangularSequences.Add(seq4);

            // Fill array of holes centers
            int iPointIndex = 0;
            for (int i = 0; i < RectangularSequences.Count; i++) // Add each sequence
            {
                for (int j = 0; j < RectangularSequences[i].HolesCentersPoints.Length / 2; j++) // Add each point in the sequence
                {
                    HolesCentersPoints2D[iPointIndex + j].X = RectangularSequences[i].HolesCentersPoints[j].X;
                    HolesCentersPoints2D[iPointIndex + j].Y = RectangularSequences[i].HolesCentersPoints[j].Y;
                }

                iPointIndex += RectangularSequences[i].HolesCentersPoints.Length / 2;
            }

            // TODO - temporary nastavit pre pole suradnic ktore je sucastou plate
            // teoereticky moze mat usporadanie iny pocet ako je pocet na plate, napriklad ak sa usporiadanie odzrkadli alebo skopiruje vramci plochy (napr. Typ KE)
            HolesCentersPoints = HolesCentersPoints2D;
        }

        public void Calc_HolesCentersCoord2DKneePlate(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float fSlope_rad,
            ref Point[] HolesCentersPoints)
        {
            // Coordinates of [0,0] of sequence point on plate
            float fx_cBG = flZ + FCrscRafterDepth;
            float fy_cBG = 0.0f;

            float fx_cUG = flZ + FCrscRafterDepth * (float)Math.Sin(fSlope_rad);
            float fy_cUG = fhY_1 - FCrscRafterDepth * (float)Math.Cos(fSlope_rad);

            // Bottom group - column
            CScrewRectSequence seq1 = RectangularSequences[0]; // TODO - Doriesit ako pristupovat k premennym v struct a menit ich, neda sa odkazovat referenciou
            seq1.HolesCentersPoints = Get_ScrewSequencePointCoordinates(RectangularSequences[0]);
            RectangularSequences[0] = seq1;

            CScrewRectSequence seq2 = RectangularSequences[1];
            seq2.HolesCentersPoints = Get_ScrewSequencePointCoordinates(RectangularSequences[1]);
            RectangularSequences[1] = seq2;

            // Rotate screws by colum slope (bottom group only)
            // Rotate about [0,0] 90 deg
            RotateSequence_CCW_rad(0, 0, 0.5f * (float)Math.PI, RectangularSequences[0]);
            RotateSequence_CCW_rad(0, 0, 0.5f * (float)Math.PI, RectangularSequences[1]);

            // Upper group - rafter
            CScrewRectSequence seq3 = RectangularSequences[0]; // TODO - Doriesit ako pristupovat k premennym v struct a menit ich, neda sa odkazovat referenciou
            seq3.HolesCentersPoints = Get_ScrewSequencePointCoordinates(RectangularSequences[2]);
            RectangularSequences[2] = seq3;

            CScrewRectSequence seq4 = RectangularSequences[1];
            seq4.HolesCentersPoints = Get_ScrewSequencePointCoordinates(RectangularSequences[3]);
            RectangularSequences[3] = seq4;

            // Rotate screws by roof slope (upper group only)
            // Rotate about [0,0]
            RotateSequence_CCW_rad(0, 0, fSlope_rad, RectangularSequences[2]);
            RotateSequence_CCW_rad(0, 0, fSlope_rad, RectangularSequences[3]);

            // Translate from [0,0] on plate to the final position
            // Bottom Group
            TranslateSequence(fx_cBG, fy_cBG, RectangularSequences[0]);
            TranslateSequence(fx_cBG, fy_cBG, RectangularSequences[1]);

            // Upper Group
            TranslateSequence(fx_cUG, fy_cUG, RectangularSequences[2]);
            TranslateSequence(fx_cUG, fy_cUG, RectangularSequences[3]);

            // Fill array of holes centers
            int iPointIndex = 0;
            for (int i = 0; i < RectangularSequences.Count; i++) // Add each sequence
            {
                for (int j = 0; j < RectangularSequences[i].HolesCentersPoints.Length; j++) // Add each point in the sequence
                {
                    HolesCentersPoints2D[iPointIndex + j] = RectangularSequences[i].HolesCentersPoints[j];
                }

                iPointIndex += RectangularSequences[i].HolesCentersPoints.Length;
            }

            // TODO - temporary nastavit pre pole suradnic ktore je sucastou plate
            // teoereticky moze mat usporadanie iny pocet ako je pocet na plate, napriklad ak sa usporiadanie odzrkadli alebo skopiruje vramci plochy (napr. Typ KE)
            //HolesCentersPoints = HolesCentersPoints2D;
        }

        public Point[] GetMirroredSequenceAboutY(float fXDistanceOfMirrorAxis, CScrewSequence InputSequence)
        {
            Point[] OutputSequence = new  Point[InputSequence.HolesCentersPoints.Length];
            for (int i = 0; i< InputSequence.HolesCentersPoints.Length; i++)
            {
                OutputSequence[i].X = 2 * fXDistanceOfMirrorAxis + InputSequence.HolesCentersPoints[i].X * (-1f);
                OutputSequence[i].Y = InputSequence.HolesCentersPoints[i].Y;
            }

            return OutputSequence;
        }

        public void RotateSequence_CCW_rad(float fRotationCenterPoint_x, float fRotationCenterPoint_y, float fRotationAngle_rad, CScrewRectSequence sequence)
        {
            Point[] seqPoints = sequence.HolesCentersPoints;
            Geom2D.TransformPositions_CCW_rad(fRotationCenterPoint_x, fRotationCenterPoint_y, fRotationAngle_rad, ref seqPoints);
            sequence.HolesCentersPoints = seqPoints; // Skontrolovat ci je to potrebne nastavit
        }

        public void TranslateSequence(float fPoint_x, float fPoint_y, CScrewRectSequence sequence)
        {
            Point[] seqPoints = sequence.HolesCentersPoints;
            Geom2D.TransformPositions_CCW_rad(fPoint_x, fPoint_y, 0, ref seqPoints);
            sequence.HolesCentersPoints = seqPoints; // Skontrolovat ci je to potrebne nastavit
        }

        public void Calc_HolesControlPointsCoord3D(float flZ, float ft)
        {
            for (int i = 0; i < IHolesNumber; i++)
            {
                arrConnectorControlPoints3D[i].X = HolesCentersPoints2D[i].X;
                arrConnectorControlPoints3D[i].Y = HolesCentersPoints2D[i].Y - flZ; // Musime odpocitat zalomenie hrany plechu, v 2D zobrazeni sa totiz pripocitalo
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