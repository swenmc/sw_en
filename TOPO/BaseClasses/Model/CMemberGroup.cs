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

        private EMemberType_FS MmemberType;
        public EMemberType_FS MemberType_FS
        {
            get { return MmemberType; }
            set { MmemberType = value; }
        }
        
        private EMemberType_FS_Position MmemberType_FS_Position;
        public EMemberType_FS_Position MemberType_FS_Position
        {
            get { return MmemberType_FS_Position; }
            set { MmemberType_FS_Position = value; }
        }

        private CCrSc McrossSection;

        public CCrSc CrossSection
        {
            get { return McrossSection; }
            set { McrossSection = value; }
        }

        private float MDeflectionLimit_PermanentLoad;

        public float DeflectionLimit_PermanentLoad
        {
            get { return MDeflectionLimit_PermanentLoad; }
            set { MDeflectionLimit_PermanentLoad = value; }
        }

        private float MDeflectionLimit_ImposedLoad;

        public float DeflectionLimit_ImposedLoad
        {
            get { return MDeflectionLimit_ImposedLoad; }
            set { MDeflectionLimit_ImposedLoad = value; }
        }

        private float MDeflectionLimit_Total;

        public float DeflectionLimit_Total
        {
            get { return MDeflectionLimit_Total; }
            set { MDeflectionLimit_Total = value; }
        }

        public CMemberGroup()
        { }

        public CMemberGroup(int ID_temp, string sName_temp, EMemberType_FS memberTypeFS_temp, EMemberType_FS_Position memberTypeFS_position, CCrSc crossSection_temp, float fDeflectionLimitPermanentLoad, float fDeflectionLimitTotal, float fTime_temp)
        {
            ID = ID_temp;
            Name = sName_temp;
            MemberType_FS = memberTypeFS_temp;
            MemberType_FS_Position = memberTypeFS_position;
            CrossSection = crossSection_temp;
            FTime = fTime_temp;
            MDeflectionLimit_PermanentLoad = fDeflectionLimitPermanentLoad;
            MDeflectionLimit_ImposedLoad = fDeflectionLimitPermanentLoad; // TODO - dopracovat samostatny limit pre imposed
            MDeflectionLimit_Total = fDeflectionLimitTotal;

            ListOfMembers = new List<CMember>();
        }

        public CMemberGroup(int ID_temp, string sName_temp, EMemberType_FS memberTypeFS_temp, EMemberType_FS_Position memberTypeFS_position, CCrSc crossSection_temp, List<CMember> memberList, float fTime_temp)
        {
            ID = ID_temp;
            Name = sName_temp;
            MemberType_FS = memberTypeFS_temp;
            MemberType_FS_Position = memberTypeFS_position;
            CrossSection = crossSection_temp;
            ListOfMembers = memberList;
            FTime = fTime_temp;
        }
    }
}
