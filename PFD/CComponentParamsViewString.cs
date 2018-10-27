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

        public CComponentParamsViewString(string name, string shortcut, string value, string unit) : base(name, shortcut, unit, "TextBox")
        {
            MValue = value;
        }
    }
}
