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
    public class CProjectInfoVM : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_ProjectName;
        private string m_Site;
        private string m_ProjectNumber;
        private string m_ProjectPart;
        private DateTime m_Date;

        private string m_ContactPerson;
        private string m_ContactPersonPhone;
        private string m_ContactPersonEmail;
        private string m_CustomerName;

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

        public string ContactPerson
        {
            get
            {
                return m_ContactPerson;
            }

            set
            {
                m_ContactPerson = value;
                NotifyPropertyChanged("ContactPerson");
            }
        }

        public string ContactPersonPhone
        {
            get
            {
                return m_ContactPersonPhone;
            }

            set
            {
                m_ContactPersonPhone = value;
                NotifyPropertyChanged("ContactPersonPhone");
            }
        }
        public string ContactPersonEmail
        {
            get
            {
                return m_ContactPersonEmail;
            }

            set
            {
                m_ContactPersonEmail = value;
                NotifyPropertyChanged("ContactPersonEmail");
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
            CProjectInfo pi = new CProjectInfo(m_ProjectName, m_Site, m_ProjectNumber, m_ProjectPart, m_Date, m_ContactPerson, m_ContactPersonPhone, m_ContactPersonEmail, m_CustomerName);
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

            ContactPerson = pi.ContactPerson;
            ContactPersonPhone = pi.ContactPersonPhone;
            ContactPersonEmail = pi.ContactPersonEmail;
            CustomerName = pi.CustomerName;
        }
    }
}
