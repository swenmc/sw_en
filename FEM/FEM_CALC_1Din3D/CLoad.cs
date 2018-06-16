using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;
using MATH;
using FEM_CALC_BASE;

namespace FEM_CALC_1Din3D
{
    public class CLoad
    {
       ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       // Node load
       ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////












       ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       // Member transverse load
       ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

       // Primary End Forces in Local Coordinate System of Member

       // Member default - no transverse load at member
        public void GetEndLoad(CE_1D Element)
        {
            for (int i = 0; i < 6; i++)
            {
                Element.m_VElemPEF_LCS_StNode.FVectorItems[i] = 0f;
                Element.m_VElemPEF_LCS_EnNode.FVectorItems[i] = 0f;
            }

            // Set Primary End Forces of Elemnent in Global Coordinate system
            SetGetPEF_GCS(Element);

            // Update Element Primary End Forces Vector (1x12) in LCS
            UpdateElemPEF_LCS_Vector(Element);
            // Update Element Primary End Forces Vector (1x12) in GCS
            UpdateElemPEF_GCS_Vector(Element);
       } 

        // Member 1-2 - Loaded by uniform load
        public void GetEndLoad_g(CE_1D Element, float fg_z)
        {
            switch (Element.m_eSuppType3D)
            {
                case EElemSuppType3D.e3DEl_000000_000000:
                case EElemSuppType3D.e3DEl_000000_0_00_0:
                case EElemSuppType3D.e3DEl_0_00_0_000000:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = fg_z * Element.FLength / 2f;

                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = -fg_z * (float)Math.Pow(Element.FLength, 2f) / 12f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = 0f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = fg_z * Element.FLength / 2f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = fg_z * (float)Math.Pow(Element.FLength, 2f) / 12f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = 0f;
                       
                        break;
                    }
                case EElemSuppType3D.e3DEl_000____000000:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = -3 * fg_z * Element.FLength / 8f;

                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = 0f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = 5 * fg_z * Element.FLength / 8f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = fg_z * (float)Math.Pow(Element.FLength, 2f) / 8f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = 0f;

                        break;
                    }
                case EElemSuppType3D.e3DEl_000000_000___:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = 5 * fg_z * Element.FLength / 8f;

                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = fg_z * (float)Math.Pow(Element.FLength, 2f) / 8f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = 0f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = -3 * fg_z * Element.FLength / 8f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = 0f;

                        break;
                    }

                default:
                    {
                        GetEndLoad(Element);
                      break;
                    }
            }

            // Set Primary End Forces of Elemnent in Global Coordinate system
            SetGetPEF_GCS(Element);

            // Update Element Primary End Forces Vector (1x12) in LCS
            UpdateElemPEF_LCS_Vector(Element);
            // Update Element Primary End Forces Vector (1x12) in GCS
            UpdateElemPEF_GCS_Vector(Element);

        }









        // Member 1-3
        public void GetEndLoad_F(CE_1D Element, float fFx, float fFy, float fFz)
        {
            switch (Element.m_eSuppType3D)
            {
                case EElemSuppType3D.e3DEl_000000_000000:
                case EElemSuppType3D.e3DEl_000000_0_00_0:
                case EElemSuppType3D.e3DEl_0_00_0_000000:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = fFx / 2f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = -0.5f * fFz;

                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = fFz * Element.FLength / 8f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = 0f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = fFx / 2f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = -0.5f * fFz;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = -fFz * Element.FLength / 8f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = 0f;
                        break;
                    }
                case EElemSuppType3D.e3DEl_000____000000:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = fFx / 2f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = 5f / 16f * fFz;

                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = 0f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = fFx / 2f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = 11f / 16f * fFz;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = 3f / 16f * fFz * Element.FLength;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = 0f;
                        break;
                    }
                case EElemSuppType3D.e3DEl_000000_000___:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = fFx / 2f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = 11f / 16f * fFz;

                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = 3f / 16f * fFz * Element.FLength;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = 0f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = fFx / 2f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = 5f / 16f * fFz;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = 0f;
                        break;
                    }
                default:
                    {
                        GetEndLoad(Element);
                        break;
                    }
            }

            // Set Primary End Forces of Elemnent in Global Coordinate system
            SetGetPEF_GCS(Element);

            // Update Element Primary End Forces Vector (1x12) in LCS
            UpdateElemPEF_LCS_Vector(Element);
            // Update Element Primary End Forces Vector (1x12) in GCS
            UpdateElemPEF_GCS_Vector(Element);
        }







        // Member 1-4
        public void GetEndLoad_M(CE_1D Element, float fMx, float fMy, float fMz)
        {
            switch (Element.m_eSuppType3D)
            {
                case EElemSuppType3D.e3DEl_000000_000000:
                case EElemSuppType3D.e3DEl_000000_0_00_0:
                case EElemSuppType3D.e3DEl_0_00_0_000000:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 9f * fMz / (8f * Element.FLength);
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = 9f * fMy / (8f * Element.FLength);
                        
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = fMx / 2f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = fMy * Element.FLength / 12f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = fMz * Element.FLength / 12f;
                        
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 9f * fMz / (8f * Element.FLength);
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = 9f * fMy / (8f * Element.FLength);
                        
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = -fMx / 2f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = -fMy * Element.FLength / 12f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = -fMz * Element.FLength / 12f; 

                        break;
                    }
                case EElemSuppType3D.e3DEl_000____000000:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = 9f * fMy / (8f * Element.FLength);

                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = 0f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = 9f * fMy / (8f * Element.FLength);

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = -fMx;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = fMy * Element.FLength / 8f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = 0f;
                        break;
                    }
                case EElemSuppType3D.e3DEl_000000_000___:
                    {
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[2] = 9f * fMy / (8f * Element.FLength);

                        Element.m_VElemPEF_LCS_StNode.FVectorItems[3] = fMx;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[4] = fMy * Element.FLength / 8f;
                        Element.m_VElemPEF_LCS_StNode.FVectorItems[5] = 0f;

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[0] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[1] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[2] = 9f * fMy / (8f * Element.FLength);

                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[3] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[4] = 0f;
                        Element.m_VElemPEF_LCS_EnNode.FVectorItems[5] = 0f;
                        break;
                    }
                default:
                    {
                        GetEndLoad(Element);
                        break;
                    }
            }

            // Set Primary End Forces of Elemnent in Global Coordinate system
            SetGetPEF_GCS(Element);

            // Update Element Primary End Forces Vector (1x12) in LCS
            UpdateElemPEF_LCS_Vector(Element);
            // Update Element Primary End Forces Vector (1x12) in GCS
            UpdateElemPEF_GCS_Vector(Element);
        }













        // Set vector of ends  primary forces in global coordinate system
        public void SetGetPEF_GCS(CE_1D Element)
        {
            // Start Node
            // [PEF_GCS i] = [A0T] * [PEF_LCS i]
            Element.m_VElemPEF_GCS_StNode = VectorF.fMultiplyMatrVectr(MatrixF.GetTransMatrix(Element.m_fATRMatr3D), Element.m_VElemPEF_LCS_StNode);

            // End Node
            // [PEF_GCS j] = [A0T] * [PEF_LCS j]
            Element.m_VElemPEF_GCS_EnNode = VectorF.fMultiplyMatrVectr(MatrixF.GetTransMatrix(Element.m_fATRMatr3D), Element.m_VElemPEF_LCS_EnNode);
        }

        // Update Element Primary End Forces Vector (1x12) in LCS
        void UpdateElemPEF_LCS_Vector(CE_1D Element)
        {
            for (int i = 0; i < 6; i++)
            {
                Element.m_ArrElemPEF_LCS.m_fArrMembers[0, i] = Element.m_VElemPEF_LCS_StNode.FVectorItems[i];  // Start Node
                Element.m_ArrElemPEF_LCS.m_fArrMembers[1, i] = Element.m_VElemPEF_LCS_EnNode.FVectorItems[i];  // End Node
            }
        }

        // Update Element Primary End Forces Vector (1x12) in GCS
        void UpdateElemPEF_GCS_Vector(CE_1D Element)
        {
            for (int i = 0; i < 6; i++)
            {
                Element.m_ArrElemPEF_GCS.m_fArrMembers[0, i] = Element.m_VElemPEF_GCS_StNode.FVectorItems[i];  // Start Node
                Element.m_ArrElemPEF_GCS.m_fArrMembers[1, i] = Element.m_VElemPEF_GCS_EnNode.FVectorItems[i];  // End Node
            }
        }

    }
}
