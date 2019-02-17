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

        private bool MShowLoadsOnFrameMembers;

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

        public List<CMemberInternalForcesInLoadCases> MemberInternalForces;
        public List<CMemberDeflectionsInLoadCases> MemberDeflections;
        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS;
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS;
        public List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;

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
                    MShowLoadsOnFrameMembers);
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

            // Ukazkovy vypocet pre jednu vaznicu, po kompletnom dokonceni vypoctu zmazat ak sa nic z toho nepouzije
            if (false) // Ukazkovy vypocet, zatial nemazat
            {
                float fA_g = (float)Model.m_arrCrSc[4].A_g;
                float fPurlinSelfWeight = fA_g * fMaterial_density * GlobalConstants.fg_acceleration;
                float fPurlinDeadLoadLinear = GeneralLoad.fDeadLoadTotal_Roof * PurlinDistance + fPurlinSelfWeight;
                float fPurlinImposedLoadLinear = Loadinput.ImposedActionRoof * 1000 * PurlinDistance;
                float fsnowValue = Snow.fs_ULS_Nu_1 * ((0.5f * GableWidth) / ((0.5f * GableWidth) / (float)Math.Cos(fRoofPitch_radians))); // Consider projection acc. to Figure 4.1
                float fPurlinSnowLoadLinear = fsnowValue * PurlinDistance;

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // TEMPORARY - vypocet na modeli jedneho pruta
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                float fPurlinWindLoadLinear = Wind.fp_e_max_D_roof_ULS_Theta_4[0, 0];

                float fp_i_min_min;
                float fp_i_min_max;
                float fp_i_max_min;
                float fp_i_max_max;

                GetMinAndMaxValueInTheArray(Wind.fp_i_min_ULS_Theta_4, out fp_i_min_min, out fp_i_min_max);
                GetMinAndMaxValueInTheArray(Wind.fp_i_max_ULS_Theta_4, out fp_i_max_min, out fp_i_max_max);

                float[] fp_e_min_min = new float[3];
                float[] fp_e_min_max = new float[3];
                float[] fp_e_max_min = new float[3];
                float[] fp_e_max_max = new float[3];

                GetMinAndMaxValueInTheArray(Wind.fp_e_min_D_roof_ULS_Theta_4, out fp_e_min_min[0], out fp_e_min_max[0]);
                GetMinAndMaxValueInTheArray(Wind.fp_e_min_U_roof_ULS_Theta_4, out fp_e_min_min[1], out fp_e_min_max[1]);
                GetMinAndMaxValueInTheArray(Wind.fp_e_min_R_roof_ULS_Theta_4, out fp_e_min_min[2], out fp_e_min_max[2]);

                GetMinAndMaxValueInTheArray(Wind.fp_e_max_D_roof_ULS_Theta_4, out fp_e_max_min[0], out fp_e_max_max[0]);
                GetMinAndMaxValueInTheArray(Wind.fp_e_max_U_roof_ULS_Theta_4, out fp_e_max_min[1], out fp_e_max_max[1]);
                GetMinAndMaxValueInTheArray(Wind.fp_e_max_R_roof_ULS_Theta_4, out fp_e_max_min[2], out fp_e_max_max[2]);

                float fp_e_min_min_value;
                float fp_e_min_max_value;
                float fp_e_max_min_value;
                float fp_e_max_max_value;

                GetMinAndMaxValueInTheArray(fp_e_min_min, out fp_e_min_min_value, out fp_e_min_max_value);
                GetMinAndMaxValueInTheArray(fp_e_max_max, out fp_e_max_min_value, out fp_e_max_max_value);

                float fp_min = fp_i_min_min + fp_e_min_min_value;
                float fp_max = fp_i_max_max + fp_e_max_max_value;

                float fWu_min_linear = fp_min * PurlinDistance;
                float fWu_max_linear = fp_max * PurlinDistance;

                // Transform loads from global coordinate system to the purlin coordinate system
                float fSinAlpha = (float)Math.Sin((RoofPitch_deg / 180f) * MathF.fPI);
                float fCosAlpha = (float)Math.Cos((RoofPitch_deg / 180f) * MathF.fPI);

                float fPurlinDeadLoadLinear_LCS_y = fPurlinDeadLoadLinear * fSinAlpha;
                float fPurlinDeadLoadLinear_LCS_z = fPurlinDeadLoadLinear * fCosAlpha;

                float fPurlinImposedLoadLinear_LCS_y = fPurlinImposedLoadLinear * fSinAlpha;
                float fPurlinImposedLoadLinear_LCS_z = fPurlinImposedLoadLinear * fCosAlpha;

                float fPurlinSnowLoadLinear_LCS_y = fPurlinSnowLoadLinear * fSinAlpha;
                float fPurlinSnowLoadLinear_LCS_z = fPurlinSnowLoadLinear * fCosAlpha;

                // Combinations of action
                // 4.2.2 Strength
                // Purlin (a) (b) (d) (e) (g)
                /*
                int iNumberOfLoadCombinations = 5;
                float[] fE_d_load_values_LCS_y = new float[iNumberOfLoadCombinations];

                // Ukazka generovania kombinacii

                fE_d_load_values_LCS_y[0] = 1.35f * fPurlinDeadLoadLinear_LCS_y;                                              // 4.2.2 (a)
                fE_d_load_values_LCS_y[1] = 1.20f * fPurlinDeadLoadLinear_LCS_y + 1.50f * fPurlinImposedLoadLinear_LCS_y;     // 4.2.2 (b)
                fE_d_load_values_LCS_y[2] = 1.20f * fPurlinDeadLoadLinear_LCS_y;                                              // 4.2.2 (d)
                fE_d_load_values_LCS_y[3] = 0.90f * fPurlinDeadLoadLinear_LCS_y;                                              // 4.2.2 (e)
                fE_d_load_values_LCS_y[4] = 1.20f * fPurlinDeadLoadLinear_LCS_y + fPurlinSnowLoadLinear_LCS_y;                // 4.2.2 (g)

                float[] fE_d_load_values_LCS_z = new float[iNumberOfLoadCombinations];

                fE_d_load_values_LCS_z[0] = 1.35f * fPurlinDeadLoadLinear_LCS_z;                                              // 4.2.2 (a)
                fE_d_load_values_LCS_z[1] = 1.20f * fPurlinDeadLoadLinear_LCS_z + 1.50f * fPurlinImposedLoadLinear_LCS_z;     // 4.2.2 (b)
                fE_d_load_values_LCS_z[2] = 1.20f * fPurlinDeadLoadLinear_LCS_z + fWu_max_linear;                             // 4.2.2 (d)
                fE_d_load_values_LCS_z[3] = 0.90f * fPurlinDeadLoadLinear_LCS_z + Math.Abs(fWu_min_linear);                   // 4.2.2 (e)
                fE_d_load_values_LCS_z[4] = 1.20f * fPurlinDeadLoadLinear_LCS_z + fPurlinSnowLoadLinear_LCS_z;                // 4.2.2 (g)
                */
            }

            const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
            const int iNumberOfSegments = iNumberOfDesignSections - 1;

            float[] fx_positions = new float[iNumberOfDesignSections];
            designBucklingLengthFactors sBucklingLengthFactors = new designBucklingLengthFactors();
            designMomentValuesForCb sMomentValuesforCb = new designMomentValuesForCb();
            basicInternalForces[] sBIF_x = null;
            basicDeflections[] sBDeflections_x = null;

            // Tu by sa mal napojit 3D FEM vypocet
            //RunFEMSOlver();

            ////////////////////////////////////////////////////////////////////////
            // TODO 201 - TO Ondrej - tu som nieco zacal tvorit ale po napojeni BFENet mi to nefunguje, lebo je tam nejaka vynimka s WPF
            // Calculation of frame model

            // Extract 2D frames from complex 3D model and create frame models

            //CModel_PFD model_temp = Model;

            //if (Model is CModel_PFD_01_GR)
            //    model_temp = (CModel_PFD_01_GR)Model; // TODO - ziskat pristup na data 3D modelu, zatial som to urobil takto
            // j eurcite nejaky dovod to pretypovavat?

            //List<CModel> frameModels = new List<CModel>(); // Zoznam vsetkych frames - este neviem ci bude potrebny

            CModel_PFD_01_GR model = (CModel_PFD_01_GR)Model;
            List<CFrame> frames = model.GetFramesFromModel();
            foreach (CFrame frame in frames)
            {                
                // 1. Create SW_EN Model of frame (Extract data from 3D model)
                CModel frameModel_i = new Examples.CExample_2D_15_PF(
                            frame.Members,
                            model.GetFrameCNSupports(frame), // TODO - mali by sme prebrat len typ podpory na stlpoch konkretneho ramu a nie vsetky z 3D modelu
                            Model.m_arrLoadCases, // TODO Ondrej - prevziat aj loads on members (MMemberLoadsList priradeny v Load case, ale zozname ponechat len zatazenia prutov ktore sa nachadzaju v rame) alebo ich dogenerovat podla polohy frame Y = i * L1_frame
                            Model.m_arrLoadCombs);

                // TO Ondrej - potrebujeme dogenerovat do load cases jednotlive zatazenia na rame, kedze je to option v GUI, tak sa v CModel_PFD_01_GR nevyrobili a nie je co pocitat


                //frameModels.Add(frameModel_i); // Add particular frame to the list of frames // // Zoznam vsetkych frames - este neviem ci bude potrebny

                // 2. Create BFENet model of frame and calculate internal forces on frame
                RunExample3 bfenetModel = new RunExample3(); // TO Ondrej - Toto prepojenie na BFENet a komunikaciu v ramci BFENet by chcelo nejako skulturnit

                List<List<List<basicInternalForces>>> internalforces;
                // TO Ondrej - Toto prepojenie na BFENet a komunikaciu v ramci BFENet by chcelo nejako skulturnit
                bfenetModel.Example3(frameModel_i, out internalforces); // TO Ondrej - Example3 bola staticka metoda, zmenil som ju - je to urobene v tom duchu ako su priklady v BriefFiniteElementNet.CodeProjectExamples trieda Program.cs ale treba to dam do nejakeho wrappera
                                                                        // TO Ondrej - Ak teraz spustim Calculate tak to nefunguje, je tam nejaka vynimka neviem ci to nesuvisi s tym ze som to static zakomentoval

                // TODO  201 - To Ondrej  - potrebujeme do ramu dostat aj prvky spojov, resp upravit to tak ze vnutorne sily sa nastavia prutom v hlavnom modeli a potom prebehne posudzovanie na prutoch hlavneho modelu, teraz to pada na spojoch, predpokladam ze preto lebo to posudzuje members z modelu samostatneho ramu kde nie su ziadne spoje


                // 3. Assign results to the original members from 3D model
                // TO Ondrej - vytvorit zoznamy CMemberInternalForcesInLoadCases

                // 4. Run design of frame members

                // 5. Run design of joints
            }

            //for (int iFrameIndex = 0; iFrameIndex < Frames; iFrameIndex++)
            //{
            //    // Determinate particular frame member indices
            //    int iEavesPurlinNoInOneFrame = 2;
            //    int iFrameNodesNo = 5;

            //    // TO Ondrej - obecnejsie by bolo nacitat podla hodnoty Y vsetky pruty v reze, tento kod plati len ak je ram zo 4 prutov, 
            //    //ale do buducna moze byt ich pocet iny
            //    // Pripadne mozeme prutom dat nejaky priznak ci sa nachadzaju v nejakom rame alebo vytvorit "skupinu/list" ramov 
            //    //(objekt Frame ktory bude v sebe obsahovat zoznam prutov) uz v CModel_PFD_01_GR a podobne a s touto identifikaciou potom pracovat v celom vypocte
            //    int indexColumn1Left  = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * (iFrameNodesNo - 1) + 0;
            //    int indexRafter1Left  = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * (iFrameNodesNo - 1) + 1;
            //    int indexRafter2Right = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * (iFrameNodesNo - 1) + 2;
            //    int indexColumn2Right = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * (iFrameNodesNo - 1) + 3;

            //    // Create array of frame members (extracted from 3D model)
            //    CMember[] members = new CMember[4];
            //    members[0] = Model.m_arrMembers[indexColumn1Left];
            //    members[1] = Model.m_arrMembers[indexRafter1Left];
            //    members[2] = Model.m_arrMembers[indexRafter2Right];
            //    members[3] = Model.m_arrMembers[indexColumn2Right];

            //    // 1. Create SW_EN Model of frame (Extract data from 3D model)
            //    CModel frameModel_i = new Examples.CExample_2D_15_PF(
            //                members,
            //                Model.m_arrNSupports, // TODO - mali by sme prebrat len typ podpory na stlpoch konkretneho ramu a nie vsetky z 3D modelu
            //                Model.m_arrLoadCases, // TODO Ondrej - prevziat aj loads on members (MMemberLoadsList priradeny v Load case) alebo ich dogenerovat podla polohy frame Y = i * L1_frame
            //                Model.m_arrLoadCombs);

            //    //frameModels.Add(frameModel_i); // Add particular frame to the list of frames // // Zoznam vsetkych frames - este neviem ci bude potrebny

            //    // 2. Create BFENet model of frame and calculate internal forces on frame
            //    RunExample3 bfenetModel = new RunExample3();

            //    List<List<List<basicInternalForces>>> internalforces;
            //    bfenetModel.Example3(frameModel_i, out internalforces); // TO Ondrej - Example3 bola staticka metoda, zmenil som ju - je to urobene v tom duchu ako su priklady v BriefFiniteElementNet.CodeProjectExamples trieda Program.cs ale treba to dam do nejakeho wrappera
            //    // TO Ondrej - Ak teraz spustim Calculate tak to nefunguje, je tam nejaka vynimka neviem ci to nesuvisi s tym ze som to static zakomentoval


            //    // 3. Assign results to the original members from 3D model
            //    // TO Ondrej - vytvorit zoznamy CMemberInternalForcesInLoadCases

            //    // 4. Run design of frame members

            //    // 5. Run design of joints

            //}

            // Calculation of simple beam model
            float fMaximumDesignRatioWholeStructure = 0;
            float fMaximumDesignRatioGirts = 0;
            float fMaximumDesignRatioPurlins = 0;
            float fMaximumDesignRatioColumns = 0;

            CMember MaximumDesignRatioWholeStructureMember = new CMember();
            CMember MaximumDesignRatioGirt = new CMember();
            CMember MaximumDesignRatioPurlin = new CMember();
            CMember MaximumDesignRatioColumn = new CMember();

            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();
            MemberInternalForces = new List<CMemberInternalForcesInLoadCases>();
            MemberDeflections = new List<CMemberDeflectionsInLoadCases>();

            System.Diagnostics.Trace.WriteLine("before calculations: " + (DateTime.Now - start).TotalMilliseconds);

            double step = 100.0 / (Model.m_arrMembers.Length * 2.0);
            double progressValue = 0;
            PFDMainWindow.UpdateProgressBarValue(progressValue, "");

            // Calculate Internal Forces For Load Cases
            foreach (CMember m in Model.m_arrMembers)
            {
                if (m.BIsDSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
                {
                    for (int i = 0; i < iNumberOfDesignSections; i++)
                        fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                    m.MBucklingLengthFactors = new List<designBucklingLengthFactors>();
                    m.MMomentValuesforCb = new List<designMomentValuesForCb>();
                    m.MBIF_x = new List<basicInternalForces[]>();
                    m.MBDef_x = new List<basicDeflections[]>();

                    foreach (CLoadCase lc in Model.m_arrLoadCases)
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

                                    // SLS - deflections
                                    calcModel.CalculateDeflectionsOnSimpleBeam_PFD(iNumberOfDesignSections, fx_positions, m, (CMLoad_21)cmload, out sBDeflections_x);
                                }
                            }
                        }

                        if (sBIF_x != null) MemberInternalForces.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb));
                        if (sBDeflections_x != null) MemberDeflections.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));

                        //m.MMomentValuesforCb.Add(sMomentValuesforCb);
                        //m.MBIF_x.Add(sBIF_x);
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
                        if (lcomb.eLComType == ELSType.eLS_ULS) // Do not perform internal foces calculation for SLS
                        {
                            // Member basic internal forces
                            designBucklingLengthFactors sBucklingLengthFactors_design;
                            designMomentValuesForCb sMomentValuesforCb_design;
                            basicInternalForces[] sBIF_x_design;
                            CMemberResultsManager.SetMemberInternalForcesInLoadCombination(m, lcomb, MemberInternalForces, iNumberOfDesignSections, out sBucklingLengthFactors_design, out sMomentValuesforCb_design, out sBIF_x_design);

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
                                    case EMemberType_FormSteel.eG: // Girt
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioGirts)
                                            {
                                                fMaximumDesignRatioGirts = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioGirt = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FormSteel.eP: // Purlin
                                        {
                                            if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioPurlins)
                                            {
                                                fMaximumDesignRatioPurlins = memberDesignModel.fMaximumDesignRatio;
                                                MaximumDesignRatioPurlin = m;
                                            }
                                            break;
                                        }
                                    case EMemberType_FormSteel.eC: // Column
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
                            CMemberResultsManager.SetMemberDeflectionsInLoadCombination(m, lcomb, MemberDeflections, iNumberOfDesignSections, out sBDeflection_x_design);

                            // Member design deflections
                            if (m.BIsDSelectedForDesign) // Only structural members (not auxiliary members or members with deactivated design)
                            {
                                designDeflections[] sDDeflection_x;
                                CMemberDesign memberDesignModel = new CMemberDesign();
                                memberDesignModel.SetDesignDeflections_PFD(iNumberOfDesignSections, m, sBDeflection_x_design, out sDDeflection_x);
                                MemberDesignResults_SLS.Add(new CMemberLoadCombinationRatio_SLS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sDDeflection_x[memberDesignModel.fMaximumDesignRatioLocationID]));

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
                    "Maximum design ratio - girts\n" +
                    "Member ID: " + MaximumDesignRatioGirt.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioGirts, 3).ToString() + "\n\n" +
                    "Maximum design ratio - purlins\n" +
                    "Member ID: " + MaximumDesignRatioPurlin.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioPurlins, 3).ToString() + "\n\n" +
                    "Maximum design ratio - columns\n" +
                    "Member ID: " + MaximumDesignRatioColumn.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioColumns, 3).ToString() + "\n\n";

            PFDMainWindow.ShowMessageBoxInPFDWindow(txt);

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
