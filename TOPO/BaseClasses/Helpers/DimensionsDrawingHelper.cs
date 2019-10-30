using _3DTools;
using BaseClasses.GraphObj;
using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using MATH;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses.Helpers
{
    public static class DimensionsDrawingHelper
    {
        public static void DrawDimensionsToTrackport(Trackport3D _trackport, DisplayOptions sDisplayOptions, CModel model, Model3DGroup gr)
        {
            // Ide o to ze nejakym sposobom ziskame zoznam uzlov alebo bodov ktore chceme kotovat a potom vyrabame koty a texty
            // Kota moze byt jedna alebo niekolko za sebou v linii - retazove kóty
            // Dalej mozu byt dalsie koty pridavane s nejakym odstupom voci prvej kolmo na hlavnu kotovaciu ciaru, tak ziskame niekolko kot pod sebou
            // Ja ich tu mam 1 - 3
            
            // DIMENSIONS FRONT
            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.FRONT)
            {
                DrawDimensionsFRONT(_trackport, model, sDisplayOptions, gr);
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.LEFT)
            {
                DrawDimensionsLEFT(_trackport, model, sDisplayOptions, gr);
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.RIGHT)
            {
                DrawDimensionsRIGHT(_trackport, model, sDisplayOptions, gr);
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.COLUMNS)
            {
                DrawDimensionsCOLUMNS(_trackport, model, sDisplayOptions, gr);
            }

            //MIDLE FRAME DIMENSIONS
            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.MIDDLE_FRAME)
            {
                DrawDimensionsMIDDLE(_trackport, model, sDisplayOptions, gr);
            }

            if (sDisplayOptions.ViewModelMembers == (int)EViewModelMemberFilters.FOUNDATIONS)
            {
                DrawDimensionsFOUNDATIONS(_trackport, model, sDisplayOptions, gr);
            }
        }



        private static void DrawDimensionsFRONT(Trackport3D _trackport, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {
            CMember m1 = model.m_arrMembers.FirstOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);
            CMember m2 = model.m_arrMembers.LastOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);
            CMember m3 = model.m_arrMembers.FirstOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeRafter);
            //CMember m4 = model.m_arrMembers.LastOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.MainColumn);

            CDimensionLinear3D dimPOKUSNA1 = new CDimensionLinear3D(m1.NodeStart.GetPoint3D(), m2.NodeEnd.GetPoint3D(), EGlobalPlane.XZ, -1, 0,
                 0.4, 0.4, 0.05, 0.15, (model.fW_frame * 1000).ToString("F0"), true);
            CDimensionLinear3D dimPOKUSNA2 = new CDimensionLinear3D(m1.NodeStart.GetPoint3D(), m1.NodeEnd.GetPoint3D(), EGlobalPlane.XZ, 0, -1,
                 0.4, 0.4, 0.05, 0.15, (model.fH1_frame * 1000).ToString("F0"), false);
            CDimensionLinear3D dimPOKUSNA3 = new CDimensionLinear3D(m3.NodeStart.GetPoint3D(), m3.NodeEnd.GetPoint3D(), EGlobalPlane.XZ, 1, -1,
                 0.4, 0.4, 0.05, 0.15, (m3.FLength * 1000).ToString("F0"), false);

            List<CDimensionLinear3D> listOfDimensions = new List<CDimensionLinear3D> { dimPOKUSNA1, dimPOKUSNA2, dimPOKUSNA3 };

            DrawDimensions(_trackport, listOfDimensions, model, displayOptions, gr);
        }
        private static void DrawDimensionsLEFT(Trackport3D _trackport, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {
            // Najdeme girts na lavej strane
            // Girts
            CMember[] membersLeftSideGirts = null;

            membersLeftSideGirts = ModelHelper.GetMembersInDistance(model, 0, 0, EMemberType_FS.eG); // smer X

            CMember[] membersLeftSideFirstBayGirts = ModelHelper.GetMembersInDistanceInterval(model, 0, model.fL1_frame, 1, EMemberType_FS.eG, true, true, false); // smer Y

            if (membersLeftSideFirstBayGirts != null)
            {
                // 1 kotovacia ciara - vsetky girts
                bool bDrawDimension_1 = true;

                // Pripravime si zoznamy kotovanych bodov
                List<CNode> membersLeftSideFirstBayGirtsNodes_1 = null;
                membersLeftSideFirstBayGirtsNodes_1 = new List<CNode>();

                // Kedze chceme kotovat od hrany musime pridat uzly na krajoch
                membersLeftSideFirstBayGirtsNodes_1.Add(new CNode(0, 0, model.fL1_frame, 0, 0));
                membersLeftSideFirstBayGirtsNodes_1.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));

                foreach (CMember m in membersLeftSideFirstBayGirts)
                {
                    if (MathF.d_equal(m.NodeStart.Y, model.fL1_frame))
                    {
                        membersLeftSideFirstBayGirtsNodes_1.Add(m.NodeStart);
                    }

                    if (MathF.d_equal(m.NodeEnd.Y, model.fL1_frame))
                    {
                        membersLeftSideFirstBayGirtsNodes_1.Add(m.NodeEnd);
                    }
                }

                if (bDrawDimension_1 == false)
                    membersLeftSideFirstBayGirtsNodes_1 = null;

                // Mame pripraveny zoznam bodov
                // Body zoradime podla Z od najvacsieho - koty kreslime , resp z +X smerom k 0

                // TO Ondrej - toto momentalne neplati lebo to by sme museli implementovat rotaciu kot
                // kreslim a radim od 0 smerom k +Z

                if (membersLeftSideFirstBayGirtsNodes_1 != null)
                    membersLeftSideFirstBayGirtsNodes_1 = membersLeftSideFirstBayGirtsNodes_1.OrderBy(n => n.Z).ToList();

                // Create Dimensions
                List<CDimensionLinear3D> listOfDimensions = null;

                float fExtensionLineLength = 0.5f;
                float fExtensionLineOffset = 0.15f;
                float fOffsetBehindMainLine = 0.05f;
                float fDistanceBetweenMainLines = 0.2f;

                if (bDrawDimension_1 == true)
                {
                    listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersLeftSideFirstBayGirtsNodes_1.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersLeftSideFirstBayGirtsNodes_1[i].GetPoint3D(), membersLeftSideFirstBayGirtsNodes_1[i + 1].GetPoint3D(),
                             EGlobalPlane.YZ, 0, -1,
                             fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersLeftSideFirstBayGirtsNodes_1[i + 1].Z - membersLeftSideFirstBayGirtsNodes_1[i].Z) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty                        
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                DrawDimensions(_trackport, listOfDimensions, model, displayOptions, gr);
            }
        }
        private static void DrawDimensionsRIGHT(Trackport3D _trackport, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {
            CMember m1 = model.m_arrMembers.FirstOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);
            CMember m2 = model.m_arrMembers.LastOrDefault(m => m.EMemberTypePosition == EMemberType_FS_Position.EdgeColumn);

            // stlpy na pravej strane maju PointEnd v Z = 0
            CDimensionLinear3D dimPOKUSNA1 = new CDimensionLinear3D(m1.NodeEnd.GetPoint3D(), m2.NodeEnd.GetPoint3D(), EGlobalPlane.YZ, -1, 0,
                0.4, 0.4, 0.05, 0.15, (model.fL_tot * 1000).ToString("F0"), true);

            // stlp vlavo - vyskova kota
            CDimensionLinear3D dimPOKUSNA2 = new CDimensionLinear3D(m1.NodeStart.GetPoint3D(), m1.NodeEnd.GetPoint3D(), EGlobalPlane.YZ, 0, -1,
                0.4, 0.4, 0.05, 0.15, (model.fH1_frame * 1000).ToString("F0"), false);

            List<CDimensionLinear3D> listOfDimensions = new List<CDimensionLinear3D> { dimPOKUSNA1, dimPOKUSNA2 };

            DrawDimensions(_trackport, listOfDimensions, model, displayOptions, gr);
        }
        private static void DrawDimensionsMIDDLE(Trackport3D _trackport, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {
            // Potrebujeme identifikovat girts, purlins, eave purlins ???
            // Na ram sa pripajaju z oboch stran, vyberieme len tie, ktore na rame koncia a vidime ich v pohlade na ram zpredu, aby sme nemali body duplicitne

            // Girts
            CMember[] membersFrontSideGirts = null;

            membersFrontSideGirts = ModelHelper.GetMembersInDistanceInterval(model, 0, model.fL1_frame, 1, EMemberType_FS.eG, true, true, false); // smer Y

            // Purlins
            CMember[] membersFrontSidePurlins = null;

            membersFrontSidePurlins = ModelHelper.GetMembersInDistanceInterval(model, 0, model.fL1_frame, 1, EMemberType_FS.eP, true, true, false); // smer Y

            List<CNode> membersBaseNodes_FrontSideGirts_1 = null; // Girts
                                                                  // Tuto celkovu kotu kreslime vzdy
            List<CNode> membersBaseNodes_FrontSideVertical_2 = new List<CNode>(); // Overall knee height H1

            List<CNode> membersBaseNodes_FrontSidePurlins_1 = null; // Purlins
                                                                    // Tuto celkovu kotu kreslime vzdy
            List<CNode> membersBaseNodes_FrontSideRafter_2 = new List<CNode>(); // Overall rafter length

            // Kedze chceme kotovat od spodnej hrany a vo vyske H1 musime pridat uzly na koncoch stlpa
            membersBaseNodes_FrontSideVertical_2.Add(new CNode(0, 0, model.fL1_frame, 0, 0));
            membersBaseNodes_FrontSideVertical_2.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));

            // Kedze chceme kotovat od zaciatku po koniec raftera musime pridat uzly na koncoch
            CMember leftRafter = model.m_arrMembers.FirstOrDefault(m => m.EMemberType == EMemberType_FS.eMR);

            membersBaseNodes_FrontSideRafter_2.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));
            membersBaseNodes_FrontSideRafter_2.Add(new CNode(0, leftRafter.NodeEnd.X, model.fL1_frame, leftRafter.NodeEnd.Z, 0));

            if (membersFrontSideGirts != null)
            {
                membersBaseNodes_FrontSideGirts_1 = new List<CNode>(); // Girts

                membersBaseNodes_FrontSideGirts_1.Add(new CNode(0, 0, model.fL1_frame, 0, 0));
                membersBaseNodes_FrontSideGirts_1.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));

                foreach (CMember m in membersFrontSideGirts)
                {
                    if (MathF.d_equal(m.NodeEnd.X, 0) && MathF.d_equal(m.NodeEnd.Y, model.fL1_frame)) // Koncovy bod pruta typu girt je na rame // Girt je na lavom stlpe
                    {
                        membersBaseNodes_FrontSideGirts_1.Add(m.NodeEnd);
                    }
                }
            }

            if (membersFrontSidePurlins != null)
            {
                membersBaseNodes_FrontSidePurlins_1 = new List<CNode>(); // Purlins

                membersBaseNodes_FrontSidePurlins_1.Add(new CNode(0, 0, model.fL1_frame, model.fH1_frame, 0));
                membersBaseNodes_FrontSidePurlins_1.Add(new CNode(0, leftRafter.NodeEnd.X, model.fL1_frame, leftRafter.NodeEnd.Z, 0));

                foreach (CMember m in membersFrontSidePurlins)
                {
                    if ((m.NodeEnd.X < leftRafter.NodeEnd.X) && MathF.d_equal(m.NodeEnd.Y, model.fL1_frame)) // Koncovy bod pruta typu purlin je na rame // Purlin je na lavom raftery
                    {
                        membersBaseNodes_FrontSidePurlins_1.Add(m.NodeEnd);
                    }
                }
            }

            bool bDrawDimensionsOnWallMembers = true;
            bool bDrawDimesnionsOnRoofMembers = true;

            if (bDrawDimensionsOnWallMembers)
            {
                // 1 kotovacia ciara - vsetky girts
                // 2 kotovacia ciara - celkovy rozmer - vyska stlpa

                bool bDrawDimension_1 = false;
                bool bDrawDimension_2 = true;

                if (membersBaseNodes_FrontSideGirts_1 != null)
                    membersBaseNodes_FrontSideGirts_1 = membersBaseNodes_FrontSideGirts_1.OrderBy(n => n.Z).ToList();

                if (membersBaseNodes_FrontSideVertical_2 != null)
                    membersBaseNodes_FrontSideVertical_2 = membersBaseNodes_FrontSideVertical_2.OrderBy(n => n.Z).ToList();

                if (membersBaseNodes_FrontSideGirts_1 != null)
                    bDrawDimension_1 = true;

                // Create Dimensions
                List<CDimensionLinear3D> listOfDimensions = null;

                float fExtensionLineLength = 0.5f;
                //float fMainLinePosition = 0.4f;
                float fExtensionLineOffset = 0.15f;
                float fOffsetBehindMainLine = 0.05f;

                float fDistanceBetweenMainLines = 0.2f;

                // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou
                if (bDrawDimension_1)
                {
                    listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_FrontSideGirts_1.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSideGirts_1[i].GetPoint3D(), membersBaseNodes_FrontSideGirts_1[i + 1].GetPoint3D(),
                            EGlobalPlane.XZ, 0, -1,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersBaseNodes_FrontSideGirts_1[i + 1].Z - membersBaseNodes_FrontSideGirts_1[i].Z) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = + fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                if (bDrawDimension_2 == true)
                {
                    if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_FrontSideVertical_2.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSideVertical_2[i].GetPoint3D(), membersBaseNodes_FrontSideVertical_2[i + 1].GetPoint3D(),
                            EGlobalPlane.XZ, 0, -1,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersBaseNodes_FrontSideVertical_2[i + 1].Z - membersBaseNodes_FrontSideVertical_2[i].Z) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = +fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                Model3DGroup dimensions3DGroup = null;
                if (displayOptions.bDisplayDimensions) dimensions3DGroup = CreateModelDimensions_Model3DGroup(listOfDimensions, model, displayOptions);
                if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

                // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
                if (dimensions3DGroup != null)
                {
                    foreach (CDimensionLinear3D dim in listOfDimensions)
                    {
                        DrawDimensionText3D(dim, _trackport.ViewPort, displayOptions);
                    }
                }
            }

            if (bDrawDimesnionsOnRoofMembers)
            {
                // 1 kotovacia ciara - vsetky purlins
                // 2 kotovacia ciara - celkovy rozmer - dlzka raftera

                bool bDrawDimension_1 = false;
                bool bDrawDimension_2 = true;

                if (membersBaseNodes_FrontSidePurlins_1 != null)
                    membersBaseNodes_FrontSidePurlins_1 = membersBaseNodes_FrontSidePurlins_1.OrderBy(n => n.Z).ToList();

                if (membersBaseNodes_FrontSideRafter_2 != null)
                    membersBaseNodes_FrontSideRafter_2 = membersBaseNodes_FrontSideRafter_2.OrderBy(n => n.Z).ToList();

                if (membersBaseNodes_FrontSidePurlins_1 != null)
                    bDrawDimension_1 = true;

                // Create Dimensions
                List<CDimensionLinear3D> listOfDimensions = null;

                float fExtensionLineLength = 0.5f;
                //float fMainLinePosition = 0.4f;
                float fExtensionLineOffset = 0.15f;
                float fOffsetBehindMainLine = 0.05f;

                float fDistanceBetweenMainLines = 0.2f;

                // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou
                if (bDrawDimension_1 == true)
                {
                    listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_FrontSidePurlins_1.Count - 1; i++)
                    {
                        float fLengthForText = MathF.Sqrt(MathF.Pow2(membersBaseNodes_FrontSidePurlins_1[i + 1].X - membersBaseNodes_FrontSidePurlins_1[i].X) + MathF.Pow2(membersBaseNodes_FrontSidePurlins_1[i + 1].Z - membersBaseNodes_FrontSidePurlins_1[i].Z));
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSidePurlins_1[i].GetPoint3D(), membersBaseNodes_FrontSidePurlins_1[i + 1].GetPoint3D(),
                            EGlobalPlane.XZ, 1, -1,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, (fLengthForText * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = + fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                if (bDrawDimension_2 == true)
                {
                    if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_FrontSideRafter_2.Count - 1; i++)
                    {
                        float fLengthForText = MathF.Sqrt(MathF.Pow2(membersBaseNodes_FrontSideRafter_2[i + 1].X - membersBaseNodes_FrontSideRafter_2[i].X) + MathF.Pow2(membersBaseNodes_FrontSideRafter_2[i + 1].Z - membersBaseNodes_FrontSideRafter_2[i].Z));
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSideRafter_2[i].GetPoint3D(), membersBaseNodes_FrontSideRafter_2[i + 1].GetPoint3D(),
                            EGlobalPlane.XZ, 1, -1,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, (fLengthForText * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = +fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                DrawDimensions(_trackport, listOfDimensions, model, displayOptions, gr);
            }
        }
        private static void DrawDimensionsCOLUMNS(Trackport3D _trackport, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {
            // Potrebujeme idenfikovat ktore pruty (stlpy) su, na ktorej strane a vyrobit medzi nimi koty

            // Front side
            CMember[] membersFrontSide = null;

            membersFrontSide = ModelHelper.GetMembersInDistance(model, 0, 1); // smer Y

            // Back side
            CMember[] membersBackSide = null;

            membersBackSide = ModelHelper.GetMembersInDistance(model, model.fL_tot, 1); // smer Y

            // Left side
            CMember[] membersLeftSide = null;

            membersLeftSide = ModelHelper.GetMembersInDistance(model, 0, 0); // smer X

            // Right side
            CMember[] membersRightSide = null;

            membersRightSide = ModelHelper.GetMembersInDistance(model, model.fW_frame, 0); // smer X

            // TO Ondrej - ak by sa toto zobecnilo mohlo by sa to pouzit aj v inych pohladoch alebo podorysoch

            if (membersFrontSide != null)
            {
                // 1 kotovacia ciara - vsetky stlpy
                // 2 kotovacia ciara - vsetky MC a EC (len left) alebo WP/C a EC (front a back)
                // 3 kotovacia ciara - celkovy rozmer (len left a front)

                bool bDrawDimension_1 = false;
                bool bDrawDimension_2 = false;
                bool bDrawDimension_3 = true;
                // Pripravime si zoznamy kotovanych bodov

                // float fh = (float)model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h;
                float fh = (float)model.m_arrMembers[0].CrScStart.h; // Docasne, prvy prut by mal byt Main Column
                                                                     // Front side
                List<CNode> membersBaseNodes_FrontSide_1 = null;
                List<CNode> membersBaseNodes_FrontSide_2 = null; // Wind posts and edge columns
                List<CNode> membersBaseNodes_FrontSide_3 = null; // Edges

                // Tuto celkovu kotu kreslime vzdy
                membersBaseNodes_FrontSide_3 = new List<CNode>();
                membersBaseNodes_FrontSide_3.Add(new CNode(0, -0.5f * fh, 0, 0, 0)); // Suradnice sa menia pre smer X
                membersBaseNodes_FrontSide_3.Add(new CNode(0, model.fW_frame + 0.5f * fh, 0, 0, 0));

                membersBaseNodes_FrontSide_1 = new List<CNode>();
                membersBaseNodes_FrontSide_2 = new List<CNode>();

                // Kedze chceme kotovat od hrany musime pridat uzly na krajoch
                membersBaseNodes_FrontSide_1.Add(new CNode(0, -0.5f * fh, 0, 0, 0));
                membersBaseNodes_FrontSide_1.Add(new CNode(0, model.fW_frame + 0.5f * fh, 0, 0, 0));

                membersBaseNodes_FrontSide_2.Add(new CNode(0, -0.5f * fh, 0, 0, 0));
                membersBaseNodes_FrontSide_2.Add(new CNode(0, model.fW_frame + 0.5f * fh, 0, 0, 0));

                foreach (CMember m in membersFrontSide)
                {
                    if (MathF.d_equal(m.NodeStart.Z, 0))
                    {
                        membersBaseNodes_FrontSide_1.Add(m.NodeStart);

                        if (m.EMemberType == EMemberType_FS.eC || m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eWP)
                            membersBaseNodes_FrontSide_2.Add(m.NodeStart);
                    }

                    if (MathF.d_equal(m.NodeEnd.Z, 0))
                    {
                        membersBaseNodes_FrontSide_1.Add(m.NodeEnd);

                        if (m.EMemberType == EMemberType_FS.eC || m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eWP)
                            membersBaseNodes_FrontSide_2.Add(m.NodeEnd);
                    }
                }

                // Ak sa v zozname na urovni 1 nenachadzaju stlpy ktore patria dveram, kotu nekreslime a zoznam zmazeme
                // To Ondrej - toto by sa asi dalo zistit uz vopred este predtym nez vyrabam zoznamy uzlov
                foreach (CMember m in membersFrontSide)
                {
                    if (!bDrawDimension_1 && (m.EMemberType == EMemberType_FS.eDF || m.EMemberType == EMemberType_FS.eDT))
                    {
                        bDrawDimension_1 = true;
                    }

                    if (!bDrawDimension_2 && (m.EMemberType == EMemberType_FS.eC || m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eWP))
                    {
                        bDrawDimension_2 = true;
                    }
                }

                if (bDrawDimension_1 == false)
                    membersBaseNodes_FrontSide_1 = null;

                if (bDrawDimension_2 == false)
                    membersBaseNodes_FrontSide_2 = null;

                // Mame pripravene 3 zoznamy bodov
                // Body zoradime podla X od najvacsieho - koty kreslime zhora nadol, resp z +X smerom k 0

                // TO Ondrej - toto momentalne neplati lebo to by sme museli implementovat rotaciu kot
                // kreslim a radim od 0 smerom k +X

                if (membersBaseNodes_FrontSide_1 != null)
                    membersBaseNodes_FrontSide_1 = membersBaseNodes_FrontSide_1.OrderBy(n => n.X).ToList();

                if (membersBaseNodes_FrontSide_2 != null)
                    membersBaseNodes_FrontSide_2 = membersBaseNodes_FrontSide_2.OrderBy(n => n.X).ToList();

                if (membersBaseNodes_FrontSide_3 != null)
                    membersBaseNodes_FrontSide_3 = membersBaseNodes_FrontSide_3.OrderBy(n => n.X).ToList();

                // Create Dimensions
                List<CDimensionLinear3D> listOfDimensions = null;

                float fExtensionLineLength = 0.2f;
                //float fMainLinePosition = 0.4f;
                float fExtensionLineOffset = 0.1f;
                float fOffsetBehindMainLine = 0.05f;

                float fDistanceBetweenMainLines = 0.2f;

                // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou

                if (bDrawDimension_1 == true)
                {
                    listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_FrontSide_1.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSide_1[i].GetPoint3D(), membersBaseNodes_FrontSide_1[i + 1].GetPoint3D(),
                           EGlobalPlane.XY, 0, -1,
                           fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersBaseNodes_FrontSide_1[i + 1].X - membersBaseNodes_FrontSide_1[i].X) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = + fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                if (bDrawDimension_2 == true)
                {
                    if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_FrontSide_2.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSide_2[i].GetPoint3D(), membersBaseNodes_FrontSide_2[i + 1].GetPoint3D(),
                            EGlobalPlane.XY, 0, -1,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersBaseNodes_FrontSide_2[i + 1].X - membersBaseNodes_FrontSide_2[i].X) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = +fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                if (bDrawDimension_3 == true)
                {
                    if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_FrontSide_3.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_FrontSide_3[i].GetPoint3D(), membersBaseNodes_FrontSide_3[i + 1].GetPoint3D(),
                            EGlobalPlane.XY, 0, -1,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersBaseNodes_FrontSide_3[i + 1].X - membersBaseNodes_FrontSide_3[i].X) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }
                }

                DrawDimensions(_trackport, listOfDimensions, model, displayOptions, gr);
            }

            // TODO Ondrej - niektore casti kodu pre jednotlive steny front a left by sli zrefaktorovat a zjednotit do funkcii
            // !!! menia sa pouzivane suradnice, pre front X a pre left Y, napriklad pre urcenie textu koty alebo zoradenie bodov koty
            // TODO - pridat kreslenie kot pre right a back, kota cislo 2 a 3 sa pre tieto strany nema zobrazit, staci zobrazit kotu cislo 1 ak existuju nejake dvere v pravej alebo zadnej stene
            // TO Ondrej - mozno by bolo dobre zaviest objekt Continuous Dimension, to by bola vlastne skupina kot, ktora ma Main Line v jednej priamke a postupne na seba nadvazuju, Extension lines mozu byt rozne dlhe ale ostatne parametre by boli podobne, dalo by sa s nou pracovat ako s celkom

            // Potrebovali by sme vymysliet co spravime ak je kota taka kratka ze sa sipky nemaju kam vykreslit a ani na text tam nie je miesto, zvycajne sa to robi cez odkazovu ciaru
            // Mozeme to urobit aj tak ze taka mini kota tam proste teraz nebude zobrazena a hotovo :)

            // TO Ondrej - ak by sa toto zobecnilo mohlo by sa to pouzit aj v inych pohladoch alebo podorysoch

            if (membersLeftSide != null)
            {
                // 1 kotovacia ciara - vsetky stlpy
                // 2 kotovacia ciara - vsetky MC a EC (len left) alebo WP/C a EC (front a back)
                // 3 kotovacia ciara - cerlkovy rozmer (len left a front)

                bool bDrawDimension_1 = false;
                bool bDrawDimension_2 = false;
                bool bDrawDimension_3 = true;
                // Pripravime si zoznamy kotovanych bodov

                // float fb = (float)model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn].b;
                float fb = (float)model.m_arrMembers[0].CrScStart.b; // Docasne, prvy prut by mal byt Main Column

                // Left side
                List<CNode> membersBaseNodes_LeftSide_1 = null;
                List<CNode> membersBaseNodes_LeftSide_2 = null; // Main columns and edge columns
                List<CNode> membersBaseNodes_LeftSide_3 = null; // Edges

                // Toto celkovu kotu kreslime vzdy
                membersBaseNodes_LeftSide_3 = new List<CNode>();
                membersBaseNodes_LeftSide_3.Add(new CNode(0, 0, -0.5f * fb, 0, 0)); // Suradnice sa menia pre smer Y
                membersBaseNodes_LeftSide_3.Add(new CNode(0, 0, model.fL_tot + 0.5f * fb, 0, 0));

                membersBaseNodes_LeftSide_1 = new List<CNode>();
                membersBaseNodes_LeftSide_2 = new List<CNode>();

                // Kedze chceme kotovat od hrany musime pridat uzly na krajoch
                membersBaseNodes_LeftSide_1.Add(new CNode(0, 0, -0.5f * fb, 0, 0));
                membersBaseNodes_LeftSide_1.Add(new CNode(0, 0, model.fL_tot + 0.5f * fb, 0, 0));

                membersBaseNodes_LeftSide_2.Add(new CNode(0, 0, -0.5f * fb, 0, 0));
                membersBaseNodes_LeftSide_2.Add(new CNode(0, 0, model.fL_tot + 0.5f * fb, 0, 0));

                foreach (CMember m in membersLeftSide)
                {
                    if (MathF.d_equal(m.NodeStart.Z, 0))
                    {
                        membersBaseNodes_LeftSide_1.Add(m.NodeStart);

                        if (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC)
                            membersBaseNodes_LeftSide_2.Add(m.NodeStart);
                    }

                    if (MathF.d_equal(m.NodeEnd.Z, 0))
                    {
                        membersBaseNodes_LeftSide_1.Add(m.NodeEnd);

                        if (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC)
                            membersBaseNodes_LeftSide_2.Add(m.NodeEnd);
                    }
                }

                // Ak sa v zozname na urovni 1 nenachadzaju stlpy ktore patria dveram, kotu nekreslime a zoznam zmazeme
                // To Ondrej - toto by sa asi dalo zistit uz vopred este predtym nez vyrabam zoznamy uzlov
                foreach (CMember m in membersLeftSide)
                {
                    if (!bDrawDimension_1 && (m.EMemberType == EMemberType_FS.eDF || m.EMemberType == EMemberType_FS.eDT))
                    {
                        bDrawDimension_1 = true;
                    }

                    if (!bDrawDimension_2 && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eEC))
                    {
                        bDrawDimension_2 = true;
                    }
                }

                if (bDrawDimension_1 == false)
                    membersBaseNodes_LeftSide_1 = null;

                if (bDrawDimension_2 == false)
                    membersBaseNodes_LeftSide_2 = null;

                // Mame pripravene 3 zoznamy bodov
                // Body zoradime podla Y od najmensieho - koty kreslime zlava smerom doprava, resp od 0 do +Y
                if (membersBaseNodes_LeftSide_1 != null)
                    membersBaseNodes_LeftSide_1 = membersBaseNodes_LeftSide_1.OrderBy(n => n.Y).ToList();

                if (membersBaseNodes_LeftSide_2 != null)
                    membersBaseNodes_LeftSide_2 = membersBaseNodes_LeftSide_2.OrderBy(n => n.Y).ToList();

                if (membersBaseNodes_LeftSide_3 != null)
                    membersBaseNodes_LeftSide_3 = membersBaseNodes_LeftSide_3.OrderBy(n => n.Y).ToList();

                // Create Dimensions
                List<CDimensionLinear3D> listOfDimensions = null;

                float fExtensionLineLength = 0.2f;
                //float fMainLinePosition = 0.4f;
                float fExtensionLineOffset = 0.1f;
                float fOffsetBehindMainLine = 0.05f;

                float fDistanceBetweenMainLines = 0.2f;

                // TODO - Ondrej  pre text koty by sme mali pouzit nejaky algorimus podobny Member Description, mali by mat nastavitelne odsadenie od main line a zobrazovat rotovat sa spolu s kotou

                if (bDrawDimension_1 == true)
                {
                    listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_LeftSide_1.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_LeftSide_1[i].GetPoint3D(), membersBaseNodes_LeftSide_1[i + 1].GetPoint3D(),
                            EGlobalPlane.XY, 1, 0,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersBaseNodes_LeftSide_1[i + 1].Y - membersBaseNodes_LeftSide_1[i].Y) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = + fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                if (bDrawDimension_2 == true)
                {
                    if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_LeftSide_2.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_LeftSide_2[i].GetPoint3D(), membersBaseNodes_LeftSide_2[i + 1].GetPoint3D(),
                            EGlobalPlane.XY, 1, 0,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersBaseNodes_LeftSide_2[i + 1].Y - membersBaseNodes_LeftSide_2[i].Y) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = +fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                if (bDrawDimension_3 == true)
                {
                    if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < membersBaseNodes_LeftSide_3.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(membersBaseNodes_LeftSide_3[i].GetPoint3D(), membersBaseNodes_LeftSide_3[i + 1].GetPoint3D(),
                            EGlobalPlane.XY, 1, 0,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((membersBaseNodes_LeftSide_3[i + 1].Y - membersBaseNodes_LeftSide_3[i].Y) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }
                }

                DrawDimensions(_trackport, listOfDimensions, model, displayOptions, gr);
            }
        }
        private static void DrawDimensionsFOUNDATIONS(Trackport3D _trackport, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {
            // Pre zakladne koty okotujeme podorys podobne ako pre Columns View

            // Potrebujeme okotovat jednotlive typy patiek
            // To znamena ziskat body na krajnej hrane patiek uplne vpravo a body na krajnej hrane patiek uplne vzadu pre jednotlive typy patiek

            // All foundation points
            List<Point3D> foundationPoints = ModelHelper.GetFootingPadsPoints(model);

            bool bDrawDimension_Right = true;
            bool bDrawDimension_Back = true;

            if (bDrawDimension_Right == true) // Kota vpravo od budovy
            {
                // Maximum right coordinate points Z = 0
                List<Point3D> foundationPointsX = ModelHelper.GetPointsInDistanceInterval(foundationPoints, model.fW_frame, model.fW_frame + 10, 0, true, false); // X
                List<Point3D> dimensionPoints_1 = ModelHelper.GetPointsInDistance(foundationPointsX, 0, 2); // Z

                dimensionPoints_1 = dimensionPoints_1.OrderBy(p => p.Y).ToList(); // Sort by Y coordinate
                List<Point3D> dimensionPoints_2 = new List<Point3D>() { dimensionPoints_1.First(), dimensionPoints_1.Last() };

                bool bDrawDimension_1 = true;
                bool bDrawDimension_2 = true;

                // Create Dimensions
                List<CDimensionLinear3D> listOfDimensions = null;

                float fExtensionLineLength = 0.2f;
                //float fMainLinePosition = 0.4f;
                float fExtensionLineOffset = 0.1f;
                float fOffsetBehindMainLine = 0.05f;

                float fDistanceBetweenMainLines = 0.2f;

                if (bDrawDimension_1 == true)
                {
                    listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < dimensionPoints_1.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(dimensionPoints_1[i], dimensionPoints_1[i + 1],
                            EGlobalPlane.XY, -1, 0,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((dimensionPoints_1[i + 1].Y - dimensionPoints_1[i].Y) * 1000).ToString("F0"), true);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = + fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                if (bDrawDimension_2 == true)
                {
                    if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < dimensionPoints_2.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(dimensionPoints_2[i], dimensionPoints_2[i + 1],
                            EGlobalPlane.XY, -1, 0,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((dimensionPoints_2[i + 1].Y - dimensionPoints_2[i].Y) * 1000).ToString("F0"), true);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = +fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                DrawDimensions(_trackport, listOfDimensions, model, displayOptions, gr);
            }

            if (bDrawDimension_Back == true) // Kota v zadnej casti budovy
            {
                // Maximum back coordinate points Z = 0
                // Maximalna suradnica zakladu pre wind post Y moze byt teoreticky byt mensia dlzka budovy (je to nespravne, ale je to mozne tak zadat :)
                List<Point3D> foundationPointsY = ModelHelper.GetPointsInDistanceInterval(foundationPoints, model.fL_tot - 0.2f, model.fL_tot + 10, 1, true, false); // Y
                List<Point3D> dimensionPoints_1 = ModelHelper.GetPointsInDistance(foundationPointsY, 0, 2); // Z

                dimensionPoints_1 = dimensionPoints_1.OrderBy(p => p.X).ToList(); // Sort by X coordinate

                List<Point3D> dimensionPoints_2 = new List<Point3D>() { dimensionPoints_1.First(), dimensionPoints_1.Last() };

                bool bDrawDimension_1 = true;
                bool bDrawDimension_2 = true;

                // Create Dimensions
                List<CDimensionLinear3D> listOfDimensions = null;

                float fExtensionLineLength = 0.2f;
                //float fMainLinePosition = 0.4f;
                float fExtensionLineOffset = 0.1f;
                float fOffsetBehindMainLine = 0.05f;

                float fDistanceBetweenMainLines = 0.25f;

                if (bDrawDimension_1 == true)
                {
                    listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < dimensionPoints_1.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(dimensionPoints_1[i], dimensionPoints_1[i + 1],
                            EGlobalPlane.XY, 0, 1,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((dimensionPoints_1[i + 1].X - dimensionPoints_1[i].X) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = + fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                if (bDrawDimension_2 == true)
                {
                    if (listOfDimensions == null) listOfDimensions = new List<CDimensionLinear3D>();
                    for (int i = 0; i < dimensionPoints_2.Count - 1; i++)
                    {
                        CDimensionLinear3D dim = new CDimensionLinear3D(dimensionPoints_2[i], dimensionPoints_2[i + 1],
                            EGlobalPlane.XY, 0, 1,
                            fExtensionLineLength, fExtensionLineLength, fOffsetBehindMainLine, fExtensionLineOffset, ((dimensionPoints_2[i + 1].X - dimensionPoints_2[i].X) * 1000).ToString("F0"), false);
                        listOfDimensions.Add(dim);
                    }

                    // Nastavime parametre pre dalsie koty
                    //fExtensionLineLength += fDistanceBetweenMainLines;
                    //fMainLinePosition = + fDistanceBetweenMainLines;
                    fExtensionLineOffset += fDistanceBetweenMainLines;
                }

                DrawDimensions(_trackport, listOfDimensions, model, displayOptions, gr);
            }
        }
        private static void DrawDimensionsROOF(Trackport3D _trackport, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {

        }
        private static void DrawDimensionsBACK(Trackport3D _trackport, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {

        }

        // Create Dimensions 3D Model
        private static void DrawDimensions(Trackport3D _trackport, List<CDimensionLinear3D> dimensions, CModel model, DisplayOptions displayOptions, Model3DGroup gr)
        {
            Model3DGroup dimensions3DGroup = null;

            if (displayOptions.bDisplayDimensions) dimensions3DGroup = CreateModelDimensions_Model3DGroup(dimensions, model, displayOptions);
            if (dimensions3DGroup != null) gr.Children.Add(dimensions3DGroup);

            // Create Dimensions Texts - !!! Pred tym nez generujem text musi byt vygenerovany 3D model koty
            if (dimensions3DGroup != null)
            {
                foreach (CDimensionLinear3D dim in dimensions)
                {
                    DrawDimensionText3D(dim, _trackport.ViewPort, displayOptions);
                }
            }
        }

        private static Model3DGroup CreateModelDimensions_Model3DGroup(List<CDimensionLinear3D> dimensions, CModel model, DisplayOptions displayOptions)
        {
            if (dimensions == null || dimensions.Count == 0)
                return null;

            float maxModelLength = MathF.Max(Drawing3D.fModel_Length_X, Drawing3D.fModel_Length_Y, Drawing3D.fModel_Length_Z);
            float fLineRadius = maxModelLength / 1500;  //velkost podla modelu, ale to cislo "1500" je potrebne data do DisplayOptions

            float scale = maxModelLength / 10;

            // ZATIAL POKUS VYKRESLIT KOTU INDIVIDUALNE, NIE VSETKY KOTY NARAZ Z CELEHO MODELU
            // Draw 3D objects (cylinder as a line)

            Model3DGroup gr = new Model3DGroup();

            foreach (CDimensionLinear3D dimension in dimensions)
            {
                dimension.ExtensionLine1Length *= scale;
                dimension.ExtensionLine2Length *= scale;
                dimension.ExtensionLines_OffsetBehindMainLine *= scale;
                dimension.DimensionMainLineDistance *= scale;
                dimension.DimensionMainLinePositionIncludingOffset *= scale;
                dimension.OffSetFromPoint *= scale;

                //dimension.MainLineLength *= scale; // Tu si nie som isty ci to treba scalovat
                dimension.SetPoints_LCS();
                //dimension.SetTextPointInLCS(); // Mozno by som zavolal toto a prepocital poziciu textu funkciou // ????

                dimension.PointText = new Point3D(dimension.PointText.X, dimension.PointText.Y * scale, dimension.PointText.Z);
                gr.Children.Add(dimension.GetDimensionModelNew(displayOptions.DimensionLineColor, fLineRadius));
            }

            return gr;
        }

        // Draw Dimension Text 3D
        public static void DrawDimensionText3D(CDimensionLinear3D dimension, Viewport3D viewPort, DisplayOptions displayOptions)
        {
            TextBlock tb = new TextBlock();
            tb.Text = dimension.Text;
            tb.FontFamily = new FontFamily("Arial");

            float fTextBlockVerticalSize = MathF.Max(Drawing3D.fModel_Length_X, Drawing3D.fModel_Length_Y, Drawing3D.fModel_Length_Z) / 100f;

            tb.FontStretch = FontStretches.UltraCondensed;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Thin;
            tb.Foreground = new SolidColorBrush(displayOptions.DimensionTextColor);
            //tb.Background = new SolidColorBrush(displayOptions.backgroundColor);  //netreba nastavovat ak chceme mat transparentne

            Vector3D over = new Vector3D(dimension.VectorOverFactor_LCS, 0, 0);
            Vector3D up = new Vector3D(0, dimension.VectorUpFactor_LCS, 0);

            // Create text
            ModelVisual3D textlabel = Drawing3D.CreateTextLabel3D(tb, true, fTextBlockVerticalSize, dimension.PointText, over, up, 0.8);

            if (Drawing3D.centerModel)
            {
                Transform3DGroup tr = new Transform3DGroup();

                if (dimension.TransformGr == null)
                {
                    throw new Exception("Dimension in local coordinate system! \nTransformation object is null! \nText label is probably created before dimension model exists!");
                }

                if (dimension.TransformGr != null)
                {
                    tr.Children.Add(dimension.TransformGr); // TO Ondrej - tu si mal zakomentovanu podmienku a ak bola dimension.TransformGr null tak to tu padlo, neviem ci moze byt null, jedine ze sa s kotou nic nerobi ale to sa mi nezda

                    // Pokus transformovat samostatne bod a vektory a az potom vytvorit label, netransformuje sa teda label ako celok
                    //Vector3D overTransformed = dimension.TransformGr.Transform(over);
                    //Vector3D upTransformed = dimension.TransformGr.Transform(up);
                    //Point3D pTransformed = dimension.TransformGr.Transform(dimension.PointText);
                    //textlabel = CreateTextLabel3D(tb, true, fTextBlockVerticalSize, pTransformed, overTransformed, upTransformed);
                }
                tr.Children.Add(Drawing3D.centerModelTransGr); // To Ondrej - Mam otazku ci treba tuto transformaciu pre text robit samostatna alebo je uz obsiahnuta v transformacii koty
                textlabel.Transform = tr; //centerModelTransGr;
            }
            viewPort.Children.Add(textlabel);
        }
    }
}
