using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Globalization;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    // BA, BB, BC, BG
    // Two web stiffeners, 6 segments
    // 270 single and nested

    [Serializable]
    public class CScrewArrangement_BX_2:CScrewArrangement
    {
        private float m_fCrscColumnDepth;

        public float FCrscColumnDepth
        {
            get
            {
                return m_fCrscColumnDepth;
            }

            set
            {
                m_fCrscColumnDepth = value;
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

        float m_fStiffenerSize; // Cross-section stiffener dimension (without screws)

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

        // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii

        // Left group
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
        public int iNumberOfScrewsInRow_xDirection_SQ3;
        public int iNumberOfScrewsInColumn_yDirection_SQ3;
        public float fx_c_SQ3;
        public float fy_c_SQ3;
        public float fDistanceOfPointsX_SQ3;
        public float fDistanceOfPointsY_SQ3;

        // Right group
        public int iNumberOfScrewsInRow_xDirection_SQ4;
        public int iNumberOfScrewsInColumn_yDirection_SQ4;
        public float fx_c_SQ4;
        public float fy_c_SQ4;
        public float fDistanceOfPointsX_SQ4;
        public float fDistanceOfPointsY_SQ4;
        public int iNumberOfScrewsInRow_xDirection_SQ5;
        public int iNumberOfScrewsInColumn_yDirection_SQ5;
        public float fx_c_SQ5;
        public float fy_c_SQ5;
        public float fDistanceOfPointsX_SQ5;
        public float fDistanceOfPointsY_SQ5;
        public int iNumberOfScrewsInRow_xDirection_SQ6;
        public int iNumberOfScrewsInColumn_yDirection_SQ6;
        public float fx_c_SQ6;
        public float fy_c_SQ6;
        public float fDistanceOfPointsX_SQ6;
        public float fDistanceOfPointsY_SQ6;

        public CScrewArrangement_BX_2() { }

        public CScrewArrangement_BX_2(
            CScrew referenceScrew_temp,
            float fCrscColumnDepth_temp,
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
            float fDistanceOfPointsY_SQ2_temp,
            int iNumberOfScrewsInRow_xDirection_SQ3_temp,
            int iNumberOfScrewsInColumn_yDirection_SQ3_temp,
            float fx_c_SQ3_temp,
            float fy_c_SQ3_temp,
            float fDistanceOfPointsX_SQ3_temp,
            float fDistanceOfPointsY_SQ3_temp

            ) : base(iNumberOfScrewsInRow_xDirection_SQ1_temp * iNumberOfScrewsInColumn_yDirection_SQ1_temp +
                     iNumberOfScrewsInRow_xDirection_SQ2_temp * iNumberOfScrewsInColumn_yDirection_SQ2_temp +
                     iNumberOfScrewsInRow_xDirection_SQ3_temp * iNumberOfScrewsInColumn_yDirection_SQ3_temp,
                     referenceScrew_temp)
        {
            referenceScrew = referenceScrew_temp;
            FCrscColumnDepth = fCrscColumnDepth_temp;
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

            iNumberOfScrewsInRow_xDirection_SQ3 = iNumberOfScrewsInRow_xDirection_SQ3_temp;
            iNumberOfScrewsInColumn_yDirection_SQ3 = iNumberOfScrewsInColumn_yDirection_SQ3_temp;
            fx_c_SQ3 = fx_c_SQ3_temp;
            fy_c_SQ3 = fy_c_SQ3_temp;
            fDistanceOfPointsX_SQ3 = fDistanceOfPointsX_SQ3_temp;
            fDistanceOfPointsY_SQ3 = fDistanceOfPointsY_SQ3_temp;

            ListOfSequenceGroups = new List<CScrewSequenceGroup>(2); // Two groups,one group on one side of column member

            UpdateArrangmentData();
        }

        public override void UpdateArrangmentData()
        {
            // TODO - toto prerobit tak ze sa parametre prevedu na cisla a nastavia v CTEKScrewsManager a nie tu
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            // Update reference screw properties
            DATABASE.DTO.CTEKScrewProperties screwProp = DATABASE.CTEKScrewsManager.GetScrewProperties(referenceScrew.Gauge.ToString());
            referenceScrew.Diameter_thread = float.Parse(screwProp.threadDiameter, nfi) / 1000; // Convert mm to m

            ListOfSequenceGroups.Clear(); // Delete previous data otherwise are added more and more new screws to the list
            ListOfSequenceGroups = new List<CScrewSequenceGroup>(2);
            ListOfSequenceGroups.Add(new CScrewSequenceGroup());

            CScrewRectSequence seq1 = new CScrewRectSequence();
            seq1.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ1;
            seq1.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ1;
            seq1.ReferencePoint = new Point(fx_c_SQ1, fy_c_SQ1);
            seq1.DistanceOfPointsX = fDistanceOfPointsX_SQ1;
            seq1.DistanceOfPointsY = fDistanceOfPointsY_SQ1;
            seq1.INumberOfConnectors = seq1.NumberOfScrewsInRow_xDirection * seq1.NumberOfScrewsInColumn_yDirection;
            seq1.HolesCentersPoints = new Point[seq1.INumberOfConnectors];
            ListOfSequenceGroups[0].ListSequence.Add(seq1);

            CScrewRectSequence seq2 = new CScrewRectSequence();
            seq2.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ2;
            seq2.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ2;
            seq2.ReferencePoint = new Point(fx_c_SQ2, fy_c_SQ2);
            seq2.DistanceOfPointsX = fDistanceOfPointsX_SQ2;
            seq2.DistanceOfPointsY = fDistanceOfPointsY_SQ2;
            seq2.INumberOfConnectors  = seq2.NumberOfScrewsInRow_xDirection * seq2.NumberOfScrewsInColumn_yDirection;
            seq2.HolesCentersPoints = new Point[seq2.INumberOfConnectors];
            ListOfSequenceGroups[0].ListSequence.Add(seq2);

            CScrewRectSequence seq3 = new CScrewRectSequence();
            seq3.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ3;
            seq3.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ3;
            seq3.ReferencePoint = new Point(fx_c_SQ3, fy_c_SQ3);
            seq3.DistanceOfPointsX = fDistanceOfPointsX_SQ3;
            seq3.DistanceOfPointsY = fDistanceOfPointsY_SQ3;
            seq3.INumberOfConnectors = seq3.NumberOfScrewsInRow_xDirection * seq3.NumberOfScrewsInColumn_yDirection;
            seq3.HolesCentersPoints = new Point[seq3.INumberOfConnectors];
            ListOfSequenceGroups[0].ListSequence.Add(seq3);

            ListOfSequenceGroups[0].NumberOfHalfCircleSequences = 0;
            ListOfSequenceGroups[0].NumberOfRectangularSequences = 3;

            if (iNumberOfScrewsInRow_xDirection_SQ4 != 0 && iNumberOfScrewsInColumn_yDirection_SQ4 != 0 &&
                iNumberOfScrewsInRow_xDirection_SQ5 != 0 && iNumberOfScrewsInColumn_yDirection_SQ5 != 0 &&
                iNumberOfScrewsInRow_xDirection_SQ6 != 0 && iNumberOfScrewsInColumn_yDirection_SQ6 != 0)
            {
                ListOfSequenceGroups.Add(new CScrewSequenceGroup());

                CScrewRectSequence seq4 = new CScrewRectSequence();
                seq4.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ4;
                seq4.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ4;
                seq4.ReferencePoint = new Point(fx_c_SQ4, fy_c_SQ4);
                seq4.DistanceOfPointsX = fDistanceOfPointsX_SQ4;
                seq4.DistanceOfPointsY = fDistanceOfPointsY_SQ4;
                seq4.INumberOfConnectors = seq4.NumberOfScrewsInRow_xDirection * seq4.NumberOfScrewsInColumn_yDirection;
                seq4.HolesCentersPoints = new Point[seq4.INumberOfConnectors];
                ListOfSequenceGroups[1].ListSequence.Add(seq4);

                CScrewRectSequence seq5 = new CScrewRectSequence();
                seq5.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ5;
                seq5.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ5;
                seq5.ReferencePoint = new Point(fx_c_SQ5, fy_c_SQ5);
                seq5.DistanceOfPointsX = fDistanceOfPointsX_SQ5;
                seq5.DistanceOfPointsY = fDistanceOfPointsY_SQ5;
                seq5.INumberOfConnectors = seq5.NumberOfScrewsInRow_xDirection * seq5.NumberOfScrewsInColumn_yDirection;
                seq5.HolesCentersPoints = new Point[seq5.INumberOfConnectors];
                ListOfSequenceGroups[1].ListSequence.Add(seq5);

                CScrewRectSequence seq6 = new CScrewRectSequence();
                seq6.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ6;
                seq6.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ6;
                seq6.ReferencePoint = new Point(fx_c_SQ6, fy_c_SQ6);
                seq6.DistanceOfPointsX = fDistanceOfPointsX_SQ6;
                seq6.DistanceOfPointsY = fDistanceOfPointsY_SQ6;
                seq6.INumberOfConnectors = seq6.NumberOfScrewsInRow_xDirection * seq6.NumberOfScrewsInColumn_yDirection;
                seq6.HolesCentersPoints = new Point[seq6.INumberOfConnectors];
                ListOfSequenceGroups[1].ListSequence.Add(seq6);

                ListOfSequenceGroups[1].NumberOfHalfCircleSequences = 0;
                ListOfSequenceGroups[1].NumberOfRectangularSequences = 3;

                // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
                RecalculateTotalNumberOfScrews();
            }
            else
            {
                // Celkovy pocet skrutiek
                // Definovane su len sekvencie v jednej group, ocakava sa ze pocet v groups je rovnaky a hodnoty sa skopiruju (napr. pre apex plate)
                RecalculateTotalNumberOfScrews();
                int iNumberOfGroupsInPlate = 2;
                IHolesNumber *= iNumberOfGroupsInPlate;
            }

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public Point[] Get_ScrewSequencePointCoordinates(CScrewRectSequence srectSeq)
        {
            // Connectors in Sequence
            return GetRegularArrayOfPointsInCartesianCoordinates(srectSeq.ReferencePoint, srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
        }

        public override void Calc_HolesCentersCoord2DBasePlate(
            float fbX,
            float flZ,
            float fhY)
        {
            // Coordinates of [0,0] of sequence point on plate (used to translate all sequences in the group)
            float fx_c = 0.000f;
            float fy_c = 0.000f;

            // Left side
            ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            ListOfSequenceGroups[0].ListSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);
            ListOfSequenceGroups[0].ListSequence[2].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[2]);

            // Set radii of connectors / screws in the group
            ListOfSequenceGroups[0].HolesRadii = ListOfSequenceGroups[0].Get_RadiiOfConnectorsInGroup();

            // Translate from [0,0] on plate to the final position
            TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);
            TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[2]);

            // Right side
            CScrewRectSequence seq4 = new CScrewRectSequence();
            seq4.HolesCentersPoints = ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints;
            seq4.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * (2 * flZ + fbX), seq4);

            CScrewRectSequence seq5 = new CScrewRectSequence();
            seq5.HolesCentersPoints = ListOfSequenceGroups[0].ListSequence[1].HolesCentersPoints;
            seq5.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * (2 * flZ + fbX), seq5);

            CScrewRectSequence seq6 = new CScrewRectSequence();
            seq6.HolesCentersPoints = ListOfSequenceGroups[0].ListSequence[2].HolesCentersPoints;
            seq6.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * (2 * flZ + fbX), seq6);

            // Add mirrored sequences into the list
            if (ListOfSequenceGroups.Count == 1) // Just in case that mirrored (right side) group doesn't exists
            {
                ListOfSequenceGroups.Add(new CScrewSequenceGroup()); // Right Side Group

                ListOfSequenceGroups[1].ListSequence.Add(seq4);
                ListOfSequenceGroups[1].ListSequence.Add(seq5);
                ListOfSequenceGroups[1].ListSequence.Add(seq6);
                ListOfSequenceGroups[1].NumberOfRectangularSequences = 3;
            }
            else // In case that group already exists set current sequences
            {
                ListOfSequenceGroups[1].ListSequence[0] = seq4;
                ListOfSequenceGroups[1].ListSequence[1] = seq5;
                ListOfSequenceGroups[1].ListSequence[2] = seq6;
                ListOfSequenceGroups[1].NumberOfRectangularSequences = 3;
            }

            ListOfSequenceGroups[1].HolesRadii = ListOfSequenceGroups[1].Get_RadiiOfConnectorsInGroup();

            FillArrayOfHolesCentersInWholeArrangement();
        }

        public override void Calc_BasePlateData(
            float fbX,
            float flZ,
            float fhY,
            float ft)
        {
            Calc_HolesCentersCoord2DBasePlate(fbX, flZ, fhY);
            Calc_HolesControlPointsCoord3D(fbX, flZ, ft);
            GenerateConnectors_BasePlate();
        }

        // Povodne funkcie - refaktorovat s CScrewArrangement_BX_1.cs

        void Calc_HolesControlPointsCoord3D(float fbX, float flZ, float ft)
        {
            // Left group
            int iGroupIndex = 0;
            int iLastItemIndex = 0;
            for (int i = 0; i < ListOfSequenceGroups[iGroupIndex].ListSequence.Count; i++)
            {
                for (int j = 0; j < ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length; j++)
                {
                    arrConnectorControlPoints3D[iLastItemIndex + j].X = - ft; // TODO Position depends on screw length
                    arrConnectorControlPoints3D[iLastItemIndex + j].Y = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].Y;
                    arrConnectorControlPoints3D[iLastItemIndex + j].Z =  flZ - ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].X;
                }

                iLastItemIndex += ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length;
            }

            // Right group
            iGroupIndex = 1;
            iLastItemIndex = 0;
            for (int i = 0; i < ListOfSequenceGroups[iGroupIndex].ListSequence.Count; i++)
            {
                for (int j = 0; j < ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length; j++)
                {
                    arrConnectorControlPoints3D[IHolesNumber / 2 + iLastItemIndex + j].X = fbX + ft; // TODO Position depends on screw length
                    arrConnectorControlPoints3D[IHolesNumber / 2 + iLastItemIndex + j].Y = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].Y;
                    arrConnectorControlPoints3D[IHolesNumber / 2 + iLastItemIndex + j].Z = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].X - flZ - fbX;
                }

                iLastItemIndex += ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length;
            }
        }

        void GenerateConnectors_BasePlate()
        {
            Screws = new CScrew[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);

                if(i < IHolesNumber / 2) // Left side (rotation 0 deg about y-axis)
                    Screws[i] = new CScrew(referenceScrew.Name, controlpoint, referenceScrew.Gauge, referenceScrew.Diameter_thread, referenceScrew.D_h_headdiameter, referenceScrew.D_w_washerdiameter, referenceScrew.T_w_washerthickness, referenceScrew.Length, referenceScrew.Mass, 0, 0, 0, true);
                else // Right side (rotation 180 deg about y-axis)
                    Screws[i] = new CScrew(referenceScrew.Name, controlpoint, referenceScrew.Gauge, referenceScrew.Diameter_thread, referenceScrew.D_h_headdiameter, referenceScrew.D_w_washerdiameter, referenceScrew.T_w_washerthickness, referenceScrew.Length, referenceScrew.Mass, 0, 180, 0, true);
            }
        }

        public Point[] GetMirroredSequenceAboutY(float fXDistanceOfMirrorAxis, CScrewSequence InputSequence)
        {
            Point[] OutputSequence = new Point[InputSequence.HolesCentersPoints.Length];
            for (int i = 0; i < InputSequence.HolesCentersPoints.Length; i++)
            {
                OutputSequence[i].X = 2 * fXDistanceOfMirrorAxis + InputSequence.HolesCentersPoints[i].X * (-1f);
                OutputSequence[i].Y = InputSequence.HolesCentersPoints[i].Y;
            }

            return OutputSequence;
        }

        public void TranslateSequence(float fPoint_x, float fPoint_y, CScrewRectSequence sequence)
        {
            Point[] seqPoints = sequence.HolesCentersPoints;
            Geom2D.TransformPositions_CCW_rad(fPoint_x, fPoint_y, 0, ref seqPoints);
            sequence.HolesCentersPoints = seqPoints; // je to potrebne takto nastavovat lebo nie je mozne volat [ref sequence.HolesCentersPoints]
        }
    }
}
