using BaseClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace EXPIMP
{
    public static class CExportToNC
	{
        //--------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------
        public static string ExportPlateToNC(CPlate plate, float fUnitFactor, string folder)
        {
            //[name]_[width]x[height]_[type]_[number] example APEX_1400x720_HOLES_001 APEX_1400x720_SETUP_001 KNEE_850x1410_HOLES_001 KNEE_850x1410_SETUP_001 , 
            //vytvorit novy s dalsim vyssim cislom, generovat tak ze dvojica ma vzdy rovnaky suffix
            //APEX je name pre J plates a KNEE je name pre K plates
            string result = string.Empty;
            try
            {
                int count = 0;
                string fileNameHoles = null;
                string fileNameSetup = null;
                string fileNameHolesPath = null;
                string fileNameSetupPath = null;
                bool namesOK = false;
                while (!namesOK)
                {
                    count++;
                    fileNameHoles = string.Format("{0}_{1}x{2}_HOLES_{3:D3}.NC", GetPlateSerieName(plate), Math.Round(plate.Width_bx * fUnitFactor, 3).ToString("F0"), Math.Round(plate.Height_hy * fUnitFactor, 3).ToString("F0"), count);
                    fileNameSetup = string.Format("{0}_{1}x{2}_SETUP_{3:D3}.NC", GetPlateSerieName(plate), Math.Round(plate.Width_bx * fUnitFactor, 3).ToString("F0"), Math.Round(plate.Height_hy * fUnitFactor, 3).ToString("F0"), count);
                    fileNameHolesPath = string.Format("{0}\\{1}", folder, fileNameHoles);
                    fileNameSetupPath = string.Format("{0}\\{1}", folder, fileNameSetup);

                    if (!File.Exists(fileNameHolesPath) && !File.Exists(fileNameSetupPath)) namesOK = true;
                }

                if (plate.DrillingRoutePoints != null)
                {
                    StringBuilder sbHoles = GetCNCFileContentForHoles(plate.DrillingRoutePoints, plate.Ft, fUnitFactor);
                    File.WriteAllText(fileNameHolesPath, sbHoles.ToString());
                    result += string.Format("File {0} has been created.\n", fileNameHoles);
                }

                StringBuilder sbSetup = GetCNCFileContentForSetup(plate.PointsOut2D, fUnitFactor);
                File.WriteAllText(fileNameSetupPath, sbSetup.ToString());
                result += string.Format("File {0} has been created.\n", fileNameSetup);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        private static string GetPlateSerieName(CPlate plate)
        {
            if (plate.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_J) return "APEX";
            else if (plate.m_ePlateSerieType_FS == ESerieTypePlate.eSerie_K) return "KNEE";
            else return "PLATE";
        }


        public static void ExportHolesToNC(List<Point> points, float fPlateThickness, float fUnitFactor)
        {
            DateTime d = DateTime.Now;
            string fileName = string.Format("ExportHOLES_{0}{1}{2}T{3}{4}{5}.NC",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

            StringBuilder sb = GetCNCFileContentForHoles(points, fPlateThickness, fUnitFactor);
            File.WriteAllText(fileName, sb.ToString());
        }
        public static StringBuilder GetCNCFileContentForHoles(List<Point> points, float fPlateThickness, float fUnitFactor)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            string str_Z = string.Format(nfi, "z-{0:F1}", Math.Round(fPlateThickness * fUnitFactor, 2)); // Drilling depth is defined by plate thickness
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("(HOLE PROGRAM)");
            sb.AppendLine("(Touch Z to top of touch plate)");
            sb.AppendLine("(Make sure you run setup program first)");
            sb.AppendLine("m4 s1000");
            sb.AppendLine("G21 G90 G64 G40 G17");
            for (int i = 1; i < points.Count; i++) // Start with index "1", skip first item - point [0,0] - do not drill in this point
            {
                if (i == 1) sb.AppendFormat(nfi, "G81 X{0:F3} Y{1:F3} {2} R40.0 F100.0{3}", Math.Round(points[i].X * fUnitFactor, 3), Math.Round(points[i].Y * fUnitFactor, 3), str_Z, Environment.NewLine);
                else sb.AppendFormat(nfi, "G81 X{0:F3} Y{1:F3} {2}{3}", Math.Round(points[i].X * fUnitFactor, 3), Math.Round(points[i].Y * fUnitFactor, 3), str_Z, Environment.NewLine);
            }
            sb.AppendLine("G00 Z40");
            sb.AppendLine("M05(Stop Spindle)");
            sb.AppendLine("M02(end of prog)");
            return sb;
        }
        public static void ExportSetupToNC(Point[] points, float fUnitFactor)
        {
            DateTime d = DateTime.Now;
            string fileName = string.Format("ExportSETUP_{0}{1}{2}T{3}{4}{5}.NC",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));

            StringBuilder sb = GetCNCFileContentForSetup(points, fUnitFactor);
            File.WriteAllText(fileName, sb.ToString());
        }
        public static StringBuilder GetCNCFileContentForSetup(Point[] points, float fUnitFactor)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            string str_Z = "z35.0";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("(SETUP PROGRAM)");
            sb.AppendLine("(Touch Z to top of touch plate)");
            sb.AppendLine("G21 G90 G64 G40 G17");
            for (int i = 0; i < points.Length; i++)
            {
                if (i == 0) sb.AppendFormat(nfi, "G81 X{0:F3} Y{1:F3} {2} R40.0 F500.0{3}", Math.Round(points[i].X * fUnitFactor, 3), Math.Round(points[i].Y * fUnitFactor, 3), str_Z, Environment.NewLine);
                else sb.AppendFormat(nfi, "G81 X{0:F3} Y{1:F3} {2}{3}", Math.Round(points[i].X * fUnitFactor, 3), Math.Round(points[i].Y * fUnitFactor, 3), str_Z, Environment.NewLine);
            }
            sb.AppendLine("G00 Z40");
            sb.AppendLine("M02(end of prog)");

            return sb;
        }
    }
}