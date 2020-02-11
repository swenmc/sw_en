using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Globalization;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_O : CScrewArrangement
    {
        private int m_NumberOfGroups;  //default 2
        private int m_NumberOfSequenceInGroup;
        private List<CScrewRectSequence> m_RectSequences;

        //public int iNumberOfScrewsInRow_xDirection_SQ1;
        //public int iNumberOfScrewsInColumn_yDirection_SQ1;
        //public float fx_c_SQ1;
        //public float fy_c_SQ1;
        //public float fDistanceOfPointsX_SQ1;
        //public float fDistanceOfPointsY_SQ1;
        //public int iNumberOfScrewsInRow_xDirection_SQ2;
        //public int iNumberOfScrewsInColumn_yDirection_SQ2;
        //public float fx_c_SQ2;
        //public float fy_c_SQ2;
        //public float fDistanceOfPointsX_SQ2;
        //public float fDistanceOfPointsY_SQ2;

        public int NumberOfGroups
        {
            get
            {
                return m_NumberOfGroups;
            }

            set
            {
                m_NumberOfGroups = value;
            }
        }

        public int NumberOfSequenceInGroup
        {
            get
            {
                return m_NumberOfSequenceInGroup;
            }

            set
            {
                m_NumberOfSequenceInGroup = value;
            }
        }

        public List<CScrewRectSequence> RectSequences
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

        public CScrewArrangement_O() { }

        //public CScrewArrangement_O(
        //    CScrew referenceScrew_temp,
        //    int iNumberOfScrewsInRow_xDirection_SQ1_temp,
        //    int iNumberOfScrewsInColumn_yDirection_SQ1_temp,
        //    float fx_c_SQ1_temp,
        //    float fy_c_SQ1_temp,
        //    float fDistanceOfPointsX_SQ1_temp,
        //    float fDistanceOfPointsY_SQ1_temp,
        //    int iNumberOfScrewsInRow_xDirection_SQ2_temp,
        //    int iNumberOfScrewsInColumn_yDirection_SQ2_temp,
        //    float fx_c_SQ2_temp,
        //    float fy_c_SQ2_temp,
        //    float fDistanceOfPointsX_SQ2_temp,
        //    float fDistanceOfPointsY_SQ2_temp) : base(iNumberOfScrewsInRow_xDirection_SQ1_temp * iNumberOfScrewsInColumn_yDirection_SQ1_temp + iNumberOfScrewsInRow_xDirection_SQ2_temp * iNumberOfScrewsInColumn_yDirection_SQ2_temp, referenceScrew_temp)
        //{
        //    referenceScrew = referenceScrew_temp;

        //    // TODO - docasne - doriesit ako by sa malo zadavat pre lubovolny pocet sekvencii
        //    iNumberOfScrewsInRow_xDirection_SQ1 = iNumberOfScrewsInRow_xDirection_SQ1_temp;
        //    iNumberOfScrewsInColumn_yDirection_SQ1 = iNumberOfScrewsInColumn_yDirection_SQ1_temp;
        //    fx_c_SQ1 = fx_c_SQ1_temp;
        //    fy_c_SQ1 = fy_c_SQ1_temp;
        //    fDistanceOfPointsX_SQ1 = fDistanceOfPointsX_SQ1_temp;
        //    fDistanceOfPointsY_SQ1 = fDistanceOfPointsY_SQ1_temp;

        //    iNumberOfScrewsInRow_xDirection_SQ2 = iNumberOfScrewsInRow_xDirection_SQ2_temp;
        //    iNumberOfScrewsInColumn_yDirection_SQ2 = iNumberOfScrewsInColumn_yDirection_SQ2_temp;
        //    fx_c_SQ2 = fx_c_SQ2_temp;
        //    fy_c_SQ2 = fy_c_SQ2_temp;
        //    fDistanceOfPointsX_SQ2 = fDistanceOfPointsX_SQ2_temp;
        //    fDistanceOfPointsY_SQ2 = fDistanceOfPointsY_SQ2_temp;

        //    ListOfSequenceGroups = new List<CScrewSequenceGroup>(1); // Two groups, each for the connection of one member in joint

        //    UpdateArrangmentData();
        //}

        public CScrewArrangement_O(
            CScrew referenceScrew_temp,
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
            float fDistanceOfPointsY_SQ2_temp)
        {
            referenceScrew = referenceScrew_temp;

            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ2_temp, iNumberOfScrewsInColumn_yDirection_SQ2_temp, fx_c_SQ2_temp, fy_c_SQ2_temp, fDistanceOfPointsX_SQ2_temp, fDistanceOfPointsY_SQ2_temp));

            NumberOfGroups = 1;
            NumberOfSequenceInGroup = 2;

            IHolesNumber = 0;
            foreach (CScrewRectSequence rectS in RectSequences)
            {
                IHolesNumber += rectS.INumberOfConnectors;
            }

            UpdateArrangmentData();
        }

        public CScrewArrangement_O(
            CScrew referenceScrew_temp,
            List<CScrewRectSequence> listRectSequences)
        {
            referenceScrew = referenceScrew_temp;

            IHolesNumber = 0;
            foreach (CScrewRectSequence rectS in listRectSequences)
            {
                IHolesNumber += rectS.INumberOfConnectors;
            }

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

            ListOfSequenceGroups = new List<CScrewSequenceGroup>(NumberOfGroups);
            int index = 0;
            for (int i = 0; i < NumberOfGroups; i++)
            {
                CScrewSequenceGroup gr = new CScrewSequenceGroup();
                gr.NumberOfHalfCircleSequences = 0;
                gr.NumberOfRectangularSequences = NumberOfSequenceInGroup;
                for (int j = 0; j < NumberOfSequenceInGroup; j++)
                {
                    gr.ListSequence.Add(RectSequences[index]);
                    index++;
                }
                ListOfSequenceGroups.Add(gr);
            }
            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            RecalculateTotalNumberOfScrews();

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        //public override void UpdateArrangmentData()
        //{
        //    // TODO - toto prerobit tak ze sa parametre prevedu na cisla a nastavia v CTEKScrewsManager a nie tu
        //    NumberFormatInfo nfi = new NumberFormatInfo();
        //    nfi.NumberDecimalSeparator = ".";

        //    // Update reference screw properties
        //    DATABASE.DTO.CTEKScrewProperties screwProp = DATABASE.CTEKScrewsManager.GetScrewProperties(referenceScrew.Gauge.ToString());
        //    referenceScrew.Diameter_thread = float.Parse(screwProp.threadDiameter, nfi) / 1000; // Convert mm to m

        //    ListOfSequenceGroups.Clear(); // Delete previous data otherwise are added more and more new screws to the list
        //    ListOfSequenceGroups = new List<CScrewSequenceGroup>(2);
        //    ListOfSequenceGroups.Add(new CScrewSequenceGroup());

        //    CScrewRectSequence seq1 = new CScrewRectSequence();
        //    seq1.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ1;
        //    seq1.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ1;
        //    seq1.ReferencePoint = new Point(fx_c_SQ1, fy_c_SQ1);
        //    seq1.DistanceOfPointsX = fDistanceOfPointsX_SQ1;
        //    seq1.DistanceOfPointsY = fDistanceOfPointsY_SQ1;
        //    seq1.INumberOfConnectors = seq1.NumberOfScrewsInRow_xDirection * seq1.NumberOfScrewsInColumn_yDirection;
        //    seq1.HolesCentersPoints = new Point[seq1.INumberOfConnectors];
        //    ListOfSequenceGroups[0].ListSequence.Add(seq1);

        //    CScrewRectSequence seq2 = new CScrewRectSequence();
        //    seq2.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ2;
        //    seq2.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ2;
        //    seq2.ReferencePoint = new Point(fx_c_SQ2, fy_c_SQ2);
        //    seq2.DistanceOfPointsX = fDistanceOfPointsX_SQ2;
        //    seq2.DistanceOfPointsY = fDistanceOfPointsY_SQ2;
        //    seq2.INumberOfConnectors = seq2.NumberOfScrewsInRow_xDirection * seq2.NumberOfScrewsInColumn_yDirection;
        //    seq2.HolesCentersPoints = new Point[seq2.INumberOfConnectors];
        //    ListOfSequenceGroups[0].ListSequence.Add(seq2);

        //    ListOfSequenceGroups[0].NumberOfHalfCircleSequences = 0;
        //    ListOfSequenceGroups[0].NumberOfRectangularSequences = 2;

        //    // Celkovy pocet skrutiek
        //    // Definovane su len sekvencie v jednej group
        //    RecalculateTotalNumberOfScrews();
        //    int iNumberOfGroupsInPlate = 1;
        //    IHolesNumber *= iNumberOfGroupsInPlate;

        //    HolesCentersPoints2D = new Point[IHolesNumber];
        //    arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        //}

        public Point[] Get_ScrewSequencePointCoordinates(CScrewRectSequence srectSeq)
        {
            // Connectors in Sequence
            return GetRegularArrayOfPointsInCartesianCoordinates(srectSeq.ReferencePoint, srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
        }

        public override void Calc_HolesCentersCoord2DFacePlate(
        float fbX_1,
        float fbX_2,
        float fhY_1)
        {
            // Coordinates of [0,0] of sequence point on plate (used to translate all sequences in the group)
            //float fx_c = 0.00f;
            //float fy_c = 0.00f;

            // Left side
            ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            ListOfSequenceGroups[0].ListSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);
            // Set radii of connectors / screws in the group
            ListOfSequenceGroups[0].HolesRadii = ListOfSequenceGroups[0].Get_RadiiOfConnectorsInGroup();

            // Rotate screws
            // Rotate about [0,0]
            //RotateSequence_CCW_rad(0, 0, 0, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            //RotateSequence_CCW_rad(0, 0, 0, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);

            // Translate from [0,0] on plate to the final position
            //TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            //TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);

            FillArrayOfHolesCentersInWholeArrangement();
        }

        public override void Calc_FacePlateData(
        float fbX_1,
        float fbX_2,
        float fhY_1,
        float ft)
        {
            Calc_HolesCentersCoord2DFacePlate(fbX_1, fbX_2, fhY_1);
            Calc_HolesControlPointsCoord3D_FlatPlate(0, 0, ft, false);
            GenerateConnectors_FlatPlate(false);
        }
    }
}
