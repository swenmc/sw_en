using BaseClasses;
using BaseClasses.Helpers;
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
                doorProps.iBayNumber = 3;
                doorProps.sDoorType = "Roller Door";
                doorProps.fDoorsHeight = 2.1f;
                doorProps.fDoorsWidth = 1.2f;
                doorProps.fDoorCoordinateXinBlock = 0.3f;
                doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                DoorBlocks.Add(doorProps);

                //doorProps = new DoorProperties();
                //doorProps.sBuildingSide = "Front";
                //doorProps.iBayNumber = 2;
                //doorProps.sDoorType = "Roller Door";
                //doorProps.fDoorsHeight = 2.1f;
                //doorProps.fDoorsWidth = 1.2f;
                //doorProps.fDoorCoordinateXinBlock = 0.3f;
                //doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                //DoorBlocks.Add(doorProps);
                //
                //doorProps = new DoorProperties();
                //doorProps.sBuildingSide = "Left";
                //doorProps.iBayNumber = 1;
                //doorProps.sDoorType = "Roller Door";
                //doorProps.fDoorsHeight = 2.1f;
                //doorProps.fDoorsWidth = 2.2f;
                //doorProps.fDoorCoordinateXinBlock = 0.5f;
                //doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                //DoorBlocks.Add(doorProps);
                //
                //doorProps = new DoorProperties();
                //doorProps.sBuildingSide = "Right";
                //doorProps.iBayNumber = 3;
                //doorProps.sDoorType = "Roller Door";
                //doorProps.fDoorsHeight = 1.1f;
                //doorProps.fDoorsWidth = 1.8f;
                //doorProps.fDoorCoordinateXinBlock = 0.6f;
                //doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                //DoorBlocks.Add(doorProps);
                //
                //doorProps = new DoorProperties();
                //doorProps.sBuildingSide = "Back";
                //doorProps.iBayNumber = 2;
                //doorProps.sDoorType = "Roller Door";
                //doorProps.fDoorsHeight = 1.1f;
                //doorProps.fDoorsWidth = 0.9f;
                //doorProps.fDoorCoordinateXinBlock = 0.4f;
                //doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                //DoorBlocks.Add(doorProps);
                //
                //doorProps = new DoorProperties();
                //doorProps.sBuildingSide = "Back";
                //doorProps.iBayNumber = 3;
                //doorProps.sDoorType = "Roller Door";
                //doorProps.fDoorsHeight = 2.1f;
                //doorProps.fDoorsWidth = 0.8f;
                //doorProps.fDoorCoordinateXinBlock = 0.5f;
                //doorProps.CoatingColor = doorProps.CoatingColors.FirstOrDefault();
                //DoorBlocks.Add(doorProps);
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


        public static bool IsEnoughtPlaceForDoors(int bayNum, float width, ObservableCollection<DoorProperties> doors)
        {            
            foreach (DoorProperties d in doors)
            {
                if (d.sBuildingSide != "Left" && d.sBuildingSide != "Right") continue;
                if (d.iBayNumber != bayNum) continue;
                
                if (width < d.fDoorsWidth + d.fDoorCoordinateXinBlock) return false;
            }

            return true;
        }
        public static bool IsEnoughtPlaceForWindows(int bayNum, float width, ObservableCollection<WindowProperties> windows)
        {
            foreach (WindowProperties w in windows)
            {
                if (w.sBuildingSide != "Left" && w.sBuildingSide != "Right") continue;
                if (w.iBayNumber != bayNum) continue;

                if (width < w.fWindowsWidth + w.fWindowCoordinateXinBay) return false;
            }
            
            return true;
        }


        public static double GetDoorsAndWindowsOpeningArea(CPFDViewModel vm)
        {
            List<string> ignoreBuildingSides = new List<string>();
            if (!vm.ModelHasRightWall()) ignoreBuildingSides.Add("Right");
            if (!vm.ModelHasLeftWall()) ignoreBuildingSides.Add("Left");
            if (!vm.ModelHasFrontWall()) ignoreBuildingSides.Add("Front");
            if (!vm.ModelHasBackWall()) ignoreBuildingSides.Add("Back");

            // Wall Doors and Windows Area
            double dDoorsAndWindowsOpeningArea = 0;

            if (vm._doorsAndWindowsVM.ModelHasDoor())
            {
                foreach (DoorProperties door in vm._doorsAndWindowsVM.DoorBlocksProperties)
                {
                    if (ignoreBuildingSides.Count > 0 && ignoreBuildingSides.Contains(door.sBuildingSide)) continue; // do not count
                    
                    dDoorsAndWindowsOpeningArea += door.fDoorsWidth * door.fDoorsHeight;
                }
            }

            if (vm._doorsAndWindowsVM.ModelHasWindow())
            {
                foreach (WindowProperties window in vm._doorsAndWindowsVM.WindowBlocksProperties)
                {
                    if (ignoreBuildingSides.Count > 0 && ignoreBuildingSides.Contains(window.sBuildingSide)) continue; // do not count

                    dDoorsAndWindowsOpeningArea += window.fWindowsWidth * window.fWindowsHeight;
                }
            }

            return dDoorsAndWindowsOpeningArea;
        }


        public static bool DoorsAreInCollisionWithFibreglass(CPFDViewModel vm, DoorProperties door, FibreglassProperties fp)
        {
            if (door.sBuildingSide != fp.Side) return false;

            float doorX1 = ModelHelper.GetBaysWidthUntil(door.iBayNumber - 1, vm._baysWidthOptionsVM.BayWidthList) + door.fDoorCoordinateXinBlock;
            float doorX2 = doorX1 + door.fDoorsWidth;
            float doorY = door.fDoorsHeight;

            if (doorY < fp.Y) return false; //fp above doors

            if (fp.X > doorX1 && fp.X < doorX2) return true;

            float fpX2 = fp.X + fp.CladdingWidthModular_Wall;
            if (fpX2 > doorX1 && fpX2 < doorX2) return true;
                        
            return false;
        }

        public static bool WindowIsInCollisionWithFibreglass(CPFDViewModel vm, WindowProperties window, FibreglassProperties fp)
        {
            if (window.sBuildingSide != fp.Side) return false;

            float windowX1 = ModelHelper.GetBaysWidthUntil(window.iBayNumber - 1, vm._baysWidthOptionsVM.BayWidthList) + window.fWindowCoordinateXinBay;
            float windowX2 = windowX1 + window.fWindowsWidth;

            float windowY1 = window.fWindowCoordinateZinBay;
            float windowY2 = windowY1 + window.fWindowsHeight;

            if (windowY1 > fp.Y + fp.Length) return false; // window above fibreglass
            if (windowY2 < fp.Y) return false; //window below fibreglass
            
            if (fp.X > windowX1 && fp.X < windowX2) return true;

            float fpX2 = fp.X + fp.CladdingWidthModular_Wall;
            if (fpX2 > windowX1 && fpX2 < windowX2) return true;
            
            return false;
        }




    }
}
