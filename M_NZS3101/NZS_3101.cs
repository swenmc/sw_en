using MATH;
using MATH.ARRAY;
using System;
using BaseClasses;


namespace M_NZS3101
{
    public class NZS_3101
    {
        // 5.2 Properties of concrete
        public float Eq_5_1_____(float ff_apostrophe_c, float fRho_density)
        {
            return (float)(4700f * Math.Sqrt(ff_apostrophe_c / 1e+6f) * Math.Pow(fRho_density / 2300f, 1.5f) * 1e+6f); // Eq. (5-1) // fE_c - output in [Pa]
        }
        public float Eq_5_2_____(float fLambda, float ff_apostrophe_c)
        {
            return (float)(0.38f * fLambda * Math.Sqrt(ff_apostrophe_c)); // Eq. (5-2) // ff_t
        }
        public float Eq_5_3_____(float fRho_density)
        {
            return Math.Min(0.4f + (0.6f * fRho_density / 2200f), 1f); // Eq. (5-3) // fLambda
        }
        public float Eq_5_4_____(float fLambda, float ff_apostrophe_c)
        {
            return 0.6f * fLambda * MathF.Sqrt(ff_apostrophe_c); // Eq. (5-4) // ff_r
        }
        public float Eq_9_1_area(float ff_apostrophe_c, float ff_y, float fb_w, float fd)
        {
            return Math.Min((MathF.Sqrt(ff_apostrophe_c) / (4f * ff_y)) * fb_w * fd, 1.4f * fb_w * fd / ff_y); // Eq. (9-1) // fA_s
        }
        public float Eq_9_1_ratio(float ff_apostrophe_c, float ff_y)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat bezrozmerne cislo [-]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa
            float ff_y_MPa = ff_y / 1000000f; // consider value in MPa

            //return Math.Min((MathF.Sqrt(ff_apostrophe_c) / (4f * ff_y)), 1.4f / ff_y); // Eq. (9-1) // Minimum reinforcement ratio
            return Math.Min((MathF.Sqrt(ff_apostrophe_c_MPa) / (4f * ff_y_MPa)), 1.4f / ff_y_MPa); // Eq. (9-1) // Minimum reinforcement ratio
        }
        public float Eq_9_4_____(float  fv_c, float fA_cv)
        {
            return fv_c * fA_cv; // Eq. (9-4) // fV_c
        }
        public float Eq_9_5_____(float fk_d, float fk_a, float fv_b)
        {
            return fk_d * fk_a * fv_b; // Eq. (9-4) // fv_c
        }
        public float Get_v_b_93934(float fp_w, float ff_apostrophe_c)
        {
            float ff_apostrophe_c_reduced = Math.Min(ff_apostrophe_c, 50e+6f);

            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat napatie v [Pa]
            float ff_apostrophe_c_reduced_MPa = ff_apostrophe_c_reduced / 1000000f; // consider value in MPa

            //return Math.Max(MathF.Min((0.07f + 10f * fp_w) * MathF.Sqrt(ff_apostrophe_c_reduced), 0.2f * MathF.Sqrt(ff_apostrophe_c_reduced)), 0.08f * MathF.Sqrt(ff_apostrophe_c_reduced)); // fv_b
            float res = Math.Max(MathF.Min((0.07f + 10f * fp_w) * MathF.Sqrt(ff_apostrophe_c_reduced_MPa), 0.2f * MathF.Sqrt(ff_apostrophe_c_reduced_MPa)), 0.08f * MathF.Sqrt(ff_apostrophe_c_reduced_MPa)); // fv_b
            return res * 1000000f;
        }
        public float Get_k_a_93934(float fMaximumAgregateSize_meter)
        {
            if (fMaximumAgregateSize_meter >= 0.019f) // 19 mm
                return 1.00f;
            else if (fMaximumAgregateSize_meter <= 0.010f) // 10 mm
                return 0.85f;
            else
                return (float)ArrayF.GetLinearInterpolationValuePositive(fMaximumAgregateSize_meter, new double[2] { 0.010, 0.019 }, new double[2] { 0.85, 1.00 });
        }
        public float Get_k_d_93934()
        {
            // TODO - dopracovat podla roznych podmienok a) - e) v 9.3.9.3.4
            return 1.00f;
        }
        public float Eq_9_10____(float ff_apostrophe_c, float fb_w, float fd)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat napatie v [N]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa
            float fb_w_mm = fb_w * 1000f; // consider value in mm
            float fd_mm = fd * 1000f; // consider value in mm

            //return 1f / 16f * MathF.Sqrt(ff_apostrophe_c) * fb_w * fd; // Eq. (9-10) // fV_s_min
            return 1f / 16f * MathF.Sqrt(ff_apostrophe_c_MPa) * fb_w_mm * fd_mm; // Eq. (9-10) // fV_s_min
        }
        public float Eq_12_4____(float fV_s, float fV_c)
        {
            return fV_s + fV_c; // Eq. (12-4) // fV_n
        }
        public float Get_V_c_12731(float fv_c, float fb_0, float fd)
        {
            return fv_c * fb_0 * fd; // fV_c
        }
        public float Eq_12_5____(float fV_asterix, float fPhi, float fV_n)
        {
            return Math.Abs(fV_asterix) / (fPhi * fV_n); // Eq. (12-5) // fV Design ratio
        }
        public float Eq_12_6____(float fk_ds, float fBeta_c, float ff_apostrophe_c)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat napatie v [Pa]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa
            //return 1f / 6f * fk_ds * (1f + (2f / fBeta_c)) * MathF.Sqrt(ff_apostrophe_c); // Eq. (12-6) // fv_c
            float res = 1f / 6f * fk_ds * (1f + (2f / fBeta_c)) * MathF.Sqrt(ff_apostrophe_c_MPa); // Eq. (12-6) // fv_c
            return res * 1000000f;
        }
        public float Eq_12_7____(float fk_ds, float fAlpha_s, float fd, float fb_0, float ff_apostrophe_c)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat napatie v [Pa]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa

            //return 1f / 6f * fk_ds * ((fAlpha_s * fd / fb_0) + 1f) * MathF.Sqrt(ff_apostrophe_c); // Eq. (12-7) // fv_c
            float res = 1f / 6f * fk_ds * ((fAlpha_s * fd / fb_0) + 1f) * MathF.Sqrt(ff_apostrophe_c_MPa); // Eq. (12-7) // fv_c
            return res * 1000000f;
        }
        public float Eq_12_8____(float fk_ds, float ff_apostrophe_c)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat napatie v [Pa]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa

            //return 1f / 3f * fk_ds * MathF.Sqrt(ff_apostrophe_c); // Eq. (12-8) // fv_c
            float res = 1f / 3f * fk_ds * MathF.Sqrt(ff_apostrophe_c_MPa); // Eq. (12-8) // fv_c
            return res * 1000000f;
        }
        public float Eq_12_9____(float ff_apostrophe_c)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat napatie v [Pa]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa

            //return 1 / 6f * MathF.Sqrt(ff_apostrophe_c); // Eq. (12-9) // fv_c
            float res = 1 / 6f * MathF.Sqrt(ff_apostrophe_c_MPa); // Eq. (12-9) // fv_c
            return res * 1000000f;
        }
        public float Eq_12_9____(float fv_n, float fv_c, float fb_x, float fd)
        {
            return (fv_n - fv_c) * fb_x * fd; // Eq. (12-10) // fV_s
        }
        public float Get_k_ds_12732(float fd)
        {
            return Math.Min(Math.Max(0.5f, MathF.Sqrt(200f / (fd * 1000f))), 1f); // fk_ds - note: d in [mm]
        }

        // 17.5.6 Strength of cast-in anchors
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
            if (Math.Abs(fV_asterix) <= 0.2f * fPhi_V * fV_n)
                return Math.Abs(fN_asterix) / (fPhi_N * fN_n); // 17.5.6.6 (a)
            else if (Math.Abs(fN_asterix) <= 0.2f * fPhi_N * fN_n)
                return Math.Abs(fV_asterix) / (fPhi_V * fV_n); // 17.5.6.6 (b)
            else
                return (Math.Abs(fN_asterix) / (fPhi_N * fN_n) + Math.Abs(fV_asterix) / (fPhi_V * fV_n)) / 1.2f; // 17.5.6.6 (c) Eq. (17-5)
        }
        public float Eq_17566___(float fN_asterix, float fPhi_fN_n, float fV_asterix, float fPhi_fV_n)
        {
            if (Math.Abs(fV_asterix) <= 0.2f * fPhi_fV_n)
                return Math.Abs(fN_asterix) / fPhi_fN_n; // 17.5.6.6 (a)
            else if (Math.Abs(fN_asterix) <= 0.2f * fPhi_fN_n)
                return Math.Abs(fV_asterix) / fPhi_fV_n; // 17.5.6.6 (b)
            else
                return (Math.Abs(fN_asterix) / fPhi_fN_n + Math.Abs(fV_asterix) / fPhi_fV_n) / 1.2f; // 17.5.6.6 (c) Eq. (17-5)
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
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat silu v [N]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa
            float fh_ef_mm = fh_ef * 1000f;  // consider value in mm 

            //return (float)(fk * fLambda * Math.Sqrt(ff_apostrophe_c) * Math.Pow(fh_ef, 1.5f)); // Eq. (17-9) // fN_b
            return (float)(fk * fLambda * Math.Sqrt(ff_apostrophe_c_MPa) * Math.Pow(fh_ef_mm, 1.5f)); // Eq. (17-9) // fN_b
        }
        public float Eq_17_9a___(float fLambda, float ff_apostrophe_c, float fh_ef)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat silu v [N]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa
            float fh_ef_mm = fh_ef * 1000f;  // consider value in mm 

            //return (float)(3.9f * fLambda * Math.Sqrt(ff_apostrophe_c) * Math.Pow(fh_ef, 5f/3f)); // Eq. (17-9(a)) // fN_b
            return (float)(3.9f * fLambda * Math.Sqrt(ff_apostrophe_c_MPa) * Math.Pow(fh_ef_mm, 5f / 3f)); // Eq. (17-9(a)) // fN_b
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
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat silu v [N]
            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa
            float fc_1_mm = fc_1 * 1000f;  // consider value in mm 
            float fA_brg_mm2 = fA_brg * 1000000f;  // consider value in mm^2

            //return 13.3f * fk_1 * fc_1 * fLambda * MathF.Sqrt(fA_brg * ff_apostrophe_c); // Eq. (17-13) // fN_sb
            return 13.3f * fk_1 * fc_1_mm * fLambda * MathF.Sqrt(fA_brg_mm2 * ff_apostrophe_c_MPa); // Eq. (17-13) // fN_sb
        }
        public float Get_k_1____(float fc_1, float fc_2)
        {
            if (fc_2 >= 3f * fc_1)
                return 1.0f;
            else //if (fc_2 < 3f * fc_1)
                return (1 + (fc_2 / fc_1)) / 4f;
        }
        public float Eq_17_14___(float iNumber, float fA_se, float ff_ut, float ff_y)
        {
            if (ff_ut < Math.Min(1.9f * ff_y, 860e+6f))
                return iNumber * fA_se * ff_ut; // Eq. (17-14) // fV_s
            else
                throw new Exception("Tensile strength fut is more than 1.9fy or 860 MPa, see 17.5.8.1(b).");
        }
        public float Eq_17_15___(float iNumber, float fA_se, float ff_ut, float ff_y)
        {
            if(ff_ut < Math.Min(1.9f * ff_y, 860e+6f))
                return iNumber * 0.6f * fA_se * ff_ut; // Eq. (17-15) // fV_s
            else
                throw new Exception("Tensile strength fut is more than 1.9fy or 860 MPa, see 17.5.8.1(b).");
        }
        public float Eq_17_16___(float fA_v, float fA_vo, float fPsi_5, float fPsi_6, float fPsi_7, float fV_b)
        {
            return fA_v / fA_vo * fPsi_5 * fPsi_6 * fPsi_7 * fV_b; // Eq. (17-16) // fV_cb
        }
        public float Eq_17_17a__(float fk_2, float fl, float fd_o, float fLambda, float ff_apostrophe_c, float fc_1)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat silu v [N]

            float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa
            float fdiameterForSQRT_mm = fd_o * 1000f; // consider value in mm
            float fc_1_mm = fc_1 * 1000f;  // consider value in mm 
            return (float)(fk_2 * Math.Pow(fl / fd_o, 0.2f) * fLambda * Math.Sqrt(/*fd_o * ff_apostrophe_c*/ fdiameterForSQRT_mm * ff_apostrophe_c_MPa) * Math.Pow(/*fc_1*/ fc_1_mm, 1.5f)); // Eq. (17-17(a)) // fV_b
        }
        public float Eq_17_17b__(float fLambda, float ff_apostrophe_c, float fc_1)
        {
            // Oprava 30.1.2020 - nesedeli vysledky po odmocneni a mocneni - malo by to vracat silu v [N]
            //float ff_apostrophe_c_MPa = ff_apostrophe_c / 1000000f; // consider value in MPa
            float fc_1_mm = fc_1 * 1000f;  // consider value in mm 

            return (float)(3.8f * fLambda * Math.Sqrt(ff_apostrophe_c) * Math.Pow(/*fc_1*/ fc_1_mm, 1.5f)); // Eq. (17-17(b)) // fV_b
        }
        public float Eq_17_18___(float fc_1, float fe_apostrophe_v, float fs)
        {
            if (MathF.d_equal(fs, 0) || fe_apostrophe_v < 0.5f * fs) // s can be zero for single anchor
                return Math.Min(1f + ((2f * fe_apostrophe_v) / (3f * fc_1)), 1f); // Eq. (17-18) // fPsi_5
            else
                throw new Exception("Condition is not fulfilled, see Eq. 17-18.");
        }
        public float Get_Psi_6__(float fc_1, float fc_2)
        {
            if (fc_2 >= 1.5f * fc_1)
                return 1.0f; // Eq. (17-19) // fPsi_6
            else //if (fc_2 < 1.5f * fc_1)
                return 0.7f + 0.3f * (fc_2 / (1.5f * fc_1)); // Eq. (17-20) // fPsi_6
        }
        public float Eq_17_21___(float fA_v, float fA_vo, float fPsi_5, float fPsi_7, float fV_b)
        {
            return 2f * fA_v / fA_vo * fPsi_5 * fPsi_7 * fV_b; // Eq. (17-21) // fV_cb
        }
        public float Eq_17_22___(float fk_cp, float fN_cb)
        {
            return fk_cp * fN_cb; // Eq. (17-22) // fV_cp
        }
        public float Get_k_cp___(float fh_ef)
        {
            if (fh_ef < 0.065f)
                return 1.0f;
            else
                return 2.0f;
        }

        public float Eq_C17566__(float fN_asterix, float fPhi_fN_n, float fV_asterix, float fPhi_fV_n)
        {
            // Figure C17.1 – Shear and tensile load interaction equation
            return (float)(Math.Pow(Math.Abs(fN_asterix) / fPhi_fN_n , 5f / 3f) + Math.Pow(Math.Abs(fV_asterix) / fPhi_fV_n, 5f / 3f));
        }
    }
}
