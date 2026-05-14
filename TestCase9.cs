using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class SearchProductTest
{
    [Test]
    public async Task SearchProductTestCase()
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

        var productName = "Blue Top";

        await page.GotoAsync(
            "https://automationexercise.com/",
            new() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // Verify home page
        await page.WaitForURLAsync("https://automationexercise.com/");

        // Click Products
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        // Verify ALL PRODUCTS page
        await page.WaitForURLAsync("**/products");

        await page.GetByText("All Products").WaitForAsync();

        // Enter product name in search input
        await page.Locator("#search_product").FillAsync(productName);

        // Click search button
        await page.Locator("#submit_search").ClickAsync();

        // Verify SEARCHED PRODUCTS is visible
        await page.GetByText("Searched Products").WaitForAsync();

        // Verify searched products are visible
        await page
            .Locator(".features_items")
            .GetByText(productName)
            .First
            .WaitForAsync();

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\search-product-test.png"
        });

        Console.WriteLine("Search product test passed successfully");
    }
}