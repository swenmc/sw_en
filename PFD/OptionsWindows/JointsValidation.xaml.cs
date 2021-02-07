using System;
using System.Collections.Generic;
using System.Data;
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
using MATH;

namespace PFD
{
    public partial class JointsValidation : Window
    {
        private CPFDViewModel _pfdVM;
        
        public JointsValidation(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;
            
            // Create Table
            DataTable dt = new DataTable("Joints");
            // Create Table Rows
            dt.Columns.Add("ID");
            dt.Columns.Add("JointType");
            dt.Columns.Add("NodeID");
            dt.Columns.Add("MainMemberID");            
            dt.Columns.Add("SecMembersIDs");
            dt.Columns.Add("Plates");

            dt.Columns.Add("ScrewsInPlates");            
            dt.Columns.Add("TotalScrews");

            dt.Columns.Add("Generate");
            dt.Columns.Add("Display");
            dt.Columns.Add("Design");
            dt.Columns.Add("MaterialList");
            
            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);
            
            DataRow row;
            foreach (CConnectionJointTypes joint in _pfdVM.Model.m_arrConnectionJoints)
            {
                row = dt.NewRow();
                row["ID"] = joint.ID;
                row["JointType"] = joint.JointType;
                row["NodeID"] = (joint.m_Node != null ? joint.m_Node.ID : 0);
                row["MainMemberID"] = (joint.m_MainMember != null ? joint.m_MainMember.ID : 0);
                row["SecMembersIDs"] = string.Join("; ", joint.GetSecondaryMembersIDs());
                row["Plates"] = string.Join("; ", joint.GetPlateNames());

                row["ScrewsInPlates"] = joint.GetScrewsInPlates();
                row["TotalScrews"] = joint.GetTotalNumberOfScrews();

                row["Generate"] = joint.BIsGenerated;
                row["Display"] = joint.BIsDisplayed;
                row["Design"] = joint.BIsSelectedForDesign;
                row["MaterialList"] = joint.BIsSelectedForMaterialList;

                dt.Rows.Add(row);
            }
            
            Datagrid_Joints.ItemsSource = ds.Tables[0].AsDataView();
            //Datagrid_Members.Loaded += Datagrid_Members_Loaded;

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        // Pomocna funkcia pre vypis suradnic bodu Point3D
        public string Point3D_ToString(System.Windows.Media.Media3D.Point3D p, int iCoordDecimalPlaces)
        {
            string sDecPlaces = "F" + iCoordDecimalPlaces;

            return $"[{p.X.ToString(sDecPlaces)};\t {p.Y.ToString(sDecPlaces)};\t {p.Z.ToString(sDecPlaces)}]";
        }

        public string Node3D_ToString(CNode n, int iCoordDecimalPlaces)
        {
            string sDecPlaces = "F" + iCoordDecimalPlaces;

            return $"ID: {n.ID} [{n.X.ToString(sDecPlaces)};\t {n.Y.ToString(sDecPlaces)};\t {n.Z.ToString(sDecPlaces)}]";
        }
    }
}
