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

                // TODO No. 68 - Pri zmene LimitState nacitat load combinations, ktora patria k danemu limit state a maju spocitane vysledky
                // PODOBNE PRE INTERNAL FORCES

                NotifyPropertyChanged("LimitStateIndex");
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

                // TODO No. 68 - Pri zmene typpu pruta, spustit vypocet pre vsetky pruty vybraneho typu pre vybranu kombinaciu a zobrazit vysledky pre najnevyhodnejsi z nich, max design ratio

                NotifyPropertyChanged("ComponentTypeIndex");
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
        public CPFDMemberDesign(CLimitState[] limitStates, CLoadCombination[] allLoadCombinations, ObservableCollection<CComponentInfo> componentList)
        {
            List<CLimitState> listLimitStates = limitStates.ToList();
            listLimitStates.Add(new CLimitState("All", ELSType.eLS_ALL));
            MLimitStates = listLimitStates.ToArray();            

            SetComponentList(componentList);            
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
                if (lc.eLComType == limitState.eLS_Type || limitState.eLS_Type == ELSType.eLS_ALL) loadCombinations.Add(new ComboItem(lc.ID, $"{lc.Name}\t{lc.CombinationKey}"));
            }

            loadCombinations.Add(new ComboItem(-1, "Envelope"));
            LoadCombinations = loadCombinations;
        }

        
        public void SetComponentList(ObservableCollection<CComponentInfo> componentList)
        {
            ComponentList = componentList.Where(s => s.Generate == true && s.Calculate == true && s.Design == true).Select(s => s.ComponentName).ToList();
            ComponentList.Add("All");
        }
        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
