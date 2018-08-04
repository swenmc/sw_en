using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CComponentInfo
    {
        private string MPrefix;
        private string MComponentName;
        private string MSection;
        private string MMaterial;
        private bool MGenerate;
        private bool MDisplay;
        private bool MCalculate;
        private bool MDesign;
        private bool MMaterialList;

        public string Prefix
        {
            get
            {
                return MPrefix;
            }

            set
            {
                MPrefix = value;
            }
        }

        public string ComponentName
        {
            get
            {
                return MComponentName;
            }

            set
            {
                MComponentName = value;
            }
        }

        public string Section
        {
            get
            {
                return MSection;
            }

            set
            {
                MSection = value;
            }
        }

        public string Material
        {
            get
            {
                return MMaterial;
            }

            set
            {
                MMaterial = value;
            }
        }

        public bool Generate
        {
            get
            {
                return MGenerate;
            }

            set
            {
                MGenerate = value;
            }
        }

        public bool Display
        {
            get
            {
                return MDisplay;
            }

            set
            {
                MDisplay = value;
            }
        }

        public bool Calculate
        {
            get
            {
                return MCalculate;
            }

            set
            {
                MCalculate = value;
            }
        }

        public bool Design
        {
            get
            {
                return MDesign;
            }

            set
            {
                MDesign = value;
            }
        }

        public bool MaterialList
        {
            get
            {
                return MMaterialList;
            }

            set
            {
                MMaterialList = value;
            }
        }

        public CComponentInfo(string prefix, string componentName, string section, string material, bool generate, bool display, bool calculate, bool design, bool materialList)
        {
            MPrefix = prefix;
            MComponentName = componentName;
            MSection = section;
            MMaterial = material;
            MGenerate = generate;
            MDisplay = display;
            MCalculate = calculate;
            MDesign = design;
            MMaterialList = materialList;
        }
    }
}
