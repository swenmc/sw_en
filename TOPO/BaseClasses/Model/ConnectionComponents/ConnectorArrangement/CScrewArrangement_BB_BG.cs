using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    public class CScrewArrangement_BB_BG:CScrewArrangement
    {
        // Po zapracovani anchor arrangement presunut
        private CAnchor m_referenceAnchor;

        public CAnchor referenceAnchor
        {
            get
            {
                return m_referenceAnchor;
            }

            set
            {
                m_referenceAnchor = value;
            }
        }

        public CScrewArrangement_BB_BG() { }

        public CScrewArrangement_BB_BG(int iScrewsNumber_temp, CScrew referenceScrew_temp, CAnchor referenceAnchor_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
            referenceAnchor = referenceAnchor_temp;
        }

        public void Calc_HolesCentersCoord2D(float fbX, float fhY, float flZ)
        {
            float fDistanceBetweenHoles = 0.5f * fhY; // Default

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];
                HolesCentersPoints2D[0] = new Point(flZ + 0.5f * fbX, 0.5f * fhY - 0.5f * fDistanceBetweenHoles);
                HolesCentersPoints2D[1] = new Point(HolesCentersPoints2D[0].X, 0.5f * fhY + 0.5f * fDistanceBetweenHoles);
            }
        }
    }
}
