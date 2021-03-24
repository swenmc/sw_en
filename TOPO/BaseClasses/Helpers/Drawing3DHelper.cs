using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BaseClasses.Helpers
{
    public static class Drawing3DHelper
    {
        //public static void TransformPoints(List<Point3D> points, Transform3DGroup transform)
        //{            
        //    for (int i = 0; i < points.Count; i++)
        //    {
        //        points[i] = transform.Transform(points[i]);
        //    }
        //}
        public static void TransformPoints(List<Point3D> points, Transform3D transform)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = transform.Transform(points[i]);
            }
        }


        public static string GetCladdingSheetDisplayText(DisplayOptions options, CCladdingOrFibreGlassSheet sheet, out int rowsCount, out int maxRowLength)
        {
            string separator = " \n";
            rowsCount = 0;
            maxRowLength = 0;
            List<string> parts = new List<string>();

            bool bDisplayID = sheet.IsFibreglass ? options.bDisplayFibreglassID : options.bDisplayCladdingID;
            bool bDisplayPrefix = sheet.IsFibreglass ? options.bDisplayFibreglassPrefix : options.bDisplayCladdingPrefix;
            bool bDisplayLengthWidth = sheet.IsFibreglass ? options.bDisplayFibreglassLengthWidth : options.bDisplayCladdingLengthWidth;
            bool bDisplayArea = sheet.IsFibreglass ? options.bDisplayFibreglassArea : options.bDisplayCladdingArea;
            bool bDisplayUnits = sheet.IsFibreglass ? options.bDisplayFibreglassUnits : options.bDisplayCladdingUnits;

            if (bDisplayID) { parts.Add(sheet.ID.ToString()); rowsCount++; }
            if (bDisplayPrefix) { parts.Add(sheet.Prefix.ToString()); rowsCount++; }
            if (bDisplayLengthWidth)
            {
                string units = bDisplayUnits ? " m" : "";
                parts.Add($"{sheet.LengthTotal.ToString("F3")}x{sheet.Width.ToString("F3")}{units}");
                rowsCount++;
            }
            if (bDisplayArea)
            {
                string units = bDisplayUnits ? " m²" : "";
                parts.Add(sheet.Area_netto.ToString("F3") + units);
                rowsCount++;
            }

            foreach (string s in parts)
            {
                if (s.Length > maxRowLength) maxRowLength = s.Length;
            }

            return string.Join(separator, parts);
        }

        public static string GetDoorDisplayText(DisplayOptions options, CStructure_Door door, out int rowsCount, out int maxRowLength)
        {
            string separator = " \n";
            rowsCount = 0;
            maxRowLength = 0;
            List<string> parts = new List<string>();

            if (options.bDisplayDoorID) { parts.Add(door.ID.ToString()); rowsCount++; }
            if (options.bDisplayDoorType) { parts.Add(door.IsRollerDoor ? "Roller Door" : "Personnel Door"); rowsCount++; }
            if (options.bDisplayDoorHeightWidth)
            {
                string units = options.bDisplayDoorUnits ? " m" : "";
                parts.Add($"{door.LengthTotal.ToString("F3")}x{door.Width.ToString("F3")}{units}");
                rowsCount++;
            }
            if (options.bDisplayDoorArea)
            {
                string units = options.bDisplayDoorUnits ? " m²" : "";
                parts.Add(door.Area_netto.ToString("F3") + units);
                rowsCount++;
            }

            foreach (string s in parts)
            {
                if (s.Length > maxRowLength) maxRowLength = s.Length;
            }

            return string.Join(separator, parts);
        }

        public static string GetWindowDisplayText(DisplayOptions options, CStructure_Window window, out int rowsCount, out int maxRowLength)
        {
            string separator = " \n";
            rowsCount = 0;
            maxRowLength = 0;
            List<string> parts = new List<string>();

            if (options.bDisplayWindowID) { parts.Add(window.ID.ToString()); rowsCount++; }
            if (options.bDisplayWindowHeightWidth)
            {
                string units = options.bDisplayWindowUnits ? " m" : "";
                parts.Add($"{window.LengthTotal.ToString("F3")}x{window.Width.ToString("F3")}{units}");
                rowsCount++;
            }
            if (options.bDisplayWindowArea)
            {
                string units = options.bDisplayWindowUnits ? " m²" : "";
                parts.Add(window.Area_netto.ToString("F3") + units);
                rowsCount++;
            }

            foreach (string s in parts)
            {
                if (s.Length > maxRowLength) maxRowLength = s.Length;
            }

            return string.Join(separator, parts);
        }


    }
}
