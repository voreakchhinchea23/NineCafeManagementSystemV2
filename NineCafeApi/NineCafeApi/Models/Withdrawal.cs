using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineCafeApi.Models
{
    public class Withdrawal
    {
        public int Id { get; set; }
        [Required]
        [Column(TypeName ="decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(255)]
        public string Reason { get; set; } = string.Empty;
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
