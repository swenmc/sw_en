using System.Windows;
using Microsoft.Win32;
using EXPIMP;

namespace sw_en_GUI
{
    /// <summary>
    /// Interaction logic for WindowDXFImport.xaml
    /// </summary>
    public partial class WindowDXFImport : Window
    {
        private CADImage CADImg = null;
        public WindowDXFImport()
        {
            InitializeComponent();
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != true) return;
            if (ofd.FileName != null)
            {
                CADImg = new CADImage();
                CADImg.Base.Y = (int)Top + 200;
                CADImg.Base.X = 100;
                CADImg.LoadFromFile(ofd.FileName);
            }
            Draw();
        }

        private void Draw() 
        {
            if (CADImg == null) return;
            foreach (DXFEntity e in CADImg.FEntities.Entities)
            {
                if (e is DXFLine) 
                {
                    DXFLine l = (DXFLine)e;
                    
                    System.Diagnostics.Trace.WriteLine(string.Format("[{0};{1};{2}]   [{3};{4},{5}]   {6}", l.Point1.X, l.Point1.Y, l.Point1.Z, l.Point2.X, l.Point2.Y, l.Point2.Z, e.FColor));
                }
                
            }
        }


    }
}
