using BaseClasses;
using MATH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PFD
{
    public static class CModelHelper
    {
        //Extension method
        //returns list of frames with members from Model
        public static List<CFrame> GetFramesFromModel(this CModel_PFD_01_GR model)
        {
            double limit = 0.0000001;
            List<CFrame> frames = new List<CFrame>();

            for (int i = 0; i < model.iFrameNo; i++)
            {
                List<CMember> frameMembers = new List<CMember>();
                List<CNode> frameNodes = new List<CNode>();

                // Add nodes to the frame
                int iFrameNodesNo = model.iFrameNodesNo; // Number of nodes in frame
                frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 0]);
                frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 1]);
                frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 2]);
                frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 3]);
                frameNodes.Add(model.m_arrNodes[i * iFrameNodesNo + 4]);

                // Add members to the frame
                foreach (CMember m in model.m_arrMembers)
                {
                    // It is not Main Column and it is not Main rafter
                    if (m.EMemberType != EMemberType_FS.eMC && m.EMemberType != EMemberType_FS.eMR && m.EMemberType != EMemberType_FS.eEC && m.EMemberType != EMemberType_FS.eER) continue;

                    if (MathF.d_equal(m.PointStart.Y, i * model.fL1_frame, limit))
                    {
                        frameMembers.Add(m);
                        //System.Diagnostics.Trace.WriteLine($"ID: {m.ID}, Name: {m.Name}, {m.PointStart.Y}");
                    }
                }

                List<CLoadCase> frameLoadCases = CModelHelper.GetLoadCasesForMembers(frameMembers, model.m_arrLoadCases);
                List<CLoadCombination> frameLoadCombinations = CModelHelper.GetLoadCombinationsForMembers(frameLoadCases.ToArray(), model.m_arrLoadCombs);
                List<CNSupport> frameSupports = model.GetFrameCNSupports();
                CFrame frame = new CFrame(frameMembers.ToArray(), frameNodes.ToArray(), frameLoadCases.ToArray(), frameLoadCombinations.ToArray(), frameSupports.ToArray());

                frames.Add(frame);
            }
            return frames;
        }

        public static List<CBeam_Simple> GetMembersFromModel(this CModel_PFD_01_GR model)
        {
            List<CBeam_Simple> simpleBeams = new List<CBeam_Simple>();

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                if (!model.m_arrMembers[i].BIsSelectedForIFCalculation) continue; //member is not selected for calculations

                CMember simpleBeamMember = new CMember();
                List<CNode> simpleBeamNodes = new List<CNode>();

                // Add nodes to the beam

                simpleBeamNodes.Add(new CNode(model.m_arrMembers[i].NodeStart.ID, 0,0,0,0));
                simpleBeamNodes.Add(new CNode(model.m_arrMembers[i].NodeEnd.ID, model.m_arrMembers[i].FLength,0,0,0));

                // Create Member Is case that simple beam model should be created for member with specific member type
                // Purlin, Eave Purlin, Girts, Columns (wind posts)
                if (model.m_arrMembers[i].BIsDisplayed && // TODO - nemusi byt zobrazeny ale mal by ist do vypoctu BCalculate = true
                    (model.m_arrMembers[i].EMemberType == EMemberType_FS.eP ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eEP ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eG ||
                    model.m_arrMembers[i].EMemberType == EMemberType_FS.eC))
                {
                    simpleBeamMember = model.m_arrMembers[i];
                    simpleBeamMember.NodeStart = simpleBeamNodes[0];
                    simpleBeamMember.NodeEnd = simpleBeamNodes[1];

                    // Validate - pridat len prut s priradenym prierezom
                    if (simpleBeamMember.CrScStart == null)
                        continue;

                    List<CLoadCase> simpleBeamLoadCases = CModelHelper.GetLoadCasesForMembers(new List<CMember> { simpleBeamMember }, model.m_arrLoadCases);
                    List<CLoadCombination> simpleBeamLoadCombinations = CModelHelper.GetLoadCombinationsForMembers(simpleBeamLoadCases.ToArray(), model.m_arrLoadCombs);
                    List<CNSupport> simpleBeamSupports = model.GetSimpleBeamCNSupports();
                    CBeam_Simple beam = new CBeam_Simple(simpleBeamMember, simpleBeamNodes.ToArray(), simpleBeamLoadCases.ToArray(), simpleBeamLoadCombinations.ToArray(), simpleBeamSupports.ToArray());

                    simpleBeams.Add(beam);
                }
            }
            return simpleBeams;
        }

        public static List<CNSupport> GetFrameCNSupports(this CModel_PFD_01_GR model)
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

            bool [] bRestrain = new bool[3];
            bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Ux] = model.m_arrNSupports[0].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Ux];
            bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Uy] = model.m_arrNSupports[0].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Uz];
            bRestrain[(int)BaseClasses.ENSupportType_2D.eNST_Rz] = model.m_arrNSupports[0].m_bRestrain[(int)BaseClasses.ENSupportType.eNST_Ry];

            CNSupport support = new CNSupport((int)ENDOF.e2DEnv, 1, null, bRestrain, 0);
            support.m_iNodeCollection = new int[2];
            support.m_iNodeCollection[0] = 1;
            support.m_iNodeCollection[1] = 5;

            frameSupports.Add(support);

            return frameSupports;
        }

        public static List<CNSupport> GetSimpleBeamCNSupports(this CModel_PFD_01_GR model)
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

        public static List<CLoadCase> GetLoadCasesForMembers(List<CMember> members, CLoadCase[] allLoadCases)
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
                    lc_new.NodeLoadsList = null;
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
                    for(int j = 0; j < memberLoadCases.Count; j++)
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

        /// <summary>
        /// Method for model validation. Tries to find IDs duplicates...
        /// </summary>
        /// <param name="model">Main model</param>
        public static void ValidateModel(this CModel_PFD_01_GR model)
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
                foreach(CComponentInfo cInfo in componentList)
                {
                    // Ak deaktivujeme prut kvoli tomu, ze bol na jeho miesto vlozeny blok, tak tu mu uz nesmieme nastavit ze je znova aktivny
                    // Myslel som ze taky prut bude mat nastavene BIsGenerated na false ale bude v m_arrMembers existovat, aby mi sedeli cisla pri generovani prutov blokov atd
                    if (m.Prefix == cInfo.Prefix &&
                        m.CrScStart.ID == (int)cInfo.MemberType && 
                        m.BIsGenerated) // !!! Set member properties only for already generated members - deactivated members (especially girts in place where block is inserted) shouldn't be activated
                    {
                        count++;
                        // Assign component properties from GUI component list to the particular members in the model
                        m.BIsGenerated = cInfo.Generate.GetValueOrDefault(false);

                        // cInfo.Generate = null for block components, set true
                        if (m.Prefix == "WF" || m.Prefix == "DF" || m.Prefix == "DL" || m.Prefix == "DT")
                            m.BIsGenerated = true;

                        m.BIsDisplayed = cInfo.Display; //nastavenie zobrazenia pre dany member
                        m.BIsSelectedForIFCalculation = cInfo.Calculate;
                        m.BIsSelectedForDesign = cInfo.Design;
                        m.BIsSelectedForMaterialList = cInfo.MaterialList;
                        break;
                    }                    
                }
            }
            System.Diagnostics.Trace.WriteLine("System.Diagnostics.Trace.WriteLine() count: " + count);
        }

        public static int GetMembersSetForCalculationsCount(CMember[] arrMembers)
        {
            int count = 0;
            foreach (CMember m in arrMembers)
            {
                if (m.BIsSelectedForIFCalculation) count++;
            }
            return count;
        }
    }
}
