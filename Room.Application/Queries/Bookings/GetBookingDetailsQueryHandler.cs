namespace Room.Application.Queries.Bookings;

public class GetBookingDetailsQueryHandler(IRepository<Booking> repository) : IRequestHandler<GetBookingDetailsQuery, RoomResponse<Booking>>
{
    public async Task<RoomResponse<Booking>> Handle(GetBookingDetailsQuery request, CancellationToken cancellationToken)
    {
        var booking = await repository.Get(request.Id, includeRelations: true, cancellationToken, booking => booking.BookingItems);
        return booking != null
            ? RoomResponse<Booking>.Success(booking)
            : RoomResponse<Booking>.Failure($"Booking with '{request.Id}' identifier was not found.");
    }
}