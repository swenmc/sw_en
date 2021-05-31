using BaseClasses;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// Interaction logic for ProjectInfo.xaml
    /// </summary>
    public partial class ProjectInfo : Window
    {
        public ProjectInfo(CProjectInfoVM vm)
        {
            InitializeComponent();

            vm.PropertyChanged += HandleProjectInfoPropertyChangedEvent;
            this.DataContext = vm;
        }

        protected void HandleProjectInfoPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            //CProjectInfoVM vm = sender as CProjectInfoVM;


        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSaveProjectInfo_Click(object sender, RoutedEventArgs e)
        {
            CProjectInfoVM vm = this.DataContext as CProjectInfoVM;
            CProjectInfo pi = vm.GetProjectInfo();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Data Files (*.eiup)|*.eiup";
            sfd.DefaultExt = "eiup";
            sfd.AddExtension = true;
            sfd.FileName = pi.ProjectName;

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = File.Open(sfd.FileName, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, pi);
                    stream.Close();
                }
            }
        }

        private void BtnLoadProjectInfo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Data Files (*.eiup)|*.eiup";
            ofd.DefaultExt = "eiup";
            ofd.AddExtension = true;

            if (ofd.ShowDialog() == true)
            {
                OpenFile(ofd.FileName);
            }
        }

        private void OpenFile(string fileName)
        {
            CProjectInfo deserializedProjectInfo = null;

            using (Stream stream = File.Open(fileName, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                deserializedProjectInfo = (CProjectInfo)binaryFormatter.Deserialize(stream);
            }

            CProjectInfoVM vm = this.DataContext as CProjectInfoVM;
            if (deserializedProjectInfo != null)
            {
                vm.SetViewModel(deserializedProjectInfo);
            }
        }

        private void BtnDistance_Click(object sender, RoutedEventArgs e)
        {
            CProjectInfoVM vm = this.DataContext as CProjectInfoVM;
            if (string.IsNullOrEmpty(vm.Site)) { MessageBox.Show("Address is empty!"); return; }

            GeoLocationInfo gli = new GeoLocationInfo(vm.Site);
            gli.ShowDialog();
        }

        private void BtnShowOnMap_Click(object sender, RoutedEventArgs e)
        {
            CProjectInfoVM vm = this.DataContext as CProjectInfoVM;
            System.Diagnostics.Process.Start($"https://www.google.sk/maps/place/{vm.Site}");
        }
    }
}
