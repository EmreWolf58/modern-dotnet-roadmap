using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Api.Model;

namespace TaskManagement.Api.Data.Configurations
{
    public class UserProfileConfiguration: IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            // Table Mapping
            builder.ToTable("UserProfiles");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Bio).HasMaxLength(500);
            builder.Property(x => x.ProfilePictureUrl).HasMaxLength(2048);

            // Relationships
            builder.HasOne(x => x.User).WithOne(x => x.Profile).HasForeignKey<UserProfile>(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
