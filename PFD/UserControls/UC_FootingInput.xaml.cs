﻿using BaseClasses;
using BaseClasses.Helpers;
using DATABASE.DTO;
using MATH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_FootingInput.xaml
    /// </summary>
    public partial class UC_FootingInput : UserControl
    {
        DisplayOptions sDisplayOptions;
        CPFDViewModel _pfdVM;
        CFootingInputVM vm;
        double Frame2DWidth;
        double Frame2DHeight;
        
        public UC_FootingInput(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;
            _pfdVM.PropertyChanged += _pfdVM_PropertyChanged;

            vm = new CFootingInputVM(pfdVM);
            vm.PropertyChanged += HandleFootingPadPropertyChangedEvent;
            this.DataContext = vm;
            vm.FootingPadMemberTypeIndex = 0;
        }

        private void _pfdVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is CComponentInfo)
            {
                if (e.PropertyName == "Generate")
                {
                    CComponentInfo ci = sender as CComponentInfo;
                    if (ci.MemberTypePosition == EMemberType_FS_Position.WindPostFrontSide || ci.MemberTypePosition == EMemberType_FS_Position.WindPostBackSide)
                    {
                        vm.SetFootingPadMemberTypes();                        
                    }
                }
                else return;
            }

            if (!(sender is CPFDViewModel)) return;
            CPFDViewModel pfdVM = sender as CPFDViewModel;
            if (pfdVM.IsSetFromCode) return;

            CFoundation pad = vm.GetSelectedFootingPad();
            CConnectionJointTypes joint = vm.GetBaseJointForSelectedNode(pad.m_Node);
            if (joint == null) return; // Ak je to foundation pad a nebol najdeny odpovedajuci joint tak je to nevalidne

            CSlab floorSlab = vm.GetFloorSlab();

            displayFootingPad(pad, joint, floorSlab);
        }

        protected void HandleFootingPadPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CFootingInputVM vm = sender as CFootingInputVM;
            if (vm != null && vm.IsSetFromCode) return;

            //// TODO - dopracovat dalsie parametre, ktore ovplyvnia preview
            if (e.PropertyName == "FootingPadSize_x_Or_a" ||
                e.PropertyName == "FootingPadSize_y_Or_b" ||
                e.PropertyName == "FootingPadSize_z_Or_h" ||

                e.PropertyName == "Eccentricity_ex_abs" ||
                e.PropertyName == "Eccentricity_ey_abs" ||

                e.PropertyName == "UseStraightReinforcementBars" ||

                e.PropertyName == "LongReinTop_x_No" ||
                e.PropertyName == "LongReinTop_x_Phi" ||
                e.PropertyName == "LongReinTop_x_distance_s_y" ||
                e.PropertyName == "LongReinTop_x_ColorIndex" ||

                e.PropertyName == "LongReinTop_y_No" ||
                e.PropertyName == "LongReinTop_y_Phi" ||
                e.PropertyName == "LongReinTop_y_distance_s_x" ||
                e.PropertyName == "LongReinTop_y_ColorIndex" ||

                e.PropertyName == "LongReinBottom_x_No" ||
                e.PropertyName == "LongReinBottom_x_Phi" ||
                e.PropertyName == "LongReinBottom_x_distance_s_y" ||
                e.PropertyName == "LongReinBottom_x_ColorIndex" ||

                e.PropertyName == "LongReinBottom_y_No" ||
                e.PropertyName == "LongReinBottom_y_Phi" ||
                e.PropertyName == "LongReinBottom_y_distance_s_x" ||
                e.PropertyName == "LongReinBottom_y_ColorIndex" ||

                e.PropertyName == "Eccentricity_ex" ||
                e.PropertyName == "Eccentricity_ey" ||

                e.PropertyName == "ConcreteCover" ||

                // Floor
                e.PropertyName == "FloorSlabThickness" ||
                e.PropertyName == "MeshConcreteCover" ||
                e.PropertyName == "ReinforcementMeshGrade" ||

                e.PropertyName == "NumberOfSawCutsInDirectionX" ||
                e.PropertyName == "NumberOfSawCutsInDirectionY" ||
                e.PropertyName == "FirstSawCutPositionInDirectionX" ||
                e.PropertyName == "FirstSawCutPositionInDirectionY" ||
                e.PropertyName == "SawCutsSpacingInDirectionX" ||
                e.PropertyName == "SawCutsSpacingInDirectionY" ||
                e.PropertyName == "CutWidth" ||
                e.PropertyName == "CutDepth" ||

                e.PropertyName == "NumberOfControlJointsInDirectionX" ||
                e.PropertyName == "NumberOfControlJointsInDirectionY" ||
                e.PropertyName == "FirstControlJointPositionInDirectionX" ||
                e.PropertyName == "FirstControlJointPositionInDirectionY" ||
                e.PropertyName == "ControlJointsSpacingInDirectionX" ||
                e.PropertyName == "ControlJointsSpacingInDirectionY" ||

                e.PropertyName == "DowelDiameter" ||
                e.PropertyName == "DowelLength" ||
                e.PropertyName == "DowelSpacing" ||

                e.PropertyName == "PerimeterDepth_LRSide" ||
                e.PropertyName == "PerimeterWidth_LRSide" ||
                e.PropertyName == "StartersLapLength_LRSide" ||
                e.PropertyName == "StartersSpacing_LRSide" ||
                e.PropertyName == "Starters_Phi_LRSide" ||
                e.PropertyName == "RebateWidth_LRSide" ||
                e.PropertyName == "Longitud_Reinf_TopAndBotom_Phi_LRSide" ||
                e.PropertyName == "Longitud_Reinf_Intermediate_Phi_LRSide" ||
                e.PropertyName == "Longitud_Reinf_Intermediate_Count_LRSide" ||

                e.PropertyName == "RebateWidth_LRSide" ||

                e.PropertyName == "PerimeterDepth_FBSide" ||
                e.PropertyName == "PerimeterWidth_FBSide" ||
                e.PropertyName == "StartersLapLength_FBSide" ||
                e.PropertyName == "StartersSpacing_FBSide" ||
                e.PropertyName == "Starters_Phi_FBSide" ||
                e.PropertyName == "RebateWidth_FBSide" ||
                e.PropertyName == "Longitud_Reinf_TopAndBotom_Phi_FBSide" ||
                e.PropertyName == "Longitud_Reinf_Intermediate_Phi_FBSide" ||
                e.PropertyName == "Longitud_Reinf_Intermediate_Count_FBSide" ||

                e.PropertyName == "RebateWidth_FBSide" ||

                //pridavane dalsie parametre pre Footing, aby sa vymazali vysledky a aktivovalo tlacidlo Calculate
                e.PropertyName == "ConcreteGrade" ||
                e.PropertyName == "AggregateSize" ||
                e.PropertyName == "ConcreteDensity" ||
                e.PropertyName == "ReinforcementGrade" ||                
                e.PropertyName == "SoilReductionFactor_Phi" ||
                e.PropertyName == "SoilReductionFactorEQ_Phi" ||
                e.PropertyName == "SoilBearingCapacity" 
                )
            {
                UpdateModelProperties();
                _pfdVM.FootingChanged = true;
            }
            CFoundation pad = vm.GetSelectedFootingPad();
            CConnectionJointTypes joint = vm.GetBaseJointForSelectedNode(pad.m_Node);
            CSlab floorSlab = vm.GetFloorSlab();

            displayFootingPad(pad, joint, floorSlab);
        }

        private void UpdateModelProperties()
        {
            if (_pfdVM.Model.m_arrSlabs != null)
            {
                _pfdVM.Model.m_arrSlabs.First().m_fDim3 = vm.FloorSlabThickness / 1000f;
                _pfdVM.Model.m_arrSlabs.First().MeshGradeName = vm.ReinforcementMeshGrade;
                _pfdVM.Model.m_arrSlabs.First().ConcreteCover = vm.MeshConcreteCover / 1000f;

                _pfdVM.Model.m_arrSlabs.First().NumberOfSawCutsInDirectionX = vm.NumberOfSawCutsInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().NumberOfSawCutsInDirectionY = vm.NumberOfSawCutsInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().FirstSawCutPositionInDirectionX = vm.FirstSawCutPositionInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().FirstSawCutPositionInDirectionY = vm.FirstSawCutPositionInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().SawCutsSpacingInDirectionX = vm.SawCutsSpacingInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().SawCutsSpacingInDirectionY = vm.SawCutsSpacingInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().ReferenceSawCut.CutWidth = vm.CutWidth / 1000f;
                _pfdVM.Model.m_arrSlabs.First().ReferenceSawCut.CutDepth = vm.CutDepth / 1000f;

                _pfdVM.Model.m_arrSlabs.First().NumberOfControlJointsInDirectionX = vm.NumberOfControlJointsInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().NumberOfControlJointsInDirectionY = vm.NumberOfControlJointsInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().FirstControlJointPositionInDirectionX = vm.FirstControlJointPositionInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().FirstControlJointPositionInDirectionY = vm.FirstControlJointPositionInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().ControlJointsSpacingInDirectionX = vm.ControlJointsSpacingInDirectionX;
                _pfdVM.Model.m_arrSlabs.First().ControlJointsSpacingInDirectionY = vm.ControlJointsSpacingInDirectionY;
                _pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.ReferenceDowel.Diameter_shank = float.Parse(vm.DowelDiameter) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.ReferenceDowel.Length = vm.DowelLength / 1000f;
                _pfdVM.Model.m_arrSlabs.First().ReferenceControlJoint.DowelSpacing = vm.DowelSpacing / 1000f;

                _pfdVM.Model.m_arrSlabs.First().PerimeterDepth_LRSide = vm.PerimeterDepth_LRSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterWidth_LRSide = vm.PerimeterWidth_LRSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().StartersLapLength_LRSide = vm.StartersLapLength_LRSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().StartersSpacing_LRSide = vm.StartersSpacing_LRSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Starters_Phi_LRSide = float.Parse(vm.Starters_Phi_LRSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_TopAndBotom_Phi_LRSide = float.Parse(vm.Longitud_Reinf_TopAndBotom_Phi_LRSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Phi_LRSide = float.Parse(vm.Longitud_Reinf_Intermediate_Phi_LRSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Count_LRSide = vm.Longitud_Reinf_Intermediate_Count_LRSide;

                _pfdVM.Model.m_arrSlabs.First().RebateWidth_LRSide = vm.RebateWidth_LRSide / 1000f;

                _pfdVM.Model.m_arrSlabs.First().PerimeterDepth_FBSide = vm.PerimeterDepth_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterWidth_FBSide = vm.PerimeterWidth_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().StartersLapLength_FBSide = vm.StartersLapLength_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().StartersSpacing_FBSide = vm.StartersSpacing_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Starters_Phi_FBSide = float.Parse(vm.Starters_Phi_FBSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_TopAndBotom_Phi_FBSide = float.Parse(vm.Longitud_Reinf_TopAndBotom_Phi_FBSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Phi_FBSide = float.Parse(vm.Longitud_Reinf_Intermediate_Phi_FBSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Count_FBSide = vm.Longitud_Reinf_Intermediate_Count_FBSide;

                _pfdVM.Model.m_arrSlabs.First().RebateWidth_FBSide = vm.RebateWidth_FBSide / 1000f;

                // TOTO ASI NIE JE POTREBNE, VYTVORIME PERIMETERS NANOVO PODLA VYSSIE NASTAVENYCH HODNOT, ALE NECHAVAM TO ZATIAL TU, MOZNO SA TO ESTE BUDE HODIT
                // Perimeters
                // TODO - nemuseli by sa pouzivat indexy, ale dalo by sa vyhladavat left, right, front, back podla 
                // parametra m_BuildingSide v objekte CSlabPerimeter
                /*
                // Index 0 / first - lava strana
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().PerimeterDepth = vm.PerimeterDepth_LRSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().PerimeterWidth = vm.PerimeterWidth_LRSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().StartersLapLength = vm.StartersLapLength_LRSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().StartersSpacing = vm.StartersSpacing_LRSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().Starters_Phi = float.Parse(vm.Starters_Phi_LRSide) / 1000f;

                // Len ak existuju roller doors, resp. rebate na lavej alebo pravej strane floor slab
                if (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().SlabRebates != null)
                    _pfdVM.Model.m_arrSlabs.First().PerimeterBeams.First().SlabRebates.First().RebateWidth = vm.RebateWidth_LRSide / 1000f;
                else if (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[1].SlabRebates != null)
                    _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[1].SlabRebates.First().RebateWidth = vm.RebateWidth_LRSide / 1000f;
                else
                {
                    _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[0].SlabRebates = null;
                    _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[1].SlabRebates = null;
                }

                // Index 2 - predna strana
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].PerimeterDepth = vm.PerimeterDepth_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].PerimeterWidth = vm.PerimeterWidth_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].StartersLapLength = vm.StartersLapLength_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].StartersSpacing = vm.StartersSpacing_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].Starters_Phi = float.Parse(vm.Starters_Phi_FBSide) / 1000f;

                // Len ak existuju roller doors, resp. rebate na prednej alebo zadnej strane floor slab
                if (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].SlabRebates != null)
                    _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].SlabRebates.First().RebateWidth = vm.RebateWidth_FBSide / 1000f;
                else if (_pfdVM.Model.m_arrSlabs.First().PerimeterBeams[3].SlabRebates != null)
                    _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[3].SlabRebates.First().RebateWidth = vm.RebateWidth_FBSide / 1000f;
                else
                {
                    _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[2].SlabRebates = null;
                    _pfdVM.Model.m_arrSlabs.First().PerimeterBeams[3].SlabRebates = null;
                }
                */

                // Update floor slab - methods
                _pfdVM.Model.m_arrSlabs.First().SetControlPoint();
                _pfdVM.Model.m_arrSlabs.First().SetTextPoint();
                _pfdVM.Model.m_arrSlabs.First().CreateSawCuts();
                _pfdVM.Model.m_arrSlabs.First().CreateControlJoints();
                _pfdVM.Model.m_arrSlabs.First().CreateMesh();
                _pfdVM.Model.m_arrSlabs.First().CreatePerimeters();
                _pfdVM.Model.m_arrSlabs.First().SetDescriptionText();

                //TODO Mato - je potrebne updatovat property v Modeli ak sa zmenia property vo view modeli v GUI (vid vyssie)
            }
        }

        private void FrameFootingPadPreview3D_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private Page3Dmodel GetFootingPad3DPreview(CFoundation pad, CConnectionJointTypes joint, bool bDisplayJointComponents = true)
        {
            // TO Ondrej - tu potrebujeme refaktorovat cast funkcie displayJoint a Drawing3D.GetJointPreviewModel
            // Mala by sa vykreslit patka (betonovy kvader prisluchajuci k uzlu) a v pripade ze je bool bDisplayJointComponents == true tak aj plech a cast pruta ktore su v danom uzle k zakladu pripojene
            // Joint je samozrejme potrebne naklonovat
            // Asi aj patku je potrebne naklonovat, nech to funguje rovnako
            // Vykreslenie vyztuze podla projektu AAC

            if (pad == null) return null; // Error - nothing to display

            sDisplayOptions = _pfdVM.GetDisplayOptions(EDisplayOptionsTypes.GUI_Foundation_Preview);
            
            ////Here is the place to overwrite displayOptions from Main Model
            //// TODO - refaktorovat s nastavenim zobrazenia joints preview
            //sDisplayOptions.bDisplayGlobalAxis = false;
            //sDisplayOptions.bDisplayMemberDescription = false;
            //sDisplayOptions.bDisplayNodes = false;
            //sDisplayOptions.bDisplayNodesDescription = false;

            //sDisplayOptions.bDisplaySolidModel = true;
            //sDisplayOptions.bDisplayMembers = true;
            //sDisplayOptions.bDisplayJoints = true;
            //sDisplayOptions.bDisplayPlates = true;
            //sDisplayOptions.bDisplayConnectors = true;

            //// Foundations
            //sDisplayOptions.bDisplayFoundations = true;
            //sDisplayOptions.bDisplayReinforcementBars = true;

            //sDisplayOptions.CO_RotateModelX = -80;
            //sDisplayOptions.CO_RotateModelY = 45;
            //sDisplayOptions.CO_RotateModelZ = 5;

            CModel padModel = Drawing3D.GetJointPreviewModel(joint, pad, ref sDisplayOptions);

            return new Page3Dmodel(padModel, sDisplayOptions, EModelType.eFooting);
        }

        private Canvas GetFootingPad2DPreview(CFoundation pad, CConnectionJointTypes joint, CSlab floorSlab, DisplayOptionsFootingPad2D opts2D)
        {
            Canvas page = new Canvas();

            if (pad == null || joint == null) return page;

            // TO Ondrej - toto je funkcia ktorou kreslime plech 
            // podobne by malo byt pre kreslenie detailu patky
            // bool hodnoty su zatial vo vnutri funkcie DrawFootingPadSideElevationToCanvas

            /*
            Drawing2D.DrawPlateToCanvas(joint.m_arrPlates.FirstOrDefault(),
               Frame2DWidth,
               Frame2DHeight,
               ref page,
             true,//  vm.DrawPoints2D,
             true,//  vm.DrawOutLine2D,
             true,//  vm.DrawPointNumbers2D,
             true,//  vm.DrawHoles2D,
             true,//  vm.DrawHoleCentreSymbol2D,
             true,//  vm.DrawDrillingRoute2D,
             true,//  vm.DrawDimensions2D,
             true,//  vm.DrawMemberOutline2D,
             true);//  vm.DrawBendLines2D);
             */

            Drawing2D.DrawFootingPadSideElevationToCanvas(pad, joint, floorSlab, Frame2DWidth, Frame2DHeight, ref page, opts2D);

            return page;
        }

        private void displayFootingPad(CFoundation pad, CConnectionJointTypes joint, CSlab floorSlab)
        {
            CFootingInputVM vm = this.DataContext as CFootingInputVM;

            SetFrame2DSize();

            // TO Ondrej - tu je problem ze zistujem velkost frame, ale vtedy su este jeho parametre NaN alebo 0 :)
            // Asi to treba trosku preusporiadat

            if (MathF.d_equal(Frame2D.ActualWidth, 0) || MathF.d_equal(Frame2D.ActualHeight, 0))
            {
                Frame2DWidth = 579; // TO Ondrej - zatial nastavujem natvrdo
                Frame2DHeight = 397; // TO Ondrej - zatial nastavujem natvrdo
            }

            // ??? TO Ondrej - dal som tu takuto validaciu pre pripad ze sa nenastavia dobre rozmery frame alebo su neinicializovane
            if (double.IsNaN(Frame2DWidth) || double.IsNaN(Frame2DHeight) || Frame2DWidth <= 0 || Frame2DHeight <= 0)
            {
                throw new ArgumentNullException("Error. Invalid size of canvas frame.");
            }

            DisplayOptionsFootingPad2D opts2D = DisplayOptionsHelper.GetDefault();
            // Create 2D page
            Canvas page2D = GetFootingPad2DPreview(pad, joint, floorSlab, opts2D);

            // Display plate in 2D preview frame
            Frame2D.Content = page2D;

            // Create 3D window
            Page3Dmodel page3D = GetFootingPad3DPreview(pad, joint);

            // Display model in 3D preview frame
            FrameFootingPadPreview3D.Content = page3D;

            this.UpdateLayout();
        }

        private void SetFrame2DSize()
        {
            Frame2DWidth = Frame2D.ActualWidth;
            Frame2DHeight = Frame2D.ActualHeight;
            // Nenastavovat z maximalnych rozmerov screen, ale z aktualnych rozmerov okna

            // TODO Ondrej - prevzate zo System Component Viewer - nastavit konstanty pre toto okno
            if (Frame2DWidth == 0) Frame2DWidth = this.Width - 669; // SystemParameters.PrimaryScreenWidth / 2 - 15;
            if (Frame2DHeight == 0) Frame2DHeight = this.Height - 116; // SystemParameters.PrimaryScreenHeight - 145;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Redraw();            
        }

        public void Redraw()
        {
            CFoundation pad = vm.GetSelectedFootingPad();
            CConnectionJointTypes joint = vm.GetBaseJointForSelectedNode(pad.m_Node);
            CSlab floorSlab = vm.GetFloorSlab();

            displayFootingPad(pad, joint, floorSlab);
        }
    }
}