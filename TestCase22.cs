using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class RecommendedItemsCartTest
{
    [Test]
    public async Task RecommendedItemsCartTestCase()
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

        // Scroll to bottom
        await page
            .Locator(".recommended_items")
            .ScrollIntoViewIfNeededAsync();

        // Verify RECOMMENDED ITEMS
        await page
            .GetByText("recommended items")
            .WaitForAsync();

        // Save recommended product name
        var productName = await page
            .Locator(".recommended_items .productinfo p")
            .First
            .InnerTextAsync();

        // Click Add To Cart on recommended product
        await page
            .Locator(".recommended_items a.add-to-cart")
            .First
            .ClickAsync();

        // Click View Cart
        await page
            .GetByRole(AriaRole.Link, new() { Name = "View Cart" })
            .ClickAsync();

        // Verify cart page
        await page.WaitForURLAsync("**/view_cart");

        // Verify product is displayed in cart
        await page
            .Locator("#cart_info_table")
            .GetByText(productName)
            .WaitForAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\recommended-items-cart-test.png"
        });

        Console.WriteLine("Recommended items cart test passed successfully");
    }
}