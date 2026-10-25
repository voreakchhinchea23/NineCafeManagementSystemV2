using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Roles
{
    public class AssignPermissionsToRoleDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "RoleId must be greater than 0.")]
        public int RoleId { get; set; }
        [Required]
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}

