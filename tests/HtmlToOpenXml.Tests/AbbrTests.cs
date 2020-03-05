using System.Linq;
using Xunit;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HtmlToOpenXml.Tests
{
    using WhichMan.HtmlToOpenXml;

    /// <summary>
    /// Tests acronym, abbreviation and blockquotes.
    /// </summary>
    public class AbbrTests : HtmlConverterTestBase
    {
        [Theory]
        [InlineData(@"<abbr title='National Aeronautics and Space Administration'>NASA</abbr>")]
        [InlineData(@"<acronym title='National Aeronautics and Space Administration'>NASA</acronym>")]
        [InlineData(@"<acronym title='www.nasa.gov'>NASA</acronym>")]
        public void ParseAbbr(string html)
        {
            var elements = converter.Parse(html);
            
                Assert.Equal(1, elements.Count);
                Assert.IsType<Paragraph>(elements[0]);
                Assert.IsType<Run>(elements[0].LastChild);
                Assert.Equal("NASA", elements[0].InnerText);

                var noteRef = elements[0].LastChild.GetFirstChild<FootnoteReference>();
                Assert.NotNull(noteRef);
                Assert.True(noteRef.Id.HasValue);

                Assert.NotNull(mainPart.FootnotesPart);
                Assert.Equal(0, mainPart.FootnotesPart.HyperlinkRelationships.Count());

                var fnotes = mainPart.FootnotesPart.Footnotes.Elements<Footnote>().FirstOrDefault(f => f.Id.Value == noteRef.Id.Value);
                Assert.NotNull(fnotes);
            
        }

        [Theory]
        [InlineData(@"<abbr title='https://en.wikipedia.org/wiki/N A S A '>NASA</abbr>", "https://en.wikipedia.org/wiki/N%20A%20S%20A")]
        [InlineData(@"<abbr title='file://C:\temp\NASA.html'>NASA</abbr>", @"file:///C:/temp/NASA.html")]
        [InlineData(@"<abbr title='\\server01\share\NASA.html'>NASA</abbr>", "file://server01/share/NASA.html")]
        [InlineData(@"<abbr title='ftp://server01/share/NASA.html'>NASA</abbr>", "ftp://server01/share/NASA.html")]
        [InlineData(@"<blockquote cite='https://en.wikipedia.org/wiki/NASA'>NASA</blockquote>", "https://en.wikipedia.org/wiki/NASA")]
        public void ParseWithLinks(string html, string expectedUri)
        {
            var elements = converter.Parse(html);
            
                Assert.Equal(1, elements.Count);
                Assert.IsType<Paragraph>(elements[0]);
                Assert.IsType<Run>(elements[0].LastChild);
                Assert.Equal("NASA", elements[0].InnerText);

                var noteRef = elements[0].LastChild.GetFirstChild<FootnoteReference>();
                Assert.NotNull(noteRef);
                Assert.True(noteRef.Id.HasValue);

                Assert.NotNull(mainPart.FootnotesPart);
                var fnotes = mainPart.FootnotesPart.Footnotes.Elements<Footnote>().FirstOrDefault(f => f.Id.Value == noteRef.Id.Value);
                Assert.NotNull(fnotes);

                var link = fnotes.FirstChild.GetFirstChild<Hyperlink>();
                Assert.NotNull(link);

                var extLink = mainPart.FootnotesPart.HyperlinkRelationships.FirstOrDefault(r => r.Id == link.Id);
                Assert.NotNull(extLink);
                Assert.True(extLink.IsExternal);
                Assert.Equal(extLink.Uri.AbsoluteUri, expectedUri);
            
        }

        [Fact]
        public void ParseDocumentEnd()
        {
            converter.AcronymPosition = AcronymPosition.DocumentEnd;
            var elements = converter.Parse(@"<acronym title='www.nasa.gov'>NASA</acronym>");

            
                var noteRef = elements[0].LastChild.GetFirstChild<EndnoteReference>();
                Assert.NotNull(noteRef);
                Assert.True(noteRef.Id.HasValue);

                Assert.NotNull(mainPart.EndnotesPart);
                var fnotes = mainPart.EndnotesPart.Endnotes.Elements<Endnote>().FirstOrDefault(f => f.Id.Value == noteRef.Id.Value);
                Assert.NotNull(fnotes);
            
        }
    }
}