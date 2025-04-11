using CSCE_432_632_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace CSCE_432_632_Project.Migrations
{
    public class RemindMeDbContext : DbContext
    {
        // Tables here
        public DbSet<User> Users { get; set; }

        public RemindMeDbContext(DbContextOptions<RemindMeDbContext> options, ILoggerFactory loggerFactory) : base(options)
        {
            
        }
    }
}
