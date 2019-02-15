using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CLoadCaseGenerator
    {
        CSurfaceLoadGenerator slg;
        CNodalLoadGenerator nlg;
        public CLoadCaseGenerator(CSurfaceLoadGenerator surfaceLoadGenerator, CNodalLoadGenerator nodalLoadGenerator)
        {
            slg = surfaceLoadGenerator;
            nlg = nodalLoadGenerator;
        }


        public CLoadCase[] GenerateLoadCases()
        {
            CLoadCase[] m_arrLoadCases = new CLoadCase[44];
            m_arrLoadCases[00] = new CLoadCase(01, "Dead load G", ELCGTypeForLimitState.eUniversal, ELCType.ePermanentLoad, ELCMainDirection.eGeneral, slg.surfaceDeadLoad);                                                          // 01
            m_arrLoadCases[01] = new CLoadCase(02, "Imposed load Q", ELCGTypeForLimitState.eUniversal, ELCType.eImposedLoad_ST, ELCMainDirection.eGeneral, slg.surfaceRoofImposedLoad);                                               // 02

            // ULS - Load Case
            m_arrLoadCases[02] = new CLoadCase(03, "Snow load Su - full", ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, slg.surfaceRoofSnowLoad_ULS_Nu_1);                                                // 03
            m_arrLoadCases[03] = new CLoadCase(04, "Snow load Su - left", ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, slg.surfaceRoofSnowLoad_ULS_Nu_2_Left);                                           // 04
            m_arrLoadCases[04] = new CLoadCase(05, "Snow load Su - right", ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, slg.surfaceRoofSnowLoad_ULS_Nu_2_Right);                                         // 05
            m_arrLoadCases[05] = new CLoadCase(06, "Wind load Wu - Cpi,min - Left - X+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX, slg.surfaceWindLoad_ULS_PlusX_Cpimin);                             // 06
            m_arrLoadCases[06] = new CLoadCase(07, "Wind load Wu - Cpi,min - Right - X-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX, slg.surfaceWindLoad_ULS_MinusX_Cpimin);                          // 07
            m_arrLoadCases[07] = new CLoadCase(08, "Wind load Wu - Cpi,min - Front - Y+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY, slg.surfaceWindLoad_ULS_PlusY_Cpimin);                            // 08
            m_arrLoadCases[08] = new CLoadCase(09, "Wind load Wu - Cpi,min - Rear - Y-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY, slg.surfaceWindLoad_ULS_MinusY_Cpimin);                           // 09
            m_arrLoadCases[09] = new CLoadCase(10, "Wind load Wu - Cpi,max - Left - X+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX, slg.surfaceWindLoad_ULS_PlusX_Cpimax);                             // 10
            m_arrLoadCases[10] = new CLoadCase(11, "Wind load Wu - Cpi,max - Right - X-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX, slg.surfaceWindLoad_ULS_MinusX_Cpimax);                          // 11
            m_arrLoadCases[11] = new CLoadCase(12, "Wind load Wu - Cpi,max - Front - Y+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY, slg.surfaceWindLoad_ULS_PlusY_Cpimax);                            // 12
            m_arrLoadCases[12] = new CLoadCase(13, "Wind load Wu - Cpi,max - Rear - Y-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY, slg.surfaceWindLoad_ULS_MinusY_Cpimax);                           // 13
            m_arrLoadCases[13] = new CLoadCase(14, "Wind load Wu - Cpe,min - Left - X+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX, slg.surfaceWindLoad_ULS_PlusX_Cpemin);                             // 14
            m_arrLoadCases[14] = new CLoadCase(15, "Wind load Wu - Cpe,min - Right - X-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX, slg.surfaceWindLoad_ULS_MinusX_Cpemin);                          // 15
            m_arrLoadCases[15] = new CLoadCase(16, "Wind load Wu - Cpe,min - Front - Y+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY, slg.surfaceWindLoad_ULS_PlusY_Cpemin);                            // 16
            m_arrLoadCases[16] = new CLoadCase(17, "Wind load Wu - Cpe,min - Rear - Y-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY, slg.surfaceWindLoad_ULS_MinusY_Cpemin);                           // 17
            m_arrLoadCases[17] = new CLoadCase(18, "Wind load Wu - Cpe,max - Left - X+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX, slg.surfaceWindLoad_ULS_PlusX_Cpemax);                             // 18
            m_arrLoadCases[18] = new CLoadCase(19, "Wind load Wu - Cpe,max - Right - X-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX, slg.surfaceWindLoad_ULS_MinusX_Cpemax);                          // 19
            m_arrLoadCases[19] = new CLoadCase(20, "Wind load Wu - Cpe,max - Front - Y+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY, slg.surfaceWindLoad_ULS_PlusY_Cpemax);                            // 20
            m_arrLoadCases[20] = new CLoadCase(21, "Wind load Wu - Cpe,max - Rear - Y-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY, slg.surfaceWindLoad_ULS_MinusY_Cpemax);                           // 21
            m_arrLoadCases[21] = new CLoadCase(22, "Earthquake load Eu - X", ELCGTypeForLimitState.eULSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusX, nlg.nodalLoadEQ_ULS_PlusX);                                              // 22
            m_arrLoadCases[22] = new CLoadCase(23, "Earthquake load Eu - Y", ELCGTypeForLimitState.eULSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusY, nlg.nodalLoadEQ_ULS_PlusY);                                              // 23

            // SLS - Load Case
            m_arrLoadCases[23] = new CLoadCase(24, "Snow load Ss - full", ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, slg.surfaceRoofSnowLoad_SLS_Nu_1);                                                // 24
            m_arrLoadCases[24] = new CLoadCase(25, "Snow load Ss - left", ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, slg.surfaceRoofSnowLoad_SLS_Nu_2_Left);                                           // 25
            m_arrLoadCases[25] = new CLoadCase(26, "Snow load Ss - right", ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, slg.surfaceRoofSnowLoad_SLS_Nu_2_Right);                                         // 26
            m_arrLoadCases[26] = new CLoadCase(27, "Wind load Ws - Cpi,min - Left - X+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX, slg.surfaceWindLoad_SLS_PlusX_Cpimin);                             // 27
            m_arrLoadCases[27] = new CLoadCase(28, "Wind load Ws - Cpi,min - Right - X-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX, slg.surfaceWindLoad_SLS_MinusX_Cpimin);                          // 28
            m_arrLoadCases[28] = new CLoadCase(29, "Wind load Ws - Cpi,min - Front - Y+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY, slg.surfaceWindLoad_SLS_PlusY_Cpimin);                            // 29
            m_arrLoadCases[29] = new CLoadCase(30, "Wind load Ws - Cpi,min - Rear - Y-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY, slg.surfaceWindLoad_SLS_MinusY_Cpimin);                           // 30
            m_arrLoadCases[30] = new CLoadCase(31, "Wind load Ws - Cpi,max - Left - X+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX, slg.surfaceWindLoad_SLS_PlusX_Cpimax);                             // 31
            m_arrLoadCases[31] = new CLoadCase(32, "Wind load Ws - Cpi,max - Right - X-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX, slg.surfaceWindLoad_SLS_MinusX_Cpimax);                          // 32
            m_arrLoadCases[32] = new CLoadCase(33, "Wind load Ws - Cpi,max - Front - Y+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY, slg.surfaceWindLoad_SLS_PlusY_Cpimax);                            // 33
            m_arrLoadCases[33] = new CLoadCase(34, "Wind load Ws - Cpi,max - Rear - Y-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY, slg.surfaceWindLoad_SLS_MinusY_Cpimax);                           // 34
            m_arrLoadCases[34] = new CLoadCase(35, "Wind load Ws - Cpe,min - Left - X+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX, slg.surfaceWindLoad_SLS_PlusX_Cpemin);                             // 35
            m_arrLoadCases[35] = new CLoadCase(36, "Wind load Ws - Cpe,min - Right - X-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX, slg.surfaceWindLoad_SLS_MinusX_Cpemin);                          // 36
            m_arrLoadCases[36] = new CLoadCase(37, "Wind load Ws - Cpe,min - Front - Y+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY, slg.surfaceWindLoad_SLS_PlusY_Cpemin);                            // 37
            m_arrLoadCases[37] = new CLoadCase(38, "Wind load Ws - Cpe,min - Rear - Y-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY, slg.surfaceWindLoad_SLS_MinusY_Cpemin);                            // 38
            m_arrLoadCases[38] = new CLoadCase(39, "Wind load Ws - Cpe,max - Left - X+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX, slg.surfaceWindLoad_SLS_PlusX_Cpemax);                             // 39
            m_arrLoadCases[39] = new CLoadCase(40, "Wind load Ws - Cpe,max - Right - X-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX, slg.surfaceWindLoad_SLS_MinusX_Cpemax);                          // 40
            m_arrLoadCases[40] = new CLoadCase(41, "Wind load Ws - Cpe,max - Front - Y+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY, slg.surfaceWindLoad_SLS_PlusY_Cpemax);                            // 41
            m_arrLoadCases[41] = new CLoadCase(42, "Wind load Ws - Cpe,max - Rear - Y-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY, slg.surfaceWindLoad_SLS_MinusY_Cpemax);                           // 42
            m_arrLoadCases[42] = new CLoadCase(43, "Earthquake load Es - X", ELCGTypeForLimitState.eSLSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusX, nlg.nodalLoadEQ_SLS_PlusX);                                              // 43
            m_arrLoadCases[43] = new CLoadCase(44, "Earthquake load Es - Y", ELCGTypeForLimitState.eSLSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusY, nlg.nodalLoadEQ_SLS_PlusY);                                              // 44

            return m_arrLoadCases;
        }

    }
}
