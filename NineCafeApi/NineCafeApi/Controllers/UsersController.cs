using Microsoft.AspNetCore.Mvc;
using NineCafeApi.Authorization;
using NineCafeApi.DTOs.Users;
using NineCafeApi.Services.Users;
using System.Security.Claims;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Perm.USER_READ)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // GET: api/users/c3b2f518-8a21-4f12-9c1a-7b3b4f98d123
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} was not found." });
            }
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        [HasPermission(Perm.USER_CREATE)]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
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

        // PUT: api/users/c3b2f518-8a21-4f12-9c1a-7b3b4f98d123
        [HttpPut("{id:guid}")]
        [HasPermission(Perm.USER_UPDATE)]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var success = await _userService.UpdateUserAsync(id, dto);
                if (!success)
                {
                    return NotFound(new { message = $"User with ID {id} was not found." });
                }
                return NoContent();
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

        // DELETE: api/users/c3b2f518-8a21-4f12-9c1a-7b3b4f98d123
        [HttpDelete("{id:guid}")]
        [HasPermission(Perm.USER_DELETE)]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            // Prevent self-deletion if authenticated
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(currentUserIdClaim, out Guid currentUserId) && currentUserId == id)
            {
                return BadRequest(new { message = "Action denied: You cannot delete your own active account." });
            }

            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"User with ID {id} was not found." });
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/users/c3b2f518-8a21-4f12-9c1a-7b3b4f98d123/change-password
        [HttpPut("{id:guid}/change-password")]
        [HasPermission(Perm.USER_CHANGE_PASSWORD)]
        public async Task<ActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
        {
            try
            {
                var success = await _userService.ChangePasswordAsync(id, dto);
                if (!success)
                {
                    return NotFound(new { message = $"User with ID {id} was not found." });
                }
                return Ok(new { message = "Password updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
