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
using BaseClasses;
using CRSC;

namespace sw_en_GUI
{
	/// <summary>
	/// Interaction logic for WindowMain.xaml
	/// </summary>
	public partial class WindowCrossSectionDB : Window
	{
		public WindowCrossSectionDB()
		{
			InitializeComponent();
			imageButton00.Source = (ImageSource)TryFindResource("GEN_F_00");
			imageButton01.Source = (ImageSource)TryFindResource("GEN_F_01");
			imageButton02.Source = (ImageSource)TryFindResource("GEN_F_02");
			imageButton03.Source = (ImageSource)TryFindResource("GEN_F_03");
			imageButton04.Source = (ImageSource)TryFindResource("GEN_F_04");
			imageButton05.Source = (ImageSource)TryFindResource("GEN_F_05");
			imageButton10.Source = (ImageSource)TryFindResource("GEN_F_06");
			imageButton11.Source = (ImageSource)TryFindResource("GEN_F_07");
			imageButton12.Source = (ImageSource)TryFindResource("GEN_F_08");
			imageButton13.Source = (ImageSource)TryFindResource("GEN_F_20");
			imageButton14.Source = (ImageSource)TryFindResource("GEN_F_21");
			imageButton15.Source = (ImageSource)TryFindResource("GEN_F_22");
			imageButton20.Source = (ImageSource)TryFindResource("GEN_F_23");
			imageButton21.Source = (ImageSource)TryFindResource("GEN_F_24");
			imageButton22.Source = (ImageSource)TryFindResource("GEN_F_25");
			imageButton23.Source = (ImageSource)TryFindResource("GEN_F_26");
			imageButton24.Source = (ImageSource)TryFindResource("GEN_F_27");
			imageButton25.Source = (ImageSource)TryFindResource("GEN_F_28");
			imageButton30.Source = (ImageSource)TryFindResource("GEN_F_50");
			imageButton31.Source = (ImageSource)TryFindResource("GEN_F_51");
			imageButton32.Source = (ImageSource)TryFindResource("GEN_F_52");
			imageButton33.Source = (ImageSource)TryFindResource("GEN_F_53");
			imageButton34.Source = (ImageSource)TryFindResource("GEN_F_54");
			imageButton35.Source = (ImageSource)TryFindResource("GEN_F_55");
			imageButton40.Source = (ImageSource)TryFindResource("GEN_F_56");
			imageButton41.Source = (ImageSource)TryFindResource("GEN_F_57");
			imageButton42.Source = (ImageSource)TryFindResource("GEN_F_58");
			imageButton43.Source = (ImageSource)TryFindResource("GEN_F_59");
			imageButton44.Source = (ImageSource)TryFindResource("GEN_F_60");
			imageButton45.Source = (ImageSource)TryFindResource("GEN_F_61");


			imageButton67.Source = (ImageSource)TryFindResource("0_MASS");
			imageButton68.Source = (ImageSource)TryFindResource("0_THIN");


			//concrete
			imgBtnConcrete00.Source = (ImageSource)TryFindResource("CON_F_00");
			imgBtnConcrete01.Source = (ImageSource)TryFindResource("CON_F_01");
			imgBtnConcrete02.Source = (ImageSource)TryFindResource("CON_F_02");
			imgBtnConcrete03.Source = (ImageSource)TryFindResource("CON_F_03");
			imgBtnConcrete04.Source = (ImageSource)TryFindResource("CON_F_04");
			imgBtnConcrete05.Source = (ImageSource)TryFindResource("CON_F_05");
			imgBtnConcrete10.Source = (ImageSource)TryFindResource("CON_F_06");
			imgBtnConcrete11.Source = (ImageSource)TryFindResource("CON_F_07");
			imgBtnConcrete12.Source = (ImageSource)TryFindResource("CON_F_20");
			imgBtnConcrete13.Source = (ImageSource)TryFindResource("CON_F_21");
			imgBtnConcrete14.Source = (ImageSource)TryFindResource("CON_F_22");
			imgBtnConcrete15.Source = (ImageSource)TryFindResource("CON_F_23");
			imgBtnConcrete20.Source = (ImageSource)TryFindResource("CON_F_24");
			imgBtnConcrete21.Source = (ImageSource)TryFindResource("CON_F_25");
			imgBtnConcrete22.Source = (ImageSource)TryFindResource("CON_F_26");
			imgBtnConcrete23.Source = (ImageSource)TryFindResource("CON_F_27");
			imgBtnConcrete24.Source = (ImageSource)TryFindResource("CON_F_28");
			imgBtnConcrete25.Source = (ImageSource)TryFindResource("CON_F_40");
			imgBtnConcrete30.Source = (ImageSource)TryFindResource("CON_F_41");
			imgBtnConcrete31.Source = (ImageSource)TryFindResource("CON_F_42");
			imgBtnConcrete32.Source = (ImageSource)TryFindResource("CON_F_43");
			imgBtnConcrete33.Source = (ImageSource)TryFindResource("CON_F_44");
			imgBtnConcrete34.Source = (ImageSource)TryFindResource("CON_F_45");
			imgBtnConcrete35.Source = (ImageSource)TryFindResource("CON_F_46");
			imgBtnConcrete40.Source = (ImageSource)TryFindResource("CON_F_47");
			imgBtnConcrete41.Source = (ImageSource)TryFindResource("CON_F_48");
			imgBtnConcrete42.Source = (ImageSource)TryFindResource("CON_F_49");
			imgBtnConcrete43.Source = (ImageSource)TryFindResource("CON_F_50");
			imgBtnConcrete68.Source = (ImageSource)TryFindResource("0_MASS");


			//Steel
			imgBtnSteelHotRolled00.Source = (ImageSource)TryFindResource("STE_F_00");
			imgBtnSteelHotRolled01.Source = (ImageSource)TryFindResource("STE_F_01");
			imgBtnSteelHotRolled02.Source = (ImageSource)TryFindResource("STE_F_02");
			imgBtnSteelHotRolled03.Source = (ImageSource)TryFindResource("STE_F_03");
			imgBtnSteelHotRolled04.Source = (ImageSource)TryFindResource("STE_F_04");
			imgBtnSteelHotRolled05.Source = (ImageSource)TryFindResource("STE_F_05");
			imgBtnSteelHotRolled10.Source = (ImageSource)TryFindResource("STE_F_06");
			imgBtnSteelHotRolled11.Source = (ImageSource)TryFindResource("STE_F_07");
			imgBtnSteelHotRolled12.Source = (ImageSource)TryFindResource("STE_F_08");
			imgBtnSteelHotRolled13.Source = (ImageSource)TryFindResource("STE_F_09");
			imgBtnSteelHotRolled14.Source = (ImageSource)TryFindResource("STE_F_10");

			imgBtnSteelColdFormed00.Source = (ImageSource)TryFindResource("STE_F_79");
			imgBtnSteelColdFormed01.Source = (ImageSource)TryFindResource("STE_F_80");
			imgBtnSteelColdFormed02.Source = (ImageSource)TryFindResource("STE_F_87");
			imgBtnSteelColdFormed03.Source = (ImageSource)TryFindResource("STE_F_88");
			imgBtnSteelColdFormed04.Source = (ImageSource)TryFindResource("STE_F_89");
			imgBtnSteelColdFormed05.Source = (ImageSource)TryFindResource("STE_F_90");
			imgBtnSteelColdFormed10.Source = (ImageSource)TryFindResource("STE_F_91");
			imgBtnSteelColdFormed11.Source = (ImageSource)TryFindResource("STE_F_92");
			imgBtnSteelColdFormed12.Source = (ImageSource)TryFindResource("STE_F_72");
			imgBtnSteelColdFormed13.Source = (ImageSource)TryFindResource("STE_F_77");
			imgBtnSteelColdFormed14.Source = (ImageSource)TryFindResource("STE_F_78");

			imgBtnSteelParamWelded00.Source = (ImageSource)TryFindResource("STE_F_20");
			imgBtnSteelParamWelded01.Source = (ImageSource)TryFindResource("STE_F_21");
			imgBtnSteelParamWelded02.Source = (ImageSource)TryFindResource("STE_F_22");
			imgBtnSteelParamWelded03.Source = (ImageSource)TryFindResource("STE_F_23");
			imgBtnSteelParamWelded04.Source = (ImageSource)TryFindResource("STE_F_24");
			imgBtnSteelParamWelded05.Source = (ImageSource)TryFindResource("STE_F_25");
			imgBtnSteelParamWelded10.Source = (ImageSource)TryFindResource("STE_F_26");
			imgBtnSteelParamWelded11.Source = (ImageSource)TryFindResource("STE_F_27");
			imgBtnSteelParamWelded12.Source = (ImageSource)TryFindResource("STE_F_28");
			imgBtnSteelParamWelded13.Source = (ImageSource)TryFindResource("STE_F_29");
			imgBtnSteelParamWelded14.Source = (ImageSource)TryFindResource("STE_F_30");
			imgBtnSteelParamWelded15.Source = (ImageSource)TryFindResource("STE_F_31");
			imgBtnSteelParamWelded20.Source = (ImageSource)TryFindResource("STE_F_32");
			imgBtnSteelParamWelded21.Source = (ImageSource)TryFindResource("STE_F_33");
			imgBtnSteelParamWelded22.Source = (ImageSource)TryFindResource("STE_F_34");
			imgBtnSteelParamWelded23.Source = (ImageSource)TryFindResource("STE_F_40");
			imgBtnSteelParamWelded24.Source = (ImageSource)TryFindResource("STE_F_41");
			imgBtnSteelParamWelded25.Source = (ImageSource)TryFindResource("STE_F_42");
			imgBtnSteelParamWelded30.Source = (ImageSource)TryFindResource("STE_F_43");
			imgBtnSteelParamWelded31.Source = (ImageSource)TryFindResource("STE_F_44");
			imgBtnSteelParamWelded32.Source = (ImageSource)TryFindResource("STE_F_45");
			imgBtnSteelParamWelded33.Source = (ImageSource)TryFindResource("STE_F_46");
			imgBtnSteelParamWelded34.Source = (ImageSource)TryFindResource("STE_F_47");
			imgBtnSteelParamWelded35.Source = (ImageSource)TryFindResource("STE_F_48");
			imgBtnSteelParamWelded40.Source = (ImageSource)TryFindResource("STE_F_49");
			imgBtnSteelParamWelded41.Source = (ImageSource)TryFindResource("STE_F_50");

			imgBtnSteelParamThin00.Source = (ImageSource)TryFindResource("STE_F_70");
			imgBtnSteelParamThin01.Source = (ImageSource)TryFindResource("STE_F_71");
			imgBtnSteelParamThin02.Source = (ImageSource)TryFindResource("STE_F_72");
			imgBtnSteelParamThin03.Source = (ImageSource)TryFindResource("STE_F_73");
			imgBtnSteelParamThin04.Source = (ImageSource)TryFindResource("STE_F_74");
			imgBtnSteelParamThin05.Source = (ImageSource)TryFindResource("STE_F_75");
			imgBtnSteelParamThin10.Source = (ImageSource)TryFindResource("STE_F_76");
			imgBtnSteelParamThin11.Source = (ImageSource)TryFindResource("STE_F_77");
			imgBtnSteelParamThin12.Source = (ImageSource)TryFindResource("STE_F_78");
			imgBtnSteelParamThin13.Source = (ImageSource)TryFindResource("STE_F_79");
			imgBtnSteelParamThin14.Source = (ImageSource)TryFindResource("STE_F_80");
			imgBtnSteelParamThin15.Source = (ImageSource)TryFindResource("STE_F_81");
			imgBtnSteelParamThin20.Source = (ImageSource)TryFindResource("STE_F_82");
			imgBtnSteelParamThin21.Source = (ImageSource)TryFindResource("STE_F_83");
			imgBtnSteelParamThin22.Source = (ImageSource)TryFindResource("STE_F_84");
			imgBtnSteelParamThin23.Source = (ImageSource)TryFindResource("STE_F_85");
			imgBtnSteelParamThin24.Source = (ImageSource)TryFindResource("STE_F_86");
			imgBtnSteelParamThin25.Source = (ImageSource)TryFindResource("STE_F_87");
			imgBtnSteelParamThin30.Source = (ImageSource)TryFindResource("STE_F_88");
			imgBtnSteelParamThin31.Source = (ImageSource)TryFindResource("STE_F_89");
			imgBtnSteelParamThin32.Source = (ImageSource)TryFindResource("STE_F_90");
			imgBtnSteelParamThin33.Source = (ImageSource)TryFindResource("STE_F_91");
			imgBtnSteelParamThin34.Source = (ImageSource)TryFindResource("STE_F_92");
			imgBtnSteelParamThin40.Source = (ImageSource)TryFindResource("STE_F_93");
			imgBtnSteelParamThin41.Source = (ImageSource)TryFindResource("STE_F_94");
			imgBtnSteelParamThin42.Source = (ImageSource)TryFindResource("STE_F_95");
			imgBtnSteelParamThin43.Source = (ImageSource)TryFindResource("STE_F_96");
			imgBtnSteelParamThin44.Source = (ImageSource)TryFindResource("STE_F_97");
			imgBtnSteelParamThin45.Source = (ImageSource)TryFindResource("0_THIN");

			imgBtnSteelBuiltUp00.Source = (ImageSource)TryFindResource("STE_F_100");
			imgBtnSteelBuiltUp01.Source = (ImageSource)TryFindResource("STE_F_110");
			imgBtnSteelBuiltUp02.Source = (ImageSource)TryFindResource("STE_F_120");

			//Composite
			imgBtnCompositeColumns00.Source = (ImageSource)TryFindResource("COM_F_00");
			imgBtnCompositeColumns01.Source = (ImageSource)TryFindResource("COM_F_01");
			imgBtnCompositeColumns02.Source = (ImageSource)TryFindResource("COM_F_02");
			imgBtnCompositeColumns03.Source = (ImageSource)TryFindResource("COM_F_03");
			imgBtnCompositeColumns04.Source = (ImageSource)TryFindResource("COM_F_04");

			imgBtnCompositeBeams00.Source = (ImageSource)TryFindResource("COM_F_20");
			imgBtnCompositeBeams01.Source = (ImageSource)TryFindResource("COM_F_21");
			imgBtnCompositeBeams02.Source = (ImageSource)TryFindResource("COM_F_30");
			imgBtnCompositeBeams03.Source = (ImageSource)TryFindResource("COM_F_31");

			//timber
			imgBtnTimberSolid00.Source = (ImageSource)TryFindResource("TIM_F_00");
			imgBtnTimberSolid01.Source = (ImageSource)TryFindResource("TIM_F_01");

			imgBtnTimberGlued00.Source = (ImageSource)TryFindResource("TIM_F_02");
			imgBtnTimberGlued01.Source = (ImageSource)TryFindResource("TIM_F_20");
			imgBtnTimberGlued02.Source = (ImageSource)TryFindResource("TIM_F_21");
			imgBtnTimberGlued03.Source = (ImageSource)TryFindResource("TIM_F_22");
			imgBtnTimberGlued04.Source = (ImageSource)TryFindResource("TIM_F_40");
			imgBtnTimberGlued05.Source = (ImageSource)TryFindResource("TIM_F_41");

			imgBtnTimberBuiltUp00.Source = (ImageSource)TryFindResource("TIM_F_60");
			imgBtnTimberBuiltUp01.Source = (ImageSource)TryFindResource("TIM_F_61");

			//Aluminium
			imgBtnAluminiumExtruded00.Source = (ImageSource)TryFindResource("STE_F_79");
			imgBtnAluminiumExtruded01.Source = (ImageSource)TryFindResource("STE_F_80");
			imgBtnAluminiumExtruded02.Source = (ImageSource)TryFindResource("STE_F_87");
			imgBtnAluminiumExtruded03.Source = (ImageSource)TryFindResource("STE_F_88");
			imgBtnAluminiumExtruded04.Source = (ImageSource)TryFindResource("STE_F_90");
			imgBtnAluminiumExtruded10.Source = (ImageSource)TryFindResource("STE_F_91");
			imgBtnAluminiumExtruded11.Source = (ImageSource)TryFindResource("STE_F_92");
			imgBtnAluminiumExtruded12.Source = (ImageSource)TryFindResource("STE_F_72");
			imgBtnAluminiumExtruded13.Source = (ImageSource)TryFindResource("STE_F_77");
			imgBtnAluminiumExtruded14.Source = (ImageSource)TryFindResource("STE_F_78");

			imgBtnAluminiumParam00.Source = (ImageSource)TryFindResource("STE_F_70");
			imgBtnAluminiumParam01.Source = (ImageSource)TryFindResource("STE_F_71");
			imgBtnAluminiumParam02.Source = (ImageSource)TryFindResource("STE_F_72");
			imgBtnAluminiumParam03.Source = (ImageSource)TryFindResource("STE_F_73");
			imgBtnAluminiumParam04.Source = (ImageSource)TryFindResource("STE_F_74");
			imgBtnAluminiumParam05.Source = (ImageSource)TryFindResource("STE_F_75");
			imgBtnAluminiumParam10.Source = (ImageSource)TryFindResource("STE_F_76");
			imgBtnAluminiumParam11.Source = (ImageSource)TryFindResource("STE_F_77");
			imgBtnAluminiumParam12.Source = (ImageSource)TryFindResource("STE_F_78");
			imgBtnAluminiumParam13.Source = (ImageSource)TryFindResource("STE_F_79");
			imgBtnAluminiumParam14.Source = (ImageSource)TryFindResource("STE_F_80");
			imgBtnAluminiumParam15.Source = (ImageSource)TryFindResource("STE_F_81");
			imgBtnAluminiumParam20.Source = (ImageSource)TryFindResource("STE_F_82");
			imgBtnAluminiumParam21.Source = (ImageSource)TryFindResource("STE_F_83");
			imgBtnAluminiumParam22.Source = (ImageSource)TryFindResource("STE_F_84");
			imgBtnAluminiumParam23.Source = (ImageSource)TryFindResource("STE_F_85");
			imgBtnAluminiumParam24.Source = (ImageSource)TryFindResource("STE_F_86");
			imgBtnAluminiumParam25.Source = (ImageSource)TryFindResource("STE_F_87");
			imgBtnAluminiumParam30.Source = (ImageSource)TryFindResource("STE_F_88");
			imgBtnAluminiumParam31.Source = (ImageSource)TryFindResource("STE_F_89");
			imgBtnAluminiumParam32.Source = (ImageSource)TryFindResource("STE_F_90");
			imgBtnAluminiumParam33.Source = (ImageSource)TryFindResource("STE_F_91");
			imgBtnAluminiumParam34.Source = (ImageSource)TryFindResource("STE_F_92");
			imgBtnAluminiumParam40.Source = (ImageSource)TryFindResource("STE_F_93");
			imgBtnAluminiumParam41.Source = (ImageSource)TryFindResource("STE_F_94");
			imgBtnAluminiumParam42.Source = (ImageSource)TryFindResource("STE_F_95");
			imgBtnAluminiumParam43.Source = (ImageSource)TryFindResource("STE_F_96");
			imgBtnAluminiumParam44.Source = (ImageSource)TryFindResource("STE_F_97");
			imgBtnAluminiumParam45.Source = (ImageSource)TryFindResource("0_THIN");

		}

		
		

		private void listBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (listBoxMenu.SelectedIndex) 
			{
				case 0:
					hideAllGroupBoxes();
					groupBoxGeneral.Visibility = Visibility.Visible;
					break;
				case 1:
					hideAllGroupBoxes();
					groupBoxConcrete.Visibility = Visibility.Visible;
					break;
				case 2:
					hideAllGroupBoxes();
					gridSteel.Visibility = Visibility.Visible;
					break;
				case 3:
					hideAllGroupBoxes();
					gridComposite.Visibility = Visibility.Visible;
					break;
				case 4:
					hideAllGroupBoxes();
					gridTimber.Visibility = Visibility.Visible;
					break;
				case 5:
					hideAllGroupBoxes();
					gridAluminium.Visibility = Visibility.Visible;
					break;
				default:
					MessageBox.Show("Not supported value: "+listBoxMenu.SelectedIndex);
					break;
			}
		}

		private void hideAllGroupBoxes()
		{
			groupBoxGeneral.Visibility = Visibility.Hidden;
			groupBoxConcrete. Visibility = Visibility.Hidden;
			gridSteel.Visibility = Visibility.Hidden;
			gridComposite.Visibility = Visibility.Hidden;
			gridTimber.Visibility = Visibility.Hidden;
			gridAluminium.Visibility = Visibility.Hidden;
		}

        private void btnGeneral_CS0_Click(object sender, RoutedEventArgs e)
        {
            CSForm cso_form = new CSForm();
            cso_form.Show();
        }

        private void btnSteel_CSO_Click(object sender, RoutedEventArgs e)
        {
            CSForm cso_form = new CSForm();
            cso_form.Show();
        }

        private void btnAluminium_CSO_Click(object sender, RoutedEventArgs e)
        {
            CSForm cso_form = new CSForm();
            cso_form.Show();
        }


        

       
	}
}
