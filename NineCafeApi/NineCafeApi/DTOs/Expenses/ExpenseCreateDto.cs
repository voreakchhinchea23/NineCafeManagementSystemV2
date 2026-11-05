using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Expenses
{
    public class ExpenseCreateDto
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Other { get; set; }

        [Required]
        public Guid UserPublicId { get; set; }
    }
}
