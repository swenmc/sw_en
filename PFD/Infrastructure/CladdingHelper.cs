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
                vm.DoorBlocksProperties,
                vm.WindowBlocksProperties,
                columnSection, //(CRSC.CCrSc_TW)vm.Model.m_arrCrSc[EMemberType_FS_Position.EdgeColumn] //takto som to chcel mat, ale Model je null
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

            SetCladdingSheetsMaterial(vm, cladding);

            return cladding;
        }

        //TODO - Mato review
        private static void SetCladdingSheetsMaterial(CPFDViewModel vm, CCladding cladding)
        {
            //-----------------------------------------------------------------------------
            CTS_CrscProperties prop_RoofCladding = vm._claddingOptionsVM.RoofCladdingProps;
            CTS_CrscProperties prop_WallCladding = vm._claddingOptionsVM.WallCladdingProps;
            CTS_CoilProperties prop_RoofCladdingCoil;
            CTS_CoilProperties prop_WallCladdingCoil;
            CoatingColour prop_RoofCladdingColor;
            CoatingColour prop_WallCladdingColor;
            vm._claddingOptionsVM.GetCTS_CoilProperties(out prop_RoofCladdingCoil, out prop_WallCladdingCoil, out prop_RoofCladdingColor, out prop_WallCladdingColor);

            string prop_RoofCladdingMaterialName = prop_RoofCladdingCoil.materialName;
            string prop_WallCladdingMaterialName = prop_WallCladdingCoil.materialName;

            //To Mato - podla mna take nieco by sme potrebovali
            //CMatPropertiesRC props = CMaterialManager.LoadMaterialPropertiesRC(prop_WallCladdingMaterialName);
            //float wall_rho = (float)props.Rho;
            //props = CMaterialManager.LoadMaterialPropertiesRC(prop_RoofCladdingMaterialName);
            //float roof_rho = (float)props.Rho;

            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetCladdingSheets_Wall())
            {
                sheet.m_Mat.Name = prop_WallCladdingMaterialName;
            }
            foreach (CCladdingOrFibreGlassSheet sheet in cladding.GetCladdingSheets_Roof())
            {
                sheet.m_Mat.Name = prop_WallCladdingMaterialName;
            }
        }




        //toto je povodny kod, ak by sme ho na nieco potrebovali
        //13.2.2021
        //tak tento kod sa mi nepozdava, treba to refaktorovat
        //if (_pfdVM._modelOptionsVM.EnableCladding && _pfdVM._claddingOptionsVM != null)
        //    m_arrGOCladding = new List<BaseClasses.GraphObj.CCladding>(1) { new BaseClasses.GraphObj.CCladding(0, eKitset,
        //    sGeometryInputData,
        //    _pfdVM._canopiesOptionsVM.CanopiesList,
        //    _pfdVM._baysWidthOptionsVM.BayWidthList,
        //    _pfdVM._claddingOptionsVM.FibreglassProperties,
        //    vm.DoorBlocksProperties,
        //    vm.WindowBlocksProperties,
        //    (CCrSc_TW)m_arrCrSc[EMemberType_FS_Position.EdgeColumn],
        //    fDist_FrontColumns, fDist_BackColumns,
        //    _pfdVM._claddingOptionsVM.WallCladdingColors.ElementAtOrDefault(_pfdVM._claddingOptionsVM.WallCladdingColorIndex).Name,
        //    _pfdVM._claddingOptionsVM.RoofCladdingColors.ElementAtOrDefault(_pfdVM._claddingOptionsVM.RoofCladdingColorIndex).Name,
        //    _pfdVM._claddingOptionsVM.WallCladding, _pfdVM._claddingOptionsVM.WallCladdingCoating,
        //    _pfdVM._claddingOptionsVM.RoofCladding, _pfdVM._claddingOptionsVM.RoofCladdingCoating,
        //    (Color)ColorConverter.ConvertFromString(_pfdVM._claddingOptionsVM.WallCladdingColors.ElementAtOrDefault(_pfdVM._claddingOptionsVM.WallCladdingColorIndex).CodeHEX),
        //    (Color)ColorConverter.ConvertFromString(_pfdVM._claddingOptionsVM.RoofCladdingColors.ElementAtOrDefault(_pfdVM._claddingOptionsVM.RoofCladdingColorIndex).CodeHEX), true, 0,
        //    _pfdVM._claddingOptionsVM.WallCladdingProps.height_m,
        //    _pfdVM._claddingOptionsVM.RoofCladdingProps.height_m,
        //    _pfdVM._claddingOptionsVM.WallCladdingProps.widthRib_m,
        //    _pfdVM._claddingOptionsVM.RoofCladdingProps.widthRib_m,
        //    (float)_pfdVM._claddingOptionsVM.WallCladdingProps.widthModular_m,
        //    (float)_pfdVM._claddingOptionsVM.RoofCladdingProps.widthModular_m,
        //    _pfdVM._claddingOptionsVM.RoofEdgeOverHang_FB_Y,
        //    _pfdVM._claddingOptionsVM.RoofEdgeOverHang_LR_X,
        //    _pfdVM._claddingOptionsVM.CanopyRoofEdgeOverHang_LR_X,
        //    _pfdVM._claddingOptionsVM.WallBottomOffset_Z,
        //    _pfdVM._claddingOptionsVM.ConsiderRoofCladdingFor_FB_WallHeight) };

    }
}
