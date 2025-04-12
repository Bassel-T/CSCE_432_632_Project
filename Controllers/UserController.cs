using CSCE_432_632_Project.Migrations;
using CSCE_432_632_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSCE_432_632_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly RemindMeDbContext _context;

        public UserController(ILogger<UserController> logger, RemindMeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var user = new User();

            // Add item to database
            var added = _context.Users.Add(user);

            // Save the changes
            var result = await _context.SaveChangesAsync();

            // Failed to save
            if (result == 0)
            {
                return BadRequest("Failed to create user");
            }

            return Ok(added.Entity.Id);
        }
    }
}
