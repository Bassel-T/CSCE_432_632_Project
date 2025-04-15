using CSCE_432_632_Project.Migrations;
using CSCE_432_632_Project.Models;
using CSCE_432_632_Project.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CSCE_432_632_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly RemindMeDbContext _context;

        public RoomController(ILogger<RoomController> logger, RemindMeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("createRoom")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequestModel request, [FromQuery] string deviceId)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var existing = await _context.Rooms.AnyAsync(x => x.Name == request.RoomName);

            if (existing)
            {
                // Ensure room names are unique
                return Conflict("Room already exists.");
            }

            // Create a new room
            var room = new Room
            {
                Name = request.RoomName,
                Password = request.Password
            };

            _context.Rooms.Add(room);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Mac == deviceId);

            // Create a new userRoom
            var roomMember = new UserRoom
            {
                UserId = user.Id,
                RoomId = room.Id,
                Role = Role.CAREGIVER
            };

            _context.Add(roomMember);

            var result = await _context.SaveChangesAsync();

            // Check if the room was created successfully
            if (result == 0)
            {
                return BadRequest("Failed to create room");
            }

            return Ok(room.Id);
        }

        [HttpPost("joinRoom")]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequestModel request, [FromQuery] string deviceId)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var roomsList = await _context.Rooms.Where(x => x.Name == request.RoomName).ToListAsync(); 
            var room = roomsList.FirstOrDefault(x => x.ValidatePassword(request.Password));

            if (room == null)
            {
                // Ensure room names are unique
                return NotFound("Could not find room");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Mac == deviceId);

            if (user == null)
            {
                return Unauthorized("Could not find user.");
            }

            var existingPair = _context.UserRooms.Any(x => x.UserId == user.Id && x.RoomId == room.Id);

            if (existingPair)
            {
                // Just let them in, they're already in
                return Ok("User is already in the room.");
            }

            var roomMember = new UserRoom
            {
                UserId = user.Id,
                RoomId = room.Id,
                Role = Role.ASSISTED
            };

            _context.Add(roomMember);

            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return BadRequest("Failed to join room");
            }

            return Ok(room.Id);
        }
    }
}
