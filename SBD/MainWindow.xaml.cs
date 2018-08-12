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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Globalization;
using System.Data;
using MATH;
using BaseClasses;
using MATERIAL;
using CRSC;
using M_AS4600;
using FEM_CALC_BASE;
using M_BASE;
using PFD;

namespace SBD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public struct sInPutData
    {
        public int iPointID { set; get; }
        public double fy_Coordinate { set; get; }
        public double fz_Coordinate { set; get; }
        public double ft_Thickness { set; get; }
    }

    public struct sInPutDataText
    {
        public string sPointID { set; get; }
        public string sy_Coordinate { set; get; }
        public string sz_Coordinate { set; get; }
        public string st_Thickness { set; get; }
    }

    public struct sOutPutData
    {
        public string sPropertyFullName { set; get; }
        public string sPropertySymbol { set; get; }
        public string sPropertyValue { set; get; }
        public string sPropertyUnit { set; get; }
        public string sPropertyEquation { set; get; }
    }

    public partial class MainWindow : Window
    {
        CCrSc_TW section;

        public List<sInPutData> listOfInputData = new List<sInPutData>(0);
        public List<sInPutDataText> listOfInputDataText = new List<sInPutDataText>(0);
        public List<sOutPutData> listOfOutPutData = new List<sOutPutData>(0);

        public CSBDViewModel vm;

        public MainWindow()
        {
            InitializeComponent();

            // Model
            vm = new CSBDViewModel();
            vm.PropertyChanged += HandleViewModelPropertyChangedEvent;
            this.DataContext = vm;

            // Pomocny prierez pre testovanie
            CCrSc_3_50020_C sectionC_temp = new CCrSc_3_50020_C(0.5f, 0.2f, 0.00195f, Colors.Orange);

            // TODO Temporary
            // Suradnice bodov na strednici C 50020
            sectionC_temp.arrPointCoord = new float[11, 3]
            {
                {0.080f,  0.21f, 0.00195f},
                {0.099f,  0.21f, 0.00195f},
                {0.099f,  0.24f, 0.00195f},
                {0.000f,  0.24f, 0.00195f},
                {0.000f,  0.06f, 0.00195f},
                {0.039f,  0.00f, 0.00195f},
                {0.000f, -0.06f, 0.00195f},
                {0.000f, -0.24f, 0.00195f},
                {0.099f, -0.24f, 0.00195f},
                {0.099f, -0.21f, 0.00195f},
                {0.080f, -0.21f, 0.00195f}
            };

            SetListValuesFromCrossSection(sectionC_temp);

            List<string> colInputBinding = new List<string> { "sPointID", "sy_Coordinate", "sz_Coordinate", "st_Thickness" };
            List<string> colInputHeader = new List<string> { "ID", "y-coordinate", "z-coordinate", "t" };

            AddCordinateDataToDataGridRow(listOfInputDataText, 4, colInputBinding, colInputHeader, DataGrid_SectionCoordinates);
        }

        private void Calculate_Button_Click(object sender, RoutedEventArgs e)
        {
            // Temporary for constructor
            List<double> listOfyCoordinates = new List<double>();
            List<double> listOfzCoordinates = new List<double>();
            List<double> listOftCoordinates = new List<double>();

            for (int i = 0; i < listOfInputData.Count; i++)
            {
                listOfyCoordinates.Add(listOfInputData[i].fy_Coordinate);
                listOfzCoordinates.Add(listOfInputData[i].fz_Coordinate);
                listOftCoordinates.Add(listOfInputData[i].ft_Thickness);
            }

            if (CoordinatesAreEqual(listOfInputData[0].fy_Coordinate, listOfInputData[0].fz_Coordinate, listOfInputData[listOfInputData.Count - 1].fy_Coordinate, listOfInputData[listOfInputData.Count - 1].fz_Coordinate)) // Closed cross-section
                section = new CSC(listOfyCoordinates, listOfzCoordinates, listOftCoordinates);
            else
                section = new CSO(listOfyCoordinates, listOfzCoordinates, listOftCoordinates); // Open cross-section

            List<string> colInputBinding = new List<string> { "sPointID", "sy_Coordinate", "sz_Coordinate", "st_Thickness" };
            List<string> colInputHeader = new List<string> { "ID", "y-coordinate", "z-coordinate", "t" };

            AddCordinateDataToDataGridRow(listOfInputDataText, 4, colInputBinding, colInputHeader, DataGrid_SectionCoordinates);

            // Get Input data from datagrid
            //getListsFromDatagrid();

            // Display section in GUI canvas
            // Primarny projekt by mal byt SW_EN, pre GUI sw_en_GUI
            // z toho by mali cerpat projekty PFD a SBD

            // TODO - refactoring
            //sw_en_GUI.WindowCrossSection2D a = new sw_en_GUI.WindowCrossSection2D(section, Canvas_Section.Width, Canvas_Section.Height);
            //Canvas_Section = a.CanvasSection2D;

            // Calculation of internal forces and deflection
            const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
            const int iNumberOfSegments = iNumberOfDesignSections - 1;

            float[] fx_positions = new float[iNumberOfDesignSections];

            for (int i = 0; i < iNumberOfDesignSections; i++)
                fx_positions[i] = ((float)i / (float)iNumberOfSegments) * vm.Length; // Int must be converted to the float to get decimal numbers

            int iNumberOfLoadCombinations = 1;

            // Internal forces
            designMomentValuesForCb[] sMomentValuesforCb;
            basicInternalForces[,] sBIF_x;

            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();

            float fLoadUnitMultiplier = 1000; // From kN to N
            calcModel.CalculateInternalForcesOnSimpleBeam(iNumberOfDesignSections, section, vm.Length, fx_positions, vm.Loadqy * fLoadUnitMultiplier, vm.Loadqz * fLoadUnitMultiplier, out sBIF_x, out sMomentValuesforCb);

            // Design
            designInternalForces[,] sDIF_x;
            CMemberDesign designModel = new CMemberDesign();
            designModel.SetDesignForcesAndMemberDesign(iNumberOfLoadCombinations, iNumberOfDesignSections, section, vm.Length, sBIF_x, sMomentValuesforCb, out sDIF_x);

            // TODO - toto zobrazenie detailov v Gridview pre PFD a SBD treba refaktorovat a vytvorit jednotnu bazu pre zobrazovanie dat
            // v datagrid napriec roznymi projektmi

            // Display section properties
            // TODO Vlozit detaily z vypoctu prierezu do datagrid - refaktorovat s projektom CRSC - metoda DisplaySectionPropertiesInDataGrid
            // Najlepsie budes asi previest projekt CRSC na WPF a cele to refaktorovat (tabulky, vykreslovanie atd)
            FillOutPutDataList(section);

            List<string> colBinding = new List<string> { "sPropertyFullName", "sPropertySymbol", "sPropertyValue", "sPropertyUnit" };
            List<string> colHeader = new List<string> { "Name", "Symbol", "Value", "Unit" };

            AddSectionDataToDataGridRow(listOfOutPutData, 4, colBinding, colHeader, DataGrid_SectionProperties);

            // Display results for maximum design ratio
            UC_MemberDesign b_temp = new UC_MemberDesign(); // TODO - refaktoring, tento riadkoch by tu nemal byt, vyvotirit spolocnu bazovu triedu pre vlozenie dat do DATAGRIDu
            b_temp.DisplayDesignResultsInGridView(this.DataGrid_Results, designModel.listOfMemberDesignInLocations[designModel.fMaximumDesignRatioLocationID]);
        }

        protected void HandleViewModelPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDViewModel viewModel = sender as CPFDViewModel;
            if (viewModel != null && viewModel.IsSetFromCode) return; //ak je to property nastavena v kode napr. pri zmene typu modelu tak nic netreba robit
        }

        // TODO - refactoring
        // TODO  refactoring DisplaySectionPropertiesInDataGrid, CRSC (CSForm - WinForms) a FillOutPutDataList
        public void FillOutPutDataList(CCrSc_TW cs)
        {
            // Values
            // Round numerical values
            int dec_place_num1 = 1;
            int dec_place_num2 = 2;

            // TODO - zapracovat do GUI nastavenie jednotiek
            bool bDisplayInMM = true;
            bool bDisplayAnglesInDegrees = true;

            float fUnitMultilier_Dim = 1;
            string s_unit_length = "m";

            if (bDisplayInMM)
            {
                fUnitMultilier_Dim = 1000; // m to mm
                s_unit_length = "mm"; // TODO - zapracovat nastavitelne jednotky (napriec celou aplikaciou, vypocet v zakladnych jednotkach SI)
            }

            float fUnitMultiplierAngle = 1f;
            string s_unit_angle = "rad";

            if(bDisplayAnglesInDegrees)
            {
                fUnitMultiplierAngle = 180f / MathF.fPI; // rad to deg
                s_unit_angle = "deg";
            }

            float fUnitMultilier_Dim2 = MathF.Pow2(fUnitMultilier_Dim); // m^2 to mm^2
            float fUnitMultilier_Dim3 = MathF.Pow3(fUnitMultilier_Dim); // m^3 to mm^3
            float fUnitMultilier_Dim4 = MathF.Pow4(fUnitMultilier_Dim); // m^4 to mm^4
            float fUnitMultilier_Dim6 = MathF.Pow6(fUnitMultilier_Dim); // m^6 to mm^6

            double d_A = Math.Round(cs.A_g * fUnitMultilier_Dim2, dec_place_num2);
            double d_A_vy = Math.Round(cs.A_vy * fUnitMultilier_Dim2, dec_place_num2);
            double d_A_vz = Math.Round(cs.A_vz * fUnitMultilier_Dim2, dec_place_num2);
            double d_y_gc = Math.Round(cs.D_y_gc * fUnitMultilier_Dim, dec_place_num2);
            double d_z_gc = Math.Round(cs.D_z_gc * fUnitMultilier_Dim, dec_place_num2);
            double d_S_y0 = Math.Round(cs.S_y0 * fUnitMultilier_Dim3, dec_place_num2);
            double d_S_z0 = Math.Round(cs.S_z0 * fUnitMultilier_Dim3, dec_place_num2);
            double d_I_y = Math.Round(cs.I_y * fUnitMultilier_Dim4, dec_place_num2);
            double d_I_z = Math.Round(cs.I_z * fUnitMultilier_Dim4, dec_place_num2);
            double d_Wy_el_1 = Math.Round(cs.W_y_el_1 * fUnitMultilier_Dim3, dec_place_num2);
            double d_Wz_el_1 = Math.Round(cs.W_z_el_1 * fUnitMultilier_Dim3, dec_place_num2);
            double d_Wy_el_2 = Math.Round(cs.W_y_el_2 * fUnitMultilier_Dim3, dec_place_num2);
            double d_Wz_el_2 = Math.Round(cs.W_z_el_2 * fUnitMultilier_Dim3, dec_place_num2);

            double d_Alpha = Math.Round(cs.Alpha_rad * fUnitMultiplierAngle, dec_place_num2);
            double d_I_yz = Math.Round(cs.I_yz * fUnitMultilier_Dim4, dec_place_num2);
            double d_I_eps = Math.Round(cs.I_epsilon * fUnitMultilier_Dim4, dec_place_num2);
            double d_I_eta = Math.Round(cs.I_mikro * fUnitMultilier_Dim4, dec_place_num2);
            double d_I_ome = Math.Round(cs.Iomega * fUnitMultilier_Dim6, dec_place_num2);
            double d_ome_mean = Math.Round(cs.Omega_mean * fUnitMultilier_Dim2, dec_place_num2);
            double d_ome_max = Math.Round(cs.Omega_max * fUnitMultilier_Dim2, dec_place_num2);
            double d_I_y_ome = Math.Round(cs.Iy_omega * fUnitMultilier_Dim6, dec_place_num2);
            double d_I_z_ome = Math.Round(cs.Iz_omega * fUnitMultilier_Dim6, dec_place_num2);
            double d_I_ome_ome = Math.Round(cs.Iomega_omega, dec_place_num2);

            double d_y_s = Math.Round(cs.D_y_s * fUnitMultilier_Dim, dec_place_num2);
            double d_z_s = Math.Round(cs.D_z_s * fUnitMultilier_Dim, dec_place_num2);
            double d_I_p = Math.Round(cs.Ip * fUnitMultilier_Dim4, dec_place_num2);
            double d_y_j = Math.Round(cs.D_y_j * fUnitMultilier_Dim, dec_place_num2);
            double d_z_j = Math.Round(cs.D_z_j * fUnitMultilier_Dim, dec_place_num2);
            double d_I_w = Math.Round(cs.I_w * fUnitMultilier_Dim6, dec_place_num2);
            double d_W_w = Math.Round(cs.W_w * fUnitMultilier_Dim4, dec_place_num2);
            double d_I_t = Math.Round(cs.I_t * fUnitMultilier_Dim4, dec_place_num2);
            double d_W_t = Math.Round(cs.W_t_el * fUnitMultilier_Dim3, dec_place_num2);

            double d_Beta_y = Math.Round(cs.Beta_y * fUnitMultilier_Dim, dec_place_num2);
            double d_Beta_z = Math.Round(cs.Beta_z * fUnitMultilier_Dim, dec_place_num2);

            // Units
            string s_sup_2 = "\xB2";
            string s_sup_3 = "\xB3";
            string s_sup_4 = "\u2074";
            string s_sup_6 = "\u2076";

            string s_unit_area = s_unit_length + s_sup_2;
            string s_unit_first_moment_of_area = s_unit_length + s_sup_3;
            string s_unit_second_moment_of_area = s_unit_length + s_sup_4;
            string s_unit_moment_omega = s_unit_length + s_sup_6;

            listOfOutPutData.Clear(); // Clear list

            listOfOutPutData.Add(SetOutPutDataProperties("Area", "Ag =", d_A.ToString(), s_unit_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Shear area", "Avy =", d_A_vy.ToString(), s_unit_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Shear area", "Avz =", d_A_vz.ToString(), s_unit_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Centroid position", "ygc =", d_y_gc.ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("First moment of area", "SyO =", d_S_y0.ToString(), s_unit_first_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Moment of intertia", "Iy =", d_I_y.ToString(), s_unit_second_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Centroid position", "zgc =", d_z_gc.ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("First moment of area", "SzO =", d_S_z0.ToString(), s_unit_first_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Moment of intertia", "Iz =", d_I_z.ToString(), s_unit_second_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Elastic modulus", "Wyel1 =", d_Wy_el_1.ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Elastic modulus", "Wzel1 =", d_Wz_el_1.ToString(), s_unit_first_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Elastic modulus", "Wyel2 =", d_Wy_el_2.ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Elastic modulus", "Wzel2 =", d_Wz_el_2.ToString(), s_unit_first_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Angle of principal axes", "α =", d_Alpha.ToString(), s_unit_angle, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Product moment of area", "Iyz =", d_I_yz.ToString(), s_unit_second_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Moment of intertia", "Iξ =", d_I_eps.ToString(), s_unit_second_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Moment of intertia", "Iη =", d_I_eta.ToString(), s_unit_second_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Moment of intertia", "Iω =", d_I_ome.ToString(), s_unit_moment_omega, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Mean sectorial coordinate", "ω mean =", d_ome_mean.ToString(), s_unit_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Maximum sectorial coordinate", "ω max =", d_ome_max.ToString(), s_unit_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Sectorial constant", "Iyω =", d_I_y_ome.ToString(), s_unit_moment_omega, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Sectorial constant", "Izω =", d_I_z_ome.ToString(), s_unit_moment_omega, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Sectorial constant", "Iωω =", d_I_ome_ome.ToString(), s_unit_moment_omega, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Shear centre distance from centroid", "ys =", d_y_s.ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Shear centre position from centroid", "zs =", d_z_s.ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Polar moment of area", "Ip =", d_I_p.ToString(), s_unit_second_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Non-symmetry factor", "yj =", d_y_j.ToString().ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Non-symmetry factor", "zj =", d_z_j.ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Warping constant", "Iw =", d_I_w.ToString(), s_unit_moment_omega, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Warping modulus", "Ww =", d_W_w.ToString(), s_unit_first_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Torsion constant", "It =", d_I_t.ToString(), s_unit_second_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Torsion modulus", "Wt =", d_W_t.ToString(), s_unit_first_moment_of_area, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Monosymmetry factor", "βy =", d_Beta_y.ToString(), s_unit_length, ""));
            listOfOutPutData.Add(SetOutPutDataProperties("Monosymmetry factor", "βz =", d_Beta_z.ToString(), s_unit_length, ""));
        }

        private sInPutData SetInPutDataProperties(int iPoint, double y, double z, double t)
        {
            sInPutData sData = new sInPutData();
            sData.iPointID = iPoint;
            sData.fy_Coordinate = y;
            sData.fz_Coordinate = z;
            sData.ft_Thickness = t;

            return sData;
        }

        private sInPutDataText SetInPutDataProperties(string iPoint, string y, string z, string t)
        {
            sInPutDataText sData = new sInPutDataText();
            sData.sPointID = iPoint;
            sData.sy_Coordinate = y;
            sData.sz_Coordinate = z;
            sData.st_Thickness = t;

            return sData;
        }

        private sOutPutData SetOutPutDataProperties(string name, string symbol, string value, string unit, string equation)
        {
            sOutPutData sData = new sOutPutData();
            sData.sPropertyFullName = name;
            sData.sPropertySymbol = symbol;
            sData.sPropertyValue = value;
            sData.sPropertyUnit = unit;
            sData.sPropertyEquation = equation;

            return sData;
        }

        // TODO  - refactoring (iny typ zoznamu vstupnej struktury)
        private void AddSectionDataToDataGridRow(List<sOutPutData> listOfData, int iNumberOfColumns, List<string> sColumnBinding, List<string> sColumnHeader, DataGrid dataGrid)
        {
            dataGrid.Items.Clear();

            if (dataGrid.Columns != null && dataGrid.Columns.Count > 0) // Only in case that there are some columns remove them
            {
                for(int i = 0; i < iNumberOfColumns; i++)
                    dataGrid.Columns.RemoveAt(0); // 4 columns (change of indexes after deleting of column)
            }

            if (dataGrid.Columns.Count == 0) // In case there are no columns in datagrid
            {
                // Create columns
                for (int i = 0; i < iNumberOfColumns; i++)
                {
                    DataGridTextColumn col = new DataGridTextColumn();
                    dataGrid.Columns.Add(col);
                    col.Binding = new Binding(sColumnBinding[i]);
                    col.Header = sColumnHeader[i];
                }

                // Add Data
                for (int i = 0; i < listOfData.Count; i++)
                {
                    dataGrid.Items.Add(listOfData[i]);
                }
            }
        }

        // TODO - refactoring (iny typ zoznamu vstupnej struktury)
        private void AddCordinateDataToDataGridRow(List<sInPutDataText> listOfData, int iNumberOfColumns, List<string> sColumnBinding, List<string> sColumnHeader, DataGrid dataGrid)
        {
            dataGrid.Items.Clear();

            if (dataGrid.Columns != null && dataGrid.Columns.Count > 0) // Only in case that there are some columns remove them
            {
                for (int i = 0; i < iNumberOfColumns; i++)
                    dataGrid.Columns.RemoveAt(0); // 4 columns (change of indexes after deleting of column)
            }

            if (dataGrid.Columns.Count == 0) // In case there are no columns in datagrid
            {
                // Create columns
                for (int i = 0; i < iNumberOfColumns; i++)
                {
                    DataGridTextColumn col = new DataGridTextColumn();
                    dataGrid.Columns.Add(col);
                    col.Binding = new Binding(sColumnBinding[i]);
                    col.Header = sColumnHeader[i];
                }

                // Add Data
                for (int i = 0; i < listOfData.Count; i++)
                {
                    dataGrid.Items.Add(listOfData[i]);
                }
            }
        }

        private void SetListValuesFromCrossSection(CCrSc_TW sectionTemp)
        {
            int iNumberOfDigits = 2;
            float fUnitFactor = 1000; // m to mm

            for(int i = 0; i < sectionTemp.arrPointCoord.Length / 3; i++)
            {
                listOfInputData.Add(SetInPutDataProperties(i+1, sectionTemp.arrPointCoord[i, 0], sectionTemp.arrPointCoord[i, 1], sectionTemp.arrPointCoord[i, 2]));

                listOfInputDataText.Add(SetInPutDataProperties(
                    listOfInputData[i].iPointID.ToString(),
                    Math.Round(listOfInputData[i].fy_Coordinate * fUnitFactor, iNumberOfDigits).ToString(),
                    Math.Round(listOfInputData[i].fz_Coordinate  * fUnitFactor, iNumberOfDigits).ToString(),
                    Math.Round(listOfInputData[i].ft_Thickness * fUnitFactor, iNumberOfDigits).ToString()));
            }
        }

        private bool CoordinatesAreEqual(double y1, double z1, double y2, double z2)
        {
            if (MathF.d_equal(y1, y2) && MathF.d_equal(z1, z2))
                return true;
            else
                return false;
        }

        private void getListsFromDatagrid()
        {
            int id;
            double y, z, t;
            listOfInputData.Clear();
            listOfInputDataText.Clear();

            for (int i = 0; i < DataGrid_SectionCoordinates.Items.Count - 1; i++)
            {
                string s = (DataGrid_SectionCoordinates.Items[i] as DataRowView).Row.ItemArray[0].ToString(); // TODO - bug nefunguje to takto

                id = Convert.ToInt32((DataGrid_SectionCoordinates.Items[i] as DataRowView).Row.ItemArray[0].ToString());
                y = Convert.ToDouble((DataGrid_SectionCoordinates.Items[i] as DataRowView).Row.ItemArray[1].ToString().Replace(",", "."), new CultureInfo("en-us"));
                z = Convert.ToDouble((DataGrid_SectionCoordinates.Items[i] as DataRowView).Row.ItemArray[2].ToString().Replace(",", "."), new CultureInfo("en-us"));
                t = Convert.ToDouble((DataGrid_SectionCoordinates.Items[i] as DataRowView).Row.ItemArray[3].ToString().Replace(",", "."), new CultureInfo("en-us"));

                listOfInputData.Add(SetInPutDataProperties(id, y, z, t));
            }
        }

        private void DataGrid_SectionCoordinates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getListsFromDatagrid(); // TODO - Bug pada to na nacitani poloziek z datagrid
        }
    }
}
