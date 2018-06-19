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
using System.Windows.Shapes;
using BaseClasses;
using BaseClasses.GraphObj;
using sw_en_GUI;

namespace PFD
{
    /// <summary>
    /// Interaction logic for System_Component_Viewer.xaml
    /// </summary>
    public partial class SystemComponentViewer : Window
    {
        public DatabaseComponents dcomponents; // Todo nahradit databazov component

        int selected_Type_Index;
        int selected_Serie_Index;
        int selected_Component_Index;

        public SystemComponentViewer()
        {
            InitializeComponent();
            dcomponents = new DatabaseComponents();

            // Database Connection

            Combobox_Type.Items.Add("Cross-section");
            Combobox_Type.Items.Add("Plate");
            Combobox_Type.Items.Add("Screw");
            Combobox_Type.SelectedIndex = 1;

            foreach (string seriename in dcomponents.arr_SeriesNames)
              Combobox_Series.Items.Add(seriename);

            foreach (string name in dcomponents.arr_Serie_L_Names)
              Combobox_Component.Items.Add(name);

            // Set data from database in to the Window textboxes
            SetDataFromDatabasetoWindow();

            // Load data from window
            LoadDataFromWindow();

            // Create Model
            CPoint controlpoint = new CPoint(0, 0, 0, 0, 0);

            float fb = dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 0] / 1000f;
            float fh = dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 1] / 1000f;
            float fl = dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 2] / 1000f;
            float ft = dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 3] / 1000f;
            int iNumberofHoles = (int)dcomponents.arr_Serie_L_Dimension[selected_Component_Index, 4];

            // Create 2D page
            WindowCrossSection2D page2D = new WindowCrossSection2D(new BaseClasses.CConCom_Plate_L(controlpoint, 1000 * fb, 1000 * fh, 1000 * fl, 1000 * ft, iNumberofHoles, true));
            // Display plate in 2D preview frame
            Frame2D.Content = page2D.Content;

            // Create 3D window
            Page3Dmodel page3D = new Page3Dmodel(new BaseClasses.CConCom_Plate_L(controlpoint, fb, fh, fl, ft, iNumberofHoles, true));
            // Display model in 3D preview frame
            Frame3D.Content = page3D;
        }

        private void SetDataFromDatabasetoWindow()
        {
            DatabaseComponents dcomponent = new DatabaseComponents(selected_Serie_Index, selected_Component_Index);
        }

        private void LoadDataFromWindow()
        {
            selected_Type_Index = Combobox_Type.SelectedIndex;
            selected_Serie_Index = Combobox_Series.SelectedIndex;
            selected_Component_Index = Combobox_Component.SelectedIndex;
        }

        private void Combobox_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

                    //UpdateAll();

        }

        private void Combobox_Series_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



        }

        private void Combobox_Component_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



        }
    }
}
