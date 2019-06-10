using System;

namespace CRSC
{
    // SOLID / MASSIVE CROSS-SECTION PROPERTIES CALCULATION
    [Serializable]
    public class CCrSc_SO : CCrSc
    {
        public CCrSc_SO() { }

        protected override void loadCrScIndices()
        {
            throw new NotImplementedException();
        }

        protected override void loadCrScIndicesFrontSide()
        {
            throw new NotImplementedException();
        }

        protected override void loadCrScIndicesShell()
        {
            throw new NotImplementedException();
        }

        protected override void loadCrScIndicesBackSide()
        {
            throw new NotImplementedException();
        }

        public override void CalculateSectionProperties()
        {
            throw new NotImplementedException();
        }
    }
}
