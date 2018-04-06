namespace XBSPDF.BO
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class XbsPdfParameter
    {
        [Required]
        public string WarrantNumber { get; set; }
        
        //[Required]
        //public string HeatNumber { get; set; }
        
        [Required]
        public string PdfFileName { get; set; }
        
        [Required]
        public int PageNumbers { get; set; }

        //[Required]
        //public string DelivaryLocation { get; set; }
    }
  
}
