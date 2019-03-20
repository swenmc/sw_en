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
        public static LoadCombinationsInternalForces GetResultsList(Model bfenet_model, List<LoadCombination> loadcombinations, bool bConsiderNodalDisplacement = false)
        {
            bool debugging = false;
            LoadCombinationsInternalForces lcIF = new LoadCombinationsInternalForces();

            // Load Combinations - results
            for (int i = 0; i < loadcombinations.Count; i++) // Each load combination
            {
                int loadCombinationID = loadcombinations[i].LcID;
                const int iNumberOfResultsSections = 11;
                double[] xLocations_rel = new double[iNumberOfResultsSections];

                // Fill relative coordinates (x_rel)
                for (int s = 0; s < iNumberOfResultsSections; s++)
                    xLocations_rel[s] = s * 1.0f / (iNumberOfResultsSections - 1);

                MembersInternalForces membersResults = new MembersInternalForces();
                for (int j = 0; j < bfenet_model.Elements.Count; j++) // Each element in the model
                {
                    if (debugging) Trace.WriteLine("Element No.: " + (j + 1).ToString());
                    if (debugging) Trace.WriteLine("Internal forces in particular x positions");

                    double elemLength = bfenet_model.Elements[j].GetElementLength();

                    MemberInternalForces mResult = new MemberInternalForces(bfenet_model.Elements[j].MID);
                    for (int k = 0; k < xLocations_rel.Length; k++)
                    {
                        if (debugging)
                        {
                            string sMessage = "x = " + String.Format(CultureInfo.InvariantCulture, "{0:0.000}", (Math.Round(xLocations_rel[k] * elemLength, 3)));
                            var eForce = (bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]);
                            Trace.WriteLine(sMessage + "\t " + eForce);
                        }

                        basicInternalForces x_resultsIF = new basicInternalForces();
                        x_resultsIF.fN = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Fx;
                        x_resultsIF.fV_yy = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Fy;
                        x_resultsIF.fV_zz = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Fz;
                        x_resultsIF.fT = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Mx;
                        x_resultsIF.fM_yy = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).My;
                        x_resultsIF.fM_zz = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]).Mz;

                        mResult.InternalForces.Add(x_resultsIF);
                        
                        // Deformations
                        // TODO - dopracovat do BFENEt aby to vracalo aspon nejake rozumne hodnoty :-)
                        // Pripadne updatovat kniznicu a pozriet sa ci taketo funkcie nepridali
                        if (debugging)
                        {
                            string sMessage = "x = " + String.Format(CultureInfo.InvariantCulture, "{0:0.000}", (Math.Round(xLocations_rel[k] * elemLength, 3)));
                            var eDisplacement = (bfenet_model.Elements[j] as FrameElement2Node).GetLocalDeformationAt_MC(xLocations_rel[k] * elemLength, loadcombinations[i]);
                            Trace.WriteLine(sMessage + "\t " + eDisplacement);
                        }

                        basicDeflections x_resultsDeflections = new basicDeflections();
                        x_resultsDeflections.fDelta_yy = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetLocalDeformationAt_MC(xLocations_rel[k] * elemLength, loadcombinations[i], bConsiderNodalDisplacement).DY;
                        x_resultsDeflections.fDelta_zz = (float)(bfenet_model.Elements[j] as FrameElement2Node).GetLocalDeformationAt_MC(xLocations_rel[k] * elemLength, loadcombinations[i], bConsiderNodalDisplacement).DZ;

                        if (x_resultsDeflections.fDelta_yy != 0 || x_resultsDeflections.fDelta_zz != 0)
                            x_resultsDeflections.fDelta_tot = (float)Math.Sqrt(Math.Pow(x_resultsDeflections.fDelta_yy, 2) + Math.Pow(x_resultsDeflections.fDelta_zz, 2));

                        mResult.Deflections.Add(x_resultsDeflections);
                    }
                    membersResults.Add(mResult.MemberID, mResult);
                }
                lcIF.Add(loadCombinationID, membersResults);
            }
            return lcIF;
        }

        public static void DisplayResultsinConsole(Model bfenet_model, List<LoadCombination> loadcombinations, bool bWriteResultsInTXTFile)
        {
            //bool bWriteResultsInTXTFile = true; // Vypise hodnoty do suboru Results.txt na disk D (oddelene tabulatorom, je mozne vlozit do stlpcov tabulky xls)

            List<Force> outputresults = new List<Force>();

            for (int i = 0; i < loadcombinations.Count; i++) // Each load combination
            {
                // Reactions in nodes
                Trace.WriteLine("Load Combination No." + (i + 1).ToString() + " Name: " + GetLoadCombinationString(loadcombinations[i]));

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
