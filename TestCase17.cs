using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class RemoveProductFromCartTest
{
    [Test]
    public async Task RemoveProductFromCartTestCase()
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

        // Add first product to cart
        var firstProduct = page.Locator(".product-image-wrapper").First;
        await firstProduct.HoverAsync();

        await firstProduct
            .Locator(".overlay-content a.add-to-cart")
            .ClickAsync();

        await page
            .GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" })
            .ClickAsync();

        // Click Cart
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Cart" })
            .ClickAsync();

        // Verify cart page
        await page.WaitForURLAsync("**/view_cart");

        var cartRows = page.Locator("#cart_info_table tbody tr");
        await cartRows.First.WaitForAsync();

        // Remove product
        await cartRows
            .First
            .Locator(".cart_quantity_delete")
            .ClickAsync();

        // Verify cart is empty / product removed
        await page
            .GetByText("Cart is empty!")
            .WaitForAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\remove-product-from-cart-test.png"
        });

        Console.WriteLine("Remove product from cart test passed successfully");
    }
}