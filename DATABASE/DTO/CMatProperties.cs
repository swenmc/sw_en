using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CMatProperties
    {
        private int m_ID;
        private string m_Standard;
        private string m_Grade;
        private int m_iNumberOfIntervals;
        private double m_t1;
        private double m_t2;
        private double m_t3;
        private double m_t4;
        private double m_f_y1;
        private double m_f_u1;
        private double m_f_y2;
        private double m_f_u2;
        private double m_f_y3;
        private double m_f_u3;
        private double m_f_y4;
        private double m_f_u4;
        private string m_note;

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

        public int iNumberOfIntervals
        {
            get
            {
                return m_iNumberOfIntervals;
            }

            set
            {
                m_iNumberOfIntervals = value;
            }
        }

        public double t1
        {
            get
            {
                return m_t1;
            }

            set
            {
                m_t1 = value;
            }
        }

        public double t2
        {
            get
            {
                return m_t2;
            }

            set
            {
                m_t2 = value;
            }
        }

        public double t3
        {
            get
            {
                return m_t3;
            }

            set
            {
                m_t3 = value;
            }
        }

        public double t4
        {
            get
            {
                return m_t4;
            }

            set
            {
                m_t4 = value;
            }
        }

        public double f_y1
        {
            get
            {
                return m_f_y1;
            }

            set
            {
                m_f_y1 = value;
            }
        }

        public double f_u1
        {
            get
            {
                return m_f_u1;
            }

            set
            {
                m_f_u1 = value;
            }
        }

        public double f_y2
        {
            get
            {
                return m_f_y2;
            }

            set
            {
                m_f_y2 = value;
            }
        }

        public double f_u2
        {
            get
            {
                return m_f_u2;
            }

            set
            {
                m_f_u2 = value;
            }
        }

        public double f_y3
        {
            get
            {
                return m_f_y3;
            }

            set
            {
                m_f_y3 = value;
            }
        }

        public double f_u3
        {
            get
            {
                return m_f_u3;
            }

            set
            {
                m_f_u3 = value;
            }
        }

        public double f_y4
        {
            get
            {
                return m_f_y4;
            }

            set
            {
                m_f_y4 = value;
            }
        }

        public double f_u4
        {
            get
            {
                return m_f_u4;
            }

            set
            {
                m_f_u4 = value;
            }
        }

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

        public CMatProperties() { }
    }
}
