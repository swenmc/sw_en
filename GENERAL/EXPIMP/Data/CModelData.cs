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
    public class CModelData
    {
        private bool debugging = true;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private int MKitsetTypeIndex;
        private int MModelIndex;
        private float MWidth;
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
        private int MRoofCladdingCoatingIndex;
        private int MRoofCladdingColorIndex;
        private int MRoofCladdingThicknessIndex;
        private int MWallCladdingIndex;
        private int MWallCladdingCoatingIndex;
        private int MWallCladdingColorIndex;
        private int MWallCladdingThicknessIndex;
        private int MRoofFibreglassThicknessIndex;
        private int MWallFibreglassThicknessIndex;
        private int MSupportTypeIndex;
        private float MFibreglassAreaRoofRatio;
        private float MFibreglassAreaWallRatio;

        private int MLoadCaseIndex;
        private int iFrontColumnNoInOneFrame;

        private ObservableCollection<DoorProperties> MDoorBlocksProperties;
        private ObservableCollection<WindowProperties> MWindowBlocksProperties;
        private ObservableCollection<CComponentInfo> MComponentList;
        private List<CSectionPropertiesText> m_ComponentDetailsList;
        private List<CMaterialPropertiesText> m_MaterialDetailsList;
        private List<CMaterialPropertiesText> m_MaterialDetailsList_RC;

        //Loads
        private string m_Location;
        private string m_DesignLife;
        private string m_ImportanceClass;
        private string m_SnowRegion;
        private string m_ExposureCategory;
        private string m_WindRegion;
        private string m_TerrainCategory;
        private string m_SiteSubSoilClass;

        private float m_InternalPressureCoefficientCpiMaximumSuction;
        private float m_InternalPressureCoefficientCpiMaximumPressure;
        private float m_AnnualProbabilityULS_Snow;
        private float m_AnnualProbabilityULS_Wind;
        private float m_AnnualProbabilityULS_EQ;
        private float m_AnnualProbabilitySLS;
        private float m_SiteElevation;
        private float m_FaultDistanceDmin_km;
        private float m_FaultDistanceDmax_km;
        private float m_ZoneFactorZ;
        //private float m_PeriodAlongXDirectionTx;
        //private float m_PeriodAlongYDirectionTy;
        //private float m_SpectralShapeFactorChTx;
        //private float m_SpectralShapeFactorChTy;
        private float m_AdditionalDeadActionRoof;
        private float m_AdditionalDeadActionWall;
        private float m_ImposedActionRoof;

        private float m_R_ULS_Snow;
        private float m_R_ULS_Wind;
        private float m_R_ULS_EQ;
        private float m_R_SLS;

        private bool MUseCRSCGeometricalAxes;
        private float m_SoilBearingCapacity;

        private CProjectInfo projectInfo;
        //private DisplayOptions sDisplayOptions;
        private Dictionary<int, DisplayOptions> m_displayOptionsDict;

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

        public sDesignResults sDesignResults_ULSandSLS = new sDesignResults();
        public sDesignResults sDesignResults_ULS = new sDesignResults();
        public sDesignResults sDesignResults_SLS = new sDesignResults();
        public Dictionary<EMemberType_FS_Position, CCalculMember> dictULSDesignResults;
        public Dictionary<EMemberType_FS_Position, CCalculMember> dictSLSDesignResults;
        public Dictionary<EMemberType_FS_Position, CCalculJoint> dictStartJointResults;
        public Dictionary<EMemberType_FS_Position, CCalculJoint> dictEndJointResults;

        public List<CFrame> frameModels;
        public List<CBeam_Simple> beamSimpleModels;

        public Dictionary<CConnectionDescription, CConnectionJointTypes> JointsDict;
        public Dictionary<string, Tuple<CFoundation, CConnectionJointTypes>> FootingsDict;

        private bool m_HasCladding;
        private bool m_HasCladdingFront;
        private bool m_HasCladdingBack;
        private bool m_HasCladdingLeft;
        private bool m_HasCladdingRight;
        private bool m_HasCladdingRoof;


        private CModel MModel;

        //-------------------------------------------------------------------------------------------------------------
        public int KitsetTypeIndex
        {
            get
            {
                return MKitsetTypeIndex;
            }

            set
            {
                MKitsetTypeIndex = value;
            }
        }

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
        public float Width
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

        public int RoofCladdingCoatingIndex
        {
            get
            {
                return MRoofCladdingCoatingIndex;
            }

            set
            {
                MRoofCladdingCoatingIndex = value;
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

        public int WallCladdingCoatingIndex
        {
            get
            {
                return MWallCladdingCoatingIndex;
            }

            set
            {
                MWallCladdingCoatingIndex = value;
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

        public int RoofFibreglassThicknessIndex
        {
            get
            {
                return MRoofFibreglassThicknessIndex;
            }

            set
            {
                MRoofFibreglassThicknessIndex = value;
            }
        }

        public int WallFibreglassThicknessIndex
        {
            get
            {
                return MWallFibreglassThicknessIndex;
            }

            set
            {
                MWallFibreglassThicknessIndex = value;
            }
        }

        public int SupportTypeIndex
        {
            get
            {
                return MSupportTypeIndex;
            }

            set
            {
                MSupportTypeIndex = value;
            }
        }

        public float FibreglassAreaRoofRatio
        {
            get
            {
                return MFibreglassAreaRoofRatio;
            }

            set
            {
                MFibreglassAreaRoofRatio = value;
            }
        }

        public float FibreglassAreaWallRatio
        {
            get
            {
                return MFibreglassAreaWallRatio;
            }

            set
            {
                MFibreglassAreaWallRatio = value;
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

        public bool UseCRSCGeometricalAxes
        {
            get
            {
                return MUseCRSCGeometricalAxes;
            }

            set
            {
                MUseCRSCGeometricalAxes = value;
                
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

        public List<CSectionPropertiesText> ComponentDetailsList
        {
            get
            {
                if (m_ComponentDetailsList == null)
                {
                    m_ComponentDetailsList = CSectionManager.LoadSectionPropertiesNamesSymbolsUnits();
                }
                return m_ComponentDetailsList;
            }

            set
            {
                m_ComponentDetailsList = value;
            }
        }

        public List<CMaterialPropertiesText> MaterialDetailsList
        {
            get
            {
                if (m_MaterialDetailsList == null)
                {
                    m_MaterialDetailsList = CMaterialManager.LoadMaterialPropertiesNamesSymbolsUnits("MaterialsSQLiteDB");
                }
                return m_MaterialDetailsList;
            }

            set
            {
                m_MaterialDetailsList = value;
            }
        }

        public List<CMaterialPropertiesText> MaterialDetailsList_RC
        {
            get
            {
                if (m_MaterialDetailsList_RC == null)
                {
                    m_MaterialDetailsList_RC = CMaterialManager.LoadMaterialPropertiesNamesSymbolsUnits("MaterialsRCSQLiteDB");
                }
                return m_MaterialDetailsList_RC;
            }

            set
            {
                m_MaterialDetailsList_RC = value;
            }
        }

        public CModel Model
        {
            get
            {
                return MModel;
            }

            set
            {
                MModel = value;
            }
        }

        public string Location
        {
            get
            {
                return m_Location;
            }

            set
            {
                m_Location = value;
            }
        }

        public string DesignLife
        {
            get
            {
                return m_DesignLife;
            }

            set
            {
                m_DesignLife = value;
            }
        }

        public string ImportanceClass
        {
            get
            {
                return m_ImportanceClass;
            }

            set
            {
                m_ImportanceClass = value;
            }
        }

        public string SnowRegion
        {
            get
            {
                return m_SnowRegion;
            }

            set
            {
                m_SnowRegion = value;
            }
        }

        public string ExposureCategory
        {
            get
            {
                return m_ExposureCategory;
            }

            set
            {
                m_ExposureCategory = value;
            }
        }

        public string WindRegion
        {
            get
            {
                return m_WindRegion;
            }

            set
            {
                m_WindRegion = value;
            }
        }

        public string TerrainCategory
        {
            get
            {
                return m_TerrainCategory;
            }

            set
            {
                m_TerrainCategory = value;
            }
        }

        public string SiteSubSoilClass
        {
            get
            {
                return m_SiteSubSoilClass;
            }

            set
            {
                m_SiteSubSoilClass = value;
            }
        }

        public float InternalPressureCoefficientCpiMaximumSuction
        {
            get
            {
                return m_InternalPressureCoefficientCpiMaximumSuction;
            }

            set
            {
                m_InternalPressureCoefficientCpiMaximumSuction = value;
            }
        }

        public float InternalPressureCoefficientCpiMaximumPressure
        {
            get
            {
                return m_InternalPressureCoefficientCpiMaximumPressure;
            }

            set
            {
                m_InternalPressureCoefficientCpiMaximumPressure = value;
            }
        }

        public float AnnualProbabilityULS_Snow
        {
            get
            {
                return m_AnnualProbabilityULS_Snow;
            }

            set
            {
                m_AnnualProbabilityULS_Snow = value;
            }
        }

        public float AnnualProbabilityULS_Wind
        {
            get
            {
                return m_AnnualProbabilityULS_Wind;
            }

            set
            {
                m_AnnualProbabilityULS_Wind = value;
            }
        }

        public float AnnualProbabilityULS_EQ
        {
            get
            {
                return m_AnnualProbabilityULS_EQ;
            }

            set
            {
                m_AnnualProbabilityULS_EQ = value;
            }
        }

        public float AnnualProbabilitySLS
        {
            get
            {
                return m_AnnualProbabilitySLS;
            }

            set
            {
                m_AnnualProbabilitySLS = value;
            }
        }

        public float SiteElevation
        {
            get
            {
                return m_SiteElevation;
            }

            set
            {
                m_SiteElevation = value;
            }
        }

        public float FaultDistanceDmin_km
        {
            get
            {
                return m_FaultDistanceDmin_km;
            }

            set
            {
                m_FaultDistanceDmin_km = value;
            }
        }

        public float FaultDistanceDmax_km
        {
            get
            {
                return m_FaultDistanceDmax_km;
            }

            set
            {
                m_FaultDistanceDmax_km = value;
            }
        }

        public float ZoneFactorZ
        {
            get
            {
                return m_ZoneFactorZ;
            }

            set
            {
                m_ZoneFactorZ = value;
            }
        }
        /*
        public float PeriodAlongXDirectionTx
        {
            get
            {
                return m_PeriodAlongXDirectionTx;
            }

            set
            {
                m_PeriodAlongXDirectionTx = value;
            }
        }

        public float PeriodAlongYDirectionTy
        {
            get
            {
                return m_PeriodAlongYDirectionTy;
            }

            set
            {
                m_PeriodAlongYDirectionTy = value;
            }
        }

        public float SpectralShapeFactorChTx
        {
            get
            {
                return m_SpectralShapeFactorChTx;
            }

            set
            {
                m_SpectralShapeFactorChTx = value;
            }
        }

        public float SpectralShapeFactorChTy
        {
            get
            {
                return m_SpectralShapeFactorChTy;
            }

            set
            {
                m_SpectralShapeFactorChTy = value;
            }
        }
        */
        public float AdditionalDeadActionRoof
        {
            get
            {
                return m_AdditionalDeadActionRoof;
            }

            set
            {
                m_AdditionalDeadActionRoof = value;
            }
        }

        public float AdditionalDeadActionWall
        {
            get
            {
                return m_AdditionalDeadActionWall;
            }

            set
            {
                m_AdditionalDeadActionWall = value;
            }
        }

        public float ImposedActionRoof
        {
            get
            {
                return m_ImposedActionRoof;
            }

            set
            {
                m_ImposedActionRoof = value;
            }
        }

        public float R_ULS_Snow
        {
            get
            {
                return m_R_ULS_Snow;
            }

            set
            {
                m_R_ULS_Snow = value;
            }
        }

        public float R_ULS_Wind
        {
            get
            {
                return m_R_ULS_Wind;
            }

            set
            {
                m_R_ULS_Wind = value;
            }
        }

        public float R_ULS_EQ
        {
            get
            {
                return m_R_ULS_EQ;
            }

            set
            {
                m_R_ULS_EQ = value;
            }
        }

        public float R_SLS
        {
            get
            {
                return m_R_SLS;
            }

            set
            {
                m_R_SLS = value;
            }
        }

        public float SoilBearingCapacity
        {
            get
            {
                return m_SoilBearingCapacity;
            }

            set
            {
                m_SoilBearingCapacity = value;
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

        //public DisplayOptions DisplayOptions
        //{
        //    get
        //    {
        //        return f;
        //    }

        //    set
        //    {
        //        sDisplayOptions = value;
        //    }
        //}

        public bool HasCladdingFront
        {
            get
            {
                return m_HasCladdingFront;
            }

            set
            {
                m_HasCladdingFront = value;
            }
        }

        public bool HasCladdingBack
        {
            get
            {
                return m_HasCladdingBack;
            }

            set
            {
                m_HasCladdingBack = value;
            }
        }

        public bool HasCladdingLeft
        {
            get
            {
                return m_HasCladdingLeft;
            }

            set
            {
                m_HasCladdingLeft = value;
            }
        }

        public bool HasCladdingRight
        {
            get
            {
                return m_HasCladdingRight;
            }

            set
            {
                m_HasCladdingRight = value;
            }
        }

        public bool HasCladdingRoof
        {
            get
            {
                return m_HasCladdingRoof;
            }

            set
            {
                m_HasCladdingRoof = value;
            }
        }

        public bool HasCladding
        {
            get
            {
                return m_HasCladding;
            }

            set
            {
                m_HasCladding = value;
            }
        }

        public Dictionary<int, DisplayOptions> DisplayOptionsDict
        {
            get
            {
                return m_displayOptionsDict;
            }

            set
            {
                m_displayOptionsDict = value;
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
