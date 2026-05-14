using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class VerifyProductsInfoTest
{
    [Test]
    public async Task VerifyProductsInfoTestCase()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 500
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

        await page
            .GetByText("All Products")
            .WaitForAsync();

        // Get all products
        var products = page.Locator(".product-image-wrapper");

        var productCount = await products.CountAsync();

        Assert.That(productCount, Is.GreaterThan(0));

        // Verify each product
        for (int i = 0; i < productCount; i++)
        {
            var product = products.Nth(i);

            // Verify image
            await product
                .Locator("img")
                .WaitForAsync();

            // Verify name
            var name = product.Locator(".productinfo p");

            await name.WaitForAsync();

            var productName = await name.InnerTextAsync();

            Assert.That(productName.Trim(), Is.Not.Empty);

            // Verify price
            var price = product.Locator(".productinfo h2");

            await price.WaitForAsync();

            var productPrice = await price.InnerTextAsync();

            Assert.That(productPrice.Contains("Rs."), Is.True);
        }

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\verify-products-info-test.png"
        });

        Console.WriteLine("Verify products info test passed successfully");
    }
}