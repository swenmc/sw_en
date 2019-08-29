using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CLine : CEntity
    {
        public int[] m_iPointsCollection; // List / Collection of points IDs

        public CLine()
        {
        
        }

        // Constructor 2
        public CLine(int iLine_ID, int[] iPCollection, int fTime)
        {
            ID = iLine_ID;
            m_iPointsCollection = iPCollection;
            FTime = fTime;
        }







        // 3D

        private Point3D m_Start;
        private Point3D m_End;

        private ELinePatternType m_patternType;
        private Point3DCollection m_PointsCollection;

        private float m_Length;

        public Point3D Start
        {
            get
            {
                return m_Start;
            }

            set
            {
                m_Start = value;
            }
        }

        public Point3D End
        {
            get
            {
                return m_End;
            }

            set
            {
                m_End = value;
            }
        }

        public ELinePatternType PatternType
        {
            get
            {
                return m_patternType;
            }

            set
            {
                m_patternType = value;
            }
        }

        public Point3DCollection PointsCollection
        {
            get
            {
                return m_PointsCollection;
            }

            set
            {
                m_PointsCollection = value;
            }
        }

        public float Length
        {
            get
            {
                return m_Length;
            }

            set
            {
                m_Length = value;
            }
        }

        private double segmentSpacing = 0.05f;
        private double dashSegmentLength = 0.1f;
        private double dotSegmentLength = 0.02f;
        private int iNumberOfSegmentsInSequence;
        private double sequenceLength;
        private Point3DCollection SequencePoints;

        public CLine(ELinePatternType patternType, Point3D start, Point3D end)
        {
            m_patternType = patternType;
            m_Start = start;
            m_End = end;

            // Straight line length
            Length = (float)Math.Sqrt((float)Math.Pow(m_End.X - m_Start.X, 2f) + (float)Math.Pow(m_End.Y - m_Start.Y, 2f) + (float)Math.Pow(m_End.Z - m_Start.Z, 2f));

            // Pripravime si sekvenciu / kolekciu bodov ciar, ktora sa bude opakovat
            SetLineSequencePoints();

            sequenceLength = SequencePoints[SequencePoints.Count - 1].X; // Pozicia x posledneho bodu v sekvencii

            // Inicializujeme a naplnime hlavnu kolekciu
            if (Length > 0)
            {
                m_PointsCollection = new Point3DCollection();

                // Pridavame sekvencie, kym nie je koncovy bod sekvencie (alebo jej segmentu) za koncom ciary

                double sequenceStart = m_Start.X;

                do
                {
                    for (int i = 0; i < SequencePoints.Count; i++) // Pridavame body zo sekvencie
                    {
                        Point3D p = new Point3D(sequenceStart + SequencePoints[i].X, SequencePoints[i].Y, SequencePoints[i].Z); // Nastavime bodu suradnicu X

                        if (p.X < (m_Start.X + m_Length)) // Bod pridame len ak je zaciatok segmentu v sekvencii mensi nez dlzka pruta !!! spravne by sme mali osetrit ze sa moze pridat zaciatok segmentu, ale uz na neprida koniec !!! Pocet bodov by mal byt parny
                            PointsCollection.Add(p);
                    }

                    sequenceStart += (sequenceLength + segmentSpacing); // Pripocitame dlzku sekvencie a dlzku medzery, aby sme dostali zaciatok x novej sekvencie
                }
                while
                (sequenceStart < (m_Start.X + m_Length)); // Pokracujeme kym je zaciatok sekvencie  mensi nez dlzka

                // Skontrolujeme ci mame parny pocet bodov v zozname
                if (m_PointsCollection.Count % 2 != 0)
                {
                    // Pocet bodov je neparny, pridali sme zaciatok segmentu, ale uz nie koniec
                    if (m_PointsCollection[m_PointsCollection.Count - 1].X < (m_Start.X + m_Length)) // posledny bod v sekvencii este nie je na konci ciary, pridame posledny bod s x = length
                        m_PointsCollection.Add(new Point3D(m_Start.X + m_Length, 0, 0));
                    else // Odstranime posledny bod - inak by mal posledny segment nulovu dlzku
                        m_PointsCollection.RemoveAt(m_PointsCollection.Count - 1);
                }
            }
        }

        private void SetLineSequencePoints()
        {
            // Define sequence of line segments in x-direction
            switch (m_patternType)
            {
                case ELinePatternType.DASHED:
                    {
                        iNumberOfSegmentsInSequence = 1;
                        SequencePoints = new Point3DCollection(2 * iNumberOfSegmentsInSequence);

                        SequencePoints.Add(new Point3D(0, 0, 0));
                        SequencePoints.Add(new Point3D(dashSegmentLength, 0, 0));
                        break;
                    }
                case ELinePatternType.DOTTED:
                    {
                        iNumberOfSegmentsInSequence = 1;
                        SequencePoints = new Point3DCollection(2 * iNumberOfSegmentsInSequence);

                        SequencePoints.Add(new Point3D(0, 0, 0));
                        SequencePoints.Add(new Point3D(dotSegmentLength, 0, 0));
                        break;
                    }
                case ELinePatternType.DASHDOTTED:
                    {
                        iNumberOfSegmentsInSequence = 2;
                        SequencePoints = new Point3DCollection(2 * iNumberOfSegmentsInSequence);

                        SequencePoints.Add(new Point3D(0, 0, 0));
                        SequencePoints.Add(new Point3D(dashSegmentLength, 0, 0));

                        SequencePoints.Add(new Point3D(dashSegmentLength + segmentSpacing, 0, 0));
                        SequencePoints.Add(new Point3D(dashSegmentLength + segmentSpacing + dotSegmentLength, 0, 0));
                        break;
                    }
                case ELinePatternType.DIVIDE:
                    {
                        iNumberOfSegmentsInSequence = 3;
                        SequencePoints = new Point3DCollection(2 * iNumberOfSegmentsInSequence);

                        SequencePoints.Add(new Point3D(0, 0, 0));
                        SequencePoints.Add(new Point3D(dashSegmentLength, 0, 0));

                        SequencePoints.Add(new Point3D(dashSegmentLength + segmentSpacing, 0, 0));
                        SequencePoints.Add(new Point3D(dashSegmentLength + segmentSpacing + dotSegmentLength, 0, 0));

                        SequencePoints.Add(new Point3D(dashSegmentLength + segmentSpacing + dotSegmentLength + segmentSpacing, 0, 0));
                        SequencePoints.Add(new Point3D(dashSegmentLength + segmentSpacing + dotSegmentLength + segmentSpacing + dotSegmentLength, 0, 0));
                        break;
                    }
                case ELinePatternType.CONTINUOUS:
                default:
                    {
                        iNumberOfSegmentsInSequence = 1;
                        SequencePoints = new Point3DCollection(2 * iNumberOfSegmentsInSequence);
                        segmentSpacing = 0;

                        SequencePoints.Add(new Point3D(0, 0, 0));
                        SequencePoints.Add(new Point3D(m_Length, 0, 0));
                        break;
                    }
            }
        }
    }
}
