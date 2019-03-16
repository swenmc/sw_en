using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public static class CBeamsCalculations
    {
        public static void RunBeamsCalculations(List<CBeam_Simple> beamSimpleModels, bool bCalculateLoadCasesOnly, Solver solverWindow)
        {
            int count = 0;
            solverWindow.SetBeamsProgress(count, beamSimpleModels.Count);
            double step = 50.0 / beamSimpleModels.Count;

            List<WaitHandle> waitHandles = new List<WaitHandle>();
            int maxWaitHandleCount = 64;  //maximum is 64 // MC: zatial najlepsie sa u mna javi 8 cores a 8 waithandlecount => 19 sekund

            List<CBeamCalculations> recs = new List<CBeamCalculations>();
            List<IAsyncResult> results = new List<IAsyncResult>();
            CBeamCalculations recalc = null;
            IAsyncResult result = null;
            Object lockObject = new Object();

            foreach (CBeam_Simple beam in beamSimpleModels)
            {
                recalc = new CBeamCalculations(lockObject);
                result = recalc.BeginBeamCalculations(beam, bCalculateLoadCasesOnly, null, null);
                waitHandles.Add(result.AsyncWaitHandle);
                recs.Add(recalc);
                results.Add(result);
                if (waitHandles.Count >= maxWaitHandleCount)
                {
                    int index = WaitHandle.WaitAny(waitHandles.ToArray());
                    waitHandles.RemoveAt(index);
                    recs[index].EndBeamCalculations(results[index]);
                    recs.RemoveAt(index);
                    results.RemoveAt(index);
                    count++;
                    solverWindow.SetBeamsProgress(count, beamSimpleModels.Count);
                    solverWindow.Progress += step;
                    solverWindow.UpdateProgress();
                }
            }

            //if (waitHandles.Count > 0) WaitHandle.WaitAll(waitHandles.ToArray());
            //for (var i = 0; i < recs.Count; i++)
            //{
            //    recs[i].EndBeamCalculations(results[i]);
            //    count++;
            //    solverWindow.SetBeamsProgress(count, beamSimpleModels.Count);
            //    solverWindow.Progress += step;
            //    solverWindow.UpdateProgress();
            //}

            while (waitHandles.Count > 0)
            {
                int index = WaitHandle.WaitAny(waitHandles.ToArray());
                waitHandles.RemoveAt(index);
                recs[index].EndBeamCalculations(results[index]);
                recs.RemoveAt(index);
                results.RemoveAt(index);
                count++;
                solverWindow.SetBeamsProgress(count, beamSimpleModels.Count);
                solverWindow.Progress += step;
                solverWindow.UpdateProgress();
            }

            waitHandles.Clear();
            recs.Clear();
            results.Clear();
        }
    }
}
