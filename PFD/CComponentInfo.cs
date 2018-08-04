using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CComponentInfo
    {
        public string Prefix;
        public string ComponentName;
        public string Section;
        public string Material;
        public bool Generate;
        public bool Display;
        public bool Calculate;
        public bool Design;
        public bool MaterialList;

        private List<string> MSections;

        public List<string> Sections
        {
            get
            {
                if (MSections == null)
                {
                    MSections = new List<string> { "Box 63020", "Box 63020", "C 50020", "C 27095", "C 270115", "Box 10075", "Box 10075", "C 27095", "C 27095" };
                }                
                return MSections;
            }
        }

        public CComponentInfo(string prefix, string componentName, string section, string material, bool generate, bool display, bool calculate, bool design, bool materialList)
        {
            Prefix = prefix;
            ComponentName = componentName;
            Section = section;
            Material = material;
            Generate = generate;
            Display = display;
            Calculate = calculate;
            Design = design;
            MaterialList = materialList;
        }
    }
}
