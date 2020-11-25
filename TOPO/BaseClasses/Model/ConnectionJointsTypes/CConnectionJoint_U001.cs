using System;
using System.Windows;
using System.Collections.Generic;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Globalization;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;
using System.Linq;

namespace BaseClasses
{
    public class CConnectionJoint_U001 : CConnectionJointTypes
    {
        private CScrewArrangement_CB m_ScrewArrangement;

        // Cross Bracing to Main Column / Edge Column / Main Rafter / Edge Rafter / Purlin ???

        public bool m_bScrewInPlusZDirection;

        public CScrewArrangement_CB ScrewArrangement
        {
            get
            {
                return m_ScrewArrangement;
            }

            set
            {
                m_ScrewArrangement = value;
            }
        }

        public CConnectionJoint_U001() { }

        public CConnectionJoint_U001(CNode Node_temp, CMember MainMember_temp, CMember SecondaryConnectedMember_temp, bool bScrewInPlusZDirection)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainMember_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;
            m_bScrewInPlusZDirection = bScrewInPlusZDirection;

            float fAlignment_x = 0;

            if (m_SecondaryMembers[0] != null)
                fAlignment_x = -m_SecondaryMembers[0].FAlignment_Start;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            // Defaultny stav
            int iConnectorNumber = 4; // Default 4
            CScrew referenceScrew = new CScrew("TEK", "14");
            float fe_edge = 0.015f; // Defaultna vzdialenost of okraja 15 mm
            m_ScrewArrangement = new CScrewArrangement_CB(iConnectorNumber, referenceScrew, 2, 2, fe_edge, fe_edge, 0.05f, (float)m_SecondaryMembers[0].CrScStart.h - 2 * fe_edge);
            m_ScrewArrangement.Calc_HolesCentersCoord2D();
            m_ScrewArrangement.arrConnectorControlPoints3D = new Point3D[m_ScrewArrangement.IHolesNumber];
            m_ScrewArrangement.Calc_HolesControlPointsCoord3D_FlatPlate(0,0, (float)m_SecondaryMembers[0].CrScStart.t_min, m_bScrewInPlusZDirection);
            m_ScrewArrangement.GenerateConnectors_FlatPlate(m_bScrewInPlusZDirection);

            // TODO Ondrej - Task 616

            // Vytvorili sme objekty skrutiek v 3D - vid trieda CConnector
            // Takto vygenerovane skrutky smeruju v smere LCS z

            // Tuto skupinu skrutiek (cele screw arrangement_CB) potrebujeme podobne ako to robime u plates otocit a umiestnit v ramci spoja v LCS pruta na zaciatok a na koniec podla toho na ktorom uzle pruta sa spoj nachadza
            // Mali by sme k tomu pouzit ControlPoint_P1 a RotationVector_P1

            // Asi by to malo fungovat tak, že objekt CConnectorArrangement, resp. CScrewArrangement bude mať pre 3D transformačné funkcie podobné ako CPlate
            // Tomuto objektu by sa potom nastavoval ControlPoint_P1 a RotationVector_P1 a podla tohto by sa transformovali vsetci Screws ktore su sucastou objektu, resp. vsetkych jeho groups a ich sekvencii
            // Treba si vsak uvedomit ze CScrewArrangement ako take nie je realny 3D utvar, ale len zdruzuje jednotlive realne CScrews, aby sa s nimi dalo pracovat ako s jednou skupinkou

            // Alebo potrebujeme vytvorit nejaky zastupny objekt, nieco ako virtualPlate v ramci ktoreho sa budu Screws vykreslovat a transformovat ako celok, hoci objekt by realne vlastne neexistoval

            // Tretou moznostou je, ze CScrewArrangement by patrilo pod CMember ktory je pripajany, ale tu je problem ze U001 sa priradzuje CMember, takze by sa to asi cyklilo


            //m_arrConnectors = new CConnector[iConnectorNumber];

            //// Nakopirujeme skrutky z screw arrangement do pola skrutiek patriacich priamo do jointu (mozno by sa to mohlo zjednotit s tym ze v jointe nebude pole connectors ale priamo connector arrangement)
            //for(int i = 0; i < sa.Screws.Length; i++)
            //{
            //    m_arrConnectors[i] = sa.Screws[i];
            //}

            if (ConnectorGroups == null) ConnectorGroups = new List<CConnectorGroup>();
            ConnectorGroups.Add(new CConnectorGroup(m_ScrewArrangement.Screws));

            ConnectorGroups.First().ControlPoint = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
            ConnectorGroups.First().RotationVector = new Vector3D(90, 0, 0);

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                if (m_SecondaryMembers[0] != null)
                    fAlignment_x = -m_SecondaryMembers[0].FAlignment_End;

                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ConnectorGroups.First().ControlPoint = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
                ConnectorGroups.First().RotationVector = new Vector3D(180 + 90, 0, 180 + 0);
            }
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_U001(m_Node, m_MainMember, m_SecondaryMembers[0], m_bScrewInPlusZDirection);
        }

        public override void UpdateJoint()
        {
            float fAlignment_x = 0;

            if (m_SecondaryMembers[0] != null)
                fAlignment_x = -m_SecondaryMembers[0].FAlignment_Start;


            ConnectorGroups = new List<CConnectorGroup>();
            ConnectorGroups.Add(new CConnectorGroup(m_ScrewArrangement.Screws));
            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            ConnectorGroups.First().ControlPoint = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
            ConnectorGroups.First().RotationVector = new Vector3D(90, 0, 0);

            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                if (m_SecondaryMembers[0] != null)
                    fAlignment_x = -m_SecondaryMembers[0].FAlignment_End;

                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ConnectorGroups.First().ControlPoint = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
                ConnectorGroups.First().RotationVector = new Vector3D(180 + 90, 0, 180 + 0);
            }
        }

        public void UpdateJointScrewArrangementData()
        {
            if (ScrewArrangement != null)
                ScrewArrangement.UpdateArrangmentData();

            //ScrewArrangement.Calc_HolesCentersCoord2D((float)m_SecondaryMembers[0].CrScStart.h, (float)m_SecondaryMembers[0].CrScStart.h - 2 * 0.02f);
            ScrewArrangement.Calc_HolesCentersCoord2D();
            ScrewArrangement.arrConnectorControlPoints3D = new Point3D[ScrewArrangement.IHolesNumber];
            ScrewArrangement.Calc_HolesControlPointsCoord3D_FlatPlate(0, 0, (float)m_SecondaryMembers[0].CrScStart.t_min, m_bScrewInPlusZDirection);
            ScrewArrangement.GenerateConnectors_FlatPlate(m_bScrewInPlusZDirection);
        }

    }
}
