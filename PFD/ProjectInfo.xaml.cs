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
            RunAsync().GetAwaiter().GetResult();

        }


        static HttpClient client = new HttpClient();
        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://geocode.search.hereapi.com/v1/geocode");
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {

                // Get the product
                //HttpResponseMessage msg = await GetResponseAsync("https://geocode.search.hereapi.com/v1/geocode?apiKey=7IG_k7xRWzWLgFG2eLDoGcu9yo-49DCPFBj1tu-aqfA&q=Lubotin,Slovensko");

                //MessageBox.Show(msg.ToString());
                HttpResponseMessage msg2 = await GetResponseAsync("?apiKey=7IG_k7xRWzWLgFG2eLDoGcu9yo-49DCPFBj1tu-aqfA&q=Lubotin,Slovensko");
                MessageBox.Show(msg2.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        //static async Task<Product> GetProductAsync(string path)
        //{
        //    Product product = null;
        //    HttpResponseMessage response = await client.GetAsync(path);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        product = await response.Content.ReadAsAsync<Product>();
        //    }
        //    return product;
        //}

        static async Task<HttpResponseMessage> GetResponseAsync(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            //if (response.IsSuccessStatusCode)
            //{
            //    product = await response.Content.ReadAsAsync<Product>();
            //}
            return response;
        }


    }
}
