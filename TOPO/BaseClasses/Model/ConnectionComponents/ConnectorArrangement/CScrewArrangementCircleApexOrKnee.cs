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
    public class CScrewArrangementCircleApexOrKnee : CScrewArrangement
    {
        private int m_iHolesInCirclesNumber; // Pocet skrutiek v dvoch kruhoch

        public int IHolesInCirclesNumber
        {
            get
            {
                return m_iHolesInCirclesNumber;
            }

            set
            {
                m_iHolesInCirclesNumber = value;
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


        private int m_iNumberOfGroupsInJoint = 2; // Pocet kruhov na jednom plechu

        public int INumberOfGroupsInJoint
        {
            get
            {
                return m_iNumberOfGroupsInJoint;
            }

            set
            {
                m_iNumberOfGroupsInJoint = value;
            }
        }

        int m_iNumberOfCirclesInGroup = 1; // pocet kruhov v jednej skupine (group)

        public int INumberOfCirclesInGroup
        {
            get
            {
                return m_iNumberOfCirclesInGroup;
            }

            set
            {
                m_iNumberOfCirclesInGroup = value;
            }
        }

        int m_iNumberOfCircleSequencesInGroup = 2; // pocet polkruhov v "kruhu" na jednom plechu (sekvencia - sequence)

        public int INumberOfCircleSequencesInGroup
        {
            get
            {
                return m_iNumberOfCircleSequencesInGroup;
            }

            set
            {
                m_iNumberOfCircleSequencesInGroup = value;
            }
        }

        private int iNumberOfAdditionalConnectorsInOneSequence;
 
        public float[] HolesCenterRadii;

        // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii
        public int iNumberOfCircleScrewsSequencesInOneGroup = 2;

        private int iNumberOfScrewsInOneHalfCircleSequence_SQ1; // pocet skrutiek v "polkruhu"

        public int INumberOfScrewsInOneHalfCircleSequence_SQ1
        {
            get
            {
                return iNumberOfScrewsInOneHalfCircleSequence_SQ1;
            }

            set
            {
                iNumberOfScrewsInOneHalfCircleSequence_SQ1 = value;
            }
        }

        private float m_fRadius_SQ1;

        public float FRadius_SQ1
        {
            get
            {
                return m_fRadius_SQ1;
            }

            set
            {
                m_fRadius_SQ1 = value;
            }
        }

        // Corner screws
        private bool m_bUseAdditionalCornerScrews; // Pocet skrutiek v rohoch - celkovo 4 skrutky * 4 rohy * 2 kruhy

        public bool BUseAdditionalCornerScrews
        {
            get
            {
                return m_bUseAdditionalCornerScrews;
            }

            set
            {
                m_bUseAdditionalCornerScrews = value;
            }
        }

        private int m_iAdditionalConnectorNumberInRow_xDirection;

        public int IAdditionalConnectorNumberInRow_xDirection
        {
            get
            {
                return m_iAdditionalConnectorNumberInRow_xDirection;
            }

            set
            {
                m_iAdditionalConnectorNumberInRow_xDirection = value;
            }
        }

        private int m_iAdditionalConnectorNumberInColumn_yDirection;

        public int IAdditionalConnectorNumberInColumn_yDirection
        {
            get
            {
                return m_iAdditionalConnectorNumberInColumn_yDirection;
            }

            set
            {
                m_iAdditionalConnectorNumberInColumn_yDirection = value;
            }
        }

        private int m_iAdditionalConnectorNumber;

        public int IAdditionalConnectorNumber
        {
            get
            {
                return m_iAdditionalConnectorNumber;
            }

            set
            {
                m_iAdditionalConnectorNumber = value;
            }
        }

        private int m_iAdditionalConnectorInCornerNumber;

        public int IAdditionalConnectorInCornerNumber
        {
            get
            {
                return m_iAdditionalConnectorInCornerNumber;
            }

            set
            {
                m_iAdditionalConnectorInCornerNumber = value;
            }
        }

        private float m_fAdditionalScrewsDistance_x;

        public float FAdditionalScrewsDistance_x
        {
            get
            {
                return m_fAdditionalScrewsDistance_x;
            }

            set
            {
                m_fAdditionalScrewsDistance_x = value;
            }
        }

        private float m_fAdditionalScrewsDistance_y;

        public float FAdditionalScrewsDistance_y
        {
            get
            {
                return m_fAdditionalScrewsDistance_y;
            }

            set
            {
                m_fAdditionalScrewsDistance_y = value;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public CScrewArrangementCircleApexOrKnee()
        { }

        public CScrewArrangementCircleApexOrKnee(
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            int iNumberOfCirclesInGroup_temp,
            int iNumberOfScrewsInOneHalfCircleSequence_SQ1_temp,
            float fRadius_SQ1_temp,
            bool bUseAdditionalCornerScrews_temp,
            int iAdditionalConnectorInCornerNumber_temp,
            float fAdditionalScrewsDistance_x_temp,
            float fAdditionalScrewsDistance_y_temp)
        {
            referenceScrew = referenceScrew_temp;
            FCrscRafterDepth = fCrscRafterDepth_temp;
            FCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            FStiffenerSize = fStiffenerSize_temp;

            // Circle
            // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii
            INumberOfCirclesInGroup = iNumberOfCirclesInGroup_temp; // pocet kruhov
            INumberOfScrewsInOneHalfCircleSequence_SQ1 = iNumberOfScrewsInOneHalfCircleSequence_SQ1_temp; // Pocet v segmente kruhu
            FRadius_SQ1 = fRadius_SQ1_temp;

            // Corner screws parameters
            BUseAdditionalCornerScrews = bUseAdditionalCornerScrews_temp;
            IAdditionalConnectorInCornerNumber = iAdditionalConnectorInCornerNumber_temp; // Spolu v jendom rohu
            IAdditionalConnectorNumberInRow_xDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber); // v smere x, pocet v riadku
            IAdditionalConnectorNumberInColumn_yDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber); // v smere y, pocet v stlpci
            FAdditionalScrewsDistance_x = fAdditionalScrewsDistance_x_temp;
            FAdditionalScrewsDistance_y = fAdditionalScrewsDistance_y_temp;

            ListOfSequenceGroups = new List<CScrewSequenceGroup>(2); // Two groups, each for the connection of one member in joint

            UpdateArrangmentData();
        }

        public void UpdateArrangmentData()
        {
            ListOfSequenceGroups.Clear(); // Delete previous data otherwise are added more and more new screws to the list
            ListOfSequenceGroups = new List<CScrewSequenceGroup>(2);
            ListOfSequenceGroups.Add(new CScrewSequenceGroup());

            ListOfSequenceGroups[0].NumberOfHalfCircleSequences = 2;

            CScrewHalfCircleSequence seq1 = new CScrewHalfCircleSequence();
            seq1.INumberOfScrews = iNumberOfScrewsInOneHalfCircleSequence_SQ1;
            seq1.Radius = FRadius_SQ1;
            ListOfSequenceGroups[0].ListScrewSequence.Add(seq1);

            CScrewHalfCircleSequence seq2 = new CScrewHalfCircleSequence();
            seq2.INumberOfScrews = iNumberOfScrewsInOneHalfCircleSequence_SQ1;
            seq2.Radius = FRadius_SQ1;
            ListOfSequenceGroups[0].ListScrewSequence.Add(seq2);

            if (BUseAdditionalCornerScrews) // 4 corners in one group
            {
                ListOfSequenceGroups[0].NumberOfRectangularSequences = 4;

                CScrewRectSequence seq_Corner1 = new CScrewRectSequence(IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection);
                seq_Corner1.DistanceOfPointsX = FAdditionalScrewsDistance_x;
                seq_Corner1.DistanceOfPointsY = FAdditionalScrewsDistance_y;
                ListOfSequenceGroups[0].ListScrewSequence.Add(seq_Corner1);

                CScrewRectSequence seq_Corner2 = new CScrewRectSequence(IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection);
                seq_Corner2.DistanceOfPointsX = FAdditionalScrewsDistance_x;
                seq_Corner2.DistanceOfPointsY = FAdditionalScrewsDistance_y;
                ListOfSequenceGroups[0].ListScrewSequence.Add(seq_Corner2);

                CScrewRectSequence seq_Corner3 = new CScrewRectSequence(IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection);
                seq_Corner3.DistanceOfPointsX = FAdditionalScrewsDistance_x;
                seq_Corner3.DistanceOfPointsY = FAdditionalScrewsDistance_y;
                ListOfSequenceGroups[0].ListScrewSequence.Add(seq_Corner3);

                CScrewRectSequence seq_Corner4 = new CScrewRectSequence(IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection);
                seq_Corner4.DistanceOfPointsX = FAdditionalScrewsDistance_x;
                seq_Corner4.DistanceOfPointsY = FAdditionalScrewsDistance_y;
                ListOfSequenceGroups[0].ListScrewSequence.Add(seq_Corner4);
            }

            // Celkovy pocet skrutiek
            // Definovane su len sekvencie v jednej group, ocakava sa ze pocet v groups je rovnaky a hodnoty sa skopiruju (napr. pre apex plate)
            IHolesNumber = 0;

            foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
            {
                foreach (CScrewSequence seq in group.ListScrewSequence)
                    IHolesNumber += seq.INumberOfScrews;
            }

            IHolesNumber *= INumberOfGroupsInJoint;

            if (false) // TODO - rozlisit knee a apex plate
            {
                /*
                ListOfSequenceGroups.Add(new CScrewSequenceGroup());

                CScrewRectSequence seq3 = new CScrewRectSequence();
                seq3.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ3;
                seq3.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ3;
                seq3.ReferencePoint = new Point(fx_c_SQ3, fy_c_SQ3);
                seq3.DistanceOfPointsX = fDistanceOfPointsX_SQ3;
                seq3.DistanceOfPointsY = fDistanceOfPointsY_SQ3;
                seq3.HolesCentersPoints = new Point[seq3.NumberOfScrewsInRow_xDirection * seq3.NumberOfScrewsInColumn_yDirection];
                ListOfSequenceGroups[1].ListScrewSequence.Add(seq3);

                CScrewRectSequence seq4 = new CScrewRectSequence();
                seq4.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ4;
                seq4.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ4;
                seq4.ReferencePoint = new Point(fx_c_SQ4, fy_c_SQ4);
                seq4.DistanceOfPointsX = fDistanceOfPointsX_SQ4;
                seq4.DistanceOfPointsY = fDistanceOfPointsY_SQ4;
                seq4.HolesCentersPoints = new Point[seq4.NumberOfScrewsInRow_xDirection * seq4.NumberOfScrewsInColumn_yDirection];
                ListOfSequenceGroups[1].ListScrewSequence.Add(seq4);
                */
                // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
                IHolesNumber = 0;

                foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
                {
                    foreach (CScrewSequence seq in group.ListScrewSequence)
                        IHolesNumber += seq.INumberOfScrews;
                }
            }

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public void Get_ScrewGroup_IncludingAdditionalScrews(
            float fx_c,
            float fy_c,
            float fAngle_seq_rotation_init_point_deg,
            float fRotation_rad,
            ref CScrewSequenceGroup group)
        {
            float fAngle_seq_rotation_deg = fRotation_rad * 180f / MathF.fPI; // Input value (roof pitch)

            float fAngle_interval_deg = 180 - (2f * fAngle_seq_rotation_init_point_deg); // Angle between sequence center, first and last point in the sequence

            // Half circle sequence
            float[,] fSequenceTop_temp = Geom2D.GetArcPointCoordArray_CCW_deg(FRadius_SQ1, fAngle_seq_rotation_init_point_deg, fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, iNumberOfScrewsInOneHalfCircleSequence_SQ1, false);
            float[,] fSequenceBottom_temp = Geom2D.GetArcPointCoordArray_CCW_deg(FRadius_SQ1, 180 + fAngle_seq_rotation_init_point_deg, 180 + fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, iNumberOfScrewsInOneHalfCircleSequence_SQ1, false);

            if (group.NumberOfHalfCircleSequences > 0)
            {
                // TODO - docasne, previest pole float na pole Points
                Point[] fSequenceTop = Geom2D.GetConvertedFloatToPointArray(fSequenceTop_temp);
                Point[] fSequenceBottom = Geom2D.GetConvertedFloatToPointArray(fSequenceBottom_temp);

                group.ListScrewSequence[0].HolesCentersPoints = fSequenceTop;
                group.ListScrewSequence[1].HolesCentersPoints = fSequenceBottom;
            }

            // Add addtional point the sequences
            if (BUseAdditionalCornerScrews && group.NumberOfRectangularSequences > 0)
            {
                // For square
                if (IAdditionalConnectorNumberInRow_xDirection == 0 && IAdditionalConnectorNumberInColumn_yDirection == 0)
                {
                    IAdditionalConnectorNumberInRow_xDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber);
                    IAdditionalConnectorNumberInColumn_yDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber);
                }

                // Additional corner connectors
                // Top part of group
                Point[] cornerConnectorsInGroupTopLeft = GetAdditionaConnectorsCoordinatesInOneSequence(new Point(-FRadius_SQ1, FRadius_SQ1 - (IAdditionalConnectorNumberInColumn_yDirection - 1) * FAdditionalScrewsDistance_y), IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection, FAdditionalScrewsDistance_x, FAdditionalScrewsDistance_y);
                float fDistanceBetweenLeftAndRightReferencePoint = 2 * FRadius_SQ1 - (IAdditionalConnectorNumberInRow_xDirection - 1) * FAdditionalScrewsDistance_x;
                Point[] cornerConnectorsInGroupTopRight = GetAdditionaConnectorsCoordinatesInOneSequence(new Point(-FRadius_SQ1 + fDistanceBetweenLeftAndRightReferencePoint, FRadius_SQ1 - (IAdditionalConnectorNumberInColumn_yDirection - 1) * FAdditionalScrewsDistance_y), IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection, FAdditionalScrewsDistance_x, FAdditionalScrewsDistance_y);

                // Bottom part of group
                Point[] cornerConnectorsInGroupBottomLeft = new Point[cornerConnectorsInGroupTopLeft.Length];
                Point[] cornerConnectorsInGroupBottomRight = new Point[cornerConnectorsInGroupTopRight.Length];

                // Copy items
                cornerConnectorsInGroupTopLeft.CopyTo(cornerConnectorsInGroupBottomLeft, 0);
                cornerConnectorsInGroupTopRight.CopyTo(cornerConnectorsInGroupBottomRight, 0);

                // Rotate bottom part
                Geom2D.TransformPositions_CCW_deg(0, 0, 180, ref cornerConnectorsInGroupBottomLeft);
                Geom2D.TransformPositions_CCW_deg(0, 0, 180, ref cornerConnectorsInGroupBottomRight);

                // Set group parameters
                group.ListScrewSequence[group.NumberOfHalfCircleSequences + 0].HolesCentersPoints = cornerConnectorsInGroupTopLeft;
                group.ListScrewSequence[group.NumberOfHalfCircleSequences + 1].HolesCentersPoints = cornerConnectorsInGroupTopRight;
                group.ListScrewSequence[group.NumberOfHalfCircleSequences + 2].HolesCentersPoints = cornerConnectorsInGroupBottomLeft;
                group.ListScrewSequence[group.NumberOfHalfCircleSequences + 3].HolesCentersPoints = cornerConnectorsInGroupBottomRight;
            }

            // Set radii of connectors / screws in the connection
            group.Get_RadiiOfScrewsInGroup();

            group.TransformGroup(new Point(fx_c, fy_c), fAngle_seq_rotation_deg);
        }

        public Point[] GetAdditionaConnectorsCoordinatesInOneSequence(Point refPoint,
            int iNumberOfPointsInXDirection,
            int iNumberOfPointsInYDirection,
            float fDistanceOfPointsX,
            float fDistanceOfPointsY)
        {
            Point[] arrayPoints = GetRegularArrayOfPointsInCartesianCoordinates(refPoint, iNumberOfPointsInXDirection, iNumberOfPointsInYDirection, fDistanceOfPointsX, fDistanceOfPointsY);
            return arrayPoints;
        }

        public void Calc_HolesCentersCoord2DApexPlate(
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        {
            float fDistanceOfCenterFromLeftEdge = fbX / 4f;
            float fx_c1 = fDistanceOfCenterFromLeftEdge;
            float fy_c1 = flZ + ((fhY_1 / 2f) / (float)Math.Cos(fSlope_rad)) + (fDistanceOfCenterFromLeftEdge * (float)Math.Tan(fSlope_rad));

            float fx_c2 = fbX - fDistanceOfCenterFromLeftEdge; // Symmetrical
            float fy_c2 = fy_c1;

            float fAdditionalMargin = 0.01f; // Temp - TODO - put to the input data
            float fRadius = 0.5f * FCrscWebStraightDepth - 2 * fAdditionalMargin; // m // Input - depending on depth of cross-section
            float fAngle_seq_rotation_init_point_deg = (float)(Math.Atan(0.5f * FStiffenerSize / fDistanceOfCenterFromLeftEdge) / MathF.fPI * 180f); // Input - constant for cross-section according to the size of middle sfiffener

            // Left side
            CScrewSequenceGroup group1 = ListOfSequenceGroups[0]; // Indexovana polozka sa neda predat referenciou
            Get_ScrewGroup_IncludingAdditionalScrews(fx_c1, fy_c1, fAngle_seq_rotation_init_point_deg, fSlope_rad, ref group1);
            ListOfSequenceGroups[0] = group1;

            // Right side
            CScrewSequenceGroup group2 = GetMirroredScrewGroupAboutY(group1);
            // TODO - skontrolovat - Toto je asi zbytocne ak bola skupina na predchadzajucom riadku odzkradlena staci ju pridat do zoznamu
            Get_ScrewGroup_IncludingAdditionalScrews(fx_c2, fy_c2, fAngle_seq_rotation_init_point_deg, -fSlope_rad, ref group2);

            // TODO - toto by malo byt osetrene vopred, pozname pocet skupin
            if (ListOfSequenceGroups.Count == 1)
                ListOfSequenceGroups.Add(group2);
            else
                ListOfSequenceGroups[1] = group2;

            // Fill array of holes centers
            FillArrayOfHolesCentersInWholeArrangement();
        }

        public void Calc_HolesCentersCoord2DKneePlate(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        {
            // Bottom Circle (Main Column)
            float fDistanceOfCenterFromLeftEdge = flZ + fbX_1 / 2f;
            float fx_c1 = fDistanceOfCenterFromLeftEdge;
            float fy_c1 = fhY_1 / 4f;

            // Top Circle (Main Rafter)
            float fxInTopMemberAxis = 0.2f * (fbX_2 - fbX_1); // TODO - hodnota je v smere lokalnej osi x prievkalu, je urcena priblizne z vodorovnych rozmerov plechu, do buducna bo bolo dobre pohrat sa s jej urcenim na zaklade sklonu prievkalu a dalsich rozmerov, tak aby spoj nekolidoval s eave purlin a skrutky nevysli mimo plech

            float fx_c2 = fxInTopMemberAxis * (float)Math.Cos(fSlope_rad) + fDistanceOfCenterFromLeftEdge;
            float fy_c2 = fxInTopMemberAxis * (float)Math.Sin(fSlope_rad) + ((fhY_1 + fx_c1 * (float)Math.Atan(fSlope_rad)) - (0.5f * FCrscRafterDepth / (float)Math.Cos(fSlope_rad))); // TODO Dopracovat podla sklonu rafteru

            float fAdditionalMargin = 0.01f; // Temp - TODO - put to the input data
            float fRadius = 0.5f * FCrscWebStraightDepth - 2 * fAdditionalMargin; // m // Input - depending on depth of cross-section
            float fAngle_seq_rotation_init_point_deg = (float)(Math.Atan(0.5f * FStiffenerSize / fDistanceOfCenterFromLeftEdge) / MathF.fPI * 180f); // Input - constant for cross-section according to the size of middle sfiffener

            // Bottom side
            CScrewSequenceGroup group1 = ListOfSequenceGroups[0]; // Indexovana polozka sa neda predat referenciou
            Get_ScrewGroup_IncludingAdditionalScrews(fx_c1, fy_c1, fAngle_seq_rotation_init_point_deg, MathF.fPI / 2f, ref group1);
            ListOfSequenceGroups[0] = group1;

            // Top side
            CScrewSequenceGroup group2 = ListOfSequenceGroups[0]; // Indexovana polozka sa neda predat referenciou
            Get_ScrewGroup_IncludingAdditionalScrews(fx_c2, fy_c2, fAngle_seq_rotation_init_point_deg, fSlope_rad, ref group1);

            // TODO - toto by malo byt osetrene vopred, pozname pocet skupin
            if (ListOfSequenceGroups.Count == 1)
                ListOfSequenceGroups.Add(group2);
            else
                ListOfSequenceGroups[1] = group2;

            // Fill array of holes centers
            FillArrayOfHolesCentersInWholeArrangement();
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

        public CScrewSequenceGroup GetMirroredScrewGroupAboutY(CScrewSequenceGroup group)
        {
            CScrewSequenceGroup groupOut = new CScrewSequenceGroup();

            groupOut = group;

            foreach (CScrewSequence seq in groupOut.ListScrewSequence)
            {
                for (int i = 0; i < seq.INumberOfScrews; i++)
                {
                    seq.HolesCentersPoints[i].X *= -1; // Change X coordinate (mirror about Y)
                }
            }

            return groupOut;
        }
    }
}