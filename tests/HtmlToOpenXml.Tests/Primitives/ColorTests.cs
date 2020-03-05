using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlToOpenXml.Tests.Primitives
{
    using WhichMan.HtmlToOpenXml.Primitives;
    using Xunit;

    /// <summary>
    /// Tests Html color style attribute.
    /// </summary>
    public class ColorTests
    {
        [Theory]
        [InlineData("", 0, 0, 0, 0d)]
        [InlineData("#F00", 255, 0, 0, 1d)]
        [InlineData("#00FFFF", 0, 255, 255, 1d)]
        [InlineData("red", 255, 0, 0, 1d)]
        [InlineData("rgb(106, 90, 205)", 106, 90, 205, 1d)]
        [InlineData("rgba(106, 90, 205, 0.6)", 106, 90, 205, 0.6d)]
        [InlineData("hsl(248, 53%, 58%)", 106, 91, 205, 1)]
        [InlineData("hsla(9, 100%, 64%, 0.6)", 255, 99, 71, 0.6d)]
        [InlineData("hsl(0, 100%, 50%)", 255, 0, 0, 1)]
        // Percentage not respected that should be maxed out
        [InlineData("hsl(0, 200%, 150%)", 255, 255, 255, 1)]
        // Failure that leads to empty
        [InlineData("rgba(1.06, 90, 205, 0.6)", 0, 0, 0, 0.0d)]
        [InlineData("rgba(a, r, g, b)", 0, 0, 0, 0.0d)]
        public void ParseColor(string htmlColor, byte red, byte green, byte blue, double alpha)
        {
            var color = HtmlColor.Parse(htmlColor);


            Assert.Equal(color.R, red);
            Assert.Equal(color.B, blue);
            Assert.Equal(color.G, green);
            Assert.Equal(color.A, alpha);

        }

        [Fact]
        public void HexColor()
        {
            var color = HtmlColor.FromArgb(255, 0, 0);
            Assert.Equal("FF0000", color.ToHexString());

            color = HtmlColor.FromHsl(0, 248, 0.53, 0.58);
            Assert.Equal("6A5BCD", color.ToHexString());
        }
    }
}
