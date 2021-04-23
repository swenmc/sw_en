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
        public static bool DisplayCladdingTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm._modelOptionsVM.IndividualCladdingSheets && vm.ModelHasPurlinsOrGirts();
        }
        public static bool DisplayFibreglassTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm._modelOptionsVM.IndividualCladdingSheets &&  vm.ModelHasPurlinsOrGirts() && vm._claddingOptionsVM.HasFibreglass();
        }
    }
}