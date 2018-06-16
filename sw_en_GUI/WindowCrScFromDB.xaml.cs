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

namespace sw_en_GUI
{
	/// <summary>
	/// Interaction logic for WindowCrScFromDB.xaml
	/// </summary>
	public partial class WindowCrScFromDB : Window
	{
		public WindowCrScFromDB()
		{
			InitializeComponent();
		}

		private void buttonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
