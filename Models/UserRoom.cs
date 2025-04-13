namespace CSCE_432_632_Project.Models
{
    public class UserRoom
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; }

        public Role Role { get; set; }
    }
}
