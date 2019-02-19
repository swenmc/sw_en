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
        List<CMemberLoadCombinationRatio_ULS> DesignResults_ULS;
        List<CMemberLoadCombinationRatio_SLS> DesignResults_SLS;

        public UC_MemberDesign() { } // TODO - Refaktorovat, tento konstruktor je pouzity v projekte SBD

        public UC_MemberDesign(CModel_PFD model, CComponentListVM compList, List<CMemberLoadCombinationRatio_ULS> designResults_ULS, List<CMemberLoadCombinationRatio_SLS> designResults_SLS)
        {
            InitializeComponent();

            Model = model;
            DesignResults_ULS = designResults_ULS;
            DesignResults_SLS = designResults_SLS;

            // TODO - Ondrej zobrazit vysledky pre dany vyber v comboboxe, UC_MemberDesign a 
            // tabItem Member Design sa zobrazuje len ak su vysledky k dispozicii
            // teraz je tam nejaka chyba ze pri prepinani comboboxov sa vzdy pridavaju nove stlpce s vysledkami

            // Member Design
            CPFDMemberDesign mdinput = new CPFDMemberDesign(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            mdinput.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = mdinput;
        }
        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDMemberDesign mdinput = sender as CPFDMemberDesign;
            if (mdinput != null && mdinput.IsSetFromCode) return;

            CMemberGroup GroupOfMembersWithSelectedType = Model.listOfModelMemberGroups[mdinput.ComponentTypeIndex];

            // Calculate governing member design ratio in member group
            CCalculMember cGoverningMemberResults;

            if (Model.m_arrLoadCombs[mdinput.LoadCombinationIndex].eLComType == ELSType.eLS_ULS)
                CalculateGoverningMemberDesignDetails(DesignResults_ULS, GroupOfMembersWithSelectedType, out cGoverningMemberResults);
            else
                CalculateGoverningMemberDesignDetails(DesignResults_SLS, GroupOfMembersWithSelectedType, out cGoverningMemberResults);
        }
        
        // Calculate governing member design ratio
        public void CalculateGoverningMemberDesignDetails(List<CMemberLoadCombinationRatio_ULS> DesignResults, CMemberGroup GroupOfMembersWithSelectedType, out CCalculMember cGoverningMemberResults)
        {
            cGoverningMemberResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = 0;
                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    // Select member with identical ID from the list of results
                    CMemberLoadCombinationRatio_ULS res = DesignResults.Find(i => i.Member.ID == m.ID);
                    if (res == null) continue;
                    CCalculMember c = new CCalculMember(false, res.DesignInternalForces, m, res.DesignBucklingLengthFactors, res.DesignMomentValuesForCb);

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
                CalculateForMemberLoadCase(GroupOfMembersWithSelectedType);
            }
        }

        private void CalculateForMemberLoadCase(CMemberGroup GroupOfMembersWithSelectedType)
        {
            // TODO Ondrej - tento kod (108-331) je asi zhodny alebo velmi podobny s CPFDViewModel.cs
            // Novsi je CPFDViewModel.cs
            // Asi by sa to malo zrefaktorovat

            const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
            const int iNumberOfSegments = iNumberOfDesignSections - 1;

            float[] fx_positions = new float[iNumberOfDesignSections];
            designBucklingLengthFactors sBucklingLengthFactors = new designBucklingLengthFactors();
            designMomentValuesForCb sMomentValuesforCb = new designMomentValuesForCb();
            basicInternalForces[] sBIF_x = null;
            basicDeflections[] sBDeflections_x = null;

            // Tu by sa mal napojit FEM vypocet
            //RunFEMSOlver();
            float fMaximumDesignRatioWholeStructure = 0;
            float fMaximumDesignRatioGirts = 0;
            float fMaximumDesignRatioPurlins = 0;
            float fMaximumDesignRatioColumns = 0;

            CMember MaximumDesignRatioWholeStructureMember = new CMember();
            CMember MaximumDesignRatioGirt = new CMember();
            CMember MaximumDesignRatioPurlin = new CMember();
            CMember MaximumDesignRatioColumn = new CMember();

            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();
            List<CMemberInternalForcesInLoadCases> MemberInternalForces = new List<CMemberInternalForcesInLoadCases>();
            List<CMemberDeflectionsInLoadCases> MemberDeflections = new List<CMemberDeflectionsInLoadCases>();
            
            //double step = 100.0 / (Model.m_arrMembers.Length * 2.0);
            //double progressValue = 0;
            //PFDMainWindow.UpdateProgressBarValue(progressValue, "");

            // Calculate Internal Forces For Load Cases
            foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
            {
                if (m.BIsDSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
                {
                    for (int i = 0; i < iNumberOfDesignSections; i++)
                        fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                    m.MBucklingLengthFactors = new List<designBucklingLengthFactors>();
                    m.MMomentValuesforCb = new List<designMomentValuesForCb>();
                    m.MBIF_x = new List<basicInternalForces[]>();
                    m.MBDef_x = new List<basicDeflections[]>();

                    foreach (CLoadCase lc in Model.m_arrLoadCases)
                    {
                        // Calculate Internal forces just for Load Cases that are included in ULS
                        if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eULSOnly)
                        {
                            foreach (CMLoad cmload in lc.MemberLoadsList)
                            {
                                if (cmload.Member.ID == m.ID) // TODO - Zatial pocitat len pre zatazenia, ktore lezia priamo skumanom na prute, po zavedeni 3D solveru upravit
                                {
                                    // ULS - internal forces
                                    calcModel.CalculateInternalForcesOnSimpleBeam_PFD(iNumberOfDesignSections, fx_positions, m, (CMLoad_21)cmload, out sBIF_x, out sBucklingLengthFactors, out sMomentValuesforCb);

                                    // SLS - deflections
                                    calcModel.CalculateDeflectionsOnSimpleBeam_PFD(iNumberOfDesignSections, fx_positions, m, (CMLoad_21)cmload, out sBDeflections_x);
                                }
                            }
                        }

                        if (sBIF_x != null) MemberInternalForces.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb));
                        if (sBDeflections_x != null) MemberDeflections.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));

                        //m.MMomentValuesforCb.Add(sMomentValuesforCb);
                        //m.MBIF_x.Add(sBIF_x);
                    }
                }
                //progressValue += step;
                //PFDMainWindow.UpdateProgressBarValue(progressValue, "Calculating Internal Forces. MemberID: " + m.ID);
            }

            // Design of members
            // Calculate Internal Forces For Load Cases

            List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
            List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();

            List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS = new List<CJointLoadCombinationRatio_ULS>();

            foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
            {
                if (m.BIsDSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
                {
                    for (int i = 0; i < iNumberOfDesignSections; i++)
                        fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                    foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
                    {
                        if (lcomb.eLComType == ELSType.eLS_ULS) // Do not perform internal foces calculation for SLS
                        {
                            designBucklingLengthFactors sBucklingLengthFactors_design;

                            // Member basic internal forces
                            designMomentValuesForCb sMomentValuesforCb_design;
                            basicInternalForces[] sBIF_x_design;
                            CMemberResultsManager.SetMemberInternalForcesInLoadCombination(m, lcomb, MemberInternalForces, iNumberOfDesignSections, out sBucklingLengthFactors_design, out sMomentValuesforCb_design, out sBIF_x_design);

                            // Member design internal forces
                            if (m.BIsDSelectedForDesign) // Only structural members (not auxiliary members or members with deactivated design)
                            {
                                designInternalForces[] sMemberDIF_x;

                                // Member Design
                                CMemberDesign memberDesignModel = new CMemberDesign();
                                memberDesignModel.SetDesignForcesAndMemberDesign_PFD(iNumberOfDesignSections, m, sBIF_x_design, sBucklingLengthFactors_design, sMomentValuesforCb_design, out sMemberDIF_x);
                                MemberDesignResults_ULS.Add(new CMemberLoadCombinationRatio_ULS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sMemberDIF_x[memberDesignModel.fMaximumDesignRatioLocationID], sBucklingLengthFactors, sMomentValuesforCb_design));

                                // Set maximum design ratio of whole structure
                                if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioWholeStructure)
                                {
                                    fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                                    MaximumDesignRatioWholeStructureMember = m;
                                }

                                // Joint Design
                                designInternalForces[] sJointDIF_x;
                                CJointDesign jointDesignModel = new CJointDesign();

                                CConnectionJointTypes jointStart;
                                CConnectionJointTypes jointEnd;
                                jointDesignModel.SetDesignForcesAndJointDesign_PFD(iNumberOfDesignSections, Model, m, sBIF_x_design, out jointStart, out jointEnd, out sJointDIF_x);

                                // Start Joint
                                JointDesignResults_ULS.Add(new CJointLoadCombinationRatio_ULS(m, jointStart, lcomb, jointDesignModel.fDesignRatio_Start, sJointDIF_x[jointDesignModel.fDesignRatioLocationID_Start]));

                                // End Joint
                                JointDesignResults_ULS.Add(new CJointLoadCombinationRatio_ULS(m, jointEnd, lcomb, jointDesignModel.fDesignRatio_End, sJointDIF_x[jointDesignModel.fDesignRatioLocationID_End]));

                                // Output (for debugging - member results)
                                bool bDebugging = false; // Testovacie ucely
                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString() + "\n");

                                // Output (for debugging - member connection / joint results)
                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Joint ID: " + jointStart.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(jointDesignModel.fDesignRatio_Start, 3).ToString() + "\n");

                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Joint ID: " + jointEnd.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(jointDesignModel.fDesignRatio_End, 3).ToString() + "\n");

                                 // Output - set maximum design ratio by component Type
                                switch (m.EMemberType)
                                {
                                    case EMemberType_FormSteel.eG: // Girt
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioGirts)
                                            {
                                                fMaximumDesignRatioGirts = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioGirt = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FormSteel.eP: // Purlin
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioPurlins)
                                            {
                                                fMaximumDesignRatioPurlins = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioPurlin = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FormSteel.eC: // Column
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioColumns)
                                            {
                                                fMaximumDesignRatioColumns = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioColumn = m;
                                            }
                                            break;
                                        }
                                    default:
                                        // TODO - modifikovat podla potrieb pre ukladanie - doplnit vsetky typy
                                        break;
                                }
                            }
                        }
                        else // SLS
                        {
                            // Member basic deflections
                            basicDeflections[] sBDeflection_x_design;
                            CMemberResultsManager.SetMemberDeflectionsInLoadCombination(m, lcomb, MemberDeflections, iNumberOfDesignSections, out sBDeflection_x_design);

                            // Member design deflections
                            if (m.BIsDSelectedForDesign) // Only structural members (not auxiliary members or members with deactivated design)
                            {
                                designDeflections[] sDDeflection_x;
                                CMemberDesign memberDesignModel = new CMemberDesign();
                                memberDesignModel.SetDesignDeflections_PFD(iNumberOfDesignSections, m, sBDeflection_x_design, out sDDeflection_x);
                                MemberDesignResults_SLS.Add(new CMemberLoadCombinationRatio_SLS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sDDeflection_x[memberDesignModel.fMaximumDesignRatioLocationID]));

                                // Set maximum design ratio of whole structure
                                if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioWholeStructure)
                                {
                                    fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                                    MaximumDesignRatioWholeStructureMember = m;
                                }

                                // Output (for debugging)
                                bool bDebugging = true; // Testovacie ucely
                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString());
                            }
                        }
                    }
                }
                //progressValue += step;
                //PFDMainWindow.UpdateProgressBarValue(progressValue, "Calculating Member Design. MemberID: " + m.ID);
            }
        }

        public void CalculateGoverningMemberDesignDetails(List<CMemberLoadCombinationRatio_SLS> DesignResults, CMemberGroup GroupOfMembersWithSelectedType, out CCalculMember cGoverningMemberResults)
        {
            cGoverningMemberResults = null;

            if (DesignResults != null) // In case that results set is not empty calculate design details and display particular design results in datagrid
            {
                float fMaximumDesignRatio = 0;

                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    CMemberLoadCombinationRatio_SLS res = DesignResults.Find(i => i.Member.ID == m.ID);
                    if (res == null) continue;
                    CCalculMember c = new CCalculMember(false, res.DesignDeflections, m);

                    if (c.fEta_max > fMaximumDesignRatio)
                    {
                        fMaximumDesignRatio = c.fEta_max;
                        cGoverningMemberResults = c;
                    }
                }

                if (cGoverningMemberResults != null)
                    cGoverningMemberResults.DisplayDesignResultsInGridView(ELSType.eLS_SLS, Results_GridView);
                    //DisplayDesignResultsInGridView(ELSType.eLS_SLS, Results_GridView, cGoverningMemberResults);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }
            }
        }

        //public void DisplayDesignResultsInGridView(ELSType eCombinationType, DataGrid dataGrid, CCalculMember obj_CalcDesign)
        //{
        //    DeleteLists();

        //    if (eCombinationType == ELSType.eLS_ULS)
        //        SetResultsDetailsFor_ULS(obj_CalcDesign);
        //    else
        //        SetResultsDetailsFor_SLS(obj_CalcDesign);

        //    // TODO Ondrej - prepracovat a spojit s UC_JointDesign
        //    // hm? co prepracovat? Ako spojit s UCJointDesign?
            
            
        //    // Create Table
        //    DataTable table = new DataTable("Table");
        //    table.Columns.Add("Symbol", typeof(String));                        
        //    table.Columns.Add("Value", typeof(String));
        //    table.Columns.Add("Unit", typeof(String));

        //    table.Columns.Add("Symbol1", typeof(String));
        //    table.Columns.Add("Value1", typeof(String));
        //    table.Columns.Add("Unit1", typeof(String));

        //    table.Columns.Add("Symbol2", typeof(String));
        //    table.Columns.Add("Value2", typeof(String));
        //    table.Columns.Add("Unit2", typeof(String));

        //    // Set Column Caption
        //    table.Columns["Symbol1"].Caption = table.Columns["Symbol2"].Caption = "Symbol";
        //    table.Columns["Value1"].Caption = table.Columns["Value2"].Caption = "Value";
        //    table.Columns["Unit1"].Caption = table.Columns["Unit2"].Caption = "Unit";
            
        //    // Create Dataset
        //    ds = new DataSet();
        //    // Add Table to Dataset
        //    ds.Tables.Add(table);

        //    for (int i = 0; i < zoznamMenuNazvy.Count; i++)
        //    {
        //        DataRow row = table.NewRow();
        //        try
        //        {
        //            row["Symbol"] = zoznamMenuNazvy[i];
        //            row["Value"] = zoznamMenuHodnoty[i];
        //            row["Unit"] = zoznamMenuJednotky[i];
        //            i++;
        //            if (i >= zoznamMenuNazvy.Count) break;
        //            row["Symbol1"] = zoznamMenuNazvy[i];
        //            row["Value1"] = zoznamMenuHodnoty[i];
        //            row["Unit1"] = zoznamMenuJednotky[i];
        //            i++;
        //            if (i >= zoznamMenuNazvy.Count) break;
        //            row["Symbol2"] = zoznamMenuNazvy[i];
        //            row["Value2"] = zoznamMenuHodnoty[i];
        //            row["Unit2"] = zoznamMenuJednotky[i];                    
        //        }
        //        catch (ArgumentOutOfRangeException) { }
        //        table.Rows.Add(row);
        //    }

        //    dataGrid.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            
        //    //styling datagrid
        //    //TO Mato - tu sa mozes vyblaznit a ponastavovat rozne Style property tak ako sa ti zapaci (farbicky a podobne)
        //    Style alignRight = new Style();
        //    alignRight.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));            
            
        //    Style alignLeft = new Style();            
        //    alignLeft.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left));
        //    //alignLeft.Setters.Add(new Setter(DataGridCell.WidthProperty, DataGridLength.SizeToCells));
        //    Style alignCenter = new Style();
        //    alignCenter.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));            
        //    alignCenter.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.AliceBlue)));
        //    //alignCenter.Setters.Add(new Setter(DataGridCell.WidthProperty, DataGridLength.SizeToCells));
            
        //    dataGrid.Columns[0].CellStyle = alignLeft;
        //    dataGrid.Columns[1].CellStyle = alignRight;
        //    dataGrid.Columns[2].CellStyle = alignCenter;
        //    dataGrid.Columns[3].CellStyle = alignLeft;
        //    dataGrid.Columns[4].CellStyle = alignRight;
        //    dataGrid.Columns[5].CellStyle = alignCenter;
        //    dataGrid.Columns[6].CellStyle = alignLeft;
        //    dataGrid.Columns[7].CellStyle = alignRight;
        //    dataGrid.Columns[8].CellStyle = alignCenter;

        //    // Set Column Header
        //    dataGrid.Columns[0].Header = dataGrid.Columns[3].Header = dataGrid.Columns[6].Header = "Symbol";
        //    dataGrid.Columns[1].Header = dataGrid.Columns[4].Header = dataGrid.Columns[7].Header = "Value";
        //    dataGrid.Columns[2].Header = dataGrid.Columns[5].Header = dataGrid.Columns[8].Header = "Unit";
            
        //    //Set Column Width
        //    //Results_GridView.Columns[0].Width = Results_GridView.Columns[3].Width = Results_GridView.Columns[6].Width = 117;
        //    //Results_GridView.Columns[1].Width = Results_GridView.Columns[4].Width = Results_GridView.Columns[7].Width = 90;
        //    //Results_GridView.Columns[2].Width = Results_GridView.Columns[5].Width = Results_GridView.Columns[8].Width = 90;            

        //}
    }
}
