using System.Windows.Media;
using System.Windows.Media.Media3D;
using MATH;

namespace BaseClasses
{
    public class CMLoad_24 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        private float m_faA;     // Distance of Load from Member Start / User Input (distance between load start point and member start)
        private float m_fa;      // Distance of Midpoint(Centre) of Load from Member Start 
        private float m_fs;      // Distance of Load / Length along which load acts
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }
        public float FaA
        {
            get { return m_faA; }
            set { m_faA = value; }
        }
        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }
        public float Fs
        {
            get { return m_fs; }
            set { m_fs = value; }
        }

        private float m_fd_calc;
        private float m_fc_calc;
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_24()
        {


        }

        public CMLoad_24(float fq, float faA, float fs)
        {
            Fq = fq;
            FaA = faA; // Distance between load start point and member start
            Fs = fs;
            // Calculate Load Centre Position from Start of Member
            m_fa = m_faA + m_fs / 2.0f;

            m_fd_calc = m_faA + fs;
        }

        public CMLoad_24(int id_temp,
            float fq,
            float faA,
            float fs,
            EMLoadTypeDistr mLoadTypeDistr,
            ELoadType mLoadType,
            ELoadCoordSystem mLoadCS,
            ELoadDirection mLoadDir,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            FaA = faA; // Distance between load start point and member start
            Fs = fs;
            // Calculate Load Centre Position from Start of Member
            m_fa = m_faA + m_fs / 2.0f;

            m_fd_calc = m_faA + fs;

            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            ELoadCS = mLoadCS;
            ELoadDir = mLoadDir;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public CMLoad_24(int id_temp,
            float fq,
            float faA,
            float fs,
            CMember member_aux,
            EMLoadTypeDistr mLoadTypeDistr,
            ELoadType mLoadType,
            ELoadCoordSystem mLoadCS,
            ELoadDirection mLoadDir,
            bool bIsDislayed,
            int fTime)
        {
            ID = id_temp;
            Fq = fq;
            FaA = faA; // Distance between load start point and member start
            Fs = fs;
            // Calculate Load Centre Position from Start of Member
            m_fa = m_faA + m_fs / 2.0f;

            m_fd_calc = m_faA + fs;

            Member = member_aux;
            MLoadTypeDistr = mLoadTypeDistr;
            MLoadType = mLoadType;
            ELoadCS = mLoadCS;
            ELoadDir = mLoadDir;
            BIsDisplayed = bIsDislayed;
            FTime = fTime;
        }

        public override Model3DGroup CreateM_3D_G_Load(bool bConsiderCrossSectionDimensions, float fDisplayin3D_ratio)
        {
            return CreateUniformLoadSequence(Fq, FaA, Fs, bConsiderCrossSectionDimensions, fDisplayin3D_ratio);
        }

        // Partial load per beam segment - simply supported beam
        // Docasne, hodnoty reakcii zavisia od typu podopretia pruta - rozpracovane v projkte FEM_CALC
        public void Set_value_c(float fL)
        {
            m_fc_calc = fL - m_faA - m_fs;
        }

        public override float Get_SSB_SupportReactionValue_RA_Start(float fL)
        {
            Set_value_c(fL);
            return Fq * Fs / (2 * fL) * (Fs + 2 * m_fc_calc);
        }
        public override float Get_SSB_SupportReactionValue_RB_End(float fL)
        {
            return Fq * Fs / (2 * fL) * (2 * FaA + Fs);
        }
        public override float Get_SSB_V_max(float fL)
        {
            Set_value_c(fL);

            if (FaA < m_fc_calc)
                return Get_SSB_SupportReactionValue_RA_Start(fL);
            else
                return Get_SSB_SupportReactionValue_RB_End(fL);
        }
        public override float Get_SSB_M_max(float fL)
        {
            float fRA_Start = Get_SSB_SupportReactionValue_RA_Start(fL);

            return fRA_Start * (FaA + (fRA_Start / (2 * Fq)));
        }
        public override float Get_SSB_V_x(float fx, float fL)
        {
            float fRA_Start = Get_SSB_SupportReactionValue_RA_Start(fL);
            float fRB_End = Get_SSB_SupportReactionValue_RB_End(fL);

            if (fx <= FaA)
                return fRA_Start;
            else if (fx < m_fd_calc)
                return fRA_Start - Fq * (fx - FaA);
            else
                return -fRB_End;
        }
        public override float Get_SSB_M_x(float fx, float fL)
        {
            float fRA_Start = Get_SSB_SupportReactionValue_RA_Start(fL);
            float fRB_End = Get_SSB_SupportReactionValue_RB_End(fL);

            if (fx <=FaA)
                return fRA_Start * fx;
            else if(fx <m_fd_calc)
                return fRA_Start * fx - ((Fq / 2f) * MathF.Pow2(fx -FaA));
            else
                return fRB_End * (fL - fx);
        }
        public override float Get_SSB_Delta_max(float fL, float fE, float fI)
        {
            int iNumberOfSections = 51;
            float fDelta_max = 0;

            for (int i = 0; i < iNumberOfSections; i++)
            {
                float d = Get_SSB_Delta_x(i / (iNumberOfSections-1) * fL, fL, fE, fI);

                if (d > fDelta_max)
                    fDelta_max = d;
            }

            return fDelta_max;
        }
        public override float Get_SSB_Delta_x(float fx, float fL, float fE, float fI)
        {
            Set_value_c(fL);

            if (fx <= FaA)
                return (-Fq * Fs * (Fs + 2f * m_fc_calc) * fx / (48f * fE * fI * fL)) * (4 * (MathF.Pow2(fL) - MathF.Pow2(fx)) - MathF.Pow2(Fs + 2 * m_fc_calc) - MathF.Pow2(Fs));
            else if (fx < m_fd_calc)
                return (-Fq / (48f * fE * fI)) * (Fs * fx / fL * (4 * (Fs + 2 * m_fc_calc) * (MathF.Pow2(fL) - MathF.Pow2(fx)) - MathF.Pow3(Fs + 2 * m_fc_calc) - MathF.Pow2(Fs) * (Fs + 2 * m_fc_calc)) + 2 * MathF.Pow4(fx - FaA));
            else
                return (-Fq * Fs / (48 * fE * fI)) * ((fx / fL) * (Fs + 2 * m_fc_calc) * (4 * (MathF.Pow2(fL) - MathF.Pow2(fx)) - MathF.Pow2(Fs + 2 * m_fc_calc)) - MathF.Pow2(Fs) * (2 * m_fd_calc - Fs) * (1 - (fx / fL)) + MathF.Pow3(2 * fx - FaA - m_fd_calc));
        }
    }
}
