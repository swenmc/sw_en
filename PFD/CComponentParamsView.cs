using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CComponentParamsView
    {
        private string MName;
        private string MShortCut;
        private string MValue;
        private string MUnit;

        public string Name
        {
            get
            {
                return MName;
            }

            set
            {
                MName = value;
            }
        }

        public string ShortCut
        {
            get
            {
                return MShortCut;
            }

            set
            {
                MShortCut = value;
            }
        }

        public string Value
        {
            get
            {
                return MValue;
            }

            set
            {
                MValue = value;
            }
        }

        public string Unit
        {
            get
            {
                return MUnit;
            }

            set
            {
                MUnit = value;
            }
        }

        public CComponentParamsView(string name, string shortcut, string value, string unit)
        {
            MName = name;
            MShortCut = shortcut;
            MValue = value;
            MUnit = unit;
        }
    }
}
