using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_G : CScrewArrangement
    {
        public CScrewArrangement_G() { }

        public CScrewArrangement_G(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fbX1, float fbX2, float fhY, float flZ)
        {
            // TODO
        }
    }
}
