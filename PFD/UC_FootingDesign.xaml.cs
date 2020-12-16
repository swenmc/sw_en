using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BaseClasses;
using M_AS4600;
using BaseClasses.Helpers;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_FootingDesign.xaml
    /// </summary>
    public partial class UC_FootingDesign : UserControl
    {
        bool UseCRSCGeometricalAxes;        
        CPFDFootingDesign vm;

        private CPFDViewModel _pfdVM;        
        //public List<CJointLoadCombinationRatio_ULS> DesignResults_ULS;
        public List<CFootingLoadCombinationRatio_ULS> FootingResults_ULS;

        public UC_FootingDesign(bool bUseCRSCGeometricalAxes_temp, CPFDViewModel pfdVM, CComponentListVM compList)
        {
            InitializeComponent();

            UseCRSCGeometricalAxes = bUseCRSCGeometricalAxes_temp;
            _pfdVM = pfdVM;

            //DesignResults_ULS = _pfdVM.JointDesignResults_ULS;
            FootingResults_ULS = _pfdVM.FootingDesignResults_ULS;

            // Footing Design
            vm = new CPFDFootingDesign(_pfdVM.Model.m_arrLimitStates, _pfdVM.Model.m_arrLoadCombs, compList.ComponentList);
            vm.PropertyChanged += HandleFootingDesignPropertyChangedEvent;
            this.DataContext = vm;

            vm.LimitStateIndex = 0;
        }

        protected void HandleFootingDesignPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDFootingDesign vm = sender as CPFDFootingDesign;
            if (vm != null && vm.IsSetFromCode) return;
            if (vm.ComponentTypeIndex == -1) return;

            if (vm.ComponentList.Count == 0) { Results_FootingPad_GridView.DataContext = null; this.IsEnabled = false; return; }
            else this.IsEnabled = true;
            
            // Calculate governing member design ratio in member group
            CCalculJoint cGoverningMemberFootingResults;
            CalculateGoverningMemberDesignDetails(vm.SelectedLoadCombinationID, out cGoverningMemberFootingResults);
        }

        // Calculate governing member design ratio
        public void CalculateGoverningMemberDesignDetails(int loadCombinationID, out CCalculJoint cGoverningMemberFootingResults)
        {
            cGoverningMemberFootingResults = null;

            CalculationSettingsFoundation footingCalcSettings = _pfdVM.FootingVM.GetCalcSettings();

            if (FootingResults_ULS != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {                
                CFootingLoadCombinationRatio_ULS res_FLCR = null;
                IEnumerable<CFootingLoadCombinationRatio_ULS> results;
                if (vm.ComponentList[vm.ComponentTypeIndex] == "All")
                {
                    if (loadCombinationID == -1) //envelope
                    {
                        results = FootingResults_ULS;
                        res_FLCR = CJointHelper.FindResultWithMaximumDesignRatio(results);

                        loadCombinationID = res_FLCR.LoadCombination.ID;
                        textGoverningLoadComb.Text = BaseHelper.GetGoverningLoadCombText(res_FLCR.LoadCombination);
                    }
                    else
                    {
                        results = FootingResults_ULS.Where(j => j.LoadCombination.ID == loadCombinationID);
                        res_FLCR = CJointHelper.FindResultWithMaximumDesignRatio(results);
                        textGoverningLoadComb.Text = "";
                    }
                    textGoverningMember.Text = BaseHelper.GetGoverningMemberText(res_FLCR.Member);
                }
                else
                {
                    CMemberGroup GroupOfMembersWithSelectedType = _pfdVM.Model.listOfModelMemberGroups.FirstOrDefault(c => c.Name == vm.ComponentList[vm.ComponentTypeIndex]);
                    if (loadCombinationID == -1) //envelope
                    {
                        results = FootingResults_ULS.Where(j => j.Member.EMemberTypePosition == GroupOfMembersWithSelectedType.MemberType_FS_Position);
                        res_FLCR = CJointHelper.FindResultWithMaximumDesignRatio(results);

                        loadCombinationID = res_FLCR.LoadCombination.ID;
                        textGoverningLoadComb.Text = BaseHelper.GetGoverningLoadCombText(res_FLCR.LoadCombination);
                    }
                    else
                    {
                        results = FootingResults_ULS.Where(j => j.Member.EMemberTypePosition == GroupOfMembersWithSelectedType.MemberType_FS_Position && j.LoadCombination.ID == loadCombinationID);
                        res_FLCR = CJointHelper.FindResultWithMaximumDesignRatio(results);
                        textGoverningLoadComb.Text = "";
                    }
                    textGoverningMember.Text = "";
                }
                                
                CCalculJoint cJoint = new CCalculJoint(false, UseCRSCGeometricalAxes, _pfdVM._designOptionsVM.ShearDesignAccording334, _pfdVM._designOptionsVM.UniformShearDistributionInAnchors, 
                    res_FLCR.Joint, _pfdVM.Model, footingCalcSettings, res_FLCR.DesignInternalForces, true);
                cGoverningMemberFootingResults = cJoint;

                // Display calculation results - details
                if (cGoverningMemberFootingResults != null)
                    cGoverningMemberFootingResults.DisplayFootingDesignResultsInGridView(Results_FootingPad_GridView);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available! [FootingDesign]");
                }
            }
        }

        //odlozene stare funkcie
        //protected void HandleFootingDesignPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        //{
        //    if (sender == null) return;
        //    CPFDFootingDesign vm = sender as CPFDFootingDesign;
        //    if (vm != null && vm.IsSetFromCode) return;
        //    if (vm.ComponentTypeIndex == -1) return;

        //    if (vm.ComponentList.Count == 0) { Results_FootingPad_GridView.DataContext = null; this.IsEnabled = false; return; }
        //    else this.IsEnabled = true;

        //    CMemberGroup GroupOfMembersWithSelectedType = null;

        //    if (vm.ComponentList[vm.ComponentTypeIndex] == "All") //All
        //    {
        //        CMemberGroup gr = _pfdVM.Model.listOfModelMemberGroups.FirstOrDefault(c => c.MemberType_FS_Position == _pfdVM.sDesignResults_ULS.MaximumDesignRatioWholeStructureMember.EMemberTypePosition);
        //        if (gr != null) gr = gr.Clone();
        //        gr.ListOfMembers = new List<CMember> { _pfdVM.sDesignResults_ULS.MaximumDesignRatioWholeStructureMember };
        //        GroupOfMembersWithSelectedType = gr;
        //        textGoverningMember.Text = BaseHelper.GetGoverningMemberText(_pfdVM.sDesignResults_ULS.MaximumDesignRatioWholeStructureMember);
        //    }
        //    else
        //    {
        //        GroupOfMembersWithSelectedType = _pfdVM.Model.listOfModelMemberGroups.FirstOrDefault(c => c.Name == vm.ComponentList[vm.ComponentTypeIndex]);
        //        textGoverningMember.Text = "";
        //    }

        //    // Calculate governing member design ratio in member group
        //    CCalculJoint cGoverningMemberFootingResults;

        //    CalculateGoverningMemberDesignDetails(DesignResults_ULS, vm.SelectedLoadCombinationID, GroupOfMembersWithSelectedType, out cGoverningMemberFootingResults);
        //}

        //// Calculate governing member design ratio
        //public void CalculateGoverningMemberDesignDetails(List<CJointLoadCombinationRatio_ULS> DesignResults, int loadCombinationID, CMemberGroup GroupOfMembersWithSelectedType, out CCalculJoint cGoverningMemberFootingResults)
        //{
        //    cGoverningMemberFootingResults = null;

        //    CalculationSettingsFoundation footingCalcSettings = _pfdVM.FootingVM.GetCalcSettings();

        //    if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
        //    {
        //        float fMaximumDesignRatio = float.MinValue;

        //        if (loadCombinationID == -1) //envelope
        //        {
        //            loadCombinationID = _pfdVM.sDesignResults_ULSandSLS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination.ID;
        //            textGoverningLoadComb.Text = BaseHelper.GetGoverningLoadCombText(_pfdVM.sDesignResults_ULS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination);
        //        }
        //        else { textGoverningLoadComb.Text = ""; }

        //        foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
        //        {
        //            CConnectionJointTypes cjStart = null;
        //            CConnectionJointTypes cjEnd = null;
        //            _pfdVM.Model.GetModelMemberStartEndConnectionJoints(m, out cjStart, out cjEnd);

        //            CConnectionJointTypes joint = cjStart;
        //            CFoundation f = _pfdVM.Model.GetFoundationForJointFromModel(joint);
        //            if (f == null) { f = _pfdVM.Model.GetFoundationForJointFromModel(cjEnd); joint = cjEnd; }
        //            if (f == null) continue;

        //            CJointLoadCombinationRatio_ULS res = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombinationID && i.Joint.m_Node.ID == joint.m_Node.ID);
        //            if (res == null) continue;

        //            CCalculJoint cJoint = new CCalculJoint(false, UseCRSCGeometricalAxes, _pfdVM._designOptionsVM.ShearDesignAccording334, _pfdVM._designOptionsVM.UniformShearDistributionInAnchors, joint, _pfdVM.Model, footingCalcSettings, res.DesignInternalForces);

        //            // Find member in the group of members with maximum joint design ratio
        //            if (cJoint.fEta_max_footing > fMaximumDesignRatio)
        //            {
        //                fMaximumDesignRatio = cJoint.fEta_max_footing;
        //                // Prepocitat spoj a dopocitat detaily - To Ondrej, asi to nie je velmi efektivne ale nema zmysel ukladat to pri kazdom, len pre ten ktory bude zobrazeny
        //                cJoint = new CCalculJoint(false, UseCRSCGeometricalAxes, _pfdVM._designOptionsVM.ShearDesignAccording334, _pfdVM._designOptionsVM.UniformShearDistributionInAnchors, joint, _pfdVM.Model, footingCalcSettings, res.DesignInternalForces, true);
        //                cGoverningMemberFootingResults = cJoint;
        //            }
        //        }

        //        // Display calculation results - details
        //        if (cGoverningMemberFootingResults != null)
        //            cGoverningMemberFootingResults.DisplayFootingDesignResultsInGridView(Results_FootingPad_GridView);
        //        else
        //        {
        //            // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
        //            // throw new Exception("Results of selected component are not available!");
        //            MessageBox.Show("Results of selected component are not available! [FootingDesign]");
        //        }
        //    }
        //}
    }
}
