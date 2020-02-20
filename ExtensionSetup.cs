using System;
using System.IO;
using System.Reflection;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace InstallChromeExt
{
    public class AuthenticatorSetup
    {
        // Download extension install file (.crx) and locate in directory
		// Ensure that property 'Copy to output directory' is set to copy file
		public static readonly string ExtensionPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Extensions/");
        public static readonly string AuthenticatorKeyFileName = (ExtensionPath) + "authenticator.txt";
        public static readonly string AuthenticatorExtensionFileName = (ExtensionPath) + "authenticator.crx";
    }

    public class GetAuthenticatorKey
    {
        private readonly IWebDriver _driver;

        // Read the contents of the authenticator extensin back up file
        private readonly string _authenticatorKey = File.ReadAllText(AuthenticatorSetup.AuthenticatorKeyFileName);
        private readonly string _extensionImportUrl = "chrome-extension://bhghoamapcdpbohphigoooaddinpkbai/view/import.html";
        private readonly string _extensionUrl = "chrome-extension://bhghoamapcdpbohphigoooaddinpkbai/view/popup.html";
        private readonly string _importBtnXPathSelector = "//label[@for='import_code_radio']";
        private readonly string _importKeyTextFieldCssSelector = "textarea[spellcheck='false']";
        private readonly string _submitImportTextBackUpBtnXPathSelector = "//button[contains(text(), 'Import Text Backup')]";
        private readonly string _authKeyCssSelector = "#codes div.code";

        // field that will be set to the value
        // of the authenticator code
        public string AuthKey { get; set; }

        readonly IWebElement ImportKeyTextField;

        public GetAuthenticatorKey(IWebDriver driver)
        {
            this._driver = driver;
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Deal with additional information window
            // Need to find out what happens when un-install authenticator extension
            // and re-run text
            _driver.SwitchTo().Window(driver.WindowHandles.Last());
            // Opens authenticator URL in in additional window
            _driver.Navigate().GoToUrl(_extensionImportUrl);

            wait.Until<bool>(_driver => {
                                            try
                                            {
                                                var importBtn = _driver.FindElement(By.XPath(_importBtnXPathSelector));
                                                importBtn.Click();
                                                return true;

                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                return false;
                                            }
                                        }
                                    );

            ImportKeyTextField = _driver.FindElement(By.CssSelector(_importKeyTextFieldCssSelector));
            ImportKeyTextField.SendKeys(_authenticatorKey);

            var importBackupTextBtn = wait.Until<IWebElement>(_driver => _driver.FindElement(By.XPath(_submitImportTextBackUpBtnXPathSelector)));
            importBackupTextBtn.Click();

            var alert = wait.Until<bool>(_driver => {
                                                    try {
                                                        _driver.SwitchTo().Alert();
                                                        return true;
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine(e.Message);
                                                        return false;
                                                    }
                                                }
                                            );
            // Cancel Alert window
            _driver.SwitchTo()
            .Alert()
            .Accept();

            // Switch to first window, open blank, switch to it and open
            // authenticator URL in that window in order to extract code
            _ = _driver.SwitchTo().Window(driver.WindowHandles.First());
            _ = ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(driver.WindowHandles.Last());
            _driver.Navigate().GoToUrl(_extensionUrl);

            var _authKey = wait.Until<IWebElement>(_driver => _driver.FindElement(By.CssSelector(_authKeyCssSelector)));
            AuthKey = _authKey.Text;

            //Switch back to login.php window
            _driver.SwitchTo().Window(driver.WindowHandles.First());
        }
    }
}