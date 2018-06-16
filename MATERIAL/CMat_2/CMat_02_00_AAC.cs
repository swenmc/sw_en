using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATERIAL
{
    public class CMat_02_00_AAC : CMat_02_00
    {

        double m_Rho_m;

        public double Rho_m
        {
            get { return m_Rho_m; }
            set { m_Rho_m = value; }
            //get = m_Rho_m;
            //set = m_Rho_m = value;
        }
        double fcflk0_05;

        public double Fcflk0_05
        {
            get { return fcflk0_05; }
            set { fcflk0_05 = value; }
        }

        double fcflk0_95;

        public double Fcflk0_95
        {
            get { return fcflk0_95; }
            set { fcflk0_95 = value; }
        }


        public CMat_02_00_AAC()
        {

        }

        public CMat_02_00_AAC(int id_database, float frho_m_density)
        {
            FillData(id_database, frho_m_density);
        }

        public void FillData(int id_database, double rho_m_density)
        {
            Rho_m = rho_m_density; //kg/m^3
            Fck = AAC_fck_array[id_database] * 1.0e+6; // Pa

            Fctk0_05 = 0.1f * Fck;
            Fctk0_95 = 0.24f * Fck;
            Fcflk0_05 = 0.18f * Fck;
            Fcflk0_95 = 0.36f * Fck;
            D_Ecm = 5 * (Rho_m - 150) * 1e+6f; //Pa EN 12602 - 4.2.7(6)
        }

        // Autoclaved Aered Concrete

        private float[] AAC_fck_array = new float[] {1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 5.5f, 6, 7, 8, 9, 10}; // MPa
    }
}
