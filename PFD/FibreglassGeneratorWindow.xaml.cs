using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BaseClasses;
using MATH;

namespace PFD
{
    public partial class FibreglassGeneratorWindow : Window
    {        
        FibreglassGeneratorViewModel vm;
        CPFDViewModel _pfdVM;

        public FibreglassGeneratorWindow(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;
                        
            vm = new FibreglassGeneratorViewModel((EModelType_FS)_pfdVM.KitsetTypeIndex, _pfdVM.Width, _pfdVM.Length, _pfdVM._claddingOptionsVM.WallCladdingProps.widthModular_m, _pfdVM._claddingOptionsVM.RoofCladdingProps.widthModular_m);
            vm.PropertyChanged += HandleFibreglassPropertyChanged;
            this.DataContext = vm;
        }

        private void HandleFibreglassPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
        }
        
        public List<FibreglassProperties> GetFibreglassProperties()
        {
            if (vm.AddFibreglass == false) return new List<FibreglassProperties>();

            return GetFibreglassPropertiesBasedOnPeriodicity();
        }

        public List<FibreglassProperties> GetFibreglassToDelete()
        {   
            if (vm.DeleteFibreglass == false) return new List<FibreglassProperties>();

            return GetFibreglassPropertiesBasedOnPeriodicity();
        }

        private List<FibreglassProperties> GetFibreglassPropertiesBasedOnPeriodicity()
        {
            List<FibreglassProperties> items = new List<FibreglassProperties>();
            
            int index = vm.PeriodicityValues.IndexOf(vm.Periodicity);
            if (index < 0) return items;

            for (int i = 0; i < vm.XValues.Count; i += (index + 1))
            {
                FibreglassProperties f = vm.GetFibreglass();
                f.X = vm.XValues[i];
                items.Add(f);
            }

            return items;
        }





        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            vm.AddFibreglass = true;
            this.Close();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            vm.DeleteFibreglass = true;
            this.Close();
        }
    }
}
