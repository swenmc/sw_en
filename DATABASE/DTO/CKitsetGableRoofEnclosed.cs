using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CKitsetGableRoofEnclosed
    {
        int MID;
        string MModelName;
        string MWidth;
        string MLength;
        string MWall_height;
        string MDistance_L1;
        string MIFrames;
        string MC_array_code;
        string MColumn8;
        string MMainColumn;
        string MMainRafter;
        string MEdgePurlin;
        string MGirt;
        string MPurlin;
        string MColumnFrontSide;
        string MColumnBackSide;
        string MGirtFrontSide;
        string MGirtBackSide;
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------

        public int ID
        {
            get
            {
                return MID;
            }

            set
            {
                MID = value;
            }
        }

        public string ModelName
        {
            get
            {
                return MModelName;
            }

            set
            {
                MModelName = value;
            }
        }

        public string Width
        {
            get
            {
                return MWidth;
            }

            set
            {
                MWidth = value;
            }
        }

        public string Length
        {
            get
            {
                return MLength;
            }

            set
            {
                MLength = value;
            }
        }

        public string Wall_height
        {
            get
            {
                return MWall_height;
            }

            set
            {
                MWall_height = value;
            }
        }

        public string Distance_L1
        {
            get
            {
                return MDistance_L1;
            }

            set
            {
                MDistance_L1 = value;
            }
        }

        public string IFrames
        {
            get
            {
                return MIFrames;
            }

            set
            {
                MIFrames = value;
            }
        }

        public string C_array_code
        {
            get
            {
                return MC_array_code;
            }

            set
            {
                MC_array_code = value;
            }
        }

        public string Column8
        {
            get
            {
                return MColumn8;
            }

            set
            {
                MColumn8 = value;
            }
        }

        public string MainColumn
        {
            get
            {
                return MMainColumn;
            }

            set
            {
                MMainColumn = value;
            }
        }

        public string MainRafter
        {
            get
            {
                return MMainRafter;
            }

            set
            {
                MMainRafter = value;
            }
        }

        public string EdgePurlin
        {
            get
            {
                return MEdgePurlin;
            }

            set
            {
                MEdgePurlin = value;
            }
        }

        public string Girt
        {
            get
            {
                return MGirt;
            }

            set
            {
                MGirt = value;
            }
        }

        public string Purlin
        {
            get
            {
                return MPurlin;
            }

            set
            {
                MPurlin = value;
            }
        }

        public string ColumnFrontSide
        {
            get
            {
                return MColumnFrontSide;
            }

            set
            {
                MColumnFrontSide = value;
            }
        }

        public string ColumnBackSide
        {
            get
            {
                return MColumnBackSide;
            }

            set
            {
                MColumnBackSide = value;
            }
        }

        public string GirtFrontSide
        {
            get
            {
                return MGirtFrontSide;
            }

            set
            {
                MGirtFrontSide = value;
            }
        }

        public string GirtBackSide
        {
            get
            {
                return MGirtBackSide;
            }

            set
            {
                MGirtBackSide = value;
            }
        }

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public CKitsetGableRoofEnclosed() { }

    }
}
