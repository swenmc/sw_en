using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
	[Serializable]
	public class CLoadCombination
	{
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

        public CLoadCombination(int id_temp, string name_temp,  ELSType eLComType_temp, List<float> LoadCasesFactors_temp, List<CLoadCase> LoadCases_temp)
        {
            ID = id_temp;
            Name = name_temp;
            eLComType = eLComType_temp;
            LoadCasesFactorsList = LoadCasesFactors_temp;
            LoadCasesList = LoadCases_temp;
        }
    }
}
