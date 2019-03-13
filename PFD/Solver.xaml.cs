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
    /// Interaction logic for Solver.xaml
    /// </summary>
    public partial class Solver : Window
    {
        public double Progress = 0;
        public Solver()
        {
            InitializeComponent();
            Progress = 0;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //public void UpdateProgressBarValue(double progressValue)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        SolverProgressBar.Value = progressValue;                
        //    });
        //}
        public void UpdateProgress()
        {
            Dispatcher.Invoke(() =>
            {
                SolverProgressBar.Value = Progress;
            });
        }

        public void SetCountsLabels(int nodesCount, int membersCount, int jointsCount, int loadCombCount)
        {
            Dispatcher.Invoke(() =>
            {
                LabelNodesCount.Text = nodesCount.ToString();
                LabelMembersCount.Text = membersCount.ToString();
                LabelJointsCount.Text = jointsCount.ToString();
                LabelLoadCombCount.Text = loadCombCount.ToString();
            });
        }

        public void SetInputData()
        {
            Dispatcher.Invoke(() =>
            {
                LabelInputData.Text = "Input Data Processed";
                LabelInputData.FontWeight = FontWeights.Bold;                
            });
        }
        
        public void SetFrames()
        {
            Dispatcher.Invoke(() =>
            {
                LabelFrames.FontWeight = FontWeights.Bold;                
            });
        }
        public void SetFramesProgress(int actual, int total)
        {
            Dispatcher.Invoke(() =>
            {
                LabelFramesCounting.Text = $"Frame: {actual}/{total}";                
            });
        }
        public void SetBeams()
        {
            Dispatcher.Invoke(() =>
            {
                LabelBeams.FontWeight = FontWeights.Bold;                
            });
        }
        public void SetBeamsProgress(int actual, int total)
        {
            Dispatcher.Invoke(() =>
            {
                LabelBeamsCounting.Text = $"Beam: {actual}/{total}";
            });
        }

        public void SetMemberDesignLoadCase()
        {
            Dispatcher.Invoke(() =>
            {
                LabelMemberDesignLoadCase.FontWeight = FontWeights.Bold;
            });
        }
        public void SetMemberDesignLoadCaseProgress(int actual, int total)
        {
            Dispatcher.Invoke(() =>
            {
                LabelMemberDesignLoadCaseProgress.Text = $"Member: {actual}/{total}";
            });
        }
        public void SetMemberDesignLoadCombination()
        {
            Dispatcher.Invoke(() =>
            {
                LabelMemberDesignLoadCombination.FontWeight = FontWeights.Bold;
            });
        }
        public void SetMemberDesignLoadCombinationProgress(int actual, int total)
        {
            Dispatcher.Invoke(() =>
            {
                LabelMemberDesignLoadCombinationProgress.Text = $"Member: {actual}/{total}";
            });
        }

        public void SetSumaryFinished()
        {
            Dispatcher.Invoke(() =>
            {
                LabelSummaryState.Text = "Finished.";
                LabelSummaryState.Foreground = Brushes.Green;
                LabelSummaryState.FontWeight = FontWeights.Bold;
            });
        }
        


    }
}
