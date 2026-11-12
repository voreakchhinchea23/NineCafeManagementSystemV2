using Microsoft.AspNetCore.Mvc;
using NineCafeApi.Authorization;
using NineCafeApi.DTOs.Prices;
using NineCafeApi.Services.Prices;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Perm.PRICE_READ)]
    public class PricesController : ControllerBase
    {
        private readonly IPriceService _priceService;

        public PricesController(IPriceService priceService)
        {
            _priceService = priceService;
        }

        // GET: api/prices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceResponseDto>>> GetPrices()
        {
            var prices = await _priceService.GetAllPricesAsync();
            return Ok(prices);
        }

        // GET: api/prices/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PriceResponseDto>> GetPriceById(int id)
        {
            var price = await _priceService.GetPriceByIdAsync(id);
            if (price == null)
            {
                return NotFound(new { message = $"Price with ID {id} was not found." });
            }
            return Ok(price);
        }

        // POST: api/prices
        [HttpPost]
        [HasPermission(Perm.PRICE_CREATE)]
        public async Task<ActionResult<PriceResponseDto>> CreatePrice([FromBody] CreatePriceDto dto)
        {
            try
            {
                var createdPrice = await _priceService.CreatePriceAsync(dto);
                return CreatedAtAction(nameof(GetPriceById), new { id = createdPrice.Id }, createdPrice);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/prices/5
        [HttpPut("{id:int}")]
        [HasPermission(Perm.PRICE_UPDATE)]
        public async Task<ActionResult<PriceResponseDto>> UpdatePrice(int id, [FromBody] CreatePriceDto dto)
        {
            try
            {
                var updatedPrice = await _priceService.UpdatePriceAsync(id, dto);
                if (updatedPrice == null)
                {
                    return NotFound(new { message = $"Price with ID {id} was not found." });
                }
                return Ok(updatedPrice);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE: api/prices/5
        [HttpDelete("{id:int}")]
        [HasPermission(Perm.PRICE_DELETE)]
        public async Task<IActionResult> DeletePrice(int id)
        {
            var success = await _priceService.DeletePriceAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Price with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
