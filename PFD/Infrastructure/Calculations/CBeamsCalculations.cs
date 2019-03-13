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
        public static void RunBeamsCalculations(List<CBeam_Simple> beamSimpleModels, bool DeterminateCombinationResultsByFEMSolver)
        {
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
                result = recalc.BeginBeamCalculations(beam, DeterminateCombinationResultsByFEMSolver, null, null);
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
                }
            }

            if (waitHandles.Count > 0) WaitHandle.WaitAll(waitHandles.ToArray());
            for (var i = 0; i < recs.Count; i++)
            {
                recs[i].EndBeamCalculations(results[i]);
            }
            waitHandles.Clear();
            recs.Clear();
            results.Clear();
        }
    }
}
