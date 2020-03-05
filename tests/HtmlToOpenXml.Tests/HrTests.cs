using Xunit;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HtmlToOpenXml.Tests
{
    /// <summary>
    /// Tests Horizontal Lines.
    /// </summary>
    public class HrTests : HtmlConverterTestBase
    {
        [Fact]
        public void ParseHr ()
        {
            var elements = converter.Parse("<hr>");
            AssertIsHr (elements[0], false);
        }

        [Fact]
        public void ParseHrNoSpacing ()
        {
            // this should not generate a particular spacing
            var elements = converter.Parse("<p style='border-top:1px solid black'>Before</p><hr>");
            
                AssertIsHr (elements[1], false);
            
        }

        [Theory]
        [InlineData("<p style='border-bottom:1px solid black'>Before</p><hr>")]
        [InlineData("<table><tr><td>Cell</td></tr></table><hr>")]
        public void ParseHrWithSpacing (string html)
        {
            var elements = converter.Parse(html);
            
                AssertIsHr (elements[1], true);
            
        }

        private void AssertIsHr (OpenXmlCompositeElement hr, bool expectSpacing)
        {
            
                Assert.Equal(2, hr.ChildElements.Count);
                var props = hr.GetFirstChild<ParagraphProperties>();
                Assert.NotNull(props);

                Assert.Equal(props.ChildElements.Count, expectSpacing? 2:1);
                Assert.NotNull(props.ParagraphBorders);
                Assert.NotNull(props.ParagraphBorders.TopBorder);

                if (expectSpacing)
                    Assert.NotNull(props.SpacingBetweenLines);
            
        }
    }
}