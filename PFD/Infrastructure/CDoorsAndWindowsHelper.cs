using BaseClasses;
using DATABASE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public static class CDoorsAndWindowsHelper
    {
        public static ObservableCollection<DoorProperties> GetDefaultDoorProperties(bool bRelease)
        {
            ObservableCollection<DoorProperties> DoorBlocks = new ObservableCollection<DoorProperties>();
            if (!bRelease) // To Ondrej - v release verzii tu mam vratit prazdnu kolekciu alebo null?
            {
                DoorProperties doorProps;

                doorProps = new DoorProperties();
                doorProps.sBuildingSide = "Front";
                doorProps.iBayNumber = 2;
                doorProps.sDoorType = "Roller Door";
                doorProps.fDoorsHeight = 2.1f;
                doorProps.fDoorsWidth = 1.2f;
                doorProps.fDoorCoordinateXinBlock = 0.3f;
                doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                DoorBlocks.Add(doorProps);

                doorProps = new DoorProperties();
                doorProps.sBuildingSide = "Left";
                doorProps.iBayNumber = 1;
                doorProps.sDoorType = "Roller Door";
                doorProps.fDoorsHeight = 2.1f;
                doorProps.fDoorsWidth = 2.2f;
                doorProps.fDoorCoordinateXinBlock = 0.5f;
                doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                DoorBlocks.Add(doorProps);
                
                doorProps = new DoorProperties();
                doorProps.sBuildingSide = "Right";
                doorProps.iBayNumber = 3;
                doorProps.sDoorType = "Roller Door";
                doorProps.fDoorsHeight = 1.1f;
                doorProps.fDoorsWidth = 1.8f;
                doorProps.fDoorCoordinateXinBlock = 0.6f;
                doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                DoorBlocks.Add(doorProps);
                
                doorProps = new DoorProperties();
                doorProps.sBuildingSide = "Back";
                doorProps.iBayNumber = 2;
                doorProps.sDoorType = "Roller Door";
                doorProps.fDoorsHeight = 1.1f;
                doorProps.fDoorsWidth = 0.9f;
                doorProps.fDoorCoordinateXinBlock = 0.4f;
                doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                DoorBlocks.Add(doorProps);
                
                doorProps = new DoorProperties();
                doorProps.sBuildingSide = "Back";
                doorProps.iBayNumber = 3;
                doorProps.sDoorType = "Roller Door";
                doorProps.fDoorsHeight = 2.1f;
                doorProps.fDoorsWidth = 0.8f;
                doorProps.fDoorCoordinateXinBlock = 0.5f;
                doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                DoorBlocks.Add(doorProps);
            }

            return DoorBlocks;
        }

        public static void SetDefaultDoorParams(DoorProperties d)
        {
            d.sBuildingSide = "Front";
            d.iBayNumber = 1;
            d.sDoorType = "Personnel Door";
            d.fDoorsWidth = 0.8f;
            d.fDoorsHeight = 2.1f;
            d.fDoorCoordinateXinBlock = 0.4f;
            d.CoatingColor = d.CoatingColors.FirstOrDefault();
        }
        public static void SetDefaultWindowParams(WindowProperties w)
        {
            w.sBuildingSide = "Front";
            w.iBayNumber = 1;
            w.fWindowsHeight = 0.6f;
            w.fWindowsWidth = 0.6f;
            w.fWindowCoordinateXinBay = 0.4f;
            w.fWindowCoordinateZinBay = 2f;
            w.iNumberOfWindowColumns = 2;
            w.CoatingColor = w.CoatingColors.FirstOrDefault();
        }

        public static ObservableCollection<WindowProperties> GetDefaultWindowsProperties(bool bRelease)
        {
            ObservableCollection<WindowProperties> WindowBlocks = new ObservableCollection<WindowProperties>();

            if (!bRelease) // To Ondrej - v release verzii tu mam vratit prazdnu kolekciu alebo null?
            {
                WindowProperties windowProps = new WindowProperties();
                windowProps.sBuildingSide = "Left"; // "Front"
                windowProps.iBayNumber = 2;
                windowProps.fWindowsHeight = 0.6f;
                windowProps.fWindowsWidth = 0.6f;
                windowProps.fWindowCoordinateXinBay = 0.4f;
                windowProps.fWindowCoordinateZinBay = 0.8f;
                windowProps.iNumberOfWindowColumns = 2;
                windowProps.CoatingColor = windowProps.CoatingColors.FirstOrDefault();
                WindowBlocks.Add(windowProps);
            }
            return WindowBlocks;
        }
    }
}
