using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Model.TaskModel;

namespace TaskManagement.Api.Data
{
    public class AppDbContext : DbContext // Bizim AppDbContext sınıfımız EF Core'un DbContext sınıfından miras alıyor.
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) //DbContextOptions<AppDbContext>: AppDbContext için EF Core ayarlarını taşır.
        {
            //base(options): DbContextOptions context'in ayarlarını taşır, base(options) bu ayarları EF Core'un temel DbContext sınıfına aktarır.
        }

        public DbSet<TaskModel> Tasks { get; set; }

    }
}
