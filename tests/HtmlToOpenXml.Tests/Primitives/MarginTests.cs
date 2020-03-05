namespace HtmlToOpenXml.Tests.Primitives
{
    using WhichMan.HtmlToOpenXml.Primitives;
    using Xunit;

    /// <summary>
    /// Tests Html margin style attribute.
    /// </summary>
    public class MarginTests
    {
        [Theory]
        [InlineData("25px 50px 75px 100px", 25, 50, 75, 100)]
        [InlineData("25px 50px 75px", 25, 50, 75, 50)]
        [InlineData("25px 50px", 25, 50, 25, 50)]
        [InlineData("25px", 25, 25, 25, 25)]
        public void Parse(string html, int top, int right, int bottom, int left)
        {
            var margin = Margin.Parse(html);


            Assert.True(margin.IsValid);
            Assert.Equal(margin.Top.ValueInPx, top);
            Assert.Equal(margin.Right.ValueInPx, right);
            Assert.Equal(margin.Bottom.ValueInPx, bottom);
            Assert.Equal(margin.Left.ValueInPx, left);

        }

        [Fact]
        public void ParseFloat()
        {
            var margin = Margin.Parse("0 50% 1em .00001pt");


            Assert.True(margin.IsValid);

            Assert.Equal(0, margin.Top.Value);
            Assert.Equal(UnitMetric.Pixel, margin.Top.Type);

            Assert.Equal(50, margin.Right.Value);
            Assert.Equal(UnitMetric.Percent, margin.Right.Type);

            Assert.Equal(1, margin.Bottom.Value);
            Assert.Equal(UnitMetric.EM, margin.Bottom.Type);

            Assert.Equal(.00001, margin.Left.Value);
            Assert.Equal(UnitMetric.Point, margin.Left.Type);
            // but due to conversion: 0 (OpenXml relies mostly on long value, not on float)
            Assert.Equal(0, margin.Left.ValueInPoint);

        }
    }
}