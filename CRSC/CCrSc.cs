using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATERIAL;
using CENEX;

namespace CRSC
{
    [Serializable]
    public abstract class CCrSc
    {
        // Collection of indices for generation of member surface
        // Plati aj pre pruty s nabehom s rovnakym poctom bodov v reze - 2D
        // Use also for tapered members with same number of nodes (points) in section - 2D
        private Int32Collection m_TriangleIndices;
        private Int32Collection m_WireFrameIndices;

        public Int32Collection TriangleIndices
        {
            get { return m_TriangleIndices; }
            set { m_TriangleIndices = value; }
        }

        // New
        private Int32Collection m_TriangleIndicesFrontSide;

        public Int32Collection TriangleIndicesFrontSide
        {
            get { return m_TriangleIndicesFrontSide; }
            set { m_TriangleIndicesFrontSide = value; }
        }

        private Int32Collection m_TriangleIndicesShell;

        public Int32Collection TriangleIndicesShell
        {
            get { return m_TriangleIndicesShell; }
            set { m_TriangleIndicesShell = value; }
        }

        private Int32Collection m_TriangleIndicesBackSide;

        public Int32Collection TriangleIndicesBackSide
        {
            get { return m_TriangleIndicesBackSide; }
            set { m_TriangleIndicesBackSide = value; }
        }

        // Type of cross-section
        private bool m_bIsShapeSolid; // 0 (false) - Hollow section, 1 (true) - Solid section

        public bool IsShapeSolid
        {
            get { return m_bIsShapeSolid; }
            set { m_bIsShapeSolid = value; }
        }

        // Total number of points per section in 2D
        private short m_iTotNoPoints;

        public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }

        // Number of points per inside surface of section in 2D - hollow sections
        private short m_iNoPointsIn;

        public short INoPointsIn
        {
            get { return m_iNoPointsIn; }
            set { m_iNoPointsIn = value; }
        }

        // Number of points per outside surface of section in 2D - hollow sections
        private short m_iNoPointsOut;

        public short INoPointsOut
        {
            get { return m_iNoPointsOut; }
            set { m_iNoPointsOut = value; }
        }

        // Use for Inside surface of hollow sections
        private float[,] m_CrScPointsIn;

        public float[,] CrScPointsIn
        {
            get { return m_CrScPointsIn; }
            set { m_CrScPointsIn = value; }
        }

        // Use for Outside surface of hollow sections and surface of solid sections
        private float[,] m_CrScPointsOut;

        public float[,] CrScPointsOut
        {
            get { return m_CrScPointsOut; }
            set { m_CrScPointsOut = value; }
        }

        private int m_iCrSc_ID;

        public int ICrSc_ID
        {
            get { return m_iCrSc_ID; }
            set { m_iCrSc_ID = value; }
        }

        // Number of auxliary points in one section in one surface (auxialiary points of section in 2D in one surface), these points should be defined before real edges (outside or inside points) definition
        private int m_iNoAuxPoints;

        public int INoAuxPoints
        {
            get { return m_iNoAuxPoints; }
            set { m_iNoAuxPoints = value; }
        }

        private Color m_color;

        public Color CSColor
        {
            get { return m_color; }
            set { m_color = value; }
        }

        // !!! Pre FEM vypocet nepotrebujeme vsetky charakteristiky, len Ag, Avy, Avz, Iy, Iz, It !!!!
        // !!! Ostatne charakteristiky postaci nacitavat az pri posudeni

        // Gross cross-section area Ag
        // Shear effective area A_y_v (A_2_v, A_u_v) - optional
        // Shear effective area A_z_v (A_3_v, A_v_v) - optional
        // Moment of inertia - major principal axis Iy (I2, Iu)
        // Moment of inertia - minor principal axis Iz (I3, Iv)
        // Torsional inertia constant I_T
        // Section warping constant I_w  - optional (7th degree of freedom)


        // Gross-cross section area
        //m_fAg = m_fb * m_fh; // Unit [m2]
        // Second moment of Area / Moment of inertia
        //m_fIy = 1f / 12f * m_fb * m_fh * m_fh * m_fh;  // Unit [m4]
        //m_fIz = 1f / 12f * m_fb * m_fb * m_fb * m_fh;  // Unit [m4]
        // Torsional constant (St. Venant Section Modulus)
        //m_fI_t = (m_fb * m_fb * m_fb * m_fh * m_fh * m_fh) / ((3.645f - (0.06f * m_fh / m_fb)) * (m_fb * m_fb + m_fh * m_fh));  // Unit [m4]

        private double m_h, m_b; // Total depth and width of section (must be defined for all section shapes)
        public double m_y_min, m_y_max, m_z_min, m_z_max;
        private double m_b_in; // Closed cross-section - internal clear dimension between internal outline points in local y-direction
        private double m_h_in; // Closed cross-section - internal clear dimension internal outline points in local z-direction

        private double m_U,
        m_A_g,
        m_A_vy,
        m_A_vz,

        m_S_y0,
        m_S_z0,
        m_I_y0,
        m_I_z0,
        m_I_yz0,

        m_S_y,
        m_I_y,
        m_i_y_radius_gyration,
        m_W_y_el,
        m_W_y_pl,
        m_f_y_plel,
        m_S_z,
        m_I_z,
        m_i_z_radius_gyration,
        m_W_z_el,
        m_W_z_pl,
        m_f_z_plel,
        m_W_y_el_1,
        m_W_y_el_2,
        m_W_z_el_1,
        m_W_z_el_2,

        m_I_epsilon,
        m_i_epsilon_radius_gyration,
        m_I_mikro,
        m_i_mikro_radius_gyration,
        m_I_yz,
        m_i_yz_radius_gyration,
        m_I_p,
        m_i_p_radius_gyration,
        m_I_p_M,
        m_i_p_M_radius_gyration,
        m_I_Omega_M,
        m_i_Omega_M_radius_gyration,

        m_W_t_el,
        m_I_t,
        m_i_t,
        m_q_t,
        m_W_t_pl,
        m_f_t_plel,
        m_I_w,
        m_W_w,
        m_Eta_y_v,
        m_A_y_v_el,
        m_A_y_v_pl,
        m_f_y_v_plel,
        m_Eta_z_v,
        m_A_z_v_el,
        m_A_z_v_pl,
        m_f_z_v_plel,
        m_alpha;

        public double S_y0
        {
            get { return m_S_y0; }
            set { m_S_y0 = value; }
        }

        public double S_z0
        {
            get { return m_S_z0; }
            set { m_S_z0 = value; }
        }

        public double I_y0
        {
            get { return m_I_y0; }
            set { m_I_y0 = value; }
        }

        public double I_z0
        {
            get { return m_I_z0; }
            set { m_I_z0 = value; }
        }

        public double I_yz0
        {
            get { return m_I_yz0; }
            set { m_I_yz0 = value; }
        }

        public double I_w
        {
            get { return m_I_w; }
            set { m_I_w = value; }
        }

        public double W_w
        {
            get { return m_W_w; }
            set { m_W_w = value; }
        }

        public double Eta_z_v
        {
            get { return m_Eta_z_v; }
            set { m_Eta_z_v = value; }
        }

        public double A_z_v_pl
        {
            get { return m_A_z_v_pl; }
            set { m_A_z_v_pl = value; }
        }

        public double A_z_v_el
        {
            get { return m_A_z_v_el; }
            set { m_A_z_v_el = value; }
        }

        public double A_y_v_pl
        {
            get { return m_A_y_v_pl; }
            set { m_A_y_v_pl = value; }
        }

        public double A_y_v_el
        {
            get { return m_A_y_v_el; }
            set { m_A_y_v_el = value; }
        }

        public double Eta_y_v
        {
            get { return m_Eta_y_v; }
            set { m_Eta_y_v = value; }
        }

        public double f_t_plel
        {
            get { return m_f_t_plel; }
            set { m_f_t_plel = value; }
        }

        public double W_t_pl
        {
            get { return m_W_t_pl; }
            set { m_W_t_pl = value; }
        }

        public double W_t_el
        {
            get { return m_W_t_el; }
            set { m_W_t_el = value; }
        }

        public double i_t
        {
            get { return m_i_t; }
            set { m_i_t = value; }
        }

        public double f_z_v_plel
        {
            get { return m_f_z_v_plel; }
            set { m_f_z_v_plel = value; }
        }

        public double f_z_plel
        {
            get { return m_f_z_plel; }
            set { m_f_z_plel = value; }
        }

        public double W_z_pl
        {
            get { return m_W_z_pl; }
            set { m_W_z_pl = value; }
        }

        public double W_z_el
        {
            get { return m_W_z_el; }
            set { m_W_z_el = value; }
        }

        public double f_y_plel
        {
            get { return m_f_y_plel; }
            set { m_f_y_plel = value; }
        }

        public double f_y_v_plel
        {
            get { return m_f_y_v_plel; }
            set { m_f_y_v_plel = value; }
        }

        public double W_y_pl
        {
            get { return m_W_y_pl; }
            set { m_W_y_pl = value; }
        }

        public double W_y_el
        {
            get { return m_W_y_el; }
            set { m_W_y_el = value; }
        }

        public double W_y_el_1
        {
            get { return m_W_y_el_1; }
            set { m_W_y_el_1 = value; }
        }

        public double W_z_el_1
        {
            get { return m_W_z_el_1; }
            set { m_W_z_el_1 = value; }
        }

        public double W_y_el_2
        {
            get { return m_W_y_el_2; }
            set { m_W_y_el_2 = value; }
        }

        public double W_z_el_2
        {
            get { return m_W_z_el_2; }
            set { m_W_z_el_2 = value; }
        }

        public double h
        {
            get { return m_h; }
            set { m_h = value; }
        }

        public double b
        {
            get { return m_b; }
            set { m_b = value; }
        }

        public double h_in
        {
            get { return m_h_in; }
            set { m_h_in = value; }
        }

        public double b_in
        {
            get { return m_b_in; }
            set { m_b_in = value; }
        }

        public double I_y
        {
            get { return m_I_y; }
            set { m_I_y = value; }
        }

        public double i_y_rg
        {
            get { return m_i_y_radius_gyration; }
            set { m_i_y_radius_gyration = value; }
        }

        public double I_z
        {
            get { return m_I_z; }
            set { m_I_z = value; }
        }

        public double i_z_rg
        {
            get { return m_i_z_radius_gyration; }
            set { m_i_z_radius_gyration = value; }
        }

        public double I_epsilon
        {
            get { return m_I_epsilon; }
            set { m_I_epsilon = value; }
        }

        public double i_epsilon_rg
        {
            get { return m_i_epsilon_radius_gyration; }
            set { m_i_epsilon_radius_gyration = value; }
        }

        public double I_mikro
        {
            get { return m_I_mikro; }
            set { m_I_mikro = value; }
        }

        public double i_mikro_rg
        {
            get { return m_i_mikro_radius_gyration; }
            set { m_i_mikro_radius_gyration = value; }
        }

        public double I_yz
        {
            get { return m_I_yz; }
            set { m_I_yz = value; }
        }

        public double i_yz_rg
        {
            get { return m_i_yz_radius_gyration; }
            set { m_i_yz_radius_gyration = value; }
        }

        public double I_p
        {
            get { return m_I_p; }
            set { m_I_p = value; }
        }

        public double i_p_rg
        {
            get { return m_i_p_radius_gyration; }
            set { m_i_p_radius_gyration = value; }
        }

        public double I_p_M
        {
            get { return m_I_p_M; }
            set { m_I_p_M = value; }
        }

        public double i_p_M_rg
        {
            get { return m_i_p_M_radius_gyration; }
            set { m_i_p_M_radius_gyration = value; }
        }

        public double I_Omega_M
        {
            get { return m_I_Omega_M; }
            set { m_I_Omega_M = value; }
        }

        public double i_Omega_M_rg
        {
            get { return m_i_Omega_M_radius_gyration; }
            set { m_i_Omega_M_radius_gyration = value; }
        }

        public double I_t
        {
            get { return m_I_t; }
            set { m_I_t = value; }
        }

        public double S_y
        {
            get { return m_S_y; }
            set { m_S_y = value; }
        }

        public double S_z
        {
            get { return m_S_z; }
            set { m_S_z = value; }
        }

        public double A_g
        {
            get { return m_A_g; }
            set { m_A_g = value; }
        }

        public double A_vy
        {
            get { return m_A_vy; }
            set { m_A_vy = value; }
        }

        public double A_vz
        {
            get { return m_A_vz; }
            set { m_A_vz = value; }
        }

        public double U
        {
            get { return m_U; }
            set { m_U = value; }
        }

        public double Alpha
        {
            get { return m_alpha; }
            set { m_alpha = value; }
        }

        public double y_min
        {
            get { return m_y_min; }
            set { m_y_min = value; }
        }

        public double y_max
        {
            get { return m_y_max; }
            set { m_y_max = value; }
        }

        public double z_min
        {
            get { return m_z_min; }
            set { m_z_min = value; }
        }

        public double z_max
        {
            get { return m_z_max; }
            set { m_z_max = value; }
        }

        public Int32Collection WireFrameIndices
        {
            get { return m_WireFrameIndices; }
            set { m_WireFrameIndices = value; }
        }

        public CMat_00 m_Mat = new CMat_00();

		//// Constructor 1
		//public CCrSc()
		//{ 
        
		//}
		//// Constructor 2
		//public CCrSc(CMat_00 objMat)
		//{
		//    m_Mat = objMat; // !!! Nevytvarat lokalne kopie !!!
		//}


		// Draw Rectangle / Add rectangle indices - clockwise CW numbering of input points 1,2,3,4 (see scheme)
		// Add in order 1,2,3,4
		protected void AddRectangleIndices_CW_1234(Int32Collection Indices,
			  int point1, int point2,
			  int point3, int point4)
		{
			// Main numbering is clockwise

			// 1  _______  2
			//   |_______| 
			// 4           3

			// Triangles Numbering is Counterclockwise
			// Top Right
			Indices.Add(point1);
			Indices.Add(point3);
			Indices.Add(point2);

			// Bottom Left
			Indices.Add(point1);
			Indices.Add(point4);
			Indices.Add(point3);
		}
        protected void AddRectangleWireFrameIndices_CW_1234(Int32Collection Indices,
              int point1, int point2,
              int point3, int point4)
        {
            // Main numbering is clockwise
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);
            Indices.Add(point4);
        }

        // Draw Rectangle / Add rectangle indices - countrer-clockwise CCW numbering of input points 1,2,3,4 (see scheme)
        // Add in order 1,4,3,2
        protected void AddRectangleIndices_CCW_1234(Int32Collection Indices,
			  int point1, int point2,
			  int point3, int point4)
		{
			// Main input numbering is clockwise, add indices counter-clockwise

			// 1  _______  2
			//   |_______| 
			// 4           3

			// Triangles Numbering is Clockwise
			// Top Right
			Indices.Add(point1);
			Indices.Add(point2);
			Indices.Add(point3);

			// Bottom Left
			Indices.Add(point1);
			Indices.Add(point3);
			Indices.Add(point4);
		}

        // Draw Triangle / Add triangle indices - clockwise CW numbering of input points 1,2,3 (see scheme)
        // Add in order 1,2,3,4
        protected void AddTriangleIndices_CW_123(Int32Collection Indices,
              int point1, int point2,
              int point3)
        {
            // Main numbering is clockwise

            // 1  _______  2
            //           | 
            //             3

            // Triangle Numbering is Counterclockwise
            Indices.Add(point1);
            Indices.Add(point3);
            Indices.Add(point2);
        }

        // Draw Triangle / Add triangle indices - countrer-clockwise CCW numbering of input points 1,2,3 (see scheme)
        // Add in order 1,3,2
        protected void AddTriangleIndices_CCW_123(Int32Collection Indices,
              int point1, int point2,
              int point3)
        {
            // Main input numbering is clockwise, add indices counter-clockwise

            // 1  _______  2
            //           | 
            //             3

            // Triangles Numbering is Clockwise
            Indices.Add(point1);
            Indices.Add(point2);
            Indices.Add(point3);
        }

        // Draw Prism CaraLaterals
        // Kresli plast hranola pre kontinualne pravidelne cislovanie bodov
        protected void DrawCaraLaterals(int secNum, Int32Collection TriangelsIndices)
		{
			// secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy

			// Shell (Face)Surface
			// Cycle for regular numbering of section points

			for (int i = 0; i < secNum; i++)
			{
				if (i < secNum - 1)
					AddRectangleIndices_CW_1234(TriangelsIndices, i, secNum + i, secNum + i + 1, i + 1);
				else
					AddRectangleIndices_CW_1234(TriangelsIndices, i, secNum + i, secNum, 0); // Last Element
			}
		}

		// Draw Prism CaraLaterals
		// Kresli plast hranola pre pravidelne cislovanie bodov s vynechanim pociatocnych uzlov - pomocne 
		protected void DrawCaraLaterals(int iAuxNum, int secNum, Int32Collection TriangelsIndices)
		{
			// iAuxNum - number of auxiliary points - start ofset
			// secNum - number of one base edges / - pocet rohov - hranicnych bodov jednej podstavy (tento pocet neobsahuje pomocne body iAuxNum)

			// Shell (Face)Surface
			// Cycle for regular numbering of section points

			for (int i = 0; i < secNum; i++)
			{
				if (i < secNum - 1)
					AddRectangleIndices_CW_1234(TriangelsIndices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum + i + 1, iAuxNum + i + 1);
				else
					AddRectangleIndices_CW_1234(TriangelsIndices, iAuxNum + i, 2 * iAuxNum + secNum + i, 2 * iAuxNum + secNum, iAuxNum + 0); // Last Element
			}
		}

		// Draw Sector of Solid Circle
		// Kresli vyrez kruhu,
		// Parametre:
		// pocet pomocnych uzlov;  ID stredu vyrezu; ID  prvy bod obluka; pocet segmentov (trojuholnikov); kolekcia, do ktorej sa zapisuju trojice, vzostupne cislovanie CW
		protected void AddSolidCircleSectorIndices(int iCentrePointID, int iArcFirstPointID, int iRadiusSegment, Int32Collection TriangelsIndices, bool bAscendingNumCW)
		{
			for (int i = 0; i < iRadiusSegment; i++)
			{
				TriangelsIndices.Add(iCentrePointID); // Centre point
				if (!bAscendingNumCW) // Clock-wise
				{
					TriangelsIndices.Add(iArcFirstPointID + 1 + i);
					TriangelsIndices.Add(iArcFirstPointID + i);
				}
				else // Counter Clock-wise
				{
					TriangelsIndices.Add(iArcFirstPointID + i);
					TriangelsIndices.Add(iArcFirstPointID + 1 + i);
				}
			}
		}

        protected abstract void loadCrScIndices();

        // New
        protected abstract void loadCrScIndicesFrontSide();
        protected abstract void loadCrScIndicesShell();
        protected abstract void loadCrScIndicesBackSide();

        public void CalculateCrScLimits(out float fTempMax_X, out float fTempMin_X, out float fTempMax_Y, out float fTempMin_Y)
        {
            fTempMax_X = float.MinValue;
            fTempMin_X = float.MaxValue;
            fTempMax_Y = float.MinValue;
            fTempMin_Y = float.MaxValue;

            if (CrScPointsOut != null) // Some points exist
            {
                for (int i = 0; i < CrScPointsOut.Length / 2; i++)
                {
                    // Maximum X - coordinate
                    if (CrScPointsOut[i, 0] > fTempMax_X)
                        fTempMax_X = CrScPointsOut[i, 0];

                    // Minimum X - coordinate
                    if (CrScPointsOut[i, 0] < fTempMin_X)
                        fTempMin_X = CrScPointsOut[i, 0];

                    // Maximum Y - coordinate
                    if (CrScPointsOut[i, 1] > fTempMax_Y)
                        fTempMax_Y = CrScPointsOut[i, 1];

                    // Minimum Y - coordinate
                    if (CrScPointsOut[i, 1] < fTempMin_Y)
                        fTempMin_Y = CrScPointsOut[i, 1];
                }
            }
        }

        public void GetGeometryCenterPointCoordinates(out float x, out float y)
        {
            float fTempMax_X, fTempMin_X, fTempMax_Y, fTempMin_Y;

            CalculateCrScLimits(out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);

            x = 0.5f * (fTempMax_X - fTempMin_X);
            y = 0.5f * (fTempMax_Y - fTempMin_Y);
        }

        public float[,] GetCoordinatesInGeometryRelatedToGeometryCenterPoint(float[,] array_in)
        {
            float[,] array_out = array_in;
            float x, y;

            GetGeometryCenterPointCoordinates(out x, out y);

            for (int i = 0; i < array_out.Length / 2; i++)
            {
                array_out[i, 0] -= x;
                array_out[i, 1] -= y;
            }

            return array_out;
        }

    }
}
