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
    /// Interaction logic for DesignResultsSummary.xaml
    /// </summary>
    public partial class DesignResultsSummary : Window
    {
        public DesignResultsSummary(string text)
        {
            InitializeComponent();

            txtBoxDesignSummary.Text = text;
        }
    }
}
