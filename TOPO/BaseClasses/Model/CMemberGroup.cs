using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRSC;

namespace BaseClasses
{
    [Serializable]
    public class CMemberGroup : CEntityGroup
    {
        private List<CMember> MlistOfMembers;

        public List<CMember> ListOfMembers
        {
            get { return MlistOfMembers; }
            set { MlistOfMembers = value; }
        }

        private CCrSc McrossSection;

        public CCrSc CrossSection
        {
            get { return McrossSection; }
            set { McrossSection = value; }
        }

        public CMemberGroup()
        { }

        public CMemberGroup(int ID_temp, string sName_temp, CCrSc crossSection_temp, float fTime_temp)
        {
            ID = ID_temp;
            Name = sName_temp;
            CrossSection = crossSection_temp;
            FTime = fTime_temp;

            ListOfMembers = new List<CMember>();
        }

        public CMemberGroup(int ID_temp, string sName_temp, CCrSc crossSection_temp, List<CMember> memberList, float fTime_temp)
        {
            ID = ID_temp;
            Name = sName_temp;
            CrossSection = crossSection_temp;
            ListOfMembers = memberList;
            FTime = fTime_temp;
        }
    }
}
