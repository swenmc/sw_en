namespace AutoTests
{
    #region using

    using NUnit.Framework;

    using OpenQA.Selenium;

    #endregion

    [TestFixture]
    public class TC : BaseForMainWindowTest
    {
        #region Public Methods and Operators

        [Test]
        public void ClickButtonGenerateModel()
        {
            var btn = this.MainWindow.FindElement(By.Id("ButtonGenerateModel"));
            btn.Click();

            System.Threading.Thread.Sleep(10000);
            Assert.DoesNotThrow(btn.Click, "Generate model exception");
        }

        [Test]
        public void ClickButtonCalculate()
        {
            var btn = this.MainWindow.FindElement(By.Id("ButtonCalculateForces"));
            btn.Click();

            System.Threading.Thread.Sleep(15000);
            Assert.IsTrue(btn.Enabled);
        }

        //[Test]
        public void ChangeToMonopitchRoof()
        {
            var list = this.MainWindow.FindElement(By.Id("Combobox_KitsetType"));
            
            //tu to chcipne,lebo to nevie najst element
            var listItem1 = list.FindElement(By.Name("Monopitch Roof"));   
            this.Driver.ExecuteScript("input: ctrl_click", listItem1);
            
            Assert.IsTrue(listItem1.Selected);

            // LeftClick on Text "Monopitch Roof" at (33,1)
            //Console.WriteLine("LeftClick on Text \"Monopitch Roof\" at (33,1)");
            //string xpath_LeftClickTextMonopitchR_33_1 = "/ListItem[@ClassName=\"ListBoxItem\"][@Name=\"Monopitch Roof\"]/Text[@ClassName=\"TextBlock\"][@Name=\"Monopitch Roof\"]";
            //var winElem_LeftClickTextMonopitchR_33_1 = this.MainWindow.FindElement(By.XPath(xpath_LeftClickTextMonopitchR_33_1));
            //if (winElem_LeftClickTextMonopitchR_33_1 != null)
            //{
            //    winElem_LeftClickTextMonopitchR_33_1.Click();
            //}
            //else
            //{
            //    System.Diagnostics.Trace.WriteLine($"Failed to find element using xpath: {xpath_LeftClickTextMonopitchR_33_1}");
            //    return;
            //}


            




        }

        //[Test]
        public void ClickByElementBoundingRecatngleCenter()
        {
            var list = this.MainWindow.FindElement(By.Id("TextListBox"));

            var listItem1 = list.FindElement(By.Name("March"));

            this.Driver.ExecuteScript("input: brc_click", listItem1);

            Assert.IsTrue(listItem1.Selected);
        }

        #endregion
    }
}
