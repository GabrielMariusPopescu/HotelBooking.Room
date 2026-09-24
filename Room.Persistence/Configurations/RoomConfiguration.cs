namespace Room.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Domain.Models.Room>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Room> builder)
    {
        builder.HasKey(room => room.Id);
        builder.Property(bookingItem => bookingItem.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.PricePerNight)
            .HasPrecision(18, 2);

        // Seed Dummy Rooms
        builder.HasData(
            new Domain.Models.Room
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Ocean View Suite",
                Number = 101,
                RoomType = RoomType.Standard.GetDisplayName(), 
                RoomStatus = RoomStatus.Available.GetDisplayName(), 
                PricePerNight = 250.00m,
                IsExcluded = false,
                Created = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Domain.Models.Room
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Standard Double",
                Number = 102,
                RoomType = RoomType.Suite.GetDisplayName(),
                RoomStatus = RoomStatus.Available.GetDisplayName(),
                PricePerNight = 150.00m,
                IsExcluded = false,
                Created = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}