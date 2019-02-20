using System;
using System.Collections.Generic;

namespace BaseClasses
{
    [Serializable]
    public class CLimitState : CObject
    {
        //----------------------------------------------------------------------------
        public ELSType eLS_Type;

        List<CLoadCombination> LoadCombinationsList;
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CLimitState()
        {

        }

        public CLimitState(string sLimitState_name_temp, ELSType eLS_Type_temp)
        {
            Name = sLimitState_name_temp;
            eLS_Type = eLS_Type_temp;
        }

        public CLimitState(string sLimitState_name_temp, ELSType eLS_Type_temp, List<CLoadCombination> lLoadCombinations_temp)
        {
            Name = sLimitState_name_temp;
            eLS_Type = eLS_Type_temp;
            LoadCombinationsList = lLoadCombinations_temp;
        }
	}
}
