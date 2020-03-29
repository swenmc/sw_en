using BaseClasses;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
       

    }
}
