using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using TaskManagement.Api.Model.TaskModels;

namespace TaskManagement.Api.Data.Configurations
{
    public class TaskConfiguration: IEntityTypeConfiguration<TaskModel>
    {
        public void Configure(EntityTypeBuilder<TaskModel> builder) //EntityTypeBuilder<TaskModel> :TaskModel entity'sinin yapılandırılmasını sağlayan nesnedir.
        {
            builder.ToTable("Tasks");
            builder.HasKey(x => x.Id); //TaskModel entity'sinin Primary Key alanı Id property'sidir.
            builder.HasIndex(x => x.Title);

            builder.HasIndex(x => new // 2 alanlı index oluşturmak için HasIndex metodu kullanılır
            {
                x.UserId,
                x.IsCompleted
            });
            /*
             şöyle yazıyorduk:
                modelBuilder.Entity<TaskModel>()
                    .Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

            Artık doğrudan:
                builder.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

            yazabiliriz.
            Çünkü builder zaten TaskModel entity'sini temsil ediyor.
             */

            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.CreatedDate).HasColumnName("CreatedAt").HasColumnType("datetime2");

            builder.HasData(new TaskModel
            {
                Id = 1,
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
        }
    }
}
