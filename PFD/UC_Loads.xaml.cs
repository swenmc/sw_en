using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_Loads.xaml
    /// </summary>
    public partial class UC_Loads : UserControl
    {
        public UC_Loads()
        {
            InitializeComponent();

            DatabaseLocations dlocations = new DatabaseLocations();
            foreach (string locationname in dlocations.arr_LocationNames)
                Combobox_Location.Items.Add(locationname);

            // Loading
            CPFDLoadInput loadinput = new CPFDLoadInput(11); // Default - Auckland
            loadinput.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = loadinput; // Toto prepise aj predchadzajuce nastavenie z vm, musi to byt inak Todo Ondrej
            
        }
        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDLoadInput loadInput = sender as CPFDLoadInput;
            if (loadInput != null && loadInput.IsSetFromCode) return;
        }
    }
}
