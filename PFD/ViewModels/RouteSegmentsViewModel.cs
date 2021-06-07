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
    public class RouteSegmentsViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_ID;
        private string m_TransportType;
        private int m_Distance;
        private string m_Time;
        private float m_UnitPrice;
        private float m_Price;
        
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public string ID
        {
            get
            {
                return m_ID;
            }

            set
            {
                m_ID = value;
                NotifyPropertyChanged("ID");
            }
        }

        public string TransportType
        {
            get
            {
                return m_TransportType;
            }

            set
            {
                m_TransportType = value;
                NotifyPropertyChanged("TransportType");
            }
        }

        public int Distance
        {
            get
            {
                return m_Distance;
            }

            set
            {
                m_Distance = value;                
                NotifyPropertyChanged("Distance");
            }
        }

        public string Time
        {
            get
            {
                return m_Time;
            }

            set
            {
                m_Time = value;
                NotifyPropertyChanged("Time");
            }
        }

        public float UnitPrice
        {
            get
            {
                return m_UnitPrice;
            }

            set
            {
                m_UnitPrice = value;
                UpdatePrice();
                NotifyPropertyChanged("UnitPrice");
            }
        }

        public float Price
        {
            get
            {
                return m_Price;
            }

            set
            {
                m_Price = value;
                NotifyPropertyChanged("Price");
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public RouteSegmentsViewModel()
        {
            
        }

        public RouteSegmentsViewModel(string id, string transportType, int distance, string duration)
        {
            ID = id;
            TransportType = transportType;
            Distance = distance;
            Time = duration;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        

        public void SetViewModel(RouteSegmentsViewModel vm)
        {
            if (vm == null) return;

            ID = vm.ID;
            TransportType = vm.TransportType;
            Distance = vm.Distance;
            Time = vm.Time;
            UnitPrice = vm.UnitPrice;
            Price = vm.Price;
        }

        private void UpdatePrice()
        {
            Price = UnitPrice * Distance;
        }
    }
}
