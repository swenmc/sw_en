using BaseClasses;
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
        public static ObservableCollection<DoorProperties> GetDefaultDoorProperties()
        {
            ObservableCollection<DoorProperties> DoorBlocks = new ObservableCollection<DoorProperties>();
            DoorProperties doorProps = new DoorProperties();
            doorProps.sBuildingSide = "Front";
            doorProps.iBayNumber = 1;
            doorProps.sDoorType = "Personnel Door";
            doorProps.fDoorsHeight = 2.1f;
            doorProps.fDoorsWidth = 0.3f;
            doorProps.fDoorCoordinateXinBlock = 0.6f;
            DoorBlocks.Add(doorProps);

            doorProps = new DoorProperties();
            doorProps.sBuildingSide = "Back";
            doorProps.iBayNumber = 1;
            doorProps.sDoorType = "Personnel Door";
            doorProps.fDoorsHeight = 2.2f;
            doorProps.fDoorsWidth = 0.7f;
            doorProps.fDoorCoordinateXinBlock = 0.6f;
            DoorBlocks.Add(doorProps);

            doorProps = new DoorProperties();
            doorProps.sBuildingSide = "Left";
            doorProps.iBayNumber = 3;
            doorProps.sDoorType = "Roller Door";
            doorProps.fDoorsHeight = 2.4f;
            doorProps.fDoorsWidth = 1.7f;
            doorProps.fDoorCoordinateXinBlock = 1.2f;
            DoorBlocks.Add(doorProps);

            doorProps = new DoorProperties();
            doorProps.sBuildingSide = "Right";
            doorProps.iBayNumber = 1;
            doorProps.sDoorType = "Roller Door";
            doorProps.fDoorsHeight = 2.6f;
            doorProps.fDoorsWidth = 2.0f;
            doorProps.fDoorCoordinateXinBlock = 0.6f;
            DoorBlocks.Add(doorProps);
            
            return DoorBlocks;
        }

        public static ObservableCollection<WindowProperties> GetDefaultWindowsProperties()
        {
            ObservableCollection<WindowProperties> WindowBlocks = new ObservableCollection<WindowProperties>();
            WindowProperties windowProps = new WindowProperties();
            windowProps.sBuildingSide = "Back";
            windowProps.iBayNumber = 5;
            windowProps.fWindowsHeight = 0.6f;
            windowProps.fWindowsWidth = 0.6f;
            windowProps.fWindowCoordinateXinBay = 0.3f;
            windowProps.fWindowCoordinateZinBay = 0.8f;
            windowProps.iNumberOfWindowColumns = 2;
            WindowBlocks.Add(windowProps);

            windowProps = new WindowProperties();
            windowProps.sBuildingSide = "Right";
            windowProps.iBayNumber = 3;
            windowProps.fWindowsHeight = 0.7f;
            windowProps.fWindowsWidth = 2.2f;
            windowProps.fWindowCoordinateXinBay = 0.6f;
            windowProps.fWindowCoordinateZinBay = 1.5f;
            windowProps.iNumberOfWindowColumns = 3;
            WindowBlocks.Add(windowProps);

            return WindowBlocks;
        }




    }
}
