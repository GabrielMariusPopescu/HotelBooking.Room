namespace Room.Application.Queries.Bookings;

public class GetBookingsQueryHandler(IRepository<Booking> repository): IRequestHandler<GetBookingsQuery, Response<IEnumerable<Booking>>>
{
    public async Task<Response<IEnumerable<Booking>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = (await repository.Get(cancellationToken)).ToList();
        return bookings.Any()
            ? Response<IEnumerable<Booking>>.Success(bookings)
            : Response<IEnumerable<Booking>>.Failure("No bookings found.");
    }
}