using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    [Serializable]
    public class CLoadCombination : CObject
    {
        /*
        ULS Strength Load Combinations:
        Ed1 = 1.35G + 1.35[TH, TF]
                min
        Ed2 = 1.2G + 1.5Q + 1.2[TH, TF]
                min
        Ed3 = 1.2G + 1.5ψl Q
        Ed4 = 1.2G + Wu + ψc Q
        Ed5 = 0.9G + Wu
        Ed6 = G + Eu + ψc Q + AF + TH
        Ed7 = 0.9G + Eu
        Ed8 = 1.2G + Su + ψc Q + 1.2[TH, TF]
                min
        Ed9 = 1.2G + 1.2TE + Wu
        Where:
        Eu Ultimate seismic load
        Su Ultimate load due to snow, liquid, rainwater ponding or earth pressure
        Wu Ultimate wind load
        ψl Imposed Action Long-term Factor
        ψc Imposed Action Combination Factor
        SLS Load Combinations:
        S1 = G + ψl Q + [TH, TF]
                min
        S2 = G + ψs Q
        S3 = G + ψl Q + Es + [TH, TF]
                min
        S4 = G + ψl Q + Ws + [TH, TF] min

        Where:
        ψs Imposed Action Short-term Factor
        Es Serviceability seismic load
        Ws Serviceability wind load
        Specific SLS conditions directly associated with process and operating requirements shall be assessed
        separately and shall apply where these are more critical.

        WSD Method Load Combinations:
        A1 = G + [TH, TF]min + AF
        A2 = G + Q + [TH, TF]min + AF
        A3 = G + ψl Q
        A4 = G + Wu / 1.5 + ψc Q + AF + TH
        A5 = 0.7G + Wu/1.5
        A6 = G + 0.8Eu + AF + TH
        A7 = G + 0.8Eu
        A8 = G + Su / 1.5 + ψc Q + [TH, TF]min
        In any load combination containing either a wind or earthquake ULS load case, the allowable stress may be
        increased if permitted by the allowable stress material design standard for such occasional loads.

        AS/NZS 1170
        4.2 COMBINATIONS OF ACTIONS FOR ULTIMATE LIMIT STATES

        4.2.1 Stability
        (a) For combinations that produce net stabilizing effects (Ed,stb):
        Ed,stb = [0.9G] permanent action only (does not apply to prestressing forces)

        (b) For combinations that produce net destabilizing effects (Ed,dst):
        (i) Ed,dst = [1.35G] permanent action only (does not apply to prestressing forces)
        (ii) Ed,dst = [1.2G, 1.5Q] permanent and imposed action
        (iv) Ed,dst = [1.2G, Wu, ψcQ] permanent, wind and imposed action
        (v) Ed,dst = [G, Eu, ψEQ] permanent, earthquake and imposed action
        (vi) Ed,dst = [1.2G, Su, ψcQ] permanent action, actions given in Clause 4.2.3 and imposed action

        4.2.2 Strength
        (a) Ed = [1.35G] permanent action only (does not apply to prestressing forces)
        (b) Ed = [1.2G, 1.5Q] permanent and imposed action
        (c) Ed = [1.2G, 1.5ψlQ] permanent and long-term imposed action
        (d) Ed = [1.2G, Wu, ψcQ] permanent, wind and imposed action
        (e) Ed = [0.9G, Wu] permanent and wind action reversal
        (f) Ed = [G, Eu, ψEQ] permanent, earthquake and imposed action
        (g) Ed = [1.2G, Su, ψcQ] permanent action, actions given in Clause 4.2.3 and imposed action

        4.3 COMBINATIONS OF ACTIONS FOR SERVICEABILITY LIMIT STATES
        Combinations of actions for the serviceability limit states shall be those appropriate for the
        serviceability condition being considered. Appropriate combinations may include one or a
        number of the following using the short-term and long-term values given in Table 4.1:
        (a) G
        (b) ψsQ
        (c) ψlQ
        (d) Ws
        (e) Es
        (f) Serviceability values of other actions, as appropriate.
        */

        //----------------------------------------------------------------------------
        // TODO - zmenit na ENUM
        /*
        private string m_Type;

        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }*/

        public ELSType eLComType;
        public List<float> LoadCasesFactorsList = new List<float>();
        public List<CLoadCase> LoadCasesList = new List<CLoadCase>();

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CLoadCombination()
        {

        }

        public CLoadCombination(ELSType eLComType_temp)
        {
            eLComType = eLComType_temp;
        }

        public CLoadCombination(List<float> LoadCasesFactors_temp, List<CLoadCase> LoadCases_temp)
        {
            LoadCasesFactorsList = LoadCasesFactors_temp;
            LoadCasesList = LoadCases_temp;
        }

        public CLoadCombination(int id_temp, string name_temp, ELSType eLComType_temp)
        {
            ID = id_temp;
            Name = name_temp;
            eLComType = eLComType_temp;
        }

        public CLoadCombination(int id_temp, string name_temp, ELSType eLComType_temp, List<float> LoadCasesFactors_temp, List<CLoadCase> LoadCases_temp)
        {
            ID = id_temp;
            Name = name_temp;
            eLComType = eLComType_temp;
            LoadCasesFactorsList = LoadCasesFactors_temp;
            LoadCasesList = LoadCases_temp;
        }
    }
}
