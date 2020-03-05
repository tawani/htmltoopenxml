using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlToOpenXml.Tests
{
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Wordprocessing;
    using WhichMan.HtmlToOpenXml.Utilities;
    using Xunit;

    /// <summary>
    /// Tests Bold, Italic, Underline, Strikethrough.
    /// </summary>
    public class ElementTests : HtmlConverterTestBase
    {
        [Theory]
        [InlineData(typeof(Bold), @"<b>Bold</b>")]
        [InlineData(typeof(Bold), @"<strong>Strong</strong>")]
        [InlineData(typeof(Italic), @"<i>Italic</i>")]
        [InlineData(typeof(Italic), @"<em>Italic</em>")]
        [InlineData(typeof(Strike), @"<s>Strike</s>")]
        [InlineData(typeof(Strike), @"<strike>Strike</strike>")]
        [InlineData(typeof(Strike), @"<del>Del</del>")]
        [InlineData(typeof(Underline), @"<u>Underline</u>")]
        [InlineData(typeof(Underline), @"<ins>Inserted</ins>")]
        public void ParseHtmlElements(Type type, string html)
        {
            ParsePhrasing(type, html);
        }

        [Theory]
        [InlineData(@"<sub>Subscript</sub>", VerticalPositionValues.Subscript)]
        [InlineData(@"<sup>Superscript</sup>", VerticalPositionValues.Superscript)]
        public void ParseSubSup(string html, VerticalPositionValues val)
        {
            var textAlign = ParsePhrasing<VerticalTextAlignment>(html);

            Assert.True(textAlign.Val.HasValue);
            Assert.Equal(textAlign.Val.Value, val);

        }

        [Fact]
        public void ParseStyle()
        {
            var elements = converter.Parse(@"<b style=""
font-style:italic;
font-size:12px;
color:red;
text-decoration:underline;
"">bold with italic style</b>");
            Assert.Equal(1, elements.Count);

            Run run = elements[0].GetFirstChild<Run>();
            Assert.NotNull(run);

            RunProperties runProperties = run.GetFirstChild<RunProperties>();

            Assert.NotNull(runProperties);
            Assert.True(runProperties.HasChild<Bold>());
            Assert.True(runProperties.HasChild<Italic>());
            Assert.True(runProperties.HasChild<FontSize>());
            Assert.True(runProperties.HasChild<Underline>());
            Assert.True(runProperties.HasChild<Color>());

        }

        private T ParsePhrasing<T>(string html) where T : OpenXmlElement
        {
            var elements = converter.Parse(html);
            Assert.Equal(1, elements.Count);

            Run run = elements[0].GetFirstChild<Run>();
            Assert.NotNull(run);

            RunProperties runProperties = run.GetFirstChild<RunProperties>();
            Assert.NotNull(runProperties);

            var tag = runProperties.GetFirstChild<T>();
            Assert.NotNull(tag);
            return tag;
        }

        private void ParsePhrasing(Type type, string html)
        {
            var elements = converter.Parse(html);
            Assert.Equal(1, elements.Count);

            Run run = elements[0].GetFirstChild<Run>();
            Assert.NotNull(run);

            RunProperties runProperties = run.GetFirstChild<RunProperties>();
            Assert.NotNull(runProperties);

            var tag = runProperties.FirstChild;
            Assert.NotNull(tag);
            Assert.IsType(type, tag);
        }
    }
}
