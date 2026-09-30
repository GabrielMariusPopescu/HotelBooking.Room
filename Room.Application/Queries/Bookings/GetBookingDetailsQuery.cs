namespace Room.Application.Queries.Bookings;

public class GetBookingDetailsQuery(Guid id): IRequest<RoomResponse<Booking>>
{
    public Guid Id { get; set; } = id;
}