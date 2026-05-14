using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class SubscriptionTest
{
    [Test]
    public async Task SubscriptionTestCase()
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
        _ = await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");

        // Verify SUBSCRIPTION text
        await page.GetByText("Subscription").WaitForAsync();

        // Enter email
        await page
            .Locator("#susbscribe_email")
            .FillAsync("testemail@gmail.com");

        // Click arrow button
        await page
            .Locator("#subscribe")
            .ClickAsync();

        // Verify success message
        await page
            .Locator("#success-subscribe")
            .GetByText("You have been successfully subscribed!")
            .WaitForAsync();

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\subscription-test.png"
        });

        Console.WriteLine("Subscription test passed successfully");
    }
}