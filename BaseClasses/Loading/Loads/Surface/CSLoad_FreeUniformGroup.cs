using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    public class CSLoad_FreeUniformGroup : CSLoad_Free
    {
        private List<CSLoad_FreeUniform> m_loadList;

        public List<CSLoad_FreeUniform> MLoadList
        {
            get { return m_loadList; }
            set { m_loadList = value; }
        }

        public CSLoad_FreeUniformGroup(
            ELoadCoordSystem eLoadCS_temp,
            ELoadDir eLoadDirection_temp,
            CPoint pControlPoint_temp,
            float[] fX_dimension,
            float fY_dimension,
            float[,] fValues,
            float m_fRotationX_deg_temp,
            float m_fRotationY_deg_temp,
            float m_fRotationZ_deg_temp,
            Color color_temp,
            bool bIsDisplayed,
            float fTime) : base(eLoadCS_temp, eLoadDirection_temp, bIsDisplayed, fTime)
        {
            MLoadList = new List<CSLoad_FreeUniform>(1);

            int indexDirection = 1; // 0-3
            for(int i = 0; i < fX_dimension.Length; i++)
            {
                float segment_x_dimension = fX_dimension[i];
                MLoadList.Add(new CSLoad_FreeUniform(eLoadCS_temp, eLoadDirection_temp, pControlPoint_temp, segment_x_dimension, fY_dimension, fValues[indexDirection, i], m_fRotationX_deg_temp, m_fRotationY_deg_temp, m_fRotationZ_deg_temp, color_temp, bIsDisplayed, fTime));
            }
        }
    }
}
