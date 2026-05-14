using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

public class Tests
{
    [Test]
    public async Task RegisterTest()
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

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Signup / Login" })
            .ClickAsync();

        await page.GetByText("New User Signup!").WaitForAsync();

        var random = DateTime.Now.Ticks;
        var email = $"test{random}@gmail.com";

        await page.Locator("input[data-qa='signup-name']").FillAsync("Test User");
        await page.Locator("input[data-qa='signup-email']").FillAsync(email);

        await page.Locator("button[data-qa='signup-button']").ClickAsync();

        await page.GetByText("Enter Account Information").WaitForAsync();

        await page.Locator("#id_gender1").CheckAsync();

        await page.Locator("#password").FillAsync("Test12345");

        await page.Locator("#days").SelectOptionAsync("10");
        await page.Locator("#months").SelectOptionAsync("5");
        await page.Locator("#years").SelectOptionAsync("1999");

        await page.Locator("#newsletter").CheckAsync();
        await page.Locator("#optin").CheckAsync();

        await page.Locator("#first_name").FillAsync("Test");
        await page.Locator("#last_name").FillAsync("User");
        await page.Locator("#company").FillAsync("Test Company");
        await page.Locator("#address1").FillAsync("Test Address 1");
        await page.Locator("#address2").FillAsync("Test Address 2");

        await page.Locator("#country").SelectOptionAsync("Canada");

        await page.Locator("#state").FillAsync("Test State");
        await page.Locator("#city").FillAsync("Test City");
        await page.Locator("#zipcode").FillAsync("12345");
        await page.Locator("#mobile_number").FillAsync("0712345678");

        await page.Locator("button[data-qa='create-account']").ClickAsync();

        await page.GetByText("Account Created!").WaitForAsync();

        await page.Locator("a[data-qa='continue-button']").ClickAsync();

        await page.GetByText("Logged in as").WaitForAsync();

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Delete Account" })
            .ClickAsync();

        await page.GetByText("Account Deleted!").WaitForAsync();

        await page.Locator("a[data-qa='continue-button']").ClickAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\register-account-test.png",
            FullPage = true
        });
    }
}