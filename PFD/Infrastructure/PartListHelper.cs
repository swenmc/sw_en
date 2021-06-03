using BaseClasses;
using BaseClasses.GraphObj;
using MATH;
using PFD.Infrastructure;
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

            double dLimitSheetLengthToConsider = 0.20; // Neuvazovat kratsie plechy ako je tento limit
            double dLimitSheetWidthToConsider = 0.10; // Neuvazovat uzsie plechy ako je tento limit

            if (cladding.HasCladdingSheets_Roof())
            {
                // 11 - Standard Roofing
                // Sposob A
                double ribWidthRoof = vm._claddingOptionsVM.RoofCladdingProps.widthRib_m;
                fixingPointTributaryArea = ribWidthRoof * vm.Model.fDist_Purlin;

                double fRoofCladdingAreaFibreglass = vm._claddingOptionsVM.FibreglassAreaRoofRatio / 100 * vm.TotalRoofArea; // Todo skontrolovat
                double fRoofCladdingArea_WithoutFibreglass = vm.TotalRoofAreaInclCanopies - fRoofCladdingAreaFibreglass; // Todo skontrolovat

                iNumberOfFixingPoints = (int)(fRoofCladdingArea_WithoutFibreglass / fixingPointTributaryArea);

                // Pridavok
                iNumberOfFixingPoints = (int)(iNumberOfFixingPoints * 0.8571f); // Navysime pocet o rezervu

                if (vm._modelOptionsVM.IndividualCladdingSheets)
                {
                    // Sposob B

                    int iNumberOfFixingPoints2 = 0;

                    if (cladding.HasCladdingSheets_RoofRight())
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfCladdingSheetsRoofRight)
                        {
                            if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                                iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal / vm.Model.fDist_Purlin)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/); // Nepripocitavame 1, pretoze plechy sa prekryvaju
                        }
                    }

                    if (cladding.HasCladdingSheets_RoofLeft())
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfCladdingSheetsRoofLeft)
                        {
                            if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                                iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal / vm.Model.fDist_Purlin)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/); // Nepripocitavame 1, pretoze plechy sa prekryvaju
                        }
                    }

                    // Kontrola - priblizna
                    if (vm.debugging && !MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 25))
                    {
                        // Exception
                        throw new Exception("Algorithm error. Different count of items!");
                    }

                    iNumberOfFixingPoints = iNumberOfFixingPoints2; // Ak su zapnute individual sheet, pouzijeme vysledky z individual sheets
                }

                itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx115 (plastic profile washer and galvanized cap)", iNumberOfFixingPoints, "Roof Cladding");
                claddingAccessoriesItems_Piece.Add(itemPiece);

                if (cladding.HasFibreglassSheets_Roof())
                {
                    // 12 - Fibreglass rooflites

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

                    // Pridavok
                    iNumberOfFixingPoints = (int)(iNumberOfFixingPoints * 0.700f); // Navysime pocet o rezervu

                    // Sposob B

                    int iNumberOfFixingPoints2 = 0;
                    iNumberLapstitchFixingPoints = 0; // Pozdlzne na okraji sheet, To Ondrej - TODO doriesit ak su 2 fibreglass sheets vedla seba, asi by sme vedeli osetrit aspon ciastocne podla pozicie X a Y zobrat okraj len raz resp max. dlzku ak su ine
                    dLapstitchFixingPointsSpacing = 0.6; // TODO napojit na DB - hodnota je v DB

                    if (cladding.HasFibreglassSheets_RoofRight())
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfFibreGlassSheetsRoofRight)
                        {
                            if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                            {
                                iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal / vm.Model.fDist_Purlin)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);

                                // TO Ondrej - zistit ci existuje nejaky iny susediaci sheet s rovnakou suradnicou Y, ktory je o v pozicii i+1
                                // Ak ano, ak by sme mali pouzit pre iNumberLapstitchFixingPoints namiesto 2 * len 1 * maximum z sheet[i].LengthTotal a sheet[i+1].LengthTotal
                                //iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal / dLapstitchFixingPointsSpacing);
                                int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenPurlins * ((int)(sheet.LengthTotal / vm.Model.fDist_Purlin) + 1);
                                supportBracketBetweenPurlinsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                iNumberOfSupportBracketBetweenPurlinsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 + 2); // Pridany jeden bod pre koncove rebro FG + 2 pre rebra cladding sheet
                                dLapFoamPacker_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                            }
                        }

                        // TO Ondrej - spocitam celkovu maximalnu dlzku
                        double dLapstitchLengthTotal = 2 * cladding.listOfFibreGlassSheetsRoofRight.Sum(item => item.LengthTotal);

                        // Pre kazdu poziciu X a RoofLength_Y prejdeme sheets ktore v danom X koncia a ktore zacinaju 
                        // Vytvorime si nejaky zoznam intervalov Y kde sheet zacina a konci
                        // V danom mieste X porovname tieto intervaly a zistime na akej dlzke sa vzajomne prekryvaju

                        // to Mato - a toto je ako co? myslim ten index = 0
                        //List<float> xpositions = vm._claddingOptionsVM.FibreglassProperties[0].XValues; 
                        //myslel si toto? 
                        List<float> xpositions = vm._claddingOptionsVM.FibreglassProperties.FirstOrDefault(f => f.Side == "Roof" || f.Side == "Roof-Right Side").XValues;

                        double dIntersectionLengthTotal = cladding.GetSheetCollectionLongitudinalIntersectionLength(cladding.listOfFibreGlassSheetsRoofRight, xpositions, vm.RoofLength_Y);

                        // Tuto dlzku odpocitame od dLapstitchLengthTotal
                        dLapstitchLengthTotal -= dIntersectionLengthTotal;
                        iNumberLapstitchFixingPoints += (int)Math.Round(dLapstitchLengthTotal / dLapstitchFixingPointsSpacing);
                    }

                    if (cladding.HasFibreglassSheets_RoofLeft())
                    {
                        foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfFibreGlassSheetsRoofLeft)
                        {
                            if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                            {
                                iNumberOfFixingPoints2 += ((int)(sheet.LengthTotal / vm.Model.fDist_Purlin)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                //iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal / dLapstitchFixingPointsSpacing);
                                int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenPurlins * ((int)(sheet.LengthTotal / vm.Model.fDist_Purlin) + 1);
                                supportBracketBetweenPurlinsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                iNumberOfSupportBracketBetweenPurlinsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1 + 2); // Pridany jeden bod pre koncove rebro FG + 2 pre rebra cladding sheet
                                dLapFoamPacker_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                            }
                        }

                        // TO Ondrej - spocitam celkovu maximalnu dlzku
                        double dLapstitchLengthTotal = 2 * cladding.listOfFibreGlassSheetsRoofLeft.Sum(item => item.LengthTotal);

                        // Pre kazdu poziciu X a RoofLength_Y prejdeme sheets ktore v danom X koncia a ktore zacinaju 
                        // Vytvorime si nejaky zoznam intervalov Y kde sheet zacina a konci
                        // V danom mieste X porovname tieto intervaly a zistime na akej dlzke sa vzajomne prekryvaju
                        //List<float> xpositions = vm._claddingOptionsVM.FibreglassProperties[0].XValues;
                        //Mato - myslel si toto?
                        List<float> xpositions = vm._claddingOptionsVM.FibreglassProperties.FirstOrDefault(f => f.Side == "Roof-Left Side").XValues;
                        double dIntersectionLengthTotal = cladding.GetSheetCollectionLongitudinalIntersectionLength(cladding.listOfFibreGlassSheetsRoofLeft, xpositions, vm.RoofLength_Y);

                        // Tuto dlzku odpocitame od dLapstitchLengthTotal
                        dLapstitchLengthTotal -= dIntersectionLengthTotal;
                        iNumberLapstitchFixingPoints += (int)Math.Round(dLapstitchLengthTotal / dLapstitchFixingPointsSpacing);
                    }

                    // Kontrola - priblizna
                    if (vm.debugging && !MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 25))
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
                        if (vm.FibreglassRoofRidgeCapFlashing_TotalLength > 0)
                        {
                            // Todo 841 - napojit - To Ondrej - prosim skontrolovat
                            int iNumberOfRidgePlasticBlocks = (int)(vm.FibreglassRoofRidgeCapFlashing_TotalLength / ribWidthRoof);
                            itemPiece = new CCladdingAccessories_Item_Piece("Plastic ridge block - fibreglass", iNumberOfRidgePlasticBlocks);
                            claddingAccessoriesItems_Piece.Add(itemPiece);
                        }
                    }

                    // 13 - Rooflite support bracket

                    // Support bracket
                    itemPiece = new CCladdingAccessories_Item_Piece("Fibreglass support bracket 30x40x1400 - 1 mm", (int)(supportBracketBetweenPurlinsLengthTotal / 1.4f) + 1, "Roof Fibreglass Support Bracket");
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
                        iNumberOfFixingPoints = 2 * ((int)(vm.RoofLength_Y / ribWidthRoof) + 1);

                        bool bStandardRidge = vm._doorsAndWindowsVM.HasFlashing(EFlashingType.RoofRidge); // TODO - napojit - accessories flashings, neviem ci je infill ridge  odpovedajuce soft edge alebo tym myslia nieco ine - Otazka na NZ

                        if (bStandardRidge)
                        {
                            // 15 - Standard ridge

                            // Apex ridge flashing rivets
                            itemPiece = new CCladdingAccessories_Item_Piece("Apex ridge flashing rivet 73AS6.4", iNumberOfFixingPoints, "Standard Ridge");
                            claddingAccessoriesItems_Piece.Add(itemPiece);
                        }
                        else
                        {
                            // 16 - Infill ridge

                            // TEK screws 14gx115
                            itemPiece = new CCladdingAccessories_Item_Piece("Apex ridge flashing TEK screw 14gx115  (neo washer)", iNumberOfFixingPoints, "Infill Ridge");
                            claddingAccessoriesItems_Piece.Add(itemPiece);

                            // Plastic ridge blocks
                            itemPiece = new CCladdingAccessories_Item_Piece("Plastic ridge block", iNumberOfFixingPoints, "Infill Ridge");
                            claddingAccessoriesItems_Piece.Add(itemPiece);

                            // TEK screws 12gx20
                            itemPiece = new CCladdingAccessories_Item_Piece("Ridge TEK screw 12gx20 (neo washer)", iNumberOfFixingPoints, "Infill Ridge");
                            claddingAccessoriesItems_Piece.Add(itemPiece);
                        }

                        // TODO
                        // Apex brace - malo by to byt samostatne pri plates // Otazka na Zeland - ake je spacing
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

                // 17 - Barge

                double dBargeflashingFixingSpacing = 0.3f; // DB
                int iNumberOfFixingPointsBirdProofFlashing = 0;
                double dFixingPointsBargeCladdingSheetEdge = 2; // DB
                int iNumberOfFixingPointsBargeCladdingSheetEdge = 0;

                double dGutterBracketSpacing = 2 * vm._claddingOptionsVM.RoofCladdingProps.widthRib_m; // DB
                int iNumberOfGutterBrackets = 0;
                int iNumberOfGutterBracketFixingPoints = 0;
                int iNumberOfGutterFixingPoints = 0;
                double dEavePurlinBirdProofFixingPointSpacing = 1; // DB
                int iNumberEavePurlinBirdProofFixingPoints = 0;

                // TODO 840 - Barge Flashing Length - To Ondrej

                int iRoofSidesCount = 0;

                if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
                {
                    if (cladding.HasCladdingSheets_WallFront() && cladding.HasCladdingSheets_WallBack()) iRoofSidesCount = 2;
                    else if (cladding.HasCladdingSheets_WallFront() || cladding.HasCladdingSheets_WallBack()) iRoofSidesCount = 1;
                    else iRoofSidesCount = 0;

                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.Barge))
                    {
                        iNumberOfFixingPoints = 2 * (iRoofSidesCount * ((int)(vm.RoofSideLength / dBargeflashingFixingSpacing) + 1)); // Top and bottom
                        iNumberOfFixingPointsBargeCladdingSheetEdge = Math.Min(2, iRoofSidesCount * ((int)(vm.RoofSideLength / dFixingPointsBargeCladdingSheetEdge) + 1));

                        if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.BargeBirdproof) && (cladding.HasCladdingSheets_WallFront() || cladding.HasCladdingSheets_WallBack()))
                            iNumberOfFixingPointsBirdProofFlashing = iRoofSidesCount * ((int)(vm.RoofSideLength / vm._claddingOptionsVM.WallCladdingProps.widthRib_m) + 1);
                    }

                    if (vm._doorsAndWindowsVM.HasGutter())
                    {
                        iNumberOfGutterBrackets = (int)(vm.RoofLength_Y / dGutterBracketSpacing) + 1;
                        iNumberOfGutterBracketFixingPoints = 2 * iNumberOfGutterBrackets;

                        iNumberOfGutterFixingPoints = (int)(vm.RoofLength_Y / vm._claddingOptionsVM.RoofCladdingProps.widthRib_m) + 1; // Each pan
                        iNumberOfGutterFixingPoints += iNumberOfGutterBrackets;
                    }

                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.EavePurlinBirdproofStrip))
                        iNumberEavePurlinBirdProofFixingPoints = (int)(vm.RoofLength_Y / dEavePurlinBirdProofFixingPointSpacing) + 1;
                }
                else if (vm.KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
                {
                    if (cladding.HasCladdingSheets_WallFront() && cladding.HasCladdingSheets_WallBack()) iRoofSidesCount = 4;
                    else if (cladding.HasCladdingSheets_WallFront() || cladding.HasCladdingSheets_WallBack()) iRoofSidesCount = 2;
                    else iRoofSidesCount = 0;

                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.Barge))
                    {
                        iNumberOfFixingPoints = 2 * (iRoofSidesCount * ((int)(vm.RoofSideLength / dBargeflashingFixingSpacing) + 1)); // Top and bottom
                        iNumberOfFixingPointsBargeCladdingSheetEdge = Math.Min(2, iRoofSidesCount * ((int)(vm.RoofSideLength / dFixingPointsBargeCladdingSheetEdge) + 1));

                        if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.BargeBirdproof) && (cladding.HasCladdingSheets_WallFront() || cladding.HasCladdingSheets_WallBack()))
                            iNumberOfFixingPointsBirdProofFlashing = iRoofSidesCount * ((int)(vm.RoofSideLength / vm._claddingOptionsVM.WallCladdingProps.widthRib_m) + 1);
                    }

                    if (vm._doorsAndWindowsVM.HasGutter())
                    {
                        iNumberOfGutterBrackets = 2 * ((int)(vm.RoofLength_Y / dGutterBracketSpacing) + 1);
                        iNumberOfGutterBracketFixingPoints = 2 * iNumberOfGutterBrackets;

                        iNumberOfGutterFixingPoints = 2 * ((int)(vm.RoofLength_Y / vm._claddingOptionsVM.RoofCladdingProps.widthRib_m) + 1); // Each pan
                        iNumberOfGutterFixingPoints += iNumberOfGutterBrackets;
                    }

                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.EavePurlinBirdproofStrip))
                        iNumberEavePurlinBirdProofFixingPoints = 2 * ((int)(vm.RoofLength_Y / dEavePurlinBirdProofFixingPointSpacing) + 1);
                }

                if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.Barge))
                {
                    // Barge flashing fixing - Rivets
                    itemPiece = new CCladdingAccessories_Item_Piece("Barge flashing rivet 73AS6.4", iNumberOfFixingPoints);
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Barge cladding sheet edge fixing - TEK screws 12gx42
                    itemPiece = new CCladdingAccessories_Item_Piece("TEK screw 14gx42 (bonded washer)", iNumberOfFixingPointsBirdProofFlashing, "Barge");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.BargeBirdproof) && (cladding.HasCladdingSheets_WallFront() || cladding.HasCladdingSheets_WallBack()))
                    {
                        // Bird proof flashing fixing - Rivets
                        itemPiece = new CCladdingAccessories_Item_Piece("Birdgproof flashing rivet 73AS6.4", iNumberOfFixingPointsBirdProofFlashing, "Barge");
                        claddingAccessoriesItems_Piece.Add(itemPiece);
                    }
                }

                if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.EavePurlinBirdproofStrip))
                {
                    // Eave purlin bird proof flashing fixing
                    itemPiece = new CCladdingAccessories_Item_Piece("Birdproof strip wafer TEK screw 10g", iNumberEavePurlinBirdProofFixingPoints, "Eave purlin");
                    claddingAccessoriesItems_Piece.Add(itemPiece);

                    // Eave purlin bird proof plastic blocks
                    itemPiece = new CCladdingAccessories_Item_Piece("Plastic gutter block", iNumberEavePurlinBirdProofFixingPoints, "Eave purlin");
                    claddingAccessoriesItems_Piece.Add(itemPiece);
                }

                if (vm._doorsAndWindowsVM.HasGutter())
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

            if (cladding.HasCladdingSheets_Wall())
            {
                // Openings
                // Urcime zakladne parametre, mohli by sem uz prist pripravene

                double dBuildingPerimeter = 2 * (vm.LengthOverall + vm.WidthOverall);
                double dBuildingCladdingPerimeterWithoutDoors = dBuildingPerimeter; // Obvod budovy bez sirky dveri

                // Wall Fibreglass Area
                double fWallCladdingAreaFibreglass = vm._claddingOptionsVM.FibreglassAreaWallRatio / 100 * vm.TotalWallArea; // Todo skontrolovat

                int iNumberOfRollerDoorTrimmers = 0; // Trimmers or extension plates

                bool bAnyRollerDoorExists = vm._doorsAndWindowsVM.ModelHasRollerDoor();
                bool bAnyPADoorExists = vm._doorsAndWindowsVM.ModelHasPersonelDoor();

                // Wall Doors and Windows Area
                double dDoorsAndWindowsOpeningArea = CDoorsAndWindowsHelper.GetDoorsAndWindowsOpeningArea(vm);

                if (vm._doorsAndWindowsVM.ModelHasDoor())
                {
                    foreach (DoorProperties door in vm._doorsAndWindowsVM.DoorBlocksProperties)
                    {
                        dBuildingCladdingPerimeterWithoutDoors -= door.fDoorsWidth;

                        if (door.sDoorType == "Roller Door")
                            iNumberOfRollerDoorTrimmers += 2;
                    }
                }

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
                iNumberOfFixingPoints = (int)(iNumberOfFixingPoints * 0.8506f); // Navysime pocet o rezervu

                if (vm._modelOptionsVM.IndividualCladdingSheets)
                {
                    // Sposob B

                    int iNumberOfFixingPoints2 = 0;

                    if (cladding.HasCladdingSheets_WallLeft())
                    {
                        int iSeamFixingPointsPerSheetWidth = 1;

                        foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfCladdingSheetsLeftWall)
                        {
                            if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                            {
                                if (!vm._modelOptionsVM.IndividualCladdingSheets)
                                    iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet

                                iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(sheet.LengthTotal / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                            }
                        }
                    }

                    if (cladding.HasCladdingSheets_WallFront())
                    {
                        int iSeamFixingPointsPerSheetWidth = 1;

                        foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfCladdingSheetsFrontWall)
                        {
                            if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                            {
                                double dSheetLength = sheet.LengthTotal;
                                if (!vm._modelOptionsVM.IndividualCladdingSheets)
                                {
                                    if (sheet.NumberOfEdges == 5)
                                        dSheetLength = MathF.Average(sheet.LengthTopLeft, sheet.LengthTopTip);
                                    else
                                        dSheetLength = MathF.Average(sheet.LengthTopLeft, sheet.LengthTopRight);

                                    iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet
                                }

                                iNumberOfFixingPoints2 += profileFactor * ((int)(dSheetLength / vm.Model.fDist_Girt)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(dSheetLength / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                            }
                        }
                    }

                    if (cladding.HasCladdingSheets_WallRight())
                    {
                        int iSeamFixingPointsPerSheetWidth = 1;

                        foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfCladdingSheetsRightWall)
                        {
                            if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                            {
                                if (!vm._modelOptionsVM.IndividualCladdingSheets)
                                    iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet

                                iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(sheet.LengthTotal / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                            }
                        }
                    }

                    if (cladding.HasCladdingSheets_WallBack())
                    {
                        int iSeamFixingPointsPerSheetWidth = 1;

                        foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfCladdingSheetsBackWall)
                        {
                            if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                            {
                                double dSheetLength = sheet.LengthTotal;
                                if (!vm._modelOptionsVM.IndividualCladdingSheets)
                                {
                                    if (sheet.NumberOfEdges == 5)
                                        dSheetLength = MathF.Average(sheet.LengthTopLeft, sheet.LengthTopTip);
                                    else
                                        dSheetLength = MathF.Average(sheet.LengthTopLeft, sheet.LengthTopRight);

                                    iSeamFixingPointsPerSheetWidth = (int)(sheet.Width / sheet.BasicModularWidth) + 1; // Nemusime uvazovat okraj, tam je corner flashing, ale uvazujem to ako rezervu a aby sedel pocet s individual sheet
                                }

                                iNumberOfFixingPoints2 += profileFactor * ((int)(dSheetLength / vm.Model.fDist_Girt)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                iNumberCladdingSeamFixingPoints += iSeamFixingPointsPerSheetWidth * ((int)(dSheetLength / dCladdingSeamFixingSpacing) + 1); // One sheet side only
                            }
                        }
                    }

                    // Kontrola - priblizna
                    if (vm.debugging && !MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 25))
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

                // TO Ondrej - nastavit spravne pocet rohov, default je 4 kedze predpokladame ze existuju vsetky steny,
                // ak je niektora zo stien wall cladding deaktivovana tak je pocet rohov 2, ak su deaktivovane 2 (nie protilahle),
                // tak jedna a ked je zapnuta len jedna, dve protilahle alebo ziadna tak 0
                // vtedy sme nemali tento riadok zobrazit vobec, vseobecne by bolo dobre pridat podmienku ze ak je item count = 0 alebo item length = 0 m, tak sa do
                // tabuliek part list nepridaju
                // TO Ondrej - prosim skontrolovat tieto podmienky

                int iNumberOfCorners = 0;

                if (cladding.HasCladdingSheets_WallLeft() && cladding.HasCladdingSheets_WallFront() && cladding.HasCladdingSheets_WallRight() && cladding.HasCladdingSheets_WallBack())
                    iNumberOfCorners = 4; // Styri steny
                else if ((cladding.HasCladdingSheets_WallLeft() && cladding.HasCladdingSheets_WallFront() && cladding.HasCladdingSheets_WallRight()) ||
                    (cladding.HasCladdingSheets_WallFront() && cladding.HasCladdingSheets_WallRight() && cladding.HasCladdingSheets_WallBack()) ||
                    (cladding.HasCladdingSheets_WallRight() && cladding.HasCladdingSheets_WallBack() && cladding.HasCladdingSheets_WallLeft()) ||
                    (cladding.HasCladdingSheets_WallBack() && cladding.HasCladdingSheets_WallLeft() && cladding.HasCladdingSheets_WallFront()))
                    iNumberOfCorners = 2; // Len tri steny, ktore tvoria dva rohy
                else if ((cladding.HasCladdingSheets_WallLeft() && cladding.HasCladdingSheets_WallFront()) ||
                    (cladding.HasCladdingSheets_WallFront() && cladding.HasCladdingSheets_WallRight()) ||
                    (cladding.HasCladdingSheets_WallRight() && cladding.HasCladdingSheets_WallBack()) ||
                    (cladding.HasCladdingSheets_WallBack() && cladding.HasCladdingSheets_WallLeft()))
                    iNumberOfCorners = 1; // Len dve steny, ktore tvoria jeden roh
                else
                    iNumberOfCorners = 0; // Dve protilahle steny, jedna stena, alebo ziadna stena

                if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.WallCorner))
                {
                    // 22 - Cladding corner

                    double dCornerFlashingFixingPointsSpacing = 0.3; // DB (kotvenie dvoch stran flashing)
                    iNumberOfFixingPoints = 2 * ((int)(vm.WallCornerFlashing_TotalLength / dCornerFlashingFixingPointsSpacing) + iNumberOfCorners); // Len priblizne

                    itemPiece = new CCladdingAccessories_Item_Piece("Corner flashing rivet 73AS6.4", iNumberOfFixingPoints, "Wall Cladding");
                    claddingAccessoriesItems_Piece.Add(itemPiece);
                }

                if (cladding.HasFibreglassSheets_Wall())
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

                    // Pridavok
                    iNumberOfFixingPoints = (int)(iNumberOfFixingPoints * 0.700f); // Navysime pocet o rezervu

                    if (vm._modelOptionsVM.IndividualCladdingSheets)
                    {
                        // Sposob B

                        int iNumberOfFixingPoints2 = 0;
                        iNumberLapstitchFixingPoints = 0; // Pozdlzne na okraji sheet, To Ondrej - TODO doriesit ak su 2 fibreglass sheets vedla seba, asi by sme vedeli osetrit aspon ciastocne podla pozicie X a Y zobrat okraj len raz resp max. dlzku ak su ine
                        dLapstitchFixingPointsSpacing = 0.6; // TODO napojit na DB - hodnota je v DB

                        if (cladding.HasFibreglass_WallLeft())
                        {
                            foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfFibreGlassSheetsWallLeft)
                            {
                                if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                                {
                                    iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                    // TO Ondrej - zistit ci existuje nejaky iny susediaci sheet s rovnakou suradnicou Y, ktory je o v pozicii i+1
                                    // Ak ano, ak by sme mali pouzit pre iNumberLapstitchFixingPoints namiesto 2 * len 1 * maximum z sheet[i].LengthTotal a sheet[i+1].LengthTotal
                                    iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal / dLapstitchFixingPointsSpacing);
                                    int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt) + 1);
                                    iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                    supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                    iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                    dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                                }
                            }
                        }

                        if (cladding.HasFibreglass_WallFront())
                        {
                            foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfFibreGlassSheetsWallFront)
                            {
                                if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                                {
                                    iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                    // TO Ondrej - zistit ci existuje nejaky iny susediaci sheet
                                    iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal / dLapstitchFixingPointsSpacing);
                                    int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt) + 1);
                                    iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                    supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                    iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                    dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                                }
                            }
                        }

                        if (cladding.HasFibreglass_WallRight())
                        {
                            foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfFibreGlassSheetsWallRight)
                            {
                                if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                                {
                                    iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                    // TO Ondrej - zistit ci existuje nejaky iny susediaci sheet
                                    iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal / dLapstitchFixingPointsSpacing);
                                    int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt) + 1);
                                    iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                    supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                    iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                    dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                                }
                            }
                        }

                        if (cladding.HasFibreglass_WallBack())
                        {
                            foreach (CCladdingOrFibreGlassSheet sheet in cladding.listOfFibreGlassSheetsWallBack)
                            {
                                if (sheet.LengthTotal_Real > dLimitSheetLengthToConsider && sheet.Width > dLimitSheetWidthToConsider)
                                {
                                    iNumberOfFixingPoints2 += profileFactor * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt)/* + 1*/) * ((int)(sheet.Width / sheet.CladdingWidthRibModular)/* + 1*/);
                                    // TO Ondrej - zistit ci existuje nejaky iny susediaci sheet
                                    iNumberLapstitchFixingPoints += 2 * (int)(sheet.LengthTotal / dLapstitchFixingPointsSpacing);
                                    int iNumberOfSupportBracketsPerSheet = iNumberOfSupportBracketBetweenGirts * ((int)(sheet.LengthTotal / vm.Model.fDist_Girt) + 1);
                                    iNumberOfSupportBracketBetweenGirtsToCladdingFixingPoints += 4 * iNumberOfSupportBracketsPerSheet;
                                    supportBracketBetweenGirtsLengthTotal += iNumberOfSupportBracketsPerSheet * sheet.Width;
                                    iNumberOfSupportBracketBetweenGirtsFixingPoints += iNumberOfSupportBracketsPerSheet * ((int)(sheet.Width / sheet.CladdingWidthRibModular) + 1); // Pridany jeden bod pre koncove rebro FG
                                    dLapSealantBead_TotalLength += sheet.Width / sheet.BasicModularWidth * sheet.CoilOrFlatSheetWidth;
                                }
                            }
                        }

                        // Kontrola - priblizna
                        if (vm.debugging && !MathF.i_approxequal(iNumberOfFixingPoints, iNumberOfFixingPoints2, 25))
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

                if (bAnyRollerDoorExists)
                {
                    // 26 - Roller door trim

                    // Roller door trim flashing fixing

                    double dRollerDoorflashingFixingSpacing = 0.3f; // DB
                    iNumberOfFixingPoints = 0;

                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.RollerDoorTrimmer))
                        iNumberOfFixingPoints = 2 * (int)(vm.RollerDoorTrimmerFlashing_TotalLength / dRollerDoorflashingFixingSpacing); // 2 sides resp. top and bottom

                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.RollerDoorHeader))
                        iNumberOfFixingPoints += 5 * (int)(vm.RollerDoorLintelFlashing_TotalLength / dRollerDoorflashingFixingSpacing);

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
                    if (vm._doorsAndWindowsVM.HasFlashing(EFlashingType.PADoorHeaderCap))
                    {
                        // 29 - PA door trim
                        // PA door header cap flashing fixing

                        double dPADoorflashingFixingSpacing = 0.3f; // DB
                        iNumberOfFixingPoints = 2 * (int)(vm.PADoorLintelFlashing_TotalLength / dPADoorflashingFixingSpacing);
                        itemPiece = new CCladdingAccessories_Item_Piece("Flashing rivet 73AS6.4", iNumberOfFixingPoints, "Personnel Door");
                        claddingAccessoriesItems_Piece.Add(itemPiece);
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