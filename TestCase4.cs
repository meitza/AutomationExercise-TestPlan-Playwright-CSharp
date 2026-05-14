using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class LogoutUserTest
{
    [Test]
    public async Task LogoutUserTestCase()
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

        var email = "MAtestcase4@gmail.com";
        var password = "parola";

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

        // Enter correct email
        await page
            .Locator("input[data-qa='login-email']")
            .FillAsync(email);

        // Enter correct password
        await page
            .Locator("input[data-qa='login-password']")
            .FillAsync(password);

        // Click login button
        await page
            .Locator("button[data-qa='login-button']")
            .ClickAsync();

        // Verify logged in
        await page.GetByText("Logged in as").WaitForAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\logged-in-as-username.png",
            
        });


        // Click Logout
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Logout" })
            .ClickAsync();

        // Verify user navigated to login page
        await page.GetByText("Login to your account").WaitForAsync();

       
        // Screenshot final
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\logout-user-test.png",
            FullPage = true
        });

        Console.WriteLine("Logout user test passed successfully");
    }
}