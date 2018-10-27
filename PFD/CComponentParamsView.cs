using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    [Serializable]
    public class CComponentParamsView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //-------------------------------------------------------------------------------------------------------------
        private string MName;
        private string MShortCut;
        private string MUnit;
        private string MCheckType;

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

        //public string Value
        //{
        //    get
        //    {
        //        return MValue;
        //    }

        //    set
        //    {
        //        MValue = value;
        //        //NotifyPropertyChanged("Value");
        //    }
        //}

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

        public string CheckType
        {
            get
            {
                return MCheckType;
            }

            set
            {
                MCheckType = value;
            }
        }

        public CComponentParamsView(string name, string shortcut, string unit, string checkType)
        {
            MName = name;
            MShortCut = shortcut;
            //MValue = value;
            MUnit = unit;
            MCheckType = checkType;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
