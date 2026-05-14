using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class SearchProductsCartAfterLoginTest
{
    [Test]
    public async Task SearchProductsCartAfterLoginTestCase()
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

        var productName = "Blue Top";

        // Pune aici un cont valid existent
        var email = "adrian@meita.com";
        var password = "parola";

        await page.GotoAsync("https://automationexercise.com/");

        // Click Products
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        await page.WaitForURLAsync("**/products");

        await page.GetByText("All Products").WaitForAsync();

        // Search product
        await page.Locator("#search_product").FillAsync(productName);
        await page.Locator("#submit_search").ClickAsync();

        await page.GetByText("Searched Products").WaitForAsync();

        await page
            .Locator(".features_items")
            .GetByText(productName)
            .First
            .WaitForAsync();

        // Add searched product to cart
        var searchedProduct = page.Locator(".product-image-wrapper").First;

        await searchedProduct.HoverAsync();

        await searchedProduct
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

        await page.WaitForURLAsync("**/view_cart");

        // Verify product visible in cart
        await page
            .Locator("#cart_info_table")
            .GetByText(productName)
            .WaitForAsync();

        // Click Signup / Login
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Signup / Login" })
            .ClickAsync();

        await page.GetByText("Login to your account").WaitForAsync();

        // Login
        await page.Locator("input[data-qa='login-email']").FillAsync(email);
        await page.Locator("input[data-qa='login-password']").FillAsync(password);
        await page.Locator("button[data-qa='login-button']").ClickAsync();

        await page.GetByText("Logged in as").WaitForAsync();

        // Go again to Cart
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Cart" })
            .ClickAsync();

        await page.WaitForURLAsync("**/view_cart");

        // Verify product still visible after login
        await page
            .Locator("#cart_info_table")
            .GetByText(productName)
            .WaitForAsync();

        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\search-products-cart-after-login-test.png"
        });

        Console.WriteLine("Search products and verify cart after login test passed successfully");
    }
}