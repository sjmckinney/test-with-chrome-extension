# Test With Chrome Extension

A simple test that demonstrates the use of the Google Authenticator extension.

This is possible because it is possible to access the extensions functionality by opening a series of html files located with the loaded extension

## Test pre-requisites

Add the chromedriver  or chromedriver.exe to the `_drivers` directory.

The `Extensions` directory contains the `authenticator.crx` file which is some sort of compressed structure that contains the extension.

It is possible to use the [CRX Extractor/Downloader](https://chrome.google.com/webstore/detail/crx-extractordownloader/ajkhmmldknmfjnmeedkbkkojgobmljda) extension to download an extension in either `.crx` or `.zip` format. It is possible to install the extension in the browser by configuring the `options` object using the following;

```csharp
ChromeOptions options = new ChromeOptions();
options.AddExtensions(AuthenticatorSetup.AuthenticatorExtensionFileName);
IWebDriver _driver = new ChromeDriver(service, options);
```

The `authenticator.txt` file contains a single backup entry. This results in the display of a single code making it easy for the webdriver code to find the single webelement that contains the code. Multiple entries will require that the automation code differentiate between multiple identical webelements.