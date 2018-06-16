using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    public enum eCH_EC3
    {
        // N
        eCH_EC3_10000,     // None or too small design forces
        eCH_EC3_10101,     // Tension force Nt
        eCH_EC3_10151,     // Tension force Nt and torsional moment Mx
        eCH_EC3_10201,     // Compression force Nc
        eCH_EC3_10251,     // Compression force Nc and torsional moment Mx

        // N and Vz
        eCH_EC3_10301,     // Tension force Nt and shear force Vz
        eCH_EC3_10351,     // Tension force Nt, shear force Vz and torsional moment Mx
        eCH_EC3_10401,     // Compression force Nc and shear force Vz
        eCH_EC3_10451,     // Compression force Nc, shear force Vz and torsional moment Mx

        // N and Vy
        eCH_EC3_10501,     // Tension force Nt and shear force Vy
        eCH_EC3_10551,     // Tension force Nt, shear force Vy and torsional moment Mx
        eCH_EC3_10601,     // Compression force Nc and shear force Vy
        eCH_EC3_10651,     // Compression force Nc, shear force Vy and torsional moment Mx

        // N, Vz + Vy
        eCH_EC3_10701,     // Tension force Nt, shear force Vz and shear force Vy
        eCH_EC3_10751,     // Tension force Nt, shear dorce Vz, shear force Vy and torsional moment Mx
        eCH_EC3_10801,     // Compression force Nc, shear force Vz and shear force Vy
        eCH_EC3_10851,     // Compression force Nc, shear dorce Vz, shear force Vy and torsional moment Mx

        // V
        eCH_EC3_20101,     // Shear force Vz
        eCH_EC3_20151,     // Shear force Vz and torsional moment Mx
        eCH_EC3_20201,     // Shear force Vy
        eCH_EC3_20251,     // Shear force Vy and torsional moment Mx
        eCH_EC3_20301,     // Shear force Vz and shear force Vy
        eCH_EC3_20351,     // Shear force Vz, shear force Vy and torsional moment Mx

        // T
        eCH_EC3_30001,     // Torsional moment Mx
  
        // M
        eCH_EC3_40101,     // Bending moment My
        eCH_EC3_40151,     // Bending moment My and torsional moment Mx
        eCH_EC3_40201,     // Bending moment Mz
        eCH_EC3_40251,     // Bending moment Mz and torsional moment Mx
        eCH_EC3_40301,     // Bending moment My and Mz
        eCH_EC3_40351,     // Bending moment My, bending moment Mz and torsional moment Mx

        // M + V
        eCH_EC3_50101,     // Shear force Vz and bending moment My
        eCH_EC3_50151,     // Shear force Vz and bending moment My and torsional moment Mx
        eCH_EC3_50201,     // Shear force Vy and bending moment My
        eCH_EC3_50251,     // Shear force Vy and bending moment My and torsional moment Mx
        eCH_EC3_50301,     // Shear force Vy, shear force Vz and bending moment My
        eCH_EC3_50351,     // Shear force Vy, shear force Vz and bending moment My and torsional moment Mx

        eCH_EC3_50401,     // Shear force Vy and bending moment Mz
        eCH_EC3_50451,     // Shear force Vy and bending moment Mz and torsional moment Mx
        eCH_EC3_50501,     // Shear force Vz and bending moment Mz
        eCH_EC3_50551,     // Shear force Vz and bending moment Mz and torsional moment Mx
        eCH_EC3_50601,     // Shear force Vy, shear force Vz and bending moment Mz
        eCH_EC3_50651,     // Shear force Vy, shear force Vz and bending moment Mz and torsional moment Mx

        eCH_EC3_50701,     // Shear force Vy, bending moment My and bending moment Mz
        eCH_EC3_50751,     // Shear force Vy, bending moment My, bending moment Mz and torsional moment Mx
        eCH_EC3_50801,     // Shear force Vz, bending moment My and bending moment Mz
        eCH_EC3_50851,     // Shear force Vz, bending moment My, bending moment Mz and torsional moment Mx
        eCH_EC3_50901,     // Shear force Vy, shear force Vz, bending moment My and bending moment Mz
        eCH_EC3_50951,     // Shear force Vy, shear force Vz, bending moment My, bending moment Mz and torsional moment Mx


        // N + My
        eCH_EC3_60101,     // Tension force Nt, shear force Vz and bending moment My
        eCH_EC3_60151,     // Tension force Nt, shear force Vz, bending moment My and torsional moment Mx
        eCH_EC3_60201,     // Tension force Nt, shear force Vy and bending moment My
        eCH_EC3_60251,     // Tension force Nt, shear force Vy, bending moment My and torsional moment Mx
        eCH_EC3_60301,     // Tension force Nt, shear force Vy, shear force Vz and bending moment My
        eCH_EC3_60351,     // Tension force Nt, shear force Vy, shear force Vz, bending moment My and torsional moment Mx

        eCH_EC3_60401,     // Compression force Nc, shear force Vz and bending moment My
        eCH_EC3_60451,     // Compression force Nc, shear force Vz, bending moment My and torsional moment Mx
        eCH_EC3_60501,     // Compression force Nc, shear force Vy and bending moment My
        eCH_EC3_60551,     // Compression force Nc, shear force Vy, bending moment My and torsional moment Mx
        eCH_EC3_60601,     // Compression force Nc, shear force Vy, shear force Vz and bending moment My
        eCH_EC3_60651,     // Compression force Nc, shear force Vy, shear force Vz, bending moment My and torsional moment Mx

        // N + Mz
        eCH_EC3_60701,     // Tension force Nt, shear force Vz and bending moment Mz
        eCH_EC3_60751,     // Tension force Nt, shear force Vz, bending moment Mz and torsional moment Mx
        eCH_EC3_60801,     // Tension force Nt, shear force Vy and bending moment Mz
        eCH_EC3_60851,     // Tension force Nt, shear force Vy, bending moment Mz and torsional moment Mx
        eCH_EC3_60901,     // Tension force Nt, shear force Vy, shear force Vz and bending moment Mz
        eCH_EC3_60951,     // Tension force Nt, shear force Vy, shear force Vz, bending moment Mz and torsional moment Mx

        eCH_EC3_61001,     // Compression force Nc, shear force Vz and bending moment Mz
        eCH_EC3_61051,     // Compression force Nc, shear force Vz, bending moment Mz and torsional moment Mx
        eCH_EC3_61101,     // Compression force Nc, shear force Vy and bending moment Mz
        eCH_EC3_61151,     // Compression force Nc, shear force Vy, bending moment Mz and torsional moment Mx
        eCH_EC3_61201,     // Compression force Nc, shear force Vy, shear force Vz and bending moment Mz
        eCH_EC3_61251,     // Compression force Nc, shear force Vy, shear force Vz, bending moment Mz and torsional moment Mx

        // N + My + Mz
        eCH_EC3_61301,     // Tension force Nt, shear force Vy and bending moment My and bending moment Mz
        eCH_EC3_61351,     // Tension force Nt, shear force Vy, bending moment My, bending moment Mz and torsional moment Mx
        eCH_EC3_61401,     // Tension force Nt, shear force Vz and bending moment My and bending moment Mz
        eCH_EC3_61451,     // Tension force Nt, shear force Vz, bending moment My, bending moment Mz and torsional moment Mx
        eCH_EC3_61501,     // Tension force Nt, shear force Vy, shear force Vz, bending moment My and bending moment Mz
        eCH_EC3_61551,     // Tension force Nt, shear force Vy, shear force Vz, bending moment My, bending moment Mz and torsional moment Mx

        eCH_EC3_61601,     // Compression force Nc, shear force Vy and bending moment My and bending moment Mz
        eCH_EC3_61651,     // Compression force Nc, shear force Vy, bending moment My, bending moment Mz and torsional moment Mx
        eCH_EC3_61701,     // Compression force Nc, shear force Vz and bending moment My and bending moment Mz
        eCH_EC3_61751,     // Compression force Nc, shear force Vz, bending moment My, bending moment Mz and torsional moment Mx
        eCH_EC3_61801,     // Compression force Nc, shear force Vy, shear force Vz, bending moment My and bending moment Mz
        eCH_EC3_61851,     // Compression force Nc, shear force Vy, shear force Vz, bending moment My, bending moment Mz and torsional moment Mx


        // Overall Stability
        // Lateral torsional bending
        eCH_EC3_80101,     // Bending moment My
        //eCH_EC3_8201,     // Bending moment Mz
        eCH_EC3_80301,     // Bending moment My and bending moment Mz

        // Compression force and bending
        eCH_EC3_90001,     // Axial compression force Nc
        eCH_EC3_90101,     // Axial compression force Nc and bending moment My
        eCH_EC3_90201,     // Axial compression force Nc and bending moment Mz
        eCH_EC3_90301      // Axial compression force Nc, bending moment My and bending moment Mz


        // Idea of global modeling

        /* beam, segment Mz, Mz, cross-section*/
    };

    class CH_ENUM
    {
    }
}
