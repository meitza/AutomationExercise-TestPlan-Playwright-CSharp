using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class PlaceOrderRegisterBeforeCheckoutTest
{
    [Test]
    public async Task PlaceOrderRegisterBeforeCheckoutTestCase()
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

        var random = DateTime.Now.Ticks;
        var name = "Test User";
        var email = $"test{random}@gmail.com";

        await page.GotoAsync("https://automationexercise.com/");

        await page.WaitForURLAsync("https://automationexercise.com/");

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Signup / Login" })
            .ClickAsync();

        await page.GetByText("New User Signup!").WaitForAsync();

        await page.Locator("input[data-qa='signup-name']").FillAsync(name);
        await page.Locator("input[data-qa='signup-email']").FillAsync(email);
        await page.Locator("button[data-qa='signup-button']").ClickAsync();

        await page.GetByText("Enter Account Information").WaitForAsync();

        await page.Locator("#id_gender1").CheckAsync();
        await page.Locator("#password").FillAsync("Test12345");

        await page.Locator("#days").SelectOptionAsync("10");
        await page.Locator("#months").SelectOptionAsync("5");
        await page.Locator("#years").SelectOptionAsync("1999");

        await page.Locator("#newsletter").CheckAsync();
        await page.Locator("#optin").CheckAsync();

        await page.Locator("#first_name").FillAsync("Test");
        await page.Locator("#last_name").FillAsync("User");
        await page.Locator("#company").FillAsync("Test Company");
        await page.Locator("#address1").FillAsync("Test Address 1");
        await page.Locator("#address2").FillAsync("Test Address 2");
        await page.Locator("#country").SelectOptionAsync("Canada");
        await page.Locator("#state").FillAsync("Test State");
        await page.Locator("#city").FillAsync("Test City");
        await page.Locator("#zipcode").FillAsync("12345");
        await page.Locator("#mobile_number").FillAsync("0712345678");

        await page.Locator("button[data-qa='create-account']").ClickAsync();

        await page.GetByText("Account Created!").WaitForAsync();

        await page.Locator("a[data-qa='continue-button']").ClickAsync();

        await page.GetByText("Logged in as").WaitForAsync();

        var firstProduct = page.Locator(".product-image-wrapper").First;
        await firstProduct.HoverAsync();

        await firstProduct
            .Locator(".overlay-content a.add-to-cart")
            .ClickAsync();

        await page
            .GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" })
            .ClickAsync();

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Cart" })
            .ClickAsync();

        await page.WaitForURLAsync("**/view_cart");

        await page.GetByText("Proceed To Checkout").ClickAsync();

        await page.GetByText("Address Details").WaitForAsync();
        await page.GetByText("Review Your Order").WaitForAsync();

        await page
            .Locator("textarea[name='message']")
            .FillAsync("Please deliver this order as soon as possible.");

        await page
            .GetByRole(AriaRole.Link, new() { Name = "Place Order" })
            .ClickAsync();

        await page.Locator("input[data-qa='name-on-card']").FillAsync("Test User");
        await page.Locator("input[data-qa='card-number']").FillAsync("4111111111111111");
        await page.Locator("input[data-qa='cvc']").FillAsync("123");
        await page.Locator("input[data-qa='expiry-month']").FillAsync("12");
        await page.Locator("input[data-qa='expiry-year']").FillAsync("2030");

        await page
            .Locator("button[data-qa='pay-button']")
            .ClickAsync();

        await page
            .Locator("p")
            .Filter(new() { HasText = "Congratulations! Your order has been confirmed!" })
            .WaitForAsync();

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Delete Account" })
            .ClickAsync();

        await page.GetByText("Account Deleted!").WaitForAsync();

        await page.Locator("a[data-qa='continue-button']").ClickAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\place-order-register-before-checkout-test.png"
        });

        Console.WriteLine("Place order register before checkout test passed successfully");
    }
}