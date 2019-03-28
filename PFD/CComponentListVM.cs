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

namespace PFD
{
    public class CComponentListVM : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<CComponentInfo> MComponentList;
        private int MSelectedComponentIndex;

        private List<string> MSections;
        private List<string> MSectionsForColumnsOrRafters;
        private List<string> MSectionsForGirtsOrPurlins;
        private List<string> MSectionsForDoorOrWindowFrame;
        private List<string> MSectionsForRollerDoorTrimmer;

        private List<string> MMaterials;
        private List<CSectionPropertiesText> m_ComponentDetailsList;
        private List<CMaterialPropertiesText> m_MaterialDetailsList;

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

                if(e.PropertyName == "Material") SetComponentDetails();
                else if(e.PropertyName == "Section") SetComponentDetails();

                if (e.PropertyName == "Generate")
                {
                    if (!ValidateGirts()) { cInfo.Generate = !cInfo.Generate; return; }
                    SetGirtsAndColumns(cInfo);
                }
            }

            PropertyChanged(sender, e);
        }

        private bool ValidateGirts()
        {
            //Ak je zaskrtnute generovanie front girts alebo back girts musia byt zaskrtnute aj girt (teda side wall)
            CComponentInfo girt = ComponentList.First(c => c.ComponentName == "Girt");
            CComponentInfo girtFront = ComponentList.First(c => c.ComponentName == "Girt - Front Side");
            CComponentInfo girtBack = ComponentList.First(c => c.ComponentName == "Girt - Back Side");

            if (girt.Generate)
            {
                return true;
            }
            else
            {
                if (girtFront.Generate || girtBack.Generate) return false;
            }

            return true;
        }

        private void SetGirtsAndColumns(CComponentInfo cInfo)
        {
            //Ak je zaskrtnute generovanie girts front side musia byt zapnute columns front side, a podobne pre back side girts a back side columns.
            if (cInfo.ComponentName == "Column - Front Side")
            {
                CComponentInfo girtFront = ComponentList.First(c => c.ComponentName == "Girt - Front Side");
                if(girtFront.Generate != cInfo.Generate) girtFront.Generate = cInfo.Generate;
            }
            else if (cInfo.ComponentName == "Column - Back Side")
            {
                CComponentInfo girtBack = ComponentList.First(c => c.ComponentName == "Girt - Back Side");
                if (girtBack.Generate != cInfo.Generate) girtBack.Generate = cInfo.Generate;
            }
            else if (cInfo.ComponentName == "Girt - Front Side" && cInfo.Generate) //iba ked zapnem Girt tak sa musi zapnut aj column
            {
                CComponentInfo columnFront = ComponentList.First(c => c.ComponentName == "Column - Front Side");
                if (columnFront.Generate != cInfo.Generate) columnFront.Generate = cInfo.Generate;
            }
            else if (cInfo.ComponentName == "Girt - Back Side" && cInfo.Generate) //iba ked zapnem Girt tak sa musi zapnut aj column
            {
                CComponentInfo columnBack = ComponentList.First(c => c.ComponentName == "Column - Back Side");
                if (columnBack.Generate != cInfo.Generate) columnBack.Generate = cInfo.Generate;
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
                    m_MaterialDetailsList = CMaterialManager.LoadMaterialPropertiesNamesSymbolsUnits();
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

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CComponentListVM()
        {
            MComponentList = new ObservableCollection<CComponentInfo>();
            
            List<CComponentPrefixes> list_CompPref = CComponentManager.LoadComponentsPrefixes();

            CComponentInfo ci = null;
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eMC].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eMC].ComponentName, "63020", "G550‡", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eMR].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eMR].ComponentName, "63020", "G550‡", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eEC].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eEC].ComponentName, "63020", "G550‡", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eER].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eER].ComponentName, "63020", "G550‡", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eEP].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eEP].ComponentName, "50020", "G550‡", true, true, true, true, true, SectionsForGirtsOrPurlins);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eG].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eG].ComponentName, "27095", "G550‡", true, true, true, true, true, SectionsForGirtsOrPurlins);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eP].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eP].ComponentName, "270115", "G550‡", true, true, true, true, true, SectionsForGirtsOrPurlins);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eC].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eC].ComponentName + " - Front Side", "270115n", "G550‡", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eC].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eC].ComponentName + " - Back Side", "270115n", "G550‡", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eG].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eG].ComponentName + " - Front Side", "27095", "G550‡", true, true, true, true, true, SectionsForGirtsOrPurlins);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eG].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eG].ComponentName + " - Back Side", "27095", "G550‡", true, true, true, true, true, SectionsForGirtsOrPurlins);
            MComponentList.Add(ci);

            foreach (CComponentInfo cInfo in MComponentList)
            {
                cInfo.PropertyChanged += ComponentListItem_PropertyChanged;
            }
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
            CComponentInfo cInfo = ComponentList[MSelectedComponentIndex];
            if (cInfo == null) return;

            // Cross-section properties
            List<string> listSectionPropertyValue = CSectionManager.LoadSectionPropertiesStringList(cInfo.Section);

            for (int i = 0; i < ComponentDetailsList.Count; i++)
            {
                ComponentDetailsList[i].Value = listSectionPropertyValue[i];
            }
            ComponentDetailsList = new List<CSectionPropertiesText>(ComponentDetailsList);

            // Material properties
            List<string> listMaterialPropertyValue = CMaterialManager.LoadMaterialPropertiesStringList(cInfo.Material);

            for (int i = 0; i < MaterialDetailsList.Count; i++)
            {
                MaterialDetailsList[i].Value = listMaterialPropertyValue[i];
            }
            MaterialDetailsList = new List<CMaterialPropertiesText>(MaterialDetailsList);
        }
    }
}
