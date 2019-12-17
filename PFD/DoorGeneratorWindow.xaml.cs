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
using BaseClasses;
using MATH;

namespace PFD
{
    public partial class DoorGeneratorWindow : Window
    {
        List<int> lefRightBays;
        List<int> frontBackBays;
        DoorGeneratorViewModel vm;

        public DoorGeneratorWindow(int lrBaysNum, int fbBaysNum)
        {
            InitializeComponent();
            lefRightBays = new List<int>();
            frontBackBays = new List<int>();

            for (int i = 1; i <= lrBaysNum; i++)
            {
                CheckBox c = new CheckBox();
                c.Name = $"leftBay{i}";
                c.Content = i;
                c.Margin = new Thickness(0, 0, 20, 0);
                leftBays.Children.Add(c);
                c = new CheckBox();
                c.Name = $"rightBay{i}";
                c.Content = i;
                c.Margin = new Thickness(0, 0, 20, 0);
                rightBays.Children.Add(c);

                lefRightBays.Add(i);
            }

            for (int i = 1; i <= fbBaysNum; i++)
            {
                CheckBox c = new CheckBox();
                c.Name = $"frontBay{i}";
                c.Content = i;
                c.Margin = new Thickness(0, 0, 20, 0);
                frontBays.Children.Add(c);
                c = new CheckBox();
                c.Name = $"backBay{i}";
                c.Content = i;
                c.Margin = new Thickness(0, 0, 20, 0);
                backBays.Children.Add(c);

                frontBackBays.Add(i);
            }

            vm = new DoorGeneratorViewModel();
            vm.PropertyChanged += HandleDoorGeneratorPropertyChanged;
            this.DataContext = vm;
        }

        private void HandleDoorGeneratorPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
        }


        public List<DoorProperties> GetDoorProperties()
        {
            List<DoorProperties> doorProperties = new List<DoorProperties>();
            if (vm.AddDoors == false) return doorProperties;
            
            for (int i = 1; i <= lefRightBays.Count; i++)
            {
                if ((leftBays.Children[i] as CheckBox).IsChecked == true)
                {
                    DoorProperties dp = GetModelDoorProperties();
                    dp.Bays = lefRightBays;
                    dp.iBayNumber = i;
                    dp.sBuildingSide = "Left";
                    doorProperties.Add(dp);
                }
                if ((rightBays.Children[i] as CheckBox).IsChecked == true)
                {
                    DoorProperties dp = GetModelDoorProperties();
                    dp.Bays = lefRightBays;
                    dp.iBayNumber = i;
                    dp.sBuildingSide = "Right";
                    doorProperties.Add(dp);
                }
            }

            for (int i = 1; i <= frontBackBays.Count; i++)
            {
                if ((frontBays.Children[i] as CheckBox).IsChecked == true)
                {
                    DoorProperties dp = GetModelDoorProperties();
                    dp.Bays = frontBackBays;
                    dp.iBayNumber = i;
                    dp.sBuildingSide = "Front";
                    doorProperties.Add(dp);
                }
                if ((backBays.Children[i] as CheckBox).IsChecked == true)
                {
                    DoorProperties dp = GetModelDoorProperties();
                    dp.Bays = frontBackBays;
                    dp.iBayNumber = i;
                    dp.sBuildingSide = "Back";
                    doorProperties.Add(dp);
                }
            }

            return doorProperties;
        }

        private DoorProperties GetModelDoorProperties()
        {
            DoorProperties dp = new DoorProperties();
            dp.fDoorsHeight = vm.DoorsHeight;
            dp.fDoorsWidth = vm.DoorsWidth;
            dp.fDoorCoordinateXinBlock = vm.DoorCoordinateXinBlock;
            dp.sDoorType = vm.DoorType;
            dp.CoatingColors = vm.CoatingColors;
            dp.CoatingColor = vm.CoatingColor;
            return dp;
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            vm.AddDoors = true;
            this.Close();
        }

        private void LeftBaysAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox c in leftBays.Children)
            {
                c.IsChecked = true;
            }
        }

        private void LeftBaysAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox c in leftBays.Children)
            {
                c.IsChecked = false;
            }
        }

        private void RightBaysAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox c in rightBays.Children)
            {
                c.IsChecked = true;
            }
        }

        private void RightBaysAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox c in rightBays.Children)
            {
                c.IsChecked = false;
            }
        }

        private void FrontBaysAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox c in frontBays.Children)
            {
                c.IsChecked = true;
            }
        }

        private void FrontBaysAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox c in frontBays.Children)
            {
                c.IsChecked = false;
            }
        }

        private void BackBaysAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox c in backBays.Children)
            {
                c.IsChecked = true;
            }
        }

        private void BackBaysAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox c in backBays.Children)
            {
                c.IsChecked = false;
            }
        }
    }
}
