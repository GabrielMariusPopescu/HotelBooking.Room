namespace Room.Application.Queries.Bookings;

public class GetBookingDetailsQuery(Guid id): IRequest<Response<Booking>>
{
    public Guid Id { get; set; } = id;
}