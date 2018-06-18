using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    public abstract class CBolt : CConnectionComponentEntity3D
    {
        public CBolt()
        {
            BIsDisplayed = true;
        }

        public CBolt(bool bIsDisplayed)
        {
            BIsDisplayed = bIsDisplayed;
        }

        protected override void loadIndices()
        { }
    }
}