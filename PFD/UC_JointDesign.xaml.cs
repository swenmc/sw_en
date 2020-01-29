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
    /// Interaction logic for UC_JointDesign.xaml
    /// </summary>
    public partial class UC_JointDesign : UserControl
    {
        bool UseCRSCGeometricalAxes;
        
        CPFDViewModel _pfdVM;
        CalculationSettingsFoundation FootingCalcSettings;
        public List<CJointLoadCombinationRatio_ULS> DesignResults_ULS;

        public UC_JointDesign(bool bUseCRSCGeometricalAxes_temp, CPFDViewModel pfdVM, CComponentListVM compList)
        {
            InitializeComponent();

            UseCRSCGeometricalAxes = bUseCRSCGeometricalAxes_temp;            
            _pfdVM = pfdVM;
            DesignResults_ULS = _pfdVM.JointDesignResults_ULS;

            // Joint Design
            CPFDJointsDesign vm = new CPFDJointsDesign(_pfdVM.Model.m_arrLimitStates, _pfdVM.Model.m_arrLoadCombs, compList.ComponentList);
            vm.PropertyChanged += HandleJointDesignPropertyChangedEvent;
            this.DataContext = vm;

            vm.LimitStateIndex = 0;
        }

        protected void HandleJointDesignPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDJointsDesign vm = sender as CPFDJointsDesign;
            if (vm != null && vm.IsSetFromCode) return;
            if (vm.ComponentTypeIndex == -1) return;

            CMemberGroup GroupOfMembersWithSelectedType = _pfdVM.Model.listOfModelMemberGroups.FirstOrDefault(c => c.Name == vm.ComponentList[vm.ComponentTypeIndex]);

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

            FootingCalcSettings = _pfdVM.FootingVM.GetCalcSettings();

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = float.MinValue;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    CConnectionJointTypes cjStart = null;
                    CConnectionJointTypes cjEnd = null;
                    _pfdVM.Model.GetModelMemberStartEndConnectionJoints(m, out cjStart, out cjEnd);

                    if (loadCombinationID == -1) //envelope
                    {
                        loadCombinationID = _pfdVM.sDesignResults_ULS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination.ID;
                    }

                    CJointLoadCombinationRatio_ULS resStart = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombinationID && i.Joint.m_Node.ID == cjStart.m_Node.ID);
                    CJointLoadCombinationRatio_ULS resEnd = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombinationID && i.Joint.m_Node.ID == cjEnd.m_Node.ID);
                    if (resStart == null) continue;
                    if (resEnd == null) continue;                    
                    ////-------------------------------------------------------------------------------------------------------------
                    //// TODO Ondrej - potrebujem sem dostat nastavenia vypoctu z UC_FootingInput a nahradit tieto konstanty
                    //CalculationSettingsFoundation FootingCalcSettings = new CalculationSettingsFoundation();
                    //FootingCalcSettings.ConcreteGrade = "30";
                    //FootingCalcSettings.AggregateSize = 0.02f;
                    //FootingCalcSettings.ConcreteDensity = 2300f;
                    //FootingCalcSettings.ReinforcementGrade = "500E";
                    //FootingCalcSettings.SoilReductionFactor_Phi = 0.5f;
                    //FootingCalcSettings.SoilReductionFactorEQ_Phi = 0.8f;
                    //FootingCalcSettings.SoilBearingCapacity = 100e+3f;
                    //FootingCalcSettings.FloorSlabThickness = 0.125f;
                    ////-------------------------------------------------------------------------------------------------------------

                    CCalculJoint cJointStart = new CCalculJoint(false, UseCRSCGeometricalAxes, cjStart, _pfdVM.Model, FootingCalcSettings, resStart.DesignInternalForces);
                    CCalculJoint cJointEnd = new CCalculJoint(false, UseCRSCGeometricalAxes, cjEnd, _pfdVM.Model, FootingCalcSettings, resEnd.DesignInternalForces);

                    // Find member in the group of members with maximum start or end joint design ratio
                    if (cJointStart.fEta_max > fMaximumDesignRatio || cJointEnd.fEta_max > fMaximumDesignRatio)
                    {
                        // Set new maximum design ratio
                        if (cJointStart.fEta_max > fMaximumDesignRatio)
                            fMaximumDesignRatio = cJointStart.fEta_max;
                        else
                            fMaximumDesignRatio = cJointEnd.fEta_max;

                        // Set start joint and end joint design details
                        // One joint is joint with maximum design ratio, the other joint is corresponding joint for selected member and load combination

                        // Prepocitat spoj a dopocitat detaily - To Ondrej, asi to nie je velmi efektivne ale nema zmysel ukladat to pri kazdom, len pre ten ktory bude zobrazeny
                        //To Mato - toto mi musis vsvetlit preco sa to tu prepocitava znovu akurat jeden bool na konci je zmeneny
                        cJointStart = new CCalculJoint(false, UseCRSCGeometricalAxes, cjStart, _pfdVM.Model, FootingCalcSettings, resStart.DesignInternalForces, true);
                        cGoverningMemberStartJointResults = cJointStart;

                        // Prepocitat spoj a dopocitat detaily - To Ondrej, asi to nie je velmi efektivne ale nema zmysel ukladat to pri kazdom, len pre ten ktory bude zobrazeny
                        cJointEnd = new CCalculJoint(false, UseCRSCGeometricalAxes, cjEnd, _pfdVM.Model, FootingCalcSettings, resEnd.DesignInternalForces, true);
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
                    MessageBox.Show("Results of selected component are not available! [JointDesign]");
                }

                // End Joint
                if (cGoverningMemberEndJointResults != null)
                    cGoverningMemberEndJointResults.DisplayDesignResultsInGridView(Results_JointAtMemberEnd_GridView);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available! [JointDesign]");
                }
            }
        }
    }
}
