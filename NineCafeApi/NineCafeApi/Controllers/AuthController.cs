using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineCafeApi.DTOs.Auth;
using NineCafeApi.Services.Auth;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(result);
        }

        // POST: api/auth/logout
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout([FromServices] ITokenBlacklistService blacklistService)
        {
            var token = Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "")
                .Trim();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Invalid token." });
            }

            // Blacklist the token for 8 hours (until its natural expiration)
            blacklistService.BlacklistToken(token, TimeSpan.FromHours(8));

            return Ok(new { message = "Successfully logged out. Token has been revoked." });
        }
    }
}