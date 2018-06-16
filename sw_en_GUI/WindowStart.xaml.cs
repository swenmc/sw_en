using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for WindowStart.xaml
    /// </summary>
    public partial class WindowStart : Window
    {
        public WindowStart()
        {
            InitializeComponent();
            image1.Source = (ImageSource)TryFindResource("XYZ");
            imgFiles.Source = (ImageSource)TryFindResource("files");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
