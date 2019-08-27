using BaseClasses;
using BaseClasses.GraphObj;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;


namespace Examples
{
    public class CExample_3D_90 : CExample
    {
        public CExample_3D_90()
        {
            m_arrGOPoints = new Point3D[1];
            //m_arrGOLines = new BaseClasses.GraphObj.CLine[21];
            m_arrGOAreas = new BaseClasses.GraphObj.CArea[0];
            m_arrGOVolumes = new BaseClasses.GraphObj.CVolume[1];

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            //m_arrCrSc[0] = new CRSC.CCrSc_0_05(0.1f, 0.05f);

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Ground
            // Level 0 [-1.000]

            float x = 0.0f;
            float y = 0.0f;
            float z = 10.0f;

            m_arrGOPoints[00] = new Point3D(x , y , z);

            DiffuseMaterial DiffMat1 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(255, 255, 120)));

            BitmapImage brickjpg = new BitmapImage();
            brickjpg.BeginInit();
            brickjpg.UriSource = new Uri(@"brick.jpg", UriKind.RelativeOrAbsolute);
            brickjpg.EndInit();

            ImageBrush brickIB = new ImageBrush(brickjpg);
            brickIB.TileMode = TileMode.Tile; 
            brickIB.Stretch = Stretch.None;

            DiffuseMaterial DiffMat2 = new DiffuseMaterial(brickIB);

            // Ground
            // Level 0 [-1.000]
            m_arrGOVolumes[000] = new CVolume(001, EVolumeShapeType.eShape3DPrism_8Edges, m_arrGOPoints[000], 1, 1, 1, DiffMat1, DiffMat2, true, 0);

        }
    }
}
