using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public class CFrameCalculations
    {
        //-------------------------------------------------------------------------------------------------------------------------------
        FrameCalculationsAsyncStub stub = null;
        public delegate void FrameCalculationsAsyncStub(CModel frame, bool DeterminateCombinationResultsByFEMSolver);
        private Object theLock;

        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        public CFrameCalculations(Object lockObject)
        {
            theLock = lockObject;
        }


        public void FrameCalculations(CModel frame, bool DeterminateCombinationResultsByFEMSolver)
        {
            CModelToBFEMNetConverter.Convert(frame, DeterminateCombinationResultsByFEMSolver);
        }

        

        //-------------------------------------------------------------------------------------------------------------------------------
        public IAsyncResult BeginFrameCalculations(CModel frame, bool DeterminateCombinationResultsByFEMSolver, AsyncCallback cb, object s)
        {
            stub = new FrameCalculationsAsyncStub(FrameCalculations);
            //using delegate for asynchronous implementation   
            return stub.BeginInvoke(frame, DeterminateCombinationResultsByFEMSolver, cb, null);
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public void EndFrameCalculations(IAsyncResult call)
        {
            stub.EndInvoke(call);
        }



    }
}
