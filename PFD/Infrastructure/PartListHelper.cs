using BaseClasses;
using BaseClasses.GraphObj;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PFD
{
    public static class PartListHelper
    {
        public static bool DisplayCladdingSheetsTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm._modelOptionsVM.IndividualCladdingSheets && vm.ModelHasPurlinsOrGirts();
        }
        public static bool DisplayFibreglassSheetsTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm._modelOptionsVM.IndividualCladdingSheets &&  vm.ModelHasPurlinsOrGirts() && vm._claddingOptionsVM.HasFibreglass();
        }

        public static bool DisplayCladdingTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && !vm._modelOptionsVM.IndividualCladdingSheets && vm.ModelHasPurlinsOrGirts();
        }
        public static bool DisplayFibreglassTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && !vm._modelOptionsVM.IndividualCladdingSheets && vm.ModelHasPurlinsOrGirts() && vm._claddingOptionsVM.HasFibreglass();
        }

        public static bool DisplayRoofNettingTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasRoof();
        }

        public static bool DisplayFlashingsTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts() && vm._doorsAndWindowsVM != null && vm._doorsAndWindowsVM.Flashings.Count > 0;
        }
        public static bool DisplayGuttersTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasRoof() && vm._doorsAndWindowsVM != null && vm._doorsAndWindowsVM.Gutters.Count > 0;
        }
        public static bool DisplayDownpipesTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasRoof() && vm._doorsAndWindowsVM != null && vm._doorsAndWindowsVM.Downpipes.Count > 0;
        }
        public static bool DisplayPackersTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts() && vm._doorsAndWindowsVM != null && vm._doorsAndWindowsVM.ModelHasRollerDoor() && vm._doorsAndWindowsVM.AreBothRollerDoorHeaderFlashings();
        }
    }
}