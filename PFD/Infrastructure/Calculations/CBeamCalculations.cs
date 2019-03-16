using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public class CBeamCalculations
    {
        //-------------------------------------------------------------------------------------------------------------------------------
        BeamCalculationsAsyncStub stub = null;
        public delegate void BeamCalculationsAsyncStub(CModel beam, bool bCalculateLoadCasesOnly);
        private Object theLock;

        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        public CBeamCalculations(Object lockObject)
        {
            theLock = lockObject;
        }
        public void BeamCalculations(CModel beam, bool bCalculateLoadCasesOnly)
        {
            CModelToBFEMNetConverter.Convert(beam, bCalculateLoadCasesOnly);
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public IAsyncResult BeginBeamCalculations(CModel beam, bool bCalculateLoadCasesOnly, AsyncCallback cb, object s)
        {
            stub = new BeamCalculationsAsyncStub(BeamCalculations);
            //using delegate for asynchronous implementation   
            return stub.BeginInvoke(beam, bCalculateLoadCasesOnly, cb, null);
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public void EndBeamCalculations(IAsyncResult call)
        {
            stub.EndInvoke(call);
        }



    }
}
