using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EXPIMP
{
    public static class ExportHelper
    {
        public static RenderTargetBitmap SaveViewPortContentAsImage(Viewport3D viewPort)
        {

            // Scale dimensions from 96 dpi to 600 dpi.
            double scale = 300 / 96;
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)(scale * viewPort.ActualWidth),
                                                            (int)(scale * viewPort.ActualHeight),
                                                            scale * 96,
                                                            scale * 96, System.Windows.Media.PixelFormats.Default);
            viewPort.InvalidateVisual();
            bmp.Render(viewPort);
            bmp.Freeze();
            SaveBitmapImage(bmp);
            return bmp;

        }

        public static void SaveBitmapImage(RenderTargetBitmap bmp)
        {
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream stm = File.Create("ViewPort.png"))
            {
                png.Save(stm);
            }
        }
    }
}
