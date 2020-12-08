using BaseClasses;
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
        CModel_PFD model;
        sDesignResults designResults_ULSandSLS;
        sDesignResults designResults_ULS;
        sDesignResults designResults_SLS;

        public DesignResultsSummary(CModel_PFD model_pfd, sDesignResults sDesignResults_ULSandSLS, sDesignResults sDesignResults_ULS, sDesignResults sDesignResults_SLS)
        {
            InitializeComponent();

            model = model_pfd;
            designResults_ULSandSLS = sDesignResults_ULSandSLS;
            designResults_ULS = sDesignResults_ULS;
            designResults_SLS = sDesignResults_SLS;


            MessageBox.Show("MaximumDesignRatio: " + designResults_ULSandSLS.DesignResults[EMemberType_FS_Position.MainColumn].MaximumDesignRatio);
            //txtBoxDesignSummary.Text = text;
        }
    }
}
