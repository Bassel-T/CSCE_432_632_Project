using CSCE_432_632_Project.Migrations;
using CSCE_432_632_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

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

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestVideo([FromQuery] string deviceId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Mac == deviceId);

            if (user == null)
            {
                return Unauthorized();
            }

            var room = await _context.UserRooms
                .Include(x => x.Room)
                .Where(x => x.UserId == user.Id)
                .Select(x => x.Room)
                .FirstOrDefaultAsync();

            if (room == null)
            {
                return NotFound("Room not found.");
            }

            var now = DateTimeOffset.Now;

            var video = await _context.Videos
                .Where(x => x.RoomId == room.Id && x.ScheduledDate < now)
                .OrderByDescending(x => x.ScheduledDate)
                .FirstOrDefaultAsync();

            if (video == null)
            {
                return NotFound("Video not found");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedVideos", video.Name.ToString());

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Video not found");
            }

            FileResult result = File(System.IO.File.OpenRead(filePath), "video/mp4");

            // MP4 is an assumption!
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "video/mp4", result.FileDownloadName);
        }
    }
}
