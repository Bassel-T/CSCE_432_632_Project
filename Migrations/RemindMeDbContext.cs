using Microsoft.EntityFrameworkCore;

namespace CSCE_432_632_Project.Migrations
{
    public class RemindMeDbContext : DbContext
    {
        // Tables here


        public RemindMeDbContext(DbContextOptions<RemindMeDbContext> options) : base(options)
        {

        }
    }
}
