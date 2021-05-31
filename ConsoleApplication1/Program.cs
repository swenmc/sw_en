using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
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

        static void Main(string[] args)
        {
            //CCNCPathFinder finder = new CCNCPathFinder();
            RunAsync().GetAwaiter().GetResult();

        }

        static HttpClient client = new HttpClient();
        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://geocode.search.hereapi.com/v1/geocode");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {

                // Get the product
                //HttpResponseMessage msg = await GetResponseAsync("https://geocode.search.hereapi.com/v1/geocode?apiKey=7IG_k7xRWzWLgFG2eLDoGcu9yo-49DCPFBj1tu-aqfA&q=Lubotin,Slovensko");

                //MessageBox.Show(msg.ToString());
                GeoResponse msg2 = await GetGeoResponseAsync("?apiKey=7IG_k7xRWzWLgFG2eLDoGcu9yo-49DCPFBj1tu-aqfA&q=dom svatej alzbety,Slovensko");
                Console.WriteLine(msg2.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static async Task<GeoResponse> GetGeoResponseAsync(string path)
        {
            GeoResponse res = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                res = await response.Content.ReadAsAsync<GeoResponse>();
            }
            return res;
        }

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
