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

namespace PFD
{
    /// <summary>
    /// Interaction logic for MaterialList.xaml
    /// </summary>
    public partial class MaterialList : Window
    {
        List<string> listMemberPrefix = new List<string>(1);
        List<string> listMemberCrScName = new List<string>(1);
        List<int>    listMemberQuantity = new List<int>(1);
        List<string> listMemberMaterialName = new List<string>(1);
        List<float>  listMemberLength = new List<float>(1);
        List<float>  listMemberWeightPerLength = new List<float>(1);
        List<float>  listMemberWeightPerPiece = new List<float>(1);
        List<float>  listMemberTotalLength = new List<float>(1);
        List<float>  listMemberTotalWeight = new List<float>(1);

        List<string> listPlatePrefix = new List<string>(1);
        List<string> listPlateQuantity = new List<string>(1);
        List<string> listMaterialName = new List<string>(1);
        List<string> listPlateWidth_bx = new List<string>(1);
        List<string> listPlateHeight_hy = new List<string>(1);
        List<string> listPlateArea = new List<string>(1);
        List<string> listPlateWeightPerPiece = new List<string>(1);
        List<string> listPlateTotalWeight = new List<string>(1);

        DatabaseComponents databaseCopm = new DatabaseComponents();

        public MaterialList()
        {
            InitializeComponent();
        }

        public MaterialList(CModel model)
        {
            InitializeComponent();

            // For each cross-section shape / size add one row
            for (int i = 0; i < model.m_arrCrSc.Length; i++)
            {
                listMemberCrScName[i] = model.m_arrCrSc[i].Name;

                for (int j = 0; j < model.m_arrCrSc[i].AssignedMembersList.Count; j++) // Each member in the list
                {
                    listMemberPrefix.Add(databaseCopm.arr_Member_Types_Prefix[(int)model.m_arrMembers[j].eMemberType_FS, 0]);
                    listMemberQuantity[i] += j;
                    listMemberMaterialName[i] = model.m_arrCrSc[i].m_Mat.Name;
                    listMemberLength[i] =
                    listMemberWeightPerLength[i] =
                    listMemberWeightPerPiece[i] = listMemberLength[i] * listMemberWeightPerLength[i];
                    listMemberTotalLength[i] = listMemberQuantity[i] * listMemberLength[i];
                    listMemberTotalWeight[i] = (listMemberTotalLength[i] * listMemberWeightPerLength[i]);
                }
            }
        }

        private void DeleteCalculationResults()
        {
            //Todo - asi sa to da jednoduchsie
            DeleteLists();

            Datagrid_Members.ItemsSource = null;
            Datagrid_Members.Items.Clear();
            Datagrid_Members.Items.Refresh();

            Datagrid_Plates.ItemsSource = null;
            Datagrid_Plates.Items.Clear();
            Datagrid_Plates.Items.Refresh();

            Datagrid_Screws.ItemsSource = null;
            Datagrid_Screws.Items.Clear();
            Datagrid_Screws.Items.Refresh();
        }

        // Deleting lists for updating actual values
        private void DeleteLists()
        {
            listMemberPrefix.Clear();
            listMemberCrScName.Clear();
            listMemberQuantity.Clear();
            listMemberMaterialName.Clear();
            listMemberLength.Clear();
            listMemberWeightPerLength.Clear();
            listMemberWeightPerPiece.Clear();
            listMemberTotalLength.Clear();
            listMemberTotalWeight.Clear();

            listPlatePrefix.Clear();
            listPlateQuantity.Clear();
            listMaterialName.Clear();
            listPlateWidth_bx.Clear();
            listPlateHeight_hy.Clear();
            listPlateArea.Clear();
            listPlateWeightPerPiece.Clear();
            listPlateTotalWeight.Clear();
        }
    }
}
