using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests;

[TestFixture]
public class AddReviewOnProductTest
{
    [Test]
    public async Task AddReviewOnProductTestCase()
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

        // Click Products
        await page
            .Locator("#header")
            .GetByRole(AriaRole.Link, new() { Name = "Products" })
            .ClickAsync();

        // Verify ALL PRODUCTS page
        await page.WaitForURLAsync("**/products");

        await page.GetByText("All Products").WaitForAsync();

        // Click View Product
        await page
            .Locator(".choose a")
            .First
            .ClickAsync();

        // Verify review section
        await page
            .GetByText("Write Your Review")
            .WaitForAsync();

        // Fill review form
        await page
            .Locator("#name")
            .FillAsync("Test User");

        await page
            .Locator("#email")
            .FillAsync("test@gmail.com");

        await page
            .Locator("#review")
            .FillAsync("This is a very good product.");

        // Submit review
        await page
            .Locator("#button-review")
            .ClickAsync();

        // Verify success message
        await page
            .GetByText("Thank you for your review.")
            .WaitForAsync();

        // Screenshot
        await page.ScreenshotAsync(new()
        {
            Path = @"C:\Users\meita\PlaywrightTests\screenshots\add-review-product-test.png"
        });

        Console.WriteLine("Add review on product test passed successfully");
    }
}