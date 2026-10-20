using Core.Features.Tracking.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Mappings;

public class TrackedEntryBaseMapping : IEntityTypeConfiguration<TrackedEntryBase>
{
    public void Configure(EntityTypeBuilder<TrackedEntryBase> builder)
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
            .ToTable(x => x.HasCheckConstraint("ck_entries_type_not_zero", "\"type\" <> 0"));

        builder
            .ToTable(x => x.HasCheckConstraint(
                "ck_entries_end_time_is_greater_than_start_time",
                "\"end_time\" > \"start_time\""
            ));

        builder
            .ToTable(x => x.HasCheckConstraint(
                "ck_entries_time_no_seconds",
                "date_trunc('minute', \"start_time\") = \"start_time\" AND date_trunc('minute', \"end_time\") = \"end_time\""
            ));
    }
}
