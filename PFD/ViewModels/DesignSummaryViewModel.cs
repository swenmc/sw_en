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
using DATABASE.DTO;
using DATABASE;

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

        private List<CConnectionDescription> m_AllJointTypes;

        private CLimitState[] m_LimitStates;
        private CLimitState[] m_LimitStatesJoints;

        private List<MemberDesignResultItem> m_MemberDesignResultsSummary;
        private List<JointDesignResultItem> m_JointDesignResultsSummary;

        public bool IsSetFromCode = false;

        sDesignResults DesignResults_ULSandSLS;
        sDesignResults DesignResults_ULS;
        sDesignResults DesignResults_SLS;
        List<CComponentInfo> ComponentList;
        List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;
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

                LoadJointDesignSummaryResults();

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
        public CLimitState[] LimitStatesJoints
        {
            get
            {
                return m_LimitStatesJoints;
            }

            set
            {
                m_LimitStatesJoints = value;
                NotifyPropertyChanged("LimitStatesJoints");
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

        public List<JointDesignResultItem> JointDesignResultsSummary
        {
            get
            {
                return m_JointDesignResultsSummary;
            }

            set
            {
                m_JointDesignResultsSummary = value;
                NotifyPropertyChanged("JointDesignResultsSummary");
            }
        }

        public List<CConnectionDescription> AllJointTypes
        {
            get
            {
                if(m_AllJointTypes == null)
                    m_AllJointTypes = CJointsManager.LoadJointsConnectionDescriptions();
                return m_AllJointTypes;
            }
        }

        



        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DesignSummaryViewModel(CLimitState[] limitStates, List<CComponentInfo> componentList, sDesignResults sDesignResults_ULSandSLS, sDesignResults sDesignResults_ULS, sDesignResults sDesignResults_SLS, List<CJointLoadCombinationRatio_ULS> jointDesignResults_ULS)
        {
            List<CLimitState> listLimitStates = new List<CLimitState>() { new CLimitState("All", ELSType.eLS_ALL) };
            listLimitStates.AddRange(limitStates);
            m_LimitStates = listLimitStates.ToArray();
            m_LimitStatesJoints = m_LimitStates.Where(ls => ls.eLS_Type != ELSType.eLS_SLS).ToArray();

            DesignResults_ULSandSLS = sDesignResults_ULSandSLS;
            DesignResults_ULS = sDesignResults_ULS;
            DesignResults_SLS = sDesignResults_SLS;
            JointDesignResults_ULS = jointDesignResults_ULS;
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

        private void LoadJointDesignSummaryResults()
        {
            CLimitState limitState = m_LimitStates[m_LimitStateIndex_JD];

            if (limitState.eLS_Type == ELSType.eLS_ALL)
            {
                FillJointDesignSummaryResults();
            }
            else if (limitState.eLS_Type == ELSType.eLS_ULS)
            {
                FillJointDesignSummaryResults();
            }
            else if (limitState.eLS_Type == ELSType.eLS_SLS)
            {
                JointDesignResultsSummary = new List<JointDesignResultItem>();
            }
        }

        private void FillJointDesignSummaryResults()
        {
            List<JointDesignResultItem> items = new List<JointDesignResultItem>();

            foreach (CConnectionDescription c in AllJointTypes)
            {
                CJointLoadCombinationRatio_ULS res = FindResultWithMaximumDesignRatio(JointDesignResults_ULS.Where(j => (int)j.Joint.JointType == c.ID));
                if(res != null) items.Add(new JointDesignResultItem(c.Name, c.JoinType, res.LoadCombination.Name, res.Member.ID, res.MaximumDesignRatio));
            }
            
            JointDesignResultsSummary = items;
        }


        private CJointLoadCombinationRatio_ULS FindResultWithMaximumDesignRatio(IEnumerable<CJointLoadCombinationRatio_ULS> results)
        {
            CJointLoadCombinationRatio_ULS result = null;
            float maxDesignRatio = float.MinValue;
            foreach (CJointLoadCombinationRatio_ULS r in results)
            {
                if (r.MaximumDesignRatio > maxDesignRatio) { result = r; maxDesignRatio = r.MaximumDesignRatio; }

            }
            return result;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
