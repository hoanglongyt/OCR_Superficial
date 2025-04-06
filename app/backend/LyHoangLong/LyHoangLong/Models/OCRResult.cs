using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OcrSystem.Models
{
    public class OCRResult
    {
        [Key]
        public int OCRID { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }

        [Required]
        public string OCRText { get; set; }

        public float? Confidence { get; set; }

        public DateTime ProcessedAt { get; set; } = DateTime.Now;
    }
}
