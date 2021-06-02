using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using BaseClasses;
using FEM_CALC_BASE;
using System.Data;
using MATH;
using M_BASE;
using M_EC1.AS_NZS;
using BriefFiniteElementNet.CodeProjectExamples;
//using BriefFiniteElementNet;
using System.Windows;
using PFD.Infrastructure;
using System.Collections.ObjectModel;
using DATABASE.DTO;
using EXPIMP;
using M_AS4600;
using BaseClasses.Helpers;
using System.Windows.Media;
using DATABASE;
using System.Globalization;
using CRSC;
using System.Windows.Media.Media3D;

namespace PFD
{
    [Serializable]
    public class CPFDViewModel : INotifyPropertyChanged
    {
        public bool debugging = false;

        [NonSerialized]
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        [NonSerialized]
        public MainWindow PFDMainWindow;
        [NonSerialized]
        public Solver SolverWindow;
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsSetFromCode = false;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private bool MIsRelease;

        private int MKitsetTypeIndex;
        private int MModelIndex;
        private float MWidth;
        private float MLength;
        private float MWallHeight;
        private float MWidthOverall;
        private float MLengthOverall;
        private float MWallHeightOverall;

        private float MMainColumnCrsc_z_plus;
        private float MMainRafterCrsc_z_plus;
        private float MEdgeColumnCrsc_y_minus;
        private float MEdgeColumnCrsc_y_plus;

        private float MRoofPitch_deg;
        private int MFrames;
        private float MGirtDistance;
        private float MPurlinDistance;
        private float MColumnDistance;
        private float MBottomGirtPosition;
        private float MFrontFrameRakeAngle;
        private float MBackFrameRakeAngle;

        private float m_TotalRoofArea;
        private float m_TotalRoofAreaInclCanopies;
        private float m_TotalWallArea;
        private float m_WallAreaLeft;
        private float m_WallAreaRight;
        private float m_WallAreaFront;
        private float m_WallAreaBack;

        private float m_BuildingArea_Gross;
        private float m_BuildingVolume_Gross;
        private float m_RoofSideLength;
        private float m_RoofLength_Y;

        private double m_Height_1_final_edge_LR_Wall;
        private double m_Height_2_final_edge_LR_Wall;
        private double m_Height_1_final_edge_FB_Wall;
        private double m_Height_2_final_edge_FB_Wall;
        private double m_AdditionalOffsetWall;
        private double m_AdditionalOffsetRoof;

        private int MOneRafterPurlinNo;
        private int MSupportTypeIndex;
        private List<string> m_SupportTypes;
        private List<string> m_ModelTypes;
        private List<string> m_KitsetTypes;

        private int MViewIndex;
        private int MViewModelMemberFilterIndex;

        private bool MSynchronizeGUI;
        private bool MRecreateModel;

        private int MLoadCaseIndex;

        private int iFrontColumnNoInOneFrame;
        private bool m_TransformScreenLines3DToCylinders3D;

        private bool m_ModelOptionsChanged;
        private bool m_SolverOptionsChanged;
        private bool m_DesignOptionsChanged;
        private bool m_DisplayOptionsChanged;
        private bool m_CrossBracingOptionsChanged;
        private bool m_BaysWidthOptionsChanged;
        private bool m_CanopiesOptionsChanged;
        private bool m_CladdingOptionsChanged;
        private bool m_DoorsAndWindowsChanged;

        private bool m_OptionsLoaded;

        // Loads - generate options
        private bool MGenerateNodalLoads;
        private bool MGenerateLoadsOnGirts;
        private bool MGenerateLoadsOnPurlins;
        private bool MGenerateLoadsOnColumns;
        private bool MGenerateLoadsOnFrameMembers;
        private bool MGenerateSurfaceLoads;

        //// Displacement / Deflection Limits
        //private float MVerticalDisplacementLimitDenominator_Rafter_PL;
        //private float MVerticalDisplacementLimitDenominator_Rafter_TL;
        //private float MHorizontalDisplacementLimitDenominator_Column_TL;
        //private float MVerticalDisplacementLimitDenominator_Purlin_PL;
        //private float MVerticalDisplacementLimitDenominator_Purlin_TL;
        //private float MHorizontalDisplacementLimitDenominator_Girt_TL;

        // Load Combination - options
        //private bool MDeterminateCombinationResultsByFEMSolver;
        //private bool MUseFEMSolverCalculationForSimpleBeam;
        //private bool MDeterminateMemberLocalDisplacementsForULS;

        // Local member load direction used for load definition, calculation of internal forces and design
        // Use geometrical or principal axes of cross-section to define load direction etc.
        private bool MUseCRSCGeometricalAxes = true;
        //private bool MShearDesignAccording334; // Use shear design according to 3.3.4 or 7


        private List<string> MModelViews;
        private List<string> MViewModelMemberFilters;

        private ObservableCollection<CComponentInfo> MComponentList;
        private bool MModelCalculatedResultsValid;
        private bool MRecreateJoints;
        private bool MRecreateFoundations;
        private bool MRecreateFloorSlab;
        private bool MRecreateSawCuts;
        private bool MRecreateControlJoints;
        private bool MRecreateRCMesh;

        private bool MFootingChanged;
        private bool m_RecreateQuotation = false;
        // Popis pre Ondreja - doors and windows
        // GUI

        // Personnel door su len z jedneho prierezu BOX 10075
        // Roller door mozu mat dva prierezy - door trimmer C270115btb a niekedy aj door lintel z C27095 (podla toho aka je vzdialenost medzi hornou hranou dveri a najblizsim girt nad tym)
        // U blokov sa momentalne moze a nemusi vytvorit crsc pre girt, ten sa potom ale nepridava do celkoveho pola m_arrCrSc pretoze tam uz je

        // Window - podobne ako door, posledna polozka je combobox s 1-20?? - pocet stlpikov okna, ktore rozdelia okno na viacero casti

        // Ked pridam do modelu prve dvere alebo okno mali by sa do Component list pridat prierezy, ktore tam este nie su (trimmer, lintel), tieto prierezy by sa mali vyrobit len raz a ked budem pridavat dalsie dvere alebo okna, tak by sa mali len priradzovat
        // pri pridani dveri sa mozu vygenerovat rozne prierezy podla typu dveri a geometrie (C270115btb-trimmer pre roller door, C27095-lintel pre roller door, BOX 10075 pre personnel door), pri pridani window sa generuje len BOX 10075

        // Prierezy bloku sa momentalne pridavaju do zoznamu vsetkych prierezov pre kazde dvere/okno samostatne
        // vid CBlock_3D_001_DoorInBay, line 41, takze ked pridam 10 dveri s BOX 10075 tak ten BOX 10075 je tam 10 krat samostatny pre kazde dvere co je zbytocne
        // v CModel_PFD_01_GR sa vo funkcii AddDoorOrWindowBlockProperties menia velkosti povodnych poli a pridavaju sa do nich nove objekty z blokov
        // prierez pre GIRT z bloku sa nepridava lebo uz v zozname 3D modelu existuje

        // Je potrebne skontrolovat ci sa prutom z blokov priradia vsetky "bool" parametre a ci sa dane pruty priradia do group of members vid CModel_PFD_01_GR AddMembersToMemberGroupsLists()

        // Potrebujeme opravit 3D grafiku
        // a otocit vsetky lokane osy prutov bloku rovnakym smerom
        // nastavit spravne excentricity a pootocenia
        // "vymysliet" algoritmus ako sa tieto udaje zmenia ak blok otocim a presuniem z prednej steny na lavu, pravu, zadnu (toto je poriadny oriesok)

        // Door trimmers bude potrebne pridat do zoznamov typov prutov ktore su zatazene surface loads, tu je komplikacia ze kazdy prut moze mat inu zatazovaciu sirku
        // vid FreeSurfaceLoadsMemberTypeData

        [NonSerialized]
        private CModel_PFD MModel;

        //-------------------------------------------------------------------------------------------------------------        
        private float m_fBayWidth;
        private float m_fHeight_H2; // Apex height for gable roof or right side wall heigth for monopitch roof
        private float m_fHeight_H2_Overall; // Celkova vyska vratane rozmeru prierezu
        private float m_fRoofPitch_radians;
        //private float fMaterial_density = 7850f; // [kg /m^3] (malo by byt zadane v databaze materialov) //nebolo to pouzite nikde

        [NonSerialized]
        public CCalcul_1170_1 GeneralLoad;
        [NonSerialized]
        public CCalcul_1170_2 Wind;
        [NonSerialized]
        public CCalcul_1170_3 Snow;
        [NonSerialized]
        public CCalcul_1170_5 Eq;
        [NonSerialized]
        public CPFDLoadInput Loadinput;

        [NonSerialized]
        public List<CMemberInternalForcesInLoadCases> MemberInternalForcesInLoadCases;
        [NonSerialized]
        public List<CMemberDeflectionsInLoadCases> MemberDeflectionsInLoadCases;
        [NonSerialized]
        public List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations;
        [NonSerialized]
        public List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations;
        [NonSerialized]
        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
        [NonSerialized]
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();
        [NonSerialized]
        public List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;
        [NonSerialized]
        public List<CFootingLoadCombinationRatio_ULS> FootingDesignResults_ULS;

        [NonSerialized]
        public sDesignResults sDesignResults_ULSandSLS = new sDesignResults();
        [NonSerialized]
        public sDesignResults sDesignResults_ULS = new sDesignResults();
        [NonSerialized]
        public sDesignResults sDesignResults_SLS = new sDesignResults();

        [NonSerialized]
        public List<CFrame> frameModels;
        [NonSerialized]
        public List<CBeam_Simple> beamSimpleModels;

        //To Mato bolo by dobre porozmyslat, co sa ma serializovat a co je zbytocne serializovat
        [NonSerialized]
        public SeisLoadDataInput sSeisInputData;

        //-------------------------------------------------------------------------------------------------------------
        public bool IsRelease
        {
            get
            {
                return MIsRelease;
            }
            set
            {
                MIsRelease = value;
            }
        }

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

                if (MKitsetTypeIndex > 1) // Temporary
                {
                    System.Windows.MessageBox.Show("Selected kitset type is not implemented.");
                    return;
                    //  throw new ArgumentException("Selected kitset type is not implemented.");
                }

                // Nastavit do comboboxu Model type prislusne modely pre dany typ kitsetu
                if (MKitsetTypeIndex == 0) ModelTypes = CDatabaseManager.GetStringList("ModelsSQLiteDB", "KitsetMonoRoofEnclosed", "modelName");
                else if (MKitsetTypeIndex == 1) ModelTypes = CDatabaseManager.GetStringList("ModelsSQLiteDB", "KitsetGableRoofEnclosed", "modelName");
                else if (MKitsetTypeIndex == 2) ModelTypes = CDatabaseManager.GetStringList("ModelsSQLiteDB", "KitsetShelterSingleSpan", "modelName");
                else if (MKitsetTypeIndex == 3) ModelTypes = CDatabaseManager.GetStringList("ModelsSQLiteDB", "KitsetShelterDoubleSpan", "modelName");

                ModelIndex = 1; // Nastavime defaultny model index pre vybrany kitset type (menim property aby som vyvolal aj zmenu modelu)

                UpdateAccesoriesOnModelTypeChange();

                NotifyPropertyChanged("KitsetTypeIndex");
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
                if (MModelIndex == -1) return;

                //dolezite je volat private fields a nie Properties pokial nechceme aby sa volali setter metody
                CDatabaseModels dmodel = null;
                try
                {
                    dmodel = new CDatabaseModels(MKitsetTypeIndex, MModelIndex);
                }
                catch (Exception ex)
                {

                }
                if (dmodel == null) { NotifyPropertyChanged("ModelIndex"); return; }  //Bug 593

                bool isChangedFromCode = IsSetFromCode;

                if (!isChangedFromCode) IsSetFromCode = true;

                //najprv sa nastavi component list view model podla DB
                _componentVM.MembersSectionsDict = dmodel.MembersSectionsDict;
                _componentVM.SetModelComponentListProperties(); //set default components sections
                _componentVM.SetILSProperties(dmodel);
                _componentVM.UpdateBracingBlocks();

                RoofPitch_deg = dmodel.fRoof_Pitch_deg;
                RoofPitch_radians = MRoofPitch_deg * MathF.fPI / 180f;

                WidthOverall = dmodel.fWidth_overall;
                Width = MWidthOverall - 2 * MainColumnCrsc_z_plus;
                LengthOverall = dmodel.fLength_overall;
                Length = MLengthOverall - Math.Abs(EdgeColumnCrsc_y_minus) - EdgeColumnCrsc_y_plus;
                WallHeightOverall = dmodel.fWall_height_overall;
                WallHeight = GetCenterLineHeight_H1();

                GirtDistance = dmodel.fdist_girt;
                PurlinDistance = dmodel.fdist_purlin;
                ColumnDistance = dmodel.fdist_frontcolumn;
                BottomGirtPosition = dmodel.fdist_girt_bottom;
                Frames = dmodel.iFrNo;
                BayWidth = MLength / (MFrames - 1); // je sice v DB, ale istejsie je to tu prepocitavat
                //_baysWidthOptionsVM = new BayWidthOptionsViewModel(Frames - 1, BayWidth);
                _baysWidthOptionsVM.ResetBaysWidths(Frames - 1, BayWidth);

                FrontFrameRakeAngle = dmodel.fRakeAngleFrontFrame_deg;
                BackFrameRakeAngle = dmodel.fRakeAngleBackFrame_deg;

                SetResultsAreNotValid();

                _claddingOptionsVM.SetDefaultValuesOnModelIndexChange(this);
                CalculateCladdingParameters_Mato();

                SupportTypeIndex = 1; // Pinned // Defaultna hodnota indexu v comboboxe

                RecreateJoints = true;
                RecreateFoundations = true;
                RecreateFloorSlab = true;

                RecreateSawCuts = true;
                RecreateControlJoints = true;
                MRecreateRCMesh = true;

                RecreateModel = true;
                if (!isChangedFromCode) IsSetFromCode = false;
                NotifyPropertyChanged("ModelIndex");
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
                if (value < 3 || value > 100)
                    throw new ArgumentException("Width must be between 3 and 100 [m]");
                MWidth = value;

                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode)
                {
                    IsSetFromCode = true;
                    WidthOverall = MWidth + 2 * MainColumnCrsc_z_plus;
                    IsSetFromCode = isChangedFromCode;
                    _claddingOptionsVM.UpdateFibreglassPropertiesMaxX();
                }

                if (MModelIndex != 0)
                {
                    // UHOL ZACHOVAME ROVNAKY - V OPACNOM PRIPADE SA NEUPDATOVALA SPRAVNE VYSKA h2

                    // Recalculate roof pitch
                    //RoofPitch_radians = (float)Math.Atan((Height_H2 - MWallHeight) / (0.5f * MWidth));
                    // Set new value in GUI
                    //MRoofPitch_deg = (RoofPitch_radians * 180f / MathF.fPI);
                    // Recalculate roof height

                    if (MKitsetTypeIndex == 0)
                    {
                        Height_H2 = MWallHeight + MWidth * (float)Math.Tan(RoofPitch_radians);
                        Height_H2_Overall = GetOverallHeight_H2();

                        // Re-calculate value of distance between columns (number of columns per frame is always even
                        int iOneRafterFrontColumnNo = Math.Max(1, (int)((MWidth - 0.95 * MColumnDistance) / MColumnDistance));
                        IFrontColumnNoInOneFrame = 1 * iOneRafterFrontColumnNo;
                    }
                    else if (MKitsetTypeIndex == 1)
                    {
                        Height_H2 = MWallHeight + 0.5f * MWidth * (float)Math.Tan(RoofPitch_radians);
                        Height_H2_Overall = GetOverallHeight_H2();

                        // Re-calculate value of distance between columns (number of columns per frame is always even
                        int iOneRafterFrontColumnNo = Math.Max(1, (int)((0.5f * MWidth - 0.45f * MColumnDistance) / MColumnDistance));
                        IFrontColumnNoInOneFrame = 2 * iOneRafterFrontColumnNo;
                    }
                    else
                    {
                        Height_H2 = 0; // Exception
                        Height_H2_Overall = 0;
                        IFrontColumnNoInOneFrame = 0;
                    }

                    MColumnDistance = MWidth / (IFrontColumnNoInOneFrame + 1); // Update distance between columns
                }
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;
                if (!IsSetFromCode) SetCustomModel();
                if (!IsSetFromCode) CalculateCladdingParameters_Mato();
                NotifyPropertyChanged("Width");
            }
        }

        public void SetCustomModel()
        {
            if (!ModelTypes.Contains("Custom")) ModelTypes.Add("Custom");
            ModelIndex = ModelTypes.Count - 1;
        }

        //L_tot
        //-------------------------------------------------------------------------------------------------------------
        public float Length
        {
            get
            {
                return MLength;
            }

            set
            {
                if (value < 3 || value > 300)
                    throw new ArgumentException("Length must be between 3 and 300 [m]");
                MLength = value;

                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode)
                {
                    IsSetFromCode = true;
                    LengthOverall = MLength + Math.Abs(EdgeColumnCrsc_y_minus) + EdgeColumnCrsc_y_plus;
                    IsSetFromCode = isChangedFromCode;
                    _claddingOptionsVM.UpdateFibreglassPropertiesMaxX();
                }

                if (MModelIndex != 0)
                {
                    // Recalculate BayWidth
                    BayWidth = MLength / (MFrames - 1);
                }
                if (!IsSetFromCode) _baysWidthOptionsVM = new BayWidthOptionsViewModel(Frames - 1, BayWidth);

                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;
                if (!IsSetFromCode) SetCustomModel();
                if (!IsSetFromCode) CalculateCladdingParameters_Mato();
                NotifyPropertyChanged("Length");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float WallHeight
        {
            get
            {
                return MWallHeight;
            }

            set
            {
                if (value < 2 || value > 30)
                    throw new ArgumentException("Wall Height must be between 2 and 30 [m]");
                MWallHeight = value;

                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode)
                {
                    IsSetFromCode = true;
                    WallHeightOverall = GetOverallHeight_H1();
                    IsSetFromCode = isChangedFromCode;
                }

                if (MModelIndex != 0)
                {
                    // Recalculate roof heigth
                    if (MKitsetTypeIndex == 0)
                    {
                        Height_H2 = MWallHeight + MWidth * (float)Math.Tan(RoofPitch_radians);
                        Height_H2_Overall = GetOverallHeight_H2();
                    }
                    else if (MKitsetTypeIndex == 1)
                    {
                        Height_H2 = MWallHeight + 0.5f * MWidth * (float)Math.Tan(RoofPitch_radians);
                        Height_H2_Overall = GetOverallHeight_H2();
                    }
                    else
                    {
                        Height_H2 = 0; // Exception
                        Height_H2_Overall = 0;
                    }
                }
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;
                if (!IsSetFromCode) SetCustomModel();
                if (!IsSetFromCode) CalculateCladdingParameters_Mato();
                NotifyPropertyChanged("WallHeight");
            }
        }

        public float WidthOverall
        {
            get
            {
                return MWidthOverall;
            }

            set
            {
                if (value < 3 || value > 100)
                    throw new ArgumentException("Width must be between 3 and 100 [m]");
                MWidthOverall = value;

                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode)
                {
                    IsSetFromCode = true;
                    Width = MWidthOverall - 2 * MainColumnCrsc_z_plus;
                    IsSetFromCode = isChangedFromCode;
                    _claddingOptionsVM.UpdateFibreglassPropertiesMaxX();
                }

                if (!IsSetFromCode) SetCustomModel();
                if (!IsSetFromCode) CalculateCladdingParameters_Mato();
                NotifyPropertyChanged("WidthOverall");
            }
        }

        public float LengthOverall
        {
            get
            {
                return MLengthOverall;
            }

            set
            {
                if (value < 3 || value > 300)
                    throw new ArgumentException("Length must be between 3 and 300 [m]");
                MLengthOverall = value;

                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode)
                {
                    IsSetFromCode = true;
                    Length = MLengthOverall - Math.Abs(EdgeColumnCrsc_y_minus) - EdgeColumnCrsc_y_plus;
                    IsSetFromCode = isChangedFromCode;
                    _claddingOptionsVM.UpdateFibreglassPropertiesMaxX();
                }

                if (MModelIndex != 0)
                {
                    // Recalculate BayWidth
                    BayWidth = MLength / (MFrames - 1);
                }
                if (!IsSetFromCode) _baysWidthOptionsVM = new BayWidthOptionsViewModel(Frames - 1, BayWidth);
                if (!IsSetFromCode) SetCustomModel();
                if (!IsSetFromCode) CalculateCladdingParameters_Mato();
                NotifyPropertyChanged("LengthOverall");
            }
        }

        public float WallHeightOverall
        {
            get
            {
                return MWallHeightOverall;
            }

            set
            {
                if (value < 2 || value > 30)
                    throw new ArgumentException("Wall Height must be between 2 and 30 [m]");
                MWallHeightOverall = value;

                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode)
                {
                    IsSetFromCode = true;
                    WallHeight = GetCenterLineHeight_H1();
                    IsSetFromCode = isChangedFromCode;
                }
                if (!IsSetFromCode) SetCustomModel();
                if (!IsSetFromCode) CalculateCladdingParameters_Mato();
                NotifyPropertyChanged("WallHeightOverall");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float RoofPitch_deg
        {
            get
            {
                return MRoofPitch_deg;
            }

            set
            {
                // Todo - Ondrej - pada ak zadam napr 60 stupnov, tato kontrola ma prebehnut len ak je hodnota zadana do text boxu uzivatelsky, 
                // ak sa prepocita z inych hodnot (napr. b), tak by sa mala zobrazit
                // aj ked je nevalidna a malo by vypisat nizsie uvedene varovanie, model by sa nemal prekreslit kym nie su vsetky hodnoty validne

                if (MKitsetTypeIndex == 0) // Monopitch roof
                {
                    float fMinimumRoofPitchLimit_radians = (float)Math.Atan((MWallHeight - MBottomGirtPosition) / MWidth); // Kladny sklon v radianoch
                    float fMinimumRoofPitchLimit_deg = fMinimumRoofPitchLimit_radians / (MathF.fPI / 180f); // Kladny sklon v stupnoch
                    fMinimumRoofPitchLimit_deg--; // Od kladneho sklonu opocitame jeden stupen
                    fMinimumRoofPitchLimit_deg = Math.Min(fMinimumRoofPitchLimit_deg, 20); // Mensia z vypocitanej hodnoty a hodnoty 20 stupnov

                    if (value < -fMinimumRoofPitchLimit_deg || value > 20)
                        throw new ArgumentException("Roof Pitch must be between " + Math.Round(-fMinimumRoofPitchLimit_deg, 0) + " and 20 degrees");
                }
                else if (MKitsetTypeIndex == 1) // Gable roof
                {
                    if (value < 3 || value > 20)
                        throw new ArgumentException("Roof Pitch must be between 3 and 20 degrees");
                }
                else
                    throw new ArgumentException("Invalid input. Kitset model is not implemented");

                MRoofPitch_deg = value;

                if (MModelIndex != 0)
                {
                    RoofPitch_radians = MRoofPitch_deg * MathF.fPI / 180f;

                    // Recalculate h2
                    if (MKitsetTypeIndex == 0)
                    {
                        Height_H2 = MWallHeight + MWidth * (float)Math.Tan(RoofPitch_radians);
                        Height_H2_Overall = GetOverallHeight_H2();
                    }
                    else if (MKitsetTypeIndex == 1)
                    {
                        Height_H2 = MWallHeight + 0.5f * MWidth * (float)Math.Tan(RoofPitch_radians);
                        Height_H2_Overall = GetOverallHeight_H2();
                    }
                    else
                    {
                        Height_H2 = 0; // Exception
                        Height_H2_Overall = 0;
                    }
                }
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                if (!IsSetFromCode) SetCustomModel();
                if (!IsSetFromCode) CalculateCladdingParameters_Mato();
                NotifyPropertyChanged("RoofPitch_deg");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int Frames
        {
            get
            {
                return MFrames;
            }

            set
            {
                if (value < 3 || value > 75) // TODO - Je potrebne zapracovat len 2 koncove ramy
                    throw new ArgumentException("Number of frames must be between 3 and 75");
                MFrames = value;

                if (MModelIndex != 0)
                {
                    // Recalculate L1
                    BayWidth = MLength / (MFrames - 1);
                }

                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;

                //To Mato
                //podla mna by sme tu potrebovali vediet tie  RafterFlyBracingPosition_Items
                //tie vypocty co si mi dal, tak tu nevidim tie premenne s kt. pracujes
                //iOneRafterPurlinNo = (int)((fRafterLength - fFirstPurlinPosition) / fDist_Purlin) + 1;
                //float fFirstPurlinPosition = fDist_Purlin;
                //float fRafterLength = MathF.Sqrt(MathF.Pow2(fH2_frame - fH1_frame) + MathF.Pow2(0.5f * fW_frame));

                // To Ondrej - takto spocitas pocet purlins na jednom raftery, pocet je bez edge purlin (to je uplne na kraji)
                // Problem je, ze MPurlinDistance nie je na zaciatku este nastavene, skus sa na to pozriet ci sa to da rozbehat aby sme pocet pozicii vedeli spocitat dynamicky a nepreberali to z UC Members
                float fRafterLength;

                if (MKitsetTypeIndex == 0)
                    fRafterLength = MathF.Sqrt(MathF.Pow2(Height_H2 - MWallHeight) + MathF.Pow2(MWidth));
                else if (MKitsetTypeIndex == 1)
                    fRafterLength = MathF.Sqrt(MathF.Pow2(Height_H2 - MWallHeight) + MathF.Pow2(0.5f * MWidth));
                else
                    fRafterLength = 0; // Exception

                float fFirstPurlinPosition = MPurlinDistance;
                OneRafterPurlinNo = (int)((fRafterLength - fFirstPurlinPosition) / MPurlinDistance) + 1;

                //Update CrossBracings, Canopies and BayWidths view models
                UpdateViewModelsOnFramesChange();

                if (!IsSetFromCode) SetCustomModel();  //TODO Mato - toto si mozes zavesit vsade kde to treba, ku kazdej prperty a zmene na nej
                if (!IsSetFromCode) CalculateCladdingParameters_Mato();
                NotifyPropertyChanged("Frames");
            }
        }


        private void UpdateViewModelsOnFramesChange()
        {
            //709
            //To Mato: pri zmene Frames sa znovu robia cross bracings atd, to znamena,ze tu nastavim nanovo _modelOptions.EnableCrossbracing???
            if (IsSetFromCode) //zmeneny je Model
            {
                //ak je zmeneny model, nesnazime sa o update ale priamo nanovo
                //_crossBracingOptionsVM = new CrossBracingOptionsViewModel(Frames - 1, OneRafterPurlinNo);
                UpdateCrossBracingViewModel();
                _modelOptionsVM.EnableCrossBracing = true;  //default zapnute

                //ak je zmeneny model, nesnazime sa o update ale priamo nanovo
                //_canopiesOptionsVM = new CanopiesOptionsViewModel(Frames - 1, Width);                
                UpdateCanopiesViewModel();
                _modelOptionsVM.EnableCanopies = false; //default vypnute

                _modelOptionsVM.VariousBayWidths = false; //default vypnute
            }
            else
            {
                if (_modelOptionsVM.EnableCrossBracing == true) //iba ak je zapnute
                {
                    UpdateCrossBracingViewModel();
                }
                if (_modelOptionsVM.EnableCanopies == true) //iba ak je zapnute
                {
                    UpdateCanopiesViewModel();
                }
            }

            _baysWidthOptionsVM = new BayWidthOptionsViewModel(Frames - 1, BayWidth);
        }

        public void UpdateCanopiesViewModel()
        {
            if (_canopiesOptionsVM == null) _canopiesOptionsVM = new CanopiesOptionsViewModel(Frames - 1, Width);
            else _canopiesOptionsVM.Update(Frames - 1, Width);
        }
        public void UpdateCrossBracingViewModel()
        {
            if (_crossBracingOptionsVM == null) _crossBracingOptionsVM = new CrossBracingOptionsViewModel(Frames - 1, OneRafterPurlinNo);
            else _crossBracingOptionsVM.Update(Frames - 1, OneRafterPurlinNo);
        }

        //-------------------------------------------------------------------------------------------------------------
        public float GirtDistance
        {
            get
            {
                return MGirtDistance;
            }

            set
            {
                if (value < 0.5 || value > 4)
                    throw new ArgumentException("Girt distance must be between 0.5 and 4.0 [m]");

                //TO Mato - je toto cisto iba na zobrazovanie? pokial to pouzivas aj na vypocty, tak je to zle, nemozes len tak poslat dokelu cislo a zaokruhlit
                MGirtDistance = (float)Math.Round(value, 3); //Display only limited number of decimal places
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;
                NotifyPropertyChanged("GirtDistance");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float PurlinDistance
        {
            get
            {
                return MPurlinDistance;
            }

            set
            {
                if (value < 0.5 || value > 4)
                    throw new ArgumentException("Purlin distance must be between 0.5 and 4.0 [m]");

                //TO Mato - je toto cisto iba na zobrazovanie? pokial to pouzivas aj na vypocty, tak je to zle, nemozes len tak poslat dokelu cislo a zaokruhlit
                MPurlinDistance = (float)Math.Round(value, 3); //Display only limited number of decimal places
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;
                NotifyPropertyChanged("PurlinDistance");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float ColumnDistance
        {
            get
            {
                return MColumnDistance;
            }

            set
            {
                if (value < 1 || value > 0.65 * MWidth)
                    throw new ArgumentException("Column distance must be between 1 and " + Math.Round(0.65 * MWidth, 3) + " [m]");
                MColumnDistance = value;

                if (MModelIndex != 0)
                {
                    // Re-calculate value of distance between columns (number of columns per frame is always even
                    int iOneRafterFrontColumnNo;

                    if (MKitsetTypeIndex == 0)
                    {
                        iOneRafterFrontColumnNo = Math.Max(1, (int)((MWidth - 0.95 * MColumnDistance) / MColumnDistance));
                        IFrontColumnNoInOneFrame = iOneRafterFrontColumnNo;
                    }
                    else if (MKitsetTypeIndex == 1)
                    {
                        iOneRafterFrontColumnNo = Math.Max(1, (int)((0.5f * MWidth - 0.45f * MColumnDistance) / MColumnDistance));
                        IFrontColumnNoInOneFrame = 2 * iOneRafterFrontColumnNo;
                    }
                    else
                    {
                        iOneRafterFrontColumnNo = 0; // Exception
                        IFrontColumnNoInOneFrame = 0;
                    }

                    // Update value of distance between columns

                    // Todo
                    // Nie je to trosku na hlavu? Pouzivatel zada Column distance a ono sa mu to nejako prepocita a zmeni???
                    // Odpoved: Spravnejsie by bolo zadat pocet stlpov, ale je to urobene tak ze to musi byt parne cislo aby boli stlpiky symetricky voci stredu
                    // Pripadalo mi lepsie ak si uzivatel zada nejaku vzdialenost ktoru priblizne vie, pocet stlpikov by si musel spocitat 
                    // alebo tam musi byt dalsi textbox pre column distance kde sa ta dopocitana vzdialenost bude zobrazovat, ale je to dalsi readonly riadok na vstupe navyse
                    // chcel som tam mat toho co najmenej a len najnutnejsie hodnoty
                    // Mozes to tak upravit ak je to logickejsie a spravnejsie

                    MColumnDistance = MWidth / (IFrontColumnNoInOneFrame + 1);
                }
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;
                NotifyPropertyChanged("ColumnDistance");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float BottomGirtPosition
        {
            get
            {
                return MBottomGirtPosition;
            }

            set
            {
                if (value < 0.2 || value > 0.6 * MWallHeight) // Limit is 60% of main column height, could be more but it is
                    throw new ArgumentException("Bottom Girt Position must be between 0.2 and " + Math.Round(0.6 * MWallHeight, 3) + " [m]");
                MBottomGirtPosition = value;
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                NotifyPropertyChanged("BottomGirtPosition");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FrontFrameRakeAngle
        {
            get
            {
                return MFrontFrameRakeAngle;
            }

            set
            {
                float frontFrameRakeAngle_limit_rad = (float)(Math.Atan(BayWidth / MWidth) - (Math.PI / 180)); // minus 1 radian
                float frontFrameRakeAngle_limit_deg = frontFrameRakeAngle_limit_rad * 180f / MathF.fPI;

                if (value < -frontFrameRakeAngle_limit_deg || value > frontFrameRakeAngle_limit_deg)
                {
                    string s = "Front frame angle must be between " + (-Math.Round(frontFrameRakeAngle_limit_deg, 1)) + " and " + Math.Round(frontFrameRakeAngle_limit_deg, 1) + " degrees";
                    throw new ArgumentException(s);
                }
                MFrontFrameRakeAngle = value;
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                NotifyPropertyChanged("FrontFrameRakeAngle");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float BackFrameRakeAngle
        {
            get
            {
                return MBackFrameRakeAngle;
            }

            set
            {
                float backFrameRakeAngle_limit_rad = (float)(Math.Atan(BayWidth / MWidth) - (Math.PI / 180)); // minus 1 radian
                float backFrameRakeAngle_limit_deg = backFrameRakeAngle_limit_rad * 180f / MathF.fPI;

                if (value < -backFrameRakeAngle_limit_deg || value > backFrameRakeAngle_limit_deg)
                {
                    string s = "Back frame angle must be between " + (-Math.Round(backFrameRakeAngle_limit_deg, 1)) + " and " + Math.Round(backFrameRakeAngle_limit_deg, 1) + " degrees";
                    throw new ArgumentException(s);
                }
                MBackFrameRakeAngle = value;
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                NotifyPropertyChanged("BackFrameRakeAngle");
            }
        }
        
        //-------------------------------------------------------------------------------------------------------------
        public int SupportTypeIndex
        {
            get
            {
                return MSupportTypeIndex;
            }

            set
            {
                MSupportTypeIndex = value;
                SetResultsAreNotValid();
                RecreateModel = true;
                NotifyPropertyChanged("SupportTypeIndex");
            }
        }
        
        //-------------------------------------------------------------------------------------------------------------
        public int LoadCaseIndex
        {
            get
            {
                return MLoadCaseIndex;
            }

            set
            {
                MLoadCaseIndex = value;
                RecreateModel = true;
                NotifyPropertyChanged("LoadCaseIndex");
            }
        }

        public CModel_PFD Model
        {
            get
            {
                return MModel;
            }

            set
            {
                MModel = value;
                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode) IsSetFromCode = true;
                _doorsAndWindowsVM.SetModelBays(this);
                if (!isChangedFromCode) IsSetFromCode = false;
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
                RecreateModel = false;
                NotifyPropertyChanged("UseCRSCGeometricalAxes");
            }
        }

        public bool GenerateNodalLoads
        {
            get
            {
                return MGenerateNodalLoads;
            }

            set
            {
                MGenerateNodalLoads = value;
                NotifyPropertyChanged("GenerateNodalLoads");
            }
        }

        public bool GenerateLoadsOnGirts
        {
            get
            {
                return MGenerateLoadsOnGirts;
            }

            set
            {
                MGenerateLoadsOnGirts = value;
                NotifyPropertyChanged("GenerateLoadsOnGirts");
            }
        }

        public bool GenerateLoadsOnPurlins
        {
            get
            {
                return MGenerateLoadsOnPurlins;
            }

            set
            {
                MGenerateLoadsOnPurlins = value;
                NotifyPropertyChanged(" GenerateLoadsOnPurlins");
            }
        }

        public bool GenerateLoadsOnColumns
        {
            get
            {
                return MGenerateLoadsOnColumns;
            }

            set
            {
                MGenerateLoadsOnColumns = value;
                NotifyPropertyChanged(" GenerateLoadsOnColumns");
            }
        }

        public bool GenerateLoadsOnFrameMembers
        {
            get
            {
                return MGenerateLoadsOnFrameMembers;
            }

            set
            {
                MGenerateLoadsOnFrameMembers = value;
                NotifyPropertyChanged("ShowLoadsOnFrameMembers");
            }
        }

        public bool GenerateSurfaceLoads
        {
            get
            {
                return MGenerateSurfaceLoads;
            }

            set
            {
                MGenerateSurfaceLoads = value;
                NotifyPropertyChanged("GenerateSurfaceLoads");
            }
        }



        public List<string> ModelViews
        {
            get
            {
                if (MModelViews == null) MModelViews = new List<string>() { "ISO Front-Right", "ISO Front-Left", "ISO Back-Right", "ISO Back-Left", "Front", "Back", "Left", "Right", "Top" /*"Bottom",*/  };
                return MModelViews;
            }
        }

        public List<string> ViewModelMemberFilters
        {
            get
            {
                if (MViewModelMemberFilters == null) MViewModelMemberFilters = new List<string>() { "All", "Front Side", "Back Side", "Left Side", "Right Side", "Roof", "Middle Frame", "Columns", "Foundations", "Floor" };
                return MViewModelMemberFilters;
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

                if (MComponentList == null) return;

                foreach (CComponentInfo c in MComponentList)
                {
                    c.PropertyChanged -= HandleComponentInfoPropertyChangedEvent;
                    c.PropertyChanged += HandleComponentInfoPropertyChangedEvent;
                }
                NotifyPropertyChanged("ComponentList");
            }
        }

        public bool ModelCalculatedResultsValid
        {
            get
            {
                return MModelCalculatedResultsValid;
            }

            set
            {
                MModelCalculatedResultsValid = value;
                NotifyPropertyChanged("ModelCalculatedResultsValid");
            }
        }

        public CJointsVM JointsVM
        {
            get
            {
                return _jointsVM;
            }

            set
            {
                _jointsVM = value;
                _jointsVM.PropertyChanged += _jointsVM_PropertyChanged;
            }
        }

        public CFootingInputVM FootingVM
        {
            get
            {
                return _footingVM;
            }

            set
            {
                _footingVM = value;
                //_footingVM.PropertyChanged += _footingVM_PropertyChanged;
            }
        }

        private void _footingVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public bool RecreateJoints
        {
            get
            {
                return MRecreateJoints;
            }

            set
            {
                MRecreateJoints = value;
            }
        }

        public bool RecreateFoundations
        {
            get
            {
                return MRecreateFoundations;
            }

            set
            {
                MRecreateFoundations = value;
            }
        }

        public bool RecreateFloorSlab
        {
            get
            {
                return MRecreateFloorSlab;
            }

            set
            {
                MRecreateFloorSlab = value;
            }
        }

        public bool RecreateSawCuts
        {
            get
            {
                return MRecreateSawCuts;
            }

            set
            {
                MRecreateSawCuts = value;
            }
        }

        public bool RecreateControlJoints
        {
            get
            {
                return MRecreateControlJoints;
            }

            set
            {
                MRecreateControlJoints = value;
            }
        }

        public bool RecreateRCMesh
        {
            get
            {
                return MRecreateRCMesh;
            }

            set
            {
                MRecreateRCMesh = value;
            }
        }

        public bool SynchronizeGUI
        {
            get
            {
                return MSynchronizeGUI;
            }

            set
            {
                MSynchronizeGUI = value;
                if (MSynchronizeGUI) NotifyPropertyChanged("SynchronizeGUI");
            }
        }

        public bool RecreateModel
        {
            get
            {
                return MRecreateModel;
            }

            set
            {
                MRecreateModel = value;
                if (MRecreateModel == true) RecreateQuotation = true;
            }
        }

        public bool FootingChanged
        {
            get
            {
                return MFootingChanged;
            }

            set
            {
                MFootingChanged = value;
                SetResultsAreNotValid();
                if (MSynchronizeGUI) NotifyPropertyChanged("FootingChanged");
            }
        }

        public int ViewIndex
        {
            get
            {
                return MViewIndex;
            }

            set
            {
                MViewIndex = value;
                if (MSynchronizeGUI) NotifyPropertyChanged("ViewIndex");
            }
        }

        public int ViewModelMemberFilterIndex
        {
            get
            {
                return MViewModelMemberFilterIndex;
            }

            set
            {
                MViewModelMemberFilterIndex = value;
                if (MSynchronizeGUI) NotifyPropertyChanged("ViewModelMemberFilterIndex");
            }
        }

        public bool TransformScreenLines3DToCylinders3D
        {
            get
            {
                return m_TransformScreenLines3DToCylinders3D;
            }

            set
            {
                m_TransformScreenLines3DToCylinders3D = value;
            }
        }
        
        public bool RecreateQuotation
        {
            get
            {
                return m_RecreateQuotation;
            }

            set
            {
                bool changed = false;
                if (value == true && m_RecreateQuotation == false) changed = true;
                m_RecreateQuotation = value;
                if (changed) NotifyPropertyChanged("RecreateQuotation");
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
        
        public DoorsAndWindowsViewModel _doorsAndWindowsVM;

        public CComponentListVM _componentVM;
        [NonSerialized]
        public CProjectInfoVM _projectInfoVM;
        [NonSerialized]
        private CJointsVM _jointsVM;
        [NonSerialized]
        private CFootingInputVM _footingVM;

        public DisplayOptionsViewModel _displayOptionsVM;
        public ModelOptionsViewModel _modelOptionsVM;
        public SolverOptionsViewModel _solverOptionsVM;
        public DesignOptionsViewModel _designOptionsVM;
        public CladdingOptionsViewModel _claddingOptionsVM;

        public CrossBracingOptionsViewModel _crossBracingOptionsVM;
        public CanopiesOptionsViewModel _canopiesOptionsVM;
        public BayWidthOptionsViewModel _baysWidthOptionsVM;

        [NonSerialized]
        public QuotationViewModel _quotationViewModel;
        [NonSerialized]
        public QuotationDisplayOptionsViewModel _quotationDisplayOptionsVM;
        [NonSerialized]
        public QuotationDisplayOptionsViewModel _quotationExportOptionsVM;

        [NonSerialized]
        public DocumentationSettingsViewModel _documentationExportOptionsVM;
        [NonSerialized]
        public LayoutsExportOptionsViewModel _layoutsExportOptionsVM;

        [NonSerialized]
        public CPFDLoadInput _loadInput;


        public bool ModelOptionsChanged
        {
            get
            {
                return m_ModelOptionsChanged;
            }

            set
            {
                m_ModelOptionsChanged = value;

                SetResultsAreNotValid();
                RecreateModel = true;

                if (MSynchronizeGUI) NotifyPropertyChanged("ModelOptionsChanged");

            }
        }

        public bool CrossBracingOptionsChanged
        {
            get
            {
                return m_CrossBracingOptionsChanged;
            }

            set
            {
                m_CrossBracingOptionsChanged = value;

                SetResultsAreNotValid();
                RecreateModel = true;
                RecreateJoints = true;
                RecreateQuotation = true;
                _componentVM.UpdateComponentList(_crossBracingOptionsVM.HasWallCrosses(), _crossBracingOptionsVM.HasRoofCrosses());
                ComponentList = _componentVM.ComponentList;

                if (MSynchronizeGUI) NotifyPropertyChanged("CrossBracingOptionsChanged");

            }
        }
        public bool CanopiesOptionsChanged
        {
            get
            {
                return m_CanopiesOptionsChanged;
            }

            set
            {
                m_CanopiesOptionsChanged = value;

                SetResultsAreNotValid();
                RecreateModel = true;
                RecreateJoints = true;
                CalculateCladdingParameters_Mato(); //before recreate quotation
                RecreateQuotation = true;
                SetComponentListAccordingToCanopies();                

                if (MSynchronizeGUI) NotifyPropertyChanged("CanopiesOptionsChanged");

            }
        }

        public bool CladdingOptionsChanged
        {
            get
            {
                return m_CladdingOptionsChanged;
            }

            set
            {
                m_CladdingOptionsChanged = value;

                RecreateModel = true;
                RecreateQuotation = true;
                //To mato - treba spustit prepocet ak sa zmenia Cladding Options?
                CalculateCladdingParameters_Mato();
                //to mato a co vysledky? treba vymazat ak sa zmenil Cladding options?
                //SetResultsAreNotValid();

                if (MSynchronizeGUI) NotifyPropertyChanged("CladdingOptionsChanged");
            }
        }

        public bool BaysWidthOptionsChanged
        {
            get
            {
                return m_BaysWidthOptionsChanged;
            }

            set
            {
                m_BaysWidthOptionsChanged = value;

                if (!IsSetFromCode) IsSetFromCode = true;
                Length = _baysWidthOptionsVM.GetTotalWidth();
                LengthOverall = MLength + Math.Abs(EdgeColumnCrsc_y_minus) + EdgeColumnCrsc_y_plus;
                BayWidth = MLength / (MFrames - 1);
                IsSetFromCode = false;

                SetResultsAreNotValid();
                RecreateModel = true;
                if (!IsSetFromCode) SetCustomModel();
                NotifyPropertyChanged("BaysWidthOptionsChanged");
            }
        }

        public bool SolverOptionsChanged
        {
            get
            {
                return m_SolverOptionsChanged;
            }

            set
            {
                m_SolverOptionsChanged = value;

                SetResultsAreNotValid();
                RecreateModel = false;

                NotifyPropertyChanged("SolverOptionsChanged");
            }
        }

        public bool DesignOptionsChanged
        {
            get
            {
                return m_DesignOptionsChanged;
            }

            set
            {
                m_DesignOptionsChanged = value;

                SetResultsAreNotValid();

                NotifyPropertyChanged("DesignOptionsChanged");
            }
        }

        public bool DisplayOptionsChanged
        {
            get
            {
                return m_DisplayOptionsChanged;
            }

            set
            {
                m_DisplayOptionsChanged = value;

                RecreateModel = false;

                NotifyPropertyChanged("DisplayOptionsChanged");
            }
        }

        public bool DoorsAndWindowsChanged
        {
            get
            {
                return m_DoorsAndWindowsChanged;
            }

            set
            {
                m_DoorsAndWindowsChanged = value;
                RecreateQuotation = true;
                RecreateModel = true;
                RecreateJoints = true;
                RecreateFloorSlab = true;
                SetResultsAreNotValid();
                SetComponentListAccordingToDoors();
                NotifyPropertyChanged("DoorsAndWindowsChanged");
            }
        }

        public bool OptionsLoaded
        {
            get
            {
                return m_OptionsLoaded;
            }

            set
            {
                m_OptionsLoaded = value;

                SetResultsAreNotValid();
                RecreateModel = true;

                NotifyPropertyChanged("OptionsLoaded");
            }
        }

        public List<string> SupportTypes
        {
            get
            {
                if (m_SupportTypes == null) m_SupportTypes = new List<string>() { "Fixed", "Pinned" };
                return m_SupportTypes;
            }

            set
            {
                m_SupportTypes = value;
            }
        }

        public List<string> ModelTypes
        {
            get
            {
                if (m_ModelTypes == null) m_ModelTypes = CDatabaseManager.GetStringList("ModelsSQLiteDB", "KitsetGableRoofEnclosed", "modelName");
                return m_ModelTypes;
            }

            set
            {
                m_ModelTypes = value;
                NotifyPropertyChanged("ModelTypes");
            }
        }

        public List<string> KitsetTypes
        {
            get
            {
                if (m_KitsetTypes == null) m_KitsetTypes = CDatabaseManager.GetStringList("ModelsSQLiteDB", "ModelType", "modelTypeName_short");

                // V databaze su sice 4 typy ale 3 a 4 este nie su implementovane, tak ich zatial zmazeme.
                m_KitsetTypes.RemoveRange(2, 2); // Zmazat polozky s indexom 2 a 3
                return m_KitsetTypes;
            }

            set
            {
                m_KitsetTypes = value;
            }
        }

        public int OneRafterPurlinNo
        {
            get
            {
                return MOneRafterPurlinNo;
            }

            set
            {
                MOneRafterPurlinNo = value;
            }
        }

        //public CTS_CrscProperties RoofCladdingProps
        //{
        //    get
        //    {
        //        if (m_RoofCladdingProps == null)
        //        {
        //            m_RoofCladdingProps = CTrapezoidalSheetingManager.GetSectionProperties($"{RoofCladding}-{RoofCladdingThickness}");
        //        }
        //        else if (m_RoofCladdingProps.name != RoofCladding || m_RoofCladdingProps.thickness_for_name != RoofCladdingThickness)
        //        {
        //            m_RoofCladdingProps = CTrapezoidalSheetingManager.GetSectionProperties($"{RoofCladding}-{RoofCladdingThickness}");
        //        }
        //        return m_RoofCladdingProps;
        //    }

        //    set
        //    {
        //        m_RoofCladdingProps = value;
        //    }
        //}

        //public CTS_CrscProperties WallCladdingProps
        //{
        //    get
        //    {
        //        if (m_WallCladdingProps == null)
        //        {
        //            m_WallCladdingProps = CTrapezoidalSheetingManager.GetSectionProperties($"{WallCladding}-{WallCladdingThickness}");
        //        }
        //        else if (m_WallCladdingProps.name != WallCladding || m_WallCladdingProps.thickness_for_name != WallCladdingThickness)
        //        {
        //            m_WallCladdingProps = CTrapezoidalSheetingManager.GetSectionProperties($"{WallCladding}-{WallCladdingThickness}");
        //        }
        //        return m_WallCladdingProps;
        //    }

        //    set
        //    {
        //        m_WallCladdingProps = value;
        //    }
        //}

        public float MainColumnCrsc_z_plus
        {
            get
            {
                if (ComponentList == null) return 0;
                CComponentInfo ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainColumn);
                if (ci != null)
                {
                    //MMainColumnCrsc_z_plus = (float)CrScFactory.GetCrSc(ComponentList[(int)EMemberGroupNames.eMainColumn].Section).z_max;
                    MMainColumnCrsc_z_plus = (float)CrScFactory.GetCrSc(ci.Section).z_max;
                }

                return MMainColumnCrsc_z_plus;
            }

            set
            {
                MMainColumnCrsc_z_plus = value;
            }
        }

        public float EdgeColumnCrsc_y_minus
        {
            get
            {
                if (ComponentList == null) return 0;
                CComponentInfo ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgeColumn);
                if (ci != null)
                {
                    //MEdgeColumnCrsc_y_minus = (float)CrScFactory.GetCrSc(ComponentList[(int)EMemberGroupNames.eMainColumn_EF].Section).y_min;
                    MEdgeColumnCrsc_y_minus = (float)CrScFactory.GetCrSc(ci.Section).y_min;
                }

                return MEdgeColumnCrsc_y_minus;
            }

            set
            {
                MEdgeColumnCrsc_y_minus = value;
            }
        }

        public float EdgeColumnCrsc_y_plus
        {
            get
            {
                if (ComponentList == null) return 0;
                CComponentInfo ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgeColumn);
                if (ci != null)
                {
                    //MEdgeColumnCrsc_y_plus = (float)CrScFactory.GetCrSc(ComponentList[(int)EMemberGroupNames.eMainColumn_EF].Section).y_max;
                    MEdgeColumnCrsc_y_plus = (float)CrScFactory.GetCrSc(ci.Section).y_max;
                }

                return MEdgeColumnCrsc_y_plus;
            }

            set
            {
                MEdgeColumnCrsc_y_plus = value;
            }
        }

        public float MainRafterCrsc_z_plus
        {
            get
            {
                if (ComponentList == null) return 0;
                CComponentInfo ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainRafter);
                if (ci != null)
                {
                    //MMainRafterCrsc_z_plus = (float)CrScFactory.GetCrSc(ComponentList[(int)EMemberGroupNames.eRafter].Section).z_max;
                    MMainRafterCrsc_z_plus = (float)CrScFactory.GetCrSc(ci.Section).z_max;
                }

                return MMainRafterCrsc_z_plus;
            }

            set
            {
                MMainRafterCrsc_z_plus = value;
            }
        }

        public float BayWidth
        {
            get
            {
                return m_fBayWidth;
            }

            set
            {
                m_fBayWidth = value;
            }
        }

        public float Height_H2
        {
            get
            {
                return m_fHeight_H2;
            }

            set
            {
                m_fHeight_H2 = value;
            }
        }

        public float Height_H2_Overall
        {
            get
            {
                return m_fHeight_H2_Overall;
            }

            set
            {
                m_fHeight_H2_Overall = value;
            }
        }

        public float RoofPitch_radians
        {
            get
            {
                return m_fRoofPitch_radians;
            }

            set
            {
                m_fRoofPitch_radians = value;
            }
        }

        public float TotalRoofArea
        {
            get
            {
                return m_TotalRoofArea;
            }

            set
            {
                m_TotalRoofArea = value;
            }
        }

        public float TotalRoofAreaInclCanopies
        {
            get
            {
                return m_TotalRoofAreaInclCanopies;
            }

            set
            {
                m_TotalRoofAreaInclCanopies = value;
            }
        }

        public float TotalWallArea
        {
            get
            {
                return m_TotalWallArea;
            }

            set
            {
                m_TotalWallArea = value;
            }
        }
        public float WallAreaLeft
        {
            get
            {
                return m_WallAreaLeft;
            }

            set
            {
                m_WallAreaLeft = value;
            }
        }

        public float WallAreaRight
        {
            get
            {
                return m_WallAreaRight;
            }

            set
            {
                m_WallAreaRight = value;
            }
        }

        public float WallAreaFront
        {
            get
            {
                return m_WallAreaFront;
            }

            set
            {
                m_WallAreaFront = value;
            }
        }

        public float WallAreaBack
        {
            get
            {
                return m_WallAreaBack;
            }

            set
            {
                m_WallAreaBack = value;
            }
        }

        public float BuildingArea_Gross
        {
            get
            {
                return m_BuildingArea_Gross;
            }

            set
            {
                m_BuildingArea_Gross = value;
            }
        }

        public float BuildingVolume_Gross
        {
            get
            {
                return m_BuildingVolume_Gross;
            }

            set
            {
                m_BuildingVolume_Gross = value;
            }
        }

        public float RoofSideLength
        {
            get
            {
                return m_RoofSideLength;
            }

            set
            {
                m_RoofSideLength = value;
            }
        }

        public float RoofLength_Y
        {
            get
            {
                return m_RoofLength_Y;
            }

            set
            {
                m_RoofLength_Y = value;
            }
        }

        public double Height_1_final_edge_LR_Wall
        {
            get
            {
                return m_Height_1_final_edge_LR_Wall;
            }

            set
            {
                m_Height_1_final_edge_LR_Wall = value;
            }
        }

        public double Height_2_final_edge_LR_Wall
        {
            get
            {
                return m_Height_2_final_edge_LR_Wall;
            }

            set
            {
                m_Height_2_final_edge_LR_Wall = value;
            }
        }

        public double Height_1_final_edge_FB_Wall
        {
            get
            {
                return m_Height_1_final_edge_FB_Wall;
            }

            set
            {
                m_Height_1_final_edge_FB_Wall = value;
            }
        }

        public double Height_2_final_edge_FB_Wall
        {
            get
            {
                return m_Height_2_final_edge_FB_Wall;
            }

            set
            {
                m_Height_2_final_edge_FB_Wall = value;
            }
        }

        public double AdditionalOffsetWall
        {
            get
            {
                return m_AdditionalOffsetWall;
            }

            set
            {
                m_AdditionalOffsetWall = value;
            }
        }

        public double AdditionalOffsetRoof
        {
            get
            {
                return m_AdditionalOffsetRoof;
            }

            set
            {
                m_AdditionalOffsetRoof = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDViewModel(int kitsetTypeIndex, int modelIndex, bool bRelease, ObservableCollection<DoorProperties> doorBlocksProperties, ObservableCollection<WindowProperties> windowBlocksProperties,
            CComponentListVM componentVM, CPFDLoadInput loadInput, CProjectInfoVM projectInfoVM)
        {
            MIsRelease = bRelease;

            IsSetFromCode = true;

            _doorsAndWindowsVM = new DoorsAndWindowsViewModel(doorBlocksProperties, windowBlocksProperties);

            _componentVM = componentVM;
            SetComponentListAccordingToDoorsAndWindows();
            _componentVM.PropertyChanged += ComponentVM_PropertyChanged;
            ComponentList = _componentVM.ComponentList;

            _loadInput = loadInput;
            _loadInput.PropertyChanged += _loadInput_PropertyChanged;

            _projectInfoVM = projectInfoVM;

            _displayOptionsVM = new DisplayOptionsViewModel(bRelease);
            _modelOptionsVM = new ModelOptionsViewModel();
            _solverOptionsVM = new SolverOptionsViewModel();
            _designOptionsVM = new DesignOptionsViewModel();
            _claddingOptionsVM = new CladdingOptionsViewModel();

            RecreateModel = true;
            ViewIndex = (int)EModelViews.ISO_FRONT_RIGHT;
            ViewModelMemberFilterIndex = (int)EViewModelMemberFilters.All;
            TransformScreenLines3DToCylinders3D = false;

            //BracingEverySecondRowOfGirts = true;
            //BracingEverySecondRowOfPurlins = true;

            GenerateNodalLoads = true;
            GenerateLoadsOnGirts = true;
            GenerateLoadsOnPurlins = true;
            GenerateLoadsOnColumns = true;
            GenerateLoadsOnFrameMembers = true;
            GenerateSurfaceLoads = true;

            //DeterminateCombinationResultsByFEMSolver = false;
            //UseFEMSolverCalculationForSimpleBeam = false;
            //DeterminateMemberLocalDisplacementsForULS = false;
            //ShearDesignAccording334 = false;

            // Set default kitset model type
            KitsetTypeIndex = kitsetTypeIndex;


            //_canopiesOptionsVM = new CanopiesOptionsViewModel(Frames - 1, Width);  //nastavujeme az po tom co sa nastavi ModelIndex
            //SetComponentListAccordingToCanopies();

            //temp defaults
            //Width = 30;
            //Length = 100;
            //Frames = 40;

            //_crossBracingOptionsVM = new CrossBracingOptionsViewModel(Frames - 1, ComponentList[(int)EMemberType_FS_Position.MainRafter - 1].ILS_Items);

            //nastavi sa default model type a zaroven sa nastavia vsetky property ViewModelu (samozrejme sa updatuje aj View) 
            //vid setter metoda pre ModelIndex
            //ModelIndex = modelIndex;

            MModelCalculatedResultsValid = false;
            MRecreateJoints = true;
            MSynchronizeGUI = true;

            _quotationDisplayOptionsVM = new QuotationDisplayOptionsViewModel(true);

            IsSetFromCode = false;

            _worker.DoWork += CalculateInternalForces;
            _worker.WorkerSupportsCancellation = true;
        }

        private void _jointsVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CJointsVM vm = sender as CJointsVM;
            if (vm.IsSetFromCode) return;
            if (e.PropertyName == "TabItems") return;
            if (e.PropertyName == "JointTypeIndex") return;
            if (e.PropertyName == "JointTypes") return;
            if (e.PropertyName == "SelectedTabIndex") return;

            if (e.PropertyName == "ChangedScrewArrangementParameter") return;
            if (e.PropertyName == "ChangedAnchorArrangementParameter") return;
            if (e.PropertyName == "ChangedGeometryParameter") return;

            if (PropertyChanged != null) PropertyChanged(sender, e);
        }

        private void _loadInput_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CPFDLoadInput vm = sender as CPFDLoadInput;
            if (vm.IsSetFromCode) return;

            SetResultsAreNotValid();

            if (PropertyChanged != null) PropertyChanged(sender, e);
        }

        public void SetDefaultFlashings()
        {
            _doorsAndWindowsVM.Flashings = GetDefaultFlashings();
            SetFlashingsNames();
        }

        public ObservableCollection<CAccessories_LengthItemProperties> GetDefaultFlashings()
        {
            float fRoofSideLength = 0;

            if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                fRoofSideLength = MathF.Sqrt(MathF.Pow2(Height_H2_Overall - WallHeightOverall) + MathF.Pow2(WidthOverall)); // Dlzka hrany strechy
            }
            else if (KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                fRoofSideLength = MathF.Sqrt(MathF.Pow2(Height_H2_Overall - WallHeightOverall) + MathF.Pow2(0.5f * WidthOverall)); // Dlzka hrany strechy
            }
            else
            {
                // Exception - not implemented
                fRoofSideLength = 0;
            }

            float fRoofRidgeFlashing_TotalLength = 0;
            float fWallCornerFlashing_TotalLength = 0;
            float fBargeFlashing_TotalLength = 0;
            float fEdgePurlinBirdproofStrip_TotalLength = 0;

            float fFibreglassRoofRidgeCap_TotalLength = 0;

            if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 2 * WallHeightOverall + 2 * Height_H2_Overall;
                fBargeFlashing_TotalLength = 2 * fRoofSideLength;
                fEdgePurlinBirdproofStrip_TotalLength = RoofLength_Y;
            }
            else if (KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                fRoofRidgeFlashing_TotalLength = RoofLength_Y;
                fWallCornerFlashing_TotalLength = 4 * WallHeightOverall;
                fBargeFlashing_TotalLength = 4 * fRoofSideLength;
                fEdgePurlinBirdproofStrip_TotalLength = 2 * RoofLength_Y;

                fFibreglassRoofRidgeCap_TotalLength = 0; // TODO - Ondrej dopracovat zistit podla polohy fibreglass sheets, ktore zacinaju resp koncia v ridge +/- 200 mm
            }
            else
            {
                // Exception - not implemented
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 0;
                fBargeFlashing_TotalLength = 0;
                fEdgePurlinBirdproofStrip_TotalLength = 0;
            }

            float fRollerDoorTrimmerFlashing_TotalLength = 0;
            float fRollerDoorLintelFlashing_TotalLength = 0;
            float fRollerDoorLintelCapFlashing_TotalLength = 0;
            //float fPADoorTrimmerFlashing_TotalLength = 0; // Neexistuje
            float fPADoorLintelFlashing_TotalLength = 0;
            float fWindowFlashing_TotalLength = 0;

            ObservableCollection<CAccessories_LengthItemProperties> flashings = new ObservableCollection<CAccessories_LengthItemProperties>();

            if (KitsetTypeIndex != (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                flashings.Add(new CAccessories_LengthItemProperties("Roof Ridge", "Flashings", fRoofRidgeFlashing_TotalLength, 2));
            }

            flashings.Add(new CAccessories_LengthItemProperties("Wall Corner", "Flashings", fWallCornerFlashing_TotalLength, 2));
            flashings.Add(new CAccessories_LengthItemProperties("Barge", "Flashings", fBargeFlashing_TotalLength, 2));
            flashings.Add(new CAccessories_LengthItemProperties("Barge Birdproof", "Flashings", fBargeFlashing_TotalLength, 2));
            flashings.Add(new CAccessories_LengthItemProperties("Eave Purlin Birdproof Strip", "Flashings", fEdgePurlinBirdproofStrip_TotalLength, 2));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Trimmer", "Flashings", fRollerDoorTrimmerFlashing_TotalLength, 4));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Header", "Flashings", fRollerDoorLintelFlashing_TotalLength, 4));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Header Cap", "Flashings", fRollerDoorLintelCapFlashing_TotalLength, 4));
            //flashings.Add(new CAccessories_LengthItemProperties("PA Door Trimmer", "Flashings", fPADoorTrimmerFlashing_TotalLength, 18)); // Neexistuje v GCD - ram dveri je hlinikovy
            flashings.Add(new CAccessories_LengthItemProperties("PA Door Header Cap", "Flashings", fPADoorLintelFlashing_TotalLength, 18));
            flashings.Add(new CAccessories_LengthItemProperties("Window", "Flashings", fWindowFlashing_TotalLength, 9)); // TODO - doriesit ???? Asi tiez neexistuje, nie je v detailoch, ale len lintel/header cap
            flashings.Add(new CAccessories_LengthItemProperties("Fibreglass Roof Ridge Cap", "Flashings", fFibreglassRoofRidgeCap_TotalLength, 2));

            return flashings;
        }

        public void SetDefaultDownpipes()
        {
            _doorsAndWindowsVM.Downpipes = GetDefaultDownpipes();
        }

        public ObservableCollection<CAccessories_DownpipeProperties> GetDefaultDownpipes()
        {
            // Zatial bude natvrdo jeden riadok s poctom zvodov, prednastavenou dlzkou ako vyskou steny a farbou, rovnaky default ako gutter
            int iCountOfDownpipePoints = 0;
            float fDownpipesTotalLength = 0;

            if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                iCountOfDownpipePoints = 2; // TODO - prevziat z GUI - 2 rohy budovy kde je nizsia vyska steny (H1 alebo H2)
                fDownpipesTotalLength = iCountOfDownpipePoints * Math.Min(WallHeightOverall, Height_H2_Overall); // Pocet zvodov krat vyska steny
            }
            else if (KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                iCountOfDownpipePoints = 4; // TODO - prevziat z GUI - 4 rohy strechy
                fDownpipesTotalLength = iCountOfDownpipePoints * WallHeightOverall; // Pocet zvodov krat vyska steny
            }
            else
            {
                // Exception - not implemented
                iCountOfDownpipePoints = 0;
                fDownpipesTotalLength = 0;
            }

            CAccessories_DownpipeProperties downpipe = new CAccessories_DownpipeProperties("RP80®", iCountOfDownpipePoints, fDownpipesTotalLength, 2);

            return new ObservableCollection<CAccessories_DownpipeProperties>() { downpipe };
        }

        public void SetFlashingsNames()
        {
            if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                _doorsAndWindowsVM.FlashingsNames = new List<string>() { "Wall Corner", "Barge", "Barge Birdproof", "Eave Purlin Birdproof Strip",
                        "Roller Door Trimmer", "Roller Door Header", "Roller Door Header Cap",
                        /*"PA Door Trimmer",*/  "PA Door Header Cap", "Window"};
            }
            else if (KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                _doorsAndWindowsVM.FlashingsNames = new List<string>() { "Roof Ridge", "Roof Ridge (Soft Edge)", "Wall Corner", "Barge", "Barge Birdproof", "Eave Purlin Birdproof Strip",
                        "Roller Door Trimmer", "Roller Door Header", "Roller Door Header Cap",
                        /*"PA Door Trimmer",*/ "PA Door Header Cap", "Window", "Fibreglass Roof Ridge Cap"};
            }
            else
            {
                // Exception - not implemented
                _doorsAndWindowsVM.FlashingsNames = null;
            }
        }

        public void SetDefaultGutters()
        {
            _doorsAndWindowsVM.Gutters = GetDefaultGutters();
        }

        public ObservableCollection<CAccessories_LengthItemProperties> GetDefaultGutters()
        {
            float fGuttersTotalLength = 0; // na dvoch okrajoch strechy

            if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                fGuttersTotalLength = RoofLength_Y; // na jednom okraji strechy
            }
            else if (KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                fGuttersTotalLength = 2 * RoofLength_Y; // na dvoch okrajoch strechy
            }
            else
            {
                // Exception - not implemented
                fGuttersTotalLength = 0;
            }

            CAccessories_LengthItemProperties gutter = new CAccessories_LengthItemProperties("Roof Gutter 430", "Gutters", fGuttersTotalLength, 2);
            return new ObservableCollection<CAccessories_LengthItemProperties> { gutter };
        }

        public void UpdateAccesoriesOnModelTypeChange()
        {
            SetDefaultFlashings();
            SetDefaultGutters();
            SetDefaultDownpipes();
        }

        //toto je cele divne
        //podla mna mame 2 eventy na to iste
        //private void ComponentVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        // a potom tento handler - HandleComponentInfoPropertyChangedEvent(object sender, PropertyChangedEventArgs e) ktory to len vyhadzuje vyssie do CPFDViewModel
        //Tu je potrebny dost brutal refaktoring
        // 1. odstranit ComponentList z CPFDViewModelu
        // pouzivat cisto iba vm._componentVM.ComponentList
        //potom budu eventy ciste a spravovane iba v UpdateAllMetode, obavam sa ale,ze nejaky vyznam to malo pokial nechceme prekreslovat model...

        private void ComponentVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedComponentIndex") return;
            else if (e.PropertyName == "ComponentDetailsList") return;
            else if (e.PropertyName == "MaterialDetailsList") return;

            if (e.PropertyName == "Section" || e.PropertyName == "Material" ||
                e.PropertyName == "Generate" || e.PropertyName == "Calculate" ||
                e.PropertyName == "Design")
            {
                SetResultsAreNotValid();
                return;
            }
            //if (e.PropertyName == "ILS")  //zatial netreba
            //{
            //    if (PropertyChanged != null) PropertyChanged(sender, e);
            //}

            if (e.PropertyName == "AllMaterialListChanged")
            {
                if (PropertyChanged != null) PropertyChanged(sender, e);
            }

            if (sender is FrameMembersInfo)
            {
                if (e.PropertyName == "ColumnSectionColor") return;
                if (e.PropertyName == "RafterSectionColor") return;
                RecreateModel = true;
                if (PropertyChanged != null) PropertyChanged(sender, e);
            }
            else if (sender is BayMembersInfo)
            {
                if (e.PropertyName == "SectionColor_EP") return;
                if (e.PropertyName == "SectionColor_G") return;
                if (e.PropertyName == "SectionColor_P") return;
                if (e.PropertyName == "SectionColor_GB") return;
                if (e.PropertyName == "SectionColor_PB") return;
                if (e.PropertyName == "SectionColor_CBW") return;
                if (e.PropertyName == "SectionColor_CBR") return;

                RecreateModel = true;
                if (PropertyChanged != null) PropertyChanged(sender, e);
            }
            else if (sender is OthersMembersInfo)
            {
                RecreateModel = true;
                if (PropertyChanged != null) PropertyChanged(sender, e);
            }

        }

        public void Run()
        {
            if (!_worker.IsBusy) _worker.RunWorkerAsync();
        }

        private void CalculateInternalForces(object sender, DoWorkEventArgs e)
        {
            Calculate();
        }

        public void GenerateMemberLoadsIfNotGenerated()
        {
            CModel_PFD model;

            if (this.Model is CModel_PFD_01_MR)
                model = (CModel_PFD_01_MR)this.Model;
            else if (this.Model is CModel_PFD_01_GR)
                model = (CModel_PFD_01_GR)this.Model;
            else
            {
                model = null;
                throw new Exception("Kitset model is not implemented.");
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Generate loads if they are not generated
            DateTime start = DateTime.Now;
            if (debugging) System.Diagnostics.Trace.WriteLine("GenerateMemberLoadsIfNotGenerated: " + (DateTime.Now - start).TotalMilliseconds);
            if (!GenerateLoadsOnFrameMembers ||
                !GenerateLoadsOnGirts ||
                !GenerateLoadsOnPurlins ||
                !GenerateLoadsOnColumns)
            {
                //Toto tu je blbost, pretoze sa to aj tak zrube z nejakeho dovodu
                CMemberLoadGenerator loadGenerator = new CMemberLoadGenerator(model, GeneralLoad, Snow, Wind);
                if (debugging) System.Diagnostics.Trace.WriteLine("After CMemberLoadGenerator: " + (DateTime.Now - start).TotalMilliseconds);

                LoadCasesMemberLoads memberLoadsOnFrames = new LoadCasesMemberLoads();

                if (!GenerateLoadsOnFrameMembers)
                    memberLoadsOnFrames = loadGenerator.GetGenerateMemberLoadsOnFrames();
                if (debugging) System.Diagnostics.Trace.WriteLine("After GetListOfGenerateMemberLoadsOnFrames: " + (DateTime.Now - start).TotalMilliseconds);

                LoadCasesMemberLoads memberLoadsOnGirtsPurlinsColumns = new LoadCasesMemberLoads();

                if (!GenerateLoadsOnGirts || !GenerateLoadsOnPurlins || !GenerateLoadsOnColumns)
                    memberLoadsOnGirtsPurlinsColumns = loadGenerator.GetGeneratedMemberLoads(model.m_arrLoadCases, model.m_arrMembers);
                if (debugging) System.Diagnostics.Trace.WriteLine("After GetListOfGeneratedMemberLoads: " + (DateTime.Now - start).TotalMilliseconds);

                #region Merge Member Load Lists
                if ((GenerateLoadsOnGirts ||
                    GenerateLoadsOnPurlins ||
                    GenerateLoadsOnColumns) && GenerateLoadsOnFrameMembers)
                {
                    if (memberLoadsOnFrames.Count != memberLoadsOnGirtsPurlinsColumns.Count)
                    {
                        throw new Exception("Not all member load list in all load cases were generated for frames and single members.");
                    }

                    // Merge lists
                    memberLoadsOnFrames.Merge(memberLoadsOnGirtsPurlinsColumns); // Merge both to first LoadCasesMemberLoads
                    // Assign merged list of member loads to the load cases
                    loadGenerator.AssignMemberLoadListsToLoadCases(memberLoadsOnFrames);
                }
                if (debugging) System.Diagnostics.Trace.WriteLine("After AssignMemberLoadListsToLoadCases: " + (DateTime.Now - start).TotalMilliseconds);
                #endregion
            }
            if (debugging) System.Diagnostics.Trace.WriteLine("END OF GenerateMemberLoadsIfNotGenerated: " + (DateTime.Now - start).TotalMilliseconds);
        }

        private void Calculate()
        {
            SolverWindow.SetInputData();
            SolverWindow.Progress = 1;
            SolverWindow.UpdateProgress();

            DateTime start = DateTime.Now;
            if (debugging) System.Diagnostics.Trace.WriteLine("STARTING CALCULATE: " + (DateTime.Now - start).TotalMilliseconds);

            CModel_PFD model = Model;

            CModelHelper.SetMembersAccordingTo(model.m_arrMembers, ComponentList);

            //tieto texty budeme zobrazovat vsetky members alebo len tie ktore su urcene pre vypocet?
            int membersCalcCount = CModelHelper.GetMembersSetForCalculationsCount(model.m_arrMembers);

            SolverWindow.SetCountsLabels(model.m_arrNodes.Length, membersCalcCount/*model.m_arrMembers.Length*/, model.m_arrConnectionJoints.Count, model.m_arrLoadCases.Length, model.m_arrLoadCombs.Length);
            SolverWindow.SetMemberDesignLoadCaseProgress(0, membersCalcCount);
            SolverWindow.SetMemberDesignLoadCombinationProgress(0, membersCalcCount);

            // Validate model before calculation (compare IDs)
            CModelHelper.ValidateModel(model);

            SolverWindow.Progress = 2;
            SolverWindow.UpdateProgress();

            if (debugging) System.Diagnostics.Trace.WriteLine("After validation: " + (DateTime.Now - start).TotalMilliseconds);
            // Tu by sa mal napojit 3D FEM vypocet v pripade ze budeme pocitat vsetko v 3D
            //RunFEMSOlver();

            SolverWindow.SetFrames();
            SolverWindow.SetFramesProgress(0, 0);
            if (!_componentVM.NoFrameMembersForCalculate())
            {
                // Calculation of frame model
                frameModels = model.GetFramesFromModel(); // Create models of particular frames
                if (debugging) System.Diagnostics.Trace.WriteLine("After frameModels = model.GetFramesFromModel(); " + (DateTime.Now - start).TotalMilliseconds);
                CFramesCalculations.RunFramesCalculations(frameModels, !_solverOptionsVM.DeterminateCombinationResultsByFEMSolver, SolverWindow);
                if (debugging) System.Diagnostics.Trace.WriteLine("After frameModels: " + (DateTime.Now - start).TotalMilliseconds);
            }

            SolverWindow.SetBeams();

            if (_solverOptionsVM.DeterminateCombinationResultsByFEMSolver || _solverOptionsVM.UseFEMSolverCalculationForSimpleBeam)
            {
                // Calculation of simple beam model
                beamSimpleModels = model.GetMembersFromModel(); // Create models of particular beams
                if (debugging) System.Diagnostics.Trace.WriteLine("After beamSimpleModels = model.GetMembersFromModel(); " + (DateTime.Now - start).TotalMilliseconds);
                CBeamsCalculations.RunBeamsCalculations(beamSimpleModels, !_solverOptionsVM.DeterminateCombinationResultsByFEMSolver, SolverWindow);
                if (debugging) System.Diagnostics.Trace.WriteLine("After beamSimpleModels: " + (DateTime.Now - start).TotalMilliseconds);
            }
            else
            {
                int beamsCount = model.GetSimpleBeamsCount();
                SolverWindow.SetBeamsProgress(beamsCount, beamsCount);
            }

            CMemberDesignCalculations memberDesignCalculations = new CMemberDesignCalculations(SolverWindow, this);
            memberDesignCalculations.CalculateAll(_solverOptionsVM.MultiCoreCalculation);
            SetDesignMembersLists(memberDesignCalculations);

            System.Diagnostics.Trace.WriteLine("end of calculations: " + (DateTime.Now - start).TotalMilliseconds);

            PFDMainWindow.UpdateResults();
        }

        private void SetDesignMembersLists(CMemberDesignCalculations mdc)
        {
            // TODO - toto budeme asi potrebovat vycistit, ukladame a nastavujeme toho zbytocne prilis vela
            // TODO pokial by sme mali private members a public properties, tak by sa dalo hned pozriet,ci sa pouziva property
            MemberInternalForcesInLoadCombinations = mdc.MemberInternalForcesInLoadCombinations;
            MemberDeflectionsInLoadCombinations = mdc.MemberDeflectionsInLoadCombinations;

            MemberInternalForcesInLoadCases = mdc.MemberInternalForcesInLoadCases;
            MemberDeflectionsInLoadCases = mdc.MemberDeflectionsInLoadCases;

            MemberDesignResults_ULS = mdc.MemberDesignResults_ULS;
            MemberDesignResults_SLS = mdc.MemberDesignResults_SLS;
            JointDesignResults_ULS = mdc.JointDesignResults_ULS;
            FootingDesignResults_ULS = mdc.FootingDesignResults_ULS;

            sDesignResults_ULSandSLS = mdc.sDesignResults_ULSandSLS;
            sDesignResults_ULS = mdc.sDesignResults_ULS;
            sDesignResults_SLS = mdc.sDesignResults_SLS;
        }

        public void SetComponentListAccordingToDoorsAndWindows()
        {
            // To Ondrej _doorsAndWindowsVM nemusi byt vzdy inicializovane

            if (_doorsAndWindowsVM != null)
            {
                SetComponentListAccordingToDoors();
                SetComponentListAccordingToWindows();
            }

            ComponentList = _componentVM.ComponentList;
        }

        private void SetComponentListAccordingToDoors()
        {
            if (_doorsAndWindowsVM.ModelHasPersonelDoor()) _componentVM.AddPersonelDoor();
            else _componentVM.RemovePersonelDoor();

            if (_doorsAndWindowsVM.ModelHasRollerDoor()) _componentVM.AddRollerDoor();
            else _componentVM.RemoveRollerDoor();

        }

        private void SetComponentListAccordingToWindows()
        {
            if (_doorsAndWindowsVM.ModelHasWindow()) _componentVM.AddWindow();
            else _componentVM.RemoveWindow();
        }

        public void SetComponentListAccordingToCanopies()
        {
            if (_canopiesOptionsVM.HasCanopies())
            {
                _componentVM.AddCanopy(_canopiesOptionsVM.HasCanopiesMainRafter(), _canopiesOptionsVM.HasCanopiesPurlin(), _canopiesOptionsVM.HasCanopiesCrossBracing());
                _componentVM.SetCanopyRafterFlyBracingPosition_Items(_canopiesOptionsVM.GetMaxPurlinCount());
                ComponentList = _componentVM.ComponentList;
            }
            else _componentVM.RemoveCanopy();
        }

        private void SetResultsAreNotValid()
        {
            ModelCalculatedResultsValid = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }



        private void HandleComponentInfoPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) this.PropertyChanged(sender, e);
        }

        public CModelData GetModelData()
        {
            CModelData data = new CModelData();
            data.ModelIndex = ModelIndex;
            data.Width = MWidth;
            data.Length = MLength;
            data.WallHeight = MWallHeight;
            data.RoofPitch_deg = MRoofPitch_deg;
            data.Frames = MFrames;
            data.GirtDistance = MGirtDistance;
            data.PurlinDistance = MPurlinDistance;
            data.ColumnDistance = MColumnDistance;
            data.BottomGirtPosition = MBottomGirtPosition;
            data.FrontFrameRakeAngle = MFrontFrameRakeAngle;
            data.BackFrameRakeAngle = MBackFrameRakeAngle;
            data.RoofCladdingIndex = _claddingOptionsVM.RoofCladdingIndex;
            data.RoofCladdingCoatingIndex = _claddingOptionsVM.RoofCladdingCoatingIndex;
            data.RoofCladdingColorIndex = _claddingOptionsVM.RoofCladdingColorIndex;
            data.RoofCladdingThicknessIndex = _claddingOptionsVM.RoofCladdingThicknessIndex;
            data.WallCladdingIndex = _claddingOptionsVM.WallCladdingIndex;
            data.WallCladdingCoatingIndex = _claddingOptionsVM.WallCladdingCoatingIndex;
            data.WallCladdingColorIndex = _claddingOptionsVM.WallCladdingColorIndex;
            data.WallCladdingThicknessIndex = _claddingOptionsVM.WallCladdingThicknessIndex;

            data.RoofFibreglassThicknessIndex = _claddingOptionsVM.RoofFibreglassThicknessIndex;
            data.WallFibreglassThicknessIndex = _claddingOptionsVM.WallFibreglassThicknessIndex;

            data.SupportTypeIndex = MSupportTypeIndex;
            data.FibreglassAreaRoofRatio = _claddingOptionsVM.FibreglassAreaRoofRatio;
            data.FibreglassAreaWallRatio = _claddingOptionsVM.FibreglassAreaWallRatio;
            data.LoadCaseIndex = MLoadCaseIndex;
            data.IFrontColumnNoInOneFrame = IFrontColumnNoInOneFrame;
            data.UseCRSCGeometricalAxes = UseCRSCGeometricalAxes;

            data.GeneralLoad = GeneralLoad;
            data.Wind = Wind;
            data.Snow = Snow;
            data.Eq = Eq;

            if (_doorsAndWindowsVM != null)
            {
                data.DoorBlocksProperties = _doorsAndWindowsVM.DoorBlocksProperties;
                data.WindowBlocksProperties = _doorsAndWindowsVM.WindowBlocksProperties;
            }

            data.ComponentList = ComponentList;
            data.Model = Model;

            //Load input
            data.Location = _loadInput.ListLocations[_loadInput.LocationIndex];
            data.DesignLife = _loadInput.ListDesignLife[_loadInput.DesignLifeIndex];
            data.ImportanceClass = _loadInput.ListImportanceClass[_loadInput.ImportanceClassIndex];
            data.SnowRegion = _loadInput.ListSnowRegion[_loadInput.SnowRegionIndex];
            data.ExposureCategory = _loadInput.ListExposureCategory[_loadInput.ExposureCategoryIndex];
            data.WindRegion = _loadInput.ListWindRegion[_loadInput.WindRegionIndex];
            data.TerrainCategory = _loadInput.ListTerrainCategory[_loadInput.TerrainCategoryIndex];
            data.SiteSubSoilClass = _loadInput.ListSiteSubSoilClass[_loadInput.SiteSubSoilClassIndex];
            data.SiteElevation = _loadInput.SiteElevation;
            data.R_SLS = _loadInput.R_SLS;
            data.AnnualProbabilitySLS = _loadInput.AnnualProbabilitySLS;

            data.InternalPressureCoefficientCpiMaximumPressure = _loadInput.InternalPressureCoefficientCpiMaximumPressure;
            data.InternalPressureCoefficientCpiMaximumSuction = _loadInput.InternalPressureCoefficientCpiMaximumSuction;

            // Konverzia na zakladne jednotky SI
            data.AdditionalDeadActionWall = _loadInput.AdditionalDeadActionWall * 1000;
            data.AdditionalDeadActionRoof = _loadInput.AdditionalDeadActionRoof * 1000;
            data.ImposedActionRoof = _loadInput.ImposedActionRoof * 1000;
            data.AnnualProbabilityULS_Snow = _loadInput.AnnualProbabilityULS_Snow;
            data.R_ULS_Snow = _loadInput.R_ULS_Snow;
            data.AnnualProbabilityULS_Wind = _loadInput.AnnualProbabilityULS_Wind;
            data.R_ULS_Wind = _loadInput.R_ULS_Wind;
            data.AnnualProbabilityULS_EQ = _loadInput.AnnualProbabilityULS_EQ;
            data.R_ULS_EQ = _loadInput.R_ULS_EQ;
            data.FaultDistanceDmin_km = _loadInput.FaultDistanceDmin_km;
            data.FaultDistanceDmax_km = _loadInput.FaultDistanceDmax_km;
            data.ZoneFactorZ = _loadInput.ZoneFactorZ;
            //data.PeriodAlongXDirectionTx = _loadInput.PeriodAlongXDirectionTx;
            //data.PeriodAlongYDirectionTy = _loadInput.PeriodAlongYDirectionTy;
            //data.SpectralShapeFactorChTx = _loadInput.SpectralShapeFactorChTx;
            //data.SpectralShapeFactorChTy = _loadInput.SpectralShapeFactorChTy;


            //toto som zakomentoval aby sa nezrubavalo ak nie su validne vysledky
            //if (ModelCalculatedResultsValid) // Skusame nacitat vysledky len ak su spocitane

            data.sDesignResults_ULSandSLS = sDesignResults_ULSandSLS;
            data.sDesignResults_ULS = sDesignResults_ULS;
            data.sDesignResults_SLS = sDesignResults_SLS;

            // BUG 437
            // Bracing Blocks by mali ma bCalculate a bDesign stale na False
            data.dictULSDesignResults = GetDesignResultsULS();
            data.dictSLSDesignResults = GetDesignResultsSLS();
            GetGoverningMemberJointsDesignDetails(out data.dictStartJointResults, out data.dictEndJointResults);

            data.MemberInternalForcesInLoadCombinations = MemberInternalForcesInLoadCombinations;
            data.MemberDeflectionsInLoadCombinations = MemberDeflectionsInLoadCombinations;
            data.frameModels = frameModels;


            data.JointsDict = JointsVM.DictJoints;
            data.FootingsDict = FootingVM.DictFootings;

            //tu treba dat vsetky data ktore si chces hore dole prenasat z PFDViewModelu
            //Task 366
            data.SoilBearingCapacity = FootingVM.SoilBearingCapacity;

            data.ProjectInfo = _projectInfoVM.GetProjectInfo();
            data.DisplayOptions = GetDisplayOptions();

            data.HasCladding = _modelOptionsVM.EnableCladding && ModelHasPurlinsOrGirts();
            data.HasCladdingBack = _modelOptionsVM.EnableCladding && ModelHasBackWall();
            data.HasCladdingFront = _modelOptionsVM.EnableCladding && ModelHasFrontWall();
            data.HasCladdingLeft = _modelOptionsVM.EnableCladding && ModelHasLeftWall();
            data.HasCladdingRight = _modelOptionsVM.EnableCladding && ModelHasRightWall();
            data.HasCladdingRoof = _modelOptionsVM.EnableCladding && ModelHasRoof();
            return data;
        }

        public QuotationData GetQuotationData()
        {
            QuotationData data = new QuotationData();

            if (KitsetTypeIndex == 0)
                data.RoofShape = "Monopitch";
            else if (KitsetTypeIndex == 1)
                data.RoofShape = "Gable";
            else
            {
                data.RoofShape = ""; // Not implemented
                throw new Exception("Model shape is not implemented.");
            }
            data.KitSetTypeIndex = MKitsetTypeIndex;
            data.Width_Overall = MWidthOverall;
            data.Length_Overall = MLengthOverall;
            data.WallHeight_Overall = MWallHeightOverall;
            data.RoofPitch_deg = MRoofPitch_deg;
            data.Frames = MFrames;
            data.GirtDistance = MGirtDistance;
            data.PurlinDistance = MPurlinDistance;
            data.ColumnDistance = MColumnDistance;
            data.BottomGirtPosition = MBottomGirtPosition;

            data.BayWidth = BayWidth;
            data.ApexHeight_H2_Overall = Height_H2_Overall;

            data.RoofCladding = _claddingOptionsVM.RoofCladding;
            data.WallCladding = _claddingOptionsVM.WallCladding;
            data.RoofCladdingThickness_mm = _claddingOptionsVM.RoofCladdingThickness;
            data.WallCladdingThickness_mm = _claddingOptionsVM.WallCladdingThickness;
            data.RoofCladdingCoating = _claddingOptionsVM.RoofCladdingCoating;
            data.WallCladdingCoating = _claddingOptionsVM.WallCladdingCoating;
            data.RoofFibreglassThickness_mm = _claddingOptionsVM.RoofFibreglassThicknessTypes.ElementAtOrDefault(_claddingOptionsVM.RoofFibreglassThicknessIndex);
            data.WallFibreglassThickness_mm = _claddingOptionsVM.WallFibreglassThicknessTypes.ElementAtOrDefault(_claddingOptionsVM.WallFibreglassThicknessIndex);

            data.Location = _loadInput.ListLocations[_loadInput.LocationIndex];
            data.WindRegion = _loadInput.ListWindRegion[_loadInput.WindRegionIndex];

            if (_doorsAndWindowsVM != null)
            {
                data.NumberOfRollerDoors = _doorsAndWindowsVM.DoorBlocksProperties.Where(d => d.sDoorType == "Roller Door").Count();
                data.NumberOfPersonnelDoors = _doorsAndWindowsVM.DoorBlocksProperties.Where(d => d.sDoorType == "Personnel Door").Count();
            }

            data.ProjectInfo = _projectInfoVM.GetProjectInfo();

            data.BuildingArea_Gross = _quotationViewModel.BuildingArea_Gross;
            data.BuildingMass = _quotationViewModel.BuildingMass;
            data.BuildingNetPrice_WithoutMargin_WithoutGST = _quotationViewModel.BuildingNetPrice_WithoutMargin_WithoutGST;
            data.BuildingPrice_PCM = _quotationViewModel.BuildingPrice_PCM;
            data.BuildingPrice_PPKG = _quotationViewModel.BuildingPrice_PPKG;
            data.BuildingPrice_PSM = _quotationViewModel.BuildingPrice_PSM;
            data.BuildingPrice_WithMargin_WithoutGST = _quotationViewModel.BuildingPrice_WithMargin_WithoutGST;
            data.BuildingVolume_Gross = _quotationViewModel.BuildingVolume_Gross;
            data.GST_Absolute = _quotationViewModel.GST_Absolute;
            data.GST_Percentage = _quotationViewModel.GST_Percentage;
            data.Margin_Absolute = _quotationViewModel.Margin_Absolute;
            data.Margin_Percentage = _quotationViewModel.Margin_Percentage;
            data.Markup_Absolute = _quotationViewModel.Markup_Absolute;
            data.Markup_Percentage = _quotationViewModel.Markup_Percentage;
            data.TotalBuildingPrice_IncludingGST = _quotationViewModel.TotalBuildingPrice_IncludingGST;

            data.HasCladding = _modelOptionsVM.EnableCladding && ModelHasPurlinsOrGirts();
            data.HasCladdingWalls = _modelOptionsVM.EnableCladding && ModelHasWalls();
            data.HasCladdingBack = _modelOptionsVM.EnableCladding && ModelHasBackWall();
            data.HasCladdingFront = _modelOptionsVM.EnableCladding && ModelHasFrontWall();
            data.HasCladdingLeft = _modelOptionsVM.EnableCladding && ModelHasLeftWall();
            data.HasCladdingRight = _modelOptionsVM.EnableCladding && ModelHasRightWall();
            data.HasCladdingRoof = _modelOptionsVM.EnableCladding && ModelHasRoof();

            return data;
        }

        private float GetOverallHeight_H1()
        {
            float fz1 = MainColumnCrsc_z_plus * (float)Math.Tan(RoofPitch_radians);
            float fz3 = MainRafterCrsc_z_plus / (float)Math.Cos(RoofPitch_radians);
            float fz2 = fz3 - fz1;

            return MWallHeight + fz2;
        }

        private float GetCenterLineHeight_H1()
        {
            float fz1 = MainColumnCrsc_z_plus * (float)Math.Tan(RoofPitch_radians);
            float fz3 = MainRafterCrsc_z_plus / (float)Math.Cos(RoofPitch_radians);
            float fz2 = fz3 - fz1;

            return MWallHeightOverall - fz2;
        }

        private float GetOverallHeight_H2()
        {
            float fz1 = MainColumnCrsc_z_plus * (float)Math.Tan(RoofPitch_radians);
            float fz3 = MainRafterCrsc_z_plus / (float)Math.Cos(RoofPitch_radians);
            float fz2 = fz3 + fz1;

            if (MKitsetTypeIndex == 1) // Gable roof
                fz2 = fz3;

            return Height_H2 + fz2;
        }


        private float GetCenterlineHeight_H2()
        {
            float fz1 = MainColumnCrsc_z_plus * (float)Math.Tan(RoofPitch_radians);
            float fz3 = MainRafterCrsc_z_plus / (float)Math.Cos(RoofPitch_radians);
            float fz2 = fz3 + fz1;

            if (MKitsetTypeIndex == 1) // Gable roof
                fz2 = fz3;

            return Height_H2_Overall - fz2;
        }

        private Dictionary<EMemberType_FS_Position, CCalculMember> GetDesignResultsULS()
        {
            Dictionary<EMemberType_FS_Position, CCalculMember> dictULSDesignResults = new Dictionary<EMemberType_FS_Position, CCalculMember>();

            foreach (CMemberGroup mGr in Model.listOfModelMemberGroups)
            {
                //Tu je problem ze DesignResults nemaju hodnoty pre BracingBlockGirts = 12, BracingBlockPurlins = 13, BracingBlocksGirtsFrontSide = 14, BracingBlocksGirtsBackSide = 15,
                if (!sDesignResults_ULS.DesignResults.ContainsKey(mGr.MemberType_FS_Position)) continue;

                CLoadCombination governingLoadComb = sDesignResults_ULS.DesignResults[mGr.MemberType_FS_Position].GoverningLoadCombination;
                if (governingLoadComb == null) continue;
                CMember governingMember = sDesignResults_ULS.DesignResults[mGr.MemberType_FS_Position].MemberWithMaximumDesignRatio;
                if (governingMember == null) continue;
                CCalculMember cGoverningMemberResultsULS;
                CalculateGoverningMemberDesignDetails(UseCRSCGeometricalAxes, _designOptionsVM.ShearDesignAccording334, _designOptionsVM.IgnoreWebStiffeners, MemberDesignResults_ULS, governingMember, governingLoadComb.ID, out cGoverningMemberResultsULS);
                dictULSDesignResults.Add(mGr.MemberType_FS_Position, cGoverningMemberResultsULS);
            }
            return dictULSDesignResults;
        }

        private Dictionary<EMemberType_FS_Position, CCalculMember> GetDesignResultsSLS()
        {
            Dictionary<EMemberType_FS_Position, CCalculMember> dictSLSDesignResults = new Dictionary<EMemberType_FS_Position, CCalculMember>();

            foreach (CMemberGroup mGr in Model.listOfModelMemberGroups)
            {
                //Tu je problem ze DesignResults nemaju hodnoty pre BracingBlockGirts = 12, BracingBlockPurlins = 13, BracingBlocksGirtsFrontSide = 14, BracingBlocksGirtsBackSide = 15,
                if (!sDesignResults_SLS.DesignResults.ContainsKey(mGr.MemberType_FS_Position)) continue;

                CLoadCombination governingLoadComb = sDesignResults_SLS.DesignResults[mGr.MemberType_FS_Position].GoverningLoadCombination;
                if (governingLoadComb == null) continue;
                CMember governingMember = sDesignResults_SLS.DesignResults[mGr.MemberType_FS_Position].MemberWithMaximumDesignRatio;
                if (governingMember == null) continue;
                CCalculMember cGoverningMemberResultsSLS;
                CalculateGoverningMemberDesignDetails(UseCRSCGeometricalAxes, MemberDesignResults_SLS, governingMember, governingLoadComb.ID, mGr, out cGoverningMemberResultsSLS);
                dictSLSDesignResults.Add(mGr.MemberType_FS_Position, cGoverningMemberResultsSLS);
            }
            return dictSLSDesignResults;
        }

        // Calculate governing member design ratio
        private void CalculateGoverningMemberDesignDetails(bool bUseCRSCGeometricalAxes, bool bShearDesignAccording334, bool bIgnoreWebStiffeners, List<CMemberLoadCombinationRatio_ULS> DesignResults, CMember m, int loadCombID, out CCalculMember cGoverningMemberResults)
        {
            CMemberLoadCombinationRatio_ULS res = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombID);
            cGoverningMemberResults = new CCalculMember(false, bUseCRSCGeometricalAxes, bShearDesignAccording334, bIgnoreWebStiffeners, res.DesignInternalForces, m, res.DesignBucklingLengthFactors, res.DesignMomentValuesForCb);
        }

        public void CalculateGoverningMemberDesignDetails(bool bUseCRSCGeometricalAxes, List<CMemberLoadCombinationRatio_SLS> DesignResults, CMember m, int loadCombID, CMemberGroup GroupOfMembersWithSelectedType, out CCalculMember cGoverningMemberResults)
        {
            CMemberLoadCombinationRatio_SLS res = DesignResults.FirstOrDefault(i => i.Member.ID == m.ID && i.LoadCombination.ID == loadCombID);

            // Limit zavisi od typu zatazenia (load combination) a typu pruta
            float fDelfectionLimitFraction_Denominator = GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_Total;
            float fDeflectionLimit = GroupOfMembersWithSelectedType.DeflectionLimit_Total;

            //Mato??? toto nechapem, potrebujem vysvetlit
            // TO Ondrej: Ak je v kombinacii len load case typu permanent load, potrebujem nastavit do fDeflectionLimit hodnotu DeflectionLimit_PermanentLoad definovanu pre group, pre ine kombinacie pouzijem total
            // V nasom pripade mame v SLS len jednu taku kombinaciu, ta ma cislo 41, TO 41 by malo byt nahradene niecim inym, nie len ID, ked sa zmeni pocet kombinacii tak sa to pokazi
            // Da sa pouzit metoda z triedy CLoadCombination IsCombinationOfPermanentLoadCasesOnly()

            if (loadCombID == 41) // TODO Combination of permanent load (TODO - nacitat/zistit spravne parametre kombinacie (je typu SLS a obsahuje len load cases typu permanent), neurcovat podla cisla ID)
            {
                fDelfectionLimitFraction_Denominator = GroupOfMembersWithSelectedType.DeflectionLimitFraction_Denominator_PermanentLoad;
                fDeflectionLimit = GroupOfMembersWithSelectedType.DeflectionLimit_PermanentLoad;
            }

            cGoverningMemberResults = new CCalculMember(false, bUseCRSCGeometricalAxes, MKitsetTypeIndex == 1, res.DesignDeflections, m, fDelfectionLimitFraction_Denominator, fDeflectionLimit);
        }

        public void GetGoverningMemberJointsDesignDetails(out Dictionary<EMemberType_FS_Position, CCalculJoint> dictStartJointResults, out Dictionary<EMemberType_FS_Position, CCalculJoint> dictEndJointResults)
        {
            dictStartJointResults = new Dictionary<EMemberType_FS_Position, CCalculJoint>();
            dictEndJointResults = new Dictionary<EMemberType_FS_Position, CCalculJoint>();

            if (JointDesignResults_ULS == null) return;

            foreach (CMemberGroup mGr in Model.listOfModelMemberGroups)
            {
                //Tu je problem ze DesignResults nemaju hodnoty pre BracingBlockGirts = 12, BracingBlockPurlins = 13, BracingBlocksGirtsFrontSide = 14, BracingBlocksGirtsBackSide = 15,
                if (!sDesignResults_ULS.DesignResults.ContainsKey(mGr.MemberType_FS_Position)) continue;

                CLoadCombination governingLoadComb = sDesignResults_ULS.DesignResults[mGr.MemberType_FS_Position].GoverningLoadCombination;
                if (governingLoadComb == null) continue;
                CMember governingMember = sDesignResults_ULS.DesignResults[mGr.MemberType_FS_Position].MemberWithMaximumDesignRatio;
                if (governingMember == null) continue;

                CConnectionJointTypes cjStart = null;
                CConnectionJointTypes cjEnd = null;
                Model.GetModelMemberStartEndConnectionJoints(governingMember, out cjStart, out cjEnd);

                CJointLoadCombinationRatio_ULS resStart = JointDesignResults_ULS.FirstOrDefault(i => i.Member.ID == governingMember.ID && i.LoadCombination.ID == governingLoadComb.ID && i.Joint != null && cjStart != null && i.Joint.m_Node.ID == cjStart.m_Node.ID);
                CJointLoadCombinationRatio_ULS resEnd = JointDesignResults_ULS.FirstOrDefault(i => i.Member.ID == governingMember.ID && i.LoadCombination.ID == governingLoadComb.ID && i.Joint != null && cjEnd != null && i.Joint.m_Node.ID == cjEnd.m_Node.ID);
                if (resStart == null) continue;
                if (resEnd == null) continue;

                CalculationSettingsFoundation FootingCalcSettings = FootingVM.GetCalcSettings();

                CCalculJoint cGoverningMemberStartJointResults = new CCalculJoint(false, UseCRSCGeometricalAxes, _designOptionsVM.ShearDesignAccording334, _designOptionsVM.UniformShearDistributionInAnchors, cjStart, Model, FootingCalcSettings, resStart.DesignInternalForces, true);
                CCalculJoint cGoverningMemberEndJointResults = new CCalculJoint(false, UseCRSCGeometricalAxes, _designOptionsVM.ShearDesignAccording334, _designOptionsVM.UniformShearDistributionInAnchors, cjEnd, Model, FootingCalcSettings, resEnd.DesignInternalForces, true);

                dictStartJointResults.Add(mGr.MemberType_FS_Position, cGoverningMemberStartJointResults);
                dictEndJointResults.Add(mGr.MemberType_FS_Position, cGoverningMemberEndJointResults);
            }
        }

        public DisplayOptions GetDisplayOptions()
        {
            DisplayOptions sDisplayOptions = new DisplayOptions();
            // Get display options from GUI
            sDisplayOptions.bUseLightDirectional = _displayOptionsVM.LightDirectional;
            sDisplayOptions.bUseLightPoint = _displayOptionsVM.LightPoint;
            sDisplayOptions.bUseLightSpot = _displayOptionsVM.LightSpot;
            sDisplayOptions.bUseLightAmbient = _displayOptionsVM.LightAmbient;

            sDisplayOptions.bUseDiffuseMaterial = _displayOptionsVM.MaterialDiffuse;
            sDisplayOptions.bUseEmissiveMaterial = _displayOptionsVM.MaterialEmissive;

            sDisplayOptions.bDisplayMembers = _displayOptionsVM.DisplayMembers;
            sDisplayOptions.bDisplayJoints = _displayOptionsVM.DisplayJoints;
            sDisplayOptions.bDisplayPlates = _displayOptionsVM.DisplayPlates;
            sDisplayOptions.bDisplayConnectors = _displayOptionsVM.DisplayConnectors;
            sDisplayOptions.bDisplayNodes = _displayOptionsVM.DisplayNodes;

            sDisplayOptions.bDisplayFoundations = _displayOptionsVM.DisplayFoundations;
            sDisplayOptions.bDisplayReinforcementBars = _displayOptionsVM.DisplayReinforcementBars;
            sDisplayOptions.bDisplayFloorSlab = _displayOptionsVM.DisplayFloorSlab;
            sDisplayOptions.bDisplaySawCuts = _displayOptionsVM.DisplaySawCuts;
            sDisplayOptions.bDisplayControlJoints = _displayOptionsVM.DisplayControlJoints;
            sDisplayOptions.bDisplayNodalSupports = _displayOptionsVM.DisplayNodalSupports;

            sDisplayOptions.bDisplayMembersWireFrame = _displayOptionsVM.DisplayMembersWireFrame;
            sDisplayOptions.bDisplayJointsWireFrame = _displayOptionsVM.DisplayJointsWireFrame;
            sDisplayOptions.bDisplayPlatesWireFrame = _displayOptionsVM.DisplayPlatesWireFrame;
            sDisplayOptions.bDisplayConnectorsWireFrame = _displayOptionsVM.DisplayConnectorsWireFrame;
            sDisplayOptions.bDisplayNodesWireFrame = _displayOptionsVM.DisplayNodesWireFrame;
            sDisplayOptions.bDisplayFoundationsWireFrame = _displayOptionsVM.DisplayFoundationsWireFrame;
            sDisplayOptions.bDisplayReinforcementBarsWireFrame = _displayOptionsVM.DisplayReinforcementBarsWireFrame;
            sDisplayOptions.bDisplayFloorSlabWireFrame = _displayOptionsVM.DisplayFloorSlabWireFrame;
            sDisplayOptions.bDisplayCladdingWireFrame = _displayOptionsVM.DisplayCladdingWireFrame && _modelOptionsVM.EnableCladding;//bug 835 (pokial nie je EnableCladding tak ani bDisplayCladdingWireframe nemoze byt)
            sDisplayOptions.bDisplayFibreglassWireFrame = _displayOptionsVM.DisplayFibreglassWireFrame;
            sDisplayOptions.bDisplayDoorsWireFrame = _displayOptionsVM.DisplayDoorsWireFrame;
            sDisplayOptions.bDisplayWindowsWireFrame = _displayOptionsVM.DisplayWindowsWireFrame;
            sDisplayOptions.bDoorsSimpleSolidModel = _displayOptionsVM.DoorsSimpleSolidModel;
            sDisplayOptions.bDoorsSimpleWireframe = _displayOptionsVM.DoorsSimpleWireframe;
            sDisplayOptions.bWindowOutlineOnly = _displayOptionsVM.WindowOutlineOnly;

            sDisplayOptions.bDisplayCladding = _displayOptionsVM.DisplayCladding && _modelOptionsVM.EnableCladding; //bug 835 (pokial nie je EnableCladding tak ani bDisplayCladding nemoze byt)
            sDisplayOptions.bDisplayCladdingLeftWall = _displayOptionsVM.DisplayCladdingLeftWall;
            sDisplayOptions.bDisplayCladdingRightWall = _displayOptionsVM.DisplayCladdingRightWall;
            sDisplayOptions.bDisplayCladdingFrontWall = _displayOptionsVM.DisplayCladdingFrontWall;
            sDisplayOptions.bDisplayCladdingBackWall = _displayOptionsVM.DisplayCladdingBackWall;
            sDisplayOptions.bDisplayCladdingRoof = _displayOptionsVM.DisplayCladdingRoof;
            sDisplayOptions.bDisplayFibreglass = _displayOptionsVM.DisplayFibreglass;
            sDisplayOptions.bDisplayDoors = _displayOptionsVM.DisplayDoors;
            sDisplayOptions.bDisplayWindows = _displayOptionsVM.DisplayWindows;

            sDisplayOptions.bDisplayMemberDescription = _displayOptionsVM.ShowMemberDescription;
            sDisplayOptions.bDisplayMemberID = _displayOptionsVM.ShowMemberID;
            sDisplayOptions.bDisplayMemberPrefix = _displayOptionsVM.ShowMemberPrefix;
            sDisplayOptions.bDisplayMemberCrossSectionStartName = _displayOptionsVM.ShowMemberCrossSectionStartName;
            sDisplayOptions.bDisplayMemberRealLength = _displayOptionsVM.ShowMemberRealLength;
            sDisplayOptions.bDisplayMemberRealLengthInMM = _displayOptionsVM.ShowMemberRealLengthInMM;
            sDisplayOptions.bDisplayMemberRealLengthUnit = _displayOptionsVM.ShowMemberRealLengthUnit;
            sDisplayOptions.bDisplayNodesDescription = _displayOptionsVM.ShowNodesDescription;

            sDisplayOptions.bDisplayFoundationsDescription = _displayOptionsVM.ShowFoundationsDescription;
            sDisplayOptions.bDisplayFloorSlabDescription = _displayOptionsVM.ShowFloorSlabDescription;
            sDisplayOptions.bDisplaySawCutsDescription = _displayOptionsVM.ShowSawCutsDescription;
            sDisplayOptions.bDisplayControlJointsDescription = _displayOptionsVM.ShowControlJointsDescription;

            sDisplayOptions.bDisplayCladdingDescription = _displayOptionsVM.DisplayCladdingDescription;
            sDisplayOptions.bDisplayCladdingID = _displayOptionsVM.DisplayCladdingID;
            sDisplayOptions.bDisplayCladdingPrefix = _displayOptionsVM.DisplayCladdingPrefix;
            sDisplayOptions.bDisplayCladdingLengthWidth = _displayOptionsVM.DisplayCladdingLengthWidth;
            sDisplayOptions.bDisplayCladdingArea = _displayOptionsVM.DisplayCladdingArea;
            sDisplayOptions.bDisplayCladdingUnits = _displayOptionsVM.DisplayCladdingUnits;

            sDisplayOptions.bDisplayFibreglassDescription = _displayOptionsVM.DisplayFibreglassDescription;
            sDisplayOptions.bDisplayFibreglassID = _displayOptionsVM.DisplayFibreglassID;
            sDisplayOptions.bDisplayFibreglassPrefix = _displayOptionsVM.DisplayFibreglassPrefix;
            sDisplayOptions.bDisplayFibreglassLengthWidth = _displayOptionsVM.DisplayFibreglassLengthWidth;
            sDisplayOptions.bDisplayFibreglassArea = _displayOptionsVM.DisplayFibreglassArea;
            sDisplayOptions.bDisplayFibreglassUnits = _displayOptionsVM.DisplayFibreglassUnits;

            sDisplayOptions.bDisplayDoorDescription = _displayOptionsVM.DisplayDoorDescription;
            sDisplayOptions.bDisplayDoorID = _displayOptionsVM.DisplayDoorID;
            sDisplayOptions.bDisplayDoorType = _displayOptionsVM.DisplayDoorType;
            sDisplayOptions.bDisplayDoorHeightWidth = _displayOptionsVM.DisplayDoorHeightWidth;
            sDisplayOptions.bDisplayDoorArea = _displayOptionsVM.DisplayDoorArea;
            sDisplayOptions.bDisplayDoorUnits = _displayOptionsVM.DisplayDoorUnits;

            sDisplayOptions.bDisplayWindowDescription = _displayOptionsVM.DisplayWindowDescription;
            sDisplayOptions.bDisplayWindowID = _displayOptionsVM.DisplayWindowID;
            sDisplayOptions.bDisplayWindowHeightWidth = _displayOptionsVM.DisplayWindowHeightWidth;
            sDisplayOptions.bDisplayWindowArea = _displayOptionsVM.DisplayWindowArea;
            sDisplayOptions.bDisplayWindowUnits = _displayOptionsVM.DisplayWindowUnits;

            sDisplayOptions.bDisplayDimensions = _displayOptionsVM.ShowDimensions;
            sDisplayOptions.bDisplayGridlines = _displayOptionsVM.ShowGridLines;
            sDisplayOptions.bDisplaySectionSymbols = _displayOptionsVM.ShowSectionSymbols;
            sDisplayOptions.bDisplayDetailSymbols = _displayOptionsVM.ShowDetailSymbols;
            sDisplayOptions.bDisplaySlabRebates = _displayOptionsVM.ShowSlabRebates;

            sDisplayOptions.bDisplayMembersCenterLines = _displayOptionsVM.DisplayMembersCenterLines;
            sDisplayOptions.bDisplaySolidModel = _displayOptionsVM.DisplaySolidModel;
            sDisplayOptions.bDisplayWireFrameModel = _displayOptionsVM.DisplayWireFrameModel;

            sDisplayOptions.bDistinguishedColor = _displayOptionsVM.DisplayDistinguishedColorMember;
            //sDisplayOptions.bTransparentMemberModel = _displayOptionsVM.DisplayTransparentModelMember;

            sDisplayOptions.bDisplayGlobalAxis = _displayOptionsVM.ShowGlobalAxis;
            sDisplayOptions.bDisplayLocalMembersAxis = _displayOptionsVM.ShowLocalMembersAxis;
            sDisplayOptions.bDisplaySurfaceLoadAxis = _displayOptionsVM.ShowSurfaceLoadsAxis;

            sDisplayOptions.bDisplayLoads = _displayOptionsVM.ShowLoads;
            sDisplayOptions.bDisplayNodalLoads = _displayOptionsVM.ShowNodalLoads;
            sDisplayOptions.bDisplayMemberLoads = _displayOptionsVM.ShowLoadsOnMembers;
            sDisplayOptions.bDisplayMemberLoads_Girts = _displayOptionsVM.ShowLoadsOnGirts;
            sDisplayOptions.bDisplayMemberLoads_Purlins = _displayOptionsVM.ShowLoadsOnPurlins;
            sDisplayOptions.bDisplayMemberLoads_EavePurlins = _displayOptionsVM.ShowLoadsOnEavePurlins;
            sDisplayOptions.bDisplayMemberLoads_WindPosts = _displayOptionsVM.ShowLoadsOnWindPosts;
            sDisplayOptions.bDisplayMemberLoads_Frames = _displayOptionsVM.ShowLoadsOnFrameMembers;
            sDisplayOptions.bDisplaySurfaceLoads = _displayOptionsVM.ShowSurfaceLoads;

            sDisplayOptions.bDisplayLoadsLabels = _displayOptionsVM.ShowLoadsLabels;
            sDisplayOptions.bDisplayLoadsLabelsUnits = _displayOptionsVM.ShowLoadsLabelsUnits;

            sDisplayOptions.DisplayIn3DRatio = _displayOptionsVM.DisplayIn3DRatio;
            sDisplayOptions.bColorsAccordingToMembersPrefix = _displayOptionsVM.ColorsAccordingToMembersPrefix;
            sDisplayOptions.bColorsAccordingToMembersPosition = _displayOptionsVM.ColorsAccordingToMembersPosition;
            sDisplayOptions.bColorsAccordingToMembers = sDisplayOptions.bColorsAccordingToMembersPrefix || sDisplayOptions.bColorsAccordingToMembersPosition;

            sDisplayOptions.bColorsAccordingToSections = _displayOptionsVM.ColorsAccordingToSections;
            sDisplayOptions.bColoredCenterlines = _displayOptionsVM.ColoredCenterlines;

            //sDisplayOptions.wireFrameColor = WireframeColor;
            sDisplayOptions.wireFrameColor = _displayOptionsVM.WireframeColor;
            sDisplayOptions.fWireFrameLineThickness = _displayOptionsVM.WireFrameLineThickness;

            sDisplayOptions.memberCenterlineColor = _displayOptionsVM.MemberCenterlineColor;
            sDisplayOptions.fmemberCenterlineThickness = _displayOptionsVM.MemberCenterlineThickness;

            //sDisplayOptions.fNodeDescriptionTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            //sDisplayOptions.fMemberDescriptionTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            //sDisplayOptions.fDimensionTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            //sDisplayOptions.fGridLineLabelTextFontSize = 30; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            //sDisplayOptions.fSectionSymbolLabelTextFontSize = 30; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            //sDisplayOptions.fDetailSymbolLabelTextFontSize = 30; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report

            //sDisplayOptions.fSawCutTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            //sDisplayOptions.fControlJointTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report

            //sDisplayOptions.fFoundationTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            //sDisplayOptions.fFloorSlabTextFontSize = 12;  // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report

            sDisplayOptions.NodeColor = _displayOptionsVM.NodeColor;
            sDisplayOptions.NodeDescriptionTextColor = _displayOptionsVM.NodeDescriptionTextColor;
            sDisplayOptions.MemberDescriptionTextColor = _displayOptionsVM.MemberDescriptionTextColor;
            sDisplayOptions.DimensionTextColor = _displayOptionsVM.DimensionTextColor;
            sDisplayOptions.DimensionLineColor = _displayOptionsVM.DimensionLineColor;

            sDisplayOptions.GridLineLabelTextColor = _displayOptionsVM.GridLineLabelTextColor;
            sDisplayOptions.GridLineColor = _displayOptionsVM.GridLineColor;
            sDisplayOptions.GridLinePatternType = (ELinePatternType)_displayOptionsVM.GridLinePatternType;

            sDisplayOptions.SectionSymbolLabelTextColor = _displayOptionsVM.SectionSymbolLabelTextColor;
            sDisplayOptions.SectionSymbolColor = _displayOptionsVM.SectionSymbolColor;

            sDisplayOptions.DetailSymbolLabelTextColor = _displayOptionsVM.DetailSymbolLabelTextColor;
            sDisplayOptions.DetailSymbolLabelBackColor = _displayOptionsVM.DetailSymbolLabelBackColor;
            sDisplayOptions.DetailSymbolColor = _displayOptionsVM.DetailSymbolColor;

            sDisplayOptions.SawCutTextColor = _displayOptionsVM.SawCutTextColor;
            sDisplayOptions.SawCutLineColor = _displayOptionsVM.SawCutLineColor;
            sDisplayOptions.SawCutLinePatternType = (ELinePatternType)_displayOptionsVM.SawCutLinePatternType;

            sDisplayOptions.ControlJointTextColor = _displayOptionsVM.ControlJointTextColor;
            sDisplayOptions.ControlJointLineColor = _displayOptionsVM.ControlJointLineColor;
            sDisplayOptions.ControlJointLinePatternType = (ELinePatternType)_displayOptionsVM.ControlJointLinePatternType;

            sDisplayOptions.FoundationTextColor = _displayOptionsVM.FoundationTextColor;
            sDisplayOptions.FloorSlabTextColor = _displayOptionsVM.FloorSlabTextColor;

            sDisplayOptions.FoundationColor = _displayOptionsVM.FoundationColor;
            sDisplayOptions.FloorSlabColor = _displayOptionsVM.FloorSlabColor;

            sDisplayOptions.SlabRebateColor = _displayOptionsVM.SlabRebateColor;

            sDisplayOptions.CladdingTextColor = _displayOptionsVM.CladdingTextColor;
            sDisplayOptions.FibreglassTextColor = _displayOptionsVM.FibreglassTextColor;
            sDisplayOptions.DoorTextColor = _displayOptionsVM.DoorTextColor;
            sDisplayOptions.WindowTextColor = _displayOptionsVM.WindowTextColor;

            if (FootingVM != null)
            {
                sDisplayOptions.ReinforcementBarColor_Top_x = FootingVM.LongReinTop_x_Color;
                sDisplayOptions.ReinforcementBarColor_Top_y = FootingVM.LongReinTop_y_Color;
                sDisplayOptions.ReinforcementBarColor_Bottom_x = FootingVM.LongReinBottom_x_Color;
                sDisplayOptions.ReinforcementBarColor_Bottom_y = FootingVM.LongReinBottom_y_Color;
            }

            sDisplayOptions.PlateColor = _displayOptionsVM.PlateColor;
            sDisplayOptions.ScrewColor = _displayOptionsVM.ScrewColor;
            sDisplayOptions.AnchorColor = _displayOptionsVM.AnchorColor;
            sDisplayOptions.WasherColor = _displayOptionsVM.WasherColor;
            sDisplayOptions.NutColor = _displayOptionsVM.NutColor;

            sDisplayOptions.bCladdingSheetColoursByID = _displayOptionsVM.CladdingSheetColoursByID;
            sDisplayOptions.bUseTextures = _displayOptionsVM.UseTextures;
            sDisplayOptions.bUseTexturesMembers = _displayOptionsVM.UseTexturesMembers;
            sDisplayOptions.bUseTexturesPlates = _displayOptionsVM.UseTexturesPlates;
            sDisplayOptions.bUseTexturesCladding = _displayOptionsVM.UseTexturesCladding;
            sDisplayOptions.bUseDistColorOfSheetWithoutOverlap = _displayOptionsVM.UseDifColorForSheetWithOverlap;
            sDisplayOptions.CladdingSheetNoOverlapColor = _displayOptionsVM.CladdingSheetColor;
            sDisplayOptions.FibreglassSheetNoOverlapColor = _displayOptionsVM.FibreglassSheetColor;

            sDisplayOptions.fMemberSolidModelOpacity = _displayOptionsVM.MemberSolidModelOpacity;
            sDisplayOptions.fPlateSolidModelOpacity = _displayOptionsVM.PlateSolidModelOpacity;
            sDisplayOptions.fScrewSolidModelOpacity = _displayOptionsVM.ScrewSolidModelOpacity;
            sDisplayOptions.fAnchorSolidModelOpacity = _displayOptionsVM.AnchorSolidModelOpacity;
            sDisplayOptions.fFoundationSolidModelOpacity = _displayOptionsVM.FoundationSolidModelOpacity;
            sDisplayOptions.fReinforcementBarSolidModelOpacity = _displayOptionsVM.ReinforcementBarSolidModelOpacity;
            sDisplayOptions.fFloorSlabSolidModelOpacity = _displayOptionsVM.FloorSlabSolidModelOpacity;
            sDisplayOptions.fSlabRebateSolidModelOpacity = _displayOptionsVM.SlabRebateSolidModelOpacity;

            sDisplayOptions.fFrontCladdingOpacity = _displayOptionsVM.FrontCladdingOpacity;
            sDisplayOptions.fLeftCladdingOpacity = _displayOptionsVM.LeftCladdingOpacity;
            sDisplayOptions.fRoofCladdingOpacity = _displayOptionsVM.RoofCladdingOpacity;
            sDisplayOptions.fFlashingOpacity = _displayOptionsVM.FlashingOpacity;
            sDisplayOptions.fDoorPanelOpacity = _displayOptionsVM.DoorPanelOpacity;
            sDisplayOptions.fWindowPanelOpacity = _displayOptionsVM.WindowPanelOpacity;
            sDisplayOptions.fFibreglassOpacity = _displayOptionsVM.FibreglassOpacity;

            sDisplayOptions.backgroundColor = _displayOptionsVM.BackgroundColor;
            sDisplayOptions.ModelView = ViewIndex;
            sDisplayOptions.ViewModelMembers = ViewModelMemberFilterIndex;

            sDisplayOptions.IsExport = false;
            //To Mato tu sa daju ponastavovat velkosti relativne podla velkosti modelu
            sDisplayOptions.ExportFloorSlabTextSize = _displayOptionsVM.ExportFloorSlabTextSize;
            sDisplayOptions.ExportGridlinesSize = _displayOptionsVM.ExportGridlinesSize;
            sDisplayOptions.ExportGridLineLabelSize = _displayOptionsVM.ExportGridLineLabelSize;
            sDisplayOptions.ExportSectionSymbolsSize = _displayOptionsVM.ExportSectionSymbolsSize;
            sDisplayOptions.ExportSectionSymbolLabelSize = _displayOptionsVM.ExportSectionSymbolLabelSize;
            sDisplayOptions.ExportDetailSymbolSize = _displayOptionsVM.ExportDetailSymbolSize;
            sDisplayOptions.ExportDetailSymbolLabelSize = _displayOptionsVM.ExportDetailSymbolLabelSize;
            sDisplayOptions.ExportMembersDescriptionSize = _displayOptionsVM.ExportMembersDescriptionSize;
            sDisplayOptions.ExportNodesDescriptionSize = _displayOptionsVM.ExportNodesDescriptionSize;
            sDisplayOptions.ExportSawCutTextSize = _displayOptionsVM.ExportSawCutTextSize;
            sDisplayOptions.ExportControlJointTextSize = _displayOptionsVM.ExportControlJointTextSize;
            sDisplayOptions.ExportFoundationTextSize = _displayOptionsVM.ExportFoundationTextSize;
            sDisplayOptions.ExportDimensionsTextSize = _displayOptionsVM.ExportDimensionsTextSize;
            sDisplayOptions.ExportDimensionsLineRadius = _displayOptionsVM.ExportDimensionsLineRadius;
            sDisplayOptions.ExportDimensionsScale = _displayOptionsVM.ExportDimensionsScale;
            sDisplayOptions.ExportDescriptionTextWidthScaleFactor = _displayOptionsVM.ExportDescriptionTextWidthScaleFactor;
            sDisplayOptions.ExportCladdingDescriptionSize = _displayOptionsVM.ExportCladdingDescriptionSize;
            sDisplayOptions.ExportFibreglassDescriptionSize = _displayOptionsVM.ExportFibreglassDescriptionSize;
            sDisplayOptions.ExportDoorDescriptionSize = _displayOptionsVM.ExportDoorDescriptionSize;
            sDisplayOptions.ExportWindowDescriptionSize = _displayOptionsVM.ExportWindowDescriptionSize;

            sDisplayOptions.GUIFloorSlabTextSize = _displayOptionsVM.GUIFloorSlabTextSize;
            sDisplayOptions.GUIGridlinesSize = _displayOptionsVM.GUIGridlinesSize;
            sDisplayOptions.GUIGridLineLabelSize = _displayOptionsVM.GUIGridLineLabelSize;
            sDisplayOptions.GUISectionSymbolsSize = _displayOptionsVM.GUISectionSymbolsSize;
            sDisplayOptions.GUISectionSymbolLabelSize = _displayOptionsVM.GUISectionSymbolLabelSize;
            sDisplayOptions.GUIDetailSymbolSize = _displayOptionsVM.GUIDetailSymbolSize;
            sDisplayOptions.GUIDetailSymbolLabelSize = _displayOptionsVM.GUIDetailSymbolLabelSize;
            sDisplayOptions.GUIMembersDescriptionSize = _displayOptionsVM.GUIMembersDescriptionSize;
            sDisplayOptions.GUINodesDescriptionSize = _displayOptionsVM.GUINodesDescriptionSize;
            sDisplayOptions.GUISawCutTextSize = _displayOptionsVM.GUISawCutTextSize;
            sDisplayOptions.GUIControlJointTextSize = _displayOptionsVM.GUIControlJointTextSize;
            sDisplayOptions.GUIFoundationTextSize = _displayOptionsVM.GUIFoundationTextSize;
            sDisplayOptions.GUIDimensionsTextSize = _displayOptionsVM.GUIDimensionsTextSize;
            sDisplayOptions.GUIDimensionsLineRadius = _displayOptionsVM.GUIDimensionsLineRadius;
            sDisplayOptions.GUIDimensionsScale = _displayOptionsVM.GUIDimensionsScale;
            sDisplayOptions.GUIDescriptionTextWidthScaleFactor = _displayOptionsVM.GUIDescriptionTextWidthScaleFactor;
            sDisplayOptions.GUICladdingDescriptionSize = _displayOptionsVM.GUICladdingDescriptionSize;
            sDisplayOptions.GUIFibreglassDescriptionSize = _displayOptionsVM.GUIFibreglassDescriptionSize;
            sDisplayOptions.GUIDoorDescriptionSize = _displayOptionsVM.GUIDoorDescriptionSize;
            sDisplayOptions.GUIWindowDescriptionSize = _displayOptionsVM.GUIWindowDescriptionSize;

            return sDisplayOptions;
        }

        public void CountFlashings()
        {
            if (_doorsAndWindowsVM == null) return;
            if (_doorsAndWindowsVM.Flashings == null) return;

            float fRoofRidgeFlashing_TotalLength = 0;
            float fWallCornerFlashing_TotalLength = 0;
            float fBargeFlashing_TotalLength = 0;
            float fBargeBirdProofFlashing_TotalLength = 0;
            float fGutterEavePurlinBirdProofStrip_TotalLength = 0;
            float fFibreglassRoofRidgeCapFlashing_TotalLength = 0;

            if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 2 * WallHeightOverall + 2 * Height_H2_Overall;
                fBargeFlashing_TotalLength = 2 * RoofSideLength;
                fBargeBirdProofFlashing_TotalLength = 2 * RoofSideLength;
                fGutterEavePurlinBirdProofStrip_TotalLength = RoofLength_Y;
            }
            else if (KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                fRoofRidgeFlashing_TotalLength = RoofLength_Y;
                fWallCornerFlashing_TotalLength = 4 * WallHeightOverall;
                fBargeFlashing_TotalLength = 4 * RoofSideLength;
                fBargeBirdProofFlashing_TotalLength = 4 * RoofSideLength;
                fGutterEavePurlinBirdProofStrip_TotalLength = 2 * RoofLength_Y;

                fFibreglassRoofRidgeCapFlashing_TotalLength = 0; // TODO - zistit ktore FG sheet koncia na hrane strechy a spocitat ich sirku
            }
            else
            {
                // Exception - not implemented
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 0;
                fBargeFlashing_TotalLength = 0;
                fBargeBirdProofFlashing_TotalLength = 0;
                fGutterEavePurlinBirdProofStrip_TotalLength = 0;
                fFibreglassRoofRidgeCapFlashing_TotalLength = 0;
            }

            //To Mato - nie som si uplne isty, kde chceme toto nastavovat,ci tu, alebo vseobecne pri zmene modelu
            CAccessories_LengthItemProperties flashing = null;

            //Tab Accesories: tam su vsetky Flashings, aby bolo vidno co vsetko je na vyber pre dany typ budovy
            //Tab Quotation: tu sa uz konkretne riesi ktore Flashings su na danej budove(vypinaju sa tie,ktore nie su pouzite)

            // TO Ondrej - TOTO tu budeme musiet poupravovat podobne ako je to Cladding Accessories, nepocitalo sa s tym ze niektore casti budovy sa mozu vypinat

            if (_modelOptionsVM.EnableCladding) //to Mato-zaroven neviem coho vsetkeho sa to tyka
            {
                if (MathF.d_equal(TotalRoofArea, 0)) // if roof doesn't exist
                {
                    fRoofRidgeFlashing_TotalLength = 0;
                    fBargeFlashing_TotalLength = 0;
                    fBargeBirdProofFlashing_TotalLength = 0;
                    fGutterEavePurlinBirdProofStrip_TotalLength = 0;
                    fFibreglassRoofRidgeCapFlashing_TotalLength = 0;
                }

                if (MathF.d_equal(TotalWallArea, 0)) // if walls doen't exist
                {
                    fWallCornerFlashing_TotalLength = 0;
                    fBargeBirdProofFlashing_TotalLength = 0;
                }
                else
                {
                    fWallCornerFlashing_TotalLength = fWallCornerFlashing_TotalLength / 4.0f * GetCornersCount();
                }
            }
            else
            {
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 0;
                fBargeFlashing_TotalLength = 0;
                fBargeBirdProofFlashing_TotalLength = 0;
                fGutterEavePurlinBirdProofStrip_TotalLength = 0;
                fFibreglassRoofRidgeCapFlashing_TotalLength = 0;
            }

            flashing = _doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.ID == (int)EFlashingType.RoofRidge); // Roof Ridge
            if (flashing != null) flashing.Length_total = fRoofRidgeFlashing_TotalLength;

            flashing = _doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.ID == (int)EFlashingType.RoofRidgeSoftEdge); // Roof Ridge - Soft Edge
            if (flashing != null) flashing.Length_total = fRoofRidgeFlashing_TotalLength;

            flashing = _doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.ID == (int)EFlashingType.WallCorner); // Wall Corner
            if (flashing != null) flashing.Length_total = fWallCornerFlashing_TotalLength;

            flashing = _doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.ID == (int)EFlashingType.Barge); // Barge
            if (flashing != null) flashing.Length_total = fBargeFlashing_TotalLength;

            flashing = _doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.ID == (int)EFlashingType.BargeBirdproof); // Barge BirdProof
            if (flashing != null) flashing.Length_total = fBargeBirdProofFlashing_TotalLength;

            flashing = _doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.ID == (int)EFlashingType.EavePurlinBirdproofStrip); // Eave Purlin Birdproof Strip
            if (flashing != null) flashing.Length_total = fGutterEavePurlinBirdProofStrip_TotalLength;

            flashing = _doorsAndWindowsVM.Flashings.FirstOrDefault(f => f.ID == (int)EFlashingType.FibreglassRoofRidgeCap); // Fibreglass Roof Ridge Cap
            if (flashing != null) flashing.Length_total = fFibreglassRoofRidgeCapFlashing_TotalLength;
        }

        private int GetCornersCount()
        {
            int count = 0;
            if (!MathF.d_equal(WallAreaLeft, 0) && !MathF.d_equal(WallAreaFront, 0)) count++;
            if (!MathF.d_equal(WallAreaLeft, 0) && !MathF.d_equal(WallAreaBack, 0)) count++;
            if (!MathF.d_equal(WallAreaRight, 0) && !MathF.d_equal(WallAreaFront, 0)) count++;
            if (!MathF.d_equal(WallAreaRight, 0) && !MathF.d_equal(WallAreaBack, 0)) count++;
            return count;
        }

        public void CalculateCladdingParameters_Mato()
        {
            if (_claddingOptionsVM == null) return;

            AdditionalOffsetWall = 0.010; // 10 mm Aby nekolidovali plochy cladding s members
            AdditionalOffsetRoof = 0.010; // 10 mm Aby nekolidovali plochy cladding s members (cross-bracing) na streche

            // Pridame odsadenie aby prvky ramov konstrukcie vizualne nekolidovali s povrchom cladding
            double column_crsc_y_minus_temp = EdgeColumnCrsc_y_minus - AdditionalOffsetWall;
            double column_crsc_y_plus_temp = EdgeColumnCrsc_y_plus + AdditionalOffsetWall;
            double column_crsc_z_plus_temp = MainColumnCrsc_z_plus + AdditionalOffsetWall;

            double height_1_final = WallHeight + MainColumnCrsc_z_plus / Math.Cos(RoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon - prevziat z vypoctu polohy edge purlin
            double height_2_final = Height_H2 + MainColumnCrsc_z_plus / Math.Cos(RoofPitch_deg * Math.PI / 180); // TODO - dopocitat presne, zohladnit edge purlin a sklon

            Height_1_final_edge_LR_Wall = height_1_final - column_crsc_z_plus_temp * Math.Tan(RoofPitch_deg * Math.PI / 180);
            Height_2_final_edge_LR_Wall = height_2_final;

            double height_1_final_edge_Roof = height_1_final + AdditionalOffsetRoof - (column_crsc_z_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_LR_X) * Math.Tan(RoofPitch_deg * Math.PI / 180);
            double height_2_final_edge_Roof = height_2_final + AdditionalOffsetRoof;

            if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                Height_2_final_edge_LR_Wall = height_2_final + column_crsc_z_plus_temp * Math.Tan(RoofPitch_deg * Math.PI / 180);
                height_2_final_edge_Roof = height_2_final + AdditionalOffsetRoof + (column_crsc_z_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_LR_X) * Math.Tan(RoofPitch_deg * Math.PI / 180);
            }

            // Nastavime rovnaku vysku hornej hrany
            Height_1_final_edge_FB_Wall = Height_1_final_edge_LR_Wall;
            Height_2_final_edge_FB_Wall = Height_2_final_edge_LR_Wall;

            if (_claddingOptionsVM.ConsiderRoofCladdingFor_FB_WallHeight)
            {
                Height_1_final_edge_FB_Wall = Height_1_final_edge_FB_Wall + _claddingOptionsVM.RoofCladdingProps.height_m * Math.Tan(RoofPitch_deg * Math.PI / 180);
                Height_2_final_edge_FB_Wall = Height_2_final_edge_FB_Wall + _claddingOptionsVM.RoofCladdingProps.height_m * Math.Tan(RoofPitch_deg * Math.PI / 180);

                if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
                    Height_2_final_edge_FB_Wall = height_2_final + (column_crsc_z_plus_temp + _claddingOptionsVM.RoofCladdingProps.height_m) * Math.Tan(RoofPitch_deg * Math.PI / 180);
            }

            // Wall Cladding Edges

            Point3D pfront0_baseleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, _claddingOptionsVM.WallBottomOffset_Z);
            Point3D pfront1_baseright = new Point3D(Width + column_crsc_z_plus_temp, column_crsc_y_minus_temp, _claddingOptionsVM.WallBottomOffset_Z);

            Point3D pback0_baseleft = new Point3D(-column_crsc_z_plus_temp, Length + column_crsc_y_plus_temp, _claddingOptionsVM.WallBottomOffset_Z);
            Point3D pback1_baseright = new Point3D(Width + column_crsc_z_plus_temp, Length + column_crsc_y_plus_temp, _claddingOptionsVM.WallBottomOffset_Z);

            // Wall Points
            //Point3D pLRWall_front2_heightright = new Point3D();
            //Point3D pLRWall_back2_heightright = new Point3D();
            //Point3D pLRWall_front3_heightleft = new Point3D();
            //Point3D pLRWall_back3_heightleft = new Point3D();
            //Point3D pLRWall_front4_top = new Point3D();
            //Point3D pLRWall_back4_top = new Point3D();
            //
            //Point3D pFBWall_front2_heightright = new Point3D();
            //Point3D pFBWall_back2_heightright = new Point3D();
            //Point3D pFBWall_front3_heightleft = new Point3D();
            //Point3D pFBWall_back3_heightleft = new Point3D();
            //Point3D pFBWall_front4_top = new Point3D();
            //Point3D pFBWall_back4_top = new Point3D();

            // Roof Points - oddelene pretoze strecha ma presahy
            Point3D pRoof_front2_heightright = new Point3D();
            Point3D pRoof_back2_heightright = new Point3D();
            Point3D pRoof_front3_heightleft = new Point3D();
            Point3D pRoof_back3_heightleft = new Point3D();
            Point3D pRoof_front4_top = new Point3D();
            Point3D pRoof_back4_top = new Point3D();

            List<Point> WallDefinitionPoints_FrontOrBack_Cladding; // Cladding (od hrany WallBottomOffset_Z)
            List<Point> WallDefinitionPoints_FrontOrBack_Netto; // Od podlahy - (+0.000)

            // Roof edge offset from centerline in Y-direction
            float fRoofEdgeOffsetFromCenterline = -(float)column_crsc_y_minus_temp + (float)_claddingOptionsVM.RoofEdgeOverHang_FB_Y;

            // Toto jedine som dolnil a inicializoval, ale realne nie je dana premenna nikde pouzita
            // TO ONDREJ Pouziva len dalej v kode CCladding.cs, v tychto ifoch a len nastaví
            int iNumberOfFrontBackWallEdges = 0;

            int iNumberOfRoofSides = 0; // Number of roof planes (2 - gable, 1 - monopitch)

            RoofLength_Y = (float)(fRoofEdgeOffsetFromCenterline + Length + column_crsc_y_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_FB_Y);

            // Nastavenie bodov suradnic hornych bodov stien a bodov strechy pre monopitch a gable roof model
            if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
            {
                // Monopitch Roof

                // Wall
                iNumberOfFrontBackWallEdges = 4;
                //pLRWall_front2_heightright = new Point3D(Width + column_crsc_z_plus_temp, column_crsc_y_minus_temp, Height_2_final_edge_FB_Wall);
                //pLRWall_front3_heightleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, Height_1_final_edge_FB_Wall);

                //pLRWall_back2_heightright = new Point3D(Width + column_crsc_z_plus_temp, Length + column_crsc_y_plus_temp, Height_2_final_edge_FB_Wall);
                //pLRWall_back3_heightleft = new Point3D(-column_crsc_z_plus_temp, Length + column_crsc_y_plus_temp, Height_1_final_edge_FB_Wall);

                //pFBWall_front2_heightright = new Point3D(pLRWall_front2_heightright.X, pLRWall_front2_heightright.Y, Height_2_final_edge_FB_Wall);
                //pFBWall_back2_heightright = new Point3D(pLRWall_back2_heightright.X, pLRWall_back2_heightright.Y, Height_2_final_edge_FB_Wall);
                //pFBWall_front3_heightleft = new Point3D(pLRWall_front3_heightleft.X, pLRWall_front3_heightleft.Y, Height_1_final_edge_FB_Wall);
                //pFBWall_back3_heightleft = new Point3D(pLRWall_back3_heightleft.X, pLRWall_back3_heightleft.Y, Height_1_final_edge_FB_Wall);

                WallDefinitionPoints_FrontOrBack_Cladding = new List<Point>(4) {
                    new Point(-column_crsc_z_plus_temp, _claddingOptionsVM.WallBottomOffset_Z),
                    new Point(Width + column_crsc_z_plus_temp, _claddingOptionsVM.WallBottomOffset_Z),
                    new Point(Width + column_crsc_z_plus_temp, Height_2_final_edge_FB_Wall),
                    new Point(-column_crsc_z_plus_temp, Height_1_final_edge_FB_Wall) };

                // Bez bottom offset pre cladding
                WallDefinitionPoints_FrontOrBack_Netto = new List<Point>(4) {
                    new Point(-column_crsc_z_plus_temp, 0),
                    new Point(Width + column_crsc_z_plus_temp, 0),
                    new Point(Width + column_crsc_z_plus_temp, Height_2_final_edge_FB_Wall),
                    new Point(-column_crsc_z_plus_temp, Height_1_final_edge_FB_Wall) };

                // Roof
                iNumberOfRoofSides = 1;
                pRoof_front2_heightright = new Point3D(Width + column_crsc_z_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_LR_X, column_crsc_y_minus_temp - _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_2_final_edge_Roof);
                pRoof_front3_heightleft = new Point3D(-column_crsc_z_plus_temp - _claddingOptionsVM.RoofEdgeOverHang_LR_X, column_crsc_y_minus_temp - _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_1_final_edge_Roof);

                pRoof_back2_heightright = new Point3D(Width + column_crsc_z_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_LR_X, Length + column_crsc_y_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_2_final_edge_Roof);
                pRoof_back3_heightleft = new Point3D(-column_crsc_z_plus_temp - _claddingOptionsVM.RoofEdgeOverHang_LR_X, Length + column_crsc_y_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_1_final_edge_Roof);

                RoofSideLength = Drawing3D.GetPoint3DDistanceFloat(pRoof_front3_heightleft, pRoof_front2_heightright);
            }
            else if (KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
            {
                // Gable Roof Building

                // Wall
                iNumberOfFrontBackWallEdges = 5;
                //pLRWall_front2_heightright = new Point3D(Width + column_crsc_z_plus_temp, column_crsc_y_minus_temp, Height_1_final_edge_LR_Wall);
                //pLRWall_front3_heightleft = new Point3D(-column_crsc_z_plus_temp, column_crsc_y_minus_temp, Height_1_final_edge_LR_Wall);
                //pLRWall_front4_top = new Point3D(0.5 * Width, column_crsc_y_minus_temp, Height_2_final_edge_LR_Wall);

                //pLRWall_back2_heightright = new Point3D(Width + column_crsc_z_plus_temp, Length + column_crsc_y_plus_temp, Height_1_final_edge_LR_Wall);
                //pLRWall_back3_heightleft = new Point3D(-column_crsc_z_plus_temp, Length + column_crsc_y_plus_temp, Height_1_final_edge_LR_Wall);
                //pLRWall_back4_top = new Point3D(0.5 * Width, Length + column_crsc_y_plus_temp, Height_2_final_edge_LR_Wall);

                //pFBWall_front2_heightright = new Point3D(pLRWall_front2_heightright.X, pLRWall_front2_heightright.Y, Height_1_final_edge_FB_Wall);
                //pFBWall_back2_heightright = new Point3D(pLRWall_back2_heightright.X, pLRWall_back2_heightright.Y, Height_1_final_edge_FB_Wall);
                //pFBWall_front3_heightleft = new Point3D(pLRWall_front3_heightleft.X, pLRWall_front3_heightleft.Y, Height_1_final_edge_FB_Wall);
                //pFBWall_back3_heightleft = new Point3D(pLRWall_back3_heightleft.X, pLRWall_back3_heightleft.Y, Height_1_final_edge_FB_Wall);
                //pFBWall_front4_top = new Point3D(pLRWall_front4_top.X, pLRWall_front4_top.Y, Height_2_final_edge_FB_Wall);
                //pFBWall_back4_top = new Point3D(pLRWall_back4_top.X, pLRWall_back4_top.Y, Height_2_final_edge_FB_Wall);

                WallDefinitionPoints_FrontOrBack_Cladding = new List<Point>(5) {
                    new Point(-column_crsc_z_plus_temp, _claddingOptionsVM.WallBottomOffset_Z),
                    new Point(Width + column_crsc_z_plus_temp, _claddingOptionsVM.WallBottomOffset_Z),
                    new Point(Width + column_crsc_z_plus_temp, Height_1_final_edge_FB_Wall),
                    new Point(0.5 * Width, Height_2_final_edge_FB_Wall),
                    new Point(-column_crsc_z_plus_temp, Height_1_final_edge_FB_Wall) };

                // Bez bottom offset pre cladding
                WallDefinitionPoints_FrontOrBack_Netto = new List<Point>(5) {
                    new Point(-column_crsc_z_plus_temp, 0),
                    new Point(Width + column_crsc_z_plus_temp, 0),
                    new Point(Width + column_crsc_z_plus_temp, Height_1_final_edge_FB_Wall),
                    new Point(0.5 * Width, Height_2_final_edge_FB_Wall),
                    new Point(-column_crsc_z_plus_temp, Height_1_final_edge_FB_Wall) };

                // Roof
                iNumberOfRoofSides = 2;
                pRoof_front2_heightright = new Point3D(Width + column_crsc_z_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_LR_X, column_crsc_y_minus_temp - _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_1_final_edge_Roof);
                pRoof_front3_heightleft = new Point3D(-column_crsc_z_plus_temp - _claddingOptionsVM.RoofEdgeOverHang_LR_X, column_crsc_y_minus_temp - _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_1_final_edge_Roof);
                pRoof_front4_top = new Point3D(0.5 * Width, column_crsc_y_minus_temp - _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_2_final_edge_Roof);

                pRoof_back2_heightright = new Point3D(Width + column_crsc_z_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_LR_X, Length + column_crsc_y_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_1_final_edge_Roof);
                pRoof_back3_heightleft = new Point3D(-column_crsc_z_plus_temp - _claddingOptionsVM.RoofEdgeOverHang_LR_X, Length + column_crsc_y_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_1_final_edge_Roof);
                pRoof_back4_top = new Point3D(0.5 * Width, Length + column_crsc_y_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_FB_Y, height_2_final_edge_Roof);

                RoofSideLength = Drawing3D.GetPoint3DDistanceFloat(pRoof_front2_heightright, pRoof_front4_top);
            }
            else
            {
                throw new Exception("Not implemented kitset type.");
            }

            // Canopies
            float totalAreaOfCanopies = 0;
            double canopyOverhangOffset_y = _claddingOptionsVM.RoofEdgeOverHang_FB_Y; // TODO - zadavat v GUI ako cladding property pre roof, toto bude pre roof a canopy rovnake

            // Canopies
            foreach (CCanopiesInfo canopy in _canopiesOptionsVM.CanopiesList)
            {
                double width_temp;

                if (canopy.Right)
                {
                    bool hasNextCanopy = ModelHelper.IsNeighboringRightCanopy(_canopiesOptionsVM.CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1));
                    bool hasPreviousCanopy = ModelHelper.IsNeighboringRightCanopy(_canopiesOptionsVM.CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1));

                    float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? _claddingOptionsVM.RoofEdgeOverHang_FB_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                    float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == _canopiesOptionsVM.CanopiesList.Count - 1) ? _claddingOptionsVM.RoofEdgeOverHang_FB_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                    float fBayStartCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, _baysWidthOptionsVM.BayWidthList) - fCanopyBayStartOffset;
                    float fBayEndCoordinate_Y_Right = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, _baysWidthOptionsVM.BayWidthList) + fCanopyBayEndOffset;

                    float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Right + fRoofEdgeOffsetFromCenterline;
                    int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / _claddingOptionsVM.RoofCladdingProps.widthRib_m);
                    double dWidthOfWholeRibs = iNumberOfWholeRibs * _claddingOptionsVM.RoofCladdingProps.widthRib_m;
                    double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                    float fCanopyCladdingWidth = (float)canopy.WidthRight + _claddingOptionsVM.CanopyRoofEdgeOverHang_LR_X - (float)column_crsc_z_plus_temp - _claddingOptionsVM.RoofEdgeOverHang_LR_X;

                    float fCanopy_EdgeCoordinate_z = 0;
                    Point3D pfront_left = new Point3D();
                    Point3D pback_left = new Point3D();
                    if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
                    {
                        fCanopy_EdgeCoordinate_z = (float)height_2_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(RoofPitch_deg * Math.PI / 180);
                        pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y_Right, height_2_final_edge_Roof);
                        pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y_Right, height_2_final_edge_Roof);
                    }
                    else if (KitsetTypeIndex == (int)EModelType_FS.eKitsetGableRoofEnclosed)
                    {
                        fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-RoofPitch_deg * Math.PI / 180);
                        pfront_left = new Point3D(pRoof_front2_heightright.X, fBayStartCoordinate_Y_Right, height_1_final_edge_Roof);
                        pback_left = new Point3D(pRoof_back2_heightright.X, fBayEndCoordinate_Y_Right, height_1_final_edge_Roof);
                    }
                    Point3D pfront_right = new Point3D(Width + (float)column_crsc_z_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_LR_X + fCanopyCladdingWidth, fBayStartCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);
                    Point3D pback_right = new Point3D(Width + (float)column_crsc_z_plus_temp + _claddingOptionsVM.RoofEdgeOverHang_LR_X + fCanopyCladdingWidth, fBayEndCoordinate_Y_Right, fCanopy_EdgeCoordinate_z);

                    float poinstsDist = Drawing3D.GetPoint3DDistanceFloat(pfront_right, pfront_left);

                    //double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

                    width_temp = pback_left.Y - pfront_left.Y;

                    totalAreaOfCanopies += poinstsDist * (float)width_temp;
                }

                if (canopy.Left)
                {
                    // 2 ______ 1
                    //  |      |
                    //  |      |
                    //  |______|
                    // 3        0

                    bool hasNextCanopy = ModelHelper.IsNeighboringLeftCanopy(_canopiesOptionsVM.CanopiesList.ElementAtOrDefault(canopy.BayIndex + 1));
                    bool hasPreviousCanopy = ModelHelper.IsNeighboringLeftCanopy(_canopiesOptionsVM.CanopiesList.ElementAtOrDefault(canopy.BayIndex - 1));

                    float fCanopyBayStartOffset = hasPreviousCanopy ? 0f : ((canopy.BayIndex == 0 ? _claddingOptionsVM.RoofEdgeOverHang_FB_Y : (float)canopyOverhangOffset_y) - (float)column_crsc_y_minus_temp); // Positive value
                    float fCanopyBayEndOffset = hasNextCanopy ? 0f : (((canopy.BayIndex == _canopiesOptionsVM.CanopiesList.Count - 1) ? _claddingOptionsVM.RoofEdgeOverHang_FB_Y : (float)canopyOverhangOffset_y) + (float)column_crsc_y_plus_temp);

                    float fBayStartCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex, _baysWidthOptionsVM.BayWidthList) - fCanopyBayStartOffset;
                    float fBayEndCoordinate_Y_Left = ModelHelper.GetBaysWidthUntil(canopy.BayIndex + 1, _baysWidthOptionsVM.BayWidthList) + fCanopyBayEndOffset;

                    float fBayStartCoordinateFromRoofEdge = fBayStartCoordinate_Y_Left + fRoofEdgeOffsetFromCenterline;
                    int iNumberOfWholeRibs = (int)(fBayStartCoordinateFromRoofEdge / _claddingOptionsVM.RoofCladdingProps.widthRib_m);
                    double dWidthOfWholeRibs = iNumberOfWholeRibs * _claddingOptionsVM.RoofCladdingProps.widthRib_m;
                    double dPartialRib = fBayStartCoordinateFromRoofEdge - dWidthOfWholeRibs; // To Ondrej - Posun rebier v metroch

                    float fCanopyCladdingWidth = (float)canopy.WidthLeft + _claddingOptionsVM.CanopyRoofEdgeOverHang_LR_X - (float)column_crsc_z_plus_temp - _claddingOptionsVM.RoofEdgeOverHang_LR_X;
                    float fCanopy_EdgeCoordinate_z = (float)height_1_final_edge_Roof + fCanopyCladdingWidth * (float)Math.Tan(-RoofPitch_deg * Math.PI / 180);

                    Point3D pfront_left = new Point3D(-(float)column_crsc_z_plus_temp - _claddingOptionsVM.RoofEdgeOverHang_LR_X - fCanopyCladdingWidth, fBayStartCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                    Point3D pback_left = new Point3D(-(float)column_crsc_z_plus_temp - _claddingOptionsVM.RoofEdgeOverHang_LR_X - fCanopyCladdingWidth, fBayEndCoordinate_Y_Left, fCanopy_EdgeCoordinate_z);
                    Point3D pfront_right = new Point3D(pRoof_front3_heightleft.X, fBayStartCoordinate_Y_Left, height_1_final_edge_Roof);
                    Point3D pback_right = new Point3D(pRoof_back3_heightleft.X, fBayEndCoordinate_Y_Left, height_1_final_edge_Roof);

                    float poinstsDist = Drawing3D.GetPoint3DDistanceFloat(pfront_right, pfront_left);

                    //double wpWidthOffset = dPartialRib / (pback_left.Y - pfront_left.Y); // To Ondrej - Posun rebier relativne

                    width_temp = pback_left.Y - pfront_left.Y;

                    totalAreaOfCanopies += poinstsDist * (float)width_temp;
                }
            }

            float fWallArea_Left = 0; float fWallArea_Right = 0;

            if(ModelHasLeftWall()) fWallArea_Left = (float)(LengthOverall + 2 * AdditionalOffsetWall) * (float)(Height_1_final_edge_LR_Wall - _claddingOptionsVM.WallBottomOffset_Z);

            if(ModelHasRightWall())
            {
                if (KitsetTypeIndex == (int)EModelType_FS.eKitsetMonoRoofEnclosed)
                    fWallArea_Right = (float)(LengthOverall + 2 * AdditionalOffsetWall) * (float)(Height_2_final_edge_LR_Wall - _claddingOptionsVM.WallBottomOffset_Z);
                else
                    fWallArea_Right = (float)(LengthOverall + 2 * AdditionalOffsetWall) * (float)(Height_1_final_edge_LR_Wall - _claddingOptionsVM.WallBottomOffset_Z);
            }

            float fWallArea_Front = 0;
            if (ModelHasFrontWall()) fWallArea_Front = Geom2D.PolygonArea(WallDefinitionPoints_FrontOrBack_Cladding.ToArray());

            float fWallArea_Back = 0;
            if (ModelHasBackWall()) fWallArea_Back = Geom2D.PolygonArea(WallDefinitionPoints_FrontOrBack_Cladding.ToArray());

            BuildingArea_Gross = WidthOverall * LengthOverall;
            BuildingVolume_Gross = Geom2D.PolygonArea(WallDefinitionPoints_FrontOrBack_Netto.ToArray()) * LengthOverall; // Bez bottom offset pre cladding

            WallAreaLeft = fWallArea_Left; // To Ondrej potrebujeme tieto lokalne premenne a potom aj properties ???
            WallAreaRight = fWallArea_Right;
            WallAreaFront = fWallArea_Front;
            WallAreaBack = fWallArea_Back;
            TotalWallArea = fWallArea_Left + fWallArea_Right + fWallArea_Front + fWallArea_Back;

            CComponentInfo purlin = ComponentList.FirstOrDefault(x => x.MemberTypePosition == EMemberType_FS_Position.Purlin);
            if (purlin != null && purlin.Generate == true)
            {
                TotalRoofArea = iNumberOfRoofSides * RoofSideLength * RoofLength_Y;
                TotalRoofAreaInclCanopies = TotalRoofArea + totalAreaOfCanopies;
            }
            else { TotalRoofArea = 0; TotalRoofAreaInclCanopies = 0; }
        }

        //To Mato - alebo to nazveme ModelHasCladding?
        public bool ModelHasPurlinsOrGirts()
        {
            if (ModelHasRoof()) return true;
            if (ModelHasFrontWall()) return true;
            if (ModelHasBackWall()) return true;
            if (ModelHasLeftWall()) return true;
            if (ModelHasRightWall()) return true;

            return false;
        }
        public bool ModelHasWalls()
        {
            if (ModelHasFrontWall()) return true;
            if (ModelHasBackWall()) return true;
            if (ModelHasLeftWall()) return true;
            if (ModelHasRightWall()) return true;

            return false;
        }
        public bool ModelHasRoof()
        {
            CComponentInfo purlin = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Purlin);
            if (purlin != null && purlin.Generate == true) return true;
            else return false;
        }
        public bool ModelHasFrontWall()
        {
            CComponentInfo gF = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.GirtFrontSide);
            if (gF != null && gF.Generate == true) return true;
            else return false;
        }
        public bool ModelHasBackWall()
        {
            CComponentInfo gB = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.GirtBackSide);
            if (gB != null && gB.Generate == true) return true;
            else return false;
        }
        public bool ModelHasLeftWall()
        {
            CComponentInfo gL = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Girt);
            if (gL != null && gL.Generate == true) return true;
            else return false;
        }
        public bool ModelHasRightWall()
        {
            CComponentInfo gR = ComponentList.LastOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Girt);
            if (gR != null && gR.Generate == true) return true;
            else return false;
        }
    }
}