using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    [Serializable]
    public class CComponentParamsViewList : CComponentParamsView
    {
        //-------------------------------------------------------------------------------------------------------------
        private string MValue;
        private ObservableCollection<string> MValues;

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

        public ObservableCollection<string> Values
        {
            get
            {
                if (MValues == null) MValues = new ObservableCollection<string>();
                return MValues;
            }

            set
            {
                MValues = value;
                NotifyPropertyChanged("Values");
            }
        }


        public CComponentParamsViewList(string name, string shortcut, string value, List<string> values, string unit) : base(name, shortcut, unit, "ComboBox")
        {   
            MValue = value;
            MValues = new ObservableCollection<string>(values);
        }
    }
}
