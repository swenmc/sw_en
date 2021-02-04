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
using BaseClasses.Helpers;

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
        private List<JointDesignResultItem> m_FootingDesignResultsSummary;

        public bool IsSetFromCode = false;

        sDesignResults DesignResults_ULSandSLS;
        sDesignResults DesignResults_ULS;
        sDesignResults DesignResults_SLS;
        List<CComponentInfo> ComponentList;
        List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;
        List<CFootingLoadCombinationRatio_ULS> FootingDesignResults_ULS;

        CModel_PFD Model;
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

                LoadFootingDesignSummaryResults();

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
        public List<JointDesignResultItem> FootingDesignResultsSummary
        {
            get
            {
                return m_FootingDesignResultsSummary;
            }

            set
            {
                m_FootingDesignResultsSummary = value;
                NotifyPropertyChanged("FootingDesignResultsSummary");
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
        public DesignSummaryViewModel(CModel_PFD model, List<CComponentInfo> componentList, sDesignResults sDesignResults_ULSandSLS, sDesignResults sDesignResults_ULS, 
            sDesignResults sDesignResults_SLS, List<CJointLoadCombinationRatio_ULS> jointDesignResults_ULS, List<CFootingLoadCombinationRatio_ULS> footingDesignResults_ULS)
        {
            Model = model;
            List<CLimitState> listLimitStates = new List<CLimitState>() { new CLimitState("All", ELSType.eLS_ALL) };
            listLimitStates.AddRange(model.m_arrLimitStates);
            m_LimitStates = listLimitStates.ToArray();
            m_LimitStatesJoints = m_LimitStates.Where(ls => ls.eLS_Type != ELSType.eLS_SLS).ToArray();

            DesignResults_ULSandSLS = sDesignResults_ULSandSLS;
            DesignResults_ULS = sDesignResults_ULS;
            DesignResults_SLS = sDesignResults_SLS;
            JointDesignResults_ULS = jointDesignResults_ULS;
            FootingDesignResults_ULS = footingDesignResults_ULS;
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
                CJointLoadCombinationRatio_ULS res = CJointHelper.FindResultWithMaximumDesignRatio(JointDesignResults_ULS.Where(j => j.Joint != null && (int)j.Joint.JointType == c.ID));
                if(res != null) items.Add(new JointDesignResultItem(c.Name, c.JoinType, res.LoadCombination.Name, res.Joint.ID, res.MaximumDesignRatio));
            }
            JointDesignResultsSummary = items;
        }

        private void LoadFootingDesignSummaryResults()
        {
            CLimitState limitState = m_LimitStates[m_LimitStateIndex_JD];

            if (limitState.eLS_Type == ELSType.eLS_ALL)
            {
                FillFootingDesignSummaryResults();
            }
            else if (limitState.eLS_Type == ELSType.eLS_ULS)
            {
                FillFootingDesignSummaryResults();
            }
            else if (limitState.eLS_Type == ELSType.eLS_SLS)
            {
                JointDesignResultsSummary = new List<JointDesignResultItem>();
            }
        }

        private void FillFootingDesignSummaryResults()
        {
            List<JointDesignResultItem> items = new List<JointDesignResultItem>();

            IEnumerable<CFootingLoadCombinationRatio_ULS> ta_results_MC = FootingDesignResults_ULS.Where(j => j.Joint is CConnectionJoint_TA01 && j.Member.EMemberTypePosition == EMemberType_FS_Position.MainColumn);
            IEnumerable<CFootingLoadCombinationRatio_ULS> ta_results_EC = FootingDesignResults_ULS.Where(j => j.Joint is CConnectionJoint_TA01 && j.Member.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);
            IEnumerable<CFootingLoadCombinationRatio_ULS> tb_results_WPF = FootingDesignResults_ULS.Where(j => j.Joint is CConnectionJoint_TB01 && j.Member.EMemberTypePosition == EMemberType_FS_Position.WindPostFrontSide);
            IEnumerable<CFootingLoadCombinationRatio_ULS> tb_results_WPB = FootingDesignResults_ULS.Where(j => j.Joint is CConnectionJoint_TB01 && j.Member.EMemberTypePosition == EMemberType_FS_Position.WindPostBackSide);
            IEnumerable<CFootingLoadCombinationRatio_ULS> tc_results = FootingDesignResults_ULS.Where(j => j.Joint is CConnectionJoint_TC01);
            IEnumerable<CFootingLoadCombinationRatio_ULS> td_results = FootingDesignResults_ULS.Where(j => j.Joint is CConnectionJoint_TD01);

            CFootingLoadCombinationRatio_ULS res_ta_MC = CJointHelper.FindResultWithMaximumDesignRatio(ta_results_MC);
            CFootingLoadCombinationRatio_ULS res_ta_EC = CJointHelper.FindResultWithMaximumDesignRatio(ta_results_EC);
            CFootingLoadCombinationRatio_ULS res_tb_WPF = CJointHelper.FindResultWithMaximumDesignRatio(tb_results_WPF);
            CFootingLoadCombinationRatio_ULS res_tb_WPB = CJointHelper.FindResultWithMaximumDesignRatio(tb_results_WPB);
            CFootingLoadCombinationRatio_ULS res_tc = CJointHelper.FindResultWithMaximumDesignRatio(tc_results);
            CFootingLoadCombinationRatio_ULS res_td = CJointHelper.FindResultWithMaximumDesignRatio(td_results);

            if (res_ta_MC != null) items.Add(new JointDesignResultItem(res_ta_MC.Member.Name, GetFootingTypeAcordingToMemberType(res_ta_MC.Member.EMemberTypePosition), res_ta_MC.LoadCombination.Name, res_ta_MC.Joint.ID, res_ta_MC.MaximumDesignRatio));
            if (res_ta_EC != null) items.Add(new JointDesignResultItem(res_ta_EC.Member.Name, GetFootingTypeAcordingToMemberType(res_ta_EC.Member.EMemberTypePosition), res_ta_EC.LoadCombination.Name, res_ta_EC.Joint.ID, res_ta_EC.MaximumDesignRatio));
            if (res_tb_WPF != null) items.Add(new JointDesignResultItem(res_tb_WPF.Member.Name, GetFootingTypeAcordingToMemberType(res_tb_WPF.Member.EMemberTypePosition), res_tb_WPF.LoadCombination.Name, res_tb_WPF.Joint.ID, res_tb_WPF.MaximumDesignRatio));
            if (res_tb_WPB != null) items.Add(new JointDesignResultItem(res_tb_WPB.Member.Name, GetFootingTypeAcordingToMemberType(res_tb_WPB.Member.EMemberTypePosition), res_tb_WPB.LoadCombination.Name, res_tb_WPB.Joint.ID, res_tb_WPB.MaximumDesignRatio));
            if (res_tc != null) items.Add(new JointDesignResultItem(res_tc.Member.Name, GetFootingTypeAcordingToMemberType(res_tc.Member.EMemberTypePosition), res_tc.LoadCombination.Name, res_tc.Joint.ID, res_tc.MaximumDesignRatio));
            if (res_td != null) items.Add(new JointDesignResultItem(res_td.Member.Name, GetFootingTypeAcordingToMemberType(res_td.Member.EMemberTypePosition), res_td.LoadCombination.Name, res_td.Joint.ID, res_td.MaximumDesignRatio));
            
            FootingDesignResultsSummary = items;
        }

        private string GetFootingTypeAcordingToMemberType(EMemberType_FS_Position pos)
        {
            switch (pos)
            {
                case EMemberType_FS_Position.MainColumn: return "A";
                case EMemberType_FS_Position.EdgeColumn: return "B";
                case EMemberType_FS_Position.WindPostFrontSide: return "C";
                case EMemberType_FS_Position.WindPostBackSide: return "D";                
            }
            return "";
        }


        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
