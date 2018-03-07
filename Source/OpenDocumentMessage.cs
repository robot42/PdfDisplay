namespace PdfDisplay
{
    public class OpenDocumentMessage
    {
        public OpenDocumentMessage(string fileName)
        {
            this.FileName = fileName;
        }

        public string FileName { get; }
    }
}