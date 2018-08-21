namespace CRSC
{
    public class Enums
    {
        public enum ECrScShType0
        {

            //General
            eCrSc_0_00,
            eCrSc_0_01,
            eCrSc_0_02,
            eCrSc_0_03,
            eCrSc_0_04,

            // Concrete
            eCrSc_2_00,
            eCrSc_2_01,
            eCrSc_2_02,
            eCrSc_2_03,
            eCrSc_2_04,

            // Steel
            eCrSc_3_00,
            eCrSc_3_01,
            eCrSc_3_02,
            eCrSc_3_03,
            eCrSc_3_04,

            // Composite
            eCrSc_4_00,
            eCrSc_4_01,
            eCrSc_4_02,
            eCrSc_4_03,
            eCrSc_4_04,

            // Timber
            eCrSc_5_00,
            eCrSc_5_01,
            eCrSc_5_02,
            eCrSc_5_03,
            eCrSc_5_04,

            // Aluminium
            eCrSc_9_00,
            eCrSc_9_01,
            eCrSc_9_02,
            eCrSc_9_03,
            eCrSc_9_04
        }


        public enum ECrScShType1
        {
            eCrScType_I,   // I and H - section
            eCrScType_C,   // C and U (channel) - section
            eCrScType_L,   // L (angle) - section , equal and unequal
            eCrScType_T,   // T - section
            eCrScType_Z,   // Z - section
            eCrScType_BOX, // box - section, hollow - section (square and rectangular) 
            eCrScType_FB,  // flat bar
            eCrScType_RB,  // round bar
            eCrScType_TU,  // Tube
            eCrScType_GE   // General
        }

        public enum ECrScSym
        {
            eSym_D, // Doubly symmetrical cross-section
            eSym_M, // Monosymetrical cross-section
            eSym_C, // Centrally symmetrical cross-section
            eSym_A  // Asymetrical cross-section
        }

        public enum ECrScShType2
        {
            eO,  // Open cross-section
            eOC, // Open cross-section with closed parts
            eCO, // Closed cross-section with some outstanding parts
            eC,  // closed cross-section
            eS   // Solid cross-section
        }





    }
}
