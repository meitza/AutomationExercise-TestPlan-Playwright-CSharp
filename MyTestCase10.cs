using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class NavbarLinksTest
{
    [Test]
    public async Task NavbarLinksTestCase()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 800
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

        // Verify navbar visible
        await page
            .Locator("#header")
            .WaitForAsync();

        // HOME
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Home" })
            .First
            .ClickAsync();

        await page.WaitForURLAsync("https://automationexercise.com/");

        // PRODUCTS
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        await page.WaitForURLAsync("**/products");

        // CART
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Cart" })
            .ClickAsync();

        await page.WaitForURLAsync("**/view_cart");

        // SIGNUP / LOGIN
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Signup / Login" })
            .ClickAsync();

        await page.WaitForURLAsync("**/login");

        // TEST CASES
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Test Cases" })
            .ClickAsync();

        await page.WaitForURLAsync("**/test_cases");

        // API TESTING
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "API Testing" })
            .ClickAsync();

        await page.WaitForURLAsync("**/api_list");

        // VIDEO TUTORIALS
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Video Tutorials" })
            .ClickAsync();

        // Wait for YouTube page
        await page.WaitForURLAsync("**youtube.com**");

        // Return to site
        await page.GoBackAsync();

        await page.WaitForURLAsync("**automationexercise.com**");

        // CONTACT US
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Contact us" })
            .ClickAsync();

        await page.WaitForURLAsync("**/contact_us");

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\navbar-links-test.png"
        });

        Console.WriteLine("Navbar links test passed successfully");
    }
}