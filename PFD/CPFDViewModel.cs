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

namespace PFD
{
    public class CPFDViewModel : INotifyPropertyChanged
    {
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        public MainWindow PFDMainWindow;
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsSetFromCode = false;

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

        // Load Case - display options
        private bool MShowLoads;
        private bool MShowNodalLoads;
        private bool MShowLoadsOnMembers;
        private bool MShowLoadsOnPurlinsAndGirts;
        private bool MShowLoadsOnFrameMembers;
        private bool MShowSurfaceLoads;

        private bool MShowLoadsLabels;
        private bool MShowLoadsLabelsUnits;



        //member description options
        private bool MShowMemberDescription;
        private bool MShowMemberID;
        private bool MShowMemberPrefix;
        private bool MShowMemberCrossSectionStartName;
        private bool MShowMemberRealLength;

        // Load Combination - options
        private bool MDeterminateCombinationResultsByFEMSolver;

        private CModel_PFD MModel;
        //-------------------------------------------------------------------------------------------------------------
        //tieto treba spracovat nejako
        public float fL1;
        public float fh2;
        public float fRoofPitch_radians;
        public float fMaterial_density = 7850f; // [kg /m^3] (malo by byt zadane v databaze materialov)

        public List<PropertiesToInsertOpening> DoorBlocksToInsertProperties;
        public List<PropertiesToInsertOpening> WindowBlocksToInsertProperties;
        public List<DoorProperties> DoorBlocksProperties;
        public List<WindowProperties> WindowBlocksProperties;
        public CCalcul_1170_1 GeneralLoad;
        public CCalcul_1170_2 Wind;
        public CCalcul_1170_3 Snow;
        public CCalcul_1170_5 Eq;
        public CPFDLoadInput Loadinput;

        public List<CMemberInternalForcesInLoadCases> MemberInternalForcesInLoadCases;
        public List<CMemberDeflectionsInLoadCases> MemberDeflectionsInLoadCases;

        public List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations;
        public List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations;

        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();
        public List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;

        public List<CFrame> frameModels;
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

                //dolezite je volat private fields a nie Properties pokial nechceme aby sa volali setter metody
                CDatabaseModels dmodel = new CDatabaseModels(MModelIndex);
                IsSetFromCode = true;
                GableWidth = dmodel.fb;
                Length = dmodel.fL;
                WallHeight = dmodel.fh;
                Frames = dmodel.iFrNo;
                RoofPitch_deg = dmodel.fRoof_Pitch_deg;
                GirtDistance = dmodel.fdist_girt;
                PurlinDistance = dmodel.fdist_purlin;
                ColumnDistance = dmodel.fdist_frontcolumn;
                BottomGirtPosition = dmodel.fdist_girt_bottom;
                FrontFrameRakeAngle = dmodel.fRakeAngleFrontFrame_deg;
                BackFrameRakeAngle = dmodel.fRakeAngleBackFrame_deg;

                IsSetFromCode = false;

                //tieto riadky by som tu najradsej nemal, resp. ich nejako spracoval ako dalsie property
                fL1 = MLength / (MFrames - 1);
                fRoofPitch_radians = MRoofPitch_deg * MathF.fPI / 180f;
                fh2 = MWallHeight + 0.5f * MGableWidth * (float)Math.Tan(fRoofPitch_radians);

                RoofCladdingIndex = 1;
                RoofCladdingColorIndex = 22;
                WallCladdingIndex = 0;
                WallCladdingColorIndex = 22;

                NotifyPropertyChanged("ModelIndex");
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
                if (value < 3 || value > 100)
                    throw new ArgumentException("Gable Width must be between 3 and 100 [m]");
                MGableWidth = value;

                if (MModelIndex != 0)
                {
                    // Recalculate roof pitch
                    fRoofPitch_radians = (float)Math.Atan((fh2 - MWallHeight) / (0.5f * MGableWidth));
                    // Set new value in GUI
                    MRoofPitch_deg = (fRoofPitch_radians * 180f / MathF.fPI);
                }

                NotifyPropertyChanged("GableWidth");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        public float Length
        {
            get
            {
                return MLength;
            }

            set
            {
                if (value < 3 || value > 150)
                    throw new ArgumentException("Length must be between 3 and 150 [m]");
                MLength = value;

                if (MModelIndex != 0)
                {
                    // Recalculate fL1
                    fL1 = MLength / (MFrames - 1);
                }

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
                    fh2 = MWallHeight + 0.5f * MGableWidth * (float)Math.Tan(fRoofPitch_radians);
                }

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

                if (value < 3 || value > 35)
                    throw new ArgumentException("Roof Pitch must be between 3 and 35 degrees");
                MRoofPitch_deg = value;

                if (MModelIndex != 0)
                {
                    fRoofPitch_radians = MRoofPitch_deg * MathF.fPI / 180f;
                    // Recalculate h2
                    fh2 = MWallHeight + 0.5f * MGableWidth * (float)Math.Tan(fRoofPitch_radians);
                }

                NotifyPropertyChanged("RoofPitch");
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
                if (value < 2 || value > 50)
                    throw new ArgumentException("Number of frames must be between 2 and 50");
                MFrames = value;

                if (MModelIndex != 0)
                {
                    // Recalculate L1
                    fL1 = MLength / (MFrames - 1);
                }

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
                if (value < 0.5 || value > 5)
                    throw new ArgumentException("Girt distance must be between 0.5 and 5 [m]");

                MGirtDistance = (float)Math.Round(value, 3); //Display only limited number of decimal places - Todo - Ondrej Review

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
                if (value < 0.5 || value > 5)
                    throw new ArgumentException("Purlin distance must be between 0.5 and 5 [m]");

                MPurlinDistance = (float)Math.Round(value, 3); //Display only limited number of decimal places - Todo - Ondrej Review

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
                if (value < 1 || value > 10)
                    throw new ArgumentException("Column distance must be between 1 and 10 [m]");
                MColumnDistance = value;

                if (MModelIndex != 0)
                {
                    // Re-calculate value of distance between columns (number of columns per frame is always even
                    int iOneRafterFrontColumnNo = (int)((0.5f * MGableWidth) / MColumnDistance);
                    int iFrontColumnNoInOneFrame = 2 * iOneRafterFrontColumnNo;
                    // Update value of distance between columns

                    // Todo
                    // Nie je to trosku na hlavu? Pouzivatel zada Column distance a ono sa mu to nejako prepocita a zmeni???
                    // Odpoved: Spravnejsie by bolo zadat pocet stlpov, ale je to urobene tak ze to musi byt parne cislo aby boli stlpiky symetricky voci stredu
                    // Pripadalo mi lepsie ak si uzivatel zada nejaku vzdialenost ktoru priblizne vie, pocet stlpikov by si musel spocitat 
                    // alebo tam musi byt dalsi textbox pre column distance kde sa ta dopocitana vzdialenost bude zobrazovat, ale je to dalsi readonly riadok na vstupe navyse
                    // chcel som tam mat toho co najmenej a len najnutnejsie hodnoty
                    // Mozes to tak upravit ak je to logickejsie a spravnejsie

                    MColumnDistance = (MGableWidth / (iFrontColumnNoInOneFrame + 1));
                }

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
                if (value < 0.2 || value > 0.8 * MWallHeight) // Limit is 80% of main column height, could be more but it is 
                    throw new ArgumentException("Bottom Girt Position between 0.2 and " + Math.Round(0.8 * MWallHeight, 3) + " [m]");
                MBottomGirtPosition = value;
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
                float frontFrameRakeAngle_limit_rad = (float)(Math.Atan(fL1 / MGableWidth) - (Math.PI / 180)); // minus 1 radian
                float frontFrameRakeAngle_limit_deg = frontFrameRakeAngle_limit_rad * 180f / MathF.fPI;

                if (value < -frontFrameRakeAngle_limit_deg || value > frontFrameRakeAngle_limit_deg)
                {
                    string s = "Front frame angle must be between " + (-Math.Round(frontFrameRakeAngle_limit_deg, 1)) + " and " + Math.Round(frontFrameRakeAngle_limit_deg, 1) + " degrees";
                    throw new ArgumentException(s);
                }
                MFrontFrameRakeAngle = value;

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
                float backFrameRakeAngle_limit_rad = (float)(Math.Atan(fL1 / MGableWidth) - (Math.PI / 180)); // minus 1 radian
                float backFrameRakeAngle_limit_deg = backFrameRakeAngle_limit_rad * 180f / MathF.fPI;

                if (value < -backFrameRakeAngle_limit_deg || value > backFrameRakeAngle_limit_deg)
                {
                    string s = "Back frame angle must be between " + (-Math.Round(backFrameRakeAngle_limit_deg, 1)) + " and " + Math.Round(backFrameRakeAngle_limit_deg, 1) + " degrees";
                    throw new ArgumentException(s);
                }
                MBackFrameRakeAngle = value;

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
                MRoofCladdingIndex = value;

                NotifyPropertyChanged("RoofCladdingIndex");
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
                MWallCladdingIndex = value;

                NotifyPropertyChanged("WallCladdingIndex");
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

                NotifyPropertyChanged("WallCladdingThicknessIndex");
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
            }
        }

        public bool ShowLoads
        {
            get
            {
                return MShowLoads;
            }

            set
            {
                MShowLoads = value;
                NotifyPropertyChanged("ShowLoads");
            }
        }

        public bool ShowNodalLoads
        {
            get
            {
                return MShowNodalLoads;
            }

            set
            {
                MShowNodalLoads = value;
                NotifyPropertyChanged("ShowNodalLoads");
            }
        }

        public bool ShowLoadsOnMembers
        {
            get
            {
                return MShowLoadsOnMembers;
            }

            set
            {
                MShowLoadsOnMembers = value;
                NotifyPropertyChanged("ShowLoadsOnMembers");
            }
        }

        public bool ShowLoadsOnPurlinsAndGirts
        {
            get
            {
                return MShowLoadsOnPurlinsAndGirts;
            }

            set
            {
                MShowLoadsOnPurlinsAndGirts = value;
                NotifyPropertyChanged("ShowLoadsOnPurlinsAndGirts");
            }
        }

        public bool ShowLoadsOnFrameMembers
        {
            get
            {
                return MShowLoadsOnFrameMembers;
            }

            set
            {
                MShowLoadsOnFrameMembers = value;
                NotifyPropertyChanged("ShowLoadsOnFrameMembers");
            }
        }

        public bool ShowSurfaceLoads
        {
            get
            {
                return MShowSurfaceLoads;
            }

            set
            {
                MShowSurfaceLoads = value;
                NotifyPropertyChanged("ShowSurfaceLoads");
            }
        }

        public bool DeterminateCombinationResultsByFEMSolver
        {
            get
            {
                return MDeterminateCombinationResultsByFEMSolver;
            }

            set
            {
                MDeterminateCombinationResultsByFEMSolver = value;
                NotifyPropertyChanged("DeterminateCombinationResultsByFEMSolver");
            }
        }

        public bool ShowMemberDescription
        {
            get
            {
                return MShowMemberDescription;
            }

            set
            {
                MShowMemberDescription = value;
                NotifyPropertyChanged("ShowMemberDescription");
            }
        }

        public bool ShowMemberID
        {
            get
            {
                return MShowMemberID;
            }

            set
            {
                MShowMemberID = value;
                NotifyPropertyChanged("ShowMemberID");
            }
        }

        public bool ShowMemberPrefix
        {
            get
            {
                return MShowMemberPrefix;
            }

            set
            {
                MShowMemberPrefix = value;
                NotifyPropertyChanged("ShowMemberPrefix");
            }
        }

        public bool ShowMemberCrossSectionStartName
        {
            get
            {
                return MShowMemberCrossSectionStartName;
            }

            set
            {
                MShowMemberCrossSectionStartName = value;
                NotifyPropertyChanged("ShowMemberCrossSectionStartName");
            }
        }

        public bool ShowMemberRealLength
        {
            get
            {
                return MShowMemberRealLength;
            }

            set
            {
                MShowMemberRealLength = value;
                NotifyPropertyChanged("ShowMemberRealLength");
            }
        }

        public bool ShowLoadsLabels
        {
            get
            {
                return MShowLoadsLabels;
            }

            set
            {
                MShowLoadsLabels = value;
                NotifyPropertyChanged("ShowLoadsLabels");
            }
        }

        public bool ShowLoadsLabelsUnits
        {
            get
            {
                return MShowLoadsLabelsUnits;
            }

            set
            {
                MShowLoadsLabelsUnits = value;
                NotifyPropertyChanged("ShowLoadsLabelsUnits");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CPFDViewModel(int modelIndex, List<PropertiesToInsertOpening> doorBlocksToInsertProperties, List<PropertiesToInsertOpening> windowBlocksToInsertProperties,
            List<DoorProperties> doorBlocksProperties, List<WindowProperties> windowBlocksProperties)
        {
            DoorBlocksProperties = doorBlocksProperties;
            WindowBlocksToInsertProperties = windowBlocksToInsertProperties;
            DoorBlocksProperties = doorBlocksProperties;
            WindowBlocksProperties = windowBlocksProperties;

            ShowMemberID = true;
            ShowMemberRealLength = true;

            ShowLoads = true;
            ShowLoadsOnMembers = true;
            ShowLoadsOnPurlinsAndGirts = true;
            ShowLoadsOnFrameMembers = true;
            ShowNodalLoads = true;
            ShowSurfaceLoads = false;
            ShowLoadsLabels = true;
            ShowLoadsLabelsUnits = true;
            

            //nastavi sa default model type a zaroven sa nastavia vsetky property ViewModelu (samozrejme sa updatuje aj View) 
            //vid setter metoda pre ModelIndex
            ModelIndex = modelIndex;

            IsSetFromCode = false;

            _worker.DoWork += CalculateInternalForces;
            _worker.WorkerSupportsCancellation = true;
        }

        public void CreateModel()
        {
            // Create 3D model of structure including loading
            MModel = new CModel_PFD_01_GR(
                    WallHeight,
                    GableWidth,
                    fL1, Frames,
                    fh2,
                    GirtDistance,
                    PurlinDistance,
                    ColumnDistance,
                    BottomGirtPosition,
                    FrontFrameRakeAngle,
                    BackFrameRakeAngle,
                    DoorBlocksToInsertProperties,
                    WindowBlocksToInsertProperties,
                    DoorBlocksProperties,
                    WindowBlocksProperties,
                    GeneralLoad,
                    Wind,
                    Snow,
                    Eq,
                    MShowNodalLoads,
                    MShowLoadsOnMembers,
                    MShowLoadsOnPurlinsAndGirts,
                    MShowLoadsOnFrameMembers,
                    MShowSurfaceLoads);
        }

        public void Run()
        {
            if (!_worker.IsBusy) _worker.RunWorkerAsync();
        }

        private void CalculateInternalForces(object sender, DoWorkEventArgs e)
        {
            Calculate();
        }

        private void Calculate()
        {
            DateTime start = DateTime.Now;

            const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
            const int iNumberOfSegments = iNumberOfDesignSections - 1;

            float[] fx_positions = new float[iNumberOfDesignSections];
            designBucklingLengthFactors sBucklingLengthFactors = new designBucklingLengthFactors();
            designMomentValuesForCb sMomentValuesforCb = new designMomentValuesForCb();
            basicInternalForces[] sBIF_x = null;
            basicDeflections[] sBDeflections_x = null;

            CModel_PFD_01_GR model = (CModel_PFD_01_GR)Model;

            // Validate model before calculation (compare IDs)
            CModelHelper.ValidateModel(model);

            // Tu by sa mal napojit 3D FEM vypocet v pripade ze budeme pocitat vsetko v 3D
            //RunFEMSOlver();

            ////////////////////////////////////////////////////////////////////////
            // Calculation of frame model

            if (!ShowLoadsOnMembers || !ShowLoadsOnFrameMembers) // Generate loads if they are not generated
            {
                CMemberLoadGenerator loadGenerator = new CMemberLoadGenerator(model, GeneralLoad, Snow, Wind);
                loadGenerator.GenerateLoadsOnFrames();
            }

            frameModels = model.GetFramesFromModel(); // Create models of particular frames

            foreach (CFrame frame in frameModels)
            {
                // Convert SW_EN model to BFENet model
                CModelToBFEMNetConverter converter = new CModelToBFEMNetConverter();
                // Convert model and calculate results
                Model bfemNetModel = converter.Convert(frame, !DeterminateCombinationResultsByFEMSolver);
                //PFDMainWindow.ShowBFEMNetModel(bfemNetModel); // Zobrazovat len na vyziadanie
            }

            // Calculation of simple beam model
            float fMaximumDesignRatioWholeStructure = 0;
            float fMaximumDesignRatioMainColumn = 0;
            float fMaximumDesignRatioMainRafter = 0;
            float fMaximumDesignRatioEndColumn = 0;
            float fMaximumDesignRatioEndRafter = 0;
            float fMaximumDesignRatioGirts = 0;
            float fMaximumDesignRatioPurlins = 0;
            float fMaximumDesignRatioColumns = 0;

            CMember MaximumDesignRatioWholeStructureMember = new CMember();
            CMember MaximumDesignRatioMainColumn = new CMember();
            CMember MaximumDesignRatioMainRafter = new CMember();
            CMember MaximumDesignRatioEndColumn = new CMember();
            CMember MaximumDesignRatioEndRafter = new CMember();
            CMember MaximumDesignRatioGirt = new CMember();
            CMember MaximumDesignRatioPurlin = new CMember();
            CMember MaximumDesignRatioColumn = new CMember();

            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();
            MemberInternalForcesInLoadCases = new List<CMemberInternalForcesInLoadCases>();
            MemberDeflectionsInLoadCases = new List<CMemberDeflectionsInLoadCases>();

            MemberInternalForcesInLoadCombinations = new List<CMemberInternalForcesInLoadCombinations>();
            MemberDeflectionsInLoadCombinations = new List<CMemberDeflectionsInLoadCombinations>();

            System.Diagnostics.Trace.WriteLine("before calculations: " + (DateTime.Now - start).TotalMilliseconds);

            double step = 100.0 / (Model.m_arrMembers.Length * 2.0);
            double progressValue = 0;
            PFDMainWindow.UpdateProgressBarValue(progressValue, "");

            // Calculate Internal Forces For Load Cases
            foreach (CMember m in Model.m_arrMembers)
            {
                if (m.BIsDSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
                {
                    if((!DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER)) ||
                    (m.EMemberType != EMemberType_FS.eMC && m.EMemberType != EMemberType_FS.eMR && m.EMemberType != EMemberType_FS.eEC && m.EMemberType != EMemberType_FS.eER))
                    {
                        for (int i = 0; i < iNumberOfDesignSections; i++)
                            fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                        m.MBucklingLengthFactors = new List<designBucklingLengthFactors>();
                        m.MMomentValuesforCb = new List<designMomentValuesForCb>();
                        m.MBIF_x = new List<basicInternalForces[]>();
                        m.MBDef_x = new List<basicDeflections[]>();

                        foreach (CLoadCase lc in Model.m_arrLoadCases)
                        {
                            // Frame member
                            if (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR ||
                                m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER)
                            {
                                // BEFENet - calculate load cases only

                                // Set indices to search in results
                                int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri
                                int iLoadCaseIndex = lc.ID - 1; // nastavit index podla ID load casu
                                int iMemberIndex = frameModels[iFrameIndex].GetMemberIndexInFrame(m); //podla ID pruta a indexu ramu treba identifikovat do ktoreho ramu prut z globalneho modelu patri a ktory prut v rame mu odpoveda

                                // Calculate Internal forces just for Load Cases that are included in ULS
                                if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eULSOnly)
                                {
                                    // Nastavit vysledky pre prut ramu

                                    sBucklingLengthFactors.fBeta_x_FB_fl_ex = 1.0f;

                                    sBucklingLengthFactors.fBeta_y_FB_fl_ey = 1.0f;
                                    sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 1.0f;
                                    sBucklingLengthFactors.fBeta_LTB_fl_LTB = 1.0f;

                                    if (m.EMemberType == EMemberType_FS.eMR)
                                    {
                                        sBucklingLengthFactors.fBeta_y_FB_fl_ey = 0.5f;
                                        sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 0.5f;
                                        sBucklingLengthFactors.fBeta_LTB_fl_LTB = 0.5f;
                                    }

                                    sMomentValuesforCb.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces[2].fM_yy;
                                    sMomentValuesforCb.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces[5].fM_yy;
                                    sMomentValuesforCb.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces[7].fM_yy;
                                    sMomentValuesforCb.fM_max = MathF.Max(sMomentValuesforCb.fM_14, sMomentValuesforCb.fM_24, sMomentValuesforCb.fM_34); // TODO - urcit z priebehu sil na danom prute

                                    sBIF_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();
                                }

                                if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eSLSOnly)
                                {
                                    //sBDeflections_x = (deflectionsframes[iFrameIndex][iLoadCaseIndex][iMemberIndex]).ToArray();
                                    sBDeflections_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
                                }

                                if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb));
                                if (sBDeflections_x != null) MemberDeflectionsInLoadCases.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));
                            }
                            else // Single member
                            {
                                // Calculate Internal forces just for Load Cases that are included in ULS
                                if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eULSOnly)
                                {
                                    foreach (CMLoad cmload in lc.MemberLoadsList)
                                    {
                                        if (cmload.Member.ID == m.ID) // TODO - Zatial pocitat len pre zatazenia, ktore lezia priamo skumanom na prute, po zavedeni 3D solveru upravit
                                        {
                                            // ULS - internal forces
                                            calcModel.CalculateInternalForcesOnSimpleBeam_PFD(iNumberOfDesignSections, fx_positions, m, (CMLoad_21)cmload, out sBIF_x, out sBucklingLengthFactors, out sMomentValuesforCb);
                                        }
                                    }
                                }

                                if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eSLSOnly)
                                {
                                    foreach (CMLoad cmload in lc.MemberLoadsList)
                                    {
                                        if (cmload.Member.ID == m.ID) // TODO - Zatial pocitat len pre zatazenia, ktore lezia priamo skumanom na prute, po zavedeni 3D solveru upravit
                                        {
                                            // SLS - deflections
                                            calcModel.CalculateDeflectionsOnSimpleBeam_PFD(iNumberOfDesignSections, fx_positions, m, (CMLoad_21)cmload, out sBDeflections_x);
                                        }
                                    }
                                }

                                if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb));
                                if (sBDeflections_x != null) MemberDeflectionsInLoadCases.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));

                                //m.MMomentValuesforCb.Add(sMomentValuesforCb);
                                //m.MBIF_x.Add(sBIF_x);
                            }
                        }
                    }
                }
                progressValue += step;
                PFDMainWindow.UpdateProgressBarValue(progressValue, "Calculating Internal Forces. MemberID: " + m.ID);
            }

            // Design of members
            // Calculate Internal Forces For Load Cases

            MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
            MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();

            JointDesignResults_ULS = new List<CJointLoadCombinationRatio_ULS>();

            foreach (CMember m in Model.m_arrMembers)
            {
                if (m.BIsDSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
                {
                    for (int i = 0; i < iNumberOfDesignSections; i++)
                        fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                    foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
                    {
                        if (lcomb.eLComType == ELSType.eLS_ULS) // Do not perform internal foces calculation for ULS
                        {
                            // Member basic internal forces
                            designBucklingLengthFactors sBucklingLengthFactors_design;
                            designMomentValuesForCb sMomentValuesforCb_design;
                            basicInternalForces[] sBIF_x_design;

                            // TODO 201 - Pripravit vysledky na jednotlivych prutoch povodneho 3D modelu pre pruty ramov aj ostatne pruty ktore su samostatne
                            // Todo je dost skareda vec, asi by sa Example3 v BFENet malo prerobit len na vypocet load cases tu s tym pracovat uz podobne pre pruty ramu a jednotlive samostatne pruty ako su purlins a girts
                            // Chcelo by to ten Example3 upravit a zobecnit tak aby sa z neho dali tahat rozne vysledky, podobne ako sa to da zo samotnej kniznice BFENet

                            // Frame member - vysledky pocitane pre load combinations
                            if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
                            {
                                // Nastavit vysledky pre prut ramu

                                sBucklingLengthFactors_design.fBeta_x_FB_fl_ex = 1.0f;

                                sBucklingLengthFactors_design.fBeta_y_FB_fl_ey = 1.0f;
                                sBucklingLengthFactors_design.fBeta_z_TB_TFB_l_ez = 1.0f;
                                sBucklingLengthFactors_design.fBeta_LTB_fl_LTB = 1.0f;

                                if (m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eER)
                                {
                                    sBucklingLengthFactors_design.fBeta_y_FB_fl_ey = 0.5f;
                                    sBucklingLengthFactors_design.fBeta_z_TB_TFB_l_ez = 0.5f;
                                    sBucklingLengthFactors_design.fBeta_LTB_fl_LTB = 0.5f;
                                }

                                int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri
                                int iMemberIndex = frameModels[iFrameIndex].GetMemberIndexInFrame(m); //podla ID pruta a indexu ramu treba identifikovat do ktoreho ramu prut z globalneho modelu patri a ktory prut v rame mu odpoveda

                                // TODO - hodnoty by sme mali ukladat presne vo stvrtinach, alebo umoznit ich dopocet - tj dostat sa k modelu BFENet a pouzit priamo funkciu
                                // pre nacianie vnutornych sil z objektu BFENet FrameElement2Node GetInternalForcesAt vid Example3 a funkcia GetResultsList

                                sMomentValuesforCb_design.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces[2].fM_yy;
                                sMomentValuesforCb_design.fM_24 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces[5].fM_yy;
                                sMomentValuesforCb_design.fM_34 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces[7].fM_yy;
                                sMomentValuesforCb_design.fM_max = MathF.Max(sMomentValuesforCb_design.fM_14, sMomentValuesforCb_design.fM_24, sMomentValuesforCb_design.fM_34); // TODO - urcit z priebehu sil na danom prute

                                sBIF_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();

                                // BFENet ma vracia vysledky pre ohybove momenty s opacnym znamienkom ako je nasa znamienkova dohoda
                                // Preto hodnoty momentov prenasobime
                                float fInternalForceSignFactor = -1; // TODO 191 - TO Ondrej Vnutorne sily z BFENet maju opacne znamienko, takze ich potrebujeme zmenit, alebo musime zaviest ine vykreslovanie pre momenty a ine pre sily

                                sMomentValuesforCb_design.fM_14 *= fInternalForceSignFactor;
                                sMomentValuesforCb_design.fM_24 *= fInternalForceSignFactor;
                                sMomentValuesforCb_design.fM_34 *= fInternalForceSignFactor;
                                sMomentValuesforCb_design.fM_max *= fInternalForceSignFactor;

                                for(int i = 0; i < sBIF_x_design.Length; i++)
                                {
                                    sBIF_x_design[i].fT *= fInternalForceSignFactor;
                                    sBIF_x_design[i].fM_yy *= fInternalForceSignFactor;
                                    sBIF_x_design[i].fM_yu *= fInternalForceSignFactor;
                                    sBIF_x_design[i].fM_zz *= fInternalForceSignFactor;
                                    sBIF_x_design[i].fM_zv *= fInternalForceSignFactor;
                                }
                            }
                            else // Single Member or Frame Member (only LC calculated) - vysledky pocitane pre load cases
                            {
                                CMemberResultsManager.SetMemberInternalForcesInLoadCombination(m, lcomb, MemberInternalForcesInLoadCases, iNumberOfDesignSections, out sBucklingLengthFactors_design, out sMomentValuesforCb_design, out sBIF_x_design);
                            }

                            // 22.2.2019 - Ulozime vnutorne sily v kombinacii - pre zobrazenie v Internal forces
                            if (sBIF_x_design != null) MemberInternalForcesInLoadCombinations.Add(new CMemberInternalForcesInLoadCombinations(m, lcomb, sBIF_x_design, sMomentValuesforCb_design));

                            // Member design internal forces
                            if (m.BIsDSelectedForDesign) // Only structural members (not auxiliary members or members with deactivated design)
                            {
                                designInternalForces[] sMemberDIF_x;

                                // Member Design
                                CMemberDesign memberDesignModel = new CMemberDesign();
                                memberDesignModel.SetDesignForcesAndMemberDesign_PFD(iNumberOfDesignSections, m, sBIF_x_design, sBucklingLengthFactors_design, sMomentValuesforCb_design, out sMemberDIF_x);
                                MemberDesignResults_ULS.Add(new CMemberLoadCombinationRatio_ULS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sMemberDIF_x[memberDesignModel.fMaximumDesignRatioLocationID], sBucklingLengthFactors, sMomentValuesforCb_design));

                                // Set maximum design ratio of whole structure
                                if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioWholeStructure)
                                {
                                    fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                                    MaximumDesignRatioWholeStructureMember = m;
                                }

                                // Joint Design
                                designInternalForces[] sJointDIF_x;
                                CJointDesign jointDesignModel = new CJointDesign();

                                CConnectionJointTypes jointStart;
                                CConnectionJointTypes jointEnd;
                                jointDesignModel.SetDesignForcesAndJointDesign_PFD(iNumberOfDesignSections, Model, m, sBIF_x_design, out jointStart, out jointEnd, out sJointDIF_x);

                                // Validation - Main member of joint must be defined
                                if (jointStart.m_MainMember == null)
                                    throw new ArgumentNullException("Error" + "Joint No: " + jointStart.ID + " Main member is not defined.");
                                if (jointEnd.m_MainMember == null)
                                    throw new ArgumentNullException("Error" + "Joint No: " + jointEnd.ID + " Main member is not defined.");

                                // Start Joint
                                JointDesignResults_ULS.Add(new CJointLoadCombinationRatio_ULS(m, jointStart, lcomb, jointDesignModel.fDesignRatio_Start, sJointDIF_x[jointDesignModel.fDesignRatioLocationID_Start]));

                                // End Joint
                                JointDesignResults_ULS.Add(new CJointLoadCombinationRatio_ULS(m, jointEnd, lcomb, jointDesignModel.fDesignRatio_End, sJointDIF_x[jointDesignModel.fDesignRatioLocationID_End]));

                                // Output (for debugging - member results)
                                bool bDebugging = false; // Testovacie ucely
                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString() + "\n");

                                // Output (for debugging - member connection / joint results)
                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Joint ID: " + jointStart.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(jointDesignModel.fDesignRatio_Start, 3).ToString() + "\n");

                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Joint ID: " + jointEnd.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(jointDesignModel.fDesignRatio_End, 3).ToString() + "\n");

                                // Output - set maximum design ratio by component Type
                                switch (m.EMemberType)
                                {
                                    case EMemberType_FS.eMC: // Main Column
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioMainColumn)
                                            {
                                                fMaximumDesignRatioMainColumn = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioMainColumn = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FS.eMR: // Main Rafter
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioMainRafter)
                                            {
                                                fMaximumDesignRatioMainRafter = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioMainRafter = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FS.eEC: // End Column
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioEndColumn)
                                            {
                                                fMaximumDesignRatioEndColumn = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioEndColumn = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FS.eER: // End Rafter
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioEndRafter)
                                            {
                                                fMaximumDesignRatioEndRafter = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioEndRafter = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FS.eG: // Girt
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioGirts)
                                            {
                                                fMaximumDesignRatioGirts = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioGirt = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FS.eP: // Purlin
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioPurlins)
                                            {
                                                fMaximumDesignRatioPurlins = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioPurlin = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FS.eC: // Column
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioColumns)
                                            {
                                                fMaximumDesignRatioColumns = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioColumn = m;
                                            }
                                            break;
                                        }
                                    default:
                                        // TODO - modifikovat podla potrieb pre ukladanie - doplnit vsetky typy
                                        break;
                                }
                            }
                        }
                        else // SLS
                        {
                            // Member basic deflections
                            basicDeflections[] sBDeflection_x_design;

                            // Member design deflections
                            if (m.BIsDSelectedForDesign) // Only structural members (not auxiliary members or members with deactivated design)
                            {
                                designDeflections[] sDDeflection_x;
                                CMemberDesign memberDesignModel = new CMemberDesign();

                                // TODO - Pripravit vysledky na jednotlivych prutoch povodneho 3D modelu pre pruty ramov aj ostatne pruty ktore su samostatne
                                // Frame member - vysledky pocitane pre load combinations
                                if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
                                {
                                    int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri
                                    int iLoadCombinationIndex = lcomb.ID - 1; // nastavit index podla ID combinacie
                                    int iMemberIndex = frameModels[iFrameIndex].GetMemberIndexInFrame(m); //podla ID pruta a indexu ramu treba identifikovat do ktoreho ramu prut z globalneho modelu patri a ktory prut v rame mu odpoveda

                                    //sBDeflection_x_design = (deflectionsframes[iFrameIndex][iLoadCombinationIndex][iMemberIndex]).ToArray();
                                    sBDeflection_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].Deflections.ToArray();

                                    memberDesignModel.SetDesignDeflections_PFD(iNumberOfDesignSections, m, sBDeflection_x_design, out sDDeflection_x);
                                    MemberDesignResults_SLS.Add(new CMemberLoadCombinationRatio_SLS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sDDeflection_x[memberDesignModel.fMaximumDesignRatioLocationID]));
                                }
                                else // Single Member or Frame Member (only LC calculated) - vysledky pocitane pre load cases
                                {
                                    CMemberResultsManager.SetMemberDeflectionsInLoadCombination(m, lcomb, MemberDeflectionsInLoadCases, iNumberOfDesignSections, out sBDeflection_x_design);
                                    memberDesignModel.SetDesignDeflections_PFD(iNumberOfDesignSections, m, sBDeflection_x_design, out sDDeflection_x);
                                    MemberDesignResults_SLS.Add(new CMemberLoadCombinationRatio_SLS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sDDeflection_x[memberDesignModel.fMaximumDesignRatioLocationID]));
                                }

                                // 22.2.2019 - Ulozime priehyby v kombinacii - pre zobrazenie v Internal forces
                                if (sBDeflection_x_design != null) MemberDeflectionsInLoadCombinations.Add(new CMemberDeflectionsInLoadCombinations(m, lcomb, sBDeflection_x_design));

                                // Set maximum design ratio of whole structure
                                if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioWholeStructure)
                                {
                                    fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                                    MaximumDesignRatioWholeStructureMember = m;
                                }

                                // Output (for debugging)
                                bool bDebugging = false; // Testovacie ucely
                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString());
                            }
                        }
                    }
                }
                progressValue += step;
                PFDMainWindow.UpdateProgressBarValue(progressValue, "Calculating Member Design. MemberID: " + m.ID);
            }

            progressValue = 100;
            PFDMainWindow.UpdateProgressBarValue(progressValue, "Done.");
            //Member_Design.IsEnabled = true;
            //Internal_Forces.IsEnabled = true;
            System.Diagnostics.Trace.WriteLine("end of calculations: " + (DateTime.Now - start).TotalMilliseconds);
            // TODO Ondrej, zostavovat modely a pocitat vn. sily by malo stacit len pre load cases
            // Pre Load Combinations by sme mali len poprenasobovat hodnoty z load cases faktormi a spocitat ich hodnoty ako jednoduchy sucet, nemusi sa vytvarat nahradny vypoctovy model
            // Potom by mal prebehnut cyklus pre design (vsetky pruty a vsetky load combination, ale uz len pre memberDesignModel s hodnotami vn sil v rezoch)

            string txt = "Calculation Results \n" +
                    "Maximum design ratio \n" +
                    "Member ID: " + MaximumDesignRatioWholeStructureMember.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioWholeStructure, 3).ToString() + "\n\n" +
                    "Maximum design ratio - main columns\n" +
                    "Member ID: " + MaximumDesignRatioMainColumn.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioMainColumn, 3).ToString() + "\n\n" +
                    "Maximum design ratio - rafters\n" +
                    "Member ID: " + MaximumDesignRatioMainRafter.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioMainRafter, 3).ToString() + "\n\n" +
                    "Maximum design ratio - end columns\n" +
                    "Member ID: " + MaximumDesignRatioEndColumn.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioEndColumn, 3).ToString() + "\n\n" +
                    "Maximum design ratio - end rafters\n" +
                    "Member ID: " + MaximumDesignRatioEndRafter.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioEndRafter, 3).ToString() + "\n\n" +
                    "Maximum design ratio - girts\n" +
                    "Member ID: " + MaximumDesignRatioGirt.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioGirts, 3).ToString() + "\n\n" +
                    "Maximum design ratio - purlins\n" +
                    "Member ID: " + MaximumDesignRatioPurlin.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioPurlins, 3).ToString() + "\n\n" +
                    "Maximum design ratio - columns\n" +
                    "Member ID: " + MaximumDesignRatioColumn.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioColumns, 3).ToString() + "\n\n";

            PFDMainWindow.ShowMessageBoxInPFDWindow(txt);

            PFDMainWindow.UpdateResults();
        }

        private void GetMinAndMaxValueInTheArray(float[] array, out float min, out float max)
        {
            if (array != null)
            {
                min = max = array[0];

                foreach (float f in array)
                {
                    if (Math.Abs(f) > Math.Abs(min))
                        min = f;

                    if (Math.Abs(f) > Math.Abs(max))
                        max = f;
                }
            }
            else // Exception
            {
                min = max = float.MaxValue;
            }
        }

        private void GetMinAndMaxValueInTheArray(float[,] array, out float min, out float max)
        {
            if (array != null)
            {
                min = max = array[0, 0];

                foreach (float f in array)
                {
                    if (Math.Abs(f) > Math.Abs(min))
                        min = f;

                    if (Math.Abs(f) > Math.Abs(max))
                        max = f;
                }
            }
            else // Exception
            {
                min = max = float.MaxValue;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
