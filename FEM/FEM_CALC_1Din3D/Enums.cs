using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEM_CALC_1Din3D
{
    //  DOF Vector Enum
    // Degrees of freedom 0-5 (6 - warping)
    public enum e3D_DOF
    {
        eUX = 0, // Displacement in X-Direction
        eUY = 1, // Displacement in Y-Direction
        eUZ = 2, // Displacement in Z-Direction
        eRX = 3, // Rotation around X-Axis
        eRY = 4, // Rotation around Y-Axis
        eRZ = 5, // Rotation around Z-Axis
        eW = 6   // Warping (not implemented yet)
    }

    //  Nodal Load Array Constants
    public enum e3D_E_F
    {
        eFX = 0,
        eFY = 1,
        eFZ = 2,

        eMX = 3,
        eMY = 4,
        eMZ = 5
    }
    
    // Internal Forces Enum
    public enum e3D_I_F
    {
      eNx = 0,
      eVy = 1,
      eVz = 2,

      eMx = 3,
      eMy = 4,
      eMz = 5
    }

    // Element SupportType
    public enum EElemSuppType3D
    {
        // UX, UY, UZ, RX, RY, RZ - see e3D_DOF

        e3DEl_000000_000000 = 0, // Start Node - restrained DOF,                                                    End Node - restrained DOF
        e3DEl_000000_______ = 1, // Start Node - restrained DOF,                                                    End Node - free DOF
        e3DEl________000000 = 2, // Start Node - free DOF,                                                          End Node - restrained DOF
        e3DEl_000000_000___ = 3, // Start Node - restrained DOF,                                                    End Node - rotation hinge
        e3DEl_000____000000 = 4, // Start Node - rotation hinge,                                                    End Node - restrained DOF
        e3DEl_000000_0_00_0 = 5, // Start Node - restrained DOF,                                                    End Node - free displacement in y-Axis nad rotation about y-Axis
        e3DEl_0_00_0_000000 = 6, // Start Node - free displacement in y-Axis nad rotation about y-Axis,             End Node - restrained DOF
        e3DEl_0000___0000__ = 7,
        e3DEl_0000____000__ = 8,
        e3DEl__000___0000__ = 9,
        e3DEl_000____000___ = 10, // Start Node - rotation hinge,                                                    End Node - rotation hinge
        e3DEl______________ = 99 // not supported member
    }

    //  DOF Vector Enum
    public enum e2D_DOF
    {
        eUX = 0,
        eUY = 1,
        eRZ = 2
    }
}
