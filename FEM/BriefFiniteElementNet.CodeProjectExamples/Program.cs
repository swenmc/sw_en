using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;

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
            //Example2();
            //DocSnippets.Test1();


            // POKUS O NAPOJENIE SW_EN modelu
            MATERIAL.CMat mat = new MATERIAL.CMat();
            CRSC.CCrSc_3_63020_BOX crsc1 = new CRSC.CCrSc_3_63020_BOX(1, 0.63f, 0.18f, 0.00195f, 0.00195f, System.Windows.Media.Colors.Coral);
            CRSC.CCrSc_3_63020_BOX crsc2 = new CRSC.CCrSc_3_63020_BOX(1, 0.63f, 0.18f, 0.00195f, 0.00295f, System.Windows.Media.Colors.Cyan);

            BaseClasses.CExample model = new Examples.CExample_2D_13_PF(mat, crsc1, crsc2, 20f, 6f, 8f, 100f, 200f, -100f, 1f);

            /*
            Example3(model);
            */
            Console.ReadLine();
        }

        private static void Example1()
        {
            Console.WriteLine("Example 1: Simple 3D truss with four members");


            // Initiating Model, Nodes and Members
            var model = new Model();

            var n1 = new Node(1, 1, 0);
            n1.Label = "n1";//Set a unique label for node
            var n2 = new Node(-1, 1, 0) {Label = "n2"};//using object initializer for assigning Label
            var n3 = new Node(1, -1, 0) {Label = "n3"};
            var n4 = new Node(-1, -1, 0) {Label = "n4"};
            var n5 = new Node(0, 0, 1) {Label = "n5"};

            var e1 = new TrussElement2Node(n1, n5) {Label = "e1"};
            var e2 = new TrussElement2Node(n2, n5) {Label = "e2"};
            var e3 = new TrussElement2Node(n3, n5) {Label = "e3"};
            var e4 = new TrussElement2Node(n4, n5) {Label = "e4"};
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
            e1.G = e2.G = e3.G = e4.G = 210e9/(2*(1 + 0.3));//G = E / (2*(1+no))

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

            var ll = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Global);
            var lr = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Local);

            e2.Loads.Add(ll);
            e3.Loads.Add(lr);

            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();

            model.Solve();
        }

        // Pokus o napojenie SW_EN
        private static void Example3(BaseClasses.CModel topomodel)
        {
            // Dokumentacia a priklady
            // https://bfenet.readthedocs.io/en/latest/example/loadcasecomb/index.html
            // https://bfenet.readthedocs.io/en/latest/example/inclinedframe/index.html
            // BarIncliendFrameExample.cs file

            // TO Ondrej - pointa je v tom ze potrebujeme preklopit nase datove objekty a zatazenia do objektov BFEMNet, vytvorit model, spustit vypocet, nacitat vysledky a tie pouzit v nasom hlavnom programe
            // Skopiroval som priklad 2, Nieco som skusil napojit a zakomentoval som to co sa nepouzije, ale ked som pridal referencie na nase projekty tak to neide prelozit
            // Ten DebugerVisualizer, ktory sa mal otvorit cez ikonku lupy ked spustim Example 2 som v VS nenasiel takze to okno s 3D sa mi nepodarilo zobrazit

            Console.WriteLine("Example 3: Simple 3D Frame with distributed loads");

            var model = new Model();

            /*
            var n1 = new Node(-10, 0, 0);
            var n2 = new Node(-10, 0, 6);
            var n3 = new Node(0, 0, 8);
            var n4 = new Node(10, 0, 6);
            var n5 = new Node(10, 0, 0);

            model.Nodes.Add(n1, n2, n3, n4, n5);
            */

            // Nodes
            NodeCollection nodecollection_temp = new NodeCollection(model);

            for (int i = 0; i < topomodel.m_arrNodes.Length; i++)
            {
                nodecollection_temp.Add(new Node(topomodel.m_arrNodes[i].X, topomodel.m_arrNodes[i].Y, topomodel.m_arrNodes[i].Z));
            }

            model.Nodes = nodecollection_temp;

            // Cross-sections
            /*
            var secAA = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var secBB = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.52, 0.01, 0.006));
            */

            // To Ondrej - Pochopil som to tak, ze hodnoty pre prierez A,Iy,Iz je mozne zadat numericky alebo definovat Geometry,
            // ale Geometry ma len definiciou pre obdlznik a tvar I
            // do buducna by sme mohli geometry rozsirit alebo mozeme predavat hodnoty z nasich prierezov do solvera len ciselne

            /*
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
            */

            // Elements (Members)
            ElementCollection elementcollection_temp = new ElementCollection(model);

            for (int i = 0; i < topomodel.m_arrMembers.Length; i++)
            {
                var eTemp = new FrameElement2Node(nodecollection_temp[topomodel.m_arrMembers[i].NodeStart.ID - 1], nodecollection_temp[topomodel.m_arrMembers[i].NodeEnd.ID - 1]);
                eTemp.Label = "e" + topomodel.m_arrMembers[i].ID.ToString();
                eTemp.A = topomodel.m_arrMembers[i].CrScStart.A_g;
                eTemp.Iy = topomodel.m_arrMembers[i].CrScStart.I_y;
                eTemp.Iz = topomodel.m_arrMembers[i].CrScStart.I_z;
                eTemp.E = topomodel.m_arrMembers[i].CrScStart.m_Mat.m_fE;
                eTemp.G = topomodel.m_arrMembers[i].CrScStart.m_Mat.m_fG;

                // Note: Elements with UseOverridedProperties = true are shown with square sections(dimension of section automatically tunes for better visualization of elements)
                // but elements with UseOverridedProperties = false will be shown with their real section with real dimesion.
                eTemp.UseOverridedProperties = true;

                elementcollection_temp.Add(eTemp);
            }

            model.Elements = elementcollection_temp;

            // Supports
            /*
            n1.Constraints =
                n2.Constraints =
                    n3.Constraints =
                        n4.Constraints =
                            n5.Constraints =
                                Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;//DY fixed and RX fixed and RZ fixed

            n1.Constraints = n1.Constraints & Constraints.MovementFixed;
            n5.Constraints = n5.Constraints & Constraints.MovementFixed;
            */

            // 2D model in XZ plane - we set for all nodes deflection DY fixed and rotation RX fixed and RZ fixed
            // podoprieme vsetky uzly pre posun z roviny XZ a pre pootocenie okolo X a Z

            for (int i = 0; i < topomodel.m_arrNodes.Length; i++)
            {
                model.Nodes[i].Constraints = Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;
            }

            // Prejdeme vsetky podpory, vsetky uzly im priradene a nastavime na tychto uzloch podopretie pre prislusne posuny alebo pootocenia
            for (int i = 0; i < topomodel.m_arrNSupports.Length; i++)
            {
                for (int j = 0; j < topomodel.m_arrNSupports[i].m_iNodeCollection.Length; j++)
                {
                    for (int k = 0; k < model.Nodes.Count; k++)
                    {
                        if (k == (topomodel.m_arrNSupports[i].m_iNodeCollection[j] - 1)) // porovnat index v poli (pripadne ID, ale je treba zistit ako sa urcuju ID objektu node v BFEMNet) // TO Ondrej, chcelo by to rozhodnut ci budeme pouzivat pri porovnavani indexy z pola alebo ID objektov (ID objektov mozu nemusia byt kontinualne 1,2,3,6,7,8,9
                        {
                            if (topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Ux] == true)
                                model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedDX;
                            if (topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Uy] == true)
                                model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedDY;
                            if (topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Uz] == true)
                                model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedDZ;
                            if (topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Rx] == true)
                                model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedRX;
                            if (topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Ry] == true)
                                model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedRY;
                            if (topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Rz] == true)
                                model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedRZ;
                        }
                    }
                }
            }

            /*
            var ll = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Global);
            var lr = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Local);

            e2.Loads.Add(ll);
            e3.Loads.Add(lr);
            */

            // TODO - dopracovat load cases, je potrebne nastudovat ako sa to v BFEMNet da nastavovat, ake su typy atd - load cases, load combinations

            // Load Cases
            List<LoadCase> loadcases = new List<LoadCase>();

            for (int i = 0; i < topomodel.m_arrLoadCases.Length; i++)
            {
                LoadType loadtype;

                if (topomodel.m_arrLoadCases[i].Type == BaseClasses.ELCType.ePermanentLoad)
                    loadtype = LoadType.Dead;
                else if (topomodel.m_arrLoadCases[i].Type == BaseClasses.ELCType.eImposedLoad_LT || topomodel.m_arrLoadCases[i].Type == BaseClasses.ELCType.eImposedLoad_ST)
                    loadtype = LoadType.Live;
                else if (topomodel.m_arrLoadCases[i].Type == BaseClasses.ELCType.eSnow)
                    loadtype = LoadType.Snow;
                else if (topomodel.m_arrLoadCases[i].Type == BaseClasses.ELCType.eWind)
                    loadtype = LoadType.Wind;
                else if (topomodel.m_arrLoadCases[i].Type == BaseClasses.ELCType.eEarthquake)
                    loadtype = LoadType.Quake;
                else
                    loadtype = LoadType.Other;

                loadcases.Add(new LoadCase(topomodel.m_arrLoadCases[i].Name, loadtype));
            }

            // Load Combinations
            List<LoadCombination> loadcombinations = new List<LoadCombination>();

            for (int i = 0; i < topomodel.m_arrLoadCombs.Length; i++)
            {
                // LoadCombination dedi od Dictionary<LoadCase, double>, tj. load case a faktor

                LoadCombination lcomb = new LoadCombination();

                // Add specific load cases into the combination and set load factors
                for (int j = 0; j < topomodel.m_arrLoadCombs[i].LoadCasesList.Count; j++)
                {
                    lcomb.Add(loadcases[topomodel.m_arrLoadCombs[i].LoadCasesList[j].ID], topomodel.m_arrLoadCombs[i].LoadCasesFactorsList[j]);
                }

                loadcombinations.Add(lcomb);
            }

            // Loads

            for (int i = 0; i < loadcases.Count; i++) // Each load case
            {
                for (int j = 0; j < topomodel.m_arrLoadCases[i].MemberLoadsList.Count; j++) // Each member load
                {
                    for (int k = 0; k < topomodel.m_arrLoadCases[i].MemberLoadsList[j].IMemberCollection.Length; k++) // Each loaded member
                    {
                        // TODO - zistit ake ma BFEMNet typy zatazeni vypracovat kluc ako to konvertovat
                        // BFEMNet ma tri typy - concentrated, uniform, trapezoidal

                        // load
                        var l = new UniformLoad1D();

                        if (topomodel.m_arrLoadCases[i].MemberLoadsList[j] is BaseClasses.CMLoad_21) // Uniform load per whole member
                        {
                            BaseClasses.CMLoad_21 load = new BaseClasses.CMLoad_21();
                            // Create member load
                            if (topomodel.m_arrLoadCases[i].MemberLoadsList[j].MLoadType == BaseClasses.EMLoadType.eMLT_F && topomodel.m_arrLoadCases[i].MemberLoadsList[j] is BaseClasses.CMLoad_21)
                                load = (BaseClasses.CMLoad_21)topomodel.m_arrLoadCases[i].MemberLoadsList[j];

                            // TODO - nastavit spravny smer a system v ktorom je zatazenie zadane
                            // Skontrolovat zadanie v GCS a LCS

                            CoordinationSystem eCS;
                            if (load.ELoadCS == BaseClasses.ELoadCoordSystem.eGCS)
                                eCS = CoordinationSystem.Global;
                            else // if (load.ELoadCS == ELoadCoordSystem.eLCS || load.ELoadCS == ELoadCoordSystem.ePCS)
                                eCS = CoordinationSystem.Local;

                            LoadDirection eLD;

                            if (load.EDirPPC == BaseClasses.EMLoadDirPCC1.eMLD_PCC_FXX_MXX)
                                eLD = LoadDirection.X;
                            else if (load.EDirPPC == BaseClasses.EMLoadDirPCC1.eMLD_PCC_FYU_MZV)
                                eLD = LoadDirection.Y;
                            else //if (load.EDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
                                eLD = LoadDirection.Z;

                            l = new UniformLoad1D(load.Fq, eLD, eCS, loadcases[i]);
                        }
                        else if (topomodel.m_arrLoadCases[i].MemberLoadsList[j] is BaseClasses.CMLoad_24) // Uniform load on segment
                        {
                            BaseClasses.CMLoad_24 load = new BaseClasses.CMLoad_24();

                            // Create member load
                            if (topomodel.m_arrLoadCases[i].MemberLoadsList[j].MLoadType == BaseClasses.EMLoadType.eMLT_F && topomodel.m_arrLoadCases[i].MemberLoadsList[j] is BaseClasses.CMLoad_24)
                                load = (BaseClasses.CMLoad_24)topomodel.m_arrLoadCases[i].MemberLoadsList[j];

                            // TODO - nastavit spravny smer a system v ktorom je zatazenie zadane
                            // Skontrolovat zadanie v GCS a LCS

                            CoordinationSystem eCS;
                            if (load.ELoadCS == BaseClasses.ELoadCoordSystem.eGCS)
                                eCS = CoordinationSystem.Global;
                            else // if (load.ELoadCS == ELoadCoordSystem.eLCS || load.ELoadCS == ELoadCoordSystem.ePCS)
                                eCS = CoordinationSystem.Local;

                            LoadDirection eLD;

                            if (load.EDirPPC == BaseClasses.EMLoadDirPCC1.eMLD_PCC_FXX_MXX)
                                eLD = LoadDirection.X;
                            else if (load.EDirPPC == BaseClasses.EMLoadDirPCC1.eMLD_PCC_FYU_MZV)
                                eLD = LoadDirection.Y;
                            else //if (load.EDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
                                eLD = LoadDirection.Z;

                            // PartialTrapezoidalLoad
                            // TODO - toto by sme potrebovali, pisu tam ze je to obsolete ale na internete je uz priklad
                            // Urcite mame najnovsie zdroje?, resp. to mozno mali v starsej verzii a skryli to
                            // https://bfenet.readthedocs.io/en/latest/loads/elementLoads/trapezoidalload.html
                            // https://media.readthedocs.org/pdf/bfenet/latest/bfenet.pdf

                            l = new UniformLoad1D(load.Fq, eLD, eCS, loadcases[i]);
                        }
                        else
                        {
                            // Not implemented load type
                            // l = new UniformLoad1D();
                        }
                        elementcollection_temp[topomodel.m_arrLoadCases[i].MemberLoadsList[j].IMemberCollection[k]].Loads.Add(l);
                    }
                }
            }

            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();

            model.Solve();
            // model.Solve_MPC(); //no different with Model.Solve() - toto su asi identicke prikazy

            // Ako ziskat reakcie a vnutrne sily pre kombinacie
            // identifikator uzla (nazov "N3") + pozadovana kombinacia
            // indetifikator elementu (nazov "e4") + pozicia x na prute (0 je asi stred pruta ???) a kombinacia

            /*
            var n3Force = model.Nodes["N3"].GetSupportReaction(combination1);
            Console.WriteLine(n3Force);
            or for finding internal force of e4 element with combination D + 0.8 L at it’s centre:
            var e4Force = (model.Elements["e4"] as BarElement).GetInternalForceAt(0, combination1);
            Console.WriteLine(e4Force);
            */
        }

        private static void LoadComb()
        {
            new BarIncliendFrameExample().Run();
        }
    }
}
