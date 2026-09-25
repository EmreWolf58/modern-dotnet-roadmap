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
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region TaskModel Config
            modelBuilder.Entity<TaskModel>().Property(x => x.Title).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<TaskModel>().Property(x => x.Description).HasMaxLength(1000);
            modelBuilder.Entity<TaskModel>().Property(x => x.CreatedDate).HasColumnName("CreatedAt").HasColumnType("datetime2");
            #endregion
            #region User Config
            modelBuilder.Entity<User>().Property(x => x.Username).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<User>().Property(x => x.Email).IsRequired().HasMaxLength(200);
            #endregion
            #region Comment Config
            modelBuilder.Entity<Comment>().Property(x => x.Content).IsRequired().HasMaxLength(1000);
            modelBuilder.Entity<Comment>().Property(x => x.CreatedDate).HasColumnType("datetime2");
            #endregion
            #region Tag Config
            modelBuilder.Entity<Tag>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            #endregion


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
