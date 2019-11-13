using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class CNut : CEntity3D
    {
        public float fA_accrosFlats;
        public float fB_accrosCorners;
        public float fC_headHeight;

        public CNut(string sTypeDiameter)
        {
            // TODO - vypracovat databazu

            /*

            DIA  PITCH  A ACROSS FLATS     B ACROSS CORNERS     C HEAD HEIGHT     E SHANK DIAMETER
                   MM       MAX MIN              MIN                  MAX             MAX MIN
            M5 0.8 8 7.78 9.2 3.5 5 4.82
            M6 1 10 9.78 11.5 4 6 5.82
            M8 1.25 13 12.73 15 5.3 8 7.78
            M10 1.5 16 15.73 18.4 6.4 10 9.78
            M12 1.75 18 17.73 20.7 7.5 12 11.73
            M14 2 21 20.67 24.2 8.8 14 13.73
            M16 2 24 23.67 27.7 10 16 15.73
            M18 2.5 27 26.67 31.2 11.5 18 17.73
            M20 2.5 30 29.67 34.6 12.5 20 19.67
            M22 2.5 34 33.38 39.3 14 22 21.67
            M24 3 36 35.38 41.6 15 24 23.67
            M27 3 41 40.38 47.3 16.7 27 26.67
            M30 3.5 46 45 53.1 18.7 30 29.67
            M33 3.5 50 49 57.7 20.5 33 32.61
            M36 4 55 53.8 63.5 22.5 36 35.61
             */

            // DOCASNE
            if(sTypeDiameter == "M16")
            {
                fA_accrosFlats = 24f / 1000f;
                fB_accrosCorners = 27.7f / 1000f;
                fC_headHeight = 10f / 1000f;
            }
            else if(sTypeDiameter == "M20")
            {
                fA_accrosFlats = 30f / 1000f;
                fB_accrosCorners = 34.6f / 1000f;
                fC_headHeight = 12.5f / 1000f;
            }
            else
            {
                throw new Exception("Not implemented!");
            }
        }
    }
}
