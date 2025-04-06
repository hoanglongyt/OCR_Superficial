using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OcrSystemApi.Models
{
    public class User : IdentityUser<int>
    {
        [Required, StringLength(100)]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Invoice> Invoices { get; set; }
    }
}