using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC5
{
    /// <summary>
    /// 
    ///
    /// 
    /// Comments
    /// in each modul EC2, EC3, EC4, EC5, EC9 will (directory "EC2", ....)
    /// we will creat same (or very similar) system of namespaces, classes and methods
    /// 
    /// "INP" is namespace where input data and method (about cross-cestion, member section, material ...) will be loaded 
    /// from database or dialogs (given by user) and prepared for calculation
    /// 
    /// "SOLV" is namespace where are stored method and classes necessary for calculation (algorithm data)
    /// 
    /// "OUT" is namespace where are preprared data for saving and displaying in results tables
    /// 
    ///  "SOLV" -> "CS" - checks of cross-section
    ///  "SOLV" -> "ST" - checks of stability
    ///  
    ///  "_1_EQ" - namaspace where are defined classes and functions equations) acc. to code (standard) clauses (name of class)
    ///            these functons return just one value of variable
    ///  
    ///  "_2_CL" - namespace where are defined functions based on "groups" of equations for simplier using in checks, auxialiary methods could be defined
    ///          - need to use functions from "_1_EQ"
    ///          - these functions return many variables (values)
    ///          
    ///  "_3_CH" - namespace where are defined checks "final functions and ratio", output variables are inicialized here, final ratio should be saved separately
    ///          - need to use functions from "_1_EQ" and "_2_CL"
    ///          - subsystem of folders is created for simplier orientation, each "check" (as defined in enum bellow) has its own input and output data,
    ///            which we need to load and save separately (in addittion is effective to save just final ratio
    ///            Theredore is created separate class for each check (named and numbered as bellow)
    ///            Numbers 8000-10000 are for Stability design
    ///            these function return final ratio and many other auxiliary vaules which we would like to display on screen
    ///      
    ///            
    /// Finally, now I need to prepare system of structures, functions, classes and namespaces.
    /// For final output displaying is maybe better to do not save input data which are used in check (cross-section and material properties) in "_3_CH"
    /// just load them from "INP" namespace or other namespaces.
    /// In main calculation we will save just final ratios. For displaying details is better to run particular check again and save all output data.
    /// Find idea how to save saparately final check ratio (one float value from each check) and other output data separately
    /// other data will be displayed just in case that user choose to display details of selected check (it could be eventualy calculated again and save for displaying)
    /// 
    /// 
    /// </summary>

    public enum eCH_EC5
    {
        eCH_EC5_1000,     // None or too small design forces
        eCH_EC5_1101,     // Axial tension force Nt
        eCH_EC5_1151,     // Axial tension force Nt and torsional moment Mx
        eCH_EC5_1201,     // Axial compression force Nc
        eCH_EC5_1251,     // Axial compression force Nc and torsional moment Mx
        eCH_EC5_1301,     // Axial tension force Nt and shear force Vz
        eCH_EC5_1351,     // Axial tension force Nt, shear force Vz and torsional moment Mx
        eCH_EC5_1401,     // Axial compression force Nc and shear force Vz
        eCH_EC5_1451,     // Axial compression force Nc, shear force Vz and torsional moment Mx
        eCH_EC5_1501,     // Axial tension force Nt and shear force Vy
        eCH_EC5_1551,     // Axial tension force Nt, shear force Vy and torsional moment Mx
        eCH_EC5_1601,     // Axial compression force Nc and shear force Vy
        eCH_EC5_1651,     // Axial compression force Nc, shear force Vy and torsional moment Mx
        eCH_EC5_1701,     // Axial tension force Nt, shear force Vz and shear force Vy
        eCH_EC5_1751,     // Axial tension force Nt, shear dorce Vz, shear force Vy and torsional moment Mx
        eCH_EC5_1801,     // Axial compression force Nc, shear force Vz and shear force Vy
        eCH_EC5_1851,     // Axial compression force Nc, shear dorce Vz, shear force Vy and torsional moment Mx

        eCH_EC5_2101,     // Shear force Vz
        eCH_EC5_2151,     // Shear force Vz and torsional moment Mx
        eCH_EC5_2201,     // Shear force Vy
        eCH_EC5_2251,     // Shear force Vy and torsional moment Mx
        eCH_EC5_2301,     // Shear force Vz and shear force Vy
        eCH_EC5_2351,     // Shear force Vz, shear force Vy and torsional moment Mx

        eCH_EC5_3001,     // Torsional moment Mx

        eCH_EC5_4101,     // Bending moment My
        eCH_EC5_4151,     // Bending moment My and torsional moment Mx
        eCH_EC5_4201,     // Bending moment Mz
        eCH_EC5_4251,     // Bending moment Mz and torsional moment Mx
        eCH_EC5_4301,     // Bending moment My and Mz
        eCH_EC5_4351,     // Bending moment My, bending moment Mz and torsional moment Mx

        eCH_EC5_5101,     // Shear force Vz and bending moment My
        eCH_EC5_5151,     // Shear force Vz and bending moment My and torsional moment Mx
        eCH_EC5_5201,     // Shear force Vy and bending moment Mz
        eCH_EC5_5251,     // Shear force Vy and bending moment Mz and torsional moment Mx
        eCH_EC5_5301,     // Shear force Vy, shear force Vz, bending moment My and bending moment Mz
        eCH_EC5_5351,     // Shear force Vy, shear force Vz, bending moment My, bending moment Mz and torsional moment Mx

        eCH_EC5_6101,     // Tension force Nt, shear force Vz and bending moment My
        eCH_EC5_6151,     // Tension force Nt, shear force Vz, bending moment My and torsional moment Mx
        eCH_EC5_6201,     // Compression force Nc, shear force Vz and bending moment My
        eCH_EC5_6251,     // Compression force Nc, shear force Vz, bending moment My and torsional moment Mx
        eCH_EC5_6301,     // Tension force Nt, shear force Vy and bending moment Mz
        eCH_EC5_6351,     // Tension force Nt, shear force Vy, bending moment Mz and torsional moment Mx
        eCH_EC5_6401,     // Compression force Nc, shear force Vy and bending moment Mz
        eCH_EC5_6451,     // Compression force Nc, shear force Vy, bending moment Mz and torsional moment Mx
        eCH_EC5_6501,     // Tension force Nt, shear force Vz, shear force Vy, bending moment My and bending moment Mz
        eCH_EC5_6551,     // Tension force Nt, shear force Vz, shear force Vy, bending moment My, bending moment Mz and torsional moment Mx
        eCH_EC5_6601,     // Compression force Nc, shear force Vz, shear force Vy, bending moment My and bending moment Mz
        eCH_EC5_6651,     // Compression force Nc, shear force Vz, shear force Vy, bending moment My, bending moment Mz and torsional moment Mx

        eCH_EC5_8101,     // Bending moment My
        eCH_EC5_8201,     // Bending moment Mz
        eCH_EC5_8301,     // Bending moment My and bending moment Mz

        eCH_EC5_9001,     // Axial compression force Nc
        eCH_EC5_9101,     // Axial compression force Nc and bending moment My
        eCH_EC5_9201,     // Axial compression force Nc and bending moment Mz
        eCH_EC5_9301      // Axial compression force Nc, bending moment My and bending moment Mz


         // Idea of global modeling

        /* beam, segment Mz, Mz, cross-section*/
    };
    class CH_ENUM
    {
    }
}
