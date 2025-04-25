using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OcrSystemApi.Models
{
    public class InvoiceImage
    {
        [Key]
        public int ImageID { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        public User Users { get; set; }
        [Required, StringLength(255)]
        public string ImageURL { get; set; }
        public DateTime UploadedAt { get; set; } // Keep only one declaration
        public ICollection<OCRResult> OCRResults { get; set; }
    }
}
