using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class VerifyAllBrandPagesTest
{
    [Test]
    public async Task VerifyAllBrandPagesTestCase()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 700
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

        // Click Products
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        // Verify ALL PRODUCTS page
        await page.WaitForURLAsync("**/products");

        await page.GetByText("All Products").WaitForAsync();

        // Verify Brands section
        await page
            .Locator(".brands_products")
            .GetByText("Brands")
            .WaitForAsync();

        // Brand names
        var brands = new[]
        {
            "Polo",
            "H&M",
            "Madame",
            "Mast & Harbour",
            "Babyhug",
            "Allen Solly Junior",
            "Kookie Kids",
            "Biba"
        };

        foreach (var brand in brands)
        {
            // Click exact brand link
            await page
                .Locator(".brands-name a")
                .Filter(new() { HasText = brand })
                .First
                .ClickAsync();

            // Verify URL changed
            await page.WaitForURLAsync("**/brand_products/**");

            // Verify products exist
            var products = page.Locator(".features_items .product-image-wrapper");

            await products.First.WaitForAsync();

            var count = await products.CountAsync();

            Assert.That(count, Is.GreaterThan(0));

            Console.WriteLine($"{brand} verified successfully");
        }

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\verify-all-brand-pages-test.png"
        });

        Console.WriteLine("Verify all brand pages test passed successfully");
    }
}