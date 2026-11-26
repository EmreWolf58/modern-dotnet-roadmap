using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using TaskManagement.Api.Model;


namespace TaskManagement.Api.Data.Configurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Email).IsUnique(); //Artık aynı Email adresiyle ikinci bir kullanıcı oluşturulmaya çalışılırsa veritabanı buna izin vermeyecektir.
            builder.HasIndex(x => x.Username).IsUnique();

            builder.Property(x => x.Username).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(200);

            builder.HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@taskmanagement.com"
            });
        }
    }
}
