namespace Room.Domain.Models;

public class Booking : BaseEntity
{
    public Guid HotelId { get; set; }

    public Guid UserId { get; set; }

    public DateOnly CheckIn { get; set; }

    public DateOnly CheckOut { get; set; }

    public int Guests { get; set; }

    public decimal TotalPrice { get; set; }

    public string BookingStatus { get; set; }

    public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public Booking()
    {
        
    }
    
    public Booking(DateOnly checkIn, DateOnly checkOut, int guests)
    {
        Created = DateTime.UtcNow;
        CheckIn = checkIn;
        CheckOut = checkOut;
        Guests = guests;
    }
}