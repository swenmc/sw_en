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
    public class CPFDMemberInternalForces : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MLimitStateIndex;
        private int MLoadCombinationIndex;
        private int MComponentTypeIndex;

        private ObservableCollection<CComponentInfo> MComponentList;
        private CLimitState[] MLimitStates;
        private ObservableCollection<CLoadCombination> MLoadCombinations;

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

                SetLoadCombinations();

                //TODO No. 68
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
                if(value != -1) MLoadCombinationIndex = value;
                //TODO No. 68
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
                //TODO No. 68
                NotifyPropertyChanged("ComponentTypeIndex");
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

        public CLimitState[] LimitStates
        {
            get
            {
                return MLimitStates;
            }

            set
            {
                MLimitStates = value;
                NotifyPropertyChanged("LimitStates");
            }
        }

        public ObservableCollection<CLoadCombination> LoadCombinations
        {
            get
            {
                return MLoadCombinations;
            }

            set
            {
                MLoadCombinations = value;                
                NotifyPropertyChanged("LoadCombinations");                
            }
        }

        private CLoadCombination[] m_allLoadCombinations;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDMemberInternalForces(CLimitState[] limitStates, CLoadCombination[] allLoadCombinations, ObservableCollection<CComponentInfo> componentList)
        {
            MLimitStates = limitStates;            
            MComponentList = componentList;
            m_allLoadCombinations = allLoadCombinations;


            // Set default
            LimitStateIndex = 0;            
            ComponentTypeIndex = 0;

            IsSetFromCode = false;
        }

        private void SetLoadCombinations()
        {
            CLimitState limitState = LimitStates[LimitStateIndex];

            ObservableCollection<CLoadCombination> loadCombinations = new ObservableCollection<CLoadCombination>();
            foreach (CLoadCombination lc in m_allLoadCombinations)
            {
                if (lc.eLComType == limitState.eLS_Type) loadCombinations.Add(lc);
            }
            LoadCombinations = loadCombinations;
            LoadCombinationIndex = 0;       
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
