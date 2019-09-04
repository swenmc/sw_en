using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CSlabRebate : CEntity3D
    {
        private float m_RebateWidth;      // Distance from the slab edge
        private float m_RebatePosition;   // Roller door start position + half of trimmer width
        private float m_RebateLength;     // Roller door clear width
        private float m_RebateDepth_Step; // Step size (10 mm)
        private float m_RebateDepth_Edge; // Total depth at the edge (20 mm)

        //-------------------------------------------------------------------------------------------------------------
        public float RebateWidth
        {
            get
            {
                return m_RebateWidth;
            }

            set
            {
                m_RebateWidth = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebatePosition
        {
            get
            {
                return m_RebatePosition;
            }

            set
            {
                m_RebatePosition = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateLength
        {
            get
            {
                return m_RebateLength;
            }

            set
            {
                m_RebateLength = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateDepth_Step
        {
            get
            {
                return m_RebateDepth_Step;
            }

            set
            {
                m_RebateDepth_Step = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RebateDepth_Edge
        {
            get
            {
                return m_RebateDepth_Edge;
            }

            set
            {
                m_RebateDepth_Edge = value;
            }
        }

        public CSlabRebate(int id,
           float rebateWidth,
           float rebatePosition,
           float rebateLength,
           float rebateDepth_Step,
           float rebateDepth_Edge,
           bool bIsDiplayed_temp,
           int fTime)
        {
            ID = id;
            m_RebateWidth = rebateWidth;
            m_RebatePosition = rebatePosition;
            m_RebateLength = rebateLength;
            m_RebateDepth_Step = rebateDepth_Step;
            m_RebateDepth_Edge = rebateDepth_Edge;
            BIsDisplayed = bIsDiplayed_temp;
            FTime = fTime;
        }
     }
}
