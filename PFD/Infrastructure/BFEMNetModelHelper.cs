using BaseClasses;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace PFD
{
    public static class BFEMNetModelHelper
    {
        
        public static void GetResultsList(Model bfenet_model, List<LoadCombination> loadcombinations,
            out List<List<List<basicInternalForces>>> resultsoutput,
            out List<List<List<basicDeflections>>> resultsoutput_deflections)
        {
            resultsoutput = new List<List<List<basicInternalForces>>>();

            resultsoutput_deflections = new List<List<List<basicDeflections>>>();

            // Load Cases - results
            for (int i = 0; i < loadcombinations.Count; i++) // Each load combination
            {
                // TODO - mozno by bolo lepsie nacitavat z BFENet len vysledky loadcases, ktorych je menej nez kombinacii a vysledky kombinovat az pri zobrazeni

                // Internal forces
                List<List<basicInternalForces>> resultsoutput_loadcases = new List<List<basicInternalForces>>();
                // Deflections
                List<List<basicDeflections>> resultsoutput_loadcases_def = new List<List<basicDeflections>>();


                //throw new NotImplementedException();



            }

            // Load Combinations - results
            for (int i = 0; i < loadcombinations.Count; i++) // Each load combination
            {
                // Internal forces
                List<List<basicInternalForces>> resultsoutput_combination = new List<List<basicInternalForces>>();
                // Deflections
                List<List<basicDeflections>> resultsoutput_combination_def = new List<List<basicDeflections>>();

                const int iNumberOfResultsSections = 11;
                double[] xLocations_rel = new double[iNumberOfResultsSections];

                // Fill relative coordinates (x_rel)
                for (int s = 0; s < iNumberOfResultsSections; s++)
                    xLocations_rel[s] = s * 1.0f / (iNumberOfResultsSections - 1);

                for (int j = 0; j < bfenet_model.Elements.Count; j++) // Each element in the model
                {
                    Trace.WriteLine("Element No.: " + (j + 1).ToString());
                    Trace.WriteLine("Internal forces in particular x positions");

                    double elemLength = bfenet_model.Elements[j].GetElementLength();

                    List<basicInternalForces> resultsoutput_member = new List<basicInternalForces>();

                    for (int k = 0; k < xLocations_rel.Length; k++)
                    {
                        string sMessage = "x = " + String.Format(CultureInfo.InvariantCulture, "{0:0.000}", (Math.Round(xLocations_rel[k] * elemLength, 3)));
                        var eForce = (bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]);
                        Trace.WriteLine(sMessage + "\t " + eForce);

                        basicInternalForces temp_x_results = new basicInternalForces();
                        temp_x_results.fN = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Fx;
                        temp_x_results.fV_yy = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Fy;
                        temp_x_results.fV_zz = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Fz;
                        temp_x_results.fT = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Mx;
                        temp_x_results.fM_yy = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).My;
                        temp_x_results.fM_zz = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Mz;

                        resultsoutput_member.Add(temp_x_results);
                    }

                    resultsoutput_combination.Add(resultsoutput_member);

                    // Deformations
                    // TODO - dopracovat do BFENEt aby to vracalo aspon nejake rozumne hodnoty :-)
                    // Pripadne updatovat kniznicu a pozriet sa ci taketo funkcie nepridali
                    List<basicDeflections> resultsoutput_member_deflections = new List<basicDeflections>();

                    for (int k = 0; k < xLocations_rel.Length; k++)
                    {
                        string sMessage = "x = " + String.Format(CultureInfo.InvariantCulture, "{0:0.000}", (Math.Round(xLocations_rel[k] * elemLength, 3)));
                        var eDisplacement = (bfenet_model.Elements[j] as FrameElement2Node).GetLocalDeformationAt_MC(xLocations_rel[k] * elemLength, loadcombinations[i]);
                        Trace.WriteLine(sMessage + "\t " + eDisplacement);

                        basicDeflections temp_x_results = new basicDeflections();
                        temp_x_results.fDelta_yy = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetLocalDeformationAt_MC(xLocations_rel[k] * elemLength, loadcombinations[i]).DY;
                        temp_x_results.fDelta_zz = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetLocalDeformationAt_MC(xLocations_rel[k] * elemLength, loadcombinations[i]).DZ;

                        resultsoutput_member_deflections.Add(temp_x_results);
                    }

                    resultsoutput_combination_def.Add(resultsoutput_member_deflections);
                }

                resultsoutput.Add(resultsoutput_combination);

                resultsoutput_deflections.Add(resultsoutput_combination_def);
            }
        }


        public static void DisplayResultsinConsole(Model bfenet_model, List<LoadCombination> loadcombinations, bool bWriteResultsInTXTFile)
        {
            //bool bWriteResultsInTXTFile = true; // Vypise hodnoty do suboru Results.txt na disk D (oddelene tabulatorom, je mozne vlozit do stlpcov tabulky xls)

            List<Force> outputresults = new List<Force>();

            for (int i = 0; i < loadcombinations.Count; i++) // Each load combination
            {
                // Reactions in nodes
                Trace.WriteLine("Load Combination No." + (i + 1).ToString() + " Name: " + GetLoadCombinationString(loadcombinations[i])); 
                // DONE - zapracovat vypis kluca kombinacie 1.00*LC1 + 1.50*LC2 (ak je to mozne niekde vydolovat) a pripadne zaviest do BFENet aj property "Name" pre tuto triedu "LoadCombination"
                

                // Total reactions
                // Show sum of reactions

                double fx = 0;
                double fy = 0;
                double fz = 0;
                double mx = 0;
                double my = 0;
                double mz = 0;

                for (int j = 0; j < bfenet_model.Nodes.Count; j++) // Each node in the model
                {
                    var rj = bfenet_model.Nodes[j].GetSupportReaction(loadcombinations[i]);
                    Trace.WriteLine("Total reactions SUM in Node No.: " + (j + 1).ToString() + " is: " + rj.ToString());

                    fx += rj.Fx;
                    fy += rj.Fy;
                    fz += rj.Fz;

                    mx += rj.Mx;
                    my += rj.My;
                    mz += rj.Mz;
                }

                // Total reactions
                // Show sum of reactions

                Force rtotalsum = new Force(); // Sum per all nodes

                rtotalsum.Fx = fx;
                rtotalsum.Fy = fy;
                rtotalsum.Fz = fz;
                rtotalsum.Mx = fx;
                rtotalsum.My = fy;
                rtotalsum.Mz = fz;

                Trace.WriteLine("Total reactions SUM :" + rtotalsum.ToString() + "\n");

                // Internal forces

                const int iNumberOfResultsSections = 11;
                double[] xLocations_rel = new double[iNumberOfResultsSections];

                // Fill relative coordinates (x_rel)
                for (int s = 0; s < iNumberOfResultsSections; s++)
                    xLocations_rel[s] = s * 1.0f / (iNumberOfResultsSections - 1);

                for (int j = 0; j < bfenet_model.Elements.Count; j++) // Each element in the model
                {
                    Trace.WriteLine("Element No.: " + (j + 1).ToString());
                    Trace.WriteLine("Internal forces in particular x positions");

                    double elemLength = bfenet_model.Elements[j].GetElementLength();

                    for (int k = 0; k < xLocations_rel.Length; k++)
                    {
                        string sMessage = "x = " + String.Format(CultureInfo.InvariantCulture, "{0:0.000}", (Math.Round(xLocations_rel[k] * elemLength, 3)));
                        var eForce = (bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]);
                        Trace.WriteLine(sMessage + "\t " + eForce);

                        if (bWriteResultsInTXTFile)
                            outputresults.Add(eForce);
                    }
                }
            }

            if (bWriteResultsInTXTFile)
            {
                try
                {
                    Trace.WriteLine("\nWritting results to the file D:\\Results.txt.");

                    //Pass the filepath and filename to the StreamWriter Constructor
                    StreamWriter sw = new StreamWriter("D:\\Results.txt");

                    //Write a line of text
                    sw.WriteLine("Results");

                    sw.WriteLine("Fx [N]" + "\t" +
                                 "Fy [N]" + "\t" +
                                 "Fz [N]" + "\t" +
                                 "Mx [Nm]" + "\t" +
                                 "My [Nm]" + "\t" +
                                 "Mz [Nm]");

                    for (int i = 0; i < outputresults.Count; i++)
                    {
                        //sw.WriteLine(outputresults[i]);
                        sw.WriteLine(outputresults[i].Fx + "\t" +
                            outputresults[i].Fy + "\t" +
                            outputresults[i].Fz + "\t" +
                            outputresults[i].Mx + "\t" +
                            outputresults[i].My + "\t" +
                            outputresults[i].Mz);
                    }

                    //Close the file
                    sw.Close();
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Trace.WriteLine("Executing finally block.");
                }
            }

            
            
        }

        public static double GetElementLength(this Element elm)
        {
            if (elm.Nodes.Length == 2)
                return (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            return 0.0;
        }


        public static string GetLoadCombinationString(LoadCombination lc)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            string s = null;
            foreach (KeyValuePair<LoadCase, double> kvp in lc)
            {
                if (s != null) s += " + ";
                else s = "";

                s += $"{kvp.Value.ToString("F2", nfi)}*[{kvp.Key.CaseName}]";
            }
            return s;
        }
    }
}
