using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class MainSliderLoopTest
{
    [Test]
    public async Task MainSliderLoopTestCase()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 1200
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
            new() { WaitUntil = WaitUntilState.DOMContentLoaded }
        );

        // Verify slider visible
        await page
            .Locator("#slider-carousel")
            .WaitForAsync();

        var slider = page.Locator("#slider-carousel");

        // First slide text
        var firstSlideText = await slider
            .Locator(".item.active")
            .InnerTextAsync();

        // Click NEXT 3 times
        for (int i = 0; i < 3; i++)
        {
            await slider
                .Locator(".right")
                .First
                .ClickAsync();

            await page.WaitForTimeoutAsync(1500);
        }

        // Verify returned to first slide
        var currentSlideText = await slider
            .Locator(".item.active")
            .InnerTextAsync();

        Assert.That(currentSlideText, Is.EqualTo(firstSlideText));

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\main-slider-loop-test.png"
        });

        Console.WriteLine("Main slider loop test passed successfully");
    }
}