using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class LoadCasesMemberLoads : Dictionary<int, List<CMLoad>>
    {
        public LoadCasesMemberLoads()
        {
        }

        public void Merge(LoadCasesMemberLoads otherLoads)
        {
            foreach (KeyValuePair<int, List<CMLoad>> kvp in this)
            {
                if (otherLoads.ContainsKey(kvp.Key)) this[kvp.Key].AddRange(otherLoads[kvp.Key]);                
            }
        }

    }
}
