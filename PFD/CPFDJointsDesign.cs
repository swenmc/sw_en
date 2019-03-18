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
    public class CPFDJointsDesign : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MLimitStateIndex;
        private int MComponentTypeIndex;
        private int MSelectedLoadCombinationID;

        private ObservableCollection<CComponentInfo> MComponentList;
        private List<CLimitState> MLimitStates;
        private List<CLoadCombination> MLoadCombinations;

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
        //public int LoadCombinationIndex
        //{
        //    get
        //    {
        //        return MLoadCombinationIndex;
        //    }

        //    set
        //    {
        //        MLoadCombinationIndex = value;
        //        //TODO No. 68
        //        NotifyPropertyChanged("LoadCombinationIndex");
        //    }
        //}

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
        
        public List<CLimitState> LimitStates
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

        public List<CLoadCombination> LoadCombinations
        {
            get
            {
                return MLoadCombinations;
            }

            set
            {
                MLoadCombinations = value;
                NotifyPropertyChanged("LoadCombinations");
                SelectedLoadCombinationID = MLoadCombinations[0].ID;
            }
        }
        public int SelectedLoadCombinationID
        {
            get
            {
                return MSelectedLoadCombinationID;
            }

            set
            {
                MSelectedLoadCombinationID = value;
                NotifyPropertyChanged("SelectedLoadCombinationID");
            }
        }

        private CLoadCombination[] m_allLoadCombinations;

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
        public CPFDJointsDesign(CLimitState[] limitStates, CLoadCombination[] allLoadCombinations, ObservableCollection<CComponentInfo> componentList)
        {
            SetLimitStates(limitStates);
            MComponentList = componentList;
            m_allLoadCombinations = allLoadCombinations;


            // Set default
            LimitStateIndex = 0;
            ComponentTypeIndex = 0;

            IsSetFromCode = false;
        }

        private void SetLimitStates(CLimitState[] modelLimitStates)
        {
            MLimitStates = new List<CLimitState>();
            foreach (CLimitState ls in modelLimitStates)
            {
                if (ls.eLS_Type == ELSType.eLS_SLS) continue;  //task 213 = vynechanie SLS

                MLimitStates.Add(ls);
            }
        }

        private void SetLoadCombinations()
        {
            CLimitState limitState = LimitStates[LimitStateIndex];

            List<CLoadCombination> loadCombinations = new List<CLoadCombination>();
            foreach (CLoadCombination lc in m_allLoadCombinations)
            {
                if (lc.eLComType == limitState.eLS_Type) loadCombinations.Add(lc);
            }
            LoadCombinations = loadCombinations;
        }
        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
