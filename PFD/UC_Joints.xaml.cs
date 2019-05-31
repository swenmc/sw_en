using BaseClasses;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_MemberDesign.xaml
    /// </summary>
    public partial class UC_Joints : UserControl
    {        
        CModel_PFD Model;
        CJointsVM vm;


        public UC_Joints(CModel_PFD model)
        {
            InitializeComponent();
                        
            Model = model;
            
            vm = new CJointsVM();
            vm.PropertyChanged += HandleJointsPropertyChangedEvent;
            this.DataContext = vm;

            vm.JointTypeIndex = 1;
            ArrangeConnectionJoints();
        }
        protected void HandleJointsPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CJointsVM vm = sender as CJointsVM;
            if (vm != null && vm.IsSetFromCode) return;

            if (e.PropertyName == "TabItems") return;
            if (e.PropertyName == "JointTypeIndex") SetDynamicTabs(vm);

            //int count = 0;
            //foreach (CConnectionJointTypes joint in Model.m_arrConnectionJoints)
            //{
            //    if (joint is CConnectionJoint_A001) count++;
            //}
            //MessageBox.Show($"Count: {count}");



        }

        private void ArrangeConnectionJoints()
        {
            Dictionary<int, List<CConnectionJointTypes>> jointsDict = new Dictionary<int, List<CConnectionJointTypes>>();
            foreach (CConnectionDescription c in vm.JointTypes)
            {
                jointsDict.Add(c.ID, GetConnectionJointTypesFor(c));
            }
            
            

        }

        private List<CConnectionJointTypes> GetConnectionJointTypesFor(CConnectionDescription con)
        {
            List<CConnectionJointTypes> items = Model.m_arrConnectionJoints.FindAll(c => c.GetType() == GetTypeFor(con.JoinType));
            List<CConnectionJointTypes> resItems = new List<CConnectionJointTypes>();

            foreach (CConnectionJointTypes joint in items)
            {
                if (joint.m_MainMember == null) continue;

                //TO MAto - tu potrebudem cely tento switch skontrolovat/opravit/doplnit lebo ty tomu viac rozumies ako ja
                switch (con.ID)
                {
                    //1   Base - main column
                    case 1:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainColumn)
                            resItems.Add(joint);
                        break;
                    //2   Knee - main rafter to column
                    case 2:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.MainRafter &&
                            joint.m_SecondaryMembers != null && 
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.MainColumn ||
                                joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeColumn)
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
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn)
                            resItems.Add(joint);
                        break;
                    //5   Knee - edge rafter to column
                    case 5:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeColumn)
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
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Purlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.MainRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //8   Purlin to edge rafter
                    case 8:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Purlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //9   Purlin to main rafter -fly bracing
                    case 9:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Purlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.MainRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //10  Purlin to edge rafter -fly bracing
                    case 10:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Purlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //11  Edge purlin to main rafter
                    case 11:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgePurlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.MainRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //12  Edge purlin to edge rafter
                    case 12:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.EdgePurlin &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //13  Girt to main column
                    case 13:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Girt &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.MainColumn)
                            )
                            resItems.Add(joint);
                        break;
                    //14  Girt to edge column
                    case 14:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Girt &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeColumn)
                            )
                            resItems.Add(joint);
                        break;
                    //15  Base - wind post - front
                    case 15:

                        break;
                    //16  Base - wind post - back
                    case 16:
                        break;
                    //17  Wind post to edge rafter - front
                    case 17:
                        break;
                    //18  Wind post to edge rafter - back
                    case 18:
                        break;
                    //19  Girt to edge column -front
                    case 19:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.Girt &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeColumn)
                            )
                            resItems.Add(joint);
                        break;
                    //20  Girt to edge column -back
                    case 20:
                        break;
                    //21  Girt to wind post -front
                    case 21:
                        break;
                    //22  Girt to wind post -back
                    case 22:
                        break;
                    //23  Base - door trimmer
                    case 23:
                        break;
                    //24  Door trimmer to girt
                    case 24:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Girt)
                            )
                            resItems.Add(joint);
                        break;
                    //25  Door trimmer to edge pulin
                    case 25:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgePurlin)
                            )
                            resItems.Add(joint);
                        break;
                    //26  Door trimmer to edge rafter
                    case 26:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //27  Door lintel to trimmer
                    case 27:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorLintel &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                            )
                            resItems.Add(joint);
                        break;
                    //28  Base - door frame
                    case 28:
                        break;
                    //29  Door frame to girt
                    case 29:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Girt)
                            )
                            resItems.Add(joint);
                        break;
                    //30  Door frame to edge pulin
                    case 30:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgePurlin)
                            )
                            resItems.Add(joint);
                        break;
                    //31  Door frame to edge rafter
                    case 31:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //32  Door frame lintel to trimmer
                    case 32:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.DoorLintel &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                            )
                            resItems.Add(joint);
                        break;
                    //33  Window frame to girt
                    case 33:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.WindowFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.Girt)
                            )
                            resItems.Add(joint);
                        break;
                    //34  Window frame to edge pulin
                    case 34:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.WindowFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgePurlin)
                            )
                            resItems.Add(joint);
                        break;
                    //35  Window frame to edge rafter
                    case 35:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.WindowFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.EdgeRafter)
                            )
                            resItems.Add(joint);
                        break;
                    //36  Window frame lintel to trimmer
                    case 36:
                        if (joint.m_MainMember.EMemberTypePosition == EMemberType_FS_Position.WindowFrame &&
                            joint.m_SecondaryMembers != null &&
                                (joint.m_SecondaryMembers[0].EMemberTypePosition == EMemberType_FS_Position.DoorTrimmer)
                            )
                            resItems.Add(joint);
                        break;
                } //end switch
            } //end foreach

            return resItems;
        }
        private Type GetTypeFor(string strType)
        {
            switch (strType)
            {
                case "A001": return typeof(CConnectionJoint_A001);
                case "B001": return typeof(CConnectionJoint_B001);
                case "C001": return typeof(CConnectionJoint_C001);
                case "CT01": return typeof(CConnectionJoint_CT01);
                case "D001": return typeof(CConnectionJoint_D001);
                case "E001": return typeof(CConnectionJoint_E001);
                case "J001": return typeof(CConnectionJoint_J001);
                case "L001": return typeof(CConnectionJoint_L001);
                case "S001": return typeof(CConnectionJoint_S001);
                case "T001": return typeof(CConnectionJoint_T001);
                case "T002": return typeof(CConnectionJoint_T002);
                case "T003": return typeof(CConnectionJoint_T003);
                case "TA01": return typeof(CConnectionJoint_TA01);
                case "TB01": return typeof(CConnectionJoint_TB01);                
                default:

                    //temp it should throw exceoton if there is not recognized joint type
                    return null;
                    //throw new Exception($"Type of connection joint [{strType}] not recognized. (Method GetTypeFor)");
            }
        } 

        private void SetDynamicTabs(CJointsVM vm)
        {
            List<TabItem> tabItems = new List<TabItem>();
            TabItem t1 = new TabItem();
            t1.Header = "T1 header";

            TabItem t2 = new TabItem();
            t2.Header = "T2 header";

            TabItem t3 = new TabItem();
            t3.Header = "T3 header";
            if(vm.JointTypeIndex >= 0) tabItems.Add(t1);
            if (vm.JointTypeIndex >= 1) tabItems.Add(t2);
            if (vm.JointTypeIndex >= 2) tabItems.Add(t3);
            vm.TabItems = tabItems;
        }

        
    }
}
