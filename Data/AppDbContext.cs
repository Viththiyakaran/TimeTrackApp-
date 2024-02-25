using Microsoft.EntityFrameworkCore;
using TimeTrackApp.Models;

namespace TimeTrackApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UserModel> User { get; set; }

        public DbSet<TaskModel> Task { get; set; }

        public DbSet<TaskHistoryModel> TaskHistory { get; set; }
    }
}
