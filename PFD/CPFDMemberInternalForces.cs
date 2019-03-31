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
        private int MComponentTypeIndex;
        private int MSelectedLoadCombinationID;

        private List<string> MComponentList;
        private CLimitState[] MLimitStates;
        private List<ComboItem> MLoadCombinations;

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
        //        if(value != -1) MLoadCombinationIndex = value;
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

        public List<string> ComponentList
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

        public List<ComboItem> LoadCombinations
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
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDMemberInternalForces(CLimitState[] limitStates, CLoadCombination[] allLoadCombinations, ObservableCollection<CComponentInfo> componentList)
        {
            MLimitStates = limitStates;
            MComponentList = componentList.Where(s => s.Calculate == true).Select(s => s.ComponentName).ToList();
            m_allLoadCombinations = allLoadCombinations;

            // Set default
            LimitStateIndex = 0;
            ComponentTypeIndex = 0;

            IsSetFromCode = false;
        }
                
        private void SetLoadCombinations()
        {
            CLimitState limitState = LimitStates[LimitStateIndex];

            List<ComboItem> loadCombinations = new List<ComboItem>();
            foreach (CLoadCombination lc in m_allLoadCombinations)
            {
                if (lc.eLComType == limitState.eLS_Type) loadCombinations.Add(new ComboItem(lc.ID, $"{lc.Name}\t{lc.CombinationKey}"));
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
