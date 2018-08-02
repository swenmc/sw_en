using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseClasses;
using CRSC;

namespace M_AS4600
{
    public partial class Form1 : Form
    {
        public float fN;
        public float fN_c;
        public float fN_t;
        public float fV_xu;
        public float fV_yv;
        public float fV_xx;
        public float fV_yy;
        public float fT;
        public float fM_xu;
        public float fM_yv;
        public float fM_xx;
        public float fM_yy;

        public CSO cso = new CSO();

        public Form1()
        {
            InitializeComponent();
        }

        private void Calculate_Click(object sender, EventArgs e)
        {
            fN = -5000f;
            fN_c = fN > 0 ? 0f : Math.Abs(fN);
            fN_t = fN < 0 ? 0f : fN;
            fV_xu = 4100;
            fV_yv = 564556;
            fV_xx = 0;
            fV_yy = 0;
            fT = 0;
            fM_xu =54548;
            fM_yv = 5454;
            fM_xx =0;
            fM_yy=0;


            cso.A_g = 2100;
            cso.I_y = 11200;
            cso.I_z = 55406;
            cso.I_yz = 12;
            cso.I_t = 5887878;
            cso.I_w = 5277778;
            cso.A_vy = 6546;
            cso.A_vz = 65465;
            cso.W_y_el = 556;
            cso.W_z_el = 564;
            cso.W_y_pl = 742;
            cso.W_z_pl = 545;

            cso.h = 0.27f;
            cso.b = 0.09f;
            cso.t_min = 0.00115;
            cso.t_max = 0.00115;

            cso.m_Mat.m_ff_yk[0] = 5e+8f;
            cso.m_Mat.m_fE = 2.1e+8f;
            cso.m_Mat.m_fG = 8.1e+7f;
            cso.m_Mat.m_fNu = 0.297f;

            cso.i_y_rg = 0.102f;
            cso.i_z_rg = 0.052f;
            cso.i_yz_rg = 0.102f;

            cso.D_y_s = 0.043f;
            cso.D_z_s = 0.0f;

            //CCalcul calculate = new CCalcul(fN, fN_c, fN_t, fV_xu, fV_yv, fT, fM_xu, fM_yv, cso);
        }
    }
}
