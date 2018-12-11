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
        DataSet ds;
        List<string> zoznamMenuNazvy = new List<string>(4);          // premenne zobrazene v tabulke
        List<string> zoznamMenuHodnoty = new List<string>(4);        // hodnoty danych premennych
        List<string> zoznamMenuJednotky = new List<string>(4);       // jednotky danych premennych

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
            //tabItem Member Design sa zobrazuje len ak su vysledky k dispozicii

            
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
                    //tu sa vyberie zo zoznamu taky prvok ktory ma zhodne Member.ID
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
                    DisplayDesignResultsInGridView(ELSType.eLS_ULS, Results_GridView, cGoverningMemberResults);
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

                //Mato mam otazku: Preco tu neprejdeme zoznam DesignResults a vypocet spravime pre vsetky Members z DesignResults, ktore maju dany typ vybraty v kombe ComponentType???
                // To Ondrej GroupOfMembers maju rovnake typy ako ComponentType (napriklad Girts - Front Side) ale Member ma ine typy (napr. len Girt)
                // Mena GroupOfMembers alebo ComponentType kombinuju polohu v konstrukcii a zakladny typ

                // Ak tomu rozumiem spravne podla toho co navrhujes by sme presli vsetky Girts ktore su na stene vpredu, vzadu a tak dalej
                // Member moze mat zakladny typ girt a mozeme mu pridat este jeden girt-front side alebo nejaky odkaz na structure part (napr. Front Side) cim by sme sa k tomu dostali
                // Ide o to ze girt je vseobecny nazov a v component je kombinacia nazvu a polohy

                // Nieco ako stolicka (ta sama o sebe nevie kde je, vie len ze je stolicka), stoly, obrazy
                // preto som do stolicky nechcel davat informaciu, patrim do obyvacky

                // a potom su stolicky v obyvacke, stolicky v kuchyni, stolicky v jedalni
                // Myslel som ze CEntityGroup alebo CMemberGroup bude nieco ako obyvacka, kuchyna, jedalen a ta bude vediet ze je obyvacka a ktore stolicky, stoly, obrazy v nej su

                // Ak to vidis logicky inak a jednoduchsie, nebranim sa
                // Mozno ma stolicka vediet nielen to ze je stolicka, ale aj kde je:)

                foreach (CMember m in GroupOfMembersWithSelectedType.ListOfMembers)
                {
                    // TODO - Ondrej, vieme member ale potrebujeme sa dostat v zozname DesignResults na riadok ktory odpoveda uvedenemu member
                    // hodnota ID - 1 je nespolahlive pretoze pocet zaznamov v DesignResults nemusi byt rovnaky ako pocet prutov v modeli, nemusia sa pocitat vsetky

                    //tu sa vyberie zo zoznamu taky prvok ktory ma zhodne Member.ID
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
                    DisplayDesignResultsInGridView(ELSType.eLS_SLS, Results_GridView, cGoverningMemberResults);
                else
                {
                    // Error - object is null, results are not available, object shouldn't be in the list or there must be valid results (or reasonable invalid design ratio)
                    // throw new Exception("Results of selected component are not available!");
                    MessageBox.Show("Results of selected component are not available!");
                }
            }
        }

        public void DisplayDesignResultsInGridView(ELSType eCombinationType, DataGrid dataGrid, CCalculMember obj_CalcDesign)
        {
            DeleteLists();

            if (eCombinationType == ELSType.eLS_ULS)
                SetResultsDetailsFor_ULS(obj_CalcDesign);
            else
                SetResultsDetailsFor_SLS(obj_CalcDesign);

            // TODO Ondrej - prepracovat a spojit s UC_JointDesign
            // hm? co prepracovat? Ako spojit s UCJointDesign?
            
            
            // Create Table
            DataTable table = new DataTable("Table");
            table.Columns.Add("Symbol", typeof(String));                        
            table.Columns.Add("Value", typeof(String));
            table.Columns.Add("Unit", typeof(String));

            table.Columns.Add("Symbol1", typeof(String));
            table.Columns.Add("Value1", typeof(String));
            table.Columns.Add("Unit1", typeof(String));

            table.Columns.Add("Symbol2", typeof(String));
            table.Columns.Add("Value2", typeof(String));
            table.Columns.Add("Unit2", typeof(String));

            // Set Column Caption
            table.Columns["Symbol1"].Caption = table.Columns["Symbol2"].Caption = "Symbol";
            table.Columns["Value1"].Caption = table.Columns["Value2"].Caption = "Value";
            table.Columns["Unit1"].Caption = table.Columns["Unit2"].Caption = "Unit";
            
            // Create Dataset
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < zoznamMenuNazvy.Count; i++)
            {
                DataRow row = table.NewRow();
                try
                {
                    row["Symbol"] = zoznamMenuNazvy[i];
                    row["Value"] = zoznamMenuHodnoty[i];
                    row["Unit"] = zoznamMenuJednotky[i];
                    i++;
                    if (i >= zoznamMenuNazvy.Count) break;
                    row["Symbol1"] = zoznamMenuNazvy[i];
                    row["Value1"] = zoznamMenuHodnoty[i];
                    row["Unit1"] = zoznamMenuJednotky[i];
                    i++;
                    if (i >= zoznamMenuNazvy.Count) break;
                    row["Symbol2"] = zoznamMenuNazvy[i];
                    row["Value2"] = zoznamMenuHodnoty[i];
                    row["Unit2"] = zoznamMenuJednotky[i];                    
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            dataGrid.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
            
            //styling datagrid
            //TO Mato - tu sa mozes vyblaznit a ponastavovat rozne Style property tak ako sa ti zapaci (farbicky a podobne)
            Style alignRight = new Style();
            alignRight.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));            
            
            Style alignLeft = new Style();            
            alignLeft.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left));
            //alignLeft.Setters.Add(new Setter(DataGridCell.WidthProperty, DataGridLength.SizeToCells));
            Style alignCenter = new Style();
            alignCenter.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));            
            alignCenter.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.AliceBlue)));
            //alignCenter.Setters.Add(new Setter(DataGridCell.WidthProperty, DataGridLength.SizeToCells));
            
            dataGrid.Columns[0].CellStyle = alignLeft;
            dataGrid.Columns[1].CellStyle = alignRight;
            dataGrid.Columns[2].CellStyle = alignCenter;
            dataGrid.Columns[3].CellStyle = alignLeft;
            dataGrid.Columns[4].CellStyle = alignRight;
            dataGrid.Columns[5].CellStyle = alignCenter;
            dataGrid.Columns[6].CellStyle = alignLeft;
            dataGrid.Columns[7].CellStyle = alignRight;
            dataGrid.Columns[8].CellStyle = alignCenter;

            //foreach (var column in dataGrid.Columns) {
            //    column.Width = DataGridLength.SizeToHeader;
            //}


            /*
            // Set Column Header
            Results_GridView.Columns[0].Header = Results_GridView.Columns[3].Header = Results_GridView.Columns[6].Header = "Symbol";
            Results_GridView.Columns[1].Header = Results_GridView.Columns[4].Header = Results_GridView.Columns[7].Header = "Value";
            Results_GridView.Columns[2].Header = Results_GridView.Columns[5].Header = Results_GridView.Columns[8].Header = "Unit";
            */

            //Set Column Width
            //Results_GridView.Columns[0].Width = Results_GridView.Columns[3].Width = Results_GridView.Columns[6].Width = 117;
            //Results_GridView.Columns[1].Width = Results_GridView.Columns[4].Width = Results_GridView.Columns[7].Width = 90;
            //Results_GridView.Columns[2].Width = Results_GridView.Columns[5].Width = Results_GridView.Columns[8].Width = 90;            

        }

        private void DeleteLists()
        {
            // Deleting lists for updating actual values
            zoznamMenuNazvy.Clear();
            zoznamMenuHodnoty.Clear();
            zoznamMenuJednotky.Clear();
        }

        private void SetResultsDetailsFor_ULS(CCalculMember obj_CalcDesign)
        {
            float fUnitFactor_Force = 0.001f;     // from N to kN
            float fUnitFactor_Moment = 0.001f;    // from Nm to kNm
            float fUnitFactor_Stress = 0.000001f; // from Pa to MPa

            int iNumberOfDecimalPlaces = 3;


            // Display results in datagrid
            // AS 4600 output variables

            // Tension
            if (obj_CalcDesign.fEta_Nt > 0)
            {
                zoznamMenuNazvy.Add("φ t");
                zoznamMenuHodnoty.Add(obj_CalcDesign.fPhi_t.ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("N t,min");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_t_min * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[N]");

                zoznamMenuNazvy.Add("η Nt");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_Nt, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");
            }

            // Compression
            // Global Buckling
            if (obj_CalcDesign.fEta_721_N > 0 || obj_CalcDesign.fEta_722_M_xu > 0)
            {
                zoznamMenuNazvy.Add("f ox");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_ox * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                zoznamMenuNazvy.Add("f oy");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_oy * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                zoznamMenuNazvy.Add("f oz");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_oz * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                if (obj_CalcDesign.fEta_721_N > 0)
                {
                    zoznamMenuNazvy.Add("f oc");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_oc * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[Pa]");

                    zoznamMenuNazvy.Add("λ c");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.flambda_c, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");

                    zoznamMenuNazvy.Add("N y");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_y * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N oc");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_oc * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N ce");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_ce * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    // Local Buckling
                    zoznamMenuNazvy.Add("f ol");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_oy * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[Pa]");

                    zoznamMenuNazvy.Add("λ l");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.flambda_l, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");

                    zoznamMenuNazvy.Add("N ol");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_ol * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N cl");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_cl * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    // Distorsial Buckling
                    zoznamMenuNazvy.Add("f od");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_od * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[Pa]");

                    zoznamMenuNazvy.Add("λ d");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.flambda_d, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");

                    zoznamMenuNazvy.Add("N od");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_od * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N cd");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_cd * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N c,min");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_c_min * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("φ c");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fPhi_c, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");

                    zoznamMenuNazvy.Add("η Nc");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_721_N, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");
                }
            }

            // Bending

            zoznamMenuNazvy.Add("M p,x");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_p_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M y,x");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_y_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M p,y");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_p_yv * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M y,y");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_y_yv * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            if (obj_CalcDesign.fEta_722_M_xu > 0)
            {
                // Bending about x/u axis
                zoznamMenuNazvy.Add("C b");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fC_b, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("M o,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_o_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("M be,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_be_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                // Local Buckling
                zoznamMenuNazvy.Add("f ol,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_ol_bend * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                zoznamMenuNazvy.Add("M ol,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_ol_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("λ l,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fLambda_l_xu, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("M bl,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_bl_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                // Distrosial buckling
                zoznamMenuNazvy.Add("f od,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_od_bend * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                zoznamMenuNazvy.Add("M od,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_od_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("λ d,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fLambda_d_xu, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("M bd,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_bd_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("φ b");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fPhi_b, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_722_M_xu, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");
            }

            // Shear
            if (obj_CalcDesign.fEta_723_9_xu_yv > 0)
            {
                zoznamMenuNazvy.Add("V y,y");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fV_y_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[N]");

                zoznamMenuNazvy.Add("V v,y");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fV_v_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[N]");

                zoznamMenuNazvy.Add("V cr,y");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fV_cr_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[N]");

                zoznamMenuNazvy.Add("λ v,y");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fLambda_v_yv, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("φ v");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fPhi_v, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_723_9_xu_yv, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_723_11_V_yv, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                if (obj_CalcDesign.fEta_723_10_xu > 0)
                {
                    zoznamMenuNazvy.Add("η");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_723_10_xu, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");
                }
           
                if(obj_CalcDesign.fEta_723_12_xu_yv_10 > 0)
                {
                    zoznamMenuNazvy.Add("η");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_723_12_xu_yv_10, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");
                }
            }

            // Interation of internal forces
            zoznamMenuNazvy.Add("M s,x");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_s_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M b,x");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_b_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            // Compression and bending
            if (obj_CalcDesign.fEta_721_N > 0 && obj_CalcDesign.fEta_724 > 0)
            {
                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_724, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");
            }

            // Tension and bending
            if (obj_CalcDesign.fEta_Nt > 0 && obj_CalcDesign.fEta_725_1 > 0)
            {
                zoznamMenuNazvy.Add("M s,x,f");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_s_xu_f * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("M s,y,f");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_s_yv_f * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_725_1, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_725_2, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");
            }

            // Maximum design ratio
            zoznamMenuNazvy.Add("η max");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_max, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");
        }

        private void SetResultsDetailsFor_SLS(CCalculMember obj_CalcDesign)
        {
            // Display results in datagrid

            // Deflection
            // δ

            zoznamMenuNazvy.Add("δ lim");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fLimitDeflection.ToString());
            zoznamMenuJednotky.Add("[mm]");

            // Design ratio
            zoznamMenuNazvy.Add("η x/u");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_yu.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η y/v");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_zv.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_yy.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_zz.ToString());
            zoznamMenuJednotky.Add("[mm]");
        }
    }
}
