using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Configurations;

public class BoardMemberConfiguration : IEntityTypeConfiguration<BoardMember>
{
    public void Configure(EntityTypeBuilder<BoardMember> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasIndex(m => new { m.BoardId, m.UserId }).IsUnique();
        builder.Property(m => m.Role).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(m => m.Board).WithMany(b => b.Members).HasForeignKey(m => m.BoardId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.User).WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
