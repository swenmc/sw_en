using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CDimensionLinear3D
    {
        private Point3D m_PointStart;
        private Point3D m_PointEnd;
        private Point3D m_PointText;

        private Point3D m_PointStartL2;
        private Point3D m_PointEndL2;
        private Point3D m_PointMainLine1;
        private Point3D m_PointMainLine2;

        private Vector3D m_Direction;
        private Vector3D m_Horizontal;
        private Vector3D m_Vertical;
        private double m_DimensionLinesLength;
        private double m_DimensionMainLineDistance;

        private string m_Text;

        public Point3D PointStart
        {
            get
            {
                return m_PointStart;
            }

            set
            {
                m_PointStart = value;
            }
        }

        public Point3D PointEnd
        {
            get
            {
                return m_PointEnd;
            }

            set
            {
                m_PointEnd = value;
            }
        }

        public Vector3D Horizontal
        {
            get
            {
                return m_Horizontal;
            }

            set
            {
                m_Horizontal = value;
            }
        }

        public Vector3D Vertical
        {
            get
            {
                return m_Vertical;
            }

            set
            {
                m_Vertical = value;
            }
        }

        public double DimensionLinesLength
        {
            get
            {
                return m_DimensionLinesLength;
            }

            set
            {
                m_DimensionLinesLength = value;
            }
        }

        public double DimensionMainLineDistance
        {
            get
            {
                return m_DimensionMainLineDistance;
            }

            set
            {
                m_DimensionMainLineDistance = value;
            }
        }

        public string Text
        {
            get
            {
                return m_Text;
            }

            set
            {
                m_Text = value;
            }
        }

        public Vector3D Direction
        {
            get
            {
                return m_Direction;
            }

            set
            {
                m_Direction = value;
            }
        }

        public Point3D PointText
        {
            get
            {
                return m_PointText;
            }

            set
            {
                m_PointText = value;
            }
        }

        public CDimensionLinear3D() { }
        public CDimensionLinear3D(Point3D pointStart, Point3D pointEnd, Vector3D direction, Vector3D horizontal,  Vector3D vertical, double dimensionLinesLength, double dimensionMainLineDistance, string text)
        {
            m_PointStart = pointStart;
            m_PointEnd = pointEnd;
            m_Direction = direction;
            m_Horizontal = horizontal;
            m_Vertical = vertical;
            m_DimensionLinesLength = dimensionLinesLength;
            m_DimensionMainLineDistance = dimensionMainLineDistance;
            m_Text = text;
            SetTextPoint();
        }

        public void SetTextPoint()
        {
            m_PointText = new Point3D() {
                X = (m_PointStart.X + m_PointEnd.X) / 2 + Direction.X * DimensionLinesLength,
                Y = (m_PointStart.Y + m_PointEnd.Y) / 2 + Direction.Y * DimensionLinesLength,
                Z = (m_PointStart.Z + m_PointEnd.Z) / 2 + Direction.Z * DimensionLinesLength
            };
            
        }

        public void SetPoints()
        {
            m_PointMainLine1 = new Point3D()
            {
                X = m_PointStart.X + Direction.X * DimensionMainLineDistance,
                Y = m_PointStart.Y + Direction.Y * DimensionMainLineDistance,
                Z = m_PointStart.Z + Direction.Z * DimensionMainLineDistance,
            };

            m_PointMainLine2 = new Point3D()
            {
                X = m_PointEnd.X + Direction.X * DimensionMainLineDistance,
                Y = m_PointEnd.Y + Direction.Y * DimensionMainLineDistance,
                Z = m_PointEnd.Z + Direction.Z * DimensionMainLineDistance,
            };

        }




    }
}
