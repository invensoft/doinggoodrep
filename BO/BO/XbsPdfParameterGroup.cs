namespace XBSPDF.BO
{
    using System.Collections.Generic;

    class XbsPdfParameterGroup
    {
        public string WarrantNumber { get; set; }
        public string PdfFileName { get; set; }
        public List<int> PageNumbers { get; set; }
        //public string DelivaryLocation { get; set; }
    }
}
