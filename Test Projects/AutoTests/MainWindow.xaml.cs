using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoTests
{
    public class DesktopSession
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723/";
        WindowsDriver<WindowsElement> desktopSession;

        public DesktopSession()
        {
            //DesiredCapabilities appCapabilities = new DesiredCapabilities();
            //appCapabilities.SetCapability("app", "Root");
            //appCapabilities.SetCapability("deviceName", "WindowsPC");
            ////desktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);

            AppiumOptions opts = new AppiumOptions();
            opts.AddAdditionalCapability("app", "Root");
            opts.AddAdditionalCapability("deviceName", "WindowsPC");
            desktopSession = new WindowsDriver<WindowsElement>(opts);
        }

        ~DesktopSession()
        {
            desktopSession.Quit();
        }

        public WindowsDriver<WindowsElement> DesktopSessionElement
        {
            get { return desktopSession; }
        }

        public WindowsElement FindElementByAbsoluteXPath(string xPath, int nTryCount = 10)
        {
            WindowsElement uiTarget = null;

            while (nTryCount-- > 0)
            {
                try
                {
                    uiTarget = desktopSession.FindElementByXPath(xPath);
                }
                catch
                {
                }

                if (uiTarget != null)
                {
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }

            return uiTarget;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            // Launch Notepad
            //DesiredCapabilities appCapabilities = new DesiredCapabilities();
            //appCapabilities.SetCapability("app", @"C:\Windows\System32\notepad.exe");
            //NotepadSession = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
            //// Control the AlarmClock app
            //NotepadSession.FindElementByClassName("Edit").SendKeys("This is some text");
                        
            DesktopSession desktopSession = new DesktopSession();
            System.Threading.Thread.Sleep(2000);


            // LeftClick on Window "" at (210,182)
            Console.WriteLine("LeftClick on Window \"\" at (210,182)");
            string xpath_LeftClickWindow_210_182 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_210_182 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_210_182);
            if (winElem_LeftClickWindow_210_182 != null)
            {
                winElem_LeftClickWindow_210_182.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_210_182}");
                return;
            }


            // KeyboardInput VirtualKeys="Keys.NumberPad1 + Keys.NumberPad0 + Keys.NumberPad1 + Keys.NumberPad0" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"Keys.NumberPad1 + Keys.NumberPad0 + Keys.NumberPad1 + Keys.NumberPad0\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickWindow_210_182.SendKeys(Keys.NumberPad1 + Keys.NumberPad0 + Keys.NumberPad1 + Keys.NumberPad0);


            // LeftClick on Window "" at (145,196)
            Console.WriteLine("LeftClick on Window \"\" at (145,196)");
            string xpath_LeftClickWindow_145_196 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_145_196 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_145_196);
            if (winElem_LeftClickWindow_145_196 != null)
            {
                winElem_LeftClickWindow_145_196.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_145_196}");
                return;
            }


            // KeyboardInput VirtualKeys="Keys.NumberPad2 + Keys.NumberPad0 + Keys.NumberPad2 + Keys.NumberPad0" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"Keys.NumberPad2 + Keys.NumberPad0 + Keys.NumberPad2 + Keys.NumberPad0\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickWindow_145_196.SendKeys(Keys.NumberPad2 + Keys.NumberPad0 + Keys.NumberPad2 + Keys.NumberPad0);


            // LeftClick on Window "" at (152,229)
            Console.WriteLine("LeftClick on Window \"\" at (152,229)");
            string xpath_LeftClickWindow_152_229 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_152_229 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_152_229);
            if (winElem_LeftClickWindow_152_229 != null)
            {
                winElem_LeftClickWindow_152_229.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_152_229}");
                return;
            }


            // LeftClick on Window "" at (219,275)
            Console.WriteLine("LeftClick on Window \"\" at (219,275)");
            string xpath_LeftClickWindow_219_275 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_219_275 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_219_275);
            if (winElem_LeftClickWindow_219_275 != null)
            {
                winElem_LeftClickWindow_219_275.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_219_275}");
                return;
            }


            // KeyboardInput VirtualKeys="Keys.NumberPad5 + Keys.NumberPad5" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"Keys.NumberPad5 + Keys.NumberPad5\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickWindow_219_275.SendKeys(Keys.NumberPad5 + Keys.NumberPad5);


            // LeftClick on Window "" at (272,575)
            Console.WriteLine("LeftClick on Window \"\" at (272,575)");
            string xpath_LeftClickWindow_272_575 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_272_575 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_272_575);
            if (winElem_LeftClickWindow_272_575 != null)
            {
                winElem_LeftClickWindow_272_575.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_272_575}");
                return;
            }


            // LeftClick on Window "" at (518,667)
            Console.WriteLine("LeftClick on Window \"\" at (518,667)");
            string xpath_LeftClickWindow_518_667 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_518_667 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_518_667);
            if (winElem_LeftClickWindow_518_667 != null)
            {
                winElem_LeftClickWindow_518_667.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_518_667}");
                return;
            }


            // LeftClick on Button "OK" at (52,10)
            Console.WriteLine("LeftClick on Button \"OK\" at (52,10)");
            string xpath_LeftClickButtonOK_52_10 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Pane[@ClassName=\"SplashScreen\"][@Name=\"Please wait while the application opens\"]/Window[@ClassName=\"#32770\"]/Button[@ClassName=\"Button\"][@Name=\"OK\"]";
            var winElem_LeftClickButtonOK_52_10 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonOK_52_10);
            if (winElem_LeftClickButtonOK_52_10 != null)
            {
                winElem_LeftClickButtonOK_52_10.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonOK_52_10}");
                return;
            }


            // LeftClick on Window "" at (178,1126)
            Console.WriteLine("LeftClick on Window \"\" at (178,1126)");
            string xpath_LeftClickWindow_178_1126 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_178_1126 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_178_1126);
            if (winElem_LeftClickWindow_178_1126 != null)
            {
                winElem_LeftClickWindow_178_1126.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_178_1126}");
                return;
            }


            // LeftClick on Window "" at (650,432)
            Console.WriteLine("LeftClick on Window \"\" at (650,432)");
            string xpath_LeftClickWindow_650_432 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_650_432 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_650_432);
            if (winElem_LeftClickWindow_650_432 != null)
            {
                winElem_LeftClickWindow_650_432.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_650_432}");
                return;
            }


            // MouseWheel on Window "" at (350,113) drag (104,0)
            Console.WriteLine("MouseWheel on Window \"\" at (350,113) drag (104,0)");
            string xpath_MouseWheelWindow_350_113 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_MouseWheelWindow_350_113 = desktopSession.FindElementByAbsoluteXPath(xpath_MouseWheelWindow_350_113);
            if (winElem_MouseWheelWindow_350_113 != null)
            {
                //TODO: Wheel at (618,386) on winElem_MouseWheelWindow_350_113, Count:104, Total Amount:0
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_MouseWheelWindow_350_113}");
                return;
            }


            // LeftClick on Button "Close" at (12,8)
            Console.WriteLine("LeftClick on Button \"Close\" at (12,8)");
            string xpath_LeftClickButtonClose_12_8 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"][@Name=\"Design Summary\"]/TitleBar[@AutomationId=\"TitleBar\"]/Button[@Name=\"Close\"][@AutomationId=\"Close\"]";
            var winElem_LeftClickButtonClose_12_8 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonClose_12_8);
            if (winElem_LeftClickButtonClose_12_8 != null)
            {
                winElem_LeftClickButtonClose_12_8.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonClose_12_8}");
                return;
            }


            // LeftClick on Window "" at (389,319)
            Console.WriteLine("LeftClick on Window \"\" at (389,319)");
            string xpath_LeftClickWindow_389_319 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_389_319 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_389_319);
            if (winElem_LeftClickWindow_389_319 != null)
            {
                winElem_LeftClickWindow_389_319.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_389_319}");
                return;
            }


            // LeftClick on Text "Monopitch Roof" at (74,5)
            Console.WriteLine("LeftClick on Text \"Monopitch Roof\" at (74,5)");
            string xpath_LeftClickTextMonopitchR_74_5 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"][@Name=\"FS - Engenius\"]/Window[@ClassName=\"Popup\"]/ListItem[@ClassName=\"ListBoxItem\"][@Name=\"Monopitch Roof\"]/Text[@ClassName=\"TextBlock\"][@Name=\"Monopitch Roof\"]";
            var winElem_LeftClickTextMonopitchR_74_5 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickTextMonopitchR_74_5);
            if (winElem_LeftClickTextMonopitchR_74_5 != null)
            {
                winElem_LeftClickTextMonopitchR_74_5.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickTextMonopitchR_74_5}");
                return;
            }


            // LeftClick on Window "" at (97,40)
            Console.WriteLine("LeftClick on Window \"\" at (97,40)");
            string xpath_LeftClickWindow_97_40 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_97_40 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_97_40);
            if (winElem_LeftClickWindow_97_40 != null)
            {
                winElem_LeftClickWindow_97_40.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_97_40}");
                return;
            }


            // LeftClick on Window "" at (184,32)
            Console.WriteLine("LeftClick on Window \"\" at (184,32)");
            string xpath_LeftClickWindow_184_32 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_184_32 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_184_32);
            if (winElem_LeftClickWindow_184_32 != null)
            {
                winElem_LeftClickWindow_184_32.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_184_32}");
                return;
            }


            // LeftClick on Window "" at (245,36)
            Console.WriteLine("LeftClick on Window \"\" at (245,36)");
            string xpath_LeftClickWindow_245_36 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_245_36 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_245_36);
            if (winElem_LeftClickWindow_245_36 != null)
            {
                winElem_LeftClickWindow_245_36.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_245_36}");
                return;
            }


            // LeftClick on Window "" at (298,36)
            Console.WriteLine("LeftClick on Window \"\" at (298,36)");
            string xpath_LeftClickWindow_298_36 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_298_36 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_298_36);
            if (winElem_LeftClickWindow_298_36 != null)
            {
                winElem_LeftClickWindow_298_36.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_298_36}");
                return;
            }


            // LeftClick on Window "" at (377,32)
            Console.WriteLine("LeftClick on Window \"\" at (377,32)");
            string xpath_LeftClickWindow_377_32 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_377_32 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_377_32);
            if (winElem_LeftClickWindow_377_32 != null)
            {
                winElem_LeftClickWindow_377_32.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_377_32}");
                return;
            }


            // LeftClick on Window "" at (449,40)
            Console.WriteLine("LeftClick on Window \"\" at (449,40)");
            string xpath_LeftClickWindow_449_40 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_449_40 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_449_40);
            if (winElem_LeftClickWindow_449_40 != null)
            {
                winElem_LeftClickWindow_449_40.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_449_40}");
                return;
            }


            // LeftClick on Window "" at (514,38)
            Console.WriteLine("LeftClick on Window \"\" at (514,38)");
            string xpath_LeftClickWindow_514_38 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_514_38 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_514_38);
            if (winElem_LeftClickWindow_514_38 != null)
            {
                winElem_LeftClickWindow_514_38.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_514_38}");
                return;
            }


            // LeftClick on Window "" at (581,18)
            Console.WriteLine("LeftClick on Window \"\" at (581,18)");
            string xpath_LeftClickWindow_581_18 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_581_18 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_581_18);
            if (winElem_LeftClickWindow_581_18 != null)
            {
                winElem_LeftClickWindow_581_18.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_581_18}");
                return;
            }


            // LeftClick on Window "" at (494,38)
            Console.WriteLine("LeftClick on Window \"\" at (494,38)");
            string xpath_LeftClickWindow_494_38 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_494_38 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_494_38);
            if (winElem_LeftClickWindow_494_38 != null)
            {
                winElem_LeftClickWindow_494_38.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_494_38}");
                return;
            }


            // LeftClick on Window "" at (40,19)
            Console.WriteLine("LeftClick on Window \"\" at (40,19)");
            string xpath_LeftClickWindow_40_19 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_40_19 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_40_19);
            if (winElem_LeftClickWindow_40_19 != null)
            {
                winElem_LeftClickWindow_40_19.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_40_19}");
                return;
            }


            // LeftClick on Window "" at (209,107)
            Console.WriteLine("LeftClick on Window \"\" at (209,107)");
            string xpath_LeftClickWindow_209_107 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_209_107 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_209_107);
            if (winElem_LeftClickWindow_209_107 != null)
            {
                winElem_LeftClickWindow_209_107.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_209_107}");
                return;
            }


            // LeftClick on Text "Monopitch Roof" at (212,150)
            Console.WriteLine("LeftClick on Text \"Monopitch Roof\" at (212,150)");
            string xpath_LeftClickTextMonopitchR_212_150 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"][@Name=\"FS - Engenius\"]/Window[@ClassName=\"Popup\"]/ListItem[@ClassName=\"ListBoxItem\"][@Name=\"Monopitch Roof\"]/Text[@ClassName=\"TextBlock\"][@Name=\"Monopitch Roof\"]";
            var winElem_LeftClickTextMonopitchR_212_150 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickTextMonopitchR_212_150);
            if (winElem_LeftClickTextMonopitchR_212_150 != null)
            {
                winElem_LeftClickTextMonopitchR_212_150.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickTextMonopitchR_212_150}");
                return;
            }


            // KeyboardInput VirtualKeys="Keys.NumberPad1 + Keys.NumberPad0 + Keys.NumberPad1 + Keys.NumberPad0" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"Keys.NumberPad1 + Keys.NumberPad0 + Keys.NumberPad1 + Keys.NumberPad0\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickTextMonopitchR_212_150.SendKeys(Keys.NumberPad1 + Keys.NumberPad0 + Keys.NumberPad1 + Keys.NumberPad0);


            // LeftClick on Window "" at (135,189)
            Console.WriteLine("LeftClick on Window \"\" at (135,189)");
            string xpath_LeftClickWindow_135_189 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_135_189 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_135_189);
            if (winElem_LeftClickWindow_135_189 != null)
            {
                winElem_LeftClickWindow_135_189.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_135_189}");
                return;
            }


            // KeyboardInput VirtualKeys="Keys.NumberPad4 + Keys.NumberPad4" CapsLock=False NumLock=True ScrollLock=False
            Console.WriteLine("KeyboardInput VirtualKeys=\"Keys.NumberPad4 + Keys.NumberPad4\" CapsLock=False NumLock=True ScrollLock=False");
            System.Threading.Thread.Sleep(100);
            winElem_LeftClickWindow_135_189.SendKeys(Keys.NumberPad4 + Keys.NumberPad4);


            // LeftClick on Window "" at (485,688)
            Console.WriteLine("LeftClick on Window \"\" at (485,688)");
            string xpath_LeftClickWindow_485_688 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_485_688 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_485_688);
            if (winElem_LeftClickWindow_485_688 != null)
            {
                winElem_LeftClickWindow_485_688.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_485_688}");
                return;
            }


            // LeftClick on Button "OK" at (41,10)
            Console.WriteLine("LeftClick on Button \"OK\" at (41,10)");
            string xpath_LeftClickButtonOK_41_10 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Pane[@ClassName=\"SplashScreen\"][@Name=\"Please wait while the application opens\"]/Window[@ClassName=\"#32770\"]/Button[@ClassName=\"Button\"][@Name=\"OK\"]";
            var winElem_LeftClickButtonOK_41_10 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonOK_41_10);
            if (winElem_LeftClickButtonOK_41_10 != null)
            {
                winElem_LeftClickButtonOK_41_10.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonOK_41_10}");
                return;
            }


            // LeftClick on Window "" at (115,44)
            Console.WriteLine("LeftClick on Window \"\" at (115,44)");
            string xpath_LeftClickWindow_115_44 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_115_44 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_115_44);
            if (winElem_LeftClickWindow_115_44 != null)
            {
                winElem_LeftClickWindow_115_44.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_115_44}");
                return;
            }


            // LeftClick on Window "" at (162,39)
            Console.WriteLine("LeftClick on Window \"\" at (162,39)");
            string xpath_LeftClickWindow_162_39 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_162_39 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_162_39);
            if (winElem_LeftClickWindow_162_39 != null)
            {
                winElem_LeftClickWindow_162_39.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_162_39}");
                return;
            }


            // LeftClick on Window "" at (306,34)
            Console.WriteLine("LeftClick on Window \"\" at (306,34)");
            string xpath_LeftClickWindow_306_34 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_306_34 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_306_34);
            if (winElem_LeftClickWindow_306_34 != null)
            {
                winElem_LeftClickWindow_306_34.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_306_34}");
                return;
            }


            // LeftClick on Window "" at (356,40)
            Console.WriteLine("LeftClick on Window \"\" at (356,40)");
            string xpath_LeftClickWindow_356_40 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_356_40 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_356_40);
            if (winElem_LeftClickWindow_356_40 != null)
            {
                winElem_LeftClickWindow_356_40.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_356_40}");
                return;
            }


            // LeftClick on Window "" at (435,39)
            Console.WriteLine("LeftClick on Window \"\" at (435,39)");
            string xpath_LeftClickWindow_435_39 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_435_39 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_435_39);
            if (winElem_LeftClickWindow_435_39 != null)
            {
                winElem_LeftClickWindow_435_39.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_435_39}");
                return;
            }


            // LeftClick on Window "" at (499,31)
            Console.WriteLine("LeftClick on Window \"\" at (499,31)");
            string xpath_LeftClickWindow_499_31 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_499_31 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_499_31);
            if (winElem_LeftClickWindow_499_31 != null)
            {
                winElem_LeftClickWindow_499_31.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_499_31}");
                return;
            }


            // LeftClick on Window "" at (568,23)
            Console.WriteLine("LeftClick on Window \"\" at (568,23)");
            string xpath_LeftClickWindow_568_23 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_568_23 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_568_23);
            if (winElem_LeftClickWindow_568_23 != null)
            {
                winElem_LeftClickWindow_568_23.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_568_23}");
                return;
            }


            // LeftClick on Window "" at (503,37)
            Console.WriteLine("LeftClick on Window \"\" at (503,37)");
            string xpath_LeftClickWindow_503_37 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_503_37 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_503_37);
            if (winElem_LeftClickWindow_503_37 != null)
            {
                winElem_LeftClickWindow_503_37.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_503_37}");
                return;
            }


            // LeftClick on Window "" at (670,424)
            Console.WriteLine("LeftClick on Window \"\" at (670,424)");
            string xpath_LeftClickWindow_670_424 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_670_424 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_670_424);
            if (winElem_LeftClickWindow_670_424 != null)
            {
                winElem_LeftClickWindow_670_424.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_670_424}");
                return;
            }


            // MouseWheel on Window "" at (437,191) drag (17,-2040)
            Console.WriteLine("MouseWheel on Window \"\" at (437,191) drag (17,-2040)");
            string xpath_MouseWheelWindow_437_191 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_MouseWheelWindow_437_191 = desktopSession.FindElementByAbsoluteXPath(xpath_MouseWheelWindow_437_191);
            if (winElem_MouseWheelWindow_437_191 != null)
            {
                //TODO: Wheel at (618,386) on winElem_MouseWheelWindow_437_191, Count:17, Total Amount:-2040
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_MouseWheelWindow_437_191}");
                return;
            }


            // LeftClick on Window "" at (999,651)
            Console.WriteLine("LeftClick on Window \"\" at (999,651)");
            string xpath_LeftClickWindow_999_651 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@ClassName=\"Window\"]";
            var winElem_LeftClickWindow_999_651 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickWindow_999_651);
            if (winElem_LeftClickWindow_999_651 != null)
            {
                winElem_LeftClickWindow_999_651.Click();
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickWindow_999_651}");
                return;
            }


            // MouseWheel on Button "WinAppDriverUIRecorder.exe - 1 running window" at (29,21) drag (4,480)
            Console.WriteLine("MouseWheel on Button \"WinAppDriverUIRecorder.exe - 1 running window\" at (29,21) drag (4,480)");
            string xpath_MouseWheelButtonWinAppDriv_29_21 = "/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Pane[@ClassName=\"Shell_TrayWnd\"][@Name=\"Taskbar\"]/ToolBar[@ClassName=\"MSTaskListWClass\"][@Name=\"Running applications\"]/Button[@Name=\"WinAppDriverUIRecorder.exe - 1 running window\"][starts-with(@AutomationId,\"C:\\Users\\Ondrej Pažin\\Desktop\\WinAppDriverUIRecorder\\WinAppDrive\")]";
            var winElem_MouseWheelButtonWinAppDriv_29_21 = desktopSession.FindElementByAbsoluteXPath(xpath_MouseWheelButtonWinAppDriv_29_21);
            if (winElem_MouseWheelButtonWinAppDriv_29_21 != null)
            {
                //TODO: Wheel at (343,1160) on winElem_MouseWheelButtonWinAppDriv_29_21, Count:4, Total Amount:480
            }
            else
            {
                Console.WriteLine($"Failed to find element using xpath: {xpath_MouseWheelButtonWinAppDriv_29_21}");
                return;
            }



        }
    }
}
