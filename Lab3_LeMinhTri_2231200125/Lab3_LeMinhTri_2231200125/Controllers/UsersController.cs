using Lab3_LeMinhTri_2231200125.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db) {
            _db = db;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe() {
            var userFullName = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(new { FullName = userFullName, Role = role });
        }

        [HttpGet]
        [Authorize(Policy ="ActiveUserOnly")]
        public async Task<IActionResult> GetAllActiveUsers() {
            var users = await _db.User.Where(u => u.IsActive && !u.IsDeleted).ToListAsync();
            return Ok(users);
        }
    }
}
