using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class ScrollUpWithoutArrowTest
{
    [Test]
    public async Task ScrollUpWithoutArrowTestCase()
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

        await page.GotoAsync(
            "https://automationexercise.com/",
            new() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // Verify home page
        await page.WaitForURLAsync("https://automationexercise.com/");

        // Scroll down to footer
        await page
            .Locator("#footer")
            .ScrollIntoViewIfNeededAsync();

        // Verify SUBSCRIPTION text
        await page
            .Locator("#footer")
            .GetByText("Subscription")
            .WaitForAsync();

        // Scroll up manually
        await page.EvaluateAsync("window.scrollTo(0, 0)");

        // Verify top heading is visible
        await page
            .GetByRole(AriaRole.Heading, new() { Name = "Full-Fledged practice website" })
            .WaitForAsync();

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\scroll-up-without-arrow-test.png"
        });

        Console.WriteLine("Scroll up without arrow test passed successfully");
    }
}