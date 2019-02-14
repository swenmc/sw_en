using BaseClasses.GraphObj.Objects_3D;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public static class CLoadGenerator
    {

        //// TODO - Ondrej, pripravit staticku triedu a metody pre generovanie member load zo surface load v zlozke Loading
        //float fMemberLoadValue = ((CSLoad_FreeUniform)m_arrLoadCases[01].SurfaceLoadsList[0]).fValue * fDist_Purlin;

        //    foreach (CMember m in listOfPurlins)
        //    {
        //        m_arrLoadCases[01].MemberLoadsList = new List<CMLoad>();
        //        m_arrLoadCases[01].MemberLoadsList.Add(new CMLoad_21(fMemberLoadValue, m, EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
        //    }

        public static void GenerateMemberLoads(CLoadCase[] m_arrLoadCases, List<CMember> members, float fDist)
        {
            foreach (CLoadCase lc in m_arrLoadCases)
            {
                int iLoadID_Temp = 1;
                foreach (CSLoad_Free csload in lc.SurfaceLoadsList)
                {
                    if (csload is CSLoad_FreeUniform)
                    {
                        float fMemberLoadValue = ((CSLoad_FreeUniform)csload).fValue * fDist;
                        foreach (CMember m in members)
                        {
                            lc.MemberLoadsList.Add(new CMLoad_21(iLoadID_Temp, fMemberLoadValue, m, EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
                            iLoadID_Temp += 1;
                        }
                    }
                    if (csload is CSLoad_FreeUniformGroup)
                    {
                        CSLoad_FreeUniformGroup group = (CSLoad_FreeUniformGroup)csload;
                        foreach (CSLoad_FreeUniform csloadFree in group.LoadList)
                        {
                            float fMemberLoadValue = csloadFree.fValue * fDist;
                            foreach (CMember m in members)
                            {
                                lc.MemberLoadsList.Add(new CMLoad_21(iLoadID_Temp, fMemberLoadValue, m, EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
                                iLoadID_Temp += 1;
                            }
                        }
                    }
                }
            }
        }
        
        //public static void GenerateMemberLoads(CLoadCase loadCase, List<CMember> members, float fDist)
        //{
        //    foreach (CSLoad_Free csload in loadCase.SurfaceLoadsList)
        //    {
        //        float fMemberLoadValue = ((CSLoad_FreeUniform)csload).fValue * fDist;
        //        foreach (CMember m in members)
        //        {
        //            loadCase.MemberLoadsList.Add(new CMLoad_21(fMemberLoadValue, m, EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0));
        //        }
        //    }
        //}

    }
}
