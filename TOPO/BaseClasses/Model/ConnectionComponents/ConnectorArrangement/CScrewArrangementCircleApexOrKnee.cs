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

        private float m_fRadius;

        public float FRadius
        {
            get
            {
                return m_fRadius;
            }

            set
            {
                m_fRadius = value;
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

        private int iNumberOfScrewsInCircleGroup; // Pocet skrutiek v kruhu
        private int iNumberOfScrewsInOneHalfCircleSequence; // pocet skrutiek v "polkruhu"
        private int iNumberOfAdditionalConnectorsInOneGroup;
        private int iNumberOfAdditionalConnectorsInOneSequence;

        private int iNumberOfScrewsInOneGroupIncludingAdditional;
        private int iNumberOfScrewsInOneSequenceIncludingAdditional;

        float m_fSlope_rad;
        public float[] HolesCenterRadii;

        public CScrewArrangementCircleApexOrKnee()
        { }

        public CScrewArrangementCircleApexOrKnee(
            int iHolesInCirclesNumber_temp,
            CScrew referenceScrew_temp,
            float fRadius_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            bool bUseAdditionalCornerScrews_temp,
            int iAdditionalConnectorNumber_temp,
            float fAdditionalScrewsDistance_x_temp,
            float fAdditionalScrewsDistance_y_temp) : base(iHolesInCirclesNumber_temp + (bUseAdditionalCornerScrews_temp ? iAdditionalConnectorNumber_temp : 0), referenceScrew_temp)
        {
            IHolesInCirclesNumber = iHolesInCirclesNumber_temp;
            referenceScrew = referenceScrew_temp;
            FRadius = fRadius_temp;
            FCrscRafterDepth = fCrscRafterDepth_temp;
            FCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            FStiffenerSize = fStiffenerSize_temp;
            BUseAdditionalCornerScrews = bUseAdditionalCornerScrews_temp;
            IAdditionalConnectorNumber = iAdditionalConnectorNumber_temp;
            FAdditionalScrewsDistance_x = fAdditionalScrewsDistance_x_temp;
            FAdditionalScrewsDistance_y = fAdditionalScrewsDistance_y_temp;

            UpdateArrangmentData();
        }

        public void UpdateArrangmentData()
        {
            IHolesNumber = IHolesInCirclesNumber + (BUseAdditionalCornerScrews ? IAdditionalConnectorNumber : 0);
            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];

            iNumberOfScrewsInCircleGroup = IHolesInCirclesNumber / INumberOfCircleGroupsInJoint;
            iNumberOfScrewsInOneHalfCircleSequence = iNumberOfScrewsInCircleGroup / INumberOfCircleSequencesInGroup;

            iNumberOfAdditionalConnectorsInOneGroup = BUseAdditionalCornerScrews ? IAdditionalConnectorNumber / INumberOfCircleGroupsInJoint : 0;
            iNumberOfAdditionalConnectorsInOneSequence = BUseAdditionalCornerScrews ? iNumberOfAdditionalConnectorsInOneGroup / INumberOfCircleGroupsInJoint : 0;

            iNumberOfScrewsInOneGroupIncludingAdditional = iNumberOfScrewsInCircleGroup + iNumberOfAdditionalConnectorsInOneGroup;
            iNumberOfScrewsInOneSequenceIncludingAdditional = iNumberOfScrewsInOneHalfCircleSequence + iNumberOfAdditionalConnectorsInOneSequence;
        }

        public void Get_ScrewGroup_IncludingAdditionalScrews(
            float fx_c,
            float fy_c,
            float fAngle_seq_rotation_init_point_deg,
            float fRotation_rad,
            out Point[] fSequenceTop,
            out Point[] fSequenceBottom,
            out float[] fSequenceTopRadii,
            out float[] fSequenceBottomRadii)
        {
            float fAngle_seq_rotation_deg = fRotation_rad * 180f / MathF.fPI; // Input value (roof pitch)

            float fAngle_interval_deg = 180 - (2f * fAngle_seq_rotation_init_point_deg); // Angle between sequence center, first and last point in the sequence

            // Half circle sequence
            float[,] fSequenceTop_temp = Geom2D.GetArcPointCoordArray_CCW_deg(FRadius, fAngle_seq_rotation_init_point_deg, fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, iNumberOfScrewsInOneHalfCircleSequence, false);
            float[,] fSequenceBottom_temp = Geom2D.GetArcPointCoordArray_CCW_deg(FRadius, 180 + fAngle_seq_rotation_init_point_deg, 180 + fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, iNumberOfScrewsInOneHalfCircleSequence, false);

            // TODO - docasne, previest pole float na pole Points
            fSequenceTop = GetConvertedFloatToPointArray(fSequenceTop_temp);
            fSequenceBottom = GetConvertedFloatToPointArray(fSequenceBottom_temp);

            // Add addtional point the sequences
            if (BUseAdditionalCornerScrews)
            {
                // For square
                int iNumberOfScrewsInColumn_xDirection = (int)Math.Sqrt(iNumberOfAdditionalConnectorsInOneSequence / 2);
                int iNumberOfScrewsInRow_yDirection = (int)Math.Sqrt(iNumberOfAdditionalConnectorsInOneSequence / 2);

                // Additional corner connectors in Sequence
                Point[] cornerConnectorsInTopSequence = GetAdditionaConnectorsCoordinatesInOneSequence(2 * FRadius - (iNumberOfScrewsInColumn_xDirection - 1) * FAdditionalScrewsDistance_x, new Point(-FRadius, FRadius - (iNumberOfScrewsInRow_yDirection - 1) * FAdditionalScrewsDistance_y), 0, iNumberOfScrewsInColumn_xDirection, iNumberOfScrewsInRow_yDirection, FAdditionalScrewsDistance_x, FAdditionalScrewsDistance_y);
                Point[] cornerConnectorsInBottomSequence = GetAdditionaConnectorsCoordinatesInOneSequence(2 * FRadius - (iNumberOfScrewsInColumn_xDirection - 1) * FAdditionalScrewsDistance_x, new Point(-FRadius, FRadius - (iNumberOfScrewsInRow_yDirection - 1) * FAdditionalScrewsDistance_y), 180, iNumberOfScrewsInColumn_xDirection, iNumberOfScrewsInRow_yDirection, FAdditionalScrewsDistance_x, FAdditionalScrewsDistance_y);

                // Add additional connectors into the array
                // Store original array
                Point[] fSequenceTop_original = fSequenceTop;
                Point[] fSequenceBottom_original = fSequenceBottom;

                // Set new size of array (items are deleted), TODO - find way how to resize two dimensional array
                fSequenceTop = new Point[fSequenceTop_original.Length + cornerConnectorsInTopSequence.Length];
                fSequenceBottom = new Point[fSequenceBottom_original.Length + cornerConnectorsInBottomSequence.Length];

                // Add items (point coordinates) from original array
                for (int i = 0; i < fSequenceTop_original.Length; i++)
                {
                    fSequenceTop[i].X = fSequenceTop_original[i].X;
                    fSequenceTop[i].Y = fSequenceTop_original[i].Y;
                }

                for (int i = 0; i < fSequenceBottom_original.Length; i++)
                {
                    fSequenceBottom[i].X = fSequenceBottom_original[i].X;
                    fSequenceBottom[i].Y = fSequenceBottom_original[i].Y;
                }

                // Add items (point coordinates) from additional array of connectors
                for (int i = 0; i < cornerConnectorsInTopSequence.Length / 2; i++)
                {
                    fSequenceTop[fSequenceTop_original.Length + i].X = cornerConnectorsInTopSequence[i].X;
                    fSequenceTop[fSequenceTop_original.Length + i].Y = cornerConnectorsInTopSequence[i].Y;
                }

                for (int i = 0; i < cornerConnectorsInBottomSequence.Length / 2; i++)
                {
                    fSequenceBottom[fSequenceBottom_original.Length + i].X = cornerConnectorsInBottomSequence[i].X;
                    fSequenceBottom[fSequenceBottom_original.Length + i].Y = cornerConnectorsInBottomSequence[i].Y;
                }
            }

            // Rotate about [0,0]
            Geom2D.TransformPositions_CCW_deg(0, 0, fAngle_seq_rotation_deg, ref fSequenceTop);
            Geom2D.TransformPositions_CCW_deg(0, 0, fAngle_seq_rotation_deg, ref fSequenceBottom);

            // Set radii of connectors / screws in the connection
            fSequenceTopRadii = new float[fSequenceTop.Length];

            for (int i = 0; i < fSequenceTop.Length; i++)
                fSequenceTopRadii[i] = (float)Math.Sqrt(MathF.Pow2(fSequenceTop[i].X) + MathF.Pow2(fSequenceTop[i].Y));

            fSequenceBottomRadii = new float[fSequenceBottom.Length];
            for (int i = 0; i < fSequenceTop.Length; i++)
                fSequenceBottomRadii[i] = (float)Math.Sqrt(MathF.Pow2(fSequenceBottom[i].X) + MathF.Pow2(fSequenceBottom[i].Y));

            // Translate
            Geom2D.TransformPositions_CCW_deg(fx_c, fy_c, 0, ref fSequenceTop);
            Geom2D.TransformPositions_CCW_deg(fx_c, fy_c, 0, ref fSequenceBottom);
        }

        public Point[] GetAdditionaConnectorsCoordinatesInOneSequence(float fDistanceBetweenCornerPartsControlPointsX,
            Point refPoint,
            float fRotationAngle_deg,
            int iNumberOfPointsInXDirection,
            int iNumberOfPointsInYDirection,
            float fDistanceOfPointsX,
            float fDistanceOfPointsY)
        {
            Point[] fLeftPoints = GetRegularArrayOfPointsInCartesianCoordinates(refPoint, iNumberOfPointsInXDirection, iNumberOfPointsInYDirection, fDistanceOfPointsX, fDistanceOfPointsY);

            Point pRefRight = new Point(fDistanceBetweenCornerPartsControlPointsX + refPoint.X, refPoint.Y);
            Point[] fRightPoints = GetRegularArrayOfPointsInCartesianCoordinates(pRefRight, iNumberOfPointsInXDirection, iNumberOfPointsInYDirection, fDistanceOfPointsX, fDistanceOfPointsY);

            Point[] array = new Point[2 * iNumberOfPointsInXDirection * iNumberOfPointsInYDirection];

            for (int i = 0; i < iNumberOfPointsInXDirection * iNumberOfPointsInYDirection; i++) // Merge two array into one
            {
                array[i].X = fLeftPoints[i].X;
                array[i].Y = fLeftPoints[i].Y;

                array[iNumberOfPointsInXDirection * iNumberOfPointsInYDirection + i].X = fRightPoints[i].X;
                array[iNumberOfPointsInXDirection * iNumberOfPointsInYDirection + i].Y = fRightPoints[i].Y;
            }

            // Rotate points about [0,0] // Used for top or bottom sequence (0 or 180 degrees)
            Geom2D.TransformPositions_CCW_deg(0, 0, fRotationAngle_deg, ref array);

            return array;
        }

        public void Calc_HolesCentersCoord2DApexPlate(
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad,
            ref Point[] fHolesCentersPoints2D)
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
            Point[] fSequenceLeftTop;
            Point[] fSequenceLeftBottom;
            float[] fSequenceLeftTopRadii;
            float[] fSequenceLeftBottomRadii;
            Get_ScrewGroup_IncludingAdditionalScrews(fx_c1, fy_c1, fAngle_seq_rotation_init_point_deg, fSlope_rad, out fSequenceLeftTop, out fSequenceLeftBottom, out fSequenceLeftTopRadii, out fSequenceLeftBottomRadii);

            // Right side
            Point[] fSequenceRightTop;
            Point[] fSequenceRightBottom;
            float[] fSequenceRightTopRadii;
            float[] fSequenceRightBottomRadii;
            Get_ScrewGroup_IncludingAdditionalScrews(fx_c2, fy_c2, fAngle_seq_rotation_init_point_deg, -fSlope_rad, out fSequenceRightTop, out fSequenceRightBottom, out fSequenceRightTopRadii, out fSequenceRightBottomRadii);

            // Fill array of holes centers
            for (int i = 0; i < iNumberOfScrewsInOneSequenceIncludingAdditional; i++) // Add all 4 sequences in one cycle
            {
                HolesCentersPoints2D[i] = new Point(fSequenceLeftTop[i].X, fSequenceLeftTop[i].Y);
                HolesCentersPoints2D[iNumberOfScrewsInOneSequenceIncludingAdditional + i] = new Point(fSequenceLeftBottom[i].X, fSequenceLeftBottom[i].Y);
                HolesCentersPoints2D[2 * iNumberOfScrewsInOneSequenceIncludingAdditional + i] = new Point(fSequenceRightTop[i].X, fSequenceRightTop[i].Y);
                HolesCentersPoints2D[3 * iNumberOfScrewsInOneSequenceIncludingAdditional + i] = new Point(fSequenceRightBottom[i].X, fSequenceRightBottom[i].Y);
            }

            // TODO - tempoerary nastavit pre pole suradnic ktore je sucastou plate
            // teoereticky moze mat usporadanie iny pocet ako je pocet na plate, napriklad ak sa usporiadanie odzrkadli alebo skopiruje vramci plochy (napr. Typ KE)
            fHolesCentersPoints2D = HolesCentersPoints2D;
        }

        public void Calc_HolesCentersCoord2DKneePlate(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float fSlope_rad,
            ref Point[] fHolesCentersPoints2D,
            ref float[] fHolesCenterRadii)
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

            // Left side
            Point[] fSequenceLeftTop;
            Point[] fSequenceLeftBottom;
            float[] fSequenceLeftTopRadii;
            float[] fSequenceLeftBottomRadii;
            Get_ScrewGroup_IncludingAdditionalScrews(fx_c1, fy_c1, fAngle_seq_rotation_init_point_deg, MathF.fPI / 2f, out fSequenceLeftTop, out fSequenceLeftBottom, out fSequenceLeftTopRadii, out fSequenceLeftBottomRadii);

            // Right side
            Point[] fSequenceRightTop;
            Point[] fSequenceRightBottom;
            float[] fSequenceRightTopRadii;
            float[] fSequenceRightBottomRadii;
            Get_ScrewGroup_IncludingAdditionalScrews(fx_c2, fy_c2, fAngle_seq_rotation_init_point_deg, fSlope_rad, out fSequenceRightTop, out fSequenceRightBottom, out fSequenceRightTopRadii, out fSequenceRightBottomRadii);

            // Fill array of holes centers
            for (int i = 0; i < iNumberOfScrewsInOneSequenceIncludingAdditional; i++) // Add all 4 sequences in one cycle
            {
                HolesCentersPoints2D[i] = new Point(fSequenceLeftTop[i].X, fSequenceLeftTop[i].Y);
                HolesCentersPoints2D[iNumberOfScrewsInOneSequenceIncludingAdditional + i] = new Point(fSequenceLeftBottom[i].X, fSequenceLeftBottom[i].Y);
                HolesCentersPoints2D[2 * iNumberOfScrewsInOneSequenceIncludingAdditional + i] = new Point(fSequenceRightTop[i].X, fSequenceRightTop[i].Y);
                HolesCentersPoints2D[3 * iNumberOfScrewsInOneSequenceIncludingAdditional + i] = new Point(fSequenceRightBottom[i].X, fSequenceRightBottom[i].Y);

                fHolesCenterRadii[i] = fSequenceLeftTopRadii[i];
                fHolesCenterRadii[iNumberOfScrewsInOneSequenceIncludingAdditional + i] = fSequenceLeftBottomRadii[i];
                fHolesCenterRadii[2 * iNumberOfScrewsInOneSequenceIncludingAdditional + i] = fSequenceRightTopRadii[i];
                fHolesCenterRadii[3 * iNumberOfScrewsInOneSequenceIncludingAdditional + i] = fSequenceRightBottomRadii[i];
            }

            // TODO - temporary nastavit pre pole suradnic ktore je sucastou plate
            // teoereticky moze mat usporadanie iny pocet ako je pocet na plate, napriklad ak sa usporiadanie odzrkadli alebo skopiruje vramci plochy (napr. Typ KE)
            fHolesCentersPoints2D = HolesCentersPoints2D;
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