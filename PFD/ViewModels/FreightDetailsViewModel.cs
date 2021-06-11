using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Globalization;
using BaseClasses;
using System.Collections.ObjectModel;
using DATABASE;
using DATABASE.DTO;
using System.Windows.Media;
using System.Windows;

namespace PFD
{
    [Serializable]
    public class FreightDetailsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_ProjectSite;
        private string m_Destination;
        private string m_Lat;
        private string m_Lng;
        private int m_MaxTruckLoad;
        private float m_MaxItemLength;
        private int m_MaxItemLengthBasic;
        private int m_MaxItemLengthOversize;
        private double m_TotalTransportedMass;
        private int m_NumberOfTrucks;
        private float m_RoadUnitPrice1;
        private float m_RoadUnitPrice2;
        private float m_FerryUnitPrice;
        private float m_TotalFreightCost;
        
        private List<RouteSegmentsViewModel> m_RouteSegments;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        
        public string Destination
        {
            get
            {
                return m_Destination;
            }

            set
            {
                m_Destination = value;
                NotifyPropertyChanged("Destination");
            }
        }

        public string Lat
        {
            get
            {
                return m_Lat;
            }

            set
            {
                m_Lat = value;
                NotifyPropertyChanged("Lat");
            }
        }

        public string Lng
        {
            get
            {
                return m_Lng;
            }

            set
            {
                m_Lng = value;
                NotifyPropertyChanged("Lng");
            }
        }

        public int MaxTruckLoad
        {
            get
            {
                return m_MaxTruckLoad;
            }

            set
            {
                m_MaxTruckLoad = value;
                if (m_MaxTruckLoad < 500 || m_MaxTruckLoad > 40000) throw new Exception($"Value out of range <500; 40000>");
                CalculateNumberOfTrucks();
                NotifyPropertyChanged("MaxTruckLoad");
            }
        }

        public int MaxItemLengthBasic
        {
            get
            {
                return m_MaxItemLengthBasic;
            }

            set
            {
                m_MaxItemLengthBasic = value;
                if (m_MaxItemLengthBasic < 1 || m_MaxItemLengthBasic > 40) throw new Exception($"Value out of range <1; 40>");
                NotifyPropertyChanged("MaxItemLengthBasic");
            }
        }

        public int MaxItemLengthOversize
        {
            get
            {
                return m_MaxItemLengthOversize;
            }

            set
            {
                m_MaxItemLengthOversize = value;
                if (m_MaxItemLengthOversize < m_MaxItemLengthBasic || m_MaxItemLengthOversize > 40) throw new Exception($"Value out of range <{m_MaxItemLengthBasic}; 40>");
                if(m_MaxItemLengthOversize < MaxItemLength) throw new Exception($"Minimal oversize length is {m_MaxItemLength} m.");
                NotifyPropertyChanged("MaxItemLengthOversize");
            }
        }

        public double TotalTransportedMass
        {
            get
            {
                return m_TotalTransportedMass;
            }

            set
            {
                m_TotalTransportedMass = value;                
                NotifyPropertyChanged("TotalTransportedMass");
            }
        }

        public int NumberOfTrucks
        {
            get
            {
                return m_NumberOfTrucks;
            }

            set
            {
                m_NumberOfTrucks = value;
                CountFreightCosts();
                NotifyPropertyChanged("NumberOfTrucks");
            }
        }

        public float RoadUnitPrice1
        {
            get
            {
                return m_RoadUnitPrice1;
            }

            set
            {
                m_RoadUnitPrice1 = value;
                if (m_RoadUnitPrice1 < 2 || m_RoadUnitPrice1 > 30) throw new Exception($"Value out of range <2; 30>");
                NotifyPropertyChanged("RoadUnitPrice1");
            }
        }

        public float RoadUnitPrice2
        {
            get
            {
                return m_RoadUnitPrice2;
            }

            set
            {
                m_RoadUnitPrice2 = value;
                if (m_RoadUnitPrice2 < 2 || m_RoadUnitPrice2 > 30) throw new Exception($"Value out of range <2; 30>");
                NotifyPropertyChanged("RoadUnitPrice2");
            }
        }

        public float FerryUnitPrice
        {
            get
            {
                return m_FerryUnitPrice;
            }

            set
            {
                m_FerryUnitPrice = value;
                if (m_FerryUnitPrice < 2 || m_FerryUnitPrice > 30) throw new Exception($"Value out of range <2; 30>");
                NotifyPropertyChanged("FerryUnitPrice");
            }
        }

        public float TotalFreightCost
        {
            get
            {
                return m_TotalFreightCost;
            }

            set
            {
                m_TotalFreightCost = value;
                NotifyPropertyChanged("TotalFreightCost");
            }
        }

        public List<RouteSegmentsViewModel> RouteSegments
        {
            get
            {
                return m_RouteSegments;
            }

            set
            {
                m_RouteSegments = value;
                SetRouteSegmentsPrices();
            }
        }

        public string ProjectSite
        {
            get
            {
                return m_ProjectSite;
            }

            set
            {
                m_ProjectSite = value;
            }
        }

        public float MaxItemLength
        {
            get
            {
                return m_MaxItemLength;
            }

            set
            {
                m_MaxItemLength = value;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FreightDetailsViewModel()
        { }

        public FreightDetailsViewModel(string projectSite, double buildingMass, List<RouteSegmentsViewModel> routeSegments, string destination, string lat, string lng, float maxItemLength)
        {
            ProjectSite = projectSite;

            MaxItemLength = maxItemLength;

            TotalTransportedMass = buildingMass;
            //defaults
            MaxTruckLoad = 20000; // 20 000 kg = 20 ton
            MaxItemLengthBasic = 18;
            MaxItemLengthOversize = 30;

            RoadUnitPrice1 = 6;
            RoadUnitPrice2 = 8;
            FerryUnitPrice = 10;

            Destination = destination;
            Lat = lat;
            Lng = lng;
            RouteSegments = routeSegments; //vypocitaju sa prices

            CountFreightCosts();
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetRouteSegmentsPrices()
        {
            if (RouteSegments == null) return;

            foreach (RouteSegmentsViewModel rs in RouteSegments)
            {
                if (rs.TransportType == "truck") rs.UnitPrice = (MaxItemLength > MaxItemLengthBasic ? m_RoadUnitPrice2 : m_RoadUnitPrice1);
                else if (rs.TransportType == "ferry") rs.UnitPrice = FerryUnitPrice;
                else throw new Exception("TransportType not recognized: {rs.TransportType}");
            }
        }
        

        public void SetViewModel(FreightDetailsViewModel vm)
        {
            if (vm == null) return;

            ProjectSite = vm.ProjectSite;
            Destination = vm.Destination;
            Lat = vm.Lat;
            Lng = vm.Lng;
            MaxItemLength = vm.MaxItemLength;
            MaxItemLengthBasic = vm.MaxItemLengthBasic;
            MaxItemLengthOversize = vm.MaxItemLengthOversize;
            TotalTransportedMass = vm.TotalTransportedMass;
            MaxTruckLoad = vm.MaxTruckLoad;
            NumberOfTrucks = vm.NumberOfTrucks;
            RoadUnitPrice1 = vm.RoadUnitPrice1;
            RoadUnitPrice2 = vm.RoadUnitPrice2;
            FerryUnitPrice = vm.FerryUnitPrice;

            RouteSegments = vm.RouteSegments;

            TotalFreightCost = vm.TotalFreightCost;
        }

        public void CalculateNumberOfTrucks()
        {
            NumberOfTrucks = (int)Math.Ceiling(TotalTransportedMass / MaxTruckLoad);
        }

        public float PricePerTruck()
        {
            float price = 0;
            if (RouteSegments == null) return price;
            if (RouteSegments.Count == 0) return price;

            foreach (RouteSegmentsViewModel rs in RouteSegments)
            {
                price += rs.Price;
            }
            return price;
        }

        public void CountFreightCosts()
        {
            TotalFreightCost = PricePerTruck() * NumberOfTrucks;
        }

        public void Update()
        {
            CalculateNumberOfTrucks();
            CountFreightCosts();
        }

        public void Validate()
        {
            if (MaxItemLength > MaxItemLengthOversize)
            {
                MessageBox.Show($"Minimal oversize length is {MaxItemLength} m. It will be changed automatically.");
                MaxItemLengthOversize = (int)Math.Ceiling(MaxItemLength);
            } 
        }
    }
}
