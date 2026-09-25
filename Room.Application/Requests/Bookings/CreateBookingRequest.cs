namespace Room.Application.Requests.Bookings;

public class CreateBookingRequest
{
    public DateOnly CheckIn { get; set; }

    public DateOnly CheckOut { get; set; }

    public int Guests { get; set; }

    public IEnumerable<Guid> RoomIds { get; set; } = Enumerable.Empty<Guid>();
}