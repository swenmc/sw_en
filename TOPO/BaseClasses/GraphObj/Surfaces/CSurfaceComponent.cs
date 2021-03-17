using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    // Uvazujeme zatial 4 alebo 5 definicnych bodov
    [Serializable]
    public class CSurfaceComponent : CEntity3D
    {
        //private Point3D m_ControlPoint;
        int m_iNumberOfEdges;
        double m_CoordinateInPlane_x;
        double m_CoordinateInPlane_y;
        double m_Width; // Bezne uvazujeme width modular podla DB, ale pre koncove plechy moze byt sirka mensia
        double m_dLengthTotal;
        double m_dLengthTopRight;
        double m_dTipCoordinate_x;
        double m_dLengthTopTip;
        double m_dLengthTopLeft;
        
        //To Mato
        //preco tu mame control point ked CEntity3D ma v sebe control point???
        //komentujem a zmazeme neskor
        //public Point3D ControlPoint
        //{
        //    get
        //    {
        //        return m_ControlPoint;
        //    }

        //    set
        //    {
        //        m_ControlPoint = value;
        //    }
        //}

        public int NumberOfEdges
        {
            get
            {
                return m_iNumberOfEdges;
            }

            set
            {
                m_iNumberOfEdges = value;
            }
        }

        public double CoordinateInPlane_x
        {
            get
            {
                return m_CoordinateInPlane_x;
            }

            set
            {
                m_CoordinateInPlane_x = value;
            }
        }

        public double CoordinateInPlane_y
        {
            get
            {
                return m_CoordinateInPlane_y;
            }

            set
            {
                m_CoordinateInPlane_y = value;
            }
        }

        public double Width
        {
            get
            {
                return m_Width;
            }

            set
            {
                m_Width = value;
            }
        }

        public double LengthTotal
        {
            get
            {
                return m_dLengthTotal;
            }

            set
            {
                m_dLengthTotal = value;
            }
        }

        public double LengthTopRight
        {
            get
            {
                return m_dLengthTopRight;
            }

            set
            {
                m_dLengthTopRight = value;
            }
        }

        public double TipCoordinate_x
        {
            get
            {
                return m_dTipCoordinate_x;
            }

            set
            {
                m_dTipCoordinate_x = value;
            }
        }

        public double LengthTopTip
        {
            get
            {
                return m_dLengthTopTip;
            }

            set
            {
                m_dLengthTopTip = value;
            }
        }

        public double LengthTopLeft
        {
            get
            {
                return m_dLengthTopLeft;
            }

            set
            {
                m_dLengthTopLeft = value;
            }
        }

        public CSurfaceComponent()
        {

        }

        public CSurfaceComponent(int compID, int numberOfCorners,
        double coordinateInPlane_x, double coordinateInPlane_y, Point3D controlPoint_GCS,
        double width, double lengthTopLeft, double lengthTopRight, double tipCoordinate_x, double lengthTopTip,
        bool bIsDisplayed, float fTime)
        {
            ID = compID;
            m_iNumberOfEdges = numberOfCorners;
            m_CoordinateInPlane_x = coordinateInPlane_x;
            m_CoordinateInPlane_y = coordinateInPlane_y;
            ControlPoint = controlPoint_GCS;
            m_Width = width;
            m_dLengthTopLeft = lengthTopLeft;
            m_dLengthTopRight = lengthTopRight;
            m_dTipCoordinate_x = tipCoordinate_x;
            m_dLengthTopTip = lengthTopTip;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            // 5 edges
            //   3 ____ 2
            //    /    |
            // 4 /     |
            //  |      |
            //  |      |
            //  |______|
            // 0        1

            // 4 edges
            // 3 ______ 2
            //  |      |
            //  |      |
            //  |      |
            //  |      |
            //  |______|
            // 0        1
            //
            //
            //  y
            // /|\
            //  |___\ x
            //      /

            if (m_iNumberOfEdges == 4)
                m_dLengthTotal = Math.Max(m_dLengthTopLeft, m_dLengthTopRight);
            else
                m_dLengthTotal = Math.Max(Math.Max(m_dLengthTopLeft, m_dLengthTopRight), m_dLengthTopTip);
        }
    }
}
