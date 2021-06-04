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
        private string m_Distance;
        private string m_Time;
        private string m_UnitPrice;
        private string m_Price;
        
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

        public string Distance
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

        public string UnitPrice
        {
            get
            {
                return m_UnitPrice;
            }

            set
            {
                m_UnitPrice = value;
                NotifyPropertyChanged("UnitPrice");
            }
        }

        public string Price
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
    }
}
