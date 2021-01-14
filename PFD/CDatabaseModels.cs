﻿using BaseClasses;
using BaseClasses.Helpers;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CDatabaseModels
    {
        //mena premmennych treba zvolit tak aby bolo mozne im rozumiet b,L,L1,h,FrNo  to nema sancu nikto rozkodovat co znamena
        private float m_fb; // Building Width (X-axis)
        private float m_fL; // Building Length (Y-axis)
        private float m_fL1; // Bay Length / Distance between frames (Y-axis)
        private float m_fh; // Building Wall Height (Z-axis)
        private int m_iFrNo; // Number of Frames
        private float m_fRoof_Pitch_deg; // Building roof pitch
        private float m_fdist_girt; // Distance between girts (Z-axis)
        private float m_fdist_purlin; // Distance between purlins (X-axis in roof pitch slope)
        private float m_fdist_frontcolumn; // Distance between Front Wind Posts (X-axis)
        private float m_fdist_girt_bottom; // Distance between bottom girt and column slab (Z-axis)
        private float m_fRakeAngleFrontFrame_deg; // Angle between X-axis and first frame
        private float m_fRakeAngleBackFrame_deg; // Angle between X-axis and last frame

        private int m_iMainColumnFlyBracingEveryXXGirt;
        private int m_iRafterFlyBracingEveryXXPurlin;
        private int m_iEdgePurlin_ILS_Number;
        private int m_iGirt_ILS_Number;
        private int m_iPurlin_ILS_Number;
        private int m_iFrontColumnFlyBracingEveryXXGirt;
        private int m_iBackColumnFlyBracingEveryXXGirt;
        private int m_iGirtFrontSide_ILS_Number;
        private int m_iGirtBackSide_ILS_Number;

        private Dictionary<EMemberType_FS_Position, string> m_membersSectionsDict;

        #region properties
        
        public float fb
        {
            get
            {
                return m_fb;
            }

            set
            {
                m_fb = value;
            }
        }

        public float fL
        {
            get
            {
                return m_fL;
            }

            set
            {
                m_fL = value;
            }
        }

        public float fL1
        {
            get
            {
                return m_fL1;
            }

            set
            {
                m_fL1 = value;
            }
        }

        public float fh
        {
            get
            {
                return m_fh;
            }

            set
            {
                m_fh = value;
            }
        }

        public int iFrNo
        {
            get
            {
                return m_iFrNo;
            }

            set
            {
                m_iFrNo = value;
            }
        }

        public float fRoof_Pitch_deg
        {
            get
            {
                return m_fRoof_Pitch_deg;
            }

            set
            {
                m_fRoof_Pitch_deg = value;
            }
        }

        public float fdist_girt
        {
            get
            {
                return m_fdist_girt;
            }

            set
            {
                m_fdist_girt = value;
            }
        }

        public float fdist_purlin
        {
            get
            {
                return m_fdist_purlin;
            }

            set
            {
                m_fdist_purlin = value;
            }
        }

        public float fdist_frontcolumn
        {
            get
            {
                return m_fdist_frontcolumn;
            }

            set
            {
                m_fdist_frontcolumn = value;
            }
        }

        public float fdist_girt_bottom
        {
            get
            {
                return m_fdist_girt_bottom;
            }

            set
            {
                m_fdist_girt_bottom = value;
            }
        }

        public float fRakeAngleFrontFrame_deg
        {
            get
            {
                return m_fRakeAngleFrontFrame_deg;
            }

            set
            {
                m_fRakeAngleFrontFrame_deg = value;
            }
        }

        public float fRakeAngleBackFrame_deg
        {
            get
            {
                return m_fRakeAngleBackFrame_deg;
            }

            set
            {
                m_fRakeAngleBackFrame_deg = value;
            }
        }

        public int iMainColumnFlyBracingEveryXXGirt
        {
            get
            {
                return m_iMainColumnFlyBracingEveryXXGirt;
            }

            set
            {
                m_iMainColumnFlyBracingEveryXXGirt = value;
            }
        }

        public int iRafterFlyBracingEveryXXPurlin
        {
            get
            {
                return m_iRafterFlyBracingEveryXXPurlin;
            }

            set
            {
                m_iRafterFlyBracingEveryXXPurlin = value;
            }
        }

        public int iEdgePurlin_ILS_Number
        {
            get
            {
                return m_iEdgePurlin_ILS_Number;
            }

            set
            {
                m_iEdgePurlin_ILS_Number = value;
            }
        }

        public int iGirt_ILS_Number
        {
            get
            {
                return m_iGirt_ILS_Number;
            }

            set
            {
                m_iGirt_ILS_Number = value;
            }
        }

        public int iPurlin_ILS_Number
        {
            get
            {
                return m_iPurlin_ILS_Number;
            }

            set
            {
                m_iPurlin_ILS_Number = value;
            }
        }

        public int iFrontColumnFlyBracingEveryXXGirt
        {
            get
            {
                return m_iFrontColumnFlyBracingEveryXXGirt;
            }

            set
            {
                m_iFrontColumnFlyBracingEveryXXGirt = value;
            }
        }

        public int iBackColumnFlyBracingEveryXXGirt
        {
            get
            {
                return m_iBackColumnFlyBracingEveryXXGirt;
            }

            set
            {
                m_iBackColumnFlyBracingEveryXXGirt = value;
            }
        }

        public int iGirtFrontSide_ILS_Number
        {
            get
            {
                return m_iGirtFrontSide_ILS_Number;
            }

            set
            {
                m_iGirtFrontSide_ILS_Number = value;
            }
        }

        public int iGirtBackSide_ILS_Number
        {
            get
            {
                return m_iGirtBackSide_ILS_Number;
            }

            set
            {
                m_iGirtBackSide_ILS_Number = value;
            }
        }

        public Dictionary<EMemberType_FS_Position, string> MembersSectionsDict
        {
            get
            {
                return m_membersSectionsDict;
            }

            set
            {
                m_membersSectionsDict = value;
            }
        }

        #endregion properties

        public CDatabaseModels()
        { }

        public CDatabaseModels(int iSelectedKitsetTypeIndex, int iSelectedModelIndex)
        {
            string sDatabaseTableName;

            /*
            1   KitsetMonoRoofEnclosed
            2   KitsetGableRoofEnclosed
            3   KitsetSheltersSingleSpan
            4   KitsetSheltersDoubleSpan
            */

            if (iSelectedKitsetTypeIndex == 0)
                sDatabaseTableName = "KitsetMonoRoofEnclosed";
            else if (iSelectedKitsetTypeIndex == 1)
                sDatabaseTableName = "KitsetGableRoofEnclosed";
            else
                throw new Exception("Selected kitset type is not implemented.");

            CKitsetMonoOrGableRoofEnclosed model = CModelsManager.LoadModelKitsetMonoOrGableRoofEnclosed(iSelectedModelIndex + 1, sDatabaseTableName);
            if (model == null) throw new Exception("Model is null.");

            fb = ConversionsHelper.ParseFloat(model.Width_overall);
            fL = ConversionsHelper.ParseFloat(model.Length_overall);
            fh = ConversionsHelper.ParseFloat(model.Wall_height_overall);
            iFrNo = int.Parse(model.IFrames);
            fL1 = fL / (iFrNo - 1);
            fRoof_Pitch_deg = 5;
            fdist_girt_bottom = 0.3f; // Distance from concrete foundation to the centerline

            float fDefaultDistanceOfGirts = 1.2f; // 2 meters
            float fDefaultDistanceOfPurlins = 1.2f; // 2 meters
            float fRoofPitch_radians = fRoof_Pitch_deg * MATH.MathF.fPI / 180f;

            float fRafterLength;
            float fdist_purlin_end; // Pre Gable Roof 250 mm od apex, pre Monopitch roof = 0;

            if (iSelectedKitsetTypeIndex == 0)
            {
                fRafterLength = fb / (float)Math.Cos(fRoofPitch_radians);
                fdist_purlin_end = fDefaultDistanceOfPurlins; // Posledna vaznica od konca rafteru
                // Front and back column spacing (default 5 m)
                fdist_frontcolumn = Math.Min(5, fb / 2); // Minimum - one column in the middle of width
            }
            else if (iSelectedKitsetTypeIndex == 1)
            {
                fRafterLength = (0.5f * fb) / (float)Math.Cos(fRoofPitch_radians);
                fdist_purlin_end = 0.25f; // Posledna vaznica od konca rafteru
                // Front and back column spacing (default 5 m)
                fdist_frontcolumn = Math.Min(5, fb / 3); // Minimum - two columns per width
            }
            else
            {
                fRafterLength = 0; // Exception
                fdist_purlin_end = 0;
                fdist_frontcolumn = 0;
            }

            int iDefaultNumberOfGirtsPerColumn = (int)((fh - fdist_girt_bottom) / fDefaultDistanceOfGirts);
            fdist_girt = (fh - fdist_girt_bottom) / Math.Max(iDefaultNumberOfGirtsPerColumn, 1); // Minimalne 1 girt - bottom

            int iDefaultNumberOfPurlinsPerRafter = (int)((fRafterLength - fdist_purlin_end) / fDefaultDistanceOfPurlins);
            fdist_purlin = (fRafterLength - fdist_purlin_end) / Math.Max(iDefaultNumberOfPurlinsPerRafter, 1); // Minimalne 1 purlin

            fRakeAngleFrontFrame_deg = 0.0f; // Angle between first frame and global X-axis
            fRakeAngleBackFrame_deg = 0.0f; // Angle between last frame and global X-axis

            iMainColumnFlyBracingEveryXXGirt = int.Parse(model.ColumnFlyBracingEveryXXGirt); // 0; // Default pre stlpy
            iRafterFlyBracingEveryXXPurlin = int.Parse(model.RafterFlyBracingEveryXXPurlin);
            iEdgePurlin_ILS_Number = int.Parse(model.EdgePurlin_ILS_Number);
            iGirt_ILS_Number = int.Parse(model.Girt_ILS_Number);
            iPurlin_ILS_Number = int.Parse(model.Purlin_ILS_Number);
            iFrontColumnFlyBracingEveryXXGirt = int.Parse(model.ColumnFrontSideFlyBracingEveryXXGirt); // 0; // Default pre stlpy
            iBackColumnFlyBracingEveryXXGirt = int.Parse(model.ColumnBackSideFlyBracingEveryXXGirt); // 0; // Default pre stlpy
            iGirtFrontSide_ILS_Number = int.Parse(model.GirtFrontSide_ILS_Number);
            iGirtBackSide_ILS_Number = int.Parse(model.GirtBackSide_ILS_Number);

            MembersSectionsDict = new Dictionary<EMemberType_FS_Position, string>();
            MembersSectionsDict.Add(EMemberType_FS_Position.MainColumn, model.MainColumn);
            MembersSectionsDict.Add(EMemberType_FS_Position.MainRafter, model.MainRafter);
            MembersSectionsDict.Add(EMemberType_FS_Position.EdgeColumn, model.EdgeColumn);
            MembersSectionsDict.Add(EMemberType_FS_Position.EdgeRafter, model.EdgeRafter);
            MembersSectionsDict.Add(EMemberType_FS_Position.EdgePurlin, model.EdgePurlin);
            MembersSectionsDict.Add(EMemberType_FS_Position.Girt, model.Girt);
            MembersSectionsDict.Add(EMemberType_FS_Position.Purlin, model.Purlin);
            MembersSectionsDict.Add(EMemberType_FS_Position.WindPostFrontSide, model.ColumnFrontSide);
            MembersSectionsDict.Add(EMemberType_FS_Position.WindPostBackSide, model.ColumnBackSide);
            MembersSectionsDict.Add(EMemberType_FS_Position.GirtFrontSide, model.GirtFrontSide);
            MembersSectionsDict.Add(EMemberType_FS_Position.GirtBackSide, model.GirtBackSide);
            MembersSectionsDict.Add(EMemberType_FS_Position.DoorFrame, model.DoorFrame);
            MembersSectionsDict.Add(EMemberType_FS_Position.DoorTrimmer, model.DoorTrimmer);
            MembersSectionsDict.Add(EMemberType_FS_Position.DoorLintel, model.DoorLintel);
            MembersSectionsDict.Add(EMemberType_FS_Position.WindowFrame, model.WindowFrame);

            MembersSectionsDict.Add(EMemberType_FS_Position.BracingBlockGirts, model.BracingBlockGirts);
            MembersSectionsDict.Add(EMemberType_FS_Position.BracingBlockPurlins, model.BracingBlockPurlins);
            MembersSectionsDict.Add(EMemberType_FS_Position.BracingBlockGirtsFrontSide, model.BracingBlocksGirtsFrontSide);
            MembersSectionsDict.Add(EMemberType_FS_Position.BracingBlockGirtsBackSide, model.BracingBlocksGirtsBackSide);

            MembersSectionsDict.Add(EMemberType_FS_Position.CrossBracingWall, model.CrossBracingWalls);
            MembersSectionsDict.Add(EMemberType_FS_Position.CrossBracingRoof, model.CrossBracingRoof);

            MembersSectionsDict.Add(EMemberType_FS_Position.MainRafterCanopy, model.MainRafterCanopy);
            MembersSectionsDict.Add(EMemberType_FS_Position.EdgeRafterCanopy, model.EdgeRafterCanopy);
            MembersSectionsDict.Add(EMemberType_FS_Position.PurlinCanopy, model.PurlinCanopy);
            MembersSectionsDict.Add(EMemberType_FS_Position.BracingBlockPurlinsCanopy, model.BracingBlockPurlinsCanopy);
            MembersSectionsDict.Add(EMemberType_FS_Position.CrossBracingRoofCanopy, model.CrossBracingRoofCanopy);
        }
    }
}























































