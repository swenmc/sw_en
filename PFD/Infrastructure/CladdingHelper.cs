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

namespace PFD.Infrastructure
{
    public static class CladdingHelper
    {
        public static CCladding GetCladding(int claddingIndex, CPFDViewModel vm, BuildingGeometryDataInput sGeometryInputData, CCrSc_TW columnSection)
        {
            CCladding cladding = new CCladding(claddingIndex, (EModelType_FS)vm.KitsetTypeIndex, 
                sGeometryInputData,
                vm._modelOptionsVM.IndividualCladdingSheets,
                vm._componentVM.ComponentList,
                vm._canopiesOptionsVM.CanopiesList,
                vm._baysWidthOptionsVM.BayWidthList,
                vm._claddingOptionsVM.FibreglassProperties,
                vm._doorsAndWindowsVM == null ? null : vm._doorsAndWindowsVM.DoorBlocksProperties,
                vm._doorsAndWindowsVM == null ? null : vm._doorsAndWindowsVM.WindowBlocksProperties,
                columnSection, //(CRSC.CCrSc_TW)vm.Model.m_arrCrSc[EMemberType_FS_Position.EdgeColumn] //takto som to chcel mat, ale Model je null
                vm.ColumnDistance, vm.ColumnDistance,
                new MATERIAL.CMat_03_00(vm._claddingOptionsVM.WallCladdingProps.material_Name, 200e+9f, 80e+9f, 0.3f, 7850f),
                new MATERIAL.CMat_03_00(vm._claddingOptionsVM.RoofCladdingProps.material_Name, 200e+9f, 80e+9f, 0.3f, 7850f),
                new MATERIAL.CMat_10_00("default"), // Default - TODO dopracovat databazu materialov fibreglass, pripade do GUI moznost volby materialu
                new MATERIAL.CMat_10_00("default"), // Default - TODO dopracovat databazu materialov fibreglass, pripade do GUI moznost volby materialu
                vm._claddingOptionsVM.WallCladdingColors.ElementAtOrDefault(vm._claddingOptionsVM.WallCladdingColorIndex).Name,
                vm._claddingOptionsVM.RoofCladdingColors.ElementAtOrDefault(vm._claddingOptionsVM.RoofCladdingColorIndex).Name,
                vm._claddingOptionsVM.WallCladding, vm._claddingOptionsVM.WallCladdingCoating,
                vm._claddingOptionsVM.RoofCladding, vm._claddingOptionsVM.RoofCladdingCoating,
                (Color)ColorConverter.ConvertFromString(vm._claddingOptionsVM.WallCladdingColors.ElementAtOrDefault(vm._claddingOptionsVM.WallCladdingColorIndex).CodeHEX),
                (Color)ColorConverter.ConvertFromString(vm._claddingOptionsVM.RoofCladdingColors.ElementAtOrDefault(vm._claddingOptionsVM.RoofCladdingColorIndex).CodeHEX),
                (Color)ColorConverter.ConvertFromString(vm._claddingOptionsVM.FibreglassColors.ElementAtOrDefault(vm._claddingOptionsVM.ColorWall_FG_Index).CodeHEX),
                (Color)ColorConverter.ConvertFromString(vm._claddingOptionsVM.FibreglassColors.ElementAtOrDefault(vm._claddingOptionsVM.ColorRoof_FG_Index).CodeHEX),
                vm._claddingOptionsVM.FibreglassColors.ElementAtOrDefault(vm._claddingOptionsVM.ColorWall_FG_Index).Name,
                vm._claddingOptionsVM.FibreglassColors.ElementAtOrDefault(vm._claddingOptionsVM.ColorRoof_FG_Index).Name,
                true, 0,
                vm._claddingOptionsVM.WallCladdingProps.height_m,
                vm._claddingOptionsVM.RoofCladdingProps.height_m,
                vm._claddingOptionsVM.WallCladdingProps.widthRib_m,
                vm._claddingOptionsVM.RoofCladdingProps.widthRib_m,
                (float)vm._claddingOptionsVM.WallCladdingProps.widthModular_m,
                (float)vm._claddingOptionsVM.RoofCladdingProps.widthModular_m,
                vm._claddingOptionsVM.RoofEdgeOverHang_FB_Y,
                vm._claddingOptionsVM.RoofEdgeOverHang_LR_X,
                vm._claddingOptionsVM.CanopyRoofEdgeOverHang_LR_X,
                vm._claddingOptionsVM.WallBottomOffset_Z,
                vm._claddingOptionsVM.ConsiderRoofCladdingFor_FB_WallHeight);

            return cladding;
        }
    }
}
