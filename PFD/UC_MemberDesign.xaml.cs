using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BaseClasses;
using M_AS4600;
using CRSC;
using FEM_CALC_BASE;
using M_BASE;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_MemberDesign.xaml
    /// </summary>
    public partial class UC_MemberDesign : UserControl
    {
        CModel_PFD Model;
        public List<CMemberLoadCombinationRatio_ULS> DesignResults_ULS;
        public List<CMemberLoadCombinationRatio_SLS> DesignResults_SLS;
        public bool UseCRSCGeometricalAxes;

        public UC_MemberDesign(bool bUseCRSCGeometricalAxes, CModel_PFD model, CComponentListVM compList, List<CMemberLoadCombinationRatio_ULS> designResults_ULS, List<CMemberLoadCombinationRatio_SLS> designResults_SLS)
        {
            InitializeComponent();

            UseCRSCGeometricalAxes = bUseCRSCGeometricalAxes;
            Model = model;
            DesignResults_ULS = designResults_ULS;
            DesignResults_SLS = designResults_SLS;

            // Member Design
            CPFDMemberDesign vm = new CPFDMemberDesign(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            vm.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = vm;

            vm.LimitStateIndex = 0;
        }
        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDMemberDesign vm = sender as CPFDMemberDesign;
            if (vm != null && vm.IsSetFromCode) return;
            
            CMemberGroup GroupOfMembersWithSelectedType = Model.listOfModelMemberGroups[vm.ComponentTypeIndex];

            // Calculate governing member design ratio in member group
            CCalculMember cGoverningMemberResults;
            
            if (vm.LimitStates[vm.LimitStateIndex].eLS_Type == ELSType.eLS_ULS)
                CalculateGoverningMemberDesignDetails(UseCRSCGeometricalAxes, DesignResults_ULS, vm.SelectedLoadCombinationID, GroupOfMembersWithSelectedType, out cGoverningMemberResults);
            else
                CalculateGoverningMemberDesignDetails(UseCRSCGeometricalAxes, DesignResults_SLS, vm.SelectedLoadCombinationID, GroupOfMembersWithSelectedType, out cGoverningMemberResults);
        }

        // Calculate governing member design ratio
        public void CalculateGoverningMemberDesignDetails(bool bUseCRSCGeometricalAxes, List<CMemberLoadCombinationRatio_ULS> DesignResults, int loadCombID, CMemberGroup GroupOfMembersWithSelectedType, out CCalculMember cGoverningMemberResults)
        {
            cGoverningMemberResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = 0;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    // Select member with identical ID from the list of results
                    CMemberLoadCombinationRatio_ULS res = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombID);
                    if (res == null) continue;
                    CCalculMember c = new CCalculMember(false, bUseCRSCGeometricalAxes, res.DesignInternalForces, m, res.DesignBucklingLengthFactors, res.DesignMomentValuesForCb);

                    if (c.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = c.fEta_max;
                        cGoverningMemberResults = c;
                    }
                }

                if (cGoverningMemberResults != null)
                    cGoverningMemberResults.DisplayDesignResultsInGridView(ELSType.eLS_ULS, Results_GridView);
                    //DisplayDesignResultsInGridView(ELSType.eLS_ULS, Results_GridView, cGoverningMemberResults);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }
            }
            else
            {
                MessageBox.Show("Results not available! Click Calculate!");
                //CalculateForMemberLoadCase(GroupOfMembersWithSelectedType);
            }
        }

        public void CalculateGoverningMemberDesignDetails(bool bUseCRSCGeometricalAxes, List<CMemberLoadCombinationRatio_SLS> DesignResults, int loadCombID, CMemberGroup GroupOfMembersWithSelectedType, out CCalculMember cGoverningMemberResults)
        {
            cGoverningMemberResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = 0;

                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    CMemberLoadCombinationRatio_SLS res = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombID);
                    if (res == null) continue;
                    CCalculMember c = new CCalculMember(false, bUseCRSCGeometricalAxes, res.DesignDeflections, m);

                    if (c.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = c.fEta_max;
                        cGoverningMemberResults = c;
                    }
                }

                if (cGoverningMemberResults != null)
                    cGoverningMemberResults.DisplayDesignResultsInGridView(ELSType.eLS_SLS, Results_GridView);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }
            }
        }

        
    }
}
