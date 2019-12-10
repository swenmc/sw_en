using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CTS_CoatingProperties
    {
        private int m_ID;
        private string m_name;
        private int[] m_ColorIDs;
        private int m_PriceCode;

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

        public string Name
        {
            get
            {
                return m_name;
            }

            set
            {
                m_name = value;
            }
        }

        public int[] ColorIDs
        {
            get
            {
                return m_ColorIDs;
            }

            set
            {
                m_ColorIDs = value;
            }
        }

        public int PriceCode
        {
            get
            {
                return m_PriceCode;
            }

            set
            {
                m_PriceCode = value;
            }
        }

        public CTS_CoatingProperties() { }
    }
}
