using BaseClasses;
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

        public UC_FootingInput(CPFDViewModel pfdVM/*, CJointsVM jointsVM*/)
        {
            InitializeComponent();

            _pfdVM = pfdVM;
            _pfdVM.PropertyChanged += _pfdVM_PropertyChanged;

            vm = new CFootingInputVM(pfdVM);
            vm.PropertyChanged += HandleFootingPadPropertyChangedEvent;
            this.DataContext = vm;
            vm.FootingPadMemberTypeIndex = 0;

            CFoundation pad = vm.GetSelectedFootingPad();
            CConnectionJointTypes joint = vm.GetBaseJointForSelectedNode(pad.m_Node);
            displayFootingPad(pad, joint);
        }

        private void _pfdVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is CPFDViewModel)) return;
            CFoundation pad = vm.GetSelectedFootingPad(); 
            CConnectionJointTypes joint = vm.GetBaseJointForSelectedNode(pad.m_Node);

            //Toto Mato trosku nerozumiem,ze naco tu taketo vypocty tu su. Resp. ci naozaj musia byt...
            // Joint with base plate and anchors
            if (joint != null && joint.m_arrPlates != null && joint.m_arrPlates[0] is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)joint.m_arrPlates[0];
                float feccentricity_x = 0;
                float feccentricity_y = 0;
                float fpad_x = pad.m_fDim1;
                float fpad_y = pad.m_fDim2;

                float fx_plateEdge_to_pad = 0.5f * (fpad_x - basePlate.Fb_X) + feccentricity_x;
                float fy_plateEdge_to_pad = 0.5f * (fpad_y - basePlate.Fh_Y) + feccentricity_y;

                float fx_minus_plateEdge_to_pad = fx_plateEdge_to_pad;
                float fy_minus_plateEdge_to_pad = fy_plateEdge_to_pad;
                float fx_plus_plateEdge_to_pad = fpad_x - fx_plateEdge_to_pad - basePlate.Fb_X;
                float fy_plus_plateEdge_to_pad = fpad_y - fy_plateEdge_to_pad - basePlate.Fh_Y;

                float fx_min_plateEdge_to_pad = Math.Min(fx_minus_plateEdge_to_pad, fx_plus_plateEdge_to_pad);
                float fy_min_plateEdge_to_pad = Math.Max(fy_minus_plateEdge_to_pad, fy_plus_plateEdge_to_pad);

                basePlate.AnchorArrangement.SetEdgeDistances(basePlate, pad, fx_plateEdge_to_pad, fy_plateEdge_to_pad);
            }

            displayFootingPad(pad, joint);
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

                e.PropertyName == "PerimeterDepth_FBSide" ||
                e.PropertyName == "PerimeterWidth_FBSide" ||
                e.PropertyName == "StartersLapLength_FBSide" ||
                e.PropertyName == "StartersSpacing_FBSide" ||
                e.PropertyName == "Starters_Phi_FBSide" ||
                e.PropertyName == "RebateWidth_FBSide" ||
                e.PropertyName == "Longitud_Reinf_TopAndBotom_Phi_FBSide" ||
                e.PropertyName == "Longitud_Reinf_Intermediate_Phi_FBSide" ||
                e.PropertyName == "Longitud_Reinf_Intermediate_Count_FBSide"
                )
            {
                UpdateModelProperties();
                _pfdVM.FootingChanged = true;
            }
            CFoundation pad = vm.GetSelectedFootingPad();
            CConnectionJointTypes joint = vm.GetBaseJointForSelectedNode(pad.m_Node);
            displayFootingPad(pad, joint);
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

                _pfdVM.Model.m_arrSlabs.First().PerimeterDepth_FBSide = vm.PerimeterDepth_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().PerimeterWidth_FBSide = vm.PerimeterWidth_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().StartersLapLength_FBSide = vm.StartersLapLength_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().StartersSpacing_FBSide = vm.StartersSpacing_FBSide / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Starters_Phi_FBSide = float.Parse(vm.Starters_Phi_FBSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_TopAndBotom_Phi_FBSide = float.Parse(vm.Longitud_Reinf_TopAndBotom_Phi_FBSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Phi_FBSide = float.Parse(vm.Longitud_Reinf_Intermediate_Phi_FBSide) / 1000f;
                _pfdVM.Model.m_arrSlabs.First().Longitud_Reinf_Intermediate_Count_FBSide = vm.Longitud_Reinf_Intermediate_Count_FBSide;

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
                _pfdVM.Model.m_arrSlabs.First().CreatePerimeters();
                _pfdVM.Model.m_arrSlabs.First().SetDescriptionText();

                //TODO Mato - je potrebne updatovat property v Modeli ak sa zmenia property vo view modeli v GUI (vid vyssie)
            }
        }

        private void FrameFootingPadPreview3D_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void displayFootingPad(CFoundation pad, CConnectionJointTypes joint, bool bDisplayJointComponents = true)
        {
            // TO Ondrej - tu potrebujeme refaktorovat cast funkcie displayJoint a Drawing3D.GetJointPreviewModel
            // Mala by sa vykreslit patka (betonovy kvader prisluchajuci k uzlu) a v pripade ze je bool bDisplayJointComponents == true tak aj plech a cast pruta ktore su v danom uzle k zakladu pripojene
            // Joint je samozrejme potrebne naklonovat
            // Asi aj patku je potrebne naklonovat, nech to funguje rovnako
            // Vykreslenie vyztuze podla projektu AAC

            if (pad == null) return; // Error - nothing to display

            sDisplayOptions = _pfdVM.GetDisplayOptions();
            //Here is the place to overwrite displayOptions from Main Model
            // TODO - refaktorovat s nastavenim zobrazenia joints preview
            sDisplayOptions.bDisplayGlobalAxis = false;
            sDisplayOptions.bDisplayMemberDescription = false;

            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayMembers = true;
            sDisplayOptions.bDisplayJoints = true;
            sDisplayOptions.bDisplayPlates = true;
            sDisplayOptions.bDisplayConnectors = true;

            // Foundations
            sDisplayOptions.bDisplayFoundations = true;
            sDisplayOptions.bDisplayReinforcementBars = true;

            sDisplayOptions.RotateModelX = -80;
            sDisplayOptions.RotateModelY = 45;
            sDisplayOptions.RotateModelZ = 5;

            CModel padModel = Drawing3D.GetJointPreviewModel(joint, pad, ref sDisplayOptions);

            Page3Dmodel page1 = new Page3Dmodel(padModel, sDisplayOptions, EModelType.eFooting);

            // Display model in 3D preview frame
            FrameFootingPadPreview3D.Content = page1;
            FrameFootingPadPreview3D.UpdateLayout();
        }
    }
}