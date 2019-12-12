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

        private string m_ContactPerson;
        private string m_ContactPersonPhone;
        private string m_ContactPersonEmail;
        private string m_CustomerName;

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

        public string ContactPerson
        {
            get
            {
                return m_ContactPerson;
            }

            set
            {
                m_ContactPerson = value;
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

        

        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        public CProjectInfo(string projectName, string site, string projectNumber, string projectPart, DateTime date, string contactPerson, string contactPersonPhone, string contactPersonEmail, string customerName)
        {
            // TO Ondrej - ak to uzivatel neneditoval a je to nevyplnene typu null, tak to potom assertuje pri exporte
            m_ProjectName = projectName == null ? "" : projectName;
            m_Site = site = site == null ? "" : site;
            m_ProjectNumber = projectNumber == null ? "" : projectNumber;
            m_ProjectPart = projectPart == null ? "" : projectPart;
            m_Date = date;
            m_ContactPerson = contactPerson;
            m_ContactPersonPhone = contactPersonPhone;
            m_ContactPersonEmail = contactPersonEmail;
            m_CustomerName = customerName;
        }
    }
}
