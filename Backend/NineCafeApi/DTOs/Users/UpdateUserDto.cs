using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Users
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9+\-]+$", ErrorMessage = "Phone number can only contain numbers, '+' and '-'.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Phone number must be between 8 and 20 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Role.")]
        public int RoleId { get; set; }
    }
}