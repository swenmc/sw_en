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

            //To Mato - dlho som hladal, ale problem bol nakoniec inde a to v loadIndices metode,ktora sa volala stale a nacitala nanovo TriangleIndices - ale asi len zo srandy tam nebola :-)
            // 3D Triangle Indices
            for (int i = 0; i < TriangleIndices.Count; i += 3)
            {
                //if (i != 0 && i != 3 && i != TriangleIndices.Count - 4 && i != TriangleIndices.Count - 7) continue;
                //int index1 = TriangleIndices[i];
                int index2 = TriangleIndices[i + 1];
                int index3 = TriangleIndices[i + 2];

                //TriangleIndices[i] = index1;
                TriangleIndices[i + 1] = index3;
                TriangleIndices[i + 2] = index2;
            }

            // BUG 638 - ?????? Pregenerovat screw arrangement v ramci funkcie alebo samostatne
            // Regenerate Screws

            if (ScrewArrangement != null)
            {
                // TO Ondrej - toto som dorobil, skus sa na to pozriet
                // Pripadne sa to moze aj presunutut ako samostatna funkcia niekam do Plate Helper ????
                // To Mato - zatial to nechame tu, ak sa to ma pouzit aj niekde inde, tak potom to mozme presuvat

                if (this is CConCom_Plate_JA)
                {
                    CConCom_Plate_JA plate = (CConCom_Plate_JA)this;
                    ScrewArrangement.Calc_ApexPlateData(0, plate.Fb_X, 0, plate.Fh_Y1, Ft, m_fSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_JB || this is CConCom_Plate_JBS)
                {
                    CConCom_Plate_JB plate = (CConCom_Plate_JB)this;
                    ScrewArrangement.Calc_ApexPlateData(0, plate.Fb_X, plate.Fl_Z, plate.Fh_Y1, Ft, m_fSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_JCS)
                {
                    CConCom_Plate_JCS plate = (CConCom_Plate_JCS)this;
                    ScrewArrangement.Calc_ApexPlateData(plate.LipBase_dim_x, plate.Fb_X1_AndLips, 0, plate.Fh_Y1, Ft, m_fSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_KA)
                {
                    CConCom_Plate_KA plate = (CConCom_Plate_KA)this;
                    ScrewArrangement.Calc_KneePlateData(plate.Fb_X1, plate.Fb_X2, 0, plate.Fh_Y1, Ft, FSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_KB || this is CConCom_Plate_KBS)
                {
                    CConCom_Plate_KB plate = (CConCom_Plate_KB)this;
                    ScrewArrangement.Calc_KneePlateData(plate.Fb_X1, plate.Fb_X2, 0, plate.Fh_Y1, Ft, FSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_KC || this is CConCom_Plate_KCS)
                {
                    CConCom_Plate_KC plate = (CConCom_Plate_KC)this;
                    ScrewArrangement.Calc_KneePlateData(plate.Fb_X1, plate.Fb_X2, plate.Fl_Z, plate.Fh_Y1, Ft, FSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_KD || this is CConCom_Plate_KDS)
                {
                    CConCom_Plate_KD plate = (CConCom_Plate_KD)this;
                    ScrewArrangement.Calc_KneePlateData(plate.Fb_X1, plate.Fb_X2, plate.Fl_Z, plate.Fh_Y1, Ft, FSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_KES)
                {
                    CConCom_Plate_KES plate = (CConCom_Plate_KES)this;
                    ScrewArrangement.Calc_KneePlateData(plate.Fb_X1, plate.Fb_X2, plate.Fl_Z, plate.Fh_Y1, Ft, FSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_KFS)
                {
                    CConCom_Plate_KFS plate = (CConCom_Plate_KFS)this;
                    ScrewArrangement.Calc_KneePlateData(plate.Fb_X1, plate.Fb_X2, plate.Fl_Z, plate.Fh_Y1, Ft, FSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_KGS)
                {
                    CConCom_Plate_KGS plate = (CConCom_Plate_KGS)this;
                    ScrewArrangement.Calc_KneePlateData(plate.Fb_X1, plate.Fb_X2, plate.Fl_Z, plate.Fh_Y1, Ft, FSlope_rad, !m_bScrewInPlusZDirection);
                }
                else if (this is CConCom_Plate_KHS)
                {
                    CConCom_Plate_KHS plate = (CConCom_Plate_KHS)this;
                    ScrewArrangement.Calc_KneePlateData(plate.Fb_X1, plate.Fb_X2, plate.Fl_Z, plate.Fh_Y1, Ft, FSlope_rad, !m_bScrewInPlusZDirection);
                }
                else
                {
                    // Plate KK - not to mirror or exception
                }
            }
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
