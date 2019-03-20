using BaseClasses;
using BriefFiniteElementNet;
using System;
using System.Collections.Generic;

namespace PFD
{
    public static class CModelToBFEMNetConverter
    {

        public static Model Convert(CModel topomodel, bool bCalculateLoadCasesOnly, bool bConsiderNodalDisplacement = false)
        {
            // Dokumentacia a priklady
            // https://bfenet.readthedocs.io/en/latest/example/loadcasecomb/index.html
            // https://bfenet.readthedocs.io/en/latest/example/inclinedframe/index.html
            // BarIncliendFrameExample.cs file

            // Pointa je v tom ze potrebujeme preklopit nase datove objekty a zatazenia do objektov BFEMNet, vytvorit model, 
            //spustit vypocet, nacitat vysledky a tie pouzit v nasom hlavnom programe

            var model = new Model();
            // Nodes
            NodeCollection nodeCollection = new NodeCollection(model);

            for (int i = 0; i < topomodel.m_arrNodes.Length; i++)
            {
                nodeCollection.Add(new Node(topomodel.m_arrNodes[i].X, topomodel.m_arrNodes[i].Y, topomodel.m_arrNodes[i].Z) { Label = "n" + topomodel.m_arrNodes[i].ID });
            }

            model.Nodes = nodeCollection;

            // Cross-sections
            // To Ondrej - Pochopil som to tak, ze hodnoty pre prierez A,Iy,Iz je mozne zadat numericky alebo definovat Geometry,
            // ale Geometry ma len definiciou pre obdlznik a tvar I
            // do buducna by sme mohli geometry rozsirit alebo mozeme predavat hodnoty z nasich prierezov do solvera len ciselne
            //Mato: Podla mna by sa malo dat rozsirit SectionGenerator a to tak ze vstupom bude nas Crsc a vygeneruje to len to pole bodov

            // Elements (Members)
            ElementCollection elementCollection = new ElementCollection(model);

            for (int i = 0; i < topomodel.m_arrMembers.Length; i++)
            {
                // Find index of start and end node of member
                int iStartNodeIndex = int.MinValue;
                int iEndNodeIndex = int.MinValue;

                for (int j = 0; j< topomodel.m_arrNodes.Length; j ++)
                {
                    if (topomodel.m_arrMembers[i].NodeStart == topomodel.m_arrNodes[j]) iStartNodeIndex = j;
                    if (topomodel.m_arrMembers[i].NodeEnd == topomodel.m_arrNodes[j]) iEndNodeIndex = j;
                }

                // Validation
                if(iStartNodeIndex == int.MinValue || iEndNodeIndex == int.MinValue)
                {
                    throw new ArgumentNullException("Start or end index of member definiton nodes wasn't found.");
                }

                var element_1D_2Node = new FrameElement2Node(nodeCollection[iStartNodeIndex], nodeCollection[iEndNodeIndex]);
                element_1D_2Node.Label = "e" + topomodel.m_arrMembers[i].ID.ToString();
                element_1D_2Node.MID = topomodel.m_arrMembers[i].ID;

                var sec = new BriefFiniteElementNet.Sections.UniformParametric1DSection(topomodel.m_arrMembers[i].CrScStart.A_g,
                    topomodel.m_arrMembers[i].CrScStart.I_y,
                    topomodel.m_arrMembers[i].CrScStart.I_z,
                    topomodel.m_arrMembers[i].CrScStart.I_t);

                var mat = new BriefFiniteElementNet.Materials.UniformIsotropicMaterial(topomodel.m_arrMembers[i].CrScStart.m_Mat.m_fE, topomodel.m_arrMembers[i].CrScStart.m_Mat.m_fNu);
                element_1D_2Node.E = mat.YoungModulus;
                element_1D_2Node.G = topomodel.m_arrMembers[i].CrScStart.m_Mat.m_fG;
                //element_1D_2Node.MassDensity = topomodel.m_arrMembers[i].m_Mat.m_fRho;

                element_1D_2Node.A = sec.A;
                element_1D_2Node.Ay = 0.00f;
                element_1D_2Node.Az = 0.00f; // Todo - doplnit do databazy
                element_1D_2Node.Iy = sec.Iy; // TODO - doriesit Iyy a Iyu - geometricke alebo hlavne osi prierezu
                element_1D_2Node.Iz = sec.Iz;
                element_1D_2Node.J = sec.J;
                //element_1D_2Node.ConsiderShearDeformation = true;

                if (topomodel.m_eSLN == BaseClasses.ESLN.e2DD_1D)
                {
                    if (topomodel.m_arrMembers[i].CnRelease1 != null)
                        element_1D_2Node.HingedAtStart = !(bool)topomodel.m_arrMembers[i].CnRelease1.m_bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Rz];

                    if (topomodel.m_arrMembers[i].CnRelease2 != null)
                        element_1D_2Node.HingedAtEnd = !(bool)topomodel.m_arrMembers[i].CnRelease2.m_bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Rz];
                }
                else
                {
                    if (topomodel.m_arrMembers[i].CnRelease1 != null)
                        element_1D_2Node.HingedAtStart = !(bool)topomodel.m_arrMembers[i].CnRelease1.m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Ry];
                    if (topomodel.m_arrMembers[i].CnRelease2 != null)
                        element_1D_2Node.HingedAtEnd = !(bool)topomodel.m_arrMembers[i].CnRelease2.m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Ry];
                }

                // Note: Elements with UseOverridedProperties = true are shown with square sections(dimension of section automatically tunes for better visualization of elements)
                // but elements with UseOverridedProperties = false will be shown with their real section with real dimesion.
                element_1D_2Node.UseOverridedProperties = true;
                
                elementCollection.Add(element_1D_2Node);
            }

            model.Elements = elementCollection;

            // SUPPORTS
            // 2D model in XZ plane - we set for all nodes deflection DY fixed and rotation RX fixed and RZ fixed
            // podoprieme vsetky uzly pre posun z roviny XZ a pre pootocenie okolo X a Z
            // Zabranime vsetkym uzlom aby sa posunuli v smere Y a pootocili okolo X a Z pretoze ram je v rovine XZ, 
            // ale pocitame ho 3D solverom ktory berie do uvahy ze sa to moze posunut aj mimo tejto roviny,
            // takze musi byt podoprety tak ze sa v smere Y nemoze posunut, stale musi byt fixovany len v rovine XZ
            // Preto sa na vsetky uzly nastavia tieto tri podmienky

            if (topomodel.m_eSLN == ESLN.e2DD_1D) // 2D model is defined
            {
                for (int i = 0; i < topomodel.m_arrNodes.Length; i++)
                {
                    model.Nodes[i].Constraints = Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;
                }
            }

            // Prejdeme vsetky podpory, vsetky uzly im priradene a nastavime na tychto uzloch podopretie pre prislusne posuny alebo pootocenia            
            // Tu by sa mali nastavit podpory v rovine ramu (posuny UX a UZ) a pototocenie RY
            for (int i = 0; i < topomodel.m_arrNSupports.Length; i++)
            {
                for (int j = 0; j < topomodel.m_arrNSupports[i].m_iNodeCollection.Length; j++)
                {
                    for (int k = 0; k < model.Nodes.Count; k++)
                    {
                        if (k == (topomodel.m_arrNSupports[i].m_iNodeCollection[j] - 1))
                        // porovnat index v poli (pripadne ID, ale je treba zistit ako sa urcuju ID objektu node v BFEMNet) 
                        // TO Ondrej, chcelo by to rozhodnut ci budeme pouzivat pri porovnavani indexy z pola alebo ID objektov (ID objektov mozu nemusia byt kontinualne 1,2,3,6,7,8,9
                        // 2019_02_18  Naco sa porovnava index v poli???
                        {
                            // Set restraints depending on the FEM DOF number
                            if (topomodel.m_eSLN == BaseClasses.ESLN.e2DD_1D)
                            {
                                // 2D
                                if (topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Ux] == true)
                                    model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedDX;
                                if (topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Uy] == true)
                                    model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedDZ;
                                if (topomodel.m_arrNSupports[i].m_bRestrain.Length > (int)BaseClasses.ENSupportType_2D.eNST_Rz && topomodel.m_arrNSupports[i].m_bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Rz] == true)
                                    model.Nodes[k].Constraints = model.Nodes[i].Constraints & Constraints.FixedRY;
                            }
                            else if (topomodel.m_eSLN == BaseClasses.ESLN.e3DD_1D)
                            {
                                // 3D
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
                            else
                            {
                                // Not implenented or not defined type
                            }
                        }
                    }
                }
            }

            // Load Cases
            List<LoadCase> loadcases = new List<LoadCase>();

            for (int i = 0; i < topomodel.m_arrLoadCases.Length; i++)
            {
                LoadType loadtype = GetBFEMLoadType(topomodel.m_arrLoadCases[i].Type);

                loadcases.Add(new LoadCase(topomodel.m_arrLoadCases[i].Name, loadtype));
            }

            // Load Combinations
            List<LoadCombination> loadcombinations = new List<LoadCombination>();

            int iNumberOFCycles;

            if (bCalculateLoadCasesOnly)
                iNumberOFCycles = topomodel.m_arrLoadCases.Length;
            else
                iNumberOFCycles = topomodel.m_arrLoadCombs.Length;

            for (int i = 0; i < iNumberOFCycles; i++)
            {
                // LoadCombination dedi od Dictionary<LoadCase, double>, tj. load case a faktor
                LoadCombination lcomb = new LoadCombination();
                lcomb.LcID = topomodel.m_arrLoadCombs[i].ID;

                if (bCalculateLoadCasesOnly)
                {
                    // Add one specific load case into the combination and set load factor to 1.00
                    lcomb.Add(loadcases[topomodel.m_arrLoadCases[i].ID - 1], 1f);
                }
                else
                {
                    // Add specific load cases into the combination and set load factors
                    for (int j = 0; j < topomodel.m_arrLoadCombs[i].LoadCasesList.Count; j++)
                    {
                        lcomb.Add(loadcases[topomodel.m_arrLoadCombs[i].LoadCasesList[j].ID - 1], topomodel.m_arrLoadCombs[i].LoadCasesFactorsList[j]);
                    }
                }

                loadcombinations.Add(lcomb);
            }

            // Loads
            for (int i = 0; i < loadcases.Count; i++) // Each load case
            {
                for (int j = 0; j < topomodel.m_arrLoadCases[i].MemberLoadsList.Count; j++) // Each member load
                {
                    // TODO - prepracovat system pre priradzovanie typovych objektov Loads, Supports a podobne,
                    // nemusel by existovat vzdy samostatny objekt pre kazdu realnu poziciu v konstrukcii ale stacil by jeden typovy,
                    // ktory by obsahoval zoznam objektov, ku ktorym je priradeny, je potrebne vyriesit ako by sa tento jeden objekt opakovane vykreslil na roznych miestach (objektoch) kam je priradeny

                    if (topomodel.m_arrLoadCases[i].MemberLoadsList[j] == null) continue;
                    //if (topomodel.m_arrLoadCases[i].MemberLoadsList[j].IMemberCollection != null) // PODOBNY PROBLEM AKO S CNSUPPORT - mal by to byt objekt v ktorom je list prutov ktorym je priradeny, ale teraz je CMLoad definovane na kazdom prute zvlast

                    Load BFEMLoad = GetBFEMLoad(topomodel.m_arrLoadCases[i].MemberLoadsList[j], loadcases[i]);
                    if (BFEMLoad != null)
                    {
                        int iMemberIndex_FM = -1;

                        if (topomodel is CBeam_Simple) // Simple beam model
                        {
                            iMemberIndex_FM = 0;
                        }
                        else if (topomodel is CFrame) // Frame model
                        {
                            CFrame frame = (CFrame)topomodel;

                            // Zatazenie odkazuje na globalne ID pruta, ale tu sa zmeni na ID elementu v BFENet (0-3)
                            iMemberIndex_FM = frame.GetMemberIndexInFrame(topomodel.m_arrLoadCases[i].MemberLoadsList[j].Member);
                        }
                        else
                        {
                            // General 3D model - not implemented yet
                            throw new Exception("Not implemented model type.");
                        }

                        if (iMemberIndex_FM < 0 || iMemberIndex_FM > elementCollection.Count - 1) // Validation
                        {
                            // Index pruta ma byt od 0 - 3, ale odkaz v zatazeni je Member.ID je z globalneho modelu
                            throw new ArgumentException("Invalid ID of member assigned to the member load");
                        }

                        elementCollection[iMemberIndex_FM].Loads.Add(BFEMLoad);
                    }
                }
            }

            model.Solve();
            // model.Solve_MPC(); //no difference with Model.Solve() - toto su asi identicke prikazy

            //model.ShowInternalForce();  //toto tu nebude fungovat pokial to chceme pocitat v Background THREAD
            //model.Show(); //toto tu nebude fungovat pokial to chceme pocitat v Background THREAD

            bool bDebugMemberResults = false; // Display member IF results in console

            if(bDebugMemberResults)
               BFEMNetModelHelper.DisplayResultsinConsole(model, loadcombinations, false);

            // Vytvori zoznamy zoznamov struktur vysledkov pre kazdu kombinaciu, kazdy prut, kazde x miesto na prute
            topomodel.LoadCombInternalForcesResults = BFEMNetModelHelper.GetResultsList(model, loadcombinations, bConsiderNodalDisplacement);
            topomodel.BFEMNetModel = model;
            return model;
        }

        public static LoadType GetBFEMLoadType(ELCType LoadCaseType)
        {
            LoadType loadtype = LoadType.Other;
            if (LoadCaseType == ELCType.ePermanentLoad)
                loadtype = LoadType.Dead;
            else if (LoadCaseType == ELCType.eImposedLoad_LT || LoadCaseType == ELCType.eImposedLoad_ST)
                loadtype = LoadType.Live;
            else if (LoadCaseType == ELCType.eSnow)
                loadtype = LoadType.Snow;
            else if (LoadCaseType == ELCType.eWind)
                loadtype = LoadType.Wind;
            else if (LoadCaseType == ELCType.eEarthquake)
                loadtype = LoadType.Quake;

            return loadtype;
        }

        private static Load GetBFEMLoad(CMLoad memberLoad, LoadCase loadCase)
        {
            // BFEMNet ma tri typy - concentrated, uniform, trapezoidal load, dopracoval som partial uniform load
            var lu = new UniformLoad1D();
            var lpu = new PartialUniformLoad1D();

            if (memberLoad is BaseClasses.CMLoad_21) // Uniform load per whole member
            {
                lu = new UniformLoad1D();
                BaseClasses.CMLoad_21 load = new BaseClasses.CMLoad_21();
                // Create member load
                if (memberLoad.MLoadType == BaseClasses.ELoadType.eLT_F && memberLoad is BaseClasses.CMLoad_21)
                    load = (BaseClasses.CMLoad_21)memberLoad;

                // TODO - nastavit spravny smer a system v ktorom je zatazenie zadane
                // Skontrolovat zadanie v GCS a LCS

                CoordinationSystem eCS;
                if (load.ELoadCS == BaseClasses.ELoadCoordSystem.eGCS)
                    eCS = CoordinationSystem.Global;
                else // if (load.ELoadCS == ELoadCoordSystem.eLCS || load.ELoadCS == ELoadCoordSystem.ePCS)
                    eCS = CoordinationSystem.Local;

                LoadDirection eLD;

                if (load.ELoadDir == BaseClasses.ELoadDirection.eLD_X)
                    eLD = LoadDirection.X;
                else if (load.ELoadDir == BaseClasses.ELoadDirection.eLD_Y)
                    eLD = LoadDirection.Y;
                else //if (load.EDirection == ELoadDirection.eLD_Z)
                    eLD = LoadDirection.Z;

                lu = new UniformLoad1D(load.Fq, eLD, eCS, loadCase);
                return lu;
            }
            else if (memberLoad is BaseClasses.CMLoad_24) // Uniform load on segment
            {
                lpu = new PartialUniformLoad1D();
                BaseClasses.CMLoad_24 load = new BaseClasses.CMLoad_24();

                // Create member load
                if (memberLoad.MLoadType == BaseClasses.ELoadType.eLT_F && memberLoad is BaseClasses.CMLoad_24)
                    load = (BaseClasses.CMLoad_24)memberLoad;

                // TODO - nastavit spravny smer a system v ktorom je zatazenie zadane
                // Skontrolovat zadanie v GCS a LCS

                CoordinationSystem eCS;
                if (load.ELoadCS == BaseClasses.ELoadCoordSystem.eGCS)
                    eCS = CoordinationSystem.Global;
                else // if (load.ELoadCS == ELoadCoordSystem.eLCS || load.ELoadCS == ELoadCoordSystem.ePCS)
                    eCS = CoordinationSystem.Local;

                LoadDirection eLD;

                if (load.ELoadDir == BaseClasses.ELoadDirection.eLD_X)
                    eLD = LoadDirection.X;
                else if (load.ELoadDir == BaseClasses.ELoadDirection.eLD_Y)
                    eLD = LoadDirection.Y;
                else //if (load.ELoadDir == ELoadDirection.eLD_Z)
                    eLD = LoadDirection.Z;

                // PartialTrapezoidalLoad
                // TODO - toto by sme potrebovali, pisu tam ze je to obsolete ale na internete je uz priklad
                // Urcite mame najnovsie zdroje?, resp. to mozno mali v starsej verzii a skryli to
                // https://bfenet.readthedocs.io/en/latest/loads/elementLoads/trapezoidalload.html
                // https://media.readthedocs.org/pdf/bfenet/latest/bfenet.pdf

                // Isolocation [-1,1]
                double dStartIsoLocation;
                double dEndIsoLocation;

                // Prevod z absolutnych suradnic [0,L] na relativne [-1,1]
                if (load.FaA < 0.5 * load.Member.FLength)
                    dStartIsoLocation = -(0.5 * load.Member.FLength - load.FaA) / (0.5 * load.Member.FLength);
                else
                    dStartIsoLocation = (load.FaA - 0.5 * load.Member.FLength) / (0.5 * load.Member.FLength);

                if ((load.FaA + load.Fs) < 0.5 * load.Member.FLength)
                    dEndIsoLocation = -(0.5 * load.Member.FLength - (load.FaA + load.Fs)) / (0.5 * load.Member.FLength);
                else
                    dEndIsoLocation = ((load.FaA + load.Fs) - 0.5 * load.Member.FLength) / (0.5 * load.Member.FLength);

                lpu = new PartialUniformLoad1D(load.Fq, dStartIsoLocation, dEndIsoLocation, eLD, eCS, loadCase);
                return lpu;
            }
            else
            {
                // Not implemented load type
                // l = new UniformLoad1D();
            }
            return null;
        }

    }
}
