using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class CartSubscriptionTest
{
    [Test]
    public async Task CartSubscriptionTestCase()
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

        // Click Cart button
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Cart" })
            .ClickAsync();

        // Verify cart page
        await page.WaitForURLAsync("**/view_cart");

        // Scroll to footer
        await page
            .Locator("#footer")
            .ScrollIntoViewIfNeededAsync();

        // Verify SUBSCRIPTION text
        await page
            .Locator("#footer")
            .GetByText("Subscription")
            .WaitForAsync();

        // Enter email
        await page
            .Locator("#susbscribe_email")
            .FillAsync("testemail@gmail.com");

        // Click subscribe button
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
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\cart-subscription-test.png"
        });

        Console.WriteLine("Cart subscription test passed successfully");
    }
}