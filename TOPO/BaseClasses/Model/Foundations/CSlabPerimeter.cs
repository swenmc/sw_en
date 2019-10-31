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
    public class CSlabPerimeter : CEntity3D
    {
        private string m_BuildingSide;
        private float m_PerimeterDepth;
        private float m_PerimeterWidth;
        private float m_StartersLapLength;
        private float m_StartersSpacing;
        private float m_Starters_Phi;

        private float m_Longitud_Reinf_TopAndBotom_Phi;
        private float m_Longitud_Reinf_Intermediate_Phi;
        private int m_Longitud_Reinf_Intermediate_Count;

        private List<CSlabRebate> m_SlabRebates;

        //-------------------------------------------------------------------------------------------------------------
        public string BuildingSide
        {
            get
            {
                return m_BuildingSide;
            }

            set
            {
                m_BuildingSide = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterDepth
        {
            get
            {
                return m_PerimeterDepth;
            }

            set
            {
                m_PerimeterDepth = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PerimeterWidth
        {
            get
            {
                return m_PerimeterWidth;
            }

            set
            {
                m_PerimeterWidth = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersLapLength
        {
            get
            {
                return m_StartersLapLength;
            }

            set
            {
                m_StartersLapLength = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float StartersSpacing
        {
            get
            {
                return m_StartersSpacing;
            }

            set
            {
                m_StartersSpacing = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Starters_Phi
        {
            get
            {
                return m_Starters_Phi;
            }

            set
            {
                m_Starters_Phi = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Longitud_Reinf_TopAndBotom_Phi
        {
            get
            {
                return m_Longitud_Reinf_TopAndBotom_Phi;
            }

            set
            {
                m_Longitud_Reinf_TopAndBotom_Phi = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Longitud_Reinf_Intermediate_Phi
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Phi;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Phi = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int Longitud_Reinf_Intermediate_Count
        {
            get
            {
                return m_Longitud_Reinf_Intermediate_Count;
            }

            set
            {
                m_Longitud_Reinf_Intermediate_Count = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public List<CSlabRebate> SlabRebates
        {
            get
            {
                return m_SlabRebates;
            }

            set
            {
                m_SlabRebates = value;
            }
        }

        public CSlabPerimeter(int id,
           string side,
           float perimeterDepth,
           float perimeterWidth,
           float startersLapLength,
           float startersSpacing,
           float starters_Phi,
           float longitud_Reinf_TopAndBotom_Phi,
           float longitud_Reinf_Intermediate_Phi,
           int longitud_Reinf_Intermediate_Count,
           bool bIsDiplayed_temp,
           int fTime,
           List<CSlabRebate> slabRebates = null)
        {
            m_BuildingSide = side;
            m_PerimeterDepth = perimeterDepth;
            m_PerimeterWidth = perimeterWidth;
            m_StartersLapLength = startersLapLength;
            m_StartersSpacing = startersSpacing;
            m_Starters_Phi = starters_Phi;
            m_Longitud_Reinf_TopAndBotom_Phi = longitud_Reinf_TopAndBotom_Phi;
            m_Longitud_Reinf_Intermediate_Phi = longitud_Reinf_Intermediate_Phi;
            m_Longitud_Reinf_Intermediate_Count = longitud_Reinf_Intermediate_Count;
            BIsDisplayed = bIsDiplayed_temp;
            FTime = fTime;
            m_SlabRebates = slabRebates;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is CSlabPerimeter)) return false;

            CSlabPerimeter perimeter = (CSlabPerimeter)obj;
            if (MathF.d_equal(PerimeterDepth, perimeter.PerimeterDepth) &&
                MathF.d_equal(PerimeterWidth, perimeter.PerimeterWidth) &&
                MathF.d_equal(StartersLapLength, perimeter.StartersLapLength) &&
                MathF.d_equal(Starters_Phi, perimeter.Starters_Phi) &&
                MathF.d_equal(StartersSpacing, perimeter.StartersSpacing) &&
                MathF.d_equal(Longitud_Reinf_TopAndBotom_Phi, perimeter.Longitud_Reinf_TopAndBotom_Phi) &&
                MathF.d_equal(Longitud_Reinf_Intermediate_Phi, perimeter.Longitud_Reinf_Intermediate_Phi) &&
                Longitud_Reinf_Intermediate_Count == perimeter.Longitud_Reinf_Intermediate_Count) return true;
            else return false;
        }

        public bool EqualsWithRebates(object obj)
        {
            if (!base.Equals(obj)) return false;

            if (obj == null) return false;
            if (!(obj is CSlabPerimeter)) return false;

            CSlabPerimeter perimeter = (CSlabPerimeter)obj;
            if (SlabRebates.FirstOrDefault() == null) return false;
            if (perimeter.SlabRebates.FirstOrDefault() == null) return false;

            return MathF.d_equal( SlabRebates.FirstOrDefault().RebateWidth, perimeter.SlabRebates.FirstOrDefault().RebateWidth);
        }
    }
}
