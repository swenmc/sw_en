using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_BASE.Concrete
{
    public class MatTemp
    {
        // Concrete
        // Table of Strength
        int[,] ConcreteMat = new int[14, 2] 
                {
{12, 15  }, // C12/15
{16, 20  }, // C16/20
{20, 25  }, // C20/25
{25, 30  }, // C25/30
{30, 37  }, // C30/37
{35, 45  }, // C35/45
{40, 50  }, // C40/50
{45, 55  }, // C45/55
{50, 60  }, // C50/60
{55, 67  }, // C55/67
{60, 75  }, // C60/70
{70, 85  }, // C70/85
{80, 95  }, // C80/95
{90, 105 }  // C90/105
                };

        public int Get_f_ck(short i)
        {
            return ConcreteMat[i, 0]; // f_ck
        }
        public int Get_f_ck_cube(short i)
        {
            return ConcreteMat[i, 1]; // f_ck_cube
        }


        public float m_ff_ck;
        public float m_ff_ck_cube;
        public float m_ff_cm;
        public float m_ff_ctm;
        public float m_ff_ctk_005;
        public float m_ff_ctk_095;
        public float m_fE_cm;
        public float m_fEps_c1;
        public float m_fEps_cu1;
        public float m_fEps_c2;
        public float m_fEps_cu2;
        public float m_fn;
        public float m_fEps_c3;
        public float m_fEps_cu3;

        public float Get_f_cm()
        {
            return m_ff_ck + 8f;// (MPa)
        }

        public float Get_f_ctm()
        {
            if (m_ff_ck <= 50f)
                return 0.30f * (float)Math.Pow(m_ff_ck, 2 / 3f); // <= C50/60	
            else
               return 2.12f * (float)Math.Log(1 + (m_ff_cm / 10f), Math.E); // > C50/60
        }
        public float Get_f_ctk_005()
        {
            return 0.7f * m_ff_ctm;// 5% kvantil			
        }
        public float Get_f_ctk_095()
        {
            return 1.3f * m_ff_ctm; // 95% kvantil
        }
        public float Get_E_cm()
        {
            return 22 * (float)Math.Pow(m_ff_cm / 10f, 0.3f); // E_cm (fcm v MPa)			
        }
        public float Get_Eps_c1()
        {
            return Math.Min(0.7f * (float)Math.Pow(m_ff_cm, 0.31f), 2.8f); // Eps_c1 (0/00) 
        }
        public float Get_Eps_cu1()
        {
            if (m_ff_ck >= 50f)
            return 2.8f + 27 * MathF.Pow4((98f - m_ff_cm) / 100f);  // Eps_cu1 (0/00)
            return 3.5f;
        }
        public float Get_Eps_c2()
        {
            if (m_ff_ck >= 50f)
                return 2.0f + 0.085f* (float)Math.Pow(m_ff_ck - 50f, 0.53f); // Eps_c2 0/00)
            return 2f;
        }
        public float Get_Eps_cu2()
        {
            if (m_ff_ck >= 50f)
                return 2.6f + 35f * MathF.Pow4((90f - m_ff_ck) / 100f);  // Eps_cu2 (0/00)
            return 3.5f;
        }
        public float Get_fn()
        {
            if (m_ff_ck >= 50f)
                return 1.4f + 23.4f * MathF.Pow4((90f - m_ff_ck) / 100f); // (-)
            return 2f;
        }
        public float Get_Eps_c3()
        {
            if (m_ff_ck >= 50f)
                return 1.75f + 0.55f * ((m_ff_ck - 50f) / 40f); // Eps_c3 (0/00)
            return 1.75f;
        }
        public float Get_Eps_cu3()
        {
            if (m_ff_ck >= 50f)
                return 2.6f + 35f * MathF.Pow4((90f - m_ff_ck) / 100f); // Eps_cu3 (0/00)
            return 3.5f;
        }

        public void GetConData()
        {
        //m_ff_ck
        m_ff_cm = Get_f_cm();
        m_ff_ctm = Get_f_ctm();
        m_ff_ctk_005 = Get_f_ctk_005();
        m_ff_ctk_095 = Get_f_ctk_095();
        m_fE_cm = Get_E_cm();
        m_fEps_c1=Get_Eps_c1();
        m_fEps_cu1 = Get_Eps_cu1();
        m_fEps_c2 = Get_Eps_c2();
        m_fEps_cu2 = Get_Eps_cu2();
        m_fn=Get_fn();
        m_fEps_c3 = Get_Eps_c3();
        m_fEps_cu3 = Get_Eps_cu3();
        }



        // Reinforcement
        //Table of Strength		
        //fyk	ftk
        //MPa	MPa Reinforcement

        int[,] ReinfMat = new int [9, 2] {
	{400,420},   // B 400A
    {500,525},   // B 500A
	{600,630},   // B 600A
	{400,432},   // B 400B
	{500,540},   // B 500B
	{600,648},   // B 600B
	{400,460},   // B 400C
	{500,575},   // B 500C
	{600,690}    // B 600C
    };

        public int Get_Reinf_f_yk(short i)
        {
               return ReinfMat[i,0]; // f_yk
        }
        public int Get_Reinf_f_tk(short i)
        {
            return ReinfMat[i, 1]; // f_tk
        }

        // Table of Diameters
        float[] ReinfDiameters = new float[19] { 4.0f, 4.5f, 5.0f, 5.5f, 6.0f, 6.5f, 7.0f, 8.0f, 10f, 12f, 14f, 16f, 18f, 20f, 22f, 25f, 28f, 32f, 36f };

        public float Get_Reinf_d_s(short i)
        {
            return ReinfDiameters[i]; // d_s
        }
















    }
}
