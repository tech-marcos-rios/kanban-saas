using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Configurations;

public class BoardListConfiguration : IEntityTypeConfiguration<BoardList>
{
    public void Configure(EntityTypeBuilder<BoardList> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Title).IsRequired().HasMaxLength(100);
        builder.HasOne(l => l.Board).WithMany().HasForeignKey(l => l.BoardId).OnDelete(DeleteBehavior.Cascade);
    }
}
