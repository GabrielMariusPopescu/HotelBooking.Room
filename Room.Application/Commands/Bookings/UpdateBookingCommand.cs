namespace Room.Application.Commands.Bookings;

public class UpdateBookingCommand(
    Guid id,
    DateOnly checkIn,
    DateOnly checkOut,
    int guests,
    decimal totalPrice,
    BookingStatus bookingStatus) : IRequest<Response<Booking>>
{
    public Guid Id { get; set; } = id;
    
    public DateOnly CheckIn { get; set; } = checkIn;

    public DateOnly CheckOut { get; set; } = checkOut;

    public int Guests { get; set; } = guests;

    public decimal TotalPrice { get; set; } = totalPrice;

    public BookingStatus BookingStatus { get; set; } = bookingStatus;
}