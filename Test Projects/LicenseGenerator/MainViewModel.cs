using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace LicenseGenerator
{
    [Serializable]
    public class MainViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private string m_CustomerName;
        private List<string> m_Roles;
        private string m_Role;
        private int m_Year;
        private int m_Month;
        private int m_Day;
        private string m_key;

        public bool IsSetFromCode = false;
                
        //-------------------------------------------------------------------------------------------------------------
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

        public List<string> Roles
        {
            get
            {
                if (m_Roles == null) m_Roles = new List<string>() { "developer", "admin", "customer", "architect", "engineer", "salesperson"};
                return m_Roles;
            }

            set
            {
                m_Roles = value;
                Role = m_Roles.First();
                NotifyPropertyChanged("Roles");
            }
        }

        public string Role
        {
            get
            {
                return m_Role;
            }

            set
            {
                m_Role = value;
                NotifyPropertyChanged("Role");
            }
        }

        public int Year
        {
            get
            {
                return m_Year;
            }

            set
            {
                m_Year = value;
                NotifyPropertyChanged("Year");
            }
        }

        public int Month
        {
            get
            {
                return m_Month;
            }

            set
            {
                m_Month = value;
                NotifyPropertyChanged("Month");
            }
        }

        public int Day
        {
            get
            {
                return m_Day;
            }

            set
            {
                m_Day = value;
                NotifyPropertyChanged("Day");
            }
        }

        public string Key
        {
            get
            {
                return m_key;
            }

            set
            {
                m_key = value;
                NotifyPropertyChanged("Key");
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public MainViewModel()
        {
            DateTime trial = DateTime.Now.AddDays(30);
            Year = trial.Year;
            Month = trial.Month;
            Day = trial.Day;

            Role = Roles.First();

            IsSetFromCode = false;
        }

        public string GetLicenseKey()
        {
            string text = $"opmc_{Year}_{Month}_{Day}_{Role}";
            string passPhrase = $"opmc_{CustomerName}";
            string encryptedstring = StringCipher.Encrypt(text, passPhrase);
            //string decryptedstring = StringCipher.Decrypt(encryptedstring, passPhrase);
            return encryptedstring;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
