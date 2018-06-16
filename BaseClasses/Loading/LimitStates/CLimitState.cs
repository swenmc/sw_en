using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
	[Serializable]
	public class CLimitState
	{
		//----------------------------------------------------------------------------
		private int m_ID;

		public int ID
		{
			get { return m_ID; }
			set { m_ID = value; }
		}

        ELSType eLS_Type;

        List<CLoadCombination> LoadCombinationsList;
		//----------------------------------------------------------------------------
		//----------------------------------------------------------------------------
		//----------------------------------------------------------------------------
		public CLimitState()
		{

		}

        public CLimitState(ELSType eLS_Type_temp)
        {
            eLS_Type = eLS_Type_temp;
        }

        public CLimitState(ELSType eLS_Type_temp, List<CLoadCombination> lLoadCombinations_temp)
        {
            eLS_Type = eLS_Type_temp;
            LoadCombinationsList = lLoadCombinations_temp;
        }
	}
}
