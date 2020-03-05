namespace HtmlToOpenXml.Tests
{
    using System;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;
    using WhichMan.HtmlToOpenXml;

    public abstract class HtmlConverterTestBase : IDisposable
    {
        private System.IO.MemoryStream generatedDocument;
        private WordprocessingDocument package;

        protected HtmlConverter converter;
        protected MainDocumentPart mainPart;


        public HtmlConverterTestBase()
        {
            generatedDocument = new System.IO.MemoryStream();
            package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document);

            mainPart = package.MainDocumentPart;
            if (mainPart == null)
            {
                mainPart = package.AddMainDocumentPart();
                new Document(new Body()).Save(mainPart);
            }

            this.converter = new HtmlConverter(mainPart);
        }

        public void Dispose()
        {
            package?.Dispose();
            generatedDocument?.Dispose();
        }
    }
}