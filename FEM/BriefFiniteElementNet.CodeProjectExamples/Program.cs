using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;
using System.IO;
using System.Globalization;
using BaseClasses;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //InternalForceExample.Run();
            //DocSnippets.Test2();

            //Example1();
            Example2();

            //Example4();
            //Example5();
            //Example6();
            //Example7();
            //Example8();
            //Example9();

            //DocSnippets.Test1();

            //Console.ReadLine();
        }

        private static void Example1()
        {
            Console.WriteLine("Example 1: Simple 3D truss with four members");


            // Initiating Model, Nodes and Members
            var model = new Model();

            var n1 = new Node(1, 1, 0);
            n1.Label = "n1";//Set a unique label for node
            var n2 = new Node(-1, 1, 0) { Label = "n2" };//using object initializer for assigning Label
            var n3 = new Node(1, -1, 0) { Label = "n3" };
            var n4 = new Node(-1, -1, 0) { Label = "n4" };
            var n5 = new Node(0, 0, 1) { Label = "n5" };

            var e1 = new TrussElement2Node(n1, n5) { Label = "e1" };
            var e2 = new TrussElement2Node(n2, n5) { Label = "e2" };
            var e3 = new TrussElement2Node(n3, n5) { Label = "e3" };
            var e4 = new TrussElement2Node(n4, n5) { Label = "e4" };
            //Note: labels for all members should be unique, else you will receive InvalidLabelException when adding it to model

            e1.A = e2.A = e3.A = e4.A = 9e-4;
            e1.E = e2.E = e3.E = e4.E = 210e9;

            model.Nodes.Add(n1, n2, n3, n4, n5);
            model.Elements.Add(e1, e2, e3, e4);

            //Applying restrains


            n1.Constraints = n2.Constraints = n3.Constraints = n4.Constraints = Constraint.Fixed;
            n5.Constraints = Constraint.RotationFixed;


            //Applying load
            var force = new Force(0, 1000, -1000, 0, 0, 0);
            n5.Loads.Add(new NodalLoad(force));//adds a load with LoadCase of DefaultLoadCase to node loads

            //Adds a NodalLoad with Default LoadCase

            model.Solve();

            var r1 = n1.GetSupportReaction();
            var r2 = n2.GetSupportReaction();
            var r3 = n3.GetSupportReaction();
            var r4 = n4.GetSupportReaction();

            var rt = r1 + r2 + r3 + r4;//shows the Fz=1000 and Fx=Fy=Mx=My=Mz=0.0

            Console.WriteLine("Total reactions SUM :" + rt.ToString());
        }

        private static void Example2()
        {
            // https://www.codeproject.com/articles/794983/finite-element-method-programming-in-csharp#ex1

            Console.WriteLine("Example 2: Simple 3D Frame with distributed loads");

            var model = new Model();

            var n1 = new Node(-10, 0, 0);
            var n2 = new Node(-10, 0, 6);
            var n3 = new Node(0, 0, 8);
            var n4 = new Node(10, 0, 6);
            var n5 = new Node(10, 0, 0);

            model.Nodes.Add(n1, n2, n3, n4, n5);

            var secAA = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var secBB = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.52, 0.01, 0.006));

            var e1 = new FrameElement2Node(n1, n2);
            e1.Label = "e1";
            var e2 = new FrameElement2Node(n2, n3);
            e2.Label = "e2";
            var e3 = new FrameElement2Node(n3, n4);
            e3.Label = "e3";
            var e4 = new FrameElement2Node(n4, n5);
            e4.Label = "e4";


            e1.Geometry = e4.Geometry = secAA;
            e2.Geometry = e3.Geometry = secBB;

            e1.E = e2.E = e3.E = e4.E = 210e9;
            e1.G = e2.G = e3.G = e4.G = 210e9 / (2 * (1 + 0.3));//G = E / (2*(1+no))

            e1.UseOverridedProperties =
                e2.UseOverridedProperties = e3.UseOverridedProperties = e4.UseOverridedProperties = false;

            model.Elements.Add(e1, e2, e3, e4);

            n1.Constraints =
                n2.Constraints =
                    n3.Constraints =
                        n4.Constraints =
                            n5.Constraints =
                                Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;//DY fixed and RX fixed and RZ fixed

            n1.Constraints = n1.Constraints & Constraints.MovementFixed;
            n5.Constraints = n5.Constraints & Constraints.MovementFixed;

            // Load Case
            LoadCase lc1 = new LoadCase("lc1", LoadType.Default);

            // Load Combinations
            LoadCombination lcomb1 = new LoadCombination();
            lcomb1.Add(lc1, 1.00);

            List<LoadCombination> loadcombinations = new List<LoadCombination>();
            loadcombinations.Add(lcomb1);

            var ll = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Global, lc1);
            var lr = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Local, lc1);

            e2.Loads.Add(ll);
            e3.Loads.Add(lr);

            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();
            //wnd.Show();

            model.Solve();

            DisplayResultsinConsole(model, loadcombinations, true);
        }

        private static void Example4()
        {
            Console.WriteLine("Example 4: Simple Beam - Trapezoidal load");

            // https://bfenet.readthedocs.io/en/latest/loads/elementLoads/trapezoidalload.html

            var bar = new BarElement();                           //creating new instance of element

            // TO ONDREJ - tu to nevyzera dobre - konstruktor pre PartialTrapezoidalLoad neexistuje
            // Pozri \BriefFiniteElementNet\Loads\PartialTrapezoidalLoad.cs

            /*
            // Zakomentovane z dovodu nefunkcnosti
            var load = new  PartialTrapezoidalLoad();             //creating new instance of load
            load.StartMagnitudes = new double[] { 2000 };         //set Magnitude at start
            load.EndMagnitudes = new double[] { 1000 };           //set Magnitude at end

            load.StartLocations = new double[] { -1 + 2 / 6 };    //set locations of trapezoidal load
            load.EndLocations = new double[] { 1 - 1 / 6 };       //set locations of trapezoidal load

            load.Direction = Vector.K;                            //set direction
            load.CoordinationSystem = CoordinationSystem.Global;  //set coordination system

            bar.Loads.Add(load);                                  //apply load to element
            */
        }

        private static void Example5()
        {
            Console.WriteLine("Example 5: Simple Beam - Concentrated load on member");

            // Pozri \BriefFiniteElementNet\Loads\ConcentratedLoad.cs
            // TO Ondrej - Vyzera to tak ze zatazenie prutu koncentrovanou silou nefunguje ani pre Bar Element ani pre FrameElement2Node

            var model = new Model();

            var n1 = new Node(0, 0, 0);
            var n2 = new Node(5, 0, 0);

            model.Nodes.Add(n1, n2);

            var secAA = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));

            // Frame Element
            var e1 = new FrameElement2Node(n1, n2);
            e1.Label = "e1";

            e1.Geometry = secAA;

            e1.E = 210e9;
            e1.G = 210e9 / (2 * (1 + 0.3));//G = E / (2*(1+no))

            e1.UseOverridedProperties = false;

            /*
            // Bar Element
            var a = 0.1 * 0.1;//area, assume sections are 10cm*10cm rectangular
            var iy = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Iy
            var iz = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Iz
            var j = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Polar
            var e = 20e9;//young modulus, 20 [GPa]
            var nu = 0.2;//poissons ratio

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

            var e1 = new BarElement(n1, n2);
            e1.Section = sec;
            e1.Material = mat;
            */

            model.Elements.Add(e1);

            n1.Constraints =
                n2.Constraints =
                                Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;//DY fixed and RX fixed and RZ fixed

            n1.Constraints = n1.Constraints & Constraints.MovementFixed;
            n2.Constraints = n2.Constraints & Constraints.FixedDZ;

            // Load Case
            LoadCase lc1 = new LoadCase("lc1", LoadType.Default);

            // Load Combinations
            LoadCombination lcomb1 = new LoadCombination();
            lcomb1.Add(lc1, 1.00);

            List<LoadCombination> loadcombinations = new List<LoadCombination>();
            loadcombinations.Add(lcomb1);

            var q = new Force(5000 * Vector.K, Vector.Zero);
            var load = new ConcentratedLoad1D(q, 0.5 * e1.GetElementLength(), CoordinationSystem.Local, lc1);    //creating new instance of load
            e1.Loads.Add(load);                                                                                   //apply load to element

            // Model Check
            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();

            // Run Solver
            model.Solve();
            model.ShowInternalForce();
            model.Show();

            // Output
            DisplayResultsinConsole(model, loadcombinations, true);
        }

        private static void Example6()
        {
            Console.WriteLine("Example 6: Simple Beam - Two uniform loads on member");

            var model = new Model();

            var n1 = new Node(0, 0, 0);
            var n2 = new Node(5, 0, 0);

            model.Nodes.Add(n1, n2);

            var secAA = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));

            // Frame Element
            var e1 = new FrameElement2Node(n1, n2);
            e1.Label = "e1";

            e1.Geometry = secAA;

            e1.E = 210e9;
            e1.G = 210e9 / (2 * (1 + 0.3));//G = E / (2*(1+no))

            e1.UseOverridedProperties = false;

            /*
            // Bar Element
            var a = 0.1 * 0.1;//area, assume sections are 10cm*10cm rectangular
            var iy = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Iy
            var iz = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Iz
            var j = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Polar
            var e = 20e9;//young modulus, 20 [GPa]
            var nu = 0.2;//poissons ratio

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

            var e1 = new BarElement(n1, n2);
            e1.Section = sec;
            e1.Material = mat;
            */

            model.Elements.Add(e1);

            n1.Constraints =
                n2.Constraints =
                                Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;//DY fixed and RX fixed and RZ fixed

            n1.Constraints = n1.Constraints & Constraints.MovementFixed;
            n2.Constraints = n2.Constraints & Constraints.FixedDZ;

            // Load Case
            LoadCase lc1 = new LoadCase("lc1", LoadType.Default);

            // Load Combinations
            LoadCombination lcomb1 = new LoadCombination();
            lcomb1.Add(lc1, 1.00);

            List<LoadCombination> loadcombinations = new List<LoadCombination>();
            loadcombinations.Add(lcomb1);

            var load1 = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Global, lc1);
            var load2 = new UniformLoad1D(-5000, LoadDirection.Z, CoordinationSystem.Local, lc1);
            e1.Loads.Add(load1);                                                                                   //apply load to element
            e1.Loads.Add(load2);

            // Model Check
            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();

            // Run Solver
            model.Solve();
            model.ShowInternalForce();
            model.Show();

            // Output
            DisplayResultsinConsole(model, loadcombinations, true);
        }

        private static void Example7()
        {
            Console.WriteLine("Example 7: Simple Beam - Uniform load");

            var model = new Model();

            var n1 = new Node(0, 0, 0);
            var n2 = new Node(5, 0, 0);

            model.Nodes.Add(n1, n2);

            var secAA = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));

            // Frame Element
            var e1 = new FrameElement2Node(n1, n2);
            e1.Label = "e1";

            e1.Geometry = secAA;

            e1.E = 210e9;
            e1.G = 210e9 / (2 * (1 + 0.3));//G = E / (2*(1+no))

            e1.UseOverridedProperties = false;

            model.Elements.Add(e1);

            n1.Constraints =
                n2.Constraints =
                                Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;//DY fixed and RX fixed and RZ fixed

            // Simply supported
            n1.Constraints = n1.Constraints & Constraints.MovementFixed;
            n2.Constraints = n2.Constraints & Constraints.FixedDZ;

            // Both Ends Fixed
            //n1.Constraints = n1.Constraints & Constraints.FixedRY;
            //n2.Constraints = n2.Constraints & Constraints.FixedRY;

            // Load Case
            LoadCase lc1 = new LoadCase("lc1", LoadType.Default);

            // Load Combinations
            LoadCombination lcomb1 = new LoadCombination();
            lcomb1.Add(lc1, 1.00);

            List<LoadCombination> loadcombinations = new List<LoadCombination>();
            loadcombinations.Add(lcomb1);

            var load = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Global, lc1);             //creating new instance of load
            e1.Loads.Add(load);                                  //apply load to element

            // Display Global Equivalent Nodal Load
            var eForce = e1.GetGlobalEquivalentNodalLoads(load);
            Console.WriteLine("Global Equivalent Nodal Load" + eForce[0]); // Start
            Console.WriteLine("Global Equivalent Nodal Load" + eForce[1]); // End

            // Model Check
            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();

            // Run Solver
            model.Solve();
            model.ShowInternalForce();
            model.Show();

            // Output
            DisplayResultsinConsole(model, loadcombinations, true);
        }

        private static void Example8()
        {
            Console.WriteLine("Example 8: Simple Beam - Partial uniform load");

            var model = new Model();

            var n1 = new Node(0, 0, 0);
            var n2 = new Node(5, 0, 0);

            model.Nodes.Add(n1, n2);

            var secAA = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));

            // Frame Element
            var e1 = new FrameElement2Node(n1, n2);
            e1.Label = "e1";

            e1.Geometry = secAA;

            e1.E = 210e9;
            e1.G = 210e9 / (2 * (1 + 0.3));//G = E / (2*(1+no))

            e1.UseOverridedProperties = false;

            model.Elements.Add(e1);

            n1.Constraints =
                n2.Constraints =
                                Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;//DY fixed and RX fixed and RZ fixed

            // Simply supported
            n1.Constraints = n1.Constraints & Constraints.MovementFixed;
            n2.Constraints = n2.Constraints & Constraints.FixedDZ;

            // Both Ends Fixed
            //n1.Constraints = n1.Constraints & Constraints.FixedRY;
            //n2.Constraints = n2.Constraints & Constraints.FixedRY;

            // Load Case
            LoadCase lc1 = new LoadCase("lc1", LoadType.Default);

            // Load Combinations
            LoadCombination lcomb1 = new LoadCombination();
            lcomb1.Add(lc1, 1.00);

            List<LoadCombination> loadcombinations = new List<LoadCombination>();
            loadcombinations.Add(lcomb1);

            // Start and end offset, resp. isolocation [-1,1]
            var load = new PartialUniformLoad1D(-10000, -1 + 0.05, 1 - 0.1, LoadDirection.Z, CoordinationSystem.Global, lc1);             //creating new instance of load

            e1.Loads.Add(load);                                  //apply load to element

            // Display Global Equivalent Nodal Load
            var eForce = e1.GetGlobalEquivalentNodalLoads(load);
            Console.WriteLine("Global Equivalent Nodal Load " + eForce[0]); // Start
            Console.WriteLine("Global Equivalent Nodal Load " + eForce[1]); // End

            // Model Check
            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();

            // Run Solver
            model.Solve();
            model.ShowInternalForce();
            model.Show();

            // Output
            DisplayResultsinConsole(model, loadcombinations, true);
        }

        private static void Example9()
        {
            Console.WriteLine("Example 9: Simple Beam - Partial uniform load");

            var model = new Model();

            var n1 = new Node(0, 0, 0);
            var n2 = new Node(5, 0, 0);

            model.Nodes.Add(n1, n2);

            var secAA = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));

            // Frame Element
            var e1 = new FrameElement2Node(n1, n2);
            e1.Label = "e1";

            e1.Geometry = secAA;

            e1.E = 210e9;
            e1.G = 210e9 / (2 * (1 + 0.3));//G = E / (2*(1+no))

            e1.UseOverridedProperties = false;

            model.Elements.Add(e1);

            n1.Constraints =
                n2.Constraints =
                                Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;//DY fixed and RX fixed and RZ fixed

            // Simply supported
            n1.Constraints = n1.Constraints & Constraints.MovementFixed;
            n2.Constraints = n2.Constraints & Constraints.FixedDZ;

            // Both Ends Fixed
            //n1.Constraints = n1.Constraints & Constraints.FixedRY;
            //n2.Constraints = n2.Constraints & Constraints.FixedRY;

            // Load Case
            LoadCase lc1 = new LoadCase("lc1", LoadType.Default);

            // Load Combinations
            LoadCombination lcomb1 = new LoadCombination();
            lcomb1.Add(lc1, 1.00);

            List<LoadCombination> loadcombinations = new List<LoadCombination>();
            loadcombinations.Add(lcomb1);

            // Start and end offset, resp. isolocation [-1,1]
            var load1 = new PartialUniformLoad1D(-10000, -1 + 0.05, 0 - 0.5, LoadDirection.Z, CoordinationSystem.Global, lc1);             //creating new instance of load
            var load2 = new PartialUniformLoad1D(-10000, -1 + 0.5, 1 - 0.5, LoadDirection.Z, CoordinationSystem.Global, lc1);             //creating new instance of load
            var load3 = new PartialUniformLoad1D(-10000, 1 - 0.5, 1 - 0.1, LoadDirection.Z, CoordinationSystem.Global, lc1);             //creating new instance of load

            e1.Loads.Add(load1);                                  //apply load to element
            e1.Loads.Add(load2);                                  //apply load to element
            e1.Loads.Add(load3);                                  //apply load to element

            // Display Global Equivalent Nodal Load
            //var eForce = e1.GetGlobalEquivalentNodalLoads(load1);
            //Console.WriteLine("Global Equivalent Nodal Load " + eForce[0]); // Start
            //Console.WriteLine("Global Equivalent Nodal Load " + eForce[1]); // End

            // Model Check
            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();

            // Run Solver
            model.Solve();
            model.ShowInternalForce();
            model.Show();

            // Output
            DisplayResultsinConsole(model, loadcombinations, true);
        }

        private static void LoadComb()
        {
            new BarIncliendFrameExample().Run();
        }

        public static void DisplayResultsinConsole(Model bfenet_model, List<LoadCombination> loadcombinations, bool bWriteResultsInTXTFile)
        {
            //bool bWriteResultsInTXTFile = true; // Vypise hodnoty do suboru Results.txt na disk D (oddelene tabulatorom, je mozne vlozit do stlpcov tabulky xls)

            List<Force> outputresults = new List<Force>();

            for (int i = 0; i < loadcombinations.Count; i++) // Each load combination
            {
                // Reactions in nodes
                Console.WriteLine("Load Combination No." + (i + 1).ToString() + " Name: " + GetLoadCombinationString(loadcombinations[i]));

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
                    Console.WriteLine("Total reactions SUM in Node No.: " + (j + 1).ToString() + " is: " + rj.ToString());

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

                Console.WriteLine("Total reactions SUM :" + rtotalsum.ToString() + "\n");

                // Internal forces

                const int iNumberOfResultsSections = 11;
                double[] xLocations_rel = new double[iNumberOfResultsSections];

                // Fill relative coordinates (x_rel)
                for (int s = 0; s < iNumberOfResultsSections; s++)
                    xLocations_rel[s] = s * 1.0f / (iNumberOfResultsSections - 1);

                for (int j = 0; j < bfenet_model.Elements.Count; j++) // Each element in the model
                {
                    Console.WriteLine("Element No.: " + (j + 1).ToString());
                    Console.WriteLine("Internal forces in particular x positions");

                    double elemLength = bfenet_model.Elements[j].GetElementLength();

                    for (int k = 0; k < xLocations_rel.Length; k++)
                    {
                        string sMessage = "x = " + String.Format(CultureInfo.InvariantCulture, "{0:0.000}", (Math.Round(xLocations_rel[k] * elemLength, 3)));
                        var eForce = (bfenet_model.Elements[j] as FrameElement2Node).GetInternalForceAt(xLocations_rel[k] * elemLength, loadcombinations[i]);
                        Console.WriteLine(sMessage + "\t " + eForce);

                        if (bWriteResultsInTXTFile)
                            outputresults.Add(eForce);
                    }
                }
            }

            if (bWriteResultsInTXTFile)
            {
                try
                {
                    Console.WriteLine("\nWritting results to the file D:\\Results.txt.");

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
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
            }

            try
            {
                Console.ReadKey();
            }
            catch
            {
                // Konzolu nebolo mozne zobrazit ak je projekt spusteny z inej aplikacie
            }

            // To Ondrej - neviem ako presne toto funguje a co to zobrazuje :) ... asi vysledky pre vsetky spocitane zatazovacie stavy

            bool bDisplayLastCalculated = false;

            if (bDisplayLastCalculated)
            {
                Console.WriteLine("\nmodel.LastResult.Forces:");
                foreach (KeyValuePair<LoadCase, double[]> kvp in bfenet_model.LastResult.Forces)
                {
                    Console.WriteLine($"{kvp.Key.CaseName} {kvp.Key.LoadType.ToString()} count: {kvp.Value.Length} values: {string.Join(";", kvp.Value)} ");
                }
                Console.WriteLine("\nmodel.LastResult.ConcentratedForces:");
                foreach (KeyValuePair<LoadCase, double[]> kvp in bfenet_model.LastResult.ConcentratedForces)
                {
                    Console.WriteLine($"{kvp.Key.CaseName} {kvp.Key.LoadType.ToString()} count: {kvp.Value.Length} values: {string.Join(";", kvp.Value)} ");
                }
                Console.WriteLine("\nmodel.LastResult.Displacements:");
                foreach (KeyValuePair<LoadCase, double[]> kvp in bfenet_model.LastResult.Displacements)
                {
                    Console.WriteLine($"{kvp.Key.CaseName} {kvp.Key.LoadType.ToString()} count: {kvp.Value.Length} values: {string.Join(";", kvp.Value)} ");
                }
                Console.WriteLine("\nmodel.LastResult.ElementForces:");
                foreach (KeyValuePair<LoadCase, double[]> kvp in bfenet_model.LastResult.ElementForces)
                {
                    Console.WriteLine($"{kvp.Key.CaseName} {kvp.Key.LoadType.ToString()} count: {kvp.Value.Length} values: {string.Join(";", kvp.Value)} ");
                }
            }
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
