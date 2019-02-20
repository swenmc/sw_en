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
using MATH;

namespace PFD
{
    /// <summary>
    /// Interaction logic for LoadCasesList.xaml
    /// </summary>
    public partial class UC_LoadCaseList : UserControl
    {
        CPFDViewModel _pfdViewModel;
        public UC_LoadCaseList(CPFDViewModel vm)
        {
            _pfdViewModel = vm;
            InitializeComponent();
            
            List<CLoadCaseView> loadCases = new List<CLoadCaseView>();
            // For each load case add one row
            for (int i = 0; i < vm.Model.m_arrLoadCases.Length; i++)
            {
                loadCases.Add(new CLoadCaseView(vm.Model.m_arrLoadCases[i].ID, vm.Model.m_arrLoadCases[i].Name, vm.Model.m_arrLoadCases[i].Type.GetFriendlyName()));                
            }
            
            Datagrid_LoadCases.ItemsSource = loadCases;

            // Set Column Header
            // TODO Ondrej - chcelo by to nastavit Column Name na ID namiesto loadCaseID, teraz to berie asi z nazvu property vo view model CLoadCaseView
            //Datagrid_LoadCases.Columns[0].Header = "ID";
            //Datagrid_LoadCases.Columns[1].Header = "Name";
            //Datagrid_LoadCases.Columns[2].Header = "Type";

            // Set Column Width
            //Datagrid_LoadCases.Columns[0].Width = 100;
            //Datagrid_LoadCases.Columns[1].Width = 100;
            //Datagrid_LoadCases.Columns[2].Width = 100;
        }

        private void Datagrid_LoadCases_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGridCellInfo row = e.AddedCells.FirstOrDefault();
            if (row != null)
            {
                if (row.Item == null) return;
                if (row.Item is CLoadCaseView)
                {
                    CLoadCaseView lcw = (CLoadCaseView)row.Item;
                    int index = GetLoadCaseIndex(lcw);
                    if (index != -1) _pfdViewModel.LoadCaseIndex = index;
                }
            } 
        }

        private int GetLoadCaseIndex(CLoadCaseView lcw)
        {
            if (lcw == null) return -1;
            for (int i = 0; i < _pfdViewModel.Model.m_arrLoadCases.Length; i++)
            {
                if (lcw.LoadCaseID == _pfdViewModel.Model.m_arrLoadCases[i].ID) return i;
            }

            return -1;
        }

    }
}
