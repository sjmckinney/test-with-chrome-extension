using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace InstallChromeExt
{
    public class Tests
    {
        public IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            var service = ChromeDriverService.CreateDefaultService($"{Path.GetFullPath(@"./")}_drivers");
            ChromeOptions options = new ChromeOptions();
            // Running in incognito mode will cause extensions to run as disabled
            //options.AddArguments("--incognito");

            // Seemingly adding extension as part of options will cause a new window to open
            // with information about risks of reinstalling extension
            // Additional window to be dealt with in automation
            options.AddExtensions(AuthenticatorSetup.AuthenticatorExtensionFileName);
            //service.LogPath = "./chromedriver.log";
            //service.EnableVerboseLogging = true;
            _driver = new ChromeDriver(service, options);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        public void Test1()
        {
            _driver.Url = "https://www.34protons.co.uk/demo_2_0/login.php";
            var username = _driver.FindElement(By.Id("username"));
            username.SendKeys("user");
            var password = _driver.FindElement(By.Id("password"));
            password.SendKeys("123");
            GetAuthenticatorKey getAuthenticatorKey = new GetAuthenticatorKey(_driver);
            string AuthKey = getAuthenticatorKey.AuthKey;
            var code = _driver.FindElement(By.Id("code"));
            code.SendKeys(AuthKey);
            Assert.That(AuthKey, Does.Match("\\d{6}"));
        }
    }
}