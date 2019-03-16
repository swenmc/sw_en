using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;

namespace FEM_CALC_BASE
{
    public class CMLoadPart_TORS : CMLoad
    {
         //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoadPart_TORS()
        { }

        public CMLoadPart_TORS(CMLoad Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            if (Load is CMLoad_11)
            {
                GetMLoadPart_11((CMLoad_11)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_12)
            {
                GetMLoadPart_12((CMLoad_12)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_21)
            {
                GetMLoadPart_21((CMLoad_21)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_22)
            {
                GetMLoadPart_22((CMLoad_22)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_23)
            {
                GetMLoadPart_23((CMLoad_23)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_24)
            {
                GetMLoadPart_24((CMLoad_24)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_31)
            {
                GetMLoadPart_31((CMLoad_31)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_32)
            {
                GetMLoadPart_32((CMLoad_32)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_33)
            {
                GetMLoadPart_33((CMLoad_33)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_34)
            {
                GetMLoadPart_34((CMLoad_34)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_35)
            {
                GetMLoadPart_35((CMLoad_35)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_36)
            {
                GetMLoadPart_36((CMLoad_36)Load, Member, eMType, out fMa, out fMb);
            }
            else if (Load is CMLoad_41)
            {
                GetMLoadPart_41((CMLoad_41)Load, Member, eMType, out fMa, out fMb);
            }
            else // Exception
            {
                fMa = fMb = float.MaxValue; // Temporary !!!!
            }
        }

        void GetMLoadPart_11(CMLoad_11 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained against axial direction displacement
                    {
                        GetEIF_00_00_11_XX(Load, Member.FLength, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____11_XX(Load, Member.FLength, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                case Enums.EElemSuppType2D.eEl____00:
                     {
                        GetEIF____00_11_XX(Load, Member.FLength, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_12(CMLoad_12 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Load in the middle
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained against axial direction displacement
                    {
                        GetEIF_00_00_12_XX(Load, Member.FLength, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____12_XX(Load, Member.FLength, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_12_XX(Load, Member.FLength, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_21(CMLoad_21 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_22(CMLoad_22 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_23(CMLoad_23 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_24(CMLoad_24 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_31(CMLoad_31 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_32(CMLoad_32 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_33(CMLoad_33 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_34(CMLoad_34 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_35(CMLoad_35 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_36(CMLoad_36 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }

        void GetMLoadPart_41(CMLoad_41 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fMa, out float fMb)
        {
            // Temporary
            fMa = fMb = 0.0f;
        }






        #region End Reactions of Both Sides Restrained member / Reakcie obojstranne votknuteho nosnika

        void GetEIF_00_00_11_XX(CMLoad_11 Load, float fL, out float fMa, out float fMb)
        {
            fMa = Load.FF / 2.0f;
            fMb = fMa;
        }
        void GetEIF_00_00_12_XX(CMLoad_12 Load, float fL, out float fMa, out float fMb)
        {
            fMa = Load.FF / 2.0f;
            fMb = fMa;
        }






        #endregion

        #region Reactions of One Side Supported and Other Side Unsupported Member

        void GetEIF_00____11_XX(CMLoad_11 Load, float fL, out float fMa, out float fMb)
        {
            fMa = Load.FF;
            fMb = 0.0f;
        }
        void GetEIF_00____12_XX(CMLoad_12 Load, float fL, out float fMa, out float fMb)
        {
            fMa = Load.FF;
            fMb = 0.0f;
        }







        #endregion

        #region Reactions of One Side Unsupported and Other Side Supported Member

        void GetEIF____00_11_XX(CMLoad_11 Load, float fL, out float fMa, out float fMb)
        {
            fMa = 0.0f;
            fMb = Load.FF; // Sign ??
        }
        void GetEIF____00_12_XX(CMLoad_12 Load, float fL, out float fMa, out float fMb)
        {
            // Temporary
            fMa = 0.0f;
            fMb = Load.FF; // Sign ??
        }


        #endregion



        // Simply supported beam
        // Docasne, hodnoty reakcii zavisia od typu podopretia pruta - rozpracovane v projekte FEM_CALC
        public override float Get_SSB_SupportReactionValue_RA_Start(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_SupportReactionValue_RB_End(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_V_max(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_M_max(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_V_x(float fx, float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_M_x(float fx, float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_Delta_max(float fL, float fE, float fI)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_Delta_x(float fx, float fL, float fE, float fI)
        {
            throw new NotImplementedException();
        }

    }
}
