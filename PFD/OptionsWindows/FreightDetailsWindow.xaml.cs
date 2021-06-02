using BaseClasses;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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
    public partial class FreightDetailsWindow : Window
    {
        static string m_projectSite;
        static GeoResponse m_data;
        static string m_lat;
        static string m_lng;

        static RoutingResponse m_routing;
        static HttpClient client;
        static HttpClient clientRouting;
        public FreightDetailsWindow()
        {
            InitializeComponent();

            //m_projectSite = projectSite;

            //if (string.IsNullOrEmpty(projectSite)) { MessageBox.Show("Address is empty!"); return; }

            //try
            //{
            //    client = new HttpClient();
            //    RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //    ShowGeoResponseData(m_data.items.FirstOrDefault());

            //    client.Dispose();
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }

            //try
            //{
            //    clientRouting = new HttpClient();
            //    RunAsyncRouting().ConfigureAwait(false).GetAwaiter().GetResult();

            //    ShowRoutingResponseData(m_routing);

            //    clientRouting.Dispose();
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
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

            m_lat = data.position.lat;
            m_lng = data.position.lng;

            //webBrowser.Source = new Uri($"https://www.google.sk/maps/place/{data.address.label}");
            //dynamic activeX = this.webBrowser.GetType().InvokeMember("ActiveXInstance",
            //        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            //        null, this.webBrowser, new object[] { });
            //activeX.Silent = true;
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

        static async Task RunAsyncRouting()
        {
            // Update port # in the following line.
            clientRouting.BaseAddress = new Uri("https://router.hereapi.com/v8/routes");
            clientRouting.DefaultRequestHeaders.Accept.Clear();
            clientRouting.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            //NumberFormatInfo nfi = new NumberFormatInfo();
            //nfi.NumberDecimalSeparator = ".";
            //string transportMode = "car";
            string transportMode = "truck";
            try
            {
                string url = $"?apiKey=7IG_k7xRWzWLgFG2eLDoGcu9yo-49DCPFBj1tu-aqfA&transportMode={transportMode}&origin=-36.979182055684475,174.82199048707665&destination={m_lat},{m_lng}&return=summary";
                m_routing = await GetRoutingResponseAsync(url).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static async Task<RoutingResponse> GetRoutingResponseAsync(string path)
        {
            RoutingResponse res = null;
            HttpResponseMessage response = await clientRouting.GetAsync(path).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                res = await response.Content.ReadAsAsync<RoutingResponse>().ConfigureAwait(false);
            }
            return res;
        }

        private void ShowRoutingResponseData(RoutingResponse data)
        {
            if (data == null) { TextBox_routesFound.Text = $"Number of routes found: {0}"; return; }
            if (data.routes == null) { TextBox_routesFound.Text = $"Number of routes found: {0}"; return; }

            TextBox_routesFound.Text = $"Number of routes found: {data.routes.Length}";

            Route route = data.routes.FirstOrDefault();
            if (route == null) return;

            TextBox_routesFound.Text += $", Number of sections: {route.sections.Length}";

            TextBox_Duration.Text = GetRouteDuration(route);
            TextBox_Length.Text = GetRouteLength(route);
            TextBox_Transport.Text = GetRouteTransport(route);
        }

        private string GetRouteDuration(Route route)
        {
            int duration = 0;
            foreach (RouteSection s in route.sections)
            {
                duration += int.Parse(s.summary.duration);
            }

            int hours = duration / 3600;
            int min = (duration % 3600) / 60;
            return $"{hours} h {min} min.";
        }
        private string GetRouteLength(Route route)
        {
            int length = 0;
            foreach (RouteSection s in route.sections)
            {
                length += int.Parse(s.summary.length);
            }

            return $"{(int)Math.Round(length / 1000.0)} km";
        }
        private string GetRouteTransport(Route route)
        {
            List<string> transports =  new List<string>();
            foreach (RouteSection s in route.sections)
            {
                transports.Add(s.transport.mode);                
            }

            return string.Join(" - ", transports);            
        }

        private void BtnShowOnMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start($"https://www.google.sk/maps/place/{m_data.items.FirstOrDefault().address.label}");
            }
            catch (Exception) { /* Do Nothing */ }
        }

        private void BtnShowRoute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start($"https://www.google.com/maps/dir/4-6+Waokauri+Place,+Māngere,+Auckland+2022,+Nový+Zéland/{m_data.items.FirstOrDefault().address.label}");
            }
            catch (Exception) { /* Do Nothing */ }
        }
    }

    //public class ResponseItem
    //{
    //    public string title { get; set; }
    //    public string id { get; set; }
    //    public string resultType { get; set; }
    //    public string houseNumberType { get; set; }
    //    public string addressBlockType { get; set; }
    //    public string localityType { get; set; }
    //    public string administrativeAreaType { get; set; }
    //    public Address address { get; set; }
    //    public Position position { get; set; }
    //}

    //public class Position
    //{
    //    public string lat { get; set; }
    //    public string lng { get; set; }
    //}

    //public class Address
    //{
    //    public string label { get; set; }
    //    public string countryCode { get; set; }
    //    public string countryName { get; set; }
    //    public string stateCode { get; set; }
    //    public string state { get; set; }
    //    public string countyCode { get; set; }
    //    public string county { get; set; }

    //    public string city { get; set; }
    //    public string district { get; set; }
    //    public string subdistrict { get; set; }
    //    public string street { get; set; }
    //    public string block { get; set; }
    //    public string subblock { get; set; }
    //    public string postalCode { get; set; }
    //    public string houseNumber { get; set; }
    //}

    //public class GeoResponse
    //{
    //    public ResponseItem[] items { get; set; }
    //}



    //public class RoutingResponse
    //{
    //    public Route[] routes { get; set; }
    //}

    //public class Route
    //{
    //    public string id { get; set; }
    //    public RouteSection[] sections { get; set; }
    //}

    //public class RouteSection
    //{
    //    public string type { get; set; }
    //    public RouteSummary summary { get; set; }
    //    public Transport transport { get; set; }

    //}
    //public class RouteSummary
    //{
    //    public string duration { get; set; }
    //    public string length { get; set; }
    //    public string baseDuration { get; set; }
    //}
    //public class Transport
    //{
    //    public string name { get; set; }
    //    public string mode { get; set; }        
    //}
}
