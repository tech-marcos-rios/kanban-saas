using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasColumnType("text");

        builder.HasOne(c => c.List).WithMany().HasForeignKey(c => c.ListId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.AssignedUser).WithMany().HasForeignKey(c => c.AssignedUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
