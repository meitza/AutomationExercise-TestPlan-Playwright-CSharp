using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class PlaceOrderLoginBeforeCheckoutTest
{
    [Test]
    public async Task PlaceOrderLoginBeforeCheckoutTestCase()
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

        var email = "MAtestcase16@gmail.com";
        var password = "parola";

        await page.GotoAsync(
            "https://automationexercise.com/",
            new() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // Verify home page
        await page.WaitForURLAsync("https://automationexercise.com/");

        // Click Signup / Login
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Signup / Login" })
            .ClickAsync();

        // Login
        await page.GetByText("Login to your account").WaitForAsync();

        await page.Locator("input[data-qa='login-email']").FillAsync(email);
        await page.Locator("input[data-qa='login-password']").FillAsync(password);
        await page.Locator("button[data-qa='login-button']").ClickAsync();

        // Verify logged in
        await page.GetByText("Logged in as").WaitForAsync();

        // Add product to cart
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

        // Proceed to checkout
        await page.GetByText("Proceed To Checkout").ClickAsync();

        // Verify checkout details
        await page.GetByText("Address Details").WaitForAsync();
        await page.GetByText("Review Your Order").WaitForAsync();

        // Comment
        await page
            .Locator("textarea[name='message']")
            .FillAsync("Please deliver this order as soon as possible.");

        // Place order
        await page
            .GetByRole(AriaRole.Link, new() { Name = "Place Order" })
            .ClickAsync();

        // Payment details
        await page.Locator("input[data-qa='name-on-card']").FillAsync("Test User");
        await page.Locator("input[data-qa='card-number']").FillAsync("4111111111111111");
        await page.Locator("input[data-qa='cvc']").FillAsync("123");
        await page.Locator("input[data-qa='expiry-month']").FillAsync("12");
        await page.Locator("input[data-qa='expiry-year']").FillAsync("2030");

        // Pay and confirm
        await page.Locator("button[data-qa='pay-button']").ClickAsync();

        // Verify success message
        await page
            .Locator("p")
            .Filter(new() { HasText = "Congratulations! Your order has been confirmed!" })
            .WaitForAsync();

        // Delete account
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Delete Account" })
            .ClickAsync();

        // Verify deleted
        await page.GetByText("Account Deleted!").WaitForAsync();

        await page.Locator("a[data-qa='continue-button']").ClickAsync();

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\place-order-login-before-checkout-test.png"
        });

        Console.WriteLine("Place order login before checkout test passed successfully");
    }
}