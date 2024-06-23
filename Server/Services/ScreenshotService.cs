using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DominosDriverHustleComp.Server.Services
{
    public class ScreenshotService
    {
        private readonly ISendGridClient _emailClient;
        private readonly ILogger<ScreenshotService> _logger;

        public ScreenshotService(ISendGridClient emailClient, ILogger<ScreenshotService> logger)
        {
            _emailClient = emailClient;
            _logger = logger;
        }

        public async Task ScreenshotReport(DateTime weekEnding, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Screenshot requested for {weekEnding}", weekEnding.ToString("d"));

            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox"); // this is a hack to work around chrome running as root. https://stackoverflow.com/a/70468050
            options.AddArgument("--headless");

            var service = ChromeDriverService.CreateDefaultService("./chromedriver");
            var driver = new ChromeDriver(service, options);

            driver.Navigate().GoToUrl("http://localhost:8080/report/" + weekEnding.ToString("s"));

            _logger.LogInformation("Waiting for element...");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(d => {
                try
                {
                    return d.FindElement(By.XPath("//*[@id=\"app\"]/div/main/article/div/table/tbody/tr[1]")).Displayed;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });

            // Set window height to table height
            var tblElement = driver.FindElement(By.TagName("table"));
            var tblHeight = (long)driver.ExecuteScript("return arguments[0].offsetHeight;", tblElement);
            var currentWindowSize = driver.Manage().Window.Size;

            // if we dont divide by 2, only the `tbody` is stretched to take up the full height of the fullscreen
            // meaning each of the driver rows is really thicc.
            // not entirely sure where the math for this comes from but dividing by 2 fixes it
            driver.Manage().Window.Size = new System.Drawing.Size(currentWindowSize.Width, (int)tblHeight / 2);
            _logger.LogInformation("Setting window height to {height}", tblHeight);

            // We can not automatically request fullscreen because of security reasons.
            // Instead, create a link that will enter fullscreen and click it.
            driver.ExecuteScript("""
                let a = document.createElement("a");
                let linkText = document.createTextNode("Fullscreen");
                a.id = "fullscreenHack";
                a.appendChild(linkText);
                a.title = "Fullscreen";
                a.href = "#";
                a.onclick = function()
                {
                    document.querySelector("table").requestFullscreen();
                    return false;
                }
                document.querySelector("article").appendChild(a);
                """);

            // Actually click the link
            var element = driver.FindElement(By.Id("fullscreenHack"));
            Actions actions = new(driver);
            actions.MoveToElement(element).Click().Perform();

            var screenshot = driver.GetScreenshot();
            driver.Quit();

            var storeEmail = Environment.GetEnvironmentVariable("STORE_EMAIL");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(storeEmail, $"Driver Hustle Comp"),
                Subject = $"Driver Hustle Comp Report WE {weekEnding:d}",
                PlainTextContent = $"Driver Hustle Competition Report for Week Ending {weekEnding:d}",
                HtmlContent = $"Driver Hustle Competition Report for week ending {weekEnding:d}<br /><img src=\"cid:IMAGE01\" />",
                Attachments = [
                    new Attachment
                    {
                        ContentId = "IMAGE01",
                        Content = screenshot.AsBase64EncodedString,
                        Type = "image/png",
                        Filename = $"{weekEnding:d}.png",
                        Disposition = "inline"
                    }
                ]
            };
            msg.AddTo(storeEmail);

            var response = await _emailClient.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Screenshot email sent");
            }
            else
            {
                var message = await response.Body.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to send email ({code}): {message}", response.StatusCode, message);
            }
        }
    }
}
