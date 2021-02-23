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

            SetControlsVisibility();
        }

        private void HandleFibreglassPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            SetControlsVisibility();
        }

        private void SetControlsVisibility()
        {
            if (vm.GenerateRaster)
            {
                LabelNumberOfRows.Visibility = Visibility.Visible;
                Combobox_RowsCount.Visibility = Visibility.Visible;
                chbEqualSpacing.Visibility = Visibility.Visible;
                chbEnableVariableLengths.Visibility = Visibility.Visible;

                if (vm.EqualSpacing)
                {
                    LabelSpacing.Visibility = Visibility.Visible;
                    TxtSpacing.Visibility = Visibility.Visible;
                }
                else
                {
                    LabelSpacing.Visibility = Visibility.Collapsed;
                    TxtSpacing.Visibility = Visibility.Collapsed;
                }

                TxtY2.Visibility = Visibility.Visible;
                TxtY3.Visibility = Visibility.Visible;
                TxtY4.Visibility = Visibility.Visible;
                TxtY5.Visibility = Visibility.Visible;
                TxtLength2.Visibility = Visibility.Visible;
                TxtLength3.Visibility = Visibility.Visible;
                TxtLength4.Visibility = Visibility.Visible;
                TxtLength5.Visibility = Visibility.Visible;
                if (vm.RowsCount < 3) { TxtY3.Visibility = Visibility.Collapsed; TxtLength3.Visibility = Visibility.Collapsed; }
                if (vm.RowsCount < 4) { TxtY4.Visibility = Visibility.Collapsed; TxtLength4.Visibility = Visibility.Collapsed; }
                if (vm.RowsCount < 5) { TxtY5.Visibility = Visibility.Collapsed; TxtLength5.Visibility = Visibility.Collapsed; }

                TxtY2.IsEnabled = vm.EnableVariableLengths;
                TxtY3.IsEnabled = vm.EnableVariableLengths;
                TxtY4.IsEnabled = vm.EnableVariableLengths;
                TxtY5.IsEnabled = vm.EnableVariableLengths;
            }
            else
            {
                LabelNumberOfRows.Visibility = Visibility.Collapsed;
                Combobox_RowsCount.Visibility = Visibility.Collapsed;
                chbEqualSpacing.Visibility = Visibility.Collapsed;
                chbEnableVariableLengths.Visibility = Visibility.Collapsed;
                LabelSpacing.Visibility = Visibility.Collapsed;
                TxtSpacing.Visibility = Visibility.Collapsed;
                TxtY2.Visibility = Visibility.Collapsed;
                TxtY3.Visibility = Visibility.Collapsed;
                TxtY4.Visibility = Visibility.Collapsed;
                TxtY5.Visibility = Visibility.Collapsed;
                TxtLength2.Visibility = Visibility.Collapsed;
                TxtLength3.Visibility = Visibility.Collapsed;
                TxtLength4.Visibility = Visibility.Collapsed;
                TxtLength5.Visibility = Visibility.Collapsed;
            }
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
