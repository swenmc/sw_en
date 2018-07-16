using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    [Serializable]
    public class CLoadCombination
    {/*
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
    */
        //----------------------------------------------------------------------------
        private int m_ID;

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private string m_Name;

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

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
