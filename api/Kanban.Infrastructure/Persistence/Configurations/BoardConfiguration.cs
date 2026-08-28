using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
        builder.HasOne(b => b.Owner).WithMany().HasForeignKey(b => b.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.Metadata.FindNavigation(nameof(Board.Members))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
