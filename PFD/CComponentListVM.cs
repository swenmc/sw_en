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

        // DATABASE DATA (MDBModels - table sections)
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

        private List<string> MSections;
        private List<string> MSectionsForColumnsOrRafters;
        private List<string> MSectionsForGirtsOrPurlins;
        private List<string> MSectionsForDoorOrWindowFrame;
        private List<string> MSectionsForRollerDoorTrimmer;

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
                NotifyPropertyChanged("ComponentList");
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

        // TODO No 47 - Ondrej - nastavit pre rozne riadky iny obsah comboboxu "sections"
        // Pre prefixy MC, MR, C nastavit SectionsForColumnsOrRafters
        // Pre prefixy EP, G, P nastavit SectionsForGirtsOrPurlins
        // mozno by to mohlo byt pripravene v nejakom zozname sectionID v databaze pre kazdy prefix, vid MDBModels - tabulka componentPrefixes a tabulka sections

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
                NotifyPropertyChanged("SelectedComponentIndex");
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CComponentListVM()
        {
            // TODO No 47 - Ondrej - nastavit pre rozne riadky iny obsah comboboxu "sections"
            // Pre prefixy MC, MR, C nastavit SectionsForColumnsOrRafters
            // Pre prefixy EP, G, P nastavit SectionsForGirtsOrPurlins
            //a pre ostatne? nastavil som vsetky
            
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
                list_CompPref[(int)EMemberType_FS.eEC].ComponentName, "63020", "G550‡", true, true, true, true, true, Sections);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eER].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eER].ComponentName, "63020", "G550‡", true, true, true, true, true, Sections);
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
                list_CompPref[(int)EMemberType_FS.eG].ComponentName + " - Front Side", "27095", "G550‡", true, true, true, true, true, Sections);
            MComponentList.Add(ci);
            ci = new CComponentInfo(list_CompPref[(int)EMemberType_FS.eG].ComponentPrefix,
                list_CompPref[(int)EMemberType_FS.eG].ComponentName + " - Back Side", "27095", "G550‡", true, true, true, true, true, Sections);
            MComponentList.Add(ci);

            SelectedComponentIndex = 0;
        }

        //-------------------------------------------------------------------------------------------------------------
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
