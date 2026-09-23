using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Model;
using TaskManagement.Api.Model.TaskModels;

namespace TaskManagement.Api.Data
{
    public class AppDbContext : DbContext // Bizim AppDbContext sınıfımız EF Core'un DbContext sınıfından miras alıyor.
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) //DbContextOptions<AppDbContext>: AppDbContext için EF Core ayarlarını taşır.
        {
            //base(options): DbContextOptions context'in ayarlarını taşır, base(options) bu ayarları EF Core'un temel DbContext sınıfına aktarır.
        }

        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskModel>().HasData(new TaskModel
            {
                Id=1,
                Title = "Master EF Core",
                Description = "Learn Entity Framework Core basics",
                IsCompleted = false,
                CreatedDate = new DateTime(2026, 9, 22),
                IsDeleted = false,
                DeletedDate = null,
                Priority = 1,
                UserId = 1
            },
            new TaskModel
            {
                Id = 2,
                Title = "Learn Migrations",
                Description = "Practice EF Core migrations",
                IsCompleted = false,
                CreatedDate = new DateTime(2026, 9, 22),
                IsDeleted = false,
                DeletedDate = null,
                Priority = 2,
                UserId = 1
            });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@taskmanagement.com"
            });
        }
    }
}
