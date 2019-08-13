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

        public CDimensionLinear3D() { }
        public CDimensionLinear3D(Point3D pointStart, Point3D pointEnd, Vector3D horizontal,  Vector3D vertical, double dimensionLinesLength, double dimensionMainLineDistance, string text)
        {
            m_PointStart = pointStart;
            m_PointEnd = pointEnd;
            m_Horizontal = horizontal;
            m_Vertical = vertical;
            m_DimensionLinesLength = dimensionLinesLength;
            m_DimensionMainLineDistance = dimensionMainLineDistance;
            m_Text = text;
        }

        

        
    }
}
