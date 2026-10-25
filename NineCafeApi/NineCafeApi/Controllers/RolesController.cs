using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineCafeApi.Authorization;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Roles;
using NineCafeApi.Models;
using NineCafeApi.Services.Roles;

[Route("api/[controller]")]
[ApiController]
[HasPermission(Perm.ROLE_READ)]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET: api/roles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
    {
        var roles = await _roleService.GetRolesAsync();
        return Ok(roles);
    }

    // GET: api/roles/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoleResponseDto>> GetRoleById(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null) 
        {
            return NotFound(new { message = $"Role with ID {id} was not found." });
        }
            
        return Ok(role);
    }

    // POST: api/roles
    [HttpPost]
    [HasPermission(Perm.ROLE_CREATE)]
    public async Task<ActionResult<RoleResponseDto>> CreateRole([FromBody] CreateRoleDto dto)
    {
        try
        {
            var createdRole = await _roleService.CreateRoleAsync(dto);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    // PUT: api/roles/5
    [HttpPut("{id:int}")]
    [HasPermission(Perm.ROLE_UPDATE)]
    public async Task<ActionResult> UpdateRole(int id, [FromBody] CreateRoleDto dto)
    {
        try
        {
            var success = await _roleService.UpdateRoleAsync(dto, id);
            if (!success)
            {
                return NotFound(new { message = $"Role with ID {id} was not found." });
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    // DELETE: api/roles/5
    [HttpDelete("{id:int}")]
    [HasPermission(Perm.ROLE_DELETE)]
    public async Task<ActionResult> DeleteRole(int id)
    {
        try
        {
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Role with ID {id} was not found." });
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/roles/assign-permissions
    [HttpPost("assign-permissions")]
    [HasPermission(Perm.ROLE_ASSIGN_PERMISSION)]
    public async Task<ActionResult> AssignPermissions([FromBody] AssignPermissionsToRoleDto dto)
    {
        var success = await _roleService.AssignPermissionToRoleAsync(dto);
        if (!success)
        {
            return NotFound(new { message = $"Role with ID {dto.RoleId} was not found." });
        }
        return Ok(new { message = "Permissions assigned successfully." });
    }

    // POST: api/roles/1/permissions/5
    [HttpPost("{roleId:int}/permissions/{permissionId:int}")]
    [HasPermission(Perm.ROLE_ASSIGN_PERMISSION)]
    public async Task<ActionResult> AddSinglePermission(int roleId, int permissionId)
    {
        var success = await _roleService.AddSinglePermissionAsync(roleId, permissionId);
        if (!success)
        {
            return NotFound(new { message = "Role or Permission not found." });
        }
        return NoContent();
    }

    // DELETE: api/roles/1/permissions/5
    [HttpDelete("{roleId:int}/permissions/{permissionId:int}")]
    [HasPermission(Perm.ROLE_ASSIGN_PERMISSION)]
    public async Task<ActionResult> RemoveSinglePermission(int roleId, int permissionId)
    {
        var success = await _roleService.RemoveSinglePermissionAsync(roleId, permissionId);
        if (!success)
        {
            return NotFound(new { message = "Role-Permission assignment not found." });
        }
        return NoContent();
    }
}
