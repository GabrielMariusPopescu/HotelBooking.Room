namespace Room.Application.Commands.Bookings;

public class CreateBookingCommandHandler(
    IRepository<Booking> bookingRepository,
    IRepository<Domain.Models.Room> roomRepository)
    : IRequestHandler<CreateBookingCommand, RoomResponse<Booking>>
{
    public async Task<RoomResponse<Booking>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var overlappingBookingExists = await OverlappingBookingExists(request, cancellationToken);
        if (overlappingBookingExists)
            return RoomResponse<Booking>.Failure("One or more requested rooms are already booked for the selected dates.");
        
        var rooms = (await roomRepository.Get(cancellationToken))
            .Where(room => request.RoomIds.Contains(room.Id))
            .ToList();
        
        if (rooms.Count != request.RoomIds.Count())
            return RoomResponse<Booking>.Failure("One or more requested rooms do not exist.");
        
        var (totalPrice, bookingItems) = BookItems(request, rooms);

        Booking booking = new(
            request.CheckIn,
            request.CheckOut,
            request.Guests,
            totalPrice,
            BookingStatus.Pending.GetDisplayName())
        {
            BookingItems = bookingItems
        };

        var dbBooking = await bookingRepository.Add(booking, cancellationToken);
        return dbBooking != null
            ? RoomResponse<Booking>.Success(dbBooking)
            : RoomResponse<Booking>.Failure($"Booking could not be created.");
    }

    private async Task<bool> OverlappingBookingExists(CreateBookingCommand request, CancellationToken cancellationToken) =>
        await bookingRepository
            .Any(booking => booking.BookingStatus != BookingStatus.Cancelled.GetDisplayName() &&
                            booking.CheckIn < request.CheckOut &&        
                            booking.CheckOut > request.CheckIn &&        
                            booking.BookingItems.Any(bookingItem => 
                                request.RoomIds.Contains(bookingItem.RoomId)), 
                cancellationToken);

    private static Tuple<decimal,List<BookingItem>> BookItems(CreateBookingCommand request, IEnumerable<Domain.Models.Room> rooms)
    {
        List<BookingItem> bookingItems = [];
        bookingItems
            .AddRange(rooms
                .Select(room => 
                    new BookingItem
                    {
                        RoomId = room.Id, 
                        PricePerNight = room.PricePerNight, 
                        Adults = request.Guests
                    }));

        var stayDurationDays = request.CheckOut.DayNumber - request.CheckIn.DayNumber;
        var totalPrice = bookingItems.Sum(bookingItem => bookingItem.PricePerNight) * stayDurationDays;

        return new Tuple<decimal, List<BookingItem>>(totalPrice, bookingItems);
    }
}