using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Mappings;

public class TrackingEntryBaseMapping : IEntityTypeConfiguration<TrackingEntryBase>
{
    public void Configure(EntityTypeBuilder<TrackingEntryBase> builder)
    {
        builder
            .Property(x => x.Duration)
            .HasComputedColumnSql("end_time - start_time", stored: true);

        builder
            .Property(x => x.StartTime)
            .HasColumnType("timestamp without time zone");

        builder
            .Property(x => x.EndTime)
            .HasColumnType("timestamp without time zone");

        builder
            .ToTable(x => x.HasCheckConstraint("ck_work_entries_type_not_zero", "\"type\" <> 0"));

        builder
            .ToTable(x => x.HasCheckConstraint(
                "ck_work_entries_end_time_is_greater_than_start_time",
                "\"end_time\" > \"start_time\""));
    }
}
