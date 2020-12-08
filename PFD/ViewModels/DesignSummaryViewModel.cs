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
    public class DesignSummaryViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int m_LimitStateIndex_MD;
        private int m_LimitStateIndex_JD;
        private int m_LimitStateIndex_FD;
                
        private CLimitState[] m_LimitStates;

        private List<MemberDesignResultItem> m_MemberDesignResultsSummary;
                
        public bool IsSetFromCode = false;

        sDesignResults DesignResults_ULSandSLS;
        sDesignResults DesignResults_ULS;
        sDesignResults DesignResults_SLS;
        List<CComponentInfo> ComponentList;
        //-------------------------------------------------------------------------------------------------------------
        public int LimitStateIndex_MD
        {
            get
            {
                return m_LimitStateIndex_MD;
            }

            set
            {
                m_LimitStateIndex_MD = value;

                LoadMemberDesignSummaryResults();

                NotifyPropertyChanged("LimitStateIndex_MD");
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        public int LimitStateIndex_JD
        {
            get
            {
                return m_LimitStateIndex_JD;
            }

            set
            {
                m_LimitStateIndex_JD = value;                

                NotifyPropertyChanged("LimitStateIndex_JD");
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        public int LimitStateIndex_FD
        {
            get
            {
                return m_LimitStateIndex_FD;
            }

            set
            {
                m_LimitStateIndex_FD = value;                

                NotifyPropertyChanged("LimitStateIndex_FD");
            }
        }
        
        public CLimitState[] LimitStates
        {
            get
            {
                return m_LimitStates;
            }

            set
            {
                m_LimitStates = value;
                NotifyPropertyChanged("LimitStates");
            }
        }
        

        public List<MemberDesignResultItem> MemberDesignResultsSummary
        {
            get
            {
                return m_MemberDesignResultsSummary;
            }

            set
            {
                m_MemberDesignResultsSummary = value;
                NotifyPropertyChanged("MemberDesignResultsSummary");
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DesignSummaryViewModel(CLimitState[] limitStates, List<CComponentInfo> componentList, sDesignResults sDesignResults_ULSandSLS, sDesignResults sDesignResults_ULS, sDesignResults sDesignResults_SLS)
        {
            List<CLimitState> listLimitStates = new List<CLimitState>() { new CLimitState("All", ELSType.eLS_ALL) };
            listLimitStates.AddRange(limitStates);
            m_LimitStates = listLimitStates.ToArray();

            DesignResults_ULSandSLS = sDesignResults_ULSandSLS;
            DesignResults_ULS = sDesignResults_ULS;
            DesignResults_SLS = sDesignResults_SLS;
            ComponentList = componentList;

            // Set default
            LimitStateIndex_MD = 0;
            LimitStateIndex_JD = 0;
            LimitStateIndex_FD = 0;            

            IsSetFromCode = false;
        }

        private void LoadMemberDesignSummaryResults()
        {
            CLimitState limitState = m_LimitStates[m_LimitStateIndex_MD];

            if (limitState.eLS_Type == ELSType.eLS_ALL)
            {
                FillMemberDesignSummaryResults(DesignResults_ULSandSLS);
            }
            else if (limitState.eLS_Type == ELSType.eLS_ULS)
            {
                FillMemberDesignSummaryResults(DesignResults_ULS);
            }
            else if (limitState.eLS_Type == ELSType.eLS_SLS)
            {
                FillMemberDesignSummaryResults(DesignResults_SLS);
            }
        }

        private void FillMemberDesignSummaryResults(sDesignResults designResults)
        {
            List<MemberDesignResultItem> items = new List<MemberDesignResultItem>();
            foreach (CComponentInfo ci in ComponentList)
            {
                DesignResultItem res = null;
                designResults.DesignResults.TryGetValue(ci.MemberTypePosition, out res);

                if (res != null && res.GoverningLoadCombination != null && res.MemberWithMaximumDesignRatio != null)
                    items.Add(new MemberDesignResultItem(ci.Prefix, ci.ComponentName, res.GoverningLoadCombination.Name, res.MemberWithMaximumDesignRatio.ID, res.MaximumDesignRatio));

            }
            MemberDesignResultsSummary = items;
        }


        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
