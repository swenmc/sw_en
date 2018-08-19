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
using System.Globalization;
using System.Text.RegularExpressions;
using BaseClasses;
using MATERIAL;
using BaseClasses.CRSC;

namespace AAC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AAC_Database_Data AAC_data = new AAC_Database_Data();
        AAC_Panel obj_panel;
        CultureInfo ci;

        public MainWindow()
        {
            ci = new CultureInfo("en-us");

            InitializeComponent();

            LoadDataFromWindow();

            CreatePanel();

            PanelPreview page1 = new PanelPreview(obj_panel);

            // Display panel model in 3D preview
            Frame1.Content = page1;
        }

        public enum E_SupportMaterialType
        {
            masonry,
            steel,
            concrete,
            wood
        };

        #region Variables
        // Combobox input

        AAC_Panel.E_AACElementType selected_AAC_ComponentIndex;
        int selected_StandardIndex;
        E_SupportMaterialType selected_SupportMaterial_1S_Index;
        E_SupportMaterialType selected_SupportMaterial_2E_Index;
        int selected_AAC_StrengthClassIndex;
        int selected_AAC_DensityClassIndex;
        int selected_Reinforcement_StrengthClassIndex;
        int selected_Reinforcement_d_long_upper_Index;
        int selected_Reinforcement_d_long_lower_Index;
        int selected_Reinforcement_d_trans_Index;

        float fg_grav_acc_constant = 10.0f; // Gravitational acceleration constant

        // Geometry

        float fL_w = 0.0f; // Length between supports
        float fa_1 = 0.0f; // Support Length at the Start
        float fa_1_min = 0.0f; // Support Minimum Length
        float fa_2_min = 0.0f; // Support Minimum Length
        float fc_1 = 0.0f; // Concrete Cover - lower longitudinal reinforcement
        float fc_2 = 0.0f; // Concrete Cover - upper longitudinal reinforcement
        float fc_trans = 0.02f; // ???? Concrete cover of transversal reinforcement (distance between end of bar and concrete surface)

        // Panel

        float fb = 0.0f;
        float fh = 0.0f;
        float fL = 0.0f;

        // Concrete

        float fFactor_Alpha = 0.0f; // Reduction coefficient for long term effect on compressive strength of concrete
        float fRho_m = 0.0f;
        public float fGamma_c_DBF = 0.0f;
        public float fGamma_c_BF = 0.0f;

        // Reinforcement

        float ff_yk_0 = 0.0f;

        int number_long_upper_bars = 0;
        int number_long_lower_bars = 0;
        int number_trans_bars_half = 0;
        int number_trans_bars = 0;

        public float fd_long_upper = 0.0f;
        public float fd_long_lower = 0.0f;
        public float fd_trans = 0.0f;

        public float fsl_upper = 0.0f;
        public float fsl_lower = 0.0f;

        public float fGamma_s = 0.0f;

        int number_trans_rein_arr_1 = 0;
        int number_trans_rein_arr_2 = 0;
        int number_trans_rein_arr_3 = 0;

        public float ftrans_rein_arr_dist_1 = 0.0f;
        public float ftrans_rein_arr_dist_2 = 0.0f;
        public float ftrans_rein_arr_dist_3 = 0.0f;

        // Loading

        float fgamma_g = 0.0f;
        float fgamma_q = 0.0f;
        float fPsi_1 = 0.0f;
        float fPsi_2 = 0.0f;
        float fg_k = 0.0f;
        float fq_k = 0.0f;

        // Transport

        float fb_s = 0.0f;
        float fgamma_t = 0.0f;
        float fRho_trans = 0.0f;

        #endregion

        #region Selection Changed
        private void ComboBox_AAC_Component_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_AAC_ComponentIndex = (AAC_Panel.E_AACElementType)combobox.SelectedIndex;

            switch (selected_AAC_ComponentIndex)
            {
                case AAC_Panel.E_AACElementType.Beam:
                    myStackPanel.Children.Add(new Label { Content  = "Zdar Mato.Ta toto je label." });
                    //myStackPanel.Children.Add(new Button { Content = "Button"});                    

                    ComboBox cbox = new ComboBox();
                    cbox.Width = 280;
                    cbox.Height = 30;
                    cbox.Background = Brushes.LightBlue;
                    ComboBoxItem cboxitem = new ComboBoxItem();
                    cboxitem.Content = "Item 1";
                    cbox.Items.Add(cboxitem);
                    ComboBoxItem cboxitem2 = new ComboBoxItem();
                    cboxitem2.Content = "Item 2";
                    cbox.Items.Add(cboxitem2);
                    ComboBoxItem cboxitem3 = new ComboBoxItem();
                    cboxitem3.Content = "Item 3";
                    cbox.Items.Add(cboxitem3);
                    myStackPanel.Children.Add(cbox);
                    myStackPanel.Width = 200;
                    myStackPanel.Height = 200;
                    myStackPanel.Background = Brushes.Green;

                    break;
                case AAC_Panel.E_AACElementType.Floor_Panel:
                    
                    break;
                case AAC_Panel.E_AACElementType.Horizontal_Wall_Panel:
                    
                    break;
                case AAC_Panel.E_AACElementType.Roof_Panel: break;
                case AAC_Panel.E_AACElementType.Vertical_Wall_Panel_1:
                    //skryt prvok
                    ComboBox_SupportMaterial_1S.Visibility = Visibility.Hidden;
                    ComboBox_SupportMaterial_2E.Visibility = Visibility.Hidden;
                    break;
                case AAC_Panel.E_AACElementType.Vertical_Wall_Panel_2:
                    if (ComboBox_SupportMaterial_1S == null) return;
                    if (ComboBox_SupportMaterial_2E == null) return;
                    //znovu zobrazit prvok
                    ComboBox_SupportMaterial_1S.Visibility = Visibility.Visible;
                    ComboBox_SupportMaterial_2E.Visibility = Visibility.Visible;
                    break;
                default: break;
            }

        }

        private void ComboBox_Standard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_StandardIndex = combobox.SelectedIndex;
        }

        private void ComboBox_SupportMaterial_1S_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_SupportMaterial_1S_Index = (E_SupportMaterialType)combobox.SelectedIndex;
        }

        private void ComboBox_SupportMaterial_2E_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_SupportMaterial_2E_Index = (E_SupportMaterialType)combobox.SelectedIndex;
        }

        private void TextBoxLength_h_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text)) return;
            fh = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLength_b_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fb = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLength_Lw_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fL_w = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLength_L_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fL = (float) Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLength_a1_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fa_1 = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLength_c1_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fc_1 = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLength_c2_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fc_2 = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void ComboBox_AAC_CSC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_AAC_StrengthClassIndex = combobox.SelectedIndex;
        }

        private void ComboBox_AAC_DC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_AAC_DensityClassIndex = combobox.SelectedIndex;
        }

        private void TextBoxConcreteDensity_rho_m_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fRho_m = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxConcreteFactor_Alpha_c_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fFactor_Alpha = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxConcreteFactor_Gamma_c_DBF_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fGamma_c_DBF = (float)Convert.ToDecimal(textBox.Text, ci); // Ductile bending failure (1.44)
        }

        private void TextBoxConcreteFactor_Gamma_c_BF_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fGamma_c_BF = (float)Convert.ToDecimal(textBox.Text, ci); // Brittle failure (1.73)
        }

        // Reinforcement

        private void ComboBox_Reinforcement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_Reinforcement_StrengthClassIndex = combobox.SelectedIndex;
        }

        private void TextBoxLongReinUpper_No_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            number_long_upper_bars = Convert.ToInt32(textBox.Text);
        }

        private void TextBoxLongReinLower_No_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            number_long_lower_bars = Convert.ToInt32(textBox.Text);
        }

        private void TextBoxTransRein_No_half_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            number_trans_bars_half = Convert.ToInt32(textBox.Text);
            number_trans_bars = 2 * number_trans_bars_half;
        }

        private void ComboBox_LongReinUpper_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_Reinforcement_d_long_upper_Index = combobox.SelectedIndex;
        }

        private void TextBoxLongReinUpper_distance_sl_2_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fsl_upper = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void ComboBox_LongReinLower_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_Reinforcement_d_long_lower_Index = combobox.SelectedIndex;
        }

        private void TextBoxLongReinLower_distance_sl_1_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fsl_lower = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void ComboBox_TransRein_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get control that raised this event.
            ComboBox combobox = (ComboBox)sender;
            selected_Reinforcement_d_trans_Index = combobox.SelectedIndex;
        }

        private void TextBoxReinforcementFactor_Gamma_s_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fGamma_s = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        // Transversal Reinforcement Arrangement

        private void TextBoxTransReinArr_1_No_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            number_trans_rein_arr_1 = Convert.ToInt32(textBox.Text, ci);
        }

        private void TextBoxTransReinArr_1_distance_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            ftrans_rein_arr_dist_1 = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxTransReinArr_2_No_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            number_trans_rein_arr_2 = Convert.ToInt32(textBox.Text);
        }

        private void TextBoxTransReinArr_2_distance_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            ftrans_rein_arr_dist_2 = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxTransReinArr_3_No_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            number_trans_rein_arr_3 = Convert.ToInt32(textBox.Text);
        }

        private void TextBoxTransReinArr_3_distance_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            ftrans_rein_arr_dist_3 = (float)Convert.ToDecimal(textBox.Text, ci);
        }
        
        // Loading

        private void TextBoxLoadingFactorGamma_g_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;            
            fgamma_g = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLoadingFactorGamma_q_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            fgamma_q = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLoadingFactorPsi_1_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            fPsi_1 = (float)Convert.ToDecimal(textBox.Text,ci);
        }

        private void TextBoxLoadingFactorPsi_2_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            fPsi_2 = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLoadingValue_g_k_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            fg_k = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxLoadingValue_q_k_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            fq_k = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        // Transport

        private void TextBoxTransportValue_b_s_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            fb_s = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxTransportValue_Gamma_t_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            fgamma_t = (float)Convert.ToDecimal(textBox.Text, ci);
        }

        private void TextBoxAACPanelDensity_Rho_trans_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as TextBox;
            fRho_trans = (float)Convert.ToDecimal(textBox.Text, ci);
        }
        #endregion

        private void LoadDataFromWindow()
        {
            selected_AAC_ComponentIndex = (AAC_Panel.E_AACElementType)ComboBox_AAC_Component.SelectedIndex;
            selected_StandardIndex = ComboBox_Standard.SelectedIndex;
            selected_SupportMaterial_1S_Index = (E_SupportMaterialType)ComboBox_SupportMaterial_1S.SelectedIndex;
            selected_SupportMaterial_2E_Index = (E_SupportMaterialType)ComboBox_SupportMaterial_2E.SelectedIndex;

            fh = (float)Convert.ToDecimal(TextBoxCrossSectionHeight_h.Text, ci);
            fb = (float)Convert.ToDecimal(TextBoxCrossSectionWidth_b.Text,ci);
            fL_w = (float)Convert.ToDecimal(TextBoxLength_Lw.Text,ci);
            fL = (float)Convert.ToDecimal(TextBoxLength_L.Text,ci);
            fa_1 = (float)Convert.ToDecimal(TextBoxSupportWidth_a1.Text,ci);
            fc_1 = (float)Convert.ToDecimal(TextBoxConcreteCover_c1.Text,ci);
            fc_2 = (float)Convert.ToDecimal(TextBoxConcreteCover_c2.Text,ci);

            selected_AAC_StrengthClassIndex = ComboBox_AAC_CSC.SelectedIndex;
            selected_AAC_DensityClassIndex = ComboBox_AAC_DC.SelectedIndex;
            fRho_m = (float)Convert.ToDecimal(TextBoxConcreteDensity_rho_m.Text,ci);
            fFactor_Alpha = (float)Convert.ToDecimal(TextBoxConcreteFactor_Alpha_c.Text,ci);
            fGamma_c_DBF = (float)Convert.ToDecimal(TextBoxConcreteFactor_Gamma_c_DBF.Text,ci);
            fGamma_c_BF = (float)Convert.ToDecimal(TextBoxConcreteFactor_Gamma_c_BF.Text,ci);

            selected_Reinforcement_StrengthClassIndex = ComboBox_Reinforcement.SelectedIndex;
            number_long_upper_bars = Convert.ToInt32(TextBoxLongReinUpper_No.Text);
            number_long_lower_bars = Convert.ToInt32(TextBoxLongReinLower_No.Text);
            number_trans_bars = 2 * Convert.ToInt32(TextBoxTransRein_No_half.Text);
            selected_Reinforcement_d_long_upper_Index = ComboBox_LongReinUpper.SelectedIndex;
            fsl_upper = (float)Convert.ToDecimal(TextBoxLongReinUpper_distance_sl_2.Text,ci);
            selected_Reinforcement_d_long_lower_Index = ComboBox_LongReinLower.SelectedIndex;
            fsl_lower = (float)Convert.ToDecimal(TextBoxLongReinLower_distance_sl_1.Text,ci);
            selected_Reinforcement_d_trans_Index = ComboBox_TransRein.SelectedIndex;

            fGamma_s = (float)Convert.ToDecimal(TextBoxReinforcementFactor_Gamma_s.Text,ci);

            number_trans_rein_arr_1 = Convert.ToInt32(TextBoxTransReinArr_1_No.Text);
            number_trans_rein_arr_2 = Convert.ToInt32(TextBoxTransReinArr_2_No.Text);
            number_trans_rein_arr_3 = Convert.ToInt32(TextBoxTransReinArr_3_No.Text);

            ftrans_rein_arr_dist_1 = (float)Convert.ToDecimal(TextBoxTransReinArr_1_distance_x.Text,ci);
            ftrans_rein_arr_dist_2 = (float)Convert.ToDecimal(TextBoxTransReinArr_2_distance_x.Text,ci);
            ftrans_rein_arr_dist_3 = (float)Convert.ToDecimal(TextBoxTransReinArr_3_distance_x.Text,ci);

            fgamma_g = (float)Convert.ToDecimal(TextBoxLoadingFactorGamma_g.Text,ci);
            fgamma_q = (float)Convert.ToDecimal(TextBoxLoadingFactorGamma_q.Text,ci);
            fPsi_1 = (float)Convert.ToDecimal(TextBoxLoadingFactorPsi_1.Text,ci);
            fPsi_2 = (float)Convert.ToDecimal(TextBoxLoadingFactorPsi_2.Text,ci);
            fg_k = (float)Convert.ToDecimal(TextBoxLoadingValue_g_k.Text,ci);
            fq_k = (float)Convert.ToDecimal(TextBoxLoadingValue_q_k.Text,ci);

            fb_s = (float)Convert.ToDecimal(TextBoxTransportValue_b_s.Text,ci);
            fgamma_t = (float)Convert.ToDecimal(TextBoxTransportValue_Gamma_t.Text,ci);
            fRho_trans = (float)Convert.ToDecimal(TextBoxAACPanelDensity_Rho_trans.Text,ci);
        }

        private void CreatePanel()
        {
            // Fill data of panel object
            AAC_data.Get_Database_Data(selected_Reinforcement_StrengthClassIndex,
                     selected_Reinforcement_d_long_upper_Index,
                     selected_Reinforcement_d_long_lower_Index,
                     selected_Reinforcement_d_trans_Index,
                     out ff_yk_0,
                     out fd_long_upper,
                     out fd_long_lower,
                     out fd_trans);

            // Define panel shape - cross-section

            CCrSc panel_crsc;

            if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Beam)
                panel_crsc = new CCrSc_2_00_AAC_Beam(fh, fb); // Solid Beam
            else if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Floor_Panel)
                panel_crsc = new CCrSc_2_00_AAC_Floor_Panel(fh, fb);
            else if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Horizontal_Wall_Panel)
                panel_crsc = new CCrSc_2_00_AAC_Horizontal_Wall_Panel(fh, fb);
            else if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Roof_Panel)
                panel_crsc = new CCrSc_2_00_AAC_Roof_Panel(fh, fb);
            else if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Vertical_Wall_Panel_1)
                panel_crsc = new CCrSc_2_00_AAC_Wall_Panel_1(fh, fb);
            else if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Vertical_Wall_Panel_2)
                panel_crsc = new CCrSc_2_00_AAC_Wall_Panel_2(fh, fb);
            else
                panel_crsc = null; // Unexpected index (type of panel)

            // Validate input

            if ((1 + number_trans_rein_arr_1 + number_trans_rein_arr_2 + number_trans_rein_arr_3) != number_trans_bars_half)
            {
                MessageBox.Show("Error in number of transversal bars.");
            }

            // Create Panel
            obj_panel = new AAC_Panel(
                selected_AAC_ComponentIndex,
                fL,
                panel_crsc,
                new MATERIAL.CMat_02_00_AAC(selected_AAC_StrengthClassIndex, fRho_m),
                new MATERIAL.CMat_03_00(),
                number_long_lower_bars,
                number_long_upper_bars,
                number_trans_bars,
                number_trans_bars,
                fd_long_upper,
                fd_long_lower,
                fd_trans,
                fd_trans,
                fsl_upper,
                fsl_lower,
                fc_1,
                fc_2,
                fc_trans,
                fc_trans,
                number_trans_rein_arr_1,
                number_trans_rein_arr_2,
                number_trans_rein_arr_3,
                ftrans_rein_arr_dist_1,
                ftrans_rein_arr_dist_2,
                ftrans_rein_arr_dist_3
                );

            obj_panel.Reinforcement.m_ff_yk[0] = ff_yk_0;
            obj_panel.FillReinforcementData(0, obj_panel.Long_Bottom_Bars_Array, fd_long_lower, fc_1);
            obj_panel.FillReinforcementData(0, obj_panel.Long_Upper_Bars_Array, fd_long_upper, fc_2);
            obj_panel.FillReinforcementData(1, obj_panel.Trans_Bottom_Bars_Array, fd_trans, fc_trans); // Zatial rovnaky priemer hornej aj spodnej priecnej vyztuze
            obj_panel.FillReinforcementData(1, obj_panel.Trans_Upper_Bars_Array, fd_trans, fc_trans);
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            //this.Clear(); // Delete results of previous calculation

            // Floor Panel according to EN 12602

            float fa_2 = (obj_panel.fL - fL_w - fa_1); // Todo predpoklada symetricke ulozenie na podpory

            fa_1_min = Get_minimum_support_a_min(selected_SupportMaterial_1S_Index);
            fa_2_min = Get_minimum_support_a_min(selected_SupportMaterial_2E_Index);

            float fL_eff = fL_w + 1.0f / 3.0f * fa_1_min + 1.0f / 3.0f * fa_2_min;

            float fd_eff_lower = (float)Math.Min(obj_panel.Cross_Section.h - fc_1 - 0.5f * fd_long_lower, 0.4f); // Effective Depth of Section, see A.6
            float fb_load = (float)obj_panel.Cross_Section.b; // !!!!!! ????? Could be different

            float fA_c_lower = (float)obj_panel.Cross_Section.b * fd_eff_lower; // Compressed area of concrete
            float fn_p_support = 2.0f;

            // Internal forces

            fg_k *= 1000.0f; // N / m^2
            fq_k *= 1000.0f; // N / m^2

            // Characteristic combinations
            float fG_d1 = fgamma_g * fb_load * fg_k;
            float fQ_d1 = fgamma_q * fb_load * fq_k;

            float fV_Sd_1 = 0.5f * (fG_d1 + fQ_d1) * fL_eff;
            float fM_Sd_1 = ((fG_d1 + fQ_d1) * fL_eff * fL_eff) / 8.0f;

            // Frequent combinations
            float fG_d2 = fb_load * fg_k;
            float fQ_d2 = fPsi_1 * fb_load * fq_k;

            float fV_Sd_2 = 0.5f * (fG_d2 + fQ_d2) * fL_eff;
            float fM_Sd_2 = ((fG_d2 + fQ_d2) * fL_eff * fL_eff) / 8.0f;

            // Quasi-permanent combinations
            float fG_d3 = fb_load * fg_k;
            float fQ_d3 = fPsi_2 * fb_load * fq_k;

            float fV_Sd_3 = 0.5f * (fG_d3 + fQ_d3) * fL_eff;
            float fM_Sd_3 = ((fG_d3 + fQ_d3) * fL_eff * fL_eff) / 8.0f;

            // Internal forces for transport situations
            float fG_t = fgamma_g * fg_grav_acc_constant * fRho_trans * (float)obj_panel.Cross_Section.b * (float)obj_panel.Cross_Section.h;

            // Cantilever
            float fL_cantilever = 0.5f * (obj_panel.fL - fb_s);
            float fV_t = fgamma_t * fG_t * fL_cantilever;
            float fM_t = (float)(0.5f * fgamma_t * fG_t * Math.Pow(fL_cantilever, 2));

            // Design
            double fTau_Rd = 1e+6 * (0.063f * Math.Sqrt(0.000001 * obj_panel.Concrete.Fck) / fGamma_c_BF); // Fck in MPa, Tau Rd in Pa
            double thousand_md = 1000 * fM_Sd_1 * obj_panel.Concrete.D_gamaMc / (fFactor_Alpha * obj_panel.Concrete.Fck * fA_c_lower * fd_eff_lower);

            // Annex A
            AAC_data.GetAAC_values_for_1000md(thousand_md);

            double fEpsilon_c = AAC_data.AAC_value_array_for_1000md[0];
            double fEpsilon_s = AAC_data.AAC_value_array_for_1000md[1];
            double fk_x = AAC_data.AAC_value_array_for_1000md[2];
            double fk_z = AAC_data.AAC_value_array_for_1000md[3];
            double thousand_omega_S235 = AAC_data.AAC_value_array_for_1000md[4];
            double thousand_omega_S500 = AAC_data.AAC_value_array_for_1000md[5];
            double thousand_omega = 0.0;

            if (obj_panel.Reinforcement.m_ff_yk[0] <= 2.51e+8)
                thousand_omega = thousand_omega_S235;
            else
                thousand_omega = thousand_omega_S500;

            float fomega = (float)thousand_omega / 1000.0f;

            double fA_s_b_min = fA_c_lower * fomega * fFactor_Alpha * obj_panel.Concrete.Fck * fGamma_s / (fGamma_c_BF * obj_panel.Reinforcement.m_ff_yk[0]);

            double fA_s_exis_lower = number_long_lower_bars * Math.PI * (fd_long_lower * fd_long_lower / 4.0); // Bottom Reinforcement
 
            if (fA_s_b_min > fA_s_exis_lower)
            {
                MessageBox.Show("Error! Check area of longitudinal bottom reinforcement.");
            }

            // Upper Reinforcement
            float fd_eff_u = (float)Math.Min(obj_panel.Cross_Section.h - fc_2 - 0.5f * fd_long_upper, 0.4f); // Effective Depth
            float fA_c_u = (float)obj_panel.Cross_Section.b * fd_eff_u; // Compressed area of concrete

            double thousand_md_upper = 1000 * fM_t * fGamma_c_BF / (fFactor_Alpha * obj_panel.Concrete.Fck * fA_c_u * fd_eff_u);

            AAC_data.GetAAC_values_for_1000md(thousand_md_upper);

            double fEpsilon_c_u = AAC_data.AAC_value_array_for_1000md[0];
            double fEpsilon_s_u = AAC_data.AAC_value_array_for_1000md[1];
            double fk_x_u = AAC_data.AAC_value_array_for_1000md[2];
            double fk_z_u = AAC_data.AAC_value_array_for_1000md[3];
            double thousand_omega_S235_u = AAC_data.AAC_value_array_for_1000md[4];
            double thousand_omega_S500_u = AAC_data.AAC_value_array_for_1000md[5];
            double thousand_omega_u = 0.0;

            if (obj_panel.Reinforcement.m_ff_yk[0] <= 2.51e+8)
                thousand_omega_u = thousand_omega_S235_u;
            else
                thousand_omega_u = thousand_omega_S500_u;

            float fomega_u = (float)thousand_omega_u / 1000.0f;

            double fA_s_u_min = fA_c_u * fomega_u * fFactor_Alpha * obj_panel.Concrete.Fck * fGamma_s / (fGamma_c_DBF * obj_panel.Reinforcement.m_ff_yk[0]);

            double fA_s_exis_upper = number_long_upper_bars * Math.PI * (fd_long_upper * fd_long_upper / 4.0); // Upper Reinforcement

            if (fA_s_u_min > fA_s_exis_upper)
            {
                MessageBox.Show("Error! Check area of longitudinal upper reinforcement.");
            }

            // Minimum Reinforcement

            double ff_cflm = 0.27f * /*0.8f **/ obj_panel.Concrete.Fck; // flexural strength

            float fA_ct = (float)obj_panel.Cross_Section.b * 0.5f * (float)obj_panel.Cross_Section.h;

            // k = 1 - pure tension in the whole cross-section
            // k = 0.4 - pure bending without normal compressive force

            float fk_A34 = 0.4f;
            double fAs_min = fk_A34 * fA_ct * ff_cflm / obj_panel.Reinforcement.m_ff_yk[0];

            float fb_w = (float)obj_panel.Cross_Section.b; //  ??????? Moze byt ina

            double rho_l = Math.Min(fA_s_exis_lower / (obj_panel.Cross_Section.b * fd_eff_lower), 0.005); // A.4

            // A.6
            double V_Rd_1_A6_1 = 0.5f * obj_panel.Concrete.Fctk0_05 / fGamma_c_BF * fb_w * fd_eff_lower;
            double V_Rd_1_A6_2 = fTau_Rd * (1.0f - 0.83f * fd_eff_lower) * (1 + 240 * rho_l) * fb_w * fd_eff_lower;
            double V_Rd_1 = Math.Max(V_Rd_1_A6_1, V_Rd_1_A6_2);

            double fDesign_Ratio_VSd_1_to_VRd1 = fV_Sd_1 / V_Rd_1;

            // Check distance between bars of longitudinal reinforcement
            float fsl_1_min, fsl_1_max;
            float fsl_2_min, fsl_2_max;

            Minimum_Spacing_of_Long_Bars(fd_long_lower, fd_eff_lower, out fsl_1_min, out fsl_1_max);

            if (fsl_1_min > fsl_lower || fsl_1_max < fsl_lower)
            {
                MessageBox.Show("Error! Check limit for distance between bars of longitudinal reinforcement.");
            }

            //Minimum_Spacing_of_Long_Bars(d_long_upper, fd_eff_upper, out fsl_2_min, out fsl_2_max);


            // Table A.1 - Bond Classes
            //       Kc1    Kc2
            // B1   1.35   2.20
            // B2   1.50   2.70

            // B1
            float fK_c1 = 1.35f;
            float fK_c2 = 2.2f;

            // Transverse reinforcement bars
            double e_dis = fc_1 + fd_long_lower + 0.5f * fd_trans;

            double z = 0.9f * fd_eff_lower; // 0.9 ????
            float fM_d1_max = fM_Sd_1;
            float fd_support = Math.Max(fa_1, fa_2); // Ulozenie na podpore ???
            float fM_d1_support = fV_Sd_1 * fd_support;

            // Maximum tensile forceobj_panel.Reinforcement.m_ff_yk_0
            double F_ld_max = fM_d1_max / z;
            double F_ld_support = fM_d1_support / z;

            // Design value of bearing strength at support
            int n_p_support = 2;
            int n_t_support = 2; // Number of transverse bars between the section concerned and the end of the component

            float fm_support = Get_m(n_p_support, n_t_support);

            double f_ld_support = Get_fld(fK_c1, fm_support, e_dis, fd_trans, fFactor_Alpha, obj_panel.Concrete.Fck, fGamma_c_DBF);

            // Limit value of bearing strength at support
            double f_ld_limit_support = fK_c2 * obj_panel.Concrete.Fck / fGamma_c_DBF;

            if (f_ld_support > f_ld_limit_support)
                f_ld_support = f_ld_limit_support;

            // Design value of bearing strength at middle of span
            int n_p_midspan = 2;
            int n_t_midspan = (int)(0.5f * number_trans_bars); // // Number of transverse bars between the section concerned and the end of the component Eq. (A.48) Polovica z celkoveho poctu priecnych vyztuznych prutov na spodnom okraji
            float fm_midspan = Get_m(n_p_midspan, n_t_midspan);

            double f_ld_midspan = Get_fld(fK_c1, fm_midspan, e_dis, fd_trans, fFactor_Alpha, obj_panel.Concrete.Fck, fGamma_c_BF);

            // Limit value of bearing strength at midspan
            double f_ld_limit_midspan = fK_c2 * obj_panel.Concrete.Fck / fGamma_c_BF;

            if (f_ld_midspan > f_ld_limit_midspan)
                f_ld_midspan = f_ld_limit_midspan;

            // Anchorage force capacity - bottom reinforcement
            double A_sl = fA_s_exis_lower; // Cross sectional area of the reinforcing bar with the larger diameter of the connection

            float t_t = (number_long_lower_bars - 2) * fsl_lower + 2 * 0.5f * fsl_lower + 0.015f + 0.015f; // Vzdialenost medzi pozdlznou vystuzou + presah // TODO

            double F_RA_support = 0.83f * n_t_support * fd_trans * t_t * f_ld_support;

            // Welding strength classes - Table 5c (S1 - 0.25, S2 - 0.5)
            float fk_w = 0.25f;
            int n_l = number_long_lower_bars; // Number of longitudinal bars - Eq. (A.48)

            double F_wg = fk_w * A_sl * obj_panel.Reinforcement.m_ff_yk[0];
            double F_RA_support_limit = 0.6f * n_l * n_t_support * F_wg / fGamma_s;

            if (F_RA_support > F_RA_support_limit) //A.48
                F_RA_support = F_RA_support_limit;

            // TODO definovat pocet a roztec uzivatelsky podla poctu zvolenych transveral bars
            // Moze byt ine pre hornu a spodnu vystuz

            double dist_a1 = 0.03; // Cover 
            double dist_a2 = 0.1;
            double dist_a3 = 0.32;
            double dist_a4 = 0.5;

            // Todo dynamicky pocet prvkov v poli a x suradnice
            double[] tras_bar_position_in_panel_array = new double[number_trans_bars];
            tras_bar_position_in_panel_array[0] = dist_a1;
            tras_bar_position_in_panel_array[1] = tras_bar_position_in_panel_array[0] + dist_a2;
            tras_bar_position_in_panel_array[2] = tras_bar_position_in_panel_array[1] + dist_a2;
            tras_bar_position_in_panel_array[3] = tras_bar_position_in_panel_array[2] + dist_a2;
            tras_bar_position_in_panel_array[4] = tras_bar_position_in_panel_array[3] + dist_a2;
            tras_bar_position_in_panel_array[5] = tras_bar_position_in_panel_array[4] + dist_a3;
            tras_bar_position_in_panel_array[6] = tras_bar_position_in_panel_array[5] + dist_a4;
            tras_bar_position_in_panel_array[7] = tras_bar_position_in_panel_array[6] + dist_a4;
            tras_bar_position_in_panel_array[8] = tras_bar_position_in_panel_array[7] + dist_a4;
            tras_bar_position_in_panel_array[9] = tras_bar_position_in_panel_array[8] + dist_a4;
            tras_bar_position_in_panel_array[10] = tras_bar_position_in_panel_array[9] + dist_a4;
            tras_bar_position_in_panel_array[11] = tras_bar_position_in_panel_array[10] + dist_a4;
            tras_bar_position_in_panel_array[12] = tras_bar_position_in_panel_array[11] + dist_a4;
            tras_bar_position_in_panel_array[13] = tras_bar_position_in_panel_array[12] + dist_a3;
            tras_bar_position_in_panel_array[14] = tras_bar_position_in_panel_array[13] + dist_a2;
            tras_bar_position_in_panel_array[15] = tras_bar_position_in_panel_array[14] + dist_a2;
            tras_bar_position_in_panel_array[16] = tras_bar_position_in_panel_array[15] + dist_a2;
            tras_bar_position_in_panel_array[17] = tras_bar_position_in_panel_array[16] + dist_a2;

            double[] F_RA_array = new double[n_t_midspan]; // Array of values

            for (int i = 0; i < n_t_midspan; i++)
            {
                int n_p = 2;
                int n_t = i + 1;
                float fm = Get_m(n_p, n_t);
                double f_ld = Get_fld(fK_c1, fm, e_dis, fd_trans, fFactor_Alpha, obj_panel.Concrete.Fck, i < 2 ? fGamma_c_DBF : fGamma_c_BF);
                double f_ld_limit = fK_c2 * obj_panel.Concrete.Fck / (i < 2 ? fGamma_c_DBF : fGamma_c_BF);

                if (f_ld > f_ld_limit)
                    f_ld = f_ld_limit;

                double F_RA = 0.83f * n_t * fd_trans * t_t * f_ld;
                double F_RA_limit = 0.6f * n_l * n_t * F_wg / fGamma_s;

                if (F_RA > F_RA_limit)
                    F_RA = F_RA_limit;

                F_RA_array[i] = F_RA;
            }

            double[] F_ld_transversal_bar_array = new double[number_trans_bars];

            for (int i = 0; i < tras_bar_position_in_panel_array.Length; i++)
            {
                double M_d1_x_transversal_bar;

                if (tras_bar_position_in_panel_array[i] < fa_1 || tras_bar_position_in_panel_array[i] > (obj_panel.fL -fa_2))
                    M_d1_x_transversal_bar = 0;
                else
                    M_d1_x_transversal_bar = fV_Sd_1 * tras_bar_position_in_panel_array[i] - (((fG_d1 + fQ_d1) * tras_bar_position_in_panel_array[i] * tras_bar_position_in_panel_array[i]) / 2.0);

                // tensile force
                F_ld_transversal_bar_array[i] = M_d1_x_transversal_bar / z;
            }

            float fDesign_Ratio_F_ld_F_RA_max = 0.0f;
            float fF_ld_max = 0.0f;
            float fF_RA_max = 0.0f;

            // Design Check
            for (int i = 0; i < n_t_midspan; i++)
            {
                float fdesignRatio_temp = (float) (F_ld_transversal_bar_array[i] / F_RA_array[i]);

                if (fdesignRatio_temp > fDesign_Ratio_F_ld_F_RA_max)
                {
                    fF_ld_max = (float)F_ld_transversal_bar_array[i];
                    fF_RA_max = (float)F_RA_array[i];
                    fDesign_Ratio_F_ld_F_RA_max = fdesignRatio_temp;
                }
            }

            // Anchorage force capacity - upper reinforcement

            // doplnit


            // Serviceability Limit States

            // Cracking moment
            ff_cflm = 0.27f * 0.8f * obj_panel.Concrete.Fck; // TODO flexural strength 0.8 niekde je s 0.8 inde nie, zistit preco ?????

            obj_panel.Cross_Section.W_y_el = (float)(obj_panel.Cross_Section.b * Math.Pow(obj_panel.Cross_Section.h, 2) / 6.0);

            double M_cr = obj_panel.Cross_Section.W_y_el * ff_cflm; // TODO podmienka pre typ posudenia SLS - uncracked, cracked condition

            float fd_1 = fd_long_lower;
            float fd_2 = fd_long_upper;

            double A_s1 = fA_s_exis_lower;
            double A_s2 = fA_s_exis_upper;

            float fy_s1 = fc_1 + 0.5f * fd_long_lower;
            float fy_s2 = (float)obj_panel.Cross_Section.h - fc_2 + 0.5f * fd_long_upper - fd_trans; // Horna vyztuz sa nachadza pod priecnou ??? TODO overit podla vykresov

            // Deflection under uncracked condition
            // Short-term deflection
            // Ratio of the modulus of elasticity of reinforcing steel and AAC

            obj_panel.Reinforcement.m_fE = 2.0e11f; // 210 MPa - structural steel, 200 MPa - reinforcement

            float fn_short_term = (float)(obj_panel.Reinforcement.m_fE / obj_panel.Concrete.D_Ecm);

            double fI_c_brutto_shortterm = Get_I_c_brutto((float)obj_panel.Cross_Section.b, obj_panel.Cross_Section.h, fn_short_term, number_long_lower_bars, fd_1, number_long_upper_bars, fd_2); // Moment of inertia of AAC and reinforcement
            double y_s_shortterm = Get_y_s((float)obj_panel.Cross_Section.b, (float)obj_panel.Cross_Section.h, fn_short_term, fy_s1, fy_s2, A_s1, A_s2); // Centre of grafity
            double I_st_shortterm = Get_I_st((float)obj_panel.Cross_Section.b, (float)obj_panel.Cross_Section.h, fn_short_term, y_s_shortterm, fy_s1, fy_s2, A_s1, A_s2); // Moment of inertia of reinforcement
            double I_ci_shortterm = fI_c_brutto_shortterm + I_st_shortterm;
            double Ecm_Ici_shortterm = obj_panel.Concrete.D_Ecm * I_ci_shortterm;
            double y_el_shortterm = 5.0f / 48.0f * fM_Sd_2 * Math.Pow(fL_eff, 2.0f) / (Ecm_Ici_shortterm); // Deflection due to load combination 2
            double y_el_lim = fL_eff / 250.0f;

            double y_el_shortterm_des_ratio = y_el_shortterm / y_el_lim;

            // Long-term deflection
            float fPhi = 1.0f; // Todo - temporary
            double fE_c_eff = obj_panel.Concrete.D_Ecm / (1 + fPhi);
            float fn_long_term = (float)(obj_panel.Reinforcement.m_fE / fE_c_eff);

            double fI_c_brutto_longterm = Get_I_c_brutto((float)obj_panel.Cross_Section.b, obj_panel.Cross_Section.h, fn_long_term, number_long_lower_bars, fd_1, number_long_upper_bars, fd_2);
            double y_s_longterm = Get_y_s((float)obj_panel.Cross_Section.b, (float)obj_panel.Cross_Section.h, fn_long_term, fy_s1, fy_s2, A_s1, A_s2);
            double I_st_longterm = Get_I_st((float)obj_panel.Cross_Section.b, (float)obj_panel.Cross_Section.h, fn_long_term, y_s_longterm, fy_s1, fy_s2, A_s1, A_s2);
            double I_ci_longterm = fI_c_brutto_longterm + I_st_longterm;
            double Eceff_Ici_longterm = fE_c_eff * I_ci_longterm;
            double y_el_longterm = 5.0f / 48.0f * fM_Sd_3 * Math.Pow(fL_eff, 2.0f) / (Eceff_Ici_longterm);

            double y_el_longterm_des_ratio = y_el_longterm / y_el_lim;

            // Deflection under cracked condition
            // Short-term deflection

            double fA = obj_panel.Cross_Section.b * obj_panel.Concrete.D_Ecm / (2.0f * A_s1 * obj_panel.Reinforcement.m_fE);
            double x = (Math.Sqrt(1.0f + 4.0f * fd_eff_lower * fA) - 1.0f) / (2.0f * fA);

            double fI_c_brutto_shortterm_crk = Get_I_c_brutto((float)obj_panel.Cross_Section.b, x, fn_short_term, number_long_lower_bars, fd_1, number_long_upper_bars, fd_2);
            double y_s_shortterm_crk = Get_y_s_x((float)obj_panel.Cross_Section.b, (float)obj_panel.Cross_Section.h, x, fn_short_term, fy_s1, fy_s2, A_s1, A_s2);
            double I_st_shortterm_crk = Get_I_st_x((float)obj_panel.Cross_Section.b, (float)obj_panel.Cross_Section.h, x, fn_short_term, y_s_shortterm_crk, fy_s1, fy_s2, A_s1, A_s2);
            double I_ci_shortterm_crk = fI_c_brutto_shortterm_crk + I_st_shortterm_crk;
            double Ecm_Ici_shortterm_crk = obj_panel.Concrete.D_Ecm * I_ci_shortterm_crk;
            double y_el_shortterm_crk = 5.0f / 48.0f * fM_Sd_2 * Math.Pow(fL_eff, 2.0f) / (Ecm_Ici_shortterm_crk);

            double y_el_shortterm_crk_des_ratio = y_el_shortterm_crk / y_el_lim;

            // Long-term deflection
            double fI_c_brutto_longterm_crk = Get_I_c_brutto((float)obj_panel.Cross_Section.b, x, fn_long_term, number_long_lower_bars, fd_1, number_long_upper_bars, fd_2);
            double y_s_longterm_crk = Get_y_s_x((float)obj_panel.Cross_Section.b, (float)obj_panel.Cross_Section.h, x, fn_long_term, fy_s1, fy_s2, A_s1, A_s2);
            double I_st_longterm_crk = Get_I_st_x((float)obj_panel.Cross_Section.b, (float)obj_panel.Cross_Section.h, x, fn_long_term, y_s_longterm_crk, fy_s1, fy_s2, A_s1, A_s2);
            double I_ci_longterm_crk = fI_c_brutto_longterm_crk + I_st_longterm_crk;
            double Eceff_Ici_longterm_crk = fE_c_eff * I_ci_longterm_crk;
            double y_el_longterm_crk = 5.0f / 48.0f * fM_Sd_3 * Math.Pow(fL_eff, 2.0f) / (Eceff_Ici_longterm_crk);

            double y_el_longterm_crk_des_ratio = y_el_longterm_crk / y_el_lim;

            // Combination of deflection uncracked / cracked

            double k = 1.0f - 0.8f * Math.Pow(M_cr / fM_Sd_2, 2);

            // Short-term deflection
            double y_el_comb_shortterm = Get_y_el_comb(k, y_el_shortterm, y_el_shortterm_crk);
            double y_el_comb_shortterm_des_ratio = y_el_comb_shortterm / y_el_lim;
            // Long-term deflection
            double y_el_comb_longterm = Get_y_el_comb(k, y_el_longterm, y_el_longterm_crk);
            double y_el_comb_longterm_des_ratio = y_el_comb_longterm / y_el_lim;

            // Output - Results
            MessageBox.Show("Ultimate Limit State\n\n" +

            "Design value of shear force V Sd 1 = " + Math.Round(0.001 * fV_Sd_1, 2) + " kN \n" +
            "Design resistance value of shear force V Rd 1 = " + Math.Round(0.001 * V_Rd_1, 2) + " kN \n" +
            "Maximum design ratio V Sd 1 / V Rd 1 = " + Math.Round(fDesign_Ratio_VSd_1_to_VRd1, 4) * 100 + " % \n\n" +

            "Tensile force F ld = " + Math.Round(0.001 * fF_ld_max, 2) + " kN \n" +
            "Anchorange force capacity F RA = " + Math.Round(0.001 * fF_RA_max, 2) + " kN \n" +
            "Maximum design ratio F ld / F RA = " + Math.Round(fDesign_Ratio_F_ld_F_RA_max, 4) * 100 + " % \n\n" +

            "Serviceability Limit State\n\n" +

            "Deflection under uncracked condition\n\n" +
            "Short-term deflection\n" +
            "Deflection y el,st = " + Math.Round(1000 * y_el_shortterm, 2) + " mm \n" +
            "Deflection limit y el,lim = " + Math.Round(1000 * y_el_lim, 2) + " mm \n" +
            "Maximum design ratio y el,st / y el,lim = " + Math.Round(y_el_shortterm_des_ratio, 4) * 100 + " % \n\n" +
            "Long-term deflection\n" +
            "Deflection y el,lt = " + Math.Round(1000 * y_el_longterm, 2) + " mm \n" +
            "Deflection limit y el,lim = " + Math.Round(1000 * y_el_lim, 2) + " mm \n" +
            "Maximum design ratio y el,lt / y el,lim = " + Math.Round(y_el_longterm_des_ratio, 4) * 100 + " % \n\n" +

            "Deflection under cracked condition\n\n" +
            "Short-term deflection\n" +
            "Deflection y el,st,cr = " + Math.Round(1000 * y_el_shortterm_crk, 2) + " mm \n" +
            "Deflection limit y el,lim = " + Math.Round(1000 * y_el_lim, 2) + " mm \n" +
            "Maximum design ratio y el,st,cr / y el,lim = " + Math.Round(y_el_shortterm_crk_des_ratio, 4) * 100 + " % \n\n" +
            "Long-term deflection\n" +
            "Deflection y el,lt,cr = " + Math.Round(1000 * y_el_longterm_crk, 2) + " mm \n" +
            "Deflection limit y el,lim = " + Math.Round(1000 * y_el_lim, 2) + " mm \n" +
            "Maximum design ratio y el,lt,cr / y el,lim = " + Math.Round(y_el_longterm_crk_des_ratio, 4) * 100 + " % \n\n" +

            "Combination of deflection uncracked / cracked\n\n" +
            "Short-term deflection\n" +
            "Deflection y el,st,com = " + Math.Round(1000 * y_el_comb_shortterm, 2) + " mm \n" +
            "Deflection limit y el,lim = " + Math.Round(1000 * y_el_lim, 2) + " mm \n" +
            "Maximum design ratio y el,st,com / y el,lim = " + Math.Round(y_el_comb_shortterm_des_ratio, 4) * 100 + " % \n\n" +
            "Long-term deflection\n" +
            "Deflection y el,lt,com = " + Math.Round(1000 * y_el_comb_longterm, 2) + " mm \n" +
            "Deflection limit y el,lim = " + Math.Round(1000 * y_el_lim, 2) + " mm \n" +
            "Maximum design ratio y el,lt,com / y el,lim = " + Math.Round(y_el_comb_longterm_des_ratio, 4) * 100 + " % \n");
        }

        private void Display_Graph_Click(object sender, RoutedEventArgs e)
        {
            Window a = new Window1();

            a.Show();
            //this.Close();
        }

        // Private Auxiliary Functions
        private double Get_I_c_brutto (float fb, double x, float n, int i_number_1, float fd_1, int i_number_2, float fd_2)
        {
            return 1.0f / 12.0f * fb * Math.Pow(x, 3.0) + n * (i_number_1 * Math.PI * 0.25f * Math.Pow(0.5f * fd_1, 4) + i_number_2 * Math.PI * 0.25f * Math.Pow(0.5f * fd_2, 4));
        }

        private double Get_y_s(float fb, float fh, float n, float fy_s1, float fy_s2, double A_s1, double A_s2)
        {
            return (fb * fh * 0.5f * fh + n * (A_s1 * fy_s1 + A_s2 * fy_s2)) / (fb * fh + n * (A_s1 + A_s2));
        }

        private double Get_y_s_x(float fb, float fh, double x, float n, float fy_s1, float fy_s2, double A_s1, double A_s2)
        {
            return (fb * x * (fh - 0.5f * x) + n * (A_s1 * fy_s1 + A_s2 * fy_s2)) / (fb * x + n * (A_s1 + A_s2));
        }

        private double Get_I_st(float fb, float fh, float n, double y_s, float fy_s1, float fy_s2, double A_s1, double A_s2)
        {
            return fb * fh * Math.Pow(0.5f * fh - y_s, 2.0f) + n * (A_s1 * Math.Pow(fy_s1 - y_s, 2.0f) + A_s2 * Math.Pow(fy_s2 - y_s, 2.0f));
        }

        private double Get_I_st_x(float fb, float fh, double x, float n, double y_s, float fy_s1, float fy_s2, double A_s1, double A_s2)
        {
            return fb * x * Math.Pow(fh - 0.5f * x - y_s, 2.0f) + n * (A_s1 * Math.Pow(fy_s1 - y_s, 2.0f) + A_s2 * Math.Pow(fy_s2 - y_s, 2.0f));
        }

        private double Get_y_el_comb(double k, double fp_I, double fp_II)
        {
            return k * fp_II + (1.0f - k) * fp_I;
        }

        private double Get_fld(float fK_c1, float fm, double e, double d, float fFactor_Alpha, double fck, float fGamma_c)
        {
            return fK_c1 * fm * Math.Pow(e / d, 1.0 / 3.0) * fFactor_Alpha * fck / fGamma_c;
        }

        private float Get_m(int np, int nt)
        {
            if (nt > 0)
                return 1.0f + 0.3f * np / nt;
            else
                return 0.0f;
        }

        public float Get_minimum_support_a_min(E_SupportMaterialType SupportMaterialIndex)
        {
            //AAC element

            //0 - "Floor Panel"
            //1 - "Roof Panel"
            //2 - "Vertical Wall Panel"
            //3 - "Horizontal Wall Panel"
            //4 - "Beam"

            //Support Material

            //0 - "masonry"
            //1 - "steel"
            //2 - "concrete"
            //3 - "wood"

            if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Beam)
                return 0.10f;
            else if (SupportMaterialIndex == E_SupportMaterialType.masonry)
                return 0.07f;
            else
                return 0.05f;
        }

        public void Minimum_Spacing_of_Long_Bars(float fd_bar, float fd_eff_section, out float fsl_min, out float fsl_max)
        {
            if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Floor_Panel || selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Roof_Panel)
            {
                fsl_min = 0.05f;
                fsl_max = 2.0f * fd_eff_section;
            }
            else if (selected_AAC_ComponentIndex == AAC_Panel.E_AACElementType.Beam)
            {
                fsl_min = 2.5f * fd_bar;
                fsl_max = 2.0f * fd_eff_section;
            }
            else // Walls
            {
                fsl_min = 0.05f;
                fsl_max = 0.7f;
            }
        }

        private void TextBoxCrossSectionHeight_h_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
        // Use the DataObject.Pasting Handler 
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }

    public class ComboItem
    {
        public string ItemText { get; set; }
        public float ItemFloat { get; set; }

        public ComboItem(string itemText, float itemFloat)
        {
            this.ItemText = itemText;
            this.ItemFloat = itemFloat;
        }

        public override string ToString()
        {
            return this.ItemText;
        }
    }
}
