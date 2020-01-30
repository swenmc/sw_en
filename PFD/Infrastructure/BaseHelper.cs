using BaseClasses;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PFD
{
    public static class BaseHelper
    {
        public static string GetInfoText(CMember m, CLoadCombination loadComb)
        {
            if (m == null) return "";
            if (loadComb == null) return "";

            StringBuilder sb = new StringBuilder();
            if (loadComb.eLComType == ELSType.eLS_SLS) sb.Append("Governing Limit State: SLS");
            else sb.Append("Governing Limit State: ULS");

            sb.Append($" / Governing Load Combination: { loadComb.Name} { loadComb.CombinationKey}");
            sb.Append($" / Governing Member Type: {m.EMemberTypePosition.GetFriendlyName()}");

            return sb.ToString();
        }
        public static string GetGoverningMemberText(CMember m)
        {
            if (m == null) return "";
            return $"Governing: {m.EMemberTypePosition.GetFriendlyName()}";
            //return $"Governing Member Type: {m.EMemberTypePosition.GetFriendlyName()}";
        }
        public static string GetGoverningLoadCombText(CLoadCombination loadComb)
        {            
            if (loadComb == null) return "";
            return $"Governing: { loadComb.Name} { loadComb.CombinationKey}";
            //return $"Governing Load Combination: { loadComb.Name} { loadComb.CombinationKey}";
        }

        public static string GetGoverningLimitStateText(ELSType t)
        {
            if (t == ELSType.eLS_SLS) return "Governing: SLS"; //return "Governing Limit State: SLS";
            else if (t == ELSType.eLS_ULS) return "Governing: ULS"; //return "Governing Limit State: ULS";
            else return "All";
        }

    }
}
