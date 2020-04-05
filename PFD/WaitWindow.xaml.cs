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

namespace PFD
{
    /// <summary>
    /// Interaction logic for WaitWindow.xaml
    /// </summary>
    public partial class WaitWindow : Window
    {
        public WaitWindow(string fileType)
        {
            InitializeComponent();

            // Set icon depending on type of exported file
            if (fileType == "DOC")
                //WaitImage.Source = new BitmapImage(new Uri("./Resources/DOCfilelogo.png", UriKind.Relative));
                WaitImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/DOCfilelogo.png"));
            else if (fileType == "XLS")
                //WaitImage.Source = new BitmapImage(new Uri("./Resources/XLSfilelogo.png", UriKind.Relative));
                WaitImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/XLSfilelogo.png"));
            else if (fileType == "PDF")
                //WaitImage.Source = new BitmapImage(new Uri("./Resources/PDFfilelogo.png", UriKind.Relative));
                WaitImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/PDFfilelogo.png"));
            else if (fileType == "CNC")
                //WaitImage.Source = new BitmapImage(new Uri("./Resources/CNCfilelogo.png", UriKind.Relative));
                WaitImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/CNCfilelogo.png"));
            else
            {
                throw new Exception("Not defined file type icon.");
                // Not defined icon
            }
        }
    }
}
