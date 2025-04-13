namespace CSCE_432_632_Project.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Mac { get; set; } // Unique to each device. Device ID, not actual MAC address
    }
}
