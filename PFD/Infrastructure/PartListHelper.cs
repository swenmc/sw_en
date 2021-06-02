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
            return vm._modelOptionsVM.EnableCladding && vm._modelOptionsVM.IndividualCladdingSheets && vm.ModelHasPurlinsOrGirts() && vm._claddingOptionsVM.HasFibreglass();
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

        public static bool DisplayCladdingAccesoriesTable(CPFDViewModel vm)
        {
            return vm._modelOptionsVM.EnableCladding && vm.ModelHasPurlinsOrGirts() && vm._doorsAndWindowsVM != null;
        }




        public static void GetTableCladdingAccessoriesLists(CPFDViewModel vm, out List<CCladdingAccessories_Item_Piece> claddingAccessoriesItems_Piece, out List<CCladdingAccessories_Item_Length> claddingAccessoriesItems_Length)
        {
            // Cladding Accessories Item and Fixing - IN WORK
            claddingAccessoriesItems_Piece = new List<CCladdingAccessories_Item_Piece>();
            claddingAccessoriesItems_Length = new List<CCladdingAccessories_Item_Length>();

            CCladdingAccessories_Item_Piece itemPiece;
            CCladdingAccessories_Item_Length itemLength;

            if (vm.Model.m_arrGOCladding == null) return;
            if (vm.Model.m_arrGOCladding.Count == 0) return;
            if (!vm._modelOptionsVM.EnableCladding) return;

            CCladding cladding = vm.Model.m_arrGOCladding[0];

            int iNumberOfFixingPoints = 0;
            double fixingPointTributaryArea = 0;

            int iNumberLapstitchFixingPoints = 0;
            double dLapstitchFixingPointsSpacing = 0;

            if (vm.Model.m_arrGOCladding[0].HasCladdingSheets_Roof())
            {
                // 11 - Standard Roofing
                // Sposob A
                double ribWidthRoof = vm._claddingOptionsVM.RoofCladdingProps.widthRib_m;
                fixingPointTributaryArea = ribWidthRoof * vm.Model.fDist_Purlin;

                double fRoofCladdingAreaFibreglass = vm._claddingOptionsVM.FibreglassAreaRoofRatio / 100 * vm.TotalRoofArea; // Todo skontrolovat
                double fRoofCladdingArea_WithoutFibreglass = vm.TotalRoofAreaInclCanopies - fRoofCladdingAreaFibreglass; // Todo skontrolovat

                iNumberOfFixingPoints = (int)(fRoofCladdingArea_WithoutFibreglass / fixingPointTributaryArea);

                // Pridavok
                iNumberOfFixingPoints = (int)(iNumberOfFixingPoints * 1.1478f); // Navysime pocet o rezervu

                if (vm._modelOptionsVM.IndividualCladdingSheets)
                {
                    int iNumberOfFixingPoints2 = 0;
                    // Sposob B

                    if (vm.Model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight != null)
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfCladdingSheetsRoofRight)
                        {
                            iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                        }
                    }

                    if (vm.Model.m_arrGOCladding[0].listOfCladdingSheetsRoofLeft != null)
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfCladdingSheetsRoofLeft)
                        {
                            iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                        }
                    }

                    // Kontrola - priblizna
                    if (!MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 15))
                    {
                        // Exception
                        throw new Exception("Algorithm error. Different count of items!");
                    }

                    iNumberOfFixingPoints = iNumberOfFixingPoints2; // Ak su zapnute individual sheet, pouzijeme vysledky z individual sheets
                }

                itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx115 (plastic profile washer and galvanized cap)", iNumberOfFixingPoints, "Roof Cladding");
                claddingAccessoriesItems_Piece.Add(itemPiece);

                if (vm.Model.m_arrGOCladding[0].HasFibreglassSheets_Roof())
                {
                    // 12 - Fibreglass rooflites

                    // Todo napojit
                    // Zistit kolko FG sheets (resp ich sirku) konci na hrane alebo tesne pod hranou strechy pre gable roof (napriklad < 0.3 m
                    // Podla suradnice y a hodnoty length v porovnani s length_left_basic (right side) a hodnoty y = 0 (left side)

                    int iNumberOfFGSheetsRidge = 5; // Todo napojit
                    double dTotalLengthFGSheetsRidge = 20.15; // Todo napojit

                    int iNumberOfSupportBracketBetweenPurlins;
                    double supportBracketBetweenPurlinsLengthTotal = 0;
                    int iNumberOfSupportBracketBetweenPurlinsFixingPoints = 0;

                    if (vm.Model.fDist_Purlin <= 2.5)
                        iNumberOfSupportBracketBetweenPurlins = 0;
                    if (vm.Model.fDist_Purlin <= 4.5)
                        iNumberOfSupportBracketBetweenPurlins = 1;
                    else
                        iNumberOfSupportBracketBetweenPurlins = 2;

                    double dLapFoamPacker_TotalLength = 0;

                    // Sposob A
                    // Tento sposob je mozno zbytocny, lebo fibreglass su vzdy zadane ako individual sheets
                    fixingPointTributaryArea = ribWidthRoof * vm.Model.fDist_Purlin;

                    iNumberOfFixingPoints = (int)(fRoofCladdingAreaFibreglass / fixingPointTributaryArea);

                    // Sposob B

                    int iNumberOfFixingPoints2 = 0;
                    iNumberLapstitchFixingPoints = 0; // Pozdlzne na okraji sheet, TODO doriesit ak su 2 fibreglass sheets vedla seba
                    dLapstitchFixingPointsSpacing = 0.6; // TODO napojit na DB - hodnota je v DB

                    if (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofRight != null)
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofRight)
                        {
                            iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                            iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                            int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenPurlins * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Purlin) + 1);
                            supportBracketBetweenPurlinsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                            iNumberOfSupportBracketBetweenPurlinsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 + 2); // Pridany jeden bod pre koncove rebro FG + 2 pre rebra cladding sheet
                            dLapFoamPacker_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                        }
                    }

                    if (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofLeft != null)
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsRoofLeft)
                        {
                            iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Purlin) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                            iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                            int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenPurlins * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Purlin) + 1);
                            supportBracketBetweenPurlinsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                            iNumberOfSupportBracketBetweenPurlinsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 + 2); // Pridany jeden bod pre koncove rebro FG + 2 pre rebra cladding sheet
                            dLapFoamPacker_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                        }
                    }

                    // Kontrola - priblizna
                    if (!MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 15))
                    {
                        // Exception
                        throw new Exception("Algorithm error. Different count of items!");
                    }

                    iNumberOfFixingPoints = iNumberOfFixingPoints2; // Pouzijeme vysledky zo sheets

                    // Crown roof fixing
                    itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx115 (plastic profile washer and galvanized cap)", iNumberOfFixingPoints, "Roof Fibreglass");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Lapstitch fixing
                    itemPiece = new CCladdingAccessories_Item_Piece("Lapstitch with TEK screw 12gx20 (neo washer)", iNumberLapstitchFixingPoints, "Roof Fibreglass");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Protection strip
                    double fLengthProtectionstrip = iNumberOfFixingPoints2 * ribWidthRoof;

                    // CAccessories_LengthItemProperties - asi by bolo dobre pouzit
                    itemLength = new CCladdingAccessories_Item_Length("Fibreglass protection strip 80 mm wide", fLengthProtectionstrip);
                    claddingAccessoriesItems_Length.Add(itemLength);

                    if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed && vm._doorsAndWindowsVM.HasFlashing(EFlashingType.FibreglassRoofRidgeCap)) // Gable Roof Only
                    {
                        // Plastic blocks - Ridge - Fibreglass edge cap
                        int iNumberOfRidgePlasticBlocks = (int)(dTotalLengthFGSheetsRidge / ribWidthRoof);
                        itemPiece = new CCladdingAccessories_Item_Piece("Plastic ridge block - fibreglass", iNumberOfRidgePlasticBlocks);
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }

                    // 13 - Rooflite support bracket

                    // Support bracket
                    itemPiece = new CCladdingAccessories_Item_Piece("Fibreglass support bracket 30x40x1400-1 mm", (int)(supportBracketBetweenPurlinsLengthTotal / 1.4f) + 1, "Roof Fibreglass Support Bracket");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Support bracket fixing
                    itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx115 (plastic profile washer and galvanized cap)", iNumberOfSupportBracketBetweenPurlinsFixingPoints, "Roof Fibreglass Support Bracket");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // 14 - Roofing lap

                    // Continuous closed cell foam packer
                    itemLength = new CCladdingAccessories_Item_Length("Continuous closed cell foam packer 10x12 mm", dLapFoamPacker_TotalLength/*, "Roof Fibreglass"*/);
                    claddingAccessoriesItems_Length.Add(itemLength);

                    if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed) // Gable Roof Only
                    {
                        // Roof ridge length
                        // Rovnake ako ridge flashing length

                        iNumberOfFixingPoints = 2 * ((int)(vm.RoofLength_Y / ribWidthRoof) + 1);

                        bool bStandardRidge = true; // TODO - napojit - accessories flashings

                        if (bStandardRidge)
                        {
                            // 15 - Standard ridge

                            // Apex ridge flashing rivets
                            itemPiece = new CCladdingAccessories_Item_Piece("Apex ridge flashing rivet 73AS6.4", iNumberOfFixingPoints);
                            claddingAccessoriesItems_Piece.Add(itemPiece);
                        }
                        else
                        {
                            // 16 - Infill Ridge

                            // TEK screws 14gx115
                            itemPiece = new CCladdingAccessories_Item_Piece("Apex ridge flashing TEK screw 14gx115  (neo washer)", iNumberOfFixingPoints);
                            claddingAccessoriesItems_Piece.Add(itemPiece);

                            // Plastic ridge blocks
                            itemPiece = new CCladdingAccessories_Item_Piece("Plastic ridge block", iNumberOfFixingPoints);
                            claddingAccessoriesItems_Piece.Add(itemPiece);

                            // TEK screws 12gx20
                            itemPiece = new CCladdingAccessories_Item_Piece("Ridge TEK screw 12gx20 (neo washer)", iNumberOfFixingPoints);
                            claddingAccessoriesItems_Piece.Add(itemPiece);
                        }

                        // TODO
                        // Apex brace - malo by to byt samostatne pri plates
                        double dApexBraceSpacing = 1; // 1 m???

                        int iNumberOfRidgeApexBracePoints = (int)(vm.Length / dApexBraceSpacing) + 1;
                        int iNumberOfRidgeApexBracePlates = 2 * iNumberOfRidgeApexBracePoints;
                        iNumberOfFixingPoints = 4 * iNumberOfRidgeApexBracePlates; // 4 TEK screws per brace

                        // Apex brace
                        itemPiece = new CCladdingAccessories_Item_Piece("Apex brace Angle L30/1-650", iNumberOfRidgeApexBracePoints);
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Apex brace TEK screws 10g
                        itemPiece = new CCladdingAccessories_Item_Piece("Apex brace wafer TEK screw 10g", iNumberOfFixingPoints);
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }
                }
            }

            // 17 - Barge

            if (cladding.HasCladdingSheets_Roof())
            {
                double dBargeFlashing_TotalLength = 0;
                double dBargeflashingFixingSpacing = 0.3f; // DB
                int iNumberOfFixingPointsBirdProofFlashing = 0;
                double dFixingPointsBargeCladdingSheetEdge = 2; // DB
                int iNumberOfFixingPointsBargeCladdingSheetEdge = 0;

                double dGutter_TotalLength = 0;
                double dGutterBracketSpacing = 2 * vm._claddingOptionsVM.RoofCladdingProps.widthRib_m; // DB
                int iNumberOfGutterBrackets = 0;
                int iNumberOfGutterBracketFixingPoints = 0;
                int iNumberOfGutterFixingPoints = 0;
                double dEavePurlinBirdProofFixingPointSpacing = 1; // DB
                int iNumberEavePurlinBirdProofFixingPoints = 0;

                // TODO  // pridat CANOPIES ???? !!!!!!!!!!!!!!
                // Asi bude potrebne prechadzat zoznam canopies ...

                int iRoofSidesCount = 0;

                if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
                {
                    if (cladding.HasCladdingSheets_WallFront() && cladding.HasCladdingSheets_WallBack()) iRoofSidesCount = 2;
                    else if (cladding.HasCladdingSheets_WallFront() || cladding.HasCladdingSheets_WallBack()) iRoofSidesCount = 1;
                    else iRoofSidesCount = 0;

                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.Barge))
                    {
                        dBargeFlashing_TotalLength = iRoofSidesCount * vm.RoofSideLength;
                        iNumberOfFixingPoints = 2 * (iRoofSidesCount * ((int)(vm.RoofSideLength / dBargeflashingFixingSpacing) + 1)); // Top and bottom
                        iNumberOfFixingPointsBargeCladdingSheetEdge = Math.Min(2, iRoofSidesCount * ((int)(vm.RoofSideLength / dFixingPointsBargeCladdingSheetEdge) + 1));

                        // TO Ondrej - podmienka bool bFrontOrBackWallCladdingExists a Barge BirdProof Flashing Exists
                        if (true)
                            iNumberOfFixingPointsBirdProofFlashing = iRoofSidesCount * ((int)(vm.RoofSideLength / vm._claddingOptionsVM.WallCladdingProps.widthRib_m) + 1);
                    }

                    if (vm._doorsAndWindowsVM.HasGutter())
                    {
                        dGutter_TotalLength = vm.RoofSideLength;
                        iNumberOfGutterBrackets = (int)(vm.RoofSideLength / dGutterBracketSpacing) + 1;
                        iNumberOfGutterBracketFixingPoints = 2 * iNumberOfGutterBrackets;

                        iNumberOfGutterFixingPoints = (int)(vm.RoofSideLength / vm._claddingOptionsVM.RoofCladdingProps.widthRib_m) + 1; // Each pan
                        iNumberOfGutterFixingPoints += iNumberOfGutterBrackets;
                    }

                    // TO Ondrej - podmienka bool Eave purlin bird proof flashing Exists (tab Accessories)
                    if (true)
                        iNumberEavePurlinBirdProofFixingPoints = (int)(vm.RoofSideLength / dEavePurlinBirdProofFixingPointSpacing) + 1;
                }
                else if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
                {
                    if (cladding.HasCladdingSheets_WallFront() && cladding.HasCladdingSheets_WallBack()) iRoofSidesCount = 4;
                    else if (cladding.HasCladdingSheets_WallFront() || cladding.HasCladdingSheets_WallBack()) iRoofSidesCount = 2;
                    else iRoofSidesCount = 0;

                    // TO Ondrej - podmienka bool bBargeFlashingExists (tab Accessories)
                    if (true)
                    {
                        dBargeFlashing_TotalLength = iRoofSidesCount * vm.RoofSideLength;
                        iNumberOfFixingPoints = 2 * (iRoofSidesCount * ((int)(vm.RoofSideLength / dBargeflashingFixingSpacing) + 1)); // Top and bottom
                        iNumberOfFixingPointsBargeCladdingSheetEdge = Math.Min(2, iRoofSidesCount * ((int)(vm.RoofSideLength / dFixingPointsBargeCladdingSheetEdge) + 1));

                        // TO Ondrej - podmienka bool bFrontOrBackWallCladdingExists a Barge BirdProof Flashing Exists
                        if (true)
                            iNumberOfFixingPointsBirdProofFlashing = iRoofSidesCount * ((int)(vm.RoofSideLength / vm._claddingOptionsVM.WallCladdingProps.widthRib_m) + 1);
                    }

                    // TO Ondrej - podmienka bool bGutterExists (tab Accessories)
                    if (true)
                    {
                        dGutter_TotalLength = 2 * vm.RoofSideLength;
                        iNumberOfGutterBrackets = 2 * ((int)(vm.RoofSideLength / dGutterBracketSpacing) + 1);
                        iNumberOfGutterBracketFixingPoints = 2 * iNumberOfGutterBrackets;

                        iNumberOfGutterFixingPoints = 2 * ((int)(vm.RoofSideLength / vm._claddingOptionsVM.RoofCladdingProps.widthRib_m) + 1); // Each pan
                        iNumberOfGutterFixingPoints += iNumberOfGutterBrackets;
                    }

                    // TO Ondrej - podmienka bool Eave purlin bird proof flashing Exists (tab Accessories)
                    if (true)
                        iNumberEavePurlinBirdProofFixingPoints = 2 * ((int)(vm.RoofSideLength / dEavePurlinBirdProofFixingPointSpacing) + 1);
                }



                // TODO - dopracovat podmienky
                // Pouzit ak su front a back wall

                // TO Ondrej - podmienka bool RoofCladding Exists a (bool bFrontWallCladding Exists alebo bBackWallCladding Exists)
                if (true)
                {
                    // Barge flashing fixing - Rivets
                    itemPiece = new CCladdingAccessories_Item_Piece("Barge flashing rivet 73AS6.4", iNumberOfFixingPoints);
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Barge cladding sheet edge fixing - TEK screws 12gx42
                    itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx42 (bonded washer)", iNumberOfFixingPointsBirdProofFlashing, "Barge");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // TO Ondrej - podmienka bool bFrontOrBackWallCladdingExists a Barge BirdProof Flashing Exists
                    if (true)
                    {
                        // TODO - dopracovat podmienky
                        // Pouzit ak su front a back wall
                        // Bird proof flashing fixing - Rivets
                        itemPiece = new CCladdingAccessories_Item_Piece("Birdgproof flashing rivet 73AS6.4", iNumberOfFixingPointsBirdProofFlashing, "Barge");
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }
                }

                // TO Ondrej - podmienka bool RoofCladding Exists
                if (true)
                {
                    // 18 - Gutter

                    // TO Ondrej - podmienka bool Eave purlin bird proof flashing Exists (tab Accessories)
                    if (true)
                    {
                        // Eave purlin bird proof flashing fixing
                        itemPiece = new CCladdingAccessories_Item_Piece("Birdproof strip wafer TEK screw 10g", iNumberEavePurlinBirdProofFixingPoints, "Eave purlin");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Eave purlin bird proof plastic blocks
                        itemPiece = new CCladdingAccessories_Item_Piece("Plastic gutter block", iNumberEavePurlinBirdProofFixingPoints, "Eave purlin");
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }

                    // TO Ondrej - podmienka bool RoofCladding Exists a bGuttersExist (tab Accessories)
                    if (true)
                    {
                        // 18 - Gutter

                        // Gutter brackets
                        itemPiece = new CCladdingAccessories_Item_Piece("Gutter bracket 300x26x15 mm", iNumberOfGutterBrackets);
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Gutter bracket fixing
                        itemPiece = new CCladdingAccessories_Item_Piece("Gutter TEK screw 12gx20 (neo washer)", iNumberOfGutterBracketFixingPoints);
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Gutter fixing
                        itemPiece = new CCladdingAccessories_Item_Piece("Gutter rivet 73AS6.4", iNumberOfGutterFixingPoints);
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }
                }

            }



            if (cladding.HasCladdingSheets_Wall())
            {
                // Openings
                // Urcime zakladne parametre, mohli by sem uz prist pripravene

                double dBuildingPerimeter = 2 * (vm.LengthOverall + vm.WidthOverall);
                double dBuildingCladdingPerimeterWithoutDoors = dBuildingPerimeter; // Obvod budovy bez sirky dveri

                // Wall Fibreglass Area
                double fWallCladdingAreaFibreglass = vm._claddingOptionsVM.FibreglassAreaWallRatio / 100 * vm.TotalWallArea; // Todo skontrolovat

                double dRollerDoorTrimmerLengh = 0;
                double dRollerDoorHeaderLengh = 0;
                int iNumberOfRollerDoorTrimmers = 0; // Trimmers or extension plates

                double dPADoorHeaderLengh = 0;

                bool bAnyRollerDoorExists = vm._doorsAndWindowsVM.ModelHasRollerDoor();
                bool bAnyPADoorExists = vm._doorsAndWindowsVM.ModelHasPersonelDoor();

                // Wall Doors and Windows Area
                double dDoorsAndWindowsOpeningArea = 0; // !!!! Tu su dvere a okna zo vsetkych stien, je potrebne doriesit ak sa niektora stena nerata, aby sa neuvazovali doors a windows z danej steny

                if (vm._doorsAndWindowsVM.ModelHasDoor())
                {
                    //To Mato - dalo by sa to takto pouzit 
                    //dRollerDoorTrimmerLengh = vm._doorsAndWindowsVM.GetRollerDoorTrimmerLengh();
                    //dRollerDoorHeaderLengh = vm._doorsAndWindowsVM.GetRollerDoorHeaderLengh();

                    foreach (DoorProperties door in vm._doorsAndWindowsVM.DoorBlocksProperties)
                    {
                        dBuildingCladdingPerimeterWithoutDoors -= door.fDoorsWidth;

                        if (door.sDoorType == "Roller Door")
                        {
                            dRollerDoorTrimmerLengh += door.fDoorsHeight * 2;
                            dRollerDoorHeaderLengh += door.fDoorsWidth;
                            iNumberOfRollerDoorTrimmers += 2;
                        }
                        else
                        {
                            dPADoorHeaderLengh += door.fDoorsWidth;
                        }

                        dDoorsAndWindowsOpeningArea += door.fDoorsWidth * door.fDoorsHeight;
                    }
                }

                if (vm._doorsAndWindowsVM.ModelHasWindow())
                {
                    foreach (WindowProperties window in vm._doorsAndWindowsVM.WindowBlocksProperties)
                    {
                        dDoorsAndWindowsOpeningArea += window.fWindowsWidth * window.fWindowsHeight;
                    }
                }

                // TO Ondrej - podmienka bool Wall Cladding Exists
                if (true)
                {
                    // 21 - Cladding

                    int profileFactor = 1; // 1 - Smartdek, 2 - Purlindek and Speedclad

                    double dCladdingSeamFixingSpacing = 0.6; // DB
                    int iNumberCladdingSeamFixingPoints = 0;

                    // Sposob A
                    double ribWidthWall = vm._claddingOptionsVM.WallCladdingProps.widthRib_m;
                    fixingPointTributaryArea = (ribWidthWall / (float)profileFactor) * vm.Model.fDist_Girt;

                    double fWallCladdingArea_WithoutFibreglassAndOpenings = vm.TotalWallArea - fWallCladdingAreaFibreglass - dDoorsAndWindowsOpeningArea; // Todo skontrolovat ak nie su aktivne vsetky steny !!!
                    iNumberOfFixingPoints = (int)(fWallCladdingArea_WithoutFibreglassAndOpenings / fixingPointTributaryArea);

                    // Pridavok
                    iNumberOfFixingPoints = (int)(iNumberOfFixingPoints * 1.2645f); // Navysime pocet o rezervu

                    if (vm._modelOptionsVM.IndividualCladdingSheets)
                    {
                        int iNumberOfFixingPoints2 = 0;
                        // Sposob B
                        // TO Ondrej - mam podozrenie ze tieto zoznamy obsahuju sheet pred nadelenim !!!!!!!
                        // TODO Ondrej - potrebujeme zaistit aby to sem voslo az ked je vsetko nadelene !!!!!

                        // TO Ondrej - podmienka bool WallCladdingLeft Exists
                        if (vm.Model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall != null)
                        {
                            int iSeamFixingPointsPerSheetWidth = 1;

                            foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfCladdingSheetsLeftWall)
                            {
                                if (!vm._modelOptionsVM.IndividualCladdingSheets)
                                    iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet

                                iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(sheet.LengthTotal_Real / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                            }
                        }

                        // TO Ondrej - podmienka bool WallCladdingFront Exists
                        if (vm.Model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall != null)
                        {
                            int iSeamFixingPointsPerSheetWidth = 1;

                            foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfCladdingSheetsFrontWall)
                            {
                                double dSheetLength = sheet.LengthTotal_Real;
                                if (!vm._modelOptionsVM.IndividualCladdingSheets)
                                {
                                    if (sheet.NumberOfEdges == 5)
                                        dSheetLength = MathF.Average(sheet.LengthTopLeft_Real, sheet.LengthTopTip_Real);
                                    else
                                        dSheetLength = MathF.Average(sheet.LengthTopLeft_Real, sheet.LengthTopRight_Real);

                                    iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet
                                }

                                iNumberOfFixingPoints2 += profileFactor * ((int)(dSheetLength / vm.Model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(dSheetLength / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                            }
                        }

                        // TO Ondrej - podmienka bool WallCladdingRight Exists
                        if (vm.Model.m_arrGOCladding[0].listOfCladdingSheetsRightWall != null)
                        {
                            int iSeamFixingPointsPerSheetWidth = 1;

                            foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfCladdingSheetsRightWall)
                            {
                                if (!vm._modelOptionsVM.IndividualCladdingSheets)
                                    iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet

                                iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(sheet.LengthTotal_Real / dCladdingSeamFixingSpacing)/* + 1*/); // One sheet side only
                            }
                        }

                        // TO Ondrej - podmienka bool WallCladdingBack Exists
                        if (vm.Model.m_arrGOCladding[0].listOfCladdingSheetsBackWall != null)
                        {
                            int iSeamFixingPointsPerSheetWidth = 1;

                            foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfCladdingSheetsBackWall)
                            {
                                double dSheetLength = sheet.LengthTotal_Real;
                                if (!vm._modelOptionsVM.IndividualCladdingSheets)
                                {
                                    if (sheet.NumberOfEdges == 5)
                                        dSheetLength = MathF.Average(sheet.LengthTopLeft_Real, sheet.LengthTopTip_Real);
                                    else
                                        dSheetLength = MathF.Average(sheet.LengthTopLeft_Real, sheet.LengthTopRight_Real);

                                    iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet
                                }

                                iNumberOfFixingPoints2 += profileFactor * ((int)(dSheetLength / vm.Model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(dSheetLength / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                            }
                        }

                        // Kontrola - priblizna
                        if (!MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 15))
                        {
                            // Exception
                            throw new Exception("Algorithm error. Different count of items!");
                        }

                        iNumberOfFixingPoints = iNumberOfFixingPoints2; // Ak su zapnute individual sheet, pouzijeme vysledky z individual sheets
                    }

                    itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 12gx20 (neo washer)", iNumberOfFixingPoints, "Wall Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Fixing between wall cladding sheets
                    itemPiece = new CCladdingAccessories_Item_Piece("Seam fix cladding rivet 73AS6.4", iNumberCladdingSeamFixingPoints, "Wall Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Damp proof course
                    itemLength = new CCladdingAccessories_Item_Length("Damp proof course beneath angle", dBuildingCladdingPerimeterWithoutDoors/*, "Wall Cladding"*/);
                    claddingAccessoriesItems_Length.Add(itemLength);

                    // Angle
                    itemLength = new CCladdingAccessories_Item_Length("Angle 50x50x1 mm", dBuildingCladdingPerimeterWithoutDoors/*, "Wall Cladding"*/);
                    claddingAccessoriesItems_Length.Add(itemLength);

                    // Foam bird proof strip
                    itemLength = new CCladdingAccessories_Item_Length("Foam birdproof strip", dBuildingCladdingPerimeterWithoutDoors/*, "Wall Cladding"*/);
                    claddingAccessoriesItems_Length.Add(itemLength);

                    // Angle fixing
                    double dAngleFixingPointsSpacing = 0.6; // DB
                    iNumberOfFixingPoints = (int)(dBuildingCladdingPerimeterWithoutDoors / dAngleFixingPointsSpacing) + 4; // 4 strany - len priblizne
                    itemPiece = new CCladdingAccessories_Item_Piece("Angle suredrive concrete anchor 6.5x50", iNumberOfFixingPoints, "Wall Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // TO Ondrej - podmienka bool Wall Corner Flashing Exists
                    // TO Ondrej - nastavit spravne pocet rohov, default je 4 kedze predpokladame ze existuju vsetky steny,
                    // ak je niektora zo stien wall cladding deaktivovana tak je pocet rohov 2, ak su deaktivovane 2,
                    // tak jedna a ked je zapnuta len jedna alebo ziadna tak 0
                    // vtedy sme nemali tento riadok zobrazit vobec, vsetobecne by bolo dobre pridat podmienku ze ak je item count = 0 alebo item length = 0 m, tak sa do
                    // tabuliek part list nepridaju
                    if (true)
                    {
                        // 22 - Cladding corner
                        double dCornerFlashingLength = 4 * vm.WallHeightOverall; // TODO napojit, zohladnit ktore steny su zapnute a ktore vypnute
                        double dCornerFlashingFixingPointsSpacing = 0.3; // DB (kotvenie dvoch stran flashing)
                        iNumberOfFixingPoints = 2 * ((int)(dCornerFlashingLength / dCornerFlashingFixingPointsSpacing) + 4); // 4 rohy - len priblizne

                        itemPiece = new CCladdingAccessories_Item_Piece("Corner flashing rivet 73AS6.4", iNumberOfFixingPoints, "Wall Cladding");
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }

                    // TO Ondrej - podmienka bool Wall Fibreglass Sheet Exists
                    if (true)
                    {
                        if ((vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallLeft != null && vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallLeft.Count > 0) ||
                            (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallFront != null && vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallFront.Count > 0) ||
                            (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallRight != null && vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallRight.Count > 0) ||
                            (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallBack != null && vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallBack.Count > 0))
                        {
                            // 23 - Fibreglass walllite

                            int iNumberOfSupportBracketBetweenGirts;
                            double supportBracketBetweenGirtsLengthTotal = 0;
                            int iNumberOfSupportBracketBetweenGirtsFixingPoints = 0;
                            int iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints = 0; // 12gx20 - 4 pcs per bracket

                            if (vm.Model.fDist_Purlin <= 1.8)
                                iNumberOfSupportBracketBetweenGirts = 0;
                            if (vm.Model.fDist_Purlin <= 5.4)
                                iNumberOfSupportBracketBetweenGirts = 1;
                            else
                                iNumberOfSupportBracketBetweenGirts = 2;

                            double dLapSealantBead_TotalLength = 0;

                            // Sposob A
                            // Tento sposob je mozno zbytocny, lebo fibreglass su vzdy zadane ako individual sheets
                            ribWidthWall = vm._claddingOptionsVM.WallCladdingProps.widthRib_m;
                            fixingPointTributaryArea = (ribWidthWall / (float)profileFactor) * vm.Model.fDist_Girt;

                            iNumberOfFixingPoints = (int)(fWallCladdingAreaFibreglass / fixingPointTributaryArea);

                            if (vm._modelOptionsVM.IndividualCladdingSheets)
                            {
                                // Sposob B

                                int iNumberOfFixingPoints2 = 0;
                                iNumberLapstitchFixingPoints = 0; // Pozdlzne na okraji sheet, TODO doriesit ak su 2 fibreglass sheets vedla seba
                                dLapstitchFixingPointsSpacing = 0.6; // TODO napojit na DB - hodnota je v DB

                                if (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallLeft != null)
                                {
                                    foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallLeft)
                                    {
                                        iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                                        iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                                        int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1);
                                        iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                        supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                        iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                        dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                                    }
                                }

                                if (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallFront != null)
                                {
                                    foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallFront)
                                    {
                                        iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                                        iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                                        int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1);
                                        iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                        supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                        iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                        dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                                    }
                                }

                                if (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallRight != null)
                                {
                                    foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallRight)
                                    {
                                        iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                                        iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                                        int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1);
                                        iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                        supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                        iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                        dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                                    }
                                }

                                if (vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallBack != null)
                                {
                                    foreach (CCladdingOrFibreGlassSheet sheet in vm.Model.m_arrGOCladding[0].listOfFibreGlassSheetsWallBack)
                                    {
                                        iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1) * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1);
                                        iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal_Real / dLapstitchFixingPointsSpacing);
                                        int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal_Real / vm.Model.fDist_Girt) + 1);
                                        iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                        supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                        iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                        dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                                    }
                                }

                                // Kontrola - priblizna
                                if (!MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 15))
                                {
                                    // Exception
                                    throw new Exception("Algorithm error. Different count of items!");
                                }

                                iNumberOfFixingPoints = iNumberOfFixingPoints2; // Pouzijeme vysledky zo sheets

                                // Lapstitch fixing
                                itemPiece = new CCladdingAccessories_Item_Piece("Lap stitching TEK screw 12gx20 (neo washer)", iNumberLapstitchFixingPoints, "Wall Fibreglass");
                                claddingAccessoriesItems_Piece.Add(itemPiece);

                                // Support bracket
                                itemPiece = new CCladdingAccessories_Item_Piece("U bracket 40x30x1400 - 1 mm", (int)(supportBracketBetweenGirtsLengthTotal / 1.4) + 1, "Wall Fibreglass Support Bracket");
                                claddingAccessoriesItems_Piece.Add(itemPiece);

                                // Support bracket fixing
                                itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 12gx20 (neo and bonded washer)", iNumberOfSupportBracketBetweenGirtsFixingPoints, "Wall Fibreglass Support Bracket");
                                claddingAccessoriesItems_Piece.Add(itemPiece);

                                // Support bracket fixing to cladding
                                itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 12gx20 (neo washer)", iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints, "Wall Fibreglass Support Bracket");
                                claddingAccessoriesItems_Piece.Add(itemPiece);

                                // 24 - Cladding lap

                                // Silicone sealant bead
                                itemLength = new CCladdingAccessories_Item_Length("Silicone sealant bead", dLapSealantBead_TotalLength);
                                claddingAccessoriesItems_Length.Add(itemLength);
                            }

                            // Pan fibreglass sheet fixing
                            itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 12gx20 (neo and bonded washer)", iNumberOfFixingPoints, "Wall Fibreglass");
                            claddingAccessoriesItems_Piece.Add(itemPiece);
                        }
                    }

                    if (bAnyRollerDoorExists)
                    {
                        // 26 - Roller door trim

                        // Roller door trim flashing fixing

                        double dRollerDoorflashingFixingSpacing = 0.3f; // DB

                        // TO Ondrej - podmienka bool RollerDoorTrimmerFlashing Exists
                        if (true)
                            iNumberOfFixingPoints = 2 * (int)(dRollerDoorTrimmerLengh / dRollerDoorflashingFixingSpacing); // 2 sides resp. top and bottom

                        // TO Ondrej - podmienka bool RollerDoorHeader(Lintel)Flashing Exists
                        if (true)
                            iNumberOfFixingPoints += 5 * (int)(dRollerDoorHeaderLengh / dRollerDoorflashingFixingSpacing);

                        itemPiece = new CCladdingAccessories_Item_Piece("Flashing rivet 73AS6.4", iNumberOfFixingPoints, "Roller Door");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // 27 - Roller door mounting
                        // Roller door extension plate
                        itemPiece = new CCladdingAccessories_Item_Piece("Roller door extension plate", iNumberOfRollerDoorTrimmers, "Roller Door");
                        claddingAccessoriesItems_Piece.Add(itemPiece);

                        // Roller door extension plate fixing
                        int iNumberOfFixingPointsPerPlate = 2 * 6; // DB
                        iNumberOfFixingPoints = iNumberOfRollerDoorTrimmers * iNumberOfFixingPointsPerPlate;
                        itemPiece = new CCladdingAccessories_Item_Piece("Roller door extension plate TEK screw 14gx22", iNumberOfFixingPoints, "Roller Door");
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }

                    if (bAnyPADoorExists)
                    {
                        // TO Ondrej - podmienka bool PAHeaderCapFlashing Exists
                        if (true)
                        {
                            // 29 - PA door trim
                            // PA door header cap flashing fixing

                            double dPADoorflashingFixingSpacing = 0.3f; // DB
                            iNumberOfFixingPoints = 2 * (int)(dPADoorHeaderLengh / dPADoorflashingFixingSpacing);
                            itemPiece = new CCladdingAccessories_Item_Piece("Flashing rivet 73AS6.4", iNumberOfFixingPoints, "Personnel Door");
                            claddingAccessoriesItems_Piece.Add(itemPiece);
                        }
                    }
                }
            }

        }


        public static DataSet GetTableCladdingAccessories_Items_Length(List<CCladdingAccessories_Item_Length> claddingAccessoriesItems_Length, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            if (claddingAccessoriesItems_Length == null) return null;
            if (claddingAccessoriesItems_Length.Count == 0) return null;
            // IN WORK - rozpracovane

            // TODO - Dopracovat
            double dTotalItemsMass_Table = 0;
            double dTotalItemsPrice_Table = 0;

            List<QuotationItem> quotation = new List<QuotationItem>(); // TODO Docanse - upravit 
            foreach (CCladdingAccessories_Item_Length item in claddingAccessoriesItems_Length)
            {
                QuotationItem qitem = new QuotationItem();

                qitem.Name = item.Name;
                qitem.Length = (float)item.m_length;

                quotation.Add(qitem);

                //dTotalItemsMass_Table += item.Mass;
                //dTotalItemsPrice_Table += item.Price_NZD;
            }

            dBuildingMass += dTotalItemsMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalItemsPrice_Table;

            // Create Table
            DataTable table = new DataTable("CladdingAccessories_Items_Length");
            // Create Table Rows
            table.Columns.Add(QuotationHelper.colProp_Name.ColumnName, QuotationHelper.colProp_Name.DataType);
            table.Columns.Add(QuotationHelper.colProp_Length_m.ColumnName, QuotationHelper.colProp_Length_m.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitMass_LM.ColumnName, QuotationHelper.colProp_UnitMass_LM.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_LM_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(table);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            DataRow row = null;
            foreach (QuotationItem item in quotation)
            {
                row = table.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Name.ColumnName] = item.Name;
                    row[QuotationHelper.colProp_Length_m.ColumnName] = item.Length.ToString("F2");
                    row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = 0; // TODO //item.UnitMassLength.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = 0; // TODO //item.UnitPrice_Length_NZD.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            // Last row
            row = table.NewRow();
            row[QuotationHelper.colProp_Name.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Length_m.ColumnName] = "";
            row[QuotationHelper.colProp_UnitMass_LM.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalItemsMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_LM_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalItemsPrice_Table.ToString("F2");
            table.Rows.Add(row);

            return ds;
        }

        public static DataSet GetTableCladdingAccessories_Items_Piece(List<CCladdingAccessories_Item_Piece> claddingAccessoriesItems_Piece, ref double dBuildingMass, ref double dBuildingNetPrice_WithoutMargin_WithoutGST)
        {
            if (claddingAccessoriesItems_Piece == null) return null;
            if (claddingAccessoriesItems_Piece.Count == 0) return null;
            // IN WORK - rozpracovane

            // TODO - Dopracovat
            double dTotalItemsMass_Table = 0;
            double dTotalItemsPrice_Table = 0;
            int iTotalItemsNumber_Table = 0;

            List<QuotationItem> quotation = new List<QuotationItem>(); // TODO Docanse - upravit 
            foreach (CCladdingAccessories_Item_Piece item in claddingAccessoriesItems_Piece)
            {
                QuotationItem qitem = new QuotationItem();

                // Nastavime parametre z CCladdingAccessories_Item_Piece do QuotationItem (TO Ondrej, toto je asi zbytocny krok ???)
                qitem.Name = item.Name;
                qitem.Quantity = item.Count;
                qitem.Note = item.Note;

                quotation.Add(qitem);

                //dTotalItemsMass_Table += item.Mass_per_piece;
                //dTotalItemsPrice_Table += item.Price_PPP_NZD;
                iTotalItemsNumber_Table += item.Count;
            }

            dBuildingMass += dTotalItemsMass_Table;
            dBuildingNetPrice_WithoutMargin_WithoutGST += dTotalItemsPrice_Table;

            // Create Table
            DataTable table = new DataTable("CladdingAccessories_Items_Piece");
            // Create Table Rows
            table.Columns.Add(QuotationHelper.colProp_Name.ColumnName, QuotationHelper.colProp_Name.DataType);
            table.Columns.Add(QuotationHelper.colProp_Count.ColumnName, QuotationHelper.colProp_Count.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitMass_P.ColumnName, QuotationHelper.colProp_UnitMass_P.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalMass.ColumnName, QuotationHelper.colProp_TotalMass.DataType);
            table.Columns.Add(QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName, QuotationHelper.colProp_UnitPrice_P_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_TotalPrice_NZD.ColumnName, QuotationHelper.colProp_TotalPrice_NZD.DataType);
            table.Columns.Add(QuotationHelper.colProp_Note.ColumnName, QuotationHelper.colProp_Note.DataType);

            // Set Table Column Properties
            QuotationHelper.SetDataTableColumnProperties(table);

            // Create Datases
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            DataRow row = null;
            foreach (QuotationItem item in quotation)
            {
                row = table.NewRow();

                try
                {
                    row[QuotationHelper.colProp_Name.ColumnName] = item.Name;
                    row[QuotationHelper.colProp_Count.ColumnName] = item.Quantity;
                    row[QuotationHelper.colProp_UnitMass_P.ColumnName] = item.MassPerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalMass.ColumnName] = item.TotalMass.ToString("F2");
                    row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = item.PricePerPiece.ToString("F2");
                    row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = item.TotalPrice.ToString("F2");
                    row[QuotationHelper.colProp_Note.ColumnName] = item.Note;
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }

            // Last row
            row = table.NewRow();
            row[QuotationHelper.colProp_Name.ColumnName] = "Total:";
            row[QuotationHelper.colProp_Count.ColumnName] = iTotalItemsNumber_Table;
            row[QuotationHelper.colProp_UnitMass_P.ColumnName] = "";
            row[QuotationHelper.colProp_TotalMass.ColumnName] = dTotalItemsMass_Table.ToString("F2");
            row[QuotationHelper.colProp_UnitPrice_P_NZD.ColumnName] = "";
            row[QuotationHelper.colProp_TotalPrice_NZD.ColumnName] = dTotalItemsPrice_Table.ToString("F2");
            row[QuotationHelper.colProp_Note.ColumnName] = "";
            table.Rows.Add(row);

            return ds;
        }

    }
}