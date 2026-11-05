using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Debts
{
    public class DebtCreateDto
    {
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public bool IsPaid { get; set; } = false;
    }
}
