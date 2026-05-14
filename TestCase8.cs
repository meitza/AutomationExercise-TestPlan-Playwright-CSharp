using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class ProductsDetailTest
{
    [Test]
    public async Task ProductsDetailTestCase()
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

        // Click Products
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        // Verify ALL PRODUCTS page
        await page.WaitForURLAsync("**/products");
        await page.GetByText("All Products").WaitForAsync();

        // Verify products list is visible
        await page.Locator(".features_items").WaitForAsync();

        // Click View Product of first product
        await page
            .Locator(".features_items .choose a")
            .First
            .ClickAsync();

        // Verify product detail page
        await page.WaitForURLAsync("**/product_details/*");

        // Verify product details are visible
        await page.Locator(".product-information h2").WaitForAsync();

        await page
            .Locator(".product-information")
            .GetByText("Category:")
            .WaitForAsync();

        await page
            .Locator(".product-information")
            .GetByText("Rs.")
            .WaitForAsync();

        await page
            .Locator(".product-information")
            .GetByText("Availability:")
            .WaitForAsync();

        await page
            .Locator(".product-information")
            .GetByText("Condition:")
            .WaitForAsync();

        await page
            .Locator(".product-information")
            .GetByText("Brand:")
            .WaitForAsync();

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\product-detail-test.png"
        });

        Console.WriteLine("Products detail test passed successfully");
    }
}