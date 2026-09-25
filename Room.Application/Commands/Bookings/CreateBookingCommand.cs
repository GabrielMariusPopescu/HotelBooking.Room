namespace Room.Application.Commands.Bookings;

public class CreateBookingCommand(
    DateOnly checkIn,
    DateOnly checkOut,
    int guests,
    IEnumerable<Guid> roomIds) : IRequest<Response<Booking>>
{
    public DateOnly CheckIn { get; set; } = checkIn;

    public DateOnly CheckOut { get; set; } = checkOut;

    public int Guests { get; set; } = guests;

    public IEnumerable<Guid> RoomIds { get; set; } = roomIds;
}