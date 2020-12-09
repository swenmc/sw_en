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

        float m_fSlope_rad;

        public float FSlope_rad
        {
            get
            {
                return m_fSlope_rad;
            }

            set
            {
                m_fSlope_rad = value;
            }
        }

        bool m_bScrewInPlusZDirection;

        public bool ScrewInPlusZDirection
        {
            get
            {
                return m_bScrewInPlusZDirection;
            }

            set
            {
                m_bScrewInPlusZDirection = value;
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

        // Mirror plate about XY-plane, screws are orientated in the Z-direction of in LCS of plate
        public void MirrorPlate()
        {
            // 2D mirror
            // TODO - doriesit ako budeme zrkadlit 2D, aby sme nepokazili 3D zrkadlenie
                /*
                for (int i = 0; i < ITotNoPointsin2D; i++)
                {
                    PointsOut2D[i].X *= -1;
                }

                if (ScrewArrangement != null)
                {
                    for (int i = 0; i < ScrewArrangement.IHolesNumber; i++)
                    {
                        ScrewArrangement.HolesCentersPoints2D[i].X *= -1;
                        arrConnectorControlPoints3D[i].X *= -1;
                    }
                }*/

            // 3D mirror

                for (int i = 0; i < ITotNoPointsin3D; i++)
                {
                    arrPoints3D[i].Z *= -1;
                }

            // 3D Triangle Indices
            for (int i = 0; i < TriangleIndices.Count; i+=3)
            {
                //int index1 = TriangleIndices[i];
                int index2 = TriangleIndices[i + 1];
                int index3 = TriangleIndices[i + 2];

                //TriangleIndices[i] = index1;
                TriangleIndices[i + 1] = index3;
                TriangleIndices[i + 2] = index2;
            }

            // BUG 638 - ?????? Pregenerovat screw arrangement v ramci funkcie alebo samostatne
            // Regenerate Screws
            /*
            if (ScrewArrangement != null)
            {
                if (ScrewArrangement is CScrewArrangementRect_PlateType_JKL)
                {
                    ScrewArrangement.Calc_HolesControlPointsCoord3D_FlatPlate(flZ, 0, Ft, !m_bScrewInPlusZDirection);
                    ((CScrewArrangementRect_PlateType_JKL)ScrewArrangement).GenerateConnectors_FlatPlate(!m_bScrewInPlusZDirection); // Opacny smer ako mal povodny plech
                }
            }
            */
        }

        public override void CopyParams(CPlate plate)
        {
            base.CopyParams(plate);

            //doplnit parametre specificke pre danu triedu

            if (plate is CPlate_Frame)
            {
                CPlate_Frame refPlate = (CPlate_Frame)plate;

                this.m_fe_min_x = refPlate.m_fe_min_x;
                this.m_fe_min_y = refPlate.m_fe_min_y;
            }
       }
    }
}
