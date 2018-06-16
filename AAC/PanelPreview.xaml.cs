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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using BaseClasses;
using BaseClasses.GraphObj;
using _3DTools;
using MATH;

namespace AAC
{
    /// <summary>
    /// Interaction logic for PanelPreview.xaml
    /// </summary>
    public partial class PanelPreview : Page
    {
        AAC_Panel obj_panel = new AAC_Panel();

        //--------------------------------------------------------------------------------------------
        public PanelPreview(AAC_Panel panel)
        {
            obj_panel = panel;

            InitializeComponent();

            CreatePreviewModel();
        }

        //--------------------------------------------------------------------------------------------
        public void CreatePreviewModel()
        {
            // Default view
            Point3D lookAtPoint = new Point3D(1, 0, -0.5);

            //!!!!!!! POZOR PRIEHLADNOST ZAVISI NA PORADI VYKRESLOVANIA OBJEKTOV!!!!!!!!!
            bool bIsReinfocementSurfaceTransparent = false;
            bool bIsPanelSurfaceTransparent = true;
            bool bDisplayReinforcement = true; // Display reinforcement mesh
            bool bDisplayConcretePanel = true; // Display concrete
            bool bDisplayConcretePanel_WireFrame = true; // Display Wireframe
            bool bSeeFrontSide = true; // See front side or back side of panel

            Point3D camera_position;

            if (bSeeFrontSide)
                camera_position = new Point3D(-0.8, 0.4, -0.1); // FrontSide of Panel
            else
                camera_position = new Point3D(obj_panel.fL + 0.8, 0.4, -0.1); // BackSide of Panel // TODO - ERROR - nevykresluje sa zadne celo panela ani vyztuze ?????

            _trackport.PerspectiveCamera.Position = camera_position;
            LookAt(_trackport.PerspectiveCamera, lookAtPoint);

            //OrthographicCamera camera = new OrthographicCamera(); // Doplnit moznost zobrazit v pravouhlom priemietani

            // Light Direction
            _trackport.Light.Direction = new Vector3D(-1, -10, 0);

            // Define a lighting model
            //DirectionalLight qLight = new DirectionalLight();
            //qLight.Color = Colors.White;
            //qLight.Direction = new Vector3D(-0.5, -0.25, -0.5);

            Model3DGroup model = new Model3DGroup();

            EGCS eGCS = EGCS.eGCSLeftHanded;

            // Reinforcement
            if (bDisplayReinforcement)
            {
                SolidColorBrush brRein_l1 = new SolidColorBrush(Color.FromRgb(255, 150, 0)); // Material color - Front Side
                SolidColorBrush brRein_l2 = new SolidColorBrush(Color.FromRgb(255, 150, 0)); // Material color - Shell
                SolidColorBrush brRein_l3 = new SolidColorBrush(Color.FromRgb(255, 150, 0)); // Material color - Back Side

                SolidColorBrush brRein_u1 = new SolidColorBrush(Color.FromRgb(250, 230, 50)); // Material color - Front Side
                SolidColorBrush brRein_u2 = new SolidColorBrush(Color.FromRgb(250, 230, 50)); // Material color - Shell
                SolidColorBrush brRein_u3 = new SolidColorBrush(Color.FromRgb(250, 230, 50)); // Material color - Back Side

                double y1_lower = 0.102;

                for (int i = 0; i < obj_panel.Long_Bottom_Bars_Array.Length; i++)
                    model.Children.Add(obj_panel.Long_Bottom_Bars_Array[i].GetMemberModel(eGCS, bIsReinfocementSurfaceTransparent, new Point3D(obj_panel.fc_1, y1_lower + i * obj_panel.fsl_lower, obj_panel.fc_1 + 0.5 * obj_panel.fd_long_lower), new Point3D(obj_panel.Long_Bottom_Bars_Array[0].fL - obj_panel.fc_1, y1_lower + i * obj_panel.fsl_lower, obj_panel.fc_1 + 0.5 * obj_panel.fd_long_lower), obj_panel.Long_Bottom_Bars_Array[i].Cross_Section, brRein_l1, brRein_l2, brRein_l3, null));

                double y2_upper = 0.06;
                for (int i = 0; i < obj_panel.Long_Upper_Bars_Array.Length; i++)
                    model.Children.Add(obj_panel.Long_Upper_Bars_Array[i].GetMemberModel(eGCS, bIsReinfocementSurfaceTransparent, new Point3D(obj_panel.fc_2, y2_upper + i * obj_panel.fsl_upper, (float)obj_panel.Cross_Section.h - obj_panel.fc_2 - 0.5 * obj_panel.fd_long_upper), new Point3D(obj_panel.Long_Upper_Bars_Array[0].fL - obj_panel.fc_2, y2_upper + i * obj_panel.fsl_upper, (float)obj_panel.Cross_Section.h - obj_panel.fc_2 - 0.5 * obj_panel.fd_long_upper), obj_panel.Long_Upper_Bars_Array[i].Cross_Section, brRein_u1, brRein_u2, brRein_u3, null));

                double x1 = 0.03;

                double[] trans_rein_position_x_array = new double[obj_panel.number_trans_lower_bars];

                trans_rein_position_x_array[0] = x1;

                // Fill Array of x positions of transversal bars - now it is for 3 different x values between bars, TODO make it more general
                for (int i = 1; i < obj_panel.number_trans_lower_bars; i++)
                {
                    if (i < obj_panel.number_trans_lower_bars / 2) // 1st half of panel
                    {
                        if (i <= obj_panel.number_tr_rein_arr_1)
                            trans_rein_position_x_array[i] = x1 + i * obj_panel.fx_tr_rein_arr_1;
                        else if (i <= (obj_panel.number_tr_rein_arr_1 + obj_panel.number_tr_rein_arr_2))
                            trans_rein_position_x_array[i] = x1 + obj_panel.number_tr_rein_arr_1 * obj_panel.fx_tr_rein_arr_1 + (i - obj_panel.number_tr_rein_arr_1) * obj_panel.fx_tr_rein_arr_2;
                        else
                            trans_rein_position_x_array[i] = x1 + obj_panel.number_tr_rein_arr_1 * obj_panel.fx_tr_rein_arr_1 + obj_panel.number_tr_rein_arr_2 * obj_panel.fx_tr_rein_arr_2 + (i - (obj_panel.number_tr_rein_arr_1 + obj_panel.number_tr_rein_arr_2)) * obj_panel.fx_tr_rein_arr_3;
                    }
                    else // Symmetrical - second half of panel
                    {
                        trans_rein_position_x_array[i] = obj_panel.fL - trans_rein_position_x_array[-(i - obj_panel.number_trans_lower_bars + 1)];
                    }
                }

                double trans_lower_start = y1_lower - obj_panel.fc_trans_lower;
                double l_trans_lower = (obj_panel.number_long_lower_bars - 1) * obj_panel.fsl_lower + 2 * obj_panel.fc_trans_lower;

                double trans_upper_start = y2_upper - obj_panel.fc_trans_upper;
                double l_trans_upper = (obj_panel.number_long_upper_bars - 1) * obj_panel.fsl_upper + 2 * obj_panel.fc_trans_upper;

                for (int i = 0; i < obj_panel.Trans_Bottom_Bars_Array.Length; i++)
                    model.Children.Add(obj_panel.Trans_Bottom_Bars_Array[i].GetMemberModel(eGCS, bIsReinfocementSurfaceTransparent, new Point3D(trans_rein_position_x_array[i], trans_lower_start, obj_panel.fc_1 + obj_panel.fd_long_lower + 0.5 * obj_panel.fd_trans_lower), new Point3D(trans_rein_position_x_array[i], trans_lower_start + l_trans_lower, obj_panel.fc_1 + obj_panel.fd_long_lower + 0.5 * obj_panel.fd_trans_lower), obj_panel.Trans_Bottom_Bars_Array[i].Cross_Section, brRein_l1, brRein_l2, brRein_l3, null));

                for (int i = 0; i < obj_panel.Trans_Upper_Bars_Array.Length; i++)
                    model.Children.Add(obj_panel.Trans_Upper_Bars_Array[i].GetMemberModel(eGCS, bIsReinfocementSurfaceTransparent, new Point3D(trans_rein_position_x_array[i], trans_upper_start, obj_panel.Cross_Section.h - obj_panel.fc_2 - obj_panel.fd_long_upper - 0.5 * obj_panel.fd_trans_upper), new Point3D(trans_rein_position_x_array[i], trans_upper_start + l_trans_upper, obj_panel.Cross_Section.h - obj_panel.fc_2 - obj_panel.fd_long_upper - 0.5 * obj_panel.fd_trans_upper), obj_panel.Trans_Upper_Bars_Array[i].Cross_Section, brRein_u1, brRein_u2, brRein_u3, null));
            }

            // Panel - Solid Surface
            if (bDisplayConcretePanel)
            {
                SolidColorBrush br1 = new SolidColorBrush(Color.FromRgb(130, 130, 130));  // Material color - Front Side
                SolidColorBrush br2 = new SolidColorBrush(Color.FromRgb(210, 210, 210));  // Material color - Shell
                SolidColorBrush br3 = new SolidColorBrush(Color.FromRgb(131, 131, 131));  // Material color - Back Side

                br1.Opacity = 0.5;
                br2.Opacity = 0.7;
                br3.Opacity = 0.5;

                DiffuseMaterial qDiffTrans = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(130, 130, 130, 1)));
                SpecularMaterial qSpecTrans = new SpecularMaterial(new SolidColorBrush(Color.FromArgb(210, 210, 210, 210)), 90.0);

                MaterialGroup qOuterMaterial = new MaterialGroup();
                qOuterMaterial.Children.Add(qDiffTrans);
                qOuterMaterial.Children.Add(qSpecTrans);

                model.Children.Add(obj_panel.GetMemberModel(eGCS, bIsPanelSurfaceTransparent, new Point3D(0, 0, 0), new Point3D(obj_panel.fL, 0, 0), obj_panel.Cross_Section, br1, br2, br3, qOuterMaterial));
            }

            // Transform (Rotate) panel to display it in system of WPF coordinates z_WPF = x_Panel, x_WPF = y_Panel, y_WPF = z_Panel
            // Toto by sa asi malo zmazat, staci nastavit spravne poziciu a vektor pre kameru
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 270);
            myRotateTransform3D.Rotation = rotation;
            model.Transform = myRotateTransform3D;

            // Panel - Wire Frame
            if (bDisplayConcretePanel_WireFrame)
            {
                ScreenSpaceLines3D wireFrame_FrontSide = wireFrame(0);
                ScreenSpaceLines3D wireFrame_BackSide = wireFrame(obj_panel.fL);
                ScreenSpaceLines3D wireFrame_Lateral = wireFrameLateral(0, obj_panel.fL);

                _trackport.ViewPort.Children.Add(wireFrame_FrontSide);
                _trackport.ViewPort.Children.Add(wireFrame_BackSide);
                _trackport.ViewPort.Children.Add(wireFrame_Lateral);
            }

            _trackport.Model = (Model3D)model;
            _trackport.Trackball.TranslateScale = 1000;   //step for moving object (panning)
            _trackport.SetupScene();
        }

        public ScreenSpaceLines3D wireFrame(double x)
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();
            wireFrame.Color = Color.FromRgb(200, 200, 200);
            wireFrame.Thickness = 1.0;

            for (int i = 0; i < obj_panel.Cross_Section.CrScPointsOut.Length / 2 - obj_panel.Cross_Section.INoAuxPoints; i++)
            {
                Point3D pi = new Point3D();
                Point3D pj = new Point3D();

                // Note: Due to default rotation is y considered as z_Crsc and z asi -y_Crsc
                // Malo by sa zadat v originalnych suradniach a rotovat do defaultneho pohladu ako jedna Model3DGroup ale ScreenSpaceLines3D sa do Model3DGroup nedaju pridat
                // Este elegantnejsie by bolo nastavit defaultne zobrazenie tak ze z je vertikalna os na obrazovke, nie osa kolma na obrazovku

                if (i < obj_panel.Cross_Section.CrScPointsOut.Length / 2 - obj_panel.Cross_Section.INoAuxPoints - 1)
                {
                    pi = new Point3D(x, obj_panel.Cross_Section.CrScPointsOut[i + obj_panel.Cross_Section.INoAuxPoints, 1], -obj_panel.Cross_Section.CrScPointsOut[obj_panel.Cross_Section.INoAuxPoints + i, 0]);
                    pj = new Point3D(x, obj_panel.Cross_Section.CrScPointsOut[i + obj_panel.Cross_Section.INoAuxPoints + 1, 1], -obj_panel.Cross_Section.CrScPointsOut[obj_panel.Cross_Section.INoAuxPoints + i + 1, 0]);
                }
                else // Last line
                {
                    pi = new Point3D(x, obj_panel.Cross_Section.CrScPointsOut[obj_panel.Cross_Section.INoAuxPoints + i, 1], -obj_panel.Cross_Section.CrScPointsOut[obj_panel.Cross_Section.INoAuxPoints + i, 0]);
                    pj = new Point3D(x, obj_panel.Cross_Section.CrScPointsOut[obj_panel.Cross_Section.INoAuxPoints + 0, 1], -obj_panel.Cross_Section.CrScPointsOut[obj_panel.Cross_Section.INoAuxPoints + 0, 0]);
                }

                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }
            return wireFrame;
        }

        public ScreenSpaceLines3D wireFrameLateral(double x1, double x2)
        {
            ScreenSpaceLines3D wireFrame = new ScreenSpaceLines3D();
            wireFrame.Color = Color.FromRgb(200, 200, 200);
            wireFrame.Thickness = 1.0;

            for (int i = 0; i < obj_panel.Cross_Section.CrScPointsOut.Length / 2 - obj_panel.Cross_Section.INoAuxPoints; i++)
            {
                Point3D pi = new Point3D();
                Point3D pj = new Point3D();

                // Note: Due to default rotation is y considered as z_Crsc and z asi -y_Crsc
                // Malo by sa zadat v originalnych suradniach a rotovat do defaultneho pohladu ako jedna Model3DGroup ale ScreenSpaceLines3D sa do Model3DGroup nedaju pridat
                // Este elegantnejsie by bolo nastavit defaultne zobrazenie tak ze z je vertikalna os na obrazovke, nie osa kolma na obrazovku

                pi = new Point3D(x1, obj_panel.Cross_Section.CrScPointsOut[i + obj_panel.Cross_Section.INoAuxPoints, 1], -obj_panel.Cross_Section.CrScPointsOut[obj_panel.Cross_Section.INoAuxPoints + i, 0]);
                pj = new Point3D(x2, obj_panel.Cross_Section.CrScPointsOut[i + obj_panel.Cross_Section.INoAuxPoints, 1], -obj_panel.Cross_Section.CrScPointsOut[obj_panel.Cross_Section.INoAuxPoints + i, 0]);

                wireFrame.Points.Add(pi);
                wireFrame.Points.Add(pj);
            }
            return wireFrame;
        }

        //--------------------------------------------------------------------------------------------
        private void LookAt(PerspectiveCamera camera, Point3D lookAtPoint)
        {
            camera.LookDirection = lookAtPoint - camera.Position;
        }
    }
}

