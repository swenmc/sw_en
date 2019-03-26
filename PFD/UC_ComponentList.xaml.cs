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
using System.Data.SqlClient;
using System.Data;
using BaseClasses;
using DATABASE;
using MATH;
using System.ComponentModel;
using DATABASE.DTO;

namespace PFD
{
    /// <summary>
    /// Interaction logic for ComponentList.xaml
    /// </summary>
    public partial class UC_ComponentList : UserControl
    {
        public UC_ComponentList()
        {
            InitializeComponent();
            
            // Obsah tejto tabulky co sa tyka prierezov by sa mal prebrat z modelu CExample_3D_901_PF - m_arrSections
            // Material prevziat z prierezov

            // TODO - Ondrej - obsah tohto zoznamu preberat z CExample_3D_901_PF - zoznam listOfModelMemberGroups
            CComponentListVM cl = new CComponentListVM();
            cl.PropertyChanged += HandleComponentListPropertyChangedEvent;
            this.DataContext = cl;
            cl.SelectedComponentIndex = 0;
        }

        protected void HandleComponentListPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CComponentListVM viewModel = sender as CComponentListVM;
            if (viewModel == null) return;
        }
    }
}
