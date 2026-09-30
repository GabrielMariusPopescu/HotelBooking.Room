namespace Room.Application.Queries.Bookings;

public class GetBookingsQuery : IRequest<RoomResponse<IEnumerable<Booking>>>;