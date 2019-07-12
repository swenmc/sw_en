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

            CFoundation pad = GetSelectedFootingPad();
            CConnectionJointTypes joint = GetBaseJointForSelectedNode(pad.m_Node);
            displayFootingPad(pad, joint);
        }

        private void _pfdVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //ja neviem co sa to tu robi...

            //if (!(sender is CPFDViewModel)) return;
            //CFoundation pad = GetSelectedFootingPad(); // TO DO Ondrej - dopracovat a napojit objekty pad a joint ako parametre funkcie
            //CConnectionJointTypes joint = GetBaseJointForSelectedNode(pad.m_Node); // Napojit objekt joint ktory prislucha k danemu typu patky

            //// Joint with base plate and anchors
            //if (joint != null && joint.m_arrPlates != null && joint.m_arrPlates[0] is CConCom_Plate_B_basic)
            //{
            //    CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)joint.m_arrPlates[0];
            //    float feccentricity_x = 0;
            //    float feccentricity_y = 0;
            //    float fpad_x = pad.m_fDim1;
            //    float fpad_y = pad.m_fDim2;

            //    float fx_plateEdge_to_pad = 0.5f * (fpad_x - basePlate.Fb_X) + feccentricity_x;
            //    float fy_plateEdge_to_pad = 0.5f * (fpad_y - basePlate.Fh_Y) + feccentricity_y;

            //    float fx_minus_plateEdge_to_pad = fx_plateEdge_to_pad;
            //    float fy_minus_plateEdge_to_pad = fy_plateEdge_to_pad;
            //    float fx_plus_plateEdge_to_pad = fpad_x - fx_plateEdge_to_pad - basePlate.Fb_X;
            //    float fy_plus_plateEdge_to_pad = fpad_y - fy_plateEdge_to_pad - basePlate.Fh_Y;

            //    float fx_min_plateEdge_to_pad = Math.Min(fx_minus_plateEdge_to_pad, fx_plus_plateEdge_to_pad);
            //    float fy_min_plateEdge_to_pad = Math.Max(fy_minus_plateEdge_to_pad, fy_plus_plateEdge_to_pad);

            //    basePlate.AnchorArrangement.SetEdgeDistances(basePlate, pad, fx_plateEdge_to_pad, fy_plateEdge_to_pad);
            //}

            //displayFootingPad(pad, joint);
        }

        protected void HandleFootingPadPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CFootingInputVM vm = sender as CFootingInputVM;
            if (vm != null && vm.IsSetFromCode) return;

            //// TODO - dopracovat dalsie parametre, ktore ovplyvnia preview
            //if (e.PropertyName == "FootingPadSize_x_Or_a" ||
            //    e.PropertyName == "FootingPadSize_y_Or_b" ||
            //    e.PropertyName == "FootingPadSize_z_Or_h"
            //    )
            //{
            //    CFoundation pad = GetSelectedFootingPad();
            //    CConnectionJointTypes joint = GetBaseJointForSelectedNode(pad.m_Node);
            //    displayFootingPad(pad, joint);
            //}
            CFoundation pad = GetSelectedFootingPad();
            CConnectionJointTypes joint = GetBaseJointForSelectedNode(pad.m_Node);
            displayFootingPad(pad, joint);
        }

        private CFoundation GetSelectedFootingPad()
        {
            // Select type of footing pads that match with selected footing pad of member type in GUI
            List<CFoundation> listOfSelectedTypePads = new List<CFoundation>(); //all pads in list should be the same!
            EMemberType_FS_Position memberType;
            if (vm.FootingPadMemberTypeIndex == 0) // TODO - porovnavam s indexom v comboboxe 0-3, asi by bolo istejsie zobrazovat v comboboxe items naviazane na EMemberType_FS_Position, aby sa to neznicilo ked co comboboxu pridam nejaky dalsi typ alebo zmenim poradie
                memberType = EMemberType_FS_Position.MainColumn;
            else if (vm.FootingPadMemberTypeIndex == 1)
                memberType = EMemberType_FS_Position.EdgeColumn;
            else if (vm.FootingPadMemberTypeIndex == 2)
                memberType = EMemberType_FS_Position.ColumnFrontSide;
            else if (vm.FootingPadMemberTypeIndex == 3)
                memberType = EMemberType_FS_Position.ColumnBackSide;
            else
            {
                throw new Exception("Not defined member type!");
            }

            for (int i = 0; i < _pfdVM.Model.m_arrFoundations.Count; i++)
            {
                if (memberType == _pfdVM.Model.m_arrFoundations[i].m_ColumnMemberTypePosition)
                    listOfSelectedTypePads.Add(_pfdVM.Model.m_arrFoundations[i]);
            }

            // All pads in list should be the same!
            CFoundation pad = listOfSelectedTypePads.FirstOrDefault();

            return pad;
        }

        private CConnectionJointTypes GetBaseJointForSelectedNode(CNode node)
        {
            // Vrati spoj typu base plate pre uzol selektovanej patky

            for (int i = 0; i < _pfdVM.Model.m_arrConnectionJoints.Count; i++)
            {
                if (node == _pfdVM.Model.m_arrConnectionJoints[i].m_Node && _pfdVM.Model.m_arrConnectionJoints[i].m_arrPlates[0] is CConCom_Plate_B_basic)
                {
                    return _pfdVM.Model.m_arrConnectionJoints[i];
                }
            }

            return null; // Error - joint wasn't found
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
            sDisplayOptions.bDisplayGlobalAxis = true;
            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayPlates = true;
            sDisplayOptions.bDisplayConnectors = true;
            sDisplayOptions.bDisplayJoints = true;
            sDisplayOptions.RotateModelX = -90;
            sDisplayOptions.RotateModelY = 45;

            // TODO - Vyrobit model patky + vyztuz + plech spoja a cast pruta spoja
            // Refaktorovat funkciu displayJoint(CConnectionJointTypes joint) aby bolo mozne vykreslit joint v Table UC_Joints aj UC_FootingInput
            // TODO - Pridat do modelu spoj (uzol spoja, uzly prutov, pruty, plechy, skrutky)

            CModel padModel = Drawing3D.GetJointPreviewModel(joint, pad, ref sDisplayOptions);

            Page3Dmodel page1 = new Page3Dmodel(padModel, sDisplayOptions, EModelType.eFooting);

            // Display model in 3D preview frame
            FrameFootingPadPreview3D.Content = page1;
            FrameFootingPadPreview3D.UpdateLayout();
        }
    }
}