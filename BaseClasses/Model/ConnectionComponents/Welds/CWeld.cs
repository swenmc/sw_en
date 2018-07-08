using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    public abstract class CWeld : CConnectionComponentEntity3D
    {
        public CWeld()
        {
            BIsDisplayed = true;
        }

        public CWeld(bool bIsDisplayed)
        {
            BIsDisplayed = bIsDisplayed;
        }

        protected override void loadIndices()
        { }
    }
}