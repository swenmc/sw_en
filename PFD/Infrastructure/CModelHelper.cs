using BaseClasses;
using MATH;
using System;
using System.Collections.Generic;
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
                    if (m.EMemberType != EMemberType_FormSteel.eMC && m.EMemberType != EMemberType_FormSteel.eMR) continue;

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
            CNSupport support = model.m_arrNSupports[0];
            support.m_iNodeCollection = new int[2];
            support.m_iNodeCollection[0] = 1;
            support.m_iNodeCollection[1] = 5;

            frameSupports.Add(support);

            return frameSupports;
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
    }
}
