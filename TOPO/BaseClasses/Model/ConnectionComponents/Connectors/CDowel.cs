﻿using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CDowel : CConnector
    {
        public CDowel()
        { }

        public CDowel(Point3D controlpoint, float fDiameter_temp, float fLength_temp, float fMass_temp)
        {
            ControlPoint = controlpoint;
            Length = fLength_temp;
            Diameter_shank = fDiameter_temp;

            Diameter_hole = GetDiameter_Hole();

            Mass = fMass_temp;

            m_DiffuseMat = new DiffuseMaterial(Brushes.DarkSalmon);
            //m_cylinder = new Cylinder(0.5f * Diameter_shank, Length, m_DiffuseMat);
        }
    }
}
