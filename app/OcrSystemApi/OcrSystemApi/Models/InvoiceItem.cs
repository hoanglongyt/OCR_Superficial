using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OcrSystemApi.Models
{
    public class InvoiceItem
    {
        [Key]
        public int ItemID { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }

        [Required, StringLength(255)]
        public string Description { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
