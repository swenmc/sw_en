using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PFD.Infrastructure
{
    public class SalesmanPlateCalculation
    {
        //-------------------------------------------------------------------------------------------------------------------------------
        SalesmanPlateCalculationAsyncStub stub = null;
        public delegate void SalesmanPlateCalculationAsyncStub(CPlate plate);
        private Object theLock;

        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        public SalesmanPlateCalculation(Object lockObject)
        {
            theLock = lockObject;
        }
        public void PlateCalculation(CPlate plate)
        {
            List<Point> points = null;

            if (plate.ScrewArrangement == null) return; // Screw arrangmenet must exists

            points = plate.ScrewArrangement.HolesCentersPoints2D.ToList();

            if (points == null || points.Count == 0)
            {
                return;
            }

            // Calculate size of plate and width to height ratio to set size of "salesman" algorthim window
            double fTempMax_X = 0, fTempMin_X = 0, fTempMax_Y = 0, fTempMin_Y = 0;
            Drawing2D.CalculateModelLimits(plate.ScrewArrangement.HolesCentersPoints2D, out fTempMax_X, out fTempMin_X, out fTempMax_Y, out fTempMin_Y);
            double fWidth = fTempMax_X - fTempMin_X;
            double fHeigth = fTempMax_Y - fTempMin_Y;
            //double fHeightToWidthRatio = fHeigth / fWidth;

            // Add coordinates of drilling machine start point
            points.Insert(0, new System.Windows.Point(0, 0));

            TwoOpt.ModelSync model = new TwoOpt.ModelSync();
            model.Run(points, fWidth, fHeigth);

            List<System.Windows.Point> PathPoints = new List<System.Windows.Point>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                PathPoints.Add(points[model._tour.GetCities()[i]]);
            }

            plate.DrillingRoutePoints = PathPoints;            
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public IAsyncResult BeginSalesmanPlateCalculation(CPlate plate, AsyncCallback cb, object s)
        {
            stub = new SalesmanPlateCalculationAsyncStub(PlateCalculation);
            //using delegate for asynchronous implementation   
            return stub.BeginInvoke(plate, cb, null);
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public void EndSalesmanPlateCalculation(IAsyncResult call)
        {
            stub.EndInvoke(call);
        }



    }
}
