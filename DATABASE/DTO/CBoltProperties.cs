using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CBoltProperties
    {
        private int m_ID;
        private string m_Name;
        private string m_Standard;
        private double m_threadDiameter;
        private double m_shankDiameter;
        private double m_pitchDiameter;
        private double m_pitch_coarse;
        private double m_pitch_fine;
        private string m_code;
        private double m_threadAngle_deg;
        private double m_H;

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
                return m_Name;
            }

            set
            {
                m_Name = value;
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

        public double ThreadDiameter
        {
            get
            {
                return m_threadDiameter;
            }

            set
            {
                m_threadDiameter = value;
            }
        }

        public double ShankDiameter
        {
            get
            {
                return m_shankDiameter;
            }

            set
            {
                m_shankDiameter = value;
            }
        }

        public double PitchDiameter
        {
            get
            {
                return m_pitchDiameter;
            }

            set
            {
                m_pitchDiameter = value;
            }
        }

        public double Pitch_coarse
        {
            get
            {
                return m_pitch_coarse;
            }

            set
            {
                m_pitch_coarse = value;
            }
        }

        public double Pitch_fine
        {
            get
            {
                return m_pitch_fine;
            }

            set
            {
                m_pitch_fine = value;
            }
        }

        public string Code
        {
            get
            {
                return m_code;
            }

            set
            {
                m_code = value;
            }
        }

        public double ThreadAngle_deg
        {
            get
            {
                return m_threadAngle_deg;
            }

            set
            {
                m_threadAngle_deg = value;
            }
        }

        public double H
        {
            get
            {
                return m_H;
            }

            set
            {
                m_H = value;
            }
        }

        public CBoltProperties() { }
    }
}
