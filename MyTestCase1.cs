using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class CartPersistsAfterRefreshTest
{
    [Test]
    public async Task CartPersistsAfterRefreshTestCase()
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

        await page.GotoAsync(
            "https://automationexercise.com/",
            new() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        await page.WaitForURLAsync("https://automationexercise.com/");

        var firstProduct = page.Locator(".product-image-wrapper").First;

        var productName = await firstProduct
            .Locator(".productinfo p")
            .InnerTextAsync();

        await firstProduct.HoverAsync();

        await firstProduct
            .Locator(".overlay-content a.add-to-cart")
            .ClickAsync();

        await page
            .GetByRole(AriaRole.Link, new() { Name = "View Cart" })
            .ClickAsync();

        await page.WaitForURLAsync("**/view_cart");

        var cartRow = page.Locator("#cart_info_table tbody tr").First;

        await cartRow.WaitForAsync();

        await page
            .Locator("#cart_info_table")
            .GetByText(productName)
            .WaitForAsync();

        var priceBeforeRefresh = await cartRow.Locator(".cart_price").InnerTextAsync();
        var quantityBeforeRefresh = await cartRow.Locator(".cart_quantity").InnerTextAsync();
        var totalBeforeRefresh = await cartRow.Locator(".cart_total").InnerTextAsync();

        await page.ReloadAsync(new()
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        await page.WaitForURLAsync("**/view_cart");

        var cartRowAfterRefresh = page.Locator("#cart_info_table tbody tr").First;

        await cartRowAfterRefresh.WaitForAsync();

        await page
            .Locator("#cart_info_table")
            .GetByText(productName)
            .WaitForAsync();

        var priceAfterRefresh = await cartRowAfterRefresh.Locator(".cart_price").InnerTextAsync();
        var quantityAfterRefresh = await cartRowAfterRefresh.Locator(".cart_quantity").InnerTextAsync();
        var totalAfterRefresh = await cartRowAfterRefresh.Locator(".cart_total").InnerTextAsync();

        Assert.That(priceAfterRefresh, Is.EqualTo(priceBeforeRefresh));
        Assert.That(quantityAfterRefresh.Trim(), Is.EqualTo(quantityBeforeRefresh.Trim()));
        Assert.That(totalAfterRefresh, Is.EqualTo(totalBeforeRefresh));

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\cart-persists-after-refresh-test.png"
        });

        Console.WriteLine("Cart persists after refresh test passed successfully");
    }
}