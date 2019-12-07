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
using DATABASE;
using DATABASE.DTO;

namespace BaseClasses
{
    [Serializable]
    public class CAnchorArrangement_BB_BG : CAnchorArrangement_Rectangular
    {
        // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii
        public int iNumberOfScrewsInRow_xDirection_SQ1;
        public int iNumberOfScrewsInColumn_yDirection_SQ1;
        public float fx_c_SQ1;
        public float fy_c_SQ1;
        public float[] fDistanceOfPointsX_SQ1;
        public float[] fDistanceOfPointsY_SQ1;

        public CAnchorArrangement_BB_BG() { }

        public CAnchorArrangement_BB_BG(string plateName_temp, CAnchor referenceAnchor_temp)
        {
            CPlate_B_Properties prop = CJointsManager.GetPlate_B_Properties(plateName_temp);

            // TODO - nacitat z databazy parametre

            // Anchor arrangement parameters
            IHolesNumber = prop.iNumberHolesAnchors;
            NumberOfAnchorsInYDirection = prop.iNoOfAnchorsInRow;
            NumberOfAnchorsInZDirection = prop.iNoOfAnchorsInColumn;

            // Parametre sekvencie
            iNumberOfScrewsInRow_xDirection_SQ1 = NumberOfAnchorsInYDirection;
            iNumberOfScrewsInColumn_yDirection_SQ1 = NumberOfAnchorsInZDirection;
            fx_c_SQ1 = (float)prop.a1_pos_cp_x;
            fy_c_SQ1 = (float)prop.a1_pos_cp_y;
            float dist_x1 = (float)prop.dist_x1;
            float dist_y1 = (float)prop.dist_y1;
            float dist_x2 = (float)prop.dist_x2;
            float dist_y2 = (float)prop.dist_y2;

            if (float.IsNaN(dist_x2) || float.IsNaN(dist_y2))
            {
                fDistanceOfPointsX_SQ1 = new float[1] { dist_x1 };
                fDistanceOfPointsY_SQ1 = new float[1] { dist_y1 };
            }
            else
            {
                fDistanceOfPointsX_SQ1 = new float[2] { dist_x1, dist_x2 };
                fDistanceOfPointsY_SQ1 = new float[2] { dist_y1, dist_y2 };
            }

            referenceAnchor = referenceAnchor_temp;
            HoleRadius = 0.5f * referenceAnchor.Diameter_thread; // Anchor diameter
            RadiusAngle = 360; // Circle total angle to generate holes

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
            ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints = Get_AnchorSequencePointCoordinates((CAnchorRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

            // Set radii of connectors / screws in the group
            ListOfSequenceGroups[0].HolesRadii = ListOfSequenceGroups[0].Get_RadiiOfConnectorsInGroup();

            // Translate from [0,0] on plate to the final position
            TranslateSequence(flZ + 0,  0, (CAnchorRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

            FillArrayOfHolesCentersInWholeArrangement();
        }

        void Calc_HolesControlPointsCoord3D(float fbX, float flZ, float ft)
        {
            int iGroupIndex = 0;
            int iLastItemIndex = 0;

            for (int i = 0; i < ListOfSequenceGroups[iGroupIndex].ListSequence.Count; i++)
            {
                for (int j = 0; j < ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length; j++)
                {
                    arrConnectorControlPoints3D[iLastItemIndex + j].X = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].X - flZ + ft;
                    arrConnectorControlPoints3D[iLastItemIndex + j].Y = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].Y;
                    arrConnectorControlPoints3D[iLastItemIndex + j].Z = referenceAnchor.PortionOtAnchorAbovePlate_abs;
                }

                iLastItemIndex += ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length;
            }
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

            // Skopirovat polozky a odratat flZ
            for (int i = 0; i < IHolesNumber; i++)
            {
                holesCentersPointsfor3D[i].X = ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints[i].X - flZ;
                holesCentersPointsfor3D[i].Y = ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints[i].Y;
            }
        }

        void GenerateAnchors_BasePlate()
        {
            Anchors = new CAnchor[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);

                Anchors[i] = new CAnchor(referenceAnchor.Name, referenceAnchor.m_Mat.Name, controlpoint, referenceAnchor.Length, referenceAnchor.h_effective, referenceAnchor.WasherPlateTop, referenceAnchor.WasherBearing, 0, 90 , 0, true);
            }
        }
    }
}
