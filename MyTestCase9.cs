using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class LogoRedirectHomeTest
{
    [Test]
    public async Task LogoRedirectHomeTestCase()
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

        // Verify homepage
        await page.WaitForURLAsync("https://automationexercise.com/");

        // Go to Products page
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        // Verify Products page
        await page.WaitForURLAsync("**/products");

        await page.GetByText("All Products").WaitForAsync();

        // Click logo
        await page
            .Locator("div.logo a")
            .ClickAsync();

        // Verify redirected to homepage
        await page.WaitForURLAsync("https://automationexercise.com/");

        // Verify homepage slider visible
        await page
            .Locator("#slider-carousel")
            .WaitForAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\logo-redirect-home-test.png"
        });

        Console.WriteLine("Logo redirect home test passed successfully");
    }
}