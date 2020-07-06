using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_CB:CScrewArrangement
    {
        public CScrewArrangement_CB() { }

        public CScrewArrangement_CB(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fhY, float fx_edge, float fy_edge, float fx/*, float fy*/)
        {
            /*
            float fx_edge = 0.02f;  // x-direction
            float fy_edge = 0.02f;  // y-direction
            float fx = 0.06f;  // x-direction
            float fy = 0.06f;  // y-direction
            */

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                HolesCentersPoints2D[0] = new Point(fx_edge, fy_edge);

                HolesCentersPoints2D[1] = new Point(fx_edge, fhY - fy_edge);

                HolesCentersPoints2D[2] = new Point(fx_edge + fx, fy_edge);

                HolesCentersPoints2D[3] = new Point(fx_edge + fx, fhY - fy_edge);
            }
        }
    }
}
