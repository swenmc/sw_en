using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    public class CL_55
    {
        float Get_k_Sigma_EC315_Tab41(float fSigma_A, float fSigma_B)
        {
            float fPsi;
            float fSigma_1 = Math.Max(-fSigma_A, -fSigma_B);
            float fSigma_2 = Math.Min(-fSigma_A, -fSigma_B);

            if (Math.Max(fSigma_1, fSigma_2) > 0)
            {
                fPsi = fSigma_2 / fSigma_1;
                fPsi = Math.Min(fPsi, 1f);

                if (fPsi == 1f)
                    return 4.0f;
                else if (fPsi > 0f && fPsi < 1f)
                    return 8.2f / (1.05f + fPsi);
                else if (fPsi == 0f)
                    return 7.81f;
                else if (fPsi > -1f && fPsi < 0f)
                    return 7.81f - 6.29f * fPsi + 9.78f * (float)Math.Pow(fPsi, 2f);
                else if (fPsi == -1f)
                    return 23.9f;
                else
                    return 5.98f * (float)Math.Pow(1f - fPsi, 2f);
            }
            else
                return 0f;
        }

        public void GetClassTab52_INT(float rSigma_A,
                              float rSigma_B,
                              float rSigma_N,
                              float rF_f_com,
                              float rF_w,
                              float rF_f_ten,
                              float rN_Ed,
                              float fc,
                              float ft,
                              float ff_yd,
                              float rEps,
                              int iPsi_Fix_Sigma_N,
                              bool bEps_Kl3_Sigma_com,
                              float rSigma_com_Ed,
                              float fct,
                              float fAlpha,
                              float fSigma_fyd_1,
                              float fSigma_fyd_2,
                              float fPsi,
                              float fLambda_1,
                              float fLambda_2,
                              float fLambda_3,
                              int iClass,
                              bool bStainlessS)
        {
            float rSigma_min = Math.Min(rSigma_A, rSigma_B);
            float rk_Sigma = Get_k_Sigma_EC315_Tab41(rSigma_A, rSigma_B);

            fct = fc / ft;

            if (rSigma_min < 0)
            {
                if (rF_w != 0f)
                {
                    fAlpha = 0.5f * (-rF_f_com + rF_f_ten + rF_w - rN_Ed) / rF_w;
                    fAlpha = Math.Max(0f, fAlpha);
                    fAlpha = Math.Min(1f, fAlpha);
                }
                else
                    fAlpha = 1f;

                float rSigma_M_c = Math.Min(rSigma_A, rSigma_B) - rSigma_N;
                float rSigma_M_t = Math.Max(rSigma_A, rSigma_B) - rSigma_N;

                if (iPsi_Fix_Sigma_N == 0)
                {
                    if (rSigma_M_c != 0f)
                    {
                        float rVielfaches_M = (ff_yd + rSigma_N) / -rSigma_M_c;

                        fSigma_fyd_1 = ff_yd;
                        fSigma_fyd_2 = -rSigma_N - rVielfaches_M * rSigma_M_t;
                    }
                    else
                    {
                        float rSigma_M = ff_yd + rSigma_N;

                        fSigma_fyd_1 = ff_yd;
                        fSigma_fyd_2 = -rSigma_N - rSigma_M;

                        if (rSigma_M_t == 0.0f)
                            fSigma_fyd_1 = fSigma_fyd_2 = ff_yd;
                    }
                }
                else
                {
                    float rVielfaches_M = ff_yd / (-rSigma_N - rSigma_M_c);

                    fSigma_fyd_1 = ff_yd;
                    fSigma_fyd_2 = rVielfaches_M * (-rSigma_N - rSigma_M_t);
                }

                fPsi = fSigma_fyd_2 / fSigma_fyd_1;

                fPsi = (fPsi > 1.0f) ? 1.0f : fPsi;

                if ((fSigma_fyd_2 - fSigma_fyd_1) == 0.0f)
                    fAlpha = 1.0f;


                if (bStainlessS)
                {
                    if (fAlpha == 0.5f)
                    {
                        fLambda_1 = 56.0f * rEps;
                        fLambda_2 = 58.2f * rEps;
                    }
                    else if (fAlpha == 1f)
                    {
                        fLambda_1 = 25.7f * rEps;
                        fLambda_2 = 26.7f * rEps;
                    }
                    else if (fAlpha > 0.5f)
                    {
                        fLambda_1 = 308.0f * rEps / (13f * fAlpha - 1f);
                        fLambda_2 = 320.0f * rEps / (13f * fAlpha - 1f);
                    }
                    else if (fAlpha > 0.0f)
                    {
                        fLambda_1 = 28.0f * rEps / fAlpha;
                        fLambda_2 = 29.1f * rEps / fAlpha;
                    }
                    else
                    {
                        fLambda_1 = float.MaxValue;
                        fLambda_2 = float.MaxValue;
                    }

                    if (fPsi == -1.0f)
                        fLambda_3 = 74.80f * rEps;
                    else if (fPsi == 1f)
                        fLambda_3 = 30.7f * rEps;
                    else
                        fLambda_3 = 15.3f * rEps * (float)Math.Sqrt(rk_Sigma);

                    if (bEps_Kl3_Sigma_com && rSigma_com_Ed > 0f)
                        fLambda_3 *= Math.Max((float)Math.Sqrt(ff_yd / rSigma_com_Ed), 1f);
                }
                else
                {
                    if (fAlpha == 0.5f)
                    {
                        fLambda_1 = 72.0f * rEps;
                        fLambda_2 = 83.0f * rEps;
                    }
                    else if (fAlpha == 1f)
                    {
                        fLambda_1 = 33.0f * rEps;
                        fLambda_2 = 38.0f * rEps;
                    }
                    else if (fAlpha > 0.5f)
                    {
                        fLambda_1 = 396.0f * rEps / (13f * fAlpha - 1f);
                        fLambda_2 = 456.0f * rEps / (13f * fAlpha - 1f);
                    }
                    else if (fAlpha > 0.0f)
                    {
                        fLambda_1 = 36.0f * rEps / fAlpha;
                        fLambda_2 = 41.5f * rEps / fAlpha;
                    }
                    else
                    {
                        fLambda_1 = float.MaxValue;
                        fLambda_2 = float.MaxValue;
                    }
                }

                if (fPsi == -1.0f)
                    fLambda_3 = 124.0f * rEps;
                else if (fPsi == 1f)
                    fLambda_3 = 42.0f * rEps;
                else if (fPsi > -1f)
                    fLambda_3 = 42.0f * rEps / (0.67f + 0.33f * fPsi);
                else
                    fLambda_3 = 62.0f * rEps * (1f - fPsi) * ((float)Math.Sqrt(-fPsi));


                if (bEps_Kl3_Sigma_com && rSigma_com_Ed > 0f)
                    fLambda_3 *= Math.Max((float)Math.Sqrt(ff_yd / rSigma_com_Ed), 1f);

                if (fct <= fLambda_1 && fct <= fLambda_3)
                    iClass = 1;
                else if (fct <= fLambda_2 && fct <= fLambda_3)
                    iClass = 2;
                else if (fct <= fLambda_3)
                    iClass = 3;
                else
                    iClass = 4;
            }
            else
                iClass = 0;
        }

        public void GetClassTab52_INT_GE(float fSigma_A,
                                    float fSigma_B,
                                    float fc,
                                    float ft,
                                    float fEps,
                                    float fct,
                                    float fPsi,
                                    float fLambda_3,
                                    int iClass)
        {
            float fSigma_min = Math.Min(fSigma_A, fSigma_B);

            if (fSigma_min < 0)
            {
                fPsi = Math.Max(fSigma_A, fSigma_B) / fSigma_min;
                fPsi = Math.Min(1f, fPsi);

                if (fPsi == -1.0f)
                    fLambda_3 = 124f * fEps;
                else if (fPsi == 1f)
                    fLambda_3 = 42f * fEps;
                else
                {
                    if (fPsi > -1f)
                        fLambda_3 = 42f * fEps / (0.67f + 0.33f * fPsi);
                    else
                        fLambda_3 = 62.0f * fEps * (1f - fPsi) * (float)Math.Sqrt(-fPsi);
                }

                fct = fc / ft;

                if (fct <= fLambda_3)
                    iClass = 3;
                else
                    iClass = 4;
            }
            else
                iClass = 0;
        }
        public void GetClassTab52_OUT(float fSigma_A,
                             float fSigma_B,
                             float fc,
                             float ft,
                             float fEps,
                             float fct,
                             float fLambda_1,
                             float fLambda_2,
                             float fLambda_3,
                             int iClass,
                             bool bStainlessS)
        {
             float fSigma_min = Math.Min(fSigma_A, fSigma_B);

            fct = fc / ft;

            if (fSigma_min < 0)
            {
                if (bStainlessS)
                {
                    fLambda_1 = 9.0f * fEps;
                    fLambda_2 = 9.4f * fEps;
                    fLambda_3 = 11.0f * fEps;
                }
                else
                {
                    fLambda_1 = 9.0f * fEps;
                    fLambda_2 = 10.0f * fEps;
                    fLambda_3 = 14.0f * fEps;
                }

                if (fct <= fLambda_1)
                    iClass = 1;
                else if (fct <= fLambda_2)
                    iClass = 2;
                else if (fct <= fLambda_3)
                    iClass = 3;
                else
                    iClass = 4;
            }
            else
                iClass = 0;
        }

        public void GetClassTab52_OUT_L(float fSigma_min,
                          float fh,
                          float fb,
                          float ft_a,
                          float ft_b,
                          float fr,
                          float fEps,
                          float ft,
                          float fc,
                          float fct,
                          float fht,
                          float fbh2t,
                          float fLambda_3,
                          float fLambda_3_a,
                          float fLambda_3_b,
                          int iClass,
                          bool bStainlessS)
        {
            if (fSigma_min < 0f)
            {
                if (bStainlessS)
                {
                    fLambda_3 = 11.0f * fEps;
                    fLambda_3_a = 11.9f * fEps;
                    fLambda_3_b = 9.1f * fEps;
                }
                else
                {
                    fLambda_3 = 14.0f * fEps;
                    fLambda_3_a = 15.0f * fEps;
                    fLambda_3_b = 11.5f * fEps;
                }

                float fct_h = (fh - ft_b - fr) / ft_a;
                float fct_b = (fb - ft_a - fr) / ft_b;

                if (fct_h > fct_b)
                {
                    ft = ft_a;
                    fc = fh - ft_b - fr;
                    fct = fct_h;
                }
                else
                {
                    ft = ft_b;
                    fc = fb - ft_a - fr;
                    fct = fct_b;
                }

                fht = Math.Max(fh / ft_a, fb / ft_b);
                fbh2t = (fb + fh) / (ft_a + ft_b);

                if (fct <= fLambda_3 && fht <= fLambda_3_a && fbh2t <= fLambda_3_b)
                    iClass = 3;
                else
                    iClass = 4;
            }
            else
                iClass = 0;
        }
        public void GetClassTab52_TU(float rSigma,
                            float rN_Ed,
                            float rd,
                            float ft,
                            float fEps,
                            float fdt,
                            float fLambda_1,
                            float fLambda_2,
                            float fLambda_3,
                            int iClass,
                            bool bStainlessS)
        {
            if (rSigma < 0)
            {
                fLambda_1 = 50f * (float)Math.Pow(fEps, 2f);
                fLambda_2 = 70f * (float)Math.Pow(fEps, 2f);

                if (bStainlessS && rN_Ed >= 0.0f)
                    fLambda_3 = 280f * (float)Math.Pow(fEps, 2f);
                else
                    fLambda_3 = 90f * (float)Math.Pow(fEps, 2f);

                fdt = rd / ft;

                if (fdt <= fLambda_1)
                    iClass = 1;
                else if (fdt <= fLambda_2)
                    iClass = 2;
                else if (fdt <= fLambda_3)
                    iClass = 3;
                else
                    iClass = 4;
            }
            else
                iClass = 0;
        }
    }
}
