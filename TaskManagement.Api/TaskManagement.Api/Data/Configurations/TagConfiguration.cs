using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Api.Model;

namespace TaskManagement.Api.Data.Configurations
{
    public class TagConfiguration: IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            // Table Mapping
            builder.ToTable("Tags");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Indexes
            builder.HasIndex(x => x.Name).IsUnique();

            // Properties
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        }
    }
}
