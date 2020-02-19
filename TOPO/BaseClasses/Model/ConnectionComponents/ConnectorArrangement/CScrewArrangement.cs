using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using MATH;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public abstract class CScrewArrangement : CConnectorArrangement
    {
        private List<CScrewSequenceGroup> m_listOfSequenceGroups;

        public List<CScrewSequenceGroup> ListOfSequenceGroups
        {
            get
            {
                return m_listOfSequenceGroups;
            }

            set
            {
                m_listOfSequenceGroups = value;
            }
        }

        private CScrew m_referenceScrew;

        public CScrew referenceScrew
        {
            get
            {
                return m_referenceScrew;
            }

            set
            {
                m_referenceScrew = value;
            }
        }

        // TO Ondrej - mam tu pole m_arrPlateScrews ale aj zoznam List<CScrewSequenceGroup> m_listOfSequenceGroups
        // To je v istom zmysle duplicita, v tom zozname su jednotlive skupiny skrutiek, v nich sekvencie a v tych sekvenciach by mali byt skrutky
        // V tom poli su objeky skrutiek zo vsetkych skupin a vsetkych sekvencii

        // TODO - chcelo by to refaktorovat hierarchiu tych objektov a dat tomu hlavu a patu :)

        private CScrew[] m_arrPlateScrews;

        public CScrew[] Screws
        {
            get
            {
                return m_arrPlateScrews;
            }

            set
            {
                m_arrPlateScrews = value;
            }
        }

        public Point3D[] arrConnectorControlPoints3D; // Array of control points for inserting connectors (bolts, screws, anchors, ...)

        public CScrewArrangement()
        { }

        public CScrewArrangement(int iScrewsNumber_temp, CScrew referenceScrew_temp) : base(iScrewsNumber_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        //public List<Point> GetHolesCentersPoints2D()
        //{
        //    List<Point> points = null;
        //    if (this.HolesCentersPoints2D != null)
        //    {
        //        points = new List<Point>(this.HolesCentersPoints2D.Length);
        //        for (int i = 0; i < this.HolesCentersPoints2D.Length; i++)
        //        {
        //            points.Add(new Point(this.HolesCentersPoints2D[i].X, this.HolesCentersPoints2D[i].Y));
        //        }
        //    }
        //    return points;
        //}

        public virtual void Calc_KneePlateData(
            float fbX1,
            float fbX2,
            float flZ,
            float fhY1,
            float ft,
            float fSlope_rad,
            bool bScrewInPlusZDirection)
        { }

        public virtual void Calc_HolesCentersCoord2DKneePlate(
            float fbX_1,
            float fbX_2,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        { }

        public virtual void Calc_ApexPlateData(
            float fOffset_x,
            float fbX,
            float flZ,
            float fhY_1,
            float ft,
            float fSlope_rad,
            bool bScrewInPlusZDirection)
        { }
        public virtual void Calc_HolesCentersCoord2DApexPlate(
            float fOffset_x,
            float fbX,
            float flZ,
            float fhY_1,
            float fSlope_rad)
        { }

        public virtual void UpdateArrangmentData() { }

        public virtual void Calc_HolesControlPointsCoord3D_FlatPlate(float fx, float fy, float ft, bool bScrewInPlusZDirection)
        {
            for (int i = 0; i < IHolesNumber; i++)
            {
                arrConnectorControlPoints3D[i].X = HolesCentersPoints2D[i].X - fx; // Odpocitat hodnotu flZ pridanu pre 2D zobrazenie (knee plate alebo apex JC)
                arrConnectorControlPoints3D[i].Y = HolesCentersPoints2D[i].Y - fy; // Odpocitat hodnotu flZ pridanu pre 2D zobrazenie (apex plate)

                if (bScrewInPlusZDirection)
                    arrConnectorControlPoints3D[i].Z = -referenceScrew.T_ht_headTotalThickness;
                else
                    arrConnectorControlPoints3D[i].Z = ft + referenceScrew.T_ht_headTotalThickness;
            }
        }

        public virtual void GenerateConnectors_FlatPlate(bool bScrewInPlusZDirection)
        {
            Screws = new CScrew[IHolesNumber];

            for (int i = 0; i < IHolesNumber; i++)
            {
                Point3D controlpoint = new Point3D(arrConnectorControlPoints3D[i].X, arrConnectorControlPoints3D[i].Y, arrConnectorControlPoints3D[i].Z);

                if (bScrewInPlusZDirection)
                    Screws[i] = new CScrew(referenceScrew, controlpoint, 0, -90, 0);
                else
                    Screws[i] = new CScrew(referenceScrew, controlpoint, 0, 90, 0);
            }
        }

        // BASE PLATE
        public virtual void Calc_BasePlateData(
            float fbX,
            float flZ,
            float fhY,
            float ft)
        { }

        public virtual void Calc_HolesCentersCoord2DBasePlate(
            float fbX,
            float flZ,
            float fhY)
        { }

        // FACE PLATE "O"- KNEE JOINT
        public virtual void Calc_FacePlateData(
            float fbX1,
            float fbX2,
            float fhY1,
            float ft)
        { }

        public virtual void Calc_HolesCentersCoord2DFacePlate(
            float fbX1,
            float fbX2,
            float fhY1)
        { }

        public virtual void RecalculateTotalNumberOfScrews()
        {
            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            IHolesNumber = 0;

            foreach (CScrewSequenceGroup group in ListOfSequenceGroups)
            {
                foreach (CScrewSequence seq in group.ListSequence)
                    IHolesNumber += seq.INumberOfConnectors;
            }

            // Validation
            if (IHolesNumber < 0)
                IHolesNumber = 0;
        }

        public void RecalculateTotalNumberOfScrews_2()
        {
            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            IHolesNumber = 0;

            for (int i = 0; i < ListOfSequenceGroups.Count; i++) // Add each group
            {
                for (int j = 0; j < ListOfSequenceGroups[i].ListSequence.Count; j++) // Add each sequence in group
                {
                    if (ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints == null) continue;

                    IHolesNumber += ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints.Length;
                }
            }

            // Validation
            if (IHolesNumber < 0)
                IHolesNumber = 0;
        }

        public override void FillArrayOfHolesCentersInWholeArrangement()
        {
            //-------------------------------------------------------------------------------------------------------------------------------------------------
            // Po vyrieseni zmazat celu tuto cast

            //TODO - tu bude potrebne doplnit nejaku kontrolu, a rozsirenie pola HolesCentersPoints2D, lebo sa to tu nejako zrubava
            //resp. nejako reagovat na zmenu poctu skrutiek a nasledna inicializacia tohto pola
            //ja vlastne ani neviem naco to tu toto vlastne je, sa mi nezda,zeby to nieco robilo

            // TO Ondrej
            // Tato funkcia nerobi nic ine len pozbera suradnice z jednotlivych groups a ich sekvencii a vsetky body nakopiruje do jedneho velkeho pola HolesCentersPoints2D.
            // To pole sa potom pouziva pre vykreslovanie skrutiek na 2D plechu a neviem kde vsade este.

            // Pred tym nez sa zavola tato funckia sa musi spravne alokovat velkost pola, to znamena ze musi byt prepocitane IHolesNumber
            // vid metoda RecalculateTotalNumberOfScrews To by sa malo zmenit uz pred tym nez sa zmenene screw arrangement nastavuje plechu
            // Akurat si nie som isty ako to bude s tymi mirrorovanymi groups. Tie by uz mali byt vytvorene a updatovane ked sa zavola tato funckia.
            // Vsetky parametre sekvencii a groups musia byt pred volanim tejto funkcie aktualizovane.

            // Problem je ze sice sa zmeni v sekvencii velkost pola HolesCentersPoints ale dalsie parametre ako INumberOfConnectors zostane povodne cislo
            // Vyrobil som druhu metodu RecalculateTotalNumberOfScrews_2, ktora prepocita pocet skrutiek podla velkosti pola HolesCentersPoints
            // Spravne by sa vsak mal nastavit pri zmene arrangement a sekvencii pocet jej prvkov seq.INumberOfConnectors

            RecalculateTotalNumberOfScrews_2(); // Docasne to prepocitavam a menim tu ale urcite by sme to mali urobit niekde skor
            HolesCentersPoints2D = new Point[IHolesNumber]; // Docasne
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            // Fill array of holes centers - whole arrangement
            int iPointIndex = 0;
            for (int i = 0; i < ListOfSequenceGroups.Count; i++) // Add each group
            {
                for (int j = 0; j < ListOfSequenceGroups[i].ListSequence.Count; j++) // Add each sequence in group
                {
                    if (ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints == null) continue;

                    for (int k = 0; k < ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints.Length; k++) // Add each point in the sequence
                    {
                        HolesCentersPoints2D[iPointIndex + k].X = ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints[k].X;
                        HolesCentersPoints2D[iPointIndex + k].Y = ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints[k].Y;
                    }

                    iPointIndex += ListOfSequenceGroups[i].ListSequence[j].HolesCentersPoints.Length;
                }
            }
        }
    }
}
