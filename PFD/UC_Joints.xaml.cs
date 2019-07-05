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
    /// Interaction logic for UC_MemberDesign.xaml
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
            foreach (CConnectionDescription c in vm.JointTypes)
            {
                jointsDict.Add(c.ID, GetConnectionJointTypesFor(c));

                //----------------------------------------------------------------------------------------------------------------------------
                // TO - Ondrej - pozri prosim na toto
                // Skusal som nastavovat uhly ale zistil som ze vobec nesedia indexy typov s tym co je v comboboxe
                // Ocakava sa ze typy v jointsDict a vm.JointTypes si koresponduju
                // Tu to vyzera tak ze sa zoznam zacne naplnat spravne, ale pri vystupe je to uz inak
                // prepisuju sa typy, napriklad TA001 je typ ktory ma edge column aj main colum,
                // Najprv sa pre index 1 zapise main column ale pre index 4 sa nastavi edge column a prepise sa aj typ s indexom 1
                // nesedia potom indexy typov v joint.Helper

                // Validation - pokus odchytit zmenu v uz zapisanych zaznamoch
                EJointType type = EJointType.eBase_MainColumn; // Hladany typ spoja, ktoreho zmenu sledujeme

                List<CConnectionJointTypes> list = new List<CConnectionJointTypes>();
                list = jointsDict[(int)type];

                if (list[0].JointType != type) // Porovname typ prveho spoja ktory je zapisany v zozname dictionary so sledovanym typom
                {
                    throw new Exception("Original type: " + type + ", index No. " + (int)type + "\n"+
                                        "New type: " + vm.JointTypes[c.ID-1].Name + " description ID: " + c.ID + ", object type: " + vm.JointTypes[c.ID - 1].JoinType);
                }
                //----------------------------------------------------------------------------------------------------------------------------
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

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Validation
            // TODO - Ondrej - mozeme to presunut niekam inam a validovat to skor
            // Ucel validacie - Skontrolovat ci maju vsetky pruty v spojoch priradeny EMemberTypePosition

            foreach (CConnectionJointTypes joint in _pfdVM.Model.m_arrConnectionJoints)
            {
                if (joint.m_MainMember == null)
                {
                    new ArgumentNullException("Undefined main member of joint ID:" + joint.ID);
                }

                if(joint.m_MainMember.EMemberTypePosition <= 0)
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
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            foreach (CConnectionJointTypes joint in items)
            {
                if (joint.m_MainMember == null) continue;
                
                //To Mato - tu potrebudem cely tento switch skontrolovat/opravit/doplnit lebo ty tomu viac rozumies ako ja
                // Predbezne skontrolovane, ale este to budeme musiet poladit a podoplnat.
                // A) ak nie su v modeli okna alebo dvere tak niektore z tychto spojov by nemali byt v comboboxe
                // B) Je potrebne doplnit nejake typy spojov hlavne pre front a back girts (napriklad ak su v prednej alebo zadnej stene vlozene otvory)

                // To Ondrej Bug 328, door trimmer to girt, main member - girt ma EMemberPositionType = 0, takze spoj sa nedetekuje spravne
                // Musime si byt isti ze vsetky pruty maju nastaveny MemberPositionType, inak je nieco zle
                if (joint.m_SecondaryMembers != null && joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                {
                    throw new Exception("Main Member Type: " + joint.m_MainMember.EMemberType + ", Main Member Position Type: " + joint.m_MainMember.EMemberTypePosition);
                }

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
            //all joints in list sholud be the same!
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
            foreach (CConnectionDescription cd in vm.JointTypes)
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

            CConnectionJointTypes jointClone = joint.Clone();
            sDisplayOptions = _pfdVM.GetDisplayOptions();
            //Here is the place to overwrite displayOptions from Main Model
            sDisplayOptions.bDisplayGlobalAxis = true;
            sDisplayOptions.bDisplaySolidModel = true;
            sDisplayOptions.bDisplayPlates = true;
            sDisplayOptions.bDisplayConnectors = true;
            sDisplayOptions.bDisplayJoints = true;

            // TO Ondrej - tuto funkciu treba trosku ucesat, refaktorovat casti kodu pre Main Member a cast pre Secondary Members

            // Problem 1 - ani jeden z uzlov pruta, ktore patria ku joint nekonci v spoji (vyskytuje sa najma pre main member, napr purlin pripojena k rafter)
            // TODO - problem je u main members ak nekoncia v uzle spoja
            // TODO - potrebujeme vytvorit funkciu (Drawing3D.cs PointLiesOnLineSegment), ktora najde vsetky "medzilahle" uzly, ktore lezia na prute, naplni nejaky zoznam uzlov v objekte pruta (List<CNode>IntermediateNodes)
            // TODO - potom vieme pre Main Member zistit, ktory z tychto uzlov je joint Node a vykreslit segment main member na jednu a na druhu stranu od tohto uzla

            // Problem 2 - joint nema spravne definovany main member (definovany je napr. ako main member prut rovnakeho typu s najnizsim ID)
            // TODO - vyssie uvedeny zoznam medzilahlych uzlov na prute vieme pouzit aj na to ze ak Main member nie je skutocny main member prisluchajuci ku spoju ale len prvy prut rovnakeho typu, 
            // tak mozeme najst taky prut, ktory ma v zozname IntermediateNodes joint.m_Node
            // a zaroven je rovnakeho typu ako main member, to by mal byt skutocny main member, ktory patri k joint.m_Node a mozeme ho nahradit
            // tento problem by sme mali riesit uz niekde pred touto funkciou, idealne uz pri vytvarani spojov v CModel_PFD_01_GR.cs

            float fMainMemberLength = 0;
            float fSecondaryMemberLength = 0;

            for (int i = 0; i < jointClone.m_arrPlates.Length; i++)
            {
                fMainMemberLength = Math.Max(jointClone.m_arrPlates[i].Width_bx, jointClone.m_arrPlates[i].Height_hy);
                fSecondaryMemberLength = fMainMemberLength;
            }

            float fMainMemberLengthFactor = 0.9f; // Upravi dlzku urcenu z maximalneho rozmeru plechu
            float fSecondaryMemberLengthFactor = 0.9f; // Upravi dlzku urcenu z maximalneho rozmeru  // Bug 320 - Musi byt rovnake ako main member kvoli plechu Apex - jeden rafter je main, jeden je secondary

            fMainMemberLength *= fMainMemberLengthFactor;
            fSecondaryMemberLength *= fSecondaryMemberLengthFactor;

            CModel jointModel = new CModel();

            //jointModel.m_arrConnectionJoints = new List<CConnectionJointTypes>() { joint };

            int iNumberMainMembers = 0;
            int iNumberSecondaryMembers = 0;

            if (jointClone.m_MainMember != null)
                iNumberMainMembers = 1;

            if (jointClone.m_SecondaryMembers != null)
                iNumberSecondaryMembers = jointClone.m_SecondaryMembers.Length;

            jointModel.m_arrMembers = new CMember[iNumberMainMembers + iNumberSecondaryMembers];

            // Main Member
            if (jointClone.m_MainMember != null)
            {
                CMember m = jointClone.m_MainMember;
                
                CNode nodeJoint = jointClone.m_Node; // Joint Node
                CNode nodeOtherEnd;             // Volny uzol na druhej strane pruta
                float fX;
                float fY;
                float fZ;

                if (jointClone.m_Node.ID == m.NodeStart.ID)
                {
                    nodeOtherEnd = m.NodeEnd;
                    m.FAlignment_End = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany

                    fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                    fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                    fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                    v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)v.X;
                    fY = (float)v.Y;
                    fZ = (float)v.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    nodeOtherEnd.X = nodeJoint.X + fX * fMainMemberLength;
                    nodeOtherEnd.Y = nodeJoint.Y + fY * fMainMemberLength;
                    nodeOtherEnd.Z = nodeJoint.Z + fZ * fMainMemberLength;
                }
                else if (jointClone.m_Node.ID == m.NodeEnd.ID)
                {
                    nodeOtherEnd = m.NodeStart;
                    m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany

                    fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                    fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                    fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                    v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)v.X;
                    fY = (float)v.Y;
                    fZ = (float)v.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    nodeOtherEnd.X = nodeJoint.X + fX * fMainMemberLength;
                    nodeOtherEnd.Y = nodeJoint.Y + fY * fMainMemberLength;
                    nodeOtherEnd.Z = nodeJoint.Z + fZ * fMainMemberLength;
                }
                else
                {
                    fMainMemberLength *= 2; // Zdvojnasobime vykreslovanu dlzku pruta kedze vykreslujeme na 2 strany od nodeJoint

                    // Relativny priemet casti pruta medzi zaciatocnym uzlom a uzlom spoja do GCS
                    fX = (m.NodeStart.X - nodeJoint.X) / m.FLength;
                    fY = (m.NodeStart.Y - nodeJoint.Y) / m.FLength;
                    fZ = (m.NodeStart.Z - nodeJoint.Z) / m.FLength;

                    // TO Ondrej - ak je prut velmi dlhy a fX az fZ su velmi male cisla, tak pre pripad ze jointNode je blizko k Start alebo End Node hlavneho pruta
                    // vyjde vzdialenost (fX, resp. fY, fZ) * fMainMemberLength velmi mala
                    // Urobil som to tak ze urcim vektor z absolutnych dlzok priemetu a potom ho normalizujem, takze absolutna vzdialenost priemetu nodeJoint a m.NodeStart, resp. m.NodeEnd
                    // by nemala hrat rolu, mozes sa na to pozriet. Mozno Ta napadne nieco elegantnejsie, rozdiel vidiet napriklad pri spoji girt to Main column

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D vStart = new Vector3D(m.NodeStart.X - nodeJoint.X, m.NodeStart.Y - nodeJoint.Y, m.NodeStart.Z - nodeJoint.Z);
                    vStart.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)vStart.X;
                    fY = (float)vStart.Y;
                    fZ = (float)vStart.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    // Nastavenie novych suradnic - zaciatok skrateneho (orezaneho) pruta
                    m.NodeStart.X = nodeJoint.X + fX * fMainMemberLength / 2;
                    m.NodeStart.Y = nodeJoint.Y + fY * fMainMemberLength / 2;
                    m.NodeStart.Z = nodeJoint.Z + fZ * fMainMemberLength / 2;

                    // Relativny priemet casti pruta medzi uzlom spoja a koncovym uzlom do GCS
                    fX = (m.NodeEnd.X - nodeJoint.X) / m.FLength;
                    fY = (m.NodeEnd.Y - nodeJoint.Y) / m.FLength;
                    fZ = (m.NodeEnd.Z - nodeJoint.Z) / m.FLength;

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D vEnd = new Vector3D(m.NodeEnd.X - nodeJoint.X, m.NodeEnd.Y - nodeJoint.Y, m.NodeEnd.Z - nodeJoint.Z);
                    vEnd.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)vEnd.X;
                    fY = (float)vEnd.Y;
                    fZ = (float)vEnd.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    // Nastavenie novych suradnic - koniec skrateneho (orezaneho) pruta
                    m.NodeEnd.X = nodeJoint.X + fX * fMainMemberLength / 2;
                    m.NodeEnd.Y = nodeJoint.Y + fY * fMainMemberLength / 2;
                    m.NodeEnd.Z = nodeJoint.Z + fZ * fMainMemberLength / 2;

                    m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                    m.FAlignment_End = 0;   // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                }
                
                m.Fill_Basic();

                jointModel.m_arrMembers[0] = m; // Set new member (member array)
                jointClone.m_MainMember = m; // Set new member (joint)                
            }

            // Secondary members
            if (jointClone.m_SecondaryMembers != null)
            {
                for (int i = 0; i < jointClone.m_SecondaryMembers.Length; i++)
                {
                    CMember m = jointClone.m_SecondaryMembers[i];

                    CNode nodeJoint = jointClone.m_Node; // Joint Node
                    CNode nodeOtherEnd;             // Volny uzol na druhej strane pruta

                    if (jointClone.m_Node.ID == m.NodeStart.ID)
                    {
                        nodeOtherEnd = m.NodeEnd;
                        m.FAlignment_End = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                    }
                    else
                    {
                        nodeOtherEnd = m.NodeStart;
                        m.FAlignment_Start = 0; // Nastavime nulove odsadenie, aby nebol volny koniec pruta orezany
                    }

                    float fX = (nodeOtherEnd.X - nodeJoint.X) / m.FLength;
                    float fY = (nodeOtherEnd.Y - nodeJoint.Y) / m.FLength;
                    float fZ = (nodeOtherEnd.Z - nodeJoint.Z) / m.FLength;

                    //-------------------------------------------------------------------------------------------------------------------------------
                    // TODO - Pokus vyriesit prilis kratke pruty
                    Vector3D v = new Vector3D(nodeOtherEnd.X - nodeJoint.X, nodeOtherEnd.Y - nodeJoint.Y, nodeOtherEnd.Z - nodeJoint.Z);
                    v.Normalize(); // Normalizujeme vektor priemetov, aby dlzky neboli prilis male
                    fX = (float)v.X;
                    fY = (float)v.Y;
                    fZ = (float)v.Z;
                    //-------------------------------------------------------------------------------------------------------------------------------

                    nodeOtherEnd.X = nodeJoint.X + fX * fSecondaryMemberLength;
                    nodeOtherEnd.Y = nodeJoint.Y + fY * fSecondaryMemberLength;
                    nodeOtherEnd.Z = nodeJoint.Z + fZ * fSecondaryMemberLength;

                    m.Fill_Basic();

                    jointModel.m_arrMembers[1 + i] = m; // Set new member (member array)
                    jointClone.m_SecondaryMembers[i] = m; // Set new member (joint)
                }
            }
            
            List<CNode> nodeList = new List<CNode>();

            for (int i = 0; i < jointModel.m_arrMembers.Length; i++)
            {
                // Pridavat len uzly ktore este neboli pridane
                if (nodeList.IndexOf(jointModel.m_arrMembers[i].NodeStart) == -1) nodeList.Add(jointModel.m_arrMembers[i].NodeStart);
                if (nodeList.IndexOf(jointModel.m_arrMembers[i].NodeEnd) == -1) nodeList.Add(jointModel.m_arrMembers[i].NodeEnd);
            }
            jointModel.m_arrNodes = nodeList.ToArray();

            // Bug - TODO - TO Ondrej - skusal som nastavovat uhly a zistil som ze je nejaky problem s joint a jointClone, nemaju rovnaky typ
            // Skusil som to opravit, tak sa na to pozri a zmaz tento komentar.

            jointClone = joint.RecreateJoint();
            jointClone.m_arrPlates = joint.m_arrPlates;

            jointModel.m_arrConnectionJoints = new List<CConnectionJointTypes>() { jointClone };

            CJointHelper.SetJoinModelRotationDisplayOptions(joint, ref sDisplayOptions); // TODO Ondrej - posiela sa sem joint, ale ten nema ocakavany typ !!! Skusil som to opravit, tak sa na to pozri a zmaz tento komentar.
            Page3Dmodel page1 = new Page3Dmodel(jointModel, sDisplayOptions);

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
