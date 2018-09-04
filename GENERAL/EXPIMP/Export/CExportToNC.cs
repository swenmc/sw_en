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
        public static void ExportHolesToNC(List<Point> points)
        {            
            DateTime d = DateTime.Now;
            string fileName = string.Format("ExportHOLES_{0}{1}{2}T{3}{4}{5}.NC",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));            

            StringBuilder sb = GetCNCFileContentForHoles(points);            
            File.WriteAllText(fileName, sb.ToString());
        }
        public static StringBuilder GetCNCFileContentForHoles(List<Point> points)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            string str_Z = "z - 3.0";
            StringBuilder sb = new StringBuilder();            
            sb.AppendLine("(HOLE PROGRAM)");
            sb.AppendLine("(Touch Z to top of touch plate)");
            sb.AppendLine("(Make sure you run setup program first)");
            sb.AppendLine("m4 s1000");
            sb.AppendLine("G21 G90 G64 G40 G17");
            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0) sb.AppendFormat(nfi, "G81 X{0:F3} Y{1:F3} {2} R40.0 F100.0{3}", Math.Round(points[i].X, 3), Math.Round(points[i].Y, 3), str_Z, Environment.NewLine);
                else sb.AppendFormat(nfi, "G81 X{0:F3} Y{1:F3} {2}{3}", Math.Round(points[i].X, 3), Math.Round(points[i].Y, 3), str_Z, Environment.NewLine);
            }
            sb.AppendLine("G00 Z40");
            sb.AppendLine("M05(Stop Spindle)");
            sb.AppendLine("M02(end of prog)");
            return sb;
        }
        public static void ExportSetupToNC(List<Point> points)
        {
            DateTime d = DateTime.Now;
            string fileName = string.Format("ExportSETUP_{0}{1}{2}T{3}{4}{5}.NC",
                d.Year, d.Month.ToString("D2"), d.Day.ToString("D2"), d.Hour.ToString("D2"), d.Minute.ToString("D2"), d.Second.ToString("D2"));            

            StringBuilder sb = GetCNCFileContentForSetup(points);
            File.WriteAllText(fileName, sb.ToString());
        }
        public static StringBuilder GetCNCFileContentForSetup(List<Point> points)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            string str_Z = "z35.0";
            StringBuilder sb = new StringBuilder();            
            sb.AppendLine("(SETUP PROGRAM)");
            sb.AppendLine("(Touch Z to top of touch plate)");
            sb.AppendLine("G21 G90 G64 G40 G17");
            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0) sb.AppendFormat(nfi, "G81 X{0:F3} Y{1:F3} {2} R40.0 F500.0{3}", Math.Round(points[i].X, 3), Math.Round(points[i].Y, 3), str_Z, Environment.NewLine);
                else sb.AppendFormat(nfi, "G81 X{0:F3} Y{1:F3} {2}{3}", Math.Round(points[i].X, 3), Math.Round(points[i].Y, 3), str_Z, Environment.NewLine);
            }
            sb.AppendLine("G00 Z40");
            sb.AppendLine("M02(end of prog)");

            return sb;
        }


    }
}
