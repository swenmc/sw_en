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
            foreach (CNSupport support in model.m_arrNSupports)
            {
                foreach (CMember m in frame.Members)
                {
                    if (support.m_iNodeCollection.Contains(m.NodeStart.ID) || support.m_iNodeCollection.Contains(m.NodeEnd.ID))
                        frameSupports.Add(support);
                    
                }
            }
            return frameSupports;
            
        }



    }
}
