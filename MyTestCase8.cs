using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class SearchNonExistingProductTest
{
    [Test]
    public async Task SearchNonExistingProductTestCase()
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

        await page.WaitForURLAsync("https://automationexercise.com/");

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        await page.WaitForURLAsync("**/products");

        await page.GetByText("All Products").WaitForAsync();

        await page
            .Locator("#search_product")
            .FillAsync("ProductThatDoesNotExist123");

        await page
            .Locator("#submit_search")
            .ClickAsync();

        await page.GetByText("Searched Products").WaitForAsync();

        var productsCount = await page
            .Locator(".features_items .product-image-wrapper")
            .CountAsync();

        Assert.That(productsCount, Is.EqualTo(0));

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\search-non-existing-product-test.png"
        });

        Console.WriteLine("Search non-existing product test passed successfully");
    }
}