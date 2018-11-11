using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CAnnualProbability
    {
        float m_AnnualProbabilityULS_Wind;
        float m_AnnualProbabilityULS_Snow;
        float m_AnnualProbabilityULS_EQ;
        float m_AnnualProbabilitySLS;
        float m_R_ULS_Wind;
        float m_R_ULS_Snow;
        float m_R_ULS_EQ;
        float m_R_SLS;

        public float AnnualProbabilityULS_Wind
        {
            get
            {
                return m_AnnualProbabilityULS_Wind;
            }

            set
            {
                m_AnnualProbabilityULS_Wind = value;
            }
        }

        public float AnnualProbabilityULS_Snow
        {
            get
            {
                return m_AnnualProbabilityULS_Snow;
            }

            set
            {
                m_AnnualProbabilityULS_Snow = value;
            }
        }

        public float AnnualProbabilityULS_EQ
        {
            get
            {
                return m_AnnualProbabilityULS_EQ;
            }

            set
            {
                m_AnnualProbabilityULS_EQ = value;
            }
        }

        public float AnnualProbabilitySLS
        {
            get
            {
                return m_AnnualProbabilitySLS;
            }

            set
            {
                m_AnnualProbabilitySLS = value;
            }
        }

        public float R_ULS_Wind
        {
            get
            {
                return m_R_ULS_Wind;
            }

            set
            {
                m_R_ULS_Wind = value;
            }
        }

        public float R_ULS_Snow
        {
            get
            {
                return m_R_ULS_Snow;
            }

            set
            {
                m_R_ULS_Snow = value;
            }
        }

        public float R_ULS_EQ
        {
            get
            {
                return m_R_ULS_EQ;
            }

            set
            {
                m_R_ULS_EQ = value;
            }
        }

        public float R_SLS
        {
            get
            {
                return m_R_SLS;
            }

            set
            {
                m_R_SLS = value;
            }
        }

        public CAnnualProbability() { }
    }
}
