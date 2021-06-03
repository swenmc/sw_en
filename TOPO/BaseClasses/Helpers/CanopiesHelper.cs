using BaseClasses;
using BaseClasses.GraphObj;
using CRSC;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BaseClasses.Helpers
{
    public static class CanopiesHelper
    {
        public static bool IsNeighboringLeftCanopy(CCanopiesInfo canopiesInfo)
        {
            if (canopiesInfo == null) return false;

            if (canopiesInfo.Left == true) return true;
            else return false;
        }
        public static bool IsNeighboringRightCanopy(CCanopiesInfo canopiesInfo)
        {
            if (canopiesInfo == null) return false;

            if (canopiesInfo.Right == true) return true;
            else return false;
        }

        public static CCanopiesInfo GetPreviousNeighboringCanopyLeft(CCanopiesInfo canopy, ObservableCollection<CCanopiesInfo> CanopiesList)
        {
            bool hasPreviousCanopy = IsNeighboringLeftCanopy(CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1));
            if (hasPreviousCanopy) return CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1);
            else return null;
        }
        public static CCanopiesInfo GetPreviousNeighboringCanopyRight(CCanopiesInfo canopy, ObservableCollection<CCanopiesInfo> CanopiesList)
        {
            bool hasPreviousCanopy = IsNeighboringRightCanopy(CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1));
            if (hasPreviousCanopy) return CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1);
            else return null;
        }
        public static CCanopiesInfo GetNextNeighboringCanopyLeft(CCanopiesInfo canopy, ObservableCollection<CCanopiesInfo> CanopiesList)
        {
            bool hasNextCanopy = IsNeighboringLeftCanopy(CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1));
            if (hasNextCanopy) return CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1);
            else return null;
        }
        public static CCanopiesInfo GetNextNeighboringCanopyRight(CCanopiesInfo canopy, ObservableCollection<CCanopiesInfo> CanopiesList)
        {
            bool hasNextCanopy = IsNeighboringRightCanopy(CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1));
            if (hasNextCanopy) return CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1);
            else return null;
        }
    }
}