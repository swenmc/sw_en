using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;

namespace PFD.Infrastructure
{
    public class GeoLocation
    {
        static string m_projectSite;
        static string m_lat;
        static string m_lng;

        static GeoResponse m_data;
        static RoutingResponse m_routing;

        static HttpClient client;
        static HttpClient clientRouting;

        private FreightDetailsViewModel freightDetails;

        public GeoResponse Data
        {
            get
            {
                return m_data;
            }

            set
            {
                m_data = value;
            }
        }

        public RoutingResponse Routing
        {
            get
            {
                return m_routing;
            }

            set
            {
                m_routing = value;
            }
        }

        public GeoLocation(string projectSite)
        {
            m_projectSite = projectSite;

            if (string.IsNullOrEmpty(projectSite)) { MessageBox.Show("Address is empty!"); return; }

            try
            {
                client = new HttpClient();

                RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                if (Data.items.FirstOrDefault() != null)
                {
                    m_lat = Data.items.FirstOrDefault().position.lat;
                    m_lng = Data.items.FirstOrDefault().position.lng;
                    client.Dispose();
                }
                else
                {
                    client.Dispose();
                    return;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            try
            {
                clientRouting = new HttpClient();

                RunAsyncRouting().ConfigureAwait(false).GetAwaiter().GetResult();

                clientRouting.Dispose();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        
        public List<RouteSegmentsViewModel> GetRouteSegments()
        {
            List<RouteSegmentsViewModel>  routeSegments = new List<RouteSegmentsViewModel>();
            
            if (Routing != null)
            {
                RouteSegmentsViewModel routeSegment = null;

                Route route = Routing.routes.FirstOrDefault();
                if (route == null) return routeSegments;

                int i = 1;
                foreach (RouteSection s in route.sections)
                {
                    routeSegment = new RouteSegmentsViewModel(i.ToString(), GetRouteTransport(s), GetRouteLength(s), GetRouteDuration(s));
                    routeSegments.Add(routeSegment);
                    i++;
                }
            }
            return routeSegments;
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
            List<string> transports = new List<string>();
            foreach (RouteSection s in route.sections)
            {
                transports.Add(s.transport.mode);
            }

            return string.Join(" - ", transports);
        }

        private int GetRouteDuration(RouteSection section)
        {
            return int.Parse(section.summary.duration);
        }

        //To Mato - moze to tak byt,ze zaokruhlime na cele km? ci radsej dame Ceiling? alebo budeme pouzivat desatinne aj pre km?
        private int GetRouteLength(RouteSection section)
        {
            int length = int.Parse(section.summary.length);
            
            return (int)Math.Round(length / 1000.0);
        }
        private string GetRouteTransport(RouteSection section)
        {
            return section.transport.mode;            
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

    public class RoutingResponse
    {
        public Route[] routes { get; set; }
    }

    public class Route
    {
        public string id { get; set; }
        public RouteSection[] sections { get; set; }
    }

    public class RouteSection
    {
        public string type { get; set; }
        public RouteSummary summary { get; set; }
        public Transport transport { get; set; }

    }
    public class RouteSummary
    {
        public string duration { get; set; }
        public string length { get; set; }
        public string baseDuration { get; set; }
    }
    public class Transport
    {
        public string name { get; set; }
        public string mode { get; set; }
    }
}
