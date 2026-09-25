namespace Room.Application.Requests.Bookings;

public class UpdateBookingRequest
{
    public DateOnly CheckIn { get; set; }

    public DateOnly CheckOut { get; set; }

    public int Guests { get; set; }

    public decimal TotalPrice { get; set; }

    public required string BookingStatus { get; set; }
}