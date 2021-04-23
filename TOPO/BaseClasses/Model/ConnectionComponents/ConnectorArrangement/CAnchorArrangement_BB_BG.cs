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
        public int iNumberOfAnchorsInRow_xDirection_SQ1;
        public int iNumberOfAnchorsInColumn_yDirection_SQ1;
        //public float fx_c_SQ1;
        //public float fy_c_SQ1;
        public List<float> fDistanceOfPointsX_SQ1;
        public List<float> fDistanceOfPointsY_SQ1;

        private List<CAnchorRectSequence> m_RectSequences;



        private bool m_UniformDistributionOfShear;

        public bool UniformDistributionOfShear
        {
            get
            {
                return m_UniformDistributionOfShear;
            }

            set
            {
                m_UniformDistributionOfShear = value;
            }
        }

        public List<CAnchorRectSequence> RectSequences
        {
            get
            {
                return m_RectSequences;
            }

            set
            {
                m_RectSequences = value;
            }
        }

        public CAnchorArrangement_BB_BG() { }

        public CAnchorArrangement_BB_BG(string plateName_temp, CAnchor referenceAnchor_temp, bool uniformDistributionOfShear)
        {
            m_UniformDistributionOfShear = uniformDistributionOfShear; // TODO by malo prist z nastavenia Design Options

            CPlate_B_Properties prop = CJointsManager.GetPlate_B_Properties(plateName_temp);

            // TODO - nacitat z databazy parametre

            // Anchor arrangement parameters
            IHolesNumber = prop.iNumberHolesAnchors;
            NumberOfAnchorsInYDirection = prop.iNoOfAnchorsInRow;
            NumberOfAnchorsInZDirection = prop.iNoOfAnchorsInColumn;

            // Parametre sekvencie
            iNumberOfAnchorsInRow_xDirection_SQ1 = NumberOfAnchorsInYDirection;
            iNumberOfAnchorsInColumn_yDirection_SQ1 = NumberOfAnchorsInZDirection;
            //fx_c_SQ1 = (float)prop.a1_pos_cp_x;
            //fy_c_SQ1 = (float)prop.a1_pos_cp_y;
            //m_RefPointX = prop.a1_pos_cp_x;
            //m_RefPointY = prop.a1_pos_cp_y;

            float dist_x1 = (float)prop.dist_x1;
            float dist_y1 = (float)prop.dist_y1;
            float dist_x2 = (float)prop.dist_x2;
            float dist_y2 = (float)prop.dist_y2;
            float dist_x3 = (float)prop.dist_x3;
            float dist_y3 = (float)prop.dist_y3;

            if (float.IsNaN(dist_x2) || float.IsNaN(dist_y2))
            {
                if (MathF.d_equal(dist_x1, 0f)) dist_x1 = 0.08f;
                if (MathF.d_equal(dist_y1, 0f)) dist_y1 = 0.08f;

                fDistanceOfPointsX_SQ1 = new List<float> { dist_x1 };
                fDistanceOfPointsY_SQ1 = new List<float> { dist_y1 };
            }
            else if (float.IsNaN(dist_x3) || float.IsNaN(dist_y3))
            {
                fDistanceOfPointsX_SQ1 = new List<float> { dist_x1, dist_x2 };
                fDistanceOfPointsY_SQ1 = new List<float> { dist_y1, dist_y2 };
            }
            else
            {
                fDistanceOfPointsX_SQ1 = new List<float> { dist_x1, dist_x2, dist_x3 };
                fDistanceOfPointsY_SQ1 = new List<float> { dist_y1, dist_y2, dist_y3 };
            }

            referenceAnchor = referenceAnchor_temp;
            HoleRadius = 0.5f * referenceAnchor.Diameter_thread; // Anchor diameter
            RadiusAngle = 360; // Circle total angle to generate holes

            CAnchorRectSequence seq1 = new CAnchorRectSequence();
            seq1.RefPointX = prop.a1_pos_cp_x;
            seq1.RefPointY = prop.a1_pos_cp_y;
            seq1.NumberOfAnchorsInRow_xDirection = iNumberOfAnchorsInRow_xDirection_SQ1;
            seq1.NumberOfAnchorsInColumn_yDirection = iNumberOfAnchorsInColumn_yDirection_SQ1;
            //seq1.ReferencePoint = new Point(fx_c_SQ1, fy_c_SQ1);
            //if (fDistanceOfPointsX_SQ1.Count > 1)
            if (fDistanceOfPointsX_SQ1.Count > 1 && seq1.NumberOfAnchorsInRow_xDirection > 1)
            {
                seq1.SameDistancesX = false;
                seq1.DistancesOfPointsX = fDistanceOfPointsX_SQ1;
            }
            else
            {
                seq1.SameDistancesX = true;
                seq1.DistanceOfPointsX = fDistanceOfPointsX_SQ1.First();
            }

            //if (fDistanceOfPointsY_SQ1.Count > 1)
            if (fDistanceOfPointsY_SQ1.Count > 1 && seq1.NumberOfAnchorsInColumn_yDirection > 1)
            {
                seq1.SameDistancesY = false;
                seq1.DistancesOfPointsY = fDistanceOfPointsY_SQ1;
            }
            else
            {
                seq1.SameDistancesY = true;
                seq1.DistanceOfPointsY = fDistanceOfPointsY_SQ1.First();
            }

            seq1.INumberOfConnectors = seq1.NumberOfAnchorsInRow_xDirection * seq1.NumberOfAnchorsInColumn_yDirection;
            seq1.HolesCentersPoints = new Point[seq1.INumberOfConnectors];
            RectSequences = new List<CAnchorRectSequence>() { seq1 };

            UpdateArrangmentData();
        }

        public override void UpdateArrangmentData()
        {
            ListOfSequenceGroups = new List<CAnchorSequenceGroup>(1);
            ListOfSequenceGroups.Add(new CAnchorSequenceGroup());

            ListOfSequenceGroups[0].ListSequence.Add(RectSequences.First());

            ListOfSequenceGroups[0].NumberOfRectangularSequences = 1;

            // Celkovy pocet kotiev
            // Definovane su len sekvencie v jednej group, ocakava sa ze pocet v groups je rovnaky a hodnoty sa skopiruju
            RecalculateTotalNumberOfAnchors();
            int iNumberOfGroupsInPlate = 1;
            IHolesNumber *= iNumberOfGroupsInPlate;

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        //public Point[] Get_AnchorSequencePointCoordinates(CAnchorRectSequence srectSeq)
        //{
        //    // Connectors in Sequence
        //    return GetRegularArrayOfPointsInCartesianCoordinates(new Point(srectSeq.RefPointX, srectSeq.RefPointY) /*srectSeq.ReferencePoint*/, srectSeq.NumberOfAnchorsInRow_xDirection, srectSeq.NumberOfAnchorsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
        //}
        public Point[] Get_AnchorSequencePointCoordinates(CAnchorRectSequence srectSeq)
        {
            // Connectors in Sequence
            if (srectSeq.SameDistancesX && srectSeq.SameDistancesY) // Ak su pre oba smery vzdialenosti skrutiek rovnake, posielame do konstruktora len jedno cislo pre rozostup (vzdialenost) skrutiek
                return GetRegularArrayOfPointsInCartesianCoordinates(new Point(srectSeq.RefPointX, srectSeq.RefPointY), srectSeq.NumberOfAnchorsInRow_xDirection, srectSeq.NumberOfAnchorsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
            else // Ak su aspon pre jeden smer vzdialenosti skrutiek rozdielne, posielame do konstruktora zoznam rozostupov (rozne vzdialenosti) skrutiek
                return GetRegularArrayOfPointsInCartesianCoordinates(new Point(srectSeq.RefPointX, srectSeq.RefPointY), srectSeq.NumberOfAnchorsInRow_xDirection, srectSeq.NumberOfAnchorsInColumn_yDirection, srectSeq.DistancesOfPointsX.ToArray(), srectSeq.DistancesOfPointsY.ToArray());
        }


        public override void Calc_HolesCentersCoord2D(float fbX, float fhY, float flZ)
        {
            ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints = Get_AnchorSequencePointCoordinates((CAnchorRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

            // Set radii of connectors / screws in the group
            ListOfSequenceGroups[0].HolesRadii = ListOfSequenceGroups[0].Get_RadiiOfConnectorsInGroup();

            // Translate from [0,0] on plate to the final position
            TranslateSequence(flZ + 0, 0, (CAnchorRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

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

        public void GenerateAnchors_BasePlate()
        {
            Anchors = new CAnchor[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);

                // Preberaju sa parametre z referencneho objektu anchor
                Anchors[i] = new CAnchor(referenceAnchor.Name, referenceAnchor.m_Mat.Name, controlpoint, referenceAnchor.Length, referenceAnchor.h_effective, referenceAnchor.PortionOtAnchorAbovePlate_abs, referenceAnchor.WasherPlateTop, referenceAnchor.WasherBearing, 0, 90, 0, true);
            }
        }
    }
}
