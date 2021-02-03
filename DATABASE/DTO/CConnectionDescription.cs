using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    [Serializable]
    public class CConnectionDescription
    {
        private int m_ID;
        private string m_Name;
        private string m_JoinType;

        private string m_MainMemberPrefix_FS;
        private string m_SecondaryMemberPrefix_FS;
        private string m_SecondaryMember2Prefix_FS;

        private string m_MainMemberPrefix_FS_position;
        private string m_SecondaryMemberPrefix_FS_position;
        private string m_SecondaryMember2Prefix_FS_position;

        private int m_MainMemberPrefix_FS_ID;
        private int m_SecondaryMemberPrefix_FS_ID;
        private int m_SecondaryMember2Prefix_FS_ID;

        private int m_MainMemberPrefix_FS_position_ID;
        private int m_SecondaryMemberPrefix_FS_position_ID;
        private int m_SecondaryMember2Prefix_FS_position_ID;

        private string m_Note;

        public int ID
        {
            get
            {
                return m_ID;
            }

            set
            {
                m_ID = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                m_Name = value;
            }
        }

        public string JoinType
        {
            get
            {
                return m_JoinType;
            }

            set
            {
                m_JoinType = value;
            }
        }

        public string MainMemberPrefix_FS
        {
            get
            {
                return m_MainMemberPrefix_FS;
            }

            set
            {
                m_MainMemberPrefix_FS = value;
            }
        }

        public string SecondaryMemberPrefix_FS
        {
            get
            {
                return m_SecondaryMemberPrefix_FS;
            }

            set
            {
                m_SecondaryMemberPrefix_FS = value;
            }
        }

        public string SecondaryMember2Prefix_FS
        {
            get
            {
                return m_SecondaryMember2Prefix_FS;
            }

            set
            {
                m_SecondaryMember2Prefix_FS = value;
            }
        }

        public string MainMemberPrefix_FS_position
        {
            get
            {
                return m_MainMemberPrefix_FS_position;
            }

            set
            {
                m_MainMemberPrefix_FS_position = value;
            }
        }

        public string SecondaryMemberPrefix_FS_position
        {
            get
            {
                return m_SecondaryMemberPrefix_FS_position;
            }

            set
            {
                m_SecondaryMemberPrefix_FS_position = value;
            }
        }

        public string SecondaryMember2Prefix_FS_position
        {
            get
            {
                return m_SecondaryMember2Prefix_FS_position;
            }

            set
            {
                m_SecondaryMember2Prefix_FS_position = value;
            }
        }

        public int MainMemberPrefix_FS_ID
        {
            get
            {
                return m_MainMemberPrefix_FS_ID;
            }

            set
            {
                m_MainMemberPrefix_FS_ID = value;
            }
        }

        public int SecondaryMemberPrefix_FS_ID
        {
            get
            {
                return m_SecondaryMemberPrefix_FS_ID;
            }

            set
            {
                m_SecondaryMemberPrefix_FS_ID = value;
            }
        }

        public int SecondaryMember2Prefix_FS_ID
        {
            get
            {
                return m_SecondaryMember2Prefix_FS_ID;
            }

            set
            {
                m_SecondaryMember2Prefix_FS_ID = value;
            }
        }

        public int MainMemberPrefix_FS_position_ID
        {
            get
            {
                return m_MainMemberPrefix_FS_position_ID;
            }

            set
            {
                m_MainMemberPrefix_FS_position_ID = value;
            }
        }

        public int SecondaryMemberPrefix_FS_position_ID
        {
            get
            {
                return m_SecondaryMemberPrefix_FS_position_ID;
            }

            set
            {
                m_SecondaryMemberPrefix_FS_position_ID = value;
            }
        }

        public int SecondaryMember2Prefix_FS_position_ID
        {
            get
            {
                return m_SecondaryMember2Prefix_FS_position_ID;
            }

            set
            {
                m_SecondaryMember2Prefix_FS_position_ID = value;
            }
        }

        public string Note
        {
            get
            {
                return m_Note;
            }

            set
            {
                m_Note = value;
            }
        }

        public CConnectionDescription() { }
    }
}
