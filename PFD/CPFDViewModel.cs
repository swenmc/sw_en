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
using BriefFiniteElementNet;
using PFD.Infrastructure;
using System.Collections.ObjectModel;
using DATABASE.DTO;
using EXPIMP;
using M_AS4600;
using BaseClasses.Helpers;
using System.Windows.Media;
using DATABASE;
using System.Globalization;

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
        private float MRoofPitch_deg;
        private int MFrames;
        private float MGirtDistance;
        private float MPurlinDistance;
        private float MColumnDistance;
        private float MBottomGirtPosition;
        private float MFrontFrameRakeAngle;
        private float MBackFrameRakeAngle;
        private int MRoofCladdingIndex;
        private int MRoofCladdingID;
        private int MRoofCladdingCoatingIndex;
        private int MRoofCladdingCoatingID;
        private List<CoatingColour> MRoofCladdingColors;
        private int MRoofCladdingColorIndex;
        private int MRoofCladdingThicknessIndex;
        private int MWallCladdingIndex;
        private int MWallCladdingID;
        private int MWallCladdingCoatingIndex;
        private int MWallCladdingCoatingID;
        private List<CoatingColour> MWallCladdingColors;
        private int MWallCladdingColorIndex;
        private int MWallCladdingThicknessIndex;

        private int MOneRafterPurlinNo;

        private int MRoofFibreglassThicknessIndex;
        private int MWallFibreglassThicknessIndex;

        private int MSupportTypeIndex;

        private float MFibreglassAreaRoof;
        private float MFibreglassAreaWall;

        private List<string> m_Claddings;
        private List<string> m_Coatings;
        private string m_RoofCladding;
        private string m_WallCladding;
        private string m_RoofCladdingCoating;
        private string m_WallCladdingCoating;

        private List<string> m_RoofCladdingsThicknessTypes;
        private List<string> m_WallCladdingsThicknessTypes;
        private string m_RoofCladdingThickness;
        private string m_WallCladdingThickness;

        private List<string> m_RoofFibreglassThicknessTypes;
        private List<string> m_WallFibreglassThicknessTypes;

        private List<string> m_SupportTypes;
        private List<string> m_ModelTypes;
        private List<string> m_KitsetTypes;


        //private int MWireframeColorIndex;
        //public Color WireframeColor;

        private int MViewIndex;
        private int MViewModelMemberFilterIndex;

        private bool MSynchronizeGUI;
        private bool MRecreateModel;

        private int MLoadCaseIndex;

        private int iFrontColumnNoInOneFrame;
        private bool m_TransformScreenLines3DToCylinders3D;

        private bool m_GeneralOptionsChanged;
        private bool m_SolverOptionsChanged;
        private bool m_DesignOptionsChanged;
        private bool m_DisplayOptionsChanged;
        private bool m_CrossBracingOptionsChanged;
        private bool m_BaysWidthOptionsChanged;

        //private bool m_BracingEverySecondRowOfGirts;
        //private bool m_BracingEverySecondRowOfPurlins;

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

        private ObservableCollection<DoorProperties> MDoorBlocksProperties;
        private ObservableCollection<WindowProperties> MWindowBlocksProperties;
        private List<string> MBuildingSides;
        private List<string> MDoorsTypes;
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
        //tieto treba spracovat nejako
        public float fBayWidth;
        public float fHeight_H2; // Apex height for gable roof or right side wall heigth for monopitch roof
        public float fRoofPitch_radians;
        public float fMaterial_density = 7850f; // [kg /m^3] (malo by byt zadane v databaze materialov)

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
        public sDesignResults sDesignResults_ULSandSLS = new sDesignResults();
        [NonSerialized]
        public sDesignResults sDesignResults_ULS = new sDesignResults();
        [NonSerialized]
        public sDesignResults sDesignResults_SLS = new sDesignResults();

        [NonSerialized]
        public List<CFrame> frameModels;
        [NonSerialized]
        public List<CBeam_Simple> beamSimpleModels;

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
                Width = dmodel.fb;
                Length = dmodel.fL;
                WallHeight = dmodel.fh;
                RoofPitch_deg = dmodel.fRoof_Pitch_deg;
                GirtDistance = dmodel.fdist_girt;
                PurlinDistance = dmodel.fdist_purlin;
                ColumnDistance = dmodel.fdist_frontcolumn;
                BottomGirtPosition = dmodel.fdist_girt_bottom;
                Frames = dmodel.iFrNo;
                fBayWidth = MLength / (MFrames - 1);

                FrontFrameRakeAngle = dmodel.fRakeAngleFrontFrame_deg;
                BackFrameRakeAngle = dmodel.fRakeAngleBackFrame_deg;
                                
                _componentVM.SetModelComponentListProperties(dmodel.MembersSectionsDict); //set default components sections

                _componentVM.SetILSProperties(dmodel);

                SetResultsAreNotValid();

                //tieto riadky by som tu najradsej nemal, resp. ich nejako spracoval ako dalsie property

                // TO Ondrej - prerob to ako treba
                // Povodne to bolo tak ze properties boli len parametre ktore boli zadavane v GUI
                // Ak je programatorsky spravnejsie, ze ma byt vsetko co sa tu pouziva property, tak nemam namietky

                
                fRoofPitch_radians = MRoofPitch_deg * MathF.fPI / 180f;

                if (MKitsetTypeIndex == 0)
                {
                    fHeight_H2 = MWallHeight + MWidth * (float)Math.Tan(fRoofPitch_radians);

                    // Re-calculate value of distance between columns (number of columns per frame is always even
                    int iOneRafterFrontColumnNo = (int)((MWidth - 0.95 * MColumnDistance) / MColumnDistance);
                    IFrontColumnNoInOneFrame = 1 * iOneRafterFrontColumnNo;
                }
                else if (MKitsetTypeIndex == 1)
                {
                    fHeight_H2 = MWallHeight + 0.5f * MWidth * (float)Math.Tan(fRoofPitch_radians);

                    // Re-calculate value of distance between columns (number of columns per frame is always even
                    int iOneRafterFrontColumnNo = (int)((0.5f * MWidth - 0.45f * MColumnDistance) / MColumnDistance);
                    IFrontColumnNoInOneFrame = 2 * iOneRafterFrontColumnNo;
                }
                else
                {
                    fHeight_H2 = 0; // Exception
                    IFrontColumnNoInOneFrame = 0;
                }

                MColumnDistance = MWidth / (IFrontColumnNoInOneFrame + 1); // Update distance between columns

                RoofCladdingIndex = 1;
                RoofCladdingCoatingIndex = 1;

                RoofCladdingColorIndex = 8;
                WallCladdingIndex = 0;

                WallCladdingCoatingIndex = 1;
                WallCladdingColorIndex = 8;
                SupportTypeIndex = 1; // Pinned // Defaultna hodnota indexu v comboboxe

                FibreglassAreaRoof = 0; // % 0-ziadne fibreglass, 99 - takmer cela strecha fibreglass
                FibreglassAreaWall = 0; // % 0-ziadne fibreglass, 99 - takmer cela strecha fibreglass

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

                if (MModelIndex != 0)
                {
                    // UHOL ZACHOVAME ROVNAKY - V OPACNOM PRIPADE SA NEUPDATOVALA SPRAVNE VYSKA h2

                    // Recalculate roof pitch
                    //fRoofPitch_radians = (float)Math.Atan((fHeight_H2 - MWallHeight) / (0.5f * MWidth));
                    // Set new value in GUI
                    //MRoofPitch_deg = (fRoofPitch_radians * 180f / MathF.fPI);
                    // Recalculate roof height

                    if (MKitsetTypeIndex == 0)
                    {
                        fHeight_H2 = MWallHeight + MWidth * (float)Math.Tan(fRoofPitch_radians);

                        // Re-calculate value of distance between columns (number of columns per frame is always even
                        int iOneRafterFrontColumnNo = (int)((MWidth - 0.95 * MColumnDistance) / MColumnDistance);
                        IFrontColumnNoInOneFrame = 1 * iOneRafterFrontColumnNo;
                    }
                    else if (MKitsetTypeIndex == 1)
                    {
                        fHeight_H2 = MWallHeight + 0.5f * MWidth * (float)Math.Tan(fRoofPitch_radians);

                        // Re-calculate value of distance between columns (number of columns per frame is always even
                        int iOneRafterFrontColumnNo = (int)((0.5f * MWidth - 0.45f * MColumnDistance) / MColumnDistance);
                        IFrontColumnNoInOneFrame = 2 * iOneRafterFrontColumnNo;
                    }
                    else
                    {
                        fHeight_H2 = 0; // Exception
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
                NotifyPropertyChanged("Width");
            }
        }

        private void SetCustomModel()
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

                if (MModelIndex != 0)
                {
                    // Recalculate fBayWidth
                    fBayWidth = MLength / (MFrames - 1);
                }
                if (!IsSetFromCode) _baysWidthOptionsVM = new BayWidthOptionsViewModel(Frames - 1, fBayWidth);

                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;
                if (!IsSetFromCode) SetCustomModel();
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

                if (MModelIndex != 0)
                {
                    // Recalculate roof heigth
                    if (MKitsetTypeIndex == 0)
                        fHeight_H2 = MWallHeight + MWidth * (float)Math.Tan(fRoofPitch_radians);
                    else if (MKitsetTypeIndex == 1)
                        fHeight_H2 = MWallHeight + 0.5f * MWidth * (float)Math.Tan(fRoofPitch_radians);
                    else
                        fHeight_H2 = 0; // Exception
                }
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                RecreateFloorSlab = true;
                RecreateFoundations = true;
                if (!IsSetFromCode) SetCustomModel();
                NotifyPropertyChanged("WallHeight");
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
                    fRoofPitch_radians = MRoofPitch_deg * MathF.fPI / 180f;

                    // Recalculate h2
                    if (MKitsetTypeIndex == 0)
                        fHeight_H2 = MWallHeight + MWidth * (float)Math.Tan(fRoofPitch_radians);
                    else if (MKitsetTypeIndex == 1)
                        fHeight_H2 = MWallHeight + 0.5f * MWidth * (float)Math.Tan(fRoofPitch_radians);
                    else
                        fHeight_H2 = 0; // Exception
                }
                SetResultsAreNotValid();
                RecreateJoints = true;
                RecreateModel = true;
                if (!IsSetFromCode) SetCustomModel();
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
                    fBayWidth = MLength / (MFrames - 1);
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
                    fRafterLength = MathF.Sqrt(MathF.Pow2(fHeight_H2 - MWallHeight) + MathF.Pow2(MWidth));
                else if (MKitsetTypeIndex == 1)
                    fRafterLength = MathF.Sqrt(MathF.Pow2(fHeight_H2 - MWallHeight) + MathF.Pow2(0.5f * MWidth));
                else
                    fRafterLength = 0; // Exception

                float fFirstPurlinPosition = MPurlinDistance;
                OneRafterPurlinNo = (int)((fRafterLength - fFirstPurlinPosition) / MPurlinDistance) + 1;
                                
                _crossBracingOptionsVM = new CrossBracingOptionsViewModel(Frames - 1, OneRafterPurlinNo);
                _baysWidthOptionsVM = new BayWidthOptionsViewModel(Frames - 1, fBayWidth);
                
                if (!IsSetFromCode) SetCustomModel();  //TODO Mato - toto si mozes zavesit vsade kde to treba, ku kazdej prperty a zmene na nej
                NotifyPropertyChanged("Frames");
            }
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

                MGirtDistance = (float)Math.Round(value, 3); //Display only limited number of decimal places - Todo - Ondrej Review
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

                MPurlinDistance = (float)Math.Round(value, 3); //Display only limited number of decimal places - Todo - Ondrej Review
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
                        iOneRafterFrontColumnNo = (int)((MWidth - 0.95 * MColumnDistance) / MColumnDistance);
                        IFrontColumnNoInOneFrame = iOneRafterFrontColumnNo;
                    }
                    else if (MKitsetTypeIndex == 1)
                    {
                        iOneRafterFrontColumnNo = (int)((0.5f * MWidth - 0.45f * MColumnDistance) / MColumnDistance);
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
                float frontFrameRakeAngle_limit_rad = (float)(Math.Atan(fBayWidth / MWidth) - (Math.PI / 180)); // minus 1 radian
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
                float backFrameRakeAngle_limit_rad = (float)(Math.Atan(fBayWidth / MWidth) - (Math.PI / 180)); // minus 1 radian
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
        public int RoofCladdingIndex
        {
            get
            {
                return MRoofCladdingIndex;
            }

            set
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                MRoofCladdingIndex = value;

                bool isChangedFromCode = IsSetFromCode;

                if (!isChangedFromCode) IsSetFromCode = true;
                MRoofCladdingID = MRoofCladdingIndex;
                RoofCladding = Claddings.ElementAtOrDefault(MRoofCladdingIndex);
                RoofCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(RoofCladdingCoatingID) && p.claddingIDs.Contains(RoofCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                RoofCladdingThicknessIndex = 0;
                RoofCladdingThickness = RoofCladdingsThicknessTypes.ElementAtOrDefault(RoofCladdingThicknessIndex);

                RoofFibreglassThicknessTypes = CDatabaseManager.GetStringList("FibreglassSQLiteDB", RoofCladding, "name");
                RoofFibreglassThicknessIndex = 0;

                SetResultsAreNotValid();
                if (!isChangedFromCode) IsSetFromCode = false;

                RecreateQuotation = false;//musi byt zmena nasilu
                RecreateModel = true;
                NotifyPropertyChanged("RoofCladdingIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int RoofCladdingCoatingIndex
        {
            get
            {
                return MRoofCladdingCoatingIndex;
            }

            set
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                MRoofCladdingCoatingIndex = value;

                bool isChangedFromCode = IsSetFromCode;

                if (!isChangedFromCode) IsSetFromCode = true;
                RoofCladdingCoatingID = MRoofCladdingCoatingIndex + 1;
                RoofCladdingCoating = Coatings.ElementAtOrDefault(MRoofCladdingCoatingIndex);
                RoofCladdingColors = CCoatingColorManager.LoadCoatingColours(RoofCladdingCoatingID);
                RoofCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(RoofCladdingCoatingID) && p.claddingIDs.Contains(RoofCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                RoofCladdingColorIndex = 0;
                RoofCladdingThicknessIndex = 0;
                RoofCladdingThickness = RoofCladdingsThicknessTypes.ElementAtOrDefault(RoofCladdingThicknessIndex);

                if (!isChangedFromCode) IsSetFromCode = false;
                RecreateQuotation = false;//musi byt zmena nasilu
                RecreateQuotation = true;
                NotifyPropertyChanged("RoofCladdingCoatingIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int RoofCladdingColorIndex
        {
            get
            {
                return MRoofCladdingColorIndex;
            }

            set
            {
                MRoofCladdingColorIndex = value;
                RecreateQuotation = true;
                NotifyPropertyChanged("RoofCladdingColorIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int RoofCladdingThicknessIndex
        {
            get
            {
                return MRoofCladdingThicknessIndex;
            }

            set
            {
                MRoofCladdingThicknessIndex = value;

                RoofCladdingThickness = RoofCladdingsThicknessTypes.ElementAtOrDefault(MRoofCladdingThicknessIndex);
                //SetResultsAreNotValid();
                RecreateQuotation = true;
                NotifyPropertyChanged("RoofCladdingThicknessIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallCladdingIndex
        {
            get
            {
                return MWallCladdingIndex;
            }

            set
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                MWallCladdingIndex = value;

                bool isChangedFromCode = IsSetFromCode;

                if (!isChangedFromCode) IsSetFromCode = true;
                WallCladdingID = MWallCladdingIndex;
                WallCladding = Claddings.ElementAtOrDefault(MWallCladdingIndex);
                WallCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(WallCladdingCoatingID) && p.claddingIDs.Contains(WallCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                WallCladdingThicknessIndex = 0;
                WallCladdingThickness = WallCladdingsThicknessTypes.ElementAtOrDefault(WallCladdingThicknessIndex);

                WallFibreglassThicknessTypes = CDatabaseManager.GetStringList("FibreglassSQLiteDB", WallCladding, "name");
                WallFibreglassThicknessIndex = 0;

                if (!isChangedFromCode) IsSetFromCode = false;
                RecreateQuotation = false;//musi byt zmena nasilu
                RecreateQuotation = true;
                NotifyPropertyChanged("WallCladdingIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallCladdingCoatingIndex
        {
            get
            {
                return MWallCladdingCoatingIndex;
            }

            set
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                MWallCladdingCoatingIndex = value;

                bool isChangedFromCode = IsSetFromCode;
                if (!isChangedFromCode) IsSetFromCode = true;
                WallCladdingCoatingID = MWallCladdingCoatingIndex + 1;
                WallCladdingCoating = Coatings.ElementAtOrDefault(MWallCladdingCoatingIndex);
                WallCladdingColors = CCoatingColorManager.LoadCoatingColours(WallCladdingCoatingID);
                WallCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(WallCladdingCoatingID) && p.claddingIDs.Contains(WallCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                WallCladdingThicknessIndex = 0;
                WallCladdingThickness = WallCladdingsThicknessTypes.ElementAtOrDefault(WallCladdingThicknessIndex);
                WallCladdingColorIndex = 0;

                if (!isChangedFromCode) IsSetFromCode = false;
                RecreateQuotation = false;//musi byt zmena nasilu
                RecreateQuotation = true;
                NotifyPropertyChanged("WallCladdingCoatingIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallCladdingColorIndex
        {
            get
            {
                return MWallCladdingColorIndex;
            }

            set
            {
                MWallCladdingColorIndex = value;
                RecreateQuotation = true;
                NotifyPropertyChanged("WallCladdingColorIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallCladdingThicknessIndex
        {
            get
            {
                return MWallCladdingThicknessIndex;
            }

            set
            {
                MWallCladdingThicknessIndex = value;


                WallCladdingThickness = WallCladdingsThicknessTypes.ElementAtOrDefault(MWallCladdingThicknessIndex);
                //SetResultsAreNotValid();
                RecreateQuotation = true;
                NotifyPropertyChanged("WallCladdingThicknessIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int RoofFibreglassThicknessIndex
        {
            get
            {
                return MRoofFibreglassThicknessIndex;
            }

            set
            {
                MRoofFibreglassThicknessIndex = value;
                //SetResultsAreNotValid();
                RecreateQuotation = true;
                NotifyPropertyChanged("RoofFibreglassThicknessIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public int WallFibreglassThicknessIndex
        {
            get
            {
                return MWallFibreglassThicknessIndex;
            }

            set
            {
                MWallFibreglassThicknessIndex = value;
                //SetResultsAreNotValid();
                RecreateQuotation = true;
                NotifyPropertyChanged("WallFibreglassThicknessIndex");
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
        public float FibreglassAreaRoof
        {
            get
            {
                return MFibreglassAreaRoof;
            }

            set
            {
                if (value < 0.0 || value > 99.0) // Limit is 99% of area
                    throw new ArgumentException("Fibreglass area must be between 0.0 and 99 [%]");
                MFibreglassAreaRoof = value;
                //SetResultsAreNotValid();
                //RecreateJoints = true;
                RecreateModel = true;
                NotifyPropertyChanged("FibreglassAreaRoof");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float FibreglassAreaWall
        {
            get
            {
                return MFibreglassAreaWall;
            }

            set
            {
                if (value < 0.0 || value > 99.0) // Limit is 99% of area
                    throw new ArgumentException("Fibreglass area must be between 0.0 and 99 [%]");
                MFibreglassAreaWall = value;
                //SetResultsAreNotValid();
                //RecreateJoints = true;
                RecreateModel = true;
                NotifyPropertyChanged("FibreglassAreaWall");
            }
        }

        //premiestnene do DisplayOptions Window
        //-------------------------------------------------------------------------------------------------------------
        //public int WireframeColorIndex
        //{
        //    get
        //    {
        //        return MWireframeColorIndex;
        //    }

        //    set
        //    {
        //        MWireframeColorIndex = value;

        //        List<CComboColor> listOfMediaColours = CComboBoxHelper.ColorList;

        //        WireframeColor = listOfMediaColours[MWireframeColorIndex].Color;

        //        RecreateModel = true;
        //        NotifyPropertyChanged("WireframeColorIndex");
        //    }
        //}

        //-------------------------------------------------------------------------------------------------------------
        //public int BackgroundColorIndex
        //{
        //    get
        //    {
        //        return MBackgroundColorIndex;
        //    }

        //    set
        //    {
        //        MBackgroundColorIndex = value;

        //        List<CComboColor> listOfMediaColours = CComboBoxHelper.ColorList;

        //        BackgroundColor = listOfMediaColours[MBackgroundColorIndex].Color.Value;

        //        RecreateModel = true;
        //        NotifyPropertyChanged("BackgroundColorIndex");
        //    }
        //}

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
                SetModelBays();
                if (!isChangedFromCode) IsSetFromCode = false;
            }
        }

        //public bool DeterminateCombinationResultsByFEMSolver
        //{
        //    get
        //    {
        //        return MDeterminateCombinationResultsByFEMSolver;
        //    }

        //    set
        //    {
        //        MDeterminateCombinationResultsByFEMSolver = value;
        //        SetResultsAreNotValid();
        //        RecreateModel = false;
        //        NotifyPropertyChanged("DeterminateCombinationResultsByFEMSolver");
        //    }
        //}

        //public bool UseFEMSolverCalculationForSimpleBeam
        //{
        //    get
        //    {
        //        return MUseFEMSolverCalculationForSimpleBeam;
        //    }

        //    set
        //    {
        //        MUseFEMSolverCalculationForSimpleBeam = value;
        //        SetResultsAreNotValid();
        //        RecreateModel = false;
        //        NotifyPropertyChanged("UseFEMSolverCalculationForSimpleBeam");
        //    }
        //}

        //public bool DeterminateMemberLocalDisplacementsForULS
        //{
        //    get
        //    {
        //        return MDeterminateMemberLocalDisplacementsForULS;
        //    }

        //    set
        //    {
        //        MDeterminateMemberLocalDisplacementsForULS = value;
        //        SetResultsAreNotValid();
        //        RecreateModel = false;
        //        NotifyPropertyChanged("DeterminateMemberLocalDisplacementsForULS");
        //    }
        //}

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

        public ObservableCollection<DoorProperties> DoorBlocksProperties
        {
            get
            {
                if (MDoorBlocksProperties == null) MDoorBlocksProperties = new ObservableCollection<DoorProperties>();
                return MDoorBlocksProperties;
            }

            set
            {
                MDoorBlocksProperties = value;
                if (MDoorBlocksProperties == null) return;
                MDoorBlocksProperties.CollectionChanged += DoorBlocksProperties_CollectionChanged;
                foreach (DoorProperties d in MDoorBlocksProperties)
                {
                    d.PropertyChanged -= HandleDoorPropertiesPropertyChangedEvent;
                    d.PropertyChanged += HandleDoorPropertiesPropertyChangedEvent;
                }
                RecreateModel = true;
                RecreateJoints = true;
                RecreateFloorSlab = true;
                NotifyPropertyChanged("DoorBlocksProperties");
            }
        }

        private void DoorBlocksProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            //{
            //    DoorProperties d = MDoorBlocksProperties.LastOrDefault();
            //    if (d != null)
            //    {
            //        CDoorsAndWindowsHelper.SetDefaultDoorParams(d);
            //        d.PropertyChanged += HandleDoorPropertiesPropertyChangedEvent;
            //        NotifyPropertyChanged("DoorBlocksProperties_Add");
            //        SetResultsAreNotValid();
            //    }
            //}
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                RecreateModel = true;
                RecreateJoints = true;
                RecreateFloorSlab = true;
                NotifyPropertyChanged("DoorBlocksProperties_CollectionChanged");
                SetResultsAreNotValid();
            }
            SetComponentListAccordingToDoors();
        }

        public ObservableCollection<WindowProperties> WindowBlocksProperties
        {
            get
            {
                if (MWindowBlocksProperties == null) MWindowBlocksProperties = new ObservableCollection<WindowProperties>();
                return MWindowBlocksProperties;
            }

            set
            {
                MWindowBlocksProperties = value;
                if (MWindowBlocksProperties == null) return;
                MWindowBlocksProperties.CollectionChanged += WindowBlocksProperties_CollectionChanged;
                foreach (WindowProperties w in MWindowBlocksProperties)
                {
                    w.PropertyChanged -= HandleWindowPropertiesPropertyChangedEvent;
                    w.PropertyChanged += HandleWindowPropertiesPropertyChangedEvent;
                }
                RecreateModel = true;
                RecreateJoints = true;
                NotifyPropertyChanged("WindowBlocksProperties");
            }
        }

        private void WindowBlocksProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            //{
            //    WindowProperties w = MWindowBlocksProperties.LastOrDefault();
            //    if (w != null)
            //    {
            //        CDoorsAndWindowsHelper.SetDefaultWindowParams(w);
            //        w.PropertyChanged += HandleWindowPropertiesPropertyChangedEvent;
            //        NotifyPropertyChanged("WindowBlocksProperties_Add");
            //        SetResultsAreNotValid();
            //    }
            //}
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                RecreateModel = true;
                RecreateJoints = true;
                NotifyPropertyChanged("WindowBlocksProperties_CollectionChanged");
                SetResultsAreNotValid();
            }
            SetComponentListAccordingToWindows();
        }

        public List<string> BuildingSides
        {
            get
            {
                if (MBuildingSides == null) MBuildingSides = new List<string>() { "Left", "Right", "Front", "Back" };
                return MBuildingSides;
            }
        }

        public List<string> DoorsTypes
        {
            get
            {
                if (MDoorsTypes == null) MDoorsTypes = new List<string>() { "Personnel Door", "Roller Door" };
                return MDoorsTypes;
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

        public List<int> WindowColumns
        {
            get
            {
                return new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9, 10 };
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

        //public bool BracingEverySecondRowOfGirts
        //{
        //    get
        //    {
        //        return m_BracingEverySecondRowOfGirts;
        //    }

        //    set
        //    {
        //        m_BracingEverySecondRowOfGirts = value;
        //        RecreateJoints = true;
        //        if (MSynchronizeGUI) NotifyPropertyChanged("BracingEverySecondRowOfGirts");
        //    }
        //}

        //public bool BracingEverySecondRowOfPurlins
        //{
        //    get
        //    {
        //        return m_BracingEverySecondRowOfPurlins;
        //    }

        //    set
        //    {
        //        m_BracingEverySecondRowOfPurlins = value;
        //        RecreateJoints = true;
        //        if (MSynchronizeGUI) NotifyPropertyChanged("BracingEverySecondRowOfPurlins");
        //    }
        //}

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

        public List<CoatingColour> RoofCladdingColors
        {
            get
            {
                if (MRoofCladdingColors == null) MRoofCladdingColors = CCoatingColorManager.LoadCoatingColours(RoofCladdingCoatingIndex + 1);

                return MRoofCladdingColors;
            }

            set
            {
                MRoofCladdingColors = value;
                NotifyPropertyChanged("RoofCladdingColors");
            }
        }

        public List<CoatingColour> WallCladdingColors
        {
            get
            {
                if (MWallCladdingColors == null) MWallCladdingColors = CCoatingColorManager.LoadCoatingColours(WallCladdingCoatingIndex + 1);
                return MWallCladdingColors;
            }

            set
            {
                MWallCladdingColors = value;
                NotifyPropertyChanged("WallCladdingColors");
            }
        }

        public int RoofCladdingID
        {
            get
            {
                return MRoofCladdingID;
            }

            set
            {
                MRoofCladdingID = value;
            }
        }

        public int RoofCladdingCoatingID
        {
            get
            {
                return MRoofCladdingCoatingID;
            }

            set
            {
                MRoofCladdingCoatingID = value;
            }
        }

        public int WallCladdingID
        {
            get
            {
                return MWallCladdingID;
            }

            set
            {
                MWallCladdingID = value;
            }
        }

        public int WallCladdingCoatingID
        {
            get
            {
                return MWallCladdingCoatingID;
            }

            set
            {
                MWallCladdingCoatingID = value;
            }
        }

        public List<string> Claddings
        {
            get
            {
                if (m_Claddings == null) m_Claddings = CDatabaseManager.GetStringList("TrapezoidalSheetingSQLiteDB", "trapezoidalSheeting_m", "name");
                return m_Claddings;
            }

            set
            {
                m_Claddings = value;
            }
        }

        public List<string> Coatings
        {
            get
            {
                if (m_Coatings == null) m_Coatings = CDatabaseManager.GetStringList("TrapezoidalSheetingSQLiteDB", "coating", "name_short");
                return m_Coatings;
            }

            set
            {
                m_Coatings = value;
            }
        }

        public string RoofCladding
        {
            get
            {
                return m_RoofCladding;
            }

            set
            {
                m_RoofCladding = value;
            }
        }

        public string WallCladding
        {
            get
            {
                return m_WallCladding;
            }

            set
            {
                m_WallCladding = value;
            }
        }

        public string RoofCladdingCoating
        {
            get
            {
                return m_RoofCladdingCoating;
            }

            set
            {
                m_RoofCladdingCoating = value;
            }
        }

        public string WallCladdingCoating
        {
            get
            {
                return m_WallCladdingCoating;
            }

            set
            {
                m_WallCladdingCoating = value;
            }
        }

        public List<string> RoofCladdingsThicknessTypes
        {
            get
            {
                if (m_RoofCladdingsThicknessTypes == null)
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";
                    m_RoofCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(RoofCladdingCoatingID) && p.claddingIDs.Contains(RoofCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                }
                return m_RoofCladdingsThicknessTypes;
            }

            set
            {
                m_RoofCladdingsThicknessTypes = value;
                NotifyPropertyChanged("RoofCladdingsThicknessTypes");
            }
        }

        public List<string> WallCladdingsThicknessTypes
        {
            get
            {
                if (m_WallCladdingsThicknessTypes == null)
                {
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";
                    m_WallCladdingsThicknessTypes = ThicknessPropertiesList.Where(p => p.coatingIDs.Contains(WallCladdingCoatingID) && p.claddingIDs.Contains(WallCladdingID)).Select(p => (p.thicknessCore * 100).ToString("F2", nfi) + " mm").ToList();
                }
                return m_WallCladdingsThicknessTypes;
            }

            set
            {
                m_WallCladdingsThicknessTypes = value;
                NotifyPropertyChanged("WallCladdingsThicknessTypes");
            }
        }

        private List<CTS_ThicknessProperties> m_ThicknessPropertiesList;
        public List<CTS_ThicknessProperties> ThicknessPropertiesList
        {
            get
            {
                if (m_ThicknessPropertiesList == null) m_ThicknessPropertiesList = CTrapezoidalSheetingManager.LoadThicknessPropertiesList();
                return m_ThicknessPropertiesList;
            }

            set
            {
                m_ThicknessPropertiesList = value;
            }
        }

        public string RoofCladdingThickness
        {
            get
            {
                return m_RoofCladdingThickness;
            }

            set
            {
                m_RoofCladdingThickness = value;
            }
        }

        public string WallCladdingThickness
        {
            get
            {
                return m_WallCladdingThickness;
            }

            set
            {
                m_WallCladdingThickness = value;
            }
        }

        public List<string> RoofFibreglassThicknessTypes
        {
            get
            {
                return m_RoofFibreglassThicknessTypes;
            }

            set
            {
                m_RoofFibreglassThicknessTypes = value;
                NotifyPropertyChanged("RoofFibreglassThicknessTypes");
            }
        }

        public List<string> WallFibreglassThicknessTypes
        {
            get
            {
                return m_WallFibreglassThicknessTypes;
            }

            set
            {
                m_WallFibreglassThicknessTypes = value;
                NotifyPropertyChanged("WallFibreglassThicknessTypes");
            }
        }

        private ObservableCollection<int> frontBays;
        private ObservableCollection<int> backBays;
        private ObservableCollection<int> leftRightBays;

        private void SetModelBays()
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

            frontBays = new ObservableCollection<int>();
            backBays = new ObservableCollection<int>();
            leftRightBays = new ObservableCollection<int>();

            int iFrameNo = model != null ? model.iFrameNo : 4;
            int i = 0;
            while (i < iFrameNo - 1)
            {
                leftRightBays.Add((++i));
            }
            i = 0;
            while (i < IFrontColumnNoInOneFrame + 1)
            {
                frontBays.Add((++i));
            }
            i = 0;
            while (i < IFrontColumnNoInOneFrame + 1)
            {
                backBays.Add((++i));
            }

            SetDoorsBays();
            SetWindowsBays();
            SetDoorsWindowsValidationProperties();
        }

        public void SetModelBays(int iFrameNo)
        {
            frontBays = new ObservableCollection<int>();
            backBays = new ObservableCollection<int>();
            leftRightBays = new ObservableCollection<int>();

            int i = 0;
            while (i < iFrameNo - 1)
            {
                leftRightBays.Add((++i));
            }
            i = 0;
            while (i < IFrontColumnNoInOneFrame + 1)
            {
                frontBays.Add((++i));
            }
            i = 0;
            while (i < IFrontColumnNoInOneFrame + 1)
            {
                backBays.Add((++i));
            }

            SetDoorsBays(false);
            SetWindowsBays(false);
        }

        private void SetDoorsBays(bool check = true)
        {
            foreach (DoorProperties d in MDoorBlocksProperties)
            {
                if (d.sBuildingSide == "Front" && !d.Bays.SequenceEqual(frontBays)) d.Bays = frontBays;
                else if (d.sBuildingSide == "Back" && !d.Bays.SequenceEqual(backBays)) d.Bays = backBays;
                else if (d.sBuildingSide == "Left" && !d.Bays.SequenceEqual(leftRightBays)) d.Bays = leftRightBays;
                else if (d.sBuildingSide == "Right" && !d.Bays.SequenceEqual(leftRightBays)) d.Bays = leftRightBays;
            }
            if (check) CheckDoorsBays();
        }

        private bool SetDoorsBays(DoorProperties d)
        {
            if (d.sBuildingSide == "Front" && !d.Bays.SequenceEqual(frontBays)) d.Bays = frontBays;
            else if (d.sBuildingSide == "Back" && !d.Bays.SequenceEqual(backBays)) d.Bays = backBays;
            else if (d.sBuildingSide == "Left" && !d.Bays.SequenceEqual(leftRightBays)) d.Bays = leftRightBays;
            else if (d.sBuildingSide == "Right" && !d.Bays.SequenceEqual(leftRightBays)) d.Bays = leftRightBays;

            if (!CheckDoorsBays(d))
            {
                this.IsSetFromCode = true;
                d.sBuildingSide = d.sBuildingSide_old;
                this.IsSetFromCode = false;
                return false;
            }
            return true;
        }

        private void CheckDoorsBays()
        {
            foreach (DoorProperties d in MDoorBlocksProperties)
            {
                if (d.iBayNumber > d.Bays.Count) d.iBayNumber = 1;
                if (MDoorBlocksProperties.Where(x => x.iBayNumber == d.iBayNumber && x.sBuildingSide == d.sBuildingSide).Count() > 1)
                {
                    //d.iBayNumber++; //tu by sa dala napisat funkcia na najdenie volneho bay na umiesnenie dveri
                    int bayNum = GetFreeBayFor(d);
                    if (bayNum == -1) PFDMainWindow.ShowMessageBoxInPFDWindow($"Not possible to find free bay on this side. [{d.sBuildingSide}]");
                    else d.iBayNumber = bayNum;
                }
            }
        }

        private bool CheckDoorsBays(DoorProperties d)
        {
            bool isValid = true;
            if (d.iBayNumber > d.Bays.Count) d.iBayNumber = 1;

            if (MDoorBlocksProperties.Where(x => x.iBayNumber == d.iBayNumber && x.sBuildingSide == d.sBuildingSide).Count() > 1)
            {
                PFDMainWindow.ShowMessageBoxInPFDWindow("This bay is already occupied with a door.");
                isValid = false;
                //throw new Exception($"This bay is already occupied with a door.");
            }
            if (MWindowBlocksProperties.Where(x => x.iBayNumber == d.iBayNumber && x.sBuildingSide == d.sBuildingSide).Count() == 1)
            {
                PFDMainWindow.ShowMessageBoxInPFDWindow("This bay is already occupied with a window.");
                isValid = false;
                //throw new Exception($"This bay is already occupied with a window.");
            }
            return isValid;
        }

        //private void CheckDoorsBays(DoorProperties d)
        //{
        //    if (d.iBayNumber > d.Bays.Count) d.iBayNumber = 1;
        //    if (MDoorBlocksProperties.Where(x => x.iBayNumber == d.iBayNumber && x.sBuildingSide == d.sBuildingSide).Count() > 1)
        //    {
        //        //d.iBayNumber++; //tu by sa dala napisat funkcia na najdenie volneho bay na umiesnenie dveri
        //        int bayNum = GetFreeBayFor(d);
        //        if (bayNum == -1)
        //        {
        //            throw new Exception($"Not possible to find free bay on this side. [{d.sBuildingSide}]");
        //            //PFDMainWindow.ShowMessageBoxInPFDWindow($"Not possible to find free bay on this side. [{d.sBuildingSide}]");
        //        }
        //        else
        //        {
        //            d.IsSetFromCode = true;
        //            d.iBayNumber = bayNum;
        //            d.IsSetFromCode = false;
        //        }
        //    }
        //}

        private void SetWindowsBays(bool check = true)
        {
            foreach (WindowProperties w in MWindowBlocksProperties)
            {
                if (w.sBuildingSide == "Front" && !w.Bays.SequenceEqual(frontBays)) w.Bays = frontBays;
                else if (w.sBuildingSide == "Back" && !w.Bays.SequenceEqual(backBays)) w.Bays = backBays;
                else if (w.sBuildingSide == "Left" && !w.Bays.SequenceEqual(leftRightBays)) w.Bays = leftRightBays;
                else if (w.sBuildingSide == "Right" && !w.Bays.SequenceEqual(leftRightBays)) w.Bays = leftRightBays;
            }
            if (check)
            {
                CheckWindowsBays();
            }
        }

        private bool SetWindowsBays(WindowProperties w)
        {
            if (w.sBuildingSide == "Front" && !w.Bays.SequenceEqual(frontBays)) w.Bays = frontBays;
            else if (w.sBuildingSide == "Back" && !w.Bays.SequenceEqual(backBays)) w.Bays = backBays;
            else if (w.sBuildingSide == "Left" && !w.Bays.SequenceEqual(leftRightBays)) w.Bays = leftRightBays;
            else if (w.sBuildingSide == "Right" && !w.Bays.SequenceEqual(leftRightBays)) w.Bays = leftRightBays;

            if (!CheckWindowsBays(w))
            {
                this.IsSetFromCode = true;
                w.sBuildingSide = w.sBuildingSide_old;
                this.IsSetFromCode = false;
                return false;
            }
            return true;
        }

        private void CheckWindowsBays()
        {
            foreach (WindowProperties w in MWindowBlocksProperties)
            {
                if (w.iBayNumber > w.Bays.Count) w.iBayNumber = 1;
                if (MWindowBlocksProperties.Where(x => x.iBayNumber == w.iBayNumber && x.sBuildingSide == w.sBuildingSide).Count() > 1)
                {
                    //w.iBayNumber++; //tu by sa dala napisat funkcia na najdenie volneho bay na umiesnenie okna
                    int bayNum = GetFreeBayFor(w);
                    if (bayNum == -1) PFDMainWindow.ShowMessageBoxInPFDWindow($"Not possible to find free bay on this side. [{w.sBuildingSide}]");
                    else
                    {
                        w.IsSetFromCode = true;
                        w.iBayNumber = bayNum;
                        w.IsSetFromCode = false;
                    }
                }
            }
        }

        private bool CheckWindowsBays(WindowProperties w)
        {
            bool isValid = true;
            if (w.iBayNumber > w.Bays.Count) w.iBayNumber = 1;
            if (MWindowBlocksProperties.Where(x => x.iBayNumber == w.iBayNumber && x.sBuildingSide == w.sBuildingSide).Count() > 1)
            {
                PFDMainWindow.ShowMessageBoxInPFDWindow("The position is already occupied with a window.");
                isValid = false;
            }
            if (MDoorBlocksProperties.Where(x => x.iBayNumber == w.iBayNumber && x.sBuildingSide == w.sBuildingSide).Count() == 1)
            {
                PFDMainWindow.ShowMessageBoxInPFDWindow("The position is already occupied with a door.");
                isValid = false;
            }
            return isValid;
        }

        //private void CheckWindowsBays(WindowProperties w)
        //{
        //    if (w.iBayNumber > w.Bays.Count) w.iBayNumber = 1;
        //    if (MWindowBlocksProperties.Where(x => x.iBayNumber == w.iBayNumber && x.sBuildingSide == w.sBuildingSide).Count() > 1)
        //    {
        //        //w.iBayNumber++; //tu by sa dala napisat funkcia na najdenie volneho bay na umiesnenie okna
        //        int bayNum = GetFreeBayFor(w);
        //        if (bayNum == -1) PFDMainWindow.ShowMessageBoxInPFDWindow($"Not possible to find free bay on this side. [{w.sBuildingSide}]");
        //        else w.iBayNumber = bayNum;
        //    }
        //}

        private int GetFreeBayFor(WindowProperties win)
        {
            foreach (int bayNum in win.Bays)
            {
                if (MWindowBlocksProperties.Where(x => x.iBayNumber == bayNum && x.sBuildingSide == win.sBuildingSide).Count() == 0) return bayNum;
            }
            return -1;
        }

        private int GetFreeBayFor(DoorProperties d)
        {
            foreach (int bayNum in d.Bays)
            {
                if (MDoorBlocksProperties.Where(x => x.iBayNumber == bayNum && x.sBuildingSide == d.sBuildingSide).Count() == 0) return bayNum;
            }
            return -1;
        }

        private void SetDoorsWindowsValidationProperties()
        {
            SetDoorsValidationProperties();
            SetWindowsValidationProperties();
        }

        private void SetDoorsValidationProperties()
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

            foreach (DoorProperties d in MDoorBlocksProperties)
            {
                //task 600
                //d.SetValidationValues(MWallHeight, model.fL1_frame, model.fDist_FrontColumns, model.fDist_BackColumns);
                d.SetValidationValues(MWallHeight, model.GetBayWidth(d.iBayNumber), model.fDist_FrontColumns, model.fDist_BackColumns);
            }
        }

        private void SetWindowsValidationProperties()
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

            foreach (WindowProperties w in MWindowBlocksProperties)
            {
                //task 600
                //w.SetValidationValues(MWallHeight, model.fL1_frame, model.fDist_FrontColumns, model.fDist_BackColumns);
                w.SetValidationValues(MWallHeight, model.GetBayWidth(w.iBayNumber), model.fDist_FrontColumns, model.fDist_BackColumns);
            }
        }

        [NonSerialized]
        private CComponentListVM _componentVM;
        [NonSerialized]
        private CProjectInfoVM _projectInfoVM;
        [NonSerialized]
        private CJointsVM _jointsVM;
        [NonSerialized]
        private CFootingInputVM _footingVM;

        public DisplayOptionsViewModel _displayOptionsVM;
        public GeneralOptionsViewModel _generalOptionsVM;
        public SolverOptionsViewModel _solverOptionsVM;
        public DesignOptionsViewModel _designOptionsVM;

        public CrossBracingOptionsViewModel _crossBracingOptionsVM;
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

        private ObservableCollection<CAccessories_LengthItemProperties> m_Flashings;
        private List<string> m_FlashingsNames;
        public ObservableCollection<CAccessories_LengthItemProperties> Flashings
        {
            get
            {
                if (m_Flashings == null)
                {
                    SetDefaultFlashings();
                }
                return m_Flashings;
            }

            set
            {
                if (value == null) return;
                m_Flashings = value;
                m_Flashings.CollectionChanged += Flashings_CollectionChanged;
                foreach (CAccessories_LengthItemProperties item in Flashings)
                {
                    item.PropertyChanged += FlashingsItem_PropertyChanged;
                }

                NotifyPropertyChanged("Flashings");
            }
        }

        private void Flashings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                RecreateQuotation = true;
            }
        }

        public void FlashingsItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (!ValidateFlashings())
                {
                    PFDMainWindow.ShowMessageBoxInPFDWindow("ERROR.\nDuplicated definition of flashing type.\nChoose a unique type, please.");
                    CAccessories_LengthItemProperties item = sender as CAccessories_LengthItemProperties;
                    if (item != null) item.Name = item.NameOld;
                    PFDMainWindow.Datagrid_Flashings.ItemsSource = null;
                    PFDMainWindow.Datagrid_Flashings.ItemsSource = Flashings;
                }
            }
            if (e.PropertyName == "Thickness") return;
            if (e.PropertyName == "Width_total") return;
            PropertyChanged(sender, e);
        }

        public void SetDefaultFlashings()
        {
            float fRoofSideLength = 0;

            if (Model is CModel_PFD_01_MR)
            {
                fRoofSideLength = MathF.Sqrt(MathF.Pow2(Model.fH2_frame - Model.fH1_frame) + MathF.Pow2(Model.fW_frame)); // Dlzka hrany strechy
            }
            else if (Model is CModel_PFD_01_GR)
            {
                fRoofSideLength = MathF.Sqrt(MathF.Pow2(Model.fH2_frame - Model.fH1_frame) + MathF.Pow2(0.5f * Model.fW_frame)); // Dlzka hrany strechy
            }
            else
            {
                // Exception - not implemented
                fRoofSideLength = 0;
            }

            float fRoofRidgeFlashing_TotalLength = 0;
            float fWallCornerFlashing_TotalLength = 0;
            float fBargeFlashing_TotalLength = 0;

            if (Model is CModel_PFD_01_MR)
            {
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 2 * Model.fH1_frame + 2 * Model.fH2_frame;
                fBargeFlashing_TotalLength = 2 * fRoofSideLength;
            }
            else if (Model is CModel_PFD_01_GR)
            {
                fRoofRidgeFlashing_TotalLength = Model.fL_tot;
                fWallCornerFlashing_TotalLength = 4 * Model.fH1_frame;
                fBargeFlashing_TotalLength = 4 * fRoofSideLength;
            }
            else
            {
                // Exception - not implemented
                fRoofRidgeFlashing_TotalLength = 0;
                fWallCornerFlashing_TotalLength = 0;
                fBargeFlashing_TotalLength = 0;
            }

            float fRollerDoorTrimmerFlashing_TotalLength = 0;
            float fRollerDoorLintelFlashing_TotalLength = 0;
            float fRollerDoorLintelCapFlashing_TotalLength = 0;
            float fPADoorTrimmerFlashing_TotalLength = 0;
            float fPADoorLintelFlashing_TotalLength = 0;
            float fWindowFlashing_TotalLength = 0;

            ObservableCollection<CAccessories_LengthItemProperties> flashings = new ObservableCollection<CAccessories_LengthItemProperties>();

            if (KitsetTypeIndex != 0)
            {
                flashings.Add(new CAccessories_LengthItemProperties("Roof Ridge", "Flashings", fRoofRidgeFlashing_TotalLength, 2));
            }

            flashings.Add(new CAccessories_LengthItemProperties("Wall Corner", "Flashings", fWallCornerFlashing_TotalLength, 2));
            flashings.Add(new CAccessories_LengthItemProperties("Barge", "Flashings", fBargeFlashing_TotalLength, 2));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Trimmer", "Flashings", fRollerDoorTrimmerFlashing_TotalLength, 4));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Header", "Flashings", fRollerDoorLintelFlashing_TotalLength, 4));
            flashings.Add(new CAccessories_LengthItemProperties("Roller Door Header Cap", "Flashings", fRollerDoorLintelCapFlashing_TotalLength, 4));
            flashings.Add(new CAccessories_LengthItemProperties("PA Door Trimmer", "Flashings", fPADoorTrimmerFlashing_TotalLength, 18));
            flashings.Add(new CAccessories_LengthItemProperties("PA Door Header", "Flashings", fPADoorLintelFlashing_TotalLength, 18));
            flashings.Add(new CAccessories_LengthItemProperties("Window", "Flashings", fWindowFlashing_TotalLength, 9));
            Flashings = flashings;

            SetFlashingsNames();
        }

        public bool ValidateFlashings()
        {
            foreach (CAccessories_LengthItemProperties item in Flashings)
            {
                int count = Flashings.Where(f => f.Name == item.Name).Count();
                if (count > 1) return false;

                if (item.Name == "Roof Ridge")
                {
                    if (Flashings.FirstOrDefault(f => f.Name == "Roof Ridge (Soft Edge)") != null) return false;
                }

                if (item.Name == "Roof Ridge (Soft Edge)")
                {
                    if (Flashings.FirstOrDefault(f => f.Name == "Roof Ridge") != null) return false;
                }
            }
            return true;
        }

        public void AccessoriesItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Thickness") return;
            if (e.PropertyName == "Width_total") return;
            PropertyChanged(sender, e);
        }


        private ObservableCollection<CAccessories_LengthItemProperties> m_Gutters;
        private List<string> m_GuttersNames;
        public ObservableCollection<CAccessories_LengthItemProperties> Gutters
        {
            get
            {
                if (m_Gutters == null)
                {
                    float fGuttersTotalLength = 0; // na dvoch okrajoch strechy

                    if (MModel is CModel_PFD_01_MR)
                    {
                        fGuttersTotalLength = Model.fL_tot; // na jednom okraji strechy
                    }
                    else if (MModel is CModel_PFD_01_GR)
                    {
                        fGuttersTotalLength = 2 * Model.fL_tot; // na dvoch okrajoch strechy
                    }
                    else
                    {
                        // Exception - not implemented
                        fGuttersTotalLength = 0;
                    }

                    CAccessories_LengthItemProperties gutter = new CAccessories_LengthItemProperties("Roof Gutter 430", "Gutters", fGuttersTotalLength, 2);
                    gutter.PropertyChanged += AccessoriesItem_PropertyChanged;
                    Gutters = new ObservableCollection<CAccessories_LengthItemProperties> { gutter };

                    NotifyPropertyChanged("Gutters");
                }
                return m_Gutters;
            }

            set
            {
                if (value == null) return;
                m_Gutters = value;
                m_Gutters.CollectionChanged += Gutters_CollectionChanged;
            }
        }

        private void Gutters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                RecreateQuotation = true;
            }
        }

        private ObservableCollection<CAccessories_DownpipeProperties> m_Downpipes;
        public ObservableCollection<CAccessories_DownpipeProperties> Downpipes
        {
            get
            {
                if (m_Downpipes == null)
                {
                    SetDefaultDownpipes();
                }
                return m_Downpipes;
            }

            set
            {
                if (value == null) return;
                m_Downpipes = value;
                m_Downpipes.CollectionChanged += Downpipes_CollectionChanged;

                NotifyPropertyChanged("Downpipes");
            }
        }

        public void SetDefaultDownpipes()
        {
            // Zatial bude natvrdo jeden riadok s poctom zvodov, prednastavenou dlzkou ako vyskou steny a farbou, rovnaky default ako gutter
            int iCountOfDownpipePoints = 0;
            float fDownpipesTotalLength = 0;

            if (MModel is CModel_PFD_01_MR)
            {
                iCountOfDownpipePoints = 2; // TODO - prevziat z GUI - 2 rohy budovy kde je nizsia vyska steny (H1 alebo H2)
                fDownpipesTotalLength = iCountOfDownpipePoints * Math.Min(MModel.fH1_frame, MModel.fH2_frame); // Pocet zvodov krat vyska steny
            }
            else if (MModel is CModel_PFD_01_GR)
            {
                iCountOfDownpipePoints = 4; // TODO - prevziat z GUI - 4 rohy strechy
                fDownpipesTotalLength = iCountOfDownpipePoints * MModel.fH1_frame; // Pocet zvodov krat vyska steny
            }
            else
            {
                // Exception - not implemented
                iCountOfDownpipePoints = 0;
                fDownpipesTotalLength = 0;
            }

            CAccessories_DownpipeProperties downpipe = new CAccessories_DownpipeProperties("RP80®", iCountOfDownpipePoints, fDownpipesTotalLength, 2);

            downpipe.PropertyChanged += AccessoriesItem_PropertyChanged;
            Downpipes = new ObservableCollection<CAccessories_DownpipeProperties>() { downpipe };
        }

        private void Downpipes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                RecreateQuotation = true;
            }
        }

        public List<string> FlashingsNames
        {
            get
            {
                if (m_FlashingsNames == null) SetFlashingsNames();
                return m_FlashingsNames;
            }
            set
            {
                m_FlashingsNames = value;
                NotifyPropertyChanged("FlashingsNames");
            }
        }
        public List<string> AllFlashingsNames
        {
            get
            {
                return new List<string>() { "Roof Ridge", "Roof Ridge (Soft Edge)", "Wall Corner", "Barge", "Roller Door Trimmer", "Roller Door Header", "Roller Door Header Cap",
                        "PA Door Trimmer",  "PA Door Header", "Window"};
            }
        }

        private void SetFlashingsNames()
        {
            if (KitsetTypeIndex == 0)
            {
                FlashingsNames = new List<string>() { "Wall Corner", "Barge", "Roller Door Trimmer", "Roller Door Header", "Roller Door Header Cap",
                        "PA Door Trimmer",  "PA Door Header", "Window"};
            }
            else
            {
                FlashingsNames = new List<string>() { "Roof Ridge", "Roof Ridge (Soft Edge)", "Wall Corner", "Barge", "Roller Door Trimmer", "Roller Door Header", "Roller Door Header Cap",
                        "PA Door Trimmer",  "PA Door Header", "Window"};
            }
        }

        public List<string> GuttersNames
        {
            get
            {
                if (m_GuttersNames == null) m_GuttersNames = new List<string>() { "Roof Gutter 430", "Roof Gutter 520", "Roof Gutter 550"/*, "Internal Gutter"*/ };
                return m_GuttersNames;
            }
        }

        public bool GeneralOptionsChanged
        {
            get
            {
                return m_GeneralOptionsChanged;
            }

            set
            {
                m_GeneralOptionsChanged = value;

                SetResultsAreNotValid();
                RecreateModel = true;

                if (MSynchronizeGUI) NotifyPropertyChanged("GeneralOptionsChanged");

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

                if (MSynchronizeGUI) NotifyPropertyChanged("CrossBracingOptionsChanged");

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
                IsSetFromCode = false;

                SetResultsAreNotValid();
                RecreateModel = true;

                if (MSynchronizeGUI) NotifyPropertyChanged("BaysWidthOptionsChanged");

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



        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDViewModel(int kitsetTypeIndex, int modelIndex, bool bRelease, ObservableCollection<DoorProperties> doorBlocksProperties, ObservableCollection<WindowProperties> windowBlocksProperties,
            CComponentListVM componentVM, CPFDLoadInput loadInput, CProjectInfoVM projectInfoVM)
        {
            MIsRelease = bRelease;            

            IsSetFromCode = true;
            DoorBlocksProperties = doorBlocksProperties;
            WindowBlocksProperties = windowBlocksProperties;

            _componentVM = componentVM;
            SetComponentListAccordingToDoorsAndWindows();
            _componentVM.PropertyChanged += ComponentVM_PropertyChanged;
            ComponentList = _componentVM.ComponentList;

            _loadInput = loadInput;
            _loadInput.PropertyChanged += _loadInput_PropertyChanged;

            _projectInfoVM = projectInfoVM;

            _displayOptionsVM = new DisplayOptionsViewModel(bRelease);
            _generalOptionsVM = new GeneralOptionsViewModel();
            _solverOptionsVM = new SolverOptionsViewModel();
            _designOptionsVM = new DesignOptionsViewModel();

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

            QuotationDisplayOptions _quotationDisplayOptions = new QuotationDisplayOptions();
            //_quotationExportOptions = new QuotationExportOptions(_quotationDisplayOptions);
            _quotationDisplayOptionsVM = new QuotationDisplayOptionsViewModel(_quotationDisplayOptions);

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

            if (e.PropertyName == "AllMaterialListChanged")
            {
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

            if (_solverOptionsVM.DeterminateCombinationResultsByFEMSolver || _solverOptionsVM.UseFEMSolverCalculationForSimpleBeam)
            {
                SolverWindow.SetBeams();
                // Calculation of simple beam model
                beamSimpleModels = model.GetMembersFromModel(); // Create models of particular beams
                if (debugging) System.Diagnostics.Trace.WriteLine("After beamSimpleModels = model.GetMembersFromModel(); " + (DateTime.Now - start).TotalMilliseconds);
                CBeamsCalculations.RunBeamsCalculations(beamSimpleModels, !_solverOptionsVM.DeterminateCombinationResultsByFEMSolver, SolverWindow);
                if (debugging) System.Diagnostics.Trace.WriteLine("After beamSimpleModels: " + (DateTime.Now - start).TotalMilliseconds);
            }

            CalculationSettingsFoundation footingSettings = FootingVM.GetCalcSettings();
            CMemberDesignCalculations memberDesignCalculations = new CMemberDesignCalculations(SolverWindow, model, UseCRSCGeometricalAxes, _designOptionsVM.ShearDesignAccording334, _designOptionsVM.IgnoreWebStiffeners, _designOptionsVM.UniformShearDistributionInAnchors,
                _solverOptionsVM.DeterminateCombinationResultsByFEMSolver, _solverOptionsVM.UseFEMSolverCalculationForSimpleBeam, _solverOptionsVM.DeterminateMemberLocalDisplacementsForULS,
                footingSettings, frameModels, beamSimpleModels);
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

            sDesignResults_ULSandSLS = mdc.sDesignResults_ULSandSLS;
            sDesignResults_ULS = mdc.sDesignResults_ULS;
            sDesignResults_SLS = mdc.sDesignResults_SLS;
        }

        public void SetComponentListAccordingToDoorsAndWindows()
        {
            SetComponentListAccordingToDoors();
            SetComponentListAccordingToWindows();
        }

        private void SetComponentListAccordingToDoors()
        {
            if (ModelHasPersonelDoor()) _componentVM.AddPersonelDoor();
            else _componentVM.RemovePersonelDoor();

            if (ModelHasRollerDoor()) _componentVM.AddRollerDoor();
            else _componentVM.RemoveRollerDoor();

        }

        private void SetComponentListAccordingToWindows()
        {
            if (ModelHasWindow()) _componentVM.AddWindow();
            else _componentVM.RemoveWindow();
        }

        private bool ModelHasPersonelDoor()
        {
            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sDoorType == "Personnel Door") return true;
            }
            return false;
        }

        private bool ModelHasRollerDoor()
        {
            foreach (DoorProperties d in DoorBlocksProperties)
            {
                if (d.sDoorType == "Roller Door") return true;
            }
            return false;
        }

        private bool ModelHasWindow()
        {
            if (WindowBlocksProperties == null) return false;

            return WindowBlocksProperties.Count > 0;
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

        private void HandleDoorPropertiesPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (IsSetFromCode) return;

                if (e.PropertyName == "sBuildingSide")
                {
                    SetResultsAreNotValid();
                    if (sender is DoorProperties) { if (!SetDoorsBays(sender as DoorProperties)) return; }
                    if (sender is WindowProperties) { if (!SetWindowsBays(sender as WindowProperties)) return; }
                }
                else if (e.PropertyName == "iBayNumber")
                {
                    SetResultsAreNotValid();
                    if (sender is DoorProperties)
                    {
                        DoorProperties d = sender as DoorProperties;
                        if (!CheckDoorsBays(d)) { IsSetFromCode = true; d.iBayNumber = d.iBayNumber_old; IsSetFromCode = false; return; }
                    }
                    if (sender is WindowProperties)
                    {
                        WindowProperties w = sender as WindowProperties;
                        if (!CheckWindowsBays(w)) { IsSetFromCode = true; w.iBayNumber = w.iBayNumber_old; IsSetFromCode = false; return; }
                    }
                }
                else if (e.PropertyName == "sDoorType")
                {
                    SetResultsAreNotValid();
                    SetComponentListAccordingToDoors();
                }
                else if (e.PropertyName == "CoatingColor" || e.PropertyName == "Series" || e.PropertyName == "Series")
                {
                    return;
                }

                if (e.PropertyName == "fDoorsHeight" || e.PropertyName == "fDoorsWidth" ||
                    e.PropertyName == "fDoorCoordinateXinBlock")
                {
                    SetResultsAreNotValid();
                }
                RecreateFloorSlab = true;
                this.PropertyChanged(sender, e);
            }
            catch (Exception ex)
            {
                //task 551
                //toto este prerobit tak,ze zdetekuje koliziu dveri a okna
                PFDMainWindow.ShowMessageBoxInPFDWindow(ex.Message);
                //bug 436
                //tu by som chcel reagovat na to,ze neexistuje volna bay, zistit koliziu = ze su rovnake objekty a jeden surovo zmazat
                var duplicates = DoorBlocksProperties.GroupBy(d => new { d.iBayNumber, d.sBuildingSide }).Where(g => g.Count() > 1).Select(g => g.FirstOrDefault());
                if (duplicates.Count() > 0)
                {
                    var doorProps = DoorBlocksProperties.GroupBy(d => new { d.iBayNumber, d.sBuildingSide }).Where(g => g.Count() == 1).Select(g => g.FirstOrDefault()).ToList();
                    doorProps.AddRange(duplicates);
                    DoorBlocksProperties = new ObservableCollection<DoorProperties>(doorProps);
                }
            }
        }

        private void HandleWindowPropertiesPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (IsSetFromCode) return;

                if (e.PropertyName == "sBuildingSide")
                {
                    SetResultsAreNotValid();
                    if (sender is DoorProperties) { if (!SetDoorsBays(sender as DoorProperties)) return; }
                    if (sender is WindowProperties) { if (!SetWindowsBays(sender as WindowProperties)) return; }
                }
                else if (e.PropertyName == "iBayNumber")
                {
                    SetResultsAreNotValid();
                    if (sender is DoorProperties)
                    {
                        DoorProperties d = sender as DoorProperties;
                        if (!CheckDoorsBays(d)) { IsSetFromCode = true; d.iBayNumber = d.iBayNumber_old; IsSetFromCode = false; return; }
                    }
                    if (sender is WindowProperties)
                    {
                        WindowProperties w = sender as WindowProperties;
                        if (!CheckWindowsBays(w)) { IsSetFromCode = true; w.iBayNumber = w.iBayNumber_old; IsSetFromCode = false; return; }
                    }
                }
                else if (e.PropertyName == "fWindowsHeight" || e.PropertyName == "fWindowsWidth" || e.PropertyName == "fWindowCoordinateXinBay" || e.PropertyName == "fWindowCoordinateZinBay")
                {
                    SetResultsAreNotValid();
                }
                this.PropertyChanged(sender, e);
            }
            catch (Exception ex)
            {
                //task 551
                //toto este prerobit tak,ze zdetekuje koliziu dveri a okna
                PFDMainWindow.ShowMessageBoxInPFDWindow(ex.Message);
                //bug 436
                //tu by som chcel reagovat na to,ze neexistuje volna bay, zistit koliziu = ze su rovnake objekty a jeden surovo zmazat
                var duplicates = WindowBlocksProperties.GroupBy(d => new { d.iBayNumber, d.sBuildingSide }).Where(g => g.Count() > 1).Select(g => g.FirstOrDefault());
                if (duplicates.Count() > 0)
                {
                    var windowsProps = WindowBlocksProperties.GroupBy(d => new { d.iBayNumber, d.sBuildingSide }).Where(g => g.Count() == 1).Select(g => g.FirstOrDefault()).ToList();
                    windowsProps.AddRange(duplicates);
                    WindowBlocksProperties = new ObservableCollection<WindowProperties>(windowsProps);
                }
            }


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
            data.RoofCladdingIndex = MRoofCladdingIndex;
            data.RoofCladdingCoatingIndex = MRoofCladdingCoatingIndex;
            data.RoofCladdingColorIndex = MRoofCladdingColorIndex;
            data.RoofCladdingThicknessIndex = MRoofCladdingThicknessIndex;
            data.WallCladdingIndex = MWallCladdingIndex;
            data.WallCladdingCoatingIndex = MWallCladdingCoatingIndex;
            data.WallCladdingColorIndex = MWallCladdingColorIndex;
            data.WallCladdingThicknessIndex = MWallCladdingThicknessIndex;

            data.RoofFibreglassThicknessIndex = MRoofFibreglassThicknessIndex;
            data.WallFibreglassThicknessIndex = MWallFibreglassThicknessIndex;

            data.SupportTypeIndex = MSupportTypeIndex;
            data.FibreglassAreaRoof = MFibreglassAreaRoof;
            data.FibreglassAreaWall = MFibreglassAreaWall;
            data.LoadCaseIndex = MLoadCaseIndex;
            data.IFrontColumnNoInOneFrame = IFrontColumnNoInOneFrame;
            data.UseCRSCGeometricalAxes = UseCRSCGeometricalAxes;

            data.GeneralLoad = GeneralLoad;
            data.Wind = Wind;
            data.Snow = Snow;
            data.Eq = Eq;

            data.DoorBlocksProperties = DoorBlocksProperties;
            data.WindowBlocksProperties = WindowBlocksProperties;

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
            data.Width = MWidth;
            data.Length = MLength;
            data.WallHeight = MWallHeight;
            data.RoofPitch_deg = MRoofPitch_deg;
            data.Frames = MFrames;
            data.GirtDistance = MGirtDistance;
            data.PurlinDistance = MPurlinDistance;
            data.ColumnDistance = MColumnDistance;
            data.BottomGirtPosition = MBottomGirtPosition;

            data.BayWidth = fBayWidth;
            data.ApexHeight_H2 = fHeight_H2;

            data.RoofCladding = RoofCladding;
            data.WallCladding = WallCladding;
            data.RoofCladdingThickness_mm = RoofCladdingThickness;
            data.WallCladdingThickness_mm = WallCladdingThickness;
            data.RoofCladdingCoating = RoofCladdingCoating;
            data.WallCladdingCoating = WallCladdingCoating;
            data.RoofFibreglassThickness_mm = RoofFibreglassThicknessTypes.ElementAtOrDefault(MRoofFibreglassThicknessIndex);
            data.WallFibreglassThickness_mm = WallFibreglassThicknessTypes.ElementAtOrDefault(MWallFibreglassThicknessIndex);

            data.Location = _loadInput.ListLocations[_loadInput.LocationIndex];
            data.WindRegion = _loadInput.ListWindRegion[_loadInput.WindRegionIndex];
            data.NumberOfRollerDoors = MDoorBlocksProperties.Where(d => d.sDoorType == "Roller Door").Count();
            data.NumberOfPersonnelDoors = MDoorBlocksProperties.Where(d => d.sDoorType == "Personnel Door").Count();

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

            return data;
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

                CJointLoadCombinationRatio_ULS resStart = JointDesignResults_ULS.FirstOrDefault(i => i.Member.ID == governingMember.ID && i.LoadCombination.ID == governingLoadComb.ID && i.Joint.m_Node.ID == cjStart.m_Node.ID);
                CJointLoadCombinationRatio_ULS resEnd = JointDesignResults_ULS.FirstOrDefault(i => i.Member.ID == governingMember.ID && i.LoadCombination.ID == governingLoadComb.ID && i.Joint.m_Node.ID == cjEnd.m_Node.ID);
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
            sDisplayOptions.bDisplayDimensions = _displayOptionsVM.ShowDimensions;
            sDisplayOptions.bDisplayGridlines = _displayOptionsVM.ShowGridLines;
            sDisplayOptions.bDisplaySectionSymbols = _displayOptionsVM.ShowSectionSymbols;
            sDisplayOptions.bDisplayDetailSymbols = _displayOptionsVM.ShowDetailSymbols;
            sDisplayOptions.bDisplaySlabRebates = _displayOptionsVM.ShowSlabRebates;

            sDisplayOptions.bDisplayMembersCenterLines = _displayOptionsVM.DisplayMembersCenterLines;
            sDisplayOptions.bDisplaySolidModel = _displayOptionsVM.DisplaySolidModel;
            sDisplayOptions.bDisplayWireFrameModel = _displayOptionsVM.DisplayWireFrameModel;

            sDisplayOptions.bDistinguishedColor = _displayOptionsVM.DisplayDistinguishedColorMember;
            sDisplayOptions.bTransparentMemberModel = _displayOptionsVM.DisplayTransparentModelMember;

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
            sDisplayOptions.bColorsAccordingToMembers = _displayOptionsVM.ColorsAccordingToMembers;
            sDisplayOptions.bColorsAccordingToSections = _displayOptionsVM.ColorsAccordingToSections;

            //sDisplayOptions.wireFrameColor = WireframeColor;
            sDisplayOptions.wireFrameColor = _displayOptionsVM.WireframeColor;
            sDisplayOptions.fWireFrameLineThickness = _displayOptionsVM.WireFrameLineThickness;

            sDisplayOptions.memberCenterlineColor = _displayOptionsVM.MemberCenterlineColor;
            sDisplayOptions.fmemberCenterlineThickness = _displayOptionsVM.MemberCenterlineThickness;

            sDisplayOptions.fNodeDescriptionTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            sDisplayOptions.fMemberDescriptionTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            sDisplayOptions.fDimensionTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            sDisplayOptions.fGridLineLabelTextFontSize = 30; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            sDisplayOptions.fSectionSymbolLabelTextFontSize = 30; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            sDisplayOptions.fDetailSymbolLabelTextFontSize = 30; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report

            sDisplayOptions.fSawCutTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            sDisplayOptions.fControlJointTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report

            sDisplayOptions.fFoundationTextFontSize = 12; // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report
            sDisplayOptions.fFloorSlabTextFontSize = 12;  // TODO dopracovat nastavitelne v GUI - samostatne pre 3D scenu a report

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

            sDisplayOptions.bUseTextures = _displayOptionsVM.UseTextures;

            sDisplayOptions.fMemberSolidModelOpacity = _displayOptionsVM.MemberSolidModelOpacity;
            sDisplayOptions.fPlateSolidModelOpacity = _displayOptionsVM.PlateSolidModelOpacity;
            sDisplayOptions.fScrewSolidModelOpacity = _displayOptionsVM.ScrewSolidModelOpacity;
            sDisplayOptions.fAnchorSolidModelOpacity = _displayOptionsVM.AnchorSolidModelOpacity;
            sDisplayOptions.fFoundationSolidModelOpacity = _displayOptionsVM.FoundationSolidModelOpacity;
            sDisplayOptions.fReinforcementBarSolidModelOpacity = _displayOptionsVM.ReinforcementBarSolidModelOpacity;
            sDisplayOptions.fFloorSlabSolidModelOpacity = _displayOptionsVM.FloorSlabSolidModelOpacity;
            sDisplayOptions.fSlabRebateSolidModelOpacity = _displayOptionsVM.SlabRebateSolidModelOpacity;

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

            return sDisplayOptions;
        }

        public void GetCTS_CoilProperties(out CTS_CrscProperties prop_RoofCladding, out CTS_CrscProperties prop_WallCladding,
            out CTS_CoilProperties prop_RoofCladdingCoil, out CTS_CoilProperties prop_WallCladdingCoil,
            out CoatingColour prop_RoofCladdingColor, out CoatingColour prop_WallCladdingColor)
        {
            List<CTS_CoatingProperties> coatingsProperties = CTrapezoidalSheetingManager.LoadCoatingPropertiesList();

            prop_RoofCladding = CTrapezoidalSheetingManager.GetSectionProperties($"{RoofCladding}-{RoofCladdingThickness}");

            prop_WallCladding = CTrapezoidalSheetingManager.GetSectionProperties($"{WallCladding}-{WallCladdingThickness}");

            CTS_CoatingProperties prop_RoofCladdingCoating = new CTS_CoatingProperties();
            prop_RoofCladdingCoating = CTrapezoidalSheetingManager.LoadCoatingProperties(RoofCladdingCoating);

            CTS_CoatingProperties prop_WallCladdingCoating = new CTS_CoatingProperties();
            prop_WallCladdingCoating = CTrapezoidalSheetingManager.LoadCoatingProperties(WallCladdingCoating);

            prop_RoofCladdingColor = RoofCladdingColors.ElementAtOrDefault(RoofCladdingColorIndex); // TODO Ondrej - pre Formclad a vyber color Zinc potrebujem vratit spravnu farbu odpovedajuce ID = 18 v databaze
            prop_WallCladdingColor = WallCladdingColors.ElementAtOrDefault(WallCladdingColorIndex);

            prop_RoofCladdingCoil = CTrapezoidalSheetingManager.GetCladdingCoilProperties(coatingsProperties.ElementAtOrDefault(RoofCladdingCoatingIndex), prop_RoofCladdingColor, prop_RoofCladding); // Ceny urcujeme podla coating a color
            prop_WallCladdingCoil = CTrapezoidalSheetingManager.GetCladdingCoilProperties(coatingsProperties.ElementAtOrDefault(WallCladdingCoatingIndex), prop_WallCladdingColor, prop_WallCladding); // Ceny urcujeme podla coating a color
        }


    }
}
