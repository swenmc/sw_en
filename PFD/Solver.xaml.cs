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

        public void SetCountsLabels(int nodesCount, int membersCount, int jointsCount, int loadCasesCount, int loadCombCount)
        {
            Dispatcher.Invoke(() =>
            {
                LabelNodesCount.Text = nodesCount.ToString();
                LabelMembersCount.Text = membersCount.ToString();
                LabelJointsCount.Text = jointsCount.ToString();
                LabelLoadCasesCount.Text = loadCasesCount.ToString();
                LabelLoadCombCount.Text = loadCombCount.ToString();
            });
        }

        public void SetInputData()
        {
            Dispatcher.Invoke(() =>
            {
                LabelInputData.Text = "Input Data Processed";
                SetActiveRowFormat(LabelInputData);
            });
        }
        
        public void SetFrames()
        {
            Dispatcher.Invoke(() =>
            {
                SetInActiveRowFormat(LabelInputData);
                SetActiveRowFormat(LabelFrames, LabelFramesCounting);
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
                SetInActiveRowFormat(LabelFrames, LabelFramesCounting);
                SetActiveRowFormat(LabelBeams, LabelBeamsCounting);
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
                SetInActiveRowFormat(LabelBeams, LabelBeamsCounting);
                SetActiveRowFormat(LabelDeterminateDesignInternalForces, LabelDeterminateDesignInternalForcesProgress);
            });
        }

        public void SetMemberDesignLoadCaseProgress(int actual, int total)
        {
            Dispatcher.Invoke(() =>
            {
                LabelDeterminateDesignInternalForcesProgress.Text = $"Member: {actual}/{total}";
            });
        }

        public void SetMemberDesignLoadCombination()
        {
            Dispatcher.Invoke(() =>
            {
                SetInActiveRowFormat(LabelDeterminateDesignInternalForces, LabelDeterminateDesignInternalForcesProgress);
                SetActiveRowFormat(LabelMemberDesignLoadCombination, LabelMemberDesignLoadCombinationProgress);
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
                SetInActiveRowFormat(LabelMemberDesignLoadCombination, LabelMemberDesignLoadCombinationProgress);

                LabelSummaryState.Text = "Calculation successful.";
                LabelSummaryState.Foreground = Brushes.Black;
                //LabelSummaryState.FontWeight = FontWeights.Bold;

                BtnOK.IsEnabled = true;
            });
        }

        private void SetActiveRowFormat(TextBlock label1, TextBlock label2 = null)
        {
            Color rgbColor = new Color();
            rgbColor = Color.FromRgb(23, 102, 156);

            label1.FontWeight = FontWeights.Bold;
            label1.FontSize = 12.5;
            label1.Foreground = new SolidColorBrush(rgbColor);

            if (label2 != null)
            {
                label2.FontWeight = FontWeights.Bold;
                label2.FontSize = 12.5;
                label2.Foreground = new SolidColorBrush(rgbColor);
            }
        }

        private void SetInActiveRowFormat(TextBlock label1, TextBlock label2 = null)
        {
            label1.FontWeight = FontWeights.Normal;
            label1.FontSize = 12;
            label1.Foreground = new SolidColorBrush(Colors.Black);

            if (label2 != null)
            {
                label2.FontWeight = FontWeights.Normal;
                label2.FontSize = 12;
                label2.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
