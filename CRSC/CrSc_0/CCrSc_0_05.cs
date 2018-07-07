using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_05 : CCrSc
    {
        // Rectangular/ Square - Flat bar

        //----------------------------------------------------------------------------
        //private float h;   // Height/ Depth/ Vyska
        //private float b;   // Width  / Sirka
        //private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing
        //public float[,] m_CrScPoint; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        //----------------------------------------------------------------------------
        // Cross-section properties
        //----------------------------------------------------------------------------

        /*float m_fU, m_fA,
                m_fS_y, m_fI_y, m_fW_y_el, m_fW_y_pl, m_ff_y_plel,
                m_fS_z, m_fI_z, m_fW_z_el, m_fW_z_pl, m_ff_z_plel,
                m_fW_t_el, m_fI_t, m_fi_t, m_fW_t_pl, m_ff_t_plel, m_fI_w,
                m_fEta_y_v, m_fA_y_v_el, m_fA_y_v_pl, m_ff_y_v_plel,
                m_fEta_z_v, m_fA_z_v_el, m_fA_z_v_pl, m_ff_z_v_plel;*/

        // Temp

        bool bIndicesCW = true; // Clockwise or counter-clockwise system

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_05() { }
        /*public CCrSc_0_05(float fh, float fb)
        {
            IsShapeSolid = true;
            ITotNoPoints = 4;
            h = fh;
            b = fb;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }*/

        public CCrSc_0_05(float fh, float fb)
        {
            IsShapeSolid = true;
            ITotNoPoints = 4;
            h = fh;
            b = fb;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges

            // Particular indices Rozpracovane pre vykreslovanie cela prutu inou farbou
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // All indices together
            loadCrScIndices();

            // Calculate cross-section properties
            Calc_A();
            Calc_I_y();
            Calc_I_z();
            Calc_I_t();
        }

        //----------------------------------------------------------------------------
        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            CrScPointsOut = Geom2D.GetRectanglePointCoord((float)h, (float)b);
        }

        // Perimeter of section
        void Calc_U()
        {
            U = 2 * (h + b);
        }
        // Section area
        void Calc_A()
        {
            A_g = b * h;
        }


        // First moment o area
        void Calc_S_y()
        {
            S_y = b * MathF.Pow2(h) / 8f;
        }
        // Second moment of area
        void Calc_I_y()
        {
            I_y = b * MathF.Pow3(h) / 12f;
        }
        // Section modulus - elastic
        void Calc_W_y_el()
        {
            W_y_el = b * MathF.Pow3(h) / 6f;
        }
        // Section modulus - plastic
        void Calc_W_y_pl()
        {
            W_y_pl = b * MathF.Pow2(h) / 4f;
        }
        // Shape factor - plastic/elastic
        void Calc_f_y_plel()
        {
            f_y_plel = 1.5f;
        }


        // First moment o area
        void Calc_S_z()
        {
            S_z = h * MathF.Pow2(b) / 8f;
        }
        // Second moment of area
        void Calc_I_z()
        {
            I_z = h * MathF.Pow3(b) / 12f;
        }
        // Section modulus - elastic
        void Calc_W_z_el()
        {
            W_z_el = h * MathF.Pow3(b) / 6f;
        }
        // Section modulus - plastic
        void Calc_W_z_pl()
        {
            W_z_pl = h * MathF.Pow2(b) / 4f;
        }
        // Shape factor - plastic/elastic
        void Calc_f_z_plel()
        {
            f_z_plel = 1.5f;
        }


        // Torsional inertia constant
        void Calc_I_t()
        {
            // http://www.xcalcs.com
            if (h >= b)
                I_t = h * MathF.Pow3(b) * ((1 - 192 * b / MathF.Pow5(MathF.fPI) * h * ((float)Math.Tanh(Math.PI * h / (2 * b))) + (float)Math.Tanh(3 * Math.PI * h / (2 * b)) / 243f)) / 3f;
            else
                I_t = b * MathF.Pow3(h) * ((1 - 192 * h / MathF.Pow5(MathF.fPI) * b * ((float)Math.Tanh(Math.PI * b / (2 * h))) + (float)Math.Tanh(3 * Math.PI * b / (2 * h)) / 243f)) / 3f;

            // Alternative  - EN 1999-1-1, eq. (J.2)
            if (h >= b)
                I_t = (h * MathF.Pow3(b) / 3.0f) * (1.0f - 0.63f * b / h + 0.052f * MathF.Pow5(b) / MathF.Pow5(h));
            else
                I_t = (b * MathF.Pow3(h) / 3.0f) * (1.0f - 0.63f * h / b + 0.052f * MathF.Pow5(h) / MathF.Pow5(b));
        }
        // Torsional radius of gyration
        void Calc_i_t()
        {
            i_t = MathF.Sqrt(I_t / A_g);
        }
        // Torsional section modulus - elastic
        void Calc_W_t_el()
        {
            if (h >= b)
                W_t_el = I_t / b * (1 - 8 * (1 / (float)Math.Cosh(Math.PI * h / (2 * b)) + 1 / 9f * (float)Math.Cosh(3 * Math.PI * h / (2 * b))) / MathF.Pow2(MathF.fPI));
            else
                W_t_el = I_t / h * (1 - 8 * (1 / (float)Math.Cosh(Math.PI * b / (2 * h)) + 1 / 9f * (float)Math.Cosh(3 * Math.PI * b / (2 * h))) / MathF.Pow2(MathF.fPI));
        }
        // Torsional section modulus - plastic
        void Calc_W_t_pl()
        {
            if (h >= b)
                W_t_pl = MathF.Pow2(b) * (3 * h - b) / 6f;
            else
                W_t_pl = MathF.Pow2(h) * (3 * b - h) / 6f;
        }
        // Torsional shape factor plastic/elastic
        void Calc_f_t_plel()
        {
            f_t_plel = W_t_pl / W_t_el;
        }
        // Section warping constant
        void Calc_I_w()
        {
            if (h >= b)
                I_w = (MathF.Pow3(h) * MathF.Pow3(b) / 144.0f) * (1.0f - 4.884f * MathF.Pow2(b) / MathF.Pow2(h) + 4.97f * MathF.Pow3(b) / MathF.Pow3(h) - 1.067f * MathF.Pow5(b) / MathF.Pow5(h)); // EN 1999-1-1, eq. (J.4)
            else
                I_w = (MathF.Pow3(b) * MathF.Pow3(h) / 144.0f) * (1.0f - 4.884f * MathF.Pow2(h) / MathF.Pow2(b) + 4.97f * MathF.Pow3(h) / MathF.Pow3(b) - 1.067f * MathF.Pow5(h) / MathF.Pow5(b)); // EN 1999-1-1, eq. (J.4)
        }



        // Shear factor
        void Calc_Eta_y_v()
        {
            Eta_y_v = 1.2f;
        }
        // Shear effective area - elastic
        void Calc_A_y_v_el()
        {
            A_y_v_el = 0.75f * A_g; // Temp
        }
        // Shape factor for shear - plastic/elastic
        void Calc_f_y_v_plel()
        {
            f_y_v_plel = 1.00f; // Temp
        }
        // Shear effective area - plastic
        void Calc_A_y_v_pl()
        {
            A_y_v_pl = f_y_v_plel * A_y_v_el; // Temp
        }


        // Shear factor
        void Calc_Eta_z_v()
        {
            Eta_z_v = 1.2f;
        }
        // Shear effective area - elastic
        void Calc_A_z_v_el()
        {
            A_z_v_el = 0.75f * A_g; // Temp
        }
        // Shape factor for shear - plastic/elastic
        void Calc_f_z_v_plel()
        {
            f_z_v_plel = 1.00f; // Temp
        }
        // Shear effective area - plastic
        void Calc_A_z_v_pl()
        {
            A_z_v_pl = f_z_v_plel * A_z_v_el; // Temp
        }

        protected override void loadCrScIndices()
        {
            // const int secNum = 4;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 3);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 7, 6, 5);

            // Shell Surface
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 4, 5, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 5, 6, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 6, 7, 3);
            AddRectangleIndices_CW_1234(TriangleIndices, 3, 7, 4, 0);
        }

        protected override void loadCrScIndicesFrontSide()
        {
           TriangleIndicesFrontSide = new Int32Collection();
            // Front Side / Forehead
           if(bIndicesCW)
             AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 0, 1, 2, 3);
           else
             AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 0, 1, 2, 3);
        }

        protected override void loadCrScIndicesShell()
        {
            TriangleIndicesShell = new Int32Collection();
            // Shell Surface
            if (bIndicesCW)
            {
                AddRectangleIndices_CW_1234(TriangleIndicesShell, 0, 4, 5, 1);
                AddRectangleIndices_CW_1234(TriangleIndicesShell, 1, 5, 6, 2);
                AddRectangleIndices_CW_1234(TriangleIndicesShell, 2, 6, 7, 3);
                AddRectangleIndices_CW_1234(TriangleIndicesShell, 3, 7, 4, 0);
            }
            else
            {
                AddRectangleIndices_CCW_1234(TriangleIndicesShell, 0, 4, 5, 1);
                AddRectangleIndices_CCW_1234(TriangleIndicesShell, 1, 5, 6, 2);
                AddRectangleIndices_CCW_1234(TriangleIndicesShell, 2, 6, 7, 3);
                AddRectangleIndices_CCW_1234(TriangleIndicesShell, 3, 7, 4, 0);
            }

        }

        protected override void loadCrScIndicesBackSide()
        {
            TriangleIndicesBackSide = new Int32Collection();
            // Back Side
            if (bIndicesCW) // Back view
              AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, 0, 1, 2, 3);
            else
              AddRectangleIndices_CW_1234(TriangleIndicesBackSide, 0, 1, 2, 3);
        }

    }
}
