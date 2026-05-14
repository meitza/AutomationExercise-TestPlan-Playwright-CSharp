using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class MobileBackToTopTest
{
    [Test]
    public async Task MobileBackToTopTestCase()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 1000
        });

        var context = await browser.NewContextAsync(new()
        {
            ViewportSize = new ViewportSize
            {
                Width = 390,
                Height = 844
            },
            IsMobile = true
        });

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

        await page.WaitForURLAsync("https://automationexercise.com/");

        await page
            .Locator("#footer")
            .ScrollIntoViewIfNeededAsync();

        await page
            .Locator("#footer")
            .GetByText("Subscription")
            .WaitForAsync();

        await page
            .Locator("#scrollUp")
            .WaitForAsync();

        await page
            .Locator("#scrollUp")
            .ClickAsync();

        await page.WaitForFunctionAsync("window.scrollY === 0");

        await page
            .Locator("#slider-carousel")
            .WaitForAsync();

        Console.WriteLine("Mobile back to top test passed successfully");
    }
}