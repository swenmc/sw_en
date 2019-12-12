using System;

namespace BaseClasses
{
    [Serializable]
    public class CProjectInfo
    {
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

        public string ProjectName
        {
            get
            {
                return m_ProjectName;
            }

            set
            {
                m_ProjectName = value;
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
            }
        }

        public DateTime Date
        {
            get
            {
                return m_Date;
            }

            set
            {
                m_Date = value;
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
            }
        }


        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        public CProjectInfo(string projectName, string site, string projectNumber, string projectPart, DateTime date,
            string customerName, string customerContactPerson,
            string salesPerson, string salesPersonPhone, string salesPersonEmail)
        {
            m_ProjectName = projectName == null ? "" : projectName;
            m_Site = site = site == null ? "" : site;
            m_ProjectNumber = projectNumber == null ? "" : projectNumber;
            m_ProjectPart = projectPart == null ? "" : projectPart;
            m_Date = date;

            m_CustomerName = customerName == null ? "" : customerName;
            m_CustomerContactPerson = customerContactPerson == null ? "" : customerContactPerson;

            m_SalesPerson = salesPerson == null ? "" : salesPerson;
            m_SalesPersonPhone = salesPersonPhone == null ? "" : salesPersonPhone;
            m_SalesPersonEmail = salesPersonEmail == null ? "" : salesPersonEmail;
        }
    }
}
