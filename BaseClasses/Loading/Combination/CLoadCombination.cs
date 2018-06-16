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

        ELSType eLComType;
        List<CLoadCase> LoadCasesList;
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

        public CLoadCombination(List<CLoadCase> LoadCases_temp)
        {
            LoadCasesList = LoadCases_temp;
        }
    }
}
