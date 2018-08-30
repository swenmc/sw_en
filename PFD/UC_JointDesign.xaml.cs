using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
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
using BaseClasses;
using M_AS4600;
using CRSC;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_JointDesign.xaml
    /// </summary>
    public partial class UC_JointDesign : UserControl
    {
        CModel_PFD Model;

        public UC_JointDesign() { } // TODO - Refaktorovat, tento konstruktor je pouzity v projekte SBD

        public UC_JointDesign(CModel_PFD model, CComponentListVM compList)
        {
            InitializeComponent();

            Model = model;

            // Joint Design
            CPFDJointsDesign jdinput = new CPFDJointsDesign(model.m_arrLimitStates, model.m_arrLoadCombs, compList.ComponentList);
            jdinput.PropertyChanged += HandleLoadInputPropertyChangedEvent;
            this.DataContext = jdinput;
        }

        protected void HandleLoadInputPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CPFDJointsDesign jdinput = sender as CPFDJointsDesign;
            if (jdinput != null && jdinput.IsSetFromCode) return;

            CMember cMember = new CMember();

            // K diskusii
            // Moznosti su zobrazovat vysledky podla typu spoja alebo po prutoch pre spoje na konci a na zaciatku

            // Prva moznost 
            // Pre vybrany typ spoja prejst vsetky vsetky spoje daneho typu a vybrat najhorsi


            // Druha moznost
            // Pre vybrany prut potrebujeme mat pristupne designInternalForces na jeho zaciatku a na konci

            // pre sadu na zaciatku a posudi spoj, ktory je na zaciatku pruta

            // pre sadu na konci sa posudi spoj, ktory je na konci pruta

            // TODO Ondrej - najst pre dany typ spoja vsetky uzly na ktorych je definovany


            // TODO Ondrej - najst pre zaciatocny CNode a koncovy CNode na prute priradeny Joint (CConectionJointTypes)
            // Asi by sa to malo priradit uz priamo v CModel_PFD, resp by tam mala byt tato funckia dostupna

            designInternalForces sDIF_temp = new designInternalForces();
            CCalculJoint cGoverningMemberStartJointResults = new CCalculJoint(Model.m_arrConnectionJoints[0], sDIF_temp);
        }
    }
}
