using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineCafeApi.Authorization;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Permissions;
using NineCafeApi.Models;
using NineCafeApi.Services.Permissions;

[Route("api/[controller]")]
[ApiController]
[HasPermission(Perm.PERMISSION_READ)]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    // GET: api/Permissions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionResponseDto>>> GetPermissions()
    {
        var permissions = await _permissionService.GetAllAsync();
        return Ok(permissions);
    }

    // GET: api/Permissions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PermissionResponseDto>> GetPermission(int id)
    {
        var permission = await _permissionService.GetByIdAsync(id);
        if (permission == null)
        {
            return NotFound($"Permission with ID {id} was not found.");
        }
        return Ok(permission);
    }

    // GET: api/Permissions/module/1
    [HttpGet("module/{moduleId}")]
    public async Task<ActionResult<IEnumerable<PermissionResponseDto>>> GetPermissionsByModule(int moduleId)
    {
        var permissions = await _permissionService.GetByModuleIdAsync(moduleId);
        return Ok(permissions);

    }

    // POST: api/Permissions
    [HttpPost]
    [HasPermission(Perm.PERMISSION_CREATE)]
    public async Task<ActionResult<PermissionResponseDto>> PostPermission([FromBody] CreatePermissionDto dto)
    {
        try
        {
            var createdPermission = await _permissionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPermission), new { id = createdPermission.Id }, createdPermission);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message }); // Invalid ModuleId
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // Duplicate permission name
        }
    }

    // PUT: api/Permissions/5
    [HttpPut("{id}")]
    [HasPermission(Perm.PERMISSION_UPDATE)]
    public async Task<ActionResult<PermissionResponseDto>> PutPermission(int id, [FromBody] CreatePermissionDto dto)
    {
        try
        {
            var updatedPermission = await _permissionService.UpdateAsync(id, dto);
            if (updatedPermission == null)
            {
                return NotFound($"Permission with ID {id} was not found.");
            }
            return Ok(updatedPermission);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// DELETE: api/Permissions/5
    [HttpDelete("{id}")]
    [HasPermission(Perm.PERMISSION_DELETE)]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var success = await _permissionService.DeleteAsync(id);
        if (!success)
        {
            return NotFound($"Permission with ID {id} was not found.");
        }
        return NoContent();
    }
}
