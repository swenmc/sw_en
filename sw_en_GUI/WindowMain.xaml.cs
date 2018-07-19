using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using WPF.MDI;
using SharedLibraries.EXPIMP;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using BaseClasses;
using CRSC;
using FEM_CALC_BASE;
using FEM_CALC_1Din2D;
using FEM_CALC_1Din3D;
using _3DTools;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        private CModel model = null;
        private bool m_bDebugging = false; // Console Output

        List<Trackport3D> list_trackports;
        public WindowMain()
        {
            InitializeComponent();

            ImageIzometric.Source = (ImageSource)TryFindResource("Izometric");
            ImagePerspective.Source = (ImageSource)TryFindResource("Perspective");
            ImageViewX.Source = (ImageSource)TryFindResource("ViewX");
            ImageViewY.Source = (ImageSource)TryFindResource("ViewY");
            ImageViewZ.Source = (ImageSource)TryFindResource("ViewZ");

            // 12.6.2013
            Container.Children.CollectionChanged += (o, e) => Menu_RefreshWindows();
            // Create list of model windows
            list_trackports = new List<Trackport3D>();

            /*
			Container.Children.CollectionChanged += (o, e) => Menu_RefreshWindows();
			Window2 win = new Window2();
			list_trackports = new List<Trackport3D>();
			list_trackports.Add(win._trackport);

			Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Window " + (Container.Children.Count + 1) });
            */

        }

        /// <summary>
        /// Refresh windows list
        /// </summary>
        void Menu_RefreshWindows()
        {
            WindowsMenu.Items.Clear();
            MenuItem mi;
            for (int i = 0; i < Container.Children.Count; i++)
            {
                MdiChild child = Container.Children[i];
                mi = new MenuItem { Header = child.Title };
                mi.Click += (o, e) => child.Focus();
                WindowsMenu.Items.Add(mi);
            }
            WindowsMenu.Items.Add(new Separator());
            WindowsMenu.Items.Add(mi = new MenuItem { Header = "Cascade" });
            mi.Click += (o, e) => Container.MdiLayout = MdiLayout.Cascade;
            WindowsMenu.Items.Add(mi = new MenuItem { Header = "Horizontally" });
            mi.Click += (o, e) => Container.MdiLayout = MdiLayout.TileHorizontal;
            WindowsMenu.Items.Add(mi = new MenuItem { Header = "Vertically" });
            mi.Click += (o, e) => Container.MdiLayout = MdiLayout.TileVertical;

            WindowsMenu.Items.Add(new Separator());
            WindowsMenu.Items.Add(mi = new MenuItem { Header = "Close all" });
            mi.Click += (o, e) => Container.Children.Clear();
        }

        private void menuItemNew_Click(object sender, RoutedEventArgs e)
        {
            // Start Window
            WindowStart st = new WindowStart();
            st.Show();

            // Model Window
            Window2 win = new Window2(m_bDebugging);
            list_trackports.Add(win._trackport);

            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Window " + (Container.Children.Count + 1) });
            win.Close();            
        }

        private void menuItemSave_Click(object sender, RoutedEventArgs e)
        {
            if (model != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ".cnx";
                sfd.Filter = "Cenex binary documents (.cnx)|*.cnx";
                if (sfd.ShowDialog() == true)
                {
                    try
                    {
                        string filename = sfd.FileName;
                        FileStream fs = new FileStream(filename, FileMode.Create);
                        // Create a BinaryFormatter object to perform the serialization
                        BinaryFormatter bf = new BinaryFormatter();
                        // Use the BinaryFormatter object to serialize the data to the file
                        bf.Serialize(fs, model);
                        // Close the file
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        if (!EventLog.SourceExists("sw_en"))
                        {
                            EventLog.CreateEventSource("sw_en", "Application");
                        }
                        EventLog.WriteEntry("sw_en", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
                    }
                }
            }
            else
            {
                // Neinicializovany objekt vypoctoveho modelu
                MessageBox.Show("Model not initialized. Import data first!!!");
            }
        }

        private void menuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".cnx";
            ofd.Filter = "Cenex binary documents (.cnx)|*.cnx";
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    // Open file from which to read the data
                    FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                    // Create a BinaryFormatter object to perform the deserialization
                    BinaryFormatter bf = new BinaryFormatter();
                    // Create the object to store the deserialized data
                    model = (CModel)bf.Deserialize(fs);
                    // Close the file
                    fs.Close();
                }
                catch (Exception ex)
                {
                    if (!EventLog.SourceExists("sw_en"))
                    {
                        EventLog.CreateEventSource("sw_en", "Application");
                    }
                    EventLog.WriteEntry("sw_en", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
                }
            }
        }

        private void menuItemView3Dview_Click(object sender, RoutedEventArgs e)
        {
            //Window2 win2 = new Window2(m_bDebugging);
            //win2.ShowDialog();
            Window1 win1 = new Window1();
            win1.ShowDialog();
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".xlsx";
            ofd.Filter = "Excel documents(.xlsx, .xls)|*.xlsx;*.xls";
            if (ofd.ShowDialog() == true)
            {
                // Osetrit nacitavanie dat z prazdnych naformatovanych riadkov a pod
                try
                {
                    DataSet ds = CImportFromExcel.ImportFromExcel(ofd.FileName);
                    if (ds == null)
                    {
                        MessageBox.Show(string.Format("Error occurs. File {0} can not be read", ofd.FileName));
                        return;
                    }
                    dataGridNodes.ItemsSource = ds.Tables[0].DefaultView;
                    dataGridNodes.DataContext = ds;
                    dataGridMaterials.ItemsSource = ds.Tables[1].DefaultView;
                    dataGridMaterials.DataContext = ds;
                    dataGridCrossSections.ItemsSource = ds.Tables[2].DefaultView;
                    dataGridCrossSections.DataContext = ds;
                    dataGridMemberReleases.ItemsSource = ds.Tables[3].DefaultView;
                    dataGridMemberReleases.DataContext = ds;
                    dataGridMemberEccentricities.ItemsSource = ds.Tables[4].DefaultView;
                    dataGridMemberEccentricities.DataContext = ds;
                    dataGridMemberDivisions.ItemsSource = ds.Tables[5].DefaultView;
                    dataGridMemberDivisions.DataContext = ds;
                    dataGridNodalSupport.ItemsSource = ds.Tables[6].DefaultView;
                    dataGridNodalSupport.DataContext = ds;
                    //dataGridMemberElasticFoundations.ItemsSource = ds.Tables[7].DefaultView;
                    dataGridMemberElasticFoundations.DataContext = ds;
                    initModelObject(ofd.FileName);
                }
                catch (Exception ex)
                {
                    if (!EventLog.SourceExists("sw_en"))
                    {
                        EventLog.CreateEventSource("sw_en", "Application");
                    }
                    EventLog.WriteEntry("sw_en", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
                }
            }
        }

        private void initModelObject(string fileName)
        {
            model = new CModel(fileName);
            model.m_arrNodes = getNodes(((DataSet)dataGridNodes.DataContext).Tables[0]);
            //model.m_arrMat materials are in CENEX project  Class should be moved to BaseClassess project. 
            model.m_arrCrSc = getCrossSections(((DataSet)dataGridNodes.DataContext).Tables[2]);
            //model.m_arrNReleases 
            //member Eccentricities  
            model.m_arrMembers = getMembers(((DataSet)dataGridNodes.DataContext).Tables[5]);

            // Set properties to the Member (nodes, crsc)

            for (int i = 0; i < model.m_arrMembers.Length; i++)
            {
                // Nodes
                model.m_arrMembers[i].NodeStart = model.m_arrNodes[model.m_arrMembers[i].NodeStart.ID - 1];
                model.m_arrMembers[i].NodeEnd = model.m_arrNodes[model.m_arrMembers[i].NodeEnd.ID - 1];

                // Set Cross-section
                model.m_arrMembers[i].CrScStart = model.m_arrCrSc[model.m_arrMembers[i].CrScStart.ICrSc_ID - 1];

                // Temp - nacitava sa z tabulky alebo z databazy, dopracovat
                // Parametre pre IPN 300
                //model.m_arrMembers[i].CrSc = new CCrSc_3_00(0, 8, 0.300f, 0.125f, 0.0162f, 0.0108f, 0.0108f, 0.0065f, 0.2416f); // Example - FEM calculation 3D
                model.m_arrMembers[i].CrScStart = new CCrSc_0_05(0.1f, 0.05f); // Temp
                model.m_arrMembers[i].CrScStart.I_t = 5.69e-07f;
                model.m_arrMembers[i].CrScStart.I_y = 9.79e-05f;
                model.m_arrMembers[i].CrScStart.I_z = 4.49e-06f;
                model.m_arrMembers[i].CrScStart.A_g = 6.90e-03f;
                model.m_arrMembers[i].CrScStart.A_vy = 4.01e-03f;
                model.m_arrMembers[i].CrScStart.A_vz = 2.89e-03f;
            }

            model.m_arrNSupports = getNSupports(((DataSet)dataGridNodes.DataContext).Tables[6]);
        }

        private CNode[] getNodes(DataTable dt)
        {
            List<CNode> nodes = new List<CNode>();
            CNode node = null;

            int Node_ID;
            float Coord_X;
            float Coord_Y;
            float Coord_Z;
            int fTime = 100;

            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    int.TryParse(row["NodeID"].ToString(), out Node_ID);
                    float.TryParse(row["NodeCoordinateX"].ToString(), out Coord_X);
                    float.TryParse(row["NodeCoordinateY"].ToString(), out Coord_Y);
                    float.TryParse(row["NodeCoordinateZ"].ToString(), out Coord_Z);

                    node = new CNode(Node_ID, Coord_X, Coord_Y, Coord_Z, fTime);
                    nodes.Add(node);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            //foreach (CNode n in nodes) 
            //{
            //    Console.WriteLine(string.Format("{0},{1},{2},{3},{4}", 
            //        n.ID, n.FCoord_X, n.FCoord_Y, n.FCoord_Z, n.FTime));
            //}
            return nodes.ToArray();
        }

        private CMember[] getMembers(DataTable dt)
        {
            List<CMember> members = new List<CMember>();
            CMember member = null;

            int Line_ID;
            CNode Node1;
            int node1ID;
            CNode Node2;
            int node2ID;
            float fRotation;
            CCrSc_3_00 CrSc1; // Temp
            int iCrSc1ID;
            CCrSc_3_00 CrSc2; // Temp
            int iCrSc2ID;
            // ReleaseStartID
            // ReleaseEndID
            int Time = 100;
            float Length;

            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    // Nodes
                    int.TryParse(row["MemberID"].ToString(), out Line_ID);
                    if (Line_ID == 0) continue; //do not include empty MemberID or with memberID=0

                    Node1 = new CNode();
                    int.TryParse(row["NodeStartID"].ToString(), out node1ID);
                    Node1.ID = node1ID;

                    Node2 = new CNode();
                    int.TryParse(row["NodeEndID"].ToString(), out node2ID);
                    Node2.ID = node2ID;

                    //Cross-sections
                    CrSc1 = new CRSC.CCrSc_3_00();
                    int.TryParse(row["CrossSectionStartID"].ToString(), out iCrSc1ID);
                    CrSc1.ICrSc_ID = iCrSc1ID;

                    CrSc2 = new CRSC.CCrSc_3_00();
                    int.TryParse(row["CrossSectionEndID"].ToString(), out iCrSc2ID);
                    CrSc2.ICrSc_ID = iCrSc2ID;

                    // Create member
                    member = new CMember(Line_ID, Node1, Node2, CrSc1, Time);

                    float.TryParse(row["Length"].ToString(), out Length);
                    member.FLength = Length;

                    members.Add(member);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            foreach (CMember m in members)
            {
                Console.WriteLine(string.Format("IMember_ID: {0},NodeStart.ID: {1},NodeEnd.ID: {2},m.FLength: {3}",
                    m.ID, m.NodeStart.ID, m.NodeEnd.ID, m.FLength));
            }
            return members.ToArray();
        }

        private CCrSc[] getCrossSections(DataTable dt)
        {
            List<CCrSc> list_crsc = new List<CCrSc>();
            CCrSc crsc = null;

            int CrSc_ID;
            float fI_t, fI_y, fI_z, fA_g, fA_vy, fA_vz;

            foreach (DataRow row in dt.Rows)
            {
                try
                {

                    //crsc = new CCrSc_3_00(0, 8, 300, 125, 16.2f, 10.8f, 10.8f, 6.5f, 241.6f); // I 300 section
                    crsc = new CCrSc_0_05(0.1f, 0.05f);

                    int.TryParse(row["MaterialID"].ToString(), out CrSc_ID);
                    crsc.ICrSc_ID = CrSc_ID;

                    float.TryParse(row["fI_t"].ToString(), out fI_t);
                    crsc.I_t = fI_t;

                    float.TryParse(row["fI_y"].ToString(), out fI_y);
                    crsc.I_y = fI_y;

                    float.TryParse(row["fI_z"].ToString(), out fI_z);
                    crsc.I_z = fI_z;

                    float.TryParse(row["fA_g"].ToString(), out fA_g);
                    crsc.A_g = fA_g;

                    float.TryParse(row["fA_vy"].ToString(), out fA_vy);
                    crsc.A_vy = fA_vy;

                    float.TryParse(row["fA_vz"].ToString(), out fA_vz);
                    crsc.A_vz = fA_vz;

                    list_crsc.Add(crsc);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return list_crsc.ToArray();
        }

        private CNSupport[] getNSupports(DataTable dt)
        {
            List<CNSupport> nsupports = new List<CNSupport>();
            CNSupport nsupport = null;

            int eNDOF = 0;
            int iSupport_ID;
            List<int> nodeCollection;
            bool[] bRestrain = new bool[6];
            int fTime = 100;
            bool bux, buy, buz, brx, bry, brz;

            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    nodeCollection = new List<int>();
                    nsupport = new CNSupport(eNDOF);
                    int.TryParse(row["NSupportID"].ToString(), out iSupport_ID);
                    nsupport.ID = iSupport_ID;
                    foreach (string s in row["NodesIDCollection"].ToString().Split(','))
                    {
                        nodeCollection.Add(int.Parse(s));
                    }
                    nsupport.m_iNodeCollection = nodeCollection.ToArray();

                    bool.TryParse(row["bux"].ToString(), out bux);
                    bRestrain[0] = bux;

                    bool.TryParse(row["buy"].ToString(), out buy);
                    bRestrain[1] = buy;

                    bool.TryParse(row["buz"].ToString(), out buz);
                    bRestrain[2] = buz;

                    bool.TryParse(row["brx"].ToString(), out brx);
                    bRestrain[3] = brx;

                    bool.TryParse(row["bry"].ToString(), out bry);
                    bRestrain[4] = bry;

                    bool.TryParse(row["brz"].ToString(), out brz);
                    bRestrain[5] = brz;
                    nsupport.m_bRestrain = bRestrain;
                    nsupport.FTime = fTime;

                    nsupports.Add(nsupport);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return nsupports.ToArray();
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".xlsx";
            sfd.Filter = "Excel documents(.xlsx)|*.xlsx|Excel documents(.xls)|*.xls";
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    ArrayList a = new ArrayList();
                    a.Add(dataGridNodes.DataContext);
                    a.Add(sfd.FileName);
                    ThreadPool.QueueUserWorkItem(exportData, (object)a);
                }
                catch (ArgumentNullException ex)
                {
                    if (!EventLog.SourceExists("sw_en"))
                    {
                        EventLog.CreateEventSource("sw_en", "Application");
                    }
                    EventLog.WriteEntry("sw_en", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);

                }
                catch (Exception ex)
                {
                    if (!EventLog.SourceExists("sw_en"))
                    {
                        EventLog.CreateEventSource("sw_en", "Application");
                    }
                    EventLog.WriteEntry("sw_en", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
                }

            }
        }

        public static void exportData(object obj)
        {
            ArrayList args = (ArrayList)obj;
            CExportToExcel.ExportToExcel(args[0] as DataSet, args[1] as string);
        }

        private void menuItemView2Dview_Click(object sender, RoutedEventArgs e)
        {
            WindowPaint p = new WindowPaint();
            p.ShowDialog();
        }

        private void menuItemViewCrSc_Click(object sender, RoutedEventArgs e)
        {
            WindowCrossSection2D csdraw = new WindowCrossSection2D();
            csdraw.ShowDialog();
        }

        private void Container_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftCtrl:
                    foreach (Trackport3D t in list_trackports)
                    {
                        t.Trackball.IsCtrlDown = e.IsDown;
                    }
                    break;
            }
        }

        private void Container_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftCtrl:
                    foreach (Trackport3D t in list_trackports)
                    {
                        t.Trackball.IsCtrlDown = e.IsDown;
                    }
                    break;
            }
        }

        private void menuItemViewShowModel_Click(object sender, RoutedEventArgs e)
        {
            Window2 win = new Window2(model, m_bDebugging);

            list_trackports.Add(win._trackport);

            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Window " + (Container.Children.Count + 1) });
        }

        // Database
        private void menuItemDB_Materials_Click(object sender, RoutedEventArgs e)
        {
            WindowMaterialDB win = new WindowMaterialDB();
            win.ShowDialog();
        }

        private void menuItemDB_CrossSections_Click(object sender, RoutedEventArgs e)
        {
            WindowCrossSectionDB win = new WindowCrossSectionDB();
            win.ShowDialog();
        }

        // Calculation
        private void menuItemCalculate_Click(object sender, RoutedEventArgs e)
        {
            /*
            // Create calculation object and run calculation
            FEM_CALC_BASE.CFEM_CALC obj_Calc = new FEM_CALC_BASE.CFEM_CALC(); // Nove vypoctove jadro

            if (model.m_eSLN == ESLN.e2DD_1D)
            {
                FEM_CALC_1Din2D.CFEM_CALC obj_Calc_2D = new FEM_CALC_1Din2D.CFEM_CALC(model, m_bDebugging);
                obj_Calc = (FEM_CALC_BASE.CFEM_CALC)(obj_Calc_2D); // Change to basic type
            }
            else if (model.m_eSLN == ESLN.e3DD_1D)
            {
                FEM_CALC_1Din3D.CFEM_CALC obj_Calc_3D = new FEM_CALC_1Din3D.CFEM_CALC(model, m_bDebugging);
                obj_Calc = (FEM_CALC_BASE.CFEM_CALC)(obj_Calc_3D); // Change to basic type
            }
            else
            {
                // Not implemented
            }

            */




            FEM_CALC_1Din3D.CFEM_CALC obj_Calc = new FEM_CALC_1Din3D.CFEM_CALC();         // Povodny priklad - zadanie c 4

            // Auxialiary string - result data

            int iDispDecPrecision = 3; // Precision of numerical values of displacement and rotations
            string sDOFResults = null;

            for (int i = 0; i < obj_Calc.m_V_Displ.FVectorItems.Length; i++)
            {
                int iNodeNumber = obj_Calc.m_fDisp_Vector_CN[i, 1] + 1; // Incerase index (1st member "0" to "1"
                int iNodeDOFNumber = obj_Calc.m_fDisp_Vector_CN[i, 2] + 1;

                sDOFResults += "Node No:" + "\t" + iNodeNumber + "\t" +
                               "Node DOF No:" + "\t" + iNodeDOFNumber + "\t" +
                               "Value:" + "\t" + String.Format("{0:0.000}", Math.Round(obj_Calc.m_V_Displ.FVectorItems[i], iDispDecPrecision))
                               + "\n";
            }

            // Main String
            string sMessageCalc =
                "Calculation was successful!" + "\n\n" +
                "Result - vector of calculated values of unrestraint DOF displacement or rotation" + "\n\n" + sDOFResults;

            // Display Message
            MessageBox.Show(sMessageCalc, "Solver Message", MessageBoxButton.OK);

        }

        // Modules
        private void menuItemModuleSteel_Click(object sender, RoutedEventArgs e)
        {
            //CENEX.EN1993_1_1 objEC3 = new EN1993_1_1();
            //objEC3.


        }

        // 3D Blocks
        private void menuItemBlock3D_001_DoorInBay_Click(object sender, RoutedEventArgs e)
        {
            // Girt
            CCrSc_3_270XX_C crsc = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00115f, Colors.Orange);
            CMemberEccentricity eccentricity = new CMemberEccentricity(0, 0);
            CMember refgirt = new CMember(0, new CNode(0,0,0,0), new CNode(1,1,0,0), crsc, 0);
            refgirt.EccentricityStart = eccentricity;
            refgirt.EccentricityEnd = eccentricity;
            refgirt.DTheta_x = Math.PI / 2;

            CCrSc_3_63020_BOX crscColumn = new CCrSc_3_63020_BOX(0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Green);
            CMember mColumn = new CMember(0, new CNode(0, 0, 0, 0, 0), new CNode(1, 0, 0, 5, 0), crscColumn, 0);

            model = new EXAMPLES._3D.CBlock_3D_001_DoorInBay("Left", 2.1f, 0.9f, 0.6f, 0.5f, 0.3f, 0.9f, refgirt, mColumn, 9.3f);
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Block 3D 001 Doors in bay" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        // 2D Examples

        private void menuItemExample2D_01_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._2D.CExample_2D_01();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 2D 01" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample2D_02_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._2D.CExample_2D_02();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 2D 02" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample2D_03_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._2D.CExample_2D_03();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 2D 03" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample2D_04_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._2D.CExample_2D_04();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 2D 04" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        // Doplnit nejake priklady 5 -10

        private void menuItemExample2D_11_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._2D.CExample_2D_11();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 2D 11" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
            FEM_CALC_1Din2D.CFEM_CALC obj_Calc = new FEM_CALC_1Din2D.CFEM_CALC(model, m_bDebugging); // Nove vypoctove jadro
        }

        private void menuItemExample2D_12_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._2D.CExample_2D_12();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 2D 12" + " - Window " + (Container.Children.Count + 1) });
            win.Close();

            FEM_CALC_1Din2D.CFEM_CALC obj_Calc = new FEM_CALC_1Din2D.CFEM_CALC(model, m_bDebugging); // Nove vypoctove jadro
        }

        // 3D Examples

        private void menuItemExample3D_01_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_01();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 01" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_02_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_02();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 02" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_03_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_03();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 03" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_04_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_04();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 04" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_05_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_05();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 05" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_06_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_06();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 06" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_07_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_07();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 07" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_08_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_08();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 08" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_09_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_09();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 09" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_10_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_10();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 10" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_11_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_11();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 11" + " - Window " + (Container.Children.Count + 1) });

            FEM_CALC_1Din3D.CFEM_CALC obj_Calc = new FEM_CALC_1Din3D.CFEM_CALC(model, m_bDebugging); // Nove vypoctove jadro
            win.Close();
        }

        private void menuItemExample3D_21_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_21();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 21" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_50_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_50();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 50" + " - Window " + (Container.Children.Count + 1) });

            FEM_CALC_1Din3D.CFEM_CALC obj_Calc = new FEM_CALC_1Din3D.CFEM_CALC(model, m_bDebugging); // Nove vypoctove jadro
            win.Close();
        }

        private void menuItemExample3D_80_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_80();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 80" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_90_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_90();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 90" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_901_PF_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_901_PF(6,10,5,5, 8, 1f,1.2f,2f,0.3f,0,0, null);
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 901 PF" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_902_OM_Click(object sender, RoutedEventArgs e)
        {
            model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_902_OM();
            Window2 win = new Window2(model, m_bDebugging);
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 902 OM" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void menuItemExample3D_Ondro_Click(object sender, RoutedEventArgs e)
        {
            //model = new sw_en_GUI.EXAMPLES._3D.CExample_3D_Ondro();
            //Window2 win = new Window2(model, m_bDebugging);
            //list_trackports.Add(win._trackport);
            //Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Example 3D 90" + " - Window " + (Container.Children.Count + 1) });
            WindowHelixTrial w = new WindowHelixTrial();
            w.ShowDialog();
        }

        private void menuItemTestWindow_Click(object sender, RoutedEventArgs e)
        {
            Window3 win = new Window3();
            list_trackports.Add(win._trackport);
            Container.Children.Add(new MdiChild { Content = (UIElement)win.Content, Title = "Test" + " - Window " + (Container.Children.Count + 1) });
            win.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //for (int i = 0; i < Container.Children.Count; i++)
            //{
            //    Container.Children[i].Close();
            //}
        }

        private void menuItemTestDXFImport_Click(object sender, RoutedEventArgs e)
        {
            WindowDXFImport w = new WindowDXFImport();
            w.ShowDialog();
        }



    }
}
