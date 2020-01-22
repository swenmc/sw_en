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
    public partial class WindowsGeneratorWindow : Window
    {
        List<int> lefRightBays;
        List<int> frontBackBays;
        WindowGeneratorViewModel vm;

        float fWallHeight;
        float fL1;
        float fColumnsDistance;        

        public WindowsGeneratorWindow(int lrBaysNum, int fbBaysNum, float wallHeight, float L1, float columnsDistance)
        {
            InitializeComponent();

            fWallHeight = wallHeight;
            fL1 = L1;
            fColumnsDistance = columnsDistance;

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

            vm = new WindowGeneratorViewModel();
            vm.PropertyChanged += HandleWindowGeneratorPropertyChanged;
            this.DataContext = vm;
        }

        private void HandleWindowGeneratorPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
        }


        public List<WindowProperties> GetWindowsProperties()
        {
            List<WindowProperties> windowsProperties = new List<WindowProperties>();
            if (vm.AddWindows == false) return windowsProperties;


            for (int i = 1; i <= lefRightBays.Count; i++)
            {
                if ((leftBays.Children[i] as CheckBox).IsChecked == true)
                {
                    WindowProperties wp = GetModelWindowProperties("Left");
                    wp.Bays = lefRightBays;
                    wp.iBayNumber = i;                    
                    windowsProperties.Add(wp);
                }
                if ((rightBays.Children[i] as CheckBox).IsChecked == true)
                {
                    WindowProperties wp = GetModelWindowProperties("Right");
                    wp.Bays = lefRightBays;
                    wp.iBayNumber = i;                    
                    windowsProperties.Add(wp);
                }
            }

            for (int i = 1; i <= frontBackBays.Count; i++)
            {
                if ((frontBays.Children[i] as CheckBox).IsChecked == true)
                {
                    WindowProperties wp = GetModelWindowProperties("Front");
                    wp.Bays = frontBackBays;
                    wp.iBayNumber = i;                    
                    windowsProperties.Add(wp);
                }
                if ((backBays.Children[i] as CheckBox).IsChecked == true)
                {
                    WindowProperties wp = GetModelWindowProperties("Back");
                    wp.Bays = frontBackBays;
                    wp.iBayNumber = i;                    
                    windowsProperties.Add(wp);
                }
            }

            return windowsProperties;
        }

        public List<WindowProperties> GetWindowsToDelete()
        {
            List<WindowProperties> windowProperties = new List<WindowProperties>();
            if (vm.DeleteWindows == false) return windowProperties;

            for (int i = 1; i <= lefRightBays.Count; i++)
            {
                if ((leftBays.Children[i] as CheckBox).IsChecked == true)
                {
                    WindowProperties wp = new WindowProperties();
                    wp.Bays = lefRightBays;
                    wp.iBayNumber = i;
                    wp.sBuildingSide = "Left";
                    windowProperties.Add(wp);
                }
                if ((rightBays.Children[i] as CheckBox).IsChecked == true)
                {
                    WindowProperties wp = new WindowProperties();
                    wp.Bays = lefRightBays;
                    wp.iBayNumber = i;
                    wp.sBuildingSide = "Right";
                    windowProperties.Add(wp);
                }
            }

            for (int i = 1; i <= frontBackBays.Count; i++)
            {
                if ((frontBays.Children[i] as CheckBox).IsChecked == true)
                {
                    WindowProperties wp = new WindowProperties();
                    wp.Bays = frontBackBays;
                    wp.iBayNumber = i;
                    wp.sBuildingSide = "Front";
                    windowProperties.Add(wp);
                }
                if ((backBays.Children[i] as CheckBox).IsChecked == true)
                {
                    WindowProperties wp = new WindowProperties();
                    wp.Bays = frontBackBays;
                    wp.iBayNumber = i;
                    wp.sBuildingSide = "Back";
                    windowProperties.Add(wp);
                }
            }

            return windowProperties;
        }

        private WindowProperties GetModelWindowProperties(string buildingSide)
        {
            WindowProperties wp = new WindowProperties();
            //wp.SetValidationValues(fWallHeight, fL1, fColumnsDistance, fColumnsDistance); //bug fix 500 - zmazal som,aby sa validovalo na inom mieste

            wp.sBuildingSide = buildingSide;
            wp.fWindowsHeight = vm.WindowHeight;
            wp.fWindowsWidth = vm.WindowWidth;
            wp.fWindowCoordinateXinBay = vm.WindowCoordinateXinBay;
            wp.fWindowCoordinateZinBay = vm.WindowCoordinateZinBay;
            wp.iNumberOfWindowColumns = vm.NumberOfWindowColumns;
            return wp;
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            vm.AddWindows = true;
            this.Close();
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            vm.DeleteWindows = true;
            this.Close();
        }
    }
}
