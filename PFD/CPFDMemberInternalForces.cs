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

namespace PFD
{
    public class CPFDMemberInternalForces : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MLimitStateIndex;
        private int MLoadCombinationIndex;
        private int MComponentTypeIndex;

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        public int LimitStateIndex
        {
            get
            {
                return MLimitStateIndex;
            }

            set
            {
                MLimitStateIndex = value;

                NotifyPropertyChanged("LimitStateIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int LoadCombinationIndex
        {
            get
            {
                return MLoadCombinationIndex;
            }

            set
            {
                MLoadCombinationIndex = value;

                NotifyPropertyChanged("LoadCombinationIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int ComponentTypeIndex
        {
            get
            {
                return MComponentTypeIndex;
            }

            set
            {
                MComponentTypeIndex = value;

                NotifyPropertyChanged("ComponentTypeIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDMemberInternalForces()
        {
            // Set default
            LimitStateIndex = 0;
            LoadCombinationIndex = 0;
            ComponentTypeIndex = 0;

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
