using System.ComponentModel.DataAnnotations.Schema;

namespace NineCafeApi.DTOs.Withdrawals
{
    public class WithdrawalResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
    }
}
