namespace XBSPDF
{
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System.IO;

    class XbsPdfUtility
    {
        public static void CheckFileIsEmpty(string pdfFilePath, FileStream fs)
        {
            if (File.Exists(pdfFilePath))
            {
                long length = new System.IO.FileInfo(pdfFilePath).Length;

                if (length == 0)
                {
                    Document Document = new Document();

                    PdfWriter Writer = PdfWriter.GetInstance(Document, fs);
                    Document.Open();

                    PdfContentByte cb = Writer.DirectContent;

                    cb.BeginText();
                    string text = "Dummy Text";
                    cb.EndText();

                    Document.Close();
                    fs.Close();

                    DirectoryInfo ParentDir = Directory.GetParent(pdfFilePath);
                    File.Delete(pdfFilePath);

                    if (ParentDir.GetFiles().Length == 0 && ParentDir.GetDirectories().Length == 0)
                    {
                        ParentDir.Delete();
                    }
                }
            }
        }
    }
}
