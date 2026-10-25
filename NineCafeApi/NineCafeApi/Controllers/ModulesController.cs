using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineCafeApi.Models;
using NineCafeApi.Data;
using NineCafeApi.Services.Modules;
using NineCafeApi.DTOs.Modules;
using NineCafeApi.Authorization;

[Route("api/[controller]")]
[ApiController]
[HasPermission(Perm.MODULE_READ)]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _moduleService;
    public ModulesController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    // GET: api/Module
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleResponseDto>>> GetModule()
    {
        var modules = await _moduleService.GetAllAsync();
        return Ok(modules);
    }

    // GET: api/Module/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ModuleResponseDto>> GetModule(int id)
    {
        var module = await _moduleService.GetByIdAsync(id);

        if (module == null)
        {
            return NotFound();
        }

        return Ok(module);
    }

    // PUT: api/Modules/5
    [HttpPut("{id}")]
    [HasPermission(Perm.MODULE_UPDATE)]
    public async Task<ActionResult<ModuleResponseDto>> PutModule(int id, [FromBody] CreateModuleDto dto)
    {
        var updatedModule = await _moduleService.UpdateAsync(id, dto);
        if (updatedModule == null)
        {
            return NotFound($"Module with ID {id} was not found.");
        }
        return Ok(updatedModule);
    }

    // POST: api/Modules
    [HttpPost]
    [HasPermission(Perm.MODULE_CREATE)]
    public async Task<ActionResult<ModuleResponseDto>> PostModule([FromBody] CreateModuleDto dto)
    {
        try
        {
            var createdModule = await _moduleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetModule), 
                new { id = createdModule.Id }, createdModule);
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = $"Module with name '{dto.Name}' already exists." });
        }
    }

    // DELETE: api/Modules/5
    [HttpDelete("{id}")]
    [HasPermission(Perm.MODULE_DELETE)]
    public async Task<IActionResult> DeleteModule(int id)
    {
        var success = await _moduleService.DeleteAsync(id);
        if (!success)
        {
            return NotFound($"Module with ID {id} was not found.");
        }
        return NoContent();
    }
}
