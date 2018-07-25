using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using BaseClasses;
using MATH;
using MATERIAL;

namespace CRSC
{
    // THIN-WALLED CROSS-SECTION PROPERTIES CALCULATION

    public abstract class CCrSc_TW : CCrSc
    {
        #region variables

        // VARIABLES
        // EN 1999-1-1:2007 Annex J - J.4, page 183

        public float[,] arrPointCoord;
        public List<double> y_suradnice;
        public List<double> z_suradnice;
        public List<double> t_hodnoty;
        //public double _h;
        //public double _b;
        //public double _A_g;                 // Cross-section area (J.6)
        public double _A_elem;                // Area off segment (J.5)
        //public double _d_A_vy;              // Cross-section shear area
        //public double _d_A_vz;              // Cross-section shear area
        //public double _Sy0;                 // Static modulus to primary axis yO and zO  (J.7) and (J.9)
        //public double _Sz0;                 // Static modulus to primary axis zO
        //public double _Sy;                  // Static modulus to centre of gravity
        //public double _Sz;                  // Static modulus to centre of gravity
        //public double d_z_gc;               // Gentre of gravity coordinate // (J.7)
        //public double d_y_gc;               // Gentre of gravity coordinate // (J.7)
        //public double _Iy0;                 // Moment of inertia (Second moment of area) y0-y0 and z0-z0 // (J.8)
        //public double _Iy;                  // Moment of inertia (Second moment of area) y-y and z-z // (J.8)
        //public double _Iz0;                 // (J.10)
        //public double _Iz;                  // (J.10)
        //public double _Iyz0;                // Deviacni moment k puvodnim osam y a z // (J.11)
        //public double _Iyz;                 // Deviacni moment k osam y a z prochazejicim tezistem // (J.11)
        public double _alfa;                  // Rotation of main axis / Natoceni hlavnich os  // (J.12)
        //public double _Iepsilon;            // Moment of inertia (Second moment of area) to main axis - greek letters XI and ETA  // (J.13)
        //public double _Imikro;              // (J.14)

        public double[] omega0i;            // Vysecove souradnice (J.15)
        public double[] omega;
        public double _Iomega;              // Stredni vysecova souradnice  // (J.16)

        public double _omega_mean           // Stredni vysecova souradnice  // (J.16)
             , _Iy_omega0            // Staticky vysecový moment // (J.17)
             , _Iz_omega0            // (J.18)
             , _Iomega_omega0        // (J.19)
             , _Iy_omega             // Staticky vysecový moment // (J.17)
             , _Iz_omega             // Staticky vysecový moment // (J.18) 
             , _Iomega_omega         // Staticky vysecový moment // (J.19)
             , _Ip                   // Polarni moment setrvacnosti // (J.26)
             //, d_y_sc                // Souradnice stredu smyku (J.20) // (J.20)
             //, d_z_sc                // (J.20)
             //, d_y_s                 // Vzdalenost stredu smyku a teziste // (J.25)
             //, d_z_s                 // (J.25)
             //, d_I_w                 // Vysecovy moment setrvacnosti (J.21)  // Warping constant, Iw - Vysecovy moment setrvacnosti
                                     //, d_I_t                 // St. Venant torsional constant / Moment tuhosti v prostem krouceni // (J.22)
                                     //, d_W_t                 // Modul odporu prierezu v kruteni / Modul tuhosti v prostem krouceni // (J.22)
             , omega_max             // Nejvetsi vysecove poradnice a vysecovy modul // (J.24)
             //, d_W_w                 // Vysecovy modul (J.24)
             , d_z_j                 // Factors of asymetry (J.27) and (J.28)  // according Annex I
             , d_y_j                 // (J.28)
             , d_y_ci                // Partial coordinates of centre of cross-section segments // (J.29)
             , d_z_ci;                // Partial coordinates of centre of cross-section segments
             //, _Sw                   // Sectorial product of area  Staticky vysecovy moment // missing formula
             //, _Wy_el_1              // Elastic cross-section modulus y-y and z-z
             //, _Wy_el_2              // Elastic cross-section modulus y-y and z-z
             //, _Wz_el_1              // Elastic cross-section modulus y-y and z-z
             //, _Wz_el_2              // Elastic cross-section modulus y-y and z-z
             //, _Wy_pl                // Plastic cross-section modulus y-y and z-z
             //, _Wz_pl;               // Plastic cross-section modulus y-y and z-z

        public double[] d_omega_s;          // Vysecove souradnice ktere jsou vztazeny ke stredu smyku (J.23) // (J.23)
        public double m_Beta_y, m_Beta_z;      // Monosymmetry constant AS / NZS standards
        public double m_t_min;
        public double m_t_max;

        public double dBending_curve_stress_x1;
        public double dBending_curve_stress_x2;
        public double dBending_curve_stress_x3;
        public double dBending_curve_stress_y;
        public double dCompression_curve_stress_1;
        public double dCompression_curve_stress_2;
        public double dCompression_curve_stress_3;

        #endregion

        #region Properties

        public double A_elem
        {
            get { return _A_elem; }
            set { _A_elem = value; }
        }

        public double Iomega
        {
            get { return _Iomega; }
            set { _Iomega = value; }
        }
        public double Omega_mean
        {
            get { return _omega_mean; }
            set { _omega_mean = value; }
        }

        public double D_y_j
        {
            get { return d_y_j; }
            set { d_y_j = value; }
        }

        public double D_z_j
        {
            get { return d_z_j; }
            set { d_z_j = value; }
        }

        public double Omega_max
        {
            get { return omega_max; }
            set { omega_max = value; }
        }

        public double Ip
        {
            get { return _Ip; }
            set { _Ip = value; }
        }

        public double Iomega_omega
        {
            get { return _Iomega_omega; }
            set { _Iomega_omega = value; }
        }

        public double Iz_omega
        {
            get { return _Iz_omega; }
            set { _Iz_omega = value; }
        }

        public double Iy_omega
        {
            get { return _Iy_omega; }
            set { _Iy_omega = value; }
        }

        public double Beta_y
        {
            get { return m_Beta_y; }
            set { m_Beta_y = value; }
        }

        public double Beta_z
        {
            get { return m_Beta_z; }
            set { m_Beta_z = value; }
        }

        public double t_min
        {
            get { return m_t_min; }
            set { m_t_min = value; }
        }

        public double t_max
        {
            get { return m_t_max; }
            set { m_t_max = value; }
        }

        public double Bending_curve_stress_x1
        {
            get { return dBending_curve_stress_x1; }
            set { dBending_curve_stress_x1 = value; }
        }

        public double Bending_curve_stress_x2
        {
            get { return dBending_curve_stress_x2; }
            set { dBending_curve_stress_x2 = value; }
        }

        public double Bending_curve_stress_x3
        {
            get { return dBending_curve_stress_x3; }
            set { dBending_curve_stress_x3 = value; }
        }

        public double Bending_curve_stress_y
        {
            get { return dBending_curve_stress_y; }
            set { dBending_curve_stress_y = value; }
        }

        public double Compression_curve_stress_1
        {
            get { return dCompression_curve_stress_1; }
            set { dCompression_curve_stress_1 = value; }
        }

        public double Compression_curve_stress_2
        {
            get { return dCompression_curve_stress_2; }
            set { dCompression_curve_stress_2 = value; }
        }

        public double Compression_curve_stress_3
        {
            get { return dCompression_curve_stress_3; }
            set { dCompression_curve_stress_3 = value; }
        }

        // end of cross-section variables definition
        #endregion

        public new CMat_03_00 m_Mat = new CMat_03_00();

        // Default example
        public CCrSc_TW()
        {
            m_Mat.Name = "Steel S500"; //Temporary
        }

        // Data in datagrid
        public CCrSc_TW(List<double> y_suradnice, List<double> z_suradnice, List<double> t_hodnoty)
        {
            m_Mat.Name = "Steel S500"; //Temporary

            int count = y_suradnice.Count;

            this.y_suradnice = y_suradnice;
            this.z_suradnice = z_suradnice;
            this.t_hodnoty = t_hodnoty;
        }

        protected override void loadCrScIndices()
        {
            int capacity = TriangleIndicesFrontSide.Count + TriangleIndicesShell.Count + TriangleIndicesBackSide.Count;
            List<int> list = new List<int>(capacity);

            // Postup pridavania bodov (positions) do kolekcie
            // Front side - outside
            // Front side - inside

            // Back side - outside
            // Back side - inside

            // Postup pridavania indices (indexov bodov) do kolekcie
            // Front side
            // Back side
            // Shell - outside
            // Shell - inside

            // TODO Martin - bude potrebne skontrolovat ako to funguje pre auxiliary points ak je pocet vacsi nez nula (body v ploche prierezu, ktore nelezia na outline)

            list.AddRange(TriangleIndicesFrontSide);
            list.AddRange(TriangleIndicesBackSide);
            list.AddRange(TriangleIndicesShell);

            TriangleIndices = new Int32Collection(list);
        }

        protected override void loadCrScIndicesFrontSide()
        {
            TriangleIndicesFrontSide = new Int32Collection();

            // Front Side / Forehead
            if (IsShapeSolid)
            {
                for (int i = 0; i < (ITotNoPoints - 2) / 2; i++)
                    AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, i, i + 1, ITotNoPoints - i - 2, ITotNoPoints - i - 1);
            }
            else
            {
                // INoAuxPoints - number of auxiliary points in inside/outside collection of points
                // INoPointsOut - number of real points in inside/outside collection of points
                // INoAuxPoints + INoPointsOut - total number of points in inside/outside collection of section

                INoAuxPoints = 0;
                // Front Side / Forehead

                for (int i = 0; i < INoPointsOut; i++)
                {
                    if (i < INoPointsOut - 1)
                        AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, INoAuxPoints + i, INoAuxPoints + i + 1, INoAuxPoints + i + (INoAuxPoints + INoPointsOut) + 1, INoAuxPoints + i + (INoAuxPoints + INoPointsOut));
                    else
                        AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, INoAuxPoints + i, INoAuxPoints + 0, INoAuxPoints + i + INoAuxPoints + 1, INoAuxPoints + i + (INoAuxPoints + INoPointsOut)); // Last Element
                }
            }
        }

        protected override void loadCrScIndicesShell()
        {
            TriangleIndicesShell = new Int32Collection();

            // Shell Surface OutSide
            if (IsShapeSolid)
            {
                for (int i = 0; i < ITotNoPoints - 1; i++)
                {
                    if (i < ITotNoPoints - 2)
                        AddRectangleIndices_CCW_1234(TriangleIndicesShell, i, ITotNoPoints + i, ITotNoPoints + i + 1, i + 1);
                    else
                        AddRectangleIndices_CCW_1234(TriangleIndicesShell, i, ITotNoPoints + i, ITotNoPoints, 0); // Last Element
                }
            }
            else
            {
                // iAux - number of auxiliary points in inside/outside collection of points
                // INoPointsOut - number of real points in inside/outside collection of points
                // iAux + INoPointsOut - total number of points in inside/outside collection of section

                // Shell Surface OutSide
                for (int i = 0; i < INoPointsOut; i++)
                {
                    if (i < INoPointsOut - 1)
                        AddRectangleIndices_CCW_1234(TriangleIndicesShell, INoAuxPoints + i, 2 * (INoAuxPoints + INoPointsOut) + INoAuxPoints + i, 2 * (INoAuxPoints + INoPointsOut) + INoAuxPoints + i + 1, INoAuxPoints + i + 1);
                    else
                        AddRectangleIndices_CCW_1234(TriangleIndicesShell, INoAuxPoints + i, 2 * (INoAuxPoints + INoPointsOut) + INoAuxPoints + i, 2 * (INoAuxPoints + INoPointsOut) + INoAuxPoints, INoAuxPoints + 0); // Last Element
                }

                // Shell Surface Inside
                for (int i = 0; i < INoPointsOut; i++)
                {
                    if (i < INoPointsOut - 1)
                        AddRectangleIndices_CCW_1234(TriangleIndicesShell, INoAuxPoints + INoPointsOut + INoAuxPoints + i, INoAuxPoints + INoPointsOut + INoAuxPoints + i + 1, 2 * (INoAuxPoints + INoPointsOut) + i + 2 * INoAuxPoints + INoPointsOut + 1, 2 * (INoAuxPoints + INoPointsOut) + i + 2 * INoAuxPoints + INoPointsOut);
                    else
                        AddRectangleIndices_CCW_1234(TriangleIndicesShell, 2 * (INoAuxPoints + INoPointsOut) + 2 * INoAuxPoints + i + 1, 2 * (INoAuxPoints + INoPointsOut) + i + 2 * INoAuxPoints + INoPointsOut, INoAuxPoints + INoPointsOut + INoAuxPoints + i, 2 * INoAuxPoints + INoPointsOut); // Last Element
                }
            }
        }

        protected override void loadCrScIndicesBackSide()
        {
            TriangleIndicesBackSide = new Int32Collection();

            // Back Side
            if (IsShapeSolid)
            {
                for (int i = 0; i < (ITotNoPoints - 2) / 2; i++)
                    AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + i, ITotNoPoints + i + 1, ITotNoPoints + ITotNoPoints - i - 2, ITotNoPoints + ITotNoPoints - i - 1);
            }
            else
            {
                // INoAuxPoints - number of auxiliary points in inside/outside collection of points
                // INoPointsOut - number of real points in inside/outside collection of points
                // INoAuxPoints + INoPointsOut - total number of points in inside/outside collection of section

                // Back Side
                for (int i = 0; i < INoPointsOut; i++)
                {
                    if (i < INoPointsOut - 1)
                        AddRectangleIndices_CW_1234(TriangleIndicesBackSide, 2 * (INoAuxPoints + INoPointsOut) + INoAuxPoints + i, 2 * (INoAuxPoints + INoPointsOut) + i + 2 * INoAuxPoints + INoPointsOut, 2 * (INoAuxPoints + INoPointsOut) + i + 2 * INoAuxPoints + INoPointsOut + 1, 2 * (INoAuxPoints + INoPointsOut) + INoAuxPoints + i + 1);
                    else
                        AddRectangleIndices_CW_1234(TriangleIndicesBackSide, 2 * (INoAuxPoints + INoPointsOut) + INoAuxPoints + i, 2 * (INoAuxPoints + INoPointsOut) + i + 2 * INoAuxPoints + INoPointsOut, 2 * (INoAuxPoints + INoPointsOut) + i + 2 * INoAuxPoints + 1, 2 * (INoAuxPoints + INoPointsOut) + INoAuxPoints + 0); // Last Element
                }
            }
        }
    }
}
