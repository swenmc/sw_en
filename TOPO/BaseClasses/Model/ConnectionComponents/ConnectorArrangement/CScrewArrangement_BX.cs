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
    // BD, BE, BF, BJ
    // One middle web stiffener, 4 segments
    // 50020 (single and nested) and 63020

    // BA, BB, BC, BG
    // Two web stiffeners, 6 segments
    // 270 single and nested

    [Serializable]
    public class CScrewArrangement_BX : CScrewArrangement
    {
        private int m_NumberOfGroups;
        private int m_NumberOfSequenceInGroup;
        private List<CScrewRectSequence> m_RectSequences;
        private bool m_MirroredGroups;

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

        public CScrewArrangement_BX() { }

        // Jedna sekvencia s rovnakymi vzdialenostami na jednej strane plate
        public CScrewArrangement_BX(
            CScrew referenceScrew_temp,
            int iNumberOfScrewsInRow_xDirection_SQ1_temp,
            int iNumberOfScrewsInColumn_yDirection_SQ1_temp,
            float fx_c_SQ1_temp,
            float fy_c_SQ1_temp,
            float fDistanceOfPointsX_SQ1_temp,
            float fDistanceOfPointsY_SQ1_temp) : base(iNumberOfScrewsInRow_xDirection_SQ1_temp * iNumberOfScrewsInColumn_yDirection_SQ1_temp, referenceScrew_temp)
        {
            referenceScrew = referenceScrew_temp;

            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            //RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));

            MirroredGroups = true;
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

        // Dve sekvencie s rovnakymi vzdialenostami na jednej strane plate
        public CScrewArrangement_BX(
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
            float fDistanceOfPointsY_SQ2_temp) : base(iNumberOfScrewsInRow_xDirection_SQ1_temp * iNumberOfScrewsInColumn_yDirection_SQ1_temp + iNumberOfScrewsInRow_xDirection_SQ2_temp * iNumberOfScrewsInColumn_yDirection_SQ2_temp, referenceScrew_temp)
        {
            referenceScrew = referenceScrew_temp;
            MirroredGroups = true;

            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ2_temp, iNumberOfScrewsInColumn_yDirection_SQ2_temp, fx_c_SQ2_temp, fy_c_SQ2_temp, fDistanceOfPointsX_SQ2_temp, fDistanceOfPointsY_SQ2_temp));
            //RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            //RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ2_temp, iNumberOfScrewsInColumn_yDirection_SQ2_temp, fx_c_SQ2_temp, fy_c_SQ2_temp, fDistanceOfPointsX_SQ2_temp, fDistanceOfPointsY_SQ2_temp));

            NumberOfGroups = 2;
            NumberOfSequenceInGroup = 2;

            IHolesNumber = 0;
            foreach (CScrewRectSequence rectS in RectSequences)
            {
                IHolesNumber += rectS.INumberOfConnectors;
            }
            if (MirroredGroups) IHolesNumber = IHolesNumber * 2;
            UpdateArrangmentData();
        }

        // Tento konstruktor by sa nemal pre databazove plates typu B pouzit
        // Tri sekvencie s rovnakymi vzdialenostami na jednej strane plate
        public CScrewArrangement_BX(
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
            float fDistanceOfPointsY_SQ2_temp,
            int iNumberOfScrewsInRow_xDirection_SQ3_temp,
            int iNumberOfScrewsInColumn_yDirection_SQ3_temp,
            float fx_c_SQ3_temp,
            float fy_c_SQ3_temp,
            float fDistanceOfPointsX_SQ3_temp,
            float fDistanceOfPointsY_SQ3_temp) : base(iNumberOfScrewsInRow_xDirection_SQ1_temp * iNumberOfScrewsInColumn_yDirection_SQ1_temp + iNumberOfScrewsInRow_xDirection_SQ2_temp * iNumberOfScrewsInColumn_yDirection_SQ2_temp + iNumberOfScrewsInRow_xDirection_SQ3_temp * iNumberOfScrewsInColumn_yDirection_SQ3_temp, referenceScrew_temp)
        {
            referenceScrew = referenceScrew_temp;
            MirroredGroups = true;

            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ2_temp, iNumberOfScrewsInColumn_yDirection_SQ2_temp, fx_c_SQ2_temp, fy_c_SQ2_temp, fDistanceOfPointsX_SQ2_temp, fDistanceOfPointsY_SQ2_temp));
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ3_temp, iNumberOfScrewsInColumn_yDirection_SQ3_temp, fx_c_SQ3_temp, fy_c_SQ3_temp, fDistanceOfPointsX_SQ3_temp, fDistanceOfPointsY_SQ3_temp));
            //RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, fDistanceOfPointsX_SQ1_temp, fDistanceOfPointsY_SQ1_temp));
            //RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ2_temp, iNumberOfScrewsInColumn_yDirection_SQ2_temp, fx_c_SQ2_temp, fy_c_SQ2_temp, fDistanceOfPointsX_SQ2_temp, fDistanceOfPointsY_SQ2_temp));
            //RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ3_temp, iNumberOfScrewsInColumn_yDirection_SQ3_temp, fx_c_SQ3_temp, fy_c_SQ3_temp, fDistanceOfPointsX_SQ3_temp, fDistanceOfPointsY_SQ3_temp));

            NumberOfGroups = 2;
            NumberOfSequenceInGroup = 3;

            IHolesNumber = 0;
            foreach (CScrewRectSequence rectS in RectSequences)
            {
                IHolesNumber += rectS.INumberOfConnectors;
            }
            if (MirroredGroups) IHolesNumber = IHolesNumber * 2;
            UpdateArrangmentData();
        }

        // Jedna sekvencia s roznymi vzdialenostami na jednej strane plate
        public CScrewArrangement_BX(
            CScrew referenceScrew_temp,
            int iNumberOfScrewsInRow_xDirection_SQ1_temp,
            int iNumberOfScrewsInColumn_yDirection_SQ1_temp,
            float fx_c_SQ1_temp,
            float fy_c_SQ1_temp,
            List<float> distancesOfPointsX_SQ1_temp,
            List<float> distancesOfPointsY_SQ1_temp) : base(iNumberOfScrewsInRow_xDirection_SQ1_temp * iNumberOfScrewsInColumn_yDirection_SQ1_temp, referenceScrew_temp)
        {
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------
            // TO Ondrej - tento konstruktor by sa dal pozjednodusovat,
            // mohli by sme don poslat len List distancesOfPointsX a distancesOfPointsY
            // Podla poctu prvkov v tomto zozname + 1 by sa urcili NumberOfScrewsInRow_xDirection a NumberOfScrewsInColumn_yDirection
            // Ak by bol pocet prvkov v zozname 1, tak by sa m_SameDistances nastavilo na true, ak by to bolo viac nez jedna, tak na false
            //To Mato - toto si Mato prekombinoval, to sa neda, aby z toho pola bol aj pocet screws a zaroven aby podla poctu to rozhodlo ze je sameDistance :-D co ak budem chciet pocet 4 a same distance :-D

            // To Ondrej - nie je mi uplne jasne preco sa to neda

            // Pride mi pole s jednym prvkom.
            // nastavim ze pocet skrutiek je 2
            // nastavim bSameDistance = true
            // pri generovani rastra potom budem brat vzdy distance ako tento jediny prvok pola

            // Pride mi pole s viac nez jednym prvkom.
            // Nastavim pocet skrutiek na velkost pola + 1
            // Zistim ci su vsetky vzdialenosti v poli rovnake (napriklad tymto MathF.d_equal(... , ...)
            // Ak su rovnake, nastavim bSameDistance = true,
            // Ak nie su rovnake, nastavim bSameDistance = false;
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------

            referenceScrew = referenceScrew_temp;
            MirroredGroups = true;
            //FCrscColumnDepth = fCrscColumnDepth_temp;
            //FCrscWebStraightDepth = fCrscWebStraightDepth_temp;
            //FStiffenerSize = fStiffenerSize_temp;

            //bool sameDistancesX = true;
            //bool sameDistancesY = true;

            //if (distancesOfPointsX_SQ1_temp.Count > 1)
            //     sameDistancesX = false;

            //if (distancesOfPointsY_SQ1_temp.Count > 1)
            //    sameDistancesY = false;

            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, distancesOfPointsX_SQ1_temp, distancesOfPointsY_SQ1_temp));
            //RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection_SQ1_temp, iNumberOfScrewsInColumn_yDirection_SQ1_temp, fx_c_SQ1_temp, fy_c_SQ1_temp, distancesOfPointsX_SQ1_temp, distancesOfPointsY_SQ1_temp));

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

        public override void UpdateArrangmentData()
        {
            ListOfSequenceGroups = new List<CScrewSequenceGroup>(NumberOfGroups);
            
            for (int i = 0; i < NumberOfGroups; i++)
            {
                CScrewSequenceGroup gr = new CScrewSequenceGroup();
                gr.NumberOfHalfCircleSequences = 0;
                gr.NumberOfRectangularSequences = NumberOfSequenceInGroup;
                
                for (int j = 0; j < NumberOfSequenceInGroup; j++)
                {                    
                    // Pridame sekvenciu do skupiny
                    if(i == 0) gr.ListSequence.Add(RectSequences[j]);
                    else gr.ListSequence.Add(RectSequences[j].Copy());
                }
                ListOfSequenceGroups.Add(gr);
            }
            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            RecalculateTotalNumberOfScrews();

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }

        public Point[] Get_ScrewSequencePointCoordinates(CScrewRectSequence srectSeq)
        {
            // Connectors in Sequence
            if(srectSeq.SameDistancesX && srectSeq.SameDistancesY) // Ak su pre oba smery vzdialenosti skrutiek rovnake, posielame do konstruktora len jedno cislo pre rozostup (vzdialenost) skrutiek
                return GetRegularArrayOfPointsInCartesianCoordinates(new Point(srectSeq.RefPointX, srectSeq.RefPointY), srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistanceOfPointsX, srectSeq.DistanceOfPointsY);
            else // Ak su aspon pre jeden smer vzdialenosti skrutiek rozdielne, posielame do konstruktora zoznam rozostupov (rozne vzdialenosti) skrutiek
                return GetRegularArrayOfPointsInCartesianCoordinates(new Point(srectSeq.RefPointX, srectSeq.RefPointY), srectSeq.NumberOfScrewsInRow_xDirection, srectSeq.NumberOfScrewsInColumn_yDirection, srectSeq.DistancesOfPointsX.ToArray(), srectSeq.DistancesOfPointsY.ToArray());
        }

        public override void Calc_HolesCentersCoord2DBasePlate(
            float fbX,
            float flZ,
            float fhY)
        {
            // Coordinates of [0,0] of sequence point on plate (used to translate all sequences in the group)
            //float fx_c = 0.000f;
            //float fy_c = 0.000f;

            int grCount = 0;
            foreach (CScrewSequenceGroup gr in ListOfSequenceGroups)
            {
                grCount++;
                foreach (CScrewRectSequence sc in gr.ListSequence)
                {
                    sc.HolesCentersPoints = Get_ScrewSequencePointCoordinates(sc);
                    
                    if (grCount == 2) //second group is mirrored
                    {
                        sc.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * (2 * flZ + fbX), sc);
                    }
                }

                gr.HolesRadii = gr.Get_RadiiOfConnectorsInGroup();
            }

            FillArrayOfHolesCentersInWholeArrangement();


            //// Left side
            //ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            //ListOfSequenceGroups[0].ListSequence[1].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);
            //ListOfSequenceGroups[0].ListSequence[2].HolesCentersPoints = Get_ScrewSequencePointCoordinates((CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[2]);
            //// Set radii of connectors / screws in the group
            //ListOfSequenceGroups[0].HolesRadii = ListOfSequenceGroups[0].Get_RadiiOfConnectorsInGroup();


            ////TO Mato - toto mi pride riadne divne, ze posuvame nieco, resp. rotujeme o 0 stupnov z [0,0]- sak to nespravi vobec nic
            //// Translate from [0,0] on plate to the final position
            //TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[0]);
            //TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[1]);
            //TranslateSequence(fx_c, fy_c, (CScrewRectSequence)ListOfSequenceGroups[0].ListSequence[2]);

            //// Right side
            //CScrewRectSequence seq3 = new CScrewRectSequence();
            //seq3.HolesCentersPoints = ListOfSequenceGroups[0].ListSequence[0].HolesCentersPoints;
            //seq3.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * (2 * flZ + fbX), seq3);

            //CScrewRectSequence seq4 = new CScrewRectSequence();
            //seq4.HolesCentersPoints = ListOfSequenceGroups[0].ListSequence[1].HolesCentersPoints;
            //seq4.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * (2 * flZ + fbX), seq4);

            //CScrewRectSequence seq6 = new CScrewRectSequence();
            //seq6.HolesCentersPoints = ListOfSequenceGroups[0].ListSequence[2].HolesCentersPoints;
            //seq6.HolesCentersPoints = GetMirroredSequenceAboutY(0.5f * (2 * flZ + fbX), seq6);

            //// Add mirrored sequences into the list
            //if (ListOfSequenceGroups.Count == 1) // Just in case that mirrored (right side) group doesn't exists
            //{
            //    ListOfSequenceGroups.Add(new CScrewSequenceGroup()); // Right Side Group

            //    ListOfSequenceGroups[1].ListSequence.Add(seq3);
            //    ListOfSequenceGroups[1].ListSequence.Add(seq4);
            //    ListOfSequenceGroups[1].ListSequence.Add(seq6);
            //    ListOfSequenceGroups[1].NumberOfRectangularSequences = 2;
            //}
            //else // In case that group already exists set current sequences
            //{
            //    ListOfSequenceGroups[1].ListSequence[0] = seq3;
            //    ListOfSequenceGroups[1].ListSequence[1] = seq4;
            //    ListOfSequenceGroups[1].NumberOfRectangularSequences = 2;
            //}

            //ListOfSequenceGroups[1].HolesRadii = ListOfSequenceGroups[1].Get_RadiiOfConnectorsInGroup();

            //FillArrayOfHolesCentersInWholeArrangement();
        }

        public override void Calc_BasePlateData(
            float fbX,
            float flZ,
            float fhY,
            float ft)
        {
            Calc_HolesCentersCoord2DBasePlate(fbX, flZ, fhY);
            Calc_HolesControlPointsCoord3D(fbX, flZ, ft);
            GenerateConnectors_BasePlate();
        }

        void Calc_HolesControlPointsCoord3D(float fbX, float flZ, float ft)
        {
            // Left group
            int iGroupIndex = 0;
            int iLastItemIndex = 0;
            for (int i = 0; i < ListOfSequenceGroups[iGroupIndex].ListSequence.Count; i++)
            {
                for (int j = 0; j < ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length; j++)
                {
                    arrConnectorControlPoints3D[iLastItemIndex + j].X = -referenceScrew.T_ht_headTotalThickness;
                    arrConnectorControlPoints3D[iLastItemIndex + j].Y = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].Y;
                    arrConnectorControlPoints3D[iLastItemIndex + j].Z =  flZ - ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].X;
                }

                iLastItemIndex += ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length;
            }

            // Right group
            iGroupIndex = 1;
            iLastItemIndex = 0;
            for (int i = 0; i < ListOfSequenceGroups[iGroupIndex].ListSequence.Count; i++)
            {
                for (int j = 0; j < ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length; j++)
                {
                    arrConnectorControlPoints3D[IHolesNumber / 2 + iLastItemIndex + j].X = fbX + 2 * ft + referenceScrew.T_ht_headTotalThickness;
                    arrConnectorControlPoints3D[IHolesNumber / 2 + iLastItemIndex + j].Y = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].Y;
                    arrConnectorControlPoints3D[IHolesNumber / 2 + iLastItemIndex + j].Z = ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints[j].X - flZ - fbX;
                }

                iLastItemIndex += ListOfSequenceGroups[iGroupIndex].ListSequence[i].HolesCentersPoints.Length;
            }
        }

        void GenerateConnectors_BasePlate()
        {
            Screws = new CScrew[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);

                if(i < IHolesNumber /2) // Left side (rotation 0 deg about y-axis)
                    Screws[i] = new CScrew(referenceScrew, controlpoint, 0, 0, 0);
                else // Right side (rotation 180 deg about y-axis)
                    Screws[i] = new CScrew(referenceScrew, controlpoint, 0, 180, 0);
            }
        }

        public Point[] GetMirroredSequenceAboutY(float fXDistanceOfMirrorAxis, CScrewSequence InputSequence)
        {
            Point[] OutputSequence = new Point[InputSequence.HolesCentersPoints.Length];
            for (int i = 0; i < InputSequence.HolesCentersPoints.Length; i++)
            {
                OutputSequence[i].X = 2 * fXDistanceOfMirrorAxis + InputSequence.HolesCentersPoints[i].X * (-1f);
                OutputSequence[i].Y = InputSequence.HolesCentersPoints[i].Y;
            }

            return OutputSequence;
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
        private void AddSequenceToEachGroup()
        {
            int grIndex = 0;
            foreach (CScrewSequenceGroup gr in ListOfSequenceGroups)
            {
                CScrewRectSequence rS = new CScrewRectSequence();
                gr.ListSequence.Add(rS);

                if (MirroredGroups)
                {
                    if(grIndex ==0 ) RectSequences.Insert(NumberOfSequenceInGroup, rS);
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
                    if(i == 0) RectSequences.RemoveAt(NumberOfSequenceInGroup - 1);
                }
                else
                {
                    RectSequences.RemoveAt(i * NumberOfSequenceInGroup + NumberOfSequenceInGroup - 1);
                }                
            }
        }


    }
}
