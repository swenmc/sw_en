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
    public class CScrewArrangementCircleApexOrKnee : CScrewArrangement
    {
        private int m_iHolesInCirclesNumber;

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

        private bool m_bUseAdditionalCornerScrews;

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

        private int m_iNumberOfCircleGroupsInJoint = 2; // Pocet kruhov na jednom plechu

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

        int m_iNumberOfCircleSequencesInGroup = 2; // pocet polkruhov v "kruhu" na jednom plechu

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

        int iNumberOfScrewsInCircleGroup; // Pocet skrutiek v kruhu
        int iNumberOfScrewsInOneSequence; // pocet skrutiek v "polkruhu"

        float m_fSlope_rad;
        public float[] HolesCenterRadii;

        public CScrewArrangementCircleApexOrKnee(
            int iHolesInCirclesNumber_temp,
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            bool bUseAdditionalCornerScrews_temp,
            int iAdditionalConnectorNumber_temp) : base(iHolesInCirclesNumber_temp + iAdditionalConnectorNumber_temp, referenceScrew_temp)
        {
            IHolesInCirclesNumber = iHolesInCirclesNumber_temp;
            referenceScrew = referenceScrew_temp;
            m_fCrscRafterDepth = fCrscRafterDepth_temp;
            m_fCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            m_fStiffenerSize = fStiffenerSize_temp;
            m_bUseAdditionalCornerScrews = bUseAdditionalCornerScrews_temp;
            m_iAdditionalConnectorNumber = iAdditionalConnectorNumber_temp;

            IHolesNumber = IHolesInCirclesNumber + IAdditionalConnectorNumber;
            HolesCentersPoints2D = new float[IHolesNumber, 2];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];

            iNumberOfScrewsInCircleGroup = IHolesInCirclesNumber / INumberOfCircleGroupsInJoint;
            iNumberOfScrewsInOneSequence = iNumberOfScrewsInCircleGroup / INumberOfCircleSequencesInGroup;
        }

        public void Get_ScrewGroup_Circle(
            float fx_c,
            float fy_c,
            float fRadius,
            float fAngle_seq_rotation_init_point_deg,
            float fRotation_rad,
            bool bUseAdditionalCornerScrews,
            int iAdditionalConnectorNumberinGroup,
            out float[,] fSequenceTop,
            out float[,] fSequenceBottom,
            out float[] fSequenceTopRadii,
            out float[] fSequenceBottomRadii)
        {
            float fAngle_seq_rotation_deg = fRotation_rad * 180f / MathF.fPI; // Input value (roof pitch)

            float fAngle_interval_deg = 180 - (2f * fAngle_seq_rotation_init_point_deg); // Angle between sequence center, first and last point in the sequence

            // Circle sequence
            fSequenceTop = Geom2D.GetArcPointCoordArray_CCW_deg(fRadius, fAngle_seq_rotation_init_point_deg, fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, iNumberOfScrewsInOneSequence, false);
            fSequenceBottom = Geom2D.GetArcPointCoordArray_CCW_deg(fRadius, 180 + fAngle_seq_rotation_init_point_deg, 180 + fAngle_seq_rotation_init_point_deg + fAngle_interval_deg, iNumberOfScrewsInOneSequence, false);

            // Add addtional point the sequences
            if (bUseAdditionalCornerScrews)
            {
                // Additional corner connectors in Sequence
                float fDistance_y = 0.03f; // TODO - konstanta podla rozmerov prierezu
                float fDistance_x = fDistance_y; // Square arrangement

                float[,] cornerConnectorsInTopSequence = GetAdditionaConnectorsCoordinatesInOneSequence(2 * fRadius - fDistance_x, -fRadius, fRadius - fDistance_y, 0, 2, 2, fDistance_x, fDistance_y);
                float[,] cornerConnectorsInBottomSequence = GetAdditionaConnectorsCoordinatesInOneSequence(2 * fRadius - fDistance_x, -fRadius, fRadius - fDistance_y, 180, 2, 2, fDistance_x, fDistance_y);

                // Add additional connectors into the array
                // Store original array
                float[,] fSequenceTop_original = fSequenceTop;
                float[,] fSequenceBottom_original = fSequenceBottom;

                // Set new size of array (items are deleted), TODO - find way how to resize two dimensional array
                fSequenceTop = new float[fSequenceTop_original.Length / 2 + cornerConnectorsInTopSequence.Length / 2, 2];
                fSequenceBottom = new float[fSequenceBottom_original.Length / 2 + cornerConnectorsInBottomSequence.Length / 2, 2];

                // Add items (point coordinates) from original array
                for (int i = 0; i < fSequenceTop_original.Length / 2; i++)
                {
                    fSequenceTop[i, 0] = fSequenceTop_original[i, 0];
                    fSequenceTop[i, 1] = fSequenceTop_original[i, 1];
                }

                for (int i = 0; i < fSequenceBottom_original.Length / 2; i++)
                {
                    fSequenceBottom[i, 0] = fSequenceBottom_original[i, 0];
                    fSequenceBottom[i, 1] = fSequenceBottom_original[i, 1];
                }

                // Add items (point coordinates) from additional array of connectors
                for (int i = 0; i < cornerConnectorsInTopSequence.Length / 2; i++)
                {
                    fSequenceTop[fSequenceTop_original.Length / 2 + i, 0] = cornerConnectorsInTopSequence[i, 0];
                    fSequenceTop[fSequenceTop_original.Length / 2 + i, 1] = cornerConnectorsInTopSequence[i, 1];
                }

                for (int i = 0; i < cornerConnectorsInBottomSequence.Length / 2; i++)
                {
                    fSequenceBottom[fSequenceBottom_original.Length / 2 + i, 0] = cornerConnectorsInBottomSequence[i, 0];
                    fSequenceBottom[fSequenceBottom_original.Length / 2 + i, 1] = cornerConnectorsInBottomSequence[i, 1];
                }
            }

            // Rotate about [0,0]
            Geom2D.TransformPositions_CCW_deg(0, 0, fAngle_seq_rotation_deg, ref fSequenceTop);
            Geom2D.TransformPositions_CCW_deg(0, 0, fAngle_seq_rotation_deg, ref fSequenceBottom);

            // Set radii of connectors / screws in the connection
            fSequenceTopRadii = new float[fSequenceTop.Length / 2];

            for (int i = 0; i < fSequenceTop.Length / 2; i++)
                fSequenceTopRadii[i] = (float)Math.Sqrt(MathF.Pow2(fSequenceTop[i, 0]) + MathF.Pow2(fSequenceTop[i, 1]));

            fSequenceBottomRadii = new float[fSequenceBottom.Length / 2];
            for (int i = 0; i < fSequenceTop.Length / 2; i++)
                fSequenceBottomRadii[i] = (float)Math.Sqrt(MathF.Pow2(fSequenceBottom[i, 0]) + MathF.Pow2(fSequenceBottom[i, 1]));

            // Translate
            Geom2D.TransformPositions_CCW_deg(fx_c, fy_c, 0, ref fSequenceTop);
            Geom2D.TransformPositions_CCW_deg(fx_c, fy_c, 0, ref fSequenceBottom);
        }

        public float[,] GetAdditionaConnectorsCoordinatesInOneSequence(float fDistanceBetweenCornerPartsControlPointsX,
            float fcPointX,
            float fcPointY,
            float fRotationAngle_deg,
            int iNumberOfPointsInXDirection,
            int iNumberOfPointsInYDirection,
            float fDistanceOfPointsX,
            float fDistanceOfPointsY)
        {
            float[,] fLeftPoints = GetRegularArrayOfPointsInCartesianCoordinates(fcPointX, fcPointY, iNumberOfPointsInXDirection, iNumberOfPointsInYDirection, fDistanceOfPointsX, fDistanceOfPointsY);
            float[,] fRightPoints = GetRegularArrayOfPointsInCartesianCoordinates(fDistanceBetweenCornerPartsControlPointsX + fcPointX, fcPointY, iNumberOfPointsInXDirection, iNumberOfPointsInYDirection, fDistanceOfPointsX, fDistanceOfPointsY);

            float[,] array = new float[2 * iNumberOfPointsInXDirection * iNumberOfPointsInYDirection, 2];

            for (int i = 0; i < iNumberOfPointsInXDirection * iNumberOfPointsInYDirection; i++) // Merge two array into one
            {
                array[i, 0] = fLeftPoints[i, 0];
                array[i, 1] = fLeftPoints[i, 1];

                array[iNumberOfPointsInXDirection * iNumberOfPointsInYDirection + i, 0] = fRightPoints[i, 0];
                array[iNumberOfPointsInXDirection * iNumberOfPointsInYDirection + i, 1] = fRightPoints[i, 1];
            }

            // Rotate points about [0,0] // Used for top or bottom sequence (0 or 180 degrees)
            Geom2D.TransformPositions_CCW_deg(0, 0, fRotationAngle_deg, ref array);

            return array;
        }

        public void Calc_HolesCentersCoord2D(
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad,
            bool bUseAdditionalCornerScrews,
            int iAdditionalConnectorNumber,
            float fCrscWebStraightDepth,
            float fStiffenerSize,
            ref float [,] fHolesCentersPoints2D)
        {
            // Circle
            bool bIsCircleJointArrangement = true;

            if (bIsCircleJointArrangement)
            {
                float fDistanceOfCenterFromLeftEdge = fbX / 4f;
                float fx_c1 = fDistanceOfCenterFromLeftEdge;
                float fy_c1 = flZ + ((fhY_1 / 2f) / (float)Math.Cos(fSlope_rad)) + (fDistanceOfCenterFromLeftEdge * (float)Math.Tan(fSlope_rad));

                float fx_c2 = fbX - fDistanceOfCenterFromLeftEdge; // Symmetrical
                float fy_c2 = fy_c1;

                int iNumberOfAddionalConnectorsInOneGroup = bUseAdditionalCornerScrews ? (iAdditionalConnectorNumber / INumberOfCircleGroupsInJoint) : 0;
                int iNumberOfScrewsInOneSequence = IHolesNumber / (INumberOfCircleGroupsInJoint * INumberOfCircleSequencesInGroup);

                float fAdditionalMargin = 0.01f; // Temp - TODO - put to the input data
                float fRadius = 0.5f * fCrscWebStraightDepth - 2 * fAdditionalMargin; // m // Input - depending on depth of cross-section
                float fAngle_seq_rotation_init_point_deg = (float)(Math.Atan(0.5f * fStiffenerSize / fDistanceOfCenterFromLeftEdge) / MathF.fPI * 180f); // Input - constant for cross-section according to the size of middle sfiffener

                // Left side
                float[,] fSequenceLeftTop;
                float[,] fSequenceLeftBottom;
                float[] fSequenceLeftTopRadii;
                float[] fSequenceLeftBottomRadii;
                Get_ScrewGroup_Circle(fx_c1, fy_c1, fRadius, fAngle_seq_rotation_init_point_deg, fSlope_rad, bUseAdditionalCornerScrews, iNumberOfAddionalConnectorsInOneGroup, out fSequenceLeftTop, out fSequenceLeftBottom, out fSequenceLeftTopRadii, out fSequenceLeftBottomRadii);

                // Right side
                float[,] fSequenceRightTop;
                float[,] fSequenceRightBottom;
                float[] fSequenceRightTopRadii;
                float[] fSequenceRightBottomRadii;
                Get_ScrewGroup_Circle(fx_c2, fy_c2, fRadius, fAngle_seq_rotation_init_point_deg, -fSlope_rad, bUseAdditionalCornerScrews, iNumberOfAddionalConnectorsInOneGroup, out fSequenceRightTop, out fSequenceRightBottom, out fSequenceRightTopRadii, out fSequenceRightBottomRadii);

                // Fill array of holes centers
                for (int i = 0; i < iNumberOfScrewsInOneSequence; i++) // Add all 4 sequences in one cycle
                {
                    HolesCentersPoints2D[i, 0] = fSequenceLeftTop[i, 0];
                    HolesCentersPoints2D[i, 1] = fSequenceLeftTop[i, 1];

                    HolesCentersPoints2D[iNumberOfScrewsInOneSequence + i, 0] = fSequenceLeftBottom[i, 0];
                    HolesCentersPoints2D[iNumberOfScrewsInOneSequence + i, 1] = fSequenceLeftBottom[i, 1];

                    HolesCentersPoints2D[2 * iNumberOfScrewsInOneSequence + i, 0] = fSequenceRightTop[i, 0];
                    HolesCentersPoints2D[2 * iNumberOfScrewsInOneSequence + i, 1] = fSequenceRightTop[i, 1];

                    HolesCentersPoints2D[3 * iNumberOfScrewsInOneSequence + i, 0] = fSequenceRightBottom[i, 0];
                    HolesCentersPoints2D[3 * iNumberOfScrewsInOneSequence + i, 1] = fSequenceRightBottom[i, 1];
                }
            }
            else
            {
                // TODO - zapracovat rozne usporiadanie skrutiek

            }

            // TODO - tempoerary nastavit pre pole suradnic ktore je sucastou plate
            // teoereticky moze mat usporadanie iny pocet ako je pocet na plate, napriklad ak sa usporiadanie odzrkadli alebo skopiruje vramci plochy (napr. Typ KE)
            fHolesCentersPoints2D = HolesCentersPoints2D;
        }

        public void Calc_HolesCentersCoord2D(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float fSlope_rad,
            ref float[,] fHolesCentersPoints2D,
            ref float[] fHolesCenterRadii
    )
        {
            // Bottom Circle (Main Column)
            float fDistanceOfCenterFromLeftEdge = flZ + fbX_1 / 2f;
            float fx_c1 = fDistanceOfCenterFromLeftEdge;
            float fy_c1 = fhY_1 / 4f;

            // Top Circle (Main Rafter)
            float fxInTopMemberAxis = 0.2f * (fbX_2 - fbX_1); // TODO - hodnota je v smere lokalnej osi x prievkalu, je urcena priblizne z vodorovnych rozmerov plechu, do buducna bo bolo dobre pohrat sa s jej urcenim na zaklade sklonu prievkalu a dalsich rozmerov, tak aby spoj nekolidoval s eave purlin a skrutky nevysli mimo plech

            float fx_c2 = fxInTopMemberAxis * (float)Math.Cos(fSlope_rad) + fDistanceOfCenterFromLeftEdge;
            float fy_c2 = fxInTopMemberAxis * (float)Math.Sin(fSlope_rad) + ((fhY_1 + fx_c1 * (float)Math.Atan(fSlope_rad)) - (0.5f * FCrscRafterDepth / (float)Math.Cos(fSlope_rad))); // TODO Dopracovat podla sklonu rafteru

            int iNumberOfAddionalConnectorsInOneGroup = IAdditionalConnectorNumber / INumberOfCircleGroupsInJoint;
            int iNumberOfScrewsInOneSequence = IHolesNumber / (INumberOfCircleGroupsInJoint * INumberOfCircleSequencesInGroup);

            float fAdditionalMargin = 0.01f; // Temp - TODO - put to the input data
            float fRadius = 0.5f * FCrscWebStraightDepth - 2 * fAdditionalMargin; // m // Input - depending on depth of cross-section
            float fAngle_seq_rotation_init_point_deg = (float)(Math.Atan(0.5f * FStiffenerSize / fDistanceOfCenterFromLeftEdge) / MathF.fPI * 180f); // Input - constant for cross-section according to the size of middle sfiffener

            // Left side
            float[,] fSequenceLeftTop;
            float[,] fSequenceLeftBottom;
            float[] fSequenceLeftTopRadii;
            float[] fSequenceLeftBottomRadii;
            Get_ScrewGroup_Circle(fx_c1, fy_c1, fRadius, fAngle_seq_rotation_init_point_deg, MathF.fPI / 2f, BUseAdditionalCornerScrews, iNumberOfAddionalConnectorsInOneGroup, out fSequenceLeftTop, out fSequenceLeftBottom, out fSequenceLeftTopRadii, out fSequenceLeftBottomRadii);

            // Right side
            float[,] fSequenceRightTop;
            float[,] fSequenceRightBottom;
            float[] fSequenceRightTopRadii;
            float[] fSequenceRightBottomRadii;
            Get_ScrewGroup_Circle(fx_c2, fy_c2, fRadius, fAngle_seq_rotation_init_point_deg, fSlope_rad, BUseAdditionalCornerScrews, iNumberOfAddionalConnectorsInOneGroup, out fSequenceRightTop, out fSequenceRightBottom, out fSequenceRightTopRadii, out fSequenceRightBottomRadii);

            // Fill array of holes centers
            for (int i = 0; i < iNumberOfScrewsInOneSequence; i++) // Add all 4 sequences in one cycle
            {
                HolesCentersPoints2D[i, 0] = fSequenceLeftTop[i, 0];
                HolesCentersPoints2D[i, 1] = fSequenceLeftTop[i, 1];

                HolesCentersPoints2D[iNumberOfScrewsInOneSequence + i, 0] = fSequenceLeftBottom[i, 0];
                HolesCentersPoints2D[iNumberOfScrewsInOneSequence + i, 1] = fSequenceLeftBottom[i, 1];

                HolesCentersPoints2D[2 * iNumberOfScrewsInOneSequence + i, 0] = fSequenceRightTop[i, 0];
                HolesCentersPoints2D[2 * iNumberOfScrewsInOneSequence + i, 1] = fSequenceRightTop[i, 1];

                HolesCentersPoints2D[3 * iNumberOfScrewsInOneSequence + i, 0] = fSequenceRightBottom[i, 0];
                HolesCentersPoints2D[3 * iNumberOfScrewsInOneSequence + i, 1] = fSequenceRightBottom[i, 1];

                fHolesCenterRadii[i] = fSequenceLeftTopRadii[i];
                fHolesCenterRadii[iNumberOfScrewsInOneSequence + i] = fSequenceLeftBottomRadii[i];
                fHolesCenterRadii[2 * iNumberOfScrewsInOneSequence + i] = fSequenceRightTopRadii[i];
                fHolesCenterRadii[3 * iNumberOfScrewsInOneSequence + i] = fSequenceRightBottomRadii[i];
            }

            // TODO - tempoerary nastavit pre pole suradnic ktore je sucastou plate
            // teoereticky moze mat usporadanie iny pocet ako je pocet na plate, napriklad ak sa usporiadanie odzrkadli alebo skopiruje vramci plochy (napr. Typ KE)
            fHolesCentersPoints2D = HolesCentersPoints2D;
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
