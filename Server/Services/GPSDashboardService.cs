using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using OpenQA.Selenium.Chrome;

namespace DominosDriverHustleComp.Server.Services
{
    /// <summary>
    /// Service responsible for launching GPS Dashboard, logging in, and
    /// sending the auth token back to this server
    /// </summary>
    public class GPSDashboardService : BackgroundService
    {
        private readonly IServer _server;
        private ChromeDriver? _driver;
        public GPSDashboardService(IServer server)
        {
            _server = server;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await PreStartBrowser(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
            }

            _driver?.Close();
        }

        private async Task PreStartBrowser(CancellationToken cancellationToken)
        {
            var addresses = _server.Features.GetRequiredFeature<IServerAddressesFeature>().Addresses;

            while (!cancellationToken.IsCancellationRequested && addresses.Count == 0)
            {
                await Task.Yield();
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            var address = addresses.FirstOrDefault(string.Empty);
            var fixedAddress = address.Replace("*", "localhost");
            StartBrowser(fixedAddress);
        }

        private void StartBrowser(string address)
        {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox"); // this is a hack to work around chrome running as root. https://stackoverflow.com/a/70468050
            options.AddArgument("--headless");
            options.AddArgument("--remote-debugging-port=9222"); // Default debug port, just make sure as its bound from the docker container
            options.AddArgument("--remote-debugging-address=0.0.0.0");
            options.AddArgument("--auto-open-devtools-for-tabs"); // Always do this for easier debugging

            var service = ChromeDriverService.CreateDefaultService();
            service.AllowedIPAddresses = " "; // allow anyone to debug
            _driver = new ChromeDriver(service, options);

            ///@TODO bail if these env-vars arent set as we cant login or get orders from the portal
            var envGPSStoreNumber = Environment.GetEnvironmentVariable("GPS_STORENUMBER");
            var envGPSStoreEmail = Environment.GetEnvironmentVariable("GPS_STOREEMAIL");
            var envGPSStorePass = Environment.GetEnvironmentVariable("GPS_STOREPASS");
            var source = $"const gpsStoreNumber = \"{envGPSStoreNumber}\";\nconst gpsEmail = \"{envGPSStoreEmail}\";\nconst gpsPass = \"{envGPSStorePass}\";\nconst server = \"{address}\"\n";
            source += File.ReadAllText("GPSDashboard.js");

            var cmdparams = new Dictionary<string, object>
            {
                { "source", source }
            };
            _driver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", cmdparams);

            _driver.Navigate().GoToUrl("https://gps-dashboard.dominos.com.au");
        }
    }
}
