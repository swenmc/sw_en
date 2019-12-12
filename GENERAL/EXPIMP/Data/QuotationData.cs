using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using M_AS4600;
using M_EC1.AS_NZS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EXPIMP
{
    public class QuotationData
    {
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private float MGableWidth;
        private float MLength;
        private float MWallHeight;
        private float MRoofPitch_deg;
        private int MFrames;
        private float MGirtDistance;
        private float MPurlinDistance;
        private float MColumnDistance;
        private float MBottomGirtPosition;
        private CProjectInfo projectInfo;
        

        //-------------------------------------------------------------------------------------------------------------
        public float GableWidth
        {
            get
            {
                return MGableWidth;
            }
            set
            {                
                MGableWidth = value;
            }
        }

        public float Length
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

        public float WallHeight
        {
            get
            {
                return MWallHeight;
            }

            set
            {
                MWallHeight = value;
            }
        }

        public float RoofPitch_deg
        {
            get
            {
                return MRoofPitch_deg;
            }

            set
            {
                MRoofPitch_deg = value;
            }
        }

        public int Frames
        {
            get
            {
                return MFrames;
            }

            set
            {
                MFrames = value;
            }
        }

        public float GirtDistance
        {
            get
            {
                return MGirtDistance;
            }

            set
            {
                MGirtDistance = value;
            }
        }

        public float PurlinDistance
        {
            get
            {
                return MPurlinDistance;
            }

            set
            {
                MPurlinDistance = value;
            }
        }

        public float ColumnDistance
        {
            get
            {
                return MColumnDistance;
            }

            set
            {
                MColumnDistance = value;
            }
        }

        public float BottomGirtPosition
        {
            get
            {
                return MBottomGirtPosition;
            }

            set
            {
                MBottomGirtPosition = value;
            }
        }

       
        public CProjectInfo ProjectInfo
        {
            get
            {
                return projectInfo;
            }

            set
            {
                projectInfo = value;
            }
        }
        
        
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public QuotationData()
        {
        }
    }
}
