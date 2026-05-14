using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class LoginDeleteTest
{
    [Test]
    public async Task LoginAndDeleteAccountTest()
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

        var email = "MAtestcase2@gmail.com";
        var password = "parola";

        await page.GotoAsync(
            "https://automationexercise.com/",
            new() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        await page.WaitForURLAsync("https://automationexercise.com/");

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Signup / Login" })
            .ClickAsync();

        await page.GetByText("Login to your account").WaitForAsync();

        await page.Locator("input[data-qa='login-email']").FillAsync(email);
        await page.Locator("input[data-qa='login-password']").FillAsync(password);

        await page.Locator("button[data-qa='login-button']").ClickAsync();

        await page.GetByText("Logged in as").WaitForAsync();

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Delete Account" })
            .ClickAsync();

        await page.GetByText("Account Deleted!").WaitForAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\login-delete-account-test.png",
            FullPage = true
        });

        Console.WriteLine("Login and delete account test passed successfully");
    }
}