using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MATH;

namespace BaseClasses
{
    public class CScrewSequenceGroup
    {
        private int m_iNumberOfHalfCircleSequences;

        public int NumberOfHalfCircleSequences
        {
            get
            {
                return m_iNumberOfHalfCircleSequences;
            }

            set
            {
                m_iNumberOfHalfCircleSequences = value;
            }
        }

        private int m_iNumberOfRectangularSequences;

        public int NumberOfRectangularSequences
        {
            get
            {
                return m_iNumberOfRectangularSequences;
            }

            set
            {
                m_iNumberOfRectangularSequences = value;
            }
        }

        private List<CScrewSequence> m_listScrewSequence;

        public List<CScrewSequence> ListScrewSequence
        {
            get
            {
                if (m_listScrewSequence == null) m_listScrewSequence = new List<CScrewSequence>();
                return m_listScrewSequence;
            }

            set
            {
                m_listScrewSequence = value;
            }
        }

        private float [] m_fScrewHolesRadii;

        public float [] ScrewHolesRadii
        {
            get
            {
                return m_fScrewHolesRadii;
            }

            set
            {
                m_fScrewHolesRadii = value;
            }
        }

        public CScrewSequenceGroup()
        {
            ListScrewSequence = new List<CScrewSequence>();
        }

        public float[] Get_RadiiOfScrewsInGroup()
        {
            // TODO najprv spocitat polohu taziska skrutiek a potom pocitat polomery od tohoto taziska
            Point pCentroid = new Point(0, 0);

            // Count total number of screws in group and allocate size of array
            int iTotalNumberOfScrewsInGroup = 0;

            foreach (CScrewSequence seq in ListScrewSequence)
            {
                iTotalNumberOfScrewsInGroup += seq.INumberOfScrews;
            }

            float[] fArrayOfRadii = new float[iTotalNumberOfScrewsInGroup];

            int iLastItemIdex = 0;

            foreach (CScrewSequence seq in ListScrewSequence)
            {
                // Set radii of connectors / screws in the connection

                for (int i = 0; i < seq.INumberOfScrews; i++)
                    fArrayOfRadii[iLastItemIdex + i] = (float)Math.Sqrt(MathF.Pow2(seq.HolesCentersPoints[i].X - pCentroid.X) + MathF.Pow2(seq.HolesCentersPoints[i].Y - pCentroid.Y));

                iLastItemIdex += seq.INumberOfScrews;
            }

            return fArrayOfRadii;
        }

        public void TransformGroup(Point referencePoint, float fAngle_seq_rotation_deg)
        {
            // Pootocit a presunut skupinu do finalnej polohy na plechu

            foreach (CScrewSequence seq in ListScrewSequence)
            {
                Point[] array = seq.HolesCentersPoints;

                // Rotate about [0,0]
                Geom2D.TransformPositions_CCW_deg(0, 0, fAngle_seq_rotation_deg, ref array);
                // Translate
                Geom2D.TransformPositions_CCW_deg((float)referencePoint.X, (float)referencePoint.Y, 0, ref array);

                seq.HolesCentersPoints = array;
            }
        }
    }
}
