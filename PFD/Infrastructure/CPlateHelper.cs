using BaseClasses;
using DATABASE;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace PFD
{
    public static class CPlateHelper
    {
        public static List<CComponentParamsView> GetScrewArrangementProperties(CScrewArrangement screwArrangement)
        {
            int iNumberOfDecimalPlaces_Length = 1;
            float fUnitFactor_Length = 1000;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";


            List<CComponentParamsView> screwArrangmenetProperties = new List<CComponentParamsView>();

            if (screwArrangement != null && screwArrangement is CScrewArrangementCircleApexOrKnee)
            {
                CScrewArrangementCircleApexOrKnee circArrangement = (CScrewArrangementCircleApexOrKnee)screwArrangement;

                List<string> listScrewGauges = CTEKScrewsManager.LoadTEKScrewsProperties().Select(i => i.gauge).ToList();

                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.ScrewGaugeS.Name, CParamsResources.ScrewGaugeS.Symbol, circArrangement.referenceScrew.Gauge.ToString(), listScrewGauges, CParamsResources.ScrewGaugeS.Unit));  // TODO prerobit na vyber objektu skrutky z databazy
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscDepthS.Name, CParamsResources.CrscDepthS.Symbol, (Math.Round(circArrangement.FCrscRafterDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscDepthS.Unit, false));
                //screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebStraightDepthS.Name, CParamsResources.CrscWebStraightDepthS.Symbol, (Math.Round(circArrangement.FCrscWebStraightDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebStraightDepthS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebMiddleStiffenerSizeS.Name, CParamsResources.CrscWebMiddleStiffenerSizeS.Symbol, (Math.Round(circArrangement.FStiffenerSize * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebMiddleStiffenerSizeS.Unit, false));

                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.NumberOfCirclesInGroupS.Name, CParamsResources.NumberOfCirclesInGroupS.Symbol, circArrangement.INumberOfCirclesInGroup.ToString(), CParamsResources.NumberOfCirclesInGroupS.Unit));
                CScrewSequenceGroup gr = circArrangement.ListOfSequenceGroups.FirstOrDefault();
                if (gr != null)
                {
                    int c = 0;
                    for (int i = 0; i < gr.ListSequence.Count; i++)
                    {
                        if (gr.ListSequence[i] is CScrewHalfCircleSequence && i % 2 == 1)
                        {
                            c++;
                            CScrewHalfCircleSequence screwHalfCircleSequence = gr.ListSequence[i] as CScrewHalfCircleSequence;
                            screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.NumberOfScrewsInCircleSequenceS.Name + " " + (c), CParamsResources.NumberOfScrewsInCircleSequenceS.Symbol, screwHalfCircleSequence.INumberOfConnectors.ToString(), CParamsResources.NumberOfScrewsInCircleSequenceS.Unit));
                            screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.RadiusOfScrewsInCircleSequenceS.Name + " " + (c), CParamsResources.RadiusOfScrewsInCircleSequenceS.Symbol, (Math.Round(screwHalfCircleSequence.Radius * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.RadiusOfScrewsInCircleSequenceS.Unit));
                        }
                    }
                }

                screwArrangmenetProperties.Add(new CComponentParamsViewBool(CParamsResources.UseAdditionalCornerScrewsS.Name, CParamsResources.UseAdditionalCornerScrewsS.Symbol, circArrangement.BUseAdditionalCornerScrews, CParamsResources.UseAdditionalCornerScrewsS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.PositionOfCornerSequence_xS.Name, CParamsResources.PositionOfCornerSequence_xS.Symbol, (Math.Round(circArrangement.FPositionOfCornerSequence_x * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PositionOfCornerSequence_xS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.PositionOfCornerSequence_yS.Name, CParamsResources.PositionOfCornerSequence_yS.Symbol, (Math.Round(circArrangement.FPositionOfCornerSequence_y * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PositionOfCornerSequence_yS.Unit));
                //screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.NumberOfAdditionalScrewsInCornerS.Name, CParamsResources.NumberOfAdditionalScrewsInCornerS.Symbol, circArrangement.IAdditionalConnectorInCornerNumber.ToString(nfi), CParamsResources.NumberOfAdditionalScrewsInCornerS.Unit));
                List<string> listAdditionalScrewsInCorner = new List<string>() { "1", "4", "9", "16", "25", "36", "49", "64", "81", "100" };
                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.NumberOfAdditionalScrewsInCornerS.Name, CParamsResources.NumberOfAdditionalScrewsInCornerS.Symbol, circArrangement.IAdditionalConnectorInCornerNumber.ToString(nfi), listAdditionalScrewsInCorner, CParamsResources.NumberOfAdditionalScrewsInCornerS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.DistanceOfAdditionalScrewsInxS.Name, CParamsResources.DistanceOfAdditionalScrewsInxS.Symbol, (Math.Round(circArrangement.FAdditionalScrewsDistance_x * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.DistanceOfAdditionalScrewsInxS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.DistanceOfAdditionalScrewsInyS.Name, CParamsResources.DistanceOfAdditionalScrewsInyS.Symbol, (Math.Round(circArrangement.FAdditionalScrewsDistance_y * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.DistanceOfAdditionalScrewsInyS.Unit));
            }
            else if (screwArrangement != null && screwArrangement is CScrewArrangementRectApexOrKnee)
            {
                CScrewArrangementRectApexOrKnee rectArrangement = (CScrewArrangementRectApexOrKnee)screwArrangement;

                List<string> listScrewGauges = CTEKScrewsManager.LoadTEKScrewsProperties().Select(i => i.gauge).ToList();

                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.ScrewGaugeS.Name, CParamsResources.ScrewGaugeS.Symbol, rectArrangement.referenceScrew.Gauge.ToString(), listScrewGauges, CParamsResources.ScrewGaugeS.Unit));  // TODO prerobit na vyber objektu skrutky z databazy
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscDepthS.Name, CParamsResources.CrscDepthS.Symbol, (Math.Round(rectArrangement.FCrscRafterDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscDepthS.Unit, false));
                //screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebStraightDepthS.Name, CParamsResources.CrscWebStraightDepthS.Symbol, (Math.Round(rectArrangement.FCrscWebStraightDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebStraightDepthS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebMiddleStiffenerSizeS.Name, CParamsResources.CrscWebMiddleStiffenerSizeS.Symbol, (Math.Round(rectArrangement.FStiffenerSize * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebMiddleStiffenerSizeS.Unit, false));

                // TODO Ondrej, toto by malo byt obecne pre rozny pocet rectangular sequences, pripadne groups

                // TODO - Ondrej, TODO No. 105
                // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
                // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
                // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ1", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ1", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ1", "xc1", (Math.Round(rectArrangement.fx_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ1", "yc1", (Math.Round(rectArrangement.fy_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ1", "x1", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ1", "y1", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ2", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ2", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ2", "xc2", (Math.Round(rectArrangement.fx_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ2", "yc2", (Math.Round(rectArrangement.fy_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ2", "bx2", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ2", "by2", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                if (rectArrangement.iNumberOfScrewsInRow_xDirection_SQ3 != 0 && rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ3 != 0)
                {
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ3", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ3.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ3", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ3.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ3", "xc3", (Math.Round(rectArrangement.fx_c_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ3", "yc3", (Math.Round(rectArrangement.fy_c_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ3", "x3", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ3", "y3", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                }
                if (rectArrangement.iNumberOfScrewsInRow_xDirection_SQ4 != 0 && rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ4 != 0)
                {
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ4", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ4.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ4", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ4.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ4", "xc4", (Math.Round(rectArrangement.fx_c_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ4", "yc4", (Math.Round(rectArrangement.fy_c_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ4", "x4", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ4", "y4", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                }
            }
            else if (screwArrangement != null && screwArrangement is CScrewArrangement_BX_1)
            {
                CScrewArrangement_BX_1 rectArrangement = (CScrewArrangement_BX_1)screwArrangement;

                List<string> listScrewGauges = CTEKScrewsManager.LoadTEKScrewsProperties().Select(i => i.gauge).ToList();

                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.ScrewGaugeS.Name, CParamsResources.ScrewGaugeS.Symbol, rectArrangement.referenceScrew.Gauge.ToString(), listScrewGauges, CParamsResources.ScrewGaugeS.Unit));  // TODO prerobit na vyber objektu skrutky z databazy
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscDepthS.Name, CParamsResources.CrscDepthS.Symbol, (Math.Round(rectArrangement.FCrscColumnDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscDepthS.Unit, false));
                //screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebStraightDepthS.Name, CParamsResources.CrscWebStraightDepthS.Symbol, (Math.Round(rectArrangement.FCrscWebStraightDepth * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebStraightDepthS.Unit));
                screwArrangmenetProperties.Add(new CComponentParamsViewString(CParamsResources.CrscWebMiddleStiffenerSizeS.Name, CParamsResources.CrscWebMiddleStiffenerSizeS.Symbol, (Math.Round(rectArrangement.FStiffenerSize * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.CrscWebMiddleStiffenerSizeS.Unit, false));

                // TODO Ondrej, toto by malo byt obecne pre rozny pocet rectangular sequences, pripadne groups

                // TODO - Ondrej, TODO No. 105
                // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
                // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
                // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ1", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ1", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ1", "xc1", (Math.Round(rectArrangement.fx_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ1", "yc1", (Math.Round(rectArrangement.fy_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ1", "x1", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ1", "y1", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ2", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ2", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ2", "xc2", (Math.Round(rectArrangement.fx_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ2", "yc2", (Math.Round(rectArrangement.fy_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ2", "bx2", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ2", "by2", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                if (rectArrangement.iNumberOfScrewsInRow_xDirection_SQ3 != 0 && rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ3 != 0)
                {
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ3", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ3.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ3", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ3.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ3", "xc3", (Math.Round(rectArrangement.fx_c_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ3", "yc3", (Math.Round(rectArrangement.fy_c_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ3", "x3", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ3", "y3", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ3 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                }
                if (rectArrangement.iNumberOfScrewsInRow_xDirection_SQ4 != 0 && rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ4 != 0)
                {
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ4", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ4.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ4", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ4.ToString(), "[-]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ4", "xc4", (Math.Round(rectArrangement.fx_c_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ4", "yc4", (Math.Round(rectArrangement.fy_c_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ4", "x4", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                    screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ4", "y4", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ4 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                }
            }
            else if (screwArrangement != null && screwArrangement is CScrewArrangement_O)
            {
                CScrewArrangement_O rectArrangement = (CScrewArrangement_O)screwArrangement;

                List<string> listScrewGauges = CTEKScrewsManager.LoadTEKScrewsProperties().Select(i => i.gauge).ToList();

                screwArrangmenetProperties.Add(new CComponentParamsViewList(CParamsResources.ScrewGaugeS.Name, CParamsResources.ScrewGaugeS.Symbol, rectArrangement.referenceScrew.Gauge.ToString(), listScrewGauges, CParamsResources.ScrewGaugeS.Unit));  // TODO prerobit na vyber objektu skrutky z databazy
                // TODO Ondrej, toto by malo byt obecne pre rozny pocet rectangular sequences, pripadne groups
                // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                // Jedna skupina a 2 sekvencie
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ1", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ1", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ1.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ1", "xc1", (Math.Round(rectArrangement.fx_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ1", "yc1", (Math.Round(rectArrangement.fy_c_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ1", "x1", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ1", "y1", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));

                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in row SQ2", "No", rectArrangement.iNumberOfScrewsInRow_xDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Number of screws in column SQ2", "No", rectArrangement.iNumberOfScrewsInColumn_yDirection_SQ2.ToString(), "[-]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate x SQ2", "xc2", (Math.Round(rectArrangement.fx_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Inserting point coordinate y SQ2", "yc2", (Math.Round(rectArrangement.fy_c_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws x SQ2", "bx2", (Math.Round(rectArrangement.fDistanceOfPointsX_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
                screwArrangmenetProperties.Add(new CComponentParamsViewString("Distance between screws y SQ2", "by2", (Math.Round(rectArrangement.fDistanceOfPointsY_SQ2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), "[mm]"));
            }
            else
            {
                // Screw arrangement is not implemented
            }

            return screwArrangmenetProperties;
        }


        public static void DataGridScrewArrangement_ValueChanged(CComponentParamsView item, CPlate plate)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            float fLengthUnitFactor = 1000; // GUI input in mm, change to m used in source code

            // Set current screw arrangement parameters
            if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangementCircleApexOrKnee)
            {
                CScrewArrangementCircleApexOrKnee arrangementTemp = (CScrewArrangementCircleApexOrKnee)plate.ScrewArrangement;
                if (item is CComponentParamsViewString)
                {
                    CComponentParamsViewString itemStr = item as CComponentParamsViewString;
                    if (string.IsNullOrEmpty(itemStr.Value)) return;
                    float item_val = 0;
                    if (!float.TryParse(itemStr.Value.Replace(",", "."), NumberStyles.AllowDecimalPoint, nfi, out item_val)) return;

                    if (item.Name.Equals(CParamsResources.CrscDepthS.Name)) arrangementTemp.FCrscRafterDepth = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.CrscWebStraightDepthS.Name)) arrangementTemp.FCrscWebStraightDepth = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.CrscWebMiddleStiffenerSizeS.Name)) arrangementTemp.FStiffenerSize = item_val / fLengthUnitFactor;

                    // Circle screws
                    // Changed number of circles
                    if (item.Name.Equals(CParamsResources.NumberOfCirclesInGroupS.Name))
                    {
                        int numberOfCirclesInGroup = int.Parse(itemStr.Value);
                        arrangementTemp.NumberOfCirclesInGroup_Updated(numberOfCirclesInGroup);

                        //listScrewArrangementProperties = GetScrewArrangementProperties(arrangementTemp);
                        //vm.SetScrewArrangementProperties(arrangementTemp);
                    }

                    // Changed number of screws in circle
                    if (item.Name.Contains(CParamsResources.NumberOfScrewsInCircleSequenceS.Name + " "))
                    {
                        int circleNum = int.Parse(item.Name.Substring((CParamsResources.NumberOfScrewsInCircleSequenceS.Name + " ").Length));
                        UpdateCircleSequencesNumberOfScrews(circleNum, itemStr, ref arrangementTemp);
                    }
                    // Changed Radius
                    if (item.Name.Contains(CParamsResources.RadiusOfScrewsInCircleSequenceS.Name + " "))
                    {
                        int circleNum = int.Parse(item.Name.Substring((CParamsResources.RadiusOfScrewsInCircleSequenceS.Name + " ").Length));
                        UpdateCircleSequencesRadius(circleNum, fLengthUnitFactor, itemStr, ref arrangementTemp);
                    }

                    // Corner screws
                    if (item.Name.Equals(CParamsResources.PositionOfCornerSequence_xS.Name)) arrangementTemp.FPositionOfCornerSequence_x = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.PositionOfCornerSequence_yS.Name)) arrangementTemp.FPositionOfCornerSequence_y = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.DistanceOfAdditionalScrewsInxS.Name)) arrangementTemp.FAdditionalScrewsDistance_x = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.DistanceOfAdditionalScrewsInyS.Name)) arrangementTemp.FAdditionalScrewsDistance_y = item_val / fLengthUnitFactor;
                }
                else if (item is CComponentParamsViewBool)
                {
                    CComponentParamsViewBool itemBool = item as CComponentParamsViewBool;
                    if (item.Name.Equals(CParamsResources.UseAdditionalCornerScrewsS.Name))
                    {
                        arrangementTemp.BUseAdditionalCornerScrews = itemBool.Value;
                    }
                }
                else if (item is CComponentParamsViewList)
                {
                    CComponentParamsViewList itemList = item as CComponentParamsViewList;
                    if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                    if (item.Name.Equals(CParamsResources.NumberOfAdditionalScrewsInCornerS.Name)) arrangementTemp.IAdditionalConnectorInCornerNumber = int.Parse(itemList.Value);
                }

                arrangementTemp.UpdateArrangmentData();         // Update data of screw arrangement
                plate.ScrewArrangement = arrangementTemp;       // Set current screw arrangement to the plate
            }
            else if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangementRectApexOrKnee)
            {
                CScrewArrangementRectApexOrKnee arrangementTemp = (CScrewArrangementRectApexOrKnee)plate.ScrewArrangement;

                if (item is CComponentParamsViewString)
                {
                    CComponentParamsViewString itemStr = item as CComponentParamsViewString;
                    if (string.IsNullOrEmpty(itemStr.Value)) return;
                    float item_val = 0;
                    if (!float.TryParse(itemStr.Value.Replace(",", "."), NumberStyles.AllowDecimalPoint, nfi, out item_val)) return;

                    if (item.Name.Equals(CParamsResources.CrscDepthS.Name)) arrangementTemp.FCrscRafterDepth = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.CrscWebStraightDepthS.Name)) arrangementTemp.FCrscWebStraightDepth = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.CrscWebMiddleStiffenerSizeS.Name)) arrangementTemp.FStiffenerSize = item_val / fLengthUnitFactor;

                    // TODO - Ondrej, TODO No. 105
                    // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
                    // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
                    // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                    if (item.Name == "Number of screws in row SQ1") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ1 = int.Parse(itemStr.Value);
                    if (item.Name == "Number of screws in column SQ1") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ1 = int.Parse(itemStr.Value);
                    if (item.Name == "Inserting point coordinate x SQ1") arrangementTemp.fx_c_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ1") arrangementTemp.fy_c_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ1") arrangementTemp.fDistanceOfPointsX_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ1") arrangementTemp.fDistanceOfPointsY_SQ1 = item_val / fLengthUnitFactor;

                    if (item.Name == "Number of screws in row SQ2") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ2 = int.Parse(itemStr.Value);
                    if (item.Name == "Number of screws in column SQ2") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ2 = int.Parse(itemStr.Value);
                    if (item.Name == "Inserting point coordinate x SQ2") arrangementTemp.fx_c_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ2") arrangementTemp.fy_c_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ2") arrangementTemp.fDistanceOfPointsX_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ2") arrangementTemp.fDistanceOfPointsY_SQ2 = item_val / fLengthUnitFactor;

                    if (item.Name == "Number of screws in row SQ3") { int num = 0; int.TryParse(itemStr.Value, out num); if (num > 0) arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ3 = num; else itemStr.Value = arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ3.ToString(); }
                    if (item.Name == "Number of screws in column SQ3") { int num = 0; int.TryParse(itemStr.Value, out num); if (num > 0) arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ3 = num; else itemStr.Value = arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ3.ToString(); }
                    if (item.Name == "Inserting point coordinate x SQ3") arrangementTemp.fx_c_SQ3 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ3") arrangementTemp.fy_c_SQ3 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ3") arrangementTemp.fDistanceOfPointsX_SQ3 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ3") arrangementTemp.fDistanceOfPointsY_SQ3 = item_val / fLengthUnitFactor;

                    if (item.Name == "Number of screws in row SQ4") { int num = 0; int.TryParse(itemStr.Value, out num); if (num > 0) arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ4 = num; else itemStr.Value = arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ4.ToString(); }
                    if (item.Name == "Number of screws in column SQ4") { int num = 0; int.TryParse(itemStr.Value, out num); if (num > 0) arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ4 = num; else itemStr.Value = arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ4.ToString(); }
                    if (item.Name == "Inserting point coordinate x SQ4") arrangementTemp.fx_c_SQ4 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ4") arrangementTemp.fy_c_SQ4 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ4") arrangementTemp.fDistanceOfPointsX_SQ4 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ4") arrangementTemp.fDistanceOfPointsY_SQ4 = item_val / fLengthUnitFactor;
                }
                else if (item is CComponentParamsViewList)
                {
                    CComponentParamsViewList itemList = item as CComponentParamsViewList;
                    if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                }

                arrangementTemp.UpdateArrangmentData();        // Update data of screw arrangement
                plate.ScrewArrangement = arrangementTemp;      // Set current screw arrangement to the plate
            }
            else if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangement_BX_1)
            {
                CScrewArrangement_BX_1 arrangementTemp = (CScrewArrangement_BX_1)plate.ScrewArrangement;

                if (item is CComponentParamsViewString)
                {
                    CComponentParamsViewString itemStr = item as CComponentParamsViewString;
                    if (string.IsNullOrEmpty(itemStr.Value)) return;
                    float item_val = 0;
                    if (!float.TryParse(itemStr.Value.Replace(",", "."), NumberStyles.AllowDecimalPoint, nfi, out item_val)) return;

                    if (item.Name.Equals(CParamsResources.CrscDepthS.Name)) arrangementTemp.FCrscColumnDepth = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.CrscWebStraightDepthS.Name)) arrangementTemp.FCrscWebStraightDepth = item_val / fLengthUnitFactor;
                    if (item.Name.Equals(CParamsResources.CrscWebMiddleStiffenerSizeS.Name)) arrangementTemp.FStiffenerSize = item_val / fLengthUnitFactor;

                    // TODO - Ondrej, TODO No. 105
                    // Toto by sme mali zobecnit, pridat parametre pre pocet groups (default 2) pocet sekvencii v kazdej group (default 2) a moznost menit ich (podobne ako pri circle arrangement - circle number)
                    // Groups pridane navyse voci defaultu by mali pocet skrutiek 0 a vsetky parametre 0, nie generovane ako circle
                    // Pred spustenim generovania drilling route by sa mohlo skontrolovat ci nie su niektore zo skrutiek v poli HolesCenter2D identicke

                    if (item.Name == "Number of screws in row SQ1") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ1 = int.Parse(itemStr.Value);
                    if (item.Name == "Number of screws in column SQ1") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ1 = int.Parse(itemStr.Value);
                    if (item.Name == "Inserting point coordinate x SQ1") arrangementTemp.fx_c_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ1") arrangementTemp.fy_c_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ1") arrangementTemp.fDistanceOfPointsX_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ1") arrangementTemp.fDistanceOfPointsY_SQ1 = item_val / fLengthUnitFactor;

                    if (item.Name == "Number of screws in row SQ2") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ2 = int.Parse(itemStr.Value);
                    if (item.Name == "Number of screws in column SQ2") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ2 = int.Parse(itemStr.Value);
                    if (item.Name == "Inserting point coordinate x SQ2") arrangementTemp.fx_c_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ2") arrangementTemp.fy_c_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ2") arrangementTemp.fDistanceOfPointsX_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ2") arrangementTemp.fDistanceOfPointsY_SQ2 = item_val / fLengthUnitFactor;

                    if (item.Name == "Number of screws in row SQ3") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ3 = int.Parse(itemStr.Value);
                    if (item.Name == "Number of screws in column SQ3") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ3 = int.Parse(itemStr.Value);
                    if (item.Name == "Inserting point coordinate x SQ3") arrangementTemp.fx_c_SQ3 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ3") arrangementTemp.fy_c_SQ3 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ3") arrangementTemp.fDistanceOfPointsX_SQ3 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ3") arrangementTemp.fDistanceOfPointsY_SQ3 = item_val / fLengthUnitFactor;

                    if (item.Name == "Number of screws in row SQ4") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ4 = int.Parse(itemStr.Value);
                    if (item.Name == "Number of screws in column SQ4") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ4 = int.Parse(itemStr.Value);
                    if (item.Name == "Inserting point coordinate x SQ4") arrangementTemp.fx_c_SQ4 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ4") arrangementTemp.fy_c_SQ4 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ4") arrangementTemp.fDistanceOfPointsX_SQ4 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ4") arrangementTemp.fDistanceOfPointsY_SQ4 = item_val / fLengthUnitFactor;
                }
                else if (item is CComponentParamsViewList)
                {
                    CComponentParamsViewList itemList = item as CComponentParamsViewList;
                    if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                }

                arrangementTemp.UpdateArrangmentData();        // Update data of screw arrangement
                plate.ScrewArrangement = arrangementTemp;      // Set current screw arrangement to the plate
            }
            else if (plate.ScrewArrangement != null && plate.ScrewArrangement is CScrewArrangement_O)
            {
                CScrewArrangement_O arrangementTemp = (CScrewArrangement_O)plate.ScrewArrangement;

                if (item is CComponentParamsViewString)
                {
                    CComponentParamsViewString itemStr = item as CComponentParamsViewString;
                    if (string.IsNullOrEmpty(itemStr.Value)) return;
                    float item_val = 0;
                    if (!float.TryParse(itemStr.Value.Replace(",", "."), NumberStyles.AllowDecimalPoint, nfi, out item_val)) return;

                    if (item.Name == "Number of screws in row SQ1") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ1 = int.Parse(itemStr.Value);
                    if (item.Name == "Number of screws in column SQ1") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ1 = int.Parse(itemStr.Value);
                    if (item.Name == "Inserting point coordinate x SQ1") arrangementTemp.fx_c_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ1") arrangementTemp.fy_c_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ1") arrangementTemp.fDistanceOfPointsX_SQ1 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ1") arrangementTemp.fDistanceOfPointsY_SQ1 = item_val / fLengthUnitFactor;

                    if (item.Name == "Number of screws in row SQ2") arrangementTemp.iNumberOfScrewsInRow_xDirection_SQ2 = int.Parse(itemStr.Value);
                    if (item.Name == "Number of screws in column SQ2") arrangementTemp.iNumberOfScrewsInColumn_yDirection_SQ2 = int.Parse(itemStr.Value);
                    if (item.Name == "Inserting point coordinate x SQ2") arrangementTemp.fx_c_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Inserting point coordinate y SQ2") arrangementTemp.fy_c_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws x SQ2") arrangementTemp.fDistanceOfPointsX_SQ2 = item_val / fLengthUnitFactor;
                    if (item.Name == "Distance between screws y SQ2") arrangementTemp.fDistanceOfPointsY_SQ2 = item_val / fLengthUnitFactor;
                }
                else if (item is CComponentParamsViewList)
                {
                    CComponentParamsViewList itemList = item as CComponentParamsViewList;
                    if (item.Name.Equals(CParamsResources.ScrewGaugeS.Name)) arrangementTemp.referenceScrew.Gauge = int.Parse(itemList.Value);
                }

                arrangementTemp.UpdateArrangmentData();        // Update data of screw arrangement
                plate.ScrewArrangement = arrangementTemp;      // Set current screw arrangement to the plate
            }
            else
            {
                // Screw arrangement is not implemented
            }
        }

        public static void UpdateCircleSequencesNumberOfScrews(int iCircleNumberInGroup, CComponentParamsViewString itemNewValueString, ref CScrewArrangementCircleApexOrKnee arrangementTemp)
        {
            int numberOfScrews = int.Parse(itemNewValueString.Value);
            if (numberOfScrews < 2) return; // Validacia - pocet skrutiek v kruhu musi byt min 2, inak ignorovat

            // Change each group
            foreach (CScrewSequenceGroup gr in arrangementTemp.ListOfSequenceGroups)
            {
                IEnumerable<CConnectorSequence> halfCircleSequences = (IEnumerable<CConnectorSequence>)gr.ListSequence.Where(s => s is CScrewHalfCircleSequence);
                CConnectorSequence seq = null;
                seq = halfCircleSequences.ElementAtOrDefault((iCircleNumberInGroup - 1) * 2); //1.half of circle
                if (seq != null) seq.INumberOfConnectors = numberOfScrews;
                seq = halfCircleSequences.ElementAtOrDefault((iCircleNumberInGroup - 1) * 2 + 1); //2.half of circle
                if (seq != null) seq.INumberOfConnectors = numberOfScrews;
            }
            // Recalculate total number of screws in the arrangement
            arrangementTemp.RecalculateTotalNumberOfScrews();
        }

        public static void UpdateCircleSequencesRadius(int iCircleNumberInGroup, float fLengthUnitFactor, CComponentParamsViewString itemNewValueString, ref CScrewArrangementCircleApexOrKnee arrangementTemp)
        {
            float radius = (float.Parse(itemNewValueString.Value) / fLengthUnitFactor);
            if (!IsValidCircleRadius(radius, arrangementTemp)) throw new Exception("Radius is not valid.");  //if radius is not valid => return
            // Change each group
            foreach (CScrewSequenceGroup gr in arrangementTemp.ListOfSequenceGroups)
            {
                IEnumerable<CConnectorSequence> halfCircleSequences = (IEnumerable<CConnectorSequence>)gr.ListSequence.Where(s => s is CScrewHalfCircleSequence); // Bug - To Ondrej tu je nejaka chyba v pretypovani, ja som to kedysi menil, aby mi to slo prelozit ale ... :)
                CConnectorSequence seq = null;
                seq = halfCircleSequences.ElementAtOrDefault((iCircleNumberInGroup - 1) * 2); //1.half of circle
                if (seq != null) ((CScrewHalfCircleSequence)seq).Radius = radius;
                seq = halfCircleSequences.ElementAtOrDefault((iCircleNumberInGroup - 1) * 2 + 1); //2.half of circle
                if (seq != null) ((CScrewHalfCircleSequence)seq).Radius = radius;
            }
        }

        private static bool IsValidCircleRadius(float radius, CScrewArrangementCircleApexOrKnee arrangementTemp)
        {
            float fAdditionalMargin = 0.02f; // TODO - napojit na GUI, napojit na generovanie screw arrangement - vid Circle Arrangement Get_ScrewGroup_IncludingAdditionalScrews
            if (radius > 0.5 * arrangementTemp.FStiffenerSize + fAdditionalMargin) return true;
            else return false;
        }


        public static void DataGridGeometryParams_ValueChanged(CComponentParamsView item, CPlate plate)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            float fLengthUnitFactor = 1000; // GUI input in mm, change to m used in source code
            float fDegToRadianFactor = 180f / MathF.fPI;

            bool bUseRoofSlope = true;

            CComponentParamsViewString itemStr = item as CComponentParamsViewString;
            if (itemStr == null) return;
            float item_val = 0;
            if (!float.TryParse(itemStr.Value.Replace(",", "."), NumberStyles.AllowDecimalPoint, nfi, out item_val)) return;

            // Set current basic geometry of plate
            if (plate is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic plateTemp = (CConCom_Plate_B_basic)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fb_X = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeightS.Name)) plateTemp.Fh_Y = item_val / fLengthUnitFactor;

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else if (plate is CConCom_Plate_JA)
            {
                CConCom_Plate_JA plateTemp = (CConCom_Plate_JA)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fb_X = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = item_val / fLengthUnitFactor;

                if (bUseRoofSlope)
                {
                    if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = item_val / fDegToRadianFactor;
                }
                else
                {
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = item_val / fLengthUnitFactor;
                }

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else if (plate is CConCom_Plate_JB || plate is CConCom_Plate_JBS)
            {
                CConCom_Plate_JB plateTemp = (CConCom_Plate_JB)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidthS.Name)) plateTemp.Fb_X = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = item_val / fLengthUnitFactor;

                if (bUseRoofSlope)
                {
                    if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = item_val / fDegToRadianFactor;
                }
                else
                {
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = item_val / fLengthUnitFactor;
                }

                if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = item_val / fLengthUnitFactor;

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else if (plate is CConCom_Plate_KA)
            {
                CConCom_Plate_KA plateTemp = (CConCom_Plate_KA)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = item_val / fLengthUnitFactor;

                if (bUseRoofSlope)
                {
                    if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = item_val / fDegToRadianFactor;
                }
                else
                {
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = item_val / fLengthUnitFactor;
                }

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else if (plate is CConCom_Plate_KB || plate is CConCom_Plate_KBS)
            {
                CConCom_Plate_KB plateTemp = (CConCom_Plate_KB)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = item_val / fLengthUnitFactor;

                if (bUseRoofSlope)
                {
                    if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = item_val / fDegToRadianFactor;
                }
                else
                {
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = item_val / fLengthUnitFactor;
                }

                if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = item_val / fLengthUnitFactor;

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else if (plate is CConCom_Plate_KC || plate is CConCom_Plate_KCS)
            {
                CConCom_Plate_KC plateTemp = (CConCom_Plate_KC)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = item_val / fLengthUnitFactor;

                if (bUseRoofSlope)
                {
                    if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = item_val / fDegToRadianFactor;
                }
                else
                {
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = item_val / fLengthUnitFactor;
                }

                if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = item_val / fLengthUnitFactor;

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else if (plate is CConCom_Plate_KD || plate is CConCom_Plate_KDS)
            {
                CConCom_Plate_KD plateTemp = (CConCom_Plate_KD)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = item_val / fLengthUnitFactor;

                if (bUseRoofSlope)
                {
                    if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = item_val / fDegToRadianFactor;
                }
                else
                {
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = item_val / fLengthUnitFactor;
                }

                if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = item_val / fLengthUnitFactor;

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else if (plate is CConCom_Plate_KE) // Nepouzivat, kym nebude zobecnene screw arrangement
            {
                CConCom_Plate_KE plateTemp = (CConCom_Plate_KE)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth2S.Name)) plateTemp.Fb_X2 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = item_val / fLengthUnitFactor;

                if (bUseRoofSlope)
                {
                    if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = item_val / fDegToRadianFactor;
                }
                else
                {
                    if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = item_val / fLengthUnitFactor;
                }

                if (item.Name.Equals(CParamsResources.PlateLipS.Name)) plateTemp.Fl_Z = item_val / fLengthUnitFactor;

                if (item.Name.Equals(CParamsResources.RafterWidthS.Name)) plateTemp.Fb_XR = item_val / fLengthUnitFactor;

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else if (plate is CConCom_Plate_O)
            {
                CConCom_Plate_O plateTemp = (CConCom_Plate_O)plate;

                if (item.Name.Equals(CParamsResources.PlateThicknessS.Name)) plateTemp.Ft = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateWidth1S.Name)) plateTemp.Fb_X1 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.RafterWidthS.Name)) plateTemp.Fb_X2 = item_val / fLengthUnitFactor; // Oznacene ako BR ale premenna je bX2
                if (item.Name.Equals(CParamsResources.PlateHeight1S.Name)) plateTemp.Fh_Y1 = item_val / fLengthUnitFactor;
                if (item.Name.Equals(CParamsResources.PlateHeight2S.Name)) plateTemp.Fh_Y2 = item_val / fLengthUnitFactor;

                if (item.Name.Equals(CParamsResources.RoofSlopeS.Name)) plateTemp.FSlope_rad = item_val / fDegToRadianFactor;

                // Update plate data
                plateTemp.UpdatePlateData(plateTemp.ScrewArrangement);
                plate = plateTemp;
            }
            else
            {
                // Plate is not implemented
            }
        }

        public static List<CComponentParamsView> GetComponentProperties(CPlate plate)
        {
            float fUnitFactor_Length = 1000;
            int iNumberOfDecimalPlaces_Length = 1;
            float fUnitFactor_Rotation = 180f / MathF.fPI; // Factor from radians to degrees
            int iNumberOfDecimalPlaces_Rotation = 1;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            bool bUseRoofSlope = true;

            List<CComponentParamsView> geometry = new List<CComponentParamsView>();

            geometry.Add(new CComponentParamsViewString(CParamsResources.PlateNameS.Name, "", plate.Name, ""));
            geometry.Add(new CComponentParamsViewString(CParamsResources.PlateThicknessS.Name, CParamsResources.PlateThicknessS.Symbol, (Math.Round(plate.Ft * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateThicknessS.Unit));

            if (plate is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic plateTemp = (CConCom_Plate_B_basic)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidthS.Name, CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fb_X * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeightS.Name, CParamsResources.PlateHeightS.Symbol, (Math.Round(plateTemp.Fh_Y * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeightS.Unit));
            }
            else if (plate is CConCom_Plate_JA)
            {
                CConCom_Plate_JA plateTemp = (CConCom_Plate_JA)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidthS.Name, CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fb_X * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit, false));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }
            }
            else if (plate is CConCom_Plate_JB || plate is CConCom_Plate_JBS)
            {
                CConCom_Plate_JB plateTemp = (CConCom_Plate_JB)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidthS.Name, CParamsResources.PlateWidthS.Symbol, (Math.Round(plateTemp.Fb_X * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit, false));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KA)
            {
                CConCom_Plate_KA plateTemp = (CConCom_Plate_KA)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit, false));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }
            }
            else if (plate is CConCom_Plate_KB || plate is CConCom_Plate_KBS)
            {
                CConCom_Plate_KB plateTemp = (CConCom_Plate_KB)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit, false));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KC || plate is CConCom_Plate_KCS)
            {
                CConCom_Plate_KC plateTemp = (CConCom_Plate_KC)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit, false));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KD)
            {
                CConCom_Plate_KD plateTemp = (CConCom_Plate_KD)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit, false));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
            }
            else if (plate is CConCom_Plate_KE)
            {
                CConCom_Plate_KE plateTemp = (CConCom_Plate_KE)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth2S.Name, CParamsResources.PlateWidth2S.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));

                if (bUseRoofSlope)
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit, false));
                }
                else
                {
                    geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                }

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateLipS.Name, CParamsResources.PlateLipS.Symbol, (Math.Round(plateTemp.Fl_Z * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateLipS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.RafterWidthS.Name, CParamsResources.RafterWidthS.Symbol, (Math.Round(plateTemp.Fb_XR * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.RafterWidthS.Unit));
            }
            else if (plate is CConCom_Plate_O)
            {
                CConCom_Plate_O plateTemp = (CConCom_Plate_O)plate;

                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateWidth1S.Name, CParamsResources.PlateWidth1S.Symbol, (Math.Round(plateTemp.Fb_X1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateWidth1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.RafterWidthS.Name, CParamsResources.RafterWidthS.Symbol, (Math.Round(plateTemp.Fb_X2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.RafterWidthS.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight1S.Name, CParamsResources.PlateHeight1S.Symbol, (Math.Round(plateTemp.Fh_Y1 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight1S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.PlateHeight2S.Name, CParamsResources.PlateHeight2S.Symbol, (Math.Round(plateTemp.Fh_Y2 * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlateHeight2S.Unit));
                geometry.Add(new CComponentParamsViewString(CParamsResources.RoofSlopeS.Name, CParamsResources.RoofSlopeS.Symbol, (Math.Round(plateTemp.FSlope_rad * fUnitFactor_Rotation, iNumberOfDecimalPlaces_Rotation)).ToString(nfi), CParamsResources.RoofSlopeS.Unit, false));
            }
            else
            {
                // Plate is not implemented
            }

            return geometry;
        }

        public static List<CComponentParamsView> GetComponentDetails(CPlate plate)
        {
            float fUnitFactor_Length = 1000;
            int iNumberOfDecimalPlaces_Length = 1;
            float fUnitFactor_Area = 1000000;//fUnitFactor_Length * fUnitFactor_Length;
            float fUnitFactor_Volume = 1000000000;//fUnitFactor_Length * fUnitFactor_Length * fUnitFactor_Length;
            int iNumberOfDecimalPlaces_Area = 1;
            int iNumberOfDecimalPlaces_Volume = 1;
            int iNumberOfDecimalPlaces_Mass = 1;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            List<CComponentParamsView> details = new List<CComponentParamsView>();
            details.Add(new CComponentParamsViewString(CParamsResources.PlatePerimeterS.Name, CParamsResources.PlatePerimeterS.Symbol, (Math.Round(plate.fCuttingRouteDistance * fUnitFactor_Length, iNumberOfDecimalPlaces_Length)).ToString(nfi), CParamsResources.PlatePerimeterS.Unit));
            details.Add(new CComponentParamsViewString(CParamsResources.PlateSurfaceS.Name, CParamsResources.PlateSurfaceS.Symbol, (Math.Round(plate.fSurface * fUnitFactor_Area, iNumberOfDecimalPlaces_Area)).ToString(nfi), CParamsResources.PlateSurfaceS.Unit));
            details.Add(new CComponentParamsViewString(CParamsResources.PlateAreaS.Name, CParamsResources.PlateAreaS.Symbol, (Math.Round(plate.fArea * fUnitFactor_Area, iNumberOfDecimalPlaces_Area)).ToString(nfi), CParamsResources.PlateAreaS.Unit));
            details.Add(new CComponentParamsViewString(CParamsResources.PlateVolumeS.Name, CParamsResources.PlateVolumeS.Symbol, (Math.Round(plate.fVolume * fUnitFactor_Volume, iNumberOfDecimalPlaces_Volume)).ToString(nfi), CParamsResources.PlateVolumeS.Unit));
            details.Add(new CComponentParamsViewString(CParamsResources.PlateMassS.Name, CParamsResources.PlateMassS.Symbol, Math.Round(plate.fMass, iNumberOfDecimalPlaces_Mass).ToString(nfi), CParamsResources.PlateMassS.Unit));

            return details;
        }

        public static void ScrewArrangementChanged(CConnectionJointTypes joint, CPlate plate, int screwArrangementIndex)
        {
            int iNumberofHoles = 0;

            CAnchor referenceAnchor = new CAnchor("M16", "8.8", 0.33f, true);
            CScrew referenceScrew = new CScrew("TEK", "14");

            // Default values
            // Member cross-section parameters
            // Base Plate
            float fColumnDepth = 0.63f;
            float fColumnWebStraightDepth = fColumnDepth - 2 * 0.025f - 2 * 0.002f;
            float fColumnWebSiffenerSize_1 = 0.18f;

            // Apex, knee plate
            float fRafterDepth = 0.63f;
            float fRafterWebStraightDepth = fColumnDepth - 2 * 0.025f - 2 * 0.002f;
            float fRafterWebSiffenerSize_1 = 0.18f;

            // Circle arrangement
            bool bUseAdditionalConnectors = true;
            int iNumberOfAdditionalConnectorsInCorner = 4;
            int iConnectorNumberInCircleSequence = 20;
            float fConnectorRadiusInCircleSequence = 0.25f;
            float fAdditionalConnectorDistance = 0.03f;

            // V pripade ze plate je priradena spoju, mozeme ako default pouzit parametre urcene podla members definovanych v spoji
            if (joint != null)
            {
                // Base plate, knee joint - column is main member
                fColumnDepth = (float)joint.m_MainMember.CrScStart.h;
                fColumnWebStraightDepth = fColumnDepth - 2 * 0.025f - 2 * (float)joint.m_MainMember.CrScStart.t_min; // TODO - nacitat z databazy
                fColumnWebSiffenerSize_1 = 0.18f; // TODO dopracovat do databazy, zohladnit pocet vyztuh, asymetricka poloha

                if (joint.m_SecondaryMembers != null) // Apex, knee, ... for knee joint - column is main member and rafter is secondary member
                {
                    fRafterDepth = (float)joint.m_SecondaryMembers[0].CrScStart.h;
                    fRafterWebStraightDepth = fColumnDepth - 2 * 0.025f - 2 * (float)joint.m_SecondaryMembers[0].CrScStart.t_min; // TODO - nacitat z databazy
                    fRafterWebSiffenerSize_1 = 0.2857142f * (float)joint.m_SecondaryMembers[0].CrScStart.h; // TODO dopracovat do databazy, zohladnit pocet vyztuh, asymetricka poloha

                    // Recalculate default radius and number of screws depending on cross-section depth
                    float fMinimumDistanceBetweenScrews = 0.02f;
                    fAdditionalConnectorDistance = Math.Max(fMinimumDistanceBetweenScrews, 0.05f * fRafterWebStraightDepth);
                    fConnectorRadiusInCircleSequence = 0.45f * fRafterWebStraightDepth; // TODO - dynamicky podla velkosti prierezu
                    float fDistanceBetweenScrewsInCircle = 0.05f;
                    iConnectorNumberInCircleSequence = (int)((2f * MathF.fPI * fConnectorRadiusInCircleSequence) / fDistanceBetweenScrewsInCircle); // 20; // TODO - dynamicky podla velkosti prierezu
                }
            }

            CScrewArrangement_BX_1 screwArrangement_BX_01 = new CScrewArrangement_BX_1(referenceScrew, fColumnDepth, fColumnDepth - 2 * 0.025f - 2 * 0.002f, 0.18f,
                3, 5, 0.05f, 0.029f, 0.05f, 0.05f,
                3, 5, 0.05f, 0.401f, 0.05f, 0.05f);
            CScrewArrangement_BX_2 screwArrangement_BX_02 = new CScrewArrangement_BX_2(referenceScrew, fColumnDepth, fColumnDepth - 2 * 0.008f - 2 * 0.002f, 0.058f,
                3, 1, 0.04f, 0.03f, 0.05f, 0.05f,
                3, 1, 0.04f, 0.14f, 0.05f, 0.05f,
                3, 1, 0.04f, 0.26f, 0.05f, 0.05f);
            CScrewArrangement_L screwArrangement_L = new CScrewArrangement_L(iNumberofHoles, referenceScrew);
            CScrewArrangement_F screwArrangement_F = new CScrewArrangement_F(iNumberofHoles, referenceScrew);
            CScrewArrangement_LL screwArrangement_LL = new CScrewArrangement_LL(iNumberofHoles, referenceScrew);
            CScrewArrangement_O screwArrangement_O = new CScrewArrangement_O(referenceScrew, 1, 10, 0.02f, 0.02f, 0.05f, 0.05f, 1, 10, 0.18f, 0.02f, 0.05f, 0.05f);

            List<CScrewSequenceGroup> screwSeqGroups = new List<CScrewSequenceGroup>();
            CScrewSequenceGroup gr1 = new CScrewSequenceGroup();
            gr1.NumberOfHalfCircleSequences = 2;
            gr1.NumberOfRectangularSequences = 4;
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr1);
            CScrewSequenceGroup gr2 = new CScrewSequenceGroup();
            gr2.NumberOfHalfCircleSequences = 2;
            gr2.NumberOfRectangularSequences = 4;
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr2);

            CScrewArrangementCircleApexOrKnee screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, fRafterDepth, fRafterWebStraightDepth, fRafterWebSiffenerSize_1, 1, screwSeqGroups, bUseAdditionalConnectors, fConnectorRadiusInCircleSequence, fConnectorRadiusInCircleSequence, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
            CScrewArrangementRectApexOrKnee screwArrangementRectangleApex = new CScrewArrangementRectApexOrKnee(referenceScrew, fRafterDepth, fRafterWebStraightDepth, fRafterWebSiffenerSize_1, 10, 2, 0.05f, 0.05f, 0.07f, 0.05f, 8, 2, 0.15f, 0.55f, 0.075f, 0.05f);
            //CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 10, 2);
            CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, fRafterDepth, fRafterWebStraightDepth, fRafterWebSiffenerSize_1, 12, 2, 0.040f, 0.047f, 0.050f, 0.158f, 12, 2, 0.040f, 0.425f, 0.050f, 0.158f, 12, 2, 0.05f, 0.047f, 0.05f, 0.158f, 14, 2, 0.05f, 0.425f, 0.05f, 0.158f);

            switch (plate.m_ePlateSerieType_FS)
            {
                case ESerieTypePlate.eSerie_B:
                    {
                        if (plate is CConCom_Plate_B_basic)
                        {
                            if (screwArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else
                                plate.ScrewArrangement = screwArrangement_BX_02;
                        }
                        break;
                    }
                case ESerieTypePlate.eSerie_J:
                    {
                        if (plate is CConCom_Plate_JA) // JA
                        {
                            if (screwArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (screwArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleApex;
                            else // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else // JB
                        {
                            if (screwArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (screwArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleApex;
                            else // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_K:
                    {
                        if (plate is CConCom_Plate_KA) // KA
                        {
                            if (screwArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (screwArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else if (plate is CConCom_Plate_KB) // KB
                        {
                            if (screwArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (screwArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(screwArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;

                        }
                        else if (plate is CConCom_Plate_KC) // KC
                        {
                            if (screwArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (screwArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(screwArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else if (plate is CConCom_Plate_KD) // KD
                        {
                            if (screwArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (screwArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(screwArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        else // KE - TODO - screws are not implemented !!!
                        {
                            if (screwArrangementIndex == 0) // Undefined
                                plate.ScrewArrangement = null;
                            else if (screwArrangementIndex == 1) // Rectangular
                                plate.ScrewArrangement = screwArrangementRectangleKnee;
                            else//(screwArrangementIndex == 2) // Circle
                                plate.ScrewArrangement = screwArrangementCircle;
                        }
                        break;
                    }
                case ESerieTypePlate.eSerie_O:
                    {
                        if (screwArrangementIndex == 0) // Undefined
                            plate.ScrewArrangement = null;
                        else if (screwArrangementIndex == 1) // Rectangular - Plate O
                            plate.ScrewArrangement = screwArrangement_O;

                        break;
                    }
                default:
                    {
                        // Not implemented
                        break;
                    }
            }
        }

        public static string[] GetPlateScrewArangementTypes(CPlate plate)
        {
            CDatabaseComponents dc = new CDatabaseComponents();
            switch (plate.m_ePlateSerieType_FS)
            {
                case ESerieTypePlate.eSerie_B:
                    {
                        return dc.arr_Serie_B_ScrewArrangement_Names;
                    }
                case ESerieTypePlate.eSerie_L:
                    {
                        return dc.arr_Serie_L_ScrewArrangement_Names;
                    }
                case ESerieTypePlate.eSerie_LL:
                    {
                        return dc.arr_Serie_LL_ScrewArrangement_Names;
                    }
                case ESerieTypePlate.eSerie_F:
                    {
                        return dc.arr_Serie_F_ScrewArrangement_Names;
                    }
                case ESerieTypePlate.eSerie_Q:
                    {
                        return new string[] { "Undefined" };
                    }
                case ESerieTypePlate.eSerie_S:
                    {
                        return new string[] { "Undefined" };
                    }
                case ESerieTypePlate.eSerie_T:
                    {
                        return new string[] { "Undefined" };
                    }
                case ESerieTypePlate.eSerie_X:
                    {
                        return new string[] { "Undefined" };
                    }
                case ESerieTypePlate.eSerie_Y:
                    {
                        return new string[] { "Undefined" };
                    }
                case ESerieTypePlate.eSerie_J:
                    {
                        return dc.arr_Serie_J_ScrewArrangement_Names;
                        //ScrewArrangementIndex = 2; 
                    }
                case ESerieTypePlate.eSerie_K:
                    {
                        return dc.arr_Serie_K_ScrewArrangement_Names;
                        //ScrewArrangementIndex = 2;
                    }
                case ESerieTypePlate.eSerie_N:
                    {
                        return new string[] { "Undefined" };
                    }
                case ESerieTypePlate.eSerie_O:
                    {
                        return dc.arr_Serie_O_ScrewArrangement_Names;
                    }
                default:
                    {
                        // Not implemented
                        return new string[] { "" };
                    }
            }
        }

        public static int GetPlateScrewArangementIndex(CPlate plate)
        {
            CDatabaseComponents dc = new CDatabaseComponents();
            switch (plate.m_ePlateSerieType_FS)
            {
                case ESerieTypePlate.eSerie_B:
                    {
                        if (plate.ScrewArrangement == null) return 0;
                        else if (plate.ScrewArrangement is CScrewArrangement_BX_1) return 1;
                        else if (plate.ScrewArrangement is CScrewArrangement_BX_2) return 2;
                        else return 0;

                        //??? mato
                        //dc.arr_Serie_B_ScrewArrangement_Names;
                        //nenasiel som odpovedajuce triedy k arr_Serie_B_ScrewArrangement_Names
                        //je potrebne nieco na zaklade coho povieme aky index to je podla toho aky screw arrangement ma Plate
                    }
                case ESerieTypePlate.eSerie_L:
                    {
                        if (plate.ScrewArrangement == null) return 0;
                        else if (plate.ScrewArrangement is CScrewArrangement_L) return 1;
                        else return 0;
                    }
                case ESerieTypePlate.eSerie_LL:
                    {
                        if (plate.ScrewArrangement == null) return 0;
                        else if (plate.ScrewArrangement is CScrewArrangement_LL) return 1;
                        else return 0;
                    }
                case ESerieTypePlate.eSerie_F:
                    {
                        if (plate.ScrewArrangement == null) return 0;
                        else if (plate.ScrewArrangement is CScrewArrangement_F) return 1;
                        else return 0;
                    }
                case ESerieTypePlate.eSerie_Q:
                    {
                        return 0;
                    }
                case ESerieTypePlate.eSerie_S:
                    {
                        return 0;
                    }
                case ESerieTypePlate.eSerie_T:
                    {
                        return 0;
                    }
                case ESerieTypePlate.eSerie_X:
                    {
                        return 0;
                    }
                case ESerieTypePlate.eSerie_Y:
                    {
                        return 0;
                    }
                case ESerieTypePlate.eSerie_J:
                    {
                        if (plate.ScrewArrangement == null) return 0;
                        else if (plate.ScrewArrangement is CScrewArrangementRectApexOrKnee) return 1;
                        else if (plate.ScrewArrangement is CScrewArrangementCircleApexOrKnee) return 2;
                        else return 0;
                    }
                case ESerieTypePlate.eSerie_K:
                    {
                        if (plate.ScrewArrangement == null) return 0;
                        else if (plate.ScrewArrangement is CScrewArrangementRectApexOrKnee) return 1;
                        else if (plate.ScrewArrangement is CScrewArrangementCircleApexOrKnee) return 2;
                        else return 0;
                    }
                case ESerieTypePlate.eSerie_N:
                    {
                        return 0;
                    }
                case ESerieTypePlate.eSerie_O:
                    {
                        if (plate.ScrewArrangement == null) return 0;
                        else if (plate.ScrewArrangement is CScrewArrangement_O) return 1;
                        else return 0;
                    }
                default:
                    {
                        // Not implemented
                        return 0;
                    }
            }
        }

        public static void UpdatePlateScrewArrangementData(CPlate plate)
        {
            if (plate.ScrewArrangement != null) plate.ScrewArrangement.UpdateArrangmentData();

            if (plate is CConCom_Plate_B_basic) // Base plates
                plate.UpdatePlateData(plate.ScrewArrangement);
            else // other plates (without anchors)
                plate.UpdatePlateData(plate.ScrewArrangement);
        }
    }
}
