using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;

namespace MATERIAL
{
    // Default steel material class
    public class CMat_03_00:CMat
    {
        // Default - steel
        // General material properties

        // Design properties
        public float [] m_ft_interval;
        public float [] m_ff_yk;
        public float [] m_ff_u;

        public string m_standard;
        public string Standard
        {
            get { return m_standard; }
            set { m_standard = value; }
        }

        public string m_note;
        public string Note
        {
            get { return m_note; }
            set { m_note = value; }
        }

        // Constructor
        public CMat_03_00()
        {
            Standard = "";
            Name = "Steel S235"; // Default Name
            m_sMatType = 3;
            m_fE = 2.1e11f;   // Unit [Pa]
            m_fNu = 0.3f;    // Unit [-]
            m_fG = m_fE / (2f * (1f + m_fNu)); // Unit [Pa]
            m_fAlpha_T = 1.2e-5f; // Unit [1/Celsius degree]

            m_ft_interval = new float[1];
            m_ff_yk = new float[1];
            m_ff_u = new float[1];

            m_ft_interval[0] = 0.4f; // 400 mm
            m_ff_yk[0] = 2.35e+8f;
            m_ff_u[0] = 3.60e+8f;
        }

        public CMat_03_00(string name_temp, float ft, float fy, float fu)
        {
            Standard = "";
            Name = name_temp; // Default Name
            m_sMatType = 3;
            m_fE = 2.1e11f;   // Unit [Pa]
            m_fNu = 0.3f;    // Unit [-]
            m_fG = m_fE / (2f * (1f + m_fNu)); // Unit [Pa]
            m_fAlpha_T = 1.2e-5f; // Unit [1/Celsius degree]

            m_ft_interval = new float [1] { ft };
            m_ff_yk = new float[1] { fy };
            m_ff_u = new float[1] { fu };
        }

        public CMat_03_00(string standard_temp, string name_temp, float [] ft, float [] fy, float [] fu)
        {
            Standard = standard_temp;
            Name = name_temp; // Default Name
            m_sMatType = 3;
            m_fE = 2.1e11f;   // Unit [Pa]
            m_fNu = 0.3f;    // Unit [-]
            m_fG = m_fE / (2f * (1f + m_fNu)); // Unit [Pa]
            m_fAlpha_T = 1.2e-5f; // Unit [1/Celsius degree]

            m_ft_interval = ft;
            m_ff_yk = fy;
            m_ff_u = fu;
        }

    }
}
