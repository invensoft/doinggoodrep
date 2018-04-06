namespace XBSPDF
{
    using System.Collections.Generic;
    using XBSPDF.BO;
    using System.Linq;
    using iTextSharp.text.pdf;
    using System.IO;
    using System.ComponentModel.DataAnnotations;
    using iTextSharp.text;

    public class XbsPdfWriter
    {
        private List<XbsPdfParameter> XbsPdfParameters { get; set; }

        private List<XbsPdfParameterGroup> XbsPdfParameterGroups { get; set; }

        [Required]
        public string OutPutPath { get; set; }

        public XbsPdfWriter()
        {
            this.XbsPdfParameters = new List<XbsPdfParameter>();
        }

        public XbsPdfWriter(List<XbsPdfParameter> xbsPdfParameter)
        {
            this.XbsPdfParameters = xbsPdfParameter;
        }

        public XbsPdfWriter(XbsPdfParameter xbsPdfParameter)
        {
            this.XbsPdfParameters.Add(xbsPdfParameter);
        }

        public int Count
        {
            get
            {
                return XbsPdfParameters.Count;
            }
        }

        public void CreatePdf()
        {
            if (!IsValidPath())
            {
                throw new DirectoryNotFoundException("Invalid output path");
            }

            // converting data to ours desired format.
            var Result0 = MakeGroup();

            // group by warrant 
            var Result1 = Result0.GroupBy(u => u.WarrantNumber).Select(s => s.ToList()).ToList();

            var OutPutPdfFile = string.Empty;
            var TempOutPutPdfFile = string.Empty;
            bool IsPdfExist = false;

            FileStream fs = null;

            // loop i for each warrant 
            foreach (var i in Result1)
            {
                //var ParentOutDirName = OutPutPath + i.FirstOrDefault().DelivaryLocation;
                var ParentOutDirName = OutPutPath; // Delivary location is not required as discussed with Ramya.

                if (!Directory.Exists(ParentOutDirName))
                {
                    Directory.CreateDirectory(ParentOutDirName);
                }

                OutPutPdfFile = ParentOutDirName + "\\" + i.FirstOrDefault().WarrantNumber + ".pdf";
                TempOutPutPdfFile = CreateTempPathFile(OutPutPdfFile);

                var writer = CreateWriter(OutPutPdfFile, fs, out IsPdfExist);

                // loop j, object inside each warrant in i 
                foreach (var j in i)
                {
                    using (PdfReader Reader = new PdfReader(j.PdfFileName))
                    {
                        Reader.ConsolidateNamedDestinations();

                        Reader.SelectPages(j.PageNumbers);

                        writer.AddPages(Reader);

                        Reader.Close();
                    }
                }

                writer.Close();

                // if pdf file exist, just rename the file.
                if (IsPdfExist)
                    System.IO.File.Move(TempOutPutPdfFile, OutPutPdfFile);

                // if the pdf file is empty, delete it.
                XbsPdfUtility.CheckFileIsEmpty(OutPutPdfFile, fs);

            }
        }

        private string CreateTempPathFile(string originalPdfFile)
        {
            // creating a temporary file path.
            string Directory = Path.GetDirectoryName(originalPdfFile);
            string FileName = Path.GetFileName(originalPdfFile);
            string FileNameWithoutExt = Path.GetFileNameWithoutExtension(originalPdfFile);
            string FullPath = System.IO.Path.Combine(Directory, FileNameWithoutExt);
            string TempPdfFile = FullPath + "_temp.pdf";
            // creating temporary file path end.

            return TempPdfFile;
        }

        private PdfConcatenate CreateWriter(string outPutPdfFile, FileStream fs, out bool isPdfFileExist)
        {
            PdfConcatenate Writer = null;

            if (File.Exists(outPutPdfFile))
            {
                isPdfFileExist = true;
                Writer = ReadExistingPdfAndCreateWriter(outPutPdfFile, fs);
            }
            else
            {
                isPdfFileExist = false;
                fs = new FileStream(outPutPdfFile, FileMode.Create, FileAccess.ReadWrite);
                Writer = new PdfConcatenate(fs);
                Writer.Open();
            }

            return Writer;
        }

        // append one pdf to another at runtime.
        // rename the temporary pdf file wont work.
        private PdfConcatenate ReadExistingPdfAndCreateWriter(string outPutPdfFile, FileStream fsOriginalPdfFile)
        {
            string OriginalPdfFile = outPutPdfFile;

            // creating a temporary file path.
            string TempPdfFile = CreateTempPathFile(OriginalPdfFile);

            PdfReader OriginalFileReader = new PdfReader(OriginalPdfFile);
            OriginalFileReader.ConsolidateNamedDestinations();

            var Pages = new List<int>();

            // create a temporary .pdf file and open it.
            FileStream fsTempPdfFile = new FileStream(TempPdfFile, FileMode.Create, FileAccess.ReadWrite);
            PdfConcatenate Writer = new PdfConcatenate(fsTempPdfFile);
            Writer.Open();

            // reading all page number available in original pdf file
            for (int i = 1; i <= OriginalFileReader.NumberOfPages; i++)
            {
                Pages.Add(i);
            }

            // select all the pages available in original pdf file
            OriginalFileReader.SelectPages(Pages);

            // write all pages into temporary pdf file
            Writer.AddPages(OriginalFileReader);

            OriginalFileReader.Close();
            OriginalFileReader.Dispose();

            // delete the original file.
            File.Delete(OriginalPdfFile);

            return Writer;

        }

        private List<XbsPdfParameterGroup> MakeGroup()
        {
            // group by warrant
            var result1 = XbsPdfParameters.GroupBy(u => u.WarrantNumber).Select(s => s.ToList()).ToList();

            List<int> PageList = null;

            XbsPdfParameterGroups = new List<XbsPdfParameterGroup>();

            foreach (var i in result1)
            {
                // group by pdf file path
                var result2 = i.GroupBy(u => u.PdfFileName).Select(s => s.ToList()).ToList();

                foreach (var j in result2)
                {
                    PageList = new List<int>();
                    var xbsPdfParameterGroup = new XbsPdfParameterGroup();

                    foreach (var k in j)
                    {
                        PageList.Add(k.PageNumbers);
                    }

                    xbsPdfParameterGroup.PageNumbers = PageList;
                    xbsPdfParameterGroup.PdfFileName = j.FirstOrDefault().PdfFileName;
                    xbsPdfParameterGroup.WarrantNumber = j.FirstOrDefault().WarrantNumber;

                    XbsPdfParameterGroups.Add(xbsPdfParameterGroup);
                }
            }

            return XbsPdfParameterGroups;
        }

        // Checking valid output path.
        private bool IsValidPath()
        {
            DirectoryInfo dir = new DirectoryInfo(OutPutPath);

            if (dir.Exists)
                return true;
            else
                return false;
        }
    }
}
