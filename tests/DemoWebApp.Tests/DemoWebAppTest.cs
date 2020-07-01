
using System;
using NUnit.Framework;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DemoWebApp.Tests
{
    public class DemoWebAppTest
    {
        private IHost app;

        [OneTimeSetUp]
        public void SetUpWebApp()
        {
            app = DemoWebApp.Program.CreateHostBuilder(new string[] { }).Build();
            app.RunAsync();
        }

        [TestCase(typeof(ChromeDriver))]
        [TestCase(typeof(FirefoxDriver))]
        [TestCase(typeof(InternetExplorerDriver))] // ie requires workstation configuration: http://www.programmersought.com/article/1603471677/
        public void TestButtonClick(Type drvType)
        {
            bool clicked;
            using (var driver = (IWebDriver)Activator.CreateInstance(drvType))
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl("http://localhost:5000");
                driver.FindElement(By.Id("clickmeButton")).Click();

                clicked = wait.Until(ExpectedConditions.TextToBePresentInElement(
                    driver.FindElement(By.Id("displayHeader")), "Button clicked"));
            }
            Assert.True(clicked, "button not clicked.");
        }

        [OneTimeTearDown]
        public void TearDownWebApp()
        {
            app.StopAsync();
            app.WaitForShutdown();
        }
    }
}
