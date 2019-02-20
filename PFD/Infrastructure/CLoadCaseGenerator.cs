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
        public CLoadCaseGenerator()
        {
        }

        public CLoadCase[] GenerateLoadCases()
        {
            CLoadCase[] m_arrLoadCases = new CLoadCase[44];
            
            m_arrLoadCases[(int)ELCName.eDL_G] = new CLoadCase(01, ELCName.eDL_G.GetFriendlyName(), ELCGTypeForLimitState.eUniversal, ELCType.ePermanentLoad, ELCMainDirection.eGeneral);                                   // 01
            m_arrLoadCases[(int)ELCName.eIL_Q] = new CLoadCase(02, ELCName.eIL_Q.GetFriendlyName(), ELCGTypeForLimitState.eUniversal, ELCType.eImposedLoad_ST, ELCMainDirection.eGeneral);                               // 02

            // ULS - Load Case
            m_arrLoadCases[(int)ELCName.eSL_Su_Full] = new CLoadCase(03, ELCName.eSL_Su_Full.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ);                                        // 03
            m_arrLoadCases[(int)ELCName.eSL_Su_Left] = new CLoadCase(04, ELCName.eSL_Su_Left.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ);                                        // 04
            m_arrLoadCases[(int)ELCName.eSL_Su_Right] = new CLoadCase(05, ELCName.eSL_Su_Right.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ);                                       // 05
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Left_X_Plus] = new CLoadCase(06, ELCName.eWL_Wu_Cpi_min_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX);                         // 06
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Right_X_Minus] = new CLoadCase(07, ELCName.eWL_Wu_Cpi_min_Right_X_Minus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX);                       // 07
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Front_Y_Plus] = new CLoadCase(08, ELCName.eWL_Wu_Cpi_min_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY);                        // 08
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Rear_Y_Minus] = new CLoadCase(09, ELCName.eWL_Wu_Cpi_min_Rear_Y_Minus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY);                        // 09
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Left_X_Plus] = new CLoadCase(10, ELCName.eWL_Wu_Cpi_max_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX);                         // 10
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Right_X_Minus] = new CLoadCase(11, ELCName.eWL_Wu_Cpi_max_Right_X_Minus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX);                       // 11
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Front_Y_Plus] = new CLoadCase(12, ELCName.eWL_Wu_Cpi_max_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY);                        // 12
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Rear_Y_Minus] = new CLoadCase(13, ELCName.eWL_Wu_Cpi_max_Rear_Y_Minus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY);                        // 13
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Left_X_Plus] = new CLoadCase(14, ELCName.eWL_Wu_Cpe_min_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX);                         // 14
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Right_X_Minus] = new CLoadCase(15, ELCName.eWL_Wu_Cpe_min_Right_X_Minus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX);                       // 15
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Front_Y_Plus] = new CLoadCase(16, ELCName.eWL_Wu_Cpe_min_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY);                        // 16
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Rear_Y_Minus] = new CLoadCase(17, ELCName.eWL_Wu_Cpe_min_Rear_Y_Minus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY);                        // 17
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Left_X_Plus] = new CLoadCase(18, ELCName.eWL_Wu_Cpe_max_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX);                         // 18
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Right_X_Minus] = new CLoadCase(19, ELCName.eWL_Wu_Cpe_max_Right_X_Minus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX);                       // 19
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Front_Y_Plus] = new CLoadCase(20, ELCName.eWL_Wu_Cpe_max_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY);                        // 20
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Rear_Y_Minus] = new CLoadCase(21, ELCName.eWL_Wu_Cpe_max_Rear_Y_Minus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY);                        // 21
            m_arrLoadCases[(int)ELCName.eEQ_Eu_Left_X_Plus] = new CLoadCase(22, ELCName.eEQ_Eu_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusX);                               // 22
            m_arrLoadCases[(int)ELCName.eEQ_Eu_Front_Y_Plus] = new CLoadCase(23, ELCName.eEQ_Eu_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eULSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusY);                               // 23

            // SLS - Load Case
            m_arrLoadCases[(int)ELCName.eSL_Ss_Full] = new CLoadCase(24, ELCName.eSL_Ss_Full.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ);                                        // 24
            m_arrLoadCases[(int)ELCName.eSL_Ss_Left] = new CLoadCase(25, ELCName.eSL_Ss_Left.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ);                                        // 25
            m_arrLoadCases[(int)ELCName.eSL_Ss_Right] = new CLoadCase(26, ELCName.eSL_Ss_Right.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ);                                       // 26
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Left_X_Plus] = new CLoadCase(27, ELCName.eWL_Ws_Cpi_min_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX);                         // 27
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Right_X_Minus] = new CLoadCase(28, ELCName.eWL_Ws_Cpi_min_Right_X_Minus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX);                       // 28
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Front_Y_Plus] = new CLoadCase(29, ELCName.eWL_Ws_Cpi_min_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY);                        // 29
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Rear_Y_Minus] = new CLoadCase(30, ELCName.eWL_Ws_Cpi_min_Rear_Y_Minus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY);                        // 30
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Left_X_Plus] = new CLoadCase(31, ELCName.eWL_Ws_Cpi_max_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX);                         // 31
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Right_X_Minus] = new CLoadCase(32, ELCName.eWL_Ws_Cpi_max_Right_X_Minus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX);                       // 32
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Front_Y_Plus] = new CLoadCase(33, ELCName.eWL_Ws_Cpi_max_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY);                        // 33
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Rear_Y_Minus] = new CLoadCase(34, ELCName.eWL_Ws_Cpi_max_Rear_Y_Minus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY);                        // 34
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Left_X_Plus] = new CLoadCase(35, ELCName.eWL_Ws_Cpe_min_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX);                         // 35
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Right_X_Minus] = new CLoadCase(36, ELCName.eWL_Ws_Cpe_min_Right_X_Minus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX);                       // 36
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Front_Y_Plus] = new CLoadCase(37, ELCName.eWL_Ws_Cpe_min_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY);                        // 37
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Rear_Y_Minus] = new CLoadCase(38, ELCName.eWL_Ws_Cpe_min_Rear_Y_Minus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY);                        // 38
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Left_X_Plus] = new CLoadCase(39, ELCName.eWL_Ws_Cpe_max_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX);                         // 39
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Right_X_Minus] = new CLoadCase(40, ELCName.eWL_Ws_Cpe_max_Right_X_Minus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX);                       // 40
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Front_Y_Plus] = new CLoadCase(41, ELCName.eWL_Ws_Cpe_max_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY);                        // 41
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Rear_Y_Minus] = new CLoadCase(42, ELCName.eWL_Ws_Cpe_max_Rear_Y_Minus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY);                        // 42
            m_arrLoadCases[(int)ELCName.eEQ_Es_Left_X_Plus] = new CLoadCase(43, ELCName.eEQ_Es_Left_X_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusX);                               // 43
            m_arrLoadCases[(int)ELCName.eEQ_Es_Front_Y_Plus] = new CLoadCase(44, ELCName.eEQ_Es_Front_Y_Plus.GetFriendlyName(), ELCGTypeForLimitState.eSLSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusY);                               // 44

            return m_arrLoadCases;
        }
    }
}
