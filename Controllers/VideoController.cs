using CSCE_432_632_Project.Migrations;
using CSCE_432_632_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCE_432_632_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class VideoController : ControllerBase
    {
        private readonly ILogger<VideoController> _logger;
        private readonly RemindMeDbContext _context;

        public VideoController(ILogger<VideoController> logger, RemindMeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateRoom([FromForm] string deviceId, [FromForm] IFormFile video, [FromForm] DateTimeOffset date, [FromForm] TimeSpan time)
        {
            if (video == null || video.Length == 0)
                return BadRequest("No video file received.");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Mac == deviceId);

            if (user == null)
            {
                return Unauthorized();
            }

            var room = await _context.UserRooms.Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (room == null)
            {
                return StatusCode(500, "User's room was not found.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedVideos");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var newName = Guid.NewGuid();

            var filePath = Path.Combine(uploadsFolder, newName.ToString());

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await video.CopyToAsync(stream);
            }

            var current = DateTimeOffset.Now;

            var scheduledTime = new DateTimeOffset(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds, date.Offset);

            if (Path.Exists(filePath))
            {
                await _context.Videos.AddAsync(new Video
                {
                    CreatedDate = current,
                    Name = newName,
                    RoomId = room.RoomId,
                    ScheduledDate = scheduledTime
                });

                var result = await _context.SaveChangesAsync();

                if (result == 0)
                {
                    System.IO.File.Delete(filePath);
                    return StatusCode(500, "Could not save the video to the database.");
                }
            }
            else
            {
                return StatusCode(500, "Could not save the video file.");
            }
            return Ok(new { Message = "Video uploaded successfully", FilePath = filePath });
        }
    }
}
