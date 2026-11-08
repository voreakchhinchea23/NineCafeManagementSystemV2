using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [StringLength(30)]
        public string Action { get; set; } = string.Empty; // "create", "update", "delete"

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = string.Empty; // "DailySale", "Withdrawal", "Debt", "Expense", "Price", "Supplier"

        [StringLength(50)]
        public string? EntityId { get; set; }

        public string? BeforeData { get; set; }

        public string? AfterData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
