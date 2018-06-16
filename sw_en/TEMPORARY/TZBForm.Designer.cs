namespace CENEX
{
    partial class TZBForm
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
            this.ComboBox1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.labelA = new System.Windows.Forms.Label();
            this.ComboBox_A = new System.Windows.Forms.ComboBox();
            this.ComboBox_B = new System.Windows.Forms.ComboBox();
            this.labelB = new System.Windows.Forms.Label();
            this.labelC = new System.Windows.Forms.Label();
            this.ComboBox_C = new System.Windows.Forms.ComboBox();
            this.labelD = new System.Windows.Forms.Label();
            this.ComboBox_D = new System.Windows.Forms.ComboBox();
            this.labelE = new System.Windows.Forms.Label();
            this.ComboBox_E = new System.Windows.Forms.ComboBox();
            this.ComboBox_F = new System.Windows.Forms.ComboBox();
            this.labelF = new System.Windows.Forms.Label();
            this.ComboBox_G = new System.Windows.Forms.ComboBox();
            this.labelG = new System.Windows.Forms.Label();
            this.ComboBox_R = new System.Windows.Forms.ComboBox();
            this.labelR = new System.Windows.Forms.Label();
            this.ComboBox_U = new System.Windows.Forms.ComboBox();
            this.labelUhol = new System.Windows.Forms.Label();
            this.ComboBox_L = new System.Windows.Forms.ComboBox();
            this.labelL = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBoxpozn = new System.Windows.Forms.TextBox();
            this.pozn = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.aktual = new System.Windows.Forms.Label();
            this.schema = new System.Windows.Forms.Label();
            this.Rozpocet = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.richTextBoxAct = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBoxNew = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ComboBox1
            // 
            this.ComboBox1.FormattingEnabled = true;
            this.ComboBox1.Items.AddRange(new object[] {
            "Štvorhranná rúra - RU AxB-L",
            "Štvrohranný oblúk - OB AxB/90°",
            "Štvorhranná odbočka - OD AxB/CxD/ExF",
            "Štvorhranná regulačná klapka - RK AxB",
            "Iné"});
            this.ComboBox1.Location = new System.Drawing.Point(464, 61);
            this.ComboBox1.Name = "ComboBox1";
            this.ComboBox1.Size = new System.Drawing.Size(277, 21);
            this.ComboBox1.TabIndex = 47;
            this.ComboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label6.Location = new System.Drawing.Point(377, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 20);
            this.label6.TabIndex = 53;
            this.label6.Text = "Tvar prvku";
            // 
            // labelA
            // 
            this.labelA.AutoSize = true;
            this.labelA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelA.Location = new System.Drawing.Point(399, 94);
            this.labelA.Name = "labelA";
            this.labelA.Size = new System.Drawing.Size(58, 20);
            this.labelA.TabIndex = 54;
            this.labelA.Text = "A [mm]";
            // 
            // ComboBox_A
            // 
            this.ComboBox_A.FormattingEnabled = true;
            this.ComboBox_A.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_A.Location = new System.Drawing.Point(464, 94);
            this.ComboBox_A.Name = "ComboBox_A";
            this.ComboBox_A.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_A.TabIndex = 55;
            // 
            // ComboBox_B
            // 
            this.ComboBox_B.FormattingEnabled = true;
            this.ComboBox_B.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_B.Location = new System.Drawing.Point(464, 129);
            this.ComboBox_B.Name = "ComboBox_B";
            this.ComboBox_B.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_B.TabIndex = 57;
            // 
            // labelB
            // 
            this.labelB.AutoSize = true;
            this.labelB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelB.Location = new System.Drawing.Point(398, 128);
            this.labelB.Name = "labelB";
            this.labelB.Size = new System.Drawing.Size(58, 20);
            this.labelB.TabIndex = 56;
            this.labelB.Text = "B [mm]";
            // 
            // labelC
            // 
            this.labelC.AutoSize = true;
            this.labelC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelC.Location = new System.Drawing.Point(399, 166);
            this.labelC.Name = "labelC";
            this.labelC.Size = new System.Drawing.Size(58, 20);
            this.labelC.TabIndex = 60;
            this.labelC.Text = "C [mm]";
            // 
            // ComboBox_C
            // 
            this.ComboBox_C.FormattingEnabled = true;
            this.ComboBox_C.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_C.Location = new System.Drawing.Point(464, 165);
            this.ComboBox_C.Name = "ComboBox_C";
            this.ComboBox_C.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_C.TabIndex = 61;
            // 
            // labelD
            // 
            this.labelD.AutoSize = true;
            this.labelD.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelD.Location = new System.Drawing.Point(399, 200);
            this.labelD.Name = "labelD";
            this.labelD.Size = new System.Drawing.Size(59, 20);
            this.labelD.TabIndex = 60;
            this.labelD.Text = "D [mm]";
            // 
            // ComboBox_D
            // 
            this.ComboBox_D.FormattingEnabled = true;
            this.ComboBox_D.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_D.Location = new System.Drawing.Point(464, 200);
            this.ComboBox_D.Name = "ComboBox_D";
            this.ComboBox_D.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_D.TabIndex = 61;
            // 
            // labelE
            // 
            this.labelE.AutoSize = true;
            this.labelE.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelE.Location = new System.Drawing.Point(400, 235);
            this.labelE.Name = "labelE";
            this.labelE.Size = new System.Drawing.Size(58, 20);
            this.labelE.TabIndex = 60;
            this.labelE.Text = "E [mm]";
            // 
            // ComboBox_E
            // 
            this.ComboBox_E.FormattingEnabled = true;
            this.ComboBox_E.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_E.Location = new System.Drawing.Point(463, 234);
            this.ComboBox_E.Name = "ComboBox_E";
            this.ComboBox_E.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_E.TabIndex = 61;
            // 
            // ComboBox_F
            // 
            this.ComboBox_F.FormattingEnabled = true;
            this.ComboBox_F.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_F.Location = new System.Drawing.Point(657, 95);
            this.ComboBox_F.Name = "ComboBox_F";
            this.ComboBox_F.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_F.TabIndex = 65;
            // 
            // labelF
            // 
            this.labelF.AutoSize = true;
            this.labelF.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelF.Location = new System.Drawing.Point(595, 95);
            this.labelF.Name = "labelF";
            this.labelF.Size = new System.Drawing.Size(57, 20);
            this.labelF.TabIndex = 64;
            this.labelF.Text = "F [mm]";
            // 
            // ComboBox_G
            // 
            this.ComboBox_G.FormattingEnabled = true;
            this.ComboBox_G.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_G.Location = new System.Drawing.Point(657, 128);
            this.ComboBox_G.Name = "ComboBox_G";
            this.ComboBox_G.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_G.TabIndex = 67;
            // 
            // labelG
            // 
            this.labelG.AutoSize = true;
            this.labelG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelG.Location = new System.Drawing.Point(592, 130);
            this.labelG.Name = "labelG";
            this.labelG.Size = new System.Drawing.Size(60, 20);
            this.labelG.TabIndex = 66;
            this.labelG.Text = "G [mm]";
            // 
            // ComboBox_R
            // 
            this.ComboBox_R.FormattingEnabled = true;
            this.ComboBox_R.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_R.Location = new System.Drawing.Point(657, 166);
            this.ComboBox_R.Name = "ComboBox_R";
            this.ComboBox_R.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_R.TabIndex = 69;
            // 
            // labelR
            // 
            this.labelR.AutoSize = true;
            this.labelR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelR.Location = new System.Drawing.Point(592, 166);
            this.labelR.Name = "labelR";
            this.labelR.Size = new System.Drawing.Size(59, 20);
            this.labelR.TabIndex = 68;
            this.labelR.Text = "R [mm]";
            // 
            // ComboBox_U
            // 
            this.ComboBox_U.FormattingEnabled = true;
            this.ComboBox_U.Items.AddRange(new object[] {
            "0",
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "35",
            "40",
            "45",
            "50",
            "55",
            "60",
            "65",
            "70",
            "75",
            "80",
            "85",
            "90",
            "95",
            "100",
            "105",
            "110",
            "115",
            "120",
            "125",
            "130",
            "135"});
            this.ComboBox_U.Location = new System.Drawing.Point(657, 234);
            this.ComboBox_U.Name = "ComboBox_U";
            this.ComboBox_U.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_U.TabIndex = 71;
            // 
            // labelUhol
            // 
            this.labelUhol.AutoSize = true;
            this.labelUhol.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelUhol.Location = new System.Drawing.Point(580, 235);
            this.labelUhol.Name = "labelUhol";
            this.labelUhol.Size = new System.Drawing.Size(72, 20);
            this.labelUhol.TabIndex = 70;
            this.labelUhol.Text = "Uhol α [°]";
            // 
            // ComboBox_L
            // 
            this.ComboBox_L.FormattingEnabled = true;
            this.ComboBox_L.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
            this.ComboBox_L.Location = new System.Drawing.Point(657, 202);
            this.ComboBox_L.Name = "ComboBox_L";
            this.ComboBox_L.Size = new System.Drawing.Size(82, 21);
            this.ComboBox_L.TabIndex = 73;
            // 
            // labelL
            // 
            this.labelL.AutoSize = true;
            this.labelL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelL.Location = new System.Drawing.Point(596, 200);
            this.labelL.Name = "labelL";
            this.labelL.Size = new System.Drawing.Size(56, 20);
            this.labelL.TabIndex = 72;
            this.labelL.Text = "L [mm]";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(27, 52);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(283, 246);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 74;
            this.pictureBox1.TabStop = false;
            // 
            // textBoxpozn
            // 
            this.textBoxpozn.Location = new System.Drawing.Point(463, 273);
            this.textBoxpozn.Name = "textBoxpozn";
            this.textBoxpozn.Size = new System.Drawing.Size(275, 20);
            this.textBoxpozn.TabIndex = 75;
            // 
            // pozn
            // 
            this.pozn.AutoSize = true;
            this.pozn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.pozn.Location = new System.Drawing.Point(399, 273);
            this.pozn.Name = "pozn";
            this.pozn.Size = new System.Drawing.Size(49, 20);
            this.pozn.TabIndex = 76;
            this.pozn.Text = "Pozn.";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(15, 357);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(726, 307);
            this.dataGridView1.TabIndex = 77;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button1.Location = new System.Drawing.Point(464, 313);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(275, 26);
            this.button1.TabIndex = 78;
            this.button1.Text = "Pridaj novú položku";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(513, 682);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(96, 23);
            this.buttonDelete.TabIndex = 79;
            this.buttonDelete.Text = "Zmaž rozpočet";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(615, 682);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(96, 23);
            this.buttonSave.TabIndex = 80;
            this.buttonSave.Text = "Ulož rozpočet";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(717, 682);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(96, 23);
            this.buttonLoad.TabIndex = 81;
            this.buttonLoad.Text = "Otvor rozpočet";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // aktual
            // 
            this.aktual.AutoSize = true;
            this.aktual.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aktual.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.aktual.Location = new System.Drawing.Point(498, 18);
            this.aktual.Name = "aktual";
            this.aktual.Size = new System.Drawing.Size(168, 29);
            this.aktual.TabIndex = 82;
            this.aktual.Text = "Nová položka";
            // 
            // schema
            // 
            this.schema.AutoSize = true;
            this.schema.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.schema.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.schema.Location = new System.Drawing.Point(22, 9);
            this.schema.Name = "schema";
            this.schema.Size = new System.Drawing.Size(225, 29);
            this.schema.TabIndex = 83;
            this.schema.Text = "Schéma geometrie";
            // 
            // Rozpocet
            // 
            this.Rozpocet.AutoSize = true;
            this.Rozpocet.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Rozpocet.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.Rozpocet.Location = new System.Drawing.Point(22, 313);
            this.Rozpocet.Name = "Rozpocet";
            this.Rozpocet.Size = new System.Drawing.Size(121, 29);
            this.Rozpocet.TabIndex = 84;
            this.Rozpocet.Text = "Rozpočet";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(761, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 20);
            this.label3.TabIndex = 87;
            this.label3.Text = "Vlastnosti";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(760, 313);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(211, 29);
            this.label1.TabIndex = 85;
            this.label1.Text = "Aktuálna položka";
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(202, 248);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(929, 682);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(96, 23);
            this.buttonEdit.TabIndex = 88;
            this.buttonEdit.Text = "Upraviť položku";
            this.buttonEdit.UseVisualStyleBackColor = true;
            // 
            // richTextBoxAct
            // 
            this.richTextBoxAct.Location = new System.Drawing.Point(765, 385);
            this.richTextBoxAct.Name = "richTextBoxAct";
            this.richTextBoxAct.Size = new System.Drawing.Size(260, 279);
            this.richTextBoxAct.TabIndex = 89;
            this.richTextBoxAct.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(761, 357);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 90;
            this.label2.Text = "Vlastnosti";
            // 
            // richTextBoxNew
            // 
            this.richTextBoxNew.Location = new System.Drawing.Point(765, 59);
            this.richTextBoxNew.Name = "richTextBoxNew";
            this.richTextBoxNew.Size = new System.Drawing.Size(260, 239);
            this.richTextBoxNew.TabIndex = 91;
            this.richTextBoxNew.Text = "";
            // 
            // TZBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1037, 719);
            this.Controls.Add(this.richTextBoxNew);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.richTextBoxAct);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Rozpocet);
            this.Controls.Add(this.schema);
            this.Controls.Add(this.aktual);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.pozn);
            this.Controls.Add(this.textBoxpozn);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ComboBox_L);
            this.Controls.Add(this.labelL);
            this.Controls.Add(this.ComboBox_U);
            this.Controls.Add(this.labelUhol);
            this.Controls.Add(this.ComboBox_R);
            this.Controls.Add(this.labelR);
            this.Controls.Add(this.ComboBox_G);
            this.Controls.Add(this.labelG);
            this.Controls.Add(this.ComboBox_F);
            this.Controls.Add(this.labelF);
            this.Controls.Add(this.ComboBox_C);
            this.Controls.Add(this.labelC);
            this.Controls.Add(this.ComboBox_D);
            this.Controls.Add(this.labelD);
            this.Controls.Add(this.ComboBox_E);
            this.Controls.Add(this.labelE);
            this.Controls.Add(this.ComboBox_B);
            this.Controls.Add(this.labelB);
            this.Controls.Add(this.ComboBox_A);
            this.Controls.Add(this.labelA);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ComboBox1);
            this.Name = "TZBForm";
            this.Text = "TZB1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelA;
        private System.Windows.Forms.ComboBox ComboBox_A;
        private System.Windows.Forms.ComboBox ComboBox_B;
        private System.Windows.Forms.Label labelB;
        private System.Windows.Forms.Label labelC;
        private System.Windows.Forms.ComboBox ComboBox_C;
        private System.Windows.Forms.Label labelD;
        private System.Windows.Forms.ComboBox ComboBox_D;
        private System.Windows.Forms.Label labelE;
        private System.Windows.Forms.ComboBox ComboBox_E;
        private System.Windows.Forms.ComboBox ComboBox_F;
        private System.Windows.Forms.Label labelF;
        private System.Windows.Forms.ComboBox ComboBox_G;
        private System.Windows.Forms.Label labelG;
        private System.Windows.Forms.ComboBox ComboBox_R;
        private System.Windows.Forms.Label labelR;
        private System.Windows.Forms.ComboBox ComboBox_U;
        private System.Windows.Forms.Label labelUhol;
        private System.Windows.Forms.ComboBox ComboBox_L;
        private System.Windows.Forms.Label labelL;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBoxpozn;
        private System.Windows.Forms.Label pozn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Label aktual;
        private System.Windows.Forms.Label schema;
        private System.Windows.Forms.Label Rozpocet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.RichTextBox richTextBoxAct;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBoxNew;
    }
}