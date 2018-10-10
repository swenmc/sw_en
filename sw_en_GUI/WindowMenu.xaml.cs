using System;
using System.Windows;
using System.Windows.Media;

using System.Data;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for WindowMenu.xaml
    /// </summary>
    public partial class WindowMenu : Window
	{
        private bool m_bDebugging = false; // Console Output

		public WindowMenu()
		{
			InitializeComponent();
			
			imageButton00.Source = (ImageSource)TryFindResource("GEN_F_00");
			imageButton01.Source = (ImageSource)TryFindResource("GEN_F_01");
			imageButton02.Source = (ImageSource)TryFindResource("GEN_F_02");
			imageButton10.Source = (ImageSource)TryFindResource("GEN_F_03");
			imageButton11.Source = (ImageSource)TryFindResource("GEN_F_04");
			imageButton12.Source = (ImageSource)TryFindResource("GEN_F_05");
			imageButton20.Source = (ImageSource)TryFindResource("GEN_F_06");
			imageButton21.Source = (ImageSource)TryFindResource("GEN_F_07");
			imageButton22.Source = (ImageSource)TryFindResource("GEN_F_08");
		}

		private void Button00_Click(object sender, RoutedEventArgs e)
		{
			Viewer3D view = new Viewer3D();
			view.Show();
		}

		private void Button01_Click(object sender, RoutedEventArgs e)
		{
            Window2 view = new Window2(m_bDebugging);
            view.Show();
		}

        private void Button02_Click(object sender, RoutedEventArgs e)
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //path = path.Substring(0, path.LastIndexOf("\\"));
            //path = path.Substring(0, path.LastIndexOf("\\"));
            //path = path.Substring(0, path.LastIndexOf("\\"));
            //MessageBox.Show(path);
            //path +="\\Resources\\test.db";

            //TODO Mato - presunut do projektu DATABASE vsetky nacitavania z databazy
            //presunut do 
   //         string connString = String.Format("Data Source={0};New=True;Version=3", "test.db");				

			//SQLiteConnection sqlconn = new SQLiteConnection(connString);			
			//sqlconn.Open();

			//SQLiteCommand cmd = sqlconn.CreateCommand();
			//string CommandText = "SELECT * FROM CONCRETE";
			//cmd.CommandText = CommandText;

			////SQLiteDataReader reader = cmd.ExecuteReader();
			////List<string> list = new List<string>();
			////while (reader.Read()) 
			////{
			////    list.Add(reader["mat_name"].ToString());
			////}

			////dataGrid1.ItemsSource = list;

			//SQLiteDataAdapter DB = new SQLiteDataAdapter(CommandText, sqlconn);
			//DataSet DS = new DataSet();
			//DS.Reset();
			//DB.Fill(DS);
			//DataTable DT = DS.Tables[0];
			//dataGrid1.ItemsSource = DT.DefaultView; 
			//sqlconn.Close();
        }

        private void Button03_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button04_Click(object sender, RoutedEventArgs e)
        {

        }

		private void buttonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
