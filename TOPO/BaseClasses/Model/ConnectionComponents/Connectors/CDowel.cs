using BaseClasses.GraphObj;
using BaseClasses.GraphObj.Objects_3D;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CDowel : CConnector
    {
        public CDowel()
        { }

        public CDowel(CPoint controlpoint, float fDiameter_temp, float fLength_temp, float fMass_temp, bool bIsDisplayed)
        {
            m_pControlPoint = controlpoint;
            BIsDisplayed = bIsDisplayed;
            Length = fLength_temp;
            Diameter_shank = fDiameter_temp;
            Mass = fMass_temp;

            m_DiffuseMat = new DiffuseMaterial(Brushes.DarkSalmon);
            m_cylinder = new Cylinder(0.5f * Diameter_thread, Length, m_DiffuseMat);
        }
    }
}
