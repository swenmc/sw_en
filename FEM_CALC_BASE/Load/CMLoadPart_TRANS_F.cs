using System;
using BaseClasses;
using MATH;
using BaseClasses.CRSC;

namespace FEM_CALC_BASE
{
    public class CMLoadPart_TRANS_F:CMLoad
    {
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoadPart_TRANS_F()
        { }


        public CMLoadPart_TRANS_F(CMLoad Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Type objType = Load.GetType(); // typeof(Load)

            if (Load is CMLoad_11)
            {
                GetMLoadPart_11((CMLoad_11)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_12)
            {
                GetMLoadPart_12((CMLoad_12)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_21)
            {
                GetMLoadPart_21((CMLoad_21)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_22)
            {
                GetMLoadPart_22((CMLoad_22)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_23)
            {
                GetMLoadPart_23((CMLoad_23)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_24)
            {
                GetMLoadPart_24((CMLoad_24)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_31)
            {
                GetMLoadPart_31((CMLoad_31)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_32)
            {
                GetMLoadPart_32((CMLoad_32)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_33)
            {
                GetMLoadPart_33((CMLoad_33)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_34)
            {
                GetMLoadPart_34((CMLoad_34)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_35)
            {
                GetMLoadPart_35((CMLoad_35)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_36)
            {
                GetMLoadPart_36((CMLoad_36)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_41)
            {
                GetMLoadPart_41((CMLoad_41)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_51z)
            {
               GetMLoadPart_51z((CMLoad_51z)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else if (Load is CMLoad_51y)
            {
                GetMLoadPart_51y((CMLoad_51y)Load, Member, eMType, out fA, out fB, out fMa, out fMb);
            }
            else // Exception
            {
                fA = fB = fMa = fMb = float.MaxValue; // Temporary !!!!
            }
       }
        
        


        void GetMLoadPart_11(CMLoad_11 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_11_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__11_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_11_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__11_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____11_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_11_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_12(CMLoad_12 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_12_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__12_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_12_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__12_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____12_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_12_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_21(CMLoad_21 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_21_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__21_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_21_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__21_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____21_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_21_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_22(CMLoad_22 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_22_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__22_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_22_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__22_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____22_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_22_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_23(CMLoad_23 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_23_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__23_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_23_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__23_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____23_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_23_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_24(CMLoad_24 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_24_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__24_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_24_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__24_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____24_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_24_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_31(CMLoad_31 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_31_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__31_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_31_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__31_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_0__00_31_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF_0__0__31_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_32(CMLoad_32 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_32_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__32_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_32_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__32_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_0__00_32_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF_0__0__32_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_33(CMLoad_33 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_33_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__33_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_33_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__33_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_0__00_33_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF_0__0__33_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_34(CMLoad_34 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_34_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__34_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_34_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__34_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_0__00_34_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF_0__0__34_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_35(CMLoad_35 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_35_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__35_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_35_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__35_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_0__00_35_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF_0__0__35_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_36(CMLoad_36 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_36_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__36_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_36_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__36_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_0__00_36_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF_0__0__36_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_41(CMLoad_41 Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_41_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__41_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_41_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__41_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_0__00_41_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF_0__0__41_UV(Load, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_51z(CMLoad_51z Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_51_UV(Load, Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__51_UV(Load, Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_51_UV(Load, Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__51_UV(Load, Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_00____51_UV(Load, Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF____00_51_UV(Load, Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

        void GetMLoadPart_51y(CMLoad_51y Load, CE_1D_BASE Member, FEM_CALC_BASE.Enums.EElemSuppType2D eMType, out float fA, out float fB, out float fMa, out float fMb)
        {
            switch (eMType)
            {
                case Enums.EElemSuppType2D.eEl_00_00: // Both Side restrained
                    {
                        GetEIF_00_00_51_UV(Load,  Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00_0_:
                    {
                        GetEIF_00_0__51_UV(Load,  Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__00:
                    {
                        GetEIF_0__00_51_UV(Load, Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_0__0_:
                    {
                        GetEIF_0__0__51_UV(Load,  Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl_00___:
                    {
                        GetEIF_0__00_51_UV(Load,  Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                case Enums.EElemSuppType2D.eEl____00:
                    {
                        GetEIF_0__0__51_UV(Load, Member.CrSc, Member.FLength, out fA, out fB, out fMa, out fMb);
                        return;
                    }
                default:
                    {
                        // Exception
                        fA = fB = fMa = fMb = 0.0f;
                        return;
                    }
            }
        }

    












        ///////////////////////////////////////////////////
        // Particular EIF not depending of direction -         // Pozri: Sobota Jan, Statika Stavebnych konstrukcii 2, Tab. 2.1

        //----------------------------------------------------------------------------------------------------------------------------
        // Obojstranne votknutie
        //----------------------------------------------------------------------------------------------------------------------------

        #region End Moments and Reactions of Both Sides Restrained member / Koncove momenty a reakcie votknuteho nosnika
        // Singular Load
        void GetEIF_00_00_11_UV(CMLoad_11 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = Load.FF * (float)Math.Pow(fL - Load.Fa, 2) / (float)Math.Pow(fL, 3) * (fL + 2f * Load.Fa);
            fB = Load.FF * (float)Math.Pow(Load.Fa, 2) / (float)Math.Pow(fL, 3) * (fL + 2f * (fL - Load.Fa));
            fMa = -Load.FF * Load.Fa * (float)Math.Pow(fL - Load.Fa, 2) / (float)Math.Pow(fL, 2);
            fMb = Load.FF * (float)Math.Pow(Load.Fa, 2) * (fL - Load.Fa) / (float)Math.Pow(fL, 2);
        }
        void GetEIF_00_00_12_UV(CMLoad_12 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = fB = Load.FF / 2f;

            fMa = -Load.FF * fL / 8f;
            fMb = -fMa; /*Load.FF * fL/8f;*/
        }
        // Uniform Load
        void GetEIF_00_00_21_UV(CMLoad_21 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = fB = Load.Fq * fL / 2f;
            fMa = -Load.Fq * fL * fL / 12f;
            fMb = -fMa; /*Load.Fq * fL * fL / 12f;*/
        }
        void GetEIF_00_00_22_UV(CMLoad_22 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = (Load.Fq * Load.Fa) / (2f * fL) * ((2f * fL * (fL * fL - Load.Fa * Load.Fa)) + (float)Math.Pow(Load.Fa, 3));
            fB = (Load.Fq * (float)Math.Pow(Load.Fa, 3) / (2f * (float)Math.Pow(fL, 3))) * (fL + (fL - Load.Fa));
            fMa = -(Load.Fq * Load.Fa * Load.Fa) / 12f * (float)Math.Pow(fL, 2) * (Load.Fa * Load.Fa + 4f * Load.Fa * (fL - Load.Fa) + 6f * (float)Math.Pow(fL - Load.Fa, 2));
            fMb = (Load.Fq * (float)Math.Pow(Load.Fa, 3) / (12f * fL * fL)) * (Load.Fa * +4f * (fL - Load.Fa));
        }
        void GetEIF_00_00_23_UV(CMLoad_23 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {

            // Otocenie

            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_00_24_UV(CMLoad_24 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = ((Load.Fq * Load.Fs) / (4f * (float)Math.Pow(fL, 3))) * (4f * ((float)Math.Pow(fL - Load.Fa, 2) * (3f * Load.Fa + (fL - Load.Fa)) + (float)Math.Pow(Load.Fs, 2) * (Load.Fa - (fL - Load.Fa))));
            fB = ((Load.Fq * Load.Fs) / (4f * (float)Math.Pow(fL, 3))) * (4f * (float)Math.Pow(Load.Fa, 2) * (Load.Fa + 3f * (fL - Load.Fa)) + (float)Math.Pow(Load.Fs, 2) * ((fL - Load.Fa) - Load.Fa));
            fMa = -(Load.Fq * Load.Fs / (12f * fL * fL)) * (12f * Load.Fa * (float)Math.Pow(fL - Load.Fa, 2) + (float)Math.Pow(Load.Fs, 2) * (Load.Fa - 2 * (fL - Load.Fa)));
            fMb = (Load.Fq * Load.Fs / (12f * fL * fL)) * (12f * (float)Math.Pow(Load.Fa, 2) * (fL - Load.Fa) + (float)Math.Pow(Load.Fs, 2) * ((fL - Load.Fa) - 2 * Load.Fa));
        }
        // Own mathematical class MathF used in functions written below
        // Triangular Load
        void GetEIF_00_00_31_UV(CMLoad_31 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = fB = Load.Fq * fL / 4f;
            fMa = -5f * Load.Fq * MathF.Pow2(fL) / 96f;
            fMb = -fMa;
        }
        void GetEIF_00_00_32_UV(CMLoad_32 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = 7f / 20f * Load.Fq * fL;
            fB = 3f / 20f * Load.Fq * fL;
            fMa = -Load.Fq * MathF.Pow2(fL) / 20f;
            fMb = Load.Fq * MathF.Pow2(fL) / 30f;
        }
        void GetEIF_00_00_33_UV(CMLoad_33 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = 3f / 20f * Load.Fq * fL;
            fB = 7f / 20f * Load.Fq * fL;
            fMa = -Load.Fq * MathF.Pow2(fL) / 30f;
            fMb = Load.Fq * MathF.Pow2(fL) / 20f;
        }
        void GetEIF_00_00_34_UV(CMLoad_34 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa - Load.Fs;
            fA = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(fb) * (3f * fL - 2f * fb) + 8f * Load.Fa * fb * Load.Fs + MathF.Pow2(Load.Fs) * (3f * Load.Fa + 5f * fb + 1.4f * Load.Fs));
            fB = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(Load.Fa) * (3f * fL - 2f * Load.Fa) + 4f * Load.Fa * fb * Load.Fs + MathF.Pow2(Load.Fs) * (3f * Load.Fa + fb + 0.6f * Load.Fs));
            fMa = -(Load.Fq * Load.Fs / (60f * MathF.Pow2(fL))) * (10f * MathF.Pow2(fb) * (3f * Load.Fa + Load.Fs) + MathF.Pow2(Load.Fs) * (15f * Load.Fa + 10f * fb + 3f * Load.Fs) + 40f * Load.Fa * fb * Load.Fs);
            fMb = (Load.Fq * Load.Fs / (60f * MathF.Pow2(fL))) * (10f * MathF.Pow2(Load.Fa) * (3f * fb + 2f * Load.Fs) + MathF.Pow2(Load.Fs) * (10f * Load.Fa + 5f * fb + 2f * Load.Fs) + 20f * Load.Fa * fb * Load.Fs);
        }
        void GetEIF_00_00_35_UV(CMLoad_35 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa - Load.Fs;
            fA = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(fb) * (3f * fL - 2f * fb) + 4f * Load.Fa * fb * Load.Fs + MathF.Pow2(Load.Fs) * (3f * fb + Load.Fa + 0.6f * Load.Fs));
            fB = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(Load.Fa) * (3f * fL - 2f * Load.Fa) + 8f * Load.Fa * fb * Load.Fs + MathF.Pow2(Load.Fs) * (3f * fb + 5f * Load.Fa + 1.4f * Load.Fs));
            fMa = (Load.Fq * Load.Fs / (60f * MathF.Pow2(fL))) * (10f * MathF.Pow2(fb) * (3f * Load.Fa + 2f * Load.Fs) + MathF.Pow2(Load.Fs) * (10f * fb + 5f * Load.Fa + 2f * Load.Fs) + 20f * Load.Fa * fb * Load.Fs);
            fMb = -(Load.Fq * Load.Fs / (60f * MathF.Pow2(fL))) * (10f * MathF.Pow2(Load.Fa) * (3f * fb + Load.Fs) + MathF.Pow2(Load.Fs) * (15f * fb + 10f * Load.Fa + 3f * Load.Fs) + 40f * Load.Fa * fb * Load.Fs);
        }
        void GetEIF_00_00_36_UV(CMLoad_36 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = (Load.Fq * Load.Fs / (2f * MathF.Pow3(fL))) * (2f * MathF.Pow2(fb) * (fL + 2f * Load.Fa) + MathF.Pow2(Load.Fs) * (Load.Fa - fb));
            fB = (Load.Fq * Load.Fs / (2f * MathF.Pow3(fL))) * (2f * MathF.Pow2(Load.Fa) * (fL + 2f * fb) + MathF.Pow2(Load.Fs) * (fb - Load.Fa));
            fMa = -(Load.Fq * Load.Fs / (6f * MathF.Pow2(fL))) * (6f * Load.Fa * MathF.Pow2(fb) + MathF.Pow2(Load.Fs) * (Load.Fa - 2f * fb));
            fMb = (Load.Fq * Load.Fs / (6f * MathF.Pow2(fL))) * (6f * MathF.Pow2(Load.Fa) * fb + MathF.Pow2(Load.Fs) * (fb - 2f * Load.Fa));
        }
        // Trapezoidal
        void GetEIF_00_00_41_UV(CMLoad_41 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - 2f * Load.Fa;
            fA = fB = (Load.Fq / 2f) * (fL - Load.Fa);
            fMa = -(Load.Fq / (12f * fL)) * (MathF.Pow3(fL) + MathF.Pow3(Load.Fa) - 2f * MathF.Pow2(Load.Fa) * fL);
            fMb = -fMa;
        }
        // Temperature
        void GetEIF_00_00_51_UV(CMLoad_51z Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = fB = 0f;
            fMa = -objCrSc.m_Mat.m_fE * (float)objCrSc.I_y * objCrSc.m_Mat.m_fAlpha_T * (Load.Ft_0_b - Load.Ft_0_u) / (float)objCrSc.h;
            fMb = -fMa;
        }
        void GetEIF_00_00_51_UV(CMLoad_51y Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = fB = 0f;
            fMa = -objCrSc.m_Mat.m_fE * (float)objCrSc.I_z * objCrSc.m_Mat.m_fAlpha_T * (Load.Ft_0_l - Load.Ft_0_r) / (float)objCrSc.b;
            fMb = -fMa;
        }
        #endregion


        //----------------------------------------------------------------------------------------------------------------------------
        // Jedna strana nosnika votknuta a druha jednoducho podopreta
        //----------------------------------------------------------------------------------------------------------------------------

        #region End Moments and Reactions of One Side Restraint and Other Side Simply Supported Member
        // OTOCENIE PODOPRETIA DOPRACOVAT !!!!

        void GetEIF_00_0__11_UV(CMLoad_11 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = Load.FF *fb / (2 * MathF.Pow3(fL)) * (3f * MathF.Pow2(fL) - MathF.Pow2(fb));
            fB = ((Load.FF * MathF.Pow2(Load.Fa)) / (2f * MathF.Pow3(fL))) * (2f * fL + fb);
            fMa = (Load.FF * fb * Load.Fa / (2 * MathF.Pow2(fL))) * (fb + fL);
            fMb = 0f;
        }
        void GetEIF_00_0__12_UV(CMLoad_12 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__21_UV(CMLoad_21 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // !!! Signs
            fA = 5 / 8f * Load.Fq * fL;
            fB = 3 / 8f * Load.Fq * fL;
            fMa = - Load.Fq * fL * fL / 8f;
            fMb = 0f;
        }
        void GetEIF_00_0__22_UV(CMLoad_22 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__23_UV(CMLoad_23 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__24_UV(CMLoad_24 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__31_UV(CMLoad_31 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__32_UV(CMLoad_32 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__33_UV(CMLoad_33 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }         
        void GetEIF_00_0__34_UV(CMLoad_34 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__35_UV(CMLoad_35 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__36_UV(CMLoad_36 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__41_UV(CMLoad_41 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__51_UV(CMLoad_51z Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00_0__51_UV(CMLoad_51y Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        #endregion


        //----------------------------------------------------------------------------------------------------------------------------
        // Jedna strana nosnika jednoducho podopreta a druha votknuta
        //----------------------------------------------------------------------------------------------------------------------------

        #region End Moments and Reactions of One Side Simply Supported and the Other Side Restraint Member
        // Singular Load
        void GetEIF_0__00_11_UV(CMLoad_11 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = ((Load.FF * MathF.Pow2(fb)) / (2f*MathF.Pow3(fL)))* (2f*fL + Load.Fa);
            fB = Load.FF * Load.Fa / (2 * MathF.Pow3(fL)) * (3f * MathF.Pow2(fL) - MathF.Pow2(Load.Fa));
            fMa = 0f;
            fMb = (Load.FF * Load.Fa * fb / (2 * MathF.Pow2(fL))) * (Load.Fa + fL);
        }
        void GetEIF_0__00_12_UV(CMLoad_12 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA  = 5f/16f*Load.FF;
            fB = 11f / 16f * Load.FF;
            fMa = 0f;
            fMb = 3f/16f*Load.FF*fL;
        }
        // Uniform Load
        void GetEIF_0__00_21_UV(CMLoad_21 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
             // !!! Signs
            fA = 3/8f * Load.Fq * fL;
            fB = 5/8f*Load.Fq * fL;
            fMa = 0f;
            fMb = Load.Fq * fL * fL / 8f;
        }
        void GetEIF_0__00_22_UV(CMLoad_22 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = (Load.Fq * Load.Fa) / (8f * MathF.Pow3(fL)) * (2f * MathF.Pow2(fL) * (Load.Fa + 4f * fb) + MathF.Pow3(Load.Fa));
            fB = (Load.Fq * MathF.Pow3(Load.Fa)) / (8f * MathF.Pow3(fL)) * (6f * MathF.Pow2(fL) -MathF.Pow2(Load.Fa));
            fMa = 0f;
            fMb = (Load.Fq * MathF.Pow2(Load.Fa)) / (8f * MathF.Pow2(fL)) * (2f*MathF.Pow2(fL) - MathF.Pow2(Load.Fa));
        }
        void GetEIF_0__00_23_UV(CMLoad_23 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = (Load.Fq * MathF.Pow3(fb) / (8f * MathF.Pow3(fL))) * (3f * fL + Load.Fa);
            fB = ((Load.Fq * fb) / (8f * MathF.Pow3(fL))) * (14f * (2f * MathF.Pow2(fL) - MathF.Pow2(fb)) + MathF.Pow3(fb));
            fMa = 0f;
            fMb = (Load.Fq * MathF.Pow2(fb) / 8f * MathF.Pow2(fL)) * (4f * Load.Fa * fL + MathF.Pow2(fb));
        }
        void GetEIF_0__00_24_UV(CMLoad_24 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = (Load.Fq * Load.Fs / (8f*MathF.Pow3(fL))) * (4f * MathF.Pow2(fb) * (2f*fL+Load.Fa) + Load.Fa * MathF.Pow2(Load.Fs));
            fB = (Load.Fq * Load.Fs * Load.Fa / (8f * MathF.Pow3(fL))) * (8f * MathF.Pow2(fL) + 4f * fb * (fL + Load.Fa) - MathF.Pow2(Load.Fs));
            fMa = 0f;
            fMb = (Load.Fq * Load.Fs * Load.Fa / (8f * MathF.Pow2(fL))) * (4f * fb * (fL + Load.Fa) - MathF.Pow2(Load.Fs));
        }
        // Triangular Load
        void GetEIF_0__00_31_UV(CMLoad_31 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = 11f / 64f * Load.Fq * fL;
            fB = 21f/64f * Load.Fq * fL;
            fMa = 0f;
            fMb = 5f/64f*Load.Fq*MathF.Pow2(fL);
        }
        void GetEIF_0__00_32_UV(CMLoad_32 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = 11f/40f * Load.Fq * fL;
            fB = 9f / 40f * Load.Fq * fL;
            fMa = 0f;
            fMb = 7f /120f * Load.Fq * MathF.Pow2(fL);
        }
        void GetEIF_0__00_33_UV(CMLoad_33 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = 1f / 10f * Load.Fq * fL;
            fB = 2f / 5f * Load.Fq * fL;
            fMa = 0f;
            fMb = 1f / 15f * Load.Fq * MathF.Pow2(fL);
        }
        void GetEIF_0__00_34_UV(CMLoad_34 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa - Load.Fs;
            fA = (Load.Fq * Load.Fs / (4f*MathF.Pow3(fL))) * (MathF.Pow2(fb) * (3f*Load.Fa + 2*fb + 5*Load.Fs) + MathF.Pow2(Load.Fs) * (1.5f * Load.Fa + 4f*fb + 1.1f * Load.Fs) + 4f* Load.Fa*fb*Load.Fs);
            fB = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f* MathF.Pow2(Load.Fa) * (Load.Fa + 3 * fb + 3 * Load.Fs) + MathF.Pow2(fb) * (3*Load.Fa + Load.Fs) + MathF.Pow2(Load.Fs)*(4.5f * Load.Fa + 2f * fb + 0.9f * Load.Fs) + 8f * Load.Fa * fb * Load.Fs);
            fMa = 0f;
            fMb = Load.Fq * Load.Fs / (12f * MathF.Pow2(fL)) * (MathF.Pow2(Load.Fa) * (6f * fb + 4f * Load.Fs) + MathF.Pow2(fb)*(3f * Load.Fa + Load.Fs) + MathF.Pow2(Load.Fs) * (3.5f * Load.Fa + 2f * fb + 0.7f * Load.Fs) + 8f * Load.Fa * fb * Load.Fs);
        }
        void GetEIF_0__00_35_UV(CMLoad_35 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa - Load.Fs;
            fA = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (MathF.Pow2(fb) * (3f * Load.Fa + 2 * fb + 4 * Load.Fs) + MathF.Pow2(Load.Fs) * (0.5f * Load.Fa + 2f * fb + 0.4f * Load.Fs) + 2f * Load.Fa * fb * Load.Fs);
            fB = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(Load.Fa) * (Load.Fa + 3 * fb + 3 * Load.Fs) + MathF.Pow2(fb) * (3 * Load.Fa + 2f*Load.Fs) + MathF.Pow2(Load.Fs) * (5.5f * Load.Fa + 4f * fb + 1.6f * Load.Fs) + 10f * Load.Fa * fb * Load.Fs);
            fMa = 0f;
            fMb = Load.Fq * Load.Fs / (12f * MathF.Pow2(fL)) * (MathF.Pow2(Load.Fa) * (6f * fb + 2f * Load.Fs) + MathF.Pow2(fb) * (3f * Load.Fa + 2f*Load.Fs) + MathF.Pow2(Load.Fs) * (2.5f * Load.Fa + 4f * fb + 0.8f * Load.Fs) + 10f * Load.Fa * fb * Load.Fs);
        }
        void GetEIF_0__00_36_UV(CMLoad_36 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(fb) *(2f*fL + Load.Fa) + Load.Fa*MathF.Pow2(Load.Fs));
            fB = (Load.Fq * Load.Fs * Load.Fa / (4f * MathF.Pow3(fL))) * (4f * Load.Fa * fL + 2f*MathF.Pow2(fb) * (3f*fL + Load.Fa) - MathF.Pow2(Load.Fs));
            fMa = 0f;
            fMb = (Load.Fq * Load.Fa * Load.Fs / (4f * MathF.Pow2(fL))) * (2f * fb * (fL + Load.Fa) -MathF.Pow2(Load.Fs));
        }
        // Trapezoidal
        void GetEIF_0__00_41_UV(CMLoad_41 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Not implemented yet

            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        // Temperature
        void GetEIF_0__00_51_UV(CMLoad_51z Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = -3f / 2f * objCrSc.m_Mat.m_fE * (float)objCrSc.I_y / fL * objCrSc.m_Mat.m_fAlpha_T * (Load.Ft_0_b - Load.Ft_0_u) / (float)objCrSc.h; 
            fB = -fA;
            fMa = 0f;
            fMb = 3f / 2f * ((objCrSc.m_Mat.m_fE * (float)objCrSc.I_y * objCrSc.m_Mat.m_fAlpha_T * (Load.Ft_0_b - Load.Ft_0_u)) / (float)objCrSc.h);
        }
        void GetEIF_0__00_51_UV(CMLoad_51y Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = -3f / 2f * objCrSc.m_Mat.m_fE * (float)objCrSc.I_y / fL * objCrSc.m_Mat.m_fAlpha_T * (Load.Ft_0_l - Load.Ft_0_r) / (float)objCrSc.h; 
            fB = -fA;
            fMa = 0f;
            fMb = 3f / 2f * ((objCrSc.m_Mat.m_fE * (float)objCrSc.I_y * objCrSc.m_Mat.m_fAlpha_T * (Load.Ft_0_l - Load.Ft_0_r)) / (float)objCrSc.h);
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------
        // Objostranne jednoducho podoprety prut
        //----------------------------------------------------------------------------------------------------------------------------

        // http://www.enggpedia.com/civil-engineering-encyclopedia/articles/1592-simply-supported-udl-beam-formulas

        #region Reactions of Both Sides Simply Supported Member
        // Singular Load
        void GetEIF_0__0__11_UV(CMLoad_11 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = Load.FF * fb / fL;
            fB = Load.FF * Load.Fa / fL;
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__12_UV(CMLoad_12 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = Load.FF / 2f;
            fB = fA;
            fMa = fMb = 0f;
        }
        // Uniform Load
        void GetEIF_0__0__21_UV(CMLoad_21 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Whole Member
            fA = 0.5f * Load.Fq * fL;
            fB = fA;
            fMa = fMb = 0f;

            // IF distribution formulas
            float fx = 0; // Temp
            float fx_M_max = fL / 2.0f;
            float fM_max = Load.Fq * MathF.Pow2(fL);
            float fM_x = Load.Fq * fx / 2.0f * (fL - fx);

            // Deflection formulas
            float fE = 0.0f, fI = 0.0f;
            float fx_Delta_max = fL / 2.0f;
            float fDelta_max = 5.0f * Load.Fq * MathF.Pow4(fL) / (384.0f * fE * fI);

        }
        void GetEIF_0__0__22_UV(CMLoad_22 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = ((Load.Fq * Load.Fa) / (2f*fL)) * (2f *fL - Load.Fa);
            fB = Load.Fq * MathF.Pow2(Load.Fa) / (2f * fL);
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__23_UV(CMLoad_23 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = (Load.Fq * MathF.Pow3(fb) / (8f * MathF.Pow3(fL))) * (3f * fL + Load.Fa);
            fB = ((Load.Fq * fb) / (8f * MathF.Pow3(fL))) * (14f * (2f * MathF.Pow2(fL) - MathF.Pow2(fb)) + MathF.Pow3(fb));
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__24_UV(CMLoad_24 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Partly in general position on member
            // Fa - distance from start
            // Fs - length of uniform load

            float fb = fL - Load.Fa;
            fA = (Load.Fq * Load.Fs / (8f * MathF.Pow3(fL))) * (4f * MathF.Pow2(fb) * (2f * fL + Load.Fa) + Load.Fa * MathF.Pow2(Load.Fs));
            fB = (Load.Fq * Load.Fs * Load.Fa / (8f * MathF.Pow3(fL))) * (8f * MathF.Pow2(fL) + 4f * fb * (fL + Load.Fa) - MathF.Pow2(Load.Fs));
            fMa = fMb = 0f;

            float fx = 0.0f, fV_x;

            if (fx < Load.Fa)
                fV_x = fA;
            else if (fx < (Load.Fa + Load.Fs))
                fV_x = fA - Load.Fq * (fx - Load.Fa);
            else
                fV_x = -fB;

            //float 



        }
        // Triangular Load
        void GetEIF_0__0__31_UV(CMLoad_31 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = 11f / 64f * Load.Fq * fL;
            fB = 21f / 64f * Load.Fq * fL;
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__32_UV(CMLoad_32 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = 11f / 40f * Load.Fq * fL;
            fB = 9f / 40f * Load.Fq * fL;
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__33_UV(CMLoad_33 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = 1f / 10f * Load.Fq * fL;
            fB = 2f / 5f * Load.Fq * fL;
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__34_UV(CMLoad_34 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa - Load.Fs;
            fA = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (MathF.Pow2(fb) * (3f * Load.Fa + 2 * fb + 5 * Load.Fs) + MathF.Pow2(Load.Fs) * (1.5f * Load.Fa + 4f * fb + 1.1f * Load.Fs) + 4f * Load.Fa * fb * Load.Fs);
            fB = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(Load.Fa) * (Load.Fa + 3 * fb + 3 * Load.Fs) + MathF.Pow2(fb) * (3 * Load.Fa + Load.Fs) + MathF.Pow2(Load.Fs) * (4.5f * Load.Fa + 2f * fb + 0.9f * Load.Fs) + 8f * Load.Fa * fb * Load.Fs);
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__35_UV(CMLoad_35 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa - Load.Fs;
            fA = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (MathF.Pow2(fb) * (3f * Load.Fa + 2 * fb + 4 * Load.Fs) + MathF.Pow2(Load.Fs) * (0.5f * Load.Fa + 2f * fb + 0.4f * Load.Fs) + 2f * Load.Fa * fb * Load.Fs);
            fB = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(Load.Fa) * (Load.Fa + 3 * fb + 3 * Load.Fs) + MathF.Pow2(fb) * (3 * Load.Fa + 2f * Load.Fs) + MathF.Pow2(Load.Fs) * (5.5f * Load.Fa + 4f * fb + 1.6f * Load.Fs) + 10f * Load.Fa * fb * Load.Fs);
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__36_UV(CMLoad_36 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            float fb = fL - Load.Fa;
            fA = (Load.Fq * Load.Fs / (4f * MathF.Pow3(fL))) * (2f * MathF.Pow2(fb) * (2f * fL + Load.Fa) + Load.Fa * MathF.Pow2(Load.Fs));
            fB = (Load.Fq * Load.Fs * Load.Fa / (4f * MathF.Pow3(fL))) * (4f * Load.Fa * fL + 2f * MathF.Pow2(fb) * (3f * fL + Load.Fa) - MathF.Pow2(Load.Fs));
            fMa = fMb = 0f;
        }
        // Trapezoidal
        void GetEIF_0__0__41_UV(CMLoad_41 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Not implemented yet
            // Temporary
            fA = 0.0f;
            fB = 0.0f;

            fMa = fMb = 0f;
        }
        // Temperature
        void GetEIF_0__0__51_UV(CMLoad_51z Load,  CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = -3f / 2f * objCrSc.m_Mat.m_fE * (float)objCrSc.I_y / fL * objCrSc.m_Mat.m_fAlpha_T * (Load.Ft_0_b - Load.Ft_0_u) / (float)objCrSc.h;
            fB = -fA;
            fMa = fMb = 0f;
        }
        void GetEIF_0__0__51_UV(CMLoad_51y Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            fA = -3f / 2f * objCrSc.m_Mat.m_fE * (float)objCrSc.I_y / fL * objCrSc.m_Mat.m_fAlpha_T * (Load.Ft_0_l - Load.Ft_0_r) / (float)objCrSc.h;
            fB = -fA;
            fMa = fMb = 0f;
        }

        #endregion

       // Este chyba konzola !!!!!!


        void GetEIF_00____11_UV(CMLoad_11 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____12_UV(CMLoad_12 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____21_UV(CMLoad_21 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____22_UV(CMLoad_22 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____23_UV(CMLoad_23 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____24_UV(CMLoad_24 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____31_UV(CMLoad_31 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____32_UV(CMLoad_32 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____33_UV(CMLoad_33 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;

        }
        void GetEIF_00____34_UV(CMLoad_34 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____35_UV(CMLoad_35 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____36_UV(CMLoad_36 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____41_UV(CMLoad_41 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____51_UV(CMLoad_51z Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF_00____51_UV(CMLoad_51y Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }








        void GetEIF____00_11_UV(CMLoad_11 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_12_UV(CMLoad_12 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_21_UV(CMLoad_21 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_22_UV(CMLoad_22 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_23_UV(CMLoad_23 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_24_UV(CMLoad_24 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_31_UV(CMLoad_31 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_32_UV(CMLoad_32 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_33_UV(CMLoad_33 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;

        }
        void GetEIF____00_34_UV(CMLoad_34 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_35_UV(CMLoad_35 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_36_UV(CMLoad_36 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_41_UV(CMLoad_41 Load, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_51_UV(CMLoad_51z Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
        void GetEIF____00_51_UV(CMLoad_51y Load, CCrSc objCrSc, float fL, out float fA, out float fB, out float fMa, out float fMb)
        {
            // Temporary
            fA = 0.0f;
            fB = 0.0f;
            fMa = 0.0f;
            fMb = 0.0f;
        }
    }
}
