using BaseClasses;
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
using System.Xml;

namespace PFD
{
    /// <summary>
    /// Interaction logic for UC_MemberDesign.xaml
    /// </summary>
    public partial class UC_Joints : UserControl
    {        
        CModel_PFD Model;
        CJointsVM vm;
        Dictionary<int, List<CConnectionJointTypes>> jointsDict;
        List<CConnectionJointTypes> list_joints;

        public UC_Joints(CModel_PFD model)
        {
            InitializeComponent();
                        
            Model = model;
            
            vm = new CJointsVM();
            vm.PropertyChanged += HandleJointsPropertyChangedEvent;
            this.DataContext = vm;

            ArrangeConnectionJoints();

            vm.JointTypeIndex = 0;
        }
        
        protected void HandleJointsPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            CJointsVM vm = sender as CJointsVM;
            if (vm != null && vm.IsSetFromCode) return;

            if (e.PropertyName == "TabItems") return;
            if (e.PropertyName == "SelectedTabIndex") return;
            if (e.PropertyName == "JointTypeIndex") SetDynamicTabs(vm);
            


        }

        private void ArrangeConnectionJoints()
        {
            jointsDict = new Dictionary<int, List<CConnectionJointTypes>>();
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
            if (jointsDict == null) ArrangeConnectionJoints();
            List<TabItem> tabItems = new List<TabItem>();

            CConnectionDescription con = vm.JointTypes[vm.JointTypeIndex];
            list_joints = jointsDict[con.ID];
            //all joints in list sholud be the same!
            CConnectionJointTypes joint = list_joints.FirstOrDefault();

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

                        ScrollViewer sw = new ScrollViewer();
                        sw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                        sw.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        //sw.Width = 600;
                        StackPanel sp = new StackPanel();
                        sp.Width = 580;
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


                        if (plate.ScrewArrangement != null)
                        {                            
                            List<CComponentParamsView> screwArrangementParams = CPlateHelper.GetScrewArrangementProperties(plate.ScrewArrangement);
                            Label lSA = new Label() { Content = "Screw Arrangement: " };
                            //lSA.SetValue(Grid.RowProperty, 0);
                            sp.Children.Add(lSA);
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

                        sw.Content = sp;

                        ti.Content = sw;
                        tabItems.Add(ti);
                    }
                }                
            }
                        
            vm.TabItems = tabItems;
            vm.SelectedTabIndex = 0;
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

            //dgSA.SetBinding(DataGrid.ItemsSourceProperty, new Binding("ScrewArrangementParameters"));
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
                
                if (screwArrangementParams != null)
                {
                    ScrollViewer sw = vm.TabItems[vm.SelectedTabIndex].Content as ScrollViewer;
                    StackPanel sp = sw.Content as StackPanel;
                    DataGrid dgSA = sp.Children[1] as DataGrid;
                    dgSA.ItemsSource = screwArrangementParams;
                    foreach (CComponentParamsView cpw in screwArrangementParams)
                    {
                        cpw.PropertyChanged += HandleScrewArrangementComponentParamsViewPropertyChangedEvent;
                    }
                }
            }

            HandleJointsPropertyChangedEvent(sender, e);            
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

            DataGridTextColumn tc3 = new DataGridTextColumn();
            tc3.Binding = new Binding("Value");
            Style style = new Style(typeof(TextBlock));            
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
            tc3.ElementStyle = style;            
            tc3.IsReadOnly = false;
            tc3.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dg.Columns.Add(tc3);

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

                ScrollViewer sw = vm.TabItems[vm.SelectedTabIndex].Content as ScrollViewer;
                StackPanel sp = sw.Content as StackPanel;
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
            HandleJointsPropertyChangedEvent(sender, e);
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
            <TextBox TextAlignment='Right' Text='{Binding Value, Mode=TwoWay, UpdateSourceTrigger=LostFocus}' />
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
    }
}
