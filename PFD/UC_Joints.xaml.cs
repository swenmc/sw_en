using BaseClasses;
using BaseClasses.Helpers;
using DATABASE;
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
    /// Interaction logic for UC_Joints.xaml
    /// </summary>
    public partial class UC_Joints : UserControl
    {
        DisplayOptions sDisplayOptions;
        CPFDViewModel _pfdVM;
        CJointsVM vm;
        Dictionary<int, List<CConnectionJointTypes>> jointsDict;
        List<CConnectionJointTypes> list_joints;

        bool paramsChanged = false;

        public UC_Joints(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;
            _pfdVM.PropertyChanged += _pfdVM_PropertyChanged;

            vm = new CJointsVM();
            vm.PropertyChanged += HandleJointsPropertyChangedEvent;
            this.DataContext = vm;

            ArrangeConnectionJoints();
            //DebugJoints();
            vm.JointTypeIndex = 1;


            //vsetok kod dole su len pokusy nastavit hned po loade aplikace v tabe Joints prvy tab na aktivny, lebo sa mi zda ako disablovany...nic s tym neviem spravit
            //JointsTabControl.SelectedIndex = 0;
            //JointsTabControl.SelectedItem = vm.TabItems[vm.SelectedTabIndex];
            //vm.TabItems[vm.SelectedTabIndex].Focus();
            //UpdateLayout();

            //this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            //{
            //    JointsTabControl.SelectedIndex = 0;
            //    //JointsTabControl.SelectedItem = JointsTabControl.Items[0];
            //    JointsTabControl.Focus();
            //    // Do something to update my gui
            //}));

        }

        private void _pfdVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is CPFDViewModel)) return;

            if (_pfdVM.IsSetFromCode) return;

            //To Mato - tu by mozno trebalo pridat premenne pri zmene ktorych je nutne prekreslit Joint preview

            if (e.PropertyName == "ModelIndex") return;

            CConnectionJointTypes joint = GetSelectedJoint();
            displayJoint(joint);
        }

        protected void HandleJointsPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CJointsVM vm = sender as CJointsVM;
            if (vm != null && vm.IsSetFromCode) return;

            if (e.PropertyName == "TabItems") return;
            if (e.PropertyName == "SelectedTabIndex") return;
            if (e.PropertyName == "JointTypeIndex") SetDynamicTabs(vm);

            if (e.PropertyName == "ChangedAnchorArrangementParameter" || e.PropertyName == "ChangedScrewArrangementParameter" || e.PropertyName == "ChangedGeometryParameter")
            {
                CConnectionJointTypes joint = GetSelectedJoint();
                displayJoint(joint);
            }
        }

        private void DebugJoints()
        {
            int count = 0;
            foreach (CConnectionJointTypes joint in _pfdVM.Model.m_arrConnectionJoints)
            {
                count++;
                if (joint.m_SecondaryMembers != null) System.Diagnostics.Trace.WriteLine($"{count}. {joint.GetType()} {joint.m_MainMember.EMemberTypePosition} {joint.m_SecondaryMembers.Count()} {joint.m_SecondaryMembers[0].EMemberTypePosition}");
                else System.Diagnostics.Trace.WriteLine($"{count}. {joint.GetType()} {joint.m_MainMember.EMemberTypePosition}");
            }
        }
        public void ArrangeConnectionJoints()
        {
            jointsDict = new Dictionary<int, List<CConnectionJointTypes>>();
            vm.DictJoints = new Dictionary<CConnectionDescription, CConnectionJointTypes>();
            List<CConnectionDescription> listJointTypes = new List<CConnectionDescription>();
            foreach (CConnectionDescription c in vm.AllJointTypes)
            {
                List<CConnectionJointTypes> listFoundJointTypes = GetConnectionJointTypesFor(c);
                jointsDict.Add(c.ID, listFoundJointTypes);
                if (listFoundJointTypes.Count > 0)
                {
                    listJointTypes.Add(c);
                    vm.DictJoints.Add(c, listFoundJointTypes.FirstOrDefault());
                }
            }

            //validacia podla tasku 323
            ValidateModelJointsCounts();

            int selectedIndex = vm.JointTypeIndex;
            vm.JointTypes = listJointTypes; //task 324
            if (vm.JointTypes.Count > selectedIndex) vm.JointTypeIndex = selectedIndex;
            else vm.JointTypeIndex = 0;
        }

        private void ValidateModelConnectionJoints()
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Validation
            // Ucel validacie - Skontrolovat ci maju vsetky pruty v spojoch priradeny EMemberTypePosition
            foreach (CConnectionJointTypes joint in _pfdVM.Model.m_arrConnectionJoints)
            {
                if (joint.m_MainMember == null)
                {
                    if (_pfdVM.debugging)
                        new ArgumentNullException("Undefined main member of joint ID:" + joint.ID);
                }

                if (joint.m_MainMember.EMemberTypePosition <= 0)
                {
                    if (_pfdVM.debugging)
                        new ArgumentNullException("Undefined main member type of joint ID: " + joint.ID + ", member ID: " + joint.m_MainMember.ID);
                }

                if (joint.m_SecondaryMembers != null && joint.m_SecondaryMembers.Length > 0)
                {
                    for (int i = 0; i < joint.m_SecondaryMembers.Length; i++)
                    {
                        if (joint.m_SecondaryMembers[i].EMemberTypePosition <= 0 && _pfdVM.debugging)
                            new ArgumentNullException("Undefined secondary member type of joint ID: " + joint.ID + ", member ID: " + joint.m_SecondaryMembers[i].ID);
                    }
                }
            }

        }
        private void ValidateModelJointsCounts()
        {
            int modelJointsCount = _pfdVM.Model.m_arrConnectionJoints.Count;

            int jointsIdentified = 0;
            foreach (List<CConnectionJointTypes> list_joints in jointsDict.Values)
            {
                if (list_joints == null) continue;
                jointsIdentified += list_joints.Count;
            }

            if (modelJointsCount != jointsIdentified && _pfdVM.debugging)
            {
                System.Diagnostics.Trace.WriteLine($"JOINTS VALIDATION ERROR: Not all joints were identified. Identified joints count: [{jointsIdentified}]. Model joints count: [{modelJointsCount}]");

                //throw new Exception($"Not all joints were identified. Identified joints count: [{jointsIdentified}]. Model joints count: [{modelJointsCount}]");
            }
        }

        private List<CConnectionJointTypes> GetConnectionJointTypesFor(CConnectionDescription con)
        {
            if (_pfdVM.Model.m_arrConnectionJoints == null) return null; // Ak neexistuju v modeli spoje vratime null

            List<CConnectionJointTypes> items = _pfdVM.Model.m_arrConnectionJoints.FindAll(c => c.GetType() == GetTypeFor(con.JoinType));
            List<CConnectionJointTypes> resItems = new List<CConnectionJointTypes>();

            bool debugging = false;
            if (debugging)
            {
                System.Diagnostics.Trace.WriteLine($"{con.ID}. {con.Name} {con.JoinType}");
                int count = 0;
                foreach (CConnectionJointTypes joint in items)
                {
                    count++;
                    if (joint.m_SecondaryMembers != null) System.Diagnostics.Trace.WriteLine($"{count}. {joint.GetType()} {joint.m_MainMember.EMemberTypePosition} {joint.m_SecondaryMembers.Count()} {joint.m_SecondaryMembers[0].EMemberTypePosition}");
                    else System.Diagnostics.Trace.WriteLine($"{count}. {joint.GetType()} {joint.m_MainMember.EMemberTypePosition}");
                }
            }

            ValidateModelConnectionJoints();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            foreach (CConnectionJointTypes joint in items)
            {
                if (joint.m_MainMember == null) continue;
                if (joint.BIsGenerated == false) continue;

                switch (con.ID)
                {
                    //1   Base - main column
                    case 1:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainColumn && joint.m_SecondaryMembers == null)
                            resItems.Add(joint);
                        break;
                    //2   Knee - main rafter to column
                    case 2:
                        if ((joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainColumn) &&
                            joint.m_SecondaryMembers != null && joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.MainRafter
                            )
                            resItems.Add(joint);
                        break;
                    //3   Apex - main rafters
                    case 3:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.MainRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //4   Base - edge column
                    case 4:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn && joint.m_SecondaryMembers == null)
                            resItems.Add(joint);
                        break;
                    //5   Knee - edge rafter to column
                    case 5:
                        if ((joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn) &&
                            joint.m_SecondaryMembers != null && joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeRafter
                            )
                            resItems.Add(joint);
                        break;
                    //6   Apex - edge rafters
                    case 6:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //7   Purlin to main rafter
                    case 7:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Purlin)
                            )
                            resItems.Add(joint);
                        break;
                    //8   Purlin to edge rafter
                    case 8:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Purlin)
                            )
                            resItems.Add(joint);
                        break;
                    //9   Purlin to main rafter - fly bracing
                    case 9:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Purlin)
                            )
                            resItems.Add(joint);
                        break;
                    //10  Purlin to edge rafter - fly bracing
                    case 10:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Purlin)
                            )
                            resItems.Add(joint);
                        break;
                    //11  Edge purlin to main rafter
                    case 11:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgePurlin)
                            )
                            resItems.Add(joint);
                        break;
                    //12  Edge purlin to edge rafter
                    case 12:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgePurlin)
                            )
                            resItems.Add(joint);
                        break;
                    //13  Girt to main column
                    case 13:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainColumn &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Girt)
                            )
                            resItems.Add(joint);
                        break;
                    //14  Girt to edge column
                    case 14:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Girt)
                            )
                            resItems.Add(joint);
                        break;
                    //15  Base - wind post - front
                    case 15:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.ColumnFrontSide && joint.m_SecondaryMembers == null)
                            resItems.Add(joint);
                        break;
                    //16  Base - wind post - back
                    case 16:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.ColumnBackSide && joint.m_SecondaryMembers == null)
                            resItems.Add(joint);
                        break;
                    //17  Wind post to edge rafter - front
                    case 17:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.ColumnFrontSide)
                            )
                            resItems.Add(joint);
                        break;
                    //18  Wind post to edge rafter - back
                    case 18:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.ColumnBackSide)
                            )
                            resItems.Add(joint);
                        break;
                    //19  Girt to edge column - front
                    case 19:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.GirtFrontSide)
                            )
                            resItems.Add(joint);
                        break;
                    //20  Girt to edge column - back
                    case 20:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.GirtBackSide)
                            )
                            resItems.Add(joint);
                        break;
                    //21  Girt to wind post - front
                    case 21:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.ColumnFrontSide &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.GirtFrontSide)
                            )
                            resItems.Add(joint);
                        break;
                    //22  Girt to wind post - back
                    case 22:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.ColumnBackSide &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.GirtBackSide)
                            )
                            resItems.Add(joint);
                        break;
                    //23  Base - door trimmer
                    case 23:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer && joint.m_SecondaryMembers == null)
                            resItems.Add(joint);
                        break;
                    //24  Door trimmer to girt
                    case 24:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Girt &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                            )
                            resItems.Add(joint);
                        break;
                    //25  Door trimmer to girt - front
                    case 25:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.GirtFrontSide &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                            )
                            resItems.Add(joint);
                        break;
                    //26  Door trimmer to girt - back
                    case 26:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.GirtBackSide &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                            )
                            resItems.Add(joint);
                        break;
                    //27  Girt to door trimmer
                    case 27:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Girt)
                            )
                            resItems.Add(joint);
                        break;
                    //28  Girt to door trimmer - front
                    case 28:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.GirtFrontSide)
                            )
                            resItems.Add(joint);
                        break;
                    //29  Girt to door trimmer - back
                    case 29:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.GirtBackSide)
                            )
                            resItems.Add(joint);
                        break;
                    //30  Door trimmer to edge pulin
                    case 30:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgePurlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                            )
                            resItems.Add(joint);
                        break;
                    //31  Door trimmer to edge rafter
                    case 31:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                            )
                            resItems.Add(joint);
                        break;
                    //32  Door lintel to trimmer
                    case 32:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorLintel)
                            )
                            resItems.Add(joint);
                        break;
                    //33  Base - door frame
                    case 33:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorFrame && joint.m_SecondaryMembers == null)
                            resItems.Add(joint);
                        break;
                    //34  Door frame to girt
                    case 34:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Girt &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //35  Door frame to girt - front
                    case 35:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.GirtFrontSide &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //36  Door frame to girt - back
                    case 36:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.GirtBackSide &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //37  Girt to door frame
                    case 37:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Girt)
                            )
                            resItems.Add(joint);
                        break;
                    //38  Girt to door frame - front
                    case 38:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.GirtFrontSide)
                            )
                            resItems.Add(joint);
                        break;
                    //39  Girt to door frame - back
                    case 39:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.GirtBackSide)
                            )
                            resItems.Add(joint);
                        break;
                    //40  Door frame to edge pulin
                    case 40:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgePurlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //41  Door frame to edge rafter
                    case 41:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //42  Door frame
                    case 42:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //43  Window frame to girt
                    case 43:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Girt &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.WindowFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //44  Window frame to girt - front
                    case 44:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.GirtFrontSide &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.WindowFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //45  Window frame to girt - back
                    case 45:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.GirtBackSide &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.WindowFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //46  Window frame to edge pulin
                    case 46:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgePurlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.WindowFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //47  Window frame to edge rafter
                    case 47:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.WindowFrame)
                            )
                            resItems.Add(joint);
                        break;
                    //48  Window frame (header / sill to window frame column)
                    case 48:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.WindowFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.WindowFrame)
                            )
                            resItems.Add(joint);
                        break;
                } //end switch
            } //end foreach

            //----------------------------------------------------------------------------------------------------------------------------
            // TO - Ondrej - pozri prosim na toto
            // Ak sme nasli vsetky spoje daneho typu objektu a vyselektovali z nich tie, ktore maju ocakavane typy prutov
            // Nastavime tymto spojom spravny typ spoja podla typov a polohy prutov, ktore su k nim pripojene
            // Do buducna bude istejsie nastavovat typ spoja uz priamo pri vytvoreni objektu spoja, potom bude cely tento switch zbytocny, alebo aspon vyrazne kratsi

            // Ano jasne. Jedine naozaj dobre riesenie je nastavit enum EJointType priamo pri vytvarani konkretneho objektu Joint
            foreach (CConnectionJointTypes joint in resItems)
            {
                joint.JointType = (EJointType)con.ID;
            }
            //----------------------------------------------------------------------------------------------------------------------------

            return resItems;
        }
        private Type GetTypeFor(string strType)
        {
            switch (strType)
            {
                case "A001": return typeof(CConnectionJoint_A001);
                case "B001": return typeof(CConnectionJoint_B001);
                //case "C001": return typeof(CConnectionJoint_C001); // Tieto spoje budu zmazane
                //case "CT01": return typeof(CConnectionJoint_CT01);
                //case "D001": return typeof(CConnectionJoint_D001);
                //case "E001": return typeof(CConnectionJoint_E001);
                //case "J001": return typeof(CConnectionJoint_J001);
                //case "L001": return typeof(CConnectionJoint_L001);
                case "S001": return typeof(CConnectionJoint_S001);
                case "T001": return typeof(CConnectionJoint_T001);
                //case "T002": return typeof(CConnectionJoint_T002); (TODO Marin - rozpracovat podrobne)
                case "T003": return typeof(CConnectionJoint_T003);
                case "TA01": return typeof(CConnectionJoint_TA01);
                case "TB01": return typeof(CConnectionJoint_TB01);
                case "TC01": return typeof(CConnectionJoint_TC01);
                case "TD01": return typeof(CConnectionJoint_TD01);
                default:

                    //temp it should throw exceoton if there is not recognized joint type
                    return null;
                    //throw new Exception($"Type of connection joint [{strType}] not recognized. (Method GetTypeFor)");
            }
        }
        private void SetDynamicTabs(CJointsVM vm)
        {
            if (jointsDict == null) ArrangeConnectionJoints();
            List<TabItem> tabItems = new List<TabItem>();
            CConnectionJointTypes joint = GetSelectedJoint();

            if (joint == null)
            {
                TabItem t1 = new TabItem();
                t1.Header = "Results";
                StackPanel sp = new StackPanel();
                Label l = new Label();
                l.Content = "No joints found for selected type.";
                sp.Children.Add(l);
                t1.Content = sp;
                tabItems.Add(t1);
            }
            else
            {
                if (joint.m_arrPlates != null)
                {
                    foreach (CPlate plate in joint.m_arrPlates)
                    {
                        TabItem ti = new TabItem();
                        ti.Header = plate.Name;

                        SetTabContent(ti, plate);

                        tabItems.Add(ti);
                    }
                }

                displayJoint(joint);
            }

            vm.TabItems = tabItems;
            vm.SelectedTabIndex = 0;
        }

        private void SetTabContent(TabItem ti, CPlate plate)
        {
            //ScrollViewer sw = new ScrollViewer();
            //sw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            //sw.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            //sw.Width = 560;
            StackPanel sp = new StackPanel();
            sp.Width = 560;
            sp.VerticalAlignment = VerticalAlignment.Top;
            sp.HorizontalAlignment = HorizontalAlignment.Left;

            ComboBox selectPlateType = new ComboBox();
            selectPlateType.HorizontalAlignment = HorizontalAlignment.Left;
            selectPlateType.Width = 200;
            selectPlateType.Height = 20;
            List<string> series = CPlateHelper.GetPlateSeries(plate);
            selectPlateType.ItemsSource = series;
            selectPlateType.SelectedIndex = series.IndexOf(plate.Name);
            selectPlateType.SelectionChanged += SelectPlateSerie_SelectionChanged;
            sp.Children.Add(selectPlateType);


            //Grid grid = new Grid();
            //RowDefinition row = new RowDefinition();
            //row.Height = new GridLength(40);
            //grid.RowDefinitions.Add(row);

            //row = new RowDefinition();
            //row.Height = new GridLength(1.0, GridUnitType.Star);
            //grid.RowDefinitions.Add(row);

            //row = new RowDefinition();
            //row.Height = new GridLength(40);
            //grid.RowDefinitions.Add(row);

            //row = new RowDefinition();
            //row.Height = new GridLength(1.0, GridUnitType.Star);
            //grid.RowDefinitions.Add(row);

            //row = new RowDefinition();
            //row.Height = new GridLength(40);
            //grid.RowDefinitions.Add(row);

            //row = new RowDefinition();
            //row.Height = new GridLength(1.0, GridUnitType.Star);
            //grid.RowDefinitions.Add(row);

            // Base Plate - Anchor Arrangement
            if (plate is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;

                // Anchor arrangement
                StackPanel spAA = new StackPanel();
                sp.Width = 550;
                spAA.Orientation = Orientation.Horizontal;
                Label lAA = new Label() { Content = "Anchor Arrangement: " };
                ComboBox selectAA = new ComboBox();   
                
                selectAA.Width = 200;
                selectAA.Height = 20;
                selectAA.ItemsSource = CPlateHelper.GetPlateAnchorArangementTypes(basePlate);
                selectAA.SelectedIndex = CPlateHelper.GetPlateAnchorArangementIndex(basePlate);               
                selectAA.SelectionChanged += SelectAA_SelectionChanged;
                spAA.Children.Add(lAA);
                spAA.Children.Add(selectAA);
                sp.Children.Add(spAA);

                if (basePlate.AnchorArrangement != null)
                {
                    List<CComponentParamsView> anchorArrangementParams = CPlateHelper.GetAnchorArrangementProperties(basePlate.AnchorArrangement);
                    //lAA.SetValue(Grid.RowProperty, 0);
                    sp.Children.Add(GetDatagridForAnchorArrangement(anchorArrangementParams));
                }
            }

            // Screw Arrangement

            StackPanel spSA = new StackPanel();
            sp.Width = 550;
            spSA.Orientation = Orientation.Horizontal;
            Label lSA = new Label() { Content = "Screw Arrangement: " };
            ComboBox selectSA = new ComboBox();
            selectSA.Width = 200;
            selectSA.Height = 20;
            selectSA.ItemsSource = CPlateHelper.GetPlateScrewArangementTypes(plate);
            selectSA.SelectedIndex = CPlateHelper.GetPlateScrewArangementIndex(plate);
            selectSA.SelectionChanged += SelectSA_SelectionChanged;
            spSA.Children.Add(lSA);
            spSA.Children.Add(selectSA);
            sp.Children.Add(spSA);

            if (plate.ScrewArrangement != null)
            {
                List<CComponentParamsView> screwArrangementParams = CPlateHelper.GetScrewArrangementProperties(plate.ScrewArrangement);
                //lSA.SetValue(Grid.RowProperty, 0);
                sp.Children.Add(GetDatagridForScrewArrangement(screwArrangementParams));
            }

            Label l = new Label() { Content = "Geometry: ", Margin = new Thickness(0, 15, 0, 0) };
            //l.SetValue(Grid.RowProperty, 2);
            sp.Children.Add(l);
            List<CComponentParamsView> geometryParams = CPlateHelper.GetComponentProperties(plate);
            sp.Children.Add(GetDatagridForGeometry(geometryParams));

            l = new Label() { Content = "Details: ", Margin = new Thickness(0, 15, 0, 0) };
            //l.SetValue(Grid.RowProperty, 4);
            sp.Children.Add(l);

            List<CComponentParamsView> details = CPlateHelper.GetComponentDetails(plate);
            sp.Children.Add(GetDatagridForDetails(details));

            //sw.Content = sp;

            ti.Content = sp;
            ti.IsEnabled = true;
        }


        private void SelectPlateSerie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CPlate plate = GetSelectedPlate();
            if (plate == null) return;
            CConnectionJointTypes joint = GetSelectedJoint();
            if (joint == null) return;

            ComboBox cb = sender as ComboBox;
            if (cb == null) return;
            int componentIndex = cb.SelectedIndex;
            string componentName = cb.SelectedValue.ToString();

            paramsChanged = true;


            float fb_R; // Rafter Width
            float fb_B; // Wind Post Width
            float fb; // in plane XY -X coord
            float fb2; // in plane XY - X coord
            float fh; // in plane XY - Y coord
            float fh2; // in plane XY - Y coord
            float fl; // out of plane - Z coord
            float fl2; // out of plane - Z coord
            float ft;
            int iNumberOfStiffeners = 0;
            float fb_fl; // Flange - Z-section
            float fc_lip1; // LIP - Z-section
            float fRoofPitch_rad;
            float fGamma_rad; // Plate M alebo N uhol medzi hranou prierezu a vonkajsou hranou plechu
            int iNumberofHoles = 0;

            bool bUseSimpleShapeOfPlates = true; // Zjednoduseny alebo presny tvar plechu
            Point3D controlpoint = new Point3D(0, 0, 0);

            CAnchor referenceAnchor = new CAnchor("M16", "8.8", 0.33f, 0.3f, true);
            CScrew referenceScrew = new CScrew("TEK", "14");

            float fCrsc_h = 0.27f; // Default depth of connected member cross-section

            bool bUseAdditionalConnectors = true;
            int iNumberOfAdditionalConnectorsInCorner = 4;
            int iConnectorNumberInCircleSequence = 20;
            float fConnectorRadiusInCircleSequence = 0.25f;

            List<CScrewSequenceGroup> screwSeqGroups = new List<CScrewSequenceGroup>();
            CScrewSequenceGroup gr1 = new CScrewSequenceGroup();
            gr1.NumberOfHalfCircleSequences = 2;
            gr1.NumberOfRectangularSequences = 4;
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr1);
            CScrewSequenceGroup gr2 = new CScrewSequenceGroup();
            gr2.NumberOfHalfCircleSequences = 2;
            gr2.NumberOfRectangularSequences = 4;
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr2);

            // 63020 default
            CScrewArrangementCircleApexOrKnee screwArrangementCircle = new CScrewArrangementCircleApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 1, screwSeqGroups, bUseAdditionalConnectors, fConnectorRadiusInCircleSequence, fConnectorRadiusInCircleSequence, iNumberOfAdditionalConnectorsInCorner, 0.03f, 0.03f);
            CScrewArrangementRectApexOrKnee screwArrangementRectangleApex = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 0.05f, 0.05f, 0.07f, 0.05f, 8, 2, 0.15f, 0.55f, 0.075f, 0.05f);
            //CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 10, 2, 10, 2);
            CScrewArrangementRectApexOrKnee screwArrangementRectangleKnee = new CScrewArrangementRectApexOrKnee(referenceScrew, 0.63f, 0.63f - 2 * 0.025f - 2 * 0.002f, 0.18f, 12, 2, 0.040f, 0.047f, 0.050f, 0.158f, 12, 2, 0.040f, 0.425f, 0.050f, 0.158f, 12, 2, 0.05f, 0.047f, 0.05f, 0.158f, 14, 2, 0.05f, 0.425f, 0.05f, 0.158f);

            // 270xx default
            bool bUseAdditionalConnectors_270xx = false;
            int iNumberOfAdditionalConnectorsInCorner_270xx = 4;
            int iConnectorNumberInCircleSequence_270xx = 8;
            float fConnectorRadiusInCircleSequence_270xx = 0.12f;

            List<CScrewSequenceGroup> screwSeqGroups_270xx = new List<CScrewSequenceGroup>();
            CScrewSequenceGroup gr1_270xx = new CScrewSequenceGroup();
            gr1_270xx.NumberOfHalfCircleSequences = 2;
            gr1_270xx.NumberOfRectangularSequences = 4;
            gr1_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
            gr1_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
            screwSeqGroups_270xx.Add(gr1_270xx);
            CScrewSequenceGroup gr2_270xx = new CScrewSequenceGroup();
            gr2_270xx.NumberOfHalfCircleSequences = 2;
            gr2_270xx.NumberOfRectangularSequences = 4;
            gr2_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
            gr2_270xx.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence_270xx, iConnectorNumberInCircleSequence_270xx));
            screwSeqGroups_270xx.Add(gr2_270xx);

            CDatabaseComponents dcomponents = new CDatabaseComponents();

            switch (plate.m_ePlateSerieType_FS)
            {
                case ESerieTypePlate.eSerie_B:
                    {
                        CPlate_B_Properties prop = CJointsManager.GetPlate_B_Properties(componentIndex + 1);
                        fb = (float)prop.dim1;
                        fb2 = fb;
                        fh = (float)prop.dim2y;
                        fl = (float)prop.dim3;
                        ft = (float)prop.t;
                        iNumberofHoles = prop.iNumberHolesAnchors; // !!!! - rozlisovat medzi otvormi pre skrutky a pre anchors

                        plate = new CConCom_Plate_B_basic(prop.Name, controlpoint, fb, fh, fl, ft, 0, 0, 0, referenceAnchor, CJointHelper.GetBasePlateArrangement(prop.Name, referenceScrew/*, fh*/)); // B
                        break;
                    }
                case ESerieTypePlate.eSerie_L:
                    {
                        CPlate_L_Properties prop = CJointsManager.GetPlate_L_Properties(componentIndex + 1);
                        CScrewArrangement_L screwArrangement_L = new CScrewArrangement_L(prop.NumberOfHolesScrews, referenceScrew);

                        fb = (float)prop.dim1;
                        fb2 = fb;
                        fh = (float)prop.dim2y;
                        fl = (float)prop.dim3;
                        ft = (float)prop.thickness;
                        iNumberofHoles = prop.NumberOfHolesScrews;
                        plate = new CConCom_Plate_F_or_L(prop.Name, controlpoint, fb, fh, fl, ft, fCrsc_h, 0, 0, 0, screwArrangement_L); // L
                        break;
                    }
                case ESerieTypePlate.eSerie_LL:
                    {
                        CPlate_LL_Properties prop = CJointsManager.GetPlate_LL_Properties(componentIndex + 1);
                        CScrewArrangement_LL screwArrangement_LL = new CScrewArrangement_LL(prop.NumberOfHolesScrews, referenceScrew);
                        fb = (float)prop.dim11;
                        fb2 = (float)prop.dim12;
                        fh = (float)prop.dim2y;
                        fl = (float)prop.dim3;
                        ft = (float)prop.thickness;
                        iNumberofHoles = prop.NumberOfHolesScrews;
                        plate = new CConCom_Plate_LL(prop.Name, controlpoint, fb, fb2, fh, fl, ft, 0, 0, 0, screwArrangement_LL); // LL
                        break;
                    }
                case ESerieTypePlate.eSerie_F:
                    {
                        CPlate_F_Properties prop = CJointsManager.GetPlate_F_Properties(componentIndex + 1);
                        CScrewArrangement_F screwArrangement_F = new CScrewArrangement_F(prop.NumberOfHolesScrews, referenceScrew);
                        fb = (float)prop.dim11;
                        fb2 = (float)prop.dim12;
                        fh = (float)prop.dim2y;
                        fl = (float)prop.dim3;
                        ft = (float)prop.thickness;
                        iNumberofHoles = prop.NumberOfHolesScrews;
                        plate = new CConCom_Plate_F_or_L(prop.Name, controlpoint, fb, fb2, fh, fl, ft, fCrsc_h, 0f, 0f, 0f, screwArrangement_F); // F
                        break;
                    }
                case ESerieTypePlate.eSerie_G:
                    {
                        CScrewArrangement_G screwArrangement_G = new CScrewArrangement_G(/*iNumberofHoles, */ referenceScrew);
                        fb = dcomponents.arr_Serie_G_Dimension[componentIndex, 0] / 1000f;
                        fb2 = dcomponents.arr_Serie_G_Dimension[componentIndex, 1] / 1000f;
                        fh = dcomponents.arr_Serie_G_Dimension[componentIndex, 2] / 1000f;
                        fh2 = dcomponents.arr_Serie_G_Dimension[componentIndex, 3] / 1000f;
                        fl = dcomponents.arr_Serie_G_Dimension[componentIndex, 4] / 1000f;
                        ft = dcomponents.arr_Serie_G_Dimension[componentIndex, 5] / 1000f;
                        plate = new CConCom_Plate_G(dcomponents.arr_Serie_G_Names[componentIndex], controlpoint, fb, fb2, fh, fh2, fl, 0.5f * fh2, ft, 0f, 0f, 0f, screwArrangement_G); // G
                        break;
                    }
                case ESerieTypePlate.eSerie_H:
                    {
                        CScrewArrangement_H screwArrangement_H = new CScrewArrangement_H(/*iNumberofHoles, */ referenceScrew);
                        fb = dcomponents.arr_Serie_H_Dimension[componentIndex, 0] / 1000f;
                        fh = dcomponents.arr_Serie_H_Dimension[componentIndex, 1] / 1000f;
                        fh2 = dcomponents.arr_Serie_H_Dimension[componentIndex, 2] / 1000f;
                        ft = dcomponents.arr_Serie_H_Dimension[componentIndex, 3] / 1000f;
                        plate = new CConCom_Plate_H(dcomponents.arr_Serie_H_Names[componentIndex], controlpoint, fb, fh, fh2, 0.2f * fb, ft, 11f * MathF.fPI / 180f, 0f, 0f, 0f, screwArrangement_H); // H
                        break;
                    }
                case ESerieTypePlate.eSerie_Q:
                    {
                        fb = dcomponents.arr_Serie_Q_Dimension[componentIndex, 0] / 1000f;
                        fh = dcomponents.arr_Serie_Q_Dimension[componentIndex, 1] / 1000f;
                        fl = dcomponents.arr_Serie_Q_Dimension[componentIndex, 2] / 1000f;
                        ft = dcomponents.arr_Serie_Q_Dimension[componentIndex, 3] / 1000f;
                        iNumberofHoles = (int)dcomponents.arr_Serie_Q_Dimension[componentIndex, 4];
                        plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_Q_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles); // Q
                        break;
                    }
                case ESerieTypePlate.eSerie_T:
                    {
                        fb = dcomponents.arr_Serie_T_Dimension[componentIndex, 0] / 1000f;
                        fh = dcomponents.arr_Serie_T_Dimension[componentIndex, 1] / 1000f;
                        fl = dcomponents.arr_Serie_T_Dimension[componentIndex, 2] / 1000f;
                        ft = dcomponents.arr_Serie_T_Dimension[componentIndex, 3] / 1000f;
                        iNumberofHoles = (int)dcomponents.arr_Serie_T_Dimension[componentIndex, 4];
                        plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_T_Names[0], controlpoint, fb, fh, fl, ft, iNumberofHoles); // T
                        break;
                    }
                case ESerieTypePlate.eSerie_Y:
                    {
                        fb = dcomponents.arr_Serie_Y_Dimension[componentIndex, 0] / 1000f;
                        fh = dcomponents.arr_Serie_Y_Dimension[componentIndex, 1] / 1000f;
                        fl = dcomponents.arr_Serie_Y_Dimension[componentIndex, 2] / 1000f;
                        fl2 = dcomponents.arr_Serie_Y_Dimension[componentIndex, 3] / 1000f;
                        ft = dcomponents.arr_Serie_Y_Dimension[componentIndex, 4] / 1000f;
                        iNumberofHoles = (int)dcomponents.arr_Serie_Y_Dimension[componentIndex, 5];
                        plate = new CConCom_Plate_Q_T_Y(dcomponents.arr_Serie_Y_Names[0], controlpoint, fb, fh, fl, fl2, ft, iNumberofHoles); // Y
                        break;
                    }
                case ESerieTypePlate.eSerie_J:
                    {
                        fb = dcomponents.arr_Serie_J_Dimension[componentIndex, 0] / 1000f;
                        fh = dcomponents.arr_Serie_J_Dimension[componentIndex, 1] / 1000f;
                        fh2 = dcomponents.arr_Serie_J_Dimension[componentIndex, 2] / 1000f;
                        fl = dcomponents.arr_Serie_J_Dimension[componentIndex, 3] / 1000f;
                        ft = dcomponents.arr_Serie_J_Dimension[componentIndex, 4] / 1000f;
                        iNumberofHoles = (int)dcomponents.arr_Serie_J_Dimension[componentIndex, 5];
                        if (componentIndex == 0) // JA
                        {
                            //to Mato - moze zdedit ScrewArangement z povodnej plate???
                            plate = new CConCom_Plate_JA(dcomponents.arr_Serie_J_Names[0], controlpoint, fb, fh, fh2, ft, 0, 0, 0, true, plate.ScrewArrangement);
                        }
                        else if (componentIndex == 1) // JB
                        {
                            if (bUseSimpleShapeOfPlates)
                            {
                                plate = new CConCom_Plate_JBS(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true, plate.ScrewArrangement);
                            }
                            else
                            {
                                plate = new CConCom_Plate_JB(dcomponents.arr_Serie_J_Names[1], controlpoint, fb, fh, fh2, fl, ft, 0, 0, 0, true, plate.ScrewArrangement);
                            }
                        }
                        else //(componentIndex == 2) // JC
                        {
                            float fw = 0.205f; // TODO - dopracovat vypocet tak aby sa konvertovali hodnoty z databazy
                            float fd = 0.27f;
                            float fSlope_rad = 3f * MathF.fPI / 180f; // Default 3 stupne

                            plate = new CConCom_Plate_JCS(dcomponents.arr_Serie_J_Names[2], controlpoint, fd, fw, fl, fSlope_rad, ft, 0, 0, 0, true, plate.ScrewArrangement);
                        }

                        break;
                    }
                case ESerieTypePlate.eSerie_K:
                    {
                        fb_R = dcomponents.arr_Serie_K_Dimension[componentIndex, 0] / 1000f;
                        fb = dcomponents.arr_Serie_K_Dimension[componentIndex, 1] / 1000f;
                        fh = dcomponents.arr_Serie_K_Dimension[componentIndex, 2] / 1000f;
                        fb2 = dcomponents.arr_Serie_K_Dimension[componentIndex, 3] / 1000f;
                        fh2 = dcomponents.arr_Serie_K_Dimension[componentIndex, 4] / 1000f;
                        fl = dcomponents.arr_Serie_K_Dimension[componentIndex, 5] / 1000f;
                        ft = dcomponents.arr_Serie_K_Dimension[componentIndex, 6] / 1000f;
                        iNumberofHoles = (int)dcomponents.arr_Serie_K_Dimension[componentIndex, 7];
                        if (componentIndex == 0) // KA
                        {
                            plate = new CConCom_Plate_KA(dcomponents.arr_Serie_K_Names[0], controlpoint, fb, fh, fb2, fh2, ft, 0, 0, 0, false, plate.ScrewArrangement);
                        }
                        else if (componentIndex == 1) // KB
                        {
                            if (bUseSimpleShapeOfPlates)
                            {
                                plate = new CConCom_Plate_KBS(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, plate.ScrewArrangement);
                            }
                            else
                            {
                                plate = new CConCom_Plate_KB(dcomponents.arr_Serie_K_Names[1], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, plate.ScrewArrangement);
                            }
                        }
                        else if (componentIndex == 2) // KC
                        {
                            if (bUseSimpleShapeOfPlates)
                            {
                                plate = new CConCom_Plate_KCS(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, plate.ScrewArrangement);
                            }
                            else
                            {
                                plate = new CConCom_Plate_KC(dcomponents.arr_Serie_K_Names[2], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, plate.ScrewArrangement);

                            }
                        }
                        else if (componentIndex == 3) // KD
                        {
                            if (bUseSimpleShapeOfPlates)
                            {
                                plate = new CConCom_Plate_KDS(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, plate.ScrewArrangement);

                            }
                            else
                            {
                                plate = new CConCom_Plate_KD(dcomponents.arr_Serie_K_Names[3], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, plate.ScrewArrangement);

                            }
                        }
                        else if (componentIndex == 4) // KES
                        {
                            plate = new CConCom_Plate_KES(dcomponents.arr_Serie_K_Names[4], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, plate.ScrewArrangement);

                        }
                        else if (componentIndex == 5) // KFS
                        {
                            plate = new CConCom_Plate_KFS(dcomponents.arr_Serie_K_Names[5], controlpoint, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, false, plate.ScrewArrangement);
                        }
                        else // KK - TODO - screws are not implemented !!!
                        {
                            plate = new CConCom_Plate_KK(dcomponents.arr_Serie_K_Names[6], controlpoint, fb_R, fb, fh, fb2, fh2, fl, ft, 0, 0, 0, plate.ScrewArrangement);
                        }
                        break;
                    }
                case ESerieTypePlate.eSerie_M:
                    {
                        // b, h, t, iHoles, bBeam, slope_deg
                        fb = dcomponents.arr_Serie_M_Dimension[componentIndex, 0] / 1000f;
                        fh = dcomponents.arr_Serie_M_Dimension[componentIndex, 1] / 1000f;
                        ft = dcomponents.arr_Serie_M_Dimension[componentIndex, 2] / 1000f;
                        iNumberofHoles = (int)dcomponents.arr_Serie_M_Dimension[componentIndex, 3];
                        fb_B = dcomponents.arr_Serie_M_Dimension[componentIndex, 4] / 1000f;
                        fRoofPitch_rad = dcomponents.arr_Serie_M_Dimension[componentIndex, 5] / 180 * MathF.fPI;
                        fGamma_rad = dcomponents.arr_Serie_M_Dimension[componentIndex, 6] / 180 * MathF.fPI;
                        CScrewArrangement_M screwArrangement_M = new CScrewArrangement_M(iNumberofHoles, referenceScrew);
                        // b, h, t, iHoles, bBeam, slope_rad
                        plate = new CConCom_Plate_M(dcomponents.arr_Serie_M_Names[0], controlpoint, 0.5f * (fb - fb_B), 0.5f * (fb - fb_B), fh, ft, fb_B, fRoofPitch_rad, fGamma_rad, 0, 0, 0, screwArrangement_M); // M
                        break;
                    }
                case ESerieTypePlate.eSerie_N:
                    {
                        fb = dcomponents.arr_Serie_N_Dimension[componentIndex, 0] / 1000f;
                        fb2 = dcomponents.arr_Serie_N_Dimension[componentIndex, 1] / 1000f;
                        fh = dcomponents.arr_Serie_N_Dimension[componentIndex, 2] / 1000f;
                        fl = dcomponents.arr_Serie_N_Dimension[componentIndex, 3] / 1000f;
                        ft = dcomponents.arr_Serie_N_Dimension[componentIndex, 4] / 1000f;
                        iNumberofHoles = (int)dcomponents.arr_Serie_N_Dimension[componentIndex, 5];
                        CScrewArrangement_N screwArrangement_N = new CScrewArrangement_N(iNumberofHoles, referenceScrew);
                        plate = new CConCom_Plate_N(dcomponents.arr_Serie_N_Names[0], controlpoint, fb, fb2, fh, fl, ft, 0, 0, 0, screwArrangement_N); // N
                        break;
                    }
                case ESerieTypePlate.eSerie_O:
                    {
                        fb = dcomponents.arr_Serie_O_Dimension[componentIndex, 0] / 1000f;
                        fb2 = dcomponents.arr_Serie_O_Dimension[componentIndex, 1] / 1000f;
                        fh = dcomponents.arr_Serie_O_Dimension[componentIndex, 2] / 1000f;
                        fh2 = dcomponents.arr_Serie_O_Dimension[componentIndex, 3] / 1000f;
                        ft = dcomponents.arr_Serie_O_Dimension[componentIndex, 4] / 1000f;
                        iNumberofHoles = (int)dcomponents.arr_Serie_O_Dimension[componentIndex, 5];

                        CScrewArrangement_O sc = plate.ScrewArrangement as CScrewArrangement_O;
                        plate = new CConCom_Plate_O(dcomponents.arr_Serie_O_Names[0], controlpoint, fb, fb2, fh, fh2, ft, 11f * MathF.fPI / 180f, 0, 0, 0, sc);

                        //CScrewArrangement_O screwArrangement_O = new CScrewArrangement_O(referenceScrew, 1, 10, 0.02f, 0.02f, 0.05f, 0.05f, 1, 10, 0.18f, 0.02f, 0.05f, 0.05f);
                        //if (vm.ScrewArrangementIndex == 0) // Undefined
                        //    plate = new CConCom_Plate_O(dcomponents.arr_Serie_O_Names[0], controlpoint, fb, fb2, fh, fh2, ft, 11f * MathF.fPI / 180f, 0, 0, 0, null);
                        //else //if (vm.ScrewArrangementIndex == 1) // Rectangular
                        //    plate = new CConCom_Plate_O(dcomponents.arr_Serie_O_Names[0], controlpoint, fb, fb2, fh, fh2, ft, 11f * MathF.fPI / 180f, 0, 0, 0, screwArrangement_O); // O
                        break;
                    }
                default:
                    {
                        // Not implemented
                        break;
                    }
            } //end switch

            joint.m_arrPlates[vm.SelectedTabIndex] = plate;

            TabItem ti = vm.TabItems[vm.SelectedTabIndex];
            ti.Header = componentName;
            SetTabContent(ti, plate);

            displayJoint(joint);

            //if (_pfdVM.SynchronizeGUI) _pfdVM.SynchronizeGUI = true;
        }



        private void SelectAA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CPlate plate = GetSelectedPlate();
            if (plate == null) return;
            CConnectionJointTypes joint = GetSelectedJoint();
            if (joint == null) return;

            ComboBox cbAA = sender as ComboBox;
            if (cbAA == null) return;            

            paramsChanged = true;
            ChangeAllSameJointsPlateAnchorArrangement(cbAA.SelectedIndex);
            //CPlateHelper.AnchorArrangementChanged(joint, plate, cbAA.SelectedIndex);
            //CPlateHelper.UpdatePlateAnchorArrangementData(plate);

            TabItem ti = vm.TabItems[vm.SelectedTabIndex];
            SetTabContent(ti, plate);

            displayJoint(joint);

            //if (_pfdVM.SynchronizeGUI) _pfdVM.SynchronizeGUI = true;
        }

        private void SelectSA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CPlate plate = GetSelectedPlate();
            if (plate == null) return;
            CConnectionJointTypes joint = GetSelectedJoint();
            if (joint == null) return;

            ComboBox cbSA = sender as ComboBox;
            if (cbSA == null) return;

            paramsChanged = true;
            ChangeAllSameJointsPlateScrewArrangement(cbSA.SelectedIndex);
            //CPlateHelper.ScrewArrangementChanged(joint, plate, cbSA.SelectedIndex);
            //CPlateHelper.UpdatePlateScrewArrangementData(plate);

            TabItem ti = vm.TabItems[vm.SelectedTabIndex];
            SetTabContent(ti, plate);

            displayJoint(joint);

            //if (_pfdVM.SynchronizeGUI) _pfdVM.SynchronizeGUI = true;
        }

        private void ChangeAllSameJointsPlateAnchorArrangement(int anchorArrangementIndex)
        {
            List<CConnectionJointTypes> joints = GetSelectedJoints();
            foreach (CConnectionJointTypes joint in joints)
            {
                CPlateHelper.AnchorArrangementChanged(joint, joint.m_arrPlates[vm.SelectedTabIndex], anchorArrangementIndex);
                CPlateHelper.UpdatePlateAnchorArrangementData(joint.m_arrPlates[vm.SelectedTabIndex]);
            }
        }

        private void ChangeAllSameJointsPlateScrewArrangement(int screwArrangementIndex)
        {
            List<CConnectionJointTypes> joints = GetSelectedJoints();
            foreach (CConnectionJointTypes joint in joints)
            {
                CPlateHelper.ScrewArrangementChanged(joint, joint.m_arrPlates[vm.SelectedTabIndex], screwArrangementIndex);
                CPlateHelper.UpdatePlateScrewArrangementData(joint.m_arrPlates[vm.SelectedTabIndex]);
            }
        }

        private CPlate GetSelectedPlate()
        {
            CConnectionJointTypes joint = GetSelectedJoint();
            if (joint == null) return null;
            if (joint.m_arrPlates == null) return null;

            return joint.m_arrPlates[vm.SelectedTabIndex];
        }

        private CConnectionJointTypes GetSelectedJoint()
        {
            if (vm.JointTypeIndex == -1) return null;
            CConnectionDescription con = vm.JointTypes[vm.JointTypeIndex];
            list_joints = jointsDict[con.ID];
            //all joints in list should be the same!
            CConnectionJointTypes joint = list_joints.FirstOrDefault();
            //CConnectionJointTypes joint = list_joints.LastOrDefault();
            //joint.JointType = (EJointType)(con.ID);
            return joint;
        }
        private List<CConnectionJointTypes> GetSelectedJoints()
        {
            CConnectionDescription con = vm.JointTypes[vm.JointTypeIndex];
            return jointsDict[con.ID];
        }

        private DataGrid GetDatagridForAnchorArrangement(List<CComponentParamsView> anchorArrangementParams)
        {
            DataGrid dgAA = new DataGrid();
            dgAA.Name = "DatagridForAnchorArrangement";
            //dgAA.SetValue(Grid.RowProperty, 1);
            dgAA.ItemsSource = anchorArrangementParams;
            dgAA.HorizontalAlignment = HorizontalAlignment.Stretch;
            dgAA.AutoGenerateColumns = false;
            dgAA.IsEnabled = true;
            dgAA.IsReadOnly = false;
            dgAA.HeadersVisibility = DataGridHeadersVisibility.None;
            dgAA.SelectionMode = DataGridSelectionMode.Single;
            dgAA.SelectionUnit = DataGridSelectionUnit.Cell;

            DataGridTextColumn tc1 = new DataGridTextColumn();
            tc1.Binding = new Binding("Name");
            tc1.CellStyle = GetReadonlyCellStyle();
            tc1.IsReadOnly = true;
            tc1.Width = new DataGridLength(5.0, DataGridLengthUnitType.Star);
            dgAA.Columns.Add(tc1);

            DataGridTextColumn tc2 = new DataGridTextColumn();
            tc2.Binding = new Binding("ShortCut");
            tc2.CellStyle = GetReadonlyCellStyle();
            tc2.IsReadOnly = true;
            tc2.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dgAA.Columns.Add(tc2);

            DataGridTemplateColumn tc3 = new DataGridTemplateColumn();
            tc3.IsReadOnly = false;
            tc3.CellTemplate = GetDataTemplate();
            tc3.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dgAA.Columns.Add(tc3);

            DataGridTextColumn tc4 = new DataGridTextColumn();
            tc4.Binding = new Binding("Unit");
            tc4.CellStyle = GetReadonlyCellStyle();
            tc4.IsReadOnly = true;
            tc4.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dgAA.Columns.Add(tc4);

            foreach (CComponentParamsView cpw in anchorArrangementParams)
            {
                cpw.PropertyChanged += HandleAnchorArrangementComponentParamsViewPropertyChangedEvent;
            }

            return dgAA;
        }
        private void HandleAnchorArrangementComponentParamsViewPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is CComponentParamsView)) return;
            CComponentParamsView item = sender as CComponentParamsView;

            CConnectionJointTypes joint = GetSelectedJoint();

            CPlate plate = joint.m_arrPlates[vm.SelectedTabIndex];

            if (plate is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
                CPlateHelper.DataGridAnchorArrangement_ValueChanged(item, basePlate);
                List<CComponentParamsView> anchorArrangementParams = CPlateHelper.GetAnchorArrangementProperties(basePlate.AnchorArrangement);

                CPlateHelper.UpdatePlateScrewArrangementData(plate);

                if (anchorArrangementParams != null)
                {
                    StackPanel sp = vm.TabItems[vm.SelectedTabIndex].Content as StackPanel;
                    DataGrid dgAA = sp.Children[2] as DataGrid;
                    dgAA.ItemsSource = anchorArrangementParams;
                    foreach (CComponentParamsView cpw in anchorArrangementParams)
                    {
                        cpw.PropertyChanged += HandleAnchorArrangementComponentParamsViewPropertyChangedEvent;
                    }
                }
            }
            vm.ChangedAnchorArrangementParameter = item;


            //foreach (CConnectionJointTypes joint in list_joints)
            //{
            //    CPlate plate = joint.m_arrPlates[vm.SelectedTabIndex];

            //    if (plate is CConCom_Plate_B_basic)
            //    {
            //        CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
            //        CPlateHelper.DataGridAnchorArrangement_ValueChanged(item, basePlate);
            //        List<CComponentParamsView> anchorArrangementParams = CPlateHelper.GetAnchorArrangementProperties(basePlate.AnchorArrangement);

            //        CPlateHelper.UpdatePlateScrewArrangementData(plate);

            //        if (anchorArrangementParams != null)
            //        {
            //            //ScrollViewer sw = vm.TabItems[vm.SelectedTabIndex].Content as ScrollViewer;
            //            //StackPanel sp = sw.Content as StackPanel;
            //            StackPanel sp = vm.TabItems[vm.SelectedTabIndex].Content as StackPanel;
            //            DataGrid dgAA = sp.Children[2] as DataGrid;
            //            dgAA.ItemsSource = anchorArrangementParams;
            //            foreach (CComponentParamsView cpw in anchorArrangementParams)
            //            {
            //                cpw.PropertyChanged += HandleAnchorArrangementComponentParamsViewPropertyChangedEvent;
            //            }
            //        }
            //    }
            //}

            //vm.ChangedAnchorArrangementParameter = item;
            ////HandleJointsPropertyChangedEvent(sender, e);            
        }

        private DataGrid GetDatagridForScrewArrangement(List<CComponentParamsView> screwArrangementParams)
        {
            DataGrid dgSA = new DataGrid();
            dgSA.Name = "DatagridForScrewArrangement";
            //dgSA.SetValue(Grid.RowProperty, 1);
            dgSA.ItemsSource = screwArrangementParams;
            dgSA.HorizontalAlignment = HorizontalAlignment.Stretch;
            dgSA.AutoGenerateColumns = false;
            dgSA.IsEnabled = true;
            dgSA.IsReadOnly = false;
            dgSA.HeadersVisibility = DataGridHeadersVisibility.None;
            dgSA.SelectionMode = DataGridSelectionMode.Single;
            dgSA.SelectionUnit = DataGridSelectionUnit.Cell;

            DataGridTextColumn tc1 = new DataGridTextColumn();
            tc1.Binding = new Binding("Name");
            tc1.CellStyle = GetReadonlyCellStyle();
            tc1.IsReadOnly = true;
            tc1.Width = new DataGridLength(5.0, DataGridLengthUnitType.Star);
            dgSA.Columns.Add(tc1);

            DataGridTextColumn tc2 = new DataGridTextColumn();
            tc2.Binding = new Binding("ShortCut");
            tc2.CellStyle = GetReadonlyCellStyle();
            tc2.IsReadOnly = true;
            tc2.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dgSA.Columns.Add(tc2);

            DataGridTemplateColumn tc3 = new DataGridTemplateColumn();
            tc3.IsReadOnly = false;
            tc3.CellTemplate = GetDataTemplate();
            tc3.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dgSA.Columns.Add(tc3);

            DataGridTextColumn tc4 = new DataGridTextColumn();
            tc4.Binding = new Binding("Unit");
            tc4.CellStyle = GetReadonlyCellStyle();
            tc4.IsReadOnly = true;
            tc4.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dgSA.Columns.Add(tc4);

            foreach (CComponentParamsView cpw in screwArrangementParams)
            {
                cpw.PropertyChanged += HandleScrewArrangementComponentParamsViewPropertyChangedEvent;
            }

            return dgSA;
        }
        private void HandleScrewArrangementComponentParamsViewPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is CComponentParamsView)) return;
            CComponentParamsView item = sender as CComponentParamsView;

            CConnectionJointTypes joint = GetSelectedJoint();
            CPlate plate = joint.m_arrPlates[vm.SelectedTabIndex];
            CPlateHelper.DataGridScrewArrangement_ValueChanged(item, plate);
            List<CComponentParamsView> screwArrangementParams = CPlateHelper.GetScrewArrangementProperties(plate.ScrewArrangement);

            CPlateHelper.UpdatePlateScrewArrangementData(plate);

            if (screwArrangementParams != null)
            {
                StackPanel sp = vm.TabItems[vm.SelectedTabIndex].Content as StackPanel;

                int screwArrangementGridIndex = 2;
                if (plate is CConCom_Plate_B_basic) { screwArrangementGridIndex = 4; }

                DataGrid dgSA = sp.Children[screwArrangementGridIndex] as DataGrid;
                dgSA.ItemsSource = screwArrangementParams;
                foreach (CComponentParamsView cpw in screwArrangementParams)
                {
                    cpw.PropertyChanged += HandleScrewArrangementComponentParamsViewPropertyChangedEvent;
                }
            }
            vm.ChangedScrewArrangementParameter = item;
            

            //foreach (CConnectionJointTypes joint in list_joints)
            //{
            //    CPlate plate = joint.m_arrPlates[vm.SelectedTabIndex];
            //    CPlateHelper.DataGridScrewArrangement_ValueChanged(item, plate);
            //    List<CComponentParamsView> screwArrangementParams = CPlateHelper.GetScrewArrangementProperties(plate.ScrewArrangement);

            //    CPlateHelper.UpdatePlateScrewArrangementData(plate);                

            //    if (screwArrangementParams != null)
            //    {                    
            //        StackPanel sp = vm.TabItems[vm.SelectedTabIndex].Content as StackPanel;
            //        DataGrid dgSA = sp.Children[2] as DataGrid;
            //        dgSA.ItemsSource = screwArrangementParams;
            //        foreach (CComponentParamsView cpw in screwArrangementParams)
            //        {
            //            cpw.PropertyChanged += HandleScrewArrangementComponentParamsViewPropertyChangedEvent;
            //        }
            //    }
            //}

            //vm.ChangedScrewArrangementParameter = item;
            ////HandleJointsPropertyChangedEvent(sender, e);
        }
        private DataGrid GetDatagridForGeometry(List<CComponentParamsView> geometryParams)
        {
            DataGrid dg = new DataGrid();
            //dg.SetValue(Grid.RowProperty, 3);
            dg.ItemsSource = geometryParams;
            dg.HorizontalAlignment = HorizontalAlignment.Stretch;
            dg.AutoGenerateColumns = false;
            dg.IsEnabled = true;
            dg.IsReadOnly = false;
            dg.HeadersVisibility = DataGridHeadersVisibility.None;
            dg.SelectionMode = DataGridSelectionMode.Single;
            dg.SelectionUnit = DataGridSelectionUnit.Cell;

            DataGridTextColumn tc1 = new DataGridTextColumn();
            tc1.Binding = new Binding("Name");
            tc1.CellStyle = GetReadonlyCellStyle();
            tc1.IsReadOnly = true;
            tc1.Width = new DataGridLength(5.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc1);

            DataGridTextColumn tc2 = new DataGridTextColumn();
            tc2.Binding = new Binding("ShortCut");
            tc2.CellStyle = GetReadonlyCellStyle();
            tc2.IsReadOnly = true;
            tc2.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc2);

            DataGridTemplateColumn tc3 = new DataGridTemplateColumn();
            tc3.IsReadOnly = false;
            tc3.CellTemplate = GetDataTemplate();
            tc3.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc3);

            //DataGridTextColumn tc3 = new DataGridTextColumn();
            //tc3.Binding = new Binding("Value");            
            //Style style = new Style(typeof(TextBlock));
            //style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            //tc3.ElementStyle = style;
            //tc3.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            //dg.Columns.Add(tc3);

            DataGridTextColumn tc4 = new DataGridTextColumn();
            tc4.Binding = new Binding("Unit");
            tc4.CellStyle = GetReadonlyCellStyle();
            tc4.IsReadOnly = true;
            tc4.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc4);

            foreach (CComponentParamsView cpw in geometryParams)
            {
                cpw.PropertyChanged += HandleGeometryComponentParamsViewPropertyChangedEvent;
            }

            return dg;
        }
        private void HandleGeometryComponentParamsViewPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is CComponentParamsView)) return;
            CComponentParamsView item = sender as CComponentParamsView;
            foreach (CConnectionJointTypes joint in list_joints)
            {
                CPlate plate = joint.m_arrPlates[vm.SelectedTabIndex];
                CPlateHelper.DataGridGeometryParams_ValueChanged(item, plate);

                //ScrollViewer sw = vm.TabItems[vm.SelectedTabIndex].Content as ScrollViewer;
                //StackPanel sp = sw.Content as StackPanel;
                StackPanel sp = vm.TabItems[vm.SelectedTabIndex].Content as StackPanel;
                DataGrid dgGeometry = sp.Children[3] as DataGrid;
                DataGrid dgDetails = sp.Children[5] as DataGrid;
                List<CComponentParamsView> geometryParams = CPlateHelper.GetComponentProperties(plate);
                foreach (CComponentParamsView cpw in geometryParams)
                {
                    cpw.PropertyChanged += HandleGeometryComponentParamsViewPropertyChangedEvent;
                }
                dgGeometry.ItemsSource = geometryParams;
                dgDetails.ItemsSource = CPlateHelper.GetComponentDetails(plate);
            }
            vm.ChangedGeometryParameter = item;
            //HandleJointsPropertyChangedEvent(sender, e);
        }
        private DataGrid GetDatagridForDetails(List<CComponentParamsView> detailsParams)
        {
            DataGrid dg = new DataGrid();
            //dg.SetValue(Grid.RowProperty, 5);
            dg.ItemsSource = detailsParams;
            dg.HorizontalAlignment = HorizontalAlignment.Stretch;
            dg.AutoGenerateColumns = false;
            dg.IsEnabled = true;
            dg.IsReadOnly = true;
            dg.HeadersVisibility = DataGridHeadersVisibility.None;
            dg.SelectionMode = DataGridSelectionMode.Single;
            dg.SelectionUnit = DataGridSelectionUnit.Cell;

            DataGridTextColumn tc1 = new DataGridTextColumn();
            tc1.Binding = new Binding("Name");
            tc1.CellStyle = GetReadonlyCellStyle();
            tc1.IsReadOnly = true;
            tc1.Width = new DataGridLength(5.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc1);

            DataGridTextColumn tc2 = new DataGridTextColumn();
            tc2.Binding = new Binding("ShortCut");
            tc2.CellStyle = GetReadonlyCellStyle();
            tc2.IsReadOnly = true;
            tc2.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc2);

            DataGridTextColumn tc3 = new DataGridTextColumn();
            tc3.Binding = new Binding("Value");
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            tc3.ElementStyle = style;
            tc3.CellStyle = GetReadonlyCellStyle();
            tc3.IsReadOnly = true;
            tc3.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc3);

            DataGridTextColumn tc4 = new DataGridTextColumn();
            tc4.Binding = new Binding("Unit");
            tc4.CellStyle = GetReadonlyCellStyle();
            tc4.IsReadOnly = true;
            tc4.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc4);
            return dg;
        }
        private Style GetReadonlyCellStyle()
        {
            Style style = new Style(typeof(DataGridCell));
            style.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.WhiteSmoke)));
            style.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush(Colors.Black)));
            return style;
        }
        private DataTemplate GetDataTemplate()
        {
            DataTemplate retVal = null;

            var context = new ParserContext();
            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            string s = @"<DataTemplate><ContentControl Content='{Binding}'><ContentControl.Style><Style TargetType='ContentControl'><Style.Triggers>            
            <DataTrigger Binding='{Binding CheckType}' Value='CheckBox'>
            <Setter Property='ContentTemplate'>
            <Setter.Value>
            <DataTemplate>
            <CheckBox HorizontalAlignment='Center' IsChecked='{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' />
            </DataTemplate>
            </Setter.Value>
            </Setter>
            </DataTrigger>
            <DataTrigger Binding='{Binding CheckType}' Value='ComboBox' >
            <Setter Property='ContentTemplate'>
            <Setter.Value>
            <DataTemplate>
            <ComboBox HorizontalAlignment='Right' SelectedValue='{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' ItemsSource='{Binding Values, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' />
            </DataTemplate>
            </Setter.Value>
            </Setter>
            </DataTrigger>
            <DataTrigger Binding='{Binding CheckType}' Value='TextBox'>
            <Setter Property='ContentTemplate'>
            <Setter.Value>
            <DataTemplate>
            <TextBox TextAlignment='Right' Text='{Binding Value, Mode=TwoWay, UpdateSourceTrigger=LostFocus}' IsEnabled='{Binding IsEnabled}' />
            </DataTemplate>
            </Setter.Value>
            </Setter>
            </DataTrigger>
            </Style.Triggers>
            </Style>
            </ContentControl.Style>
            </ContentControl>
            </DataTemplate>";

            retVal = XamlReader.Parse(s, context) as DataTemplate;
            return retVal;
        }

        private DataGridTextColumn GetDataGridTextColumn()
        {
            DataGridTextColumn retVal = null;

            var context = new ParserContext();
            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            string s = @"<DataGridTextColumn Header='It3' Width='60' IsReadOnly='{Binding IsReadOnly}' Binding='{Binding Value, Mode=TwoWay}'>
            <DataGridTextColumn.ElementStyle><Style TargetType='{x:Type TextBlock}' ><Setter Property='HorizontalAlignment' Value='Right' /></Style>
            </DataGridTextColumn.ElementStyle></DataGridTextColumn>";

            retVal = XamlReader.Parse(s, context) as DataGridTextColumn;
            return retVal;
        }
        private void showAllJointsCount_Checked(object sender, RoutedEventArgs e)
        {
            CreateGridAndShowResultsCount();
        }
        private void CreateGridAndShowResultsCount()
        {
            List<TabItem> tabItems = new List<TabItem>();

            TabItem tab = new TabItem();
            tab.Header = "Joint types count";
            if (tab == null) return;
            ScrollViewer sw = new ScrollViewer();
            sw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            sw.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            StackPanel sp = new StackPanel();
            DataGrid dg = new DataGrid();
            dg.HeadersVisibility = DataGridHeadersVisibility.None;
            dg.MinWidth = 500;
            List<Tuple<int, string, string, int>> results = new List<Tuple<int, string, string, int>>();
            foreach (CConnectionDescription cd in vm.AllJointTypes)
            {
                Tuple<int, string, string, int> t = Tuple.Create(cd.ID, cd.Name, cd.JoinType, jointsDict[cd.ID].Count);
                results.Add(t);
            }

            dg.ItemsSource = results;
            sp.Children.Add(dg);
            sw.Content = sp;
            tab.Content = sw;

            tabItems.Add(tab);

            vm.TabItems = tabItems;
            vm.SelectedTabIndex = 0;
        }
        private void showAllJointsCount_Unchecked(object sender, RoutedEventArgs e)
        {
            SetDynamicTabs(vm);
        }
        private void displayJoint(CConnectionJointTypes joint)
        {
            if (joint == null) return; // Error - nothing to display

            sDisplayOptions = _pfdVM.GetDisplayOptions();
            //Here is the place to overwrite displayOptions from Main Model
            // TODO - refaktorovat s nastavenim zobrazenia footing pad preview
            sDisplayOptions.bDisplayGlobalAxis = false;
            sDisplayOptions.bDisplayMemberDescription = false;
            sDisplayOptions.bDisplayNodes = false;
            sDisplayOptions.bDisplayNodesDescription = false;
            sDisplayOptions.bDisplayMembersCenterLines = false;

            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayMembers = true;
            sDisplayOptions.bDisplayJoints = true;
            sDisplayOptions.bDisplayPlates = true;
            sDisplayOptions.bDisplayConnectors = true;

            CModel jointModel = Drawing3D.GetJointPreviewModel(joint, null, ref sDisplayOptions);

            CJointHelper.SetJoinModelRotationDisplayOptions(joint, ref sDisplayOptions);
            Page3Dmodel page1 = new Page3Dmodel(jointModel, sDisplayOptions, EModelType.eJoint);

            // Display model in 3D preview frame
            FrameJointPreview3D.Content = page1;
            FrameJointPreview3D.UpdateLayout();
        }

        private void FrameJointPreview3D_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!paramsChanged) return;

            MessageBox.Show("Changed joint params will be applied to the model.");

            foreach (List<CConnectionJointTypes> sameJoints in jointsDict.Values)
            {
                CConnectionJointTypes refJoint = sameJoints.FirstOrDefault();
                if (refJoint == null) continue;
                foreach (CConnectionJointTypes joint in sameJoints)
                {
                    joint.m_arrPlates = refJoint.m_arrPlates;
                }
            }

            if (_pfdVM.SynchronizeGUI) _pfdVM.SynchronizeGUI = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            paramsChanged = false;
        }
    }
}
