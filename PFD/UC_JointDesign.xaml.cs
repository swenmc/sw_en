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

        List<CJointLoadCombinationRatio_ULS> DesignResults_ULS;

        public UC_JointDesign(CModel_PFD model, CComponentListVM compList, List<CJointLoadCombinationRatio_ULS> designResults_ULS)
        {
            InitializeComponent();

            DesignResults_ULS = designResults_ULS;
            Model = model;

            // Joint Design
            CPFDJointsDesign jdinput = new CPFDJointsDesign(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            jdinput.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = jdinput;
        }

        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDJointsDesign jdinput = sender as CPFDJointsDesign;
            if (jdinput != null && jdinput.IsSetFromCode) return;

            CMemberGroup GroupOfMembersWithSelectedType = Model.listOfModelMemberGroups[jdinput.ComponentTypeIndex];

            // Calculate governing member design ratio in member group
            CCalculJoint cGoverningMemberStartJointResults;
            CCalculJoint cGoverningMemberEndJointResults;

            CalculateGoverningMemberDesignDetails(DesignResults_ULS, GroupOfMembersWithSelectedType, out cGoverningMemberStartJointResults, out cGoverningMemberEndJointResults);

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
        public void CalculateGoverningMemberDesignDetails(List<CJointLoadCombinationRatio_ULS> DesignResults, CMemberGroup GroupOfMembersWithSelectedType, out CCalculJoint cGoverningMemberStartJointResults, out CCalculJoint cGoverningMemberEndJointResults)
        {
            cGoverningMemberStartJointResults = null;
            cGoverningMemberEndJointResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = 0;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    CJointLoadCombinationRatio_ULS res = DesignResults.Find(i => i.Member.ID == m.ID);
                    if (res == null) continue;
                    
                    CConnectionJointTypes jStart_temp = null;
                    CConnectionJointTypes jEnd_temp = null;
                    foreach (CConnectionJointTypes cj in Model.m_arrConnectionJoints)
                    {
                        CMember mm = cj.m_SecondaryMembers.First(mem => mem.ID == m.ID);
                        if (mm != null)
                        {
                            if (mm.NodeStart == cj.m_Node) jStart_temp = cj;
                            if (mm.NodeEnd == cj.m_Node) jEnd_temp = cj;
                        }
                    }

                    if(jStart_temp == null || jEnd_temp == null) Model.m_arrConnectionJoints.Select(j => j.m_MainMember != null && j.m_MainMember.ID == m.ID);

                    foreach (CConnectionJointTypes cj in Model.m_arrConnectionJoints)
                    {
                        if (cj.m_MainMember != null && cj.m_MainMember.ID == m.ID)
                        {
                            if (cj.m_MainMember.NodeStart == cj.m_Node) jStart_temp = cj;
                            if (cj.m_MainMember.NodeEnd == cj.m_Node) jEnd_temp = cj;
                        }
                    }

                    if (jStart_temp == null || jEnd_temp == null) throw new Exception("Start or end connection joint not found.");
                    
                    
                    CCalculJoint cJointStart = new CCalculJoint(false, jStart_temp, res.DesignInternalForces);
                    CCalculJoint cJointEnd = new CCalculJoint(false, jStart_temp, res.DesignInternalForces);

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
                    DisplayDesignResultsInGridView(Results_JointAtMemberStart_GridView, cGoverningMemberStartJointResults);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }

                // End Joint
                if (cGoverningMemberEndJointResults != null)
                    DisplayDesignResultsInGridView(Results_JointAtMemberEnd_GridView, cGoverningMemberEndJointResults);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }
            }
        }

        public void DisplayDesignResultsInGridView(DataGrid dataGrid, CCalculJoint obj_CalcDesign)
        {
            // TODO  pripravit zoznam parametrov pre datagrid
            // To Ondrej - Ako by to malo vyzerat, pouzit Tuple, zapracovat do viewmodel
            // TODO - Ondrej
            // Prepracovat a refaktorovat DisplayDesignResultsInGridView v UC_MemberDesign.xaml.cs
        }
    }
}
