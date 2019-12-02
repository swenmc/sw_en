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

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_Quotation.xaml
    /// </summary>
    public partial class UC_Quotation : UserControl
    {
        public UC_Quotation(CPFDViewModel vm)
        {
            InitializeComponent();

            CModel model = vm.Model;
            // Members

            // TODO Ondrej
            // Sem dat members podla Crsc, alebo podla MemberType_FS alebo podla MemberType_FS_Position
            // V jednom riadku spocitat dlzku pre vsetky pruty daneho typu

            // Cross-section | Total Length [m] | Price PLM | Total Price
            // 270115 | 245.54 | 4.54 | Total Price

            // Plates

            // TODO Ondrej - sem dat Plates presne ako su v UC_Material

            // TODO - dopracovat apex brace plates

            // Screws

            // Anchors

            // Washers

            // Bolts


            // Doors and windows
            float fTotalAreaOfOpennings = 0;

            float fRollerDoorTrimmerFlashing_TotalLength = 0;
            float fRollerDoorLintelFlashing_TotalLength = 0;
            float fRollerDoorLintelCapFlashing_TotalLength = 0;
            float fPADoorTrimmerFlashing_TotalLength = 0;
            float fPADoorLintelFlashing_TotalLength = 0;
            float fWindowFlashing_TotalLength = 0;

            foreach (DoorProperties dp in vm.DoorBlocksProperties)
            {
                fTotalAreaOfOpennings += dp.fDoorsWidth * dp.fDoorsHeight;

                if (dp.sDoorType == "Roller Door")
                {
                    fRollerDoorTrimmerFlashing_TotalLength += (dp.fDoorsHeight * 2); 
                    fRollerDoorLintelFlashing_TotalLength += dp.fDoorsWidth;
                    fRollerDoorLintelCapFlashing_TotalLength = dp.fDoorsWidth;
                }
                else
                {
                    fPADoorTrimmerFlashing_TotalLength += (dp.fDoorsHeight * 2);
                    fPADoorLintelFlashing_TotalLength += dp.fDoorsWidth;
                }
            }

            foreach (WindowProperties wp in vm.WindowBlocksProperties)
            {
                fTotalAreaOfOpennings += wp.fWindowsWidth * wp.fWindowsHeight;

                fWindowFlashing_TotalLength += (2 * wp.fWindowsWidth + 2 * wp.fWindowsHeight);
            }

            // Cladding

            List<Point> fWallDefinitionPoints_Left = new List<Point>(4) { new Point(0, 0), new Point(model.fL_tot, 0), new Point(model.fL_tot, model.fH1_frame), new Point(0, model.fH1_frame) };
            List<Point> fWallDefinitionPoints_Front = new List<Point>(5) { new Point(0, 0), new Point(model.fW_frame, 0), new Point(model.fW_frame, model.fH1_frame), new Point(0.5 * model.fW_frame, model.fH2_frame), new Point(0, model.fH1_frame) };

            float fWallArea_Left = MATH.Geom2D.PolygonArea(fWallDefinitionPoints_Left.ToArray());
            float fWallArea_Right = fWallArea_Left;

            // Tieto plochy by sa mali zohladnovat len ak su zapnute girt na prislusnych stranach - bGenerate
            float fWallArea_Front = MATH.Geom2D.PolygonArea(fWallDefinitionPoints_Front.ToArray());
            float fWallArea_Back = fWallArea_Front;

            float fWallArea_Total = fWallArea_Left + fWallArea_Right + fWallArea_Front + fWallArea_Back;

            // Dlzka hrany strechy
            float fRoofSideLength = MathF.Sqrt(MathF.Pow2(model.fH2_frame - model.fH1_frame) + MathF.Pow2(0.5f * model.fW_frame));

            // Tato plocha by sa mala zohladnovat len ak su zapnute purlins - bGenerate

            float fRoofArea = 2 * fRoofSideLength * model.fL_tot;

            float fFibreGlassArea_Roof = 0.2f * fRoofArea; // Priesvitna cast strechy TODO Percento pre fibre glass zadavat zatial v GUI, mozeme zadavat aj pocet a velkost fibreglass tabul
            float fFibreGlassArea_Walls = 0.05f * fWallArea_Total; // Priesvitna cast strechy TODO Percento zadavat zatial v GUI, mozeme zadavat aj pocet a velkost fibreglass tabul

            // Plocha stien bez otvorov a fibre glass
            float fWallArea_Total_Netto = fWallArea_Total - fTotalAreaOfOpennings - fFibreGlassArea_Walls;

            // Plocha strechy bez fibre glass
            float fRoofArea_Total_Netto = fRoofArea - fFibreGlassArea_Roof;

            float fRoofCladdingPrice_PSM_NZD = 5.20f; // Cena roof cladding za 1 m^2 // TODO - zapracovat do databazy
            float fWallCladdingPrice_PSM_NZD = 4.20f; // Cena wall cladding za 1 m^2 // TODO - zapracovat do databazy

            // TODO Ondrej
            // Zobrazit Datagrid s 2 riadkami - wall cladding a roof cladding - zobrazit nazov, hrubku, farbu (vid UC_General), celkovu plochu, cenu za meter stvorcovy a celkovu cenu
            // Cladding | Thickness | Color | Total Area | Price PSM | Total Price
            // PurlinDek | 0.75 mm | Titania | 324.4 | 5.20 | Total Price
            // SmartDek  | 0.55 mm | Black   | 245.9 | 4.20 | Total Price
            float fRoofCladdingPrice_Total_NZD = fRoofArea_Total_Netto * fRoofCladdingPrice_PSM_NZD; // TODO Ondrej
            float fWallCladdingPrice_Total_NZD = fWallArea_Total_Netto * fWallCladdingPrice_PSM_NZD; // TODO Ondrej

            // Gutters
            float fGuttersTotalLength = 2 * model.fL_tot; // na 2 okrajoch strechy
            float fRoofGutterPrice_PLM_NZD = 2.20f; // Cena roof gutter za 1 m dlzky // TODO - zapracovat do databazy podla sirok

            // TODO Ondrej
            // Zobrazit Datagrid
            // Roof Gutter | Total Length | Price PLM | Total Price
            float fGuttersPrice_Total_NZD = fGuttersTotalLength * fRoofGutterPrice_PLM_NZD; // TODO Ondrej

            // FibreGlass
            // TODO Ondrej
            // Zobrazit Datagrid s 2 riadkami
            // FibreGlass Roof | Total Area | Price PSM | Total Price
            // FibreGlass Walls | Total Area | Price PSM | Total Price

            float fRoofFibreGlassPrice_PSM_NZD = 10.40f; // Cena roof fibreglass za 1 m^2 // TODO - zapracovat do databazy
            float fWallFibreGlassPrice_PSM_NZD = 9.10f; // Cena wall fibreglass za 1 m^2 // TODO - zapracovat do databazy

            float fRoofFibreGlassPrice_Total_NZD = fFibreGlassArea_Roof * fRoofFibreGlassPrice_PSM_NZD; // TODO Ondrej
            float fWallFibreGlassPrice_Total_NZD = fFibreGlassArea_Walls * fWallFibreGlassPrice_PSM_NZD; // TODO Ondrej

            // Roof Netting and Sisalation
            // Roof Sisalation Foil
            // Roof Safe Net

            float fRoofSisalationFoilPrice_PSM_NZD = 0.90f; // Cena roof foil za 1 m^2 // TODO - zapracovat do databazy
            float fRoofSafeNetPrice_PSM_NZD = 1.10f; // Cena roof net za 1 m^2 // TODO - zapracovat do databazy

            // TODO Ondrej
            // Zobrazit Datagrid s 2 riadkami
            // Roof Sisalation Foil | Total Area | Price PSM | Total Price
            // Roof Safe Net | Total Area | Price PSM | Total Price
            float fRoofSisalationFoilPrice_Total_NZD = fRoofArea * fRoofSisalationFoilPrice_PSM_NZD; // TODO Ondrej
            float fRoofSafeNetPrice_Total_NZD = fRoofArea * fRoofSafeNetPrice_PSM_NZD; // TODO Ondrej

            // Flashing and Packers
            float fRoofRidgeFlashingPrice_PLM_NZD = 3.90f; // Cena roof ridge flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fWallCornerFlashingPrice_PLM_NZD = 2.90f; // Cena corner flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fBargeFlashingPrice_PLM_NZD = 3.90f; // Cena barge flashing za 1 m dlzky // TODO - zapracovat do databazy

            float fRollerDoorTrimmerFlashingPrice_PLM_NZD = 3.90f; // Cena roller door trimmer flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fRollerDoorLintelFlashingPrice_PLM_NZD = 3.80f; // Cena roller door lintel flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fRollerDoorLintelCapFlashingPrice_PLM_NZD = 1.90f; // Cena cap flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fPADoorTrimmerFlashingPrice_PLM_NZD = 1.90f; // Cena PA door trimmer flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fPADoorLintelFlashingPrice_PLM_NZD = 1.80f; // Cena PA door lintel flashing za 1 m dlzky // TODO - zapracovat do databazy
            float fWindowFlashingPrice_PLM_NZD = 1.90f; // Cena window flashing za 1 m dlzky // TODO - zapracovat do databazy

            // Zobrazit Datagrid
            // Flashing | Total Length | Price PLM | Total Price
            // Roof Ridge Flashing | 41.12 | 3.90 | Total Price
            float fRoofRidgeFlashingPrice_Total_NZD = model.fL_tot * fRoofRidgeFlashingPrice_PLM_NZD; // TODO Ondrej
            float fWallCornerFlashingPrice_Total_NZD = 4 * model.fH1_frame * fWallCornerFlashingPrice_PLM_NZD; // TODO Ondrej
            float fBargeFlashingFlashingPrice_Total_NZD = 4 * fRoofSideLength * fBargeFlashingPrice_PLM_NZD; // TODO Ondrej

            float fRollerDoorTrimmerFlashingPrice_Total_NZD = fRollerDoorTrimmerFlashing_TotalLength * fRollerDoorTrimmerFlashingPrice_PLM_NZD;
            float fRollerDoorLintelFlashingPrice_Total_NZD = fRollerDoorLintelFlashing_TotalLength * fRollerDoorLintelFlashingPrice_PLM_NZD;
            float fRollerDoorLintelCapFlashingPrice_Total_NZD = fRollerDoorLintelCapFlashing_TotalLength * fRollerDoorLintelCapFlashingPrice_PLM_NZD;
            float fPADoorTrimmerFlashingPrice_Total_NZD = fPADoorTrimmerFlashing_TotalLength * fPADoorTrimmerFlashingPrice_PLM_NZD;
            float fPADoorLintelFlashingPrice_Total_NZD = fPADoorLintelFlashing_TotalLength * fPADoorLintelFlashingPrice_PLM_NZD;
            float fWindowFlashingPrice_Total_NZD = fWindowFlashing_TotalLength * fWindowFlashingPrice_PLM_NZD;

            // Footing pads

            // Floor Slab

            // Perimeters






        }
    }
}
