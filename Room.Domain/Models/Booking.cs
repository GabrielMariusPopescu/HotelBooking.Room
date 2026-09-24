namespace Room.Domain.Models;

public class Booking : BaseEntity
{
    public Guid HotelId { get; set; }

    public Guid UserId { get; set; }

    public DateOnly CheckIn { get; set; }

    public DateOnly CheckOut { get; set; }

    public int Guests { get; set; }

    public decimal TotalPrice { get; set; }

    public required string BookingStatus { get; set; }

    public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
}