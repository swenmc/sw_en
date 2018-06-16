using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FEM_CALC_1Din3D
{
    public partial class CalcForm : Form
    {
        public CalcForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CFEM_CALC obj_Calc = new CFEM_CALC();

           // MessageBox.Show("Calculation was successful!", "Solver Message" /*, MessageBoxButtons.OK, MessageBoxIcon.Exclamation*/);


            // Auxialiary string - result data

            int iDispDecPrecision = 3; // Precision of numerical values of displacement and rotations
            string sDOFResults = null;

            for (int i = 0; i < obj_Calc.m_V_Displ.FVectorItems.Length; i++)
            {
                int iNodeNumber    = obj_Calc.m_fDisp_Vector_CN[i, 1]+1; // Incerase index (1st member "0" to "1"
                int iNodeDOFNumber = obj_Calc.m_fDisp_Vector_CN[i, 2]+1;

                sDOFResults += "Node No:" + "\t" + iNodeNumber + "\t" +
                               "Node DOF No:" + "\t" + iNodeDOFNumber + "\t" +
                               "Value:" + "\t" + String.Format("{0:0.000}" ,Math.Round(obj_Calc.m_V_Displ.FVectorItems[i], iDispDecPrecision))
                               + "\n";
            }

            // Main String
           string sMessageCalc=
               "Calculation was successful!" +"\n\n"+
               "Result - vector of calculated values of unrestraint DOF displacement or rotation" + "\n\n" + sDOFResults;

            // Display Message
           MessageBox.Show(sMessageCalc, "Solver Message", MessageBoxButtons.OK);
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
