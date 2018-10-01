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
        private float m_fCrscRafterDepth;
        private float m_fCrscWebStraightDepth;
        private float m_fStiffenerSize; // Middle cross-section stiffener dimension (without screws)
        private int m_iNumberOfGroupsInJoint = 2; // Pocet kruhov na jednom plechu
        private int m_iNumberOfCirclesInGroup = 1; // pocet kruhov v jednej skupine (group)
        private int m_iNumberOfCircleSequencesInGroup = 2; // pocet polkruhov v "kruhu" na jednom plechu (sekvencia - sequence)
        private float[] m_HolesCenterRadii; // Array of screw radii in one group related to the screw arrangement centroid
        private int iNumberOfCircleScrewsSequencesInOneGroup = 2;

        // Corner screws
        private bool m_bUseAdditionalCornerScrews; // Pocet skrutiek v rohoch - celkovo 4 skrutky * 4 rohy * 2 kruhy
        private int m_iAdditionalConnectorNumberInRow_xDirection;
        private int m_iAdditionalConnectorNumberInColumn_yDirection;
        private int m_iAdditionalConnectorNumber;
        private int m_iAdditionalConnectorInCornerNumber;
        private float m_fAdditionalScrewsDistance_x;
        private float m_fAdditionalScrewsDistance_y;

        #region properties

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

        public int INumberOfCircleScrewsSequencesInOneGroup
        {
            get
            {
                return iNumberOfCircleScrewsSequencesInOneGroup;
            }

            set
            {
                iNumberOfCircleScrewsSequencesInOneGroup = value;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public CScrewArrangementCircleApexOrKnee()
        { }

        public CScrewArrangementCircleApexOrKnee(
            CScrew referenceScrew_temp,
            float fCrscRafterDepth,
            float fCrscWebStraightDepth,
            float fStiffenerSize,
            int iNumberOfCirclesInGroup,
            List<CScrewSequenceGroup> screwSequenceGroup,
            bool bUseAdditionalCornerScrews,
            int iAdditionalConnectorInCornerNumber,
            float fAdditionalScrewsDistance_x,
            float fAdditionalScrewsDistance_y)
        {
            referenceScrew = referenceScrew_temp;
            FCrscRafterDepth = fCrscRafterDepth;
            FCrscWebStraightDepth = fCrscWebStraightDepth;
            FStiffenerSize = fStiffenerSize;

            // Circle
            INumberOfCirclesInGroup = iNumberOfCirclesInGroup; // pocet kruhov

            // Corner screws parameters
            BUseAdditionalCornerScrews = bUseAdditionalCornerScrews;
            IAdditionalConnectorInCornerNumber = iAdditionalConnectorInCornerNumber; // Spolu v jednom rohu
            FAdditionalScrewsDistance_x = fAdditionalScrewsDistance_x;
            FAdditionalScrewsDistance_y = fAdditionalScrewsDistance_y;
            
            ListOfSequenceGroups = screwSequenceGroup;

            UpdateAdditionalCornerScrews();
        }

        public void NumberOfCirclesInGroup_Updated(int newNumberOfCirclesInGroup)
        {
            if (newNumberOfCirclesInGroup < 0) return;

            if (newNumberOfCirclesInGroup == 0)
            {
                foreach (CScrewSequenceGroup g in ListOfSequenceGroups)
                {
                    g.ListScrewSequence.RemoveAll(s => s is CScrewHalfCircleSequence);
                }
            }
            else if (newNumberOfCirclesInGroup < INumberOfCirclesInGroup)
            {
                CScrewSequenceGroup gr = ListOfSequenceGroups.FirstOrDefault();
                if (gr == null) return;
                //int actualNumOfCircles = gr.ListScrewSequence.Where(s => s is CScrewHalfCircleSequence).Count() / 2;

                foreach (CScrewSequenceGroup g in ListOfSequenceGroups)
                {
                    for (var i = INumberOfCirclesInGroup; i > newNumberOfCirclesInGroup; i--)
                    {
                        g.ListScrewSequence.Remove(g.ListScrewSequence.LastOrDefault(s => s is CScrewHalfCircleSequence)); //last circle 1.half
                        g.ListScrewSequence.Remove(g.ListScrewSequence.LastOrDefault(s => s is CScrewHalfCircleSequence)); //last circle 1.half
                    }
                }
            }
            else if (newNumberOfCirclesInGroup > INumberOfCirclesInGroup)
            {
                int numToAdd = newNumberOfCirclesInGroup - INumberOfCirclesInGroup;
                for (int i = 0; i < numToAdd; i++)
                {
                    foreach (CScrewSequenceGroup g in ListOfSequenceGroups)
                    {
                        // Add one circle - two half-circle sequences
                        int lastHalfCircleNumberOfScrews = 16;
                        float lastHalfCircleRadius = 0.25f;

                        CScrewSequence lastCircleScrewSequence = g.ListScrewSequence.LastOrDefault(p => p is CScrewHalfCircleSequence); 
                        if (lastCircleScrewSequence != null)
                        {
                            lastHalfCircleNumberOfScrews = lastCircleScrewSequence.INumberOfScrews;
                            lastHalfCircleRadius = ((CScrewHalfCircleSequence)lastCircleScrewSequence).Radius;
                        }

                        // Add first half-circle sequence
                        CScrewHalfCircleSequence screwHalfCircleSequence1 = new CScrewHalfCircleSequence();
                        screwHalfCircleSequence1.INumberOfScrews = lastHalfCircleNumberOfScrews - 2; // Kazdy novy kruh o 2 skrutky menej
                        screwHalfCircleSequence1.Radius = lastHalfCircleRadius - 0.05f; // Kazdy novy kruh o 50 mm mensi priemer
                        g.ListScrewSequence.Add(screwHalfCircleSequence1);
                        g.NumberOfHalfCircleSequences += 1; // Add 1 sequence

                        // Add second half-circle sequence
                        CScrewHalfCircleSequence screwHalfCircleSequence2 = new CScrewHalfCircleSequence();
                        screwHalfCircleSequence2.INumberOfScrews = lastHalfCircleNumberOfScrews - 2; // Kazdy novy kruh o 2 skrutky menej
                        screwHalfCircleSequence2.Radius = lastHalfCircleRadius - 0.05f; // Kazdy novy kruh o 50 mm mensi priemer
                        g.ListScrewSequence.Add(screwHalfCircleSequence2);
                        g.NumberOfHalfCircleSequences += 1; // Add 1 sequence
                    }
                }
            }

            INumberOfCirclesInGroup = newNumberOfCirclesInGroup;
        }

        public void UpdateAdditionalCornerScrews()
        {
            IAdditionalConnectorNumberInRow_xDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber); // v smere x, pocet v riadku
            IAdditionalConnectorNumberInColumn_yDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber); // v smere y, pocet v stlpci

            //---------------------------------------------------------------------------------------------------------------------------------
            if (BUseAdditionalCornerScrews) // 4 corners in one group
            {
                foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
                {
                    bool bAddNewSequences;

                    if (group.NumberOfRectangularSequences == 0) // if number of rectangular sequences is less than 4 set four (each corner)
                        bAddNewSequences = true;
                    else if (group.NumberOfRectangularSequences == 4)
                        bAddNewSequences = false;
                    else
                    {
                        if (group.NumberOfRectangularSequences < 4)
                        {
                            // Exception - not all rectangular sequences in corner were deleted!
                            throw new Exception("Not all rectangular sequences in corner were deleted!");
                        }
                        else
                        {
                            // Exception - more than 4 corner sequences
                            throw new Exception("More than 4 corner sequences were defined!");
                        }
                    }

                    // Set number of rectangular sequences
                    group.NumberOfRectangularSequences = 4;

                    for (int i = 0; i < group.NumberOfRectangularSequences; i++)
                    {
                        CScrewRectSequence seq_Corner = new CScrewRectSequence(IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection);
                        seq_Corner.DistanceOfPointsX = FAdditionalScrewsDistance_x;
                        seq_Corner.DistanceOfPointsY = FAdditionalScrewsDistance_y;

                        if (bAddNewSequences)
                            group.ListScrewSequence.Add(seq_Corner); // Add new sequence
                        else
                        {
                            CScrewSequence seq = group.ListScrewSequence.Where(s => s is CScrewRectSequence).ElementAtOrDefault(i);
                            if (seq == null) group.ListScrewSequence.Add(seq_Corner);
                            else seq = seq_Corner;
                        }
                    }
                }
            }
            else // Corner screws are deactivated (remove all sequences - type rectangluar from group
            {
                // Remove all rectangular sequences
                foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
                {
                    group.ListScrewSequence.RemoveAll(s => s is CScrewRectSequence);
                    // Set current number of rectangular sequences (it should be "0" in case that corner screw sequences are not used, all other sequences are half-circle)
                    group.NumberOfRectangularSequences = 0;
                }
            }

            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            RecalculateTotalNumberOfScrews();

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public override void UpdateArrangmentData()
        {
            // Tu treba updatovat vsetko
            // To Ondrej - velmi uz ani nie je co

            // Mohli by sa tu presunut casti funkcii:
            // NumberOfCirclesInGroup_Updated
            // Calc_HolesCentersCoord2DApexPlate
            // Calc_HolesCentersCoord2DKneePlate

            // ale Calc_xxxx sa potom volaju pri plate update - public override void UpdatePlateData(CScrewArrangement screwArrangement)
            // pretoze poloha screw group sa pocita z rozmerov plechu

            // Circles



            // Additional Corner Screws
            UpdateAdditionalCornerScrews();
        }

        public void Get_ScrewGroup_IncludingAdditionalScrews(float fx_c,
        float fy_c,
        float fRotation_rad,
        ref CScrewSequenceGroup group)
        {
            float fAngle_seq_rotation_deg = fRotation_rad * 180f / MathF.fPI; // Input value (roof pitch)
            float fAdditionalMargin = 0.01f; //naco je toto dobre???
            // To Ondrej Uprostred prierezu 63020 (vid crsc 3D view je "vyztuha", teda je tam medzera, do ktorej sa neda pripevnit plech skrutkami (preto je kruh skrutiek rozdeleny na 2 segmenty)
            // hodnota additional margin je pouzita na to aby nebola krajna skrutka v kruhovej sekvencii priamo na zakrivenej hrane ale dalej, nastavene je tvrdo 10 mm ale moze to byt aj viac a nastavitelne uzivatelom)
            // dalo by sa to pripocitavat priamo k FStiffenerSize
            // Zatial som to nerobil user-defined, lebo to nie je az take dolezite, nastavovat 10 mm alebo 30 mm, uvidim ci to budu chciet naozaj nastavovat

            if (group.NumberOfHalfCircleSequences > 0)
            {
                int count = 0;
                foreach (CScrewSequence screwSequence in group.ListScrewSequence)
                {
                    if (!(screwSequence is CScrewHalfCircleSequence)) continue;
                    CScrewHalfCircleSequence circSeq = screwSequence as CScrewHalfCircleSequence;

                    // Input - according to the size of middle sfiffener and circle radius
                    float fAngle_seq_rotation_init_point_deg = (float)(Math.Atan((0.5f * FStiffenerSize + 2 * fAdditionalMargin) / circSeq.Radius) / MathF.fPI * 180f);
                    // Angle between sequence center, first and last point in the sequence
                    float fAngle_interval_deg = 180 - (2f * fAngle_seq_rotation_init_point_deg); 
                    if (count % 2 == 0)
                    {
                        // Half circle sequence
                        List<Point> fSequenceTop = Geom2D.GetArcPointCoord_CCW_deg(circSeq.Radius, fAngle_seq_rotation_init_point_deg, fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, circSeq.INumberOfScrews, false);
                        screwSequence.HolesCentersPoints = fSequenceTop.ToArray();
                    }
                    else
                    {
                        List<Point> fSequenceBottom = Geom2D.GetArcPointCoord_CCW_deg(circSeq.Radius, 180 + fAngle_seq_rotation_init_point_deg, 180 + fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, circSeq.INumberOfScrews, false);
                        screwSequence.HolesCentersPoints = fSequenceBottom.ToArray();
                    }
                    count++;
                }
            }

            // Add additional point the sequences
            if (BUseAdditionalCornerScrews && group.NumberOfRectangularSequences > 0)
            {
                CScrewSequence seq = group.ListScrewSequence.FirstOrDefault(s => s is CScrewHalfCircleSequence);
                float FRadius_SQ1 = 0;
                if (seq != null) FRadius_SQ1 = ((CScrewHalfCircleSequence)seq).Radius;

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
                IEnumerable<CScrewSequence> rectSequences = group.ListScrewSequence.Where(s => s is CScrewRectSequence);
                if (rectSequences.ElementAtOrDefault(0) != null) rectSequences.ElementAtOrDefault(0).HolesCentersPoints = cornerConnectorsInGroupTopLeft;
                if (rectSequences.ElementAtOrDefault(1) != null) rectSequences.ElementAtOrDefault(1).HolesCentersPoints = cornerConnectorsInGroupTopRight;
                if (rectSequences.ElementAtOrDefault(2) != null) rectSequences.ElementAtOrDefault(2).HolesCentersPoints = cornerConnectorsInGroupBottomLeft;
                if (rectSequences.ElementAtOrDefault(3) != null) rectSequences.ElementAtOrDefault(3).HolesCentersPoints = cornerConnectorsInGroupBottomRight;
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

        public override void Calc_HolesCentersCoord2DApexPlate(
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

            if (ListOfSequenceGroups != null && ListOfSequenceGroups.Count == 2)
            {
                // Left side
                CScrewSequenceGroup group1 = ListOfSequenceGroups[0]; // Indexovana polozka sa neda predat referenciou
                Get_ScrewGroup_IncludingAdditionalScrews(fx_c1, fy_c1, fSlope_rad, ref group1);
                ListOfSequenceGroups[0] = group1;

                // Right side
                CScrewSequenceGroup group2 = ListOfSequenceGroups[1]; // GetMirroredScrewGroupAboutY(group1);
                Get_ScrewGroup_IncludingAdditionalScrews(fx_c2, fy_c2, -fSlope_rad, ref group2);
                ListOfSequenceGroups[1] = group2;
            }
            
            // Fill array of holes centers
            FillArrayOfHolesCentersInWholeArrangement();
        }

        public override void Calc_HolesCentersCoord2DKneePlate(
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

            if (ListOfSequenceGroups != null && ListOfSequenceGroups.Count == 2)
            {
                // Bottom side
                CScrewSequenceGroup group1 = ListOfSequenceGroups[0]; // Indexovana polozka sa neda predat referenciou
                Get_ScrewGroup_IncludingAdditionalScrews(fx_c1, fy_c1, MathF.fPI / 2f, ref group1); // Rotate - 90 deg
                ListOfSequenceGroups[0] = group1;

                // Top side
                CScrewSequenceGroup group2 = ListOfSequenceGroups[1]; // Indexovana polozka sa neda predat referenciou
                Get_ScrewGroup_IncludingAdditionalScrews(fx_c2, fy_c2, fSlope_rad, ref group2); // Rotate - Roof Slope
                ListOfSequenceGroups[1] = group2;
            }

            // Fill array of holes centers
            FillArrayOfHolesCentersInWholeArrangement();
        }

        public override void Calc_ApexPlateData(
            float fbX,
            float flZ,
            float fhY_1,
            float ft,
            float fSlope_rad)
        {
            Calc_HolesCentersCoord2DApexPlate(fbX, flZ, fhY_1, fSlope_rad);
            Calc_HolesControlPointsCoord3D_FlatPlate(0, flZ, ft);
            GenerateConnectors_FlatPlate();
        }

        public override void Calc_KneePlateData(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float ft,
            float fSlope_rad)
        {
            Calc_HolesCentersCoord2DKneePlate(fbX_1, fbX_2, flZ, fhY_1, fSlope_rad);
            Calc_HolesControlPointsCoord3D_FlatPlate(flZ, 0, ft);
            GenerateConnectors_FlatPlate();
        }

        public CScrewSequenceGroup GetMirroredScrewGroupAboutY(CScrewSequenceGroup group)
        {
            CScrewSequenceGroup groupOut = new CScrewSequenceGroup();

            for(int i = 0; i < group.ListScrewSequence.Count; i++)
            {
                CScrewSequence seqTemp = new CScrewSequence();
                seqTemp.HolesCentersPoints = new Point[group.ListScrewSequence[i].HolesCentersPoints.Length];

                for (int j = 0; j < group.ListScrewSequence[i].HolesCentersPoints.Length; j++)
                {
                    seqTemp.HolesCentersPoints[j].X = group.ListScrewSequence[i].HolesCentersPoints[j].X * -1; // Change X coordinate (mirror about Y)
                    seqTemp.HolesCentersPoints[j].Y = group.ListScrewSequence[i].HolesCentersPoints[j].Y;
                }

                groupOut.ListScrewSequence.Add(seqTemp);
            }

            return groupOut;
        }
    }
}