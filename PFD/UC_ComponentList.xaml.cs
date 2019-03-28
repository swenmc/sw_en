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
using DATABASE.DTO;

namespace PFD
{
    /// <summary>
    /// Interaction logic for ComponentList.xaml
    /// </summary>
    public partial class UC_ComponentList : UserControl
    {
        public UC_ComponentList()
        {
            InitializeComponent();
            
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
        }

        //private void DataGridCheckBoxColumn_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is CheckBox)
        //    {
        //        CComponentInfo viewModel = ((CheckBox)sender).DataContext as CComponentInfo;

        //    }
            
            
        //    //viewModel.ComponentList[viewModel.SelectedComponentIndex].Generate = false;

        //}

        private void Datagrid_Components_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                //CComponentInfo cInfo = ((DataGrid)sender).SelectedValue as CComponentInfo;
                CComponentListVM vm = this.DataContext as CComponentListVM;
                CComponentInfo cInfo = vm.ComponentList[((DataGrid)sender).SelectedIndex];

                if (cInfo.Generate)
                {
                    if (cInfo.ValidateGenerateCouldBeChanged()) cInfo.Generate = false;
                    else
                    {
                        cInfo.IsSetFromCode = true;
                        cInfo.Display = false;
                        cInfo.Design = false;
                        cInfo.Calculate = false;
                        cInfo.IsSetFromCode = false;
                        cInfo.MaterialList = false;
                    }

                }
                else
                {
                    cInfo.IsSetFromCode = true;
                    cInfo.Generate = true;
                    cInfo.Display = true;
                    cInfo.Design = true;
                    cInfo.Calculate = true;
                    cInfo.IsSetFromCode = false;
                    cInfo.MaterialList = true;
                }
                
            }
            
        }

        private void Datagrid_Components_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                string header = ((DataGrid)sender).CurrentColumn.Header.ToString();
                CComponentListVM vm = this.DataContext as CComponentListVM;
                CComponentInfo selectedInfo = ((DataGrid)sender).SelectedValue as CComponentInfo;

                bool generate = selectedInfo.Generate;
                bool calc = selectedInfo.Calculate;
                bool design = selectedInfo.Design;
                bool display = selectedInfo.Display;
                bool material = selectedInfo.MaterialList;
                
                if (vm == null) return;

                foreach (CComponentInfo cInfo in vm.ComponentList)
                {
                    if (vm.ComponentList.IndexOf(cInfo) == vm.ComponentList.Count - 1)
                    {
                        cInfo.IsSetFromCode = false;
                    }
                    else cInfo.IsSetFromCode = true;

                    if (header == "Generate") cInfo.Generate = !generate;
                    if (header == "Calculate") cInfo.Calculate = !calc;
                    if (header == "Design") cInfo.Design = !design;
                    if (header == "Display") cInfo.Display = !display;
                    if (header == "MaterialList") cInfo.MaterialList = !material;
                }
                

            }
        }
    }
}
