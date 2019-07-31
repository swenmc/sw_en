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

namespace CoverterExcelToPdf
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
                WaitImage.Source = new BitmapImage(new Uri("Resources/DOCfilelogo.png", UriKind.Relative));
            else if (fileType == "XLS")
                WaitImage.Source = new BitmapImage(new Uri("Resources/XLSfilelogo.png", UriKind.Relative));
            else if (fileType == "PDF")
                WaitImage.Source = new BitmapImage(new Uri("Resources/PDFfilelogo.png", UriKind.Relative));
            else
            {
                throw new Exception("Not defined file type icon.");
                // Not defined icon
            }
        }
    }
}
