using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.DTOs.AuthDTOs;
using Lab3_LeMinhTri_2231200125.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext dbContext, IConfiguration configuration) {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto) {
            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null) {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            if (user.IsDeleted || user.IsLocked || !user.IsActive) {
                return Unauthorized(new { Message = "Account is not active or has been locked/deleted." });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (!isPasswordValid) {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var emailExists = await _dbContext.User.AnyAsync(u => u.Email == registerDto.Email);
            if (emailExists) {
                return BadRequest(new { Message = "Email is already in use." });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var activationCode = Guid.NewGuid().ToString();

            var newUser = new User {
                Fullname = registerDto.Fullname,
                Email = registerDto.Email,
                Password = hashedPassword,
                ActiveCode = activationCode,
                IsActive = false,
                IsDeleted = false,
                IsLocked = false,
                CreatedDate = DateTime.Now,
                UserCode = Guid.NewGuid().ToString()
            };

            _dbContext.User.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Registration successful. Please check your email for activation code." });
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateAccount([FromQuery] string code) {
            if (string.IsNullOrEmpty(code)) {
                return BadRequest(new { Message = "Activation code is required." });
            }

            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.ActiveCode == code);

            if (user == null) {
                return NotFound(new { Message = "Invalid or expired activation code." });
            }

            if (user.IsActive) {
                return Ok(new { Message = "Account is already active." });
            }

            user.IsActive = true;
            user.ActiveCode = null; // Clear the code so it can't be used again

            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Account activated successfully. You can now log in." });
        }

        private string GenerateJwtToken(User user) {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Add user ID as a claim
                new Claim(ClaimTypes.Role, user.Role), // Add user role as a claim
                new Claim(ClaimTypes.Name, user.Fullname), // Add full name as a claim

                new Claim("IsActive", user.IsActive.ToString()), //exercise 6
                new Claim("CreatedDate", user.CreatedDate.ToString("o")), // exercise 8
                new Claim("CanManageCategories", user.CanManageCategories.ToString()), // exercise 9
                new Claim("EmailConfirmed", user.EmailConfirmed.ToString()) // exercise 10)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), // Token expires in 30 minutes
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}