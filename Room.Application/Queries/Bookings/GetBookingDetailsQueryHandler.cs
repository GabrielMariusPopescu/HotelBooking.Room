namespace Room.Application.Queries.Bookings;

public class GetBookingDetailsQueryHandler(IRepository<Booking> repository) : IRequestHandler<GetBookingDetailsQuery, Response<Booking>>
{
    public async Task<Response<Booking>> Handle(GetBookingDetailsQuery request, CancellationToken cancellationToken)
    {
        var booking = await repository.Get(request.Id, cancellationToken);
        return booking != null
            ? Response<Booking>.Success(booking)
            : Response<Booking>.Failure($"Booking with '{request.Id}' identifier was not found.");
    }
}