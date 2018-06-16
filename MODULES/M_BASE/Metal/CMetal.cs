using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_BASE
{
    // Main eNum Types
    // Cross-section Shape Type
    public enum ECrScShType1
    {
        eCrScType_I,   // I and H - section
        eCrScType_C,   // C and U (channel) - section
        eCrScType_L,   // L (angle) - section , equal and unequal
        eCrScType_T,   // T - section
        eCrScType_Z,   // Z - section
        eCrScType_HL,  // HoLLow / box - section, hollow - section (square and rectangular) 
        eCrScType_FB,  // Flat Bar
        eCrScType_RB,  // Round Bar
        eCrScType_TU,  // TUbe
        eCrScType_CF,  // CruciForm symmetrical section
        eCrScType_GE   // General
    }

    public enum ECrScPrType1
    {
        eCrSc_rold,     // rolled
        eCrSc_wld,      // welded
        eCrSc_cldfrm,   // cold formed
        eCrSc_wldnorm,  // welded and normalized
        eCrSc_blt_up    // built-up
    }

    public enum ECrScSymmetry1
    {
        eDS,     // Doubly symmetrical
        eMS,     // Mono symmetrical
        eCS,     // Centrally symmetrical
        eAS,     // Asymmetrical section 
    }

    public enum ECrScMonoSymmetry1
    {
        eM_y,    // Symmetrical about y-Axis
        eM_z,    // Symmetrical about z-Axis
        eM_s,    // Symmetrical about strong (major) axis
        eM_w,    // Symmetrical about weak (minor) axis
    }




    // General Functions of all metal codes (steel, aluminium)
    // Load Definition and Moment Diagrams
    // Stress analyis, general resistances, critical bucling loads etc.

    public class CMetal
    {
        // Ratio of end moments at member
        public float Get_Psi_i___(float fM_1a, float fM_1b)
        {
            // Test zero denominator
            if ((Math.Abs(fM_1a) > Math.Abs(fM_1b)) && fM_1a != 0f)
                return Math.Min(Math.Max(-1, fM_1b / fM_1a), 1f);
            else if ((Math.Abs(fM_1b) > Math.Abs(fM_1a)) && fM_1b != 0f)
                return Math.Min(Math.Max(-1, fM_1a / fM_1b), 1f);
            else
                return 0f;
        }

    }
}
