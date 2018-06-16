namespace CENEX
{
    partial class Annex
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
            this.annexBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.annexBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // annexBox1
            // 
            this.annexBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.annexBox1.Location = new System.Drawing.Point(0, 0);
            this.annexBox1.Name = "annexBox1";
            this.annexBox1.Size = new System.Drawing.Size(653, 778);
            this.annexBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.annexBox1.TabIndex = 1;
            this.annexBox1.TabStop = false;
            // 
            // Annex
            // 
            this.ClientSize = new System.Drawing.Size(653, 778);
            this.Controls.Add(this.annexBox1);
            this.Name = "Annex";
            ((System.ComponentModel.ISupportInitialize)(this.annexBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox annexBox1;
        
         

    }
}