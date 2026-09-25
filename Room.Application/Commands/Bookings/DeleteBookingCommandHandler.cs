namespace Room.Application.Commands.Bookings;

public class DeleteBookingCommandHandler(IRepository<Booking> repository) : IRequestHandler<DeleteBookingCommand, Response<Guid>>
{
    public async Task<Response<Guid>> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var dbBooking = await repository.Get(request.Id, cancellationToken);
        if (dbBooking == null)
            return Response<Guid>.Failure($"Booking with '{request.Id}' identifier could not be found.");

        dbBooking.BookingStatus = BookingStatus.Cancelled.GetDisplayName();
        dbBooking.LastUpdated = DateTime.UtcNow;

        var disabled = await repository.Disable(dbBooking, cancellationToken);
        return disabled
            ? Response<Guid>.Success(dbBooking.Id)
            : Response<Guid>.Failure($"Booking with '{request.Id}' identifier could not be disabled.");
    }
}