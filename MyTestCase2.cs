using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class AddSameProductMultipleTimesTest
{
    [Test]
    public async Task AddSameProductMultipleTimesTestCase()
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
            .GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" })
            .ClickAsync();

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

        await cartRow
            .Locator(".cart_quantity")
            .GetByText("2")
            .WaitForAsync();

        var priceText = await cartRow.Locator(".cart_price").InnerTextAsync();
        var totalText = await cartRow.Locator(".cart_total").InnerTextAsync();

        var price = int.Parse(priceText.Replace("Rs.", "").Trim());
        var total = int.Parse(totalText.Replace("Rs.", "").Trim());

        Assert.That(total, Is.EqualTo(price * 2));

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\add-same-product-multiple-times-test.png"
        });

        Console.WriteLine("Add same product multiple times test passed successfully");
    }
}