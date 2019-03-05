using System;
using System.Collections.Generic;

namespace BaseClasses
{
    // Class definition - lateral-torsional segment of beam
    // Straigth or curved part of member between two intermediate lateral torsional supports (supporting both upper and bottom flange)
    // As default one segment exist for one member defined, segment could be straight or curved

    [Serializable]
    public class CSegment_LTB : CEntity
    {
        private float m_SegmentStartCoord_Rel;

        public float SegmentStartCoord_Rel
        {
            get { return m_SegmentStartCoord_Rel; }
            set { m_SegmentStartCoord_Rel = value; }
        }

        private float m_SegmentEndCoord_Rel;

        public float SegmentEndCoord_Rel
        {
            get { return m_SegmentEndCoord_Rel; }
            set { m_SegmentEndCoord_Rel = value; }
        }

        private float m_SegmentStartCoord_Abs;

        public float SegmentStartCoord_Abs
        {
            get { return m_SegmentStartCoord_Abs; }
            set { m_SegmentStartCoord_Abs = value; }
        }

        private float m_SegmentEndCoord_Abs;

        public float SegmentEndCoord_Abs
        {
            get { return m_SegmentEndCoord_Abs; }
            set { m_SegmentEndCoord_Abs = value; }
        }

        private float m_SegmentLength;

        public float SegmentLength
        {
            get { return m_SegmentLength; }
            set { m_SegmentLength = value; }
        }

        private List<designBucklingLengthFactors> m_BucklingLengthFactors; // Zoznam faktorov vzpernych dlzok segmentu pre kazdu load combination ???

        public List<designBucklingLengthFactors> BucklingLengthFactors
        {
            get { return m_BucklingLengthFactors; }
            set { m_BucklingLengthFactors = value; }
        }

        public CSegment_LTB(int id_temp, bool bIsRelativeCoordinate_x, float segmentStart, float segmentEnd, float fLengthMember)
        {
            ID = id_temp;

            if (bIsRelativeCoordinate_x)
            {
                SegmentStartCoord_Rel = segmentStart;
                SegmentEndCoord_Rel = segmentEnd;
                SegmentStartCoord_Abs = segmentStart * fLengthMember;
                SegmentEndCoord_Abs = segmentEnd * fLengthMember;
            }
            else
            {
                SegmentStartCoord_Abs = segmentStart;
                SegmentEndCoord_Abs = segmentEnd;
                SegmentStartCoord_Rel = segmentStart / fLengthMember;
                SegmentEndCoord_Rel = segmentEnd / fLengthMember;
            }

            SegmentLength = SegmentEndCoord_Abs - SegmentStartCoord_Abs;

            BucklingLengthFactors = new List<designBucklingLengthFactors>();

            designBucklingLengthFactors sBucklingLengthFactors;
            sBucklingLengthFactors.fBeta_x_FB_fl_ex = 1.0f; // Default - Length of member (pre prvky ramu column MC, EC a rafter MR,ER moze byt ina)

            sBucklingLengthFactors.fBeta_y_FB_fl_ey = SegmentLength / fLengthMember;
            sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez = SegmentLength / fLengthMember;
            sBucklingLengthFactors.fBeta_LTB_fl_LTB = SegmentLength / fLengthMember;

            BucklingLengthFactors.Add(sBucklingLengthFactors);
        }
    }
}
