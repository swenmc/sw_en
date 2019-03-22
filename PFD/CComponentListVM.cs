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
        4   270115    C 270115
        5   270115btb C 270115 back to back
        6   270115n   C 270115 nested
        7   50020     C 50020
        8   50020n    C 50020 nested
        9   63020     Box 63020
        10  63020s1   Box 63020 single stiffener
        11  63020s2   Box 63020 double stiffener
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
                    //MSections = new List<string> { "Box 63020", "Box 63020", "C 50020", "C 27095", "C 270115", "Box 10075", "Box 10075", "C 27095", "C 27095" };
                    MSections = CDatabaseManager.GetStringList("ModelsSQLiteDB", "sections", "sectionName");
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
                    MSectionsForColumnsOrRafters = new List<string>(5);
                    MSectionsForColumnsOrRafters.Add(Sections[0]);   // DB ID ? toto som pridal lebo inak bolo pre C nastavenie nieco co nebolo v zozname
                    MSectionsForColumnsOrRafters.Add(Sections[5]);   // DB ID 6
                    MSectionsForColumnsOrRafters.Add(Sections[7]);   // DB ID 8
                    MSectionsForColumnsOrRafters.Add(Sections[8]);   // DB ID 9
                    MSectionsForColumnsOrRafters.Add(Sections[9]);   // DB ID 10
                    MSectionsForColumnsOrRafters.Add(Sections[10]);  // DB ID 11
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
                    MSectionsForGirtsOrPurlins = new List<string>(5);
                    MSectionsForGirtsOrPurlins.Add(Sections[2]);   // DB ID 3
                    MSectionsForGirtsOrPurlins.Add(Sections[3]);   // DB ID 4
                    MSectionsForGirtsOrPurlins.Add(Sections[5]);   // DB ID 6
                    MSectionsForGirtsOrPurlins.Add(Sections[6]);   // DB ID 7
                    MSectionsForGirtsOrPurlins.Add(Sections[7]);   // DB ID 8
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
                    MSectionsForRollerDoorTrimmer.Add(Sections[4]); // DB ID 5

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
            CDatabaseComponents database = new CDatabaseComponents(); // System components database

            CComponentInfo ci = null;            
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eMC, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eMC, 1], "Box 63020", "G550", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eMR, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eMR, 1], "Box 63020", "G550", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eEC, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eEC, 1], "Box 63020", "G550", true, true, true, true, true, Sections);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eER, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eER, 1], "Box 63020", "G550", true, true, true, true, true, Sections);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eEP, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eEP, 1], "C 50020", "G550", true, true, true, true, true, SectionsForGirtsOrPurlins);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 1], "C 27095", "G550", true, true, true, true, true, SectionsForGirtsOrPurlins);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eP, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eP, 1], "C 270115", "G550", true, true, true, true, true, SectionsForGirtsOrPurlins);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eC, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eC, 1] + " - Front Side", "Box 10075", "G550", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eC, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eC, 1] + " - Back Side", "Box 10075", "G550", true, true, true, true, true, SectionsForColumnsOrRafters);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 1] + " - Front Side", "C 27095", "G550", true, true, true, true, true, Sections);
            MComponentList.Add(ci);
            ci = new CComponentInfo(database.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 0], 
                database.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 1] + " - Back Side", "C 27095", "G550", true, true, true, true, true, Sections);
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
