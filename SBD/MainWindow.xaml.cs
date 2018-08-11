using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MATH;
using BaseClasses;
using MATERIAL;
using CRSC;
using M_AS4600;
using FEM_CALC_BASE;
using M_BASE;

namespace SBD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CCrSc_TW section;
        public List<double> y_coordinates = new List<double>();
        public List<double> z_coordinates = new List<double>();
        public List<double> t_thicknesses = new List<double>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calculate_Button_Click(object sender, RoutedEventArgs e)
        {
            // Pomocny prierez pre testovanie
            CCrSc_3_50020_C sectionC_temp = new CCrSc_3_50020_C(0.5f, 0.2f,  0.001f, Colors.Orange);

            // Temporary;
            SetListValuesFromCrossSection(sectionC_temp);

            if (CoordinatesAreEqual(y_coordinates[0], z_coordinates[0], y_coordinates[y_coordinates.Count - 1], z_coordinates[z_coordinates.Count - 1])) // Closed cross-section
                section = new CSC(y_coordinates, z_coordinates, t_thicknesses);
            else
                section = new CSO(y_coordinates, z_coordinates, t_thicknesses); // Open cross-section

            // Uniform Load
            float fLoadValue_qz = 1000; // N/m
            float fLoadValue_qy =  200; // N/m

            // Beam length
            float fLength = 10f; // m

            // Calculation of internal forces and deflection
            const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
            const int iNumberOfSegments = iNumberOfDesignSections - 1;

            float[] fx_positions = new float[iNumberOfDesignSections];

            for (int i = 0; i < iNumberOfDesignSections; i++)
                fx_positions[i] = ((float)i / (float)iNumberOfSegments) * fLength; // Int must be converted to the float to get decimal numbers

            int iNumberOfLoadCombinations = 1;

            // Internal forces
            designMomentValuesForCb[] sMomentValuesforCb;
            basicInternalForces[,] sBIF_x;

            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();
            calcModel.CalculateInternalForcesOnSimpleBeam(iNumberOfDesignSections, section, fLength, fx_positions, fLoadValue_qy, fLoadValue_qz, out sBIF_x, out sMomentValuesforCb);

            // Design
            designInternalForces[,] sDIF_x;
            CMemberDesign designModel = new CMemberDesign();
            designModel.SetDesignForcesAndMemberDesign(iNumberOfLoadCombinations, iNumberOfDesignSections, section, fLength, sBIF_x, sMomentValuesforCb, out sDIF_x);

            // TODO - toto zobrazenie detailov v Gridview pre PFD a SBD treba refaktorovat a vytvorit jednotnu bazu pre zobrazovanie dat
            // v datagrid napriec roznymi projektmi

            // Display section properties
            // TODO Vlozit detaily z vypoctu prierezu do datagrid - refaktorovat s projektom CRSC - metoda DisplaySectionPropertiesInDataGrid
            // Najlepsie budes asi previest projekt CRSC na WPF

            // DisplaySectionPropertiesInDataGrid(this.DataGrid_SectionProperties, section);

            // Display results for maximum design ratio
            PFD.UC_MemberDesign b_temp = new PFD.UC_MemberDesign(); // TODO - refaktoring, tento riadkoch by tu nemal byt, vyvotirit spolocnu bazovu triedu pre vlozenie dat do DATAGRIDu
            b_temp.DisplayDesignResultsInGridView(this.DataGrid_Results, designModel.listOfMemberDesignInLocations[designModel.fMaximumDesignRatioLocationID]);
        }

        private void SetListValuesFromCrossSection(CCrSc_TW sectionTemp)
        {
            // Priblizne, mala by sa nacitavat strednica
            for(int i = 0; i < sectionTemp.INoPointsOut; i++)
            {
                y_coordinates.Add(sectionTemp.CrScPointsOut[i, 0]);
                z_coordinates.Add(sectionTemp.CrScPointsOut[i, 1]);
                t_thicknesses.Add(/*sectionTemp.t_min*/0.001); // TODO TEMPORARY
            }
        }

        private bool CoordinatesAreEqual(double y1, double z1, double y2, double z2)
        {
            if (MathF.d_equal(y1, y2) && MathF.d_equal(z1, z2))
                return true;
            else
                return false;
        }
    }
}
