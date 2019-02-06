using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Configuration;
using _3DTools;
using BaseClasses;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private bool bDebugging;
        public DisplayOptions sDisplayOptions;
        //public 
        public EGCS eGCS = EGCS.eGCSLeftHanded;
        //EGCS eGCS = EGCS.eGCSRightHanded;

        // Tutorial
        /// http://kindohm.com/technical/WPF3DTutorial.htm  ScreenSpaceLines3D

        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        public Window2()
        {
        }

        public Window2(bool bDebugging_temp)
        {
            InitializeComponent();

            bDebugging = bDebugging_temp;

            GeometryModel3D SolidModel3D = new GeometryModel3D();
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();

            SolidModel3D.Geometry = mesh;
            SolidColorBrush br = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            SolidModel3D.Material = new DiffuseMaterial(br);

            Model3DGroup gr = new Model3DGroup();
            gr.Children.Add(SolidModel3D); // Add solid to model group

            _trackport.Model = (Model3D)gr;

            _trackport.Trackball.TranslateScale = 1000; //step for moving object (panning)

            _trackport.SetupScene();
        }

        public Window2(CModel cmodel, DisplayOptions sDisplayOptions_temp, bool bDebugging_temp)
        {
            sDisplayOptions = sDisplayOptions_temp;
            bDebugging = bDebugging_temp;

            InitializeComponent();

            Drawing3D.DrawToTrackPort(_trackport, cmodel, sDisplayOptions, null);
        }

        public Window2(CModel cmodel, DisplayOptions sDisplayOptions_temp, CLoadCase loadCaseToDisplay, bool bDebugging_temp)
        {
            sDisplayOptions = sDisplayOptions_temp;
            bDebugging = bDebugging_temp;

            InitializeComponent();

            Drawing3D.DrawToTrackPort(_trackport, cmodel, sDisplayOptions, loadCaseToDisplay);
        }
    }
}
