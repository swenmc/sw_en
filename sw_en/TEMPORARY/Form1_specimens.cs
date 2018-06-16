using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Configuration;
using System.Collections;
using System.Web;
using System.Drawing.Text;







namespace CENEX
{
    public partial class Form1_specimens : Form
    {
        public Form1_specimens()
        {
            InitializeComponent();
            
            
            
            
            //  this.SSTextOut(pDC, "ms^2/Hz+H_2O-mc^^2__4", &rect, DT_CENTER);
            
        }





private void Labels_text ()
        {
    // Not working

label1.Text = "<sub>Hey</sub>";  // For subscript

label2.Text = "<sup>Hey</sup>";  // For superscript

// richTextBox1.SelectionCharOffset = 2;
        }
private void PopulateListBoxWithFonts()
{
    
    foreach (FontFamily oneFontFamily in FontFamily.Families)
    {
        listBox1.Items.Add(oneFontFamily.Name);
    }

    this.Font = new Font(this.Font.FontFamily, this.Font.Size, this.Font.Style | FontStyle.Bold);

    


    FontFamily ffamily = btn2.Font.FontFamily;
    string str = ffamily.ToString();
    if (str == ("Arial Black"))
    {
        FontFamily btn2_1 = new FontFamily("Arial");
        btn2.Text = "FontFamily";
    }
    else
    {
        FontFamily btn2_1 = new FontFamily("Arial Black");
        btn2.Text = "Control font family changes from Arial to Arial Black.";

    }


    foreach (FontFamily oneFontFamily in FontFamily.Families)
{
    string oneFontFamilyName = oneFontFamily.Name;


    listBox2.Text = oneFontFamilyName;
   
listBox2.Items.Add(oneFontFamily.Name);

}


}
/*      
private void SSTextOut(CDC* pDC, CString data , CRect* drawRect, int justification)
{
 //Necessary initializations
 pDC.SaveDC();
 
 CSize sz;
 CRect outRect (0,0,0,0);

 CFont* pFont = pDC->GetCurrentFont();
 CFont* oldFont;
 pDC->SetTextAlign(TA_BOTTOM|TA_LEFT);

 LOGFONT lf;
 pFont->GetLogFont(&lf);

 CPoint sub,sup,subofs,supofs;

 // Calculate subscript/superscript size and offsets
 sub.x=lf.lfWidth/2;
 sup.x=lf.lfWidth/2;
 sub.y=lf.lfHeight/3*2;
 sup.y=lf.lfHeight/3*2;

 subofs.x=lf.lfWidth/2;
 supofs.x=lf.lfWidth/2;
 subofs.y=lf.lfHeight/6;
 supofs.y=lf.lfHeight/3;

 lf.lfWidth=sub.x;
 lf.lfHeight=sub.y;
 CFont SubFont;
 SubFont.CreateFontIndirect(&lf);
 
 lf.lfWidth=sup.x;
 lf.lfHeight=sup.y;
 CFont SupFont;
 SupFont.CreateFontIndirect(&lf);
 
 CString temp = data;
 TCHAR c;
 
 // Calculate the size of the text that needs to be displayed
 do
 {
  int x=0;
  CString s = "";
  c=' ';
  bool bFind=true;

  // Find the first "^" or "_", indicating the sub- or superscript
  while (bFind)
  {
   x=data.FindOneOf("^_");
   if (x==-1) 
   {
    bFind=false;
    x=data.GetLength();
   }
   else if (x==data.GetLength()-1) bFind=false;
   else if (data[x]!=data[x+1]) 
   {
    bFind=false; 
    c=data[x];
   }
   else x++;
   s=s+data.Left(x);
   data.Delete(0,min(x+1,data.GetLength()));
  }
  sz = pDC->GetTextExtent(s);
  outRect.right+=sz.cx;
  if (outRect.Height()<sz.cy) outRect.top=outRect.bottom-sz.cy;
  
  switch (c) 
  {
  case '^':
   oldFont = pDC->SelectObject(&SupFont);
   sz = pDC->GetTextExtent(data[0]);
   outRect.right+=sz.cx+supofs.x;
   data.Delete(0);
   pDC->SelectObject(oldFont);
   break;
  case '_':
   oldFont = pDC->SelectObject(&SubFont);
   sz = pDC->GetTextExtent(data[0]);
   outRect.right+=sz.cx+subofs.x;
   data.Delete(0);
   pDC->SelectObject(oldFont);
   break;
  }
 }
 while (c!=' ');
 
 // Adjust text position
 outRect.bottom+=2*subofs.y;
 outRect.top-=2*subofs.x;
 CPoint Origin;
 Origin.y = drawRect->Height()/2+outRect.Height()/2+drawRect->top;

 switch (justification)
 {
 case DT_CENTER:
  Origin.x = drawRect->Width()/2-outRect.Width()/2+drawRect->left;
  break;
 case DT_LEFT:
  Origin.x = drawRect->left;
  break;
 case DT_RIGHT:
  Origin.x = drawRect->right-outRect.Width();
 }

 CPoint pnt = Origin;

 data = temp;

 // Draw text
 do
 {
  int x=0;
  CString s = "";
  c=' ';
  bool bFind=true;

  // Find the first "^" or "_", indicating the sub- or superscript
  while (bFind)
  {
   x=data.FindOneOf("^_");
   if (x==-1) 
   {
    bFind=false;
    x=data.GetLength();
   }
   else if (x==data.GetLength()-1) bFind=false;
   else if (data[x]!=data[x+1]) 
   {
    bFind=false; 
    c=data[x];
   }
   else x++;
   s=s+data.Left(x);
   data.Delete(0,min(x+1,data.GetLength()));
  }
  // Draw main text
  pDC->ExtTextOut(pnt.x,pnt.y,ETO_CLIPPED,drawRect,s,NULL);
  sz = pDC->GetTextExtent(s);
  pnt.x+=sz.cx;
  
  // Draw subscript or superscript
  switch (c) 
  {
  case '^':
   oldFont = pDC->SelectObject(&SupFont);
   pDC->ExtTextOut(pnt.x+supofs.x,pnt.y-supofs.y,ETO_CLIPPED,drawRect,data[0],NULL);
   sz = pDC->GetTextExtent(data[0]);
   pnt.x+=sz.cx+supofs.x;
   data.Delete(0);
   pDC->SelectObject(oldFont);
   break;
  case '_':
   oldFont = pDC->SelectObject(&SubFont);
   pDC->ExtTextOut(pnt.x+subofs.x,pnt.y+subofs.y,ETO_CLIPPED,drawRect,data[0],NULL);
   sz = pDC->GetTextExtent(data[0]);
   pnt.x+=sz.cx+supofs.x;
   data.Delete(0);
   pDC->SelectObject(oldFont);
   break;
  }
 }
 while (c!=' ');

 // Done, restoring the device context
 pDC->RestoreDC(-1);
}

*/
/*
private void fontfamily_specimen ()
{
    TextBlock textBlock1 = new TextBlock();
    TextBlock textBlock2 = new TextBlock();

    textBlock1.TextWrapping = textBlock2.TextWrapping = TextWrapping.Wrap;
    textBlock2.Background = Brushes.AntiqueWhite;
    textBlock2.TextAlignment = TextAlignment.Center;

    textBlock1.Inlines.Add(new Bold(new Run("TextBlock")));
    textBlock1.Inlines.Add(new Run(" is designed to be "));
    textBlock1.Inlines.Add(new Italic(new Run("lightweight")));
    textBlock1.Inlines.Add(new Run(", and is geared specifically at integrating "));
    textBlock1.Inlines.Add(new Italic(new Run("small")));
    textBlock1.Inlines.Add(new Run(" portions of flow content into a UI."));

    textBlock2.Text =
        "By default, a TextBlock provides no UI beyond simply displaying its contents.";



textBlock1.FontFamily = new FontFamily("Comic Sans MS");
// Create a new FontFamily object, using an absolute URI reference.
// myTextBlock.FontFamily = new FontFamily("file:///d:/MyFonts/#Pericles Light");

// Create a new FontFamily object, using a base URI reference and a relative URI reference.
// myTextBlock.FontFamily = new FontFamily(new Uri("file:///d:/MyFonts/"), "./#Pericles Light");

// The font resource reference includes the base URI reference (application directory level),
// and a relative URI reference.
// myTextBlock.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./resources/#Pericles Light");






}
*/




public void FillFontComboBox(ComboBox comboBoxFonts)
{
    using (InstalledFontCollection fontFamilies = new InstalledFontCollection ())
    // Enumerate the current set of system fonts,
    // and fill the combo box with the names of the fonts.
        foreach (FontFamily fontFamily in fontFamilies.Families)
    {
        // FontFamily.Source contains the font family name.
        comboBoxFonts.Items.Add(fontFamily.Name);
    }
   
        
    comboBoxFonts.SelectedIndex = 0;
}

private void btn2_Click(object sender, EventArgs e)
{
    this.Labels_text();
    this.PopulateListBoxWithFonts();
    this.FillFontComboBox(comboBoxFonts);
    this.fontDialog1.ShowDialog();
    this.List3();

    FontListForm i = new FontListForm ();
    i.ShowDialog();


}
private void List3 ()
{
   /* Creating the instance of the InstalledFontCollection Class*/

   InstalledFontCollection ifc=new InstalledFontCollection();
   IEnumerator ie;
   ie=ifc.Families.GetEnumerator();
   while(ie.MoveNext())
   {
      listBox3.Items.Add(ie.Current.ToString());
   }

 
}




     
    }
}



