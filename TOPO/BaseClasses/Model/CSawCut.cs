using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CSawCut : CEntity3D
    {
        private Point3D m_PointStart;

        public Point3D PointStart
        {
            get { return m_PointStart; }
            set { m_PointStart = value; }
        }
        private Point3D m_PointEnd;

        public Point3D PointEnd
        {
            get { return m_PointEnd; }
            set { m_PointEnd = value; }
        }

        private float m_fLength;

        public float Length
        {
            get { return m_fLength; }
            set { m_fLength = value; }
        }

        public CSawCut(int id, Point3D start, Point3D end, bool bIsDiplayed_temp, int fTime)
        {
            ID = id;
            PointStart = start;
            PointEnd = end;
            BIsDisplayed = bIsDiplayed_temp;
            FTime = fTime;

            Length = (float)Math.Sqrt((float)Math.Pow(m_PointEnd.X - m_PointStart.X, 2f) + (float)Math.Pow(m_PointEnd.Y - m_PointStart.Y, 2f) + (float)Math.Pow(m_PointEnd.Z - m_PointStart.Z, 2f));
        }

        public GeometryModel3D GetSawCutModel(System.Windows.Media.Color color)
        {
            GeometryModel3D model = new GeometryModel3D();

            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(color)); // TODO Ondrej - urobit nastavitelnu hrubku a farbu

            float fLineThickness = 0.002f; // hrubka = priemer pre export do 2D (2 x polomer valca)
            float fLineCylinderRadius = 0.005f; //0.005f * fLength; // Nastavovat ! polomer valca, co najmensi ale viditelny - 3D

            return CVolume.CreateM_G_M_3D_Volume_Cylinder(new Point3D(0,0,0), 13, fLineCylinderRadius, m_fLength, material, 0);
        }
    }
}
