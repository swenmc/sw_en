using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    [Serializable]
    public class CComponentParamsViewString : CComponentParamsView
    {
        //-------------------------------------------------------------------------------------------------------------
        private string MValue;

        public string Value
        {
            get
            {
                return MValue;
            }

            set
            {
                MValue = value;
                NotifyPropertyChanged("Value");
            }
        }

        public CComponentParamsViewString(string name, string shortcut, string value, string unit, bool isEnabled = true) : base(name, shortcut, unit, "TextBox", isEnabled)
        {
            MValue = value;
        }

        public CComponentParamsViewString(string name, string shortcut, string value, string unit, string checkType, bool isEnabled = true) : base(name, shortcut, unit, checkType, isEnabled)
        {
            MValue = value;
        }

    }
}
