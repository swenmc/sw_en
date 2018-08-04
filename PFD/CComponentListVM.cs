using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Globalization;
using BaseClasses;
using System.Collections.ObjectModel;

namespace PFD
{
    public class CComponentListVM : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<CComponentInfo> MComponentList;
        private List<string> MSections;
        
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------        
        public ObservableCollection<CComponentInfo> ComponentList
        {
            get
            {
                return MComponentList;
            }

            set
            {
                MComponentList = value;
                NotifyPropertyChanged("ComponentList");
            }
        }
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

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CComponentListVM()
        {
            MComponentList = new ObservableCollection<CComponentInfo>();
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
