using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CKitsetMonoOrGableRoofEnclosed
    {
        int MID;
        string MModelName;
        string MWidth_overall;
        string MLength_overall;
        string MWall_height_overall;
        string MLength_centerline;
        string MDistance_L1;
        string MIFrames;
        string MC_array_code;
        string MMainColumn;
        string MMainRafter;
        string MEdgeColumn;
        string MEdgeRafter;
        string MEdgePurlin;
        string MGirt;
        string MPurlin;
        string MColumnFrontSide;
        string MColumnBackSide;
        string MGirtFrontSide;
        string MGirtBackSide;
        string MDoorFrame;
        string MDoorTrimmer;
        string MDoorLintel;
        string MWindowFrame;

        string MBracingBlockGirts;
        string MBracingBlockPurlins;
        string MBracingBlocksGirtsFrontSide;
        string MBracingBlocksGirtsBackSide;
        string MCrossBracingWalls;
        string MCrossBracingRoof;

        string MMainRafterCanopy;
        string MEdgeRafterCanopy;
        string MPurlinCanopy;
        string MEdgePurlinCanopy;
        string MBracingBlockPurlinsCanopy;
        string MCrossBracingRoofCanopy;

        string MColumnFlyBracingEveryXXGirt;
        string MRafterFlyBracingEveryXXPurlin;
        string MColumnFrontSideFlyBracingEveryXXGirt;
        string MColumnBackSideFlyBracingEveryXXGirt;
        string MEdgePurlin_ILS_Number;
        string MGirt_ILS_Number;
        string MPurlin_ILS_Number;
        string MGirtFrontSide_ILS_Number;
        string MGirtBackSide_ILS_Number;
        string MPurlinCanopy_ILS_Number;

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

        public string Width_overall
        {
            get
            {
                return MWidth_overall;
            }

            set
            {
                MWidth_overall = value;
            }
        }

        public string Length_overall
        {
            get
            {
                return MLength_overall;
            }

            set
            {
                MLength_overall = value;
            }
        }

        public string Wall_height_overall
        {
            get
            {
                return MWall_height_overall;
            }

            set
            {
                MWall_height_overall = value;
            }
        }

        public string Length_centerline
        {
            get
            {
                return MLength_centerline;
            }

            set
            {
                MLength_centerline = value;
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

        public string EdgeColumn
        {
            get
            {
                return MEdgeColumn;
            }

            set
            {
                MEdgeColumn = value;
            }
        }

        public string EdgeRafter
        {
            get
            {
                return MEdgeRafter;
            }

            set
            {
                MEdgeRafter = value;
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

        public string DoorFrame
        {
            get
            {
                return MDoorFrame;
            }

            set
            {
                MDoorFrame = value;
            }
        }

        public string DoorTrimmer
        {
            get
            {
                return MDoorTrimmer;
            }

            set
            {
                MDoorTrimmer = value;
            }
        }

        public string DoorLintel
        {
            get
            {
                return MDoorLintel;
            }

            set
            {
                MDoorLintel = value;
            }
        }

        public string WindowFrame
        {
            get
            {
                return MWindowFrame;
            }

            set
            {
                MWindowFrame = value;
            }
        }

        public string MainRafterCanopy
        {
            get
            {
                return MMainRafterCanopy;
            }

            set
            {
                MMainRafterCanopy = value;
            }
        }

        public string EdgeRafterCanopy
        {
            get
            {
                return MEdgeRafterCanopy;
            }

            set
            {
                MEdgeRafterCanopy = value;
            }
        }

        public string PurlinCanopy
        {
            get
            {
                return MPurlinCanopy;
            }

            set
            {
                MPurlinCanopy = value;
            }
        }

        public string EdgePurlinCanopy
        {
            get
            {
                return MEdgePurlinCanopy;
            }

            set
            {
                MEdgePurlinCanopy = value;
            }
        }

        public string BracingBlockPurlinsCanopy
        {
            get
            {
                return MBracingBlockPurlinsCanopy;
            }

            set
            {
                MBracingBlockPurlinsCanopy = value;
            }
        }

        public string CrossBracingRoofCanopy
        {
            get
            {
                return MCrossBracingRoofCanopy;
            }

            set
            {
                MCrossBracingRoofCanopy = value;
            }
        }

        public string ColumnFlyBracingEveryXXGirt
        {
            get
            {
                return MColumnFlyBracingEveryXXGirt;
            }

            set
            {
                MColumnFlyBracingEveryXXGirt = value;
            }
        }

        public string RafterFlyBracingEveryXXPurlin
        {
            get
            {
                return MRafterFlyBracingEveryXXPurlin;
            }

            set
            {
                MRafterFlyBracingEveryXXPurlin = value;
            }
        }

        public string ColumnFrontSideFlyBracingEveryXXGirt
        {
            get
            {
                return MColumnFrontSideFlyBracingEveryXXGirt;
            }

            set
            {
                MColumnFrontSideFlyBracingEveryXXGirt = value;
            }
        }

        public string ColumnBackSideFlyBracingEveryXXGirt
        {
            get
            {
                return MColumnBackSideFlyBracingEveryXXGirt;
            }

            set
            {
                MColumnBackSideFlyBracingEveryXXGirt = value;
            }
        }

        public string EdgePurlin_ILS_Number
        {
            get
            {
                return MEdgePurlin_ILS_Number;
            }

            set
            {
                MEdgePurlin_ILS_Number = value;
            }
        }

        public string Girt_ILS_Number
        {
            get
            {
                return MGirt_ILS_Number;
            }

            set
            {
                MGirt_ILS_Number = value;
            }
        }

        public string Purlin_ILS_Number
        {
            get
            {
                return MPurlin_ILS_Number;
            }

            set
            {
                MPurlin_ILS_Number = value;
            }
        }

        public string GirtFrontSide_ILS_Number
        {
            get
            {
                return MGirtFrontSide_ILS_Number;
            }

            set
            {
                MGirtFrontSide_ILS_Number = value;
            }
        }

        public string GirtBackSide_ILS_Number
        {
            get
            {
                return MGirtBackSide_ILS_Number;
            }

            set
            {
                MGirtBackSide_ILS_Number = value;
            }
        }

        public string BracingBlockGirts
        {
            get
            {
                return MBracingBlockGirts;
            }

            set
            {
                MBracingBlockGirts = value;
            }
        }

        public string BracingBlockPurlins
        {
            get
            {
                return MBracingBlockPurlins;
            }

            set
            {
                MBracingBlockPurlins = value;
            }
        }

        public string BracingBlocksGirtsFrontSide
        {
            get
            {
                return MBracingBlocksGirtsFrontSide;
            }

            set
            {
                MBracingBlocksGirtsFrontSide = value;
            }
        }

        public string BracingBlocksGirtsBackSide
        {
            get
            {
                return MBracingBlocksGirtsBackSide;
            }

            set
            {
                MBracingBlocksGirtsBackSide = value;
            }
        }

        public string CrossBracingWalls
        {
            get
            {
                return MCrossBracingWalls;
            }

            set
            {
                MCrossBracingWalls = value;
            }
        }

        public string CrossBracingRoof
        {
            get
            {
                return MCrossBracingRoof;
            }

            set
            {
                MCrossBracingRoof = value;
            }
        }

        public string PurlinCanopy_ILS_Number
        {
            get
            {
                return MPurlinCanopy_ILS_Number;
            }

            set
            {
                MPurlinCanopy_ILS_Number = value;
            }
        }

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        public CKitsetMonoOrGableRoofEnclosed() { }

    }
}
