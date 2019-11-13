using System;

namespace BaseClasses
{
    // Class CReinforcementBar
    [Serializable]
    public class CReinforcementMesh : CEntity3D
    {
        private string m_MaterialName;
        private double m_wireDiameter;
        private double m_centersDistance;

        public string MaterialName
        {
            get
            {
                return m_MaterialName;
            }

            set
            {
                m_MaterialName = value;
            }
        }

        public double WireDiameter
        {
            get
            {
                return m_wireDiameter;
            }

            set
            {
                m_wireDiameter = value;
            }
        }

        public double CentersDistance
        {
            get
            {
                return m_centersDistance;
            }

            set
            {
                m_centersDistance = value;
            }
        }

        public CReinforcementMesh(string GradeName)
        {
            Name = GradeName;

            // Nacitame vlastnosti z databazy
            DATABASE.DTO.CMeshProperties prop = new DATABASE.DTO.CMeshProperties();
            prop = DATABASE.CMeshesManager.GetMeshProperties(Name);

            // Nastavime vlastnosti objektu
            m_Mat = new MATERIAL.CMat_03_00();
            m_Mat.Name = prop.MaterialName;
            m_wireDiameter = prop.WireDiameter;
            m_centersDistance = prop.CentersDistance;
        }
    }
}
