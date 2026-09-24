namespace Room.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(bookingItem => bookingItem.Id);
        builder.HasIndex(bookingItem => bookingItem.HotelId);
        builder.HasIndex(bookingItem => bookingItem.UserId);
        builder.Property(bookingItem => bookingItem.TotalPrice)
            .HasPrecision(18, 2);

        builder.HasData(
            new Booking
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                HotelId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                UserId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                CheckIn = new DateOnly(2026, 10, 1),
                CheckOut = new DateOnly(2026, 10, 5),
                Guests = 2,
                TotalPrice = 1000.00m,
                BookingStatus = BookingStatus.Pending.GetDisplayName(),
                Created = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Booking
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                HotelId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                UserId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                CheckIn = new DateOnly(2026, 11, 10),
                CheckOut = new DateOnly(2026, 11, 12),
                Guests = 2,
                TotalPrice = 300.00m,
                BookingStatus = BookingStatus.Confirmed.GetDisplayName(),
                Created = new DateTime(2026, 9, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}