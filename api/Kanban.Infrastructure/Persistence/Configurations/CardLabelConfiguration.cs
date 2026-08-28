using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Configurations;

public class CardLabelConfiguration : IEntityTypeConfiguration<CardLabel>
{
    public void Configure(EntityTypeBuilder<CardLabel> builder)
    {
        builder.HasKey(cl => new { cl.CardId, cl.LabelId });

        builder.HasOne(cl => cl.Card).WithMany().HasForeignKey(cl => cl.CardId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(cl => cl.Label).WithMany().HasForeignKey(cl => cl.LabelId).OnDelete(DeleteBehavior.Cascade);
    }
}
