using System;
using System.Windows.Forms;
using Crownwood.Magic.Docking;
using Crownwood.Magic.Common;
using CENEX.TEMPORARY;
using DATABASE;
using M_EC1;
using M_EC2;
using M_EC4;
using M_EC5;
using M_EC6;
using M_EC7;
using CRSC;

namespace CENEX
{
    public partial class MainForm : Form
    {
        DatabaseConnection dat_conn;
        DockingManager _manager;
        Content c;
        public MainForm()
        {

            InitializeComponent();
            

            ////////////////////////////////////////////////////////////////////////////////////////////
            // Open File Data and Program Database
            ////////////////////////////////////////////////////////////////////////////////////////////
            dat_conn = DatabaseConnection.getInstance();
            


            /////////////////////////////////////////////////////////////////////////////////////////////
            // MainTree and Table Tabpage
            /////////////////////////////////////////////////////////////////////////////////////////////
            //docking
            _manager = new DockingManager(this, VisualStyle.IDE);
            // Create Content which contains a RichTextBox
            c = _manager.Contents.Add(new TreeForm(), "tree menu");
            _manager.AddContentWithState(c, State.DockLeft);



            ////////////////////////////////////////////////////////////////////////////////////////////
            // Main WorkSpace - Show main working window - display model graphics
            ////////////////////////////////////////////////////////////////////////////////////////////

            // ShowModelGraphicWindow();
        

        
        
        }
        

        private void treeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _manager.AddContentWithState(c, State.DockLeft);
        }

        private void steelInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EN1993_1_1Form inStForm = new EN1993_1_1Form();
            inStForm.ShowDialog();
        }
        private void TZBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TZBForm iTZB = new TZBForm();
            iTZB.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            EN1993_1_1Form i = new EN1993_1_1Form();
            i.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            EN1993_1_1Form i = new EN1993_1_1Form();
            i.ShowDialog();
        }
        private void crossSectionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            CSForm iCSForm = new CSForm();
            iCSForm.ShowDialog();
        }

        private void en1999ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EN1999_1_1Form inAlForm = new EN1999_1_1Form();
            inAlForm.ShowDialog();
        }

        private void fontsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1_specimens a = new Form1_specimens();
            a.ShowDialog();
        }
        private void eC1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_M_EC1 obj_M_C1_Form = new Form_M_EC1();
            obj_M_C1_Form.ShowDialog();
        }
        private void eC2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_M_EC2 obj_EN1992_1_1Form = new Form_M_EC2();
            obj_EN1992_1_1Form.ShowDialog();
        }
        private void eC3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EN1993_1_1Form obj_EN1993_1_1Form = new EN1993_1_1Form ();
            obj_EN1993_1_1Form.ShowDialog();
        }
        private void eC4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_M_EC4 obj_M_C4_Form = new Form_M_EC4();
            obj_M_C4_Form.ShowDialog();
        }
        private void eC5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_M_EC5 obj_M_C5_Form = new Form_M_EC5();
            obj_M_C5_Form.ShowDialog();
        }
        private void eC6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_M_EC6 obj_M_C6_Form = new Form_M_EC6();
            obj_M_C6_Form.ShowDialog();
        }
        private void eC7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_M_EC7 obj_M_C7_Form = new Form_M_EC7();
            obj_M_C7_Form.ShowDialog();
        }
        private void eC8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form_M_EC8 obj_M_C8_Form = new Form_M_EC8();
            //obj_M_C8_Form.ShowDialog();
        }
        private void eC9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EN1999_1_1Form obj_EN1999_1_1Form = new EN1999_1_1Form();
            obj_EN1999_1_1Form.ShowDialog();
        }

        private void paintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PaintForm obj_PaintForm = new PaintForm();
            obj_PaintForm.ShowDialog();
        }

        private void materialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
           //Data_MatForm obj_Data_MatForm = new Data_MatForm();
           // obj_Data_MatForm.ShowDialog();
        }

        private void crosssectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data_CSForm obj_Data_CSForm = new Data_CSForm();
            obj_Data_CSForm.ShowDialog();
        }

        private void eC3ConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EN1993_1_8Form_PIN obj_EN1993_1_8Form_PIN = new EN1993_1_8Form_PIN();
            obj_EN1993_1_8Form_PIN.ShowDialog();
        }

        private void temporaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGrid_Ukazky obj_DataGrid_Ukazky = new DataGrid_Ukazky();
            obj_DataGrid_Ukazky.ShowDialog();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            WindowsFormsApplication1.Form1 obj_Form1 = new WindowsFormsApplication1.Form1();
            obj_Form1.ShowDialog();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            PaintForm2 obj_Form1 = new PaintForm2();
            obj_Form1.ShowDialog();

            /*
            // Create / Open Model Graphic Window - Display Model Topology
            PaintForm2 obj_ModelGraphic = new PaintForm2();
            obj_ModelGraphic.ShowDialog(); // Show Form Dialog
            */
        }

        private void ShowModelGraphicWindow()
        {
            // Create / Open Model Graphic Window - Display Model Topology
            PaintForm2 obj_ModelGraphic = new PaintForm2();
            obj_ModelGraphic.ShowDialog(); // Show Form Dialog
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void dSolutionToolStripMenuItem3D_Click(object sender, EventArgs e)
        {
            FEM_CALC_1Din3D.CalcForm obj_Calc = new FEM_CALC_1Din3D.CalcForm();
            obj_Calc.ShowDialog();
        }

        private void dSolutionToolStripMenuItem2D_Click(object sender, EventArgs e)
        {
            FEM_CALC_1Din2D.CalcForm obj_Calc = new FEM_CALC_1Din2D.CalcForm();
            obj_Calc.ShowDialog();
        }

        private void iMPORTDXFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DXFImportForm f = new DXFImportForm();
            f.ShowDialog();
        }

        

        
        

        
    }
}
