using OpenQA.Selenium.Chrome;

namespace DominosDriverHustleComp.Server.Services
{
    public class OverspeedsService
    {
        private ChromeDriver? _driver = null;
        public void FetchOverspeeds()
        {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox"); // this is a hack to work around chrome running as root. https://stackoverflow.com/a/70468050
            options.AddArgument("--headless");
            options.AddArgument("--remote-debugging-port=9223"); // Default debug port, just make sure as its bound from the docker container
            options.AddArgument("--remote-debugging-address=0.0.0.0");
            options.AddArgument("--auto-open-devtools-for-tabs"); // Always do this for easier debugging

            var service = ChromeDriverService.CreateDefaultService("./chromedriver");
            _driver = new ChromeDriver(service, options);

            var envDrivosityEmail = Environment.GetEnvironmentVariable("DRIVOSITY_EMAIL");
            var envDrivosityPass = Environment.GetEnvironmentVariable("DRIVOSITY_PASS");
            var source = $"const dipEmail = \"{envDrivosityEmail}\";\nconst dipPass = \"{envDrivosityPass}\";\n";
            source += File.ReadAllText("Drivosity.js");

            var cmdparams = new Dictionary<string, object>
            {
                { "source", source }
            };
            _driver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", cmdparams);

            _driver.Navigate().GoToUrl("https://dip.drivosity.com/live");

            ///@TODO quit the driver once overspeeds have been received.
            /// using WebDriverWait.Until is not feasible since its a blocking/busy wait
            /// and will prevent the server from starting.
        }

        public void StopBrowser()
        {
            _driver?.Quit();
            _driver = null;
        }
    }
}
