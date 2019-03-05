using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
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

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_JointDesign.xaml
    /// </summary>
    public partial class UC_JointDesign : UserControl
    {
        CModel_PFD Model;

        public List<CJointLoadCombinationRatio_ULS> DesignResults_ULS;

        public UC_JointDesign(CModel_PFD model, CComponentListVM compList, List<CJointLoadCombinationRatio_ULS> designResults_ULS)
        {
            InitializeComponent();

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

            CMemberGroup GroupOfMembersWithSelectedType = Model.listOfModelMemberGroups[vm.ComponentTypeIndex];

            // Calculate governing member design ratio in member group
            CCalculJoint cGoverningMemberStartJointResults;
            CCalculJoint cGoverningMemberEndJointResults;

            CalculateGoverningMemberDesignDetails(DesignResults_ULS, vm.SelectedLoadCombinationID,  GroupOfMembersWithSelectedType, out cGoverningMemberStartJointResults, out cGoverningMemberEndJointResults);

            // K diskusii
            // Moznosti su zobrazovat vysledky podla typu spoja alebo po prutoch pre spoje na konci a na zaciatku
            //Model.m_arrMembers.Select(m => m.Name.Contains("girt"))


            // Prva moznost - asi zacneme tymto
            // Pre vybrany typ spoja prejst vsetky vsetky spoje daneho typu a vybrat najhorsi
            //Model.m_arrConnectionJoints.Select(j => j.mSe)

            // Druha moznost
            // Pre vybrany prut potrebujeme mat pristupne designInternalForces na jeho zaciatku a na konci

            // pre sadu na zaciatku a posudi spoj, ktory je na zaciatku pruta

            // pre sadu na konci sa posudi spoj, ktory je na konci pruta

            // TODO Ondrej - najst pre dany typ spoja vsetky uzly na ktorych je definovany


            // TODO Ondrej - najst pre zaciatocny CNode a koncovy CNode na prute priradeny Joint (CConectionJointTypes)
            // Asi by sa to malo priradit uz priamo v CModel_PFD, resp by tam mala byt tato funckia dostupna
        }

        // Calculate governing member design ratio
        public void CalculateGoverningMemberDesignDetails(List<CJointLoadCombinationRatio_ULS> DesignResults, int loadCombinationID, CMemberGroup GroupOfMembersWithSelectedType, out CCalculJoint cGoverningMemberStartJointResults, out CCalculJoint cGoverningMemberEndJointResults)
        {
            cGoverningMemberStartJointResults = null;
            cGoverningMemberEndJointResults = null;
            
            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = 0;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    CConnectionJointTypes cjStart = null;
                    CConnectionJointTypes cjEnd = null;
                    Model.GetModelMemberStartEndConnectionJoints(m, out cjStart, out cjEnd);

                    CJointLoadCombinationRatio_ULS resStart = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombinationID && i.Joint == cjStart);
                    CJointLoadCombinationRatio_ULS resEnd = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombinationID && i.Joint == cjEnd);
                    if (resStart == null) continue;
                    if (resEnd == null) continue;
                    
                    CCalculJoint cJointStart = new CCalculJoint(false, cjStart, resStart.DesignInternalForces);
                    CCalculJoint cJointEnd = new CCalculJoint(false, cjEnd, resEnd.DesignInternalForces);

                    if (cJointStart.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = cJointStart.fEta_max;
                        cGoverningMemberStartJointResults = cJointStart;
                    }

                    if (cJointEnd.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = cJointEnd.fEta_max;
                        cGoverningMemberEndJointResults = cJointEnd;
                    }
                }

                // TODO - Ondrej
                // Rozhodujuce vyuzitie pre spoj na zaciatku pruta moze byt napr. z pruta c 5, ale rozhodujuce vyuzitie pre spoj na konci pruta moze byt z pruta 8
                // Potrebovali by sme z tychto dvoch vyuziti urcit maximum a pre dany prut s tymto maximom zobrazit vyuzite pre start a end joint (hoci jedno z nich nie je maximalne v skupine)

                // Display calculation results - details
                // Start Joint
                if (cGoverningMemberStartJointResults != null)
                    cGoverningMemberStartJointResults.DisplayDesignResultsInGridView(Results_JointAtMemberStart_GridView);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }

                // End Joint
                if (cGoverningMemberEndJointResults != null)
                    cGoverningMemberEndJointResults.DisplayDesignResultsInGridView(Results_JointAtMemberEnd_GridView);
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
