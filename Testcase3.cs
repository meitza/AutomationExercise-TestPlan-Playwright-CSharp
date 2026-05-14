using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class InvalidLoginTest
{
    [Test]
    public async Task InvalidLoginTestCase()
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

        // Click Signup / Login
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Signup / Login" })
            .ClickAsync();

        // Verify Login to your account
        await page.GetByText("Login to your account").WaitForAsync();

        // Enter incorrect email
        await page
            .Locator("input[data-qa='login-email']")
            .FillAsync("wrongemail@gmail.com");

        // Enter incorrect password
        await page
            .Locator("input[data-qa='login-password']")
            .FillAsync("WrongPassword123");

        // Click login button
        await page
            .Locator("button[data-qa='login-button']")
            .ClickAsync();

        // Verify error message
        await page
            .GetByText("Your email or password is incorrect!")
            .WaitForAsync();

        // Screenshot final
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\invalid-login-test.png",
            FullPage = true
        });

        Console.WriteLine("Invalid login test passed successfully");
    }
}