/*
C# Programming Tips & Techniques
by Charles Wright, Kris Jamsa

Publisher: Osborne/McGraw-Hill (December 28, 2001)
ISBN: 0072193794
*/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace CENEX
{
  /// <summary>
  /// Summary description for FontListForm.
  /// </summary>
  public class FontListForm : System.Windows.Forms.Form
  {
    private System.Windows.Forms.ListBox listBox1;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public FontListForm()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();
      InitForm ();
      //
      // TODO: Add any constructor code after InitializeComponent call
      //
    }

    FontFamily [] fonts;
    protected void InitForm ()
    {
      fonts = FontFamily.Families;
      foreach (FontFamily font in fonts)
        listBox1.Items.Add (font.Name);
    }
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if (components != null) 
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // listBox1
      // 
      this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
      this.listBox1.Location = new System.Drawing.Point(22, 21);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new System.Drawing.Size(226, 190);
      this.listBox1.TabIndex = 0;
      this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_OnDoubleClick);
      this.listBox1.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox1_OnMeasureItem);
      this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_OnDrawItem);
      this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_OnSelChanged);
      // 
      // FontListForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(266, 243);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                      this.listBox1});
      this.Name = "FontListForm";
      this.Text = "Font Families";
      this.ResumeLayout(false);

    }
    #endregion

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main_FontList() 
    {
      Application.Run(new FontListForm());
    }

    private void listBox1_OnDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
    {
      Rectangle rc = e.Bounds;
      string strSample = "The quick red fox jumps over the lazy brown dog";
      Font fntText;
      Font fntSample;
      int cy = 3 * (rc.Bottom - rc.Top) / 4;
      try
      {
        fntText = new Font ("Times New Roman", 8, GraphicsUnit.Point);
      }
      catch (ArgumentException)
      {
        MessageBox.Show ("Cannot create Times New Roman font", "Pied font",
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return;
      }
      try
      {
        fntText = new Font ("Times New Roman", 8, GraphicsUnit.Point);
        fntSample = new Font (fonts[e.Index], 8, GraphicsUnit.Point);
      }
      catch (ArgumentException)
      {
        DialogResult mbResult = MessageBox.Show("Cannot open font " + fonts[e.Index].Name +"\nDo you want to delete this item from the list","Pied Font Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        switch (mbResult)
        {
          case DialogResult.Yes:
            listBox1.Items.RemoveAt (e.Index);
                // My code
            ActiveForm.Close();
            ActiveForm.Close();
            
            return;
          case DialogResult.No:
            break;
        }
        strSample = "Cannot open font " + fonts[e.Index].Name;
        fntSample = fntText;
      }
      Brush brush;
      string str = fonts[e.Index].Name + " -- ";
      if ((e.State & DrawItemState.Selected) != 0)
      {
        e.Graphics.FillRectangle (Brushes.DarkBlue, rc);
        e.DrawFocusRectangle ();
        brush = Brushes.White;
      }
      else
      {
        e.Graphics.FillRectangle (Brushes.White, rc);
        brush = Brushes.Black;
      }
      e.Graphics.DrawString (str, fntText, brush, rc);
      StringFormat format = new StringFormat ();
      format.LineAlignment = StringAlignment.Center;
      format.FormatFlags = StringFormatFlags.LineLimit;
      SizeF size = e.Graphics.MeasureString (str, fntText);
      rc = new Rectangle (rc.Left + (int) (size.Width + .5),
        rc.Top, rc.Width, rc.Height);
      e.Graphics.DrawString (strSample, fntSample, brush, rc, format);
      fntText.Dispose ();
      fntSample.Dispose();
    }

    private void listBox1_OnSelChanged(object sender, System.EventArgs e)
    {
      listBox1.Invalidate ();
    }

    private void listBox1_OnMeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
    {
      MeasureItemEventArgs args = e;
      Font plain = new Font ("Times New Roman", 10, GraphicsUnit.Point);
      SizeF size = e.Graphics.MeasureString ("Test", plain);
      args.ItemHeight = (int) (size.Height + .5);
      plain.Dispose ();
    }

    private void listBox1_OnDoubleClick(object sender, System.EventArgs e)
    {
      if (e is ListChangedEventArgs)
      {
        frmSampleText stuff = new frmSampleText ();
      }
    }
  }
  /// <summary>
  /// Summary description for frmSampleText.
  /// </summary>
  public class frmSampleText : System.Windows.Forms.Form
  {
    private System.Windows.Forms.TextBox textBox1;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public frmSampleText()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(8, 8);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(272, 136);
      this.textBox1.TabIndex = 0;
      this.textBox1.Text = "textBox1";
      // 
      // frmSampleText
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(6, 16);
      this.ClientSize = new System.Drawing.Size(296, 157);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                      this.textBox1});
      this.Name = "frmSampleText";
      this.Text = "Sample Text";
      this.ResumeLayout(false);

    }
    #endregion
  }

}
