using System.Threading;

namespace Room.Application.Commands.Bookings;

public class UpdateBookingCommandHandler(IRepository<Booking> repository): IRequestHandler<UpdateBookingCommand, Response<Booking>>
{
    public async Task<Response<Booking>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var existingBooking = await repository.Get(request.Id, includeRelations: false, cancellationToken);
        if (existingBooking == null)
            return Response<Booking>.Failure($"Booking with '{request.Id}' identifier could not be found.");

        var lockedRoomIds = existingBooking.BookingItems.Select(bookingItem => bookingItem.RoomId);
        var overlappingBookingExists = await OverlappingBookingExists(lockedRoomIds, request, cancellationToken);
        if(overlappingBookingExists)
            return Response<Booking>.Failure("The updated dates conflict with an existing reservation for these rooms.");

        var stayDurationDays = request.CheckOut.DayNumber - request.CheckIn.DayNumber;
        var totalPrice = existingBooking.BookingItems.Sum(bookingItem => bookingItem.PricePerNight) * stayDurationDays;
        
        existingBooking.CheckIn = request.CheckIn;
        existingBooking.CheckOut = request.CheckOut;
        existingBooking.Guests = request.Guests;
        existingBooking.TotalPrice = totalPrice;
        existingBooking.BookingStatus = request.BookingStatus.GetDisplayName();
        existingBooking.LastUpdated = DateTime.UtcNow;

        var updated = await repository.Update(existingBooking, cancellationToken);
        return updated
            ? Response<Booking>.Success(existingBooking)
            : Response<Booking>.Failure($"Booking with '{request.Id}' identifier cannot be updated.");
    }

    private async Task<bool> OverlappingBookingExists(IEnumerable<Guid> lockedRoomIds,
        UpdateBookingCommand request, CancellationToken cancellationToken) 
        => await repository
            .Any(booking => 
                booking.Id != request.Id &&
                                           booking.BookingStatus != BookingStatus.Cancelled.GetDisplayName() &&
                                           booking.CheckIn < request.CheckOut &&
                                           booking.CheckOut > request.CheckIn &&
                                           booking.BookingItems.Any(item => lockedRoomIds.Contains(item.RoomId)), 
                cancellationToken);
}