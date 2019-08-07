using BaseClasses;
using BaseClasses.Helpers;
using DATABASE.DTO;
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

            if (e.PropertyName == "ChangedScrewArrangementParameter" || e.PropertyName == "ChangedGeometryParameter")
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
            List<CConnectionDescription> listJointTypes = new List<CConnectionDescription>();
            foreach (CConnectionDescription c in vm.AllJointTypes)
            {
                List<CConnectionJointTypes> listFoundJointTyes = GetConnectionJointTypesFor(c);
                jointsDict.Add(c.ID, listFoundJointTyes);
                if (listFoundJointTyes.Count > 0) listJointTypes.Add(c);

                //----------------------------------------------------------------------------------------------------------------------------
                // TO Ondrej - pozri prosim na toto
                // Skusal som nastavovat uhly ale zistil som ze vobec nesedia indexy typov s tym co je v comboboxe
                // Ocakava sa ze typy v jointsDict a vm.JointTypes si koresponduju
                // Tu to vyzera tak ze sa zoznam zacne naplnat spravne, ale pri vystupe je to uz inak
                // prepisuju sa typy, napriklad TA001 je typ ktory ma edge column aj main colum,
                // Najprv sa pre index 1 zapise main column ale pre index 4 sa nastavi edge column a prepise sa aj typ s indexom 1
                // nesedia potom indexy typov v joint.Helper

                // TO Ondrej - kedze sme to vyriesili tak neviem ci chces tuto validaciu nechat aktivnu alebo si ju niekde presunut / "odlozit" pokial by sa to este v buducnosti hodilo

                //To Mato - ja tuto validaciu nepotrebujem - zakomentuvavam
                //// Validation - pokus odchytit zmenu v uz zapisanych zaznamoch
                //EJointType type = EJointType.eBase_MainColumn; // Hladany typ spoja, ktoreho zmenu sledujeme

                //List<CConnectionJointTypes> list = new List<CConnectionJointTypes>();
                //list = jointsDict[(int)type];

                //if (list[0].JointType != type) // Porovname typ prveho spoja ktory je zapisany v zozname dictionary so sledovanym typom
                //{
                //    throw new Exception("Original type: " + type + ", index No. " + (int)type + "\n"+
                //                        "New type: " + vm.JointTypes[c.ID-1].Name + " description ID: " + c.ID + ", object type: " + vm.JointTypes[c.ID - 1].JoinType);
                //}
                //----------------------------------------------------------------------------------------------------------------------------
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
                    new ArgumentNullException("Undefined main member of joint ID:" + joint.ID);
                }

                if (joint.m_MainMember.EMemberTypePosition <= 0)
                {
                    new ArgumentNullException("Undefined main member type of joint ID: " + joint.ID + ", member ID: " + joint.m_MainMember.ID);
                }

                if (joint.m_SecondaryMembers != null && joint.m_SecondaryMembers.Length > 0)
                {
                    for (int i = 0; i < joint.m_SecondaryMembers.Length; i++)
                    {
                        if (joint.m_SecondaryMembers[i].EMemberTypePosition <= 0)
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

            if (modelJointsCount != jointsIdentified)
            {
                System.Diagnostics.Trace.WriteLine($"JOINTS VALIDATION ERROR: Not all joints were identified. Identified joints count: [{jointsIdentified}]. Model joints count: [{modelJointsCount}]");
                
                //throw new Exception($"Not all joints were identified. Identified joints count: [{jointsIdentified}]. Model joints count: [{modelJointsCount}]");
            }
        }

        private List<CConnectionJointTypes> GetConnectionJointTypesFor(CConnectionDescription con)
        {
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

        private void SelectSA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CPlate plate = GetSelectedPlate();
            if (plate == null) return;
            CConnectionJointTypes joint = GetSelectedJoint();
            if (joint == null) return;

            ComboBox cbSA = sender as ComboBox;
            if (cbSA == null) return;
            ChangeAllSameJointsPlateScrewArrangement(cbSA.SelectedIndex);
            //CPlateHelper.ScrewArrangementChanged(joint, plate, cbSA.SelectedIndex);
            //CPlateHelper.UpdatePlateScrewArrangementData(plate);

            TabItem ti = vm.TabItems[vm.SelectedTabIndex];
            SetTabContent(ti, plate);

            displayJoint(joint);

            if (_pfdVM.SynchronizeGUI) _pfdVM.SynchronizeGUI = true;
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

        private DataGrid GetDatagridForScrewArrangement(List<CComponentParamsView> screwArrangementParams)
        {
            DataGrid dgSA = new DataGrid();
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
            foreach (CConnectionJointTypes joint in list_joints)
            {
                CPlate plate = joint.m_arrPlates[vm.SelectedTabIndex];
                CPlateHelper.DataGridScrewArrangement_ValueChanged(item, plate);
                List<CComponentParamsView> screwArrangementParams = CPlateHelper.GetScrewArrangementProperties(plate.ScrewArrangement);

                CPlateHelper.UpdatePlateScrewArrangementData(plate);                

                if (screwArrangementParams != null)
                {
                    //ScrollViewer sw = vm.TabItems[vm.SelectedTabIndex].Content as ScrollViewer;
                    //StackPanel sp = sw.Content as StackPanel;
                    StackPanel sp = vm.TabItems[vm.SelectedTabIndex].Content as StackPanel;
                    DataGrid dgSA = sp.Children[1] as DataGrid;
                    dgSA.ItemsSource = screwArrangementParams;
                    foreach (CComponentParamsView cpw in screwArrangementParams)
                    {
                        cpw.PropertyChanged += HandleScrewArrangementComponentParamsViewPropertyChangedEvent;
                    }
                }
            }

            vm.ChangedScrewArrangementParameter = item;
            //HandleJointsPropertyChangedEvent(sender, e);            
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
            sDisplayOptions.bDisplayGlobalAxis = true;
            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayPlates = true;
            sDisplayOptions.bDisplayConnectors = true;
            sDisplayOptions.bDisplayJoints = true;

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
    }
}
