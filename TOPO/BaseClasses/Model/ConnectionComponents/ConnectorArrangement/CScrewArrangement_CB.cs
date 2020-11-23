using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_CB:CScrewArrangement
    {
        // TODO 624
        // Tento objekt moze byt samostatny alebo ho mozeme zlucit s CScrewArrangementRect_PlateType_JKL
        // Podla mna by mal byt radsej samostatny kedze tento objekt ma byt nezavisly na plates, ale obsah Calc_HolesCentersCoord2D by sa refaktoroval

        private int m_NumberOfGroups;  //default 2
        private int m_NumberOfGroupsWithoutMirrored;
        private int m_DefaultNumberOfSequencesInGroup;
        private bool m_MirroredGroups;
        private List<CScrewRectSequence> m_RectSequences;

        public CScrewArrangement_CB() { }

        public CScrewArrangement_CB(int iScrewsNumber_temp, CScrew referenceScrew_temp, int iNumberOfScrewsInRow_xDirection, int iNumberOfScrewsInColumn_yDirection, float refPointX, float refPointY, float distanceOfPointsX, float distanceOfPointsY)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
            
            m_MirroredGroups = false;
            
            RectSequences = new List<CScrewRectSequence>();
            RectSequences.Add(new CScrewRectSequence(iNumberOfScrewsInRow_xDirection, iNumberOfScrewsInColumn_yDirection, refPointX, refPointY, distanceOfPointsX, distanceOfPointsY));
            
            NumberOfGroups = 1;
            //NumberOfSequenceInGroup = 1;
            DefaultNumberOfSequencesInGroup = 1;

            IHolesNumber = 0;
            foreach (CScrewRectSequence rectS in RectSequences)
            {
                IHolesNumber += rectS.INumberOfConnectors;
            }
            if (MirroredGroups) IHolesNumber = IHolesNumber * 2;

            UpdateArrangmentData();
        }

        public bool MirroredGroups
        {
            get
            {
                return m_MirroredGroups;
            }

            set
            {
                m_MirroredGroups = value;
            }
        }

        public int NumberOfGroups
        {
            get
            {
                return m_NumberOfGroups;
            }

            set
            {
                m_NumberOfGroups = value;
            }
        }

        public int NumberOfGroupsWithoutMirrored
        {
            get
            {
                return m_NumberOfGroupsWithoutMirrored;
            }

            set
            {
                m_NumberOfGroupsWithoutMirrored = value;
            }
        }

        public List<CScrewRectSequence> RectSequences
        {
            get
            {
                return m_RectSequences;
            }

            set
            {
                m_RectSequences = value;
            }
        }

        public int DefaultNumberOfSequencesInGroup
        {
            get
            {
                return m_DefaultNumberOfSequencesInGroup;
            }

            set
            {
                m_DefaultNumberOfSequencesInGroup = value;
            }
        }

        public void Calc_HolesCentersCoord2D(float fhY, float fx_edge, float fy_edge, float fx/*, float fy*/)
        {
            /*
            float fx_edge = 0.03f;  // x-direction
            float fy_edge = 0.02f;  // y-direction
            float fx = 0.06f;  // x-direction
            float fy = 0.06f;  // y-direction
            */

            // TODO 624 - urobit toto dynamicke, aby sa generovalo podla rect screw arrangement - rovnake alebo rozne vzdialenosti medzi jednotlivymi radmi alebo stlpcami skrutiek
            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                HolesCentersPoints2D[0] = new Point(fx_edge, fy_edge);

                HolesCentersPoints2D[1] = new Point(fx_edge, fhY - fy_edge);

                HolesCentersPoints2D[2] = new Point(fx_edge + fx, fy_edge);

                HolesCentersPoints2D[3] = new Point(fx_edge + fx, fhY - fy_edge);

                HolesCentersPoints2D[4] = new Point(fx_edge + 2*fx, fy_edge);

                HolesCentersPoints2D[5] = new Point(fx_edge + 2*fx, fhY - fy_edge);
            }
        }


        public override void UpdateArrangmentData()
        {
            // TODO - toto prerobit tak ze sa parametre prevedu na cisla a nastavia v CTEKScrewsManager a nie tu
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            // Update reference screw properties
            DATABASE.DTO.CTEKScrewProperties screwProp = DATABASE.CTEKScrewsManager.GetScrewProperties(referenceScrew.Gauge.ToString());
            referenceScrew.Diameter_thread = float.Parse(screwProp.threadDiameter, nfi) / 1000; // Convert mm to m


            //co je toto za blud? to nanovo stale vytvarame?
            //upravene,ze iba ak nie je inicializovane vobec tak vtedy
            if (ListOfSequenceGroups == null || ListOfSequenceGroups.Count == 0)
            {
                ListOfSequenceGroups = new List<CScrewSequenceGroup>(NumberOfGroups);
                int index = 0;
                for (int i = 0; i < NumberOfGroups; i++)
                {
                    CScrewSequenceGroup gr = new CScrewSequenceGroup();
                    gr.NumberOfHalfCircleSequences = 0;
                    gr.NumberOfRectangularSequences = DefaultNumberOfSequencesInGroup;

                    for (int j = 0; j < DefaultNumberOfSequencesInGroup; j++)
                    {
                        // Pridame sekvenciu do skupiny
                        if (MirroredGroups)
                        {
                            //toto tu nechapem, preco len ak je i = 0
                            if (i == 0) gr.ListSequence.Add(RectSequences[j]);
                            else gr.ListSequence.Add(RectSequences[j].Copy());
                        }
                        else
                        {
                            gr.ListSequence.Add(RectSequences[index]);
                            index++;
                        }
                    }
                    ListOfSequenceGroups.Add(gr);
                }
            }

            // Celkovy pocet skrutiek, pocet moze byt v kazdej sekvencii rozny
            RecalculateTotalNumberOfScrews();

            HolesCentersPoints2D = new Point[IHolesNumber];
            arrConnectorControlPoints3D = new Point3D[IHolesNumber];
        }


        public void MirrorGroupsChanged(bool mirrorGroups)
        {
            if (mirrorGroups) //zaskrtnute = polku treba zmazat
            {
                RectSequences.RemoveRange(RectSequences.Count / 2, RectSequences.Count / 2);
            }
            else
            {
                int originalCount = RectSequences.Count;
                for (int i = 0; i < originalCount; i++)
                {
                    RectSequences.Add(RectSequences[i].Copy());
                }
            }

            System.Diagnostics.Trace.WriteLine("----------------------------------------");
            System.Diagnostics.Trace.WriteLine("NumberOfGroups:" + NumberOfGroups);
            System.Diagnostics.Trace.WriteLine("NumberOfGroupsWithoutMirrored:" + NumberOfGroupsWithoutMirrored);
            System.Diagnostics.Trace.WriteLine("MirroredGroups:" + MirroredGroups);
            System.Diagnostics.Trace.WriteLine("RectSequences:" + RectSequences.Count);
        }

        public void NumberOfGroups_Updated(int newNumberOfGroups)
        {
            if (newNumberOfGroups < 0) return;
            if (newNumberOfGroups > 5) return;

            if (!MirroredGroups)
            {
                if (newNumberOfGroups < NumberOfGroups)
                {
                    while (newNumberOfGroups < NumberOfGroups)
                    {
                        RemoveSequenceGroup();
                        NumberOfGroups--;
                    }
                }
                else if (newNumberOfGroups > NumberOfGroups)
                {
                    while (newNumberOfGroups > NumberOfGroups)
                    {
                        AddSequenceGroup();
                        NumberOfGroups++;
                    }
                }
            }
            else //su mirorovane
            {
                if (newNumberOfGroups < NumberOfGroupsWithoutMirrored)
                {
                    while (newNumberOfGroups < NumberOfGroupsWithoutMirrored)
                    {
                        RemoveMirroredSequenceGroup();
                        NumberOfGroups -= 2;
                    }
                }
                else if (newNumberOfGroups > NumberOfGroupsWithoutMirrored)
                {
                    while (newNumberOfGroups > NumberOfGroupsWithoutMirrored)
                    {
                        AddMirroredSequenceGroup();
                        NumberOfGroups += 2;
                    }
                }

            }

        }
        public void NumberOfSequenceInGroup_Updated(int groupID, int newNumberOfSequencesInGroup)
        {
            if (newNumberOfSequencesInGroup < 0) return;
            if (newNumberOfSequencesInGroup > 10) return;

            int groupIndex = groupID - 1;

            CScrewSequenceGroup gr = ListOfSequenceGroups.ElementAtOrDefault(groupIndex);
            if (gr == null) return;
            UpdateSequencesInGroup(gr, groupIndex, newNumberOfSequencesInGroup);

            if (MirroredGroups)
            {
                CScrewSequenceGroup grMirror = ListOfSequenceGroups.ElementAtOrDefault(groupIndex + NumberOfGroupsWithoutMirrored);
                UpdateSequencesInGroup(grMirror, groupIndex + NumberOfGroupsWithoutMirrored, newNumberOfSequencesInGroup);
            }
        }

        private void UpdateSequencesInGroup(CScrewSequenceGroup gr, int groupIndex, int newNumberOfSequencesInGroup)
        {
            if (newNumberOfSequencesInGroup < gr.NumberOfRectangularSequences)
            {
                while (newNumberOfSequencesInGroup < gr.NumberOfRectangularSequences)
                {
                    RemoveSequenceFromGroup(groupIndex);
                    gr.NumberOfRectangularSequences--;
                }
            }
            else if (newNumberOfSequencesInGroup > gr.NumberOfRectangularSequences)
            {
                while (newNumberOfSequencesInGroup > gr.NumberOfRectangularSequences)
                {
                    AddSequenceToGroup(groupIndex);
                    gr.NumberOfRectangularSequences++;
                }
            }

        }

        private void AddSequenceGroup()
        {
            //CScrewSequenceGroup gr_toCopy = ListOfSequenceGroups.FirstOrDefault();
            int rectSeqNum = 1;
            //if (gr_toCopy != null) rectSeqNum = gr_toCopy.NumberOfRectangularSequences;

            CScrewSequenceGroup gr = new CScrewSequenceGroup();
            for (int i = 0; i < rectSeqNum; i++)
            {
                CScrewRectSequence rS = new CScrewRectSequence();
                RectSequences.Add(rS);
                gr.ListSequence.Add(rS);
            }
            ListOfSequenceGroups.Add(gr);
        }
        private void RemoveSequenceGroup()
        {
            CScrewSequenceGroup gr = ListOfSequenceGroups.LastOrDefault();
            int rectSeqNum = gr.NumberOfRectangularSequences;
            if (gr == null) return;

            ListOfSequenceGroups.Remove(gr);
            for (int i = 0; i < rectSeqNum; i++)
            {
                RectSequences.RemoveAt(RectSequences.Count - 1);
            }
        }
        private void AddMirroredSequenceGroup()
        {
            CScrewSequenceGroup gr = new CScrewSequenceGroup();
            CScrewRectSequence rS = new CScrewRectSequence();
            RectSequences.Add(rS);
            gr.ListSequence.Add(rS);
            ListOfSequenceGroups.Insert(NumberOfGroupsWithoutMirrored, gr);

            CScrewSequenceGroup grMirror = new CScrewSequenceGroup();
            CScrewRectSequence rSMirror = new CScrewRectSequence();
            grMirror.ListSequence.Add(rSMirror);
            ListOfSequenceGroups.Add(grMirror);
        }
        private void RemoveMirroredSequenceGroup()
        {
            CScrewSequenceGroup gr = ListOfSequenceGroups[NumberOfGroupsWithoutMirrored - 1];
            CScrewSequenceGroup grMirrored = ListOfSequenceGroups.LastOrDefault();

            int rectSeqNum = gr.NumberOfRectangularSequences;
            if (gr == null) return;

            ListOfSequenceGroups.Remove(gr);
            for (int i = 0; i < rectSeqNum; i++)
            {
                RectSequences.RemoveAt(RectSequences.Count - 1);
            }

            if (grMirrored == null) return;
            ListOfSequenceGroups.Remove(grMirrored);
        }

        private void AddSequenceToGroup(int grIndex)
        {
            int seqIndex = GetTotalSequenceIndex(grIndex + 1, 0);
            CScrewRectSequence rS = new CScrewRectSequence();

            ListOfSequenceGroups[grIndex].ListSequence.Add(rS);
            if (RectSequences.Count >= seqIndex) RectSequences.Insert(seqIndex, rS);
        }
        private void RemoveSequenceFromGroup(int grIndex)
        {
            ListOfSequenceGroups[grIndex].ListSequence.RemoveAt(ListOfSequenceGroups[grIndex].ListSequence.Count - 1);
            int seqIndex = GetTotalSequenceIndex(grIndex + 1, 0);
            if (RectSequences.Count > seqIndex) RectSequences.RemoveAt(seqIndex - 1);
        }

    }
}
