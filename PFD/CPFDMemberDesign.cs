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
    public class CPFDMemberDesign : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MLimiStateIndex;
        private int MLoadCombinationIndex;
        private int MComponentTypeIndex;

        private List<string> MComponentsNames;
        private ObservableCollection<CComponentInfo> MComponentList;

        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        public int LimiStateIndex
        {
            get
            {
                return MLimiStateIndex;
            }

            set
            {
                MLimiStateIndex = value;

                NotifyPropertyChanged("LimiStateIndex");
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

        public List<string> ComponentsNames
        {
            get
            {
                return MComponentsNames;
            }

            set
            {
                MComponentsNames = value;
                NotifyPropertyChanged("ComponentsNames");
            }
        }

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

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDMemberDesign(ObservableCollection<CComponentInfo> componentList)
        {
            //MComponentsNames = componentsNames;
            MComponentList = componentList;

            // Set default
            LimiStateIndex = 0;
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
