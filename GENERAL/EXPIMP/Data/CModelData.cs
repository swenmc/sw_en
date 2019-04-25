using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using M_EC1.AS_NZS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EXPIMP
{
    public class CModelData
    {
        private bool debugging = true;
       
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MModelIndex;
        private float MGableWidth;
        private float MLength;
        private float MWallHeight;
        private float MRoofPitch_deg;
        private int MFrames;
        private float MGirtDistance;
        private float MPurlinDistance;
        private float MColumnDistance;
        private float MBottomGirtPosition;
        private float MFrontFrameRakeAngle;
        private float MBackFrameRakeAngle;
        private int MRoofCladdingIndex;
        private int MRoofCladdingColorIndex;
        private int MRoofCladdingThicknessIndex;
        private int MWallCladdingIndex;
        private int MWallCladdingColorIndex;
        private int MWallCladdingThicknessIndex;
        private int MLoadCaseIndex;
        private int iFrontColumnNoInOneFrame;
        
        private ObservableCollection<DoorProperties> MDoorBlocksProperties;
        private ObservableCollection<WindowProperties> MWindowBlocksProperties;
        private ObservableCollection<CComponentInfo> MComponentList;
        private List<CMaterialPropertiesText> m_MaterialDetailsList;




        //private CModel_PFD MModel;
        //-------------------------------------------------------------------------------------------------------------
        //tieto treba spracovat nejako
        public float fL1;
        public float fh2;
        public float fRoofPitch_radians;
        public float fMaterial_density = 7850f; // [kg /m^3] (malo by byt zadane v databaze materialov)

        public CCalcul_1170_1 GeneralLoad;
        public CCalcul_1170_2 Wind;
        public CCalcul_1170_3 Snow;
        public CCalcul_1170_5 Eq;
        //public CPFDLoadInput Loadinput;

        public List<CMemberInternalForcesInLoadCases> MemberInternalForcesInLoadCases;
        public List<CMemberDeflectionsInLoadCases> MemberDeflectionsInLoadCases;

        public List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations;
        public List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations;

        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();
        public List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;

        public List<CFrame> frameModels;
        public List<CBeam_Simple> beamSimpleModels;

        //-------------------------------------------------------------------------------------------------------------
        public int ModelIndex
        {
            get
            {
                return MModelIndex;
            }

            set
            {
                MModelIndex = value;
            }
        }

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

        public float FrontFrameRakeAngle
        {
            get
            {
                return MFrontFrameRakeAngle;
            }

            set
            {
                MFrontFrameRakeAngle = value;
            }
        }

        public float BackFrameRakeAngle
        {
            get
            {
                return MBackFrameRakeAngle;
            }

            set
            {
                MBackFrameRakeAngle = value;
            }
        }

        public int RoofCladdingIndex
        {
            get
            {
                return MRoofCladdingIndex;
            }

            set
            {
                MRoofCladdingIndex = value;
            }
        }

        public int RoofCladdingColorIndex
        {
            get
            {
                return MRoofCladdingColorIndex;
            }

            set
            {
                MRoofCladdingColorIndex = value;
            }
        }

        public int RoofCladdingThicknessIndex
        {
            get
            {
                return MRoofCladdingThicknessIndex;
            }

            set
            {
                MRoofCladdingThicknessIndex = value;
            }
        }

        public int WallCladdingIndex
        {
            get
            {
                return MWallCladdingIndex;
            }

            set
            {
                MWallCladdingIndex = value;
            }
        }

        public int WallCladdingColorIndex
        {
            get
            {
                return MWallCladdingColorIndex;
            }

            set
            {
                MWallCladdingColorIndex = value;
            }
        }

        public int WallCladdingThicknessIndex
        {
            get
            {
                return MWallCladdingThicknessIndex;
            }

            set
            {
                MWallCladdingThicknessIndex = value;
            }
        }

        public int LoadCaseIndex
        {
            get
            {
                return MLoadCaseIndex;
            }

            set
            {
                MLoadCaseIndex = value;
            }
        }

        public int IFrontColumnNoInOneFrame
        {
            get
            {
                return iFrontColumnNoInOneFrame;
            }

            set
            {
                iFrontColumnNoInOneFrame = value;
            }
        }

        public ObservableCollection<DoorProperties> DoorBlocksProperties
        {
            get
            {
                return MDoorBlocksProperties;
            }

            set
            {
                MDoorBlocksProperties = value;
            }
        }

        public ObservableCollection<WindowProperties> WindowBlocksProperties
        {
            get
            {
                return MWindowBlocksProperties;
            }

            set
            {
                MWindowBlocksProperties = value;
            }
        }

        public ObservableCollection<CComponentInfo> ComponentList
        {
            get
            {
                return MComponentList;
            }

            set
            {
                MComponentList = value;
            }
        }
        public List<CMaterialPropertiesText> MaterialDetailsList
        {
            get
            {
                if (m_MaterialDetailsList == null)
                {
                    m_MaterialDetailsList = CMaterialManager.LoadMaterialPropertiesNamesSymbolsUnits();
                }
                return m_MaterialDetailsList;
            }

            set
            {
                m_MaterialDetailsList = value;                
            }
        }

        //private CComponentListVM _componentVM;
        //private CPFDLoadInput _loadInput;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CModelData()
        {            
        }
        
    }
}
