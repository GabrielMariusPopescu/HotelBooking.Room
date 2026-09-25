namespace Room.Application.Commands.Bookings;

public class UpdateBookingCommandHandler(IRepository<Booking> repository): IRequestHandler<UpdateBookingCommand, Response<Booking>>
{
    public async Task<Response<Booking>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var dbBooking = await repository.Get(request.Id, cancellationToken);
        if (dbBooking == null)
            return Response<Booking>.Failure($"Booking with '{request.Id}' identifier could not be found.");

        dbBooking.CheckIn = request.CheckIn;
        dbBooking.CheckOut = request.CheckOut;
        dbBooking.Guests = request.Guests;
        dbBooking.TotalPrice = request.TotalPrice;
        dbBooking.BookingStatus = request.BookingStatus.GetDisplayName();
        dbBooking.LastUpdated = DateTime.UtcNow;

        var updated = await repository.Update(dbBooking, cancellationToken);
        return updated
            ? Response<Booking>.Success(dbBooking)
            : Response<Booking>.Failure($"Booking with '{request.Id}' identifier cannot be updated.");
    }
}