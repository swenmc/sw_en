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
        public static CCladding GetCladding(int claddingIndex, CPFDViewModel vm, BuildingGeometryDataInput sGeometryInputData)
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
                BaseClasses.Helpers.CrScFactory.GetCrSc(vm.ComponentList[(int)EMemberType_FS_Position.EdgeColumn].Section), // Vyrobime cross-section podla nazvu prierezu v zozname (crsc by mohol do funckie vstupovat aj ako objekt z modelu)
                vm.ColumnDistance, vm.ColumnDistance,
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
                vm._claddingOptionsVM.WallFibreglassThicknessTypes.ElementAtOrDefault(vm._claddingOptionsVM.WallFibreglassThicknessIndex),
                vm._claddingOptionsVM.RoofFibreglassThicknessTypes.ElementAtOrDefault(vm._claddingOptionsVM.RoofFibreglassThicknessIndex),
                true, 0,
                vm._claddingOptionsVM.WallCladdingProps,
                vm._claddingOptionsVM.RoofCladdingProps,
                vm._claddingOptionsVM.WallCladdingCoilProps,
                vm._claddingOptionsVM.RoofCladdingCoilProps,
                vm._claddingOptionsVM.RoofEdgeOverHang_FB_Y,
                vm._claddingOptionsVM.RoofEdgeOverHang_LR_X,
                vm._claddingOptionsVM.CanopyRoofEdgeOverHang_LR_X,
                vm._claddingOptionsVM.WallBottomOffset_Z,
                vm._claddingOptionsVM.ConsiderRoofCladdingFor_FB_WallHeight,
                vm._claddingOptionsVM.MaxSheetLengthRoof,
                vm._claddingOptionsVM.MaxSheetLengthWall,
                vm._claddingOptionsVM.MaxSheetLengthRoofFibreglass,
                vm._claddingOptionsVM.MaxSheetLengthWallFibreglass,
                vm._claddingOptionsVM.RoofCladdingOverlap,
                vm._claddingOptionsVM.WallCladdingOverlap,
                vm._claddingOptionsVM.RoofFibreglassOverlap,
                vm._claddingOptionsVM.WallFibreglassOverlap
                );

            return cladding;
        }
    }
}