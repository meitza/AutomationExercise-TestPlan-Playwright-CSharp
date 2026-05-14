using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class VerifyAddressDetailsCheckoutTest
{
    [Test]
    public async Task VerifyAddressDetailsCheckoutTestCase()
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

        var random = DateTime.Now.Ticks;
        var name = "Test User";
        var email = $"test{random}@gmail.com";
        var password = "Test12345";

        var firstName = "Test";
        var lastName = "User";
        var company = "Test Company";
        var address1 = "Test Address 1";
        var address2 = "Test Address 2";
        var country = "Canada";
        var state = "Test State";
        var city = "Test City";
        var zipcode = "12345";
        var mobile = "0712345678";

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
        await page.Locator("#password").FillAsync(password);

        await page.Locator("#days").SelectOptionAsync("10");
        await page.Locator("#months").SelectOptionAsync("5");
        await page.Locator("#years").SelectOptionAsync("1999");

        await page.Locator("#newsletter").CheckAsync();
        await page.Locator("#optin").CheckAsync();

        await page.Locator("#first_name").FillAsync(firstName);
        await page.Locator("#last_name").FillAsync(lastName);
        await page.Locator("#company").FillAsync(company);
        await page.Locator("#address1").FillAsync(address1);
        await page.Locator("#address2").FillAsync(address2);
        await page.Locator("#country").SelectOptionAsync(country);
        await page.Locator("#state").FillAsync(state);
        await page.Locator("#city").FillAsync(city);
        await page.Locator("#zipcode").FillAsync(zipcode);
        await page.Locator("#mobile_number").FillAsync(mobile);

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

        var deliveryAddress = page.Locator("#address_delivery");
        var billingAddress = page.Locator("#address_invoice");

        await deliveryAddress.GetByText($"{firstName} {lastName}").WaitForAsync();
        await deliveryAddress.GetByText(company).WaitForAsync();
        await deliveryAddress.GetByText(address1).WaitForAsync();
        await deliveryAddress.GetByText(address2).WaitForAsync();
        await deliveryAddress.GetByText($"{city} {state} {zipcode}").WaitForAsync();
        await deliveryAddress.GetByText(country).WaitForAsync();
        await deliveryAddress.GetByText(mobile).WaitForAsync();

        await billingAddress.GetByText($"{firstName} {lastName}").WaitForAsync();
        await billingAddress.GetByText(company).WaitForAsync();
        await billingAddress.GetByText(address1).WaitForAsync();
        await billingAddress.GetByText(address2).WaitForAsync();
        await billingAddress.GetByText($"{city} {state} {zipcode}").WaitForAsync();
        await billingAddress.GetByText(country).WaitForAsync();
        await billingAddress.GetByText(mobile).WaitForAsync();

        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Delete Account" })
            .ClickAsync();

        await page.GetByText("Account Deleted!").WaitForAsync();

        await page.Locator("a[data-qa='continue-button']").ClickAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\verify-address-details-checkout-test.png"
        });

        Console.WriteLine("Verify address details checkout test passed successfully");
    }
}