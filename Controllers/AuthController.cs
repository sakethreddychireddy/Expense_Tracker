using Expense_Tracker.DTO.AuthDtos;
using Expense_Tracker.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Expense_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly ILogger<ExpenseTrackerController> _logger;

        public AuthController(IAuthServices authServices,ILogger<ExpenseTrackerController> logger)
        {
            _authServices = authServices;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _authServices.UserExistsAsync(registerUserDto.Email))
                return Conflict("Email is already registered.");

            var user = await _authServices.RegisterUserAsync(registerUserDto);

            if (user == null)
                throw new ApplicationException("User registration failed.");

            return CreatedAtAction(nameof(Register), new { id = user.Id }, new
            {
                user.Id,
                user.Email,
                user.Role
            });
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authServices.LoginAsync(loginUserDto);

            if (string.IsNullOrEmpty(result.Token))
                return BadRequest("Invalid username or password.");

            return Ok(result); // returns { Token, RefreshToken }
        }
        [Authorize]
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logoutDto)
        {
            if (string.IsNullOrEmpty(logoutDto.RefreshToken))
                return BadRequest(new { message = "Refresh token is required." });

            // Extract user ID from access token
            var tokenHeader = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenHeader);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == "sub");

            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid access token." });

            int userId = int.Parse(userIdClaim.Value);

            var result = await _authServices.LogoutAsync(logoutDto.UserId, logoutDto.RefreshToken);

            if (!result)
                return NotFound(new { message = "No active refresh token found to revoke." });
            _logger.LogInformation("Revoked refresh token for user {UserId}.", userId);
            return Ok(new { message = "Logout successful. Refresh token revoked." });
        }
    }
}
