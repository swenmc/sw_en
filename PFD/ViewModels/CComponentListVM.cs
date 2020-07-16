using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Globalization;
using BaseClasses;
using System.Collections.ObjectModel;
using DATABASE;
using DATABASE.DTO;
using System.Windows.Media;
using BaseClasses.Helpers;

namespace PFD
{
    [Serializable]
    public class CComponentListVM : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<CComponentInfo> MComponentList;
        private int MSelectedComponentIndex;

        private List<string> MSections;
        private List<CComboColor> MColors;
        private List<string> MSectionsForColumnsOrRafters;
        private List<string> MSectionsForGirtsOrPurlins;
        private List<string> MSectionsForGirtsOrPurlinsBracing;
        private List<string> MSectionsForDoorOrWindowFrame;
        private List<string> MSectionsForRollerDoorTrimmer;
        private List<string> MSectionsForRollerDoorLintel;
        private List<string> MSectionsForCrossBracing;

        private List<string> MColumnFlyBracingPosition_Items;
        private List<string> MRafterFlyBracingPosition_Items;
        private List<string> MDefaultILS_Items;
        private List<string> MEmptyILS_Items;

        private List<string> MMaterials;
        private List<CSectionPropertiesText> m_ComponentDetailsList;
        private List<CMaterialPropertiesText> m_MaterialDetailsList;

        private bool m_allMaterialListChanged;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public ObservableCollection<CComponentInfo> ComponentList
        {
            get
            {
                return MComponentList;
            }

            set
            {
                MComponentList = value;
                foreach (CComponentInfo ci in ComponentList)
                {
                    ci.PropertyChanged += ComponentListItem_PropertyChanged;
                }
                NotifyPropertyChanged("ComponentList");
            }
        }

        private void ComponentListItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is CComponentInfo)
            {
                CComponentInfo cInfo = sender as CComponentInfo;
                if (cInfo.IsSetFromCode) return;
                if (e.PropertyName == "GenerateIsEnabled") return;
                if (e.PropertyName == "ILS_Items") return; 

                if (e.PropertyName == "Material") SetComponentDetails();
                else if (e.PropertyName == "Section") SetComponentDetails();

                if (e.PropertyName == "Generate")
                {
                    if (!ValidateGirts()) { cInfo.IsSetFromCode = true; cInfo.Generate = !cInfo.Generate; ValidateGirts(); cInfo.IsSetFromCode = false; return; }
                    SetGirtsAndColumns(cInfo);
                }

                if (e.PropertyName == "ILS")
                {
                    SetSameILS(cInfo); // TO Ondrej tu by sme este potrebovali reagovat na zmenu tak, ze sa nastavia hodnoty do modelu a prekresli sa grafika ak je zapnuta synchronizacia
                    // Napriklad ak je nastavene girts ILS = "None" a zmenim to na "2", tak sa zmeni premenna v modeli a dogeneruju sa pruty girt bracing block
                }
            }

            PropertyChanged(sender, e);
        }

        private void SetSameILS(CComponentInfo cInfo)
        {
            CComponentInfo cInfo2 = null;
            if (cInfo.MemberTypePosition == EMemberType_FS_Position.EdgeColumn)
            {
                cInfo2 = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainColumn);
            }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.MainColumn)
            {
                cInfo2 = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgeColumn);
            }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.EdgeRafter)
            {
                cInfo2 = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainRafter);
            }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.MainRafter)
            {
                cInfo2 = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgeRafter);
            }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.EdgePurlin)
            {
                cInfo2 = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Purlin);
            }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.Purlin)
            {
                cInfo2 = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgePurlin);
            }

            if (cInfo2 != null && cInfo.ILS != cInfo2.ILS)
            {
                cInfo2.IsSetFromCode = true;
                cInfo2.ILS = cInfo.ILS;
                cInfo2.IsSetFromCode = false;
            } 
        }


        private bool ValidateGirts()
        {
            //To Mato - ja osobne tejto podmienke proste nerozumiem
            //Ak je zaskrtnute generovanie front girts alebo back girts musia byt zaskrtnute aj girt (teda side wall)
            CComponentInfo girt = ComponentList.First(c => c.ComponentName == "Girt");
            CComponentInfo girtFront = ComponentList.First(c => c.ComponentName == "Girt - Front Side");
            CComponentInfo girtBack = ComponentList.First(c => c.ComponentName == "Girt - Back Side");

            //ak je front aj back false tak vtedy dovolit editovat girt
            if (!girtFront.Generate.Value && !girtBack.Generate.Value) { girt.GenerateIsEnabled = true; girt.GenerateIsReadonly = false; }
            else { girt.GenerateIsEnabled = false; girt.GenerateIsReadonly = true; }

            if (girt.GenerateIsEnabled && !girt.Generate.Value) { girtFront.GenerateIsEnabled = false; girtFront.GenerateIsReadonly = true; girtBack.GenerateIsEnabled = false; girtBack.GenerateIsReadonly = true; }
            else { girtFront.GenerateIsEnabled = true; girtFront.GenerateIsReadonly = false; girtBack.GenerateIsEnabled = true; girtBack.GenerateIsReadonly = false; }

            if (girt.Generate.Value)
            {
                return true;
            }
            else
            {
                if (girtFront.Generate.Value || girtBack.Generate.Value) return false;
            }

            return true;
        }

        private void SetGirtsAndColumns(CComponentInfo cInfo)
        {
            //Ak je zaskrtnute generovanie girts front side musia byt zapnute columns front side, a podobne pre back side girts a back side columns.
            if (cInfo.ComponentName == "Column - Front Side")
            {
                CComponentInfo girtFront = ComponentList.First(c => c.ComponentName == "Girt - Front Side");
                if (girtFront.Generate != cInfo.Generate) { girtFront.IsSetFromCode = true; girtFront.Generate = cInfo.Generate; girtFront.IsSetFromCode = false; }
                }
            else if (cInfo.ComponentName == "Column - Back Side")
            {
                CComponentInfo girtBack = ComponentList.First(c => c.ComponentName == "Girt - Back Side");
                if (girtBack.Generate != cInfo.Generate) { girtBack.IsSetFromCode = true; girtBack.Generate = cInfo.Generate; girtBack.IsSetFromCode = false; }
                }
            else if (cInfo.ComponentName == "Girt - Front Side" && cInfo.Generate.Value) //iba ked zapnem Girt tak sa musi zapnut aj column
            {
                CComponentInfo columnFront = ComponentList.First(c => c.ComponentName == "Column - Front Side");
                if (columnFront.Generate != cInfo.Generate) { columnFront.IsSetFromCode = true; columnFront.Generate = cInfo.Generate; columnFront.IsSetFromCode = false; }
                }
            else if (cInfo.ComponentName == "Girt - Back Side" && cInfo.Generate.Value) //iba ked zapnem Girt tak sa musi zapnut aj column
            {
                CComponentInfo columnBack = ComponentList.First(c => c.ComponentName == "Column - Back Side");
                if (columnBack.Generate != cInfo.Generate) { columnBack.IsSetFromCode = true; columnBack.Generate = cInfo.Generate; columnBack.IsSetFromCode = false; }
                }

            //task 505
            //volba generate by mala byt viazana na bool generate pre purlins resp. girts na jednotlivych stranach budovy, podobne ako su girts viazane na columns.

            if (cInfo.MemberTypePosition == EMemberType_FS_Position.Purlin)
            {
                CComponentInfo purlinBlock = ComponentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.BracingBlockPurlins);
                if (purlinBlock.Generate != cInfo.Generate) { purlinBlock.IsSetFromCode = true; purlinBlock.Generate = cInfo.Generate; purlinBlock.IsSetFromCode = false; }
                }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.BracingBlockPurlins && cInfo.Generate.Value) //iba ked zapnem blocks tak sa musi zapnut aj purlins
            {
                CComponentInfo purlin = ComponentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.Purlin);
                if (purlin.Generate != cInfo.Generate) { purlin.IsSetFromCode = true; purlin.Generate = cInfo.Generate; purlin.IsSetFromCode = false; }
                }


            if (cInfo.MemberTypePosition == EMemberType_FS_Position.GirtFrontSide) 
            {
                CComponentInfo ci = ComponentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.BracingBlockGirtsFrontSide);
                if (ci.Generate != cInfo.Generate) { ci.IsSetFromCode = true; ci.Generate = cInfo.Generate; ci.IsSetFromCode = false; }
                }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.GirtBackSide)
            {
                CComponentInfo ci = ComponentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.BracingBlockGirtsBackSide);
                if (ci.Generate != cInfo.Generate) { ci.IsSetFromCode = true; ci.Generate = cInfo.Generate; ci.IsSetFromCode = false; }
            }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.BracingBlockGirtsFrontSide && cInfo.Generate.Value) //iba ked zapnem blocks tak sa musi zapnut aj girt front
            {
                CComponentInfo ci = ComponentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.GirtFrontSide);
                if (ci.Generate != cInfo.Generate) { ci.IsSetFromCode = true; ci.Generate = cInfo.Generate; ci.IsSetFromCode = false; }
            }
            else if (cInfo.MemberTypePosition == EMemberType_FS_Position.BracingBlockGirtsBackSide && cInfo.Generate.Value) //iba ked zapnem blocks tak sa musi zapnut aj girt back
            {
                CComponentInfo ci = ComponentList.First(c => c.MemberTypePosition == EMemberType_FS_Position.GirtBackSide);
                if (ci.Generate != cInfo.Generate) { ci.IsSetFromCode = true; ci.Generate = cInfo.Generate; ci.IsSetFromCode = false; }
            }

        }

        public List<CSectionPropertiesText> ComponentDetailsList
        {
            get
            {
                if (m_ComponentDetailsList == null)
                {
                    m_ComponentDetailsList = CSectionManager.LoadSectionPropertiesNamesSymbolsUnits();
                }
                return m_ComponentDetailsList;
            }

            set
            {
                m_ComponentDetailsList = value;
                NotifyPropertyChanged("ComponentDetailsList");
            }
        }
        public List<CMaterialPropertiesText> MaterialDetailsList
        {
            get
            {
                if (m_MaterialDetailsList == null)
                {
                    m_MaterialDetailsList = CMaterialManager.LoadMaterialPropertiesNamesSymbolsUnits("MaterialsSQLiteDB");
                }
                return m_MaterialDetailsList;
            }

            set
            {
                m_MaterialDetailsList = value;
                NotifyPropertyChanged("MaterialDetailsList");
            }
        }

        private List<string> Sections
        {
            get
            {
                if (MSections == null)
                {
                    MSections = CDatabaseManager.GetStringList("SectionsSQLiteDB", "tableSections_m", "sectionName_short");
                }
                return MSections;
            }
        }

        // Nastavit pre rozne typy component iny obsah comboboxu "sections"
        // Mozno by to mohlo byt predpripravene v nejakom zozname sectionID v databaze pre kazdy prefix,
        // vid MDBModels - tabulka componentPrefixes a MDBSections, kazdy prefix (typ pruta) by potom obsahoval zoznam ID prierezov ktore mu mozu byt priradene

        /*
        -----------------------------------------
        ID  section   sectionName
        -----------------------------------------
        1   10075     Box 10075
        2   27055     C 27055
        3   27095     C 27095
        4   27095n    C 27095n
        5   270115    C 270115
        6   270115btb C 270115 back to back
        7   270115n   C 270115 nested
        8   50020     C 50020
        9   50020n    C 50020 nested
        10  63020     Box 63020
        11  63020s1   Box 63020 single stiffener
        12  63020s2   Box 63020 double stiffener
        13  1x50x1    Strip 1x50x1
        14  2x50x1    Strip 2x50x1
        15  3x50x1    Strip 3x50x1
        16  4x50x1    Strip 4x50x1
        17  1x100x1   Strip 1x100x1
        18  2x100x1   Strip 2x100x1
        19  3x100x1   Strip 3x100x1
        20  4x100x1   Strip 4x100x1
        */

        public List<string> SectionsForColumnsOrRafters
        {
            get
            {
                if (MSectionsForColumnsOrRafters == null)
                {
                    MSectionsForColumnsOrRafters = new List<string>(8);
                    MSectionsForColumnsOrRafters.Add(Sections[3]);   // DB ID 4
                    MSectionsForColumnsOrRafters.Add(Sections[4]);   // DB ID 5
                    MSectionsForColumnsOrRafters.Add(Sections[6]);   // DB ID 7
                    MSectionsForColumnsOrRafters.Add(Sections[7]);   // DB ID 8
                    MSectionsForColumnsOrRafters.Add(Sections[8]);   // DB ID 9
                    MSectionsForColumnsOrRafters.Add(Sections[9]);   // DB ID 10
                    MSectionsForColumnsOrRafters.Add(Sections[10]);  // DB ID 11
                    MSectionsForColumnsOrRafters.Add(Sections[11]);  // DB ID 12
                }
                return MSectionsForColumnsOrRafters;
            }
        }

        public List<string> SectionsForGirtsOrPurlins
        {
            get
            {
                if (MSectionsForGirtsOrPurlins == null)
                {
                    MSectionsForGirtsOrPurlins = new List<string>(6);
                    MSectionsForGirtsOrPurlins.Add(Sections[2]);   // DB ID 3
                    MSectionsForGirtsOrPurlins.Add(Sections[3]);   // DB ID 4
                    MSectionsForGirtsOrPurlins.Add(Sections[4]);   // DB ID 5
                    MSectionsForGirtsOrPurlins.Add(Sections[6]);   // DB ID 7
                    MSectionsForGirtsOrPurlins.Add(Sections[7]);   // DB ID 8
                    MSectionsForGirtsOrPurlins.Add(Sections[8]);   // DB ID 9
                }
                return MSectionsForGirtsOrPurlins;
            }
        }

        public List<string> SectionsForGirtsOrPurlinsBracing
        {
            get
            {
                if (MSectionsForGirtsOrPurlinsBracing == null)
                {
                    MSectionsForGirtsOrPurlinsBracing = new List<string>(2);
                    MSectionsForGirtsOrPurlinsBracing.Add(Sections[1]);   // DB ID 2
                    MSectionsForGirtsOrPurlinsBracing.Add(Sections[2]);   // DB ID 3
                }
                return MSectionsForGirtsOrPurlinsBracing;
            }
        }

        public List<string> SectionsForDoorOrWindowFrame
        {
            get
            {
                if (MSectionsForDoorOrWindowFrame == null)
                {
                    MSectionsForDoorOrWindowFrame = new List<string>(1);
                    MSectionsForDoorOrWindowFrame.Add(Sections[0]); // DB ID 1

                }
                return MSectionsForDoorOrWindowFrame;
            }
        }

        public List<string> SectionsForRollerDoorTrimmer
        {
            get
            {
                if (MSectionsForRollerDoorTrimmer == null)
                {
                    MSectionsForRollerDoorTrimmer = new List<string>(1);
                    MSectionsForRollerDoorTrimmer.Add(Sections[5]); // DB ID 6

                }
                return MSectionsForRollerDoorTrimmer;
            }
        }
        public List<string> SectionsForRollerDoorLintel
        {
            get
            {
                if (MSectionsForRollerDoorLintel == null)
                {
                    MSectionsForRollerDoorLintel = new List<string>(1);
                    MSectionsForRollerDoorLintel.Add(Sections[2]); // DB ID 3

                }
                return MSectionsForRollerDoorLintel;
            }
        }

        public List<string> SectionsForCrossBracing
        {
            get
            {
                if (MSectionsForCrossBracing == null)
                {
                    MSectionsForCrossBracing = new List<string>(8);
                    MSectionsForCrossBracing.Add(Sections[12]);   // DB ID 13
                    MSectionsForCrossBracing.Add(Sections[13]);   // DB ID 14
                    MSectionsForCrossBracing.Add(Sections[14]);   // DB ID 15
                    MSectionsForCrossBracing.Add(Sections[15]);   // DB ID 16
                    MSectionsForCrossBracing.Add(Sections[16]);   // DB ID 17
                    MSectionsForCrossBracing.Add(Sections[17]);   // DB ID 18
                    MSectionsForCrossBracing.Add(Sections[18]);   // DB ID 19
                    MSectionsForCrossBracing.Add(Sections[19]);   // DB ID 20
                }
                return MSectionsForCrossBracing;
            }
        }

        public int SelectedComponentIndex
        {
            get
            {
                return MSelectedComponentIndex;
            }

            set
            {
                MSelectedComponentIndex = value;
                SetComponentDetails();
                NotifyPropertyChanged("SelectedComponentIndex");
            }
        }

        public List<string> Materials
        {
            get
            {
                if (MMaterials == null) MMaterials = CMaterialManager.GetMaterialTypesList();
                return MMaterials;
            }
        }

        public List<BaseClasses.Helpers.CComboColor> Colors
        {
            get
            {
                 return MColors;
            }

            set
            {
                MColors = value;
            }
        }

        // TODO - pocet poloziek by mohol byt zavisly na tom kolko purlins sa vygenerovalo, aby nebolo mozne nastavit vaznicu s vyssim poradim nez existuju na jednej priecli (rafter)
        public List<string> ColumnFlyBracingPosition_Items
        {
            get
            {
                if (MColumnFlyBracingPosition_Items == null)
                {
                    MColumnFlyBracingPosition_Items = new List<string>() { "None", "Every girt", "Every 2nd girt", "Every 3rd girt", "Every 4th girt", "Every 5th girt",
                        "Every 6th girt", "Every 7th girt", "Every 8th girt", "Every 9th girt"};
                }
                return MColumnFlyBracingPosition_Items;
            }
            set
            {
                MColumnFlyBracingPosition_Items = value;
                NotifyPropertyChanged("ColumnFlyBracingPosition_Items");
            }
        }
        public List<string> RafterFlyBracingPosition_Items
        {
            get
            {
                if (MRafterFlyBracingPosition_Items == null)
                {
                    MRafterFlyBracingPosition_Items = new List<string>() { "None", "Every purlin", "Every 2nd purlin", "Every 3rd purlin", "Every 4th purlin", "Every 5th purlin",
                        "Every 6th purlin", "Every 7th purlin", "Every 8th purlin", "Every 9th purlin"};
                }
                return MRafterFlyBracingPosition_Items;
            }
            set
            {
                MRafterFlyBracingPosition_Items = value;
                NotifyPropertyChanged("RafterFlyBracingPosition_Items");
            }
        }
        public List<string> DefaultILS_Items
        {
            get
            {
                if (MDefaultILS_Items == null)
                {
                    MDefaultILS_Items = new List<string>() { "None", "1", "2", "3", "4", "5" };
                }
                return MDefaultILS_Items;
            }
        }
        public List<string> EmptyILS_Items
        {
            get
            {
                if (MEmptyILS_Items == null)
                {
                    MEmptyILS_Items = new List<string>() { "None" };
                }
                return MEmptyILS_Items;
            }
        }

        public bool AllMaterialListChanged
        {
            get
            {
                return m_allMaterialListChanged;
            }

            set
            {
                m_allMaterialListChanged = value;
                NotifyPropertyChanged("AllMaterialListChanged");
            }
        }

        private List<CComponentPrefixes> list_CompPref;
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CComponentListVM()
        {
            MComponentList = new ObservableCollection<CComponentInfo>();

            list_CompPref = CComponentManager.LoadComponentsPrefixes();

            MColors = CComboBoxHelper.ColorList; // Set Color List

            CComponentInfo ci = null;
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eMC].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eMC].ComponentColorName)), 
                list_CompPref[(int)EMemberType_FS.eMC].ComponentName, "63020", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForColumnsOrRafters, ColumnFlyBracingPosition_Items, Colors, EMemberType_FS_Position.MainColumn);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eMR].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eMR].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eMR].ComponentName, "63020", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForColumnsOrRafters, RafterFlyBracingPosition_Items, Colors, EMemberType_FS_Position.MainRafter);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eEC].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eEC].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eEC].ComponentName, "63020", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForColumnsOrRafters, ColumnFlyBracingPosition_Items, Colors, EMemberType_FS_Position.EdgeColumn);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eER].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eER].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eER].ComponentName, "63020", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForColumnsOrRafters, RafterFlyBracingPosition_Items, Colors, EMemberType_FS_Position.EdgeRafter);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eEP].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eEP].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eEP].ComponentName, "50020", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForGirtsOrPurlins, DefaultILS_Items, Colors, EMemberType_FS_Position.EdgePurlin);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eG].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eG].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eG].ComponentName, "27095", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForGirtsOrPurlins, DefaultILS_Items, Colors, EMemberType_FS_Position.Girt);
            ci.GenerateIsEnabled = false; ci.GenerateIsReadonly = true;
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eP].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eP].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eP].ComponentName, "270115", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForGirtsOrPurlins, DefaultILS_Items, Colors, EMemberType_FS_Position.Purlin);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eWP].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eWP].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eWP].ComponentName + " - Front Side", "270115n", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForColumnsOrRafters, ColumnFlyBracingPosition_Items, Colors, EMemberType_FS_Position.WindPostFrontSide);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eWP].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eWP].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eWP].ComponentName + " - Back Side", "270115n", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForColumnsOrRafters, ColumnFlyBracingPosition_Items, Colors, EMemberType_FS_Position.WindPostBackSide);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eG].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eG].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eG].ComponentName + " - Front Side", "27095", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForGirtsOrPurlins, DefaultILS_Items, Colors, EMemberType_FS_Position.GirtFrontSide);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eG].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eG].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eG].ComponentName + " - Back Side", "27095", "Green", "G550‡", "None", true, true, true, true, true, 
                SectionsForGirtsOrPurlins, DefaultILS_Items, Colors, EMemberType_FS_Position.GirtBackSide);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eGB].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eGB].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eGB].ComponentName, "27095", "Green", "G550‡", "None", true, true, false, false, true,
                SectionsForGirtsOrPurlinsBracing, EmptyILS_Items, Colors, EMemberType_FS_Position.BracingBlockGirts);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.ePB].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.ePB].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.ePB].ComponentName, "27095", "Green", "G550‡", "None", true, true, false, false, true,
                SectionsForGirtsOrPurlinsBracing, EmptyILS_Items, Colors, EMemberType_FS_Position.BracingBlockPurlins);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eGB].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eGB].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eGB].ComponentName + " - Front Side", "27095", "Green", "G550‡", "None", true, true, false, false, true,
                SectionsForGirtsOrPurlinsBracing, EmptyILS_Items, Colors, EMemberType_FS_Position.BracingBlockGirtsFrontSide);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eGB].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eGB].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eGB].ComponentName + " - Back Side", "27095", "Green", "G550‡", "None", true, true, false, false, true,
                SectionsForGirtsOrPurlinsBracing, EmptyILS_Items, Colors, EMemberType_FS_Position.BracingBlockGirtsBackSide);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eCB].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eCB].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eCB].ComponentName + " - Walls", "1x100x1", "Olive", "G550‡", "None", true, true, false, false, true,
                SectionsForCrossBracing, EmptyILS_Items, Colors, EMemberType_FS_Position.CrossBracingWall);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eCB].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eCB].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eCB].ComponentName + " - Roof", "1x100x1", "Olive", "G550‡", "None", true, true, false, false, true,
                SectionsForCrossBracing, EmptyILS_Items, Colors, EMemberType_FS_Position.CrossBracingRoof);
            MComponentList.Add(ci);

            SetComponentSectionsColors();

            foreach (CComponentInfo cInfo in MComponentList)
            {
                cInfo.PropertyChanged += ComponentListItem_PropertyChanged;
            }
        }

        private void SetComponentSectionsColors()
        {
            foreach (CComponentInfo cInfo in MComponentList)
            {
                CrScProperties prop = CSectionManager.GetSectionProperties(cInfo.Section);
                cInfo.SectionColor = prop.colorName;
            }
        }

        public void SetModelComponentListProperties(Dictionary<EMemberType_FS_Position, string> MembersSectionsDict)
        {
            foreach (CComponentInfo cInfo in ComponentList)
            {
                cInfo.IsSetFromCode = true;
                cInfo.Section = MembersSectionsDict[cInfo.MemberTypePosition];
                cInfo.IsSetFromCode = false;
            }
        }
        public void SetILSProperties(CDatabaseModels dmodel)
        {
            //TO Mato - lepsie ak by z Db chodila hned hodnota a nie index do nejakeho pola
            CComponentInfo ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainColumn);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iMainColumnFlyBracingEveryXXGirt); ci.IsSetFromCode = false; }
            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgeColumn);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iMainColumnFlyBracingEveryXXGirt); ci.IsSetFromCode = false; }

            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainRafter);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iRafterFlyBracingEveryXXPurlin); ci.IsSetFromCode = false; }
            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgeRafter);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iRafterFlyBracingEveryXXPurlin); ci.IsSetFromCode = false; }

            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgePurlin);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iEdgePurlin_ILS_Number); ci.IsSetFromCode = false; }

            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Girt);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iGirt_ILS_Number); ci.IsSetFromCode = false; }
            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.Purlin);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iPurlin_ILS_Number); ci.IsSetFromCode = false; }

            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.WindPostFrontSide);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iFrontColumnFlyBracingEveryXXGirt); ci.IsSetFromCode = false; }
            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.WindPostBackSide);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iBackColumnFlyBracingEveryXXGirt); ci.IsSetFromCode = false; }

            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.GirtFrontSide);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iGirtFrontSide_ILS_Number); ci.IsSetFromCode = false; }
            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.GirtBackSide);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, dmodel.iGirtBackSide_ILS_Number); ci.IsSetFromCode = false; }

            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.BracingBlockGirts);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, 0); ci.IsSetFromCode = false; }
            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.BracingBlockPurlins);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, 0); ci.IsSetFromCode = false; }
            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.BracingBlockGirtsFrontSide);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, 0); ci.IsSetFromCode = false; }
            ci = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.BracingBlockGirtsBackSide);
            if (ci != null) { ci.IsSetFromCode = true; SetComponentInfoILS(ci, 0); ci.IsSetFromCode = false; }
        }

        private void SetComponentInfoILS(CComponentInfo ci, int index)
        {
            if (ci == null) return;
            string ils = ci.ILS_Items.ElementAtOrDefault(index);
            if (ils == null) ils = ci.ILS_Items.FirstOrDefault();
            ci.ILS = ils;
        }
        private void SetComponentInfoILS(CComponentInfo ci)
        {
            if (ci == null) return;

            int index = ci.ILS_Items.IndexOf(ci.ILS); // in case that it not longer exists in ILS_Items - it will set it to first
            SetComponentInfoILS(ci, index);  
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetComponentDetails()
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Cross-section details
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (MSelectedComponentIndex == -1) return;
            CComponentInfo cInfo = ComponentList[MSelectedComponentIndex];
            if (cInfo == null) return;

            // Cross-section properties
            List<string> listSectionPropertyValue = CSectionManager.LoadSectionPropertiesStringList(cInfo.Section);

            for (int i = 0; i < ComponentDetailsList.Count; i++)
            {
                ComponentDetailsList[i].Value = listSectionPropertyValue[i];
            }
            ComponentDetailsList.RemoveAll(d => d.VisibleInGUI == false);

            ComponentDetailsList = new List<CSectionPropertiesText>(ComponentDetailsList);

            // Material properties
            List<string> listMaterialPropertyValue = CMaterialManager.LoadSteelMaterialPropertiesStringList(cInfo.Material);

            for (int i = 0; i < MaterialDetailsList.Count; i++)
            {
                MaterialDetailsList[i].Value = listMaterialPropertyValue[i];
            }
            MaterialDetailsList = new List<CMaterialPropertiesText>(MaterialDetailsList);
        }

        public void AddPersonelDoor()
        {
            CComponentInfo cInfo = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.DoorFrame);
            if (cInfo != null) return; //already exist in the collection

            CrScProperties prop = CSectionManager.GetSectionProperties("10075");

            cInfo = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eDF].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eDF].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eDF].ComponentName, "10075", prop.colorName, "G550‡", "None", null, true, false, false, true, 
                SectionsForDoorOrWindowFrame, EmptyILS_Items, Colors, EMemberType_FS_Position.DoorFrame);
            cInfo.PropertyChanged += ComponentListItem_PropertyChanged;
            ComponentList.Add(cInfo);

        }
        public void RemovePersonelDoor()
        {
            CComponentInfo cInfo = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.DoorFrame);
            if (cInfo != null) ComponentList.Remove(cInfo);
        }

        public void AddRollerDoor()
        {
            CComponentInfo cDT = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.DoorTrimmer);
            if (cDT == null)
            {
                CrScProperties prop = CSectionManager.GetSectionProperties("270115btb");

                cDT = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eDT].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eDT].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eDT].ComponentName, "270115btb", prop.colorName, "G550‡", "None", null, true, false, false, true, 
                SectionsForRollerDoorTrimmer, EmptyILS_Items, Colors, EMemberType_FS_Position.DoorTrimmer);
                cDT.PropertyChanged += ComponentListItem_PropertyChanged;
                ComponentList.Add(cDT);
            }

            CComponentInfo cDL = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.DoorLintel);
            if (cDL == null)
            {
                CrScProperties prop = CSectionManager.GetSectionProperties("27095");

                cDL = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eDL].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eDL].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eDL].ComponentName, "27095", prop.colorName, "G550‡", "None", null, true, false, false, true, 
                SectionsForRollerDoorLintel, EmptyILS_Items, Colors, EMemberType_FS_Position.DoorLintel);
                cDL.PropertyChanged += ComponentListItem_PropertyChanged;
                ComponentList.Add(cDL);
            }
        }
        public void RemoveRollerDoor()
        {
            CComponentInfo cDT = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.DoorTrimmer);
            if (cDT != null) ComponentList.Remove(cDT);

            CComponentInfo cDL = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.DoorLintel);
            if (cDL != null) ComponentList.Remove(cDL);
        }

        public void AddWindow()
        {
            CComponentInfo cInfo = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.WindowFrame);
            if (cInfo != null) return; //already exist in the collection

            CrScProperties prop = CSectionManager.GetSectionProperties("10075");

            cInfo = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eWF].ComponentPrefix, MColors.Find(x => x.Name.Equals(list_CompPref[(int)EMemberType_FS.eWF].ComponentColorName)),
                list_CompPref[(int)EMemberType_FS.eWF].ComponentName, "10075", prop.colorName, "G550‡", "None", null, true, false, false, true, 
                SectionsForDoorOrWindowFrame, EmptyILS_Items, MColors, EMemberType_FS_Position.WindowFrame);
            cInfo.PropertyChanged += ComponentListItem_PropertyChanged;
            ComponentList.Add(cInfo);
        }
        public void RemoveWindow()
        {
            CComponentInfo cInfo = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.WindowFrame);
            if (cInfo != null) ComponentList.Remove(cInfo);
        }

        public bool NoFrameMembersForCalculate()
        {
            return !ComponentList.Any(c => (c.MemberTypePosition == EMemberType_FS_Position.MainColumn ||
            c.MemberTypePosition == EMemberType_FS_Position.MainRafter ||
            c.MemberTypePosition == EMemberType_FS_Position.EdgeColumn ||
            c.MemberTypePosition == EMemberType_FS_Position.EdgeRafter) && c.Calculate == true);
        }

        public bool NoCompomentsForCalculate()
        {
            return !ComponentList.Any(c => c.Calculate == true);
        }
        public bool NoCompomentsForDesign()
        {
            return !ComponentList.Any(c => c.Design == true);
        }
        public bool NoCompomentsForMaterialList()
        {
            return !ComponentList.Any(c => c.MaterialList == true);
        }

        public void SetRafterFlyBracingPosition_Items(int iPurlinsNum)
        {
            List<string> items = new List<string>();
            for (int i = 0; i <= iPurlinsNum; i++)
            {
                if(i == 0) items.Add("None");
                if (i == 1) items.Add("Every purlin");
                if (i == 2) items.Add("Every 2nd purlin");
                if (i == 3) items.Add("Every 3rd purlin");
                if (i >= 4) items.Add($"Every {i}th purlin");

            }
            RafterFlyBracingPosition_Items = items;

            CComponentInfo MR = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainRafter);
            if (MR != null) { MR.IsSetFromCode = true; MR.ILS_Items = items; SetComponentInfoILS(MR); MR.IsSetFromCode = false; }
            CComponentInfo ER = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgeRafter);
            if (ER != null) { ER.IsSetFromCode = true; ER.ILS_Items = items; SetComponentInfoILS(ER); ER.IsSetFromCode = false; }
        }
        
        public void SetColumnFlyBracingPosition_Items(int iGirtsNum)
        {
            List<string> items = new List<string>();
            for (int i = 0; i <= iGirtsNum; i++)
            {
                if (i == 0) items.Add("None");
                if (i == 1) items.Add("Every girt");
                if (i == 2) items.Add("Every 2nd girt");
                if (i == 3) items.Add("Every 3rd girt");
                if (i >= 4) items.Add($"Every {i}th girt");

            }
            ColumnFlyBracingPosition_Items = items;

            CComponentInfo MC = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.MainColumn);
            if (MC != null) { MC.IsSetFromCode = true; MC.ILS_Items = items; SetComponentInfoILS(MC); MC.IsSetFromCode = false; }
            CComponentInfo EC = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.EdgeColumn);
            if (EC != null) { EC.IsSetFromCode = true; EC.ILS_Items = items; SetComponentInfoILS(EC); EC.IsSetFromCode = false; }
        }
        public void SetFrontColumnFlyBracingPosition_Items(int iFrontColumnGirtsNum)
        {
            List<string> items = new List<string>();
            for (int i = 0; i <= iFrontColumnGirtsNum; i++)
            {
                if (i == 0) items.Add("None");
                if (i == 1) items.Add("Every girt");
                if (i == 2) items.Add("Every 2nd girt");
                if (i == 3) items.Add("Every 3rd girt");
                if (i >= 4) items.Add($"Every {i}th girt");

            }
            ColumnFlyBracingPosition_Items = items;
            CComponentInfo CFS = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.WindPostFrontSide);
            if (CFS != null) { CFS.IsSetFromCode = true; CFS.ILS_Items = items; SetComponentInfoILS(CFS); CFS.IsSetFromCode = false; }
        }
        public void SetBackColumnFlyBracingPosition_Items(int iBackColumnGirtsNum)
        {
            List<string> items = new List<string>();
            for (int i = 0; i <= iBackColumnGirtsNum; i++)
            {
                if (i == 0) items.Add("None");
                if (i == 1) items.Add("Every girt");
                if (i == 2) items.Add("Every 2nd girt");
                if (i == 3) items.Add("Every 3rd girt");
                if (i >= 4) items.Add($"Every {i}th girt");

            }
            ColumnFlyBracingPosition_Items = items;
            CComponentInfo CBS = ComponentList.FirstOrDefault(c => c.MemberTypePosition == EMemberType_FS_Position.WindPostBackSide);
            if (CBS != null) { CBS.IsSetFromCode = true; CBS.ILS_Items = items; SetComponentInfoILS(CBS); CBS.IsSetFromCode = false; }
        }
    }
}
