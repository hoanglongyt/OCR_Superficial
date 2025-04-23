using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OcrSystemApi.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        public DateTime InvoiceDate { get; set; }

        [Required, StringLength(255)]
        public string Vendor { get; set; }

        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public ICollection<InvoiceImage> InvoiceImages { get; set; }
        public ICollection<OCRResult> OCRResults { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
