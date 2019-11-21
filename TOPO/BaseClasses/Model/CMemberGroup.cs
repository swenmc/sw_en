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

        private float MDeflectionLimitFraction_Denominator_PermanentLoad;

        public float DeflectionLimitFraction_Denominator_PermanentLoad
        {
            get { return MDeflectionLimitFraction_Denominator_PermanentLoad; }
            set { MDeflectionLimitFraction_Denominator_PermanentLoad = value; }
        }

        private float MDeflectionLimitFraction_Denominator_ImposedLoad;

        public float DeflectionLimitFraction_Denominator_ImposedLoad
        {
            get { return MDeflectionLimitFraction_Denominator_ImposedLoad; }
            set { MDeflectionLimitFraction_Denominator_ImposedLoad = value; }
        }

        private float MDeflectionLimitFraction_Denominator_Total;

        public float DeflectionLimitFraction_Denominator_Total
        {
            get { return MDeflectionLimitFraction_Denominator_Total; }
            set { MDeflectionLimitFraction_Denominator_Total = value; }
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

        /*
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
        */

        public CMemberGroup(int ID_temp, string sName_temp, EMemberType_FS memberTypeFS_temp, EMemberType_FS_Position memberTypeFS_position, CCrSc crossSection_temp, float fDeflectionLimitFraction_Denominator_PermanentLoad, float fDeflectionLimitFraction_Denominator_ImposedLoad, float fDeflectionLimitFraction_Denominator_Total, float fTime_temp)
        {
            ID = ID_temp;
            Name = sName_temp;
            MemberType_FS = memberTypeFS_temp;
            MemberType_FS_Position = memberTypeFS_position;
            CrossSection = crossSection_temp;
            FTime = fTime_temp;

            MDeflectionLimitFraction_Denominator_PermanentLoad = fDeflectionLimitFraction_Denominator_PermanentLoad;
            MDeflectionLimitFraction_Denominator_ImposedLoad = fDeflectionLimitFraction_Denominator_ImposedLoad;
            MDeflectionLimitFraction_Denominator_Total = fDeflectionLimitFraction_Denominator_Total;

            MDeflectionLimit_PermanentLoad = 1f / MDeflectionLimitFraction_Denominator_PermanentLoad;
            MDeflectionLimit_ImposedLoad = 1f / MDeflectionLimitFraction_Denominator_ImposedLoad;
            MDeflectionLimit_Total = 1f / MDeflectionLimitFraction_Denominator_Total;

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
