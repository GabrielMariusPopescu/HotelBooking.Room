namespace Room.Persistence.Configurations;

public class BookingItemConfiguration : IEntityTypeConfiguration<BookingItem>
{
    public void Configure(EntityTypeBuilder<BookingItem> builder)
    {
        builder.HasKey(bookingItem => bookingItem.Id);
        builder.Property(bookingItem => bookingItem.PricePerNight)
            .HasPrecision(18, 2);
        builder.HasOne(bookingItem => bookingItem.Booking)
            .WithMany(booking => booking.BookingItems)
            .HasForeignKey(bookingItem => bookingItem.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bookingItem => bookingItem.Room)
            .WithMany()
            .HasForeignKey(bookingItem => bookingItem.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new BookingItem
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                BookingId = Guid.Parse("33333333-3333-3333-3333-333333333333"), 
                RoomId = Guid.Parse("11111111-1111-1111-1111-111111111111"),  
                PricePerNight = 250.00m,
                Adults = 2,
                Children = 0,
                Created = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new BookingItem
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                BookingId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                RoomId = Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                PricePerNight = 150.00m,
                Adults = 2,
                Children = 0,
                Created = new DateTime(2026, 9, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}