using CSCE_432_632_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace CSCE_432_632_Project.Migrations
{
    public class RemindMeDbContext : DbContext
    {
        // Tables here
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<Video> Videos { get; set; }

        public RemindMeDbContext(DbContextOptions<RemindMeDbContext> options, ILoggerFactory loggerFactory) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the UserRoom table
            modelBuilder.Entity<UserRoom>().HasKey(ur => new { ur.UserId, ur.RoomId });
            modelBuilder.Entity<UserRoom>().HasOne(ur => ur.User);
            modelBuilder.Entity<UserRoom>().HasOne(ur => ur.Room);
            modelBuilder.Entity<UserRoom>().Property(x => x.Role).HasConversion<string>();

            // Configure the Video table
            modelBuilder.Entity<Video>().HasOne(v => v.Room);
        }
    }
}
