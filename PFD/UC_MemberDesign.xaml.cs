using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BaseClasses;
using M_AS4600;
using System.Text;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_MemberDesign.xaml
    /// </summary>
    public partial class UC_MemberDesign : UserControl
    {
        public bool UseCRSCGeometricalAxes;
        //public bool ShearDesignAccording334;
        //public bool IgnoreWebStiffeners;
        CModel_PFD Model;
        public List<CMemberLoadCombinationRatio_ULS> DesignResults_ULS;
        public List<CMemberLoadCombinationRatio_SLS> DesignResults_SLS;
        
        public sDesignResults sDesignResults_ULSandSLS = new sDesignResults();
        public sDesignResults sDesignResults_SLS = new sDesignResults();
        public sDesignResults sDesignResults_ULS = new sDesignResults();

        public DesignOptionsViewModel designOptionsVM;

        public UC_MemberDesign(bool bUseCRSCGeometricalAxes, DesignOptionsViewModel doVM, CModel_PFD model, CComponentListVM compList, List<CMemberLoadCombinationRatio_ULS> listDesignResults_ULS, List<CMemberLoadCombinationRatio_SLS> listDesignResults_SLS, sDesignResults designResults_ULS_SLS, sDesignResults designResults_ULS, sDesignResults designResults_SLS)
        {
            InitializeComponent();

            designOptionsVM = doVM;

            UseCRSCGeometricalAxes = bUseCRSCGeometricalAxes;
            
            Model = model;
            DesignResults_ULS = listDesignResults_ULS;
            DesignResults_SLS = listDesignResults_SLS;
            sDesignResults_ULSandSLS = designResults_ULS_SLS;
            sDesignResults_ULS = designResults_ULS;
            sDesignResults_SLS = designResults_SLS;

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
            if (vm.ComponentTypeIndex == -1) return;

            CMemberGroup GroupOfMembersWithSelectedType = null;

            if (vm.ComponentList[vm.ComponentTypeIndex] == "All") //All
            {
                if (vm.LimitStates[vm.LimitStateIndex].eLS_Type == ELSType.eLS_ULS)
                {
                    CMemberGroup gr = Model.listOfModelMemberGroups.FirstOrDefault(c => c.MemberType_FS_Position == sDesignResults_ULS.MaximumDesignRatioWholeStructureMember.EMemberTypePosition);
                    if (gr != null) gr = gr.Clone();
                    gr.ListOfMembers = new List<CMember> { sDesignResults_ULS.MaximumDesignRatioWholeStructureMember };
                    GroupOfMembersWithSelectedType = gr;
                    textGoverningMember.Text = BaseHelper.GetGoverningMemberText(sDesignResults_ULS.MaximumDesignRatioWholeStructureMember);
                }
                else if(vm.LimitStates[vm.LimitStateIndex].eLS_Type == ELSType.eLS_SLS)
                {
                    CMemberGroup gr = Model.listOfModelMemberGroups.FirstOrDefault(c => c.MemberType_FS_Position == sDesignResults_SLS.MaximumDesignRatioWholeStructureMember.EMemberTypePosition);
                    if (gr != null) gr = gr.Clone();
                    gr.ListOfMembers = new List<CMember> { sDesignResults_SLS.MaximumDesignRatioWholeStructureMember };
                    GroupOfMembersWithSelectedType = gr;
                    textGoverningMember.Text = BaseHelper.GetGoverningMemberText(sDesignResults_SLS.MaximumDesignRatioWholeStructureMember);
                }
                else if (vm.LimitStates[vm.LimitStateIndex].eLS_Type == ELSType.eLS_ALL)
                {
                    CMemberGroup gr = Model.listOfModelMemberGroups.FirstOrDefault(c => c.MemberType_FS_Position == sDesignResults_ULSandSLS.MaximumDesignRatioWholeStructureMember.EMemberTypePosition);
                    if (gr != null) gr = gr.Clone();
                    gr.ListOfMembers = new List<CMember> { sDesignResults_ULSandSLS.MaximumDesignRatioWholeStructureMember };
                    GroupOfMembersWithSelectedType = gr;
                    textGoverningMember.Text = BaseHelper.GetGoverningMemberText(sDesignResults_ULSandSLS.MaximumDesignRatioWholeStructureMember);                    
                }
            }
            else
            {
                GroupOfMembersWithSelectedType = Model.listOfModelMemberGroups.FirstOrDefault(c => c.Name == vm.ComponentList[vm.ComponentTypeIndex]);                
                textGoverningMember.Text = "";                
            } 
                

            // Prepiseme defaultne hodnoty limitov hodnotami z GUI - design options
            // TODO - ak budu v design options vsetky potrebne limity, mozu sa defaulty odstranit

            // Nastavime menovatele zlomkov
            if (GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eEC ||
                GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eMC)
            {
                GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_Total = designOptionsVM.HorizontalDisplacementLimitDenominator_Column_TL;
            }
            else if (GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eER ||
                     GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eMR)
            {
                GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_PermanentLoad = designOptionsVM.VerticalDisplacementLimitDenominator_Rafter_PL;
                GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_Total = designOptionsVM.VerticalDisplacementLimitDenominator_Rafter_TL;
            }
            else if(GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eC) // Wind post - TODO bude potrebne prerobit enum eC vsade na eWP
            {
                GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_Total = designOptionsVM.HorizontalDisplacementLimitDenominator_Windpost_TL;
            }
            else if(GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eP ||
                GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eEP) // TODO Nastavovat aj horizontalny smer
            {
                GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_PermanentLoad = designOptionsVM.VerticalDisplacementLimitDenominator_Purlin_PL;
                GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_Total = designOptionsVM.VerticalDisplacementLimitDenominator_Purlin_TL;
            }
            else if(GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eG ||
                   GroupOfMembersWithSelectedType.MemberType_FS == EMemberType_FS.eBG)
            {
                GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_Total = designOptionsVM.HorizontalDisplacementLimitDenominator_Girt_TL;
            }
            else
            {
                //a co ak je ALL???
            }

            // Prepocitame limitne hodnoty
            GroupOfMembersWithSelectedType.RecalculateDeflectionLimits();

            // Calculate governing member design ratio in member group
            CCalculMember cGoverningMemberResults;

            int loadCombID = vm.SelectedLoadCombinationID;

            // TODO 511 To Ondrej - podla mna by to malo byt nejako takto, asi to da napisat aj krajsie, tak to prosim uprav
            // Teoreticky sa to da urobit aj tak, ze sa do CalculateGoverningMemberDesignDetails prida dalsi cyklus cez vsetky load combinations a potom cez vsetky members v skupine a ked je loadcombID = -1 tak sa prejde cely ten cyklus cez vsetky load combinations
            // Nejako obdobne to treba implementovat aj pre joint design a footing pad design
            // Tam by mali rozhodovat design ratia pre zaciatocny alebo koncovy uzol na prute, vyberie sa najhorsi a k nemu sa zobrazuje myslim odpovedajuci z toho isteho pruta

            textGoverningLimitState.Text = "";
            if (vm.LimitStates[vm.LimitStateIndex].eLS_Type == ELSType.eLS_ULS)
            {
                if (vm.SelectedLoadCombinationID == -1) //"envelope"
                {
                    // ULS
                    loadCombID = sDesignResults_ULS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination.ID;
                    textGoverningLoadComb.Text = BaseHelper.GetGoverningLoadCombText(sDesignResults_ULS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination);

                    //// Vysledky vsetkych prutov daneho typu zo vsetkych kombinacii daneho limit state
                    //List<CMemberLoadCombinationRatio_ULS> allSpecificMemberTypeResults = DesignResults_ULS.FindAll(i => i.Member.EMemberTypePosition == GroupOfMembersWithSelectedType.MemberType_FS_Position);
                    //// Najdi maximalne vyuzitie z vysledkov
                    //float fMaxDesignRatio = allSpecificMemberTypeResults.Max(f => f.MaximumDesignRatio);
                    //// Nacitaj vysledkovy zaznam pre maximalne vyuzitie
                    //var governingResults = allSpecificMemberTypeResults.First(x=> MATH.MathF.d_equal(x.MaximumDesignRatio, fMaxDesignRatio));
                    //// Nastav load combination pre maximalne vyuzitie (vsetky load combinations vo vybranom limit state)
                    //loadCombID = governingResults.LoadCombination.ID;
                }
                else { textGoverningLoadComb.Text = ""; }

                CalculateGoverningMemberDesignDetails(UseCRSCGeometricalAxes, designOptionsVM.ShearDesignAccording334, designOptionsVM.IgnoreWebStiffeners, DesignResults_ULS, loadCombID, GroupOfMembersWithSelectedType, out cGoverningMemberResults);
            }
            else if (vm.LimitStates[vm.LimitStateIndex].eLS_Type == ELSType.eLS_SLS)
            {
                if (vm.SelectedLoadCombinationID == -1) //"envelope"
                {
                    // SLS
                    loadCombID = sDesignResults_SLS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination.ID;
                    textGoverningLoadComb.Text = BaseHelper.GetGoverningLoadCombText(sDesignResults_SLS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination);                    
                }
                else { textGoverningLoadComb.Text = ""; }

                CalculateGoverningMemberDesignDetails(UseCRSCGeometricalAxes, DesignResults_SLS, loadCombID, GroupOfMembersWithSelectedType, out cGoverningMemberResults);
            }
            else if (vm.LimitStates[vm.LimitStateIndex].eLS_Type == ELSType.eLS_ALL)
            {
                if (vm.SelectedLoadCombinationID == -1) //"envelope"
                {
                    // ALL
                    loadCombID = sDesignResults_ULSandSLS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination.ID;
                    textGoverningLoadComb.Text = BaseHelper.GetGoverningLoadCombText(sDesignResults_ULSandSLS.DesignResults[GroupOfMembersWithSelectedType.MemberType_FS_Position].GoverningLoadCombination);                    
                }
                else { textGoverningLoadComb.Text = ""; }

                CalculateGoverningMemberDesignDetails(UseCRSCGeometricalAxes, designOptionsVM.ShearDesignAccording334, designOptionsVM.IgnoreWebStiffeners, DesignResults_ULS, DesignResults_SLS, loadCombID, GroupOfMembersWithSelectedType, out cGoverningMemberResults);
            }
        }
        
        // Calculate governing member design ratio
        public void CalculateGoverningMemberDesignDetails(bool bUseCRSCGeometricalAxes, bool bShearDesignAccording334, bool bIgnoreWebStiffeners, List<CMemberLoadCombinationRatio_ULS> DesignResults, int loadCombID, CMemberGroup GroupOfMembersWithSelectedType, out CCalculMember cGoverningMemberResults)
        {
            cGoverningMemberResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = float.MinValue;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    // Select member with identical ID from the list of results
                    CMemberLoadCombinationRatio_ULS res = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombID);
                    if (res == null) continue;
                    CCalculMember c = new CCalculMember(false, bUseCRSCGeometricalAxes, bShearDesignAccording334, bIgnoreWebStiffeners, res.DesignInternalForces, m, res.DesignBucklingLengthFactors, res.DesignMomentValuesForCb);

                    if (c.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = c.fEta_max;
                        cGoverningMemberResults = c;
                    }
                }

                if (cGoverningMemberResults != null)
                    cGoverningMemberResults.DisplayDesignResultsInGridView(ELSType.eLS_ULS, Results_GridView);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available! [MemberDesign]");
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

                    // Limit zavisi od typu zatazenia (load combination) a typu pruta
                    float fDeflectionLimitFraction_Denominator = GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_Total;
                    float fDeflectionLimit = GroupOfMembersWithSelectedType.DeflectionLimit_Total;

                    // TODO Ondrej - identifikacia ci kombinacia obsahuje len load cases typu permanent
                    // Da sa pouzit metoda v triede CLoadCombination IsCombinationOfPermanentLoadCasesOnly()

                    if (loadCombID == 41) // TODO Combination of permanent load (TODO - nacitat spravne typ kombinacie, neurcovat podla cisla ID)
                    {
                        fDeflectionLimitFraction_Denominator = GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_PermanentLoad;
                        fDeflectionLimit = GroupOfMembersWithSelectedType.DeflectionLimit_PermanentLoad;
                    }

                    CCalculMember calcul = new CCalculMember(false, bUseCRSCGeometricalAxes, res.DesignDeflections, m, fDeflectionLimitFraction_Denominator, fDeflectionLimit);

                    if (calcul.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = calcul.fEta_max;
                        cGoverningMemberResults = calcul;
                    }
                }

                if (cGoverningMemberResults != null)
                    cGoverningMemberResults.DisplayDesignResultsInGridView(ELSType.eLS_SLS, Results_GridView);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available! [MemberDesign]");
                }
            }
        }

        // Calculate governing member design ratio
        public void CalculateGoverningMemberDesignDetails(bool bUseCRSCGeometricalAxes, bool bShearDesignAccording334, bool bIgnoreWebStiffeners, List<CMemberLoadCombinationRatio_ULS> DesignResults_ULS, List<CMemberLoadCombinationRatio_SLS> DesignResults_SLS, int loadCombID, CMemberGroup GroupOfMembersWithSelectedType, out CCalculMember cGoverningMemberResults)
        {
            cGoverningMemberResults = null;
            bool isULS = true;

            if (DesignResults_ULS != null && DesignResults_SLS != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = float.MinValue;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    //ULS
                    // Select member with identical ID from the list of results
                    CMemberLoadCombinationRatio_ULS res_ULS = DesignResults_ULS.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombID);
                    if (res_ULS != null)
                    {
                        CCalculMember c = new CCalculMember(false, bUseCRSCGeometricalAxes, bShearDesignAccording334, bIgnoreWebStiffeners, res_ULS.DesignInternalForces, m, res_ULS.DesignBucklingLengthFactors, res_ULS.DesignMomentValuesForCb);

                        if (c.fEta_max > fMaximumDesignRatio)
                        {
                            fMaximumDesignRatio = c.fEta_max;
                            cGoverningMemberResults = c;
                            isULS = true;
                        }
                    }
                                        
                    //SLS
                    CMemberLoadCombinationRatio_SLS res_SLS = DesignResults_SLS.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombID);
                    if (res_SLS != null)
                    {
                        // Limit zavisi od typu zatazenia (load combination) a typu pruta
                        float fDeflectionLimitFraction_Denominator = GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_Total;
                        float fDeflectionLimit = GroupOfMembersWithSelectedType.DeflectionLimit_Total;

                        // TODO Ondrej - identifikacia ci kombinacia obsahuje len load cases typu permanent
                        // Da sa pouzit metoda v triede CLoadCombination IsCombinationOfPermanentLoadCasesOnly()

                        if (loadCombID == 41) // TODO Combination of permanent load (TODO - nacitat spravne typ kombinacie, neurcovat podla cisla ID)
                        {
                            fDeflectionLimitFraction_Denominator = GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_PermanentLoad;
                            fDeflectionLimit = GroupOfMembersWithSelectedType.DeflectionLimit_PermanentLoad;
                        }

                        CCalculMember calcul = new CCalculMember(false, bUseCRSCGeometricalAxes, res_SLS.DesignDeflections, m, fDeflectionLimitFraction_Denominator, fDeflectionLimit);

                        if (calcul.fEta_max > fMaximumDesignRatio)
                        {
                            fMaximumDesignRatio = calcul.fEta_max;
                            cGoverningMemberResults = calcul;
                            isULS = false;
                        }
                    }
                }

                if (cGoverningMemberResults != null)
                {
                    if (isULS)
                    {
                        cGoverningMemberResults.DisplayDesignResultsInGridView(ELSType.eLS_ULS, Results_GridView);
                        textGoverningLimitState.Text = BaseHelper.GetGoverningLimitStateText(ELSType.eLS_ULS);
                    }
                    else
                    {
                        cGoverningMemberResults.DisplayDesignResultsInGridView(ELSType.eLS_SLS, Results_GridView);
                        textGoverningLimitState.Text = BaseHelper.GetGoverningLimitStateText(ELSType.eLS_SLS);
                    }
                }                    
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available! [MemberDesign]");
                }
            }
            else
            {
                MessageBox.Show("Results not available! Click Calculate!");
                //CalculateForMemberLoadCase(GroupOfMembersWithSelectedType);
            }
        }

    }
}
