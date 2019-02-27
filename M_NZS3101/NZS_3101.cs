using MATH;
using MATH.ARRAY;
using System;
using BaseClasses;


namespace M_NZS_3101
{
    public class NZS_3101
    {
        public float Eq_17_1____(float fN_asterix, float fPhi, float fN_n)
        {
            return fN_asterix / (fPhi * fN_n); // Eq. (17-1) // fN Design ratio
        }
        public float Eq_17_2____(float fV_asterix, float fPhi, float fV_n)
        {
            return fV_asterix / (fPhi * fV_n); // Eq. (17-2) // fV Design ratio
        }
        public float Eq_17564___(bool bIsDuctileSteelInTension)
        {
            if (bIsDuctileSteelInTension)
                return 0.75f; // Eq. (17-3)
            else
                return 0.65f; // Eq. (17-4) (17-4(a)) (17-4(b))
        }
        public float Eq_17566___(float fN_asterix, float fPhi_N, float fN_n, float fV_asterix, float fPhi_V, float fV_n)
        {
            if (fV_asterix <= 0.2f * fPhi_V * fV_n)
                return fN_asterix / (fPhi_N * fN_n); // 17.5.6.6 (a)
            else if (fN_asterix <= 0.2f * fPhi_N * fN_n)
                return fV_asterix / (fPhi_V * fV_n); // 17.5.6.6 (b)
            else
                return (fN_asterix / (fPhi_N * fN_n) + fV_asterix / (fPhi_V * fV_n)) / 1.2f; // 17.5.6.6 (c) Eq. (17-5)
        }
        public float Eq_17_6____(int iNumber, float fA_se, float ff_ut)
        {
            return iNumber * fA_se * ff_ut; // Eq. (17-6) // fN_s
        }
        public float Eq_17_7____(float fPsi_1, float fPsi_2, float fPsi_3, float fA_n, float fA_no, float fN_b)
        {
            return fPsi_1 * fPsi_2 * fPsi_3 * fA_n / fA_no * fN_b; // Eq. (17-7) // fN_cb
        }
        public float Eq_17_8____(float fe_apostrophe_n, float fh_ef)
        {
            return Math.Min(1f / (1f + ((2f * fe_apostrophe_n) / (3f * fh_ef))), 1f); // Eq. (17-8) // fPsi_1
        }
        public float Get_Psi_2__(float fc_min, float fh_ef)
        {
            if (fc_min >= 1.5f * fh_ef)
                return 1.0f;
            else //if (fc_min < 1.5f * fh_ef)
                return 0.7f + 0.3f * fc_min / (1.5f * fh_ef);
        }
        public float Eq_17_9____(float fk, float fLambda, float ff_apostrophe_c, float fh_ef)
        {
            return (float)(fk * fLambda * Math.Sqrt(ff_apostrophe_c) * Math.Pow(fh_ef, 1.5f)); // Eq. (17-9) // fN_b
        }
        public float Eq_17_9a___(float fLambda, float ff_apostrophe_c, float fh_ef)
        {
            return (float)(3.9f * fLambda * Math.Sqrt(ff_apostrophe_c) * Math.Pow(fh_ef, 5f/3f)); // Eq. (17-9(a)) // fN_b
        }
        public float Eq_17_10___(float fPsi_4, float fN_p)
        {
            return fPsi_4 * fN_p; // Eq. (17-10) // fN_pn
        }
        public float Eq_17_11___(float ff_apostrophe_c, float fA_brg)
        {
            return 8f * ff_apostrophe_c * fA_brg; // Eq. (17-11) // fN_p
        }
        public float Eq_17_12___(float ff_apostrophe_c, float fe_h, float fd_o)
        {
            if (3f * fd_o <= fe_h && fe_h <= 4.5f * fd_o)
                return 0.9f * ff_apostrophe_c * fe_h * fd_o; // Eq. (17-12) // fN_p
            else
                throw new Exception("Distance eh is out of interval, see 17.5.7.3(b).");
        }
        public float Eq_17_13___(float fk_1, float fc_1, float fLambda, float fA_brg, float ff_apostrophe_c)
        {
            return 13.3f * fk_1 * fc_1 * fLambda * MathF.Sqrt(fA_brg * ff_apostrophe_c); // Eq. (17-13) // fN_sb
        }
        public float Get_k_1__(float fc_1, float fc_2)
        {
            if (fc_2 >= 3f * fc_1)
                return 1.0f;
            else //if (fc_2 < 3f * fc_1)
                return (1 + (fc_2 / fc_1)) / 4f;
        }
        public float Eq_17_14___(float iNumber, float fA_se, float ff_ut)
        {
            return iNumber * fA_se * ff_ut; // Eq. (17-14) // fV_s
        }
        public float Eq_17_15___(float iNumber, float fA_se, float ff_ut, float ff_y)
        {
            if(ff_ut < Math.Max(1.9f * ff_y, 860f))
                return iNumber * 0.6f * fA_se * ff_ut; // Eq. (17-15) // fV_s
            else
                throw new Exception("Tensile strength fut is more than 1.9fy or 860 MPa, see 17.5.8.1(b).");
        }






        // TODO - rovnice z NZS 3101 - IN WORK
    }
}
