using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public static class CFramesCalculations
    {
        public static void RunFramesCalculations(List<CFrame> framesModels, bool DeterminateCombinationResultsByFEMSolver, Solver solverWindow)
        {
            int count = 0;            
            solverWindow.SetFramesProgress(count, framesModels.Count);
            double step = 10.0 / framesModels.Count;

            List<WaitHandle> waitHandles = new List<WaitHandle>();
            int maxWaitHandleCount = 64;  //maximum is 64

            List<CFrameCalculations> recs = new List<CFrameCalculations>();
            List<IAsyncResult> results = new List<IAsyncResult>();
            CFrameCalculations recalc = null;
            IAsyncResult result = null;
            Object lockObject = new Object();

            foreach (CFrame frame in framesModels)
            {
                recalc = new CFrameCalculations(lockObject);
                result = recalc.BeginFrameCalculations(frame, DeterminateCombinationResultsByFEMSolver, null, null);
                waitHandles.Add(result.AsyncWaitHandle);
                recs.Add(recalc);
                results.Add(result);
                if (waitHandles.Count >= maxWaitHandleCount)
                {
                    int index = WaitHandle.WaitAny(waitHandles.ToArray());
                    waitHandles.RemoveAt(index);
                    recs[index].EndFrameCalculations(results[index]);
                    recs.RemoveAt(index);
                    results.RemoveAt(index);
                    count++;
                    solverWindow.SetFramesProgress(count, framesModels.Count);
                    solverWindow.Progress += step;
                    solverWindow.UpdateProgress();
                }
            }

            //if (waitHandles.Count > 0) WaitHandle.WaitAll(waitHandles.ToArray());
            //for (var i = 0; i < recs.Count; i++)
            //{
            //    recs[i].EndFrameCalculations(results[i]);
            //    count++;
            //    solverWindow.SetFramesProgress(count, framesModels.Count);
            //    solverWindow.Progress += step;
            //    solverWindow.UpdateProgress();
            //}
            while (waitHandles.Count > 0)
            {
                int index = WaitHandle.WaitAny(waitHandles.ToArray());
                waitHandles.RemoveAt(index);
                recs[index].EndFrameCalculations(results[index]);
                recs.RemoveAt(index);
                results.RemoveAt(index);
                count++;
                solverWindow.SetFramesProgress(count, framesModels.Count);
                solverWindow.Progress += step;
                solverWindow.UpdateProgress();
            }

            waitHandles.Clear();
            recs.Clear();
            results.Clear();
        }
    }
}
