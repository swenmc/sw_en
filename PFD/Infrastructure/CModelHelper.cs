using BaseClasses;
using MATH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                CFrame frame = new CFrame();

                foreach (CMemberGroup gr in model.listOfModelMemberGroups)
                {
                    foreach (CMember m in gr.ListOfMembers)
                    {
                        //it is not Main Column and it is not Main rafter
                        if (m.EMemberType != EMemberType_FormSteel.eMC && m.EMemberType != EMemberType_FormSteel.eMR) continue;

                        if (MathF.d_equal(m.PointStart.Y, i * model.fL1_frame, limit))
                        {
                            frame.Members.Add(m);
                            //System.Diagnostics.Trace.WriteLine($"ID: {m.ID}, Name: {m.Name}, {m.PointStart.Y}");
                        }
                    }
                }

                frames.Add(frame);
            }
            return frames;
        }


        public static List<CNSupport> GetFrameCNSupports(this CModel_PFD_01_GR model, CFrame frame)
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
            List<CLoadCase> membersLoadCases = new List<CLoadCase>();
            foreach (CLoadCase lc in allLoadCases.ToList())
            {
                List<CMLoad> loads = new List<CMLoad>();
                foreach (CMLoad load in lc.MemberLoadsList)
                {
                    if (members.Exists(m => m.ID == load.Member.ID)) loads.Add(load);
                }

                if (loads.Count > 0)
                {
                    lc.NodeLoadsList = null;
                    lc.SurfaceLoadsList = null;
                    lc.MemberLoadsList = loads;
                    membersLoadCases.Add(lc);
                }
            }
            return membersLoadCases;
        }

        

    }
}
