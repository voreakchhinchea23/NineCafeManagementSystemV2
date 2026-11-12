using Microsoft.AspNetCore.Mvc;
using NineCafeApi.Authorization;
using NineCafeApi.DTOs.Suppliers;
using NineCafeApi.Services.Suppliers;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Perm.SUPPLIER_READ)]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET: api/suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierResponseDto>>> GetSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        // GET: api/suppliers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SupplierResponseDto>> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = $"Supplier with ID {id} was not found." });
            }
            return Ok(supplier);
        }

        // POST: api/suppliers
        [HttpPost]
        [HasPermission(Perm.SUPPLIER_CREATE)]
        public async Task<ActionResult<SupplierResponseDto>> CreateSupplier([FromBody] SupplierCreateDto dto)
        {
            var createdSupplier = await _supplierService.CreateSupplierAsync(dto);
            return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.Id }, createdSupplier);
        }

        // PUT: api/suppliers/5
        [HttpPut("{id:int}")]
        [HasPermission(Perm.SUPPLIER_UPDATE)]
        public async Task<ActionResult<SupplierResponseDto>> UpdateSupplier(int id, [FromBody] SupplierCreateDto dto)
        {
            var updatedSupplier = await _supplierService.UpdateSupplierAsync(id, dto);
            if (updatedSupplier == null)
            {
                return NotFound(new { message = $"Supplier with ID {id} was not found." });
            }
            return Ok(updatedSupplier);
        }

        // DELETE: api/suppliers/5
        [HttpDelete("{id:int}")]
        [HasPermission(Perm.SUPPLIER_DELETE)]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var success = await _supplierService.DeleteSupplierAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Supplier with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
