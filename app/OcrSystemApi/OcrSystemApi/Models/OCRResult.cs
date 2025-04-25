using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OcrSystemApi.Models
{
    public class OCRResult
    {
        [Key]
        public int OCRID { get; set; }

        [ForeignKey("InvoiceImage")]
        public int ImageID { get; set; }
        public InvoiceImage InvoiceImages { get; set; }

        [Required]
        public string OCRText { get; set; }

        public float? Confidence { get; set; }

        public DateTime ProcessedAt { get; set; } = DateTime.Now;
    }
}
