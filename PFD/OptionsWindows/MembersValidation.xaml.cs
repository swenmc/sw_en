﻿using System;
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
    public partial class MembersValidation : Window
    {
        private CPFDViewModel _pfdVM;
        
        public MembersValidation(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;
            
            // Create Table
            DataTable dt = new DataTable("Members");
            // Create Table Rows
            dt.Columns.Add("ID");
            dt.Columns.Add("MType");
            dt.Columns.Add("MTypePosition");
            dt.Columns.Add("MTypeFEM");
            dt.Columns.Add("CrSc");
            //dt.Columns.Add("CrScEnd");
            dt.Columns.Add("Mat");

            dt.Columns.Add("NodeStart");
            dt.Columns.Add("NodeEnd");
            
            dt.Columns.Add("IsGenerated");
            dt.Columns.Add("IsDisplayed");
            dt.Columns.Add("IsSelectedForDesign");
            dt.Columns.Add("IsSelectedForIFCalculation");
            dt.Columns.Add("IsSelectedForMaterialList");

            dt.Columns.Add("PointStart");
            dt.Columns.Add("PointEnd");


            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(dt);
            
            DataRow row;
            foreach (CMember m in _pfdVM.Model.m_arrMembers)
            {
                row = dt.NewRow();
                row["ID"] = m.ID;
                row["MType"] = m.EMemberType;
                row["MTypePosition"] = m.EMemberTypePosition;
                row["MTypeFEM"] = m.EMemberType_FEM;

                string mat = m.CrScStart != null ? m.CrScStart.m_Mat?.Name : m.CrScEnd?.m_Mat?.Name;
                row["CrSc"] = m.CrScStart?.Name_short;
                //row["CrScEnd"] = m.CrScEnd?.Name_short;
                row["Mat"] = mat;

                row["NodeStart"] = m.NodeStart?.ToString();
                row["NodeEnd"] = m.NodeEnd?.ToString();
                
                row["IsGenerated"] = m.BIsGenerated;
                row["IsDisplayed"] = m.BIsDisplayed;
                row["IsSelectedForDesign"] = m.BIsSelectedForDesign;
                row["IsSelectedForIFCalculation"] = m.BIsSelectedForIFCalculation;
                row["IsSelectedForMaterialList"] = m.BIsSelectedForMaterialList;

                row["PointStart"] = m.PointStart.ToString();
                row["PointEnd"] = m.PointEnd.ToString();



                dt.Rows.Add(row);
            }
            
            Datagrid_Members.ItemsSource = ds.Tables[0].AsDataView();
            //Datagrid_Members.Loaded += Datagrid_Members_Loaded;

            if (this.Height > System.Windows.SystemParameters.PrimaryScreenHeight - 30) this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

       

        
    }
}
