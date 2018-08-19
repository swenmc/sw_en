using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;

namespace FEM_CALC_BASE
{
    public class CMLoadPart_AXIAL_F : CMLoad
    {
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoadPart_AXIAL_F()
        { }

        public CMLoadPart_AXIAL_F(CMLoad Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            if (Load is CMLoad_11)
            {
                GetMLoadPart_11((CMLoad_11)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_12)
            {
                GetMLoadPart_12((CMLoad_12)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_21)
            {
                GetMLoadPart_21((CMLoad_21)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_22)
            {
                GetMLoadPart_22((CMLoad_22)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_23)
            {
                GetMLoadPart_23((CMLoad_23)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_24)
            {
                GetMLoadPart_24((CMLoad_24)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_31)
            {
                GetMLoadPart_31((CMLoad_31)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_32)
            {
                GetMLoadPart_32((CMLoad_32)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_33)
            {
                GetMLoadPart_33((CMLoad_33)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_34)
            {
                GetMLoadPart_34((CMLoad_34)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_35)
            {
                GetMLoadPart_35((CMLoad_35)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_36)
            {
                GetMLoadPart_36((CMLoad_36)Load, Member, eMType, out fA, out fB);
            }
            else if (Load is CMLoad_41)
            {
                GetMLoadPart_41((CMLoad_41)Load, Member, eMType, out fA, out fB);
            }
            else // Exception
            {
                fA = fB = float.MaxValue; // Temporary !!!!
            }
        }

        void GetMLoadPart_11(CMLoad_11 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained against axial direction displacement
                case Enums.EElemSuppType2D.eEl_00_0_:
                case Enums.EElemSuppType2D.eEl_0__00:
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_00_00_11_XX(Load, Member.FLength, out fA, out fB);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____11_XX(Load, Member.FLength, out fA, out fB);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_11_XX(Load, Member.FLength, out fA, out fB);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_12(CMLoad_12 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_21(CMLoad_21 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_22(CMLoad_22 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_23(CMLoad_23 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_24(CMLoad_24 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_31(CMLoad_31 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_32(CMLoad_32 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_33(CMLoad_33 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_34(CMLoad_34 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_35(CMLoad_35 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_36(CMLoad_36 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }

        void GetMLoadPart_41(CMLoad_41 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB)
        {
            // Temporary
            fA = fB = 0.0f;
        }






        #region End Reactions of Both Sides Restrained member / Reakcie obojstranne votknuteho nosnika

        void GetEIF_00_00_11_XX(CMLoad_11 Load, float fL, out float fA, out float fB)
        {
            fA = Load.FF * (float)Math.Pow(fL - Load.Fa, 2) / (float)Math.Pow(fL, 3) * (fL + 2f * Load.Fa);
            fB = Load.FF * (float)Math.Pow(Load.Fa, 2) / (float)Math.Pow(fL, 3) * (fL + 2f * (fL - Load.Fa));
        }
        void GetEIF_00_00_12_XX(CMLoad_12 Load, float fL, out float fA, out float fB)
        {
            fA = fB = Load.FF / 2f;
        }






        #endregion

        #region Reactions of One Side Supported and Other Side Unsupported Member

        void GetEIF_00____11_XX(CMLoad_11 Load, float fL, out float fA, out float fB)
        {
            fA = Load.FF;
            fB = 0.0f;
        }
        void GetEIF_00____12_XX(CMLoad_12 Load, float fL, out float fA, out float fB)
        {
            fA = Load.FF;
            fB = 0.0f;
        }







        #endregion

        #region Reactions of One Side Unsupported and Other Side Supported Member

        void GetEIF____00_11_XX(CMLoad_11 Load, float fL, out float fA, out float fB)
        {
            fA = 0.0f;
            fB = Load.FF;
        }
        void GetEIF____00_12_XX(CMLoad_12 Load, float fL, out float fA, out float fB)
        {
            // Temporary
            fA = 0.0f;
            fB = Load.FF;
        }


        #endregion
    }
}
