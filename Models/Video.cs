namespace CSCE_432_632_Project.Models
{
    public class Video
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; }

        public Guid Name { get; set; }

        public DateTimeOffset ScheduledDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
