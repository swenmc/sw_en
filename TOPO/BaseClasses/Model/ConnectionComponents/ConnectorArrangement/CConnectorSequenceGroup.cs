using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public abstract class CConnectorSequenceGroup
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

        private List<CConnectorSequence> m_listSequence;

        public List<CConnectorSequence> ListSequence
        {
            get
            {
                if (m_listSequence == null) m_listSequence = new List<CConnectorSequence>();
                return m_listSequence;
            }

            set
            {
                m_listSequence = value;
            }
        }

        private float [] m_fHolesRadii;

        public float [] HolesRadii
        {
            get
            {
                return m_fHolesRadii;
            }

            set
            {
                m_fHolesRadii = value;
            }
        }

        public CConnectorSequenceGroup()
        {
            ListSequence = new List<CConnectorSequence>();
        }

        public float[] Get_RadiiOfConnectorsInGroup()
        {
            // Najprv spocitat polohu taziska skrutiek a potom pocitat polomery od tohoto taziska
            Point pCentroid = new Point(0, 0);

            //Cx = ∑Cix Aix / ∑Aix
            //Cy = ∑Ciy Aiy / ∑Aiy
            double fSumXPositions = 0;
            double fSumYPositions = 0;

            int iLastItemIndex = 0;

            foreach (CConnectorSequence seq in ListSequence)
            {
                // Set centroid position

                for (int i = 0; i < seq.INumberOfConnectors; i++)
                {
                    fSumXPositions += seq.HolesCentersPoints[i].X - pCentroid.X; // Origin used as centroid [0,0]
                    fSumYPositions += seq.HolesCentersPoints[i].Y - pCentroid.Y; // Origin used as centroid [0,0]
                }

                iLastItemIndex += seq.INumberOfConnectors;
            }

            // Set new centroid coordinates
            pCentroid.X = fSumXPositions / iLastItemIndex;
            pCentroid.Y = fSumYPositions / iLastItemIndex;

            // Count total number of screws in group and allocate size of array
            int iTotalNumberOfConnectorsInGroup = 0;

            foreach (CConnectorSequence seq in ListSequence)
            {
                iTotalNumberOfConnectorsInGroup += seq.INumberOfConnectors;
            }

            float[] fArrayOfRadii = new float[iTotalNumberOfConnectorsInGroup];

            iLastItemIndex = 0;

            foreach (CConnectorSequence seq in ListSequence)
            {
                // Set radii of connectors / screws in the connection

                for (int i = 0; i < seq.INumberOfConnectors; i++)
                    fArrayOfRadii[iLastItemIndex + i] = (float)Math.Sqrt(MathF.Pow2(seq.HolesCentersPoints[i].X - pCentroid.X) + MathF.Pow2(seq.HolesCentersPoints[i].Y - pCentroid.Y));

                iLastItemIndex += seq.INumberOfConnectors;
            }

            return fArrayOfRadii;
        }

        public void TransformGroup(Point referencePoint, float fAngle_seq_rotation_deg)
        {
            // Pootocit a presunut skupinu do finalnej polohy na plechu

            foreach (CConnectorSequence seq in ListSequence)
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
