using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class ExistingEmailSignupTest
{
    [Test]
    public async Task ExistingEmailSignupTestCase()
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

        // Existing account email
        var existingEmail = "test@gmail.com";

        await page.GotoAsync(
            "https://automationexercise.com/",
            new() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // Verify home page
        await page.WaitForURLAsync("https://automationexercise.com/");

        // Click Signup / Login
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Signup / Login" })
            .ClickAsync();

        // Verify New User Signup
        await page.GetByText("New User Signup!").WaitForAsync();

        // Enter name
        await page
            .Locator("input[data-qa='signup-name']")
            .FillAsync("Test User");

        // Enter already registered email
        await page
            .Locator("input[data-qa='signup-email']")
            .FillAsync(existingEmail);

        // Click Signup button
        await page
            .Locator("button[data-qa='signup-button']")
            .ClickAsync();

        // Verify error message
        await page
            .GetByText("Email Address already exist!")
            .WaitForAsync();

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\existing-email-error.png"
        });

        Console.WriteLine("Existing email signup test passed successfully");
    }
}