namespace CENEX
{
    partial class TreeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Setup");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Structures");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Objects");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Groups");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Members");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Project", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Linear");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Surface");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Table", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Load case");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Load Cases", new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10});
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Groups");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Combinations");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Classes");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Loads", new System.Windows.Forms.TreeNode[] {
            treeNode11,
            treeNode12,
            treeNode13,
            treeNode14});
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Materials");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Member properties");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Document");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Concrete");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Steel");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Timber");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Materials", new System.Windows.Forms.TreeNode[] {
            treeNode19,
            treeNode20,
            treeNode21});
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Permanent");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Static");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Snow");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("Wind");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Climate", new System.Windows.Forms.TreeNode[] {
            treeNode25,
            treeNode26});
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Variable", new System.Windows.Forms.TreeNode[] {
            treeNode24,
            treeNode27});
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Loads", new System.Windows.Forms.TreeNode[] {
            treeNode23,
            treeNode28});
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Geometric shapes");
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Concrete");
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Tie");
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Rolled");
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Cold-formed");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Welded");
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("Built-up");
            System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("Steel", new System.Windows.Forms.TreeNode[] {
            treeNode32,
            treeNode33,
            treeNode34,
            treeNode35,
            treeNode36});
            System.Windows.Forms.TreeNode treeNode38 = new System.Windows.Forms.TreeNode("Composite");
            System.Windows.Forms.TreeNode treeNode39 = new System.Windows.Forms.TreeNode("Timber");
            System.Windows.Forms.TreeNode treeNode40 = new System.Windows.Forms.TreeNode("Sections", new System.Windows.Forms.TreeNode[] {
            treeNode30,
            treeNode31,
            treeNode37,
            treeNode38,
            treeNode39});
            System.Windows.Forms.TreeNode treeNode41 = new System.Windows.Forms.TreeNode("Database", new System.Windows.Forms.TreeNode[] {
            treeNode22,
            treeNode29,
            treeNode40});
            System.Windows.Forms.TreeNode treeNode42 = new System.Windows.Forms.TreeNode("Libraries", new System.Windows.Forms.TreeNode[] {
            treeNode41});
            System.Windows.Forms.TreeNode treeNode43 = new System.Windows.Forms.TreeNode("EN 1993-1-1");
            System.Windows.Forms.TreeNode treeNode44 = new System.Windows.Forms.TreeNode("EN 1993-1-2");
            System.Windows.Forms.TreeNode treeNode45 = new System.Windows.Forms.TreeNode("EN 1993-1-3");
            System.Windows.Forms.TreeNode treeNode46 = new System.Windows.Forms.TreeNode("EN 1993-1-4");
            System.Windows.Forms.TreeNode treeNode47 = new System.Windows.Forms.TreeNode("EN 1993-1-5");
            System.Windows.Forms.TreeNode treeNode48 = new System.Windows.Forms.TreeNode("EN 1993-1", new System.Windows.Forms.TreeNode[] {
            treeNode43,
            treeNode44,
            treeNode45,
            treeNode46,
            treeNode47});
            System.Windows.Forms.TreeNode treeNode49 = new System.Windows.Forms.TreeNode("EN 1993-2");
            System.Windows.Forms.TreeNode treeNode50 = new System.Windows.Forms.TreeNode("EN 1993-3");
            System.Windows.Forms.TreeNode treeNode51 = new System.Windows.Forms.TreeNode("EC3", new System.Windows.Forms.TreeNode[] {
            treeNode48,
            treeNode49,
            treeNode50});
            System.Windows.Forms.TreeNode treeNode52 = new System.Windows.Forms.TreeNode("AISC");
            System.Windows.Forms.TreeNode treeNode53 = new System.Windows.Forms.TreeNode("Steel", new System.Windows.Forms.TreeNode[] {
            treeNode51,
            treeNode52});
            System.Windows.Forms.TreeNode treeNode54 = new System.Windows.Forms.TreeNode("EN 1992-1-1");
            System.Windows.Forms.TreeNode treeNode55 = new System.Windows.Forms.TreeNode("EN 1992-1-2");
            System.Windows.Forms.TreeNode treeNode56 = new System.Windows.Forms.TreeNode("EN 1992-1", new System.Windows.Forms.TreeNode[] {
            treeNode54,
            treeNode55});
            System.Windows.Forms.TreeNode treeNode57 = new System.Windows.Forms.TreeNode("EN 1992-2");
            System.Windows.Forms.TreeNode treeNode58 = new System.Windows.Forms.TreeNode("EC2", new System.Windows.Forms.TreeNode[] {
            treeNode56,
            treeNode57});
            System.Windows.Forms.TreeNode treeNode59 = new System.Windows.Forms.TreeNode("AISC");
            System.Windows.Forms.TreeNode treeNode60 = new System.Windows.Forms.TreeNode("Concrete", new System.Windows.Forms.TreeNode[] {
            treeNode58,
            treeNode59});
            System.Windows.Forms.TreeNode treeNode61 = new System.Windows.Forms.TreeNode("EN 1994-1-1");
            System.Windows.Forms.TreeNode treeNode62 = new System.Windows.Forms.TreeNode("EN 1994-1-2");
            System.Windows.Forms.TreeNode treeNode63 = new System.Windows.Forms.TreeNode("EN 1994-1", new System.Windows.Forms.TreeNode[] {
            treeNode61,
            treeNode62});
            System.Windows.Forms.TreeNode treeNode64 = new System.Windows.Forms.TreeNode("EN 1994-2");
            System.Windows.Forms.TreeNode treeNode65 = new System.Windows.Forms.TreeNode("EC4", new System.Windows.Forms.TreeNode[] {
            treeNode63,
            treeNode64});
            System.Windows.Forms.TreeNode treeNode66 = new System.Windows.Forms.TreeNode("Composite", new System.Windows.Forms.TreeNode[] {
            treeNode65});
            System.Windows.Forms.TreeNode treeNode67 = new System.Windows.Forms.TreeNode("EN 1995-1-1");
            System.Windows.Forms.TreeNode treeNode68 = new System.Windows.Forms.TreeNode("EN 1995-1-2");
            System.Windows.Forms.TreeNode treeNode69 = new System.Windows.Forms.TreeNode("EN 1995-1", new System.Windows.Forms.TreeNode[] {
            treeNode67,
            treeNode68});
            System.Windows.Forms.TreeNode treeNode70 = new System.Windows.Forms.TreeNode("EN 1995-2");
            System.Windows.Forms.TreeNode treeNode71 = new System.Windows.Forms.TreeNode("EC5", new System.Windows.Forms.TreeNode[] {
            treeNode69,
            treeNode70});
            System.Windows.Forms.TreeNode treeNode72 = new System.Windows.Forms.TreeNode("Timber", new System.Windows.Forms.TreeNode[] {
            treeNode71});
            System.Windows.Forms.TreeNode treeNode73 = new System.Windows.Forms.TreeNode("EN 1999-1-1");
            System.Windows.Forms.TreeNode treeNode74 = new System.Windows.Forms.TreeNode("EN 1999-1-2");
            System.Windows.Forms.TreeNode treeNode75 = new System.Windows.Forms.TreeNode("EN 1999-1-3");
            System.Windows.Forms.TreeNode treeNode76 = new System.Windows.Forms.TreeNode("EN 1999-1-4");
            System.Windows.Forms.TreeNode treeNode77 = new System.Windows.Forms.TreeNode("EN 1999-1-5");
            System.Windows.Forms.TreeNode treeNode78 = new System.Windows.Forms.TreeNode("EN 1999-1", new System.Windows.Forms.TreeNode[] {
            treeNode73,
            treeNode74,
            treeNode75,
            treeNode76,
            treeNode77});
            System.Windows.Forms.TreeNode treeNode79 = new System.Windows.Forms.TreeNode("EC9", new System.Windows.Forms.TreeNode[] {
            treeNode78});
            System.Windows.Forms.TreeNode treeNode80 = new System.Windows.Forms.TreeNode("Alluminium", new System.Windows.Forms.TreeNode[] {
            treeNode79});
            System.Windows.Forms.TreeNode treeNode81 = new System.Windows.Forms.TreeNode("Codes", new System.Windows.Forms.TreeNode[] {
            treeNode53,
            treeNode60,
            treeNode66,
            treeNode72,
            treeNode80});
            System.Windows.Forms.TreeNode treeNode82 = new System.Windows.Forms.TreeNode("Moduls");
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Setup";
            treeNode2.Name = "Node1";
            treeNode2.Text = "Structures";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Objects";
            treeNode4.Name = "Node3";
            treeNode4.Text = "Groups";
            treeNode5.Name = "Node4";
            treeNode5.Text = "Members";
            treeNode6.BackColor = System.Drawing.Color.White;
            treeNode6.ForeColor = System.Drawing.Color.Navy;
            treeNode6.Name = "Project";
            treeNode6.NodeFont = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode6.Text = "Project";
            treeNode7.Name = "Node40";
            treeNode7.Text = "Linear";
            treeNode8.Name = "Node41";
            treeNode8.Text = "Surface";
            treeNode9.Name = "Node27";
            treeNode9.Text = "Table";
            treeNode10.Name = "Node43";
            treeNode10.Text = "Load case";
            treeNode11.Name = "Node25";
            treeNode11.Text = "Load Cases";
            treeNode12.Name = "Node24";
            treeNode12.Text = "Groups";
            treeNode13.Name = "Node23";
            treeNode13.Text = "Combinations";
            treeNode14.Name = "Node42";
            treeNode14.Text = "Classes";
            treeNode15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            treeNode15.Name = "Node5";
            treeNode15.NodeFont = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode15.Text = "Loads";
            treeNode16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            treeNode16.Name = "Node6";
            treeNode16.NodeFont = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode16.Text = "Materials";
            treeNode17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            treeNode17.Name = "Node7";
            treeNode17.NodeFont = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode17.Text = "Member properties";
            treeNode18.Name = "Node12";
            treeNode18.Text = "Document";
            treeNode19.Name = "Node16";
            treeNode19.Text = "Concrete";
            treeNode20.Name = "Node20";
            treeNode20.Text = "Steel";
            treeNode21.Name = "Node22";
            treeNode21.Text = "Timber";
            treeNode22.Name = "Node15";
            treeNode22.Text = "Materials";
            treeNode23.Name = "Node30";
            treeNode23.Text = "Permanent";
            treeNode24.Name = "Node35";
            treeNode24.Text = "Static";
            treeNode25.Name = "Node37";
            treeNode25.Text = "Snow";
            treeNode26.Name = "Node39";
            treeNode26.Text = "Wind";
            treeNode27.Name = "Node36";
            treeNode27.Text = "Climate";
            treeNode28.Name = "Node34";
            treeNode28.Text = "Variable";
            treeNode29.Name = "Node29";
            treeNode29.Text = "Loads";
            treeNode30.Name = "Node5";
            treeNode30.Text = "Geometric shapes";
            treeNode31.Name = "Node8";
            treeNode31.Text = "Concrete";
            treeNode32.Name = "Node9";
            treeNode32.Text = "Tie";
            treeNode33.Name = "Node1";
            treeNode33.Text = "Rolled";
            treeNode34.Name = "Node4";
            treeNode34.Text = "Cold-formed";
            treeNode35.Name = "Node6";
            treeNode35.Text = "Welded";
            treeNode36.Name = "Node13";
            treeNode36.Text = "Built-up";
            treeNode37.Name = "Node10";
            treeNode37.Text = "Steel";
            treeNode38.Name = "Node11";
            treeNode38.Text = "Composite";
            treeNode39.Name = "Node12";
            treeNode39.Text = "Timber";
            treeNode40.Name = "Node0";
            treeNode40.Text = "Sections";
            treeNode41.Name = "Node14";
            treeNode41.Text = "Database";
            treeNode42.ForeColor = System.Drawing.Color.Black;
            treeNode42.Name = "Node13";
            treeNode42.NodeFont = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode42.Text = "Libraries";
            treeNode43.Name = "EN1993-1-1";
            treeNode43.Text = "EN 1993-1-1";
            treeNode44.Name = "EN1993-1-2";
            treeNode44.Text = "EN 1993-1-2";
            treeNode45.Name = "EN1993-1-3";
            treeNode45.Text = "EN 1993-1-3";
            treeNode46.Name = "EN1993-1-4";
            treeNode46.Text = "EN 1993-1-4";
            treeNode47.Name = "EN1993-1-5";
            treeNode47.Text = "EN 1993-1-5";
            treeNode48.Name = "EN1993-1";
            treeNode48.Text = "EN 1993-1";
            treeNode49.Name = "EN1993-2";
            treeNode49.Text = "EN 1993-2";
            treeNode50.Name = "EN1993-3";
            treeNode50.Text = "EN 1993-3";
            treeNode51.Name = "EC3";
            treeNode51.Text = "EC3";
            treeNode52.Name = "AISC";
            treeNode52.Text = "AISC";
            treeNode53.Name = "Node8";
            treeNode53.Text = "Steel";
            treeNode54.Name = "EN1992-1-1";
            treeNode54.Text = "EN 1992-1-1";
            treeNode55.Name = "EN1992-1-2";
            treeNode55.Text = "EN 1992-1-2";
            treeNode56.Name = "EN1992-1";
            treeNode56.Text = "EN 1992-1";
            treeNode57.Name = "EN1992-2";
            treeNode57.Text = "EN 1992-2";
            treeNode58.Name = "EC2";
            treeNode58.Text = "EC2";
            treeNode59.Name = "AISC";
            treeNode59.Text = "AISC";
            treeNode60.Name = "Concrete";
            treeNode60.Text = "Concrete";
            treeNode61.Name = "EN1994-1-1";
            treeNode61.Text = "EN 1994-1-1";
            treeNode62.Name = "EN1994-1-2";
            treeNode62.Text = "EN 1994-1-2";
            treeNode63.Name = "EN1994-1";
            treeNode63.Text = "EN 1994-1";
            treeNode64.Name = "EN1994-2";
            treeNode64.Text = "EN 1994-2";
            treeNode65.Name = "EC4";
            treeNode65.Text = "EC4";
            treeNode66.Name = "Node10";
            treeNode66.Text = "Composite";
            treeNode67.Name = "EN1995-1-1";
            treeNode67.Text = "EN 1995-1-1";
            treeNode68.Name = "EN1995-1-2";
            treeNode68.Text = "EN 1995-1-2";
            treeNode69.Name = "EN1995-1";
            treeNode69.Text = "EN 1995-1";
            treeNode70.Name = "EN1995-2";
            treeNode70.Text = "EN 1995-2";
            treeNode71.Name = "EC5";
            treeNode71.Text = "EC5";
            treeNode72.Name = "Node11";
            treeNode72.Text = "Timber";
            treeNode73.Name = "EN 1999-1-1";
            treeNode73.Text = "EN 1999-1-1";
            treeNode74.Name = "EN 1999-1-2";
            treeNode74.Text = "EN 1999-1-2";
            treeNode75.Name = "EN 1999-1-3";
            treeNode75.Text = "EN 1999-1-3";
            treeNode76.Name = "EN1999-1-4";
            treeNode76.Text = "EN 1999-1-4";
            treeNode77.Name = "EN1999-1-5";
            treeNode77.Text = "EN 1999-1-5";
            treeNode78.Name = "EN1999-1";
            treeNode78.Text = "EN 1999-1";
            treeNode79.Name = "EC9";
            treeNode79.Text = "EC9";
            treeNode80.Name = "Node46";
            treeNode80.Text = "Alluminium";
            treeNode81.ForeColor = System.Drawing.Color.Purple;
            treeNode81.Name = "Node45";
            treeNode81.NodeFont = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode81.Text = "Codes";
            treeNode82.Name = "Moduls";
            treeNode82.Text = "Moduls";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode42,
            treeNode81,
            treeNode82});
            this.treeView1.Size = new System.Drawing.Size(216, 416);
            this.treeView1.TabIndex = 18;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // TreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 416);
            this.Controls.Add(this.treeView1);
            this.Name = "TreeForm";
            this.Text = "TreeForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
    }
}