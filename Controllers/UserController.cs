using CSCE_432_632_Project.Migrations;
using CSCE_432_632_Project.Models;
using CSCE_432_632_Project.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> CreateUser([FromQuery] string deviceId)
        {
            var existing = _context.Users.FirstOrDefault(x => x.Mac == deviceId);

            if (existing != null)
            {
                return Ok(existing.Id);
            }

            var user = new User()
            {
                Mac = deviceId
            };

            // Add item to database
            var added = await _context.Users.AddAsync(user);

            // Save the changes
            var result = await _context.SaveChangesAsync();

            // Failed to save
            if (result == 0)
            {
                return BadRequest("Failed to create user");
            }

            return Ok(added.Entity.Id);
        }

        [HttpGet("inRoom")]
        public async Task<IActionResult> IsUserInRoom([FromQuery] string deviceId)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(x => x.Mac == deviceId);

            if (existing == null)
            {
                return Ok(UserInRoomState.NO_USER);
            }

            var userRoom = await _context.UserRooms.FirstOrDefaultAsync(x => x.UserId == existing.Id);

            if (userRoom == null)
            {
                return Ok(UserInRoomState.NO_ROOM);
            }

            if (userRoom.Role == Role.CAREGIVER)
            {
                return Ok(UserInRoomState.CAREGIVER);
            }
            else
            {
                return Ok(UserInRoomState.ASSISTED);
            }
        }
    }
}
