using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    [Serializable]
    public abstract class CPlate_Frame : CPlate
    {
        private float m_fe_min_x; // Minimalna vzdialenost skrutiek - smer x

        public float e_min_x
        {
            get
            {
                return m_fe_min_x;
            }

            set
            {
                m_fe_min_x = value;
            }
        }

        private float m_fe_min_y; // Minimalna vzdialenost skrutiek - smer y

        public float e_min_y
        {
            get
            {
                return m_fe_min_y;
            }

            set
            {
                m_fe_min_y = value;
            }
        }

        public void SetMinimumScrewToEdgeDistances_Basic(CScrewArrangement screwArrangement)
        {
            e_min_x = 0;
            e_min_y = 0;

            if (screwArrangement != null && screwArrangement.HolesCentersPoints2D != null && screwArrangement.HolesCentersPoints2D.Length > 0)
            {
                e_min_x = float.MaxValue;
                e_min_y = float.MaxValue;

                for (int i = 0; i < screwArrangement.HolesCentersPoints2D.Length; i++)
                {
                    // Minimum edge distances - zadane v suradnicovom smere plechu
                    e_min_x = Math.Min(e_min_x, (float)screwArrangement.HolesCentersPoints2D[i].X);
                    e_min_y = Math.Min(e_min_y, (float)screwArrangement.HolesCentersPoints2D[i].Y);
                }
            }
        }

        public virtual void SetMinimumScrewToEdgeDistances(CScrewArrangement screwArrangement)
        {
            SetMinimumScrewToEdgeDistances_Basic(screwArrangement);
        }
        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            
        }
    }
}
