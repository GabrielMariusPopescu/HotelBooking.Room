namespace Room.Domain.Models;

public class BookingItem : BaseEntity
{
    public Guid BookingId { get; set; }

    public Booking? Booking { get; set; }

    public Guid RoomId { get; set; }

    public Room? Room { get; set; }

    public decimal PricePerNight { get; set; }

    public int Adults { get; set; }

    public int Children { get; set; }
}