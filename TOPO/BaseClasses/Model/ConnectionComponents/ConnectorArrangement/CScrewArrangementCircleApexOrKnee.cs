﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Globalization;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangementCircleApexOrKnee : CScrewArrangement
    {
        private float m_fCrscRafterDepth;
        private float m_fCrscWebStraightDepth;
        private float m_fStiffenerSize; // Middle cross-section stiffener dimension (without screws)
        private int m_iNumberOfGroupsInJoint = 2; // Pocet kruhov na jednom plechu
        private int m_iNumberOfCirclesInGroup = 1; // pocet kruhov v jednej skupine (group)
        private int m_iNumberOfCircleSequencesInGroup = 2; // pocet polkruhov v "kruhu" na jednom plechu (sekvencia - sequence)
        private int iNumberOfCircleScrewsSequencesInOneGroup = 2;

        // Corner screws
        private bool m_bUseAdditionalCornerScrews; // Pocet skrutiek v rohoch - celkovo 4 skrutky * 4 rohy * 2 kruhy
        private float m_fPositionOfCornerSequence_x;
        private float m_fPositionOfCornerSequence_y;
        private int m_iAdditionalConnectorNumberInRow_xDirection;
        private int m_iAdditionalConnectorNumberInColumn_yDirection;
        private int m_iAdditionalConnectorNumber;
        private int m_iAdditionalConnectorInCornerNumber;
        private float m_fAdditionalCornerScrewsDistance_x;
        private float m_fAdditionalCornerScrewsDistance_y;

        //Extra screws
        private bool bUseExtraScrews;
        private int iExtraNumberOfRows;
        private int iExtraNumberOfScrewsInRow;
        private float m_fPositionOfExtraScrewsSequence_y;
        private float m_fExtraScrewsDistance_x;
        private float m_fExtraScrewsDistance_y;

        #region properties

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
        public float FPositionOfCornerSequence_x
        {
            get
            {
                return m_fPositionOfCornerSequence_x;
            }

            set
            {
                m_fPositionOfCornerSequence_x = value;
            }
        }
        public float FPositionOfCornerSequence_y
        {
            get
            {
                return m_fPositionOfCornerSequence_y;
            }

            set
            {
                m_fPositionOfCornerSequence_y = value;
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
        public float FAdditionalCornerScrewsDistance_x
        {
            get
            {
                return m_fAdditionalCornerScrewsDistance_x;
            }

            set
            {
                m_fAdditionalCornerScrewsDistance_x = value;
            }
        }
        public float FAdditionalCornerScrewsDistance_y
        {
            get
            {
                return m_fAdditionalCornerScrewsDistance_y;
            }

            set
            {
                m_fAdditionalCornerScrewsDistance_y = value;
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

        public bool UseExtraScrews
        {
            get
            {
                return bUseExtraScrews;
            }

            set
            {
                bUseExtraScrews = value;
            }
        }

        public int ExtraNumberOfRows
        {
            get
            {
                return iExtraNumberOfRows;
            }

            set
            {
                iExtraNumberOfRows = value;
            }
        }

        public int ExtraNumberOfScrewsInRow
        {
            get
            {
                return iExtraNumberOfScrewsInRow;
            }

            set
            {
                iExtraNumberOfScrewsInRow = value;
            }
        }

        public float PositionOfExtraScrewsSequence_y
        {
            get
            {
                return m_fPositionOfExtraScrewsSequence_y;
            }

            set
            {
                m_fPositionOfExtraScrewsSequence_y = value;
            }
        }

        public float ExtraScrewsDistance_x
        {
            get
            {
                return m_fExtraScrewsDistance_x;
            }

            set
            {
                m_fExtraScrewsDistance_x = value;
            }
        }

        public float ExtraScrewsDistance_y
        {
            get
            {
                return m_fExtraScrewsDistance_y;
            }

            set
            {
                m_fExtraScrewsDistance_y = value;
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
            float fPositionOfCornerSequence_x,
            float fPositionOfCornerSequence_y,
            int iAdditionalConnectorInCornerNumber,
            float fAdditionalScrewsDistance_x,
            float fAdditionalScrewsDistance_y,
            bool bUseExtraScrews,
            int iExtraNumberOfRows,
            int iExtraNumberOfScrewsInRow,
            float positionOfExtraScrewsSequence_y,
            float extraScrewsDistance_x,
            float extraScrewsDistance_y
            )
        {
            referenceScrew = referenceScrew_temp;
            FCrscRafterDepth = fCrscRafterDepth;
            FCrscWebStraightDepth = fCrscWebStraightDepth;
            FStiffenerSize = fStiffenerSize;

            // Circle
            INumberOfCirclesInGroup = iNumberOfCirclesInGroup; // pocet kruhov

            // Corner screws parameters
            BUseAdditionalCornerScrews = bUseAdditionalCornerScrews;
            FPositionOfCornerSequence_x = fPositionOfCornerSequence_x;
            FPositionOfCornerSequence_y = fPositionOfCornerSequence_y;
            IAdditionalConnectorInCornerNumber = iAdditionalConnectorInCornerNumber; // Spolu v jednom rohu
            FAdditionalCornerScrewsDistance_x = fAdditionalScrewsDistance_x;
            FAdditionalCornerScrewsDistance_y = fAdditionalScrewsDistance_y;

            //Extra screws
            UseExtraScrews = bUseExtraScrews;
            ExtraNumberOfRows = iExtraNumberOfRows;
            ExtraNumberOfScrewsInRow = iExtraNumberOfScrewsInRow;
            PositionOfExtraScrewsSequence_y = positionOfExtraScrewsSequence_y;
            ExtraScrewsDistance_x = extraScrewsDistance_x;
            ExtraScrewsDistance_y = extraScrewsDistance_y;

            ListOfSequenceGroups = screwSequenceGroup;

            //UpdateAdditionalCornerScrews();
            UpdateAdditionalScrews();
        }

        public void NumberOfCirclesInGroup_Updated(int newNumberOfCirclesInGroup)
        {
            if (newNumberOfCirclesInGroup < 0) return;
            if (newNumberOfCirclesInGroup > 5) return;

            if (newNumberOfCirclesInGroup == 0)
            {
                foreach (CScrewSequenceGroup g in ListOfSequenceGroups)
                {
                    g.ListSequence.RemoveAll(s => s is CScrewHalfCircleSequence);
                }
            }
            else if (newNumberOfCirclesInGroup < INumberOfCirclesInGroup)
            {
                CScrewSequenceGroup gr = ListOfSequenceGroups.FirstOrDefault();
                if (gr == null) return;
                //int actualNumOfCircles = gr.ListSequence.Where(s => s is CScrewHalfCircleSequence).Count() / 2;

                foreach (CScrewSequenceGroup g in ListOfSequenceGroups)
                {
                    for (var i = INumberOfCirclesInGroup; i > newNumberOfCirclesInGroup; i--)
                    {
                        g.ListSequence.Remove(g.ListSequence.LastOrDefault(s => s is CScrewHalfCircleSequence)); //last circle 1.half
                        g.ListSequence.Remove(g.ListSequence.LastOrDefault(s => s is CScrewHalfCircleSequence)); //last circle 1.half
                    }
                }
            }
            else if (newNumberOfCirclesInGroup > INumberOfCirclesInGroup)
            {
                const float fDistancePerHalfCircle = 0.04f; // Auxialiary value of distance between screws in arc

                int numToAdd = newNumberOfCirclesInGroup - INumberOfCirclesInGroup;
                for (int i = 0; i < numToAdd; i++)
                {
                    foreach (CScrewSequenceGroup g in ListOfSequenceGroups)
                    {
                        // Add one circle - two half-circle sequences
                        float lastHalfCircleRadius = 0.25f;
                        float lastHalfCircleLength = (0.5f * 2 * MathF.fPI * lastHalfCircleRadius);
                        int lastHalfCircleNumberOfScrews = (int)(lastHalfCircleLength / fDistancePerHalfCircle) + 1;

                        CScrewSequence lastCircleScrewSequence = (CScrewSequence)g.ListSequence.LastOrDefault(p => p is CScrewHalfCircleSequence);
                        if (lastCircleScrewSequence != null)
                        {
                            lastHalfCircleNumberOfScrews = lastCircleScrewSequence.INumberOfConnectors;
                            lastHalfCircleRadius = ((CScrewHalfCircleSequence)lastCircleScrewSequence).Radius;
                        }

                        // Add first half-circle sequence
                        CScrewHalfCircleSequence screwHalfCircleSequence1 = new CScrewHalfCircleSequence();
                        screwHalfCircleSequence1.Radius = lastHalfCircleRadius - 0.03f; // Kazdy novy kruh o 30 mm mensi polomer
                        screwHalfCircleSequence1.INumberOfConnectors = (int)((0.5f * 2 * MathF.fPI * screwHalfCircleSequence1.Radius) / fDistancePerHalfCircle) + 1; // lastHalfCircleNumberOfScrews - 2; // Kazdy novy kruh o 2 skrutky menej

                        g.ListSequence.Add(screwHalfCircleSequence1);
                        g.NumberOfHalfCircleSequences += 1; // Add 1 sequence

                        // Add second half-circle sequence
                        CScrewHalfCircleSequence screwHalfCircleSequence2 = new CScrewHalfCircleSequence();
                        screwHalfCircleSequence2.Radius = lastHalfCircleRadius - 0.03f; // Kazdy novy kruh o 30 mm mensi polomer
                        screwHalfCircleSequence2.INumberOfConnectors = (int)((0.5f * 2 * MathF.fPI * screwHalfCircleSequence2.Radius) / fDistancePerHalfCircle) + 1; // lastHalfCircleNumberOfScrews - 2; // Kazdy novy kruh o 2 skrutky menej

                        g.ListSequence.Add(screwHalfCircleSequence2);
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
                    //bool bAddNewSequences;

                    //if (group.NumberOfRectangularSequences == 0) // if number of rectangular sequences is less than 4 set four (each corner)
                    //    bAddNewSequences = true;
                    //else if (group.NumberOfRectangularSequences == 4)
                    //    bAddNewSequences = false;
                    //else
                    //{
                    //    if (group.NumberOfRectangularSequences < 4)
                    //    {
                    //        // Exception - not all rectangular sequences in corner were deleted!
                    //        throw new Exception("Not all rectangular sequences in corner were deleted!");
                    //    }
                    //    else
                    //    {
                    //        // Exception - more than 4 corner sequences
                    //        throw new Exception("More than 4 corner sequences were defined!");
                    //    }
                    //}

                    // Set number of rectangular sequences
                    group.NumberOfRectangularSequences = 4;

                    for (int i = 0; i < group.NumberOfRectangularSequences; i++)
                    {
                        CScrewRectSequence seq_Corner = new CScrewRectSequence(IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection);
                        seq_Corner.DistanceOfPointsX = FAdditionalCornerScrewsDistance_x;
                        seq_Corner.DistanceOfPointsY = FAdditionalCornerScrewsDistance_y;

                        //if (bAddNewSequences)
                        //    group.ListSequence.Add(seq_Corner); // Add new sequence
                        //else
                        //{
                        CScrewSequence seq = (CScrewSequence)group.ListSequence.Where(s => s is CScrewRectSequence).ElementAtOrDefault(i);
                        if (seq == null) group.ListSequence.Add(seq_Corner);
                        else
                        {
                            int index = group.ListSequence.IndexOf(seq);
                            if (index != -1) group.ListSequence[index] = seq_Corner;
                        }
                        //}
                    }
                }
            }
            else // Corner screws are deactivated (remove all sequences - type rectangluar from group
            {
                // Remove all rectangular sequences
                foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
                {
                    group.ListSequence.RemoveAll(s => s is CScrewRectSequence);
                    // Set current number of rectangular sequences (it should be "0" in case that corner screw sequences are not used, all other sequences are half-circle)
                    group.NumberOfRectangularSequences = 0;
                }
            }

            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            RecalculateTotalNumberOfScrews();

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public void UpdateAdditionalScrews()
        {
            IAdditionalConnectorNumberInRow_xDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber); // v smere x, pocet v riadku
            IAdditionalConnectorNumberInColumn_yDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber); // v smere y, pocet v stlpci

            //---------------------------------------------------------------------------------------------------------------------------------
            if (BUseAdditionalCornerScrews || UseExtraScrews) // 4 corners in one group, 2 extra lines
            {
                foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
                {                    
                    // Set number of rectangular sequences
                    if (BUseAdditionalCornerScrews && UseExtraScrews) group.NumberOfRectangularSequences = 6;
                    else if (BUseAdditionalCornerScrews) group.NumberOfRectangularSequences = 4;
                    else if (UseExtraScrews) group.NumberOfRectangularSequences = 2;

                    if (BUseAdditionalCornerScrews)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            CScrewRectSequence seq_Corner = new CScrewRectSequence(IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection);
                            seq_Corner.DistanceOfPointsX = FAdditionalCornerScrewsDistance_x;
                            seq_Corner.DistanceOfPointsY = FAdditionalCornerScrewsDistance_y;

                            CScrewSequence seq = (CScrewSequence)group.ListSequence.Where(s => s is CScrewRectSequence).ElementAtOrDefault(i);
                            if (seq == null) group.ListSequence.Add(seq_Corner);
                            else
                            {
                                int index = group.ListSequence.IndexOf(seq);
                                if (index != -1) group.ListSequence[index] = seq_Corner;
                            }
                        }
                    }
                    if (UseExtraScrews)
                    {
                        int startIndex = BUseAdditionalCornerScrews ? 4 : 0;

                        for (int i = startIndex; i < startIndex + 2; i++)
                        {
                            CScrewRectSequence seq_Extra = new CScrewRectSequence(ExtraNumberOfScrewsInRow, ExtraNumberOfRows);
                            seq_Extra.DistanceOfPointsX = FAdditionalCornerScrewsDistance_x;
                            seq_Extra.DistanceOfPointsY = FAdditionalCornerScrewsDistance_y;

                            CScrewSequence seq = (CScrewSequence)group.ListSequence.Where(s => s is CScrewRectSequence).ElementAtOrDefault(i);
                            if (seq == null) group.ListSequence.Add(seq_Extra);
                            else
                            {
                                int index = group.ListSequence.IndexOf(seq);
                                if (index != -1) group.ListSequence[index] = seq_Extra;
                            }
                        }
                    }
                }
            }

            if (!BUseAdditionalCornerScrews) // Corner screws are deactivated (remove all sequences - type rectangluar from group
            {
                // Remove all rectangular sequences
                foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
                {
                    if (UseExtraScrews)
                    {
                        if (group.ListSequence.Where(s=> s is CScrewRectSequence).Count() == 6)
                        {                            
                            List<CConnectorSequence> sequencesToRemove = group.ListSequence.Where(s => s is CScrewRectSequence).ToList().GetRange(0, 4);
                            group.ListSequence.RemoveAll(s=> sequencesToRemove.Contains(s));                            
                            group.NumberOfRectangularSequences = 2;
                        }
                    }
                    else
                    {
                        group.ListSequence.RemoveAll(s => s is CScrewRectSequence);
                        // Set current number of rectangular sequences (it should be "0" in case that corner screw sequences are not used, all other sequences are half-circle)
                        group.NumberOfRectangularSequences = 0;
                    }
                }
            }
            if (!UseExtraScrews) // extra screws are deactivated
            {
                // Remove all rectangular sequences
                foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
                {
                    if (BUseAdditionalCornerScrews)
                    {
                        if (group.ListSequence.Where(s => s is CScrewRectSequence).Count() == 6)
                        {
                            List<CConnectorSequence> sequencesToRemove = group.ListSequence.Where(s => s is CScrewRectSequence).ToList().GetRange(4, 2);
                            group.ListSequence.RemoveAll(s => sequencesToRemove.Contains(s));                            
                            group.NumberOfRectangularSequences = 4;
                        }
                    }
                    else
                    {
                        group.ListSequence.RemoveAll(s => s is CScrewRectSequence);
                        // Set current number of rectangular sequences (it should be "0" in case that corner screw sequences are not used, all other sequences are half-circle)
                        group.NumberOfRectangularSequences = 0;
                    }
                }
            }

            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            RecalculateTotalNumberOfScrews();

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }


        public override void UpdateArrangmentData()
        {   
            // Additional Corner Screws            
            UpdateAdditionalScrews();
        }

        public void Get_ScrewGroup_IncludingAdditionalScrews(float fx_c,
        float fy_c,
        float fRotation_rad,
        ref CScrewSequenceGroup group)
        {
            float fAngle_seq_rotation_deg = fRotation_rad * 180f / MathF.fPI; // Input value (roof pitch)
            float fAdditionalMargin = 0.02f; //naco je toto dobre???
            // To Ondrej Uprostred prierezu 63020 (vid crsc 3D view je "vyztuha", teda je tam medzera, do ktorej sa neda pripevnit plech skrutkami (preto je kruh skrutiek rozdeleny na 2 segmenty)
            // hodnota additional margin je pouzita na to aby nebola krajna skrutka v kruhovej sekvencii priamo na zakrivenej hrane ale dalej, nastavene je tvrdo 20 mm ale moze to byt aj viac a nastavitelne uzivatelom)
            // dalo by sa to pripocitavat priamo k FStiffenerSize
            // Zatial som to nerobil user-defined, lebo to nie je az take dolezite, nastavovat 20 mm alebo 30 mm, uvidim ci to budu chciet naozaj nastavovat

            if (group.NumberOfHalfCircleSequences > 0)
            {
                int count = 0;
                foreach (CScrewSequence screwSequence in group.ListSequence)
                {
                    if (!(screwSequence is CScrewHalfCircleSequence)) continue;
                    CScrewHalfCircleSequence circSeq = screwSequence as CScrewHalfCircleSequence;

                    // Input - according to the size of middle sfiffener, additional margin and circle radius
                    float fAngle_seq_rotation_init_point_deg = (float)(Math.Asin((0.5f * FStiffenerSize + fAdditionalMargin) / circSeq.Radius) / MathF.fPI * 180f);
                    // Angle between sequence center, first and last point in the sequence
                    float fAngle_interval_deg = 180 - (2f * fAngle_seq_rotation_init_point_deg);
                    if (count % 2 == 0)
                    {
                        // Half circle sequence
                        List<Point> fSequenceTop = Geom2D.GetArcPointCoord_CCW_deg(circSeq.Radius, fAngle_seq_rotation_init_point_deg, fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, circSeq.INumberOfConnectors, false);
                        screwSequence.HolesCentersPoints = fSequenceTop.ToArray();
                    }
                    else
                    {
                        List<Point> fSequenceBottom = Geom2D.GetArcPointCoord_CCW_deg(circSeq.Radius, 180 + fAngle_seq_rotation_init_point_deg, 180 + fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, circSeq.INumberOfConnectors, false);
                        screwSequence.HolesCentersPoints = fSequenceBottom.ToArray();
                    }
                    count++;
                }
            }

            // Add additional point the sequences
            if (BUseAdditionalCornerScrews && group.NumberOfRectangularSequences > 0)
            {
                bool bSetAdditionalCornerScrewsByRadius_SQ1 = false; // TODO - Ondrej, moze sa pridat do GUI, uzivatel moze riadit polohu podla hodnoty radiusu prvej kruhovej sekvencie ale ju zadavat nezavisle

                CScrewSequence seq = (CScrewSequence)group.ListSequence.FirstOrDefault(s => s is CScrewHalfCircleSequence);

                if (bSetAdditionalCornerScrewsByRadius_SQ1 && seq != null) FPositionOfCornerSequence_x = ((CScrewHalfCircleSequence)seq).Radius;
                if (bSetAdditionalCornerScrewsByRadius_SQ1 && seq != null) FPositionOfCornerSequence_y = ((CScrewHalfCircleSequence)seq).Radius;

                // For square
                if (IAdditionalConnectorNumberInRow_xDirection == 0 && IAdditionalConnectorNumberInColumn_yDirection == 0)
                {
                    IAdditionalConnectorNumberInRow_xDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber);
                    IAdditionalConnectorNumberInColumn_yDirection = (int)Math.Sqrt(IAdditionalConnectorInCornerNumber);
                }

                // Additional corner connectors
                // Top part of group
                Point[] cornerConnectorsInGroupTopLeft = GetAdditionaConnectorsCoordinatesInOneSequence(new Point(-FPositionOfCornerSequence_x, FPositionOfCornerSequence_y - (IAdditionalConnectorNumberInColumn_yDirection - 1) * FAdditionalCornerScrewsDistance_y), IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection, FAdditionalCornerScrewsDistance_x, FAdditionalCornerScrewsDistance_y);
                float fDistanceBetweenLeftAndRightReferencePoint = 2 * FPositionOfCornerSequence_x - (IAdditionalConnectorNumberInRow_xDirection - 1) * FAdditionalCornerScrewsDistance_x;
                Point[] cornerConnectorsInGroupTopRight = GetAdditionaConnectorsCoordinatesInOneSequence(new Point(-FPositionOfCornerSequence_x + fDistanceBetweenLeftAndRightReferencePoint, FPositionOfCornerSequence_y - (IAdditionalConnectorNumberInColumn_yDirection - 1) * FAdditionalCornerScrewsDistance_y), IAdditionalConnectorNumberInRow_xDirection, IAdditionalConnectorNumberInColumn_yDirection, FAdditionalCornerScrewsDistance_x, FAdditionalCornerScrewsDistance_y);

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
                IEnumerable<CConnectorSequence> rectSequences = group.ListSequence.Where(s => s is CScrewRectSequence);
                if (rectSequences.ElementAtOrDefault(0) != null) rectSequences.ElementAtOrDefault(0).HolesCentersPoints = cornerConnectorsInGroupTopLeft;
                if (rectSequences.ElementAtOrDefault(1) != null) rectSequences.ElementAtOrDefault(1).HolesCentersPoints = cornerConnectorsInGroupTopRight;
                if (rectSequences.ElementAtOrDefault(2) != null) rectSequences.ElementAtOrDefault(2).HolesCentersPoints = cornerConnectorsInGroupBottomLeft;
                if (rectSequences.ElementAtOrDefault(3) != null) rectSequences.ElementAtOrDefault(3).HolesCentersPoints = cornerConnectorsInGroupBottomRight;
            }

            // Add extra line of points
            if (UseExtraScrews && group.NumberOfRectangularSequences > 0)
            {
                // Pozicia rows pre smer x ma byt symetrická podla stredu group (polohu urcuje stred polkruhovej sekvencie)
                // Spocitame celkovú dĺžku radu skrutiek - suradnica je vlavo od stredu, takze je to zaporna polovica dlzky radu skrutiek
                float fExtraScrewsRowLength_x_Direction = (iExtraNumberOfScrewsInRow - 1) * ExtraScrewsDistance_x;
                float fPositionOfExtraSequence_x = 0.5f * fExtraScrewsRowLength_x_Direction; // Toto nebude parameter, ale urcuje sa automaticky

                // Top part of group
                Point[] extraConnectorsInGroupTop = GetAdditionaConnectorsCoordinatesInOneSequence(new Point(-fPositionOfExtraSequence_x, PositionOfExtraScrewsSequence_y - (iExtraNumberOfRows - 1) * ExtraScrewsDistance_y), ExtraNumberOfScrewsInRow, iExtraNumberOfRows, ExtraScrewsDistance_x, ExtraScrewsDistance_y);

                // Bottom part of group
                Point[] extraConnectorsInGroupBottom = new Point[extraConnectorsInGroupTop.Length];

                // Copy items
                extraConnectorsInGroupTop.CopyTo(extraConnectorsInGroupBottom, 0);

                // Rotate bottom part
                Geom2D.TransformPositions_CCW_deg(0, 0, 180, ref extraConnectorsInGroupBottom);

                // Set group parameters
                IEnumerable<CConnectorSequence> rectSequences = group.ListSequence.Where(s => s is CScrewRectSequence);
                //change last 2 rect sequences
                if (rectSequences.ElementAtOrDefault(rectSequences.Count() - 1) != null) rectSequences.ElementAtOrDefault(rectSequences.Count() - 1).HolesCentersPoints = extraConnectorsInGroupTop;
                if (rectSequences.ElementAtOrDefault(rectSequences.Count() - 2) != null) rectSequences.ElementAtOrDefault(rectSequences.Count() - 2).HolesCentersPoints = extraConnectorsInGroupBottom;
            }

            // Set radii of connectors / screws in the group
            group.HolesRadii = group.Get_RadiiOfConnectorsInGroup();

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
            float fOffset_x,
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        {
            float fcut = 0.005f;

            // Riadiaci bod je vlavo dole [0, 0]
            float fCenterPosition_original_unrotated_x = ((fbX / 2f) / (float)Math.Cos(fSlope_rad)) - fcut - 0.5f * FCrscRafterDepth;
            float fCenterPosition_original_unrotated_y = 0.5f * FCrscRafterDepth;

            float fx_c1 = fOffset_x + Geom2D.GetRotatedPosition_x_CCW_rad(fCenterPosition_original_unrotated_x, fCenterPosition_original_unrotated_y, fSlope_rad);
            float fy_c1 = flZ + Geom2D.GetRotatedPosition_y_CCW_rad(fCenterPosition_original_unrotated_x, fCenterPosition_original_unrotated_y, fSlope_rad);

            float fx_c2 = fbX - fx_c1; // Symmetrical
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
            float fy_c1 = fbX_1 / 2f; //fhY_1 / 4f;

            // Top Circle (Main Rafter)
            // Riadiaci bod je vlavo hore [0, fhY_1]
            float fcut = 0.005f;

            float fAdditional_x = fSlope_rad > 0 ? 0 : FCrscRafterDepth * (float)Math.Tan(-fSlope_rad); // Falling knee (distance in x direction for 0 deg

            float fx_c2 = flZ + Geom2D.GetRotatedPosition_x_CCW_rad(fAdditional_x + 0.5f * FCrscRafterDepth + fcut, -0.5f * FCrscRafterDepth, fSlope_rad);
            float fy_c2 = fhY_1 + Geom2D.GetRotatedPosition_y_CCW_rad(fAdditional_x + 0.5f * FCrscRafterDepth + fcut, -0.5f * FCrscRafterDepth, fSlope_rad);

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
            float fOffset_x,
            float fbX,
            float flZ,
            float fhY_1,
            float ft,
            float fSlope_rad,
            bool bScrewInPlusZDirection = true)
        {
            Calc_HolesCentersCoord2DApexPlate(fOffset_x, fbX, flZ, fhY_1, fSlope_rad);
            Calc_HolesControlPointsCoord3D_FlatPlate(fOffset_x, flZ, ft, bScrewInPlusZDirection);
            GenerateConnectors_FlatPlate(bScrewInPlusZDirection);
        }

        public override void Calc_KneePlateData(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float ft,
            float fSlope_rad,
            bool bScrewInPlusZDirection = false)
        {
            Calc_HolesCentersCoord2DKneePlate(fbX_1, fbX_2, flZ, fhY_1, fSlope_rad);
            Calc_HolesControlPointsCoord3D_FlatPlate(flZ, 0, ft, bScrewInPlusZDirection);
            GenerateConnectors_FlatPlate(bScrewInPlusZDirection);
        }

        public CScrewSequenceGroup GetMirroredScrewGroupAboutY(CScrewSequenceGroup group)
        {
            CScrewSequenceGroup groupOut = new CScrewSequenceGroup();

            for (int i = 0; i < group.ListSequence.Count; i++)
            {
                CScrewSequence seqTemp = new CScrewSequence();
                seqTemp.HolesCentersPoints = new Point[group.ListSequence[i].HolesCentersPoints.Length];

                for (int j = 0; j < group.ListSequence[i].HolesCentersPoints.Length; j++)
                {
                    seqTemp.HolesCentersPoints[j].X = group.ListSequence[i].HolesCentersPoints[j].X * -1; // Change X coordinate (mirror about Y)
                    seqTemp.HolesCentersPoints[j].Y = group.ListSequence[i].HolesCentersPoints[j].Y;
                }

                groupOut.ListSequence.Add(seqTemp);
            }

            return groupOut;
        }
    }
}