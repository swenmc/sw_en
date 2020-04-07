using BaseClasses;
using EXPIMP;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
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


    }
}
