using Microsoft.AspNetCore.Mvc;
using NineCafeApi.DTOs.Prices;
using NineCafeApi.Services.Prices;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var prices = await _priceService.GetAllAsync();
            return Ok(prices);
        }

        // GET: api/prices/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PriceResponseDto>> GetPriceById(int id)
        {
            var price = await _priceService.GetByIdAsync(id);
            if (price == null)
            {
                return NotFound(new { message = $"Price with ID {id} was not found." });
            }
            return Ok(price);
        }

        // POST: api/prices
        [HttpPost]
        public async Task<ActionResult<PriceResponseDto>> CreatePrice([FromBody] CreatePriceDto dto)
        {
            try
            {
                var createdPrice = await _priceService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetPriceById), new { id = createdPrice.Id }, createdPrice);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/prices/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<PriceResponseDto>> UpdatePrice(int id, [FromBody] CreatePriceDto dto)
        {
            try
            {
                var updatedPrice = await _priceService.UpdateAsync(id, dto);
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
        public async Task<IActionResult> DeletePrice(int id)
        {
            var success = await _priceService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Price with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}