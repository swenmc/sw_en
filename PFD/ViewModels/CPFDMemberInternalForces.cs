﻿using System;
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

        private bool MComponentListHasFrameMembers;
        private bool MIsFrameMember;
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

                CComponentInfo ci = m_componentList.ElementAtOrDefault(MComponentTypeIndex);
                if (ci != null)
                {
                    IsFrameMember = ci.MemberTypePosition == EMemberType_FS_Position.MainColumn || ci.MemberTypePosition == EMemberType_FS_Position.MainRafter ||
                        ci.MemberTypePosition == EMemberType_FS_Position.EdgeColumn || ci.MemberTypePosition == EMemberType_FS_Position.EdgeRafter;
                }

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
                MComponentTypeIndex = 0;
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
        public bool ComponentListHasFrameMembers
        {
            get
            {
                return MComponentListHasFrameMembers;
            }

            set
            {
                MComponentListHasFrameMembers = value;
            }
        }
        public bool IsFrameMember
        {
            get
            {
                return MIsFrameMember;
            }

            set
            {
                MIsFrameMember = value;
            }
        }

        private CLoadCombination[] m_allLoadCombinations;
        private List<CComponentInfo> m_componentList;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDMemberInternalForces(CLimitState[] limitStates, CLoadCombination[] allLoadCombinations, ObservableCollection<CComponentInfo> componentList)
        {
            MLimitStates = limitStates;
            SetComponentList(componentList);
            m_allLoadCombinations = allLoadCombinations;
            

            // Set default
            LimitStateIndex = 0;
            ComponentTypeIndex = 0;

            IsSetFromCode = false;
        }
        public void SetComponentList(ObservableCollection<CComponentInfo> componentList)
        {
            ComponentList = componentList.Where(s => s.Generate == true && s.Calculate == true).Select(s => s.ComponentName).ToList();
            m_componentList = componentList.Where(s => s.Generate == true && s.Calculate == true).ToList();
            ComponentListHasFrameMembers = componentList.Where(c => c.Generate == true && c.Calculate == true).Any(c => c.MemberTypePosition == EMemberType_FS_Position.MainColumn || c.MemberTypePosition == EMemberType_FS_Position.MainRafter ||
                c.MemberTypePosition == EMemberType_FS_Position.EdgeColumn || c.MemberTypePosition == EMemberType_FS_Position.EdgeRafter);
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
