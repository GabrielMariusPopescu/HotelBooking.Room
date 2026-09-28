namespace Room.Application.Requests.Bookings;

public class UpdateBookingRequest
{
    public required Guid Id { get; set; }
    
    public DateOnly CheckIn { get; set; }

    public DateOnly CheckOut { get; set; }

    public int Guests { get; set; }

    public decimal TotalPrice { get; set; }

    public required string BookingStatus { get; set; }
}