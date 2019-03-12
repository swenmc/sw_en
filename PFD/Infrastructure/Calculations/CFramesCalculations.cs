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
        public static void RunFramesCalculations(List<CFrame> framesModels, bool DeterminateCombinationResultsByFEMSolver)
        {
            List<WaitHandle> waitHandles = new List<WaitHandle>();
            int maxWaitHandleCount = 16;  //maximum is 64

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
                }
            }

            if (waitHandles.Count > 0) WaitHandle.WaitAll(waitHandles.ToArray());
            for (var i = 0; i < recs.Count; i++)
            {
                recs[i].EndFrameCalculations(results[i]);
            }
            waitHandles.Clear();
            recs.Clear();
            results.Clear();
        }
    }
}
