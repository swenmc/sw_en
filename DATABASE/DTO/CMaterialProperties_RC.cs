using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CMaterialProperties_RC
    {
        private int m_ID;
        private string m_Standard;
        private string m_Grade;
        //private string m_E;
        //private string m_G;
        private string m_Nu;

        private string m_fc;
        private string m_Rho;
        private string m_Alpha;

        public int ID
        {
            get
            {
                return m_ID;
            }

            set
            {
                m_ID = value;
            }
        }

        public string Standard
        {
            get
            {
                return m_Standard;
            }

            set
            {
                m_Standard = value;
            }
        }

        public string Grade
        {
            get
            {
                return m_Grade;
            }

            set
            {
                m_Grade = value;
            }
        }

        /*
        public string E
        {
            get
            {
                return m_E;
            }

            set
            {
                m_E = value;
            }
        }
        */

        /*
        public string G
        {
            get
            {
                return m_G;
            }

            set
            {
                m_G = value;
            }
        }
        */
        public string Nu
        {
            get
            {
                return m_Nu;
            }

            set
            {
                m_Nu = value;
            }
        }

        public string Fc
        {
            get
            {
                return m_fc;
            }
            set
            {
                m_fc = value;
            }
        }

        public string Rho
        {
            get
            {
                return m_Rho;
            }

            set
            {
                m_Rho = value;
            }
        }

        public string Alpha
        {
            get
            {
                return m_Alpha;
            }

            set
            {
                m_Alpha = value;
            }
        }

        /*
        public string Note
        {
            get
            {
                return m_note;
            }

            set
            {
                m_note = value;
            }
        }
        */

        public CMaterialProperties_RC() { }
    }
}
