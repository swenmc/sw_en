using System;
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
    public class CScrewArrangementRect_PlateType_JKL : CScrewArrangement
    {
        // TODO - Ondrej, TODO No. 105
        // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
        // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
        // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

        private int m_NumberOfGroups;  //default 2
        private int m_NumberOfSequenceInGroup;
        private List<CScrewRectSequence> m_RectSequences;
        private bool m_MirroredGroups;
        private bool m_IsApex;

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

        public bool MirroredGroups
        {
            get
            {
                return m_MirroredGroups;
            }

            set
            {
                m_MirroredGroups = value;
            }
        }

        public bool IsApex
        {
            get
            {
                return m_IsApex;
            }

            set
            {
                m_IsApex = value;
            }
        }

        public CScrewArrangementRect_PlateType_JKL()
        { }

        // Jedna sekvencia s rovnakymi vzdialenostami na jednej strane plate
        // Apex
        public CScrewArrangementRect_PlateType_JKL(
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            int iNumberOfScrewsInRow_xDirection_SQ1_temp,
            int iNumberOfScrewsInColumn_yDirection_SQ1_temp,
            float fx_c_SQ1_temp,
            float fy_c_SQ1_temp,
            float fDistanceOfPointsX_SQ1_temp,
            float fDistanceOfPointsY_SQ1_temp) : base(iNumberOfScrewsInRow_xDirection_SQ1_temp * iNumberOfScrewsInColumn_yDirection_SQ1_temp, referenceScrew_temp)
        {
            referenceScrew = referenceScrew_temp;
            FCrscRafterDepth = fCrscRafterDepth_temp;
            FCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            FStiffenerSize = fStiffenerSize_temp;
            m_IsApex = true;
            m_MirroredGroups = true;

            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            //RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));

            NumberOfGroups = 2;
            NumberOfSequenceInGroup = 1;

            IHolesNumber = 0;
            foreach (CScrewRectSequence rectS in RectSequences)
            {
                IHolesNumber += rectS.INumberOfConnectors;
            }
            if (MirroredGroups) IHolesNumber = IHolesNumber * 2;

            UpdateArrangmentData();
        }

        // Apex - 2 groups a v kazdej 2 nezavisle sekvencie (vyuzijeme mirror)
        // Knee - 2 groups a kazdej 1 sekvencia

        public CScrewArrangementRect_PlateType_JKL(
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
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
            bool bIsApex/* = true*/)
        {
            referenceScrew = referenceScrew_temp;
            FCrscRafterDepth = fCrscRafterDepth_temp;
            FCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            FStiffenerSize = fStiffenerSize_temp;
            m_IsApex = bIsApex;
            m_MirroredGroups = m_IsApex;

            // TODO - povodne sa tento konstruktor pouzival len pre apex takze 1 a 3 a 2 a 4 sekvencia boli rovnake
            // Chcem ho vsak pouzivat aj pre knee plates pre pripad ze jedna group obsahuje len jednu sekvenciu, takze budu 2 skupiny a v kazdej jedna sekvencia
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------

            // TO Ondrej - zaviest funkcionalitu podla _BX arrangement pre MirroredGroups a potom tuto cast odkomentovat a nahradit nou riadky, ktore su vyssie
            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ2_temp, iNumberOfScrewsInColumn_yDirection_SQ2_temp, fx_c_SQ2_temp, fy_c_SQ2_temp, fDistanceOfPointsX_SQ2_temp, fDistanceOfPointsY_SQ2_temp));

            NumberOfGroups = 2;

            if (m_IsApex)
            {
                NumberOfSequenceInGroup = 2; // Pre Apex 2 sekvencie v jednej group
            }
            else
            {
                NumberOfSequenceInGroup = 1; // Pre Knee jedna sekvencia v jednej group
            }
            
            IHolesNumber = 0;
            foreach (CScrewRectSequence rectS in RectSequences)
            {
                IHolesNumber += rectS.INumberOfConnectors;
            }
            if (MirroredGroups) IHolesNumber = IHolesNumber * 2;
            UpdateArrangmentData();
        }

        // Knee
        public CScrewArrangementRect_PlateType_JKL(
            CScrew referenceScrew_temp,
            float fCrscRafterDepth_temp,
            float fCrscWebStraightDepth_temp,
            float fStiffenerSize_temp,
            int iNumberOfScrewsInRow_xDirection_G1_SQ1_temp,  // Bottom group of knee plate - G1 - SQ1
            int iNumberOfScrewsInColumn_yDirection_G1_SQ1_temp,
            float fx_c_SQ1_temp,
            float fy_c_SQ1_temp,
            float fDistanceOfPointsX_SQ1_temp,
            float fDistanceOfPointsY_SQ1_temp,
            int iNumberOfScrewsInRow_xDirection_G1_SQ2_temp,  // Bottom group of knee plate - G1 - SQ2
            int iNumberOfScrewsInColumn_yDirection_G1_SQ2_temp,
            float fx_c_SQ2_temp,
            float fy_c_SQ2_temp,
            float fDistanceOfPointsX_SQ2_temp,
            float fDistanceOfPointsY_SQ2_temp,
            int iNumberOfScrewsInRow_xDirection_G2_SQ3_temp,  // Upper group of knee plate - G2 - SQ3
            int iNumberOfScrewsInColumn_yDirection_G2_SQ3_temp,
            float fx_c_SQ3_temp,
            float fy_c_SQ3_temp,
            float fDistanceOfPointsX_SQ3_temp,
            float fDistanceOfPointsY_SQ3_temp,
            int iNumberOfScrewsInRow_xDirection_G2_SQ4_temp,  // Upper group of knee plate - G2 - SQ4
            int iNumberOfScrewsInColumn_yDirection_G2_SQ4_temp,
            float fx_c_SQ4_temp,
            float fy_c_SQ4_temp,
            float fDistanceOfPointsX_SQ4_temp,
            float fDistanceOfPointsY_SQ4_temp)
        {
            referenceScrew = referenceScrew_temp;
            FCrscRafterDepth = fCrscRafterDepth_temp;
            FCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            FStiffenerSize = fStiffenerSize_temp;
            m_IsApex = false;
            m_MirroredGroups = false;

            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_G1_SQ1_temp, iNumberOfScrewsInColumn_yDirection_G1_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_G1_SQ2_temp, iNumberOfScrewsInColumn_yDirection_G1_SQ2_temp, fx_c_SQ2_temp, fy_c_SQ2_temp, fDistanceOfPointsX_SQ2_temp, fDistanceOfPointsY_SQ2_temp));
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_G2_SQ3_temp, iNumberOfScrewsInColumn_yDirection_G2_SQ3_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_G2_SQ4_temp, iNumberOfScrewsInColumn_yDirection_G2_SQ4_temp, fx_c_SQ2_temp, fy_c_SQ2_temp, fDistanceOfPointsX_SQ2_temp, fDistanceOfPointsY_SQ2_temp));

            NumberOfGroups = 2;
            NumberOfSequenceInGroup = 2;

            IHolesNumber = 0;
            foreach (CScrewRectSequence rectS in RectSequences)
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
                    // Pridame sekvenciu do skupiny
                    if (MirroredGroups)
                    {
                        if (i == 0) gr.ListSequence.Add(RectSequences[j]);
                        else gr.ListSequence.Add(RectSequences[j].Copy());
                    }
                    else
                    {
                        gr.ListSequence.Add(RectSequences[index]);
                        index++;
                    }
                }
                ListOfSequenceGroups.Add(gr);
            }
            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            RecalculateTotalNumberOfScrews();

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        //uchovane povodne neskor treba zmazat
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

        //    if (iNumberOfScrewsInRow_xDirection_SQ3 != 0 && iNumberOfScrewsInColumn_yDirection_SQ3 != 0 &&
        //        iNumberOfScrewsInRow_xDirection_SQ4 != 0 && iNumberOfScrewsInColumn_yDirection_SQ4 != 0)
        //    {
        //        ListOfSequenceGroups.Add(new CScrewSequenceGroup());

        //        CScrewRectSequence seq3 = new CScrewRectSequence();
        //        seq3.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ3;
        //        seq3.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ3;
        //        seq3.ReferencePoint = new Point(fx_c_SQ3, fy_c_SQ3);
        //        seq3.DistanceOfPointsX = fDistanceOfPointsX_SQ3;
        //        seq3.DistanceOfPointsY = fDistanceOfPointsY_SQ3;
        //        seq3.INumberOfConnectors = seq3.NumberOfScrewsInRow_xDirection * seq3.NumberOfScrewsInColumn_yDirection;
        //        seq3.HolesCentersPoints = new Point[seq3.INumberOfConnectors];
        //        ListOfSequenceGroups[1].ListSequence.Add(seq3);

        //        CScrewRectSequence seq4 = new CScrewRectSequence();
        //        seq4.NumberOfScrewsInRow_xDirection = iNumberOfScrewsInRow_xDirection_SQ4;
        //        seq4.NumberOfScrewsInColumn_yDirection = iNumberOfScrewsInColumn_yDirection_SQ4;
        //        seq4.ReferencePoint = new Point(fx_c_SQ4, fy_c_SQ4);
        //        seq4.DistanceOfPointsX = fDistanceOfPointsX_SQ4;
        //        seq4.DistanceOfPointsY = fDistanceOfPointsY_SQ4;
        //        seq4.INumberOfConnectors = seq4.NumberOfScrewsInRow_xDirection * seq4.NumberOfScrewsInColumn_yDirection;
        //        seq4.HolesCentersPoints = new Point[seq4.INumberOfConnectors];
        //        ListOfSequenceGroups[1].ListSequence.Add(seq4);

        //        ListOfSequenceGroups[1].NumberOfHalfCircleSequences = 0;
        //        ListOfSequenceGroups[1].NumberOfRectangularSequences = 2;

        //        // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
        //        RecalculateTotalNumberOfScrews();
        //    }
        //    else
        //    {
        //        // Celkovy pocet skrutiek
        //        // Definovane su len sekvencie v jednej group, ocakava sa ze pocet v groups je rovnaky a hodnoty sa skopiruju (napr. pre apex plate)
        //        RecalculateTotalNumberOfScrews();
        //        int iNumberOfGroupsInPlate = 2;
        //        IHolesNumber *= iNumberOfGroupsInPlate;
        //    }

        //    HolesCentersPoints2D = new Point[IHolesNumber];
        //    arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        //}

        public Point[] Get_ScrewSequencePointCoordinates(CScrewRectSequence srectSeq)
        {
            // Connectors in Sequence
            if (srectSeq.SameDistancesX && srectSeq.SameDistancesY) // Ak su pre oba smery vzdialenosti skrutiek rovnake, posielame do konstruktora len jedno cislo pre rozostup (vzdialenost) skrutiek
                return GetRegularArrayOfPointsInCartesianCoordinates(new Point(srectSeq.RefPointX, srectSeq.RefPointY), srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
            else // Ak su aspon pre jeden smer vzdialenosti skrutiek rozdielne, posielame do konstruktora zoznam rozostupov (rozne vzdialenosti) skrutiek
                return GetRegularArrayOfPointsInCartesianCoordinates(new Point(srectSeq.RefPointX, srectSeq.RefPointY), srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistancesOfPointsX.ToArray(), srectSeq.DistancesOfPointsY.ToArray());

            //// Connectors in Sequence
            //return GetRegularArrayOfPointsInCartesianCoordinates(new Point(srectSeq.RefPointX, srectSeq.RefPointY), srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
        }

        public override void Calc_HolesCentersCoord2DApexPlate(
            float fOffset_x,
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        {
            // Coordinates of [0,0] of sequence point on plate (used to translate all sequences in the group)
            float fx_c = fOffset_x + 0.00f;
            float fy_c = flZ + 0.00f;

            int grCount = 0;
            foreach (CScrewSequenceGroup gr in ListOfSequenceGroups)
            {
                grCount++;
                foreach (CScrewRectSequence sc in gr.ListSequence)
                {
                    sc.HolesCentersPoints = Get_ScrewSequencePointCoordinates(sc);
                    // Rotate screws by roof slope
                    // Rotate about [0,0]
                    RotateSequence_CCW_rad(0, 0, fSlope_rad, sc);

                    // Translate from [0,0] on plate to the final position
                    TranslateSequence(fx_c, fy_c, sc);

                    if (grCount == 2) //second group is mirrored
                    {
                        sc.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * fbX, sc);
                    }
                }

                gr.HolesRadii = gr.Get_RadiiOfConnectorsInGroup();
            }

            FillArrayOfHolesCentersInWholeArrangement();
                       

            //// Left side
            //ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

            //if(ListOfSequenceGroups[0].ListSequence.Count == 2) // TODO Ondrej - zaviest mat moznost mat dynamicky pocet sekvencii
            //   ListOfSequenceGroups[0].ListSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);
            //// Set radii of connectors / screws in the group
            //ListOfSequenceGroups[0].HolesRadii = ListOfSequenceGroups[0].Get_RadiiOfConnectorsInGroup();

            //// Rotate screws by roof slope
            //// Rotate about [0,0]
            //RotateSequence_CCW_rad(0, 0, fSlope_rad, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

            //if (ListOfSequenceGroups[0].ListSequence.Count == 2)
            //    RotateSequence_CCW_rad(0, 0, fSlope_rad, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);

            //// Translate from [0,0] on plate to the final position
            //TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);

            //if (ListOfSequenceGroups[0].ListSequence.Count == 2)
            //    TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);

            //// Right side
            //CScrewRectSequence seq3 = new CScrewRectSequence();
            //seq3.HolesCentersPoints = ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints;
            //seq3.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * fbX, seq3);

            //CScrewRectSequence seq4 = new CScrewRectSequence();
            //if (ListOfSequenceGroups[0].ListSequence.Count == 2)
            //{
            //    seq4.HolesCentersPoints = ListOfSequenceGroups[0].ListSequence[1].HolesCentersPoints;
            //    seq4.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * fbX, seq4);
            //}

            //// Add mirrored sequences into the list
            //if (ListOfSequenceGroups.Count == 1) // Just in case that mirrored (right side) group doesn't exists
            //{
            //    ListOfSequenceGroups.Add(new CScrewSequenceGroup()); // Right Side Group

            //    ListOfSequenceGroups[1].ListSequence.Add(seq3);
            //    ListOfSequenceGroups[1].NumberOfRectangularSequences = 1;

            //    if (ListOfSequenceGroups[0].ListSequence.Count == 2)
            //    {
            //        ListOfSequenceGroups[1].ListSequence.Add(seq4);
            //        ListOfSequenceGroups[1].NumberOfRectangularSequences = 2;
            //    }
            //}
            //else // In case that group already exists set current sequences
            //{
            //    ListOfSequenceGroups[1].ListSequence[0] = seq3;
            //    ListOfSequenceGroups[1].NumberOfRectangularSequences = 1;

            //    if (ListOfSequenceGroups[0].ListSequence.Count == 2)
            //    {
            //        ListOfSequenceGroups[1].ListSequence[1] = seq4;
            //        ListOfSequenceGroups[1].NumberOfRectangularSequences = 2;
            //    }
            //}
            //ListOfSequenceGroups[1].HolesRadii = ListOfSequenceGroups[1].Get_RadiiOfConnectorsInGroup();

            //FillArrayOfHolesCentersInWholeArrangement();
        }

        public override void Calc_HolesCentersCoord2DKneePlate(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        {
            //To Mato - toto tu na nieco potrebujeme?
            //To Ondrej: TY BETAR TY MI TU NEKOMENTUJ TOTO :))))

            // Coordinates of [0,0] of sequence point on plate (used to translate all sequences in the group)
            float fx_cBG = flZ + FCrscRafterDepth;
            float fy_cBG = 0;

            float fx_cUG = flZ + FCrscRafterDepth * (float)Math.Sin(fSlope_rad);
            float fy_cUG = fhY_1 - FCrscRafterDepth * (float)Math.Cos(fSlope_rad);

            int grCount = 0;
            foreach (CScrewSequenceGroup gr in ListOfSequenceGroups)
            {
                grCount++;
                foreach (CScrewRectSequence sc in gr.ListSequence)
                {
                    sc.HolesCentersPoints = Get_ScrewSequencePointCoordinates(sc);

                    if (grCount % 2 == 1)
                    {
                        // Bottom Group

                        // Rotate about [0,0] 90 deg
                        RotateSequence_CCW_rad(0, 0, 0.5f * (float)Math.PI, sc);

                        // Translate from [0,0] on plate to the final position
                        TranslateSequence(fx_cBG, fy_cBG, sc);
                    }
                    else
                    {
                        // Upper Group

                        // Rotate screws by roof slope
                        // Rotate about [0,0]
                        RotateSequence_CCW_rad(0, 0, fSlope_rad, sc);

                        // Translate from [0,0] on plate to the final position
                        TranslateSequence(fx_cUG, fy_cUG, sc);
                    }
                }

                gr.HolesRadii = gr.Get_RadiiOfConnectorsInGroup();
            }

            FillArrayOfHolesCentersInWholeArrangement();



            ////TODO Mato - ak mame dynamicke sekvencie tak na pevne indexy mozme zabudnut, ziadne taketo somarinky tu byt nemozu
            //if (ListOfSequenceGroups.Count > 0)
            //{
            //    //temp na surovo prec ak je menej ako 2 lebo sa to samozrejme zrube
            //    if (ListOfSequenceGroups[0].ListSequence.Count < 2) return;

            //    // Bottom group - column
            //    ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            //    ListOfSequenceGroups[0].ListSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);
            //    // Set radii of connectors / screws in the group
            //    ListOfSequenceGroups[0].HolesRadii = ListOfSequenceGroups[0].Get_RadiiOfConnectorsInGroup();

            //    // Rotate screws by colum slope (bottom group only)
            //    // Rotate about [0,0] 90 deg
            //    RotateSequence_CCW_rad(0, 0, 0.5f * (float)Math.PI, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            //    RotateSequence_CCW_rad(0, 0, 0.5f * (float)Math.PI, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);

            //    if (ListOfSequenceGroups.Count > 1)
            //    {
            //        // Upper group - rafter
            //        ListOfSequenceGroups[1].ListSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[1].ListSequence[0]);
            //        ListOfSequenceGroups[1].ListSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[1].ListSequence[1]);
            //        // Set radii of connectors / screws in the group
            //        ListOfSequenceGroups[1].HolesRadii = ListOfSequenceGroups[1].Get_RadiiOfConnectorsInGroup();

            //        // Rotate screws by roof slope (upper group only)
            //        // Rotate about [0,0]
            //        RotateSequence_CCW_rad(0, 0, fSlope_rad, (CScrewRectSequence)ListOfSequenceGroups[1].ListSequence[0]);
            //        RotateSequence_CCW_rad(0, 0, fSlope_rad, (CScrewRectSequence)ListOfSequenceGroups[1].ListSequence[1]);
            //    }
                

            //    // Translate from [0,0] on plate to the final position
            //    // Bottom Group
            //    TranslateSequence(fx_cBG, fy_cBG, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            //    TranslateSequence(fx_cBG, fy_cBG, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);

            //    if (ListOfSequenceGroups.Count > 1)
            //    {
            //        // Upper Group
            //        TranslateSequence(fx_cUG, fy_cUG, (CScrewRectSequence)ListOfSequenceGroups[1].ListSequence[0]);
            //        TranslateSequence(fx_cUG, fy_cUG, (CScrewRectSequence)ListOfSequenceGroups[1].ListSequence[1]);
            //    }
            //}

            

            //FillArrayOfHolesCentersInWholeArrangement();
        }

        public void Calc_HolesCentersCoord2D_PlateL(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY)
        {
            // Coordinates of [0,0] of sequence point on plate (used to translate all sequences in the group)
            float fx_c = 0;
            float fy_c = 0;

            int grCount = 0;
            foreach (CScrewSequenceGroup gr in ListOfSequenceGroups)
            {
                grCount++;
                foreach (CScrewRectSequence sc in gr.ListSequence)
                {
                    sc.HolesCentersPoints = Get_ScrewSequencePointCoordinates(sc);

                    // Pre Plate L nerotujeme sekvencie ani skupiny
                    // Translate from [0,0] on plate to the final position
                    TranslateSequence(fx_c, fy_c, sc);

                    if (grCount == 2 /*&& TODO Ondrej IsMirrored - zohladnit parameter z GUI*/) //second group is mirrored
                    {
                        // Pozicia osi zrkadlenia pre L 0.5 * fl_Z + 0.5 * (0.5 * fl_Z + 0.5 * fb_X1)
                        sc.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * flZ + 0.5f * (0.5f * flZ + 0.5f * fbX_1), sc);
                    }
                }

                gr.HolesRadii = gr.Get_RadiiOfConnectorsInGroup();
            }

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

        public Point[] GetMirroredSequenceAboutY(float fXDistanceOfMirrorAxis, CScrewSequence InputSequence)
        {
            Point[] OutputSequence = new  Point[InputSequence.HolesCentersPoints.Length];
            for (int i = 0; i< InputSequence.HolesCentersPoints.Length; i++)
            {
                OutputSequence[i].X = 2 * fXDistanceOfMirrorAxis + InputSequence.HolesCentersPoints[i].X * (-1f);
                OutputSequence[i].Y = InputSequence.HolesCentersPoints[i].Y;
            }

            return OutputSequence;
        }

        public void RotateSequence_CCW_rad(float fRotationCenterPoint_x, float fRotationCenterPoint_y, float fRotationAngle_rad, CScrewRectSequence sequence)
        {
            Point[] seqPoints = sequence.HolesCentersPoints;
            Geom2D.TransformPositions_CCW_rad(fRotationCenterPoint_x, fRotationCenterPoint_y, fRotationAngle_rad, ref seqPoints);
            sequence.HolesCentersPoints = seqPoints; // je to potrebne takto nastavovat lebo nie je mozne volat [ref sequence.HolesCentersPoints]
        }

        public void TranslateSequence(float fPoint_x, float fPoint_y, CScrewRectSequence sequence)
        {
            Point[] seqPoints = sequence.HolesCentersPoints;
            Geom2D.TransformPositions_CCW_rad(fPoint_x, fPoint_y, 0, ref seqPoints);
            sequence.HolesCentersPoints = seqPoints; // je to potrebne takto nastavovat lebo nie je mozne volat [ref sequence.HolesCentersPoints]
        }


        public void NumberOfGroups_Updated(int newNumberOfGroups)
        {
            if (newNumberOfGroups < 0) return;
            if (newNumberOfGroups > 5) return;

            if (newNumberOfGroups < NumberOfGroups)
            {
                while (newNumberOfGroups < NumberOfGroups)
                {
                    RemoveSequenceGroup();
                    NumberOfGroups--;
                }                
            }
            else if (newNumberOfGroups > NumberOfGroups)
            {
                while (newNumberOfGroups > NumberOfGroups)
                {
                    AddSequenceGroup();
                    NumberOfGroups++;
                }
            }
        }
        public void NumberOfSequenceInGroup_Updated(int newNumberOfSequenceInGroup)
        {
            if (newNumberOfSequenceInGroup < 0) return;
            if (newNumberOfSequenceInGroup > 10) return;

            if (newNumberOfSequenceInGroup < NumberOfSequenceInGroup)
            {
                while (newNumberOfSequenceInGroup < NumberOfSequenceInGroup)
                {
                    RemoveSequenceFromEachGroup();
                    NumberOfSequenceInGroup--;
                }
            }
            else if (newNumberOfSequenceInGroup > NumberOfSequenceInGroup)
            {
                while (newNumberOfSequenceInGroup > NumberOfSequenceInGroup)
                {
                    AddSequenceToEachGroup();
                    NumberOfSequenceInGroup++;
                }
            }
        }

        private void AddSequenceGroup()
        {
            CScrewSequenceGroup gr = new CScrewSequenceGroup();
            for (int i = 0; i < NumberOfSequenceInGroup; i++)
            {
                CScrewRectSequence rS = new CScrewRectSequence();
                RectSequences.Add(rS);
                gr.ListSequence.Add(rS);
            }
            ListOfSequenceGroups.Add(gr);
        }
        private void RemoveSequenceGroup()
        {
            ListOfSequenceGroups.RemoveAt(ListOfSequenceGroups.Count - 1);
            for (int i = 0; i < NumberOfSequenceInGroup; i++)
            {                
                RectSequences.RemoveAt(RectSequences.Count - 1);                
            }
        }

        private void AddSequenceToEachGroup()
        {
            int grIndex = 0;
            foreach (CScrewSequenceGroup gr in ListOfSequenceGroups)
            {
                CScrewRectSequence rS = new CScrewRectSequence();
                gr.ListSequence.Add(rS);

                if (MirroredGroups)
                {
                    if (grIndex == 0) RectSequences.Insert(NumberOfSequenceInGroup, rS);
                }
                else
                {
                    RectSequences.Insert(grIndex * NumberOfSequenceInGroup + NumberOfSequenceInGroup, rS);
                }

                grIndex++;
            }
        }
        private void RemoveSequenceFromEachGroup()
        {
            for (int i = ListOfSequenceGroups.Count - 1; i >= 0; i--)
            {
                ListOfSequenceGroups[i].ListSequence.RemoveAt(ListOfSequenceGroups[i].ListSequence.Count - 1);
                if (MirroredGroups)
                {
                    if (i == 0) RectSequences.RemoveAt(NumberOfSequenceInGroup - 1);
                }
                else
                {
                    RectSequences.RemoveAt(i * NumberOfSequenceInGroup + NumberOfSequenceInGroup - 1);
                }
            }
        }
    }
}