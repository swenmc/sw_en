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
    public partial class GeoLocationInfo : Window
    {
        static string m_projectSite;
        static GeoResponse m_data;
        static HttpClient client;
        public GeoLocationInfo(string projectSite)
        {
            InitializeComponent();

            m_projectSite = projectSite;

            try
            {
                client = new HttpClient();
                RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                ShowGeoResponseData(m_data.items.FirstOrDefault());

                client.Dispose();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://geocode.search.hereapi.com/v1/geocode");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                m_data = await GetGeoResponseAsync($"?apiKey=7IG_k7xRWzWLgFG2eLDoGcu9yo-49DCPFBj1tu-aqfA&q={m_projectSite}").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void ShowGeoResponseData(ResponseItem data)
        {
            if (data == null) return;

            TextBox_Title.Text = data.title;
            TextBox_resultType.Text = data.resultType;
            TextBox_houseNumberType.Text = data.houseNumberType;
            TextBox_addressBlockType.Text = data.addressBlockType;
            TextBox_localityType.Text = data.localityType;
            TextBox_administrativeAreaType.Text = data.administrativeAreaType;

            TextBox_label.Text = data.address.label;
            TextBox_countryCode.Text = data.address.countryCode;
            TextBox_countryName.Text = data.address.countryName;
            TextBox_stateCode.Text = data.address.stateCode;
            TextBox_state.Text = data.address.state;            
            TextBox_county.Text = data.address.county;
            TextBox_city.Text = data.address.city;
            TextBox_district.Text = data.address.district;
            TextBox_subdistrict.Text = data.address.subdistrict;
            TextBox_street.Text = data.address.street;
            TextBox_block.Text = data.address.block;
            TextBox_subblock.Text = data.address.subblock;
            TextBox_postalCode.Text = data.address.postalCode;
            TextBox_houseNumber.Text = data.address.houseNumber;
            TextBox_lat.Text = data.position.lat;
            TextBox_lng.Text = data.position.lng;
        }

        static async Task<GeoResponse> GetGeoResponseAsync(string path)
        {
            GeoResponse res = null;
            HttpResponseMessage response = await client.GetAsync(path).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                res = await response.Content.ReadAsAsync<GeoResponse>().ConfigureAwait(false);
            }
            return res;
        }


    }

    public class ResponseItem
    {
        public string title { get; set; }
        public string id { get; set; }
        public string resultType { get; set; }
        public string houseNumberType { get; set; }
        public string addressBlockType { get; set; }
        public string localityType { get; set; }
        public string administrativeAreaType { get; set; }
        public Address address { get; set; }
        public Position position { get; set; }
    }

    public class Position
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class Address
    {
        public string label { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string stateCode { get; set; }
        public string state { get; set; }
        public string countyCode { get; set; }
        public string county { get; set; }

        public string city { get; set; }
        public string district { get; set; }
        public string subdistrict { get; set; }
        public string street { get; set; }
        public string block { get; set; }
        public string subblock { get; set; }
        public string postalCode { get; set; }
        public string houseNumber { get; set; }
    }

    public class GeoResponse
    {
        public ResponseItem[] items { get; set; }
    }
}
