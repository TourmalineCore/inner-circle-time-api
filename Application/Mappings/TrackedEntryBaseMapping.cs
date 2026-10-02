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
            .HasColumnType("timestamp without time zone")
            // In time tracking, the accuracy is a minute — seconds are not stored.
            // StartTime and EndTime setters in TrackedEntryBase trim them.
            // PropertyAccessMode.Property forces EF Core to go through the getter and setter
            // instead of accessing the private fields directly.
            // Without it, the trimming will not work.
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder
            .Property(x => x.EndTime)
            .HasColumnType("timestamp without time zone")
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder
            .ToTable(x => x.HasCheckConstraint("ck_entries_type_not_zero", "\"type\" <> 0"));

        builder
            .ToTable(x => x.HasCheckConstraint(
                "ck_entries_end_time_is_greater_than_start_time",
                "\"end_time\" > \"start_time\""));
    }
}
