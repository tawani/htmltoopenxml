using System;
using System.Linq;
using Xunit;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HtmlToOpenXml.Tests
{
    using WhichMan.HtmlToOpenXml.Utilities;

    /// <summary>
    /// Tests parser with various complex input Html.
    /// </summary>
    public class ParserTests : HtmlConverterTestBase
    {
        [Theory]
        [InlineData("<!--<p>some text</p>-->")]
        [InlineData("<script>$.appendTo('<p>some text</p>', document);</script>")]
        public void ParseIgnore(string html)
        {
            // the inner html shouldn't be interpreted
            var elements = converter.Parse(html);
            Assert.Equal(0, elements.Count);
        }

        [Fact]
        public void ParseUnclosedTag()
        {
            var elements = converter.Parse("<p>some text in <i>italics <b>,bold and italics</p>");
            
                Assert.Equal(1, elements.Count);
                Assert.Equal(3, elements[0].ChildElements.Count);

                var runProperties1 = elements[0].ChildElements[0].GetFirstChild<RunProperties>();
                Assert.Null(runProperties1);

                runProperties1 = elements[0].ChildElements[1].GetFirstChild<RunProperties>();
                Assert.NotNull(runProperties1);
                Assert.True(runProperties1.HasChild<Italic>());
                Assert.False(runProperties1.HasChild<Bold>());

                runProperties1 = elements[0].ChildElements[2].GetFirstChild<RunProperties>();
                Assert.NotNull(runProperties1);
                Assert.True(runProperties1.HasChild<Italic>());
                Assert.True(runProperties1.HasChild<Bold>());
            

            elements = converter.Parse("<p>First paragraph in semi-<i>italics <p>Second paragraph still italic <b>but also in bold</b></p>");
            
                Assert.Equal(2, elements.Count);
                Assert.Equal(2, elements[0].ChildElements.Count);
                Assert.Equal(2, elements[1].ChildElements.Count);

                var runProperties = elements[0].ChildElements[0].GetFirstChild<RunProperties>();
                Assert.Null(runProperties);

                runProperties = elements[0].ChildElements[1].GetFirstChild<RunProperties>();
                Assert.NotNull(runProperties);
                Assert.True(runProperties.HasChild<Italic>());

                runProperties = elements[1].FirstChild.GetFirstChild<RunProperties>();
                Assert.NotNull(runProperties);
                Assert.True(runProperties.HasChild<Italic>());
                Assert.False(runProperties.HasChild<Bold>());

                runProperties = elements[1].ChildElements[1].GetFirstChild<RunProperties>();
                Assert.NotNull(runProperties);
                Assert.True(runProperties.HasChild<Italic>());
                Assert.True(runProperties.HasChild<Bold>());
            

            // this should generate a new paragraph with its own style
            elements = converter.Parse("<p>First paragraph in <i>italics </i><p>Second paragraph not in italic</p>");
            
                Assert.Equal(2, elements.Count);
                Assert.Equal(2, elements[0].ChildElements.Count);
                Assert.Equal(1, elements[1].ChildElements.Count);
                Assert.IsType<Run>(elements[1].FirstChild);

                var runProperties3 = elements[1].FirstChild.GetFirstChild<RunProperties>();
                Assert.Null(runProperties3);
            
        }

        [Theory]
        [InlineData("<p>Some\ntext</p>",1)]
        [InlineData("<p>Some <b>bold\n</b>text</p>",3)]
        [InlineData("\t<p>Some <b>bold\n</b>text</p>",3)]
        [InlineData("  <p>Some text</p> ",1)]
        public void ParseNewline (string html, int count)
        {
            var elements = converter.Parse(html);
            Assert.Equal(count, elements[0].ChildElements.Count);
        }

        [Fact]
        public void ParseDisorderedTable ()
        {
            // table parts should be reordered
            var elements = converter.Parse(@"
<table>
<tbody>
    <tr><td>Body</td></tr>
</tbody>
<thead>
    <tr><td>Header</td></tr>
</thead>
<tfoot>
    <tr><td>Footer</td></tr>
</tfoot>
</table>");

            
                Assert.Equal(1, elements.Count);
                Assert.IsType<Table>(elements[0]);

                var rows = elements[0].Elements<TableRow>();
                Assert.Equal(3, rows.Count());
                Assert.Equal("Header", rows.ElementAt(0).InnerText);
                Assert.Equal("Body", rows.ElementAt(1).InnerText);
                Assert.Equal("Footer", rows.ElementAt(2).InnerText);
            
        }

        [Fact]
        public void ParseNotTag ()
        {
            var elements = converter.Parse(" < b >bold</b>");
            
                Assert.Equal(1, elements.Count);
                Assert.Equal(1, elements[0].ChildElements.Count);
                Assert.Null(elements[0].FirstChild.GetFirstChild<RunProperties>());
            

            elements = converter.Parse(" <3");
            
                Assert.Equal(1, elements.Count);
                Assert.Equal(1, elements[0].ChildElements.Count);
                Assert.Null(elements[0].FirstChild.GetFirstChild<RunProperties>());
            
        }

        [Fact]
        public void ParseNewlineFlow ()
        {
            // the new line should generate a space between "bold" and "text"
            var elements = converter.Parse(" <span>This is a <b>bold\n</b>text</span>");
        }
    }
}