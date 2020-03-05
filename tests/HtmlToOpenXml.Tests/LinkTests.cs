using System;
using System.Linq;
using Xunit;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HtmlToOpenXml.Tests
{
    /// <summary>
    /// Tests hyperlink.
    /// </summary>
    public class LinkTests: HtmlConverterTestBase
    {
        [Fact]
        public void ParseLink()
        {
            var elements = converter.Parse(@"<a href=""www.site.com"" title=""Test Tooltip"">Test Caption</a>");
            
            Assert.Equal(1, elements.Count);
            Assert.IsType<Paragraph>(elements[0]);
            Assert.IsType<Hyperlink>(elements[0].FirstChild);
            Assert.IsType<Run>(elements[0].FirstChild.FirstChild);
            Assert.Equal("Test Caption", elements[0].InnerText);

            var hyperlink = (Hyperlink)elements[0].FirstChild;
            Assert.NotNull(hyperlink.Tooltip);
            Assert.Equal("Test Tooltip", hyperlink.Tooltip.Value);

            Assert.NotNull(hyperlink.Id);
            Assert.True(hyperlink.History.Value);
            Assert.True(mainPart.HyperlinkRelationships.Any());

            var extLink = mainPart.HyperlinkRelationships.FirstOrDefault(r => r.Id == hyperlink.Id);
            Assert.NotNull(extLink);
            Assert.True(extLink.IsExternal);
            Assert.Equal("http://www.site.com/", extLink.Uri.AbsoluteUri);

        }

        [Theory]
        [InlineData(@"<a href=""javascript:alert()"">Js</a>")]
        [InlineData(@"<a href=""site.com"">Unknow site</a>")]
        public void ParseInvalidLink(string html)
        {
            // invalid link leads to simple Run with no link

            var elements = converter.Parse(html);

            Assert.Equal(1, elements.Count);
            Assert.IsType<Paragraph>(elements[0]);
            Assert.IsType<Run>(elements[0].FirstChild);
            Assert.IsType<Text>(elements[0].FirstChild.FirstChild);
        }

        [Fact]
        public void ParseTextImageLink()
        {
            var elements = converter.Parse(@"<a href=""www.site.com""><img src=""data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="" alt=""Red dot"" /> Test Caption</a>");

            Assert.IsType<Hyperlink>(elements[0].FirstChild);

            var hyperlink = (Hyperlink)elements[0].FirstChild;
            Assert.Equal(2, hyperlink.ChildElements.Count);
            Assert.IsType<Run>(hyperlink.FirstChild);
            Assert.IsType<Drawing>(hyperlink.FirstChild.FirstChild);
            Assert.Equal(" Test Caption", hyperlink.LastChild.InnerText);

        }

        [Fact]
        public void ParseAnchorLink()
        {
            var elements = converter.Parse(@"<a href=""#anchor1"">Anchor1</a>");

            Assert.Equal(1, elements.Count);
            Assert.IsType<Paragraph>(elements[0]);
            Assert.IsType<Hyperlink>(elements[0].FirstChild);

            var hyperlink = (Hyperlink)elements[0].FirstChild;
            Assert.Null(hyperlink.Id);
            Assert.True(hyperlink.Anchor == "anchor1");


            converter.ExcludeLinkAnchor = true;

            // _top is always present and bypass the previous rule
            elements = converter.Parse(@"<a href=""#_top"">Anchor2</a>");

            var hyperlink2 = (Hyperlink)elements[0].FirstChild;
            Assert.True(hyperlink2.Anchor == "_top");


            // this should generate a Run and not an Hyperlink
            elements = converter.Parse(@"<a href=""#_anchor3"">Anchor3</a>");
            Assert.IsType<Run>(elements[0].FirstChild);

            converter.ExcludeLinkAnchor = false;
        }
    }
}
