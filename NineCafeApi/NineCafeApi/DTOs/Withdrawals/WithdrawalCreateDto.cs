using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Withdrawals
{
    public class WithdrawalCreateDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(255)]
        public string Reason { get; set; } = string.Empty;
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
