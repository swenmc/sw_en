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
    public class CProjectInfoVM : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_ProjectName;
        private string m_Site;
        private string m_ProjectNumber;
        private string m_ProjectPart;
        private DateTime m_Date;
        private string m_CustomerName;
        private string m_CustomerContactPerson;

        private string m_SalesPerson;
        private string m_SalesPersonPhone;
        private string m_SalesPersonEmail;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public string ProjectName
        {
            get
            {
                return m_ProjectName;
            }

            set
            {
                m_ProjectName = value;
                NotifyPropertyChanged("ProjectName");
            }
        }

        public string Site
        {
            get
            {
                return m_Site;
            }

            set
            {
                m_Site = value;
                NotifyPropertyChanged("Site");
            }
        }

        public string ProjectNumber
        {
            get
            {
                return m_ProjectNumber;
            }

            set
            {
                m_ProjectNumber = value;
                NotifyPropertyChanged("ProjectNumber");
            }
        }

        public string ProjectPart
        {
            get
            {
                return m_ProjectPart;
            }

            set
            {
                m_ProjectPart = value;
                NotifyPropertyChanged("ProjectPart");
            }
        }

        public DateTime ProjectDate
        {
            get
            {
                return m_Date;
            }

            set
            {
                m_Date = value;
                NotifyPropertyChanged("ProjectDate");
            }
        }

        public string CustomerName
        {
            get
            {
                return m_CustomerName;
            }

            set
            {
                m_CustomerName = value;
                NotifyPropertyChanged("CustomerName");
            }
        }

        public string CustomerContactPerson
        {
            get
            {
                return m_CustomerContactPerson;
            }

            set
            {
                m_CustomerContactPerson = value;
                NotifyPropertyChanged("CustomerContactPerson");
            }
        }

        public string SalesPerson
        {
            get
            {
                return m_SalesPerson;
            }

            set
            {
                m_SalesPerson = value;
                NotifyPropertyChanged("SalesPerson");
            }
        }

        public string SalesPersonPhone
        {
            get
            {
                return m_SalesPersonPhone;
            }

            set
            {
                m_SalesPersonPhone = value;
                NotifyPropertyChanged("SalesPersonPhone");
            }
        }
        public string SalesPersonEmail
        {
            get
            {
                return m_SalesPersonEmail;
            }

            set
            {
                m_SalesPersonEmail = value;
                NotifyPropertyChanged("SalesPersonEmail");
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CProjectInfoVM()
        {
            ProjectDate = DateTime.Now;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public CProjectInfo GetProjectInfo()
        {
            CProjectInfo pi = new CProjectInfo(m_ProjectName, m_Site, m_ProjectNumber, m_ProjectPart, m_Date, m_CustomerName, m_CustomerContactPerson,
                                               m_SalesPerson, m_SalesPersonPhone, m_SalesPersonEmail);
            return pi;
        }

        public void SetViewModel(CProjectInfo pi)
        {
            if (pi == null) return;
            ProjectName = pi.ProjectName;
            ProjectNumber = pi.ProjectNumber;
            ProjectPart = pi.ProjectPart;
            Site = pi.Site;
            ProjectDate = pi.Date;

            CustomerName = pi.CustomerName;
            CustomerContactPerson = pi.CustomerContactPerson;

            SalesPerson = pi.SalesPerson;
            SalesPersonPhone = pi.SalesPersonPhone;
            SalesPersonEmail = pi.SalesPersonEmail;
        }        
    }
}
