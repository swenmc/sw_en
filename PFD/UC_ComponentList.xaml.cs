using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using BaseClasses;
using DATABASE;
using MATH;
using System.ComponentModel;

namespace PFD
{
    /// <summary>
    /// Interaction logic for ComponentList.xaml
    /// </summary>
    public partial class UC_ComponentList : UserControl
    {
        DataSet ds;
        List<string> listSectionPropertyName = new List<string>(1);
        List<string> listSectionPropertySymbol = new List<string>(1);
        List<string> listSectionPropertyValue = new List<string>(1);
        List<string> listSectionPropertyUnit = new List<string>(1);
        
        public UC_ComponentList()
        {
            InitializeComponent();

            // TODO Ondrej - zrusit napojenie na objekt DatabaseComponents database a nahradit vstupmi z SQL databazy - MDBModels, tabulka componentPrefixes
            // Obsah tejto tabulky co sa tyka prierezov by sa mal prebrat z modelu CExample_3D_901_PF - m_arrSections
            // Material prevziat z prierezov

            // TODO - Ondrej - obsah tohto zoznamu preberat z CExample_3D_901_PF - zoznam listOfModelMemberGroups
            CComponentListVM cl = new CComponentListVM();
            cl.PropertyChanged += HandleComponentListPropertyChangedEvent;
            this.DataContext = cl;
            cl.SelectedComponentIndex = 0;
        }

        protected void HandleComponentListPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CComponentListVM viewModel = sender as CComponentListVM;
            if (viewModel == null) return;

            if (e.PropertyName == "SelectedComponentIndex")
            {
                SetComponentDetails(viewModel.SelectedComponentIndex);
            }
        }
        
        private void DeleteDetails()
        {
            listSectionPropertyName.Clear();
            listSectionPropertySymbol.Clear();
            listSectionPropertyValue.Clear();
            listSectionPropertyUnit.Clear();
        }

        //private void Datagrid_Components_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // TODO Ondrej - zapracovat update obsahu Datagrid_ComponentDetails podla indexu v comboboxe
        //    // na vybranom riadku v Datagrid_Components
        //    // TERAZ JE TO NESPRAVNE, neviem ako vydolovat z comboboxe v gride selectedindex na prislusnom riadku

        //    var rowIndex = this.Datagrid_Components.Items.IndexOf(this.Datagrid_Components.SelectedItem);

        //    if(rowIndex > 0)
        //        iSelectedComponentCrossSectionIndex = rowIndex + 1;

        //    int sectionID = iSelectedComponentCrossSectionIndex; // iSelectedComponentCrossSectionIndex; // TODO Ondrej Nastavovat obsah Datagrid_ComponentDetails podla prierezu na vybranom riadku v Datagrid_Components

        //    SetComponentDetails(sectionID);
        //}

        private void SetComponentDetails(int sectionID)
        {

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Cross-section details
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // TODO Ondrej - zapracovat do CComponentListVM

            // TO Ondrej, viem ze som to spravil nespravne starym sposobom, ale nejako sa mi s tym VM nedarilo, nevedel som kde mam co presne dat,
            // asi som to este dostatocne nepochopil, prosim o refaktoring

            DeleteDetails(); // Clear previous data

            List<DATABASE.DTO.CSectionPropertiesText> sectionTexts = CSectionManager.LoadSectionPropertiesNamesSymbolsUnits();
            listSectionPropertyValue = CSectionManager.LoadSectionPropertiesStringList(sectionID+1);

            // Default - other properties
            foreach (DATABASE.DTO.CSectionPropertiesText textRow in sectionTexts)
            {
                listSectionPropertyName.Add(textRow.text);
                listSectionPropertySymbol.Add(textRow.symbol);
                listSectionPropertyUnit.Add(textRow.unit_NmmMpa);
            }

            // Create Table
            DataTable table = new DataTable("Table");

            // Create Table Rows
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("Symbol", typeof(String));
            table.Columns.Add("Value", typeof(String));
            table.Columns.Add("Unit", typeof(String));

            // Set Column Caption
            table.Columns["Name"].Caption = "Name";
            table.Columns["Symbol"].Caption = "Symbol";
            table.Columns["Value"].Caption = "Value";
            table.Columns["Unit"].Caption = "Unit";

            // Create Datases
            ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < listSectionPropertyName.Count; i++)
            {
                DataRow row = table.NewRow();

                try
                {
                    row["Name"] = listSectionPropertyName[i];
                    row["Symbol"] = listSectionPropertySymbol[i];
                    row["Value"] = listSectionPropertyValue[i];
                    row["Unit"] = listSectionPropertyUnit[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            Datagrid_ComponentDetails.ItemsSource = ds.Tables[0].AsDataView();  //draw the table to datagridview
        }
    }
}
