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

namespace PFD
{
    [Serializable]
    public class FreightDetailsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_Destination;
        private string m_Lat;
        private string m_Lng;
        private int m_MaxTruckLoad;
        private int m_MaxItemLengthBasic;
        private int m_MaxItemLengthOversize;
        private int m_TotalTransportedMass;
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
                NotifyPropertyChanged("MaxItemLengthOversize");
            }
        }

        public int TotalTransportedMass
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
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public FreightDetailsViewModel()
        {
            
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        

        public void SetViewModel(FreightDetailsViewModel vm)
        {
            if (vm == null) return;

            Destination = vm.Destination;
            Lat = vm.Lat;
            Lng = vm.Lng;
            MaxTruckLoad = vm.MaxTruckLoad;
            MaxItemLengthBasic = vm.MaxItemLengthBasic;
            MaxItemLengthOversize = vm.MaxItemLengthOversize;
            TotalTransportedMass = vm.TotalTransportedMass;
            NumberOfTrucks = vm.NumberOfTrucks;
            RoadUnitPrice1 = vm.RoadUnitPrice1;
            RoadUnitPrice2 = vm.RoadUnitPrice2;
            FerryUnitPrice = vm.FerryUnitPrice;
            TotalFreightCost = vm.TotalFreightCost;
        }        
    }
}
