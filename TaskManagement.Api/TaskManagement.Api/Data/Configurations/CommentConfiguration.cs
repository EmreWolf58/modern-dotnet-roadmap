using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Api.Model;

namespace TaskManagement.Api.Data.Configurations
{
    public class CommentConfiguration: IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            // Table Mapping
            builder.ToTable("Comments");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Content).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.CreatedDate).HasColumnType("datetime2");

            // Relationships
            builder.HasOne(x => x.Task).WithMany(x => x.Comments).HasForeignKey(x => x.TaskId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
