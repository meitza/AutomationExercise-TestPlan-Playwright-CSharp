using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class ProductQuantityCartTest
{
    [Test]
    public async Task ProductQuantityCartTestCase()
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

        // Click View Product for first product on home page
        await page
            .Locator(".features_items .choose a")
            .First
            .ClickAsync();

        // Verify product detail page is opened
        await page.WaitForURLAsync("**/product_details/*");

        await page
            .Locator(".product-information")
            .WaitForAsync();

        // Increase quantity to 4
        await page
            .Locator("#quantity")
            .FillAsync("4");

        // Click Add to cart
        await page
            .GetByRole(AriaRole.Button, new() { Name = "Add to cart" })
            .ClickAsync();

        // Click View Cart
        await page
            .GetByRole(AriaRole.Link, new() { Name = "View Cart" })
            .ClickAsync();

        // Verify cart page
        await page.WaitForURLAsync("**/view_cart");

        // Verify product is displayed in cart with quantity 4
        var cartRow = page.Locator("#cart_info_table tbody tr").First;

        await cartRow.WaitForAsync();

        await cartRow
            .Locator(".cart_quantity")
            .GetByText("4")
            .WaitForAsync();

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\product-quantity-cart-test.png"
        });

        Console.WriteLine("Product quantity cart test passed successfully");
    }
}