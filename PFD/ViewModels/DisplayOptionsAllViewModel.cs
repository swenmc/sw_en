using BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace PFD
{
    [Serializable]
    public class DisplayOptionsAllViewModel : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        private List<DisplayOptionsViewModel> m_DisplayOptionsList;

        public bool IsSetFromCode = false;

        public List<DisplayOptionsViewModel> DisplayOptionsList
        {
            get
            {
                if (m_DisplayOptionsList == null) m_DisplayOptionsList = new List<DisplayOptionsViewModel>();
                return m_DisplayOptionsList;
            }

            set
            {
                m_DisplayOptionsList = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------




        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public DisplayOptionsAllViewModel()
        {
            IsSetFromCode = true;

            CreateAllViewModelsWithDefaults();

            IsSetFromCode = false;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetViewModel(DisplayOptionsAllViewModel newVM)
        {
            IsSetFromCode = true;

            DisplayOptionsList = newVM.DisplayOptionsList;

            IsSetFromCode = false;
        }


        private void CreateAllViewModelsWithDefaults()
        {
            m_DisplayOptionsList = new List<DisplayOptionsViewModel>();

            for (int i = 0; i <= (int)EDisplayOptionsTypes.Layouts_Foundations; i++)
            {
                m_DisplayOptionsList.Add(new DisplayOptionsViewModel());
            }

            InitViewModelsDefaults();
        }

        private void InitViewModelsDefaults()
        {
            // IN WORK 17.6.2021
            // TODO - 701

            #region GUI
            #region 3D SCENE
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LightDirectional = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LightPoint = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LightSpot = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LightAmbient = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].MaterialDiffuse = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].MaterialEmissive = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayJoints = true; // Vypnute v defaulte
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayPlates = true; // Vypnute v defaulte
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayConnectors = true; // Vypnute v defaulte
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFoundations = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayReinforcementBars = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFloorSlab = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplaySawCuts = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayControlJoints = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayMembersWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayJointsWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayPlatesWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayConnectorsWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayNodesWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFoundationsWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayReinforcementBarsWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFloorSlabWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFibreglassWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDoorsWireFrame = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayWindowsWireFrame = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DoorsSimpleSolidModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DoorsSimpleWireframe = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].WindowOutlineOnly = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladding = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingLeftWall = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingRightWall = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingFrontWall = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingBackWall = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingRoof = false;
            //DisplayIndividualCladdingSheets = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFibreglass = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDoors = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayWindows = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayNodalSupports = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayMembersCenterLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplaySolidModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayWireFrameModel = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDistinguishedColorMember = false;
            //DisplayTransparentModelMember = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ColorsAccordingToMembersPrefix = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ColorsAccordingToMembersPosition = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ColorsAccordingToSections = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ColoredCenterlines = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].CladdingSheetColoursByID = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowNodesDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowMemberDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowMemberID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowMemberPrefix = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowMemberRealLength = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowMemberRealLengthInMM = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowMemberRealLengthUnit = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowMemberCrossSectionStartName = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowFoundationsDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowSawCutsDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowControlJointsDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowDimensions = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowGridLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowDetailSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowSlabRebates = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingID = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingPrefix = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingLengthWidth = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingArea = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayCladdingUnits = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFibreglassDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFibreglassID = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFibreglassPrefix = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFibreglassLengthWidth = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFibreglassArea = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFibreglassUnits = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDoorDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDoorID = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDoorType = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDoorHeightWidth = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDoorArea = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayDoorUnits = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayWindowDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayWindowID = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayWindowHeightWidth = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayWindowArea = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayWindowUnits = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].CladdingSheetColoursByID = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].UseTextures = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].UseTexturesMembers = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].UseTexturesPlates = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].UseTexturesCladding = false;

            if (false /*bRelease TODO 701 - zakomentovane */) // Vsetko okrem centerlines a zakladnych kot vypneme
            {
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LightAmbient = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayJoints = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayPlates = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFoundations = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayFloorSlab = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplaySawCuts = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayControlJoints = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayMembersCenterLines = true; // Zobrazujeme
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplaySolidModel = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowSlabRebates = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowMemberDescription = false;
            }

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoads = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoadsOnMembers = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoadsOnGirts = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoadsOnPurlins = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoadsOnEavePurlins = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoadsOnWindPosts = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoadsOnFrameMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowNodalLoads = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowSurfaceLoads = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoadsLabels = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLoadsLabelsUnits = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowGlobalAxis = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowLocalMembersAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ShowSurfaceLoadsAxis = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DisplayIn3DRatio = 0.003f;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.CadetBlue);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].WireFrameLineThickness = 2;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].MemberCenterlineColorIndex = CComboBoxHelper.GetColorIndex(Colors.WhiteSmoke);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].MemberCenterlineThickness = 2;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].NodeDescriptionTextFontSize = 12;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].MemberDescriptionTextFontSize = 12;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DimensionTextFontSize = 12;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].GridLineLabelTextFontSize = 30;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SectionSymbolLabelTextFontSize = 30;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DetailSymbolLabelTextFontSize = 30;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SawCutTextFontSize = 12;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ControlJointTextFontSize = 12;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FoundationTextFontSize = 12;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FloorSlabTextFontSize = 12;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].NodeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].NodeDescriptionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].MemberDescriptionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Beige);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DimensionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGreen);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DimensionLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGreen);

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].GridLineLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Coral);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].GridLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Coral);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].GridLinePatternType = (int)ELinePatternType.DASHDOTTED;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SectionSymbolLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SectionSymbolColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DetailSymbolLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightPink);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DetailSymbolLabelBackColorIndex = CComboBoxHelper.GetColorIndexWithTransparent(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DetailSymbolColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightPink);

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SawCutTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Goldenrod);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SawCutLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Goldenrod);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SawCutLinePatternType = (int)ELinePatternType.DOTTED;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ControlJointTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.BlueViolet);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ControlJointLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.BlueViolet);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ControlJointLinePatternType = (int)ELinePatternType.DIVIDE;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FoundationTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.HotPink);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FloorSlabTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.HotPink);

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FoundationColorIndex = CComboBoxHelper.GetColorIndex(Colors.DarkGray);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FloorSlabColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGray);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SlabRebateColorIndex = CComboBoxHelper.GetColorIndex(Colors.DarkOrange);

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].CladdingTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.OrangeRed);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FibreglassTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Indigo);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DoorTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Coral);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].WindowTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Cyan);

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].PlateColorIndex = CComboBoxHelper.GetColorIndex(Colors.Gray);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ScrewColorIndex = CComboBoxHelper.GetColorIndex(Colors.Blue);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].AnchorColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGoldenrodYellow);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].WasherColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightGreen);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].NutColorIndex = CComboBoxHelper.GetColorIndex(Colors.LightPink);

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].CladdingSheetColorIndex = CComboBoxHelper.GetColorIndex(Colors.Yellow);
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FibreglassSheetColorIndex = CComboBoxHelper.GetColorIndex(Colors.OrangeRed);

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].MemberSolidModelOpacity = 0.8f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].PlateSolidModelOpacity = 0.5f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ScrewSolidModelOpacity = 0.9f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].AnchorSolidModelOpacity = 0.9f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FoundationSolidModelOpacity = 0.4f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ReinforcementBarSolidModelOpacity = 0.9f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FloorSlabSolidModelOpacity = 0.3f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SlabRebateSolidModelOpacity = 0.3f;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FrontCladdingOpacity = 0.95f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].LeftCladdingOpacity = 0.95f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].RoofCladdingOpacity = 0.95f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FlashingOpacity = 0.90f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DoorPanelOpacity = 0.95f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].WindowPanelOpacity = 0.95f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FibreglassOpacity = 0.70f;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].BackgroundColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            //// Text 7.0 mm
            //ExportGridlinesSize = 1f / 20f;
            //ExportGridLineLabelSize = 1f / 40f;

            //ExportSectionSymbolsSize = 1f / 20f;
            //ExportSectionSymbolLabelSize = 1f / 40f;
            //ExportDetailSymbolSize = 1f / 20f;
            //ExportDetailSymbolLabelSize = 1 / 40f;

            //// Text 2.5 - 3.0 mm
            //ExportFloorSlabTextSize = 1f / 20f; // 4 riadky + 3 medzery
            //ExportMembersDescriptionSize = 1f / 100f;
            //ExportNodesDescriptionSize = 1f / 100f;
            //ExportSawCutTextSize = 1f / 100f;
            //ExportControlJointTextSize = 1f / 100f;
            //ExportFoundationTextSize = 1f / 100f;
            //ExportDimensionsTextSize = 1f / 100f;
            //ExportDimensionsLineRadius = 1f / 1500f;
            //ExportDimensionsScale = 1f / 10f;
            //ExportDescriptionTextWidthScaleFactor = 0.6f;
            //ExportCladdingDescriptionSize = 1f / 100f;
            //ExportFibreglassDescriptionSize = 1f / 100f;
            //ExportDoorDescriptionSize = 1f / 100f;
            //ExportWindowDescriptionSize = 1f / 100f;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].GridlinesSize = 1f / 20f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].GridLineLabelSize = 1f / 40f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SectionSymbolsSize = 1f / 20f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SectionSymbolLabelSize = 1f / 40f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DetailSymbolSize = 1f / 20f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DetailSymbolLabelSize = 1 / 40f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FloorSlabTextSize = 1f / 20f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].MembersDescriptionSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].NodesDescriptionSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].SawCutTextSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].ControlJointTextSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FoundationTextSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DimensionsTextSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DimensionsLineRadius = 1f / 1500f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DimensionsScale = 1f / 10f;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DescriptionTextWidthScaleFactor = 0.3f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].CladdingDescriptionSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].FibreglassDescriptionSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].DoorDescriptionSize = 1f / 100f;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene].WindowDescriptionSize = 1f / 100f;

            #endregion 3D SCENE
            #region JOINT PREVIEW
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview] = DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene];
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].ShowGlobalAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].ShowMemberDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].ShowNodesDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].DisplayMembersCenterLines = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].DisplaySolidModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].DisplayMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].DisplayJoints = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].DisplayPlates = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview].DisplayConnectors = true;
            #endregion JOINT PREVIEW
            #region FOUNDATION PREVIEW
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview] = DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene];
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].ShowGlobalAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].ShowMemberDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].ShowNodesDescription = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplaySolidModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayJoints = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayPlates = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayConnectors = true;

            // Foundations
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayFoundations = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayReinforcementBars = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].RotateModelX = -80;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].RotateModelY = 45;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].RotateModelZ = 5;

            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayFoundations = true; // Display always footing pads
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview].DisplayReinforcementBars = true; // Display always reinforcement bars
            #endregion FOUNDATION PREVIEW
            #region ACCESSORIES PREVIEW
            // TODO
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Accessories_Preview] = DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_3D_Scene];
            DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Accessories_Preview].DisplaySolidModel = true;

            #endregion ACCESSORIES PREVIEW
            #endregion GUI
            #region REPORT
            #region 3D SCENE

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].MembersDescriptionSize = 1f / 60f;  //??? tu je zmena k tasku 701 - zrusene bolo ExportMembersDescriptionSize
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].ViewsPageSize = EPageSizes.A4;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].CreateHorizontalGridlines = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].CreateVerticalGridlinesFront = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].CreateVerticalGridlinesBack = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].CreateVerticalGridlinesLeft = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].CreateVerticalGridlinesRight = false;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].UseOrtographicCamera = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].DisplaySolidModel = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].DisplayMembersCenterLines = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].DisplayJoints = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].DisplayPlates = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].ShowMemberDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].ShowMemberID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].ShowMemberCrossSectionStartName = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].ShowMemberPrefix = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].ShowMemberRealLength = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].ShowMemberRealLengthInMM = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].ShowMemberRealLengthUnit = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].MemberDescriptionTextColor = Colors.Black;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_3DScene].MemberCenterlineColor = Colors.Black;

            #endregion 3D SCENE
            #region JOINTS
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints] = DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview];

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].IsExport = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayMembersCenterLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplaySolidModel = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayJoints = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayPlates = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayConnectors = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].ShowNodesDescription = false;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].UseOrtographicCamera = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].ShowGlobalAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].ShowMemberDescription = false;

            // Do dokumentu exporujeme aj s wireframe
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayWireFrameModel = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].TransformScreenLines3DToCylinders3D = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayMembersWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayJointsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayPlatesWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].DisplayConnectorsWireFrame = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Joints].WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black); // Farba linii pre export, moze sa urobit nastavitelna samostatne pre 3D preview a export

            #endregion JOINTS
            #region FOUNDATIONS
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations] = DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview];
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].IsExport = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayMembersCenterLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplaySolidModel = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayJoints = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayPlates = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayConnectors = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].ShowNodesDescription = false;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].UseOrtographicCamera = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].ShowGlobalAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].ShowMemberDescription = false;

            // Do dokumentu exporujeme aj s wireframe
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayWireFrameModel = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].TransformScreenLines3DToCylinders3D = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayMembersWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayJointsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayPlatesWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayConnectorsWireFrame = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black); // Farba linii pre export, moze sa urobit nastavitelna samostatne pre 3D preview a export

            // Foundations
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayFoundations = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayReinforcementBars = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayFoundationsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].DisplayReinforcementBarsWireFrame = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].RotateModelX = -80;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].RotateModelY = 45;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Report_Foundations].RotateModelZ = 5;

            #endregion FOUNDATIONS
            #endregion REPORT
            #region LAYOUTS
            #region 3D SCENE
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].CenterLinesMemberModelAndIDs = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].View = EModelViews.ISO_FRONT_RIGHT;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].Filter = EViewModelMemberFilters.All;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ModelView = (int)view;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ViewModelMembers = (int)filter;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].IsExport = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].UseOrtographicCamera = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ColorsAccordingToMembersPrefix = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ColorsAccordingToSections = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowGlobalAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowMemberDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplaySolidModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayMembersCenterLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayWireFrameModel = false; //musi byt false, lebo to je neskutocne vela dat a potom OutOfMemory Exception
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].TransformScreenLines3DToCylinders3D = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayJoints = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayPlates = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowNodesDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayNodalSupports = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayFoundations = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayFloorSlab = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplaySawCuts = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayControlJoints = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowFoundationsDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowFloorSlabDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowSawCutsDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowControlJointsDescription = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowGridLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowDetailSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowDimensions = false; // V zakladnom 3D nezobrazujeme koty

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].CreateHorizontalGridlines = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].CreateVerticalGridlinesFront = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].CreateVerticalGridlinesBack = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].CreateVerticalGridlinesLeft = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].CreateVerticalGridlinesRight = false;

            if (false /* TODO 701 - zakomentovane bCenterLinesMemberModelAndIDs */) // Prenastavujeme hodnoty pre centerline model a zobrazene member IDs
            {
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplaySolidModel = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayMembersCenterLines = true;

                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayJoints = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].DisplayPlates = false;

                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowMemberDescription = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowMemberID = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowMemberCrossSectionStartName = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowMemberPrefix = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowMemberRealLength = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowMemberRealLengthInMM = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].ShowMemberRealLengthUnit = false;

                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].MemberDescriptionTextColor = Colors.Black;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_3D_Scene].MemberCenterlineColor = Colors.Black;
            }
            #endregion 3D SCENE
            #region FRAME VIEWS
            #region ELEVATIONS

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].UseOrtographicCamera = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ColorsAccordingToMembersPrefix = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ColorsAccordingToSections = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowGlobalAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplaySolidModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayMembersCenterLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayWireFrameModel = false;   //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].TransformScreenLines3DToCylinders3D = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberID = false; // V Defaulte nezobrazujeme unikatne cislo pruta

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowNodesDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayNodalSupports = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayMembers = true;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayJoints = true;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayPlates = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplaySawCuts = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayControlJoints = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowGridLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowDetailSymbols = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayCladding = false;

            // TO Ondrej - Tu by to chcelo vymysliet nejaky mechanizmus, ktory na zaklade rozmerov vykresu a velkosti obrazku modelu urci aka ma byt vyska textu v jednotlivych pohladoch, na papieri by to malo byt cca - 2-2.5 mm, pripadne do 3 mm (6 - 8 PT)
            // Vysku textu mozeme nastavovat ako velkost fontu ale pre export do 2D je lepsie uzivatelsky nastavovat velkost v mm lebo stavbari nevedia aky velky je font c. 8, pripadne tam bude prepocet z bodov na mm

            /*
            07.0 PT     09 PX     2.5 MM     0.60 EM     060 %
            07.0 PT     10 PX     2.5 MM     0.60 EM     060 %
            08.0 PT     11 PX     2.8 MM     0.70 EM     070 %
            09.0 PT     12 PX     3.4 MM     0.80 EM     080 %
            09.0 PT     13 PX     3.4 MM     0.80 EM     080 %
            10.0 PT     13 PX     3.4 MM     0.80 EM     080 %
            10.5 PT     14 PX     3.6 MM     0.85 EM     085 %
            11.0 PT     15 PX     3.9 MM     0.95 EM     095 %
            12.0 PT     16 PX     4.2 MM     1.05 EM     105 %
            12.0 PT     17 PX     4.2 MM     1.05 EM     105 %
            13.0 PT     17 PX     4.2 MM     1.10 EM     110 %
            13.0 PT     18 PX     4.8 MM     1.10 EM     110 %
            14.0 PT     19 PX     5.0 MM     1.20 EM     120 %
            15.0 PT     20 PX     5.4 MM     1.33 EM     133 %
            16.0 PT     21 PX     5.8 MM     1.40 EM     140 %
            16.0 PT     22 PX     5.8 MM     1.40 EM     140 %
            17.0 PT     23 PX     6.2 MM     1.50 EM     150 %
            */

            //-----------------------------------------------------------
            // TODO 701 - Pridane
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].WireFrameLineThickness = 2;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].NodeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].NodeDescriptionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CladdingTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FibreglassTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DoorTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].WindowTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].BackgroundColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].PlateColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ScrewColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].AnchorColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].WasherColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].NutColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ColoredCenterlines = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CladdingSheetColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FibreglassSheetColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            //-----------------------------------------------------------

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].MemberDescriptionTextFontSize = 14; // Font 14 znamena 0.14 m v 3D grafike, takze hodnota / 100f
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].MemberDescriptionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.DarkGreen);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DimensionTextFontSize = 14;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DimensionTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.DarkBlue);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DimensionLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].GridLineLabelTextFontSize = 30;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].GridLineLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].GridLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].GridLinePatternType = (int)ELinePatternType.DASHDOTTED;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SawCutTextFontSize = 14;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SawCutTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SawCutLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SawCutLinePatternType = (int)ELinePatternType.DOTTED;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ControlJointTextFontSize = 14;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ControlJointTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ControlJointLineColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ControlJointLinePatternType = (int)ELinePatternType.DIVIDE;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SectionSymbolLabelTextFontSize = 30;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SectionSymbolLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SectionSymbolColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SectionSymbolLinePatternType = ELinePatternType.DASHDOTTED;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DetailSymbolLabelTextFontSize = 30;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DetailSymbolLabelTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DetailSymbolLabelBackColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DetailSymbolColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DetailSymbolLinePatternType = ELinePatternType.CONTINUOUS;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FoundationTextFontSize = 14;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FoundationColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FoundationTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FloorSlabTextFontSize = 14;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FloorSlabColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FloorSlabTextColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SlabRebateColorIndex = CComboBoxHelper.GetColorIndex(Colors.White);

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ReinforcementBarColor_Top_x = CComboBoxHelper.GetColorIndex(Colors.Black);
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ReinforcementBarColor_Top_y = CComboBoxHelper.GetColorIndex(Colors.Black);
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ReinforcementBarColor_Bottom_x = CComboBoxHelper.GetColorIndex(Colors.Black);
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ReinforcementBarColor_Bottom_y = CComboBoxHelper.GetColorIndex(Colors.Black);

            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].PlateColor = CComboBoxHelper.GetColorIndex(Colors.Gray;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ScrewColor = CComboBoxHelper.GetColorIndex(Colors.Black;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].AnchorColor = CComboBoxHelper.GetColorIndex(Colors.Black;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].WasherColor = CComboBoxHelper.GetColorIndex(Colors.Gray;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].NutColor = CComboBoxHelper.GetColorIndex(Colors.Black;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FoundationSolidModelOpacity = 0;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].FloorSlabSolidModelOpacity = 0;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SlabRebateSolidModelOpacity = 0;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ReinforcementBarSolidModelOpacity = 1;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].IsExport = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].SameScaleForViews = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateHorizontalGridlines = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateVerticalGridlinesFront = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateVerticalGridlinesBack = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateVerticalGridlinesLeft = false;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateVerticalGridlinesRight = false;

            // FRONT VIEW

            if (true /* TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ViewMembers == EViewModelMemberFilters.FRONT*/)
            {
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberDescription = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberPrefix = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberRealLengthInMM = true;

                // DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].bDisplayJoints = true; // Ak chceme zobrazovat znacky detailov, musime do filtrovaneho modelu exportovat aj spoje, bude to zavisiet na tom ci je zapnute ich zobrazenie, alebo to budeme robit vzdy
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowGridLines = true; // Vertical
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowSectionSymbols = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowDetailSymbols = true;
                //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayDimensions = true;

                // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateHorizontalGridlines = false;
                // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateVerticalGridlinesFront = true;
            }

            if (true /* TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ViewMembers == EViewModelMemberFilters.BACK*/)
            {
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberDescription = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberPrefix = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberRealLengthInMM = true;

                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowGridLines = true; // Vertical
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowSectionSymbols = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowDetailSymbols = true;
                //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayDimensions = true;

                // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateHorizontalGridlines = false;
                // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateVerticalGridlinesBack = true;
            }

            if (true /* TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ViewMembers == EViewModelMemberFilters.LEFT*/)
            {
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberDescription = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberPrefix = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberRealLengthInMM = true;

                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowGridLines = true;// Vertical
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowSectionSymbols = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowDetailSymbols = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowDimensions = true;

                // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateHorizontalGridlines = false;
                // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateVerticalGridlinesLeft = true;
            }

            if (true /* TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ViewMembers == EViewModelMemberFilters.RIGHT*/)
            {
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberDescription = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberPrefix = true;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowMemberRealLengthInMM = true;

                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowGridLines = true; // Vertical
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowSectionSymbols = false;
                DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].ShowDetailSymbols = true;
                //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].DisplayDimensions = true;

                // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateHorizontalGridlines = false;
                // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations].CreateVerticalGridlinesRight = true;
            }
            #endregion ELEVATIONS
            // TODO Pre nasledujuce options Frame Views treba doplnit vsade to co je pre elevations
            #region ROOF
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof] = DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations];

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof].ShowMemberDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof].ShowMemberPrefix = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof].ShowMemberRealLengthInMM = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof].ShowGridLines = true; // Horizontal
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof].ShowDetailSymbols = true;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof].DisplayDimensions = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Roof].CreateHorizontalGridlines = true;
            #endregion ROOF
            #region FRAMES
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames] = DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations];

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].ShowMemberDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].ShowMemberPrefix = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].ShowMemberRealLengthInMM = true;

            // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].DisplayWireFrameModel = true;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].DisplayMembersWireFrame = true;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].TransformScreenLines3DToCylinders3D = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].ShowGridLines = true; // Vertical
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].ShowDetailSymbols = false;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].DisplayDimensions = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Frames].CreateVerticalGridlinesFront = true;
            #endregion FRAMES
            #region COLUMNS
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns] = DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations];

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].ShowMemberDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].ShowMemberPrefix = true;

            // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].DisplayWireFrameModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].DisplayFloorSlabWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].DisplayMembersWireFrame = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].TransformScreenLines3DToCylinders3D = true;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].fWireFrameLineThickness = fWireFrameLineThickness_Final;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].DisplayFoundations = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].DisplayReinforcementBars = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].DisplayFloorSlab = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].ShowFloorSlabDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].ShowGridLines = true; // Horizontal
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].ShowDetailSymbols = false;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].DisplayDimensions = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Columns].CreateHorizontalGridlines = true;

            #endregion COLUMNS
            #region FOUNDATIONS
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations] = DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations];

            // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].DisplayWireFrameModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].DisplayFoundationsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].DisplayFloorSlabWireFrame = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].TransformScreenLines3DToCylinders3D = true;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].fWireFrameLineThickness = fWireFrameLineThickness_Final;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].DisplayFoundations = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].DisplayReinforcementBars = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].DisplayFloorSlab = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].ShowFloorSlabDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].ShowFoundationsDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].ShowMemberDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].ShowGridLines = true; // Horizontal
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].ShowDetailSymbols = false;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].DisplayDimensions = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Foundations].CreateHorizontalGridlines = true;

            #endregion FOUNDATIONS
            #region FLOOR
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor] = DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations];

            // Chceme pre ucely exportu zobrazit wireframe a prerobit ciary wireframe na 3D valce
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplayWireFrameModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplayFoundationsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplayFloorSlabWireFrame = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].TransformScreenLines3DToCylinders3D = true;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].fWireFrameLineThickness = fWireFrameLineThickness_Final;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplayFoundations = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplayReinforcementBars = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplayFloorSlab = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].ShowFloorSlabDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].ShowFoundationsDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].ShowMemberDescription = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplaySawCuts = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].ShowSawCutsDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplayControlJoints = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].ShowControlJointsDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].ShowGridLines = true; // Horizontal
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].ShowSectionSymbols = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].ShowDetailSymbols = false;
            //DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].DisplayDimensions = true;

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Floor].CreateHorizontalGridlines = true;
            #endregion FLOOR
            #endregion FRAME VIEWS
            #region CLADDING VIEWS
            #region ELEVATIONS
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations] = DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_FW_Elevations];

            // Defaultne hodnoty pre vsetky pohlady
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].TransformScreenLines3DToCylinders3D = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Red);

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayMembers = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayJoints = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayFoundations = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayFloorSlab = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].ShowLocalMembersAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].ShowDimensions = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladding = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingLeftWall = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingRightWall = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingFrontWall = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingBackWall = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingRoof = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayFibreglass = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayDoors = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayWindows = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayWireFrameModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayFibreglassWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayDoorsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayWindowsWireFrame = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingPrefix = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayCladdingLengthWidth = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayFibreglassDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayFibreglassID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayFibreglassPrefix = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayFibreglassLengthWidth = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayDoorDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayDoorID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayDoorType = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayDoorHeightWidth = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayWindowDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayWindowID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations].DisplayWindowHeightWidth = true;


            #endregion ELEVATIONS
            #region ROOF
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof] = DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Elevations];

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].TransformScreenLines3DToCylinders3D = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Blue); // Pokus

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayMembers = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayJoints = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayFoundations = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayFloorSlab = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].ShowLocalMembersAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].ShowDimensions = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladding = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingLeftWall = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingRightWall = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingFrontWall = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingBackWall = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingRoof = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayFibreglass = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayDoors = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayWindows = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayWireFrameModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayFibreglassWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayDoorsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayWindowsWireFrame = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingPrefix = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayCladdingLengthWidth = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayFibreglassDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayFibreglassID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayFibreglassPrefix = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayFibreglassLengthWidth = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayDoorDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayDoorID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayDoorType = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayDoorHeightWidth = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayWindowDescription = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayWindowID = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_CW_Roof].DisplayWindowHeightWidth = true;

            //if (viewMembers == EViewCladdingFilters.CLADDING_FRONT)
            //{
            //    //opts.bDisplayCladdingFrontWall = true;
            //}
            //else if (viewMembers == EViewCladdingFilters.CLADDING_BACK)
            //{
            //    //opts.bDisplayCladdingBackWall = true;
            //}
            //else if (viewMembers == EViewCladdingFilters.CLADDING_LEFT)
            //{
            //    //opts.bDisplayCladdingLeftWall = true;
            //}
            //else if (viewMembers == EViewCladdingFilters.CLADDING_RIGHT)
            //{
            //    //opts.bDisplayCladdingRightWall = true;
            //}
            //else if (viewMembers == EViewCladdingFilters.CLADDING_ROOF)
            //{
            //    //opts.bDisplayCladdingRoof = true;
            //}


            #endregion ROOF
            #endregion CLADDING VIEWS
            #region JOINTS
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints] = DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Joint_Preview];

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].UseOrtographicCamera = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ColorsAccordingToMembersPrefix = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ColorsAccordingToSections = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ShowGlobalAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplaySolidModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayMembersCenterLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ShowMemberID = false; // V Defaulte nezobrazujeme unikatne cislo pruta
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ShowMemberDescription = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayJoints = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayPlates = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayConnectors = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ShowNodesDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayNodalSupports = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ShowGridLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].ShowDetailSymbols = false;

            // Do dokumentu exporujeme aj s wireframe
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayWireFrameModel = true; //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].WireFrameLineThickness = 2;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].TransformScreenLines3DToCylinders3D = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayMembersWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayJointsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayPlatesWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].DisplayConnectorsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Joints].WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);


            #endregion JOINTS
            #region FOUNDATIONS
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations] = DisplayOptionsList[(int)EDisplayOptionsTypes.GUI_Foundation_Preview];

            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].UseOrtographicCamera = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ColorsAccordingToMembersPrefix = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ColorsAccordingToSections = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ShowGlobalAxis = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplaySolidModel = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayMembersCenterLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ShowMemberID = false; // V Defaulte nezobrazujeme unikatne cislo pruta
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ShowMemberDescription = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayMembers = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayJoints = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayPlates = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayConnectors = true;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayNodes = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ShowNodesDescription = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayNodalSupports = false;

            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ShowGridLines = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ShowSectionSymbols = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].ShowDetailSymbols = false;

            // Do dokumentu exporujeme aj s wireframe
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayWireFrameModel = true; //default treba mat false, lebo to robi len problemy a wireframe budeme povolovat len tam kde ho naozaj aj chceme
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].WireFrameLineThickness = 2;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].TransformScreenLines3DToCylinders3D = false;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayMembersWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayJointsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayPlatesWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayConnectorsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].WireframeColorIndex = CComboBoxHelper.GetColorIndex(Colors.Black);

            // Foundations
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayFoundations = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayReinforcementBars = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayFoundationsWireFrame = true;
            DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].DisplayReinforcementBarsWireFrame = true;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].RotateModelX = -80;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].RotateModelY = 45;
            // TODO 701 - zakomentovane DisplayOptionsList[(int)EDisplayOptionsTypes.Layouts_Foundations].RotateModelZ = 5;
            #endregion FOUNDATIONS
            #endregion LAYOUTS
        }
    }
}