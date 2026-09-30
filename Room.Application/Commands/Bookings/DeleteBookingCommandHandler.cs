namespace Room.Application.Commands.Bookings;

public class DeleteBookingCommandHandler(IRepository<Booking> repository) : IRequestHandler<DeleteBookingCommand, RoomResponse<Guid>>
{
    public async Task<RoomResponse<Guid>> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var dbBooking = await repository.Get(request.Id, includeRelations: false, cancellationToken);
        if (dbBooking == null)
            return RoomResponse<Guid>.Failure($"Booking with '{request.Id}' identifier could not be found.");

        dbBooking.BookingStatus = BookingStatus.Cancelled.GetDisplayName();
        dbBooking.LastUpdated = DateTime.UtcNow;

        var disabled = await repository.Disable(dbBooking, cancellationToken);
        return disabled
            ? RoomResponse<Guid>.Success(dbBooking.Id)
            : RoomResponse<Guid>.Failure($"Booking with '{request.Id}' identifier could not be disabled.");
    }
}