namespace HtmlToOpenXml.Tests.ImageFormats
{
    using Utilities;
    using WhichMan.HtmlToOpenXml.Primitives;
    using WhichMan.HtmlToOpenXml.Utilities.Imaging;
    using Xunit;

    /// <summary>
    /// Tests acronym, abbreviation and blockquotes.
    /// </summary>
    public class ImageHeaderTests
    {
		[Theory]
        [InlineData("Resources.html2openxml.bmp")]
        [InlineData("Resources.html2openxml.gif")]
        [InlineData("Resources.html2openxml.jpg")]
        [InlineData("Resources.html2openxml.png")]
        [InlineData("Resources.html2openxml.emf")]
        public void ReadSize(string resourceName)
        {
            using (var imageStream = ResourceHelper.GetStream(resourceName))
            {
                Size size = ImageHeader.GetDimensions(imageStream);
                Assert.Equal(100, size.Width);
                Assert.Equal(100, size.Height);
            }
        }

        [Fact]
        public void ReadSizeAnimatedGif()
        {
            using (var imageStream = ResourceHelper.GetStream("Resources.stan.gif"))
            {
                Size size = ImageHeader.GetDimensions(imageStream);
                Assert.Equal(252, size.Width);
                Assert.Equal(318, size.Height);
            }
        }

        /// <summary>
        /// This test case cover PNG file where the dimension stands in the 2nd frame
        /// (SOF2 progressive DCT, a contrario of SOF1 baseline DCT).
        /// </summary>
        /// <remarks>https://github.com/onizet/html2openxml/issues/40</remarks>
        [Fact]
        public void ReadSizePngSof2()
        {
            using (var imageStream = ResourceHelper.GetStream("Resources.lumileds.png"))
            {
                Size size = ImageHeader.GetDimensions(imageStream);
                Assert.Equal(500, size.Width);
                Assert.Equal(500, size.Height);
            }
        }

        [Fact]
        public void ReadSizeEmf()
        {
            using (var imageStream = ResourceHelper.GetStream("Resources.html2openxml.emf"))
            {
                Size size = ImageHeader.GetDimensions(imageStream);
                Assert.Equal(100, size.Width);
                Assert.Equal(100, size.Height);
            }
        }
    }
}