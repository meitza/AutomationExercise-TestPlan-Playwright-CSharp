using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class EmptyCartCheckoutTest
{
    [Test]
    public async Task EmptyCartCheckoutTestCase()
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

        // Verify home page
        await page.WaitForURLAsync("https://automationexercise.com/");

        // Click Cart
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Cart" })
            .ClickAsync();

        // Verify cart page
        await page.WaitForURLAsync("**/view_cart");

        // Verify cart is empty
        await page
            .GetByText("Cart is empty!")
            .WaitForAsync();

        // Verify Proceed To Checkout button does not exist
        var checkoutButton = page.GetByText("Proceed To Checkout");

        Assert.That(await checkoutButton.CountAsync(), Is.EqualTo(0));

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\empty-cart-checkout-test.png"
        });

        Console.WriteLine("Empty cart checkout test passed successfully");
    }
}