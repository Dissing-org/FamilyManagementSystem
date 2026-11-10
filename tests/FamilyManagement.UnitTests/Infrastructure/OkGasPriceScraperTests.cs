using FamilyManagement.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace FamilyManagement.UnitTests.Infrastructure;

public class OkGasPriceScraperTests
{
    [Theory]
    [InlineData("<h2>OK Gas Variabelpris</h2><h2>10,63 kr./m3</h2>", 10.63)]
    [InlineData("<span>Aktuel pris: 10.63 kr./m3</span>", 10.63)]
    [InlineData("<p>Pris lige nu: 4,25 kr. pr. m3 ekskl. moms</p>", 4.25)]
    [InlineData("<div>Gaspris: 12,50 kr./m³</div>", 12.50)]
    public void ParsePriceFromHtml_WithValidMatches_ReturnsParsedPrice(string html, decimal expected)
    {
        var price = OkGasPriceScraper.ParsePriceFromHtml(html);
        price.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("<html><body>No gas prices mentioned here</body></html>")]
    public void ParsePriceFromHtml_WithInvalidHtml_ReturnsNull(string html)
    {
        var price = OkGasPriceScraper.ParsePriceFromHtml(html);
        price.Should().BeNull();
    }
}
