using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Api.DTOS.AuthControllerDTOS;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        public AuthController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login(LoginRequestDto loginRequestDto)
        {
            if (loginRequestDto.Username != "admin" || loginRequestDto.Password != "123456")
            {
                return Unauthorized("Kullanıcı adı veya şifre hatalı");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, loginRequestDto.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, "admin@test.com")
            };

            var token = _jwtService.GenerateToken(claims);

            return Ok(new
            {
                token
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                Id= id,
                Username= username,
                Role= role,
                Email= email
            });

        }

        [Authorize(Roles="Admin")]
        [HttpGet("admin")]
        public IActionResult AdminPanel()
        {
            return Ok("Admin paneline hoş geldiniz!");
        }
        [Authorize(Roles = "User")]
        [HttpGet("user")]
        public IActionResult UserPanel()
        {
            return Ok("User paneline hoş geldiniz!");
        }

        [Authorize(Policy = "OnlyAdmins")]
        [HttpGet("policy")]
        public IActionResult PolicyTest()
        {
            return Ok("Policy başarılı.");
        }
    }
}
