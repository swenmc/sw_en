using System;
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

        private float[] m_HolesCenterRadii; // Array of screw radii in one group related to the screw arrangement centroid

        public float[] HolesCenterRadii
        {
            get
            {
                return m_HolesCenterRadii;
            }

            set
            {
                m_HolesCenterRadii = value;
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

            ListOfSequenceGroups = new List<CScrewSequenceGroup>(2); // Two group, each for the connection of one member in joint

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

            ListOfSequenceGroups = new List<CScrewSequenceGroup>(2); // Two group, each for the connection of one member in joint

            UpdateArrangmentData();
        }

        public override void UpdateArrangmentData()
        {
            ListOfSequenceGroups.Clear(); // Delete previous data otherwise are added more and more new screws to the list
            ListOfSequenceGroups = new List<CScrewSequenceGroup>(2);
            ListOfSequenceGroups.Add(new CScrewSequenceGroup());

            CScrewRectSequence seq1 = new CScrewRectSequence();
            seq1.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ1;
            seq1.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ1;
            seq1.ReferencePoint = new Point(fx_c_SQ1, fy_c_SQ1);
            seq1.DistanceOfPointsX = fDistanceOfPointsX_SQ1;
            seq1.DistanceOfPointsY = fDistanceOfPointsY_SQ1;
            seq1.INumberOfScrews = seq1.NumberOfScrewsInRow_xDirection * seq1.NumberOfScrewsInColumn_yDirection;
            seq1.HolesCentersPoints = new Point[seq1.INumberOfScrews];
            ListOfSequenceGroups[0].ListScrewSequence.Add(seq1);

            CScrewRectSequence seq2 = new CScrewRectSequence();
            seq2.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ2;
            seq2.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ2;
            seq2.ReferencePoint = new Point(fx_c_SQ2, fy_c_SQ2);
            seq2.DistanceOfPointsX = fDistanceOfPointsX_SQ2;
            seq2.DistanceOfPointsY = fDistanceOfPointsY_SQ2;
            seq2.INumberOfScrews = seq2.NumberOfScrewsInRow_xDirection * seq2.NumberOfScrewsInColumn_yDirection;
            seq2.HolesCentersPoints = new Point[seq2.INumberOfScrews];
            ListOfSequenceGroups[0].ListScrewSequence.Add(seq2);

            ListOfSequenceGroups[0].NumberOfHalfCircleSequences = 0;
            ListOfSequenceGroups[0].NumberOfRectangularSequences = 2;

            // Celkovy pocet skrutiek
            // Definovane su len sekvencie v jednej group, ocakava sa ze pocet v groups je rovnaky a hodnoty sa skopiruju (napr. pre apex plate)
            RecalculateTotalNumberOfScrews();

            int iNumberOfGroupsInPlate = 2;

            IHolesNumber *= iNumberOfGroupsInPlate;

            if (iNumberOfScrewsInRow_xDirection_SQ3 != 0 && iNumberOfScrewsInColumn_yDirection_SQ3 != 0 &&
               iNumberOfScrewsInRow_xDirection_SQ4 != 0 && iNumberOfScrewsInColumn_yDirection_SQ4 != 0)
            {
                ListOfSequenceGroups.Add(new CScrewSequenceGroup());

                CScrewRectSequence seq3 = new CScrewRectSequence();
                seq3.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ3;
                seq3.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ3;
                seq3.ReferencePoint = new Point(fx_c_SQ3, fy_c_SQ3);
                seq3.DistanceOfPointsX = fDistanceOfPointsX_SQ3;
                seq3.DistanceOfPointsY = fDistanceOfPointsY_SQ3;
                seq3.INumberOfScrews = seq3.NumberOfScrewsInRow_xDirection * seq3.NumberOfScrewsInColumn_yDirection;
                seq3.HolesCentersPoints = new Point[seq3.INumberOfScrews];
                ListOfSequenceGroups[1].ListScrewSequence.Add(seq3);

                CScrewRectSequence seq4 = new CScrewRectSequence();
                seq4.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ4;
                seq4.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ4;
                seq4.ReferencePoint = new Point(fx_c_SQ4, fy_c_SQ4);
                seq4.DistanceOfPointsX = fDistanceOfPointsX_SQ4;
                seq4.DistanceOfPointsY = fDistanceOfPointsY_SQ4;
                seq4.INumberOfScrews = seq3.NumberOfScrewsInRow_xDirection * seq4.NumberOfScrewsInColumn_yDirection;
                seq4.HolesCentersPoints = new Point[seq4.INumberOfScrews];
                ListOfSequenceGroups[1].ListScrewSequence.Add(seq4);

                ListOfSequenceGroups[1].NumberOfHalfCircleSequences = 0;
                ListOfSequenceGroups[1].NumberOfRectangularSequences = 2;

                // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
                RecalculateTotalNumberOfScrews();
            }

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public Point[] Get_ScrewSequencePointCoordinates(CScrewRectSequence srectSeq)
        {
            // Connectors in Sequence
            return GetRegularArrayOfPointsInCartesianCoordinates(srectSeq.ReferencePoint, srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
        }

        public override void Calc_HolesCentersCoord2DApexPlate(
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        {
            // Coordinates of [0,0] of sequence point on plate
            float fx_c = 0.05f;
            float fy_c = 0.05f;

            // Left side
            ListOfSequenceGroups[0].ListScrewSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[0]);
            ListOfSequenceGroups[0].ListScrewSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[1]);

            // Rotate screws by roof slope
            // Rotate about [0,0]
            RotateSequence_CCW_rad(0, 0, fSlope_rad, (CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[0]);
            RotateSequence_CCW_rad(0, 0, fSlope_rad, (CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[1]);

            // Translate from [0,0] on plate to the final position
            TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[0]);
            TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[1]);

            // Right side
            CScrewRectSequence seq3 = new CScrewRectSequence();
            seq3.HolesCentersPoints = ListOfSequenceGroups[0].ListScrewSequence[0].HolesCentersPoints;
            seq3.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * fbX, seq3);

            CScrewRectSequence seq4 = new CScrewRectSequence();
            seq4.HolesCentersPoints = ListOfSequenceGroups[0].ListScrewSequence[1].HolesCentersPoints;
            seq4.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * fbX, seq4);

            // Add mirrored sequences into the list
            if (ListOfSequenceGroups.Count == 1) // Just in case that mirrored (right side) group doesn't exists
            {
                ListOfSequenceGroups.Add(new CScrewSequenceGroup()); // Right Side Group

                ListOfSequenceGroups[1].ListScrewSequence.Add(seq3);
                ListOfSequenceGroups[1].ListScrewSequence.Add(seq4);
                ListOfSequenceGroups[1].NumberOfRectangularSequences = 2;
            }
            else // In case that group already exists set current sequences
            {
                ListOfSequenceGroups[1].ListScrewSequence[0] = seq3;
                ListOfSequenceGroups[1].ListScrewSequence[1] = seq4;
                ListOfSequenceGroups[1].NumberOfRectangularSequences = 2;
            }

            FillArrayOfHolesCentersInWholeArrangement();
        }

        public override void Calc_HolesCentersCoord2DKneePlate(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        {
            // Coordinates of [0,0] of sequence point on plate
            float fx_cBG = flZ + FCrscRafterDepth;
            float fy_cBG = 0.0f;

            float fx_cUG = flZ + FCrscRafterDepth * (float)Math.Sin(fSlope_rad);
            float fy_cUG = fhY_1 - FCrscRafterDepth * (float)Math.Cos(fSlope_rad);

            // Bottom group - column
            ListOfSequenceGroups[0].ListScrewSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[0]);
            ListOfSequenceGroups[0].ListScrewSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[1]);

            // Rotate screws by colum slope (bottom group only)
            // Rotate about [0,0] 90 deg
            RotateSequence_CCW_rad(0, 0, 0.5f * (float)Math.PI, (CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[0]);
            RotateSequence_CCW_rad(0, 0, 0.5f * (float)Math.PI, (CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[1]);

            // Upper group - rafter
            ListOfSequenceGroups[1].ListScrewSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[1].ListScrewSequence[0]);
            ListOfSequenceGroups[1].ListScrewSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[1].ListScrewSequence[1]);

            // Rotate screws by roof slope (upper group only)
            // Rotate about [0,0]
            RotateSequence_CCW_rad(0, 0, fSlope_rad, (CScrewRectSequence)ListOfSequenceGroups[1].ListScrewSequence[0]);
            RotateSequence_CCW_rad(0, 0, fSlope_rad, (CScrewRectSequence)ListOfSequenceGroups[1].ListScrewSequence[1]);

            // Translate from [0,0] on plate to the final position
            // Bottom Group
            TranslateSequence(fx_cBG, fy_cBG, (CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[0]);
            TranslateSequence(fx_cBG, fy_cBG, (CScrewRectSequence)ListOfSequenceGroups[0].ListScrewSequence[1]);

            // Upper Group
            TranslateSequence(fx_cUG, fy_cUG, (CScrewRectSequence)ListOfSequenceGroups[1].ListScrewSequence[0]);
            TranslateSequence(fx_cUG, fy_cUG, (CScrewRectSequence)ListOfSequenceGroups[1].ListScrewSequence[1]);

            FillArrayOfHolesCentersInWholeArrangement();
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

        public override void Calc_HolesControlPointsCoord3D(float flZ, float ft)
        {
            for (int i = 0; i < IHolesNumber; i++)
            {
                arrConnectorControlPoints3D[i].X = HolesCentersPoints2D[i].X;
                arrConnectorControlPoints3D[i].Y = HolesCentersPoints2D[i].Y - flZ; // Musime odpocitat zalomenie hrany plechu, v 2D zobrazeni sa totiz pripocitalo
                arrConnectorControlPoints3D[i].Z = -ft; // TODO Position depends on screw length;
            }
        }

        public override void GenerateConnectors()
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