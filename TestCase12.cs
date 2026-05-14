using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class AddProductsToCartTest
{
    [Test]
    public async Task AddTwoProductsToCartTestCase()
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

        var firstProduct = page.Locator(".product-image-wrapper").Nth(0);
        await firstProduct.HoverAsync();

        await firstProduct
            .Locator(".overlay-content a.add-to-cart")
            .ClickAsync();

        await page
            .GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" })
            .ClickAsync();

        var secondProduct = page.Locator(".product-image-wrapper").Nth(1);
        await secondProduct.HoverAsync();

        await secondProduct
            .Locator(".overlay-content a.add-to-cart")
            .ClickAsync();

        await page
            .GetByRole(AriaRole.Link, new() { Name = "View Cart" })
            .ClickAsync();

        await page.WaitForURLAsync("**/view_cart");

        var cartRows = page.Locator("#cart_info_table tbody tr");

        await NUnit.Framework.Assert.ThatAsync(
            async () => await cartRows.CountAsync(),
            Is.EqualTo(2)
        );

        for (int i = 0; i < 2; i++)
        {
            var row = cartRows.Nth(i);

            await row.Locator(".cart_price").WaitForAsync();
            await row.Locator(".cart_quantity").WaitForAsync();
            await row.Locator(".cart_total").WaitForAsync();

            var priceText = await row.Locator(".cart_price").InnerTextAsync();
            var quantityText = await row.Locator(".cart_quantity").InnerTextAsync();
            var totalText = await row.Locator(".cart_total").InnerTextAsync();

            Assert.That(priceText, Does.Contain("Rs."));
            Assert.That(quantityText.Trim(), Is.EqualTo("1"));
            Assert.That(totalText, Does.Contain("Rs."));
        }

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\add-two-products-cart-test.png"
        });

        Console.WriteLine("Add two products to cart test passed successfully");
    }
}