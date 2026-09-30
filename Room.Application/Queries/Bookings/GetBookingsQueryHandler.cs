namespace Room.Application.Queries.Bookings;

public class GetBookingsQueryHandler(IRepository<Booking> repository): IRequestHandler<GetBookingsQuery, RoomResponse<IEnumerable<Booking>>>
{
    public async Task<RoomResponse<IEnumerable<Booking>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = (await repository.Get(cancellationToken)).ToList();
        return bookings.Any()
            ? RoomResponse<IEnumerable<Booking>>.Success(bookings)
            : RoomResponse<IEnumerable<Booking>>.Failure("No bookings found.");
    }
}