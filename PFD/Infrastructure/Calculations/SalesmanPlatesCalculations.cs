using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public static class SalesmanPlatesCalculations
    {
        public static void RunSalesmanPlatesCalculations(List<CPlate> plates)
        {
            //int count = 0;
            //solverWindow.SetBeamsProgress(count, plates.Count);
            //double step = 50.0 / plates.Count;

            List<WaitHandle> waitHandles = new List<WaitHandle>();
            int maxWaitHandleCount = 64;  //maximum is 64 // MC: zatial najlepsie sa u mna javi 8 cores a 8 waithandlecount => 19 sekund

            List<SalesmanPlateCalculation> recs = new List<SalesmanPlateCalculation>();
            List<IAsyncResult> results = new List<IAsyncResult>();
            SalesmanPlateCalculation recalc = null;
            IAsyncResult result = null;
            Object lockObject = new Object();

            foreach (CPlate plate in plates)
            {
                recalc = new SalesmanPlateCalculation(lockObject);
                result = recalc.BeginSalesmanPlateCalculation(plate, null, null);
                waitHandles.Add(result.AsyncWaitHandle);
                recs.Add(recalc);
                results.Add(result);
                if (waitHandles.Count >= maxWaitHandleCount)
                {
                    int index = WaitHandle.WaitAny(waitHandles.ToArray());
                    waitHandles.RemoveAt(index);
                    recs[index].EndSalesmanPlateCalculation(results[index]);
                    recs.RemoveAt(index);
                    results.RemoveAt(index);
                    //count++;
                    //solverWindow.SetBeamsProgress(count, plates.Count);
                    //solverWindow.Progress += step;
                    //solverWindow.UpdateProgress();
                }
            }

            while (waitHandles.Count > 0)
            {
                int index = WaitHandle.WaitAny(waitHandles.ToArray());
                waitHandles.RemoveAt(index);
                recs[index].EndSalesmanPlateCalculation(results[index]);
                recs.RemoveAt(index);
                results.RemoveAt(index);
                //count++;
                //solverWindow.SetBeamsProgress(count, plates.Count);
                //solverWindow.Progress += step;
                //solverWindow.UpdateProgress();
            }

            waitHandles.Clear();
            recs.Clear();
            results.Clear();
        }
    }
}
