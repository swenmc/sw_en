using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;







namespace CENEX.TEMPORARY
{
            public partial class DataGrid_Ukazky : Form
        {
            private System.ComponentModel.Container components;
            private Button button13;
            private Button button23;
            private DataGrid myDataGrid;
            private DataSet myDataSet;
            private bool TablesAlreadyAdded;

            public DataGrid_Ukazky()
            {
                // Required for Windows Form Designer support.
                InitializeComponent();
                // Call SetUp to bind the controls.
                SetUp();

                CreateMyListView();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
            private void InitializeComponent()
            {
                System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
                this.button13 = new System.Windows.Forms.Button();
                this.button23 = new System.Windows.Forms.Button();
                this.myDataGrid = new System.Windows.Forms.DataGrid();
                
                ((System.ComponentModel.ISupportInitialize)(this.myDataGrid)).BeginInit();
                
                this.SuspendLayout();
                //
                // button13
                //
                this.button13.Location = new System.Drawing.Point(500, 16);
                this.button13.Name = "button13";
                this.button13.Size = new System.Drawing.Size(120, 24);
                this.button13.TabIndex = 0;
                this.button13.Text = "Change Appearance";
                this.button13.Click += new System.EventHandler(this.button1_Click);
                //
                // button23
                //
                this.button23.Location = new System.Drawing.Point(500, 16);
                this.button23.Name = "button23";
                this.button23.Size = new System.Drawing.Size(120, 24);
                this.button23.TabIndex = 1;
                this.button23.Text = "Get Binding Manager";
                this.button23.Click += new System.EventHandler(this.button2_Click);
                //
                // myDataGrid
                //
                this.myDataGrid.CaptionText = "Microsoft DataGrid Control";
                this.myDataGrid.DataMember = "";
                this.myDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
                this.myDataGrid.Location = new System.Drawing.Point(500, 50);
                this.myDataGrid.Name = "myDataGrid";
                this.myDataGrid.Size = new System.Drawing.Size(300, 200);
                this.myDataGrid.TabIndex = 2;
                this.myDataGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseUp);
               
                //
                // Form
                //
                this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
                this.ClientSize = new System.Drawing.Size(893, 565);
               
                this.Controls.Add(this.button13);
                this.Controls.Add(this.button23);
                this.Controls.Add(this.myDataGrid);
                this.Name = "Form";
                this.Text = "DataGrid Control Sample";
                ((System.ComponentModel.ISupportInitialize)(this.myDataGrid)).EndInit();
                
                this.ResumeLayout(false);

            }

            public static void Main2()
            {
              //  Application.Run(new Form4());
            }

            private void SetUp()
            {
                // Create a DataSet with two tables and one relation.
                MakeDataSet();
                /* Bind the DataGrid to the DataSet. The dataMember
                specifies that the Customers table should be displayed.*/
                myDataGrid.SetDataBinding(myDataSet, "Customers");
            }

            protected void button1_Click(object sender, System.EventArgs e)
            {
                if (TablesAlreadyAdded) return;
                AddCustomDataTableStyle();
            }

            private void AddCustomDataTableStyle()
            {
                DataGridTableStyle ts1 = new DataGridTableStyle();
                ts1.MappingName = "Customers";
                // Set other properties.
                ts1.AlternatingBackColor = Color.LightGray;

                /* Add a GridColumnStyle and set its MappingName
                to the name of a DataColumn in the DataTable.
                Set the HeaderText and Width properties. */

                DataGridColumnStyle boolCol = new DataGridBoolColumn();
                boolCol.MappingName = "Current";
                boolCol.HeaderText = "IsCurrent Customer";
                boolCol.Width = 150;
                ts1.GridColumnStyles.Add(boolCol);

                // Add a second column style.
                DataGridColumnStyle TextCol = new DataGridTextBoxColumn();
                TextCol.MappingName = "custName";
                TextCol.HeaderText = "Customer Name";
                TextCol.Width = 250;
                ts1.GridColumnStyles.Add(TextCol);

                // Create the second table style with columns.
                DataGridTableStyle ts2 = new DataGridTableStyle();
                ts2.MappingName = "Orders";

                // Set other properties.
                ts2.AlternatingBackColor = Color.LightBlue;

                // Create new ColumnStyle objects
                DataGridColumnStyle cOrderDate =
                new DataGridTextBoxColumn();
                cOrderDate.MappingName = "OrderDate";
                cOrderDate.HeaderText = "Order Date";
                cOrderDate.Width = 100;
                ts2.GridColumnStyles.Add(cOrderDate);

                /* Use a PropertyDescriptor to create a formatted
                column. First get the PropertyDescriptorCollection
                for the data source and data member. */
                PropertyDescriptorCollection pcol = this.BindingContext
                [myDataSet, "Customers.custToOrders"].GetItemProperties();

                /* Create a formatted column using a PropertyDescriptor.
                The formatting character "c" specifies a currency format. */
                DataGridColumnStyle csOrderAmount =
                new DataGridTextBoxColumn(pcol["OrderAmount"], "c", true);
                csOrderAmount.MappingName = "OrderAmount";
                csOrderAmount.HeaderText = "Total";
                csOrderAmount.Width = 100;
                ts2.GridColumnStyles.Add(csOrderAmount);

                /* Add the DataGridTableStyle instances to
                the GridTableStylesCollection. */
                myDataGrid.TableStyles.Add(ts1);
                myDataGrid.TableStyles.Add(ts2);

                // Sets the TablesAlreadyAdded to true so this doesn't happen again.
                TablesAlreadyAdded = true;
            }

            protected void button2_Click(object sender, System.EventArgs e)
            {
                BindingManagerBase bmGrid;
                bmGrid = BindingContext[myDataSet, "Customers"];
                MessageBox.Show("Current BindingManager Position: " + bmGrid.Position);
            }

            private void Grid_MouseUp(object sender, MouseEventArgs e)
            {
                // Create a HitTestInfo object using the HitTest method.

                // Get the DataGrid by casting sender.
                DataGrid myGrid = (DataGrid)sender;
                DataGrid.HitTestInfo myHitInfo = myGrid.HitTest(e.X, e.Y);
                Console.WriteLine(myHitInfo);
                Console.WriteLine(myHitInfo.Type);
                Console.WriteLine(myHitInfo.Row);
                Console.WriteLine(myHitInfo.Column);
            }

            // Create a DataSet with two tables and populate it.
            private void MakeDataSet()
            {
                // Create a DataSet.
                myDataSet = new DataSet("myDataSet");

                // Create two DataTables.
                DataTable tCust = new DataTable("Customers");
                DataTable tOrders = new DataTable("Orders");

                // Create two columns, and add them to the first table.
                DataColumn cCustID = new DataColumn("CustID", typeof(int));
                DataColumn cCustName = new DataColumn("CustName");
                DataColumn cCurrent = new DataColumn("Current", typeof(bool));
                tCust.Columns.Add(cCustID);
                tCust.Columns.Add(cCustName);
                tCust.Columns.Add(cCurrent);

                // Create three columns, and add them to the second table.
                DataColumn cID =
                new DataColumn("CustID", typeof(int));
                DataColumn cOrderDate =
                new DataColumn("orderDate", typeof(DateTime));
                DataColumn cOrderAmount =
                new DataColumn("OrderAmount", typeof(decimal));
                tOrders.Columns.Add(cOrderAmount);
                tOrders.Columns.Add(cID);
                tOrders.Columns.Add(cOrderDate);

                // Add the tables to the DataSet.
                myDataSet.Tables.Add(tCust);
                myDataSet.Tables.Add(tOrders);

                // Create a DataRelation, and add it to the DataSet.
                DataRelation dr = new DataRelation
                ("custToOrders", cCustID, cID);
                myDataSet.Relations.Add(dr);

                /* Populates the tables. For each customer and order,
                creates two DataRow variables. */
                DataRow newRow1;
                DataRow newRow2;

                // Create three customers in the Customers Table.
                for (int i = 1; i < 4; i++)
                {
                    newRow1 = tCust.NewRow();
                    newRow1["custID"] = i;
                    // Add the row to the Customers table.
                    tCust.Rows.Add(newRow1);
                }
                // Give each customer a distinct name.
                tCust.Rows[0]["custName"] = "Customer1";
                tCust.Rows[1]["custName"] = "Customer2";
                tCust.Rows[2]["custName"] = "Customer3";

                // Give the Current column a value.
                tCust.Rows[0]["Current"] = true;
                tCust.Rows[1]["Current"] = true;
                tCust.Rows[2]["Current"] = false;

                // For each customer, create five rows in the Orders table.
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 1; j < 6; j++)
                    {
                        newRow2 = tOrders.NewRow();
                        newRow2["CustID"] = i;
                        newRow2["orderDate"] = new DateTime(2001, i, j * 2);
                        newRow2["OrderAmount"] = i * 10 + j * .1;
                        // Add the row to the Orders table.
                        tOrders.Rows.Add(newRow2);
                    }
                }
            }

            private void CreateMyListView()
            {
                // Create a new ListView control.
               ListView listView1 = new ListView();
                listView1.Bounds = new Rectangle(new Point(10, 10), new Size(300, 200));


                // Set the view to show details.
                listView1.View = View.Details;
                // Allow the user to edit item text.
                listView1.LabelEdit = true;
                // Allow the user to rearrange columns.
                listView1.AllowColumnReorder = true;
                // Display check boxes.
                listView1.CheckBoxes = true;
                // Select the item and subitems when selection is made.
                listView1.FullRowSelect = true;
                // Display grid lines.
                listView1.GridLines = true;
                // Sort the items in the list in ascending order.
                listView1.Sorting = SortOrder.Ascending;

                // Create three items and three sets of subitems for each item.
                ListViewItem item1 = new ListViewItem("item1", 0);
                // Place a check mark next to the item.
                item1.Checked = true;
                item1.SubItems.Add("1");
                item1.SubItems.Add("2");
                item1.SubItems.Add("3");
                ListViewItem item2 = new ListViewItem("item2", 1);
                item2.SubItems.Add("4");
                item2.SubItems.Add("5");
                item2.SubItems.Add("6");
                ListViewItem item3 = new ListViewItem("item3", 0);
                // Place a check mark next to the item.
                item3.Checked = true;
                item3.SubItems.Add("7");
                item3.SubItems.Add("8");
                item3.SubItems.Add("9");

                // Create columns for the items and subitems.
                listView1.Columns.Add("Item Column", -2, HorizontalAlignment.Left);
                listView1.Columns.Add("Column 2", -2, HorizontalAlignment.Left);
                listView1.Columns.Add("Column 3", -2, HorizontalAlignment.Left);
                listView1.Columns.Add("Column 4", -2, HorizontalAlignment.Center);

                //Add the items to the ListView.
                listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

                // Create two ImageList objects.
                ImageList imageListSmall = new ImageList();
                ImageList imageListLarge = new ImageList();


                // TREBA NASTAVIT CESTU K NEJAKYM OBRAZKOM


                /*
                // Initialize the ImageList objects with bitmaps.
                imageListSmall.Images.Add(Bitmap.FromFile("C:\\Documents and Settings\\All Users\\Dokumenty\\Obrázky\\Ukázky obrázků\\Lekníny.jpg"));
                imageListSmall.Images.Add(Bitmap.FromFile("C:\\Documents and Settings\\All Users\\Dokumenty\\Obrázky\\Ukázky obrázků\\Modré vrcholky.jpg"));
                imageListLarge.Images.Add(Bitmap.FromFile("C:\\Documents and Settings\\All Users\\Dokumenty\\Obrázky\\Ukázky obrázků\\Západ slunce.jpg"));
                imageListLarge.Images.Add(Bitmap.FromFile("C:\\Documents and Settings\\All Users\\Dokumenty\\Obrázky\\Ukázky obrázků\\Zima.jpg"));
                
                 */

                //Assign the ImageList objects to the ListView.
                listView1.LargeImageList = imageListLarge;
                listView1.SmallImageList = imageListSmall;

                // Add the ListView to the control collection.
                this.Controls.Add(listView1);
            }

        }




    }