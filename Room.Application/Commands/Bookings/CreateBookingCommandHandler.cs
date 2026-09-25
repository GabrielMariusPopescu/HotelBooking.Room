namespace Room.Application.Commands.Bookings;

public class CreateBookingCommandHandler(IRepository<Booking> repository) : IRequestHandler<CreateBookingCommand, Response<Booking>>
{
    public async Task<Response<Booking>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        Booking booking = new(
            request.CheckIn,
            request.CheckOut,
            request.Guests,
            request.TotalPrice,
            request.BookingStatus.GetDisplayName());

        var dbBooking = await repository.Add(booking, cancellationToken);
        return dbBooking != null
            ? Response<Booking>.Success(dbBooking)
            : Response<Booking>.Failure($"Booking could not be created.");
    }
}