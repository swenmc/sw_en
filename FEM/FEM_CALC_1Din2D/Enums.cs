using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEM_CALC_1Din2D
{
    //  DOF Vector Enum
    public enum e2D_DOF
    {
        eUX = 0,
        eUY = 1,
        eRZ = 2
    }

    //  Nodal Load Array Constants
    public enum e2D_E_F
    {
        eFX = 0,
        eFY = 1,
        eMZ = 2
    }
    
    // Internal Forces Enum
    public enum e2D_I_F
    {
      eN = 0,
      eV = 1,
      eM = 2
    }

    //// Element SupportType
    //public enum EElemSuppType
    //{
    //    e2DEl_000_000 = 0,

    //    e2DEl_000_00_ = 1,
    //    e2DEl_000_0_0 = 2,
    //    e2DEl_000_0__ = 3,
    //    e2DEl_000__00 = 4,
    //    e2DEl_000___0 = 5,
    //    e2DEl_000____ = 6,

    //    e2DEl_00__000 = 7,
    //    e2DEl_0_0_000 = 8,
    //    e2DEl_0___000 = 9,
    //    e2DEl__00_000 = 10,
    //    e2DEl___0_000 = 11,
    //    e2DEl_____000 = 12,

    //    e2DEl_00__00_ = 13,
    //    e2DEl_0_0_0_0 = 14,
    //    e2DEl_0___0__ = 15,

    //    e2DEl__00__00 = 16,
    //    e2DEl___0___0 = 17,

    //    e2DEl_00___0_ = 18,
    //    e2DEl__0__00_ = 19,

    //    e2DEl_00__0__ = 20,
    //    e2DEl_0___00_ = 21,

    //    e2DEl_000__0_ = 22,
    //    e2DEl__0__000 = 23,

    //    e2DEl________ = 99 // not supported member
    //}
}
