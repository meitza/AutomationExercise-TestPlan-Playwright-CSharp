using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class BrandProductsTest
{
    [Test]
    public async Task BrandProductsTestCase()
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
                await route.AbortAsync();
            else
                await route.ContinueAsync();
        });

        var page = await context.NewPageAsync();

        await page.GotoAsync("https://automationexercise.com/");

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        await page.WaitForURLAsync("**/products");

        await page
            .Locator(".brands_products")
            .GetByText("Brands")
            .WaitForAsync();

        await page
            .Locator(".brands-name a[href='/brand_products/Polo']")
            .ClickAsync();

        await page.WaitForURLAsync("**/brand_products/Polo");

        await page
            .GetByText("Brand - Polo Products")
            .WaitForAsync();

        await page
            .Locator(".features_items .product-image-wrapper")
            .First
            .WaitForAsync();

        await page
            .Locator(".brands-name a[href='/brand_products/H&M']")
            .ClickAsync();

        await page.WaitForURLAsync("**/brand_products/H&M");

        await page
            .GetByText("Brand - H&M Products")
            .WaitForAsync();

        await page
            .Locator(".features_items .product-image-wrapper")
            .First
            .WaitForAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\brand-products-test.png"
        });

        Console.WriteLine("Brand products test passed successfully");
    }
}