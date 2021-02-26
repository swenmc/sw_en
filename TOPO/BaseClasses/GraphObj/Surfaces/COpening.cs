using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class COpening : CSurfaceComponent
    {
        // TO Ondrej - zaviedol som objekt COpening, ale kedze ma rovnake parametre ako CSurfaceComponent, tak asi nie je velmi ucelne ho teraz
        // pouzivat, skor su momentalne zbytocne problemy s pretypovanim a je to zbytocne spomalenie

        public COpening()
        {

        }

        public COpening(int compID, int numberOfCorners,
        double coordinateInPlane_x, double coordinateInPlane_y, Point3D controlPoint_GCS,
        double width, double lengthTopLeft, double lengthTopRight, double tipCoordinate_x, double lengthTopTip,
        bool bIsDisplayed, float fTime)
        {
            ID = compID;
            NumberOfEdges = numberOfCorners;
            CoordinateInPlane_x = coordinateInPlane_x;
            CoordinateInPlane_y = coordinateInPlane_y;
            ControlPoint = controlPoint_GCS;
            Width = width;
            LengthTopLeft = lengthTopLeft;
            LengthTopRight = lengthTopRight;
            TipCoordinate_x = tipCoordinate_x;
            LengthTopTip = lengthTopTip;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            if (NumberOfEdges == 4)
                LengthTotal = Math.Max(LengthTopLeft, LengthTopRight);
            else
                LengthTotal = Math.Max(Math.Max(LengthTopLeft, LengthTopRight), LengthTopTip);
        }
    }
}
