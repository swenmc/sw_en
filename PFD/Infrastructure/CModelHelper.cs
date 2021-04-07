using BaseClasses;
using BaseClasses.GraphObj;
using BaseClasses.Helpers;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace PFD
{
    public static class CModelHelper
    {
        const bool debugging = false;
        //Extension method
        //returns list of frames with members from Model
        public static List<CFrame> GetFramesFromModel(this CModel_PFD model)
        {
            double limit = 0.0000001;
            List<CFrame> frames = new List<CFrame>();

            for (int i = 0; i < model.iFrameNo; i++)
            {
                List<CMember> frameMembers = new List<CMember>();
                List<CNode> frameNodes = new List<CNode>();

                //// Add nodes to the frame
                //int iFrameNodesNo = model.iFrameNodesNo; // Number of nodes in frame
                //frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 0]);
                //frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 1]);
                //frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 2]);
                //frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 3]);

                //// Gable roof
                //if (model is CModel_PFD_01_GR)
                //    frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 4]);

                // Canopy
                // TODO 680 - dopracovat načítanie uzlov Canopy
                // Dostat sa ku canopy vo vm alebo pridat canopy properties do CModel_PFD podobne ako
                // ObservableCollection<DoorProperties> DoorBlocksProperties;

                // vid zoznamy FrameIndexList_Left a FrameIndexList_Right
                // Potrebujeme sa dopracovat k ID uzla ktory bol na danom rame vytvoreny pre Rafter - Canopy na lavej alebo pravej strane

                // Da sa to urobit aj tak, ze sa vyberu pruty s danou suradnicou Y pre vybrany frame a urobi sa zjednotenie skupiny ich koncovych uzlov, duplicitne uzly sa odstrania

                /*
                if (model.canopyList[i].IsLeft)
                {
                    if (model is CModel_PFD_01_GR)
                        frameNodes.Add(model.m_arrNodes[nx1]);
                    else
                        frameNodes.Add(model.m_arrNodes[nx2]);
                }

                if (model.canopyList[i].IsRight)
                {
                    if (model is CModel_PFD_01_GR)
                        frameNodes.Add(model.m_arrNodes[ny1]);
                    else
                        frameNodes.Add(model.m_arrNodes[ny2]);
                }
                */

                // Add members to the frame
                foreach (CMember m in model.m_arrMembers)
                {
                    // It is not Main Column and it is not Main rafter
                    if (m.EMemberType != EMemberType_FS.eMC && m.EMemberType != EMemberType_FS.eMR && m.EMemberType != EMemberType_FS.eEC && m.EMemberType != EMemberType_FS.eER) continue;

                    //task 600
                    //if (MathF.d_equal(m.PointStart.Y, i * model.fL1_frame, limit))
                    if (MathF.d_equal(m.PointStart.Y, model.GetBaysWidthUntilFrameIndex(i), limit))
                    {
                        frameMembers.Add(m);
                        //System.Diagnostics.Trace.WriteLine($"ID: {m.ID}, Name: {m.Name}, {m.PointStart.Y}");
                        if (!frameNodes.Contains(m.NodeStart)) frameNodes.Add(m.NodeStart);
                        //else System.Diagnostics.Trace.WriteLine($"ID: {m.ID}, Name: {m.Name}, {m.PointStart.Y}, NodeStart exists {m.NodeStart.ID}");
                        if (!frameNodes.Contains(m.NodeEnd)) frameNodes.Add(m.NodeEnd);
                        //else System.Diagnostics.Trace.WriteLine($"ID: {m.ID}, Name: {m.Name}, {m.PointStart.Y}, NodeEnd exists {m.NodeStart.ID}");
                    }
                    
                }

                // TODO 680 - odstranit duplicitne uzly s identickymi suradnicami, nahradit ID v prutoch a pripadne dalsich zoznamoch

                List<CLoadCase> frameLoadCases = CModelHelper.GetLoadCasesForNodesAndMembers(frameNodes, frameMembers, model.m_arrLoadCases);
                List<CLoadCombination> frameLoadCombinations = CModelHelper.GetLoadCombinationsForMembers(frameLoadCases.ToArray(), model.m_arrLoadCombs);
                List<CNSupport> frameSupports = model.GetFrameCNSupports();
                CFrame frame = new CFrame(model.eKitset, model.fRoofPitch_rad, frameMembers.ToArray(), frameNodes.ToArray(), frameLoadCases.ToArray(), frameLoadCombinations.ToArray(), frameSupports.ToArray());

                frames.Add(frame);
            }
            return frames;
        }

        public static List<CBeam_Simple> GetMembersFromModel(this CModel_PFD model)
        {
            List<CBeam_Simple> simpleBeams = new List<CBeam_Simple>();

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                if (!model.m_arrMembers[i].BIsSelectedForIFCalculation) continue; //member is not selected for calculations

                CMember simpleBeamMember = new CMember();
                List<CNode> simpleBeamNodes = new List<CNode>();

                // Add nodes to the beam

                simpleBeamNodes.Add(new CNode(model.m_arrMembers[i].NodeStart.ID, 0, 0, 0, 0));
                simpleBeamNodes.Add(new CNode(model.m_arrMembers[i].NodeEnd.ID, model.m_arrMembers[i].FLength, 0, 0, 0));

                // Create Member Is case that simple beam model should be created for member with specific member type
                // Purlin, Eave Purlin, Girts, Columns (wind posts)
                if (model.m_arrMembers[i].BIsDisplayed && // TODO - nemusi byt zobrazeny ale mal by ist do vypoctu BCalculate = true
                    (model.m_arrMembers[i].EMemberType == EMemberType_FS.eP ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eEP ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eG ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eWP))
                {
                    simpleBeamMember = model.m_arrMembers[i].CloneMember();
                    simpleBeamMember.NodeStart = simpleBeamNodes[0];
                    simpleBeamMember.NodeEnd = simpleBeamNodes[1];

                    // Validate - pridat len prut s priradenym prierezom
                    if (simpleBeamMember.CrScStart == null)
                        continue;

                    List<CLoadCase> simpleBeamLoadCases = CModelHelper.GetLoadCasesForNodesAndMembers(simpleBeamNodes, new List<CMember> { simpleBeamMember }, model.m_arrLoadCases);
                    List<CLoadCombination> simpleBeamLoadCombinations = CModelHelper.GetLoadCombinationsForMembers(simpleBeamLoadCases.ToArray(), model.m_arrLoadCombs);
                    List<CNSupport> simpleBeamSupports = model.GetSimpleBeamCNSupports();
                    CBeam_Simple beam = new CBeam_Simple(simpleBeamMember, simpleBeamNodes.ToArray(), simpleBeamLoadCases.ToArray(), simpleBeamLoadCombinations.ToArray(), simpleBeamSupports.ToArray());

                    simpleBeams.Add(beam);
                }
            }
            return simpleBeams;
        }
        public static int GetSimpleBeamsCount(this CModel_PFD model)
        {
            int simpleBeamsCount = 0;
            

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                if (!model.m_arrMembers[i].BIsSelectedForIFCalculation) continue; //member is not selected for calculations
                
                // Purlin, Eave Purlin, Girts, Columns (wind posts)
                if (model.m_arrMembers[i].BIsDisplayed && // TODO - nemusi byt zobrazeny ale mal by ist do vypoctu BCalculate = true
                    (model.m_arrMembers[i].EMemberType == EMemberType_FS.eP ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eEP ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eG ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eWP))
                {
                    // Validate - pridat len prut s priradenym prierezom
                    if (model.m_arrMembers[i].CrScStart == null)
                        continue;
                    
                    simpleBeamsCount++;
                }
            }
            return simpleBeamsCount;
        }

        public static List<CNSupport> GetFrameCNSupports(this CModel_PFD model)
        {
            List<CNSupport> frameSupports = new List<CNSupport>();
            /*
            foreach (CNSupport support in model.m_arrNSupports)
            {
                foreach (CMember m in frame.Members)
                {
                    // TO Ondrej
                    // Povodna predstava bola taka ze bude existovat tzv typovy objekt CNSupport, ktory bude mat definovane nejake parametre
                    // a v kolekcii m_iNodeCollection bude zoznam uzlov ktorym je priradeny
                    // Potom som to vsak nevedel vykreslit na viacerych uzloch naraz a tak som vytvaral objekt CNSupport pre kazdy uzol
                    // Je to priradene v CNSupport -> Node
                    // Do buducna by to chcelo urobit tak ako sa programatorsky patri :-)
                    //if (support.m_iNodeCollection != null && (support.m_iNodeCollection.Contains(m.NodeStart.ID) || support.m_iNodeCollection.Contains(m.NodeEnd.ID)))
                    //    frameSupports.Add(support);

                    // Aktualne plati zhruba toto
                    if(support.Node == m.NodeStart || support.Node == m.NodeEnd) // Add support to the list
                    {
                        // TO Ondrej - Este by sa malo osetrit ze podpora, ktora je na konci nejakeho pruta a zaroven na zaciatku ineho pruta (je teda priradena End Node nejakeho pruta ktory je zaroven Start Node ineho), ktory je k nemu pripojeny bola pridana len raz

                        // Docasne - naplnime aj kolekciu

                        if (support.Node == m.NodeStart)
                        {
                            support.m_iNodeCollection = new int[1];
                            support.m_iNodeCollection[0] = m.NodeStart.ID;
                        }

                        if (support.Node == m.NodeEnd)
                        {
                            support.m_iNodeCollection = new int[1];
                            support.m_iNodeCollection[0] = m.NodeEnd.ID;
                        }
                        
                    }
                }
            }
            */

            // TODO - TEMPORARY - natvrdo vyrobena podpora a pridany do kolekcie prvy a posledny uzol ramu

            // Vyrobime podporu v 2D (rovina XY, rotacia okolo Z) z podpory v 3D (rovina XZ,rotacia okolo Y)

            // Pre ramy pouzijeme prvu existujucu podporu is indexom [0] // Docasne riesenie

            bool[] bRestrain = new bool[3];
            bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Ux] = model.m_arrNSupports[0].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Ux];
            bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Uy] = model.m_arrNSupports[0].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Uz];
            bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Rz] = model.m_arrNSupports[0].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Ry];

            CNSupport support = new CNSupport((int)ENDOF.e2DEnv, 1, null, bRestrain, 0);
            support.m_iNodeCollection = new int[2];
            support.m_iNodeCollection[0] = 1;

            // Gable roof
            if (model is CModel_PFD_01_MR)
                support.m_iNodeCollection[1] = 4;
            else if (model is CModel_PFD_01_GR)
                support.m_iNodeCollection[1] = 5;
            else
            {
                support = null;
                throw new Exception("Kitset model is not implemented.");
            }

            frameSupports.Add(support);

            return frameSupports;
        }

        public static List<CNSupport> GetSimpleBeamCNSupports(this CModel_PFD model)
        {
            List<CNSupport> simpleBeamSupports = new List<CNSupport>();

            // Vyrobime podporu v 2D (rovina XY, rotacia okolo Z) z podpory v 3D (rovina XZ,rotacia okolo Y)

            bool[] bRestrain1 = new bool[6];
            bRestrain1[(int)BaseClasses.ENSupportType.eNST_Ux] = true;
            bRestrain1[(int)BaseClasses.ENSupportType.eNST_Uy] = true;
            bRestrain1[(int)BaseClasses.ENSupportType.eNST_Uz] = true;
            bRestrain1[(int)BaseClasses.ENSupportType.eNST_Rx] = true;
            bRestrain1[(int)BaseClasses.ENSupportType.eNST_Ry] = false;
            bRestrain1[(int)BaseClasses.ENSupportType.eNST_Rz] = false;

            bool[] bRestrain2 = new bool[6];
            bRestrain2[(int)BaseClasses.ENSupportType.eNST_Ux] = false;
            bRestrain2[(int)BaseClasses.ENSupportType.eNST_Uy] = true;
            bRestrain2[(int)BaseClasses.ENSupportType.eNST_Uz] = true;
            bRestrain2[(int)BaseClasses.ENSupportType.eNST_Rx] = true;
            bRestrain2[(int)BaseClasses.ENSupportType.eNST_Ry] = false;
            bRestrain2[(int)BaseClasses.ENSupportType.eNST_Rz] = false;

            CNSupport support1 = new CNSupport((int)ENDOF.e3DEnv, 1, null, bRestrain1, 0);
            CNSupport support2 = new CNSupport((int)ENDOF.e3DEnv, 2, null, bRestrain2, 0);

            support1.m_iNodeCollection = new int[1];
            support1.m_iNodeCollection[0] = 1; // Start Node

            support2.m_iNodeCollection = new int[1]; // End Node
            support2.m_iNodeCollection[0] = 2;

            simpleBeamSupports.Add(support1);
            simpleBeamSupports.Add(support2);

            return simpleBeamSupports;
        }

        public static List<CLoadCase> GetLoadCasesForNodesAndMembers(List<CNode> nodes, List<CMember> members, CLoadCase[] allLoadCases)
        {
            List<CLoadCase> listOfOriginalLoadCases = allLoadCases.ToList();
            List<CLoadCase> listOfNewLoadCases = new List<CLoadCase>();
            foreach (CLoadCase lc in listOfOriginalLoadCases)
            {
                // Create Copy of Load Case
                CLoadCase lc_new = new CLoadCase();
                lc_new.ID = lc.ID;
                lc_new.Prefix = lc.Prefix;
                lc_new.Name = lc.Name;
                lc_new.MType_LS = lc.MType_LS;
                lc_new.Type = lc.Type;
                lc_new.MainDirection = lc.MainDirection;
                lc_new.Factor = lc.Factor;
                lc_new.NodeLoadsList = lc.NodeLoadsList;
                lc_new.MemberLoadsList = lc.MemberLoadsList;
                lc_new.SurfaceLoadsList = lc.SurfaceLoadsList;

                // Nodal Loads
                List<CNLoad> listOfOriginalNodalLoads = lc.NodeLoadsList;
                List<CNLoad> listOfNewNodalLoads = new List<CNLoad>();

                foreach (CNLoad load in listOfOriginalNodalLoads)
                {
                    if (nodes.Exists(n => n.ID == load.Node.ID)) listOfNewNodalLoads.Add(load);
                }

                // Member Loads
                List<CMLoad> listOfOriginalMemberLoads = lc.MemberLoadsList;
                List<CMLoad> listOfNewMemberLoads = new List<CMLoad>();

                foreach (CMLoad load in listOfOriginalMemberLoads)
                {
                    if (members.Exists(m => m.ID == load.Member.ID)) listOfNewMemberLoads.Add(load);
                }

                // TO Ondrej
                // if (loads.Count > 0) - Tu si nie som isty ci je dobre vyrabat len load case ktory obsahuje nejake zatazenie, je to sice v istom zmysle optimalizacia
                // ale mozu s tym byt problemy v tom ze load case ktory existuje v 3D modeli nebude existovat v 2D modeli
                // a potom v kombinaciach napriklad CO1 = 1.5 * LC1 + 1.4 * LC2 budeme musiet osetrit ze ak v LC2 nebolo zatazenie pruta
                // tak sme tento LC2 nevyrobili a nepocitali a vysledok kombinacie je CO1 = 1.5 * LC1
                // Osetrit to a zosynchronizovat to v BFENet a vo vystupe vysledkov moze byt dost pracne

                // Ja by som to zatial urobil tak ze sa budu vyrabat a pocitat aj "prazdne" load cases, aby sa z BFENet dali tahat vysledky pre kombinacie
                // ktore taketo LC obsahuju

                // Padalo to pri vytvarani load combinations, zatial som to zakomentoval.
                // Pre ram sa teda vytvaraju sa vsetky Load Cases aj ked su prazdne

                //if (loads.Count > 0)
                //{
                lc_new.NodeLoadsList = listOfNewNodalLoads;
                lc_new.SurfaceLoadsList = null;
                lc_new.MemberLoadsList = listOfNewMemberLoads;
                listOfNewLoadCases.Add(lc_new);
                //}
            }
            return listOfNewLoadCases;
        }

        public static List<CLoadCombination> GetLoadCombinationsForMembers(CLoadCase[] framememberLoadCases, CLoadCombination[] allLoadCombinations)
        {
            List<CLoadCase> memberLoadCases = framememberLoadCases.ToList();
            List<CLoadCombination> membersLoadCombinations = new List<CLoadCombination>();
            foreach (CLoadCombination lcomb in allLoadCombinations.ToList())
            {
                CLoadCombination newloadcombination = new CLoadCombination();

                // Do novej kombinacie nastavime vsetky parametre povodnej, asi by sa to dalo krajsie :)
                // Iny bude len zoznam load cases (load cases z 3D modelu su nahradene load cases z modelu ramu)
                newloadcombination.ID = lcomb.ID;
                newloadcombination.Name = lcomb.Name;
                newloadcombination.Prefix = lcomb.Prefix;
                newloadcombination.eLComType = lcomb.eLComType;
                newloadcombination.CombinationKey = lcomb.CombinationKey;
                newloadcombination.Formula = lcomb.Formula;

                for (int i = 0; i < lcomb.LoadCasesList.Count; i++) // Asi sa to da zapisat jednoduchsie, najst vsetky load cases ktorych ID je v kombinacii a nastavit namiesto nich zatazovaci stav z allLoadCases, load factor by mal zostat rovnaky
                {
                    for (int j = 0; j < memberLoadCases.Count; j++)
                    {
                        if (lcomb.LoadCasesList[i].Name == memberLoadCases[j].Name) // Dany load case v originalnej kombinacii je rovnaky ako load case v memberLoadCases
                        {
                            // Set Factor and Frame Model Load Case
                            newloadcombination.LoadCasesFactorsList.Add(lcomb.LoadCasesFactorsList[i]);
                            newloadcombination.LoadCasesList.Add(memberLoadCases[j]);
                            break; // Load case can't be added many times.
                        }
                    }
                }

                membersLoadCombinations.Add(newloadcombination);
            }
            return membersLoadCombinations;
        }

        //podla ID pruta treba identifikovat do ktoreho ramu patri
        public static int GetFrameIndexForMember(CMember m, List<CFrame> frames)
        {
            // Validate argument
            if (m == null || frames == null)
                return -1;

            for (int i = 0; i < frames.Count; i++)
            {
                if (Array.Exists(frames[i].m_arrMembers, mem => mem.ID == m.ID)) return i;
            }

            return -1; //not found
        }

        //podla ID pruta treba identifikovat do ktoreho zaznamu Simple Beam patri
        public static int GetSimpleBeamIndexForMember(CMember m, List<CBeam_Simple> simpleBeams)
        {
            // Validate argument
            if (m == null || simpleBeams == null)
                return -1;

            for (int i = 0; i < simpleBeams.Count; i++)
            {
                if (Array.Exists(simpleBeams[i].m_arrMembers, mem => mem.ID == m.ID)) return i;
            }

            return -1; //not found
        }

        public static int GetNodeIndexInFrame(this CFrame frame, CNode n)
        {
            for (int i = 0; i < frame.m_arrNodes.Length; i++)
            {
                if (frame.m_arrNodes[i].ID == n.ID) return i;
            }

            return -1; //not found
        }

        //podla ID pruta a indexu ramu treba identifikovat do ktoreho ramu prut z globalneho modelu patri a ktory prut v rame mu odpoveda
        public static int GetMemberIndexInFrame(this CFrame frame, CMember m)
        {
            for (int i = 0; i < frame.m_arrMembers.Length; i++)
            {
                if (frame.m_arrMembers[i].ID == m.ID) return i;
            }

            return -1; //not found
        }

        public static int GetLoadCombinationIndex(this CModel model, int id)
        {
            for (int i = 0; i < model.m_arrLoadCombs.Length; i++)
            {
                if (model.m_arrLoadCombs[i].ID == id) return i;
            }

            return -1; //not found
        }

        public static CLoadCombination GetLoadCombinationWithID(this CModel model, int id)
        {
            for (int i = 0; i < model.m_arrLoadCombs.Length; i++)
            {
                if (model.m_arrLoadCombs[i].ID == id) return model.m_arrLoadCombs[i];
            }

            return null;
        }

        //public static CFoundation GetFoundationForJointFromModel(this CModel model, CConnectionJointTypes joint)
        //{
        //    foreach (CFoundation f in model.m_arrFoundations)
        //    {
        //        if (joint.m_Node.ID == f.m_Node.ID) return f;
        //    }
        //    return null;
        //}

        /// <summary>
        /// Method for model validation. Tries to find IDs duplicates...
        /// </summary>
        /// <param name="model">Main model</param>
        public static void ValidateModel(this CModel_PFD model)
        {
            //find duplicate Member IDs
            var duplicateMembers = model.m_arrMembers.GroupBy(m => m.ID).Select(g => new { Count = g.Count(), ID = g.Key }).Where(g => g.Count > 1);
            foreach (var duplicateM in duplicateMembers)
            {
                List<CMember> errorMembers = model.m_arrMembers.Where(m => m.ID == duplicateM.ID).ToList();
                string s = "";
                foreach (CMember m in errorMembers)
                {
                    for (int i = 0; i < model.m_arrMembers.Length; i++)
                    {
                        s += i + ", ";
                    }
                }
                throw new Exception($"Member With same ID [{duplicateM.ID}] are at indexes: {s}");
            }

            //find duplicate Nodes IDs
            var duplicateNodes = model.m_arrNodes.GroupBy(m => m.ID).Select(g => new { Count = g.Count(), ID = g.Key }).Where(g => g.Count > 1);
            foreach (var duplicateNode in duplicateNodes)
            {
                List<CNode> errorNodes = model.m_arrNodes.Where(m => m.ID == duplicateNode.ID).ToList();
                string s = "";
                foreach (CNode m in errorNodes)
                {
                    for (int i = 0; i < model.m_arrNodes.Length; i++)
                    {
                        s += i + ", ";
                    }
                }
                throw new Exception($"Nodes With same ID [{duplicateNode.ID}] are at indexes: {s}");
            }
        }

        public static void SetMembersAccordingTo(CMember[] m_arrMembers, ObservableCollection<CComponentInfo> componentList)
        {
            int count = 0;

            foreach (CMember m in m_arrMembers)
            {
                foreach (CComponentInfo cInfo in componentList)
                {
                    // Ak deaktivujeme prut kvoli tomu, ze bol na jeho miesto vlozeny blok, tak tu mu uz nesmieme nastavit ze je znova aktivny
                    // Myslel som ze taky prut bude mat nastavene BIsGenerated na false ale bude v m_arrMembers existovat, aby mi sedeli cisla pri generovani prutov blokov atd
                    //if (m.Prefix == cInfo.Prefix &&
                    //    m.CrScStart.ID == (int)cInfo.MemberTypePosition &&
                    if (m.EMemberTypePosition == cInfo.MemberTypePosition && //takto to chcem pouzit, Mato skusis kazdemu member nastavit typ?
                        m.BIsGenerated) // !!! Set member properties only for already generated members - deactivated members (especially girts in place where block is inserted) shouldn't be activated
                    {
                        count++;
                        // Assign component properties from GUI component list to the particular members in the model
                        m.BIsGenerated = cInfo.Generate.GetValueOrDefault(false);

                        // Door and window blocks
                        // cInfo.Generate = null for block components, set true
                        if (m.EMemberTypePosition == EMemberType_FS_Position.WindowFrame ||
                            m.EMemberTypePosition == EMemberType_FS_Position.DoorFrame ||
                            m.EMemberTypePosition == EMemberType_FS_Position.DoorLintel ||
                            m.EMemberTypePosition ==  EMemberType_FS_Position.DoorTrimmer)
                        {
                            m.BIsGenerated = true;
                        }

                        // Canopies
                        if(m.EMemberTypePosition == EMemberType_FS_Position.MainRafterCanopy ||
                           m.EMemberTypePosition == EMemberType_FS_Position.EdgeRafterCanopy ||
                           m.EMemberTypePosition == EMemberType_FS_Position.PurlinCanopy ||
                           m.EMemberTypePosition == EMemberType_FS_Position.BracingBlockPurlinsCanopy ||
                           m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingRoofCanopy)
                        {
                            m.BIsGenerated = true;
                        }

                        //task 674
                        if (cInfo.MemberTypePosition == EMemberType_FS_Position.Girt)
                        {
                            if (m.IsLeftGirt() && cInfo.ComponentName.EndsWith("Right Side")) continue; //skip settings
                            if (m.IsRightGirt() && cInfo.ComponentName.EndsWith("Left Side")) continue; //skip settings
                        }

                        m.BIsDisplayed = cInfo.Display; //nastavenie zobrazenia pre dany member
                        m.BIsSelectedForIFCalculation = cInfo.Calculate;
                        m.BIsSelectedForDesign = cInfo.Design;
                        m.BIsSelectedForMaterialList = cInfo.MaterialList;

                        // Set Member Color
                        //m.Color = cInfo.Color.Color.Value;  //(Color)ColorConverter.ConvertFromString(cInfo.Color.Name);
                        m.Color = (Color)ColorConverter.ConvertFromString(cInfo.Color.Name);
                        if (m.CrScStart != null) m.CrScStart.CSColor = (Color)ColorConverter.ConvertFromString(cInfo.SectionColor);
                        if (debugging) System.Diagnostics.Trace.WriteLine("Prefix: " + m.Prefix + ", "+ m.EMemberTypePosition + ", " + m.BIsGenerated + ", " + m.BIsDisplayed + ", " + cInfo.Color.Name);
                        break;
                    }
                    else if (m.Prefix == cInfo.Prefix &&
                        m.CrScStart.ID == (int)cInfo.MemberTypePosition && !m.BIsGenerated)
                    {
                        if (debugging) System.Diagnostics.Trace.WriteLine("Prefix: " + m.Prefix + ", " + m.BIsGenerated + ", " + m.BIsDisplayed);
                    }
                }
            }

            if (debugging) System.Diagnostics.Trace.WriteLine("SetMembersAccordingTo count: " + count);
        }

        public static int GetMembersSetForCalculationsCount(CMember[] arrMembers)
        {
            int count = 0;
            foreach (CMember m in arrMembers)
            {
                if (!m.BIsGenerated) continue;
                if (m.BIsSelectedForIFCalculation) count++;
            }
            return count;
        }
        public static int GetMembersSetForDesignCalculationsCount(CMember[] arrMembers)
        {
            int count = 0;
            foreach (CMember m in arrMembers)
            {
                if (!m.BIsGenerated) continue;
                if (m.BIsSelectedForDesign) count++;
            }
            return count;
        }


        public static void ChangeMembersMaterial(CComponentInfo cInfo, CModel model)
        {
            List<CMember> members = model.m_arrMembers.Where(m => m.EMemberTypePosition == cInfo.MemberTypePosition).ToList();
            foreach (CMember m in members)
            {
                if (m.CrScStart != null) m.CrScStart.m_Mat.Name = cInfo.Material;
                if (m.CrScEnd != null) m.CrScEnd.m_Mat.Name = cInfo.Material;
            }

        }


        //v tychto metodach robime to iste co robime v metode CModelHelper.SetMembersAccordingTo(m_arrMembers, componentList);
        public static void ChangeMembersIsSelectedForMaterialList(CComponentInfo cInfo, CModel model)
        {
            List<CMember> members = model.m_arrMembers.Where(m => m.EMemberTypePosition == cInfo.MemberTypePosition).ToList();
            foreach (CMember m in members)
            {
                if (cInfo.MemberTypePosition == EMemberType_FS_Position.Girt)
                {
                    if (m.IsLeftGirt() && cInfo.ComponentName.EndsWith("Right Side")) continue; //skip settings
                    if (m.IsRightGirt() && cInfo.ComponentName.EndsWith("Left Side")) continue; //skip settings
                }
                m.BIsSelectedForMaterialList = cInfo.MaterialList;
            }
        }
        public static void ChangeJointsIsSelectedForMaterialList(CComponentInfo cInfo, CModel model)
        {
            foreach (CConnectionJointTypes joint in model.m_arrConnectionJoints) // For each joint
            {
                joint.SetJointIsSelectedForMaterialListAccordingToMember();
            }
        }


        public static void ChangeMembersIsSelectedForMaterialList(CPFDViewModel vm)
        {
            foreach (CComponentInfo cInfo in vm.ComponentList)
            {
                foreach (CMember m in vm.Model.m_arrMembers)
                {
                    if (cInfo.MemberTypePosition == EMemberType_FS_Position.Girt)
                    {
                        if (m.IsLeftGirt() && cInfo.ComponentName.EndsWith("Right Side")) continue; //skip settings
                        if (m.IsRightGirt() && cInfo.ComponentName.EndsWith("Left Side")) continue; //skip settings
                    }

                    if (m.EMemberTypePosition == cInfo.MemberTypePosition)
                        m.BIsSelectedForMaterialList = cInfo.MaterialList;
                }
            }
        }

        public static CComponentInfo GetComponentInfo(ObservableCollection<CComponentInfo> componentList, EMemberType_FS_Position memberType)
        {
            return componentList.FirstOrDefault(c => c.MemberTypePosition == memberType);
        }
        public static string GetComponentInfoName(ObservableCollection<CComponentInfo> componentList, EMemberType_FS_Position memberType)
        {
            CComponentInfo ci = componentList.FirstOrDefault(c => c.MemberTypePosition == memberType);
            if (ci != null) return ci.ComponentName;
            else return string.Empty;
        }
        public static bool IsGenerateSet(ObservableCollection<CComponentInfo> componentList, EMemberType_FS_Position memberType)
        {
            CComponentInfo ci = componentList.FirstOrDefault(c => c.MemberTypePosition == memberType);
            if (ci == null) return false;
            if (ci.Generate == null) return false;
            return ci.Generate.Value;
        }


        public static List<CMember> GetMembersForNode(this CModel_PFD model, CNode node, bool IncludeMembersFromSamePositionNodes, bool IncludeIntermediateNodesMembers)
        {
            // Bug 715

            // TODO Ondrej - este asi budeme potrebovat toto rozsirit o jeden bool, ktory bude zohladnovat aj CNode, ktore su medzilahle na prute
            // To znamena, ze lezia na linii pruta, ale nie su na jeho zaciatku ani konci
            // Tieto uzly by mali byt v zozname model.m_arrMembers[i].IntermediateNodes
            // Akurat neviem, ci je tento zoznam vzdy uplne spravne naplneny pre vsetky pruty
            // Je totiz mozne ze medzilahle uzly vzniknu az potom co sa tento prut vytvori a uz sa mu do zoznamu nepriradia.

            IEnumerable<CMember> foundMembers = model.GetMembersForNode(node);
            List<CMember> members = foundMembers.ToList();

            if (IncludeMembersFromSamePositionNodes)
            {
                List<CNode> sameNodes = model.GetNodesWithSamePosition(node);
                foreach (CNode n in sameNodes)
                {
                    IEnumerable<CMember> newFoundMembers = model.GetMembersForNode(n);
                    foreach (CMember m in newFoundMembers)
                    {
                        if (members.Contains(m)) continue;
                        else members.Add(m);
                    }
                }
            }

            //najde aj members ktore maju v intermediate nodes tento dany Node
            if (IncludeIntermediateNodesMembers)
            {
                foreach (CMember m in model.m_arrMembers)
                {
                    if (m.IntermediateNodes.Contains(node))
                    {
                        if (members.Contains(m)) continue;
                        else members.Add(m);
                    }
                }
            }

            return members;
        }

        public static IEnumerable<CMember> GetMembersForNode(this CModel_PFD model, CNode node)
        {
            return model.m_arrMembers.Where(m => (m.NodeStart != null && m.NodeStart.Equals(node)) || (m.NodeEnd != null && m.NodeEnd.Equals(node)));
        }

        public static List<CNode> GetNodesWithSamePosition(this CModel_PFD model, CNode node)
        {
            List<CNode> nodes = new List<CNode>();
            foreach (CNode n in model.m_arrNodes)
            {
                if (n.ID == node.ID) continue; //it is the node in parameter = do not include

                if (ModelHelper.IsNodesLocationIdentical(n, node)) nodes.Add(n);
            }
            return nodes;
        }

        public static CNode GetNodeWithSamePositionButLowerID(CNode node, CNode[] nodes, bool deactivateDuplicitNodes = true)
        {
            CNode resNode = node;
            foreach (CNode n in nodes)
            {
                if (n == null) continue;
                if (n.ID == node.ID) continue; //it is the node in parameter = do not include

                if (ModelHelper.IsNodesLocationIdentical(n, node) && n.ID < node.ID)
                {
                    if (deactivateDuplicitNodes) node.BIsGenerated = false;
                    resNode = n;
                }
            }
            return resNode;
        }


        public static bool ModelHasCladding(CModel_PFD model)
        {
            if (model == null || model.m_arrGOCladding == null) return false;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return false;

            return cladding.HasCladdingSheets();
        }
        public static bool ModelHasCladding_Roof(CModel_PFD model)
        {
            if (model == null || model.m_arrGOCladding == null) return false;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return false;

            return cladding.HasCladdingSheets_Roof();
        }
        public static bool ModelHasCladding_Wall(CModel_PFD model)
        {
            if (model == null || model.m_arrGOCladding == null) return false;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return false;

            return cladding.HasCladdingSheets_Wall();
        }
        public static bool ModelHasFibreglass(CModel_PFD model)
        {
            if (model == null || model.m_arrGOCladding == null) return false;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return false;

            return cladding.HasFibreglassSheets();
        }
        public static bool ModelHasFibreglass_Roof(CModel_PFD model)
        {
            if (model == null || model.m_arrGOCladding == null) return false;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return false;

            return cladding.HasFibreglassSheets_Roof();
        }
        public static bool ModelHasFibreglass_Wall(CModel_PFD model)
        {
            if (model == null || model.m_arrGOCladding == null) return false;
            CCladding cladding = model.m_arrGOCladding.FirstOrDefault();
            if (cladding == null) return false;

            return cladding.HasFibreglassSheets_Wall();
        }
    }
}
