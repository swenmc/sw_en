using BaseClasses;
using BaseClasses.Results;
using EXPIMP;
using MATH;
using PFD.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PFD
{
    public static class DocumentationHelper
    {
        public static DirectoryInfo CreateDocumentationFolder(string parent_folder, string projectNumber)
        {
            int version = 1;
            string documentationFolder = $"{projectNumber}_version_{version}";
            while (Directory.Exists($"{parent_folder}\\{documentationFolder}"))
            {
                version++;
                documentationFolder = $"{projectNumber}_version_{version}";
            }

            DirectoryInfo di = Directory.CreateDirectory($"{parent_folder}\\{documentationFolder}");
            return di;
        }

        public static void CreateCNFilesDocumentation(List<CPlate> plates, string parent_folder)
        {
            float fUnitFactor = 1000; // defined in m, exported in mm

            DirectoryInfo di = Directory.CreateDirectory($"{parent_folder}\\CNC");

            foreach (CPlate plate in plates)
            {
                //Export Plate to NC = create Setup and Holes NC files
                CExportToNC.ExportPlateToNC(plate, fUnitFactor, di.FullName);
            }
        }

        public static void SavePlatesFiles(List<CPlate> plates, string parent_folder)
        {
            //CProductionInfo pInfo = new CProductionInfo(vm.JobNumber, vm.Customer, vm.Amount, vm.AmountRH, vm.AmountLH);
            foreach (CPlate plate in plates)
            {
                string fileName = GetPlateFileName(plate.Name, parent_folder);

                object[] arr = new object[2];
                arr[0] = plate;
                arr[1] = null;

                using (Stream stream = File.Open(fileName, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, arr);
                    stream.Close();
                }
            }
        }

        public static void SavePlatesDXF_2D(List<CPlate> plates, string parent_folder)
        {
            foreach (CPlate plate in plates)
            {
                Canvas dxfCanvas = Drawing2D.DrawRealPlateToCanvas(plate, true, true, true, true, true, true, true, true, true, true, true);

                CExportToDXF.ExportCanvas_DXF(dxfCanvas, 0, 0, GetPlateFileName_DXF(plate.Name, parent_folder));
            }
        }

        public static void SavePlatesDXF_3D(List<CPlate> plates, string parent_folder)
        {
            DisplayOptions sDisplayOptions = new DisplayOptions();
            // Create 3D window
            sDisplayOptions.bDisplayGlobalAxis = false;
            sDisplayOptions.bUseEmissiveMaterial = true;
            sDisplayOptions.bUseLightAmbient = true;
            sDisplayOptions.bDisplayConnectors = true;
            sDisplayOptions.bDisplayWireFrameModel = true;

            sDisplayOptions.NodeColor = Colors.Red;
            sDisplayOptions.NodesDescriptionSize = 1f / 5;  //tu je zmeneny riadok vyssie kvoli 701
            sDisplayOptions.NodeDescriptionTextColor = Colors.Red;

            foreach (CPlate plate in plates)
            {
                Page3Dmodel page3D = new Page3Dmodel(plate, sDisplayOptions);
                page3D.UpdateLayout();
                CExportToDXF.ExportViewPort_DXF(page3D._trackport.ViewPort, GetPlateFileName_DXF_3D(plate.Name, parent_folder));
            }
        }

        private static string GetPlateFileName(string plateName, string parent_folder)
        {
            int count = 0;
            bool nameOK = false;
            string fileName = $"{parent_folder}\\Plate_{plateName}.scw"; ;
            while (!nameOK)
            {
                if (!System.IO.File.Exists(fileName)) nameOK = true;
                else fileName = $"{parent_folder}\\Plate_{plateName}_{++count}.scw";
            }
            return fileName;
        }
        private static string GetPlateFileName_DXF(string plateName, string parent_folder)
        {
            int count = 0;
            bool nameOK = false;
            string fileName = $"{parent_folder}\\Plate_{plateName}_2D.dxf"; ;
            while (!nameOK)
            {
                if (!System.IO.File.Exists(fileName)) nameOK = true;
                else fileName = $"{parent_folder}\\Plate_{plateName}_{++count}_2D.dxf";
            }
            return fileName;
        }
        private static string GetPlateFileName_DXF_3D(string plateName, string parent_folder)
        {
            int count = 0;
            bool nameOK = false;
            string fileName = $"{parent_folder}\\Plate_{plateName}_3D.dxf"; ;
            while (!nameOK)
            {
                if (!System.IO.File.Exists(fileName)) nameOK = true;
                else fileName = $"{parent_folder}\\Plate_{plateName}_{++count}_3D.dxf";
            }
            return fileName;
        }

        public static void ExportMembersExcelDocument(CMaterialListViewModel materialListVM, string parentFolder)
        {
            List<string[]> tableParams = new List<string[]>();
            tableParams.Add(new string[] { "Prefix", "Cross-section", "Count [-]", "Material Name", "Length [m]", "Unit Mass [kg/m]", "Mass Per Piece", "Total Length [m]", "Total Mass [kg]"});
            foreach (MaterialListMember m in materialListVM.MembersMaterialList)
            {
                string[] arr = new string[] {m.Prefix, m.CrScName, m.Quantity.ToString(), m.MaterialName, m.LengthStr, m.MassPerLengthStr,
                            m.MassPerPieceStr, m.TotalLength.ToString("F2"), m.TotalMass.ToString("F2") };
                tableParams.Add(arr);
            }
            ExportToExcelDocument.ExportToExcel($"{parentFolder}//MembersData.xlsx", tableParams, "Data");
        }
    }
}
