using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CComponentParamsViewBool: CComponentParamsView
    {
        //-------------------------------------------------------------------------------------------------------------
        private bool MValue;
        

        public bool Value
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
        

        public CComponentParamsViewBool(string name, string shortcut, bool value, string unit) : base(name, shortcut, unit, "CheckBox")
        {   
            MValue = value;            
        }

        


    }
}
