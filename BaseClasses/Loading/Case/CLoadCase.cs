using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
	[Serializable]
	public class CLoadCase
	{
		//----------------------------------------------------------------------------
		private int m_ID;

		public int ID
		{
			get { return m_ID; }
			set { m_ID = value; }
		}

        List<CLoad> LoadsList;
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CLoadCase()
		{

		}

        public CLoadCase(List<CLoad> Loads_temp)
        {
            LoadsList = Loads_temp;
        }
    }
}
