using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    public class CL_EF : CBase
    {
        public void GetEff_OUT(float fSigma_A,
                                            float fSigma_B,
                                            float rc,
                                            float rt,
                                            float fEps,
                                            float fSigma_1,
                                            float fSigma_2,
                                            float fPsi,
                                            float fk_Sigma,
                                            float fLambda_rel_p,
                                            float fRho,
                                            float fb_eff,
                                            float fb_red,
                                            float fb_em,
                                            bool bStainlessS)
        {
            if (fSigma_B <= fSigma_A)
            {
                fSigma_1 = -fSigma_B;
                fSigma_2 = -fSigma_A;
            }
            else
            {
                fSigma_1 = -fSigma_A;
                fSigma_2 = -fSigma_B;
            }

            if (Math.Max(fSigma_1, fSigma_2) > 0)
            {
                fPsi = fSigma_2 / fSigma_1;
                fPsi = Math.Min(fPsi, 1f);

                if (fSigma_B <= fSigma_A)
                {
                    if (fPsi == 1f)
                        fk_Sigma = 0.43f;
                    else if (fPsi == 0f)
                        fk_Sigma = 0.57f;
                    else if (fPsi == -1f)
                        fk_Sigma = 0.85f;
                    else
                        fk_Sigma = 0.57f - 0.21f * fPsi + 0.07f * (float)Math.Pow(fPsi, 2f);
                }
                else
                {
                    if (fPsi == 1f)
                        fk_Sigma = 0.43f;
                    else if (fPsi == 0f)
                        fk_Sigma = 1.7f;
                    else if (fPsi == -1f)
                        fk_Sigma = 23.8f;
                    else if (fPsi > 0f && fPsi < 1f)
                        fk_Sigma = 0.578f / (fPsi + 0.34f);
                    else
                        fk_Sigma = 1.7f - 5f * fPsi + 17.1f * (float)Math.Pow(fPsi, 2f);
                }

                fLambda_rel_p = rc / rt / (28.4f * fEps * (float)Math.Sqrt(fk_Sigma));

                if (fLambda_rel_p <= 0.748f)
                    fRho = 1.0f;
                else
                {
                    if (bStainlessS)
                        fRho = 1f / fLambda_rel_p - 0.242f / (float)Math.Pow(fLambda_rel_p, 2f);
                    else
                        fRho = (fLambda_rel_p - 0.188f) / (float)Math.Pow(fLambda_rel_p, 2f);

                    fRho = Math.Min(fRho, 1.0f);
                }

                if (fSigma_B <= fSigma_A)
                {
                    if (fPsi >= 0f && fPsi <= 1f)
                    {
                        fb_eff = fRho * rc;
                        fb_red = rc - fb_eff;
                        fb_em = fb_red / 2f;
                    }
                    else
                    {
                        fb_eff = fRho * rc / (1f - fPsi);
                        fb_red = (1f - fRho) * rc / (1f - fPsi);
                        fb_em = fb_red / 2f;
                    }
                }
                else
                {
                    if (fPsi >= 0f && fPsi <= 1f)
                    {
                        fb_eff = fRho * rc;
                        fb_red = rc - fb_eff;
                        fb_em = fb_red / 2f;
                    }
                    else
                    {
                        fb_eff = fRho * rc / (1f - fPsi);
                        fb_red = (1f - fRho) * rc / (1f - fPsi);
                        fb_em = rc - fb_eff - fb_red / 2f;
                    }
                }
            }
            else
            {
                fb_eff = rc;
                fb_red = 0f;
                fb_em = 0f;
            }
        }

        public void GetEff_INT(float fSigma_A,
                                      float fSigma_B,
                                      float rb_rel,
                                      float rt,
                                      float fEps,
                                      float fSigma_1,
                                      float fSigma_2,
                                      float fPsi,
                                      float fk_Sigma,
                                      float fLambda_rel_p,
                                      float fRho,
                                      float fb_eff,
                                      float fb_e1,
                                      float fb_e2,
                                      float fb_red,
                                      float fb_em,
                                      bool bStainlessS)
        {
            fSigma_1 = Math.Max(-fSigma_A, -fSigma_B);
            fSigma_2 = Math.Min(-fSigma_A, -fSigma_B);

            fPsi = fSigma_2 / fSigma_1;
            fPsi = Math.Min(fPsi, 1f);

            if (Math.Max(fSigma_1, fSigma_2) > 0)
            {
                if (fPsi == 1f)
                    fk_Sigma = 4.0f;
                else if (fPsi > 0f && fPsi < 1f)
                    fk_Sigma = 8.2f / (1.05f + fPsi);
                else if (fPsi == 0f)
                    fk_Sigma = 7.81f;
                else if (fPsi > -1f && fPsi < 0f)
                    fk_Sigma = 7.81f - 6.29f * fPsi + 9.78f * (float)Math.Pow(fPsi, 2f);
                else if (fPsi == -1f)
                    fk_Sigma = 23.9f;
                else
                    fk_Sigma = 5.98f * (float)Math.Pow(1f - fPsi, 2f);

                fLambda_rel_p = rb_rel / rt / (28.4f * fEps * (float)Math.Sqrt(fk_Sigma));

                if (fLambda_rel_p <= 0.5f + (float)Math.Sqrt(0.085f - 0.055f * fPsi))
                    fRho = 1.0f;
                else
                {
                    if (bStainlessS)
                        fRho = 0.772f / fLambda_rel_p - 0.125f / (float)Math.Pow(fLambda_rel_p, 2f);
                    else
                        fRho = (fLambda_rel_p - 0.055f * (3f + fPsi)) / (float)Math.Pow(fLambda_rel_p, 2f);

                    fRho = Math.Min(fRho, 1.0f);
                }

                if (fPsi == 1f)
                {
                    fb_eff = fRho * rb_rel;
                    fb_e1 = 0.5f * fb_eff;
                    fb_e2 = 0.5f * fb_eff;
                    fb_red = rb_rel - fb_eff;
                }
                else if (fPsi >= 0f && fPsi < 1f)
                {
                    fb_eff = fRho * rb_rel;
                    fb_e1 = 2f * fb_eff / (5f - fPsi);
                    fb_e2 = fb_eff - fb_e1;
                    fb_red = rb_rel - fb_eff;
                }
                else
                {
                    fb_eff = fRho * rb_rel / (1f - fPsi);
                    fb_e1 = 0.4f * fb_eff;
                    fb_e2 = 0.6f * fb_eff;
                    fb_red = (1f - fRho) * rb_rel / (1f - fPsi);
                }

                fb_em = fb_e1 + fb_red / 2f;

                if (fSigma_A > fSigma_B)
                    fb_em = rb_rel - fb_em;
            }
            else
            {
                fb_eff = rb_rel;
                fb_e1 = 0.5f * rb_rel;
                fb_e2 = fb_e1;
                fb_red = 0.0f;
                fb_em = fb_e1;
                fk_Sigma = 0.0f;
                fLambda_rel_p = 0.0f;
                fRho = 0.0f;
            }
        }
    }
}
