using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class ContactUsTest
{
    [Test]
    public async Task ContactUsFormTestCase()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 1000
        });

        var context = await browser.NewContextAsync();

        // Ad blocker
        await context.RouteAsync("**/*", async route =>
        {
            var url = route.Request.Url;

            if (
                url.Contains("doubleclick") ||
                url.Contains("googlesyndication") ||
                url.Contains("googleads") ||
                url.Contains("adservice") ||
                url.Contains("adsystem") ||
                url.Contains("adnxs")
            )
            {
                await route.AbortAsync();
            }
            else
            {
                await route.ContinueAsync();
            }
        });

        var page = await context.NewPageAsync();

        await page.GotoAsync("https://automationexercise.com/");

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Contact us" })
            .ClickAsync();

        await page
            .Locator(".contact-form")
            .GetByText("Get In Touch")
            .WaitForAsync();

        await page.Locator("input[data-qa='name']").FillAsync("Test User");
        await page.Locator("input[data-qa='email']").FillAsync("test@gmail.com");
        await page.Locator("input[data-qa='subject']").FillAsync("Test Subject");
        await page.Locator("textarea[data-qa='message']").FillAsync("This is a test message.");

        await page
            .Locator("input[name='upload_file']")
            .SetInputFilesAsync(@"C:\Users\meita\PlaywrightTests\screenshots\logout-user-test.png");

        page.Dialog += async (_, dialog) =>
        {
            await dialog.AcceptAsync();
        };

        await page.Locator("input[data-qa='submit-button']").ClickAsync();

        await page
            .Locator(".contact-form .status.alert.alert-success")
            .WaitForAsync();

        await page
            .Locator("#form-section a.btn.btn-success")
            .ClickAsync();

        await page.WaitForURLAsync("https://automationexercise.com/");

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\contact-us-final.png"
        });

        Console.WriteLine("Contact Us test passed successfully");
    }
}