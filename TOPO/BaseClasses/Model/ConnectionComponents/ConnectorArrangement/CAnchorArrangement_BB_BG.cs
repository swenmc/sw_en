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
    [Serializable]
    public class CAnchorArrangement_BB_BG : CAnchorArrangement_Rectangular
    {
        private float m_fDistanceBetweenHoles;

        public float DistanceBetweenHoles
        {
            get
            {
                return m_fDistanceBetweenHoles;
            }

            set
            {
                m_fDistanceBetweenHoles = value;
            }
        }

        // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii
        public int iNumberOfScrewsInRow_xDirection_SQ1;
        public int iNumberOfScrewsInColumn_yDirection_SQ1;
        public float fx_c_SQ1;
        public float fy_c_SQ1;
        public float fDistanceOfPointsX_SQ1;
        public float fDistanceOfPointsY_SQ1;

        public CAnchorArrangement_BB_BG() { }

        public CAnchorArrangement_BB_BG(float fDistanceBetweenHoles_temp, CAnchor referenceAnchor_temp)
        {
            IHolesNumber = 2; // 2 Otvory
            NumberOfAnchorsInYDirection = 1;
            NumberOfAnchorsInZDirection = 2;
            DistanceBetweenHoles = fDistanceBetweenHoles_temp;
            referenceAnchor = referenceAnchor_temp;
            HoleRadius = 0.5f * referenceAnchor.Diameter_thread; // Anchor diameter
            RadiusAngle = 360; // Circle total angle to generate holes

            // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii
            iNumberOfScrewsInRow_xDirection_SQ1 = NumberOfAnchorsInYDirection;
            iNumberOfScrewsInColumn_yDirection_SQ1 = NumberOfAnchorsInZDirection;
            fx_c_SQ1 = 0; //???
            fy_c_SQ1 = 0; //???
            fDistanceOfPointsX_SQ1 = 0;
            fDistanceOfPointsY_SQ1 = DistanceBetweenHoles;

            ListOfSequenceGroups = new List<CAnchorSequenceGroup>(1); // One group

            UpdateArrangmentData();
        }

        public override void UpdateArrangmentData()
        {
            // TODO - toto prerobit tak ze sa parametre prevedu na cisla a nastavia v CAnchorsManager a nie tu
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            // Update reference anchor properties
            //DATABASE.DTO.CTEKScrewProperties screwProp = DATABASE.CTEKScrewsManager.GetScrewProperties(referenceScrew.Gauge.ToString());
            //referenceScrew.Diameter_thread = float.Parse(screwProp.threadDiameter, nfi) / 1000; // Convert mm to m

            ListOfSequenceGroups.Clear(); // Delete previous data otherwise are added more and more new screws to the list
            ListOfSequenceGroups = new List<CAnchorSequenceGroup>(1);
            ListOfSequenceGroups.Add(new CAnchorSequenceGroup());

            CAnchorRectSequence seq1 = new CAnchorRectSequence();
            seq1.NumberOfAnchorsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ1;
            seq1.NumberOfAnchorsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ1;
            seq1.ReferencePoint = new Point(fx_c_SQ1, fy_c_SQ1);
            seq1.DistanceOfPointsX = fDistanceOfPointsX_SQ1;
            seq1.DistanceOfPointsY = fDistanceOfPointsY_SQ1;
            seq1.INumberOfConnectors = seq1.NumberOfAnchorsInRow_xDirection * seq1.NumberOfAnchorsInColumn_yDirection;
            seq1.HolesCentersPoints = new Point[seq1.INumberOfConnectors];
            ListOfSequenceGroups[0].ListSequence.Add(seq1);

            ListOfSequenceGroups[0].NumberOfRectangularSequences = 1;

            // Celkovy pocet kotiev
            // Definovane su len sekvencie v jednej group, ocakava sa ze pocet v groups je rovnaky a hodnoty sa skopiruju
            RecalculateTotalNumberOfAnchors();
            int iNumberOfGroupsInPlate = 1;
            IHolesNumber *= iNumberOfGroupsInPlate;

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public Point[] Get_AnchorSequencePointCoordinates(CAnchorRectSequence srectSeq)
        {
            // Connectors in Sequence
            return GetRegularArrayOfPointsInCartesianCoordinates(srectSeq.ReferencePoint, srectSeq.NumberOfAnchorsInRow_xDirection, srectSeq.NumberOfAnchorsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
        }

        public override void Calc_HolesCentersCoord2D(float fbX, float fhY, float flZ)
        {
            if(DistanceBetweenHoles<=0) // Ak uz nebola definovana, nastavit vzdialenost - vylepsit
                fDistanceOfPointsY_SQ1 = DistanceBetweenHoles = 0.5f * fhY; // Default

            //HolesCentersPoints2D = new Point[IHolesNumber];
            //HolesCentersPoints2D[0] = new Point(flZ + 0.5f * fbX, 0.5f * fhY - 0.5f * DistanceBetweenHoles);
            //HolesCentersPoints2D[1] = new Point(HolesCentersPoints2D[0].X, 0.5f * fhY + 0.5f * DistanceBetweenHoles);

            // Coordinates of [0,0] of sequence point on plate (used to translate all sequences in the group)
            float fx_c = flZ + 0.5f * fbX;
            float fy_c = 0.5f * fhY - 0.5f * DistanceBetweenHoles;

            ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints = Get_AnchorSequencePointCoordinates((CAnchorRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

            // Set radii of connectors / screws in the group
            ListOfSequenceGroups[0].HolesRadii = ListOfSequenceGroups[0].Get_RadiiOfConnectorsInGroup();

            // Translate from [0,0] on plate to the final position
            TranslateSequence(fx_c, fy_c, (CAnchorRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

            FillArrayOfHolesCentersInWholeArrangement();
        }

        void Calc_HolesControlPointsCoord3D(float fbX, float flZ, float ft)
        {
            float fPortionOtAnchorAbovePlate_rel = 0.2f; // [-] // Suradnica konca kotvy nad plechom (maximum z 20% dlzky kotvy alebo 30 mm)
            float fPortionOtAnchorAbovePlate_abs = Math.Max(fPortionOtAnchorAbovePlate_rel * referenceAnchor.Length, 0.03f); // [m]

            int iGroupIndex = 0;
            int iLastItemIndex = 0;

            for (int i = 0; i < ListOfSequenceGroups[iGroupIndex].ListSequence.Count; i++)
            {
                for (int j = 0; j < ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length; j++)
                {
                    arrConnectorControlPoints3D[iLastItemIndex + j].X = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].X - flZ;
                    arrConnectorControlPoints3D[iLastItemIndex + j].Y = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].Y;
                    arrConnectorControlPoints3D[iLastItemIndex + j].Z = fPortionOtAnchorAbovePlate_abs;
                }

                iLastItemIndex += ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length;
            }

            // Test - plate BA
            //float fhY = 0.27f;
            //arrConnectorControlPoints3D[0] = new Point3D(0.5f * fbX, 0.5f * fhY - 0.5f * DistanceBetweenHoles, fPortionOtAnchorAbovePlate_abs);
            //arrConnectorControlPoints3D[1] = new Point3D(0.5f * fbX, 0.5f * fhY + 0.5f * DistanceBetweenHoles, fPortionOtAnchorAbovePlate_abs);
        }

        public override void Calc_BasePlateData(
        float fbX,
        float flZ,
        float fhY,
        float ft)
        {
            Calc_HolesCentersCoord2D(fbX, fhY, flZ);
            Calc_HolesCentersCoord3D(fbX, fhY, flZ);
            Calc_HolesControlPointsCoord3D(fbX, flZ, ft);
            GenerateAnchors_BasePlate();
        }

        public void Calc_HolesCentersCoord3D(float fbX, float fhY, float flZ)
        {
            holesCentersPointsfor3D = new Point[IHolesNumber];
            holesCentersPointsfor3D[0] = new Point(0.5f * fbX, 0.5f * fhY - 0.5f * DistanceBetweenHoles);
            holesCentersPointsfor3D[1] = new Point(0.5f * fbX, 0.5f * fhY + 0.5f * DistanceBetweenHoles);
        }

        void GenerateAnchors_BasePlate()
        {
            Anchors = new CAnchor[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                CPoint controlpoint = new CPoint(0, arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z, 0);

                Anchors[i] = new CAnchor(referenceAnchor.Name, controlpoint, referenceAnchor.Diameter_shank, referenceAnchor.Diameter_thread, referenceAnchor.Length, referenceAnchor.Mass, 0, 90 , 0, true);
            }
        }
    }
}
