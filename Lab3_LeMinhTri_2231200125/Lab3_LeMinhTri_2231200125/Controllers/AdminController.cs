using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase {
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard() {
            return Ok("Welcome, Admin!");
        }

    }
}
