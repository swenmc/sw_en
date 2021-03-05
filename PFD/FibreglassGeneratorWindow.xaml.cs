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

                // Fibreglass sheet y-positions
                TxtY2.Visibility = Visibility.Visible;
                TxtY3.Visibility = Visibility.Visible;
                TxtY4.Visibility = Visibility.Visible;
                TxtY5.Visibility = Visibility.Visible;

                // Ak je zapnute equal spacing, tak hodnoty maju byt diabled / resp. read-only
                TxtY2.IsEnabled = !vm.EqualSpacing;
                TxtY3.IsEnabled = !vm.EqualSpacing;
                TxtY4.IsEnabled = !vm.EqualSpacing;
                TxtY5.IsEnabled = !vm.EqualSpacing;

                // Fibreglass sheet lengths
                TxtLength2.Visibility = Visibility.Visible;
                TxtLength3.Visibility = Visibility.Visible;
                TxtLength4.Visibility = Visibility.Visible;
                TxtLength5.Visibility = Visibility.Visible;

                // Ak je zapnute enable variable lengths, tak hodnoty maju byt editovatelne
                TxtLength2.IsEnabled = vm.EnableVariableLengths;
                TxtLength3.IsEnabled = vm.EnableVariableLengths;
                TxtLength4.IsEnabled = vm.EnableVariableLengths;
                TxtLength5.IsEnabled = vm.EnableVariableLengths;

                if (vm.RowsCount < 3) { TxtY3.Visibility = Visibility.Collapsed; TxtLength3.Visibility = Visibility.Collapsed; }
                if (vm.RowsCount < 4) { TxtY4.Visibility = Visibility.Collapsed; TxtLength4.Visibility = Visibility.Collapsed; }
                if (vm.RowsCount < 5) { TxtY5.Visibility = Visibility.Collapsed; TxtLength5.Visibility = Visibility.Collapsed; }
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

        public List<FibreglassProperties> GetFibreglassPropertiesWithNoCollisions()
        {
            if (vm.AddFibreglass == false) return new List<FibreglassProperties>();

            return GetNoCollisionsItems(GetFibreglassPropertiesBasedOnPeriodicity());
        }
        //public List<FibreglassProperties> GetFibreglassProperties()
        //{
        //    if (vm.AddFibreglass == false) return new List<FibreglassProperties>();

        //    return GetFibreglassPropertiesBasedOnPeriodicity();
        //}

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

            for (int i = index; i < vm.XValues.Count; i += (index + 1))
            {
                if (!vm.GenerateRaster)
                {
                    FibreglassProperties fg = vm.GetFibreglass();
                    fg.X = vm.XValues[i];
                    items.Add(fg);
                }
                else
                {
                    for (int j = 0; j < vm.RowsCount; j++) // Pre kazdu zadany rad na suradnici Y nastavime hodnotu
                    {
                        FibreglassProperties fg = vm.GetFibreglass();
                        fg.X = vm.XValues[i];
                                                
                        if (j == 1)
                        {
                            fg.Y = /*.EqualSpacing ? (vm.Y + vm.Spacing) :*/ vm.Y2;                            
                            fg.Length = vm.Length2;
                        }
                        else if (j == 2)
                        {
                            fg.Y = /*vm.EqualSpacing ? (vm.Y2 + vm.Spacing) :*/ vm.Y3;                            
                            fg.Length = vm.Length3;
                        }
                        else if (j == 3)
                        {
                            fg.Y = /*vm.EqualSpacing ? (vm.Y3 + vm.Spacing) :*/ vm.Y4;                            
                            fg.Length = vm.Length4;
                        }
                        else if (j == 4)
                        {
                            fg.Y = /*vm.EqualSpacing ? (vm.Y4 + vm.Spacing) :*/ vm.Y5;                            
                            fg.Length = vm.Length5;
                        }

                        items.Add(fg);
                    } //end rows count
                } //end else generate raster
            }

            return items;
        }

        private List<FibreglassProperties> GetNoCollisionsItems(List<FibreglassProperties> items)
        {
            List<FibreglassProperties> itemsWithNoCollisions = new List<FibreglassProperties>();

            foreach (FibreglassProperties f in items)
            {
                if (!HasCollisions(f, itemsWithNoCollisions)) itemsWithNoCollisions.Add(f);
            }

            return itemsWithNoCollisions;
        }
        private bool WillGeneratedItemsHaveCollisions()
        {
            List<FibreglassProperties> items = GetFibreglassPropertiesBasedOnPeriodicity();
            List<FibreglassProperties> itemsWithNoCollisions = new List<FibreglassProperties>();
            bool collisionDetected = false;
            foreach (FibreglassProperties f in items)
            {
                if (!HasCollisions(f, itemsWithNoCollisions)) itemsWithNoCollisions.Add(f);
                else { collisionDetected = true; break; }
            }

            items = null;
            itemsWithNoCollisions = null;
            return collisionDetected;
        }
        private bool HasCollisions(FibreglassProperties f, List<FibreglassProperties> items)
        {
            foreach (FibreglassProperties fp in items)
            {
                if (f.IsInCollisionWith(fp)) return true;
            }

            return false;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool collisionDetected = WillGeneratedItemsHaveCollisions();
            if (collisionDetected)
            {
                MessageBoxResult res = MessageBox.Show("Collisions ware detected. Do you want to solve them automatically?", "Attention", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.No)
                {
                    return; //do not close generator window, user must change values
                }
            }
            
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
