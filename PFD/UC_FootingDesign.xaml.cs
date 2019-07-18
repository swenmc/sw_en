using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BaseClasses;
using M_AS4600;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_FootingDesign.xaml
    /// </summary>
    public partial class UC_FootingDesign : UserControl
    {
        bool UseCRSCGeometricalAxes;
        CModel_PFD Model;
        public List<CJointLoadCombinationRatio_ULS> DesignResults_ULS;

        public UC_FootingDesign(bool bUseCRSCGeometricalAxes_temp, CModel_PFD model, CComponentListVM compList, List<CJointLoadCombinationRatio_ULS> designResults_ULS)
        {
            InitializeComponent();

            UseCRSCGeometricalAxes = bUseCRSCGeometricalAxes_temp;
            DesignResults_ULS = designResults_ULS;
            Model = model;

            // Joint Design
            CPFDJointsDesign vm = new CPFDJointsDesign(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            vm.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = vm;

            vm.LimitStateIndex = 0;
        }

        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDJointsDesign vm = sender as CPFDJointsDesign;
            if (vm != null && vm.IsSetFromCode) return;
            if (vm.ComponentTypeIndex == -1) return;

            CMemberGroup GroupOfMembersWithSelectedType = Model.listOfModelMemberGroups.FirstOrDefault(c => c.Name == vm.ComponentList[vm.ComponentTypeIndex]);

            // Calculate governing member design ratio in member group
            CCalculJoint cGoverningMemberStartJointResults;

            // docasne zakomentovane CalculateGoverningMemberDesignDetails(DesignResults_ULS, vm.SelectedLoadCombinationID,  GroupOfMembersWithSelectedType, out cGoverningMemberStartJointResults);
        }

        // Calculate governing member design ratio
        public void CalculateGoverningMemberDesignDetails(List<CJointLoadCombinationRatio_ULS> DesignResults, int loadCombinationID, CMemberGroup GroupOfMembersWithSelectedType, out CCalculJoint cGoverningMemberStartJointResults, out CCalculJoint cGoverningMemberEndJointResults)
        {
            cGoverningMemberStartJointResults = null;
            cGoverningMemberEndJointResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = float.MinValue;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    CConnectionJointTypes cjStart = null;
                    //docasne zakomentovane Model.GetModelMemberStartEndConnectionJoints(m, out cjStart);

                    CJointLoadCombinationRatio_ULS resStart = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombinationID && i.Joint.m_Node.ID == cjStart.m_Node.ID);
                    if (resStart == null) continue;

                    CCalculJoint cJointStart = new CCalculJoint(false, UseCRSCGeometricalAxes, cjStart, resStart.DesignInternalForces);

                    // Find member in the group of members with maximum start or end joint design ratio
                    if (cJointStart.fEta_max > fMaximumDesignRatio)
                    {
                        // Prepocitat spoj a dopocitat detaily - To Ondrej, asi to nie je velmi efektivne ale nema zmysel ukladat to pri kazdom, len pre ten ktory bude zobrazeny
                        cJointStart = new CCalculJoint(false, UseCRSCGeometricalAxes, cjStart, resStart.DesignInternalForces, true);
                        cGoverningMemberStartJointResults = cJointStart;
                    }
                }

                // Display calculation results - details
                // Start Joint
                if (cGoverningMemberStartJointResults != null)
                    cGoverningMemberStartJointResults.DisplayDesignResultsInGridView(Results_FootingPad_GridView);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available! [FootingDesign]");
                }
            }
        }
    }
}
