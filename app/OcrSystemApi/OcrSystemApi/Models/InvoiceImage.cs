using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OcrSystemApi.Models
{
    public class InvoiceImage
    {
        [Key]
        public int ImageID { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }

        [Required, StringLength(255)]
        public string ImageURL { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
