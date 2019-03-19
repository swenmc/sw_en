using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;

namespace PFD
{
    /// <summary>
    /// Interaction logic for Solver.xaml
    /// </summary>
    public partial class Solver : Window
    {
        public double Progress = 0;
        Stopwatch stopWatch = new Stopwatch();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        bool UseFEMSolverCalculationForSimpleBeam;
        string sResultsSummaryText;

        public Solver(bool bUseFEMSolverCalculationForSimpleBeam)
        {
            InitializeComponent();
            Progress = 0;
            DisplayCalculationTime();
            UseFEMSolverCalculationForSimpleBeam = bUseFEMSolverCalculationForSimpleBeam;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DesignResultsSummary designSummaryWindow = new DesignResultsSummary(sResultsSummaryText);
            designSummaryWindow.Show();

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
                if(!UseFEMSolverCalculationForSimpleBeam)
                    SetInActiveRowFormat(LabelFrames, LabelFramesCounting);

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

        public void SetSumaryFinished(string sResultsSummaryTextAll)
        {
            Dispatcher.Invoke(() =>
            {
                SetInActiveRowFormat(LabelMemberDesignLoadCombination, LabelMemberDesignLoadCombinationProgress);

                LabelSummaryState.Text = "Calculation successful.";
                LabelSummaryState.Foreground = Brushes.Black;
                //LabelSummaryState.FontWeight = FontWeights.Bold;

                if (stopWatch.IsRunning)
                {
                    stopWatch.Stop();
                }

                sResultsSummaryText = sResultsSummaryTextAll; // Set output window text
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

        public void DisplayCalculationTime()
        {
            dispatcherTimer.Tick += new EventHandler(dt_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);

            stopWatch.Start();
            dispatcherTimer.Start();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                string currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                LabelTimer.Text = currentTime;
            }
        }

        public void ShowMessageBox(string text)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(text);
            });
        }
    }
}
