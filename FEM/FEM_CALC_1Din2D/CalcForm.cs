using System;
using System.Windows.Forms;

namespace FEM_CALC_1Din2D
{
    public partial class CalcForm : Form
    {
        private bool m_bDebugging = false; // Console Output

        public CalcForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Commented - this will be deleted or modified after implementing new core (choose GUI - WPF or WINFORMs)
            //CFEM_CALC obj_Calc = new CFEM_CALC(m_bDebugging);

           // MessageBox.Show("Calculation was successful!", "Solver Message" /*, MessageBoxButtons.OK, MessageBoxIcon.Exclamation*/);


            // Auxialiary string - result data
            string sDOFResults = null;

            /*
            for (int i = 0; i < obj_Calc.m_V_Displ.Length; i++)
            {
                int iNodeNumber    = obj_Calc.m_fDisp_Vector_CN[i, 1]+1; // Incerase index (1st member "0" to "1"
                int iNodeDOFNumber = obj_Calc.m_fDisp_Vector_CN[i, 2]+1;

                sDOFResults += "Node No:" + "\t" + iNodeNumber + "\t" +
                             "Node DOF No:" + "\t" + iNodeDOFNumber + "\t" +
                             "Value:" + "\t" + obj_Calc.m_V_Displ[i]
                             + "\n";
            }
           */

           // Pokus dat to dokopy - 29.1.2019

           MATERIAL.CMat mat = new MATERIAL.CMat();
           CRSC.CCrSc_3_63020_BOX crsc1 = new CRSC.CCrSc_3_63020_BOX(1,0.63f,0.18f,0.00195f,0.00195f,System.Windows.Media.Colors.Coral);
           CRSC.CCrSc_3_63020_BOX crsc2 = new CRSC.CCrSc_3_63020_BOX(1, 0.63f, 0.18f, 0.00195f, 0.00295f, System.Windows.Media.Colors.Cyan);

           BaseClasses.CExample model = new Examples.CExample_2D_13_PF(mat,crsc1, crsc2,20f, 6f, 8f, 1f, 1f, 1f, 1f);

           CFEM_CALC objCalc = new CFEM_CALC(model, m_bDebugging);

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
