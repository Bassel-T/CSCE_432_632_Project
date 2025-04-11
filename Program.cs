using CSCE_432_632_Project.Migrations;
using Microsoft.EntityFrameworkCore;

namespace CSCE_432_632_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<RemindMeDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("RemindMeDbContext"));
            });

            var app = builder.Build();

            RunMigrations(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void RunMigrations(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RemindMeDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
