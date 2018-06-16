using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    public class CL
    {

        // CL_61 obCL_61 = new CL_61();
        CL_62 obCL_62 = new CL_62();
        CL_63 obCL_63 = new CL_63();

        // 6.2.3 Tension - Axial Resistance
        public float Get_fN_t_Rd(bool bHoleExist, int iConCat, float fA, float fA_net, float ff_y, float ff_u, float fGamma_M0, float fGamma_M2)
        {

            float fN_pl_Rd = obCL_62.Eq_66______(fA, ff_y, fGamma_M0);


            if (bHoleExist) // if holes exist
            {
                float fN_u_Rd = obCL_62.Eq_67______(fA_net, ff_u, fGamma_M2);

                // C
                if (iConCat == 2)
                {
                    float fN_net_Rd = obCL_62.Eq_68______(fA_net, ff_y, fGamma_M0);
                    return Math.Min(fN_pl_Rd, Math.Min(fN_u_Rd, fN_net_Rd));
                }

                return Math.Min(fN_pl_Rd, fN_u_Rd);
            }
            return fN_pl_Rd;
        }
        public float Get_fN_t_Rd(float fA, float ff_y, float fGamma_M0)
        {
            return obCL_62.Eq_66______(fA, ff_y, fGamma_M0);
        }

        // 6.2.4 Compression - Axial Resistance
        public float Get_fN_c_Rd(int iClass, float fA, float fA_eff, float ff_y, float fGamma_M0)
        {
            if (iClass < 4)
                return obCL_62.Eq_610_____(fA, ff_y, fGamma_M0);
            else
                return obCL_62.Eq_611_____(fA_eff, ff_y, fGamma_M0);
        }

        // 6.2.7 Torsion
        public float Get_fTau_t_open(float fT_t_Ed, float fI_t, float ft_max)
        {
            return fT_t_Ed / fI_t * ft_max;
        }
        public float Get_fTau_t_solid(float fT_t_Ed, float fW_t)
        {
            return fT_t_Ed / fW_t;
        }
        public float Get_fTau_t_solid(float fT_t_Ed, float fI_t, float ft_max)
        {
            return fT_t_Ed / fI_t * ft_max;
        }
        public float Get_fTau_t_close(float fT_t_Ed, float fA_k, float ft_min)
        {
            return fT_t_Ed / 2f * fA_k * ft_min;
        }


    }
}
